using System.Data;
using System.Text.Json;
using System.Xml;
using System.Xml.Linq;
using Dumpy.Tests;
using Dumpy.Tests.Models;
using Spectre.Console;
using Rule = Spectre.Console.Rule;

PrintHeader("Strings");
"String".DumpInternal();
3.2.DumpInternal();
DateTime.Now.DumpInternal();

PrintHeader("Object");
new {Name = "John Doe", Age = 30}.DumpInternal();

PrintHeader("Collection");
Enumerable.Range(0, 5).Select(x => new
{
    Id = x + 1,
    Hash = TestHelper.GenerateRandomString(10),
    Message = TestHelper.GenerateRandomString(30),
    Created = DateTime.Now
}).DumpInternal();

PrintHeader("Tuple");
(KitchenSink car, DateTime created) tuple = (new KitchenSink(), DateTime.Now);
tuple.DumpInternal();

PrintHeader("2D Array");
int[,] numbers = { { 1, 4, 2 }, { 3, 6, 8 } };
numbers.DumpInternal();

PrintHeader("Memory");
Memory<int> memory = new[] { 1, 4, 2 };
memory.DumpInternal();

PrintHeader("FileSystemInfo");
new FileInfo("/tmp/text.txt").DumpInternal();

PrintHeader("XmlNode");
var xmlDoc = new XmlDocument();
xmlDoc.LoadXml("<person><name>My Name</name></person>");
xmlDoc.DumpInternal();

PrintHeader("XNode");
XElement.Parse("<person><name>My Name</name></person>").DumpInternal();

PrintHeader("JSON");
JsonDocument.Parse(JsonSerializer.Serialize(new KitchenSink())).DumpInternal();

PrintHeader("DataTable");
var table = new DataTable("Table name");
table.Columns.Add("Name");
table.Columns.Add("Date of Birth", typeof(DateTime));
table.Columns.Add("Salary", typeof(decimal));
table.Rows.Add("John Doe", DateTime.Parse("1980/1/1"), 1000);
table.Rows.Add("Jane Doe", DateTime.Parse("1981/1/1"), 2000);
table.DumpInternal();

PrintHeader("DataSet");
var table1 = new DataTable();
table1.Columns.Add("Name");
table1.Columns.Add("Age", typeof(int));
table1.Rows.Add("John Doe", 25);

var table2 = new DataTable();
table2.Columns.Add("Item");
table2.Columns.Add("Price", typeof(decimal));
table2.Rows.Add("Coffee", 5);

var dataSet = new DataSet();
dataSet.Tables.Add(table1);
dataSet.Tables.Add(table2);
dataSet.DumpInternal();

PrintHeader("Kitchen Sink");
new KitchenSink().DumpInternal();

return;

void PrintHeader(string text)
{
    Console.WriteLine();
    var rule = new Rule($"[bold][deeppink1_1]{text}[/][/]");
    rule.Justification = Justify.Left;
    AnsiConsole.Write(rule);
    Console.WriteLine();
}