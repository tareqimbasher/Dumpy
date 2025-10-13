using System.Data;
using System.Text.Json;
using System.Xml;
using System.Xml.Linq;
using Dumpy.Console;
using Dumpy.ConsoleApp;

PrintHeader("Strings");
"String".Dump();
3.2.Dump();
DateTime.Now.Dump();

PrintHeader("Object");
new Car().Dump();

PrintHeader("Collection");
new[] { new Car(), new Car() }.Dump();
new Dictionary<string, int>() { { "Student 1", 90 }, { "Student 2", 100 } }.Dump();

PrintHeader("Tuple");
(Car car, DateTime created) tuple = (new Car(), DateTime.Now);
tuple.DumpConsole();

PrintHeader("2D Array");
int[,] numbers = { { 1, 4, 2 }, { 3, 6, 8 } };
numbers.DumpConsole();

PrintHeader("Memory");
Memory<int> memory = new[] { 1, 4, 2 };
memory.DumpConsole();

PrintHeader("FileSystemInfo");
new FileInfo("/tmp/text.txt").DumpConsole();

PrintHeader("XmlNode");
var xmlDoc = new XmlDocument();
xmlDoc.LoadXml("<person><name>My Name</name></person>");
xmlDoc.Dump();

PrintHeader("XNode");
XElement.Parse("<person><name>My Name</name></person>").Dump();

PrintHeader("JSON");
JsonDocument.Parse(JsonSerializer.Serialize(new Car())).Dump();

PrintHeader("DataTable");
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
table.DumpConsole();

return;

void PrintHeader(string text)
{
    Console.WriteLine($"\n{text}\n{new string('-', text.Length + 1)}");
}