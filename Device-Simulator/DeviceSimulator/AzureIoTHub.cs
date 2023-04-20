using Azure.Messaging.EventHubs.Consumer;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Common.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;

namespace DeviceSimulator
{
    public static class AzureIoTHub
    {
        /// <summary>
        /// Please replace with correct connection string value
        /// The connection string could be got from Azure IoT Hub -> Shared access policies -> iothubowner -> Connection String:
        /// </summary>
        private const string iotHubConnectionString = "HostName=adtwindHubkpku2rzpus.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=lDuGL07V2eEf5v5vKTqVyzp6TQ8w7ATZLQ3bKIrFkX8=";
        private const string adtInstanceUrl = "https://adtwindadtkpku2rzpus.api.eus.digitaltwins.azure.net";
        //private const string alertTurbineId = "T102";
        //private const string alertVariableName = "Alert";
        //private const string alertDescription = "Light icing (rotor bl. ice sensor)";
        //private const double alertTemp = -6.0D;
        //private const double alertPower = 200.0D;
        //private const double alertWindSpeed = 7.0D;
        //private const double alertRotorSpeed = 1.4D;
        //private const double alertTempVariance = 1.0D;
        //private const double alertPowerVariance = 45D;
        //private const double alertWindSpeedVariance = 0.40D;
        //private const double alertRotorVariance = .10D;
        //private const int alertCode = 400;
        private static List<string> deviceConnectionStrings;
        //private static bool alertSent = false;
        //private static int alertIndex;

        public static async Task<List<Device>> CreateDeviceIdentitiesAsyc(List<string> deviceIds)
        {
            var registryManager = RegistryManager.CreateFromConnectionString(iotHubConnectionString);
            List<Device> devices = new List<Device>();
            deviceConnectionStrings = new List<string>();
            for (int i = 0; i < deviceIds.Count; i++)
            {
                var device = new Device(deviceIds[i]);
                device = await CreateOrGetDevice(registryManager, device);
                devices.Add(device);
                //if (device.Id == alertTurbineId)
                //{
                //    alertIndex = i;
                //}
                deviceConnectionStrings.Add(CreateConnectionString(device));
            }
            return devices;
        }

        private static string CreateConnectionString(Device device)
        {
            string connectionString = string.Format("{0};DeviceId={1};SharedAccessKey={2}", iotHubConnectionString.Split(';')[0], device.Id.ToString(), device.Authentication.SymmetricKey.PrimaryKey.ToString());
            return connectionString;
        }

        private static async Task<Device> CreateOrGetDevice(RegistryManager registryManager, Device device)
        {
            try
            {
                Device createdDevice = await registryManager.AddDeviceAsync(device);
                Console.WriteLine("Adding device " + device.Id);
                return createdDevice;
            }
            catch (DeviceAlreadyExistsException)
            {
                Console.WriteLine("Retrieved device " + device.Id);
                return await registryManager.GetDeviceAsync(device.Id);
            }
        }

        private static readonly HttpClient client = new HttpClient();

        //private static async Task GetStrapiToken()
        //{
        //    client.DefaultRequestHeaders.Accept.Clear();
        //    client.DefaultRequestHeaders.Accept.Add(
        //new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
        //    client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

        //    var jsonString = "{\"identifier\":\"thiennam209@gmail.com\",\"password\":\"Metaverse123@\" }";
        //    var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

        //    var token = await client.PostAsync("http://20.119.54.193:1337/api/auth/local", httpContent);
        //    string json = await token.Content.ReadAsStringAsync();
        //    var data = (JObject)JsonConvert.DeserializeObject(json);
        //    string JWT = data["jwt"].Value<string>();

        //    client.DefaultRequestHeaders.Authorization =
        //new AuthenticationHeaderValue("Bearer", JWT);
        //}

        //private static async Task<String> GetWindTurbineAlertTrigger()
        //{
        //    var stringTask = client.GetStringAsync("http://20.119.54.193:1337/api/wind-turbine-alert-triggers/1");
        //    var msg = await stringTask;

        //    return msg;
        //}

        //private static void CancelWindTurbineAlertTrigger()
        //{
        //    var jsonString = "{\"data\":{\"isCancelAlert\":\"false\"}}";
        //    var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
        //    var stringTask = client.PutAsync("http://20.119.54.193:1337/api/wind-turbine-alert-triggers/1", httpContent);
        //}

        public static async Task SendDeviceToCloudMessageAsync(CancellationToken cancelToken)
        {
            List<DeviceClient> deviceClients = new List<DeviceClient>();
            foreach (string deviceConnectionString in deviceConnectionStrings)
            {
                //use connection string to create a device client
                deviceClients.Add(DeviceClient.CreateFromConnectionString(deviceConnectionString));
            }

            //await GetStrapiToken();

            List<TelemetryData> data = Telemetry.GetDataLines();
            int dataIterator = 0;
            while (!cancelToken.IsCancellationRequested)
            {
                //var WindTurbineAlertTrigger = await GetWindTurbineAlertTrigger();
                //var apiData = JObject.Parse(WindTurbineAlertTrigger)["data"]["attributes"];

                //string isCancelAlert = apiData["isCancelAlert"].ToString();

                //if (isCancelAlert == "True")
                //{
                //    await SendAlert(isCancelAlert);
                //    CancelWindTurbineAlertTrigger();
                //}

                for (int i = 0; i < deviceClients.Count; i++)
                {
                    Microsoft.Azure.Devices.Client.Message message = new Microsoft.Azure.Devices.Client.Message();
                    message = ConstructTelemetryDataPoint(data[i + dataIterator]);
                    //if (alertSent && data[i + dataIterator].turbineId == alertTurbineId)
                    //{
                    //    // This is sending a specified Alert message
                    //    message = ConstructTelemetryDataPoint(data[i + dataIterator], isAlert: true);
                    //}
                    //else
                    //{
                    //    //Basic telemetry message without alert
                    //    message = ConstructTelemetryDataPoint(data[i + dataIterator], isAlert: false);
                    //}
                    await deviceClients[i].SendEventAsync(message);
                }
                if (dataIterator < data.Count - deviceClients.Count)
                {
                    dataIterator += deviceClients.Count;
                }
                else
                {
                    // Console.WriteLine("Press any key to restart the data sequence");
                    // Console.ReadKey();
                    dataIterator = 0;
                }
                await Task.Delay(2000);
                //Keep this value above 1000 to keep a safe buffer above the ADT service limits
                //See https://aka.ms/adt-limits for more info
            }
        }

        private static Microsoft.Azure.Devices.Client.Message ConstructTelemetryDataPoint(TelemetryData data)
        {
            Random rand = new Random();
            TelemetryData telData = new TelemetryData(data);
            //if (isAlert)
            //{
            //    telData.eventCodeDescription = alertDescription;
            //    telData.eventCode = alertCode;
            //    telData.windSpeed = alertWindSpeed + (alertWindSpeedVariance * rand.NextDouble());
            //    telData.temperature = alertTemp + (alertTempVariance * rand.NextDouble());
            //    telData.rotorSpeed = alertRotorSpeed + (alertRotorVariance * rand.NextDouble());
            //    telData.power = alertPower + (alertPowerVariance * rand.NextDouble());
            //}

            var payload = new
            {
                DeviceId = telData.deviceid,
                TimeInterval = telData.TimeInterval,
                humidity = telData.humidity,
                temperature = telData.temperature,
                pressure = telData.pressure,
                magnetometerX = telData.magnetometerX,
                magnetometerY = telData.magnetometerY,
                magnetometerZ = telData.magnetometerZ,
                accelerometerX = telData.accelerometerX,
                accelerometerY = telData.accelerometerY,
                accelerometerZ = telData.accelerometerZ,
                gyroscopeX = telData.gyroscopeX,
                gyroscopeY = telData.gyroscopeY,
                gyroscopeZ = telData.gyroscopeZ,
            };
            var messageString = System.Text.Json.JsonSerializer.Serialize(payload);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"{DateTime.Now} > Sending message: {messageString}");
            return new Microsoft.Azure.Devices.Client.Message(Encoding.UTF8.GetBytes(messageString))
            {
                ContentType = "application/json",
                ContentEncoding = "utf-8"
            };
        }

        //public static async Task SendAlert(string isCancelAlert)
        //{
        //    try
        //    {
        //        if (isCancelAlert != "True")
        //        {
        //            var payload = new
        //            {
        //                TurbineID = alertTurbineId,
        //                Alert = !alertSent
        //            };

        //            var messageString = System.Text.Json.JsonSerializer.Serialize(payload);
        //            var client = DeviceClient.CreateFromConnectionString(deviceConnectionStrings[alertIndex]);
        //            var message = new Microsoft.Azure.Devices.Client.Message(Encoding.UTF8.GetBytes(messageString))
        //            {
        //                ContentType = "application/json",
        //                ContentEncoding = "utf-8"
        //            };

        //            await client.SendEventAsync(message);
        //            alertSent = !alertSent;
        //        }
        //        else
        //        {
        //            var payload = new
        //            {
        //                TurbineID = alertTurbineId,
        //                Alert = false
        //            };

        //            var messageString = System.Text.Json.JsonSerializer.Serialize(payload);
        //            var client = DeviceClient.CreateFromConnectionString(deviceConnectionStrings[alertIndex]);
        //            var message = new Microsoft.Azure.Devices.Client.Message(Encoding.UTF8.GetBytes(messageString))
        //            {
        //                ContentType = "application/json",
        //                ContentEncoding = "utf-8"
        //            };

        //            await client.SendEventAsync(message);
        //            alertSent = false;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e.Message);
        //    }
        //}

        public static async Task ReceiveMessagesFromDeviceAsync(CancellationToken cancelToken)
        {
            try
            {
                string eventHubConnectionString = await IotHubConnection.GetEventHubsConnectionStringAsync(iotHubConnectionString);
                await using var consumerClient = new EventHubConsumerClient(
                    EventHubConsumerClient.DefaultConsumerGroupName,
                    eventHubConnectionString);

                await foreach (PartitionEvent partitionEvent in consumerClient.ReadEventsAsync(cancelToken))
                {
                    if (partitionEvent.Data == null) continue;


                    string data = Encoding.UTF8.GetString(partitionEvent.Data.Body.ToArray());
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Message received. Partition: {partitionEvent.Partition.PartitionId} Data: '{data}'");
                }
            }
            catch (TaskCanceledException) { } // do nothing
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading event: {ex}");
            }
        }
    }
}
