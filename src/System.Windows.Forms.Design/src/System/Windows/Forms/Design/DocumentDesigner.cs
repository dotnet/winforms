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
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms.Design.Behavior;
using Microsoft.Win32;
using static Interop;

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  Provides a designer that extends the ScrollableControlDesigner and implements
    ///  IRootDesigner.
    /// </summary>
    [ToolboxItemFilter("System.Windows.Forms")]
    public partial class DocumentDesigner : ScrollableControlDesigner, IRootDesigner, IToolboxUser, IOleDragClient
    {
        private DesignerFrame frame;
        private ControlCommandSet commandSet;
        private InheritanceService inheritanceService;
        private EventHandlerService eventHandlerService;
        private DesignBindingValueUIHandler designBindingValueUIHandler;
        private BehaviorService behaviorService;
        private SelectionManager selectionManager;
        private DesignerExtenders designerExtenders;
        private InheritanceUI inheritanceUI;
        private PbrsForward pbrsFwd;
        private ArrayList suspendedComponents;
        private UndoEngine undoEngine;
        private bool initializing;   // is the designer initializing?

        //used to keep the state of the tab order view
        //
        private bool queriedTabOrder;
        private MenuCommand tabOrderCommand;

        static internal IDesignerSerializationManager manager;

        // The component tray
        //
        private ComponentTray componentTray;

        private int trayHeight = 80;
        private bool trayLargeIcon;
        private bool trayAutoArrange;
        private bool trayLayoutSuspended;

        // ActiveX support
        //
        private static readonly Guid htmlDesignTime = new Guid("73CEF3DD-AE85-11CF-A406-00AA00C00940");

        private Hashtable axTools;
        private static readonly TraceSwitch AxToolSwitch = new TraceSwitch("AxTool", "ActiveX Toolbox Tracing");
        private const string AxClipFormat = "CLSID";
        private ToolboxItemCreatorCallback toolboxCreator;

        /// <summary>
        ///  Property shadow for ContainerControl's AutoScaleDimensions.  We shadow here so it
        ///  always returns the CurrentAutoScaleDimensions for the control. This way the control's
        ///  state always adapts to the current font / monitor.
        /// </summary>
        private SizeF AutoScaleDimensions
        {
            get
            {
                ContainerControl c = Control as ContainerControl;
                if (c != null)
                {
                    return c.CurrentAutoScaleDimensions;
                }

                Debug.Fail("AutoScaleDimensions should not be shadowed on non-ContainerControl objects.");
                return SizeF.Empty;
            }
            set
            {
                ContainerControl c = Control as ContainerControl;
                if (c != null)
                {
                    c.AutoScaleDimensions = value;
                }
            }
        }

        /// <summary>
        ///  Property shadow for ContainerControl's AutoScaleMode.  We shadow here so it
        ///  never gets to the control; it can be very distracting if you change the font
        ///  and have the form you're designing suddenly move on you.
        /// </summary>
        private AutoScaleMode AutoScaleMode
        {
            get
            {
                ContainerControl c = Control as ContainerControl;
                if (c != null)
                {
                    return c.AutoScaleMode;
                }

                Debug.Fail("AutoScaleMode should not be shadowed on non-ContainerControl objects.");
                return AutoScaleMode.Inherit;
            }
            set
            {
                ShadowProperties[nameof(AutoScaleMode)] = value;
                ContainerControl c = Control as ContainerControl;
                if (c != null && c.AutoScaleMode != value)
                {
                    c.AutoScaleMode = value;

                    // If we're not loading and this changes update
                    // the current auto scale dimensions.
                    IDesignerHost host = GetService(typeof(IDesignerHost)) as IDesignerHost;
                    if (host != null && !host.Loading)
                    {
                        c.AutoScaleDimensions = c.CurrentAutoScaleDimensions;
                    }
                }
            }
        }

        /// <summary>
        ///  BackColor property on control.  We shadow this property at design time.
        /// </summary>
        private Color BackColor
        {
            get
            {
                return Control.BackColor;
            }
            set
            {
                ShadowProperties[nameof(BackColor)] = value;
                if (value.IsEmpty)
                {
                    value = SystemColors.Control;
                }

                Control.BackColor = value;
            }
        }

        /// <summary>
        ///  Location property on control.  We shadow this property at design time.
        /// </summary>
        [DefaultValue(typeof(Point), "0, 0")]
        private Point Location
        {
            get
            {
                return (Point)ShadowProperties[nameof(Location)];
            }
            set
            {
                ShadowProperties[nameof(Location)] = value;
            }
        }

        /// <summary>
        ///  We override our selectino rules to make the document non-sizeable.
        /// </summary>
        public override SelectionRules SelectionRules
        {
            get
            {
                SelectionRules rules = base.SelectionRules;
                rules &= ~(SelectionRules.Moveable | SelectionRules.TopSizeable | SelectionRules.LeftSizeable);

                return rules;
            }
        }

        /// <summary>
        ///  Determines if the tab order UI is active.  When tab order is active, we don't want to forward
        ///  any WndProc messages to the menu editor service (those are all non-selectable components)
        /// </summary>
        private bool TabOrderActive
        {
            get
            {
                if (!queriedTabOrder)
                {
                    queriedTabOrder = true;
                    IMenuCommandService menuCommandService = (IMenuCommandService)GetService(typeof(IMenuCommandService));
                    if (menuCommandService != null)
                        tabOrderCommand = menuCommandService.FindCommand(StandardCommands.TabOrder);
                }

                if (tabOrderCommand != null)
                {
                    return tabOrderCommand.Checked;
                }

                return false;
            }
        }

        /// <summary>
        /// </summary>
        [DefaultValue(true)]
        private bool TrayAutoArrange
        {
            get
            {
                return trayAutoArrange;
            }

            set
            {
                trayAutoArrange = value;
                if (componentTray != null)
                {
                    componentTray.AutoArrange = trayAutoArrange;
                }
            }
        }

        /// <summary>
        /// </summary>
        [DefaultValue(false)]
        private bool TrayLargeIcon
        {
            get
            {
                return trayLargeIcon;
            }

            set
            {
                trayLargeIcon = value;
                if (componentTray != null)
                {
                    componentTray.ShowLargeIcons = trayLargeIcon;
                }
            }
        }

        /// <summary>
        /// </summary>
        [DefaultValue(80)]
        private int TrayHeight
        {
            get
            {
                if (componentTray != null)
                {
                    return componentTray.Height;
                }
                else
                {
                    return trayHeight;
                }
            }

            set
            {
                trayHeight = value;
                if (componentTray != null)
                {
                    componentTray.Height = trayHeight;
                }
            }
        }

        /// <internalonly/>
        /// <summary>
        /// Retrieves the control view instance for the given component.
        /// For Win32 designer, this will often be the component itself.
        /// </summary>
        Control IOleDragClient.GetControlForComponent(object component)
        {
            Control c = GetControl(component);
            if (c != null)
                return c;

            if (componentTray != null)
            {
                return ((IOleDragClient)componentTray).GetControlForComponent(component);
            }

            return null;
        }

        internal virtual bool CanDropComponents(DragEventArgs de)
        {
            // If there is no tray we bail.
            if (componentTray == null)
                return true;

            // Figure out if any of the components in the drag-drop are children
            // of our own tray. If so, we should prevent this drag-drop from proceeding.
            //
            OleDragDropHandler ddh = GetOleDragHandler();
            object[] dragComps = ddh.GetDraggingObjects(de);

            if (dragComps != null)
            {
                IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
                for (int i = 0; i < dragComps.Length; i++)
                {
                    IComponent comp = dragComps[i] as IComponent;
                    if (host == null || dragComps[i] == null || comp == null)
                    {
                        continue;
                    }

                    if (componentTray.IsTrayComponent(comp))
                    {
                        return false;
                    }
                }
            }

            //ToolStripItems cannot be dropped on any ParentControlDesigners since they have custom DataObject Format.
            if (de.Data is ToolStripItemDataObject)
            {
                return false;
            }

            return true;
        }

        private ToolboxItem CreateAxToolboxItem(IDataObject dataObject)
        {
            // Read the stream out of the dataobject and get hold of the CLSID of the Toolbox item.
            //
            MemoryStream stm = (MemoryStream)dataObject.GetData(AxClipFormat, true);
            int len = (int)stm.Length;
            byte[] bytes = new byte[len];
            stm.Read(bytes, 0, len);

            string clsid = Text.Encoding.Default.GetString(bytes);
            int index = clsid.IndexOf("}");
            clsid = clsid.Substring(0, index + 1);
            Debug.WriteLineIf(AxToolSwitch.TraceVerbose, "\tCLSID of the Toolbox item: " + clsid);

            // Look to see if we can find the Control key for this CLSID. If present, create a
            // new AxToolboxItem and add it to the cache.
            //
            if (IsSupportedActiveXControl(clsid))
            {
                AxToolboxItem tool;
                // Look to see if we have already cached the ToolboxItem.
                //
                if (axTools != null)
                {
                    tool = (AxToolboxItem)axTools[clsid];
                    if (tool != null)
                    {
                        if (AxToolSwitch.TraceVerbose)
                            Debug.WriteLine("Found AxToolboxItem in tool cache");
                        return tool;
                    }
                }

                tool = new AxToolboxItem(clsid);
                if (axTools == null)
                {
                    axTools = new Hashtable();
                }

                axTools.Add(clsid, tool);
                Debug.WriteLineIf(AxToolSwitch.TraceVerbose, "\tAdded AxToolboxItem");
                return tool;
            }
            else
            {
                return null;
            }
        }

        private ToolboxItem CreateCfCodeToolboxItem(IDataObject dataObject)
        {
            object serializationData = dataObject.GetData(OleDragDropHandler.NestedToolboxItemFormat, false);
            if (serializationData != null)
            {
                return (ToolboxItem)serializationData;
            }

            serializationData = dataObject.GetData(OleDragDropHandler.DataFormat, false);
            if (serializationData != null)
            {    //backcompat
                return new OleDragDropHandler.CfCodeToolboxItem(serializationData);
                ;
            }

            return null;
        }

        /// <summary>
        ///  Disposes of this designer.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
                Debug.Assert(host != null, "Must have a designer host on dispose");

                if (host != null)
                {
                    //Remove Adorner Window which hosts DropDowns.
                    ToolStripAdornerWindowService toolWindow = (ToolStripAdornerWindowService)GetService(typeof(ToolStripAdornerWindowService));
                    if (toolWindow != null)
                    {
                        toolWindow.Dispose();
                        host.RemoveService(typeof(ToolStripAdornerWindowService));
                    }

                    host.Activated -= new EventHandler(OnDesignerActivate);
                    host.Deactivated -= new EventHandler(OnDesignerDeactivate);

                    // If the tray wasn't destroyed, then we got some sort of imbalance
                    // in our add/remove calls.  Don't sweat it, but do remove the tray.
                    //
                    if (componentTray != null)
                    {
                        ISplitWindowService sws = (ISplitWindowService)GetService(typeof(ISplitWindowService));
                        if (sws != null)
                        {
                            sws.RemoveSplitWindow(componentTray);
                            componentTray.Dispose();
                            componentTray = null;
                        }

                        host.RemoveService(typeof(ComponentTray));
                    }

                    IComponentChangeService cs = (IComponentChangeService)GetService(typeof(IComponentChangeService));
                    if (cs != null)
                    {
                        cs.ComponentAdded -= new ComponentEventHandler(OnComponentAdded);
                        cs.ComponentChanged -= new ComponentChangedEventHandler(OnComponentChanged);
                        cs.ComponentRemoved -= new ComponentEventHandler(OnComponentRemoved);
                    }

                    if (undoEngine != null)
                    {
                        undoEngine.Undoing -= new EventHandler(OnUndoing);
                        undoEngine.Undone -= new EventHandler(OnUndone);
                    }

                    if (toolboxCreator != null)
                    {
                        IToolboxService toolbox = (IToolboxService)GetService(typeof(IToolboxService));
                        Debug.Assert(toolbox != null, "No toolbox service available");
                        if (toolbox != null)
                        {
                            Debug.WriteLineIf(AxToolSwitch.TraceVerbose, "Removing DocumentDesigner as CLSID ToolboxItemCreator");
                            toolbox.RemoveCreator(AxClipFormat, host);

                            Debug.WriteLineIf(AxToolSwitch.TraceVerbose, "Removing DocumentDesigner as CF_CODE ToolboxItemCreator");
                            toolbox.RemoveCreator(OleDragDropHandler.DataFormat, host);

                            Debug.WriteLineIf(AxToolSwitch.TraceVerbose, "Removing DocumentDesigner as CF_TOOLBOXITEM ToolboxItemCreator");
                            toolbox.RemoveCreator(OleDragDropHandler.NestedToolboxItemFormat, host);
                        }

                        toolboxCreator = null;
                    }
                }

                ISelectionService ss = (ISelectionService)GetService(typeof(ISelectionService));
                if (ss != null)
                {
                    ss.SelectionChanged -= new EventHandler(OnSelectionChanged);
                }

                if (behaviorService != null)
                {
                    behaviorService.Dispose();
                    behaviorService = null;
                }

                if (selectionManager != null)
                {
                    selectionManager.Dispose();
                    selectionManager = null;
                }

                if (componentTray != null)
                {
                    if (host != null)
                    {
                        ISplitWindowService sws = (ISplitWindowService)GetService(typeof(ISplitWindowService));
                        if (sws != null)
                        {
                            sws.RemoveSplitWindow(componentTray);
                        }
                    }

                    componentTray.Dispose();
                    componentTray = null;
                }

                if (pbrsFwd != null)
                {
                    pbrsFwd.Dispose();
                    pbrsFwd = null;
                }

                if (frame != null)
                {
                    frame.Dispose();
                    frame = null;
                }

                if (commandSet != null)
                {
                    commandSet.Dispose();
                    commandSet = null;
                }

                if (inheritanceService != null)
                {
                    inheritanceService.Dispose();
                    inheritanceService = null;
                }

                if (inheritanceUI != null)
                {
                    inheritanceUI.Dispose();
                    inheritanceUI = null;
                }

                if (designBindingValueUIHandler != null)
                {
                    IPropertyValueUIService pvUISvc = (IPropertyValueUIService)GetService(typeof(IPropertyValueUIService));
                    if (pvUISvc != null)
                    {
                        pvUISvc.RemovePropertyValueUIHandler(new PropertyValueUIHandler(designBindingValueUIHandler.OnGetUIValueItem));
                        pvUISvc = null;
                    }

                    designBindingValueUIHandler.Dispose();
                    designBindingValueUIHandler = null;
                }

                if (designerExtenders != null)
                {
                    designerExtenders.Dispose();
                    designerExtenders = null;
                }

                if (axTools != null)
                {
                    axTools.Clear();
                }

                if (host != null)
                {
                    host.RemoveService(typeof(BehaviorService));
                    host.RemoveService(typeof(ToolStripAdornerWindowService));
                    host.RemoveService(typeof(SelectionManager));
                    host.RemoveService(typeof(IInheritanceService));
                    host.RemoveService(typeof(IEventHandlerService));
                    host.RemoveService(typeof(IOverlayService));
                    host.RemoveService(typeof(ISplitWindowService));
                    host.RemoveService(typeof(InheritanceUI));
                }
            }

            base.Dispose(disposing);
        }

        /// <summary>
        ///  Returns an array of Glyph objects representing the selection
        ///  borders and grab handles for the related Component.  Note that
        ///  based on 'selType' the Glyphs returned will either: represent
        ///  a fully resizeable selection border with grab handles, a locked
        ///  selection border, or a single 'hidden' selection Glyph.
        /// </summary>
        public override GlyphCollection GetGlyphs(GlyphSelectionType selectionType)
        {
            GlyphCollection glyphs = new GlyphCollection();

            if (selectionType != GlyphSelectionType.NotSelected)
            {
                Point loc = BehaviorService.ControlToAdornerWindow((Control)Component);
                Rectangle translatedBounds = new Rectangle(loc, ((Control)Component).Size);
                bool primarySelection = (selectionType == GlyphSelectionType.SelectedPrimary);

                bool locked = false;
                PropertyDescriptor prop = TypeDescriptor.GetProperties(Component)["Locked"];
                if (prop != null)
                {
                    locked = (bool)prop.GetValue(Component);
                }

                bool autoSize = false;
                prop = TypeDescriptor.GetProperties(Component)["AutoSize"];
                if (prop != null)
                {
                    autoSize = (bool)prop.GetValue(Component);
                }

                AutoSizeMode mode = AutoSizeMode.GrowOnly;
                prop = TypeDescriptor.GetProperties(Component)["AutoSizeMode"];
                if (prop != null)
                {
                    mode = (AutoSizeMode)prop.GetValue(Component);
                }

                SelectionRules rules = SelectionRules;

                if (locked)
                {
                    // the lock glyph
                    glyphs.Add(new LockedHandleGlyph(translatedBounds, primarySelection));

                    //the four locked border glyphs
                    glyphs.Add(new LockedBorderGlyph(translatedBounds, SelectionBorderGlyphType.Top));
                    glyphs.Add(new LockedBorderGlyph(translatedBounds, SelectionBorderGlyphType.Bottom));
                    glyphs.Add(new LockedBorderGlyph(translatedBounds, SelectionBorderGlyphType.Left));
                    glyphs.Add(new LockedBorderGlyph(translatedBounds, SelectionBorderGlyphType.Right));
                }

                // we check if the control is a form because we only want to disable resizing
                // for components that are not Forms.
                else if (autoSize && (mode == AutoSizeMode.GrowAndShrink) && !(Control is Form))
                {
                    //the non-resizeable grab handle
                    glyphs.Add(new NoResizeHandleGlyph(translatedBounds, rules, primarySelection, null));

                    //the four resizeable border glyphs
                    glyphs.Add(new NoResizeSelectionBorderGlyph(translatedBounds, rules, SelectionBorderGlyphType.Top, null));
                    glyphs.Add(new NoResizeSelectionBorderGlyph(translatedBounds, rules, SelectionBorderGlyphType.Bottom, null));
                    glyphs.Add(new NoResizeSelectionBorderGlyph(translatedBounds, rules, SelectionBorderGlyphType.Left, null));
                    glyphs.Add(new NoResizeSelectionBorderGlyph(translatedBounds, rules, SelectionBorderGlyphType.Right, null));
                }

                else
                {
                    glyphs.Add(new GrabHandleGlyph(translatedBounds, GrabHandleGlyphType.MiddleRight, StandardBehavior, primarySelection));
                    glyphs.Add(new GrabHandleGlyph(translatedBounds, GrabHandleGlyphType.LowerRight, StandardBehavior, primarySelection));
                    glyphs.Add(new GrabHandleGlyph(translatedBounds, GrabHandleGlyphType.MiddleBottom, StandardBehavior, primarySelection));
                    glyphs.Add(new SelectionBorderGlyph(translatedBounds, rules, SelectionBorderGlyphType.Top, null));
                    glyphs.Add(new SelectionBorderGlyph(translatedBounds, rules, SelectionBorderGlyphType.Bottom, StandardBehavior));
                    glyphs.Add(new SelectionBorderGlyph(translatedBounds, rules, SelectionBorderGlyphType.Left, null));
                    glyphs.Add(new SelectionBorderGlyph(translatedBounds, rules, SelectionBorderGlyphType.Right, StandardBehavior));
                }
            }

            return glyphs;
        }

        /// <summary>
        ///  Examines the current selection for a suitable frame designer.  This
        ///  is used when we are creating a new component so we know what control
        ///  to parent the component to.  This will always return a frame designer,
        ///  and may walk all the way up the control parent chain to this designer
        ///  if it needs to.
        /// </summary>
        private ParentControlDesigner GetSelectedParentControlDesigner()
        {
            ISelectionService s = (ISelectionService)GetService(typeof(ISelectionService));
            ParentControlDesigner parentControlDesigner = null;

            if (s != null)
            {
                // We first try the primary selection.  If that is null
                // or isn't a Control, we then walk the set of selected
                // objects.  Failing all of this, we default to us.
                //
                object sel = s.PrimarySelection;

                if (sel == null || !(sel is Control))
                {
                    sel = null;

                    ICollection sels = s.GetSelectedComponents();

                    foreach (object obj in sels)
                    {
                        if (obj is Control)
                        {
                            sel = obj;
                            break;
                        }
                    }
                }

                if (sel != null)
                {
                    // Now that we have our currently selected component
                    // we can walk up the parent chain looking for a frame
                    // designer.
                    //
                    Debug.Assert(sel is Control, "Our logic is flawed - sel should be a Control");
                    Control c = (Control)sel;

                    IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));

                    if (host != null)
                    {
                        while (c != null)
                        {
                            ParentControlDesigner designer = host.GetDesigner(c) as ParentControlDesigner;

                            if (designer != null)
                            {
                                parentControlDesigner = designer;
                                break;
                            }

                            c = c.Parent;
                        }
                    }
                }
            }

            if (parentControlDesigner == null)
            {
                parentControlDesigner = this;
            }

            return parentControlDesigner;
        }

        /// <summary>
        ///  Determines if the given tool is supported by this designer.
        ///  If a tool is supported then it will be enabled in the toolbox
        ///  when this designer regains focus.  Otherwise, it will be disabled.
        ///  Once a tool is marked as enabled or disabled it may not be
        ///  queried again.
        /// </summary>
        protected virtual bool GetToolSupported(ToolboxItem tool)
        {
            return true;
        }

        /// <summary>
        ///  Initializes the designer with the given component.  The designer can
        ///  get the component's site and request services from it in this call.
        /// </summary>
        public override void Initialize(IComponent component)
        {
            initializing = true;
            base.Initialize(component);
            initializing = false;

            // If the back color of the control has not been established, force it to be
            // "Control" so it doesn't walk up the parent chain and grab the document window's
            // back color.
            //
            PropertyDescriptor backProp = TypeDescriptor.GetProperties(Component.GetType())["BackColor"];
            if (backProp != null && backProp.PropertyType == typeof(Color) && !backProp.ShouldSerializeValue(Component))
            {
                Control.BackColor = SystemColors.Control;
            }

            IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
            IExtenderProviderService exps = (IExtenderProviderService)GetService(typeof(IExtenderProviderService));
            if (exps != null)
            {
                designerExtenders = new DesignerExtenders(exps);
            }

            if (host != null)
            {
                host.Activated += new EventHandler(OnDesignerActivate);
                host.Deactivated += new EventHandler(OnDesignerDeactivate);

                ServiceCreatorCallback callback = new ServiceCreatorCallback(OnCreateService);
                host.AddService(typeof(IEventHandlerService), callback);

                // M3.2 CONTROL ARRAY IS CUT

                // Create the view for this component. We first create the designer frame so we can provide
                // the overlay and split window services, and then later on we initilaize the frame with
                // the designer view.
                //
                frame = new DesignerFrame(component.Site);

                IOverlayService os = frame;
                host.AddService(typeof(IOverlayService), os);
                host.AddService(typeof(ISplitWindowService), frame);

                behaviorService = new BehaviorService(Component.Site, frame);
                host.AddService(typeof(BehaviorService), behaviorService);

                selectionManager = new SelectionManager(host, behaviorService);

                host.AddService(typeof(SelectionManager), selectionManager);
                host.AddService(typeof(ToolStripAdornerWindowService), callback);

                // And component add and remove events for our tray
                //
                IComponentChangeService cs = (IComponentChangeService)GetService(typeof(IComponentChangeService));
                if (cs != null)
                {
                    cs.ComponentAdded += new ComponentEventHandler(OnComponentAdded);
                    cs.ComponentChanged += new ComponentChangedEventHandler(OnComponentChanged);
                    cs.ComponentRemoved += new ComponentEventHandler(OnComponentRemoved);
                }

                // We must do the inheritance scan early, but not so early that we haven't hooked events
                // to handle invisible components.  We also use the variable "inheritanceService"
                // as a check in OnCreateHandle -- we cannot call base.OnCreateHandle if we have
                // not done an inheritance scan yet, because this will cause the base control
                // class to hook all of the controls we may want to inherit.  So, we do the
                // scan, assign the variable, and then call OnCreateHandle if needed.
                //
                inheritanceUI = new InheritanceUI();
                host.AddService(typeof(InheritanceUI), inheritanceUI);

                InheritanceService isvc = new DocumentInheritanceService(this);
                host.AddService(typeof(IInheritanceService), isvc);

                manager = host.GetService(typeof(IDesignerSerializationManager)) as IDesignerSerializationManager;
                isvc.AddInheritedComponents(component, component.Site.Container);
                manager = null;

                inheritanceService = isvc;
                if (Control.IsHandleCreated)
                {
                    OnCreateHandle();
                }

                IPropertyValueUIService pvUISvc = (IPropertyValueUIService)component.Site.GetService(typeof(IPropertyValueUIService));

                if (pvUISvc != null)
                {
                    designBindingValueUIHandler = new DesignBindingValueUIHandler();
                    pvUISvc.AddPropertyValueUIHandler(new PropertyValueUIHandler(designBindingValueUIHandler.OnGetUIValueItem));
                }

                // Add the DocumentDesigner as a creator of CLSID toolbox items.
                //
                IToolboxService toolbox = (IToolboxService)host.GetService(typeof(IToolboxService));
                if (toolbox != null)
                {
                    Debug.WriteLineIf(AxToolSwitch.TraceVerbose, "Adding DocumentDesigner as CLSID ToolboxItemCreator");
                    toolboxCreator = new ToolboxItemCreatorCallback(OnCreateToolboxItem);
                    toolbox.AddCreator(toolboxCreator, AxClipFormat, host);
                    toolbox.AddCreator(toolboxCreator, OleDragDropHandler.DataFormat, host);
                    toolbox.AddCreator(toolboxCreator, OleDragDropHandler.NestedToolboxItemFormat, host);
                }

                // Listen for the completed load.  When finished, we need to select the form.  We don'
                // want to do it before we're done, however, or else the dimensions of the selection rectangle
                // could be off because during load, change events are not fired.
                //
                host.LoadComplete += new EventHandler(OnLoadComplete);
            }

            // Setup our menu command structure.
            //
            Debug.Assert(component.Site != null, "Designer host should have given us a site by now.");
            commandSet = new ControlCommandSet(component.Site);

            // Finally hook the designer view into the frame.  We do this last because the frame may
            // cause the control to be created, and if this happens before the inheritance scan we
            // will subclass the inherited controls before we get a chance to attach designers.
            //
            frame.Initialize(Control);
            pbrsFwd = new PbrsForward(frame, component.Site);

            // And force some shadow properties that we change in the course of
            // initializing the form.
            //
            Location = new Point(0, 0);
        }

        /// <summary>
        ///  Checks to see if the give CLSID is an ActiveX control
        ///  that we support.  This ignores designtime controls.
        /// </summary>
        private bool IsSupportedActiveXControl(string clsid)
        {
            RegistryKey key = null;
            RegistryKey designtimeKey = null;

            try
            {
                string controlKey = "CLSID\\" + clsid + "\\Control";
                key = Registry.ClassesRoot.OpenSubKey(controlKey);
                if (key != null)
                {
                    // ASURT 36817:
                    // We are not going to support design-time controls for this revision. We use the guids under
                    // HKCR\Component Categories to decide which categories to avoid. Currently the only one is
                    // the "HTML Design-time Control" category implemented by VID controls.
                    //
                    string category = "CLSID\\" + clsid + "\\Implemented Categories\\{" + htmlDesignTime.ToString() + "}";
                    designtimeKey = Registry.ClassesRoot.OpenSubKey(category);
                    return (designtimeKey == null);
                }

                return false;
            }
            finally
            {
                if (key != null)
                    key.Close();

                if (designtimeKey != null)
                    designtimeKey.Close();
            }
        }

        private void OnUndone(object source, EventArgs e)
        {
            //resume all suspended comps we found earlier
            if (suspendedComponents != null)
            {
                foreach (Control c in suspendedComponents)
                {
                    c.ResumeLayout(false/*performLayout*/);
                    c.PerformLayout();
                }
            }
        }

        private void OnUndoing(object source, EventArgs e)
        {
            // attempt to suspend all components within the icontainer
            // plus the root component's parent.
            //
            IDesignerHost host = GetService(typeof(IDesignerHost)) as IDesignerHost;
            if (host != null)
            {
                IContainer container = host.Container;
                if (container != null)
                {
                    suspendedComponents = new ArrayList(container.Components.Count + 1);

                    foreach (IComponent comp in container.Components)
                    {
                        Control control = comp as Control;
                        if (control != null)
                        {
                            control.SuspendLayout();
                            //add this control to our suspended components list so we can resume
                            //later
                            suspendedComponents.Add(control);
                        }
                    }

                    // Also suspend the rooot component's parent.
                    Control root = host.RootComponent as Control;
                    if (root != null)
                    {
                        Control rootParent = root.Parent;
                        if (rootParent != null)
                        {
                            rootParent.SuspendLayout();
                            suspendedComponents.Add(rootParent);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///  Called when a component is added to the design container.
        ///  If the component isn't a control, this will demand create
        ///  the component tray and add the component to it.
        /// </summary>
        private void OnComponentAdded(object source, ComponentEventArgs ce)
        {
            IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
            if (host != null)
            {
                IComponent component = ce.Component;

                bool addControl = true;

                // This is the mirror to logic in ParentControlDesigner.  The component should be
                // added somewhere, and this logic decides where.
                //

                // LETS SEE IF WE ARE TOOLSTRIP in which case we want to get added
                // to the componentTray even though this is a control..
                // We should think of implementing an interface so that we can have many more
                // controls behaving like this.
                //
                ToolStripDesigner td = host.GetDesigner(component) as ToolStripDesigner;

                if (td == null)
                {
                    ControlDesigner cd = host.GetDesigner(component) as ControlDesigner;
                    if (cd != null)
                    {
                        Form form = cd.Control as Form;
                        if (form == null || !form.TopLevel)
                        {
                            addControl = false;
                        }
                    }
                }

                if (addControl &&
                    TypeDescriptor.GetAttributes(component).Contains(DesignTimeVisibleAttribute.Yes))
                {
                    if (componentTray == null)
                    {
                        ISplitWindowService sws = (ISplitWindowService)GetService(typeof(ISplitWindowService));
                        if (sws != null)
                        {
                            componentTray = new ComponentTray(this, Component.Site);
                            sws.AddSplitWindow(componentTray);

                            componentTray.Height = trayHeight;
                            componentTray.ShowLargeIcons = trayLargeIcon;
                            componentTray.AutoArrange = trayAutoArrange;

                            host.AddService(typeof(ComponentTray), componentTray);
                        }
                    }

                    if (componentTray != null)
                    {
                        // Suspend the layout of the tray through the loading
                        // process. This way, we won't continuously try to layout
                        // components in auto arrange mode. We will instead let
                        // the controls restore themselves to their persisted state
                        // and then let auto-arrange kick in once.
                        //
                        if (host != null && host.Loading && !trayLayoutSuspended)
                        {
                            trayLayoutSuspended = true;
                            componentTray.SuspendLayout();
                        }

                        componentTray.AddComponent(component);
                    }
                }
            }
        }

        /// <summary>
        ///  Called when a component is removed from the design container.
        ///  Here we check to see there are no more components on the tray
        ///  and if not, we will remove the tray.
        /// </summary>
        private void OnComponentRemoved(object source, ComponentEventArgs ce)
        {
            // ToolStrip is designableAsControl but has a ComponentTray Entry ... so special case it out.
            bool designableAsControl = (ce.Component is Control && !(ce.Component is ToolStrip)) && !(ce.Component is Form && ((Form)ce.Component).TopLevel);
            if (!designableAsControl && componentTray != null)
            {
                componentTray.RemoveComponent(ce.Component);

                if (componentTray.ComponentCount == 0)
                {
                    ISplitWindowService sws = (ISplitWindowService)GetService(typeof(ISplitWindowService));
                    if (sws != null)
                    {
                        sws.RemoveSplitWindow(componentTray);
                        IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
                        if (host != null)
                        {
                            host.RemoveService(typeof(ComponentTray));
                        }

                        componentTray.Dispose();
                        componentTray = null;
                    }
                }
            }
        }

        /// <summary>
        ///  Called when the context menu should be displayed.  This displays the document
        ///  context menu.
        /// </summary>
        protected override void OnContextMenu(int x, int y)
        {
            IMenuCommandService mcs = (IMenuCommandService)GetService(typeof(IMenuCommandService));
            if (mcs != null)
            {
                ISelectionService selSvc = (ISelectionService)GetService(typeof(ISelectionService));
                if (selSvc != null)
                {
                    // Here we check to see if we're the only component selected.  If not, then
                    // we'll display the standard component menu.
                    //
                    if (selSvc.SelectionCount == 1 && selSvc.GetComponentSelected(Component))
                    {
                        mcs.ShowContextMenu(MenuCommands.ContainerMenu, x, y);
                    }

                    // Try to see if the component selected has a contextMenuStrip to show
                    else
                    {
                        Component selComp = selSvc.PrimarySelection as Component;
                        if (selComp != null)
                        {
                            IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
                            if (host != null)
                            {
                                ComponentDesigner compDesigner = host.GetDesigner(selComp) as ComponentDesigner;
                                if (compDesigner != null)
                                {
                                    compDesigner.ShowContextMenu(x, y);
                                    return;
                                }
                            }
                        }

                        mcs.ShowContextMenu(MenuCommands.SelectionMenu, x, y);
                    }
                }
            }
        }

        /// <summary>
        ///  This is called immediately after the control handle has been created.
        /// </summary>
        protected override void OnCreateHandle()
        {
            // Don't call base unless our inheritance serivce is already running.
            if (inheritanceService != null)
            {
                base.OnCreateHandle();
            }
        }

        /// <summary>
        ///  Creates some of the more infrequently used services we offer.
        /// </summary>
        private object OnCreateService(IServiceContainer container, Type serviceType)
        {
            if (serviceType == typeof(IEventHandlerService))
            {
                if (eventHandlerService == null)
                {
                    eventHandlerService = new EventHandlerService(frame);
                }

                return eventHandlerService;
            }
            else if (serviceType == typeof(ToolStripAdornerWindowService))
            {
                return new ToolStripAdornerWindowService(Component.Site, frame);
            }

            Debug.Fail("Called back to create a service we don't know how to create: " + serviceType.Name);
            return null;
        }

        /// <summary>
        ///  Called when our document becomes active.  We paint our form's
        ///  border the appropriate color here.
        /// </summary>
        private ToolboxItem OnCreateToolboxItem(object serializedData, string format)
        {
            Debug.WriteLineIf(AxToolSwitch.TraceVerbose, "Checking to see if: " + format + " is supported.");

            IDataObject dataObject = serializedData as IDataObject;

            if (dataObject == null)
            {
                Debug.Fail("Toolbox service didn't pass us a data object; that should never happen");
                return null;
            }

            // CLSID format.
            //
            if (format.Equals(AxClipFormat))
            {
                return CreateAxToolboxItem(dataObject);
            }

            // CF_CODE format
            //
            if (format.Equals(OleDragDropHandler.DataFormat) ||
                format.Equals(OleDragDropHandler.NestedToolboxItemFormat))
            {
                return CreateCfCodeToolboxItem(dataObject);
            }

            return null;
        }

        /// <summary>
        ///  Called when our document becomes active.  Here we try to
        ///  select the appropriate toolbox tab.
        /// </summary>
        private void OnDesignerActivate(object source, EventArgs evevent)
        {
            if (undoEngine == null)
            {
                undoEngine = GetService(typeof(UndoEngine)) as UndoEngine;
                if (undoEngine != null)
                {
                    undoEngine.Undoing += new EventHandler(OnUndoing);
                    undoEngine.Undone += new EventHandler(OnUndone);
                }
            }
        }

        /// <summary>
        ///  Called by the host when we become inactive.  Here we update the
        ///  title bar of our form so it's the inactive color.
        /// </summary>
        private unsafe void OnDesignerDeactivate(object sender, EventArgs e)
        {
            Control control = Control;
            if (control != null && control.IsHandleCreated)
            {
                User32.SendMessageW(control.Handle, User32.WM.NCACTIVATE);
                User32.RedrawWindow(control.Handle, null, IntPtr.Zero, User32.RDW.FRAME);
            }
        }

        /// <summary>
        ///  Called when the designer is finished loading.  Here we select the form.
        /// </summary>
        private void OnLoadComplete(object sender, EventArgs e)
        {
            // Remove the handler; we're done looking at this.
            //
            ((IDesignerHost)sender).LoadComplete -= new EventHandler(OnLoadComplete);

            // Restore the tray layout.
            //
            if (trayLayoutSuspended && componentTray != null)
                componentTray.ResumeLayout();

            // Select this component.
            //
            ISelectionService ss = (ISelectionService)GetService(typeof(ISelectionService));
            if (ss != null)
            {
                ss.SelectionChanged += new EventHandler(OnSelectionChanged);
                ss.SetSelectedComponents(new object[] { Component }, SelectionTypes.Replace);
                Debug.Assert(ss.PrimarySelection == Component, "Bug in selection service:  form should have primary selection.");
            }
        }

        private void OnComponentChanged(object sender, ComponentChangedEventArgs e)
        {
            Control ctrl = e.Component as Control;
            if (ctrl != null && ctrl.IsHandleCreated)
            {
                User32.NotifyWinEvent((int)AccessibleEvents.LocationChange, new HandleRef(ctrl, ctrl.Handle), User32.OBJID.CLIENT, 0);
                if (frame.Focused)
                {
                    User32.NotifyWinEvent((int)AccessibleEvents.Focus, new HandleRef(ctrl, ctrl.Handle), User32.OBJID.CLIENT, 0);
                }
            }
        }

        /// <summary>
        ///  Called by the selection service when the selection has changed.  We do a number
        ///  of selection-related things here.
        /// </summary>
        private void OnSelectionChanged(object sender, EventArgs e)
        {
            ISelectionService svc = (ISelectionService)GetService(typeof(ISelectionService));
            if (svc != null)
            {
                ICollection selComponents = svc.GetSelectedComponents();

                // Setup the correct active accessibility selection / focus data
                //
                Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo, "MSAA: SelectionChanged");
                foreach (object selObj in selComponents)
                {
                    Control c = selObj as Control;
                    if (c != null)
                    {
                        Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo, "MSAA: SelectionAdd, control = " + c.ToString());
                        User32.NotifyWinEvent((int)AccessibleEvents.SelectionAdd, new HandleRef(c, c.Handle), User32.OBJID.CLIENT, 0);
                    }
                }

                Control primary = svc.PrimarySelection as Control;
                if (primary != null)
                {
                    Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo, "MSAA: Focus, control = " + primary.ToString());
                    User32.NotifyWinEvent((int)AccessibleEvents.Focus, new HandleRef(primary, primary.Handle), User32.OBJID.CLIENT, 0);
                }

                // see if there are visual controls selected.  If so, we add a context attribute.
                // Otherwise, we remove the attribute.  We do not count the form.
                //
                IHelpService hs = (IHelpService)GetService(typeof(IHelpService));

                if (hs != null)
                {
                    ushort type = 0;
                    string[] types = new string[]
                    {
                        "VisualSelection",
                        "NonVisualSelection",
                        "MixedSelection"
                    };

                    foreach (object obj in selComponents)
                    {
                        if (obj is Control)
                        {
                            if (obj != Component)
                            {
                                type |= 1;
                            }
                        }
                        else
                        {
                            type |= 2;
                        }

                        if (type == 3)
                        {
                            break;
                        }
                    }

                    // Remove any prior attribute
                    //
                    for (int i = 0; i < types.Length; i++)
                    {
                        hs.RemoveContextAttribute("Keyword", types[i]);
                    }

                    if (type != 0)
                    {
                        hs.AddContextAttribute("Keyword", types[type - 1], HelpKeywordType.GeneralKeyword);
                    }
                }
            }
        }

        /// <summary>
        ///  Allows a designer to filter the set of properties
        ///  the component it is designing will expose through the
        ///  TypeDescriptor object.  This method is called
        ///  immediately before its corresponding "Post" method.
        ///  If you are overriding this method you should call
        ///  the base implementation before you perform your own
        ///  filtering.
        /// </summary>
        protected override void PreFilterProperties(IDictionary properties)
        {
            PropertyDescriptor prop;

            base.PreFilterProperties(properties);

            // Add any properties that the Tray will persist.
            properties["TrayHeight"] = TypeDescriptor.CreateProperty(typeof(DocumentDesigner), "TrayHeight", typeof(int),
                                                            BrowsableAttribute.No,
                                                            DesignOnlyAttribute.Yes,
                                                            new SRDescriptionAttribute("FormDocumentDesignerTraySizeDescr"),
                                                            CategoryAttribute.Design);

            properties["TrayLargeIcon"] = TypeDescriptor.CreateProperty(typeof(DocumentDesigner), "TrayLargeIcon", typeof(bool),
                                                               BrowsableAttribute.No,
                                                               DesignOnlyAttribute.Yes,
                                                               CategoryAttribute.Design);

            // Expose the doublebuffered property on control
            properties["DoubleBuffered"] = TypeDescriptor.CreateProperty(typeof(Control), "DoubleBuffered", typeof(bool),
                                                                BrowsableAttribute.Yes,
                                                                DesignOnlyAttribute.No);

            // Handle shadowed properties
            //
            string[] shadowProps = new string[]
            {
                "Location",
                "BackColor"
            };

            string[] noBrowseProps = new string[]
            {
                "Anchor",
                "Dock",
                "TabIndex",
                "TabStop",
                "Visible"
            };

            Attribute[] empty = Array.Empty<Attribute>();

            for (int i = 0; i < shadowProps.Length; i++)
            {
                prop = (PropertyDescriptor)properties[shadowProps[i]];
                if (prop != null)
                {
                    properties[shadowProps[i]] = TypeDescriptor.CreateProperty(typeof(DocumentDesigner), prop, empty);
                }
            }

            prop = (PropertyDescriptor)properties["AutoScaleDimensions"];
            if (prop != null)
            {
                properties["AutoScaleDimensions"] = TypeDescriptor.CreateProperty(typeof(DocumentDesigner), prop, DesignerSerializationVisibilityAttribute.Visible);
            }

            prop = (PropertyDescriptor)properties["AutoScaleMode"];
            if (prop != null)
            {
                properties["AutoScaleMode"] = TypeDescriptor.CreateProperty(typeof(DocumentDesigner), prop, DesignerSerializationVisibilityAttribute.Visible, BrowsableAttribute.Yes);
            }

            for (int i = 0; i < noBrowseProps.Length; i++)
            {
                prop = (PropertyDescriptor)properties[noBrowseProps[i]];
                if (prop != null)
                {
                    properties[noBrowseProps[i]] = TypeDescriptor.CreateProperty(prop.ComponentType, prop, BrowsableAttribute.No, DesignerSerializationVisibilityAttribute.Hidden);
                }
            }
        }

        /// <summary>
        ///  Resets the back color to be based on the parent's back color.
        /// </summary>
        private void ResetBackColor()
        {
            BackColor = Color.Empty;
        }

        private bool ShouldSerializeAutoScaleDimensions()
        {
            // Visual inheritance always adds default
            // value attributes that adopt the current values. This
            // isn't right for auto scale, however,because we always
            // want to write out the auto scale values.  So, we have
            // to be a bit sleazy here and trick the inheritance engine
            // to think that these properties currently have their
            // default values.
            return !initializing && AutoScaleMode != AutoScaleMode.None && AutoScaleMode != AutoScaleMode.Inherit;
        }

        private bool ShouldSerializeAutoScaleMode()
        {
            // Visual inheritance always adds default
            // value attributes that adopt the current values. This
            // isn't right for auto scale, however,because we always
            // want to write out the auto scale values.  So, we have
            // to be a bit sleazy here and trick the inheritance engine
            // to think that these properties currently have their
            // default values.
            return !initializing && ShadowProperties.Contains(nameof(AutoScaleMode));
        }

        /// <summary>
        ///  Returns true if the BackColor property should be persisted in code gen.
        /// </summary>
        private bool ShouldSerializeBackColor()
        {
            // We push Color.Empty into our shadow cash during
            // init and also whenever we are reset.  We do this
            // so we can push a real color into the controls
            // back color to stop it from walking the parent chain.
            // But, we want it to look like we didn't push a color
            // so code gen won't write out "Color.Control"
            if (!ShadowProperties.Contains(nameof(BackColor)) || ((Color)ShadowProperties[nameof(BackColor)]).IsEmpty)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        ///  This will be called when the user double-clicks on a
        ///  toolbox item.  The document designer should create
        ///  a component for the given tool.
        /// </summary>
        protected virtual void ToolPicked(ToolboxItem tool)
        {
            // If the tab order UI is showing, don't allow us to place a tool.
            //
            IMenuCommandService mcs = (IMenuCommandService)GetService(typeof(IMenuCommandService));
            if (mcs != null)
            {
                MenuCommand cmd = mcs.FindCommand(StandardCommands.TabOrder);
                if (cmd != null && cmd.Checked)
                {
                    return;
                }
            }

            IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
            if (host != null)
            {
                host.Activate();
            }

            // Just find the currently selected frame designer and ask it to create the tool.
            //
            try
            {
                ParentControlDesigner designer = GetSelectedParentControlDesigner();
                if (!InvokeGetInheritanceAttribute(designer).Equals(InheritanceAttribute.InheritedReadOnly))
                {
                    InvokeCreateTool(designer, tool);
                    IToolboxService toolboxService = (IToolboxService)GetService(typeof(IToolboxService));
                    if (toolboxService != null)
                    {
                        toolboxService.SelectedToolboxItemUsed();
                    }
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
        }

        /// <internalonly/>
        /// <summary>
        /// The list of technologies that this designer can support
        /// for its view.  Examples of different technologies are
        /// WinForms and Web Forms.  Other object models can be
        /// supported at design time, but they most be able to
        /// provide a view in one of the supported technologies.
        /// </summary>
        ViewTechnology[] IRootDesigner.SupportedTechnologies
        {
            get
            {
                return new ViewTechnology[] { ViewTechnology.Default, (ViewTechnology)1 };
            }
        }

        /// <internalonly/>
        /// <summary>
        /// The view for this document.  The designer
        /// should assume that the view will be shown shortly
        /// after this call is made and make any necessary
        /// preparations.
        /// </summary>

        //We can live with this one. We have obsoleted some of the enum values. This method
        //only takes on argument, so it is pretty obvious what argument is bad.
        [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
        object IRootDesigner.GetView(ViewTechnology technology)
        {
            if (technology != ViewTechnology.Default && technology != (ViewTechnology)1)
            {
                throw new ArgumentException();
            }

            return frame;
        }

        /// <summary>
        ///  Determines if the given tool is supported by this designer.
        ///  If a tool is supported then it will be enabled in the toolbox
        ///  when this designer regains focus.  Otherwise, it will be disabled.
        ///  Once a tool is marked as enabled or disabled it may not be
        ///  queried again.
        /// </summary>
        bool IToolboxUser.GetToolSupported(ToolboxItem tool)
        {
            return GetToolSupported(tool);
        }

        /// <internalonly/>
        /// <summary>
        /// <para>Selects the specified tool.</para>
        /// </summary>
        void IToolboxUser.ToolPicked(ToolboxItem tool)
        {
            using (DpiHelper.EnterDpiAwarenessScope(User32.DPI_AWARENESS_CONTEXT.SYSTEM_AWARE))
            {
                ToolPicked(tool);
            }
        }
    }
}
