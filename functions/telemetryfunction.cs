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
                    string deviceId = (string)alertMessage["systemProperties"]["iothub-connection-device-id"];
                    var ID = alertMessage["body"]["DeviceId"];
                    //var alert = alertMessage["body"]["Alert"];
                    log.LogInformation($"Device:{deviceId} Device Id is:{ID}");
                    //log.LogInformation($"Device:{deviceId} Alert Status is:{alert}");

                    var updateProperty = new JsonPatchDocument();
                    //updateProperty.AppendReplace("/Alert", alert.Value<bool>());
                    updateProperty.AppendReplace("/DeviceId", ID.Value<string>());
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
                else if (eventGridEvent != null && eventGridEvent.Data != null)
                {

                    JObject deviceMessage = (JObject)JsonConvert.DeserializeObject(eventGridEvent.Data.ToString());
                    string deviceId = (string)deviceMessage["systemProperties"]["iothub-connection-device-id"];
                    var ID = deviceMessage["body"]["DeviceID"];
                    var TimeInterval = deviceMessage["body"]["TimeInterval"];
                    var humidity = deviceMessage["body"]["Humidity"];
                    var temperature = deviceMessage["body"]["Temperature"];
                    var pressure = deviceMessage["body"]["Pressure"];
                    var magnetometerX = deviceMessage["body"]["MagnetometerX"];
                    var magnetometerY = deviceMessage["body"]["MagnetometerY"];
                    var magnetometerZ = deviceMessage["body"]["MagnetometerZ"];
                    var accelerometerX = deviceMessage["body"]["AccelerometerX"];
                    var accelerometerY = deviceMessage["body"]["AccelerometerY"];
                    var accelerometerZ = deviceMessage["body"]["AccelerometerZ"];
                    var gyroscopeX = deviceMessage["body"]["GyroscopeX"];
                    var gyroscopeY = deviceMessage["body"]["GyroscopeY"];
                    var gyroscopeZ = deviceMessage["body"]["GyroscopeZ"];

                    log.LogInformation($"Device:{deviceId} Device Id is:{ID}");
                    log.LogInformation($"Device:{deviceId} Time interval is:{TimeInterval}");
                    log.LogInformation($"Device:{deviceId} humidity is:{humidity}");
                    log.LogInformation($"Device:{deviceId} temperature is:{temperature}");
                    log.LogInformation($"Device:{deviceId} pressure is:{pressure}");
                    log.LogInformation($"Device:{deviceId} magnetometerX is:{magnetometerX}");
                    log.LogInformation($"Device:{deviceId} magnetometerY is:{magnetometerY}");
                    log.LogInformation($"Device:{deviceId} magnetometerZ is:{magnetometerZ}");
                    log.LogInformation($"Device:{deviceId} accelerometerX is:{accelerometerX}");
                    log.LogInformation($"Device:{deviceId} accelerometerY is:{accelerometerY}");
                    log.LogInformation($"Device:{deviceId} accelerometerZ is:{accelerometerZ}");
                    log.LogInformation($"Device:{deviceId} gyroscopeX is:{gyroscopeX}");
                    log.LogInformation($"Device:{deviceId} gyroscopeY is:{gyroscopeY}");
                    log.LogInformation($"Device:{deviceId} gyroscopeZ is:{gyroscopeZ}");
                    var updateProperty = new JsonPatchDocument();
                    var turbineTelemetry = new Dictionary<string, Object>()
                    {
                        ["DeviceId"] = ID,
                        ["TimeInterval"] = TimeInterval,
                        ["Humidity"] = humidity,
                        ["Temperature"] = temperature,
                        ["Pressure"] = pressure,
                        ["MagnetometerX"] = magnetometerX,
                        ["MagnetometerY"] = magnetometerY,
                        ["MagnetometerZ"] = magnetometerZ,
                        ["AccelerometerZ"] = accelerometerX,
                        ["AccelerometerY"] = accelerometerY,
                        ["AccelerometerZ"] = accelerometerZ,
                        ["GyroscopeX"] = gyroscopeX,
                        ["GyroscopeY"] = gyroscopeY,
                        ["GyroscopeZ"] = gyroscopeZ,
                    };
                    updateProperty.AppendAdd("/DeviceId", ID.Value<string>());

                    log.LogInformation(updateProperty.ToString());
                    try
                    {
                        await client.PublishTelemetryAsync(deviceId, Guid.NewGuid().ToString(), JsonConvert.SerializeObject(turbineTelemetry));
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