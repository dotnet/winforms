// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms.Design.Behavior;
using Windows.Win32.System.SystemServices;

namespace System.Windows.Forms.Design;

/// <summary>
///  Provides a designer that can design components that extend Control.
/// </summary>
public partial class ControlDesigner : ComponentDesigner
{
    protected static readonly Point InvalidPoint = new(int.MinValue, int.MinValue);

    private static uint s_currentProcessId;
    private IDesignerHost? _host;                       // the host for our designer

    private bool _liveRegion;                           // is the mouse is over a live region of the control?
    private bool _inHitTest;                            // A popular way to implement GetHitTest is by WM_NCHITTEST
                                                        //  ...which would cause a cycle.
    private bool _hasLocation;                          // Do we have a location property?
    private bool _locationChecked;                      // And did we check it
    private bool _locked;                               // Signifies if this control is locked or not
    private bool _enabledchangerecursionguard;

    // Behavior work
    private BehaviorService? _behaviorService;          // we cache this 'cause we use it so often
    private ResizeBehavior? _resizeBehavior;            // the standard behavior for our selection glyphs - demand created
    private ContainerSelectorBehavior? _moveBehavior;   // the behavior for non-resize glyphs - demand created

    // Services that we use enough to cache
    private ISelectionUIService? _selectionUIService;
    private IEventHandlerService? _eventService;
    private IToolboxService? _toolboxService;
    private InheritanceUI? _inheritanceUI;
    private IOverlayService? _overlayService;

    // Transient values that are used during mouse drags
    private Point _mouseDragLast = InvalidPoint;        // the last position of the mouse during a drag.
    private bool _mouseDragMoved;                       // has the mouse been moved during this drag?
    private int _lastMoveScreenX;
    private int _lastMoveScreenY;

    // Values used to simulate double clicks for controls that don't support them.
    private uint _lastClickMessageTime;
    private int _lastClickMessagePositionX;
    private int _lastClickMessagePositionY;

    private event EventHandler? DisposingHandler;
    private CollectionChangeEventHandler? _dataBindingsCollectionChanged;
    private Exception? _thrownException;

    private bool _ctrlSelect;                           // if the CTRL key was down at the mouse down
    private bool _toolPassThrough;                      // a tool is selected, allow the parent to draw a rect for it.
    private bool _removalNotificationHooked;
    private bool _revokeDragDrop = true;
    private bool _hadDragDrop;

    private DesignerControlCollection? _controls;

    private static bool s_inContextMenu;

    private DockingActionList? _dockingAction;
    private Dictionary<IntPtr, bool>? _subclassedChildren;

    protected BehaviorService? BehaviorService => _behaviorService ??= GetService<BehaviorService>();

    internal bool ForceVisible { get; set; } = true;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    private DesignerControlCollection Controls => _controls ??= new DesignerControlCollection(Control);

    private Point Location
    {
        get
        {
            Point loc = Control.Location;

            if (Control.Parent is ScrollableControl parent)
            {
                Point pt = parent.AutoScrollPosition;
                loc.Offset(-pt.X, -pt.Y);
            }

            return loc;
        }
        set
        {
            if (Control.Parent is ScrollableControl parent)
            {
                Point pt = parent.AutoScrollPosition;
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
            List<IComponent>? sitedChildren = null;
            foreach (Control control in Control.Controls)
            {
                if (control.Site is not null)
                {
                    sitedChildren ??= [];
                    sitedChildren.Add(control);
                }
            }

            return sitedChildren ?? base.AssociatedComponents;
        }
    }

    protected AccessibleObject? accessibilityObj;

    public virtual AccessibleObject AccessibilityObject
        => accessibilityObj ??= new ControlDesignerAccessibleObject(this, Control);

    /// <summary>
    ///  Retrieves the control we're designing.
    /// </summary>
    public virtual Control Control => (Control)Component;

    /// <summary>
    ///  Determines whether drag rectangles can be drawn on this designer.
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

    private string? Name
    {
        get => Component.Site?.Name;
        set
        {
            // Don't do anything here during loading, if a refactor changed it we don't want to do anything.
            if ((!TryGetService(out IDesignerHost? host) || (host is not null && !host.Loading))
                && Component.Site is not null)
            {
                Component.Site.Name = value;
            }
        }
    }

    /// <summary>
    ///  Returns the parent component for this control designer. The default implementation just checks to see if
    ///  the component being designed is a control, and if it is it returns its parent. This property can return
    ///  null if there is no parent component.
    /// </summary>
    protected override IComponent? ParentComponent =>
        Component is Control c && c.Parent is not null ? c.Parent : base.ParentComponent;

    /// <summary>
    ///  Determines whether or not the ControlDesigner will allow SnapLine alignment during a drag operation when
    ///  the primary drag control is over this designer, or when a control is being dragged from the toolbox, or
    ///  when a control is being drawn through click-drag.
    /// </summary>
    public virtual bool ParticipatesWithSnapLines => true;

    public bool AutoResizeHandles { get; set; }

    private IDesignerTarget? DesignerTarget { get; set; }

    private Dictionary<IntPtr, bool> SubclassedChildWindows => _subclassedChildren ??= [];

    /// <summary>
    ///  Retrieves a set of rules concerning the movement capabilities of a component. This should be one or more
    ///  flags from the SelectionRules class. If no designer provides rules for a component, the component will
    ///  not get any UI services.
    /// </summary>
    public virtual SelectionRules SelectionRules
    {
        get
        {
            object component = Component;
            SelectionRules rules = SelectionRules.Visible;
            PropertyDescriptor? prop;
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(component);
            PropertyDescriptor? autoSizeProp = props["AutoSize"];
            PropertyDescriptor? autoSizeModeProp = props["AutoSizeMode"];

            if ((prop = props["Location"]) is not null && !prop.IsReadOnly)
            {
                rules |= SelectionRules.Moveable;
            }

            if ((prop = props["Size"]) is not null && !prop.IsReadOnly)
            {
                Debug.Assert(_host is not null);
                if (AutoResizeHandles && Component != _host?.RootComponent)
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

            if (props["Dock"] is PropertyDescriptor propDock)
            {
                DockStyle dock = (DockStyle)(int)propDock.GetValue(component)!;

                // gotta adjust if the control's parent is mirrored... this is just such that we add the right
                // resize handles. We need to do it this way, since resize glyphs are added in AdornerWindow
                // coords, and the AdornerWindow is never mirrored.
                if (Control.Parent is not null && Control.Parent.IsMirrored)
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

            if (props["Locked"] is PropertyDescriptor pd)
            {
                object? value = pd.GetValue(component);

                // Make sure that value is a boolean, in case someone else added this property
                if (value is bool boolean && boolean)
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
        Point nativeOffset = default;
        if (Control.Parent is { } parent)
        {
            PInvokeCore.MapWindowPoints(Control, parent, ref nativeOffset);
        }

        Point offset = Control.Location;

        // If the 2 controls do not have the same orientation, then force one to make sure we calculate the correct offset
        if (Control.IsMirrored != Control.Parent?.IsMirrored)
        {
            offset.Offset(Control.Width, 0);
        }

        return new Point(Math.Abs(nativeOffset.X - offset.X), nativeOffset.Y - offset.Y);
    }

    /// <summary>
    ///  Per AutoSize spec, determines if a control is resizable.
    /// </summary>
    private bool IsResizableConsiderAutoSize(PropertyDescriptor? autoSizeProp, PropertyDescriptor? autoSizeModeProp)
    {
        object component = Component;
        bool resizable = true;
        bool autoSize = false;
        bool growOnly = false;

        if (autoSizeProp?.Attributes is AttributeCollection attributes
            && !(attributes.Contains(DesignerSerializationVisibilityAttribute.Hidden)
                || attributes.Contains(BrowsableAttribute.No)))
        {
            autoSize = (bool)autoSizeProp!.GetValue(component)!;
        }

        if (autoSizeModeProp is not null)
        {
            AutoSizeMode mode = (AutoSizeMode)autoSizeModeProp.GetValue(component)!;
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
    public virtual IList SnapLines => EdgeAndMarginSnapLines().Unwrap();

    internal IList<SnapLine> SnapLinesInternal => EdgeAndMarginSnapLines();

    internal IList<SnapLine> EdgeAndMarginSnapLines() => EdgeAndMarginSnapLines(Control.Margin);

    internal IList<SnapLine> EdgeAndMarginSnapLines(Padding margin)
    {
        List<SnapLine> snapLines = new(8);
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

    protected override InheritanceAttribute? InheritanceAttribute
        => IsRootDesigner ? InheritanceAttribute.Inherited : base.InheritanceAttribute;

    internal new bool IsRootDesigner
    {
        get
        {
            Debug.Assert(Component is not null, "this.component needs to be set before this method is valid.");
            return TryGetService(out IDesignerHost? host) && Component == host.RootComponent;
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
    public virtual ControlDesigner? InternalControlDesigner(int internalControlIndex) => null;

    /// <summary>
    ///  Default processing for messages. This method causes the message to get processed by windows, skipping the
    ///  control. This is useful if you want to block this message from getting to the control, but you do not
    ///  want to block it from getting to Windows itself because it causes other messages to be generated.
    /// </summary>
    protected void BaseWndProc(ref Message m)
        => m.ResultInternal = PInvokeCore.DefWindowProc(m.HWND, (uint)m.MsgInternal, m.WParamInternal, m.LParamInternal);

    internal override bool CanBeAssociatedWith(IDesigner parentDesigner) => CanBeParentedTo(parentDesigner);

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
    protected void DefWndProc(ref Message m) => DesignerTarget?.DefWndProc(ref m);

    /// <summary>
    ///  Displays the given exception to the user.
    /// </summary>
    protected void DisplayError(Exception e)
    {
        if (TryGetService(out IUIService? uis))
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
            if (HasComponent)
            {
                if (_dataBindingsCollectionChanged is not null)
                {
                    Control.DataBindings.CollectionChanged -= _dataBindingsCollectionChanged;
                }

                if (Inherited && _inheritanceUI is not null)
                {
                    _inheritanceUI.RemoveInheritedControl(Control);
                }

                if (_removalNotificationHooked)
                {
                    if (TryGetService(out IComponentChangeService? componentChangeService))
                    {
                        componentChangeService.ComponentRemoved -= DataSource_ComponentRemoved;
                    }

                    _removalNotificationHooked = false;
                }

                DisposingHandler?.Invoke(this, EventArgs.Empty);
                UnhookChildControls(Control);
            }

            DesignerTarget?.Dispose();

            if (HasComponent)
            {
                Control.ControlAdded -= OnControlAdded;
                Control.ControlRemoved -= OnControlRemoved;
                Control.ParentChanged -= OnParentChanged;
                Control.SizeChanged -= OnSizeChanged;
                Control.LocationChanged -= OnLocationChanged;
                Control.EnabledChanged -= OnEnabledChanged;
            }
        }

        base.Dispose(disposing);
    }

    private void OnControlAdded(object? sender, ControlEventArgs e)
    {
        if (e.Control is null || _host is null || _host.GetDesigner(e.Control) is ControlDesigner)
        {
            return;
        }

        // No designer means we must replace the window target in this control.
        IWindowTarget oldTarget = e.Control.WindowTarget;
        if (oldTarget is not ChildWindowTarget)
        {
            e.Control.WindowTarget = new ChildWindowTarget(this, e.Control, oldTarget);

            // Controls added in UserControl.OnLoad() do not setup sniffing WndProc properly.
            e.Control.ControlAdded += OnControlAdded;
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
            PInvoke.RevokeDragDrop(e.Control);

            // We only hook the control's children if there was no designer. We leave it up to the designer
            // to hook its own children.
            HookChildControls(e.Control);
        }
    }

    private void DataSource_ComponentRemoved(object? sender, ComponentEventArgs e)
    {
        // It is possible to use the control designer with NON CONTROl types.
        if (Component is not Control ctl)
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
            if (TryGetService(out IComponentChangeService? componentChangeService))
            {
                componentChangeService.ComponentRemoved -= DataSource_ComponentRemoved;
            }

            _removalNotificationHooked = false;
        }

        ctl.DataBindings.CollectionChanged += _dataBindingsCollectionChanged;
    }

    /// <summary>
    ///  Enables design time functionality for a child control. The child control is a child of this control
    ///  designer's control. The child does not directly participate in persistence, but it will if it is exposed
    ///  as a property of the main control. Consider a control like the SplitContainer:  it has two panels,
    ///  Panel1 and Panel2. These panels are exposed through read only Panel1 and Panel2 properties on the
    ///  SplitContainer class. SplitContainer's designer calls EnableDesignTime for each panel, which allows other
    ///  components to be dropped on them. But, in order for the contents of Panel1 and Panel2 to be saved,
    ///  SplitContainer itself needed to expose the panels as public properties. The child parameter is the control
    ///  to enable. The name parameter is the name of this control as exposed to the end user. Names need to be
    ///  unique within a control designer, but do not have to be unique to other control designer's children. This
    ///  method returns true if the child control could be enabled for design time, or false if the hosting
    ///  infrastructure does not support it. To support this feature, the hosting infrastructure must expose the
    ///  INestedContainer class as a service off of the site.
    /// </summary>
    protected bool EnableDesignMode(Control child, string name)
    {
        ArgumentNullException.ThrowIfNull(child);
        ArgumentNullException.ThrowIfNull(name);

        if (!TryGetService(out INestedContainer? nc))
        {
            return false;
        }

        // Only add the child if it doesn't already exist. VSWhidbey #408041.
        for (int i = 0; i < nc.Components.Count; i++)
        {
            if (child.Equals(nc.Components[i]))
            {
                return true;
            }
        }

        nc.Add(child, name);
        return true;
    }

    /// <summary>
    ///  Enables or disables drag/drop support. This hooks drag event handlers to the control.
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
            rc.DragDrop += OnDragDrop;
            rc.DragOver += OnDragOver;
            rc.DragEnter += OnDragEnter;
            rc.DragLeave += OnDragLeave;
            rc.GiveFeedback += OnGiveFeedback;
            _hadDragDrop = rc.AllowDrop;

            if (!_hadDragDrop)
            {
                rc.AllowDrop = true;
            }

            _revokeDragDrop = false;
        }
        else
        {
            rc.DragDrop -= OnDragDrop;
            rc.DragOver -= OnDragOver;
            rc.DragEnter -= OnDragEnter;
            rc.DragLeave -= OnDragLeave;
            rc.GiveFeedback -= OnGiveFeedback;

            if (!_hadDragDrop)
            {
                rc.AllowDrop = false;
            }

            _revokeDragDrop = true;
        }
    }

    private void OnGiveFeedback(object? s, GiveFeedbackEventArgs e) => OnGiveFeedback(e);

    private void OnDragLeave(object? s, EventArgs e) => OnDragLeave(e);

    private void OnDragEnter(object? s, DragEventArgs e)
    {
        // Tell the BehaviorService to monitor mouse messages so it can send appropriate drag notifications.
        BehaviorService?.StartDragNotification();

        OnDragEnter(e);
    }

    private void OnDragOver(object? s, DragEventArgs e) => OnDragOver(e);

    private void OnDragDrop(object? s, DragEventArgs e)
    {
        // This will cause the Behavior Service to return from 'drag mode'
        BehaviorService?.EndDragNotification();

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
        Cursor? cursor = Cursor.Current;

        // get the correctly translated bounds
        Rectangle translatedBounds = BehaviorService?.ControlRectInAdornerWindow(Control) ?? Rectangle.Empty;

        // create our glyph, and set its cursor appropriately
        ControlBodyGlyph? g = null;
        Control? parent = Control.Parent;

        if (parent is not null && _host is not null && _host.RootComponent != Component)
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

                if (TryGetService(out ISelectionService? sel) && sel.GetComponentSelected(Control))
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
    ///  control. Note that based on 'selectionType' the Glyphs returned will either: represent a fully resizeable
    ///  selection border with grab handles, a locked selection border, or a single 'hidden' selection Glyph.
    /// </summary>
    public virtual GlyphCollection GetGlyphs(GlyphSelectionType selectionType)
    {
        GlyphCollection glyphs = [];

        if (selectionType == GlyphSelectionType.NotSelected)
        {
            return glyphs;
        }

        if (BehaviorService is null)
        {
            throw new InvalidOperationException();
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
                && _behaviorService?.DesignerActionUI is { } designerActionUI)
            {
                Glyph? dapGlyph = designerActionUI.GetDesignerActionGlyph(Component);
                if (dapGlyph is not null)
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
                && _behaviorService?.DesignerActionUI is { } designerActionUI)
            {
                Glyph? dapGlyph = designerActionUI.GetDesignerActionGlyph(Component);
                if (dapGlyph is not null)
                {
                    glyphs.Insert(0, dapGlyph); // we WANT to be in front of the other UI
                }
            }
        }

        return glyphs;
    }

    /// <summary>
    ///  Demand creates the StandardBehavior related to this
    ///  ControlDesigner. This is used to associate the designer's
    ///  selection glyphs to a common Behavior (resize in this case).
    /// </summary>
    internal virtual Behavior.Behavior StandardBehavior => _resizeBehavior ??= new ResizeBehavior(Component.Site);

    internal virtual bool SerializePerformLayout => false;

    /// <summary>
    ///  Allows your component to support a design time user interface. A TabStrip control, for example, has a
    ///  design time user interface that allows the user to click the tabs to change tabs. To implement this,
    ///  TabStrip returns true whenever the given point is within its tabs.
    /// </summary>
    protected virtual bool GetHitTest(Point point) => false;

    /// <summary>
    ///  Hooks the children of the given control. We need to do this for child controls that are not in design
    ///  mode, which is the case for composite controls.
    /// </summary>
    protected void HookChildControls(Control firstChild)
    {
        foreach (Control child in firstChild.Controls)
        {
            if (child is null || _host is null || _host.GetDesigner(child) is ControlDesigner)
            {
                continue;
            }

            // No designer means we must replace the window target in this control.
            IWindowTarget oldTarget = child.WindowTarget;
            if (oldTarget is not ChildWindowTarget)
            {
                child.WindowTarget = new ChildWindowTarget(this, child, oldTarget);
                child.ControlAdded += OnControlAdded;
            }

            if (child.IsHandleCreated)
            {
                Application.OleRequired();
                PInvoke.RevokeDragDrop(child);
                HookChildHandles((HWND)child.Handle);
            }
            else
            {
                child.HandleCreated += OnChildHandleCreated;
            }

            // We only hook the children's children if there was no designer. We leave it up to the
            // designer to hook its own children.
            HookChildControls(child);
        }
    }

    private void OnChildHandleCreated(object? sender, EventArgs e)
    {
        Control? child = sender as Control;

        Debug.Assert(child is not null);

        if (child is not null)
        {
            Debug.Assert(child.IsHandleCreated);
            HookChildHandles((HWND)child.Handle);
        }
    }

    /// <summary>
    ///  Called by the host when we're first initialized.
    /// </summary>
    public override void Initialize(IComponent component)
    {
        // Visibility works as follows:  If the control's property is not actually set, then set our shadow to true.
        // Otherwise, grab the shadow value from the control directly and then set the control to be visible if it
        // is not the root component. Root components will be set to visible = true in their own time by the view.
        PropertyDescriptorCollection props = TypeDescriptor.GetProperties(component.GetType());
        PropertyDescriptor? visibleProp = props["Visible"];
        Visible = visibleProp is null
            || visibleProp.PropertyType != typeof(bool)
            || !visibleProp.ShouldSerializeValue(component)
            || (bool)visibleProp.GetValue(component)!;

        PropertyDescriptor? enabledProp = props["Enabled"];
        Enabled = enabledProp is null
            || enabledProp.PropertyType != typeof(bool)
            || !enabledProp.ShouldSerializeValue(component)
            || (bool)enabledProp.GetValue(component)!;

        base.Initialize(component);

        // And get other commonly used services.
        _host = GetService<IDesignerHost>();

        // This is to create the action in the DAP for this component if it requires docking/undocking logic
        AttributeCollection attributes = TypeDescriptor.GetAttributes(Component);
        DockingAttribute? dockingAttribute = (DockingAttribute?)attributes[typeof(DockingAttribute)];
        if (dockingAttribute is not null && dockingAttribute.DockingBehavior != DockingBehavior.Never)
        {
            // Create the action for this control
            _dockingAction = new DockingActionList(this);

            // Add our 'dock in parent' or 'undock in parent' action
            if (TryGetService(out DesignerActionService? designerActionService))
            {
                designerActionService.Add(Component, _dockingAction);
            }
        }

        // Hook up the property change notifications we need to track. One for data binding.
        // More for control add / remove notifications
        _dataBindingsCollectionChanged = DataBindingsCollectionChanged;
        Control.DataBindings.CollectionChanged += _dataBindingsCollectionChanged;

        Control.ControlAdded += OnControlAdded;
        Control.ControlRemoved += OnControlRemoved;
        Control.ParentChanged += OnParentChanged;

        Control.SizeChanged += OnSizeChanged;
        Control.LocationChanged += OnLocationChanged;

        // Replace the control's window target with our own. This allows us to hook messages.
        DesignerTarget = new DesignerWindowTarget(this);

        // If the handle has already been created for this control, invoke OnCreateHandle so we can hookup our
        // child control subclass.
        if (Control.IsHandleCreated)
        {
            OnCreateHandle();
        }

        // If we are an inherited control, notify our inheritance UI
        if (Inherited && _host is not null
            && _host.RootComponent != component
            && InheritanceAttribute is not null)
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

        // Always make controls enabled, event inherited ones. Otherwise we won't be able to select them.
        Control.Enabled = true;

        // we move enabledchanged below the set to avoid any possible stack overflows. this can occur if the parent
        // is not enabled when we set enabled to true.
        Control.EnabledChanged += OnEnabledChanged;

        // And force some shadow properties that we change in the course of initializing the form.
        AllowDrop = Control.AllowDrop;
    }

    // This is a workaround to some problems with the ComponentCache that we should fix. When this is removed
    // remember to change ComponentCache's RemoveEntry method back to private (from internal).
    private void OnSizeChanged(object? sender, EventArgs e)
    {
        object component = Component;
        if (TryGetService(out ComponentCache? cache) && component is not null)
        {
            cache.RemoveEntry(component);
        }
    }

    private void OnLocationChanged(object? sender, EventArgs e)
    {
        object component = Component;
        if (TryGetService(out ComponentCache? cache) && component is not null)
        {
            cache.RemoveEntry(component);
        }
    }

    private void OnParentChanged(object? sender, EventArgs e)
    {
        if (Control.IsHandleCreated)
        {
            OnHandleChange();
        }
    }

    private void OnControlRemoved(object? sender, ControlEventArgs e)
    {
        if (e.Control is not null)
        {
            // No designer means we must replace the window target in this control.
            if (e.Control.WindowTarget is ChildWindowTarget oldTarget)
            {
                e.Control.WindowTarget = oldTarget.OldWindowTarget;
            }

            UnhookChildControls(e.Control);
        }
    }

    private void DataBindingsCollectionChanged(object? sender, CollectionChangeEventArgs e)
    {
        // It is possible to use the control designer with NON CONTROl types.
        if (Component is Control ctl)
        {
            if (ctl.DataBindings.Count == 0 && _removalNotificationHooked)
            {
                // Remove the notification for the ComponentRemoved event
                if (TryGetService(out IComponentChangeService? componentChangeService))
                {
                    componentChangeService.ComponentRemoved -= DataSource_ComponentRemoved;
                }

                _removalNotificationHooked = false;
            }
            else if (ctl.DataBindings.Count > 0 && !_removalNotificationHooked)
            {
                // Add the notification for the ComponentRemoved event
                if (TryGetService(out IComponentChangeService? componentChangeService))
                {
                    componentChangeService.ComponentRemoved += DataSource_ComponentRemoved;
                }

                _removalNotificationHooked = true;
            }
        }
    }

    private void OnEnabledChanged(object? sender, EventArgs e)
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
    ///  Accessor for AllowDrop. Since we often turn this on, we shadow it so it doesn't show up to the user.
    /// </summary>
    private bool AllowDrop
    {
        get => (bool)ShadowProperties[nameof(AllowDrop)]!;
        set => ShadowProperties[nameof(AllowDrop)] = value;
    }

    /// <summary>
    ///  Accessor method for the enabled property on control. We shadow this property at design time.
    /// </summary>
    private bool Enabled
    {
        get => (bool)ShadowProperties[nameof(Enabled)]!;
        set => ShadowProperties[nameof(Enabled)] = value;
    }

    private bool Visible
    {
        get => (bool)ShadowProperties[nameof(Visible)]!;
        set => ShadowProperties[nameof(Visible)] = value;
    }

    /// <summary>
    ///  ControlDesigner overrides this method to handle after-drop cases.
    /// </summary>
    public override void InitializeExistingComponent(IDictionary? defaultValues)
    {
        base.InitializeExistingComponent(defaultValues);

        // unhook any sited children that got ChildWindowTargets
        foreach (Control control in Control.Controls)
        {
            if (control is not null)
            {
                ISite? site = control.Site;
                if (site is not null && control.WindowTarget is ChildWindowTarget target)
                {
                    control.WindowTarget = target.OldWindowTarget;
                }
            }
        }
    }

    /// <summary>
    ///  ControlDesigner overrides this method. It will look at the default property for the control and,
    ///  if it is of type string, it will set this property's value to the name of the component. It only does
    ///  this if the designer has been configured with this option in the options service. This method also
    ///  connects the control to its parent and positions it. If you override this method, you should always
    ///  call base.
    /// </summary>
    public override void InitializeNewComponent(IDictionary? defaultValues)
    {
        ISite? site = Component.Site;
        if (site is not null)
        {
            PropertyDescriptor? textProp = TypeDescriptor.GetProperties(Component)["Text"];
            if (textProp is not null && textProp.PropertyType == typeof(string) && !textProp.IsReadOnly && textProp.IsBrowsable)
            {
                textProp.SetValue(Component, site.Name);
            }
        }

        if (defaultValues is not null && defaultValues["Parent"] is IComponent parent
            && TryGetService(out IDesignerHost? host))
        {
            if (host.GetDesigner(parent) is ParentControlDesigner parentDesigner)
            {
                parentDesigner.AddControl(Control, defaultValues);
            }

            if (parent is Control parentControl)
            {
                // Some containers are docked differently (instead of DockStyle.None) when they are added through the designer
                AttributeCollection attributes = TypeDescriptor.GetAttributes(Component);
                DockingAttribute? dockingAttribute = (DockingAttribute?)attributes[typeof(DockingAttribute)];

                if (dockingAttribute is not null && dockingAttribute.DockingBehavior != DockingBehavior.Never
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
                        PropertyDescriptor? dockProp = TypeDescriptor.GetProperties(Component)["Dock"];
                        if (dockProp is not null && dockProp.IsBrowsable)
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
    ///  Called when the designer is initialized. This allows the designer to provide some meaningful default
    ///  values in the component. The default implementation of this sets the components default property to
    ///  it's name, if that property is a string.
    /// </summary>
    [Obsolete("This method has been deprecated. Use InitializeNewComponent instead.  https://go.microsoft.com/fwlink/?linkid=14202")]
    public override void OnSetComponentDefaults()
    {
        ISite? site = Component.Site;
        if (site is not null)
        {
            PropertyDescriptor? textProp = TypeDescriptor.GetProperties(Component)["Text"];
            if (textProp is not null && textProp.IsBrowsable)
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
            PInvoke.RevokeDragDrop(Control);
        }
    }

    /// <summary>
    ///  Called when a drag-drop operation enters the control designer view
    /// </summary>
    protected virtual void OnDragEnter(DragEventArgs de)
    {
        // unhook our events - we don't want to create an infinite loop.
        Control control = Control;
        DragEventHandler handler = new(OnDragEnter);
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
        DragEventHandler handler = new(OnDragDrop);
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
        EventHandler handler = new(OnDragLeave);
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
        DragEventHandler handler = new(OnDragOver);
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

        // If the CTRL key isn't down, select this component, otherwise, we wait until the mouse up. Make sure the component is selected
        if (!_ctrlSelect && TryGetService(out ISelectionService? selectionService))
        {
            selectionService.SetSelectedComponents(new object[] { Component }, SelectionTypes.Primary);
        }

        Control.Capture = true;
    }

    /// <summary>
    ///  Called at the end of a drag operation. This either commits or rolls back the drag.
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
                ISelectionService? selectionService = GetService<ISelectionService>();
                bool shiftSelect = (Control.ModifierKeys & Keys.Shift) != 0;
                if (!shiftSelect &&
                    (_ctrlSelect
                        || (selectionService is not null && !selectionService.GetComponentSelected(Component))))
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
        if (BehaviorService is not null && BehaviorService.Dragging && cancel)
        {
            BehaviorService.CancelDrag = true;
        }

        // Leave this here in case we are doing a ComponentTray drag
        _selectionUIService ??= GetService<ISelectionUIService>();

        if (_selectionUIService is null)
        {
            return;
        }

        // We must check to ensure that UI service is still in drag mode. It is possible that the user hit escape,
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
        if (TryGetService(out ISelectionService? selectionService) && !Component.Equals(selectionService.PrimarySelection))
        {
            selectionService.SetSelectedComponents(new object[] { Component }, SelectionTypes.Primary | SelectionTypes.Toggle);
        }

        if (BehaviorService is not null && selectionService is not null)
        {
            // create our list of controls-to-drag
            List<IComponent> dragControls = [];
            ICollection selComps = selectionService.GetSelectedComponents();

            // must identify a required parent to avoid dragging mixes of children
            Control? requiredParent = null;
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
                        continue; // mixed selection of different parents - don't add this
                    }

                    if (_host?.GetDesigner(comp) is ControlDesigner des && (des.SelectionRules & SelectionRules.Moveable) != 0)
                    {
                        dragControls.Add(comp);
                    }
                }
            }

            // if we have controls-to-drag, create our new behavior and start the drag/drop operation
            if (dragControls.Count > 0)
            {
                using Graphics adornerGraphics = BehaviorService.AdornerWindowGraphics;
                DropSourceBehavior dsb = new(dragControls, Control.Parent, _mouseDragLast);
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
        Control? parent = ctl;
        object? parentDesigner = null;

        while (parentDesigner is null && parent is not null)
        {
            parent = parent.Parent;
            if (parent is not null)
            {
                object? designer = _host?.GetDesigner(parent);
                if (designer != this)
                {
                    parentDesigner = designer;
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
        Control? parent = ctl;
        object? parentDesigner = null;

        while (parentDesigner is null && parent is not null)
        {
            parent = parent.Parent;
            if (parent is not null)
            {
                object? designer = _host?.GetDesigner(parent);
                if (designer != this)
                {
                    parentDesigner = designer;
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
        Control? parent = ctl;
        object? parentDesigner = null;

        while (parentDesigner is null && parent is not null)
        {
            parent = parent.Parent;
            if (parent is not null)
            {
                object? designer = _host?.GetDesigner(parent);
                if (designer != this)
                {
                    parentDesigner = designer;
                }
            }
        }

        if (parentDesigner is ControlDesigner cd)
        {
            cd.OnMouseLeave();
        }
    }

    /// <summary>
    ///  Called when the control we're designing has finished painting. This method gives the designer a chance
    ///  to paint any additional adornments on top of the control.
    /// </summary>
    protected virtual void OnPaintAdornments(PaintEventArgs pe)
    {
        // If this control is being inherited, paint it
        if (_inheritanceUI is not null && pe.ClipRectangle.IntersectsWith(InheritanceUI.InheritanceGlyphRectangle))
        {
            pe.Graphics.DrawImage(InheritanceUI.InheritanceGlyph, 0, 0);
        }
    }

    /// <summary>
    ///  Called each time the cursor needs to be set.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   The ControlDesigner behavior here will set the cursor to one of three things:
    ///  </para>
    ///  <list type="number">
    ///   <item>
    ///    <description>
    ///     If the toolbox service has a tool selected, it will allow the toolbox service to set the cursor.
    ///    </description>
    ///   </item>
    ///   <item>
    ///    <description>
    ///     If the selection UI service shows a locked selection, or if there is no location property on the
    ///     control, then the default arrow will be set. Otherwise, the four headed arrow will be set to indicate that
    ///     the component can be clicked and moved.
    ///    </description>
    ///   </item>
    ///   <item>
    ///    <description>
    ///     If the user is currently dragging a component, the crosshair cursor will be used instead of the four
    ///     headed arrow.
    ///    </description>
    ///   </item>
    ///  </list>
    /// </remarks>
    protected virtual void OnSetCursor()
    {
        if (Control.Dock != DockStyle.None)
        {
            Cursor.Current = Cursors.Default;
            return;
        }

        _toolboxService ??= GetService<IToolboxService>();

        if (_toolboxService is not null && _toolboxService.SetCursor())
        {
            return;
        }

        if (!_locationChecked)
        {
            _locationChecked = true;
            try
            {
                _hasLocation = TypeDescriptor.GetProperties(Component)["Location"] is not null;
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
    ///  TypeDescriptor object. This method is called immediately before its corresponding "Post" method. If you
    ///  are overriding this method you should call the base implementation before you perform your own filtering.
    /// </summary>
    protected override void PreFilterProperties(IDictionary properties)
    {
        base.PreFilterProperties(properties);

        // Handle shadowed properties
        string[] shadowProps = ["Visible", "Enabled", "AllowDrop", "Location", "Name"];

        for (int i = 0; i < shadowProps.Length; i++)
        {
            if (properties[shadowProps[i]] is PropertyDescriptor prop)
            {
                properties[shadowProps[i]] = TypeDescriptor.CreateProperty(typeof(ControlDesigner), prop, []);
            }
        }

        // replace this one separately because it is of a different type (DesignerControlCollection) than the
        // original property (ControlCollection)
        if (properties["Controls"] is PropertyDescriptor controlsProp)
        {
            Attribute[] attrs = new Attribute[controlsProp.Attributes.Count];
            controlsProp.Attributes.CopyTo(attrs, 0);
            properties["Controls"] = TypeDescriptor.CreateProperty(
                typeof(ControlDesigner),
                "Controls",
                typeof(DesignerControlCollection),
                attrs);
        }

        if (properties["Size"] is PropertyDescriptor sizeProp)
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
    ///  Unhooks the children of the given control. We need to do this for child controls that are not in design
    ///  mode, which is the case for composite controls.
    /// </summary>
    protected void UnhookChildControls(Control firstChild)
    {
        _host ??= GetService<IDesignerHost>();

        foreach (Control child in firstChild.Controls)
        {
            if (child is not null)
            {
                IWindowTarget? oldTarget = child.WindowTarget;
                if (oldTarget is ChildWindowTarget target)
                {
                    child.WindowTarget = target.OldWindowTarget;
                }

                if (oldTarget is not DesignerWindowTarget)
                {
                    UnhookChildControls(child);
                }
            }
        }
    }

    /// <summary>
    ///  This method should be called by the extending designer for each message the control would normally
    ///  receive. This allows the designer to pre-process messages before allowing them to be routed to the control.
    /// </summary>
    protected virtual unsafe void WndProc(ref Message m)
    {
        IMouseHandler? mouseHandler = null;

        // We look at WM_NCHITTEST to determine if the mouse is in a live region of the control
        if (m.MsgInternal == PInvokeCore.WM_NCHITTEST && !_inHitTest)
        {
            _inHitTest = true;
            Point pt = PARAM.ToPoint(m.LParamInternal);
            try
            {
                _liveRegion = GetHitTest(pt);
            }
            catch (Exception e)
            {
                _liveRegion = false;
                if (e.IsCriticalException())
                {
                    throw;
                }
            }

            _inHitTest = false;
        }

        // Check to see if the mouse is in a live region of the control and that the context key is not being fired
        bool isContextKey = m.MsgInternal == PInvokeCore.WM_CONTEXTMENU;
        if (_liveRegion && (IsMouseMessage(m.MsgInternal) || isContextKey))
        {
            // The ActiveX DataGrid control brings up a context menu on right mouse down when it is in edit mode.
            // And, when we generate a WM_CONTEXTMENU message later, it calls DefWndProc() which by default calls
            // the parent (formdesigner). The FormDesigner then brings up the AxHost context menu. This code
            // causes recursive WM_CONTEXTMENU messages to be ignored till we return from the live region message.
            if (m.MsgInternal == PInvokeCore.WM_CONTEXTMENU)
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
                if (m.MsgInternal == PInvokeCore.WM_CONTEXTMENU)
                {
                    s_inContextMenu = false;
                }

                if (m.MsgInternal == PInvokeCore.WM_LBUTTONUP)
                {
                    // terminate the drag. TabControl loses shortcut menu options after adding ActiveX control.
                    OnMouseDragEnd(true);
                }
            }

            return;
        }

        // Get the x and y coordinates of the mouse message
        Point location = default;

        // Look for a mouse handler.
        // CONSIDER - I really don't like this one bit. We need a
        //          : centralized handler so we can do a global override for the tab order
        //          : UI, but the designer is a natural fit for an object oriented UI.
        if ((m.MsgInternal >= PInvokeCore.WM_MOUSEFIRST && m.MsgInternal <= PInvokeCore.WM_MOUSELAST)
            || (m.MsgInternal >= PInvokeCore.WM_NCMOUSEMOVE && m.MsgInternal <= PInvokeCore.WM_NCMBUTTONDBLCLK)
            || m.MsgInternal == PInvokeCore.WM_SETCURSOR)
        {
            _eventService ??= GetService<IEventHandlerService>();

            if (_eventService is not null)
            {
                mouseHandler = (IMouseHandler?)_eventService.GetHandler(typeof(IMouseHandler));
            }
        }

        if (m.MsgInternal >= PInvokeCore.WM_MOUSEFIRST && m.MsgInternal <= PInvokeCore.WM_MOUSELAST)
        {
            location = PARAM.ToPoint(m.LParamInternal);
            PInvokeCore.MapWindowPoints(m, (HWND)default, ref location);
        }
        else if (m.MsgInternal >= PInvokeCore.WM_NCMOUSEMOVE && m.MsgInternal <= PInvokeCore.WM_NCMBUTTONDBLCLK)
        {
            location = PARAM.ToPoint(m.LParamInternal);
        }

        // This is implemented on the base designer for UI activation support. We call it so that we can support
        // UI activation.
        MouseButtons button = MouseButtons.None;
        switch (m.MsgInternal)
        {
            case PInvokeCore.WM_CREATE:
                DefWndProc(ref m);

                // Only call OnCreateHandle if this is our OWN window handle -- the designer window procs are
                // re-entered for child controls.
                if (m.HWnd == Control.Handle)
                {
                    OnCreateHandle();
                }

                break;

            case PInvokeCore.WM_GETOBJECT:
                if (m.LParamInternal == (int)OBJECT_IDENTIFIER.OBJID_CLIENT)
                {
                    m.ResultInternal = AccessibilityObject?.GetLRESULT(m.WParamInternal) ?? default;
                }
                else
                {
                    // m.lparam != OBJID_CLIENT, so do default message processing.
                    DefWndProc(ref m);
                }

                break;

            case PInvokeCore.WM_MBUTTONDOWN:
            case PInvokeCore.WM_MBUTTONUP:
            case PInvokeCore.WM_MBUTTONDBLCLK:
            case PInvokeCore.WM_NCMOUSEHOVER:
            case PInvokeCore.WM_NCMOUSELEAVE:
            case PInvokeCore.WM_MOUSEWHEEL:
            case PInvokeCore.WM_NCMBUTTONDOWN:
            case PInvokeCore.WM_NCMBUTTONUP:
            case PInvokeCore.WM_NCMBUTTONDBLCLK:
                // We intentionally eat these messages.
                break;
            case PInvokeCore.WM_MOUSEHOVER:
                if (mouseHandler is not null)
                {
                    mouseHandler.OnMouseHover(Component);
                }
                else
                {
                    OnMouseHover();
                }

                break;
            case PInvokeCore.WM_MOUSELEAVE:
                OnMouseLeave();
                BaseWndProc(ref m);
                break;
            case PInvokeCore.WM_NCLBUTTONDBLCLK:
            case PInvokeCore.WM_LBUTTONDBLCLK:
            case PInvokeCore.WM_NCRBUTTONDBLCLK:
            case PInvokeCore.WM_RBUTTONDBLCLK:
                button = m.MsgInternal == PInvokeCore.WM_NCRBUTTONDBLCLK || m.MsgInternal == PInvokeCore.WM_RBUTTONDBLCLK
                    ? MouseButtons.Right
                    : MouseButtons.Left;

                if (button == MouseButtons.Left)
                {
                    // We handle doubleclick messages, and we also process our own simulated double clicks for
                    // controls that don't specify CS_WANTDBLCLKS.
                    if (mouseHandler is not null)
                    {
                        mouseHandler.OnMouseDoubleClick(Component);
                    }
                    else
                    {
                        OnMouseDoubleClick();
                    }
                }

                break;
            case PInvokeCore.WM_NCLBUTTONDOWN:
            case PInvokeCore.WM_LBUTTONDOWN:
            case PInvokeCore.WM_NCRBUTTONDOWN:
            case PInvokeCore.WM_RBUTTONDOWN:
                button = m.MsgInternal == PInvokeCore.WM_NCRBUTTONDOWN || m.MsgInternal == PInvokeCore.WM_RBUTTONDOWN
                    ? MouseButtons.Right
                    : MouseButtons.Left;

                // We don't really want the focus, but we want to focus the designer. Below we handle WM_SETFOCUS
                // and do the right thing.
                PInvokeCore.SendMessage(Control, PInvokeCore.WM_SETFOCUS);

                // We simulate doubleclick for things that don't...
                if (button == MouseButtons.Left && IsDoubleClick(location.X, location.Y))
                {
                    if (mouseHandler is not null)
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

                        if (_toolboxService?.GetSelectedToolboxItem(GetService<IDesignerHost>()) is not null)
                        {
                            // There is a tool to be dragged, so set passthrough and pass to the parent.
                            _toolPassThrough = true;
                        }
                    }
                    else
                    {
                        _toolPassThrough = false;
                    }

                    if (_toolPassThrough && Control.Parent is not null)
                    {
                        PInvokeCore.SendMessage(
                            Control.Parent,
                            m.MsgInternal,
                            m.WParamInternal,
                            GetParentPointFromLparam(m.LParamInternal));
                        return;
                    }

                    if (mouseHandler is not null)
                    {
                        mouseHandler.OnMouseDown(Component, button, location.X, location.Y);
                    }
                    else if (button == MouseButtons.Left)
                    {
                        OnMouseDragBegin(location.X, location.Y);
                    }
                    else if (button == MouseButtons.Right)
                    {
                        GetService<ISelectionService>()?.SetSelectedComponents(
                            new object[] { Component },
                            SelectionTypes.Primary);
                    }

                    _lastMoveScreenX = location.X;
                    _lastMoveScreenY = location.Y;
                }

                break;

            case PInvokeCore.WM_NCMOUSEMOVE:
            case PInvokeCore.WM_MOUSEMOVE:
                if (((MODIFIERKEYS_FLAGS)(nint)m.WParamInternal).HasFlag(MODIFIERKEYS_FLAGS.MK_LBUTTON))
                {
                    button = MouseButtons.Left;
                }
                else if (((MODIFIERKEYS_FLAGS)(nint)m.WParamInternal).HasFlag(MODIFIERKEYS_FLAGS.MK_RBUTTON))
                {
                    button = MouseButtons.Right;
                    _toolPassThrough = false;
                }
                else
                {
                    _toolPassThrough = false;
                }

                if (_lastMoveScreenX != location.X || _lastMoveScreenY != location.Y)
                {
                    if (_toolPassThrough && Control.Parent is not null)
                    {
                        PInvokeCore.SendMessage(
                            Control.Parent,
                            m.MsgInternal,
                            m.WParamInternal,
                            GetParentPointFromLparam(m.LParamInternal));
                        return;
                    }

                    if (mouseHandler is not null)
                    {
                        mouseHandler.OnMouseMove(Component, location.X, location.Y);
                    }
                    else if (button == MouseButtons.Left)
                    {
                        OnMouseDragMove(location.X, location.Y);
                    }
                }

                _lastMoveScreenX = location.X;
                _lastMoveScreenY = location.Y;

                // We eat WM_NCMOUSEMOVE messages, since we don't want the non-client area/ of design time
                // controls to repaint on mouse move.
                if (m.MsgInternal == PInvokeCore.WM_MOUSEMOVE)
                {
                    BaseWndProc(ref m);
                }

                break;
            case PInvokeCore.WM_NCLBUTTONUP:
            case PInvokeCore.WM_LBUTTONUP:
            case PInvokeCore.WM_NCRBUTTONUP:
            case PInvokeCore.WM_RBUTTONUP:
                // This is implemented on the base designer for UI activation support.
                button = m.MsgInternal == PInvokeCore.WM_NCRBUTTONUP || m.MsgInternal == PInvokeCore.WM_RBUTTONUP
                    ? MouseButtons.Right
                    : MouseButtons.Left;

                // And terminate the drag.
                if (mouseHandler is not null)
                {
                    mouseHandler.OnMouseUp(Component, button);
                }
                else
                {
                    if (_toolPassThrough && Control.Parent is not null)
                    {
                        PInvokeCore.SendMessage(
                            Control.Parent,
                            m.MsgInternal,
                            m.WParamInternal,
                            GetParentPointFromLparam(m.LParamInternal));
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
            case PInvokeCore.WM_PRINTCLIENT:
                {
                    using Graphics g = Graphics.FromHdc((HDC)m.WParamInternal);
                    using PaintEventArgs e = new(g, Control.ClientRectangle);
                    DefWndProc(ref m);
                    OnPaintAdornments(e);
                }

                break;
            case PInvokeCore.WM_PAINT:
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

                    RECT clip = default;
                    using var hrgn = new RegionScope(0, 0, 0, 0);
                    PInvoke.GetUpdateRgn(m.HWND, hrgn, false);
                    PInvoke.GetUpdateRect(m.HWND, &clip, false);
                    using Region region = hrgn.ToRegion();

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
                        PInvokeCore.MapWindowPoints(m.HWND, Control, ref point);
                        graphics.TranslateTransform(-point.X, -point.Y);
                        PInvokeCore.MapWindowPoints(m.HWND, Control, ref clip);
                    }

                    Rectangle paintRect = clip;
                    using PaintEventArgs pevent = new(graphics, paintRect);

                    graphics.Clip = region;
                    if (_thrownException is null)
                    {
                        OnPaintAdornments(pevent);
                    }
                    else
                    {
                        using BeginPaintScope scope = new(m.HWND);
                        PaintException(pevent, _thrownException);
                    }

                    if (OverlayService is not null)
                    {
                        // This will allow any Glyphs to re-paint after this control and its designer has painted
                        paintRect.Location = Control.PointToScreen(paintRect.Location);
                        OverlayService.InvalidateOverlays(paintRect);
                    }

                    break;
                }

            case PInvokeCore.WM_NCPAINT:
            case PInvokeCore.WM_NCACTIVATE:
                if (m.Msg == (int)PInvokeCore.WM_NCACTIVATE)
                {
                    DefWndProc(ref m);
                }
                else if (_thrownException is null)
                {
                    DefWndProc(ref m);
                }

                // For some reason we don't always get an NCPAINT with the WM_NCACTIVATE usually this repros with
                // themes on.... this can happen when someone calls RedrawWindow without the flags to send an
                // NCPAINT. So that we don't double process this event, our calls to redraw window should not have
                // RDW_ERASENOW | RDW_UPDATENOW.
                if (OverlayService is not null)
                {
                    if (Control is not null && Control.Size != Control.ClientSize && Control.Parent is { } parent)
                    {
                        // we have a non-client region to invalidate
                        Rectangle controlScreenBounds = new(parent.PointToScreen(Control.Location), Control.Size);
                        Rectangle clientAreaScreenBounds = new(Control.PointToScreen(Point.Empty), Control.ClientSize);

                        using Region nonClient = new(controlScreenBounds);
                        nonClient.Exclude(clientAreaScreenBounds);
                        OverlayService.InvalidateOverlays(nonClient);
                    }
                }

                break;

            case PInvokeCore.WM_SETCURSOR:
                // We always handle setting the cursor ourselves.

                if (_liveRegion)
                {
                    DefWndProc(ref m);
                    break;
                }

                if (mouseHandler is not null)
                {
                    mouseHandler.OnSetCursor(Component);
                }
                else
                {
                    OnSetCursor();
                }

                break;
            case PInvokeCore.WM_SIZE:
                if (_thrownException is not null)
                {
                    Control.Invalidate();
                }

                DefWndProc(ref m);
                break;
            case PInvokeCore.WM_CANCELMODE:
                // When we get cancelmode (i.e. you tabbed away to another window) then we want to cancel any
                // pending drag operation!
                OnMouseDragEnd(true);
                DefWndProc(ref m);
                break;
            case PInvokeCore.WM_SETFOCUS:
                // We eat the focus unless the target is a ToolStrip edit node (TransparentToolStrip). If we eat
                // the focus in that case, the Windows Narrator won't follow navigation via the keyboard.
                // NB:  "ToolStrip" is a bit of a misnomer here, because the ToolStripTemplateNode is also used
                // for MenuStrip, StatusStrip, etc...
                // if (Control.FromHandle(m.HWnd) is ToolStripTemplateNode.TransparentToolStrip)
                // {
                //    DefWndProc(ref m);
                // }
                // else
                if (_host is not null && _host.RootComponent is not null && _host.GetDesigner(_host.RootComponent) is IRootDesigner rd)
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
            case PInvokeCore.WM_CONTEXTMENU:
                if (s_inContextMenu)
                {
                    break;
                }

                // We handle this in addition to a right mouse button. Why?  Because we often eat the right mouse
                // button, so it may never generate a WM_CONTEXTMENU. However, the system may generate one in
                // response to an F-10.
                location = PARAM.ToPoint(m.LParamInternal);

                bool handled = GetService<ToolStripKeyboardHandlingService>()?.OnContextMenu(location.X, location.Y) ?? false;

                if (!handled)
                {
                    if (location.X == -1 && location.Y == -1)
                    {
                        // For shift-F10.
                        location = Cursor.Position;
                    }

                    OnContextMenu(location.X, location.Y);
                }

                break;
            default:
                if (m.MsgInternal == RegisteredMessage.WM_MOUSEENTER)
                {
                    OnMouseEnter();
                    BaseWndProc(ref m);
                }
                else if (m.MsgInternal < PInvokeCore.WM_KEYFIRST || m.MsgInternal > PInvokeCore.WM_KEYLAST)
                {
                    // We eat all key handling to the control. Controls generally should not be getting focus
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
        stringFormat.SetMeasurableCharacterRanges([new(0, exceptionText.Length)]);

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

        Rectangle imageRect = new(marginX, marginY, glyphSize.Width, glyphSize.Height);
        Rectangle textRect = clientRectangle;
        textRect.X = imageRect.X + imageRect.Width + 2 * marginX;
        textRect.Y = imageRect.Y;
        textRect.Width -= (textRect.X + marginX + penThickness);
        textRect.Height -= (textRect.Y + marginY + penThickness);

        using (Font errorFont = new(
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

            using (Pen pen = new(Color.Red, penThickness))
            {
                e.Graphics.DrawRectangle(pen, borderRectangle);
            }

            using Icon err = SystemIcons.GetStockIcon(StockIconId.Error);
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

    private IOverlayService? OverlayService => _overlayService ??= GetService<IOverlayService>();

    private static bool IsMouseMessage(MessageId msg) =>
        (msg >= PInvokeCore.WM_MOUSEFIRST && msg <= PInvokeCore.WM_MOUSELAST)
            || (uint)msg switch
            {
                // WM messages not covered by the above block
                PInvokeCore.WM_MOUSEHOVER
                    or PInvokeCore.WM_MOUSELEAVE
                    or PInvokeCore.WM_NCMOUSEMOVE
                    or PInvokeCore.WM_NCLBUTTONDOWN
                    or PInvokeCore.WM_NCLBUTTONUP
                    or PInvokeCore.WM_NCLBUTTONDBLCLK
                    or PInvokeCore.WM_NCRBUTTONDOWN
                    or PInvokeCore.WM_NCRBUTTONUP
                    or PInvokeCore.WM_NCRBUTTONDBLCLK
                    or PInvokeCore.WM_NCMBUTTONDOWN
                    or PInvokeCore.WM_NCMBUTTONUP
                    or PInvokeCore.WM_NCMBUTTONDBLCLK
                    or PInvokeCore.WM_NCMOUSEHOVER
                    or PInvokeCore.WM_NCMOUSELEAVE
                    or PInvokeCore.WM_NCXBUTTONDOWN
                    or PInvokeCore.WM_NCXBUTTONUP
                    or PInvokeCore.WM_NCXBUTTONDBLCLK => true,
                _ => false,
            };

    private bool IsDoubleClick(int x, int y)
    {
        bool doubleClick = false;
        int wait = SystemInformation.DoubleClickTime;
        uint elapsed = PInvoke.GetTickCount() - _lastClickMessageTime;
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
            _lastClickMessageTime = PInvoke.GetTickCount();
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
            if (e.IsCriticalException())
            {
                throw;
            }
        }
    }

    private nint GetParentPointFromLparam(nint lParam)
    {
        Point pt = PARAM.ToPoint(lParam);
        pt = Control.PointToScreen(pt);

        // We have already checked if Parent is null before calling the method.
        pt = Control.Parent!.PointToClient(pt);
        return PARAM.ToInt(pt.X, pt.Y);
    }

    internal void HookChildHandles(HWND firstChild)
    {
        HWND hwndChild = firstChild;
        while (!hwndChild.IsNull)
        {
            if (!IsWindowInCurrentProcess(hwndChild))
            {
                break;
            }

            // Is it a control?
            Control? child = Control.FromHandle(hwndChild);
            if (child is null)
            {
                // No control. We must subclass this control.
                if (!SubclassedChildWindows.ContainsKey(hwndChild))
                {
                    // Some controls (primarily RichEdit) will register themselves as
                    // drag-drop source/targets when they are instantiated. Since these hwnds do not
                    // have a Windows Forms control associated with them, we have to RevokeDragDrop()
                    // for them so that the ParentControlDesigner()'s drag-drop support can work
                    // correctly.
                    PInvoke.RevokeDragDrop(hwndChild);
                    new ChildSubClass(this, hwndChild);
                    SubclassedChildWindows[hwndChild] = true;
                }
            }

            // UserControl is a special ContainerControl which should "hook to all the WindowHandles"
            // Since it doesn't allow the Mouse to pass through any of its contained controls.
            // Please refer to VsWhidbey : 293117
            if (child is null || Control is UserControl)
            {
                // Now do the children of this window.
                HookChildHandles(PInvoke.GetWindow(hwndChild, GET_WINDOW_CMD.GW_CHILD));
            }

            hwndChild = PInvoke.GetWindow(hwndChild, GET_WINDOW_CMD.GW_HWNDNEXT);
        }
    }

    private static bool IsWindowInCurrentProcess(HWND hwnd)
    {
        PInvoke.GetWindowThreadProcessId(hwnd, out uint pid);
        return pid == CurrentProcessId;
    }

    private static uint CurrentProcessId
    {
        get
        {
            if (s_currentProcessId == 0)
            {
                s_currentProcessId = PInvoke.GetCurrentProcessId();
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
        //  1. Child handles that do not have a Control associated with them. We must subclass these and prevent
        //      them from getting design-time events.
        //  2. Child handles that do have a Control associated with them, but the control does not have a designer.
        //      We must hook the WindowTarget on these controls and prevent them from getting design-time events.
        //  3. Child handles that do have a Control associated with them, and the control has a designer. We ignore
        //      these and let the designer handle their messages.
        HookChildHandles(PInvoke.GetWindow(Control, GET_WINDOW_CMD.GW_CHILD));
        HookChildControls(Control);
    }

    internal void RemoveSubclassedWindow(IntPtr hwnd) =>
        SubclassedChildWindows.Remove(hwnd);

    internal void SetUnhandledException(Control? owner, Exception exception)
    {
        if (_thrownException is not null)
        {
            return;
        }

        _thrownException = exception;
        owner ??= Control;

        string? typeName = null;
        string? stack = null;
        if (exception.StackTrace is not null)
        {
            string[] exceptionLines = exception.StackTrace.Split('\r', '\n');
            typeName = owner.GetType().FullName;
            if (typeName is not null)
            {
                stack = string.Join(Environment.NewLine, exceptionLines.Where(l => l.Contains(typeName)));
            }
        }

        InvalidOperationException wrapper = new(
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
