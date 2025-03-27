﻿namespace TestProject1;

public static class DeviceManagerTestExtensions
{
    // Extension method to get device by ID (moved to static class)
    public static Device? GetDeviceById(this DeviceManager manager, string id) // Changed id type to string
    {
        var fieldInfo = typeof(DeviceManager).GetField("devices", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (fieldInfo != null)
        {
            var devicesList = fieldInfo.GetValue(manager) as List<Device>;
            return devicesList?.Find(d => d.Id == id);
        }
        return null; // Or throw exception if something is very wrong
    }
}