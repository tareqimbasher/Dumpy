using System;

namespace Dumpy;

/// <summary>
/// Thrown as a result of an unexpected stoppage during serialization.
/// </summary>
public class SerializationException : Exception
{
    public SerializationException(string message) : base(message)
    {
    }
}