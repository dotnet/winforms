// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms.Design.Behavior;

namespace System.Windows.Forms.Design;

internal class ToolStripKeyboardHandlingService
{
    private ISelectionService _selectionService;
    private IComponentChangeService _componentChangeService;
    private IServiceProvider _provider;
    private IMenuCommandService _menuCommandService;
    private readonly IDesignerHost _designerHost;
    // primary selection during shift operation is the LAST selected item which is different
    // from selSvc.PrimarySelection, hence cache it
    private object _shiftPrimary;
    private bool _shiftPressed;
    // our cache of currently selected DesignerToolStripControl Host....
    private object _currentSelection;
    // is the templateNode in InSitu Mode?
    private bool _templateNodeActive;
    private ToolStripTemplateNode _activeTemplateNode;
    // is the TemplateNode ContextMenu open. When the TemplateNode AddItems ContextMenu is
    // opened we want to Disable all the Commands... And we enable them when the contextMenu closes...
    // But if the menu closes by "enter Key" we get OnKeyDefault and hence go into InSitu Edit Mode..
    // to avoid this we have a new flag to IGNORE the first OnKeyDefault.
    private bool _templateNodeContextMenuOpen;
    // old commands
    private List<MenuCommand> _oldCommands;
    // our commands
    private List<MenuCommand> _newCommands;
    // need to add this separately since the VbDATA guys return us their paste command when the DataSource is copy pasted.
    private MenuCommand _oldCommandPaste;
    private MenuCommand _newCommandPaste;
    private bool _commandsAdded;
    private bool _copyInProgress;
    private bool _cutOrDeleteInProgress;
    private bool _contextMenuShownByKeyBoard; // We should know when the contextMenu is shown by KeyBoard shortcut.
    private object _ownerItemAfterCut; // This value is set only of the ToolStripMenuItem is cut and now we need to reopen the dropDown which was closed in the previous CUT operation.

    /// <summary>
    ///  This creates a service for handling the keyboard navigation at design time.
    /// </summary>
    public ToolStripKeyboardHandlingService(IServiceProvider serviceProvider)
    {
        _provider = serviceProvider;
        _selectionService = (ISelectionService)serviceProvider.GetService(typeof(ISelectionService));
        Debug.Assert(_selectionService is not null, "ToolStripKeyboardHandlingService relies on the selection service, which is unavailable.");
        if (_selectionService is not null)
        {
            _selectionService.SelectionChanging += OnSelectionChanging;
            _selectionService.SelectionChanged += OnSelectionChanged;
        }

        _designerHost = (IDesignerHost)_provider.GetService(typeof(IDesignerHost));
        Debug.Assert(_designerHost is not null, "ToolStripKeyboardHandlingService relies on the selection service, which is unavailable.");
        _designerHost?.AddService(this);

        _componentChangeService = (IComponentChangeService)_designerHost.GetService(typeof(IComponentChangeService));
        Debug.Assert(_componentChangeService is not null, "ToolStripKeyboardHandlingService relies on the componentChange service, which is unavailable.");
        if (_componentChangeService is not null)
        {
            _componentChangeService.ComponentRemoved += OnComponentRemoved;
        }
    }

    // Currently active TemplateNode
    internal ToolStripTemplateNode ActiveTemplateNode
    {
        get => _activeTemplateNode;
        set
        {
            _activeTemplateNode = value;
            ResetActiveTemplateNodeSelectionState();
        }
    }

    // This property is set on the controlDesigner and used in the ToolStripItemDesigner.
    // There is no way of knowing whether the ContextMenu is show via-keyBoard or Click and
    // we need to know this since we check if the Bounds are within the toolStripItem while showing the ContextMenu.
    internal bool ContextMenuShownByKeyBoard
    {
        get => _contextMenuShownByKeyBoard;
        set => _contextMenuShownByKeyBoard = value;
    }

    // When Copy (Through Control + Drag) this boolean is set to true.
    // Problem is that during copy the DesignerUtils creates new components and
    // as a result the ToolStripMenuItemDesigner and ToolStripDesigners get the "ComponentAdding/ComponentAdded" events
    // where they try to parent the components. We don't need to "parent" in case of control + drag.
    internal bool CopyInProgress
    {
        get => _copyInProgress;
        set
        {
            if (value != CopyInProgress)
            {
                _copyInProgress = value;
            }
        }
    }

    // We need to listen to MenuCommands.Delete since we are going to change the selection here instead
    // of OnComponentRemoved The OnComponentRemoved gets called through various different places like Partial Reload,
    // Full Reload and Undo-Redo transactions Changing the selection in "OnComponentRemoved"
    // thus is expensive in terms of flicker and code that gets run causing PERF hit.
    internal bool CutOrDeleteInProgress
    {
        get => _cutOrDeleteInProgress;
        set
        {
            if (value != _cutOrDeleteInProgress)
            {
                _cutOrDeleteInProgress = value;
            }
        }
    }

    /// <summary>
    ///  Retrieves the selection service, which this service uses while selecting the toolStrip Item.
    /// </summary>
    private IDesignerHost Host
    {
        get => _designerHost;
    }

    /// <summary>
    ///  Retrieves the menu editor service, which we cache for speed.
    /// </summary>
    private IMenuCommandService MenuService
    {
        get
        {
            if (_menuCommandService is null)
            {
                if (_provider is not null)
                {
                    _menuCommandService = (IMenuCommandService)_provider.GetService(typeof(IMenuCommandService));
                }
            }

            return _menuCommandService;
        }
    }

    // When the TemplateNode gets selected ... we don't set in the SelectionService.SelectedComponents
    // since we want to blank out the propertygrid ... so we keep the selected cache here.
    internal object SelectedDesignerControl
    {
        get => _currentSelection;
        set
        {
            if (value != SelectedDesignerControl)
            {
                if (SelectedDesignerControl is DesignerToolStripControlHost prevDesignerNode)
                {
                    prevDesignerNode.RefreshSelectionGlyph();
                }

                _currentSelection = value;
                if (_currentSelection is not null)
                {
                    if (_currentSelection is DesignerToolStripControlHost curDesignerNode)
                    {
                        curDesignerNode.SelectControl();
                        if (curDesignerNode.AccessibilityObject is ToolStripItem.ToolStripItemAccessibleObject acc)
                        {
                            acc.AddState(AccessibleStates.Selected | AccessibleStates.Focused);
                            ToolStrip owner = curDesignerNode.GetCurrentParent();
                            int focusIndex = 0;
                            if (owner is not null)
                            {
                                focusIndex = owner.Items.IndexOf(curDesignerNode);
                            }

                            PInvoke.NotifyWinEvent(
                                (uint)AccessibleEvents.SelectionAdd,
                                owner,
                                (int)OBJECT_IDENTIFIER.OBJID_CLIENT,
                                focusIndex + 1);
                            PInvoke.NotifyWinEvent(
                                (uint)AccessibleEvents.Focus,
                                owner,
                                (int)OBJECT_IDENTIFIER.OBJID_CLIENT,
                                focusIndex + 1);
                        }
                    }
                }
            }
        }
    }

    internal object OwnerItemAfterCut
    {
        get => _ownerItemAfterCut;
        set => _ownerItemAfterCut = value;
    }

    // When shift key is pressed we need to know where to start from .. this object keeps a track of that item.
    internal object ShiftPrimaryItem
    {
        get => _shiftPrimary;
        set => _shiftPrimary = value;
    }

    /// <summary>
    ///  Retrieves the selection service, which this service uses while selecting the toolStrip Item.
    /// </summary>
    private ISelectionService SelectionService
    {
        get => _selectionService;
    }

    // When the ToolStripTemplateNode becomes active, the ToolStripKeyBoardHandlingService shouldn't process any MenuCommands...
    internal bool TemplateNodeActive
    {
        get => _templateNodeActive;
        set
        {
            _templateNodeActive = value;

            // Disable all our Commands when TemplateNode is Active. Remove the new Commands
            if (_newCommands is not null)
            {
                foreach (MenuCommand newCommand in _newCommands)
                {
                    newCommand.Enabled = !_templateNodeActive;
                }
            }
        }
    }

    // boolean which returns if the TemplateNode contextMenu is open.
    internal bool TemplateNodeContextMenuOpen
    {
        get => _templateNodeContextMenuOpen;
        set
        {
            _templateNodeContextMenuOpen = value;
            // Disable all our Commands when templateNodeContextMenuOpen. Remove the new Commands
            if (_newCommands is not null)
            {
                foreach (MenuCommand newCommand in _newCommands)
                {
                    newCommand.Enabled = !_templateNodeActive;
                }
            }
        }
    }

    // Adds our commands to the MenuCommandService.
    public void AddCommands()
    {
        IMenuCommandService mcs = MenuService;
        if (mcs is not null & !_commandsAdded)
        {
            // Demand Create the oldCommands
            if (_oldCommands is null)
            {
                PopulateOldCommands();
            }

            // Remove the Old Commands
            foreach (MenuCommand oldCommand in _oldCommands)
            {
                if (oldCommand is not null)
                {
                    mcs.RemoveCommand(oldCommand);
                }
            }

            // DemandCreate the new Commands.
            if (_newCommands is null)
            {
                PopulateNewCommands();
            }

            // Add our Commands
            foreach (MenuCommand newCommand in _newCommands)
            {
                if (newCommand is not null && mcs.FindCommand(newCommand.CommandID) is null)
                {
                    mcs.AddCommand(newCommand);
                }
            }

            _commandsAdded = true;
        }
    }

    // private function to get Next toolStripItem based on direction.
    private static ToolStripItem GetNextItem(ToolStrip parent, ToolStripItem startItem, ArrowDirection direction)
    {
        if (parent.RightToLeft == RightToLeft.Yes && (direction == ArrowDirection.Left || direction == ArrowDirection.Right))
        {
            if (direction == ArrowDirection.Right)
            {
                direction = ArrowDirection.Left;
            }
            else if (direction == ArrowDirection.Left)
            {
                direction = ArrowDirection.Right;
            }
        }

        return parent.GetNextItem(startItem, direction);
    }

    /// <summary>
    ///  This is the private helper function which gets the next control in the TabOrder..
    /// </summary>
    private static Control GetNextControlInTab(Control basectl, Control ctl, bool forward)
    {
        if (forward)
        {
            while (ctl != basectl)
            {
                int targetIndex = ctl.TabIndex;
                bool hitCtl = false;
                Control found = null;
                Control p = ctl.Parent;
                // Cycle through the controls in z-order looking for the one with the next highest tab index.
                // Because there can be dups, we have to start with the existing tab index and remember
                // to exclude the current control.
                int parentControlCount = 0;
                Control.ControlCollection parentControls = p.Controls;
                if (parentControls is not null)
                {
                    parentControlCount = parentControls.Count;
                }

                for (int c = 0; c < parentControlCount; c++)
                {
                    // The logic for this is a bit lengthy, so I have broken it into separate clauses: We are not interested in ourself.
                    if (parentControls[c] != ctl)
                    {
                        // We are interested in controls with >= tab indexes to ctl.
                        // We must include those controls with equal indexes to account for duplicate indexes.
                        if (parentControls[c].TabIndex >= targetIndex)
                        {
                            // Check to see if this control replaces the "best match" we've already found.
                            if (found is null || found.TabIndex > parentControls[c].TabIndex)
                            {
                                // Finally, check to make sure that if this tab index is the same as ctl,
                                // that we've already encountered ctl in the z-order.
                                // If it isn't the same, than we're more than happy with it.
                                if ((parentControls[c].Site is not null && parentControls[c].TabIndex != targetIndex) || hitCtl)
                                {
                                    found = parentControls[c];
                                }
                            }
                        }
                    }
                    else
                    {
                        // We track when we have encountered "ctl". We never want to select ctl again, but we want
                        // to know when we've seen it in case we find another control with the same tab index.
                        hitCtl = true;
                    }
                }

                if (found is not null)
                {
                    return found;
                }

                ctl = ctl.Parent;
            }
        }
        else
        {
            if (ctl != basectl)
            {
                int targetIndex = ctl.TabIndex;
                bool hitCtl = false;
                Control found = null;
                Control p = ctl.Parent;
                // Cycle through the controls in reverse z-order looking for the next lowest tab index.
                // We must start with the same tab index as ctl, because there can be dups.
                int parentControlCount = 0;
                Control.ControlCollection parentControls = p.Controls;
                if (parentControls is not null)
                {
                    parentControlCount = parentControls.Count;
                }

                for (int c = parentControlCount - 1; c >= 0; c--)
                {
                    // The logic for this is a bit lengthy, so I have broken it into separate clauses: We are not interested in ourself.
                    if (parentControls[c] != ctl)
                    {
                        // We are interested in controls with <= tab indexes to ctl.
                        // We must include those controls with equal indexes to account for duplicate indexes.
                        if (parentControls[c].TabIndex <= targetIndex)
                        {
                            // Check to see if this control replaces the "best match" we've already found.
                            if (found is null || found.TabIndex < parentControls[c].TabIndex)
                            {
                                // Finally, check to make sure that if this tab index is the same as ctl,
                                // that we've already encountered ctl in the z-order.
                                // If it isn't the same, than we're more than happy with it.
                                if (parentControls[c].TabIndex != targetIndex || hitCtl)
                                {
                                    found = parentControls[c];
                                }
                            }
                        }
                    }
                    else
                    {
                        // We track when we have encountered "ctl". We never want to select ctl again, but we want
                        // to know when we've seen it in case we find another control with the same tab index.
                        hitCtl = true;
                    }
                }

                // If we were unable to find a control we should return the control's parent. However, if that parent is us, return NULL.
                if (found is not null)
                {
                    ctl = found;
                }
                else
                {
                    return p == basectl ? null : p;
                }
            }
        }

        return ctl == basectl ? null : ctl;
    }

    // this will invoke the OLD command from our command handler.
    private void InvokeOldCommand(object sender)
    {
        MenuCommand command = sender as MenuCommand;
        foreach (MenuCommand oldCommand in _oldCommands)
        {
            if (oldCommand is not null && oldCommand.CommandID == command.CommandID)
            {
                oldCommand.Invoke();
                break;
            }
        }
    }

    private void OnComponentRemoved(object sender, ComponentEventArgs e)
    {
        bool toolStripPresent = false;
        ComponentCollection comps = _designerHost.Container.Components;
        foreach (IComponent comp in comps)
        {
            if (comp is ToolStrip)
            {
                toolStripPresent = true;
                break;
            }
        }

        if (!toolStripPresent)
        {
            ToolStripKeyboardHandlingService keyboardHandlingService = (ToolStripKeyboardHandlingService)_provider.GetService(typeof(ToolStripKeyboardHandlingService));
            if (keyboardHandlingService is not null)
            {
                // since we are going away .. restore the old commands.
                keyboardHandlingService.RestoreCommands();
                // clean up.
                keyboardHandlingService.RemoveCommands();
                _designerHost.RemoveService<ToolStripKeyboardHandlingService>();
            }
        }
    }

    public bool OnContextMenu(int x, int y)
    {
        if (TemplateNodeActive)
        {
            return true;
        }

        // commandsAdded means that either toolstrip, toolStripItem or templatenode is selected.
        if (_commandsAdded && x == -1 && y == -1)
        {
            ContextMenuShownByKeyBoard = true;
            Point p = Cursor.Position;
            x = p.X;
            y = p.Y;
        }

        // This has to be done since ToolStripTemplateNode is unsited component that supports its own contextMenu.
        // When the Selection is null, templateNode can be selected.
        // So this block of code here checks if ToolStripKeyBoardHandlingService is present if so,
        // tries to check if the templatenode is selected if so, then gets the templateNode and shows the ContextMenu.
        if (SelectionService.PrimarySelection is not Component
            && SelectedDesignerControl is DesignerToolStripControlHost controlHost
            && controlHost.Control is ToolStripTemplateNode.TransparentToolStrip tool)
        {
            ToolStripTemplateNode node = tool.TemplateNode;
            if (node is not null)
            {
                node.ShowContextMenu(new Point(x, y));
                return true;
            }
        }

        return false;
    }

    // Handler for Copy Command
    private void OnCommandCopy(object sender, EventArgs e)
    {
        bool cutCommand = false;
        try
        {
            // If the Command is CUT and the new Selection is DesignerToolStripControlHost then select it and open its parentDropDown.
            if (sender is MenuCommand com && com.CommandID == StandardCommands.Cut)
            {
                cutCommand = true;
                CutOrDeleteInProgress = true;
            }

            InvokeOldCommand(sender);

            if (cutCommand && OwnerItemAfterCut is ToolStripDropDownItem parentItem)
            {
                if (Host.GetDesigner(parentItem.DropDown) is ToolStripDropDownDesigner dropDownDesigner)
                {
                    SelectionService.SetSelectedComponents(new object[] { dropDownDesigner.Component }, SelectionTypes.Replace);
                }
                else if (!parentItem.DropDown.Visible && Host.GetDesigner(parentItem) is ToolStripMenuItemDesigner designer)
                {
                    designer.SetSelection(true);
                    if (SelectedDesignerControl is DesignerToolStripControlHost curDesignerNode)
                    {
                        curDesignerNode.SelectControl();
                    }
                }
            }

            // this is done So that the Data Behavior doesn't mess up with the copy command during addition of the ToolStrip..
            IMenuCommandService mcs = MenuService;
            if (mcs is not null && _newCommandPaste is null)
            {
                _oldCommandPaste = mcs.FindCommand(StandardCommands.Paste);
                if (_oldCommandPaste is not null)
                {
                    mcs.RemoveCommand(_oldCommandPaste);
                }

                _newCommandPaste = new MenuCommand(new EventHandler(OnCommandPaste), StandardCommands.Paste);
                if (mcs.FindCommand(_newCommandPaste.CommandID) is null)
                {
                    mcs.AddCommand(_newCommandPaste);
                }
            }
        }
        finally
        {
            cutCommand = false;
            CutOrDeleteInProgress = false;
        }
    }

    private void OnCommandDelete(object sender, EventArgs e)
    {
        try
        {
            CutOrDeleteInProgress = true;
            // INVOKE THE OldCommand
            InvokeOldCommand(sender);
            // END
        }
        finally
        {
            CutOrDeleteInProgress = false;
        }
    }

    // Handler for Paste Command
    private void OnCommandPaste(object sender, EventArgs e)
    {
        // IF TemplateNode is Active DO NOT Support Paste. This is what MainMenu did
        // We used to incorrectly paste the item to the parent's collection;
        // so in order to make a simple fix I am being consistent with MainMenu.
        if (TemplateNodeActive)
        {
            return;
        }

        ISelectionService selSvc = SelectionService;
        IDesignerHost host = Host;
        if (selSvc is not null && host is not null)
        {
            if (selSvc.PrimarySelection is not IComponent comp)
            {
                comp = (IComponent)SelectedDesignerControl;
            }

            ToolStripItem item = comp as ToolStripItem;
            ToolStrip parent = null;
            // Case 1: If SelectedObj is ToolStripItem select all items in its immediate parent.
            if (item is not null)
            {
                parent = item.GetCurrentParent();
            }

            parent?.SuspendLayout();

            // INVOKE THE OldCommand
            _oldCommandPaste?.Invoke();

            if (parent is not null)
            {
                parent.ResumeLayout();
                // Since the Glyphs don't get correct bounds as the ToolStrip Layout is suspended .. force Glyph Updates.
                BehaviorService behaviorService = (BehaviorService)_provider.GetService(typeof(BehaviorService));
                behaviorService?.SyncSelection();

                // For ContextMenuStrip; since its not a control .. we don't get called on GetGlyphs directly
                // through the BehaviorService So we need this internal call to push the glyphs on the SelectionManager
                if (host.GetDesigner(item) is ToolStripItemDesigner)
                {
                    ToolStripDropDown dropDown = ToolStripItemDesigner.GetFirstDropDown(item);
                    if (dropDown is not null && !dropDown.IsAutoGenerated)
                    {
                        if (host.GetDesigner(dropDown) is ToolStripDropDownDesigner dropDownDesigner)
                        {
                            dropDownDesigner.AddSelectionGlyphs();
                        }
                    }
                }

                // For Items on DropDown .. we have to manage Glyphs...
                if (parent is ToolStripDropDown parentDropDown && parentDropDown.Visible)
                {
                    if (parentDropDown.OwnerItem is ToolStripDropDownItem ownerItem)
                    {
                        if (host.GetDesigner(ownerItem) is ToolStripMenuItemDesigner itemDesigner)
                        {
                            itemDesigner.ResetGlyphs(ownerItem);
                        }
                    }
                }

                // Get the Selection and ShowDropDown only on ToolStripDropDownItems to show dropDowns after paste operation.
                if (selSvc.PrimarySelection is ToolStripDropDownItem dropDownItem && dropDownItem.DropDown.Visible)
                {
                    // Hide the DropDown
                    dropDownItem.HideDropDown();
                    if (host.GetDesigner(dropDownItem) is ToolStripMenuItemDesigner selectedItemDesigner)
                    {
                        selectedItemDesigner.InitializeDropDown();
                        selectedItemDesigner.InitializeBodyGlyphsForItems(false, dropDownItem);
                        selectedItemDesigner.InitializeBodyGlyphsForItems(true, dropDownItem);
                    }
                }
            }
        }
    }

    // Handler for Home Command
    private void OnCommandHome(object sender, EventArgs e)
    {
        ISelectionService selSvc = SelectionService;
        if (selSvc is not null)
        {
            if (selSvc.PrimarySelection is not ToolStripItem item)
            {
                item = SelectedDesignerControl as ToolStripItem;
            }

            // Process Keys only if we are a ToolStripItem and the TemplateNode is not in InSitu Mode.
            if (item is not null)
            {
                // only select the last item only if there is an Item added in addition to the TemplateNode...
                ToolStrip parent = item.GetCurrentParent();
                int count = parent.Items.Count;
                if (count >= 3) // 3 //3 for the total number of items .. two ToolStripItems + 1 TemplateNode.
                {
                    bool shiftPressed = (Control.ModifierKeys & Keys.Shift) > 0;
                    if (shiftPressed)
                    {
                        // Select all the items between current "item" till the Last item
                        int startIndexOfSelection = 0;
                        int endIndexOfSelection = Math.Max(0, parent.Items.IndexOf(item));
                        int countofItemsSelected = (endIndexOfSelection - startIndexOfSelection) + 1;

                        object[] totalObjects = new object[countofItemsSelected];
                        int j = 0;
                        for (int i = startIndexOfSelection; i <= endIndexOfSelection; i++)
                        {
                            totalObjects[j++] = parent.Items[i];
                        }

                        selSvc.SetSelectedComponents(totalObjects, SelectionTypes.Replace);
                    }
                    else
                    {
                        SetSelection(parent.Items[0]);
                    }
                }
            }
        }
    }

    // Handler for End Command
    private void OnCommandEnd(object sender, EventArgs e)
    {
        ISelectionService selSvc = SelectionService;
        if (selSvc is not null)
        {
            if (selSvc.PrimarySelection is not ToolStripItem item)
            {
                item = SelectedDesignerControl as ToolStripItem;
            }

            // Process Keys only if we are a ToolStripItem and the TemplateNode is not in InSitu Mode.
            if (item is not null)
            {
                // only select the last item only if there is an Item added in addition to the TemplateNode...
                ToolStrip parent = item.GetCurrentParent();
                int count = parent.Items.Count;
                if (count >= 3)  // 3 //3 for the total number of items .. two ToolStripItems + 1 TemplateNode.
                {
                    bool shiftPressed = (Control.ModifierKeys & Keys.Shift) > 0;
                    if (shiftPressed)
                    {
                        // Select all the items between current "item" till the Last item
                        int startIndexOfSelection = parent.Items.IndexOf(item);
                        int endIndexOfSelection = Math.Max(startIndexOfSelection, count - 2);
                        int countofItemsSelected = (endIndexOfSelection - startIndexOfSelection) + 1;

                        object[] totalObjects = new object[countofItemsSelected];
                        int j = 0;
                        for (int i = startIndexOfSelection; i <= endIndexOfSelection; i++)
                        {
                            totalObjects[j++] = parent.Items[i];
                        }

                        selSvc.SetSelectedComponents(totalObjects, SelectionTypes.Replace);
                    }
                    else
                    {
                        SetSelection(parent.Items[count - 2]);
                    }
                }
            }
        }
    }

    // Handler for SelectALL Command
    private void OnCommandSelectAll(object sender, EventArgs e)
    {
        ISelectionService selSvc = SelectionService;
        if (selSvc is not null)
        {
            object selectedObj = selSvc.PrimarySelection;
            // Case 1: If SelectedObj is ToolStripItem select all items in its immediate parent.
            if (selectedObj is ToolStripItem)
            {
                ToolStripItem selectedItem = selectedObj as ToolStripItem;
                ToolStrip parent = selectedItem.GetCurrentParent();
                if (parent is ToolStripOverflow)
                {
                    parent = selectedItem.Owner;
                }

                SelectItems(parent);
                BehaviorService behaviorService = (BehaviorService)_provider.GetService(typeof(BehaviorService));
                behaviorService?.Invalidate();

                return;
            }

            // Case 2: if SelectedObj is ToolStrip ... then select all the item contained in it.
            if (selectedObj is ToolStrip)
            {
                ToolStrip parent = selectedObj as ToolStrip;
                SelectItems(parent);
                return;
            }

            // Case 3: if selectedOj is ToolStripPanel ... select the ToolStrips within the ToolStripPanel...
            if (selectedObj is ToolStripPanel)
            {
                ToolStripPanel parentToolStripPanel = selectedObj as ToolStripPanel;
                selSvc.SetSelectedComponents(parentToolStripPanel.Controls, SelectionTypes.Replace);
                return;
            }
        }
    }

    // this will get called for "Chrome Panel" command. We have to show the ItemList if The TemplateNode is selected.
    private void OnKeyShowDesignerActions(object sender, EventArgs e)
    {
        ISelectionService selSvc = SelectionService;
        if (selSvc is not null)
        {
            if (selSvc.PrimarySelection is null)
            {
                if (SelectedDesignerControl is DesignerToolStripControlHost controlHost)
                {
                    if (controlHost.Control is ToolStripTemplateNode.TransparentToolStrip tool)
                    {
                        ToolStripTemplateNode node = tool.TemplateNode;
                        if (node is not null)
                        {
                            node.ShowDropDownMenu();
                            return;
                        }
                    }
                }
            }
        }

        // INVOKE THE OldCommand
        InvokeOldCommand(sender);
        // END
    }

    // Command handler for enter key.
    private void OnKeyDefault(object sender, EventArgs e)
    {
        // Return if the contextMenu was open during this KeyDefault...
        if (_templateNodeContextMenuOpen)
        {
            _templateNodeContextMenuOpen = false;
            return;
        }

        // Return key. Handle it like a double-click on the primary selection
        ISelectionService selSvc = SelectionService;
        IDesignerHost host = Host;
        if (selSvc is not null)
        {
            if (selSvc.PrimarySelection is not IComponent pri)
            {
                if (SelectedDesignerControl is DesignerToolStripControlHost typeHereNode)
                {
                    if (host is not null)
                    {
                        if (typeHereNode.IsOnDropDown && !typeHereNode.IsOnOverflow)
                        {
                            ToolStripDropDownItem ownerItem = (ToolStripDropDownItem)((ToolStripDropDown)(typeHereNode.Owner)).OwnerItem;
                            if (host.GetDesigner(ownerItem) is ToolStripMenuItemDesigner itemDesigner)
                            {
                                if (!itemDesigner.IsEditorActive)
                                {
                                    itemDesigner.EditTemplateNode(true);
                                    if (ActiveTemplateNode is not null)
                                    {
                                        ActiveTemplateNode._ignoreFirstKeyUp = true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (host.GetDesigner(typeHereNode.Owner) is ToolStripDesigner tooldesigner)
                            {
                                tooldesigner.ShowEditNode(true);
                                if (ActiveTemplateNode is not null)
                                {
                                    ActiveTemplateNode._ignoreFirstKeyUp = true;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (host is not null)
                {
                    IDesigner designer = host.GetDesigner(pri);
                    if (designer is ToolStripMenuItemDesigner tooldesigner)
                    {
                        if (tooldesigner.IsEditorActive)
                        {
                            return;
                        }
                        else
                        {
                            tooldesigner.ShowEditNode(false);
                            if (ActiveTemplateNode is not null)
                            {
                                ActiveTemplateNode._ignoreFirstKeyUp = true;
                            }
                        }
                    }
                    else if (designer is not null)
                    {
                        // INVOKE THE OldCommand
                        InvokeOldCommand(sender);
                    }
                }
            }
        }
    }

    /// <summary>
    ///  This is a function which gets called when the item goes into InSitu Edit mode.
    /// </summary>
    private void OnKeyEdit(object sender, EventArgs e)
    {
        // This method allows the ToolStrip Template Node into the EditMode.
        ISelectionService selSvc = SelectionService;
        IDesignerHost host = Host;
        if (selSvc is not null)
        {
            if (selSvc.PrimarySelection is not IComponent comp)
            {
                comp = (IComponent)SelectedDesignerControl;
            }

            if (comp is ToolStripItem)
            {
                if (host is not null)
                {
                    CommandID cmd = ((MenuCommand)sender).CommandID;
                    if (cmd.Equals(MenuCommands.EditLabel))
                    {
                        if (comp is ToolStripMenuItem)
                        {
                            if (host.GetDesigner(comp) is ToolStripMenuItemDesigner designer)
                            {
                                if (!designer.IsEditorActive)
                                {
                                    designer.ShowEditNode(false);
                                }
                            }
                        }

                        if (comp is DesignerToolStripControlHost)
                        {
                            DesignerToolStripControlHost typeHereNode = comp as DesignerToolStripControlHost;
                            if (typeHereNode.IsOnDropDown)
                            {
                                ToolStripDropDownItem ownerItem = (ToolStripDropDownItem)((ToolStripDropDown)(typeHereNode.Owner)).OwnerItem;
                                if (host.GetDesigner(ownerItem) is ToolStripMenuItemDesigner itemDesigner)
                                {
                                    if (!itemDesigner.IsEditorActive)
                                    {
                                        itemDesigner.EditTemplateNode(false);
                                    }
                                }
                            }
                            else
                            {
                                if (host.GetDesigner(typeHereNode.Owner) is ToolStripDesigner tooldesigner)
                                {
                                    tooldesigner.ShowEditNode(false);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    ///  This is a function which gets called when the arrow keys are used at design time on ToolStrips.
    /// </summary>
    private void OnKeyMove(object sender, EventArgs e)
    {
        // Arrow keys. Begin a drag if the selection isn't locked.
        ISelectionService selSvc = SelectionService;
        if (selSvc is not null)
        {
            MenuCommand cmd = (MenuCommand)sender;
            _shiftPressed = cmd.CommandID.Equals(MenuCommands.KeySizeWidthIncrease)
                || cmd.CommandID.Equals(MenuCommands.KeySizeWidthDecrease)
                || cmd.CommandID.Equals(MenuCommands.KeySizeHeightDecrease)
                || cmd.CommandID.Equals(MenuCommands.KeySizeHeightIncrease);

            // check for ContextMenu..
            if (selSvc.PrimarySelection is ContextMenuStrip)
            {
                if (cmd.CommandID.Equals(MenuCommands.KeyMoveDown))
                {
                    ProcessUpDown(true);
                }

                return;
            }

            if (selSvc.PrimarySelection is not ToolStripItem item)
            {
                item = SelectedDesignerControl as ToolStripItem;
            }

            // Process Keys only if we are a ToolStripItem and the TemplateNode is not in InSitu Mode.
            if (item is not null)
            {
                if (cmd.CommandID.Equals(MenuCommands.KeyMoveRight) || cmd.CommandID.Equals(MenuCommands.KeyNudgeRight) || cmd.CommandID.Equals(MenuCommands.KeySizeWidthIncrease))
                {
                    if (!ProcessRightLeft(true))
                    {
                        RotateTab(false);
                        return;
                    }
                }

                if (cmd.CommandID.Equals(MenuCommands.KeyMoveLeft) || cmd.CommandID.Equals(MenuCommands.KeyNudgeLeft) || cmd.CommandID.Equals(MenuCommands.KeySizeWidthDecrease))
                {
                    if (!ProcessRightLeft(false))
                    {
                        RotateTab(true);
                        return;
                    }
                }

                if (cmd.CommandID.Equals(MenuCommands.KeyMoveDown) || cmd.CommandID.Equals(MenuCommands.KeyNudgeDown) || cmd.CommandID.Equals(MenuCommands.KeySizeHeightIncrease))
                {
                    ProcessUpDown(true);
                    return;
                }

                if (cmd.CommandID.Equals(MenuCommands.KeyMoveUp) || cmd.CommandID.Equals(MenuCommands.KeyNudgeUp) || cmd.CommandID.Equals(MenuCommands.KeySizeHeightDecrease))
                {
                    ProcessUpDown(false);
                    return;
                }
            }
            else
            {
                // INVOKE THE OldCommand
                InvokeOldCommand(sender);
            }
        }
    }

    /// <summary>
    ///  This is a function which gets called when Cancel is pressed when we are on ToolStripItem.
    /// </summary>
    private void OnKeyCancel(object sender, EventArgs e)
    {
        ISelectionService selSvc = SelectionService;
        if (selSvc is not null)
        {
            if (selSvc.PrimarySelection is not ToolStripItem item)
            {
                item = SelectedDesignerControl as ToolStripItem;
            }

            // Process Keys only if we are a ToolStripItem and the TemplateNode is not in InSitu Mode.
            if (item is not null)
            {
                MenuCommand cmd = (MenuCommand)sender;
                bool reverse = (cmd.CommandID.Equals(MenuCommands.KeyReverseCancel));
                RotateParent(reverse);
                return;
            }
            else
            {
                // Check if the ToolStripDropDown (which is designable) is currently selected. If so this should select the "RootComponent"
                if (selSvc.PrimarySelection is ToolStripDropDown dropDown && dropDown.Site is not null)
                {
                    selSvc.SetSelectedComponents(new object[] { Host.RootComponent }, SelectionTypes.Replace);
                }
                else
                {
                    // INVOKE THE OldCommand
                    InvokeOldCommand(sender);
                }
            }
        }
    }

    /// <summary>
    ///  This function allows the CommandSet to select the right item when the Tab and Arrow keys are used.
    /// </summary>
    private void OnKeySelect(object sender, EventArgs e)
    {
        MenuCommand cmd = (MenuCommand)sender;
        bool reverse = cmd.CommandID.Equals(MenuCommands.KeySelectPrevious);
        ProcessKeySelect(reverse);
    }

    /// <summary>
    ///  Called when the current selection changes. Here we determine what commands can and can't be enabled.
    /// </summary>
    private void OnSelectionChanging(object sender, EventArgs e)
    {
        if (SelectionService.PrimarySelection is not Component primarySelection)
        {
            primarySelection = SelectedDesignerControl as ToolStripItem;
        }

        ToolStrip tool = primarySelection as ToolStrip;
        if (tool is not null)
        {
            InheritanceAttribute ia = (InheritanceAttribute)TypeDescriptor.GetAttributes(tool)[typeof(InheritanceAttribute)];
            if (ia is not null && (ia.InheritanceLevel == InheritanceLevel.Inherited || ia.InheritanceLevel == InheritanceLevel.InheritedReadOnly))
            {
                return;
            }
        }

        if (tool is null && !(primarySelection is ToolStripItem))
        {
            RestoreCommands();
            // Reset the cached item...
            SelectedDesignerControl = null;
        }
    }

    /// <summary>
    ///  Called when the current selection changes. Here we determine what commands can and can't be enabled.
    /// </summary>
    private void OnSelectionChanged(object sender, EventArgs e)
    {
        if (SelectionService.PrimarySelection is not Component primarySelection)
        {
            primarySelection = SelectedDesignerControl as ToolStripItem;
        }

        ToolStrip tool = primarySelection as ToolStrip;
        if (tool is not null)
        {
            InheritanceAttribute ia = (InheritanceAttribute)TypeDescriptor.GetAttributes(tool)[typeof(InheritanceAttribute)];
            if (ia is not null && (ia.InheritanceLevel == InheritanceLevel.Inherited || ia.InheritanceLevel == InheritanceLevel.InheritedReadOnly))
            {
                return;
            }
        }

        if (tool is not null || primarySelection is ToolStripItem)
        {
            // Remove the Panel if any
            BehaviorService behaviorService = (BehaviorService)_provider.GetService(typeof(BehaviorService));
            if (behaviorService is not null)
            {
                DesignerActionUI designerUI = behaviorService.DesignerActionUI;
                designerUI?.HideDesignerActionPanel();
            }

            AddCommands();
        }
    }

    // helper function to select the next item.
    public void ProcessKeySelect(bool reverse)
    {
        if (SelectionService is not { } selectionService)
        {
            return;
        }

        if (selectionService.PrimarySelection is not ToolStripItem item)
        {
            item = SelectedDesignerControl as ToolStripItem;
        }

        // Process Keys only if we are a ToolStripItem and the TemplateNode is not in InSitu Mode.
        if (item is not null)
        {
            if (!ProcessRightLeft(!reverse))
            {
                RotateTab(reverse);
                return;
            }

            return;
        }
        else if (selectionService.PrimarySelection is ToolStrip)
        {
            RotateTab(reverse);
        }
    }

    /// <summary>
    ///  This is the private helper function which is used to select the toolStripItem in the 'right' direction.
    /// </summary>
    private bool ProcessRightLeft(bool right)
    {
        object targetSelection = null;
        object currentSelection;

        if (SelectionService is not { } selectionService
            || Host is not { } host
            || host.RootComponent is not Control)
        {
            return false;
        }

        currentSelection = selectionService.PrimarySelection;
        if (_shiftPressed && ShiftPrimaryItem is not null)
        {
            currentSelection = ShiftPrimaryItem;
        }

        currentSelection ??= SelectedDesignerControl;

        if (currentSelection is Control)
        {
            return false;
        }

        ToolStripItem toolStripItem = selectionService.PrimarySelection as ToolStripItem;
        if (_shiftPressed && ShiftPrimaryItem is not null)
        {
            toolStripItem = ShiftPrimaryItem as ToolStripItem;
        }

        toolStripItem ??= SelectedDesignerControl as ToolStripItem;

        if (toolStripItem is DesignerToolStripControlHost && toolStripItem.GetCurrentParent() is ToolStripDropDown parent)
        {
            if (right)
            {
                // no where to go .. since we are on DesignerToolStripControlHost for DropDown.
            }
            else
            {
                targetSelection = parent is ToolStripOverflow
                    ? GetNextItem(parent, toolStripItem, ArrowDirection.Left)
                    : (object)parent.OwnerItem;
            }

            if (targetSelection is not null)
            {
                SetSelection(targetSelection);
                return true;
            }
        }
        else
        {
            ToolStripItem item = selectionService.PrimarySelection as ToolStripItem;
            if (_shiftPressed && ShiftPrimaryItem is not null)
            {
                item = ShiftPrimaryItem as ToolStripDropDownItem;
            }

            item ??= SelectedDesignerControl as ToolStripDropDownItem;

            if (item is null || !item.IsOnDropDown)
            {
                return false;
            }

            bool menusCascadeRight = SystemInformation.RightAlignedMenus;
            if (((menusCascadeRight && right) || (!menusCascadeRight && right))
                && item is ToolStripDropDownItem dropDownItem)
            {
                targetSelection = GetNextItem(dropDownItem.DropDown, null, ArrowDirection.Right);

                if (targetSelection is not null)
                {
                    SetSelection(targetSelection);

                    // Open the DropDown after the Selection is Completed.
                    if (!dropDownItem.DropDown.Visible
                        && host.GetDesigner(dropDownItem) is ToolStripMenuItemDesigner designer)
                    {
                        designer.InitializeDropDown();
                    }

                    return true;
                }
            }

            if (!right && !menusCascadeRight)
            {
                ToolStripItem owner = ((ToolStripDropDown)item.Owner).OwnerItem;
                if (!owner.IsOnDropDown)
                {
                    ToolStrip mainTool = owner.GetCurrentParent();
                    targetSelection = GetNextItem(mainTool, owner, ArrowDirection.Left);
                }
                else
                {
                    targetSelection = owner;
                }

                if (targetSelection is not null)
                {
                    SetSelection(targetSelection);
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    ///  This is the private helper function which is used to select the toolStripItem in the 'down' direction.
    /// </summary>
    public void ProcessUpDown(bool down)
    {
        object targetSelection = null;
        object currentSelection;

        if (SelectionService is not { } selectionService
            || Host is not { } host
            || host.RootComponent is not Control)
        {
            return;
        }

        currentSelection = selectionService.PrimarySelection;
        if (_shiftPressed && ShiftPrimaryItem is not null)
        {
            currentSelection = ShiftPrimaryItem;
        }

        // Check for ContextMenuStrip first.
        if (currentSelection is ContextMenuStrip contextMenu)
        {
            if (down)
            {
                targetSelection = GetNextItem(contextMenu, null, ArrowDirection.Down);
                SetSelection(targetSelection);
            }

            return;
        }

        currentSelection ??= SelectedDesignerControl;

        if (currentSelection is Control)
        {
            return;
        }

        ToolStripItem item = selectionService.PrimarySelection as ToolStripItem;
        if (_shiftPressed && ShiftPrimaryItem is not null)
        {
            item = ShiftPrimaryItem as ToolStripItem;
        }

        item ??= SelectedDesignerControl as ToolStripItem;

        ToolStripDropDown parentToMoveOn = null;
        if (item is null)
        {
            return;
        }

        if (item is DesignerToolStripControlHost)
        {
            // If down arrow is pressed open the dropDown.
            if (down)
            {
                if (SelectedDesignerControl is DesignerToolStripControlHost controlHost
                    && controlHost.Control is ToolStripTemplateNode.TransparentToolStrip tool
                    && tool.TemplateNode is { } node)
                {
                    node.ShowDropDownMenu();
                    return;
                }
            }
            else
            {
                parentToMoveOn = item.GetCurrentParent() as ToolStripDropDown;
            }
        }
        else
        {
            ToolStripDropDownItem dropDownItem = item as ToolStripDropDownItem;
            if (dropDownItem is not null && !dropDownItem.IsOnDropDown)
            {
                parentToMoveOn = dropDownItem.DropDown;
                item = null;
            }
            else if (dropDownItem is not null)
            {
                parentToMoveOn = ((dropDownItem.Placement == ToolStripItemPlacement.Overflow)
                    ? dropDownItem.Owner.OverflowButton.DropDown
                    : dropDownItem.Owner) as ToolStripDropDown;
                item = dropDownItem;
            }

            if (dropDownItem is null)
            {
                parentToMoveOn = item.GetCurrentParent() as ToolStripDropDown;
            }
        }

        if (parentToMoveOn is null)
        {
            // This will be null for NON dropDownItems.
            return;
        }

        if (down)
        {
            targetSelection = GetNextItem(parentToMoveOn, item, ArrowDirection.Down);

            // Check the index to know if we have wrapped around.
            // Only on NON ContextMenuStrip, ToolStripDropDown (added from toolbox).
            if (parentToMoveOn.OwnerItem is not null
                && !parentToMoveOn.OwnerItem.IsOnDropDown
                && parentToMoveOn.OwnerItem.Owner?.Site is not null
                && targetSelection is ToolStripItem newSelection)
            {
                // We are wrapping around on the FirstDropDown select OwnerItem.
                if (parentToMoveOn.Items.IndexOf(newSelection) != -1
                    && parentToMoveOn.Items.IndexOf(newSelection) <= parentToMoveOn.Items.IndexOf(item))
                {
                    targetSelection = parentToMoveOn.OwnerItem;
                }
            }

            if (_shiftPressed && SelectionService.GetComponentSelected(targetSelection))
            {
                SelectionService.SetSelectedComponents(new object[] { ShiftPrimaryItem, targetSelection }, SelectionTypes.Remove);
            }
        }
        else
        {
            // We don't want to WRAP around for items on toolStrip Overflow, if the currentSelection is the
            // topMost item on the Overflow, but select the one on the PARENT toolStrip.
            if (parentToMoveOn is ToolStripOverflow)
            {
                if (item == GetNextItem(parentToMoveOn, startItem: null, ArrowDirection.Down))
                {
                    if (item.Owner is ToolStrip owner)
                    {
                        targetSelection = GetNextItem(owner, parentToMoveOn.OwnerItem, ArrowDirection.Left);
                    }
                }
                else
                {
                    targetSelection = GetNextItem(parentToMoveOn, item, ArrowDirection.Up);
                }
            }
            else
            {
                targetSelection = GetNextItem(parentToMoveOn, item, ArrowDirection.Up);
            }

            // Check the index to know if we have wrapped around.
            if (parentToMoveOn.OwnerItem is not null
                && !parentToMoveOn.OwnerItem.IsOnDropDown
                && parentToMoveOn.OwnerItem.Owner?.Site is not null
                && targetSelection is ToolStripItem newSelection
                && item is not null)
            {
                // We are wrapping around on the FirstDropDown select OwnerItem.
                if (parentToMoveOn.Items.IndexOf(newSelection) != -1
                    && parentToMoveOn.Items.IndexOf(newSelection) >= parentToMoveOn.Items.IndexOf(item))
                {
                    targetSelection = parentToMoveOn.OwnerItem;
                }
            }

            if (_shiftPressed && SelectionService.GetComponentSelected(targetSelection))
            {
                SelectionService.SetSelectedComponents(new object[] { ShiftPrimaryItem, targetSelection }, SelectionTypes.Remove);
            }
        }

        if (targetSelection is not null && targetSelection != item)
        {
            SetSelection(targetSelection);
        }
    }

    // caches the old commands from the menuCommand service.
    private void PopulateOldCommands()
    {
        _oldCommands ??= [];

        IMenuCommandService mcs = MenuService;
        if (mcs is not null)
        {
            _oldCommands.Add(mcs.FindCommand(MenuCommands.KeySelectNext));
            _oldCommands.Add(mcs.FindCommand(MenuCommands.KeySelectPrevious));
            _oldCommands.Add(mcs.FindCommand(MenuCommands.KeyDefaultAction));

            _oldCommands.Add(mcs.FindCommand(MenuCommands.KeyMoveUp));
            _oldCommands.Add(mcs.FindCommand(MenuCommands.KeyMoveDown));
            _oldCommands.Add(mcs.FindCommand(MenuCommands.KeyMoveLeft));
            _oldCommands.Add(mcs.FindCommand(MenuCommands.KeyMoveRight));

            _oldCommands.Add(mcs.FindCommand(MenuCommands.KeyNudgeUp));
            _oldCommands.Add(mcs.FindCommand(MenuCommands.KeyNudgeDown));
            _oldCommands.Add(mcs.FindCommand(MenuCommands.KeyNudgeLeft));
            _oldCommands.Add(mcs.FindCommand(MenuCommands.KeyNudgeRight));

            _oldCommands.Add(mcs.FindCommand(MenuCommands.KeySizeWidthIncrease));
            _oldCommands.Add(mcs.FindCommand(MenuCommands.KeySizeHeightIncrease));
            _oldCommands.Add(mcs.FindCommand(MenuCommands.KeySizeWidthDecrease));
            _oldCommands.Add(mcs.FindCommand(MenuCommands.KeySizeHeightDecrease));

            _oldCommands.Add(mcs.FindCommand(MenuCommands.KeyCancel));
            _oldCommands.Add(mcs.FindCommand(MenuCommands.KeyReverseCancel));
            _oldCommands.Add(mcs.FindCommand(StandardCommands.Copy));
            _oldCommands.Add(mcs.FindCommand(StandardCommands.SelectAll));
            _oldCommands.Add(mcs.FindCommand(MenuCommands.KeyInvokeSmartTag));

            _oldCommands.Add(mcs.FindCommand(StandardCommands.Cut));
            _oldCommands.Add(mcs.FindCommand(StandardCommands.Delete));
        }
    }

    // populates a list of our custom commands to be added to menu command service.
    private void PopulateNewCommands()
    {
        _newCommands ??= [];

        _newCommands.Add(new MenuCommand(new EventHandler(OnKeySelect), MenuCommands.KeySelectNext));
        _newCommands.Add(new MenuCommand(new EventHandler(OnKeySelect), MenuCommands.KeySelectPrevious));
        _newCommands.Add(new MenuCommand(new EventHandler(OnKeyDefault), MenuCommands.KeyDefaultAction));
        _newCommands.Add(new MenuCommand(new EventHandler(OnKeyEdit), MenuCommands.EditLabel));

        _newCommands.Add(new MenuCommand(new EventHandler(OnKeyMove), MenuCommands.KeyMoveUp));
        _newCommands.Add(new MenuCommand(new EventHandler(OnKeyMove), MenuCommands.KeyMoveDown));
        _newCommands.Add(new MenuCommand(new EventHandler(OnKeyMove), MenuCommands.KeyMoveLeft));
        _newCommands.Add(new MenuCommand(new EventHandler(OnKeyMove), MenuCommands.KeyMoveRight));

        _newCommands.Add(new MenuCommand(new EventHandler(OnKeyMove), MenuCommands.KeyNudgeUp));
        _newCommands.Add(new MenuCommand(new EventHandler(OnKeyMove), MenuCommands.KeyNudgeDown));
        _newCommands.Add(new MenuCommand(new EventHandler(OnKeyMove), MenuCommands.KeyNudgeLeft));
        _newCommands.Add(new MenuCommand(new EventHandler(OnKeyMove), MenuCommands.KeyNudgeRight));

        _newCommands.Add(new MenuCommand(new EventHandler(OnKeyMove), MenuCommands.KeySizeWidthIncrease));
        _newCommands.Add(new MenuCommand(new EventHandler(OnKeyMove), MenuCommands.KeySizeHeightIncrease));
        _newCommands.Add(new MenuCommand(new EventHandler(OnKeyMove), MenuCommands.KeySizeWidthDecrease));
        _newCommands.Add(new MenuCommand(new EventHandler(OnKeyMove), MenuCommands.KeySizeHeightDecrease));

        _newCommands.Add(new MenuCommand(new EventHandler(OnKeyCancel), MenuCommands.KeyCancel));
        _newCommands.Add(new MenuCommand(new EventHandler(OnKeyCancel), MenuCommands.KeyReverseCancel));
        _newCommands.Add(new MenuCommand(new EventHandler(OnCommandCopy), StandardCommands.Copy));
        _newCommands.Add(new MenuCommand(new EventHandler(OnCommandSelectAll), StandardCommands.SelectAll));

        _newCommands.Add(new MenuCommand(new EventHandler(OnCommandHome), MenuCommands.KeyHome));
        _newCommands.Add(new MenuCommand(new EventHandler(OnCommandEnd), MenuCommands.KeyEnd));
        _newCommands.Add(new MenuCommand(new EventHandler(OnCommandHome), MenuCommands.KeyShiftHome));
        _newCommands.Add(new MenuCommand(new EventHandler(OnCommandEnd), MenuCommands.KeyShiftEnd));

        // Command for opening the DropDown for templatenode.
        _newCommands.Add(new MenuCommand(new EventHandler(OnKeyShowDesignerActions), MenuCommands.KeyInvokeSmartTag));

        _newCommands.Add(new MenuCommand(new EventHandler(OnCommandCopy), StandardCommands.Cut));
        _newCommands.Add(new MenuCommand(new EventHandler(OnCommandDelete), StandardCommands.Delete));
    }

    // restores the old commands back into the menu command service.
    public void RestoreCommands()
    {
        IMenuCommandService mcs = MenuService;
        if (mcs is not null & _commandsAdded)
        {
            // Remove the new Commands
            if (_newCommands is not null)
            {
                foreach (MenuCommand newCommand in _newCommands)
                {
                    mcs.RemoveCommand(newCommand);
                }
            }

            // Add old Commands
            if (_oldCommands is not null)
            {
                foreach (MenuCommand oldCommand in _oldCommands)
                {
                    if (oldCommand is not null && mcs.FindCommand(oldCommand.CommandID) is null)
                    {
                        mcs.AddCommand(oldCommand);
                    }
                }
            }

            if (_newCommandPaste is not null)
            {
                mcs.RemoveCommand(_newCommandPaste);
                _newCommandPaste = null;
            }

            if (_oldCommandPaste is not null && mcs.FindCommand(_oldCommandPaste.CommandID) is null)
            {
                mcs.AddCommand(_oldCommandPaste);
                _oldCommandPaste = null;
            }

            _commandsAdded = false;
        }
    }

    internal void ResetActiveTemplateNodeSelectionState()
    {
        if (SelectedDesignerControl is not null)
        {
            if (SelectedDesignerControl is DesignerToolStripControlHost curDesignerNode)
            {
                curDesignerNode.RefreshSelectionGlyph();
            }
        }
    }

    /// <summary>
    ///  Disposes of this object, removing all commands from the menu service.
    /// </summary>
    public void RemoveCommands()
    {
        IMenuCommandService mcs = MenuService;
        if (mcs is not null && _commandsAdded)
        {
            // Remove our Commands...
            if (_newCommands is not null)
            {
                foreach (MenuCommand newCommand in _newCommands)
                {
                    mcs.RemoveCommand(newCommand);
                }
            }
        }

        if (_newCommandPaste is not null)
        {
            mcs.RemoveCommand(_newCommandPaste);
            _newCommandPaste = null;
        }

        if (_oldCommandPaste is not null)
        {
            _oldCommandPaste = null;
        }

        if (_newCommands is not null)
        {
            _newCommands.Clear();
            _newCommands = null;
        }

        if (_oldCommands is not null)
        {
            _oldCommands.Clear();
            _oldCommands = null;
        }

        if (_selectionService is not null)
        {
            _selectionService.SelectionChanging -= OnSelectionChanging;
            _selectionService.SelectionChanged -= OnSelectionChanged;
            _selectionService = null;
        }

        if (_componentChangeService is not null)
        {
            _componentChangeService.ComponentRemoved -= OnComponentRemoved;
            _componentChangeService = null;
        }

        _currentSelection = null;
        _shiftPrimary = null;
        _provider = null;
        _menuCommandService = null;
        _activeTemplateNode = null;
    }

    /// <summary>
    ///  This function allows the service to select the parent for the selected Item.
    /// </summary>
    private void RotateParent(bool backwards)
    {
        Control current = null;
        object next = null;
        ToolStripItem toolStripItem = null;
        ISelectionService selSvc = SelectionService;
        IDesignerHost host = Host;
        if (selSvc is null || host is null || !(host.RootComponent is Control))
        {
            return;
        }

        IContainer container = host.Container;
        if (selSvc.PrimarySelection is not Control component)
        {
            component = SelectedDesignerControl as Control;
        }

        if (component is not null)
        {
            current = component;
        }
        else
        {
            toolStripItem = selSvc.PrimarySelection as ToolStripItem;
            toolStripItem ??= SelectedDesignerControl as ToolStripItem;

            if (toolStripItem is null)
            {
                current = (Control)host.RootComponent;
            }
        }

        if (backwards)
        {
            if (current is not null)
            {
                next = current.Controls.Count > 0 ? current.Controls[0] : (object)current;
            }
            else if (toolStripItem is not null)
            {
                next = toolStripItem.Owner.Controls[0];
            }
        }
        else
        {
            if (current is not null)
            {
                next = current.Parent;
                if (!(next is Control nextControl) || nextControl.Site is null || nextControl.Site.Container != container)
                {
                    next = current;
                }
            }
            else if (toolStripItem is not null)
            {
                if (toolStripItem.IsOnDropDown && toolStripItem.Placement != ToolStripItemPlacement.Overflow)
                {
                    next = ((ToolStripDropDown)toolStripItem.Owner).OwnerItem;
                }
                else if (toolStripItem.IsOnDropDown && toolStripItem.Placement == ToolStripItemPlacement.Overflow)
                {
                    ToolStrip owner = toolStripItem.Owner;
                    owner?.OverflowButton.HideDropDown();

                    next = toolStripItem.Owner;
                }
                else
                {
                    next = toolStripItem.Owner;
                }
            }
        }

        if (next is DesignerToolStripControlHost)
        {
            SelectedDesignerControl = next;
            selSvc.SetSelectedComponents(null, SelectionTypes.Replace);
        }
        else
        {
            SelectedDesignerControl = null;
            selSvc.SetSelectedComponents(new object[] { next }, SelectionTypes.Replace);
        }
    }

    /// <summary>
    ///  This function allows the service to rotate the TabSelection when TAB key is pressed.
    /// </summary>
    // Okay to suppress because of complex code path
    public void RotateTab(bool backwards)
    {
        Control ctl;
        Control baseCtl;
        object targetSelection = null;
        object currentSelection;
        ISelectionService selSvc = SelectionService;
        IDesignerHost host = Host;
        if (selSvc is null || host is null || !(host.RootComponent is Control))
        {
            return;
        }

        IContainer container = host.Container;
        baseCtl = (Control)host.RootComponent;
        // We must handle two cases of logic here. We are responsible for handling selection within ourself,
        // and also for components on the tray. For our own tabbing around, we want to go by tab-order.
        // When we get to the end of the form, however, we go by selection order into the tray.
        // And, when we're at the end of the tray we start back at the form. We must reverse this logic to go backwards.
        currentSelection = selSvc.PrimarySelection;
        if (_shiftPressed && ShiftPrimaryItem is not null)
        {
            currentSelection = ShiftPrimaryItem;
        }

        if (currentSelection is null)
        {
            currentSelection = SelectedDesignerControl;
            // If we are on templateNode and tabbing ahead ... the select the next Control on the parent ...
            if (currentSelection is not null)
            {
                if (currentSelection is DesignerToolStripControlHost templateNodeItem && (!templateNodeItem.IsOnDropDown || (templateNodeItem.IsOnDropDown && templateNodeItem.IsOnOverflow)))
                {
                    ctl = templateNodeItem.Owner;
                    if ((ctl.RightToLeft != RightToLeft.Yes && !backwards) || (ctl.RightToLeft == RightToLeft.Yes && backwards))
                    {
                        targetSelection = GetNextControlInTab(baseCtl, ctl, !backwards);
                        if (targetSelection is null)
                        {
                            ComponentTray tray = (ComponentTray)_provider.GetService(typeof(ComponentTray));
                            if (tray is not null)
                            {
                                targetSelection = tray.GetNextComponent((IComponent)currentSelection, !backwards);
                                if (targetSelection is not null)
                                {
                                    ControlDesigner controlDesigner = host.GetDesigner((IComponent)targetSelection) as ControlDesigner;
                                    // In Whidbey controls like ToolStrips have componentTray presence,
                                    // so don't select them again through component tray since here we
                                    // select only Components. Hence only components that have ComponentDesigners
                                    // should be selected via the ComponentTray.
                                    while (controlDesigner is not null)
                                    {
                                        // if the targetSelection from the Tray is a control .. try the next one.
                                        targetSelection = tray.GetNextComponent((IComponent)targetSelection, !backwards);
                                        controlDesigner = targetSelection is not null ? host.GetDesigner((IComponent)targetSelection) as ControlDesigner : null;
                                    }
                                }
                            }

                            targetSelection ??= baseCtl;
                        }
                    }
                }
            }
        }

        ctl = currentSelection as Control;
        // Added New Code for ToolStrip Tabbing..
        if (targetSelection is null && ctl is ToolStrip wb)
        {
            ToolStripItemCollection collection = wb.Items;
            if (collection is not null)
            {
                targetSelection = !backwards ? collection[0] : (object)collection[wb.Items.Count - 1];
            }
        }

        // ctl is NOT A CONTROL ... so its Component. Try this for ToolStripItem.
        if (targetSelection is null && ctl is null)
        {
            ToolStripItem item = selSvc.PrimarySelection as ToolStripItem;
            if (_shiftPressed && ShiftPrimaryItem is not null)
            {
                item = ShiftPrimaryItem as ToolStripItem;
            }

            item ??= SelectedDesignerControl as ToolStripItem;

            if (item is not null && item.IsOnDropDown && item.Placement != ToolStripItemPlacement.Overflow)
            {
                // You come here only for DesignerToolStripControlHost on the DropDown ...
                Debug.WriteLineIf(item is DesignerToolStripControlHost, " Why are we here for non DesignerMenuItem??");
                if (item is DesignerToolStripControlHost designerItem)
                {
                    ToolStripItem parentItem = ((ToolStripDropDown)designerItem.Owner).OwnerItem;
                    ToolStripDropDown dropDown = ToolStripItemDesigner.GetFirstDropDown((ToolStripDropDownItem)parentItem);
                    item = dropDown is not null ? dropDown.OwnerItem : parentItem;
                }
            }

            if (item is not null and not DesignerToolStripControlHost)
            {
                ToolStrip parent = item.GetCurrentParent();
                if (parent is not null)
                {
                    if (backwards)
                    {
                        // We are item on ToolStripOverflow...
                        if (parent is ToolStripOverflow)
                        {
                            ToolStripItem firstItem = GetNextItem(parent, null, ArrowDirection.Down);
                            if (item == firstItem)
                            {
                                if (item.Owner is ToolStrip owner)
                                {
                                    targetSelection = GetNextItem(owner, ((ToolStripDropDown)parent).OwnerItem, ArrowDirection.Left);
                                }
                            }
                            else
                            {
                                targetSelection = GetNextItem(parent, item, ArrowDirection.Left);
                            }
                        }

                        // check if this is the first item .. if so move out of ToolStrip...
                        else if (item == parent.Items[0] && parent.RightToLeft != RightToLeft.Yes)
                        {
                            // If Shift Pressed ... stop at 1st Item..
                            if (_shiftPressed)
                            {
                                return;
                            }

                            targetSelection = GetNextControlInTab(baseCtl, parent, !backwards);
                            if (targetSelection is null)
                            {
                                ComponentTray tray = (ComponentTray)_provider.GetService(typeof(ComponentTray));
                                if (tray is not null)
                                {
                                    targetSelection = tray.GetNextComponent((IComponent)currentSelection, !backwards);
                                    if (targetSelection is not null)
                                    {
                                        ControlDesigner controlDesigner = host.GetDesigner((IComponent)targetSelection) as ControlDesigner;
                                        // In Whidbey controls like ToolStrips have componentTray presence,
                                        // So don't select them again through component tray since here we
                                        // select only Components. Hence only components that have ComponentDesigners
                                        // should be selected via the ComponentTray.
                                        while (controlDesigner is not null)
                                        {
                                            // if the targetSelection from the Tray is a control .. try the next one.
                                            targetSelection = tray.GetNextComponent((IComponent)targetSelection, !backwards);
                                            controlDesigner = targetSelection is not null ? host.GetDesigner((IComponent)targetSelection) as ControlDesigner : null;
                                        }
                                    }
                                }

                                targetSelection ??= baseCtl;
                            }
                        }
                        else
                        {
                            targetSelection = GetNextItem(parent, item, ArrowDirection.Left);
                            if (_shiftPressed && SelectionService.GetComponentSelected(targetSelection))
                            {
                                SelectionService.SetSelectedComponents(new object[] { ShiftPrimaryItem, targetSelection }, SelectionTypes.Remove);
                            }
                        }
                    }
                    else
                    {
                        // We are item on ToolStripOverflow...
                        if (parent is ToolStripOverflow)
                        {
                            targetSelection = GetNextItem(parent, item, ArrowDirection.Down);
                        }
                        else if (item == parent.Items[0] && parent.RightToLeft == RightToLeft.Yes)
                        {
                            // If Shift Pressed ... stop at 1st Item..
                            if (_shiftPressed)
                            {
                                return;
                            }

                            targetSelection = GetNextControlInTab(baseCtl, parent, !backwards);
                            // this is the First control in TabOrder... Select the Form..
                            targetSelection ??= baseCtl;
                        }
                        else
                        {
                            targetSelection = GetNextItem(parent, item, ArrowDirection.Right);
                            if (_shiftPressed && SelectionService.GetComponentSelected(targetSelection))
                            {
                                SelectionService.SetSelectedComponents(new object[] { ShiftPrimaryItem, targetSelection }, SelectionTypes.Remove);
                            }
                        }
                    }
                }
            }

            // This is a DesignerToolStripControlHost on the Main ToolStrip.
            else if (item is not null)
            {
                ToolStrip parent = item.GetCurrentParent();
                if (parent is not null)
                {
                    // flip the semantics of backwards...
                    if (parent.RightToLeft == RightToLeft.Yes)
                    {
                        backwards = !backwards;
                    }

                    if (backwards)
                    {
                        ToolStripItemCollection collection = parent.Items;
                        targetSelection = collection.Count >= 2 ? collection[collection.Count - 2] : GetNextControlInTab(baseCtl, parent, !backwards);
                    }
                    else
                    {
                        ToolStripItemCollection collection = parent.Items;
                        targetSelection = collection[0];
                    }
                }
            }
        }

        if (targetSelection is null && ctl is not null && (baseCtl.Contains(ctl) || baseCtl == currentSelection))
        {
            // Our current selection is a control. Select the next control in the z-order.
            while ((ctl = GetNextControlInTab(baseCtl, ctl, !backwards)) is not null)
            {
                if (ctl.Site is not null && ctl.Site.Container == container && !(ctl is ToolStripPanel))
                {
                    break;
                }
            }

            targetSelection = ctl;
        }

        if (targetSelection is null)
        {
            ComponentTray tray = (ComponentTray)_provider.GetService(typeof(ComponentTray));
            if (tray is not null)
            {
                targetSelection = tray.GetNextComponent((IComponent)currentSelection, !backwards);
            }

            if (targetSelection is null || targetSelection == currentSelection)
            {
                targetSelection = baseCtl;
            }
        }

        // Special Casing since moving to TemplateNode to TemplateNode is moving from null selection to null selection.
        if (targetSelection is DesignerToolStripControlHost && currentSelection is DesignerToolStripControlHost)
        {
            SelectedDesignerControl = targetSelection;
            selSvc.SetSelectedComponents(new object[] { targetSelection }, SelectionTypes.Replace);
            selSvc.SetSelectedComponents(null, SelectionTypes.Replace);
        }
        else
        {
            SetSelection(targetSelection);
        }
    }

    // Select components.
    private void SelectItems(ToolStrip parent)
    {
        object[] totalObjects = new object[parent.Items.Count - 1];
        for (int i = 0; i < parent.Items.Count - 1; i++)
        {
            if (parent.Items[i] is DesignerToolStripControlHost)
            {
                continue;
            }

            totalObjects[i] = parent.Items[i];
        }

        SelectionService.SetSelectedComponents(totalObjects, SelectionTypes.Replace);
    }

    // Helper function called by all handlers to select the target Selection.
    private void SetSelection(object targetSelection)
    {
        ISelectionService selSvc = SelectionService;
        if (selSvc is not null)
        {
            // Cache original selection
            ICollection originalSelComps = selSvc.GetSelectedComponents();
            // Add the TemplateNode to the Selection if it is currently Selected as the GetSelectedComponents won't do it for us.
            ArrayList origSel = new(originalSelComps);
            if (origSel.Count == 0)
            {
                if (SelectedDesignerControl is not null)
                {
                    origSel.Add(SelectedDesignerControl);
                }
            }

            if (targetSelection is DesignerToolStripControlHost)
            {
                if (!_shiftPressed)
                {
                    SelectedDesignerControl = targetSelection;
                    selSvc.SetSelectedComponents(null, SelectionTypes.Replace);
                }
            }
            else
            {
                if (targetSelection is ToolStripOverflowButton overFlowButton)
                {
                    SelectedDesignerControl = null;

                    overFlowButton.ShowDropDown();

                    object newSelection = GetNextItem(overFlowButton.DropDown, null, ArrowDirection.Down);

                    if (!_shiftPressed)
                    {
                        ShiftPrimaryItem = null;
                        selSvc.SetSelectedComponents(new object[] { newSelection }, SelectionTypes.Replace);
                    }
                    else
                    {
                        selSvc.SetSelectedComponents(new object[] { newSelection });
                        ShiftPrimaryItem = targetSelection;
                    }
                }
                else
                {
                    SelectedDesignerControl = null;
                    if (!_shiftPressed)
                    {
                        ShiftPrimaryItem = null;
                        selSvc.SetSelectedComponents(new object[] { targetSelection }, SelectionTypes.Replace);
                    }
                    else
                    {
                        selSvc.SetSelectedComponents(new object[] { targetSelection });
                        ShiftPrimaryItem = targetSelection;
                    }
                }
            }

            // Invalidate old & new selection.
            ToolStripDesignerUtils.InvalidateSelection(origSel, targetSelection as ToolStripItem, _provider, _shiftPressed);
        }

        // reset the shiftPressed since we end selection
        _shiftPressed = false;
    }
}
