using System;

namespace Dumpy.Renderers;

public interface IGenericConverter
{
    void Convert<T>(ref ValueStringBuilder writer, T? value, Type targetType, DumpOptions options);
}

public interface IConverter<in T>
{
    void Convert(ref ValueStringBuilder writer, T? value, Type targetType, DumpOptions options);
}
