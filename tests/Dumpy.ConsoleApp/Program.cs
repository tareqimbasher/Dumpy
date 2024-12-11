using Dumpy;
using Dumpy.ConsoleApp;

Console.Clear();

DumperTests.Run();
return;

var car = new Car();
car.Dump();

new[] { new Car(), new Car(), new Car() }.Dump();
