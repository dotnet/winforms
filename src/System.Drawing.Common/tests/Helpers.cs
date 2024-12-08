// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing.Printing;
using System.Text;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Gdi;
using Xunit.Sdk;

namespace System.Drawing;

public static unsafe class Helpers
{
    // This MUST come before s_anyInstalledPrinters. Caching for performance in tests.
    public static PrinterSettings.StringCollection InstalledPrinters { get; } = PrinterSettings.InstalledPrinters;

    private static readonly bool s_anyInstalledPrinters = InstalledPrinters.Count > 0;

    public const string AnyInstalledPrinters = $"{nameof(Helpers)}.{nameof(AreAnyPrintersInstalled)}";

    public static bool AreAnyPrintersInstalled() => s_anyInstalledPrinters;

    public static string GetTestBitmapPath(string fileName) => GetTestPath("bitmaps", fileName);
    public static string GetTestFontPath(string fileName) => GetTestPath("fonts", fileName);
    public static string GetTestColorProfilePath(string fileName) => GetTestPath("colorProfiles", fileName);

    private static string GetTestPath(string directoryName, string fileName) => Path.Combine(AppContext.BaseDirectory, directoryName, fileName);

    public static void VerifyBitmap(Bitmap bitmap, Color[][] colors)
    {
        for (int y = 0; y < colors.Length; y++)
        {
            for (int x = 0; x < colors[y].Length; x++)
            {
                Color expectedColor = Color.FromArgb(colors[y][x].ToArgb());
                Color actualColor = bitmap.GetPixel(x, y);

                if (expectedColor != actualColor)
                {
                    throw GetBitmapEqualFailureException(bitmap, colors, x, y);
                }
            }
        }
    }

    private static EqualException GetBitmapEqualFailureException(Bitmap bitmap, Color[][] colors, int firstFailureX, int firstFailureY)
    {
        // Print out the whole bitmap to provide a view of the whole image, rather than just the difference between
        // a single pixel.
        StringBuilder actualStringBuilder = new();
        StringBuilder expectedStringBuilder = new();

        actualStringBuilder.AppendLine();
        expectedStringBuilder.AppendLine();

        for (int y = 0; y < bitmap.Height; y++)
        {
            for (int x = 0; x < bitmap.Width; x++)
            {
                PrintColor(actualStringBuilder, bitmap.GetPixel(x, y));
                PrintColor(expectedStringBuilder, colors[y][x]);
                if (x != bitmap.Width - 1)
                {
                    actualStringBuilder.Append(", ");
                    expectedStringBuilder.Append(", ");
                }
            }

            actualStringBuilder.AppendLine();
            expectedStringBuilder.AppendLine();
        }

        return EqualException.ForMismatchedValues(
            expectedStringBuilder.ToString(),
            actualStringBuilder.ToString(),
            $"Bitmaps were different at {firstFailureX}, {firstFailureY}.");
    }

    private static void PrintColor(StringBuilder stringBuilder, Color color)
    {
        stringBuilder.Append($"Color.FromArgb({color.A}, {color.R}, {color.G}, {color.B})");
    }

    public static Color EmptyColor => Color.FromArgb(0, 0, 0, 0);

    internal static Rectangle GetWindowDCRect(HDC hdc) => GetHWndRect(PInvokeCore.WindowFromDC(hdc));

    internal static Rectangle GetHWndRect(HWND hwnd)
    {
        if (hwnd.IsNull)
        {
            return GetMonitorRectForWindow(hwnd);
        }

        PInvokeCore.GetClientRect(hwnd, out RECT rect);
        return rect;
    }

    private static Rectangle GetMonitorRectForWindow(HWND hwnd)
    {
        HMONITOR hmonitor = PInvokeCore.MonitorFromWindow(hwnd, MONITOR_FROM_FLAGS.MONITOR_DEFAULTTOPRIMARY);
        ((nint)hmonitor.Value).Should().NotBe(0);

        MONITORINFO info = new()
        {
            cbSize = (uint)sizeof(MONITORINFO)
        };

        PInvokeCore.GetMonitorInfo(hmonitor, ref info).Should().Be(BOOL.TRUE);
        return info.rcMonitor;
    }

    public static void VerifyBitmapNotBlank(Bitmap bmp)
    {
        Color emptyColor = Color.FromArgb(0);
        for (int y = 0; y < bmp.Height; y++)
        {
            for (int x = 0; x < bmp.Width; x++)
            {
                Color pixel = bmp.GetPixel(x, y);
                if (!pixel.Equals(emptyColor))
                {
                    return;
                }
            }
        }

        throw new XunitException("The entire image was blank.");
    }
}
