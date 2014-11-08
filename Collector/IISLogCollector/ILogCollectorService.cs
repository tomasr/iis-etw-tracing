using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IISLogCollector {
  public interface ILogCollectorService : IDisposable {
    void Start();
    void Stop();
  }
}
