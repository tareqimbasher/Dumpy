using System.Diagnostics;
using Newtonsoft.Json;
using Xunit.Abstractions;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Dumpy.Tests.Renderers.Html;

public class PerformanceTests(ITestOutputHelper testOutputHelper)
{
    private Car[] Data = Enumerable.Range(0, 10000).Select(i => new Car(i)).ToArray();
    
    [Fact]
    public void Profile()
    {
        Dumper.DumpHtml(Data);
    }
    
    [Fact]
    public void LibComparison_Performance_Test()
    {
        const int times = 3;
        const bool preRun = true;

        foreach (var itemsCount in new[] { 1, 10, 100, 1000, 10000 })
        {
            var cars = GetCars(itemsCount);

            testOutputHelper.WriteLine($"Serializing {itemsCount} Cars");
            Benchmark("System.Text.Json", () => _ = JsonSerializer.Serialize(cars), times, preRun);
            Benchmark("Json.NET", () => _ = JsonConvert.SerializeObject(cars), times, preRun);
            Benchmark("Dumpy", () => _ = Dumper.DumpHtml(cars), times, preRun);
            testOutputHelper.WriteLine("");
        }
    }
    
    private void Benchmark(string label, Func<string> action, int runTimes = 1, bool preRun = false)
    {
        var stopWatch = new Stopwatch();

        if (preRun)
        {
            stopWatch.Start();

            // Run action first so that if first call caches data that later calls will use,
            // we don't get skewed results for non-cached call
            action();
        }

        double? firstRunMs = stopWatch.IsRunning ? stopWatch.Elapsed.TotalMilliseconds : null;

        var timings = new List<double>();
        var charLength = 0;

        for (int i = 0; i < runTimes; i++)
        {
            stopWatch.Restart();
            var output = action();
            charLength = output.Length;
            timings.Add(stopWatch.Elapsed.TotalMilliseconds);
        }

        stopWatch.Stop();

        timings.Sort();
        var median = timings[timings.Count / 2];

        var result = $"### {label,-25} => MEDIAN: {median}ms | AVG: {Math.Round(timings.Average(), 4)}ms | LEN: {charLength}";
        if (firstRunMs.HasValue)
        {
            result += $" | First Run: {firstRunMs}ms";
        }

        testOutputHelper.WriteLine(result);
    }

    private static List<Car> GetCars(int count)
    {
        var cars = new List<Car>();

        for (int i = 0; i < count; i++)
            cars.Add(new Car(i));

        return cars;
    }
    
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
