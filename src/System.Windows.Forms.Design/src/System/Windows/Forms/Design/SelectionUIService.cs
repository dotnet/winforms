// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using Microsoft.Win32;

namespace System.Windows.Forms.Design;

/// <summary>
///  The selection manager handles selection within a form. There is one selection manager for each form
///  or top level designer. A selection consists of an array of components. One component is designated the
///  "primary" selection and is displayed with different grab handles. An individual selection may or
///  may not have UI associated with it. If the selection manager can find a suitable designer that is representing
///  the selection, it will highlight the designer's border. If the merged property set has a location property,
///  the selection's rules will allow movement. Also, if the property set has a size property,
///  the selection's rules will allow for sizing. Grab handles may be drawn around the designer and
///  user interactions involving the selection frame and grab handles are initiated here, but
///  the actual movement of the objects is done in a designer object that implements the ISelectionHandler interface.
/// </summary>
internal sealed partial class SelectionUIService : Control, ISelectionUIService
{
    private static readonly Point s_invalidPoint = new(int.MinValue, int.MinValue);
    private const int HITTEST_CONTAINER_SELECTOR = 0x0001;
    private const int HITTEST_NORMAL_SELECTION = 0x0002;
    private const int HITTEST_DEFAULT = HITTEST_CONTAINER_SELECTOR | HITTEST_NORMAL_SELECTION;

    // These are used during a drag operation, either through our own handle drag or through ISelectionUIService
    private ISelectionUIHandler? _dragHandler; // the current drag handler
    private object[]? _dragComponents; // the controls being dragged
    private SelectionRules _dragRules; // movement constraints for the drag
    private bool _dragMoved;
    private object? _containerDrag; // object being dragged during a container drag
    // These are used during a drag of a selection grab handle
    private bool _ignoreCaptureChanged;
    private int _mouseDragHitTest; // where the hit occurred that caused the drag
    private Point _mouseDragAnchor = s_invalidPoint; // anchor point of the drag
    private Rectangle _mouseDragOffset = Rectangle.Empty; // current drag offset
    private Point _lastMoveScreenCoord = Point.Empty;
    private bool _ctrlSelect; // was the CTRL key down when the drag began
    private bool _mouseDragging; // Are we actually doing a drag?
    private ContainerSelectorActiveEventHandler? _containerSelectorActive; // the event we fire when user interacts with container selector
    private Dictionary<object, SelectionUIItem> _selectionItems;
    private readonly Dictionary<object, ISelectionUIHandler> _selectionHandlers; // Component UI handlers

    private bool _savedVisible; // we stash this when we mess with visibility ourselves.
    private bool _batchMode;
    private bool _batchChanged;
    private bool _batchSync;
    private readonly ISelectionService? _selSvc;
    private readonly IDesignerHost _host;
    private DesignerTransaction? _dragTransaction;

    /// <summary>
    ///  Creates a new selection manager object. The selection manager manages all selection of all designers
    ///  under the current form file.
    /// </summary>
    public SelectionUIService(IDesignerHost host) : base()
    {
        SetStyle(ControlStyles.StandardClick | ControlStyles.Opaque | ControlStyles.OptimizedDoubleBuffer, true);
        _host = host;
        _dragHandler = null;
        _dragComponents = null;
        _selectionItems = [];
        _selectionHandlers = [];
        AllowDrop = true;
        // Not really any reason for this, except that it can be handy when using Spy++
        Text = "SelectionUIOverlay";

        _selSvc = host.GetService<ISelectionService>();
        if (_selSvc is not null)
        {
            _selSvc.SelectionChanged += OnSelectionChanged;
        }

        // And configure the events we want to listen to.
        host.TransactionOpened += OnTransactionOpened;
        host.TransactionClosed += OnTransactionClosed;
        if (host.InTransaction)
        {
            OnTransactionOpened(host, EventArgs.Empty);
        }

        // Listen to the SystemEvents so that we can resync selection based on display settings etc.
        SystemEvents.DisplaySettingsChanged += OnSystemSettingChanged;
        SystemEvents.InstalledFontsChanged += OnSystemSettingChanged;
        SystemEvents.UserPreferenceChanged += OnUserPreferenceChanged;
    }

    /// <summary>
    ///  override of control.
    /// </summary>
    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams cp = base.CreateParams;
            cp.Style &= ~(int)(WINDOW_STYLE.WS_CLIPSIBLINGS | WINDOW_STYLE.WS_CLIPCHILDREN);
            return cp;
        }
    }

    /// <summary>
    ///  Called to initiate a mouse drag on the selection overlay. We cache some state here.
    /// </summary>
    private void BeginMouseDrag(Point anchor, int hitTest)
    {
        Capture = true;
        _ignoreCaptureChanged = false;
        _mouseDragAnchor = anchor;
        _mouseDragging = true;
        _mouseDragHitTest = hitTest;
        _mouseDragOffset = default;
        _savedVisible = Visible;
    }

    /// <summary>
    ///  Displays the given exception to the user.
    /// </summary>
    private void DisplayError(Exception e)
    {
        IUIService? uis = _host.GetService<IUIService>();
        if (uis is not null)
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

            RTLAwareMessageBox.Show(null, message, null, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, 0);
        }
    }

    /// <summary>
    ///  Disposes the entire selection UI manager.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (_selSvc is not null)
            {
                _selSvc.SelectionChanged -= OnSelectionChanged;
            }

            if (_host is not null)
            {
                _host.TransactionOpened -= OnTransactionOpened;
                _host.TransactionClosed -= OnTransactionClosed;
                if (_host.InTransaction)
                {
                    OnTransactionClosed(_host, new DesignerTransactionCloseEventArgs(true, true));
                }
            }

            foreach (SelectionUIItem s in _selectionItems.Values)
            {
                s.Dispose();
            }

            _selectionHandlers.Clear();
            _selectionItems.Clear();
            // Listen to the SystemEvents so that we can resync selection based on display settings etc.
            SystemEvents.DisplaySettingsChanged -= OnSystemSettingChanged;
            SystemEvents.InstalledFontsChanged -= OnSystemSettingChanged;
            SystemEvents.UserPreferenceChanged -= OnUserPreferenceChanged;
        }

        base.Dispose(disposing);
    }

    /// <summary>
    ///  Called when we want to finish a mouse drag and clean up our variables. We call this from multiple places,
    ///  depending on the state of the finish. This does NOT end the drag -- for that must call EndDrag.
    ///  This just cleans up the state of the mouse.
    /// </summary>
    private void EndMouseDrag(Point position)
    {
        // it's possible for us to be destroyed in a drag -- e.g. if this is the tray's SelectionUIService and
        // the last item is dragged out, so check disposed first.
        if (IsDisposed)
        {
            return;
        }

        _ignoreCaptureChanged = true;
        Capture = false;
        _mouseDragAnchor = s_invalidPoint;
        _mouseDragOffset = Rectangle.Empty;
        _mouseDragHitTest = 0;
        _dragMoved = false;
        SetSelectionCursor(position);
        _mouseDragging = _ctrlSelect = false;
    }

    /// <summary>
    ///  Determines the selection hit test at the given point. The point should be in screen coordinates.
    /// </summary>
    private HitTestInfo GetHitTest(Point value, int flags)
    {
        Point pt = PointToClient(value);
        foreach (SelectionUIItem item in _selectionItems.Values)
        {
            if ((flags & HITTEST_CONTAINER_SELECTOR) != 0)
            {
                if (item is ContainerSelectionUIItem && (item.GetRules() & SelectionRules.Visible) != SelectionRules.None)
                {
                    int hitTest = item.GetHitTest(pt);
                    if ((hitTest & SelectionUIItem.CONTAINER_SELECTOR) != 0)
                    {
                        return new HitTestInfo(hitTest, item, true);
                    }
                }
            }

            if ((flags & HITTEST_NORMAL_SELECTION) != 0)
            {
                if (item is not ContainerSelectionUIItem && (item.GetRules() & SelectionRules.Visible) != SelectionRules.None)
                {
                    int hitTest = item.GetHitTest(pt);
                    if (hitTest != SelectionUIItem.NOHIT)
                    {
                        if (hitTest != 0)
                        {
                            return new HitTestInfo(hitTest, item);
                        }
                        else
                        {
                            return new HitTestInfo(SelectionUIItem.NOHIT, item);
                        }
                    }
                }
            }
        }

        return new HitTestInfo(SelectionUIItem.NOHIT, null);
    }

    private ISelectionUIHandler? GetHandler(object component)
        => _selectionHandlers.TryGetValue(component, out ISelectionUIHandler? value) ? value : null;

    /// <summary>
    ///  This method returns a well-formed name for a drag transaction based on the rules it is given.
    /// </summary>
    public static string GetTransactionName(SelectionRules rules, object[] objects)
    {
        // Determine a nice name for the drag operation
        string transactionName;
        if ((rules & SelectionRules.Moveable) != 0)
        {
            if (objects.Length > 1)
            {
                transactionName = string.Format(SR.DragDropMoveComponents, objects.Length);
            }
            else
            {
                string? name = string.Empty;
                if (objects.Length > 0)
                {
                    if (objects[0] is IComponent { Site: { } site })
                    {
                        name = site.Name;
                    }
                    else
                    {
                        name = objects[0].GetType().Name;
                    }
                }

                transactionName = string.Format(SR.DragDropMoveComponent, name);
            }
        }
        else if ((rules & SelectionRules.AllSizeable) != 0)
        {
            if (objects.Length > 1)
            {
                transactionName = string.Format(SR.DragDropSizeComponents, objects.Length);
            }
            else
            {
                string? name = string.Empty;
                if (objects.Length > 0)
                {
                    if (objects[0] is IComponent { Site: { } site })
                    {
                        name = site.Name;
                    }
                    else
                    {
                        name = objects[0].GetType().Name;
                    }
                }

                transactionName = string.Format(SR.DragDropSizeComponent, name);
            }
        }
        else
        {
            transactionName = string.Format(SR.DragDropDragComponents, objects.Length);
        }

        return transactionName;
    }

    /// <summary>
    ///  Called by the designer host when it is entering or leaving a batch operation.
    ///  Here we queue up selection notification and we turn off our UI.
    /// </summary>
    private void OnTransactionClosed(object? sender, DesignerTransactionCloseEventArgs e)
    {
        if (e.LastTransaction)
        {
            _batchMode = false;
            if (_batchChanged)
            {
                _batchChanged = false;
                ((ISelectionUIService)this).SyncSelection();
            }

            if (_batchSync)
            {
                _batchSync = false;
                ((ISelectionUIService)this).SyncComponent(null);
            }
        }
    }

    /// <summary>
    ///  Called by the designer host when it is entering or leaving a batch operation.
    ///  Here we queue up selection notification and we turn off our UI.
    /// </summary>
    private void OnTransactionOpened(object? sender, EventArgs e)
    {
        _batchMode = true;
    }

    /// <summary>
    ///  update our window region on first create. We shouldn't do this before the handle is created or else we will force creation.
    /// </summary>
    protected override void OnHandleCreated(EventArgs e)
    {
        Debug.Assert(!RecreatingHandle, "Perf hit: we are recreating the docwin handle");
        base.OnHandleCreated(e);
        // Default the shape of the control to be empty, so that if nothing is initially selected that our window
        // surface doesn't interfere.
        UpdateWindowRegion();
    }

    /// <summary>
    ///  Called whenever a component changes. Here we update our selection information so that the
    ///  selection rectangles are all up to date.
    /// </summary>
    private void OnComponentChanged(object? sender, ComponentChangedEventArgs ccevent)
    {
        if (!_batchMode)
        {
            ((ISelectionUIService)this).SyncSelection();
        }
        else
        {
            _batchChanged = true;
        }
    }

    /// <summary>
    ///  called by the formcore when someone has removed a component.
    ///  This will remove any selection on the component without disturbing the rest of the selection.
    /// </summary>
    private void OnComponentRemove(object? sender, ComponentEventArgs ce)
    {
        _selectionHandlers.Remove(ce.Component!);
        _selectionItems.Remove(ce.Component!);
        ((ISelectionUIService)this).SyncComponent(ce.Component);
    }

    /// <summary>
    ///  Called to invoke the container active event, if a designer has bound to it.
    /// </summary>
    private void OnContainerSelectorActive(ContainerSelectorActiveEventArgs e)
    {
        _containerSelectorActive?.Invoke(this, e);
    }

    /// <summary>
    ///  Called when the selection changes. We sync up the UI with the selection at this point.
    /// </summary>
    private void OnSelectionChanged(object? sender, EventArgs e)
    {
        ICollection selection = _selSvc!.GetSelectedComponents();
        Dictionary<object, SelectionUIItem> newSelection = new(selection.Count);
        bool shapeChanged = false;
        foreach (object comp in selection)
        {
            bool create = true;
            if (_selectionItems.TryGetValue(comp, out SelectionUIItem? existingItem))
            {
                if (existingItem is ContainerSelectionUIItem item)
                {
                    item.Dispose();
                    shapeChanged = true;
                }
                else
                {
                    newSelection[comp] = existingItem;
                    create = false;
                }
            }

            if (create)
            {
                shapeChanged = true;
                newSelection[comp] = new SelectionUIItem(this, comp);
            }
        }

        if (!shapeChanged)
        {
            shapeChanged = _selectionItems.Keys.Count != newSelection.Keys.Count;
        }

        _selectionItems = newSelection;

        if (shapeChanged)
        {
            UpdateWindowRegion();
        }

        Invalidate();
        Update();
    }

    /// <summary>
    ///  User setting requires that we repaint.
    /// </summary>
    private void OnSystemSettingChanged(object? sender, EventArgs e) => Invalidate();

    /// <summary>
    ///  User setting requires that we repaint.
    /// </summary>
    private void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e) => Invalidate();

    /// <summary>
    ///  Inheriting classes should override this method to handle this event.
    ///  Call super.onDragEnter to send this event to any registered event listeners.
    /// </summary>
    protected override void OnDragEnter(DragEventArgs devent)
    {
        base.OnDragEnter(devent);
        _dragHandler?.OleDragEnter(devent);
    }

    /// <summary>
    ///  Inheriting classes should override this method to handle this event.
    ///  Call super.onDragOver to send this event to any registered event listeners.
    /// </summary>
    protected override void OnDragOver(DragEventArgs devent)
    {
        base.OnDragOver(devent);
        _dragHandler?.OleDragOver(devent);
    }

    /// <summary>
    ///  Inheriting classes should override this method to handle this event.
    ///  Call super.onDragLeave to send this event to any registered event listeners.
    /// </summary>
    protected override void OnDragLeave(EventArgs e)
    {
        base.OnDragLeave(e);
        _dragHandler?.OleDragLeave();
    }

    /// <summary>
    ///  Inheriting classes should override this method to handle this event.
    ///  Call super.onDragDrop to send this event to any registered event listeners.
    /// </summary>
    protected override void OnDragDrop(DragEventArgs devent)
    {
        base.OnDragDrop(devent);
        _dragHandler?.OleDragDrop(devent);
    }

    /// <summary>
    ///  Inheriting classes should override this method to handle this event.
    ///  Call base.OnDoubleClick to send this event to any registered event listeners.
    /// </summary>
    protected override void OnDoubleClick(EventArgs devent)
    {
        base.OnDoubleClick(devent);
        if (_selSvc is not null)
        {
            object? selComp = _selSvc.PrimarySelection;
            Debug.Assert(selComp is not null, "Illegal selection on double-click");
            if (selComp is not null)
            {
                ISelectionUIHandler? handler = GetHandler(selComp);
                handler?.OnSelectionDoubleClick((IComponent)selComp);
            }
        }
    }

    /// <summary>
    ///  Overrides Control to handle our selection grab handles.
    /// </summary>
    // Standard 'catch all - rethrow critical' exception pattern
    protected override void OnMouseDown(MouseEventArgs me)
    {
        if (_dragHandler is null && _selSvc is not null)
        {
            try
            {
                // First, did the user step on anything?
                Point anchor = PointToScreen(new Point(me.X, me.Y));
                HitTestInfo hti = GetHitTest(anchor, HITTEST_DEFAULT);
                int hitTest = hti.hitTest;
                if ((hitTest & SelectionUIItem.CONTAINER_SELECTOR) != 0)
                {
                    _selSvc.SetSelectedComponents(new object[] { hti.selectionUIHit!._component }, SelectionTypes.Auto);
                    // Then do a drag...
                    SelectionRules rules = SelectionRules.Moveable;
                    if (((ISelectionUIService)this).BeginDrag(rules, anchor.X, anchor.Y))
                    {
                        Visible = false;
                        _containerDrag = hti.selectionUIHit._component;
                        BeginMouseDrag(anchor, hitTest);
                    }
                }
                else if (hitTest != SelectionUIItem.NOHIT && me.Button == MouseButtons.Left)
                {
                    SelectionRules rules = SelectionRules.None;
                    // If the CTRL key isn't down, select this component, otherwise, we wait until the mouse up. Make sure the component is selected
                    _ctrlSelect = (ModifierKeys & Keys.Control) != Keys.None;
                    if (!_ctrlSelect)
                    {
                        _selSvc.SetSelectedComponents(new object[] { hti.selectionUIHit!._component }, SelectionTypes.Primary);
                    }

                    if ((hitTest & SelectionUIItem.MOVE_MASK) != 0)
                    {
                        rules |= SelectionRules.Moveable;
                    }

                    if ((hitTest & SelectionUIItem.SIZE_MASK) != 0)
                    {
                        if ((hitTest & (SelectionUIItem.SIZE_X | SelectionUIItem.POS_RIGHT)) == (SelectionUIItem.SIZE_X | SelectionUIItem.POS_RIGHT))
                        {
                            rules |= SelectionRules.RightSizeable;
                        }

                        if ((hitTest & (SelectionUIItem.SIZE_X | SelectionUIItem.POS_LEFT)) == (SelectionUIItem.SIZE_X | SelectionUIItem.POS_LEFT))
                        {
                            rules |= SelectionRules.LeftSizeable;
                        }

                        if ((hitTest & (SelectionUIItem.SIZE_Y | SelectionUIItem.POS_TOP)) == (SelectionUIItem.SIZE_Y | SelectionUIItem.POS_TOP))
                        {
                            rules |= SelectionRules.TopSizeable;
                        }

                        if ((hitTest & (SelectionUIItem.SIZE_Y | SelectionUIItem.POS_BOTTOM)) == (SelectionUIItem.SIZE_Y | SelectionUIItem.POS_BOTTOM))
                        {
                            rules |= SelectionRules.BottomSizeable;
                        }

                        if (((ISelectionUIService)this).BeginDrag(rules, anchor.X, anchor.Y))
                        {
                            BeginMouseDrag(anchor, hitTest);
                        }
                    }
                    else
                    {
                        // Our mouse is in drag mode. We defer the actual move until the user moves the mouse.
                        _dragRules = rules;
                        BeginMouseDrag(anchor, hitTest);
                    }
                }
                else if (hitTest == SelectionUIItem.NOHIT)
                {
                    _dragRules = SelectionRules.None;
                    _mouseDragAnchor = s_invalidPoint;
                    return;
                }
            }
            catch (Exception e) when (!e.IsCriticalException())
            {
                if (e != CheckoutException.Canceled)
                {
                    DisplayError(e);
                }
            }
        }
    }

    /// <summary>
    ///  Overrides Control to handle our selection grab handles.
    /// </summary>
    protected override void OnMouseMove(MouseEventArgs me)
    {
        base.OnMouseMove(me);
        Point screenCoord = PointToScreen(new Point(me.X, me.Y));
        HitTestInfo hti = GetHitTest(screenCoord, HITTEST_CONTAINER_SELECTOR);
        int hitTest = hti.hitTest;
        if (hitTest != SelectionUIItem.CONTAINER_SELECTOR && hti.selectionUIHit is not null)
        {
            OnContainerSelectorActive(new ContainerSelectorActiveEventArgs());
        }

        if (_lastMoveScreenCoord == screenCoord)
        {
            return;
        }

        // If we're not dragging then set the cursor correctly.
        if (!_mouseDragging)
        {
            SetSelectionCursor(screenCoord);
        }
        else
        {
            // we have to make sure the mouse moved farther than the minimum drag distance before we actually start the drag
            if (!((ISelectionUIService)this).Dragging && (_mouseDragHitTest & SelectionUIItem.MOVE_MASK) != 0)
            {
                Size minDragSize = SystemInformation.DragSize;
                if (
                   Math.Abs(screenCoord.X - _mouseDragAnchor.X) < minDragSize.Width &&
                   Math.Abs(screenCoord.Y - _mouseDragAnchor.Y) < minDragSize.Height)
                {
                    return;
                }
                else
                {
                    _ignoreCaptureChanged = true;
                    if (((ISelectionUIService)this).BeginDrag(_dragRules, _mouseDragAnchor.X, _mouseDragAnchor.Y))
                    {
                        // we're moving, so we don't care about the ctrl key any more
                        _ctrlSelect = false;
                    }
                    else
                    {
                        EndMouseDrag(MousePosition);
                        return;
                    }
                }
            }

            Rectangle old = _mouseDragOffset;
            if ((_mouseDragHitTest & SelectionUIItem.MOVE_X) != 0)
            {
                _mouseDragOffset.X = screenCoord.X - _mouseDragAnchor.X;
            }

            if ((_mouseDragHitTest & SelectionUIItem.MOVE_Y) != 0)
            {
                _mouseDragOffset.Y = screenCoord.Y - _mouseDragAnchor.Y;
            }

            if ((_mouseDragHitTest & SelectionUIItem.SIZE_X) != 0)
            {
                if ((_mouseDragHitTest & SelectionUIItem.POS_LEFT) != 0)
                {
                    _mouseDragOffset.X = screenCoord.X - _mouseDragAnchor.X;
                    _mouseDragOffset.Width = _mouseDragAnchor.X - screenCoord.X;
                }
                else
                {
                    _mouseDragOffset.Width = screenCoord.X - _mouseDragAnchor.X;
                }
            }

            if ((_mouseDragHitTest & SelectionUIItem.SIZE_Y) != 0)
            {
                if ((_mouseDragHitTest & SelectionUIItem.POS_TOP) != 0)
                {
                    _mouseDragOffset.Y = screenCoord.Y - _mouseDragAnchor.Y;
                    _mouseDragOffset.Height = _mouseDragAnchor.Y - screenCoord.Y;
                }
                else
                {
                    _mouseDragOffset.Height = screenCoord.Y - _mouseDragAnchor.Y;
                }
            }

            if (!old.Equals(_mouseDragOffset))
            {
                Rectangle delta = _mouseDragOffset;
                delta.X -= old.X;
                delta.Y -= old.Y;
                delta.Width -= old.Width;
                delta.Height -= old.Height;
                if (delta.X != 0 || delta.Y != 0 || delta.Width != 0 || delta.Height != 0)
                {
                    // Go to default cursor for moves...
                    if ((_mouseDragHitTest & SelectionUIItem.MOVE_X) != 0 || (_mouseDragHitTest & SelectionUIItem.MOVE_Y) != 0)
                    {
                        Cursor = Cursors.Default;
                    }

                    ((ISelectionUIService)this).DragMoved(delta);
                }
            }
        }
    }

    /// <summary>
    ///  Overrides Control to handle our selection grab handles.
    /// </summary>
    // Standard 'catch all - rethrow critical' exception pattern
    protected override void OnMouseUp(MouseEventArgs me)
    {
        try
        {
            Point screenCoord = PointToScreen(new Point(me.X, me.Y));
            if (_ctrlSelect && !_mouseDragging && _selSvc is not null)
            {
                HitTestInfo hti = GetHitTest(screenCoord, HITTEST_DEFAULT);
                _selSvc.SetSelectedComponents(new object[] { hti.selectionUIHit!._component }, SelectionTypes.Primary);
            }

            if (_mouseDragging)
            {
                object? oldContainerDrag = _containerDrag;
                bool oldDragMoved = _dragMoved;
                EndMouseDrag(screenCoord);
                if (((ISelectionUIService)this).Dragging)
                {
                    ((ISelectionUIService)this).EndDrag(false);
                }

                if (me.Button == MouseButtons.Right && oldContainerDrag is not null && !oldDragMoved)
                {
                    OnContainerSelectorActive(new ContainerSelectorActiveEventArgs());
                }
            }
        }
        catch (Exception e) when (!e.IsCriticalException())
        {
            if (e != CheckoutException.Canceled)
            {
                DisplayError(e);
            }
        }
    }

    /// <summary>
    ///  If the selection manager move, this indicates that the form has autoscrolling enabled and has been scrolled.
    ///  We have to invalidate here because we may get moved before the rest of the components so we may draw the
    ///  selection in the wrong spot.
    /// </summary>
    protected override void OnMove(EventArgs e)
    {
        base.OnMove(e);
        Invalidate();
    }

    /// <summary>
    ///  overrides control.onPaint. here we paint the selection handles. The window's region was setup earlier.
    /// </summary>
    protected override void OnPaint(PaintEventArgs e)
    {
        // Paint the regular selection items first, and then the container selectors last so they draw over the top.
        foreach (SelectionUIItem item in _selectionItems.Values)
        {
            if (item is ContainerSelectionUIItem)
            {
                continue;
            }

            item.DoPaint(e.Graphics);
        }

        foreach (SelectionUIItem item in _selectionItems.Values)
        {
            if (item is ContainerSelectionUIItem)
            {
                item.DoPaint(e.Graphics);
            }
        }
    }

    /// <summary>
    ///  Sets the appropriate selection cursor at the given point.
    /// </summary>
    private void SetSelectionCursor(Point pt)
    {
        Point clientCoords = PointToClient(pt);
        // We render the cursor in the same order we paint.
        foreach (SelectionUIItem item in _selectionItems.Values)
        {
            if (item is ContainerSelectionUIItem)
            {
                continue;
            }

            Cursor? cursor = item.GetCursorAtPoint(clientCoords);
            if (cursor is not null)
            {
                Cursor = cursor == Cursors.Default ? null : cursor;
                return;
            }
        }

        foreach (SelectionUIItem item in _selectionItems.Values)
        {
            if (item is ContainerSelectionUIItem)
            {
                Cursor? cursor = item.GetCursorAtPoint(clientCoords);
                if (cursor is not null)
                {
                    Cursor = cursor == Cursors.Default ? null : cursor;
                    return;
                }
            }
        }

        // Don't know what to set; just use the default.
        Cursor = null;
    }

    /// <summary>
    ///  called when the overlay region is invalid and should be updated
    /// </summary>
    private void UpdateWindowRegion()
    {
        Region region = new(new Rectangle(0, 0, 0, 0));
        foreach (SelectionUIItem item in _selectionItems.Values)
        {
            region.Union(item.GetRegion());
        }

        Region = region;
    }

    /// <summary>
    ///  Override of our control's WNDPROC. We diddle with capture a bit, and it's important to turn this off if the capture changes.
    /// </summary>
    protected override void WndProc(ref Message m)
    {
        switch (m.MsgInternal)
        {
            case PInvokeCore.WM_LBUTTONUP:
            case PInvokeCore.WM_RBUTTONUP:
                if (_mouseDragAnchor != s_invalidPoint)
                {
                    _ignoreCaptureChanged = true;
                }

                break;
            case PInvokeCore.WM_CAPTURECHANGED:
                if (!_ignoreCaptureChanged && _mouseDragAnchor != s_invalidPoint)
                {
                    EndMouseDrag(MousePosition);
                    if (((ISelectionUIService)this).Dragging)
                    {
                        ((ISelectionUIService)this).EndDrag(true);
                    }
                }

                _ignoreCaptureChanged = false;
                break;
        }

        base.WndProc(ref m);
    }

    /// <summary>
    ///  This can be used to determine if the user is in the middle of a drag operation.
    /// </summary>
    bool ISelectionUIService.Dragging
    {
        get => _dragHandler is not null;
    }

    /// <summary>
    ///  Determines if the selection UI is shown or not.
    /// </summary>
    bool ISelectionUIService.Visible
    {
        get => Visible;
        set => Visible = value;
    }

    /// <summary>
    ///  Adds an event handler to the ContainerSelectorActive event.
    ///  This event is fired whenever the user interacts with the container selector in a manor that
    ///  would indicate that the selector should continued to be displayed.
    ///  Since the container selector normally will vanish after a timeout,
    ///  designers should listen to this event and reset the timeout when this event occurs.
    /// </summary>
    event ContainerSelectorActiveEventHandler? ISelectionUIService.ContainerSelectorActive
    {
        add => _containerSelectorActive += value;
        remove => _containerSelectorActive -= value;
    }

    /// <summary>
    ///  Assigns a selection UI handler to a given component.
    ///  The handler will be called when the UI service needs information about the component.
    ///  A single selection UI handler can be assigned to multiple components.
    ///  When multiple components are dragged, only a single handler may control the drag.
    ///  Because of this, only components that are assigned the same handler as the primary selection
    ///  are included in drag operations. A selection UI handler is automatically unassigned when the component
    ///  is removed from the container or disposed.
    /// </summary>
    void ISelectionUIService.AssignSelectionUIHandler(object component, ISelectionUIHandler handler)
    {
        if (_selectionHandlers.TryGetValue(component, out ISelectionUIHandler? oldHandler))
        {
            // The collection editors do not dispose objects from the collection before setting a new collection.
            // This causes items that are common to the old and new collections to come through this code path again,
            // causing the exception to fire. So, we check to see if the SelectionUIHandler is same,
            // and bail out in that case.
            if (handler == oldHandler)
            {
                return;
            }

            Debug.Fail("A component may have only one selection UI handler.");
            throw new InvalidOperationException();
        }

        _selectionHandlers[component] = handler;
        // If this component is selected, create a new UI handler for it.
        if (_selSvc is not null && _selSvc.GetComponentSelected(component))
        {
            SelectionUIItem item = new(this, component);
            _selectionItems[component] = item;
            UpdateWindowRegion();
            item.Invalidate();
        }
    }

    void ISelectionUIService.ClearSelectionUIHandler(object component, ISelectionUIHandler handler)
    {
        _selectionHandlers.TryGetValue(component, out ISelectionUIHandler? oldHandler);
        if (oldHandler == handler)
        {
            _selectionHandlers.Remove(component);
        }
    }

    /// <summary>
    ///  This can be called by an outside party to begin a drag of the currently selected set of components.
    /// </summary>
    // Standard 'catch all - rethrow critical' exception pattern
    bool ISelectionUIService.BeginDrag(SelectionRules rules, int initialX, int initialY)
    {
        if (_dragHandler is not null)
        {
            Debug.Fail("Caller is starting a drag, but there is already one in progress -- we cannot nest these!");
            return false;
        }

        if (rules == SelectionRules.None)
        {
            Debug.Fail("Caller is starting requesting a drag with no drag rules.");
            return false;
        }

        if (_selSvc is null)
        {
            return false;
        }

        _savedVisible = Visible;
        // First, get the list of controls
        ICollection col = _selSvc.GetSelectedComponents();
        object[] objects = new object[col.Count];
        col.CopyTo(objects, 0);
        objects = ((ISelectionUIService)this).FilterSelection(objects, rules);
        if (objects.Length == 0)
        {
            return false; // nothing selected
        }

        // We allow all components with the same UI handler as the primary selection to participate in the drag.
        ISelectionUIHandler? primaryHandler = null;
        object? primary = _selSvc.PrimarySelection;
        if (primary is not null)
        {
            primaryHandler = GetHandler(primary);
        }

        if (primaryHandler is null)
        {
            return false; // no UI handler for selection
        }

        // Now within the given selection, add those items that have the same UI handler and that have the proper rule constraints.
        List<object> list = [];
        for (int i = 0; i < objects.Length; i++)
        {
            if (GetHandler(objects[i]) == primaryHandler)
            {
                SelectionRules compRules = primaryHandler.GetComponentRules(objects[i]);
                if ((compRules & rules) == rules)
                {
                    list.Add(objects[i]);
                }
            }
        }

        if (list.Count == 0)
        {
            return false; // nothing matching the given constraints
        }

        objects = [.. list];
        bool dragging = false;
        // We must setup state before calling QueryBeginDrag. It is possible that QueryBeginDrag will cancel a drag
        // (if it places a modal dialog, for example), so we must have the drag data all setup before it cancels.
        // Then, we will check again after QueryBeginDrag to see if a cancel happened.
        _dragComponents = objects;
        _dragRules = rules;
        _dragHandler = primaryHandler;
        string transactionName = GetTransactionName(rules, objects);
        _dragTransaction = _host.CreateTransaction(transactionName);
        try
        {
            if (primaryHandler.QueryBeginDrag(objects, rules, initialX, initialY))
            {
                if (_dragHandler is not null)
                {
                    try
                    {
                        dragging = primaryHandler.BeginDrag(objects, rules, initialX, initialY);
                    }
                    catch (Exception e)
                    {
                        Debug.Fail("Drag handler threw during BeginDrag -- bad handler!", e.ToString());
                        dragging = false;
                    }
                }
            }
        }
        finally
        {
            if (!dragging)
            {
                _dragComponents = null;
                _dragRules = 0;
                _dragHandler = null;

                // Always commit this -- BeginDrag returns false for our drags because it is a complete operation.
                if (_dragTransaction is not null)
                {
                    _dragTransaction.Commit();
                    _dragTransaction = null;
                }
            }
        }

        return dragging;
    }

    /// <summary>
    ///  Called by an outside party to update drag information. This can only be called after a successful call to beginDrag.
    /// </summary>
    void ISelectionUIService.DragMoved(Rectangle offset)
    {
        Rectangle newOffset = Rectangle.Empty;
        if (_dragHandler is null)
        {
            throw new InvalidOperationException(SR.DesignerBeginDragNotCalled);
        }

        Debug.Assert(_dragComponents is not null, "We should have a set of drag controls here");
        if ((_dragRules & SelectionRules.Moveable) == SelectionRules.None && (_dragRules & (SelectionRules.TopSizeable | SelectionRules.LeftSizeable)) == SelectionRules.None)
        {
            newOffset = offset with { X = 0, Y = 0 };
        }

        if ((_dragRules & SelectionRules.AllSizeable) == SelectionRules.None)
        {
            if (newOffset.IsEmpty)
            {
                newOffset = offset with { Width = 0, Height = 0 };
            }
            else
            {
                newOffset.Width = newOffset.Height = 0;
            }
        }

        if (!newOffset.IsEmpty)
        {
            offset = newOffset;
        }

        Visible = false;
        _dragMoved = true;
        _dragHandler.DragMoved(_dragComponents, offset);
    }

    /// <summary>
    ///  Called by an outside party to finish a drag operation. This can only be called after a successful call to beginDrag.
    /// </summary>
    // Standard 'catch all - rethrow critical' exception pattern
    void ISelectionUIService.EndDrag(bool cancel)
    {
        _containerDrag = null;
        ISelectionUIHandler? handler = _dragHandler;
        object[] components = _dragComponents!;
        // Clean these out so that even if we throw an exception we don't die.
        _dragHandler = null;
        _dragComponents = null;
        _dragRules = SelectionRules.None;
        if (handler is null)
        {
            throw new InvalidOperationException();
        }

        // Typically, the handler will be changing a bunch of component properties here.
        // Optimize this by enclosing it within a batch call.
        DesignerTransaction? trans = null;
        try
        {
            IComponent? comp = components[0] as IComponent;
            if (components.Length > 1 || (components.Length == 1 && comp is not null && comp.Site is null))
            {
                trans = _host.CreateTransaction(string.Format(SR.DragDropMoveComponents, components.Length));
            }
            else if (components.Length == 1)
            {
                if (comp is not null)
                {
                    trans = _host.CreateTransaction(string.Format(SR.DragDropMoveComponent, comp.Site!.Name));
                }
            }

            try
            {
                handler.EndDrag(components, cancel);
            }
            catch (Exception e)
            {
                Debug.Fail(e.ToString());
            }
        }
        finally
        {
            trans?.Commit();

            // Reset the selection. This will re-display our selection.
            Visible = _savedVisible;
            ((ISelectionUIService)this).SyncSelection();
            if (_dragTransaction is not null)
            {
                _dragTransaction.Commit();
                _dragTransaction = null;
            }

            // In case this drag was initiated by us, ensure that our mouse state is correct
            EndMouseDrag(MousePosition);
        }
    }

    /// <summary>
    ///  Filters the set of selected components. The selection service will retrieve all components that are currently selected.
    ///  This method allows you to filter this set down to components that match your criteria.
    ///  The selectionRules parameter must contain one or more flags from the SelectionRules class.
    ///  These flags allow you to constrain the set of selected objects to visible, movable, sizeable or all objects.
    /// </summary>
    object[] ISelectionUIService.FilterSelection(object[] components, SelectionRules selectionRules)
    {
        object[]? selection = null;
        if (components is null)
        {
            return [];
        }

        // Mask off any selection object that doesn't adhere to the given ruleset.
        // We can ignore this if the ruleset is zero, as all components would be accepted.
        if (selectionRules != SelectionRules.None)
        {
            List<object> list = [];
            foreach (object comp in components)
            {
                if (_selectionItems.TryGetValue(comp, out SelectionUIItem? item) && item is not ContainerSelectionUIItem)
                {
                    if ((item.GetRules() & selectionRules) == selectionRules)
                    {
                        list.Add(comp);
                    }
                }
            }

            selection = [.. list];
        }

        return selection ?? [];
    }

    /// <summary>
    ///  Retrieves the width and height of a selection border grab handle.
    ///  Designers may need this to properly position their user interfaces.
    /// </summary>
    Size ISelectionUIService.GetAdornmentDimensions(AdornmentType adornmentType) => adornmentType switch
    {
        AdornmentType.GrabHandle => new Size(SelectionUIItem.GRABHANDLE_WIDTH, SelectionUIItem.GRABHANDLE_HEIGHT),
        AdornmentType.ContainerSelector or AdornmentType.Maximum => new Size(ContainerSelectionUIItem.CONTAINER_WIDTH, ContainerSelectionUIItem.CONTAINER_HEIGHT),
        _ => new Size(0, 0),
    };

    /// <summary>
    ///  Tests to determine if the given screen coordinate is over an adornment for the specified component.
    ///  This will only return true if the adornment, and selection UI, is visible.
    /// </summary>
    bool ISelectionUIService.GetAdornmentHitTest(object component, Point value) =>
        GetHitTest(value, HITTEST_DEFAULT).hitTest != SelectionUIItem.NOHIT;

    /// <summary>
    ///  Determines if the component is currently "container" selected. Container selection is a visual aid for
    ///  selecting containers. It doesn't affect the normal "component" selection.
    /// </summary>
    bool ISelectionUIService.GetContainerSelected([NotNullWhen(true)] object? component)
        => (component is not null
        && _selectionItems.TryGetValue(component, out SelectionUIItem? value)
        && value is ContainerSelectionUIItem);

    /// <summary>
    ///  Retrieves a set of flags that define rules for the selection.
    ///  Selection rules indicate if the given component can be moved or sized, for example.
    /// </summary>
    SelectionRules ISelectionUIService.GetSelectionRules(object component)
    {
        if (!_selectionItems.TryGetValue(component, out SelectionUIItem? selection))
        {
            Debug.Fail("The component is not currently selected.");
            throw new InvalidOperationException();
        }

        return selection.GetRules();
    }

    /// <summary>
    ///  Allows you to configure the style of the selection frame that a component uses.
    ///  This is useful if your component supports different modes of operation
    ///  (such as an in-place editing mode and a static design mode).
    ///  Where possible, you should leave the selection style as is and use the design-time hit testing
    ///  feature of the IDesigner interface to provide features at design time.
    ///  The value of style must be one of the SelectionStyle enum values.
    ///  The selection style is only valid for the duration that the component is selected.
    /// </summary>
    SelectionStyles ISelectionUIService.GetSelectionStyle(object component)
        => !_selectionItems.TryGetValue(component, out SelectionUIItem? item) ? SelectionStyles.None : item.Style;

    /// <summary>
    ///  Changes the container selection status of the given component.
    ///  Container selection is a visual aid for selecting containers.
    ///  It doesn't affect the normal "component" selection.
    /// </summary>
    void ISelectionUIService.SetContainerSelected(object component, bool selected)
    {
        if (selected)
        {
            _selectionItems.TryGetValue(component, out SelectionUIItem? existingItem);
            if (existingItem is not ContainerSelectionUIItem)
            {
                existingItem?.Dispose();

                SelectionUIItem item = new ContainerSelectionUIItem(this, component);
                _selectionItems[component] = item;
                // Now update our region and invalidate
                UpdateWindowRegion();
                existingItem?.Invalidate();

                item.Invalidate();
            }
        }
        else
        {
            if (!_selectionItems.TryGetValue(component, out SelectionUIItem? existingItem) || existingItem is ContainerSelectionUIItem)
            {
                _selectionItems.Remove(component);
                existingItem?.Dispose();

                UpdateWindowRegion();
                existingItem?.Invalidate();
            }
        }
    }

    /// <summary>
    ///  Allows you to configure the style of the selection frame that a component uses.
    ///  This is useful if your component supports different modes of operation
    ///  (such as an in-place editing mode and a static design mode). Where possible,
    ///  you should leave the selection style as is and use the design-time hit testing feature of the
    ///  <see cref="IDesigner"/> interface to provide features at design time.
    ///  The value of style must be one of the SelectionStyle enum values.
    ///  The selection style is only valid for the duration that the component is selected.
    /// </summary>
    void ISelectionUIService.SetSelectionStyle(object component, SelectionStyles style)
    {
        _selectionItems.TryGetValue(component, out SelectionUIItem? selUI);
        if (_selSvc is not null && _selSvc.GetComponentSelected(component))
        {
            selUI = new SelectionUIItem(this, component);
            _selectionItems[component] = selUI;
        }

        if (selUI is not null)
        {
            selUI.Style = style;
            UpdateWindowRegion();
            selUI.Invalidate();
        }
    }

    /// <summary>
    ///  This should be called when a component has been moved, sized or re-parented, but the change was not the result
    ///  of a property change. All property changes are monitored by the selection UI service, so this is automatic
    ///  most of the time. There are times, however, when a component may be moved without a property change
    ///  notification occurring. Scrolling an auto scroll Win32 form is an example of this.
    ///  This method simply re-queries all currently selected components for their bounds and
    ///  updates the selection handles for any that have changed.
    /// </summary>
    void ISelectionUIService.SyncSelection()
    {
        if (_batchMode)
        {
            _batchChanged = true;
        }
        else
        {
            if (IsHandleCreated)
            {
                bool updateRegion = false;
                foreach (SelectionUIItem item in _selectionItems.Values)
                {
                    updateRegion |= item.UpdateSize();
                    item.UpdateRules();
                }

                if (updateRegion)
                {
                    UpdateWindowRegion();
                    Update();
                }
            }
        }
    }

    /// <summary>
    ///  This should be called when a component's property changed, that the designer thinks should result
    ///  in a selection UI change. This method simply re-queries all currently selected components for their bounds
    ///  and updates the selection handles for any that have changed.
    /// </summary>
    void ISelectionUIService.SyncComponent(object? component)
    {
        if (_batchMode)
        {
            _batchSync = true;
        }
        else
        {
            if (IsHandleCreated)
            {
                foreach (SelectionUIItem item in _selectionItems.Values)
                {
                    item.UpdateRules();
                    item.Dispose();
                }

                UpdateWindowRegion();
                Invalidate();
                Update();
            }
        }
    }
}
