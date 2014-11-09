using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Session;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Winterdom.Diagnostics.TraceProcessor;
using Winterdom.Diagnostics.Tracing.IISTraceEvent;

namespace IISLogCollector {
  [Export(typeof(ILogCollectorService))]
  public class IISEtwLogCollector : ILogCollectorService, IDisposable {
    public const String SessionName = "iis-etw-collector";
    private TraceEventSession eventSession;
    private ETWTraceEventSource eventSource;
    private TraceEventParser eventParser;
    private IObservable<TraceEvent> observableStream;
    private ITraceSourceProcessor sourceProcessor;

    [ImportingConstructor]
    public IISEtwLogCollector(ITraceSourceProcessor processor) {
      this.sourceProcessor = processor;
    }

    public void Start() {
      this.eventSession = new TraceEventSession(
        SessionName, TraceEventSessionOptions.Create
      );
      this.eventSession.EnableProvider(IISLogTraceEventParser.ProviderName);
      this.eventSource = new ETWTraceEventSource(
        SessionName, TraceEventSourceType.Session
      );
      this.eventParser = new IISLogTraceEventParser(this.eventSource);

      this.observableStream = this.eventParser.Observe(
        IISLogTraceEventParser.ProviderName, null
      );
      this.sourceProcessor.Start(this.observableStream);
      new Task(Process).Start();
    }

    public void Stop() {
      if ( this.eventSource != null ) {
        this.eventSource.StopProcessing();
      }
      if ( this.sourceProcessor != null ) {
        this.sourceProcessor.Stop().Wait();
      }
    }

    public void Dispose() {
      if ( this.eventSource != null ) {
        this.eventSource.Dispose();
        this.eventSource = null;
      }
      if ( this.eventSession != null ) {
        this.eventSession.Dispose();
        this.eventSession = null;
      }
      this.eventParser = null;
    }

    private void Process() {
      this.eventSource.Process();
    }
  }
}
