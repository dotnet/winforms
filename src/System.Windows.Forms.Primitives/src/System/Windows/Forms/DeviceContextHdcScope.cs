// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using static Interop;
using Gdi = Windows.Win32.Graphics.Gdi;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Helper to scope getting a <see cref="HDC"/> from a <see cref="IDeviceContext"/> object. Releases
    ///  the <see cref="HDC"/> when disposed, unlocking the parent <see cref="IDeviceContext"/> object.
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
        public HDC HDC { get; }

        private readonly int _savedHdcState;

        /// <summary>
        ///  Gets the <see cref="HDC"/> from the given <paramref name="deviceContext"/>.
        /// </summary>
        /// <remarks>
        ///  <para>
        ///   When a <see cref="Graphics"/> object is created from a <see cref="HDC"/> the clipping region and
        ///   the viewport origin are applied (<see cref="Gdi32.GetViewportExtEx(HDC, out Size)"/>). The clipping
        ///   region isn't reflected in <see cref="Graphics.Clip"/>, which is combined with the HDC HRegion.
        ///  </para>
        ///  <para>
        ///   The Graphics object saves and restores DC state when performing operations that would modify the DC to
        ///   maintain the DC in its original or returned state after <see cref="Graphics.ReleaseHdc()"/>.
        ///  </para>
        /// </remarks>
        /// <param name="applyGraphicsState">
        ///  Applies the origin transform and clipping region of the <paramref name="deviceContext"/> if it is an
        ///  object of type <see cref="Graphics"/>. Otherwise this is a no-op.
        /// </param>
        /// <param name="saveHdcState">
        ///  When true, saves and restores the <see cref="HDC"/> state.
        /// </param>
        public DeviceContextHdcScope(
            IDeviceContext deviceContext,
            bool applyGraphicsState = true,
            bool saveHdcState = false) : this(
                deviceContext,
                applyGraphicsState ? ApplyGraphicsProperties.All : ApplyGraphicsProperties.None,
                saveHdcState)
        {
        }

        /// <summary>
        ///  Prefer to use <see cref="DeviceContextHdcScope(IDeviceContext, bool, bool)"/>.
        /// </summary>
        /// <remarks>
        ///  <para>
        ///   Ideally we'd not bifurcate what properties we apply unless we're absolutely sure we only want one.
        ///  </para>
        /// </remarks>
        public unsafe DeviceContextHdcScope(
            IDeviceContext deviceContext,
            ApplyGraphicsProperties applyGraphicsState,
            bool saveHdcState = false)
        {
            if (deviceContext is null)
            {
                // As we're throwing in the constructor, `this` will never be passed back and as such .Dispose()
                // can't be called. We don't have anything to release at this point so there is no point in having
                // the finalizer run.
#if DEBUG
                DisposalTracking.SuppressFinalize(this!);
#endif
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
            else if (provider is not null && provider.IsGraphicsStateClean)
            {
                // We have IGraphicsHdcProvider and it is telling us we have no properties to apply (case 1 above)
                needToApplyProperties = false;
            }

            if (provider is not null)
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
                HDC = HDC.IsNull ? (HDC)DeviceContext.GetHdc() : HDC;
                ValidateHDC();
                _savedHdcState = saveHdcState ? Gdi32.SaveDC(HDC) : 0;
                return;
            }

            // We have a Graphics object (either directly passed in or given to us by IGraphicsHdcProvider)
            // that needs properties applied.

            bool applyTransform = applyGraphicsState.HasFlag(ApplyGraphicsProperties.TranslateTransform);
            bool applyClipping = applyGraphicsState.HasFlag(ApplyGraphicsProperties.Clipping);

            Region? clipRegion = null;
            PointF offset = default;
            if (applyClipping)
            {
                graphics.GetContextInfo(out offset, out clipRegion);
            }
            else if (applyTransform)
            {
                graphics.GetContextInfo(out offset);
            }

            using (clipRegion)
            {
                applyTransform = applyTransform && !offset.IsEmpty;
                applyClipping = clipRegion is not null;

                using var graphicsRegion = applyClipping ? new Gdi32.RegionScope(clipRegion!, graphics) : default;
                applyClipping = applyClipping && !graphicsRegion!.Region.IsNull;

                HDC = (HDC)graphics.GetHdc();

                if (saveHdcState || applyClipping || applyTransform)
                {
                    _savedHdcState = Gdi32.SaveDC(HDC);
                }

                if (applyClipping)
                {
                    // If the Graphics object was created from a native DC the actual clipping region is the intersection
                    // between the original DC clip region and the GDI+ one - for display Graphics it is the same as
                    // Graphics.VisibleClipBounds.

                    RegionType type;

                    using var dcRegion = new Gdi32.RegionScope(HDC);
                    if (!dcRegion.IsNull)
                    {
                        type = (RegionType)PInvoke.CombineRgn(graphicsRegion!, dcRegion, graphicsRegion!, Gdi.RGN_COMBINE_MODE.RGN_AND);
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
                    Gdi32.OffsetViewportOrgEx(HDC, (int)offset.X, (int)offset.Y, null);
                }
            }
        }

        public static implicit operator HDC(in DeviceContextHdcScope scope) => scope.HDC;
        public static implicit operator nint(in DeviceContextHdcScope scope) => scope.HDC;

        [Conditional("DEBUG")]
        private void ValidateHDC()
        {
            if (HDC.IsNull)
            {
                // We don't want the disposal tracking to fire as it will take down unrelated tests.
#if DEBUG
                DisposalTracking.SuppressFinalize(this!);
#endif
                throw new InvalidOperationException("Null HDC");
            }

            var type = Gdi32.GetObjectType((HGDIOBJ)HDC.Value);
            switch (type)
            {
                case Gdi32.OBJ.DC:
                case Gdi32.OBJ.MEMDC:
                case Gdi32.OBJ.METADC:
                case Gdi32.OBJ.ENHMETADC:
                    break;
                default:
#if DEBUG
                    DisposalTracking.SuppressFinalize(this!);
#endif
                    throw new InvalidOperationException($"Invalid handle ({type})");
            }
        }

        public void Dispose()
        {
            if (_savedHdcState != 0)
            {
                Gdi32.RestoreDC(HDC, _savedHdcState);
            }

            // Note that Graphics keeps track of the HDC it passes back, so we don't need to pass it back in
            if (DeviceContext is not IGraphicsHdcProvider)
            {
                DeviceContext?.ReleaseHdc();
            }

#if DEBUG
            DisposalTracking.SuppressFinalize(this!);
#endif
        }
    }
}
