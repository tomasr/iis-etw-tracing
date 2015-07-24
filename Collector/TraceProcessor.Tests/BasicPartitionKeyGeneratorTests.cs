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
  public class BasicPartitionKeyGeneratorTests : BaseEtwTraceTests {

    [TestMethod]
    public void PartitionKeyIncludesMachineName() {
      var traceEvent = LoadSampleTrace().First();
      IPartitionKeyGenerator gen = new BasicPartitionKeyGenerator();
      String key = gen.GetKey(traceEvent);
      Assert.IsTrue(key.IndexOf(Environment.MachineName) >= 0);
    }

  }
}
