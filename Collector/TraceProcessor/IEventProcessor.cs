using Microsoft.Diagnostics.Tracing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winterdom.Diagnostics.TraceProcessor {
  /// <summary>
  /// Processes a single event at atime
  /// </summary>
  public interface IEventProcessor {
    Task Process(TraceEvent traceEvent);
  }
}
