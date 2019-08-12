// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms.Design.Behavior;
using Microsoft.Win32;
using static Interop;

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  Provides the component tray UI for the form designer.
    /// </summary>
    [ToolboxItem(false)]
    [DesignTimeVisible(false)]
    [ProvideProperty("Location", typeof(IComponent))]
    [ProvideProperty("TrayLocation", typeof(IComponent))]
    public class ComponentTray : ScrollableControl, IExtenderProvider, ISelectionUIHandler, IOleDragClient
    {
        private static readonly Point InvalidPoint = new Point(int.MinValue, int.MinValue);
        private IServiceProvider serviceProvider; // Where services come from.
        private Point whiteSpace = Point.Empty; // space to leave between components.
        private Size grabHandle = Size.Empty; // Size of the grab handles.

        private ArrayList controls; // List of items in the tray in the order of their layout.
        private SelectionUIHandler dragHandler; // the thing responsible for handling mouse drags
        private ISelectionUIService selectionUISvc; // selectiuon UI; we use this a lot
        private IToolboxService toolboxService; // cached for drag/drop

        /// <summary>
        ///  Provides drag and drop functionality through OLE.
        /// </summary>
        internal OleDragDropHandler oleDragDropHandler; // handler class for ole drag drop operations.

        private readonly IDesigner mainDesigner; // the designer that is associated with this tray
        private IEventHandlerService eventHandlerService = null; // Event Handler service to handle keyboard and focus.
        private bool queriedTabOrder;
        private MenuCommand tabOrderCommand;
        private ICollection selectedObjects;

        // Services that we use on a high enough frequency to merit caching.
        private IMenuCommandService menuCommandService;
        private readonly CommandSet privateCommandSet = null;
        private InheritanceUI inheritanceUI;

        private Point mouseDragStart = InvalidPoint; // the starting location of a drag
        private Point mouseDragEnd = InvalidPoint; // the ending location of a drag
        private Rectangle mouseDragWorkspace = Rectangle.Empty; // a temp work rectangle we cache for perf
        private ToolboxItem mouseDragTool; // the tool that's being dragged; only for drag/drop
        private Point mouseDropLocation = InvalidPoint; // where the tool was dropped
        private bool showLargeIcons = false;// Show Large icons or not.
        private bool autoArrange = false; // allows for auto arranging icons.
        private Point autoScrollPosBeforeDragging = Point.Empty;//Used to return the correct scroll pos. after a drag

        // Component Tray Context menu items...
        private readonly MenuCommand menucmdArrangeIcons = null;
        private readonly MenuCommand menucmdLineupIcons = null;
        private readonly MenuCommand menucmdLargeIcons = null;
        private bool fResetAmbient = false;
        private bool fSelectionChanged = false;
        private ComponentTrayGlyphManager glyphManager;//used to manage any glyphs added to the tray

        // Empty class for build time dependancy

        /// <summary>
        ///  Creates a new component tray.  The component tray
        ///  will monitor component additions and removals and create
        ///  appropriate UI objects in its space.
        /// </summary>
        public ComponentTray(IDesigner mainDesigner, IServiceProvider serviceProvider)
        {
            AutoScroll = true;
            this.mainDesigner = mainDesigner;
            this.serviceProvider = serviceProvider;
            AllowDrop = true;
            Text = "ComponentTray"; // makes debugging easier
            SetStyle(ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
            controls = new ArrayList();
            IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
            IExtenderProviderService es = (IExtenderProviderService)GetService(typeof(IExtenderProviderService));
            Debug.Assert(es != null, "Component tray wants an extender provider service, but there isn't one.");
            if (es != null)
            {
                es.AddExtenderProvider(this);
            }

            if (GetService(typeof(IEventHandlerService)) == null)
            {
                if (host != null)
                {
                    eventHandlerService = new EventHandlerService(this);
                    host.AddService(typeof(IEventHandlerService), eventHandlerService);
                }
            }

            IMenuCommandService mcs = MenuService;
            if (mcs != null)
            {
                Debug.Assert(menucmdArrangeIcons == null, "Non-Null Menu Command for ArrangeIcons");
                Debug.Assert(menucmdLineupIcons == null, "Non-Null Menu Command for LineupIcons");
                Debug.Assert(menucmdLargeIcons == null, "Non-Null Menu Command for LargeIcons");
                menucmdArrangeIcons = new MenuCommand(new EventHandler(OnMenuArrangeIcons), StandardCommands.ArrangeIcons);
                menucmdLineupIcons = new MenuCommand(new EventHandler(OnMenuLineupIcons), StandardCommands.LineupIcons);
                menucmdLargeIcons = new MenuCommand(new EventHandler(OnMenuShowLargeIcons), StandardCommands.ShowLargeIcons);
                menucmdArrangeIcons.Checked = AutoArrange;
                menucmdLargeIcons.Checked = ShowLargeIcons;
                mcs.AddCommand(menucmdArrangeIcons);
                mcs.AddCommand(menucmdLineupIcons);
                mcs.AddCommand(menucmdLargeIcons);
            }

            IComponentChangeService componentChangeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));
            if (componentChangeService != null)
            {
                componentChangeService.ComponentRemoved += new ComponentEventHandler(OnComponentRemoved);
            }

            if (GetService(typeof(IUIService)) is IUIService uiService)
            {
                Color styleColor;
                if (uiService.Styles["ArtboardBackground"] is Color)
                {
                    styleColor = (Color)uiService.Styles["ArtboardBackground"];
                }
                //Can't use 'as' here since Color is a value type
                else if (uiService.Styles["VsColorDesignerTray"] is Color)
                {
                    styleColor = (Color)uiService.Styles["VsColorDesignerTray"];
                }
                else if (uiService.Styles["HighlightColor"] is Color)
                {
                    // Since v1, we have had code here that checks for HighlightColor, so some hosts (like WinRes) have been setting it. If VsColorDesignerTray isn't present, we look for HighlightColor for backward compat.
                    styleColor = (Color)uiService.Styles["HighlightColor"];
                }
                else
                {
                    //No style color provided? Let's pick a default.
                    styleColor = SystemColors.Info;
                }

                if (uiService.Styles["ArtboardBackgroundText"] is Color)
                {
                    ForeColor = (Color)uiService.Styles["ArtboardBackgroundText"];
                }
                else if (uiService.Styles["VsColorPanelText"] is Color)
                {
                    ForeColor = (Color)uiService.Styles["VsColorPanelText"];
                }

                BackColor = styleColor;
                Font = (Font)uiService.Styles["DialogFont"];
            }

            ISelectionService selSvc = (ISelectionService)GetService(typeof(ISelectionService));
            if (selSvc != null)
            {
                selSvc.SelectionChanged += new EventHandler(OnSelectionChanged);
            }

            // Listen to the SystemEvents so that we can resync selection based on display settings etc.
            SystemEvents.DisplaySettingsChanged += new EventHandler(OnSystemSettingChanged);
            SystemEvents.InstalledFontsChanged += new EventHandler(OnSystemSettingChanged);
            SystemEvents.UserPreferenceChanged += new UserPreferenceChangedEventHandler(OnUserPreferenceChanged);

            if (GetService(typeof(BehaviorService)) is BehaviorService behSvc)
            {
                //this object will manage any glyphs that get added to our tray
                glyphManager = new ComponentTrayGlyphManager(selSvc, behSvc);
            }
        }

        private void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            if (IsHandleCreated)
            {
                fResetAmbient = true;
                ResetTrayControls();
                BeginInvoke(new AsyncInvokeHandler(Invalidate), new object[] { true });
            }
        }

        private void OnComponentRefresh(RefreshEventArgs e)
        {
            if (e.ComponentChanged is IComponent component)
            {
                TrayControl control = TrayControl.FromComponent(component);
                if (control != null)
                {
                    bool shouldDisplay = CanDisplayComponent(component);
                    if (shouldDisplay != control.Visible || !shouldDisplay)
                    {
                        control.Visible = shouldDisplay;
                        Rectangle bounds = control.Bounds;
                        bounds.Inflate(grabHandle);
                        bounds.Inflate(grabHandle);
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
                fResetAmbient = true;
                ResetTrayControls();
                BeginInvoke(new AsyncInvokeHandler(Invalidate), new object[] { true });
            }
        }

        private void ResetTrayControls()
        {
            ControlCollection children = (ControlCollection)Controls;
            if (children == null)
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
            selectedObjects = ((ISelectionService)sender).GetSelectedComponents();
            object primary = ((ISelectionService)sender).PrimarySelection;
            Invalidate();
            fSelectionChanged = true;
            // Accessibility information
            foreach (object selObj in selectedObjects)
            {
                if (selObj is IComponent component)
                {
                    Control c = TrayControl.FromComponent(component);
                    if (c != null)
                    {
                        Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo, "MSAA: SelectionAdd, traycontrol = " + c.ToString());
                        UnsafeNativeMethods.NotifyWinEvent((int)AccessibleEvents.SelectionAdd, new HandleRef(c, c.Handle), NativeMethods.OBJID_CLIENT, 0);
                    }
                }
            }

            if (primary is IComponent comp)
            {
                Control c = TrayControl.FromComponent(comp);
                if (c != null && IsHandleCreated)
                {
                    ScrollControlIntoView(c);
                    UnsafeNativeMethods.NotifyWinEvent((int)AccessibleEvents.Focus, new HandleRef(c, c.Handle), NativeMethods.OBJID_CLIENT, 0);
                }
                if (glyphManager != null)
                {
                    glyphManager.SelectionGlyphs.Clear();
                    IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
                    foreach (object selObj in selectedObjects)
                    {
                        if (selObj is IComponent selectedComponent && !(host.GetDesigner(selectedComponent) is ControlDesigner))
                        { // don't want to do it for controls that are also in the tray
                            GlyphCollection glyphs = glyphManager.GetGlyphsForComponent(selectedComponent);
                            if (glyphs != null && glyphs.Count > 0)
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
                PropertyDescriptor trayIconProp = TypeDescriptor.GetProperties(mainDesigner.Component)["TrayLargeIcon"];
                if (trayIconProp != null)
                {
                    trayIconProp.SetValue(mainDesigner.Component, !ShowLargeIcons);
                }
            }
            finally
            {
                if (t != null)
                {
                    t.Commit();
                }
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
                if (t != null)
                {
                    t.Commit();
                }
            }
        }

        private void DoLineupIcons()
        {
            if (autoArrange)
            {
                return;
            }

            bool oldValue = autoArrange;
            autoArrange = true;
            try
            {
                DoAutoArrange(true);
            }
            finally
            {
                autoArrange = oldValue;
            }
        }

        private void DoAutoArrange(bool dirtyDesigner)
        {
            if (controls == null || controls.Count <= 0)
            {
                return;
            }

            controls.Sort(new AutoArrangeComparer());

            SuspendLayout();

            //Reset the autoscroll position before auto arranging.
            //This way, when OnLayout gets fired after this, we won't
            //have to move every component again.  Note that sync'ing
            //the selection will automatically select & scroll into view
            //the right components
            AutoScrollPosition = new Point(0, 0);

            try
            {
                Control prevCtl = null;
                bool positionedGlobal = true;
                foreach (Control ctl in controls)
                {
                    if (!ctl.Visible)
                    {
                        continue;
                    }
                    // If we're auto arranging, always move the control.  If not, move the control only if it was never given a position.  This auto arranges it until the user messes with it, or until its position is saved into the resx. (if one control is no longer positioned, move all the other one as  we don't want them to go under one another)
                    if (autoArrange)
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
                if (selectionUISvc != null)
                {
                    selectionUISvc.SyncSelection();
                }
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

                PropertyDescriptor trayAAProp = TypeDescriptor.GetProperties(mainDesigner.Component)["TrayAutoArrange"];
                if (trayAAProp != null)
                {
                    trayAAProp.SetValue(mainDesigner.Component, !AutoArrange);
                }
            }
            finally
            {
                if (t != null)
                {
                    t.Commit();
                }
            }
        }

        public bool AutoArrange
        {
            get => autoArrange;
            set
            {
                if (autoArrange != value)
                {
                    autoArrange = value;
                    menucmdArrangeIcons.Checked = value;

                    if (autoArrange)
                    {
                        DoAutoArrange(true);
                    }
                }
            }
        }

        /// <summary>
        ///  Gets the number of compnents contained within this tray.
        /// </summary>
        public int ComponentCount
        {
            get => Controls.Count;
        }

        internal GlyphCollection SelectionGlyphs
        {
            get
            {
                if (glyphManager != null)
                {
                    return glyphManager.SelectionGlyphs;
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
            get => showLargeIcons;
            set
            {
                if (showLargeIcons != value)
                {
                    showLargeIcons = value;
                    menucmdLargeIcons.Checked = ShowLargeIcons;

                    ResetTrayControls();
                    Invalidate(true);
                }
            }
        }

        bool IExtenderProvider.CanExtend(object extendee)
        {
            return (extendee is IComponent comp) && (TrayControl.FromComponent(comp) != null);
        }

        IComponent IOleDragClient.Component
        {
            get => mainDesigner.Component;
        }

        bool IOleDragClient.CanModifyComponents
        {
            get => true;
        }

        bool IOleDragClient.AddComponent(IComponent component, string name, bool firstAdd)
        {
            // the designer for controls decides what to do here
            if (mainDesigner is IOleDragClient oleDragClient)
            {
                try
                {
                    oleDragClient.AddComponent(component, name, firstAdd);
                    PositionControl(TrayControl.FromComponent(component));
                    mouseDropLocation = InvalidPoint;
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
                    if (host != null && host.Container != null)
                    {
                        if (host.Container.Components[name] != null)
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
            if (oleDragDropHandler == null)
            {
                oleDragDropHandler = new TrayOleDragDropHandler(DragHandler, serviceProvider, this);
            }
            return oleDragDropHandler;
        }

        internal virtual SelectionUIHandler DragHandler
        {
            get
            {
                if (dragHandler == null)
                {
                    dragHandler = new TraySelectionUIHandler(this);
                }
                return dragHandler;
            }
        }

        void ISelectionUIHandler.DragMoved(object[] components, Rectangle offset) => DragHandler.DragMoved(components, offset);

        void ISelectionUIHandler.EndDrag(object[] components, bool cancel)
        {
            DragHandler.EndDrag(components, cancel);
            GetOleDragHandler().DoEndDrag(components, cancel);
            // Here, after the drag is finished and after we have resumed layout, adjust the location of the components we dragged by the scroll offset
            if (!autoScrollPosBeforeDragging.IsEmpty)
            {
                foreach (IComponent comp in components)
                {
                    TrayControl tc = TrayControl.FromComponent(comp);
                    if (tc != null)
                    {
                        SetTrayLocation(comp, new Point(tc.Location.X - autoScrollPosBeforeDragging.X, tc.Location.Y - autoScrollPosBeforeDragging.Y));
                    }
                }
                AutoScrollPosition = new Point(-autoScrollPosBeforeDragging.X, -autoScrollPosBeforeDragging.Y);
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

        bool ISelectionUIHandler.QueryBeginDrag(object[] components, SelectionRules rules, int initialX, int initialY) => DragHandler.QueryBeginDrag(components, rules, initialX, initialY);

        void ISelectionUIHandler.ShowContextMenu(IComponent component)
        {
            Point cur = Control.MousePosition;
            OnContextMenu(cur.X, cur.Y, true);
        }

        private void OnContextMenu(int x, int y, bool useSelection)
        {
            if (!TabOrderActive)
            {
                Capture = false;
                IMenuCommandService mcs = MenuService;
                if (mcs != null)
                {
                    Capture = false;
                    Cursor.Clip = Rectangle.Empty;
                    ISelectionService s = (ISelectionService)GetService(typeof(ISelectionService));
                    if (useSelection && s != null && !(1 == s.SelectionCount && s.PrimarySelection == mainDesigner.Component))
                    {
                        mcs.ShowContextMenu(MenuCommands.TraySelectionMenu, x, y);
                    }
                    else
                    {
                        mcs.ShowContextMenu(MenuCommands.ComponentTrayMenu, x, y);
                    }
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
            if (selectionUISvc == null)
            {
                selectionUISvc = (ISelectionUIService)GetService(typeof(ISelectionUIService));

                // If there is no selection service, then we will provide our own.
                if (selectionUISvc == null)
                {
                    selectionUISvc = new SelectionUIService(host);
                    host.AddService(typeof(ISelectionUIService), selectionUISvc);
                }
                grabHandle = selectionUISvc.GetAdornmentDimensions(AdornmentType.GrabHandle);
            }

            // Create a new instance of a tray control.
            TrayControl trayctl = new TrayControl(this, component);
            SuspendLayout();
            try
            {
                // Add it to us.
                Controls.Add(trayctl);
                controls.Add(trayctl);
                // CanExtend can actually be called BEFORE the component is added to the ComponentTray. ToolStrip is such as scenario:
                // 1. Add a timer to the Tray.
                // 2. Add a ToolStrip.
                // 3. ToolStripDesigner.Initialize will be called before ComponentTray.AddComponent, so the ToolStrip is not yet added to the tray.
                // 4. TooStripDesigner.Initialize calls GetProperties, which causes our CanExtend to be called.
                // 5. CanExtend will return false, since the component has not yet been added.
                // 6. This causes all sorts of badness
                // Fix is to refresh.
                TypeDescriptor.Refresh(component);
                if (host != null && !host.Loading)
                {
                    PositionControl(trayctl);
                }
                if (selectionUISvc != null)
                {
                    selectionUISvc.AssignSelectionUIHandler(component, this);
                }
                InheritanceAttribute attr = trayctl.InheritanceAttribute;
                if (attr.InheritanceLevel != InheritanceLevel.NotInherited)
                {
                    InheritanceUI iui = InheritanceUI;
                    if (iui != null)
                    {
                        iui.AddInheritedControl(trayctl, attr.InheritanceLevel);
                    }
                }
            }
            finally
            {
                ResumeLayout();
            }
            if (host != null && !host.Loading)
            {
                ScrollControlIntoView(trayctl);
            }
        }

        [CLSCompliant(false)]
        protected virtual bool CanCreateComponentFromTool(ToolboxItem tool)
        {
            IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
            Debug.Assert(host != null, "Service object could not provide us with a designer host.");
            // Disallow controls to be added to the component tray.
            Type compType = host.GetType(tool.TypeName);
            if (compType == null)
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
                    if (attributeBaseType != null && attributeBaseType == designerBaseType)
                    {
                        bool foundService = false;
                        ITypeResolutionService tr = (ITypeResolutionService)GetService(typeof(ITypeResolutionService));
                        if (tr != null)
                        {
                            foundService = true;
                            designerType = tr.GetType(da.DesignerTypeName);
                        }

                        if (!foundService)
                        {
                            designerType = Type.GetType(da.DesignerTypeName);
                        }

                        if (designerType != null)
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
        ///  If it returns true, then the component will get a glyph in the tray area.  If it returns
        ///  false, then the component will not actually be added to the tray.  The default
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
            // We invoke the drag drop handler for this.  This implementation is shared between all designers that create components.
            GetOleDragHandler().CreateTool(tool, null, 0, 0, 0, 0, false, false);
        }

        /// <summary>
        ///  Displays the given exception to the user.
        /// </summary>
        protected void DisplayError(Exception e)
        {
            IUIService uis = (IUIService)GetService(typeof(IUIService));
            if (uis != null)
            {
                uis.ShowError(e);
            }
            else
            {
                string message = e.Message;
                if (message == null || message.Length == 0)
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
            if (disposing && controls != null)
            {
                IExtenderProviderService es = (IExtenderProviderService)GetService(typeof(IExtenderProviderService));
                Debug.Assert(!CompModSwitches.CommonDesignerServices.Enabled || (es != null), "IExtenderProviderService not found");
                if (es != null)
                {
                    es.RemoveExtenderProvider(this);
                }

                IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
                if (eventHandlerService != null)
                {
                    if (host != null)
                    {
                        host.RemoveService(typeof(IEventHandlerService));
                        eventHandlerService = null;
                    }
                }

                IComponentChangeService componentChangeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));
                if (componentChangeService != null)
                {
                    componentChangeService.ComponentRemoved -= new ComponentEventHandler(OnComponentRemoved);
                }

                SystemEvents.DisplaySettingsChanged -= new EventHandler(OnSystemSettingChanged);
                SystemEvents.InstalledFontsChanged -= new EventHandler(OnSystemSettingChanged);
                SystemEvents.UserPreferenceChanged -= new UserPreferenceChangedEventHandler(OnUserPreferenceChanged);
                IMenuCommandService mcs = MenuService;
                if (mcs != null)
                {
                    Debug.Assert(menucmdArrangeIcons != null, "Null Menu Command for ArrangeIcons");
                    Debug.Assert(menucmdLineupIcons != null, "Null Menu Command for LineupIcons");
                    Debug.Assert(menucmdLargeIcons != null, "Null Menu Command for LargeIcons");
                    mcs.RemoveCommand(menucmdArrangeIcons);
                    mcs.RemoveCommand(menucmdLineupIcons);
                    mcs.RemoveCommand(menucmdLargeIcons);
                }

                if (privateCommandSet != null)
                {
                    privateCommandSet.Dispose();
                    // If we created a private command set, we also added a selection ui service to the host
                    if (host != null)
                    {
                        host.RemoveService(typeof(ISelectionUIService));
                    }
                }
                selectionUISvc = null;

                if (inheritanceUI != null)
                {
                    inheritanceUI.Dispose();
                    inheritanceUI = null;
                }

                serviceProvider = null;
                controls.Clear();
                controls = null;

                if (glyphManager != null)
                {
                    glyphManager.Dispose();
                    glyphManager = null;
                }
            }
            base.Dispose(disposing);
        }

        /// <summary>
        ///  Similar to GetNextControl on Control, this method returns the next
        ///  component in the tray, given a starting component.  It will return
        ///  null if the end (or beginning, if forward is false) of the list
        ///  is encountered.
        /// </summary>
        public IComponent GetNextComponent(IComponent component, bool forward)
        {
            for (int i = 0; i < controls.Count; i++)
            {
                TrayControl control = (TrayControl)controls[i];
                if (control.Component == component)
                {
                    int targetIndex = (forward ? i + 1 : i - 1);
                    if (targetIndex >= 0 && targetIndex < controls.Count)
                    {
                        return ((TrayControl)controls[targetIndex]).Component;
                    }
                    // Reached the end of the road.
                    return null;
                }
            }
            // If we got here then the component isn't in our list.  Prime the caller with either the first or the last.
            if (controls.Count > 0)
            {
                int targetIndex = (forward ? 0 : controls.Count - 1);
                return ((TrayControl)controls[targetIndex]).Component;
            }
            return null;
        }

        /// <summary>
        ///  Accessor method for the location extender property.  We offer this extender
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
            if (loc != null)
            {
                // In this case the component already had a Location property, and what the caller wants is the underlying components Location, not the tray location. Why? Because we now use TrayLocation.
                return (Point)(loc.GetValue(receiver));
            }
            else
            {
                // If the component didn't already have a Location property, then the caller really wants the tray location. Could be a 3rd party vendor.
                return GetTrayLocation(receiver);
            }
        }

        /// <summary>
        ///  Accessor method for the location extender property.  We offer this extender
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
            if (c == null)
            {
                Debug.Fail("Anything we're extending should have a component view.");
                return new Point();
            }
            Point loc = c.Location;
            Point autoScrollLoc = AutoScrollPosition;
            return new Point(loc.X - autoScrollLoc.X, loc.Y - autoScrollLoc.Y);
        }

        /// <summary>
        ///  Gets the requsted service type.
        /// </summary>
        protected override object GetService(Type serviceType)
        {
            object service = null;
            Debug.Assert(serviceProvider != null, "Trying to access services too late or too early.");
            if (serviceProvider != null)
            {
                service = serviceProvider.GetService(serviceType);
            }
            return service;
        }

        /// <summary>
        ///  Returns true if the given componenent is being shown on the tray.
        /// </summary>
        public bool IsTrayComponent(IComponent comp)
        {
            if (TrayControl.FromComponent(comp) == null)
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
            //give our glyphs first chance at this
            if (glyphManager != null && glyphManager.OnMouseDoubleClick(e))
            {
                //handled by a glyph - so don't send to the comp tray
                return;
            }
            base.OnDoubleClick(e);
            if (!TabOrderActive)
            {
                OnLostCapture();
                IEventBindingService eps = (IEventBindingService)GetService(typeof(IEventBindingService));
                Debug.Assert(!CompModSwitches.CommonDesignerServices.Enabled || (eps != null), "IEventBindingService not found");
                if (eps != null)
                {
                    eps.ShowCode();
                }
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
        ///  Called in response to a drag drop for OLE drag and drop.  Here we
        ///  drop a toolbox component on our parent control.
        /// </summary>
        protected override void OnDragDrop(DragEventArgs de)
        {
            // This will be used once during PositionComponent to place the component at the drop point. It is automatically set to null afterwards, so further components appear after the first one dropped.
            mouseDropLocation = PointToClient(new Point(de.X, de.Y));
            autoScrollPosBeforeDragging = AutoScrollPosition; // save the scroll position
            if (mouseDragTool != null)
            {
                ToolboxItem tool = mouseDragTool;
                mouseDragTool = null;
                Debug.Assert(!CompModSwitches.CommonDesignerServices.Enabled || (GetService(typeof(IDesignerHost)) != null), "IDesignerHost not found");
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
                    if (ClientUtils.IsCriticalException(e))
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
            mouseDropLocation = InvalidPoint;
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
                if (toolboxService == null)
                {
                    toolboxService = (IToolboxService)GetService(typeof(IToolboxService));
                }
                OleDragDropHandler dragDropHandler = GetOleDragHandler();
                object[] dragComps = dragDropHandler.GetDraggingObjects(de);
                // Only assume the items came from the ToolBox if dragComps == null
                if (toolboxService != null && dragComps == null)
                {
                    mouseDragTool = toolboxService.DeserializeToolboxItem(de.Data, (IDesignerHost)GetService(typeof(IDesignerHost)));
                }
                if (mouseDragTool != null)
                {
                    Debug.Assert(0 != (int)(de.AllowedEffect & (DragDropEffects.Move | DragDropEffects.Copy)), "DragDropEffect.Move | .Copy isn't allowed?");
                    if ((int)(de.AllowedEffect & DragDropEffects.Move) != 0)
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
            mouseDragTool = null;
            GetOleDragHandler().DoOleDragLeave();
            ResumeLayout();
        }

        /// <summary>
        ///  Called when a drag drop object is dragged over the control designer view
        /// </summary>
        protected override void OnDragOver(DragEventArgs de)
        {
            if (mouseDragTool != null)
            {
                Debug.Assert(0 != (int)(de.AllowedEffect & DragDropEffects.Copy), "DragDropEffect.Move isn't allowed?");
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
        ///  This is called when we lose capture.  Here we get rid of any
        ///  rubber band we were drawing.  You should put any cleanup
        ///  code in here.
        /// </summary>
        protected virtual void OnLostCapture()
        {
            if (mouseDragStart != InvalidPoint)
            {
                Cursor.Clip = Rectangle.Empty;
                if (mouseDragEnd != InvalidPoint)
                {
                    DrawRubber(mouseDragStart, mouseDragEnd);
                    mouseDragEnd = InvalidPoint;
                }
                mouseDragStart = InvalidPoint;
            }
        }

        private void DrawRubber(Point start, Point end)
        {
            mouseDragWorkspace.X = Math.Min(start.X, end.X);
            mouseDragWorkspace.Y = Math.Min(start.Y, end.Y);
            mouseDragWorkspace.Width = Math.Abs(end.X - start.X);
            mouseDragWorkspace.Height = Math.Abs(end.Y - start.Y);
            mouseDragWorkspace = RectangleToScreen(mouseDragWorkspace);
            ControlPaint.DrawReversibleFrame(mouseDragWorkspace, BackColor, FrameStyle.Dashed);
        }

        /// <summary>
        ///  Inheriting classes should override this method to handle this event.
        ///  Call base.onMouseDown to send this event to any registered event listeners.
        /// </summary>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            //give our glyphs first chance at this
            if (glyphManager != null && glyphManager.OnMouseDown(e))
            {
                //handled by a glyph - so don't send to the comp tray
                return;
            }

            base.OnMouseDown(e);
            if (!TabOrderActive)
            {
                if (toolboxService == null)
                {
                    toolboxService = (IToolboxService)GetService(typeof(IToolboxService));
                }
                FocusDesigner();
                if (e.Button == MouseButtons.Left && toolboxService != null)
                {
                    ToolboxItem tool = toolboxService.GetSelectedToolboxItem((IDesignerHost)GetService(typeof(IDesignerHost)));
                    if (tool != null)
                    {
                        // mouseDropLocation is checked in PositionControl, which should get called as a result of adding a new component.  This allows us to set the position without flickering, while still providing support for auto layout if the control was double clicked or added through extensibility.
                        mouseDropLocation = new Point(e.X, e.Y);
                        try
                        {
                            CreateComponentFromTool(tool);
                            toolboxService.SelectedToolboxItemUsed();
                        }
                        catch (Exception ex)
                        {
                            DisplayError(ex);
                            if (ClientUtils.IsCriticalException(ex))
                            {
                                throw;
                            }
                        }
                        mouseDropLocation = InvalidPoint;
                        return;
                    }
                }

                // If it is the left button, start a rubber band drag to laso controls.
                if (e.Button == MouseButtons.Left)
                {
                    mouseDragStart = new Point(e.X, e.Y);
                    Capture = true;
                    Cursor.Clip = RectangleToScreen(ClientRectangle);
                }
                else
                {
                    try
                    {
                        ISelectionService ss = (ISelectionService)GetService(typeof(ISelectionService));
                        Debug.Assert(!CompModSwitches.CommonDesignerServices.Enabled || (ss != null), "ISelectionService not found");
                        if (ss != null)
                        {
                            ss.SetSelectedComponents(new object[] { mainDesigner.Component });
                        }
                    }
                    catch (Exception ex)
                    {
                        // nothing we can really do here; just eat it.
                        if (ClientUtils.IsCriticalException(ex))
                        {
                            throw;
                        }
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
            //give our glyphs first chance at this
            if (glyphManager != null && glyphManager.OnMouseMove(e))
            {
                //handled by a glyph - so don't send to the comp tray
                return;
            }
            base.OnMouseMove(e);

            // If we are dragging, then draw our little rubber band.
            if (mouseDragStart != InvalidPoint)
            {
                if (mouseDragEnd != InvalidPoint)
                {
                    DrawRubber(mouseDragStart, mouseDragEnd);
                }
                else
                {
                    mouseDragEnd = new Point(0, 0);
                }
                mouseDragEnd.X = e.X;
                mouseDragEnd.Y = e.Y;
                DrawRubber(mouseDragStart, mouseDragEnd);
            }
        }

        /// <summary>
        ///  Inheriting classes should override this method to handle this event.
        ///  Call base.onMouseUp to send this event to any registered event listeners.
        /// </summary>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            //give our glyphs first chance at this
            if (glyphManager != null && glyphManager.OnMouseUp(e))
            {
                //handled by a glyph - so don't send to the comp tray
                return;
            }

            if (mouseDragStart != InvalidPoint && e.Button == MouseButtons.Left)
            {
                object[] comps;
                Capture = false;
                Cursor.Clip = Rectangle.Empty;
                if (mouseDragEnd != InvalidPoint)
                {
                    DrawRubber(mouseDragStart, mouseDragEnd);
                    Rectangle rect = new Rectangle
                    {
                        X = Math.Min(mouseDragStart.X, e.X),
                        Y = Math.Min(mouseDragStart.Y, e.Y),
                        Width = Math.Abs(e.X - mouseDragStart.X),
                        Height = Math.Abs(e.Y - mouseDragStart.Y)
                    };
                    comps = GetComponentsInRect(rect);
                    mouseDragEnd = InvalidPoint;
                }
                else
                {
                    comps = Array.Empty<object>();
                }

                if (comps.Length == 0)
                {
                    comps = new object[] { mainDesigner.Component };
                }
                try
                {
                    ISelectionService ss = (ISelectionService)GetService(typeof(ISelectionService));
                    Debug.Assert(!CompModSwitches.CommonDesignerServices.Enabled || (ss != null), "ISelectionService not found");
                    if (ss != null)
                    {
                        ss.SetSelectedComponents(comps);
                    }
                }
                catch (Exception ex)
                {
                    // nothing we can really do here; just eat it.
                    if (ClientUtils.IsCriticalException(ex))
                    {
                        throw;
                    }
                }
                mouseDragStart = InvalidPoint;
            }
            base.OnMouseUp(e);
        }

        private object[] GetComponentsInRect(Rectangle rect)
        {
            ArrayList list = new ArrayList();
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
            return list.ToArray();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            if (fResetAmbient || fSelectionChanged)
            {
                fResetAmbient = false;
                fSelectionChanged = false;
                IUIService uiService = (IUIService)GetService(typeof(IUIService));
                if (uiService != null)
                {
                    Color styleColor;
                    if (uiService.Styles["ArtboardBackground"] is Color)
                    {
                        styleColor = (Color)uiService.Styles["ArtboardBackground"];
                    }
                    //Can't use 'as' here since Color is a value type
                    else if (uiService.Styles["VsColorDesignerTray"] is Color)
                    {
                        styleColor = (Color)uiService.Styles["VsColorDesignerTray"];
                    }
                    else if (uiService.Styles["HighlightColor"] is Color)
                    {
                        // Since v1, we have had code here that checks for HighlightColor, so some hosts (like WinRes) have been setting it. If VsColorDesignerTray isn't present, we look for HighlightColor for backward compat.
                        styleColor = (Color)uiService.Styles["HighlightColor"];
                    }
                    else
                    {
                        //No style color provided? Let's pick a default.
                        styleColor = SystemColors.Info;
                    }

                    BackColor = styleColor;
                    Font = (Font)uiService.Styles["DialogFont"];
                    foreach (Control ctl in controls)
                    {
                        ctl.BackColor = styleColor;
                        ctl.ForeColor = ForeColor;
                    }
                }
            }

            base.OnPaint(pe);
            Graphics gr = pe.Graphics;
            // Now, if we have a selection, paint it
            if (selectedObjects != null)
            {
                bool first = true;//indicates the first iteration of our foreach loop
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
                    foreach (object o in selectedObjects)
                    {
                        Control c = ((IOleDragClient)this).GetControlForComponent(o);
                        if (c != null && c.Visible)
                        {
                            Rectangle innerRect = c.Bounds;
                            if (SystemInformation.HighContrast)
                            {
                                c.ForeColor = SystemColors.HighlightText;
                                c.BackColor = SystemColors.Highlight;
                            }
                            NoResizeHandleGlyph glyph = new NoResizeHandleGlyph(innerRect, SelectionRules.None, first, null);
                            gr.FillRectangle(selectionBorderBrush, DesignerUtils.GetBoundsForNoResizeSelectionType(innerRect, SelectionBorderGlyphType.Top));
                            gr.FillRectangle(selectionBorderBrush, DesignerUtils.GetBoundsForNoResizeSelectionType(innerRect, SelectionBorderGlyphType.Bottom));
                            gr.FillRectangle(selectionBorderBrush, DesignerUtils.GetBoundsForNoResizeSelectionType(innerRect, SelectionBorderGlyphType.Left));
                            gr.FillRectangle(selectionBorderBrush, DesignerUtils.GetBoundsForNoResizeSelectionType(innerRect, SelectionBorderGlyphType.Right));
                            // Need to draw this one last
                            DesignerUtils.DrawNoResizeHandle(gr, glyph.Bounds, first, glyph);
                        }
                        first = false;
                    }
                }
                finally
                {
                    if (selectionBorderBrush != null)
                    {
                        selectionBorderBrush.Dispose();
                    }
                }
            }
            //paint any glyphs
            if (glyphManager != null)
            {
                glyphManager.OnPaintGlyphs(pe);
            }
        }

        /// <summary>
        ///  Sets the cursor.  You may override this to set your own
        ///  cursor.
        /// </summary>
        protected virtual void OnSetCursor()
        {
            if (toolboxService == null)
            {
                toolboxService = (IToolboxService)GetService(typeof(IToolboxService));
            }
            if (toolboxService == null || !toolboxService.SetCursor())
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
            if (c != null)
            {
                try
                {
                    InheritanceAttribute attr = c.InheritanceAttribute;
                    if (attr.InheritanceLevel != InheritanceLevel.NotInherited && inheritanceUI != null)
                    {
                        inheritanceUI.RemoveInheritedControl(c);
                    }
                    if (controls != null)
                    {
                        int index = controls.IndexOf(c);
                        if (index != -1)
                        {
                            controls.RemoveAt(index);
                        }
                    }
                }
                finally
                {
                    c.Dispose();
                }
            }
        }

        /// <summary>
        ///  Accessor method for the location extender property.  We offer this extender
        ///  to all non-visual components.
        /// </summary>
        public void SetLocation(IComponent receiver, Point location)
        {
            // This really should only be called when we are loading.
            IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
            if (host != null && host.Loading)
            {
                // If we are loading, and we get called here, that's because we have provided the extended Location property. In this case we are loading an old project, and what we are really setting is the tray location.
                SetTrayLocation(receiver, location);
            }
            else
            {
                // we are not loading
                PropertyDescriptor loc = TypeDescriptor.GetProperties(receiver.GetType())["Location"];
                if (loc != null)
                {
                    // so if the component already had the Location property, what the caller wants is really the underlying component's Location property.
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
        ///  Accessor method for the location extender property.  We offer this extender
        ///  to all non-visual components.
        /// </summary>
        public void SetTrayLocation(IComponent receiver, Point location)
        {
            TrayControl c = TrayControl.FromComponent(receiver);
            if (c == null)
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
            switch (m.Msg)
            {
                case WindowMessages.WM_CANCELMODE:
                    // When we get cancelmode (i.e. you tabbed away to another window) then we want to cancel any pending drag operation!
                    OnLostCapture();
                    break;
                case WindowMessages.WM_SETCURSOR:
                    OnSetCursor();
                    return;
                case WindowMessages.WM_HSCROLL:
                case WindowMessages.WM_VSCROLL:
                    // When we scroll, we reposition a control without causing a property change event.  Therefore, we must tell the selection UI service to sync itself.
                    base.WndProc(ref m);
                    if (selectionUISvc != null)
                    {
                        selectionUISvc.SyncSelection();
                    }
                    return;
                case WindowMessages.WM_STYLECHANGED:
                    // When the scroll bars first appear, we need to invalidate so we properly paint our grid.
                    Invalidate();
                    break;
                case WindowMessages.WM_CONTEXTMENU:
                    // Pop a context menu for the composition designer.
                    int x = NativeMethods.Util.SignedLOWORD(unchecked((int)(long)m.LParam));
                    int y = NativeMethods.Util.SignedHIWORD(unchecked((int)(long)m.LParam));
                    if (x == -1 && y == -1)
                    {
                        // for shift-F10
                        Point mouse = Control.MousePosition;
                        x = mouse.X;
                        y = mouse.Y;
                    }
                    OnContextMenu(x, y, true);
                    break;
                case WindowMessages.WM_NCHITTEST:
                    if (glyphManager != null)
                    {
                        // Get a hit test on any glyhs that we are managing this way - we know where to route appropriate  messages
                        Point pt = new Point((short)NativeMethods.Util.LOWORD(unchecked((int)(long)m.LParam)), (short)NativeMethods.Util.HIWORD(unchecked((int)(long)m.LParam)));
                        var pt1 = new Point();
                        NativeMethods.MapWindowPoints(IntPtr.Zero, Handle, ref pt1, 1);
                        pt.Offset(pt1.X, pt1.Y);
                        glyphManager.GetHitTest(pt);
                    }
                    base.WndProc(ref m);
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        internal TrayControl GetTrayControlFromComponent(IComponent comp)
        {
            return TrayControl.FromComponent(comp);
        }

        private bool TabOrderActive
        {
            get
            {
                if (!queriedTabOrder)
                {
                    queriedTabOrder = true;
                    IMenuCommandService mcs = MenuService;
                    if (mcs != null)
                    {
                        tabOrderCommand = mcs.FindCommand(MenuCommands.TabOrder);
                    }
                }
                if (tabOrderCommand != null)
                {
                    return tabOrderCommand.Checked;
                }
                return false;
            }
        }

        private InheritanceUI InheritanceUI
        {
            get
            {
                if (inheritanceUI == null)
                {
                    inheritanceUI = new InheritanceUI();
                }
                return inheritanceUI;
            }
        }

        private IMenuCommandService MenuService
        {
            get
            {
                if (menuCommandService == null)
                {
                    menuCommandService = (IMenuCommandService)GetService(typeof(IMenuCommandService));
                }
                return menuCommandService;
            }
        }

        internal void FocusDesigner()
        {
            IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
            if (host != null && host.RootComponent != null)
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
                if (mainDesigner is ParentControlDesigner designer)
                {
                    return designer.ParentGridSize;
                }

                return new Size(8, 8);
            }
        }

        internal void UpdatePastePositions(ArrayList components)
        {
            foreach (TrayControl c in components)
            {
                if (!CanDisplayComponent(c.Component))
                {
                    return;
                }

                if (mouseDropLocation == InvalidPoint)
                {
                    Control prevCtl = null;
                    if (controls.Count > 1)
                    {
                        prevCtl = (Control)controls[controls.Count - 1];
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
            Debug.Assert(c.Visible, "TrayControl for " + c.Component + " should not be positioned");
            if (!autoArrange)
            {
                if (mouseDropLocation != InvalidPoint)
                {
                    if (!c.Location.Equals(mouseDropLocation))
                    {
                        c.Location = mouseDropLocation;
                    }
                }
                else
                {
                    Control prevCtl = null;
                    if (controls.Count > 1)
                    {
                        // PositionControl can be called when all the controls have been added (from IOleDragClient.AddComponent), so we can't use the old way of looking up the previous control (prevCtl = controls[controls.Count - 2]
                        int index = controls.IndexOf(c);
                        Debug.Assert(index >= 1, "Got the wrong index, how could that be?");
                        if (index >= 1)
                        {
                            prevCtl = (Control)controls[index - 1];
                        }
                    }
                    PositionInNextAutoSlot(c, prevCtl, true);
                }
            }
            else
            {
                if (mouseDropLocation != InvalidPoint)
                {
                    RearrangeInAutoSlots(c, mouseDropLocation);
                }
                else
                {
                    Control prevCtl = null;
                    if (controls.Count > 1)
                    {
                        int index = controls.IndexOf(c);
                        Debug.Assert(index >= 1, "Got the wrong index, how could that be?");
                        if (index >= 1)
                        {
                            prevCtl = (Control)controls[index - 1];
                        }
                    }
                    PositionInNextAutoSlot(c, prevCtl, true);
                }
            }
        }

        internal void RearrangeInAutoSlots(Control c, Point pos)
        {
#if DEBUG
            int index = controls.IndexOf(c);
            Debug.Assert(index != -1, "Add control to the list of controls before autoarranging.!!!");
            Debug.Assert(Visible == c.Visible, "TrayControl for " + ((TrayControl)c).Component + " should not be positioned");
#endif
            TrayControl tc = (TrayControl)c;
            tc.Positioned = true;
            tc.Location = pos;
        }

        private bool PositionInNextAutoSlot(TrayControl c, Control prevCtl, bool dirtyDesigner)
        {
            Debug.Assert(c.Visible, "TrayControl for " + c.Component + " should not be positioned");
            if (whiteSpace.IsEmpty)
            {
                Debug.Assert(selectionUISvc != null, "No SelectionUIService available for tray.");
                whiteSpace = new Point(selectionUISvc.GetAdornmentDimensions(AdornmentType.GrabHandle));
                whiteSpace.X = whiteSpace.X * 2 + 3;
                whiteSpace.Y = whiteSpace.Y * 2 + 3;
            }

            if (prevCtl == null)
            {
                Rectangle display = DisplayRectangle;
                Point newLoc = new Point(display.X + whiteSpace.X, display.Y + whiteSpace.Y);
                if (!c.Location.Equals(newLoc))
                {
                    c.Location = newLoc;
                    if (dirtyDesigner)
                    {
                        IComponent comp = c.Component;
                        Debug.Assert(comp != null, "Component for the TrayControl is null");
                        PropertyDescriptor ctlLocation = TypeDescriptor.GetProperties(comp)["TrayLocation"];
                        if (ctlLocation != null)
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
                // Calcuate the next location for this control.
                Rectangle bounds = prevCtl.Bounds;
                Point newLoc = new Point(bounds.X + bounds.Width + whiteSpace.X, bounds.Y);

                // Check to see if it goes over the edge of our window.  If it does, then wrap it.
                if (newLoc.X + c.Size.Width > Size.Width)
                {
                    newLoc.X = whiteSpace.X;
                    newLoc.Y += bounds.Height + whiteSpace.Y;
                }

                if (!c.Location.Equals(newLoc))
                {
                    if (dirtyDesigner)
                    {
                        IComponent comp = c.Component;
                        Debug.Assert(comp != null, "Component for the TrayControl is null");
                        PropertyDescriptor ctlLocation = TypeDescriptor.GetProperties(comp)["TrayLocation"];
                        if (ctlLocation != null)
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
            private bool _ctrlSelect = false; // was the ctrl key down on the mouse down?
            private bool _positioned = false; // Have we given this control an explicit location yet?
            private const int WhiteSpace = 5;
            private readonly int _borderWidth;
            internal bool _fRecompute = false; // This flag tells the TrayControl that it needs to retrieve the font and the background color before painting.

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
                Debug.Assert(!CompModSwitches.CommonDesignerServices.Enabled || (cs != null), "IComponentChangeService not found");
                if (cs != null)
                {
                    cs.ComponentRename += new ComponentRenameEventHandler(OnComponentRename);
                }

                ISite site = component.Site;
                string name = null;

                if (site != null)
                {
                    name = site.Name;
                    IDictionaryService ds = (IDictionaryService)site.GetService(typeof(IDictionaryService));
                    Debug.Assert(ds != null, "ComponentTray relies on IDictionaryService, which is not available.");
                    if (ds != null)
                    {
                        ds.SetValue(GetType(), this);
                    }
                }

                if (name == null)
                {
                    // We always want name to have something in it, so we default to the class name.  This way the design instance contains something semi-intuitive if we don't have a site.
                    name = component.GetType().Name;
                }
                Text = name;
                _inheritanceAttribute = (InheritanceAttribute)TypeDescriptor.GetAttributes(component)[typeof(InheritanceAttribute)];
                TabStop = false;

            }

            /// <summary>
            ///  Retrieves the compnent this control is representing.
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
            // CONSIDER: this method gets called three or four times per component, and is even reentrant (CreateGraphics can force handle creation, and OnCreateHandle calls this method).  There's probably a better way to do this, but since this doesn't seem to be on the critical path, I'm not going to lose sleep over it.
            private void AdjustSize()
            {
                // CONSIDER: this forces handle creation.  Can we delay this calculation?
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
                    if (gr != null)
                    {
                        gr.Dispose();
                    }
                }

                if (_tray.glyphManager != null)
                {
                    _tray.glyphManager.UpdateLocation(this);
                }
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
                    if (site != null)
                    {
                        IComponentChangeService cs = (IComponentChangeService)site.GetService(typeof(IComponentChangeService));
                        Debug.Assert(!CompModSwitches.CommonDesignerServices.Enabled || (cs != null), "IComponentChangeService not found");
                        if (cs != null)
                        {
                            cs.ComponentRename -= new ComponentRenameEventHandler(OnComponentRename);
                        }

                        IDictionaryService ds = (IDictionaryService)site.GetService(typeof(IDictionaryService));
                        Debug.Assert(!CompModSwitches.CommonDesignerServices.Enabled || (ds != null), "IDictionaryService not found");
                        if (ds != null)
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
                TrayControl c = null;
                if (component == null)
                {
                    return null;
                }

                ISite site = component.Site;
                if (site != null)
                {
                    IDictionaryService ds = (IDictionaryService)site.GetService(typeof(IDictionaryService));
                    Debug.Assert(!CompModSwitches.CommonDesignerServices.Enabled || (ds != null), "IDictionaryService not found");
                    if (ds != null)
                    {
                        c = (TrayControl)ds.GetValue(typeof(TrayControl));
                    }
                }
                return c;
            }

            /// <summary>
            ///  Delegate that is called in response to a name change.  Here we update our own stashed version of the name, recalcuate our size and repaint.
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
            ///  Overrides handle creation notification for a control.  Here we just ensure that we're the proper size.
            /// </summary>
            protected override void OnHandleCreated(EventArgs e)
            {
                base.OnHandleCreated(e);
                AdjustSize();
            }

            /// <summary>
            ///  Called in response to a double-click of the left mouse button.  The default behavior here calls onDoubleClick on IMouseHandler
            /// </summary>
            protected override void OnDoubleClick(EventArgs e)
            {
                base.OnDoubleClick(e);
                if (!_tray.TabOrderActive)
                {
                    IDesignerHost host = (IDesignerHost)_tray.GetService(typeof(IDesignerHost));
                    Debug.Assert(host != null, "Component tray does not have access to designer host.");
                    if (host != null)
                    {
                        _mouseDragLast = InvalidPoint;
                        Capture = false;
                        // We try to get a designer for the component and let it view the event.  If this fails, then we'll try to do it ourselves.
                        IDesigner designer = host.GetDesigner(_component);
                        if (designer == null)
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
                        if (sel != null)
                        {
                            sel.SetSelectedComponents(new object[] { Component }, SelectionTypes.Primary);
                        }
                        _ctrlSelect = false;
                    }
                    return;
                }
                _mouseDragMoved = false;
                _ctrlSelect = false;
                Capture = false;
                OnSetCursor();

                // And now finish the drag.
                Debug.Assert(_tray.selectionUISvc != null, "We shouldn't be able to begin a drag without this");
                if (_tray.selectionUISvc != null && _tray.selectionUISvc.Dragging)
                {
                    _tray.selectionUISvc.EndDrag(cancel);
                }
            }

            /// <summary>
            ///  Called when the mouse button is pressed down.  Here, we provide drag support for the component.
            /// </summary>
            protected override void OnMouseDown(MouseEventArgs me)
            {
                base.OnMouseDown(me);
                if (!_tray.TabOrderActive)
                {
                    _tray.FocusDesigner();
                    // If this is the left mouse button, then begin a drag.
                    if (me.Button == MouseButtons.Left)
                    {
                        Capture = true;
                        _mouseDragLast = PointToScreen(new Point(me.X, me.Y));
                        // If the CTRL key isn't down, select this component, otherwise, we wait until the mouse up. Make sure the component is selected
                        _ctrlSelect = NativeMethods.GetKeyState((int)Keys.ControlKey) != 0;
                        if (!_ctrlSelect)
                        {
                            ISelectionService sel = (ISelectionService)_tray.GetService(typeof(ISelectionService));
                            // Make sure the component is selected
                            Debug.Assert(!CompModSwitches.CommonDesignerServices.Enabled || (sel != null), "ISelectionService not found");
                            if (sel != null)
                            {
                                sel.SetSelectedComponents(new object[] { Component }, SelectionTypes.Primary);
                            }
                        }
                    }
                }
            }

            /// <summary>
            ///  Called when the mouse is moved over the component.  We update our drag information here if we're dragging the component around.
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
                    if (sel != null)
                    {
                        sel.SetSelectedComponents(new object[] { Component }, SelectionTypes.Primary);
                    }

                    // Notify the selection service that all the components are in the "mouse down" mode.
                    if (_tray.selectionUISvc != null && _tray.selectionUISvc.BeginDrag(SelectionRules.Visible | SelectionRules.Moveable, _mouseDragLast.X, _mouseDragLast.Y))
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
            ///  Called when the mouse button is released.  Here, we finish our drag if one was started.
            /// </summary>
            protected override void OnMouseUp(MouseEventArgs me)
            {
                base.OnMouseUp(me);
                OnEndDrag(false);
            }

            /// <summary>
            ///  Called when we are to display our context menu for this component.
            /// </summary>
            private void OnContextMenu(int x, int y)
            {
                if (!_tray.TabOrderActive)
                {
                    Capture = false;
                    // Ensure that this component is selected.
                    ISelectionService s = (ISelectionService)_tray.GetService(typeof(ISelectionService));
                    if (s != null && !s.GetComponentSelected(_component))
                    {
                        s.SetSelectedComponents(new object[] { _component }, SelectionTypes.Replace);
                    }
                    IMenuCommandService mcs = _tray.MenuService;
                    if (mcs != null)
                    {
                        Capture = false;
                        Cursor.Clip = Rectangle.Empty;
                        mcs.ShowContextMenu(MenuCommands.TraySelectionMenu, x, y);
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
                StringFormat format = new StringFormat();
                Brush foreBrush = new SolidBrush(ForeColor);
                try
                {
                    format.Alignment = StringAlignment.Center;
                    if (_tray.ShowLargeIcons)
                    {
                        if (null != _toolboxBitmap)
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
                        if (null != _toolboxBitmap)
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
                    if (format != null)
                    {
                        format.Dispose();
                    }
                    if (foreBrush != null)
                    {
                        foreBrush.Dispose();
                    }
                }

                // If this component is being inherited, paint it as such
                if (!InheritanceAttribute.NotInherited.Equals(_inheritanceAttribute))
                {
                    InheritanceUI iui = _tray.InheritanceUI;
                    if (iui != null)
                    {
                        e.Graphics.DrawImage(iui.InheritanceGlyph, 0, 0);
                    }
                }
            }

            /// <summary>
            ///  Overrides control's FontChanged.  Here we re-adjust our size if the font changes.
            /// </summary>
            protected override void OnFontChanged(EventArgs e)
            {
                AdjustSize();
                base.OnFontChanged(e);
            }

            /// <summary>
            ///  Overrides control's LocationChanged.  Here, we make sure that any glyphs associated with us are also relocated.
            /// </summary>
            protected override void OnLocationChanged(EventArgs e)
            {
                if (_tray.glyphManager != null)
                {
                    _tray.glyphManager.UpdateLocation(this);
                }
            }

            /// <summary>
            ///  Overrides control's TextChanged.  Here we re-adjust our size if the font changes.
            /// </summary>
            protected override void OnTextChanged(EventArgs e)
            {
                AdjustSize();
                base.OnTextChanged(e);
            }

            /// <summary>
            ///  Called each time the cursor needs to be set.  The ControlDesigner behavior here will set the cursor to one of three things:
            ///  1.  If the selection UI service shows a locked selection, or if there is no location property on the control, then the default arrow will be set.
            ///  2.  Otherwise, the four headed arrow will be set to indicate that the component can be clicked and moved.
            ///  3.  If the user is currently dragging a component, the crosshair cursor will be used instead of the four headed arrow.
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
                    // In case an unhandled exception was encountered, we don't want to leave the cursor with some strange shape Currently we have watson logs with FileNotFoundException only, so we are scoping the catch only to that type.
                    Cursor.Current = Cursors.Default;
                    Debug.Fail(e.Message);
                    return;
                }

                if (prop != null && ((bool)prop.GetValue(_component)) == true)
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

            public override string ToString() => "ComponentTray: " + _component.ToString();

            internal void UpdateIconInfo()
            {
                ToolboxBitmapAttribute attr = (ToolboxBitmapAttribute)TypeDescriptor.GetAttributes(_component)[typeof(ToolboxBitmapAttribute)];
                if (attr != null)
                {
                    _toolboxBitmap = attr.GetImage(_component, _tray.ShowLargeIcons);
                }

                // Get the size of the bitmap so we can size our component correctly.
                if (null == _toolboxBitmap)
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
            ///  This creates a method signature in the source code file for the default event on the component and navigates the user's cursor to that location.
            /// </summary>
            public virtual void ViewDefaultEvent(IComponent component)
            {
                EventDescriptor defaultEvent = TypeDescriptor.GetDefaultEvent(component);
                PropertyDescriptor defaultPropEvent = null;
                bool eventChanged = false;
                IEventBindingService eps = (IEventBindingService)GetService(typeof(IEventBindingService));
                Debug.Assert(!CompModSwitches.CommonDesignerServices.Enabled || (eps != null), "IEventBindingService not found");
                if (eps != null)
                {
                    defaultPropEvent = eps.GetEventProperty(defaultEvent);
                }

                // If we couldn't find a property for this event, or if the property is read only, then abort and just show the code.
                if (defaultPropEvent == null || defaultPropEvent.IsReadOnly)
                {
                    if (eps != null)
                    {
                        eps.ShowCode();
                    }
                    return;
                }

                string handler = (string)defaultPropEvent.GetValue(component);

                // If there is no handler set, set one now.
                if (handler == null)
                {
                    eventChanged = true;
                    handler = eps.CreateUniqueMethodName(component, defaultEvent);
                }
                IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
                DesignerTransaction trans = null;

                try
                {
                    if (host != null)
                    {
                        trans = host.CreateTransaction(string.Format(SR.WindowsFormsAddEvent, defaultEvent.Name));
                    }
                    // Save the new value... BEFORE navigating to it!
                    if (eventChanged && defaultPropEvent != null)
                    {
                        defaultPropEvent.SetValue(component, handler);
                    }
                    eps.ShowCode(component, defaultEvent);
                }
                finally
                {
                    if (trans != null)
                    {
                        trans.Commit();
                    }
                }
            }

            /// <summary>
            ///  This method should be called by the extending designer for each message the control would normally receive.  This allows the designer to pre-process messages before allowing them to be routed to the control.
            /// </summary>
            protected override void WndProc(ref Message m)
            {
                switch (m.Msg)
                {
                    case WindowMessages.WM_SETCURSOR:
                        // We always handle setting the cursor ourselves.
                        OnSetCursor();
                        break;
                    case WindowMessages.WM_CONTEXTMENU:
                        // We must handle this ourselves.  Control only allows regular Windows Forms context menus, which doesn't do us much good.  Also, control's button up processing calls DefwndProc first, which causes a right mouse up to be routed as a WM_CONTEXTMENU.  If we don't respond to it here, this message will be bubbled up to our parent, which would pop up a container context menu instead of our own.
                        int x = NativeMethods.Util.SignedLOWORD(unchecked((int)(long)m.LParam));
                        int y = NativeMethods.Util.SignedHIWORD(unchecked((int)(long)m.LParam));
                        if (x == -1 && y == -1)
                        {
                            // for shift-F10
                            Point mouse = Control.MousePosition;
                            x = mouse.X;
                            y = mouse.Y;
                        }
                        OnContextMenu(x, y);
                        break;
                    case WindowMessages.WM_NCHITTEST:
                        if (_tray.glyphManager != null)
                        {
                            // Make sure tha we send our glyphs hit test messages over the TrayControls too
                            Point pt = new Point((short)NativeMethods.Util.LOWORD(unchecked((int)(long)m.LParam)), (short)NativeMethods.Util.HIWORD(unchecked((int)(long)m.LParam)));
                            var pt1 = new Point();
                            NativeMethods.MapWindowPoints(IntPtr.Zero, Handle, ref pt1, 1);
                            pt.Offset(pt1.X, pt1.Y);
                            pt.Offset(Location.X, Location.Y);//offset the loc of the traycontrol -so now we're in comptray coords
                            _tray.glyphManager.GetHitTest(pt);
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
                readonly ComponentTray _tray;
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
                        if (s != null)
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
            private Adorner _traySelectionAdorner; //we'll use a single adorner to manage the glyphs
            private Glyph _hitTestedGlyph; //the last glyph we hit tested (can be null)
            private readonly ISelectionService _selSvc; //we need the selection service fo r the hover behavior
            private readonly BehaviorService _behaviorSvc;

            /// <summary>
            ///  Constructor that simply creates an empty adorner.
            /// </summary>
            public ComponentTrayGlyphManager(ISelectionService selSvc, BehaviorService behaviorSvc)
            {
                _selSvc = selSvc;
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
            ///  Clears teh adorner of glyphs.
            /// </summary>
            public void Dispose()
            {
                if (_traySelectionAdorner != null)
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
                GlyphCollection glyphs = new GlyphCollection();
                if (_behaviorSvc != null && comp != null)
                {
                    if (_behaviorSvc.DesignerActionUI != null)
                    {
                        Glyph g = _behaviorSvc.DesignerActionUI.GetDesignerActionGlyph(comp);
                        if (g != null)
                        {
                            glyphs.Add(g);
                        }
                    }
                }
                return glyphs;
            }

            /// <summary>
            ///  Called from the tray's NCHITTEST message in the WndProc. We use this to loop through our glyphs and identify which one is successfully hit tested.  From here, we know where to send our messages.
            /// </summary>
            public Cursor GetHitTest(Point p)
            {
                for (int i = 0; i < _traySelectionAdorner.Glyphs.Count; i++)
                {
                    Cursor hitTestCursor = _traySelectionAdorner.Glyphs[i].GetHitTest(p);
                    if (hitTestCursor != null)
                    {
                        _hitTestedGlyph = _traySelectionAdorner.Glyphs[i];
                        return hitTestCursor;
                    }
                }
                _hitTestedGlyph = null;
                return null;
            }

            /// <summary>
            ///  Called when the tray receives this mouse message.  Here,  we'll give our glyphs the first chance to repsond to the message before the tray even sees it.
            /// </summary>
            public bool OnMouseDoubleClick(MouseEventArgs e)
            {
                if (_hitTestedGlyph != null && _hitTestedGlyph.Behavior != null)
                {
                    return _hitTestedGlyph.Behavior.OnMouseDoubleClick(_hitTestedGlyph, e.Button, new Point(e.X, e.Y));
                }
                return false;
            }

            /// <summary>
            ///  Called when the tray receives this mouse message.  Here,  we'll give our glyphs the first chance to repsond to the message before the tray even sees it.
            /// </summary>
            public bool OnMouseDown(MouseEventArgs e)
            {
                if (_hitTestedGlyph != null && _hitTestedGlyph.Behavior != null)
                {
                    return _hitTestedGlyph.Behavior.OnMouseDown(_hitTestedGlyph, e.Button, new Point(e.X, e.Y));
                }
                return false;
            }

            /// <summary>
            ///  Called when the tray receives this mouse message.  Here,  we'll give our glyphs the first chance to repsond to the message before the tray even sees it.
            /// </summary>
            public bool OnMouseMove(MouseEventArgs e)
            {
                if (_hitTestedGlyph != null && _hitTestedGlyph.Behavior != null)
                {
                    return _hitTestedGlyph.Behavior.OnMouseMove(_hitTestedGlyph, e.Button, new Point(e.X, e.Y));
                }
                return false;
            }

            /// <summary>
            ///  Called when the tray receives this mouse message.  Here,  we'll give our glyphs the first chance to repsond to the message before the tray even sees it.
            /// </summary>
            public bool OnMouseUp(MouseEventArgs e)
            {
                if (_hitTestedGlyph != null && _hitTestedGlyph.Behavior != null)
                {
                    return _hitTestedGlyph.Behavior.OnMouseUp(_hitTestedGlyph, e.Button);
                }
                return false;
            }

            /// <summary>
            ///  Called when the comp tray or any tray control paints. This will simply enumerate through the glyphs in our  Adorner and ask them to paint
            /// </summary>
            public void OnPaintGlyphs(PaintEventArgs pe)
            {
                //Paint any glyphs our tray adorner has
                foreach (Glyph g in _traySelectionAdorner.Glyphs)
                {
                    g.Paint(pe);
                }
            }

            /// <summary>
            ///  Called when a tray control's location has changed. We'll loop through our glyphs and invalidate any that are associated with the component.
            /// </summary>
            public void UpdateLocation(TrayControl trayControl)
            {
                foreach (Glyph g in _traySelectionAdorner.Glyphs)
                {
                    //only look at glyphs that derive from designerglyph base (actions)
                    if (g is DesignerActionGlyph desGlyph && ((DesignerActionBehavior)(desGlyph.Behavior)).RelatedComponent.Equals(trayControl.Component))
                    {
                        desGlyph.UpdateAlternativeBounds(trayControl.Bounds);
                    }
                }
            }
        }

        internal class AutoArrangeComparer : IComparer
        {
            int IComparer.Compare(object o1, object o2)
            {
                Debug.Assert(o1 != null && o2 != null, "Null objects sent for comparison!!!");
                Point tcLoc1 = ((Control)o1).Location;
                Point tcLoc2 = ((Control)o2).Location;
                int height = ((Control)o1).Height / 2;
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
                _snapSize = new Size();
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
            ///  Called when the user has completed the drag.  The designer should remove any UI feedback it may be providing.
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
            ///  Given a rectangle, this updates the dimensions of it with any grid snaps and returns a new rectangle.  If no changes to the rectangle's size were needed, this may return the same rectangle.
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
                if (dataObj != null)
                {
                    if (dataObj is ComponentDataObjectWrapper cdow)
                    {
                        ComponentDataObject cdo = (ComponentDataObject)cdow.InnerData;
                        comps = cdo.Components;
                    }
                    else
                    {
                        try
                        {
                            object serializationData = dataObj.GetData(OleDragDropHandler.DataFormat, true);
                            if (serializationData == null)
                            {
                                return false;
                            }

                            IDesignerSerializationService ds = (IDesignerSerializationService)GetService(typeof(IDesignerSerializationService));
                            if (ds == null)
                            {
                                return false;
                            }
                            comps = ds.Deserialize(serializationData);
                        }
                        catch (Exception e)
                        {
                            if (ClientUtils.IsCriticalException(e))
                            {
                                throw;
                            }
                            // we return false on any exception
                        }
                    }
                }

                if (comps != null && comps.Count > 0)
                {
                    foreach (object comp in comps)
                    {
                        if (comp is Point)
                        {
                            continue;
                        }
                        if (comp is Control || !(comp is IComponent))
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
}
