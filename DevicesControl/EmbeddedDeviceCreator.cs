using System.Text.RegularExpressions;

namespace DevicesControl;

public class EmbeddedDeviceCreator : IDeviceCreator
{
    public string DeviceTypePrefix => "ED";

    public Device CreateDevice(string[] parts)
    {
        if (parts.Length != 5) return null;
        string deviceId = parts[0];
        string name = parts[1];
        bool edTurnedOn;
        if (!bool.TryParse(parts[2], out edTurnedOn)) return null;
        string ip = parts[3];
        string networkName = parts[4];
        if (!Regex.IsMatch(ip, @"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$")) return null;
        return new EmbeddedDevice(deviceId, name, edTurnedOn, ip, networkName);
    }
}