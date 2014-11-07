using Microsoft.Diagnostics.Tracing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winterdom.Diagnostics.TraceProcessor.Impl {
  public class TraceSourceProcessor : ITraceSourceProcessor, IObserver<TraceEvent> {
    private IEventProcessor eventProcessor;
    private IDisposable subscription;

    public TraceSourceProcessor(IEventProcessor processor) {
      this.eventProcessor = processor;
    }

    public void Start(IObservable<TraceEvent> eventStream) {
      if ( this.subscription != null ) {
        throw new InvalidOperationException("Already started.");
      }
      this.subscription = eventStream.Subscribe(this);
    }

    public void Stop() {
      if ( this.subscription != null ) {
        this.subscription.Dispose();
        this.subscription = null;
      }
    }


    void IObserver<TraceEvent>.OnCompleted() {
      this.Stop();
    }

    void IObserver<TraceEvent>.OnError(Exception error) {
      // TODO: implement
    }

    void IObserver<TraceEvent>.OnNext(TraceEvent traceEvent) {
      this.eventProcessor.Process(traceEvent);
      // TODO: implement checkpointing
    }
  }
}
