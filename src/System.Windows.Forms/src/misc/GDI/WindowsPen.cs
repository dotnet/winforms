// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using static Interop;

namespace System.Windows.Forms.Internal
{
    /// <summary>
    ///  Encapsulates a GDI Pen object.
    /// </summary>
    internal sealed partial class WindowsPen : MarshalByRefObject, ICloneable, IDisposable
    {
        private IntPtr _nativeHandle;

        private readonly DeviceContext _dc;

        private WindowsBrush _wndBrush;
        private Gdi32.PS _style;
        private readonly Color _color;
        private readonly int _width;

        private const int CosmeticPenWidth = 1;

#if GDI_FINALIZATION_WATCH
        private string AllocationSite = DbgUtil.StackTrace;
#endif

        public WindowsPen(DeviceContext dc) :
            this(dc, default, CosmeticPenWidth, Color.Black)
        {
        }

        public WindowsPen(DeviceContext dc, Color color) :
            this(dc, default, CosmeticPenWidth, color)
        {
        }

        public WindowsPen(DeviceContext dc, WindowsBrush windowsBrush) :
            this(dc, default, CosmeticPenWidth, windowsBrush)
        {
        }

        public WindowsPen(DeviceContext dc, Gdi32.PS style, int width, Color color)
        {
            _style = style;
            _width = width;
            _color = color;
            _dc = dc;
        }

        public WindowsPen(DeviceContext dc, Gdi32.PS style, int width, WindowsBrush windowsBrush)
        {
            Debug.Assert(windowsBrush != null, "null windowsBrush");

            _style = style;
            _wndBrush = (WindowsBrush)windowsBrush.Clone();
            _width = width;
            _color = windowsBrush.Color;
            _dc = dc;
        }

        private unsafe void CreatePen()
        {
            if (_width > 1)
            {
                // Geometric pen.
                // From MSDN: if width > 1, the style must be PS_NULL, PS_SOLID, or PS_INSIDEFRAME.
                _style |= Gdi32.PS.GEOMETRIC | Gdi32.PS.SOLID;
            }

            if (_wndBrush == null)
            {
                _nativeHandle = Gdi32.CreatePen(_style, _width, ColorTranslator.ToWin32(_color));
            }
            else
            {
                var lb = new Gdi32.LOGBRUSH
                {
                    lbColor = ColorTranslator.ToWin32(_wndBrush.Color),
                    lbStyle = Gdi32.BS.SOLID,
                    lbHatch = IntPtr.Zero
                };

                // Note: We currently don't support custom styles, that's why 0 and null for last two params.
                _nativeHandle = Gdi32.ExtCreatePen(_style, _width, ref lb, 0, null);
            }
        }

        public object Clone()
        {
            return (_wndBrush != null) ?
                new WindowsPen(_dc, _style, _width, (WindowsBrush)_wndBrush.Clone()) :
                new WindowsPen(_dc, _style, _width, _color);
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
            if (_nativeHandle != IntPtr.Zero && _dc != null)
            {
                DbgUtil.AssertFinalization(this, disposing);

                _dc.DeleteObject(_nativeHandle, GdiObjectType.Pen);
                _nativeHandle = IntPtr.Zero;
            }

            if (_wndBrush != null)
            {
                _wndBrush.Dispose();
                _wndBrush = null;
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
                if (_nativeHandle == IntPtr.Zero)
                {
                    CreatePen();
                }

                return _nativeHandle;
            }
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}: Style={1}, Color={2}, Width={3}, Brush={4}",
                GetType().Name,
                _style,
                _color,
                _width,
                _wndBrush != null ? _wndBrush.ToString() : "null");
        }
    }
}
