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
    public TableOptions Tables { get; set; } = new();

    /// <summary>
    /// Styling options.
    /// </summary>
    public StyleOptions Styles { get; set; } = new();
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
    public static StyleOptions Plain { get; } = new()
    {
        Border = Style.Plain,
        TableHeaderText = Style.Plain,
        TableTitleText = Style.Plain,
        TableBorderType = TableBorder.Ascii,
        String = Style.Plain,
        Char = Style.Plain,
        Boolean = Style.Plain,
        Enum = Style.Plain,
        Guid = Style.Plain,
        DateAndTime = Style.Plain,
        Numeric = Style.Plain,
        Null = Style.Plain,
        EmptyCollection = Style.Plain
    };

    public Style Border { get; set; } = new Style(Color.PaleTurquoise4);
    public Style TitleText { get; set; } = new Style(decoration: Decoration.Bold | Decoration.Underline);

    public Style TableTitleText { get; set; } = new Style(decoration: Decoration.Bold | Decoration.Dim);
    public Style TableHeaderText { get; set; } = new Style(decoration: Decoration.Bold);
    public TableBorder TableBorderType { get; set; } = TableBorder.Rounded;

    public Style String { get; set; } = new Style(Color.LightSalmon1);
    public Style Char { get; set; } = new Style(Color.LightSalmon1);
    public Style Boolean { get; set; } = new Style(Color.Cyan1);
    public Style Enum { get; set; } = new Style(Color.Yellow4_1);
    public Style Guid { get; set; } = new Style(Color.Plum3);
    public Style DateAndTime { get; set; } = new Style(Color.Gold3);
    public Style Numeric { get; set; } = new Style(Color.SkyBlue2);

    public Style Null { get; set; } = new Style(decoration: Decoration.Dim);
    public Style EmptyCollection { get; set; } = new Style(decoration: Decoration.Dim | Decoration.Bold);
}