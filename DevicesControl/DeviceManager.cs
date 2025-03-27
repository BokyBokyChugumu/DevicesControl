using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

public class DeviceManager
{
    private List<Device> devices = new List<Device>();
    private const int MaxDevices = 15;

    public DeviceManager(string filePath)
    {
        if (File.Exists(filePath))
        {
            LoadFromFile(filePath);
        }
        else
        {
            throw new FileNotFoundException(filePath);
        }
    }

    private void LoadFromFile(string filePath)
    {
        Console.WriteLine("Loading devices from file...");
        var lines = File.ReadAllLines(filePath);

        foreach (var line in lines)
        {
            try
            {
                if (devices.Count >= MaxDevices) break;

                var parts = line.Split(',', StringSplitOptions.TrimEntries);
                if (parts.Length < 2) continue;

                // Removed Regex.Match and int.TryParse for ID. Using parts[0] directly as ID string.
                string deviceId = parts[0];
                string type = deviceId.Split('-')[0]; // Extract type like "SW", "P", "ED"
                string name = parts[1];

                switch (type)
                {
                    case "SW":
                        if (parts.Length != 4) continue;
                        bool swTurnedOn;
                        if (!bool.TryParse(parts[2], out swTurnedOn)) continue;
                        if (!int.TryParse(parts[3].TrimEnd('%'), out int battery) || battery < 0 || battery > 100) continue;
                        devices.Add(new Smartwatch(deviceId, name, swTurnedOn, battery)); // Using deviceId as string ID
                        break;

                    case "P":
                        if (parts.Length > 4 || parts.Length < 3) continue;
                        bool pcTurnedOn;
                        if (!bool.TryParse(parts[2], out pcTurnedOn)) continue;
                        string os = parts.Length == 4 && parts[3] != "null" ? parts[3] : null;
                        devices.Add(new PersonalComputer(deviceId, name, pcTurnedOn, os)); // Using deviceId as string ID
                        break;

                    case "ED":
                        if (parts.Length != 5) continue;
                        bool edTurnedOn;
                        if (!bool.TryParse(parts[2], out edTurnedOn)) continue;
                        string ip = parts[3];
                        string networkName = parts[4];
                        if (!Regex.IsMatch(ip, @"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$")) continue;
                        devices.Add(new EmbeddedDevice(deviceId, name, edTurnedOn, ip, networkName)); // Using deviceId as string ID
                        break;

                    default:
                        continue;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при разборе строки: {line} - {ex.Message}");
            }
        }
    }

    
    public void EditDevice(string id, string propertyName, object newValue) // Changed id type to string
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

    public void RemoveDevice(string id) => devices.RemoveAll(d => d.Id == id); // Changed id type to string

    public void TurnOnDevice(string id)
    {
        var device = devices.Find(d => d.Id == id);
        if (device != null)
        {
            try { device.TurnOn(); }
            catch (Exception ex) { Console.WriteLine($"Error: {ex.Message}. Device ID: " + id); }
        }
    }

    public void TurnOffDevice(string id) => devices.Find(d => d.Id == id)?.TurnOff(); // Changed id type to string

    public void ShowAllDevices()
    {
        foreach (var device in devices)
            Console.WriteLine(device.GetSaveFormat());
    }
}
