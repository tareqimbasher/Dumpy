# Dumpy.Html

Generate compact, structured HTML representations of objects for user reports, debugging, logging artifacts, or sharing
in docs and bug reports.

**Supports:** `netstandard2.1`, `net6.0` and later

# Installation

### .NET CLI

    dotnet add package Dumpy.Html

### Package Manager

    Install-Package Dumpy.Html

### PackageReference

```xml

<ItemGroup>
    <PackageReference Include="Dumpy.Html" Version="1.0.0"/>
</ItemGroup>
```

# Quick start

#### Dump any object to HTML

```csharp
using Dumpy.Html;

var person = new { Name = "Ada", Age = 37 };
string html = person.Dump();
// Save or embed the HTML
System.IO.File.WriteAllText("person.html", html);
```

#### Customize rendering via options

```csharp
using Dumpy;
using Dumpy.Html;

var options = new HtmlDumpOptions
{
    MaxDepth = 6,
    ReferenceLoopHandling = ReferenceLoopHandling.IgnoreAndSerializeCyclicReference,
    AddTitleAttributes = true,
    CssClasses = new CssClassOptions
    {
        Enabled = true,
        Null = "null-element",
    }
};

string html = person.Dump(options);
```

# Options

**Default options:**

```csharp
new HtmlDumpOptions
{
    // Choose what to do with cyclic references, options: 
    // Error, Ignore, IgnoreAndSerializeCyclicReference, Serialize
    ReferenceLoopHandling = ReferenceLoopHandling.Error,

    // Limit traversal depth to avoid overly deep graphs.
    MaxDepth = 10,

    // Limit the number of items to dump from a collection. Set if you dump large lists, arrays...etc.
    MaxCollectionItems = int.MaxValue,

    // Whether to include dump instance fields.
    IncludeFields = false,

    // Whether to include non public properties and fields.
    IncludeNonPublicMembers = false,

    // Filter which member (property of field) will be included in the output.
    MemberFilter = m => true,

    // Register custom HtmlConverter instances to control how specific types are rendered.
    Converters = { new MyHtmlConverter() },

    // Add HTML title attributes (ex. type names) for tooltips.
    AddTitleAttributes = false,

    // Control CSS classes applied to special cases (nulls, empty collections, etc.).
    CssClasses = new CssClassOptions
    {
        Enabled = true,                         // Disable to stop emitting CSS classes
        Null = "dm-null",                       // Added to null values
        EmptyCollection = "dm-empty",           // Added to empty collections (arrays, dictionaries, tuples...etc)
        CyclicReference = "dm-cyclic",          // Added to generated cyclic reference elements
        MaxDepthReached = "dm-depth-max",       // Added to generated max depth reached elements
        TableInfoHeader = "dm-t-info",          // Added to the info row of a table
        TableDataHeader = "dm-t-data",          // Added to the header rowo the table
    },
}
```

# API overview

The main utility class is the `HtmlDumper` static class, which provides the following extension methods.

```csharp
// Dumps the value to an HTML string. The second one lets you specify the runtime type.
string DumpHtml<T>(this T value, HtmlDumpOptions? options = null)
string DumpHtml<T>(this T value, Type valueType, HtmlDumpOptions? options = null)

// Low-level API that writes directly to a ValueStringBuilder (advanced)
void DumpHtml<T>(ref ValueStringBuilder writer, T? value, Type valueType, HtmlDumpOptions options)
```

# Usage

**Dump to an HTML string and save it:**

```csharp
var html = person.Dump();
System.IO.File.WriteAllText("person.html", html);
```

**Specify the runtime type explicitly:**

```csharp
Employee employee = person;
var html = employee.Dump(typeof(Person));
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

To add your own converter, create an `HtmlConverter` or a factory and register it via `HtmlDumpOptions.Converters`

```csharp
var options = new HtmlDumpOptions();
options.Converters.Add(new MyCustomTypeHtmlConverter());
```
