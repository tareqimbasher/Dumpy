using System.Diagnostics;
using Dumpy;
using Dumpy.ConsoleApp;

var cars = new List<Helper.Car>();

for (int i = 0; i < 10000; i++)
{
    cars.Add(new Helper.Car());
}

var sw = Stopwatch.StartNew();

Dumper.DumpHtml(cars);

sw.Stop();

Console.WriteLine($"Elapsed time: {sw.ElapsedMilliseconds}ms");


namespace Dumpy.ConsoleApp
{
    class Helper
    {
        private static readonly Random _random = new();

        static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        public class Car
        {
            public string Make { get; set; } = GenerateRandomString(10);
            public string Model { get; set; } = GenerateRandomString(15);
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
            public string Label { get; set; } = GenerateRandomString(15);
            public string Description { get; set; } = GenerateRandomString(50);
            public bool Included { get; set; } = true;
        }
    }
}