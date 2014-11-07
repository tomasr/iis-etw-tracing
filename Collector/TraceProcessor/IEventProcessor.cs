using Microsoft.Diagnostics.Tracing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winterdom.Diagnostics.TraceProcessor {
  /// <summary>
  /// Processes a single event at a time
  /// </summary>
  public interface IEventProcessor : IDisposable {
    Task<TraceEvent> Process(TraceEvent traceEvent);
    Task Flush();
  }
}
