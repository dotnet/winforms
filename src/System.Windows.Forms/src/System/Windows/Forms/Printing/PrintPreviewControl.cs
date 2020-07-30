// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  The raw "preview" part of print previewing, without any dialogs or buttons.
    ///  Most PrintPreviewControl's are found on PrintPreviewDialog's,
    ///  but they don't have to be.
    /// </summary>
    [DefaultProperty(nameof(Document))]
    [SRDescription(nameof(SR.DescriptionPrintPreviewControl))]
    public class PrintPreviewControl : Control
    {
        Size virtualSize = new Size(1, 1);
        Point position = new Point(0, 0);
        Point lastOffset;
        bool antiAlias;

        private const int SCROLL_PAGE = 100;
        private const int SCROLL_LINE = 5;
        private const double DefaultZoom = .3;

        private const int border = 10; // spacing per page, in mm

        private PrintDocument document;
        private PreviewPageInfo[] pageInfo; // null if needs refreshing
        private int startPage;  // 0-based
        private int rows = 1;
        private int columns = 1;
        private bool autoZoom = true;

        // The following are all computed by ComputeLayout
        private bool layoutOk;
        private Size imageSize = System.Drawing.Size.Empty; // 100ths of inch, not pixels
        private Point screendpi = Point.Empty;
        private double zoom = DefaultZoom;
        bool pageInfoCalcPending;
        bool exceptionPrinting;

        /// <summary>
        ///  Initializes a new instance of the <see cref='PrintPreviewControl'/> class.
        /// </summary>
        public PrintPreviewControl()
        {
            ResetBackColor();
            ResetForeColor();
            Size = new Size(100, 100);
            SetStyle(ControlStyles.ResizeRedraw, false);
            SetStyle(ControlStyles.Opaque | ControlStyles.OptimizedDoubleBuffer, true);
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(false)]
        [SRDescription(nameof(SR.PrintPreviewAntiAliasDescr))]
        public bool UseAntiAlias
        {
            get
            {
                return antiAlias;
            }
            set
            {
                antiAlias = value;
            }
        }

        /// <summary>
        ///  Gets or sets a value If true (the default), resizing the control or changing the number of pages shown
        ///  will automatically adjust Zoom to make everything visible.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(true)]
        [SRDescription(nameof(SR.PrintPreviewAutoZoomDescr))]
        public bool AutoZoom
        {
            get { return autoZoom; }
            set
            {
                if (autoZoom != value)
                {
                    autoZoom = value;
                    InvalidateLayout();
                }
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating the document to preview.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(null)]
        [SRDescription(nameof(SR.PrintPreviewDocumentDescr))]
        public PrintDocument Document
        {
            get { return document; }
            set
            {
                document = value;
                InvalidatePreview();
            }
        }

        /// <summary>
        ///  Gets or sets the number of pages
        ///  displayed horizontally across the screen.
        /// </summary>
        [DefaultValue(1)]
        [SRCategory(nameof(SR.CatLayout))]
        [SRDescription(nameof(SR.PrintPreviewColumnsDescr))]
        public int Columns
        {
            get { return columns; }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidLowBoundArgumentEx, nameof(Columns), value, 1));
                }

                columns = value;
                InvalidateLayout();
            }
        }

        /// <summary>
        ///  Gets the CreateParams used to create the window.
        ///  If a subclass overrides this function, it must call the base implementation.
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style |= (int)User32.WS.HSCROLL;
                cp.Style |= (int)User32.WS.VSCROLL;
                return cp;
            }
        }

        /// <summary>
        ///  The virtual coordinate of the upper left visible pixel.
        /// </summary>
        [SRCategory(nameof(SR.CatLayout))]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRDescription(nameof(SR.ControlWithScrollbarsPositionDescr))]
        private Point Position
        {
            get { return position; }
            set
            {
                SetPositionNoInvalidate(value);
            }
        }

        /// <summary>
        ///  Gets or sets the number of pages
        ///  displayed vertically down the screen.
        /// </summary>
        [DefaultValue(1)]
        [SRDescription(nameof(SR.PrintPreviewRowsDescr))]
        [SRCategory(nameof(SR.CatBehavior))]
        public int Rows
        {
            get { return rows; }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidLowBoundArgumentEx, nameof(Rows), value, 1));
                }

                rows = value;
                InvalidateLayout();
            }
        }

        /// <summary>
        ///  This is used for international applications where the language
        ///  is written from RightToLeft. When this property is true,
        ///  control placement and text will be from right to left.
        /// </summary>
        [SRCategory(nameof(SR.CatAppearance))]
        [Localizable(true)]
        [AmbientValue(RightToLeft.Inherit)]
        [SRDescription(nameof(SR.ControlRightToLeftDescr))]
        public override RightToLeft RightToLeft
        {
            get => base.RightToLeft;
            set
            {
                base.RightToLeft = value;
                InvalidatePreview();
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Bindable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override string Text
        {
            get => base.Text;
            set => base.Text = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler TextChanged
        {
            add => base.TextChanged += value;
            remove => base.TextChanged -= value;
        }

        /// <summary>
        ///  Gets or sets the page number of the upper left page.
        /// </summary>
        [DefaultValue(0)]
        [SRDescription(nameof(SR.PrintPreviewStartPageDescr))]
        [SRCategory(nameof(SR.CatBehavior))]
        public int StartPage
        {
            get
            {
                int value = startPage;
                if (pageInfo != null)
                {
                    value = Math.Min(value, pageInfo.Length - (rows * columns));
                }
                value = Math.Max(value, 0);

                return value;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidLowBoundArgumentEx, nameof(StartPage), value, 0));
                }
                int oldValue = StartPage;
                startPage = value;
                if (oldValue != startPage)
                {
                    InvalidateLayout();
                    OnStartPageChanged(EventArgs.Empty);
                }
            }
        }

        private static readonly object EVENT_STARTPAGECHANGED = new object();

        [SRCategory(nameof(SR.CatPropertyChanged))]
        [SRDescription(nameof(SR.RadioButtonOnStartPageChangedDescr))]
        public event EventHandler StartPageChanged
        {
            add => Events.AddHandler(EVENT_STARTPAGECHANGED, value);
            remove => Events.RemoveHandler(EVENT_STARTPAGECHANGED, value);
        }

        /// <summary>
        ///  How big the control would be if the screen was infinitely large.
        /// </summary>
        [SRCategory(nameof(SR.CatLayout))]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRDescription(nameof(SR.ControlWithScrollbarsVirtualSizeDescr))]
        private Size VirtualSize
        {
            get { return virtualSize; }
            set
            {
                SetVirtualSizeNoInvalidate(value);
                Invalidate();
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating how large the pages will appear.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.PrintPreviewZoomDescr))]
        [DefaultValue(DefaultZoom)]
        public double Zoom
        {
            get { return zoom; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException(SR.PrintPreviewControlZoomNegative);
                }

                autoZoom = false;
                zoom = value;
                InvalidateLayout();
            }
        }

        private int AdjustScroll(Message m, int pos, int maxPos, bool horizontal)
        {
            switch ((User32.SBH)PARAM.LOWORD(m.WParam))
            {
                case User32.SBH.THUMBPOSITION:
                case User32.SBH.THUMBTRACK:
                    var si = new User32.SCROLLINFO
                    {
                        cbSize = (uint)Marshal.SizeOf<User32.SCROLLINFO>(),
                        fMask = User32.SIF.TRACKPOS
                    };
                    User32.SB direction = horizontal ? User32.SB.HORZ : User32.SB.VERT;
                    if (User32.GetScrollInfo(m.HWnd, direction, ref si).IsTrue())
                    {
                        pos = si.nTrackPos;
                    }
                    else
                    {
                        pos = PARAM.HIWORD(m.WParam);
                    }
                    break;
                case User32.SBH.LINELEFT:
                    if (pos > SCROLL_LINE)
                    {
                        pos -= SCROLL_LINE;
                    }
                    else
                    {
                        pos = 0;
                    }
                    break;
                case User32.SBH.LINERIGHT:
                    if (pos < maxPos - SCROLL_LINE)
                    {
                        pos += SCROLL_LINE;
                    }
                    else
                    {
                        pos = maxPos;
                    }
                    break;
                case User32.SBH.PAGELEFT:
                    if (pos > SCROLL_PAGE)
                    {
                        pos -= SCROLL_PAGE;
                    }
                    else
                    {
                        pos = 0;
                    }
                    break;
                case User32.SBH.PAGERIGHT:
                    if (pos < maxPos - SCROLL_PAGE)
                    {
                        pos += SCROLL_PAGE;
                    }
                    else
                    {
                        pos = maxPos;
                    }
                    break;
            }
            return pos;
        }

        // This function computes everything in terms of physical size (millimeters), not pixels
        private void ComputeLayout()
        {
            Debug.Assert(pageInfo != null, "Must call ComputePreview first");
            layoutOk = true;
            if (pageInfo.Length == 0)
            {
                ClientSize = Size;
                return;
            }

            using var hdc = new User32.GetDcScope(Handle);
            screendpi = new Point(Gdi32.GetDeviceCaps(hdc, Gdi32.DeviceCapability.LOGPIXELSX),
                                  Gdi32.GetDeviceCaps(hdc, Gdi32.DeviceCapability.LOGPIXELSY));

            Size pageSize = pageInfo[StartPage].PhysicalSize;
            Size controlPhysicalSize = new Size(PixelsToPhysical(new Point(Size), screendpi));

            if (autoZoom)
            {
                double zoomX = ((double)controlPhysicalSize.Width - border * (columns + 1)) / (columns * pageSize.Width);
                double zoomY = ((double)controlPhysicalSize.Height - border * (rows + 1)) / (rows * pageSize.Height);
                zoom = Math.Min(zoomX, zoomY);
            }

            imageSize = new Size((int)(zoom * pageSize.Width), (int)(zoom * pageSize.Height));
            int virtualX = (imageSize.Width * columns) + border * (columns + 1);
            int virtualY = (imageSize.Height * rows) + border * (rows + 1);
            SetVirtualSizeNoInvalidate(new Size(PhysicalToPixels(new Point(virtualX, virtualY), screendpi)));
        }

        // "Prints" the document to memory
        private void ComputePreview()
        {
            int oldStart = StartPage;

            if (document is null)
            {
                pageInfo = Array.Empty<PreviewPageInfo>();
            }
            else
            {
                PrintController oldController = document.PrintController;
                PreviewPrintController previewController = new PreviewPrintController
                {
                    UseAntiAlias = UseAntiAlias
                };
                document.PrintController = new PrintControllerWithStatusDialog(previewController,
                                                                               SR.PrintControllerWithStatusDialog_DialogTitlePreview);

                document.Print();
                pageInfo = previewController.GetPreviewPageInfo();
                Debug.Assert(pageInfo != null, "ReviewPrintController did not give us preview info");

                document.PrintController = oldController;
            }

            if (oldStart != StartPage)
            {
                OnStartPageChanged(EventArgs.Empty);
            }
        }

        // Recomputes the sizes and positions of pages without forcing a new "preview print"
        private void InvalidateLayout()
        {
            layoutOk = false;
            Invalidate();
        }

        /// <summary>
        ///  Refreshes the preview of the document.
        /// </summary>
        public void InvalidatePreview()
        {
            pageInfo = null;
            InvalidateLayout();
        }

        /// <summary>
        ///  Invalidate the layout, if necessary.
        /// </summary>
        protected override void OnResize(EventArgs eventargs)
        {
            InvalidateLayout();
            base.OnResize(eventargs);
        }

        void CalculatePageInfo()
        {
            if (pageInfoCalcPending)
            {
                return;
            }

            pageInfoCalcPending = true;
            try
            {
                if (pageInfo is null)
                {
                    try
                    {
                        ComputePreview();
                    }
                    catch
                    {
                        exceptionPrinting = true;
                        throw;
                    }
                    finally
                    {
                        Invalidate();
                    }
                }
            }
            finally
            {
                pageInfoCalcPending = false;
            }
        }

        /// <summary>
        ///  Paints the control.
        /// </summary>
        protected override void OnPaint(PaintEventArgs pevent)
        {
            Color backColor = GetBackColor(SystemInformation.HighContrast);
            using var backBrush = backColor.GetCachedSolidBrushScope();

            if (pageInfo is null || pageInfo.Length == 0)
            {
                pevent.Graphics.FillRectangle(backBrush, ClientRectangle);

                if (pageInfo != null || exceptionPrinting)
                {
                    // Calculate formats
                    using StringFormat format = new StringFormat
                    {
                        Alignment = ControlPaint.TranslateAlignment(ContentAlignment.MiddleCenter),
                        LineAlignment = ControlPaint.TranslateLineAlignment(ContentAlignment.MiddleCenter)
                    };

                    // Do actual drawing
                    using var brush = ForeColor.GetCachedSolidBrushScope();
                    pevent.Graphics.DrawString(
                        exceptionPrinting ? SR.PrintPreviewExceptionPrinting : SR.PrintPreviewNoPages,
                        Font,
                        brush,
                        ClientRectangle,
                        format);
                }
                else
                {
                    BeginInvoke(new MethodInvoker(CalculatePageInfo));
                }
            }
            else
            {
                if (!layoutOk)
                {
                    ComputeLayout();
                }

                Size controlPhysicalSize = new Size(PixelsToPhysical(new Point(Size), screendpi));

                //Point imagePixels = PhysicalToPixels(new Point(imageSize), screendpi);
                Point virtualPixels = new Point(VirtualSize);

                // center pages on screen if small enough
                Point offset = new Point(
                    Math.Max(0, (Size.Width - virtualPixels.X) / 2),
                    Math.Max(0, (Size.Height - virtualPixels.Y) / 2));
                offset.X -= Position.X;
                offset.Y -= Position.Y;
                lastOffset = offset;

                int borderPixelsX = PhysicalToPixels(border, screendpi.X);
                int borderPixelsY = PhysicalToPixels(border, screendpi.Y);

                Region originalClip = pevent.Graphics.Clip;
                Rectangle[] pageRenderArea = new Rectangle[rows * columns];
                Point lastImageSize = Point.Empty;
                int maxImageHeight = 0;

                using (var clipScope = new GraphicsClipScope(pevent.GraphicsInternal))
                {
                    for (int row = 0; row < rows; row++)
                    {
                        // Initialize our LastImageSize variable...
                        lastImageSize.X = 0;
                        lastImageSize.Y = maxImageHeight * row;

                        for (int column = 0; column < columns; column++)
                        {
                            int imageIndex = StartPage + column + row * columns;
                            if (imageIndex < pageInfo.Length)
                            {
                                Size pageSize = pageInfo[imageIndex].PhysicalSize;
                                if (autoZoom)
                                {
                                    double zoomX = ((double)controlPhysicalSize.Width - border * (columns + 1))
                                        / (columns * pageSize.Width);
                                    double zoomY = ((double)controlPhysicalSize.Height - border * (rows + 1))
                                        / (rows * pageSize.Height);
                                    zoom = Math.Min(zoomX, zoomY);
                                }

                                imageSize = new Size((int)(zoom * pageSize.Width), (int)(zoom * pageSize.Height));
                                Point imagePixels = PhysicalToPixels(new Point(imageSize), screendpi);

                                int x = offset.X + borderPixelsX * (column + 1) + lastImageSize.X;
                                int y = offset.Y + borderPixelsY * (row + 1) + lastImageSize.Y;

                                lastImageSize.X += imagePixels.X;
                                //The Height is the Max of any PageHeight..
                                maxImageHeight = Math.Max(maxImageHeight, imagePixels.Y);

                                pageRenderArea[imageIndex - StartPage] = new Rectangle(x, y, imagePixels.X, imagePixels.Y);
                                pevent.Graphics.ExcludeClip(pageRenderArea[imageIndex - StartPage]);
                            }
                        }
                    }

                    pevent.Graphics.FillRectangle(backBrush, ClientRectangle);
                }

                for (int i = 0; i < pageRenderArea.Length; i++)
                {
                    if (i + StartPage < pageInfo.Length)
                    {
                        Rectangle box = pageRenderArea[i];
                        pevent.Graphics.DrawRectangle(Pens.Black, box);
                        using (var brush = ForeColor.GetCachedSolidBrushScope())
                        {
                            pevent.Graphics.FillRectangle(brush, box);
                        }
                        box.Inflate(-1, -1);
                        if (pageInfo[i + StartPage].Image != null)
                        {
                            pevent.Graphics.DrawImage(pageInfo[i + StartPage].Image, box);
                        }
                        box.Width--;
                        box.Height--;
                        pevent.Graphics.DrawRectangle(Pens.Black, box);
                    }
                }
            }

            base.OnPaint(pevent); // raise paint event
        }

        protected virtual void OnStartPageChanged(EventArgs e)
        {
            if (Events[EVENT_STARTPAGECHANGED] is EventHandler eh)
            {
                eh(this, e);
            }
        }

        private static int PhysicalToPixels(int physicalSize, int dpi)
        {
            return (int)(physicalSize * dpi / 100.0);
        }

        private static Point PhysicalToPixels(Point physical, Point dpi)
        {
            return new Point(PhysicalToPixels(physical.X, dpi.X),
                             PhysicalToPixels(physical.Y, dpi.Y));
        }

        private static Size PhysicalToPixels(Size physicalSize, Point dpi)
        {
            return new Size(PhysicalToPixels(physicalSize.Width, dpi.X),
                            PhysicalToPixels(physicalSize.Height, dpi.Y));
        }

        private static int PixelsToPhysical(int pixels, int dpi)
        {
            return (int)(pixels * 100.0 / dpi);
        }

        private static Point PixelsToPhysical(Point pixels, Point dpi)
        {
            return new Point(PixelsToPhysical(pixels.X, dpi.X),
                             PixelsToPhysical(pixels.Y, dpi.Y));
        }

        private static Size PixelsToPhysical(Size pixels, Point dpi)
        {
            return new Size(PixelsToPhysical(pixels.Width, dpi.X),
                            PixelsToPhysical(pixels.Height, dpi.Y));
        }

        /// <summary>
        ///  Resets the back color to the defaults for the PrintPreviewControl.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void ResetBackColor()
        {
            BackColor = SystemColors.AppWorkspace;
        }

        /// <summary>
        ///  Resets the back color to the defaults for the PrintPreviewControl.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void ResetForeColor()
        {
            ForeColor = Color.White;
        }

        /// <summary>
        ///  WM_HSCROLL handler
        /// </summary>
        private void WmHScroll(ref Message m)
        {
            // The lparam is handle of the sending scrollbar, or NULL when
            // the scrollbar sending the message is the "form" scrollbar...
            //
            if (m.LParam != IntPtr.Zero)
            {
                base.WndProc(ref m);
                return;
            }

            Point locPos = position;
            int pos = locPos.X;
            int maxPos = Math.Max(Width, virtualSize.Width /*- Width*/);

            locPos.X = AdjustScroll(m, pos, maxPos, true);
            Position = locPos;
        }

        private void SetPositionNoInvalidate(Point value)
        {
            Point current = position;

            position = value;
            position.X = Math.Min(position.X, virtualSize.Width - Width);
            position.Y = Math.Min(position.Y, virtualSize.Height - Height);
            if (position.X < 0)
            {
                position.X = 0;
            }

            if (position.Y < 0)
            {
                position.Y = 0;
            }

            RECT scroll = ClientRectangle;
            User32.ScrollWindow(
                this,
                current.X - position.X,
                current.Y - position.Y,
                ref scroll,
                ref scroll);

            User32.SetScrollPos(this, User32.SB.HORZ, position.X, BOOL.TRUE);
            User32.SetScrollPos(this, User32.SB.VERT, position.Y, BOOL.TRUE);
        }

        internal void SetVirtualSizeNoInvalidate(Size value)
        {
            virtualSize = value;
            SetPositionNoInvalidate(position); // Make sure it's within range

            var info = new User32.SCROLLINFO
            {
                cbSize = (uint)Marshal.SizeOf<User32.SCROLLINFO>(),
                fMask = User32.SIF.RANGE | User32.SIF.PAGE,
                nMin = 0,
                nMax = Math.Max(Height, virtualSize.Height) - 1,
                nPage = (uint)Height
            };
            User32.SetScrollInfo(this, User32.SB.VERT, ref info, BOOL.TRUE);

            info.fMask = User32.SIF.RANGE | User32.SIF.PAGE;
            info.nMin = 0;
            info.nMax = Math.Max(Width, virtualSize.Width) - 1;
            info.nPage = (uint)Width;
            User32.SetScrollInfo(this, User32.SB.HORZ, ref info, BOOL.TRUE);
        }

        /// <summary>
        ///  WM_VSCROLL handler
        /// </summary>
        private void WmVScroll(ref Message m)
        {
            // The lparam is handle of the sending scrollbar, or NULL when
            // the scrollbar sending the message is the "form" scrollbar...
            //
            if (m.LParam != IntPtr.Zero)
            {
                base.WndProc(ref m);
                return;
            }

            Point locPos = Position;
            int pos = locPos.Y;
            int maxPos = Math.Max(Height, virtualSize.Height/* - Height*/);

            locPos.Y = AdjustScroll(m, pos, maxPos, false);
            Position = locPos;
        }

        /// <summary>
        ///  Handles the WM_KEYDOWN message.
        /// </summary>
        private void WmKeyDown(ref Message msg)
        {
            Keys keyData = (Keys)((int)msg.WParam | (int)ModifierKeys);
            Point locPos = Position;
            int pos = 0;
            int maxPos = 0;

            switch (keyData & Keys.KeyCode)
            {
                case Keys.PageUp:
                    if ((keyData & Keys.Modifiers) == Keys.Control)
                    {
                        pos = locPos.X;
                        if (pos > SCROLL_PAGE)
                        {
                            pos -= SCROLL_PAGE;
                        }
                        else
                        {
                            pos = 0;
                        }
                        locPos.X = pos;
                        Position = locPos;
                    }
                    else if (StartPage > 0)
                    {
                        StartPage--;
                    }
                    break;
                case Keys.PageDown:
                    if ((keyData & Keys.Modifiers) == Keys.Control)
                    {
                        pos = locPos.X;
                        maxPos = Math.Max(Width, virtualSize.Width /*- Width*/);
                        if (pos < maxPos - SCROLL_PAGE)
                        {
                            pos += SCROLL_PAGE;
                        }
                        else
                        {
                            pos = maxPos;
                        }
                        locPos.X = pos;
                        Position = locPos;
                    }
                    else if (StartPage < pageInfo.Length)
                    {
                        StartPage++;
                    }
                    break;
                case Keys.Home:
                    if ((keyData & Keys.Modifiers) == Keys.Control)
                    {
                        StartPage = 0;
                    }

                    break;
                case Keys.End:
                    if ((keyData & Keys.Modifiers) == Keys.Control)
                    {
                        StartPage = pageInfo.Length;
                    }

                    break;

                case Keys.Up:
                    pos = locPos.Y;
                    if (pos > SCROLL_LINE)
                    {
                        pos -= SCROLL_LINE;
                    }
                    else
                    {
                        pos = 0;
                    }
                    locPos.Y = pos;
                    Position = locPos;
                    break;
                case Keys.Down:

                    pos = locPos.Y;
                    maxPos = Math.Max(Height, virtualSize.Height/* - Height*/);

                    if (pos < maxPos - SCROLL_LINE)
                    {
                        pos += SCROLL_LINE;
                    }
                    else
                    {
                        pos = maxPos;
                    }
                    locPos.Y = pos;
                    Position = locPos;
                    break;
                case Keys.Left:

                    pos = locPos.X;
                    if (pos > SCROLL_LINE)
                    {
                        pos -= SCROLL_LINE;
                    }
                    else
                    {
                        pos = 0;
                    }
                    locPos.X = pos;
                    Position = locPos;
                    break;
                case Keys.Right:
                    pos = locPos.X;
                    maxPos = Math.Max(Width, virtualSize.Width /*- Width*/);
                    if (pos < maxPos - SCROLL_LINE)
                    {
                        pos += SCROLL_LINE;
                    }
                    else
                    {
                        pos = maxPos;
                    }
                    locPos.X = pos;
                    Position = locPos;
                    break;
            }
        }

        protected override void WndProc(ref Message m)
        {
            switch ((User32.WM)m.Msg)
            {
                case User32.WM.VSCROLL:
                    WmVScroll(ref m);
                    break;
                case User32.WM.HSCROLL:
                    WmHScroll(ref m);
                    break;
                case User32.WM.KEYDOWN:
                    WmKeyDown(ref m);
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        /// <summary>
        ///  Indicates whether the <see cref='Control.BackColor'/> property should be
        ///  persisted.
        /// </summary>
        internal override bool ShouldSerializeBackColor()
        {
            return !BackColor.Equals(SystemColors.AppWorkspace);
        }

        /// <summary>
        ///  Indicates whether the <see cref='Control.ForeColor'/> property should be
        ///  persisted.
        /// </summary>
        internal override bool ShouldSerializeForeColor()
        {
            return !ForeColor.Equals(Color.White);
        }

        /// <summary>
        ///  Gets back color respectively to the High Contrast theme is applied or not
        ///  and taking into account saved custom back color.
        /// </summary>
        /// <param name="isHighContract">Indicates whether High Contrast theme is applied or not.</param>
        /// <returns>
        ///  Standard back color for PrintPreview control in standard theme (1),
        ///  contrasted color if there is High Contrast theme applied (2) and
        ///  custom color if this is set irrespectively to HC or not HC mode (3).
        /// </returns>
        private Color GetBackColor(bool isHighContract)
        {
            return (isHighContract && !ShouldSerializeBackColor()) ? SystemColors.ControlDark : BackColor;
        }
    }
}
