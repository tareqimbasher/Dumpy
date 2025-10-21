using System.Diagnostics.CodeAnalysis;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Dumpy;
using Dumpy.Console;
using Dumpy.Html;

public static class Setup
{
    static Setup()
    {
        // ConfigureDumpy.Update(options =>
        // {
        //     options.UseRemoteViewer(new Uri("ws://localhost:5689"));
        // });
    }

    public static T? DumpInternal<T>(this T? value, string? title = null, DumpOptions? options = null)
    {
        value.DumpConsole(title, new ConsoleDumpOptions()
        {
            IncludeFields = true
        });
        //value.DumpHtml();
        //value.DumpToHtmlViewer();
        return value;
    }

    [return: NotNullIfNotNull("value")]
    public static T? DumpToHtmlViewer<T>(this T? value, HtmlDumpOptions? options = null)
    {
        var html = HtmlDumper.DumpHtml(value, options);
        SendToHtmlViewer(html);

        return value;
    }

    private static void SendToHtmlViewer(string html)
    {
        html = JsonSerializer.Serialize(html); // to convert it to a JSON string
        var json = $"{{\"type\": \"data\", \"data\":{html}}}";

        using ClientWebSocket ws = new();
        ws.ConnectAsync(new Uri("ws://localhost:5689/"), CancellationToken.None).Wait();
        ws.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(json)), WebSocketMessageType.Text, true, CancellationToken.None).Wait();
    }
}