// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

/// <summary>
///  The raw "preview" part of print previewing, without any dialogs or buttons.  Most <see cref="PrintPreviewControl"/>
///  objects are found on <see cref="PrintPreviewDialog"/> objects, but they don't have to be.
/// </summary>
[DefaultProperty(nameof(Document))]
[SRDescription(nameof(SR.DescriptionPrintPreviewControl))]
public partial class PrintPreviewControl : Control
{
    private const int ScrollSmallChange = 5;
    private const double DefaultZoom = .3;

    // Spacing per page, in mm
    private const int Border = 10;

    private static readonly object s_startPageChangedEvent = new();

    private PrintDocument? _document;
    private PreviewPageInfo[]? _pageInfo; // null if needs refreshing
    private int _startPage;  // 0-based
    private int _rows = 1;
    private int _columns = 1;
    private bool _autoZoom = true;
    private Size _virtualSize = new(1, 1);
    private Point _position = new(0, 0);

    private readonly int _focusHOffset = SystemInformation.HorizontalFocusThickness;
    private readonly int _focusVOffset = SystemInformation.VerticalFocusThickness;
    private readonly HorizontalScrollBar _hScrollBar;
    private readonly VerticalScrollBar _vScrollBar;
    private bool _scrollLayoutPending;

    // The following are all computed by ComputeLayout
    private bool _layoutOk;

    // 100ths of an inch, not pixels
    private Size _imageSize = Size.Empty;
    private Point _screenDPI = Point.Empty;
    private double _zoom = DefaultZoom;
    private bool _pageInfoCalcPending;
    private bool _exceptionPrinting;

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

        _hScrollBar = new()
        {
            AccessibleName = SR.HScrollBarDefaultAccessibleName,
            Anchor = AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right,
            Left = _focusHOffset,
            SmallChange = ScrollSmallChange,
            RightToLeft = RightToLeft.No,
            Visible = false,
            TabStop = false
        };

        _hScrollBar.ValueChanged += scrollBar_ValueChanged;
        Controls.Add(_hScrollBar);

        _vScrollBar = new()
        {
            AccessibleName = SR.VScrollBarDefaultAccessibleName,
            Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom,
            SmallChange = ScrollSmallChange,
            Top = _focusVOffset,
            Visible = false,
            TabStop = false
        };

        _vScrollBar.ValueChanged += scrollBar_ValueChanged;
        Controls.Add(_vScrollBar);
    }

    /// <summary>
    ///  Gets or sets a value indicating whether printing uses the anti-aliasing features of the operating system.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(false)]
    [SRDescription(nameof(SR.PrintPreviewAntiAliasDescr))]
    public bool UseAntiAlias
    {
        get; set;
    }

    /// <summary>
    ///  Gets or sets a value indicating whether resizing the control or changing the number of pages shown
    ///  automatically adjusts the <see cref="Zoom"/> property.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(true)]
    [SRDescription(nameof(SR.PrintPreviewAutoZoomDescr))]
    public bool AutoZoom
    {
        get => _autoZoom;
        set
        {
            if (_autoZoom != value)
            {
                _autoZoom = value;
                InvalidateLayout();
            }
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
        get => _zoom;
        set
        {
            if (value <= 0)
            {
                throw new ArgumentException(SR.PrintPreviewControlZoomNegative);
            }

            _autoZoom = false;
            _zoom = value;
            InvalidateLayout();
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
        get => _document;
        set
        {
            _document = value;
            InvalidatePreview();
        }
    }

    /// <summary>
    ///  Gets or sets the number of pages displayed vertically down the screen.
    /// </summary>
    [DefaultValue(1)]
    [SRDescription(nameof(SR.PrintPreviewRowsDescr))]
    [SRCategory(nameof(SR.CatBehavior))]
    public int Rows
    {
        get => _rows;
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value);

            _rows = value;
            InvalidateLayout();
        }
    }

    /// <summary>
    ///  Gets or sets the number of pages displayed horizontally across the screen.
    /// </summary>
    [DefaultValue(1)]
    [SRCategory(nameof(SR.CatLayout))]
    [SRDescription(nameof(SR.PrintPreviewColumnsDescr))]
    public int Columns
    {
        get => _columns;
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value);

            _columns = value;
            InvalidateLayout();
        }
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
            int value = _startPage;
            if (_pageInfo is not null)
            {
                value = Math.Min(value, _pageInfo.Length - (_rows * _columns));
            }

            value = Math.Max(value, 0);

            return value;
        }
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);

            int oldValue = StartPage;
            _startPage = value;
            if (oldValue != _startPage)
            {
                InvalidateLayout();
                OnStartPageChanged(EventArgs.Empty);
            }
        }
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.RadioButtonOnStartPageChangedDescr))]
    public event EventHandler? StartPageChanged
    {
        add => Events.AddHandler(s_startPageChangedEvent, value);
        remove => Events.RemoveHandler(s_startPageChangedEvent, value);
    }

    protected virtual void OnStartPageChanged(EventArgs e)
    {
        if (Events[s_startPageChangedEvent] is EventHandler eh)
        {
            eh(this, e);
        }
    }

    /// <summary>
    ///  Gets or sets a value indicating whether control's elements are aligned to support locales using right-to-left fonts.
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

    [DefaultValue(false)]
    [DispId(PInvokeCore.DISPID_TABSTOP)]
    public new bool TabStop
    {
        get => base.TabStop;
        set => base.TabStop = value;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override void ResetBackColor() => BackColor = Application.SystemColors.AppWorkspace;

    internal override bool ShouldSerializeBackColor() => !BackColor.Equals(SystemColors.AppWorkspace);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override void ResetForeColor() => ForeColor = Color.White;

    internal override bool ShouldSerializeForeColor() => !ForeColor.Equals(Color.White);

    internal override bool SupportsUiaProviders => true;

    protected override AccessibleObject CreateAccessibilityInstance()
        => new PrintPreviewControlAccessibleObject(this);

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

    protected override void OnPaintBackground(PaintEventArgs e) => PaintTransparentBackground(e, ClientRectangle);

    protected override void OnPaint(PaintEventArgs pevent)
    {
        bool isHighContrast = SystemInformation.HighContrast;
        Color backColor = GetBackColor(isHighContrast);
        using var backBrush = backColor.GetCachedSolidBrushScope();

        PaintResizeBox(pevent);
        PaintFocus(pevent, isHighContrast);

        if (_pageInfo is null || _pageInfo.Length == 0)
        {
            Rectangle rect = InsideRectangle;

            pevent.Graphics.FillRectangle(backBrush, rect);

            if (_pageInfo is not null || _exceptionPrinting)
            {
                DrawMessage(pevent.Graphics, rect, _exceptionPrinting);
            }
            else
            {
                BeginInvoke(new MethodInvoker(CalculatePageInfo));
            }
        }
        else
        {
            if (!_layoutOk)
            {
                ComputeLayout();
            }

            DrawPages(pevent.Graphics, InsideRectangle, _pageInfo, backBrush);
        }

        base.OnPaint(pevent); // raise paint event
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
        get => _virtualSize;
        set
        {
            SetVirtualSizeNoInvalidate(value);
            Invalidate();
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
        get => _position;
        set
        {
            SetPositionNoInvalidate(value);
            Invalidate();
        }
    }

    /// <summary>
    ///  Equal to the vertical percentage of the entire control that is currently viewable.
    /// </summary>
    internal double VerticalViewSize => InsideRectangle.Height * 100.0 / VirtualSize.Height;

    /// <summary>
    ///  Equal to the horizontal percentage of the entire control that is currently viewable.
    /// </summary>
    internal double HorizontalViewSize => InsideRectangle.Width * 100.0 / VirtualSize.Width;

    private Rectangle InnerClientRectangle
    {
        get
        {
            Rectangle rect = ClientRectangle;
            rect.Inflate(-_focusHOffset, -_focusVOffset);
            rect.Width = Math.Max(0, rect.Width - 1);
            rect.Height = Math.Max(0, rect.Height - 1);

            return rect;
        }
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

    private Rectangle FocusRectangle => new(0, 0, Width - 1, Height - 1);

    private Rectangle ResizeBoxRectangle => new(_vScrollBar.Left, _hScrollBar.Top, _vScrollBar.Width, _hScrollBar.Height);

    // This function computes everything in terms of physical size (millimeters), not pixels
    private void ComputeLayout()
    {
        Debug.Assert(_pageInfo is not null, "Must call ComputePreview first");
        _layoutOk = true;
        if (_pageInfo.Length == 0)
        {
            return;
        }

        using GetDcScope hdc = new(HWND);
        _screenDPI = new Point(
            PInvokeCore.GetDeviceCaps(hdc, GET_DEVICE_CAPS_INDEX.LOGPIXELSX),
            PInvokeCore.GetDeviceCaps(hdc, GET_DEVICE_CAPS_INDEX.LOGPIXELSY));

        Size pageSize = _pageInfo[StartPage].PhysicalSize;
        Size controlPhysicalSize = PixelsToPhysical(Size, _screenDPI);

        if (_autoZoom)
        {
            double zoomX = ((double)controlPhysicalSize.Width - Border * (_columns + 1)) / (_columns * pageSize.Width);
            double zoomY = ((double)controlPhysicalSize.Height - Border * (_rows + 1)) / (_rows * pageSize.Height);
            _zoom = Math.Min(zoomX, zoomY);
        }

        _imageSize = new Size((int)(_zoom * pageSize.Width), (int)(_zoom * pageSize.Height));
        int virtualX = (_imageSize.Width * _columns) + Border * (_columns + 1);
        int virtualY = (_imageSize.Height * _rows) + Border * (_rows + 1);
        SetVirtualSizeNoInvalidate(PhysicalToPixels(new Size(virtualX, virtualY), _screenDPI));
    }

    // "Prints" the document to memory
    private void ComputePreview()
    {
        int oldStart = StartPage;

        if (_document is null)
        {
            _pageInfo = [];
        }
        else
        {
            PrintController oldController = _document.PrintController;
            PreviewPrintController previewController = new PreviewPrintController
            {
                UseAntiAlias = UseAntiAlias
            };

            _document.PrintController = new PrintControllerWithStatusDialog(
                previewController,
                SR.PrintControllerWithStatusDialog_DialogTitlePreview);

            _document.Print();
            _pageInfo = previewController.GetPreviewPageInfo();
            Debug.Assert(_pageInfo is not null, $"{nameof(PreviewPrintController)} did not give us preview info.");

            _document.PrintController = oldController;
        }

        if (oldStart != StartPage)
        {
            OnStartPageChanged(EventArgs.Empty);
        }
    }

    // Recomputes the sizes and positions of pages without forcing a new "preview print"
    private void InvalidateLayout()
    {
        if (!IsHandleCreated)
        {
            return;
        }

        _layoutOk = false;
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

        _pageInfo = null;
        _exceptionPrinting = false;
        SetVirtualSizeNoInvalidate(Size.Empty);
        InvalidateLayout();
    }

    private void CalculatePageInfo()
    {
        if (_pageInfoCalcPending)
        {
            return;
        }

        _pageInfoCalcPending = true;
        try
        {
            if (_pageInfo is null)
            {
                try
                {
                    ComputePreview();
                }
                catch
                {
                    _exceptionPrinting = true;
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
            _pageInfoCalcPending = false;
        }
    }

    private void DrawMessage(Graphics g, Rectangle rect, bool isExceptionPrinting)
    {
        using var brush = ForeColor.GetCachedSolidBrushScope();

        using StringFormat format = new()
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };

        string message = isExceptionPrinting ? SR.PrintPreviewExceptionPrinting : SR.PrintPreviewNoPages;

        g.DrawString(message, Font, brush, rect, format);
    }

    private void DrawPages(Graphics g, Rectangle rect, PreviewPageInfo[] pages, Brush backBrush)
    {
        using GraphicsClipScope clipScope = new(g);
        g.SetClip(rect);

        _position.X = _hScrollBar.Value;
        _position.Y = _vScrollBar.Value;

        Size controlPhysicalSize = PixelsToPhysical(rect.Size, _screenDPI);

        // center pages on screen if small enough
        Point offset = new(
            Math.Max(0, (rect.Width - _virtualSize.Width) / 2),
            Math.Max(0, (rect.Height - _virtualSize.Height) / 2));
        offset.X -= Position.X;
        offset.Y -= Position.Y;

        int borderPixelsX = PhysicalToPixels(Border, _screenDPI.X);
        int borderPixelsY = PhysicalToPixels(Border, _screenDPI.Y);

        Rectangle[] pageRenderArea = new Rectangle[_rows * _columns];
        Point lastImageSize = Point.Empty;
        int maxImageHeight = 0;

        using (new GraphicsClipScope(g))
        {
            for (int row = 0; row < _rows; row++)
            {
                // Initialize our LastImageSize variable...
                lastImageSize.X = 0;
                lastImageSize.Y = maxImageHeight * row;

                for (int column = 0; column < _columns; column++)
                {
                    int imageIndex = StartPage + column + row * _columns;
                    if (imageIndex < pages.Length)
                    {
                        Size pageSize = pages[imageIndex].PhysicalSize;
                        if (_autoZoom)
                        {
                            double zoomX = ((double)controlPhysicalSize.Width - Border * (_columns + 1))
                                / (_columns * pageSize.Width);
                            double zoomY = ((double)controlPhysicalSize.Height - Border * (_rows + 1))
                                / (_rows * pageSize.Height);
                            _zoom = Math.Min(zoomX, zoomY);
                        }

                        _imageSize = new Size((int)(_zoom * pageSize.Width), (int)(_zoom * pageSize.Height));
                        Size imagePixels = PhysicalToPixels(_imageSize, _screenDPI);

                        int x = offset.X + borderPixelsX * (column + 1) + lastImageSize.X;
                        int y = offset.Y + borderPixelsY * (row + 1) + lastImageSize.Y;

                        lastImageSize.X += imagePixels.Width;

                        // The Height is the Max of any PageHeight..
                        maxImageHeight = Math.Max(maxImageHeight, imagePixels.Height);

                        pageRenderArea[imageIndex - StartPage] = new Rectangle(x, y, imagePixels.Width, imagePixels.Height);
                        g.ExcludeClip(pageRenderArea[imageIndex - StartPage]);
                    }
                }
            }

            g.FillRectangle(backBrush, rect);
        }

        for (int i = 0; i < pageRenderArea.Length; i++)
        {
            if (i + StartPage < pages.Length)
            {
                Rectangle box = pageRenderArea[i];
                g.DrawRectangle(Pens.Black, box);
                using (var brush = ForeColor.GetCachedSolidBrushScope())
                {
                    g.FillRectangle(brush, box);
                }

                box.Inflate(-1, -1);
                if (pages[i + StartPage].Image is not null)
                {
                    g.DrawImage(pages[i + StartPage].Image, box);
                }

                box.Width--;
                box.Height--;
                g.DrawRectangle(Pens.Black, box);
            }
        }
    }

    private void PaintResizeBox(PaintEventArgs e)
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
            ControlPaint.DrawHighContrastFocusRectangle(e.Graphics, focusRect, Application.SystemColors.ControlText);
        }
        else
        {
            ControlPaint.DrawFocusRectangle(e.Graphics, focusRect);
        }
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
        return (isHighContract && !ShouldSerializeBackColor()) ? Application.SystemColors.ControlDark : BackColor;
    }

    private static int PixelsToPhysical(int pixels, int dpi) => (int)(pixels * 100.0 / dpi);

    private static Size PixelsToPhysical(Size pixels, Point dpi) =>
        new(PixelsToPhysical(pixels.Width, dpi.X), PixelsToPhysical(pixels.Height, dpi.Y));

    private static int PhysicalToPixels(int physicalSize, int dpi) =>
        (int)(physicalSize * dpi / 100.0);

    private static Size PhysicalToPixels(Size physical, Point dpi) =>
        new(PhysicalToPixels(physical.Width, dpi.X), PhysicalToPixels(physical.Height, dpi.Y));

    private void SetPositionNoInvalidate(Point value)
    {
        _scrollLayoutPending = true;

        _hScrollBar.Value = Math.Clamp(value.X, _hScrollBar.Minimum, _hScrollBar.Maximum);
        _vScrollBar.Value = Math.Clamp(value.Y, _vScrollBar.Minimum, _vScrollBar.Maximum);

        _scrollLayoutPending = false;
    }

    internal void SetVirtualSizeNoInvalidate(Size value)
    {
        if (!IsHandleCreated)
        {
            return;
        }

        _virtualSize = value;

        LayoutScrollBars();
    }

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

        bool scrollBarsVisibilityChanged = (_vScrollBar.Visible ^ verticalScrollNeeded) || (_hScrollBar.Visible ^ horizontalScrollNeeded);

        _hScrollBar.Visible = horizontalScrollNeeded;
        if (horizontalScrollNeeded)
        {
            _hScrollBar.Top = availableRect.Bottom - _hScrollBar.Height;
            _hScrollBar.Width = availableRect.Width - (verticalScrollNeeded ? _vScrollBar.Width : 0);

            AdjustScroll(
                _hScrollBar,
                virtualDimension: _virtualSize.Width,
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
                virtualDimension: _virtualSize.Height,
                displayDimension: availableRect.Height,
                offset: (horizontalScrollNeeded ? _hScrollBar.Height : 0));
        }
        else if (_vScrollBar.Value > 0)
        {
            _vScrollBar.Value = 0;
        }

        ResumeLayout(true);
        _scrollLayoutPending = false;

        if (scrollBarsVisibilityChanged
            && IsAccessibilityObjectCreated
            && AccessibilityObject is PrintPreviewControlAccessibleObject ao)
        {
            ao.RaiseStructureChangedEvent(StructureChangeType.StructureChangeType_ChildrenInvalidated, []);
        }

        (bool horizontal, bool vertical) IsScrollNeeded(Size displaySize)
        {
            bool horizontal = _virtualSize.Width > displaySize.Width && displaySize.Width > _vScrollBar.Width;
            bool vertical = _virtualSize.Height > displaySize.Height && displaySize.Height > _hScrollBar.Height;

            if (!horizontal && vertical)
            {
                horizontal = _virtualSize.Width > (displaySize.Width - _vScrollBar.Width);
            }

            if (!vertical && horizontal)
            {
                vertical = _virtualSize.Height > (displaySize.Height - _hScrollBar.Height);
            }

            return (horizontal, vertical);
        }

        void AdjustScroll(ScrollBar scrollBar, int virtualDimension, int displayDimension, int offset)
        {
            int oldLargeChange = scrollBar.LargeChange;

            scrollBar.Maximum = virtualDimension;
            scrollBar.LargeChange = displayDimension - offset;

            if (scrollBar.Value > 0)
            {
                int diff = scrollBar.LargeChange - oldLargeChange;

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
                else if (_pageInfo is not null && StartPage < _pageInfo.Length)
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
                if (_pageInfo is not null && (keyData & Keys.Modifiers) == Keys.Control)
                {
                    StartPage = _pageInfo.Length;
                }

                break;

            case Keys.Up:
                if (_vScrollBar.Visible)
                {
                    pos = locPos.Y;
                    if (pos > _vScrollBar.SmallChange)
                    {
                        pos -= _vScrollBar.SmallChange;
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
                    maxPos = _vScrollBar.Maximum - _vScrollBar.SmallChange;

                    if (pos < maxPos - _vScrollBar.SmallChange)
                    {
                        pos += _vScrollBar.SmallChange;
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
                    if (pos > _hScrollBar.SmallChange)
                    {
                        pos -= _hScrollBar.SmallChange;
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
                    maxPos = _hScrollBar.Maximum - _hScrollBar.SmallChange;

                    if (pos < maxPos - _hScrollBar.SmallChange)
                    {
                        pos += _hScrollBar.SmallChange;
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
}
