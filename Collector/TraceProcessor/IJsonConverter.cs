using Microsoft.Diagnostics.Tracing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winterdom.Diagnostics.TraceProcessor {
  public interface IJsonConverter {
    void ToJson(TraceEvent traceEvent, TextWriter writer);
    String ToJson(TraceEvent traceEvent);
  }
}
