# Dumpy

Small, fast object dumpers for .NET. Inspect objects, collections, tuples, DataTables, JSON, XML and more — in your terminal or as portable HTML.

**Supports:** `netstandard2.1` and `net6.0+`

# Packages

- [Dumpy.Console](src/Dumpy.Console/README.md) — rich, formatted console output
- [Dumpy.Html](src/Dumpy.Html/README.md) — compact, styled HTML dumps for docs and bug reports

# Installation

### Dumpy.Console

    dotnet add package Dumpy.Console

### Dumpy.Html

    dotnet add package Dumpy.Html

# Quick start

#### Console

```csharp
using Dumpy.Console;

var person = new { Name = "Ada", Age = 37 };
person.Dump("Person");
```

#### HTML

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
