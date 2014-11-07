using Microsoft.Diagnostics.Tracing;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winterdom.Diagnostics.TraceProcessor.Impl {
  public class EventHubEventProcessor : IEventProcessor {
    private IPartitionKeyGenerator keyGenerator;
    private EventHubClient eventHubClient;

    public EventHubEventProcessor(IPartitionKeyGenerator generator) {
      this.keyGenerator = generator;
    }
    public async Task Process(TraceEvent traceEvent) {
      byte[] eventBody = EventToBytes(traceEvent);
      EventData eventData = new EventData(eventBody);
      eventData.PartitionKey = keyGenerator.GetKey(traceEvent);

      var eventHub = GetOrCreateClient();
      await eventHub.SendAsync(eventData);
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
      if ( this.eventHubClient == null ) {
        String connectionString = 
          ConfigurationManager.AppSettings["EtwHubConnectionString"];
        String eventHubName = 
          ConfigurationManager.AppSettings["EtwEventHubName"];
        var factory = 
          MessagingFactory.CreateFromConnectionString(connectionString);
        this.eventHubClient = factory.CreateEventHubClient(eventHubName);
      }
      return this.eventHubClient;
    }
  }
}
