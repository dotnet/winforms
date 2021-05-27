// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms.Design.Behavior;
using Accessibility;
using static Interop;

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  Provides a designer that can design components that extend Control.
    /// </summary>
    public partial class ControlDesigner : ComponentDesigner
    {
#pragma warning disable IDE1006 // Naming Styles - Public API
        protected static readonly Point InvalidPoint = new Point(int.MinValue, int.MinValue);
#pragma warning restore IDE1006

        private static uint s_currentProcessId;
        private IDesignerHost _host;                        // the host for our designer

        private bool _liveRegion;                           // is the mouse is over a live region of the control?
        private bool _inHitTest;                            // A popular way to implement GetHitTest is by WM_NCHITTEST
                                                            //  ...which would cause a cycle.
        private bool _hasLocation;                          // Do we have a location property?
        private bool _locationChecked;                      // And did we check it
        private bool _locked;                               // Signifies if this control is locked or not
        private bool _enabledchangerecursionguard;

        // Behavior work
        private BehaviorService _behaviorService;           // we cache this 'cause we use it so often
        private ResizeBehavior _resizeBehavior;             // the standard behavior for our selection glyphs - demand created
        private ContainerSelectorBehavior _moveBehavior;    // the behavior for non-resize glyphs - demand created

        // Services that we use enough to cache
        private ISelectionUIService _selectionUIService;
        private IEventHandlerService _eventService;
        private IToolboxService _toolboxService;
        private InheritanceUI _inheritanceUI;
        private IOverlayService _overlayService;

        // Transient values that are used during mouse drags
        private Point _mouseDragLast = InvalidPoint;        // the last position of the mouse during a drag.
        private bool _mouseDragMoved;                       // has the mouse been moved during this drag?
        private int _lastMoveScreenX;
        private int _lastMoveScreenY;

        // Values used to simulate double clicks for controls that don't support them.
        private uint _lastClickMessageTime;
        private int _lastClickMessagePositionX;
        private int _lastClickMessagePositionY;

        private Point _downPos = Point.Empty;               // point used to track first down of a double click
        private event EventHandler DisposingHandler;
        private CollectionChangeEventHandler _dataBindingsCollectionChanged;
        private Exception _thrownException;

        private bool _ctrlSelect;                           // if the CTRL key was down at the mouse down
        private bool _toolPassThrough;                      // a tool is selected, allow the parent to draw a rect for it.
        private bool _removalNotificationHooked;
        private bool _revokeDragDrop = true;
        private bool _hadDragDrop;

        private DesignerControlCollection _controls;

        private static bool s_inContextMenu;

        private DockingActionList _dockingAction;
        private StatusCommandUI _statusCommandUI;           // UI for setting the StatusBar Information..
        private Dictionary<IntPtr, bool> _subclassedChildren;

        protected BehaviorService BehaviorService => _behaviorService ??= GetService<BehaviorService>();

        internal bool ForceVisible { get; set; } = true;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        private DesignerControlCollection Controls => _controls ??= new DesignerControlCollection(Control);

        private Point Location
        {
            get
            {
                Point loc = Control.Location;

                ScrollableControl p = Control.Parent as ScrollableControl;
                if (p != null)
                {
                    Point pt = p.AutoScrollPosition;
                    loc.Offset(-pt.X, -pt.Y);
                }

                return loc;
            }
            set
            {
                ScrollableControl p = Control.Parent as ScrollableControl;
                if (p != null)
                {
                    Point pt = p.AutoScrollPosition;
                    value.Offset(pt.X, pt.Y);
                }

                Control.Location = value;
            }
        }

        /// <summary>
        ///  Retrieves a list of associated components. These are components that should be included
        ///  in a cut or copy operation on this component.
        /// </summary>
        public override ICollection AssociatedComponents
        {
            get
            {
                ArrayList sitedChildren = null;
                foreach (Control c in Control.Controls)
                {
                    if (c.Site != null)
                    {
                        if (sitedChildren is null)
                        {
                            sitedChildren = new ArrayList();
                        }

                        sitedChildren.Add(c);
                    }
                }

                if (sitedChildren != null)
                {
                    return sitedChildren;
                }

                return base.AssociatedComponents;
            }
        }

        protected AccessibleObject accessibilityObj;

        public virtual AccessibleObject AccessibilityObject
            => accessibilityObj ??= new ControlDesignerAccessibleObject(this, Control);

        /// <summary>
        ///  Retrieves the control we're designing.
        /// </summary>
        public virtual Control Control => (Control)Component;

        /// <summary>
        ///  Determines whether drag rects can be drawn on this designer.
        /// </summary>
        protected virtual bool EnableDragRect => false;

        /// <summary>
        ///  Gets / Sets this controls locked property
        /// </summary>
        private bool Locked
        {
            get => _locked;
            set
            {
                if (_locked != value)
                {
                    _locked = value;
                }
            }
        }

        private string Name
        {
            get
            {
                return Component.Site.Name;
            }
            set
            {
                // don't do anything here during loading, if a refactor changed it we don't want to do anything
                IDesignerHost host = GetService(typeof(IDesignerHost)) as IDesignerHost;
                if (host == null || (host != null && !host.Loading))
                {
                    Component.Site.Name = value;
                }
            }
        }

        /// <summary>
        ///  Returns the parent component for this control designer. The default implementation just checks to see if
        ///  the component being designed is a control, and if it is it returns its parent.  This property can return
        ///  null if there is no parent component.
        /// </summary>
        protected override IComponent ParentComponent
        {
            get
            {
                if (Component is Control c && c.Parent != null)
                {
                    return c.Parent;
                }

                return base.ParentComponent;
            }
        }

        /// <summary>
        ///  Determines whether or not the ControlDesigner will allow SnapLine alignment during a drag operation when
        ///  the primary drag control is over this designer, or when a control is being dragged from the toolbox, or
        ///  when a control is being drawn through click-drag.
        /// </summary>
        public virtual bool ParticipatesWithSnapLines => true;

        public bool AutoResizeHandles { get; set; }

        private IDesignerTarget DesignerTarget { get; set; }

        private Dictionary<IntPtr, bool> SubclassedChildWindows
            => _subclassedChildren ??= new Dictionary<IntPtr, bool>();

        /// <summary>
        ///  Retrieves a set of rules concerning the movement capabilities of a component. This should be one or more
        ///  flags from the SelectionRules class.  If no designer provides rules for a component, the component will
        ///  not get any UI services.
        /// </summary>
        public virtual SelectionRules SelectionRules
        {
            get
            {
                object component = Component;
                SelectionRules rules = SelectionRules.Visible;
                PropertyDescriptor prop;
                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(component);
                PropertyDescriptor autoSizeProp = props["AutoSize"];
                PropertyDescriptor autoSizeModeProp = props["AutoSizeMode"];

                if ((prop = props["Location"]) != null && !prop.IsReadOnly)
                {
                    rules |= SelectionRules.Moveable;
                }

                if ((prop = props["Size"]) != null && !prop.IsReadOnly)
                {
                    if (AutoResizeHandles && Component != _host.RootComponent)
                    {
                        rules = IsResizableConsiderAutoSize(autoSizeProp, autoSizeModeProp)
                            ? rules | SelectionRules.AllSizeable
                            : rules;
                    }
                    else
                    {
                        rules |= SelectionRules.AllSizeable;
                    }
                }

                PropertyDescriptor propDock = props["Dock"];
                if (propDock != null)
                {
                    DockStyle dock = (DockStyle)(int)propDock.GetValue(component);

                    // gotta adjust if the control's parent is mirrored... this is just such that we add the right
                    // resize handles. We need to do it this way, since resize glyphs are added in  AdornerWindow
                    // coords, and the AdornerWindow is never mirrored.
                    if (Control.Parent != null && Control.Parent.IsMirrored)
                    {
                        if (dock == DockStyle.Left)
                        {
                            dock = DockStyle.Right;
                        }
                        else if (dock == DockStyle.Right)
                        {
                            dock = DockStyle.Left;
                        }
                    }

                    switch (dock)
                    {
                        case DockStyle.Top:
                            rules &= ~(SelectionRules.Moveable | SelectionRules.TopSizeable | SelectionRules.LeftSizeable | SelectionRules.RightSizeable);
                            break;
                        case DockStyle.Left:
                            rules &= ~(SelectionRules.Moveable | SelectionRules.TopSizeable | SelectionRules.LeftSizeable | SelectionRules.BottomSizeable);
                            break;
                        case DockStyle.Right:
                            rules &= ~(SelectionRules.Moveable | SelectionRules.TopSizeable | SelectionRules.BottomSizeable | SelectionRules.RightSizeable);
                            break;
                        case DockStyle.Bottom:
                            rules &= ~(SelectionRules.Moveable | SelectionRules.LeftSizeable | SelectionRules.BottomSizeable | SelectionRules.RightSizeable);
                            break;
                        case DockStyle.Fill:
                            rules &= ~(SelectionRules.Moveable | SelectionRules.TopSizeable | SelectionRules.LeftSizeable | SelectionRules.RightSizeable | SelectionRules.BottomSizeable);
                            break;
                    }
                }

                PropertyDescriptor pd = props["Locked"];
                if (pd != null)
                {
                    object value = pd.GetValue(component);

                    // Make sure that value is a boolean, in case someone else added this property
                    if (value is bool boolean && boolean == true)
                    {
                        rules = SelectionRules.Locked | SelectionRules.Visible;
                    }
                }

                return rules;
            }
        }

        internal virtual bool ControlSupportsSnaplines => true;

        internal Point GetOffsetToClientArea()
        {
            var nativeOffset = new Point();
            User32.MapWindowPoints(Control.Handle, Control.Parent.Handle, ref nativeOffset, 1);
            Point offset = Control.Location;

            // If the 2 controls do not have the same orientation, then force one to make sure we calculate the correct offset
            if (Control.IsMirrored != Control.Parent.IsMirrored)
            {
                offset.Offset(Control.Width, 0);
            }

            return (new Point(Math.Abs(nativeOffset.X - offset.X), nativeOffset.Y - offset.Y));
        }

        /// <summary>
        ///  Per AutoSize spec, determines if a control is resizable.
        /// </summary>
        private bool IsResizableConsiderAutoSize(PropertyDescriptor autoSizeProp, PropertyDescriptor autoSizeModeProp)
        {
            object component = Component;
            bool resizable = true;
            bool autoSize = false;
            bool growOnly = false;

            AttributeCollection attributes = autoSizeProp?.Attributes;
            if (attributes != null
                && !(attributes.Contains(DesignerSerializationVisibilityAttribute.Hidden)
                    || attributes.Contains(BrowsableAttribute.No)))
            {
                autoSize = (bool)autoSizeProp.GetValue(component);
            }

            if (autoSizeModeProp != null)
            {
                AutoSizeMode mode = (AutoSizeMode)autoSizeModeProp.GetValue(component);
                growOnly = mode == AutoSizeMode.GrowOnly;
            }

            if (autoSize)
            {
                resizable = growOnly;
            }

            return resizable;
        }

        /// <summary>
        ///  Returns a list of SnapLine objects representing interesting alignment points for this control.
        ///  These SnapLines are used to assist in the positioning of the control on a parent's surface.
        /// </summary>
        public virtual IList SnapLines => SnapLinesInternal();

        internal IList SnapLinesInternal() => SnapLinesInternal(Control.Margin);

        internal IList SnapLinesInternal(Padding margin)
        {
            ArrayList snapLines = new ArrayList(4);
            int width = Control.Width;
            int height = Control.Height;

            // the four edges of our control
            snapLines.Add(new SnapLine(SnapLineType.Top, 0, SnapLinePriority.Low));
            snapLines.Add(new SnapLine(SnapLineType.Bottom, height - 1, SnapLinePriority.Low));
            snapLines.Add(new SnapLine(SnapLineType.Left, 0, SnapLinePriority.Low));
            snapLines.Add(new SnapLine(SnapLineType.Right, width - 1, SnapLinePriority.Low));

            // the four margins of our control
            // Even if a control does not have margins, we still want to add Margin snaplines.
            // This is because we only try to match to matching snaplines. Makes the code a little easier...
            snapLines.Add(new SnapLine(SnapLineType.Horizontal, -margin.Top, SnapLine.MarginTop, SnapLinePriority.Always));
            snapLines.Add(new SnapLine(SnapLineType.Horizontal, margin.Bottom + height, SnapLine.MarginBottom, SnapLinePriority.Always));
            snapLines.Add(new SnapLine(SnapLineType.Vertical, -margin.Left, SnapLine.MarginLeft, SnapLinePriority.Always));
            snapLines.Add(new SnapLine(SnapLineType.Vertical, margin.Right + width, SnapLine.MarginRight, SnapLinePriority.Always));
            return snapLines;
        }

        protected override InheritanceAttribute InheritanceAttribute
            => IsRootDesigner ? InheritanceAttribute.Inherited : base.InheritanceAttribute;

        internal new bool IsRootDesigner
        {
            get
            {
                Debug.Assert(Component != null, "this.component needs to be set before this method is valid.");
                return TryGetService(out IDesignerHost host) && Component == host.RootComponent;
            }
        }

        /// <summary>
        ///  Returns the number of internal control designers in the ControlDesigner. An internal control is a control
        ///  that is not in the IDesignerHost.Container.Components collection. SplitterPanel is an example of one such
        ///  control. We use this to get SnapLines for the internal control designers.
        /// </summary>
        public virtual int NumberOfInternalControlDesigners() => 0;

        /// <summary>
        ///  Returns the internal control designer with the specified index in the ControlDesigner. An internal control
        ///  is a control that is not in the IDesignerHost.Container.Components collection. SplitterPanel is an example
        ///  of one such control. internalControlIndex is zero-based.
        /// </summary>
        public virtual ControlDesigner InternalControlDesigner(int internalControlIndex) => null;

        /// <summary>
        ///  Default processing for messages.  This method causes the message to get processed by windows, skipping the
        ///  control.  This is useful if you want to block this message from getting to the control, but you do not
        ///  want to block it from getting to Windows itself because it causes other messages to be generated.
        /// </summary>
        protected void BaseWndProc(ref Message m)
            => m.Result = User32.DefWindowProcW(m.HWnd, (User32.WM)m.Msg, m.WParam, m.LParam);

        /// <summary>
        ///  Determines if the this designer can be parented to the specified designer -- generally this means if the
        ///  control for this designer can be parented into the given ParentControlDesigner's designer.
        /// </summary>
        public virtual bool CanBeParentedTo(IDesigner parentDesigner)
            => parentDesigner is ParentControlDesigner p && !Control.Contains(p.Control);

        /// <summary>
        ///  Default processing for messages. This method causes the message to get processed by the control, rather
        ///  than the designer.
        /// </summary>
        protected void DefWndProc(ref Message m) => DesignerTarget.DefWndProc(ref m);

        /// <summary>
        ///  Displays the given exception to the user.
        /// </summary>
        protected void DisplayError(Exception e)
        {
            if (TryGetService(out IUIService uis))
            {
                uis.ShowError(e);
            }
            else
            {
                string message = e.Message;
                if (string.IsNullOrEmpty(message))
                {
                    message = e.ToString();
                }

                RTLAwareMessageBox.Show(
                    Control,
                    message,
                    null,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1,
                    0);
            }
        }

        /// <summary>
        ///  Disposes of this object.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Control != null)
                {
                    if (_dataBindingsCollectionChanged != null)
                    {
                        Control.DataBindings.CollectionChanged -= _dataBindingsCollectionChanged;
                    }

                    if (Inherited && _inheritanceUI != null)
                    {
                        _inheritanceUI.RemoveInheritedControl(Control);
                    }

                    if (_removalNotificationHooked)
                    {
                        if (TryGetService(out IComponentChangeService csc))
                        {
                            csc.ComponentRemoved -= new ComponentEventHandler(DataSource_ComponentRemoved);
                        }

                        _removalNotificationHooked = false;
                    }

                    DisposingHandler?.Invoke(this, EventArgs.Empty);
                    UnhookChildControls(Control);
                }

                if (DesignerTarget != null)
                {
                    DesignerTarget.Dispose();
                }

                _downPos = Point.Empty;

                if (Control != null)
                {
                    Control.ControlAdded -= new ControlEventHandler(OnControlAdded);
                    Control.ControlRemoved -= new ControlEventHandler(OnControlRemoved);
                    Control.ParentChanged -= new EventHandler(OnParentChanged);
                    Control.SizeChanged -= new EventHandler(OnSizeChanged);
                    Control.LocationChanged -= new EventHandler(OnLocationChanged);
                    Control.EnabledChanged -= new EventHandler(OnEnabledChanged);
                }
            }

            base.Dispose(disposing);
        }

        private void OnControlAdded(object sender, ControlEventArgs e)
        {
            if (e.Control is null || _host is null || _host.GetDesigner(e.Control) is ControlDesigner)
            {
                return;
            }

            // No designer means we must replace the window target in this control.
            IWindowTarget oldTarget = e.Control.WindowTarget;
            if (!(oldTarget is ChildWindowTarget))
            {
                e.Control.WindowTarget = new ChildWindowTarget(this, e.Control, oldTarget);

                // Controls added in UserControl.OnLoad() do not  setup sniffing WndProc properly.
                e.Control.ControlAdded += new ControlEventHandler(OnControlAdded);
            }

            // Some controls (primarily RichEdit) will register themselves as drag-drop source/targets when
            // they are instantiated. We have to RevokeDragDrop() for them so that the ParentControlDesigner()'s
            // drag-drop support can work correctly. Normally, the hwnd for the child control is not created at
            // this time, and we will use the WM_CREATE message in ChildWindowTarget's WndProc() to revoke
            // drag-drop. But, if the handle was already created for some reason, we will need to revoke
            // drag-drop right away.
            if (e.Control.IsHandleCreated)
            {
                Application.OleRequired();
                Ole32.RevokeDragDrop(e.Control.Handle);

                // We only hook the control's children if there was no designer. We leave it up to the designer
                // to hook its own children.
                HookChildControls(e.Control);
            }
        }

        private void DataSource_ComponentRemoved(object sender, ComponentEventArgs e)
        {
            // It is possible to use the control designer with NON CONTROl types.
            if (!(Component is Control ctl))
            {
                return;
            }

            Debug.Assert(ctl.DataBindings.Count > 0, "we should not be notified if the control has no dataBindings");
            ctl.DataBindings.CollectionChanged -= _dataBindingsCollectionChanged;
            for (int i = 0; i < ctl.DataBindings.Count; i++)
            {
                Binding binding = ctl.DataBindings[i];
                if (binding.DataSource == e.Component)
                {
                    // remove the binding from the control's collection. this will also remove the binding from
                    // the bindingManagerBase's bindingscollection
                    // NOTE: we can't remove the bindingManager from the bindingContext, cause there may be some
                    // complex bound controls ( such as the dataGrid, or the ComboBox, or the ListBox ) that still
                    // use that bindingManager
                    ctl.DataBindings.Remove(binding);
                }
            }

            // if after removing those bindings the collection is empty, then unhook the changeNotificationService
            if (ctl.DataBindings.Count == 0)
            {
                if (TryGetService(out IComponentChangeService csc))
                {
                    csc.ComponentRemoved -= new ComponentEventHandler(DataSource_ComponentRemoved);
                }

                _removalNotificationHooked = false;
            }

            ctl.DataBindings.CollectionChanged += _dataBindingsCollectionChanged;
        }

        /// <summary>
        ///  Enables design time functionality for a child control.  The child control is a child of this control
        ///  designer's control.  The child does not directly participate in persistence, but it will if it is exposed
        ///  as a property of the main control.  Consider a control like the SplitContainer:  it has two panels,
        ///  Panel1 and Panel2.  These panels are exposed through read only Panel1 and Panel2 properties on the
        ///  SplitContainer class. SplitContainer's designer calls EnableDesignTime for each panel, which allows other
        ///  components to be dropped on them.  But, in order for the contents of Panel1 and Panel2 to be saved,
        ///  SplitContainer itself needed to expose the panels as public properties. The child parameter is the control
        ///  to enable.  The name parameter is the name of this control as exposed to the end user.  Names need to be
        ///  unique within a control designer, but do not have to be unique to other control designer's children. This
        ///  method returns true if the child control could be enabled for design time, or false if the hosting
        ///  infrastructure does not support it.  To support this feature, the hosting infrastructure must expose the
        ///  INestedContainer class as a service off of the site.
        /// </summary>
        protected bool EnableDesignMode(Control child, string name)
        {
            if (child is null)
            {
                throw new ArgumentNullException(nameof(child));
            }

            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (!TryGetService(out INestedContainer nc))
            {
                return false;
            }

            // Only add the child if it doesn't already exist. VSWhidbey #408041.
            for (int i = 0; i < nc.Components.Count; i++)
            {
                if (nc.Components[i].Equals(child))
                {
                    return true;
                }
            }

            nc.Add(child, name);
            return true;
        }

        /// <summary>
        ///  Enables or disables drag/drop support.  This hooks drag event handlers to the control.
        /// </summary>
        protected void EnableDragDrop(bool value)
        {
            Control rc = Control;
            if (rc is null)
            {
                return;
            }

            if (value)
            {
                rc.DragDrop += new DragEventHandler(OnDragDrop);
                rc.DragOver += new DragEventHandler(OnDragOver);
                rc.DragEnter += new DragEventHandler(OnDragEnter);
                rc.DragLeave += new EventHandler(OnDragLeave);
                rc.GiveFeedback += new GiveFeedbackEventHandler(OnGiveFeedback);
                _hadDragDrop = rc.AllowDrop;

                if (!_hadDragDrop)
                {
                    rc.AllowDrop = true;
                }

                _revokeDragDrop = false;
            }
            else
            {
                rc.DragDrop -= new DragEventHandler(OnDragDrop);
                rc.DragOver -= new DragEventHandler(OnDragOver);
                rc.DragEnter -= new DragEventHandler(OnDragEnter);
                rc.DragLeave -= new EventHandler(OnDragLeave);
                rc.GiveFeedback -= new GiveFeedbackEventHandler(OnGiveFeedback);

                if (!_hadDragDrop)
                {
                    rc.AllowDrop = false;
                }

                _revokeDragDrop = true;
            }
        }

        private void OnGiveFeedback(object s, GiveFeedbackEventArgs e) => OnGiveFeedback(e);

        private void OnDragLeave(object s, EventArgs e) => OnDragLeave(e);

        private void OnDragEnter(object s, DragEventArgs e)
        {
            if (BehaviorService != null)
            {
                // Tell the BehaviorService to monitor mouse messages so it can send appropriate drag notifications.
                BehaviorService.StartDragNotification();
            }

            OnDragEnter(e);
        }

        private void OnDragOver(object s, DragEventArgs e) => OnDragOver(e);

        private void OnDragDrop(object s, DragEventArgs e)
        {
            if (BehaviorService != null)
            {
                // This will cause the Behavior Service to return from 'drag mode'
                BehaviorService.EndDragNotification();
            }

            OnDragDrop(e);
        }

        internal Behavior.Behavior MoveBehavior
            => _moveBehavior ??= new ContainerSelectorBehavior(Control, Component.Site);

        /// <summary>
        ///  Returns a 'BodyGlyph' representing the bounds of this control. The BodyGlyph is responsible for hit
        ///  testing the related CtrlDes and forwarding messages directly to the designer.
        /// </summary>
        protected virtual ControlBodyGlyph GetControlGlyph(GlyphSelectionType selectionType)
        {
            // get the right cursor for this component
            OnSetCursor();
            Cursor cursor = Cursor.Current;

            // get the correctly translated bounds
            Rectangle translatedBounds = BehaviorService.ControlRectInAdornerWindow(Control);

            // create our glyph, and set its cursor appropriately
            ControlBodyGlyph g = null;
            Control parent = Control.Parent;

            if (parent != null && _host != null && _host.RootComponent != Component)
            {
                Rectangle parentRect = parent.RectangleToScreen(parent.ClientRectangle);
                Rectangle controlRect = Control.RectangleToScreen(Control.ClientRectangle);
                if (!parentRect.Contains(controlRect) && !parentRect.IntersectsWith(controlRect))
                {
                    // Since the parent is completely clipping the control, the control cannot be a drop target, and
                    // it will not get mouse messages. So we don't have to give the glyph a transparentbehavior
                    // (default for ControlBodyGlyph). But we still would like to be able to move the control, so push
                    // a MoveBehavior. If we didn't we wouldn't be able to move the control, since it won't get any
                    // mouse messages.

                    if (TryGetService(out ISelectionService sel) && sel.GetComponentSelected(Control))
                    {
                        g = new ControlBodyGlyph(translatedBounds, cursor, Control, MoveBehavior);
                    }
                    else if (cursor == Cursors.SizeAll)
                    {
                        // If we get here, OnSetCursor could have set the cursor to SizeAll. But if we fall into this
                        // category, we don't have a MoveBehavior, so we don't want to show the SizeAll cursor. Let's
                        // make sure the cursor is set to the default cursor.
                        cursor = Cursors.Default;
                    }
                }
            }

            // If null, we are not totally clipped by the parent
            g ??= new ControlBodyGlyph(translatedBounds, cursor, Control, this);

            return g;
        }

        internal ControlBodyGlyph GetControlGlyphInternal(GlyphSelectionType selectionType) => GetControlGlyph(selectionType);

        /// <summary>
        ///  Returns a collection of Glyph objects representing the selection borders and grab handles for a standard
        ///  control.  Note that based on 'selectionType' the Glyphs returned will either: represent a fully resizeable
        ///  selection border with grab handles, a locked selection border, or a single 'hidden' selection Glyph.
        /// </summary>
        public virtual GlyphCollection GetGlyphs(GlyphSelectionType selectionType)
        {
            GlyphCollection glyphs = new GlyphCollection();

            if (selectionType == GlyphSelectionType.NotSelected)
            {
                return glyphs;
            }

            Rectangle translatedBounds = BehaviorService.ControlRectInAdornerWindow(Control);
            bool primarySelection = (selectionType == GlyphSelectionType.SelectedPrimary);
            SelectionRules rules = SelectionRules;

            if (Locked || (InheritanceAttribute == InheritanceAttribute.InheritedReadOnly))
            {
                // the lock glyph
                glyphs.Add(new LockedHandleGlyph(translatedBounds, primarySelection));

                // the four locked border glyphs
                glyphs.Add(new LockedBorderGlyph(translatedBounds, SelectionBorderGlyphType.Top));
                glyphs.Add(new LockedBorderGlyph(translatedBounds, SelectionBorderGlyphType.Bottom));
                glyphs.Add(new LockedBorderGlyph(translatedBounds, SelectionBorderGlyphType.Left));
                glyphs.Add(new LockedBorderGlyph(translatedBounds, SelectionBorderGlyphType.Right));
            }
            else if ((rules & SelectionRules.AllSizeable) == SelectionRules.None)
            {
                // the non-resizeable grab handle
                glyphs.Add(new NoResizeHandleGlyph(translatedBounds, rules, primarySelection, MoveBehavior));

                // the four resizeable border glyphs
                glyphs.Add(new NoResizeSelectionBorderGlyph(translatedBounds, rules, SelectionBorderGlyphType.Top, MoveBehavior));
                glyphs.Add(new NoResizeSelectionBorderGlyph(translatedBounds, rules, SelectionBorderGlyphType.Bottom, MoveBehavior));
                glyphs.Add(new NoResizeSelectionBorderGlyph(translatedBounds, rules, SelectionBorderGlyphType.Left, MoveBehavior));
                glyphs.Add(new NoResizeSelectionBorderGlyph(translatedBounds, rules, SelectionBorderGlyphType.Right, MoveBehavior));

                // enable the designeractionpanel for this control if it needs one
                if (TypeDescriptor.GetAttributes(Component).Contains(DesignTimeVisibleAttribute.Yes)
                    && _behaviorService.DesignerActionUI != null)
                {
                    Glyph dapGlyph = _behaviorService.DesignerActionUI.GetDesignerActionGlyph(Component);
                    if (dapGlyph != null)
                    {
                        glyphs.Insert(0, dapGlyph); // we WANT to be in front of the other UI
                    }
                }
            }
            else
            {
                // Grab handles
                if ((rules & SelectionRules.TopSizeable) != 0)
                {
                    glyphs.Add(new GrabHandleGlyph(translatedBounds, GrabHandleGlyphType.MiddleTop, StandardBehavior, primarySelection));
                    if ((rules & SelectionRules.LeftSizeable) != 0)
                    {
                        glyphs.Add(new GrabHandleGlyph(translatedBounds, GrabHandleGlyphType.UpperLeft, StandardBehavior, primarySelection));
                    }

                    if ((rules & SelectionRules.RightSizeable) != 0)
                    {
                        glyphs.Add(new GrabHandleGlyph(translatedBounds, GrabHandleGlyphType.UpperRight, StandardBehavior, primarySelection));
                    }
                }

                if ((rules & SelectionRules.BottomSizeable) != 0)
                {
                    glyphs.Add(new GrabHandleGlyph(translatedBounds, GrabHandleGlyphType.MiddleBottom, StandardBehavior, primarySelection));
                    if ((rules & SelectionRules.LeftSizeable) != 0)
                    {
                        glyphs.Add(new GrabHandleGlyph(translatedBounds, GrabHandleGlyphType.LowerLeft, StandardBehavior, primarySelection));
                    }

                    if ((rules & SelectionRules.RightSizeable) != 0)
                    {
                        glyphs.Add(new GrabHandleGlyph(translatedBounds, GrabHandleGlyphType.LowerRight, StandardBehavior, primarySelection));
                    }
                }

                if ((rules & SelectionRules.LeftSizeable) != 0)
                {
                    glyphs.Add(new GrabHandleGlyph(translatedBounds, GrabHandleGlyphType.MiddleLeft, StandardBehavior, primarySelection));
                }

                if ((rules & SelectionRules.RightSizeable) != 0)
                {
                    glyphs.Add(new GrabHandleGlyph(translatedBounds, GrabHandleGlyphType.MiddleRight, StandardBehavior, primarySelection));
                }

                // the four resizeable border glyphs
                glyphs.Add(new SelectionBorderGlyph(translatedBounds, rules, SelectionBorderGlyphType.Top, StandardBehavior));
                glyphs.Add(new SelectionBorderGlyph(translatedBounds, rules, SelectionBorderGlyphType.Bottom, StandardBehavior));
                glyphs.Add(new SelectionBorderGlyph(translatedBounds, rules, SelectionBorderGlyphType.Left, StandardBehavior));
                glyphs.Add(new SelectionBorderGlyph(translatedBounds, rules, SelectionBorderGlyphType.Right, StandardBehavior));

                // enable the designeractionpanel for this control if it needs one
                if (TypeDescriptor.GetAttributes(Component).Contains(DesignTimeVisibleAttribute.Yes)
                    && _behaviorService.DesignerActionUI != null)
                {
                    Glyph dapGlyph = _behaviorService.DesignerActionUI.GetDesignerActionGlyph(Component);
                    if (dapGlyph != null)
                    {
                        glyphs.Insert(0, dapGlyph); // we WANT to be in front of the other UI
                    }
                }
            }

            return glyphs;
        }

        /// <summary>
        ///  Demand creates the StandardBehavior related to this
        ///  ControlDesigner.  This is used to associate the designer's
        ///  selection glyphs to a common Behavior (resize in this case).
        /// </summary>
        internal virtual Behavior.Behavior StandardBehavior => _resizeBehavior ??= new ResizeBehavior(Component.Site);

        internal virtual bool SerializePerformLayout => false;

        /// <summary>
        ///  Allows your component to support a design time user interface.  A TabStrip control, for example, has a
        ///  design time user interface that allows the user to click the tabs to change tabs.  To implement this,
        ///  TabStrip returns true whenever the given point is within its tabs.
        /// </summary>
        protected virtual bool GetHitTest(Point point) => false;

        /// <summary>
        ///  Hooks the children of the given control.  We need to do this for child controls that are not in design
        ///  mode, which is the case for composite controls.
        /// </summary>
        protected void HookChildControls(Control firstChild)
        {
            foreach (Control child in firstChild.Controls)
            {
                if (child == null || _host == null || _host.GetDesigner(child) is ControlDesigner)
                {
                    continue;
                }

                // No designer means we must replace the window target in this control.
                IWindowTarget oldTarget = child.WindowTarget;
                if (!(oldTarget is ChildWindowTarget))
                {
                    child.WindowTarget = new ChildWindowTarget(this, child, oldTarget);
                    child.ControlAdded += new ControlEventHandler(OnControlAdded);
                }

                if (child.IsHandleCreated)
                {
                    Application.OleRequired();
                    Ole32.RevokeDragDrop(child.Handle);
                    HookChildHandles(child.Handle);
                }
                else
                {
                    child.HandleCreated += new EventHandler(OnChildHandleCreated);
                }

                // We only hook the children's children if there was no designer. We leave it up to the
                // designer to hook its own children.
                HookChildControls(child);
            }
        }

        private void OnChildHandleCreated(object sender, EventArgs e)
        {
            Control child = sender as Control;

            Debug.Assert(child != null);

            if (child != null)
            {
                Debug.Assert(child.IsHandleCreated);
                HookChildHandles(child.Handle);
            }
        }

        /// <summary>
        ///  Called by the host when we're first initialized.
        /// </summary>
        public override void Initialize(IComponent component)
        {
            // Visibility works as follows:  If the control's property is not actually set, then set our shadow to true.
            // Otherwise, grab the shadow value from the control directly and then set the control to be visible if it
            // is not the root component.  Root components will be set to visible = true in their own time by the view.
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(component.GetType());
            PropertyDescriptor visibleProp = props["Visible"];
            Visible = visibleProp is null
                || visibleProp.PropertyType != typeof(bool)
                || !visibleProp.ShouldSerializeValue(component)
                || (bool)visibleProp.GetValue(component);

            PropertyDescriptor enabledProp = props["Enabled"];
            Enabled = enabledProp is null
                || enabledProp.PropertyType != typeof(bool)
                || !enabledProp.ShouldSerializeValue(component)
                || (bool)enabledProp.GetValue(component);

            base.Initialize(component);

            // And get other commonly used services.
            _host = GetService<IDesignerHost>();

            // This is to create the action in the DAP for this component if it requires docking/undocking logic
            AttributeCollection attributes = TypeDescriptor.GetAttributes(Component);
            DockingAttribute dockingAttribute = (DockingAttribute)attributes[typeof(DockingAttribute)];
            if (dockingAttribute != null && dockingAttribute.DockingBehavior != DockingBehavior.Never)
            {
                // Create the action for this control
                _dockingAction = new DockingActionList(this);

                // Add our 'dock in parent' or 'undock in parent' action
                if (TryGetService(out DesignerActionService das))
                {
                    das.Add(Component, _dockingAction);
                }
            }

            // Hook up the property change notifications we need to track. One for data binding.
            // More for control add / remove notifications
            _dataBindingsCollectionChanged = new CollectionChangeEventHandler(DataBindingsCollectionChanged);
            Control.DataBindings.CollectionChanged += _dataBindingsCollectionChanged;

            Control.ControlAdded += new ControlEventHandler(OnControlAdded);
            Control.ControlRemoved += new ControlEventHandler(OnControlRemoved);
            Control.ParentChanged += new EventHandler(OnParentChanged);

            Control.SizeChanged += new EventHandler(OnSizeChanged);
            Control.LocationChanged += new EventHandler(OnLocationChanged);

            // Replace the control's window target with our own. This allows us to hook messages.
            DesignerTarget = new DesignerWindowTarget(this);

            // If the handle has already been created for this control, invoke OnCreateHandle so we can hookup our
            // child control subclass.
            if (Control.IsHandleCreated)
            {
                OnCreateHandle();
            }

            // If we are an inherited control, notify our inheritance UI
            if (Inherited && _host != null && _host.RootComponent != component)
            {
                _inheritanceUI = GetService<InheritanceUI>();
                _inheritanceUI?.AddInheritedControl(Control, InheritanceAttribute.InheritanceLevel);
            }

            // When we drag one control from one form to another, we will end up here. In this case we do not want to
            // set the control to visible, so check ForceVisible.
            if ((_host is null || _host.RootComponent != component) && ForceVisible)
            {
                Control.Visible = true;
            }

            // Always make controls enabled, event inherited ones.  Otherwise we won't be able to select them.
            Control.Enabled = true;

            // we move enabledchanged below the set to avoid any possible stack overflows. this can occur if the parent
            // is not enabled when we set enabled to true.
            Control.EnabledChanged += new EventHandler(OnEnabledChanged);

            // And force some shadow properties that we change in the course of initializing the form.
            AllowDrop = Control.AllowDrop;

            // update the Status Command
            _statusCommandUI = new StatusCommandUI(component.Site);
        }

        // This is a workaround to some problems with the ComponentCache that we should fix. When this is removed
        // remember to change ComponentCache's RemoveEntry method back to private (from internal).
        private void OnSizeChanged(object sender, EventArgs e)
        {
            object component = Component;
            if (TryGetService(out ComponentCache cache) && component != null)
            {
                cache.RemoveEntry(component);
            }
        }

        private void OnLocationChanged(object sender, EventArgs e)
        {
            object component = Component;
            if (TryGetService(out ComponentCache cache) && component != null)
            {
                cache.RemoveEntry(component);
            }
        }

        private void OnParentChanged(object sender, EventArgs e)
        {
            if (Control.IsHandleCreated)
            {
                OnHandleChange();
            }
        }

        private void OnControlRemoved(object sender, ControlEventArgs e)
        {
            if (e.Control != null)
            {
                // No designer means we must replace the window target in this control.
                if (e.Control.WindowTarget is ChildWindowTarget oldTarget)
                {
                    e.Control.WindowTarget = oldTarget.OldWindowTarget;
                }

                UnhookChildControls(e.Control);
            }
        }

        private void DataBindingsCollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            // It is possible to use the control designer with NON CONTROl types.
            if (Component is Control ctl)
            {
                if (ctl.DataBindings.Count == 0 && _removalNotificationHooked)
                {
                    // Remove the notification for the ComponentRemoved event
                    if (TryGetService(out IComponentChangeService csc))
                    {
                        csc.ComponentRemoved -= new ComponentEventHandler(DataSource_ComponentRemoved);
                    }

                    _removalNotificationHooked = false;
                }
                else if (ctl.DataBindings.Count > 0 && !_removalNotificationHooked)
                {
                    // Add the notification for the ComponentRemoved event
                    if (TryGetService(out IComponentChangeService csc))
                    {
                        csc.ComponentRemoved += new ComponentEventHandler(DataSource_ComponentRemoved);
                    }

                    _removalNotificationHooked = true;
                }
            }
        }

        private void OnEnabledChanged(object sender, EventArgs e)
        {
            if (!_enabledchangerecursionguard)
            {
                _enabledchangerecursionguard = true;

                try
                {
                    Control.Enabled = true;
                }
                finally
                {
                    _enabledchangerecursionguard = false;
                }
            }
        }

        /// <summary>
        ///  Accessor for AllowDrop.  Since we often turn this on, we shadow it so it doesn't show up to the user.
        /// </summary>
        private bool AllowDrop
        {
            get => (bool)ShadowProperties[nameof(AllowDrop)];
            set => ShadowProperties[nameof(AllowDrop)] = value;
        }

        /// <summary>
        ///  Accessor method for the enabled property on control. We shadow this property at design time.
        /// </summary>
        private bool Enabled
        {
            get => (bool)ShadowProperties[nameof(Enabled)];
            set => ShadowProperties[nameof(Enabled)] = value;
        }

        private bool Visible
        {
            get => (bool)ShadowProperties[nameof(Visible)];
            set => ShadowProperties[nameof(Visible)] = value;
        }

        /// <summary>
        ///  ControlDesigner overrides this method to handle after-drop cases.
        /// </summary>
        public override void InitializeExistingComponent(IDictionary defaultValues)
        {
            base.InitializeExistingComponent(defaultValues);

            // unhook any sited children that got ChildWindowTargets
            foreach (Control c in Control.Controls)
            {
                if (c != null)
                {
                    ISite site = c.Site;
                    if (site != null && c.WindowTarget is ChildWindowTarget target)
                    {
                        c.WindowTarget = target.OldWindowTarget;
                    }
                }
            }
        }

        /// <summary>
        ///  ControlDesigner overrides this method.  It will look at the default property for the control and,
        ///  if it is of type string, it will set this property's value to the name of the component.  It only does
        ///  this if the designer has been configured with this option in the options service.  This method also
        ///  connects the control to its parent and positions it.  If you override this method, you should always
        ///  call base.
        /// </summary>
        public override void InitializeNewComponent(IDictionary defaultValues)
        {
            ISite site = Component.Site;
            if (site != null)
            {
                PropertyDescriptor textProp = TypeDescriptor.GetProperties(Component)["Text"];
                if (textProp != null && textProp.PropertyType == typeof(string) && !textProp.IsReadOnly && textProp.IsBrowsable)
                {
                    textProp.SetValue(Component, site.Name);
                }
            }

            if (defaultValues != null
                && defaultValues["Parent"] is IComponent parent
                && TryGetService(out IDesignerHost host))
            {
                if (host.GetDesigner(parent) is ParentControlDesigner parentDesigner)
                {
                    parentDesigner.AddControl(Control, defaultValues);
                }

                if (parent is Control parentControl)
                {
                    // Some containers are docked differently (instead of DockStyle.None) when they are added through the designer
                    AttributeCollection attributes = TypeDescriptor.GetAttributes(Component);
                    DockingAttribute dockingAttribute = (DockingAttribute)attributes[typeof(DockingAttribute)];

                    if (dockingAttribute != null
                        && dockingAttribute.DockingBehavior != DockingBehavior.Never
                        && dockingAttribute.DockingBehavior == DockingBehavior.AutoDock)
                    {
                        bool onlyNonDockedChild = true;
                        foreach (Control c in parentControl.Controls)
                        {
                            if (c != Control && c.Dock == DockStyle.None)
                            {
                                onlyNonDockedChild = false;
                                break;
                            }
                        }

                        if (onlyNonDockedChild)
                        {
                            PropertyDescriptor dockProp = TypeDescriptor.GetProperties(Component)["Dock"];
                            if (dockProp != null && dockProp.IsBrowsable)
                            {
                                dockProp.SetValue(Component, DockStyle.Fill);
                            }
                        }
                    }
                }
            }

            base.InitializeNewComponent(defaultValues);
        }

        /// <summary>
        ///  Called when the designer is initialized.  This allows the designer to provide some meaningful default
        ///  values in the component.  The default implementation of this sets the components's default property to
        ///  it's name, if that property is a string.
        /// </summary>
        [Obsolete("This method has been deprecated. Use InitializeNewComponent instead.  https://go.microsoft.com/fwlink/?linkid=14202")]
        public override void OnSetComponentDefaults()
        {
            ISite site = Component.Site;
            if (site != null)
            {
                PropertyDescriptor textProp = TypeDescriptor.GetProperties(Component)["Text"];
                if (textProp != null && textProp.IsBrowsable)
                {
                    textProp.SetValue(Component, site.Name);
                }
            }
        }

        /// <summary>
        ///  Called when the context menu should be displayed
        /// </summary>
        protected virtual void OnContextMenu(int x, int y) => ShowContextMenu(x, y);

        /// <summary>
        ///  This is called immediately after the control handle has been created.
        /// </summary>
        protected virtual void OnCreateHandle()
        {
            OnHandleChange();
            if (_revokeDragDrop)
            {
                Ole32.RevokeDragDrop(Control.Handle);
            }
        }

        /// <summary>
        ///  Called when a drag-drop operation enters the control designer view
        /// </summary>
        protected virtual void OnDragEnter(DragEventArgs de)
        {
            // unhook our events - we don't want to create an infinite loop.
            Control control = Control;
            DragEventHandler handler = new DragEventHandler(OnDragEnter);
            control.DragEnter -= handler;
            ((IDropTarget)Control).OnDragEnter(de);
            control.DragEnter += handler;
        }

        /// <summary>
        ///  Called to cleanup a drag and drop operation.
        /// </summary>
        protected virtual void OnDragComplete(DragEventArgs de)
        {
            // default implementation - does nothing.
        }

        /// <summary>
        ///  Called when a drag drop object is dropped onto the control designer view
        /// </summary>
        protected virtual void OnDragDrop(DragEventArgs de)
        {
            // unhook our events - we don't want to create an infinite loop.
            Control control = Control;
            DragEventHandler handler = new DragEventHandler(OnDragDrop);
            control.DragDrop -= handler;
            ((IDropTarget)Control).OnDragDrop(de);
            control.DragDrop += handler;
            OnDragComplete(de);
        }

        /// <summary>
        ///  Called when a drag-drop operation leaves the control designer view
        /// </summary>
        protected virtual void OnDragLeave(EventArgs e)
        {
            // unhook our events - we don't want to create an infinite loop.
            Control control = Control;
            EventHandler handler = new EventHandler(OnDragLeave);
            control.DragLeave -= handler;
            ((IDropTarget)Control).OnDragLeave(e);
            control.DragLeave += handler;
        }

        /// <summary>
        ///  Called when a drag drop object is dragged over the control designer view
        /// </summary>
        protected virtual void OnDragOver(DragEventArgs de)
        {
            // unhook our events - we don't want to create an infinite loop.
            Control control = Control;
            DragEventHandler handler = new DragEventHandler(OnDragOver);
            control.DragOver -= handler;
            ((IDropTarget)Control).OnDragOver(de);
            control.DragOver += handler;
        }

        /// <summary>
        ///  Event handler for our GiveFeedback event, which is called when a drag operation is in progress.
        ///  The host will call us with this when an OLE drag event happens.
        /// </summary>
        protected virtual void OnGiveFeedback(GiveFeedbackEventArgs e)
        {
        }

        /// <summary>
        ///  Called in response to the left mouse button being pressed on a component. It ensures that the component is selected.
        /// </summary>
        protected virtual void OnMouseDragBegin(int x, int y)
        {
            // Ignore another mouse down if we are already in a drag.
            if (BehaviorService is null && _mouseDragLast != InvalidPoint)
            {
                return;
            }

            _mouseDragLast = new Point(x, y);
            _ctrlSelect = (Control.ModifierKeys & Keys.Control) != 0;
            ISelectionService selectionService = GetService<ISelectionService>();

            // If the CTRL key isn't down, select this component, otherwise, we wait until the mouse up. Make sure the component is selected
            if (!_ctrlSelect && selectionService != null)
            {
                selectionService.SetSelectedComponents(new object[] { Component }, SelectionTypes.Primary);
            }

            Control.Capture = true;
        }

        /// <summary>
        ///  Called at the end of a drag operation.  This either commits or rolls back the drag.
        /// </summary>
        protected virtual void OnMouseDragEnd(bool cancel)
        {
            _mouseDragLast = InvalidPoint;
            Control.Capture = false;

            if (!_mouseDragMoved)
            {
                // ParentControlDesigner.Dispose depends on cancel having this behavior.
                if (!cancel)
                {
                    ISelectionService selectionService = GetService<ISelectionService>();
                    bool shiftSelect = (Control.ModifierKeys & Keys.Shift) != 0;
                    if (!shiftSelect &&
                        (_ctrlSelect
                            || (selectionService != null && !selectionService.GetComponentSelected(Component))))
                    {
                        selectionService?.SetSelectedComponents(new object[] { Component }, SelectionTypes.Primary);
                        _ctrlSelect = false;
                    }
                }

                return;
            }

            _mouseDragMoved = false;
            _ctrlSelect = false;

            // And now finish the drag.
            if (BehaviorService != null && BehaviorService.Dragging && cancel)
            {
                BehaviorService.CancelDrag = true;
            }

            // Leave this here in case we are doing a ComponentTray drag
            _selectionUIService ??= GetService<ISelectionUIService>();

            if (_selectionUIService is null)
            {
                return;
            }

            // We must check to ensure that UI service is still in drag mode.  It is possible that the user hit escape,
            // which will cancel drag mode.
            if (_selectionUIService.Dragging)
            {
                _selectionUIService.EndDrag(cancel);
            }
        }

        /// <summary>
        ///  Called for each movement of the mouse. This will check to see if a drag operation is in progress. If so,
        ///  it will pass the updated drag dimensions on to the selection UI service.
        /// </summary>
        protected virtual void OnMouseDragMove(int x, int y)
        {
            if (!_mouseDragMoved)
            {
                Size minDrag = SystemInformation.DragSize;
                Size minDblClick = SystemInformation.DoubleClickSize;
                minDrag.Width = Math.Max(minDrag.Width, minDblClick.Width);
                minDrag.Height = Math.Max(minDrag.Height, minDblClick.Height);

                // we have to make sure the mouse moved farther than the minimum drag distance before we actually start the drag
                if (_mouseDragLast == InvalidPoint ||
                    (Math.Abs(_mouseDragLast.X - x) < minDrag.Width &&
                     Math.Abs(_mouseDragLast.Y - y) < minDrag.Height))
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

            // Make sure the component is selected
            // But only select it if it is not already the primary selection, and we want to toggle the current primary selection.
            ISelectionService selectionService = GetService<ISelectionService>();
            if (selectionService != null && !Component.Equals(selectionService.PrimarySelection))
            {
                selectionService.SetSelectedComponents(new object[] { Component }, SelectionTypes.Primary | SelectionTypes.Toggle);
            }

            if (BehaviorService != null && selectionService != null)
            {
                // create our list of controls-to-drag
                ArrayList dragControls = new ArrayList();
                ICollection selComps = selectionService.GetSelectedComponents();

                // must identify a required parent to avoid dragging mixes of children
                Control requiredParent = null;
                foreach (IComponent comp in selComps)
                {
                    if (comp is Control control)
                    {
                        if (requiredParent is null)
                        {
                            requiredParent = control.Parent;
                        }
                        else if (!requiredParent.Equals(control.Parent))
                        {
                            continue;//mixed selection of different parents - don't add this
                        }

                        if (_host.GetDesigner(comp) is ControlDesigner des && (des.SelectionRules & SelectionRules.Moveable) != 0)
                        {
                            dragControls.Add(comp);
                        }
                    }
                }

                // if we have controls-to-drag, create our new behavior and start the drag/drop operation
                if (dragControls.Count > 0)
                {
                    using Graphics adornerGraphics = BehaviorService.AdornerWindowGraphics;
                    DropSourceBehavior dsb = new DropSourceBehavior(dragControls, Control.Parent, _mouseDragLast);
                    BehaviorService.DoDragDrop(dsb);
                }
            }

            _mouseDragLast = InvalidPoint;
            _mouseDragMoved = false;
        }

        /// <summary>
        ///  Called when the mouse first enters the control. This is forwarded to the parent designer to enable the
        ///  container selector.
        /// </summary>
        protected virtual void OnMouseEnter()
        {
            Control ctl = Control;
            Control parent = ctl;
            object parentDesigner = null;

            while (parentDesigner is null && parent != null)
            {
                parent = parent.Parent;
                if (parent != null)
                {
                    object d = _host.GetDesigner(parent);
                    if (d != this)
                    {
                        parentDesigner = d;
                    }
                }
            }

            if (parentDesigner is ControlDesigner cd)
            {
                cd.OnMouseEnter();
            }
        }

        /// <summary>
        ///  Called after the mouse hovers over the control. This is forwarded to the parent designer to enable the
        ///  container selector.
        /// </summary>
        protected virtual void OnMouseHover()
        {
            Control ctl = Control;
            Control parent = ctl;
            object parentDesigner = null;

            while (parentDesigner is null && parent != null)
            {
                parent = parent.Parent;
                if (parent != null)
                {
                    object d = _host.GetDesigner(parent);
                    if (d != this)
                    {
                        parentDesigner = d;
                    }
                }
            }

            if (parentDesigner is ControlDesigner cd)
            {
                cd.OnMouseHover();
            }
        }

        /// <summary>
        ///  Called when the mouse first enters the control. This is forwarded to the parent designer to enable the
        ///  container selector.
        /// </summary>
        protected virtual void OnMouseLeave()
        {
            Control ctl = Control;
            Control parent = ctl;
            object parentDesigner = null;

            while (parentDesigner is null && parent != null)
            {
                parent = parent.Parent;
                if (parent != null)
                {
                    object d = _host.GetDesigner(parent);
                    if (d != this)
                    {
                        parentDesigner = d;
                    }
                }
            }

            if (parentDesigner is ControlDesigner cd)
            {
                cd.OnMouseLeave();
            }
        }

        /// <summary>
        ///  Called when the control we're designing has finished painting.  This method gives the designer a chance
        ///  to paint any additional adornments on top of the control.
        /// </summary>
        protected virtual void OnPaintAdornments(PaintEventArgs pe)
        {
            // If this control is being inherited, paint it
            if (_inheritanceUI != null && pe.ClipRectangle.IntersectsWith(_inheritanceUI.InheritanceGlyphRectangle))
            {
                pe.Graphics.DrawImage(_inheritanceUI.InheritanceGlyph, 0, 0);
            }
        }

        /// <summary>
        ///  Called each time the cursor needs to be set.
        /// </summary>
        /// <remarks>
        /// The ControlDesigner behavior here will set the cursor to one of three things:
        ///
        ///  1.  If the toolbox service has a tool selected, it will allow the toolbox service to set the cursor.
        ///  2.  If the selection UI service shows a locked selection, or if there is no location property on the
        ///  control, then the default arrow will be set.
        ///  3.  Otherwise, the four headed arrow will be set to indicate that the component can be clicked and moved.
        ///  4.  If the user is currently dragging a component, the crosshair cursor will be used instead of the four
        ///  headed arrow.
        /// </remarks>
        protected virtual void OnSetCursor()
        {
            if (Control.Dock != DockStyle.None)
            {
                Cursor.Current = Cursors.Default;
                return;
            }

            _toolboxService ??= GetService<IToolboxService>();

            if (_toolboxService != null && _toolboxService.SetCursor())
            {
                return;
            }

            if (!_locationChecked)
            {
                _locationChecked = true;
                try
                {
                    _hasLocation = TypeDescriptor.GetProperties(Component)["Location"] != null;
                }
                catch
                {
                }
            }

            if (!_hasLocation)
            {
                Cursor.Current = Cursors.Default;
                return;
            }

            if (Locked)
            {
                Cursor.Current = Cursors.Default;
                return;
            }

            Cursor.Current = Cursors.SizeAll;
        }

        /// <summary>
        ///  Allows a designer to filter the set of properties the component it is designing will expose through the
        ///  TypeDescriptor object.  This method is called immediately before its corresponding "Post" method. If you
        ///  are overriding this method you should call the base implementation before you perform your own filtering.
        /// </summary>
        protected override void PreFilterProperties(IDictionary properties)
        {
            base.PreFilterProperties(properties);
            PropertyDescriptor prop;

            // Handle shadowed properties
            string[] shadowProps = new string[] { "Visible", "Enabled", "AllowDrop", "Location", "Name" };

            Attribute[] empty = Array.Empty<Attribute>();
            for (int i = 0; i < shadowProps.Length; i++)
            {
                prop = (PropertyDescriptor)properties[shadowProps[i]];
                if (prop != null)
                {
                    properties[shadowProps[i]] = TypeDescriptor.CreateProperty(typeof(ControlDesigner), prop, empty);
                }
            }

            // replace this one seperately because it is of a different type (DesignerControlCollection) than the
            // original property (ControlCollection)
            PropertyDescriptor controlsProp = (PropertyDescriptor)properties["Controls"];

            if (controlsProp != null)
            {
                Attribute[] attrs = new Attribute[controlsProp.Attributes.Count];
                controlsProp.Attributes.CopyTo(attrs, 0);
                properties["Controls"] = TypeDescriptor.CreateProperty(
                    typeof(ControlDesigner),
                    "Controls",
                    typeof(DesignerControlCollection),
                    attrs);
            }

            PropertyDescriptor sizeProp = (PropertyDescriptor)properties["Size"];
            if (sizeProp != null)
            {
                properties["Size"] = new CanResetSizePropertyDescriptor(sizeProp);
            }

            // Now we add our own design time properties.
            properties["Locked"] = TypeDescriptor.CreateProperty(
                typeof(ControlDesigner),
                "Locked",
                typeof(bool),
                new DefaultValueAttribute(false),
                BrowsableAttribute.Yes,
                CategoryAttribute.Design,
                DesignOnlyAttribute.Yes,
                new SRDescriptionAttribute(SR.lockedDescr));
        }

        /// <summary>
        ///  Hooks the children of the given control.  We need to do this for child controls that are not in design
        ///  mode, which is the case for composite controls.
        /// </summary>
        protected void UnhookChildControls(Control firstChild)
        {
            _host ??= GetService<IDesignerHost>();

            foreach (Control child in firstChild.Controls)
            {
                IWindowTarget oldTarget = null;
                if (child != null)
                {
                    // No, no designer means we must replace the window target in this control.
                    oldTarget = child.WindowTarget;
                    if (oldTarget is ChildWindowTarget target)
                    {
                        child.WindowTarget = target.OldWindowTarget;
                    }
                }

                if (!(oldTarget is DesignerWindowTarget))
                {
                    UnhookChildControls(child);
                }
            }
        }

        /// <summary>
        ///  This method should be called by the extending designer for each message the control would normally
        ///  receive. This allows the designer to pre-process messages before allowing them to be routed to the control.
        /// </summary>
        protected unsafe virtual void WndProc(ref Message m)
        {
            IMouseHandler mouseHandler = null;

            // We look at WM_NCHITTEST to determine if the mouse is in a live region of the control
            if (m.Msg == (int)User32.WM.NCHITTEST && !_inHitTest)
            {
                _inHitTest = true;
                Point pt = new Point((short)PARAM.LOWORD(m.LParam), (short)PARAM.HIWORD(m.LParam));
                try
                {
                    _liveRegion = GetHitTest(pt);
                }
                catch (Exception e)
                {
                    _liveRegion = false;
                    if (ClientUtils.IsCriticalException(e))
                    {
                        throw;
                    }
                }

                _inHitTest = false;
            }

            // Check to see if the mouse is in a live region of the control and that the context key is not being fired
            bool isContextKey = (m.Msg == (int)User32.WM.CONTEXTMENU);
            if (_liveRegion && (IsMouseMessage(m.Msg) || isContextKey))
            {
                // The ActiveX DataGrid control brings up a context menu on right mouse down when it is in edit mode.
                // And, when we generate a WM_CONTEXTMENU message later, it calls DefWndProc() which by default calls
                // the parent (formdesigner). The FormDesigner then brings up the AxHost context menu. This code
                // causes recursive WM_CONTEXTMENU messages to be ignored till we return from the live region message.
                if (m.Msg == (int)User32.WM.CONTEXTMENU)
                {
                    Debug.Assert(!s_inContextMenu, "Recursively hitting live region for context menu!!!");
                    s_inContextMenu = true;
                }

                try
                {
                    DefWndProc(ref m);
                }
                finally
                {
                    if (m.Msg == (int)User32.WM.CONTEXTMENU)
                    {
                        s_inContextMenu = false;
                    }

                    if (m.Msg == (int)User32.WM.LBUTTONUP)
                    {
                        // terminate the drag. TabControl loses shortcut menu options after adding ActiveX control.
                        OnMouseDragEnd(true);
                    }
                }

                return;
            }

            // Get the x and y coordinates of the mouse message
            int x = 0, y = 0;

            // Look for a mouse handler.
            // CONSIDER - I really don't like this one bit. We need a
            //          : centralized handler so we can do a global override for the tab order
            //          : UI, but the designer is a natural fit for an object oriented UI.
            if ((m.Msg >= (int)User32.WM.MOUSEFIRST && m.Msg <= (int)User32.WM.MOUSELAST)
                || (m.Msg >= (int)User32.WM.NCMOUSEMOVE && m.Msg <= (int)User32.WM.NCMBUTTONDBLCLK)
                || m.Msg == (int)User32.WM.SETCURSOR)
            {
                _eventService ??= GetService<IEventHandlerService>();

                if (_eventService != null)
                {
                    mouseHandler = (IMouseHandler)_eventService.GetHandler(typeof(IMouseHandler));
                }
            }

            if (m.Msg >= (int)User32.WM.MOUSEFIRST && m.Msg <= (int)User32.WM.MOUSELAST)
            {
                var pt = new Point
                {
                    X = PARAM.SignedLOWORD(m.LParam),
                    Y = PARAM.SignedHIWORD(m.LParam)
                };
                User32.MapWindowPoints(m.HWnd, IntPtr.Zero, ref pt, 1);
                x = pt.X;
                y = pt.Y;
            }
            else if (m.Msg >= (int)User32.WM.NCMOUSEMOVE && m.Msg <= (int)User32.WM.NCMBUTTONDBLCLK)
            {
                x = PARAM.SignedLOWORD(m.LParam);
                y = PARAM.SignedHIWORD(m.LParam);
            }

            // This is implemented on the base designer for UI activation support.  We call it so that we can support
            // UI activation.
            MouseButtons button = MouseButtons.None;
            switch ((User32.WM)m.Msg)
            {
                case User32.WM.CREATE:
                    DefWndProc(ref m);

                    // Only call OnCreateHandle if this is our OWN window handle -- the designer window procs are
                    // re-entered for child controls.
                    if (m.HWnd == Control.Handle)
                    {
                        OnCreateHandle();
                    }

                    break;

                case User32.WM.GETOBJECT:
                    // See "How to Handle WM_GETOBJECT" in MSDN
                    if (unchecked((int)(long)m.LParam) == User32.OBJID.CLIENT)
                    {
                        Guid IID_IAccessible = new Guid(NativeMethods.uuid_IAccessible);
                        // Get an Lresult for the accessibility Object for this control
                        IntPtr punkAcc;
                        IAccessible iacc = AccessibilityObject;
                        if (iacc is null)
                        {
                            // Accessibility is not supported on this control
                            m.Result = (IntPtr)0;
                        }
                        else
                        {
                            // Obtain the Lresult
                            punkAcc = Marshal.GetIUnknownForObject(iacc);
                            try
                            {
                                m.Result = Oleacc.LresultFromObject(ref IID_IAccessible, m.WParam, punkAcc);
                            }
                            finally
                            {
                                Marshal.Release(punkAcc);
                            }
                        }
                    }
                    else
                    {  // m.lparam != OBJID_CLIENT, so do default message processing
                        DefWndProc(ref m);
                    }

                    break;

                case User32.WM.MBUTTONDOWN:
                case User32.WM.MBUTTONUP:
                case User32.WM.MBUTTONDBLCLK:
                case User32.WM.NCMOUSEHOVER:
                case User32.WM.NCMOUSELEAVE:
                case User32.WM.MOUSEWHEEL:
                case User32.WM.NCMBUTTONDOWN:
                case User32.WM.NCMBUTTONUP:
                case User32.WM.NCMBUTTONDBLCLK:
                    // We intentionally eat these messages.
                    break;
                case User32.WM.MOUSEHOVER:
                    if (mouseHandler != null)
                    {
                        mouseHandler.OnMouseHover(Component);
                    }
                    else
                    {
                        OnMouseHover();
                    }

                    break;
                case User32.WM.MOUSELEAVE:
                    OnMouseLeave();
                    BaseWndProc(ref m);
                    break;
                case User32.WM.NCLBUTTONDBLCLK:
                case User32.WM.LBUTTONDBLCLK:
                case User32.WM.NCRBUTTONDBLCLK:
                case User32.WM.RBUTTONDBLCLK:
                    if ((m.Msg == (int)User32.WM.NCRBUTTONDBLCLK || m.Msg == (int)User32.WM.RBUTTONDBLCLK))
                    {
                        button = MouseButtons.Right;
                    }
                    else
                    {
                        button = MouseButtons.Left;
                    }

                    if (button == MouseButtons.Left)
                    {
                        // We handle doubleclick messages, and we also process our own simulated double clicks for
                        // controls that don't specify CS_WANTDBLCLKS.
                        if (mouseHandler != null)
                        {
                            mouseHandler.OnMouseDoubleClick(Component);
                        }
                        else
                        {
                            OnMouseDoubleClick();
                        }
                    }

                    break;
                case User32.WM.NCLBUTTONDOWN:
                case User32.WM.LBUTTONDOWN:
                case User32.WM.NCRBUTTONDOWN:
                case User32.WM.RBUTTONDOWN:
                    if ((m.Msg == (int)User32.WM.NCRBUTTONDOWN || m.Msg == (int)User32.WM.RBUTTONDOWN))
                    {
                        button = MouseButtons.Right;
                    }
                    else
                    {
                        button = MouseButtons.Left;
                    }

                    // We don't really want the focus, but we want to focus the designer. Below we handle WM_SETFOCUS
                    // and do the right thing.
                    User32.SendMessageW(Control.Handle, User32.WM.SETFOCUS, IntPtr.Zero, IntPtr.Zero);

                    // We simulate doubleclick for things that don't...
                    if (button == MouseButtons.Left && IsDoubleClick(x, y))
                    {
                        if (mouseHandler != null)
                        {
                            mouseHandler.OnMouseDoubleClick(Component);
                        }
                        else
                        {
                            OnMouseDoubleClick();
                        }
                    }
                    else
                    {
                        _toolPassThrough = false;
                        if (!EnableDragRect && button == MouseButtons.Left)
                        {
                            _toolboxService ??= GetService<IToolboxService>();

                            if (_toolboxService?.GetSelectedToolboxItem(GetService<IDesignerHost>()) != null)
                            {
                                // There is a tool to be dragged, so set passthrough and pass to the parent.
                                _toolPassThrough = true;
                            }
                        }
                        else
                        {
                            _toolPassThrough = false;
                        }

                        if (_toolPassThrough)
                        {
                            User32.SendMessageW(Control.Parent.Handle, (User32.WM)m.Msg, m.WParam, (IntPtr)GetParentPointFromLparam(m.LParam));
                            return;
                        }

                        if (mouseHandler != null)
                        {
                            mouseHandler.OnMouseDown(Component, button, x, y);
                        }
                        else if (button == MouseButtons.Left)
                        {
                            OnMouseDragBegin(x, y);
                        }
                        else if (button == MouseButtons.Right)
                        {
                            GetService<ISelectionService>()?.SetSelectedComponents(
                                new object[] { Component },
                                SelectionTypes.Primary);
                        }

                        _lastMoveScreenX = x;
                        _lastMoveScreenY = y;
                    }

                    break;

                case User32.WM.NCMOUSEMOVE:
                case User32.WM.MOUSEMOVE:
                    if ((unchecked((User32.MK)(long)m.WParam) & User32.MK.LBUTTON) != 0)
                    {
                        button = MouseButtons.Left;
                    }
                    else if ((unchecked((User32.MK)(long)m.WParam) & User32.MK.RBUTTON) != 0)
                    {
                        button = MouseButtons.Right;
                        _toolPassThrough = false;
                    }
                    else
                    {
                        _toolPassThrough = false;
                    }

                    if (_lastMoveScreenX != x || _lastMoveScreenY != y)
                    {
                        if (_toolPassThrough)
                        {
                            User32.SendMessageW(
                                Control.Parent.Handle,
                                (User32.WM)m.Msg,
                                m.WParam,
                                (IntPtr)GetParentPointFromLparam(m.LParam));
                            return;
                        }

                        if (mouseHandler != null)
                        {
                            mouseHandler.OnMouseMove(Component, x, y);
                        }
                        else if (button == MouseButtons.Left)
                        {
                            OnMouseDragMove(x, y);
                        }
                    }

                    _lastMoveScreenX = x;
                    _lastMoveScreenY = y;

                    // We eat WM_NCMOUSEMOVE messages, since we don't want the non-client area/ of design time
                    // controls to repaint on mouse move.
                    if (m.Msg == (int)User32.WM.MOUSEMOVE)
                    {
                        BaseWndProc(ref m);
                    }

                    break;
                case User32.WM.NCLBUTTONUP:
                case User32.WM.LBUTTONUP:
                case User32.WM.NCRBUTTONUP:
                case User32.WM.RBUTTONUP:
                    // This is implemented on the base designer for UI activation support.
                    button = m.Msg == (int)User32.WM.NCRBUTTONUP || m.Msg == (int)User32.WM.RBUTTONUP
                        ? MouseButtons.Right
                        : MouseButtons.Left;

                    // And terminate the drag.
                    if (mouseHandler != null)
                    {
                        mouseHandler.OnMouseUp(Component, button);
                    }
                    else
                    {
                        if (_toolPassThrough)
                        {
                            User32.SendMessageW(
                                Control.Parent.Handle,
                                (User32.WM)m.Msg,
                                m.WParam,
                                (IntPtr)GetParentPointFromLparam(m.LParam));
                            _toolPassThrough = false;
                            return;
                        }

                        if (button == MouseButtons.Left)
                        {
                            OnMouseDragEnd(false);
                        }
                    }

                    // clear any pass through.
                    _toolPassThrough = false;
                    BaseWndProc(ref m);
                    break;
                case User32.WM.PRINTCLIENT:
                    {
                        using Graphics g = Graphics.FromHdc(m.WParam);
                        using PaintEventArgs e = new PaintEventArgs(g, Control.ClientRectangle);
                        DefWndProc(ref m);
                        OnPaintAdornments(e);
                    }

                    break;
                case User32.WM.PAINT:
                    {
#if FEATURE_OLEDRAGDROPHANDLER
                        if (OleDragDropHandler.FreezePainting)
                        {
                            User32.ValidateRect(m.HWnd, null);
                            break;
                        }
#endif

                        if (Control is null)
                        {
                            break;
                        }

                        // First, save off the update region and call our base class.

                        RECT clip = new RECT();
                        using var hrgn = new Gdi32.RegionScope(0, 0, 0, 0);
                        User32.GetUpdateRgn(m.HWnd, hrgn, BOOL.FALSE);
                        User32.GetUpdateRect(m.HWnd, ref clip, BOOL.FALSE);
                        using Region region = hrgn.CreateGdiPlusRegion();

                        // Call the base class to do its own painting.
                        if (_thrownException is null)
                        {
                            DefWndProc(ref m);
                        }

                        // Now do our own painting.
                        using Graphics graphics = Graphics.FromHwnd(m.HWnd);

                        if (m.HWnd != Control.Handle)
                        {
                            // Re-map the clip rect we pass to the paint event args to our child coordinates.
                            Point point = default;
                            User32.MapWindowPoints(m.HWnd, Control.Handle, ref point, 1);
                            graphics.TranslateTransform(-point.X, -point.Y);
                            User32.MapWindowPoints(m.HWnd, Control.Handle, ref clip, 2);
                        }

                        Rectangle paintRect = clip;
                        using PaintEventArgs pevent = new PaintEventArgs(graphics, paintRect);

                        graphics.Clip = region;
                        if (_thrownException is null)
                        {
                            OnPaintAdornments(pevent);
                        }
                        else
                        {
                            using var scope = new User32.BeginPaintScope(m.HWnd);
                            PaintException(pevent, _thrownException);
                        }

                        if (OverlayService != null)
                        {
                            // This will allow any Glyphs to re-paint after this control and its designer has painted
                            paintRect.Location = Control.PointToScreen(paintRect.Location);
                            OverlayService.InvalidateOverlays(paintRect);
                        }

                        break;
                    }

                case User32.WM.NCPAINT:
                case User32.WM.NCACTIVATE:
                    if (m.Msg == (int)User32.WM.NCACTIVATE)
                    {
                        DefWndProc(ref m);
                    }
                    else if (_thrownException is null)
                    {
                        DefWndProc(ref m);
                    }

                    // For some reason we don't always get an NCPAINT with the WM_NCACTIVATE usually this repros with
                    // themes on.... this can happen when someone calls RedrawWindow without the flags to send an
                    // NCPAINT.  So that we don't double process this event, our calls to redraw window should not have
                    // RDW_ERASENOW | RDW_UPDATENOW.
                    if (OverlayService != null)
                    {
                        if (Control != null && Control.Size != Control.ClientSize && Control.Parent != null)
                        {
                            // we have a non-client region to invalidate
                            Rectangle controlScreenBounds = new Rectangle(Control.Parent.PointToScreen(Control.Location), Control.Size);
                            Rectangle clientAreaScreenBounds = new Rectangle(Control.PointToScreen(Point.Empty), Control.ClientSize);

                            using Region nonClient = new Region(controlScreenBounds);
                            nonClient.Exclude(clientAreaScreenBounds);
                            OverlayService.InvalidateOverlays(nonClient);
                        }
                    }

                    break;

                case User32.WM.SETCURSOR:
                    // We always handle setting the cursor ourselves.

                    if (_liveRegion)
                    {
                        DefWndProc(ref m);
                        break;
                    }

                    if (mouseHandler != null)
                    {
                        mouseHandler.OnSetCursor(Component);
                    }
                    else
                    {
                        OnSetCursor();
                    }

                    break;
                case User32.WM.SIZE:
                    if (_thrownException != null)
                    {
                        Control.Invalidate();
                    }

                    DefWndProc(ref m);
                    break;
                case User32.WM.CANCELMODE:
                    // When we get cancelmode (i.e. you tabbed away to another window) then we want to cancel any
                    // pending drag operation!
                    OnMouseDragEnd(true);
                    DefWndProc(ref m);
                    break;
                case User32.WM.SETFOCUS:
                    // We eat the focus unless the target is a ToolStrip edit node (TransparentToolStrip). If we eat
                    // the focus in that case, the Windows Narrator won't follow navigation via the keyboard.
                    // NB:  "ToolStrip" is a bit of a misnomer here, because the ToolStripTemplateNode is also used
                    // for MenuStrip, StatusStrip, etc...
                    //if (Control.FromHandle(m.HWnd) is ToolStripTemplateNode.TransparentToolStrip)
                    //{
                    //    DefWndProc(ref m);
                    //}
                    //else
                    if (_host != null && _host.RootComponent != null
                        && _host.GetDesigner(_host.RootComponent) is IRootDesigner rd)
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

                    break;
                case User32.WM.CONTEXTMENU:
                    if (s_inContextMenu)
                    {
                        break;
                    }

                    // We handle this in addition to a right mouse button. Why?  Because we often eat the right mouse
                    // button, so it may never generate a WM_CONTEXTMENU.  However, the system may generate one in
                    // response to an F-10.
                    x = PARAM.SignedLOWORD(m.LParam);
                    y = PARAM.SignedHIWORD(m.LParam);

                    bool handled = GetService<ToolStripKeyboardHandlingService>()?.OnContextMenu(x, y) ?? false;

                    if (!handled)
                    {
                        if (x == -1 && y == -1)
                        {
                            // for shift-F10
                            Point p = Cursor.Position;
                            x = p.X;
                            y = p.Y;
                        }

                        OnContextMenu(x, y);
                    }

                    break;
                default:
                    if (m.Msg == (int)User32.RegisteredMessage.WM_MOUSEENTER)
                    {
                        OnMouseEnter();
                        BaseWndProc(ref m);
                    }
                    else if (m.Msg < (int)User32.WM.KEYFIRST || m.Msg > (int)User32.WM.KEYLAST)
                    {
                        // We eat all key handling to the control.  Controls generally should not be getting focus
                        // anyway, so this shouldn't happen. However, we want to prevent this as much as possible.
                        DefWndProc(ref m);
                    }

                    break;
            }
        }

        private void PaintException(PaintEventArgs e, Exception ex)
        {
            StringFormat stringFormat = new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Near
            };

            string exceptionText = ex.ToString();
            stringFormat.SetMeasurableCharacterRanges(new CharacterRange[] { new CharacterRange(0, exceptionText.Length) });

            // rendering calculations...
            int penThickness = 2;
            Size glyphSize = SystemInformation.IconSize;
            int marginX = penThickness * 2;
            int marginY = penThickness * 2;

            Rectangle clientRectangle = Control.ClientRectangle;
            Rectangle borderRectangle = clientRectangle;
            borderRectangle.X++;
            borderRectangle.Y++;
            borderRectangle.Width -= 2;
            borderRectangle.Height -= 2;

            Rectangle imageRect = new Rectangle(marginX, marginY, glyphSize.Width, glyphSize.Height);
            Rectangle textRect = clientRectangle;
            textRect.X = imageRect.X + imageRect.Width + 2 * marginX;
            textRect.Y = imageRect.Y;
            textRect.Width -= (textRect.X + marginX + penThickness);
            textRect.Height -= (textRect.Y + marginY + penThickness);

            using (Font errorFont = new Font(
                Control.Font.FontFamily,
                Math.Max(SystemInformation.ToolWindowCaptionHeight - SystemInformation.BorderSize.Height - 2, Control.Font.Height),
                GraphicsUnit.Pixel))
            {
                using Region textRegion = e.Graphics.MeasureCharacterRanges(exceptionText, errorFont, textRect, stringFormat)[0];

                // Paint contents... clipping optimizations for less flicker...
                Region originalClip = e.Graphics.Clip;
                e.Graphics.ExcludeClip(textRegion);
                e.Graphics.ExcludeClip(imageRect);
                try
                {
                    e.Graphics.FillRectangle(Brushes.White, clientRectangle);
                }
                finally
                {
                    e.Graphics.Clip = originalClip;
                }

                using (Pen pen = new Pen(Color.Red, penThickness))
                {
                    e.Graphics.DrawRectangle(pen, borderRectangle);
                }

                Icon err = SystemIcons.Error;
                e.Graphics.FillRectangle(Brushes.White, imageRect);
                e.Graphics.DrawIcon(err, imageRect.X, imageRect.Y);
                textRect.X++;
                e.Graphics.IntersectClip(textRegion);

                try
                {
                    e.Graphics.FillRectangle(Brushes.White, textRect);
                    e.Graphics.DrawString(exceptionText, errorFont, new SolidBrush(Control.ForeColor), textRect, stringFormat);
                }
                finally
                {
                    e.Graphics.Clip = originalClip;
                }
            }

            stringFormat.Dispose();
        }

        private IOverlayService OverlayService => _overlayService ??= GetService<IOverlayService>();

        private bool IsMouseMessage(int msg)
        {
            if (msg >= (int)User32.WM.MOUSEFIRST && msg <= (int)User32.WM.MOUSELAST)
            {
                return true;
            }

            switch ((User32.WM)msg)
            {
                // WM messages not covered by the above block
                case User32.WM.MOUSEHOVER:
                case User32.WM.MOUSELEAVE:
                // WM_NC messages
                case User32.WM.NCMOUSEMOVE:
                case User32.WM.NCLBUTTONDOWN:
                case User32.WM.NCLBUTTONUP:
                case User32.WM.NCLBUTTONDBLCLK:
                case User32.WM.NCRBUTTONDOWN:
                case User32.WM.NCRBUTTONUP:
                case User32.WM.NCRBUTTONDBLCLK:
                case User32.WM.NCMBUTTONDOWN:
                case User32.WM.NCMBUTTONUP:
                case User32.WM.NCMBUTTONDBLCLK:
                case User32.WM.NCMOUSEHOVER:
                case User32.WM.NCMOUSELEAVE:
                case User32.WM.NCXBUTTONDOWN:
                case User32.WM.NCXBUTTONUP:
                case User32.WM.NCXBUTTONDBLCLK:
                    return true;
                default:
                    return false;
            }
        }

        private bool IsDoubleClick(int x, int y)
        {
            bool doubleClick = false;
            int wait = SystemInformation.DoubleClickTime;
            uint elapsed = Kernel32.GetTickCount() - _lastClickMessageTime;
            if (elapsed <= wait)
            {
                Size dblClick = SystemInformation.DoubleClickSize;
                if (x >= _lastClickMessagePositionX - dblClick.Width
                    && x <= _lastClickMessagePositionX + dblClick.Width
                    && y >= _lastClickMessagePositionY - dblClick.Height
                    && y <= _lastClickMessagePositionY + dblClick.Height)
                {
                    doubleClick = true;
                }
            }

            if (!doubleClick)
            {
                _lastClickMessagePositionX = x;
                _lastClickMessagePositionY = y;
                _lastClickMessageTime = Kernel32.GetTickCount();
            }
            else
            {
                _lastClickMessagePositionX = _lastClickMessagePositionY = 0;
                _lastClickMessageTime = 0;
            }

            return doubleClick;
        }

        private void OnMouseDoubleClick()
        {
            try
            {
                DoDefaultAction();
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

        private int GetParentPointFromLparam(IntPtr lParam)
        {
            Point pt = new Point(PARAM.SignedLOWORD(lParam), PARAM.SignedHIWORD(lParam));
            pt = Control.PointToScreen(pt);
            pt = Control.Parent.PointToClient(pt);
            return PARAM.ToInt(pt.X, pt.Y);
        }

        internal void HookChildHandles(IntPtr firstChild)
        {
            IntPtr hwndChild = firstChild;
            while (hwndChild != IntPtr.Zero)
            {
                if (!IsWindowInCurrentProcess(hwndChild))
                {
                    break;
                }

                // Is it a control?
                Control child = Control.FromHandle(hwndChild);
                if (child is null)
                {
                    // No control.  We must subclass this control.
                    if (!SubclassedChildWindows.ContainsKey(hwndChild))
                    {
                        // Some controls (primarily RichEdit) will register themselves as
                        // drag-drop source/targets when they are instantiated. Since these hwnds do not
                        // have a Windows Forms control associated with them, we have to RevokeDragDrop()
                        // for them so that the ParentControlDesigner()'s drag-drop support can work
                        // correctly.
                        Ole32.RevokeDragDrop(hwndChild);
                        new ChildSubClass(this, hwndChild);
                        SubclassedChildWindows[hwndChild] = true;
                    }
                }

                // UserControl is a special ContainerControl which should "hook to all the WindowHandles"
                // Since it doesnt allow the Mouse to pass through any of its contained controls.
                // Please refer to VsWhidbey : 293117
                if (child is null || Control is UserControl)
                {
                    // Now do the children of this window.
                    HookChildHandles(User32.GetWindow(hwndChild, User32.GW.CHILD));
                }

                hwndChild = User32.GetWindow(hwndChild, User32.GW.HWNDNEXT);
            }
        }

        private bool IsWindowInCurrentProcess(IntPtr hwnd)
        {
            User32.GetWindowThreadProcessId(hwnd, out uint pid);
            return pid == CurrentProcessId;
        }

        private uint CurrentProcessId
        {
            get
            {
                if (s_currentProcessId == 0)
                {
                    s_currentProcessId = Kernel32.GetCurrentProcessId();
                }

                return s_currentProcessId;
            }
        }

        private void OnHandleChange()
        {
            // We must now traverse child handles for this control.
            //
            // There are three types of child handles and we are interested in two of them:
            //
            //  1. Child handles that do not have a Control associated  with them. We must subclass these and prevent
            //      them from getting design-time events.
            //  2. Child handles that do have a Control associated with them, but the control does not have a designer.
            //      We must hook the WindowTarget on these controls and prevent them from getting design-time events.
            //  3. Child handles that do have a Control associated with them, and the control has a designer. We ignore
            //      these and let the designer handle their messages.
            HookChildHandles(User32.GetWindow(Control.Handle, User32.GW.CHILD));
            HookChildControls(Control);
        }

        internal void RemoveSubclassedWindow(IntPtr hwnd)
        {
            if (SubclassedChildWindows.ContainsKey(hwnd))
            {
                SubclassedChildWindows.Remove(hwnd);
            }
        }

        internal void SetUnhandledException(Control owner, Exception exception)
        {
            if (_thrownException != null)
            {
                return;
            }

            _thrownException = exception;
            if (owner is null)
            {
                owner = Control;
            }

            string stack = string.Empty;
            string[] exceptionLines = exception.StackTrace.Split('\r', '\n');
            string typeName = owner.GetType().FullName;
            foreach (string line in exceptionLines)
            {
                if (line.IndexOf(typeName) != -1)
                {
                    stack = string.Format(CultureInfo.CurrentCulture, "{0}\r\n{1}", stack, line);
                }
            }

            Exception wrapper = new Exception(
                string.Format(SR.ControlDesigner_WndProcException, typeName, exception.Message, stack),
                exception);
            DisplayError(wrapper);

            // hide all the child controls.
            foreach (Control c in Control.Controls)
            {
                c.Visible = false;
            }

            Control.Invalidate(true);
        }
    }
}
