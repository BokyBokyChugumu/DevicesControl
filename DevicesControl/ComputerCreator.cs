namespace DevicesControl;

public class ComputerCreator : IDeviceCreator
{
    public string DeviceTypePrefix => "P";

    public Device CreateDevice(string[] parts)
    {
        if (parts.Length > 4 || parts.Length < 3) return null;
        string deviceId = parts[0];
        string name = parts[1];
        bool pcTurnedOn;
        if (!bool.TryParse(parts[2], out pcTurnedOn)) return null;
        string os = parts.Length == 4 && parts[3] != "null" ? parts[3] : null;
        return new PersonalComputer(deviceId, name, pcTurnedOn, os);
    }
}