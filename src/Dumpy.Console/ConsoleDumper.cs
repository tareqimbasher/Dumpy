using System;
using System.Diagnostics.CodeAnalysis;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Dumpy.Console;

public static class ConsoleDumper
{
    private static readonly Lazy<ConsoleDumpOptions> _defaultOptions = new(() => new ConsoleDumpOptions());

    [return: NotNullIfNotNull("value")]
    public static T? DumpConsole<T>(this T? value, string? title = null, ConsoleDumpOptions? options = null)
    {
        if (title != null)
        {
            var rule = new Rule($"[bold][lightgoldenrod2_2]{title}[/][/]");
            rule.LeftJustified();
            rule.RuleStyle("darkorange");
            AnsiConsole.Write(rule);
        }

        var renderable = DumpToRenderable(value, options);
        AnsiConsole.Write(renderable);
        AnsiConsole.WriteLine();
        return value;
    }

    public static IRenderable DumpToRenderable<T>(this T? value, ConsoleDumpOptions? options = null)
    {
        options ??= _defaultOptions.Value;

        var valueType = value?.GetType() ?? typeof(T);

        return DumpToRenderable(value, valueType, options);
    }

    public static IRenderable DumpToRenderable<T>(this T? value, Type valueType, ConsoleDumpOptions? options = null)
    {
        options ??= _defaultOptions.Value;
        
        var converter = options.GetConverter(valueType);
        return converter.ConvertInner(value, valueType, options);
    }
}