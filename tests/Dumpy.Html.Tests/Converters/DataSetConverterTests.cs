using System.Data;

namespace Dumpy.Html.Tests.Converters;

public class DataSetConverterTests
{
    private static readonly HtmlDumpOptions _htmlDumpOptionsNoCss = new() { CssClasses = { Enabled = false } };

    [Fact]
    public void ConvertsNull()
    {
        var html = HtmlDumper.DumpHtml<DataSet?>(null, _htmlDumpOptionsNoCss);

        Assert.Equal("<span>null</span>", html);
    }

    [Fact]
    public void ConvertsDataSet()
    {
        var table1 = new DataTable();
        table1.Columns.Add("Name");
        table1.Columns.Add("Age", typeof(int));
        table1.Rows.Add("John Doe", 25);

        var table2 = new DataTable();
        table2.Columns.Add("Item");
        table2.Columns.Add("Price", typeof(decimal));
        table2.Rows.Add("Coffee", 5);

        var dataSet = new DataSet();
        dataSet.Tables.Add(table1);
        dataSet.Tables.Add(table2);

        const string expected = """
                                <table>
                                    <thead>
                                        <tr>
                                            <th colspan="2">NewDataSet (Tables = 2)</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr>
                                            <td>1</td>
                                            <td>
                                                <table>
                                                    <thead>
                                                        <tr>
                                                            <th colspan="3">Table1 (Rows = 1, Columns = 2)</th>
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
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>2</td>
                                            <td>
                                                <table>
                                                    <thead>
                                                        <tr>
                                                            <th colspan="3">Table2 (Rows = 1, Columns = 2)</th>
                                                        </tr>
                                                        <tr>
                                                            <th>Item</th>
                                                            <th>Price</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                        <tr>
                                                            <td>Coffee</td>
                                                            <td>5</td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                                """;

        var html = dataSet.DumpHtml(_htmlDumpOptionsNoCss);

        Assert.Equal(Util.MinimizeHtml(expected), html);
    }

    [Fact]
    public void ShowsDataSetName_IfSpecified()
    {
        var table = new DataTable();
        table.Columns.Add("Name");
        table.Columns.Add("Age", typeof(int));
        table.Rows.Add("John Doe", 25);

        var dataSet = new DataSet("My DataSet");
        dataSet.Tables.Add(table);

        const string expected = """
                                <table>
                                    <thead>
                                        <tr>
                                            <th colspan="2">My DataSet (Tables = 1)</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr>
                                            <td>1</td>
                                            <td>
                                                <table>
                                                    <thead>
                                                        <tr>
                                                            <th colspan="3">Table1 (Rows = 1, Columns = 2)</th>
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
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                                """;

        var html = dataSet.DumpHtml(_htmlDumpOptionsNoCss);

        Assert.Equal(Util.MinimizeHtml(expected), html);
    }
}