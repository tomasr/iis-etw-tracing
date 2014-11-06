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
using Winterdom.Diagnostics.Tracing.IISTraceProcessor;

namespace TraceProcessor.Tests {
  [TestClass]
  [DeploymentItem("iis.etl")]
  public class JsonExtensionsTests {
    [TestMethod]
    public void IisEventToJson() {
      var traceEvents = LoadSampleTrace("iis.etl");
      String json = traceEvents.First().ToJson();
      using ( var reader = new JsonTextReader(new StringReader(json)) ) {
        JObject obj = (JObject)JToken.ReadFrom(reader);
        Assert.IsNotNull(obj);
      }
    }

    [TestMethod]
    public void IisEventToJson_HasHeaderData() {
      var traceEvents = LoadSampleTrace("iis.etl");
      var te = traceEvents.First();
      String json = te.ToJson();
      using ( var reader = new JsonTextReader(new StringReader(json)) ) {
        JObject obj = (JObject)JToken.ReadFrom(reader);
        JObject header = (JObject)obj["header"];

        Assert.IsNotNull(header);
        Assert.AreEqual(te.TimeStampRelativeMSec, (double)header["msec"]);
        Assert.AreEqual(te.ProcessID, (int)header["processId"]);
        Assert.AreEqual(te.ThreadID, (int)header["threadId"]);
        Assert.AreEqual(te.EventName, (String)header["eventName"]);
        Assert.AreEqual((int)te.ID, (int)header["id"]);
        Assert.AreEqual(te.ProviderName, (String)header["providerName"]);
        Assert.AreEqual(te.ProcessorNumber, (int)header["cpu"]);
      }
    }

    [TestMethod]
    public void IisEventToJson_HasPayload() {
      var traceEvents = LoadSampleTrace("iis.etl");
      var te = traceEvents.First();
      String json = te.ToJson();
      using ( var reader = new JsonTextReader(new StringReader(json)) ) {
        JObject obj = (JObject)JToken.ReadFrom(reader);
        byte[] payload = (byte[])obj["payload"];

        Assert.IsNotNull(payload);
        Assert.AreEqual(232, payload.Length);
      }
    }

    [TestMethod]
    public void IisEventToJson_HasEventData() {
      var traceEvents = LoadSampleTrace("iis.etl");
      var te = traceEvents.First();
      String json = te.ToJson();
      using ( var reader = new JsonTextReader(new StringReader(json)) ) {
        JObject obj = (JObject)JToken.ReadFrom(reader);
        JObject data = (JObject)obj["event"];

        Assert.IsNotNull(data);
        Assert.AreEqual("GET", (String)data["cs_method"]);
        Assert.AreEqual("/", (String)data["cs_uri_stem"]);
        Assert.AreEqual(80, (int)data["s_port"]);
      }
    }

    private IEnumerable<TraceEvent> LoadSampleTrace(string file) {
      var traceSource = new ETWTraceEventSource(file);
      var parser = new IISLogTraceEventParser(traceSource);
      List<TraceEvent> events = new List<TraceEvent>();
      parser.IISLog += e => {
        events.Add(e.Clone());
      };
      traceSource.Process();
      return events;
    }
  }
}
