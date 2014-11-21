using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winterdom.Diagnostics.TraceProcessor {
  public interface ISendNotify {
    void OnSendComplete(Batch<EventData> batch, Exception ex);
  }
}
