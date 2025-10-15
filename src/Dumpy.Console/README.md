# Dumpy.Console

A tiny, fast object dumper that renders rich formatted output directly to your terminal. Ideal for
data visualization, debugging and diagnostics: inspect objects, collections, tuples, DataTables, and more without
writing custom pretty-printers.

**Supports:** `netstandard2.1` and `net6.0+`

# Installation

### .NET CLI

    dotnet add package Dumpy.Console

### Package Manager

    Install-Package Dumpy.Console

### PackageReference

```xml

<ItemGroup>
    <PackageReference Include="Dumpy.Console" Version="1.0.0"/>
</ItemGroup>
```

🛈 This package depends on **[Spectre.Console](https://github.com/spectreconsole/spectre.console)** for rich console
visuals.

# Quick start

#### Dump any object to the console

```csharp
using Dumpy.Console;
var person = new { Name = "Ada", Age = 37 };
person.DumpConsole();
```

#### Add a title

```csharp
person.DumpConsole("Person");
```

#### Customize rendering via options

```csharp
using Dumpy.Console;
var options = new ConsoleDumpOptions
{
    MaxDepth = 3,
    ReferenceLoopHandling = ReferenceLoopHandling.IgnoreAndSerializeCyclicReference,
    TableOptions = new TableOptions
    {
        ShowTitles = false,
    }
};
person.DumpConsole("Person", options);
```

# Options

**Default options:**

```csharp
new ConsoleDumpOptions
{
    // Choose what to do with cyclic references, options: 
    // Error, Ignore, IgnoreAndSerializeCyclicReference, Serialize
    ReferenceLoopHandling = ReferenceLoopHandling.Error,
    
    // Limit traversal depth to avoid overly deep graphs.
    MaxDepth = 64,
    
    // Limit the number of items to dump from a collection. Set if you dump large lists, arrays...etc.
    MaxCollectionItems = int.MaxValue,

    // Whether to include dump instance fields.
    IncludeFields = false,
    
    // Whether to include non public properties and fields.
    IncludeNonPublicMembers = false,
    
    // Filter which member (property of field) will be included in the output.
    MemberFilter = m => true,
    
    // Register custom ConsoleConverter instances to control how specific types are rendered.
    Converters = { new MyConsoleConverter() },

    // Control table visuals (titles, headers, row separators, expand behavior).
    TableOptions = new TableOptions
    {
        ShowTitles = true,              // Show table titles
        ShowHeaders = true,             // Show table headers
        ShowRowSeparators = true,       // Show table row line separators
        Expand = false,                 // Expand the table width to fill available space
    },
}
```

# API overview

The main utility class is the `ConsoleDumper` static class, which provides the following extension methods.

```csharp
// Dumps the value directly to the console, and returns the same value. The second one has an option "title" parameter.
T DumpConsole(this T value, ConsoleDumpOptions? options = null)
T DumpConsole(this T value, string? title, ConsoleDumpOptions? options = null)

// Converts the value to a "Spectre.Console" `IRenderable`.
IRenderable DumpToRenderable(this T value, ConsoleDumpOptions? options = null)
IRenderable DumpToRenderable(this T value, Type valueType, ConsoleDumpOptions? options = null)
```

# Usage

**Dump to the console directly:**

```csharp
person.DumpConsole();                   // <== using the extension method, or
ConsoleDumper.DumpConsole(person);      // <== using the ConsoleDumper static class
```

**Annotate the output with a title:**

```csharp
person.DumpConsole("John Doe");
```

**Dump to a `IRenderable` and then write it to the console yourself:**

```csharp
var renderable = person.DumpToRenderable();
AnsiConsole.Write(renderable);
```

# Supported types

You can dump almost any .NET object or value, including:

- Primitives, anonymous types, POCOs
- Enumerables and collections (including multidimensional arrays)
- Tuples and ValueTuples
- DataTable and DataSet
- JsonDocuments and JsonElements
- XmlNodes and XNodes

# Custom converters

To add your own converter, create a `ConsoleConverter` or a factory and register it via `ConsoleDumpOptions.Converters`

```csharp
var options = new ConsoleDumpOptions();
options.Converters.Add(new MyCustomTypeConsoleConverter());
```
