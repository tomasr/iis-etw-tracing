using Microsoft.Diagnostics.Tracing;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.ComponentModel.Composition;
using System.Diagnostics;

namespace Winterdom.Diagnostics.TraceProcessor.Impl {
  [Export(typeof(IEventProcessor))]
  public class EventHubEventProcessor : IEventProcessor {
    public int MaxBatchSize { get; private set; }
    private IPartitionKeyGenerator keyGenerator;
    private ISettings settings;
    private EventHubClient eventHubClient;
    private Batch<EventData> currentBatch;
    private BlockingCollection<Batch<EventData>> pendingFlushList;
    private Task flusher;
    private CancellationTokenSource done;

    [ImportingConstructor]
    public EventHubEventProcessor(
          IPartitionKeyGenerator generator,
          ISettings settings) {
      this.MaxBatchSize = 1024 * settings.GetInt32("MaxBatchSizeKB", 192);
      this.keyGenerator = generator;
      this.settings = settings;
      this.currentBatch = Batch<EventData>.Empty(MaxBatchSize);
      this.pendingFlushList = new BlockingCollection<Batch<EventData>>();

      this.done = new CancellationTokenSource();
      this.flusher = new Task(this.Flusher);
      this.flusher.Start();
    }

    public Task<TraceEvent> Process(TraceEvent traceEvent) {
      if ( this.done.IsCancellationRequested ) {
        throw new ObjectDisposedException("Cannot add new items when already disposed");
      }

      byte[] eventBody = EventToBytes(traceEvent);

      EventData eventData = new EventData(eventBody);
      eventData.PartitionKey = keyGenerator.GetKey(traceEvent);

      if ( !currentBatch.TryAdd(eventData, eventBody.Length) ) {
        // this needs to be an atomic operation!
        var batch = Interlocked.Exchange(ref this.currentBatch, Batch<EventData>.Empty(MaxBatchSize));
        this.pendingFlushList.Add(batch);
      }
      return Task.FromResult(traceEvent);
    }

    public Task Flush() {
      // flush any batches that haven't reached max size
      var batch = Interlocked.Exchange(ref this.currentBatch, Batch<EventData>.Empty(MaxBatchSize));
      this.pendingFlushList.Add(batch);
      return Task.FromResult(0);
    }

    public void Dispose() {
      // tell our code we're not taking in any more requests
      this.done.Cancel();
      // then wait until all pending batches are flushed
      this.flusher.Wait();
      this.eventHubClient.Close();
    }

    private void FlushBatch(IEnumerable<EventData> events) {
      var eventHub = GetOrCreateClient();
      try {
        eventHub.SendBatch(events);
        Trace.WriteLine(String.Format("Flushed {0} events.", events.Count()));
      } catch ( Exception ex ){
        eventHub.Abort();
        Trace.WriteLine(String.Format("Error On Send: {0}.", ex));
      }
    }

    private void Flusher() {
      var cancellationToken = this.done.Token;
      try {
        var enumerable = this.pendingFlushList.GetConsumingEnumerable(cancellationToken);
        foreach ( var batch in enumerable ) {
          FlushBatch(batch.Drain());
        }
      } catch ( OperationCanceledException ) {
        // expected error
        // need get any remaning entries
        foreach ( var batch in this.pendingFlushList ) {
          FlushBatch(batch.Drain());
        }
      }
    }

    private byte[] EventToBytes(TraceEvent traceEvent) {
      using ( MemoryStream ms = new MemoryStream() ) {
        using ( StreamWriter writer = new StreamWriter(ms, Encoding.UTF8) ) {
          traceEvent.ToJson(writer);
          writer.Flush();
          return ms.ToArray();
        }
      }
    }

    private EventHubClient GetOrCreateClient() {
      if ( this.eventHubClient == null || this.eventHubClient.IsClosed ) {
        String connectionString = this.settings.GetString("EtwHubConnectionString");
        String eventHubName = this.settings.GetString("EtwEventHubName");
        var factory = 
          MessagingFactory.CreateFromConnectionString(connectionString);
        this.eventHubClient = factory.CreateEventHubClient(eventHubName);
      }
      return this.eventHubClient;
    }
  }
}
