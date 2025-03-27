using System;

public class EmptyBatteryException : Exception
{
    public EmptyBatteryException(string message) : base(message) { }
}