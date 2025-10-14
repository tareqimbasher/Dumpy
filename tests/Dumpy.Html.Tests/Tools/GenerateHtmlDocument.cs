using System.Data;
using System.Reflection;
using Dumpy.Tests.Models;

namespace Dumpy.Html.Tests.Tools;

public class GenerateHtmlDocument
{
    [Fact]
    public void GeneratePage()
    {
        //Person();
        SimpleCollection();
        //PersonCollection();
        //FileSystemInfo();
        //Tuple();
        //TwoDimensionalArray();
        // Memory();
        //DataTable();
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

    private void SimpleCollection()
    {
        Write(new[] { 1, 2, 3 });
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

    private void DataTable()
    {
        var table = new DataTable("Table name");
        table.Columns.Add("Name");
        table.Columns.Add("Date of Birth", typeof(DateTime));
        table.Columns.Add("Salary", typeof(decimal));

        var row1 = table.NewRow();
        table.Rows.Add(row1);
        row1.ItemArray = ["John Doe", DateTime.Parse("1980/1/1"), 1000];
        
        var row2 = table.NewRow();
        table.Rows.Add(row2);
        row2.ItemArray = ["Jane Doe", DateTime.Parse("1981/1/1"), 2000];

        Write(table);
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