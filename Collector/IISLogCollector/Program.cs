using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using Winterdom.Diagnostics.TraceProcessor;

namespace Winterdom.EtwCollector {
  // TODO: Should be a service!
  class Program {
    [Import]
    public ILogCollectorService CollectorService { get; set; }

    static void Main(string[] args) {
      AggregateCatalog catalog = new AggregateCatalog();
      String baseDir = AppDomain.CurrentDomain.BaseDirectory;
      catalog.Catalogs.Add(new DirectoryCatalog(baseDir));
      catalog.Catalogs.Add(new AssemblyCatalog(typeof(Program).Assembly));
      CompositionContainer container = new FlatCompositionContainer(catalog);

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

  class FlatCompositionContainer : CompositionContainer {
    public FlatCompositionContainer(AggregateCatalog catalog) : base(catalog) {
    }

    protected override IEnumerable<Export> GetExportsCore(ImportDefinition definition, AtomicComposition atomicComposition) {
      var exports = base.GetExportsCore(definition, atomicComposition);
      if ( definition.ContractName == typeof(IJsonConverter).FullName ) {
        object format;
        var r = from e in exports
               where e.Metadata.TryGetValue("Format", out format)
                  && String.Equals(format, "Flat")
               select e;
        return r;
      }
      return exports;
    }
  }
}
