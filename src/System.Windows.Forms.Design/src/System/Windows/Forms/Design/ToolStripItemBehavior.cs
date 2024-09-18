// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms.Design.Behavior;

namespace System.Windows.Forms.Design;

/// <summary>
///  The behavior for the glyph that covers the items themselves. This selects the items when they are clicked,
///  and will (when implemented) do the dragging/reordering of them.
/// </summary>
internal class ToolStripItemBehavior : Behavior.Behavior
{
    private const int GLYPHBORDER = 1;
    private const int GLYPHINSET = 2;
    // new privates for "Drag Drop"
    internal Rectangle _dragBoxFromMouseDown = Rectangle.Empty;
    private Timer _timer;
    private ToolStripItemGlyph _selectedGlyph;
    private bool _doubleClickFired;
    private bool _mouseUpFired;
    private Control _dropSource;
    private IEventHandlerService _eventSvc;

    public ToolStripItemBehavior()
    {
    }

    // Gets the DropSource control.
    private Control DropSource
    {
        get
        {
            _dropSource ??= new Control();

            return _dropSource;
        }
    }

    // Returns true if oldSelection and newSelection have a commonParent.
    private static bool CommonParent(ToolStripItem oldSelection, ToolStripItem newSelection)
    {
        ToolStrip oldSelectionParent = oldSelection.GetCurrentParent();
        ToolStrip newSelectionParent = newSelection.GetCurrentParent();
        return (oldSelectionParent == newSelectionParent);
    }

    /// <summary>
    ///  Clears the insertion mark when items are being reordered
    /// </summary>
    private static void ClearInsertionMark(ToolStripItem item)
    {
        // Don't paint if cursor hasnt moved.
        if (ToolStripDesigner.s_lastCursorPosition != Point.Empty && ToolStripDesigner.s_lastCursorPosition == Cursor.Position)
        {
            return;
        }

        // Don't paint any "MouseOver" glyphs if TemplateNode is ACTIVE !
        ToolStripKeyboardHandlingService keyService = GetKeyBoardHandlingService(item);
        if (keyService is not null && keyService.TemplateNodeActive)
        {
            return;
        }

        // stuff away the lastInsertionMarkRect and clear it out _before_ we call paint OW the call to invalidate
        // won't help as it will get repainted.
        if (item is not null && item.Site is not null)
        {
            IDesignerHost designerHost = (IDesignerHost)item.Site.GetService(typeof(IDesignerHost));
            if (designerHost is not null)
            {
                Rectangle bounds = GetPaintingBounds(designerHost, item);
                bounds.Inflate(GLYPHBORDER, GLYPHBORDER);
                Region rgn = new(bounds);
                try
                {
                    bounds.Inflate(-GLYPHINSET, -GLYPHINSET);
                    rgn.Exclude(bounds);
                    BehaviorService bSvc = GetBehaviorService(item);
                    if (bSvc is not null && bounds != Rectangle.Empty)
                    {
                        bSvc.Invalidate(rgn);
                    }
                }
                finally
                {
                    rgn.Dispose();
                }
            }
        }
    }

    // Tries to put the item in the InSitu edit mode after the double click timer has ticked
    private static void EnterInSituMode(ToolStripItemGlyph glyph)
    {
        if (glyph.ItemDesigner is not null && !glyph.ItemDesigner.IsEditorActive)
        {
            glyph.ItemDesigner.ShowEditNode(false);
        }
    }

    // Gets the Selection Service.
    private static ISelectionService GetSelectionService(ToolStripItem item)
    {
        Debug.Assert(item is not null, "Item passed is null, SelectionService cannot be obtained");
        if (item.Site is not null)
        {
            ISelectionService selSvc = (ISelectionService)item.Site.GetService(typeof(ISelectionService));
            Debug.Assert(selSvc is not null, "Failed to get Selection Service!");
            return selSvc;
        }

        return null;
    }

    // Gets the Behavior Service.
    private static BehaviorService GetBehaviorService(ToolStripItem item)
    {
        Debug.Assert(item is not null, "Item passed is null, BehaviorService cannot be obtained");
        if (item.Site is not null)
        {
            BehaviorService behaviorSvc = (BehaviorService)item.Site.GetService(typeof(BehaviorService));
            Debug.Assert(behaviorSvc is not null, "Failed to get Behavior Service!");
            return behaviorSvc;
        }

        return null;
    }

    // Gets the ToolStripKeyBoardHandling Service.
    private static ToolStripKeyboardHandlingService GetKeyBoardHandlingService(ToolStripItem item)
    {
        Debug.Assert(item is not null, "Item passed is null, ToolStripKeyBoardHandlingService cannot be obtained");
        if (item.Site is not null)
        {
            ToolStripKeyboardHandlingService keyBoardSvc = (ToolStripKeyboardHandlingService)item.Site.GetService(typeof(ToolStripKeyboardHandlingService));
            Debug.Assert(keyBoardSvc is not null, "Failed to get ToolStripKeyboardHandlingService!");
            return keyBoardSvc;
        }

        return null;
    }

    // Gets the painting rect for SelectionRects
    private static Rectangle GetPaintingBounds(IDesignerHost designerHost, ToolStripItem item)
    {
        Rectangle bounds = Rectangle.Empty;
        if (designerHost.GetDesigner(item) is ToolStripItemDesigner itemDesigner)
        {
            bounds = itemDesigner.GetGlyphBounds();
            ToolStripDesignerUtils.GetAdjustedBounds(item, ref bounds);
            // So that the mouseOver glyph matches the selectionGlyph.
            bounds.Inflate(1, 1);
            bounds.Width--;
            bounds.Height--;
        }

        return bounds;
    }

    // This helper function will return true if any other MouseHandler (say TabOrder UI) is active,
    // in which case we should not handle any Mouse Messages.. Since the TabOrder UI is pre-Whidbey
    // when the TabOrder UI is up, it adds a new Overlay (a window) to the DesignerFrame
    // (something similar to AdornerWindow). This UI is a transparent control which has overrides for Mouse Messages.
    // It listens for all mouse messages through the IMouseHandler interface instead of using the new BehaviorService.
    // Hence we have to special case this scenario. (CONTROL DESIGNER ALSO DOES THIS).
    private bool MouseHandlerPresent(ToolStripItem item)
    {
        IMouseHandler mouseHandler = null;
        _eventSvc ??= (IEventHandlerService)item.Site.GetService(typeof(IEventHandlerService));

        if (_eventSvc is not null)
        {
            mouseHandler = (IMouseHandler)_eventSvc.GetHandler(typeof(IMouseHandler));
        }

        return (mouseHandler is not null);
    }

    // Occurs when the timer ticks after user has double clicked an item
    private void OnDoubleClickTimerTick(object sender, EventArgs e)
    {
        if (_timer is not null)
        {
            _timer.Enabled = false;
            _timer.Tick -= OnDoubleClickTimerTick;
            _timer.Dispose();
            _timer = null;

            // Enter InSitu
            if (_selectedGlyph is not null && _selectedGlyph.Item is ToolStripMenuItem)
            {
                EnterInSituMode(_selectedGlyph);
            }
        }
    }

    // Occurs when user doubleclicks on the TooLStripItem glyph
    public override bool OnMouseDoubleClick(Glyph g, MouseButtons button, Point mouseLoc)
    {
        if (_mouseUpFired)
        {
            _doubleClickFired = true;
        }

        return false;
    }

    // Occurs when MouseUp TooLStripItem glyph
    public override bool OnMouseUp(Glyph g, MouseButtons button)
    {
        ToolStripItemGlyph glyph = g as ToolStripItemGlyph;
        ToolStripItem glyphItem = glyph.Item;
        if (MouseHandlerPresent(glyphItem))
        {
            return false;
        }

        SetParentDesignerValuesForDragDrop(glyphItem, false, Point.Empty);
        if (_doubleClickFired)
        {
            if (glyph is not null && button == MouseButtons.Left)
            {
                ISelectionService selSvc = GetSelectionService(glyphItem);
                if (selSvc is null)
                {
                    return false;
                }

                ToolStripItem selectedItem = selSvc.PrimarySelection as ToolStripItem;
                // Check if this item is already selected ...
                if (selectedItem == glyphItem)
                {
                    // If timer != null.. we are in DoubleClick before the "InSitu Timer" so KILL IT.
                    if (_timer is not null)
                    {
                        _timer.Enabled = false;
                        _timer.Tick -= OnDoubleClickTimerTick;
                        _timer.Dispose();
                        _timer = null;
                    }

                    // If the Selecteditem is already in editmode ... bail out
                    if (selectedItem is not null)
                    {
                        ToolStripItemDesigner selectedItemDesigner = glyph.ItemDesigner;
                        if (selectedItemDesigner is not null && selectedItemDesigner.IsEditorActive)
                        {
                            return false;
                        }

                        selectedItemDesigner.DoDefaultAction();
                    }

                    _doubleClickFired = false;
                    _mouseUpFired = false;
                }
            }
        }
        else
        {
            _mouseUpFired = true;
        }

        return false;
    }

    // Occurs when MouseDown on the TooLStripItem glyph
    public override bool OnMouseDown(Glyph g, MouseButtons button, Point mouseLoc)
    {
        ToolStripItemGlyph glyph = g as ToolStripItemGlyph;
        ToolStripItem glyphItem = glyph.Item;
        ISelectionService selSvc = GetSelectionService(glyphItem);
        BehaviorService bSvc = GetBehaviorService(glyphItem);
        ToolStripKeyboardHandlingService keyService = GetKeyBoardHandlingService(glyphItem);
        if ((button == MouseButtons.Left) && (keyService is not null) && (keyService.TemplateNodeActive))
        {
            if (keyService.ActiveTemplateNode.IsSystemContextMenuDisplayed)
            {
                // skip behaviors when the context menu is displayed
                return false;
            }
        }

        IDesignerHost designerHost = (IDesignerHost)glyphItem.Site.GetService(typeof(IDesignerHost));
        Debug.Assert(designerHost is not null, "Invalid DesignerHost");

        // Cache original selection
        ICollection originalSelComps = null;
        if (selSvc is not null)
        {
            originalSelComps = selSvc.GetSelectedComponents();
        }

        // Add the TemplateNode to the Selection if it is currently Selected as the GetSelectedComponents won't do it for us.
        ArrayList origSel = new(originalSelComps);
        if (origSel.Count == 0)
        {
            if (keyService is not null && keyService.SelectedDesignerControl is not null)
            {
                origSel.Add(keyService.SelectedDesignerControl);
            }
        }

        if (keyService is not null)
        {
            keyService.SelectedDesignerControl = null;
            if (keyService.TemplateNodeActive)
            {
                // If templateNode Active .. commit and Select it
                keyService.ActiveTemplateNode.CommitAndSelect();
                // if the selected item is clicked .. then commit the node and reset the selection (refer 488002)
                if (selSvc.PrimarySelection is ToolStripItem currentSel && currentSel == glyphItem)
                {
                    selSvc.SetSelectedComponents(null, SelectionTypes.Replace);
                }
            }
        }

        if (selSvc is null || MouseHandlerPresent(glyphItem))
        {
            return false;
        }

        if (glyph is not null && button == MouseButtons.Left)
        {
            ToolStripItem selectedItem = selSvc.PrimarySelection as ToolStripItem;
            // Always set the Drag-Rect for Drag-Drop...
            SetParentDesignerValuesForDragDrop(glyphItem, true, mouseLoc);
            // Check if this item is already selected ...
            if (selectedItem is not null && selectedItem == glyphItem)
            {
                // If the Selecteditem is already in editmode ... bail out
                if (selectedItem is not null)
                {
                    ToolStripItemDesigner selectedItemDesigner = glyph.ItemDesigner;
                    if (selectedItemDesigner is not null && selectedItemDesigner.IsEditorActive)
                    {
                        return false;
                    }
                }

                // Check if this is CTRL + Click or SHIFT + Click, if so then just remove the selection
                bool removeSel = (Control.ModifierKeys & (Keys.Control | Keys.Shift)) > 0;
                if (removeSel)
                {
                    selSvc.SetSelectedComponents(new IComponent[] { selectedItem }, SelectionTypes.Remove);
                    return false;
                }

                // start Double Click Timer
                // This is required for the second down in selection which can be the first down of a
                // Double click on the glyph confusing... hence this comment ...
                // Here's the scenario ....
                // DOWN 1 - selects the ITEM
                // DOWN 2 - ITEM goes into INSITU....
                // DOUBLE CLICK - don't show code..
                // Open INSITU after the double click time
                if (selectedItem is ToolStripMenuItem)
                {
                    _timer = new Timer
                    {
                        Interval = SystemInformation.DoubleClickTime
                    };

                    _timer.Tick += OnDoubleClickTimerTick;
                    _timer.Enabled = true;
                    _selectedGlyph = glyph;
                }
            }
            else
            {
                bool shiftPressed = (Control.ModifierKeys & Keys.Shift) > 0;
                // We should process MouseDown only if we are not yet selected....
                if (!selSvc.GetComponentSelected(glyphItem))
                {
                    // Reset the State... On the Glyphs .. we get MouseDown - Mouse UP (for single Click) And we get
                    // MouseDown - MouseUp - DoubleClick - Up (for double Click) Hence reset the state at start....
                    _mouseUpFired = false;
                    _doubleClickFired = false;
                    // Implementing Shift + Click....
                    // we have 2 items, namely, selectedItem (current PrimarySelection) and
                    // glyphItem (item which has received mouseDown) FIRST check if they have common parent...
                    // IF YES then get the indices of the two and SELECT all items from LOWER index to the HIGHER index.
                    if (shiftPressed && (selectedItem is not null && CommonParent(selectedItem, glyphItem)))
                    {
                        ToolStrip parent = null;
                        if (glyphItem.IsOnOverflow)
                        {
                            parent = glyphItem.Owner;
                        }
                        else
                        {
                            parent = glyphItem.GetCurrentParent();
                        }

                        int startIndexOfSelection = Math.Min(parent.Items.IndexOf(selectedItem), parent.Items.IndexOf(glyphItem));
                        int endIndexOfSelection = Math.Max(parent.Items.IndexOf(selectedItem), parent.Items.IndexOf(glyphItem));
                        int countofItemsSelected = (endIndexOfSelection - startIndexOfSelection) + 1;

                        // if two adjacent items are selected ...
                        if (countofItemsSelected == 2)
                        {
                            selSvc.SetSelectedComponents(new IComponent[] { glyphItem });
                        }
                        else
                        {
                            object[] totalObjects = new object[countofItemsSelected];
                            int j = 0;
                            for (int i = startIndexOfSelection; i <= endIndexOfSelection; i++)
                            {
                                totalObjects[j++] = parent.Items[i];
                            }

                            selSvc.SetSelectedComponents(new IComponent[] { parent }, SelectionTypes.Replace);
                            ToolStripDesigner.s_shiftState = true;
                            selSvc.SetSelectedComponents(totalObjects, SelectionTypes.Replace);
                        }
                    }

                    // End Implementation
                    else
                    {
                        if (glyphItem.IsOnDropDown && ToolStripDesigner.s_shiftState)
                        {
                            // Invalidate glyph only if we are in ShiftState...
                            ToolStripDesigner.s_shiftState = false;
                            bSvc?.Invalidate(glyphItem.Owner.Bounds);
                        }

                        selSvc.SetSelectedComponents(new IComponent[] { glyphItem }, SelectionTypes.Auto);
                    }

                    // Set the appropriate object.
                    if (keyService is not null)
                    {
                        keyService.ShiftPrimaryItem = glyphItem;
                    }
                }

                // we are already selected and if shiftpressed...
                else if (shiftPressed || (Control.ModifierKeys & Keys.Control) > 0)
                {
                    selSvc.SetSelectedComponents(new IComponent[] { glyphItem }, SelectionTypes.Remove);
                }
            }
        }

        if (glyph is not null && button == MouseButtons.Right)
        {
            if (!selSvc.GetComponentSelected(glyphItem))
            {
                selSvc.SetSelectedComponents(new IComponent[] { glyphItem });
            }
        }

        // finally Invalidate all selections
        ToolStripDesignerUtils.InvalidateSelection(origSel, glyphItem, glyphItem.Site, false);
        return false;
    }

    /// <summary>
    ///  Overridden to paint the border on mouse enter.....
    /// </summary>
    public override bool OnMouseEnter(Glyph g)
    {
        if (g is ToolStripItemGlyph glyph)
        {
            ToolStripItem glyphItem = glyph.Item;
            if (MouseHandlerPresent(glyphItem))
            {
                return false;
            }

            ISelectionService selSvc = GetSelectionService(glyphItem);
            if (selSvc is not null)
            {
                if (!selSvc.GetComponentSelected(glyphItem))
                {
                    PaintInsertionMark(glyphItem);
                }
            }
        }

        return false;
    }

    /// <summary>
    ///  Overridden to "clear" the boundary-paint when the mouse leave the item
    /// </summary>
    public override bool OnMouseLeave(Glyph g)
    {
        if (g is ToolStripItemGlyph glyph)
        {
            ToolStripItem glyphItem = glyph.Item;
            if (MouseHandlerPresent(glyphItem))
            {
                return false;
            }

            ISelectionService selSvc = GetSelectionService(glyphItem);
            if (selSvc is not null)
            {
                if (!selSvc.GetComponentSelected(glyphItem))
                {
                    ClearInsertionMark(glyphItem);
                }
            }
        }

        return false;
    }

    /// <summary>
    ///  When any MouseMove message enters the BehaviorService's AdornerWindow (MouseMove, ncMouseMove) it is first
    ///  passed here, to the top-most Behavior in the BehaviorStack. Returning 'true' from this function signifies that
    ///  the Message was 'handled' by the Behavior and should not continue to be processed.
    /// </summary>
    public override bool OnMouseMove(Glyph g, MouseButtons button, Point mouseLoc)
    {
        bool retVal = false;
        ToolStripItemGlyph glyph = g as ToolStripItemGlyph;
        ToolStripItem glyphItem = glyph.Item;
        ISelectionService selSvc = GetSelectionService(glyphItem);
        if (selSvc is null || glyphItem.Site is null || MouseHandlerPresent(glyphItem))
        {
            return false;
        }

        if (!selSvc.GetComponentSelected(glyphItem))
        {
            PaintInsertionMark(glyphItem);
            retVal = false;
        }

        if (button == MouseButtons.Left && glyph is not null && glyph.ItemDesigner is not null && !glyph.ItemDesigner.IsEditorActive)
        {
            Rectangle dragBox = Rectangle.Empty;
            IDesignerHost designerHost = (IDesignerHost)glyphItem.Site.GetService(typeof(IDesignerHost));
            Debug.Assert(designerHost is not null, "Invalid DesignerHost");
            if (glyphItem.Placement == ToolStripItemPlacement.Overflow || (glyphItem.Placement == ToolStripItemPlacement.Main && !(glyphItem.IsOnDropDown)))
            {
                ToolStripItemDesigner itemDesigner = glyph.ItemDesigner;
                ToolStrip parentToolStrip = itemDesigner.GetMainToolStrip();
                if (designerHost.GetDesigner(parentToolStrip) is ToolStripDesigner parentDesigner)
                {
                    dragBox = parentDesigner.DragBoxFromMouseDown;
                }
            }
            else if (glyphItem.IsOnDropDown)
            {
                // Get the OwnerItem's Designer and set the value...
                if (glyphItem.Owner is ToolStripDropDown parentDropDown)
                {
                    ToolStripItem ownerItem = parentDropDown.OwnerItem;
                    if (designerHost.GetDesigner(ownerItem) is ToolStripItemDesigner ownerItemDesigner)
                    {
                        dragBox = ownerItemDesigner._dragBoxFromMouseDown;
                    }
                }
            }

            // If the mouse moves outside the rectangle, start the drag.
            if (dragBox != Rectangle.Empty && !dragBox.Contains(mouseLoc.X, mouseLoc.Y))
            {
                if (_timer is not null)
                {
                    _timer.Enabled = false;
                    _timer.Tick -= OnDoubleClickTimerTick;
                    _timer.Dispose();
                    _timer = null;
                }

                // Proceed with the drag and drop, passing in the list item.
                try
                {
                    List<ToolStripItem> dragItems = [];
                    ICollection selComps = selSvc.GetSelectedComponents();

                    // Create our list of controls-to-drag
                    foreach (IComponent comp in selComps)
                    {
                        if (comp is ToolStripItem item)
                        {
                            dragItems.Add(item);
                        }
                    }

                    // Start Drag-Drop only if ToolStripItem is the primary Selection
                    if (selSvc.PrimarySelection is ToolStripItem selectedItem)
                    {
                        ToolStrip owner = selectedItem.Owner;
                        ToolStripItemDataObject data = new(dragItems, selectedItem, owner);
                        DropSource.QueryContinueDrag += QueryContinueDrag;

                        if (glyphItem is ToolStripDropDownItem ddItem)
                        {
                            if (designerHost.GetDesigner(ddItem) is ToolStripMenuItemDesigner itemDesigner)
                            {
                                itemDesigner.InitializeBodyGlyphsForItems(false, ddItem);
                                ddItem.HideDropDown();
                            }
                        }
                        else if (glyphItem.IsOnDropDown && !glyphItem.IsOnOverflow)
                        {
                            ToolStripDropDown dropDown = glyphItem.GetCurrentParent() as ToolStripDropDown;
                            ToolStripDropDownItem ownerItem = dropDown.OwnerItem as ToolStripDropDownItem;
                            selSvc.SetSelectedComponents(new IComponent[] { ownerItem }, SelectionTypes.Replace);
                        }

                        DropSource.DoDragDrop(data, DragDropEffects.All);
                    }
                }
                finally
                {
                    DropSource.QueryContinueDrag -= QueryContinueDrag;

                    // Reset all Drag-Variables
                    SetParentDesignerValuesForDragDrop(glyphItem, false, Point.Empty);
                    ToolStripDesigner.s_dragItem = null;
                    _dropSource = null;
                }

                retVal = false;
            }
        }

        return retVal;
    }

    // OLE DragDrop virtual methods
    /// <summary>
    ///  OnDragDrop can be overridden so that a Behavior can specify its own Drag/Drop rules.
    /// </summary>
    public override void OnDragDrop(Glyph g, DragEventArgs e)
    {
        ToolStripItem currentDropItem = ToolStripDesigner.s_dragItem;
        // Ensure that the list item index is contained in the data.
        if (e.Data is ToolStripItemDataObject data && currentDropItem is not null)
        {
            // Get the PrimarySelection before the Drag operation...
            ToolStripItem selectedItem = data.PrimarySelection;
            IDesignerHost designerHost = (IDesignerHost)currentDropItem.Site.GetService(typeof(IDesignerHost));
            Debug.Assert(designerHost is not null, "Invalid DesignerHost");
            // Do DragDrop only if currentDropItem has changed.
            if (currentDropItem != selectedItem && designerHost is not null)
            {
                List<ToolStripItem> dragComponents = data.DragComponents;
                ToolStrip parentToolStrip = currentDropItem.GetCurrentParent();
                int primaryIndex = -1;
                string transDesc;
                bool copy = (e.Effect == DragDropEffects.Copy);
                if (dragComponents.Count == 1)
                {
                    string name = TypeDescriptor.GetComponentName(dragComponents[0]);
                    if (name is null || name.Length == 0)
                    {
                        name = dragComponents[0].GetType().Name;
                    }

                    transDesc = string.Format(copy ? SR.BehaviorServiceCopyControl : SR.BehaviorServiceMoveControl, name);
                }
                else
                {
                    transDesc = string.Format(copy ? SR.BehaviorServiceCopyControls : SR.BehaviorServiceMoveControls, dragComponents.Count);
                }

                DesignerTransaction designerTransaction = designerHost.CreateTransaction(transDesc);
                try
                {
                    if (currentDropItem.Site.TryGetService(out IComponentChangeService changeService))
                    {
                        if (parentToolStrip is ToolStripDropDown dropDown)
                        {
                            ToolStripItem ownerItem = dropDown.OwnerItem;
                            changeService.OnComponentChanging(ownerItem, TypeDescriptor.GetProperties(ownerItem)["DropDownItems"]);
                        }
                        else
                        {
                            changeService.OnComponentChanging(parentToolStrip, TypeDescriptor.GetProperties(parentToolStrip)["Items"]);
                        }
                    }

                    IReadOnlyList<IComponent> components;

                    // If we are copying, then we want to make a copy of the components we are dragging
                    if (copy)
                    {
                        // Remember the primary selection if we had one
                        if (selectedItem is not null)
                        {
                            primaryIndex = dragComponents.IndexOf(selectedItem);
                        }

                        ToolStripKeyboardHandlingService keyboardHandlingService = GetKeyBoardHandlingService(selectedItem);
                        if (keyboardHandlingService is not null)
                        {
                            keyboardHandlingService.CopyInProgress = true;
                        }

                        components = DesignerUtils.CopyDragObjects(dragComponents, currentDropItem.Site);
                        if (keyboardHandlingService is not null)
                        {
                            keyboardHandlingService.CopyInProgress = false;
                        }

                        if (primaryIndex != -1)
                        {
                            selectedItem = components[primaryIndex] as ToolStripItem;
                        }
                    }
                    else
                    {
                        components = dragComponents;
                    }

                    if (e.Effect == DragDropEffects.Move || copy)
                    {
                        ISelectionService selSvc = GetSelectionService(currentDropItem);
                        if (selSvc is not null)
                        {
                            // Insert the item.
                            if (parentToolStrip is ToolStripOverflow overflow)
                            {
                                parentToolStrip = overflow.OwnerItem.Owner;
                            }

                            int indexOfItemUnderMouseToDrop = parentToolStrip.Items.IndexOf(ToolStripDesigner.s_dragItem);
                            if (indexOfItemUnderMouseToDrop != -1)
                            {
                                int indexOfPrimarySelection = 0;
                                if (selectedItem is not null)
                                {
                                    indexOfPrimarySelection = parentToolStrip.Items.IndexOf(selectedItem);
                                }

                                if (indexOfPrimarySelection != -1 && indexOfItemUnderMouseToDrop > indexOfPrimarySelection)
                                {
                                    indexOfItemUnderMouseToDrop--;
                                }

                                foreach (ToolStripItem item in components)
                                {
                                    parentToolStrip.Items.Insert(indexOfItemUnderMouseToDrop, item);
                                }
                            }

                            selSvc.SetSelectedComponents(new IComponent[] { selectedItem }, SelectionTypes.Primary | SelectionTypes.Replace);
                        }
                    }

                    if (changeService is not null)
                    {
                        ToolStripDropDown dropDown = parentToolStrip as ToolStripDropDown;
                        if (dropDown is not null)
                        {
                            ToolStripItem ownerItem = dropDown.OwnerItem;
                            changeService.OnComponentChanged(ownerItem, TypeDescriptor.GetProperties(ownerItem)["DropDownItems"]);
                        }
                        else
                        {
                            changeService.OnComponentChanged(parentToolStrip, TypeDescriptor.GetProperties(parentToolStrip)["Items"]);
                        }

                        // fire extra changing/changed events.
                        if (copy)
                        {
                            if (dropDown is not null)
                            {
                                ToolStripItem ownerItem = dropDown.OwnerItem;
                                changeService.OnComponentChanging(ownerItem, TypeDescriptor.GetProperties(ownerItem)["DropDownItems"]);
                                changeService.OnComponentChanged(ownerItem, TypeDescriptor.GetProperties(ownerItem)["DropDownItems"]);
                            }
                            else
                            {
                                changeService.OnComponentChanging(parentToolStrip, TypeDescriptor.GetProperties(parentToolStrip)["Items"]);
                                changeService.OnComponentChanged(parentToolStrip, TypeDescriptor.GetProperties(parentToolStrip)["Items"]);
                            }
                        }
                    }

                    // If Parent is DropDown... we have to manage the Glyphs ....
                    foreach (ToolStripItem item in components)
                    {
                        if (item is ToolStripDropDownItem)
                        {
                            if (designerHost.GetDesigner(item) is ToolStripMenuItemDesigner itemDesigner)
                            {
                                itemDesigner.InitializeDropDown();
                            }
                        }

                        if (item.GetCurrentParent() is ToolStripDropDown dropDown && !(dropDown is ToolStripOverflow))
                        {
                            if (dropDown.OwnerItem is ToolStripDropDownItem ownerItem)
                            {
                                if (designerHost.GetDesigner(ownerItem) is ToolStripMenuItemDesigner ownerDesigner)
                                {
                                    ownerDesigner.InitializeBodyGlyphsForItems(false, ownerItem);
                                    ownerDesigner.InitializeBodyGlyphsForItems(true, ownerItem);
                                }
                            }
                        }
                    }

                    // Refresh on SelectionManager...
                    BehaviorService bSvc = GetBehaviorService(currentDropItem);
                    bSvc?.SyncSelection();
                }
                catch (Exception ex)
                {
                    if (designerTransaction is not null)
                    {
                        designerTransaction.Cancel();
                        designerTransaction = null;
                    }

                    if (ex.IsCriticalException())
                    {
                        throw;
                    }
                }
                finally
                {
                    designerTransaction?.Commit();
                }
            }
        }
    }

    /// <summary>
    ///  OnDragEnter can be overridden so that a Behavior can specify its own Drag/Drop rules.
    /// </summary>
    public override void OnDragEnter(Glyph g, DragEventArgs e)
    {
        ToolStripItemGlyph glyph = g as ToolStripItemGlyph;
        ToolStripItem glyphItem = glyph.Item;
        if (e.Data is ToolStripItemDataObject data)
        {
            // support move only within same container.
            if (data.Owner == glyphItem.Owner)
            {
                PaintInsertionMark(glyphItem);
                ToolStripDesigner.s_dragItem = glyphItem;
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }
        else
        {
            e.Effect = DragDropEffects.None;
        }
    }

    /// <summary>
    ///  OnDragLeave can be overridden so that a Behavior can specify its own Drag/Drop rules.
    /// </summary>
    public override void OnDragLeave(Glyph g, EventArgs e)
    {
        ToolStripItemGlyph glyph = g as ToolStripItemGlyph;
        ClearInsertionMark(glyph.Item);
    }

    /// <summary>
    ///  OnDragOver can be overridden so that a Behavior can specify its own Drag/Drop rules.
    /// </summary>
    public override void OnDragOver(Glyph g, DragEventArgs e)
    {
        // Determine whether string data exists in the drop data. If not, then the drop effect reflects that the drop cannot occur.
        ToolStripItemGlyph glyph = g as ToolStripItemGlyph;
        ToolStripItem glyphItem = glyph.Item;
        if (e.Data is ToolStripItemDataObject)
        {
            PaintInsertionMark(glyphItem);
            e.Effect = (Control.ModifierKeys == Keys.Control) ? DragDropEffects.Copy : DragDropEffects.Move;
        }
        else
        {
            e.Effect = DragDropEffects.None;
        }
    }

    /// <summary>
    ///  Paints the insertion mark when items are being reordered
    /// </summary>
    private static void PaintInsertionMark(ToolStripItem item)
    {
        // Don't paint if cursor hasnt moved.
        if (ToolStripDesigner.s_lastCursorPosition != Point.Empty && ToolStripDesigner.s_lastCursorPosition == Cursor.Position)
        {
            return;
        }

        // Don't paint any "MouseOver" glyphs if TemplateNode is ACTIVE !
        ToolStripKeyboardHandlingService keyService = GetKeyBoardHandlingService(item);
        if (keyService is not null && keyService.TemplateNodeActive)
        {
            return;
        }

        // Start from fresh State...
        if (item is not null && item.Site is not null)
        {
            ToolStripDesigner.s_lastCursorPosition = Cursor.Position;
            IDesignerHost designerHost = (IDesignerHost)item.Site.GetService(typeof(IDesignerHost));
            if (designerHost is not null)
            {
                Rectangle bounds = GetPaintingBounds(designerHost, item);
                BehaviorService bSvc = GetBehaviorService(item);
                if (bSvc is not null)
                {
                    Graphics g = bSvc.AdornerWindowGraphics;
                    try
                    {
                        using Pen p = new(new SolidBrush(Color.Black));
                        p.DashStyle = DashStyle.Dot;
                        g.DrawRectangle(p, bounds);
                    }
                    finally
                    {
                        g.Dispose();
                    }
                }
            }
        }
    }

    /// <summary>
    ///  QueryContinueDrag can be overridden so that a Behavior can specify its own Drag/Drop rules.
    /// </summary>
    private void QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
    {
        // Cancel the drag if the mouse moves off the form.
        if (e.Action == DragAction.Continue)
        {
            return;
        }

        if (e.EscapePressed)
        {
            e.Action = DragAction.Cancel;
            ToolStripItem item = sender as ToolStripItem;
            SetParentDesignerValuesForDragDrop(item, false, Point.Empty);
            ISelectionService selSvc = GetSelectionService(item);
            selSvc?.SetSelectedComponents(new IComponent[] { item }, SelectionTypes.Auto);

            ToolStripDesigner.s_dragItem = null;
        }
    }

    // Set values before initiating the Drag-Drop
    private void SetParentDesignerValuesForDragDrop(ToolStripItem glyphItem, bool setValues, Point mouseLoc)
    {
        if (glyphItem.Site is null)
        {
            return;
        }

        // Remember the point where the mouse down occurred. The DragSize indicates the size that the
        // mouse can move before a drag event should be started.
        Size dragSize = new(1, 1);

        IDesignerHost designerHost = (IDesignerHost)glyphItem.Site.GetService(typeof(IDesignerHost));
        Debug.Assert(designerHost is not null, "Invalid DesignerHost");

        // implement Drag Drop for individual ToolStrip Items While this item is getting selected..
        // Get the index of the item the mouse is below.
        if (glyphItem.Placement == ToolStripItemPlacement.Overflow || (glyphItem.Placement == ToolStripItemPlacement.Main && !(glyphItem.IsOnDropDown)))
        {
            ToolStripItemDesigner itemDesigner = designerHost.GetDesigner(glyphItem) as ToolStripItemDesigner;
            ToolStrip parentToolStrip = itemDesigner.GetMainToolStrip();
            if (designerHost.GetDesigner(parentToolStrip) is ToolStripDesigner parentDesigner)
            {
                if (setValues)
                {
                    parentDesigner.IndexOfItemUnderMouseToDrag = parentToolStrip.Items.IndexOf(glyphItem);
                    // Create a rectangle using the DragSize, with the mouse position being at the center
                    // of the rectangle. On SelectionChanged we recreate the Glyphs ...
                    // so need to stash this value on the parentDesigner....
                    parentDesigner.DragBoxFromMouseDown = _dragBoxFromMouseDown = new Rectangle(new Point(mouseLoc.X - (dragSize.Width / 2), mouseLoc.Y - (dragSize.Height / 2)), dragSize);
                }
                else
                {
                    parentDesigner.IndexOfItemUnderMouseToDrag = -1;
                    parentDesigner.DragBoxFromMouseDown = _dragBoxFromMouseDown = Rectangle.Empty;
                }
            }
        }
        else if (glyphItem.IsOnDropDown)
        {
            // Get the OwnerItem's Designer and set the value...
            if (glyphItem.Owner is ToolStripDropDown parentDropDown)
            {
                ToolStripItem ownerItem = parentDropDown.OwnerItem;
                if (designerHost.GetDesigner(ownerItem) is ToolStripItemDesigner ownerItemDesigner)
                {
                    if (setValues)
                    {
                        ownerItemDesigner._indexOfItemUnderMouseToDrag = parentDropDown.Items.IndexOf(glyphItem);
                        // Create a rectangle using the DragSize, with the mouse position being at the center
                        // of the rectangle. On SelectionChanged we recreate the Glyphs ... so need to stash
                        // this value on the parentDesigner....
                        ownerItemDesigner._dragBoxFromMouseDown = _dragBoxFromMouseDown = new Rectangle(new Point(mouseLoc.X - (dragSize.Width / 2), mouseLoc.Y - (dragSize.Height / 2)), dragSize);
                    }
                    else
                    {
                        ownerItemDesigner._indexOfItemUnderMouseToDrag = -1;
                        ownerItemDesigner._dragBoxFromMouseDown = _dragBoxFromMouseDown = Rectangle.Empty;
                    }
                }
            }
        }
    }
}
