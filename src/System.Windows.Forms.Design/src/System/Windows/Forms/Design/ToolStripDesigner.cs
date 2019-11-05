// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Windows.Forms.Design.Behavior;
using static Interop;

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  Designer for the ToolStrip class.
    /// </summary>
    internal class ToolStripDesigner : ControlDesigner
    {
        private const int GLYPHBORDER = 2;
        internal static Point s_lastCursorPosition = Point.Empty; //remembers last cursorPosition;
        internal static bool s_autoAddNewItems = true; // true to force newly created items to be added to the currently selected strip.
        internal static ToolStripItem s_dragItem = null; // this is used in overflow to know current item selected while drag, so that we can get the drop-index.
        internal static bool s_shiftState = false; // maintains the shift state used of invalidation. disable csharp compiler warning #0414: field assigned unused value
#pragma warning disable 0414
        internal static bool s_editTemplateNode = false; // this is used in selection changed so that unnecessary redraw is not required.
#pragma warning restore 0414
        private DesignerToolStripControlHost _editorNode = null; //new editorNode
        private ToolStripEditorManager _editManager = null; // newly added editor manager ...
        private ToolStrip _miniToolStrip = null;// the toolStrip that hosts the "New Template Node" button
        private DesignerTransaction _insertMenuItemTransaction = null; //There Should be one and only one Pending insertTransaction.
        private Rectangle _dragBoxFromMouseDown = Rectangle.Empty; //Needed to Store the DRAGDROP Rect from the ToolStripItemBehavior.
        private int _indexOfItemUnderMouseToDrag = -1; //defaulted to invalid index andwill be set by the behaviour.
        private ToolStripTemplateNode _tn = null; //templateNode
        private ISelectionService _selectionSvc = null; // cached selection service.
        private uint _editingCollection = 0; // non-zero if the collection editor is up for this ToolStrip or a child of it.
        private DesignerTransaction _pendingTransaction = null; // our transaction for adding/removing items.
        private bool _addingItem = false; // true if we are expecting to be notified of adding a ToolStripItem to the designer.
        private Rectangle _boundsToInvalidate = Rectangle.Empty; //Bounds to Invalidate if a DropDownItem is Deleted
        private bool _currentVisible = true; // Change Visibility
        private ToolStripActionList _actionLists; // Action List on Chrome...
        private ToolStripAdornerWindowService _toolStripAdornerWindowService = null; // Add the Adorner Service for OverFlow DropDown...
        private IDesignerHost _host = null;//get private copy of the DesignerHost
        private IComponentChangeService _componentChangeSvc;
        private bool _undoingCalled = false;
        private IToolboxService _toolboxService;
        private ContextMenuStrip _toolStripContextMenu;
        private bool _toolStripSelected = false;
        private bool _cacheItems = false; //ToolStripDesigner would cache items for the MenuItem when dropdown is changed.
        private ArrayList _items; //cached Items.
        private bool _disposed = false;
        private DesignerTransaction _newItemTransaction;
        private bool _fireSyncSelection = false; //fires SyncSelection when we toggle the items visibility to add the glyphs after the item gets visible.
        private ToolStripKeyboardHandlingService _keyboardHandlingService = null;
        private bool _parentNotVisible = false; //sync the parent visibility (used for ToolStripPanels)
        private bool _dontCloseOverflow = false; //When an item is added to the ToolStrip through the templateNode which is on the Overflow; we should not close the overflow (to avoid flicker)
        private bool _addingDummyItem = false; //When the dummyItem is added the toolStrip might resize (as in the Vertival Layouts). In this case we dont want the Resize to cause SyncSelection and Layouts.

        /// <summary>
        ///  Adds designer actions to the ActionLists collection.
        /// </summary>
        public override DesignerActionListCollection ActionLists
        {
            get
            {
                DesignerActionListCollection actionLists = new DesignerActionListCollection();
                actionLists.AddRange(base.ActionLists);
                if (_actionLists == null)
                {
                    _actionLists = new ToolStripActionList(this);
                }
                actionLists.Add(_actionLists);

                // First add the verbs for this component there...
                DesignerVerbCollection verbs = Verbs;
                if (verbs != null && verbs.Count != 0)
                {
                    DesignerVerb[] verbsArray = new DesignerVerb[verbs.Count];
                    verbs.CopyTo(verbsArray, 0);
                    actionLists.Add(new DesignerActionVerbList(verbsArray));
                }
                return actionLists;
            }
        }

        /// <summary>
        ///  Compute the rect for the "Add New Item" button.
        /// </summary>
        private Rectangle AddItemRect
        {
            get
            {
                Rectangle rect = new Rectangle();
                if (_miniToolStrip == null)
                {
                    return rect;
                }
                rect = _miniToolStrip.Bounds;
                return rect;
            }
        }

        /// <summary>
        ///  Accessor for Shadow Property for AllowDrop.
        /// </summary>
        private bool AllowDrop
        {
            get => (bool)ShadowProperties["AllowDrop"];
            set
            {
                if (value && AllowItemReorder)
                {
                    throw new ArgumentException(SR.ToolStripAllowItemReorderAndAllowDropCannotBeSetToTrue);
                }
                ShadowProperties["AllowDrop"] = value;
            }
        }

        /// <summary>
        ///  Accessor for Shadow Property for AllowItemReorder.
        /// </summary>
        private bool AllowItemReorder
        {
            get => (bool)ShadowProperties["AllowItemReorder"];
            set
            {
                if (value && AllowDrop)
                {
                    throw new ArgumentException(SR.ToolStripAllowItemReorderAndAllowDropCannotBeSetToTrue);
                }
                ShadowProperties["AllowItemReorder"] = value;
            }
        }

        /// <summary>
        ///  The ToolStripItems are the associated components. We want those to come with in any cut, copy opreations.
        /// </summary>
        public override System.Collections.ICollection AssociatedComponents
        {
            get
            {
                ArrayList items = new ArrayList();
                foreach (ToolStripItem item in ToolStrip.Items)
                {
                    if (!(item is DesignerToolStripControlHost addNewItem))
                    {
                        items.Add(item);
                    }
                }
                return (ICollection)items;
            }
        }

        /// <summary>
        ///  CacheItems is set to TRUE by the ToolStripMenuItemDesigner, when the Transaction of setting the DropDown property is undone. In this case the Undo adds the original items to the Main MenustripDesigners Items collection and later are moved to to the appropriate ToolStripMenuItem
        /// </summary>
        public bool CacheItems
        {
            get => _cacheItems;
            set => _cacheItems = value;
        }

        /// <summary>
        ///  False if were inherited and can't be modified.
        /// </summary>
        private bool CanAddItems
        {
            get
            {
                // Make sure the component is not being inherited -- we can't delete these!
                InheritanceAttribute ia = (InheritanceAttribute)TypeDescriptor.GetAttributes(ToolStrip)[typeof(InheritanceAttribute)];
                if (ia == null || ia.InheritanceLevel == InheritanceLevel.NotInherited)
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        // This boolean indicates whether the Control will allow SnapLines to be shown when any other targetControl is dragged on the design surface. This is true by default.
        /// </summary>
        internal override bool ControlSupportsSnaplines
        {
            get
            {
                if (!(ToolStrip.Parent is ToolStripPanel))
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        ///  DesignerContextMenu that is shown on the ToolStrip/MenuStrip/StatusStrip.
        /// </summary>
        private ContextMenuStrip DesignerContextMenu
        {
            get
            {
                if (_toolStripContextMenu == null)
                {
                    _toolStripContextMenu = new BaseContextMenuStrip(ToolStrip.Site, ToolStrip)
                    {
                        Text = "CustomContextMenu"
                    };
                }
                return _toolStripContextMenu;
            }
        }

        /// <summary>
        ///  Used by ToolStripTemplateNode. When the ToolStrip gains selection the Overflow is closed. But when an item is added through the TemplateNode which itself is on the Overflow, we should not close the Overflow as this caused weird artifacts and flicker. Hence this boolean property.
        /// </summary>
        public bool DontCloseOverflow
        {
            get => _dontCloseOverflow;
            set => _dontCloseOverflow = value;
        }

        /// <summary>
        ///  Since the Itemglyphs are recreated on the SelectionChanged, we need to cache in the "MouseDown" while the item Drag-Drop operation.
        /// </summary>
        public Rectangle DragBoxFromMouseDown
        {
            get => _dragBoxFromMouseDown;
            set => _dragBoxFromMouseDown = value;
        }

        /// <summary>
        ///  Set by the ToolStripItemCollectionEditor when it's launched for this ToolStrip so we won't pick up it's items when added.  We count this so that we can deal with nestings.
        /// </summary>
        internal bool EditingCollection
        {
            get => _editingCollection != 0;
            set
            {
                if (value)
                {
                    _editingCollection++;
                }
                else
                {
                    _editingCollection--;
                }

            }
        }

        /// <summary>
        ///  EditManager for the ToolStrip Designer. This EditorManager controls the Insitu Editing.
        /// </summary>
        public ToolStripEditorManager EditManager
        {
            get => _editManager;
        }

        /// <summary>
        ///  The TemplateNode. This is the object that actually creates miniToolStrip and manages InSitu editing.
        /// </summary>
        internal ToolStripTemplateNode Editor
        {
            get => _tn;
        }

        /// <summary>
        ///  This is the ToolStripControlHost that hosts the ToolStripTemplateNode's miniToolStrip.
        /// </summary>
        public DesignerToolStripControlHost EditorNode
        {
            get => _editorNode;
        }

        /// <summary>
        ///  This is the ToolStripTemplateNode's miniToolStrip.
        /// </summary>
        internal ToolStrip EditorToolStrip
        {
            get => _miniToolStrip;
            set
            {
                _miniToolStrip = value;
                _miniToolStrip.Parent = ToolStrip;
                LayoutToolStrip();
            }
        }

        /// <summary>
        ///  This will be set through ToolStripItemDesigner.SetItemVisible( ) if we find there is atleast one time that toggled from Visible==false to Visible==true In such a case we need to call BehaviorService.SyncSelection( ) toupdate the glyphs.
        /// </summary>
        public bool FireSyncSelection
        {
            get => _fireSyncSelection;
            set => _fireSyncSelection = value;
        }

        /// <summary>
        ///  Since the Itemglyphs are recreated on the SelectionChanged, we need to cache in the "index" of last MouseDown while the item Drag-Drop operation.
        /// </summary>
        public int IndexOfItemUnderMouseToDrag
        {
            get => _indexOfItemUnderMouseToDrag;
            set => _indexOfItemUnderMouseToDrag = value;
        }

        /// <summary>
        ///  ToolStrips if inherited act as ReadOnly.
        /// </summary>
        protected override InheritanceAttribute InheritanceAttribute
        {
            get
            {
                if ((base.InheritanceAttribute == InheritanceAttribute.Inherited))
                {
                    return InheritanceAttribute.InheritedReadOnly;
                }
                return base.InheritanceAttribute;
            }
        }

        /// <summary>
        ///  This is the insert Transaction. Now insert can happen at Main Menu level or the DropDown Level. This transaction is used to keep both in sync.
        /// </summary>
        public DesignerTransaction InsertTansaction
        {
            get => _insertMenuItemTransaction;
            set => _insertMenuItemTransaction = value;
        }

        /// <summary>
        ///  Checks if there is a seleciton of the ToolStrip or one of it's items.
        /// </summary>
        private bool IsToolStripOrItemSelected
        {
            get => _toolStripSelected;
        }

        /// <summary>
        ///  CacheItems is set to TRUE by the ToolStripMenuItemDesigner, when the Transaction of setting the DropDown property is undone. In this case the Undo adds the original items to the Main MenustripDesigners Items collection and later are moved to to the appropriate ToolStripMenuItem. This is the Items Collection.
        /// </summary>
        public ArrayList Items
        {
            get
            {
                if (_items == null)
                {
                    _items = new ArrayList();
                }
                return _items;
            }
        }

        /// <summary>
        ///  This is the new item Transaction. This is used when the Insitu editor adds new Item.
        /// </summary>
        public DesignerTransaction NewItemTransaction
        {
            get => _newItemTransaction;
            set => _newItemTransaction = value;
        }

        /// <summary>
        ///  Compute the rect for the "OverFlow" button.
        /// </summary>
        private Rectangle OverFlowButtonRect
        {
            get
            {
                Rectangle rect = new Rectangle();
                if (ToolStrip.OverflowButton.Visible)
                {
                    return ToolStrip.OverflowButton.Bounds;
                }
                else
                {
                    return rect;
                }
            }
        }

        /// <summary>
        ///  Get and cache the selection service
        /// </summary>
        internal ISelectionService SelectionService
        {
            get
            {
                if (_selectionSvc == null)
                {
                    _selectionSvc = (ISelectionService)GetService(typeof(ISelectionService));
                    Debug.Assert(_selectionSvc != null, "Failed to get Selection Service!");
                }
                return _selectionSvc;
            }
        }

        public bool SupportEditing
        {
            get
            {
                if (GetService(typeof(DesignerOptionService)) is WindowsFormsDesignerOptionService dos)
                {
                    return dos.CompatibilityOptions.EnableInSituEditing;
                }
                return true;
            }
        }

        /// <summary>
        ///  Handy way of gettting our ToolStrip
        /// </summary>
        protected ToolStrip ToolStrip
        {
            get => (ToolStrip)Component;
        }

        /// <summary>
        ///  Get and cache the toolStripKeyBoard service
        /// </summary>
        private ToolStripKeyboardHandlingService KeyboardHandlingService
        {
            get
            {
                if (_keyboardHandlingService == null)
                {
                    //Add the EditService so that the ToolStrip can do its own Tab and Keyboard Handling
                    _keyboardHandlingService = (ToolStripKeyboardHandlingService)GetService(typeof(ToolStripKeyboardHandlingService));
                    if (_keyboardHandlingService == null)
                    {
                        _keyboardHandlingService = new ToolStripKeyboardHandlingService(Component.Site);
                    }
                }
                return _keyboardHandlingService;
            }
        }

        /// <summary>
        ///  There are certain containers (like ToolStrip) that require PerformLayout to be serialized in the code gen.
        /// </summary>
        internal override bool SerializePerformLayout
        {
            get => true;
        }

        /// <summary>
        ///  Un - ShadowProperty.
        /// </summary>
        internal bool Visible
        {
            get => _currentVisible;
            set
            {
                _currentVisible = value;
                // If the user has set the Visible to false, sync the controls visible property.
                if (ToolStrip.Visible != value && !SelectionService.GetComponentSelected(ToolStrip))
                {
                    Control.Visible = value;
                }
            }
        }

        /// <summary>
        ///  This will add BodyGlyphs for the Items on the OverFlow. Since ToolStripItems are component we have to manage Adding and Deleting the glyphs ourSelves.
        /// </summary>
        private void AddBodyGlyphsForOverflow()
        {
            // now walk the ToolStrip and add glyphs for each of it's children
            foreach (ToolStripItem item in ToolStrip.Items)
            {
                if (item is DesignerToolStripControlHost)
                {
                    continue;
                }
                // make sure it's on the Overflow...
                if (item.Placement == ToolStripItemPlacement.Overflow)
                {
                    AddItemBodyGlyph(item);
                }
            }
        }

        /// <summary>
        ///  This will add BodyGlyphs for the Items on the OverFlow. Since ToolStripItems are component we have to manage Adding and Deleting the glyphs ourSelves. Called by AddBodyGlyphsForOverflow()
        /// </summary>
        private void AddItemBodyGlyph(ToolStripItem item)
        {
            if (item != null)
            {
                ToolStripItemDesigner dropDownItemDesigner = (ToolStripItemDesigner)_host.GetDesigner(item);
                if (dropDownItemDesigner != null)
                {
                    Rectangle bounds = dropDownItemDesigner.GetGlyphBounds();
                    Behavior.Behavior toolStripBehavior = new ToolStripItemBehavior();
                    // Initialize Glyph
                    ToolStripItemGlyph bodyGlyphForddItem = new ToolStripItemGlyph(item, dropDownItemDesigner, bounds, toolStripBehavior);
                    //Set the glyph for the item .. so that we can remove it later....
                    dropDownItemDesigner.bodyGlyph = bodyGlyphForddItem;
                    //Add ItemGlyph to the Collection
                    if (_toolStripAdornerWindowService != null)
                    {
                        _toolStripAdornerWindowService.DropDownAdorner.Glyphs.Add(bodyGlyphForddItem);
                    }
                }
            }
        }

        /// <summary>
        ///  Fired when a new item is chosen from the AddItems menu from the Template Node.
        /// </summary>
        private ToolStripItem AddNewItem(Type t)
        {
            Debug.Assert(_host != null, "Why didn't we get a designer host?");
            NewItemTransaction = _host.CreateTransaction(SR.ToolStripCreatingNewItemTransaction);
            IComponent component = null;
            try
            {
                _addingItem = true;
                // Suspend the Layout as we are about to add Item to the ToolStrip
                ToolStrip.SuspendLayout();
                ToolStripItemDesigner designer = null;
                try
                {
                    // The code in ComponentAdded will actually get the add done.  This should be inside the try finally because it could throw an exception and keep the toolstrip in SuspendLayout mode
                    component = _host.CreateComponent(t);
                    designer = _host.GetDesigner(component) as ToolStripItemDesigner;
                    designer.InternalCreate = true;
                    if (designer is ComponentDesigner)
                    {
                        ((ComponentDesigner)designer).InitializeNewComponent(null);
                    }
                }
                finally
                {
                    if (designer != null)
                    {
                        designer.InternalCreate = false;
                    }
                    // Resume the Layout as we are about to add Item to the ToolStrip
                    ToolStrip.ResumeLayout();
                }
            }
            catch (Exception e)
            {
                if (NewItemTransaction != null)
                {
                    NewItemTransaction.Cancel();
                    NewItemTransaction = null;
                }

                // Throw the exception unless it's a canceled checkout
                if ((!(e is CheckoutException checkoutException)) || (!checkoutException.Equals(CheckoutException.Canceled)))
                {
                    throw;
                }
            }
            finally
            {
                _addingItem = false;
            }
            return component as ToolStripItem;
        }

        // Standard 'catch all - rethrow critical' exception pattern
        internal ToolStripItem AddNewItem(Type t, string text, bool enterKeyPressed, bool tabKeyPressed)
        {
            Debug.Assert(_host != null, "Why didn't we get a designer host?");
            Debug.Assert(_pendingTransaction == null, "Adding item with pending transaction?");
            DesignerTransaction outerTransaction = _host.CreateTransaction(string.Format(SR.ToolStripAddingItem, t.Name));
            ToolStripItem item = null;
            try
            {
                _addingItem = true;
                // Suspend the Layout as we are about to add Item to the ToolStrip
                ToolStrip.SuspendLayout();
                // The code in ComponentAdded will actually get the add done.
                IComponent component = _host.CreateComponent(t, NameFromText(text, t, Component.Site));
                ToolStripItemDesigner designer = _host.GetDesigner(component) as ToolStripItemDesigner;
                try
                {
                    // ToolStripItem designer tries to set the TEXT for the item in the InitializeNewComponent(). But since we are create item thru InSitu .. we shouldnt do this. Also we shouldn't set the TEXT if we are creating a dummyItem.
                    if (!string.IsNullOrEmpty(text))
                    {
                        designer.InternalCreate = true;
                    }
                    if (designer is ComponentDesigner)
                    {
                        ((ComponentDesigner)designer).InitializeNewComponent(null);
                    }
                }
                finally
                {
                    designer.InternalCreate = false;
                }

                //Set the Text and Image..
                item = component as ToolStripItem;
                if (item != null)
                {
                    PropertyDescriptor textProperty = TypeDescriptor.GetProperties(item)["Text"];
                    Debug.Assert(textProperty != null, "Could not find 'Text' property in ToolStripItem.");
                    if (textProperty != null && !string.IsNullOrEmpty(text))
                    {
                        textProperty.SetValue(item, text);
                    }

                    //Set the Image property and DisplayStyle...
                    if (item is ToolStripButton || item is ToolStripSplitButton || item is ToolStripDropDownButton)
                    {
                        Image image = null;
                        try
                        {
                            image = new Icon(typeof(ToolStripButton), "blank").ToBitmap();
                        }
                        catch (Exception e)
                        {
                            if (ClientUtils.IsCriticalException(e))
                            {
                                throw;
                            }
                        }

                        PropertyDescriptor imageProperty = TypeDescriptor.GetProperties(item)["Image"];
                        Debug.Assert(imageProperty != null, "Could not find 'Image' property in ToolStripItem.");
                        if (imageProperty != null && image != null)
                        {
                            imageProperty.SetValue(item, image);
                        }

                        PropertyDescriptor dispProperty = TypeDescriptor.GetProperties(item)["DisplayStyle"];
                        Debug.Assert(dispProperty != null, "Could not find 'DisplayStyle' property in ToolStripItem.");
                        if (dispProperty != null)
                        {
                            dispProperty.SetValue(item, ToolStripItemDisplayStyle.Image);
                        }

                        PropertyDescriptor imageTransProperty = TypeDescriptor.GetProperties(item)["ImageTransparentColor"];
                        Debug.Assert(imageTransProperty != null, "Could not find 'DisplayStyle' property in ToolStripItem.");
                        if (imageTransProperty != null)
                        {
                            imageTransProperty.SetValue(item, Color.Magenta);
                        }
                    }
                }

                // ResumeLayout on ToolStrip.
                ToolStrip.ResumeLayout();
                if (!tabKeyPressed)
                {
                    if (enterKeyPressed)
                    {
                        if (!designer.SetSelection(enterKeyPressed))
                        {
                            if (KeyboardHandlingService != null)
                            {
                                KeyboardHandlingService.SelectedDesignerControl = _editorNode;
                                SelectionService.SetSelectedComponents(null, SelectionTypes.Replace);
                            }
                        }
                    }
                    else
                    {
                        // put the templateNode into nonselection mode && select the existing Item
                        KeyboardHandlingService.SelectedDesignerControl = null;
                        SelectionService.SetSelectedComponents(new IComponent[] { item }, SelectionTypes.Replace);
                        _editorNode.RefreshSelectionGlyph();
                    }
                }
                else
                {
                    if (_keyboardHandlingService != null)
                    {
                        KeyboardHandlingService.SelectedDesignerControl = _editorNode;
                        SelectionService.SetSelectedComponents(null, SelectionTypes.Replace);
                    }
                }

                if (designer != null && item.Placement != ToolStripItemPlacement.Overflow)
                {
                    Rectangle bounds = designer.GetGlyphBounds();
                    SelectionManager selMgr = (SelectionManager)GetService(typeof(SelectionManager));
                    Behavior.Behavior toolStripBehavior = new ToolStripItemBehavior();
                    ToolStripItemGlyph bodyGlyphForItem = new ToolStripItemGlyph(item, designer, bounds, toolStripBehavior);
                    //Add ItemGlyph to the Collection
                    selMgr.BodyGlyphAdorner.Glyphs.Insert(0, bodyGlyphForItem);
                }
                else if (designer != null && item.Placement == ToolStripItemPlacement.Overflow)
                {
                    // Add Glyphs for overflow...
                    RemoveBodyGlyphsForOverflow();
                    AddBodyGlyphsForOverflow();
                }
            }
            catch (Exception exception)
            {
                // ResumeLayout on ToolStrip.
                ToolStrip.ResumeLayout();
                if (_pendingTransaction != null)
                {
                    _pendingTransaction.Cancel();
                    _pendingTransaction = null;
                }
                if (outerTransaction != null)
                {
                    outerTransaction.Cancel();
                    outerTransaction = null;
                }

                if (exception is CheckoutException checkoutEx && checkoutEx != CheckoutException.Canceled)
                {
                    throw;
                }
            }
            finally
            {
                if (_pendingTransaction != null)
                {
                    _pendingTransaction.Cancel();
                    _pendingTransaction = null;

                    if (outerTransaction != null)
                    {
                        outerTransaction.Cancel();
                    }
                }
                else if (outerTransaction != null)
                {
                    outerTransaction.Commit();
                    outerTransaction = null;
                }
                _addingItem = false;
            }
            return item;
        }

        // <summary>
        //  Adds the new TemplateNode to the ToolStrip or MenuStrip.
        // <summary/>
        internal void AddNewTemplateNode(ToolStrip wb)
        {
            // setup the MINIToolStrip host...
            _tn = new ToolStripTemplateNode(Component, SR.ToolStripDesignerTemplateNodeEnterText, null);
            _miniToolStrip = _tn.EditorToolStrip;
            int width = _tn.EditorToolStrip.Width;
            _editorNode = new DesignerToolStripControlHost(_tn.EditorToolStrip);
            _tn.ControlHost = _editorNode;
            _editorNode.Width = width;
            ToolStrip.Items.Add(_editorNode);
            _editorNode.Visible = false;
        }

        internal void CancelPendingMenuItemTransaction()
        {
            if (_insertMenuItemTransaction != null)
            {
                _insertMenuItemTransaction.Cancel();
            }
        }

        // <summary>
        //  Check if the ToolStripItems are selected.
        // <summary/>
        private bool CheckIfItemSelected()
        {
            bool showToolStrip = false;
            object comp = SelectionService.PrimarySelection;
            if (comp == null)
            {
                comp = (IComponent)KeyboardHandlingService.SelectedDesignerControl;
            }
            if (comp is ToolStripItem item)
            {
                if (item.Placement == ToolStripItemPlacement.Overflow && item.Owner == ToolStrip)
                {
                    if (ToolStrip.CanOverflow && !ToolStrip.OverflowButton.DropDown.Visible)
                    {
                        ToolStrip.OverflowButton.ShowDropDown();
                    }
                    showToolStrip = true;
                }
                else
                {

                    if (!ItemParentIsOverflow(item))
                    {
                        if (ToolStrip.OverflowButton.DropDown.Visible)
                        {
                            ToolStrip.OverflowButton.HideDropDown();
                        }
                    }
                    if (item.Owner == ToolStrip)
                    {
                        showToolStrip = true;
                    }
                    else if (item is DesignerToolStripControlHost)
                    {
                        if (item.IsOnDropDown && item.Placement != ToolStripItemPlacement.Overflow)
                        {
                            ToolStripDropDown dropDown = (ToolStripDropDown)((DesignerToolStripControlHost)comp).GetCurrentParent();
                            if (dropDown != null)
                            {
                                ToolStripItem ownerItem = dropDown.OwnerItem;
                                ToolStripMenuItemDesigner itemDesigner = (ToolStripMenuItemDesigner)_host.GetDesigner(ownerItem);
                                ToolStripDropDown topmost = itemDesigner.GetFirstDropDown((ToolStripDropDownItem)(ownerItem));
                                ToolStripItem topMostItem = (topmost == null) ? ownerItem : topmost.OwnerItem;

                                if (topMostItem != null && topMostItem.Owner == ToolStrip)
                                {
                                    showToolStrip = true;
                                }
                            }
                        }
                    }
                    else if (item.IsOnDropDown && item.Placement != ToolStripItemPlacement.Overflow)
                    {
                        ToolStripItem parentItem = ((ToolStripDropDown)(item.Owner)).OwnerItem;
                        if (parentItem != null)
                        {
                            ToolStripMenuItemDesigner itemDesigner = (ToolStripMenuItemDesigner)_host.GetDesigner(parentItem);
                            ToolStripDropDown topmost = itemDesigner?.GetFirstDropDown((ToolStripDropDownItem)parentItem);
                            ToolStripItem topMostItem = (topmost == null) ? parentItem : topmost.OwnerItem;
                            if (topMostItem != null && topMostItem.Owner == ToolStrip)
                            {
                                showToolStrip = true;
                            }
                        }
                    }
                }
            }
            return showToolStrip;
        }

        // <summary>
        // This is called ToolStripItemGlyph to commit the TemplateNode Edition on the Parent ToolStrip.
        // <summary/>
        internal bool Commit()
        {
            if (_tn != null && _tn.Active)
            {
                _tn.Commit(false, false);
                _editorNode.Width = _tn.EditorToolStrip.Width;
            }
            else
            {
                if (SelectionService.PrimarySelection is ToolStripDropDownItem selectedItem)
                {
                    if (_host.GetDesigner(selectedItem) is ToolStripMenuItemDesigner itemDesigner && itemDesigner.IsEditorActive)
                    {
                        itemDesigner.Commit();
                        return true;
                    }
                }
                else
                {
                    if (KeyboardHandlingService != null)
                    {
                        if (KeyboardHandlingService.SelectedDesignerControl is ToolStripItem designerItem && designerItem.IsOnDropDown)
                        {
                            if (designerItem.GetCurrentParent() is ToolStripDropDown parent)
                            {
                                if (parent.OwnerItem is ToolStripDropDownItem ownerItem)
                                {
                                    if (_host.GetDesigner(ownerItem) is ToolStripMenuItemDesigner itemDesigner && itemDesigner.IsEditorActive)
                                    {
                                        itemDesigner.Commit();
                                        return true;
                                    }
                                }
                            }
                        }
                        else
                        { //check for normal ToolStripItem selection ....
                            if (SelectionService.PrimarySelection is ToolStripItem toolItem)
                            {
                                ToolStripItemDesigner itemDesigner = (ToolStripItemDesigner)_host.GetDesigner(toolItem);
                                if (itemDesigner != null && itemDesigner.IsEditorActive)
                                {
                                    itemDesigner.Editor.Commit(false, false);
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        ///  Make sure the AddNewItem button is setup properly.
        /// </summary>
        private void Control_HandleCreated(object sender, EventArgs e)
        {
            InitializeNewItemDropDown();
        }

        /// <summary>
        ///  Fired after a component has been added.  Here, we add it to the ToolStrip and select it.
        /// </summary>
        private void ComponentChangeSvc_ComponentAdded(object sender, ComponentEventArgs e)
        {
            // If another ToolStrip is getting added and we are currently selected then unselect us .. the newly added toolStrip should get selected.
            if (_toolStripSelected && e.Component is ToolStrip)
            {
                _toolStripSelected = false;
            }
            try
            {
                // make sure it's one of ours and not on DropDown.
                if (e.Component is ToolStripItem newItem && _addingItem && !newItem.IsOnDropDown)
                {
                    _addingItem = false;
                    if (CacheItems)
                    {
                        _items.Add(newItem);
                    }
                    else
                    {
                        // Get the current count of ToolStripItems.
                        int count = ToolStrip.Items.Count;
                        // notify the designer what's changed.
                        try
                        {
                            base.RaiseComponentChanging(TypeDescriptor.GetProperties(Component)["Items"]);
                            if (SelectionService.PrimarySelection is ToolStripItem selectedItem)
                            {
                                //ADD at the current Selection ...
                                if (selectedItem.Owner == ToolStrip)
                                {
                                    int indexToInsert = ToolStrip.Items.IndexOf(selectedItem);
                                    ToolStrip.Items.Insert(indexToInsert, newItem);
                                }

                            }
                            else if (count > 0)
                            {
                                // ADD at Last but one, the last one being the TemplateNode always...
                                ToolStrip.Items.Insert(count - 1, newItem);
                            }
                            else
                            {
                                ToolStrip.Items.Add(newItem);
                            }
                        }
                        finally
                        {
                            base.RaiseComponentChanged(TypeDescriptor.GetProperties(Component)["Items"], null, null);
                        }
                    }
                }
            }
            catch
            {
                if (_pendingTransaction != null)
                {
                    _pendingTransaction.Cancel();
                    _pendingTransaction = null;
                    _insertMenuItemTransaction = null;
                }
            }
            finally
            {
                if (_pendingTransaction != null)
                {
                    _pendingTransaction.Commit();
                    _pendingTransaction = null;
                    _insertMenuItemTransaction = null;
                }
            }
        }

        /// <summary>
        ///  Checks if the component being added is a child ToolStripItem.
        /// </summary>
        private void ComponentChangeSvc_ComponentAdding(object sender, ComponentEventArgs e)
        {
            if (KeyboardHandlingService != null && KeyboardHandlingService.CopyInProgress)
            {
                return;
            }

            // Return if we are not the owner !!
            object selectedItem = SelectionService.PrimarySelection;
            if (selectedItem == null)
            {
                if (_keyboardHandlingService != null)
                {
                    selectedItem = KeyboardHandlingService.SelectedDesignerControl;
                }
            }
            if (selectedItem is ToolStripItem currentSel && currentSel.Owner != ToolStrip)
            {
                return;
            }

            // we'll be adding a child item if the component is a ToolStrip item and we've currently got this ToolStrip or one of it's items selected. we do this so things like paste and undo automagically work.
            ToolStripItem addingItem = e.Component as ToolStripItem;
            if (addingItem != null && addingItem.Owner != null)
            {
                if (addingItem.Owner.Site == null)
                {
                    //we are DummyItem to the ToolStrip...
                    return;
                }
            }
            if (_insertMenuItemTransaction == null && s_autoAddNewItems && addingItem != null && !_addingItem && IsToolStripOrItemSelected && !EditingCollection)
            {
                _addingItem = true;

                if (_pendingTransaction == null)
                {
                    Debug.Assert(_host != null, "Why didn't we get a designer host?");
                    _insertMenuItemTransaction = _pendingTransaction = _host.CreateTransaction(SR.ToolStripDesignerTransactionAddingItem);
                }
            }
        }

        /// <summary>
        ///  Required to check if we need to show the Overflow, if any change has caused the item to go into the overflow.
        /// </summary>
        private void ComponentChangeSvc_ComponentChanged(object sender, ComponentChangedEventArgs e)
        {
            if (e.Component is ToolStripItem changingItem)
            {
                ToolStrip parent = changingItem.Owner;
                if (parent == ToolStrip && e.Member != null && e.Member.Name == "Overflow")
                {
                    ToolStripItemOverflow oldValue = (ToolStripItemOverflow)e.OldValue;
                    ToolStripItemOverflow newValue = (ToolStripItemOverflow)e.NewValue;
                    if (oldValue != ToolStripItemOverflow.Always && newValue == ToolStripItemOverflow.Always)
                    {
                        // If now the Item falls in the Overflow .. Open the Overflow..
                        if (ToolStrip.CanOverflow && !ToolStrip.OverflowButton.DropDown.Visible)
                        {
                            ToolStrip.OverflowButton.ShowDropDown();
                        }
                    }
                }
            }
        }

        /// <summary>
        ///  After a ToolStripItem is removed, remove it from the ToolStrip and select the next item.
        /// </summary>
        private void ComponentChangeSvc_ComponentRemoved(object sender, ComponentEventArgs e)
        {
            if (e.Component is ToolStripItem && ((ToolStripItem)e.Component).Owner == Component)
            {
                ToolStripItem item = (ToolStripItem)e.Component;
                int itemIndex = ToolStrip.Items.IndexOf(item);
                // send notifications.
                try
                {
                    if (itemIndex != -1)
                    {
                        ToolStrip.Items.Remove(item);
                        base.RaiseComponentChanged(TypeDescriptor.GetProperties(Component)["Items"], null, null);
                    }
                }
                finally
                {
                    if (_pendingTransaction != null)
                    {
                        _pendingTransaction.Commit();
                        _pendingTransaction = null;
                    }
                }

                // select the next item or the ToolStrip itself.
                if (ToolStrip.Items.Count > 1)
                {
                    itemIndex = Math.Min(ToolStrip.Items.Count - 1, itemIndex);
                    itemIndex = Math.Max(0, itemIndex);
                }
                else
                {
                    itemIndex = -1;
                }
                LayoutToolStrip();

                //Reset the Glyphs if the item removed is on the OVERFLOW,
                if (item.Placement == ToolStripItemPlacement.Overflow)
                {
                    // Add Glyphs for overflow...
                    RemoveBodyGlyphsForOverflow();
                    AddBodyGlyphsForOverflow();
                }

                if (_toolStripAdornerWindowService != null && _boundsToInvalidate != Rectangle.Empty)
                {
                    _toolStripAdornerWindowService.Invalidate(_boundsToInvalidate);
                    BehaviorService.Invalidate(_boundsToInvalidate);
                }

                if (KeyboardHandlingService.CutOrDeleteInProgress)
                {
                    IComponent targetSelection = (itemIndex == -1) ? (IComponent)ToolStrip : (IComponent)ToolStrip.Items[itemIndex];
                    // if the TemplateNode becomes the targetSelection, then set the targetSelection to null.
                    if (targetSelection != null)
                    {
                        if (targetSelection is DesignerToolStripControlHost)
                        {
                            if (KeyboardHandlingService != null)
                            {
                                KeyboardHandlingService.SelectedDesignerControl = targetSelection;
                            }
                            SelectionService.SetSelectedComponents(null, SelectionTypes.Replace);

                        }
                        else
                        {
                            SelectionService.SetSelectedComponents(new IComponent[] { targetSelection }, SelectionTypes.Replace);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///  Before a ToolStripItem is removed, open a transaction to batch the operation.
        /// </summary>
        private void ComponentChangeSvc_ComponentRemoving(object sender, ComponentEventArgs e)
        {
            if (e.Component is ToolStripItem && ((ToolStripItem)e.Component).Owner == Component)
            {
                Debug.Assert(_host != null, "Why didn't we get a designer host?");
                Debug.Assert(_pendingTransaction == null, "Removing item with pending transaction?");
                try
                {
                    _pendingTransaction = _host.CreateTransaction(SR.ToolStripDesignerTransactionRemovingItem);
                    base.RaiseComponentChanging(TypeDescriptor.GetProperties(Component)["Items"]);
                    if (e.Component is ToolStripDropDownItem dropDownItem)
                    {
                        dropDownItem.HideDropDown();
                        _boundsToInvalidate = dropDownItem.DropDown.Bounds;
                    }
                }
                catch
                {
                    if (_pendingTransaction != null)
                    {
                        _pendingTransaction.Cancel();
                        _pendingTransaction = null;
                    }
                }
            }
        }

        /// <summary>
        ///  Clean up the mess we've made!
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _disposed = true;
                if (_items != null)
                {
                    _items = null;
                }
                if (_selectionSvc != null)
                {
                    _selectionSvc = null;
                }

                EnableDragDrop(false);
                //Dispose of the EitManager
                if (_editManager != null)
                {
                    _editManager.CloseManager();
                    _editManager = null;
                }

                //tear down the TemplateNode
                if (_tn != null)
                {
                    _tn.RollBack();
                    _tn.CloseEditor();
                    _tn = null;
                }

                // teardown the add item button.
                if (_miniToolStrip != null)
                {
                    _miniToolStrip.Dispose();
                    _miniToolStrip = null;
                }

                //tearDown the EditorNode..
                if (_editorNode != null)
                {
                    _editorNode.Dispose();
                    _editorNode = null;
                }

                // tear off the ContextMenu..
                if (_toolStripContextMenu != null)
                {
                    _toolStripContextMenu.Dispose();
                    _toolStripContextMenu = null;
                }
                //Always Remove all the glyphs we added
                RemoveBodyGlyphsForOverflow();
                //tear off the OverFlow if its being shown
                if (ToolStrip.OverflowButton.DropDown.Visible)
                {
                    ToolStrip.OverflowButton.HideDropDown();
                }

                if (_toolStripAdornerWindowService != null)
                {
                    _toolStripAdornerWindowService = null;
                }
            }
            base.Dispose(disposing);
        }

        /// <summary>
        ///  Creates a method signature in the source code file for the default event on the component and navigates the user's cursor to that location in preparation to assign the default action.
        /// </summary>
        public override void DoDefaultAction()
        {
            //Dont Fire the Events if the Component is Inherited.
            if (InheritanceAttribute != InheritanceAttribute.InheritedReadOnly)
            {
                IComponent selectedItem = SelectionService.PrimarySelection as IComponent;
                if (selectedItem == null)
                {
                    if (KeyboardHandlingService != null)
                    {
                        selectedItem = (IComponent)KeyboardHandlingService.SelectedDesignerControl;
                    }

                }
                // if one of the sub-items is selected, delegate to it.
                if (selectedItem is ToolStripItem)
                {
                    if (_host != null)
                    {
                        IDesigner itemDesigner = _host.GetDesigner(selectedItem);
                        if (itemDesigner != null)
                        {
                            itemDesigner.DoDefaultAction();
                            return;
                        }
                    }
                }
                base.DoDefaultAction();
            }
        }

        /// <summary>
        ///  We add our BodyGlyphs as well as bodyGlyphs for the ToolStripItems here.
        /// </summary>
        protected override ControlBodyGlyph GetControlGlyph(GlyphSelectionType selectionType)
        {
            // Get the glyphs iff Handle is created for the toolStrip.
            if (!ToolStrip.IsHandleCreated)
            {
                return null;
            }
            SelectionManager selMgr = (SelectionManager)GetService(typeof(SelectionManager));
            if (selMgr != null && ToolStrip != null && CanAddItems && ToolStrip.Visible)
            {
                object primarySelection = SelectionService.PrimarySelection;
                Behavior.Behavior toolStripBehavior = new ToolStripItemBehavior();
                //sometimes the Collection changes when the ToolStrip gets the Selection and we are in Dummy Insitu edit... so remove that before you access the collection..
                if (ToolStrip.Items.Count > 0)
                {
                    ToolStripItem[] items = new ToolStripItem[ToolStrip.Items.Count];
                    ToolStrip.Items.CopyTo(items, 0);
                    foreach (ToolStripItem toolItem in items)
                    {
                        if (toolItem != null)
                        {
                            ToolStripItemDesigner itemDesigner = (ToolStripItemDesigner)_host.GetDesigner(toolItem);
                            bool isPrimary = (toolItem == primarySelection);
                            if (!isPrimary &&
                                itemDesigner != null &&
                                itemDesigner.IsEditorActive)
                            {

                                itemDesigner.Editor.Commit(false, false);
                            }
                        }
                    }
                }

                // now walk the ToolStrip and add glyphs for each of it's children
                foreach (ToolStripItem item in ToolStrip.Items)
                {
                    if (item is DesignerToolStripControlHost)
                    {
                        continue;
                    }
                    // make sure it's on the ToolStrip...
                    if (item.Placement == ToolStripItemPlacement.Main)
                    {
                        ToolStripItemDesigner itemDesigner = (ToolStripItemDesigner)_host.GetDesigner(item);
                        if (itemDesigner != null)
                        {
                            bool isPrimary = (item == primarySelection);
                            if (isPrimary)
                            {
                                ((ToolStripItemBehavior)toolStripBehavior)._dragBoxFromMouseDown = _dragBoxFromMouseDown;
                            }

                            // Get Back the Current Bounds if current selection is not  a primary selection
                            if (!isPrimary)
                            {
                                item.AutoSize = (itemDesigner != null) ? itemDesigner.AutoSize : true;
                            }

                            Rectangle itemBounds = itemDesigner.GetGlyphBounds();
                            Control parent = ToolStrip.Parent;
                            Rectangle parentBounds = BehaviorService.ControlRectInAdornerWindow(parent);
                            if (IsGlyphTotallyVisible(itemBounds, parentBounds) && item.Visible)
                            {
                                // Add Glyph ONLY AFTER item width is changed...
                                ToolStripItemGlyph bodyGlyphForItem = new ToolStripItemGlyph(item, itemDesigner, itemBounds, toolStripBehavior);
                                itemDesigner.bodyGlyph = bodyGlyphForItem;
                                //Add ItemGlyph to the Collection
                                selMgr.BodyGlyphAdorner.Glyphs.Add(bodyGlyphForItem);
                            }
                        }
                    }
                }
            }
            return (base.GetControlGlyph(selectionType));
        }

        /// <summary>
        ///  We add our SelectionGlyphs here. Since ToolStripItems are components we add the SelectionGlyphs for those in this call as well.
        /// </summary>
        public override GlyphCollection GetGlyphs(GlyphSelectionType selType)
        {
            // get the default glyphs for this component.
            GlyphCollection glyphs = new GlyphCollection();
            ICollection selComponents = SelectionService.GetSelectedComponents();
            foreach (object comp in selComponents)
            {
                if (comp is ToolStrip)
                {
                    GlyphCollection toolStripGlyphs = base.GetGlyphs(selType);
                    glyphs.AddRange(toolStripGlyphs);
                }
                else
                {
                    if (comp is ToolStripItem item && item.Visible)
                    {
                        ToolStripItemDesigner itemDesigner = (ToolStripItemDesigner)_host.GetDesigner(item);
                        if (itemDesigner != null)
                        {
                            itemDesigner.GetGlyphs(ref glyphs, StandardBehavior);
                        }
                    }
                }
            }

            if ((SelectionRules & SelectionRules.Moveable) != 0 && InheritanceAttribute != InheritanceAttribute.InheritedReadOnly && (selType != GlyphSelectionType.NotSelected))
            {
                //get the adornerwindow-relative coords for the container control
                Point loc = BehaviorService.ControlToAdornerWindow((Control)Component);
                Rectangle translatedBounds = new Rectangle(loc, ((Control)Component).Size);
                int glyphOffset = (int)(DesignerUtils.CONTAINERGRABHANDLESIZE * .5);
                //if the control is too small for our ideal position...
                if (translatedBounds.Width < 2 * DesignerUtils.CONTAINERGRABHANDLESIZE)
                {
                    glyphOffset = -1 * glyphOffset;
                }

                ContainerSelectorBehavior behavior = new ContainerSelectorBehavior(ToolStrip, Component.Site, true);
                ContainerSelectorGlyph containerSelectorGlyph = new ContainerSelectorGlyph(translatedBounds, DesignerUtils.CONTAINERGRABHANDLESIZE, glyphOffset, behavior);
                glyphs.Insert(0, containerSelectorGlyph);
            }
            return glyphs;
        }

        /// <summary>
        ///  Allow hit testing over the AddNewItem button only.
        /// </summary>
        protected override bool GetHitTest(Point point)
        {
            // convert to client coords.
            point = Control.PointToClient(point);

            if (_miniToolStrip != null && _miniToolStrip.Visible && AddItemRect.Contains(point))
            {
                return true;
            }
            if (OverFlowButtonRect.Contains(point))
            {
                return true;
            }
            return base.GetHitTest(point);

        }

        /// <summary>
        ///  Get the designer set up to run.
        /// </summary>
        // EditorServiceContext is newed up to add Edit Items verb.
        public override void Initialize(IComponent component)
        {
            base.Initialize(component);
            AutoResizeHandles = true;
            _host = (IDesignerHost)GetService(typeof(IDesignerHost));
            if (_host != null)
            {
                _componentChangeSvc = (IComponentChangeService)_host.GetService(typeof(IComponentChangeService));
            }

            // initialize new Manager For Editing ToolStrips
            _editManager = new ToolStripEditorManager(component);
            // setup the dropdown if our handle has been created.
            if (Control.IsHandleCreated)
            {
                InitializeNewItemDropDown();
            }

            //hookup to the AdornerService..for the overflow dropdown to be parent properly.
            _toolStripAdornerWindowService = (ToolStripAdornerWindowService)GetService(typeof(ToolStripAdornerWindowService));

            // Make sure the overflow is not toplevel
            ToolStrip.OverflowButton.DropDown.TopLevel = false;

            // init the verb.
            if (CanAddItems)
            {
                new EditorServiceContext(this, TypeDescriptor.GetProperties(Component)["Items"], SR.ToolStripItemCollectionEditorVerb);
                //Add the EditService so that the ToolStrip can do its own Tab and Keyboard Handling
                _keyboardHandlingService = (ToolStripKeyboardHandlingService)GetService(typeof(ToolStripKeyboardHandlingService));
                if (_keyboardHandlingService == null)
                {
                    _keyboardHandlingService = new ToolStripKeyboardHandlingService(Component.Site);
                }

                //Add the InsituEditService so that the ToolStrip can do its own Tab and Keyboard Handling
                ISupportInSituService inSituService = (ISupportInSituService)GetService(typeof(ISupportInSituService));
                if (inSituService == null)
                {
                    inSituService = new ToolStripInSituService(Component.Site);
                }
            }

            // ToolStrip is selected...
            _toolStripSelected = true;
            // Reset the TemplateNode Selection if any...
            if (_keyboardHandlingService != null)
            {
                KeyboardHandlingService.SelectedDesignerControl = null;
            }
        }

        /// <summary>
        ///  ControlDesigner overrides this method.  It will look at the default property for the control and, if it is of type string, it will set this property's value to the name of the component.  It only does this if the designer has been configured with this option in the options service.  This method also connects the control to its parent and positions it.  If you override this method, you should always call base.
        /// </summary>
        public override void InitializeNewComponent(IDictionary defaultValues)
        {
            Control parent = defaultValues["Parent"] as Control;
            Form parentForm = _host.RootComponent as Form;
            FormDocumentDesigner parentFormDesigner = null;
            if (parentForm != null)
            {
                parentFormDesigner = _host.GetDesigner(parentForm) as FormDocumentDesigner;
            }

            ToolStripPanel parentPanel = parent as ToolStripPanel;
            // smoke the Dock Property if the toolStrip is getting parented to the ContentPanel.
            if (parentPanel == null && parent is ToolStripContentPanel)
            {
                // smoke the dock property whenever we add a toolstrip to a toolstrip panel.
                PropertyDescriptor dockProp = TypeDescriptor.GetProperties(ToolStrip)["Dock"];
                if (dockProp != null)
                {
                    dockProp.SetValue(ToolStrip, DockStyle.None);
                }
            }

            ///  set up parenting and all the base stuff...
            if (parentPanel == null || ToolStrip is MenuStrip)
            {
                base.InitializeNewComponent(defaultValues);
            }

            if (parentFormDesigner != null)
            {
                //Set MainMenuStrip property
                if (ToolStrip is MenuStrip)
                {
                    PropertyDescriptor mainMenuStripProperty = TypeDescriptor.GetProperties(parentForm)["MainMenuStrip"];
                    if (mainMenuStripProperty != null && mainMenuStripProperty.GetValue(parentForm) == null)
                    {
                        mainMenuStripProperty.SetValue(parentForm, ToolStrip as MenuStrip);
                    }
                }
            }

            if (parentPanel != null)
            {
                if (!(ToolStrip is MenuStrip))
                {
                    PropertyDescriptor controlsProp = TypeDescriptor.GetProperties(parentPanel)["Controls"];

                    if (_componentChangeSvc != null)
                    {
                        _componentChangeSvc.OnComponentChanging(parentPanel, controlsProp);
                    }

                    parentPanel.Join(ToolStrip, parentPanel.Rows.Length);

                    if (_componentChangeSvc != null)
                    {
                        _componentChangeSvc.OnComponentChanged(parentPanel, controlsProp, parentPanel.Controls, parentPanel.Controls);
                    }

                    //Try to fire ComponentChange on the Location Property for ToolStrip.
                    PropertyDescriptor locationProp = TypeDescriptor.GetProperties(ToolStrip)["Location"];
                    if (_componentChangeSvc != null)
                    {
                        _componentChangeSvc.OnComponentChanging(ToolStrip, locationProp);
                        _componentChangeSvc.OnComponentChanged(ToolStrip, locationProp, null, null);
                    }
                }
            }
            // If we are added to any container other than ToolStripPanel.
            else if (parent != null)
            {
                // If we are adding the MenuStrip ... put it at the Last in the Controls Collection so it gets laid out first.
                if (ToolStrip is MenuStrip)
                {
                    int index = -1;
                    foreach (Control c in parent.Controls)
                    {
                        if (c is ToolStrip && (c != ToolStrip))
                        {
                            index = parent.Controls.IndexOf(c);
                        }
                    }
                    if (index == -1)
                    {
                        // always place the toolStrip first.
                        index = parent.Controls.Count - 1;
                    }
                    parent.Controls.SetChildIndex(ToolStrip, index);
                }
                // If we are not a MenuStrip then we still need to be first to be laid out "after the menuStrip"
                else
                {
                    int index = -1;
                    foreach (Control c in parent.Controls)
                    {
                        // If we found an existing toolstrip (and not a menuStrip) then we can just return .. the base would have done correct parenting for us
                        MenuStrip menu = c as MenuStrip;
                        if (c is ToolStrip && menu == null)
                        {
                            return;
                        }
                        if (menu != null)
                        {
                            index = parent.Controls.IndexOf(c);
                            break;
                        }
                    }
                    if (index == -1)
                    {
                        // always place the toolStrip first.
                        index = parent.Controls.Count;
                    }
                    parent.Controls.SetChildIndex(ToolStrip, index - 1);
                }
            }
        }

        /// <summary>
        ///  Setup the "AddNewItem" button
        /// </summary>
        private void InitializeNewItemDropDown()
        {
            if (!CanAddItems || !SupportEditing)
            {
                return;
            }
            ToolStrip toolStrip = (ToolStrip)Component;
            AddNewTemplateNode(toolStrip);
            // set up the right visibility state for the ToolStrip.
            SelSvc_SelectionChanged(null, EventArgs.Empty);
        }

        /// <summary>
        ///  This is called to ascertain if the Glyph is totally visible. This is called from ToolStripMenuItemDesigner too.
        /// </summary>
        internal static bool IsGlyphTotallyVisible(Rectangle itemBounds, Rectangle parentBounds)
        {
            return parentBounds.Contains(itemBounds);
        }

        /// <summary>
        ///  Returns true if the item is on the overflow.
        /// </summary>
        private bool ItemParentIsOverflow(ToolStripItem item)
        {
            ToolStripDropDown topmost = item.Owner as ToolStripDropDown;
            if (topmost != null)
            {
                // walk back up the chain of windows to get the topmost
                while (topmost != null && !(topmost is ToolStripOverflow))
                {
                    if (topmost.OwnerItem != null)
                    {
                        topmost = topmost.OwnerItem.GetCurrentParent() as ToolStripDropDown;
                    }
                    else
                    {
                        topmost = null;
                    }
                }
            }
            return (topmost is ToolStripOverflow);
        }

        /// <summary>
        ///  Sets up the add new button, and invalidates the behavior glyphs if needed so they always stay in sync.
        /// </summary>
        private void LayoutToolStrip()
        {
            if (!_disposed)
            {
                ToolStrip.PerformLayout();
            }
        }

        internal static string NameFromText(string text, Type componentType, IServiceProvider serviceProvider, bool adjustCapitalization)
        {
            string name = NameFromText(text, componentType, serviceProvider);
            if (adjustCapitalization)
            {
                string nameOfRandomItem = NameFromText(null, typeof(ToolStripMenuItem),
                    serviceProvider);
                if (!string.IsNullOrEmpty(nameOfRandomItem) && char.IsUpper(nameOfRandomItem[0]))
                {
                    name = char.ToUpper(name[0], CultureInfo.InvariantCulture) + name.Substring(1);
                }
            }
            return name;
        }

        /// <summary>
        ///  Computes a name from a text label by removing all spaces and non-alphanumeric characters.
        /// </summary>
        internal static string NameFromText(string text, Type componentType, IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                return null;
            }

            INameCreationService nameCreate = serviceProvider.GetService(typeof(INameCreationService)) as INameCreationService;
            IContainer container = (IContainer)serviceProvider.GetService(typeof(IContainer));
            string defaultName;
            if (nameCreate != null && container != null)
            {
                defaultName = nameCreate.CreateName(container, componentType);
            }
            else
            {
                return null;
            }

            Debug.Assert(defaultName != null && defaultName.Length > 0, "Couldn't create default name for item");

            if (text == null || text.Length == 0 || text == "-")
            {
                return defaultName;
            }

            string nameSuffix = componentType.Name;
            // remove all the non letter and number characters. Append length of the item name...
            System.Text.StringBuilder name = new System.Text.StringBuilder(text.Length + nameSuffix.Length);
            bool nextCharToUpper = false;
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (nextCharToUpper)
                {
                    if (char.IsLower(c))
                    {
                        c = char.ToUpper(c, CultureInfo.CurrentCulture);
                    }
                    nextCharToUpper = false;
                }

                if (char.IsLetterOrDigit(c))
                {
                    if (name.Length == 0)
                    {
                        if (char.IsDigit(c))
                        {
                            // most languages don't allow a digit as the first char in an identifier.
                            continue;
                        }

                        if (char.IsLower(c) != char.IsLower(defaultName[0]))
                        {
                            // match up the first char of the generated identifier with the case of the default.
                            if (char.IsLower(c))
                            {
                                c = char.ToUpper(c, CultureInfo.CurrentCulture);
                            }
                            else
                            {
                                c = char.ToLower(c, CultureInfo.CurrentCulture);
                            }
                        }
                    }
                    name.Append(c);
                }
                else
                {
                    if (char.IsWhiteSpace(c))
                    {
                        nextCharToUpper = true;
                    }
                }
            }

            if (name.Length == 0)
            {
                return defaultName;
            }

            name.Append(nameSuffix);
            string baseName = name.ToString();

            // verify we have a valid name.  If not, start appending numbers if it matches one in the container. see if this name matches another one in the container..
            object existingComponent = container.Components[baseName];

            if (existingComponent == null)
            {
                if (!nameCreate.IsValidName(baseName))
                {
                    // we don't have a name collision but this still isn't a valid name...something is wrong and we can't make a valid identifier out of this so bail.
                    return defaultName;
                }
                else
                {
                    return baseName;
                }
            }
            else
            {
                // start appending numbers.
                string newName = baseName;
                for (int indexer = 1; !nameCreate.IsValidName(newName) || container.Components[newName] != null; indexer++)
                {
                    newName = baseName + indexer.ToString(CultureInfo.InvariantCulture);
                }
                return newName;
            }
        }

        /// <summary>
        ///  DesignerContextMenu should be shown when the ToolStripDesigner.
        /// </summary>
        protected override void OnContextMenu(int x, int y)
        {
            Component selComp = SelectionService.PrimarySelection as Component;
            if (selComp is ToolStrip)
            {
                DesignerContextMenu.Show(x, y);
            }
        }

        protected override void OnDragEnter(DragEventArgs de)
        {
            base.OnDragEnter(de);
            SetDragDropEffects(de);
        }

        protected override void OnDragOver(DragEventArgs de)
        {
            base.OnDragOver(de);
            SetDragDropEffects(de);
        }

        /// <summary>
        ///  Add item on Drop and it its a MenuItem, open its dropDown.
        /// </summary>
        protected override void OnDragDrop(DragEventArgs de)
        {
            base.OnDragDrop(de);
            // There is a "drop region" before firstItem which is not included in the "ToolStrip Item glyhs" so if the drop point falls in this drop region we should insert the items at the head instead of the tail of the toolStrip.
            bool dropAtHead = false;
            ToolStrip parentToolStrip = ToolStrip;
            var offset = new Point(de.X, de.Y);
            NativeMethods.MapWindowPoints(IntPtr.Zero, parentToolStrip.Handle, ref offset, 1);
            if (ToolStrip.Orientation == Orientation.Horizontal)
            {
                if (ToolStrip.RightToLeft == RightToLeft.Yes)
                {
                    if (offset.X >= parentToolStrip.Items[0].Bounds.X)
                    {
                        dropAtHead = true;
                    }
                }
                else if (offset.X <= parentToolStrip.Items[0].Bounds.X)
                {
                    dropAtHead = true;
                }
            }
            else
            {
                if (offset.Y <= parentToolStrip.Items[0].Bounds.Y)
                {
                    dropAtHead = true;
                }
            }

            if (de.Data is ToolStripItemDataObject data)
            {
                if (data.Owner == parentToolStrip)
                {
                    string transDesc;
                    ArrayList components = data.DragComponents;
                    ToolStripItem primaryItem = data.PrimarySelection as ToolStripItem;
                    int primaryIndex = -1;
                    bool copy = (de.Effect == DragDropEffects.Copy);

                    if (components.Count == 1)
                    {
                        string name = TypeDescriptor.GetComponentName(components[0]);
                        if (name == null || name.Length == 0)
                        {
                            name = components[0].GetType().Name;
                        }
                        transDesc = string.Format(copy ? SR.BehaviorServiceCopyControl : SR.BehaviorServiceMoveControl, name);
                    }
                    else
                    {
                        transDesc = string.Format(copy ? SR.BehaviorServiceCopyControls : SR.BehaviorServiceMoveControls, components.Count);
                    }

                    // create a transaction so this happens as an atomic unit.
                    DesignerTransaction changeParent = _host.CreateTransaction(transDesc);
                    try
                    {
                        IComponentChangeService changeSvc = (IComponentChangeService)GetService(typeof(IComponentChangeService));
                        if (changeSvc != null)
                        {
                            changeSvc.OnComponentChanging(parentToolStrip, TypeDescriptor.GetProperties(parentToolStrip)["Items"]);
                        }

                        // If we are copying, then we want to make a copy of the components we are dragging
                        if (copy)
                        {
                            // Remember the primary selection if we had one
                            if (primaryItem != null)
                            {
                                primaryIndex = components.IndexOf(primaryItem);
                            }
                            if (KeyboardHandlingService != null)
                            {
                                KeyboardHandlingService.CopyInProgress = true;
                            }
                            components = DesignerUtils.CopyDragObjects(components, Component.Site) as ArrayList;
                            if (KeyboardHandlingService != null)
                            {
                                KeyboardHandlingService.CopyInProgress = false;
                            }
                            if (primaryIndex != -1)
                            {
                                primaryItem = components[primaryIndex] as ToolStripItem;
                            }
                        }

                        if (de.Effect == DragDropEffects.Move || copy)
                        {
                            // Add the item.
                            for (int i = 0; i < components.Count; i++)
                            {
                                if (dropAtHead)
                                {
                                    parentToolStrip.Items.Insert(0, components[i] as ToolStripItem);
                                }
                                else
                                {
                                    parentToolStrip.Items.Add(components[i] as ToolStripItem);
                                }
                            }

                            // show the dropDown for the primarySelection before the Drag-Drop operation started.
                            if (primaryItem is ToolStripDropDownItem primaryDropDownItem)
                            {
                                if (_host.GetDesigner(primaryDropDownItem) is ToolStripMenuItemDesigner dropDownItemDesigner)
                                {
                                    dropDownItemDesigner.InitializeDropDown();
                                }
                            }

                            //Set the Selection ..
                            SelectionService.SetSelectedComponents(new IComponent[] { primaryItem }, SelectionTypes.Primary | SelectionTypes.Replace);
                        }

                        if (changeSvc != null)
                        {
                            changeSvc.OnComponentChanged(parentToolStrip, TypeDescriptor.GetProperties(parentToolStrip)["Items"], null, null);
                        }
                        //fire extra changing/changed events so that the order is "restored" after undo/redo
                        if (copy)
                        {
                            if (changeSvc != null)
                            {
                                changeSvc.OnComponentChanging(parentToolStrip, TypeDescriptor.GetProperties(parentToolStrip)["Items"]);
                                changeSvc.OnComponentChanged(parentToolStrip, TypeDescriptor.GetProperties(parentToolStrip)["Items"], null, null);
                            }
                        }
                        // Refresh Glyphs...
                        BehaviorService.SyncSelection();
                    }

                    catch
                    {
                        if (changeParent != null)
                        {
                            changeParent.Cancel();
                            changeParent = null;
                        }

                    }
                    finally
                    {
                        if (changeParent != null)
                        {
                            changeParent.Commit();
                            changeParent = null;
                        }
                    }
                }
            }
        }

        /// <summary>
        ///  Everytime we add Item .. the TemplateNode needs to go at the end if its not there.
        /// </summary>
        private void OnItemAdded(object sender, ToolStripItemEventArgs e)
        {
            if (_editorNode != null && (e.Item != _editorNode))
            {
                int currentIndexOfEditor = ToolStrip.Items.IndexOf(_editorNode);
                if (currentIndexOfEditor == -1 || currentIndexOfEditor != ToolStrip.Items.Count - 1)
                {
                    // if the editor is not there or not at the end, add it to the end.
                    ToolStrip.SuspendLayout();
                    ToolStrip.Items.Add(_editorNode);
                    ToolStrip.ResumeLayout();
                }
            }
            LayoutToolStrip();
        }

        /// <summary>
        ///  Overriden so that the ToolStrip honors dragging only through container selector glyph.
        /// </summary>
        protected override void OnMouseDragMove(int x, int y)
        {
            if (!SelectionService.GetComponentSelected(ToolStrip))
            {
                base.OnMouseDragMove(x, y);
            }
        }

        /// <summary>
        ///  Controls the dismissal of the drop down, here - we just cancel it
        /// </summary>
        private void OnOverflowDropDownClosing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            //always dismiss this so we don't collapse the dropdown when the user clicks @ design time
            e.Cancel = (e.CloseReason == ToolStripDropDownCloseReason.ItemClicked);
        }

        /// <summary>
        ///  Remove the Glyphs for Items on the overflow when the Overflow closes.
        /// </summary>
        private void OnOverFlowDropDownClosed(object sender, EventArgs e)
        {
            if (_toolStripAdornerWindowService != null && sender is ToolStripDropDownItem ddi)
            {
                _toolStripAdornerWindowService.Invalidate(ddi.DropDown.Bounds);
                RemoveBodyGlyphsForOverflow();
            }
            //select the last item on the parent toolStrip if the current selection is on the DropDown.
            if (SelectionService.PrimarySelection is ToolStripItem curSel && curSel.IsOnOverflow)
            {
                ToolStripItem nextItem = ToolStrip.GetNextItem(ToolStrip.OverflowButton, ArrowDirection.Left);
                if (nextItem != null)
                {
                    SelectionService.SetSelectedComponents(new IComponent[] { nextItem }, SelectionTypes.Replace);
                }
            }
        }

        /// <summary>
        ///  Add Glyphs when the OverFlow opens ....
        /// </summary>
        private void OnOverFlowDropDownOpened(object sender, EventArgs e)
        {
            //Show the TemplateNode
            if (_editorNode != null)
            {
                _editorNode.Control.Visible = true;
                _editorNode.Visible = true;
            }

            ToolStripDropDownItem ddi = sender as ToolStripDropDownItem;
            if (ddi != null)
            {
                RemoveBodyGlyphsForOverflow();
                AddBodyGlyphsForOverflow();
            }
            //select the last item on the parent toolStrip if the current selection is on the DropDown.
            if (!(SelectionService.PrimarySelection is ToolStripItem curSel) || (curSel != null && !curSel.IsOnOverflow))
            {
                ToolStripItem nextItem = ddi.DropDown.GetNextItem(null, ArrowDirection.Down);
                if (nextItem != null)
                {
                    SelectionService.SetSelectedComponents(new IComponent[] { nextItem }, SelectionTypes.Replace);
                    BehaviorService.Invalidate(BehaviorService.ControlRectInAdornerWindow(ToolStrip));
                }
            }
        }

        /// <summary>
        ///  In Order to Draw the Selection Glyphs we need to reforce painting on the  the AdonerWindow.This method forces the repaint
        /// </summary>
        private void OnOverFlowDropDownPaint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            foreach (ToolStripItem item in ToolStrip.Items)
            {
                if (item.Visible && item.IsOnOverflow && SelectionService.GetComponentSelected(item))
                {
                    if (_host.GetDesigner(item) is ToolStripItemDesigner designer)
                    {
                        Rectangle r = designer.GetGlyphBounds();
                        ToolStripDesignerUtils.GetAdjustedBounds(item, ref r);
                        r.Inflate(GLYPHBORDER, GLYPHBORDER);
                        //this will allow any Glyphs to re-paint
                        //after this control and its designer has painted
                        BehaviorService b = (BehaviorService)GetService(typeof(BehaviorService));
                        if (b != null)
                        {
                            b.ProcessPaintMessage(r);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///  Change the parent of the overFlow so that it is parented to the ToolStripAdornerWindow
        /// </summary>
        private void OnOverFlowDropDownOpening(object sender, EventArgs e)
        {
            ToolStripDropDownItem ddi = sender as ToolStripDropDownItem;
            if (ddi.DropDown.TopLevel)
            {
                ddi.DropDown.TopLevel = false;
            }

            if (_toolStripAdornerWindowService != null)
            {
                ToolStrip.SuspendLayout();
                ddi.DropDown.Parent = _toolStripAdornerWindowService.ToolStripAdornerWindowControl;
                ToolStrip.ResumeLayout();
            }
        }

        /// <summary>
        ///  When Items change the size, Recalculate the glyph sizes.
        /// </summary>
        private void OnOverflowDropDownResize(object sender, EventArgs e)
        {
            ToolStripDropDown dropDown = sender as ToolStripDropDown;
            if (dropDown.Visible)
            {
                // Re-Add the Glyphs to refresh the bounds... and Add new ones if new items get pushed into the OverFlow.
                RemoveBodyGlyphsForOverflow();
                AddBodyGlyphsForOverflow();
            }

            if (_toolStripAdornerWindowService != null && dropDown != null)
            {
                _toolStripAdornerWindowService.Invalidate();
            }
        }

        /// <summary>
        ///  Set proper cursor
        /// </summary>
        protected override void OnSetCursor()
        {
            if (_toolboxService == null)
            {
                _toolboxService = (IToolboxService)GetService(typeof(IToolboxService));
            }

            if (_toolboxService == null || !_toolboxService.SetCursor() || InheritanceAttribute.Equals(InheritanceAttribute.InheritedReadOnly))
            {
                Cursor.Current = Cursors.Default;
            }
        }

        /// <summary>
        ///  ResumeLayout after Undone.
        /// </summary>
        private void OnUndone(object source, EventArgs e)
        {
            ///  IMPORTANT : The Undo Unit .. Clears of the ITems....
            if (_editorNode != null && (ToolStrip.Items.IndexOf(_editorNode) == -1))
            {
                ToolStrip.Items.Add(_editorNode);
            }
            if (_undoingCalled)
            {
                // StatusStrip required a ResumeLayout and then a performLayout... So that the Layout is proper after any user-transaction UNDONE.
                ToolStrip.ResumeLayout(true/*performLayout*/);
                ToolStrip.PerformLayout();
                // ReInitialize the Glyphs after Layout is resumed !!
                if (SelectionService.PrimarySelection is ToolStripDropDownItem selectedItem)
                {
                    if (_host.GetDesigner(selectedItem) is ToolStripMenuItemDesigner selectedItemDesigner)
                    {
                        selectedItemDesigner.InitializeBodyGlyphsForItems(false, selectedItem);
                        selectedItemDesigner.InitializeBodyGlyphsForItems(true, selectedItem);
                    }
                }
                _undoingCalled = false;
            }
            BehaviorService.SyncSelection();
        }

        /// <summary>
        ///  SuspendLayout before unDoing.
        /// </summary>
        private void OnUndoing(object source, EventArgs e)
        {
            if (CheckIfItemSelected() || SelectionService.GetComponentSelected(ToolStrip))
            {
                _undoingCalled = true;
                ToolStrip.SuspendLayout();
            }
        }

        /// <summary>
        ///  SyncSelection on ToolStrip move.
        /// </summary>
        private void OnToolStripMove(object sender, System.EventArgs e)
        {
            if (SelectionService.GetComponentSelected(ToolStrip))
            {
                BehaviorService.SyncSelection();
            }
        }

        /// <summary>
        ///  Remove all the glyphs we were are not visible..
        /// </summary>
        private void OnToolStripVisibleChanged(object sender, System.EventArgs e)
        {
            if (sender is ToolStrip tool && !tool.Visible)
            {
                SelectionManager selMgr = (SelectionManager)GetService(typeof(SelectionManager));
                Glyph[] currentBodyGlyphs = new Glyph[selMgr.BodyGlyphAdorner.Glyphs.Count];
                selMgr.BodyGlyphAdorner.Glyphs.CopyTo(currentBodyGlyphs, 0);
                //Remove the ToolStripItemGlyphs.
                foreach (Glyph g in currentBodyGlyphs)
                {
                    if (g is ToolStripItemGlyph)
                    {
                        selMgr.BodyGlyphAdorner.Glyphs.Remove(g);
                    }
                }
            }
        }

        /// <summary>
        ///  Allows a designer to filter the set of properties the component it is designing will expose through the TypeDescriptor object.  This method is called immediately before its corresponding "Post" method. If you are overriding this method you should call the base implementation before you perform your own filtering.
        /// </summary>
        protected override void PreFilterProperties(IDictionary properties)
        {
            base.PreFilterProperties(properties);
            PropertyDescriptor prop;
            string[] shadowProps = new string[] {
               "Visible",
               "AllowDrop",
               "AllowItemReorder"
            };
            Attribute[] empty = Array.Empty<Attribute>();
            for (int i = 0; i < shadowProps.Length; i++)
            {
                prop = (PropertyDescriptor)properties[shadowProps[i]];
                if (prop != null)
                {
                    properties[shadowProps[i]] = TypeDescriptor.CreateProperty(typeof(ToolStripDesigner), prop, empty);
                }
            }
        }

        /// <summary>
        ///  Remove the glyphs for individual items on the DropDown.
        /// </summary>
        private void RemoveBodyGlyphsForOverflow()
        {
            // now walk the ToolStrip and add glyphs for each of it's children
            foreach (ToolStripItem item in ToolStrip.Items)
            {
                if (item is DesignerToolStripControlHost)
                {
                    continue;
                }
                // make sure it's on the Overflow...
                if (item.Placement == ToolStripItemPlacement.Overflow)
                {
                    ToolStripItemDesigner dropDownItemDesigner = (ToolStripItemDesigner)_host.GetDesigner(item);
                    if (dropDownItemDesigner != null)
                    {
                        ControlBodyGlyph glyph = dropDownItemDesigner.bodyGlyph;
                        if (glyph != null && _toolStripAdornerWindowService != null && _toolStripAdornerWindowService.DropDownAdorner.Glyphs.Contains(glyph))
                        {
                            _toolStripAdornerWindowService.DropDownAdorner.Glyphs.Remove(glyph);
                        }
                    }
                }
            }
        }

        // <summary>
        // IMPORTANT FUNCTION: THIS IS CALLED FROM THE ToolStripItemGlyph to rollback
        // the TemplateNode Edition on the Parent ToolStrip.
        // <summary/>
        internal void RollBack()
        {
            if (_tn != null)
            {
                _tn.RollBack();
                _editorNode.Width = _tn.EditorToolStrip.Width;
            }
        }

        // <summary>
        //  Resets the ToolStrip Visible to be the default value
        // <summary/>
        private void ResetVisible()
        {
            Visible = true;
        }

        /// <summary>
        ///  When the Drag Data does not contain ToolStripItem; change the dragEffect to None;  This will result current cursor to change into NO-SMOKING cursor
        /// </summary>
        private void SetDragDropEffects(DragEventArgs de)
        {
            if (de.Data is ToolStripItemDataObject data)
            {
                if (data.Owner != ToolStrip)
                {
                    de.Effect = DragDropEffects.None;
                }
                else
                {
                    de.Effect = (Control.ModifierKeys == Keys.Control) ? DragDropEffects.Copy : DragDropEffects.Move;
                }
            }
        }

        /// <summary>
        ///  When selection changes to the ToolStrip, show the "AddItemsButton", when it leaves, hide it.
        /// </summary>
        private void SelSvc_SelectionChanging(object sender, EventArgs e)
        {
            if (_toolStripSelected)
            {
                // first commit the node
                if (_tn != null && _tn.Active)
                {
                    _tn.Commit(false, false);
                }
            }

            bool showToolStrip = CheckIfItemSelected();
            //Check All the SelectedComponents to find is toolstrips are selected
            if (!showToolStrip && !SelectionService.GetComponentSelected(ToolStrip))
            {
                ToolStrip.Visible = _currentVisible;
                if (!_currentVisible && _parentNotVisible)
                {
                    ToolStrip.Parent.Visible = _currentVisible;
                    _parentNotVisible = false;
                }
                if (ToolStrip.OverflowButton.DropDown.Visible)
                {
                    ToolStrip.OverflowButton.HideDropDown();
                }
                //Always Hide the EditorNode if the ToolStrip Is Not Selected...
                if (_editorNode != null)
                {
                    _editorNode.Visible = false;
                }
                // Show Hide Items...
                ShowHideToolStripItems(false);
                _toolStripSelected = false;
            }
        }

        /// <summary>
        ///  When selection changes to the ToolStrip, show the "AddItemsButton", when it leaves, hide it.
        /// </summary>
        private void SelSvc_SelectionChanged(object sender, EventArgs e)
        {
            if (_miniToolStrip != null && _host != null)
            {
                bool showToolStrip = false;
                bool itemSelected = CheckIfItemSelected();
                showToolStrip = itemSelected || SelectionService.GetComponentSelected(ToolStrip);
                //Check All the SelectedComponents to find is toolstrips are selected
                if (showToolStrip)
                {
                    // If now the ToolStrip is selected,, Hide its Overflow

                    if (SelectionService.GetComponentSelected(ToolStrip))
                    {
                        if (!DontCloseOverflow && ToolStrip.OverflowButton.DropDown.Visible)
                        {
                            ToolStrip.OverflowButton.HideDropDown();
                        }
                    }

                    // Show Hide Items...
                    ShowHideToolStripItems(true);
                    if (!_currentVisible || !Control.Visible)
                    {
                        // Since the control wasn't visible make it visible
                        Control.Visible = true;
                        // make the current parent visible too.
                        if (ToolStrip.Parent is ToolStripPanel && !ToolStrip.Parent.Visible)
                        {
                            _parentNotVisible = true;
                            ToolStrip.Parent.Visible = true;
                        }
                        // Since the GetBodyGlyphs is called before we come here  In this case where the ToolStrip is going from visible==false to visible==true we need to re-add the glyphs for the items.
                        BehaviorService.SyncSelection();
                    }
                    //Always Show the EditorNode if the ToolStripIsSelected and is PrimarySelection or one of item is selected.
                    if (_editorNode != null && (SelectionService.PrimarySelection == ToolStrip || itemSelected))
                    {
                        bool originalSyncSelection = FireSyncSelection;
                        ToolStripPanel parent = ToolStrip.Parent as ToolStripPanel;
                        try
                        {
                            FireSyncSelection = true;
                            _editorNode.Visible = true;
                        }
                        finally
                        {
                            FireSyncSelection = originalSyncSelection;
                        }
                    }

                    // REQUIRED FOR THE REFRESH OF GLYPHS BUT TRY TO BE SMART ABOUT THE REGION TO INVALIDATE....
                    if (!(SelectionService.PrimarySelection is ToolStripItem selectedItem))
                    {
                        if (KeyboardHandlingService != null)
                        {
                            selectedItem = KeyboardHandlingService.SelectedDesignerControl as ToolStripItem;
                        }
                    }
                    _toolStripSelected = true;
                }
            }
        }

        // <summary>
        //  Determines when should the Visible property be serialized.
        // <summary/>
        private bool ShouldSerializeVisible() => !Visible;

        // <summary>
        //  Determines when should the AllowDrop property be serialized.
        // <summary/>
        private bool ShouldSerializeAllowDrop() => (bool)ShadowProperties["AllowDrop"];

        // <summary>
        //  Determines when should the AllowItemReorder property be serialized.
        // <summary/>
        private bool ShouldSerializeAllowItemReorder() => (bool)ShadowProperties["AllowItemReorder"];

        // <summary>
        //  This is the method that gets called when the Designer has to show thwe InSitu Edit Node,
        // <summary/>
        internal void ShowEditNode(bool clicked)
        {
            // SPECIAL LOGIC TO MIMIC THE MAINMENU BEHAVIOR.. PUSH THE TEMPLATE NODE and ADD A MENUITEM HERE...
            if (ToolStrip is MenuStrip)
            {
                // The TemplateNode should no longer be selected.
                if (KeyboardHandlingService != null)
                {
                    KeyboardHandlingService.ResetActiveTemplateNodeSelectionState();
                }
                try
                {
                    ToolStripItem newItem = AddNewItem(typeof(ToolStripMenuItem));
                    if (newItem != null)
                    {
                        if (_host.GetDesigner(newItem) is ToolStripItemDesigner newItemDesigner)
                        {
                            newItemDesigner.dummyItemAdded = true;
                            ((ToolStripMenuItemDesigner)newItemDesigner).InitializeDropDown();
                            try
                            {
                                _addingDummyItem = true;
                                newItemDesigner.ShowEditNode(clicked);
                            }
                            finally
                            {
                                _addingDummyItem = false;
                            }
                        }
                    }
                }
                catch (InvalidOperationException ex)
                {
                    Debug.Assert(NewItemTransaction == null, "NewItemTransaction should have been nulled out and cancelled by now.");
                    IUIService uiService = (IUIService)GetService(typeof(IUIService));
                    uiService.ShowError(ex.Message);

                    if (KeyboardHandlingService != null)
                    {
                        KeyboardHandlingService.ResetActiveTemplateNodeSelectionState();
                    }
                }
            }
        }

        //Helper function to toggle the Item Visibility
        private void ShowHideToolStripItems(bool toolStripSelected)
        {
            //If we arent Selected then turn the TOPLEVEL ITEMS visibility WYSIWYG
            foreach (ToolStripItem item in ToolStrip.Items)
            {
                if (item is DesignerToolStripControlHost)
                {
                    continue;
                }
                // Get the itemDesigner...
                ToolStripItemDesigner itemDesigner = (ToolStripItemDesigner)_host.GetDesigner(item);
                if (itemDesigner != null)
                {
                    itemDesigner.SetItemVisible(toolStripSelected, this);
                }
            }
            if (FireSyncSelection)
            {
                BehaviorService.SyncSelection();
                FireSyncSelection = false;
            }
        }

        // this is required when addition of TemplateNode causes the toolStrip to Layout .. E.g : Spring ToolStripStatusLabel.
        private void ToolStrip_LayoutCompleted(object sender, EventArgs e)
        {
            if (FireSyncSelection)
            {
                BehaviorService.SyncSelection();
            }
        }

        /// <summary>
        ///  Make sure the AddItem button stays in the right spot.
        /// </summary>
        private void ToolStrip_Resize(object sender, EventArgs e)
        {
            if (!_addingDummyItem && !_disposed && (CheckIfItemSelected() || SelectionService.GetComponentSelected(ToolStrip)))
            {
                if (_miniToolStrip != null && _miniToolStrip.Visible)
                {
                    LayoutToolStrip();
                }
                BehaviorService.SyncSelection();
            }
        }

        /// <summary>
        ///  Handle lower level mouse input.
        /// </summary>
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WindowMessages.WM_CONTEXTMENU:
                    int x = NativeMethods.Util.SignedLOWORD(m.LParam);
                    int y = NativeMethods.Util.SignedHIWORD(m.LParam);
                    bool inBounds = GetHitTest(new Point(x, y));
                    if (inBounds)
                    {
                        return;
                    }
                    base.WndProc(ref m);
                    break;
                case WindowMessages.WM_LBUTTONDOWN:
                case WindowMessages.WM_RBUTTONDOWN:
                    // commit any insitu if any...
                    Commit();
                    base.WndProc(ref m);
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }
    }
}
