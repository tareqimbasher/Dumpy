using System;
using System.Collections.Generic;

namespace Dumpy;

// HTML => get a string and then they are responsible for doing with it whatever they want
// Console => option 1: dumps directly to the console using .DumpConsole()
//            option 2: returns IRenderable and they are responsible...etc using .DumpToRenderable()

public static class ConfigureDumpy
{
    public static DumpyOptions Options { get; } = new();

    public static void Update(Action<DumpyOptions> configure)
    {
        configure(Options);
    }
}

public record DumpyOptions
{
    public Uri? RemoteViewerUri { get; private set; }

    public void UseRemoteViewer(Uri uri)
    {
        RemoteViewerUri = uri;
    }
}

// public class DumpOutput
// {
//     public long Order { get; set; }
//     public string Channel { get; set; }
//     public string Html { get; set; }
//     public Dictionary<string, string?> Metadata { get; set; }
// }
//
// public class VsCodeExtensionForwarder
// {
//     public void Forward(string html)
//     {
//     }
// }