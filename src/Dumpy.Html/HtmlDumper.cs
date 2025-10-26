using System;
using Dumpy.Utils;

namespace Dumpy.Html;

/// <summary>
/// Provides extension methods for dumping objects to HTML strings.
/// </summary>
public static class HtmlDumper
{
    /// <summary>
    /// Default options used when no <see cref="HtmlDumpOptions"/> are provided.
    /// </summary>
    private static readonly HtmlDumpOptions _defaultOptions = new();

    /// <summary>
    /// Dumps the specified value to an HTML string using the provided options.
    /// </summary>
    /// <remarks>
    /// This method is an overload of DumpHtml().
    /// </remarks>
    /// <typeparam name="T">The static type of the value.</typeparam>
    /// <param name="value">The value to dump.</param>
    /// <param name="options">Optional dump options. If null, default options are used.</param>
    /// <returns>An HTML string representing the value.</returns>
    public static string Dump<T>(this T? value, HtmlDumpOptions? options = null) => DumpHtml(value, options);
    
    /// <summary>
    /// Dumps the specified value to an HTML string using the provided options.
    /// </summary>
    /// <typeparam name="T">The static type of the value.</typeparam>
    /// <param name="value">The value to dump.</param>
    /// <param name="options">Optional dump options. If null, default options are used.</param>
    /// <returns>An HTML string representing the value.</returns>
    public static string DumpHtml<T>(this T? value, HtmlDumpOptions? options = null)
    {
        var valueType = value?.GetType() ?? typeof(T);
        return DumpHtml(value, valueType, options);
    }

    /// <summary>
    /// Dumps the specified value to an HTML string using the provided runtime type and options.
    /// </summary>
    /// <typeparam name="T">The static type of the value.</typeparam>
    /// <param name="value">The value to dump.</param>
    /// <param name="valueType">The runtime type to use for converter selection.</param>
    /// <param name="options">Optional dump options. If null, default options are used.</param>
    /// <returns>An HTML string representing the value.</returns>
    public static string DumpHtml<T>(this T? value, Type valueType, HtmlDumpOptions? options = null)
    {
        options ??= _defaultOptions;

        var writer = new ValueStringBuilder(stackalloc char[512], 4096);

        try
        {
            DumpHtml(ref writer, value, valueType, options);
        }
        catch
        {
            writer.Dispose();
            throw;
        }

        return writer.ToString();
    }

    /// <summary>
    /// Writes the HTML representation of the specified value into the provided writer.
    /// </summary>
    /// <typeparam name="T">The static type of the value.</typeparam>
    /// <param name="writer">The destination buffer to write HTML into.</param>
    /// <param name="value">The value to dump.</param>
    /// <param name="valueType">The runtime type to use for converter selection.</param>
    /// <param name="options">Dump options to control formatting and behaviors such as max depth and reference handling.</param>
    public static void DumpHtml<T>(ref ValueStringBuilder writer, T? value, Type valueType, HtmlDumpOptions options)
    {
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
                writer.WriteMaxDepthReachedHtml(options);
                return;
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
                        // Do not serialize anything
                        return;
                    case ReferenceLoopHandling.IgnoreAndSerializeCyclicReference:
                        writer.WriteCyclicReferenceHtml(options);
                        return;
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
                converter.ConvertInner(ref writer, value, valueType, options);
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