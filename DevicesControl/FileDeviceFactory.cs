using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

public class FileDeviceFactory : IDeviceFactory
{
    private const int MaxDevices = 15;

    public List<Device> LoadDevices(string filePath)
    {
        List<Device> devices = new List<Device>();

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException(filePath);
        }

        Console.WriteLine("Loading devices from file using FileDeviceFactory...");
        var lines = File.ReadAllLines(filePath);

        foreach (var line in lines)
        {
            try
            {
                if (devices.Count >= MaxDevices) break;

                var parts = line.Split(',', StringSplitOptions.TrimEntries);
                if (parts.Length < 2) continue;

                string deviceId = parts[0];
                string type = deviceId.Split('-')[0];
                string name = parts[1];

                switch (type)
                {
                    case "SW":
                        if (parts.Length != 4) continue;
                        bool swTurnedOn;
                        if (!bool.TryParse(parts[2], out swTurnedOn)) continue;
                        if (!int.TryParse(parts[3].TrimEnd('%'), out int battery) || battery < 0 || battery > 100) continue;
                        devices.Add(new Smartwatch(deviceId, name, swTurnedOn, battery));
                        break;

                    case "P":
                        if (parts.Length > 4 || parts.Length < 3) continue;
                        bool pcTurnedOn;
                        if (!bool.TryParse(parts[2], out pcTurnedOn)) continue;
                        string os = parts.Length == 4 && parts[3] != "null" ? parts[3] : null;
                        devices.Add(new PersonalComputer(deviceId, name, pcTurnedOn, os));
                        break;

                    case "ED":
                        if (parts.Length != 5) continue;
                        bool edTurnedOn;
                        if (!bool.TryParse(parts[2], out edTurnedOn)) continue;
                        string ip = parts[3];
                        string networkName = parts[4];
                        if (!Regex.IsMatch(ip, @"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$")) continue;
                        devices.Add(new EmbeddedDevice(deviceId, name, edTurnedOn, ip, networkName));
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
        return devices;
    }
}