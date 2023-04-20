/// <summary>
/// Holds data for telemetry
/// </summary>
public class TelemetryData
{
    public string deviceid;
    public string TimeInterval;
    public double humidity;
    public double temperature;
    public double pressure;
    public double magnetometerX;
    public double magnetometerY;
    public double magnetometerZ;
    public double accelerometerX;
    public double accelerometerY;
    public double accelerometerZ;
    public double gyroscopeX;
    public double gyroscopeY;
    public double gyroscopeZ;

    public TelemetryData() { }
    
	public TelemetryData(TelemetryData data)
    {
        deviceid = data.deviceid;
        TimeInterval = data.TimeInterval;
        humidity = data.humidity;
        temperature = data.temperature;
        pressure = data.pressure;
        magnetometerX = data.magnetometerX;
        magnetometerY = data.magnetometerY;
        magnetometerZ = data.magnetometerZ;
        accelerometerX = data.accelerometerX;
        accelerometerY = data.accelerometerY;
        accelerometerZ = data.accelerometerZ;
        gyroscopeX = data.gyroscopeX;
        gyroscopeY = data.gyroscopeY;
        gyroscopeZ = data.gyroscopeZ;
    }
}
