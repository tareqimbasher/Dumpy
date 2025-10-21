using System.Reflection;
using System.Text.Json.Serialization;

namespace Dumpy.Tests.Models;

public class Item
{
    public string Label { get; set; } = TestHelper.GenerateRandomString(15);
    public string Description { get; set; } = TestHelper.GenerateRandomString(50);
    public bool Included { get; set; } = true;
}

public class KitchenSink
{
    public string Text { get; set; } = TestHelper.GenerateRandomString(15);
    public int Int { get; set; } = 2022;
    public decimal Decimal { get; set; } = 10.99m;
    public double Double { get; set; } = Math.PI;
    public string? Null { get; set; } = null;
    public bool True { get; set; } = true;
    public bool False { get; set; } = false;
    public Guid Guid { get; set; } = Guid.NewGuid();
    public DateTime DateTime { get; set; } = new DateTime(2020, 6, 1);
    public TimeOnly TimeOnly { get; set; } = new TimeOnly(0, 0, 0);
    public DateOnly DateOnly { get; set; } = new DateOnly(2020, 6, 1);

    public DateTimeOffset DateTimeOffset { get; set; } =
        new DateTimeOffset(new DateTime(2020, 6, 1), TimeSpan.FromHours(-8));

    public TimeSpan TimeSpan { get; set; } = TimeSpan.FromMinutes(5);
    public BindingFlags Enum { get; set; } = BindingFlags.FlattenHierarchy;
    public BindingFlags EnumFlags { get; set; } = BindingFlags.Instance | BindingFlags.Public;

    public Guid[] Array { get; set; } = Enumerable.Range(0, 4).Select(_ => Guid.NewGuid()).ToArray();

    [JsonIgnore] public int[,] TwoDimensionalArray = { { 1, 4, 2 }, { 3, 6, 8 } };

    public Dictionary<string, string> Dictionary { get; set; } = new Dictionary<string, string>()
    {
        { "key1", "value1" },
        { "key2", "value2" }
    };

    public List<Item> ObjectCollection { get; set; } = new()
    {
        new(),
        new(),
        new(),
    };
    
    public List<Item> EmptyCollection { get; set; } = new();
}