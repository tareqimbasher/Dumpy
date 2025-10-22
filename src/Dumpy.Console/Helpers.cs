using Spectre.Console;

namespace Dumpy.Console;

internal static class Helpers
{
    public static Table CreateTable(this ConsoleDumpOptions options)
    {
        return new Table
        {
            ShowHeaders = options.Tables.ShowHeaders,
            ShowRowSeparators = options.Tables.ShowRowSeparators,
            Expand = options.Tables.Expand,
            Border = options.Styles.TableBorder,
            BorderStyle = options.Styles.BorderStyle
        };
    }
}