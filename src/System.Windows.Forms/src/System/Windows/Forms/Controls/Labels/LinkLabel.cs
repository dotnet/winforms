// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Windows.Forms.Internal;
using System.Windows.Forms.Layout;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

/// <summary>
///  Displays text that can contain a hyperlink.
/// </summary>
[DefaultEvent(nameof(LinkClicked))]
[ToolboxItem($"System.Windows.Forms.Design.AutoSizeToolboxItem,{AssemblyRef.SystemDesign}")]
[SRDescription(nameof(SR.DescriptionLinkLabel))]
public partial class LinkLabel : Label, IButtonControl
{
    private static readonly object s_eventLinkClicked = new();
    private static Color s_iedisabledLinkColor = Color.Empty;

    private static readonly LinkComparer s_linkComparer = new();

    private DialogResult _dialogResult;

    private Color _linkColor = Color.Empty;
    private Color _activeLinkColor = Color.Empty;
    private Color _visitedLinkColor = Color.Empty;
    private Color _disabledLinkColor = Color.Empty;

    private Font? _linkFont;
    private Font? _hoverLinkFont;

    private bool _textLayoutValid;
    private bool _receivedDoubleClick;
    private readonly List<Link> _links = new(2);

    private Link? _focusLink;
    private LinkCollection? _linkCollection;
    private Region? _textRegion;
    private Cursor? _overrideCursor;

    private bool _processingOnGotFocus;  // used to avoid raising the OnGotFocus event twice after selecting a focus link.

    private LinkBehavior _linkBehavior = LinkBehavior.SystemDefault;

    /// <summary>
    ///  Initializes a new default instance of the <see cref="LinkLabel"/> class.
    /// </summary>
    public LinkLabel() : base()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint
            | ControlStyles.OptimizedDoubleBuffer
            | ControlStyles.Opaque
            | ControlStyles.UserPaint
            | ControlStyles.StandardClick
            | ControlStyles.ResizeRedraw,
            value: true);
        ResetLinkArea();
    }

    /// <summary>
    ///  Gets or sets the color used to display active links.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.LinkLabelActiveLinkColorDescr))]
    public Color ActiveLinkColor
    {
        get => _activeLinkColor.IsEmpty ? IEActiveLinkColor : _activeLinkColor;
        set
        {
            if (_activeLinkColor != value)
            {
                _activeLinkColor = value;
                InvalidateLink(null);
            }
        }
    }

    /// <summary>
    ///  Gets or sets the color used to display disabled links.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.LinkLabelDisabledLinkColorDescr))]
    public Color DisabledLinkColor
    {
        get => _disabledLinkColor.IsEmpty ? IEDisabledLinkColor : _disabledLinkColor;
        set
        {
            if (_disabledLinkColor != value)
            {
                _disabledLinkColor = value;
                InvalidateLink(null);
            }
        }
    }

    private Link? FocusLink
    {
        get => _focusLink;
        set
        {
            if (_focusLink != value)
            {
                if (_focusLink is not null)
                {
                    InvalidateLink(_focusLink);
                }

                _focusLink = value;

                if (_focusLink is not null)
                {
                    InvalidateLink(_focusLink);

                    UpdateAccessibilityLink(_focusLink);
                }
            }
        }
    }

    private static Color IELinkColor => LinkUtilities.IELinkColor;

    private static Color IEActiveLinkColor => LinkUtilities.IEActiveLinkColor;

    private static Color IEVisitedLinkColor => LinkUtilities.IEVisitedLinkColor;

    private Color IEDisabledLinkColor
    {
        get
        {
            if (s_iedisabledLinkColor.IsEmpty)
            {
                s_iedisabledLinkColor = ControlPaint.Dark(DisabledColor);
            }

            return s_iedisabledLinkColor;
        }
    }

    private Rectangle ClientRectWithPadding => LayoutUtils.DeflateRect(ClientRectangle, Padding);

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new FlatStyle FlatStyle
    {
        get => base.FlatStyle;
        set => base.FlatStyle = value;
    }

    /// <summary>
    ///  Gets or sets the range in the text that is treated as a link.
    /// </summary>
    [Editor($"System.Windows.Forms.Design.LinkAreaEditor, {AssemblyRef.SystemDesign}", typeof(UITypeEditor))]
    [RefreshProperties(RefreshProperties.Repaint)]
    [Localizable(true)]
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.LinkLabelLinkAreaDescr))]
    public LinkArea LinkArea
    {
        get => _links.Count == 0
            ? new LinkArea(0, 0)
            : new LinkArea(_links[0].Start, _links[0].Length);
        set
        {
            LinkArea pt = LinkArea;

            _links.Clear();

            if (!value.IsEmpty)
            {
                if (value.Start < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(LinkArea), value, SR.LinkLabelAreaStart);
                }

                if (value.Length < -1)
                {
                    throw new ArgumentOutOfRangeException(nameof(LinkArea), value, SR.LinkLabelAreaLength);
                }

                if (value.Start != 0 || !value.IsEmpty)
                {
                    Links.Add(new Link(this));

                    // Update the link area of the first link.
                    _links[0].Start = value.Start;
                    _links[0].Length = value.Length;
                }
            }

            UpdateSelectability();

            if (!pt.Equals(LinkArea))
            {
                InvalidateTextLayout();
                LayoutTransaction.DoLayout(ParentInternal, this, PropertyNames.LinkArea);
                AdjustSize();
                Invalidate();
            }
        }
    }

    /// <summary>
    ///  Gets ir sets a value that represents how the link will be underlined.
    /// </summary>
    [DefaultValue(LinkBehavior.SystemDefault)]
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.LinkLabelLinkBehaviorDescr))]
    public LinkBehavior LinkBehavior
    {
        get => _linkBehavior;
        set
        {
            // Valid values are 0x0 to 0x3
            SourceGenerated.EnumValidator.Validate(value);
            if (value != _linkBehavior)
            {
                _linkBehavior = value;
                InvalidateLinkFonts();
                InvalidateLink(null);
            }
        }
    }

    /// <summary>
    ///  Gets or sets the color used to display links in normal cases.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.LinkLabelLinkColorDescr))]
    public Color LinkColor
    {
        get => _linkColor.IsEmpty
            ? SystemInformation.HighContrast ? SystemColors.HotTrack : IELinkColor
            : _linkColor;
        set
        {
            if (_linkColor != value)
            {
                _linkColor = value;
                InvalidateLink(null);
            }
        }
    }

    /// <summary>
    ///  Gets the collection of links used in a <see cref="LinkLabel"/>.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public LinkCollection Links => _linkCollection ??= new LinkCollection(this);

    /// <summary>
    ///  Gets or sets a value indicating whether the link should be displayed as if it was visited.
    /// </summary>
    [DefaultValue(false)]
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.LinkLabelLinkVisitedDescr))]
    public bool LinkVisited
    {
        get => _links.Count == 0 ? false : _links[0].Visited;
        set
        {
            if (value != LinkVisited)
            {
                if (_links.Count == 0)
                {
                    Links.Add(new Link(this));
                }

                _links[0].Visited = value;
            }
        }
    }

    internal override bool OwnerDraw => true;

    protected Cursor? OverrideCursor
    {
        get => _overrideCursor;
        set
        {
            if (_overrideCursor == value)
            {
                return;
            }

            _overrideCursor = value;

            if (IsHandleCreated)
            {
                // We want to instantly change the cursor if the mouse is within our bounds.
                // This includes the case where the mouse is over one of our children
                PInvoke.GetCursorPos(out Point p);
                PInvoke.GetWindowRect(this, out var r);
                if ((r.left <= p.X && p.X < r.right && r.top <= p.Y && p.Y < r.bottom) || PInvoke.GetCapture() == HWND)
                {
                    PInvoke.SendMessage(this, PInvoke.WM_SETCURSOR, (WPARAM)HWND, (LPARAM)(int)PInvoke.HTCLIENT);
                }
            }
        }
    }

    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    public new event EventHandler? TabStopChanged
    {
        add => base.TabStopChanged += value;
        remove => base.TabStopChanged -= value;
    }

    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    public new bool TabStop
    {
        get => base.TabStop;
        set => base.TabStop = value;
    }

    [RefreshProperties(RefreshProperties.Repaint)]
    [AllowNull]
    public override string Text
    {
        get => base.Text;
        set => base.Text = value;
    }

    [RefreshProperties(RefreshProperties.Repaint)]
    public new Padding Padding
    {
        get => base.Padding;
        set => base.Padding = value;
    }

    /// <summary>
    ///  Gets or sets the color used to display the link once it has been visited.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.LinkLabelVisitedLinkColorDescr))]
    public Color VisitedLinkColor
    {
        get => _visitedLinkColor.IsEmpty
            ? SystemInformation.HighContrast ? LinkUtilities.GetVisitedLinkColor() : IEVisitedLinkColor
            : _visitedLinkColor;
        set
        {
            if (_visitedLinkColor != value)
            {
                _visitedLinkColor = value;
                InvalidateLink(null);
            }
        }
    }

    /// <summary>
    ///  Occurs when the link is clicked.
    /// </summary>
    [WinCategory("Action")]
    [SRDescription(nameof(SR.LinkLabelLinkClickedDescr))]
    public event LinkLabelLinkClickedEventHandler? LinkClicked
    {
        add => Events.AddHandler(s_eventLinkClicked, value);
        remove => Events.RemoveHandler(s_eventLinkClicked, value);
    }

    internal static Rectangle CalcTextRenderBounds(Rectangle textRect, Rectangle clientRect, ContentAlignment align)
    {
        int xLoc, yLoc, width, height;

        if ((align & WindowsFormsUtils.AnyRightAlign) != 0)
        {
            xLoc = clientRect.Right - textRect.Width;
        }
        else if ((align & WindowsFormsUtils.AnyCenterAlign) != 0)
        {
            xLoc = (clientRect.Width - textRect.Width) / 2;
        }
        else
        {
            xLoc = clientRect.X;
        }

        if ((align & WindowsFormsUtils.AnyBottomAlign) != 0)
        {
            yLoc = clientRect.Bottom - textRect.Height;
        }
        else if ((align & WindowsFormsUtils.AnyMiddleAlign) != 0)
        {
            yLoc = (clientRect.Height - textRect.Height) / 2;
        }
        else
        {
            yLoc = clientRect.Y;
        }

        // If the text rect does not fit in the client rect, make it fit.
        if (textRect.Width > clientRect.Width)
        {
            xLoc = clientRect.X;
            width = clientRect.Width;
        }
        else
        {
            width = textRect.Width;
        }

        if (textRect.Height > clientRect.Height)
        {
            yLoc = clientRect.Y;
            height = clientRect.Height;
        }
        else
        {
            height = textRect.Height;
        }

        return new Rectangle(xLoc, yLoc, width, height);
    }

    protected override AccessibleObject CreateAccessibilityInstance() => new LinkLabelAccessibleObject(this);

    internal override void ReleaseUiaProvider(HWND handle)
    {
        base.ReleaseUiaProvider(handle);

        if (OsVersion.IsWindows8OrGreater())
        {
            foreach (Link link in _links)
            {
                if (link.IsAccessibilityObjectCreated)
                {
                    PInvoke.UiaDisconnectProvider(link.AccessibleObject, skipOSCheck: true);
                }
            }
        }
    }

    protected override void CreateHandle()
    {
        base.CreateHandle();
        InvalidateTextLayout();
    }

    internal override bool CanUseTextRenderer
    {
        get
        {
            // The Gdi library doesn't currently have a way to calculate character ranges so we cannot use it for
            // painting link(s) within the text, but if the link are is null or covers the entire text we are ok
            // since it is just one area with the same size of the text binding area.

            return LinkArea.Start == 0 && (LinkArea.IsEmpty || LinkArea.Length == new StringInfo(Text).LengthInTextElements);
        }
    }

    internal override bool UseGDIMeasuring() => !UseCompatibleTextRendering;

    /// <summary>
    ///  Converts the character index into char index of the string.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This method mainly deal with surrogate. Suppose we have a string consisting of 3 surrogates, and we want the
    ///   second character, then the index we need should be 2 instead of 1, and this method returns the correct index.
    ///  </para>
    /// </remarks>
    private static int ConvertToCharIndex(int index, string text)
    {
        // This method is copied in LinkCollectionEditor. Update the other one as well if you change this method.

        if (index <= 0)
        {
            return 0;
        }

        if (string.IsNullOrEmpty(text))
        {
            Debug.Assert(text is not null, "string should not be null");

            // Do no conversion, just return the original value passed in.
            return index;
        }

        // Dealing with surrogate characters in some languages, characters can expand over multiple
        // chars, using StringInfo lets us properly deal with it.
        StringInfo stringInfo = new(text);
        int numTextElements = stringInfo.LengthInTextElements;

        if (index > numTextElements)
        {
            // Pretend all the characters after are ASCII characters
            return index - numTextElements + text.Length;
        }

        // Return the length of the substring which has specified number of characters.
        string sub = stringInfo.SubstringByTextElements(0, index);
        return sub.Length;
    }

    /// <summary>
    ///  Ensures that we have analyzed the text run so that we can render each segment and link.
    /// </summary>
    private void EnsureRun(Graphics g)
    {
        Debug.Assert(g is not null);

        if (_textLayoutValid)
        {
            return;
        }

        _textRegion?.Dispose();
        _textRegion = null;

        string text = Text;

        if (text.Length == 0)
        {
            Links.Clear();
            Links.Add(new Link(0, -1));   // default 'magic' link.
            _textLayoutValid = true;
            return;
        }

        using StringFormat textFormat = CreateStringFormat();

        using Font alwaysUnderlined = new Font(Font, Font.Style | FontStyle.Underline);

        if (UseCompatibleTextRendering)
        {
            Region[] textRegions = g.MeasureCharacterRanges(text, alwaysUnderlined, ClientRectWithPadding, textFormat);

            int regionIndex = 0;

            for (int i = 0; i < Links.Count; i++)
            {
                Link link = Links[i];
                int charStart = ConvertToCharIndex(link.Start, text);
                int charEnd = ConvertToCharIndex(link.Start + link.Length, text);
                if (LinkInText(charStart, charEnd - charStart))
                {
                    Links[i].VisualRegion = textRegions[regionIndex];
                    regionIndex++;
                }
            }

            Debug.Assert(regionIndex == (textRegions.Length - 1), "Failed to consume all link label visual regions");
            _textRegion = textRegions[^1];
        }
        else
        {
            // Use TextRenderer.MeasureText to see the size of the text
            Rectangle clientRectWithPadding = ClientRectWithPadding;
            Size clientSize = new Size(clientRectWithPadding.Width, clientRectWithPadding.Height);
            TextFormatFlags flags = CreateTextFormatFlags(clientSize);
            Size textSize = TextRenderer.MeasureText(text, alwaysUnderlined, clientSize, flags);

            // We need to take into account the padding that GDI adds around the text.
            int iLeftMargin, iRightMargin;

            TextPaddingOptions padding = default;
            if ((flags & TextFormatFlags.NoPadding) == TextFormatFlags.NoPadding)
            {
                padding = TextPaddingOptions.NoPadding;
            }
            else if ((flags & TextFormatFlags.LeftAndRightPadding) == TextFormatFlags.LeftAndRightPadding)
            {
                padding = TextPaddingOptions.LeftAndRightPadding;
            }

            using var hfont = GdiCache.GetHFONT(Font);
            DRAWTEXTPARAMS dtParams = hfont.GetTextMargins(padding);

            iLeftMargin = dtParams.iLeftMargin;
            iRightMargin = dtParams.iRightMargin;

            Rectangle visualRectangle = new(
                clientRectWithPadding.X + iLeftMargin,
                clientRectWithPadding.Y,
                textSize.Width - iRightMargin - iLeftMargin,
                textSize.Height);

            visualRectangle = CalcTextRenderBounds(visualRectangle, clientRectWithPadding, RtlTranslateContent(TextAlign));

            Region visualRegion = new Region(visualRectangle);
            if (_links is not null && _links.Count == 1)
            {
                Links[0].VisualRegion = visualRegion;
            }

            _textRegion = visualRegion;
        }

        _textLayoutValid = true;
    }

    internal override StringFormat CreateStringFormat()
    {
        StringFormat stringFormat = base.CreateStringFormat();
        if (string.IsNullOrEmpty(Text))
        {
            return stringFormat;
        }

        CharacterRange[] regions = AdjustCharacterRangesForSurrogateChars();
        stringFormat.SetMeasurableCharacterRanges(regions);

        return stringFormat;
    }

    /// <summary>
    ///  Calculate character ranges taking into account the locale.  Provided for surrogate chars support.
    /// </summary>
    private CharacterRange[] AdjustCharacterRangesForSurrogateChars()
    {
        string text = Text;

        if (string.IsNullOrEmpty(text))
        {
            return Array.Empty<CharacterRange>();
        }

        StringInfo stringInfo = new StringInfo(text);
        int textLen = stringInfo.LengthInTextElements;
        List<CharacterRange> ranges = new List<CharacterRange>(Links.Count + 1);

        foreach (Link link in Links)
        {
            int charStart = ConvertToCharIndex(link.Start, text);
            int charEnd = ConvertToCharIndex(link.Start + link.Length, text);
            if (LinkInText(charStart, charEnd - charStart))
            {
                int length = Math.Min(link.Length, textLen - link.Start);
                ranges.Add(new CharacterRange(charStart, ConvertToCharIndex(link.Start + length, text) - charStart));
            }
        }

        ranges.Add(new CharacterRange(0, text.Length));

        return ranges.ToArray();
    }

    /// <summary>
    ///  Determines whether the whole link label contains only one link,
    ///  and the link runs from the beginning of the label to the end of it.
    /// </summary>
    private bool IsOneLink()
    {
        if (_links is null || _links.Count != 1 || Text is null)
        {
            return false;
        }

        StringInfo stringInfo = new StringInfo(Text);
        if (LinkArea.Start == 0 && LinkArea.Length == stringInfo.LengthInTextElements)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    ///  Determines if the given client coordinates is contained within a portion of a link area.
    /// </summary>
    protected Link? PointInLink(int x, int y)
    {
        using Graphics g = CreateGraphicsInternal();
        Link? hit = null;

        EnsureRun(g);
        foreach (Link link in _links)
        {
            if (link.VisualRegion is not null && link.VisualRegion.IsVisible(x, y, g))
            {
                hit = link;
                break;
            }
        }

        return hit;
    }

    /// <summary>
    ///  Invalidates only the portions of the text that is linked to the specified link. If link is null, then
    ///  all linked text is invalidated.
    /// </summary>
    private void InvalidateLink(Link? link)
    {
        if (IsHandleCreated)
        {
            if (link is null || link.VisualRegion is null || IsOneLink())
            {
                Invalidate();
            }
            else
            {
                Invalidate(link.VisualRegion);
            }
        }
    }

    /// <summary>
    ///  Invalidates the current set of fonts we use when painting links. The fonts will be recreated when needed.
    /// </summary>
    private void InvalidateLinkFonts()
    {
        _linkFont?.Dispose();

        if (_hoverLinkFont is not null && _hoverLinkFont != _linkFont)
        {
            _hoverLinkFont.Dispose();
        }

        _linkFont = null;
        _hoverLinkFont = null;
    }

    private void InvalidateTextLayout() => _textLayoutValid = false;

    private bool LinkInText(int start, int length) => start >= 0 && start < Text.Length && length > 0;

    /// <summary>
    ///  Gets or sets a value that is returned to the parent form when the link label is clicked.
    /// </summary>
    DialogResult IButtonControl.DialogResult
    {
        get => _dialogResult;
        set
        {
            // Valid values are 0x0 to 0x7
            SourceGenerated.EnumValidator.Validate(value);

            _dialogResult = value;
        }
    }

    void IButtonControl.NotifyDefault(bool value)
    {
    }

    protected override void OnGotFocus(EventArgs e)
    {
        if (!_processingOnGotFocus)
        {
            base.OnGotFocus(e);
            _processingOnGotFocus = true;
        }

        try
        {
            Link? focusLink = FocusLink;
            if (focusLink is null)
            {
                // Set focus on first link.
                // This will raise the OnGotFocus event again but it will not be processed because processingOnGotFocus is true.
                Select(directed: true, forward: true);
            }
            else
            {
                InvalidateLink(focusLink);
                UpdateAccessibilityLink(focusLink);
            }
        }
        finally
        {
            if (_processingOnGotFocus)
            {
                _processingOnGotFocus = false;
            }
        }
    }

    protected override void OnLostFocus(EventArgs e)
    {
        base.OnLostFocus(e);

        if (FocusLink is not null)
        {
            InvalidateLink(FocusLink);
        }
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (e.KeyCode == Keys.Enter)
        {
            if (FocusLink is not null && FocusLink.Enabled)
            {
                OnLinkClicked(new LinkLabelLinkClickedEventArgs(FocusLink));
            }
        }
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        if (!Enabled)
        {
            return;
        }

        foreach (Link link in _links)
        {
            if ((link.State & LinkState.Hover) == LinkState.Hover
                || (link.State & LinkState.Active) == LinkState.Active)
            {
                bool activeChanged = (link.State & LinkState.Active) == LinkState.Active;
                link.State &= ~(LinkState.Hover | LinkState.Active);

                if (activeChanged || _hoverLinkFont != _linkFont)
                {
                    InvalidateLink(link);
                }

                OverrideCursor = null;
            }
        }
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);

        if (!Enabled || e.Clicks > 1)
        {
            _receivedDoubleClick = true;
            return;
        }

        for (int i = 0; i < _links.Count; i++)
        {
            if ((_links[i].State & LinkState.Hover) == LinkState.Hover)
            {
                _links[i].State |= LinkState.Active;

                Focus();
                if (_links[i].Enabled)
                {
                    FocusLink = _links[i];
                    InvalidateLink(FocusLink);
                }

                Capture = true;
                break;
            }
        }
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);

        if (Disposing || IsDisposed)
        {
            return;
        }

        if (!Enabled || e.Clicks > 1 || _receivedDoubleClick)
        {
            _receivedDoubleClick = false;
            return;
        }

        for (int i = 0; i < _links.Count; i++)
        {
            if ((_links[i].State & LinkState.Active) == LinkState.Active)
            {
                _links[i].State &= ~LinkState.Active;
                InvalidateLink(_links[i]);
                Capture = false;

                Link? clicked = PointInLink(e.X, e.Y);

                if (clicked is not null && clicked == FocusLink && clicked.Enabled)
                {
                    OnLinkClicked(new LinkLabelLinkClickedEventArgs(clicked, e.Button));
                }
            }
        }
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);

        if (!Enabled)
        {
            return;
        }

        Link? hoverLink = null;
        foreach (Link link in _links)
        {
            if ((link.State & LinkState.Hover) == LinkState.Hover)
            {
                hoverLink = link;
                break;
            }
        }

        Link? pointIn = PointInLink(e.X, e.Y);

        if (pointIn != hoverLink)
        {
            if (hoverLink is not null)
            {
                hoverLink.State &= ~LinkState.Hover;
            }

            if (pointIn is not null)
            {
                pointIn.State |= LinkState.Hover;
                if (pointIn.Enabled)
                {
                    OverrideCursor = Cursors.Hand;
                }
            }
            else
            {
                OverrideCursor = null;
            }

            if (_hoverLinkFont != _linkFont)
            {
                if (hoverLink is not null)
                {
                    InvalidateLink(hoverLink);
                }

                if (pointIn is not null)
                {
                    InvalidateLink(pointIn);
                }
            }
        }
    }

    /// <summary>
    ///  Raises the <see cref="OnLinkClicked"/> event.
    /// </summary>
    protected virtual void OnLinkClicked(LinkLabelLinkClickedEventArgs e)
    {
        ((LinkLabelLinkClickedEventHandler?)Events[s_eventLinkClicked])?.Invoke(this, e);
    }

    protected override void OnPaddingChanged(EventArgs e)
    {
        base.OnPaddingChanged(e);
        InvalidateTextLayout();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        // The focus rectangle if there is only one link
        RectangleF finalrect = RectangleF.Empty;
        Animate();

        ImageAnimator.UpdateFrames(Image);

        Graphics g = e.GraphicsInternal;

        EnsureRun(g);

        if (Text.Length == 0)
        {
            PaintLinkBackground(g);
        }
        else
        {
            // Paint enabled link label
            if (AutoEllipsis)
            {
                Rectangle clientRect = ClientRectWithPadding;
                Size preferredSize = GetPreferredSize(new Size(clientRect.Width, clientRect.Height));
                _showToolTip = clientRect.Width < preferredSize.Width || clientRect.Height < preferredSize.Height;
            }
            else
            {
                _showToolTip = false;
            }

            if (Enabled)
            {
                // Control.Enabled not to be confused with Link.Enabled
                bool optimizeBackgroundRendering = !GetStyle(ControlStyles.OptimizedDoubleBuffer);
                using var foreBrush = ForeColor.GetCachedSolidBrushScope();
                using var linkBrush = LinkColor.GetCachedSolidBrushScope();

                if (!optimizeBackgroundRendering)
                {
                    PaintLinkBackground(g);
                }

                LinkUtilities.EnsureLinkFonts(Font, LinkBehavior, ref _linkFont, ref _hoverLinkFont);

                Region originalClip = g.Clip;

                try
                {
                    if (IsOneLink())
                    {
                        // Exclude the area to draw the focus rectangle.
                        g.Clip = originalClip;
                        RectangleF[]? rects = _links[0].VisualRegion?.GetRegionScans(e.GraphicsInternal.Transform);
                        if (rects is not null && rects.Length > 0)
                        {
                            if (UseCompatibleTextRendering)
                            {
                                finalrect = new RectangleF(rects[0].Location, SizeF.Empty);
                                foreach (RectangleF rect in rects)
                                {
                                    finalrect = RectangleF.Union(finalrect, rect);
                                }
                            }
                            else
                            {
                                finalrect = ClientRectWithPadding;
                                Size finalRectSize = finalrect.Size.ToSize();

                                Size requiredSize = MeasureTextCache.GetTextSize(Text, Font, finalRectSize, CreateTextFormatFlags(finalRectSize));

                                finalrect.Width = requiredSize.Width;

                                if (requiredSize.Height < finalrect.Height)
                                {
                                    finalrect.Height = requiredSize.Height;
                                }

                                finalrect = CalcTextRenderBounds(Rectangle.Round(finalrect), ClientRectWithPadding, RtlTranslateContent(TextAlign));
                            }

                            using (Region region = new Region(finalrect))
                            {
                                g.ExcludeClip(region);
                            }
                        }
                    }
                    else
                    {
                        foreach (Link link in _links)
                        {
                            if (link.VisualRegion is not null)
                            {
                                g.ExcludeClip(link.VisualRegion);
                            }
                        }
                    }

                    // When there is only one link in link label, it's not necessary to paint with foreBrush
                    // first as it will be overlapped by linkBrush in the following steps.

                    if (!IsOneLink())
                    {
                        PaintLink(
                            e,
                            link: null,
                            foreBrush,
                            linkBrush,
                            _linkFont,
                            _hoverLinkFont,
                            optimizeBackgroundRendering,
                            finalrect);
                    }

                    foreach (Link link in _links)
                    {
                        PaintLink(
                            e,
                            link,
                            foreBrush,
                            linkBrush,
                            _linkFont,
                            _hoverLinkFont,
                            optimizeBackgroundRendering,
                            finalrect);
                    }

                    if (optimizeBackgroundRendering)
                    {
                        g.Clip = originalClip;

                        // This shouldn't be null, but it would require significant refactoring to encapsulate.
                        g.ExcludeClip(_textRegion!);
                        PaintLinkBackground(g);
                    }
                }
                finally
                {
                    g.Clip = originalClip;
                }
            }
            else
            {
                // Paint disabled link label (disabled control, not to be confused with disabled link).
                Region originalClip = g.Clip;

                try
                {
                    // We need to paint the background first before clipping to textRegion because it is calculated using
                    // ClientRectWithPadding which in some cases is smaller that ClientRectangle.

                    PaintLinkBackground(g);

                    if (UseCompatibleTextRendering)
                    {
                        // The clipping only applies when rendering through GDI+. The region shouldn't be null, but
                        // it would require significant refactoring to encapsulate.
                        g.IntersectClip(_textRegion!);

                        StringFormat stringFormat = CreateStringFormat();
                        ControlPaint.DrawStringDisabled(g, Text, Font, DisabledColor, ClientRectWithPadding, stringFormat);
                    }
                    else
                    {
                        Color foreColor;
                        using (DeviceContextHdcScope scope = new(e, applyGraphicsState: false))
                        {
                            foreColor = scope.HDC.FindNearestColor(DisabledColor);
                        }

                        Rectangle clientRectWidthPadding = ClientRectWithPadding;

                        ControlPaint.DrawStringDisabled(
                            g,
                            Text,
                            Font,
                            foreColor,
                            clientRectWidthPadding,
                            CreateTextFormatFlags(clientRectWidthPadding.Size));
                    }
                }
                finally
                {
                    g.Clip = originalClip;
                }
            }
        }

        // We can't call base.OnPaint because labels paint differently from link labels,
        // but we still need to raise the Paint event.

        RaisePaintEvent(this, e);
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
        Image? i = Image;

        if (i is not null)
        {
            Region oldClip = e.Graphics.Clip;
            Rectangle imageBounds = CalcImageRenderBounds(i, ClientRectangle, RtlTranslateAlignment(ImageAlign));
            e.Graphics.ExcludeClip(imageBounds);
            try
            {
                base.OnPaintBackground(e);
            }
            finally
            {
                e.Graphics.Clip = oldClip;
            }

            e.Graphics.IntersectClip(imageBounds);
            try
            {
                base.OnPaintBackground(e);
                DrawImage(e.Graphics, i, ClientRectangle, RtlTranslateAlignment(ImageAlign));
            }
            finally
            {
                e.Graphics.Clip = oldClip;
            }
        }
        else
        {
            base.OnPaintBackground(e);
        }
    }

    protected override void OnFontChanged(EventArgs e)
    {
        base.OnFontChanged(e);
        InvalidateTextLayout();
        InvalidateLinkFonts();
        Invalidate();
    }

    protected override void OnAutoSizeChanged(EventArgs e)
    {
        base.OnAutoSizeChanged(e);
        InvalidateTextLayout();
    }

    internal override void OnAutoEllipsisChanged()
    {
        base.OnAutoEllipsisChanged();
        InvalidateTextLayout();
    }

    protected override void OnEnabledChanged(EventArgs e)
    {
        base.OnEnabledChanged(e);

        if (!Enabled)
        {
            for (int i = 0; i < _links.Count; i++)
            {
                _links[i].State &= ~(LinkState.Hover | LinkState.Active);
            }

            OverrideCursor = null;
        }

        InvalidateTextLayout();
        Invalidate();
    }

    protected override void OnTextChanged(EventArgs e)
    {
        base.OnTextChanged(e);
        InvalidateTextLayout();
        UpdateSelectability();
    }

    protected override void OnTextAlignChanged(EventArgs e)
    {
        base.OnTextAlignChanged(e);
        InvalidateTextLayout();
        UpdateSelectability();
    }

    private void PaintLink(
        PaintEventArgs e,
        Link? link,
        SolidBrush foreBrush,
        SolidBrush linkBrush,
        Font linkFont,
        Font hoverLinkFont,
        bool optimizeBackgroundRendering,
        RectangleF finalrect)
    {
        Graphics g = e.GraphicsInternal;

        Debug.Assert(g is not null, "Must pass valid graphics");
        Debug.Assert(foreBrush is not null, "Must pass valid foreBrush");
        Debug.Assert(linkBrush is not null, "Must pass valid linkBrush");

        Font font = Font;

        if (link is not null)
        {
            if (link.VisualRegion is null)
            {
                // Don't paint anything if we are given a link with no visual region.
                return;
            }

            Color brushColor = Color.Empty;
            LinkState linkState = link.State;

            font = (linkState & LinkState.Hover) == LinkState.Hover ? hoverLinkFont : linkFont;

            if (link.Enabled)
            {
                // Not to be confused with Control.Enabled.
                if ((linkState & LinkState.Active) == LinkState.Active)
                {
                    brushColor = ActiveLinkColor;
                }
                else if ((linkState & LinkState.Visited) == LinkState.Visited)
                {
                    brushColor = VisitedLinkColor;
                }
            }
            else
            {
                brushColor = DisabledLinkColor;
            }

            g.Clip = IsOneLink() ? new Region(finalrect) : link.VisualRegion;

            if (optimizeBackgroundRendering)
            {
                PaintLinkBackground(g);
            }

            if (brushColor == Color.Empty)
            {
                brushColor = linkBrush.Color;
            }

            if (UseCompatibleTextRendering)
            {
                using var useBrush = brushColor.GetCachedSolidBrushScope();
                StringFormat stringFormat = CreateStringFormat();
                g.DrawString(Text, font, useBrush, ClientRectWithPadding, stringFormat);
            }
            else
            {
                brushColor = g.FindNearestColor(brushColor);

                Rectangle clientRectWithPadding = ClientRectWithPadding;

#pragma warning disable SA1009 // Closing parenthesis should be spaced correctly
                TextRenderer.DrawText(
                    g,
                    Text,
                    font,
                    clientRectWithPadding,
                    brushColor,
                    CreateTextFormatFlags(clientRectWithPadding.Size)
#if DEBUG
                    // Skip the asserts in TextRenderer because the DC has been modified
                    | TextRenderer.SkipAssertFlag
#endif
                    );
#pragma warning restore SA1009
            }

            if (Focused && ShowFocusCues && FocusLink == link)
            {
                // Get the rectangles making up the visual region, and draw each one.
                RectangleF[] rects = link.VisualRegion.GetRegionScans(g.Transform);
                if (rects is not null && rects.Length > 0)
                {
                    Rectangle focusRect;

                    if (IsOneLink())
                    {
                        // Draw one merged focus rectangle
                        focusRect = Rectangle.Ceiling(finalrect);
                        Debug.Assert(finalrect != RectangleF.Empty, "finalrect should be initialized");

                        ControlPaint.DrawFocusRectangle(g, focusRect, ForeColor, BackColor);
                    }
                    else
                    {
                        foreach (RectangleF rect in rects)
                        {
                            ControlPaint.DrawFocusRectangle(g, Rectangle.Ceiling(rect), ForeColor, BackColor);
                        }
                    }
                }
            }

            return;
        }

        // Painting with no link.
        // Because the code has been like that since long time, we assume that _textRegion is not null.
        g.IntersectClip(_textRegion!);

        if (optimizeBackgroundRendering)
        {
            PaintLinkBackground(g);
        }

        if (UseCompatibleTextRendering)
        {
            StringFormat stringFormat = CreateStringFormat();
            g.DrawString(Text, font, foreBrush, ClientRectWithPadding, stringFormat);
        }
        else
        {
            Color color;
            using (DeviceContextHdcScope hdc = new(g, applyGraphicsState: false))
            {
                color = ColorTranslator.FromWin32(
                    (int)PInvoke.GetNearestColor(hdc, (COLORREF)(uint)ColorTranslator.ToWin32(foreBrush.Color)).Value);
            }

            Rectangle clientRectWithPadding = ClientRectWithPadding;
            TextRenderer.DrawText(
                g,
                Text,
                font,
                clientRectWithPadding,
                color,
                CreateTextFormatFlags(clientRectWithPadding.Size));
        }
    }

    private void PaintLinkBackground(Graphics g)
    {
        using (PaintEventArgs e = new PaintEventArgs(g, ClientRectangle))
        {
            InvokePaintBackground(this, e);
        }
    }

    void IButtonControl.PerformClick()
    {
        // If a link is not currently focused, focus on the first link.
        if (FocusLink is null && Links.Count > 0)
        {
            string text = Text;
            foreach (Link link in Links)
            {
                int charStart = ConvertToCharIndex(link.Start, text);
                int charEnd = ConvertToCharIndex(link.Start + link.Length, text);
                if (link.Enabled && LinkInText(charStart, charEnd - charStart))
                {
                    FocusLink = link;
                    break;
                }
            }
        }

        // Act as if the focused link was clicked.
        if (FocusLink is not null)
        {
            OnLinkClicked(new LinkLabelLinkClickedEventArgs(FocusLink));
        }
    }

    protected override bool ProcessDialogKey(Keys keyData)
    {
        if ((keyData & (Keys.Alt | Keys.Control)) != Keys.Alt)
        {
            Keys keyCode = keyData & Keys.KeyCode;
            switch (keyCode)
            {
                case Keys.Tab:
                    if (TabStop)
                    {
                        bool forward = (keyData & Keys.Shift) != Keys.Shift;
                        if (FocusNextLink(forward))
                        {
                            return true;
                        }
                    }

                    break;
                case Keys.Up:
                case Keys.Left:
                    if (FocusNextLink(false))
                    {
                        return true;
                    }

                    break;
                case Keys.Down:
                case Keys.Right:
                    if (FocusNextLink(true))
                    {
                        return true;
                    }

                    break;
            }
        }

        return base.ProcessDialogKey(keyData);
    }

    private bool FocusNextLink(bool forward)
    {
        int focusIndex = -1;
        if (_focusLink is not null)
        {
            for (int i = 0; i < _links.Count; i++)
            {
                if (_links[i] == _focusLink)
                {
                    focusIndex = i;
                    break;
                }
            }
        }

        focusIndex = GetNextLinkIndex(focusIndex, forward);
        if (focusIndex != -1)
        {
            FocusLink = Links[focusIndex];
            return true;
        }
        else
        {
            FocusLink = null;
            return false;
        }
    }

    private int GetNextLinkIndex(int focusIndex, bool forward)
    {
        Link? test;
        string text = Text;
        int charStart = 0;
        int charEnd = 0;

        if (forward)
        {
            do
            {
                focusIndex++;

                if (focusIndex < Links.Count)
                {
                    test = Links[focusIndex];
                    charStart = ConvertToCharIndex(test.Start, text);
                    charEnd = ConvertToCharIndex(test.Start + test.Length, text);
                }
                else
                {
                    test = null;
                }
            }
            while (test is not null
                && !test.Enabled
                && LinkInText(charStart, charEnd - charStart));
        }
        else
        {
            do
            {
                focusIndex--;
                if (focusIndex >= 0)
                {
                    test = Links[focusIndex];
                    charStart = ConvertToCharIndex(test.Start, text);
                    charEnd = ConvertToCharIndex(test.Start + test.Length, text);
                }
                else
                {
                    test = null;
                }
            }
            while (test is not null
                && !test.Enabled
                && LinkInText(charStart, charEnd - charStart));
        }

        return focusIndex < 0 || focusIndex >= _links.Count ? -1 : focusIndex;
    }

    private void ResetLinkArea() => LinkArea = new LinkArea(0, -1);

    internal void ResetActiveLinkColor() => _activeLinkColor = Color.Empty;

    internal void ResetDisabledLinkColor() => _disabledLinkColor = Color.Empty;

    internal void ResetLinkColor()
    {
        _linkColor = Color.Empty;
        InvalidateLink(null);
    }

    private void ResetVisitedLinkColor() => _visitedLinkColor = Color.Empty;

    protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
    {
        // We cache too much state to try and optimize this (regions, etc). It is best to always relayout here.
        // If we want to resurrect this code in the future, remember that we need to handle a word wrapped top
        // aligned text that will become newly exposed (and therefore layed out) when we resize.
        InvalidateTextLayout();
        Invalidate();

        base.SetBoundsCore(x, y, width, height, specified);
    }

    protected override void Select(bool directed, bool forward)
    {
        // In a multi-link label, if the tab came from another control, we want to keep the currently
        // focused link, otherwise, we set the focus to the next link.
        if (directed && _links.Count > 0)
        {
            // Find which link is currently focused
            int focusIndex = -1;
            if (FocusLink is not null)
            {
                focusIndex = _links.IndexOf(FocusLink);
            }

            // We could be getting focus from ourself, so we must invalidate each time.
            FocusLink = null;

            int newFocus = GetNextLinkIndex(focusIndex, forward);
            if (newFocus == -1)
            {
                if (forward)
                {
                    // -1, so "next" will be 0
                    newFocus = GetNextLinkIndex(-1, forward);
                }
                else
                {
                    // Count, so "next" will be Count-1
                    newFocus = GetNextLinkIndex(_links.Count, forward);
                }
            }

            if (newFocus != -1)
            {
                FocusLink = _links[newFocus];
            }
        }

        base.Select(directed, forward);
    }

    /// <summary>
    ///  Determines if the color for active links should remain the same.
    /// </summary>
    internal bool ShouldSerializeActiveLinkColor() => !_activeLinkColor.IsEmpty;

    /// <summary>
    ///  Determines if the color for disabled links should remain the same.
    /// </summary>
    internal bool ShouldSerializeDisabledLinkColor() => !_disabledLinkColor.IsEmpty;

    /// <summary>
    ///  Determines if the range in text that is treated as a link should remain the same.
    /// </summary>
    private bool ShouldSerializeLinkArea()
    {
        if (_links.Count == 1)
        {
            // use field access to find out if "length" is really -1
            return Links[0].Start != 0 || Links[0]._length != -1;
        }

        return true;
    }

    /// <summary>
    ///  Determines if the color of links in normal cases should remain the same.
    /// </summary>
    internal bool ShouldSerializeLinkColor() => !_linkColor.IsEmpty;

    /// <summary>
    ///  Determines whether designer should generate code for setting the UseCompatibleTextRendering or not.
    ///  DefaultValue(false)
    /// </summary>
    private bool ShouldSerializeUseCompatibleTextRendering()
    {
        // Serialize code if LinkLabel cannot support the feature or the property's value is  not the default.
        return !CanUseTextRenderer || UseCompatibleTextRendering != Control.UseCompatibleTextRenderingDefault;
    }

    /// <summary>
    ///  Determines if the color of links that have been visited should remain the same.
    /// </summary>
    private bool ShouldSerializeVisitedLinkColor() => !_visitedLinkColor.IsEmpty;

    /// <summary>
    ///  Update accessibility with the currently focused link.
    /// </summary>
    private void UpdateAccessibilityLink(Link focusLink)
    {
        if (!IsHandleCreated)
        {
            return;
        }

        int focusIndex = -1;
        for (int i = 0; i < _links.Count; i++)
        {
            if (_links[i] == focusLink)
            {
                focusIndex = i;
            }
        }

        AccessibilityNotifyClients(AccessibleEvents.Focus, focusIndex);

        if (IsAccessibilityObjectCreated)
        {
            focusLink.AccessibleObject?.RaiseAutomationEvent(UIA_EVENT_ID.UIA_AutomationFocusChangedEventId);
        }
    }

    /// <summary>
    ///  Validates that no links overlap. This will throw an exception if they do.
    /// </summary>
    private void ValidateNoOverlappingLinks()
    {
        for (int x = 0; x < _links.Count; x++)
        {
            Link left = _links[x];
            if (left.Length < 0)
            {
                throw new InvalidOperationException(SR.LinkLabelOverlap);
            }

            for (int y = x; y < _links.Count; y++)
            {
                if (x != y)
                {
                    Link right = _links[y];
                    int maxStart = Math.Max(left.Start, right.Start);
                    int minEnd = Math.Min(left.Start + left.Length, right.Start + right.Length);
                    if (maxStart < minEnd)
                    {
                        throw new InvalidOperationException(SR.LinkLabelOverlap);
                    }
                }
            }
        }
    }

    /// <summary>
    ///  Updates the label's ability to get focus. If there are any links in the label, then the label can get
    ///  focus, else it can't.
    /// </summary>
    private void UpdateSelectability()
    {
        LinkArea pt = LinkArea;
        bool selectable = false;
        string text = Text;
        int charStart = ConvertToCharIndex(pt.Start, text);
        int charEnd = ConvertToCharIndex(pt.Start + pt.Length, text);

        if (LinkInText(charStart, charEnd - charStart))
        {
            selectable = true;
        }
        else
        {
            // If a link is currently focused, de-select it
            if (FocusLink is not null)
            {
                FocusLink = null;
            }
        }

        OverrideCursor = null;
        TabStop = selectable;
        SetStyle(ControlStyles.Selectable, selectable);
    }

    [RefreshProperties(RefreshProperties.Repaint)]
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.UseCompatibleTextRenderingDescr))]
    public new bool UseCompatibleTextRendering
    {
        get
        {
            Debug.Assert(CanUseTextRenderer || base.UseCompatibleTextRendering, "Using GDI text rendering when CanUseTextRenderer reported false.");
            return base.UseCompatibleTextRendering;
        }
        set
        {
            if (base.UseCompatibleTextRendering != value)
            {
                // Cache the value so it is restored if CanUseTextRenderer becomes true and the designer can undo changes to this as side effect.
                base.UseCompatibleTextRendering = value;
                InvalidateTextLayout();
            }
        }
    }

    internal override bool SupportsUiaProviders => true;

    /// <summary>
    ///  Handles the WM_SETCURSOR message.
    /// </summary>
    private void WmSetCursor(ref Message m)
    {
        // Accessing through the Handle property has side effects that break this logic. You must use InternalHandle.
        if ((HWND)m.WParamInternal == InternalHandle && m.LParamInternal.LOWORD == PInvoke.HTCLIENT)
        {
            Cursor.Current = OverrideCursor ?? Cursor;
        }
        else
        {
            DefWndProc(ref m);
        }
    }

    protected override void WndProc(ref Message msg)
    {
        switch (msg.MsgInternal)
        {
            case PInvoke.WM_SETCURSOR:
                WmSetCursor(ref msg);
                break;
            default:
                base.WndProc(ref msg);
                break;
        }
    }
}
