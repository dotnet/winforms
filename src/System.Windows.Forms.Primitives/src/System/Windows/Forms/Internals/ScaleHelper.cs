// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms.Primitives.Resources;
using Microsoft.Win32;

namespace System.Windows.Forms;

/// <summary>
///  Helper class for scaling.
/// </summary>
internal static partial class ScaleHelper
{
    /// <summary>
    ///  Pixels per inch at 100% scaling.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Some historical discussion of this can be found
    ///   <see href="https://en.wikipedia.org/wiki/Dots_per_inch#Computer_monitor_DPI_standards">
    ///    here.
    ///   </see>
    ///  </para>
    /// </remarks>
    internal const int OneHundredPercentLogicalDpi = 96;

    // Backing field, indicating that we will need to send a PerMonitorV2 query in due course.
    private static bool s_processPerMonitorAware;
    private static Size? s_logicalSmallSystemIconSize;

    /// <summary>
    ///  The initial primary monitor DPI (logical pixels per inch) for the process.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This value may change when <see cref="SetProcessHighDpiMode(HighDpiMode)"/> is called.
    ///   Application.SetHighDpiMode makes this call. This is intended to be an initial setup step and will not
    ///   change after the application has created the first window. As such you can treat this as a "constant".
    ///  </para>
    ///  <para>
    ///   The System DPI can, of course, change if the user changes the primary monitor's DPI.
    ///  </para>
    ///  <para>
    ///   If the startup thread is unaware this will always be 96 (100%).
    ///  </para>
    /// </remarks>
    internal static int InitialSystemDpi { get; private set; }

    static ScaleHelper() => InitializeStatics();

    private static void InitializeStatics()
    {
        s_processPerMonitorAware = GetPerMonitorAware();
        InitialSystemDpi = GetSystemDpi();

        static int GetSystemDpi()
        {
            // This will only change when the first call to set the process DPI awareness is made. Multiple calls to
            // set the DPI have no effect after making the first call. Depending on what the DPI awareness settings are
            // we'll get either the actual DPI of the primary display at process startup or the default LogicalDpi;

            if (!OsVersion.IsWindows10_1607OrGreater())
            {
                using var dc = GetDcScope.ScreenDC;
                return PInvokeCore.GetDeviceCaps(dc, GET_DEVICE_CAPS_INDEX.LOGPIXELSX);
            }

            // This avoids needing to create a DC
            return (int)PInvoke.GetDpiForSystem();
        }

        static bool GetPerMonitorAware()
        {
            if (!OsVersion.IsWindows10_1607OrGreater())
            {
                return false;
            }

            HRESULT result = PInvoke.GetProcessDpiAwareness(
                HANDLE.Null,
                out PROCESS_DPI_AWARENESS processDpiAwareness);

            Debug.Assert(result.Succeeded, $"Failed to get ProcessDpi HRESULT: {result}");
            Debug.Assert(Enum.IsDefined(processDpiAwareness));

            return result.Succeeded && processDpiAwareness switch
            {
                PROCESS_DPI_AWARENESS.PROCESS_DPI_UNAWARE => false,
                PROCESS_DPI_AWARENESS.PROCESS_SYSTEM_DPI_AWARE => false,
                PROCESS_DPI_AWARENESS.PROCESS_PER_MONITOR_DPI_AWARE => true,
                _ => true
            };
        }
    }

    /// <summary>
    ///  Returns a boolean to specify if we should enable processing of WM_DPICHANGED and related messages
    /// </summary>
    internal static bool IsThreadPerMonitorV2Aware
    {
        get
        {
            if (s_processPerMonitorAware)
            {
                // We can't cache this value because different top level windows can have different DPI awareness context
                // for mixed mode applications.
                DPI_AWARENESS_CONTEXT dpiAwareness = PInvoke.GetThreadDpiAwarenessContextInternal();
                return dpiAwareness.IsEquivalent(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2);
            }
            else
            {
                return false;
            }
        }
    }

    /// <summary>
    ///  Indicates, if rescaling becomes necessary, either because we are not 96 DPI or we're PerMonitorV2Aware.
    /// </summary>
    internal static bool IsScalingRequirementMet => IsScalingRequired || s_processPerMonitorAware;

    /// <summary>
    ///  Copies the given <see cref="Bitmap"/>, scaling if needed.
    /// </summary>
    /// <inheritdoc cref="ScaleToSize(Bitmap, Size, bool, bool)"/>
    internal static Bitmap CopyAndScaleToSize(Bitmap bitmap, Size desiredSize)
        => ScaleToSize(bitmap, desiredSize, disposeBitmap: false, alwaysCopy: true);

    /// <summary>
    ///  Scales the given <see cref="Bitmap"/> to the desired size if needed.
    /// </summary>
    /// <param name="disposeBitmap">
    ///  If <see langword="true"/>, the original bitmap will be disposed if a new bitmap is created.
    /// </param>
    /// <param name="alwaysCopy">
    ///  If <see langword="true"/>, the original will be copied even if it doesn't need scaled.
    /// </param>
    private static Bitmap ScaleToSize(Bitmap bitmap, Size desiredSize, bool disposeBitmap = false, bool alwaysCopy = false)
    {
        Size originalSize = bitmap.Size;
        if (originalSize == desiredSize)
        {
            if (alwaysCopy)
            {
                Bitmap copy = new(bitmap);
                if (disposeBitmap)
                {
                    bitmap.Dispose();
                }

                bitmap = copy;
            }

            return bitmap;
        }

        // In general this is the best quality interpolation mode we have available. While it introduces fuzziness in
        // the resulting image, it will not distort it as NearestNeighbor would (which is extremely important for
        // small zoom factors like 125%, 150%).
        InterpolationMode interpolationMode = InterpolationMode.HighQualityBicubic;

        if (desiredSize.Width % originalSize.Width == 0 && desiredSize.Height % originalSize.Height == 0)
        {
            // We will prefer NearestNeighbor algorithm for 200, 300, 400, etc zoom factors, in which each pixel
            // become a 2x2, 3x3, 4x4, etc rectangle. This produces sharp edges in the scaled image and doesn't
            // cause distortions of the original image.
            interpolationMode = InterpolationMode.NearestNeighbor;
        }
        else if (desiredSize.Width < originalSize.Width && desiredSize.Height < originalSize.Height)
        {
            // Shrinking the graphic, use Bilinear. Produces better results as it uses less neighboring pixels.
            interpolationMode = InterpolationMode.HighQualityBilinear;
        }

        Bitmap scaledBitmap = new(desiredSize.Width, desiredSize.Height, bitmap.PixelFormat);

        using (Bitmap? dispose = disposeBitmap ? bitmap : null)
        using (Graphics graphics = Graphics.FromImage(scaledBitmap))
        {
            graphics.InterpolationMode = interpolationMode;

            RectangleF sourceBounds = new(0, 0, bitmap.Size.Width, bitmap.Size.Height);
            RectangleF destinationBounds = new(0, 0, desiredSize.Width, desiredSize.Height);

            // Specify a source rectangle shifted by half of pixel to account for GDI+ considering the source origin the
            // center of top-left pixel.
            //
            // Failing to do so will result in the right and bottom of the bitmap lines being interpolated with the
            // graphics' background color, and will appear black even if we cleared the background with transparent color.
            // The apparition of these artifacts depends on the interpolation mode, on the dpi scaling factor, etc.
            // (e.g. at 150% DPI, Bicubic produces them and NearestNeighbor is fine, but at 200% DPI NearestNeighbor
            // also shows them).
            sourceBounds.Offset(-0.5f, -0.5f);

            graphics.DrawImage(bitmap, destinationBounds, sourceBounds, GraphicsUnit.Pixel);
        }

        return scaledBitmap;
    }

    /// <summary>
    ///  Scales a logical (100%) <see cref="Bitmap"/> value to the specified DPI.
    /// </summary>
    /// <param name="logicalBitmap"><see cref="Bitmap"/> in logical units (pixels at 100%).</param>
    /// <returns>Scaled <see cref="Bitmap"/>.</returns>
    internal static Bitmap ScaleToDpi(Bitmap logicalBitmap, int dpi, bool disposeBitmap = false) =>
        dpi == OneHundredPercentLogicalDpi
            ? logicalBitmap
            : ScaleToSize(logicalBitmap, ScaleToDpi(logicalBitmap.Size, dpi), disposeBitmap);

    /// <summary>
    ///  Returns whether scaling is required when converting between logical-device units,
    ///  if the application opted in the automatic scaling in the .config file.
    /// </summary>
    internal static bool IsScalingRequired => InitialSystemDpi != OneHundredPercentLogicalDpi;

    /// <summary>
    ///  Creates a scaled version of the given non system <see cref="Font"/> to the Windows Accessibility Text Size setting (also
    ///  known as Text Scaling) if needed, otherwise returns <see langword="null"/>.
    /// </summary>
    internal static Font? ScaleToSystemTextSize(Font? font)
    {
        if (font is null || font.IsSystemFont || !OsVersion.IsWindows10_1507OrGreater())
        {
            return null;
        }

        // The default(100) and max(225) text scale factor is value what Settings display text scale
        // applies and also clamps the text scale factor value between 100 and 225 value.
        // See https://docs.microsoft.com/windows/uwp/design/input/text-scaling.
        const int MinTextScaleValue = 100;
        const int MaxTextScaleValue = 225;

        try
        {
            // Retrieve the text scale factor, which is set via Settings > Display > Make Text Bigger.
            using RegistryKey? key = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Accessibility");
            if (key is not null && key.GetValue("TextScaleFactor") is int textScale)
            {
                textScale = Math.Clamp(textScale, MinTextScaleValue, MaxTextScaleValue);
                return textScale == 100 ? null : font.WithSize(font.Size * (textScale / 100.0f));
            }
        }
        catch
        {
            // Failed to read the registry for whatever reason.
#if DEBUG
            throw;
#endif
        }

        return null;
    }

    /// <summary>
    ///  Scale the given value the specified percent. Never returns less than 1.
    /// </summary>
    /// <param name="percent">Percentage value, with 1.0 equaling 100%.</param>
    internal static int ScaleToPercent(int value, double percent) => Math.Max(1, (int)Math.Round(value * percent));

    /// <summary>
    ///  Scales a logical (100%) pixel value to the specified DPI.
    /// </summary>
    /// <param name="logicalValue">Value in logical units (pixels at 100%).</param>
    internal static int ScaleToDpi(int logicalValue, int dpi)
    {
        Debug.Assert(dpi >= 96);
        if (dpi == OneHundredPercentLogicalDpi)
        {
            return logicalValue;
        }

        double scalingFactor = dpi / (double)OneHundredPercentLogicalDpi;
        return (int)Math.Round(scalingFactor * logicalValue);
    }

    /// <summary>
    ///  Scales a logical (100%) <see cref="Padding"/> value to the specified DPI.
    /// </summary>
    /// <param name="logicalPadding"><see cref="Padding"/> in logical units (pixels at 100%).</param>
    internal static Padding ScaleToDpi(Padding logicalPadding, int dpi) => dpi == OneHundredPercentLogicalDpi
        ? logicalPadding
        : new(
            ScaleToDpi(logicalPadding.Left, dpi),
            ScaleToDpi(logicalPadding.Top, dpi),
            ScaleToDpi(logicalPadding.Right, dpi),
            ScaleToDpi(logicalPadding.Bottom, dpi));

    /// <summary>
    ///  Scales a logical (100%) pixel value to the initial system DPI.
    /// </summary>
    /// <param name="logicalValue">Value in logical units (pixels at 100%).</param>
    internal static int ScaleToInitialSystemDpi(int logicalValue) => ScaleToDpi(logicalValue, InitialSystemDpi);

    /// <summary>
    ///  Scales a logical (100%) <see cref="Size"/> value to the specified DPI.
    /// </summary>
    /// <param name="logicalSize"><see cref="Size"/> in logical units (pixels at 100%).</param>
    internal static Size ScaleToDpi(Size logicalSize, int dpi) => dpi == OneHundredPercentLogicalDpi
        ? logicalSize
        : new(ScaleToDpi(logicalSize.Width, dpi), ScaleToDpi(logicalSize.Height, dpi));

    internal static Size SystemIconSize => new(
        PInvokeCore.GetSystemMetrics(SYSTEM_METRICS_INDEX.SM_CXICON),
        PInvokeCore.GetSystemMetrics(SYSTEM_METRICS_INDEX.SM_CYICON));

    internal static Size LogicalSmallSystemIconSize => s_logicalSmallSystemIconSize ??= OsVersion.IsWindows10_1607OrGreater()
        ? new(
            PInvoke.GetSystemMetricsForDpi(SYSTEM_METRICS_INDEX.SM_CXSMICON, OneHundredPercentLogicalDpi),
            PInvoke.GetSystemMetricsForDpi(SYSTEM_METRICS_INDEX.SM_CXSMICON, OneHundredPercentLogicalDpi))
        : new(16, 16);

    /// <summary>
    ///  Gets the given icon resource as a <see cref="Bitmap"/> at the default icon size.
    /// </summary>
    internal static Bitmap GetIconResourceAsDefaultSizeBitmap(Type type, string resource) =>
        GetIconResourceAsBestMatchBitmap(type, resource, Size.Empty);

    /// <summary>
    ///  Gets the given small icon (usually 16x16) resource as a <see cref="Bitmap"/> scaled to the specified dpi.
    /// </summary>
    internal static Bitmap GetSmallIconResourceAsBitmap(Type type, string resource, int dpi) =>
        GetIconResourceAsBitmap(type, resource, ScaleToDpi(LogicalSmallSystemIconSize, dpi));

    /// <summary>
    ///  Gets the given icon resource as a <see cref="Bitmap"/> of the given size.
    /// </summary>
    internal static Bitmap GetIconResourceAsBitmap(Type type, string resource, Size size)
    {
        if (size.IsEmpty)
        {
            size = SystemIconSize;
        }

        return ScaleToSize(
            GetIconResourceAsBestMatchBitmap(type, resource, size),
            size,
            disposeBitmap: true);
    }

    /// <summary>
    ///  Gets the given icon resource that is closest to the given size as a <see cref="Bitmap"/>.
    /// </summary>
    internal static Bitmap GetIconResourceAsBestMatchBitmap(Stream resourceStream, Size size)
    {
        // While more efficient than what we were doing, this could be even more so if we grabbed the bitmap data
        // directly out of the data stream.
        using Icon icon = new(resourceStream, size.IsEmpty ? SystemIconSize : size);
        return icon.ToBitmap();
    }

    /// <summary>
    ///  Gets the given icon resource that is closest to the given size as a <see cref="Bitmap"/>.
    /// </summary>
    internal static Bitmap GetIconResourceAsBestMatchBitmap(Type type, string resource, Size size)
    {
        using Stream stream = type.Module.Assembly.GetManifestResourceStream(type, resource)
            ?? throw new ArgumentException(string.Format(SR.ResourceNotFound, type, resource));

        return GetIconResourceAsBestMatchBitmap(stream, size);
    }

    /// <summary>
    ///  Gets the DPI mode for the current thread.
    /// </summary>
    internal static HighDpiMode GetThreadHighDpiMode()
    {
        // For Windows 10 RS2 and above
        if (OsVersion.IsWindows10_1607OrGreater())
        {
            DPI_AWARENESS_CONTEXT dpiAwareness = PInvoke.GetThreadDpiAwarenessContextInternal();

            if (dpiAwareness.IsEquivalent(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_SYSTEM_AWARE))
            {
                return HighDpiMode.SystemAware;
            }

            if (dpiAwareness.IsEquivalent(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_UNAWARE))
            {
                return HighDpiMode.DpiUnaware;
            }

            if (dpiAwareness.IsEquivalent(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2))
            {
                return HighDpiMode.PerMonitorV2;
            }

            if (dpiAwareness.IsEquivalent(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE))
            {
                return HighDpiMode.PerMonitor;
            }

            if (dpiAwareness.IsEquivalent(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_UNAWARE_GDISCALED))
            {
                return HighDpiMode.DpiUnawareGdiScaled;
            }
        }
        else if (OsVersion.IsWindows8_1OrGreater())
        {
            PInvoke.GetProcessDpiAwareness(HANDLE.Null, out PROCESS_DPI_AWARENESS processDpiAwareness);
            switch (processDpiAwareness)
            {
                case PROCESS_DPI_AWARENESS.PROCESS_DPI_UNAWARE:
                    return HighDpiMode.DpiUnaware;
                case PROCESS_DPI_AWARENESS.PROCESS_SYSTEM_DPI_AWARE:
                    return HighDpiMode.SystemAware;
                case PROCESS_DPI_AWARENESS.PROCESS_PER_MONITOR_DPI_AWARE:
                    return HighDpiMode.PerMonitor;
            }
        }
        else
        {
            // Available on Vista and higher.
            return PInvoke.IsProcessDPIAware() ? HighDpiMode.SystemAware : HighDpiMode.DpiUnaware;
        }

        // We should never get here.
        Debug.Fail("Unexpected DPI state.");
        return HighDpiMode.DpiUnaware;
    }

    /// <summary>
    ///  Sets the requested DPI mode. If the current OS does not support the requested mode,
    /// </summary>
    /// <returns><see langword="true"/> if the mode was successfully set.</returns>
    internal static bool SetProcessHighDpiMode(HighDpiMode highDpiMode)
    {
        bool success = false;

        if (OsVersion.IsWindows10_1703OrGreater())
        {
            DPI_AWARENESS_CONTEXT dpiAwareness = highDpiMode switch
            {
                HighDpiMode.SystemAware => DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_SYSTEM_AWARE,
                HighDpiMode.PerMonitor => DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE,
                HighDpiMode.PerMonitorV2 =>
                    // Necessary for RS1, since this SetProcessIntPtr IS available here.
                    PInvoke.IsValidDpiAwarenessContext(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2)
                        ? DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2
                        : DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_SYSTEM_AWARE,
                HighDpiMode.DpiUnawareGdiScaled =>
                    // Make sure we do not try to set a value which has been introduced in later Windows releases.
                    PInvoke.IsValidDpiAwarenessContext(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_UNAWARE_GDISCALED)
                        ? DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_UNAWARE_GDISCALED
                        : DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_UNAWARE,
                _ => DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_UNAWARE,
            };

            success = PInvoke.SetProcessDpiAwarenessContext(dpiAwareness);
        }
        else if (OsVersion.IsWindows8_1OrGreater())
        {
            PROCESS_DPI_AWARENESS dpiAwareness = highDpiMode switch
            {
                HighDpiMode.DpiUnaware or HighDpiMode.DpiUnawareGdiScaled => PROCESS_DPI_AWARENESS.PROCESS_DPI_UNAWARE,
                HighDpiMode.SystemAware => PROCESS_DPI_AWARENESS.PROCESS_SYSTEM_DPI_AWARE,
                HighDpiMode.PerMonitor or HighDpiMode.PerMonitorV2 => PROCESS_DPI_AWARENESS.PROCESS_PER_MONITOR_DPI_AWARE,
                _ => PROCESS_DPI_AWARENESS.PROCESS_SYSTEM_DPI_AWARE,
            };

            success = PInvoke.SetProcessDpiAwareness(dpiAwareness).Succeeded;
        }
        else
        {
            // Vista or higher has SetProcessDPIAware
            switch (highDpiMode)
            {
                case HighDpiMode.DpiUnaware:
                case HighDpiMode.DpiUnawareGdiScaled:
                    // We can return, there is nothing to set if we assume we're already in DpiUnaware.
                    return true;
                case HighDpiMode.SystemAware:
                case HighDpiMode.PerMonitor:
                case HighDpiMode.PerMonitorV2:
                    success = PInvoke.SetProcessDPIAware();
                    break;
            }
        }

        // Need to reset as our DPI might change if this was the first call to set the DPI context for the process.
        InitializeStatics();

        return success;
    }

    /// <summary>
    ///  Enters a scope during which the current thread's DPI awareness context is set to
    ///  <paramref name="awareness"/>
    /// </summary>
    /// <param name="awareness">The new DPI awareness for the current thread</param>
    /// <returns>
    ///  An object that, when disposed, will reset the current thread's DPI awareness to the value it had when the
    ///  object was created.
    /// </returns>
    public static IDisposable EnterDpiAwarenessScope(
        DPI_AWARENESS_CONTEXT awareness,
        DPI_HOSTING_BEHAVIOR dpiHosting = DPI_HOSTING_BEHAVIOR.DPI_HOSTING_BEHAVIOR_MIXED)
        => new DpiAwarenessScope(awareness, dpiHosting);

    /// <summary>
    ///  Invokes the given action in the System Aware DPI context.
    /// </summary>
    public static T InvokeInSystemAwareContext<T>(Func<T> func)
    {
        using (EnterDpiAwarenessScope(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_SYSTEM_AWARE))
        {
            return func();
        }
    }
}
