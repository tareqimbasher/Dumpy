namespace Dumpy;

/// <summary>
/// Methods in dealing with reference loops.
/// </summary>
public enum ReferenceLoopHandling
{
    /// <summary>
    /// Throw a <see cref="SerializationException" /> when a loop is encountered.
    /// </summary>
    Error,

    /// <summary>
    /// Ignore loop references and do not serialize.
    /// </summary>
    Ignore,

    /// <summary>
    /// Ignore loop references and serialize to a cyclic reference representation.
    /// </summary>
    IgnoreAndSerializeCyclicReference,

    /// <summary>
    /// Serialize loop references.
    /// </summary>
    Serialize
}