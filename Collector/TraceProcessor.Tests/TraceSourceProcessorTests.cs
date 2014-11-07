using Microsoft.Diagnostics.Tracing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Winterdom.Diagnostics.TraceProcessor;
using Winterdom.Diagnostics.TraceProcessor.Impl;
using Winterdom.Diagnostics.Tracing.IISTraceEvent;

namespace TraceProcessor.Tests {
  [TestClass]
  public class TraceSourceProcessorTests : BaseEtwTraceTests {
    [TestMethod]
    public void ObservesAllEvents() {
      var source = LoadEventSource();
      var parser = new IISLogTraceEventParser(source);
      var observable = parser.Observe(IISLogTraceEventParser.ProviderName, null);

      var counter = new CountingProcessor();
      var sourceProc = new TraceSourceProcessor(counter);

      sourceProc.Start(observable);
      source.Process();
      source.StopProcessing();
      sourceProc.Stop();

      Assert.AreEqual(9, counter.GetCount());
    }


    class CountingProcessor : IEventProcessor {

      private int count = 0;

      public Task Process(TraceEvent traceEvent) {
        Interlocked.Increment(ref count);
        return Task.FromResult(0);
      }

      public int GetCount() {
        return Interlocked.Exchange(ref count, 0);
      }
    }
  }
}
