// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
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
#if DEBUG
    internal class DeviceContextHdcScope : DisposalTracking.Tracker, IDisposable
#else
    internal readonly ref struct DeviceContextHdcScope
#endif
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
        ///  maintain the DC in its original or returned state after <see cref="Graphics.ReleaseHdc()"/>.
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
            bool saveHdcState = false) : this (
                deviceContext,
                applyGraphicsState ? ApplyGraphicsProperties.All : ApplyGraphicsProperties.None,
                saveHdcState)
        {
        }

        /// <summary>
        ///  Prefer to use <see cref="DeviceContextHdcScope(IDeviceContext, bool, bool)"/>.
        /// </summary>
        /// <remarks>
        ///  Ideally we'd not bifurcate what properties we apply unless we're absolutely sure we only want one.
        /// </remarks>
        public unsafe DeviceContextHdcScope(
            IDeviceContext deviceContext,
            ApplyGraphicsProperties applyGraphicsState,
            bool saveHdcState = false)
        {
            if (deviceContext is null)
            {
                DisposalTracking.SuppressFinalize(this!);
                throw new ArgumentNullException(nameof(deviceContext));
            }

            DeviceContext = deviceContext;
            _savedHdcState = 0;

            HDC = default;

            IGraphicsHdcProvider? provider = deviceContext as IGraphicsHdcProvider;
            Graphics? graphics = deviceContext as Graphics;

            // There are three states of IDeviceContext that come into this class:
            //
            //  1. One that also implements IGraphicsHdcProvider
            //  2. One that is directly on Graphics
            //  3. All other IDeviceContext instances
            //
            // In the third case there is no Graphics to apply Properties from. In the second case we must query
            // the Graphics object itself for Properties (transform and clip). In the first case the
            // IGraphicsHdcProvider will let us know if we have an "unclean" Graphics object that we need to apply
            // Properties from.
            //
            // PaintEventArgs implements IGraphicsHdcProvider and uses it to let us know that either (1) a Graphics
            // object hasn't been created yet, OR (2) the Graphics object has never been given a transform or clip.

            bool needToApplyProperties = applyGraphicsState != ApplyGraphicsProperties.None;
            if (graphics is null && provider is null)
            {
                // We have an IDeviceContext (case 3 above), we can't apply properties because there is no
                // Graphics object available.
                needToApplyProperties = false;
            }
            else if (provider != null && provider.IsGraphicsStateClean)
            {
                // We have IGraphicsHdcProvider and it is telling us we have no properties to apply (case 1 above)
                needToApplyProperties = false;
            }

            if (provider != null)
            {
                // We have a provider, grab the underlying HDC if possible unless we know we've created and
                // modified a Graphics object around it.
                HDC = needToApplyProperties ? default : provider.GetHDC();

                if (HDC.IsNull)
                {
                    graphics = provider.GetGraphics(createIfNeeded: true);
                    if (graphics is null)
                    {
                        throw new InvalidOperationException();
                    }
                    DeviceContext = graphics;
                }
            }

            if (!needToApplyProperties || graphics is null)
            {
                HDC = HDC.IsNull ? (Gdi32.HDC)DeviceContext.GetHdc() : HDC;
                _savedHdcState = saveHdcState ? Gdi32.SaveDC(HDC) : 0;
                return;
            }

            // We have a Graphics object (either directly passed in or given to us by IGraphicsHdcProvider)
            // that needs properties applied.

            bool applyTransform = applyGraphicsState.HasFlag(ApplyGraphicsProperties.TranslateTransform);
            bool applyClipping = applyGraphicsState.HasFlag(ApplyGraphicsProperties.Clipping);

            // This API is very expensive and cannot be called after GetHdc()
            object[]? data = applyTransform || applyClipping ? (object[])graphics.GetContextInfo() : null;

            using Region? clipRegion = (Region?)data?[0];
            using Matrix? worldTransform = (Matrix?)data?[1];

            // elements (XFORM) = [eM11, eM12, eM21, eM22, eDx, eDy], eDx/eDy specify the translation offset.
            float[]? elements = applyTransform ? worldTransform?.Elements : null;
            int dx = elements != null ? (int)elements[4] : 0;
            int dy = elements != null ? (int)elements[5] : 0;
            applyTransform = applyTransform && elements != null && (dx != 0 || dy != 0);

            using var graphicsRegion = applyClipping ? new Gdi32.RegionScope(clipRegion!, graphics) : default;
            applyClipping = applyClipping && !graphicsRegion!.Region.IsNull;

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
                    type = Gdi32.CombineRgn(graphicsRegion!, dcRegion, graphicsRegion!, Gdi32.RGN.AND);
                    if (type == RegionType.ERROR)
                    {
                        throw new Win32Exception();
                    }
                }

                type = Gdi32.SelectClipRgn(HDC, graphicsRegion!);
                if (type == RegionType.ERROR)
                {
                    throw new Win32Exception();
                }
            }

            if (applyTransform)
            {
                Gdi32.OffsetViewportOrgEx(HDC, dx, dy, null);
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
            if (!(DeviceContext is IGraphicsHdcProvider))
            {
                DeviceContext?.ReleaseHdc();
            }

            DisposalTracking.SuppressFinalize(this!);
        }
    }
}
