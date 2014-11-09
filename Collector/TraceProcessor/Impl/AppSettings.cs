using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winterdom.Diagnostics.TraceProcessor.Impl {

  [Export(typeof(ISettings))]
  public class AppSettings : ISettings {
    public String GetString(String name, String defaultValue = "") {
      String value = null;
      if ( !TryGetValue(name, out value) ) {
        return defaultValue;
      }
      return value;
    }

    public int GetInt32(String name, int defaultValue = default(int)) {
      String value = null;
      if ( !TryGetValue(name, out value) ) {
        return defaultValue;
      }
      return Convert.ToInt32(value);
    }

    public TimeSpan GetTimeSpan(String name, TimeSpan defaultValue = default(TimeSpan)) {
      String value = null;
      if ( !TryGetValue(name, out value) ) {
        return defaultValue;
      }
      TimeSpan result;
      if ( !TimeSpan.TryParse(value, out result) ) {
        return defaultValue;
      }
      return result;
    }

    private bool TryGetValue(String key, out String value) {
      var appSettings = ConfigurationManager.AppSettings;
      value = appSettings[key];
      return value != null;
    }
  }
}
