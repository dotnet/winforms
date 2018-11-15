// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// THIS PARTIAL CLASS CONTAINS THE BASE METHODS FOR DRAWING WITH A WINDOWSGRAPHICS.
// (Compiled in System.Windows.Forms but not in System.Drawing).

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
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    /// <include file='doc\WindowsGraphics.uex' path='docs/doc[@for="WindowsGraphics"]/*' />
    /// <devdoc>
    ///     See notes on WindowsGraphics.cs file.
    ///</devdoc>
#if WINFORMS_PUBLIC_GRAPHICS_LIBRARY
    public
#else
    internal
#endif
    sealed partial class WindowsGraphics : MarshalByRefObject, IDisposable, IDeviceContext
    {
        // Flag used by TextRenderer to clear the TextRenderer specific flags.
        public const int GdiUnsupportedFlagMask = (unchecked((int)0xFF000000));
        public static readonly Size MaxSize = new Size(Int32.MaxValue, Int32.MaxValue);

        // The value of the ItalicPaddingFactor comes from several tests using different fonts & drawing
        // flags and some benchmarking with GDI+.
        private const float ItalicPaddingFactor = 1/2f; 

        private TextPaddingOptions paddingFlags;

        /// <devdoc>
        ///    The padding options to be applied to the text bounding box internally.
        /// </devdoc>
        public TextPaddingOptions TextPadding
        {
            //Since Enum.IsDefined is only used within a Debug.Assert, it is okay to leave it
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1803:AvoidCostlyCallsWherePossible")]
            get
            {
                Debug.Assert( Enum.IsDefined(typeof(TextPaddingOptions), this.paddingFlags));
                return this.paddingFlags;
            }
            //Since Enum.IsDefined is only used within a Debug.Assert, it is okay to leave it
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1803:AvoidCostlyCallsWherePossible")]
            set
            {
                Debug.Assert( Enum.IsDefined(typeof(TextPaddingOptions), value));
                if( this.paddingFlags != value)
                {
                    this.paddingFlags = value;
                }
            }
        }

        /// Drawing methods.

        /// <devdoc>
        /// </devdoc>
        public void DrawPie(WindowsPen pen, Rectangle bounds, float startAngle, float sweepAngle) 
        {
            HandleRef hdc = new HandleRef( this.dc, this.dc.Hdc);

            if( pen != null )
            {
                // 1. Select the pen in the DC
                IntUnsafeNativeMethods.SelectObject(hdc, new HandleRef(pen, pen.HPen));
            }

            // 2. call the functions
            // we first draw a path that goes : 
            // from center of pie, draw arc (this draw the line to the beginning of the arc
            // then, draw the closing line.
            // paint the path with the pen
            int sideLength = Math.Min(bounds.Width, bounds.Height);
            Point p = new Point(bounds.X+sideLength/2, bounds.Y+sideLength/2);
            int radius = sideLength/2;
            IntUnsafeNativeMethods.BeginPath(hdc);
            IntUnsafeNativeMethods.MoveToEx(hdc, p.X, p.Y, null);
            IntUnsafeNativeMethods.AngleArc(hdc, p.X, p.Y, radius, startAngle, sweepAngle);
            IntUnsafeNativeMethods.LineTo(hdc, p.X, p.Y);
            IntUnsafeNativeMethods.EndPath(hdc);
            IntUnsafeNativeMethods.StrokePath(hdc);
        }

        private void DrawEllipse(WindowsPen pen, WindowsBrush brush,
            int nLeftRect,  // x-coord of upper-left corner of rectangle
            int nTopRect,   // y-coord of upper-left corner of rectangle
            int nRightRect, // x-coord of lower-right corner of rectangle
            int nBottomRect ) 
        { // y-coord of lower-right corner of rectangle
            HandleRef hdc = new HandleRef( this.dc, this.dc.Hdc);

            if (pen != null)
            {
                // 1. Select the pen in the DC
                IntUnsafeNativeMethods.SelectObject(hdc, new HandleRef(pen, pen.HPen));
            }

            if (brush != null)
            {
                IntUnsafeNativeMethods.SelectObject(hdc, new HandleRef(brush, brush.HBrush));
            }

            // 2. call the function
            IntUnsafeNativeMethods.Ellipse(hdc, nLeftRect, nTopRect, nRightRect, nBottomRect);
        }

        public void DrawAndFillEllipse(WindowsPen pen, WindowsBrush brush, Rectangle bounds) 
        {
            DrawEllipse(pen, brush, bounds.Left, bounds.Top, bounds.Right, bounds.Bottom);
        }


        /// Text rendering methods
        /// 

        /// <devdoc>
        ///     Draws the text at the specified point, using the given Font and foreColor.
        ///     CR/LF are honored.
        /// </devdoc>
        public void DrawText(string text, WindowsFont font, Point pt, Color foreColor)
        {
            DrawText(text, font, pt, foreColor, Color.Empty, IntTextFormatFlags.Default);
        }

        /// <devdoc>
        ///     Draws the text at the specified point, using the given Font, foreColor and backColor.
        ///     CR/LF are honored.
        /// </devdoc>
        public void DrawText(string text, WindowsFont font, Point pt, Color foreColor, Color backColor)
        {
            DrawText(text, font, pt, foreColor, backColor, IntTextFormatFlags.Default);
        }

        /// <devdoc>
        ///     Draws the text at the specified point, using the given Font and foreColor, and according to the 
        ///     specified flags.
        /// </devdoc>
        public void DrawText(string text, WindowsFont font, Point pt, Color foreColor, IntTextFormatFlags flags)
        {
            DrawText(text, font, pt, foreColor, Color.Empty, flags);
        }

        /// <devdoc>
        ///     Draws the text at the specified point, using the given Font, foreColor and backColor, and according 
        ///     to the specified flags.
        /// </devdoc>
        public void DrawText(string text, WindowsFont font, Point pt, Color foreColor, Color backColor, IntTextFormatFlags flags)
        {
            Rectangle bounds = new Rectangle( pt.X, pt.Y, Int32.MaxValue, Int32.MaxValue );
            DrawText( text, font, bounds, foreColor, backColor, flags );
        }

        /// <devdoc>
        ///     Draws the text centered in the given rectangle and using the given Font and foreColor.
        /// </devdoc>
        public void DrawText(string text, WindowsFont font, Rectangle bounds, Color foreColor)
        {
            DrawText(text, font, bounds, foreColor, Color.Empty);
        }

        /// <devdoc>
        ///     Draws the text centered in the given rectangle and using the given Font, foreColor and backColor.
        /// </devdoc>
        public void DrawText(string text, WindowsFont font, Rectangle bounds, Color foreColor, Color backColor)
        {
            DrawText(text, font, bounds, foreColor, backColor, IntTextFormatFlags.HorizontalCenter | IntTextFormatFlags.VerticalCenter);
        }

        /// <devdoc>
        ///     Draws the text in the given bounds, using the given Font and foreColor, and according to the specified flags.
        /// </devdoc>
        public void DrawText(string text, WindowsFont font, Rectangle bounds, Color color, IntTextFormatFlags flags)
        {
            DrawText( text, font, bounds, color, Color.Empty, flags );
        }

        /// <devdoc>
        ///     Draws the text in the given bounds, using the given Font, foreColor and backColor, and according to the specified
        ///     TextFormatFlags flags.
        ///     If font is null, the font currently selected in the hdc is used.
        ///     If foreColor and/or backColor are Color.Empty, the hdc current text and/or background color are used.
        /// </devdoc>
        public void DrawText(string text, WindowsFont font, Rectangle bounds, Color foreColor, Color backColor, IntTextFormatFlags flags)
        {
            if (string.IsNullOrEmpty(text) || foreColor == Color.Transparent) 
            {
                return;
            }

            Debug.Assert( ((uint)flags & GdiUnsupportedFlagMask) == 0, "Some custom flags were left over and are not GDI compliant!" );
            Debug.Assert( (flags & IntTextFormatFlags.CalculateRectangle) == 0, "CalculateRectangle flag is set, text won't be drawn" );

            HandleRef hdc = new HandleRef( this.dc, this.dc.Hdc);

            // DrawText requires default text alignment.
            if( this.dc.TextAlignment != DeviceContextTextAlignment.Default )
            {
                this.dc.SetTextAlignment(DeviceContextTextAlignment.Default );
            }

            // color empty means use the one currently selected in the dc.

            if( !foreColor.IsEmpty && foreColor != this.dc.TextColor)
            {
                this.dc.SetTextColor(foreColor);
            }

            if (font != null)
            {
                this.dc.SelectFont(font);
            }

            DeviceContextBackgroundMode newBackGndMode = (backColor.IsEmpty || backColor == Color.Transparent) ? 
                DeviceContextBackgroundMode.Transparent : 
                DeviceContextBackgroundMode.Opaque;

            if( this.dc.BackgroundMode != newBackGndMode ) 
            {
                this.dc.SetBackgroundMode( newBackGndMode );
            }

            if( newBackGndMode != DeviceContextBackgroundMode.Transparent && backColor != this.dc.BackgroundColor )
            {
                this.dc.SetBackgroundColor( backColor );
            }

            IntNativeMethods.DRAWTEXTPARAMS dtparams = GetTextMargins(font);

            bounds = AdjustForVerticalAlignment(hdc, text, bounds, flags, dtparams);

            // Adjust unbounded rect to avoid overflow since Rectangle ctr does not do param validation.
            if( bounds.Width == MaxSize.Width )
            {
                bounds.Width = bounds.Width - bounds.X;
            }
            if( bounds.Height == MaxSize.Height )
            {
                bounds.Height = bounds.Height - bounds.Y;
            }

            IntNativeMethods.RECT rect = new IntNativeMethods.RECT(bounds);

            IntUnsafeNativeMethods.DrawTextEx(hdc, text, ref rect, (int) flags, dtparams);
            

            /* No need to restore previous objects into the dc (see comments on top of the class).
             *             
            if (hOldFont != IntPtr.Zero) 
            {
                IntUnsafeNativeMethods.SelectObject(hdc, new HandleRef( null, hOldFont));
            }

            if( foreColor != textColor ) 
            {
                this.dc.SetTextColor(textColor);
            }

            if( backColor != bkColor )
            {
                this.dc.SetBackgroundColor(bkColor);
            }
        
            if( bckMode != newMode ) 
            {
                this.dc.SetBackgroundMode(bckMode);
            }

            if( align != DeviceContextTextAlignment.Default )
            {
                // Default text alignment required by DrewText.
                this.dc.SetTextAlignment(align);
            }
            */
        }

        /// <devdoc>
        /// </devdoc>
        public Color GetNearestColor(Color color) 
        {
            HandleRef hdc = new HandleRef(null, this.dc.Hdc);
            int colorResult = IntUnsafeNativeMethods.GetNearestColor(hdc, ColorTranslator.ToWin32(color));
            return ColorTranslator.FromWin32(colorResult);
        }

        /// <devdoc>
        ///    <para>
        ///       Calculates the spacing required for drawing text w/o clipping parts of a glyph.
        ///    </para>
        /// </devdoc>
        public float GetOverhangPadding( WindowsFont font )
        {
            // Some parts of a glyphs may be clipped depending on the font & font style, GDI+ adds 1/6 of tmHeight
            // to each size of the text bounding box when drawing text to account for that; we do it here as well.

            WindowsFont tmpfont = font;
                
            if( tmpfont == null) 
            {
                tmpfont = this.dc.Font;
            }

            float overhangPadding = tmpfont.Height / 6f;

            if( tmpfont != font )
            {
                tmpfont.Dispose();
            }

            return overhangPadding;
        }

        /// <devdoc>
        ///     Get the bounding box internal text padding to be used when drawing text.
        /// </devdoc>
        public IntNativeMethods.DRAWTEXTPARAMS GetTextMargins(WindowsFont font)
        {
            // DrawText(Ex) adds a small space at the beginning of the text bounding box but not at the end,
            // this is more noticeable when the font has the italic style.  We compensate with this factor.

            int leftMargin = 0;
            int rightMargin = 0;
            float overhangPadding = 0;

            switch( this.TextPadding )
            {
                case TextPaddingOptions.GlyphOverhangPadding:
                    // [overhang padding][Text][overhang padding][italic padding]
                    overhangPadding = GetOverhangPadding(font);
                    leftMargin = (int) Math.Ceiling(overhangPadding);
                    rightMargin = (int) Math.Ceiling(overhangPadding * (1 + ItalicPaddingFactor));
                    break;

                case TextPaddingOptions.LeftAndRightPadding:
                    // [2 * overhang padding][Text][2 * overhang padding][italic padding]
                    overhangPadding = GetOverhangPadding(font);
                    leftMargin = (int) Math.Ceiling(2 * overhangPadding);
                    rightMargin = (int) Math.Ceiling(overhangPadding * (2 + ItalicPaddingFactor));
                    break;

                case TextPaddingOptions.NoPadding:
                default:
                    break;

            }

            return new IntNativeMethods.DRAWTEXTPARAMS(leftMargin, rightMargin);
        }

        /// <devdoc>
        ///    <para>
        ///       Returns the Size of the given text using the specified font if not null, otherwise the font currently 
        ///       set in the dc is used.
        ///       This method is used to get the size in points of a line of text; it uses GetTextExtentPoint32 function 
        ///       which computes the width and height of the text ignoring TAB\CR\LF characters. 
        ///       A text extent is the distance between the beginning of the space and a character that will fit in the space.
        ///    </para>
        /// </devdoc>
        public Size GetTextExtent(string text, WindowsFont font)
        {
            if (string.IsNullOrEmpty(text))
            {
                return Size.Empty;
            }

            IntNativeMethods.SIZE size = new IntNativeMethods.SIZE();

            HandleRef hdc = new HandleRef(null, this.dc.Hdc);

            if (font != null)
            {
                this.dc.SelectFont(font);
            }

            IntUnsafeNativeMethods.GetTextExtentPoint32(hdc, text, size);

            // Unselect, but not from Measurement DC as it keeps the same
            // font selected for perf reasons.
            if (font != null && !MeasurementDCInfo.IsMeasurementDC(this.dc)) {
                this.dc.ResetFont();
            }

            return new Size(size.cx, size.cy);
        }

        /// <devdoc>
        ///     Returns the Size in logical units of the given text using the given Font.
        ///     CR/LF/TAB are taken into account.
        /// </devdoc>
        public Size MeasureText(string text, WindowsFont font)
        {
            return MeasureText(text, font, MaxSize, IntTextFormatFlags.Default);
        }

        /// <devdoc>
        ///     Returns the Size in logical units of the given text using the given Font and using the specified rectangle 
        ///     as the text bounding box (see overload below for more info).
        ///     TAB/CR/LF are taken into account.
        /// </devdoc>
        public Size MeasureText(string text, WindowsFont font, Size proposedSize)
        {
            return MeasureText( text, font, proposedSize, IntTextFormatFlags.Default );
        }

        /// <devdoc>
        ///     Returns the Size in logical units of the given text using the given Font, and according to the formatting flags.
        ///     The proposed size is used to create a bounding rectangle as follows:
        ///     - If there are multiple lines of text, DrawText uses the width of the rectangle pointed to by 
        ///       the lpRect parameter and extends the base of the rectangle to bound the last line of text. 
        ///     - If the largest word is wider than the rectangle, the width is expanded. 
        ///     - If the text is less than the width of the rectangle, the width is reduced. 
        ///     - If there is only one line of text, DrawText modifies the right side of the rectangle so that 
        ///       it bounds the last character in the line.
        ///     If the font is null, the hdc's current font will be used.
        ///
        ///     Note for vertical fonts (if ever supported): DrawTextEx uses GetTextExtentPoint32 for measuring the text and this 
        ///     function has the following limitation (from MSDN):
        ///     - This function assumes that the text is horizontal, that is, that the escapement is always 0. This is true for both 
        ///       the horizontal and vertical measurements of the text.  The application must convert it explicitly.
        /// </devdoc>

        
        public Size MeasureText(string text, WindowsFont font, Size proposedSize, IntTextFormatFlags flags)
        {     
            Debug.Assert( ((uint)flags & GdiUnsupportedFlagMask) == 0, "Some custom flags were left over and are not GDI compliant!" );
           

           
            if (string.IsNullOrEmpty(text)) 
            {
                return Size.Empty;
            }

            //
            // DrawText returns a rectangle useful for aligning, but not guaranteed to encompass all
            // pixels (its not a FitBlackBox, if the text is italicized, it will overhang on the right.)
            // So we need to account for this.
            //
            IntNativeMethods.DRAWTEXTPARAMS dtparams = null;

#if OPTIMIZED_MEASUREMENTDC       
            // use the cache if we've got it
            if (MeasurementDCInfo.IsMeasurementDC(this.DeviceContext)) 
            {
                dtparams = MeasurementDCInfo.GetTextMargins(this,font);
            }
#endif

            if (dtparams == null) 
            {
                dtparams = GetTextMargins(font);
            }

            //
            // If Width / Height are < 0, we need to make them larger or DrawText will return
            // an unbounded measurement when we actually trying to make it very narrow.
            //

            int minWidth = 1 + dtparams.iLeftMargin + dtparams.iRightMargin;

            if( proposedSize.Width <= minWidth ) {
                proposedSize.Width = minWidth;
            }
            if( proposedSize.Height <= 0 ) {
                proposedSize.Height = 1;
            }

            IntNativeMethods.RECT rect = IntNativeMethods.RECT.FromXYWH(0, 0, proposedSize.Width, proposedSize.Height);

            HandleRef hdc = new HandleRef(null, this.dc.Hdc);

            if (font != null)
            {
                this.dc.SelectFont(font);
            }

            // If proposedSize.Height >= MaxSize.Height it is assumed bounds needed.  If flags contain SingleLine and 
            // VerticalCenter or Bottom options, DrawTextEx does not bind the rectangle to the actual text height since 
            // it assumes the text is to be vertically aligned; we need to clear the VerticalCenter and Bottom flags to 
            // get the actual text bounds.
            if (proposedSize.Height >= MaxSize.Height && (flags & IntTextFormatFlags.SingleLine) != 0)
            {
                // Clear vertical-alignment flags.
                flags &= ~(IntTextFormatFlags.Bottom | IntTextFormatFlags.VerticalCenter);
            }

            if (proposedSize.Width == MaxSize.Width) 
            {
               // PERF: No constraining width means no word break.
               // in this case, we dont care about word wrapping - there should be enough room to fit it all
               flags &= ~(IntTextFormatFlags.WordBreak); 
            }

            flags |= IntTextFormatFlags.CalculateRectangle;
            IntUnsafeNativeMethods.DrawTextEx(hdc, text, ref rect, (int)flags, dtparams);

            /* No need to restore previous objects into the dc (see comments on top of the class).
             * 
            if( hOldFont != IntPtr.Zero )
            {
                this.dc.SelectObject(hOldFont);
            }
            */
         
            return rect.Size;
        }

        /// <devdoc>
        ///    <para>
        ///      The GDI DrawText does not do multiline alignment when IntTextFormatFlags.SingleLine is not set. This
        ///      adjustment is to workaround that limitation. We don't want to duplicate SelectObject calls here, 
        ///      so put your Font in the dc before calling this.
        ///
        ///      AdjustForVerticalAlignment is only used when the text is multiline and it fits inside the bounds passed in.
        ///      In that case we want the horizontal center of the multiline text to be at the horizontal center of the bounds.
        ///
        ///      If the text is multiline and it does not fit inside the bounds passed in, then return the bounds that were passed in.
        ///      This way we paint the top of the text at the top of the bounds passed in.
        ///    </para>
        /// </devdoc>
        public static Rectangle AdjustForVerticalAlignment(HandleRef hdc, string text, Rectangle bounds, IntTextFormatFlags flags, IntNativeMethods.DRAWTEXTPARAMS dtparams)
        {
            Debug.Assert( ((uint)flags & GdiUnsupportedFlagMask) == 0, "Some custom flags were left over and are not GDI compliant!" );

            // Ok if any Top (Cannot test IntTextFormatFlags.Top because it is 0), single line text or measuring text.
            bool isTop = (flags & IntTextFormatFlags.Bottom) == 0 && (flags & IntTextFormatFlags.VerticalCenter) == 0;
            if( isTop ||((flags & IntTextFormatFlags.SingleLine) != 0) || ((flags & IntTextFormatFlags.CalculateRectangle) != 0) )
            {
                return bounds;  
            }

            IntNativeMethods.RECT rect = new IntNativeMethods.RECT(bounds);

            // Get the text bounds.
            flags |= IntTextFormatFlags.CalculateRectangle;
            int textHeight = IntUnsafeNativeMethods.DrawTextEx(hdc, text, ref rect, (int) flags, dtparams);

            // if the text does not fit inside the bounds then return the bounds that were passed in
            if (textHeight > bounds.Height)
            {
                return bounds;
            }

            Rectangle adjustedBounds = bounds;

            if( (flags & IntTextFormatFlags.VerticalCenter) != 0 )  // Middle
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

        /// <include file='doc\WindowsGraphics.uex' path='docs/doc[@for="WindowsGraphics.DrawRectangle"]/*' />
        public void DrawRectangle(WindowsPen pen, Rectangle rect) 
        { 
            DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
        }

        /// <include file='doc\WindowsGraphics.uex' path='docs/doc[@for="WindowsGraphics.DrawRectangle3"]/*' />
        public void DrawRectangle(WindowsPen pen, int x, int y, int width, int height) 
        { 
            Debug.Assert( pen != null, "pen == null" );

            HandleRef hdc = new HandleRef(this.dc, this.dc.Hdc);

            if( pen != null )
            {
                this.dc.SelectObject(pen.HPen, GdiObjectType.Pen);
            }

            DeviceContextBinaryRasterOperationFlags rasterOp = this.dc.BinaryRasterOperation;

            if( rasterOp != DeviceContextBinaryRasterOperationFlags.CopyPen )
            {
                rasterOp = this.dc.SetRasterOperation(DeviceContextBinaryRasterOperationFlags.CopyPen); 
            }

            IntUnsafeNativeMethods.SelectObject(hdc, new HandleRef(null, IntUnsafeNativeMethods.GetStockObject(IntNativeMethods.HOLLOW_BRUSH)));
            // Add 1 to widht and height to create the 'bounding box' (convert from point to size).
            IntUnsafeNativeMethods.Rectangle(hdc, x, y, x + width , y + height );
            
            if( rasterOp != DeviceContextBinaryRasterOperationFlags.CopyPen )
            {
                this.dc.SetRasterOperation(rasterOp); 
            }
        }

        // FillRectangle overloads

        /// <include file='doc\WindowsGraphics.uex' path='docs/doc[@for="WindowsGraphics.FillRectangle"]/*' />
        public void FillRectangle(WindowsBrush brush, Rectangle rect)
        {
            FillRectangle(brush, rect.X, rect.Y, rect.Width, rect.Height);
        }

        /// <include file='doc\WindowsGraphics.uex' path='docs/doc[@for="WindowsGraphics.FillRectangle3"]/*' />
        public void FillRectangle(WindowsBrush brush, int x, int y, int width, int height)
        {
            Debug.Assert( brush != null, "brush == null" );

            HandleRef hdc  = new HandleRef(this.dc, this.dc.Hdc);
            IntPtr hBrush  = brush.HBrush;  // We don't delete this handle since we didn't create it.   
            IntNativeMethods.RECT rect = new IntNativeMethods.RECT(x, y, x + width, y + height );

#if WINFORMS_PUBLIC_GRAPHICS_LIBRARY
            if (brush is WindowsHatchBrush)
            { 
                int clr = ColorTranslator.ToWin32(((WindowsHatchBrush)brush).BackGroundColor);
                IntUnsafeNativeMethods.SetBkColor(hdc, clr );
                IntUnsafeNativeMethods.SetBkMode(hdc, (int)DeviceContextBackgroundMode.Transparent);
            }
#endif
            IntUnsafeNativeMethods.FillRect(hdc, ref rect, new HandleRef(brush, hBrush));
        }


        // DrawLine overloads

        /// <include file='doc\WindowsGraphics.uex' path='docs/doc[@for="WindowsGraphics.DrawLine"]/*' />
        /// <devdoc>
        ///     Draws a line starting from p1 (included) to p2 (excluded).  LineTo doesn't paint the last 
        ///     pixel because if it did the intersection points of connected lines would be drawn multiple 
        ///     times turning them back to the background color.
        /// </devdoc>
        public void DrawLine(WindowsPen pen, Point p1, Point p2) 
        {
            DrawLine(pen, p1.X, p1.Y, p2.X, p2.Y);
        }

        /// <include file='doc\WindowsGraphics.uex' path='docs/doc[@for="WindowsGraphics.DrawLine3"]/*' />
        public void DrawLine(WindowsPen pen, int x1, int y1, int x2, int y2)
        {
            HandleRef hdc  = new HandleRef(this.dc, this.dc.Hdc);
            
            DeviceContextBinaryRasterOperationFlags rasterOp = this.dc.BinaryRasterOperation;
            DeviceContextBackgroundMode bckMode = this.dc.BackgroundMode;

            if( rasterOp != DeviceContextBinaryRasterOperationFlags.CopyPen )
            {
                rasterOp = this.dc.SetRasterOperation( DeviceContextBinaryRasterOperationFlags.CopyPen );
            }

            if( bckMode != DeviceContextBackgroundMode.Transparent )
            {
                bckMode = this.dc.SetBackgroundMode( DeviceContextBackgroundMode.Transparent );
            }

            if (pen != null)
            {
                this.dc.SelectObject(pen.HPen, GdiObjectType.Pen);
            }

            IntNativeMethods.POINT oldPoint = new IntNativeMethods.POINT();

            IntUnsafeNativeMethods.MoveToEx(hdc, x1, y1, oldPoint);
            IntUnsafeNativeMethods.LineTo(hdc, x2, y2);

            if( bckMode != DeviceContextBackgroundMode.Transparent )
            {
                this.dc.SetBackgroundMode( bckMode );
            }

            if( rasterOp != DeviceContextBinaryRasterOperationFlags.CopyPen )
            {
                this.dc.SetRasterOperation( rasterOp );
            }
            
            IntUnsafeNativeMethods.MoveToEx(hdc, oldPoint.x, oldPoint.y, null);
        }

        /// <devdoc>
        ///     Returns a TEXTMETRIC structure for the font selected in the device context 
        ///     represented by this object, in units of pixels.
        /// </devdoc>
        public IntNativeMethods.TEXTMETRIC GetTextMetrics()
        {
            IntNativeMethods.TEXTMETRIC tm  = new IntNativeMethods.TEXTMETRIC();
            HandleRef                   hdc = new HandleRef( this.dc, this.dc.Hdc );

            // Set the mapping mode to MM_TEXT so we deal with units of pixels.
            DeviceContextMapMode mapMode = dc.MapMode;

            bool setupDC = mapMode != DeviceContextMapMode.Text;

            if( setupDC )
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
