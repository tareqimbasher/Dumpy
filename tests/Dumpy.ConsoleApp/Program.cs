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

return;

void PrintHeader(string text)
{
    Console.WriteLine($"\n{text}\n{new string('-', text.Length + 1)}");
}