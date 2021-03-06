﻿using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winterdom.Diagnostics.TraceProcessor {
  public interface IBatchSender {
    void SetNotify(ISendNotify sink);
    void Send(Batch<EventData> batch);
    void Close();
  }
}
