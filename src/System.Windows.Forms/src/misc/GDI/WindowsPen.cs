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
    using System.Runtime.InteropServices;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Globalization;
    using System.Runtime.Versioning;

    /// <devdoc>
    ///     <para>
    ///         Encapsulates a GDI Pen object.
    ///     </para>
    /// </devdoc>
#if WINFORMS_PUBLIC_GRAPHICS_LIBRARY
    public
#else
    internal
#endif
    sealed partial class WindowsPen : MarshalByRefObject, ICloneable, IDisposable
    {
        //
        // Handle to the native Windows pen object.
        // 
        private IntPtr nativeHandle;

        private const int dashStyleMask = 0x0000000F;
        private const int endCapMask    = 0x00000F00;
        private const int joinMask      = 0x0000F000;

        private DeviceContext dc;
        
        //
        // Fields with default values
        //
        private WindowsBrush wndBrush;
        private WindowsPenStyle style;
        private Color color;
        private int width;  
        
        private const int cosmeticPenWidth = 1;  // Cosmetic pen width.

#if GDI_FINALIZATION_WATCH
        private string AllocationSite = DbgUtil.StackTrace;
#endif

        [ResourceExposure(ResourceScope.Process)]
        [ResourceConsumption(ResourceScope.Process)]
        public WindowsPen(DeviceContext dc) :
            this( dc, WindowsPenStyle.Default, cosmeticPenWidth, Color.Black )
        { 
        }

        [ResourceExposure(ResourceScope.Process)]
        [ResourceConsumption(ResourceScope.Process)]
        public WindowsPen(DeviceContext dc, Color color ) :
            this( dc, WindowsPenStyle.Default, cosmeticPenWidth, color )
        {
        }

        [ResourceExposure(ResourceScope.Process)]
        [ResourceConsumption(ResourceScope.Process)]
        public WindowsPen(DeviceContext dc, WindowsBrush windowsBrush ) :
            this( dc, WindowsPenStyle.Default, cosmeticPenWidth, windowsBrush )
        {
        }

        [ResourceExposure(ResourceScope.Process)]
        public WindowsPen(DeviceContext dc, WindowsPenStyle style, int width, Color color)
        {
            this.style = style;
            this.width = width;
            this.color = color;
            this.dc    = dc;

            // CreatePen() created on demand.
        }

        [ResourceExposure(ResourceScope.Process)]
        [ResourceConsumption(ResourceScope.Process)]
        public WindowsPen(DeviceContext dc, WindowsPenStyle style, int width, WindowsBrush windowsBrush )
        {
            Debug.Assert(windowsBrush != null, "null windowsBrush" );
            
            this.style    = style;
            this.wndBrush = (WindowsBrush) windowsBrush.Clone();
            this.width    = width;
            this.color    = windowsBrush.Color;
            this.dc       = dc;
            
            // CreatePen() created on demand.
        }

        [ResourceExposure(ResourceScope.Process)]
        [ResourceConsumption(ResourceScope.Process)]
        private void CreatePen()
        { 
            if (this.width > 1)    // Geometric pen.
            {
                // From MSDN: if width > 1, the style must be PS_NULL, PS_SOLID, or PS_INSIDEFRAME. 
                this.style |= WindowsPenStyle.Geometric | WindowsPenStyle.Solid;
            }

            if (this.wndBrush == null)
            {
                this.nativeHandle = IntSafeNativeMethods.CreatePen((int) this.style, this.width, ColorTranslator.ToWin32(this.color) );
            }
            else
            { 
                IntNativeMethods.LOGBRUSH lb = new IntNativeMethods.LOGBRUSH();

                lb.lbColor = ColorTranslator.ToWin32( this.wndBrush.Color );
                lb.lbStyle = IntNativeMethods.BS_SOLID;
                lb.lbHatch = 0; 
                
                // Note: We currently don't support custom styles, that's why 0 and null for last two params.
                this.nativeHandle = IntSafeNativeMethods.ExtCreatePen((int)this.style, this.width, lb, 0, null );
            }
        }

        [ResourceExposure(ResourceScope.Process)]
        [ResourceConsumption(ResourceScope.Process)]
        public object Clone()
        {
            return (this.wndBrush != null) ? 
                new WindowsPen(this.dc, this.style, this.width, (WindowsBrush) this.wndBrush.Clone()) : 
                new WindowsPen(this.dc, this.style, this.width, this.color);
        }

        ~WindowsPen()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        void Dispose(bool disposing)
        {
            if (this.nativeHandle != IntPtr.Zero && dc != null)
            {
                DbgUtil.AssertFinalization(this, disposing);

                dc.DeleteObject(this.nativeHandle, GdiObjectType.Pen);
                this.nativeHandle = IntPtr.Zero;
            }

            if (this.wndBrush != null)
            {
                this.wndBrush.Dispose();
                this.wndBrush = null;
            }

            if (disposing)
            {
                GC.SuppressFinalize(this);
            }
        }

        public IntPtr HPen
        { 
            get
            {
                if( this.nativeHandle == IntPtr.Zero )
                {
                    CreatePen();
                }
                
                return this.nativeHandle;
            }
        }

        public override string ToString()
        {
            return String.Format( CultureInfo.InvariantCulture, "{0}: Style={1}, Color={2}, Width={3}, Brush={4}", 
                this.GetType().Name, 
                this.style, 
                this.color, 
                this.width, 
                this.wndBrush != null ? this.wndBrush.ToString() : "null" );
        }
    }

}
