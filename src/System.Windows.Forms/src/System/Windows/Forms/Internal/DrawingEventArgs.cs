// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Struct that helps ensure we build our <see cref="EventArgs"/> that wrap <see cref="DrawingEventArgs"/>
    ///  the same way.
    /// </summary>
    /// <remarks>
    ///  We should consider making this a base class for the event args that use this rather than a nested struct.
    ///  That would make things a little more robust, but would require API review as the class itself would have to
    ///  be public. The internal functionality can obviously still be internal.
    /// </remarks>
    internal partial class DrawingEventArgs
    {
        private Graphics? _graphics;

        /// <summary>
        ///  DC (Display context) for obtaining the graphics object. Used to delay getting the graphics object until
        ///  absolutely necessary (for perf reasons)
        /// </summary>
        private readonly Gdi32.HDC _hdc;
        private Gdi32.HPALETTE _oldPalette;

        public DrawingEventArgs(
            Graphics graphics,
            Rectangle clipRect,
            DrawingEventFlags flags)
        {
            _graphics = graphics ?? throw new ArgumentNullException(nameof(graphics));
            _hdc = default;
            _oldPalette = default;
            CheckGraphicsForState(graphics, flags);
            ClipRectangle = clipRect;
            Flags = flags;
        }

        /// <summary>
        ///  Internal version of constructor for performance. We try to avoid getting the graphics object until needed.
        /// </summary>
        public DrawingEventArgs(
            Gdi32.HDC dc,
            Rectangle clipRect,
            DrawingEventFlags flags)
        {
            if (dc.IsNull)
                throw new ArgumentNullException(nameof(dc));

#if DEBUG
            Gdi32.OBJ type = Gdi32.GetObjectType(dc);
            Debug.Assert(type == Gdi32.OBJ.DC
                || type == Gdi32.OBJ.ENHMETADC
                || type == Gdi32.OBJ.MEMDC
                || type == Gdi32.OBJ.METADC);
#endif

            _hdc = dc;
            _graphics = null;
            _oldPalette = default;
            Flags = flags;
            ClipRectangle = clipRect;
        }

        internal DrawingEventFlags Flags { get; private set; }
        internal Rectangle ClipRectangle { get; }

        internal bool IsStateClean => !Flags.HasFlag(DrawingEventFlags.GraphicsStateUnclean);

        /// <summary>
        ///  Gets the HDC this event is connected to.  If there is no associated HDC, or the GDI+ Graphics object has
        ///  been externally accessed (where it may have gotten a transform or clip) a null handle is returned.
        /// </summary>
        internal Gdi32.HDC HDC => IsStateClean ? default : _hdc;

        /// <summary>
        ///  Gets the <see cref='Graphics'/> object used to paint.
        /// </summary>
        internal Graphics Graphics
        {
            get
            {
                // If we're giving this out on the public API expect it to get a clip or transform applied.
                Flags |= DrawingEventFlags.GraphicsStateUnclean;
                return GetOrCreateGraphicsInternal();
            }
        }

        /// <summary>
        ///  For internal use to improve performance. DO NOT use this method if you modify the Graphics Clip or Transform.
        /// </summary>
        internal Graphics GetOrCreateGraphicsInternal(Action<Graphics>? creationAction = null)
        {
            if (_graphics is null)
            {
                Debug.Assert(!_hdc.IsNull);

                // We need to manually unset the palette here so this scope shouldn't be disposed
                var palleteScope = Gdi32.SelectPaletteScope.HalftonePalette(
                    _hdc,
                    forceBackground: false,
                    realizePalette: false);

                GC.SuppressFinalize(palleteScope);

                _oldPalette = palleteScope.HPalette;

                _graphics = Graphics.FromHdcInternal((IntPtr)_hdc);
                _graphics.PageUnit = GraphicsUnit.Pixel;
                creationAction?.Invoke(_graphics);

                CheckGraphicsForState(_graphics, Flags);
            }

            return _graphics;
        }

        internal Gdi32.HDC GetHDC() => _hdc;

        internal Graphics? GetGraphics(bool create)
        {
            CheckGraphicsForState(_graphics, Flags);
            return create ? GetOrCreateGraphicsInternal() : _graphics;
        }

        internal void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Only dispose the graphics object if we created it via the HDC.
                if (_graphics != null && !_hdc.IsNull)
                {
                    _graphics.Dispose();
                }
            }

            if (!_oldPalette.IsNull && !_hdc.IsNull)
            {
                Gdi32.SelectPalette(_hdc, _oldPalette, BOOL.FALSE);
                _oldPalette = default;
            }
        }

        [Conditional("DEBUG")]
        internal static void CheckGraphicsForState(Graphics? graphics, DrawingEventFlags flags)
        {
            if (graphics is null || !flags.HasFlag(DrawingEventFlags.CheckState)
                || flags.HasFlag(DrawingEventFlags.GraphicsStateUnclean))
            {
                return;
            }

            // Check to see if we've actually corrupted the state
            object[] data = (object[])graphics.GetContextInfo();

            using Region clipRegion = (Region)data[0];
            using Matrix worldTransform = (Matrix)data[1];

            float[] elements = worldTransform?.Elements!;
            bool isInfinite = clipRegion.IsInfinite(graphics);
            Debug.Assert((int)elements[4] == 0 && (int)elements[5] == 0, "transform has been modified");
            Debug.Assert(isInfinite, "clipping as been applied");
        }
    }
}
