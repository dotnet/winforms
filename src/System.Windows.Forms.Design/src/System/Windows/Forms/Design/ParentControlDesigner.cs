// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms.Design.Behavior;

namespace System.Windows.Forms.Design;

/// <summary>
///  The ParentControlDesigner class builds on the ControlDesigner. It adds the ability
///  to manipulate child components, and provides a selection UI handler for all
///  components it contains.
/// </summary>
public partial class ParentControlDesigner : ControlDesigner, IOleDragClient
{
    private Point _mouseDragBase = InvalidPoint;                        // the base point of the drag
    private Rectangle _mouseDragOffset = Rectangle.Empty;               // always keeps the current rectangle
    private ToolboxItem _mouseDragTool;                                 // the tool that's being dragged, if we're creating a component
    private FrameStyle _mouseDragFrame;                                 // the frame style of this mouse drag

    private OleDragDropHandler _oleDragDropHandler;                     // handler for ole drag drop operations
    private EscapeHandler _escapeHandler;                               // active during drags to override escape.
    private Control _pendingRemoveControl;                              // we've gotten an OnComponentRemoving, and are waiting for OnComponentRemove
    private IComponentChangeService _changeService;
    private DragAssistanceManager _dragManager;                         // used to apply snaplines when dragging a new tool rect on the designer's surface
    private ToolboxSnapDragDropEventArgs _toolboxSnapDragDropEventArgs; // used to store extra info about a beh. svc. dragdrop from the toolbox
    private ToolboxItemSnapLineBehavior _toolboxItemSnapLineBehavior;   // this is our generic snapline box for dragging comps from the toolbox
    private Graphics _graphics;                                         // graphics object of the adornerwindow (via BehaviorService)

    // Services that we keep around for the duration of a drag. you should always check
    // to see if you need to get this service. We cache it, but demand create it.
    private IToolboxService _toolboxService;

    private const int MinGridSize = 2;
    private const int MaxGridSize = 200;

    // designer options...
    private Point _adornerWindowToScreenOffset;                         // quick lookup for offsetting snaplines for new tools

    private bool _checkSnapLineSetting = true;                          // Since layout options is global for the duration of the designer, we should only query it once.
    private bool _defaultUseSnapLines;

    private bool _gridSnap = true;
    private Size _gridSize = Size.Empty;
    private bool _drawGrid = true;

    private bool _parentCanSetDrawGrid = true;                          // since we can inherit the grid/snap setting of our parent,
    private bool _parentCanSetGridSize = true;                          //  these 3 properties let us know if these values have
    private bool _parentCanSetGridSnap = true;                          //  been set explicitly by a user - so to ignore the parent's setting

    private bool _getDefaultDrawGrid = true;
    private bool _getDefaultGridSize = true;
    private bool _getDefaultGridSnap = true;
    private StatusCommandUI _statusCommandUI;                           // UI for setting the StatusBar Information..

    private int _suspendChanging;

    /// <summary>
    ///  This is called after the user selects a toolbox item (that has a ParentControlDesigner
    ///  associated with it) and draws a reversible rectangle on a designer's surface. If
    ///  this property returns true, it is indicating that the Controls that were lasso'd on the
    ///  designer's surface will be re-parented to this designer's control.
    /// </summary>
    protected virtual bool AllowControlLasso => true;

    /// <summary>
    ///  This is called to check whether a generic dragbox should be drawn when dragging a toolbox item
    ///  over the designer's surface.
    /// </summary>
    protected virtual bool AllowGenericDragBox => true;

    /// <summary>
    ///  This is called to check whether the z-order of dragged controls should be maintained when dropped on a
    ///  ParentControlDesigner. By default it will, but e.g. FlowLayoutPanelDesigner wants to do its own z-ordering.
    ///  If this returns true, then the DropSourceBehavior will attempt to set the index of the controls being
    ///  dropped to preserve the original order (in the dragSource). If it returns false, the index will not
    ///  be set.
    ///  If this is set to false, then the DropSourceBehavior will not treat a drag as a local drag even
    ///  if the dragSource and the dragTarget are the same. This will allow a ParentControlDesigner to hook
    ///  OnChildControlAdded to set the right child index, since in this case, the control(s) being dragged
    ///  will be removed from the dragSource and then added to the dragTarget.
    /// </summary>
    protected internal virtual bool AllowSetChildIndexOnDrop => true;

    /// <summary>
    ///  This is called when the component is added to the parent container.
    ///  Theoretically it performs the same function as IsDropOK does, but
    ///  unfortunately IsDropOK is not robust enough and does not allow for specific error messages.
    ///  This method is a chance to display the same error as is displayed at runtime.
    /// </summary>
    protected internal virtual bool CanAddComponent(IComponent component) => true;

    /// <summary>
    ///  This can be called to determine the current grid spacing and mode.
    ///  It is sensitive to what modifier keys the user currently has down and
    ///  will either return the current grid snap dimensions, or a 1x1 point
    ///  indicating no snap.
    /// </summary>
    private Size CurrentGridSize => GridSize;

    /// <summary>
    ///  Determines the default location for a control added to this designer.
    ///  it is usually (0,0), but may be modified if the container has special borders, etc.
    /// </summary>
    protected virtual Point DefaultControlLocation => new(0, 0);

    private bool DefaultUseSnapLines
    {
        get
        {
            if (_checkSnapLineSetting)
            {
                _checkSnapLineSetting = false;
                _defaultUseSnapLines = DesignerUtils.UseSnapLines(Component.Site);
            }

            return _defaultUseSnapLines;
        }
    }

    /// <summary>
    ///  Accessor method for the DrawGrid property. This property determines
    ///  if the grid should be drawn on a control.
    /// </summary>
    protected virtual bool DrawGrid
    {
        get
        {
            // If snaplines are on, the we never want to draw the grid
            if (DefaultUseSnapLines)
            {
                return false;
            }
            else if (_getDefaultDrawGrid)
            {
                _drawGrid = true;

                // Before we check our options page, we need to see if our parent
                // is a ParentControlDesigner, is so, then we will want to inherit all
                // our grid/snap setting from it - instead of our options page
                ParentControlDesigner parent = GetParentControlDesignerOfParent();
                if (parent is not null)
                {
                    _drawGrid = parent.DrawGrid;
                }
                else
                {
                    object value = DesignerUtils.GetOptionValue(ServiceProvider, "ShowGrid");
                    if (value is bool boolValue)
                    {
                        _drawGrid = boolValue;
                    }
                }
            }

            return _drawGrid;
        }

        set
        {
            if (value != _drawGrid)
            {
                if (_parentCanSetDrawGrid)
                {
                    _parentCanSetDrawGrid = false;
                }

                if (_getDefaultDrawGrid)
                {
                    _getDefaultDrawGrid = false;
                }

                _drawGrid = value;

                // invalidate the control to remove or draw the grid based on the new value
                Control control = Control;
                control?.Invalidate(true);

                // now, notify all child parent control designers that we have changed our setting
                // 'cause they might to change along with us, unless the user has explicitly set
                // those values...
                IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
                if (host is not null)
                {
                    // for (int i = 0; i < children.Length; i++) {
                    foreach (Control child in Control.Controls)
                    {
                        ParentControlDesigner designer = host.GetDesigner(child) as ParentControlDesigner;
                        designer?.DrawGridOfParentChanged(_drawGrid);
                    }
                }
            }
        }
    }

    /// <summary>
    ///  Determines whether drag rectangles can be drawn on this designer.
    /// </summary>
    protected override bool EnableDragRect => true;

    internal Size ParentGridSize => GridSize;

    /// <summary>
    ///  Gets/Sets the GridSize property for a form or user control.
    /// </summary>
    protected Size GridSize
    {
        get
        {
            if (_getDefaultGridSize)
            {
                _gridSize = new Size(8, 8);

                // Before we check our options page, we need to see if our parent
                // is a ParentControlDesigner, is so, then we will want to inherit all
                // our grid/snap setting from it - instead of our options page
                ParentControlDesigner parent = GetParentControlDesignerOfParent();
                if (parent is not null)
                {
                    _gridSize = parent.GridSize;
                }
                else
                {
                    object value = DesignerUtils.GetOptionValue(ServiceProvider, "GridSize");
                    if (value is Size size)
                    {
                        _gridSize = size;
                    }
                }
            }

            return _gridSize;
        }
        set
        {
            if (_parentCanSetGridSize)
            {
                _parentCanSetGridSize = false;
            }

            if (_getDefaultGridSize)
            {
                _getDefaultGridSize = false;
            }

            // do some validation checking here, against min & max GridSize
            if (value.Width < MinGridSize || value.Height < MinGridSize ||
                value.Width > MaxGridSize || value.Height > MaxGridSize)
                throw new ArgumentException(string.Format(SR.InvalidArgument,
                                                          "GridSize",
                                                          value.ToString()));
            _gridSize = value;

            // invalidate the control
            Control control = Control;
            control?.Invalidate(true);

            // now, notify all child parent control designers that we have changed our setting
            // 'cause they might to change along with us, unless the user has explicitly set
            // those values...
            IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
            if (host is not null)
            {
                foreach (Control child in Control.Controls)
                {
                    ParentControlDesigner designer = host.GetDesigner(child) as ParentControlDesigner;
                    designer?.GridSizeOfParentChanged(_gridSize);
                }
            }
        }
    }

    /// <summary>
    ///  This property is used by deriving classes to determine if the designer is
    ///  in a state where it has a valid MouseDragTool.
    /// </summary>
    [CLSCompliant(false)]
    protected ToolboxItem MouseDragTool => _mouseDragTool;

    /// <summary>
    ///  This property is used by deriving classes to determine if it returns the control being designed or some other
    ///  Container ...
    ///  while adding a component to it.
    ///  e.g: When SplitContainer is selected and a component is being added ... the SplitContainer designer would return a
    ///  SelectedPanel as the ParentControl for all the items being added rather than itself.
    /// </summary>
    protected virtual Control GetParentForComponent(IComponent component) => Control;

    // We need to allocation new ArrayList and pass it to the caller..
    // So its ok to Suppress this.
    protected void AddPaddingSnapLines(ref ArrayList snapLines)
        => AddPaddingSnapLinesCommon((snapLines ??= new(4)).Adapt<SnapLine>());

    internal void AddPaddingSnapLines(ref IList<SnapLine> snapLines)
        => AddPaddingSnapLinesCommon(snapLines ??= new List<SnapLine>(4));

    private void AddPaddingSnapLinesCommon(IList<SnapLine> snapLines)
    {
        // In order to add padding, we need to get the offset from the usable client area of our control
        // and the actual origin of our control. In other words: how big is the non-client area here?
        // Ex: we want to add padding on a form to the insides of the borders and below the titlebar.
        Point offset = GetOffsetToClientArea();

        // the display rectangle should be the client area combined with the padding value
        Rectangle displayRectangle = Control.DisplayRectangle;
        displayRectangle.X += offset.X; // offset for non-client area
        displayRectangle.Y += offset.Y; // offset for non-client area

        // add the four paddings of our control

        // Even if a control does not have padding, we still want to add Padding snaplines.
        // This is because we only try to match to matching snaplines. Makes the code a little easier...
        snapLines.Add(new SnapLine(SnapLineType.Vertical, displayRectangle.Left, SnapLine.PaddingLeft, SnapLinePriority.Always));
        snapLines.Add(new SnapLine(SnapLineType.Vertical, displayRectangle.Right, SnapLine.PaddingRight, SnapLinePriority.Always));
        snapLines.Add(new SnapLine(SnapLineType.Horizontal, displayRectangle.Top, SnapLine.PaddingTop, SnapLinePriority.Always));
        snapLines.Add(new SnapLine(SnapLineType.Horizontal, displayRectangle.Bottom, SnapLine.PaddingBottom, SnapLinePriority.Always));
    }

    /// <summary>
    ///  Returns a list of SnapLine objects representing interesting
    ///  alignment points for this control. These SnapLines are used
    ///  to assist in the positioning of the control on a parent's
    ///  surface.
    /// </summary>
    public override IList SnapLines
    {
        get
        {
            IList<SnapLine> snapLines = SnapLinesInternal;

            if (snapLines is null)
            {
                Debug.Fail("why did base.SnapLines return null?");
                snapLines = new List<SnapLine>(4);
            }

            AddPaddingSnapLines(ref snapLines);
            return snapLines.Unwrap();
        }
    }

    private IServiceProvider ServiceProvider => Component is not null ? Component.Site : (IServiceProvider)null;

    /// <summary>
    ///  Determines if we should snap to grid or not.
    /// </summary>
    private bool SnapToGrid
    {
        get
        {
            // If snap lines are on, the we never want to snap to grid
            if (DefaultUseSnapLines)
            {
                return false;
            }
            else if (_getDefaultGridSnap)
            {
                _gridSnap = true;

                // Before we check our options page, we need to see if our parent
                // is a ParentControlDesigner, is so, then we will want to inherit all
                // our grid/snap setting from it - instead of our options page
                ParentControlDesigner parent = GetParentControlDesignerOfParent();
                if (parent is not null)
                {
                    _gridSnap = parent.SnapToGrid;
                }
                else
                {
                    object optionValue = DesignerUtils.GetOptionValue(ServiceProvider, "SnapToGrid");
                    if (optionValue is not null and bool)
                    {
                        _gridSnap = (bool)optionValue;
                    }
                }
            }

            return _gridSnap;
        }
        set
        {
            if (_gridSnap != value)
            {
                if (_parentCanSetGridSnap)
                {
                    _parentCanSetGridSnap = false;
                }

                if (_getDefaultGridSnap)
                {
                    _getDefaultGridSnap = false;
                }

                _gridSnap = value;

                // now, notify all child parent control designers that we have changed our setting
                // 'cause they might to change along with us, unless the user has explicitly set
                // those values...
                IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
                if (host is not null)
                {
                    foreach (Control child in Control.Controls)
                    {
                        ParentControlDesigner designer = host.GetDesigner(child) as ParentControlDesigner;
                        designer?.GridSnapOfParentChanged(_gridSnap);
                    }
                }
            }
        }
    }

    internal virtual void AddChildControl(Control newChild)
    {
        if (newChild.Left == 0 && newChild.Top == 0 && newChild.Width >= Control.Width && newChild.Height >= Control.Height)
        {
            // bump the control down one grid size just so it's selectable...
            Point loc = newChild.Location;
            loc.Offset(GridSize.Width, GridSize.Height);
            newChild.Location = loc;
        }

        Control.Controls.Add(newChild);
        Control.Controls.SetChildIndex(newChild, 0);
    }

    internal void AddControl(Control newChild, IDictionary defaultValues)
    {
        Point location = Point.Empty;
        Size size = Size.Empty;
        Size offset = new(0, 0);
        bool hasLocation = (defaultValues is not null && defaultValues.Contains("Location"));
        bool hasSize = (defaultValues is not null && defaultValues.Contains("Size"));

        if (hasLocation)
            location = (Point)defaultValues["Location"];
        if (hasSize)
            size = (Size)defaultValues["Size"];
        if (defaultValues is not null && defaultValues.Contains("Offset"))
        {
            offset = (Size)defaultValues["Offset"];
        }

        // If this component doesn't have a control designer, or if this control
        // is top level, then ignore it. We have the reverse logic in OnComponentAdded
        // in the document designer so that we will add those guys to the tray.
        // Also, if the child-control has already been parented, we assume it's also been located and return immediately.
        // Otherwise, proceed with the parenting and locating.
        IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
        if (host is not null && newChild is not null && !Control.Contains(newChild)
            && host.GetDesigner(newChild) as ControlDesigner is not null && !(newChild is Form form && form.TopLevel))
        {
            Rectangle bounds = default;

            // If we were provided with a location, convert it to parent control coordinates.
            // Otherwise, get the control's size and put the location in the middle of it
            if (hasLocation)
            {
                location = Control.PointToClient(location);
                bounds.X = location.X;
                bounds.Y = location.Y;
            }
            else
            {
                // is the currently selected control this container?
                ISelectionService selSvc = (ISelectionService)GetService(typeof(ISelectionService));
                object primarySelection = selSvc.PrimarySelection;
                Control selectedControl = null;
                if (primarySelection is not null)
                {
                    selectedControl = ((IOleDragClient)this).GetControlForComponent(primarySelection);
                }

                // If the resulting control that came back isn't sited, it's not part of the
                // design surface and should not be used as a marker.
                if (selectedControl is not null && selectedControl.Site is null)
                {
                    selectedControl = null;
                }

                // if the currently selected container is this parent
                // control, default to 0,0
                if (primarySelection == Component || selectedControl is null)
                {
                    bounds.X = DefaultControlLocation.X;
                    bounds.Y = DefaultControlLocation.Y;
                }
                else
                {
                    // otherwise offset from selected control.
                    bounds.X = selectedControl.Location.X + GridSize.Width;
                    bounds.Y = selectedControl.Location.Y + GridSize.Height;
                }
            }

            // If we were not given a size, ask the control for its default. We
            // also update the location here so the control is in the middle of
            // the user's point, rather than at the edge.
            if (hasSize)
            {
                bounds.Width = size.Width;
                bounds.Height = size.Height;
            }
            else
            {
                bounds.Size = GetDefaultSize(newChild);
            }

            // If we were given neither, center the control
            if (!hasSize && !hasLocation)
            {
                // get the adjusted location, then inflate
                // the rect so we can find a nice spot
                // for this control to live.
                Rectangle tempBounds = GetAdjustedSnapLocation(Rectangle.Empty, bounds);

                // compute the stacking location
                tempBounds = GetControlStackLocation(tempBounds);
                bounds = tempBounds;
            }
            else
            {
                // Finally, convert the bounds to the appropriate grid snaps
                bounds = GetAdjustedSnapLocation(Rectangle.Empty, bounds);
            }

            // Adjust for the offset, if any
            bounds.X += offset.Width;
            bounds.Y += offset.Height;

            // check to see if we have additional information for bounds from
            // the behavior service drag drop logic
            if (defaultValues is not null && defaultValues.Contains("ToolboxSnapDragDropEventArgs"))
            {
                ToolboxSnapDragDropEventArgs e = defaultValues["ToolboxSnapDragDropEventArgs"] as ToolboxSnapDragDropEventArgs;
                Rectangle snappedBounds = DesignerUtils.GetBoundsFromToolboxSnapDragDropInfo(e, bounds, Control.IsMirrored);

                // Make sure the snapped bounds intersects with the bounds of the root control before we go
                // adjusting the drag offset. A race condition exists where the user can drag a tbx item so fast
                // that the adorner window will never receive the proper drag/mouse move messages and
                // never properly adjust the snap drag info. This cause the control to be added @ 0,0 w.r.t.
                // the adorner window.
                Control rootControl = host.RootComponent as Control;
                if (rootControl is not null && snappedBounds.IntersectsWith(rootControl.ClientRectangle))
                {
                    bounds = snappedBounds;
                }
            }

            // Parent the control to the designer and set it to the front.
            PropertyDescriptor controlsProp = TypeDescriptor.GetProperties(Control)["Controls"];
            _changeService?.OnComponentChanging(Control, controlsProp);

            AddChildControl(newChild);

            // Now see if the control has size and location properties.
            // Update these values if it does.
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(newChild);
            if (props is not null)
            {
                PropertyDescriptor prop = props["Size"];
                prop?.SetValue(newChild, new Size(bounds.Width, bounds.Height));

                // VSWhidbey# 364133 - ControlDesigner shadows the Location property. If the control is parented
                // and the parent is a scrollable control, then it expects the Location to be in display rectangle coordinates.
                // At this point bounds are in client rectangle coordinates, so we need to check if we need to adjust the coordinates.
                // The reason this worked in Everett was that the AddChildControl was done AFTER this. The AddChildControl was moved
                // above a while back. Not sure what will break if AddChildControl is moved down below, so let's just fix up things
                // here.

                Point pt = new(bounds.X, bounds.Y);
                ScrollableControl p = newChild.Parent as ScrollableControl;
                if (p is not null)
                {
                    Point ptScroll = p.AutoScrollPosition;
                    pt.Offset(-ptScroll.X, -ptScroll.Y); // always want to add the control below/right of the AutoScrollPosition
                }

                prop = props["Location"];
                prop?.SetValue(newChild, pt);
            }

            _changeService?.OnComponentChanged(Control, controlsProp, Control.Controls, Control.Controls);

            newChild.Update();
        }
    }

    /// <summary>
    ///  Adds all the child components of a component
    ///  to the given container
    /// </summary>
    private void AddChildComponents(IComponent component, IContainer container, IDesignerHost host)
    {
        Control control = GetControl(component);
        if (control is null)
        {
            return;
        }

        Control parent = control;

        Control[] children = new Control[parent.Controls.Count];
        parent.Controls.CopyTo(children, 0);

        string name;
        ISite childSite;

        for (int i = 0; i < children.Length; i++)
        {
            childSite = ((IComponent)children[i]).Site;

            IContainer childContainer;
            if (childSite is not null)
            {
                name = childSite.Name;
                if (container.Components[name] is not null)
                {
                    name = null;
                }

                childContainer = childSite.Container;
            }
            else
            {
                // name = null;
                // we don't want to add unsited child controls because
                // these may be items from a composite control. if they
                // are legitimate children, the ComponentModelPersister would have
                // sited them already.
                continue;
            }

            childContainer?.Remove(children[i]);

            if (name is not null)
            {
                container.Add(children[i], name);
            }
            else
            {
                container.Add(children[i]);
            }

            if (children[i].Parent != parent)
            {
                parent.Controls.Add(children[i]);
            }
            else
            {
                int childIndex = parent.Controls.GetChildIndex(children[i]);
                parent.Controls.Remove(children[i]);
                parent.Controls.Add(children[i]);
                parent.Controls.SetChildIndex(children[i], childIndex);
            }

            IComponentInitializer init = host.GetDesigner(component) as IComponentInitializer;
            init?.InitializeExistingComponent(null);

            AddChildComponents(children[i], container, host);
        }
    }

    /// <summary>
    ///  Disposes this component.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // If we are not in a mouse drag, then pretend we are cancelling.
            // This is such that the base will not set the primary selection to be
            // the associated Component. Doing so can cause a crash in hosted designers.
            // It doesn't make sense to do so anyway, since the designer (and thus
            // the component) is being disposed.
            OnMouseDragEnd(_mouseDragBase == InvalidPoint);

            EnableDragDrop(false);

            if (HasComponent && Control is ScrollableControl control)
            {
                control.Scroll -= OnScroll;
            }

            IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
            if (host is not null)
            {
                _changeService.ComponentRemoving -= OnComponentRemoving;
                _changeService.ComponentRemoved -= OnComponentRemoved;
                _changeService = null;
            }
        }

        base.Dispose(disposing);
    }

    /// <summary>
    ///  This is called by the parent when the ParentControlDesigner's
    ///  grid/snap settings have changed. Unless the user has explicitly
    ///  set these values, this designer will just inherit the new ones
    ///  from the parent.
    /// </summary>
    private void DrawGridOfParentChanged(bool drawGrid)
    {
        if (!_parentCanSetDrawGrid)
        {
            return;
        }

        // If the parent sets us, then treat this as if no one set us
        bool getDefaultDrawGridTemp = _getDefaultDrawGrid;
        DrawGrid = drawGrid;
        _parentCanSetDrawGrid = true;
        _getDefaultDrawGrid = getDefaultDrawGridTemp;
    }

    /// <summary>
    ///  This is called by the parent when the ParentControlDesigner's
    ///  grid/snap settings have changed. Unless the user has explicitly
    ///  set these values, this designer will just inherit the new ones
    ///  from the parent.
    /// </summary>
    private void GridSizeOfParentChanged(Size gridSize)
    {
        if (_parentCanSetGridSize)
        {
            // If the parent sets us, then treat this as if no one set us
            bool getDefaultGridSizeTemp = _getDefaultGridSize;
            GridSize = gridSize;
            _parentCanSetGridSize = true;
            _getDefaultGridSize = getDefaultGridSizeTemp;
        }
    }

    /// <summary>
    ///  This is called by the parent when the ParentControlDesigner's
    ///  grid/snap settings have changed. Unless the user has explicitly
    ///  set these values, this designer will just inherit the new ones
    ///  from the parent.
    /// </summary>
    private void GridSnapOfParentChanged(bool gridSnap)
    {
        if (_parentCanSetGridSnap)
        {
            // If the parent sets us, then treat this as if no one set us
            bool getDefaultGridSnapTemp = _getDefaultGridSnap;
            SnapToGrid = gridSnap;
            _parentCanSetGridSnap = true;
            _getDefaultGridSnap = getDefaultGridSnapTemp;
        }
    }

    /// <summary>
    ///  <para>[To be supplied.]</para>
    /// </summary>
    protected static void InvokeCreateTool(ParentControlDesigner toInvoke, ToolboxItem tool)
    {
        toInvoke.CreateTool(tool);
    }

    /// <summary>
    ///  Determines if the this designer can parent to the specified designer --
    ///  generally this means if the control for this designer can parent the
    ///  given ControlDesigner's control.
    /// </summary>
    public virtual bool CanParent(ControlDesigner controlDesigner)
    {
        return CanParent(controlDesigner.Control);
    }

    /// <summary>
    ///  Determines if the this designer can parent to the specified designer --
    ///  generally this means if the control for this designer can parent the
    ///  given ControlDesigner's control.
    /// </summary>
    public virtual bool CanParent(Control control)
    {
        return !control.Contains(Control);
    }

    /// <summary>
    ///  Creates the given tool in the center of the currently selected
    ///  control. The default size for the tool is used.
    /// </summary>
    protected void CreateTool(ToolboxItem tool)
    {
        CreateToolCore(tool, 0, 0, 0, 0, false, false);
    }

    /// <summary>
    ///  Creates the given tool in the currently selected control at the
    ///  given position. The default size for the tool is used.
    /// </summary>
    [CLSCompliant(false)]
    protected void CreateTool(ToolboxItem tool, Point location)
    {
        CreateToolCore(tool, location.X, location.Y, 0, 0, true, false);
    }

    /// <summary>
    ///  Creates the given tool in the currently selected control. The
    ///  tool is created with the provided shape.
    /// </summary>
    [CLSCompliant(false)]
    protected void CreateTool(ToolboxItem tool, Rectangle bounds)
    {
        CreateToolCore(tool, bounds.X, bounds.Y, bounds.Width, bounds.Height, true, true);
    }

    /// <summary>
    ///  This is the worker method of all CreateTool methods. It is the only one
    ///  that can be overridden.
    /// </summary>
    [CLSCompliant(false)]
    protected virtual IComponent[] CreateToolCore(ToolboxItem tool, int x, int y, int width, int height, bool hasLocation, bool hasSize)
    {
        IComponent[] comp = null;

        try
        {
            // We invoke the drag drop handler for this. This implementation is shared between all designers that
            // create components.
            comp = GetOleDragHandler().CreateTool(tool, Control, x, y, width, height, hasLocation, hasSize, _toolboxSnapDragDropEventArgs);
        }
        finally
        {
            // clear the toolboxSnap drag args so we won't provide bad information the next time around
            _toolboxSnapDragDropEventArgs = null;
        }

        return comp;
    }

    /// <summary>
    ///  Used when dragging a new tool rect on the designer's surface -
    ///  this will return some generic snaplines Allowing the rect to
    ///  snap to existing control edges on the surface.
    /// </summary>
    private static SnapLine[] GenerateNewToolSnapLines(Rectangle r)
    {
        return
        [
            new SnapLine(SnapLineType.Left, r.Right),
            new SnapLine(SnapLineType.Right, r.Right),
            new SnapLine(SnapLineType.Bottom, r.Bottom),
            new SnapLine(SnapLineType.Top, r.Bottom)
        ];
    }

    /// <summary>
    ///  Finds the array of components within the given rectangle. This uses the rectangle to
    ///  find controls within our control, and then uses those controls to find the actual
    ///  components. It returns an object list so the output can be directly fed into
    ///  the selection service.
    /// </summary>
    internal List<Control> GetComponentsInRect(Rectangle value, bool screenCoords, bool containRect)
    {
        List<Control> list = [];
        Rectangle rect = screenCoords ? Control.RectangleToClient(value) : value;

        IContainer container = Component.Site.Container;

        Control control = Control;
        int controlCount = control.Controls.Count;

        for (int i = 0; i < controlCount; i++)
        {
            Control child = control.Controls[i];
            Rectangle bounds = child.Bounds;

            container = DesignerUtils.CheckForNestedContainer(container); // ...necessary to support SplitterPanel components

            if (child.Visible && ((containRect && rect.Contains(bounds)) || (!containRect && bounds.IntersectsWith(rect))) &&
                child.Site is not null && child.Site.Container == container)
            {
                list.Add(child);
            }
        }

        return list;
    }

    /// <summary>
    ///  Returns the control that represents the UI for the given component.
    /// </summary>
    protected Control GetControl(object component)
    {
        IComponent comp = component as IComponent;
        if (comp is not null)
        {
            IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
            if (host is not null)
            {
                ControlDesigner cd = host.GetDesigner(comp) as ControlDesigner;
                if (cd is not null)
                {
                    return cd.Control;
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Computes the next default location for a control. It tries to find a spot
    /// where no other controls are being obscured and the new control has 2 corners
    /// that don't have other controls under them.
    /// </summary>
    private Rectangle GetControlStackLocation(Rectangle centeredLocation)
    {
        Control parent = Control;

        int parentHeight = parent.ClientSize.Height;
        int parentWidth = parent.ClientSize.Width;

        if (centeredLocation.Bottom >= parentHeight ||
            centeredLocation.Right >= parentWidth)
        {
            centeredLocation.X = DefaultControlLocation.X;
            centeredLocation.Y = DefaultControlLocation.Y;
        }

        return centeredLocation;
    }

    /// <summary>
    ///  Retrieves the default dimensions for the given component class.
    /// </summary>
    private static Size GetDefaultSize(IComponent component)
    {
        // Check to see if the control is AutoSized. VSWhidbey #416721
        PropertyDescriptor prop = TypeDescriptor.GetProperties(component)["AutoSize"];

        Size size;
        if (prop is not null &&
            !(prop.Attributes.Contains(DesignerSerializationVisibilityAttribute.Hidden) ||
              prop.Attributes.Contains(BrowsableAttribute.No)))
        {
            bool autoSize = (bool)prop.GetValue(component);
            if (autoSize)
            {
                prop = TypeDescriptor.GetProperties(component)["PreferredSize"];
                if (prop is not null)
                {
                    size = (Size)prop.GetValue(component);
                    if (size != Size.Empty)
                    {
                        return size;
                    }
                }
            }
        }

        // attempt to get the size property of our component
        prop = TypeDescriptor.GetProperties(component)["Size"];

        if (prop is not null)
        {
            // first, let's see if we can get a valid size...
            size = (Size)prop.GetValue(component);

            // ...if not, we'll see if there's a default size attribute...
            if (size.Width <= 0 || size.Height <= 0)
            {
                var sizeAttr = (DefaultValueAttribute)prop.Attributes[typeof(DefaultValueAttribute)];
                if (sizeAttr is not null)
                {
                    return ((Size)sizeAttr.Value);
                }
            }
            else
            {
                return size;
            }
        }

        // Couldn't get the size or a def size attrib, returning 75,23...
        return (new Size(75, 23));
    }

    /// <summary>
    ///  Returns a 'BodyGlyph' representing the bounds of this control.
    ///  The BodyGlyph is responsible for hit testing the related CtrlDes
    ///  and forwarding messages directly to the designer.
    /// </summary>
    protected override ControlBodyGlyph GetControlGlyph(GlyphSelectionType selectionType)
    {
        OnSetCursor();

        Rectangle controlRect = BehaviorService.ControlRectInAdornerWindow(Control);

        Control parent = Control.Parent;
        IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));

        if (parent is not null && host is not null && host.RootComponent != Component)
        {
            Rectangle parentRect = BehaviorService.ControlRectInAdornerWindow(parent);
            Rectangle nonClipRect = Rectangle.Intersect(parentRect, controlRect);

            // If we are not selected...

            if (selectionType == GlyphSelectionType.NotSelected)
            {
                // If we are partially clipped (not fully clipped or wholly contained) by
                // our parent,then adjust the bounds of the glyph to be the "visible" rect. VSWhidbey 530929
                if (!nonClipRect.IsEmpty && !parentRect.Contains(controlRect))
                {
                    return new ControlBodyGlyph(nonClipRect, Cursor.Current, Control, this);
                }

                // If we are completely clipped, then we do not want to be a drop target at all
                else if (nonClipRect.IsEmpty)
                {
                    return null;
                }
            }
        }

        return new ControlBodyGlyph(controlRect, Cursor.Current, Control, this);
    }

    /// <summary>
    ///  Adds our ContainerSelectorGlyph to the selection glyphs.
    /// </summary>
    public override GlyphCollection GetGlyphs(GlyphSelectionType selectionType)
    {
        GlyphCollection glyphs = base.GetGlyphs(selectionType);

        // only add this glyph if our container is 1) moveable 2) not read-only
        // AND 3) it is selected .
        if ((SelectionRules & SelectionRules.Moveable) != 0 &&
          InheritanceAttribute != InheritanceAttribute.InheritedReadOnly && selectionType != GlyphSelectionType.NotSelected)
        {
            // get the adornerwindow-relative coords for the container control
            Point loc = BehaviorService.ControlToAdornerWindow((Control)Component);
            Rectangle translatedBounds = new(loc, ((Control)Component).Size);

            int glyphOffset = (int)(DesignerUtils.s_containerGrabHandleSize * .5);

            // if the control is too small for our ideal position...
            if (translatedBounds.Width < 2 * DesignerUtils.s_containerGrabHandleSize)
            {
                glyphOffset = -1 * glyphOffset;
            }

            ContainerSelectorBehavior behavior = new((Control)Component, Component.Site, true);
            ContainerSelectorGlyph containerSelectorGlyph = new(translatedBounds, DesignerUtils.s_containerGrabHandleSize, glyphOffset, behavior);

            glyphs.Insert(0, containerSelectorGlyph);
        }

        return glyphs;
    }

    internal OleDragDropHandler GetOleDragHandler()
    {
        _oleDragDropHandler ??= new OleDragDropHandler(null, (IServiceProvider)GetService(typeof(IDesignerHost)), this);

        return _oleDragDropHandler;
    }

    /// <summary>
    /// This method return the ParentControlDesigner of the parenting control,
    /// it is used for inheriting the grid size, snap to grid, and draw grid
    /// of parenting controls.
    /// </summary>
    private ParentControlDesigner GetParentControlDesignerOfParent()
    {
        Control parent = Control.Parent;
        IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
        if (parent is not null && host is not null)
        {
            return (host.GetDesigner(parent) as ParentControlDesigner);
        }

        return null;
    }

    /// <summary>
    ///  Updates the location of the control according to the GridSnap and Size.
    ///  This method simply calls GetUpdatedRect(), then ignores the width and
    ///  height
    /// </summary>
    private Rectangle GetAdjustedSnapLocation(Rectangle originalRect, Rectangle dragRect)
    {
        Rectangle adjustedRect = GetUpdatedRect(originalRect, dragRect, true);

        // now, preserve the width and height that was originally passed in
        adjustedRect.Width = dragRect.Width;
        adjustedRect.Height = dragRect.Height;

        // we need to keep in mind that if we adjust to the snap, that we could
        // have possibly moved the control's position outside of the display rect.
        // ex: groupbox's display rect.x = 3, but we might snap to 0.
        // so we need to check with the control's designer to make sure this
        // doesn't happen
        Point minimumLocation = DefaultControlLocation;
        if (adjustedRect.X < minimumLocation.X)
        {
            adjustedRect.X = minimumLocation.X;
        }

        if (adjustedRect.Y < minimumLocation.Y)
        {
            adjustedRect.Y = minimumLocation.Y;
        }

        // here's our rect that has been snapped to grid
        return adjustedRect;
    }

    internal Point GetSnappedPoint(Point pt)
    {
        Rectangle r = GetUpdatedRect(Rectangle.Empty, new Rectangle(pt.X, pt.Y, 0, 0), false);
        return new Point(r.X, r.Y);
    }

    internal Rectangle GetSnappedRect(Rectangle originalRect, Rectangle dragRect, bool updateSize)
    {
        return GetUpdatedRect(originalRect, dragRect, updateSize);
    }

    /// <summary>
    ///  Updates the given rectangle, adjusting it for grid snaps as
    ///  needed.
    /// </summary>
    protected Rectangle GetUpdatedRect(Rectangle originalRect, Rectangle dragRect, bool updateSize)
    {
        Rectangle updatedRect;
        if (SnapToGrid)
        {
            Size gridSize = GridSize;
            Point halfGrid = new(gridSize.Width / 2, gridSize.Height / 2);

            updatedRect = dragRect;
            updatedRect.X = originalRect.X;
            updatedRect.Y = originalRect.Y;

            // decide to snap the start location to grid ...
            if (dragRect.X != originalRect.X)
            {
                updatedRect.X = (dragRect.X / gridSize.Width) * gridSize.Width;

                // Snap the location to the grid point closest to the dragRect location
                if (dragRect.X - updatedRect.X > halfGrid.X)
                {
                    updatedRect.X += gridSize.Width;
                }
            }

            if (dragRect.Y != originalRect.Y)
            {
                updatedRect.Y = (dragRect.Y / gridSize.Height) * gridSize.Height;

                // Snap the location to the grid point closest to the dragRect location
                if (dragRect.Y - updatedRect.Y > halfGrid.Y)
                {
                    updatedRect.Y += gridSize.Height;
                }
            }

            // here, we need to calculate the new size depending on how we snap to the grid ...
            if (updateSize)
            {
                // update the width and the height
                updatedRect.Width = ((dragRect.X + dragRect.Width) / gridSize.Width) * gridSize.Width - updatedRect.X;
                updatedRect.Height = ((dragRect.Y + dragRect.Height) / gridSize.Height) * gridSize.Height - updatedRect.Y;

                // ASURT 71552 <subhag> Added so that if the updated dimension is smaller than grid dimension then snap that dimension to
                // the grid dimension
                if (updatedRect.Width < gridSize.Width)
                    updatedRect.Width = gridSize.Width;
                if (updatedRect.Height < gridSize.Height)
                    updatedRect.Height = gridSize.Height;
            }
        }
        else
        {
            updatedRect = dragRect;
        }

        return updatedRect;
    }

    /// <summary>
    ///  Initializes the designer with the given component. The designer can
    ///  get the component's site and request services from it in this call.
    /// </summary>
    public override void Initialize(IComponent component)
    {
        base.Initialize(component);

        if (Control is ScrollableControl control)
        {
            control.Scroll += OnScroll;
        }

        EnableDragDrop(true);

        // Hook load events. At the end of load, we need to do a scan through all
        // of our child controls to see which ones are being inherited. We
        // connect these up.
        IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
        if (host is not null)
        {
            _changeService = (IComponentChangeService)host.GetService(typeof(IComponentChangeService));
            if (_changeService is not null)
            {
                _changeService.ComponentRemoving += OnComponentRemoving;
                _changeService.ComponentRemoved += OnComponentRemoved;
            }
        }

        // update the Status Command
        _statusCommandUI = new StatusCommandUI(component.Site);
    }

    /// <summary>
    /// </summary>
    public override void InitializeNewComponent(IDictionary defaultValues)
    {
        base.InitializeNewComponent(defaultValues);

        if (!AllowControlLasso)
        {
            return;
        }

        if (defaultValues is not null && defaultValues["Size"] is not null && defaultValues["Location"] is not null && defaultValues["Parent"] is not null)
        {
            // build our rect that may have covered some child controls
            Rectangle bounds = new((Point)defaultValues["Location"], (Size)defaultValues["Size"]);

            // ask the parent to give us the comps within this rect
            if (defaultValues["Parent"] is not IComponent parent)
            {
                Debug.Fail("Couldn't get the parent instance from 'defaultValues'");
                return;
            }

            if (GetService(typeof(IDesignerHost)) is not IDesignerHost host)
            {
                Debug.Fail("Failed to IDesignerHost");
                return;
            }

            if (host.GetDesigner(parent) is not ParentControlDesigner parentDesigner)
            {
                Debug.Fail($"Could not get ParentControlDesigner for {parent}");
                return;
            }

            List<Control> selectedControls = parentDesigner.GetComponentsInRect(bounds, true, true /* component should be fully contained*/);

            if (selectedControls is null || selectedControls.Count == 0)
            {
                // no comps to re-parent
                return;
            }

            // remove this
            if (selectedControls.Contains(Control))
            {
                selectedControls.Remove(Control);
            }

            // Finally, we have identified that we need to re-parent the lasso'd controls.
            // We will start a designer transaction, send some changing notifications
            // and swap parents...
            ReParentControls(Control, selectedControls, string.Format(SR.ParentControlDesignerLassoShortcutRedo, Control.Site.Name), host);
        }
    }

    /// <summary>
    ///  Checks if an option has the default value
    /// </summary>
    private bool IsOptionDefault(string optionName, object value)
    {
        IDesignerOptionService optSvc = (IDesignerOptionService)GetService(typeof(IDesignerOptionService));

        object defaultValue = null;

        if (optSvc is null)
        {
            if (optionName.Equals("ShowGrid"))
            {
                defaultValue = true;
            }
            else if (optionName.Equals("SnapToGrid"))
            {
                defaultValue = true;
            }
            else if (optionName.Equals("GridSize"))
            {
                defaultValue = new Size(8, 8);
            }
        }
        else
        {
            defaultValue = DesignerUtils.GetOptionValue(ServiceProvider, optionName);
        }

        if (defaultValue is not null)
        {
            return defaultValue.Equals(value);
        }
        else
        {
            return value is null;
        }
    }

    /// <summary>
    /// </summary>
    private void OnComponentRemoving(object sender, ComponentEventArgs e)
    {
        Control comp = e.Component as Control;
        if (comp is not null && comp.Parent is not null && comp.Parent == Control)
        {
            _pendingRemoveControl = comp;
            // We suspend Component Changing Events for bulk operations to avoid unnecessary serialization\deserialization for undo
            // see bug 488115
            if (_suspendChanging == 0)
            {
                _changeService.OnComponentChanging(Control, TypeDescriptor.GetProperties(Control)["Controls"]);
            }
        }
    }

    /// <summary>
    /// </summary>
    private void OnComponentRemoved(object sender, ComponentEventArgs e)
    {
        if (e.Component == _pendingRemoveControl)
        {
            _pendingRemoveControl = null;
            _changeService.OnComponentChanged(Control, TypeDescriptor.GetProperties(Control)["Controls"]);
        }
    }

    internal void SuspendChangingEvents()
    {
        _suspendChanging++;
        Debug.Assert(_suspendChanging > 0, "Unbalanced SuspendChangingEvents\\ResumeChangingEvents");
    }

    internal void ResumeChangingEvents()
    {
        _suspendChanging--;
        Debug.Assert(_suspendChanging >= 0, "Unbalanced SuspendChangingEvents\\ResumeChangingEvents");
    }

    internal void ForceComponentChanging()
    {
        _changeService.OnComponentChanging(Control, TypeDescriptor.GetProperties(Control)["Controls"]);
    }

    /// <summary>
    ///  Called in order to cleanup a drag and drop operation. Here we
    ///  cleanup any operations that were performed at the beginning of a drag.
    /// </summary>
    protected override void OnDragComplete(DragEventArgs de)
    {
        DropSourceBehavior.BehaviorDataObject data = de.Data as DropSourceBehavior.BehaviorDataObject;
        data?.CleanupDrag();
    }

    /// <summary>
    ///  Called in response to a drag drop for OLE drag and drop. Here we
    ///  drop a toolbox component on our parent control.
    /// </summary>
    // Standard 'catch all - rethrow critical' exception pattern
    protected override void OnDragDrop(DragEventArgs de)
    {
        // if needed, cache extra info about the behavior dragdrop event
        // ex: snapline and offset info
        if (de is ToolboxSnapDragDropEventArgs)
        {
            _toolboxSnapDragDropEventArgs = de as ToolboxSnapDragDropEventArgs;
        }

        DropSourceBehavior.BehaviorDataObject data = de.Data as DropSourceBehavior.BehaviorDataObject;
        if (data is not null)
        {
            data.Target = Component;
            data.EndDragDrop(AllowSetChildIndexOnDrop);

            OnDragComplete(de);
        }

        // this should only occur when D&Ding between component trays on two separate forms.
        else if (_mouseDragTool is null && data is null)
        {
            OleDragDropHandler ddh = GetOleDragHandler();
            if (ddh is not null)
            {
                IOleDragClient target = ddh.Destination;
                if (target is not null && target.Component is not null && target.Component.Site is not null)
                {
                    IContainer container = target.Component.Site.Container;
                    if (container is not null)
                    {
                        object[] dragComps = OleDragDropHandler.GetDraggingObjects(de);
                        for (int i = 0; i < dragComps.Length; i++)
                        {
                            IComponent comp = dragComps[i] as IComponent;
                            container.Add(comp);
                        }
                    }
                }
            }
        }

        if (_mouseDragTool is not null)
        {
            IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
            host?.Activate();

            try
            {
                // There may be a wizard displaying as a result of CreateTool.
                // we do not want the behavior service thinking there he is dragging while this wizard is up
                // it causes the cursor to constantly flicker to the toolbox cursor.
                // this will cause the BehSvc to return from 'drag mode'
                BehaviorService?.EndDragNotification();

                CreateTool(_mouseDragTool, new Point(de.X, de.Y));
            }
            catch (Exception e) when (!e.IsCriticalException())
            {
                DisplayError(e);
            }

            _mouseDragTool = null;
            return;
        }
    }

    /// <summary>
    ///  Called in response to a drag enter for OLE drag and drop.
    /// </summary>
    protected override void OnDragEnter(DragEventArgs de)
    {
        // Are we are new target, meaning is the drop target different than the drag source
        bool newTarget = false;

        DropSourceBehavior.BehaviorDataObject behDataObject = null;
        DropSourceBehavior.BehaviorDataObject data = de.Data as DropSourceBehavior.BehaviorDataObject;
        if (data is not null)
        {
            behDataObject = data;
            behDataObject.Target = Component;
            de.Effect = (Control.ModifierKeys == Keys.Control) ? DragDropEffects.Copy : DragDropEffects.Move;
            newTarget = !(data.Source.Equals(Component)); // Check if we are moving to a new target
        }

        // If tab order UI is being shown, then don't allow anything to be
        // dropped here.
        IMenuCommandService ms = (IMenuCommandService)GetService(typeof(IMenuCommandService));
        if (ms is not null)
        {
            MenuCommand tabCommand = ms.FindCommand(StandardCommands.TabOrder);
            if (tabCommand is not null && tabCommand.Checked)
            {
                de.Effect = DragDropEffects.None;
                return;
            }
        }

        // Get the objects that are being dragged
        object[] dragComps;
        if (behDataObject is not null && behDataObject.DragComponents is not null)
        {
            dragComps = behDataObject.DragComponents.ToArray();
        }
        else
        {
            // Keep GetOleDragHandler() for compat.
            _ = GetOleDragHandler();
            dragComps = OleDragDropHandler.GetDraggingObjects(de);
        }

        Control draggedControl = null;

        IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
        if (host is not null)
        {
            DocumentDesigner parentDesigner = host.GetDesigner(host.RootComponent) as DocumentDesigner;
            if (parentDesigner is not null)
            {
                if (!parentDesigner.CanDropComponents(de))
                {
                    de.Effect = DragDropEffects.None;
                    return;
                }
            }
        }

        if (dragComps is not null)
        {
            if (data is null)
            {
                // This should only be true, when moving a component from the Tray,
                // to a new form. In this case, we are moving targets.
                newTarget = true;
            }

            for (int i = 0; i < dragComps.Length; i++)
            {
                if (host is null || dragComps[i] is not IComponent comp)
                {
                    continue;
                }

                if (newTarget)
                {
                    // If we are dropping on a new target, then check to see if any of the components
                    // are inherited. If so, don't allow them to be moved.
                    InheritanceAttribute attr = (InheritanceAttribute)TypeDescriptor.GetAttributes(comp)[typeof(InheritanceAttribute)];
                    if (attr is not null && !attr.Equals(InheritanceAttribute.NotInherited) && !attr.Equals(InheritanceAttribute.InheritedReadOnly))
                    {
                        de.Effect = DragDropEffects.None;
                        return;
                    }
                }

                // try go get the control for the thing that's being dragged
                object draggedDesigner = host.GetDesigner(comp);
                if (draggedDesigner is IOleDragClient)
                {
                    draggedControl = ((IOleDragClient)this).GetControlForComponent(dragComps[i]);
                }

                Control ctrl = dragComps[i] as Control;
                if (draggedControl is null && ctrl is not null)
                {
                    draggedControl = ctrl;
                }

                // oh well, it's not a control so it doesn't matter
                if (draggedControl is null)
                {
                    continue;
                }

                // If we're inheriting from a private container, we can't modify the controls collection.
                // So drag-drop is only allowed within the container i.e. the dragged controls must already
                // be parented to this container.
                if (InheritanceAttribute == InheritanceAttribute.InheritedReadOnly && draggedControl.Parent != Control)
                {
                    de.Effect = DragDropEffects.None;
                    return;
                }

                // Can the component be dropped on this parent? I.e. you can only
                // drop a tab page on a tab control, not say a panel
                if (!((IOleDragClient)this).IsDropOk(comp))
                {
                    de.Effect = DragDropEffects.None;
                    return;
                }
            }

            // should only occur when dragging and dropping
            // from the component tray.
            if (data is null)
            {
                PerformDragEnter(de, host);
            }
        }

        _toolboxService ??= (IToolboxService)GetService(typeof(IToolboxService));

        // Only assume the items came from the ToolBox if dragComps == null
        if (_toolboxService is not null && dragComps is null)
        {
            _mouseDragTool = _toolboxService.DeserializeToolboxItem(de.Data, host);

            // If we have a valid toolbox item to drag and
            // we haven't pushed our behavior, then do so now...
            if ((_mouseDragTool is not null) && BehaviorService is not null && BehaviorService.UseSnapLines)
            {
                // demand create
                _toolboxItemSnapLineBehavior ??= new ToolboxItemSnapLineBehavior(Component.Site, BehaviorService, this, AllowGenericDragBox);

                if (!_toolboxItemSnapLineBehavior.IsPushed)
                {
                    BehaviorService.PushBehavior(_toolboxItemSnapLineBehavior);
                    _toolboxItemSnapLineBehavior.IsPushed = true;
                }
            }

            if (_mouseDragTool is not null)
            {
                PerformDragEnter(de, host);
            }

            // This must be called last. Tell the behavior that we are beginning a drag.
            // Yeah, this is OnDragEnter, but to the behavior this is as if we are starting a drag.
            // VSWhidbey 487816
            _toolboxItemSnapLineBehavior?.OnBeginDrag();
        }
    }

    private void PerformDragEnter(DragEventArgs de, IDesignerHost host)
    {
        host?.Activate();

        Debug.Assert((de.AllowedEffect & (DragDropEffects.Move | DragDropEffects.Copy)) != 0, "DragDropEffect.Move | .Copy isn't allowed?");
        if ((de.AllowedEffect & DragDropEffects.Move) != 0)
        {
            de.Effect = DragDropEffects.Move;
        }
        else
        {
            de.Effect = DragDropEffects.Copy;
        }

        // If we're inheriting from a private container, we can't modify the controls collection.
        if (InheritanceAttribute == InheritanceAttribute.InheritedReadOnly)
        {
            de.Effect = DragDropEffects.None;
            return;
        }

        // Also, select this parent control to indicate it will be the drop target.
        ISelectionService sel = (ISelectionService)GetService(typeof(ISelectionService));
        sel?.SetSelectedComponents(new object[] { Component }, SelectionTypes.Replace);
    }

    /// <summary>
    ///  Called when a drag-drop operation leaves the control designer view
    /// </summary>
    protected override void OnDragLeave(EventArgs e)
    {
        // if we're dragging around our generic snapline box - let's remove it here
        if (_toolboxItemSnapLineBehavior is not null && _toolboxItemSnapLineBehavior.IsPushed)
        {
            BehaviorService.PopBehavior(_toolboxItemSnapLineBehavior);
            _toolboxItemSnapLineBehavior.IsPushed = false;
        }

        _mouseDragTool = null;
    }

    /// <summary>
    ///  Called when a drag drop object is dragged over the control designer view
    /// </summary>
    protected override void OnDragOver(DragEventArgs de)
    {
        if (de.Data is DropSourceBehavior.BehaviorDataObject data)
        {
            data.Target = Component;
            de.Effect = (Control.ModifierKeys == Keys.Control) ? DragDropEffects.Copy : DragDropEffects.Move;
        }

        // If tab order UI is being shown, then don't allow anything to be dropped here.
        IMenuCommandService ms = (IMenuCommandService)GetService(typeof(IMenuCommandService));
        if (ms is not null)
        {
            MenuCommand tabCommand = ms.FindCommand(StandardCommands.TabOrder);
            Debug.Assert(tabCommand is not null, "Missing tab order command");
            if (tabCommand is not null && tabCommand.Checked)
            {
                de.Effect = DragDropEffects.None;
                return;
            }
        }

        IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
        if (host?.GetDesigner(host.RootComponent) is DocumentDesigner parentDesigner)
        {
            if (!parentDesigner.CanDropComponents(de))
            {
                de.Effect = DragDropEffects.None;
                return;
            }
        }

        if (_mouseDragTool is not null)
        {
            Debug.Assert((de.AllowedEffect & DragDropEffects.Copy) != 0, "DragDropEffect.Move isn't allowed?");
            de.Effect = DragDropEffects.Copy;
            return;
        }
    }

    /// <summary>
    ///  Called in response to the left mouse button being pressed on a
    ///  component. The designer overrides this to provide a
    ///  "lasso" selection for components within the control.
    /// </summary>

    private static int FrameWidth(FrameStyle style)
    {
        return (style == FrameStyle.Dashed ? 1 : 2);
    }

    protected override void OnMouseDragBegin(int x, int y)
    {
        Control control = Control;

        // Figure out the drag frame style. We use a dotted line for selecting
        // a component group, and a thick line for creating a new component.
        // If we are a privately inherited component, then we always use the
        // selection frame because we can't add components.
        if (!InheritanceAttribute.Equals(InheritanceAttribute.InheritedReadOnly))
        {
            _toolboxService ??= (IToolboxService)GetService(typeof(IToolboxService));

            if (_toolboxService is not null)
            {
                _mouseDragTool = _toolboxService.GetSelectedToolboxItem((IDesignerHost)GetService(typeof(IDesignerHost)));
            }
        }

        // Set the mouse capture and clipping to this control.
        control.Capture = true;

        _mouseDragFrame = (_mouseDragTool is null) ? FrameStyle.Dashed : FrameStyle.Thick;

        // Setting this non-null signifies that we are dragging with the mouse.
        _mouseDragBase = new Point(x, y);

        // Select the given object.
        ISelectionService selsvc = (ISelectionService)GetService(typeof(ISelectionService));

        selsvc?.SetSelectedComponents(new object[] { Component }, SelectionTypes.Primary);

        // Get the event handler service. We push a handler to handle the escape
        // key.
        IEventHandlerService eventSvc = (IEventHandlerService)GetService(typeof(IEventHandlerService));
        // UNDONE: Behavior Work
        // Debug.Assert(escapeHandler == null, "Why is there already an escape handler?");

        if (eventSvc is not null && _escapeHandler is null)
        {
            _escapeHandler = new EscapeHandler(this);
            eventSvc.PushHandler(_escapeHandler);
        }

        // Need this since we are drawing the frame in the adorner window
        _adornerWindowToScreenOffset = BehaviorService.AdornerWindowToScreen();
    }

    /// <summary>
    ///  Called at the end of a drag operation. This either commits or rolls back the
    ///  drag.
    /// </summary>
    // Standard 'catch all - rethrow critical' exception pattern
    protected override void OnMouseDragEnd(bool cancel)
    {
        // Do nothing if we're not dragging anything around
        if (_mouseDragBase == InvalidPoint)
        {
            Debug.Assert(_graphics is null);
            // make sure we force the drag end
            base.OnMouseDragEnd(cancel);
            return;
        }

        // Important to null these out here, just in case we throw an exception
        Rectangle offset = _mouseDragOffset;
        ToolboxItem tool = _mouseDragTool;
        Point baseVar = _mouseDragBase;

        _mouseDragOffset = Rectangle.Empty;
        _mouseDragBase = InvalidPoint;
        _mouseDragTool = null;

        Control.Capture = false;
        Cursor.Clip = Rectangle.Empty;

        // Clear out the drag frame.
        if (!offset.IsEmpty && _graphics is not null)
        {
            Rectangle frameRect = new(offset.X - _adornerWindowToScreenOffset.X,
                                                 offset.Y - _adornerWindowToScreenOffset.Y,
                                                 offset.Width, offset.Height);

            int frameWidth = FrameWidth(_mouseDragFrame);
            _graphics.SetClip(frameRect);

            using (Region newRegion = new(frameRect))
            {
                newRegion.Exclude(Rectangle.Inflate(frameRect, -frameWidth, -frameWidth));
                BehaviorService.Invalidate(newRegion);
            }

            _graphics.ResetClip();
        }

        if (_graphics is not null)
        {
            _graphics.Dispose();
            _graphics = null;
        }

        // destroy the snapline engine (if we used it)
        if (_dragManager is not null)
        {
            _dragManager.OnMouseUp();
            _dragManager = null;
        }

        // Get the event handler service and pop our handler.
        IEventHandlerService eventSvc = (IEventHandlerService)GetService(typeof(IEventHandlerService));
        if (eventSvc is not null && _escapeHandler is not null)
        {
            eventSvc.PopHandler(_escapeHandler);
            _escapeHandler = null;
        }

        // Set Status Information - but only if the offset is not empty, if it is, the user didn't move the mouse
        if (_statusCommandUI is not null && !offset.IsEmpty)
        {
            Point location = new(baseVar.X, baseVar.Y);
            location = Control.PointToClient(location);
            _statusCommandUI?.SetStatusInformation(new Rectangle(location.X, location.Y, offset.Width, offset.Height));
        }

        // Quit now if we don't have an offset rect. This indicates that the user didn't move the mouse.
        if (offset.IsEmpty && !cancel)
        {
            // BUT, if we have a selected tool, create it here
            if (tool is not null)
            {
                try
                {
                    CreateTool(tool, baseVar);
                    _toolboxService?.SelectedToolboxItemUsed();
                }
                catch (Exception e) when (!e.IsCriticalException())
                {
                    DisplayError(e);
                }
            }

            return;
        }

        // Don't do anything else if the user wants to cancel.
        if (cancel)
        {
            return;
        }

        // If we have a valid toolbox item, create the tool
        if (tool is not null)
        {
            try
            {
                // avoid allowing the user creating a 1x1 sized control (for ex)
                // by enforcing a min size 2xMinDragSize...
                Size minControlSize = new(DesignerUtils.MinDragSize.Width * 2, DesignerUtils.MinDragSize.Height * 2);
                if (offset.Width < minControlSize.Width)
                {
                    offset.Width = minControlSize.Width;
                }

                if (offset.Height < minControlSize.Height)
                {
                    offset.Height = minControlSize.Height;
                }

                CreateTool(tool, offset);
                _toolboxService?.SelectedToolboxItemUsed();
            }
            catch (Exception e) when (!e.IsCriticalException())
            {
                DisplayError(e);
            }
        }
        else
        {
            // Now find the set of controls within this offset and select them.
            var selSvc = (ISelectionService)GetService(typeof(ISelectionService));
            if (selSvc is not null)
            {
                List<Control> selection = GetComponentsInRect(offset, true, false /*component does not need to be fully contained*/);
                if (selection.Count > 0)
                {
                    selSvc.SetSelectedComponents(selection);
                }
            }
        }
    }

    /// <summary>
    ///  Called for each movement of the mouse. This will check to see if a drag operation
    ///  is in progress. If so, it will pass the updated drag dimensions on to the selection
    ///  UI service.
    /// </summary>
    protected override void OnMouseDragMove(int x, int y)
    {
        // if we pushed a snapline behavior during a drag operation - make sure we have popped it
        // if we're now receiving mouse move messages.
        if (_toolboxItemSnapLineBehavior is not null && _toolboxItemSnapLineBehavior.IsPushed)
        {
            BehaviorService.PopBehavior(_toolboxItemSnapLineBehavior);
            _toolboxItemSnapLineBehavior.IsPushed = false;
        }

        // if we're doing an OLE drag, do nothing, or
        // Do nothing if we haven't initiated a drag
        if (GetOleDragHandler().Dragging || _mouseDragBase == InvalidPoint)
        {
            return;
        }

        Rectangle oldFrameRect = _mouseDragOffset;

        // Calculate the new offset.
        _mouseDragOffset.X = _mouseDragBase.X;
        _mouseDragOffset.Y = _mouseDragBase.Y;
        _mouseDragOffset.Width = x - _mouseDragBase.X;
        _mouseDragOffset.Height = y - _mouseDragBase.Y;

        // if we have a valid dragtool - then we'll spin up our snapline engine
        // and use it when the user drags a reversible rect -- but only if the
        // parentcontroldesigner wants to allow Snaplines

        if (_dragManager is null && ParticipatesWithSnapLines && _mouseDragTool is not null && BehaviorService.UseSnapLines)
        {
            _dragManager = new DragAssistanceManager(Component.Site);
        }

        if (_dragManager is not null)
        {
            // here, we build up our new rect (offset by the adorner window)
            // and ask the snapline engine to adjust our coords
            Rectangle r = new(_mouseDragBase.X - _adornerWindowToScreenOffset.X,
                                   _mouseDragBase.Y - _adornerWindowToScreenOffset.Y,
                                   x - _mouseDragBase.X, y - _mouseDragBase.Y);
            Point offset = _dragManager.OnMouseMove(r, GenerateNewToolSnapLines(r));
            _mouseDragOffset.Width += offset.X;
            _mouseDragOffset.Height += offset.Y;
            _dragManager.RenderSnapLinesInternal();
        }

        if (_mouseDragOffset.Width < 0)
        {
            _mouseDragOffset.X += _mouseDragOffset.Width;
            _mouseDragOffset.Width = -_mouseDragOffset.Width;
        }

        if (_mouseDragOffset.Height < 0)
        {
            _mouseDragOffset.Y += _mouseDragOffset.Height;
            _mouseDragOffset.Height = -_mouseDragOffset.Height;
        }

        // If we're dragging out a new component, update the drag rectangle
        // to use snaps, if they're set.
        if (_mouseDragTool is not null)
        {
            // To snap properly, we must snap in client coordinates. So, convert, snap
            // and re-convert.
            _mouseDragOffset = Control.RectangleToClient(_mouseDragOffset);
            _mouseDragOffset = GetUpdatedRect(Rectangle.Empty, _mouseDragOffset, true);
            _mouseDragOffset = Control.RectangleToScreen(_mouseDragOffset);
        }

        _graphics ??= BehaviorService.AdornerWindowGraphics;

        // And draw the new drag frame
        if (!_mouseDragOffset.IsEmpty && _graphics is not null)
        {
            Rectangle frameRect = new(_mouseDragOffset.X - _adornerWindowToScreenOffset.X,
                                                 _mouseDragOffset.Y - _adornerWindowToScreenOffset.Y,
                                                 _mouseDragOffset.Width, _mouseDragOffset.Height);

            // graphics.SetClip(frameRect);

            // draw the new border
            using Region newRegion = new(frameRect);
            int frameWidth = FrameWidth(_mouseDragFrame);
            newRegion.Exclude(Rectangle.Inflate(frameRect, -frameWidth, -frameWidth));

            // erase the right part of the old frame
            if (!oldFrameRect.IsEmpty)
            {
                oldFrameRect.X -= _adornerWindowToScreenOffset.X;
                oldFrameRect.Y -= _adornerWindowToScreenOffset.Y;

                // Let's not try and be smart about invalidating just the part of the old frame
                // that's not part of the new frame. When I did that (using the commented out
                // lines below), you could get serious screen artifacts when dragging fast. I think
                // this might be because of some bad region forming (bad region, bad), or some missing
                // updates.

                // Since we invalidate and then immediately redraw, the flicker should be minimal.
                using Region oldRegion = new(oldFrameRect);
                oldRegion.Exclude(Rectangle.Inflate(oldFrameRect, -frameWidth, -frameWidth));
                // oldRegion.Union(newRegion);
                // oldRegion.Exclude(newRegion);
                BehaviorService.Invalidate(oldRegion);
            }

            DesignerUtils.DrawFrame(_graphics, newRegion, _mouseDragFrame, Control.BackColor);

            // graphics.ResetClip();
        }

        // We are looking at the primary control
        if (_statusCommandUI is not null)
        {
            Point offset = new(_mouseDragOffset.X, _mouseDragOffset.Y);
            offset = Control.PointToClient(offset);
            _statusCommandUI?.SetStatusInformation(new Rectangle(offset.X, offset.Y, _mouseDragOffset.Width, _mouseDragOffset.Height));
        }
    }

    /// <summary>
    ///  Called after our component has finished painting. Here we draw our grid surface
    /// </summary>
    protected override void OnPaintAdornments(PaintEventArgs pe)
    {
        if (DrawGrid)
        {
            Control control = Control;

            Rectangle displayRect = Control.DisplayRectangle;
            Rectangle clientRect = Control.ClientRectangle;

            Rectangle paintRect = new(Math.Min(displayRect.X, clientRect.X),
                                      Math.Min(displayRect.Y, clientRect.Y),
                                      Math.Max(displayRect.Width, clientRect.Width),
                                      Math.Max(displayRect.Height, clientRect.Height));

            float xlateX = paintRect.X;
            float xlateY = paintRect.Y;
            pe.Graphics.TranslateTransform(xlateX, xlateY);
            paintRect.X = paintRect.Y = 0;
            paintRect.Width++; // gpr: FillRectangle with a TextureBrush comes up one pixel short
            paintRect.Height++;
            ControlPaint.DrawGrid(pe.Graphics, paintRect, GridSize, control.BackColor);
            pe.Graphics.TranslateTransform(-xlateX, -xlateY);
        }

        base.OnPaintAdornments(pe);
    }

    /// <summary>
    ///  When the control is scrolled, we want to invalidate areas previously covered by glyphs.
    /// </summary>
    private void OnScroll(object sender, ScrollEventArgs se)
    {
        BehaviorService.Invalidate(BehaviorService.ControlRectInAdornerWindow(Control));
    }

    /// <summary>
    ///  Called each time the cursor needs to be set. The ParentControlDesigner behavior here
    ///  will set the cursor to one of three things:
    ///  1. If the toolbox service has a tool selected, it will allow the toolbox service to
    ///  set the cursor.
    ///  2. The arrow will be set. Parent controls allow dragging within their interior.
    /// </summary>
    protected override void OnSetCursor()
    {
        _toolboxService ??= (IToolboxService)GetService(typeof(IToolboxService));

        try
        {
            if (_toolboxService is null || !_toolboxService.SetCursor() || InheritanceAttribute.Equals(InheritanceAttribute.InheritedReadOnly))
            {
                Cursor.Current = Cursors.Default;
            }
        }

        catch
        {  // VSWhidbey 502536
            Cursor.Current = Cursors.Default;
        }
    }

    /// <summary>
    ///  Allows a designer to filter the set of properties
    ///  the component it is designing will expose through the
    ///  TypeDescriptor object. This method is called
    ///  immediately before its corresponding "Post" method.
    ///  If you are overriding this method you should call
    ///  the base implementation before you perform your own
    ///  filtering.
    /// </summary>
    protected override void PreFilterProperties(IDictionary properties)
    {
        base.PreFilterProperties(properties);

        // add the "GridSize, SnapToGrid and DrawGrid" property from the property grid
        // iff the LayoutOption.SnapToGrid Attribute is Set...

        if (!DefaultUseSnapLines)
        {
            properties["DrawGrid"] = TypeDescriptor.CreateProperty(typeof(ParentControlDesigner), "DrawGrid", typeof(bool),
                                                          BrowsableAttribute.Yes,
                                                          DesignOnlyAttribute.Yes,
                                                          new SRDescriptionAttribute("ParentControlDesignerDrawGridDescr"),
                                                          CategoryAttribute.Design);

            properties["SnapToGrid"] = TypeDescriptor.CreateProperty(typeof(ParentControlDesigner), "SnapToGrid", typeof(bool),
                                                            BrowsableAttribute.Yes,
                                                            DesignOnlyAttribute.Yes,
                                                            new SRDescriptionAttribute("ParentControlDesignerSnapToGridDescr"),
                                                            CategoryAttribute.Design);

            properties["GridSize"] = TypeDescriptor.CreateProperty(typeof(ParentControlDesigner), "GridSize", typeof(Size),
                                                          BrowsableAttribute.Yes,
                                                          new SRDescriptionAttribute(SR.ParentControlDesignerGridSizeDescr),
                                                          DesignOnlyAttribute.Yes,
                                                          CategoryAttribute.Design);
        }

        // We need this one always to make sure that Format -> Horizontal/Vertical Spacing works.
        properties["CurrentGridSize"] = TypeDescriptor.CreateProperty(typeof(ParentControlDesigner), "CurrentGridSize", typeof(Size),
                                                         BrowsableAttribute.No,
                                                         DesignerSerializationVisibilityAttribute.Hidden);
    }

    /// <summary>
    ///  Called after we have decided that the user has drawn a control (with a toolbox item picked) onto the designer
    ///  surface and intends to have the controls beneath the new one re-parented. Example: A user selects the 'Panel'
    ///  Control in the toolbox then drags a rectangle around four Buttons on the Form's surface. We'll attempt
    ///  to re-parent those four Buttons to the newly created Panel.
    /// </summary>
    private void ReParentControls(Control newParent, List<Control> controls, string transactionName, IDesignerHost host)
    {
        using DesignerTransaction dt = host.CreateTransaction(transactionName);
        var changeService = GetService<IComponentChangeService>();

        PropertyDescriptor controlsProp = TypeDescriptor.GetProperties(newParent)["Controls"];
        PropertyDescriptor locationProp = TypeDescriptor.GetProperties(newParent)["Location"];

        // get the location of our parent - so we can correctly offset the new lasso'd controls
        // once they are re-parented
        Point parentLoc = Point.Empty;
        if (locationProp is not null)
        {
            parentLoc = (Point)locationProp.GetValue(newParent);
        }

        changeService?.OnComponentChanging(newParent, controlsProp);

        // enumerate the lasso'd controls relocate and re-parent...
        foreach (Control control in controls)
        {
            Control oldParent = control.Parent;
            Point controlLoc = Point.Empty;

            // do not want to reparent any control that is inherited readonly
            InheritanceAttribute inheritanceAttribute = (InheritanceAttribute)TypeDescriptor.GetAttributes(control)[typeof(InheritanceAttribute)];
            if (inheritanceAttribute is not null && inheritanceAttribute == InheritanceAttribute.InheritedReadOnly)
            {
                continue;
            }

            // get the current location of the control
            PropertyDescriptor locProp = TypeDescriptor.GetProperties(control)["Location"];
            if (locProp is not null)
            {
                controlLoc = (Point)locProp.GetValue(control);
            }

            // fire comp changing on parent and control
            if (oldParent is not null)
            {
                changeService?.OnComponentChanging(oldParent, controlsProp);

                // remove control from the old parent
                oldParent.Controls.Remove(control);
            }

            // finally add & relocate the control with the new parent
            newParent.Controls.Add(control);

            Point newLoc = Point.Empty;

            // this condition will determine which way we need to 'offset' our control location
            // based on whether we are moving controls into a child or bringing them out to
            // a parent
            if (oldParent is not null)
            {
                if (oldParent.Controls.Contains(newParent))
                {
                    newLoc = new Point(controlLoc.X - parentLoc.X, controlLoc.Y - parentLoc.Y);
                }
                else
                {
                    Point oldParentLoc = (Point)locProp.GetValue(oldParent);
                    newLoc = new Point(controlLoc.X + oldParentLoc.X, controlLoc.Y + oldParentLoc.Y);
                }
            }

            locProp.SetValue(control, newLoc);

            // fire our comp changed events
            if (changeService is not null && oldParent is not null)
            {
                changeService.OnComponentChanged(oldParent, controlsProp);
            }
        }

        changeService?.OnComponentChanged(newParent, controlsProp);

        // commit the transaction
        dt.Commit();
    }

    /// <summary>
    ///  Determines if the DrawGrid property should be persisted.
    /// </summary>
    private bool ShouldSerializeDrawGrid()
    {
        // To determine if we need to persist this value, we first need to check
        // if we have a parent who is a parentcontroldesigner, then get their
        // setting...
        ParentControlDesigner parent = GetParentControlDesignerOfParent();
        if (parent is not null)
        {
            return !(DrawGrid == parent.DrawGrid);
        }

        // Otherwise, we'll compare the value to the options page...
        return !IsOptionDefault("ShowGrid", DrawGrid);
    }

    /// <summary>
    ///  Determines if the SnapToGrid property should be persisted.
    /// </summary>
    private bool ShouldSerializeSnapToGrid()
    {
        // To determine if we need to persist this value, we first need to check
        // if we have a parent who is a parentcontroldesigner, then get their
        // setting...
        ParentControlDesigner parent = GetParentControlDesignerOfParent();
        if (parent is not null)
        {
            return !(SnapToGrid == parent.SnapToGrid);
        }

        // Otherwise, we'll compare the value to the options page...
        return !IsOptionDefault("SnapToGrid", SnapToGrid);
    }

    /// <summary>
    ///  Determines if the GridSize property should be persisted.
    /// </summary>
    private bool ShouldSerializeGridSize()
    {
        // To determine if we need to persist this value, we first need to check
        // if we have a parent who is a parentcontroldesigner, then get their
        // setting...
        ParentControlDesigner parent = GetParentControlDesignerOfParent();
        if (parent is not null)
        {
            return !(GridSize.Equals(parent.GridSize));
        }

        // Otherwise, we'll compare the value to the options page...
        return !IsOptionDefault("GridSize", GridSize);
    }

    private void ResetGridSize()
    {
        _getDefaultGridSize = true;
        _parentCanSetGridSize = true;
        // invalidate the control
        Control control = Control;
        control?.Invalidate(true);
    }

    private void ResetDrawGrid()
    {
        _getDefaultDrawGrid = true;
        _parentCanSetDrawGrid = true;
        // invalidate the control
        Control control = Control;
        control?.Invalidate(true);
    }

    private void ResetSnapToGrid()
    {
        _getDefaultGridSnap = true;
        _parentCanSetGridSnap = true;
    }

    /// <internalonly/>
    IComponent IOleDragClient.Component
    {
        get
        {
            return Component;
        }
    }

    /// <internalonly/>
    /// <summary>
    /// Retrieves the control view instance for the designer that
    /// is hosting the drag.
    /// </summary>
    bool IOleDragClient.AddComponent(IComponent component, string name, bool firstAdd)
    {
        IContainer container = DesignerUtils.CheckForNestedContainer(Component.Site.Container); // ...necessary to support SplitterPanel components

        bool containerMove = true;
        IContainer oldContainer = null;
        IDesignerHost localDesignerHost = (IDesignerHost)GetService(typeof(IDesignerHost));

        if (!firstAdd)
        {
            // just a move, so reparent
            if (component.Site is not null)
            {
                oldContainer = component.Site.Container;
                containerMove = container != oldContainer;
                if (containerMove)
                {
                    oldContainer.Remove(component);
                }
            }

            if (containerMove)
            {
                // check if there's already a component by this name in the
                // container
                if (name is not null && container.Components[name] is not null)
                {
                    name = null;
                }

                // add it back
                if (name is not null)
                {
                    container.Add(component, name);
                }
                else
                {
                    container.Add(component);
                }
            }
        }

        // make sure this designer will accept this component -- we wait until
        // now to be sure the components designer has been created.
        if (!((IOleDragClient)this).IsDropOk(component))
        {
            try
            {
                IUIService uiSvc = (IUIService)GetService(typeof(IUIService));
                string error = string.Format(SR.DesignerCantParentType, component.GetType().Name, Component.GetType().Name);
                if (uiSvc is not null)
                {
                    uiSvc.ShowError(error);
                }
                else
                {
                    RTLAwareMessageBox.Show(null, error, null, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, 0);
                }

                return false;
            }
            finally
            {
                if (containerMove)
                {
                    // move it back.
                    container.Remove(component);
                    oldContainer?.Add(component);
                }
                else
                {
                    // there wad no container move ... but then this operation is not supported so
                    // just remove this component
                    container.Remove(component);
                }
            }
        }

        // this is a chance to display a more specific error messages than on IsDropOK failure
        if (!CanAddComponent(component))
        {
            return false;
        }

        // make sure we can handle this thing, otherwise hand it to the base components designer
        Control c = GetControl(component);

        if (c is not null)
        {
            // set it's handler to this
            Control parent = GetParentForComponent(component);

            if (c is not Form form || !form.TopLevel)
            {
                if (c.Parent != parent)
                {
                    PropertyDescriptor controlsProp = TypeDescriptor.GetProperties(parent)["Controls"];
                    // we want to insert rather than add it, so we add then move
                    // to the beginning

                    if (c.Parent is not null)
                    {
                        Control cParent = c.Parent;
                        _changeService?.OnComponentChanging(cParent, controlsProp);

                        cParent.Controls.Remove(c);
                        _changeService?.OnComponentChanged(cParent, controlsProp, cParent.Controls, cParent.Controls);
                    }

                    if (_suspendChanging == 0 && _changeService is not null)
                    {
                        _changeService.OnComponentChanging(parent, controlsProp);
                    }

                    parent.Controls.Add(c);
                    // sburke 78059 -- not sure why we need this call. this should move things to the beginning of the
                    // z-order, but do we need that?
                    // parent.Controls.SetChildIndex(c, 0);
                    _changeService?.OnComponentChanged(parent, controlsProp, parent.Controls, parent.Controls);
                }
                else
                {
                    // here, we redo the add to make sure the handlers get setup right
                    int childIndex = parent.Controls.GetChildIndex(c);
                    parent.Controls.Remove(c);
                    parent.Controls.Add(c);
                    parent.Controls.SetChildIndex(c, childIndex);
                }
            }

            c.Invalidate(true);
        }

        if (localDesignerHost is not null && containerMove)
        {
            // sburke -- looks like we always want to do this to ensure that sited children get
            // handled properly. if we respected the boolean before, the ui selection handlers
            // would cache designers, handlers, etc. and cause problems.
            IComponentInitializer init = localDesignerHost.GetDesigner(component) as IComponentInitializer;
            init?.InitializeExistingComponent(null);

            AddChildComponents(component, container, localDesignerHost);
        }

        return true;
    }

    /// <internalonly/>
    /// <summary>
    /// Checks if the client is read only. That is, if components can
    /// be added or removed from the designer.
    /// </summary>
    bool IOleDragClient.CanModifyComponents
    {
        get
        {
            return (!InheritanceAttribute.Equals(InheritanceAttribute.InheritedReadOnly));
        }
    }

    /// <internalonly/>
    /// <summary>
    /// Checks if it is valid to drop this type of a component on this client.
    /// </summary>
    bool IOleDragClient.IsDropOk(IComponent component)
    {
        IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));

        if (host is not null)
        {
            IDesigner designer = host.GetDesigner(component);
            bool disposeDesigner = false;

            // we need to create one then
            if (designer is null)
            {
                designer = TypeDescriptor.CreateDesigner(component, typeof(IDesigner));
                ControlDesigner cd = designer as ControlDesigner;
                if (cd is not null)
                {
                    // Make sure the component doesn't get set to Visible
                    cd.ForceVisible = false;
                }

                designer.Initialize(component);
                disposeDesigner = true;
            }

            try
            {
                ComponentDesigner cd = designer as ComponentDesigner;
                if (cd is not null)
                {
                    if (cd.CanBeAssociatedWith(this))
                    {
                        ControlDesigner controlDesigner = cd as ControlDesigner;
                        if (controlDesigner is not null)
                        {
                            return CanParent(controlDesigner);
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            finally
            {
                if (disposeDesigner)
                {
                    designer.Dispose();
                }
            }
        }

        return true;
    }

    /// <internalonly/>
    /// <summary>
    /// Retrieves the control view instance for the designer that
    /// is hosting the drag.
    /// </summary>
    Control IOleDragClient.GetDesignerControl()
    {
        return Control;
    }

    /// <internalonly/>
    /// <summary>
    /// Retrieves the control view instance for the given component.
    /// For Win32 designer, this will often be the component itself.
    /// </summary>
    Control IOleDragClient.GetControlForComponent(object component)
    {
        return GetControl(component);
    }
}
