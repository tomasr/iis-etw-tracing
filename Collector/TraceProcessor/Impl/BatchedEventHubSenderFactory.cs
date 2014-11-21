using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winterdom.Diagnostics.TraceProcessor.Impl {
  [Export(typeof(IBatchSenderFactory))]
  public class BatchedEventHubSenderFactory : IBatchSenderFactory {
    [Import]
    public ISettings Settings { get; set; }

    public IBatchSender Create() {
      return new BatchedEventHubSender(Settings);
    }
  }
}
