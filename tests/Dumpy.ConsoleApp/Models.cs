namespace Dumpy.ConsoleApp;

static class Helper
{
    private static readonly Random _random = new();

    public static string GenerateRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[_random.Next(s.Length)]).ToArray());
    }
}
    
public class Car
{
    public string Text1 { get; set; } = Helper. GenerateRandomString(15);
    public int Int { get; set; } = 2022;
    public decimal Decimal { get; set; } = 15.35m;
    public string? Null { get; set; } = null;
    public bool True { get; set; } = true;
    public bool False { get; set; } = false;
    public DateTime DateTime { get; set; } = new DateTime(2020, 1, 1);
    public DateOnly DateOnly { get; set; } = new DateOnly(2020, 1, 1);
    public TimeSpan TimeSpan { get; set; } = TimeSpan.FromMinutes(5);
    public Dictionary<string, string> Dictionary { get; set; } = new Dictionary<string, string>()
    {
        { "key1", "value1" },
        { "key2", "value2" }  
    };
    //public DateTimeOffset DateTimeOffset { get; set; } = new DateTimeOffset(new DateTime(2020, 1, 1));
    
    public List<Feature> EmptyList { get; set; } = new();
    public List<Feature> Features { get; set; } = new()
    {
        new(),
        new(),
        new(),
        // new(),
        // new()
    };
}

public class Feature
{
    public string Label { get; set; } = Helper.GenerateRandomString(15);
    public string Description { get; set; } = Helper.GenerateRandomString(50);
    public bool Included { get; set; } = true;
}