// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms.Design.Behavior;

namespace System.Windows.Forms.Design;

/// <summary>
///  This internal class wraps the InSitu Editor. The editor is a runtime ToolStrip control which contains a leftButton
///  (for image), centerLabel (for text) which gets swapped by a centerTextBox (when InSitu is ON).
///  The ToolStripTemplateNode is also responsible for intercepting the Escape and Enter keys and implements
///  the IMenuStatusHandler so that it can commit and rollback as required. Finally this ToolStripTemplateNode
///  has a private class ItemTypeToolStripMenuItem for adding ToolStripItem types to the Dropdown for addItemButton.
/// </summary>
internal class ToolStripTemplateNode : IMenuStatusHandler
{
    private const int GLYPHBORDER = 1;
    private const int GLYPHINSET = 2;

    private const int TOOLSTRIP_TEMPLATE_HEIGHT_ORIGINAL = 22;
    private const int TEMPLATE_HEIGHT_ORIGINAL = 19;
    private const int TOOLSTRIP_TEMPLATE_WIDTH_ORIGINAL = 120;
    private const int TEMPLATE_WIDTH_ORIGINAL = 31;
    private const int MINITOOLSTRIP_DROPDOWN_BUTTON_WIDTH_ORIGINAL = 11;
    private const int TEMPLATE_HOTREGION_WIDTH_ORIGINAL = 9;
    private const int MINITOOLSTRIP_TEXTBOX_WIDTH_ORIGINAL = 90;

    private static int s_toolStripTempateHeight = TOOLSTRIP_TEMPLATE_HEIGHT_ORIGINAL;
    private static int s_templateHeight = TEMPLATE_HEIGHT_ORIGINAL;
    private static int s_toolStripTemplateWidth = TOOLSTRIP_TEMPLATE_WIDTH_ORIGINAL;
    private static int s_templateWidth = TEMPLATE_WIDTH_ORIGINAL;
    private static int s_miniToolStripDropDownButtonWidth = MINITOOLSTRIP_DROPDOWN_BUTTON_WIDTH_ORIGINAL;
    private static int s_templateHotRegionWidth = TEMPLATE_HOTREGION_WIDTH_ORIGINAL;
    private static int s_miniToolStripTextBoxWidth = MINITOOLSTRIP_TEXTBOX_WIDTH_ORIGINAL;

    private static bool s_isScalingInitialized;
    internal const string CenterLabelName = "centerLabel";

    // Component for this InSitu Editor... (this is a ToolStripItem) that wants to go into InSitu
    private readonly IComponent _component;
    // Current Designer for the component that in InSitu mode
    private IDesigner _designer;
    // Get DesignerHost.
    private readonly IDesignerHost _designerHost;
    // Menu Commands to override
    private readonly MenuCommand[] _commands;
    // MenuCommands to Add
    private readonly MenuCommand[] _addCommands;
    // Actual InSitu Editor and its components...
    private TransparentToolStrip _miniToolStrip;
    // Center Label for MenuStrip TemplateNode
    private ToolStripLabel _centerLabel;
    // SplitButton reAdded for ToolStrip specific TemplateNode
    private ToolStripSplitButton _addItemButton;
    // swapped in text...
    private ToolStripControlHost _centerTextBox;

    // reqd as rtb does accept Enter..
    internal bool _ignoreFirstKeyUp;

    // This is the Bounding Rectangle for the ToolStripTemplateNode.
    // This is set by the itemDesigner in terms of the "AdornerWindow" bounds.
    // The ToolStripEditorManager uses this Bounds to actually activate the editor on the AdornerWindow.
    private Rectangle _boundingRect;
    // Keeps track of InSitu Mode.
    private bool _inSituMode;
    // Tells whether the editorNode is listening to Menu commands.
    private bool _active;

    // Need to keep a track of Last Selection to uncheck it. This is the Checked property on ToolStripItems on the Menu.
    // We check this cached in value to the current Selection on the addItemButton and if different then
    // uncheck the Checked for this lastSelection.. Check for the currentSelection and finally save the currentSelection
    // as the lastSelection for future check.
    private ItemTypeToolStripMenuItem _lastSelection;

    // This is the renderer used to Draw the Strips.....
    private MiniToolStripRenderer _renderer;
    // This is the Type that the user has selected for the new Item
    private Type _itemType;
    // Get the ToolStripKeyBoardService to notify that the TemplateNode is Active and so it shouldn't process the KeyMessages.
    private ToolStripKeyboardHandlingService _toolStripKeyBoardService;

    // Cached ISelectionService
    private ISelectionService _selectionService;
    // Cached BehaviorService
    private BehaviorService _behaviorService;
    // ControlHost for selection on mouseclicks
    private DesignerToolStripControlHost _controlHost;
    // On DropDowns the component passed in is the parent (ownerItem) and hence we need the reference for actual item
    private ToolStripItem _activeItem;

    private EventHandler _onActivated;
    private EventHandler _onClosed;
    private EventHandler _onDeactivated;
    private MenuCommand _oldUndoCommand;
    private MenuCommand _oldRedoCommand;
    // The DropDown for the TemplateNode
    private NewItemsContextMenuStrip _contextMenu;
    // the Hot Region within the templateNode ... this is used for the menustrips
    private Rectangle _hotRegion;

    private bool _imeModeSet;
    // DesignSurface to hook up to the Flushed event
    private DesignSurface _designSurface;
    // Is system context menu displayed for the InSitu text box?
    private bool _isSystemContextMenuDisplayed;
    // delay population of custom menu items until ready to open the drop down
    private bool _isPopulated;

    public ToolStripTemplateNode(IComponent component, string text)
    {
        _component = component;

        // In most of the cases this is true; except for ToolStripItems on DropDowns. the toolstripMenuItemDesigners
        // sets the public property in those cases.
        _activeItem = component as ToolStripItem;
        _designerHost = component.Site.GetService<IDesignerHost>();
        _designer = _designerHost.GetDesigner(component);
        _designSurface = component.Site.GetService<DesignSurface>();
        if (_designSurface is not null)
        {
            _designSurface.Flushed += OnLoaderFlushed;
        }

        if (!s_isScalingInitialized)
        {
            // Dimensions of the "Type Here" text box.
            s_toolStripTempateHeight = ScaleHelper.ScaleToInitialSystemDpi(TOOLSTRIP_TEMPLATE_HEIGHT_ORIGINAL);
            s_templateHeight = ScaleHelper.ScaleToInitialSystemDpi(TEMPLATE_HEIGHT_ORIGINAL);
            s_toolStripTemplateWidth = ScaleHelper.ScaleToInitialSystemDpi(TOOLSTRIP_TEMPLATE_WIDTH_ORIGINAL);
            s_templateWidth = ScaleHelper.ScaleToInitialSystemDpi(TEMPLATE_WIDTH_ORIGINAL);
            // The hot region is the arrow button next to "Type Here" box.
            s_templateHotRegionWidth = ScaleHelper.ScaleToInitialSystemDpi(TEMPLATE_HOTREGION_WIDTH_ORIGINAL);
            s_miniToolStripDropDownButtonWidth = ScaleHelper.ScaleToInitialSystemDpi(MINITOOLSTRIP_DROPDOWN_BUTTON_WIDTH_ORIGINAL);
            s_miniToolStripTextBoxWidth = ScaleHelper.ScaleToInitialSystemDpi(MINITOOLSTRIP_TEXTBOX_WIDTH_ORIGINAL);

            s_isScalingInitialized = true;
        }

        SetupNewEditNode(this, text, component);
        _commands = [];
        _addCommands = [];
    }

    /// <summary>
    ///  This property enables / disables Menu Command Handler.
    /// </summary>
    public bool Active
    {
        get => _active;
        set
        {
            if (_active != value)
            {
                _active = value;

                if (KeyboardService is not null)
                {
                    KeyboardService.TemplateNodeActive = value;
                }

                if (_active)
                {
                    // Active.. Fire Activated
                    OnActivated(new EventArgs());
                    if (KeyboardService is not null)
                    {
                        KeyboardService.ActiveTemplateNode = this;
                    }

                    IMenuCommandService menuService = (IMenuCommandService)_component.Site.GetService(typeof(IMenuCommandService));
                    if (menuService is not null)
                    {
                        _oldUndoCommand = menuService.FindCommand(StandardCommands.Undo);
                        if (_oldUndoCommand is not null)
                        {
                            menuService.RemoveCommand(_oldUndoCommand);
                        }

                        _oldRedoCommand = menuService.FindCommand(StandardCommands.Redo);
                        if (_oldRedoCommand is not null)
                        {
                            menuService.RemoveCommand(_oldRedoCommand);
                        }

                        // Disable the Commands
                        for (int i = 0; i < _addCommands.Length; i++)
                        {
                            _addCommands[i].Enabled = false;
                            menuService.AddCommand(_addCommands[i]);
                        }
                    }

                    // Listen to command and key events
                    IEventHandlerService ehs = (IEventHandlerService)_component.Site.GetService(typeof(IEventHandlerService));
                    ehs?.PushHandler(this);
                }
                else
                {
                    OnDeactivated(new EventArgs());
                    if (KeyboardService is not null)
                    {
                        KeyboardService.ActiveTemplateNode = null;
                    }

                    IMenuCommandService menuService = (IMenuCommandService)_component.Site.GetService(typeof(IMenuCommandService));
                    if (menuService is not null)
                    {
                        for (int i = 0; i < _addCommands.Length; i++)
                        {
                            menuService.RemoveCommand(_addCommands[i]);
                        }
                    }

                    if (_oldUndoCommand is not null)
                    {
                        menuService.AddCommand(_oldUndoCommand);
                    }

                    if (_oldRedoCommand is not null)
                    {
                        menuService.AddCommand(_oldRedoCommand);
                    }

                    // Stop listening to command and key events
                    IEventHandlerService ehs = (IEventHandlerService)_component.Site.GetService(typeof(IEventHandlerService));
                    ehs?.PopHandler(this);
                }
            }
        }
    }

    // Need to have a reference of the actual item that is edited.
    public ToolStripItem ActiveItem
    {
        get => _activeItem;
        set => _activeItem = value;
    }

    public event EventHandler Activated
    {
        add => _onActivated += value;
        remove => _onActivated -= value;
    }

    /// <summary>
    ///  Returns the Bounds of this ToolStripTemplateNode.
    /// </summary>
    public Rectangle Bounds
    {
        get => _boundingRect;
        set => _boundingRect = value;
    }

    public DesignerToolStripControlHost ControlHost
    {
        get => _controlHost;
        set => _controlHost = value;
    }

    /// <summary>
    ///  This is the designer contextMenu that pops when rightclicked on the TemplateNode.
    /// </summary>
    private ContextMenuStrip DesignerContextMenu
    {
        get
        {
            BaseContextMenuStrip templateNodeContextMenu = new(_component.Site)
            {
                Populated = false
            };
            templateNodeContextMenu.GroupOrdering.Clear();
            templateNodeContextMenu.GroupOrdering.AddRange([StandardGroups.Code, StandardGroups.Custom, StandardGroups.Selection, StandardGroups.Edit]);
            templateNodeContextMenu.Text = "CustomContextMenu";

            TemplateNodeCustomMenuItemCollection templateNodeCustomMenuItemCollection = new(_component.Site, _controlHost);
            foreach (ToolStripItem item in templateNodeCustomMenuItemCollection)
            {
                templateNodeContextMenu.Groups[StandardGroups.Custom].Items.Add(item);
            }

            return templateNodeContextMenu;
        }
    }

    public event EventHandler Deactivated
    {
        add => _onDeactivated += value;
        remove => _onDeactivated -= value;
    }

    public event EventHandler Closed
    {
        add => _onClosed += value;
        remove => _onClosed -= value;
    }

    /// <summary>
    ///  This property returns the actual editor ToolStrip.
    /// </summary>
    public ToolStrip EditorToolStrip
    {
        get => _miniToolStrip;
    }

    /// <summary>
    ///  This property returns the actual editor ToolStrip.
    /// </summary>
    internal TextBox EditBox
    {
        get => (_centerTextBox is not null) ? (TextBox)_centerTextBox.Control : null;
    }

    /// <summary>
    ///  HotRegion within the templateNode. this is the region which responds to the mouse.
    /// </summary>
    public Rectangle HotRegion
    {
        get => _hotRegion;
        set => _hotRegion = value;
    }

    /// <summary>
    ///  value to suggest if IME mode is set.
    /// </summary>
    public bool IMEModeSet
    {
        get => _imeModeSet;
        set => _imeModeSet = value;
    }

    /// <summary>
    ///  KeyBoardHandling service.
    /// </summary>
    private ToolStripKeyboardHandlingService KeyboardService
    {
        get
        {
            _toolStripKeyBoardService ??= (ToolStripKeyboardHandlingService)_component.Site.GetService(typeof(ToolStripKeyboardHandlingService));

            return _toolStripKeyBoardService;
        }
    }

    /// <summary>
    ///  SelectionService.
    /// </summary>
    private ISelectionService SelectionService
    {
        get
        {
            _selectionService ??= (ISelectionService)_component.Site.GetService(typeof(ISelectionService));

            return _selectionService;
        }
    }

    private BehaviorService BehaviorService
    {
        get
        {
            _behaviorService ??= (BehaviorService)_component.Site.GetService(typeof(BehaviorService));

            return _behaviorService;
        }
    }

    /// <summary>
    ///  Type of the new Item to be added.
    /// </summary>
    public Type ToolStripItemType
    {
        get => _itemType;
        set => _itemType = value;
    }

    /// <summary>
    ///  Is system context menu for the InSitu edit box displayed?.
    /// </summary>
    internal bool IsSystemContextMenuDisplayed
    {
        get => _isSystemContextMenuDisplayed;
        set => _isSystemContextMenuDisplayed = value;
    }

    /// <summary>
    ///  Helper function to add new Item when the DropDownItem (in the ToolStripTemplateNode) is clicked
    /// </summary>
    private void AddNewItemClick(object sender, EventArgs e)
    {
        // Close the DropDown.. Important for Morphing ....
        if (_addItemButton is not null)
        {
            _addItemButton.DropDown.Visible = false;
        }

        if (_component is ToolStrip && SelectionService is not null)
        {
            // Stop the Designer from closing the Overflow if its open
            ToolStripDesigner designer = _designerHost.GetDesigner(_component) as ToolStripDesigner;
            try
            {
                if (designer is not null)
                {
                    designer.DontCloseOverflow = true;
                }

                SelectionService.SetSelectedComponents(new object[] { _component });
            }
            finally
            {
                if (designer is not null)
                {
                    designer.DontCloseOverflow = false;
                }
            }
        }

        ItemTypeToolStripMenuItem senderItem = (ItemTypeToolStripMenuItem)sender;
        if (_lastSelection is not null)
        {
            _lastSelection.Checked = false;
        }

        // set the appropriate Checked state
        senderItem.Checked = true;
        _lastSelection = senderItem;
        // Set the property used in the CommitEditor (.. ) to add the correct Type.
        ToolStripItemType = senderItem.ItemType;
        // Select the parent before adding
        ToolStrip parent = _controlHost.GetCurrentParent();
        // this will add the item to the ToolStrip..
        if (parent is MenuStrip)
        {
            CommitEditor(true, true, false);
        }
        else
        {
            // In case of toolStrips/StatusStrip we want the currently added item to be selected instead of selecting the next item
            CommitEditor(true, false, false);
        }

        if (KeyboardService is not null)
        {
            KeyboardService.TemplateNodeActive = false;
        }
    }

    /// <summary>
    ///  Called when the user clicks the CenterLabel of the ToolStripTemplateNode.
    /// </summary>
    private void CenterLabelClick(object sender, MouseEventArgs e)
    {
        // For Right Button we show the DesignerContextMenu...
        if (e.Button == MouseButtons.Right)
        {
            // Don't show the DesignerContextMenu if there is any active templateNode.
            if (KeyboardService is not null && KeyboardService.TemplateNodeActive)
            {
                return;
            }

            if (KeyboardService is not null)
            {
                KeyboardService.SelectedDesignerControl = _controlHost;
            }

            SelectionService.SetSelectedComponents(null, SelectionTypes.Replace);
            if (BehaviorService is not null)
            {
                Point loc = BehaviorService.ControlToAdornerWindow(_miniToolStrip);
                loc = BehaviorService.AdornerWindowPointToScreen(loc);
                loc.Offset(e.Location);
                DesignerContextMenu.Show(loc);
            }
        }
        else
        {
            if (_hotRegion.Contains(e.Location) && !KeyboardService.TemplateNodeActive)
            {
                if (KeyboardService is not null)
                {
                    KeyboardService.SelectedDesignerControl = _controlHost;
                }

                SelectionService.SetSelectedComponents(null, SelectionTypes.Replace);
                ToolStripDropDown oldContextMenu = _contextMenu;

                // PERF: Consider refresh mechanism for the derived items.
                if (oldContextMenu is not null)
                {
                    oldContextMenu.Closed -= OnContextMenuClosed;
                    oldContextMenu.Closing -= OnContextMenuClosing;
                    oldContextMenu.Opened -= OnContextMenuOpened;
                    oldContextMenu.Dispose();
                }

                _contextMenu = null;
                ShowDropDownMenu();
            }
            else
            {
                // Remember the click position.
                ToolStripDesigner.s_lastCursorPosition = Cursor.Position;

                if (_designer is ToolStripDesigner designer)
                {
                    if (KeyboardService.TemplateNodeActive)
                    {
                        KeyboardService.ActiveTemplateNode.Commit(false, false);
                    }

                    // cause a selectionChange...
                    if (SelectionService.PrimarySelection is null)
                    {
                        SelectionService.SetSelectedComponents(new object[] { _component }, SelectionTypes.Replace);
                    }

                    KeyboardService.SelectedDesignerControl = _controlHost;
                    SelectionService.SetSelectedComponents(null, SelectionTypes.Replace);
                    designer.ShowEditNode(true);
                }

                if (_designer is ToolStripMenuItemDesigner itemDesigner)
                {
                    // cache the serviceProvider (Site) since the component can potential get disposed after the call to CommitAndSelect();
                    IServiceProvider svcProvider = _component.Site;
                    // Commit any InsituEdit Node.
                    if (KeyboardService.TemplateNodeActive)
                    {
                        if (_component is ToolStripItem currentItem)
                        {
                            // We have clicked the TemplateNode of a visible Item .. so just commit the current InSitu...
                            if (currentItem.Visible)
                            {
                                // If templateNode Active .. commit
                                KeyboardService.ActiveTemplateNode.Commit(false, false);
                            }
                            else // we have clicked the templateNode of a Invisible Item ...
                                 // so a dummyItem. In this case select the item.
                            {
                                // If templateNode Active .. commit and Select
                                KeyboardService.ActiveTemplateNode.Commit(false, true);
                            }
                        }
                        else // If Component is not a ToolStripItem
                        {
                            KeyboardService.ActiveTemplateNode.Commit(false, false);
                        }
                    }

                    if (_designer is not null)
                    {
                        itemDesigner.EditTemplateNode(true);
                    }
                    else
                    {
                        ISelectionService cachedSelSvc = (ISelectionService)svcProvider.GetService(typeof(ISelectionService));
                        if (cachedSelSvc.PrimarySelection is ToolStripItem selectedItem && _designerHost is not null)
                        {
                            if (_designerHost.GetDesigner(selectedItem) is ToolStripMenuItemDesigner menuItemDesigner)
                            {
                                // Invalidate the item only if its toplevel.
                                if (!selectedItem.IsOnDropDown)
                                {
                                    Rectangle bounds = menuItemDesigner.GetGlyphBounds();
                                    ToolStripDesignerUtils.GetAdjustedBounds(selectedItem, ref bounds);
                                    if (svcProvider.GetService(typeof(BehaviorService)) is BehaviorService bSvc)
                                    {
                                        bSvc.Invalidate(bounds);
                                    }
                                }

                                menuItemDesigner.EditTemplateNode(true);
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    ///  Painting of the templateNode on MouseEnter.
    /// </summary>
    private void CenterLabelMouseEnter(object sender, EventArgs e)
    {
        if (_renderer is not null && !KeyboardService.TemplateNodeActive)
        {
            if (_renderer.State != (int)TemplateNodeSelectionState.HotRegionSelected)
            {
                _renderer.State = (int)TemplateNodeSelectionState.MouseOverLabel;
                _miniToolStrip.Invalidate();
            }
        }
    }

    /// <summary>
    ///  Painting of the templateNode on MouseMove
    /// </summary>
    private void CenterLabelMouseMove(object sender, MouseEventArgs e)
    {
        if (_renderer is not null && !KeyboardService.TemplateNodeActive)
        {
            if (_renderer.State != (int)TemplateNodeSelectionState.HotRegionSelected)
            {
                if (_hotRegion.Contains(e.Location))
                {
                    _renderer.State = (int)TemplateNodeSelectionState.MouseOverHotRegion;
                }
                else
                {
                    _renderer.State = (int)TemplateNodeSelectionState.MouseOverLabel;
                }

                _miniToolStrip.Invalidate();
            }
        }
    }

    /// <summary>
    ///  Painting of the templateNode on MouseLeave
    /// </summary>
    private void CenterLabelMouseLeave(object sender, EventArgs e)
    {
        if (_renderer is not null && !KeyboardService.TemplateNodeActive)
        {
            if (_renderer.State != (int)TemplateNodeSelectionState.HotRegionSelected)
            {
                _renderer.State = (int)TemplateNodeSelectionState.None;
            }

            if (KeyboardService is not null && KeyboardService.SelectedDesignerControl == _controlHost)
            {
                _renderer.State = (int)TemplateNodeSelectionState.TemplateNodeSelected;
            }

            _miniToolStrip.Invalidate();
        }
    }

    /// <summary>
    ///  Painting of the templateNode on MouseEnter
    /// </summary>
    private void CenterTextBoxMouseEnter(object sender, EventArgs e)
    {
        if (_renderer is not null)
        {
            _renderer.State = (int)TemplateNodeSelectionState.TemplateNodeSelected;
            _miniToolStrip.Invalidate();
        }
    }

    /// <summary>
    ///  Painting of the templateNode on TextBox mouseLeave (in case of MenuStrip)
    /// </summary>
    private void CenterTextBoxMouseLeave(object sender, EventArgs e)
    {
        if (_renderer is not null && !Active)
        {
            _renderer.State = (int)TemplateNodeSelectionState.None;
            _miniToolStrip.Invalidate();
        }
    }

    /// <summary>
    ///  This Internal function is called from the ToolStripItemDesigner to relinquish the resources used
    ///  by the EditorToolStrip. This Function disposes the ToolStrip and its components and also
    ///  clears the event handlers associated.
    /// </summary>
    internal void CloseEditor()
    {
        if (_miniToolStrip is not null)
        {
            Active = false;
            if (_lastSelection is not null)
            {
                _lastSelection.Dispose();
                _lastSelection = null;
            }

            if (_component is ToolStrip strip)
            {
                strip.RightToLeftChanged -= OnRightToLeftChanged;
            }
            else
            {
                if (_component is ToolStripDropDownItem stripItem)
                {
                    stripItem.RightToLeftChanged -= OnRightToLeftChanged;
                }
            }

            if (_centerLabel is not null)
            {
                _centerLabel.MouseUp -= CenterLabelClick;
                _centerLabel.MouseEnter -= CenterLabelMouseEnter;
                _centerLabel.MouseMove -= CenterLabelMouseMove;
                _centerLabel.MouseLeave -= CenterLabelMouseLeave;
                _centerLabel.Dispose();
                _centerLabel = null;
            }

            if (_addItemButton is not null)
            {
                _addItemButton.MouseMove -= OnMouseMove;
                _addItemButton.MouseUp -= OnMouseUp;
                _addItemButton.MouseDown -= OnMouseDown;
                _addItemButton.DropDownOpened -= OnAddItemButtonDropDownOpened;
                _addItemButton.DropDown.Dispose();
                _addItemButton.Dispose();
                _addItemButton = null;
            }

            if (_contextMenu is not null)
            {
                _contextMenu.Closed -= OnContextMenuClosed;
                _contextMenu.Closing -= OnContextMenuClosing;
                _contextMenu.Opened -= OnContextMenuOpened;
                _contextMenu = null;
            }

            _miniToolStrip.MouseLeave -= OnMouseLeave;
            _miniToolStrip.Dispose();
            _miniToolStrip = null;

            // Surface can be null. VS Whidbey #572862
            if (_designSurface is not null)
            {
                _designSurface.Flushed -= OnLoaderFlushed;
                _designSurface = null;
            }

            _designer = null;
            OnClosed(new EventArgs());
        }
    }

    /// <summary>
    ///  This internal Function is called by item designers to ROLLBACK the current InSitu editing mode.
    /// </summary>
    internal void Commit(bool enterKeyPressed, bool tabKeyPressed)
    {
        // Commit only if we are still available !!
        if (_miniToolStrip is not null && _inSituMode)
        {
            string text = ((TextBox)(_centerTextBox.Control)).Text;
            if (string.IsNullOrEmpty(text))
            {
                RollBack();
            }
            else
            {
                CommitEditor(true, enterKeyPressed, tabKeyPressed);
            }
        }
    }

    /// <summary>
    ///  Internal function that would commit the TemplateNode
    /// </summary>
    internal void CommitAndSelect()
    {
        Commit(false, false);
    }

    private void CommitTextToDesigner(string text, bool commit, bool enterKeyPressed, bool tabKeyPressed)
    {
        if (commit && (_designer is ToolStripDesigner || _designer is ToolStripMenuItemDesigner))
        {
            Type selectedType;
            // If user has typed in "-" then Add a Separator only on DropDowns.
            if (text == "-" && _designer is ToolStripMenuItemDesigner)
            {
                ToolStripItemType = typeof(ToolStripSeparator);
            }

            if (ToolStripItemType is not null)
            {
                selectedType = ToolStripItemType;
                ToolStripItemType = null;
            }
            else
            {
                Type[] supportedTypes = ToolStripDesignerUtils.GetStandardItemTypes(_component);
                selectedType = supportedTypes[0];
            }

            if (_designer is ToolStripDesigner designer)
            {
                designer.AddNewItem(selectedType, text, enterKeyPressed, tabKeyPressed);
            }
            else
            {
                ((ToolStripItemDesigner)_designer).CommitEdit(selectedType, text, commit, enterKeyPressed, tabKeyPressed);
            }
        }
        else if (_designer is ToolStripItemDesigner designer)
        {
            designer.CommitEdit(_designer.Component.GetType(), text, commit, enterKeyPressed, tabKeyPressed);
        }
    }

    /// <summary>
    ///  This private function performs the job of committing the current InSitu Editor.
    ///  This will call the CommitEdit(...) function for the appropriate designers so that they can actually do
    ///  their own Specific things for committing (or ROLLING BACK) the InSitu Edit mode.
    ///  The commit flag is used for commit or rollback. BE SURE TO ALWAYS call ExitInSituEdit
    ///  from this function to put the EditorToolStrip in a sane "NON EDIT" mode.
    /// </summary>
    private void CommitEditor(bool commit, bool enterKeyPressed, bool tabKeyPressed)
    {
        // After the node is committed the templateNode gets the selection.
        // But the original selection is not invalidated. consider following case FOO -> BAR -> TEMPLATENODE node
        // When the TemplateNode is committed "FOO" is selected but after the commit is complete.
        // The TemplateNode gets the selection but "FOO" is never invalidated and hence retains selection.
        // So we get the selection and then invalidate it at the end of this function.
        // Get the currentSelection to invalidate.
        string text = (_centerTextBox is not null) ? ((TextBox)(_centerTextBox.Control)).Text : string.Empty;
        ExitInSituEdit();
        FocusForm();
        CommitTextToDesigner(text, commit, enterKeyPressed, tabKeyPressed);
        // finally Invalidate the selection rect ...
        if (SelectionService.PrimarySelection is ToolStripItem curSel)
        {
            if (_designerHost is not null)
            {
                if (_designerHost.GetDesigner(curSel) is ToolStripItemDesigner designer)
                {
                    Rectangle invalidateBounds = designer.GetGlyphBounds();
                    ToolStripDesignerUtils.GetAdjustedBounds(curSel, ref invalidateBounds);
                    invalidateBounds.Inflate(GLYPHBORDER, GLYPHBORDER);
                    Region rgn = new(invalidateBounds);
                    invalidateBounds.Inflate(-GLYPHINSET, -GLYPHINSET);
                    rgn.Exclude(invalidateBounds);
                    BehaviorService?.Invalidate(rgn);

                    rgn.Dispose();
                }
            }
        }
    }

    /// <summary>
    ///  The ToolStripTemplateNode enters into InSitu Edit Mode through this Function.
    ///  This Function is called by FocusEditor( ) which starts the InSitu.
    ///  The centerLabel is SWAPPED by centerTextBox and the ToolStripTemplateNode is Ready for Text.
    ///  Setting "Active = true" pushes the IEventHandler which now intercepts the Escape and Enter keys to ROLLBACK or
    ///  COMMIT the InSitu Editing respectively.
    /// </summary>
    private void EnterInSituEdit()
    {
        if (!_inSituMode)
        {
            // Listen For Commands....
            _miniToolStrip.Parent?.SuspendLayout();

            try
            {
                Active = true;
                _inSituMode = true;
                // set the renderer state to Selected...
                if (_renderer is not null)
                {
                    _renderer.State = (int)TemplateNodeSelectionState.TemplateNodeSelected;
                }

                // Set UP textBox for InSitu
                TextBox tb = new TemplateTextBox(_miniToolStrip, this)
                {
                    BorderStyle = BorderStyle.FixedSingle,
                    Text = _centerLabel.Text,
                    ForeColor = SystemColors.WindowText
                };
                _centerTextBox = new ToolStripControlHost(tb)
                {
                    Dock = DockStyle.None,
                    AutoSize = false,
                    Width = s_miniToolStripTextBoxWidth
                };

                if (_activeItem is ToolStripDropDownItem item && !item.IsOnDropDown)
                {
                    _centerTextBox.Margin = new Padding(1, 2, 1, 3);
                }
                else
                {
                    _centerTextBox.Margin = new Padding(1);
                }

                _centerTextBox.Size = _miniToolStrip.DisplayRectangle.Size - _centerTextBox.Margin.Size;
                _centerTextBox.Name = "centerTextBox";
                _centerTextBox.MouseEnter += CenterTextBoxMouseEnter;
                _centerTextBox.MouseLeave += CenterTextBoxMouseLeave;
                int index = _miniToolStrip.Items.IndexOf(_centerLabel);
                // swap in our InSitu textbox
                if (index != -1)
                {
                    _miniToolStrip.Items.Insert(index, _centerTextBox);
                    _miniToolStrip.Items.Remove(_centerLabel);
                }

                tb.KeyUp += OnKeyUp;
                tb.KeyDown += OnKeyDown;
                tb.SelectAll();
                Control baseComponent = null;
                if (_designerHost is not null)
                {
                    baseComponent = (Control)_designerHost.RootComponent;
                    PInvokeCore.SendMessage(baseComponent, PInvokeCore.WM_SETREDRAW, (WPARAM)(BOOL)false);
                    tb.Focus();
                    PInvokeCore.SendMessage(baseComponent, PInvokeCore.WM_SETREDRAW, (WPARAM)(BOOL)true);
                }
            }
            finally
            {
                _miniToolStrip.Parent?.ResumeLayout();
            }
        }
    }

    /// <summary>
    ///  The ToolStripTemplateNode exits from InSitu Edit Mode through this Function.
    ///  This Function is called by CommitEditor( ) which stops the InSitu.
    ///  The centerTextBox is SWAPPED by centerLabel and the ToolStripTemplateNode is exits the InSitu Mode.
    ///  Setting "Active = false" pops the IEventHandler.
    /// </summary>
    private void ExitInSituEdit()
    {
        // put the ToolStripTemplateNode back into "non edit state"
        if (_centerTextBox is not null && _inSituMode)
        {
            _miniToolStrip.Parent?.SuspendLayout();

            try
            {
                // if going InSitu with a real item, set & select all the text
                int index = _miniToolStrip.Items.IndexOf(_centerTextBox);
                // validate index
                if (index != -1)
                {
                    _centerLabel.Text = SR.ToolStripDesignerTemplateNodeEnterText;
                    // swap in our InSitu textbox
                    _miniToolStrip.Items.Insert(index, _centerLabel);
                    _miniToolStrip.Items.Remove(_centerTextBox);
                    ((TextBox)(_centerTextBox.Control)).KeyUp -= OnKeyUp;
                    ((TextBox)(_centerTextBox.Control)).KeyDown -= OnKeyDown;
                }

                _centerTextBox.MouseEnter -= CenterTextBoxMouseEnter;
                _centerTextBox.MouseLeave -= CenterTextBoxMouseLeave;
                _centerTextBox.Dispose();
                _centerTextBox = null;
                _inSituMode = false;
                // reset the Size....
                SetWidth(null);
            }
            finally
            {
                _miniToolStrip.Parent?.ResumeLayout();

                // POP of the Handler !!!
                Active = false;
            }
        }
    }

    /// <summary>
    ///  This internal function is called from ToolStripItemDesigner to put the current item into InSitu Edit Mode.
    /// </summary>
    internal void FocusEditor(ToolStripItem currentItem)
    {
        if (currentItem is not null)
        {
            _centerLabel.Text = currentItem.Text;
        }

        EnterInSituEdit();
    }

    /// <summary>
    ///  Called when the user enters into the InSitu edit mode.This keeps the fdesigner Form Active.....
    /// </summary>
    private void FocusForm()
    {
        if (_component.Site.GetService(typeof(ISplitWindowService)) is DesignerFrame designerFrame
            && _designerHost is not null)
        {
            Control baseComponent = (Control)_designerHost.RootComponent;
            PInvokeCore.SendMessage(baseComponent, PInvokeCore.WM_SETREDRAW, (WPARAM)(BOOL)false);
            designerFrame.Focus();
            PInvokeCore.SendMessage(baseComponent, PInvokeCore.WM_SETREDRAW, (WPARAM)(BOOL)true);
        }
    }

    protected void OnActivated(EventArgs e)
    {
        _onActivated?.Invoke(this, e);
    }

    private void OnAddItemButtonDropDownOpened(object sender, EventArgs e)
    {
        _addItemButton.DropDown.Focus();
    }

    protected void OnClosed(EventArgs e)
    {
        _onClosed?.Invoke(this, e);
    }

    /// <summary>
    ///  Painting of the templateNode on when the contextMenu is closed
    /// </summary>
    private void OnContextMenuClosed(object sender, ToolStripDropDownClosedEventArgs e)
    {
        if (_renderer is not null)
        {
            _renderer.State = (int)TemplateNodeSelectionState.TemplateNodeSelected;
            _miniToolStrip.Invalidate();
        }
    }

    private void OnContextMenuClosing(object sender, ToolStripDropDownClosingEventArgs e)
    {
        if (_addItemButton is null)
        {
            _miniToolStrip.RaiseStateChangeEvent();
        }
    }

    /// <summary>
    ///  Set the KeyBoardService member, so the designer knows that the "ContextMenu" is opened.
    /// </summary>
    private void OnContextMenuOpened(object sender, EventArgs e)
    {
        // Disable All Commands .. the Commands would be reenabled by AddNewItemClick call.
        if (KeyboardService is not null)
        {
            KeyboardService.TemplateNodeContextMenuOpen = true;
        }
    }

    protected void OnDeactivated(EventArgs e)
    {
        _onDeactivated?.Invoke(this, e);
    }

    /// <summary>
    ///  Called by the design surface when it is being flushed. This will save any changes made to TemplateNode.
    /// </summary>
    private void OnLoaderFlushed(object sender, EventArgs e)
    {
        Commit(false, false);
    }

    /// <summary>
    ///  This is small HACK. For some reason if the InSituEditor's textbox has focus the escape key is lost and
    ///  the menu service doesn't get it.... but the textbox gets it. So need to check for the escape key here and
    ///  call CommitEditor(false) which will ROLLBACK the edit.
    /// </summary>
    private void OnKeyUp(object sender, KeyEventArgs e)
    {
        if (IMEModeSet)
        {
            return;
        }

        switch (e.KeyCode)
        {
            case Keys.Up:
                Commit(false, true);
                KeyboardService?.ProcessUpDown(false);

                break;
            case Keys.Down:
                Commit(true, false);
                break;
            case Keys.Escape:
                CommitEditor(false, false, false);
                break;
            case Keys.Return:
                if (_ignoreFirstKeyUp)
                {
                    _ignoreFirstKeyUp = false;
                    return;
                }

                OnKeyDefaultAction(sender, e);
                break;
        }
    }

    /// <summary>
    ///  Select text on KeyDown.
    /// </summary>
    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        if (IMEModeSet)
        {
            return;
        }

        if (e.KeyCode == Keys.A && (e.KeyData & Keys.Control) != 0)
        {
            if (sender is TextBox t)
            {
                t.SelectAll();
            }
        }
    }

    /// <summary>
    ///  Check for the Enter key here and call CommitEditor(true) which will COMMIT the edit.
    /// </summary>
    private void OnKeyDefaultAction(object sender, EventArgs e)
    {
        // exit InSitu with committing....
        Active = false;
        Debug.Assert(_centerTextBox.Control is not null, "The TextBox is null");
        if (_centerTextBox.Control is not null)
        {
            string text = ((TextBox)(_centerTextBox.Control)).Text;
            if (string.IsNullOrEmpty(text))
            {
                CommitEditor(false, false, false);
            }
            else
            {
                CommitEditor(true, true, false);
            }
        }
    }

    /// <summary>
    ///  Called when the delete menu item is selected.
    /// </summary>
    private void OnMenuCut(object sender, EventArgs e)
    {
    }

    /// <summary>
    ///  Show ContextMenu if the Right Mouse button was pressed and we have received the following MouseUp
    /// </summary>
    private void OnMouseUp(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Right)
        {
            if (BehaviorService is not null)
            {
                Point loc = BehaviorService.ControlToAdornerWindow(_miniToolStrip);
                loc = BehaviorService.AdornerWindowPointToScreen(loc);
                loc.Offset(e.Location);
                DesignerContextMenu.Show(loc);
            }
        }
    }

    /// <summary>
    ///  Set the selection to the component.
    /// </summary>
    private void OnMouseDown(object sender, MouseEventArgs e)
    {
        if (KeyboardService is not null)
        {
            KeyboardService.SelectedDesignerControl = _controlHost;
        }

        SelectionService.SetSelectedComponents(null, SelectionTypes.Replace);
    }

    /// <summary>
    ///  Painting on the button for mouse Move.
    /// </summary>
    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        _renderer.State = (int)TemplateNodeSelectionState.None;
        if (_renderer is not null)
        {
            if (_addItemButton is not null)
            {
                if (_addItemButton.ButtonBounds.Contains(e.Location))
                {
                    _renderer.State = (int)TemplateNodeSelectionState.SplitButtonSelected;
                }
                else if (_addItemButton.DropDownButtonBounds.Contains(e.Location))
                {
                    _renderer.State = (int)TemplateNodeSelectionState.DropDownSelected;
                }
            }

            _miniToolStrip.Invalidate();
        }
    }

    /// <summary>
    ///  Painting on the button for mouse Leave.
    /// </summary>
    private void OnMouseLeave(object sender, EventArgs e)
    {
        if (SelectionService is not null)
        {
            if (SelectionService.PrimarySelection is ToolStripItem && _renderer is not null && _renderer.State != (int)TemplateNodeSelectionState.HotRegionSelected)
            {
                _renderer.State = (int)TemplateNodeSelectionState.None;
            }

            if (KeyboardService is not null && KeyboardService.SelectedDesignerControl == _controlHost)
            {
                _renderer.State = (int)TemplateNodeSelectionState.TemplateNodeSelected;
            }

            _miniToolStrip.Invalidate();
        }
    }

    private void OnRightToLeftChanged(object sender, EventArgs e)
    {
        if (sender is ToolStrip strip)
        {
            _miniToolStrip.RightToLeft = strip.RightToLeft;
        }
        else
        {
            ToolStripDropDownItem stripItem = sender as ToolStripDropDownItem;
            _miniToolStrip.RightToLeft = stripItem.RightToLeft;
        }
    }

    /// <summary>
    ///  Intercept invocation of specific commands and keys
    /// </summary>
    public bool OverrideInvoke(MenuCommand cmd)
    {
        for (int i = 0; i < _commands.Length; i++)
        {
            if (_commands[i].CommandID.Equals(cmd.CommandID))
            {
                if (cmd.CommandID == StandardCommands.Delete || cmd.CommandID == StandardCommands.Cut || cmd.CommandID == StandardCommands.Copy)
                {
                    _commands[i].Invoke();
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    ///  Intercept invocation of specific commands and keys
    /// </summary>
    public bool OverrideStatus(MenuCommand cmd)
    {
        for (int i = 0; i < _commands.Length; i++)
        {
            if (_commands[i].CommandID.Equals(cmd.CommandID))
            {
                cmd.Enabled = false;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    ///  This internal Function is called by item designers to ROLLBACK the current InSitu editing mode.
    /// </summary>
    internal void RollBack()
    {
        // RollBack only iff we are still available !!
        if (_miniToolStrip is not null && _inSituMode)
        {
            CommitEditor(false, false, false);
        }
    }

    internal void ShowContextMenu(Point pt)
    {
        DesignerContextMenu.Show(pt);
    }

    internal void ShowDropDownMenu()
    {
        if (_addItemButton is not null)
        {
            if (!_isPopulated)
            {
                _isPopulated = true;
                ToolStripDesignerUtils.GetCustomNewItemDropDown(_contextMenu, _component, null, new EventHandler(AddNewItemClick), false, _component.Site);
            }

            _addItemButton.ShowDropDown();
        }
        else
        {
            if (BehaviorService is not null)
            {
                Point loc = BehaviorService.ControlToAdornerWindow(_miniToolStrip);
                loc = BehaviorService.AdornerWindowPointToScreen(loc);
                Rectangle translatedBounds = new(loc, _miniToolStrip.Size);
                _miniToolStrip.RaiseStateChangeEvent();

                if (_contextMenu is null)
                {
                    _isPopulated = true;
                    _contextMenu = ToolStripDesignerUtils.GetNewItemDropDown(
                        _component,
                        currentItem: null,
                        AddNewItemClick,
                        convertTo: false,
                        _component.Site,
                        populateCustom: true);

                    _contextMenu.Closed += OnContextMenuClosed;
                    _contextMenu.Closing += OnContextMenuClosing;
                    _contextMenu.Opened += OnContextMenuOpened;
                    _contextMenu.Text = "ItemSelectionMenu";
                }
                else if (!_isPopulated)
                {
                    _isPopulated = true;
                    ToolStripDesignerUtils.GetCustomNewItemDropDown(
                        _contextMenu,
                        _component,
                        currentItem: null,
                        AddNewItemClick,
                        convertTo: false,
                        _component.Site);
                }

                if (_component is ToolStrip strip)
                {
                    _contextMenu.RightToLeft = strip.RightToLeft;
                }
                else
                {
                    if (_component is ToolStripDropDownItem stripItem)
                    {
                        _contextMenu.RightToLeft = stripItem.RightToLeft;
                    }
                }

                _contextMenu.Show(translatedBounds.X, translatedBounds.Y + translatedBounds.Height);
                _contextMenu.Focus();
                if (_renderer is not null)
                {
                    _renderer.State = (int)TemplateNodeSelectionState.HotRegionSelected;
                    _miniToolStrip.Invalidate();
                }
            }
        }
    }

    /// <summary>
    ///  This function sets up the MenuStrip specific TemplateNode.
    /// </summary>
    private void SetUpMenuTemplateNode(string text, IComponent currentItem)
    {
        _centerLabel = new ToolStripLabel
        {
            Text = text,
            AutoSize = false,
            IsLink = false,
            AccessibleDescription = SR.ToolStripDesignerTemplateNodeLabelToolTip,
            AccessibleRole = AccessibleRole.Text,

            Margin = new Padding(1)
        };

        if (currentItem is ToolStripDropDownItem)
        {
            _centerLabel.Margin = new Padding(1, 2, 1, 3);
        }

        _centerLabel.Padding = new Padding(0, 1, 0, 0);
        _centerLabel.Name = CenterLabelName;
        _centerLabel.Size = _miniToolStrip.DisplayRectangle.Size - _centerLabel.Margin.Size;
        _centerLabel.ToolTipText = SR.ToolStripDesignerTemplateNodeLabelToolTip;
        _centerLabel.MouseUp += CenterLabelClick;
        _centerLabel.MouseEnter += CenterLabelMouseEnter;
        _centerLabel.MouseMove += CenterLabelMouseMove;
        _centerLabel.MouseLeave += CenterLabelMouseLeave;

        _miniToolStrip.Items.AddRange((ToolStripItem[])[_centerLabel]);
    }

    /// <summary>
    ///  This function sets up TemplateNode for ToolStrip, StatusStrip, ContextMenuStrip.
    /// </summary>
    private void SetUpToolTemplateNode(IComponent component)
    {
        _addItemButton = new ToolStripSplitButton
        {
            AutoSize = false,
            Margin = new Padding(1)
        };
        _addItemButton.Size = _miniToolStrip.DisplayRectangle.Size - _addItemButton.Margin.Size;
        _addItemButton.DropDownButtonWidth = s_miniToolStripDropDownButtonWidth;
        _addItemButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
        if (component is StatusStrip)
        {
            _addItemButton.ToolTipText = SR.ToolStripDesignerTemplateNodeSplitButtonStatusStripToolTip;
        }
        else
        {
            _addItemButton.ToolTipText = SR.ToolStripDesignerTemplateNodeSplitButtonToolTip;
        }

        _addItemButton.MouseDown += OnMouseDown;
        _addItemButton.MouseMove += OnMouseMove;
        _addItemButton.MouseUp += OnMouseUp;
        _addItemButton.DropDownOpened += OnAddItemButtonDropDownOpened;
        _contextMenu = ToolStripDesignerUtils.GetNewItemDropDown(
            component,
            currentItem: null,
            AddNewItemClick,
            convertTo: false,
            component.Site,
            populateCustom: false);

        _contextMenu.Text = "ItemSelectionMenu";
        _contextMenu.Closed += OnContextMenuClosed;
        _contextMenu.Closing += OnContextMenuClosing;
        _contextMenu.Opened += OnContextMenuOpened;
        _addItemButton.DropDown = _contextMenu;
        _addItemButton.AccessibleName = SR.ToolStripDesignerTemplateNodeSplitButtonStatusStripAccessibleName;
        _addItemButton.AccessibleRole = AccessibleRole.ButtonDropDown;

        // Set up default item and image.
        try
        {
            if (_addItemButton.DropDownItems.Count > 0)
            {
                ItemTypeToolStripMenuItem firstItem = (ItemTypeToolStripMenuItem)_addItemButton.DropDownItems[0];
                _addItemButton.ImageTransparentColor = Color.Lime;
                _addItemButton.Image = ScaleHelper.GetSmallIconResourceAsBitmap(
                    typeof(ToolStripTemplateNode),
                    "ToolStripTemplateNode",
                    ScaleHelper.InitialSystemDpi);

                _addItemButton.DefaultItem = firstItem;
            }

            Debug.Assert(_addItemButton.DropDownItems.Count > 0);
        }
        catch (Exception ex) when (!ex.IsCriticalException())
        {
        }

        _miniToolStrip.Items.AddRange((ToolStripItem[])
        [
            _addItemButton
        ]);
    }

    /// <summary>
    ///  This method does actual edit node creation.
    /// </summary>
    private void SetupNewEditNode(ToolStripTemplateNode owner, string text, IComponent currentItem)
    {
        // setup the MINIToolStrip host...
        _renderer = new MiniToolStripRenderer(owner);
        _miniToolStrip = new TransparentToolStrip(owner);
        if (currentItem is ToolStrip strip)
        {
            _miniToolStrip.RightToLeft = strip.RightToLeft;
            strip.RightToLeftChanged += OnRightToLeftChanged;

            // Make TransparentToolStrip has the same "Site" as ToolStrip. This could make sure TransparentToolStrip
            // has the same design time behavior as ToolStrip.
            _miniToolStrip.Site = strip.Site;
        }

        if (currentItem is ToolStripDropDownItem stripItem)
        {
            _miniToolStrip.RightToLeft = stripItem.RightToLeft;
            stripItem.RightToLeftChanged += OnRightToLeftChanged;
        }

        _miniToolStrip.SuspendLayout();
        _miniToolStrip.CanOverflow = false;
        _miniToolStrip.Cursor = Cursors.Default;
        _miniToolStrip.Dock = DockStyle.None;
        _miniToolStrip.GripStyle = ToolStripGripStyle.Hidden;
        _miniToolStrip.Name = "miniToolStrip";
        _miniToolStrip.TabIndex = 0;
        _miniToolStrip.Visible = true;
        _miniToolStrip.Renderer = _renderer;

        // Add items to the Template ToolStrip depending upon the Parent Type...
        if (currentItem is MenuStrip or ToolStripDropDownItem)
        {
            SetUpMenuTemplateNode(text, currentItem);
            _miniToolStrip.AccessibleRole = AccessibleRole.ComboBox;
            _miniToolStrip.Text = text;
        }
        else
        {
            SetUpToolTemplateNode(currentItem);
            _miniToolStrip.AccessibleRole = AccessibleRole.ButtonDropDown;
        }

        _miniToolStrip.MouseLeave += OnMouseLeave;
        _miniToolStrip.ResumeLayout();
    }

    /// <summary>
    ///  This method does sets the width of the Editor (_miniToolStrip) based on the text passed in.
    /// </summary>
    internal void SetWidth(string text)
    {
        // REVIEW: is this function necessary anymore?
        if (string.IsNullOrEmpty(text))
        {
            _miniToolStrip.Width = _centerLabel.Width + 2;
        }
        else
        {
            _centerLabel.Text = text;
        }
    }

    /// <summary>
    ///  Private class that implements the textBox for the InSitu Editor.
    /// </summary>
    private class TemplateTextBox : TextBox
    {
        private readonly TransparentToolStrip _parent;
        private readonly ToolStripTemplateNode _owner;
        private const int IMEMODE = 229;

        public TemplateTextBox(TransparentToolStrip parent, ToolStripTemplateNode owner) : base()
        {
            _parent = parent;
            _owner = owner;
            AutoSize = false;
            Multiline = false;
        }

        /// <summary>
        ///  Get Parent Handle.
        /// </summary>
        private bool IsParentWindow(IntPtr hWnd)
        {
            if (hWnd == _parent.Handle)
            {
                return true;
            }

            return false;
        }

        protected override bool IsInputKey(Keys keyData)
        {
            switch (keyData & Keys.KeyCode)
            {
                case Keys.Return:
                    _owner.Commit(true, false);
                    return true;
            }

            return base.IsInputKey(keyData);
        }

        /// <summary>
        ///  Process the IMEMode message..
        /// </summary>
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if ((int)keyData == IMEMODE)
            {
                _owner.IMEModeSet = true;
            }
            else
            {
                _owner.IMEModeSet = false;
                _owner._ignoreFirstKeyUp = false;
            }

            return base.ProcessDialogKey(keyData);
        }

        /// <summary>
        ///  Process the WNDPROC for WM_KILLFOCUS to commit the InSitu Editor..
        /// </summary>
        protected override void WndProc(ref Message m)
        {
            switch (m.MsgInternal)
            {
                case PInvokeCore.WM_KILLFOCUS:
                    base.WndProc(ref m);
                    HWND focusedWindow = (HWND)m.WParamInternal;
                    if (!IsParentWindow(focusedWindow))
                    {
                        _owner.Commit(enterKeyPressed: false, tabKeyPressed: false);
                    }

                    break;

                // 1.Slowly click on a menu strip item twice to make it editable, while the item's dropdown menu is visible
                // 2.Select the text of the item and right click on it
                // 3.Left click 'Copy' or 'Cut' in the context menu IDE crashed because left click in step3 invoked glyph
                //   behavior, which committed and destroyed the InSitu edit box and thus the 'copy' or 'cut' action has no
                //   text to work with. Thus need to block glyph behaviors while the context menu is displayed.
                case PInvokeCore.WM_CONTEXTMENU:
                    _owner.IsSystemContextMenuDisplayed = true;
                    base.WndProc(ref m);
                    _owner.IsSystemContextMenuDisplayed = false;
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }
    }

    /// <summary>
    ///  Private class to Change the ToolStrip to a TransparentToolStrip. Our EditorToolStrip is a TransparentToolStrip
    ///  so that it picks up the itemColor.
    /// </summary>
    public class TransparentToolStrip : ToolStrip
    {
        private readonly ToolStripTemplateNode _owner;
        private readonly IComponent _currentItem;

        public TransparentToolStrip(ToolStripTemplateNode owner)
        {
            _owner = owner;
            _currentItem = owner._component;
            TabStop = true;
            SetStyle(ControlStyles.Selectable, true);
            AutoSize = false;
            AccessibleName = SR.ToolStripDesignerToolStripAccessibleName;
            AccessibleRole = AccessibleRole.ComboBox;
        }

        /// <summary>
        ///  Owner TemplateNode..
        /// </summary>
        public ToolStripTemplateNode TemplateNode
        {
            get => _owner;
        }

        /// <summary>
        ///  Commit the node and move to next selection.
        /// </summary>
        private void CommitAndSelectNext(bool forward)
        {
            _owner.Commit(enterKeyPressed: false, tabKeyPressed: true);
            _owner.KeyboardService?.ProcessKeySelect(reverse: !forward);
        }

        /// <summary>
        ///  get current selection.
        /// </summary>
        private ToolStripItem GetSelectedItem()
        {
            ToolStripItem selectedItem = null;
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].Selected)
                {
                    selectedItem = Items[i];
                }
            }

            return selectedItem;
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public override Size GetPreferredSize(Size proposedSize)
        {
            if (_currentItem is ToolStripDropDownItem)
            {
                return new Size(Width, s_toolStripTempateHeight);
            }
            else
            {
                return new Size(Width, s_templateHeight);
            }
        }

        /// <summary>
        ///  Process the Tab Key..
        /// </summary>
        private bool ProcessTabKey(bool forward)
        {
            // Give the ToolStripItem first dibs
            ToolStripItem item = GetSelectedItem();
            if (item is ToolStripControlHost)
            {
                CommitAndSelectNext(forward);
                return true;
            }

            return false;
        }

        /// <summary>
        ///  Process the Dialog Keys for the Templatenode ToolStrip..
        /// </summary>
        protected override bool ProcessDialogKey(Keys keyData)
        {
            bool retVal = false;
            if (_owner.Active)
            {
                if ((keyData & (Keys.Alt | Keys.Control)) == Keys.None)
                {
                    Keys keyCode = keyData & Keys.KeyCode;
                    switch (keyCode)
                    {
                        case Keys.Tab:
                            retVal = ProcessTabKey((keyData & Keys.Shift) == Keys.None);
                            break;
                    }
                }

                if (retVal)
                {
                    return retVal;
                }
            }

            return base.ProcessDialogKey(keyData);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            if (_currentItem is ToolStripDropDownItem)
            {
                base.SetBoundsCore(x, y, s_toolStripTemplateWidth, s_toolStripTempateHeight, specified);
            }
            else if (_currentItem is MenuStrip)
            {
                base.SetBoundsCore(x, y, s_toolStripTemplateWidth, s_templateHeight, specified);
            }
            else
            {
                base.SetBoundsCore(x, y, s_templateWidth, s_templateHeight, specified);
            }
        }

        internal void RaiseStateChangeEvent()
        {
            AccessibilityNotifyClients(AccessibleEvents.StateChange, -1);
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.MsgInternal)
            {
                case PInvokeCore.WM_GETOBJECT:
                    if (_owner._addItemButton is null)
                    {
                        // only adding patterns to _miniToolStrip associated with MenuStrip or ContextMenu
                        return;
                    }

                    break;
            }

            base.WndProc(ref m);
        }
    }

    /// <summary>
    ///  Private class that implements the custom Renderer for the TemplateNode ToolStrip.
    /// </summary>
    public class MiniToolStripRenderer : ToolStripSystemRenderer
    {
        private int _state = (int)TemplateNodeSelectionState.None;
        private readonly Color _selectedBorderColor;
        private readonly Color _defaultBorderColor;
        private readonly Color _dropDownMouseOverColor;
        private readonly Color _dropDownMouseDownColor;
        private readonly Color _toolStripBorderColor;
        private readonly ToolStripTemplateNode _owner;
        private Rectangle _hotRegion = Rectangle.Empty;

        public MiniToolStripRenderer(ToolStripTemplateNode owner) : base()
        {
            // Add Colors
            _owner = owner;
            _selectedBorderColor = Color.FromArgb(46, 106, 197);
            _defaultBorderColor = Color.FromArgb(171, 171, 171);
            _dropDownMouseOverColor = Color.FromArgb(193, 210, 238);
            _dropDownMouseDownColor = Color.FromArgb(152, 181, 226);
            _toolStripBorderColor = Color.White;
        }

        /// <summary>
        ///  Current state of the TemplateNode UI..
        /// </summary>
        public int State
        {
            get => _state;
            set => _state = value;
        }

        /// <summary>
        ///  Custom method to draw DOWN arrow on the DropDown.
        /// </summary>
        private void DrawArrow(Graphics g, Rectangle bounds)
        {
            bounds.Width--;
            DrawArrow(new ToolStripArrowRenderEventArgs(g, null, bounds, SystemInformation.HighContrast ? Color.Black : SystemColors.ControlText, ArrowDirection.Down));
        }

        /// <summary>
        ///  Drawing different DropDown states.
        /// </summary>
        private void DrawDropDown(Graphics g, Rectangle bounds, int state)
        {
            switch (state)
            {
                case 1: // TemplateNodeSelected
                case 4: // MouseOver
                    using (LinearGradientBrush brush = new(bounds, Color.White, _defaultBorderColor, LinearGradientMode.Vertical))
                    {
                        g.FillRectangle(brush, bounds);
                    }

                    break;
                case 5: // MouseOverHotRegion
                    using (SolidBrush b = new(_dropDownMouseOverColor))
                    {
                        g.FillRectangle(b, _hotRegion);
                    }

                    break;
                case 6: // HotRegionSelected
                    using (SolidBrush b = new(_dropDownMouseDownColor))
                    {
                        g.FillRectangle(b, _hotRegion);
                    }

                    break;
            }

            DrawArrow(g, bounds);
        }

        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            if (_owner._component is MenuStrip or ToolStripDropDownItem)
            {
                Graphics g = e.Graphics;
                g.Clear(_toolStripBorderColor);
            }
            else
            {
                base.OnRenderToolStripBackground(e);
            }
        }

        /// <summary>
        ///  Render ToolStrip Border
        /// </summary>
        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle bounds = new(Point.Empty, e.ToolStrip.Size);
            Pen selectborderPen = new(_toolStripBorderColor);
            Rectangle drawRect = new(bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
            g.DrawRectangle(selectborderPen, drawRect);
            selectborderPen.Dispose();
        }

        /// <summary>
        ///  Render the Center Label on the TemplateNode ToolStrip.
        /// </summary>
        protected override void OnRenderLabelBackground(ToolStripItemRenderEventArgs e)
        {
            base.OnRenderLabelBackground(e);
            ToolStripItem item = e.Item;
            Graphics g = e.Graphics;
            Rectangle bounds = new(Point.Empty, item.Size);
            Rectangle drawRect = new(bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
            Pen borderPen = new(_defaultBorderColor);
            if (_state == (int)TemplateNodeSelectionState.TemplateNodeSelected) // state Template node is selected.
            {
                using (SolidBrush brush = new(_toolStripBorderColor))
                {
                    g.FillRectangle(brush, drawRect);
                }

                if (_owner.EditorToolStrip.RightToLeft == RightToLeft.Yes)
                {
                    _hotRegion = new Rectangle(bounds.Left + 2, bounds.Top + 2, s_templateHotRegionWidth, bounds.Bottom - 4);
                }
                else
                {
                    _hotRegion = new Rectangle(bounds.Right - s_templateHotRegionWidth - 2, bounds.Top + 2, s_templateHotRegionWidth, bounds.Bottom - 4);
                }

                _owner.HotRegion = _hotRegion;

                // do the Actual Drawing
                DrawDropDown(g, _hotRegion, _state);

                borderPen.Color = Color.Black;
                item.ForeColor = _defaultBorderColor;
                g.DrawRectangle(borderPen, drawRect);
            }

            if (_state == (int)TemplateNodeSelectionState.MouseOverLabel) // state Template node is selected.
            {
                if (_owner.EditorToolStrip.RightToLeft == RightToLeft.Yes)
                {
                    _hotRegion = new Rectangle(bounds.Left + 2, bounds.Top + 2, s_templateHotRegionWidth, bounds.Bottom - 4);
                }
                else
                {
                    _hotRegion = new Rectangle(bounds.Right - s_templateHotRegionWidth - 2, bounds.Top + 2, s_templateHotRegionWidth, bounds.Bottom - 4);
                }

                _owner.HotRegion = _hotRegion;

                g.Clear(_toolStripBorderColor);
                DrawDropDown(g, _hotRegion, _state);
                borderPen.Color = Color.Black;
                borderPen.DashStyle = DashStyle.Dot;
                g.DrawRectangle(borderPen, drawRect);
            }

            if (_state == (int)TemplateNodeSelectionState.MouseOverHotRegion)
            {
                g.Clear(_toolStripBorderColor);
                DrawDropDown(g, _hotRegion, _state);
                borderPen.Color = Color.Black;
                borderPen.DashStyle = DashStyle.Dot;
                item.ForeColor = _defaultBorderColor;
                g.DrawRectangle(borderPen, drawRect);
            }

            if (_state == (int)TemplateNodeSelectionState.HotRegionSelected)
            {
                g.Clear(_toolStripBorderColor);
                DrawDropDown(g, _hotRegion, _state);
                borderPen.Color = Color.Black;
                item.ForeColor = _defaultBorderColor;
                g.DrawRectangle(borderPen, drawRect);
            }

            if (_state == (int)TemplateNodeSelectionState.None) // state Template node is not selected.
            {
                g.Clear(_toolStripBorderColor);
                g.DrawRectangle(borderPen, drawRect);
                item.ForeColor = _defaultBorderColor;
            }

            borderPen.Dispose();
        }

        /// <summary>
        ///  Render the splitButton on the TemplateNode ToolStrip..
        /// </summary>
        protected override void OnRenderSplitButtonBackground(ToolStripItemRenderEventArgs e)
        {
            // DON'T CALL THE BASE AS IT DOESNT ALLOW US TO RENDER THE DROPDOWN BUTTON ....
            // base.OnRenderSplitButtonBackground(e);
            Graphics g = e.Graphics;
            if (e.Item is ToolStripSplitButton splitButton)
            {
                // Get the DropDownButton Bounds
                Rectangle buttonBounds = splitButton.DropDownButtonBounds;
                // Draw the White Divider Line...
                using (Pen p = new(_toolStripBorderColor))
                {
                    g.DrawLine(p, buttonBounds.Left, buttonBounds.Top + 1, buttonBounds.Left, buttonBounds.Bottom - 1);
                }

                Rectangle bounds = new(Point.Empty, splitButton.Size);
                bool splitButtonSelected = false;
                if (splitButton.DropDownButtonPressed)
                {
                    // Button is pressed
                    _state = 0;
                    Rectangle fillRect = new(buttonBounds.Left + 1, buttonBounds.Top, buttonBounds.Right, buttonBounds.Bottom);
                    using (SolidBrush brush = new(_dropDownMouseDownColor))
                    {
                        g.FillRectangle(brush, fillRect);
                    }

                    splitButtonSelected = true;
                }
                else if (_state == (int)TemplateNodeSelectionState.SplitButtonSelected)
                {
                    using (SolidBrush brush = new(_dropDownMouseOverColor))
                    {
                        g.FillRectangle(brush, splitButton.ButtonBounds);
                    }

                    splitButtonSelected = true;
                }
                else if (_state == (int)TemplateNodeSelectionState.DropDownSelected)
                {
                    Rectangle fillRect = new(buttonBounds.Left + 1, buttonBounds.Top, buttonBounds.Right, buttonBounds.Bottom);
                    using (SolidBrush brush = new(_dropDownMouseOverColor))
                    {
                        g.FillRectangle(brush, fillRect);
                    }

                    splitButtonSelected = true;
                }
                else if (_state == (int)TemplateNodeSelectionState.TemplateNodeSelected)
                {
                    splitButtonSelected = true;
                }

                Pen selectborderPen;
                if (splitButtonSelected)
                {
                    // DrawSelected Boder
                    selectborderPen = new Pen(_selectedBorderColor);
                }
                else
                {
                    // Draw Gray Border
                    selectborderPen = new Pen(_defaultBorderColor);
                }

                Rectangle drawRect = new(bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
                g.DrawRectangle(selectborderPen, drawRect);
                selectborderPen.Dispose();

                // Draw the Arrow
                DrawArrow(new ToolStripArrowRenderEventArgs(g, splitButton, splitButton.DropDownButtonBounds, SystemColors.ControlText, ArrowDirection.Down));
            }
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            ToolStripItem item = e.Item as ToolStripLabel;
            if (item is not null && string.Equals(item.Name, CenterLabelName, StringComparison.InvariantCulture) && SystemInformation.HighContrast)
            {
                // "Type Here" node always has white background, text should be painted in black
                e.TextColor = Color.Black;
            }

            base.OnRenderItemText(e);
        }
    }
}
