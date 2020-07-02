// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Provides data for the <see cref='Control.Paint'/>
    ///  event.
    ///  NOTE: Please keep this class consistent with PrintPageEventArgs.
    /// </summary>
    public class PaintEventArgs : EventArgs, IDisposable, IHandle
    {
        /// <summary>
        ///  Graphics object with which painting should be done.
        /// </summary>
        private Graphics _graphics;

        /// <summary>
        ///  See ResetGraphics()
        /// </summary>
        private GraphicsState _savedGraphicsState;

        /// <summary>
        ///  DC (Display context) for obtaining the graphics object. Used to delay
        ///  getting the graphics object until absolutely necessary (for perf reasons)
        /// </summary>
        private readonly Gdi32.HDC _dc;

        private Gdi32.HPALETTE _oldPal;

        /// <summary>
        ///  Initializes a new instance of the <see cref='PaintEventArgs'/>
        ///  class with the specified graphics and clipping rectangle.
        /// </summary>
        public PaintEventArgs(Graphics graphics, Rectangle clipRect)
        {
            _graphics = graphics ?? throw new ArgumentNullException(nameof(graphics));
            ClipRectangle = clipRect;
        }

        /// <summary>
        ///  Internal version of constructor for performance
        ///  We try to avoid getting the graphics object until needed
        /// </summary>
        internal PaintEventArgs(Gdi32.HDC dc, Rectangle clipRect)
        {
            _dc = dc;
            ClipRectangle = clipRect;
        }

        ~PaintEventArgs() => Dispose(false);

        /// <summary>
        ///  Gets the rectangle in which to paint.
        /// </summary>
        public Rectangle ClipRectangle { get; }

        /// <summary>
        ///  Gets the HDC this paint event is connected to.  If there is no associated
        ///  HDC, or the GDI+ Graphics object has been created (meaning GDI+ now owns the
        ///  HDC), 0 is returned.
        /// </summary>
        internal Gdi32.HDC HDC => _graphics == null ? _dc : default;

        internal bool IsGraphicsCreated => _graphics != null;

        /// <summary>
        ///  Gets the <see cref='Drawing.Graphics'/> object used to paint.
        /// </summary>
        public Graphics Graphics
        {
            get
            {
                if (_graphics == null && !_dc.IsNull)
                {
                    // We need to manually unset the palette here so this scope shouldn't be disposed
                    var palleteScope = Gdi32.SelectPaletteScope.HalftonePalette(_dc, forceBackground: false, realizePalette: false);
                    _oldPal = palleteScope.HPalette;
                    _graphics = Graphics.FromHdcInternal((IntPtr)_dc);
                    _graphics.PageUnit = GraphicsUnit.Pixel;
                    _savedGraphicsState = _graphics.Save(); // See ResetGraphics() below
                }

                return _graphics;
            }
        }

        IntPtr IHandle.Handle => (IntPtr)_dc;

        /// <summary>
        ///  Disposes of the resources (other than memory) used by the
        /// <see cref='PaintEventArgs'/>.
        /// </summary>
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
                if (_graphics != null && !_dc.IsNull)
                {
                    _graphics.Dispose();
                }
            }

            if (!_oldPal.IsNull && !_dc.IsNull)
            {
                Gdi32.SelectPalette(_dc, _oldPal, BOOL.FALSE);
                _oldPal = default;
            }
        }

        /// <summary>
        ///  If ControlStyles.AllPaintingInWmPaint, we call this method
        ///  after OnPaintBackground so it appears to OnPaint that it's getting a fresh
        ///  Graphics.  We want to make sure AllPaintingInWmPaint is purely an optimization,
        ///  and doesn't change behavior, so we need to make sure any clipping regions established
        ///  in OnPaintBackground don't apply to OnPaint.
        /// </summary>
        internal void ResetGraphics()
        {
            if (_graphics != null)
            {
                Debug.Assert(_dc.IsNull || _savedGraphicsState != null, "Called ResetGraphics more than once?");
                if (_savedGraphicsState != null)
                {
                    _graphics.Restore(_savedGraphicsState);
                    _savedGraphicsState = null;
                }
            }
        }
    }
}
