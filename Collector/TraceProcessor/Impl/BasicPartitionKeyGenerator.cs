using Microsoft.Diagnostics.Tracing;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winterdom.Diagnostics.TraceProcessor.Impl {
  /// <summary>
  /// A basic IPartitionKeyGenerator that combines the
  /// ETW provider with the machine where the trace
  /// was generated
  /// </summary>
  [Export(typeof(IPartitionKeyGenerator))]
  public class BasicPartitionKeyGenerator : IPartitionKeyGenerator {
    public String GetKey(TraceEvent traceEvent) {
      return String.Format("{0}-{1}", traceEvent.ProviderGuid, Environment.MachineName);
    }
  }
}
