// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {
    using System.ComponentModel;
    using System.Diagnostics;
    using System;
    using System.Drawing;
    using System.Drawing.Design;
    using System.ComponentModel.Design;
    using System.Text;
    using System.Windows.Forms;
    using System.Security.Permissions;
    using Microsoft.Win32;
    using System.Windows.Forms.VisualStyles;
    using System.Runtime.InteropServices;
    using System.Globalization;
    using System.Windows.Forms.Layout;

    /// <include file='doc\TabPage.uex' path='docs/doc[@for="TabPage"]/*' />
    /// <devdoc>
    ///     TabPage implements a single page of a tab control.  It is essentially
    ///     a Panel with TabItem properties.
    /// </devdoc>
    [
    ComVisible(true),
    ClassInterface(ClassInterfaceType.AutoDispatch),
    Designer("System.Windows.Forms.Design.TabPageDesigner, " + AssemblyRef.SystemDesign),
    ToolboxItem (false),
    DesignTimeVisible (false),
    DefaultEvent ("Click"),
    DefaultProperty ("Text")
    ]
    public class TabPage : Panel {
        private ImageList.Indexer imageIndexer;
        private string toolTipText = "";
        private bool enterFired = false;
        private bool leaveFired = false;
        private bool useVisualStyleBackColor = false;

        
        /// <include file='doc\TabPage.uex' path='docs/doc[@for="TabPage.TabPage"]/*' />
        /// <devdoc>
        ///     Constructs an empty TabPage.
        /// </devdoc>
        public TabPage ()
        : base() {
            SetStyle (ControlStyles.CacheText, true);
            Text = null;
        }

        /// <devdoc>
        ///     Allows the control to optionally shrink when AutoSize is true.
        /// </devdoc>
        [
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Browsable(false),
        Localizable(false)
        ]
        public override AutoSizeMode AutoSizeMode {
            get {
                return AutoSizeMode.GrowOnly;
            }
            set {
            }
        }

        /// <devdoc>
        ///     <para>Hide AutoSize: it doesn't make sense for this control</para>
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool AutoSize
        {
            get
            {
                return base.AutoSize;
            }
            set
            {
                base.AutoSize = value;
            }
        }

        /// <include file='doc\TabPage.uex' path='docs/doc[@for="TabPage.AutoSizeChanged"]/*' />
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ControlOnAutoSizeChangedDescr))]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler AutoSizeChanged
        {
            add
            {
                base.AutoSizeChanged += value;
            }
            remove
            {
                base.AutoSizeChanged -= value;
            }
        }


        /// <include file='doc\TabPage.uex' path='docs/doc[@for="TabPage.BackColor"]/*' />
        /// <devdoc>
        ///     The background color of this control. This is an ambient property and
        ///     will always return a non-null value.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.ControlBackColorDescr))
        ]
        public override Color BackColor {
            get {
                Color color = base.BackColor;
                // If some color is Set by the user return that...
                if (color != DefaultBackColor)
                {
                    return color;
                }
                // If user has not set a color and if XP theming ON  and Parent's appearance is Normal, then return the Transparent Color....
                TabControl parent = ParentInternal as TabControl;
                if (Application.RenderWithVisualStyles && UseVisualStyleBackColor && (parent != null && parent.Appearance == TabAppearance.Normal)) {
                    return Color.Transparent;
                }
                // return base.Color by default...
                return color;
            }
            set {
                if (DesignMode) {
                    if (value != Color.Empty) {
                        PropertyDescriptor pd = TypeDescriptor.GetProperties(this)["UseVisualStyleBackColor"];
                        Debug.Assert(pd != null);
                        if (pd != null) {
                            pd.SetValue(this, false);
                        }
                    }
                }
                else {
                    UseVisualStyleBackColor = false;
                }

                base.BackColor = value;
            }
        }

        /// <include file='doc\TabPage.uex' path='docs/doc[@for="TabPage.CreateControlsInstance"]/*' />
        /// <devdoc>
        ///     Constructs the new instance of the Controls collection objects. Subclasses
        ///     should not call base.CreateControlsInstance.  Our version creates a control
        ///     collection that does not support
        /// </devdoc>
        protected override Control.ControlCollection CreateControlsInstance () {
            return new TabPageControlCollection (this);
        }


        /// <include file='doc\TabPage.uex' path='docs/doc[@for="TabPage.ImageIndexer"]/*' />
        /// <internalonly/>
        internal ImageList.Indexer ImageIndexer {
            get {
                if (imageIndexer == null) {
                    imageIndexer = new ImageList.Indexer ();
                }

                return imageIndexer;
            }
        }

        /// <include file='doc\TabPage.uex' path='docs/doc[@for="TabPage.ImageIndex"]/*' />
        /// <devdoc>
        ///     Returns the imageIndex for the tabPage.  This should point to an image
        ///     in the TabControl's associated imageList that will appear on the tab, or be -1.
        /// </devdoc>
        [
        TypeConverterAttribute (typeof(ImageIndexConverter)),
        Editor ("System.Windows.Forms.Design.ImageIndexEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor)),
        Localizable (true),
        RefreshProperties(RefreshProperties.Repaint),
        DefaultValue (-1),
        SRDescription (nameof(SR.TabItemImageIndexDescr))
        ]
        public int ImageIndex {
            get {
                return ImageIndexer.Index;
            }
            set {
                if (value < -1) {
                    throw new ArgumentOutOfRangeException ("ImageIndex", string.Format (SR.InvalidLowBoundArgumentEx,  "imageIndex", (value).ToString (CultureInfo.CurrentCulture), (-1).ToString(CultureInfo.CurrentCulture)));
                }
                TabControl parent = ParentInternal as TabControl;

                if (parent != null) {
                    this.ImageIndexer.ImageList = parent.ImageList;
                }

                this.ImageIndexer.Index = value;
                UpdateParent ();
            }
        }

        /// <include file='doc\TabPage.uex' path='docs/doc[@for="TabPage.ImageIndex"]/*' />
        /// <devdoc>
        ///     Returns the imageIndex for the tabPage.  This should point to an image
        ///     in the TabControl's associated imageList that will appear on the tab, or be -1.
        /// </devdoc>
        [
        TypeConverterAttribute (typeof(ImageKeyConverter)),
        Editor ("System.Windows.Forms.Design.ImageIndexEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor)),
        Localizable (true),
        DefaultValue (""),
        RefreshProperties(RefreshProperties.Repaint),
        SRDescription(nameof(SR.TabItemImageIndexDescr))
        ]
        public string ImageKey {
            get {
                return ImageIndexer.Key;
            }
            set {
                this.ImageIndexer.Key = value;

                TabControl parent = ParentInternal as TabControl;

                if (parent != null) {
                    this.ImageIndexer.ImageList = parent.ImageList;
                }

                UpdateParent ();
            }
        }

        /// <include file='doc\TabPage.uex' path='docs/doc[@for="TabPage.TabPage1"]/*' />
        /// <devdoc>
        ///     Constructs a TabPage with text for the tab.
        /// </devdoc>
        public TabPage (string text) : this() {
            Text = text;
        }

        /// <include file='doc\TabPage.uex' path='docs/doc[@for="TabPage.Anchor"]/*' />
        [Browsable (false), EditorBrowsable (EditorBrowsableState.Never)]
        public override AnchorStyles Anchor {
            get {
                return base.Anchor;
            }
            set {
                base.Anchor = value;
            }
        }

        /// <include file='doc\TabPage.uex' path='docs/doc[@for="TabPage.Dock"]/*' />
        [Browsable (false), EditorBrowsable (EditorBrowsableState.Never)]
        public override DockStyle Dock {
            get {
                return base.Dock;
            }
            set {
                base.Dock = value;
            }
        }

        /// <include file='doc\TabPage.uex' path='docs/doc[@for="TabPage.DockChanged"]/*' />
        /// <internalonly/>
        [Browsable (false), EditorBrowsable (EditorBrowsableState.Never)]
        new public event EventHandler DockChanged {
            add {
                base.DockChanged += value;
            }
            remove {
                base.DockChanged -= value;
            }
        }

        /// <include file='doc\TabPage.uex' path='docs/doc[@for="TabPage.Enabled"]/*' />
        [Browsable (false), EditorBrowsable (EditorBrowsableState.Never)]
        new public bool Enabled {
            get {
                return base.Enabled;
            }
            set {
                base.Enabled = value;
            }
        }

        /// <include file='doc\TabPage.uex' path='docs/doc[@for="TabPage.EnabledChanged"]/*' />
        /// <internalonly/>
        [Browsable (false), EditorBrowsable (EditorBrowsableState.Never)]
        new public event EventHandler EnabledChanged {
            add {
                base.EnabledChanged += value;
            }
            remove {
                base.EnabledChanged -= value;
            }
        }

        /// <include file='doc\TabPage.uex' path='docs/doc[@for="TabPage.UseVisualStyleBackColor"]/*' />
        /// <internalonly/>
        [
        DefaultValue (false),
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.TabItemUseVisualStyleBackColorDescr))
        ]
        public bool UseVisualStyleBackColor {
            get {
                return useVisualStyleBackColor;
            }
            set {
                useVisualStyleBackColor = value;
                this.Invalidate(true);
            }
        }
        
        /// <include file='doc\TabPage.uex' path='docs/doc[@for="TabPage.Location"]/*' />
        /// <internalonly/>
        // Make the Location property non-browsable for the TabPages.
        [Browsable (false), EditorBrowsable (EditorBrowsableState.Never)]
        new public Point Location {
            get {
                return base.Location;
            }
            set {
                base.Location = value;
            }
        }

        
        /// <include file='doc\TabPage.uex' path='docs/doc[@for="TabPage.LocationChanged"]/*' />
        /// <internalonly/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler LocationChanged {
            add {
                base.LocationChanged += value;
            }
            remove {
                base.LocationChanged -= value;
            }
        }

        /// <include file='doc\TabPage.uex' path='docs/doc[@for="TabPage.MaximumSize"]/*' />
        [DefaultValue(typeof(Size), "0, 0")]
        [Browsable (false), EditorBrowsable (EditorBrowsableState.Never)]
        public override Size MaximumSize {
            get { return base.MaximumSize; }

            set { 
                base.MaximumSize = value;
            }
        }

        /// <include file='doc\TabPage.uex' path='docs/doc[@for="TabPage.MinimumSize"]/*' />
        [Browsable (false), EditorBrowsable (EditorBrowsableState.Never)]
        public override Size MinimumSize {
            get { return base.MinimumSize; }

            set { 
                base.MinimumSize = value;
            }
        }

        /// <include file='doc\TabPage.uex' path='docs/doc[@for="TabPage.PreferredSize"]/*' />
        [Browsable (false), EditorBrowsable (EditorBrowsableState.Never)]
        public new Size PreferredSize {
            get { return base.PreferredSize; }
        }

        /// <include file='doc\TabPage.uex' path='docs/doc[@for="TabPage.TabIndex"]/*' />
        [Browsable (false), EditorBrowsable (EditorBrowsableState.Never)]
        new public int TabIndex {
            get {
                return base.TabIndex;
            }
            set {
                base.TabIndex = value;
            }
        }


        /// <devdoc>
        /// This property is required by certain controls (TabPage) to render its transparency using theming API.
        /// We dont want all controls (that are have transparent BackColor) to use theming API to render its background because it has  HUGE PERF cost.
        /// </devdoc>
        /// <internalonly/>
        internal override bool RenderTransparencyWithVisualStyles {
            get {
                return true;
            }
        }

        /// <include file='doc\TabPage.uex' path='docs/doc[@for="TabPage.TabIndexChanged"]/*' />
        /// <internalonly/>
        [Browsable (false), EditorBrowsable (EditorBrowsableState.Never)]
        new public event EventHandler TabIndexChanged {
            add {
                base.TabIndexChanged += value;
            }
            remove {
                base.TabIndexChanged -= value;
            }
        }

        /// <include file='doc\TabPage.uex' path='docs/doc[@for="TabPage.TabStop"]/*' />
        [Browsable (false), EditorBrowsable (EditorBrowsableState.Never)]
        new public bool TabStop {
            get {
                return base.TabStop;
            }
            set {
                base.TabStop = value;
            }
        }

        /// <include file='doc\TabPage.uex' path='docs/doc[@for="TabPage.TabStopChanged"]/*' />
        /// <internalonly/>
        [Browsable (false), EditorBrowsable (EditorBrowsableState.Never)]
        new public event EventHandler TabStopChanged {
            add {
                base.TabStopChanged += value;
            }
            remove {
                base.TabStopChanged -= value;
            }
        }

        /// <include file='doc\TabPage.uex' path='docs/doc[@for="TabPage.Text"]/*' />
        [
        Localizable (true),
        Browsable (true),
        EditorBrowsable (EditorBrowsableState.Always)
        ]
        public override string Text {
            get {
                return base.Text;
            }
            set {
                base.Text = value;
                UpdateParent ();
            }
        }

        /// <include file='doc\TabPage.uex' path='docs/doc[@for="TabPage.TextChanged"]/*' />
        /// <internalonly/>
        [Browsable (true), EditorBrowsable (EditorBrowsableState.Always)]
        new public event EventHandler TextChanged {
            add {
                base.TextChanged += value;
            }
            remove {
                base.TextChanged -= value;
            }
        }

        /// <include file='doc\TabPage.uex' path='docs/doc[@for="TabPage.ToolTipText"]/*' />
        /// <devdoc>
        ///     The toolTipText for the tab, that will appear when the mouse hovers
        ///     over the tab and the TabControl's showToolTips property is true.
        /// </devdoc>
        [
        DefaultValue (""),
        Localizable (true),
        SRDescription (nameof(SR.TabItemToolTipTextDescr))
        ]
        public string ToolTipText {
            get {
                return toolTipText;
            }
            set {
                if (value == null) {
                    value = "";
                }

                if (value == toolTipText)
                    return;

                toolTipText = value;
                UpdateParent ();
            }
        }

        /// <include file='doc\TabPage.uex' path='docs/doc[@for="TabPage.Visible"]/*' />
        [Browsable (false), EditorBrowsable (EditorBrowsableState.Never)]
        new public bool Visible {
            get {
                return base.Visible;
            }
            set {
                base.Visible = value;
            }
        }

        /// <include file='doc\TabPage.uex' path='docs/doc[@for="TabPage.VisibleChanged"]/*' />
        /// <internalonly/>
        [Browsable (false), EditorBrowsable (EditorBrowsableState.Never)]
        new public event EventHandler VisibleChanged {
            add {
                base.VisibleChanged += value;
            }
            remove {
                base.VisibleChanged -= value;
            }
        }

        /// <devdoc>
        ///     Assigns a new parent control. Sends out the appropriate property change
        ///     notifications for properties that are affected by the change of parent.
        /// </devdoc>
        internal override void AssignParent (Control value) {
            if (value != null && !(value is TabControl)) {
                throw new ArgumentException (string.Format (SR.TABCONTROLTabPageNotOnTabControl, value.GetType ().FullName));
            }

            base.AssignParent (value);
        }

        /// <include file='doc\TabPage.uex' path='docs/doc[@for="TabPage.GetTabPageOfComponent"]/*' />
        /// <devdoc>
        ///     Given a component, this retrieves the tab page that it's parented to, or
        /// null if it's not parented to any tab page.
        /// </devdoc>
        public static TabPage GetTabPageOfComponent (Object comp) {
            if (!(comp is Control)) {
                return null;
            }

            Control c = (Control)comp;

            while (c != null && !(c is TabPage)) {
                c = c.ParentInternal;
            }

            return (TabPage)c;
        }

        /// <include file='doc\TabPage.uex' path='docs/doc[@for="TabPage.GetTCITEM"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        internal NativeMethods.TCITEM_T GetTCITEM () {
            NativeMethods.TCITEM_T tcitem = new NativeMethods.TCITEM_T ();

            tcitem.mask = 0;
            tcitem.pszText = null;
            tcitem.cchTextMax = 0;
            tcitem.lParam = IntPtr.Zero;

            string text = Text;

            PrefixAmpersands (ref text);
            if (text != null) {
                tcitem.mask |= NativeMethods.TCIF_TEXT;
                tcitem.pszText = text;
                tcitem.cchTextMax = text.Length;
            }

            int imageIndex = ImageIndex;

            tcitem.mask |= NativeMethods.TCIF_IMAGE;
            tcitem.iImage = ImageIndexer.ActualIndex;
            return tcitem;
        }

        private void PrefixAmpersands (ref string value) {
            // Due to a comctl32 problem, ampersands underline the next letter in the 
            // text string, but the accelerators don't work.
            // So in this function, we prefix ampersands with another ampersand
            // so that they actually appear as ampersands.
            //
            // Sanity check parameter
            //
            if (value == null || value.Length == 0) {
                return;
            }

            // If there are no ampersands, we don't need to do anything here
            //
            if (value.IndexOf ('&') < 0) {
                return;
            }

            // Insert extra ampersands
            //
            StringBuilder newString = new StringBuilder ();

            for (int i = 0; i < value.Length; ++i) {
                if (value[i] == '&') {
                    if (i < value.Length - 1 && value[i + 1] == '&') {
                        ++i;    // Skip the second ampersand
                    }

                    newString.Append ("&&");
                }
                else {
                    newString.Append (value[i]);
                }
            }

            value = newString.ToString ();
        }

        /// <devdoc>
        /// This is an internal method called by the TabControl to fire the Leave event when TabControl leave occurs.
        /// </devdoc>
        internal void FireLeave (EventArgs e) {
            leaveFired = true;
            OnLeave (e);
        }

        /// <devdoc>
        /// This is an internal method called by the TabControl to fire the Enter event when TabControl leave occurs.
        /// </devdoc>
        internal void FireEnter (EventArgs e) {
            enterFired = true;
            OnEnter (e);
        }

        /// <include file='doc\TabPage.uex' path='docs/doc[@for="TabPage.OnEnter"]/*' />
        /// <devdoc>
        ///     Actually goes and fires the OnEnter event.  Inheriting controls
        ///     should use this to know when the event is fired [this is preferable to
        ///     adding an event handler on yourself for this event].  They should,
        ///     however, remember to call base.OnEnter(e); to ensure the event is
        ///     still fired to external listeners
        ///     This listener is overidden so that we can fire SAME ENTER and LEAVE 
        ///     events on the TabPage.
        ///     TabPage should fire enter when the focus is on the TABPAGE and not when the control
        ///     within the TabPage gets Focused.
        /// </devdoc>
        protected override void OnEnter (EventArgs e) {
            TabControl parent = ParentInternal as TabControl;

            if (parent != null) {
                if (enterFired) {
                    base.OnEnter (e);
                }

                enterFired = false;
            }
        }

        /// <include file='doc\TabPage.uex' path='docs/doc[@for="TabPage.OnLeave"]/*' />
        /// <devdoc>
        ///     Actually goes and fires the OnLeave event.  Inheriting controls
        ///     should use this to know when the event is fired [this is preferable to
        ///     adding an event handler on yourself for this event].  They should,
        ///     however, remember to call base.OnLeave(e); to ensure the event is
        ///     still fired to external listeners
        ///     This listener is overidden so that we can fire SAME ENTER and LEAVE 
        ///     events on the TabPage.
        ///     TabPage should fire enter when the focus is on the TABPAGE and not when the control
        ///     within the TabPage gets Focused.
        ///     Similary the Leave should fire when the TabControl (and hence the TabPage) looses
        ///     Focus. 
        /// </devdoc>
        protected override void OnLeave (EventArgs e) {
            TabControl parent = ParentInternal as TabControl;

            if (parent != null) {
                if (leaveFired) {
                    base.OnLeave (e);
                }

                leaveFired = false;
            }
        }

        /// <include file='doc\TabPage.uex' path='docs/doc[@for="TabPage.OnPaintBackground"]/*' />
        protected override void OnPaintBackground (PaintEventArgs e) {

            // Utilize the TabRenderer new to Whidbey to draw the tab pages so that the
            // panels are drawn using the correct visual styles when the application supports using visual
            // styles.

            // Does this application utilize Visual Styles?
            // Utilize the UseVisualStyleBackColor property to determine whether or
            // not the themed background should be utilized.
            TabControl parent = ParentInternal as TabControl;
            if (Application.RenderWithVisualStyles && UseVisualStyleBackColor && (parent != null && parent.Appearance == TabAppearance.Normal)) {

                Color bkcolor = UseVisualStyleBackColor ? Color.Transparent : this.BackColor;
                Rectangle inflateRect = LayoutUtils.InflateRect(DisplayRectangle, Padding);



                //To ensure that the tabpage draws correctly (the border will get clipped and
                // and gradient fill will match correctly with the tabcontrol).  Unfortunately, there is no good way to determine
                // the padding used on the tabpage.
                // I would like to use the following below, but GetMargins is busted in the theming API:
                //VisualStyleRenderer visualStyleRenderer = new VisualStyleRenderer(VisualStyleElement.Tab.Pane.Normal);
                //Padding themePadding = visualStyleRenderer.GetMargins(e.Graphics, MarginProperty.ContentMargins);
                //Rectangle rectWithBorder = new Rectangle(inflateRect.X - themePadding.Left,
                //    inflateRect.Y - themePadding.Top,
                //    inflateRect.Width + themePadding.Right + themePadding.Left,
                //    inflateRect.Height + themePadding.Bottom + themePadding.Top); 
                Rectangle rectWithBorder = new Rectangle(inflateRect.X - 4, inflateRect.Y - 2, inflateRect.Width + 8, inflateRect.Height + 6);

                TabRenderer.DrawTabPage(e.Graphics, rectWithBorder);

                // Is there a background image to paint? The TabRenderer does not currently support
                // painting the background image on the panel, so we need to draw it ourselves.
                if (this.BackgroundImage != null) {
                    ControlPaint.DrawBackgroundImage(e.Graphics, BackgroundImage, bkcolor, BackgroundImageLayout, inflateRect, inflateRect, DisplayRectangle.Location);
                }
            }
            else {
                base.OnPaintBackground (e);
            }
        }

        /// <include file='doc\TabPage.uex' path='docs/doc[@for="TabPage.SetBoundsCore"]/*' />
        /// <devdoc>
        ///     overrides main setting of our bounds so that we can control our size and that of our
        ///     TabPages...
        /// </devdoc>
        /// <internalonly/>
        protected override void SetBoundsCore (int x, int y, int width, int height, BoundsSpecified specified) {
            Control parent = ParentInternal;

            if (parent is TabControl && parent.IsHandleCreated) {
                Rectangle r = parent.DisplayRectangle;

                // LayoutEngines send BoundsSpecified.None so they can know they are the ones causing the size change
                // in the subsequent InitLayout.  We need to be careful preserve a None.
                base.SetBoundsCore (r.X, r.Y, r.Width, r.Height, specified == BoundsSpecified.None ? BoundsSpecified.None : BoundsSpecified.All);
            }
            else {
                base.SetBoundsCore (x, y, width, height, specified);
            }
        }

        /// <devdoc>
        ///     Determines if the Location property needs to be persisted.
        /// </devdoc>
        [EditorBrowsable (EditorBrowsableState.Never)]
        private bool ShouldSerializeLocation () {
            return Left != 0 || Top != 0;
        }

        /// <include file='doc\TabPage.uex' path='docs/doc[@for="TabPage.ToString"]/*' />
        /// <devdoc>
        ///     The text property is what is returned for the TabPages default printing.
        /// </devdoc>
        public override string ToString () {
            return "TabPage: {" + Text + "}";
        }

        /// <include file='doc\TabPage.uex' path='docs/doc[@for="TabPage.UpdateParent"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        internal void UpdateParent () {
            TabControl parent = ParentInternal as TabControl;

            if (parent != null) {
                parent.UpdateTab (this);
            }
        }

        /// <include file='doc\TabPage.uex' path='docs/doc[@for="TabPage.TabPageControlCollection"]/*' />
        /// <devdoc>
        ///      Our control collection will throw an exception if you try to add other tab pages.
        /// </devdoc>
        [ComVisible(false)]
        public class TabPageControlCollection : Control.ControlCollection {
            /// <internalonly/>
            /// <include file='doc\TabPage.uex' path='docs/doc[@for="TabPage.TabPageControlCollection.TabPageControlCollection"]/*' />
            /// <devdoc>
            ///      Creates a new TabPageControlCollection.
            /// </devdoc>
            public TabPageControlCollection (TabPage owner) : base(owner) {
            }

            /// <include file='doc\TabPage.uex' path='docs/doc[@for="TabPage.TabPageControlCollection.Add"]/*' />
            /// <devdoc>
            ///     Adds a child control to this control. The control becomes the last control
            ///     in the child control list. If the control is already a child of another
            ///     control it is first removed from that control.  The tab page overrides
            ///     this method to ensure that child tab pages are not added to it, as these
            ///     are illegal.
            /// </devdoc>
            public override void Add (Control value) {
                if (value is TabPage) {
                    throw new ArgumentException (string.Format (SR.TABCONTROLTabPageOnTabPage));
                }

                base.Add (value);
            }
        }
    }
}

