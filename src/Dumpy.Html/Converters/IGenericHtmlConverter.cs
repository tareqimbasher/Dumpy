using System;

namespace Dumpy.Html.Converters;

/// An HTML converter that serializes a value using the type 'T' as a shape.
/// Example: This will serialize a particular value in the shape of an object, or a tree, or a table, etc.
/// So you can take a DateTime value and pass it to a "ITableHtmlConverter" that will format any value it gets as a table, to its best ability.
/// You can also pass that same DateTIme value to a "IStringHtmlConverter" that will format any value it gets into a string.
/// In all cases, the value is serialized in a pre-determined shape specific to this IShapeWiseHtmlConverter
/// * This one has a pre-defined representation, accepts any value regardless what is
///
/// In the IHtmlConverter case, the type 'T' is defined as an interface arg, so that converter will only be able to convert values of type 'T',
/// but it can serliazes it to any representation/shape it wants, it could be a string, a table, a tree, a graph, etc.
/// * This one it always serializes a particular type in whatever shape it wants, or:
///   This one has a pre-defined type, and decides how it wants to represent it, however it wants
/// 
/// That's the difference between the two.



/// <summary>
/// 
/// </summary>
public interface IGenericHtmlConverter
{
    void Convert<T>(ref ValueStringBuilder writer, T? value, Type targetType, HtmlDumpOptions options);
}