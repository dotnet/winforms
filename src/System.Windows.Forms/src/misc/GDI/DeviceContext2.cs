// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms.Internal
{
    /// <summary>
    ///  Represents a Win32 device context.  Provides operations for setting some of the properties
    ///  of a device context.  It's the managed wrapper for an HDC.
    ///
    ///  This class is divided into two files separating the code that needs to be compiled into
    ///  reatail builds and debugging code.
    ///
    ///  WARNING: The properties of the dc are obtained on-demand, this object is light-weight because
    ///  of that; if you need to put back the old value after changing a property you need to get it
    ///  first and cache it.
    /// </summary>
    internal sealed partial class DeviceContext : MarshalByRefObject, IDeviceContext, IDisposable, IHandle
    {
        /// <summary>
        ///  See DeviceContext.cs for information about this class.  The class has been split to be able
        ///  to compile the right set of functionalities into different assemblies.
        /// </summary>
        public WindowsFont ActiveFont { get; private set; }

        /// <summary>
        ///  DC background color.
        /// </summary>
        public Color BackgroundColor
        {
            get => ColorTranslator.FromWin32(Gdi32.GetBkColor(this));
            set => ColorTranslator.FromWin32(Gdi32.SetBkColor(this, ColorTranslator.ToWin32(value)));
        }

        /// <summary>
        ///  DC background mode.
        /// </summary>
        public Gdi32.BKMODE BackgroundMode => Gdi32.GetBkMode(this);

        /// <summary>
        ///  Sets the DC background mode and returns the old value.
        /// </summary>
        public Gdi32.BKMODE SetBackgroundMode(Gdi32.BKMODE newMode) => Gdi32.SetBkMode(this, newMode);

        /// <summary>
        ///  ROP2 currently on the DC.
        /// </summary>
        public Gdi32.R2 BinaryRasterOperation => Gdi32.GetROP2(this);

        /// <summary>
        ///  Sets the DC ROP2 and returns the old value.
        /// </summary>
        public Gdi32.R2 SetRasterOperation(Gdi32.R2 rasterOperation) => Gdi32.SetROP2(this, rasterOperation);

        /// <summary>
        ///  Get the number of pixels per logical inch along the device axes. In a system with multiple display
        ///  monitors, this value is the same for all monitors.
        /// </summary>
        public Size Dpi => new Size(DpiX, DpiY);

        /// <summary>
        ///  Get the number of pixels per logical inch along the device width. In a system with multiple display
        ///  monitors, this value is the same for all monitors.
        /// </summary>
        public int DpiX => Gdi32.GetDeviceCaps(new HandleRef(this, Hdc), Gdi32.DeviceCapability.LOGPIXELSX);

        /// <summary>
        ///  Get the number of pixels per logical inch along the device (screen) height. In a system with multiple
        ///  display monitors, this value is the same for all monitors.
        /// </summary>
        public int DpiY => Gdi32.GetDeviceCaps(new HandleRef(this, Hdc), Gdi32.DeviceCapability.LOGPIXELSY);

        /// <summary>
        ///  The font selected into the device context.
        ///  It's OK to call dispose on it, the HFONT won't be deleted since the WindowsFont did not create it,
        ///  it got it from the HDC.
        /// </summary>
        public WindowsFont Font
        {
            get
            {
#if OPTIMIZED_MEASUREMENTDC
                if (MeasurementDCInfo.IsMeasurementDC(this))
                {
                    WindowsFont font = MeasurementDCInfo.LastUsedFont;
                    if (font != null && (font.Hfont != IntPtr.Zero))
                    {
#if DEBUG
                        WindowsFont currentDCFont = WindowsFont.FromHdc(Hdc);
                        if (!font.Equals(currentDCFont))
                        {
                            // just use the face name, as ToString will call here re-entrantly.
                            string lastUsedFontInfo = (font != null) ? font.Name : "null";
                            string currentFontInfo = (currentDCFont != null) ? currentDCFont.Name : "null";
                            Debug.Fail($"Font does not match... Current: {currentFontInfo} Last known: {lastUsedFontInfo}");
                        }

#endif
                        return font;
                    }
                }
#endif
                // Returns the currently selected object in the dc.
                // Note: for common DCs, GetDC assigns default attributes to the DC each time it is retrieved,
                // the default font is System.
                return WindowsFont.FromHdc(Hdc);
            }
        }

        /// <summary>
        ///  Gets a DeviceContext object initialized to refer to the primary screen device.
        ///  Consider using WindowsGraphicsCacheManager.MeasurementGraphics instead.
        /// </summary>
        public static DeviceContext ScreenDC => FromHwnd(IntPtr.Zero);

        internal void DisposeFont(bool disposing)
        {
            if (disposing)
            {
                DeviceContexts.RemoveDeviceContext(this);
            }

            if (ActiveFont != null && ActiveFont.Hfont != IntPtr.Zero)
            {
                IntPtr hCurrentFont = Gdi32.GetCurrentObject(new HandleRef(this, _hDC), Gdi32.ObjectType.OBJ_FONT);
                if (hCurrentFont == ActiveFont.Hfont)
                {
                    // select initial font back in
                    Gdi32.SelectObject(new HandleRef(this, Hdc), _hInitialFont);
                }

                ActiveFont.Dispose(disposing);
                ActiveFont = null;
            }
        }

        /// <summary>
        ///  Selects the specified object into the dc.  If the specified object is the same as the one currently selected
        ///  in the dc, the object is not set and a null value is returned.
        /// </summary>
        public IntPtr SelectFont(WindowsFont font)
        {
            // Fonts are one of the most expensive objects to select in an hdc and in many cases we are passed a Font that is the
            // same as the one already selected in the dc so to avoid a perf hit we get the hdc font's log font and compare it
            // with the one passed in before selecting it in the hdc.
            // Also, we avoid performing GDI operations that if done on an enhanced metafile DC would add an entry to it, hence
            // reducing the size of the metafile.
            if (font.Equals(Font))
            {
                return IntPtr.Zero;
            }
            IntPtr result = SelectObject(font.Hfont, GdiObjectType.Font);

            WindowsFont previousFont = ActiveFont;
            ActiveFont = font;
            _hCurrentFont = font.Hfont;

            // the measurement DC always leaves fonts selected for pref reasons.
            // in this case, we need to diposse the font since the original
            // creator didn't fully dispose.
            if (previousFont != null)
            {
                if (MeasurementDCInfo.IsMeasurementDC(this))
                {
                    previousFont.Dispose();
                }
            }

#if OPTIMIZED_MEASUREMENTDC
            // once we've changed the font, update the last used font.
            if (MeasurementDCInfo.IsMeasurementDC(this))
            {
                if (result != IntPtr.Zero)
                {
                    MeasurementDCInfo.LastUsedFont = font;
                }
                else
                {
                    // there was an error selecting the Font into the DC, we dont know anything about it.
                    MeasurementDCInfo.Reset();
                }
            }
#endif
            return result;
        }

        public void ResetFont()
        {
#if OPTIMIZED_MEASUREMENTDC
            // in this case, GDI will copy back the previously saved font into the DC.
            // we dont actually know what the font is in our measurement DC so
            // we need to clear it off.
            MeasurementDCInfo.ResetIfIsMeasurementDC(Hdc);
#endif
            Gdi32.SelectObject(new HandleRef(this, Hdc), _hInitialFont);
            ActiveFont = null;
            _hCurrentFont = _hInitialFont;
        }

        /// <summary>
        ///  DC map mode.
        /// </summary>
        public Gdi32.MM MapMode => Gdi32.GetMapMode(this);

        public bool IsFontOnContextStack(WindowsFont wf)
        {
            if (_contextStack == null)
            {
                return false;
            }

            foreach (GraphicsState g in _contextStack)
            {
                if (g.hFont == wf.Hfont)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///  Sets the DC map mode and returns the old value.
        /// </summary>
        public Gdi32.MM SetMapMode(Gdi32.MM newMode) => Gdi32.SetMapMode(this, newMode);

        /// <summary>
        ///  Selects the specified object into the dc and returns the old object.
        /// </summary>
        public IntPtr SelectObject(IntPtr hObj, GdiObjectType type)
        {
            switch (type)
            {
                case GdiObjectType.Pen:
                    _hCurrentPen = hObj;
                    break;
                case GdiObjectType.Brush:
                    _hCurrentBrush = hObj;
                    break;
                case GdiObjectType.Bitmap:
                    _hCurrentBmp = hObj;
                    break;
            }

            return Gdi32.SelectObject(new HandleRef(this, Hdc), hObj);
        }

        /// <summary>
        ///  DC text alignment.
        /// </summary>
        public Gdi32.TA TextAlignment
        {
            get => Gdi32.GetTextAlign(this);
            set => Gdi32.SetTextAlign(this, value);
        }

        /// <summary>
        ///  DC current text color.
        /// </summary>
        public Color TextColor
        {
            get => ColorTranslator.FromWin32(Gdi32.GetTextColor(this));
            set => ColorTranslator.FromWin32(Gdi32.SetTextColor(this, ColorTranslator.ToWin32(value)));
        }

        /// <summary>
        ///  DC Viewport Extent in device units.
        /// </summary>
        public unsafe Size ViewportExtent
        {
            get
            {
                Gdi32.GetViewportExtEx(this, out Size size);
                return size;
            }
            set
            {
                Gdi32.SetViewportExtEx(this, value.Width, value.Height, null);
            }
        }

        /// <summary>
        ///  DC Viewport Origin in device units.
        /// </summary>
        public unsafe Point ViewportOrigin
        {
            get
            {
                Gdi32.GetViewportOrgEx(this, out Point point);
                return point;
            }
            set
            {
                Gdi32.SetViewportOrgEx(this, value.X, value.Y, null);
            }
        }
    }
}
