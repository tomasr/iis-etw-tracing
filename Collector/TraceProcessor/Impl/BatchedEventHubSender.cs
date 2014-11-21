using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Winterdom.Diagnostics.TraceProcessor.Impl {
  public class BatchedEventHubSender : IBatchSender {
    private EventHubClient eventHubClient;
    private ISettings settings;
    private BlockingCollection<Batch<EventData>> pendingFlushList;
    private Task flusher;
    private CancellationTokenSource done;
    private ISendNotify notifySink;

    [ImportingConstructor]
    public BatchedEventHubSender(ISettings settings) {
      this.settings = settings;
      this.pendingFlushList = new BlockingCollection<Batch<EventData>>();
      this.done = new CancellationTokenSource();
      this.flusher = new Task(this.Flusher);
      this.flusher.Start();
    }

    public void SetNotify(ISendNotify sink) {
      this.notifySink = sink;
    }

    public void Send(Batch<EventData> batch) {
      this.pendingFlushList.Add(batch);
    } 

    public void Close() {
      // tell our code we're not taking in any more requests
      this.done.Cancel();
      // then wait until all pending batches are flushed
      this.flusher.Wait();
      if ( this.eventHubClient != null && !this.eventHubClient.IsClosed ) {
        this.eventHubClient.Close();
      }
    }

    private void Flusher() {
      var cancellationToken = this.done.Token;
      try {
        var enumerable = this.pendingFlushList.GetConsumingEnumerable(cancellationToken);
        foreach ( var batch in enumerable ) {
          FlushBatch(batch);
        }
      } catch ( OperationCanceledException ) {
        // expected error
        // need get any remaning entries
        foreach ( var batch in this.pendingFlushList ) {
          FlushBatch(batch);
        }
      }
    }

    private void FlushBatch(Batch<EventData> batch) {
      var client = GetOrCreateClient();
      if ( !batch.IsEmpty ) {
        Exception error = null;
        try {
          client.SendBatch(batch.Drain());
        } catch ( Exception ex ) {
          error = ex;
          this.eventHubClient.Abort();
          this.eventHubClient = null;
        }
        if ( this.notifySink != null ) {
          this.notifySink.OnSendComplete(batch, error);
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
