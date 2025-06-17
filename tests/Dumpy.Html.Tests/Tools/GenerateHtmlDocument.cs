using System.Reflection;
using Dumpy.Tests.Models;

namespace Dumpy.Html.Tests.Tools;

public class GenerateHtmlDocument
{
    [Fact]
    public void GeneratePage()
    {
        //Person();
        //PersonCollection();
        //FileSystemInfo();
        //Tuple();
        //TwoDimensionalArray();
        Memory();
    }

    private void Person()
    {
        Write(NewPerson());
    }

    private Person NewPerson()
    {
        return new Person
        {
            Name = "John",
            Age = 30,
            OtherNames = ["Jerry", "Jack"],
            Spouse = new Person
            {
                Name = "Jane",
                Age = 25
            },
            Children =
            [
                new Person
                {
                    Name = "Adam",
                    OtherNames = ["Adder"],
                    Age = 16
                },
                new Person
                {
                    Name = "June",
                    Age = 13
                }
            ]
        };
    }

    private void PersonCollection()
    {
        var collection = Enumerable.Range(0, 4).Select(x => NewPerson());
        Write(collection);
    }

    private void FileSystemInfo()
    {
        var file = new FileInfo("/does/not/exist.txt");
        var dir = new DirectoryInfo("/does/not/exist");
        Write(dir);
    }

    private void Tuple()
    {
        (Person person, DateTime created) tuple = (NewPerson(), DateTime.Now);
        Write(tuple);
    }

    private void TwoDimensionalArray()
    {
        int[,] numbers = { { 1, 4, 2 }, { 3, 6, 8 } };
        Write(numbers);
    }

    private void Memory()
    {
        Memory<int> memory = new[] { 1, 4, 2 };
        Write(memory);
    }

    private static void Write<T>(T value)
    {
        var html = HtmlDumper.DumpHtml(value, new HtmlDumpOptions { AddTitleAttributes = true });

        var doc = Consts.DocumentTemplate.Replace("HTML_REPLACE", html);

        File.WriteAllText(
            Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "dumpy.html"),
            doc);
    }
}