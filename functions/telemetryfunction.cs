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
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

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
                if (eventGridEvent.Data.ToString().Contains("Alert"))
                {
                    JObject alertMessage = (JObject)JsonConvert.DeserializeObject(eventGridEvent.Data.ToString());
                    string deviceid = (string)alertMessage["systemProperties"]["iothub-connection-device-id"];
                    var ID = alertMessage["body"]["deviceid"];
                    //var alert = alertMessage["body"]["Alert"];
                    log.LogInformation($"Device:{deviceid} Device Id is:{ID}");
                    //log.LogInformation($"Device:{deviceId} Alert Status is:{alert}");

                    var updateProperty = new JsonPatchDocument();
                    //updateProperty.AppendReplace("/Alert", alert.Value<bool>());
                    updateProperty.AppendReplace("/ieviceid", ID.Value<string>());
                    log.LogInformation(updateProperty.ToString());
                    try
                    {
                        await client.UpdateDigitalTwinAsync(deviceid, updateProperty);
                    }
                    catch (Exception e)
                    {
                        log.LogInformation(e.Message);
                    }
                }
                else if (eventGridEvent != null && eventGridEvent.Data != null)
                {

                    JObject deviceMessage = (JObject)JsonConvert.DeserializeObject(eventGridEvent.Data.ToString());
                    string deviceid = (string)deviceMessage["systemProperties"]["iothub-connection-device-id"];
                    var ID = deviceMessage["body"]["deviceid"];
                    var timeinterval = deviceMessage["body"]["timeinterval"];
                    var humidity = deviceMessage["body"]["humidity"];
                    var temperature = deviceMessage["body"]["temperature"];
                    var pressure = deviceMessage["body"]["pressure"];
                    var magnetometerX = deviceMessage["body"]["magnetometerX"];
                    var magnetometerY = deviceMessage["body"]["magnetometerY"];
                    var magnetometerZ = deviceMessage["body"]["magnetometerZ"];
                    var accelerometerX = deviceMessage["body"]["accelerometerX"];
                    var accelerometerY = deviceMessage["body"]["accelerometerY"];
                    var accelerometerZ = deviceMessage["body"]["accelerometerZ"];
                    var gyroscopeX = deviceMessage["body"]["gyroscopeX"];
                    var gyroscopeY = deviceMessage["body"]["gyroscopeY"];
                    var gyroscopeZ = deviceMessage["body"]["gyroscopeZ"];

                    log.LogInformation($"Device:{deviceid} Device Id is:{ID}");
                    log.LogInformation($"Device:{deviceid} Time interval is:{timeinterval}");
                    log.LogInformation($"Device:{deviceid} humidity is:{humidity}");
                    log.LogInformation($"Device:{deviceid} temperature is:{temperature}");
                    log.LogInformation($"Device:{deviceid} pressure is:{pressure}");
                    log.LogInformation($"Device:{deviceid} magnetometerX is:{magnetometerX}");
                    log.LogInformation($"Device:{deviceid} magnetometerY is:{magnetometerY}");
                    log.LogInformation($"Device:{deviceid} magnetometerZ is:{magnetometerZ}");
                    log.LogInformation($"Device:{deviceid} accelerometerX is:{accelerometerX}");
                    log.LogInformation($"Device:{deviceid} accelerometerY is:{accelerometerY}");
                    log.LogInformation($"Device:{deviceid} accelerometerZ is:{accelerometerZ}");
                    log.LogInformation($"Device:{deviceid} gyroscopeX is:{gyroscopeX}");
                    log.LogInformation($"Device:{deviceid} gyroscopeY is:{gyroscopeY}");
                    log.LogInformation($"Device:{deviceid} gyroscopeZ is:{gyroscopeZ}");
                    var updateProperty = new JsonPatchDocument();
                    var turbineTelemetry = new Dictionary<string, Object>()
                    {
                        ["deviceid"] = ID,
                        ["timeinterval"] = timeinterval,
                        ["humidity"] = humidity,
                        ["temperature"] = temperature,
                        ["pressure"] = pressure,
                        ["magnetometerX"] = magnetometerX,
                        ["magnetometerY"] = magnetometerY,
                        ["magnetometerZ"] = magnetometerZ,
                        ["accelerometerZ"] = accelerometerX,
                        ["accelerometerY"] = accelerometerY,
                        ["accelerometerZ"] = accelerometerZ,
                        ["gyroscopeX"] = gyroscopeX,
                        ["gyroscopeY"] = gyroscopeY,
                        ["gyroscopeZ"] = gyroscopeZ,
                    };
                    updateProperty.AppendAdd("/deviceid", ID.Value<string>());

                    log.LogInformation(updateProperty.ToString());
                    try
                    {
                        await client.PublishTelemetryAsync(deviceid, Guid.NewGuid().ToString(), JsonConvert.SerializeObject(turbineTelemetry));
                    }
                    catch (Exception e)
                    {
                        log.LogInformation(e.Message);
                    }
                }
            }
            catch (Exception e)
            {
                log.LogInformation(e.Message);
            }
        }
    }
}