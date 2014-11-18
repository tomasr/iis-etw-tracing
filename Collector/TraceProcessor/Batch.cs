using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winterdom.Diagnostics.TraceProcessor {
  /// <summary>
  /// Used to keep track of event batches
  /// TODO: figure out a better way of handling it
  /// </summary>
  /// <typeparam name="TEntry">Type of item</typeparam>
  public class Batch<TEntry> {
    private readonly int maxBatchSize;
    private int batchSize;
    private Queue<TEntry> entries;

    public bool IsEmpty {
      get { return entries.Count == 0; }
    }
    public int Count {
      get { return entries.Count; }
    }

    private Batch(int maxBatchSize) 
      : this(maxBatchSize, 0, new Queue<TEntry>()) {
    }
    private Batch(int maxBatchSize, int batchSize, Queue<TEntry> entries) {
      this.maxBatchSize = maxBatchSize;
      this.batchSize = batchSize;
      this.entries = entries;
    }

    public static Batch<TEntry> Empty(int maxBatchSize) {
      return new Batch<TEntry>(maxBatchSize);
    }

    public bool TryAdd(TEntry item, int itemSize) {
      int newBatchSize = this.batchSize + itemSize;

      if ( newBatchSize > maxBatchSize ) {
        // can't add, needs flushing
        return false;
      }
      entries.Enqueue(item);
      this.batchSize = newBatchSize;
      return true;
    }

    public IEnumerable<TEntry> Drain() {
      return this.entries;
    }
  }
}
