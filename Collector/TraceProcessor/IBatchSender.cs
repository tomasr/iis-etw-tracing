using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winterdom.Diagnostics.TraceProcessor {
  public interface IBatchSender {
    void Send(Batch<EventData> batch);
    Task SendAsync(Batch<EventData> batch);
    void Close();
    Task CloseAsync();
  }
}
