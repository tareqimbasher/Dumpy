using Dumpy;
using Dumpy.ConsoleApp;

Console.Clear();

DumperTests.Run();
return;

var car = new Car();
car.Dump();

new[] { new Car(), new Car(), new Car() }.Dump();
new FileInfo("/tmp/text.txt").Dump();
XElement.Parse("<person><name>My Car</name></person>").Dump();