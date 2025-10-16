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
        //SimpleCollection();
        //PersonCollection();
        //FileSystemInfo();
        //Tuple();
        //TwoDimensionalArray();
        // Memory();
        //DataTable();
        DataSet();
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

        table.Rows.Add("John Doe", DateTime.Parse("1980/1/1"), 1000);
        table.Rows.Add("Jane Doe", DateTime.Parse("1981/1/1"), 2000);

        Write(table);
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
        
        Write(dataSet);
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