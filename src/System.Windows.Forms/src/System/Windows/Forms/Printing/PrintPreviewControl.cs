// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms;

/// <summary>
///  The raw "preview" part of print previewing, without any dialogs or buttons.
///  Most PrintPreviewControl's are found on PrintPreviewDialog's,
///  but they don't have to be.
/// </summary>
[DefaultProperty(nameof(Document))]
[SRDescription(nameof(SR.DescriptionPrintPreviewControl))]
public partial class PrintPreviewControl : Control
{
    private Size virtualSize = new Size(1, 1);
    private Point position = new Point(0, 0);
    private Point lastOffset;
    private bool antiAlias;

    private const int SCROLL_LINE = 5;
    private const double DefaultZoom = .3;

    private const int border = 10; // spacing per page, in mm

    private PrintDocument? document;
    private PreviewPageInfo[]? pageInfo; // null if needs refreshing
    private int startPage;  // 0-based
    private int rows = 1;
    private int columns = 1;
    private bool autoZoom = true;
    private readonly int _focusHOffset = SystemInformation.HorizontalFocusThickness;
    private readonly int _focusVOffset = SystemInformation.VerticalFocusThickness;
    private HScrollBar _hScrollBar = new HScrollBar();
    private VScrollBar _vScrollBar = new VScrollBar();
    private bool _scrollLayoutPending;

    // The following are all computed by ComputeLayout
    private bool layoutOk;
    private Size imageSize = System.Drawing.Size.Empty; // 100ths of inch, not pixels
    private Point screendpi = Point.Empty;
    private double zoom = DefaultZoom;
    private bool pageInfoCalcPending;
    private bool exceptionPrinting;

    /// <summary>
    ///  Initializes a new instance of the <see cref="PrintPreviewControl"/> class.
    /// </summary>
    public PrintPreviewControl()
    {
        ResetBackColor();
        ResetForeColor();
        Size = new Size(100, 100);
        SetStyle(ControlStyles.ResizeRedraw, false);
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
        TabStop = false;

        _hScrollBar.AccessibleName = SR.PrintPreviewControlAccHorizontalScrollBarAccName;
        _hScrollBar.Anchor = AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;
        _hScrollBar.Left = _focusHOffset;
        _hScrollBar.SmallChange = SCROLL_LINE;
        _hScrollBar.Visible = false;
        _hScrollBar.ValueChanged += scrollBar_ValueChanged;
        _hScrollBar.TabIndex = 0;
        _hScrollBar.TabStop = true;
        Controls.Add(_hScrollBar);

        _vScrollBar.AccessibleName = SR.PrintPreviewControlAccVerticalScrollBarAccName;
        _vScrollBar.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
        _vScrollBar.SmallChange = SCROLL_LINE;
        _vScrollBar.Top = _focusVOffset;
        _vScrollBar.Visible = false;
        _vScrollBar.Scroll += scrollBar_ValueChanged;
        _vScrollBar.TabIndex = 1;
        _vScrollBar.TabStop = true;
        Controls.Add(_vScrollBar);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _hScrollBar.ValueChanged -= scrollBar_ValueChanged;
            _hScrollBar.Dispose();

            _vScrollBar.ValueChanged -= scrollBar_ValueChanged;
            _vScrollBar.Dispose();

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            _hScrollBar = null;
            _vScrollBar = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        base.Dispose(disposing);
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
    public PrintDocument? Document
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
            Invalidate();
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

    internal override bool SupportsUiaProviders => true;

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Bindable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [AllowNull]
    public override string Text
    {
        get => base.Text;
        set => base.Text = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? TextChanged
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
            if (pageInfo is not null)
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
    public event EventHandler? StartPageChanged
    {
        add => Events.AddHandler(EVENT_STARTPAGECHANGED, value);
        remove => Events.RemoveHandler(EVENT_STARTPAGECHANGED, value);
    }

    [DefaultValue(false)]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DispId(PInvoke.DISPID_TABSTOP)]
    public new bool TabStop
    {
        get => base.TabStop;
        set => base.TabStop = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? TabStopChanged
    {
        add => base.TabStopChanged += value;
        remove => base.TabStopChanged -= value;
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

    // This function computes everything in terms of physical size (millimeters), not pixels
    private void ComputeLayout()
    {
        Debug.Assert(pageInfo is not null, "Must call ComputePreview first");
        layoutOk = true;
        if (pageInfo.Length == 0)
        {
            return;
        }

        using GetDcScope hdc = new(HWND);
        screendpi = new Point(
            PInvoke.GetDeviceCaps(hdc, GET_DEVICE_CAPS_INDEX.LOGPIXELSX),
            PInvoke.GetDeviceCaps(hdc, GET_DEVICE_CAPS_INDEX.LOGPIXELSY));

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
            Debug.Assert(pageInfo is not null, $"{nameof(PreviewPrintController)} did not give us preview info.");

            document.PrintController = oldController;
        }

        if (oldStart != StartPage)
        {
            OnStartPageChanged(EventArgs.Empty);
        }
    }

    protected override AccessibleObject CreateAccessibilityInstance()
        => new PrintPreviewControlAccessibleObject(this);

    // Recomputes the sizes and positions of pages without forcing a new "preview print"
    private void InvalidateLayout()
    {
        if (!IsHandleCreated)
        {
            return;
        }

        layoutOk = false;
        LayoutScrollBars();
        Invalidate();
    }

    /// <summary>
    ///  Refreshes the preview of the document.
    /// </summary>
    public void InvalidatePreview()
    {
        if (!IsHandleCreated)
        {
            return;
        }

        pageInfo = null;
        exceptionPrinting = false;
        SetVirtualSizeNoInvalidate(Size.Empty);
        InvalidateLayout();
    }

    private Rectangle InsideRectangle
    {
        get
        {
            Rectangle rect = InnerClientRectangle;

            if (_hScrollBar.Visible)
            {
                rect.Height -= _hScrollBar.Height;
            }

            if (_vScrollBar.Visible)
            {
                rect.Width -= _vScrollBar.Width;

                if (RightToLeft == RightToLeft.Yes)
                {
                    rect.X += _vScrollBar.Width;
                }
            }

            return rect;
        }
    }

    /// <summary>
    ///  Invalidate the layout, if necessary.
    /// </summary>
    protected override void OnResize(EventArgs eventargs)
    {
        if (!IsHandleCreated)
        {
            return;
        }

        InvalidateLayout();
        base.OnResize(eventargs);
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        if (!Focused)
        {
            Focus();
        }

        base.OnMouseDown(e);
    }

    protected override void OnGotFocus(EventArgs e)
    {
        Invalidate();

        base.OnGotFocus(e);
    }

    protected override void OnLostFocus(EventArgs e)
    {
        Invalidate();

        base.OnLostFocus(e);
    }

    private void CalculatePageInfo()
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

    protected override void OnPaintBackground(PaintEventArgs e)
    {
        PaintTransparentBackground(e, ClientRectangle);
    }

    /// <summary>
    ///  Paints the control.
    /// </summary>
    protected override void OnPaint(PaintEventArgs pevent)
    {
        bool isHighContrast = SystemInformation.HighContrast;
        Color backColor = GetBackColor(isHighContrast);
        using var backBrush = backColor.GetCachedSolidBrushScope();

        PaintResizeBox(pevent, isHighContrast);
        PaintFocus(pevent, isHighContrast);

        if (pageInfo is null || pageInfo.Length == 0)
        {
            pevent.Graphics.FillRectangle(backBrush, ClientRectangle);

            if (pageInfo is not null || exceptionPrinting)
            {
                // Calculate formats
                using StringFormat format = new()
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
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

            Rectangle rect = InsideRectangle;

            using GraphicsClipScope clipScope = new(pevent.GraphicsInternal);
            pevent.GraphicsInternal.SetClip(rect);

            position.X = _hScrollBar.Value;
            position.Y = _vScrollBar.Value;

            Size controlPhysicalSize = new Size(PixelsToPhysical(new Point(Size), screendpi));

            // center pages on screen if small enough
            Point offset = new Point(
                Math.Max(0, (rect.Width - virtualSize.Width) / 2),
                Math.Max(0, (rect.Height - virtualSize.Height) / 2));
            offset.X -= Position.X;
            offset.Y -= Position.Y;
            lastOffset = offset;

            int borderPixelsX = PhysicalToPixels(border, screendpi.X);
            int borderPixelsY = PhysicalToPixels(border, screendpi.Y);

            Rectangle[] pageRenderArea = new Rectangle[rows * columns];
            Point lastImageSize = Point.Empty;
            int maxImageHeight = 0;

            using (new GraphicsClipScope(pevent.GraphicsInternal))
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
                    if (pageInfo[i + StartPage].Image is not null)
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

    private void PaintResizeBox(PaintEventArgs e, bool isHighContrast)
    {
        if (!_hScrollBar.Visible || !_vScrollBar.Visible)
        {
            return;
        }

        e.Graphics.FillRectangle(SystemBrushes.Control, ResizeBoxRectangle);
    }

    private void PaintFocus(PaintEventArgs e, bool isHighContrast)
    {
        Rectangle focusRect = FocusRectangle;

        if (!e.ClipRectangle.Contains(focusRect) || !Focused || !ShowFocusCues)
        {
            return;
        }

        if (isHighContrast)
        {
            ControlPaint.DrawHighContrastFocusRectangle(e.Graphics, focusRect, SystemColors.ControlText);
        }
        else
        {
            ControlPaint.DrawFocusRectangle(e.Graphics, focusRect);
        }
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

    private void SetPositionNoInvalidate(Point value)
    {
        _scrollLayoutPending = true;

        _hScrollBar.Value = Clamp(value.X, _hScrollBar.Minimum, _hScrollBar.Maximum);
        _vScrollBar.Value = Clamp(value.Y, _vScrollBar.Minimum, _vScrollBar.Maximum);

        _scrollLayoutPending = false;

        static int Clamp(int value, int min, int max)
        {
            if (value < min)
            {
                return min;
            }

            if (max < value)
            {
                return max;
            }

            return value;
        }
    }

    internal void SetVirtualSizeNoInvalidate(Size value)
    {
        if (!IsHandleCreated)
        {
            return;
        }

        virtualSize = value;

        LayoutScrollBars();
    }

    private Rectangle InnerClientRectangle
    {
        get
        {
            Rectangle rect = ClientRectangle;
            rect.Inflate(-_focusHOffset, -_focusVOffset);
            rect.Width--;
            rect.Height--;

            return rect;
        }
    }

    private Rectangle FocusRectangle
        => new(0, 0, Width - 1, Height - 1);

    private Rectangle ResizeBoxRectangle
        => new(_vScrollBar.Left, _hScrollBar.Top, _vScrollBar.Width, _hScrollBar.Height);

    private void LayoutScrollBars()
    {
        if (!IsHandleCreated)
        {
            return;
        }

        _scrollLayoutPending = true;
        SuspendLayout();

        Rectangle availableRect = InnerClientRectangle;

        (bool horizontalScrollNeeded, bool verticalScrollNeeded) = IsScrollNeeded(availableRect.Size);

        _hScrollBar.Visible = horizontalScrollNeeded;
        if (horizontalScrollNeeded)
        {
            _hScrollBar.Top = availableRect.Bottom - _hScrollBar.Height;
            _hScrollBar.Width = availableRect.Width - (verticalScrollNeeded ? _vScrollBar.Width : 0);

            AdjustScroll(
                _hScrollBar,
                virtualDimension: virtualSize.Width,
                displayDimension: availableRect.Width,
                offset: (verticalScrollNeeded ? _vScrollBar.Width : 0));
        }
        else if (_hScrollBar.Value > 0)
        {
            _hScrollBar.Value = 0;
        }

        _vScrollBar.Visible = verticalScrollNeeded;
        if (verticalScrollNeeded)
        {
            _vScrollBar.Height = availableRect.Height - (horizontalScrollNeeded ? _hScrollBar.Height : 0);

            if (RightToLeft == RightToLeft.Yes)
            {
                _vScrollBar.Left = availableRect.Left;
                _vScrollBar.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom;
            }
            else
            {
                _vScrollBar.Left = availableRect.Right - _vScrollBar.Width;
                _vScrollBar.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
            }

            AdjustScroll(
                _vScrollBar,
                virtualDimension: virtualSize.Height,
                displayDimension: availableRect.Height,
                offset: (horizontalScrollNeeded ? _hScrollBar.Height : 0));
        }
        else if (_vScrollBar.Value > 0)
        {
            _vScrollBar.Value = 0;
        }

        ResumeLayout(true);
        _scrollLayoutPending = false;

        (bool horizontal, bool vertical) IsScrollNeeded(Size displaySize)
        {
            bool horizontal = virtualSize.Width > displaySize.Width;
            bool vertical = virtualSize.Height > displaySize.Height;

            if (!horizontal && vertical)
            {
                horizontal = virtualSize.Width > (displaySize.Width - _vScrollBar.Width);
            }

            if (!vertical && horizontal)
            {
                vertical = virtualSize.Height > (displaySize.Height - _hScrollBar.Height);
            }

            return (horizontal, vertical);
        }

        void AdjustScroll(ScrollBar scrollBar, int virtualDimension, int displayDimension, int offset)
        {
            var oldLargeChange = scrollBar.LargeChange;

            scrollBar.Maximum = virtualDimension;
            scrollBar.LargeChange = displayDimension - offset;

            if (scrollBar.Value > 0)
            {
                var diff = scrollBar.LargeChange - oldLargeChange;

                if (scrollBar.Value >= diff)
                {
                    scrollBar.Value -= diff;
                }
            }
        }
    }

    private void scrollBar_ValueChanged(object? sender, EventArgs e)
    {
        if (_scrollLayoutPending)
        {
            return;
        }

        Invalidate(InsideRectangle);
    }

    /// <summary>
    ///  Handles the WM_KEYDOWN message.
    /// </summary>
    private void WmKeyDown(ref Message msg)
    {
        Keys keyData = (Keys)(nint)msg.WParamInternal | ModifierKeys;
        Point locPos = Position;
        int pos;
        int maxPos;
        bool scrollVisible = _vScrollBar.Visible || _hScrollBar.Visible;
        bool vertical = _vScrollBar.Visible;
        int largeChange = vertical ? _vScrollBar.LargeChange : _hScrollBar.LargeChange;

        switch (keyData & Keys.KeyCode)
        {
            case Keys.PageUp:
                if ((keyData & Keys.Modifiers) == Keys.Control)
                {
                    if (scrollVisible)
                    {
                        pos = vertical ? locPos.Y : locPos.X;
                        if (pos > largeChange)
                        {
                            pos -= largeChange;
                        }
                        else
                        {
                            pos = 0;
                        }

                        if (vertical)
                        {
                            locPos.Y = pos;
                        }
                        else
                        {
                            locPos.X = pos;
                        }

                        Position = locPos;
                    }
                }
                else if (StartPage > 0)
                {
                    StartPage--;
                }

                break;
            case Keys.PageDown:
                if ((keyData & Keys.Modifiers) == Keys.Control)
                {
                    if (scrollVisible)
                    {
                        pos = vertical ? locPos.Y : locPos.X;
                        maxPos = vertical ? _vScrollBar.Maximum : _hScrollBar.Maximum;
                        if (pos < maxPos - largeChange)
                        {
                            pos += largeChange;
                        }
                        else
                        {
                            pos = maxPos;
                        }

                        if (vertical)
                        {
                            locPos.Y = pos;
                        }
                        else
                        {
                            locPos.X = pos;
                        }

                        Position = locPos;
                    }
                }
                else if (pageInfo is not null && StartPage < pageInfo.Length)
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
                if (pageInfo is not null && (keyData & Keys.Modifiers) == Keys.Control)
                {
                    StartPage = pageInfo.Length;
                }

                break;

            case Keys.Up:
                if (_vScrollBar.Visible)
                {
                    pos = locPos.Y;
                    if (pos > _vScrollBar.LargeChange)
                    {
                        pos -= _vScrollBar.LargeChange;
                    }
                    else
                    {
                        pos = 0;
                    }

                    locPos.Y = pos;
                    Position = locPos;
                }

                break;
            case Keys.Down:
                if (_vScrollBar.Visible)
                {
                    pos = locPos.Y;
                    maxPos = _vScrollBar.Maximum - _vScrollBar.LargeChange;

                    if (pos < maxPos - _vScrollBar.LargeChange)
                    {
                        pos += _vScrollBar.LargeChange;
                    }
                    else
                    {
                        pos = maxPos;
                    }

                    locPos.Y = pos;
                    Position = locPos;
                }

                break;
            case Keys.Left:
                if (_hScrollBar.Visible)
                {
                    pos = locPos.X;
                    if (pos > _hScrollBar.LargeChange)
                    {
                        pos -= _hScrollBar.LargeChange;
                    }
                    else
                    {
                        pos = 0;
                    }

                    locPos.X = pos;
                    Position = locPos;
                }

                break;
            case Keys.Right:
                if (_hScrollBar.Visible)
                {
                    pos = locPos.X;
                    maxPos = _hScrollBar.Maximum - _hScrollBar.LargeChange;

                    if (pos < maxPos - _hScrollBar.LargeChange)
                    {
                        pos += _hScrollBar.LargeChange;
                    }
                    else
                    {
                        pos = maxPos;
                    }

                    locPos.X = pos;
                    Position = locPos;
                }

                break;
        }
    }

    protected override void WndProc(ref Message m)
    {
        switch (m.MsgInternal)
        {
            case PInvoke.WM_KEYDOWN:
                WmKeyDown(ref m);
                break;
            default:
                base.WndProc(ref m);
                break;
        }
    }

    /// <summary>
    ///  Indicates whether the <see cref="Control.BackColor"/> property should be persisted.
    /// </summary>
    internal override bool ShouldSerializeBackColor()
    {
        return !BackColor.Equals(SystemColors.AppWorkspace);
    }

    /// <summary>
    ///  Indicates whether the <see cref="Control.ForeColor"/> property should be persisted.
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
