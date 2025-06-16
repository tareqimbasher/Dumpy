#if NETCOREAPP3_0_OR_GREATER

using Dumpy.Html.Utils;
using System.Text.Json;
using System;
using System.Text.Json.Nodes;

namespace Dumpy.Html.Converters;

public class JsonDocumentHtmlConverter : HtmlConverter<JsonDocument>
{
    public override void Convert(
        ref ValueStringBuilder writer,
        JsonDocument? value,
        Type targetType,
        HtmlDumpOptions options
    ) => JsonHtmlConverter.WriteFromJsonDocument(ref writer, value, options);
}

public class JsonElementHtmlConverter : HtmlConverter<JsonElement>
{
    public override void Convert(
        ref ValueStringBuilder writer,
        JsonElement value,
        Type targetType,
        HtmlDumpOptions options
    ) => JsonHtmlConverter.WriteFromJsonElement(ref writer, value, options);
}

public class JsonNodeHtmlConverter : HtmlConverter<JsonNode>
{
    public override void Convert(
        ref ValueStringBuilder writer,
        JsonNode? value,
        Type targetType,
        HtmlDumpOptions options
    ) => JsonHtmlConverter.WriteFromJsonNode(ref writer, value, options);
}

internal static class JsonHtmlConverter
{
    private static readonly JsonSerializerOptions _serializerOptions = new() { WriteIndented = true };

    public static void WriteFromJsonDocument(ref ValueStringBuilder writer, JsonDocument? doc, HtmlDumpOptions options)
    {
        if (doc is null)
        {
            writer.WriteNullHtml(options);
            return;
        }

        WriteFromJsonElement(ref writer, doc.RootElement, options);
    }
    
    public static void WriteFromJsonElement(ref ValueStringBuilder writer, JsonElement element, HtmlDumpOptions options)
    {
        var json = element.ValueKind == JsonValueKind.Undefined
            ? ""
            : JsonSerializer.Serialize(element, _serializerOptions);

        WriteJson(ref writer, json);
    }

    public static void WriteFromJsonNode(ref ValueStringBuilder writer, JsonNode? node, HtmlDumpOptions options)
    {
        if (node is null)
        {
            writer.WriteNullHtml(options);
            return;
        }

        var json = node.ToJsonString(_serializerOptions);
        WriteJson(ref writer, json);
    }
    
    private static void WriteJson(ref ValueStringBuilder writer, string json)
    {
        writer.WriteOpenTag("pre");
        writer.WriteOpenTag("code", "language=\"json\"");
        writer.Append(HtmlUtil.EscapeText(json));
        writer.WriteCloseTag("code");
        writer.WriteCloseTag("pre");
    }
}
#endif