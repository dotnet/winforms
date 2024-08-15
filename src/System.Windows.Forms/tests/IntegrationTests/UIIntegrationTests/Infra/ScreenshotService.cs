// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Drawing.Imaging;

namespace System.Windows.Forms.UITests;

internal static class ScreenshotService
{
    private static readonly Lock s_lock = new();

    /// <summary>
    /// Takes a picture of the screen and saves it to the location specified by
    /// <paramref name="fullPath"/>. Files are always saved in PNG format, regardless of the
    /// file extension.
    /// </summary>
    public static void TakeScreenshot(string fullPath)
    {
        // This gate prevents concurrency for two reasons:
        //
        // 1. Only one screenshot is held in memory at a time to prevent running out of memory for large displays
        // 2. Only one screenshot is written to disk at a time to avoid exceptions if concurrent calls are writing
        //    to the same file
        lock (s_lock)
        {
            using var bitmap = TryCaptureFullScreen();
            if (bitmap is null)
            {
                return;
            }

            string directory = Path.GetDirectoryName(fullPath)!;
            Directory.CreateDirectory(directory);

            bitmap.Save(fullPath, ImageFormat.Png);
        }
    }

    /// <summary>
    /// Captures the full screen to a <see cref="Bitmap"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="Bitmap"/> containing the screen capture of the desktop, or <see langword="null"/> if a screen
    /// capture can't be created.
    /// </returns>
    internal static Bitmap? TryCaptureFullScreen()
    {
        if (Screen.PrimaryScreen is not { } primaryScreen)
            return null;

        int width = primaryScreen.Bounds.Width;
        int height = primaryScreen.Bounds.Height;

        if (width <= 0 || height <= 0)
        {
            // Don't try to take a screenshot if there is no screen.
            // This may not be an interactive session.
            return null;
        }

        Bitmap bitmap = new(width, height, PixelFormat.Format32bppArgb);

        using var graphics = Graphics.FromImage(bitmap);
        graphics.CopyFromScreen(
            sourceX: primaryScreen.Bounds.X,
            sourceY: primaryScreen.Bounds.Y,
            destinationX: 0,
            destinationY: 0,
            blockRegionSize: bitmap.Size,
            copyPixelOperation: CopyPixelOperation.SourceCopy);

        if (Cursor.Current is { } cursor)
        {
            Rectangle bounds = new(Cursor.Position - (Size)cursor.HotSpot, cursor.Size);
            cursor.Draw(graphics, bounds);
        }

        return bitmap;
    }
}
