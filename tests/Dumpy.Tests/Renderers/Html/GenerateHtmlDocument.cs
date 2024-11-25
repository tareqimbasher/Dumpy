using System.Reflection;
using Dumpy.Html;
using Dumpy.Tests.Renderers.Html.Models;

namespace Dumpy.Tests.Renderers.Html;

public class GenerateHtmlDocument
{
    [Fact(Skip = "Manual")]
    public void GeneratePage()
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

        var html = Dumper.DumpHtml(person, new HtmlDumpOptions { AddTitleAttributes = true });

        var doc = Consts.DocumentTemplate.Replace("HTML_REPLACE", html);

        File.WriteAllText(
            Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "dumpy.html"),
            doc);
    }
}