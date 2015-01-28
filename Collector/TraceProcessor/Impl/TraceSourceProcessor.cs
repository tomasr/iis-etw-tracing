using Microsoft.Diagnostics.Tracing;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Winterdom.Diagnostics.TraceProcessor.Impl {
  [Export(typeof(ITraceSourceProcessor))]
  public class TraceSourceProcessor : ITraceSourceProcessor, IObserver<TraceEvent>, ISendNotify {
    private IEventProcessor eventProcessor;
    private IDisposable subscription;
    private long eventCount;
    private long eventsProcessed;
    private object eplock;

    [ImportingConstructor]
    public TraceSourceProcessor(IEventProcessor processor) {
      this.eventProcessor = processor;
      this.eventCount = 0;
      this.eventsProcessed = 0;
      this.eplock = new object();
      processor.SetNotify(this);
    }

    public void Start(IObservable<TraceEvent> eventStream) {
      if ( this.subscription != null ) {
          this.subscription.Dispose();
      }
      this.subscription = eventStream.Subscribe(this);
    }

    public async Task Stop() {
      if ( this.subscription != null ) {
        this.subscription.Dispose();
        this.subscription = null;
      }
      if ( this.eventProcessor != null ) {
        var ep = this.eventProcessor;
        this.eventProcessor = null;
        // ensure all events are processed
        // and after that clean up around the processor
        await ep.Flush();
        ReleaseEventProcessor(ep);
      }
    }

    private void ReleaseEventProcessor(IEventProcessor ep) {
      // TODO: add tracing
      ep.Dispose();
    }

    void IObserver<TraceEvent>.OnCompleted() {
    }

    void ISendNotify.OnSendComplete(Batch<EventData> batch, Exception error) {
      if ( error != null ) {
        Trace.WriteLine(String.Format("Batch send failed: {0}", error));
      } else {
        lock ( this.eplock ) {
          this.eventsProcessed += batch.Count;
          Trace.WriteLine(String.Format("Events flushed: {0}", this.eventsProcessed));
        }
      }
    }

    void IObserver<TraceEvent>.OnError(Exception error) {
      Trace.WriteLine(String.Format("Error observed: {0}", error));
      // TODO: implement
    }

    void IObserver<TraceEvent>.OnNext(TraceEvent traceEvent) {
      this.eventProcessor.Process(traceEvent)
        .ContinueWith(OnEventProcessingComplete);
    }

    private void OnEventProcessingComplete(Task<TraceEvent> task) {
      if ( task.IsFaulted ) {
        // TODO: figure out proper error handling here :)
        Console.WriteLine(task.Exception);
      } else {
        long count = Interlocked.Increment(ref this.eventCount);
        if ( count % 100 == 0 ) {
          Trace.WriteLine(String.Format("Processed {0} events", count));
        }
      }
    }
  }
}
