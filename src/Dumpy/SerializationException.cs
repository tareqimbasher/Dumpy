using System;

namespace Dumpy;

/// <summary>
/// Thrown as a result of an unexpected stoppage during serialization.
/// </summary>
public class SerializationException(string message) : Exception(message);