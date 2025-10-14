using System.Collections;
using System.Collections.Concurrent;
using System.Xml;
using System.Xml.Linq;
using Dumpy.Html.Converters;
using Dumpy.Tests.Models;

namespace Dumpy.Html.Tests.Converters.ConverterSelection;

public class BuiltinConverterSelectionTests
{
    [Theory]
    [MemberData(nameof(StringFormattableTypesData))]
    public void StringFormattableTypes(Type type)
        => AssertOpenGenericConverter(type, typeof(StringDefaultHtmlConverter<>));

    [Theory]
    [MemberData(nameof(CollectionTypesData))]
    public void CollectionTypes(Type type)
        => AssertOpenGenericConverter(type, typeof(EnumerableDefaultHtmlConverter<>));

    [Theory]
    [MemberData(nameof(SpecialTypesData))]
    public void SpecialTypes(Type targetType, Type expectedConverterType)
        => AssertExactConverter(targetType, expectedConverterType);

    [Theory]
    [MemberData(nameof(ObjectInstancesData))]
    public void ObjectAndAnonymousTypes(object instance)
        => AssertOpenGenericConverter(instance.GetType(), typeof(ObjectDefaultHtmlConverter<>));

    private static HtmlDumpOptions CreateOptions() => new();

    private static void AssertOpenGenericConverter(Type targetType, Type openGeneric)
    {
        var options = CreateOptions();
        var converter = options.GetConverter(targetType);
        Assert.Equal(openGeneric.MakeGenericType(targetType), converter.GetType());
    }

    private static void AssertExactConverter(Type targetType, Type expectedConcrete)
    {
        var options = CreateOptions();
        var converter = options.GetConverter(targetType);
        Assert.NotNull(converter);
        Assert.Equal(expectedConcrete, converter.GetType());
    }

    public static IEnumerable<object[]> StringFormattableTypesData =>
    [
        [typeof(int)], [typeof(int?)],
        [typeof(long)], [typeof(long?)],
        [typeof(float)], [typeof(float?)],
        [typeof(double)], [typeof(double?)],
        [typeof(decimal)], [typeof(decimal?)],
        [typeof(DateTime)], [typeof(DateTime?)],
        [typeof(DateOnly)], [typeof(DateOnly?)],
        [typeof(StringComparison)], [typeof(StringComparison?)],
        [typeof(string)], [typeof(Exception)]
    ];

    public static IEnumerable<object[]> CollectionTypesData =>
    [
        [typeof(Array)],
        [typeof(string[])],
        [typeof(List<string>)],
        [typeof(IEnumerable)],
        [typeof(IEnumerable<string>)],
        [typeof(IList<string>)],
        [typeof(IReadOnlyList<string>)],
        [typeof(ICollection<string>)],
        [typeof(IReadOnlyCollection<string>)],
        [typeof(ConcurrentBag<string>)]
    ];

    public static IEnumerable<object[]> SpecialTypesData =>
    [
        [typeof(int[,]), typeof(TwoDimensionalArrayHtmlConverter<int[, ]>)],
        [typeof(XmlNode), typeof(XmlNodeHtmlConverter)],
        [typeof(XNode), typeof(XNodeHtmlConverter)]
    ];
    
    public static IEnumerable<object[]> ObjectInstancesData()
    {
        yield return [new Person { Name = "John", Age = 20 }];
        yield return [new { Name = "John", Age = 20 }]; // anonymous
    }
}