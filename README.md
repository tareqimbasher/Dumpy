# Dumpy

Small, fast object dumpers for .NET. Convert objects, collections, tuples, DataTables and more to rich formatted
output in your terminal or as portable HTML.

**Supports:** `netstandard2.1`, `net6.0` and later

# Packages

- [Dumpy.Console](src/Dumpy.Console/README.md) — Dump to rich, formatted console output.
- [Dumpy.Html](src/Dumpy.Html/README.md) — Dump to compact, structured HTML and visualize data for user reports, logging
  artifacts, or sharing in docs and bug reports.

# Quick start

## Dumpy.Console

### Installation

    dotnet add package Dumpy.Console

### Usage

```csharp
using Dumpy.Console;

var person = new { Name = "Ada", Age = 37 };
person.Dump("Person");
```

## Dumpy.Html

### Installation

    dotnet add package Dumpy.Html

### Usage

```csharp
using Dumpy.Html;

var person = new { Name = "Ada", Age = 37 };
string html = person.Dump();
System.IO.File.WriteAllText("person.html", html);
```

# Supported types

You can dump almost any .NET object or value, including:

- Primitives, anonymous types, POCOs
- Enumerables and collections (including multidimensional arrays)
- Tuples and ValueTuples
- DataTable and DataSet
- JsonDocuments and JsonElements
- XmlNodes and XNodes
