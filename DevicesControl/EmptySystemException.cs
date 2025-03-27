using System;

public class EmptySystemException : Exception
{
    public EmptySystemException(string message) : base(message) { }
}