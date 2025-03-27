using System;

class ConnectionException : Exception
{
    public ConnectionException(string message) : base(message) { }
}