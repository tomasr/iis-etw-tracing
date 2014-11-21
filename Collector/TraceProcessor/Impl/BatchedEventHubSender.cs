using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winterdom.Diagnostics.TraceProcessor.Impl {
  public class BatchedEventHubSender : IBatchSender {
    private EventHubClient eventHubClient;
    private ISettings settings;
    private Task sendInProgress;

    [ImportingConstructor]
    public BatchedEventHubSender(ISettings settings) {
      this.settings = settings;
    }

    public void Send(Batch<EventData> batch) {
      var client = GetOrCreateClient();
      if ( !batch.IsEmpty ) {
        client.SendBatch(batch.Drain());
      }
    } 

    public async Task SendAsync(Batch<EventData> batch) {
      var client = GetOrCreateClient();
      if ( !batch.IsEmpty ) {
        this.sendInProgress = client.SendBatchAsync(batch.Drain());
        await this.sendInProgress;
        this.sendInProgress = null;
      }
    }

    public void Close() {
      if ( this.eventHubClient != null && !this.eventHubClient.IsClosed ) {
        this.eventHubClient.Close();
      }
    }

    public async Task CloseAsync() {
      var task = this.sendInProgress;
      if ( task != null ) {
        await task;
      }
      if ( this.eventHubClient != null && !this.eventHubClient.IsClosed ) {
        await this.eventHubClient.CloseAsync();
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
