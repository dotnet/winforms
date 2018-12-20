// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if WINFORMS_NAMESPACE
namespace System.Windows.Forms.Internal
#elif DRAWING_NAMESPACE
namespace System.Drawing.Internal
#else
namespace System.Experimental.Gdi
#endif
{
    using System;
    using System.Internal;
    using System.Security;
    using System.Runtime.InteropServices;
    using System.Diagnostics;
    using System.ComponentModel;
    using System.Drawing;
    using System.Globalization;
    using System.Runtime.Versioning;

    /// <include file='doc\IDeviceContext.uex' path='docs/doc[@for="DeviceContext"]/*' />
    /// <devdoc>
    ///     Represents a Win32 device context.  Provides operations for setting some of the properties
    ///     of a device context.  It's the managed wrapper for an HDC.
    ///     
    ///     This class is divided into two files separating the code that needs to be compiled into
    ///     reatail builds and debugging code.
    ///     
    ///     WARNING: The properties of the dc are obtained on-demand, this object is light-weight because
    ///     of that; if you need to put back the old value after changing a property you need to get it
    ///     first and cache it.
    /// </devdoc>
#if WINFORMS_PUBLIC_GRAPHICS_LIBRARY
    public
#else
    internal
#endif
    sealed partial class DeviceContext : MarshalByRefObject, IDeviceContext, IDisposable
    {
        WindowsFont selectedFont;
        
        /// <devdoc>
        ///     See DeviceContext.cs for information about this class.  The class has been split to be able
        ///     to compile the right set of functionalities into different assemblies.
        /// </devdoc>

        
        public WindowsFont ActiveFont {
            get {
                return selectedFont;
            }
        }

        /// <devdoc>
        ///     DC background color.
        /// </devdoc>  
        public Color BackgroundColor
        {
            get
            {
                return ColorTranslator.FromWin32(IntUnsafeNativeMethods.GetBkColor( new HandleRef( this, this.Hdc )));
            }
#if WINFORMS_PUBLIC_GRAPHICS_LIBRARY
            set
            {
                SetBackgroundColor( value );
            }
#endif
        }

        /// <devdoc>
        ///     Sets the DC background color and returns the old value.
        /// </devdoc> 
        public Color SetBackgroundColor( Color newColor )
        {
            return ColorTranslator.FromWin32(IntUnsafeNativeMethods.SetBkColor( new HandleRef( this, this.Hdc ), ColorTranslator.ToWin32(newColor)));
        }

        /// <devdoc>
        ///     DC background mode.
        /// </devdoc>  
        public DeviceContextBackgroundMode BackgroundMode 
        {
            get
            {
                return (DeviceContextBackgroundMode) IntUnsafeNativeMethods.GetBkMode( new HandleRef( this, this.Hdc ) );
            }
#if WINFORMS_PUBLIC_GRAPHICS_LIBRARY
            set
            {
                SetBackgroundMode(value);
            }
#endif
        }

        /// <devdoc>
        ///     Sets the DC background mode and returns the old value.
        /// </devdoc> 
        public DeviceContextBackgroundMode SetBackgroundMode( DeviceContextBackgroundMode newMode )
        {
            return (DeviceContextBackgroundMode) IntUnsafeNativeMethods.SetBkMode(new HandleRef(this, this.Hdc), (int) newMode);
        }


        /// <devdoc>
        ///     ROP2 currently on the DC.
        /// </devdoc>
        public DeviceContextBinaryRasterOperationFlags BinaryRasterOperation
        {
            get
            {
                return (DeviceContextBinaryRasterOperationFlags) IntUnsafeNativeMethods.GetROP2( new HandleRef( this, this.Hdc ) );
            }
            /*
             * CONSIDER: implement if needed.
#if WINFORMS_PUBLIC_GRAPHICS_LIBRARY
             * 
            set
            {
            }
#endif            
            */
        }

        /// <devdoc>
        ///     Sets the DC ROP2 and returns the old value.
        /// </devdoc> 
        public DeviceContextBinaryRasterOperationFlags SetRasterOperation(DeviceContextBinaryRasterOperationFlags rasterOperation )
        {
            return (DeviceContextBinaryRasterOperationFlags) IntUnsafeNativeMethods.SetROP2(new HandleRef(this, this.Hdc), (int) rasterOperation);
        }

        ///<devdoc>
        ///     Get the number of pixels per logical inch along the device axes.
        ///     In a system with multiple display monitors, this value is the same for all monitors.
        ///</devdoc>
        public Size Dpi
        {
            get
            {
                return new Size(GetDeviceCapabilities(DeviceCapabilities.LogicalPixelsX), GetDeviceCapabilities(DeviceCapabilities.LogicalPixelsY));
            }
        }

        ///<devdoc>
        ///     Get the number of pixels per logical inch along the device width.
        ///     In a system with multiple display monitors, this value is the same for all monitors.
        ///</devdoc>
        public int DpiX
        {
            get
            {
                return GetDeviceCapabilities(DeviceCapabilities.LogicalPixelsX);
            }
        }

        ///<devdoc>
        ///     Get the number of pixels per logical inch along the device (screen) height.
        ///     In a system with multiple display monitors, this value is the same for all monitors.
        ///</devdoc>
        public int DpiY
        {
            get
            {
                return GetDeviceCapabilities(DeviceCapabilities.LogicalPixelsY);
            }
        }
                       
        /// <devdoc>
        ///     The font selected into the device context.  
        ///     It's OK to call dispose on it, the HFONT won't be deleted since the WindowsFont did not create it,
        ///     it got it from the HDC.
        /// </devdoc>
        
        public WindowsFont Font
        {
            [ResourceExposure(ResourceScope.Process)]
            [ResourceConsumption(ResourceScope.Process)]
            get
            {
#if OPTIMIZED_MEASUREMENTDC                
                if (MeasurementDCInfo.IsMeasurementDC(this)) 
                {                    
                    WindowsFont font = MeasurementDCInfo.LastUsedFont;
                    if (font != null && (font.Hfont != IntPtr.Zero)) {
#if DEBUG   
                        WindowsFont currentDCFont = WindowsFont.FromHdc( this.Hdc );
                        if (!font.Equals(currentDCFont) ) {
                            // just use the face name, as ToString will call here re-entrantly.
                            string lastUsedFontInfo = (font != null) ?  font.Name : "null";
                            string currentFontInfo = (currentDCFont != null) ?  currentDCFont.Name : "null";
                            Debug.Fail("Font does not match... Current: " + currentFontInfo + " Last known: " + lastUsedFontInfo);
                        }
                        
#endif                  
                        return font;

                    }
                }
#endif                
                // Returns the currently selected object in the dc.
                // Note: for common DCs, GetDC assigns default attributes to the DC each time it is retrieved, 
                // the default font is System.
                return WindowsFont.FromHdc( this.Hdc );
            }
#if WINFORMS_PUBLIC_GRAPHICS_LIBRARY
            set
            {
                Debug.Assert( value != null, "value == null." );
                IntPtr hOldFont = SelectFont( value );
                IntUnsafeNativeMethods.IntDeleteObject(new HandleRef(null, hOldFont));
            }
#endif
        }

        /// <devdoc>
        ///     Gets a DeviceContext object initialized to refer to the primary screen device.
        ///     Consider using WindowsGraphicsCacheManager.MeasurementGraphics instead.
        /// </devdoc>
        public static DeviceContext ScreenDC
        {
            get
            {
                return DeviceContext.FromHwnd(IntPtr.Zero);
            }
        }

        internal void DisposeFont(bool disposing) {
            if (disposing) {
                DeviceContexts.RemoveDeviceContext(this);
            }            

            if (selectedFont != null && selectedFont.Hfont != IntPtr.Zero) {
                IntPtr hCurrentFont = IntUnsafeNativeMethods.GetCurrentObject(new HandleRef(this, hDC), IntNativeMethods.OBJ_FONT);
                if (hCurrentFont == selectedFont.Hfont) {
                    // select initial font back in
                    IntUnsafeNativeMethods.SelectObject(new HandleRef(this, this.Hdc), new HandleRef( null, hInitialFont));
                    hCurrentFont = hInitialFont;
                }

                selectedFont.Dispose(disposing);                                 
                selectedFont = null;
            }
        }

        /// <devdoc>
        ///     Selects the specified object into the dc.  If the specified object is the same as the one currently selected
        ///     in the dc, the object is not set and a null value is returned.
        /// </devdoc>                         
        public IntPtr SelectFont( WindowsFont font )
        {
           
            // Fonts are one of the most expensive objects to select in an hdc and in many cases we are passed a Font that is the
            // same as the one already selected in the dc so to avoid a perf hit we get the hdc font's log font and compare it 
            // with the one passed in before selecting it in the hdc.
            // Also, we avoid performing GDI operations that if done on an enhanced metafile DC would add an entry to it, hence 
            // reducing the size of the metafile.
            if( font.Equals( this.Font ))
            {
                return IntPtr.Zero;
            }
            IntPtr result = SelectObject( font.Hfont, GdiObjectType.Font);

            WindowsFont previousFont = selectedFont;            
            selectedFont = font;
            hCurrentFont = font.Hfont;          

            // the measurement DC always leaves fonts selected for pref reasons.
            // in this case, we need to diposse the font since the original 
            // creator didn't fully dispose.
            if (previousFont != null) {
                if (MeasurementDCInfo.IsMeasurementDC(this)) {
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
            MeasurementDCInfo.ResetIfIsMeasurementDC(this.Hdc);
#endif        
            IntUnsafeNativeMethods.SelectObject(new HandleRef(this, this.Hdc), new HandleRef( null, hInitialFont ));
            selectedFont = null;
            hCurrentFont = hInitialFont;
        }

        /// <devdoc>
        ///     Retrieves device-specific information for this device. 
        /// </devdoc> 
        public int GetDeviceCapabilities( DeviceCapabilities capabilityIndex )
        {
            return IntUnsafeNativeMethods.GetDeviceCaps( new HandleRef( this, this.Hdc ), (int) capabilityIndex );
        }

        /// <devdoc>
        ///     DC map mode.
        /// </devdoc>  
        public DeviceContextMapMode MapMode
        {
            get
            {
                return (DeviceContextMapMode) IntUnsafeNativeMethods.GetMapMode( new HandleRef( this, this.Hdc ) );
            }
#if WINFORMS_PUBLIC_GRAPHICS_LIBRARY
            set
            {
                SetMapMode(value);
            }
#endif
        }

        public bool IsFontOnContextStack(WindowsFont wf)
        {
            if (contextStack == null) {
                return false;
            }

            foreach (GraphicsState g in contextStack) {
                if (g.hFont == wf.Hfont) {
                    return true;
                }
            }

            return false;
        }

        /// <devdoc>
        ///     Sets the DC map mode and returns the old value.
        /// </devdoc>
        public DeviceContextMapMode SetMapMode( DeviceContextMapMode newMode )
        {
            return (DeviceContextMapMode) IntUnsafeNativeMethods.SetMapMode( new HandleRef(this, this.Hdc), (int) newMode );
        }

        /// <devdoc>
        ///     Selects the specified object into the dc and returns the old object.
        /// </devdoc>
        public IntPtr SelectObject(IntPtr hObj, GdiObjectType type)
        {
            switch (type) {
                case GdiObjectType.Pen:
                    hCurrentPen = hObj;
                    break;
                case GdiObjectType.Brush:
                    hCurrentBrush = hObj;
                    break;

                case GdiObjectType.Bitmap:
                    hCurrentBmp = hObj;
                    break;
            }            
            return IntUnsafeNativeMethods.SelectObject(new HandleRef(this, this.Hdc), new HandleRef( null, hObj));
        }

        /// <devdoc>
        ///     DC text alignment.
        /// </devdoc>  
        public DeviceContextTextAlignment TextAlignment
        {
            get
            {
                return (DeviceContextTextAlignment) IntUnsafeNativeMethods.GetTextAlign( new HandleRef( this, this.Hdc ) );
            }
#if WINFORMS_PUBLIC_GRAPHICS_LIBRARY
            set
            {
                SetTextAlignment(value);
            }
#endif
        }

        /// <devdoc>
        ///     Sets the DC text alignment and returns the old value.
        /// </devdoc>  
        public DeviceContextTextAlignment SetTextAlignment( DeviceContextTextAlignment newAligment )
        {
            return (DeviceContextTextAlignment) IntUnsafeNativeMethods.SetTextAlign(new HandleRef(this, this.Hdc), (int) newAligment );
        }


        /// <devdoc>
        ///     DC current text color.
        /// </devdoc>  
        public Color TextColor
        {
            get
            {
                return ColorTranslator.FromWin32(IntUnsafeNativeMethods.GetTextColor( new HandleRef( this, this.Hdc ) ));
            }
#if WINFORMS_PUBLIC_GRAPHICS_LIBRARY
            set
            {
                SetTextColor(value);
            }
#endif
        }

        /// <devdoc>
        ///     Sets the DC text color and returns the old value.
        /// </devdoc>  
        public Color SetTextColor( Color newColor )
        {
            return ColorTranslator.FromWin32(IntUnsafeNativeMethods.SetTextColor(new HandleRef( this, this.Hdc), ColorTranslator.ToWin32(newColor)));
        }

        /// <devdoc>
        ///     DC Viewport Extent in device units.
        /// </devdoc>  
        public Size ViewportExtent
        {
            get
            {
                IntNativeMethods.SIZE size = new IntNativeMethods.SIZE();
                IntUnsafeNativeMethods.GetViewportExtEx( new HandleRef( this, this.Hdc ), size );

                return size.ToSize();
            }
             set
             {
                SetViewportExtent( value );
             }
        }

        /// <devdoc>
        ///     Sets the DC Viewport extent to the specified value and returns its previous value; extent values are in device units.
        /// </devdoc> 
        public Size SetViewportExtent( Size newExtent )
        {
            IntNativeMethods.SIZE oldExtent = new IntNativeMethods.SIZE();
            
            IntUnsafeNativeMethods.SetViewportExtEx( new HandleRef( this, this.Hdc ), newExtent.Width, newExtent.Height, oldExtent );

            return oldExtent.ToSize();
        } 

        /// <devdoc>
        ///     DC Viewport Origin in device units.
        /// </devdoc>  
        public Point ViewportOrigin
        {
            get
            {
                IntNativeMethods.POINT point = new IntNativeMethods.POINT();
                IntUnsafeNativeMethods.GetViewportOrgEx( new HandleRef( this, this.Hdc ), point );

                return point.ToPoint();
            }
             set
             {
                SetViewportOrigin( value );
             }
        }

        /// <devdoc>
        ///     Sets the DC Viewport origin to the specified value and returns its previous value; origin values are in device units.
        /// </devdoc> 
        public Point SetViewportOrigin( Point newOrigin )
        {
            IntNativeMethods.POINT oldOrigin = new IntNativeMethods.POINT();
            IntUnsafeNativeMethods.SetViewportOrgEx( new HandleRef( this, this.Hdc ), newOrigin.X, newOrigin.Y, oldOrigin );

            return oldOrigin.ToPoint();
        }
        
    }
}

