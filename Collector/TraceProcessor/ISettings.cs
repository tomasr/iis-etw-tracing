using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winterdom.Diagnostics.TraceProcessor {
  public interface ISettings {
    String GetString(String name, String defaultValue = "");
    int GetInt32(String name, int defaultValue = default(int));
    TimeSpan GetTimeSpan(String name, TimeSpan defaultValue = default(TimeSpan));
  }
}
