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
}