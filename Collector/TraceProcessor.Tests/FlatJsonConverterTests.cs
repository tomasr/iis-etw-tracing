using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Session;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Winterdom.Diagnostics.Tracing.IISTraceEvent;
using Winterdom.Diagnostics.TraceProcessor;

namespace TraceProcessor.Tests {
  [TestClass]
  public class FlatJsonConverterTests : BaseEtwTraceTests {
    [TestMethod]
    public void IisEventToJson() {
      var traceEvents = LoadSampleTrace();
      String json = ToJson(traceEvents.First());
      using ( var reader = new JsonTextReader(new StringReader(json)) ) {
        JObject obj = (JObject)JToken.ReadFrom(reader);
        Assert.IsNotNull(obj);
      }
    }

    [TestMethod]
    public void IisEventToJson_HasHeaderData() {
      var traceEvents = LoadSampleTrace();
      var te = traceEvents.First();
      String json = ToJson(te);
      using ( var reader = new JsonTextReader(new StringReader(json)) ) {
        JObject obj = (JObject)JToken.ReadFrom(reader);
        Assert.AreEqual(te.TimeStampRelativeMSec, (double)obj["header_msec"]);
        Assert.AreEqual(te.ProcessID, (int)obj["header_processId"]);
        Assert.AreEqual(te.ThreadID, (int)obj["header_threadId"]);
        Assert.AreEqual(te.EventName, (String)obj["header_eventName"]);
        Assert.AreEqual((int)te.ID, (int)obj["header_id"]);
        Assert.AreEqual(te.ProviderName, (String)obj["header_providerName"]);
        Assert.AreEqual(te.ProcessorNumber, (int)obj["header_cpu"]);
      }
    }

    [TestMethod]
    public void IisEventToJson_HasPayload() {
      var traceEvents = LoadSampleTrace();
      var te = traceEvents.First();
      String json = ToJson(te);
      using ( var reader = new JsonTextReader(new StringReader(json)) ) {
        JObject obj = (JObject)JToken.ReadFrom(reader);
        byte[] payload = (byte[])obj["payload"];

        Assert.IsNotNull(payload);
        Assert.AreEqual(232, payload.Length);
      }
    }

    [TestMethod]
    public void IisEventToJson_HasEventData() {
      var traceEvents = LoadSampleTrace();
      var te = traceEvents.First();
      String json = ToJson(te);
      using ( var reader = new JsonTextReader(new StringReader(json)) ) {
        JObject obj = (JObject)JToken.ReadFrom(reader);

        Assert.AreEqual("GET", (String)obj["event_cs_method"]);
        Assert.AreEqual("/", (String)obj["event_cs_uri_stem"]);
        Assert.AreEqual(80, (int)obj["event_s_port"]);
      }
    }

    private String ToJson(TraceEvent traceEvent) {
      IJsonConverter converter = new FlatJsonConverter();
      return converter.ToJson(traceEvent);
    }
  }
}
