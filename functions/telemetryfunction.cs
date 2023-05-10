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
                JObject deviceMessage = (JObject)JsonConvert.DeserializeObject(eventGridEvent.Data.ToString());
                    log.LogInformation($"alertMessage ::: {deviceMessage}");
                    string deviceId = "deviceid1";
                    var ID = deviceId;
                    var o2s = deviceMessage["body"]["o2s"];
                    var ats = deviceMessage["body"]["ats"];
                    var pressure = deviceMessage["body"]["pressure"];
                    var cps = deviceMessage["body"]["cps"];
                    var aps = deviceMessage["body"]["aps"];
                    var sas = deviceMessage["body"]["sas"];
                    var vss = deviceMessage["body"]["vss"];
                    var iat = deviceMessage["body"]["iat"];
                    var maf = deviceMessage["body"]["maf"];
                    var ect = deviceMessage["body"]["ect"];
                
                    log.LogInformation($"Device:{deviceId} Device Id is:{ID}");
                    log.LogInformation($"Device:{deviceId} humidity is:{o2s}");
                    log.LogInformation($"Device:{deviceId} humidity is:{ats}");
                    log.LogInformation($"Device:{deviceId} humidity is:{pressure}");
                    log.LogInformation($"Device:{deviceId} humidity is:{cps}");
                    log.LogInformation($"Device:{deviceId} humidity is:{aps}");
                    log.LogInformation($"Device:{deviceId} humidity is:{sas}");
                    log.LogInformation($"Device:{deviceId} humidity is:{vss}");
                    log.LogInformation($"Device:{deviceId} humidity is:{iat}");
                    log.LogInformation($"Device:{deviceId} humidity is:{maf}");
                    log.LogInformation($"Device:{deviceId} humidity is:{ect}");


                    var updateProperty = new JsonPatchDocument();
                    updateProperty.AppendReplace("/deviceid", deviceId);
                    updateProperty.AppendReplace("/o2s", o2s.Value<double>());
                    updateProperty.AppendReplace("/ats", ats.Value<double>());
                    updateProperty.AppendReplace("/pressure", pressure.Value<string>());
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
            }
            catch (Exception e)
            {
                log.LogInformation(e.Message);
            }
        }
    }
}