using Microsoft.Diagnostics.Tracing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winterdom.Diagnostics.Tracing.IISTraceProcessor {
  public interface IPartitionKeyGenerator {
    String GetKey(TraceEvent traceEvent);
  }
}
