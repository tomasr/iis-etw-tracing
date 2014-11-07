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
        var task = ep.Flush();
        await task;
        ReleaseEventProcessor(ep);
      }
    }

    private void ReleaseEventProcessor(IEventProcessor ep) {
      // TODO: add tracing
      ep.Dispose();
    }


    void IObserver<TraceEvent>.OnCompleted() {
    }

    void IObserver<TraceEvent>.OnError(Exception error) {
      // TODO: implement
    }

    void IObserver<TraceEvent>.OnNext(TraceEvent traceEvent) {
      this.eventProcessor.Process(traceEvent)
        .ContinueWith(OnEventProcessingComplete);
    }

    private void OnEventProcessingComplete(Task<TraceEvent> task) {
      if ( task.IsFaulted ) {
        Console.WriteLine(task.Exception);
      } else {
        Console.WriteLine(task.Result.Dump(true));
      }
      // TODO: figure out something here :)
    }
  }
}
