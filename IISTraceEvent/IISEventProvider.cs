using System;
using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Session;
using Winterdom.Diagnostics.TraceProcessor;
using System.ComponentModel.Composition;

namespace Winterdom.Diagnostics.Tracing.IISTraceEvent {
  [Export(typeof(IEtwEventProvider))]
  public class IISEventProvider : IEtwEventProvider {
    private IISLogTraceEventParser parser;

    public bool IsKernelProvider { get { return false; } }

    public void RegisterParser(TraceEventSource eventSource) {
      this.parser = new IISLogTraceEventParser(eventSource);
    }
    public void EnableProvider(TraceEventSession session) {
      session.EnableProvider(IISLogTraceEventParser.ProviderName);
    }
    public IObservable<TraceEvent> GetEventStream() {
        return parser.ObserveAll();
    }
  }
}
