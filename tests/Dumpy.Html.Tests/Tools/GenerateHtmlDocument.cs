using System.Data;
using System.Reflection;
using Dumpy.Tests.Models;

namespace Dumpy.Html.Tests.Tools;

public class GenerateHtmlDocument
{
    private readonly List<(string title, string html)> _htmlFragments = new();

    [Fact(Skip = "Adhoc")]
    public void GeneratePage()
    {
        Object();
        SimpleCollection();
        PersonCollection();
        Tuple();
        TwoDimensionalArray();
        Memory();
        DataTable();
        DataSet();
        FileSystemInfo();
        KitchenSink();
        
        Write();
    }

    private void Object()
    {
        Add(NewPerson(), "Object");
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
        Add(new[] { 1, 2, 3 }, "Collection of Primitives");
    }

    private void PersonCollection()
    {
        var collection = Enumerable.Range(0, 4).Select(x => NewPerson());
        Add(collection, "Collection of Objects");
    }

    private void Tuple()
    {
        (Person person, DateTime created) tuple = (NewPerson(), DateTime.Now);
        Add(tuple, "Tuple");
    }

    private void TwoDimensionalArray()
    {
        int[,] numbers = { { 1, 4, 2 }, { 3, 6, 8 } };
        Add(numbers, "2-D Array");
    }

    private void Memory()
    {
        Memory<int> memory = new[] { 1, 4, 2 };
        Add(memory, "Memory");
    }

    private void DataTable()
    {
        var table = new DataTable("Table name");
        table.Columns.Add("Name");
        table.Columns.Add("Date of Birth", typeof(DateTime));
        table.Columns.Add("Salary", typeof(decimal));

        table.Rows.Add("John Doe", DateTime.Parse("1980/1/1"), 1000);
        table.Rows.Add("Jane Doe", DateTime.Parse("1981/1/1"), 2000);

        Add(table, "DataTable");
    }

    private void DataSet()
    {
        var table1 = new DataTable("Table 1");
        table1.Columns.Add("Name");
        table1.Columns.Add("Date of Birth", typeof(DateTime));
        table1.Columns.Add("Salary", typeof(decimal));
        table1.Rows.Add("John Doe", DateTime.Parse("1980/1/1"), 1000);
        table1.Rows.Add("Jane Doe", DateTime.Parse("1981/1/1"), 2000);

        var table2 = new DataTable("Table 2");
        table2.Columns.Add("Item");
        table2.Columns.Add("Price", typeof(decimal));
        table2.Rows.Add("Coffee", 5);
        table2.Rows.Add("Car", 30000);

        var dataSet = new DataSet();
        dataSet.Tables.Add(table1);
        dataSet.Tables.Add(table2);

        Add(dataSet, "DataSet");
    }

    private void FileSystemInfo()
    {
        Add(new DirectoryInfo("/path/to/folder"), "DirectoryInfo");
        Add(new FileInfo("/path/to/file.txt"), "FileInfo");
    }
    
    private void KitchenSink()
    {
        Add(new KitchenSink(), "DirectoryInfo");
    }

    private void Add<T>(T value, string title)
    {
        var html = value.DumpHtml(new HtmlDumpOptions { AddTitleAttributes = true });
        _htmlFragments.Add((title, html));
    }

    private void Write()
    {
        var html = string.Join("<br/><hr/><br/>",
                _htmlFragments.Select(x =>
                    $"""
                    <h1>{x.title}</h1>
                    <div>{x.html}</div>
                    """
            ));

        var doc = Consts.DocumentTemplate.Replace("HTML_REPLACE", html);

        var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "dumpy.html");
        File.WriteAllText(path, doc);
    }

    // private static void Write2<T>(T value)
    // {
    //     var html = HtmlDumper.DumpHtml(value, new HtmlDumpOptions { AddTitleAttributes = true });
    //
    //     var doc = Consts.DocumentTemplate.Replace("HTML_REPLACE", html);
    //
    //     File.WriteAllText(
    //         Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "dumpy.html"),
    //         doc);
    // }
}