using System.Data;

namespace Dumpy.Html.Tests.Converters;

public class DataTableConverterTests
{
    private static readonly HtmlDumpOptions _htmlDumpOptionsNoCss = new() { CssClasses = { Enabled = false } };
    
    [Fact]
    public void ConvertsNull()
    {
        var html = HtmlDumper.DumpHtml<DataTable?>(null, _htmlDumpOptionsNoCss);

        Assert.Equal("<span>null</span>", html);
    }
    
    [Fact]
    public void ConvertsDataTable()
    {
        var table = new DataTable();
        table.Columns.Add("Name");
        table.Columns.Add("Age", typeof(int));
        table.Rows.Add("John Doe", 25);
        const string expected = """
                                <table>
                                    <thead>
                                        <tr>
                                            <th colspan="3">DataTable (Rows = 1, Columns = 2)</th>
                                        </tr>
                                        <tr>
                                            <th>Name</th>
                                            <th>Age</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr>
                                            <td>John Doe</td>
                                            <td>25</td>
                                        </tr>
                                    </tbody>
                                </table>
                                """;
        
        var html = table.DumpHtml(_htmlDumpOptionsNoCss);

        Assert.Equal(Util.MinimizeHtml(expected), html);
    }
    
    [Fact]
    public void ShowsDataTableName_IfSpecified()
    {
        var table = new DataTable("My Table");
        table.Columns.Add("Name");
        table.Columns.Add("Age", typeof(int));
        table.Rows.Add("John Doe", 25);
        const string expected = """
                                <table>
                                    <thead>
                                        <tr>
                                            <th colspan="3">My Table (Rows = 1, Columns = 2)</th>
                                        </tr>
                                        <tr>
                                            <th>Name</th>
                                            <th>Age</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr>
                                            <td>John Doe</td>
                                            <td>25</td>
                                        </tr>
                                    </tbody>
                                </table>
                                """;
        
        var html = table.DumpHtml(_htmlDumpOptionsNoCss);

        Assert.Equal(Util.MinimizeHtml(expected), html);
    }
}