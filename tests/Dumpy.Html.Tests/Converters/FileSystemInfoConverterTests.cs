namespace Dumpy.Html.Tests.Converters;

public class FileSystemInfoConverterTests
{
    private static readonly HtmlDumpOptions _htmlDumpOptionsNoCss = new() { CssClasses = { Enabled = false } };

    [Fact]
    public void ConvertsNull()
    {
        var html = HtmlDumper.DumpHtml<FileInfo?>(null, _htmlDumpOptionsNoCss);

        Assert.Equal("<span>null</span>", html);
    }

    [Fact]
    public void ConvertsFileInfo()
    {
        var file = new FileInfo("/does/not/exist.txt");

        var html = HtmlDumper.DumpHtml(file, new HtmlDumpOptions { AddTitleAttributes = true });

        Assert.Contains("FileInfo", html);
        Assert.Contains(file.Name, html);
        Assert.Contains(file.FullName, html);
        Assert.Contains(">Exists</th>", html);
        Assert.Contains(">False<", html);
        Assert.Contains(">Extension</th>", html);
        Assert.Contains(">.txt<", html);
        Assert.Contains(">LinkTarget</th>", html);
        Assert.Contains(">Attributes</th>", html);
        Assert.Contains(">CreationTime</th>", html);
        Assert.Contains(">CreationTimeUtc</th>", html);
        Assert.Contains(">LastAccessTime</th>", html);
        Assert.Contains(">LastWriteTime</th>", html);
    }

    [Fact]
    public void ConvertsFileInfo_ContainsNestedDirectoryInfo()
    {
        var file = new FileInfo("/does/not/exist.txt");

        var html = HtmlDumper.DumpHtml(file, new HtmlDumpOptions { AddTitleAttributes = true });

        Assert.Contains("DirectoryInfo", html);
        Assert.Contains(file.Directory!.Name, html);
        Assert.Contains(file.Directory.FullName, html);
    }

    [Fact]
    public void ConvertsDirectoryInfo()
    {
        var dir = new DirectoryInfo("/does/not/exist");

        var html = HtmlDumper.DumpHtml(dir, new HtmlDumpOptions { AddTitleAttributes = true });

        Assert.Contains("DirectoryInfo", html);
        Assert.Contains(dir.Name, html);
        Assert.Contains(dir.FullName, html);
        Assert.Contains(">Exists</th>", html);
        Assert.Contains(">False<", html);
        Assert.Contains(">LinkTarget</th>", html);
        Assert.Contains(">Attributes</th>", html);
        Assert.Contains(">CreationTime</th>", html);
        Assert.Contains(">CreationTimeUtc</th>", html);
        Assert.Contains(">LastAccessTime</th>", html);
        Assert.Contains(">LastWriteTime</th>", html);
    }
}
