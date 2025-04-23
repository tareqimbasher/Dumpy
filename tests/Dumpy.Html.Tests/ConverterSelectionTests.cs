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
        var options = new HtmlDumpOptions();
        var converter = options.GetConverter(type);
        
        Assert.Equal(typeof(StringDefaultHtmlConverter<>).MakeGenericType(type), converter.GetType());
    }
    
    [Fact]
    public void RegularObjectTypes()
    {
        var o = new Person
        {
            Name = "John",
            Age = 20
        };
        
        var options = new HtmlDumpOptions();
        var converter = options.GetConverter(o.GetType());
        
        Assert.Equal(typeof(ObjectDefaultHtmlConverter<>).MakeGenericType(o.GetType()), converter.GetType());
    }
    
    [Fact]
    public void AnonTypes()
    {
        var o = new
        {
            Name = "John",
            Age = 20
        };
        
        var options = new HtmlDumpOptions();
        var converter = options.GetConverter(o.GetType());
        
        Assert.Equal(typeof(ObjectDefaultHtmlConverter<>).MakeGenericType(o.GetType()), converter.GetType());
    }
    
    [Theory]
    [InlineData(typeof(Array))]
    [InlineData(typeof(int[]))]
    [InlineData(typeof(List<string>))]
    [InlineData(typeof(IEnumerable))]
    [InlineData(typeof(IEnumerable<string>))]
    [InlineData(typeof(IList<string>))]
    [InlineData(typeof(IReadOnlyList<string>))]
    [InlineData(typeof(ICollection<string>))]
    [InlineData(typeof(IReadOnlyCollection<string>))]
    [InlineData(typeof(ConcurrentBag<string>))]
    public void CollectionTypes(Type type)
    {
        var options = new HtmlDumpOptions();
        var converter = options.GetConverter(type);
        
        Assert.Equal(typeof(IEnumerableDefautHtmlConverter<>).MakeGenericType(type), converter.GetType());
    }
    
    [Theory]
    [InlineData(typeof(XmlNode), typeof(XmlNodeHtmlConverter))]
    [InlineData(typeof(XNode), typeof(XNodeHtmlConverter))]
    public void OtherTypes(Type targetType, Type expectedConverterType)
    {
        var options = new HtmlDumpOptions();
        var converter = options.GetConverter(targetType);
        
        Assert.NotNull(converter);
        Assert.Equal(expectedConverterType, converter.GetType());
    }
}
