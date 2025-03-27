using System.Collections.Generic;

public interface IDeviceFactory
{
    List<Device> LoadDevices(string filePath);
}