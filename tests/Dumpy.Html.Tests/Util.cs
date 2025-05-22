namespace Dumpy.Html.Tests;

internal static class Util
{
    private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    private static readonly Random _random = new();

    public static string GenerateRandomString(int length)
    {
        return new string(
            Enumerable
                .Repeat(Chars, length)
                .Select(s => s[_random.Next(s.Length)])
                .ToArray()
        );
    }
}