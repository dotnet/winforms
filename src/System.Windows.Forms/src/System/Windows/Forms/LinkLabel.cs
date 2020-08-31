// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Windows.Forms.Internal;
using System.Windows.Forms.Layout;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Displays text that can contain a hyperlink.
    /// </summary>
    [DefaultEvent(nameof(LinkClicked))]
    [ToolboxItem("System.Windows.Forms.Design.AutoSizeToolboxItem," + AssemblyRef.SystemDesign)]
    [SRDescription(nameof(SR.DescriptionLinkLabel))]
    public class LinkLabel : Label, IButtonControl
    {
        private static readonly object s_eventLinkClicked = new object();
        private static Color s_iedisabledLinkColor = Color.Empty;

        private static readonly LinkComparer s_linkComparer = new LinkComparer();

        private DialogResult _dialogResult;

        private Color _linkColor = Color.Empty;
        private Color _activeLinkColor = Color.Empty;
        private Color _visitedLinkColor = Color.Empty;
        private Color _disabledLinkColor = Color.Empty;

        private Font _linkFont;
        private Font _hoverLinkFont;

        private bool _textLayoutValid;
        private bool _receivedDoubleClick;
        private readonly ArrayList _links = new ArrayList(2);

        private Link _focusLink;
        private LinkCollection _linkCollection;
        private Region _textRegion;
        private Cursor _overrideCursor;

        private bool _processingOnGotFocus;  // used to avoid raising the OnGotFocus event twice after selecting a focus link.

        private LinkBehavior _linkBehavior = LinkBehavior.SystemDefault;

        /// <summary>
        ///  Initializes a new default instance of the <see cref='LinkLabel'/> class.
        /// </summary>
        public LinkLabel() : base()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint
                     | ControlStyles.OptimizedDoubleBuffer
                     | ControlStyles.Opaque
                     | ControlStyles.UserPaint
                     | ControlStyles.StandardClick
                     | ControlStyles.ResizeRedraw, true);
            ResetLinkArea();
        }

        /// <summary>
        ///  Gets or sets the color used to display active links.
        /// </summary>
        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.LinkLabelActiveLinkColorDescr))]
        public Color ActiveLinkColor
        {
            get
            {
                if (_activeLinkColor.IsEmpty)
                {
                    return IEActiveLinkColor;
                }
                else
                {
                    return _activeLinkColor;
                }
            }
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
            get
            {
                if (_disabledLinkColor.IsEmpty)
                {
                    return IEDisabledLinkColor;
                }
                else
                {
                    return _disabledLinkColor;
                }
            }
            set
            {
                if (_disabledLinkColor != value)
                {
                    _disabledLinkColor = value;
                    InvalidateLink(null);
                }
            }
        }

        private Link FocusLink
        {
            get
            {
                return _focusLink;
            }
            set
            {
                if (_focusLink != value)
                {
                    if (_focusLink != null)
                    {
                        InvalidateLink(_focusLink);
                    }

                    _focusLink = value;

                    if (_focusLink != null)
                    {
                        InvalidateLink(_focusLink);

                        UpdateAccessibilityLink(_focusLink);
                    }
                }
            }
        }

        private Color IELinkColor
        {
            get
            {
                return LinkUtilities.IELinkColor;
            }
        }

        private Color IEActiveLinkColor
        {
            get
            {
                return LinkUtilities.IEActiveLinkColor;
            }
        }
        private Color IEVisitedLinkColor
        {
            get
            {
                return LinkUtilities.IEVisitedLinkColor;
            }
        }
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

        private Rectangle ClientRectWithPadding
        {
            get
            {
                return LayoutUtils.DeflateRect(ClientRectangle, Padding);
            }
        }

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
        [Editor("System.Windows.Forms.Design.LinkAreaEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Localizable(true)]
        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.LinkLabelLinkAreaDescr))]
        public LinkArea LinkArea
        {
            get
            {
                if (_links.Count == 0)
                {
                    return new LinkArea(0, 0);
                }
                return new LinkArea(((Link)_links[0]).Start, ((Link)_links[0]).Length);
            }
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

                    if (value.Start != 0 || value.Length != 0)
                    {
                        Links.Add(new Link(this));

                        // Update the link area of the first link
                        //
                        ((Link)_links[0]).Start = value.Start;
                        ((Link)_links[0]).Length = value.Length;
                    }
                }

                UpdateSelectability();

                if (!pt.Equals(LinkArea))
                {
                    InvalidateTextLayout();
                    LayoutTransaction.DoLayout(ParentInternal, this, PropertyNames.LinkArea);
                    base.AdjustSize();
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
            get
            {
                return _linkBehavior;
            }
            set
            {
                //valid values are 0x0 to 0x3
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)LinkBehavior.SystemDefault, (int)LinkBehavior.NeverUnderline))
                {
                    throw new InvalidEnumArgumentException(nameof(LinkBehavior), (int)value, typeof(LinkBehavior));
                }
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
            get
            {
                if (_linkColor.IsEmpty)
                {
                    if (SystemInformation.HighContrast)
                    {
                        return SystemColors.HotTrack;
                    }
                    return IELinkColor;
                }
                else
                {
                    return _linkColor;
                }
            }
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
        ///  Gets the collection of links used in a <see cref='LinkLabel'/>.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public LinkCollection Links
        {
            get
            {
                if (_linkCollection is null)
                {
                    _linkCollection = new LinkCollection(this);
                }
                return _linkCollection;
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the link should be displayed as if it was visited.
        /// </summary>
        [DefaultValue(false)]
        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.LinkLabelLinkVisitedDescr))]
        public bool LinkVisited
        {
            get
            {
                if (_links.Count == 0)
                {
                    return false;
                }
                else
                {
                    return ((Link)_links[0]).Visited;
                }
            }
            set
            {
                if (value != LinkVisited)
                {
                    if (_links.Count == 0)
                    {
                        Links.Add(new Link(this));
                    }
                    ((Link)_links[0]).Visited = value;
                }
            }
        }

        // link labels must always ownerdraw
        //
        internal override bool OwnerDraw
        {
            get
            {
                return true;
            }
        }

        protected Cursor OverrideCursor
        {
            get
            {
                return _overrideCursor;
            }
            set
            {
                if (_overrideCursor != value)
                {
                    _overrideCursor = value;

                    if (IsHandleCreated)
                    {
                        // We want to instantly change the cursor if the mouse is within our bounds.
                        // This includes the case where the mouse is over one of our children
                        var r = new RECT();
                        User32.GetCursorPos(out Point p);
                        User32.GetWindowRect(this, ref r);
                        if ((r.left <= p.X && p.X < r.right && r.top <= p.Y && p.Y < r.bottom) || User32.GetCapture() == Handle)
                        {
                            User32.SendMessageW(this, User32.WM.SETCURSOR, Handle, (IntPtr)User32.HT.CLIENT);
                        }
                    }
                }
            }
        }

        // Make this event visible through the property browser.
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        new public event EventHandler TabStopChanged
        {
            add => base.TabStopChanged += value;
            remove => base.TabStopChanged -= value;
        }

        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        new public bool TabStop
        {
            get => base.TabStop;
            set => base.TabStop = value;
        }

        [RefreshProperties(RefreshProperties.Repaint)]
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
            get
            {
                if (_visitedLinkColor.IsEmpty)
                {
                    if (SystemInformation.HighContrast)
                    {
                        return LinkUtilities.GetVisitedLinkColor();
                    }
                    return IEVisitedLinkColor;
                }
                else
                {
                    return _visitedLinkColor;
                }
            }
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
        public event LinkLabelLinkClickedEventHandler LinkClicked
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

        /// <summary>
        ///  Constructs the new instance of the accessibility object for this control. Subclasses
        ///  should not call base.CreateAccessibilityObject.
        /// </summary>
        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new LinkLabelAccessibleObject(this);
        }

        /// <summary>
        ///  Creates a handle for this control. This method is called by the framework,
        ///  this should not be called directly. Inheriting classes should always call
        ///  <c>base.CreateHandle</c> when overriding this method.
        /// </summary>
        protected override void CreateHandle()
        {
            base.CreateHandle();
            InvalidateTextLayout();
        }

        /// <summary>
        ///  Determines whether the current state of the control allows for rendering text using
        ///  TextRenderer (GDI).
        ///  The Gdi library doesn't currently have a way to calculate character ranges so we cannot
        ///  use it for painting link(s) within the text, but if the link are is null or covers the
        ///  entire text we are ok since it is just one area with the same size of the text binding
        ///  area.
        /// </summary>
        internal override bool CanUseTextRenderer
        {
            get
            {
                // If no link or the LinkArea is one and covers the entire text, we can support UseCompatibleTextRendering = false.
                // Observe that LinkArea refers to the first link always.
                StringInfo stringInfo = new StringInfo(Text);
                return LinkArea.Start == 0 && (LinkArea.Length == 0 || LinkArea.Length == stringInfo.LengthInTextElements);
            }
        }

        internal override bool UseGDIMeasuring()
        {
            return !UseCompatibleTextRendering;
        }

        /// <summary>
        ///  Converts the character index into char index of the string
        ///  This method is copied in LinkCollectionEditor.cs. Update the other
        ///  one as well if you change this method.
        ///  This method mainly deal with surrogate. Suppose we
        ///  have a string consisting of 3 surrogates, and we want the
        ///  second character, then the index we need should be 2 instead of
        ///  1, and this method returns the correct index.
        /// </summary>
        private static int ConvertToCharIndex(int index, string text)
        {
            if (index <= 0)
            {
                return 0;
            }
            if (string.IsNullOrEmpty(text))
            {
                Debug.Assert(text != null, "string should not be null");
                //do no conversion, just return the original value passed in
                return index;
            }

            //Dealing with surrogate characters
            //in some languages, characters can expand over multiple
            //chars, using StringInfo lets us properly deal with it.
            StringInfo stringInfo = new StringInfo(text);
            int numTextElements = stringInfo.LengthInTextElements;

            //index is greater than the length of the string
            if (index > numTextElements)
            {
                return index - numTextElements + text.Length;  //pretend all the characters after are ASCII characters
            }
            //return the length of the substring which has specified number of characters
            string sub = stringInfo.SubstringByTextElements(0, index);
            return sub.Length;
        }

        /// <summary>
        ///  Ensures that we have analyzed the text run so that we can render each segment
        ///  and link.
        /// </summary>
        private void EnsureRun(Graphics g)
        {
            // bail early if everything is valid!
            if (_textLayoutValid)
            {
                return;
            }

            if (_textRegion != null)
            {
                _textRegion.Dispose();
                _textRegion = null;
            }

            // bail early for no text
            //
            if (Text.Length == 0)
            {
                Links.Clear();
                Links.Add(new Link(0, -1));   // default 'magic' link.
                _textLayoutValid = true;
                return;
            }

            StringFormat textFormat = CreateStringFormat();
            string text = Text;
            try
            {
                Font alwaysUnderlined = new Font(Font, Font.Style | FontStyle.Underline);
                Graphics created = null;

                try
                {
                    if (g is null)
                    {
                        g = created = CreateGraphicsInternal();
                    }

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
                        _textRegion = textRegions[textRegions.Length - 1];
                    }
                    else
                    {
                        // use TextRenderer.MeasureText to see the size of the text
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
                        User32.DRAWTEXTPARAMS dtParams = hfont.GetTextMargins(padding);

                        iLeftMargin = dtParams.iLeftMargin;
                        iRightMargin = dtParams.iRightMargin;

                        Rectangle visualRectangle = new Rectangle(clientRectWithPadding.X + iLeftMargin,
                                                                  clientRectWithPadding.Y,
                                                                  textSize.Width - iRightMargin - iLeftMargin,
                                                                  textSize.Height);
                        visualRectangle = CalcTextRenderBounds(visualRectangle /*textRect*/, clientRectWithPadding /*clientRect*/, RtlTranslateContent(TextAlign));
                        //

                        Region visualRegion = new Region(visualRectangle);
                        if (_links != null && _links.Count == 1)
                        {
                            Links[0].VisualRegion = visualRegion;
                        }
                        _textRegion = visualRegion;
                    }
                }
                finally
                {
                    alwaysUnderlined.Dispose();

                    if (created != null)
                    {
                        created.Dispose();
                    }
                }

                _textLayoutValid = true;
            }
            finally
            {
                textFormat.Dispose();
            }
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
            ArrayList ranges = new ArrayList(Links.Count);

            foreach (Link link in Links)
            {
                int charStart = ConvertToCharIndex(link.Start, text);
                int charEnd = ConvertToCharIndex(link.Start + link.Length, text);
                if (LinkInText(charStart, charEnd - charStart))
                {
                    int length = (int)Math.Min(link.Length, textLen - link.Start);
                    ranges.Add(new CharacterRange(charStart, ConvertToCharIndex(link.Start + length, text) - charStart));
                }
            }

            CharacterRange[] regions = new CharacterRange[ranges.Count + 1];
            ranges.CopyTo(regions, 0);
            regions[regions.Length - 1] = new CharacterRange(0, text.Length);

            return regions;
        }

        /// <summary>
        ///  Determines whether the whole link label contains only one link,
        ///  and the link runs from the beginning of the label to the end of it
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
        ///  Determines if the given client coordinates is contained within a portion
        ///  of a link area.
        /// </summary>
        protected Link PointInLink(int x, int y)
        {
            Graphics g = CreateGraphicsInternal();
            Link hit = null;
            try
            {
                EnsureRun(g);
                foreach (Link link in _links)
                {
                    if (link.VisualRegion != null && link.VisualRegion.IsVisible(x, y, g))
                    {
                        hit = link;
                        break;
                    }
                }
            }
            finally
            {
                g.Dispose();
                g = null;
            }
            return hit;
        }

        /// <summary>
        ///  Invalidates only the portions of the text that is linked to
        ///  the specified link. If link is null, then all linked text
        ///  is invalidated.
        /// </summary>
        private void InvalidateLink(Link link)
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
        ///  Invalidates the current set of fonts we use when painting
        ///  links.  The fonts will be recreated when needed.
        /// </summary>
        private void InvalidateLinkFonts()
        {
            if (_linkFont != null)
            {
                _linkFont.Dispose();
            }

            if (_hoverLinkFont != null && _hoverLinkFont != _linkFont)
            {
                _hoverLinkFont.Dispose();
            }

            _linkFont = null;
            _hoverLinkFont = null;
        }

        private void InvalidateTextLayout()
        {
            _textLayoutValid = false;
        }

        private bool LinkInText(int start, int length)
        {
            return (0 <= start && start < Text.Length && 0 < length);
        }

        /// <summary>
        ///  Gets or sets a value that is returned to the
        ///  parent form when the link label.
        ///  is clicked.
        /// </summary>
        DialogResult IButtonControl.DialogResult
        {
            get
            {
                return _dialogResult;
            }

            set
            {
                //valid values are 0x0 to 0x7
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)DialogResult.None, (int)DialogResult.No))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(DialogResult));
                }

                _dialogResult = value;
            }
        }

        void IButtonControl.NotifyDefault(bool value)
        {
        }

        /// <summary>
        ///  Raises the <see cref='Control.GotFocus'/> event.
        /// </summary>
        protected override void OnGotFocus(EventArgs e)
        {
            if (!_processingOnGotFocus)
            {
                base.OnGotFocus(e);
                _processingOnGotFocus = true;
            }

            try
            {
                Link focusLink = FocusLink;
                if (focusLink is null)
                {
                    // Set focus on first link.
                    // This will raise the OnGotFocus event again but it will not be processed because processingOnGotFocus is true.
                    Select(true /*directed*/, true /*forward*/);
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

        /// <summary>
        ///  Raises the <see cref='Control.LostFocus'/>
        ///  event.
        /// </summary>
        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);

            if (FocusLink != null)
            {
                InvalidateLink(FocusLink);
            }
        }

        /// <summary>
        ///  Raises the <see cref='Control.OnKeyDown'/>
        ///  event.
        /// </summary>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.KeyCode == Keys.Enter)
            {
                if (FocusLink != null && FocusLink.Enabled)
                {
                    OnLinkClicked(new LinkLabelLinkClickedEventArgs(FocusLink));
                }
            }
        }

        /// <summary>
        ///  Raises the <see cref='Control.OnMouseLeave'/>
        ///  event.
        /// </summary>
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

        /// <summary>
        ///  Raises the <see cref='Control.OnMouseDown'/>
        ///  event.
        /// </summary>
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
                if ((((Link)_links[i]).State & LinkState.Hover) == LinkState.Hover)
                {
                    ((Link)_links[i]).State |= LinkState.Active;

                    Focus();
                    if (((Link)_links[i]).Enabled)
                    {
                        FocusLink = (Link)_links[i];
                        InvalidateLink(FocusLink);
                    }
                    Capture = true;
                    break;
                }
            }
        }

        /// <summary>
        ///  Raises the <see cref='Control.OnMouseUp'/>
        ///  event.
        /// </summary>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            //
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
                if ((((Link)_links[i]).State & LinkState.Active) == LinkState.Active)
                {
                    ((Link)_links[i]).State &= (~LinkState.Active);
                    InvalidateLink((Link)_links[i]);
                    Capture = false;

                    Link clicked = PointInLink(e.X, e.Y);

                    if (clicked != null && clicked == FocusLink && clicked.Enabled)
                    {
                        OnLinkClicked(new LinkLabelLinkClickedEventArgs(clicked, e.Button));
                    }
                }
            }
        }

        /// <summary>
        ///  Raises the <see cref='Control.OnMouseMove'/>
        ///  event.
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (!Enabled)
            {
                return;
            }

            Link hoverLink = null;
            foreach (Link link in _links)
            {
                if ((link.State & LinkState.Hover) == LinkState.Hover)
                {
                    hoverLink = link;
                    break;
                }
            }

            Link pointIn = PointInLink(e.X, e.Y);

            if (pointIn != hoverLink)
            {
                if (hoverLink != null)
                {
                    hoverLink.State &= ~LinkState.Hover;
                }
                if (pointIn != null)
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
                    if (hoverLink != null)
                    {
                        InvalidateLink(hoverLink);
                    }
                    if (pointIn != null)
                    {
                        InvalidateLink(pointIn);
                    }
                }
            }
        }

        /// <summary>
        ///  Raises the <see cref='OnLinkClicked'/> event.
        /// </summary>
        protected virtual void OnLinkClicked(LinkLabelLinkClickedEventArgs e)
        {
            ((LinkLabelLinkClickedEventHandler)Events[s_eventLinkClicked])?.Invoke(this, e);
        }

        protected override void OnPaddingChanged(EventArgs e)
        {
            base.OnPaddingChanged(e);
            InvalidateTextLayout();
        }

        /// <summary>
        ///  Raises the <see cref='Control.OnPaint'/>
        ///  event.
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            RectangleF finalrect = RectangleF.Empty;   //the focus rectangle if there is only one link
            Animate();

            ImageAnimator.UpdateFrames(Image);

            Graphics g = e.GraphicsInternal;

            EnsureRun(g);

            if (Text.Length == 0)
            {
                // bail early for no text
                PaintLinkBackground(g);
            }
            else
            {
                // Paint enabled link label
                if (AutoEllipsis)
                {
                    Rectangle clientRect = ClientRectWithPadding;
                    Size preferredSize = GetPreferredSize(new Size(clientRect.Width, clientRect.Height));
                    _showToolTip = (clientRect.Width < preferredSize.Width || clientRect.Height < preferredSize.Height);
                }
                else
                {
                    _showToolTip = false;
                }

                if (Enabled)
                {
                    // Control.Enabled not to be confused with Link.Enabled
                    bool optimizeBackgroundRendering = !GetStyle(ControlStyles.OptimizedDoubleBuffer);
                    var foreBrush = ForeColor.GetCachedSolidBrushScope();
                    var linkBrush = LinkColor.GetCachedSolidBrushScope();

                    try
                    {
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
                                //exclude the area to draw the focus rectangle
                                g.Clip = originalClip;
                                RectangleF[] rects = ((Link)_links[0]).VisualRegion.GetRegionScans(e.GraphicsInternal.Transform);
                                if (rects != null && rects.Length > 0)
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
                                        finalrect = CalcTextRenderBounds(Rectangle.Round(finalrect) /*textRect*/, ClientRectWithPadding /*clientRect*/, RtlTranslateContent(TextAlign));
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
                                    if (link.VisualRegion != null)
                                    {
                                        g.ExcludeClip(link.VisualRegion);
                                    }
                                }
                            }

                            // When there is only one link in link label,
                            // it's not necessary to paint with forebrush first
                            // as it will be overlapped by linkbrush in the following steps

                            if (!IsOneLink())
                            {
                                PaintLink(e, null, foreBrush, linkBrush, optimizeBackgroundRendering, finalrect);
                            }

                            foreach (Link link in _links)
                            {
                                PaintLink(e, link, foreBrush, linkBrush, optimizeBackgroundRendering, finalrect);
                            }

                            if (optimizeBackgroundRendering)
                            {
                                g.Clip = originalClip;
                                g.ExcludeClip(_textRegion);
                                PaintLinkBackground(g);
                            }
                        }
                        finally
                        {
                            g.Clip = originalClip;
                        }
                    }
                    finally
                    {
                        foreBrush.Dispose();
                        linkBrush.Dispose();
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
                        g.IntersectClip(_textRegion);

                        if (UseCompatibleTextRendering)
                        {
                            // APPCOMPAT: Use DisabledColor because Everett used DisabledColor.
                            // (ie, dont use Graphics.GetNearestColor(DisabledColor.)
                            StringFormat stringFormat = CreateStringFormat();
                            ControlPaint.DrawStringDisabled(g, Text, Font, DisabledColor, ClientRectWithPadding, stringFormat);
                        }
                        else
                        {
                            Color foreColor;
                            using (var scope = new DeviceContextHdcScope(e, applyGraphicsState: false))
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
            Image i = Image;

            if (i != null)
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

        /// <summary>
        ///  Overriden by LinkLabel.
        /// </summary>
        internal override void OnAutoEllipsisChanged()
        {
            base.OnAutoEllipsisChanged(/*e*/);
            InvalidateTextLayout();
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);

            if (!Enabled)
            {
                for (int i = 0; i < _links.Count; i++)
                {
                    ((Link)_links[i]).State &= ~(LinkState.Hover | LinkState.Active);
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
            Link link,
            SolidBrush foreBrush,
            SolidBrush linkBrush,
            bool optimizeBackgroundRendering,
            RectangleF finalrect)
        {
            // link = null means paint the whole text
            Graphics g = e.GraphicsInternal;

            Debug.Assert(g != null, "Must pass valid graphics");
            Debug.Assert(foreBrush != null, "Must pass valid foreBrush");
            Debug.Assert(linkBrush != null, "Must pass valid linkBrush");

            Font font = Font;

            if (link != null)
            {
                if (link.VisualRegion != null)
                {
                    Color brushColor = Color.Empty;
                    LinkState linkState = link.State;

                    font = (linkState & LinkState.Hover) == LinkState.Hover ? _hoverLinkFont : _linkFont;

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
                        TextRenderer.DrawText(
                            g,
                            Text,
                            font,
                            clientRectWithPadding,
                            brushColor,
                            CreateTextFormatFlags(clientRectWithPadding.Size));
                    }

                    if (Focused && ShowFocusCues && FocusLink == link)
                    {
                        // Get the rectangles making up the visual region, and draw each one.
                        RectangleF[] rects = link.VisualRegion.GetRegionScans(g.Transform);
                        if (rects != null && rects.Length > 0)
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
                }

                // no else clause... we don't paint anything if we are given a link with no visual region.
            }
            else
            {
                // Painting with no link.
                g.IntersectClip(_textRegion);

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
                    using (var hdc = new DeviceContextHdcScope(g, applyGraphicsState: false))
                    {
                        color = ColorTranslator.FromWin32(
                            Gdi32.GetNearestColor(hdc, ColorTranslator.ToWin32(foreBrush.Color)));
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
            // If a link is not currently focused, focus on the first link
            //
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

            // Act as if the focused link was clicked
            //
            if (FocusLink != null)
            {
                OnLinkClicked(new LinkLabelLinkClickedEventArgs(FocusLink));
            }
        }

        /// <summary>
        ///  Processes a dialog key. This method is called during message pre-processing
        ///  to handle dialog characters, such as TAB, RETURN, ESCAPE, and arrow keys. This
        ///  method is called only if the isInputKey() method indicates that the control
        ///  isn't interested in the key. processDialogKey() simply sends the character to
        ///  the parent's processDialogKey() method, or returns false if the control has no
        ///  parent. The Form class overrides this method to perform actual processing
        ///  of dialog keys. When overriding processDialogKey(), a control should return true
        ///  to indicate that it has processed the key. For keys that aren't processed by the
        ///  control, the result of "base.processDialogChar()" should be returned. Controls
        ///  will seldom, if ever, need to override this method.
        /// </summary>
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
            if (_focusLink != null)
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
            Link test;
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
                } while (test != null
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
                } while (test != null
                         && !test.Enabled
                         && LinkInText(charStart, charEnd - charStart));
            }

            if (focusIndex < 0 || focusIndex >= _links.Count)
            {
                return -1;
            }
            else
            {
                return focusIndex;
            }
        }

        private void ResetLinkArea()
        {
            LinkArea = new LinkArea(0, -1);
        }

        internal void ResetActiveLinkColor()
        {
            _activeLinkColor = Color.Empty;
        }

        internal void ResetDisabledLinkColor()
        {
            _disabledLinkColor = Color.Empty;
        }

        internal void ResetLinkColor()
        {
            _linkColor = Color.Empty;
            InvalidateLink(null);
        }

        private void ResetVisitedLinkColor()
        {
            _visitedLinkColor = Color.Empty;
        }

        /// <summary>
        ///  Performs the work of setting the bounds of this control. Inheriting classes
        ///  can overide this function to add size restrictions. Inheriting classes must call
        ///  base.setBoundsCore to actually cause the bounds of the control to change.
        /// </summary>
        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            // we cache too much state to try and optimize this (regions, etc)... it is best
            // to always relayout here... If we want to resurect this code in the future,
            // remember that we need to handle a word wrapped top aligned text that
            // will become newly exposed (and therefore layed out) when we resize...
            InvalidateTextLayout();
            Invalidate();

            base.SetBoundsCore(x, y, width, height, specified);
        }

        protected override void Select(bool directed, bool forward)
        {
            if (directed)
            {
                // In a multi-link label, if the tab came from another control, we want to keep the currently
                // focused link, otherwise, we set the focus to the next link.
                if (_links.Count > 0)
                {
                    // Find which link is currently focused
                    //
                    int focusIndex = -1;
                    if (FocusLink != null)
                    {
                        focusIndex = _links.IndexOf(FocusLink);
                    }

                    // We could be getting focus from ourself, so we must
                    // invalidate each time.
                    //
                    FocusLink = null;

                    int newFocus = GetNextLinkIndex(focusIndex, forward);
                    if (newFocus == -1)
                    {
                        if (forward)
                        {
                            newFocus = GetNextLinkIndex(-1, forward); // -1, so "next" will be 0
                        }
                        else
                        {
                            newFocus = GetNextLinkIndex(_links.Count, forward); // Count, so "next" will be Count-1
                        }
                    }

                    if (newFocus != -1)
                    {
                        FocusLink = (Link)_links[newFocus];
                    }
                }
            }
            base.Select(directed, forward);
        }

        /// <summary>
        ///  Determines if the color for active links should remain the same.
        /// </summary>
        internal bool ShouldSerializeActiveLinkColor()
        {
            return !_activeLinkColor.IsEmpty;
        }

        /// <summary>
        ///  Determines if the color for disabled links should remain the same.
        /// </summary>
        internal bool ShouldSerializeDisabledLinkColor()
        {
            return !_disabledLinkColor.IsEmpty;
        }

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
        internal bool ShouldSerializeLinkColor()
        {
            return !_linkColor.IsEmpty;
        }

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
        private bool ShouldSerializeVisitedLinkColor()
        {
            return !_visitedLinkColor.IsEmpty;
        }

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
        }

        /// <summary>
        ///  Validates that no links overlap. This will throw an exception if they do.
        /// </summary>
        private void ValidateNoOverlappingLinks()
        {
            for (int x = 0; x < _links.Count; x++)
            {
                Link left = (Link)_links[x];
                if (left.Length < 0)
                {
                    throw new InvalidOperationException(SR.LinkLabelOverlap);
                }

                for (int y = x; y < _links.Count; y++)
                {
                    if (x != y)
                    {
                        Link right = (Link)_links[y];
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
                //
                if (FocusLink != null)
                {
                    FocusLink = null;
                }
            }

            OverrideCursor = null;
            TabStop = selectable;
            SetStyle(ControlStyles.Selectable, selectable);
        }

        /// <summary>
        ///  Determines whether to use compatible text rendering engine (GDI+) or not (GDI).
        /// </summary>
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

        internal override bool SupportsUiaProviders => false;

        /// <summary>
        ///  Handles the WM_SETCURSOR message
        /// </summary>
        private void WmSetCursor(ref Message m)
        {
            // Accessing through the Handle property has side effects that break this
            // logic. You must use InternalHandle.
            //
            if (m.WParam == InternalHandle && PARAM.LOWORD(m.LParam) == (int)User32.HT.CLIENT)
            {
                if (OverrideCursor != null)
                {
                    Cursor.Current = OverrideCursor;
                }
                else
                {
                    Cursor.Current = Cursor;
                }
            }
            else
            {
                DefWndProc(ref m);
            }
        }

        protected override void WndProc(ref Message msg)
        {
            switch ((User32.WM)msg.Msg)
            {
                case User32.WM.SETCURSOR:
                    WmSetCursor(ref msg);
                    break;
                default:
                    base.WndProc(ref msg);
                    break;
            }
        }

        public class LinkCollection : IList
        {
            private readonly LinkLabel _owner;

            ///  A caching mechanism for key accessor
            ///  We use an index here rather than control so that we don't have lifetime
            ///  issues by holding on to extra references.
            ///  Note this is not Thread Safe - but WinForms has to be run in a STA anyways.
            private int lastAccessedIndex = -1;

            public LinkCollection(LinkLabel owner)
            {
                _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            }

            public virtual Link this[int index]
            {
                get
                {
                    return (Link)_owner._links[index];
                }
                set
                {
                    _owner._links[index] = value;

                    _owner._links.Sort(s_linkComparer);

                    _owner.InvalidateTextLayout();
                    _owner.Invalidate();
                }
            }

            object IList.this[int index]
            {
                get => this[index];
                set
                {
                    if (value is Link link)
                    {
                        this[index] = link;
                    }
                    else
                    {
                        throw new ArgumentException(SR.LinkLabelBadLink, nameof(value));
                    }
                }
            }

            /// <summary>
            ///  Retrieves the child control with the specified key.
            /// </summary>
            public virtual Link this[string key]
            {
                get
                {
                    // We do not support null and empty string as valid keys.
                    if (string.IsNullOrEmpty(key))
                    {
                        return null;
                    }

                    // Search for the key in our collection
                    int index = IndexOfKey(key);
                    if (IsValidIndex(index))
                    {
                        return this[index];
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            [Browsable(false)]
            public int Count => _owner._links.Count;

            /// <summary>
            ///  whether we have added a non-trivial link to the collection
            /// </summary>
            public bool LinksAdded { get; private set; }

            object ICollection.SyncRoot => this;

            bool ICollection.IsSynchronized => false;

            bool IList.IsFixedSize => false;

            public bool IsReadOnly => false;

            public Link Add(int start, int length)
            {
                if (length != 0)
                {
                    LinksAdded = true;
                }

                return Add(start, length, null);
            }

            public Link Add(int start, int length, object linkData)
            {
                if (length != 0)
                {
                    LinksAdded = true;
                }

                // check for the special case where the list is in the "magic"
                // state of having only the default link in it. In that case
                // we want to clear the list before adding this link.

                if (_owner._links.Count == 1
                    && this[0].Start == 0
                    && this[0]._length == -1)
                {
                    _owner._links.Clear();
                    _owner.FocusLink = null;
                }

                Link l = new Link(_owner)
                {
                    Start = start,
                    Length = length,
                    LinkData = linkData
                };
                Add(l);
                return l;
            }

            public int Add(Link value)
            {
                if (value != null && value.Length != 0)
                {
                    LinksAdded = true;
                }
                // check for the special case where the list is in the "magic"
                // state of having only the default link in it. In that case
                // we want to clear the list before adding this link.
                //
                if (_owner._links.Count == 1
                    && this[0].Start == 0
                    && this[0]._length == -1)
                {
                    _owner._links.Clear();
                    _owner.FocusLink = null;
                }

                // Set the owner control for this link
                value.Owner = _owner;

                _owner._links.Add(value);

                if (_owner.AutoSize)
                {
                    LayoutTransaction.DoLayout(_owner.ParentInternal, _owner, PropertyNames.Links);
                    _owner.AdjustSize();
                    _owner.Invalidate();
                }

                if (_owner.Links.Count > 1)
                {
                    _owner._links.Sort(LinkLabel.s_linkComparer);
                }

                _owner.ValidateNoOverlappingLinks();
                _owner.UpdateSelectability();
                _owner.InvalidateTextLayout();
                _owner.Invalidate();

                if (_owner.Links.Count > 1)
                {
                    return IndexOf(value);
                }
                else
                {
                    return 0;
                }
            }

            int IList.Add(object value)
            {
                if (value is Link)
                {
                    return Add((Link)value);
                }
                else
                {
                    throw new ArgumentException(SR.LinkLabelBadLink, nameof(value));
                }
            }

            void IList.Insert(int index, object value)
            {
                if (value is Link)
                {
                    Add((Link)value);
                }
                else
                {
                    throw new ArgumentException(SR.LinkLabelBadLink, nameof(value));
                }
            }

            public bool Contains(Link link)
            {
                return _owner._links.Contains(link);
            }

            /// <summary>
            ///  Returns true if the collection contains an item with the specified key, false otherwise.
            /// </summary>
            public virtual bool ContainsKey(string key)
            {
                return IsValidIndex(IndexOfKey(key));
            }

            bool IList.Contains(object link)
            {
                if (link is Link)
                {
                    return Contains((Link)link);
                }
                else
                {
                    return false;
                }
            }

            public int IndexOf(Link link)
            {
                return _owner._links.IndexOf(link);
            }

            int IList.IndexOf(object link)
            {
                if (link is Link)
                {
                    return IndexOf((Link)link);
                }
                else
                {
                    return -1;
                }
            }

            /// <summary>
            ///  The zero-based index of the first occurrence of value within the entire CollectionBase, if found; otherwise, -1.
            /// </summary>
            public virtual int IndexOfKey(string key)
            {
                // Step 0 - Arg validation
                if (string.IsNullOrEmpty(key))
                {
                    return -1; // we dont support empty or null keys.
                }

                // step 1 - check the last cached item
                if (IsValidIndex(lastAccessedIndex))
                {
                    if (WindowsFormsUtils.SafeCompareStrings(this[lastAccessedIndex].Name, key, /* ignoreCase = */ true))
                    {
                        return lastAccessedIndex;
                    }
                }

                // step 2 - search for the item
                for (int i = 0; i < Count; i++)
                {
                    if (WindowsFormsUtils.SafeCompareStrings(this[i].Name, key, /* ignoreCase = */ true))
                    {
                        lastAccessedIndex = i;
                        return i;
                    }
                }

                // step 3 - we didn't find it.  Invalidate the last accessed index and return -1.
                lastAccessedIndex = -1;
                return -1;
            }

            /// <summary>
            ///  Determines if the index is valid for the collection.
            /// </summary>
            private bool IsValidIndex(int index)
            {
                return ((index >= 0) && (index < Count));
            }

            /// <summary>
            ///  Remove all links from the linkLabel.
            /// </summary>
            public virtual void Clear()
            {
                bool doLayout = _owner._links.Count > 0 && _owner.AutoSize;
                _owner._links.Clear();

                if (doLayout)
                {
                    LayoutTransaction.DoLayout(_owner.ParentInternal, _owner, PropertyNames.Links);
                    _owner.AdjustSize();
                    _owner.Invalidate();
                }

                _owner.UpdateSelectability();
                _owner.InvalidateTextLayout();
                _owner.Invalidate();
            }

            void ICollection.CopyTo(Array dest, int index)
            {
                _owner._links.CopyTo(dest, index);
            }

            public IEnumerator GetEnumerator()
            {
                if (_owner._links != null)
                {
                    return _owner._links.GetEnumerator();
                }
                else
                {
                    return Array.Empty<Link>().GetEnumerator();
                }
            }

            public void Remove(Link value)
            {
                if (value.Owner != _owner)
                {
                    return;
                }

                _owner._links.Remove(value);

                if (_owner.AutoSize)
                {
                    LayoutTransaction.DoLayout(_owner.ParentInternal, _owner, PropertyNames.Links);
                    _owner.AdjustSize();
                    _owner.Invalidate();
                }

                _owner._links.Sort(LinkLabel.s_linkComparer);

                _owner.ValidateNoOverlappingLinks();
                _owner.UpdateSelectability();
                _owner.InvalidateTextLayout();
                _owner.Invalidate();

                if (_owner.FocusLink is null && _owner._links.Count > 0)
                {
                    _owner.FocusLink = (Link)_owner._links[0];
                }
            }

            public void RemoveAt(int index)
            {
                Remove(this[index]);
            }

            /// <summary>
            ///  Removes the child control with the specified key.
            /// </summary>
            public virtual void RemoveByKey(string key)
            {
                int index = IndexOfKey(key);
                if (IsValidIndex(index))
                {
                    RemoveAt(index);
                }
            }

            void IList.Remove(object value)
            {
                if (value is Link)
                {
                    Remove((Link)value);
                }
            }
        }

        [TypeConverter(typeof(LinkConverter))]
        public class Link
        {
            private int _start;
            private bool _enabled = true;
            internal int _length;
            private string _name;

            public Link()
            {
            }

            public Link(int start, int length)
            {
                _start = start;
                _length = length;
            }

            public Link(int start, int length, object linkData)
            {
                _start = start;
                _length = length;
                LinkData = linkData;
            }

            internal Link(LinkLabel owner)
            {
                Owner = owner;
            }

            /// <summary>
            ///  Description for accessibility
            /// </summary>
            public string Description { get; set; }

            [DefaultValue(true)]
            public bool Enabled
            {
                get => _enabled;
                set
                {
                    if (_enabled != value)
                    {
                        _enabled = value;

                        if ((int)(State & (LinkState.Hover | LinkState.Active)) != 0)
                        {
                            State &= ~(LinkState.Hover | LinkState.Active);
                            if (Owner != null)
                            {
                                Owner.OverrideCursor = null;
                            }
                        }

                        Owner?.InvalidateLink(this);
                    }
                }
            }

            public int Length
            {
                get
                {
                    if (_length == -1)
                    {
                        if (Owner != null && !string.IsNullOrEmpty(Owner.Text))
                        {
                            StringInfo stringInfo = new StringInfo(Owner.Text);
                            return stringInfo.LengthInTextElements - Start;
                        }
                        else
                        {
                            return 0;
                        }
                    }

                    return _length;
                }
                set
                {
                    if (_length != value)
                    {
                        _length = value;
                        if (Owner != null)
                        {
                            Owner.InvalidateTextLayout();
                            Owner.Invalidate();
                        }
                    }
                }
            }

            [DefaultValue(null)]
            public object LinkData { get; set; }

            /// <summary>
            ///  The LinkLabel object that owns this link.
            /// </summary>
            internal LinkLabel Owner { get; set; }

            internal LinkState State { get; set; } = LinkState.Normal;

            /// <summary>
            ///  The name for the link - useful for indexing by key.
            /// </summary>
            [DefaultValue("")]
            [SRCategory(nameof(SR.CatAppearance))]
            [SRDescription(nameof(SR.TreeNodeNodeNameDescr))]
            public string Name
            {
                get => _name ?? string.Empty;
                set => _name = value;
            }

            public int Start
            {
                get => _start;
                set
                {
                    if (_start != value)
                    {
                        _start = value;

                        if (Owner != null)
                        {
                            Owner._links.Sort(LinkLabel.s_linkComparer);
                            Owner.InvalidateTextLayout();
                            Owner.Invalidate();
                        }
                    }
                }
            }

            [SRCategory(nameof(SR.CatData))]
            [Localizable(false)]
            [Bindable(true)]
            [SRDescription(nameof(SR.ControlTagDescr))]
            [DefaultValue(null)]
            [TypeConverter(typeof(StringConverter))]
            public object Tag { get; set; }

            [DefaultValue(false)]
            public bool Visited
            {
                get => (State & LinkState.Visited) == LinkState.Visited;
                set
                {
                    if (value != Visited)
                    {
                        if (value)
                        {
                            State |= LinkState.Visited;
                        }
                        else
                        {
                            State &= ~LinkState.Visited;
                        }

                        Owner?.InvalidateLink(this);
                    }
                }
            }

            internal Region VisualRegion { get; set; }
        }

        private class LinkComparer : IComparer
        {
            int IComparer.Compare(object link1, object link2)
            {
                Debug.Assert(link1 != null && link2 != null, "Null objects sent for comparison");

                int pos1 = ((Link)link1).Start;
                int pos2 = ((Link)link2).Start;

                return pos1 - pos2;
            }
        }

        internal class LinkLabelAccessibleObject : LabelAccessibleObject
        {
            public LinkLabelAccessibleObject(LinkLabel owner) : base(owner)
            {
            }

            internal override bool IsIAccessibleExSupported() => true;

            public override AccessibleObject GetChild(int index)
            {
                if (index >= 0 && index < ((LinkLabel)Owner).Links.Count)
                {
                    return new LinkAccessibleObject(((LinkLabel)Owner).Links[index]);
                }
                else
                {
                    return null;
                }
            }

            internal override object GetPropertyValue(UiaCore.UIA propertyID)
            {
                if (propertyID == UiaCore.UIA.IsEnabledPropertyId)
                {
                    if (!Owner.Enabled)
                    {
                        return false;
                    }
                }

                return base.GetPropertyValue(propertyID);
            }

            public override AccessibleObject HitTest(int x, int y)
            {
                if (!Owner.IsHandleCreated)
                {
                    return null;
                }

                Point p = Owner.PointToClient(new Point(x, y));
                Link hit = ((LinkLabel)Owner).PointInLink(p.X, p.Y);

                if (hit != null)
                {
                    return new LinkAccessibleObject(hit);
                }

                if (Bounds.Contains(x, y))
                {
                    return this;
                }

                return null;
            }

            /// <summary>
            /// </summary>
            public override int GetChildCount()
            {
                return ((LinkLabel)Owner).Links.Count;
            }
        }

        internal class LinkAccessibleObject : AccessibleObject
        {
            private readonly Link _link;

            public LinkAccessibleObject(Link link) : base()
            {
                _link = link;
            }

            public override Rectangle Bounds
            {
                get
                {
                    if (!_link.Owner.IsHandleCreated)
                    {
                        return Rectangle.Empty;
                    }

                    Region region = _link.VisualRegion;
                    Graphics g = Graphics.FromHwnd(_link.Owner.Handle);

                    // Make sure we have a region for this link
                    //
                    if (region is null)
                    {
                        _link.Owner.EnsureRun(g);
                        region = _link.VisualRegion;
                        if (region is null)
                        {
                            g.Dispose();
                            return Rectangle.Empty;
                        }
                    }

                    Rectangle rect;
                    try
                    {
                        rect = Rectangle.Ceiling(region.GetBounds(g));
                    }
                    finally
                    {
                        g.Dispose();
                    }

                    // Translate rect to screen coordinates
                    //
                    return _link.Owner.RectangleToScreen(rect);
                }
            }

            public override string DefaultAction
            {
                get
                {
                    return SR.AccessibleActionClick;
                }
            }

            public override string Description
            {
                get
                {
                    return _link.Description;
                }
            }

            public override string Name
            {
                get
                {
                    string text = _link.Owner.Text;
                    string name;

                    // return the full name of the link label
                    // as sometimes the link name in isolation
                    // is unusable when using a screen reader
                    name = text;
                    if (_link.Owner.UseMnemonic)
                    {
                        name = WindowsFormsUtils.TextWithoutMnemonics(name);
                    }

                    return name;
                }
                set => base.Name = value;
            }

            public override AccessibleObject Parent
            {
                get
                {
                    return _link.Owner.AccessibilityObject;
                }
            }

            public override AccessibleRole Role
            {
                get
                {
                    return AccessibleRole.Link;
                }
            }

            public override AccessibleStates State
            {
                get
                {
                    AccessibleStates state = AccessibleStates.Focusable;

                    // Selected state
                    //
                    if (_link.Owner.FocusLink == _link)
                    {
                        state |= AccessibleStates.Focused;
                    }

                    return state;
                }
            }

            public override string Value
            {
                get
                {
                    // Narrator announces Link's text twice, once as a Name property and once as a Value, thus removing value.
                    // Value is optional for this role (Link).
                    return string.Empty;
                }
            }

            public override void DoDefaultAction()
            {
                _link.Owner.OnLinkClicked(new LinkLabelLinkClickedEventArgs(_link));
            }

            internal override bool IsIAccessibleExSupported() => true;

            internal override object GetPropertyValue(UiaCore.UIA propertyID)
            {
                if (propertyID == UiaCore.UIA.IsEnabledPropertyId)
                {
                    if (!_link.Owner.Enabled)
                    {
                        return false;
                    }
                }

                return base.GetPropertyValue(propertyID);
            }
        }
    }
}
