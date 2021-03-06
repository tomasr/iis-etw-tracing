﻿using Microsoft.Diagnostics.Tracing;
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
  [JsonFormat(Format="Complex")]
  public class ComplexJsonConverter : IJsonConverter {
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
      writer.WritePropertyName("header");
      writer.WriteStartObject();

      WriteProperty(writer, "msec", traceEvent.TimeStampRelativeMSec);
      WriteProperty(writer, "processId", traceEvent.ProcessID);
      WriteProperty(writer, "processName", traceEvent.ProcessName);
      WriteProperty(writer, "threadId", traceEvent.ThreadID);
      WriteProperty(writer, "activityId", traceEvent.ActivityID);
      WriteProperty(writer, "relatedActivityId", traceEvent.RelatedActivityID);
      WriteProperty(writer, "eventName", traceEvent.EventName);
      WriteProperty(writer, "timeStamp", traceEvent.TimeStamp);
      WriteProperty(writer, "id", traceEvent.ID);
      WriteProperty(writer, "version", traceEvent.Version);
      WriteProperty(writer, "keywords", traceEvent.Keywords);
      WriteProperty(writer, "level", traceEvent.Level);
      WriteProperty(writer, "providerName", traceEvent.ProviderName);
      WriteProperty(writer, "providerGuid", traceEvent.ProviderGuid);
      WriteProperty(writer, "classicProvider", traceEvent.IsClassicProvider);
      WriteProperty(writer, "opcode", traceEvent.Opcode);
      WriteProperty(writer, "opcodeName", traceEvent.OpcodeName);
      WriteProperty(writer, "task", traceEvent.Task);
      WriteProperty(writer, "channel", traceEvent.Channel);
      WriteProperty(writer, "pointerSize", traceEvent.PointerSize);
      WriteProperty(writer, "cpu", traceEvent.ProcessorNumber);
      WriteProperty(writer, "eventIndex", traceEvent.EventIndex);

      writer.WriteEndObject();
    }

    private static void WritePayload(JsonTextWriter writer, TraceEvent traceEvent) {
      writer.WritePropertyName("payload");
      writer.WriteValue(traceEvent.EventData());
    }

    private static void WriteEventData(JsonTextWriter writer, TraceEvent traceEvent) {
      writer.WritePropertyName("event");
      writer.WriteStartObject();
      String[] names = traceEvent.PayloadNames;
      for ( int i = 0; i < names.Length; i++ ) {
        writer.WritePropertyName(names[i].ToLower());
        writer.WriteValue(traceEvent.PayloadValue(i));
      }
      writer.WriteEndObject();
    }

    private static void WriteProperty<T>(JsonTextWriter writer, string name, T value) {
      writer.WritePropertyName(name);
      writer.WriteValue(value);
    }
  }
}
