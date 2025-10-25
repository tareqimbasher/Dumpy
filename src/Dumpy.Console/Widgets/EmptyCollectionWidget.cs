using Spectre.Console;

namespace Dumpy.Console.Widgets;

public class EmptyCollectionWidget
{
    public static Markup New(string collectionTypeName, ConsoleDumpOptions options) =>
        new($"0 items | {collectionTypeName}", options.Styles.EmptyCollection);
}