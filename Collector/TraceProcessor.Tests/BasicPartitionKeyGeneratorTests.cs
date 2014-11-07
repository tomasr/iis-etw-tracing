using Microsoft.Diagnostics.Tracing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Winterdom.Diagnostics.TraceProcessor;
using Winterdom.Diagnostics.TraceProcessor.Impl;
using Winterdom.Diagnostics.Tracing.IISTraceEvent;

namespace TraceProcessor.Tests {
  [TestClass]
  [DeploymentItem("iis.etl")]
  public class BasicPartitionKeyGeneratorTests {

    [TestMethod]
    public void PartitionKeyIncludesTraceProvider() {
      var traceEvent = LoadSampleTrace("iis.etl").First();
      IPartitionKeyGenerator gen = new BasicPartitionKeyGenerator();
      String key = gen.GetKey(traceEvent);
      Assert.IsTrue(key.IndexOf(traceEvent.ProviderGuid.ToString()) >= 0);
    }

    [TestMethod]
    public void PartitionKeyIncludesMachineName() {
      var traceEvent = LoadSampleTrace("iis.etl").First();
      IPartitionKeyGenerator gen = new BasicPartitionKeyGenerator();
      String key = gen.GetKey(traceEvent);
      Assert.IsTrue(key.IndexOf(Environment.MachineName) >= 0);
    }

    private IEnumerable<TraceEvent> LoadSampleTrace(string file) {
      var traceSource = new ETWTraceEventSource(file);
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
