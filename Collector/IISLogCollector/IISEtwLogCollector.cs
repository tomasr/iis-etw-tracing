using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Session;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Winterdom.Diagnostics.TraceProcessor;
using Winterdom.Diagnostics.Tracing.IISTraceEvent;

namespace IISLogCollector {
  [Export(typeof(ILogCollectorService))]
  public class IISEtwLogCollector : ILogCollectorService, IDisposable {
    public const String SessionName = "iis-etw-collector";
    private TraceEventSession bufferSession;
    private ETWTraceEventSource eventSource;
    private TraceEventParser eventParser;
    private IObservable<TraceEvent> observableStream;
    private ITraceSourceProcessor sourceProcessor;
    private String traceFolder;
    private String currentFilename;
    private EventWaitHandle shutdownEvent;
    private TimeSpan bufferPeriod;

    [ImportingConstructor]
    public IISEtwLogCollector(ITraceSourceProcessor processor, ISettings settings) {
      this.sourceProcessor = processor;
      this.traceFolder = Path.GetTempPath();
      this.bufferPeriod = settings.GetTimeSpan("BufferPeriod", TimeSpan.FromMinutes(5));
      this.shutdownEvent = new EventWaitHandle(false, EventResetMode.ManualReset);
    }

    public void Start() {
      CreateBufferSession();
      // TODO: Configure how long this will take
      Task.Delay(this.bufferPeriod)
          .ContinueWith((task) => CreateEventSourceOnBufferFile());
    }

    public void Stop() {
      this.shutdownEvent.Set();
      if ( this.eventSource != null ) {
        this.eventSource.StopProcessing();
      }
      if ( this.sourceProcessor != null ) {
        this.sourceProcessor.Stop().Wait();
      }
    }

    public void Dispose() {
        this.ReleaseProcessingSession();
      if ( this.bufferSession != null ) {
        this.bufferSession.Dispose();
        this.bufferSession = null;
      }
      this.eventParser = null;
    }

    private void CreateBufferSession() {
      this.currentFilename = Path.Combine(this.traceFolder, Guid.NewGuid() + ".etl");
      this.bufferSession = new TraceEventSession(
        SessionName, this.currentFilename, TraceEventSessionOptions.Create
      );
      this.bufferSession.EnableProvider(IISLogTraceEventParser.ProviderName);
      Trace.WriteLine(String.Format("Starting buffering on: {0}", this.currentFilename));
    }

    private void CreateEventSourceOnBufferFile() {
      String oldFile = SwitchBufferFiles();
      Trace.WriteLine(String.Format("Starting processing on: {0}", oldFile));
      this.eventSource = new ETWTraceEventSource(
        oldFile, TraceEventSourceType.FileOnly
      );
      this.eventParser = new IISLogTraceEventParser(this.eventSource);

      this.observableStream = this.eventParser.ObserveAll();
      this.sourceProcessor.Start(this.observableStream);
      new Task(Process).Start();
    }

    private String SwitchBufferFiles() {
      String oldFile = this.currentFilename;
      this.currentFilename = Path.Combine(this.traceFolder, Guid.NewGuid() + ".etl");
      Trace.WriteLine(String.Format("Switching buffering to: {0}", this.currentFilename));
      this.bufferSession.SetFileName(this.currentFilename);
      return oldFile;
    }

    private void ReleaseProcessingSession() {
      if ( this.eventSource != null ) {
        this.eventSource.Dispose();
        this.eventSource = null;
      }
    }

    private void Process() {
      Stopwatch timer = Stopwatch.StartNew();
      this.eventSource.Process();
      timer.Stop();
      Trace.WriteLine(String.Format("ETL file processed in {0}", timer.Elapsed));

      // release existing event source, if any
      // and then switch to the new buffer collected
      String oldFile = this.eventSource.LogFileName;
      this.ReleaseProcessingSession();
      File.Delete(oldFile);
      
      // if we processed the file before the buffering
      // period is done, it means we're processing events 
      // faster than they are being collected
      // so wait for a while before switching the files again
      TimeSpan wait = this.bufferPeriod - timer.Elapsed;
      if ( wait < TimeSpan.Zero ) {
          wait = TimeSpan.Zero;
      }
      bool stopping = this.shutdownEvent.WaitOne(wait);
      if (!stopping) {
        CreateEventSourceOnBufferFile();
      }
    }
  }
}
