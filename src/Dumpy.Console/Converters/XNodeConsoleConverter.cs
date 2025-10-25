using System;
using System.Xml.Linq;
using Dumpy.Console.Widgets;
using Spectre.Console.Rendering;

namespace Dumpy.Console.Converters;

public class XNodeConsoleConverter : ConsoleConverter<XNode>
{
    public override IRenderable Convert(XNode? value, Type targetType, ConsoleDumpOptions options)
    {
        if (value is null)
        {
            return NullWidget.New(options);
        }
        
        return value.ToString().DumpToRenderable();
    }
}