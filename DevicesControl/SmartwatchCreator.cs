namespace DevicesControl;

public class SmartwatchCreator : IDeviceCreator
{
    public string DeviceTypePrefix => "SW";

    public Device CreateDevice(string[] parts)
    {
        if (parts.Length != 4) return null;
        string deviceId = parts[0];
        string name = parts[1];
        bool swTurnedOn;
        if (!bool.TryParse(parts[2], out swTurnedOn)) return null;
        if (!int.TryParse(parts[3].TrimEnd('%'), out int battery) || battery < 0 || battery > 100) return null;
        return new Smartwatch(deviceId, name, swTurnedOn, battery);
    }
}