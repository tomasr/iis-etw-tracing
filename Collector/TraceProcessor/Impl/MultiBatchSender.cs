using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winterdom.Diagnostics.TraceProcessor.Impl {
  public class MultiBatchSender {
    private ConcurrentBag<IBatchSender> allSenders;
    private BlockingCollection<IBatchSender> availableSenders;

    public MultiBatchSender(IEnumerable<IBatchSender> senders) {
      this.availableSenders = new BlockingCollection<IBatchSender>();
      this.allSenders = new ConcurrentBag<IBatchSender>();
      foreach ( var sender in senders ) {
        this.availableSenders.Add(sender);
        this.allSenders.Add(sender);
      }
    }

    public async Task SendAsync(Batch<EventData> batch) {
      var sender = this.availableSenders.Take();
      await sender.SendAsync(batch);
    }
  }
}
