using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winterdom.Diagnostics.TraceProcessor {
  public static class BinHexConverter {
    public static String ToBinHex(byte[] data) {
      if ( data.Length == 0 ) {
        return String.Empty;
      }
      StringBuilder sb = new StringBuilder(data.Length*2+2);
      sb.Append("0x");
      for ( int i = 0; i < data.Length; i++ ) {
        sb.AppendFormat("{0:x2}", data[i]);
      }
      return sb.ToString();
    }

    // TODO: Only used for testing, optimize later!
    public static byte[] FromBinHex(String data) {
      if ( String.IsNullOrEmpty(data) ) {
        return new byte[0];
      }
      if ( !data.StartsWith("0x") ) {
        throw new FormatException("Data is not in the binhex format");
      }
      byte[] bytes = new byte[(data.Length - 2) / 2];
      for ( int i = 2; i < data.Length; i += 2 ) {
        bytes[i] = Convert.ToByte(data.Substring(i, 2), 16);
      }
      return bytes;
    }
  }
}
