namespace Dumpy.Benchmark.Models;

public class Car(int id)
{
    public int Id { get; set; } = id;
    public Guid Uuid { get; set; } = Guid.NewGuid();
    public string Make { get; set; } = Util.GenerateRandomString(15);
    public string Model { get; set; } = Util.GenerateRandomString(15);
    public int Year { get; set; } = 2000;
    public decimal Price { get; set; } = 29999.99m;
    public DateTime CreatedDate { get; set; } = new(2000, 1, 1);

    public string[] Colors { get; set; } = ["Black", "Blue", "Green", "Red", "White", "Silver"];

    public Dictionary<int, string> PartNumbers { get; set; } = new()
    {
        { 1, "Engine" },
        { 2, "Piston" },
        { 3, "Wheel" },
    };

    public (string Name, decimal Cost)[] PartCosts { get; set; } =
    [
        ("Engine", 2000),
        ("Piston", 400),
        ("Wheel", 100),
    ];

    public List<CarFeature> Features { get; set; } = new()
    {
        new(),
        new(),
        new(),
    };
}

public class CarFeature
{
    public string Label { get; set; } = Util.GenerateRandomString(15);
    public string Description { get; set; } = Util.GenerateRandomString(50);
    public bool Included { get; set; } = true;
}