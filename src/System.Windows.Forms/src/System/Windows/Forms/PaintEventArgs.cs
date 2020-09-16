// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Provides data for the <see cref='Control.Paint'/> event.
    /// </summary>
    /// <remarks>
    ///  Please keep this class consistent with <see cref="PrintPageEventArgs"/>.
    /// </remarks>
    public partial class PaintEventArgs : EventArgs, IDisposable, IDeviceContext, IGraphicsHdcProvider
    {
        private readonly DrawingEventArgs _event;

        /// <remarks>
        ///  This is only needed for <see cref="ResetGraphics"/> callers and applies in the following places:
        ///
        ///    1. In <see cref="Control.WmPaint(ref Message)"/> when we are painting the background.
        ///    2. In <see cref="Control.WmPrintClient(ref Message)"/>.
        ///
        ///  We can potentially optimize further by skipping the save when we only use <see cref="GraphicsInternal"/>
        ///  as we shouldn't have messed with the clipping there.
        /// </remarks>
        private GraphicsState _savedGraphicsState;

        public PaintEventArgs(Graphics graphics, Rectangle clipRect) : this(
            graphics,
            clipRect,
            // If Graphics comes in on the public constructor we don't know that it has no transform or clip
            flags: DrawingEventFlags.GraphicsStateUnclean)
        {
        }

        internal PaintEventArgs(
            PaintEventArgs e,
            Rectangle clipRect)
        {
            Gdi32.HDC hdc = e.HDC;
            _event = hdc.IsNull
                ? new DrawingEventArgs(e.GraphicsInternal, clipRect, e._event.Flags)
                : new DrawingEventArgs(hdc, clipRect, e._event.Flags);
        }

        internal PaintEventArgs(
            Graphics graphics,
            Rectangle clipRect,
            DrawingEventFlags flags)
        {
            _event = new DrawingEventArgs(graphics, clipRect, flags);
            SaveStateIfNeeded(graphics);
        }

        /// <summary>
        ///  Internal version of constructor for performance. We try to avoid getting the graphics object until needed.
        /// </summary>
        internal PaintEventArgs(
            Gdi32.HDC hdc,
            Rectangle clipRect,
            DrawingEventFlags flags = DrawingEventFlags.CheckState)
        {
            _event = new DrawingEventArgs(hdc, clipRect, flags);
        }

        ~PaintEventArgs() => Dispose(false);

        /// <summary>
        ///  Gets the rectangle in which to paint.
        /// </summary>
        public Rectangle ClipRectangle => _event.ClipRectangle;

        /// <summary>
        ///  Gets the <see cref='Drawing.Graphics'/> object used to paint.
        /// </summary>
        public Graphics Graphics => _event.Graphics;

        /// <summary>
        ///  Disposes of the resources (other than memory) used by the <see cref='PaintEventArgs'/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) => _event?.Dispose(disposing);

        /// <summary>
        ///  If ControlStyles.AllPaintingInWmPaint, we call this method after OnPaintBackground so it appears to
        ///  OnPaint that it's getting a fresh Graphics.  We want to make sure AllPaintingInWmPaint is purely an
        ///  optimization, and doesn't change behavior, so we need to make sure any clipping regions established in
        ///  OnPaintBackground don't apply to OnPaint.
        /// </summary>
        internal void ResetGraphics()
        {
            Graphics graphics = _event.GetGraphics(create: false);
            if (_event.Flags.HasFlag(DrawingEventFlags.SaveState) && graphics != null)
            {
                if (_savedGraphicsState != null)
                {
                    graphics.Restore(_savedGraphicsState);
                    _savedGraphicsState = null;
                }
                else
                {
                    Debug.Fail("Called ResetGraphics more than once?");
                }
            }
        }

        private void SaveStateIfNeeded(Graphics graphics)
            => _savedGraphicsState = _event.Flags.HasFlag(DrawingEventFlags.SaveState) ? graphics.Save() : default;

        /// <summary>
        ///  For internal use to improve performance. DO NOT use this method if you modify the Graphics Clip or Transform.
        /// </summary>
        internal Graphics GraphicsInternal => _event.GetOrCreateGraphicsInternal(SaveStateIfNeeded);

        /// <summary>
        ///  Returns the <see cref="Gdi32.HDC"/> the event was created off of, if any.
        /// </summary>
        internal Gdi32.HDC HDC => _event.HDC;

        IntPtr IDeviceContext.GetHdc() => Graphics?.GetHdc() ?? IntPtr.Zero;
        void IDeviceContext.ReleaseHdc() => Graphics?.ReleaseHdc();
        Gdi32.HDC IGraphicsHdcProvider.GetHDC() => _event.GetHDC();
        Graphics IGraphicsHdcProvider.GetGraphics(bool create) => _event.GetGraphics(create);
        bool IGraphicsHdcProvider.IsGraphicsStateClean => _event.IsStateClean;
    }
}
