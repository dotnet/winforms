// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms
{
    /// <summary>
    ///  TabPage implements a single page of a tab control. It is essentially a Panel with TabItem
    ///  properties.
    /// </summary>
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    [Designer("System.Windows.Forms.Design.TabPageDesigner, " + AssemblyRef.SystemDesign)]
    [ToolboxItem(false)]
    [DesignTimeVisible(false)]
    [DefaultEvent("Click")]
    [DefaultProperty("Text")]
    public class TabPage : Panel
    {
        private ImageList.Indexer imageIndexer;
        private string toolTipText = string.Empty;
        private bool enterFired = false;
        private bool leaveFired = false;
        private bool useVisualStyleBackColor = false;

        /// <summary>
        ///  Constructs an empty TabPage.
        /// </summary>
        public TabPage() : base()
        {
            SetStyle(ControlStyles.CacheText, true);
            Text = null;
        }

        /// <summary>
        ///  Constructs a TabPage with text for the tab.
        /// </summary>
        public TabPage(string text) : this()
        {
            Text = text;
        }

        /// <summary>
        ///  Allows the control to optionally shrink when AutoSize is true.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false),]
        [Localizable(false)]
        public override AutoSizeMode AutoSizeMode
        {
            get => AutoSizeMode.GrowOnly;
            set
            {
            }
        }

        /// <summary>
        ///  Hide AutoSize: it doesn't make sense for this control
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool AutoSize
        {
            get => base.AutoSize;
            set => base.AutoSize = value;
        }

        [SRCategory(nameof(SR.CatPropertyChanged))]
        [SRDescription(nameof(SR.ControlOnAutoSizeChangedDescr))]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler AutoSizeChanged
        {
            add => base.AutoSizeChanged += value;
            remove => base.AutoSizeChanged -= value;
        }

        /// <summary>
        ///  The background color of this control. This is an ambient property and will always return
        ///  a non-null value.
        /// </summary>
        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.ControlBackColorDescr))]
        public override Color BackColor
        {
            get
            {
                Color color = base.BackColor;
                if (color != DefaultBackColor)
                {
                    return color;
                }
                else if (Application.RenderWithVisualStyles && UseVisualStyleBackColor && (ParentInternal is TabControl parent && parent.Appearance == TabAppearance.Normal))
                {
                    return Color.Transparent;
                }

                return color;
            }
            set
            {
                if (DesignMode)
                {
                    if (value != Color.Empty)
                    {
                        PropertyDescriptor pd = TypeDescriptor.GetProperties(this)["UseVisualStyleBackColor"];
                        Debug.Assert(pd != null);
                        if (pd != null)
                        {
                            pd.SetValue(this, false);
                        }
                    }
                }
                else
                {
                    UseVisualStyleBackColor = false;
                }

                base.BackColor = value;
            }
        }

        /// <summary>
        ///  Constructs the new instance of the Controls collection objects.
        /// </summary>
        protected override ControlCollection CreateControlsInstance() => new TabPageControlCollection(this);

        internal ImageList.Indexer ImageIndexer => imageIndexer ??= new ImageList.Indexer();

        /// <summary>
        ///  Returns the imageIndex for the TabPage. This should point to an image
        ///  in the TabControl's associated imageList that will appear on the tab, or be -1.
        /// </summary>
        [TypeConverter(typeof(ImageIndexConverter))]
        [Editor("System.Windows.Forms.Design.ImageIndexEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))]
        [Localizable(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue(-1)]
        [SRDescription(nameof(SR.TabItemImageIndexDescr))]
        public int ImageIndex
        {
            get => ImageIndexer.Index;
            set
            {
                if (value < -1)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidLowBoundArgumentEx, nameof(ImageIndex), value, -1));
                }

                if (ParentInternal is TabControl parent)
                {
                    ImageIndexer.ImageList = parent.ImageList;
                }

                ImageIndexer.Index = value;
                UpdateParent();
            }
        }

        /// <summary>
        ///  Returns the imageIndex for the TabPage. This should point to an image in the TabControl's
        ///  associated imageList that will appear on the tab, or be -1.
        /// </summary>
        [TypeConverter(typeof(ImageKeyConverter))]
        [Editor("System.Windows.Forms.Design.ImageIndexEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))]
        [Localizable(true)]
        [DefaultValue("")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [SRDescription(nameof(SR.TabItemImageIndexDescr))]
        public string ImageKey
        {
            get => ImageIndexer.Key;
            set
            {
                ImageIndexer.Key = value;

                if (ParentInternal is TabControl parent)
                {
                    ImageIndexer.ImageList = parent.ImageList;
                }

                UpdateParent();
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override AnchorStyles Anchor
        {
            get => base.Anchor;
            set => base.Anchor = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override DockStyle Dock
        {
            get => base.Dock;
            set => base.Dock = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler DockChanged
        {
            add => base.DockChanged += value;
            remove => base.DockChanged -= value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new bool Enabled
        {
            get => base.Enabled;
            set => base.Enabled = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler EnabledChanged
        {
            add => base.EnabledChanged += value;
            remove => base.EnabledChanged -= value;
        }

        [DefaultValue(false)]
        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.TabItemUseVisualStyleBackColorDescr))]
        public bool UseVisualStyleBackColor
        {
            get => useVisualStyleBackColor;
            set
            {
                if (useVisualStyleBackColor == value)
                {
                    return;
                }

                useVisualStyleBackColor = value;
                Invalidate(true);
            }
        }

        /// <summary>
        ///  Make the Location property non-browsable for the tab pages.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new Point Location
        {
            get => base.Location;
            set => base.Location = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler LocationChanged
        {
            add => base.LocationChanged += value;
            remove => base.LocationChanged -= value;
        }

        [DefaultValue(typeof(Size), "0, 0")]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override Size MaximumSize
        {
            get => base.MaximumSize;

            set => base.MaximumSize = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override Size MinimumSize
        {
            get => base.MinimumSize;

            set => base.MinimumSize = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new Size PreferredSize => base.PreferredSize;

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new int TabIndex
        {
            get => base.TabIndex;
            set => base.TabIndex = value;
        }

        /// <summary>
        ///  This property is required by certain controls (TabPage) to render its transparency using
        ///  theming API. We dont want all controls (that are have transparent BackColor) to use
        ///  theming API to render its background because it has large performance cost.
        /// </summary>
        internal override bool RenderTransparencyWithVisualStyles => true;

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler TabIndexChanged
        {
            add => base.TabIndexChanged += value;
            remove => base.TabIndexChanged -= value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new bool TabStop
        {
            get => base.TabStop;
            set => base.TabStop = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler TabStopChanged
        {
            add => base.TabStopChanged += value;
            remove => base.TabStopChanged -= value;
        }

        [Localizable(true)]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public override string Text
        {
            get => base.Text;
            set
            {
                base.Text = value;
                UpdateParent();
            }
        }

        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public new event EventHandler TextChanged
        {
            add => base.TextChanged += value;
            remove => base.TextChanged -= value;
        }

        /// <summary>
        ///  The toolTipText for the tab, that will appear when the mouse hovers over the tab and the
        ///  TabControl's showToolTips property is true.
        /// </summary>
        [DefaultValue("")]
        [Localizable(true)]
        [SRDescription(nameof(SR.TabItemToolTipTextDescr))]
        public string ToolTipText
        {
            get => toolTipText;
            set
            {
                if (value == null)
                {
                    value = string.Empty;
                }

                if (value == toolTipText)
                {
                    return;
                }

                toolTipText = value;
                UpdateParent();
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new bool Visible
        {
            get => base.Visible;
            set => base.Visible = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler VisibleChanged
        {
            add => base.VisibleChanged += value;
            remove => base.VisibleChanged -= value;
        }

        /// <summary>
        ///  Assigns a new parent control. Sends out the appropriate property change notifications for
        ///  properties that are affected by the change of parent.
        /// </summary>
        internal override void AssignParent(Control value)
        {
            if (value != null && !(value is TabControl))
            {
                throw new ArgumentException(string.Format(SR.TabControlTabPageNotOnTabControl, value.GetType().FullName));
            }

            base.AssignParent(value);
        }

        /// <summary>
        ///  Given a component, this retrieves the tab page that it's parented to, or null if it's not
        ///  parented to any tab page.
        /// </summary>
        public static TabPage GetTabPageOfComponent(object comp)
        {
            if (!(comp is Control c))
            {
                return null;
            }

            while (c != null && !(c is TabPage))
            {
                c = c.ParentInternal;
            }

            return (TabPage)c;
        }

        internal NativeMethods.TCITEM_T GetTCITEM()
        {
            NativeMethods.TCITEM_T tcitem = new NativeMethods.TCITEM_T
            {
                mask = 0,
                pszText = null,
                cchTextMax = 0,
                lParam = IntPtr.Zero
            };

            string text = Text;

            PrefixAmpersands(ref text);
            if (text != null)
            {
                tcitem.mask |= NativeMethods.TCIF_TEXT;
                tcitem.pszText = text;
                tcitem.cchTextMax = text.Length;
            }

            int imageIndex = ImageIndex;

            tcitem.mask |= NativeMethods.TCIF_IMAGE;
            tcitem.iImage = ImageIndexer.ActualIndex;
            return tcitem;
        }

        private void PrefixAmpersands(ref string value)
        {
            // Due to a comctl32 problem, ampersands underline the next letter in the
            // text string, but the accelerators don't work.
            // So in this function, we prefix ampersands with another ampersand
            // so that they actually appear as ampersands.
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            // If there are no ampersands, we don't need to do anything here
            if (value.IndexOf('&') < 0)
            {
                return;
            }

            // Insert extra ampersands
            var newString = new StringBuilder();
            for (int i = 0; i < value.Length; ++i)
            {
                if (value[i] == '&')
                {
                    if (i < value.Length - 1 && value[i + 1] == '&')
                    {
                        // Skip the second ampersand
                        ++i;
                    }

                    newString.Append("&&");
                }
                else
                {
                    newString.Append(value[i]);
                }
            }

            value = newString.ToString();
        }

        /// <summary>
        ///  This is an internal method called by the TabControl to fire the Leave event when TabControl leave occurs.
        /// </summary>
        internal void FireLeave(EventArgs e)
        {
            leaveFired = true;
            OnLeave(e);
        }

        /// <summary>
        ///  This is an internal method called by the TabControl to fire the Enter event when TabControl leave occurs.
        /// </summary>
        internal void FireEnter(EventArgs e)
        {
            enterFired = true;
            OnEnter(e);
        }

        /// <summary>
        ///  Actually goes and fires the OnEnter event. Inheriting controls should use this to know
        ///  when the event is fired [this is preferable to adding an event handler on yourself for
        ///  this event]. They should, however, remember to call base.OnEnter(e); to ensure the event
        ///  i still fired to external listeners
        ///  This listener is overidden so that we can fire SAME ENTER and LEAVE events on the TabPage.
        ///  TabPage should fire enter when the focus is on the TabPage and not when the control
        ///  within the TabPage gets Focused.
        /// </summary>
        protected override void OnEnter(EventArgs e)
        {
            if (ParentInternal is TabControl parent)
            {
                if (enterFired)
                {
                    base.OnEnter(e);
                }

                enterFired = false;
            }
        }

        /// <summary>
        ///  Actually goes and fires the OnLeave event. Inheriting controls should use this to know
        ///  when the event is fired [this is preferable to adding an event handler on yourself for
        ///  this event]. They should, however, remember to call base.OnLeave(e); to ensure the event
        ///  is still fired to external listeners
        ///  This listener is overidden so that we can fire same enter and leave events on the TabPage.
        ///  TabPage should fire enter when the focus is on the TabPage and not when the control within
        ///  the TabPage gets Focused.
        ///  Similary the Leave should fire when the TabControl (and hence the TabPage) loses focus.
        /// </summary>
        protected override void OnLeave(EventArgs e)
        {
            if (ParentInternal is TabControl parent)
            {
                if (leaveFired)
                {
                    base.OnLeave(e);
                }

                leaveFired = false;
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Utilize the TabRenderer new to Whidbey to draw the tab pages so that the panels are
            // drawn using the correct visual styles when the application supports using visual
            // styles.

            // Utilize the UseVisualStyleBackColor property to determine whether or not the themed
            // background should be utilized.
            if (Application.RenderWithVisualStyles && UseVisualStyleBackColor && (ParentInternal is TabControl parent && parent.Appearance == TabAppearance.Normal))
            {
                Color bkcolor = UseVisualStyleBackColor ? Color.Transparent : BackColor;
                Rectangle inflateRect = LayoutUtils.InflateRect(DisplayRectangle, Padding);

                // To ensure that the TabPage draws correctly (the border will get clipped and
                // and gradient fill will match correctly with the tabcontrol). Unfortunately,
                // there is no good way to determine the padding used on the TabPage.
                Rectangle rectWithBorder = new Rectangle(inflateRect.X - 4, inflateRect.Y - 2, inflateRect.Width + 8, inflateRect.Height + 6);

                TabRenderer.DrawTabPage(e.Graphics, rectWithBorder);

                // TabRenderer does not support painting the background image on the panel, so
                // draw it ourselves.
                if (BackgroundImage != null)
                {
                    ControlPaint.DrawBackgroundImage(e.Graphics, BackgroundImage, bkcolor, BackgroundImageLayout, inflateRect, inflateRect, DisplayRectangle.Location);
                }
            }
            else
            {
                base.OnPaintBackground(e);
            }
        }

        /// <summary>
        ///  Overrides main setting of our bounds so that we can control our size and that of our
        ///  TabPages.
        /// </summary>
        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            Control parent = ParentInternal;

            if (parent is TabControl && parent.IsHandleCreated)
            {
                Rectangle r = parent.DisplayRectangle;

                // LayoutEngines send BoundsSpecified.None so they can know they are the ones causing the size change
                // in the subsequent InitLayout. We need to be careful preserve a None.
                base.SetBoundsCore(r.X, r.Y, r.Width, r.Height, specified == BoundsSpecified.None ? BoundsSpecified.None : BoundsSpecified.All);
            }
            else
            {
                base.SetBoundsCore(x, y, width, height, specified);
            }
        }

        /// <summary>
        ///  Determines if the Location property needs to be persisted.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeLocation() => Left != 0 || Top != 0;

        /// <summary>
        ///  The text property is what is returned for the TabPages default printing.
        /// </summary>
        public override string ToString() => $"TabPage: {{{Text}}}";

        internal void UpdateParent()
        {
            if (ParentInternal is TabControl parent)
            {
                parent.UpdateTab(this);
            }
        }

        /// <summary>
        ///  Our control collection will throw an exception if you try to add other tab pages.
        /// </summary>
        [ComVisible(false)]
        public class TabPageControlCollection : ControlCollection
        {
            /// <summary>
            ///  Creates a new TabPageControlCollection.
            /// </summary>
            public TabPageControlCollection(TabPage owner) : base(owner)
            {
            }

            /// <summary>
            ///  Adds a child control to this control. The control becomes the last control
            ///  in the child control list. If the control is already a child of another
            ///  control it is first removed from that control. The tab page overrides
            ///  this method to ensure that child tab pages are not added to it, as these
            ///  are illegal.
            /// </summary>
            public override void Add(Control value)
            {
                if (value is TabPage)
                {
                    throw new ArgumentException(SR.TabControlTabPageOnTabPage);
                }

                base.Add(value);
            }
        }
    }
}
