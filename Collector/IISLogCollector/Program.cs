using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Winterdom.Diagnostics.TraceProcessor.Impl;

namespace IISLogCollector {
  // TODO: Should be a service!
  class Program {
    static void Main(string[] args) {
      var keyGenerator = new BasicPartitionKeyGenerator();
      var eventProcessor = new EventHubEventProcessor(keyGenerator);
      var processor = new TraceSourceProcessor(eventProcessor);
      using ( var collector = new IISEtwLogCollector(processor) ) {
        Console.WriteLine("Starting collection...");
        collector.Start();
        Console.ReadLine();
        collector.Stop();
      }
    }
  }
}
