// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing.Imaging;

namespace ScratchProject;

// This project is meant for temporary testing and experimenting and should be kept as simple as possible.

internal static class Program
{
    [STAThread]
    public static void Main()
    {
        long bytes = GC.GetAllocatedBytesForCurrentThread();
        var info = ImageCodecInfo.GetImageEncoders();
        long afterBytes = GC.GetAllocatedBytesForCurrentThread();
        string wow = $"ImageCodecInfo.GetImageDecoders() allocated {afterBytes - bytes} bytes";

        Application.EnableVisualStyles();
        Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);



        Application.Run(new Form1());
    }
}
