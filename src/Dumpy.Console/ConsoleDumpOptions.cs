using System.Collections.Generic;
using Dumpy.Console.Converters;

namespace Dumpy.Console;

/// <summary>
/// Provides options to use with <see cref="ConsoleDumper"/>.
/// </summary>
public sealed partial class ConsoleDumpOptions : DumpOptions
{
    /// <summary>
    /// The list of user-defined converters that were registered.
    /// </summary>
    public List<ConsoleConverter> Converters { get; set; } = new();

    /// <summary>
    /// Options related to how tables are rendered.
    /// </summary>
    public TableOptions TableOptions { get; set; } = new();
}

/// <summary>
/// Provides options related to how tables are rendered.
/// </summary>
public class TableOptions
{
    /// <summary>
    /// If true, shows table titles. Defaults to true.
    /// </summary>
    public bool ShowTitles { get; set; } = true;
    /// <summary>
    /// If true, shows table headers. Defaults to true.
    /// </summary>
    public bool ShowHeaders { get; set; } = true;
    /// <summary>
    /// If true, shows table row separators. Defaults to true.
    /// </summary>
    public bool ShowRowSeparators { get; set; } = true;
    /// <summary>
    /// If true, expands a tables width to fill available space. Defaults to false.
    /// </summary>
    public bool Expand { get; set; }
}