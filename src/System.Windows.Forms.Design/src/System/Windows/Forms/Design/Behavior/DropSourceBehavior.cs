// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;

namespace System.Windows.Forms.Design.Behavior
{
    /// <summary>
    ///  The DropSourceBehavior is created by ControlDesigner when it detects that  a drag operation has started.  This object is passed to the BehaviorService and is used to route GiveFeedback and QueryContinueDrag drag/drop messages. In response to GiveFeedback messages, this class will render the dragging controls in real-time with the help of the DragAssistanceManager (Snaplines) object or by simply snapping to grid dots.
    /// </summary>
    internal sealed class DropSourceBehavior : Behavior, IComparer
    {
        private struct DragComponent
        {
            public object dragComponent; //the dragComponent
            public int zorderIndex; //the dragComponent's z-order index
            public Point originalControlLocation; //the original control of the control in AdornerWindow coordinates
            public Point draggedLocation; //the location of the component after each drag - in AdornerWindow coordinates
            public Image dragImage; //bitblt'd image of control
            public Point positionOffset; //control position offset from primary selection
        };

        private readonly DragComponent[] dragComponents;
        private ArrayList dragObjects; // used to initialize the DragAssistanceManager
        private readonly BehaviorDataObject data;//drag data that represents the controls we're dragging & the effect/action
        private readonly DragDropEffects allowedEffects;//initial allowed effects for the drag operation
        private DragDropEffects lastEffect;//the last effect we saw (used for determining a valid drop)

        private bool targetAllowsSnapLines;//indicates if the drop target allows snaplines (flowpanels don't for ex)
        private IComponent lastDropTarget;//indicates the drop target on the last 'give feedback' event
        private Point lastSnapOffset;//the last snapoffset we used.
        // These 2 could be different (e.g. if dropping between forms)
        private readonly BehaviorService behaviorServiceSource;//ptr back to the BehaviorService in the drop source
        private BehaviorService behaviorServiceTarget;//ptr back to the BehaviorService in the drop target

        //this object will integrate SnapLines into the drag
        private DragAssistanceManager dragAssistanceManager;

        private Graphics graphicsTarget;//graphics object of the adornerwindows (via BehaviorService) in drop target

        private readonly IServiceProvider serviceProviderSource;
        private IServiceProvider serviceProviderTarget;

        private Point initialMouseLoc;//original mouse location in screen coordinates

        private Image dragImage;//A single image of the controls we are actually dragging around
        private Rectangle dragImageRect;//Rectangle of the dragImage -- in SOURCE AdornerWindow coordinates
        private Rectangle clearDragImageRect; //Rectangle used to remember the last dragimage rect we cleared
        private Point originalDragImageLocation; //original location of the drag image
        private Region dragImageRegion;

        private Point lastFeedbackLocation; // the last position we got feedback at
        private Control suspendedParent;//pointer to the parent that we suspended @ the beginning of the drag
        private Size parentGridSize; //used to snap around to grid dots if layoutmode == SnapToGrid
        private Point parentLocation;//location of parent on AdornerWindow - used for grid snap calculations
        private bool shareParent = true;//do dragged components share the parent
        private bool cleanedUpDrag;
        private StatusCommandUI statusCommandUITarget;// UI for setting the StatusBar Information in the drop target

        private readonly IDesignerHost srcHost;
        private IDesignerHost destHost;

        private bool currentShowState = true; // Initially the controls are showing

        private int primaryComponentIndex = -1; // Index of the primary component (control) in dragComponents

        /// <summary>
        ///  Constuctor that caches all needed vars for perf reasons.
        /// </summary>
        internal DropSourceBehavior(ICollection dragComponents, Control source, Point initialMouseLocation)
        {
            serviceProviderSource = source.Site as IServiceProvider;
            if (serviceProviderSource is null)
            {
                Debug.Fail("DragBehavior could not be created because the source ServiceProvider was not found");
                return;
            }

            behaviorServiceSource = (BehaviorService)serviceProviderSource.GetService(typeof(BehaviorService));
            if (behaviorServiceSource is null)
            {
                Debug.Fail("DragBehavior could not be created because the BehaviorService was not found");
                return;
            }

            if (dragComponents is null || dragComponents.Count <= 0)
            {
                Debug.Fail("There are no component to drag!");
                return;
            }

            srcHost = (IDesignerHost)serviceProviderSource.GetService(typeof(IDesignerHost));
            if (srcHost is null)
            {
                Debug.Fail("DragBehavior could not be created because the srcHost could not be found");
                return;
            }

            data = new BehaviorDataObject(dragComponents, source, this);
            allowedEffects = DragDropEffects.Copy | DragDropEffects.None | DragDropEffects.Move;
            this.dragComponents = new DragComponent[dragComponents.Count];
            parentGridSize = Size.Empty;

            lastEffect = DragDropEffects.None;
            lastFeedbackLocation = new Point(-1, -1);
            lastSnapOffset = Point.Empty;
            dragImageRect = Rectangle.Empty;
            clearDragImageRect = Rectangle.Empty;
            InitiateDrag(initialMouseLocation, dragComponents);
        }

        /// <summary>
        ///  This is the initial allowed Effect to start the drag operation with.
        /// </summary>
        internal DragDropEffects AllowedEffects
        {
            get => allowedEffects;
        }

        /// <summary>
        ///  This is the DataObject this DropSourceBehavior represents.
        /// </summary>
        internal DataObject DataObject
        {
            get => data;
        }

        /// <summary>
        ///  Here, during our drag operation, we need to determine the offset from the dragging control's position 'dragLoc' and the parent's grid. We'll return an offset for the image to 'snap to'.
        /// </summary>
        private Point AdjustToGrid(Point dragLoc)
        {
            //location of the drag with respect to the parent
            Point controlLocation = new Point(dragLoc.X - parentLocation.X, dragLoc.Y - parentLocation.Y);
            Point offset = Point.Empty;
            //determine which way we need to snap
            int xDelta = controlLocation.X % parentGridSize.Width;
            int yDelta = controlLocation.Y % parentGridSize.Height;
            // if we're more than half way to the next grid - then snap that way, otherwise snap back
            if (xDelta > parentGridSize.Width / 2)
            {
                offset.X = parentGridSize.Width - xDelta;
            }
            else
            {
                offset.X = -xDelta;
            }

            if (yDelta > parentGridSize.Height / 2)
            {
                offset.Y = parentGridSize.Height - yDelta;
            }
            else
            {
                offset.Y = -yDelta;
            }

            return offset;
        }

        private Point MapPointFromSourceToTarget(Point pt)
        {
            if (srcHost != destHost && destHost != null)
            {
                pt = behaviorServiceSource.AdornerWindowPointToScreen(pt);
                return behaviorServiceTarget.MapAdornerWindowPoint(IntPtr.Zero, pt);
            }
            else
            {
                return pt;
            }
        }

        private Point MapPointFromTargetToSource(Point pt)
        {
            if (srcHost != destHost && destHost != null)
            {
                pt = behaviorServiceTarget.AdornerWindowPointToScreen(pt);
                return behaviorServiceSource.MapAdornerWindowPoint(IntPtr.Zero, pt);
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
            if (dragImageRect != Rectangle.Empty)
            {
                Rectangle rect = dragImageRect;
                rect.Location = MapPointFromSourceToTarget(rect.Location);

                if (graphicsTarget != null)
                {
                    graphicsTarget.SetClip(rect);
                }

                if (behaviorServiceTarget != null)
                {
                    behaviorServiceTarget.Invalidate(rect);
                }

                if (graphicsTarget != null)
                {
                    graphicsTarget.ResetClip();
                }
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
            if (c.Site != null && !(c.Site is INestedSite) && destHost != null)
            {
                destHost.Container.Add(c);
            }
        }

        private void DropControl(int dragComponentIndex, Control dragTarget, Control dragSource, bool localDrag)
        {
            Control currentControl = dragComponents[dragComponentIndex].dragComponent as Control;
            if (lastEffect == DragDropEffects.Copy || (srcHost != destHost && destHost != null))
            {
                //between forms or copy
                currentControl.Visible = true;
                bool visibleState = true;
                PropertyDescriptor propLoc = TypeDescriptor.GetProperties(currentControl)["Visible"];
                if (propLoc != null)
                {
                    // store off the visible state. When adding the control to the new designer host, a new control designer will be created for the control. Since currentControl.Visible is currently FALSE (See InitiateDrag), the shadowed Visible property will be FALSE as well. This is not what we want.
                    visibleState = (bool)propLoc.GetValue(currentControl);
                }

                // Hook the control to its new designerHost
                SetDesignerHost(currentControl);
                currentControl.Parent = dragTarget;
                if (propLoc != null)
                {
                    //Make sure and set the Visible property to the correct value
                    propLoc.SetValue(currentControl, visibleState);
                }
            }
            else if (!localDrag && currentControl.Parent.Equals(dragSource))
            {
                //between containers
                dragSource.Controls.Remove(currentControl);
                currentControl.Visible = true;
                dragTarget.Controls.Add(currentControl);
            }
        }

        private void SetLocationPropertyAndChildIndex(int dragComponentIndex, Control dragTarget, Point dropPoint, int newIndex, bool allowSetChildIndexOnDrop)
        {
            PropertyDescriptor propLoc = TypeDescriptor.GetProperties(dragComponents[dragComponentIndex].dragComponent)["Location"];
            if ((propLoc != null) && (dragComponents[dragComponentIndex].dragComponent is Control currentControl))
            {
                // ControlDesigner shadows the Location property. If the control is parented and the parent is a scrollable control, then it expects the Location to be in displayrectangle coordinates. At this point bounds are in clientrectangle coordinates, so we need to check if we need to adjust the coordinates.
                Point pt = new Point(dropPoint.X, dropPoint.Y);
                if (currentControl.Parent is ScrollableControl p)
                {
                    Point ptScroll = p.AutoScrollPosition;
                    pt.Offset(-ptScroll.X, -ptScroll.Y); //always want to add the control below/right of the AutoScrollPosition
                }

                propLoc.SetValue(currentControl, pt);
                // In some cases the target designer wants to maintain its own ZOrder, in that case we shouldn't try and set the childindex. FlowLayoutPanelDesigner is one such case.
                if (allowSetChildIndexOnDrop)
                {
                    dragTarget.Controls.SetChildIndex(currentControl, newIndex);
                }
            }
        }

        /// <summary>
        ///  This is where we end the drag and commit the new control locations. To do this correctly, we loop through every control and find its propertyDescriptor for the Location.  Then call SetValue().  After this we re-enable the adorners.  Finally, we pop ourselves from the BehaviorStack.
        /// </summary>
        private void EndDragDrop(bool allowSetChildIndexOnDrop)
        {
            if (!(data.Target is Control dragTarget))
            {
                return; //can't deal with a non-control drop target yet
            }

            // If for some reason we couldn't get these guys, let's try and get them here
            if (serviceProviderTarget is null)
            {
                Debug.Fail("EndDragDrop - how can serviceProviderTarget be null?");
                serviceProviderTarget = dragTarget.Site as IServiceProvider;
                if (serviceProviderTarget is null)
                {
                    Debug.Fail("EndDragDrop - how can serviceProviderTarget be null?");
                    return;
                }
            }

            if (destHost is null)
            {
                Debug.Fail("EndDragDrop - how can destHost be null?");
                destHost = (IDesignerHost)serviceProviderTarget.GetService(typeof(IDesignerHost));
                if (destHost is null)
                {
                    Debug.Fail("EndDragDrop - how can destHost be null?");
                    return;
                }
            }

            if (behaviorServiceTarget is null)
            {
                Debug.Fail("EndDragDrop - how can behaviorServiceTarget be null?");
                behaviorServiceTarget = (BehaviorService)serviceProviderTarget.GetService(typeof(BehaviorService));
                if (behaviorServiceTarget is null)
                {
                    Debug.Fail("EndDragDrop - how can behaviorServiceTarget be null?");
                    return;
                }
            }

            // We use this list when doing a Drag-Copy, so that we can correctly restore state when we are done. See Copy code below.
            ArrayList originalControls = null;
            bool performCopy = (lastEffect == DragDropEffects.Copy);

            Control dragSource = data.Source;
            bool localDrag = dragSource.Equals(dragTarget);
            PropertyDescriptor targetProp = TypeDescriptor.GetProperties(dragTarget)["Controls"];
            PropertyDescriptor sourceProp = TypeDescriptor.GetProperties(dragSource)["Controls"];
            IComponentChangeService componentChangeSvcSource = (IComponentChangeService)serviceProviderSource.GetService(typeof(IComponentChangeService));
            IComponentChangeService componentChangeSvcTarget = (IComponentChangeService)serviceProviderTarget.GetService(typeof(IComponentChangeService));

            if (dragAssistanceManager != null)
            {
                dragAssistanceManager.OnMouseUp();
            }

            // If we are dropping between hosts, we want to set the selection in the new host to be the components that we are dropping. ... or if we are copying
            ISelectionService selSvc = null;
            if (performCopy || (srcHost != destHost && destHost != null))
            {
                selSvc = (ISelectionService)serviceProviderTarget.GetService(typeof(ISelectionService));
            }

            try
            {
                if (dragComponents != null && dragComponents.Length > 0)
                {
                    DesignerTransaction transSource = null;
                    DesignerTransaction transTarget = null;
                    string transDesc;
                    if (dragComponents.Length == 1)
                    {
                        string name = TypeDescriptor.GetComponentName(dragComponents[0].dragComponent);
                        if (name is null || name.Length == 0)
                        {
                            name = dragComponents[0].dragComponent.GetType().Name;
                        }
                        transDesc = string.Format(performCopy ? SR.BehaviorServiceCopyControl : SR.BehaviorServiceMoveControl, name);
                    }
                    else
                    {
                        transDesc = string.Format(performCopy ? SR.BehaviorServiceCopyControls : SR.BehaviorServiceMoveControls, dragComponents.Length);
                    }

                    // We don't want to create a transaction in the source, if we are doing a cross-form copy
                    if (srcHost != null && !(srcHost != destHost && destHost != null && performCopy))
                    {
                        transSource = srcHost.CreateTransaction(transDesc);
                    }

                    if (srcHost != destHost && destHost != null)
                    {
                        transTarget = destHost.CreateTransaction(transDesc);
                    }

                    try
                    {
                        ComponentTray tray = null;
                        int numberOfOriginalTrayControls = 0;
                        // If we are copying the controls, then, well, let's make a copy of'em... We then stuff the copy into the dragComponents array, since that keeps the rest of this code the same... No special casing needed.
                        if (performCopy)
                        {
                            // As part of a Ctrl-Drag, components might have been added to the component tray, make sure that their location gets updated as well (think ToolStrips). Get the current number of controls in the Component Tray in the target
                            tray = serviceProviderTarget.GetService(typeof(ComponentTray)) as ComponentTray;
                            numberOfOriginalTrayControls = tray != null ? tray.Controls.Count : 0;

                            // Get the objects to copy
                            ArrayList temp = new ArrayList();
                            for (int i = 0; i < dragComponents.Length; i++)
                            {
                                temp.Add(dragComponents[i].dragComponent);
                            }

                            // Create a copy of them
                            temp = DesignerUtils.CopyDragObjects(temp, serviceProviderTarget) as ArrayList;
                            if (temp is null)
                            {
                                Debug.Fail("Couldn't create copies of the controls we are dragging.");
                                return;
                            }

                            originalControls = new ArrayList();
                            // And stick the copied controls back into the dragComponents array
                            for (int j = 0; j < temp.Count; j++)
                            {
                                // ... but save off the old controls first
                                originalControls.Add(dragComponents[j].dragComponent);
                                dragComponents[j].dragComponent = temp[j];
                            }
                        }

                        if ((!localDrag || performCopy) && componentChangeSvcSource != null && componentChangeSvcTarget != null)
                        {
                            componentChangeSvcTarget.OnComponentChanging(dragTarget, targetProp);
                            // If we are performing a copy, then the dragSource will not change
                            if (!performCopy)
                            {
                                componentChangeSvcSource.OnComponentChanging(dragSource, sourceProp);
                            }
                        }

                        // We need to calculate initialDropPoint first to be able to calculate the new drop point for all controls Need to drop it first to make sure that the Parent gets set correctly.
                        DropControl(primaryComponentIndex, dragTarget, dragSource, localDrag);
                        Point initialDropPoint = behaviorServiceSource.AdornerWindowPointToScreen(dragComponents[primaryComponentIndex].draggedLocation);

                        // Tricky... initialDropPoint is the dropPoint in the source adornerwindow, which could be different than the target adornerwindow. But since we first convert it to screen coordinates, and then to client coordinates using the new parent, we end up dropping in the right spot. Cool, huh!
                        initialDropPoint = ((Control)dragComponents[primaryComponentIndex].dragComponent).Parent.PointToClient(initialDropPoint);

                        // Correct (only) the drop point for when Parent is mirrored, then use the offsets for the other controls, which were already corrected for mirroring in InitDrag
                        if (((Control)(dragComponents[primaryComponentIndex].dragComponent)).Parent.IsMirrored)
                        {
                            initialDropPoint.Offset(-((Control)(dragComponents[primaryComponentIndex].dragComponent)).Width, 0);
                        }

                        // check permission to do that
                        Control primaryComponent = dragComponents[primaryComponentIndex].dragComponent as Control;
                        PropertyDescriptor propLoc = TypeDescriptor.GetProperties(primaryComponent)["Location"];
                        if (primaryComponent != null && propLoc != null)
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
                        SetLocationPropertyAndChildIndex(primaryComponentIndex, dragTarget, initialDropPoint,
                                                            shareParent ? dragComponents[primaryComponentIndex].zorderIndex : 0, allowSetChildIndexOnDrop);
                        if (selSvc != null)
                        {
                            selSvc.SetSelectedComponents(new object[] { dragComponents[primaryComponentIndex].dragComponent }, SelectionTypes.Primary | SelectionTypes.Replace);
                        }

                        for (int i = 0; i < dragComponents.Length; i++)
                        {
                            if (i == primaryComponentIndex)
                            {
                                // did this one above
                                continue;
                            }

                            DropControl(i, dragTarget, dragSource, localDrag);
                            Point dropPoint = new Point(initialDropPoint.X + dragComponents[i].positionOffset.X,
                                                            initialDropPoint.Y + dragComponents[i].positionOffset.Y);
                            SetLocationPropertyAndChildIndex(i, dragTarget, dropPoint,
                                                                shareParent ? dragComponents[i].zorderIndex : 0, allowSetChildIndexOnDrop);
                            if (selSvc != null)
                            {
                                selSvc.SetSelectedComponents(new object[] { dragComponents[i].dragComponent }, SelectionTypes.Add);
                            }
                        }

                        if ((!localDrag || performCopy) && componentChangeSvcSource != null && componentChangeSvcTarget != null)
                        {
                            componentChangeSvcTarget.OnComponentChanged(dragTarget, targetProp, dragTarget.Controls, dragTarget.Controls);
                            if (!performCopy)
                            {
                                componentChangeSvcSource.OnComponentChanged(dragSource, sourceProp, dragSource.Controls, dragSource.Controls);
                            }
                        }

                        // If we did a Copy, then restore the old controls to make sure we set state correctly
                        if (originalControls != null)
                        {
                            for (int i = 0; i < originalControls.Count; i++)
                            {
                                dragComponents[i].dragComponent = originalControls[i];
                            }
                            originalControls = null;
                        }

                        // Rearrange the Component Tray - if we have to
                        if (performCopy)
                        {
                            if (tray is null)
                            {
                                // the target did not have a tray already, so let's go get it - if there is one
                                tray = serviceProviderTarget.GetService(typeof(ComponentTray)) as ComponentTray;
                            }

                            if (tray != null)
                            {
                                int numberOfTrayControlsAdded = tray.Controls.Count - numberOfOriginalTrayControls;

                                if (numberOfTrayControlsAdded > 0)
                                {
                                    ArrayList listOfTrayControls = new ArrayList();
                                    for (int i = 0; i < numberOfTrayControlsAdded; i++)
                                    {
                                        listOfTrayControls.Add(tray.Controls[numberOfOriginalTrayControls + i]);
                                    }
                                    tray.UpdatePastePositions(listOfTrayControls);
                                }
                            }
                        }

                        // We need to CleanupDrag BEFORE we commit the transaction.  The reason is that cleaning up can potentially cause a layout, and then any changes that happen due to the layout would be in a separate UndoUnit. We want the D&D to be undoable in one step.
                        CleanupDrag(false);
                        if (transSource != null)
                        {
                            transSource.Commit();
                            transSource = null;
                        }
                        if (transTarget != null)
                        {
                            transTarget.Commit();
                            transTarget = null;
                        }
                    }

                    finally
                    {
                        if (transSource != null)
                        {
                            transSource.Cancel();
                        }

                        if (transTarget != null)
                        {
                            transTarget.Cancel();
                        }
                    }
                }
            }
            finally
            {
                // If we did a Copy, then restore the old controls to make sure we set state correctly
                if (originalControls != null)
                {
                    for (int i = 0; i < originalControls.Count; i++)
                    {
                        dragComponents[i].dragComponent = originalControls[i];
                    }
                }

                // Even though we call CleanupDrag(false) twice (see above), this method guards against doing the wrong thing.
                CleanupDrag(false);
                if (statusCommandUITarget != null)
                {
                    // if selSvs is not null, then we either did a copy, or moved between forms, so use it to set the right info
                    statusCommandUITarget.SetStatusInformation(selSvc is null ? dragComponents[primaryComponentIndex].dragComponent as Component :
                                                                                selSvc.PrimarySelection as Component);
                }
            }

            // clear the last feedback loc
            lastFeedbackLocation = new Point(-1, -1);
        }

        /// <summary>
        ///  Called by the BehaviorService when the GiveFeedback event is fired. Here, we attempt to render all of our dragging control snapshots.  *After, of course, we let the DragAssistanceManager adjust the position due to any SnapLine activity.
        /// </summary>
        internal void GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            // cache off this last effect so in QueryContinueDrag we can identify (if dropped) a valid drop operation
            lastEffect = e.Effect;
            //if our target is null, we can't drop anywhere, so don't even draw images
            if (data.Target is null || e.Effect == DragDropEffects.None)
            {
                if (clearDragImageRect != dragImageRect)
                {
                    // To avoid flashing, we only want to clear the drag images if the the dragimagerect is different than the last time we got here. I.e. if we keep dragging over an area where we are  not allowed to  drop, then we only have to clear the dragimages once.
                    ClearAllDragImages();
                    clearDragImageRect = dragImageRect;
                }
                if (dragAssistanceManager != null)
                {
                    dragAssistanceManager.EraseSnapLines();
                }
                return;
            }

            bool createNewDragAssistance = false;
            Point mouseLoc = Control.MousePosition;
            bool altKeyPressed = Control.ModifierKeys == Keys.Alt;
            if (altKeyPressed && dragAssistanceManager != null)
            {
                //erase any snaplines (if we had any)
                dragAssistanceManager.EraseSnapLines();
            }

            // I can't get rid of the ole-drag/drop default cursor that show's the cross-parent drag indication
            if (data.Target.Equals(data.Source) && lastEffect != DragDropEffects.Copy)
            {
                e.UseDefaultCursors = false;
                Cursor.Current = Cursors.Default;
            }
            else
            {
                e.UseDefaultCursors = true;
            }

            // only do this drawing when the mouse pointer has actually moved so we don't continuously redraw and flicker like mad.
            Control target = data.Target as Control;
            if ((mouseLoc != lastFeedbackLocation) || (altKeyPressed && dragAssistanceManager != null))
            {
                if (!data.Target.Equals(lastDropTarget))
                {
                    serviceProviderTarget = target.Site as IServiceProvider;
                    if (serviceProviderTarget is null)
                    {
                        return;
                    }

                    IDesignerHost newDestHost = (IDesignerHost)serviceProviderTarget.GetService(typeof(IDesignerHost));
                    if (newDestHost is null)
                    {
                        return;
                    }

                    targetAllowsSnapLines = true;
                    //check to see if the current designer participate with SnapLines
                    if (newDestHost.GetDesigner(target) is ControlDesigner designer && !designer.ParticipatesWithSnapLines)
                    {
                        targetAllowsSnapLines = false;
                    }

                    statusCommandUITarget = new StatusCommandUI(serviceProviderTarget);
                    // Spin up new stuff if the host changes, or if this is the first time through (lastDropTarget will be null in this case)
                    if ((lastDropTarget is null) || (newDestHost != destHost))
                    {
                        if (destHost != null && destHost != srcHost)
                        {
                            // re-enable all glyphs in the old host... need to do this before we get the new behaviorservice
                            behaviorServiceTarget.EnableAllAdorners(true);
                        }

                        behaviorServiceTarget = (BehaviorService)serviceProviderTarget.GetService(typeof(BehaviorService));
                        if (behaviorServiceTarget is null)
                        {
                            return;
                        }

                        GetParentSnapInfo(target, behaviorServiceTarget);

                        // Disable the adorners in the new host, but only if this is not the source host, since that will already have been done
                        if (newDestHost != srcHost)
                        {
                            DisableAdorners(serviceProviderTarget, behaviorServiceTarget, true);
                        }

                        // clear the old drag images in the old graphicsTarget
                        ClearAllDragImages();

                        // Build a new dragImageRegion -- but only if we are changing hosts
                        if (lastDropTarget != null)
                        {
                            for (int i = 0; i < dragObjects.Count; i++)
                            {
                                Control dragControl = (Control)dragObjects[i];
                                Rectangle controlRect = behaviorServiceSource.ControlRectInAdornerWindow(dragControl);
                                // Can't call MapPointFromSourceToTarget since we always want to do this
                                controlRect.Location = behaviorServiceSource.AdornerWindowPointToScreen(controlRect.Location);
                                controlRect.Location = behaviorServiceTarget.MapAdornerWindowPoint(IntPtr.Zero, controlRect.Location);
                                if (i == 0)
                                {
                                    if (dragImageRegion != null)
                                    {
                                        dragImageRegion.Dispose();
                                    }
                                    dragImageRegion = new Region(controlRect);
                                }
                                else
                                {
                                    dragImageRegion.Union(controlRect);
                                }
                            }
                        }

                        if (graphicsTarget != null)
                        {
                            graphicsTarget.Dispose();
                        }
                        graphicsTarget = behaviorServiceTarget.AdornerWindowGraphics;

                        // Always force the dragassistance manager to be created in this case.
                        createNewDragAssistance = true;
                        destHost = newDestHost;
                    }
                    lastDropTarget = data.Target;
                }

                if (ShowHideDragControls(lastEffect == DragDropEffects.Copy) && !createNewDragAssistance)
                {
                    createNewDragAssistance = true;
                }

                // Create new dragassistancemanager if needed
                if (createNewDragAssistance && behaviorServiceTarget.UseSnapLines)
                {
                    if (dragAssistanceManager != null)
                    {
                        //erase any snaplines (if we had any)
                        dragAssistanceManager.EraseSnapLines();
                    }
                    dragAssistanceManager = new DragAssistanceManager(serviceProviderTarget, graphicsTarget, dragObjects, null, lastEffect == DragDropEffects.Copy);
                }

                //The new position of the primary control, i.e. where did we just drag it to
                Point newPosition = new Point(mouseLoc.X - initialMouseLoc.X + dragComponents[primaryComponentIndex].originalControlLocation.X,
                                                mouseLoc.Y - initialMouseLoc.Y + dragComponents[primaryComponentIndex].originalControlLocation.Y);
                // Map it to the target's adorner window so that we can snap correctly
                newPosition = MapPointFromSourceToTarget(newPosition);
                //The new rectangle
                Rectangle newRect = new Rectangle(newPosition.X, newPosition.Y,
                                                    dragComponents[primaryComponentIndex].dragImage.Width,
                                                    dragComponents[primaryComponentIndex].dragImage.Height);
                //if we have a valid snapline engine - ask it to offset our drag
                if (dragAssistanceManager != null)
                {
                    if (targetAllowsSnapLines && !altKeyPressed)
                    {
                        // Remembering the last snapoffset allows us to correctly erase snaplines, if the user subsequently holds down the Alt-Key. Remember that we don't physically move the mouse, we move the control (or rather the image of the control). So if we didn't remember the last snapoffset and the user then hit the Alt-Key, we would actually redraw the control at the actual mouse location, which would make the control "jump" which is not what the user would expect. Why does the control "jump"? Because when a control is snapped, we have offset the control relative to where the mouse is, but we have not update the physical mouse position. When the user hits the Alt-Key they expect the control to be where it was (whether snapped or not).
                        lastSnapOffset = dragAssistanceManager.OnMouseMove(newRect);
                    }
                    else
                    {
                        dragAssistanceManager.OnMouseMove(new Rectangle(-100, -100, 0, 0));/*just an invalid rect - so we won't snap*///);
                    }
                }
                //if we know our parent is forcing grid sizes
                else if (!parentGridSize.IsEmpty)
                {
                    lastSnapOffset = AdjustToGrid(newPosition);
                }

                // Set the new location after the drag (only need to do this for the primary control) adjusted for a snap offset
                newPosition.X += lastSnapOffset.X;
                newPosition.Y += lastSnapOffset.Y;

                // draggedLocation is the coordinates in the source AdornerWindow. Need to do this since our original location is in those coordinates
                dragComponents[primaryComponentIndex].draggedLocation = MapPointFromTargetToSource(newPosition);

                // Now draw the dragImage in the correct location
                // FIRST, INVALIDATE THE REGION THAT IS OUTSIDE OF THE DRAGIMAGERECT
                // First remember the old rect so that we can invalidate the right thing
                Rectangle previousImageRect = dragImageRect;
                // This is in Source adorner window coordinates
                newPosition = new Point(mouseLoc.X - initialMouseLoc.X + originalDragImageLocation.X,
                                        mouseLoc.Y - initialMouseLoc.Y + originalDragImageLocation.Y);
                newPosition.X += lastSnapOffset.X;
                newPosition.Y += lastSnapOffset.Y;
                // Store this off in Source adornerwindow coordinates
                dragImageRect.Location = newPosition;

                previousImageRect.Location = MapPointFromSourceToTarget(previousImageRect.Location);
                Rectangle newImageRect = dragImageRect;
                newImageRect.Location = MapPointFromSourceToTarget(newImageRect.Location);

                Rectangle unionRectangle = Rectangle.Union(newImageRect, previousImageRect);
                Region invalidRegion = new Region(unionRectangle);
                invalidRegion.Exclude(newImageRect);

                // SECOND, INVALIDATE THE TRANSPARENT REGION OF THE DRAGIMAGERECT
                using (Region invalidDragRegion = dragImageRegion.Clone())
                {
                    invalidDragRegion.Translate(mouseLoc.X - initialMouseLoc.X + lastSnapOffset.X, mouseLoc.Y - initialMouseLoc.Y + lastSnapOffset.Y);
                    invalidDragRegion.Complement(newImageRect);
                    invalidDragRegion.Union(invalidRegion);
                    behaviorServiceTarget.Invalidate(invalidDragRegion);
                }
                invalidRegion.Dispose();
                if (graphicsTarget != null)
                {
                    graphicsTarget.SetClip(newImageRect);
                    graphicsTarget.DrawImage(dragImage, newImageRect.X, newImageRect.Y);
                    graphicsTarget.ResetClip();
                }

                if (dragComponents[primaryComponentIndex].dragComponent is Control c)
                {
                    // update drag position on the status bar
                    Point dropPoint = behaviorServiceSource.AdornerWindowPointToScreen(dragComponents[primaryComponentIndex].draggedLocation);
                    dropPoint = target.PointToClient(dropPoint);
                    // must adjust offsets for the flipped X axis when our container and control are mirrored
                    if (target.IsMirrored && c.IsMirrored)
                    {
                        dropPoint.Offset(-c.Width, 0);
                    }
                    if (statusCommandUITarget != null)
                    {
                        statusCommandUITarget.SetStatusInformation(c as Component, dropPoint);
                    }
                }

                // allow any snaplines to be drawn above our drag images as long as the alt key is not pressed and the mouse is over the root comp
                if (dragAssistanceManager != null && !altKeyPressed && targetAllowsSnapLines)
                {
                    dragAssistanceManager.RenderSnapLinesInternal();
                }

                // save off the current mouse position
                lastFeedbackLocation = mouseLoc;
            }
            data.Target = null;
        }

        /// <summary>
        ///  We want to sort the dragComponents in descending z-order. We want to  make sure that we draw the control lowest in the z-order first, and drawing the control at the top of the z-order last. Remember that z-order indices are in reverse order. I.e. the control that is at the top of the z-order list has the lowest z-order index.
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
            parentGridSize = Size.Empty;
            if (bhvSvc != null && !bhvSvc.UseSnapLines)
            {
                PropertyDescriptor snapProp = TypeDescriptor.GetProperties(parentControl)["SnapToGrid"];
                if (snapProp != null && (bool)snapProp.GetValue(parentControl))
                {
                    PropertyDescriptor gridProp = TypeDescriptor.GetProperties(parentControl)["GridSize"];
                    if (gridProp != null)
                    {
                        //cache of the gridsize and the location of the parent on the adornerwindow
                        if (dragComponents[primaryComponentIndex].dragComponent is Control)
                        {
                            parentGridSize = (Size)gridProp.GetValue(parentControl);
                            parentLocation = bhvSvc.MapAdornerWindowPoint(parentControl.Handle, Point.Empty);
                            if (parentControl.Parent != null && parentControl.Parent.IsMirrored)
                            {
                                parentLocation.Offset(-parentControl.Width, 0);
                            }
                        }
                    }
                }
            }
        }

        private void DisableAdorners(IServiceProvider serviceProvider, BehaviorService behaviorService, bool hostChange)
        {
            // find our bodyglyph adorner offered by the behavior service we don't want to disable the transparent body glyphs
            Adorner bodyGlyphAdorner = null;
            SelectionManager selMgr = (SelectionManager)serviceProvider.GetService(typeof(SelectionManager));
            if (selMgr != null)
            {
                bodyGlyphAdorner = selMgr.BodyGlyphAdorner;
            }

            //disable all adorners except for bodyglyph adorner
            foreach (Adorner a in behaviorService.Adorners)
            {
                if (bodyGlyphAdorner != null && a.Equals(bodyGlyphAdorner))
                {
                    continue;
                }
                a.EnabledInternal = false;
            }
            behaviorService.Invalidate();

            if (hostChange)
            {
                selMgr.OnBeginDrag(new BehaviorDragDropEventArgs(dragObjects));
            }
        }

        /// <summary>
        ///  Called when the ControlDesigner starts a drag operation. Here, all adorners are disabled, screen shots of all related controls are taken, and the DragAssistanceManager  (for SnapLines) is created.
        /// </summary>
        private void InitiateDrag(Point initialMouseLocation, ICollection dragComps)
        {
            dragObjects = new ArrayList(dragComps);
            DisableAdorners(serviceProviderSource, behaviorServiceSource, false);
            Control primaryControl = dragObjects[0] as Control;
            Control primaryParent = primaryControl?.Parent;
            Color backColor = primaryParent != null ? primaryParent.BackColor : Color.Empty;
            dragImageRect = Rectangle.Empty;
            clearDragImageRect = Rectangle.Empty;
            initialMouseLoc = initialMouseLocation;

            //loop through every control we need to drag, calculate the offsets and get a snapshot
            for (int i = 0; i < dragObjects.Count; i++)
            {
                Control dragControl = (Control)dragObjects[i];

                dragComponents[i].dragComponent = dragObjects[i];
                dragComponents[i].positionOffset = new Point(dragControl.Location.X - primaryControl.Location.X,
                                                dragControl.Location.Y - primaryControl.Location.Y);
                Rectangle controlRect = behaviorServiceSource.ControlRectInAdornerWindow(dragControl);
                if (dragImageRect.IsEmpty)
                {
                    dragImageRect = controlRect;
                    dragImageRegion = new Region(controlRect);
                }
                else
                {
                    dragImageRect = Rectangle.Union(dragImageRect, controlRect);
                    dragImageRegion.Union(controlRect);
                }

                //Initialize the dragged location to be the current position of the control
                dragComponents[i].draggedLocation = controlRect.Location;
                dragComponents[i].originalControlLocation = dragComponents[i].draggedLocation;
                //take snapshot of each control
                DesignerUtils.GenerateSnapShot(dragControl, ref dragComponents[i].dragImage, i == 0 ? 2 : 1, 1, backColor);

                // The dragged components are not in any specific order. If they all share the same parent, we will sort them by their index in that parent's control's collection to preserve correct Z-order
                if (primaryParent != null && shareParent)
                {
                    dragComponents[i].zorderIndex = primaryParent.Controls.GetChildIndex(dragControl, false /*throwException*/);
                    if (dragComponents[i].zorderIndex == -1)
                    {
                        shareParent = false;
                    }
                }
            }
            if (shareParent)
            {
                Array.Sort(dragComponents, this);
            }

            // Now that we are sorted, set the primaryComponentIndex...
            for (int i = 0; i < dragComponents.Length; i++)
            {
                if (primaryControl.Equals(dragComponents[i].dragComponent as Control))
                {
                    primaryComponentIndex = i;
                    break;
                }
            }

            Debug.Assert(primaryComponentIndex != -1, "primaryComponentIndex was not set!");
            //suspend layout of the parent
            if (primaryParent != null)
            {
                suspendedParent = primaryParent;
                suspendedParent.SuspendLayout();
                // Get the parent's grid settings here
                GetParentSnapInfo(suspendedParent, behaviorServiceSource);
            }

            // If the thing that's being dragged is of 0 size, make the image a little  bigger so that the user can see where they're dragging it.
            int imageWidth = dragImageRect.Width;
            if (imageWidth == 0)
            {
                imageWidth = 1;
            }

            int imageHeight = dragImageRect.Height;
            if (imageHeight == 0)
            {
                imageHeight = 1;
            }

            dragImage = new Bitmap(imageWidth, imageHeight, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            using (Graphics g = Graphics.FromImage(dragImage))
            {
                g.Clear(Color.Chartreuse);
            }
            ((Bitmap)dragImage).MakeTransparent(Color.Chartreuse);
            // Gotta use 2 using's here... Too bad.
            // Draw each control into the dragimage
            using (Graphics g = Graphics.FromImage(dragImage))
            {
                using (SolidBrush brush = new SolidBrush(primaryControl.BackColor))
                {
                    for (int i = 0; i < dragComponents.Length; i++)
                    {
                        Rectangle controlRect = new Rectangle(dragComponents[i].draggedLocation.X - dragImageRect.X,
                                                  dragComponents[i].draggedLocation.Y - dragImageRect.Y,
                                                  dragComponents[i].dragImage.Width, dragComponents[i].dragImage.Height);
                        // The background
                        g.FillRectangle(brush, controlRect);
                        // The foreground
                        g.DrawImage(dragComponents[i].dragImage, controlRect,
                                    new Rectangle(0, 0, dragComponents[i].dragImage.Width, dragComponents[i].dragImage.Height),
                                    GraphicsUnit.Pixel);
                    }
                }
            }

            originalDragImageLocation = new Point(dragImageRect.X, dragImageRect.Y);
            //hide actual controls - this might cause a brief flicker, we are okay with that.
            ShowHideDragControls(false);
            cleanedUpDrag = false;
        }

        internal ArrayList GetSortedDragControls(ref int primaryControlIndex)
        {
            //create our list of controls-to-drag
            ArrayList dragControls = new ArrayList();
            primaryControlIndex = -1;
            if ((dragComponents != null) && (dragComponents.Length > 0))
            {
                primaryControlIndex = primaryComponentIndex;
                for (int i = 0; i < dragComponents.Length; i++)
                {
                    dragControls.Add(dragComponents[i].dragComponent);
                }
            }
            return dragControls;
        }

        /// <summary>
        ///  Called by the BehaviorService in response to QueryContinueDrag notifications.
        /// </summary>
        internal void QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            //Clean up if the action was cancelled, or we had no effect when dropped. Otherwise EndDragDrop() will do this after the locations have been properly changed.
            if (behaviorServiceSource != null && behaviorServiceSource.CancelDrag)
            {
                e.Action = DragAction.Cancel;
                CleanupDrag(true);
                return;
            }

            if (e.Action == DragAction.Continue)
            {
                return;
            }

            //Clean up if the action was cancelled, or we had no effect when dropped. Otherwise EndDragDrop() will do this after the locations have been properly changed.
            if (e.Action == DragAction.Cancel || lastEffect == DragDropEffects.None)
            {
                CleanupDrag(true);
                // QueryContinueDrag can be called before GiveFeedback in which case we will end up here because lastEffect == DragDropEffects.None. If we don't set e.Action, the drag will continue, and GiveFeedback will be called. But since we have cleaned up the drag, weird things happens (e.g. dragImageRegion has been disposed already, so we throw). So if we get here, let's make sure and cancel the drag.
                e.Action = DragAction.Cancel;
            }
        }

        /// <summary>
        ///  Changes the Visible state of the controls we are dragging. Returns whether we change state or not.
        /// </summary>
        internal bool ShowHideDragControls(bool show)
        {
            if (currentShowState == show)
            {
                return false;
            }

            currentShowState = show;
            if (dragComponents != null)
            {
                for (int i = 0; i < dragComponents.Length; i++)
                {
                    if (dragComponents[i].dragComponent is Control c)
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
            if (!cleanedUpDrag)
            {
                if (clearImages)
                {
                    ClearAllDragImages();
                }

                ShowHideDragControls(true);
                try
                {
                    if (suspendedParent != null)
                    {
                        suspendedParent.ResumeLayout();
                    }
                }

                finally
                {
                    suspendedParent = null;
                    //re-enable all glyphs in all adorners
                    behaviorServiceSource.EnableAllAdorners(true);
                    if (destHost != srcHost && destHost != null)
                    {
                        behaviorServiceTarget.EnableAllAdorners(true);
                        behaviorServiceTarget.SyncSelection();
                    }

                    // Layout may have caused controls to resize, which would mean their BodyGlyphs are wrong.  We need to sync these.
                    if (behaviorServiceSource != null)
                    {
                        behaviorServiceSource.SyncSelection();
                    }

                    if (dragImageRegion != null)
                    {
                        dragImageRegion.Dispose();
                        dragImageRegion = null;
                    }

                    if (dragImage != null)
                    {
                        dragImage.Dispose();
                        dragImage = null;
                    }

                    if (dragComponents != null)
                    {
                        for (int i = 0; i < dragComponents.Length; i++)
                        {
                            if (dragComponents[i].dragImage != null)
                            {
                                dragComponents[i].dragImage.Dispose();
                                dragComponents[i].dragImage = null;
                            }
                        }
                    }
                    if (graphicsTarget != null)
                    {
                        graphicsTarget.Dispose();
                        graphicsTarget = null;
                    }
                    cleanedUpDrag = true;
                }
            }
        }

        /// <summary>
        ///  This class extends from DataObject and carries additional  information such as: the list of Controls currently being dragged and the drag 'Source'.
        /// </summary>
        internal class BehaviorDataObject : DataObject
        {
            private readonly ICollection _dragComponents;
            private readonly Control _source;
            private IComponent _target;
            private readonly DropSourceBehavior _sourceBehavior;

            public BehaviorDataObject(ICollection dragComponents, Control source, DropSourceBehavior sourceBehavior) : base()
            {
                _dragComponents = dragComponents;
                _source = source;
                _sourceBehavior = sourceBehavior;
                _target = null;
            }

            public Control Source
            {
                get => _source;
            }

            public ICollection DragComponents
            {
                get => _dragComponents;
            }

            public IComponent Target
            {
                get => _target;
                set => _target = value;
            }

            internal void EndDragDrop(bool allowSetChildIndexOnDrop) => _sourceBehavior.EndDragDrop(allowSetChildIndexOnDrop);

            internal void CleanupDrag() => _sourceBehavior.CleanupDrag();

            internal ArrayList GetSortedDragControls(ref int primaryControlIndex) => _sourceBehavior.GetSortedDragControls(ref primaryControlIndex);
        }
    }
}
