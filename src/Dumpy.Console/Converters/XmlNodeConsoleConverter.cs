using System;
using System.Xml;
using Dumpy.Console.Widgets;
using Spectre.Console.Rendering;

namespace Dumpy.Console.Converters;

public class XmlNodeConsoleConverter : ConsoleConverter<XmlNode>
{
    public override IRenderable Convert(XmlNode? value, Type targetType, ConsoleDumpOptions options)
    {
        if (value is null)
        {
            return NullWidget.New(options);
        }
        
        return value.OuterXml.DumpToRenderable();
    }
}