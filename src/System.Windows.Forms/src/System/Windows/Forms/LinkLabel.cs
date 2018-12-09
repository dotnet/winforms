// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using Microsoft.Win32;
    using System.Collections;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing.Design;
    using System.Drawing.Drawing2D;
    using System.Drawing.Text;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting;
    using System.Runtime.Serialization.Formatters;
    using System.Security;
    using System.Security.Permissions;
    using System.Text;
    using System.Windows.Forms.ComponentModel;
    using System.Globalization;
    using System.Windows.Forms;
    using System.Windows.Forms.Layout;
    using System.Windows.Forms.Internal;
    using System;
    using System.Runtime.Versioning;
    
    /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Displays text that can contain a hyperlink.
    ///    </para>
    /// </devdoc>
    [
    ComVisible(true),
    ClassInterface(ClassInterfaceType.AutoDispatch),
    DefaultEvent(nameof(LinkClicked)),
    ToolboxItem("System.Windows.Forms.Design.AutoSizeToolboxItem," + AssemblyRef.SystemDesign),
    SRDescription(nameof(SR.DescriptionLinkLabel))
    ]
    public class LinkLabel : Label, IButtonControl {

        static readonly object EventLinkClicked = new object();
        static Color iedisabledLinkColor = Color.Empty;

        static LinkComparer linkComparer = new LinkComparer();

        /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.dialogResult"]/*' />
        /// <devdoc>
        ///     The dialog result that will be sent to the parent dialog form when
        ///     we are clicked.
        /// </devdoc>
        DialogResult dialogResult;

        Color linkColor = Color.Empty;
        Color activeLinkColor = Color.Empty;
        Color visitedLinkColor = Color.Empty;
        Color disabledLinkColor = Color.Empty;

        Font linkFont;
        Font hoverLinkFont;

        bool textLayoutValid = false;
        bool receivedDoubleClick = false;
        
        ArrayList links = new ArrayList(2);

        Link focusLink = null;
        LinkCollection linkCollection = null;
        Region textRegion = null;
        Cursor overrideCursor = null;

        bool processingOnGotFocus;  // used to avoid raising the OnGotFocus event twice after selecting a focus link.
        
        LinkBehavior linkBehavior = System.Windows.Forms.LinkBehavior.SystemDefault; 

        /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.LinkLabel"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Initializes a new default instance of the <see cref='System.Windows.Forms.LinkLabel'/> class.
        ///    </para>
        /// </devdoc>
        public LinkLabel() : base() {
            SetStyle(ControlStyles.AllPaintingInWmPaint
                     | ControlStyles.OptimizedDoubleBuffer
                     | ControlStyles.Opaque
                     | ControlStyles.UserPaint
                     | ControlStyles.StandardClick
                     | ControlStyles.ResizeRedraw, true);
            ResetLinkArea();
        }

        /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.ActiveLinkColor"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the color used to display active links.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.LinkLabelActiveLinkColorDescr))
        ]
        public Color ActiveLinkColor {
            get {
                if (activeLinkColor.IsEmpty) {
                    return IEActiveLinkColor;
                }
                else {
                    return activeLinkColor;
                }
            }
            set {
                if (activeLinkColor != value) {
                    activeLinkColor = value;
                    InvalidateLink(null);
                }
            }
        }

        /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.DisabledLinkColor"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the color used to display disabled links.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.LinkLabelDisabledLinkColorDescr))
        ]
        public Color DisabledLinkColor {
            get {
                if (disabledLinkColor.IsEmpty) {
                    return IEDisabledLinkColor;
                }
                else {
                    return disabledLinkColor;
                }
            }
            set {
                if (disabledLinkColor != value) {
                    disabledLinkColor = value;
                    InvalidateLink(null);
                }
            }
        }

        private Link FocusLink {
            get {
                return focusLink;
            }
            set {
                if (focusLink != value) {

                    if (focusLink != null) {
                        InvalidateLink(focusLink);
                    }

                    focusLink = value;
                    
                    if (focusLink != null) {
                        InvalidateLink(focusLink);

                        UpdateAccessibilityLink(focusLink);
                        
                    }
                }
            }
        }
        
        private Color IELinkColor {
            get {
                return LinkUtilities.IELinkColor;
            }
        }

        private Color IEActiveLinkColor {
            get {
                return LinkUtilities.IEActiveLinkColor;
            }
        }
        private Color IEVisitedLinkColor {
            get {
                return LinkUtilities.IEVisitedLinkColor;
            }
        }
        private Color IEDisabledLinkColor {
            get {
                if (iedisabledLinkColor.IsEmpty) {
                    iedisabledLinkColor = ControlPaint.Dark(DisabledColor);
                }
                return iedisabledLinkColor;
            }
        }

        private Rectangle ClientRectWithPadding {
            get {
                return LayoutUtils.DeflateRect(ClientRectangle, Padding);
            }
        }

        /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.FlatStyle"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new FlatStyle FlatStyle
        {
            get
            {
                return base.FlatStyle;
            }
            set
            {
                base.FlatStyle = value;
            }
        }

        /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.LinkArea"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the range in the text that is treated as a link.
        ///    </para>
        /// </devdoc>
        [
        Editor("System.Windows.Forms.Design.LinkAreaEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor)),
        RefreshProperties(RefreshProperties.Repaint),
        Localizable(true),
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.LinkLabelLinkAreaDescr))
        ]
        public LinkArea LinkArea {
            get {
                if (links.Count == 0) {
                    return new LinkArea(0, 0);
                }
                return new LinkArea(((Link)links[0]).Start, ((Link)links[0]).Length);
            }
            set {
                LinkArea pt = LinkArea;

                links.Clear();

                if (!value.IsEmpty) {
                    if (value.Start < 0) {
                        throw new ArgumentOutOfRangeException(nameof(LinkArea), value, SR.LinkLabelAreaStart);
                    }
                    if (value.Length < -1) {
                        throw new ArgumentOutOfRangeException(nameof(LinkArea), value, SR.LinkLabelAreaLength);
                    }

                    if (value.Start != 0 || value.Length != 0) {
                        Links.Add(new Link(this));
                        
                        // Update the link area of the first link
                        //
                        ((Link)links[0]).Start = value.Start;
                        ((Link)links[0]).Length = value.Length;
                    }
                }

                UpdateSelectability();

                if (!pt.Equals(LinkArea)) {
                    InvalidateTextLayout();
                    LayoutTransaction.DoLayout(ParentInternal, this, PropertyNames.LinkArea);
                    base.AdjustSize();
                    Invalidate();
                }
            }
        }

        /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.LinkBehavior"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets ir sets a value that represents how the link will be underlined.
        ///    </para>
        /// </devdoc>
        [
        DefaultValue(LinkBehavior.SystemDefault),
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.LinkLabelLinkBehaviorDescr))
        ]
        public LinkBehavior LinkBehavior {
            get {
                return linkBehavior;
            }
            set {
                //valid values are 0x0 to 0x3
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)LinkBehavior.SystemDefault, (int)LinkBehavior.NeverUnderline)){
                    throw new InvalidEnumArgumentException(nameof(LinkBehavior), (int)value, typeof(LinkBehavior));
                }
                if (value != linkBehavior) {
                    linkBehavior = value;
                    InvalidateLinkFonts();
                    InvalidateLink(null);
                }
            }
        }

        /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.LinkColor"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the color used to display links in normal cases.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.LinkLabelLinkColorDescr))
        ]
        public Color LinkColor {
            get {
                if (linkColor.IsEmpty) {
                    if (SystemInformation.HighContrast) {
                        return SystemColors.HotTrack;
                    }
                    return IELinkColor;
                }
                else {
                    return linkColor;
                }
            }
            set {
                if (linkColor != value) {
                    linkColor = value;
                    InvalidateLink(null);
                }
            }
        }

        /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.Links"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the collection of links used in a <see cref='System.Windows.Forms.LinkLabel'/>.
        ///    </para>
        /// </devdoc>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public LinkCollection Links {
            get {
                if (linkCollection == null) {
                    linkCollection = new LinkCollection(this);
                }
                return linkCollection;
            }
        }

        /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.LinkVisited"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets a value indicating whether the link should be displayed as if it was visited.
        ///    </para>
        /// </devdoc>
        [
        DefaultValue(false),
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.LinkLabelLinkVisitedDescr))
        ]
        public bool LinkVisited {
            get {
                if (links.Count == 0) {
                    return false;
                }
                else {
                    return((Link)links[0]).Visited;
                }
            }
            set {
                if (value != LinkVisited) {
                    if (links.Count == 0) {
                        Links.Add(new Link(this));
                    }
                    ((Link)links[0]).Visited = value;
                }
            }
        }


        // link labels must always ownerdraw
        //
        internal override bool OwnerDraw {
            get {
                return true;
            }
        }


        /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.OverrideCursor"]/*' />
        protected Cursor OverrideCursor {
            get {
                return overrideCursor;
            }
            set {
                if (overrideCursor != value) {
                    overrideCursor = value;

                    if (IsHandleCreated) {
                        // We want to instantly change the cursor if the mouse is within our bounds.
                        // This includes the case where the mouse is over one of our children
                        NativeMethods.POINT p = new NativeMethods.POINT();
                        NativeMethods.RECT r = new NativeMethods.RECT();
                        UnsafeNativeMethods.GetCursorPos(p);
                        UnsafeNativeMethods.GetWindowRect(new HandleRef(this, Handle), ref r);

                        // 
                        if ((r.left <= p.x && p.x < r.right && r.top <= p.y && p.y < r.bottom) || UnsafeNativeMethods.GetCapture() == Handle)
                            SendMessage(NativeMethods.WM_SETCURSOR, Handle, NativeMethods.HTCLIENT);
                    }
                }
            }
        }

        /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.TabStopChanged"]/*' />
        /// <internalonly/>
        // Make this event visible through the property browser.
        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        new public event EventHandler TabStopChanged {
            add {
                base.TabStopChanged += value;
            }
            remove {
                base.TabStopChanged -= value;
            }
        }

        /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.TabIndex"]/*' />
        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        new public bool TabStop {
            get {
                return base.TabStop;
            }
            set {
                base.TabStop = value;
            }
        }

        /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.Text"]/*' />
        [RefreshProperties(RefreshProperties.Repaint)]
        public override string Text {
            get {
                return base.Text;
            }
            set {
                base.Text = value;
            }
        }

        /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.Padding"]/*' />
        [RefreshProperties(RefreshProperties.Repaint)]
        public new Padding Padding {
            get {return base.Padding;}
            set { base.Padding = value;}
        }

        /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.VisitedLinkColor"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the color used to display the link once it has been visited.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.LinkLabelVisitedLinkColorDescr))
        ]
        public Color VisitedLinkColor {
            get {
                if (visitedLinkColor.IsEmpty) {
                    if (SystemInformation.HighContrast) {
                        return LinkUtilities.GetVisitedLinkColor();
                    }
                    return IEVisitedLinkColor;
                }
                else {
                    return visitedLinkColor;
                }
            }
            set {
                if (visitedLinkColor != value) {
                    visitedLinkColor = value;
                    InvalidateLink(null);
                }
            }
        }

        /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.LinkClicked"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Occurs when the link is clicked.
        ///    </para>
        /// </devdoc>
        [WinCategory("Action"), SRDescription(nameof(SR.LinkLabelLinkClickedDescr))]
        public event LinkLabelLinkClickedEventHandler LinkClicked {
            add {
                Events.AddHandler(EventLinkClicked, value);
            }
            remove {
                Events.RemoveHandler(EventLinkClicked, value);
            }
        }

        internal static Rectangle CalcTextRenderBounds(Rectangle textRect, Rectangle clientRect, ContentAlignment align) {
            int xLoc, yLoc, width, height;

            if ((align & WindowsFormsUtils.AnyRightAlign) != 0) {
                xLoc = clientRect.Right - textRect.Width;
            } else if ((align & WindowsFormsUtils.AnyCenterAlign) != 0) {
                xLoc = (clientRect.Width - textRect.Width) / 2;
            } else {
                xLoc = clientRect.X;
            }

            if ((align & WindowsFormsUtils.AnyBottomAlign) != 0) {
                yLoc = clientRect.Bottom - textRect.Height;
            } else if ((align & WindowsFormsUtils.AnyMiddleAlign) != 0) {
                yLoc = (clientRect.Height - textRect.Height) / 2;
            } else {
                yLoc = clientRect.Y;
            }

            // If the text rect does not fit in the client rect, make it fit.
            if (textRect.Width > clientRect.Width) {
                xLoc = clientRect.X;
                width = clientRect.Width;
            } else {
                width = textRect.Width;
            }

            if (textRect.Height > clientRect.Height) {
                yLoc = clientRect.Y;
                height = clientRect.Height;
            } else {
                height = textRect.Height;
            }

            return new Rectangle(xLoc, yLoc, width, height);
        }

        /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.CreateAccessibilityInstance"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    Constructs the new instance of the accessibility object for this control. Subclasses
        ///    should not call base.CreateAccessibilityObject.
        /// </devdoc>
        protected override AccessibleObject CreateAccessibilityInstance() {
            return new LinkLabelAccessibleObject(this);
        }

        /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.CreateHandle"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Creates a handle for this control. This method is called by the .NET Framework,
        ///       this should not be called. Inheriting classes should always call
        ///       base.createHandle when overriding this method.
        ///    </para>
        /// </devdoc>
        protected override void CreateHandle() {
            base.CreateHandle();
            InvalidateTextLayout();
        }

        /// <summary>
        ///     Determines whether the current state of the control allows for rendering text using 
        ///     TextRenderer (GDI).
        ///     The Gdi library doesn't currently have a way to calculate character ranges so we cannot 
        ///     use it for painting link(s) within the text, but if the link are is null or covers the
        ///     entire text we are ok since it is just one area with the same size of the text binding
        ///     area.
        /// </summary>
        internal override bool CanUseTextRenderer {
            get{
                // If no link or the LinkArea is one and covers the entire text, we can support UseCompatibleTextRendering = false.
                // Observe that LinkArea refers to the first link always.
                StringInfo stringInfo = new StringInfo( this.Text );
                return this.LinkArea.Start == 0 && ( this.LinkArea.Length == 0 || this.LinkArea.Length == stringInfo.LengthInTextElements );
            }
        }

        internal override bool UseGDIMeasuring() {
            return !UseCompatibleTextRendering;
        }
        
        /// <devdoc>
        ///     Converts the character index into char index of the string
        ///     This method is copied in LinkCollectionEditor.cs. Update the other
        ///     one as well if you change this method.
        ///     This method mainly deal with surrogate. Suppose we 
        ///     have a string consisting of 3 surrogates, and we want the
        ///     second character, then the index we need should be 2 instead of
        ///     1, and this method returns the correct index.
        /// </devdoc>
        private static int ConvertToCharIndex(int index, string text) {
            if (index <= 0) {
                return 0;
            }
            if (String.IsNullOrEmpty(text)) {
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
            if (index > numTextElements) {
                return index - numTextElements + text.Length;  //pretend all the characters after are ASCII characters
            }
            //return the length of the substring which has specified number of characters
            string sub = stringInfo.SubstringByTextElements(0, index);
            return sub.Length;
        }
        
        
        /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.EnsureRun1"]/*' />
        /// <devdoc>
        ///     Ensures that we have analyzed the text run so that we can render each segment
        ///     and link.
        /// </devdoc>
        private void EnsureRun(Graphics g) {

            // bail early if everything is valid!
            //
            if (textLayoutValid) {
                return;
            }
            if (this.textRegion != null) {
                this.textRegion.Dispose();
                this.textRegion = null;
            }

            // bail early for no text
            //
            if (Text.Length == 0) {
                Links.Clear();
                Links.Add (new Link (0, -1));   // default 'magic' link.
                textLayoutValid = true;
                return;
            }

            StringFormat textFormat = CreateStringFormat();
            string text = Text;
            try {

                Font alwaysUnderlined = new Font(Font, Font.Style | FontStyle.Underline);
                Graphics created = null;

                try {
                    if (g == null) {
                        g = created = CreateGraphicsInternal();
                    }

                    if( UseCompatibleTextRendering ){
                        Region[] textRegions = g.MeasureCharacterRanges (text, alwaysUnderlined, ClientRectWithPadding, textFormat);

                        int regionIndex = 0;
                        
                        for (int i=0; i<Links.Count; i++) {
                            Link link = Links[i];
                            int charStart = ConvertToCharIndex(link.Start, text);
                            int charEnd = ConvertToCharIndex(link.Start + link.Length, text);
                            if (LinkInText(charStart, charEnd - charStart)) {
                                Links[i].VisualRegion = textRegions[regionIndex];
                                regionIndex++;
                            }
                        }
                        
                        Debug.Assert(regionIndex == (textRegions.Length - 1), "Failed to consume all link label visual regions");
                        this.textRegion = textRegions[textRegions.Length - 1];
                    } else {
                        // use TextRenderer.MeasureText to see the size of the text
                        Rectangle clientRectWithPadding = ClientRectWithPadding;
                        Size clientSize = new Size(clientRectWithPadding.Width, clientRectWithPadding.Height);
                        TextFormatFlags flags = CreateTextFormatFlags(clientSize);
                        Size textSize = TextRenderer.MeasureText(text, alwaysUnderlined, clientSize, flags);

                        // We need to take into account the padding that GDI adds around the text.
                        int iLeftMargin, iRightMargin;
                        using( WindowsGraphics wg = WindowsGraphics.FromGraphics(g) ){
                            if( (flags & TextFormatFlags.NoPadding ) == TextFormatFlags.NoPadding ){
                                wg.TextPadding = TextPaddingOptions.NoPadding;
                            }
                            else if( (flags & TextFormatFlags.LeftAndRightPadding ) == TextFormatFlags.LeftAndRightPadding ){
                                wg.TextPadding = TextPaddingOptions.LeftAndRightPadding;
                            }

                            using (WindowsFont wf = WindowsGraphicsCacheManager.GetWindowsFont(this.Font)) {
                                IntNativeMethods.DRAWTEXTPARAMS dtParams = wg.GetTextMargins(wf);

                                iLeftMargin = dtParams.iLeftMargin; 
                                iRightMargin = dtParams.iRightMargin; 
                            }

                        }

                        Rectangle visualRectangle = new Rectangle(clientRectWithPadding.X + iLeftMargin,
                                                                  clientRectWithPadding.Y,
                                                                  textSize.Width - iRightMargin - iLeftMargin,
                                                                  textSize.Height);
                        visualRectangle = CalcTextRenderBounds(visualRectangle /*textRect*/, clientRectWithPadding /*clientRect*/, RtlTranslateContent(this.TextAlign));
                        // 


                        Region visualRegion = new Region(visualRectangle);
                        if (this.links != null && this.links.Count == 1) {
                            this.Links[0].VisualRegion = visualRegion;
                        }
                        this.textRegion = visualRegion;
                    }
                }
                finally {
                    alwaysUnderlined.Dispose();
                    alwaysUnderlined = null;

                    if (created != null) {
                        created.Dispose();
                        created = null;
                    }
                }

                textLayoutValid = true;
            }
            finally {
                textFormat.Dispose();
            }
        }

        [ResourceExposure(ResourceScope.Process)]
        [ResourceConsumption(ResourceScope.Process)]
        internal override StringFormat CreateStringFormat() {
            StringFormat stringFormat = base.CreateStringFormat();
            if (String.IsNullOrEmpty(Text)) {
                return stringFormat;
            }

            CharacterRange[] regions = AdjustCharacterRangesForSurrogateChars();
            stringFormat.SetMeasurableCharacterRanges(regions);

            return stringFormat;
        }

        /// <devdoc>
        ///     Calculate character ranges taking into account the locale.  Provided for surrogate chars support.
        /// </devdoc>
        private CharacterRange[] AdjustCharacterRangesForSurrogateChars(){
            string text = Text;

            if (String.IsNullOrEmpty(text)) {
                return new CharacterRange[]{};
            }

            StringInfo stringInfo = new StringInfo(text);                 
            int textLen = stringInfo.LengthInTextElements;
            ArrayList ranges = new ArrayList(Links.Count);

            foreach (Link link in Links) {
                int charStart = ConvertToCharIndex(link.Start, text);
                int charEnd = ConvertToCharIndex(link.Start + link.Length, text);
                if (LinkInText(charStart, charEnd - charStart)) {
                    int length = (int) Math.Min(link.Length, textLen - link.Start);
                    ranges.Add(new CharacterRange(charStart, ConvertToCharIndex(link.Start + length, text) - charStart));
                }
            }

            CharacterRange[] regions = new CharacterRange[ranges.Count + 1];
            ranges.CopyTo(regions, 0);
            regions[regions.Length - 1] = new CharacterRange(0, text.Length);

            return regions;
        }

        /// <devdoc>
        ///     Determines whether the whole link label contains only one link,
        ///     and the link runs from the beginning of the label to the end of it
        /// </devdoc>
        private bool IsOneLink() {
            if (links == null || links.Count != 1 || Text == null) {
                return false;
            }
            StringInfo stringInfo = new StringInfo(Text);
            if (LinkArea.Start == 0 && LinkArea.Length == stringInfo.LengthInTextElements) {
                return true;
            }
            return false;
        }
        
        /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.PointInLink"]/*' />
        /// <devdoc>
        ///     Determines if the given client coordinates is contained within a portion
        ///     of a link area.
        /// </devdoc>
        protected Link PointInLink(int x, int y) {
            Graphics g = CreateGraphicsInternal();
            Link hit = null;
            try {
                EnsureRun(g);
                foreach (Link link in links) {
                    if (link.VisualRegion != null && link.VisualRegion.IsVisible(x, y, g)) {
                        hit = link;
                        break;
                    }
                }
            }
            finally {
                g.Dispose();
                g = null;
            }
            return hit;
        }

        /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.InvalidateLink"]/*' />
        /// <devdoc>
        ///     Invalidates only the portions of the text that is linked to
        ///     the specified link. If link is null, then all linked text
        ///     is invalidated.
        /// </devdoc>
        private void InvalidateLink(Link link) {
            if (IsHandleCreated) {
                if (link == null || link.VisualRegion == null || IsOneLink()) {
                    Invalidate();
                }
                else{
                    Invalidate(link.VisualRegion);
                }
            }
        }

        /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.InvalidateLinkFonts"]/*' />
        /// <devdoc>
        ///     Invalidates the current set of fonts we use when painting
        ///     links.  The fonts will be recreated when needed.
        /// </devdoc>
        private void InvalidateLinkFonts() {

            if (linkFont != null) {
                linkFont.Dispose();
            }

            if (hoverLinkFont != null && hoverLinkFont != linkFont) {
                hoverLinkFont.Dispose();
            }

            linkFont = null;
            hoverLinkFont = null;
        }

        private void InvalidateTextLayout() {
            textLayoutValid = false;
        }

        private bool LinkInText(int start, int length) {
            return(0 <= start && start < Text.Length && 0 < length);
        }

        /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.IButtonControl.DialogResult"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// <para>
        /// Gets or sets a value that is returned to the
        /// parent form when the link label.
        /// is clicked.
        /// </para>
        /// </devdoc>
        DialogResult IButtonControl.DialogResult {
            get {
                return dialogResult;
            }

            set {
                //valid values are 0x0 to 0x7
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)DialogResult.None, (int)DialogResult.No))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(DialogResult));
                }

                dialogResult = value;
            }
        }

        /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.IButtonControl.NotifyDefault"]/*' />
        /// <internalonly/>
        void IButtonControl.NotifyDefault(bool value) {
        }

        /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.OnGotFocus"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Raises the <see cref='System.Windows.Forms.Control.GotFocus'/>
        ///       event.
        ///    </para>
        /// </devdoc>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2106:SecureAsserts")]
        protected override void OnGotFocus(EventArgs e) {
            if (!this.processingOnGotFocus) {
                base.OnGotFocus(e);
                this.processingOnGotFocus = true;
            }

            try {
                Link focusLink = FocusLink;
                if (focusLink == null) {
                    // 


                    IntSecurity.ModifyFocus.Assert();
                    
                    // Set focus on first link.  
                    // This will raise the OnGotFocus event again but it will not be processed because processingOnGotFocus is true.
                    Select(true /*directed*/, true /*forward*/);
                }
                else {
                    InvalidateLink(focusLink);
                    UpdateAccessibilityLink(focusLink);
                }
            }
            finally {
                if (this.processingOnGotFocus) {
                    this.processingOnGotFocus = false;
                }
            }
        }

        /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.OnLostFocus"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Raises the <see cref='System.Windows.Forms.Control.LostFocus'/>
        ///       event.
        ///    </para>
        /// </devdoc>
        protected override void OnLostFocus(EventArgs e) {
            base.OnLostFocus(e);

            if (FocusLink != null) {
                InvalidateLink(FocusLink);
            }
        }

        /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.OnKeyDown"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Raises the <see cref='System.Windows.Forms.Control.OnKeyDown'/>
        ///       event.
        ///    </para>
        /// </devdoc>
        protected override void OnKeyDown(KeyEventArgs e) {
            base.OnKeyDown(e);

            if (e.KeyCode == Keys.Enter) {
                if (FocusLink != null && FocusLink.Enabled) {
                    OnLinkClicked(new LinkLabelLinkClickedEventArgs(FocusLink));
                }
            }
        }

        /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.OnMouseLeave"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Raises the <see cref='System.Windows.Forms.Control.OnMouseLeave'/>
        ///       event.
        ///    </para>
        /// </devdoc>
        protected override void OnMouseLeave(EventArgs e) {
            base.OnMouseLeave(e);
            if (!Enabled) {
                return;
            }

            foreach (Link link in links) {
                if ((link.State & LinkState.Hover) == LinkState.Hover
                    || (link.State & LinkState.Active) == LinkState.Active) {

                    bool activeChanged = (link.State & LinkState.Active) == LinkState.Active;
                    link.State &= ~(LinkState.Hover | LinkState.Active);

                    if (activeChanged || hoverLinkFont != linkFont) {
                        InvalidateLink(link);
                    }
                    OverrideCursor = null;
                }
            }
        }

        /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.OnMouseDown"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Raises the <see cref='System.Windows.Forms.Control.OnMouseDown'/>
        ///       event.
        ///    </para>
        /// </devdoc>
        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseDown(e);

            if (!Enabled || e.Clicks > 1) {
                receivedDoubleClick = true;
                return;
            }

            for (int i=0; i<links.Count; i++) {
                if ((((Link)links[i]).State & LinkState.Hover) == LinkState.Hover) {
                    ((Link)links[i]).State |= LinkState.Active;

                    FocusInternal();
                    if (((Link)links[i]).Enabled) {
                        FocusLink = (Link)links[i];
                        InvalidateLink(FocusLink);
                    }
                    CaptureInternal = true;
                    break;
                }
            }
        }

        /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.OnMouseUp"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Raises the <see cref='System.Windows.Forms.Control.OnMouseUp'/>
        ///       event.
        ///    </para>
        /// </devdoc>
        protected override void OnMouseUp(MouseEventArgs e) {

            base.OnMouseUp(e);

            // 
            if (Disposing || IsDisposed) {
                return;
            }

            if (!Enabled || e.Clicks > 1 || receivedDoubleClick) {
                receivedDoubleClick = false;
                return;
            }

            for (int i=0; i<links.Count; i++) {
                if ((((Link)links[i]).State & LinkState.Active) == LinkState.Active) {
                    ((Link)links[i]).State &= (~LinkState.Active);
                    InvalidateLink((Link)links[i]);
                    CaptureInternal = false;

                    Link clicked = PointInLink(e.X, e.Y);

                    if (clicked != null && clicked == FocusLink && clicked.Enabled) {
                        OnLinkClicked(new LinkLabelLinkClickedEventArgs(clicked, e.Button));
                    }
                }
            }
        }
        
        /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.OnMouseMove"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Raises the <see cref='System.Windows.Forms.Control.OnMouseMove'/>
        ///       event.
        ///    </para>
        /// </devdoc>
        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);

            if (!Enabled) {
                return;
            }

            Link hoverLink = null;
            foreach (Link link in links) {
                if ((link.State & LinkState.Hover) == LinkState.Hover) {
                    hoverLink = link;
                    break;
                }
            }

            Link pointIn = PointInLink(e.X, e.Y);

            if (pointIn != hoverLink) {
                if (hoverLink != null) {
                    hoverLink.State &= ~LinkState.Hover;
                }
                if (pointIn != null) {
                    pointIn.State |= LinkState.Hover;
                    if (pointIn.Enabled) {
                        OverrideCursor = Cursors.Hand;
                    }
                }
                else {
                    OverrideCursor = null;
                }

                if (hoverLinkFont != linkFont) {
                    if (hoverLink != null) {
                        InvalidateLink(hoverLink);
                    }
                    if (pointIn != null) {
                        InvalidateLink(pointIn);
                    }
                }
            }
        }

        /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.OnLinkClicked"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Raises the <see cref='System.Windows.Forms.LinkLabel.OnLinkClicked'/> event.
        ///    </para>
        /// </devdoc>
        protected virtual void OnLinkClicked(LinkLabelLinkClickedEventArgs e) {
            LinkLabelLinkClickedEventHandler handler = (LinkLabelLinkClickedEventHandler)Events[EventLinkClicked];
            if (handler != null) {
                handler(this, e);
            }
        }

        protected override void OnPaddingChanged(EventArgs e) {
            base.OnPaddingChanged(e);
            InvalidateTextLayout();
        }

        /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.OnPaint"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Raises the <see cref='System.Windows.Forms.Control.OnPaint'/>
        ///       event.
        ///    </para>
        /// </devdoc>
        protected override void OnPaint(PaintEventArgs e) {
            RectangleF finalrect = RectangleF.Empty;   //the focus rectangle if there is only one link
            Animate();

            ImageAnimator.UpdateFrames(this.Image);
            EnsureRun(e.Graphics);

            // bail early for no text
            //
            if (Text.Length == 0) {
                PaintLinkBackground(e.Graphics);
            }
            // Paint enabled link label
            //
            else {
                if (AutoEllipsis) {
                    Rectangle clientRect = this.ClientRectWithPadding;
                    Size preferredSize = GetPreferredSize(new Size(clientRect.Width, clientRect.Height));
                    showToolTip = (clientRect.Width < preferredSize.Width || clientRect.Height < preferredSize.Height);
                }
                else {
                    showToolTip = false;
                }

                if (this.Enabled) { // Control.Enabled not to be confused with Link.Enabled
                    bool optimizeBackgroundRendering = !GetStyle(ControlStyles.OptimizedDoubleBuffer);
                    SolidBrush foreBrush = new SolidBrush(ForeColor);
                    SolidBrush linkBrush = new SolidBrush(LinkColor);

                    try {
                        if (!optimizeBackgroundRendering) {
                            PaintLinkBackground(e.Graphics);
                        }

                        LinkUtilities.EnsureLinkFonts(this.Font, this.LinkBehavior, ref this.linkFont, ref this.hoverLinkFont);

                        Region originalClip = e.Graphics.Clip;

                        try {
                            if (IsOneLink()) {
                                //exclude the area to draw the focus rectangle
                                e.Graphics.Clip = originalClip;
                                RectangleF[] rects = ((Link)links[0]).VisualRegion.GetRegionScans(e.Graphics.Transform);
                                if (rects != null && rects.Length > 0) {
                                    if (UseCompatibleTextRendering) {
                                        finalrect = new RectangleF(rects[0].Location, SizeF.Empty);
                                        foreach (RectangleF rect in rects) {
                                            finalrect = RectangleF.Union(finalrect, rect);
                                        }
                                    }
                                    else {
                                        finalrect = this.ClientRectWithPadding;
                                        Size finalRectSize = finalrect.Size.ToSize();
                                        
                                        Size requiredSize = MeasureTextCache.GetTextSize(Text, Font, finalRectSize, CreateTextFormatFlags(finalRectSize));

                                        finalrect.Width = requiredSize.Width;

                                        if (requiredSize.Height < finalrect.Height) {
                                            finalrect.Height = requiredSize.Height;
                                        }
                                        finalrect = CalcTextRenderBounds(System.Drawing.Rectangle.Round(finalrect) /*textRect*/, this.ClientRectWithPadding /*clientRect*/, RtlTranslateContent(this.TextAlign));
                                    }
                                    using (Region region = new Region(finalrect)) {
                                        e.Graphics.ExcludeClip(region);
                                    }
                                }
                            }
                            else {
                                foreach (Link link in links) {
                                    if (link.VisualRegion != null) {
                                        e.Graphics.ExcludeClip(link.VisualRegion);
                                    }
                                }
                            }

                            // When there is only one link in link label,
                            // it's not necessary to paint with forebrush first 
                            // as it will be overlapped by linkbrush in the following steps

                            if (!IsOneLink()) {
                                PaintLink(e.Graphics, null, foreBrush, linkBrush, optimizeBackgroundRendering, finalrect);
                            }

                            foreach (Link link in links) {
                                PaintLink(e.Graphics, link, foreBrush, linkBrush, optimizeBackgroundRendering, finalrect);
                            }

                            if (optimizeBackgroundRendering) {
                                e.Graphics.Clip = originalClip;
                                e.Graphics.ExcludeClip(this.textRegion);
                                PaintLinkBackground(e.Graphics);
                            }
                        }
                        finally {
                            e.Graphics.Clip = originalClip;
                        }
                    }
                    finally {
                        foreBrush.Dispose();
                        linkBrush.Dispose();
                    }
                }
                // Paint disabled link label (disabled control, not to be confused with disabled link).
                //
                else {
                    Region originalClip = e.Graphics.Clip;

                    try {
                        // We need to paint the background first before clipping to textRegion because it is calculated using
                        // ClientRectWithPadding which in some cases is smaller that ClientRectangle.
                        //
                        PaintLinkBackground(e.Graphics);
                        e.Graphics.IntersectClip(this.textRegion);

                        Color foreColor;

                        if (UseCompatibleTextRendering) {
                            // APPCOMPAT: Use DisabledColor because Everett used DisabledColor.
                            // (ie, dont use Graphics.GetNearestColor(DisabledColor.)
                            StringFormat stringFormat = CreateStringFormat();
                            ControlPaint.DrawStringDisabled(e.Graphics, Text, Font, DisabledColor, ClientRectWithPadding, stringFormat);
                        }
                        else {
                            IntPtr hdc = e.Graphics.GetHdc();
                            try {
                                using (WindowsGraphics wg = WindowsGraphics.FromHdc(hdc)) {
                                    foreColor = wg.GetNearestColor(DisabledColor);
                                }
                            }
                            finally {
                                e.Graphics.ReleaseHdc();
                            }
                            Rectangle clientRectWidthPadding = ClientRectWithPadding;
                            
                            ControlPaint.DrawStringDisabled(e.Graphics, Text, Font, foreColor, clientRectWidthPadding, CreateTextFormatFlags(clientRectWidthPadding.Size));
                        }
                    }
                    finally {
                        e.Graphics.Clip = originalClip;
                    }
                }
            }

            // We can't call base.OnPaint because labels paint differently from link labels,
            // but we still need to raise the Paint event.
            //
            RaisePaintEvent(this, e);
        }

        /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.OnPaintBackground"]/*' />
        protected override void OnPaintBackground(PaintEventArgs e) {
            Image i = this.Image;

            if (i != null) {
                Region oldClip = e.Graphics.Clip;
                Rectangle imageBounds = CalcImageRenderBounds(i, ClientRectangle, RtlTranslateAlignment(ImageAlign));
                e.Graphics.ExcludeClip(imageBounds);
                try {
                    base.OnPaintBackground(e);
                }
                finally {
                    e.Graphics.Clip = oldClip;
                }

                e.Graphics.IntersectClip(imageBounds);
                try {
                    base.OnPaintBackground(e);
                    DrawImage(e.Graphics, i, ClientRectangle, RtlTranslateAlignment(ImageAlign));
                }
                finally {
                    e.Graphics.Clip = oldClip;
                }
            }
            else {
                base.OnPaintBackground(e);
            }

        }

        /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.OnFontChanged"]/*' />
        protected override void OnFontChanged(EventArgs e) {
            base.OnFontChanged(e);
            InvalidateTextLayout();
            InvalidateLinkFonts();
            Invalidate();
        }

        /// <devdoc>
        /// </devdoc>
        protected override void OnAutoSizeChanged(EventArgs e) {
            base.OnAutoSizeChanged(e);
            InvalidateTextLayout();
        }

        /// <devdoc>
        ///     Overriden by LinkLabel.
        /// </devdoc>
        internal override void OnAutoEllipsisChanged(/*EventArgs e*/) {
            base.OnAutoEllipsisChanged(/*e*/);
            InvalidateTextLayout();
        }

        /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.OnEnabledChanged"]/*' />
        protected override void OnEnabledChanged(EventArgs e) {
            base.OnEnabledChanged(e);
            
            if (!Enabled) {
                for (int i=0; i<links.Count; i++) {
                    ((Link)links[i]).State &= ~(LinkState.Hover | LinkState.Active);
                }
                OverrideCursor = null;
            }
            InvalidateTextLayout();
            Invalidate();
        }

        /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.OnTextChanged"]/*' />
        protected override void OnTextChanged(EventArgs e) {
            base.OnTextChanged(e);
            InvalidateTextLayout();
            UpdateSelectability();
        }

        /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.OnTextAlignChanged"]/*' />
        protected override void OnTextAlignChanged(EventArgs e) {
            base.OnTextAlignChanged(e);
            InvalidateTextLayout();
            UpdateSelectability();
        }

        private void PaintLink(Graphics g, Link link, SolidBrush foreBrush, SolidBrush linkBrush, bool optimizeBackgroundRendering, RectangleF finalrect) {
            
            // link = null means paint the whole text

            Debug.Assert(g != null, "Must pass valid graphics");
            Debug.Assert(foreBrush != null, "Must pass valid foreBrush");
            Debug.Assert(linkBrush != null, "Must pass valid linkBrush");

            Font font = Font;

            if (link != null) {
                if (link.VisualRegion != null) {
                    Color brushColor = Color.Empty;
                    LinkState linkState = link.State;

                    if ((linkState & LinkState.Hover) == LinkState.Hover) {
                        font = hoverLinkFont;
                    }
                    else {
                        font = linkFont;
                    }

                    if (link.Enabled) { // Not to be confused with Control.Enabled.
                        if ((linkState & LinkState.Active) == LinkState.Active) {
                            brushColor = ActiveLinkColor;
                        }
                        else if ((linkState & LinkState.Visited) == LinkState.Visited) {
                            brushColor = VisitedLinkColor;
                        }
                        // else use linkBrush
                    }
                    else {
                        brushColor = DisabledLinkColor;
                    }

                    if (IsOneLink()) {
                        g.Clip = new Region(finalrect);
                    }
                    else {
                        g.Clip = link.VisualRegion;
                    }
                    
                    if (optimizeBackgroundRendering) {
                        PaintLinkBackground(g);
                    }

                    if( UseCompatibleTextRendering ){
                        SolidBrush useBrush = brushColor == Color.Empty ? linkBrush : new SolidBrush( brushColor );
                        StringFormat stringFormat = CreateStringFormat();
                    
                        g.DrawString(Text, font, useBrush, ClientRectWithPadding, stringFormat);

                        if( useBrush != linkBrush ){
                            useBrush.Dispose();
                        }
                    }
                    else{
                        if(brushColor == Color.Empty ){
                            brushColor = linkBrush.Color;
                        }

                        IntPtr hdc = g.GetHdc();
                        try{
                            using( WindowsGraphics wg = WindowsGraphics.FromHdc( hdc ) ) {
                                brushColor = wg.GetNearestColor(brushColor);
                            }
                        }
                        finally{
                            g.ReleaseHdc();
                        }
                        Rectangle clientRectWithPadding = ClientRectWithPadding;
                        TextRenderer.DrawText(g, Text, font, clientRectWithPadding, brushColor, CreateTextFormatFlags(clientRectWithPadding.Size));
                    }

                    if (Focused && ShowFocusCues && FocusLink == link) {
                        // Get the rectangles making up the visual region, and draw
                        // each one.
                        RectangleF[] rects = link.VisualRegion.GetRegionScans(g.Transform);
                        if( rects != null && rects.Length > 0 ){
                            Rectangle focusRect;

                            if (IsOneLink()) {
                                //draw one merged focus rectangle
                                focusRect = Rectangle.Ceiling(finalrect);
                                Debug.Assert(finalrect != RectangleF.Empty, "finalrect should be initialized");

                                ControlPaint.DrawFocusRectangle(g, focusRect, ForeColor, BackColor);
                            }
                            else {
                                foreach (RectangleF rect in rects) {
                                    ControlPaint.DrawFocusRectangle(g, Rectangle.Ceiling(rect), ForeColor, BackColor);
                                }
                            }
                        }
                    }
                }

                // no else clause... we don't paint anything if we are given a link with no visual region.
                //
            }
            else { // Painting with no link.
                g.IntersectClip(this.textRegion);

                if (optimizeBackgroundRendering) {
                    PaintLinkBackground(g);
                }

                if( UseCompatibleTextRendering ){
                    StringFormat stringFormat = CreateStringFormat();
                    g.DrawString(Text, font, foreBrush, ClientRectWithPadding, stringFormat);
                }
                else{
                    Color color;

                    IntPtr hdc = g.GetHdc();
                    try{
                        using( WindowsGraphics wg = WindowsGraphics.FromHdc( hdc ) ) {
                            color = wg.GetNearestColor(foreBrush.Color);
                        }
                    }
                    finally{
                        g.ReleaseHdc();
                    }
                    Rectangle clientRectWithPadding = ClientRectWithPadding;
                    TextRenderer.DrawText(g, Text, font, clientRectWithPadding, color, CreateTextFormatFlags(clientRectWithPadding.Size));
                }
            }
        }

        private void PaintLinkBackground(Graphics g) {
            using (PaintEventArgs e = new PaintEventArgs(g, ClientRectangle)) {
                InvokePaintBackground(this, e);
            }
        }

        /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.IButtonControl.PerformClick"]/*' />
        /// <internalonly/>
        void IButtonControl.PerformClick() {

            // If a link is not currently focused, focus on the first link
            //
            if (FocusLink == null && Links.Count > 0) {
                string text = Text;
                foreach (Link link in Links) {
                    int charStart = ConvertToCharIndex(link.Start, text);
                    int charEnd = ConvertToCharIndex(link.Start + link.Length, text);
                    if (link.Enabled && LinkInText(charStart, charEnd - charStart)) {
                        FocusLink = link;
                        break;
                    }
                }
            }

            // Act as if the focused link was clicked
            //
            if (FocusLink != null) {
                OnLinkClicked(new LinkLabelLinkClickedEventArgs(FocusLink));
            }
        }

        /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.ProcessDialogKey"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Processes a dialog key. This method is called during message pre-processing
        ///       to handle dialog characters, such as TAB, RETURN, ESCAPE, and arrow keys. This
        ///       method is called only if the isInputKey() method indicates that the control
        ///       isn't interested in the key. processDialogKey() simply sends the character to
        ///       the parent's processDialogKey() method, or returns false if the control has no
        ///       parent. The Form class overrides this method to perform actual processing
        ///       of dialog keys. When overriding processDialogKey(), a control should return true
        ///       to indicate that it has processed the key. For keys that aren't processed by the
        ///       control, the result of "base.processDialogChar()" should be returned. Controls
        ///       will seldom, if ever, need to override this method.
        ///    </para>
        /// </devdoc>
        [UIPermission(SecurityAction.LinkDemand, Window=UIPermissionWindow.AllWindows)]
        protected override bool ProcessDialogKey(Keys keyData) {
            if ((keyData & (Keys.Alt | Keys.Control)) != Keys.Alt) {
                Keys keyCode = keyData & Keys.KeyCode;
                switch (keyCode) {
                    case Keys.Tab:
                            if (TabStop) {
                            bool forward = (keyData & Keys.Shift) != Keys.Shift;
                            if (FocusNextLink(forward)) {
                                return true;
                            }
                        }
                        break;
                    case Keys.Up:
                    case Keys.Left:
                        if (FocusNextLink(false)) {
                            return true;
                        }
                        break;
                    case Keys.Down:
                    case Keys.Right:
                        if (FocusNextLink(true)) {
                            return true;
                        }
                        break;
                }
            }
            return base.ProcessDialogKey(keyData);
        }

        private bool FocusNextLink(bool forward) {
            int focusIndex = -1;
            if (focusLink != null) {
                for (int i=0; i<links.Count; i++) {
                    if (links[i] == focusLink) {
                        focusIndex = i;
                        break;
                    }
                }
            }

            focusIndex = GetNextLinkIndex(focusIndex, forward);
            if (focusIndex != -1) {
                FocusLink = Links[focusIndex];
                return true;
            }
            else {
                FocusLink = null;
                return false;
            }
        }

        private int GetNextLinkIndex(int focusIndex, bool forward) {
            Link test;
            string text = Text;
            int charStart = 0;
            int charEnd = 0;

            if (forward) {
                do {
                    focusIndex++;

                    if (focusIndex < Links.Count) {
                        test = Links[focusIndex];
                        charStart = ConvertToCharIndex(test.Start, text);
                        charEnd = ConvertToCharIndex(test.Start + test.Length, text);
                    }
                    else {
                        test = null;
                    }
                } while (test != null
                         && !test.Enabled
                         && LinkInText(charStart, charEnd - charStart));
            }
            else {
                do {
                    focusIndex--;
                    if (focusIndex >= 0) {
                        test = Links[focusIndex];
                        charStart = ConvertToCharIndex(test.Start, text);
                        charEnd = ConvertToCharIndex(test.Start + test.Length, text);
                    }
                    else {
                        test = null;
                    }
                    
                } while (test != null
                         && !test.Enabled
                         && LinkInText(charStart, charEnd - charStart));
            }

            if (focusIndex < 0 || focusIndex >= links.Count) {
                return -1;
            }
            else {
                return focusIndex;
            }
        }

        private void ResetLinkArea() {
            LinkArea = new LinkArea(0, -1);
        }

        internal void ResetActiveLinkColor() {
            activeLinkColor = Color.Empty;
        }

        internal void ResetDisabledLinkColor() {
            disabledLinkColor = Color.Empty;
        }

        internal void ResetLinkColor() {
            linkColor = Color.Empty;
            InvalidateLink(null);
        }

        private void ResetVisitedLinkColor() {
            visitedLinkColor = Color.Empty;
        }


        /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.SetBoundsCore"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Performs the work of setting the bounds of this control. Inheriting classes
        ///       can overide this function to add size restrictions. Inheriting classes must call
        ///       base.setBoundsCore to actually cause the bounds of the control to change.
        ///    </para>
        /// </devdoc>
        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) {

            // we cache too much state to try and optimize this (regions, etc)... it is best 
            // to always relayout here... If we want to resurect this code in the future, 
            // remember that we need to handle a word wrapped top aligned text that 
            // will become newly exposed (and therefore layed out) when we resize...
            //
            /*
            ContentAlignment anyTop = ContentAlignment.TopLeft | ContentAlignment.TopCenter | ContentAlignment.TopRight;

            if ((TextAlign & anyTop) == 0 || Width != width || (Image != null && (ImageAlign & anyTop) == 0)) {
                InvalidateTextLayout();
                Invalidate();
            }
            */
            
            InvalidateTextLayout();
            Invalidate();

            base.SetBoundsCore(x, y, width, height, specified);
        }

        /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.Select"]/*' />
        protected override void Select(bool directed, bool forward) {

            if (directed) {
                // In a multi-link label, if the tab came from another control, we want to keep the currently 
                // focused link, otherwise, we set the focus to the next link.
                if (links.Count > 0) {

                    // Find which link is currently focused
                    //
                    int focusIndex = -1;
                    if (FocusLink != null) {
                        focusIndex = links.IndexOf(FocusLink);
                    }

                    // We could be getting focus from ourself, so we must
                    // invalidate each time.                                   
                    //
                    FocusLink = null;

                    int newFocus = GetNextLinkIndex(focusIndex, forward);
                    if (newFocus == -1) {
                        if (forward) {
                            newFocus = GetNextLinkIndex(-1, forward); // -1, so "next" will be 0
                        }
                        else {
                            newFocus = GetNextLinkIndex(links.Count, forward); // Count, so "next" will be Count-1
                        }
                    }

                    if (newFocus != -1) {
                        FocusLink = (Link)links[newFocus];
                    }
                }
            }
            base.Select(directed, forward);
        }

        /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.ShouldSerializeActiveLinkColor"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Determines if the color for active links should remain the same.
        ///    </para>
        /// </devdoc>
        internal bool ShouldSerializeActiveLinkColor() {
            return !activeLinkColor.IsEmpty;
        }

        /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.ShouldSerializeDisabledLinkColor"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Determines if the color for disabled links should remain the same.
        ///    </para>
        /// </devdoc>
        internal bool ShouldSerializeDisabledLinkColor() {
            return !disabledLinkColor.IsEmpty;
        }

        /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.ShouldSerializeLinkArea"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Determines if the range in text that is treated as a
        ///       link should remain the same.      
        ///    </para>
        /// </devdoc>
        private bool ShouldSerializeLinkArea() {
            if (links.Count == 1) {
                // use field access to find out if "length" is really -1
                return Links[0].Start != 0 || Links[0].length != -1;
            }
            return true;

        }

        /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.ShouldSerializeLinkColor"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Determines if the color of links in normal cases should remain the same.
        ///    </para>
        /// </devdoc>
        internal bool ShouldSerializeLinkColor() {
            return !linkColor.IsEmpty;
        }

        /// <devdoc>
        ///     Determines whether designer should generate code for setting the UseCompatibleTextRendering or not.
        ///     DefaultValue(false)
        /// </devdoc>
        private bool ShouldSerializeUseCompatibleTextRendering() {
            // Serialize code if LinkLabel cannot support the feature or the property's value is  not the default.
            return !CanUseTextRenderer || UseCompatibleTextRendering != Control.UseCompatibleTextRenderingDefault; 
        }

        /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.ShouldSerializeVisitedLinkColor"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Determines if the color of links that have been visited should remain the same.
        ///    </para>
        /// </devdoc>
        private bool ShouldSerializeVisitedLinkColor() {
            return !visitedLinkColor.IsEmpty;
        }



        /// <devdoc>
        ///    <para>
        ///       Update accessibility with the currently focused link.
        ///    </para>
        /// </devdoc>
        private void UpdateAccessibilityLink(Link focusLink) {

            if (!IsHandleCreated) {

                return;
            }
            
            int focusIndex = -1;
            for (int i=0; i<links.Count; i++) {
                if (links[i] == focusLink) {
                    focusIndex = i;
                }
            }
            AccessibilityNotifyClients(AccessibleEvents.Focus, focusIndex);            
        }

        /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.ValidateNoOverlappingLinks"]/*' />
        /// <devdoc>
        ///     Validates that no links overlap. This will throw an exception if
        ///     they do.
        /// </devdoc>
        private void ValidateNoOverlappingLinks() {
            for (int x=0; x<links.Count; x++) {

                Link left = (Link)links[x];
                if (left.Length < 0) {
                    throw new InvalidOperationException(SR.LinkLabelOverlap);
                }

                for (int y=x; y<links.Count; y++) {
                    if (x != y) {
                        Link right = (Link)links[y];
                        int maxStart = Math.Max(left.Start, right.Start);
                        int minEnd = Math.Min(left.Start + left.Length, right.Start + right.Length);
                        if (maxStart < minEnd) {
                            throw new InvalidOperationException(SR.LinkLabelOverlap);
                        }
                    }
                }
            }
        }

        /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.UpdateSelectability"]/*' />
        /// <devdoc>
        ///     Updates the label's ability to get focus. If there are
        ///     any links in the label, then the label can get focus,
        ///     else it can't.
        /// </devdoc>
        private void UpdateSelectability() {
            LinkArea pt = LinkArea;
            bool selectable = false;
            string text = Text;
            int charStart = ConvertToCharIndex(pt.Start, text);
            int charEnd = ConvertToCharIndex(pt.Start + pt.Length, text);

            if (LinkInText(charStart, charEnd - charStart)) {
                selectable = true;
            }
            else {
                // If a link is currently focused, de-select it
                //
                if (FocusLink != null) {
                    FocusLink = null;
                }
            }
            
            OverrideCursor = null;
            TabStop = selectable;
            SetStyle(ControlStyles.Selectable, selectable);
        }

        /// <devdoc>
        ///     Determines whether to use compatible text rendering engine (GDI+) or not (GDI).
        /// </devdoc>
        [
        // DefaultValue(false), - // See ShouldSerailizeUseCompatibleTextRendering method.
        RefreshProperties(RefreshProperties.Repaint),
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.UseCompatibleTextRenderingDescr))
        ]
        public new bool UseCompatibleTextRendering {
            get {
                Debug.Assert( CanUseTextRenderer || base.UseCompatibleTextRendering, "Using GDI text rendering when CanUseTextRenderer reported false." );
                return base.UseCompatibleTextRendering;
            }
            set {
                if (base.UseCompatibleTextRendering != value) {
                    // Cache the value so it is restored if CanUseTextRenderer becomes true and the designer can undo changes to this as side effect.
                    base.UseCompatibleTextRendering = value;
                    InvalidateTextLayout();
                }
            }
        }

        internal override bool SupportsUiaProviders {
            get {
                return false;
            }
        }

        /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.WmSetCursor"]/*' />
        /// <devdoc>
        ///     Handles the WM_SETCURSOR message
        /// </devdoc>
        /// <internalonly/>
        private void WmSetCursor(ref Message m) {

            // Accessing through the Handle property has side effects that break this
            // logic. You must use InternalHandle.
            //
            if (m.WParam == InternalHandle && NativeMethods.Util.LOWORD(m.LParam) == NativeMethods.HTCLIENT) {
                if (OverrideCursor != null) {
                    Cursor.CurrentInternal = OverrideCursor;
                }
                else {
                    Cursor.CurrentInternal = Cursor;
                }
            }
            else {
                DefWndProc(ref m);
            }

        }

        /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.WndProc"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// </devdoc>
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message msg) {
            switch (msg.Msg) {
                case NativeMethods.WM_SETCURSOR:
                    WmSetCursor(ref msg);
                    break;
                default:
                    base.WndProc(ref msg);
                    break;
            }
        }

        /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.LinkCollection"]/*' />
        public class LinkCollection : IList {
            private LinkLabel owner;
            private bool linksAdded = false;   //whether we should serialize the linkCollection

            /// A caching mechanism for key accessor
            /// We use an index here rather than control so that we don't have lifetime
            /// issues by holding on to extra references.
            /// Note this is not Thread Safe - but WinForms has to be run in a STA anyways.
            private int lastAccessedIndex = -1;

            /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.LinkCollection.LinkCollection"]/*' />
            public LinkCollection(LinkLabel owner) {
                if (owner == null)
                    throw new ArgumentNullException(nameof(owner));
                this.owner = owner;
            }

            /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.LinkCollection.this"]/*' />
            public virtual Link this[int index] {
                get {
                    return(Link)owner.links[index];
                }
                set {
                    owner.links[index] = value;

                    owner.links.Sort(LinkLabel.linkComparer);

                    owner.InvalidateTextLayout();
                    owner.Invalidate();
                }
            }
            
            /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkCollection.IList.this"]/*' />
            /// <internalonly/>
            object IList.this[int index] {
                get {
                    return this[index];
                }
                set {
                    if (value is Link) {
                        this[index] = (Link)value;
                    }
                    else {  
                        throw new ArgumentException(SR.LinkLabelBadLink,"value");
                    }
                }
            }
           
            /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.LinkCollection.this"]/*' />
            /// <devdoc>
            ///     <para>Retrieves the child control with the specified key.</para>
            /// </devdoc>
            public virtual Link this[string key] {
                get {
                    // We do not support null and empty string as valid keys.
                    if (string.IsNullOrEmpty(key)){
                        return null;
                    }

                    // Search for the key in our collection
                    int index = IndexOfKey(key);
                    if (IsValidIndex(index)) {
                        return this[index];
                    }
                    else {
                        return null;
                    }

                }
            }
  

            /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.LinkCollection.Count"]/*' />
            [Browsable(false)]
            public int Count {
                get {
                    return owner.links.Count;
                }
            }

            /// <devdoc>
            ///    <para>whether we have added a non-trivial link to the collection</para>
            /// </devdoc>
            
            public bool LinksAdded {
                get {
                    return linksAdded;
                }
            }
            
            /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkCollection.ICollection.SyncRoot"]/*' />
            /// <internalonly/>
            object ICollection.SyncRoot {
                get {
                    return this;
                }
            }

            /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkCollection.ICollection.IsSynchronized"]/*' />
            /// <internalonly/>
            bool ICollection.IsSynchronized {
                get {
                    return false;
                }
            }
            
            /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkCollection.IList.IsFixedSize"]/*' />
            /// <internalonly/>
            bool IList.IsFixedSize {
                get {
                    return false;
                }
            }
           
            /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.LinkCollection.IsReadOnly"]/*' />
            public bool IsReadOnly {
                get {
                    return false;
                }
            }
            
            /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.LinkCollection.Add"]/*' />
            public Link Add(int start, int length) {
                if (length != 0) {
                    linksAdded = true;
                }
                return Add(start, length, null);
            }

            /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.LinkCollection.Add1"]/*' />
            public Link Add(int start, int length, object linkData) {
                if (length != 0) {
                    linksAdded = true;
                }
                // check for the special case where the list is in the "magic"
                // state of having only the default link in it. In that case
                // we want to clear the list before adding this link.
                //
                if (owner.links.Count == 1 
                    && this[0].Start == 0
                    && this[0].length == -1) {

                    owner.links.Clear();
                    owner.FocusLink = null;
                }

                Link l = new Link(owner);
                l.Start = start;
                l.Length = length;
                l.LinkData = linkData;
                Add(l);
                return l;
            }

            /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.LinkCollection.Add2"]/*' />
            public int Add(Link value) {
                if (value != null && value.Length != 0) {
                    linksAdded = true;
                }
                // check for the special case where the list is in the "magic"
                // state of having only the default link in it. In that case
                // we want to clear the list before adding this link.
                //
                if (owner.links.Count == 1 
                    && this[0].Start == 0
                    && this[0].length == -1) {

                    owner.links.Clear();
                    owner.FocusLink = null;
                }

                // Set the owner control for this link
                value.Owner = this.owner;

                owner.links.Add(value);

                if (this.owner.AutoSize) {
                    LayoutTransaction.DoLayout(this.owner.ParentInternal, this.owner, PropertyNames.Links);
                    this.owner.AdjustSize();
                    this.owner.Invalidate();
                }

                if (owner.Links.Count > 1) {
                    owner.links.Sort(LinkLabel.linkComparer);
                }

                owner.ValidateNoOverlappingLinks();
                owner.UpdateSelectability();
                owner.InvalidateTextLayout();
                owner.Invalidate();

                if (owner.Links.Count > 1) {
                    return IndexOf(value);
                }
                else {
                    return 0;
                }
            }
            
            /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkCollection.IList.Add"]/*' />
            /// <internalonly/>
            int IList.Add(object value) {
                if (value is Link) {
                    return Add((Link)value);
                }
                else {  
                    throw new ArgumentException(SR.LinkLabelBadLink,"value");
                }
            }
            
            /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkCollection.IList.Insert"]/*' />
            /// <internalonly/>
            void IList.Insert(int index, object value) {
                if (value is Link) {
                    Add((Link)value);
                }
                else {  
                    throw new ArgumentException(SR.LinkLabelBadLink, "value");
                }
            }
    
            /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.LinkCollection.Contains"]/*' />
            public bool Contains(Link link) {
                return owner.links.Contains(link);
            }

            /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.LinkCollection.ContainsKey"]/*' />
            /// <devdoc>
            ///     <para>Returns true if the collection contains an item with the specified key, false otherwise.</para>
            /// </devdoc>
            public virtual bool ContainsKey(string key) {
               return IsValidIndex(IndexOfKey(key)); 
            }
        
            /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkCollection.IList.Contains"]/*' />
            /// <internalonly/>
            bool IList.Contains(object link) {
                if (link is Link) {
                    return Contains((Link)link);
                }
                else {  
                    return false;
                }
            }
            
            /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.LinkCollection.IndexOf"]/*' />
            public int IndexOf(Link link) {
                return owner.links.IndexOf(link);
            }
            
            /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkCollection.IList.IndexOf"]/*' />
            /// <internalonly/>
            int IList.IndexOf(object link) {
                if (link is Link) {
                    return IndexOf((Link)link);
                }
                else {  
                    return -1;
                }
            }

            /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.LinkCollection.this"]/*' />
            /// <devdoc>
            ///     <para>The zero-based index of the first occurrence of value within the entire CollectionBase, if found; otherwise, -1.</para>
            /// </devdoc>
            public virtual int  IndexOfKey(String key) {
                // Step 0 - Arg validation
                if (string.IsNullOrEmpty(key)){
                    return -1; // we dont support empty or null keys.
                }

                // step 1 - check the last cached item
                if (IsValidIndex(lastAccessedIndex))
                {
                    if (WindowsFormsUtils.SafeCompareStrings(this[lastAccessedIndex].Name, key, /* ignoreCase = */ true)) {
                        return lastAccessedIndex;
                    }
                }

                // step 2 - search for the item
                for (int i = 0; i < this.Count; i ++) {
                    if (WindowsFormsUtils.SafeCompareStrings(this[i].Name, key, /* ignoreCase = */ true)) {
                        lastAccessedIndex = i;
                        return i;
                    }
                }

                // step 3 - we didn't find it.  Invalidate the last accessed index and return -1.
                lastAccessedIndex = -1;
                return -1;
            }

            /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.LinkCollection.IsValidIndex"]/*' />
            /// <devdoc>
            ///     <para>Determines if the index is valid for the collection.</para>
            /// </devdoc>
            /// <internalonly/> 
            private bool IsValidIndex(int index) {
                return ((index >= 0) && (index < this.Count));
            }

            
            /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.LinkCollection.Clear"]/*' />
            /// <devdoc>
            ///    Remove all links from the linkLabel.
            /// </devdoc>
            public virtual void Clear() {
                bool doLayout = this.owner.links.Count > 0 && this.owner.AutoSize;
                owner.links.Clear();

                if (doLayout) {
                    LayoutTransaction.DoLayout(this.owner.ParentInternal, this.owner, PropertyNames.Links);
                    this.owner.AdjustSize();
                    this.owner.Invalidate();
                }

                owner.UpdateSelectability();
                owner.InvalidateTextLayout();
                owner.Invalidate();
            }

            /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkCollection.ICollection.CopyTo"]/*' />
            /// <internalonly/>
            void ICollection.CopyTo(Array dest, int index) {
                owner.links.CopyTo(dest, index);
            }

            /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.LinkCollection.GetEnumerator"]/*' />
            public IEnumerator GetEnumerator() {
                if (owner.links != null) {
                    return owner.links.GetEnumerator();
                }
                else {
                    return new Link[0].GetEnumerator();
                }
            }
            
            /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.LinkCollection.Remove"]/*' />
            public void Remove(Link value) {

                if (value.Owner != this.owner) {
                    return;
                }

                owner.links.Remove(value);

                if (this.owner.AutoSize) {
                    LayoutTransaction.DoLayout(this.owner.ParentInternal, this.owner, PropertyNames.Links);
                    this.owner.AdjustSize();
                    this.owner.Invalidate();
                }

                owner.links.Sort(LinkLabel.linkComparer);

                owner.ValidateNoOverlappingLinks();
                owner.UpdateSelectability();
                owner.InvalidateTextLayout();
                owner.Invalidate();

                if (owner.FocusLink == null && owner.links.Count > 0) {
                    owner.FocusLink = (Link)owner.links[0];
                }
            }
            
            /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.LinkCollection.RemoveAt"]/*' />
            public void RemoveAt(int index) {
                Remove(this[index]);
            }
  
            /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="Control.ControlCollection.RemoveByKey"]/*' />
            /// <devdoc>
            ///     <para>Removes the child control with the specified key.</para>
            /// </devdoc>
            public virtual void RemoveByKey(string key) {
                    int index = IndexOfKey(key);
                    if (IsValidIndex(index)) {
                        RemoveAt(index); 
                     }
               }
            
            /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkCollection.IList.Remove"]/*' />
            /// <internalonly/>
            void IList.Remove(object value) {
                if (value is Link) {
                    Remove((Link)value);
                }                
            }
        }

        /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.Link"]/*' />
        [
        TypeConverter(typeof(LinkConverter))
        ]
        public class Link {
            private int start = 0;  
            private object linkData = null;
            private LinkState state = LinkState.Normal;
            private bool enabled = true;
            private Region visualRegion;
            internal int length = 0;  
            private LinkLabel owner = null;
            private string name = null;
            private string description = null;

            private object userData;

            /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="Link.Link"]/*' />
            public Link() {
            }

            /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="Link.Link1"]/*' />
            public Link(int start, int length) {
                this.start = start;
                this.length = length;
            }

            /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="Link.Link2"]/*' />
            public Link(int start, int length, object linkData) {
                this.start = start;
                this.length = length;
                this.linkData = linkData;
            }

            internal Link(LinkLabel owner) {
                this.owner = owner;
            }

            /// <devdoc>
            ///    <para>Description for accessibility</para>
            /// </devdoc>
            public string Description {
                get {
                    return description;
                }
                set {
                    description = value;
                }
            }

            /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.Link.Enabled"]/*' />
            [DefaultValue(true)]
            public bool Enabled {
                get {
                    return enabled;
                }
                set {
                    if (enabled != value) {
                        enabled = value;

                        if ((int)(state & (LinkState.Hover | LinkState.Active)) != 0) {
                            state &= ~(LinkState.Hover | LinkState.Active);
                            if (owner != null) {
                                owner.OverrideCursor = null;
                            }
                        }

                        if (owner != null) {
                            owner.InvalidateLink(this);
                        }
                    }
                }
            }

            /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.Link.Length"]/*' />
            public int Length {
                get { 
                    if (length == -1) {
                        if (owner != null && !String.IsNullOrEmpty(owner.Text)) {
                            StringInfo stringInfo = new StringInfo(owner.Text);
                            return stringInfo.LengthInTextElements - Start;
                        }
                        else {
                            return 0;
                        }
                    }
                    return length;
                }
                set {
                    if (length != value) {
                        length = value;
                        if (owner != null) {
                            owner.InvalidateTextLayout();
                            owner.Invalidate();
                        }
                    }
                }
            }

            /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.Link.LinkData"]/*' />
            [DefaultValue(null)]
            public object LinkData {
                get {
                    return linkData;
                }
                set {
                    linkData = value;
                }
            }

            /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.Link.Owner"]/*' />
            /// <devdoc>
            ///    <para>The LinkLabel object that owns this link.</para>
            /// </devdoc>
            internal LinkLabel Owner {
                get {
                    return owner;
                }
                set {
                    owner = value;
                }
            }

            internal LinkState State {
                get {
                    return state;
                }
                set {
                    state = value;
                }
            }
            /// <include file='doc\TreeNode.uex' path='docs/doc[@for="LinkLabel.Link.Name"]/*' />
            /// <devdoc>
            ///     The name for the link - useful for indexing by key.
            /// </devdoc>
            [
            DefaultValue(""),
            SRCategory(nameof(SR.CatAppearance)),
            SRDescription(nameof(SR.TreeNodeNodeNameDescr))
            ]
            public string Name {
                get {
                    return name == null ? "" : name;
                }
                set {
                    this.name = value;
                }
            }

            /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.Link.Start"]/*' />
            public int Start { 
                get {
                    return start;
                }
                set {
                    if (start != value) {
                        start = value;

                        if (owner != null) {
                            owner.links.Sort(LinkLabel.linkComparer);
                            owner.InvalidateTextLayout();
                            owner.Invalidate();
                        }
                    }
                }
            }

            /// <include file='doc\ColumnHeader.uex' path='docs/doc[@for="ColumnHeader.Tag"]/*' />
            [
            SRCategory(nameof(SR.CatData)),
            Localizable(false),
            Bindable(true),
            SRDescription(nameof(SR.ControlTagDescr)),
            DefaultValue(null),
            TypeConverter(typeof(StringConverter)),
            ]
            public object Tag {
                get {
                    return userData;
                }
                set {
                    userData = value;
                }
            }

            /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.Link.Visited"]/*' />
            [DefaultValue(false)]
            public bool Visited {
                get {
                    return(State & LinkState.Visited) == LinkState.Visited;
                }
                set {
                    bool old = Visited;

                    if (value) {
                        State |= LinkState.Visited;
                    }
                    else {
                        State &= ~LinkState.Visited;
                    }

                    if (old != Visited && owner != null) {
                        owner.InvalidateLink(this);
                    }
                }
            }

            internal Region VisualRegion {
                get {
                    return visualRegion;
                }
                set {
                    visualRegion = value;
                }
            }
        }

        /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.LinkComparer"]/*' />
        private class LinkComparer : IComparer {
            int IComparer.Compare(object link1, object link2) {

                Debug.Assert(link1 != null && link2 != null, "Null objects sent for comparison");

                int pos1 = ((Link)link1).Start;
                int pos2 = ((Link)link2).Start;

                return pos1 - pos2;                                       
            }
        }

        /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.LinkLabelAccessibleObject"]/*' />
        /// <internalonly/>        
        /// <devdoc>
        /// </devdoc>
        [System.Runtime.InteropServices.ComVisible(true)]
        internal class LinkLabelAccessibleObject : LabelAccessibleObject {
            /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.LinkLabelAccessibleObject.LinkLabelAccessibleObject"]/*' />
            /// <devdoc>
            /// </devdoc>
            public LinkLabelAccessibleObject(LinkLabel owner) : base(owner) {
            }

            internal override bool IsIAccessibleExSupported() {
                if (AccessibilityImprovements.Level3) {
                    return true;
                }

                return base.IsIAccessibleExSupported();
            }

            /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.LinkLabelAccessibleObject.GetChild"]/*' />
            /// <devdoc>
            /// </devdoc>
            public override AccessibleObject GetChild(int index) {
                if (index >= 0 && index < ((LinkLabel)Owner).Links.Count) {
                    return new LinkAccessibleObject(((LinkLabel)Owner).Links[index]);
                }
                else {
                    return null;
                }
            }

            internal override object GetPropertyValue(int propertyID) {
                if (propertyID == NativeMethods.UIA_IsEnabledPropertyId) {
                    if (!Owner.Enabled) {
                        return false;
                    }
                }

                return base.GetPropertyValue(propertyID);
            }
            
            public override System.Windows.Forms.AccessibleObject HitTest(int x,  int y) {
                Point p = Owner.PointToClient(new Point(x, y));
                Link hit = ((LinkLabel)Owner).PointInLink(p.X, p.Y);
                if (hit != null) {
                    return new LinkAccessibleObject(hit);
                }
                if (this.Bounds.Contains(x, y)) {
                    return this;
                }
                return null;
            }

            /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.LinkLabelAccessibleObject.GetChildCount"]/*' />
            /// <devdoc>
            /// </devdoc>
            public override int GetChildCount() {
                return((LinkLabel)Owner).Links.Count;
            }
        }

        /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.LinkAccessibleObject"]/*' />
        /// <internalonly/>        
        /// <devdoc>
        /// </devdoc>
        [System.Runtime.InteropServices.ComVisible(true)]        
        internal class LinkAccessibleObject : AccessibleObject {

            private Link link;

            public LinkAccessibleObject(Link link) : base() {
                this.link = link;
            }

            public override Rectangle Bounds {
                get {
                    Region region = link.VisualRegion;
                    Graphics g = null;

                    IntSecurity.ObjectFromWin32Handle.Assert();
                    try
                    {
                        g = Graphics.FromHwnd(link.Owner.Handle);
                    }
                    finally 
                    {
                        CodeAccessPermission.RevertAssert();
                    }

                    // Make sure we have a region for this link
                    //
                    if (region == null) {
                        link.Owner.EnsureRun(g);
                        region = link.VisualRegion;
                        if (region == null) {
                            g.Dispose();
                            return Rectangle.Empty;
                        }
                    }

                    Rectangle rect;
                    try {
                        rect = Rectangle.Ceiling(region.GetBounds(g));
                    }
                    finally {
                        g.Dispose();
                    }


                    // Translate rect to screen coordinates
                    //
                    return link.Owner.RectangleToScreen(rect);
                }
            }

            public override string DefaultAction {
                get {
                    return SR.AccessibleActionClick;
                }
            }

            public override string Description {
                get {
                    return link.Description;
                }
            }
            
            public override string Name {
                get {          
                    string text = link.Owner.Text;
                    string name;
                    if (AccessibilityImprovements.Level3) {
                        // return the full name of the link label for AI.Level3 
                        // as sometimes the link name in isolation is unusable
                        // to a customer using a screen reader
                        name = text;
                        if (link.Owner.UseMnemonic) {
                            name = WindowsFormsUtils.TextWithoutMnemonics(name);
                        }
                    } else {
                        int charStart = LinkLabel.ConvertToCharIndex(link.Start, text);
                        int charEnd = LinkLabel.ConvertToCharIndex(link.Start + link.Length, text);
                        name = text.Substring(charStart, charEnd - charStart);
                        if (AccessibilityImprovements.Level1 && link.Owner.UseMnemonic) {
                            // return the same value as the tooltip shows.
                            name = WindowsFormsUtils.TextWithoutMnemonics(name);
                        }
                    }

                    return name;
                }
                set {
                    base.Name = value;
                }
            }

            public override AccessibleObject Parent {
                [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
                get {
                    return link.Owner.AccessibilityObject;                
                }
            }

            public override AccessibleRole Role {
                get {
                    return AccessibleRole.Link;
                }
            }

            public override AccessibleStates State {
                get {
                    AccessibleStates state = AccessibleStates.Focusable;

                    // Selected state
                    //
                    if (link.Owner.FocusLink == link) {
                        state |= AccessibleStates.Focused;
                    }

                    return state;

                }
            }

            public override string Value {
                [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
                get {
                    if (AccessibilityImprovements.Level1) {
                        // Narrator announces Link's text twice, once as a Name property and once as a Value, thus removing value.
                        // Value is optional for this role (Link).
                        return string.Empty;
                    } 
                    return Name;
                }
            }

            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            public override void DoDefaultAction() {
                link.Owner.OnLinkClicked(new LinkLabelLinkClickedEventArgs(link));
            }

            internal override bool IsIAccessibleExSupported() {
                if (AccessibilityImprovements.Level3) {
                    return true;
                }

                return base.IsIAccessibleExSupported();
            }

            internal override object GetPropertyValue(int propertyID) {
                if (propertyID == NativeMethods.UIA_IsEnabledPropertyId) {
                    if (!link.Owner.Enabled) {
                        return false;
                    }
                }

                return base.GetPropertyValue(propertyID);
            }
        }
    }
}
