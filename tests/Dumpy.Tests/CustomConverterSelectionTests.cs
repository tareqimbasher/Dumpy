using Dumpy.Html;
using Dumpy.Html.Converters;

namespace Dumpy.Tests;

public class CustomConverterSelectionTests
{
    [Fact]
    public void ReturnsCorrectConverterUsingExactType()
    {
        var type = Dumper.GetUserDefinedHtmlConverterType(typeof(Car), new DumpOptions
        {
            Converters = { typeof(CarHtmlConverter) }
        });

        Assert.Equal(typeof(CarHtmlConverter), type);
    }

    [Fact]
    public void ReturnsCorrectConverterUsingInterface()
    {
        var type = Dumper.GetUserDefinedHtmlConverterType(typeof(ICar), new DumpOptions
        {
            Converters = { typeof(CarHtmlConverter) }
        });

        Assert.Equal(typeof(CarHtmlConverter), type);
    }

    [Fact]
    public void ReturnsCorrectConverterUsingDerivedType()
    {
        var type = Dumper.GetUserDefinedHtmlConverterType(typeof(FlyingCar), new DumpOptions
        {
            Converters = { typeof(CarHtmlConverter) }
        });

        Assert.Equal(typeof(CarHtmlConverter), type);
    }

    [Fact]
    public void DoesNotReturnCorrectConverterUsingBaseType()
    {
        var type = Dumper.GetUserDefinedHtmlConverterType(typeof(Vehicle), new DumpOptions
        {
            Converters = { typeof(CarHtmlConverter) }
        });

        Assert.NotEqual(typeof(CarHtmlConverter), type);
    }

    public class Vehicle
    {
    }

    public class Car : Vehicle, ICar
    {
        public string Make { get; set; }
    }

    public class FlyingCar : Car
    {
    }

    public interface ICar
    {
        string Make { get; }
    }

    public class CarHtmlConverter : IHtmlConverter<ICar>
    {
        public void Convert(ref ValueStringBuilder writer, ICar? value, Type targetType, HtmlDumpOptions options)
        {
            writer.Append("Hello from CarHtmlConverter");
        }
    }
}