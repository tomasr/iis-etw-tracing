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
      sourceProc.Stop().Wait();

      Assert.AreEqual(9, counter.GetCount());
      Assert.IsTrue(counter.FlushCalled);
      Assert.IsTrue(counter.DisposeCalled);
    }


    class CountingProcessor : IEventProcessor {

      private int count = 0;
      public bool FlushCalled { get; private set; }
      public bool DisposeCalled { get; private set; }

      public void SetNotify(ISendNotify sink) {
      }

      public Task<TraceEvent> Process(TraceEvent traceEvent) {
        Interlocked.Increment(ref count);
        return Task.FromResult(traceEvent);
      }

      public int GetCount() {
        return Interlocked.Exchange(ref count, 0);
      }
      public Task Flush() {
        FlushCalled = true;
        return Task.FromResult(0);
      }
      public void Dispose() {
        DisposeCalled = true;
      }
    }
  }
}
