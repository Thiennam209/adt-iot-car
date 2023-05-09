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
                var _deviceid = eventGridEvent.Data.ToString().Contains("deviceid");
                var _humidity = eventGridEvent.Data.ToString().Contains("humidity");
                log.LogInformation($"Device ::: {_deviceid}");
                log.LogInformation($"Humidity ::: {_humidity}");

                if (eventGridEvent.Data.ToString().Contains("humidity"))
                {
                   JObject alertMessage = (JObject)JsonConvert.DeserializeObject(eventGridEvent.Data.ToString());

                    log.LogInformation($"alertMessage ::: {alertMessage}");

                    string deviceId = (string)alertMessage["systemProperties"]["iothub-connection-device-id"];
                   var ID = alertMessage["body"]["deviceid"];
                   var humidity = alertMessage["body"]["humidity"];
                    //var timeinterval = alertMessage["body"]["timeinterval"];
                    //var temperature = alertMessage["body"]["temperature"];
                    //var pressure = alertMessage["body"]["pressure"];
                    //var magnetometerX = alertMessage["body"]["magnetometerX"];
                    //var magnetometerY = alertMessage["body"]["magnetometerY"];
                    //var magnetometerZ = alertMessage["body"]["magnetometerZ"];
                    //var accelerometerX = alertMessage["body"]["accelerometerX"];
                    //var accelerometerY = alertMessage["body"]["accelerometerY"];
                    //var accelerometerZ = alertMessage["body"]["accelerometerZ"];
                    //var gyroscopeX = alertMessage["body"]["gyroscopeX"];
                    //var gyroscopeY = alertMessage["body"]["gyroscopeY"];
                    //var gyroscopeZ = alertMessage["body"]["gyroscopeZ"];
                    log.LogInformation($"Device:{deviceId} Device Id is:{ID}");
                    log.LogInformation($"Device:{deviceId} Device Id is:{humidity}");
                    //log.LogInformation($"Device:{deviceId} Device Id is:{timeinterval}");
                    //log.LogInformation($"Device:{deviceId} Device Id is:{temperature}");
                    //log.LogInformation($"Device:{deviceId} Device Id is:{pressure}");
                    //log.LogInformation($"Device:{deviceId} Device Id is:{magnetometerX}");
                    //log.LogInformation($"Device:{deviceId} Device Id is:{magnetometerY}");
                    //log.LogInformation($"Device:{deviceId} Device Id is:{magnetometerZ}");
                    //log.LogInformation($"Device:{deviceId} Device Id is:{accelerometerX}");
                    //log.LogInformation($"Device:{deviceId} Device Id is:{accelerometerY}");
                    //log.LogInformation($"Device:{deviceId} Device Id is:{accelerometerZ}");
                    //log.LogInformation($"Device:{deviceId} Device Id is:{gyroscopeX}");
                    //log.LogInformation($"Device:{deviceId} Device Id is:{gyroscopeY}");
                    //log.LogInformation($"Device:{deviceId} Device Id is:{gyroscopeZ}");

                    var updateProperty = new JsonPatchDocument();
                    updateProperty.AppendReplace("/deviceid", ID.Value<string>());
                    updateProperty.AppendReplace("/humidity", humidity.Value<double>());
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
                else
                if (eventGridEvent != null && eventGridEvent.Data != null)
                {

                    JObject deviceMessage = (JObject)JsonConvert.DeserializeObject(eventGridEvent.Data.ToString());
                    log.LogInformation($"deviceMessage ::: {deviceMessage}");

                    string deviceId = (string)deviceMessage["systemProperties"]["iothub-connection-device-id"];
                    var ID = deviceMessage["body"]["deviceid"];
                    var humidity = deviceMessage["body"]["humidity"];
                    // var timeinterval = deviceMessage["body"]["timeinterval"];
                    // var temperature = deviceMessage["body"]["temperature"];
                    // var pressure = deviceMessage["body"]["pressure"];
                    // var magnetometerX = deviceMessage["body"]["magnetometerX"];
                    // var magnetometerY = deviceMessage["body"]["magnetometerY"];
                    // var magnetometerZ = deviceMessage["body"]["magnetometerZ"];
                    // var accelerometerX = deviceMessage["body"]["accelerometerX"];
                    // var accelerometerY = deviceMessage["body"]["accelerometerY"];
                    // var accelerometerZ = deviceMessage["body"]["accelerometerZ"];
                    // var gyroscopeX = deviceMessage["body"]["gyroscopeX"];
                    // var gyroscopeY = deviceMessage["body"]["gyroscopeY"];
                    // var gyroscopeZ = deviceMessage["body"]["gyroscopeZ"];

                    log.LogInformation($"Device:{deviceId} Device Id is:{ID}");
                    log.LogInformation($"Device:{deviceId} humidity is:{humidity}");
                    // log.LogInformation($"Device:{deviceId} Time interval is:{timeinterval}");
                    // log.LogInformation($"Device:{deviceId} temperature is:{temperature}");
                    // log.LogInformation($"Device:{deviceId} pressure is:{pressure}");
                    // log.LogInformation($"Device:{deviceId} magnetometerX is:{magnetometerX}");
                    // log.LogInformation($"Device:{deviceId} magnetometerY is:{magnetometerY}");
                    // log.LogInformation($"Device:{deviceId} magnetometerZ is:{magnetometerZ}");
                    // log.LogInformation($"Device:{deviceId} accelerometerX is:{accelerometerX}");
                    // log.LogInformation($"Device:{deviceId} accelerometerY is:{accelerometerY}");
                    // log.LogInformation($"Device:{deviceId} accelerometerZ is:{accelerometerZ}");
                    // log.LogInformation($"Device:{deviceId} gyroscopeX is:{gyroscopeX}");
                    // log.LogInformation($"Device:{deviceId} gyroscopeY is:{gyroscopeY}");
                    // log.LogInformation($"Device:{deviceId} gyroscopeZ is:{gyroscopeZ}");
                    var updateProperty = new JsonPatchDocument();
                    var turbineTelemetry = new Dictionary<string, Object>()
                    {
                        ["deviceid"] = ID,
                        ["humidity"] = humidity,
                        // ["timeinterval"] = timeinterval,
                        // ["temperature"] = temperature,
                        // ["pressure"] = pressure,
                        // ["magnetometerX"] = magnetometerX,
                        // ["magnetometerY"] = magnetometerY,
                        // ["magnetometerZ"] = magnetometerZ,
                        // ["accelerometerX"] = accelerometerX,
                        // ["accelerometerY"] = accelerometerY,
                        // ["accelerometerZ"] = accelerometerZ,
                        // ["gyroscopeX"] = gyroscopeX,
                        // ["gyroscopeY"] = gyroscopeY,
                        // ["gyroscopeZ"] = gyroscopeZ
                    };
                    updateProperty.AppendAdd("/deviceid", ID.Value<string>());

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