using Microsoft.Diagnostics.Tracing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winterdom.Diagnostics.TraceProcessor {
  [Export(typeof(IJsonConverter))]
  [JsonFormat(Format="Flat")]
  public class FlatJsonConverter : IJsonConverter {
    public void ToJson(TraceEvent traceEvent, TextWriter tw) {
      var writer = new JsonTextWriter(tw);
      writer.WriteStartObject();
      WriteHeader(writer, traceEvent);
      WriteEventData(writer, traceEvent);
      WritePayload(writer, traceEvent);
      writer.WriteEndObject();
    }
    public String ToJson(TraceEvent traceEvent) {
      using ( StringWriter sw = new StringWriter() ) {
        ToJson(traceEvent, sw);
        sw.Flush();
        return sw.ToString();
      }
    }

    private static void WriteHeader(JsonTextWriter writer, TraceEvent traceEvent) {
      WriteProperty(writer, "header_msec", traceEvent.TimeStampRelativeMSec);
      WriteProperty(writer, "header_processId", traceEvent.ProcessID);
      WriteProperty(writer, "header_processName", traceEvent.ProcessName);
      WriteProperty(writer, "header_threadId", traceEvent.ThreadID);
      WriteProperty(writer, "header_activityId", traceEvent.ActivityID);
      WriteProperty(writer, "header_relatedActivityId", traceEvent.RelatedActivityID);
      WriteProperty(writer, "header_eventName", traceEvent.EventName);
      WriteProperty(writer, "header_timeStamp", traceEvent.TimeStamp);
      WriteProperty(writer, "header_id", traceEvent.ID);
      WriteProperty(writer, "header_version", traceEvent.Version);
      WriteProperty(writer, "header_keywords", traceEvent.Keywords);
      WriteProperty(writer, "header_level", traceEvent.Level);
      WriteProperty(writer, "header_providerName", traceEvent.ProviderName);
      WriteProperty(writer, "header_providerGuid", traceEvent.ProviderGuid);
      WriteProperty(writer, "header_classicProvider", traceEvent.IsClassicProvider);
      WriteProperty(writer, "header_opcode", traceEvent.Opcode);
      WriteProperty(writer, "header_opcodeName", traceEvent.OpcodeName);
      WriteProperty(writer, "header_task", traceEvent.Task);
      WriteProperty(writer, "header_channel", traceEvent.Channel);
      WriteProperty(writer, "header_pointerSize", traceEvent.PointerSize);
      WriteProperty(writer, "header_cpu", traceEvent.ProcessorNumber);
      WriteProperty(writer, "header_eventIndex", traceEvent.EventIndex);
    }

    private static void WritePayload(JsonTextWriter writer, TraceEvent traceEvent) {
      writer.WritePropertyName("payload");
      writer.WriteValue(traceEvent.EventData());
    }

    private static void WriteEventData(JsonTextWriter writer, TraceEvent traceEvent) {
      String[] names = traceEvent.PayloadNames;
      for ( int i = 0; i < names.Length; i++ ) {
        writer.WritePropertyName("event_" + names[i].ToLower());
        writer.WriteValue(traceEvent.PayloadValue(i));
      }
    }

    private static void WriteProperty<T>(JsonTextWriter writer, string name, T value) {
      writer.WritePropertyName(name);
      writer.WriteValue(value);
    }
  }
}
