namespace Dumpy.Tests.Renderers.Html;

internal static class Util
{
    private static readonly Random _random = new();
    const string _chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    public static string GenerateRandomString(int length)
    {
        return new string(
            Enumerable
                .Repeat(_chars, length)
                .Select(s => s[_random.Next(s.Length)])
                .ToArray()
        );
    }
}