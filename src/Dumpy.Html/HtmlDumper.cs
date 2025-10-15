using System;
using Dumpy.Html;
using Dumpy.Utils;

// ReSharper disable once CheckNamespace
namespace Dumpy;

public static class HtmlDumper
{
    private static readonly HtmlDumpOptions _defaultOptions = new();

    public static string DumpHtml<T>(this T? value, HtmlDumpOptions? options = null)
    {
        var valueType = value?.GetType() ?? typeof(T);
        return DumpHtml(value, valueType, options);
    }

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
            if (value != null && !valueType.IsValueType && DumpContext.IsVisited(value))
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
                DumpContext.Enter(value);
                didEnter = true;
                var converter = options.GetConverter(valueType);
                converter.ConvertInner(ref writer, value, valueType, options);
            }
            finally
            {
                if (didEnter)
                {
                    DumpContext.Exit(value);
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