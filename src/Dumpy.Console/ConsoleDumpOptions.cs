using System.Collections.Generic;
using Dumpy.Console.Converters;

namespace Dumpy.Console;

public partial class ConsoleDumpOptions : DumpOptions
{
    /// <summary>
    /// The list of custom Console Converters to use during serialization.
    /// </summary>
    public List<ConsoleConverter> Converters { get; set; } = new();
}