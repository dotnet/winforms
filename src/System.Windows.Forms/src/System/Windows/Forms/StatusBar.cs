// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms
{
    using System.Runtime.Serialization.Formatters;
    using System.Runtime.InteropServices;
    using System.ComponentModel;
    using System.Diagnostics;
    using System;
    using System.Drawing;
    using System.Collections;
    using Microsoft.Win32;
    using System.Windows.Forms.VisualStyles;
    using System.Globalization;


    /// <summary>
    ///    <para>
    ///       Represents a Windows status bar control.
    ///    </para>
    /// </summary>
    [
    ComVisible(true),
    ClassInterface(ClassInterfaceType.AutoDispatch),
    DefaultEvent(nameof(PanelClick)),
    DefaultProperty(nameof(Text)),
    Designer("System.Windows.Forms.Design.StatusBarDesigner, " + AssemblyRef.SystemDesign),
    ]
    public class StatusBar : Control
    {

        private int sizeGripWidth = 0;
        private const int SIMPLE_INDEX = 0xFF;

        private static readonly object EVENT_PANELCLICK = new object();
        private static readonly object EVENT_SBDRAWITEM = new object();

        private bool showPanels;
        private bool layoutDirty;
        private int panelsRealized;
        private bool sizeGrip = true;
        private string simpleText;
        private Point lastClick = new Point(0, 0);
        private readonly IList panels = new ArrayList();
        private StatusBarPanelCollection panelsCollection;
        private ControlToolTip tooltips;

        private ToolTip mainToolTip = null;
        private bool toolTipSet = false;

        /// <summary>
        ///    <para>
        ///       Initializes a new default instance of the <see cref='System.Windows.Forms.StatusBar'/> class.
        ///    </para>
        /// </summary>
        public StatusBar()
        : base()
        {
            base.SetStyle(ControlStyles.UserPaint | ControlStyles.Selectable, false);

            Dock = DockStyle.Bottom;
            TabStop = false;
        }

        private static VisualStyleRenderer renderer = null;

        /// <summary>
        ///     A VisualStyleRenderer we can use to get information about the current UI theme
        /// </summary>
        private static VisualStyleRenderer VisualStyleRenderer
        {
            get
            {
                if (VisualStyleRenderer.IsSupported)
                {
                    if (renderer == null)
                    {
                        renderer = new VisualStyleRenderer(VisualStyleElement.ToolBar.Button.Normal);
                    }
                }
                else
                {
                    renderer = null;
                }
                return renderer;
            }

        }

        private int SizeGripWidth
        {
            get
            {
                if (sizeGripWidth == 0)
                {
                    if (Application.RenderWithVisualStyles && VisualStyleRenderer != null)
                    {
                        // Need to build up accurate gripper width to avoid cutting off other panes.
                        VisualStyleRenderer vsRenderer = VisualStyleRenderer;
                        VisualStyleElement thisElement;
                        Size elementSize;

                        // gripper pane width...
                        thisElement = VisualStyleElement.Status.GripperPane.Normal;
                        vsRenderer.SetParameters(thisElement);
                        elementSize = vsRenderer.GetPartSize(Graphics.FromHwndInternal(Handle), ThemeSizeType.True);
                        sizeGripWidth = elementSize.Width;

                        // ...plus gripper width
                        thisElement = VisualStyleElement.Status.Gripper.Normal;
                        vsRenderer.SetParameters(thisElement);
                        elementSize = vsRenderer.GetPartSize(Graphics.FromHwndInternal(Handle), ThemeSizeType.True);
                        sizeGripWidth += elementSize.Width;

                        // Either GetPartSize could have returned a width of zero, so make sure we have a reasonable number:
                        sizeGripWidth = Math.Max(sizeGripWidth, 16);
                    }
                    else
                    {
                        sizeGripWidth = 16;
                    }
                }
                return sizeGripWidth;
            }
        }

        /// <summary>
        ///    <para>
        ///       The background color of this control. This is an ambient property and will
        ///       always return a non-null value.
        ///    </para>
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override Color BackColor
        {
            get
            {
                // not supported, always return CONTROL
                return SystemColors.Control;
            }

            set
            {
                // no op, not supported.
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler BackColorChanged
        {
            add => base.BackColorChanged += value;
            remove => base.BackColorChanged -= value;
        }

        /// <summary>
        ///    <para>
        ///       Gets or sets the image rendered on the background of the
        ///    <see cref='System.Windows.Forms.StatusBar'/>
        ///    control.
        /// </para>
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override Image BackgroundImage
        {
            get
            {
                return base.BackgroundImage;
            }
            set
            {
                base.BackgroundImage = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler BackgroundImageChanged
        {
            add => base.BackgroundImageChanged += value;
            remove => base.BackgroundImageChanged -= value;
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override ImageLayout BackgroundImageLayout
        {
            get
            {
                return base.BackgroundImageLayout;
            }
            set
            {
                base.BackgroundImageLayout = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler BackgroundImageLayoutChanged
        {
            add => base.BackgroundImageLayoutChanged += value;
            remove => base.BackgroundImageLayoutChanged -= value;
        }

        /// <summary>
        ///    <para>
        ///       Returns the CreateParams used to create the handle for this control.
        ///       Inheriting classes should call base.getCreateParams in the manor below:
        ///    </para>
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ClassName = NativeMethods.WC_STATUSBAR;

                if (sizeGrip)
                {
                    cp.Style |= NativeMethods.SBARS_SIZEGRIP;
                }
                else
                {
                    cp.Style &= (~NativeMethods.SBARS_SIZEGRIP);
                }
                cp.Style |= NativeMethods.CCS_NOPARENTALIGN | NativeMethods.CCS_NORESIZE;

                return cp;
            }
        }

        protected override ImeMode DefaultImeMode
        {
            get
            {
                return ImeMode.Disable;
            }
        }

        /// <summary>
        ///     Deriving classes can override this to configure a default size for their control.
        ///     This is more efficient than setting the size in the control's constructor.
        /// </summary>
        protected override Size DefaultSize
        {
            get
            {
                return new Size(100, 22);
            }
        }

        /// <summary>
        ///     This property is overridden and hidden from statement completion
        ///     on controls that are based on Win32 Native Controls.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override bool DoubleBuffered
        {
            get
            {
                return base.DoubleBuffered;
            }
            set
            {
                base.DoubleBuffered = value;
            }
        }

        /// <summary>
        ///    <para>
        ///       Gets or sets the docking behavior of the <see cref='System.Windows.Forms.StatusBar'/> control.
        ///    </para>
        /// </summary>
        [
        Localizable(true),
        DefaultValue(DockStyle.Bottom)
        ]
        public override DockStyle Dock
        {
            get
            {
                return base.Dock;
            }
            set
            {
                base.Dock = value;
            }
        }

        /// <summary>
        ///    <para>
        ///       Gets or sets the font the <see cref='System.Windows.Forms.StatusBar'/>
        ///       control will use to display
        ///       information.
        ///    </para>
        /// </summary>
        [
        Localizable(true)
        ]
        public override Font Font
        {
            get { return base.Font; }
            set
            {
                base.Font = value;
                SetPanelContentsWidths(false);
            }
        }

        /// <summary>
        ///    <para>
        ///       Gets or sets
        ///       the forecolor for the control.
        ///    </para>
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override Color ForeColor
        {
            get
            {
                return base.ForeColor;
            }
            set
            {
                base.ForeColor = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler ForeColorChanged
        {
            add => base.ForeColorChanged += value;
            remove => base.ForeColorChanged -= value;
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public ImeMode ImeMode
        {
            get
            {
                return base.ImeMode;
            }
            set
            {
                base.ImeMode = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler ImeModeChanged
        {
            add => base.ImeModeChanged += value;
            remove => base.ImeModeChanged -= value;
        }

        /// <summary>
        ///    <para>
        ///       Gets the collection of <see cref='System.Windows.Forms.StatusBar'/>
        ///       panels contained within the
        ///       control.
        ///    </para>
        /// </summary>
        [
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        SRDescription(nameof(SR.StatusBarPanelsDescr)),
        Localizable(true),
        SRCategory(nameof(SR.CatAppearance)),
        MergableProperty(false)
        ]
        public StatusBarPanelCollection Panels
        {
            get
            {
                if (panelsCollection == null)
                {
                    panelsCollection = new StatusBarPanelCollection(this);
                }

                return panelsCollection;
            }
        }

        /// <summary>
        ///    <para>
        ///       The status bar text.
        ///    </para>
        /// </summary>
        [
        Localizable(true)
        ]
        public override string Text
        {
            get
            {
                if (simpleText == null)
                {
                    return "";
                }
                else
                {
                    return simpleText;
                }
            }
            set
            {
                SetSimpleText(value);
                if (simpleText != value)
                {
                    simpleText = value;
                    OnTextChanged(EventArgs.Empty);
                }
            }
        }

        /* No one is calling this, so it is ok to comment it out
        private IntPtr ToolTipHandle {
            get {
                EnumChildren c = new EnumChildren( this );
                UnsafeNativeMethods.EnumChildWindows(new HandleRef(null, UnsafeNativeMethods.GetDesktopWindow()), new NativeMethods.EnumChildrenProc(c.Callback), NativeMethods.NullHandleRef);
                return c.hWndFound;
            }
        }
        */

        /// <summary>
        ///    <para>
        ///       Gets or sets a value indicating whether panels should be shown.
        ///    </para>
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(false),
        SRDescription(nameof(SR.StatusBarShowPanelsDescr))
        ]
        public bool ShowPanels
        {
            get
            {
                return showPanels;
            }
            set
            {
                if (showPanels != value)
                {
                    showPanels = value;

                    layoutDirty = true;
                    if (IsHandleCreated)
                    {
                        int bShowPanels = (!showPanels) ? 1 : 0;

                        SendMessage(NativeMethods.SB_SIMPLE, bShowPanels, 0);

                        if (showPanels)
                        {
                            PerformLayout();
                            RealizePanels();
                        }
                        else if (tooltips != null)
                        {
                            for (int i = 0; i < panels.Count; i++)
                            {
                                tooltips.SetTool(panels[i], null);
                            }
                        }

                        SetSimpleText(simpleText);
                    }
                }
            }
        }

        /// <summary>
        ///    <para>
        ///       Gets or sets a value indicating whether a sizing grip
        ///       will be rendered on the corner of the <see cref='System.Windows.Forms.StatusBar'/>
        ///       control.
        ///    </para>
        /// </summary>
        [
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(true),
        SRDescription(nameof(SR.StatusBarSizingGripDescr))
        ]
        public bool SizingGrip
        {
            get
            {
                return sizeGrip;
            }
            set
            {
                if (value != sizeGrip)
                {
                    sizeGrip = value;
                    RecreateHandle();
                }
            }
        }

        /// <summary>
        ///    <para>
        ///       Gets or sets a value indicating whether the user will be able to tab to the
        ///    <see cref='System.Windows.Forms.StatusBar'/> .
        ///    </para>
        /// </summary>
        [DefaultValue(false)]
        new public bool TabStop
        {
            get
            {
                return base.TabStop;
            }
            set
            {
                base.TabStop = value;
            }
        }

        internal bool ToolTipSet
        {
            get
            {
                return toolTipSet;
            }
        }

        internal ToolTip MainToolTip
        {
            get
            {
                return mainToolTip;
            }
        }

        /// <summary>
        ///    <para>
        ///       Occurs when a visual aspect of an owner-drawn status bar changes.
        ///    </para>
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.StatusBarDrawItem))]
        public event StatusBarDrawItemEventHandler DrawItem
        {
            add => Events.AddHandler(EVENT_SBDRAWITEM, value);
            remove => Events.RemoveHandler(EVENT_SBDRAWITEM, value);
        }

        /// <summary>
        ///    <para>
        ///       Occurs when a panel on the status bar is clicked.
        ///    </para>
        /// </summary>
        [SRCategory(nameof(SR.CatMouse)), SRDescription(nameof(SR.StatusBarOnPanelClickDescr))]
        public event StatusBarPanelClickEventHandler PanelClick
        {
            add => Events.AddHandler(EVENT_PANELCLICK, value);
            remove => Events.RemoveHandler(EVENT_PANELCLICK, value);
        }

        /// <summary>
        ///     StatusBar Onpaint.
        /// </summary>
        /// <hideinheritance/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event PaintEventHandler Paint
        {
            add => base.Paint += value;
            remove => base.Paint -= value;
        }

        /// <summary>
        ///     Tells whether the panels have been realized.
        /// </summary>
        internal bool ArePanelsRealized()
        {
            return showPanels && IsHandleCreated;
        }

        /// <summary>
        /// </summary>
        internal void DirtyLayout()
        {
            layoutDirty = true;
        }

        /// <summary>
        ///     Makes the panel according to the sizes in the panel list.
        /// </summary>
        private void ApplyPanelWidths()
        {
            // This forces handle creation every time any time the StatusBar
            // has to be re-laidout.
            //
            if (!IsHandleCreated)
            {
                return;
            }

            StatusBarPanel panel = null;
            int length = panels.Count;

            if (length == 0)
            {
                Size sz = Size;
                int[] offsets = new int[1];
                offsets[0] = sz.Width;
                if (sizeGrip)
                {
                    offsets[0] -= SizeGripWidth;
                }
                UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.SB_SETPARTS, 1, offsets);
                SendMessage(NativeMethods.SB_SETICON, 0, IntPtr.Zero);

                return;
            }

            int[] offsets2 = new int[length];
            int currentOffset = 0;
            for (int i = 0; i < length; i++)
            {
                panel = (StatusBarPanel)panels[i];
                currentOffset += panel.Width;
                offsets2[i] = currentOffset;
                panel.Right = offsets2[i];
            }
            UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.SB_SETPARTS, length, offsets2);

            // Tooltip setup...
            //
            for (int i = 0; i < length; i++)
            {
                panel = (StatusBarPanel)panels[i];
                UpdateTooltip(panel);
            }

            layoutDirty = false;
        }

        /// <summary>
        /// </summary>
        protected override void CreateHandle()
        {
            if (!RecreatingHandle)
            {
                IntPtr userCookie = UnsafeNativeMethods.ThemingScope.Activate();

                try
                {
                    NativeMethods.INITCOMMONCONTROLSEX icc = new NativeMethods.INITCOMMONCONTROLSEX
                    {
                        dwICC = NativeMethods.ICC_BAR_CLASSES
                    };
                    SafeNativeMethods.InitCommonControlsEx(icc);
                }
                finally
                {
                    UnsafeNativeMethods.ThemingScope.Deactivate(userCookie);
                }
            }

            base.CreateHandle();
        }

        /// <summary>
        ///     Disposes this control
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (panelsCollection != null)
                {
                    StatusBarPanel[] panelCopy = new StatusBarPanel[panelsCollection.Count];
                    ((ICollection)panelsCollection).CopyTo(panelCopy, 0);
                    panelsCollection.Clear();

                    foreach (StatusBarPanel p in panelCopy)
                    {
                        p.Dispose();
                    }
                }
            }
            base.Dispose(disposing);
        }

        /// <summary>
        ///     Forces the panels to be updated, location, repainting, etc.
        /// </summary>
        private void ForcePanelUpdate()
        {
            if (ArePanelsRealized())
            {
                layoutDirty = true;
                SetPanelContentsWidths(true);
                PerformLayout();
                RealizePanels();
            }
        }

        /// <summary>
        ///    <para>
        ///       Raises the <see cref='System.Windows.Forms.Control.CreateHandle'/>
        ///       event.
        ///    </para>
        /// </summary>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (!DesignMode)
            {
                tooltips = new ControlToolTip(this);
            }

            if (!showPanels)
            {
                SendMessage(NativeMethods.SB_SIMPLE, 1, 0);
                SetSimpleText(simpleText);
            }
            else
            {
                ForcePanelUpdate();
            }
        }

        /// <summary>
        ///    <para>
        ///       Raises the <see cref='System.Windows.Forms.StatusBar.OnHandleDestroyed'/> event.
        ///    </para>
        /// </summary>
        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);
            if (tooltips != null)
            {
                tooltips.Dispose();
                tooltips = null;
            }
        }

        /* Not used
        /// <summary>
        /// </summary>
        private sealed class EnumChildren {

            public IntPtr hWndFound = IntPtr.Zero;

            private StatusBar peer;

            public EnumChildren( StatusBar peer ) {
                if (peer == null)
                    throw new ArgumentNullException(nameof(peer));
                this.peer = peer;
            }

            public bool Callback(IntPtr hWnd, IntPtr lparam) {
                if (UnsafeNativeMethods.GetParent(new HandleRef(null, hWnd)) == peer.Handle) {
                    hWndFound = hWnd;
                    return false;
                }

                return true;
            }
        }*/

        /// <summary>
        ///    <para>
        ///       Raises the <see cref='System.Windows.Forms.StatusBar.OnMouseDown'/> event.
        ///    </para>
        /// </summary>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            lastClick.X = e.X;
            lastClick.Y = e.Y;
            base.OnMouseDown(e);
        }

        /// <summary>
        ///    <para>
        ///       Raises the <see cref='System.Windows.Forms.StatusBar.OnPanelClick'/> event.
        ///    </para>
        /// </summary>
        protected virtual void OnPanelClick(StatusBarPanelClickEventArgs e)
        {
            ((StatusBarPanelClickEventHandler)Events[EVENT_PANELCLICK])?.Invoke(this, e);
        }

        /// <summary>
        ///    <para>
        ///       Raises the Layout event.
        ///    </para>
        /// </summary>
        protected override void OnLayout(LayoutEventArgs levent)
        {
            if (showPanels)
            {
                LayoutPanels();
                if (IsHandleCreated && panelsRealized != panels.Count)
                {
                    RealizePanels();
                }
            }
            base.OnLayout(levent);
        }

        /// <summary>
        ///     This function sets up all the panel on the status bar according to
        ///     the internal this.panels List.
        /// </summary>
        internal void RealizePanels()
        {
            StatusBarPanel panel = null;
            int length = panels.Count;
            int old = panelsRealized;

            panelsRealized = 0;

            if (length == 0)
            {
                SendMessage(NativeMethods.SB_SETTEXT, 0, "");
            }

            int i;
            for (i = 0; i < length; i++)
            {
                panel = (StatusBarPanel)panels[i];
                try
                {
                    panel.Realize();
                    panelsRealized++;
                }
                catch
                {
                }
            }
            for (; i < old; i++)
            {
                SendMessage(NativeMethods.SB_SETTEXT, 0, null);
            }
        }

        /// <summary>
        ///     Remove the internal list of panels without updating the control.
        /// </summary>
        internal void RemoveAllPanelsWithoutUpdate()
        {
            int size = panels.Count;
            // remove the parent reference
            for (int i = 0; i < size; i++)
            {
                StatusBarPanel sbp = (StatusBarPanel)panels[i];
                sbp.ParentInternal = null;
            }

            panels.Clear();
            if (showPanels == true)
            {
                ApplyPanelWidths();
                ForcePanelUpdate();
            }

        }

        /// <summary>
        ///     Sets the widths of any panels that have the
        ///     StatusBarPanelAutoSize.CONTENTS property set.
        /// </summary>
        internal void SetPanelContentsWidths(bool newPanels)
        {
            int size = panels.Count;
            bool changed = false;
            for (int i = 0; i < size; i++)
            {
                StatusBarPanel sbp = (StatusBarPanel)panels[i];
                if (sbp.AutoSize == StatusBarPanelAutoSize.Contents)
                {
                    int newWidth = sbp.GetContentsWidth(newPanels);
                    if (sbp.Width != newWidth)
                    {
                        sbp.Width = newWidth;
                        changed = true;
                    }
                }
            }
            if (changed)
            {
                DirtyLayout();
                PerformLayout();
            }
        }

        private void SetSimpleText(string simpleText)
        {
            if (!showPanels && IsHandleCreated)
            {

                int wparam = SIMPLE_INDEX + NativeMethods.SBT_NOBORDERS;
                if (RightToLeft == RightToLeft.Yes)
                {
                    wparam |= NativeMethods.SBT_RTLREADING;
                }

                SendMessage(NativeMethods.SB_SETTEXT, wparam, simpleText);
            }
        }

        /// <summary>
        ///     Sizes the the panels appropriately.  It looks at the SPRING AutoSize
        ///     property.
        /// </summary>
        private void LayoutPanels()
        {
            StatusBarPanel panel = null;
            int barPanelWidth = 0;
            int springNum = 0;
            StatusBarPanel[] pArray = new StatusBarPanel[panels.Count];
            bool changed = false;

            for (int i = 0; i < pArray.Length; i++)
            {
                panel = (StatusBarPanel)panels[i];
                if (panel.AutoSize == StatusBarPanelAutoSize.Spring)
                {
                    pArray[springNum] = panel;
                    springNum++;
                }
                else
                {
                    barPanelWidth += panel.Width;
                }
            }


            if (springNum > 0)
            {
                Rectangle rect = Bounds;
                int springPanelsLeft = springNum;
                int leftoverWidth = rect.Width - barPanelWidth;
                if (sizeGrip)
                {
                    leftoverWidth -= SizeGripWidth;
                }
                int copyOfLeftoverWidth = unchecked((int)0x80000000);
                while (springPanelsLeft > 0)
                {

                    int widthOfSpringPanel = (leftoverWidth) / springPanelsLeft;
                    if (leftoverWidth == copyOfLeftoverWidth)
                    {
                        break;
                    }

                    copyOfLeftoverWidth = leftoverWidth;

                    for (int i = 0; i < springNum; i++)
                    {
                        panel = pArray[i];
                        if (panel == null)
                        {
                            continue;
                        }

                        if (widthOfSpringPanel < panel.MinWidth)
                        {
                            if (panel.Width != panel.MinWidth)
                            {
                                changed = true;
                            }
                            panel.Width = panel.MinWidth;
                            pArray[i] = null;
                            springPanelsLeft--;
                            leftoverWidth -= panel.MinWidth;
                        }
                        else
                        {
                            if (panel.Width != widthOfSpringPanel)
                            {
                                changed = true;
                            }
                            panel.Width = widthOfSpringPanel;
                        }
                    }
                }
            }

            if (changed || layoutDirty)
            {
                ApplyPanelWidths();
            }
        }

        /// <summary>
        ///    <para>
        ///       Raises the <see cref='System.Windows.Forms.StatusBar.OnDrawItem'/>
        ///       event.
        ///    </para>
        /// </summary>
        protected virtual void OnDrawItem(StatusBarDrawItemEventArgs sbdievent)
        {
            ((StatusBarDrawItemEventHandler)Events[EVENT_SBDRAWITEM])?.Invoke(this, sbdievent);
        }

        /// <summary>
        ///    <para>
        ///       Raises the <see cref='System.Windows.Forms.StatusBar.OnResize'/>
        ///       event.
        ///    </para>
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            Invalidate();
            base.OnResize(e);
        }

        /// <summary>
        ///    <para>
        ///       Returns a string representation for this control.
        ///    </para>
        /// </summary>
        public override string ToString()
        {

            string s = base.ToString();
            if (Panels != null)
            {
                s += ", Panels.Count: " + Panels.Count.ToString(CultureInfo.CurrentCulture);
                if (Panels.Count > 0)
                {
                    s += ", Panels[0]: " + Panels[0].ToString();
                }
            }
            return s;
        }

        //call this when System.Windows.forms.toolTip is Associated with Statusbar....
        internal void SetToolTip(ToolTip t)
        {
            mainToolTip = t;
            toolTipSet = true;

        }

        internal void UpdateTooltip(StatusBarPanel panel)
        {
            if (tooltips == null)
            {
                if (IsHandleCreated && !DesignMode)
                {
                    //This shouldn't happen: tooltips should've already been set.  The best we can 
                    //do here is reset it.
                    tooltips = new ControlToolTip(this);
                }
                else
                {
                    return;
                }
            }

            if (panel.Parent == this && panel.ToolTipText.Length > 0)
            {
                int border = SystemInformation.Border3DSize.Width;
                ControlToolTip.Tool t = tooltips.GetTool(panel);
                if (t == null)
                {
                    t = new ControlToolTip.Tool();
                }
                t.text = panel.ToolTipText;
                t.rect = new Rectangle(panel.Right - panel.Width + border, 0, panel.Width - border, Height);
                tooltips.SetTool(panel, t);
            }
            else
            {
                tooltips.SetTool(panel, null);
            }
        }

        private void UpdatePanelIndex()
        {
            int length = panels.Count;
            for (int i = 0; i < length; i++)
            {
                ((StatusBarPanel)panels[i]).Index = i;
            }
        }

        /// <summary>
        ///     Processes messages for ownerdraw panels.
        /// </summary>
        private void WmDrawItem(ref Message m)
        {
            NativeMethods.DRAWITEMSTRUCT dis = (NativeMethods.DRAWITEMSTRUCT)m.GetLParam(typeof(NativeMethods.DRAWITEMSTRUCT));

            int length = panels.Count;
            if (dis.itemID < 0 || dis.itemID >= length)
            {
                Debug.Fail("OwnerDraw item out of range");
            }

            StatusBarPanel panel = (StatusBarPanel)
                                   panels[dis.itemID];

            Graphics g = Graphics.FromHdcInternal(dis.hDC);
            Rectangle r = Rectangle.FromLTRB(dis.rcItem.left, dis.rcItem.top, dis.rcItem.right, dis.rcItem.bottom);

            //The itemstate is not defined for a statusbar control
            OnDrawItem(new StatusBarDrawItemEventArgs(g, Font, r, dis.itemID, DrawItemState.None, panel, ForeColor, BackColor));
            g.Dispose();
        }

        /// <summary>
        /// </summary>
        private void WmNotifyNMClick(NativeMethods.NMHDR note)
        {

            if (!showPanels)
            {
                return;
            }

            int size = panels.Count;
            int currentOffset = 0;
            int index = -1;
            for (int i = 0; i < size; i++)
            {
                StatusBarPanel panel = (StatusBarPanel)panels[i];
                currentOffset += panel.Width;
                if (lastClick.X < currentOffset)
                {
                    // this is where the mouse was clicked.
                    index = i;
                    break;
                }
            }
            if (index != -1)
            {
                MouseButtons button = MouseButtons.Left;
                int clicks = 0;
                switch (note.code)
                {
                    case NativeMethods.NM_CLICK:
                        button = MouseButtons.Left;
                        clicks = 1;
                        break;
                    case NativeMethods.NM_RCLICK:
                        button = MouseButtons.Right;
                        clicks = 1;
                        break;
                    case NativeMethods.NM_DBLCLK:
                        button = MouseButtons.Left;
                        clicks = 2;
                        break;
                    case NativeMethods.NM_RDBLCLK:
                        button = MouseButtons.Right;
                        clicks = 2;
                        break;
                }

                Point pt = lastClick;
                StatusBarPanel panel = (StatusBarPanel)panels[index];

                StatusBarPanelClickEventArgs sbpce = new StatusBarPanelClickEventArgs(panel,
                                                                                      button, clicks, pt.X, pt.Y);
                OnPanelClick(sbpce);
            }
        }

        private void WmNCHitTest(ref Message m)
        {
            int x = NativeMethods.Util.LOWORD(m.LParam);
            Rectangle bounds = Bounds;
            bool callSuper = true;

            // The default implementation of the statusbar
            //       : will let you size the form when it is docked on the bottom,
            //       : but when it is anywhere else, the statusbar will be resized.
            //       : to prevent that we provide a little bit a sanity to only
            //       : allow resizing, when it would resize the form.
            //
            if (x > bounds.X + bounds.Width - SizeGripWidth)
            {
                Control parent = ParentInternal;
                if (parent != null && parent is Form)
                {
                    FormBorderStyle bs = ((Form)parent).FormBorderStyle;

                    if (bs != FormBorderStyle.Sizable
                        && bs != FormBorderStyle.SizableToolWindow)
                    {
                        callSuper = false;
                    }

                    if (!((Form)parent).TopLevel
                        || Dock != DockStyle.Bottom)
                    {

                        callSuper = false;
                    }

                    if (callSuper)
                    {
                        Control.ControlCollection children = parent.Controls;
                        int c = children.Count;
                        for (int i = 0; i < c; i++)
                        {
                            Control ctl = children[i];
                            if (ctl != this && ctl.Dock == DockStyle.Bottom)
                            {
                                if (ctl.Top > Top)
                                {
                                    callSuper = false;
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    callSuper = false;
                }
            }

            if (callSuper)
            {
                base.WndProc(ref m);
            }
            else
            {
                m.Result = (IntPtr)NativeMethods.HTCLIENT;
            }
        }

        /// <summary>
        ///    <para>
        ///       Base wndProc. All messages are sent to wndProc after getting filtered through
        ///       the preProcessMessage function. Inheriting controls should call base.wndProc
        ///       for any messages that they don't handle.
        ///    </para>
        /// </summary>
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case Interop.WindowMessages.WM_NCHITTEST:
                    WmNCHitTest(ref m);
                    break;
                case Interop.WindowMessages.WM_REFLECT + Interop.WindowMessages.WM_DRAWITEM:
                    WmDrawItem(ref m);
                    break;
                case Interop.WindowMessages.WM_NOTIFY:
                case Interop.WindowMessages.WM_NOTIFY + Interop.WindowMessages.WM_REFLECT:
                    NativeMethods.NMHDR note = (NativeMethods.NMHDR)m.GetLParam(typeof(NativeMethods.NMHDR));
                    switch (note.code)
                    {
                        case NativeMethods.NM_CLICK:
                        case NativeMethods.NM_RCLICK:
                        case NativeMethods.NM_DBLCLK:
                        case NativeMethods.NM_RDBLCLK:
                            WmNotifyNMClick(note);
                            break;
                        default:
                            base.WndProc(ref m);
                            break;
                    }
                    break;

                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        /// <summary>
        ///    <para>
        ///       The collection of StatusBarPanels that the StatusBar manages.
        ///       event.
        ///    </para>
        /// </summary>
        [
        ListBindable(false)
        ]
        public class StatusBarPanelCollection : IList
        {
            private readonly StatusBar owner;
            /// A caching mechanism for key accessor
            /// We use an index here rather than control so that we don't have lifetime
            /// issues by holding on to extra references.
            private int lastAccessedIndex = -1;


            /// <summary>
            ///    <para>
            ///       Constructor for the StatusBarPanelCollection class
            ///    </para>
            /// </summary>
            public StatusBarPanelCollection(StatusBar owner)
            {
                this.owner = owner;
            }

            /// <summary>
            ///       This method will return an individual StatusBarPanel with the appropriate index.
            /// </summary>
            public virtual StatusBarPanel this[int index]
            {
                get
                {
                    return (StatusBarPanel)owner.panels[index];
                }
                set
                {

                    if (value == null)
                    {
                        throw new ArgumentNullException(nameof(StatusBarPanel));
                    }

                    owner.layoutDirty = true;

                    if (value.Parent != null)
                    {
                        throw new ArgumentException(SR.ObjectHasParent, "value");
                    }

                    int length = owner.panels.Count;

                    if (index < 0 || index >= length)
                    {
                        throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
                    }

                    StatusBarPanel oldPanel = (StatusBarPanel)owner.panels[index];
                    oldPanel.ParentInternal = null;
                    value.ParentInternal = owner;
                    if (value.AutoSize == StatusBarPanelAutoSize.Contents)
                    {
                        value.Width = value.GetContentsWidth(true);
                    }
                    owner.panels[index] = value;
                    value.Index = index;

                    if (owner.ArePanelsRealized())
                    {
                        owner.PerformLayout();
                        value.Realize();
                    }
                }
            }

            object IList.this[int index]
            {
                get
                {
                    return this[index];
                }
                set
                {
                    if (value is StatusBarPanel)
                    {
                        this[index] = (StatusBarPanel)value;
                    }
                    else
                    {
                        throw new ArgumentException(SR.StatusBarBadStatusBarPanel, "value");
                    }
                }
            }
            /// <summary>
            ///     <para>Retrieves the child control with the specified key.</para>
            /// </summary>
            public virtual StatusBarPanel this[string key]
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

            /// <summary>
            ///    <para>
            ///       Returns an integer representing the number of StatusBarPanels
            ///       in this collection.
            ///    </para>
            /// </summary>
            [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
            public int Count
            {
                get
                {
                    return owner.panels.Count;
                }
            }

            object ICollection.SyncRoot
            {
                get
                {
                    return this;
                }
            }

            bool ICollection.IsSynchronized
            {
                get
                {
                    return false;
                }
            }

            bool IList.IsFixedSize
            {
                get
                {
                    return false;
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    return false;
                }
            }

            /// <summary>
            ///    <para>
            ///       Adds a StatusBarPanel to the collection.
            ///    </para>
            /// </summary>
            public virtual StatusBarPanel Add(string text)
            {
                StatusBarPanel panel = new StatusBarPanel
                {
                    Text = text
                };
                Add(panel);
                return panel;
            }

            /// <summary>
            ///    <para>
            ///       Adds a StatusBarPanel to the collection.
            ///    </para>
            /// </summary>
            public virtual int Add(StatusBarPanel value)
            {
                int index = owner.panels.Count;
                Insert(index, value);
                return index;
            }

            int IList.Add(object value)
            {
                if (value is StatusBarPanel)
                {
                    return Add((StatusBarPanel)value);
                }
                else
                {
                    throw new ArgumentException(SR.StatusBarBadStatusBarPanel, "value");
                }
            }

            public virtual void AddRange(StatusBarPanel[] panels)
            {
                if (panels == null)
                {
                    throw new ArgumentNullException(nameof(panels));
                }
                foreach (StatusBarPanel panel in panels)
                {
                    Add(panel);
                }
            }

            public bool Contains(StatusBarPanel panel)
            {
                return IndexOf(panel) != -1;
            }

            bool IList.Contains(object panel)
            {
                if (panel is StatusBarPanel)
                {
                    return Contains((StatusBarPanel)panel);
                }
                else
                {
                    return false;
                }
            }

            /// <summary>
            ///     <para>Returns true if the collection contains an item with the specified key, false otherwise.</para>
            /// </summary>
            public virtual bool ContainsKey(string key)
            {
                return IsValidIndex(IndexOfKey(key));
            }


            public int IndexOf(StatusBarPanel panel)
            {
                for (int index = 0; index < Count; ++index)
                {
                    if (this[index] == panel)
                    {
                        return index;
                    }
                }
                return -1;
            }

            int IList.IndexOf(object panel)
            {
                if (panel is StatusBarPanel)
                {
                    return IndexOf((StatusBarPanel)panel);
                }
                else
                {
                    return -1;
                }
            }

            /// <summary>
            ///     <para>The zero-based index of the first occurrence of value within the entire CollectionBase, if found; otherwise, -1.</para>
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
            ///    <para>
            ///       Inserts a StatusBarPanel in the collection.
            ///    </para>
            /// </summary>
            public virtual void Insert(int index, StatusBarPanel value)
            {

                //check for the value not to be null
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                //end check


                owner.layoutDirty = true;
                if (value.Parent != owner && value.Parent != null)
                {
                    throw new ArgumentException(SR.ObjectHasParent, "value");
                }

                int length = owner.panels.Count;

                if (index < 0 || index > length)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
                }

                value.ParentInternal = owner;

                switch (value.AutoSize)
                {
                    case StatusBarPanelAutoSize.None:
                    case StatusBarPanelAutoSize.Spring:
                        break;
                    case StatusBarPanelAutoSize.Contents:
                        value.Width = value.GetContentsWidth(true);
                        break;
                }

                owner.panels.Insert(index, value);
                owner.UpdatePanelIndex();

                owner.ForcePanelUpdate();
            }

            void IList.Insert(int index, object value)
            {
                if (value is StatusBarPanel)
                {
                    Insert(index, (StatusBarPanel)value);
                }
                else
                {
                    throw new ArgumentException(SR.StatusBarBadStatusBarPanel, "value");
                }
            }

            /// <summary>
            ///     <para>Determines if the index is valid for the collection.</para>
            /// </summary>
            private bool IsValidIndex(int index)
            {
                return ((index >= 0) && (index < Count));
            }

            /// <summary>
            ///    <para>
            ///       Removes all the StatusBarPanels in the collection.
            ///    </para>
            /// </summary>
            public virtual void Clear()
            {
                owner.RemoveAllPanelsWithoutUpdate();
                owner.PerformLayout();

            }

            /// <summary>
            ///    <para>
            ///       Removes an individual StatusBarPanel in the collection.
            ///    </para>
            /// </summary>
            public virtual void Remove(StatusBarPanel value)
            {

                //check for the value not to be null
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(StatusBarPanel));
                }
                //end check

                if (value.Parent != owner)
                {
                    return;
                }
                RemoveAt(value.Index);
            }

            void IList.Remove(object value)
            {
                if (value is StatusBarPanel)
                {
                    Remove((StatusBarPanel)value);
                }
            }


            /// <summary>
            ///    <para>
            ///       Removes an individual StatusBarPanel in the collection at the given index.
            ///    </para>
            /// </summary>
            public virtual void RemoveAt(int index)
            {
                int length = Count;
                if (index < 0 || index >= length)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
                }

                // clear any tooltip
                //
                StatusBarPanel panel = (StatusBarPanel)owner.panels[index];

                owner.panels.RemoveAt(index);
                panel.ParentInternal = null;

                // this will cause the panels tooltip to be removed since it's no longer a child
                // of this StatusBar.
                //
                owner.UpdateTooltip(panel);

                // We must reindex the panels after a removal...
                owner.UpdatePanelIndex();
                owner.ForcePanelUpdate();
            }
            /// <summary>
            ///     <para>Removes the child control with the specified key.</para>
            /// </summary>
            public virtual void RemoveByKey(string key)
            {
                int index = IndexOfKey(key);
                if (IsValidIndex(index))
                {
                    RemoveAt(index);
                }
            }

            void ICollection.CopyTo(Array dest, int index)
            {
                owner.panels.CopyTo(dest, index);
            }

            /// <summary>
            ///    <para>
            ///       Returns the Enumerator for this collection.
            ///    </para>
            /// </summary>
            public IEnumerator GetEnumerator()
            {
                if (owner.panels != null)
                {
                    return owner.panels.GetEnumerator();
                }
                else
                {
                    return Array.Empty<StatusBarPanel>().GetEnumerator();
                }
            }
        }
        /// <summary>
        ///     This is a tooltip control that provides tips for a single
        ///     control. Each "tool" region is defined by a rectangle and
        ///     the string that should be displayed. This implementation
        ///     is based on System.Windows.Forms.ToolTip, but this control
        ///     is lighter weight and provides less functionality... however
        ///     this control binds to rectangular regions, instead of
        ///     full controls.
        /// </summary>
        private class ControlToolTip
        {

            public class Tool
            {
                public Rectangle rect = Rectangle.Empty;
                public string text;
                internal IntPtr id = new IntPtr(-1);
            }

            private readonly Hashtable tools = new Hashtable();
            private readonly ToolTipNativeWindow window = null;
            private readonly Control parent = null;
            private int nextId = 0;

            /// <summary>
            ///    Creates a new ControlToolTip.
            /// </summary>
            public ControlToolTip(Control parent)
            {
                window = new ToolTipNativeWindow(this);
                this.parent = parent;
            }

            /// <summary>
            ///    Returns the createParams to create the window.
            /// </summary>
            protected CreateParams CreateParams
            {
                get
                {
                    NativeMethods.INITCOMMONCONTROLSEX icc = new NativeMethods.INITCOMMONCONTROLSEX
                    {
                        dwICC = NativeMethods.ICC_TAB_CLASSES
                    };
                    SafeNativeMethods.InitCommonControlsEx(icc);
                    CreateParams cp = new CreateParams
                    {
                        Parent = IntPtr.Zero,
                        ClassName = NativeMethods.TOOLTIPS_CLASS
                    };
                    cp.Style |= NativeMethods.TTS_ALWAYSTIP;
                    cp.ExStyle = 0;
                    cp.Caption = null;
                    return cp;
                }
            }

            /// <summary>
            /// </summary>
            public IntPtr Handle
            {
                get
                {
                    if (window.Handle == IntPtr.Zero)
                    {
                        CreateHandle();
                    }
                    return window.Handle;
                }
            }

            private bool IsHandleCreated
            {
                get { return window.Handle != IntPtr.Zero; }
            }

            private void AssignId(Tool tool)
            {
                tool.id = (IntPtr)nextId;
                nextId++;
            }

            /// <summary>
            ///    Sets the tool for the specified key. Keep in mind
            ///    that as soon as setTool is called, the handle for
            ///    the ControlToolTip is created, and the handle for
            ///    the parent control is also created. If the parent
            ///    handle is recreated in the future, all tools must
            ///    be re-added. The old tool for the specified key
            ///    will be removed. Passing null in for the
            ///    tool parameter will result in the tool
            ///    region being removed.
            /// </summary>
            public void SetTool(object key, Tool tool)
            {
                bool remove = false;
                bool add = false;
                bool update = false;

                Tool toRemove = null;
                if (tools.ContainsKey(key))
                {
                    toRemove = (Tool)tools[key];
                }

                if (toRemove != null)
                {
                    remove = true;
                }
                if (tool != null)
                {
                    add = true;
                }
                if (tool != null && toRemove != null
                    && tool.id == toRemove.id)
                {
                    update = true;
                }

                if (update)
                {
                    UpdateTool(tool);
                }
                else
                {
                    if (remove)
                    {
                        RemoveTool(toRemove);
                    }
                    if (add)
                    {
                        AddTool(tool);
                    }
                }

                if (tool != null)
                {
                    tools[key] = tool;
                }
                else
                {
                    tools.Remove(key);
                }

            }

            /// <summary>
            ///    Returns the tool associated with the specified key,
            ///    or null if there is no area.
            /// </summary>
            public Tool GetTool(object key)
            {
                return (Tool)tools[key];
            }


            private void AddTool(Tool tool)
            {
                if (tool != null && tool.text != null && tool.text.Length > 0)
                {
                    int ret;
                    StatusBar p = (StatusBar)parent;

                    if (p.ToolTipSet)
                    {
                        ret = (int)UnsafeNativeMethods.SendMessage(new HandleRef(p.MainToolTip, p.MainToolTip.Handle), NativeMethods.TTM_ADDTOOL, 0, GetTOOLINFO(tool));
                    }
                    else
                    {
                        ret = (int)UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.TTM_ADDTOOL, 0, GetTOOLINFO(tool));
                    }
                    if (ret == 0)
                    {
                        throw new InvalidOperationException(SR.StatusBarAddFailed);
                    }
                }
            }
            private void RemoveTool(Tool tool)
            {
                if (tool != null && tool.text != null && tool.text.Length > 0 && (int)tool.id >= 0)
                {
                    UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.TTM_DELTOOL, 0, GetMinTOOLINFO(tool));
                }
            }
            private void UpdateTool(Tool tool)
            {
                if (tool != null && tool.text != null && tool.text.Length > 0 && (int)tool.id >= 0)
                {
                    UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.TTM_SETTOOLINFO, 0, GetTOOLINFO(tool));
                }
            }


            /// <summary>
            ///    Creates the handle for the control.
            /// </summary>
            protected void CreateHandle()
            {
                if (IsHandleCreated)
                {
                    return;
                }

                window.CreateHandle(CreateParams);
                SafeNativeMethods.SetWindowPos(new HandleRef(this, Handle), NativeMethods.HWND_TOPMOST,
                                     0, 0, 0, 0,
                                     NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE |
                                     NativeMethods.SWP_NOACTIVATE);

                // Setting the max width has the added benefit of enabling multiline
                // tool tips!
                //
                UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.TTM_SETMAXTIPWIDTH, 0, SystemInformation.MaxWindowTrackSize.Width);
            }

            /// <summary>
            ///    Destroys the handle for this control.
            /// </summary>
            protected void DestroyHandle()
            {
                if (IsHandleCreated)
                {
                    window.DestroyHandle();
                    tools.Clear();
                }
            }

            /// <summary>
            ///    Disposes of the component.  Call dispose when the component is no longer needed.
            ///    This method removes the component from its container (if the component has a site)
            ///    and triggers the dispose event.
            /// </summary>
            public void Dispose()
            {
                DestroyHandle();
            }

            /// <summary>
            ///     Returns a new instance of the TOOLINFO_T structure with the minimum
            ///     required data to uniquely identify a region. This is used primarily
            ///     for delete operations. NOTE: This cannot force the creation of a handle.
            /// </summary>
            private NativeMethods.TOOLINFO_T GetMinTOOLINFO(Tool tool)
            {
                NativeMethods.TOOLINFO_T ti = new NativeMethods.TOOLINFO_T
                {
                    cbSize = Marshal.SizeOf<NativeMethods.TOOLINFO_T>(),
                    hwnd = parent.Handle
                };
                if ((int)tool.id < 0)
                {
                    AssignId(tool);
                }
                StatusBar p = (StatusBar)parent;
                if (p != null && p.ToolTipSet)
                {
                    ti.uId = parent.Handle;
                }
                else
                {
                    ti.uId = tool.id;
                }
                return ti;
            }

            /// <summary>
            ///     Returns a detailed TOOLINFO_T structure that represents the specified
            ///     region. NOTE: This may force the creation of a handle.
            /// </summary>
            private NativeMethods.TOOLINFO_T GetTOOLINFO(Tool tool)
            {
                NativeMethods.TOOLINFO_T ti = GetMinTOOLINFO(tool);
                ti.cbSize = Marshal.SizeOf<NativeMethods.TOOLINFO_T>();
                ti.uFlags |= NativeMethods.TTF_TRANSPARENT | NativeMethods.TTF_SUBCLASS;

                // RightToLeft reading order
                //
                Control richParent = parent;
                if (richParent != null && richParent.RightToLeft == RightToLeft.Yes)
                {
                    ti.uFlags |= NativeMethods.TTF_RTLREADING;
                }

                ti.lpszText = tool.text;
                ti.rect = NativeMethods.RECT.FromXYWH(tool.rect.X, tool.rect.Y, tool.rect.Width, tool.rect.Height);
                return ti;
            }


            /// <summary>
            /// </summary>
            ~ControlToolTip()
            {
                DestroyHandle();
            }

            /// <summary>
            ///    WNDPROC
            /// </summary>

            protected void WndProc(ref Message msg)
            {
                switch (msg.Msg)
                {
                    case Interop.WindowMessages.WM_SETFOCUS:
                        // 



                        return;
                    default:
                        window.DefWndProc(ref msg);
                        break;
                }
            }

            /// <summary>
            /// </summary>
            private class ToolTipNativeWindow : NativeWindow
            {
                readonly ControlToolTip control;

                internal ToolTipNativeWindow(ControlToolTip control)
                {
                    this.control = control;
                }

                protected override void WndProc(ref Message m)
                {
                    if (control != null)
                    {
                        control.WndProc(ref m);
                    }
                }
            }
        }


    }
}

