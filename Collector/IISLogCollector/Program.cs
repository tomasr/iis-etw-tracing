using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Winterdom.Diagnostics.TraceProcessor;
using Winterdom.Diagnostics.TraceProcessor.Impl;

namespace IISLogCollector {
  // TODO: Should be a service!
  class Program {
    [Import]
    public ILogCollectorService CollectorService { get; set; }

    static void Main(string[] args) {
      Type itsType = typeof(ITraceSourceProcessor);
      AggregateCatalog catalog = new AggregateCatalog();
      catalog.Catalogs.Add(new AssemblyCatalog(itsType.Assembly));
      catalog.Catalogs.Add(new AssemblyCatalog(typeof(Program).Assembly));
      CompositionContainer container = new CompositionContainer(catalog);

      Program program = new Program();
      container.SatisfyImportsOnce(program);

      program.Run();
    }

    public void Run() {
      using ( this.CollectorService ) {
        Console.WriteLine("Listening for events...");
        this.CollectorService.Start();
        Console.ReadLine();
        this.CollectorService.Stop();
      }
    }
  }
}
