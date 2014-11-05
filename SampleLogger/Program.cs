using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Winterdom.Diagnostics.Tracing.IISTraceEvent;

namespace SampleLogger {
  class Program {
    const String SessionName = "iis-etw";

    static void Main(string[] args) {
      // create a new real-time ETW trace session
      using ( var session = new TraceEventSession(SessionName) ) {
        // enable IIS ETW provider and set up a new trace source on it
        session.EnableProvider(IISLogTraceEventParser.ProviderName, TraceEventLevel.Verbose);

        using ( var traceSource = new ETWTraceEventSource(SessionName, TraceEventSourceType.Session) ) {
          Console.WriteLine("Session started, listening for events...");
          var parser = new IISLogTraceEventParser(traceSource);
          parser.IISLog += OnIISRequest;

          traceSource.Process();
          Console.ReadLine();
          traceSource.StopProcessing();
        }
      }
    }
    private static void OnIISRequest(IISLogTraceData request) {
      Console.WriteLine(request.Dump(true));
      Console.WriteLine("*******************");
    }
  }
}
