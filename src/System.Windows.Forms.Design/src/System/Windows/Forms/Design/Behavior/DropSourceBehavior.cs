// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;

namespace System.Windows.Forms.Design.Behavior;

/// <summary>
///  The DropSourceBehavior is created by ControlDesigner when it detects that a drag operation has started.
///  This object is passed to the BehaviorService and is used to route GiveFeedback and QueryContinueDrag
///  drag/drop messages. In response to GiveFeedback messages, this class will render the dragging controls
///  in real-time with the help of the DragAssistanceManager (Snaplines) object or by simply snapping to grid dots.
/// </summary>
internal sealed partial class DropSourceBehavior : Behavior, IComparer
{
    private struct DragComponent
    {
        public IComponent dragComponent; // the dragComponent
        public int zorderIndex; // the dragComponent's z-order index
        public Point originalControlLocation; // the original control of the control in AdornerWindow coordinates
        public Point draggedLocation; // the location of the component after each drag - in AdornerWindow coordinates
        public Bitmap dragImage; // bitblt'd image of control
        public Point positionOffset; // control position offset from primary selection
    }

    private readonly DragComponent[] _dragComponents;
    private List<IComponent> _dragObjects; // used to initialize the DragAssistanceManager
    private readonly BehaviorDataObject _data; // drag data that represents the controls we're dragging & the effect/action
    private readonly DragDropEffects _allowedEffects; // initial allowed effects for the drag operation
    private DragDropEffects _lastEffect; // the last effect we saw (used for determining a valid drop)

    private bool _targetAllowsSnapLines; // indicates if the drop target allows snaplines (flowpanels don't for ex)
    private IComponent _lastDropTarget; // indicates the drop target on the last 'give feedback' event
    private Point _lastSnapOffset; // the last SnapOffset we used.
    // These 2 could be different (e.g. if dropping between forms)
    private readonly BehaviorService _behaviorServiceSource; // ptr back to the BehaviorService in the drop source
    private BehaviorService _behaviorServiceTarget; // ptr back to the BehaviorService in the drop target

    // this object will integrate SnapLines into the drag
    private DragAssistanceManager _dragAssistanceManager;

    private Graphics _graphicsTarget; // graphics object of the AdornerWindows (via BehaviorService) in drop target

    private readonly IServiceProvider _serviceProviderSource;
    private IServiceProvider _serviceProviderTarget;

    private Point _initialMouseLoc; // original mouse location in screen coordinates

    private Image _dragImage; // A single image of the controls we are actually dragging around
    private Rectangle _dragImageRect; // Rectangle of the dragImage -- in SOURCE AdornerWindow coordinates
    private Rectangle _clearDragImageRect; // Rectangle used to remember the last dragimage rect we cleared
    private Point _originalDragImageLocation; // original location of the drag image
    private Region _dragImageRegion;

    private Point _lastFeedbackLocation; // the last position we got feedback at
    private Control _suspendedParent; // pointer to the parent that we suspended @ the beginning of the drag
    private Size _parentGridSize; // used to snap around to grid dots if layoutmode == SnapToGrid
    private Point _parentLocation; // location of parent on AdornerWindow - used for grid snap calculations
    private bool _shareParent = true; // do dragged components share the parent
    private bool _cleanedUpDrag;
    private StatusCommandUI _statusCommandUITarget; // UI for setting the StatusBar Information in the drop target

    private readonly IDesignerHost _srcHost;
    private IDesignerHost _destHost;

    private bool _currentShowState = true; // Initially the controls are showing

    private int _primaryComponentIndex = -1; // Index of the primary component (control) in dragComponents

    /// <summary>
    ///  Constructor that caches all needed variables for perf reasons.
    /// </summary>
    internal DropSourceBehavior(List<IComponent> dragComponents, Control source, Point initialMouseLocation)
    {
        _serviceProviderSource = source.Site;
        if (_serviceProviderSource is null)
        {
            Debug.Fail("DragBehavior could not be created because the source ServiceProvider was not found");
            return;
        }

        _behaviorServiceSource = (BehaviorService)_serviceProviderSource.GetService(typeof(BehaviorService));
        if (_behaviorServiceSource is null)
        {
            Debug.Fail("DragBehavior could not be created because the BehaviorService was not found");
            return;
        }

        if (dragComponents is null || dragComponents.Count <= 0)
        {
            Debug.Fail("There are no component to drag!");
            return;
        }

        _srcHost = (IDesignerHost)_serviceProviderSource.GetService(typeof(IDesignerHost));
        if (_srcHost is null)
        {
            Debug.Fail("DragBehavior could not be created because the srcHost could not be found");
            return;
        }

        _data = new BehaviorDataObject(dragComponents, source, this);
        _allowedEffects = DragDropEffects.Copy | DragDropEffects.None | DragDropEffects.Move;
        _dragComponents = new DragComponent[dragComponents.Count];
        _parentGridSize = Size.Empty;

        _lastEffect = DragDropEffects.None;
        _lastFeedbackLocation = new Point(-1, -1);
        _lastSnapOffset = Point.Empty;
        _dragImageRect = Rectangle.Empty;
        _clearDragImageRect = Rectangle.Empty;
        InitiateDrag(initialMouseLocation, dragComponents);
    }

    /// <summary>
    ///  This is the initial allowed Effect to start the drag operation with.
    /// </summary>
    internal DragDropEffects AllowedEffects
    {
        get => _allowedEffects;
    }

    /// <summary>
    ///  This is the DataObject this DropSourceBehavior represents.
    /// </summary>
    internal DataObject DataObject
    {
        get => _data;
    }

    /// <summary>
    ///  Here, during our drag operation, we need to determine the offset from the dragging control's position 'dragLoc'
    ///  and the parent's grid. We'll return an offset for the image to 'snap to'.
    /// </summary>
    private Point AdjustToGrid(Point dragLoc)
    {
        // location of the drag with respect to the parent
        Point controlLocation = new(dragLoc.X - _parentLocation.X, dragLoc.Y - _parentLocation.Y);
        Point offset = Point.Empty;
        // determine which way we need to snap
        int xDelta = controlLocation.X % _parentGridSize.Width;
        int yDelta = controlLocation.Y % _parentGridSize.Height;
        // if we're more than half way to the next grid - then snap that way, otherwise snap back
        if (xDelta > _parentGridSize.Width / 2)
        {
            offset.X = _parentGridSize.Width - xDelta;
        }
        else
        {
            offset.X = -xDelta;
        }

        if (yDelta > _parentGridSize.Height / 2)
        {
            offset.Y = _parentGridSize.Height - yDelta;
        }
        else
        {
            offset.Y = -yDelta;
        }

        return offset;
    }

    private Point MapPointFromSourceToTarget(Point pt)
    {
        if (_srcHost != _destHost && _destHost is not null)
        {
            pt = _behaviorServiceSource.AdornerWindowPointToScreen(pt);
            return _behaviorServiceTarget.MapAdornerWindowPoint(IntPtr.Zero, pt);
        }
        else
        {
            return pt;
        }
    }

    private Point MapPointFromTargetToSource(Point pt)
    {
        if (_srcHost != _destHost && _destHost is not null)
        {
            pt = _behaviorServiceTarget.AdornerWindowPointToScreen(pt);
            return _behaviorServiceSource.MapAdornerWindowPoint(IntPtr.Zero, pt);
        }
        else
        {
            return pt;
        }
    }

    /// <summary>
    ///  This is used to clear the drag images.
    /// </summary>
    private void ClearAllDragImages()
    {
        if (_dragImageRect != Rectangle.Empty)
        {
            Rectangle rect = _dragImageRect;
            rect.Location = MapPointFromSourceToTarget(rect.Location);

            _graphicsTarget?.SetClip(rect);

            _behaviorServiceTarget?.Invalidate(rect);

            _graphicsTarget?.ResetClip();
        }
    }

    // Yeah this is recursive, but we also need to resite all
    // the children of this control, and their children, and their children...
    private void SetDesignerHost(Control c)
    {
        foreach (Control control in c.Controls)
        {
            SetDesignerHost(control);
        }

        if (c.Site is not null && !(c.Site is INestedSite) && _destHost is not null)
        {
            _destHost.Container.Add(c);
        }
    }

    private void DropControl(int dragComponentIndex, Control dragTarget, Control dragSource, bool localDrag)
    {
        Control currentControl = _dragComponents[dragComponentIndex].dragComponent as Control;
        if (_lastEffect == DragDropEffects.Copy || (_srcHost != _destHost && _destHost is not null))
        {
            // between forms or copy
            currentControl.Visible = true;
            bool visibleState = true;
            PropertyDescriptor propLoc = TypeDescriptor.GetProperties(currentControl)["Visible"];
            if (propLoc is not null)
            {
                // store off the visible state. When adding the control to the new designer host,
                // a new control designer will be created for the control.
                // Since currentControl.Visible is currently FALSE (See InitiateDrag),
                // the shadowed Visible property will be FALSE as well. This is not what we want.
                visibleState = (bool)propLoc.GetValue(currentControl);
            }

            // Hook the control to its new designerHost
            SetDesignerHost(currentControl);
            currentControl.Parent = dragTarget;
            // Make sure and set the Visible property to the correct value
            propLoc?.SetValue(currentControl, visibleState);
        }
        else if (!localDrag && currentControl.Parent.Equals(dragSource))
        {
            // between containers
            dragSource.Controls.Remove(currentControl);
            currentControl.Visible = true;
            dragTarget.Controls.Add(currentControl);
        }
    }

    private void SetLocationPropertyAndChildIndex(int dragComponentIndex, Control dragTarget, Point dropPoint, int newIndex, bool allowSetChildIndexOnDrop)
    {
        PropertyDescriptor propLoc = TypeDescriptor.GetProperties(_dragComponents[dragComponentIndex].dragComponent)["Location"];
        if ((propLoc is not null) && (_dragComponents[dragComponentIndex].dragComponent is Control currentControl))
        {
            // ControlDesigner shadows the Location property. If the control is parented and
            // the parent is a scrollable control, then it expects the Location to be in displayRectangle coordinates.
            // At this point bounds are in clientRectangle coordinates,
            // so we need to check if we need to adjust the coordinates.
            Point pt = new(dropPoint.X, dropPoint.Y);
            if (currentControl.Parent is ScrollableControl p)
            {
                Point ptScroll = p.AutoScrollPosition;
                // always want to add the control below/right of the AutoScrollPosition
                pt.Offset(-ptScroll.X, -ptScroll.Y);
            }

            propLoc.SetValue(currentControl, pt);
            // In some cases the target designer wants to maintain its own ZOrder,
            // in that case we shouldn't try and set the childIndex. FlowLayoutPanelDesigner is one such case.
            if (allowSetChildIndexOnDrop)
            {
                dragTarget.Controls.SetChildIndex(currentControl, newIndex);
            }
        }
    }

    /// <summary>
    ///  This is where we end the drag and commit the new control locations.
    ///  To do this correctly, we loop through every control and find its propertyDescriptor for the Location.
    ///  Then call SetValue(). After this we re-enable the adorners. Finally, we pop ourselves from the BehaviorStack.
    /// </summary>
    private void EndDragDrop(bool allowSetChildIndexOnDrop)
    {
        if (_data.Target is not Control dragTarget)
        {
            return; // can't deal with a non-control drop target yet
        }

        // If for some reason we couldn't get these guys, let's try and get them here
        if (_serviceProviderTarget is null)
        {
            Debug.Fail("EndDragDrop - how can serviceProviderTarget be null?");
            _serviceProviderTarget = dragTarget.Site;
            if (_serviceProviderTarget is null)
            {
                Debug.Fail("EndDragDrop - how can serviceProviderTarget be null?");
                return;
            }
        }

        if (_destHost is null)
        {
            Debug.Fail("EndDragDrop - how can destHost be null?");
            _destHost = (IDesignerHost)_serviceProviderTarget.GetService(typeof(IDesignerHost));
            if (_destHost is null)
            {
                Debug.Fail("EndDragDrop - how can destHost be null?");
                return;
            }
        }

        if (_behaviorServiceTarget is null)
        {
            Debug.Fail("EndDragDrop - how can behaviorServiceTarget be null?");
            _behaviorServiceTarget = (BehaviorService)_serviceProviderTarget.GetService(typeof(BehaviorService));
            if (_behaviorServiceTarget is null)
            {
                Debug.Fail("EndDragDrop - how can behaviorServiceTarget be null?");
                return;
            }
        }

        // We use this list when doing a Drag-Copy, so that we can correctly restore state when we are done. See Copy code below.
        List<IComponent> originalControls = null;
        bool performCopy = (_lastEffect == DragDropEffects.Copy);

        Control dragSource = _data.Source;
        bool localDrag = dragSource.Equals(dragTarget);
        PropertyDescriptor targetProp = TypeDescriptor.GetProperties(dragTarget)["Controls"];
        PropertyDescriptor sourceProp = TypeDescriptor.GetProperties(dragSource)["Controls"];
        IComponentChangeService componentChangeSvcSource = (IComponentChangeService)_serviceProviderSource.GetService(typeof(IComponentChangeService));
        IComponentChangeService componentChangeSvcTarget = (IComponentChangeService)_serviceProviderTarget.GetService(typeof(IComponentChangeService));

        _dragAssistanceManager?.OnMouseUp();

        // If we are dropping between hosts, we want to set the selection in the new host to be the components
        // that we are dropping. ... or if we are copying.
        ISelectionService selSvc = null;
        if (performCopy || (_srcHost != _destHost && _destHost is not null))
        {
            selSvc = (ISelectionService)_serviceProviderTarget.GetService(typeof(ISelectionService));
        }

        try
        {
            if (_dragComponents is not null && _dragComponents.Length > 0)
            {
                DesignerTransaction transSource = null;
                DesignerTransaction transTarget = null;
                string transDesc;
                if (_dragComponents.Length == 1)
                {
                    string name = TypeDescriptor.GetComponentName(_dragComponents[0].dragComponent);
                    if (name is null || name.Length == 0)
                    {
                        name = _dragComponents[0].dragComponent.GetType().Name;
                    }

                    transDesc = string.Format(performCopy ? SR.BehaviorServiceCopyControl : SR.BehaviorServiceMoveControl, name);
                }
                else
                {
                    transDesc = string.Format(performCopy ? SR.BehaviorServiceCopyControls : SR.BehaviorServiceMoveControls, _dragComponents.Length);
                }

                // We don't want to create a transaction in the source, if we are doing a cross-form copy
                if (_srcHost is not null && !(_srcHost != _destHost && _destHost is not null && performCopy))
                {
                    transSource = _srcHost.CreateTransaction(transDesc);
                }

                if (_srcHost != _destHost && _destHost is not null)
                {
                    transTarget = _destHost.CreateTransaction(transDesc);
                }

                try
                {
                    ComponentTray tray = null;
                    int numberOfOriginalTrayControls = 0;
                    // If we are copying the controls, then, well, let's make a copy of 'em...
                    // We then stuff the copy into the dragComponents array, since that keeps the rest of this code
                    // the same... No special casing needed.
                    if (performCopy)
                    {
                        // As part of a Ctrl-Drag, components might have been added to the component tray,
                        // make sure that their location gets updated as well (think ToolStrips).
                        // Get the current number of controls in the Component Tray in the target.
                        tray = _serviceProviderTarget.GetService(typeof(ComponentTray)) as ComponentTray;
                        numberOfOriginalTrayControls = tray is not null ? tray.Controls.Count : 0;

                        // Get the objects to copy
                        List<IComponent> temp = [];
                        for (int i = 0; i < _dragComponents.Length; i++)
                        {
                            temp.Add(_dragComponents[i].dragComponent);
                        }

                        // Create a copy of them
                        temp = DesignerUtils.CopyDragObjects(temp, _serviceProviderTarget);
                        if (temp is null)
                        {
                            Debug.Fail("Couldn't create copies of the controls we are dragging.");
                            return;
                        }

                        originalControls = [];
                        // And stick the copied controls back into the dragComponents array
                        for (int j = 0; j < temp.Count; j++)
                        {
                            // ... but save off the old controls first
                            originalControls.Add(_dragComponents[j].dragComponent);
                            _dragComponents[j].dragComponent = temp[j];
                        }
                    }

                    if ((!localDrag || performCopy) && componentChangeSvcSource is not null && componentChangeSvcTarget is not null)
                    {
                        componentChangeSvcTarget.OnComponentChanging(dragTarget, targetProp);
                        // If we are performing a copy, then the dragSource will not change
                        if (!performCopy)
                        {
                            componentChangeSvcSource.OnComponentChanging(dragSource, sourceProp);
                        }
                    }

                    // We need to calculate initialDropPoint first to be able to calculate the new drop point for
                    // all controls Need to drop it first to make sure that the Parent gets set correctly.
                    DropControl(_primaryComponentIndex, dragTarget, dragSource, localDrag);
                    Point initialDropPoint = _behaviorServiceSource.AdornerWindowPointToScreen(_dragComponents[_primaryComponentIndex].draggedLocation);

                    // Tricky... initialDropPoint is the dropPoint in the source adornerWindow,
                    // which could be different than the target adornerWindow. But since we first convert
                    // it to screen coordinates, and then to client coordinates using the new parent,
                    // we end up dropping in the right spot. Cool, huh!
                    initialDropPoint = ((Control)_dragComponents[_primaryComponentIndex].dragComponent).Parent.PointToClient(initialDropPoint);

                    // Correct (only) the drop point for when Parent is mirrored,
                    // then use the offsets for the other controls, which were already
                    // corrected for mirroring in InitDrag.
                    if (((Control)(_dragComponents[_primaryComponentIndex].dragComponent)).Parent.IsMirrored)
                    {
                        initialDropPoint.Offset(-((Control)(_dragComponents[_primaryComponentIndex].dragComponent)).Width, 0);
                    }

                    // check permission to do that
                    Control primaryComponent = _dragComponents[_primaryComponentIndex].dragComponent as Control;
                    PropertyDescriptor propLoc = TypeDescriptor.GetProperties(primaryComponent)["Location"];
                    if (primaryComponent is not null && propLoc is not null)
                    {
                        try
                        {
                            componentChangeSvcTarget.OnComponentChanging(primaryComponent, propLoc);
                        }

                        catch (CheckoutException coEx)
                        {
                            if (coEx == CheckoutException.Canceled)
                            {
                                return;
                            }

                            throw;
                        }
                    }

                    // everything is fine, carry on...
                    SetLocationPropertyAndChildIndex(_primaryComponentIndex, dragTarget, initialDropPoint,
                                                        _shareParent ? _dragComponents[_primaryComponentIndex].zorderIndex : 0, allowSetChildIndexOnDrop);
                    selSvc?.SetSelectedComponents(new object[] { _dragComponents[_primaryComponentIndex].dragComponent }, SelectionTypes.Primary | SelectionTypes.Replace);

                    for (int i = 0; i < _dragComponents.Length; i++)
                    {
                        if (i == _primaryComponentIndex)
                        {
                            // did this one above
                            continue;
                        }

                        DropControl(i, dragTarget, dragSource, localDrag);
                        Point dropPoint = new(initialDropPoint.X + _dragComponents[i].positionOffset.X,
                                                        initialDropPoint.Y + _dragComponents[i].positionOffset.Y);
                        SetLocationPropertyAndChildIndex(i, dragTarget, dropPoint,
                                                            _shareParent ? _dragComponents[i].zorderIndex : 0, allowSetChildIndexOnDrop);
                        selSvc?.SetSelectedComponents(new object[] { _dragComponents[i].dragComponent }, SelectionTypes.Add);
                    }

                    if ((!localDrag || performCopy) && componentChangeSvcSource is not null && componentChangeSvcTarget is not null)
                    {
                        componentChangeSvcTarget.OnComponentChanged(dragTarget, targetProp, dragTarget.Controls, dragTarget.Controls);
                        if (!performCopy)
                        {
                            componentChangeSvcSource.OnComponentChanged(dragSource, sourceProp, dragSource.Controls, dragSource.Controls);
                        }
                    }

                    // If we did a Copy, then restore the old controls to make sure we set state correctly
                    if (originalControls is not null)
                    {
                        for (int i = 0; i < originalControls.Count; i++)
                        {
                            _dragComponents[i].dragComponent = originalControls[i];
                        }

                        originalControls = null;
                    }

                    // Rearrange the Component Tray - if we have to
                    if (performCopy)
                    {
                        // the target did not have a tray already, so let's go get it - if there is one
                        tray ??= _serviceProviderTarget.GetService(typeof(ComponentTray)) as ComponentTray;

                        if (tray is not null)
                        {
                            int numberOfTrayControlsAdded = tray.Controls.Count - numberOfOriginalTrayControls;

                            if (numberOfTrayControlsAdded > 0)
                            {
                                List<Control> listOfTrayControls = [];
                                for (int i = 0; i < numberOfTrayControlsAdded; i++)
                                {
                                    listOfTrayControls.Add(tray.Controls[numberOfOriginalTrayControls + i]);
                                }

                                tray.UpdatePastePositions(listOfTrayControls);
                            }
                        }
                    }

                    // We need to CleanupDrag BEFORE we commit the transaction.
                    // The reason is that cleaning up can potentially cause a layout,
                    // and then any changes that happen due to the layout would be in a separate UndoUnit.
                    // We want the D&D to be undoable in one step.
                    CleanupDrag(false);
                    if (transSource is not null)
                    {
                        transSource.Commit();
                        transSource = null;
                    }

                    if (transTarget is not null)
                    {
                        transTarget.Commit();
                        transTarget = null;
                    }
                }

                finally
                {
                    transSource?.Cancel();

                    transTarget?.Cancel();
                }
            }
        }
        finally
        {
            // If we did a Copy, then restore the old controls to make sure we set state correctly
            if (originalControls is not null)
            {
                for (int i = 0; i < originalControls.Count; i++)
                {
                    _dragComponents[i].dragComponent = originalControls[i];
                }
            }

            // Even though we call CleanupDrag(false) twice (see above), this method guards against doing the wrong thing.
            CleanupDrag(false);
            // if selSvs is not null, then we either did a copy, or moved between forms, so use it to set the right info
            _statusCommandUITarget?.SetStatusInformation(selSvc is null ? _dragComponents[_primaryComponentIndex].dragComponent as Component :
                                                                        selSvc.PrimarySelection as Component);
        }

        // clear the last feedback loc
        _lastFeedbackLocation = new Point(-1, -1);
    }

    /// <summary>
    ///  Called by the BehaviorService when the GiveFeedback event is fired.
    ///  Here, we attempt to render all of our dragging control snapshots.
    ///  *After, of course, we let the DragAssistanceManager adjust the position due to any SnapLine activity.
    /// </summary>
    internal void GiveFeedback(object sender, GiveFeedbackEventArgs e)
    {
        // cache off this last effect so in QueryContinueDrag we can identify (if dropped) a valid drop operation
        _lastEffect = e.Effect;
        // if our target is null, we can't drop anywhere, so don't even draw images
        if (_data.Target is null || e.Effect == DragDropEffects.None)
        {
            if (_clearDragImageRect != _dragImageRect)
            {
                // To avoid flashing, we only want to clear the drag images if the DragImages is different than
                // the last time we got here. I.e. if we keep dragging over an area where we are not allowed to drop,
                // then we only have to clear the DragImages once.
                ClearAllDragImages();
                _clearDragImageRect = _dragImageRect;
            }

            _dragAssistanceManager?.EraseSnapLines();

            return;
        }

        bool createNewDragAssistance = false;
        Point mouseLoc = Control.MousePosition;
        bool altKeyPressed = Control.ModifierKeys == Keys.Alt;
        if (altKeyPressed && _dragAssistanceManager is not null)
        {
            // erase any snaplines (if we had any)
            _dragAssistanceManager.EraseSnapLines();
        }

        // I can't get rid of the ole-drag/drop default cursor that show's the cross-parent drag indication
        if (_data.Target.Equals(_data.Source) && _lastEffect != DragDropEffects.Copy)
        {
            e.UseDefaultCursors = false;
            Cursor.Current = Cursors.Default;
        }
        else
        {
            e.UseDefaultCursors = true;
        }

        // only do this drawing when the mouse pointer has actually moved so we don't continuously redraw and flicker like mad.
        Control target = _data.Target as Control;
        if ((mouseLoc != _lastFeedbackLocation) || (altKeyPressed && _dragAssistanceManager is not null))
        {
            if (!_data.Target.Equals(_lastDropTarget))
            {
                _serviceProviderTarget = target.Site;
                if (_serviceProviderTarget is null)
                {
                    return;
                }

                IDesignerHost newDestHost = (IDesignerHost)_serviceProviderTarget.GetService(typeof(IDesignerHost));
                if (newDestHost is null)
                {
                    return;
                }

                _targetAllowsSnapLines = true;
                // check to see if the current designer participate with SnapLines
                if (newDestHost.GetDesigner(target) is ControlDesigner designer && !designer.ParticipatesWithSnapLines)
                {
                    _targetAllowsSnapLines = false;
                }

                _statusCommandUITarget = new StatusCommandUI(_serviceProviderTarget);
                // Spin up new stuff if the host changes, or if this is the first time through (lastDropTarget will be null in this case)
                if ((_lastDropTarget is null) || (newDestHost != _destHost))
                {
                    if (_destHost is not null && _destHost != _srcHost)
                    {
                        // re-enable all glyphs in the old host... need to do this before we get the new behaviorservice
                        _behaviorServiceTarget.EnableAllAdorners(true);
                    }

                    _behaviorServiceTarget = (BehaviorService)_serviceProviderTarget.GetService(typeof(BehaviorService));
                    if (_behaviorServiceTarget is null)
                    {
                        return;
                    }

                    GetParentSnapInfo(target, _behaviorServiceTarget);

                    // Disable the adorners in the new host, but only if this is not the source host, since that will already have been done
                    if (newDestHost != _srcHost)
                    {
                        DisableAdorners(_serviceProviderTarget, _behaviorServiceTarget, true);
                    }

                    // clear the old drag images in the old graphicsTarget
                    ClearAllDragImages();

                    // Build a new dragImageRegion -- but only if we are changing hosts
                    if (_lastDropTarget is not null)
                    {
                        for (int i = 0; i < _dragObjects.Count; i++)
                        {
                            Control dragControl = (Control)_dragObjects[i];
                            Rectangle controlRect = _behaviorServiceSource.ControlRectInAdornerWindow(dragControl);
                            // Can't call MapPointFromSourceToTarget since we always want to do this
                            controlRect.Location = _behaviorServiceSource.AdornerWindowPointToScreen(controlRect.Location);
                            controlRect.Location = _behaviorServiceTarget.MapAdornerWindowPoint(IntPtr.Zero, controlRect.Location);
                            if (i == 0)
                            {
                                _dragImageRegion?.Dispose();

                                _dragImageRegion = new Region(controlRect);
                            }
                            else
                            {
                                _dragImageRegion.Union(controlRect);
                            }
                        }
                    }

                    _graphicsTarget?.Dispose();

                    _graphicsTarget = _behaviorServiceTarget.AdornerWindowGraphics;

                    // Always force the dragassistance manager to be created in this case.
                    createNewDragAssistance = true;
                    _destHost = newDestHost;
                }

                _lastDropTarget = _data.Target;
            }

            if (ShowHideDragControls(_lastEffect == DragDropEffects.Copy) && !createNewDragAssistance)
            {
                createNewDragAssistance = true;
            }

            // Create new dragassistancemanager if needed
            if (createNewDragAssistance && _behaviorServiceTarget.UseSnapLines)
            {
                // erase any snaplines (if we had any)
                _dragAssistanceManager?.EraseSnapLines();

                _dragAssistanceManager = new DragAssistanceManager(_serviceProviderTarget, _graphicsTarget, _dragObjects, null, _lastEffect == DragDropEffects.Copy);
            }

            // The new position of the primary control, i.e. where did we just drag it to
            Point newPosition = new(mouseLoc.X - _initialMouseLoc.X + _dragComponents[_primaryComponentIndex].originalControlLocation.X,
                                            mouseLoc.Y - _initialMouseLoc.Y + _dragComponents[_primaryComponentIndex].originalControlLocation.Y);
            // Map it to the target's adorner window so that we can snap correctly
            newPosition = MapPointFromSourceToTarget(newPosition);
            // The new rectangle
            Rectangle newRect = new(newPosition.X, newPosition.Y,
                                                _dragComponents[_primaryComponentIndex].dragImage.Width,
                                                _dragComponents[_primaryComponentIndex].dragImage.Height);
            // if we have a valid snapline engine - ask it to offset our drag
            if (_dragAssistanceManager is not null)
            {
                if (_targetAllowsSnapLines && !altKeyPressed)
                {
                    // Remembering the last snapOffset allows us to correctly erase snaplines,
                    // if the user subsequently holds down the Alt-Key. Remember that we don't physically move the mouse,
                    // we move the control (or rather the image of the control).
                    // So if we didn't remember the last snapOffset and the user then hit the Alt-Key,
                    // we would actually redraw the control at the actual mouse location,
                    // which would make the control "jump" which is not what the user would expect.
                    // Why does the control "jump"? Because when a control is snapped, we have offset the control
                    // relative to where the mouse is, but we have not update the physical mouse position.
                    // When the user hits the Alt-Key they expect the control to be where it was (whether snapped or not).
                    _lastSnapOffset = _dragAssistanceManager.OnMouseMove(newRect);
                }
                else
                {
                    _dragAssistanceManager.OnMouseMove(new Rectangle(-100, -100, 0, 0)); /*just an invalid rect - so we won't snap*/// );
                }
            }

            // if we know our parent is forcing grid sizes
            else if (!_parentGridSize.IsEmpty)
            {
                _lastSnapOffset = AdjustToGrid(newPosition);
            }

            // Set the new location after the drag (only need to do this for the primary control) adjusted for a snap offset
            newPosition.X += _lastSnapOffset.X;
            newPosition.Y += _lastSnapOffset.Y;

            // draggedLocation is the coordinates in the source AdornerWindow. Need to do this since our
            // original location is in those coordinates.
            _dragComponents[_primaryComponentIndex].draggedLocation = MapPointFromTargetToSource(newPosition);

            // Now draw the dragImage in the correct location
            // FIRST, INVALIDATE THE REGION THAT IS OUTSIDE OF THE DRAGIMAGERECT
            // First remember the old rect so that we can invalidate the right thing
            Rectangle previousImageRect = _dragImageRect;
            // This is in Source adorner window coordinates
            newPosition = new Point(mouseLoc.X - _initialMouseLoc.X + _originalDragImageLocation.X,
                                    mouseLoc.Y - _initialMouseLoc.Y + _originalDragImageLocation.Y);
            newPosition.X += _lastSnapOffset.X;
            newPosition.Y += _lastSnapOffset.Y;
            // Store this off in Source adornerwindow coordinates
            _dragImageRect.Location = newPosition;

            previousImageRect.Location = MapPointFromSourceToTarget(previousImageRect.Location);
            Rectangle newImageRect = _dragImageRect;
            newImageRect.Location = MapPointFromSourceToTarget(newImageRect.Location);

            Rectangle unionRectangle = Rectangle.Union(newImageRect, previousImageRect);
            Region invalidRegion = new(unionRectangle);
            invalidRegion.Exclude(newImageRect);

            // SECOND, INVALIDATE THE TRANSPARENT REGION OF THE DRAGIMAGERECT
            using (Region invalidDragRegion = _dragImageRegion.Clone())
            {
                invalidDragRegion.Translate(mouseLoc.X - _initialMouseLoc.X + _lastSnapOffset.X, mouseLoc.Y - _initialMouseLoc.Y + _lastSnapOffset.Y);
                invalidDragRegion.Complement(newImageRect);
                invalidDragRegion.Union(invalidRegion);
                _behaviorServiceTarget.Invalidate(invalidDragRegion);
            }

            invalidRegion.Dispose();
            if (_graphicsTarget is not null)
            {
                _graphicsTarget.SetClip(newImageRect);
                _graphicsTarget.DrawImage(_dragImage, newImageRect.X, newImageRect.Y);
                _graphicsTarget.ResetClip();
            }

            if (_dragComponents[_primaryComponentIndex].dragComponent is Control c)
            {
                // update drag position on the status bar
                Point dropPoint = _behaviorServiceSource.AdornerWindowPointToScreen(_dragComponents[_primaryComponentIndex].draggedLocation);
                dropPoint = target.PointToClient(dropPoint);
                // must adjust offsets for the flipped X axis when our container and control are mirrored
                if (target.IsMirrored && c.IsMirrored)
                {
                    dropPoint.Offset(-c.Width, 0);
                }

                _statusCommandUITarget?.SetStatusInformation(c, dropPoint);
            }

            // allow any snaplines to be drawn above our drag images as long as the alt key is not pressed and the mouse is over the root comp
            if (_dragAssistanceManager is not null && !altKeyPressed && _targetAllowsSnapLines)
            {
                _dragAssistanceManager.RenderSnapLinesInternal();
            }

            // save off the current mouse position
            _lastFeedbackLocation = mouseLoc;
        }

        _data.Target = null;
    }

    /// <summary>
    ///  We want to sort the dragComponents in descending z-order. We want to make sure that we draw the control
    ///  lowest in the z-order first, and drawing the control at the top of the z-order last. Remember that z-order
    ///  indices are in reverse order. I.e. the control that is at the top of the z-order list
    ///  has the lowest z-order index.
    /// </summary>
    int IComparer.Compare(object x, object y)
    {
        DragComponent dc1 = (DragComponent)x;
        DragComponent dc2 = (DragComponent)y;
        if (dc1.zorderIndex > dc2.zorderIndex)
        {
            return -1;
        }
        else if (dc1.zorderIndex < dc2.zorderIndex)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    private void GetParentSnapInfo(Control parentControl, BehaviorService bhvSvc)
    {
        // Clear out whatever value we might have had stored off
        _parentGridSize = Size.Empty;
        if (bhvSvc is not null && !bhvSvc.UseSnapLines)
        {
            PropertyDescriptor snapProp = TypeDescriptor.GetProperties(parentControl)["SnapToGrid"];
            if (snapProp is not null && (bool)snapProp.GetValue(parentControl))
            {
                PropertyDescriptor gridProp = TypeDescriptor.GetProperties(parentControl)["GridSize"];
                if (gridProp is not null)
                {
                    // cache of the gridsize and the location of the parent on the adornerwindow
                    if (_dragComponents[_primaryComponentIndex].dragComponent is Control)
                    {
                        _parentGridSize = (Size)gridProp.GetValue(parentControl);
                        _parentLocation = bhvSvc.MapAdornerWindowPoint(parentControl.Handle, Point.Empty);
                        if (parentControl.Parent is not null && parentControl.Parent.IsMirrored)
                        {
                            _parentLocation.Offset(-parentControl.Width, 0);
                        }
                    }
                }
            }
        }
    }

    private void DisableAdorners(IServiceProvider serviceProvider, BehaviorService behaviorService, bool hostChange)
    {
        // find our body glyph adorner offered by the behavior service we don't want to disable the transparent body glyphs
        Adorner bodyGlyphAdorner = null;
        SelectionManager selMgr = (SelectionManager)serviceProvider.GetService(typeof(SelectionManager));
        if (selMgr is not null)
        {
            bodyGlyphAdorner = selMgr.BodyGlyphAdorner;
        }

        // disable all adorners except for body glyph adorner
        foreach (Adorner a in behaviorService.Adorners)
        {
            if (bodyGlyphAdorner is not null && a.Equals(bodyGlyphAdorner))
            {
                continue;
            }

            a.EnabledInternal = false;
        }

        behaviorService.Invalidate();

        if (hostChange)
        {
            selMgr.OnBeginDrag(new BehaviorDragDropEventArgs(_dragObjects));
        }
    }

    /// <summary>
    ///  Called when the ControlDesigner starts a drag operation. Here, all adorners are disabled,
    ///  screen shots of all related controls are taken, and the DragAssistanceManager (for SnapLines) is created.
    /// </summary>
    private void InitiateDrag(Point initialMouseLocation, ICollection<IComponent> dragComps)
    {
        _dragObjects = [.. dragComps];
        DisableAdorners(_serviceProviderSource, _behaviorServiceSource, false);
        Control primaryControl = _dragObjects[0] as Control;
        Control primaryParent = primaryControl?.Parent;
        Color backColor = primaryParent is not null ? primaryParent.BackColor : Color.Empty;
        _dragImageRect = Rectangle.Empty;
        _clearDragImageRect = Rectangle.Empty;
        _initialMouseLoc = initialMouseLocation;

        // loop through every control we need to drag, calculate the offsets and get a snapshot
        for (int i = 0; i < _dragObjects.Count; i++)
        {
            Control dragControl = (Control)_dragObjects[i];

            _dragComponents[i].dragComponent = _dragObjects[i];
            _dragComponents[i].positionOffset = new Point(dragControl.Location.X - primaryControl.Location.X,
                                            dragControl.Location.Y - primaryControl.Location.Y);
            Rectangle controlRect = _behaviorServiceSource.ControlRectInAdornerWindow(dragControl);
            if (_dragImageRect.IsEmpty)
            {
                _dragImageRect = controlRect;
                _dragImageRegion = new Region(controlRect);
            }
            else
            {
                _dragImageRect = Rectangle.Union(_dragImageRect, controlRect);
                _dragImageRegion.Union(controlRect);
            }

            // Initialize the dragged location to be the current position of the control
            _dragComponents[i].draggedLocation = controlRect.Location;
            _dragComponents[i].originalControlLocation = _dragComponents[i].draggedLocation;
            // take snapshot of each control
            DesignerUtils.GenerateSnapShot(dragControl, out _dragComponents[i].dragImage, i == 0 ? 2 : 1, 1, backColor);

            // The dragged components are not in any specific order. If they all share the same parent,
            // we will sort them by their index in that parent's control's collection to preserve correct Z-order.
            if (primaryParent is not null && _shareParent)
            {
                _dragComponents[i].zorderIndex = primaryParent.Controls.GetChildIndex(dragControl, false /*throwException*/);
                if (_dragComponents[i].zorderIndex == -1)
                {
                    _shareParent = false;
                }
            }
        }

        if (_shareParent)
        {
            Array.Sort(_dragComponents, this);
        }

        // Now that we are sorted, set the primaryComponentIndex...
        for (int i = 0; i < _dragComponents.Length; i++)
        {
            if (primaryControl.Equals(_dragComponents[i].dragComponent as Control))
            {
                _primaryComponentIndex = i;
                break;
            }
        }

        Debug.Assert(_primaryComponentIndex != -1, "primaryComponentIndex was not set!");
        // suspend layout of the parent
        if (primaryParent is not null)
        {
            _suspendedParent = primaryParent;
            _suspendedParent.SuspendLayout();
            // Get the parent's grid settings here
            GetParentSnapInfo(_suspendedParent, _behaviorServiceSource);
        }

        // If the thing that's being dragged is of 0 size, make the image a little bigger
        // so that the user can see where they're dragging it.
        int imageWidth = _dragImageRect.Width;
        if (imageWidth == 0)
        {
            imageWidth = 1;
        }

        int imageHeight = _dragImageRect.Height;
        if (imageHeight == 0)
        {
            imageHeight = 1;
        }

        _dragImage = new Bitmap(imageWidth, imageHeight, Drawing.Imaging.PixelFormat.Format32bppPArgb);
        using (Graphics g = Graphics.FromImage(_dragImage))
        {
            g.Clear(Color.Chartreuse);
        }

        ((Bitmap)_dragImage).MakeTransparent(Color.Chartreuse);
        // Gotta use 2 using's here... Too bad.
        // Draw each control into the dragimage
        using (Graphics g = Graphics.FromImage(_dragImage))
        {
            using SolidBrush brush = new(primaryControl.BackColor);
            for (int i = 0; i < _dragComponents.Length; i++)
            {
                Rectangle controlRect = new(_dragComponents[i].draggedLocation.X - _dragImageRect.X,
                                          _dragComponents[i].draggedLocation.Y - _dragImageRect.Y,
                                          _dragComponents[i].dragImage.Width, _dragComponents[i].dragImage.Height);
                // The background
                g.FillRectangle(brush, controlRect);
                // The foreground
                g.DrawImage(_dragComponents[i].dragImage, controlRect,
                            new Rectangle(0, 0, _dragComponents[i].dragImage.Width, _dragComponents[i].dragImage.Height),
                            GraphicsUnit.Pixel);
            }
        }

        _originalDragImageLocation = new Point(_dragImageRect.X, _dragImageRect.Y);
        // hide actual controls - this might cause a brief flicker, we are okay with that.
        ShowHideDragControls(false);
        _cleanedUpDrag = false;
    }

    internal List<IComponent> GetSortedDragControls(out int primaryControlIndex)
    {
        // create our list of controls-to-drag
        List<IComponent> dragControls = [];
        primaryControlIndex = -1;
        if ((_dragComponents is not null) && (_dragComponents.Length > 0))
        {
            primaryControlIndex = _primaryComponentIndex;
            for (int i = 0; i < _dragComponents.Length; i++)
            {
                dragControls.Add(_dragComponents[i].dragComponent);
            }
        }

        return dragControls;
    }

    /// <summary>
    ///  Called by the BehaviorService in response to QueryContinueDrag notifications.
    /// </summary>
    internal void QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
    {
        // Clean up if the action was cancelled, or we had no effect when dropped.
        // Otherwise EndDragDrop() will do this after the locations have been properly changed.
        if (_behaviorServiceSource is not null && _behaviorServiceSource.CancelDrag)
        {
            e.Action = DragAction.Cancel;
            CleanupDrag(true);
            return;
        }

        if (e.Action == DragAction.Continue)
        {
            return;
        }

        // Clean up if the action was cancelled, or we had no effect when dropped. Otherwise EndDragDrop() will do this
        // after the locations have been properly changed.
        if (e.Action == DragAction.Cancel || _lastEffect == DragDropEffects.None)
        {
            CleanupDrag(true);
            // QueryContinueDrag can be called before GiveFeedback in which case we will end up here because
            // lastEffect == DragDropEffects.None. If we don't set e.Action, the drag will continue,
            // and GiveFeedback will be called. But since we have cleaned up the drag, weird things happens
            // (e.g. dragImageRegion has been disposed already, so we throw). So if we get here,
            // let's make sure and cancel the drag.
            e.Action = DragAction.Cancel;
        }
    }

    /// <summary>
    ///  Changes the Visible state of the controls we are dragging. Returns whether we change state or not.
    /// </summary>
    internal bool ShowHideDragControls(bool show)
    {
        if (_currentShowState == show)
        {
            return false;
        }

        _currentShowState = show;
        if (_dragComponents is not null)
        {
            for (int i = 0; i < _dragComponents.Length; i++)
            {
                if (_dragComponents[i].dragComponent is Control c)
                {
                    c.Visible = show;
                }
            }
        }

        return true;
    }

    internal void CleanupDrag()
    {
        CleanupDrag(true);
    }

    internal void CleanupDrag(bool clearImages)
    {
        if (!_cleanedUpDrag)
        {
            if (clearImages)
            {
                ClearAllDragImages();
            }

            ShowHideDragControls(true);
            try
            {
                _suspendedParent?.ResumeLayout();
            }

            finally
            {
                _suspendedParent = null;
                // re-enable all glyphs in all adorners
                _behaviorServiceSource.EnableAllAdorners(true);
                if (_destHost != _srcHost && _destHost is not null)
                {
                    _behaviorServiceTarget.EnableAllAdorners(true);
                    _behaviorServiceTarget.SyncSelection();
                }

                // Layout may have caused controls to resize, which would mean their BodyGlyphs are wrong. We need to sync these.
                _behaviorServiceSource?.SyncSelection();

                if (_dragImageRegion is not null)
                {
                    _dragImageRegion.Dispose();
                    _dragImageRegion = null;
                }

                if (_dragImage is not null)
                {
                    _dragImage.Dispose();
                    _dragImage = null;
                }

                if (_dragComponents is not null)
                {
                    for (int i = 0; i < _dragComponents.Length; i++)
                    {
                        if (_dragComponents[i].dragImage is not null)
                        {
                            _dragComponents[i].dragImage.Dispose();
                            _dragComponents[i].dragImage = null;
                        }
                    }
                }

                if (_graphicsTarget is not null)
                {
                    _graphicsTarget.Dispose();
                    _graphicsTarget = null;
                }

                _cleanedUpDrag = true;
            }
        }
    }
}
