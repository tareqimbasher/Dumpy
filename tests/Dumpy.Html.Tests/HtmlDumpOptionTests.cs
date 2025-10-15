namespace Dumpy.Html.Tests;

public class HtmlDumpOptionTests
{
    [Fact]
    public void AddTitleAttributes()
    {
        var options = new HtmlDumpOptions { AddTitleAttributes = true };
        var array = new[] { 1, 2, 3 };
        const string expected = """
                                <table>
                                    <thead class="dm-t-info">
                                        <tr>
                                            <th>Int32[] (3 items)</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr>
                                            <td title="System.Int32">1</td>
                                        </tr>
                                        <tr>
                                            <td title="System.Int32">2</td>
                                        </tr>
                                        <tr>
                                            <td title="System.Int32">3</td>
                                        </tr>
                                    </tbody>
                                </table>
                                """;

        var html = HtmlDumper.DumpHtml(array, options);

        Assert.Equal(Util.MinimizeHtml(expected), html);
    }

    [Fact]
    public void CssClasses_Disabled()
    {
        var options = new HtmlDumpOptions();
        options.CssClasses.Enabled = false;

        var array = new[] { 1 };
        const string expected = """
                                <table>
                                    <thead>
                                        <tr>
                                            <th>Int32[] (1 items)</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr>
                                            <td>1</td>
                                        </tr>
                                    </tbody>
                                </table>
                                """;

        var html = HtmlDumper.DumpHtml(array, options);

        Assert.Equal(Util.MinimizeHtml(expected), html);
    }

    [Fact]
    public void CssClasses_TableDataHeader()
    {
        var options = new HtmlDumpOptions();
        options.CssClasses.TableInfoHeader = "info-header-class";

        var array = new[] { 1 };
        const string expected = """
                                <table>
                                    <thead class="info-header-class">
                                        <tr>
                                            <th>Int32[] (1 items)</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr>
                                            <td>1</td>
                                        </tr>
                                    </tbody>
                                </table>
                                """;

        var html = HtmlDumper.DumpHtml(array, options);

        Assert.Equal(Util.MinimizeHtml(expected), html);
    }
}