// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Windows.Forms.Design.Behavior;
using Microsoft.Win32;

namespace System.Windows.Forms.Design;

/// <summary>
///  Provides the component tray UI for the form designer.
/// </summary>
[ToolboxItem(false)]
[DesignTimeVisible(false)]
[ProvideProperty("Location", typeof(IComponent))]
[ProvideProperty("TrayLocation", typeof(IComponent))]
public class ComponentTray : ScrollableControl, IExtenderProvider, ISelectionUIHandler, IOleDragClient
{
    private static Point InvalidPoint { get; } = new(int.MinValue, int.MinValue);
    private IServiceProvider _serviceProvider; // Where services come from.
    private Point _whiteSpace = Point.Empty; // space to leave between components.
    private Size _grabHandle = Size.Empty; // Size of the grab handles.

    private List<Control> _controls; // List of items in the tray in the order of their layout.
    private SelectionUIHandler _dragHandler; // the thing responsible for handling mouse drags
    private ISelectionUIService _selectionUISvc; // selection UI; we use this a lot
    private IToolboxService _toolboxService; // cached for drag/drop

    /// <summary>
    ///  Provides drag and drop functionality through OLE.
    /// </summary>
    internal OleDragDropHandler _oleDragDropHandler; // handler class for ole drag drop operations.

    private readonly IDesigner _mainDesigner; // the designer that is associated with this tray
    private IEventHandlerService _eventHandlerService; // Event Handler service to handle keyboard and focus.
    private bool _queriedTabOrder;
    private MenuCommand _tabOrderCommand;
    private ICollection _selectedObjects;

    // Services that we use on a high enough frequency to merit caching.
    private IMenuCommandService _menuCommandService;
    private InheritanceUI _inheritanceUI;

    private Point _mouseDragStart = InvalidPoint; // the starting location of a drag
    private Point _mouseDragEnd = InvalidPoint; // the ending location of a drag
    private Rectangle _mouseDragWorkspace = Rectangle.Empty; // a temp work rectangle we cache for perf
    private ToolboxItem _mouseDragTool; // the tool that's being dragged; only for drag/drop
    private Point _mouseDropLocation = InvalidPoint; // where the tool was dropped
    private bool _showLargeIcons; // Show Large icons or not.
    private bool _autoArrange; // allows for auto arranging icons.
    private Point _autoScrollPosBeforeDragging = Point.Empty; // Used to return the correct scroll pos. after a drag

    // Component Tray Context menu items...
    private readonly MenuCommand _menucmdArrangeIcons;
    private readonly MenuCommand _menucmdLineupIcons;
    private readonly MenuCommand _menucmdLargeIcons;
    private bool _fResetAmbient;
    private bool _fSelectionChanged;
    private ComponentTrayGlyphManager _glyphManager; // used to manage any glyphs added to the tray

    // Empty class for build time dependancy

    /// <summary>
    ///  Creates a new component tray. The component tray
    ///  will monitor component additions and removals and create
    ///  appropriate UI objects in its space.
    /// </summary>
    public ComponentTray(IDesigner mainDesigner, IServiceProvider serviceProvider)
    {
        AutoScroll = true;
        _mainDesigner = mainDesigner;
        _serviceProvider = serviceProvider;
        AllowDrop = true;
        Text = "ComponentTray"; // makes debugging easier
        SetStyle(ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
        _controls = [];
        IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
        IExtenderProviderService es = (IExtenderProviderService)GetService(typeof(IExtenderProviderService));
        Debug.Assert(es is not null, "Component tray wants an extender provider service, but there isn't one.");
        es?.AddExtenderProvider(this);

        if (GetService(typeof(IEventHandlerService)) is null)
        {
            if (host is not null)
            {
                _eventHandlerService = new EventHandlerService(this);
                host.AddService(_eventHandlerService);
            }
        }

        IMenuCommandService mcs = MenuService;
        if (mcs is not null)
        {
            Debug.Assert(_menucmdArrangeIcons is null, "Non-Null Menu Command for ArrangeIcons");
            Debug.Assert(_menucmdLineupIcons is null, "Non-Null Menu Command for LineupIcons");
            Debug.Assert(_menucmdLargeIcons is null, "Non-Null Menu Command for LargeIcons");
            _menucmdArrangeIcons = new MenuCommand(OnMenuArrangeIcons, StandardCommands.ArrangeIcons);
            _menucmdLineupIcons = new MenuCommand(OnMenuLineupIcons, StandardCommands.LineupIcons);
            _menucmdLargeIcons = new MenuCommand(OnMenuShowLargeIcons, StandardCommands.ShowLargeIcons);
            _menucmdArrangeIcons.Checked = AutoArrange;
            _menucmdLargeIcons.Checked = ShowLargeIcons;
            mcs.AddCommand(_menucmdArrangeIcons);
            mcs.AddCommand(_menucmdLineupIcons);
            mcs.AddCommand(_menucmdLargeIcons);
        }

        IComponentChangeService componentChangeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));
        if (componentChangeService is not null)
        {
            componentChangeService.ComponentRemoved += OnComponentRemoved;
        }

        if (GetService(typeof(IUIService)) is IUIService uiService)
        {
            Color styleColor;
            if (uiService.Styles["ArtboardBackground"] is Color backgroundColor)
            {
                styleColor = backgroundColor;
            }

            // Can't use 'as' here since Color is a value type
            else if (uiService.Styles["VsColorDesignerTray"] is Color designerTrayColor)
            {
                styleColor = designerTrayColor;
            }
            else if (uiService.Styles["HighlightColor"] is Color highlightColor)
            {
                // Since v1, we have had code here that checks for HighlightColor,
                // so some hosts (like WinRes) have been setting it.
                // If VsColorDesignerTray isn't present, we look for HighlightColor for backward compatibility.
                styleColor = highlightColor;
            }
            else
            {
                // No style color provided? Let's pick a default.
                styleColor = SystemColors.Info;
            }

            if (uiService.Styles["ArtboardBackgroundText"] is Color backgroundTextColor)
            {
                ForeColor = backgroundTextColor;
            }
            else if (uiService.Styles["VsColorPanelText"] is Color panelTextColor)
            {
                ForeColor = panelTextColor;
            }

            BackColor = styleColor;
            Font = (Font)uiService.Styles["DialogFont"];
        }

        ISelectionService selSvc = (ISelectionService)GetService(typeof(ISelectionService));
        if (selSvc is not null)
        {
            selSvc.SelectionChanged += OnSelectionChanged;
        }

        // Listen to the SystemEvents so that we can resync selection based on display settings etc.
        SystemEvents.DisplaySettingsChanged += OnSystemSettingChanged;
        SystemEvents.InstalledFontsChanged += OnSystemSettingChanged;
        SystemEvents.UserPreferenceChanged += OnUserPreferenceChanged;

        if (GetService(typeof(BehaviorService)) is BehaviorService behSvc)
        {
            // this object will manage any glyphs that get added to our tray
            _glyphManager = new ComponentTrayGlyphManager(behSvc);
        }
    }

    private void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
    {
        if (IsHandleCreated)
        {
            _fResetAmbient = true;
            ResetTrayControls();
            BeginInvoke(new AsyncInvokeHandler(Invalidate), [true]);
        }
    }

    private void OnComponentRefresh(RefreshEventArgs e)
    {
        if (e.ComponentChanged is IComponent component)
        {
            TrayControl control = TrayControl.FromComponent(component);
            if (control is not null)
            {
                bool shouldDisplay = CanDisplayComponent(component);
                if (shouldDisplay != control.Visible || !shouldDisplay)
                {
                    control.Visible = shouldDisplay;
                    Rectangle bounds = control.Bounds;
                    bounds.Inflate(_grabHandle);
                    bounds.Inflate(_grabHandle);
                    Invalidate(bounds);
                    PerformLayout();
                }
            }
        }
    }

    private void OnSystemSettingChanged(object sender, EventArgs e)
    {
        if (IsHandleCreated)
        {
            _fResetAmbient = true;
            ResetTrayControls();
            BeginInvoke(new AsyncInvokeHandler(Invalidate), [true]);
        }
    }

    private void ResetTrayControls()
    {
        ControlCollection children = Controls;
        if (children is null)
        {
            return;
        }

        for (int i = 0; i < children.Count; ++i)
        {
            if (children[i] is TrayControl tc)
            {
                tc._fRecompute = true;
            }
        }
    }

    private delegate void AsyncInvokeHandler(bool children);

    private void OnSelectionChanged(object sender, EventArgs e)
    {
        _selectedObjects = ((ISelectionService)sender).GetSelectedComponents();
        object primary = ((ISelectionService)sender).PrimarySelection;
        Invalidate();
        _fSelectionChanged = true;

        // Accessibility information
        foreach (object selObj in _selectedObjects)
        {
            if (selObj is IComponent component)
            {
                Control c = TrayControl.FromComponent(component);
                if (c is not null)
                {
                    PInvoke.NotifyWinEvent(
                        (uint)AccessibleEvents.SelectionAdd,
                        c,
                        (int)OBJECT_IDENTIFIER.OBJID_CLIENT,
                        (int)PInvoke.CHILDID_SELF);
                }
            }
        }

        if (primary is IComponent comp)
        {
            Control c = TrayControl.FromComponent(comp);
            if (c is not null && IsHandleCreated)
            {
                ScrollControlIntoView(c);
                PInvoke.NotifyWinEvent(
                    (uint)AccessibleEvents.Focus,
                    c,
                    (int)OBJECT_IDENTIFIER.OBJID_CLIENT,
                    (int)PInvoke.CHILDID_SELF);
            }

            if (_glyphManager is not null)
            {
                _glyphManager.SelectionGlyphs.Clear();
                IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
                foreach (object selObj in _selectedObjects)
                {
                    if (selObj is IComponent selectedComponent && !(host.GetDesigner(selectedComponent) is ControlDesigner))
                    { // don't want to do it for controls that are also in the tray
                        GlyphCollection glyphs = _glyphManager.GetGlyphsForComponent(selectedComponent);
                        if (glyphs is not null && glyphs.Count > 0)
                        {
                            SelectionGlyphs.AddRange(glyphs);
                        }
                    }
                }
            }
        }
    }

    private void OnComponentRemoved(object sender, ComponentEventArgs cevent)
    {
        RemoveComponent(cevent.Component);
    }

    private void OnMenuShowLargeIcons(object sender, EventArgs e)
    {
        IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
        DesignerTransaction t = null;
        try
        {
            t = host.CreateTransaction(SR.TrayShowLargeIcons);
            PropertyDescriptor trayIconProp = TypeDescriptor.GetProperties(_mainDesigner.Component)["TrayLargeIcon"];
            trayIconProp?.SetValue(_mainDesigner.Component, !ShowLargeIcons);
        }
        finally
        {
            t?.Commit();
        }
    }

    private void OnMenuLineupIcons(object sender, EventArgs e)
    {
        IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
        DesignerTransaction t = null;
        try
        {
            t = host.CreateTransaction(SR.TrayLineUpIcons);
            DoLineupIcons();
        }
        finally
        {
            t?.Commit();
        }
    }

    private void DoLineupIcons()
    {
        if (_autoArrange)
        {
            return;
        }

        bool oldValue = _autoArrange;
        _autoArrange = true;
        try
        {
            DoAutoArrange(true);
        }
        finally
        {
            _autoArrange = oldValue;
        }
    }

    private void DoAutoArrange(bool dirtyDesigner)
    {
        if (_controls is null || _controls.Count <= 0)
        {
            return;
        }

        _controls.Sort(new AutoArrangeComparer());

        SuspendLayout();

        // Reset the autoscroll position before auto arranging.
        // This way, when OnLayout gets fired after this, we won't
        // have to move every component again. Note that syncing
        // the selection will automatically select & scroll into view
        // the right components
        AutoScrollPosition = new Point(0, 0);

        try
        {
            Control prevCtl = null;
            bool positionedGlobal = true;
            foreach (Control ctl in _controls)
            {
                if (!ctl.Visible)
                {
                    continue;
                }

                // If we're auto arranging, always move the control. If not, move the control only
                // if it was never given a position. This auto arranges it until the user messes with it,
                // or until its position is saved into the resx. (if one control is no longer positioned,
                // move all the other one as we don't want them to go under one another)
                if (_autoArrange)
                {
                    PositionInNextAutoSlot(ctl as TrayControl, prevCtl, dirtyDesigner);
                }
                else if (!((TrayControl)ctl).Positioned || !positionedGlobal)
                {
                    PositionInNextAutoSlot(ctl as TrayControl, prevCtl, false);
                    positionedGlobal = false;
                }

                prevCtl = ctl;
            }

            _selectionUISvc?.SyncSelection();
        }
        finally
        {
            ResumeLayout();
        }
    }

    private void OnMenuArrangeIcons(object sender, EventArgs e)
    {
        IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
        DesignerTransaction t = null;

        try
        {
            t = host.CreateTransaction(SR.TrayAutoArrange);

            PropertyDescriptor trayAAProp = TypeDescriptor.GetProperties(_mainDesigner.Component)["TrayAutoArrange"];
            trayAAProp?.SetValue(_mainDesigner.Component, !AutoArrange);
        }
        finally
        {
            t?.Commit();
        }
    }

    public bool AutoArrange
    {
        get => _autoArrange;
        set
        {
            if (_autoArrange != value)
            {
                _autoArrange = value;
                _menucmdArrangeIcons.Checked = value;

                if (_autoArrange)
                {
                    DoAutoArrange(true);
                }
            }
        }
    }

    /// <summary>
    ///  Gets the number of components contained within this tray.
    /// </summary>
    public int ComponentCount
    {
        get => Controls.Count;
    }

    internal GlyphCollection SelectionGlyphs
    {
        get
        {
            if (_glyphManager is not null)
            {
                return _glyphManager.SelectionGlyphs;
            }
            else
            {
                return null;
            }
        }
    }

    /// <summary>
    ///  Determines whether the tray will show large icon view or not.
    /// </summary>
    public bool ShowLargeIcons
    {
        get => _showLargeIcons;
        set
        {
            if (_showLargeIcons != value)
            {
                _showLargeIcons = value;
                _menucmdLargeIcons.Checked = ShowLargeIcons;

                ResetTrayControls();
                Invalidate(true);
            }
        }
    }

    bool IExtenderProvider.CanExtend(object extendee)
    {
        return (extendee is IComponent comp) && (TrayControl.FromComponent(comp) is not null);
    }

    IComponent IOleDragClient.Component
    {
        get => _mainDesigner.Component;
    }

    bool IOleDragClient.CanModifyComponents
    {
        get => true;
    }

    bool IOleDragClient.AddComponent(IComponent component, string name, bool firstAdd)
    {
        // the designer for controls decides what to do here
        if (_mainDesigner is IOleDragClient oleDragClient)
        {
            try
            {
                oleDragClient.AddComponent(component, name, firstAdd);
                PositionControl(TrayControl.FromComponent(component));
                _mouseDropLocation = InvalidPoint;
                return true;
            }
            catch
            {
            }
        }
        else
        {
            // for webforms (98109) just add the component directly to the host
            IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
            try
            {
                if (host is not null && host.Container is not null)
                {
                    if (host.Container.Components[name] is not null)
                    {
                        name = null;
                    }

                    host.Container.Add(component, name);
                    return true;
                }
            }
            catch
            {
            }
        }

        Debug.Fail("Don't know how to add component!");
        return false;
    }

    bool IOleDragClient.IsDropOk(IComponent component) => true;

    Control IOleDragClient.GetDesignerControl() => this;

    Control IOleDragClient.GetControlForComponent(object component)
    {
        if (component is IComponent comp)
        {
            return TrayControl.FromComponent(comp);
        }

        Debug.Fail("component is not IComponent");
        return null;
    }

    bool ISelectionUIHandler.BeginDrag(object[] components, SelectionRules rules, int initialX, int initialY)
    {
        if (TabOrderActive)
        {
            return false;
        }

        bool result = DragHandler.BeginDrag(components, rules, initialX, initialY);
        if (result)
        {
            if (!GetOleDragHandler().DoBeginDrag(components, rules, initialX, initialY))
            {
                return false;
            }
        }

        return result;
    }

    internal virtual OleDragDropHandler GetOleDragHandler()
    {
        _oleDragDropHandler ??= new TrayOleDragDropHandler(DragHandler, _serviceProvider, this);

        return _oleDragDropHandler;
    }

    internal virtual SelectionUIHandler DragHandler
    {
        get
        {
            _dragHandler ??= new TraySelectionUIHandler(this);

            return _dragHandler;
        }
    }

    void ISelectionUIHandler.DragMoved(object[] components, Rectangle offset) => DragHandler.DragMoved(components, offset);

    void ISelectionUIHandler.EndDrag(object[] components, bool cancel)
    {
        DragHandler.EndDrag(components, cancel);
        GetOleDragHandler().DoEndDrag();
        // Here, after the drag is finished and after we have resumed layout,
        // adjust the location of the components we dragged by the scroll offset
        if (!_autoScrollPosBeforeDragging.IsEmpty)
        {
            foreach (IComponent comp in components)
            {
                TrayControl tc = TrayControl.FromComponent(comp);
                if (tc is not null)
                {
                    SetTrayLocation(comp, new Point(tc.Location.X - _autoScrollPosBeforeDragging.X, tc.Location.Y - _autoScrollPosBeforeDragging.Y));
                }
            }

            AutoScrollPosition = new Point(-_autoScrollPosBeforeDragging.X, -_autoScrollPosBeforeDragging.Y);
        }
    }

    // We render the selection UI glyph ourselves.
    Rectangle ISelectionUIHandler.GetComponentBounds(object component) => Rectangle.Empty;

    SelectionRules ISelectionUIHandler.GetComponentRules(object component) => SelectionRules.Visible | SelectionRules.Moveable;

    Rectangle ISelectionUIHandler.GetSelectionClipRect(object component)
    {
        if (IsHandleCreated)
        {
            return RectangleToScreen(ClientRectangle);
        }

        return Rectangle.Empty;
    }

    void ISelectionUIHandler.OnSelectionDoubleClick(IComponent component)
    {
        if (!TabOrderActive)
        {
            if (((IOleDragClient)this).GetControlForComponent(component) is TrayControl tc)
            {
                tc.ViewDefaultEvent(component);
            }
        }
    }

    bool ISelectionUIHandler.QueryBeginDrag(object[] components, SelectionRules rules, int initialX, int initialY)
        => DragHandler.QueryBeginDrag(components);

    void ISelectionUIHandler.ShowContextMenu(IComponent component) => OnContextMenu(MousePosition);

    private void OnContextMenu(Point location)
    {
        if (TabOrderActive)
        {
            return;
        }

        Capture = false;
        IMenuCommandService mcs = MenuService;
        if (mcs is not null)
        {
            Capture = false;
            Cursor.Clip = Rectangle.Empty;
            ISelectionService selectionService = (ISelectionService)GetService(typeof(ISelectionService));
            if (selectionService is not null
                && !(selectionService.SelectionCount == 1 && selectionService.PrimarySelection == _mainDesigner.Component))
            {
                mcs.ShowContextMenu(MenuCommands.TraySelectionMenu, location.X, location.Y);
            }
            else
            {
                mcs.ShowContextMenu(MenuCommands.ComponentTrayMenu, location.X, location.Y);
            }
        }
    }

    void ISelectionUIHandler.OleDragEnter(DragEventArgs de) => GetOleDragHandler().DoOleDragEnter(de);

    void ISelectionUIHandler.OleDragDrop(DragEventArgs de) => GetOleDragHandler().DoOleDragDrop(de);

    void ISelectionUIHandler.OleDragOver(DragEventArgs de) => GetOleDragHandler().DoOleDragOver(de);

    void ISelectionUIHandler.OleDragLeave() => GetOleDragHandler().DoOleDragLeave();

    /// <summary>
    ///  Adds a component to the tray.
    /// </summary>
    public virtual void AddComponent(IComponent component)
    {
        IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
        // Ignore components that cannot be added to the tray
        if (!CanDisplayComponent(component))
        {
            return;
        }

        // And designate us as the selection UI handler for the control.
        if (_selectionUISvc is null)
        {
            _selectionUISvc = (ISelectionUIService)GetService(typeof(ISelectionUIService));

            // If there is no selection service, then we will provide our own.
            if (_selectionUISvc is null)
            {
                _selectionUISvc = new SelectionUIService(host);
                host.AddService(_selectionUISvc);
            }

            _grabHandle = _selectionUISvc.GetAdornmentDimensions(AdornmentType.GrabHandle);
        }

        // Create a new instance of a tray control.
        TrayControl trayctl = new(this, component);
        SuspendLayout();
        try
        {
            // Add it to us.
            Controls.Add(trayctl);
            _controls.Add(trayctl);
            // CanExtend can actually be called BEFORE the component is added to the ComponentTray.
            // ToolStrip is such as scenario:
            // 1. Add a timer to the Tray.
            // 2. Add a ToolStrip.
            // 3. ToolStripDesigner.Initialize will be called before ComponentTray.AddComponent,
            //    so the ToolStrip is not yet added to the tray.
            // 4. TooStripDesigner.Initialize calls GetProperties, which causes our CanExtend to be called.
            // 5. CanExtend will return false, since the component has not yet been added.
            // 6. This causes all sorts of badness
            // Fix is to refresh.
            TypeDescriptor.Refresh(component);
            if (host is not null && !host.Loading)
            {
                PositionControl(trayctl);
            }

            _selectionUISvc?.AssignSelectionUIHandler(component, this);

            InheritanceAttribute attr = trayctl.InheritanceAttribute;
            if (attr.InheritanceLevel != InheritanceLevel.NotInherited)
            {
                InheritanceUI iui = InheritanceUI;
                iui?.AddInheritedControl(trayctl, attr.InheritanceLevel);
            }
        }
        finally
        {
            ResumeLayout();
        }

        if (host is not null && !host.Loading)
        {
            ScrollControlIntoView(trayctl);
        }
    }

    [CLSCompliant(false)]
    protected virtual bool CanCreateComponentFromTool(ToolboxItem tool)
    {
        IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
        Debug.Assert(host is not null, "Service object could not provide us with a designer host.");
        // Disallow controls to be added to the component tray.
        Type compType = host.GetType(tool.TypeName);
        if (compType is null)
        {
            return true;
        }

        if (!compType.IsSubclassOf(typeof(Control)))
        {
            return true;
        }

        Type designerType = GetDesignerType(compType, typeof(IDesigner));
        return !(typeof(ControlDesigner).IsAssignableFrom(designerType));
    }

    private Type GetDesignerType(Type t, Type designerBaseType)
    {
        Type designerType = null;
        // Get the set of attributes for this type
        AttributeCollection attributes = TypeDescriptor.GetAttributes(t);
        for (int i = 0; i < attributes.Count; i++)
        {
            if (attributes[i] is DesignerAttribute da)
            {
                Type attributeBaseType = Type.GetType(da.DesignerBaseTypeName);
                if (attributeBaseType is not null && attributeBaseType == designerBaseType)
                {
                    bool foundService = false;
                    ITypeResolutionService tr = (ITypeResolutionService)GetService(typeof(ITypeResolutionService));
                    if (tr is not null)
                    {
                        foundService = true;
                        designerType = tr.GetType(da.DesignerTypeName);
                    }

                    if (!foundService)
                    {
                        designerType = Type.GetType(da.DesignerTypeName);
                    }

                    if (designerType is not null)
                    {
                        break;
                    }
                }
            }
        }

        return designerType;
    }

    /// <summary>
    ///  This method determines if a UI representation for the given component should be provided.
    ///  If it returns true, then the component will get a glyph in the tray area. If it returns
    ///  false, then the component will not actually be added to the tray. The default
    ///  implementation looks for DesignTimeVisibleAttribute.Yes on the component's class.
    /// </summary>
    protected virtual bool CanDisplayComponent(IComponent component)
    {
        return TypeDescriptor.GetAttributes(component).Contains(DesignTimeVisibleAttribute.Yes);
    }

    [CLSCompliant(false)]
    public void CreateComponentFromTool(ToolboxItem tool)
    {
        if (!CanCreateComponentFromTool(tool))
        {
            return;
        }

        // We invoke the drag drop handler for this. This implementation is shared between all designers that create components.
        GetOleDragHandler().CreateTool(tool, null, 0, 0, 0, 0, false, false);
    }

    /// <summary>
    ///  Displays the given exception to the user.
    /// </summary>
    protected void DisplayError(Exception e)
    {
        IUIService uis = (IUIService)GetService(typeof(IUIService));
        if (uis is not null)
        {
            uis.ShowError(e);
        }
        else
        {
            string message = e.Message;
            if (message is null || message.Length == 0)
            {
                message = e.ToString();
            }

            RTLAwareMessageBox.Show(null, message, null, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, 0);
        }
    }

    /// <summary>
    ///  Disposes of the resources (other than memory) used by the component tray object.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing && _controls is not null)
        {
            IExtenderProviderService es = (IExtenderProviderService)GetService(typeof(IExtenderProviderService));
            es?.RemoveExtenderProvider(this);

            IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
            if (_eventHandlerService is not null)
            {
                if (host is not null)
                {
                    host.RemoveService<IEventHandlerService>();
                    _eventHandlerService = null;
                }
            }

            IComponentChangeService componentChangeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));
            if (componentChangeService is not null)
            {
                componentChangeService.ComponentRemoved -= OnComponentRemoved;
            }

            SystemEvents.DisplaySettingsChanged -= OnSystemSettingChanged;
            SystemEvents.InstalledFontsChanged -= OnSystemSettingChanged;
            SystemEvents.UserPreferenceChanged -= OnUserPreferenceChanged;
            IMenuCommandService mcs = MenuService;
            if (mcs is not null)
            {
                Debug.Assert(_menucmdArrangeIcons is not null, "Null Menu Command for ArrangeIcons");
                Debug.Assert(_menucmdLineupIcons is not null, "Null Menu Command for LineupIcons");
                Debug.Assert(_menucmdLargeIcons is not null, "Null Menu Command for LargeIcons");
                mcs.RemoveCommand(_menucmdArrangeIcons);
                mcs.RemoveCommand(_menucmdLineupIcons);
                mcs.RemoveCommand(_menucmdLargeIcons);
            }

            _selectionUISvc = null;

            if (_inheritanceUI is not null)
            {
                _inheritanceUI.Dispose();
                _inheritanceUI = null;
            }

            _serviceProvider = null;
            _controls.Clear();
            _controls = null;

            if (_glyphManager is not null)
            {
                _glyphManager.Dispose();
                _glyphManager = null;
            }
        }

        base.Dispose(disposing);
    }

    /// <summary>
    ///  Similar to GetNextControl on Control, this method returns the next
    ///  component in the tray, given a starting component. It will return
    ///  null if the end (or beginning, if forward is false) of the list
    ///  is encountered.
    /// </summary>
    public IComponent GetNextComponent(IComponent component, bool forward)
    {
        for (int i = 0; i < _controls.Count; i++)
        {
            TrayControl control = (TrayControl)_controls[i];
            if (control.Component == component)
            {
                int targetIndex = (forward ? i + 1 : i - 1);
                if (targetIndex >= 0 && targetIndex < _controls.Count)
                {
                    return ((TrayControl)_controls[targetIndex]).Component;
                }

                // Reached the end of the road.
                return null;
            }
        }

        // If we got here then the component isn't in our list. Prime the caller with either the first or the last.
        if (_controls.Count > 0)
        {
            int targetIndex = (forward ? 0 : _controls.Count - 1);
            return ((TrayControl)_controls[targetIndex]).Component;
        }

        return null;
    }

    /// <summary>
    ///  Accessor method for the location extender property. We offer this extender
    ///  to all non-visual components.
    /// </summary>
    [Category("Layout")]
    [Localizable(false)]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription("ControlLocationDescr")]
    [DesignOnly(true)]
    public Point GetLocation(IComponent receiver)
    {
        PropertyDescriptor loc = TypeDescriptor.GetProperties(receiver.GetType())["Location"];
        if (loc is not null)
        {
            // In this case the component already had a Location property,
            // and what the caller wants is the underlying components Location,
            // not the tray location. Why? Because we now use TrayLocation.
            return (Point)(loc.GetValue(receiver));
        }
        else
        {
            // If the component didn't already have a Location property,
            // then the caller really wants the tray location. Could be a 3rd party vendor.
            return GetTrayLocation(receiver);
        }
    }

    /// <summary>
    ///  Accessor method for the location extender property. We offer this extender
    ///  to all non-visual components.
    /// </summary>
    [Category("Layout")]
    [Localizable(false)]
    [Browsable(false)]
    [SRDescription("ControlLocationDescr")]
    [DesignOnly(true)]
    public Point GetTrayLocation(IComponent receiver)
    {
        Control c = TrayControl.FromComponent(receiver);
        if (c is null)
        {
            Debug.Fail("Anything we're extending should have a component view.");
            return default;
        }

        Point loc = c.Location;
        Point autoScrollLoc = AutoScrollPosition;
        return new Point(loc.X - autoScrollLoc.X, loc.Y - autoScrollLoc.Y);
    }

    /// <summary>
    ///  Gets the requested service type.
    /// </summary>
    protected override object GetService(Type serviceType)
    {
        object service = null;
        Debug.Assert(_serviceProvider is not null, "Trying to access services too late or too early.");
        if (_serviceProvider is not null)
        {
            service = _serviceProvider.GetService(serviceType);
        }

        return service;
    }

    /// <summary>
    ///  Returns true if the given component is being shown on the tray.
    /// </summary>
    public bool IsTrayComponent(IComponent comp)
    {
        if (TrayControl.FromComponent(comp) is null)
        {
            return false;
        }

        foreach (Control control in Controls)
        {
            if (control is TrayControl tc && tc.Component == comp)
            {
                return true;
            }
        }

        return false;
    }

    protected override void OnMouseDoubleClick(MouseEventArgs e)
    {
        // give our glyphs first chance at this
        if (_glyphManager is not null && _glyphManager.OnMouseDoubleClick(e))
        {
            // handled by a glyph - so don't send to the comp tray
            return;
        }

        base.OnDoubleClick(e);
        if (!TabOrderActive)
        {
            OnLostCapture();
            IEventBindingService eps = (IEventBindingService)GetService(typeof(IEventBindingService));
            eps?.ShowCode();
        }
    }

    /// <summary>
    ///  Inheriting classes should override this method to handle this event.
    ///  Call base.onGiveFeedback to send this event to any registered event listeners.
    /// </summary>
    protected override void OnGiveFeedback(GiveFeedbackEventArgs gfevent)
    {
        base.OnGiveFeedback(gfevent);
        GetOleDragHandler().DoOleGiveFeedback(gfevent);
    }

    /// <summary>
    ///  Called in response to a drag drop for OLE drag and drop. Here we
    ///  drop a toolbox component on our parent control.
    /// </summary>
    protected override void OnDragDrop(DragEventArgs de)
    {
        // This will be used once during PositionComponent to place the component at the drop point. It is automatically
        // set to null afterwards, so further components appear after the first one dropped.
        _mouseDropLocation = PointToClient(new Point(de.X, de.Y));
        _autoScrollPosBeforeDragging = AutoScrollPosition;

        if (_mouseDragTool is not null)
        {
            ToolboxItem tool = _mouseDragTool;
            _mouseDragTool = null;
            try
            {
                IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
                IDesigner designer = host.GetDesigner(host.RootComponent);
                if (designer is IToolboxUser itu)
                {
                    itu.ToolPicked(tool);
                }
                else
                {
                    CreateComponentFromTool(tool);
                }
            }
            catch (Exception e)
            {
                DisplayError(e);
                if (e.IsCriticalException())
                {
                    throw;
                }
            }

            de.Effect = DragDropEffects.Copy;
        }
        else
        {
            GetOleDragHandler().DoOleDragDrop(de);
        }

        _mouseDropLocation = InvalidPoint;
        ResumeLayout();
    }

    /// <summary>
    ///  Called in response to a drag enter for OLE drag and drop.
    /// </summary>
    protected override void OnDragEnter(DragEventArgs de)
    {
        if (!TabOrderActive)
        {
            SuspendLayout();
            _toolboxService ??= (IToolboxService)GetService(typeof(IToolboxService));

            OleDragDropHandler dragDropHandler = GetOleDragHandler();
            object[] dragComps = OleDragDropHandler.GetDraggingObjects(de);
            // Only assume the items came from the ToolBox if dragComps is null
            if (_toolboxService is not null && dragComps is null)
            {
                _mouseDragTool = _toolboxService.DeserializeToolboxItem(de.Data, (IDesignerHost)GetService(typeof(IDesignerHost)));
            }

            if (_mouseDragTool is not null)
            {
                Debug.Assert((de.AllowedEffect & (DragDropEffects.Move | DragDropEffects.Copy)) != 0, "DragDropEffect.Move | .Copy isn't allowed?");
                if ((de.AllowedEffect & DragDropEffects.Move) != 0)
                {
                    de.Effect = DragDropEffects.Move;
                }
                else
                {
                    de.Effect = DragDropEffects.Copy;
                }
            }
            else
            {
                dragDropHandler.DoOleDragEnter(de);
            }
        }
    }

    /// <summary>
    ///  Called when a drag-drop operation leaves the control designer view
    /// </summary>
    protected override void OnDragLeave(EventArgs e)
    {
        _mouseDragTool = null;
        GetOleDragHandler().DoOleDragLeave();
        ResumeLayout();
    }

    /// <summary>
    ///  Called when a drag drop object is dragged over the control designer view
    /// </summary>
    protected override void OnDragOver(DragEventArgs de)
    {
        if (_mouseDragTool is not null)
        {
            Debug.Assert((de.AllowedEffect & DragDropEffects.Copy) != 0, "DragDropEffect.Move isn't allowed?");
            de.Effect = DragDropEffects.Copy;
        }
        else
        {
            GetOleDragHandler().DoOleDragOver(de);
        }
    }

    /// <summary>
    ///  Forces the layout of any docked or anchored child controls.
    /// </summary>
    protected override void OnLayout(LayoutEventArgs levent)
    {
        DoAutoArrange(false);
        // make sure selection service redraws
        Invalidate(true);
        base.OnLayout(levent);
    }

    /// <summary>
    ///  This is called when we lose capture. Here we get rid of any
    ///  rubber band we were drawing. You should put any cleanup
    ///  code in here.
    /// </summary>
    protected virtual void OnLostCapture()
    {
        if (_mouseDragStart != InvalidPoint)
        {
            Cursor.Clip = Rectangle.Empty;
            if (_mouseDragEnd != InvalidPoint)
            {
                DrawRubber(_mouseDragStart, _mouseDragEnd);
                _mouseDragEnd = InvalidPoint;
            }

            _mouseDragStart = InvalidPoint;
        }
    }

    private void DrawRubber(Point start, Point end)
    {
        _mouseDragWorkspace.X = Math.Min(start.X, end.X);
        _mouseDragWorkspace.Y = Math.Min(start.Y, end.Y);
        _mouseDragWorkspace.Width = Math.Abs(end.X - start.X);
        _mouseDragWorkspace.Height = Math.Abs(end.Y - start.Y);
        _mouseDragWorkspace = RectangleToScreen(_mouseDragWorkspace);
        ControlPaint.DrawReversibleFrame(_mouseDragWorkspace, BackColor, FrameStyle.Dashed);
    }

    /// <summary>
    ///  Inheriting classes should override this method to handle this event.
    ///  Call base.onMouseDown to send this event to any registered event listeners.
    /// </summary>
    protected override void OnMouseDown(MouseEventArgs e)
    {
        // give our glyphs first chance at this
        if (_glyphManager is not null && _glyphManager.OnMouseDown(e))
        {
            // handled by a glyph - so don't send to the comp tray
            return;
        }

        base.OnMouseDown(e);
        if (!TabOrderActive)
        {
            _toolboxService ??= (IToolboxService)GetService(typeof(IToolboxService));

            FocusDesigner();
            if (e.Button == MouseButtons.Left && _toolboxService is not null)
            {
                ToolboxItem tool = _toolboxService.GetSelectedToolboxItem((IDesignerHost)GetService(typeof(IDesignerHost)));
                if (tool is not null)
                {
                    // mouseDropLocation is checked in PositionControl,
                    // which should get called as a result of adding a new component.
                    // This allows us to set the position without flickering,
                    // while still providing support for auto layout if the control was double clicked or
                    // added through extensibility.
                    _mouseDropLocation = new Point(e.X, e.Y);
                    try
                    {
                        CreateComponentFromTool(tool);
                        _toolboxService.SelectedToolboxItemUsed();
                    }
                    catch (Exception ex)
                    {
                        DisplayError(ex);
                        if (ex.IsCriticalException())
                        {
                            throw;
                        }
                    }

                    _mouseDropLocation = InvalidPoint;
                    return;
                }
            }

            // If it is the left button, start a rubber band drag to lasso controls.
            if (e.Button == MouseButtons.Left)
            {
                _mouseDragStart = new Point(e.X, e.Y);
                Capture = true;
                Cursor.Clip = RectangleToScreen(ClientRectangle);
            }
            else
            {
                try
                {
                    ISelectionService ss = (ISelectionService)GetService(typeof(ISelectionService));
                    ss?.SetSelectedComponents(new object[] { _mainDesigner.Component });
                }
                catch (Exception ex) when (!ex.IsCriticalException())
                {
                    // Nothing we can really do here; just eat it.
                }
            }
        }
    }

    /// <summary>
    ///  Inheriting classes should override this method to handle this event.
    ///  Call base.onMouseMove to send this event to any registered event listeners.
    /// </summary>
    protected override void OnMouseMove(MouseEventArgs e)
    {
        // give our glyphs first chance at this
        if (_glyphManager is not null && _glyphManager.OnMouseMove(e))
        {
            // handled by a glyph - so don't send to the comp tray
            return;
        }

        base.OnMouseMove(e);

        // If we are dragging, then draw our little rubber band.
        if (_mouseDragStart != InvalidPoint)
        {
            if (_mouseDragEnd != InvalidPoint)
            {
                DrawRubber(_mouseDragStart, _mouseDragEnd);
            }
            else
            {
                _mouseDragEnd = new Point(0, 0);
            }

            _mouseDragEnd.X = e.X;
            _mouseDragEnd.Y = e.Y;
            DrawRubber(_mouseDragStart, _mouseDragEnd);
        }
    }

    /// <summary>
    ///  Inheriting classes should override this method to handle this event.
    ///  Call base.onMouseUp to send this event to any registered event listeners.
    /// </summary>
    protected override void OnMouseUp(MouseEventArgs e)
    {
        // give our glyphs first chance at this
        if (_glyphManager is not null && _glyphManager.OnMouseUp(e))
        {
            // handled by a glyph - so don't send to the comp tray
            return;
        }

        if (_mouseDragStart != InvalidPoint && e.Button == MouseButtons.Left)
        {
            IComponent[] comps;
            Capture = false;
            Cursor.Clip = Rectangle.Empty;
            if (_mouseDragEnd != InvalidPoint)
            {
                DrawRubber(_mouseDragStart, _mouseDragEnd);
                Rectangle rect = new Rectangle
                {
                    X = Math.Min(_mouseDragStart.X, e.X),
                    Y = Math.Min(_mouseDragStart.Y, e.Y),
                    Width = Math.Abs(e.X - _mouseDragStart.X),
                    Height = Math.Abs(e.Y - _mouseDragStart.Y)
                };
                comps = GetComponentsInRect(rect);
                _mouseDragEnd = InvalidPoint;
            }
            else
            {
                comps = [];
            }

            if (comps.Length == 0)
            {
                comps = [_mainDesigner.Component];
            }

            try
            {
                ISelectionService ss = (ISelectionService)GetService(typeof(ISelectionService));
                ss?.SetSelectedComponents(comps);
            }
            catch (Exception ex) when (!ex.IsCriticalException())
            {
                // Nothing we can really do here; just eat it.
            }

            _mouseDragStart = InvalidPoint;
        }

        base.OnMouseUp(e);
    }

    private IComponent[] GetComponentsInRect(Rectangle rect)
    {
        List<IComponent> list = [];
        int controlCount = Controls.Count;
        for (int i = 0; i < controlCount; i++)
        {
            Control child = Controls[i];
            Rectangle bounds = child.Bounds;
            if (child is TrayControl tc && bounds.IntersectsWith(rect))
            {
                list.Add(tc.Component);
            }
        }

        return [.. list];
    }

    protected override void OnPaint(PaintEventArgs pe)
    {
        if (_fResetAmbient || _fSelectionChanged)
        {
            _fResetAmbient = false;
            _fSelectionChanged = false;
            IUIService uiService = (IUIService)GetService(typeof(IUIService));
            if (uiService is not null)
            {
                Color styleColor;
                if (uiService.Styles["ArtboardBackground"] is Color backgroundColor)
                {
                    styleColor = backgroundColor;
                }

                // Can't use 'as' here since Color is a value type
                else if (uiService.Styles["VsColorDesignerTray"] is Color trayColor)
                {
                    styleColor = trayColor;
                }
                else if (uiService.Styles["HighlightColor"] is Color highlightColor)
                {
                    // Since v1, we have had code here that checks for HighlightColor,
                    // so some hosts (like WinRes) have been setting it.
                    // If VsColorDesignerTray isn't present, we look for HighlightColor for backward compatibility.
                    styleColor = highlightColor;
                }
                else
                {
                    // No style color provided? Let's pick a default.
                    styleColor = SystemColors.Info;
                }

                BackColor = styleColor;
                Font = (Font)uiService.Styles["DialogFont"];
                foreach (Control ctl in _controls)
                {
                    ctl.BackColor = styleColor;
                    ctl.ForeColor = ForeColor;
                }
            }
        }

        base.OnPaint(pe);
        Graphics gr = pe.Graphics;
        // Now, if we have a selection, paint it
        if (_selectedObjects is not null)
        {
            bool first = true; // indicates the first iteration of our foreach loop
            HatchBrush selectionBorderBrush;
            if (SystemInformation.HighContrast)
            {
                selectionBorderBrush = new HatchBrush(HatchStyle.Percent50, SystemColors.HighlightText, Color.Transparent);
            }
            else
            {
                selectionBorderBrush = new HatchBrush(HatchStyle.Percent50, SystemColors.ControlDarkDark, Color.Transparent);
            }

            try
            {
                foreach (object o in _selectedObjects)
                {
                    Control c = ((IOleDragClient)this).GetControlForComponent(o);
                    if (c is not null && c.Visible)
                    {
                        Rectangle innerRect = c.Bounds;
                        if (SystemInformation.HighContrast)
                        {
                            c.ForeColor = SystemColors.HighlightText;
                            c.BackColor = SystemColors.Highlight;
                        }

                        NoResizeHandleGlyph glyph = new(innerRect, SelectionRules.None, first, null);
                        gr.FillRectangle(selectionBorderBrush, DesignerUtils.GetBoundsForNoResizeSelectionType(innerRect, SelectionBorderGlyphType.Top));
                        gr.FillRectangle(selectionBorderBrush, DesignerUtils.GetBoundsForNoResizeSelectionType(innerRect, SelectionBorderGlyphType.Bottom));
                        gr.FillRectangle(selectionBorderBrush, DesignerUtils.GetBoundsForNoResizeSelectionType(innerRect, SelectionBorderGlyphType.Left));
                        gr.FillRectangle(selectionBorderBrush, DesignerUtils.GetBoundsForNoResizeSelectionType(innerRect, SelectionBorderGlyphType.Right));
                        // Need to draw this one last
                        DesignerUtils.DrawNoResizeHandle(gr, glyph.Bounds, first);
                    }

                    first = false;
                }
            }
            finally
            {
                selectionBorderBrush?.Dispose();
            }
        }

        // paint any glyphs
        _glyphManager?.OnPaintGlyphs(pe);
    }

    /// <summary>
    ///  Sets the cursor. You may override this to set your own
    ///  cursor.
    /// </summary>
    protected virtual void OnSetCursor()
    {
        _toolboxService ??= (IToolboxService)GetService(typeof(IToolboxService));

        if (_toolboxService is null || !_toolboxService.SetCursor())
        {
            Cursor.Current = Cursors.Default;
        }
    }

    /// <summary>
    ///  Removes a component from the tray.
    /// </summary>
    public virtual void RemoveComponent(IComponent component)
    {
        TrayControl c = TrayControl.FromComponent(component);
        if (c is not null)
        {
            try
            {
                InheritanceAttribute attr = c.InheritanceAttribute;
                if (attr.InheritanceLevel != InheritanceLevel.NotInherited && _inheritanceUI is not null)
                {
                    _inheritanceUI.RemoveInheritedControl(c);
                }

                _controls?.Remove(c);
            }
            finally
            {
                c.Dispose();
            }
        }
    }

    /// <summary>
    ///  Accessor method for the location extender property. We offer this extender
    ///  to all non-visual components.
    /// </summary>
    public void SetLocation(IComponent receiver, Point location)
    {
        // This really should only be called when we are loading.
        IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
        if (host is not null && host.Loading)
        {
            // If we are loading, and we get called here,
            // that's because we have provided the extended Location property.
            // In this case we are loading an old project,
            // and what we are really setting is the tray location.
            SetTrayLocation(receiver, location);
        }
        else
        {
            // we are not loading
            PropertyDescriptor loc = TypeDescriptor.GetProperties(receiver.GetType())["Location"];
            if (loc is not null)
            {
                // so if the component already had the Location property,
                // what the caller wants is really the underlying component's Location property.
                loc.SetValue(receiver, location);
            }
            else
            {
                // if the component didn't have a Location property, then the caller really wanted the tray location.
                SetTrayLocation(receiver, location);
            }
        }
    }

    /// <summary>
    ///  Accessor method for the location extender property. We offer this extender
    ///  to all non-visual components.
    /// </summary>
    public void SetTrayLocation(IComponent receiver, Point location)
    {
        TrayControl c = TrayControl.FromComponent(receiver);
        if (c is null)
        {
            Debug.Fail("Anything we're extending should have a component view.");
            return;
        }

        if (c.Parent == this)
        {
            Point autoScrollLoc = AutoScrollPosition;
            location = new Point(location.X + autoScrollLoc.X, location.Y + autoScrollLoc.Y);
            if (c.Visible)
            {
                RearrangeInAutoSlots(c, location);
            }
        }
        else if (!c.Location.Equals(location))
        {
            c.Location = location;
            c.Positioned = true;
        }
    }

    /// <summary>
    ///  We override our base class's WndProc to monitor certain messages.
    /// </summary>
    protected override void WndProc(ref Message m)
    {
        switch (m.MsgInternal)
        {
            case PInvokeCore.WM_CANCELMODE:
                // When we get cancelmode (i.e. you tabbed away to another window) then we want to cancel
                // any pending drag operation.
                OnLostCapture();
                break;
            case PInvokeCore.WM_SETCURSOR:
                OnSetCursor();
                return;
            case PInvokeCore.WM_HSCROLL:
            case PInvokeCore.WM_VSCROLL:
                // When we scroll, we reposition a control without causing a property change event.
                // Therefore, we must tell the selection UI service to sync itself.
                base.WndProc(ref m);
                _selectionUISvc?.SyncSelection();

                return;
            case PInvokeCore.WM_STYLECHANGED:
                // When the scroll bars first appear, we need to invalidate so we properly paint our grid.
                Invalidate();
                break;
            case PInvokeCore.WM_CONTEXTMENU:
                {
                    // Pop a context menu for the composition designer.
                    Point location = PARAM.ToPoint(m.LParamInternal);
                    if (location.X == -1 && location.Y == -1)
                    {
                        // For shift-F10.
                        location = MousePosition;
                    }

                    OnContextMenu(location);
                    break;
                }

            case PInvokeCore.WM_NCHITTEST:
                {
                    if (_glyphManager is not null)
                    {
                        // Get a hit test on any glyphs that we are managing this way.
                        // We know where to route appropriate messages.
                        Point location = PARAM.ToPoint(m.LParamInternal);
                        location.Offset(PointToClient(default));
                        _glyphManager.GetHitTest(location);
                    }

                    base.WndProc(ref m);
                    break;
                }

            default:
                base.WndProc(ref m);
                break;
        }
    }

    internal static TrayControl GetTrayControlFromComponent(IComponent comp)
    {
        return TrayControl.FromComponent(comp);
    }

    private bool TabOrderActive
    {
        get
        {
            if (!_queriedTabOrder)
            {
                _queriedTabOrder = true;
                IMenuCommandService mcs = MenuService;
                if (mcs is not null)
                {
                    _tabOrderCommand = mcs.FindCommand(StandardCommands.TabOrder);
                }
            }

            return _tabOrderCommand is not null && _tabOrderCommand.Checked;
        }
    }

    private InheritanceUI InheritanceUI
    {
        get
        {
            _inheritanceUI ??= new InheritanceUI();

            return _inheritanceUI;
        }
    }

    private IMenuCommandService MenuService
    {
        get
        {
            _menuCommandService ??= (IMenuCommandService)GetService(typeof(IMenuCommandService));

            return _menuCommandService;
        }
    }

    internal void FocusDesigner()
    {
        IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
        if (host is not null && host.RootComponent is not null)
        {
            if (host.GetDesigner(host.RootComponent) is IRootDesigner rd)
            {
                ViewTechnology[] techs = rd.SupportedTechnologies;
                if (techs.Length > 0)
                {
                    if (rd.GetView(techs[0]) is Control view)
                    {
                        view.Focus();
                    }
                }
            }
        }
    }

    internal Size ParentGridSize
    {
        get
        {
            if (_mainDesigner is ParentControlDesigner designer)
            {
                return designer.ParentGridSize;
            }

            return new Size(8, 8);
        }
    }

    internal void UpdatePastePositions(List<Control> components)
    {
        foreach (TrayControl c in components)
        {
            if (!CanDisplayComponent(c.Component))
            {
                return;
            }

            if (_mouseDropLocation == InvalidPoint)
            {
                Control prevCtl = null;
                if (_controls.Count > 1)
                {
                    prevCtl = _controls[^1];
                }

                PositionInNextAutoSlot(c, prevCtl, true);
            }
            else
            {
                PositionControl(c);
            }

            c.BringToFront();
        }
    }

    private void PositionControl(TrayControl c)
    {
        Debug.Assert(c.Visible, $"TrayControl for {c.Component} should not be positioned");
        if (!_autoArrange)
        {
            if (_mouseDropLocation != InvalidPoint)
            {
                if (!c.Location.Equals(_mouseDropLocation))
                {
                    c.Location = _mouseDropLocation;
                }
            }
            else
            {
                Control prevCtl = null;
                if (_controls.Count > 1)
                {
                    // PositionControl can be called when all the controls have been added
                    // (from IOleDragClient.AddComponent), so we can't use the old way of
                    // looking up the previous control (prevCtl = controls[controls.Count - 2]
                    int index = _controls.IndexOf(c);
                    Debug.Assert(index >= 1, "Got the wrong index, how could that be?");
                    if (index >= 1)
                    {
                        prevCtl = _controls[index - 1];
                    }
                }

                PositionInNextAutoSlot(c, prevCtl, true);
            }
        }
        else
        {
            if (_mouseDropLocation != InvalidPoint)
            {
                RearrangeInAutoSlots(c, _mouseDropLocation);
            }
            else
            {
                Control prevCtl = null;
                if (_controls.Count > 1)
                {
                    int index = _controls.IndexOf(c);
                    Debug.Assert(index >= 1, "Got the wrong index, how could that be?");
                    if (index >= 1)
                    {
                        prevCtl = _controls[index - 1];
                    }
                }

                PositionInNextAutoSlot(c, prevCtl, true);
            }
        }
    }

    internal void RearrangeInAutoSlots(Control c, Point pos)
    {
        Debug.Assert(_controls.IndexOf(c) != -1, "Add control to the list of controls before autoarranging.!!!");
        Debug.Assert(Visible == c.Visible, $"TrayControl for {((TrayControl)c).Component} should not be positioned");

        TrayControl tc = (TrayControl)c;
        tc.Positioned = true;
        tc.Location = pos;
    }

    private bool PositionInNextAutoSlot(TrayControl c, Control prevCtl, bool dirtyDesigner)
    {
        Debug.Assert(c.Visible, $"TrayControl for {c.Component} should not be positioned");
        if (_whiteSpace.IsEmpty)
        {
            Debug.Assert(_selectionUISvc is not null, "No SelectionUIService available for tray.");
            _whiteSpace = new Point(_selectionUISvc.GetAdornmentDimensions(AdornmentType.GrabHandle));
            _whiteSpace.X = _whiteSpace.X * 2 + 3;
            _whiteSpace.Y = _whiteSpace.Y * 2 + 3;
        }

        if (prevCtl is null)
        {
            Rectangle display = DisplayRectangle;
            Point newLoc = new(display.X + _whiteSpace.X, display.Y + _whiteSpace.Y);
            if (!c.Location.Equals(newLoc))
            {
                c.Location = newLoc;
                if (dirtyDesigner)
                {
                    IComponent comp = c.Component;
                    Debug.Assert(comp is not null, "Component for the TrayControl is null");
                    PropertyDescriptor ctlLocation = TypeDescriptor.GetProperties(comp)["TrayLocation"];
                    if (ctlLocation is not null)
                    {
                        Point autoScrollLoc = AutoScrollPosition;
                        newLoc = new Point(newLoc.X - autoScrollLoc.X, newLoc.Y - autoScrollLoc.Y);
                        ctlLocation.SetValue(comp, newLoc);
                    }
                }
                else
                {
                    c.Location = newLoc;
                }

                return true;
            }
        }
        else
        {
            // Calculate the next location for this control.
            Rectangle bounds = prevCtl.Bounds;
            Point newLoc = new(bounds.X + bounds.Width + _whiteSpace.X, bounds.Y);

            // Check to see if it goes over the edge of our window. If it does, then wrap it.
            if (newLoc.X + c.Size.Width > Size.Width)
            {
                newLoc.X = _whiteSpace.X;
                newLoc.Y += bounds.Height + _whiteSpace.Y;
            }

            if (!c.Location.Equals(newLoc))
            {
                if (dirtyDesigner)
                {
                    IComponent comp = c.Component;
                    Debug.Assert(comp is not null, "Component for the TrayControl is null");
                    PropertyDescriptor ctlLocation = TypeDescriptor.GetProperties(comp)["TrayLocation"];
                    if (ctlLocation is not null)
                    {
                        Point autoScrollLoc = AutoScrollPosition;
                        newLoc = new Point(newLoc.X - autoScrollLoc.X, newLoc.Y - autoScrollLoc.Y);
                        ctlLocation.SetValue(comp, newLoc);
                    }
                }
                else
                {
                    c.Location = newLoc;
                }

                return true;
            }
        }

        return false;
    }

    internal class TrayControl : Control
    {
        // Values that define this tray control
        private readonly IComponent _component; // the component this control is representing
        private Image _toolboxBitmap; // the bitmap used to represent the component
        private int _cxIcon; // the dimensions of the bitmap
        private int _cyIcon; // the dimensions of the bitmap
        private readonly InheritanceAttribute _inheritanceAttribute;

        // Services that we use often enough to cache.
        private readonly ComponentTray _tray;
        // transient values that are used during mouse drags
        private Point _mouseDragLast = InvalidPoint; // the last position of the mouse during a drag.
        private bool _mouseDragMoved; // has the mouse been moved during this drag?
        private bool _ctrlSelect; // was the ctrl key down on the mouse down?
        private bool _positioned; // Have we given this control an explicit location yet?
        private const int WhiteSpace = 5;
        private readonly int _borderWidth;
        internal bool _fRecompute; // This flag tells the TrayControl that it needs to retrieve the font and the background color before painting.

        /// <summary>
        ///  Creates a new TrayControl based on the component.
        /// </summary>
        public TrayControl(ComponentTray tray, IComponent component)
        {
            _tray = tray;
            _component = component;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.Selectable, false);
            _borderWidth = SystemInformation.BorderSize.Width;
            UpdateIconInfo();

            IComponentChangeService cs = (IComponentChangeService)tray.GetService(typeof(IComponentChangeService));
            if (cs is not null)
            {
                cs.ComponentRename += OnComponentRename;
            }

            ISite site = component.Site;
            string name = null;

            if (site is not null)
            {
                name = site.Name;
                IDictionaryService ds = (IDictionaryService)site.GetService(typeof(IDictionaryService));
                Debug.Assert(ds is not null, "ComponentTray relies on IDictionaryService, which is not available.");
                ds?.SetValue(GetType(), this);
            }

            // We always want name to have something in it, so we default to the class name.
            // This way the design instance contains something semi-intuitive if we don't have a site.
            name ??= component.GetType().Name;

            Text = name;
            _inheritanceAttribute = (InheritanceAttribute)TypeDescriptor.GetAttributes(component)[typeof(InheritanceAttribute)];
            TabStop = false;
        }

        /// <summary>
        ///  Retrieves the component this control is representing.
        /// </summary>
        public IComponent Component
        {
            get => _component;
        }

        public override Font Font
        {
            get => _tray.Font;
        }

        public InheritanceAttribute InheritanceAttribute
        {
            get => _inheritanceAttribute;
        }

        public bool Positioned
        {
            get => _positioned;
            set => _positioned = value;
        }

        /// <summary>
        ///  Adjusts the size of the control based on the contents.
        /// </summary>
        // CONSIDER: this method gets called three or four times per component,
        // and is even reentrant (CreateGraphics can force handle creation, and OnCreateHandle calls this method).
        // There's probably a better way to do this, but since this doesn't seem to be on the critical path,
        // I'm not going to lose sleep over it.
        private void AdjustSize()
        {
            // CONSIDER: this forces handle creation. Can we delay this calculation?
            Graphics gr = CreateGraphics();
            try
            {
                Size sz = Size.Ceiling(gr.MeasureString(Text, Font));

                Rectangle rc = Bounds;

                if (_tray.ShowLargeIcons)
                {
                    rc.Width = Math.Max(_cxIcon, sz.Width) + 4 * _borderWidth + 2 * WhiteSpace;
                    rc.Height = _cyIcon + 2 * WhiteSpace + sz.Height + 4 * _borderWidth;
                }
                else
                {
                    rc.Width = _cxIcon + sz.Width + 4 * _borderWidth + 2 * WhiteSpace;
                    rc.Height = Math.Max(_cyIcon, sz.Height) + 4 * _borderWidth;
                }

                Bounds = rc;
                Invalidate();
            }

            finally
            {
                gr?.Dispose();
            }

            _tray._glyphManager?.UpdateLocation(this);
        }

        protected override AccessibleObject CreateAccessibilityInstance() => new TrayControlAccessibleObject(this, _tray);

        /// <summary>
        ///  Destroys this control. Views automatically destroy themselves when they are removed from the design container.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ISite site = _component.Site;
                if (site is not null)
                {
                    if (site.TryGetService(out IComponentChangeService cs))
                    {
                        cs.ComponentRename -= OnComponentRename;
                    }

                    if (site.TryGetService(out IDictionaryService ds))
                    {
                        ds.SetValue(typeof(TrayControl), null);
                    }
                }
            }

            base.Dispose(disposing);
        }

        /// <summary>
        ///  Retrieves the tray control object for the given component.
        /// </summary>
        public static TrayControl FromComponent(IComponent component)
        {
            if (component is null)
            {
                return null;
            }

            if (component.Site.TryGetService(out IDictionaryService ds))
            {
                return (TrayControl)ds.GetValue(typeof(TrayControl));
            }

            return null;
        }

        /// <summary>
        ///  Delegate that is called in response to a name change.
        ///  Here we update our own stashed version of the name, recalculate our size and repaint.
        /// </summary>
        private void OnComponentRename(object sender, ComponentRenameEventArgs e)
        {
            if (e.Component == _component)
            {
                Text = e.NewName;
                AdjustSize();
            }
        }

        /// <summary>
        ///  Overrides handle creation notification for a control. Here we just ensure that we're the proper size.
        /// </summary>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            AdjustSize();
        }

        /// <summary>
        ///  Called in response to a double-click of the left mouse button. The default behavior here calls onDoubleClick on IMouseHandler
        /// </summary>
        protected override void OnDoubleClick(EventArgs e)
        {
            base.OnDoubleClick(e);
            if (!_tray.TabOrderActive)
            {
                IDesignerHost host = (IDesignerHost)_tray.GetService(typeof(IDesignerHost));
                Debug.Assert(host is not null, "Component tray does not have access to designer host.");
                if (host is not null)
                {
                    _mouseDragLast = InvalidPoint;
                    Capture = false;
                    // We try to get a designer for the component and let it view the event. If this fails, then we'll try to do it ourselves.
                    IDesigner designer = host.GetDesigner(_component);
                    if (designer is null)
                    {
                        ViewDefaultEvent(_component);
                    }
                    else
                    {
                        designer.DoDefaultAction();
                    }
                }
            }
        }

        /// <summary>
        ///  Terminates our drag operation.
        /// </summary>
        private void OnEndDrag(bool cancel)
        {
            _mouseDragLast = InvalidPoint;
            if (!_mouseDragMoved)
            {
                if (_ctrlSelect)
                {
                    ISelectionService sel = (ISelectionService)_tray.GetService(typeof(ISelectionService));
                    sel?.SetSelectedComponents(new object[] { Component }, SelectionTypes.Primary);

                    _ctrlSelect = false;
                }

                return;
            }

            _mouseDragMoved = false;
            _ctrlSelect = false;
            Capture = false;
            OnSetCursor();

            // And now finish the drag.
            Debug.Assert(_tray._selectionUISvc is not null, "We shouldn't be able to begin a drag without this");
            if (_tray._selectionUISvc is not null && _tray._selectionUISvc.Dragging)
            {
                _tray._selectionUISvc.EndDrag(cancel);
            }
        }

        /// <summary>
        ///  Called when the mouse button is pressed down. Here, we provide drag support for the component.
        /// </summary>
        protected override void OnMouseDown(MouseEventArgs me)
        {
            base.OnMouseDown(me);
            if (_tray.TabOrderActive)
            {
                return;
            }

            _tray.FocusDesigner();

            // If this is the left mouse button, then begin a drag.
            if (me.Button == MouseButtons.Left)
            {
                Capture = true;
                _mouseDragLast = PointToScreen(new Point(me.X, me.Y));

                // If the CTRL key isn't down, select this component, otherwise, we wait until the mouse up.
                // Make sure the component is selected.
                _ctrlSelect = PInvoke.GetKeyState((int)Keys.ControlKey) != 0;
                if (!_ctrlSelect)
                {
                    ISelectionService sel = (ISelectionService)_tray.GetService(typeof(ISelectionService));
                    // Make sure the component is selected
                    sel?.SetSelectedComponents(new object[] { Component }, SelectionTypes.Primary);
                }
            }
        }

        /// <summary>
        ///  Called when the mouse is moved over the component.
        ///  We update our drag information here if we're dragging the component around.
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs me)
        {
            base.OnMouseMove(me);
            if (_mouseDragLast == InvalidPoint)
            {
                return;
            }

            if (!_mouseDragMoved)
            {
                Size minDrag = SystemInformation.DragSize;
                Size minDblClick = SystemInformation.DoubleClickSize;
                minDrag.Width = Math.Max(minDrag.Width, minDblClick.Width);
                minDrag.Height = Math.Max(minDrag.Height, minDblClick.Height);
                // we have to make sure the mouse moved farther than the minimum drag distance before we actually start the drag
                Point newPt = PointToScreen(new Point(me.X, me.Y));
                if (_mouseDragLast == InvalidPoint ||
                    (Math.Abs(_mouseDragLast.X - newPt.X) < minDrag.Width &&
                     Math.Abs(_mouseDragLast.Y - newPt.Y) < minDrag.Height))
                {
                    return;
                }
                else
                {
                    _mouseDragMoved = true;
                    // we're on the move, so we're not in a ctrlSelect
                    _ctrlSelect = false;
                }
            }

            try
            {
                // Make sure the component is selected
                ISelectionService sel = (ISelectionService)_tray.GetService(typeof(ISelectionService));
                sel?.SetSelectedComponents(new object[] { Component }, SelectionTypes.Primary);

                // Notify the selection service that all the components are in the "mouse down" mode.
                if (_tray._selectionUISvc is not null && _tray._selectionUISvc.BeginDrag(SelectionRules.Visible | SelectionRules.Moveable, _mouseDragLast.X, _mouseDragLast.Y))
                {
                    OnSetCursor();
                }
            }
            finally
            {
                _mouseDragMoved = false;
                _mouseDragLast = InvalidPoint;
            }
        }

        /// <summary>
        ///  Called when the mouse button is released. Here, we finish our drag if one was started.
        /// </summary>
        protected override void OnMouseUp(MouseEventArgs me)
        {
            base.OnMouseUp(me);
            OnEndDrag(false);
        }

        /// <summary>
        ///  Called when we are to display our context menu for this component.
        /// </summary>
        private void OnContextMenu(Point location)
        {
            if (!_tray.TabOrderActive)
            {
                Capture = false;
                // Ensure that this component is selected.
                ISelectionService s = (ISelectionService)_tray.GetService(typeof(ISelectionService));
                if (s is not null && !s.GetComponentSelected(_component))
                {
                    s.SetSelectedComponents(new object[] { _component }, SelectionTypes.Replace);
                }

                IMenuCommandService mcs = _tray.MenuService;
                if (mcs is not null)
                {
                    Capture = false;
                    Cursor.Clip = Rectangle.Empty;
                    mcs.ShowContextMenu(MenuCommands.TraySelectionMenu, location.X, location.Y);
                }
            }
        }

        /// <summary>
        ///  Painting for our control.
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (_fRecompute)
            {
                _fRecompute = false;
                UpdateIconInfo();
            }

            base.OnPaint(e);
            Rectangle rc = ClientRectangle;
            rc.X += WhiteSpace + _borderWidth;
            rc.Y += _borderWidth;
            rc.Width -= (2 * _borderWidth + WhiteSpace);
            rc.Height -= 2 * _borderWidth;
            StringFormat format = new();
            Brush foreBrush = new SolidBrush(ForeColor);
            try
            {
                format.Alignment = StringAlignment.Center;
                if (_tray.ShowLargeIcons)
                {
                    if (_toolboxBitmap is not null)
                    {
                        int x = rc.X + (rc.Width - _cxIcon) / 2;
                        int y = rc.Y + WhiteSpace;
                        e.Graphics.DrawImage(_toolboxBitmap, new Rectangle(x, y, _cxIcon, _cyIcon));
                    }

                    rc.Y += (_cyIcon + WhiteSpace);
                    rc.Height -= _cyIcon;
                    e.Graphics.DrawString(Text, Font, foreBrush, rc, format);
                }
                else
                {
                    if (_toolboxBitmap is not null)
                    {
                        int y = rc.Y + (rc.Height - _cyIcon) / 2;
                        e.Graphics.DrawImage(_toolboxBitmap, new Rectangle(rc.X, y, _cxIcon, _cyIcon));
                    }

                    rc.X += (_cxIcon + _borderWidth);
                    rc.Width -= _cxIcon;
                    rc.Y += 3;
                    e.Graphics.DrawString(Text, Font, foreBrush, rc);
                }
            }

            finally
            {
                format?.Dispose();

                foreBrush?.Dispose();
            }

            // If this component is being inherited, paint it as such
            if (!InheritanceAttribute.NotInherited.Equals(_inheritanceAttribute))
            {
                InheritanceUI iui = _tray.InheritanceUI;
                if (iui is not null)
                {
                    e.Graphics.DrawImage(InheritanceUI.InheritanceGlyph, 0, 0);
                }
            }
        }

        /// <summary>
        ///  Overrides control's FontChanged. Here we re-adjust our size if the font changes.
        /// </summary>
        protected override void OnFontChanged(EventArgs e)
        {
            AdjustSize();
            base.OnFontChanged(e);
        }

        /// <summary>
        ///  Overrides control's LocationChanged. Here, we make sure that any glyphs associated with us are also relocated.
        /// </summary>
        protected override void OnLocationChanged(EventArgs e)
        {
            _tray._glyphManager?.UpdateLocation(this);
        }

        /// <summary>
        ///  Overrides control's TextChanged. Here we re-adjust our size if the font changes.
        /// </summary>
        protected override void OnTextChanged(EventArgs e)
        {
            AdjustSize();
            base.OnTextChanged(e);
        }

        /// <summary>
        ///  Called each time the cursor needs to be set.
        ///  The ControlDesigner behavior here will set the cursor to one of three things:
        ///  1. If the selection UI service shows a locked selection, or if there is no location property
        ///     on the control, then the default arrow will be set.
        ///  2. Otherwise, the four headed arrow will be set to indicate that the component can be clicked and moved.
        ///  3. If the user is currently dragging a component, the crosshair cursor will be used instead
        ///     of the four headed arrow.
        /// </summary>
        private void OnSetCursor()
        {
            // Check that the component is not locked.
            PropertyDescriptor prop;
            try
            {
                prop = TypeDescriptor.GetProperties(_component)["Locked"];
            }
            catch (FileNotFoundException e)
            {
                // In case an unhandled exception was encountered,
                // we don't want to leave the cursor with some strange shape.
                // Currently we have watson logs with FileNotFoundException only,
                // so we are scoping the catch only to that type.
                Cursor.Current = Cursors.Default;
                Debug.Fail(e.Message);
                return;
            }

            if (prop is not null && ((bool)prop.GetValue(_component)))
            {
                Cursor.Current = Cursors.Default;
                return;
            }

            // Ask the tray to see if the tab order UI is not running.
            if (_tray.TabOrderActive)
            {
                Cursor.Current = Cursors.Default;
                return;
            }

            if (_mouseDragMoved)
            {
                Cursor.Current = Cursors.Default;
            }
            else if (_mouseDragLast != InvalidPoint)
            {
                Cursor.Current = Cursors.Cross;
            }
            else
            {
                Cursor.Current = Cursors.SizeAll;
            }
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            if (!_tray.AutoArrange ||
                (specified & BoundsSpecified.Width) == BoundsSpecified.Width ||
                (specified & BoundsSpecified.Height) == BoundsSpecified.Height)
            {
                base.SetBoundsCore(x, y, width, height, specified);
            }

            Rectangle bounds = Bounds;
            Size parentGridSize = _tray.ParentGridSize;
            if (Math.Abs(bounds.X - x) > parentGridSize.Width || Math.Abs(bounds.Y - y) > parentGridSize.Height)
            {
                base.SetBoundsCore(x, y, width, height, specified);
            }
        }

        protected override void SetVisibleCore(bool value)
        {
            if (value && !_tray.CanDisplayComponent(_component))
            {
                return;
            }

            base.SetVisibleCore(value);
        }

        public override string ToString() => $"ComponentTray: {_component}";

        internal void UpdateIconInfo()
        {
            ToolboxBitmapAttribute attr = (ToolboxBitmapAttribute)TypeDescriptor.GetAttributes(_component)[typeof(ToolboxBitmapAttribute)];
            if (attr is not null)
            {
                _toolboxBitmap = attr.GetImage(_component, _tray.ShowLargeIcons);
            }

            // Get the size of the bitmap so we can size our component correctly.
            if (_toolboxBitmap is null)
            {
                _cxIcon = 0;
                _cyIcon = SystemInformation.IconSize.Height;
            }
            else
            {
                Size sz = _toolboxBitmap.Size;
                _cxIcon = sz.Width;
                _cyIcon = sz.Height;
            }

            AdjustSize();
        }

        /// <summary>
        ///  This creates a method signature in the source code file for the default event
        ///  on the component and navigates the user's cursor to that location.
        /// </summary>
        public virtual void ViewDefaultEvent(IComponent component)
        {
            EventDescriptor defaultEvent = TypeDescriptor.GetDefaultEvent(component);
            PropertyDescriptor defaultPropEvent = null;
            bool eventChanged = false;
            IEventBindingService eps = (IEventBindingService)GetService(typeof(IEventBindingService));
            if (eps is not null)
            {
                defaultPropEvent = eps.GetEventProperty(defaultEvent);
            }

            // If we couldn't find a property for this event, or if the property is read only, then abort and just show the code.
            if (defaultPropEvent is null || defaultPropEvent.IsReadOnly)
            {
                eps?.ShowCode();
                return;
            }

            string handler = (string)defaultPropEvent.GetValue(component);

            // If there is no handler set, set one now.
            if (handler is null)
            {
                eventChanged = true;
                handler = eps.CreateUniqueMethodName(component, defaultEvent);
            }

            IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
            DesignerTransaction trans = null;

            try
            {
                if (host is not null)
                {
                    trans = host.CreateTransaction(string.Format(SR.WindowsFormsAddEvent, defaultEvent.Name));
                }

                // Save the new value... BEFORE navigating to it!
                if (eventChanged && defaultPropEvent is not null)
                {
                    defaultPropEvent.SetValue(component, handler);
                }

                eps.ShowCode(component, defaultEvent);
            }
            finally
            {
                trans?.Commit();
            }
        }

        /// <summary>
        ///  This method should be called by the extending designer for each message the control would normally receive.
        ///  This allows the designer to pre-process messages before allowing them to be routed to the control.
        /// </summary>
        protected override void WndProc(ref Message m)
        {
            switch (m.MsgInternal)
            {
                case PInvokeCore.WM_SETCURSOR:
                    // We always handle setting the cursor ourselves.
                    OnSetCursor();
                    break;
                case PInvokeCore.WM_CONTEXTMENU:
                    // We must handle this ourselves. Control only allows regular Windows Forms context menus, which
                    // doesn't do us much good. Also, control's button up processing calls DefwndProc first, which
                    // causes a right mouse up to be routed as a WM_CONTEXTMENU. If we don't respond to it here,
                    // this message will be bubbled up to our parent, which would pop up a container context menu
                    // instead of our own.

                    Point location = PARAM.ToPoint(m.LParamInternal);
                    if (location.X == -1 && location.Y == -1)
                    {
                        // for shift-F10
                        location = MousePosition;
                    }

                    OnContextMenu(location);
                    break;
                case PInvokeCore.WM_NCHITTEST:
                    if (_tray._glyphManager is not null)
                    {
                        // Make sure that we send our glyphs hit test messages over the TrayControls too.
                        Point pt = PARAM.ToPoint(m.LParamInternal);
                        Point pt1 = PointToClient(default);
                        pt.Offset(pt1.X, pt1.Y);

                        // Offset the location of the traycontrol so we're in component tray coordinates.
                        pt.Offset(Location.X, Location.Y);
                        _tray._glyphManager.GetHitTest(pt);
                    }

                    base.WndProc(ref m);
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        private class TrayControlAccessibleObject : ControlAccessibleObject
        {
            private readonly ComponentTray _tray;
            public TrayControlAccessibleObject(TrayControl owner, ComponentTray tray) : base(owner)
            {
                _tray = tray;
            }

            private IComponent Component
            {
                get => ((TrayControl)Owner).Component;
            }

            public override AccessibleStates State
            {
                get
                {
                    AccessibleStates state = base.State;
                    ISelectionService s = (ISelectionService)_tray.GetService(typeof(ISelectionService));
                    if (s is not null)
                    {
                        if (s.GetComponentSelected(Component))
                        {
                            state |= AccessibleStates.Selected;
                        }

                        if (s.PrimarySelection == Component)
                        {
                            state |= AccessibleStates.Focused;
                        }
                    }

                    return state;
                }
            }
        }
    }

    private class ComponentTrayGlyphManager
    {
        private Adorner _traySelectionAdorner; // we'll use a single adorner to manage the glyphs
        private Glyph _hitTestedGlyph; // the last glyph we hit tested (can be null)
        private readonly BehaviorService _behaviorSvc;

        /// <summary>
        ///  Constructor that simply creates an empty adorner.
        /// </summary>
        public ComponentTrayGlyphManager(BehaviorService behaviorSvc)
        {
            _behaviorSvc = behaviorSvc;
            _traySelectionAdorner = new Adorner();
        }

        /// <summary>
        ///  This is how we publically expose our glyph collection so that other designer services can 'add value'.
        /// </summary>
        public GlyphCollection SelectionGlyphs
        {
            get => _traySelectionAdorner.Glyphs;
        }

        /// <summary>
        ///  Clears the adorner of glyphs.
        /// </summary>
        public void Dispose()
        {
            if (_traySelectionAdorner is not null)
            {
                _traySelectionAdorner.Glyphs.Clear();
                _traySelectionAdorner = null;
            }
        }

        /// <summary>
        ///  Retrieves a list of glyphs associated with the component.
        /// </summary>
        public GlyphCollection GetGlyphsForComponent(IComponent comp)
        {
            GlyphCollection glyphs = [];
            if (_behaviorSvc is not null && comp is not null)
            {
                if (_behaviorSvc.DesignerActionUI is not null)
                {
                    Glyph g = _behaviorSvc.DesignerActionUI.GetDesignerActionGlyph(comp);
                    if (g is not null)
                    {
                        glyphs.Add(g);
                    }
                }
            }

            return glyphs;
        }

        /// <summary>
        ///  Called from the tray's NCHITTEST message in the WndProc.
        ///  We use this to loop through our glyphs and identify which one is successfully hit tested.
        ///  From here, we know where to send our messages.
        /// </summary>
        public Cursor GetHitTest(Point p)
        {
            for (int i = 0; i < _traySelectionAdorner.Glyphs.Count; i++)
            {
                Cursor hitTestCursor = _traySelectionAdorner.Glyphs[i].GetHitTest(p);
                if (hitTestCursor is not null)
                {
                    _hitTestedGlyph = _traySelectionAdorner.Glyphs[i];
                    return hitTestCursor;
                }
            }

            _hitTestedGlyph = null;
            return null;
        }

        /// <summary>
        ///  Called when the tray receives this mouse message.
        ///  Here, we'll give our glyphs the first chance to respond to the message before the tray even sees it.
        /// </summary>
        public bool OnMouseDoubleClick(MouseEventArgs e)
        {
            if (_hitTestedGlyph is not null && _hitTestedGlyph.Behavior is not null)
            {
                return _hitTestedGlyph.Behavior.OnMouseDoubleClick(_hitTestedGlyph, e.Button, new Point(e.X, e.Y));
            }

            return false;
        }

        /// <summary>
        ///  Called when the tray receives this mouse message.
        ///  Here, we'll give our glyphs the first chance to respond to the message before the tray even sees it.
        /// </summary>
        public bool OnMouseDown(MouseEventArgs e)
        {
            if (_hitTestedGlyph is not null && _hitTestedGlyph.Behavior is not null)
            {
                return _hitTestedGlyph.Behavior.OnMouseDown(_hitTestedGlyph, e.Button, new Point(e.X, e.Y));
            }

            return false;
        }

        /// <summary>
        ///  Called when the tray receives this mouse message.
        ///  Here, we'll give our glyphs the first chance to respond to the message before the tray even sees it.
        /// </summary>
        public bool OnMouseMove(MouseEventArgs e)
        {
            if (_hitTestedGlyph is not null && _hitTestedGlyph.Behavior is not null)
            {
                return _hitTestedGlyph.Behavior.OnMouseMove(_hitTestedGlyph, e.Button, new Point(e.X, e.Y));
            }

            return false;
        }

        /// <summary>
        ///  Called when the tray receives this mouse message.
        ///  Here, we'll give our glyphs the first chance to respond to the message before the tray even sees it.
        /// </summary>
        public bool OnMouseUp(MouseEventArgs e)
        {
            if (_hitTestedGlyph is not null && _hitTestedGlyph.Behavior is not null)
            {
                return _hitTestedGlyph.Behavior.OnMouseUp(_hitTestedGlyph, e.Button);
            }

            return false;
        }

        /// <summary>
        ///  Called when the comp tray or any tray control paints.
        ///  This will simply enumerate through the glyphs in our Adorner and ask them to paint.
        /// </summary>
        public void OnPaintGlyphs(PaintEventArgs pe)
        {
            // Paint any glyphs our tray adorner has
            foreach (Glyph g in _traySelectionAdorner.Glyphs)
            {
                g.Paint(pe);
            }
        }

        /// <summary>
        ///  Called when a tray control's location has changed.
        ///  We'll loop through our glyphs and invalidate any that are associated with the component.
        /// </summary>
        public void UpdateLocation(TrayControl trayControl)
        {
            foreach (Glyph g in _traySelectionAdorner.Glyphs)
            {
                // only look at glyphs that derive from designerglyph base (actions)
                if (g is DesignerActionGlyph desGlyph && ((DesignerActionBehavior)(desGlyph.Behavior)).RelatedComponent.Equals(trayControl.Component))
                {
                    desGlyph.UpdateAlternativeBounds(trayControl.Bounds);
                }
            }
        }
    }

    internal class AutoArrangeComparer : IComparer<Control>
    {
        int IComparer<Control>.Compare(Control o1, Control o2)
        {
            Debug.Assert(o1 is not null && o2 is not null, "Null objects sent for comparison!!!");
            Point tcLoc1 = o1.Location;
            Point tcLoc2 = o2.Location;
            int height = o1.Height / 2;
            // If they are at the same location, they are equal.
            if (tcLoc1.X == tcLoc2.X && tcLoc1.Y == tcLoc2.Y)
            {
                return 0;
            }

            // Is the first control lower than the 2nd...
            if (tcLoc1.Y + height <= tcLoc2.Y)
            {
                return -1;
            }

            // Is the 2nd control lower than the first...
            if (tcLoc2.Y + height <= tcLoc1.Y)
            {
                return 1;
            }

            // Which control is left of the other...
            return ((tcLoc1.X <= tcLoc2.X) ? -1 : 1);
        }
    }

    private class TraySelectionUIHandler : SelectionUIHandler
    {
        private readonly ComponentTray _tray;
        private Size _snapSize = Size.Empty;

        /// <summary>
        ///  Creates a new selection UI handler for the given component tray.
        /// </summary>
        public TraySelectionUIHandler(ComponentTray tray)
        {
            _tray = tray;
            _snapSize = default;
        }

        /// <summary>
        ///  Called when the user has started the drag.
        /// </summary>
        public override bool BeginDrag(object[] components, SelectionRules rules, int initialX, int initialY)
        {
            bool value = base.BeginDrag(components, rules, initialX, initialY);
            _tray.SuspendLayout();
            return value;
        }

        /// <summary>
        ///  Called when the user has completed the drag. The designer should remove any UI feedback it may be providing.
        /// </summary>
        public override void EndDrag(object[] components, bool cancel)
        {
            base.EndDrag(components, cancel);
            _tray.ResumeLayout();
        }

        /// <summary>
        ///  Retrieves the base component for the selection handler.
        /// </summary>
        protected override IComponent GetComponent()
        {
            return _tray;
        }

        /// <summary>
        ///  Retrieves the base component's UI control for the selection handler.
        /// </summary>
        protected override Control GetControl()
        {
            return _tray;
        }

        /// <summary>
        ///  Retrieves the UI control for the given component.
        /// </summary>
        protected override Control GetControl(IComponent component)
        {
            return TrayControl.FromComponent(component);
        }

        /// <summary>
        ///  Retrieves the current grid snap size we should snap objects to.
        /// </summary>
        protected override Size GetCurrentSnapSize()
        {
            return _snapSize;
        }

        /// <summary>
        ///  We use this to request often-used services.
        /// </summary>
        protected override object GetService(Type serviceType)
        {
            return _tray.GetService(serviceType);
        }

        /// <summary>
        ///  Determines if the selection UI handler should attempt to snap objects to a grid.
        /// </summary>
        protected override bool GetShouldSnapToGrid()
        {
            return false;
        }

        /// <summary>
        ///  Given a rectangle, this updates the dimensions of it with any grid snaps and returns a new rectangle.
        ///  If no changes to the rectangle's size were needed, this may return the same rectangle.
        /// </summary>
        public override Rectangle GetUpdatedRect(Rectangle originalRect, Rectangle dragRect, bool updateSize)
        {
            return dragRect;
        }

        /// <summary>
        ///  Asks the handler to set the appropriate cursor
        /// </summary>
        public override void SetCursor()
        {
            _tray.OnSetCursor();
        }
    }

    private class TrayOleDragDropHandler : OleDragDropHandler
    {
        public TrayOleDragDropHandler(SelectionUIHandler selectionHandler, IServiceProvider serviceProvider, IOleDragClient client) : base(selectionHandler, serviceProvider, client)
        {
        }

        protected override bool CanDropDataObject(IDataObject dataObj)
        {
            ICollection comps = null;
            if (dataObj is not null)
            {
                if (dataObj is ComponentDataObjectWrapper cdow)
                {
                    ComponentDataObject cdo = cdow.InnerData;
                    comps = cdo.Components;
                }
                else
                {
                    try
                    {
                        object serializationData = dataObj.GetData(DataFormat, true);
                        if (serializationData is null)
                        {
                            return false;
                        }

                        IDesignerSerializationService ds = (IDesignerSerializationService)GetService(typeof(IDesignerSerializationService));
                        if (ds is null)
                        {
                            return false;
                        }

                        comps = ds.Deserialize(serializationData);
                    }
                    catch (Exception e) when (!e.IsCriticalException())
                    {
                        // We return false on any exception.
                    }
                }
            }

            if (comps is not null && comps.Count > 0)
            {
                foreach (object comp in comps)
                {
                    if (comp is Point)
                    {
                        continue;
                    }

                    if (comp is Control or not IComponent)
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }
    }
}
