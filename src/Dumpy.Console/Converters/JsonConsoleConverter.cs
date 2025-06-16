#if NETCOREAPP3_0_OR_GREATER

using System.Text.Json;
using System;
using System.Text.Json.Nodes;
using Dumpy.Console.Widgets;
using Spectre.Console.Json;
using Spectre.Console.Rendering;

namespace Dumpy.Console.Converters;

public class JsonDocumentConsoleConverter : ConsoleConverter<JsonDocument>
{
    public override IRenderable Convert(
        JsonDocument? value,
        Type targetType,
        ConsoleDumpOptions options
    ) => JsonConsoleConverter.WriteFromJsonDocument(value, options);
}

public class JsonElementConsoleConverter : ConsoleConverter<JsonElement>
{
    public override IRenderable Convert(
        JsonElement value,
        Type targetType,
        ConsoleDumpOptions options
    ) => JsonConsoleConverter.WriteFromJsonElement(value, options);
}

public class JsonNodeConsoleConverter : ConsoleConverter<JsonNode>
{
    public override IRenderable Convert(
        JsonNode? value,
        Type targetType,
        ConsoleDumpOptions options
    ) => JsonConsoleConverter.WriteFromJsonNode(value, options);
}

internal static class JsonConsoleConverter
{
    private static readonly JsonSerializerOptions _serializerOptions = new() { WriteIndented = true };

    public static IRenderable WriteFromJsonDocument(JsonDocument? doc, ConsoleDumpOptions options)
    {
        if (doc is null)
        {
            return NullWidget.Instance;
        }

        return WriteFromJsonElement(doc.RootElement, options);
    }

    public static IRenderable WriteFromJsonElement(JsonElement element, ConsoleDumpOptions options)
    {
        var json = element.ValueKind == JsonValueKind.Undefined
            ? ""
            : JsonSerializer.Serialize(element, _serializerOptions);

        return WriteJson(json);
    }

    public static IRenderable WriteFromJsonNode(JsonNode? node, ConsoleDumpOptions options)
    {
        if (node is null)
        {
            return NullWidget.Instance;
        }

        var json = node.ToJsonString(_serializerOptions);
        return WriteJson(json);
    }

    private static IRenderable WriteJson(string json)
    {
        return new JsonText(json);
    }
}
#endif