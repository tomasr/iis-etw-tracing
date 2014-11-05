using Microsoft.Diagnostics.Tracing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winterdom.Diagnostics.Tracing.IISTraceEvent {
  public class IISLogTraceData : TraceEvent {
    public int EnabledFieldsFlags {
      get { return GetInt32At(GetOffsetForField(0)); }
    }
    public String Date {
      get { return GetUnicodeStringAt(GetOffsetForField(1)); }
    }
    public String Time {
      get { return GetUnicodeStringAt(GetOffsetForField(2)); }
    }
    public String C_ip {
      get { return GetUnicodeStringAt(GetOffsetForField(3)); }
    }
    public String Cs_username {
      get { return GetUnicodeStringAt(GetOffsetForField(4)); }
    }
    public String S_sitename {
      get { return GetUnicodeStringAt(GetOffsetForField(5)); }
    }
    public String S_computername {
      get { return GetUnicodeStringAt(GetOffsetForField(6)); }
    }
    public String S_ip {
      get { return GetUnicodeStringAt(GetOffsetForField(7)); }
    }
    public String Cs_method {
      get { return GetUTF8StringAt(GetOffsetForField(8)); }
    }
    public String Cs_uri_stem {
      get { return GetUnicodeStringAt(GetOffsetForField(9)); }
    }
    public String Cs_uri_query {
      get { return GetUTF8StringAt(GetOffsetForField(10)); }
    }
    public int Sc_status {
      get { return GetInt16At(GetOffsetForField(11)); }
    }
    public int Sc_win32_status {
      get { return GetInt32At(GetOffsetForField(12)); }
    }
    public long Sc_bytes {
      get { return GetInt64At(GetOffsetForField(13)); }
    }
    public long Cs_bytes {
      get { return GetInt64At(GetOffsetForField(14)); }
    }
    public long Time_taken {
      get { return GetInt64At(GetOffsetForField(15)); }
    }
    public int S_port {
      get { return GetInt16At(GetOffsetForField(16)); }
    }
    public String CsUser_agent {
      get { return GetUTF8StringAt(GetOffsetForField(17)); }
    }
    public String CsCookie {
      get { return GetUTF8StringAt(GetOffsetForField(18)); }
    }
    public String CsReferer {
      get { return GetUTF8StringAt(GetOffsetForField(19)); }
    }
    public String Cs_version {
      get { return GetUnicodeStringAt(GetOffsetForField(20)); }
    }
    public String Cs_host {
      get { return GetUTF8StringAt(GetOffsetForField(21)); }
    }
    public int Sc_substatus {
      get { return GetInt16At(GetOffsetForField(22)); }
    }
    public String CustomFields {
      get { return GetUnicodeStringAt(GetOffsetForField(23)); }
    }

    private static readonly String[] PayloadNamesCache;
    public override String[] PayloadNames {
      get { return PayloadNamesCache; }
    }

    private Action<IISLogTraceData> target;
    protected override Delegate Target {
      get { return target; }
      set { target = (Action<IISLogTraceData>)value; }
    }

    static IISLogTraceData() {
      var names = "EnabledFieldsFlags,Date,Time,C_ip,Cs_username,"
               + "S_sitename,S_computername,S_ip,Cs_method,Cs_uri_stem,"
               + "Cs_uri_query,Sc_status,Sc_win32_status,Sc_bytes,"
               + "Cs_bytes,Time_taken,S_port,CsUser_agent,CsCookie,CsReferer,"
               + "Cs_version,Cs_host,Sc_substatus,CustomFields";
      PayloadNamesCache = names.Split(',');
    }

    public IISLogTraceData(Action<IISLogTraceData> action, int eventID, int task, String taskName, Guid taskGuid, int opcode, String opcodeName, Guid providerGuid, String providerName)
        : base(eventID, task, taskName, taskGuid, opcode, opcodeName, providerGuid, providerName) {
      this.Target = action;
    }

    protected override void Dispatch() {
      var action = target;
      if ( action != null ) {
        action(this);
      }
    }

    public override object PayloadValue(int index)
    {
      switch ( index ) {
        case 0: return EnabledFieldsFlags;
        case 1: return Date;
        case 2: return Time;
        case 3: return C_ip;
        case 4: return Cs_username;
        case 5: return S_sitename;
        case 6: return S_computername;
        case 7: return S_ip;
        case 8: return Cs_method;
        case 9: return Cs_uri_stem;
        case 10: return Cs_uri_query;
        case 11: return Sc_status;
        case 12: return Sc_win32_status;
        case 13: return Sc_bytes;
        case 14: return Cs_bytes;
        case 15: return Time_taken;
        case 16: return S_port;
        case 17: return CsUser_agent;
        case 18: return CsCookie;
        case 19: return CsReferer;
        case 20: return Cs_version;
        case 21: return Cs_host;
        case 22: return Sc_substatus;
        case 23: return CustomFields;
      }
      return null;
    }

    public override StringBuilder ToXml(StringBuilder sb) {
      Prefix(sb);
      XmlAttrib(sb, "EnabledFieldsFlags", EnabledFieldsFlags.ToString("x"));
      XmlAttrib(sb, "Date", Date);
      XmlAttrib(sb, "Time", Time);
      XmlAttrib(sb, "C_ip", C_ip);
      XmlAttrib(sb, "Cs_username", Cs_username);
      XmlAttrib(sb, "S_sitename", S_sitename);
      XmlAttrib(sb, "S_computername", S_computername);
      XmlAttrib(sb, "S_ip", S_ip);
      XmlAttrib(sb, "Cs_method", Cs_method);
      XmlAttrib(sb, "Cs_uri_stem", Cs_uri_stem);
      XmlAttrib(sb, "Cs_uri_query", Cs_uri_query);
      XmlAttrib(sb, "Sc_status", Sc_status);
      XmlAttrib(sb, "Sc_win32_status", Sc_win32_status);
      XmlAttrib(sb, "Sc_bytes", Sc_bytes);
      XmlAttrib(sb, "Cs_bytes", Cs_bytes);
      XmlAttrib(sb, "Time_taken", Time_taken);
      XmlAttrib(sb, "S_port", S_port);
      XmlAttrib(sb, "CsUser_agent", CsUser_agent);
      XmlAttrib(sb, "CsCookie", CsCookie);
      XmlAttrib(sb, "CsReferer", CsReferer);
      XmlAttrib(sb, "Cs_version", Cs_version);
      XmlAttrib(sb, "Cs_host", Cs_host);
      XmlAttrib(sb, "Sc_substatus", Sc_substatus);
      XmlAttrib(sb, "CustomFields", CustomFields);
      sb.Append("/>");
      return sb;
    }

    static readonly int[] FieldSizes = new int[] {
      4, 0, 0, 0, 0, 0,
      0, 0, -1, 0, -1,
      2, 4, 8, 8, 8,
      2, -1, -1, -1,
      0, -1, 2, 0
    };

    private int GetOffsetForField(int index) {
      int offset = 0;
      for ( int i = 1; i <= index; i++ ) {
        switch ( FieldSizes[i - 1] ) {
          case -1: offset = SkipUTF8String(offset); break;
          case 0: offset = SkipUnicodeString(offset); break;
          default: offset += FieldSizes[i - 1]; break;
        }
      }
        return offset;
    }
  }
}
