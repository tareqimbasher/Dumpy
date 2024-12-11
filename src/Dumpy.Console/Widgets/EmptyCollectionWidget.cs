using Spectre.Console;

namespace Dumpy.Console.Widgets;

public class EmptyCollectionWidget
{
    private static readonly Style _style = new(decoration: Decoration.Bold | Decoration.Dim);
    
    public static Markup New(string collectionTypeName) => new($"0 items | {collectionTypeName}", _style);
}