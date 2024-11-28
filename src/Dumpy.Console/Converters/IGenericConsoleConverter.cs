using System;
using Spectre.Console.Rendering;

namespace Dumpy.Console.Converters;

public interface IGenericConsoleConverter
{
    IRenderable Convert<T>(T? value, Type targetType, DumpOptions options);
}