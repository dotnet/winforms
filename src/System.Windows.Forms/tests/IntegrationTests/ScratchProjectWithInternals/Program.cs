﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ScratchProjectWithInternals;

// This project is meant for temporary testing and experimenting with access to internals and should be kept as simple as possible.

internal static class Program
{
    [STAThread]
    public unsafe static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
        Application.Run(new Form1());
    }
}
