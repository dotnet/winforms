// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Helper to scope getting a <see cref="Gdi32.HDC"/> from a <see cref="IDeviceContext"/> object. Releases
    ///  the <see cref="Gdi32.HDC"/> when disposed, unlocking the parent <see cref="IDeviceContext"/> object.
    ///
    ///  Also saves and restores the state of the HDC.
    /// </summary>
    /// <remarks>
    ///  Use in a <see langword="using" /> statement. If you must pass this around, always pass by+
    ///  <see langword="ref" /> to avoid duplicating the handle and risking a double release.
    /// </remarks>
    internal readonly ref struct DeviceContextHdcScope
    {
        public IDeviceContext DeviceContext { get; }
        public Gdi32.HDC HDC { get; }

        private readonly int _savedHdcState;

        /// <summary>
        ///  Gets the <see cref="Gdi32.HDC"/> from the the given <paramref name="deviceContext"/>.
        /// </summary>
        /// <remarks>
        ///  When a <see cref="Graphics"/> object is created from a <see cref="Gdi32.HDC"/> the clipping region and
        ///  the viewport origin are applied (<see cref="Gdi32.GetViewportExtEx(Gdi32.HDC, out Size)"/>). The clipping
        ///  region isn't reflected in <see cref="Graphics.Clip"/>, which is combined with the HDC HRegion.
        ///
        ///  The Graphics object saves and restores DC state when performing operations that would modify the DC to
        ///  maintain the DC in it's original or returned state after <see cref="Graphics.ReleaseHdc()"/>.
        /// </remarks>
        /// <param name="applyGraphicsState">
        ///  Applies the origin transform and clipping region of the <paramref name="deviceContext"/> if it is an
        ///  object of type <see cref="Graphics"/>. Otherwise this is a no-op.
        /// </param>
        /// <param name="saveHdcState">
        ///  When true, saves and restores the <see cref="Gdi32.HDC"/> state.
        /// </param>
        public DeviceContextHdcScope(
            IDeviceContext deviceContext,
            bool applyGraphicsState = true,
            bool saveHdcState = false)
        {
            DeviceContext = deviceContext ?? throw new ArgumentNullException(nameof(deviceContext));
            ApplyGraphicsProperties apply = applyGraphicsState ? ApplyGraphicsProperties.All : ApplyGraphicsProperties.None;
            _savedHdcState = 0;

            if (!(DeviceContext is Graphics graphics) || apply == ApplyGraphicsProperties.None)
            {
                // GetHdc() locks the Graphics object, it cannot be used until ReleaseHdc() is called
                HDC = (Gdi32.HDC)DeviceContext.GetHdc();
                _savedHdcState = saveHdcState ? Gdi32.SaveDC(HDC) : 0;
                return;
            }

            bool applyTransform = apply.HasFlag(ApplyGraphicsProperties.TranslateTransform);
            bool applyClipping = apply.HasFlag(ApplyGraphicsProperties.Clipping);

            // This API is very expensive
            object[]? data = applyTransform || applyClipping ? (object[])graphics.GetContextInfo() : null;

            using Region? clipRegion = (Region?)data?[0];
            using Matrix? worldTransform = (Matrix?)data?[1];

            // elements (XFORM) = [eM11, eM12, eM21, eM22, eDx, eDy], eDx/eDy specify the translation offset.
            float[]? elements = applyTransform ? worldTransform?.Elements : null;
            int dx = elements != null ? (int)elements[4] : 0;
            int dy = elements != null ? (int)elements[5] : 0;
            applyTransform = applyTransform && elements != null && (dx != 0 || dy != 0);

            using var graphicsRegion = applyClipping ? new Gdi32.RegionScope(clipRegion!, graphics) : default;
            applyClipping = applyClipping && !graphicsRegion.IsNull;

            HDC = (Gdi32.HDC)graphics.GetHdc();

            if (saveHdcState || applyClipping || applyTransform)
            {
                _savedHdcState = Gdi32.SaveDC(HDC);
            }

            if (applyClipping)
            {
                // If the Graphics object was created from a native DC the actual clipping region is the intersection
                // beteween the original DC clip region and the GDI+ one - for display Graphics it is the same as
                // Graphics.VisibleClipBounds.

                RegionType type;

                using var dcRegion = new Gdi32.RegionScope(HDC);
                if (!dcRegion.IsNull)
                {
                    type = Gdi32.CombineRgn(graphicsRegion, dcRegion, graphicsRegion, Gdi32.CombineMode.RGN_AND);
                    if (type == RegionType.ERROR)
                    {
                        throw new Win32Exception();
                    }
                }

                type = Gdi32.SelectClipRgn(HDC, graphicsRegion);
                if (type == RegionType.ERROR)
                {
                    throw new Win32Exception();
                }
            }

            if (applyTransform)
            {
                Point origin = default;
                Gdi32.OffsetViewportOrgEx(HDC, dx, dy, ref origin);
            }
        }

        public static implicit operator Gdi32.HDC(in DeviceContextHdcScope scope) => scope.HDC;
        public static explicit operator IntPtr(in DeviceContextHdcScope scope) => scope.HDC.Handle;

        public void Dispose()
        {
            if (_savedHdcState != 0)
            {
                Gdi32.RestoreDC(HDC, _savedHdcState);
            }

            // Note that Graphics keeps track of the HDC it passes back, so we don't need to pass it back in
            DeviceContext?.ReleaseHdc();
        }
    }
}
