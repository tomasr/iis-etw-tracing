using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winterdom.Diagnostics.TraceProcessor.Impl {
  [Export(typeof(IBatchSender))]
  public class MultiBatchSender : IBatchSender {
    private BlockingCollection<IBatchSender> availableSenders;
    private IBatchSenderFactory senderFactory;
    private int senderCount;

    [ImportingConstructor]
    public MultiBatchSender(ISettings settings, IBatchSenderFactory factory) {
      this.senderFactory = factory;
      this.availableSenders = new BlockingCollection<IBatchSender>();

      this.senderCount = settings.GetInt32("ConcurrentSenders", 5);
      InitializeSenders();
    }

    public void Send(Batch<EventData> batch) {
      var sender = this.availableSenders.Take();
      try {
        sender.Send(batch);
        this.availableSenders.Add(sender);
      } catch ( Exception ) {
        // if the send fail, asume the sender is not useful anymore
        // and just discard it, then replace it with a new one
        sender.Close();
        sender = this.senderFactory.Create();
        this.availableSenders.Add(sender);
        throw;
      }
    }
    public async Task SendAsync(Batch<EventData> batch) {
      var sender = this.availableSenders.Take();
      try {
        await sender.SendAsync(batch);
        this.availableSenders.Add(sender);
      } catch ( Exception ) {
        // if the send fail, asume the sender is not useful anymore
        // and just discard it, then replace it with a new one
        sender.Close();
        sender = this.senderFactory.Create();
        this.availableSenders.Add(sender);
        throw;
      }
    }

    public void Close() {
      foreach ( var sender in availableSenders ) {
        sender.Close();
      }
    }

    public async Task CloseAsync() {
      // TODO: This will miss some senders
      // if we have some "busy" senders
      Task[] closers = (from sender in availableSenders
                        select sender.CloseAsync()).ToArray();

      await Task.WhenAll(closers);
    }

    private void InitializeSenders() {
      for ( int i = 0; i < senderCount; i++ ) {
        var sender = senderFactory.Create();
        this.availableSenders.Add(sender);
      }
    }
  }
}
