// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using Microsoft.Win32;
using static Interop;

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  The selection manager handles selection within a form.  There is one selection manager for each form or top level designer. A selection consists of an array of components.  One component is designated the "primary" selection and is displayed with different grab handles. An individual selection may or may not have UI associated with it.  If the selection manager can find a suitable designer that is representing the selection, it will highlight the designer's border.  If the merged property set has a location property, the selection's rules will allow movement.  Also, if the property set has a size property, the selection's rules will allow for sizing.  Grab handles may be drawn around the designer and user interactions involving the selection frame and grab handles are initiated here, but the actual movement of the objects is done in a designer object that implements the ISelectionHandler interface.
    /// </summary>
    internal sealed class SelectionUIService : Control, ISelectionUIService
    {
        private static readonly Point s_invalidPoint = new Point(int.MinValue, int.MinValue);
        private const int HITTEST_CONTAINER_SELECTOR = 0x0001;
        private const int HITTEST_NORMAL_SELECTION = 0x0002;
        private const int HITTEST_DEFAULT = HITTEST_CONTAINER_SELECTOR | HITTEST_NORMAL_SELECTION;

        // These are used during a drag operation, either through our own handle drag or through ISelectionUIService
        private ISelectionUIHandler _dragHandler; // the current drag handler
        private object[] _dragComponents; // the controls being dragged
        private SelectionRules _dragRules; // movement constraints for the drag
        private bool _dragMoved;
        private object _containerDrag; // object being dragged during a container drag
        // These are used during a drag of a selection grab handle
        private bool _ignoreCaptureChanged;
        private int _mouseDragHitTest; // where the hit occurred that caused the drag
        private Point _mouseDragAnchor = s_invalidPoint; // anchor point of the drag
        private Rectangle _mouseDragOffset = Rectangle.Empty; // current drag offset
        private Point _lastMoveScreenCoord = Point.Empty;
        private bool _ctrlSelect; // was the CTRL key down when the drag began
        private bool _mouseDragging; // Are we actually doing a drag?
        private ContainerSelectorActiveEventHandler _containerSelectorActive; // the event we fire when user interacts with container selector
        private Hashtable _selectionItems;
        private readonly Hashtable _selectionHandlers; // Component UI handlers

        private bool _savedVisible; // we stash this when we mess with visibility ourselves.
        private bool _batchMode;
        private bool _batchChanged;
        private bool _batchSync;
        private readonly ISelectionService _selSvc;
        private readonly IDesignerHost _host;
        private DesignerTransaction _dragTransaction;

        /// <summary>
        ///  Creates a new selection manager object.  The selection manager manages all selection of all designers under the current form file.
        /// </summary>
        public SelectionUIService(IDesignerHost host) : base()
        {
            SetStyle(ControlStyles.StandardClick | ControlStyles.Opaque | ControlStyles.OptimizedDoubleBuffer, true);
            _host = host;
            _dragHandler = null;
            _dragComponents = null;
            _selectionItems = new Hashtable();
            _selectionHandlers = new Hashtable();
            AllowDrop = true;
            // Not really any reason for this, except that it can be handy when using Spy++
            Text = "SelectionUIOverlay";

            _selSvc = (ISelectionService)host.GetService(typeof(ISelectionService));
            if (_selSvc != null)
            {
                _selSvc.SelectionChanged += new EventHandler(OnSelectionChanged);
            }

            // And configure the events we want to listen to.
            host.TransactionOpened += new EventHandler(OnTransactionOpened);
            host.TransactionClosed += new DesignerTransactionCloseEventHandler(OnTransactionClosed);
            if (host.InTransaction)
            {
                OnTransactionOpened(host, EventArgs.Empty);
            }

            // Listen to the SystemEvents so that we can resync selection based on display settings etc.
            SystemEvents.DisplaySettingsChanged += new EventHandler(OnSystemSettingChanged);
            SystemEvents.InstalledFontsChanged += new EventHandler(OnSystemSettingChanged);
            SystemEvents.UserPreferenceChanged += new UserPreferenceChangedEventHandler(OnUserPreferenceChanged);
        }

        /// <summary>
        ///  override of control.
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style &= ~(int)(User32.WS.CLIPSIBLINGS | User32.WS.CLIPCHILDREN);
                return cp;
            }
        }

        /// <summary>
        ///  Called to initiate a mouse drag on the selection overlay.  We cache some state here.
        /// </summary>
        private void BeginMouseDrag(Point anchor, int hitTest)
        {
            Capture = true;
            _ignoreCaptureChanged = false;
            _mouseDragAnchor = anchor;
            _mouseDragging = true;
            _mouseDragHitTest = hitTest;
            _mouseDragOffset = new Rectangle();
            _savedVisible = Visible;
        }

        /// <summary>
        ///  Displays the given exception to the user.
        /// </summary>
        private void DisplayError(Exception e)
        {
            IUIService uis = (IUIService)_host.GetService(typeof(IUIService));
            if (uis != null)
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
        ///  Disposes the entire selection UI manager.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_selSvc != null)
                {
                    _selSvc.SelectionChanged -= new EventHandler(OnSelectionChanged);
                }

                if (_host != null)
                {
                    _host.TransactionOpened -= new EventHandler(OnTransactionOpened);
                    _host.TransactionClosed -= new DesignerTransactionCloseEventHandler(OnTransactionClosed);
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
                SystemEvents.DisplaySettingsChanged -= new EventHandler(OnSystemSettingChanged);
                SystemEvents.InstalledFontsChanged -= new EventHandler(OnSystemSettingChanged);
                SystemEvents.UserPreferenceChanged -= new UserPreferenceChangedEventHandler(OnUserPreferenceChanged);
            }
            base.Dispose(disposing);
        }

        /// <summary>
        ///  Called when we want to finish a mouse drag and clean up our variables.  We call this from multiple places, depending on the state of the finish.  This does NOT end the drag -- for that must call EndDrag. This just cleans up the state of the mouse.
        /// </summary>
        private void EndMouseDrag(Point position)
        {
            // it's possible for us to be destroyed in a drag -- e.g. if this is the tray's selectionuiservice and the last item is dragged out, so check diposed first
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
        ///  Determines the selection hit test at the given point.  The point should be in screen coordinates.
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
                    if (!(item is ContainerSelectionUIItem) && (item.GetRules() & SelectionRules.Visible) != SelectionRules.None)
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

        private ISelectionUIHandler GetHandler(object component) => (ISelectionUIHandler)_selectionHandlers[component];

        /// <summary>
        ///  This method returns a well-formed name for a drag transaction based on the rules it is given.
        /// </summary>
        public static string GetTransactionName(SelectionRules rules, object[] objects)
        {
            // Determine a nice name for the drag operation
            string transactionName;
            if ((int)(rules & SelectionRules.Moveable) != 0)
            {
                if (objects.Length > 1)
                {
                    transactionName = string.Format(SR.DragDropMoveComponents, objects.Length);
                }
                else
                {
                    string name = string.Empty;
                    if (objects.Length > 0)
                    {
                        if (objects[0] is IComponent comp && comp.Site != null)
                        {
                            name = comp.Site.Name;
                        }
                        else
                        {
                            name = objects[0].GetType().Name;
                        }
                    }
                    transactionName = string.Format(SR.DragDropMoveComponent, name);
                }
            }
            else if ((int)(rules & SelectionRules.AllSizeable) != 0)
            {
                if (objects.Length > 1)
                {
                    transactionName = string.Format(SR.DragDropSizeComponents, objects.Length);
                }
                else
                {
                    string name = string.Empty;
                    if (objects.Length > 0)
                    {
                        if (objects[0] is IComponent comp && comp.Site != null)
                        {
                            name = comp.Site.Name;
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
        ///  Called by the designer host when it is entering or leaving a batch operation.  Here we queue up selection notification and we turn off our UI.
        /// </summary>
        private void OnTransactionClosed(object sender, DesignerTransactionCloseEventArgs e)
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
        ///  Called by the designer host when it is entering or leaving a batch operation. Here we queue up selection notification and we turn off our UI.
        /// </summary>
        private void OnTransactionOpened(object sender, EventArgs e)
        {
            _batchMode = true;
        }

        /// <summary>
        ///  update our window region on first create.  We shouldn't do this before the handle is created or else we will force creation.
        /// </summary>
        protected override void OnHandleCreated(EventArgs e)
        {
            Debug.Assert(!RecreatingHandle, "Perf hit: we are recreating the docwin handle");
            base.OnHandleCreated(e);
            // Default the shape of the control to be empty, so that if nothing is initially selected that our window surface doesn't interfere.
            UpdateWindowRegion();
        }

        /// <summary>
        ///  Called whenever a component changes.  Here we update our selection information so that the selection rectangles are all up to date.
        /// </summary>
        private void OnComponentChanged(object sender, ComponentChangedEventArgs ccevent)
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
        ///  called by the formcore when someone has removed a component.  This will remove any selection on the component without disturbing the rest of the selection
        /// </summary>
        private void OnComponentRemove(object sender, ComponentEventArgs ce)
        {
            _selectionHandlers.Remove(ce.Component);
            _selectionItems.Remove(ce.Component);
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
        ///  Called when the selection changes.  We sync up the UI with the selection at this point.
        /// </summary>
        private void OnSelectionChanged(object sender, EventArgs e)
        {
            ICollection selection = _selSvc.GetSelectedComponents();
            Hashtable newSelection = new Hashtable(selection.Count);
            bool shapeChanged = false;
            foreach (object comp in selection)
            {
                object existingItem = _selectionItems[comp];
                bool create = true;
                if (existingItem != null)
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
        private void OnSystemSettingChanged(object sender, EventArgs e) => Invalidate();

        /// <summary>
        ///  User setting requires that we repaint.
        /// </summary>
        private void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e) => Invalidate();

        /// <summary>
        ///  Inheriting classes should override this method to handle this event. Call super.onDragEnter to send this event to any registered event listeners.
        /// </summary>
        protected override void OnDragEnter(DragEventArgs devent)
        {
            base.OnDragEnter(devent);
            if (_dragHandler != null)
            {
                _dragHandler.OleDragEnter(devent);
            }
        }

        /// <summary>
        ///  Inheriting classes should override this method to handle this event. Call super.onDragOver to send this event to any registered event listeners.
        /// </summary>
        protected override void OnDragOver(DragEventArgs devent)
        {
            base.OnDragOver(devent);
            if (_dragHandler != null)
            {
                _dragHandler.OleDragOver(devent);
            }
        }
        /// <summary>
        ///  Inheriting classes should override this method to handle this event. Call super.onDragLeave to send this event to any registered event listeners.
        /// </summary>
        protected override void OnDragLeave(EventArgs e)
        {
            base.OnDragLeave(e);
            if (_dragHandler != null)
            {
                _dragHandler.OleDragLeave();
            }
        }

        /// <summary>
        ///  Inheriting classes should override this method to handle this event. Call super.onDragDrop to send this event to any registered event listeners.
        /// </summary>
        protected override void OnDragDrop(DragEventArgs devent)
        {
            base.OnDragDrop(devent);
            if (_dragHandler != null)
            {
                _dragHandler.OleDragDrop(devent);
            }
        }

        /// <summary>
        ///  Inheriting classes should override this method to handle this event. Call base.OnDoiubleClick to send this event to any registered event listeners.
        /// </summary>
        protected override void OnDoubleClick(EventArgs devent)
        {
            base.OnDoubleClick(devent);
            if (_selSvc != null)
            {
                object selComp = _selSvc.PrimarySelection;
                Debug.Assert(selComp != null, "Illegal selection on double-click");
                if (selComp != null)
                {
                    ISelectionUIHandler handler = GetHandler(selComp);
                    if (handler != null)
                    {
                        handler.OnSelectionDoubleClick((IComponent)selComp);
                    }
                }
            }
        }

        /// <summary>
        ///  Overrides Control to handle our selection grab handles.
        /// </summary>
        // Standard 'catch all - rethrow critical' exception pattern
        protected override void OnMouseDown(MouseEventArgs me)
        {
            if (_dragHandler is null && _selSvc != null)
            {
                try
                {
                    // First, did the user step on anything?
                    Point anchor = PointToScreen(new Point(me.X, me.Y));
                    HitTestInfo hti = GetHitTest(anchor, HITTEST_DEFAULT);
                    int hitTest = hti.hitTest;
                    if ((hitTest & SelectionUIItem.CONTAINER_SELECTOR) != 0)
                    {
                        _selSvc.SetSelectedComponents(new object[] { hti.selectionUIHit._component }, SelectionTypes.Auto);
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
                        _ctrlSelect = (Control.ModifierKeys & Keys.Control) != Keys.None;
                        if (!_ctrlSelect)
                        {
                            _selSvc.SetSelectedComponents(new object[] { hti.selectionUIHit._component }, SelectionTypes.Primary);
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
                            // Our mouse is in drag mode.  We defer the actual move until the user moves the mouse.
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
                catch (Exception e)
                {
                    if (ClientUtils.IsCriticalException(e))
                    {
                        throw;
                    }
                    else if (e != CheckoutException.Canceled)
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
            if (hitTest != SelectionUIItem.CONTAINER_SELECTOR && hti.selectionUIHit != null)
            {
                OnContainerSelectorActive(new ContainerSelectorActiveEventArgs(hti.selectionUIHit._component));
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
                if (_ctrlSelect && !_mouseDragging && _selSvc != null)
                {
                    HitTestInfo hti = GetHitTest(screenCoord, HITTEST_DEFAULT);
                    _selSvc.SetSelectedComponents(new object[] { hti.selectionUIHit._component }, SelectionTypes.Primary);
                }

                if (_mouseDragging)
                {
                    object oldContainerDrag = _containerDrag;
                    bool oldDragMoved = _dragMoved;
                    EndMouseDrag(screenCoord);
                    if (((ISelectionUIService)this).Dragging)
                    {
                        ((ISelectionUIService)this).EndDrag(false);
                    }

                    if (me.Button == MouseButtons.Right && oldContainerDrag != null && !oldDragMoved)
                    {
                        OnContainerSelectorActive(new ContainerSelectorActiveEventArgs(oldContainerDrag, ContainerSelectorActiveEventArgsType.Contextmenu));
                    }
                }
            }
            catch (Exception e)
            {
                if (ClientUtils.IsCriticalException(e))
                {
                    throw;
                }
                else if (e != CheckoutException.Canceled)
                {
                    DisplayError(e);
                }
            }
        }

        /// <summary>
        ///  If the selection manager move, this indicates that the form has autoscolling enabled and has been scrolled.  We have to invalidate here because we may get moved before the rest of the components so we may draw the selection in the wrong spot.
        /// </summary>
        protected override void OnMove(EventArgs e)
        {
            base.OnMove(e);
            Invalidate();
        }

        /// <summary>
        ///  overrides control.onPaint.  here we paint the selection handles.  The window's region was setup earlier.
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
                Cursor cursor = item.GetCursorAtPoint(clientCoords);
                if (cursor != null)
                {
                    if (cursor == Cursors.Default)
                    {
                        Cursor = null;
                    }
                    else
                    {
                        Cursor = cursor;
                    }
                    return;
                }
            }

            foreach (SelectionUIItem item in _selectionItems.Values)
            {
                if (item is ContainerSelectionUIItem)
                {
                    Cursor cursor = item.GetCursorAtPoint(clientCoords);
                    if (cursor != null)
                    {
                        if (cursor == Cursors.Default)
                        {
                            Cursor = null;
                        }
                        else
                        {
                            Cursor = cursor;
                        }
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
            Region region = new Region(new Rectangle(0, 0, 0, 0));
            foreach (SelectionUIItem item in _selectionItems.Values)
            {
                region.Union(item.GetRegion());
            }

            Region = region;
        }

        /// <summary>
        ///  Override of our control's WNDPROC.  We diddle with capture a bit, and it's important to turn this off if the capture changes.
        /// </summary>
        protected override void WndProc(ref Message m)
        {
            switch ((User32.WM)m.Msg)
            {
                case User32.WM.LBUTTONUP:
                case User32.WM.RBUTTONUP:
                    if (_mouseDragAnchor != s_invalidPoint)
                    {
                        _ignoreCaptureChanged = true;
                    }
                    break;
                case User32.WM.CAPTURECHANGED:
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
            get => _dragHandler != null;
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
        ///  Adds an event handler to the ContainerSelectorActive event. This event is fired whenever the user interacts with the container selector in a manor that would indicate that the selector should continued to be displayed. Since the container selector normally will vanish after a timeout, designers should listen to this event and reset the timeout when this event occurs.
        /// </summary>
        event ContainerSelectorActiveEventHandler ISelectionUIService.ContainerSelectorActive
        {
            add => _containerSelectorActive += value;
            remove => _containerSelectorActive -= value;
        }

        /// <summary>
        ///  Assigns a selection UI handler to a given component.  The handler will be called when the UI service needs information about the component.  A single selection UI handler can be assigned to multiple components. When multiple components are dragged, only a single handler may control the drag.  Because of this, only components that are assigned the same handler as the primary selection are included in drag operations. A selection UI handler is automatically unassigned when the component is removed from the container or disposed.
        /// </summary>
        void ISelectionUIService.AssignSelectionUIHandler(object component, ISelectionUIHandler handler)
        {
            ISelectionUIHandler oldHandler = (ISelectionUIHandler)_selectionHandlers[component];
            if (oldHandler != null)
            {
                // The collection editors do not dispose objects from the collection before setting a new collection. This causes items that are common to the old and new collections to come through this code path  again, causing the exception to fire. So, we check to see if the SelectionUIHandler is same, and bail out in that case.
                if (handler == oldHandler)
                {
                    return;
                }
                Debug.Fail("A component may have only one selection UI handler.");
                throw new InvalidOperationException();
            }
            _selectionHandlers[component] = handler;
            // If this component is selected, create a new UI handler for it.
            if (_selSvc != null && _selSvc.GetComponentSelected(component))
            {
                SelectionUIItem item = new SelectionUIItem(this, component);
                _selectionItems[component] = item;
                UpdateWindowRegion();
                item.Invalidate();
            }
        }

        void ISelectionUIService.ClearSelectionUIHandler(object component, ISelectionUIHandler handler)
        {
            ISelectionUIHandler oldHandler = (ISelectionUIHandler)_selectionHandlers[component];
            if (oldHandler == handler)
            {
                _selectionHandlers[component] = null;
            }
        }

        /// <summary>
        ///  This can be called by an outside party to begin a drag of the currently selected set of components.
        /// </summary>
        // Standard 'catch all - rethrow critical' exception pattern
        bool ISelectionUIService.BeginDrag(SelectionRules rules, int initialX, int initialY)
        {
            if (_dragHandler != null)
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
            ISelectionUIHandler primaryHandler = null;
            object primary = _selSvc.PrimarySelection;
            if (primary != null)
            {
                primaryHandler = GetHandler(primary);
            }
            if (primaryHandler is null)
            {
                return false; // no UI handler for selection
            }

            // Now within the given selection, add those items that have the same UI handler and that have the proper rule constraints.
            ArrayList list = new ArrayList();
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
            objects = list.ToArray();
            bool dragging = false;
            // We must setup state before calling QueryBeginDrag.  It is possible that QueryBeginDrag will cancel a drag (if it places a modal dialog, for example), so we must have the drag data all setup before it cancels.  Then, we will check again after QueryBeginDrag to see if a cancel happened.
            _dragComponents = objects;
            _dragRules = rules;
            _dragHandler = primaryHandler;
            string transactionName = GetTransactionName(rules, objects);
            _dragTransaction = _host.CreateTransaction(transactionName);
            try
            {
                if (primaryHandler.QueryBeginDrag(objects, rules, initialX, initialY))
                {
                    if (_dragHandler != null)
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
                    if (_dragTransaction != null)
                    {
                        _dragTransaction.Commit();
                        _dragTransaction = null;
                    }
                }
            }
            return dragging;
        }

        /// <summary>
        ///  Called by an outside party to update drag information.  This can only be called after a successful call to beginDrag.
        /// </summary>
        void ISelectionUIService.DragMoved(Rectangle offset)
        {
            Rectangle newOffset = Rectangle.Empty;
            if (_dragHandler is null)
            {
                throw new Exception(SR.DesignerBeginDragNotCalled);
            }

            Debug.Assert(_dragComponents != null, "We should have a set of drag controls here");
            if ((_dragRules & SelectionRules.Moveable) == SelectionRules.None && (_dragRules & (SelectionRules.TopSizeable | SelectionRules.LeftSizeable)) == SelectionRules.None)
            {
                newOffset = new Rectangle(0, 0, offset.Width, offset.Height);
            }
            if ((_dragRules & SelectionRules.AllSizeable) == SelectionRules.None)
            {
                if (newOffset.IsEmpty)
                {
                    newOffset = new Rectangle(offset.X, offset.Y, 0, 0);
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
        ///  Called by an outside party to finish a drag operation.  This can only be called after a successful call to beginDrag.
        /// </summary>
        // Standard 'catch all - rethrow critical' exception pattern
        void ISelectionUIService.EndDrag(bool cancel)
        {
            _containerDrag = null;
            ISelectionUIHandler handler = _dragHandler;
            object[] components = _dragComponents;
            // Clean these out so that even if we throw an exception we don't die.
            _dragHandler = null;
            _dragComponents = null;
            _dragRules = SelectionRules.None;
            if (handler is null)
            {
                throw new InvalidOperationException();
            }

            // Typically, the handler will be changing a bunch of component properties here. Optimize this by enclosing it within a batch call.
            DesignerTransaction trans = null;
            try
            {
                IComponent comp = components[0] as IComponent;
                if (components.Length > 1 || (components.Length == 1 && comp != null && comp.Site is null))
                {
                    trans = _host.CreateTransaction(string.Format(SR.DragDropMoveComponents, components.Length));
                }
                else if (components.Length == 1)
                {
                    if (comp != null)
                    {
                        trans = _host.CreateTransaction(string.Format(SR.DragDropMoveComponent, comp.Site.Name));
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
                if (trans != null)
                {
                    trans.Commit();
                }
                // Reset the selection.  This will re-display our selection.
                Visible = _savedVisible;
                ((ISelectionUIService)this).SyncSelection();
                if (_dragTransaction != null)
                {
                    _dragTransaction.Commit();
                    _dragTransaction = null;
                }
                // In case this drag was initiated by us, ensure that our mouse state is correct
                EndMouseDrag(MousePosition);
            }
        }

        /// <summary>
        ///  Filters the set of selected components.  The selection service will retrieve all components that are currently selected.  This method allows you to filter this set down to components that match your criteria.  The selectionRules parameter must contain one or more flags from the SelectionRules class.  These flags allow you to constrain the set of selected objects to visible, movable, sizeable or all objects.
        /// </summary>
        object[] ISelectionUIService.FilterSelection(object[] components, SelectionRules selectionRules)
        {
            object[] selection = null;
            if (components is null)
            {
                return Array.Empty<object>();
            }
            // Mask off any selection object that doesn't adhere to the given ruleset. We can ignore this if the ruleset is zero, as all components would be accepted.
            if (selectionRules != SelectionRules.None)
            {
                ArrayList list = new ArrayList();
                foreach (object comp in components)
                {
                    SelectionUIItem item = (SelectionUIItem)_selectionItems[comp];
                    if (item != null && !(item is ContainerSelectionUIItem))
                    {
                        if ((item.GetRules() & selectionRules) == selectionRules)
                        {
                            list.Add(comp);
                        }
                    }
                }
                selection = (object[])list.ToArray();
            }
            return selection ?? (Array.Empty<object>());
        }

        /// <summary>
        ///  Retrieves the width and height of a selection border grab handle. Designers may need this to properly position their user interfaces.
        /// </summary>
        Size ISelectionUIService.GetAdornmentDimensions(AdornmentType adornmentType)
        {
            switch (adornmentType)
            {
                case AdornmentType.GrabHandle:
                    return new Size(SelectionUIItem.GRABHANDLE_WIDTH, SelectionUIItem.GRABHANDLE_HEIGHT);
                case AdornmentType.ContainerSelector:
                case AdornmentType.Maximum:
                    return new Size(ContainerSelectionUIItem.CONTAINER_WIDTH, ContainerSelectionUIItem.CONTAINER_HEIGHT);
            }
            return new Size(0, 0);
        }

        /// <summary>
        ///  Tests to determine if the given screen coordinate is over an adornment for the specified component. This will only return true if the adornment, and selection UI, is visible.
        /// </summary>
        bool ISelectionUIService.GetAdornmentHitTest(object component, Point value) => GetHitTest(value, HITTEST_DEFAULT).hitTest != SelectionUIItem.NOHIT;

        /// <summary>
        ///  Determines if the component is currently "container" selected. Container selection is a visual aid for selecting containers. It doesn't affect the normal "component" selection.
        /// </summary>
        bool ISelectionUIService.GetContainerSelected(object component) => (component != null && _selectionItems[component] is ContainerSelectionUIItem);

        /// <summary>
        ///  Retrieves a set of flags that define rules for the selection.  Selection rules indicate if the given component can be moved or sized, for example.
        /// </summary>
        SelectionRules ISelectionUIService.GetSelectionRules(object component)
        {
            SelectionUIItem sel = (SelectionUIItem)_selectionItems[component];
            if (sel is null)
            {
                Debug.Fail("The component is not currently selected.");
                throw new InvalidOperationException();
            }
            return sel.GetRules();
        }

        /// <summary>
        ///  Allows you to configure the style of the selection frame that a component uses.  This is useful if your component supports different modes of operation (such as an in-place editing mode and a static design mode).  Where possible, you should leave the selection style as is and use the design-time hit testing feature of the IDesigner interface to provide features at design time.  The value of style must be one of the  SelectionStyle enum values. The selection style is only valid for the duration that the component is selected.
        /// </summary>
        SelectionStyles ISelectionUIService.GetSelectionStyle(object component)
        {
            SelectionUIItem s = (SelectionUIItem)_selectionItems[component];
            if (s is null)
            {
                return SelectionStyles.None;
            }
            return s.Style;
        }

        /// <summary>
        ///  Changes the container selection status of the given component. Container selection is a visual aid for selecting containers. It doesn't affect the normal "component" selection.
        /// </summary>
        void ISelectionUIService.SetContainerSelected(object component, bool selected)
        {
            if (selected)
            {
                SelectionUIItem existingItem = (SelectionUIItem)_selectionItems[component];
                if (!(existingItem is ContainerSelectionUIItem))
                {
                    if (existingItem != null)
                    {
                        existingItem.Dispose();
                    }
                    SelectionUIItem item = new ContainerSelectionUIItem(this, component);
                    _selectionItems[component] = item;
                    // Now update our region and invalidate
                    UpdateWindowRegion();
                    if (existingItem != null)
                    {
                        existingItem.Invalidate();
                    }
                    item.Invalidate();
                }
            }
            else
            {
                SelectionUIItem existingItem = (SelectionUIItem)_selectionItems[component];
                if (existingItem is null || existingItem is ContainerSelectionUIItem)
                {
                    _selectionItems.Remove(component);
                    if (existingItem != null)
                    {
                        existingItem.Dispose();
                    }
                    UpdateWindowRegion();
                    existingItem.Invalidate();
                }
            }
        }

        /// <summary>
        ///  Allows you to configure the style of the selection frame that a component uses.  This is useful if your component supports different modes of operation (such as an in-place editing mode and a static design mode).  Where possible, you should leave the selection style as is and use the design-time hit testing feature of the IDesigner interface to provide features at design time.  The value of style must be one of the  SelectionStyle enum values. The selection style is only valid for the duration that the component is selected.
        /// </summary>
        void ISelectionUIService.SetSelectionStyle(object component, SelectionStyles style)
        {
            SelectionUIItem selUI = (SelectionUIItem)_selectionItems[component];
            if (_selSvc != null && _selSvc.GetComponentSelected(component))
            {
                selUI = new SelectionUIItem(this, component);
                _selectionItems[component] = selUI;
            }

            if (selUI != null)
            {
                selUI.Style = style;
                UpdateWindowRegion();
                selUI.Invalidate();
            }
        }

        /// <summary>
        ///  This should be called when a component has been moved, sized or re-parented, but the change was not the result of a property change.  All property changes are monitored by the selection UI service, so this is automatic most of the time.  There are times, however, when a component may be moved without a property change notification occurring.  Scrolling an auto scroll Win32 form is an example of this. This method simply re-queries all currently selected components for their bounds and udpates the selection handles for any that have changed.
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
        ///  This should be called when a component's property changed, that the designer thinks should result in a selection UI change. This method simply re-queries all currently selected components for their bounds and udpates the selection handles for any that have changed.
        /// </summary>
        void ISelectionUIService.SyncComponent(object component)
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

        /// <summary>
        ///  This class represents a single selected object.
        /// </summary>
        private class SelectionUIItem
        {
            // Flags describing how a given selection point may be sized
            public const int SIZE_X = 0x0001;
            public const int SIZE_Y = 0x0002;
            public const int SIZE_MASK = 0x0003;
            // Flags describing how a given selection point may be moved
            public const int MOVE_X = 0x0004;
            public const int MOVE_Y = 0x0008;
            public const int MOVE_MASK = 0x000C;
            // Flags describing where a given selection point is located on an object
            public const int POS_LEFT = 0x0010;
            public const int POS_TOP = 0x0020;
            public const int POS_RIGHT = 0x0040;
            public const int POS_BOTTOM = 0x0080;
            public const int POS_MASK = 0x00F0;
            // This is returned if the given selection point is not within the selection
            public const int NOHIT = 0x0100;
            // This is returned if the given selection point on the "container selector"
            public const int CONTAINER_SELECTOR = 0x0200;
            public const int GRABHANDLE_WIDTH = 7;
            public const int GRABHANDLE_HEIGHT = 7;
            // tables we use to determine how things can move and size
            internal static readonly int[] s_activeSizeArray = new int[] {
                SIZE_X | SIZE_Y | POS_LEFT | POS_TOP,      SIZE_Y | POS_TOP,      SIZE_X | SIZE_Y | POS_TOP | POS_RIGHT,
                SIZE_X | POS_LEFT,                                                SIZE_X | POS_RIGHT,
                SIZE_X | SIZE_Y | POS_LEFT | POS_BOTTOM,   SIZE_Y | POS_BOTTOM,   SIZE_X | SIZE_Y | POS_RIGHT | POS_BOTTOM
            };

            internal static readonly Cursor[] s_activeCursorArrays = new Cursor[] {
                Cursors.SizeNWSE,   Cursors.SizeNS,   Cursors.SizeNESW,
                Cursors.SizeWE,                      Cursors.SizeWE,
                Cursors.SizeNESW,   Cursors.SizeNS,   Cursors.SizeNWSE
            };

            internal static readonly int[] s_inactiveSizeArray = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
            internal static readonly Cursor[] s_inactiveCursorArray = new Cursor[] {
                Cursors.Arrow,   Cursors.Arrow,   Cursors.Arrow,
                Cursors.Arrow,                   Cursors.Arrow,
                Cursors.Arrow,   Cursors.Arrow,   Cursors.Arrow
            };

            internal int[] _sizes; // array of sizing rules for this selection
            internal Cursor[] _cursors; // array of cursors for each grab location
            internal SelectionUIService _selUIsvc;
            internal Rectangle _innerRect = Rectangle.Empty; // inner part of selection (== control bounds)
            internal Rectangle _outerRect = Rectangle.Empty; // outer part of selection (inner + border size)
            internal Region _region; // region object that defines the shape
            internal object _component; // the component we're rendering
            private readonly Control _control;
            private SelectionStyles _selectionStyle; // how do we draw this thing?
            private SelectionRules _selectionRules;
            private readonly ISelectionUIHandler _handler; // the components selection UI handler (can be null)

            ///  Its ok to call virtual method as this is a private class.
            public SelectionUIItem(SelectionUIService selUIsvc, object component)
            {
                _selUIsvc = selUIsvc;
                _component = component;
                _selectionStyle = SelectionStyles.Selected;
                // By default, a component isn't visible.  We must establish what it can do through it's UI handler.
                _handler = selUIsvc.GetHandler(component);
                _sizes = s_inactiveSizeArray;
                _cursors = s_inactiveCursorArray;
                if (component is IComponent comp)
                {
                    if (selUIsvc._host.GetDesigner(comp) is ControlDesigner cd)
                    {
                        _control = cd.Control;
                    }
                }
                UpdateRules();
                UpdateGrabSettings();
                UpdateSize();
            }

            /// <summary>
            ///  Retrieves the style of the selection frame for this selection.
            /// </summary>
            public virtual SelectionStyles Style
            {
                get => _selectionStyle;
                set
                {
                    if (value != _selectionStyle)
                    {
                        _selectionStyle = value;
                        if (_region != null)
                        {
                            _region.Dispose();
                            _region = null;
                        }
                    }
                }
            }

            /// <summary>
            ///  paints the selection
            /// </summary>
            public virtual void DoPaint(Graphics gr)
            {
                // If we're not visible, then there's nothing to do...
                //
                if ((GetRules() & SelectionRules.Visible) == SelectionRules.None)
                {
                    return;
                }

                bool fActive = false;
                if (_selUIsvc._selSvc != null)
                {
                    fActive = _component == _selUIsvc._selSvc.PrimarySelection;
                    // Office rules:  If this is a multi-select, reverse the colors for active / inactive.
                    fActive = (fActive == (_selUIsvc._selSvc.SelectionCount <= 1));
                }

                Rectangle r = new Rectangle(_outerRect.X, _outerRect.Y, GRABHANDLE_WIDTH, GRABHANDLE_HEIGHT);
                Rectangle inner = _innerRect;
                Rectangle outer = _outerRect;
                Region oldClip = gr.Clip;
                Color borderColor = SystemColors.Control;
                if (_control != null && _control.Parent != null)
                {
                    Control parent = _control.Parent;
                    borderColor = parent.BackColor;
                }
                Brush brush = new SolidBrush(borderColor);
                gr.ExcludeClip(inner);
                gr.FillRectangle(brush, outer);
                brush.Dispose();
                gr.Clip = oldClip;
                ControlPaint.DrawSelectionFrame(gr, false, outer, inner, borderColor);
                //if it's not locked & it is sizeable...
                if (((GetRules() & SelectionRules.Locked) == SelectionRules.None) && (GetRules() & SelectionRules.AllSizeable) != SelectionRules.None)
                {
                    // upper left
                    ControlPaint.DrawGrabHandle(gr, r, fActive, (_sizes[0] != 0));
                    // upper right
                    r.X = inner.X + inner.Width;
                    ControlPaint.DrawGrabHandle(gr, r, fActive, _sizes[2] != 0);
                    // lower right
                    r.Y = inner.Y + inner.Height;
                    ControlPaint.DrawGrabHandle(gr, r, fActive, _sizes[7] != 0);
                    // lower left
                    r.X = outer.X;
                    ControlPaint.DrawGrabHandle(gr, r, fActive, _sizes[5] != 0);
                    // lower middle
                    r.X += (outer.Width - GRABHANDLE_WIDTH) / 2;
                    ControlPaint.DrawGrabHandle(gr, r, fActive, _sizes[6] != 0);
                    // upper middle
                    r.Y = outer.Y;
                    ControlPaint.DrawGrabHandle(gr, r, fActive, _sizes[1] != 0);
                    // left middle
                    r.X = outer.X;
                    r.Y = inner.Y + (inner.Height - GRABHANDLE_HEIGHT) / 2;
                    ControlPaint.DrawGrabHandle(gr, r, fActive, _sizes[3] != 0);
                    // right middle
                    r.X = inner.X + inner.Width;
                    ControlPaint.DrawGrabHandle(gr, r, fActive, _sizes[4] != 0);
                }
                else
                {
                    ControlPaint.DrawLockedFrame(gr, outer, fActive);
                }
            }

            /// <summary>
            ///  Retrieves an appropriate cursor at the given point.  If there is no appropriate cursor here (ie, the point lies outside the selection rectangle), then this will return null.
            /// </summary>
            public virtual Cursor GetCursorAtPoint(Point pt)
            {
                Cursor cursor = null;
                if (PointWithinSelection(pt))
                {
                    int nOffset = -1;
                    if ((GetRules() & SelectionRules.AllSizeable) != SelectionRules.None)
                    {
                        nOffset = GetHandleIndexOfPoint(pt);
                    }

                    if (-1 == nOffset)
                    {
                        if ((GetRules() & SelectionRules.Moveable) == SelectionRules.None)
                        {
                            cursor = Cursors.Default;
                        }
                        else
                        {
                            cursor = Cursors.SizeAll;
                        }
                    }
                    else
                    {
                        cursor = _cursors[nOffset];
                    }
                }
                return cursor;
            }

            /// <summary>
            ///  returns the hit test code of the given point.  This may be one of:
            /// </summary>
            public virtual int GetHitTest(Point pt)
            {
                // Is it within our rects?
                if (!PointWithinSelection(pt))
                {
                    return NOHIT;
                }

                // Which index in the array is this?
                int nOffset = GetHandleIndexOfPoint(pt);
                // If no index, the user has picked on the hatch
                if (-1 == nOffset || _sizes[nOffset] == 0)
                {
                    return ((GetRules() & SelectionRules.Moveable) == SelectionRules.None ? 0 : MOVE_X | MOVE_Y);
                }
                return _sizes[nOffset];
            }

            /// <summary>
            ///  gets the array offset of the handle at the given point
            /// </summary>
            private int GetHandleIndexOfPoint(Point pt)
            {
                if (pt.X >= _outerRect.X && pt.X <= _innerRect.X)
                {
                    // Something on the left side.
                    if (pt.Y >= _outerRect.Y && pt.Y <= _innerRect.Y)
                    {
                        return 0; // top left
                    }

                    if (pt.Y >= _innerRect.Y + _innerRect.Height && pt.Y <= _outerRect.Y + _outerRect.Height)
                    {
                        return 5; // bottom left
                    }

                    if (pt.Y >= _outerRect.Y + (_outerRect.Height - GRABHANDLE_HEIGHT) / 2
                        && pt.Y <= _outerRect.Y + (_outerRect.Height + GRABHANDLE_HEIGHT) / 2)
                    {
                        return 3; // middle left
                    }

                    return -1; // unknown hit
                }

                if (pt.Y >= _outerRect.Y && pt.Y <= _innerRect.Y)
                {
                    // something on the top
                    Debug.Assert(!(pt.X >= _outerRect.X && pt.X <= _innerRect.X), "Should be handled by left top check");
                    if (pt.X >= _innerRect.X + _innerRect.Width && pt.X <= _outerRect.X + _outerRect.Width)
                    {
                        return 2; // top right
                    }

                    if (pt.X >= _outerRect.X + (_outerRect.Width - GRABHANDLE_WIDTH) / 2
                        && pt.X <= _outerRect.X + (_outerRect.Width + GRABHANDLE_WIDTH) / 2)
                    {
                        return 1; // top middle
                    }

                    return -1; // unknown hit
                }

                if (pt.X >= _innerRect.X + _innerRect.Width && pt.X <= _outerRect.X + _outerRect.Width)
                {
                    // something on the right side
                    Debug.Assert(!(pt.Y >= _outerRect.Y && pt.Y <= _innerRect.Y), "Should be handled by top right check");
                    if (pt.Y >= _innerRect.Y + _innerRect.Height && pt.Y <= _outerRect.Y + _outerRect.Height)
                    {
                        return 7; // bottom right
                    }

                    if (pt.Y >= _outerRect.Y + (_outerRect.Height - GRABHANDLE_HEIGHT) / 2
                        && pt.Y <= _outerRect.Y + (_outerRect.Height + GRABHANDLE_HEIGHT) / 2)
                    {
                        return 4; // middle right
                    }

                    return -1; // unknown hit
                }

                if (pt.Y >= _innerRect.Y + _innerRect.Height && pt.Y <= _outerRect.Y + _outerRect.Height)
                {
                    // something on the bottom
                    Debug.Assert(!(pt.X >= _outerRect.X && pt.X <= _innerRect.X), "Should be handled by left bottom check");

                    Debug.Assert(!(pt.X >= _innerRect.X + _innerRect.Width && pt.X <= _outerRect.X + _outerRect.Width), "Should be handled by right bottom check");

                    if (pt.X >= _outerRect.X + (_outerRect.Width - GRABHANDLE_WIDTH) / 2 && pt.X <= _outerRect.X + (_outerRect.Width + GRABHANDLE_WIDTH) / 2)
                    {
                        return 6; // bottom middle
                    }

                    return -1; // unknown hit
                }
                return -1; // unknown hit
            }

            /// <summary>
            ///  returns a region handle that defines this selection.  This is used to piece together a paint region for the surface that we draw our selection handles on
            /// </summary>
            public virtual Region GetRegion()
            {
                if (_region is null)
                {
                    if ((GetRules() & SelectionRules.Visible) != SelectionRules.None && !_outerRect.IsEmpty)
                    {
                        _region = new Region(_outerRect);
                        _region.Exclude(_innerRect);
                    }
                    else
                    {
                        _region = new Region(new Rectangle(0, 0, 0, 0));
                    }

                    if (_handler != null)
                    {
                        Rectangle handlerClip = _handler.GetSelectionClipRect(_component);
                        if (!handlerClip.IsEmpty)
                        {
                            _region.Intersect(_selUIsvc.RectangleToClient(handlerClip));
                        }
                    }
                }
                return _region;
            }

            /// <summary>
            ///  Retrieves the rules associated with this selection.
            /// </summary>
            public SelectionRules GetRules() => _selectionRules;

            public void Dispose()
            {
                if (_region != null)
                {
                    _region.Dispose();
                    _region = null;
                }
            }

            /// <summary>
            ///  Invalidates the region for this selection glyph.
            /// </summary>
            public void Invalidate()
            {
                if (!_outerRect.IsEmpty && !_selUIsvc.Disposing)
                {
                    _selUIsvc.Invalidate(_outerRect);
                }
            }

            /// <summary>
            ///  Part of our hit testing logic; determines if the point is somewhere within our selection.
            /// </summary>
            protected bool PointWithinSelection(Point pt)
            {
                // This is only supported for visible selections
                if ((GetRules() & SelectionRules.Visible) == SelectionRules.None || _outerRect.IsEmpty || _innerRect.IsEmpty)
                {
                    return false;
                }
                if (pt.X < _outerRect.X || pt.X > _outerRect.X + _outerRect.Width)
                {
                    return false;
                }
                if (pt.Y < _outerRect.Y || pt.Y > _outerRect.Y + _outerRect.Height)
                {
                    return false;
                }
                if (pt.X > _innerRect.X
                    && pt.X < _innerRect.X + _innerRect.Width
                    && pt.Y > _innerRect.Y
                    && pt.Y < _innerRect.Y + _innerRect.Height)
                {
                    return false;
                }
                return true;
            }

            /// <summary>
            ///  Updates the available grab handle settings based on the current rules.
            /// </summary>
            private void UpdateGrabSettings()
            {
                SelectionRules rules = GetRules();
                if ((rules & SelectionRules.AllSizeable) == SelectionRules.None)
                {
                    _sizes = s_inactiveSizeArray;
                    _cursors = s_inactiveCursorArray;
                }
                else
                {
                    _sizes = new int[8];
                    _cursors = new Cursor[8];
                    Array.Copy(s_activeCursorArrays, _cursors, _cursors.Length);
                    Array.Copy(s_activeSizeArray, _sizes, _sizes.Length);
                    if ((rules & SelectionRules.TopSizeable) != SelectionRules.TopSizeable)
                    {
                        _sizes[0] = 0;
                        _sizes[1] = 0;
                        _sizes[2] = 0;
                        _cursors[0] = Cursors.Arrow;
                        _cursors[1] = Cursors.Arrow;
                        _cursors[2] = Cursors.Arrow;
                    }
                    if ((rules & SelectionRules.LeftSizeable) != SelectionRules.LeftSizeable)
                    {
                        _sizes[0] = 0;
                        _sizes[3] = 0;
                        _sizes[5] = 0;
                        _cursors[0] = Cursors.Arrow;
                        _cursors[3] = Cursors.Arrow;
                        _cursors[5] = Cursors.Arrow;
                    }
                    if ((rules & SelectionRules.BottomSizeable) != SelectionRules.BottomSizeable)
                    {
                        _sizes[5] = 0;
                        _sizes[6] = 0;
                        _sizes[7] = 0;
                        _cursors[5] = Cursors.Arrow;
                        _cursors[6] = Cursors.Arrow;
                        _cursors[7] = Cursors.Arrow;
                    }
                    if ((rules & SelectionRules.RightSizeable) != SelectionRules.RightSizeable)
                    {
                        _sizes[2] = 0;
                        _sizes[4] = 0;
                        _sizes[7] = 0;
                        _cursors[2] = Cursors.Arrow;
                        _cursors[4] = Cursors.Arrow;
                        _cursors[7] = Cursors.Arrow;
                    }
                }
            }

            /// <summary>
            ///  Updates our cached selection rules based on current handler values.
            /// </summary>
            public void UpdateRules()
            {
                if (_handler is null)
                {
                    _selectionRules = SelectionRules.None;
                }
                else
                {
                    SelectionRules oldRules = _selectionRules;
                    _selectionRules = _handler.GetComponentRules(_component);
                    if (_selectionRules != oldRules)
                    {
                        UpdateGrabSettings();
                        Invalidate();
                    }
                }
            }

            /// <summary>
            ///  rebuilds the inner and outer rectangles based on the current selItem.component dimensions.  We could calcuate this every time, but that would be expensive for functions like getHitTest that are called a lot (like on every mouse move)
            /// </summary>
            public virtual bool UpdateSize()
            {
                bool sizeChanged = false;
                // Short circuit common cases
                if (_handler is null)
                {
                    return false;
                }

                if ((GetRules() & SelectionRules.Visible) == SelectionRules.None)
                {
                    return false;
                }

                _innerRect = _handler.GetComponentBounds(_component);
                if (!_innerRect.IsEmpty)
                {
                    _innerRect = _selUIsvc.RectangleToClient(_innerRect);
                    Rectangle rcOuterNew = new Rectangle(_innerRect.X - GRABHANDLE_WIDTH, _innerRect.Y - GRABHANDLE_HEIGHT, _innerRect.Width + 2 * GRABHANDLE_WIDTH, _innerRect.Height + 2 * GRABHANDLE_HEIGHT);
                    if (_outerRect.IsEmpty || !_outerRect.Equals(rcOuterNew))
                    {
                        if (!_outerRect.IsEmpty)
                        {
                            Invalidate();
                        }

                        _outerRect = rcOuterNew;
                        Invalidate();
                        if (_region != null)
                        {
                            _region.Dispose();
                            _region = null;
                        }
                        sizeChanged = true;
                    }
                }
                else
                {
                    Rectangle rcNew = new Rectangle(0, 0, 0, 0);
                    sizeChanged = _outerRect.IsEmpty || !_outerRect.Equals(rcNew);
                    _innerRect = _outerRect = rcNew;
                }
                return sizeChanged;
            }
        }

        private class ContainerSelectionUIItem : SelectionUIItem
        {
            public const int CONTAINER_WIDTH = 13;
            public const int CONTAINER_HEIGHT = 13;

            public ContainerSelectionUIItem(SelectionUIService selUIsvc, object component) : base(selUIsvc, component)
            {
            }

            public override Cursor GetCursorAtPoint(Point pt)
            {
                if ((GetHitTest(pt) & CONTAINER_SELECTOR) != 0 && (GetRules() & SelectionRules.Moveable) != SelectionRules.None)
                {
                    return Cursors.SizeAll;
                }
                else
                {
                    return null;
                }
            }

            public override int GetHitTest(Point pt)
            {
                int ht = NOHIT;
                if ((GetRules() & SelectionRules.Visible) != SelectionRules.None && !_outerRect.IsEmpty)
                {
                    Rectangle r = new Rectangle(_outerRect.X, _outerRect.Y, CONTAINER_WIDTH, CONTAINER_HEIGHT);

                    if (r.Contains(pt))
                    {
                        ht = CONTAINER_SELECTOR;
                        if ((GetRules() & SelectionRules.Moveable) != SelectionRules.None)
                        {
                            ht |= MOVE_X | MOVE_Y;
                        }
                    }
                }
                return ht;
            }

            public override void DoPaint(Graphics gr)
            {
                // If we're not visible, then there's nothing to do...
                if ((GetRules() & SelectionRules.Visible) == SelectionRules.None)
                {
                    return;
                }

                Rectangle glyphBounds = new Rectangle(_outerRect.X, _outerRect.Y, CONTAINER_WIDTH, CONTAINER_HEIGHT);
                ControlPaint.DrawContainerGrabHandle(gr, glyphBounds);
            }

            public override Region GetRegion()
            {
                if (_region is null)
                {
                    if ((GetRules() & SelectionRules.Visible) != SelectionRules.None && !_outerRect.IsEmpty)
                    {
                        Rectangle r = new Rectangle(_outerRect.X, _outerRect.Y, CONTAINER_WIDTH, CONTAINER_HEIGHT);
                        _region = new Region(r);
                    }
                    else
                    {
                        _region = new Region(new Rectangle(0, 0, 0, 0));
                    }
                }
                return _region;
            }
        }

        private struct HitTestInfo
        {
            public readonly int hitTest;
            public readonly SelectionUIItem selectionUIHit;
            public readonly bool containerSelector;

            public HitTestInfo(int hitTest, SelectionUIItem selectionUIHit)
            {
                this.hitTest = hitTest;
                this.selectionUIHit = selectionUIHit;
                containerSelector = false;
            }

            public HitTestInfo(int hitTest, SelectionUIItem selectionUIHit, bool containerSelector)
            {
                this.hitTest = hitTest;
                this.selectionUIHit = selectionUIHit;
                this.containerSelector = containerSelector;
            }

            // Standard 'catch all - rethrow critical' exception pattern
            public override bool Equals(object obj)
            {
                try
                {
                    HitTestInfo hi = (HitTestInfo)obj;
                    return hitTest == hi.hitTest && selectionUIHit == hi.selectionUIHit && containerSelector == hi.containerSelector;
                }
                catch (Exception ex)
                {
                    if (ClientUtils.IsCriticalException(ex))
                    {
                        throw;
                    }
                }
                return false;
            }

            public static bool operator ==(HitTestInfo left, HitTestInfo right)
            {
                return (left.hitTest == right.hitTest && left.selectionUIHit == right.selectionUIHit && left.containerSelector == right.containerSelector);
            }

            public static bool operator !=(HitTestInfo left, HitTestInfo right) => !(left == right);

            public override int GetHashCode()
            {
                int hash = hitTest | selectionUIHit.GetHashCode();
                if (containerSelector)
                {
                    hash |= 0x10000;
                }
                return hash;
            }
        }
    }
}
