namespace Dumpy.Html.Tests;

public class HtmlDumpOptionsCssClassesTests
{
    [Fact]
    public void CssClasses_Null()
    {
        var options = new HtmlDumpOptions();
        options.CssClasses.Null = "null-class";

        string? text = null;
        string expected = "<span class=\"null-class\">null</span>";

        var html = HtmlDumper.DumpHtml(text, options);

        Assert.Equal(expected, html);
    }

    [Fact]
    public void CssClasses_EmptyCollection()
    {
        var options = new HtmlDumpOptions();
        options.CssClasses.EmptyCollection = "empty-col-class";

        string[] array = [];
        string expected = $"""
                           <table class="empty-col-class">
                               <thead class="{CssClassOptions.DefaultTableInfoHeaderCssClass}">
                                   <tr>
                                       <th>String[] (0 items)</th>
                                   </tr>
                               </thead>
                           </table>
                           """;

        var html = HtmlDumper.DumpHtml(array, options);

        Assert.Equal(Util.MinimizeHtml(expected), html);
    }

    [Fact]
    public void CssClasses_TableInfoHeader()
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

    [Fact]
    public void CssClasses_TableDataHeader()
    {
        var options = new HtmlDumpOptions();
        options.CssClasses.TableDataHeader = "data-header-class";

        int[,] numbers = { { 1, 4 }, { 3, 6 } };
        string expected = $"""
                           <table>
                               <thead>
                                   <tr class="{CssClassOptions.DefaultTableInfoHeaderCssClass}">
                                       <th colspan="3">Int32[,](Rows = 2, Columns = 2) (4 items)</th>
                                   </tr>
                                   <tr class="data-header-class">
                                       <th></th>
                                       <th>0</th>
                                       <th>1</th>
                                   </tr>
                               </thead>
                               <tbody>
                                   <tr>
                                       <th>0</th>
                                       <td>1</td>
                                       <td>4</td>
                                   </tr>
                                   <tr>
                                       <th>1</th>
                                       <td>3</td>
                                       <td>6</td>
                                   </tr>
                               </tbody>
                           </table>
                           """;

        var html = HtmlDumper.DumpHtml(numbers, options);

        Assert.Equal(Util.MinimizeHtml(expected), html);
    }
}