// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Provides data for the <see cref='System.Windows.Forms.Control.Paint'/>
    /// event.
    /// NOTE: Please keep this class consistent with PrintPageEventArgs.
    /// </devdoc>
    public class PaintEventArgs : EventArgs, IDisposable
    {
        /// <devdoc>
        /// Graphics object with which painting should be done.
        /// </devdoc>
        private Graphics _graphics;

        /// <devdoc>
        /// See ResetGraphics()
        /// </devdoc>
        private GraphicsState _savedGraphicsState;

        /// <devdoc>
        /// DC (Display context) for obtaining the graphics object. Used to delay
        /// getting the graphics object until absolutely necessary (for perf reasons)
        /// </devdoc>
        private readonly IntPtr _dc = IntPtr.Zero;

        private IntPtr oldPal = IntPtr.Zero;

#if DEBUG
        private static readonly TraceSwitch s_paintEventFinalizationSwitch = new TraceSwitch("PaintEventFinalization", "Tracks the creation and finalization of PaintEvent objects");

        internal static string GetAllocationStack()
        {
            if (s_paintEventFinalizationSwitch.TraceVerbose)
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

        /// <devdoc>
        /// Initializes a new instance of the <see cref='System.Windows.Forms.PaintEventArgs'/>
        /// class with the specified graphics and clipping rectangle.
        /// </devdoc>
        public PaintEventArgs(Graphics graphics, Rectangle clipRect)
        {
            _graphics = graphics ?? throw new ArgumentNullException(nameof(graphics));
            ClipRectangle = clipRect;
        }

        /// <devdoc>
        /// Internal version of constructor for performance
        /// We try to avoid getting the graphics object until needed
        /// </devdoc>
        internal PaintEventArgs(IntPtr dc, Rectangle clipRect)
        {
            Debug.Assert(dc != IntPtr.Zero, "dc is not initialized.");

            _dc = dc;
            ClipRectangle = clipRect;
        }

        ~PaintEventArgs() => Dispose(false);

        /// <devdoc>
        /// Gets the rectangle in which to paint.
        /// </devdoc>
        public Rectangle ClipRectangle { get; }

        /// <devdoc>
        /// Gets the HDC this paint event is connected to.  If there is no associated
        /// HDC, or the GDI+ Graphics object has been created (meaning GDI+ now owns the
        /// HDC), 0 is returned.
        /// </devdoc>
        internal IntPtr HDC => _graphics == null ? _dc : IntPtr.Zero;

        /// <devdoc>
        /// Gets the <see cref='System.Drawing.Graphics'/> object used to paint.
        /// </devdoc>
        public System.Drawing.Graphics Graphics
        {
            get
            {
                if (_graphics == null && _dc != IntPtr.Zero)
                {
                    oldPal = Control.SetUpPalette(_dc, force: false, realizePalette: false);
                    _graphics = Graphics.FromHdcInternal(_dc);
                    _graphics.PageUnit = GraphicsUnit.Pixel;
                    _savedGraphicsState = _graphics.Save(); // See ResetGraphics() below
                }

                return _graphics;
            }
        }

        /// <devdoc>
        /// Disposes of the resources (other than memory) used by the
        /// <see cref='System.Windows.Forms.PaintEventArgs'/>.
        /// </devdoc>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
           if (disposing)
           {
                // Only dispose the graphics object if we created it via the dc.
                if (_graphics != null && _dc != IntPtr.Zero)
                {
                    _graphics.Dispose();
                }
            }

            if (oldPal != IntPtr.Zero && _dc != IntPtr.Zero)
            {
                SafeNativeMethods.SelectPalette(new HandleRef(this, _dc), new HandleRef(this, oldPal), 0);
                oldPal = IntPtr.Zero;
            }
        }

        /// <devdoc>
        /// If ControlStyles.AllPaintingInWmPaint, we call this method
        /// after OnPaintBackground so it appears to OnPaint that it's getting a fresh
        /// Graphics.  We want to make sure AllPaintingInWmPaint is purely an optimization,
        /// and doesn't change behavior, so we need to make sure any clipping regions established
        /// in OnPaintBackground don't apply to OnPaint.
        /// </devdoc>
        internal void ResetGraphics()
        {
            if (_graphics != null)
            {
                Debug.Assert(_dc == IntPtr.Zero || _savedGraphicsState != null, "Called ResetGraphics more than once?");
                if (_savedGraphicsState != null)
                {
                    _graphics.Restore(_savedGraphicsState);
                    _savedGraphicsState = null;
                }
            }
        }
    }
}
