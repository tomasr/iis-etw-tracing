using Microsoft.Diagnostics.Tracing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winterdom.Diagnostics.TraceProcessor {
  public static class TraceParserExtensions {
    public static IObservable<TraceEvent> ObserveAll(this TraceEventParser parser) {
      return parser.Observe((providerName, eventName) => {
        return EventFilterResponse.AcceptEvent;
      });
    }
  }
}
