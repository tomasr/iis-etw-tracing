using System;

namespace Winterdom.EtwCollector {
  public interface ILogCollectorService : IDisposable {
    void Start();
    void Stop();
  }
}
