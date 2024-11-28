using Dumpy.Html.Converters;

namespace Dumpy.Html.Tests;

public class CustomConverterSelectionTests
{
    [Fact]
    public void ReturnsCorrectConverterUsingExactType()
    {
        var type = Dumper.GetUserDefinedConverterType(typeof(Car), new DumpOptions
        {
            Converters = { typeof(CarHtmlConverter) }
        });

        Assert.Equal(typeof(CarHtmlConverter), type);
    }

    [Fact]
    public void ReturnsCorrectConverterUsingInterface()
    {
        var type = Dumper.GetUserDefinedConverterType(typeof(ICar), new DumpOptions
        {
            Converters = { typeof(CarHtmlConverter) }
        });

        Assert.Equal(typeof(CarHtmlConverter), type);
    }

    [Fact]
    public void ReturnsCorrectConverterUsingDerivedType()
    {
        var type = Dumper.GetUserDefinedConverterType(typeof(FlyingCar), new DumpOptions
        {
            Converters = { typeof(CarHtmlConverter) }
        });

        Assert.Equal(typeof(CarHtmlConverter), type);
    }

    [Fact]
    public void DoesNotReturnCorrectConverterUsingBaseType()
    {
        var type = Dumper.GetUserDefinedConverterType(typeof(Vehicle), new DumpOptions
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