using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winterdom.Diagnostics.TraceProcessor {
  [MetadataAttribute]
  [AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
  public class JsonFormatAttribute : Attribute {
    public String Format { get; set; }
  }
}
