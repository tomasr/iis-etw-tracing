using Microsoft.Diagnostics.Tracing;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winterdom.Diagnostics.TraceProcessor.Impl {
  /// <summary>
  /// A basic IPartitionKeyGenerator that just uses the machine name
  /// </summary>
  [Export(typeof(IPartitionKeyGenerator))]
  public class BasicPartitionKeyGenerator : IPartitionKeyGenerator {
    public String GetKey(TraceEvent traceEvent) {
      // Original code combined the Provider GUID with the machine name
      // but you cannot mix events with different partition keys in the same
      // batch.
      return Environment.MachineName;
    }
  }
}
