// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using Microsoft.DotNet.RemoteExecutor;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Gdi;
using Windows.Win32.System.Threading;
using Xunit.Sdk;

namespace System.Drawing.Tests;

public static class GdiPlusHandlesTests
{
    public static bool IsDrawingAndRemoteExecutorSupported => RemoteExecutor.IsSupported;

    [ConditionalFact(nameof(IsDrawingAndRemoteExecutorSupported))]
    public static void GraphicsDrawIconDoesNotLeakHandles()
    {
        RemoteExecutor.Invoke(() =>
        {
            const int HandleThreshold = 1;
            using Bitmap bmp = new(100, 100);
            using Icon ico = new(Helpers.GetTestBitmapPath("16x16_one_entry_4bit.ico"));

            using GetDcScope hdc = new(PInvokeCore.GetForegroundWindow());
            using Graphics graphicsFromHdc = Graphics.FromHdc(hdc);

            using Process currentProcess = Process.GetCurrentProcess();
            HANDLE processHandle = new(currentProcess.Handle);

            uint initialHandles = PInvokeCore.GetGuiResources(processHandle, GET_GUI_RESOURCES_FLAGS.GR_GDIOBJECTS);
            ValidateNoWin32Error(initialHandles);

            for (int i = 0; i < 5000; i++)
            {
                graphicsFromHdc.DrawIcon(ico, 100, 100);
            }

            uint finalHandles = PInvokeCore.GetGuiResources(processHandle, GET_GUI_RESOURCES_FLAGS.GR_GDIOBJECTS);
            ValidateNoWin32Error(finalHandles);

            Assert.InRange(finalHandles, initialHandles - HandleThreshold, initialHandles + HandleThreshold);
        }).Dispose();
    }

    private static void ValidateNoWin32Error(uint handleCount)
    {
        if (handleCount == 0)
        {
            int error = Marshal.GetLastWin32Error();

            if (error != 0)
                throw new XunitException($"GetGuiResources failed with win32 error: {error}");
        }
    }
}
