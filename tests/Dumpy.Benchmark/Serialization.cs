using System.Text.Json;
using BenchmarkDotNet.Attributes;
using Dumpify;
using Dumpy.Benchmark.Models;
using Dumpy.Html;

namespace Dumpy.Benchmark;

[MemoryDiagnoser]
[MinColumn, MaxColumn]
public class Serialization
{
    private readonly HtmlDumpOptions _htmlDumpOptions = new()
    {
        CssClasses = { Enabled = false }
    };

    private readonly Car _1 = new Car(1);
    private readonly Car[] _10 = Enumerable.Range(1, 10).Select(i => new Car(i)).ToArray();
    private readonly Car[] _100 = Enumerable.Range(1, 100).Select(i => new Car(i)).ToArray();
    private readonly Car[] _1000 = Enumerable.Range(1, 1000).Select(i => new Car(i)).ToArray();
    private readonly Car[] _10000 = Enumerable.Range(1, 10000).Select(i => new Car(i)).ToArray();

    [Benchmark(Description = "1 item - STJ")]
    public string Json1() => JsonSerializer.Serialize(_1);

    [Benchmark(Description = "1 item - Dumpy")]
    public string Html1() => HtmlDumper.DumpHtml(_1, _htmlDumpOptions);

    // [Benchmark(Description = "1 item - Dumpify")]
    // public string Dumpify1() => DumpExtensions.DumpText(_1);


    [Benchmark(Description = "10 items - STJ")]
    public string Json10() => JsonSerializer.Serialize(_10);

    [Benchmark(Description = "10 items - Dumpy")]
    public string Html10() => HtmlDumper.DumpHtml(_10, _htmlDumpOptions);

    // [Benchmark(Description = "10 items - Dumpify")]
    // public string Dumpify10() => DumpExtensions.DumpText(_10);


    [Benchmark(Description = "100 items - STJ")]
    public string Json100() => JsonSerializer.Serialize(_100);

    [Benchmark(Description = "100 items - Dumpy")]
    public string Html100() => HtmlDumper.DumpHtml(_100, _htmlDumpOptions);

    // [Benchmark(Description = "100 items - Dumpify")]
    // public string Dumpify100() => DumpExtensions.DumpText(_100);


    //[Benchmark(Description = "1000 items - STJ")]
    //public string Json1000() => JsonSerializer.Serialize(_1000);

    //[Benchmark(Description = "1000 items - Dumpy")]
    //public string Html1000() => HtmlDumper.DumpHtml(_1000, _htmlDumpOptions);

    // [Benchmark(Description = "1000 items - Dumpify")]
    // public string Dumpify1000() => DumpExtensions.DumpText(_1000);


    //[Benchmark(Description = "10000 items - STJ")]
    //public string Json10000() => JsonSerializer.Serialize(_10000);

    //[Benchmark(Description = "10000 items - Dumpy")]
    //public string Html10000() => HtmlDumper.DumpHtml(_10000, _htmlDumpOptions);

    // Takes too long
    // [Benchmark(Description = "10000 items - Dumpify")]
    // public string Dumpify10000() => DumpExtensions.DumpText(_10000);
}