using System.Reflection;
using System.Xml.Linq;
using Dumpy.Renderers.Html.Utils;

namespace Dumpy.Tests.Renderers.Html;

public class StringFormattableHtmlConverterTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(-1)]
    [InlineData(0.0)]
    [InlineData(1.0)]
    [InlineData(5.3)]
    [InlineData(true)]
    [InlineData(false)]
    [InlineData(1.3f)]
    [InlineData(5L)]
    public void ShouldCorrectlySerializePrimitiveValues<T>(T value)
    {
        var html = Dumper.DumpHtml(value);

        Assert.Equal(value!.ToString(), html);
    }

    [Theory]
    [InlineData(StringComparison.Ordinal)]
    [InlineData(DateTimeKind.Utc)]
    [InlineData(BindingFlags.Instance)]
    public void ShouldCorrectlySerializeEnumValues<T>(T value)
    {
        var html = Dumper.DumpHtml(value);

        Assert.Equal(value.ToString(), html);
    }

    [Fact]
    public void ShouldCorrectlySerializeDateTimeValues()
    {
        var date = DateTime.Now;

        var html = Dumper.DumpHtml(date);

        Assert.Equal(HtmlUtil.EscapeText(date.ToString()), html);
    }

    [Fact]
    public void ShouldCorrectlySerializeDateOnlyValues()
    {
        var date = DateOnly.Parse("2001-01-01");

        var html = Dumper.DumpHtml(date);

        Assert.Equal(HtmlUtil.EscapeText(date.ToString()), html);
    }

    [Fact]
    public void ShouldCorrectlySerializeIFormattableValues()
    {
        var val = new FormattableValue();

        var html = Dumper.DumpHtml(val);

        Assert.Equal("FormattableValuePlaceholder", html);
    }

    [Fact]
    public void ShouldCorrectlySerializeNullableValues()
    {
        var val = new int?(1);

        var html = Dumper.DumpHtml(val);

        Assert.Equal("1", html);
    }

    [Fact]
    public void ShouldCorrectlySerializeXNode()
    {
        var x = XElement.Parse("<Message>Hello</Message>");

        var html = Dumper.DumpHtml(x);

        Assert.Equal(HtmlUtil.EscapeText("<Message>Hello</Message>"), html);
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