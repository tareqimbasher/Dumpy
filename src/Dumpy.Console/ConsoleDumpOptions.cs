using System.Collections.Generic;
using Dumpy.Console.Converters;
using Spectre.Console;

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
    
    /// <summary>
    /// Styling options.
    /// </summary>
    public StyleOptions StyleOptions { get; set; } = new();
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

/// <summary>
/// Options that affect the appearance of the output.
/// </summary>
public class StyleOptions
{
    public Style TitleTextStyle { get; set; } = new Style(decoration: Decoration.Bold | Decoration.Dim);
    public Style HeaderTextStyle { get; set; } = new Style(decoration: Decoration.Bold);
    public TableBorder TableBorder { get; set; } = TableBorder.Rounded;
    public Style BorderStyle { get; set; } = new Style(Color.PaleTurquoise4);
}