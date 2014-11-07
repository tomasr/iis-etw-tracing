using Microsoft.Diagnostics.Tracing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winterdom.Diagnostics.TraceProcessor {
  /// <summary>
  /// Processes an entire TraceSource
  /// </summary>
  public interface ITraceSourceProcessor {
    void Start(IObservable<TraceEvent> eventStream);
    void Stop();
  }
}
