using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

public class DeviceManager
{
    private List<Device> devices; // Теперь принимаем список устройств извне
    private const int MaxDevices = 15;

    // Конструктор теперь принимает список устройств
    public DeviceManager(List<Device> initialDevices)
    {
        devices = initialDevices ?? new List<Device>();
        if (devices.Count > MaxDevices)
        {
            devices = devices.Take(MaxDevices).ToList(); // Обрезаем, если загружено больше, чем MaxDevices
        }
    }


    public static DeviceManager LoadFromFile(string filePath, IDeviceFactory factory) // Статический фабричный метод
    {
        List<Device> devices = factory.LoadDevices(filePath);
        return new DeviceManager(devices);
    }


    public void EditDevice(string id, string propertyName, object newValue)
    {
        var device = devices.Find(d => d.Id == id);
        if (device == null)
        {
            throw new ArgumentException($"Device with ID {id} not found.");
        }

        Type deviceType = device.GetType();
        var property = deviceType.GetProperty(propertyName);
        if (property == null)
        {
            throw new ArgumentException($"Property '{propertyName}' not found in device type {deviceType.Name}.");
        }

        try
        {
            property.SetValue(device, Convert.ChangeType(newValue, property.PropertyType));
        }
        catch (FormatException)
        {
            throw new ArgumentException($"Invalid value format for property '{propertyName}'. Expected type: {property.PropertyType.Name}.");
        }
        catch (InvalidCastException)
        {
            throw new ArgumentException($"Invalid value type for property '{propertyName}'. Expected type: {property.PropertyType.Name}.");
        }
        catch (Exception ex)
        {
            throw new ArgumentException($"Error setting property '{propertyName}': {ex.Message}");
        }
    }

    public int GetDeviceCount() => devices.Count;
    public void SaveToFile(string filePath)
    {
        var lines = devices.Select(d => d.GetSaveFormat()).ToArray();
        File.WriteAllLines(filePath, lines);
    }

    public void AddDevice(Device device)
    {
        if (devices.Count >= MaxDevices)
            throw new InvalidOperationException("Storage is full.");
        devices.Add(device);
    }

    public void RemoveDevice(string id) => devices.RemoveAll(d => d.Id == id);

    public void TurnOnDevice(string id)
    {
        var device = devices.Find(d => d.Id == id);
        if (device != null)
        {
            try { device.TurnOn(); }
            catch (Exception ex) { Console.WriteLine($"Error: {ex.Message}. Device ID: " + id); }
        }
    }

    public void TurnOffDevice(string id) => devices.Find(d => d.Id == id)?.TurnOff();

    public void ShowAllDevices()
    {
        foreach (var device in devices)
            Console.WriteLine(device.GetSaveFormat());
    }
}