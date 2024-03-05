// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms;

#pragma warning disable RS0016
#nullable disable
[Obsolete("StatusBar is obsolete. Use the StatusStrip control instead.  StatusBar will be removed in a future release.")]
public class StatusBar : Control
{
    private int sizeGripWidth;
    private static readonly object EVENT_PANELCLICK = new object();
    private static readonly object EVENT_SBDRAWITEM = new object();

    private bool layoutDirty;
    private int panelsRealized;
    private bool sizeGrip = true;
    private Point lastClick = new Point(0, 0);
    private IList panels = new ArrayList();
    private ControlToolTip tooltips;

    private ToolTip mainToolTip;
    private bool toolTipSet;

    public StatusBar()
    : base()
    {
        throw new PlatformNotSupportedException();
    }

    private static VisualStyleRenderer renderer;

    /// <devdoc>
    ///     A VisualStyleRenderer we can use to get information about the current UI theme
    /// </devdoc>
    private static VisualStyleRenderer VisualStyleRenderer
    {
        get
        {
            if (VisualStyleRenderer.IsSupported)
            {
                if (renderer is null)
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
                if (Application.RenderWithVisualStyles && VisualStyleRenderer is not null)
                {
                    // VSWhidbey 207045: need to build up accurate gripper width to avoid cutting off other panes.
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

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public override Color BackColor
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    new public event EventHandler BackColorChanged
    {
        add
        {
            throw new PlatformNotSupportedException();
        }
        remove
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public override Image BackgroundImage
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    new public event EventHandler BackgroundImageChanged
    {
        add
        {
            throw new PlatformNotSupportedException();
        }
        remove
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public override ImageLayout BackgroundImageLayout
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    new public event EventHandler BackgroundImageLayoutChanged
    {
        add
        {
            throw new PlatformNotSupportedException();
        }
        remove
        {
            throw new PlatformNotSupportedException();
        }
    }

    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams cp = base.CreateParams;
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

    protected override Size DefaultSize
    {
        get
        {
            return new Size(100, 22);
        }
    }

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

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public override DockStyle Dock
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public override Font Font
    {
        get { throw new PlatformNotSupportedException(); }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public override Color ForeColor
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    new public event EventHandler ForeColorChanged
    {
        add
        {
            throw new PlatformNotSupportedException();
        }
        remove
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    new public ImeMode ImeMode
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler ImeModeChanged
    {
        add
        {
            throw new PlatformNotSupportedException();
        }
        remove
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public StatusBarPanelCollection Panels
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public override string Text
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public bool ShowPanels
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public bool SizingGrip
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    new public bool TabStop
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
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

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public event StatusBarDrawItemEventHandler DrawItem
    {
        add
        {
            throw new PlatformNotSupportedException();
        }
        remove
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public event StatusBarPanelClickEventHandler PanelClick
    {
        add
        {
            throw new PlatformNotSupportedException();
        }
        remove
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public new event PaintEventHandler Paint
    {
        add
        {
            throw new PlatformNotSupportedException();
        }
        remove
        {
            throw new PlatformNotSupportedException();
        }
    }

    internal bool ArePanelsRealized()
    {
        return IsHandleCreated;
    }

    internal void DirtyLayout()
    {
        layoutDirty = true;
    }

    private void ApplyPanelWidths()
    {
        // This forces handle creation every time any time the StatusBar
        // has to be re-laidout.
        if (!IsHandleCreated)
            return;

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

            return;
        }

        int[] offsets2 = new int[length];
        int currentOffset = 0;
        for (int i = 0; i < length; i++)
        {
            panel = (StatusBarPanel)this.panels[i];
            currentOffset += panel.Width;
            offsets2[i] = currentOffset;
            panel.Right = offsets2[i];
        }

        // Tooltip setup...
        //
        for (int i = 0; i < length; i++)
        {
            panel = (StatusBarPanel)this.panels[i];
            UpdateTooltip(panel);
        }

        layoutDirty = false;
    }

    protected override void CreateHandle()
    {
        base.CreateHandle();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
    }

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

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        if (!DesignMode)
        {
            tooltips = new ControlToolTip(this);
        }

        ForcePanelUpdate();
    }

    protected override void OnHandleDestroyed(EventArgs e)
    {
        base.OnHandleDestroyed(e);
        if (tooltips is not null)
        {
            tooltips.Dispose();
            tooltips = null;
        }
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        lastClick.X = e.X;
        lastClick.Y = e.Y;
        base.OnMouseDown(e);
    }

    protected virtual void OnPanelClick(StatusBarPanelClickEventArgs e)
    {
        StatusBarPanelClickEventHandler handler = (StatusBarPanelClickEventHandler)Events[EVENT_PANELCLICK];
        if (handler is not null)
            handler(this, e);
    }

    protected override void OnLayout(LayoutEventArgs levent)
    {
        base.OnLayout(levent);
    }

    internal void RealizePanels()
    {
        StatusBarPanel panel = null;
        int length = panels.Count;
        int old = panelsRealized;

        panelsRealized = 0;

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
    }

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
    }

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
    }

    private void LayoutPanels()
    {
        StatusBarPanel panel = null;
        int barPanelWidth = 0;
        int springNum = 0;
        StatusBarPanel[] pArray = new StatusBarPanel[panels.Count];
        bool changed = false;

        for (int i = 0; i < pArray.Length; i++)
        {
            panel = (StatusBarPanel)this.panels[i];
            if (panel.AutoSize == StatusBarPanelAutoSize.Spring)
            {
                pArray[springNum] = panel;
                springNum++;
            }
            else
                barPanelWidth += panel.Width;
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
                    break;
                copyOfLeftoverWidth = leftoverWidth;

                for (int i = 0; i < springNum; i++)
                {
                    panel = pArray[i];
                    if (panel is null)
                        continue;

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

    protected virtual void OnDrawItem(StatusBarDrawItemEventArgs sbdievent)
    {
        StatusBarDrawItemEventHandler handler = (StatusBarDrawItemEventHandler)Events[EVENT_SBDRAWITEM];
        if (handler is not null)
            handler(this, sbdievent);
    }

    protected override void OnResize(EventArgs e)
    {
        Invalidate();
        base.OnResize(e);
    }

    public override string ToString()
    {
        string s = base.ToString();
        if (Panels is not null)
        {
            s += ", Panels.Count: " + Panels.Count.ToString(CultureInfo.CurrentCulture);
            if (Panels.Count > 0)
                s += ", Panels[0]: " + Panels[0].ToString();
        }

        return s;
    }

    new internal void SetToolTip(ToolTip t)
    {
        mainToolTip = t;
        toolTipSet = true;
    }

    internal void UpdateTooltip(StatusBarPanel panel)
    {
        if (tooltips is null)
        {
            if (IsHandleCreated && !DesignMode)
            {
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
            if (t is null)
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

    private void WmDrawItem(ref Message m)
    {
        DRAWITEMSTRUCT dis = (DRAWITEMSTRUCT)m.GetLParam(typeof(DRAWITEMSTRUCT));

        int length = panels.Count;
        if (dis.itemID < 0 || dis.itemID >= length)
            Debug.Fail("OwnerDraw item out of range");

        StatusBarPanel panel = (StatusBarPanel)
                               panels[0];

        Graphics g = Graphics.FromHdcInternal(dis.hDC);
        Rectangle r = Rectangle.FromLTRB(dis.rcItem.left, dis.rcItem.top, dis.rcItem.right, dis.rcItem.bottom);

        g.Dispose();
    }

    private void WmNotifyNMClick(NMHDR note)
    {
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
                case PInvoke.NM_CLICK:
                    button = MouseButtons.Left;
                    clicks = 1;
                    break;
                case PInvoke.NM_RCLICK:
                    button = MouseButtons.Right;
                    clicks = 1;
                    break;
                case PInvoke.NM_DBLCLK:
                    button = MouseButtons.Left;
                    clicks = 2;
                    break;
                case PInvoke.NM_RDBLCLK:
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
        int x = PARAM.LOWORD(m.LParam);
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
            if (parent is not null && parent is Form)
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
            m.Result = (IntPtr)PInvoke.HTCLIENT;
        }
    }

    protected override void WndProc(ref Message m)
    {
        switch (m.MsgInternal)
        {
            case PInvoke.WM_NCHITTEST:
                WmNCHitTest(ref m);
                break;
            case MessageId.WM_REFLECT + PInvoke.WM_DRAWITEM:
                WmDrawItem(ref m);
                break;
            case PInvoke.WM_NOTIFY:
            case PInvoke.WM_NOTIFY + MessageId.WM_REFLECT:
                NMHDR note = (NMHDR)m.GetLParam(typeof(NMHDR));
                switch (note.code)
                {
                    case PInvoke.NM_CLICK:
                    case PInvoke.NM_RCLICK:
                    case PInvoke.NM_DBLCLK:
                    case PInvoke.NM_RDBLCLK:
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

    [Obsolete("StatusBarPanelCollection has been deprecated.")]
    public class StatusBarPanelCollection : IList
    {
        private StatusBar owner;

        public StatusBarPanelCollection(StatusBar owner)
        {
            this.owner = owner;
            throw new PlatformNotSupportedException();
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public virtual StatusBarPanel this[int index]
        {
            get
            {
                throw new PlatformNotSupportedException();
            }
            set
            {
                throw new PlatformNotSupportedException();
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
                    throw new ArgumentException("SR.GetString(SR.StatusBarBadStatusBarPanel)");
                }
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public virtual StatusBarPanel this[string key]
        {
            get
            {
                throw new PlatformNotSupportedException();
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public int Count
        {
            get
            {
                throw new PlatformNotSupportedException();
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

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsReadOnly
        {
            get
            {
                throw new PlatformNotSupportedException();
            }
        }

        public virtual StatusBarPanel Add(string text)
        {
            throw new PlatformNotSupportedException();
        }

        public virtual int Add(StatusBarPanel value)
        {
            throw new PlatformNotSupportedException();
        }

        int IList.Add(object value)
        {
            if (value is StatusBarPanel)
            {
                return Add((StatusBarPanel)value);
            }
            else
            {
                throw new ArgumentException("SR.GetString(SR.StatusBarBadStatusBarPanel)");
            }
        }

        public virtual void AddRange(StatusBarPanel[] panels)
        {
            throw new PlatformNotSupportedException();
        }

        public bool Contains(StatusBarPanel panel)
        {
            throw new PlatformNotSupportedException();
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

        public virtual bool ContainsKey(string key)
        {
            throw new PlatformNotSupportedException();
        }

        public int IndexOf(StatusBarPanel panel)
        {
            throw new PlatformNotSupportedException();
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

        public virtual int IndexOfKey(string key)
        {
            throw new PlatformNotSupportedException();
        }

        public virtual void Insert(int index, StatusBarPanel value)
        {
            throw new PlatformNotSupportedException();
        }

        void IList.Insert(int index, object value)
        {
            if (value is StatusBarPanel)
            {
                Insert(index, (StatusBarPanel)value);
            }
            else
            {
                throw new ArgumentException("SR.GetString(SR.StatusBarBadStatusBarPanel)");
            }
        }

        private bool IsValidIndex(int index)
        {
            return ((index >= 0) && (index < Count));
        }

        public virtual void Clear()
        {
            throw new PlatformNotSupportedException();
        }

        public virtual void Remove(StatusBarPanel value)
        {
            throw new PlatformNotSupportedException();
        }

        void IList.Remove(object value)
        {
            if (value is StatusBarPanel)
            {
                Remove((StatusBarPanel)value);
            }
        }

        public virtual void RemoveAt(int index)
        {
            throw new PlatformNotSupportedException();
        }

        public virtual void RemoveByKey(string key)
        {
            throw new PlatformNotSupportedException();
        }

        void ICollection.CopyTo(Array dest, int index)
        {
            owner.panels.CopyTo(dest, index);
        }

        public IEnumerator GetEnumerator()
        {
            throw new PlatformNotSupportedException();
        }
    }

    private class ControlToolTip
    {
        public class Tool
        {
            public Rectangle rect = Rectangle.Empty;
            public string text;
            internal IntPtr id = new IntPtr(-1);
        }

        private Hashtable tools = new Hashtable();
        private ToolTipNativeWindow window;
        private Control parent;
        private int nextId;

        public ControlToolTip(Control parent)
        {
            window = new ToolTipNativeWindow(this);
            this.parent = parent;
        }

        protected CreateParams CreateParams
        {
            get
            {
                INITCOMMONCONTROLSEX icc = default;
                icc.dwICC = INITCOMMONCONTROLSEX_ICC.ICC_TAB_CLASSES;
                PInvoke.InitCommonControlsEx(icc);
                CreateParams cp = new CreateParams();
                cp.Parent = IntPtr.Zero;
                cp.ClassName = PInvoke.TOOLTIPS_CLASS;
                cp.Style |= (int)PInvoke.TTS_ALWAYSTIP;
                cp.ExStyle = 0;
                cp.Caption = null;
                return cp;
            }
        }

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

            if (toRemove is not null)
            {
                remove = true;
            }

            if (tool is not null)
            {
                add = true;
            }

            if (tool is not null && toRemove is not null
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

            if (tool is not null)
            {
                tools[key] = tool;
            }
            else
            {
                tools.Remove(key);
            }
        }

        public Tool GetTool(object key)
        {
            return (Tool)tools[key];
        }

        private void AddTool(Tool tool)
        {
            if (tool is not null && tool.text is not null && tool.text.Length > 0)
            {
                StatusBar p = (StatusBar)parent;
            }
        }

        private void RemoveTool(Tool tool)
        {
        }

        private void UpdateTool(Tool tool)
        {
        }

        protected void CreateHandle()
        {
            if (IsHandleCreated)
            {
                return;
            }

            window.CreateHandle(CreateParams);
        }

        protected void DestroyHandle()
        {
            if (IsHandleCreated)
            {
                window.DestroyHandle();
                tools.Clear();
            }
        }

        public void Dispose()
        {
            DestroyHandle();
        }

        ~ControlToolTip()
        {
            DestroyHandle();
        }

        protected void WndProc(ref Message msg)
        {
            switch (msg.MsgInternal)
            {
                case PInvoke.WM_SETFOCUS:
                    // bug 120872, the COMCTL StatusBar passes WM_SETFOCUS on to the DefWndProc, so
                    // it will take keyboard focus.  We don't want it doing this, so we eat
                    // the message.
                    //
                    return;
                default:
                    window.DefWndProc(ref msg);
                    break;
            }
        }

        private class ToolTipNativeWindow : NativeWindow
        {
            private ControlToolTip control;

            internal ToolTipNativeWindow(ControlToolTip control)
            {
                this.control = control;
            }

            protected override void WndProc(ref Message m)
            {
                if (control is not null)
                {
                    control.WndProc(ref m);
                }
            }
        }
    }
}
