// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if DRAWING_DESIGN_NAMESPACE
namespace System.Windows.Forms.Internal
#elif DRAWING_NAMESPACE
namespace System.Drawing.Internal
#else
namespace System.Experimental.Gdi
#endif
{
    using System;
    using System.Runtime.InteropServices;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Globalization;
    using System.Runtime.Versioning;

    /// <summary>
    ///     <para>
    ///         Encapsulates a GDI Region object.
    ///     </para>
    /// </summary>

#if WINFORMS_PUBLIC_GRAPHICS_LIBRARY
    public
#else
    internal
#endif
    sealed partial class WindowsRegion : MarshalByRefObject, ICloneable, IDisposable 
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2006:UseSafeHandleToEncapsulateNativeResources")]
        private IntPtr nativeHandle; // The hRegion, this class always takes ownership of the hRegion.
        private bool ownHandle;

#if GDI_FINALIZATION_WATCH
        private string AllocationSite = DbgUtil.StackTrace;
#endif

        /// <summary>
        /// </summary>
        private WindowsRegion() {
        }

        /// <summary>
        /// </summary>
        
        
        public WindowsRegion(Rectangle rect) {
            CreateRegion(rect);
        }

        /// <summary>
        /// </summary>
        
        
        public WindowsRegion(int x, int y, int width, int height) {
            CreateRegion(new Rectangle(x,y,width, height));
        }

        // Consider implementing a constructor that calls ExtCreateRegion(XFORM lpXform, DWORD nCount, RGNDATA lpRgnData) if needed.

        /// <summary>
        ///     Creates a WindowsRegion from a region handle, if 'takeOwnership' is true, the handle is added to the HandleCollector
        ///     and is removed & destroyed on dispose. 
        /// </summary>
        public static WindowsRegion FromHregion(IntPtr hRegion, bool takeOwnership) {
            WindowsRegion wr = new WindowsRegion();

            // Note: Passing IntPtr.Zero for hRegion is ok.  GDI+ infinite regions will have hRegion == null.  
            // GDI's SelectClipRgn interprets null region handle as resetting the clip region (all region will be available for painting).
            if( hRegion != IntPtr.Zero ) {
                wr.nativeHandle = hRegion;

                if (takeOwnership) {
                    wr.ownHandle = true;
                    Interop.HandleCollector.Add(hRegion, Interop.CommonHandles.GDI);
                }
            }
            return wr;
        }

        /// <summary>
        ///     Creates a WindowsRegion from a System.Drawing.Region. 
        /// </summary>
        public static WindowsRegion FromRegion( Region region, Graphics g ){
            if (region.IsInfinite(g)){
                // An infinite region would cover the entire device region which is the same as
                // not having a clipping region. Observe that this is not the same as having an
                // empty region, which when clipping to it has the effect of excluding the entire
                // device region.
                // To remove the clip region from a dc the SelectClipRgn() function needs to be
                // called with a null region ptr - that's why we use the empty constructor here.
                // GDI+ will return IntPtr.Zero for Region.GetHrgn(Graphics) when the region is
                // Infinite.
                return new WindowsRegion();
            }

            return WindowsRegion.FromHregion(region.GetHrgn(g), true);
        }

        /// <summary>
        /// </summary>
        
        
        public object Clone() {
            // WARNING: WindowsRegion currently supports rectangulare regions only, if the WindowsRegion was created
            //          from an HRegion and it is not rectangular this method won't work as expected.
            // Note:    This method is currently not used and is here just to implement ICloneable.
            return this.IsInfinite ? 
                new WindowsRegion() :
                new WindowsRegion(this.ToRectangle());
        }

        /// <summary>
        ///     Combines region1 & region2 into this region.   The regions cannot be null. 
        ///     The three regions need not be distinct. For example, the sourceRgn1 can equal this region. 
        /// </summary>
        public IntNativeMethods.RegionFlags CombineRegion(WindowsRegion region1, WindowsRegion region2, RegionCombineMode mode) {
            return IntUnsafeNativeMethods.CombineRgn(new HandleRef(this, this.HRegion), new HandleRef(region1, region1.HRegion), new HandleRef(region2, region2.HRegion), mode);
        }

        /// <summary>
        /// </summary>
        
        
        private void CreateRegion(Rectangle rect) {
            Debug.Assert(nativeHandle == IntPtr.Zero, "nativeHandle should be null, we're leaking handle");
            this.nativeHandle = IntSafeNativeMethods.CreateRectRgn(rect.X, rect.Y, rect.X+rect.Width, rect.Y+rect.Height);
            ownHandle = true;
        }

        /// <summary>
        /// </summary>
        public void Dispose() {
            Dispose(true);
        }

        /// <summary>
        /// </summary>
        public void Dispose(bool disposing) {
            if (this.nativeHandle != IntPtr.Zero) {
                DbgUtil.AssertFinalization(this, disposing);

                if( this.ownHandle ) {
                    IntUnsafeNativeMethods.DeleteObject(new HandleRef(this, this.nativeHandle));
                }

                this.nativeHandle = IntPtr.Zero;

                if (disposing) {
                    GC.SuppressFinalize(this);
                }
            }
        }

         /// <summary>
        /// </summary>
       ~WindowsRegion() {
            Dispose(false);
        }

        /// <summary>
        ///     The native region handle. 
        /// </summary>
        public IntPtr HRegion {
            get {
                return this.nativeHandle;
            }
        }

        /// <summary>
        /// </summary>
        public bool IsInfinite {
            get {
                return this.nativeHandle == IntPtr.Zero;
            }
        }

        /// <summary>
        ///     A rectangle representing the window region set with the SetWindowRgn function. 
        /// </summary>
        public Rectangle ToRectangle() {            
            if( this.IsInfinite ) {
                return new Rectangle( -int.MaxValue, -int.MaxValue, int.MaxValue, int.MaxValue );
            }

            IntNativeMethods.RECT rect = new IntNativeMethods.RECT();
            IntUnsafeNativeMethods.GetRgnBox(new HandleRef(this, this.nativeHandle), ref rect);
            return new Rectangle(new Point(rect.left, rect.top), rect.Size);
        }
    }
}

