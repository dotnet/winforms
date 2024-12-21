// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ScratchProject2;

// This project is meant for temporary testing and experimenting and should be kept as simple as possible.

internal static class Program
{
    [STAThread]
    public static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
#pragma warning disable WFO5001
        Application.SetColorMode(SystemColorMode.Classic);
#pragma warning restore WFO5001
        Form1 form = new();
        Application.Run(form);
    }
}
