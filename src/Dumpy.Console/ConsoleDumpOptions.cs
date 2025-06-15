using System;
using System.Collections.Generic;
using Dumpy.Console.Converters;

namespace Dumpy.Console;

public partial class ConsoleDumpOptions : DumpOptions
{
    public static Action<ConsoleDumpOptions> ConfigureDefaultOptions { get; set; } = _ => { };

    public TableOptions TableOptions { get; set; } = new();
    
    /// <summary>
    /// The list of custom Console Converters to use during serialization.
    /// </summary>
    public List<ConsoleConverter> Converters { get; set; } = new();
}

public class TableOptions
{
    public bool ShowTitles { get; set; } = true;
    public bool ShowHeaders { get; set; } = true;
}