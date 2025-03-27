namespace DevicesControl;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

public class FileDeviceFactory : IDeviceFactory
{
    private const int MaxDevices = 15;
    private readonly Dictionary<string, IDeviceCreator> _deviceCreators; // Словарь для регистрации

    public FileDeviceFactory()
    {
        // Регистрация фабрик устройств в конструкторе
        _deviceCreators = new Dictionary<string, IDeviceCreator>
        {
            { "SW", new SmartwatchCreator() },
            { "P", new ComputerCreator() },
            { "ED", new EmbeddedDeviceCreator() }
        };
    }

    public List<Device> LoadDevices(string filePath)
    {
        List<Device> devices = new List<Device>();

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException(filePath);
        }

        Console.WriteLine("Loading devices from file using FileDeviceFactory (Strategy Pattern)...");
        var lines = File.ReadAllLines(filePath);

        foreach (var line in lines)
        {
            try
            {
                if (devices.Count >= MaxDevices) break;

                var parts = line.Split(',', StringSplitOptions.TrimEntries);
                if (parts.Length < 2) continue;

                string deviceId = parts[0];
                string typePrefix = deviceId.Split('-')[0];

                if (_deviceCreators.TryGetValue(typePrefix, out var creator)) // Используем словарь для поиска фабрики
                {
                    Device device = creator.CreateDevice(parts);
                    if (device != null)
                    {
                        devices.Add(device);
                    }
                }
                else
                {
                    Console.WriteLine($"Неизвестный тип устройства: {typePrefix} в строке: {line}");
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