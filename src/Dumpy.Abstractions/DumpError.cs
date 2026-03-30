namespace Dumpy;

/// <summary>
/// Sentinel value returned when reading a property or field value fails during serialization.
/// </summary>
public sealed class DumpError
{
    public static readonly DumpError Instance = new();

    private DumpError()
    {
    }
}