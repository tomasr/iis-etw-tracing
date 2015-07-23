using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Session;
using System;

namespace Winterdom.Diagnostics.TraceProcessor {
  public interface IEtwEventProvider {
    void RegisterParser(TraceEventSource eventSource);
    void EnableProvider(TraceEventSession session);
  }
}
