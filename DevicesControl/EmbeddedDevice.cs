using System;
using System.Text.RegularExpressions;

public class EmbeddedDevice : Device
{
    private static readonly Regex IpRegex = new Regex(@"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$");

    public string IPAddress { get; private set; }
    public string NetworkName { get; private set; }

    public EmbeddedDevice(string id, string name, bool isTurnedOn, string ip, string networkName) // Changed Id type to string
        : base(id, name, isTurnedOn)
    {
        if (!IpRegex.IsMatch(ip))
            throw new ArgumentException("Invalid IP address format.");

        IPAddress = ip;
        NetworkName = networkName;
    }

    public void Connect()
    {
        if (!NetworkName.Contains("MD Ltd."))
            throw new ConnectionException("Connection failed. Invalid network.");
        Console.WriteLine($"{Name} successfully connected to {NetworkName}.");
    }

    public override void TurnOn()
    {
        try
        {
            Connect();
            base.TurnOn();
        }
        catch (ConnectionException ex)
        {
            Console.WriteLine($"Connection error: {ex.Message} (Device: {Name})");
        }
    }

    public override string GetSaveFormat() => $"{Id},{Name},{IsTurnedOn},{IPAddress},{NetworkName}";
}