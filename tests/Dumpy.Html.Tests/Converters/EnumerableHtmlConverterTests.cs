namespace Dumpy.Html.Tests.Converters;

public class EnumerableHtmlConverterTests
{
    [Fact]
    public void ShouldSerializeSimpleCollection()
    {
        var collection = new[] { "one", "two", "three" };

        var expected = """
                       <table>
                           <thead class="dm-t-info">
                               <tr>
                                   <th>String[] (3 items)</th>
                               </tr>
                           </thead>
                           <tbody>
                               <tr>
                                   <td>one</td>
                               </tr>
                               <tr>
                                   <td>two</td>
                               </tr>
                               <tr>
                                   <td>three</td>
                               </tr>
                           </tbody>
                       </table>
                       """;
        
        var html = HtmlDumper.DumpHtml(collection);
        
        Assert.Equal(Util.MinimizeHtml(expected), html);
    }
    
    [Fact]
    public void ShouldSerializeObjectCollection()
    {
        var collection = new[] { Consts.Person };
        
        var html = HtmlDumper.DumpHtml(collection);
        
        Assert.Equal(Util.MinimizeHtml(Consts.PersonTableHtmlRepresentation), html);
    }
}