// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using Microsoft.Win32;
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Windows.Forms.Internal;
    using System.Drawing.Drawing2D;
    using System.Runtime.InteropServices;

    /// <include file='doc\PaintEvent.uex' path='docs/doc[@for="PaintEventArgs"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Provides data for the <see cref='System.Windows.Forms.Control.Paint'/>
    ///       event.
    ///    </para>
    /// </devdoc>
    // NOTE: Please keep this class consistent with PrintPageEventArgs.
    public class PaintEventArgs : EventArgs, IDisposable {
        /// <devdoc>
        ///     Graphics object with which painting should be done.
        /// </devdoc>
        private Graphics graphics = null;

        // See ResetGraphics()
        private GraphicsState savedGraphicsState = null;

        /// <devdoc>
        ///     DC (Display context) for obtaining the graphics object. Used to delay getting the graphics
        ///     object until absolutely necessary (for perf reasons)
        /// </devdoc>
        private readonly IntPtr dc = IntPtr.Zero;
        IntPtr oldPal = IntPtr.Zero;

        /// <devdoc>
        ///     Rectangle into which all painting should be done.
        /// </devdoc>
        private readonly Rectangle clipRect;
        //private Control paletteSource;

#if DEBUG
        static readonly TraceSwitch PaintEventFinalizationSwitch = new TraceSwitch("PaintEventFinalization", "Tracks the creation and finalization of PaintEvent objects");
        internal static string GetAllocationStack()
        {
            if (PaintEventFinalizationSwitch.TraceVerbose)
            {
                return Environment.StackTrace;
            }
            else
            {
                return "Enabled 'PaintEventFinalization' trace switch to see stack of allocation";
            }
        }
        private string AllocationSite = PaintEventArgs.GetAllocationStack();
#endif

        /// <include file='doc\PaintEvent.uex' path='docs/doc[@for="PaintEventArgs.PaintEventArgs"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.Windows.Forms.PaintEventArgs'/> class with the specified graphics and
        ///       clipping rectangle.
        ///    </para>
        /// </devdoc>
        public PaintEventArgs(Graphics graphics, Rectangle clipRect) {
            if( graphics == null ){
                throw new ArgumentNullException(nameof(graphics));
            }

            this.graphics = graphics;
            this.clipRect = clipRect;
        }

        // Internal version of constructor for performance
        // We try to avoid getting the graphics object until needed
        internal PaintEventArgs(IntPtr dc, Rectangle clipRect) {
            Debug.Assert(dc != IntPtr.Zero, "dc is not initialized.");

            this.dc = dc;
            this.clipRect = clipRect;
        }

        /// <include file='doc\PaintEvent.uex' path='docs/doc[@for="PaintEventArgs.Finalize"]/*' />
        ~PaintEventArgs() {
            Dispose(false);
        }

        /// <include file='doc\PaintEvent.uex' path='docs/doc[@for="PaintEventArgs.ClipRectangle"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the
        ///       rectangle in which to paint.
        ///    </para>
        /// </devdoc>
        public Rectangle ClipRectangle {
            get {
                return clipRect;
            }
        }

        /// <devdoc>
        ///     Gets the HDC this paint event is connected to.  If there is no associated
        ///     HDC, or the GDI+ Graphics object has been created (meaning GDI+ now owns the
        ///     HDC), 0 is returned.
        ///
        /// </devdoc>
        internal IntPtr HDC {
            get {
                if (graphics == null)
                    return dc;
                else
                    return IntPtr.Zero;
            }
        }

        /// <include file='doc\PaintEvent.uex' path='docs/doc[@for="PaintEventArgs.Graphics"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the <see cref='System.Drawing.Graphics'/>
        ///       object used to
        ///       paint.
        ///    </para>
        /// </devdoc>
        public System.Drawing.Graphics Graphics {
            get {
                if (graphics == null && dc != IntPtr.Zero) {
                    oldPal = Control.SetUpPalette(dc, false /*force*/, false /*realize*/);
                    graphics = Graphics.FromHdcInternal(dc);
                    graphics.PageUnit = GraphicsUnit.Pixel;
                    savedGraphicsState = graphics.Save(); // See ResetGraphics() below
                }
                return graphics;
            }
        }

        // We want a way to dispose the GDI+ Graphics, but we don't want to create one
        // simply to dispose it
        // cpr: should be internal
        /// <include file='doc\PaintEvent.uex' path='docs/doc[@for="PaintEventArgs.Dispose"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Disposes
        ///       of the resources (other than memory) used by the <see cref='System.Windows.Forms.PaintEventArgs'/>.
        ///    </para>
        /// </devdoc>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <include file='doc\PaintEvent.uex' path='docs/doc[@for="PaintEventArgs.Dispose2"]/*' />
        protected virtual void Dispose(bool disposing) {
#if DEBUG
            Debug.Assert(disposing, "PaintEvent object should be explicitly disposed. Potential GDI multithreading lock issue. Allocation stack:\r\n" + AllocationSite);
#endif
           if (disposing) {
                //only dispose the graphics object if we created it via the dc.
                if (graphics != null && dc != IntPtr.Zero) {
                    graphics.Dispose();
                }
            }

            if (oldPal != IntPtr.Zero && dc != IntPtr.Zero) {
                SafeNativeMethods.SelectPalette(new HandleRef(this, dc), new HandleRef(this, oldPal), 0);
                oldPal = IntPtr.Zero;
            }
        }

        // If ControlStyles.AllPaintingInWmPaint, we call this method
        // after OnPaintBackground so it appears to OnPaint that it's getting a fresh
        // Graphics.  We want to make sure AllPaintingInWmPaint is purely an optimization,
        // and doesn't change behavior, so we need to make sure any clipping regions established
        // in OnPaintBackground don't apply to OnPaint.
        internal void ResetGraphics() {
            if (graphics != null) {
                Debug.Assert(dc == IntPtr.Zero || savedGraphicsState != null, "Called ResetGraphics more than once?");
                if (savedGraphicsState != null) {
                    graphics.Restore(savedGraphicsState);
                    savedGraphicsState = null;
                }
            }
        }
    }
}
