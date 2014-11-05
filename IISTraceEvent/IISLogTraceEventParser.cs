using Microsoft.Diagnostics.Tracing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winterdom.Diagnostics.Tracing.IISTraceEvent {
  public class IISLogTraceEventParser : TraceEventParser {
    public static readonly Guid ProviderGuid = new Guid("7E8AD27F-B271-4EA2-A783-A47BDE29143B");
    public const String ProviderName = "Microsoft-Windows-IIS-Logging";
    public const int IISLogEventId = 6200;

    public IISLogTraceEventParser(TraceEventSource source)
      : base(source) {
    }

    public event Action<IISLogTraceData> IISLog {
      add { source.RegisterEventTemplate(IISLogTemplate(value)); }
      remove { source.UnregisterEventTemplate(value, IISLogEventId, ProviderGuid); }
    }

    protected override string GetProviderName() {
      return ProviderName;
    }

    private static IISLogTraceData IISLogTemplate(Action<IISLogTraceData> action) {
      return new IISLogTraceData(action, IISLogEventId, 0, "Logs", Guid.Empty, 0, "", ProviderGuid, ProviderName);
    }
    private static TraceEvent[] Templates = null;
    protected override void EnumerateTemplates(
        Func<string, string, EventFilterResponse> eventsToObserve,
        Action<TraceEvent> callback) {

      if ( Templates == null ) {
        Templates = new TraceEvent[] {
          IISLogTemplate(null)
        };
      }

      var templs = Templates;
      foreach ( var e in templs ) {
        callback(e);
      }
    }
  }
}
