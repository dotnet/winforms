// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Drawing;
using System.Windows.Forms.Design.Behavior;

namespace System.Windows.Forms.Design;

internal class TabControlDesigner : ParentControlDesigner
{
    private bool _tabControlSelected;
    private DesignerVerbCollection _verbs;
    private DesignerVerb _removeVerb;
    private bool _disableDrawGrid;
    private int _persistedSelectedIndex;
    private bool _addingOnInitialize;
    private bool _forwardOnDrag;

    protected override bool AllowControlLasso => false;

    protected override bool DrawGrid => !_disableDrawGrid && base.DrawGrid;

    public override bool ParticipatesWithSnapLines
    {
        get
        {
            if (!_forwardOnDrag)
            {
                return false;
            }
            else
            {
                TabPageDesigner pageDesigner = GetSelectedTabPageDesigner();
                if (pageDesigner is not null)
                {
                    return pageDesigner.ParticipatesWithSnapLines;
                }

                return true;
            }
        }
    }

    private int SelectedIndex
    {
        get => _persistedSelectedIndex;
        set
        {
            // TabBase.SelectedIndex has no validation logic, so neither do we
            _persistedSelectedIndex = value;
        }
    }

    public override DesignerVerbCollection Verbs
    {
        get
        {
            if (_verbs is null)
            {
                _removeVerb = new DesignerVerb(SR.TabControlRemove, new EventHandler(OnRemove));

                _verbs = new DesignerVerbCollection();
                _verbs.Add(new DesignerVerb(SR.TabControlAdd, new EventHandler(OnAdd)));
                _verbs.Add(_removeVerb);
            }

            if (Control is not null)
            {
                _removeVerb.Enabled = Control.Controls.Count > 0;
            }

            return _verbs;
        }
    }

    public override void InitializeNewComponent(IDictionary defaultValues)
    {
        base.InitializeNewComponent(defaultValues);

        // Add 2 tab pages
        // member is OK to be null...
        try
        {
            _addingOnInitialize = true;
            OnAdd(this, EventArgs.Empty);
            OnAdd(this, EventArgs.Empty);
        }
        finally
        {
            _addingOnInitialize = false;
        }

        MemberDescriptor member = TypeDescriptor.GetProperties(component: Component)["Controls"];
        RaiseComponentChanging(member);
        RaiseComponentChanged(member, null, null);

        TabControl tc = (TabControl)Component;
        if (tc is not null)
        { // always Select the First Tab on Initializing the component...
            tc.SelectedIndex = 0;
        }
    }

    // If the TabControl already contains the control we are dropping then don't allow the drop.
    // I.e. we don't want to allow local drag-drop for TabControls.
    public override bool CanParent(Control control) => (control is TabPage && !Control.Contains(control));

    private void CheckVerbStatus()
    {
        if (_removeVerb is not null)
        {
            _removeVerb.Enabled = Control.Controls.Count > 0;
        }
    }

    protected override IComponent[] CreateToolCore(ToolboxItem tool, int x, int y, int width, int height, bool hasLocation, bool hasSize)
    {
        TabControl tc = ((TabControl)Control);
        // VSWhidbey #409457
        if (tc.SelectedTab is null)
        {
            throw new ArgumentException(string.Format(SR.TabControlInvalidTabPageType, tool.DisplayName));
        }

        IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
        if (host is not null)
        {
            TabPageDesigner selectedTabPageDesigner = host.GetDesigner(tc.SelectedTab) as TabPageDesigner;
            InvokeCreateTool(selectedTabPageDesigner, tool);
        }

        // InvokeCreate Tool will do the necessary hookups.
        return null;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ISelectionService svc = (ISelectionService)GetService(typeof(ISelectionService));
            if (svc is not null)
            {
                svc.SelectionChanged -= OnSelectionChanged;
            }

            IComponentChangeService cs = (IComponentChangeService)GetService(typeof(IComponentChangeService));
            if (cs is not null)
            {
                cs.ComponentChanged -= OnComponentChanged;
            }

            if (HasComponent && Control is TabControl tabControl)
            {
                tabControl.SelectedIndexChanged -= OnTabSelectedIndexChanged;
                tabControl.GotFocus -= OnGotFocus;
                tabControl.RightToLeftLayoutChanged -= OnRightToLeftLayoutChanged;
                tabControl.ControlAdded -= OnControlAdded;
            }
        }

        base.Dispose(disposing);
    }

    protected override bool GetHitTest(Point point)
    {
        TabControl tc = ((TabControl)Control);

        // tabControlSelected tells us if a tab page or the tab control itself is selected.
        // If the tab control is selected, then we need to return true from here - so we can switch back and forth
        // between tabs. If we're not currently selected, we want to select the tab control
        // so return false.
        if (_tabControlSelected)
        {
            Point hitTest = Control.PointToClient(point);
            return !tc.DisplayRectangle.Contains(hitTest);
        }

        return false;
    }

    internal static TabPage GetTabPageOfComponent(TabControl parent, object comp)
    {
        if (!(comp is Control))
        {
            return null;
        }

        Control c = (Control)comp;
        while (c is not null)
        {
            TabPage page = c as TabPage;
            if (page is not null && page.Parent == parent)
            {
                return page;
            }

            c = c.Parent;
        }

        return null;
    }

    public override void Initialize(IComponent component)
    {
        base.Initialize(component);

        AutoResizeHandles = true;
        TabControl control = component as TabControl;
        Debug.Assert(control is not null, "Component must be a tab control, it is a: " + component.GetType().FullName);

        ISelectionService svc = (ISelectionService)GetService(typeof(ISelectionService));
        if (svc is not null)
        {
            svc.SelectionChanged += OnSelectionChanged;
        }

        IComponentChangeService cs = (IComponentChangeService)GetService(typeof(IComponentChangeService));
        if (cs is not null)
        {
            cs.ComponentChanged += OnComponentChanged;
        }

        if (control is not null)
        {
            control.SelectedIndexChanged += OnTabSelectedIndexChanged;
            control.GotFocus += OnGotFocus;
            control.RightToLeftLayoutChanged += OnRightToLeftLayoutChanged;
            control.ControlAdded += OnControlAdded;
        }
    }

    private void OnAdd(object sender, EventArgs eevent)
    {
        TabControl tc = (TabControl)Component;

        IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
        if (host is not null)
        {
            DesignerTransaction t = null;
            try
            {
                try
                {
                    t = host.CreateTransaction(string.Format(SR.TabControlAddTab, Component.Site.Name));
                }
                catch (CheckoutException ex)
                {
                    if (ex == CheckoutException.Canceled)
                    {
                        return;
                    }

                    throw new CheckoutException("Checkout Error", ex);
                }

                MemberDescriptor member = TypeDescriptor.GetProperties(tc)["Controls"];
                TabPage page = (TabPage)host.CreateComponent(typeof(TabPage));
                if (!_addingOnInitialize)
                {
                    RaiseComponentChanging(member);
                }

                // NOTE:  We also modify padding of TabPages added through the TabPageCollectionEditor.
                // If you need to change the default Padding, change it there as well.
                page.Padding = new Padding(3);

                string pageText = null;

                PropertyDescriptor nameProp = TypeDescriptor.GetProperties(page)["Name"];
                if (nameProp is not null && nameProp.PropertyType == typeof(string))
                {
                    pageText = nameProp.GetValue(page) as string;
                }

                if (pageText is not null)
                {
                    PropertyDescriptor textProperty = TypeDescriptor.GetProperties(page)["Text"];
                    Debug.Assert(textProperty is not null, "Could not find 'Text' property in TabPage.");
                    textProperty?.SetValue(page, pageText);
                }

                PropertyDescriptor styleProp = TypeDescriptor.GetProperties(page)["UseVisualStyleBackColor"];
                if (styleProp is not null && styleProp.PropertyType == typeof(bool) && !styleProp.IsReadOnly && styleProp.IsBrowsable)
                {
                    styleProp.SetValue(page, true);
                }

                tc.Controls.Add(page);
                // Make sure that the last tab is selected.
                tc.SelectedIndex = tc.TabCount - 1;
                if (!_addingOnInitialize)
                {
                    RaiseComponentChanged(member, null, null);
                }
            }
            finally
            {
                t?.Commit();
            }
        }
    }

    private void OnComponentChanged(object sender, ComponentChangedEventArgs e) => CheckVerbStatus();

    private void OnGotFocus(object sender, EventArgs e)
    {
        IEventHandlerService eventSvc = (IEventHandlerService)GetService(typeof(IEventHandlerService));
        if (eventSvc is not null)
        {
            Control focusWnd = eventSvc.FocusWindow;
            focusWnd?.Focus();
        }
    }

    private void OnRemove(object sender, EventArgs eevent)
    {
        TabControl tc = (TabControl)Component;

        // if the control is null, or there are not tab pages, get out!...
        if (tc is null || tc.TabPages.Count == 0)
        {
            return;
        }

        // member is OK to be null...
        MemberDescriptor member = TypeDescriptor.GetProperties(Component)["Controls"];

        TabPage tp = tc.SelectedTab;

        // destroy the page
        IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
        if (host is not null)
        {
            DesignerTransaction t = null;
            try
            {
                try
                {
                    t = host.CreateTransaction(string.Format(SR.TabControlRemoveTab, ((IComponent)tp).Site.Name, Component.Site.Name));
                    RaiseComponentChanging(member);
                }
                catch (CheckoutException ex)
                {
                    if (ex == CheckoutException.Canceled)
                    {
                        return;
                    }

                    throw new CheckoutException("Checkout Error", ex);
                }

                if (tp is not null)
                {
                    host.DestroyComponent(tp);
                }

                RaiseComponentChanged(member, null, null);
            }
            finally
            {
                t?.Commit();
            }
        }
    }

    protected override void OnPaintAdornments(PaintEventArgs pe)
    {
        try
        {
            _disableDrawGrid = true;
            // we don't want to do this for the tab control designer because you can't drag anything onto it anyway.
            // so we will always return false for draw grid.
            base.OnPaintAdornments(pe);
        }
        finally
        {
            _disableDrawGrid = false;
        }
    }

    private void OnControlAdded(object sender, ControlEventArgs e)
    {
        if (e.Control is not null && !e.Control.IsHandleCreated)
        {
            // Force handle creation.
            _ = e.Control.Handle;
        }
    }

    private void OnRightToLeftLayoutChanged(object sender, EventArgs e) => BehaviorService?.SyncSelection();

    private void OnSelectionChanged(object sender, EventArgs e)
    {
        ISelectionService svc = (ISelectionService)GetService(typeof(ISelectionService));

        _tabControlSelected = false;// this is for HitTest purposes

        if (svc is not null)
        {
            ICollection selComponents = svc.GetSelectedComponents();

            TabControl tabControl = (TabControl)Component;

            foreach (object comp in selComponents)
            {
                if (comp == tabControl)
                {
                    _tabControlSelected = true;// this is for HitTest purposes
                }

                TabPage page = GetTabPageOfComponent(tabControl, comp);

                if (page is not null && page.Parent == tabControl)
                {
                    _tabControlSelected = false; // this is for HitTest purposes
                    tabControl.SelectedTab = page;
                    SelectionManager selMgr = (SelectionManager)GetService(typeof(SelectionManager));
                    selMgr.Refresh();
                    break;
                }
            }
        }
    }

    private void OnTabSelectedIndexChanged(object sender, EventArgs e)
    {
        // if this was called as a result of a prop change, don't set the selection to the control (causes flicker)
        // Attempt to select the tab control
        ISelectionService svc = (ISelectionService)GetService(typeof(ISelectionService));
        if (svc is not null)
        {
            ICollection selComponents = svc.GetSelectedComponents();

            TabControl tabControl = (TabControl)Component;
            bool selectedComponentOnTab = false;

            foreach (object comp in selComponents)
            {
                TabPage page = GetTabPageOfComponent(tabControl, comp);
                if (page is not null && page.Parent == tabControl && page == tabControl.SelectedTab)
                {
                    selectedComponentOnTab = true;
                    break;
                }
            }

            if (!selectedComponentOnTab)
            {
                svc.SetSelectedComponents(new object[] { Component });
            }
        }
    }

    protected override void PreFilterProperties(IDictionary properties)
    {
        base.PreFilterProperties(properties);

        // Handle shadowed properties
        string[] shadowProps =
            [
                "SelectedIndex",
            ];

        Attribute[] empty = [];

        for (int i = 0; i < shadowProps.Length; i++)
        {
            PropertyDescriptor prop = properties[shadowProps[i]] as PropertyDescriptor;
            if (prop is not null)
            {
                properties[shadowProps[i]] = TypeDescriptor.CreateProperty(typeof(TabControlDesigner), prop, empty);
            }
        }
    }

    private TabPageDesigner GetSelectedTabPageDesigner()
    {
        TabPageDesigner pageDesigner = null;
        TabPage selectedTab = ((TabControl)Component).SelectedTab;
        if (selectedTab is not null)
        {
            IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
            if (host is not null)
            {
                pageDesigner = host.GetDesigner(selectedTab) as TabPageDesigner;
            }
        }

        return pageDesigner;
    }

    protected override void OnDragEnter(DragEventArgs de)
    {
        // Check what we are dragging... If we are just dragging tab pages, then we do not want to forward the OnDragXXX
        _forwardOnDrag = false;

        DropSourceBehavior.BehaviorDataObject data = de.Data as DropSourceBehavior.BehaviorDataObject;
        if (data is not null)
        {
            List<IComponent> dragControls = data.GetSortedDragControls(out _);
            if (dragControls is not null)
            {
                for (int i = 0; i < dragControls.Count; i++)
                {
                    if (!(dragControls[i] is Control) || (dragControls[i] is Control && !(dragControls[i] is TabPage)))
                    {
                        _forwardOnDrag = true;
                        break;
                    }
                }
            }
        }
        else
        {
            // We must be dragging something off the toolbox, so forward the drag to the right TabPage.
            _forwardOnDrag = true;
        }

        if (_forwardOnDrag)
        {
            TabPageDesigner pageDesigner = GetSelectedTabPageDesigner();
            pageDesigner?.OnDragEnterInternal(de);
        }
        else
        {
            base.OnDragEnter(de);
        }
    }

    protected override void OnDragDrop(DragEventArgs de)
    {
        if (_forwardOnDrag)
        {
            TabPageDesigner pageDesigner = GetSelectedTabPageDesigner();
            pageDesigner?.OnDragDropInternal(de);
        }
        else
        {
            base.OnDragDrop(de);
        }

        _forwardOnDrag = false;
    }

    protected override void OnDragLeave(EventArgs e)
    {
        if (_forwardOnDrag)
        {
            TabPageDesigner pageDesigner = GetSelectedTabPageDesigner();
            pageDesigner?.OnDragLeaveInternal(e);
        }
        else
        {
            base.OnDragLeave(e);
        }

        _forwardOnDrag = false;
    }

    protected override void OnDragOver(DragEventArgs de)
    {
        if (_forwardOnDrag)
        {
            // Need to make sure that we are over a valid area. VSWhidbey# 354139. Now that all dragging/dropping is done via
            // the behavior service and adorner window, we have to do our own validation, and cannot rely on the OS to do it for us.
            TabControl tc = ((TabControl)Control);
            Point dropPoint = Control.PointToClient(new Point(de.X, de.Y));
            if (!tc.DisplayRectangle.Contains(dropPoint))
            {
                de.Effect = DragDropEffects.None;
                return;
            }

            TabPageDesigner pageDesigner = GetSelectedTabPageDesigner();
            pageDesigner?.OnDragOverInternal(de);
        }
        else
        {
            base.OnDragOver(de);
        }
    }

    protected override void OnGiveFeedback(GiveFeedbackEventArgs e)
    {
        if (_forwardOnDrag)
        {
            TabPageDesigner pageDesigner = GetSelectedTabPageDesigner();
            pageDesigner?.OnGiveFeedbackInternal(e);
        }
        else
        {
            base.OnGiveFeedback(e);
        }
    }

    protected override void WndProc(ref Message m)
    {
        switch (m.MsgInternal)
        {
            case PInvokeCore.WM_NCHITTEST:
                // The tab control always fires HTTRANSPARENT in empty areas, which causes the message to go to our parent. We want
                // the tab control's designer to get these messages, however, so change this.
                base.WndProc(ref m);
                if (m.ResultInternal == PInvoke.HTTRANSPARENT)
                {
                    m.ResultInternal = (LRESULT)(nint)PInvoke.HTCLIENT;
                }

                break;
            case PInvokeCore.WM_CONTEXTMENU:
                // We handle this in addition to a right mouse button.
                // Why?  Because we often eat the right mouse button, so
                // it may never generate a WM_CONTEXTMENU.  However, the
                // system may generate one in response to an F-10.
                int x = PARAM.SignedLOWORD(m.LParamInternal);
                int y = PARAM.SignedHIWORD(m.LParamInternal);
                if (x == -1 && y == -1)
                {
                    // for shift-F10
                    Point p = Cursor.Position;
                    x = p.X;
                    y = p.Y;
                }

                OnContextMenu(x, y);
                break;
            case PInvokeCore.WM_HSCROLL:
            case PInvokeCore.WM_VSCROLL:
                // We do this so that we can update the areas covered by glyphs correctly. VSWhidbey# 187405.
                // We just invalidate the area corresponding to the ClientRectangle in the AdornerWindow.
                BehaviorService.Invalidate(BehaviorService.ControlRectInAdornerWindow(Control));
                base.WndProc(ref m);
                break;
            default:
                base.WndProc(ref m);
                break;
        }
    }
}
