using System.Collections.Generic;
using System.IO;

/// <summary>
/// Handles loading data from data file
/// </summary>
public class Telemetry
{
	private const string dataFile = "./data.csv";

	public static List<TelemetryData> GetDataLines()
    {
		using (StreamReader sr = new StreamReader(dataFile))
		{
			List<TelemetryData> allTelemetryData = new List<TelemetryData>();
			sr.ReadLine();
			while (!sr.EndOfStream)
			{
				allTelemetryData.Add(CreateTelemetryData(sr.ReadLine()));
			}
			return allTelemetryData;
		}
    }

	private static TelemetryData CreateTelemetryData(string line)
    {
		TelemetryData data = new TelemetryData();
		string[] split = line.Split(',');
		data.deviceid = split[0];
		data.TimeInterval = split[1];
		double.TryParse(split[2], out data.humidity);
		double.TryParse(split[3], out data.temperature);
		double.TryParse(split[4], out data.pressure);
		double.TryParse(split[5], out data.magnetometerX);
		double.TryParse(split[6], out data.magnetometerY);
		double.TryParse(split[7], out data.magnetometerZ);
		double.TryParse(split[8], out data.accelerometerX);
		double.TryParse(split[9], out data.accelerometerY);
		double.TryParse(split[10], out data.accelerometerZ);
		double.TryParse(split[11], out data.gyroscopeX);
		double.TryParse(split[12], out data.gyroscopeY);
		double.TryParse(split[13], out data.gyroscopeZ);
        return data;
	}
}
