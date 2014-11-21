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
    private IBatchSender[] availableSenders;
    private IBatchSenderFactory senderFactory;
    private ISendNotify notifySink;
    private int senderCount;
    private int currentSender;

    [ImportingConstructor]
    public MultiBatchSender(ISettings settings, IBatchSenderFactory factory) {
      this.senderFactory = factory;
      this.currentSender = 0;
      this.senderCount = settings.GetInt32("ConcurrentSenders", 5);
      this.availableSenders = new IBatchSender[senderCount];

      InitializeSenders();
    }

    public void SetNotify(ISendNotify sink) {
      this.notifySink = sink;
      foreach ( var sender in availableSenders ) {
        sender.SetNotify(sink);
      }
    }

    public void Send(Batch<EventData> batch) {
      int senderIndex = (this.currentSender + 1) % this.senderCount;
      this.currentSender = senderIndex;
      this.availableSenders[this.currentSender].Send(batch);
    }

    public void Close() {
      foreach ( var sender in availableSenders ) {
        sender.Close();
      }
    }

    /*
    public async Task CloseAsync() {
      // TODO: This will miss some senders
      // if we have some "busy" senders
      Task[] closers = (from sender in availableSenders
                        select sender.CloseAsync()).ToArray();

      await Task.WhenAll(closers);
    }
    */

    private void InitializeSenders() {
      for ( int i = 0; i < senderCount; i++ ) {
        var sender = senderFactory.Create();
        this.availableSenders[i] = sender;
      }
    }
  }
}
