namespace DevicesControl;

public interface IDeviceCreator
{
    string DeviceTypePrefix { get; }
    Device CreateDevice(string[] parts);
}