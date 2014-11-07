using Microsoft.Diagnostics.Tracing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Winterdom.Diagnostics.Tracing.IISTraceEvent;

namespace TraceProcessor.Tests {
  [DeploymentItem(TraceName)]
  public abstract class BaseEtwTraceTests {
    public const String TraceName = "iis.etl";

    protected ETWTraceEventSource LoadEventSource() {
      return new ETWTraceEventSource(TraceName);
    }
    protected IEnumerable<TraceEvent> LoadSampleTrace() {
      var traceSource = LoadEventSource();
      var parser = new IISLogTraceEventParser(traceSource);
      List<TraceEvent> events = new List<TraceEvent>();
      parser.All += e => {
        events.Add(e.Clone());
      };
      traceSource.Process();
      return events;
    }
  }
}
