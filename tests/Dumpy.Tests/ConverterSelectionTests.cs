using System.Collections;
using System.Collections.Concurrent;
using System.Xml;
using System.Xml.Linq;
using Dumpy.Renderers.Html;
using Dumpy.Tests.Renderers.Html.Models;

namespace Dumpy.Tests;

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
    [InlineData(typeof(XNode))]
    [InlineData(typeof(XmlNode))]
    public void StringFormattableTypes(Type type)
    {
        var converter = Dumper.GetGenericConverter(type);
        
        Assert.Equal(typeof(StringFormattableConverter), converter.GetType());
    }
    
    [Fact]
    public void RegularObjectTypes()
    {
        var o = new Person
        {
            Name = "John",
            Age = 20
        };
        
        var converter = Dumper.GetGenericConverter(o.GetType());
        
        Assert.Equal(typeof(ObjectConverter), converter.GetType());
    }
    
    [Fact]
    public void AnonTypes()
    {
        var o = new
        {
            Name = "John",
            Age = 20
        };
        
        var converter = Dumper.GetGenericConverter(o.GetType());
        
        Assert.Equal(typeof(ObjectConverter), converter.GetType());
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
        var converter = Dumper.GetGenericConverter(type);
        
        Assert.Equal(typeof(CollectionConverter), converter.GetType());
    }
}
