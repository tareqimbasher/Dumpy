using Spectre.Console;

namespace Dumpy.Console;

internal static class Helpers
{
    public static Table CreateTable(this ConsoleDumpOptions options)
    {
        return new Table
        {
            ShowHeaders = options.TableOptions.ShowHeaders,
            ShowRowSeparators = options.TableOptions.ShowRowSeparators,
            Expand = options.TableOptions.Expand,
            Border = options.StyleOptions.TableBorder,
            BorderStyle = options.StyleOptions.BorderStyle
        };
    }
}