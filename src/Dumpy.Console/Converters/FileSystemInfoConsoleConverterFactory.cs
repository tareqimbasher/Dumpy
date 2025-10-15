using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Dumpy.Console.Converters;

public class FileSystemInfoConsoleConverterFactory : ConsoleConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeof(FileSystemInfo).IsAssignableFrom(typeToConvert);
    }

    public override ConsoleConverter? CreateConverter(Type typeToConvert, ConsoleDumpOptions options)
    {
        return Activator.CreateInstance(typeof(FileSystemInfoConsoleConverter)) as ConsoleConverter;
    }
}

public class FileSystemInfoConsoleConverter : ObjectDefaultConsoleConverter<FileSystemInfo>
{
    private static readonly HashSet<string> _serializableProperties = new()
    {
        nameof(FileSystemInfo.Attributes),
        nameof(FileSystemInfo.FullName),
        nameof(FileSystemInfo.Name),
        nameof(FileSystemInfo.Extension),
        nameof(FileSystemInfo.Exists),
        nameof(FileSystemInfo.CreationTime),
        nameof(FileSystemInfo.CreationTimeUtc),
        nameof(FileSystemInfo.LastAccessTime),
        nameof(FileSystemInfo.LastAccessTimeUtc),
        nameof(FileSystemInfo.LastWriteTime),
        nameof(FileSystemInfo.LastWriteTimeUtc),
#if NET6_0_OR_GREATER
        nameof(FileSystemInfo.LinkTarget),
#endif
#if NET7_0_OR_GREATER
        nameof(FileSystemInfo.UnixFileMode),
#endif
        nameof(FileInfo.Directory),
    };
    
    protected override MemberInfo[] GetReadableMembers(Type targetType, ConsoleDumpOptions options)
    {
        return base.GetReadableMembers(targetType, options)
            .Where(p => _serializableProperties.Contains(p.Name))
            .ToArray();
    }
}