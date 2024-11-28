using System.Diagnostics;
using System.Text.Json;
using Dumpy;
using Dumpy.ConsoleApp;
using Spectre.Console;

var r = Enumerable.Range(0, 10).Select(x => new Car()).ToArray().DumpConsole();
AnsiConsole.Write(r);
Console.WriteLine();

var cars = Enumerable.Range(0, 50000).Select(x => new Car()).ToArray();

Benchmark($"Dumpy: {cars.Length} cars", () => cars.DumpHtml());
Benchmark($"STJ  : {cars.Length} cars", () => JsonSerializer.Serialize(cars));

void Benchmark(string operation, Func<string> action)
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