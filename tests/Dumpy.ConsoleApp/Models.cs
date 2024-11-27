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
    public string Make { get; set; } = Helper.GenerateRandomString(10);
    public string Model { get; set; } = Helper. GenerateRandomString(15);
    public string Text1 { get; set; } = Helper. GenerateRandomString(15);
    public string Text2 { get; set; } = Helper. GenerateRandomString(15);
    public string Text3 { get; set; } = Helper. GenerateRandomString(15);
    public string Text4 { get; set; } = Helper. GenerateRandomString(15);
    public string Text5 { get; set; } = Helper. GenerateRandomString(15);
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
    public string Label { get; set; } = Helper.GenerateRandomString(15);
    public string Description { get; set; } = Helper.GenerateRandomString(50);
    public bool Included { get; set; } = true;
}