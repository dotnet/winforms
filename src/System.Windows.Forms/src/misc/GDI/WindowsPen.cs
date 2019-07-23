// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Drawing;
using System.Globalization;

namespace System.Windows.Forms.Internal
{
    /// <summary>
    ///  Encapsulates a GDI Pen object.
    /// </summary>
    internal sealed partial class WindowsPen : MarshalByRefObject, ICloneable, IDisposable
    {
        //
        // Handle to the native Windows pen object.
        //
        private IntPtr nativeHandle;

        private const int dashStyleMask = 0x0000000F;
        private const int endCapMask = 0x00000F00;
        private const int joinMask = 0x0000F000;

        private readonly DeviceContext dc;

        //
        // Fields with default values
        //
        private WindowsBrush wndBrush;
        private WindowsPenStyle style;
        private readonly Color color;
        private readonly int width;

        private const int cosmeticPenWidth = 1;  // Cosmetic pen width.

#if GDI_FINALIZATION_WATCH
        private string AllocationSite = DbgUtil.StackTrace;
#endif

        public WindowsPen(DeviceContext dc) :
            this(dc, WindowsPenStyle.Default, cosmeticPenWidth, Color.Black)
        {
        }

        public WindowsPen(DeviceContext dc, Color color) :
            this(dc, WindowsPenStyle.Default, cosmeticPenWidth, color)
        {
        }

        public WindowsPen(DeviceContext dc, WindowsBrush windowsBrush) :
            this(dc, WindowsPenStyle.Default, cosmeticPenWidth, windowsBrush)
        {
        }

        public WindowsPen(DeviceContext dc, WindowsPenStyle style, int width, Color color)
        {
            this.style = style;
            this.width = width;
            this.color = color;
            this.dc = dc;

            // CreatePen() created on demand.
        }

        public WindowsPen(DeviceContext dc, WindowsPenStyle style, int width, WindowsBrush windowsBrush)
        {
            Debug.Assert(windowsBrush != null, "null windowsBrush");

            this.style = style;
            wndBrush = (WindowsBrush)windowsBrush.Clone();
            this.width = width;
            color = windowsBrush.Color;
            this.dc = dc;

            // CreatePen() created on demand.
        }

        private void CreatePen()
        {
            if (width > 1)    // Geometric pen.
            {
                // From MSDN: if width > 1, the style must be PS_NULL, PS_SOLID, or PS_INSIDEFRAME.
                style |= WindowsPenStyle.Geometric | WindowsPenStyle.Solid;
            }

            if (wndBrush == null)
            {
                nativeHandle = SafeNativeMethods.CreatePen((int)style, width, ColorTranslator.ToWin32(color));
            }
            else
            {
                NativeMethods.LOGBRUSH lb = new NativeMethods.LOGBRUSH
                {
                    lbColor = ColorTranslator.ToWin32(wndBrush.Color),
                    lbStyle = IntNativeMethods.BS_SOLID,
                    lbHatch = IntPtr.Zero
                };

                // Note: We currently don't support custom styles, that's why 0 and null for last two params.
                nativeHandle = SafeNativeMethods.ExtCreatePen((int)style, width, ref lb, 0, null);
            }
        }

        public object Clone()
        {
            return (wndBrush != null) ?
                new WindowsPen(dc, style, width, (WindowsBrush)wndBrush.Clone()) :
                new WindowsPen(dc, style, width, color);
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
            if (nativeHandle != IntPtr.Zero && dc != null)
            {
                DbgUtil.AssertFinalization(this, disposing);

                dc.DeleteObject(nativeHandle, GdiObjectType.Pen);
                nativeHandle = IntPtr.Zero;
            }

            if (wndBrush != null)
            {
                wndBrush.Dispose();
                wndBrush = null;
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
                if (nativeHandle == IntPtr.Zero)
                {
                    CreatePen();
                }

                return nativeHandle;
            }
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}: Style={1}, Color={2}, Width={3}, Brush={4}",
                GetType().Name,
                style,
                color,
                width,
                wndBrush != null ? wndBrush.ToString() : "null");
        }
    }
}
