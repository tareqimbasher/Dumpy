namespace Dumpy.Html.Tests.Converters;

public class FileSystemInfoConverterTests
{
    private static readonly string _fileExpectedHtml = Util.MinimizeHtml(
        """
        <table>
            <thead>
            <tr class="dm-t-info">
                <th colspan="2" title="System.IO.FileInfo">FileInfo</th>
            </tr>
            </thead>
            <tbody>
            <tr>
                <th title="System.String">Name</th>
                <td>exist.txt</td>
            </tr>
            <tr>
                <th title="System.IO.DirectoryInfo">Directory</th>
                <td>
                    <table>
                        <thead>
                        <tr class="dm-t-info">
                            <th colspan="2" title="System.IO.DirectoryInfo">DirectoryInfo</th>
                        </tr>
                        </thead>
                        <tbody>
                        <tr>
                            <th title="System.String">Name</th>
                            <td>not</td>
                        </tr>
                        <tr>
                            <th title="System.Boolean">Exists</th>
                            <td>False</td>
                        </tr>
                        <tr>
                            <th title="System.String">FullName</th>
                            <td>/does/not</td>
                        </tr>
                        <tr>
                            <th title="System.String">Extension</th>
                            <td></td>
                        </tr>
                        <tr>
                            <th title="System.DateTime">CreationTime</th>
                            <td>1/1/1601 2:20:00 AM</td>
                        </tr>
                        <tr>
                            <th title="System.DateTime">CreationTimeUtc</th>
                            <td>1/1/1601 12:00:00 AM</td>
                        </tr>
                        <tr>
                            <th title="System.DateTime">LastAccessTime</th>
                            <td>1/1/1601 2:20:00 AM</td>
                        </tr>
                        <tr>
                            <th title="System.DateTime">LastAccessTimeUtc</th>
                            <td>1/1/1601 12:00:00 AM</td>
                        </tr>
                        <tr>
                            <th title="System.DateTime">LastWriteTime</th>
                            <td>1/1/1601 2:20:00 AM</td>
                        </tr>
                        <tr>
                            <th title="System.DateTime">LastWriteTimeUtc</th>
                            <td>1/1/1601 12:00:00 AM</td>
                        </tr>
                        <tr>
                            <th title="System.String">LinkTarget</th>
                            <td><span class="dm-null">null</span></td>
                        </tr>
                        <tr>
                            <th title="System.IO.UnixFileMode">UnixFileMode</th>
                            <td>-1</td>
                        </tr>
                        <tr>
                            <th title="System.IO.FileAttributes">Attributes</th>
                            <td>-1</td>
                        </tr>
                        </tbody>
                    </table>
                </td>
            </tr>
            <tr>
                <th title="System.Boolean">Exists</th>
                <td>False</td>
            </tr>
            <tr>
                <th title="System.String">FullName</th>
                <td>/does/not/exist.txt</td>
            </tr>
            <tr>
                <th title="System.String">Extension</th>
                <td>.txt</td>
            </tr>
            <tr>
                <th title="System.DateTime">CreationTime</th>
                <td>1/1/1601 2:20:00 AM</td>
            </tr>
            <tr>
                <th title="System.DateTime">CreationTimeUtc</th>
                <td>1/1/1601 12:00:00 AM</td>
            </tr>
            <tr>
                <th title="System.DateTime">LastAccessTime</th>
                <td>1/1/1601 2:20:00 AM</td>
            </tr>
            <tr>
                <th title="System.DateTime">LastAccessTimeUtc</th>
                <td>1/1/1601 12:00:00 AM</td>
            </tr>
            <tr>
                <th title="System.DateTime">LastWriteTime</th>
                <td>1/1/1601 2:20:00 AM</td>
            </tr>
            <tr>
                <th title="System.DateTime">LastWriteTimeUtc</th>
                <td>1/1/1601 12:00:00 AM</td>
            </tr>
            <tr>
                <th title="System.String">LinkTarget</th>
                <td><span class="dm-null">null</span></td>
            </tr>
            <tr>
                <th title="System.IO.UnixFileMode">UnixFileMode</th>
                <td>-1</td>
            </tr>
            <tr>
                <th title="System.IO.FileAttributes">Attributes</th>
                <td>-1</td>
            </tr>
            </tbody>
        </table>
        """);

    private static readonly string _directoryExpectedHtml = Util.MinimizeHtml(
        """
        <table>
            <thead>
            <tr class="dm-t-info">
                <th colspan="2" title="System.IO.DirectoryInfo">DirectoryInfo</th>
            </tr>
            </thead>
            <tbody>
            <tr>
                <th title="System.String">Name</th>
                <td>exist</td>
            </tr>
            <tr>
                <th title="System.Boolean">Exists</th>
                <td>False</td>
            </tr>
            <tr>
                <th title="System.String">FullName</th>
                <td>/does/not/exist</td>
            </tr>
            <tr>
                <th title="System.String">Extension</th>
                <td></td>
            </tr>
            <tr>
                <th title="System.DateTime">CreationTime</th>
                <td>1/1/1601 2:20:00 AM</td>
            </tr>
            <tr>
                <th title="System.DateTime">CreationTimeUtc</th>
                <td>1/1/1601 12:00:00 AM</td>
            </tr>
            <tr>
                <th title="System.DateTime">LastAccessTime</th>
                <td>1/1/1601 2:20:00 AM</td>
            </tr>
            <tr>
                <th title="System.DateTime">LastAccessTimeUtc</th>
                <td>1/1/1601 12:00:00 AM</td>
            </tr>
            <tr>
                <th title="System.DateTime">LastWriteTime</th>
                <td>1/1/1601 2:20:00 AM</td>
            </tr>
            <tr>
                <th title="System.DateTime">LastWriteTimeUtc</th>
                <td>1/1/1601 12:00:00 AM</td>
            </tr>
            <tr>
                <th title="System.String">LinkTarget</th>
                <td><span class="dm-null">null</span></td>
            </tr>
            <tr>
                <th title="System.IO.UnixFileMode">UnixFileMode</th>
                <td>-1</td>
            </tr>
            <tr>
                <th title="System.IO.FileAttributes">Attributes</th>
                <td>-1</td>
            </tr>
            </tbody>
        </table>
        """);

    [Fact]
    public void ConvertsFileInfo()
    {
        var file = new FileInfo("/does/not/exist.txt");

        var html = HtmlDumper.DumpHtml(file, new HtmlDumpOptions { AddTitleAttributes = true });

        Assert.Equal(_fileExpectedHtml, html);
    }

    [Fact]
    public void ConvertsDirectoryInfo()
    {
        var dir = new DirectoryInfo("/does/not/exist");

        var html = HtmlDumper.DumpHtml(dir, new HtmlDumpOptions { AddTitleAttributes = true });

        Assert.Equal(_directoryExpectedHtml, html);
    }
}