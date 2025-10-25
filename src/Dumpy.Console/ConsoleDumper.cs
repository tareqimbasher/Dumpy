using System;
using System.Diagnostics.CodeAnalysis;
using Spectre.Console;
using Spectre.Console.Rendering;
using Dumpy.Utils;

namespace Dumpy.Console;

/// <summary>
/// Provides helpers for dumping objects to the console using Spectre.Console and configured <see cref="ConsoleDumpOptions"/>.
/// </summary>
public static class ConsoleDumper
{
    /// <summary>
    /// Default options used when no <see cref="ConsoleDumpOptions"/> are provided.
    /// </summary>
    private static readonly ConsoleDumpOptions _defaultOptions = new();

    /// <summary>
    /// Writes a formatted representation of the specified value to the console.
    /// </summary>
    /// <typeparam name="T">The static type of the value.</typeparam>
    /// <param name="value">The value to dump.</param>
    /// <param name="options">Optional dump options. If null, default options are used.</param>
    /// <returns>The original <paramref name="value"/> for fluent usage.</returns>
    public static T? Dump<T>(this T? value, ConsoleDumpOptions? options = null) => DumpConsole(value, options);

    /// <summary>
    /// Writes a formatted representation of the specified value to the console.
    /// </summary>
    /// <typeparam name="T">The static type of the value.</typeparam>
    /// <param name="value">The value to dump.</param>
    /// <param name="options">Optional dump options. If null, default options are used.</param>
    /// <returns>The original <paramref name="value"/> for fluent usage.</returns>
    [return: NotNullIfNotNull("value")]
    public static T? DumpConsole<T>(this T? value, ConsoleDumpOptions? options = null)
    {
        return DumpConsole(value, title: null, options);
    }

    /// <summary>
    /// Writes a formatted representation of the specified value to the console with an optional title.
    /// </summary>
    /// <typeparam name="T">The static type of the value.</typeparam>
    /// <param name="value">The value to dump.</param>
    /// <param name="title">An optional title rendered as a Spectre.Console rule above the dump.</param>
    /// <param name="options">Optional dump options. If null, default options are used.</param>
    /// <returns>The original <paramref name="value"/> for fluent usage.</returns>
    [return: NotNullIfNotNull("value")]
    public static T? DumpConsole<T>(this T? value, string? title, ConsoleDumpOptions? options = null)
    {
        if (title != null)
        {
            AnsiConsole.Write(Markup.FromInterpolated($"{title}\n", options?.Styles.TitleText));
        }

        var renderable = DumpToRenderable(value, options);
        AnsiConsole.Write(renderable);
        AnsiConsole.WriteLine();
        return value;
    }

    /// <summary>
    /// Creates a Spectre.Console <see cref="IRenderable"/> that represents the specified value.
    /// </summary>
    /// <typeparam name="T">The static type of the value.</typeparam>
    /// <param name="value">The value to convert to a renderable.</param>
    /// <param name="options">Optional dump options. If null, default options are used.</param>
    /// <returns>An <see cref="IRenderable"/> that can be written to the console.</returns>
    public static IRenderable DumpToRenderable<T>(this T? value, ConsoleDumpOptions? options = null)
    {
        options ??= _defaultOptions;

        var valueType = value?.GetType() ?? typeof(T);

        return DumpToRenderable(value, valueType, options);
    }

    /// <summary>
    /// Creates a Spectre.Console <see cref="IRenderable"/> that represents the specified value using the provided runtime type and options.
    /// </summary>
    /// <typeparam name="T">The static type of the value.</typeparam>
    /// <param name="value">The value to convert to a renderable.</param>
    /// <param name="valueType">The runtime type to use for converter selection.</param>
    /// <param name="options">Optional dump options. If null, default options are used.</param>
    /// <returns>An <see cref="IRenderable"/> that can be written to the console.</returns>
    public static IRenderable DumpToRenderable<T>(this T? value, Type valueType, ConsoleDumpOptions? options = null)
    {
        options ??= _defaultOptions;

        bool isRoot = !DumpContext.IsActive;
        if (isRoot)
        {
            DumpContext.Reset();
        }

        try
        {
            // Enforce max depth
            if (DumpContext.Depth >= options.MaxDepth)
            {
                return new Markup("[dim](max depth)[/]");
            }

            // Reference loop handling (checked only for reference types)
            if (value != null && !valueType.IsValueType && DumpContext.IsVisited(value, valueType))
            {
                switch (options.ReferenceLoopHandling)
                {
                    case ReferenceLoopHandling.Error:
                        throw new SerializationException(
                            $"Self referencing loop detected for type {TypeUtil.GetName(valueType, true)}");
                    case ReferenceLoopHandling.Ignore:
                        return new Text(string.Empty);
                    case ReferenceLoopHandling.IgnoreAndSerializeCyclicReference:
                        return new Markup("[dim](cyclic)[/]");
                    case ReferenceLoopHandling.Serialize:
                        // proceed
                        break;
                }
            }

            bool didEnter = false;
            try
            {
                DumpContext.Enter(value, valueType);
                didEnter = true;
                var converter = options.GetConverter(valueType);
                return converter.ConvertInner(value, valueType, options);
            }
            finally
            {
                if (didEnter)
                {
                    DumpContext.Exit(value, valueType);
                }
            }
        }
        finally
        {
            if (isRoot)
            {
                DumpContext.Clear();
            }
        }
    }
}