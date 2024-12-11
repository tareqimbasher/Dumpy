using System;
using System.Collections.Generic;

namespace Dumpy;

public static class DumperTests
{
    public static void Run()
    {
        System.Console.WriteLine("Hello 12");
        "Hello 2".Dump();
        "Line 1\nLine 2".Dump();
        "I have a HTML special chars <here>      &     <here>".Dump();
        5.Dump();
        DateTime.Now.Dump();

        "10".Dump();
        "20".Dump();
        "30".Dump();

        "10".Dump("With title");
        "20".Dump();
        "30".Dump();
        "40".Dump("With title");

        var car1 = new Car()
        {
            Make = "Toyota",
            Model = "Avalon",
            Year = 2020
        }.Dump();

        var feature1 = new Feature()
        {
            Name = "Leather Seats",
            HasIt = true
        };

        var feature2 = new Feature()
        {
            Name = "Duck Leather Seats",
            HasIt = false,
            DependantFeatures = new List<Feature>() { feature1 }
        };

        //feature2.DependantFeatures.Add(feature2);

        var car2 = new Car()
        {
            Make = "Toyota",
            Model = "Avalon",
            Year = 2020,
            //Features = new List<Feature>() { feature1, feature2 }
        }.Dump();

        new[] { car1, car2, car2 }.Dump();

        new string('X', 10).Dump("A short string");
        new string('X', 1000).Dump("A long string");

        Array.Empty<string>().Dump();

        new
        {
            Name = "Tareq",
            Cars = Array.Empty<string>(),
            Text =
                "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX"
        }.Dump();

//Process.GetCurrentProcess().Dump("Current Process");

//Process.GetProcesses().Take(2).Dump("First 2 Processes");
    }

    class Car
    {
        public string Make { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public List<Feature> Features { get; set; }
    }

    class Feature
    {
        public string Name { get; set; }
        public bool HasIt { get; set; }
        public List<Feature> DependantFeatures { get; set; }
    }
}