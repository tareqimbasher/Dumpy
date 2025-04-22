using System.Reflection;
using System.Xml.Linq;
using Dumpy.Html.Utils;

// ReSharper disable InvokeAsExtensionMethod

namespace Dumpy.Html.Tests.Converters;

public class StringHtmlConverterTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(-1)]
    [InlineData((uint)4)]
    [InlineData((sbyte)-4)]
    [InlineData(0.0)]
    [InlineData(1.0)]
    [InlineData(5.3)]
    [InlineData(true)]
    [InlineData(false)]
    [InlineData(1.3f)]
    [InlineData(5L)]
    public void ShouldCorrectlySerializePrimitiveValues<T>(T value)
    {
        var html = HtmlDumper.DumpHtml(value);

        Assert.Equal(value!.ToString(), html);
    }

    [Theory]
    [InlineData(StringComparison.Ordinal)]
    [InlineData(DateTimeKind.Utc)]
    [InlineData(BindingFlags.Instance)]
    [InlineData(BindingFlags.Instance | BindingFlags.Public)]
    public void ShouldCorrectlySerializeEnumValues<T>(T value)
    {
        var html = HtmlDumper.DumpHtml(value);

        Assert.Equal(value!.ToString(), html);
    }

    [Fact]
    public void ShouldCorrectlySerializeDateTimeValues()
    {
        var date = DateTime.Now;

        var html = HtmlDumper.DumpHtml(date);

        Assert.Equal(HtmlUtil.EscapeText(date.ToString()), html);
    }

    [Fact]
    public void ShouldCorrectlySerializeDateOnlyValues()
    {
        var date = DateOnly.Parse("2001-01-01");

        var html = HtmlDumper.DumpHtml(date);

        Assert.Equal(HtmlUtil.EscapeText(date.ToString()), html);
    }

    [Fact]
    public void ShouldCorrectlySerializeIFormattableValues()
    {
        var val = new FormattableValue();

        var html = HtmlDumper.DumpHtml(val);

        Assert.Equal("FormattableValuePlaceholder", html);
    }

    [Fact]
    public void ShouldCorrectlySerializeNullableValues()
    {
        var val = new int?(1);

        var html = HtmlDumper.DumpHtml(val);

        Assert.Equal("1", html);
    }
}

public class FormattableValue : IFormattable
{
    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        FormattableString formattable = $"FormattableValuePlaceholder";
        return formattable.ToString(formatProvider);
    }

    public override string ToString()
    {
        return "FormattableValuePlaceholder";
    }
}