﻿namespace DevicesControl;

using System;

class Program
{
    static void Main()
    {
        string filePath = "D:\\study\\CSharp\\DevicesControl\\DevicesControl\\devices.txt";
        IDeviceFactory factory = new FileDeviceFactory(); 
        DeviceManager manager = DeviceManager.LoadFromFile(filePath, factory);

        Console.WriteLine("Devices Loaded:");
        manager.ShowAllDevices();

        Console.WriteLine("\nTurning on devices...");
        manager.TurnOnDevice("SW-1"); 
        manager.TurnOnDevice("P-1");  
        manager.TurnOnDevice("P-2");
        manager.TurnOnDevice("ED-1");

        manager.SaveToFile(filePath);
    }
}