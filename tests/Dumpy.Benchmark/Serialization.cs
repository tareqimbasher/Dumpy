using System.Text.Json;
using BenchmarkDotNet.Attributes;
using Dumpify;

namespace Dumpy.Benchmark;

[MemoryDiagnoser]
[MinColumn, MaxColumn]
public class Serialization
{
    private readonly Car _1 = new Car(1);
    private readonly Car[] _10 = Enumerable.Range(1, 10).Select(i => new Car(i)).ToArray();
    private readonly Car[] _100 = Enumerable.Range(1, 100).Select(i => new Car(i)).ToArray();
    private readonly Car[] _1000 = Enumerable.Range(1, 1000).Select(i => new Car(i)).ToArray();
    private readonly Car[] _10000 = Enumerable.Range(1, 10000).Select(i => new Car(i)).ToArray();

    [Benchmark]
    public string Json1() => JsonSerializer.Serialize(_1);

    [Benchmark]
    public string Html1() => HtmlDumper.DumpHtml(_1);
    
    [Benchmark]
    public string Dumpify1() => DumpExtensions.DumpText(_1);

    [Benchmark]
    public string Json10() => JsonSerializer.Serialize(_10);
    
    [Benchmark]
    public string Html10() => HtmlDumper.DumpHtml(_10);
    
    [Benchmark]
    public string Dumpify10() => DumpExtensions.DumpText(_10);
    
    [Benchmark]
    public string Json100() => JsonSerializer.Serialize(_100);
    
    [Benchmark]
    public string Html100() => HtmlDumper.DumpHtml(_100);
    
    [Benchmark]
    public string Dumpify100() => DumpExtensions.DumpText(_100);
    
    // [Benchmark]
    // public string Json1000() => JsonSerializer.Serialize(_1000);
    
    // [Benchmark]
    // public string Html1000() => HtmlDumper.DumpHtml(_1000);
    //
    // [Benchmark]
    // public string Dumpify1000() => DumpExtensions.DumpText(_1000);
    
    // [Benchmark]
    // public string Json10000() => JsonSerializer.Serialize(_10000);
    //
    // [Benchmark]
    // public string Html10000() => HtmlDumper.DumpHtml(_10000);

    public class Car(int id)
    {
        public int Id { get; set; } = id;
        public string Make { get; set; } = Util.GenerateRandomString(10);
        public string Model { get; set; } = Util.GenerateRandomString(15);
        public int Year { get; set; } = 2022;
        public DateTime CreatedDate { get; set; } = new(2022, 1, 1);

        public List<Feature> Features { get; set; } = new()
        {
            new(),
            new(),
            new(),
            new(),
            new()
        };
    }

    public class Feature
    {
        public string Label { get; set; } = Util.GenerateRandomString(15);
        public string Description { get; set; } = Util.GenerateRandomString(50);
        public bool Included { get; set; } = true;
    }
}