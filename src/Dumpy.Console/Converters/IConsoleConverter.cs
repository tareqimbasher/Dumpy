using System;
using Spectre.Console.Rendering;

namespace Dumpy.Console.Converters;

public interface IConsoleConverter<in T>
{
    IRenderable Convert(T? value, Type targetType, DumpOptions options);
}