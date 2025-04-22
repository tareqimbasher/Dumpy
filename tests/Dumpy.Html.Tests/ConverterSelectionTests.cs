using System.Collections;
using System.Collections.Concurrent;
using System.Xml;
using System.Xml.Linq;
using Dumpy.Html.Converters;
using Dumpy.Tests.Models;

namespace Dumpy.Html.Tests;

public class ConverterSelectionTests
{
    [Theory]
    [InlineData(typeof(int))]
    [InlineData(typeof(int?))]
    [InlineData(typeof(long))]
    [InlineData(typeof(long?))]
    [InlineData(typeof(float))]
    [InlineData(typeof(float?))]
    [InlineData(typeof(double))]
    [InlineData(typeof(double?))]
    [InlineData(typeof(decimal))]
    [InlineData(typeof(decimal?))]
    [InlineData(typeof(DateTime))]
    [InlineData(typeof(DateTime?))]
    [InlineData(typeof(DateOnly))]
    [InlineData(typeof(DateOnly?))]
    [InlineData(typeof(StringComparison))]
    [InlineData(typeof(StringComparison?))]
    [InlineData(typeof(string))]
    [InlineData(typeof(Exception))]
    public void StringFormattableTypes(Type type)
    {
        var converter = HtmlDumper.GetGenericConverter(type);
        
        Assert.Equal(typeof(StringHtmlConverter), converter.GetType());
    }
    
    [Fact]
    public void RegularObjectTypes()
    {
        var o = new Person
        {
            Name = "John",
            Age = 20
        };
        
        var converter = HtmlDumper.GetGenericConverter(o.GetType());
        
        Assert.Equal(typeof(ObjectHtmlConverter), converter.GetType());
    }
    
    [Fact]
    public void AnonTypes()
    {
        var o = new
        {
            Name = "John",
            Age = 20
        };
        
        var converter = HtmlDumper.GetGenericConverter(o.GetType());
        
        Assert.Equal(typeof(ObjectHtmlConverter), converter.GetType());
    }
    
    [Theory]
    [InlineData(typeof(Array))]
    [InlineData(typeof(int[]))]
    [InlineData(typeof(List<>))]
    [InlineData(typeof(IEnumerable))]
    [InlineData(typeof(IEnumerable<>))]
    [InlineData(typeof(IList<>))]
    [InlineData(typeof(IReadOnlyList<>))]
    [InlineData(typeof(ICollection<>))]
    [InlineData(typeof(IReadOnlyCollection<>))]
    [InlineData(typeof(ConcurrentBag<>))]
    public void CollectionTypes(Type type)
    {
        var converter = HtmlDumper.GetGenericConverter(type);
        
        Assert.Equal(typeof(CollectionHtmlConverter), converter.GetType());
    }
    
    [Theory]
    [InlineData(typeof(XmlNode), typeof(XmlNodeHtmlConverter))]
    [InlineData(typeof(XNode), typeof(XNodeHtmlConverter))]
    public void OtherTypes(Type targetType, Type expectedConverterType)
    {
        var converter = HtmlDumper.GetUserDefinedConverterType(targetType, new HtmlDumpOptions());
        
        Assert.NotNull(converter);
        Assert.Equal(expectedConverterType, converter);
    }
}
