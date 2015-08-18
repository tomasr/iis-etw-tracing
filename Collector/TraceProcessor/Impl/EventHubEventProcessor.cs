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
    private IJsonConverter jsonConverter;
    private IBatchSender batchSender;
    private Batch<EventData> currentBatch;

    [ImportingConstructor]
    public EventHubEventProcessor(
          IPartitionKeyGenerator generator,
          ISettings settings,
          IBatchSender batchSender,
          IJsonConverter jsonConverter) {
      this.MaxBatchSize = 1024 * settings.GetInt32("MaxBatchSizeKB", 192);
      this.keyGenerator = generator;
      this.batchSender = batchSender;
      this.jsonConverter = jsonConverter;
      this.currentBatch = Batch<EventData>.Empty(MaxBatchSize);
    }

    public void SetNotify(ISendNotify sink) {
      this.batchSender.SetNotify(sink);
    }

    public Task<TraceEvent> Process(TraceEvent traceEvent) {
      byte[] eventBody = EventToBytes(traceEvent);

      System.IO.File.WriteAllBytes(String.Format(@"c:\temp\etw\{0}.json", Guid.NewGuid()), eventBody);

      EventData eventData = new EventData(eventBody);
      eventData.PartitionKey = keyGenerator.GetKey(traceEvent);

      if ( !currentBatch.TryAdd(eventData, eventBody.Length) ) {
        // this needs to be an atomic operation!
        var batch = Interlocked.Exchange(ref this.currentBatch, Batch<EventData>.Empty(MaxBatchSize));
        this.batchSender.Send(batch);
      }
      return Task.FromResult(traceEvent);
    }

    // TODO: Not needed, remove
    public Task Flush() {
      return Task.FromResult(0);
    }

    public void Dispose() {
      this.batchSender.Close();
    }

    private byte[] EventToBytes(TraceEvent traceEvent) {
      using ( MemoryStream ms = new MemoryStream() ) {
        using ( StreamWriter writer = new StreamWriter(ms, Encoding.UTF8) ) {
          jsonConverter.ToJson(traceEvent, writer);
          writer.Flush();
          return ms.ToArray();
        }
      }
    }
  }
}
