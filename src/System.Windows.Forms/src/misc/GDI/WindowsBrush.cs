// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope="member", Target="System.Windows.Forms.Internal.WindowsBrush.FromLogBrush(System.Windows.Forms.Internal.IntNativeMethods+LOGBRUSH):System.Windows.Forms.Internal.WindowsBrush")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope="member", Target="System.Windows.Forms.Internal.WindowsBrush.FromHdc(System.IntPtr):System.Windows.Forms.Internal.WindowsBrush")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope="member", Target="System.Windows.Forms.Internal.WindowsBrush.FromBrush(System.Drawing.Brush):System.Windows.Forms.Internal.WindowsBrush")]

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
    using System.Runtime.Versioning;

    /// <devdoc>
    ///     <para>
    ///         Encapsulates a GDI Brush object.
    ///     </para>
    /// </devdoc>
#if WINFORMS_PUBLIC_GRAPHICS_LIBRARY
    public
#else
    internal
#endif
    abstract class WindowsBrush : MarshalByRefObject, ICloneable, IDisposable
    {
        // Handle to the native Windows brush object.
        // 
        private  DeviceContext dc;
        private IntPtr nativeHandle;        // Cannot be protected because the class is internal (C# doesn't allow it).
        private Color color = Color.White;  // GDI brushes have just one color as opposed to GDI+ that can have background color.
        // Note: We may need to implement background color too.

#if WINGRAPHICS_FINALIZATION_WATCH
        private string AllocationSite = DbgUtil.StackTrace;
#endif

        public abstract object Clone();     // Declaration required by C# even though this is an abstract class.

        protected abstract void CreateBrush();

        /// <devdoc>
        ///     Parameterless constructor to use default color.
        ///     Notice that the actual object construction is done in the derived classes.
        /// </devdoc>
        [ResourceExposure(ResourceScope.Process)]
        public WindowsBrush(DeviceContext dc)
        {
            this.dc = dc;
        }

        [ResourceExposure(ResourceScope.Process)]
        public WindowsBrush(DeviceContext dc, Color color)
        {
            this.dc = dc;
            this.color = color;
        }

        ~WindowsBrush()
        {
            Dispose(false);
        }

        protected DeviceContext DC {
            get {
                return this.dc;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (dc != null && this.nativeHandle != IntPtr.Zero)
            {
                DbgUtil.AssertFinalization(this, disposing);

                dc.DeleteObject(this.nativeHandle, GdiObjectType.Brush);
                
                this.nativeHandle = IntPtr.Zero;
            }

            if (disposing)
            {
                GC.SuppressFinalize(this);
            }
        }

        public Color Color
        {
            get
            {
                return this.color;
            }
        }

        /// <devdoc>
        ///       Gets the native Win32 brush handle. It creates it on demand.
        /// </devdoc>
        protected IntPtr NativeHandle
        {
            get
            { 
                if( this.nativeHandle == IntPtr.Zero )
                {
                    CreateBrush();
                }

                return this.nativeHandle;
            }
        
            set
            { 
                Debug.Assert(this.nativeHandle == IntPtr.Zero, "WindowsBrush object is immutable");
                Debug.Assert(value != IntPtr.Zero, "WARNING: assigning IntPtr.Zero to the nativeHandle object.");

                this.nativeHandle = value;
            }
        }

#if WINFORMS_PUBLIC_GRAPHICS_LIBRARY

        /// <devdoc>
        ///     Derived classes implement this method to get a native GDI brush wrapper with the same
        ///     properties as this object.
        /// </devdoc>
        [ResourceExposure(ResourceScope.Process)]
        [ResourceConsumption(ResourceScope.Process)]
        public static WindowsBrush FromBrush(DeviceContext dc, Brush originalBrush)
        {
            if(originalBrush is SolidBrush) {
                return  new WindowsSolidBrush(dc, ((SolidBrush)originalBrush).Color);
            }

            if(originalBrush is System.Drawing.Drawing2D.HatchBrush) {
                System.Drawing.Drawing2D.HatchBrush hatchBrush = ((System.Drawing.Drawing2D.HatchBrush)originalBrush);
                return new WindowsHatchBrush(dc, (WindowsHatchStyle) hatchBrush.HatchStyle, hatchBrush.ForegroundColor, hatchBrush.BackgroundColor);
            }

            Debug.Fail("Don't know how to convert this brush!");
            return null;
        }

        /// <devdoc>
        ///     Creates a WindowsBrush from the DC currently selected HBRUSH
        /// </devdoc>
        [ResourceExposure(ResourceScope.Process)]
        [ResourceConsumption(ResourceScope.Process)]
        public static WindowsBrush FromDC(DeviceContext dc)
        {
            IntPtr hBrush = IntUnsafeNativeMethods.GetCurrentObject(new HandleRef(null, dc.Hdc), IntNativeMethods.OBJ_BRUSH);
            IntNativeMethods.LOGBRUSH logBrush = new IntNativeMethods.LOGBRUSH();
            IntUnsafeNativeMethods.GetObject(new HandleRef(null, hBrush), logBrush);

            // don't call DeleteObject on handle from GetCurrentObject, it is the one selected in the hdc.

            return WindowsBrush.FromLogBrush(dc, logBrush );
        }

        /// <devdoc>
        ///     Creates a WindowsBrush from a LOGBRUSH.
        /// </devdoc>
        [ResourceExposure(ResourceScope.Process)]
        [ResourceConsumption(ResourceScope.Process)]
        public static WindowsBrush FromLogBrush( DeviceContext dc, IntNativeMethods.LOGBRUSH logBrush )
        {
            Debug.Assert( logBrush != null, "logBrush is null" );

            switch( logBrush.lbStyle )
            {
                // currently supported brushes:
                case IntNativeMethods.BS_HATCHED:
                    return new WindowsHatchBrush(dc, (WindowsHatchStyle) logBrush.lbHatch );

                case IntNativeMethods.BS_SOLID:
                    return new WindowsSolidBrush( dc, Color.FromArgb(logBrush.lbColor) );

                default:
                    Debug.Fail( "Don't know how to create WindowsBrush from specified logBrush" );
                    return null;
            }
        }
#endif

        /// <devdoc>
        ///    <para>
        ///       Returns the native Win32 brush handle.
        ///    </para>
        /// </devdoc>
        public IntPtr HBrush
        { 
            get
            {
                return this.NativeHandle;
            }
        }
    }

}
