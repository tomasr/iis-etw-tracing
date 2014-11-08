using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Winterdom.Diagnostics.TraceProcessor;

namespace TraceProcessor.Tests {
  [TestClass]
  public class BatchTests {
    [TestMethod]
    public void TryAddReturnsTrueIfBatchIsEmpty() {
      var batch = Batch<String>.Empty(128*1024);
      Assert.IsTrue(batch.TryAdd("1", 1024));
    }
    [TestMethod]
    public void TryAddJustToBatchSizeReturnsTrue() {
      var batch = Batch<String>.Empty(128*1024);
      for ( int i = 0; i < 127; i++ ) {
        Assert.IsTrue(batch.TryAdd("1", 1024));
      }
      // should just reach batch size
      Assert.IsTrue(batch.TryAdd("1", 1024));
    }
    [TestMethod]
    public void TryAddAfterBatchIsFullReturnsFalse() {
      var batch = Batch<String>.Empty(128*1024);
      for ( int i = 0; i < 128; i++ ) {
        Assert.IsTrue(batch.TryAdd("1", 1024));
      }
      Assert.IsFalse(batch.TryAdd("1", 1024));
    }
    [TestMethod]
    public void TryAddReturnsFalseIfBatchSizePlusNewItemIsGreaterThanMaxBatchSize() {
      var batch = Batch<String>.Empty(128*1024);
      for ( int i = 0; i < 127; i++ ) {
        Assert.IsTrue(batch.TryAdd("1", 1024));
      }
      Assert.IsFalse(batch.TryAdd("1", 2048));
    }
  }
}
