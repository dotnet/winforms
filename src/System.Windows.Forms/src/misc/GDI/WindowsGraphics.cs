// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// THIS PARTIAL CLASS CONTAINS THE BASE METHODS FOR CREATING AND DISPOSING A WINDOWSGRAPHICS AS WELL
// GETTING, DISPOSING AND WORKING WITH A DC.

using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using static Interop;

namespace System.Windows.Forms.Internal
{
    /// <summary>
    ///  WindowsGraphics is a library for rendering text and drawing using GDI; it was
    ///  created to address performance and compatibility issues found in GDI+ Graphics
    ///  class.
    ///
    ///  Note: WindowsGraphics is a stateful component, DC properties are persisted from
    ///  method calls, as opposed to Graphics (GDI+) which performs attomic operations and
    ///  always restores the hdc.
    ///
    ///  The underlying hdc is always saved and restored on dispose so external HDCs won't
    ///  be modified by WindowsGraphics.  So we don't need to restore previous objects into
    ///  the dc in method calls.
    /// </summary>
    internal sealed partial class WindowsGraphics : IDisposable, IDeviceContext, IHandle
    {
        private bool _disposeDc;
        private Graphics? _graphics; // cached when initialized FromGraphics to be able to call g.ReleaseHdc from Dispose.

        public WindowsGraphics(DeviceContext dc)
        {
            Debug.Assert(dc != null, "null dc!");
            DeviceContext = dc;
            DeviceContext.SaveHdc();
        }

        /// <summary>
        ///  Creates a WindowsGraphics from a memory DeviceContext object compatible with the primary screen device.
        ///  This object is suitable for performing text measuring but not for drawing into it because it does
        ///  not have a backup bitmap.
        /// </summary>
        public static WindowsGraphics CreateMeasurementWindowsGraphics()
        {
            DeviceContext dc = DeviceContext.FromCompatibleDC(default);
            var wg = new WindowsGraphics(dc)
            {
                _disposeDc = true // we create it, we dispose it.
            };

            // This instance is stored in a thread static so it will *always* get put on the finalizer queue.
            // As we can't do anything in the finalizer, suppress finalization for this instance (otherwise
            // our Dispose validation will assert and take down debug test runs).
            wg.SuppressFinalize();
            return wg;
        }

        public static WindowsGraphics FromHwnd(IntPtr hWnd)
        {
            DeviceContext dc = DeviceContext.FromHwnd(hWnd);
            return new WindowsGraphics(dc)
            {
                _disposeDc = true // we create it, we dispose it.
            };
        }

        public static WindowsGraphics FromHdc(Gdi32.HDC hDc)
        {
            DeviceContext dc = DeviceContext.FromHdc(hDc);
            return new WindowsGraphics(dc)
            {
                _disposeDc = true // we create it, we dispose it.
            };
        }

        /// <summary>
        ///  Creates a WindowsGraphics object from a Graphics object.  Clipping and coordinate transforms
        ///  are preserved.
        ///
        ///  Notes:
        ///  - The passed Graphics object cannot be used until the WindowsGraphics is disposed
        ///  since it borrows the hdc from the Graphics object locking it.
        ///  - Changes to the hdc using the WindowsGraphics object are not preserved into the Graphics object;
        ///  the hdc is returned to the Graphics object intact.
        ///
        ///  Some background about how Graphics uses the internal hdc when created from an existing one
        ///  (mail from GillesK from GDI+ team):
        ///  User has an HDC with a particular state:
        ///  Graphics object gets created based on that HDC. We query the HDC for its state and apply it to the Graphics.
        ///  At this stage, we do a SaveHDC and clear everything out of it.
        ///  User calls GetHdc. We restore the HDC to the state it was in and give it to the user.
        ///  User calls ReleaseHdc, we save the current state of the HDC and clear everything
        ///  (so that the graphics state gets applied next time we use it).
        ///  Next time the user calls GetHdc we give him back the state after the second ReleaseHdc.
        ///  (But the state changes between the GetHdc and ReleaseHdc are not applied to the Graphics).
        ///  Please note that this only applies the HDC created graphics, for Bitmap derived graphics, GetHdc creates a new DIBSection and
        ///  things get a lot more complicated.
        /// </summary>
        public static WindowsGraphics FromGraphics(Graphics g) => FromGraphics(g, ApplyGraphicsProperties.All);

        public static WindowsGraphics FromGraphics(Graphics graphics, ApplyGraphicsProperties properties)
        {
            bool applyTransform = properties.HasFlag(ApplyGraphicsProperties.TranslateTransform);
            bool applyClipping = properties.HasFlag(ApplyGraphicsProperties.Clipping);
            object[]? data = applyTransform || applyClipping ? (object[])graphics.GetContextInfo() : null;

            using Region? clipRegion = (Region?)data?[0];
            using Matrix? worldTransform = (Matrix?)data?[1];

            float[]? elements = applyTransform ? worldTransform?.Elements : null;

            // Apply transform and clip

            applyClipping = applyClipping && !clipRegion!.IsInfinite(graphics);
            using var graphicsRegion = applyClipping ? new Gdi32.RegionScope(clipRegion!, graphics) : default;

            // GetHdc() locks the Graphics object, it cannot be used until ReleaseHdc() is called
            Gdi32.HDC hdc = (Gdi32.HDC)graphics.GetHdc();
            WindowsGraphics wg = FromHdc(hdc);
            wg._graphics = graphics;

            try
            {
                if (applyClipping)
                {
                    // If the Graphics object was created from a native DC the actual clipping region is the intersection
                    // beteween the original DC clip region and the GDI+ one - for display Graphics it is the same as
                    // Graphics.VisibleClipBounds.

                    using var dcRegion = new Gdi32.RegionScope(hdc);
                    Gdi32.CombineRgn(graphicsRegion, dcRegion, graphicsRegion, Gdi32.CombineMode.RGN_AND);
                    Gdi32.SelectClipRgn(hdc, graphicsRegion);
                }

                if (elements != null)
                {
                    // elements (XFORM) = [eM11, eM12, eM21, eM22, eDx, eDy], eDx/eDy specify the translation offset.
                    wg.DeviceContext.TranslateTransform((int)elements[4], (int)elements[5]);
                }
            }
            catch
            {
                // We want a determinstic dispose when we fail.
                wg.Dispose();
                throw;
            }

            return wg;
        }

#if DEBUG
        // We only need the finalizer to track missed Dispose() calls.

        private readonly string _callStack = new StackTrace().ToString();

        ~WindowsGraphics()
        {
            Debug.Fail($"{nameof(WindowsGraphics)} was not disposed properly. Originating stack:\n{_callStack}");

            // We can't do anything with the fields when we're on the finalizer as they're all classes. If any of
            // them become structs they'll be a part of this instance and possible to clean up. Ideally we fix
            // the leaks and never come in on the finalizer.
            return;
        }
#endif

        [Conditional("DEBUG")]
        private void SuppressFinalize() => GC.SuppressFinalize(this);

        public DeviceContext DeviceContext { get; private set; }

        public IntPtr Handle => ((IHandle)DeviceContext).Handle;

        public void Dispose()
        {
            // Disposing can throw. As such we'll suppress preemptively so we don't roll right back in on the finalizer.
            SuppressFinalize();

            if (DeviceContext == null)
            {
                return;
            }

            try
            {
                // Restore original dc.
                DeviceContext.RestoreHdc();

                if (_disposeDc)
                {
                    DeviceContext.Dispose();
                }

                // GDI+ can fail here.
                _graphics?.ReleaseHdcInternal((IntPtr)DeviceContext.Hdc);
            }
            catch (Exception ex)
            {
                if (ClientUtils.IsCriticalException(ex))
                {
                    throw; // rethrow the original exception.
                }

                Debug.Fail($"Exception thrown during disposing: \n{ex}");
            }

            // Clear our fields that have finalizers
            DeviceContext = null!;
            _graphics = null;
        }

        IntPtr IDeviceContext.GetHdc() => (IntPtr)DeviceContext.Hdc;

        public Gdi32.HDC GetHdc() => DeviceContext.Hdc;

        public void ReleaseHdc() => DeviceContext.Dispose();
    }
}
