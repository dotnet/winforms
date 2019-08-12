// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms.Design.Behavior;
using static Interop;

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  Contains designer utilities.
    /// </summary>
    internal static class DesignerUtils
    {
        private static Size s_minDragSize = Size.Empty;
        //brush used to draw a 'hover' state over a designer action glyph
        private static SolidBrush s_hoverBrush = new SolidBrush(Color.FromArgb(50, SystemColors.Highlight));
        //brush used to draw the resizeable selection borders around controls/components
        private static HatchBrush s_selectionBorderBrush = new HatchBrush(HatchStyle.Percent50, SystemColors.ControlDarkDark, Color.Transparent);
        //Pens and Brushes used via GDI to render our grabhandles
        private static IntPtr s_grabHandleFillBrushPrimary = SafeNativeMethods.CreateSolidBrush(ColorTranslator.ToWin32(SystemColors.Window));
        private static IntPtr s_grabHandleFillBrush = SafeNativeMethods.CreateSolidBrush(ColorTranslator.ToWin32(SystemColors.ControlText));
        private static IntPtr s_grabHandlePenPrimary = SafeNativeMethods.CreatePen(NativeMethods.PS_SOLID, 1, ColorTranslator.ToWin32(SystemColors.ControlText));
        private static IntPtr s_grabHandlePen = SafeNativeMethods.CreatePen(NativeMethods.PS_SOLID, 1, ColorTranslator.ToWin32(SystemColors.Window));

        //The box-like image used as the user is dragging comps from the toolbox
        private static Bitmap s_boxImage = null;
        public static int BOXIMAGESIZE = 16;

        // selection border size
        public static int SELECTIONBORDERSIZE = 1;
        // Although the selection border is only 1, we actually want a 3 pixel hittestarea
        public static int SELECTIONBORDERHITAREA = 3;

        // We want to make sure that the 1 pixel selectionborder is centered on the handles. The fact that the border is actually 3 pixels wide works like magic. If you draw a picture, then you will see why.
        //grabhandle size (diameter)
        public static int HANDLESIZE = 7;
        //how much should the grabhandle overlap the control
        public static int HANDLEOVERLAP = 2;
        //we want the selection border to be centered on a grabhandle, so how much do. we need to offset the border from the control to make that happen
        public static int SELECTIONBORDEROFFSET = ((HANDLESIZE - SELECTIONBORDERSIZE) / 2) - HANDLEOVERLAP;

        //no-resize handle size (diameter)
        public static int NORESIZEHANDLESIZE = 5;
        //we want the selection border to be centered on a grabhandle, so how much do
        //we need to offset the border from the control to make that happen
        public static int NORESIZEBORDEROFFSET = ((NORESIZEHANDLESIZE - SELECTIONBORDERSIZE) / 2);

        //lock handle height
        public static int LOCKHANDLEHEIGHT = 9;
        //total lock handle width
        public static int LOCKHANDLEWIDTH = 7;
        //how much should the lockhandle overlap the control
        public static int LOCKHANDLEOVERLAP = 2;
        //we want the selection border to be centered on the no-resize handle, so calculate how many pixels we need
        //to offset the selection border from the control -- since the handle is not square, we need one in each direction
        public static int LOCKEDSELECTIONBORDEROFFSET_Y = ((LOCKHANDLEHEIGHT - SELECTIONBORDERSIZE) / 2) - LOCKHANDLEOVERLAP;
        public static int LOCKEDSELECTIONBORDEROFFSET_X = ((LOCKHANDLEWIDTH - SELECTIONBORDERSIZE) / 2) - LOCKHANDLEOVERLAP;

        // upper rectangle size (diameter)
        public static int LOCKHANDLESIZE_UPPER = 5;
        // lower rectangle size
        public static int LOCKHANDLEHEIGHT_LOWER = 6;
        public static int LOCKHANDLEWIDTH_LOWER = 7;

        //Offset used when drawing the upper rect of a lock handle
        public static int LOCKHANDLEUPPER_OFFSET = (LOCKHANDLEWIDTH_LOWER - LOCKHANDLESIZE_UPPER) / 2;
        //Offset used when drawing the lower rect of a lock handle
        public static int LOCKHANDLELOWER_OFFSET = (LOCKHANDLEHEIGHT - LOCKHANDLEHEIGHT_LOWER);

        public static int CONTAINERGRABHANDLESIZE = 15;
        //delay for showing snaplines on keyboard movements
        public static int SNAPELINEDELAY = 1000;

        //min new row/col style size for the table layout panel
        public static int MINIMUMSTYLESIZE = 20;
        public static int MINIMUMSTYLEPERCENT = 50;

        //min width/height used to create bitmap to paint control into.
        public static int MINCONTROLBITMAPSIZE = 1;
        //min size for row/col style during a resize drag operation
        public static int MINUMUMSTYLESIZEDRAG = 8;
        //min # of rows/cols for the tablelayoutpanel when it is newly created
        public static int DEFAULTROWCOUNT = 2;
        public static int DEFAULTCOLUMNCOUNT = 2;

        //size of the col/row grab handle glyphs for teh table layout panel
        public static int RESIZEGLYPHSIZE = 4;

        //default value for Form padding if it has not been set in the designer (usability study request)
        public static int DEFAULTFORMPADDING = 9;

        //use these value to signify ANY of the right, top, left, center, or bottom alignments with the ContentAlignment enum.
        public static readonly ContentAlignment anyTopAlignment = ContentAlignment.TopLeft | ContentAlignment.TopCenter | ContentAlignment.TopRight;
        /* UNUSED: private static readonly ContentAlignment anyBottom = ContentAlignment.BottomLeft | ContentAlignment.BottomCenter | ContentAlignment.BottomRight;*/
        public static readonly ContentAlignment anyMiddleAlignment = ContentAlignment.MiddleLeft | ContentAlignment.MiddleCenter | ContentAlignment.MiddleRight;

        /// <summary>
        ///  Scale all hardcoded sizes if needed
        /// </summary>
        static DesignerUtils()
        {
            if (!DpiHelper.IsScalingRequired)
            {
                return;
            }

            BOXIMAGESIZE = DpiHelper.LogicalToDeviceUnitsX(BOXIMAGESIZE);
            SELECTIONBORDERSIZE = DpiHelper.LogicalToDeviceUnitsX(SELECTIONBORDERSIZE);
            SELECTIONBORDERHITAREA = DpiHelper.LogicalToDeviceUnitsX(SELECTIONBORDERHITAREA);
            HANDLESIZE = DpiHelper.LogicalToDeviceUnitsX(HANDLESIZE);
            HANDLEOVERLAP = DpiHelper.LogicalToDeviceUnitsX(HANDLEOVERLAP);
            NORESIZEHANDLESIZE = DpiHelper.LogicalToDeviceUnitsX(NORESIZEHANDLESIZE);
            LOCKHANDLEHEIGHT = DpiHelper.LogicalToDeviceUnitsX(LOCKHANDLEHEIGHT);
            LOCKHANDLEWIDTH = DpiHelper.LogicalToDeviceUnitsX(LOCKHANDLEWIDTH);
            LOCKHANDLEOVERLAP = DpiHelper.LogicalToDeviceUnitsX(LOCKHANDLEOVERLAP);
            LOCKHANDLESIZE_UPPER = DpiHelper.LogicalToDeviceUnitsX(LOCKHANDLESIZE_UPPER);
            LOCKHANDLEHEIGHT_LOWER = DpiHelper.LogicalToDeviceUnitsX(LOCKHANDLEHEIGHT_LOWER);
            LOCKHANDLEWIDTH_LOWER = DpiHelper.LogicalToDeviceUnitsX(LOCKHANDLEWIDTH_LOWER);
            CONTAINERGRABHANDLESIZE = DpiHelper.LogicalToDeviceUnitsX(CONTAINERGRABHANDLESIZE);
            RESIZEGLYPHSIZE = DpiHelper.LogicalToDeviceUnitsX(RESIZEGLYPHSIZE);

            SELECTIONBORDEROFFSET = ((HANDLESIZE - SELECTIONBORDERSIZE) / 2) - HANDLEOVERLAP;
            NORESIZEBORDEROFFSET = ((NORESIZEHANDLESIZE - SELECTIONBORDERSIZE) / 2);
            LOCKEDSELECTIONBORDEROFFSET_Y = ((LOCKHANDLEHEIGHT - SELECTIONBORDERSIZE) / 2) - LOCKHANDLEOVERLAP;
            LOCKEDSELECTIONBORDEROFFSET_X = ((LOCKHANDLEWIDTH - SELECTIONBORDERSIZE) / 2) - LOCKHANDLEOVERLAP;
            LOCKHANDLEUPPER_OFFSET = (LOCKHANDLEWIDTH_LOWER - LOCKHANDLESIZE_UPPER) / 2;
            LOCKHANDLELOWER_OFFSET = (LOCKHANDLEHEIGHT - LOCKHANDLEHEIGHT_LOWER);
        }

        /// <summary>
        ///  Used when the user clicks and drags a toolbox item onto the documentdesigner - this is the small box that is painted beneath the mouse pointer.
        /// </summary>
        public static Image BoxImage
        {
            get
            {
                if (s_boxImage == null)
                {
                    s_boxImage = new Bitmap(BOXIMAGESIZE, BOXIMAGESIZE, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                    using (Graphics g = Graphics.FromImage(s_boxImage))
                    {
                        g.FillRectangle(new SolidBrush(SystemColors.InactiveBorder), 0, 0, BOXIMAGESIZE, BOXIMAGESIZE);
                        g.DrawRectangle(new Pen(SystemColors.ControlDarkDark), 0, 0, BOXIMAGESIZE - 1, BOXIMAGESIZE - 1);
                    }
                }
                return s_boxImage;
            }
        }

        /// <summary>
        ///  Used by Designer action glyphs to render a 'mouse hover' state.
        /// </summary>
        public static Brush HoverBrush
        {
            get => s_hoverBrush;
        }

        /// <summary>
        ///  Demand created size used to determine how far the user needs to drag the mouse before a drag operation starts.
        /// </summary>
        public static Size MinDragSize
        {
            get
            {
                if (s_minDragSize == Size.Empty)
                {
                    Size minDrag = SystemInformation.DragSize;
                    Size minDblClick = SystemInformation.DoubleClickSize;
                    s_minDragSize.Width = Math.Max(minDrag.Width, minDblClick.Width);
                    s_minDragSize.Height = Math.Max(minDrag.Height, minDblClick.Height);
                }
                return s_minDragSize;
            }
        }

        public static Point LastCursorPoint
        {
            get
            {
                int lastXY = SafeNativeMethods.GetMessagePos();
                return new Point(NativeMethods.Util.SignedLOWORD(lastXY), NativeMethods.Util.SignedHIWORD(lastXY));
            }
        }

        // Recreate the brushes - behaviorservice calls this when the user preferences changes
        public static void SyncBrushes()
        {
            s_hoverBrush.Dispose();
            s_hoverBrush = new SolidBrush(Color.FromArgb(50, SystemColors.Highlight));

            s_selectionBorderBrush.Dispose();
            s_selectionBorderBrush = new HatchBrush(HatchStyle.Percent50, SystemColors.ControlDarkDark, Color.Transparent);

            Gdi32.DeleteObject(s_grabHandleFillBrushPrimary);
            s_grabHandleFillBrushPrimary = SafeNativeMethods.CreateSolidBrush(ColorTranslator.ToWin32(SystemColors.Window));

            Gdi32.DeleteObject(s_grabHandleFillBrush);
            s_grabHandleFillBrush = SafeNativeMethods.CreateSolidBrush(ColorTranslator.ToWin32(SystemColors.ControlText));

            Gdi32.DeleteObject(s_grabHandlePenPrimary);
            s_grabHandlePenPrimary = SafeNativeMethods.CreatePen(NativeMethods.PS_SOLID, 1, ColorTranslator.ToWin32(SystemColors.ControlText));

            Gdi32.DeleteObject(s_grabHandlePen);
            s_grabHandlePen = SafeNativeMethods.CreatePen(NativeMethods.PS_SOLID, 1, ColorTranslator.ToWin32(SystemColors.Window));
        }

        /// <summary>
        ///  Draws a ControlDarkDark border around the given image.
        /// </summary>
        private static void DrawDragBorder(Graphics g, Size imageSize, int borderSize, Color backColor)
        {
            Pen pen = SystemPens.ControlDarkDark;
            if (backColor != Color.Empty && backColor.GetBrightness() < .5)
            {
                pen = SystemPens.ControlLight;
            }

            //draw a border w/o the corners connecting
            g.DrawLine(pen, 1, 0, imageSize.Width - 2, 0);
            g.DrawLine(pen, 1, imageSize.Height - 1, imageSize.Width - 2, imageSize.Height - 1);
            g.DrawLine(pen, 0, 1, 0, imageSize.Height - 2);
            g.DrawLine(pen, imageSize.Width - 1, 1, imageSize.Width - 1, imageSize.Height - 2);

            //loop through drawing inner-rects until we get the proper thickness
            for (int i = 1; i < borderSize; i++)
            {
                g.DrawRectangle(pen, i, i, imageSize.Width - (2 + i), imageSize.Height - (2 + i));
            }
        }

        /// <summary>
        ///  Used for drawing the borders around controls that are being resized
        /// </summary>
        public static void DrawResizeBorder(Graphics g, Region resizeBorder, Color backColor)
        {
            Brush brush = SystemBrushes.ControlDarkDark;
            if (backColor != Color.Empty && backColor.GetBrightness() < .5)
            {
                brush = SystemBrushes.ControlLight;
            }

            g.FillRegion(brush, resizeBorder);
        }

        /// <summary>
        ///  Used for drawing the frame when doing a mouse drag
        /// </summary>
        public static void DrawFrame(Graphics g, Region resizeBorder, FrameStyle style, Color backColor)
        {
            Brush brush;
            Color color = SystemColors.ControlDarkDark;
            if (backColor != Color.Empty && backColor.GetBrightness() < .5)
            {
                color = SystemColors.ControlLight;
            }
            switch (style)
            {
                case FrameStyle.Dashed:
                    brush = new HatchBrush(HatchStyle.Percent50, color, Color.Transparent);
                    break;
                case FrameStyle.Thick:
                default:
                    brush = new SolidBrush(color);
                    break;
            }
            g.FillRegion(brush, resizeBorder);
            brush.Dispose();
        }

        /// <summary>
        ///  Used for drawing the grabhandles around sizeable selected controls and components.
        /// </summary>
        public static void DrawGrabHandle(Graphics graphics, Rectangle bounds, bool isPrimary, Glyph glyph)
        {
            IntPtr hDC = graphics.GetHdc();
            try
            {
                //set our pen and brush based on primary selection
                IntPtr oldBrush = Gdi32.SelectObject(hDC, isPrimary ? s_grabHandleFillBrushPrimary : s_grabHandleFillBrush);
                IntPtr oldPen = Gdi32.SelectObject(hDC, isPrimary ? s_grabHandlePenPrimary : s_grabHandlePen);

                //draw our rounded rect grabhandle
                SafeNativeMethods.RoundRect(new HandleRef(glyph, hDC), bounds.Left, bounds.Top, bounds.Right, bounds.Bottom, 2, 2);

                //restore old pen and brush
                Gdi32.SelectObject(hDC, oldBrush);
                Gdi32.SelectObject(hDC, oldPen);
            }
            finally
            {
                graphics.ReleaseHdcInternal(hDC);
            }
        }

        /// <summary>
        ///  Used for drawing the no-resize handle for non-resizeable selected controls and components.
        /// </summary>
        public static void DrawNoResizeHandle(Graphics graphics, Rectangle bounds, bool isPrimary, Glyph glyph)
        {
            IntPtr hDC = graphics.GetHdc();
            try
            {
                //set our pen and brush based on primary selection
                IntPtr oldBrush = Gdi32.SelectObject(hDC, isPrimary ? s_grabHandleFillBrushPrimary : s_grabHandleFillBrush);
                IntPtr oldPen = Gdi32.SelectObject(hDC, s_grabHandlePenPrimary);

                //draw our rect no-resize handle
                SafeNativeMethods.Rectangle(new HandleRef(glyph, hDC), bounds.Left, bounds.Top, bounds.Right, bounds.Bottom);
                //restore old pen and brush
                Gdi32.SelectObject(hDC, oldBrush);
                Gdi32.SelectObject(hDC, oldPen);
            }
            finally
            {
                graphics.ReleaseHdcInternal(hDC);
            }
        }

        /// <summary>
        ///  Used for drawing the lock handle for locked selected controls and components.
        /// </summary>
        public static void DrawLockedHandle(Graphics graphics, Rectangle bounds, bool isPrimary, Glyph glyph)
        {
            IntPtr hDC = graphics.GetHdc();
            try
            {
                IntPtr oldPen = Gdi32.SelectObject(hDC, s_grabHandlePenPrimary);
                // Upper rect - upper rect is always filled with the primary brush
                IntPtr oldBrush = Gdi32.SelectObject(hDC, s_grabHandleFillBrushPrimary);
                SafeNativeMethods.RoundRect(new HandleRef(glyph, hDC), bounds.Left + LOCKHANDLEUPPER_OFFSET, bounds.Top,
                                            bounds.Left + LOCKHANDLEUPPER_OFFSET + LOCKHANDLESIZE_UPPER, bounds.Top + LOCKHANDLESIZE_UPPER, 2, 2);
                // Lower rect - its fillbrush depends on the primary selection
                Gdi32.SelectObject(hDC, isPrimary ? s_grabHandleFillBrushPrimary : s_grabHandleFillBrush);
                SafeNativeMethods.Rectangle(new HandleRef(glyph, hDC), bounds.Left, bounds.Top + LOCKHANDLELOWER_OFFSET, bounds.Right, bounds.Bottom);

                //restore old pen and brush
                Gdi32.SelectObject(hDC, oldBrush);
                Gdi32.SelectObject(hDC, oldPen);
            }
            finally
            {
                graphics.ReleaseHdcInternal(hDC);
            }
        }

        /// <summary>
        ///  Uses the lockedBorderBrush to draw a 'locked' border on the given Graphics at the specified bounds.
        /// </summary>
        public static void DrawSelectionBorder(Graphics graphics, Rectangle bounds)
        {
            graphics.FillRectangle(s_selectionBorderBrush, bounds);
        }

        /// <summary>
        ///  Used to generate an image that represents the given control.  First, this method will call the 'GenerateSnapShotWithWM_PRINT' method on the control.  If we believe that this method did not return us a valid image (caused by some comctl/ax controls not properly responding to a wm_print) then we will attempt to do a bitblt of the control instead.
        /// </summary>
        public static void GenerateSnapShot(Control control, ref Image image, int borderSize, double opacity, Color backColor)
        {
            //GenerateSnapShot will return a boolean value indicating if the control returned an image or not...
            if (!GenerateSnapShotWithWM_PRINT(control, ref image))
            {
                //here, we failed to get the image on wmprint - so try bitblt
                GenerateSnapShotWithBitBlt(control, ref image);
                //if we still failed - we'll just fall though, put up a border around an empty area and call it good enough
            }
            //set the opacity
            if (opacity < 1.0 && opacity > 0.0)
            {
                // make this semi-transparent
                SetImageAlpha((Bitmap)image, opacity);
            }

            // draw a drag border around this thing
            if (borderSize > 0)
            {
                using (Graphics g = Graphics.FromImage(image))
                {
                    DrawDragBorder(g, image.Size, borderSize, backColor);
                }
            }
        }

        /// <summary>
        ///  Retrieves the width and height of a selection border grab handle. Designers may need this to properly position their user interfaces.
        /// </summary>
        public static Size GetAdornmentDimensions(AdornmentType adornmentType)
        {
            switch (adornmentType)
            {
                case AdornmentType.GrabHandle:
                    return new Size(HANDLESIZE, HANDLESIZE);
                case AdornmentType.ContainerSelector:
                case AdornmentType.Maximum:
                    return new Size(CONTAINERGRABHANDLESIZE, CONTAINERGRABHANDLESIZE);
            }
            return new Size(0, 0);
        }

        public static bool UseSnapLines(IServiceProvider provider)
        {
            bool useSnapLines = true;
            object optionValue = null;
            if (provider.GetService(typeof(DesignerOptionService)) is DesignerOptionService options)
            {
                PropertyDescriptor snaplinesProp = options.Options.Properties["UseSnapLines"];
                if (snaplinesProp != null)
                {
                    optionValue = snaplinesProp.GetValue(null);
                }
            }

            if (optionValue != null && optionValue is bool)
            {
                useSnapLines = (bool)optionValue;
            }
            return useSnapLines;
        }

        public static object GetOptionValue(IServiceProvider provider, string name)
        {
            object optionValue = null;
            if (provider != null)
            {
                if (provider.GetService(typeof(DesignerOptionService)) is DesignerOptionService desOpts)
                {
                    PropertyDescriptor prop = desOpts.Options.Properties[name];
                    if (prop != null)
                    {
                        optionValue = prop.GetValue(null);
                    }
                }
                else
                {
                    if (provider.GetService(typeof(IDesignerOptionService)) is IDesignerOptionService optSvc)
                    {
                        optionValue = optSvc.GetOptionValue("WindowsFormsDesigner\\General", name);
                    }
                }
            }
            return optionValue;
        }

        /// <summary>
        ///  Uses BitBlt to geta snapshot of the control
        /// </summary>
        public static void GenerateSnapShotWithBitBlt(Control control, ref Image image)
        {
            //get the DC's and create our image
            HandleRef hWnd = new HandleRef(control, control.Handle);
            IntPtr controlDC = User32.GetDC(hWnd);
            image = new Bitmap(Math.Max(control.Width, MINCONTROLBITMAPSIZE), Math.Max(control.Height, MINCONTROLBITMAPSIZE), PixelFormat.Format32bppPArgb);

            using (Graphics gDest = Graphics.FromImage(image))
            {
                if (control.BackColor == Color.Transparent)
                {
                    gDest.Clear(SystemColors.Control);
                }
                IntPtr destDC = gDest.GetHdc();
                //perform our bitblit operation to push the image into the dest bitmap
                SafeNativeMethods.BitBlt(destDC, 0, 0, image.Width, image.Height, controlDC, 0, 0, 0xcc0020/*RasterOp.SOURCE*/);
                //clean up all our handles and what not
                gDest.ReleaseHdc(destDC);
            }
        }

        /// <summary>
        ///  Uses WM_PRINT to get a snapshot of the control.  This method will return true if the control properly responded to the wm_print message.
        /// </summary>
        public static bool GenerateSnapShotWithWM_PRINT(Control control, ref Image image)
        {
            IntPtr hWnd = control.Handle;
            image = new Bitmap(Math.Max(control.Width, MINCONTROLBITMAPSIZE), Math.Max(control.Height, MINCONTROLBITMAPSIZE), System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

            //Have to do this BEFORE we set the testcolor.
            if (control.BackColor == Color.Transparent)
            {
                using (Graphics g = Graphics.FromImage(image))
                {
                    g.Clear(SystemColors.Control);
                }
            }

            // To validate that the control responded to the wm_print message, we pre-populate the bitmap with a colored center pixel.  We assume that the control _did not_ respond to wm_print if these center pixel is still this value
            Color testColor = Color.FromArgb(255, 252, 186, 238);
            ((Bitmap)image).SetPixel(image.Width / 2, image.Height / 2, testColor);
            using (Graphics g = Graphics.FromImage(image))
            {
                IntPtr hDc = g.GetHdc();
                //send the actual wm_print message
                NativeMethods.SendMessage(hWnd, WindowMessages.WM_PRINT, hDc, (IntPtr)(NativeMethods.PRF_CHILDREN | NativeMethods.PRF_CLIENT | NativeMethods.PRF_ERASEBKGND | NativeMethods.PRF_NONCLIENT));
                g.ReleaseHdc(hDc);
            }

            //now check to see if our center pixel was cleared, if not then our wm_print failed
            if (((Bitmap)image).GetPixel(image.Width / 2, image.Height / 2).Equals(testColor))
            {
                //wm_print failed
                return false;
            }
            return true;
        }

        /// <summary>
        ///  Used by the Glyphs and ComponentTray to determine the Top, Left, Right, Bottom and Body bound rects related to their original bounds and bordersize.
        /// </summary>
        public static Rectangle GetBoundsForSelectionType(Rectangle originalBounds, SelectionBorderGlyphType type, int borderSize)
        {
            Rectangle bounds = Rectangle.Empty;
            switch (type)
            {
                case SelectionBorderGlyphType.Top:
                    bounds = new Rectangle(originalBounds.Left - borderSize, originalBounds.Top - borderSize, originalBounds.Width + 2 * borderSize, borderSize);
                    break;
                case SelectionBorderGlyphType.Bottom:
                    bounds = new Rectangle(originalBounds.Left - borderSize, originalBounds.Bottom, originalBounds.Width + 2 * borderSize, borderSize);
                    break;
                case SelectionBorderGlyphType.Left:
                    bounds = new Rectangle(originalBounds.Left - borderSize, originalBounds.Top - borderSize, borderSize, originalBounds.Height + 2 * borderSize);
                    break;
                case SelectionBorderGlyphType.Right:
                    bounds = new Rectangle(originalBounds.Right, originalBounds.Top - borderSize, borderSize, originalBounds.Height + 2 * borderSize);
                    break;
                case SelectionBorderGlyphType.Body:
                    bounds = originalBounds;
                    break;
            }
            return bounds;
        }

        /// <summary>
        ///  Used by the Glyphs and ComponentTray to determine the Top, Left, Right, Bottom and Body bound rects related to their original bounds and bordersize.
        ///  Offset - how many pixels between the border glyph and the control
        /// </summary>
        private static Rectangle GetBoundsForSelectionType(Rectangle originalBounds, SelectionBorderGlyphType type, int bordersize, int offset)
        {
            Rectangle bounds = GetBoundsForSelectionType(originalBounds, type, bordersize);
            if (offset != 0)
            {
                switch (type)
                {
                    case SelectionBorderGlyphType.Top:
                        bounds.Offset(-offset, -offset);
                        bounds.Width += 2 * offset;
                        break;
                    case SelectionBorderGlyphType.Bottom:
                        bounds.Offset(-offset, offset);
                        bounds.Width += 2 * offset;
                        break;
                    case SelectionBorderGlyphType.Left:
                        bounds.Offset(-offset, -offset);
                        bounds.Height += 2 * offset;
                        break;
                    case SelectionBorderGlyphType.Right:
                        bounds.Offset(offset, -offset);
                        bounds.Height += 2 * offset;
                        break;
                    case SelectionBorderGlyphType.Body:
                        bounds = originalBounds;
                        break;
                }
            }
            return bounds;
        }

        /// <summary>
        ///  Used by the Glyphs and ComponentTray to determine the Top, Left, Right, Bottom and Body bound rects related to their original bounds and bordersize.
        /// </summary>
        public static Rectangle GetBoundsForSelectionType(Rectangle originalBounds, SelectionBorderGlyphType type)
        {
            return GetBoundsForSelectionType(originalBounds, type, DesignerUtils.SELECTIONBORDERSIZE, SELECTIONBORDEROFFSET);
        }

        public static Rectangle GetBoundsForNoResizeSelectionType(Rectangle originalBounds, SelectionBorderGlyphType type)
        {
            return GetBoundsForSelectionType(originalBounds, type, DesignerUtils.SELECTIONBORDERSIZE, NORESIZEBORDEROFFSET);
        }

        /// <summary>
        ///  Identifes where the text baseline for our control which should be based on bounds, padding, font, and textalignment.
        /// </summary>
        public static int GetTextBaseline(Control ctrl, ContentAlignment alignment)
        {
            //determine the actual client area we are working in (w/padding)
            Rectangle face = ctrl.ClientRectangle;

            //get the font metrics via gdi
            int fontAscent = 0;
            int fontHeight = 0;
            using (Graphics g = ctrl.CreateGraphics())
            {
                IntPtr dc = g.GetHdc();
                IntPtr hFont = ctrl.Font.ToHfont();
                IntPtr hFontOld;
                try
                {
                    hFontOld = Gdi32.SelectObject(dc, hFont);
                    NativeMethods.TEXTMETRIC metrics = new NativeMethods.TEXTMETRIC();
                    UnsafeNativeMethods.GetTextMetrics(new HandleRef(ctrl, dc), metrics);
                    //add the font ascent to the baseline
                    fontAscent = metrics.tmAscent + 1;
                    fontHeight = metrics.tmHeight;
                    Gdi32.SelectObject(dc, hFontOld);
                }
                finally
                {
                    Gdi32.DeleteObject(hFont);
                    g.ReleaseHdc(dc);
                }
            }

            //now add it all up
            if ((alignment & anyTopAlignment) != 0)
            {
                return face.Top + fontAscent;
            }
            else if ((alignment & anyMiddleAlignment) != 0)
            {
                return face.Top + (face.Height / 2) - (fontHeight / 2) + fontAscent;
            }
            else
            {
                return face.Bottom - fontHeight + fontAscent;
            }
        }

        /// <summary>
        ///  Called by the ParentControlDesigner when creating a new
        ///  control - this will update the new control's bounds with the
        ///  proper toolbox/snapline information that has been stored
        ///  off
        //
        ///  isMirrored - Is the ParentControlDesigner mirrored? If so, we need
        ///  to offset for that. This is because all snapline stuff is done
        ///  using a LTR coordinate system
        /// </summary>
        public static Rectangle GetBoundsFromToolboxSnapDragDropInfo(ToolboxSnapDragDropEventArgs e, Rectangle originalBounds, bool isMirrored)
        {
            Rectangle newBounds = originalBounds;

            //this should always be the case 'cause we don't
            //create 'e' unless we have an offset
            if (e.Offset != Point.Empty)
            {
                //snap either up or down depending on offset
                if ((e.SnapDirections & ToolboxSnapDragDropEventArgs.SnapDirection.Top) != 0)
                {
                    newBounds.Y += e.Offset.Y;//snap to top - so move up our bounds
                }
                else if ((e.SnapDirections & ToolboxSnapDragDropEventArgs.SnapDirection.Bottom) != 0)
                {
                    newBounds.Y = originalBounds.Y - originalBounds.Height + e.Offset.Y;
                }

                //snap either left or right depending on offset
                if (!isMirrored)
                {
                    if ((e.SnapDirections & ToolboxSnapDragDropEventArgs.SnapDirection.Left) != 0)
                    {
                        newBounds.X += e.Offset.X;//snap to left-
                    }
                    else if ((e.SnapDirections & ToolboxSnapDragDropEventArgs.SnapDirection.Right) != 0)
                    {
                        newBounds.X = originalBounds.X - originalBounds.Width + e.Offset.X;
                    }
                }
                else
                {
                    // ParentControlDesigner is RTL, that means that the origin is upper-right, not upper-left
                    if ((e.SnapDirections & ToolboxSnapDragDropEventArgs.SnapDirection.Left) != 0)
                    {
                        // e.Offset.X is negative when we snap to left
                        newBounds.X = originalBounds.X - originalBounds.Width - e.Offset.X;
                    }
                    else if ((e.SnapDirections & ToolboxSnapDragDropEventArgs.SnapDirection.Right) != 0)
                    {
                        // e.Offset.X is positive when we snao to right
                        newBounds.X -= e.Offset.X;
                    }
                }
            }

            return newBounds;
        }

        /// <summary>
        ///  Determine a unique site name for a component, starting from a base name. Return value should be passed into the Container.Add() method. If null is returned, this just means "let container generate a default name based on component type".
        /// </summary>
        public static string GetUniqueSiteName(IDesignerHost host, string name)
        {
            // Item has no explicit name, so let host generate a type-based name instead
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            // Get the name creation service from the designer host
            INameCreationService nameCreationService = (INameCreationService)host.GetService(typeof(INameCreationService));
            if (nameCreationService == null)
            {
                return null;
            }

            // See if desired name is already in use
            object existingComponent = host.Container.Components[name];
            if (existingComponent == null)
            {
                // Name is not in use - but make sure that it contains valid characters before using it!
                return nameCreationService.IsValidName(name) ? name : null;
            }
            else
            {
                // Name is in use (and therefore basically valid), so start appending numbers
                string nameN = name;
                for (int i = 1; !nameCreationService.IsValidName(nameN); ++i)
                {
                    nameN = name + i.ToString(CultureInfo.InvariantCulture);
                }
                return nameN;
            }
        }

        /// <summary>
        ///  Applies the given opacity to the image
        /// </summary>
        private static unsafe void SetImageAlpha(Bitmap b, double opacity)
        {
            if (opacity == 1.0)
            {
                return;
            }

            byte[] alphaValues = new byte[256];
            // precompute all the possible alpha values into an array so we don't do multiplications in the loop
            for (int i = 0; i < alphaValues.Length; i++)
            {
                alphaValues[i] = (byte)(i * opacity);
            }

            // lock the data in ARGB format.
            //
            BitmapData data = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            try
            {
                // compute the number of pixels that we're modifying.
                int pixels = data.Height * data.Width;
                int* pPixels = (int*)data.Scan0;

                // have the compiler figure out where to stop for us
                // by doing the pointer math
                byte* maxAddr = (byte*)(pPixels + pixels);

                // now run through the pixels only modifyng the A byte
                for (byte* addr = (byte*)(pPixels) + 3; addr < maxAddr; addr += 4)
                {
                    // the new value is just an index into our precomputed value array from above.
                    *addr = alphaValues[*addr];
                }
            }
            finally
            {
                // now, apply the data back to the bitmap.
                b.UnlockBits(data);
            }
        }

        /// <summary>
        ///  This method removes types that are generics from the input collection
        /// </summary>
        public static ICollection FilterGenericTypes(ICollection types)
        {
            if (types == null || types.Count == 0)
            {
                return types;
            }

            //now we get each Type and add it to the destination collection if its not a generic
            ArrayList final = new ArrayList(types.Count);
            foreach (Type t in types)
            {
                if (!t.ContainsGenericParameters)
                {
                    final.Add(t);
                }
            }
            return final;
        }

        /// <summary>
        ///  Checks the given container, substituting any nested container with its owning container. Ensures that a SplitterPanel in a SplitContainer returns the same container as other form components, since SplitContainer sites its two SplitterPanels inside a nested container.
        /// </summary>
        public static IContainer CheckForNestedContainer(IContainer container)
        {
            if (container is NestedContainer nestedContainer)
            {
                return nestedContainer.Owner.Site.Container;
            }
            else
            {
                return container;
            }
        }

        /// <summary>
        ///  Used to create copies of the objects that we are dragging in a drag operation
        /// </summary>
        public static ICollection CopyDragObjects(ICollection objects, IServiceProvider svcProvider)
        {
            if (objects == null || svcProvider == null)
            {
                Debug.Fail("Invalid parameter passed to DesignerUtils.CopyObjects.");
                return null;
            }

            Cursor oldCursor = Cursor.Current;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                ComponentSerializationService css = svcProvider.GetService(typeof(ComponentSerializationService)) as ComponentSerializationService;
                IDesignerHost host = svcProvider.GetService(typeof(IDesignerHost)) as IDesignerHost;
                Debug.Assert(css != null, "No component serialization service -- we cannot copy the objects");
                Debug.Assert(host != null, "No host -- we cannot copy the objects");
                if (css != null && host != null)
                {
                    SerializationStore store = null;
                    store = css.CreateStore();
                    // Get all the objects, meaning we want the children too
                    ICollection copyObjects = GetCopySelection(objects, host);

                    // The serialization service does not (yet) handle serializing collections
                    foreach (IComponent comp in copyObjects)
                    {
                        css.Serialize(store, comp);
                    }
                    store.Close();
                    copyObjects = css.Deserialize(store);

                    // Now, copyObjects contains a flattened list of all the controls contained in the original drag objects, that's not what we want to return. We only want to return the root drag objects, so that the caller gets an identical copy - identical in terms of objects.Count
                    ArrayList newObjects = new ArrayList(objects.Count);
                    foreach (IComponent comp in copyObjects)
                    {
                        Control c = comp as Control;
                        if (c != null && c.Parent == null)
                        {
                            newObjects.Add(comp);
                        }
                        else if (c == null)
                        { // this happens when we are dragging a toolstripitem
                            if (comp is ToolStripItem item && item.GetCurrentParent() == null)
                            {
                                newObjects.Add(comp);
                            }
                        }
                    }
                    Debug.Assert(newObjects.Count == objects.Count, "Why is the count of the copied objects not the same?");
                    return newObjects;
                }
            }
            finally
            {
                Cursor.Current = oldCursor;
            }
            return null;
        }

        private static ICollection GetCopySelection(ICollection objects, IDesignerHost host)
        {
            if (objects == null || host == null)
            {
                return null;
            }

            ArrayList copySelection = new ArrayList();
            foreach (IComponent comp in objects)
            {
                copySelection.Add(comp);
                GetAssociatedComponents(comp, host, copySelection);
            }
            return copySelection;
        }

        internal static void GetAssociatedComponents(IComponent component, IDesignerHost host, ArrayList list)
        {
            if (host == null)
            {
                return;
            }

            if (!(host.GetDesigner(component) is ComponentDesigner designer))
            {
                return;
            }

            foreach (IComponent childComp in designer.AssociatedComponents)
            {
                if (childComp.Site != null)
                {
                    list.Add(childComp);
                    GetAssociatedComponents(childComp, host, list);
                }
            }
        }

        private static int TreeView_GetExtendedStyle(IntPtr handle)
        {
            IntPtr ptr = NativeMethods.SendMessage(handle, NativeMethods.TVM_GETEXTENDEDSTYLE, IntPtr.Zero, IntPtr.Zero);
            return ptr.ToInt32();
        }

        private static void TreeView_SetExtendedStyle(IntPtr handle, int extendedStyle, int mask)
        {
            NativeMethods.SendMessage(handle, NativeMethods.TVM_SETEXTENDEDSTYLE, new IntPtr(mask), new IntPtr(extendedStyle));
        }

        /// <summary>
        ///  Modify a WinForms TreeView control to use the new Explorer style theme
        /// </summary>
        /// <param name="treeView">The tree view control to modify</param>
        public static void ApplyTreeViewThemeStyles(TreeView treeView)
        {
            if (treeView == null)
            {
                throw new ArgumentNullException(nameof(treeView));
            }
            treeView.HotTracking = true;
            treeView.ShowLines = false;
            IntPtr hwnd = treeView.Handle;
            SafeNativeMethods.SetWindowTheme(hwnd, "Explorer", null);
            int exstyle = TreeView_GetExtendedStyle(hwnd);
            exstyle |= NativeMethods.TVS_EX_DOUBLEBUFFER | NativeMethods.TVS_EX_FADEINOUTEXPANDOS;
            TreeView_SetExtendedStyle(hwnd, exstyle, 0);
        }

        private static void ListView_SetExtendedListViewStyleEx(IntPtr handle, int mask, int extendedStyle)
        {
            NativeMethods.SendMessage(handle, NativeMethods.LVM_SETEXTENDEDLISTVIEWSTYLE, new IntPtr(mask), new IntPtr(extendedStyle));
        }

        /// <summary>
        ///  Modify a WinForms ListView control to use the new Explorer style theme
        /// </summary>
        /// <param name="listView">The list view control to modify</param>
        public static void ApplyListViewThemeStyles(ListView listView)
        {
            if (listView == null)
            {
                throw new ArgumentNullException(nameof(listView));
            }
            IntPtr hwnd = listView.Handle;
            SafeNativeMethods.SetWindowTheme(hwnd, "Explorer", null);
            ListView_SetExtendedListViewStyleEx(hwnd, NativeMethods.LVS_EX_DOUBLEBUFFER, NativeMethods.LVS_EX_DOUBLEBUFFER);
        }
    }
}
