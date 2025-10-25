using Spectre.Console;

namespace Dumpy.Console.Widgets;

public static class NullWidget
{
    public static Markup New(ConsoleDumpOptions options) => new("null", options.Styles.Null);
}