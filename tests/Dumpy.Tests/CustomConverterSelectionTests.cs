using Dumpy.Renderers;

namespace Dumpy.Tests;

public class CustomConverterSelectionTests
{
    [Fact]
    public void ReturnsCorrectConverterUsingExactType()
    {
        var type = Dumper.GetUserDefinedConverterType(typeof(Car), new DumpOptions
        {
            Converters = { typeof(CarConverter) }
        });
        
        Assert.Equal(typeof(CarConverter), type);
    }
    
    [Fact]
    public void ReturnsCorrectConverterUsingInterface()
    {
        var type = Dumper.GetUserDefinedConverterType(typeof(ICar), new DumpOptions
        {
            Converters = { typeof(CarConverter) }
        });
        
        Assert.Equal(typeof(CarConverter), type);
    }
    
    [Fact]
    public void ReturnsCorrectConverterUsingDerivedType()
    {
        var type = Dumper.GetUserDefinedConverterType(typeof(FlyingCar), new DumpOptions
        {
            Converters = { typeof(CarConverter) }
        });
        
        Assert.Equal(typeof(CarConverter), type);
    }
    
    [Fact]
    public void DoesNotReturnCorrectConverterUsingBaseType()
    {
        var type = Dumper.GetUserDefinedConverterType(typeof(Vehicle), new DumpOptions
        {
            Converters = { typeof(CarConverter) }
        });
        
        Assert.NotEqual(typeof(CarConverter), type);
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

    public class CarConverter : IConverter<ICar>
    {
        public void Convert(ref ValueStringBuilder writer, ICar? value, Type targetType, DumpOptions options)
        {
            writer.Append("Hello from CarConverter");
        }
    }
}