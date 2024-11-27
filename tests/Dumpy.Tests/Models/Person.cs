namespace Dumpy.Tests.Models;

public class Person
{
    public required string Name { get; set; }
    public required int Age { get; set; }

    public string[]? OtherNames { get; set; }

    public Person? Spouse { get; set; }
    public Person[]? Children { get; set; }
}