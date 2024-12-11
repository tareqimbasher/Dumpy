using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Dumpy.ConsoleApp;
using Dumpy.Html;
using Dumpy;


// new Car().DumpConsole();
// Enumerable.Range(0, 3).Select(x => new Car()).ToArray().DumpConsole();
// Console.WriteLine();

// new Car().Dump();
// Enumerable.Range(0, 3).Select(x => new Car()).ToArray().Dump();




public static class Exts
{
    // var cars = Enumerable.Range(0, 50000).Select(x => new Car()).ToArray();
    // Benchmark($"Dumpy: {cars.Length} cars", () => cars.DumpHtml());
    // Benchmark($"STJ  : {cars.Length} cars", () => JsonSerializer.Serialize(cars));

    public static void Benchmark(string operation, Func<string> action)
    {
        var sw = Stopwatch.StartNew();
        var result = action();
        sw.Stop();
        Console.WriteLine($"{operation}:" +
                          $"\n   time: {sw.ElapsedMilliseconds}ms" +
                          $"\n   length: {result.Length:N0}" +
                          $"\n   size: {result.Length * sizeof(char):N0} bytes" +
                          $"\n");
    }

    public static T? Dump<T>(this T? value, string? title = null, HtmlDumpOptions? options = null)
    {
        value.DumpConsole();
        //SendToVsExt(value.DumpHtml(options));
        return value;
    }

    // public static void UseRemoveViewer(this DumpyConfig config, string wsUri)
    // {

    // }

    private static void SendToVsExt(string html)
    {
        html = JsonSerializer.Serialize(html); // to convert it to a JSON string
        var json = $$"""
                     {"type": "data", "data":{{html}}}
                     """;

        using ClientWebSocket ws = new();
        ws.ConnectAsync(new Uri("ws://localhost:5689/"), CancellationToken.None).Wait();
        ws.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(json)), WebSocketMessageType.Text, true, CancellationToken.None).Wait();
    }
}