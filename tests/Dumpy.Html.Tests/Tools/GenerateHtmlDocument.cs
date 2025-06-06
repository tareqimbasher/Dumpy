using System.Reflection;
using Dumpy.Tests.Models;

namespace Dumpy.Html.Tests.Tools;

public class GenerateHtmlDocument
{
    [Fact]
    public void GeneratePage()
    {
        //Person();
        FileSystemInfo();
    }

    private void Person()
    {
        var person = new Person
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
        Write(person);
    }

    private void FileSystemInfo()
    {
        var file = new FileInfo("/does/not/exist.txt");
        var dir = new DirectoryInfo("/does/not/exist");
        Write(dir);
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