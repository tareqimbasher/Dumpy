using Spectre.Console;

namespace Dumpy.Console.Widgets;

public static class NullWidget
{
    public static Markup Instance { get; } = new("[grey]null[/]");
}