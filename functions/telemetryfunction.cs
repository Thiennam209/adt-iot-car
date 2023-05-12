using Azure;
using Azure.Core.Pipeline;
using Azure.DigitalTwins.Core;
using Azure.Identity;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Collections.Generic;
using Microsoft.Azure.SignalR.Protocol;



namespace My.Function
{
    // This class processes telemetry events from IoT Hub, reads temperature of a device
    // and sets the "Temperature" property of the device with the value of the telemetry.
    public class telemetryfunction
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private static string adtServiceUrl = Environment.GetEnvironmentVariable("ADT_SERVICE_URL");



        [FunctionName("telemetryfunction")]
        public async void Run([EventGridTrigger] EventGridEvent eventGridEvent, ILogger log)
        {
            try
            {
                // After this is deployed, you need to turn the Managed Identity Status to "On",
                // Grab Object Id of the function and assigned "Azure Digital Twins Owner (Preview)" role
                // to this function identity in order for this function to be authorized on ADT APIs.
                //Authenticate with Digital Twins
                var credentials = new DefaultAzureCredential();
                log.LogInformation(credentials.ToString());
                DigitalTwinsClient client = new DigitalTwinsClient(
                new Uri(adtServiceUrl), credentials, new DigitalTwinsClientOptions
                { Transport = new HttpClientTransport(httpClient) });
                log.LogInformation($"ADT service client connection created.");
                log.LogInformation($"eventGridEvent ::: {eventGridEvent}");
                log.LogInformation($"eventGridEvent.Data ::: {eventGridEvent.Data}");
                //if (eventGridEvent.Data.ToString().Contains("pressure"))
                //{
                JObject deviceMessage = (JObject)JsonConvert.DeserializeObject(eventGridEvent.Data.ToString());
                log.LogInformation($"alertMessage ::: {deviceMessage}");
                string deviceId = "deviceid1";
                var ID = "deviceid1";
                var oxys = deviceMessage["body"]["oxys"] != null ? deviceMessage["body"]["oxys"] : 1;
                var ats = deviceMessage["body"]["ats"] != null ? deviceMessage["body"]["ats"] : 1;
                var pressure = deviceMessage["body"]["pressure"] != null ? deviceMessage["body"]["pressure"] : 1;
                var cps = deviceMessage["body"]["cps"] != null ? deviceMessage["body"]["cps"] : 0;
                var aps = deviceMessage["body"]["aps"] != null ? deviceMessage["body"]["aps"] : 0;
                var sas = deviceMessage["body"]["sas"] != null ? deviceMessage["body"]["sas"] : 0;
                var vss = deviceMessage["body"]["vss"] != null ? deviceMessage["body"]["vss"] : 0;
                var iat = deviceMessage["body"]["iat"] != null ? deviceMessage["body"]["iat"] : 0;
                var maf = deviceMessage["body"]["maf"] != null ? deviceMessage["body"]["maf"] : 0;
                var ect = deviceMessage["body"]["ect"] != null ? deviceMessage["body"]["ect"] : 0;



                log.LogInformation($"Device:{deviceId} Device Id is:{ID}");
                log.LogInformation($"Device:{deviceId} oxys is:{oxys}");
                log.LogInformation($"Device:{deviceId} ats is:{ats}");
                log.LogInformation($"Device:{deviceId} pressure is:{pressure}");
                log.LogInformation($"Device:{deviceId} cps is:{cps}");
                log.LogInformation($"Device:{deviceId} aps is:{aps}");
                log.LogInformation($"Device:{deviceId} sas is:{sas}");
                log.LogInformation($"Device:{deviceId} vss is:{vss}");
                log.LogInformation($"Device:{deviceId} iat is:{iat}");
                log.LogInformation($"Device:{deviceId} maf is:{maf}");
                log.LogInformation($"Device:{deviceId} ect is:{ect}");




                var updateProperty = new JsonPatchDocument();
                updateProperty.AppendReplace("/deviceid", ID);
                updateProperty.AppendReplace("/oxys", oxys.Value<double>());
                updateProperty.AppendReplace("/ats", ats.Value<double>());
                updateProperty.AppendReplace("/pressure", pressure.Value<double>());
                updateProperty.AppendReplace("/cps", cps.Value<double>());
                updateProperty.AppendReplace("/aps", aps.Value<double>());
                updateProperty.AppendReplace("/sas", sas.Value<double>());
                updateProperty.AppendReplace("/vss", vss.Value<double>());
                updateProperty.AppendReplace("/iat", iat.Value<double>());
                updateProperty.AppendReplace("/maf", maf.Value<double>());
                updateProperty.AppendReplace("/ect", ect.Value<double>());
                log.LogInformation(updateProperty.ToString());
                try
                {
                    await client.UpdateDigitalTwinAsync(deviceId, updateProperty);
                }
                catch (Exception e)
                {
                    log.LogInformation(e.Message);
                }
                //} else if (eventGridEvent.Data.ToString().Contains("pressure") && eventGridEvent != null && eventGridEvent.Data != null)
                //{
                //    JObject deviceMessage = (JObject)JsonConvert.DeserializeObject(eventGridEvent.Data.ToString());
                //    log.LogInformation($"alertMessage ::: {deviceMessage}");
                //    string deviceId = "deviceid1";
                //    var ID = "deviceid1";
                //    var o2s = deviceMessage["body"]["o2s"];
                //    var ats = deviceMessage["body"]["ats"];
                //    var pressure = deviceMessage["body"]["pressure"];
                //    var cps = deviceMessage["body"]["cps"];
                //    var aps = deviceMessage["body"]["aps"];
                //    var sas = deviceMessage["body"]["sas"];
                //    var vss = deviceMessage["body"]["vss"];
                //    var iat = deviceMessage["body"]["iat"];
                //    var maf = deviceMessage["body"]["maf"];
                //    var ect = deviceMessage["body"]["ect"];



                //    log.LogInformation($"Device:{deviceId} Device Id is:{ID}");
                //    log.LogInformation($"Device:{deviceId} o2s is:{o2s}");
                //    log.LogInformation($"Device:{deviceId} ats is:{ats}");
                //    log.LogInformation($"Device:{deviceId} pressure is:{pressure}");
                //    log.LogInformation($"Device:{deviceId} cps is:{cps}");
                //    log.LogInformation($"Device:{deviceId} aps is:{aps}");
                //    log.LogInformation($"Device:{deviceId} sas is:{sas}");
                //    log.LogInformation($"Device:{deviceId} vss is:{vss}");
                //    log.LogInformation($"Device:{deviceId} iat is:{iat}");
                //    log.LogInformation($"Device:{deviceId} maf is:{maf}");
                //    log.LogInformation($"Device:{deviceId} ect is:{ect}");




                //    var updateProperty = new JsonPatchDocument();
                //    updateProperty.AppendReplace("/deviceid", ID);
                //    updateProperty.AppendReplace("/o2s", o2s.Value<double>() );
                //    updateProperty.AppendReplace("/ats", ats.Value<double>());
                //    updateProperty.AppendReplace("/pressure", pressure.Value<double>());
                //    updateProperty.AppendReplace("/cps", cps.Value<double>());
                //    updateProperty.AppendReplace("/aps", aps.Value<double>());
                //    updateProperty.AppendReplace("/sas", sas.Value<double>());
                //    updateProperty.AppendReplace("/vss", vss.Value<double>());
                //    updateProperty.AppendReplace("/iat", iat.Value<double>());
                //    updateProperty.AppendReplace("/maf", maf.Value<double>());
                //    updateProperty.AppendReplace("/ect", ect.Value<double>());
                //    log.LogInformation(updateProperty.ToString());
                //    try
                //    {
                //        await client.UpdateDigitalTwinAsync(deviceId, updateProperty);
                //    }
                //    catch (Exception e)
                //    {
                //        log.LogInformation(e.Message);
                //    }
                //}



            }
            catch (Exception e)
            {
                log.LogInformation(e.Message);
            }
        }
    }
}