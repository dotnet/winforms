// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms.Internal
{
    /// <summary>
    ///  See notes on WindowsGraphics.cs file.
    /// </summary>
    internal sealed partial class WindowsGraphics : MarshalByRefObject, IDisposable, IDeviceContext
    {
        // Flag used by TextRenderer to clear the TextRenderer specific flags.
        public const int GdiUnsupportedFlagMask = (unchecked((int)0xFF000000));
        public static readonly Size MaxSize = new Size(int.MaxValue, int.MaxValue);

        // The value of the ItalicPaddingFactor comes from several tests using different fonts & drawing
        // flags and some benchmarking with GDI+.
        private const float ItalicPaddingFactor = 1 / 2f;

        private TextPaddingOptions paddingFlags;

        /// <summary>
        ///  The padding options to be applied to the text bounding box internally.
        /// </summary>
        public TextPaddingOptions TextPadding
        {
            get
            {
                Debug.Assert(Enum.IsDefined(typeof(TextPaddingOptions), paddingFlags));
                return paddingFlags;
            }
            set
            {
                Debug.Assert(Enum.IsDefined(typeof(TextPaddingOptions), value));
                if (paddingFlags != value)
                {
                    paddingFlags = value;
                }
            }
        }

        ///  Drawing methods.

        public unsafe void DrawPie(WindowsPen pen, Rectangle bounds, float startAngle, float sweepAngle)
        {
            HandleRef hdc = new HandleRef(dc, dc.Hdc);

            if (pen != null)
            {
                // 1. Select the pen in the DC
                Gdi32.SelectObject(hdc, new HandleRef(pen, pen.HPen));
            }

            // 2. call the functions
            // we first draw a path that goes :
            // from center of pie, draw arc (this draw the line to the beginning of the arc
            // then, draw the closing line.
            // paint the path with the pen
            int sideLength = Math.Min(bounds.Width, bounds.Height);
            Point p = new Point(bounds.X + sideLength / 2, bounds.Y + sideLength / 2);
            int radius = sideLength / 2;
            IntUnsafeNativeMethods.BeginPath(hdc);
            Point oldPoint = default;
            IntUnsafeNativeMethods.MoveToEx(hdc, p.X, p.Y, &oldPoint);
            IntUnsafeNativeMethods.AngleArc(hdc, p.X, p.Y, radius, startAngle, sweepAngle);
            IntUnsafeNativeMethods.LineTo(hdc, p.X, p.Y);
            IntUnsafeNativeMethods.EndPath(hdc);
            IntUnsafeNativeMethods.StrokePath(hdc);
        }

        private void DrawEllipse(WindowsPen pen, WindowsBrush brush,
            int nLeftRect,  // x-coord of upper-left corner of rectangle
            int nTopRect,   // y-coord of upper-left corner of rectangle
            int nRightRect, // x-coord of lower-right corner of rectangle
            int nBottomRect)
        {
            // y-coord of lower-right corner of rectangle
            HandleRef hdc = new HandleRef(dc, dc.Hdc);

            if (pen != null)
            {
                // 1. Select the pen in the DC
                Gdi32.SelectObject(hdc, new HandleRef(pen, pen.HPen));
            }

            if (brush != null)
            {
                Gdi32.SelectObject(hdc, new HandleRef(brush, brush.HBrush));
            }

            // 2. call the function
            IntUnsafeNativeMethods.Ellipse(hdc, nLeftRect, nTopRect, nRightRect, nBottomRect);
        }

        public void DrawAndFillEllipse(WindowsPen pen, WindowsBrush brush, Rectangle bounds)
        {
            DrawEllipse(pen, brush, bounds.Left, bounds.Top, bounds.Right, bounds.Bottom);
        }

        ///  Text rendering methods
        ///
        /// <summary>
        ///  Draws the text at the specified point, using the given Font and foreColor.
        ///  CR/LF are honored.
        /// </summary>
        public void DrawText(string text, WindowsFont font, Point pt, Color foreColor)
        {
            DrawText(text, font, pt, foreColor, Color.Empty, User32.TextFormatFlags.DT_DEFAULT);
        }

        /// <summary>
        ///  Draws the text at the specified point, using the given Font, foreColor and backColor.
        ///  CR/LF are honored.
        /// </summary>
        public void DrawText(string text, WindowsFont font, Point pt, Color foreColor, Color backColor)
        {
            DrawText(text, font, pt, foreColor, backColor, User32.TextFormatFlags.DT_DEFAULT);
        }

        /// <summary>
        ///  Draws the text at the specified point, using the given Font and foreColor, and according to the
        ///  specified flags.
        /// </summary>
        public void DrawText(string text, WindowsFont font, Point pt, Color foreColor, User32.TextFormatFlags flags)
        {
            DrawText(text, font, pt, foreColor, Color.Empty, flags);
        }

        /// <summary>
        ///  Draws the text at the specified point, using the given Font, foreColor and backColor, and according
        ///  to the specified flags.
        /// </summary>
        public void DrawText(string text, WindowsFont font, Point pt, Color foreColor, Color backColor, User32.TextFormatFlags flags)
        {
            Rectangle bounds = new Rectangle(pt.X, pt.Y, int.MaxValue, int.MaxValue);
            DrawText(text, font, bounds, foreColor, backColor, flags);
        }

        /// <summary>
        ///  Draws the text centered in the given rectangle and using the given Font and foreColor.
        /// </summary>
        public void DrawText(string text, WindowsFont font, Rectangle bounds, Color foreColor)
        {
            DrawText(text, font, bounds, foreColor, Color.Empty);
        }

        /// <summary>
        ///  Draws the text centered in the given rectangle and using the given Font, foreColor and backColor.
        /// </summary>
        public void DrawText(string text, WindowsFont font, Rectangle bounds, Color foreColor, Color backColor)
        {
            DrawText(text, font, bounds, foreColor, backColor, User32.TextFormatFlags.DT_CENTER | User32.TextFormatFlags.DT_VCENTER);
        }

        /// <summary>
        ///  Draws the text in the given bounds, using the given Font and foreColor, and according to the specified flags.
        /// </summary>
        public void DrawText(string text, WindowsFont font, Rectangle bounds, Color color, User32.TextFormatFlags flags)
        {
            DrawText(text, font, bounds, color, Color.Empty, flags);
        }

        /// <summary>
        ///  Draws the text in the given bounds, using the given Font, foreColor and backColor, and according to the specified
        ///  TextFormatFlags flags.
        ///  If font is null, the font currently selected in the hdc is used.
        ///  If foreColor and/or backColor are Color.Empty, the hdc current text and/or background color are used.
        /// </summary>
        public void DrawText(string text, WindowsFont font, Rectangle bounds, Color foreColor, Color backColor, User32.TextFormatFlags flags)
        {
            if (string.IsNullOrEmpty(text) || foreColor == Color.Transparent)
            {
                return;
            }

            Debug.Assert(((uint)flags & GdiUnsupportedFlagMask) == 0, "Some custom flags were left over and are not GDI compliant!");
            Debug.Assert((flags & User32.TextFormatFlags.DT_CALCRECT) == 0, "DT_CALCRECT flag is set, text won't be drawn");

            HandleRef hdc = new HandleRef(dc, dc.Hdc);

            // DrawText requires default text alignment.
            if (dc.TextAlignment != DeviceContextTextAlignment.Default)
            {
                dc.SetTextAlignment(DeviceContextTextAlignment.Default);
            }

            // color empty means use the one currently selected in the dc.

            if (!foreColor.IsEmpty && foreColor != dc.TextColor)
            {
                dc.SetTextColor(foreColor);
            }

            if (font != null)
            {
                dc.SelectFont(font);
            }

            DeviceContextBackgroundMode newBackGndMode = (backColor.IsEmpty || backColor == Color.Transparent) ?
                DeviceContextBackgroundMode.Transparent :
                DeviceContextBackgroundMode.Opaque;

            if (dc.BackgroundMode != newBackGndMode)
            {
                dc.SetBackgroundMode(newBackGndMode);
            }

            if (newBackGndMode != DeviceContextBackgroundMode.Transparent && backColor != dc.BackgroundColor)
            {
                dc.SetBackgroundColor(backColor);
            }

            User32.DRAWTEXTPARAMS dtparams = GetTextMargins(font);

            bounds = AdjustForVerticalAlignment(hdc, text, bounds, flags, ref dtparams);

            // Adjust unbounded rect to avoid overflow since Rectangle ctr does not do param validation.
            if (bounds.Width == MaxSize.Width)
            {
                bounds.Width -= bounds.X;
            }
            if (bounds.Height == MaxSize.Height)
            {
                bounds.Height -= bounds.Y;
            }

            var rect = new RECT(bounds);
            User32.DrawTextExW(hdc, text, text.Length, ref rect, flags, ref dtparams);

            // No need to restore previous objects into the dc (see comments on top of the class).
        }

        public Color GetNearestColor(Color color)
        {
            HandleRef hdc = new HandleRef(null, dc.Hdc);
            int colorResult = IntUnsafeNativeMethods.GetNearestColor(hdc, ColorTranslator.ToWin32(color));
            return ColorTranslator.FromWin32(colorResult);
        }

        /// <summary>
        ///  Calculates the spacing required for drawing text w/o clipping parts of a glyph.
        /// </summary>
        public float GetOverhangPadding(WindowsFont font)
        {
            // Some parts of a glyphs may be clipped depending on the font & font style, GDI+ adds 1/6 of tmHeight
            // to each size of the text bounding box when drawing text to account for that; we do it here as well.

            WindowsFont tmpfont = font;

            if (tmpfont == null)
            {
                tmpfont = dc.Font;
            }

            float overhangPadding = tmpfont.Height / 6f;

            if (tmpfont != font)
            {
                tmpfont.Dispose();
            }

            return overhangPadding;
        }

        /// <summary>
        ///  Get the bounding box internal text padding to be used when drawing text.
        /// </summary>
        public User32.DRAWTEXTPARAMS GetTextMargins(WindowsFont font)
        {
            // DrawText(Ex) adds a small space at the beginning of the text bounding box but not at the end,
            // this is more noticeable when the font has the italic style.  We compensate with this factor.

            int leftMargin = 0;
            int rightMargin = 0;
            float overhangPadding = 0;

            switch (TextPadding)
            {
                case TextPaddingOptions.GlyphOverhangPadding:
                    // [overhang padding][Text][overhang padding][italic padding]
                    overhangPadding = GetOverhangPadding(font);
                    leftMargin = (int)Math.Ceiling(overhangPadding);
                    rightMargin = (int)Math.Ceiling(overhangPadding * (1 + ItalicPaddingFactor));
                    break;

                case TextPaddingOptions.LeftAndRightPadding:
                    // [2 * overhang padding][Text][2 * overhang padding][italic padding]
                    overhangPadding = GetOverhangPadding(font);
                    leftMargin = (int)Math.Ceiling(2 * overhangPadding);
                    rightMargin = (int)Math.Ceiling(overhangPadding * (2 + ItalicPaddingFactor));
                    break;

                case TextPaddingOptions.NoPadding:
                default:
                    break;

            }

            return new User32.DRAWTEXTPARAMS
            {
                iLeftMargin = leftMargin,
                iRightMargin = rightMargin
            };
        }

        /// <summary>
        ///  Returns the Size of the given text using the specified font if not null, otherwise the font currently
        ///  set in the dc is used.
        ///  This method is used to get the size in points of a line of text; it uses GetTextExtentPoint32 function
        ///  which computes the width and height of the text ignoring TAB\CR\LF characters.
        ///  A text extent is the distance between the beginning of the space and a character that will fit in the space.
        /// </summary>
        public Size GetTextExtent(string text, WindowsFont font)
        {
            if (string.IsNullOrEmpty(text))
            {
                return Size.Empty;
            }

            Size size = new Size();
            if (font != null)
            {
                dc.SelectFont(font);
            }

            Gdi32.GetTextExtentPoint32W(dc.Hdc, text, text.Length, ref size);

            // Unselect, but not from Measurement DC as it keeps the same
            // font selected for perf reasons.
            if (font != null && !MeasurementDCInfo.IsMeasurementDC(dc))
            {
                dc.ResetFont();
            }

            return new Size(size.Width, size.Height);
        }

        /// <summary>
        ///  Returns the Size in logical units of the given text using the given Font.
        ///  CR/LF/TAB are taken into account.
        /// </summary>
        public Size MeasureText(string text, WindowsFont font)
        {
            return MeasureText(text, font, MaxSize, User32.TextFormatFlags.DT_BOTTOM);
        }

        /// <summary>
        ///  Returns the Size in logical units of the given text using the given Font and using the specified rectangle
        ///  as the text bounding box (see overload below for more info).
        ///  TAB/CR/LF are taken into account.
        /// </summary>
        public Size MeasureText(string text, WindowsFont font, Size proposedSize)
        {
            return MeasureText(text, font, proposedSize, User32.TextFormatFlags.DT_BOTTOM);
        }

        /// <summary>
        ///  Returns the Size in logical units of the given text using the given Font, and according to the formatting flags.
        ///  The proposed size is used to create a bounding rectangle as follows:
        ///  - If there are multiple lines of text, DrawText uses the width of the rectangle pointed to by
        ///  the lpRect parameter and extends the base of the rectangle to bound the last line of text.
        ///  - If the largest word is wider than the rectangle, the width is expanded.
        ///  - If the text is less than the width of the rectangle, the width is reduced.
        ///  - If there is only one line of text, DrawText modifies the right side of the rectangle so that
        ///  it bounds the last character in the line.
        ///  If the font is null, the hdc's current font will be used.
        ///
        ///  Note for vertical fonts (if ever supported): DrawTextEx uses GetTextExtentPoint32 for measuring the text and this
        ///  function has the following limitation (from MSDN):
        ///  - This function assumes that the text is horizontal, that is, that the escapement is always 0. This is true for both
        ///  the horizontal and vertical measurements of the text.  The application must convert it explicitly.
        /// </summary>
        public Size MeasureText(string text, WindowsFont font, Size proposedSize, User32.TextFormatFlags flags)
        {
            Debug.Assert(((uint)flags & GdiUnsupportedFlagMask) == 0, "Some custom flags were left over and are not GDI compliant!");

            if (string.IsNullOrEmpty(text))
            {
                return Size.Empty;
            }

            // DrawText returns a rectangle useful for aligning, but not guaranteed to encompass all
            // pixels (its not a FitBlackBox, if the text is italicized, it will overhang on the right.)
            // So we need to account for this.
#if OPTIMIZED_MEASUREMENTDC
            User32.DRAWTEXTPARAMS dtparams;
            // use the cache if we've got it
            if (MeasurementDCInfo.IsMeasurementDC(DeviceContext))
            {
                dtparams = MeasurementDCInfo.GetTextMargins(this, font);
            }
            else
            {
                dtparams = GetTextMargins(font);
            }
#else

            User32.DRAWTEXTPARAMS dtparams = GetTextMargins(font);
#endif

            // If Width / Height are < 0, we need to make them larger or DrawText will return
            // an unbounded measurement when we actually trying to make it very narrow.
            int minWidth = 1 + dtparams.iLeftMargin + dtparams.iRightMargin;

            if (proposedSize.Width <= minWidth)
            {
                proposedSize.Width = minWidth;
            }
            if (proposedSize.Height <= 0)
            {
                proposedSize.Height = 1;
            }

            var rect = new RECT(0, 0, proposedSize.Width, proposedSize.Height);
            if (font != null)
            {
                dc.SelectFont(font);
            }

            // If proposedSize.Height >= MaxSize.Height it is assumed bounds needed.  If flags contain DT_SINGLELINE and
            // DT_VCENTER or DT_BOTTOM options, DrawTextEx does not bind the rectangle to the actual text height since
            // it assumes the text is to be vertically aligned; we need to clear the DT_VCENTER and DT_BOTTOM flags to
            // get the actual text bounds.
            if (proposedSize.Height >= MaxSize.Height && (flags & User32.TextFormatFlags.DT_SINGLELINE) != 0)
            {
                // Clear vertical-alignment flags.
                flags &= ~(User32.TextFormatFlags.DT_BOTTOM | User32.TextFormatFlags.DT_VCENTER);
            }

            if (proposedSize.Width == MaxSize.Width)
            {
                // PERF: No constraining width means no word break.
                // in this case, we dont care about word wrapping - there should be enough room to fit it all
                flags &= ~(User32.TextFormatFlags.DT_WORDBREAK);
            }

            flags |= User32.TextFormatFlags.DT_CALCRECT;
            User32.DrawTextExW(dc.Hdc, text, text.Length, ref rect, flags, ref dtparams);

            return rect.Size;
        }

        /// <summary>
        ///  The GDI DrawText does not do multiline alignment when User32.TextFormatFlags.DT_SINGLELINE is not set. This
        ///  adjustment is to workaround that limitation. We don't want to duplicate SelectObject calls here,
        ///  so put your Font in the dc before calling this.
        ///
        ///  AdjustForVerticalAlignment is only used when the text is multiline and it fits inside the bounds passed in.
        ///  In that case we want the horizontal center of the multiline text to be at the horizontal center of the bounds.
        ///
        ///  If the text is multiline and it does not fit inside the bounds passed in, then return the bounds that were passed in.
        ///  This way we paint the top of the text at the top of the bounds passed in.
        /// </summary>
        public static Rectangle AdjustForVerticalAlignment(HandleRef hdc, string text, Rectangle bounds, User32.TextFormatFlags flags, ref User32.DRAWTEXTPARAMS dtparams)
        {
            Debug.Assert(((uint)flags & GdiUnsupportedFlagMask) == 0, "Some custom flags were left over and are not GDI compliant!");

            // Ok if any Top (Cannot test User32.TextFormatFlags.Top because it is 0), single line text or measuring text.
            bool isTop = (flags & User32.TextFormatFlags.DT_BOTTOM) == 0 && (flags & User32.TextFormatFlags.DT_VCENTER) == 0;
            if (isTop || ((flags & User32.TextFormatFlags.DT_SINGLELINE) != 0) || ((flags & User32.TextFormatFlags.DT_CALCRECT) != 0))
            {
                return bounds;
            }

            RECT rect = new RECT(bounds);

            // Get the text bounds.
            flags |= User32.TextFormatFlags.DT_CALCRECT;
            int textHeight = User32.DrawTextExW(hdc, text, text.Length, ref rect, flags, ref dtparams);

            // if the text does not fit inside the bounds then return the bounds that were passed in
            if (textHeight > bounds.Height)
            {
                return bounds;
            }

            Rectangle adjustedBounds = bounds;

            if ((flags & User32.TextFormatFlags.DT_VCENTER) != 0)  // Middle
            {
                adjustedBounds.Y = adjustedBounds.Top + adjustedBounds.Height / 2 - textHeight / 2;
            }
            else // Bottom.
            {
                adjustedBounds.Y = adjustedBounds.Bottom - textHeight;
            }

            return adjustedBounds;
        }

        // DrawRectangle overloads

        public void DrawRectangle(WindowsPen pen, Rectangle rect)
        {
            DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
        }

        public void DrawRectangle(WindowsPen pen, int x, int y, int width, int height)
        {
            Debug.Assert(pen != null, "pen == null");

            HandleRef hdc = new HandleRef(dc, dc.Hdc);

            if (pen != null)
            {
                dc.SelectObject(pen.HPen, GdiObjectType.Pen);
            }

            DeviceContextBinaryRasterOperationFlags rasterOp = dc.BinaryRasterOperation;

            if (rasterOp != DeviceContextBinaryRasterOperationFlags.CopyPen)
            {
                rasterOp = dc.SetRasterOperation(DeviceContextBinaryRasterOperationFlags.CopyPen);
            }

            Gdi32.SelectObject(hdc, Gdi32.GetStockObject(Gdi32.StockObject.HOLLOW_BRUSH));
            // Add 1 to widht and height to create the 'bounding box' (convert from point to size).
            IntUnsafeNativeMethods.Rectangle(hdc, x, y, x + width, y + height);

            if (rasterOp != DeviceContextBinaryRasterOperationFlags.CopyPen)
            {
                dc.SetRasterOperation(rasterOp);
            }
        }

        // FillRectangle overloads

        public void FillRectangle(WindowsBrush brush, Rectangle rect)
        {
            FillRectangle(brush, rect.X, rect.Y, rect.Width, rect.Height);
        }

        public void FillRectangle(WindowsBrush brush, int x, int y, int width, int height)
        {
            Debug.Assert(brush != null, "brush == null");

            HandleRef hdc = new HandleRef(dc, dc.Hdc);
            IntPtr hBrush = brush.HBrush;  // We don't delete this handle since we didn't create it.
            RECT rect = new RECT(x, y, x + width, y + height);

            IntUnsafeNativeMethods.FillRect(hdc, ref rect, new HandleRef(brush, hBrush));
        }

        // DrawLine overloads

        /// <summary>
        ///  Draws a line starting from p1 (included) to p2 (excluded).  LineTo doesn't paint the last
        ///  pixel because if it did the intersection points of connected lines would be drawn multiple
        ///  times turning them back to the background color.
        /// </summary>
        public void DrawLine(WindowsPen pen, Point p1, Point p2)
        {
            DrawLine(pen, p1.X, p1.Y, p2.X, p2.Y);
        }

        public unsafe void DrawLine(WindowsPen pen, int x1, int y1, int x2, int y2)
        {
            HandleRef hdc = new HandleRef(dc, dc.Hdc);

            DeviceContextBinaryRasterOperationFlags rasterOp = dc.BinaryRasterOperation;
            DeviceContextBackgroundMode bckMode = dc.BackgroundMode;

            if (rasterOp != DeviceContextBinaryRasterOperationFlags.CopyPen)
            {
                rasterOp = dc.SetRasterOperation(DeviceContextBinaryRasterOperationFlags.CopyPen);
            }

            if (bckMode != DeviceContextBackgroundMode.Transparent)
            {
                bckMode = dc.SetBackgroundMode(DeviceContextBackgroundMode.Transparent);
            }

            if (pen != null)
            {
                dc.SelectObject(pen.HPen, GdiObjectType.Pen);
            }

            Point oldPoint = new Point();

            IntUnsafeNativeMethods.MoveToEx(hdc, x1, y1, &oldPoint);
            IntUnsafeNativeMethods.LineTo(hdc, x2, y2);

            if (bckMode != DeviceContextBackgroundMode.Transparent)
            {
                dc.SetBackgroundMode(bckMode);
            }

            if (rasterOp != DeviceContextBinaryRasterOperationFlags.CopyPen)
            {
                dc.SetRasterOperation(rasterOp);
            }

            IntUnsafeNativeMethods.MoveToEx(hdc, oldPoint.X, oldPoint.Y, &oldPoint);
        }

        /// <summary>
        ///  Returns a TEXTMETRIC structure for the font selected in the device context
        ///  represented by this object, in units of pixels.
        /// </summary>
        public IntNativeMethods.TEXTMETRIC GetTextMetrics()
        {
            IntNativeMethods.TEXTMETRIC tm = new IntNativeMethods.TEXTMETRIC();
            HandleRef hdc = new HandleRef(dc, dc.Hdc);

            // Set the mapping mode to MM_TEXT so we deal with units of pixels.
            DeviceContextMapMode mapMode = dc.MapMode;

            bool setupDC = mapMode != DeviceContextMapMode.Text;

            if (setupDC)
            {
                // Changing the MapMode will affect viewport and window extent and origin, we save the dc
                // state so all those properties can be properly restored once done.
                dc.SaveHdc();
            }

            try
            {
                if (setupDC)
                {
                    mapMode = dc.SetMapMode(DeviceContextMapMode.Text);
                }

                IntUnsafeNativeMethods.GetTextMetrics(hdc, ref tm);
            }
            finally
            {
                if (setupDC)
                {
                    dc.RestoreHdc();
                }
            }

            return tm;
        }
    }
}
