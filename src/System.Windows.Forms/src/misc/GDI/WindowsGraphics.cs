// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//#define GDI_FINALIZATION_WATCH

// THIS PARTIAL CLASS CONTAINS THE BASE METHODS FOR CREATING AND DISPOSING A WINDOWSGRAPHICS AS WELL
// GETTING, DISPOSING AND WORKING WITH A DC.

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
    using System.Diagnostics.CodeAnalysis;    
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Runtime.Versioning;

    /// <include file='doc\WindowsGraphics.uex' path='docs/doc[@for="WindowsGraphics"]/*' />
    /// <devdoc>
    ///     WindowsGraphics is a library for rendering text and drawing using GDI; it was
    ///     created to address performance and compatibility issues found in GDI+ Graphics
    ///     class.
    ///     
    ///     Note: WindowsGraphics is a stateful component, DC properties are persisted from 
    ///     method calls, as opposed to Graphics (GDI+) which performs attomic operations and 
    ///     always restores the hdc.
    ///     The underlying hdc is always saved and restored on dispose so external HDCs won't
    ///     be modified by WindowsGraphics.  So we don't need to restore previous objects into 
    ///     the dc in method calls.
    ///</devdoc>
#if WINFORMS_PUBLIC_GRAPHICS_LIBRARY
    public
#else
    internal
#endif
    sealed partial class WindowsGraphics : MarshalByRefObject, IDisposable, IDeviceContext
    {
        // Wrapper around the window dc this object refers to.
        // Note: this dc is only disposed when owned (created) by the WindowsGraphics.
        DeviceContext dc;
        bool disposeDc;
        Graphics graphics; // cached when initialized FromGraphics to be able to call g.ReleaseHdc from Dispose.

#if GDI_FINALIZATION_WATCH
        private string AllocationSite = DbgUtil.StackTrace;
#endif

        // Construction/destruction API

        /// <include file='doc\WindowsGraphics.uex' path='docs/doc[@for="WindowsGraphics.WindowsGraphics"]/*' />
        public WindowsGraphics( DeviceContext dc )
        {
            Debug.Assert( dc != null, "null dc!");         
            this.dc = dc;
            this.dc.SaveHdc();
            //this.disposeDc = false; // the dc is not owned by this object.
        }

        /// <devdoc>
        ///     Creates a WindowsGraphics from a memory DeviceContext object compatible with the primary screen device.
        ///     This object is suitable for performing text measuring but not for drawing into it because it does 
        ///     not have a backup bitmap.
        /// </devdoc>
        [ResourceExposure(ResourceScope.Process)]
        [ResourceConsumption(ResourceScope.Process)]
        public static WindowsGraphics CreateMeasurementWindowsGraphics()
        {
            DeviceContext dc = DeviceContext.FromCompatibleDC(IntPtr.Zero);
            WindowsGraphics wg = new WindowsGraphics(dc);
            wg.disposeDc = true; // we create it, we dispose it.
            
            return wg;
        }

        /// <devdoc>
        ///     Creates a WindowsGraphics from a memory DeviceContext object compatible with the a screen device.
        ///     This object is suitable for performing text measuring but not for drawing into it because it does 
        ///     not have a backup bitmap.
        /// </devdoc>
        [ResourceExposure(ResourceScope.Process)]
        [ResourceConsumption(ResourceScope.Process)]
        public static WindowsGraphics CreateMeasurementWindowsGraphics(IntPtr screenDC)
        {
            DeviceContext dc = DeviceContext.FromCompatibleDC(screenDC);
            WindowsGraphics wg = new WindowsGraphics(dc);
            wg.disposeDc = true; // we create it, we dispose it.
            
            return wg;
        }

        /// <include file='doc\WindowsGraphics.uex' path='docs/doc[@for="WindowsGraphics.FromHwnd"]/*' />
        public static WindowsGraphics FromHwnd(IntPtr hWnd)
        { 
            DeviceContext dc = DeviceContext.FromHwnd( hWnd );
            WindowsGraphics wg = new WindowsGraphics( dc );
            wg.disposeDc = true; // we create it, we dispose it.
             
            return wg;    
        }

        /// <include file='doc\WindowsGraphics.uex' path='docs/doc[@for="WindowsGraphics.FromHwnd"]/*' />
        [ResourceExposure(ResourceScope.Process)]
        [ResourceConsumption(ResourceScope.Process)]
        public static WindowsGraphics FromHdc(IntPtr hDc)
        {
            Debug.Assert( hDc != IntPtr.Zero, "null hDc" );
            
            DeviceContext dc = DeviceContext.FromHdc(hDc);
            WindowsGraphics wg = new WindowsGraphics( dc ); 
            wg.disposeDc = true; // we create it, we dispose it.

            return wg;
        }

        /// <devdoc>
        ///     Creates a WindowsGraphics object from a Graphics object.  Clipping and coordinate transforms
        ///     are preserved.
        ///     
        ///     Notes: 
        ///     - The passed Graphics object cannot be used until the WindowsGraphics is disposed
        ///     since it borrows the hdc from the Graphics object locking it.
        ///     - Changes to the hdc using the WindowsGraphics object are not preserved into the Graphics object;
        ///     the hdc is returned to the Graphics object intact.
        ///     
        ///     Some background about how Graphics uses the internal hdc when created from an existing one
        ///     (mail from GillesK from GDI+ team):
        ///     User has an HDC with a particular state:
        ///     Graphics object gets created based on that HDC. We query the HDC for its state and apply it to the Graphics. 
        ///     At this stage, we do a SaveHDC and clear everything out of it.
        ///     User calls GetHdc. We restore the HDC to the state it was in and give it to the user.
        ///     User calls ReleaseHdc, we save the current state of the HDC and clear everything 
        ///     (so that the graphics state gets applied next time we use it).
        ///     Next time the user calls GetHdc we give him back the state after the second ReleaseHdc. 
        ///     (But the state changes between the GetHdc and ReleaseHdc are not applied to the Graphics).
        ///     Please note that this only applies the HDC created graphics, for Bitmap derived graphics, GetHdc creates a new DIBSection and 
        ///     things get a lot more complicated.
        /// </devdoc>
        [ResourceExposure(ResourceScope.Process)]
        [ResourceConsumption(ResourceScope.Process)]
        public static WindowsGraphics FromGraphics(Graphics g)
        {
            ApplyGraphicsProperties properties = ApplyGraphicsProperties.All;
            return WindowsGraphics.FromGraphics(g, properties);
        }

        [ResourceExposure(ResourceScope.Process)]
        [ResourceConsumption(ResourceScope.Process)]
        public static WindowsGraphics FromGraphics(Graphics g, ApplyGraphicsProperties properties)
        {
            Debug.Assert( g != null, "null Graphics object." );
            //Debug.Assert( properties != ApplyGraphicsProperties.None, "Consider using other WindowsGraphics constructor if not preserving Graphics properties." );

            WindowsRegion wr = null;
            float[] elements = null;

            Region clipRgn = null;
            Matrix worldTransf = null;

            if ((properties & ApplyGraphicsProperties.TranslateTransform) != 0 || (properties & ApplyGraphicsProperties.Clipping) != 0)
            {
                object[] data = g.GetContextInfo() as object[];

                if( data != null && data.Length == 2 )
                {
                    clipRgn = data[0] as Region;
                    worldTransf = data[1] as Matrix;
                }

                if( worldTransf != null )
                {
                    if ((properties & ApplyGraphicsProperties.TranslateTransform) != 0) 
                    {
                        elements = worldTransf.Elements;
                    }
                    worldTransf.Dispose();
                }

                if( clipRgn != null )
                {
                    if ((properties & ApplyGraphicsProperties.Clipping) != 0)
                    {
                        // We have to create the WindowsRegion and dipose the Region object before locking the Graphics object,
                        // in case of an unlikely exception before releasing the WindowsRegion, the finalizer will do it for us.
                        // (no try-finally block since this method is used frequently - perf).                        
                        // If the Graphics.Clip has not been set (Region.IsInfinite) we don't need to apply it to the DC.
                        if (!clipRgn.IsInfinite(g)) 
                        {
                            wr = WindowsRegion.FromRegion(clipRgn, g); // WindowsRegion will take ownership of the hRegion.
                        }
                    }
                    clipRgn.Dispose(); // Disposing the Region object doesn't destroy the hRegion.
                }
            }

            WindowsGraphics wg = WindowsGraphics.FromHdc( g.GetHdc() ); // This locks the Graphics object.
            wg.graphics = g;

            // Apply transform and clip
            if (wr != null)
            {
                using (wr)
                {
                    // If the Graphics object was created from a native DC the actual clipping region is the intersection
                    // beteween the original DC clip region and the GDI+ one - for display Graphics it is the same as 
                    // Graphics.VisibleClipBounds.
                    wg.DeviceContext.IntersectClip(wr);

                }
            }

            if (elements != null)
            {
                // elements (XFORM) = [eM11, eM12, eM21, eM22, eDx, eDy], eDx/eDy specify the translation offset.
                wg.DeviceContext.TranslateTransform((int)elements[4], (int)elements[5]);
            }

            return wg;
        }

        /// <include file='doc\WindowsGraphics.uex' path='docs/doc[@for="WindowsGraphics.Finalize"]/*' />
        ~WindowsGraphics()
        { 
            Dispose(false);
        }

        public DeviceContext DeviceContext 
        {
            get 
            {
                return this.dc;
            }
        }


        /// <include file='doc\WindowsGraphics.uex' path='docs/doc[@for="WindowsGraphics.Dispose"]/*' />
        // Okay to suppress.
        //"WindowsGraphics object does not own the Graphics object.  For instance in a control’s Paint event we pass the 
        //GraphicsContainer object to TextRenderer, which uses WindowsGraphics; 
        //if the Graphics object is disposed then further painting will be broken."
        [SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed")]
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        [SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed")]
        [ResourceExposure(ResourceScope.None)]
        [ResourceConsumption(ResourceScope.Process, ResourceScope.Process)]
        internal void Dispose(bool disposing)
        {
            if (this.dc != null )
            {
                DbgUtil.AssertFinalization(this, disposing);

                try
                {
                    // Restore original dc.
                    this.dc.RestoreHdc();

                    if( this.disposeDc)
                    {
                        this.dc.Dispose(disposing);
                    }

                    if( this.graphics != null )    // if created from a Graphics object...
                    {
                        this.graphics.ReleaseHdcInternal(this.dc.Hdc);
                        this.graphics = null;
                    }
                                
                }
                catch(Exception ex )
                {
                    if( ClientUtils.IsSecurityOrCriticalException( ex ) )
                    {
                        throw; // rethrow the original exception.
                    }
                    Debug.Fail("Exception thrown during disposing: \r\n" + ex.ToString());
                }
                finally {
                    this.dc = null;
                }
            }
        }

        /// <include file='doc\WindowsGraphics.uex' path='docs/doc[@for="WindowsGraphics.GetHdc"]/*' />
        [ResourceExposure(ResourceScope.Process)]
        [ResourceConsumption(ResourceScope.Process)]
        public IntPtr GetHdc()
        {
            return this.dc.Hdc;
        }

        /// <include file='doc\WindowsGraphics.uex' path='docs/doc[@for="WindowsGraphics.ReleaseHdc"]/*' />
        public void ReleaseHdc()
        {
            this.dc.Dispose();
        }
    }
}
