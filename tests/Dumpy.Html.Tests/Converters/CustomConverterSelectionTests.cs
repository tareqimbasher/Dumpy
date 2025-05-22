using Dumpy.Html.Converters;

namespace Dumpy.Html.Tests.Converters;

public class CustomConverterSelectionTests
{
    [Fact]
    public void ReturnsCorrectConverterUsingExactType()
    {
        var converter = new HtmlDumpOptions
        {
            Converters = { new CarHtmlConverter() }
        }.GetConverter(typeof(Car));

        Assert.Equal(typeof(CarHtmlConverter), converter.GetType());
    }

    [Fact]
    public void ReturnsCorrectConverterUsingInterface()
    {
        var converter = new HtmlDumpOptions
        {
            Converters = { new CarHtmlConverter() }
        }.GetConverter(typeof(ICar));

        Assert.Equal(typeof(CarHtmlConverter), converter.GetType());
    }

    [Fact]
    public void ReturnsCorrectConverterUsingDerivedType()
    {
        var converter = new HtmlDumpOptions
        {
            Converters = { new CarHtmlConverter() }
        }.GetConverter(typeof(FlyingCar));

        Assert.Equal(typeof(CarHtmlConverter), converter.GetType());
    }

    [Fact]
    public void DoesNotReturnCorrectConverterUsingBaseType()
    {
        var converter = new HtmlDumpOptions
        {
            Converters = { new CarHtmlConverter() }
        }.GetConverter(typeof(Vehicle));

        Assert.NotEqual(typeof(CarHtmlConverter), converter.GetType());
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

    public class CarHtmlConverter : HtmlConverter<ICar>
    {
        public override void Convert(ref ValueStringBuilder writer, ICar? value, Type targetType,
            HtmlDumpOptions options)
        {
            writer.Append("Hello from CarHtmlConverter");
        }
    }
}