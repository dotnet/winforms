// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Windows.Forms.Design.Behavior;

namespace System.Windows.Forms.Design;

internal class ToolStripItemDesigner : ComponentDesigner
{
    private const int GLYPHINSET = 2;
    // Cached in value of the TemplateNode (which is the InSitu Editor)
    private ToolStripTemplateNode _editorNode;
    // Used by the ParentDesigner (ToolStripDesigner) to know whether there is any active Editor.
    private bool _isEditorActive;
    // this property is used in the InitializeNewComponent not to set the text for the ToolstripItem
    private bool _internalCreate;
    // hook to SelectionService to listen to SelectionChanged
    private ISelectionService _selectionService;
    // ToolStripItems Visibility needs to be WYSIWYG.
    private bool _currentVisible;
    // Required to remove Body Glyphs...
    internal ControlBodyGlyph _bodyGlyph;
    // bool which is set if we Add Dummy Item
    internal bool _dummyItemAdded;
    // Needed to Store the DRAGDROP Rect from the ToolStripItemBehavior.
    internal Rectangle _dragBoxFromMouseDown = Rectangle.Empty;
    // defaulted to invalid index. this will be set by the behavior.
    internal int _indexOfItemUnderMouseToDrag = -1;
    private ToolStripItemCustomMenuItemCollection _toolStripItemCustomMenuItemCollection;

    internal bool AutoSize
    {
        get => (bool)ShadowProperties[nameof(AutoSize)];
        set
        {
            bool autoSize = (bool)ShadowProperties[nameof(AutoSize)];
            // always set this in regardless of whether the property changed.
            // it can come back to bite later after in-situ editing if we don't.
            ShadowProperties[nameof(AutoSize)] = value;
            if (value != autoSize)
            {
                ToolStripItem.AutoSize = value;
            }
        }
    }

    private string AccessibleName
    {
        get
        {
            return (string)ShadowProperties[nameof(AccessibleName)];
        }
        set
        {
            ShadowProperties[nameof(AccessibleName)] = value;
        }
    }

    /// <summary>
    ///  Associated Parent Designer
    /// </summary>
    internal override bool CanBeAssociatedWith(IDesigner parentDesigner)
    {
        return (parentDesigner is ToolStripDesigner);
    }

    /// <summary>
    ///  Designer Custom ContextMenu.
    /// </summary>
    private ContextMenuStrip DesignerContextMenu
    {
        get
        {
            BaseContextMenuStrip toolStripContextMenu = new(Component.Site);
            // If multiple Items Selected don't show the custom properties...
            if (_selectionService.SelectionCount > 1)
            {
                toolStripContextMenu.GroupOrdering.Clear();
                toolStripContextMenu.GroupOrdering.AddRange([StandardGroups.Code, StandardGroups.Selection, StandardGroups.Edit, StandardGroups.Properties]);
            }
            else
            {
                toolStripContextMenu.GroupOrdering.Clear();
                toolStripContextMenu.GroupOrdering.AddRange([StandardGroups.Code, StandardGroups.Custom, StandardGroups.Selection, StandardGroups.Edit, StandardGroups.Properties]);
                toolStripContextMenu.Text = "CustomContextMenu";
                _toolStripItemCustomMenuItemCollection ??= new ToolStripItemCustomMenuItemCollection(Component.Site, ToolStripItem);

                foreach (ToolStripItem item in _toolStripItemCustomMenuItemCollection)
                {
                    toolStripContextMenu.Groups[StandardGroups.Custom].Items.Add(item);
                }
            }

            // Refresh the list on every show..
            _toolStripItemCustomMenuItemCollection?.RefreshItems();

            toolStripContextMenu.Populated = false;
            return toolStripContextMenu;
        }
    }

    /// <summary>
    ///  ToolStripEditorManager used this internal property to Activate the editor.
    /// </summary>
    internal virtual ToolStripTemplateNode Editor
    {
        get => _editorNode;
        set => _editorNode = value;
    }

    // ToolStripItems if Inherited ACT as Readonly.
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
    ///  ToolStripEditorManager used this internal property to set the designer's IsEditorActive to notify
    ///  if this item has entered or exited the InSitu Edit Mode.
    /// </summary>
    internal bool IsEditorActive
    {
        get => _isEditorActive;
        set => _isEditorActive = value;
    }

    /// <summary>
    ///  When the ToolStripItem is created we don't want InitializeNewComponent to set the "text" we do it ourselves
    ///  from the Text the User has provided in the InSitu Edit Mode.
    ///  Reason being the item and the Parent unnecessarily Layout and cause flicker.
    /// </summary>
    internal bool InternalCreate
    {
        get => _internalCreate;
        set => _internalCreate = value;
    }

    protected IComponent ImmediateParent
    {
        get
        {
            if (ToolStripItem is not null)
            {
                ToolStrip parent = ToolStripItem.GetCurrentParent();
                return parent ?? ToolStripItem.Owner;
            }

            return null;
        }
    }

    private ToolStripItemOverflow Overflow
    {
        get => (ToolStripItemOverflow)ShadowProperties[nameof(Overflow)];
        set
        {
            // first Hide the Overflow..
            if (ToolStripItem.IsOnOverflow)
            {
                ToolStrip strip = ToolStripItem.Owner;
                if (strip.OverflowButton.DropDown.Visible)
                {
                    strip.OverflowButton.HideDropDown();
                }
            }

            if (ToolStripItem is ToolStripDropDownItem)
            {
                ToolStripDropDownItem item = ToolStripItem as ToolStripDropDownItem;
                item.HideDropDown();
            }

            // Set the value on actual item
            if (value != ToolStripItem.Overflow)
            {
                ToolStripItem.Overflow = value;
                ShadowProperties[nameof(Overflow)] = value;
            }

            // Since this causes the whole layout to change call SyncSelection to reset the glyphs
            GetService<BehaviorService>()?.SyncSelection();
        }
    }

    protected override IComponent ParentComponent
    {
        get
        {
            if (ToolStripItem is not null)
            {
                if (ToolStripItem.IsOnDropDown && !ToolStripItem.IsOnOverflow)
                {
                    if (ImmediateParent is ToolStripDropDown parentDropDown)
                    {
                        return parentDropDown.IsAutoGenerated ? parentDropDown.OwnerItem : parentDropDown;
                    }
                }

                return GetMainToolStrip();
            }

            return null;
        }
    }

    /// <summary>
    ///  Easy method for getting to the ToolStripItem
    /// </summary>
    public ToolStripItem ToolStripItem
    {
        get => (ToolStripItem)Component;
    }

    protected bool Visible
    {
        get => (bool)ShadowProperties[nameof(Visible)];
        set
        {
            ShadowProperties[nameof(Visible)] = value;
            _currentVisible = value;
        }
    }

    /// <summary>
    ///  This method adds the Parent Hierarchy to a list and returns that list to the
    ///  Base ContextMenu provider. This way the ToolStripItem can show the right parents in
    ///  the contextMenu.
    /// </summary>
    internal List<Component> AddParentTree()
    {
        List<Component> parentControls = [];
        if (!TryGetService(out IDesignerHost designerHost))
        {
            return parentControls;
        }

        IComponent root = designerHost.RootComponent;
        Component startComp = ToolStripItem;
        if (startComp is null || root is null)
        {
            return parentControls;
        }

        while (startComp != root)
        {
            if (startComp is ToolStripItem)
            {
                ToolStripItem item = startComp as ToolStripItem;
                if (item.IsOnDropDown)
                {
                    if (item.IsOnOverflow)
                    {
                        parentControls.Add(item.Owner);
                        startComp = item.Owner;
                    }
                    else
                    {
                        if (item.Owner is ToolStripDropDown parentDropDown)
                        {
                            ToolStripItem ownerItem = parentDropDown.OwnerItem;
                            if (ownerItem is not null)
                            {
                                parentControls.Add(ownerItem);
                                startComp = ownerItem;
                            }
                        }
                    }
                }
                else
                {
                    if (item.Owner.Site is not null)
                    {
                        parentControls.Add(item.Owner);
                    }

                    startComp = item.Owner;
                }
            }
            else if (startComp is Control)
            {
                Control selectedControl = startComp as Control;
                Control parentControl = selectedControl.Parent;
                if (parentControl.Site is not null)
                {
                    parentControls.Add(parentControl);
                }

                startComp = parentControl;
            }
        }

        return parentControls;
    }

    /// <summary>
    ///  Creates the InSitu Edit Node (which is called the TemplateNode).
    /// </summary>
    private void CreateDummyNode()
    {
        _editorNode = new ToolStripTemplateNode(ToolStripItem, ToolStripItem.Text);
    }

    /// <summary>
    ///  This is called by the TemplateNode to Commit the Edit.
    ///  This Function Simply changes the "Text and Image" property of the current ToolStripItem.
    /// </summary>
    // Standard 'catch all - rethrow critical' exception pattern
    internal virtual void CommitEdit(Type type, string text, bool commit, bool enterKeyPressed, bool tabKeyPressed)
    {
        ToolStripItem newItem = null;
        SelectionManager selectionManager = GetService<SelectionManager>();
        BehaviorService behaviorService = GetService<BehaviorService>();

        ToolStrip immediateParent = ImmediateParent as ToolStrip;
        immediateParent.SuspendLayout();
        HideDummyNode();

        IDesignerHost designerHost = GetService<IDesignerHost>();
        ToolStripDesigner designer = (ToolStripDesigner)designerHost.GetDesigner(ToolStripItem.Owner);
        designer?.EditManager?.ActivateEditor(null);

        // Cannot Add ToolStripSeparator to MenuStrip
        if (immediateParent is MenuStrip
            && type == typeof(ToolStripSeparator)
            && TryGetService(out IUIService uiService))
        {
            uiService.ShowError(SR.ToolStripSeparatorError);
            commit = false;

            // Select the MenuStrip
            _selectionService?.SetSelectedComponents(new object[] { immediateParent });
        }

        if (commit)
        {
            if (_dummyItemAdded)
            {
                try
                {
                    RemoveItem();
                    newItem = designer.AddNewItem(type, text, enterKeyPressed, false /* Don't select the templateNode but select the newly added item */);
                }
                finally
                {
                    if (designer.NewItemTransaction is not null)
                    {
                        designer.NewItemTransaction.Commit();
                        designer.NewItemTransaction = null;
                    }
                }
            }
            else
            {
                // create our transaction
                DesignerTransaction designerTransaction = designerHost.CreateTransaction(SR.ToolStripItemPropertyChangeTransaction);
                try
                {
                    // Change the Text.
                    PropertyDescriptor textProp = TypeDescriptor.GetProperties(ToolStripItem)["Text"];
                    string oldValue = (string)textProp.GetValue(ToolStripItem);
                    if (textProp is not null && text != oldValue)
                    {
                        textProp.SetValue(ToolStripItem, text);
                    }

                    if (enterKeyPressed && _selectionService is not null)
                    {
                        SelectNextItem(enterKeyPressed, designer);
                    }
                }
                catch (Exception e)
                {
                    if (designerTransaction is not null)
                    {
                        designerTransaction.Cancel();
                        designerTransaction = null;
                    }

                    selectionManager?.Refresh();

                    if (e.IsCriticalException())
                    {
                        throw;
                    }
                }
                finally
                {
                    designerTransaction?.Commit();
                }
            }

            // Reset the DummyItem flag
            _dummyItemAdded = false;
        }
        else
        {
            // Refresh on SelectionManager... To Change Glyph Size.
            if (_dummyItemAdded)
            {
                _dummyItemAdded = false;
                RemoveItem();

                if (designer.NewItemTransaction is not null)
                {
                    designer.NewItemTransaction.Cancel();
                    designer.NewItemTransaction = null;
                }
            }
        }

        immediateParent.ResumeLayout();
        if (newItem is not null && !newItem.IsOnDropDown)
        {
            if (newItem is ToolStripDropDownItem dropDown)
            {
                ToolStripItemDesigner itemDesigner = (ToolStripItemDesigner)designerHost.GetDesigner(newItem);
                Rectangle itemBounds = itemDesigner.GetGlyphBounds();
                if (designerHost.RootComponent is Control parent)
                {
                    if (behaviorService is not null)
                    {
                        Rectangle parentBounds = behaviorService.ControlRectInAdornerWindow(parent);
                        if (!ToolStripDesigner.IsGlyphTotallyVisible(itemBounds, parentBounds))
                        {
                            dropDown.HideDropDown();
                        }
                    }
                }
            }
        }

        // used the SelectionManager to Add the glyphs.
        selectionManager?.Refresh();
    }

    /// <summary>
    ///  Disposes of this designer.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (_editorNode is not null)
            {
                _editorNode.CloseEditor();
                _editorNode = null;
            }

            if (ToolStripItem is not null)
            {
                ToolStripItem.Paint -= OnItemPaint;
            }

            // Now, unhook the component rename event
            if (TryGetService(out IComponentChangeService cs))
            {
                cs.ComponentRename -= OnComponentRename;
            }

            if (_selectionService is not null)
            {
                _selectionService.SelectionChanged -= OnSelectionChanged;
            }

            // Clean up the ToolStripItem Glyph if Any
            if (_bodyGlyph is not null && TryGetService(out ToolStripAdornerWindowService toolStripAdornerWindowService)
                && toolStripAdornerWindowService.DropDownAdorner.Glyphs.Contains(_bodyGlyph))
            {
                toolStripAdornerWindowService.DropDownAdorner.Glyphs.Remove(_bodyGlyph);
            }

            // Remove the Collection
            if (_toolStripItemCustomMenuItemCollection is not null && _toolStripItemCustomMenuItemCollection.Count > 0)
            {
                foreach (ToolStripItem item in _toolStripItemCustomMenuItemCollection)
                {
                    item.Dispose();
                }

                _toolStripItemCustomMenuItemCollection.Clear();
            }

            _toolStripItemCustomMenuItemCollection = null;
        }

        base.Dispose(disposing);
    }

    /// <summary>
    ///  Returns the owner of the current ToolStripItem.
    /// </summary>
    protected virtual Component GetOwnerForActionList() => (ToolStripItem.Placement == ToolStripItemPlacement.Main) ? ToolStripItem.GetCurrentParent() : ToolStripItem.Owner;

    internal virtual ToolStrip GetMainToolStrip() => ToolStripItem.Owner;

    public Rectangle GetGlyphBounds()
    {
        Rectangle r = Rectangle.Empty;
        if (TryGetService(out BehaviorService b) && ImmediateParent is not null)
        {
            Point loc = b.ControlToAdornerWindow((Control)ImmediateParent);
            r = ToolStripItem.Bounds;
            r.Offset(loc);
        }

        return r;
    }

    // Need to Fire ComponentChanging on all the DropDownItems. Please see "MorphToolStripItem" function for more details.
    private void FireComponentChanging(ToolStripDropDownItem parent)
    {
        if (parent is null)
        {
            return;
        }

        if (TryGetService(out IComponentChangeService changeService) && parent.Site is not null)
        {
            changeService.OnComponentChanging(parent, TypeDescriptor.GetProperties(parent)["DropDownItems"]);
        }

        foreach (ToolStripItem item in parent.DropDownItems)
        {
            // Don't Serialize the DesignerToolStripControlHost...
            if (item is ToolStripDropDownItem dropDownItem && dropDownItem.DropDownItems.Count > 1 /*including TN*/)
            {
                FireComponentChanging(dropDownItem);
            }
        }
    }

    private void FireComponentChanged(ToolStripDropDownItem parent)
    {
        if (parent is null)
        {
            return;
        }

        if (TryGetService(out IComponentChangeService changeService) && parent.Site is not null)
        {
            changeService.OnComponentChanged(parent, TypeDescriptor.GetProperties(parent)["DropDownItems"]);
        }

        foreach (ToolStripItem item in parent.DropDownItems)
        {
            // Don't Serialize the DesignerToolStripControlHost...
            if (item is ToolStripDropDownItem dropDownItem && dropDownItem.DropDownItems.Count > 1 /*including TN*/)
            {
                FireComponentChanged(dropDownItem);
            }
        }
    }

    public void GetGlyphs(ref GlyphCollection glyphs, Behavior.Behavior standardBehavior)
    {
        if (ImmediateParent is null)
        {
            return;
        }

        Rectangle r = GetGlyphBounds();
        ToolStripDesignerUtils.GetAdjustedBounds(ToolStripItem, ref r);
        Rectangle parentBounds = GetService<BehaviorService>().ControlRectInAdornerWindow((Control)ImmediateParent);
        if (parentBounds.Contains(r.Left, r.Top))
        {
            // Don't paint the glyphs if we are opening a DropDown...
            if (ToolStripItem.IsOnDropDown)
            {
                ToolStrip parent = ToolStripItem.GetCurrentParent();
                parent ??= ToolStripItem.Owner;

                if (parent is not null && parent.Visible)
                {
                    glyphs.Add(new MiniLockedBorderGlyph(r, SelectionBorderGlyphType.Top, standardBehavior));
                    glyphs.Add(new MiniLockedBorderGlyph(r, SelectionBorderGlyphType.Bottom, standardBehavior));
                    glyphs.Add(new MiniLockedBorderGlyph(r, SelectionBorderGlyphType.Left, standardBehavior));
                    glyphs.Add(new MiniLockedBorderGlyph(r, SelectionBorderGlyphType.Right, standardBehavior));
                }
            }
            else
            {
                glyphs.Add(new MiniLockedBorderGlyph(r, SelectionBorderGlyphType.Top, standardBehavior));
                glyphs.Add(new MiniLockedBorderGlyph(r, SelectionBorderGlyphType.Bottom, standardBehavior));
                glyphs.Add(new MiniLockedBorderGlyph(r, SelectionBorderGlyphType.Left, standardBehavior));
                glyphs.Add(new MiniLockedBorderGlyph(r, SelectionBorderGlyphType.Right, standardBehavior));
            }
        }
    }

    /// <summary>
    ///  Returns the root dropdown in the chain.
    /// </summary>
    internal static ToolStripDropDown GetFirstDropDown(ToolStripItem currentItem)
    {
        if (currentItem.Owner is ToolStripDropDown)
        {
            ToolStripDropDown topmost = currentItem.Owner as ToolStripDropDown;
            // walk back up the chain of windows to get the topmost
            while (topmost.OwnerItem is not null && (topmost.OwnerItem.Owner is ToolStripDropDown))
            {
                topmost = topmost.OwnerItem.Owner as ToolStripDropDown;
            }

            return topmost;
        }

        return null;
    }

    /// <summary>
    ///  This helper function resets the AutoSize property so that the item SNAPS back to its "preferredSize".
    /// </summary>
    private void HideDummyNode()
    {
        ToolStripItem.AutoSize = AutoSize;
        if (_editorNode is not null)
        {
            _editorNode.CloseEditor();
            _editorNode = null;
        }
    }

    /// <summary>
    ///  Get the designer set up to run.
    /// </summary>
    public override void Initialize(IComponent component)
    {
        base.Initialize(component);

        // Shadow AutoSize
        AutoSize = ToolStripItem.AutoSize;
        Visible = true;
        _currentVisible = Visible;

        // Shadow the AccessibleName as we are going to change it at DesignTime
        AccessibleName = ToolStripItem.AccessibleName;
        ToolStripItem.Paint += OnItemPaint;

        // Change the AccessibleName to point to ToolStirpItem.Name
        ToolStripItem.AccessibleName = ToolStripItem.Name;

        // Now, hook the component rename event so we can update the AccessibleName
        if (TryGetService(out IComponentChangeService cs))
        {
            cs.ComponentRename += OnComponentRename;
        }

        if (TryGetService(out _selectionService))
        {
            _selectionService.SelectionChanged += OnSelectionChanged;
        }
    }

    /// <summary>
    ///  Overridden to always initialize the ToolStripItem with Text property.
    /// </summary>
    public override void InitializeNewComponent(IDictionary defaultValues)
    {
        // Set the Text only if the item is not created internally (via InSitu Edit)
        if (!_internalCreate)
        {
            ISite site = Component.Site;
            if (site is not null && Component is ToolStripDropDownItem)
            {
                defaultValues ??= new Hashtable();

                defaultValues["Text"] = site.Name;
                IComponent component = Component;
                PropertyDescriptor pd = TypeDescriptor.GetProperties(ToolStripItem)["Text"];

                if (pd is not null && pd.PropertyType.Equals(typeof(string)))
                {
                    string current = (string)pd.GetValue(component);
                    if (current is null || current.Length == 0)
                    {
                        pd.SetValue(component, site.Name);
                    }
                }
            }
        }

        base.InitializeNewComponent(defaultValues);
        // ComboBoxes and TextBoxes shouldn't have Texts... In TextBoxBaseDesigner we do similar thing where
        // we call the base (which sets the text) and then reset it back.
        if (Component is ToolStripTextBox or ToolStripComboBox)
        {
            PropertyDescriptor textProp = TypeDescriptor.GetProperties(Component)["Text"];
            if (textProp is not null && textProp.PropertyType == typeof(string) && !textProp.IsReadOnly && textProp.IsBrowsable)
            {
                textProp.SetValue(Component, string.Empty);
            }
        }
    }

    /// <summary>
    ///  This will morph the current item to the provided type "t" of the item...
    /// </summary>
    internal virtual ToolStripItem MorphCurrentItem(Type t)
    {
        ToolStripItem newItem = null;
        if (!TryGetService(out IDesignerHost host))
        {
            Debug.Fail("Couldn't get designer host!");
            return newItem;
        }

        // create our transaction
        DesignerTransaction designerTransaction = host.CreateTransaction(SR.ToolStripMorphingItemTransaction);
        ToolStrip parent = (ToolStrip)ImmediateParent;
        // Special case overflow...
        if (parent is ToolStripOverflow)
        {
            parent = ToolStripItem.Owner;
        }

        ToolStripMenuItemDesigner ownerItemDesigner = null;

        int dummyIndex = parent.Items.IndexOf(ToolStripItem);
        string name = ToolStripItem.Name;
        ToolStripItem ownerItem = null;

        // Get the main ToolStrip to Set the Glyph for the new Item once it is MORPHED.
        if (ToolStripItem.IsOnDropDown)
        {
            if (ImmediateParent is ToolStripDropDown parentDropDown)
            {
                ownerItem = parentDropDown.OwnerItem;
                if (ownerItem is not null)
                {
                    ownerItemDesigner = (ToolStripMenuItemDesigner)host.GetDesigner(ownerItem);
                }
            }
        }

        try
        {
            // turn off Adding and Added Transactions..
            ToolStripDesigner.s_autoAddNewItems = false;
            SerializationStore serializedData = null;

            if (!TryGetService(out ComponentSerializationService serializationService))
            {
                return newItem;
            }

            serializedData = serializationService.CreateStore();
            serializationService.Serialize(serializedData, Component); // notice the use of component... since we want to preserve the type.

            // Serialize all the DropDownItems for this Item....
            SerializationStore _serializedDataForDropDownItems = null;
            ToolStripDropDownItem dropDownItem = ToolStripItem as ToolStripDropDownItem;
            if (dropDownItem is not null && typeof(ToolStripDropDownItem).IsAssignableFrom(t))
            {
                // Hide the DropDown.
                dropDownItem.HideDropDown();
                _serializedDataForDropDownItems = serializationService.CreateStore();
                SerializeDropDownItems(dropDownItem, ref _serializedDataForDropDownItems, serializationService);
                // close the SerializationStore to Serialize Items..
                _serializedDataForDropDownItems.Close();
            }

            // close the SerializationStore to Serialize the ToolStripItem
            serializedData.Close();

            // Remove the currentItem that is getting morphed..
            if (TryGetService(out IComponentChangeService changeService))
            {
                if (parent.Site is not null)
                {
                    changeService.OnComponentChanging(parent, TypeDescriptor.GetProperties(parent)["Items"]);
                }
                else if (ownerItem is not null)
                {
                    changeService.OnComponentChanging(ownerItem, TypeDescriptor.GetProperties(ownerItem)["DropDownItems"]);
                    changeService.OnComponentChanged(ownerItem, TypeDescriptor.GetProperties(ownerItem)["DropDownItems"]);
                }
            }

            FireComponentChanging(dropDownItem);
            parent.Items.Remove(ToolStripItem);
            host.DestroyComponent(ToolStripItem);
            // Create our new Item
            ToolStripItem component = (ToolStripItem)host.CreateComponent(t, name);
            // Since destroying the original item took away its DropDownItems. We need to Deserialize the items again...
            if (component is ToolStripDropDownItem)
            {
                if (_serializedDataForDropDownItems is not null)
                {
                    serializationService.Deserialize(_serializedDataForDropDownItems);
                }
            }

            // Now deserialize the newItem to morph to the old item...
            serializationService.DeserializeTo(serializedData, host.Container, false, true);
            // Add the new Item...
            newItem = (ToolStripItem)host.Container.Components[name];
            // Set the Image property and DisplayStyle...
            if (newItem.Image is null && newItem is ToolStripButton)
            {
                Image image = null;
                try
                {
                    image = new Icon(typeof(ToolStripButton), "blank").ToBitmap();
                }
                catch (Exception ex) when (!ex.IsCriticalException())
                {
                }

                PropertyDescriptor imageProperty = TypeDescriptor.GetProperties(newItem)["Image"];
                Debug.Assert(imageProperty is not null, "Could not find 'Image' property in ToolStripItem.");
                if (imageProperty is not null && image is not null)
                {
                    imageProperty.SetValue(newItem, image);
                }

                PropertyDescriptor dispProperty = TypeDescriptor.GetProperties(newItem)["DisplayStyle"];
                Debug.Assert(dispProperty is not null, "Could not find 'DisplayStyle' property in ToolStripItem.");
                dispProperty?.SetValue(newItem, ToolStripItemDisplayStyle.Image);

                PropertyDescriptor imageTransProperty = TypeDescriptor.GetProperties(newItem)["ImageTransparentColor"];
                Debug.Assert(imageTransProperty is not null, "Could not find 'DisplayStyle' property in ToolStripItem.");
                imageTransProperty?.SetValue(newItem, Color.Magenta);
            }

            parent.Items.Insert(dummyIndex, newItem);
            if (changeService is not null)
            {
                if (parent.Site is not null)
                {
                    changeService.OnComponentChanged(parent, TypeDescriptor.GetProperties(parent)["Items"]);
                }
                else if (ownerItem is not null)
                {
                    changeService.OnComponentChanging(ownerItem, TypeDescriptor.GetProperties(ownerItem)["DropDownItems"]);
                    changeService.OnComponentChanged(ownerItem, TypeDescriptor.GetProperties(ownerItem)["DropDownItems"]);
                }
            }

            FireComponentChanged(dropDownItem);
            // Add the Glyph for the DropDown ... We are responsible for the Glyph Addition
            // since BodyGlyphs for DropDownItems are added by us.
            if (newItem.IsOnDropDown && ownerItemDesigner is not null)
            {
                ownerItemDesigner.RemoveItemBodyGlyph(newItem);
                ownerItemDesigner.AddItemBodyGlyph(newItem);
            }

            // re start the ComponentAdding/Added events
            ToolStripDesigner.s_autoAddNewItems = true;
            // Invalidate the AdornerWindow to refresh selectionglyphs.
            if (newItem is not null)
            {
                if (newItem is ToolStripSeparator)
                {
                    parent.PerformLayout();
                }

                BehaviorService windowService = (BehaviorService)newItem.Site.GetService(typeof(BehaviorService));
                windowService?.Invalidate();

                // set the selection to our new item.. since we destroyed Original component.. we have to ask SelectionService from new Component
                ISelectionService selSvc = (ISelectionService)newItem.Site.GetService(typeof(ISelectionService));
                selSvc?.SetSelectedComponents(new object[] { newItem }, SelectionTypes.Replace);
            }

            return newItem;
        }
        catch
        {
            host.Container.Add(ToolStripItem);
            parent.Items.Insert(dummyIndex, ToolStripItem);
            if (designerTransaction is not null)
            {
                designerTransaction.Cancel();
                designerTransaction = null;
            }
        }
        finally
        {
            designerTransaction?.Commit();
        }

        return newItem;
    }

    /// <summary>
    ///  Raised when a component's name changes. Here we update the AccessibleName Property to match the newName.
    /// </summary>
    private void OnComponentRename(object sender, ComponentRenameEventArgs e)
    {
        if (e.Component == ToolStripItem)
        {
            ToolStripItem.AccessibleName = e.NewName;
        }
    }

    /// <summary>
    ///  This can be used for OVERFLOW !!!
    /// </summary>
    private void OnItemPaint(object sender, PaintEventArgs e)
    {
        if (ToolStripItem.GetCurrentParent() is ToolStripDropDown
            && _selectionService is not null && !IsEditorActive
            && ToolStripItem.Equals(_selectionService.PrimarySelection)
            && TryGetService(out BehaviorService behaviorService))
        {
            Point location = behaviorService.ControlToAdornerWindow((Control)ImmediateParent);
            Rectangle r = ToolStripItem.Bounds;
            r.Offset(location);
            r.Inflate(GLYPHINSET, GLYPHINSET);
            // this will allow any Glyphs to re-paint
            // after this control and its designer has painted
            behaviorService.ProcessPaintMessage(r);
        }
    }

    /// <summary>
    ///  For ToolStripItems that are not MenuItems and are on Dropdown we need ot update Selection Rect.
    /// </summary>
    private void OnSelectionChanged(object sender, EventArgs e)
    {
        if (sender is not ISelectionService sSvc)
        {
            return;
        }

        // determine if we are selected
        ToolStripItem currentSelection = sSvc.PrimarySelection as ToolStripItem;
        // Accessibility information
        if (ToolStripItem.AccessibilityObject is ToolStripItem.ToolStripItemAccessibleObject acc)
        {
            acc.AddState(AccessibleStates.None);
            ToolStrip tool = GetMainToolStrip();
            if (sSvc.GetComponentSelected(ToolStripItem))
            {
                ToolStrip owner = ImmediateParent as ToolStrip;
                int focusIndex = 0;
                if (owner is not null)
                {
                    focusIndex = owner.Items.IndexOf(currentSelection);
                }

                acc.AddState(AccessibleStates.Selected);
                if (tool is not null)
                {
                    PInvoke.NotifyWinEvent(
                        (uint)AccessibleEvents.SelectionAdd,
                        owner,
                        (int)OBJECT_IDENTIFIER.OBJID_CLIENT,
                        focusIndex + 1);
                }

                if (currentSelection == ToolStripItem)
                {
                    acc.AddState(AccessibleStates.Focused);
                    if (tool is not null)
                    {
                        PInvoke.NotifyWinEvent(
                            (uint)AccessibleEvents.Focus,
                            owner,
                            (int)OBJECT_IDENTIFIER.OBJID_CLIENT,
                            focusIndex + 1);
                    }
                }
            }
        }

        if (currentSelection is not null && currentSelection.Equals(ToolStripItem) && !(ToolStripItem is ToolStripMenuItem))
        {
            if (currentSelection.IsOnDropDown)
            {
                // If the Item is on DropDown show its DropDown and all Parent Dropdown if not visible..
                if (TryGetService(out IDesignerHost designerHost) && currentSelection.Owner is ToolStripDropDown parentDropDown)
                {
                    bool needRefresh = false;
                    if (parentDropDown.OwnerItem is ToolStripDropDownItem parentItem)
                    {
                        ToolStripMenuItemDesigner parentItemDesigner = (ToolStripMenuItemDesigner)designerHost.GetDesigner(parentItem);
                        parentItemDesigner?.InitializeDropDown();

                        needRefresh = true;
                    }
                    else if (parentDropDown is ContextMenuStrip)
                    {
                        // For ContextMenuStrip, we need use different ways to show the menu.
                        ToolStripDropDownDesigner parentDropDownDesigner = (ToolStripDropDownDesigner)designerHost.GetDesigner(parentDropDown);
                        parentDropDownDesigner?.ShowMenu(currentSelection);

                        needRefresh = true;
                    }

                    if (needRefresh)
                    {
                        // Refresh SelectionManager to Change Glyph Size.
                        GetService<SelectionManager>().Refresh();

                        // Invalidate the dropdown area. This is necessary when a different item is
                        // selected in the same dropdown.
                        GetService<BehaviorService>().Invalidate(parentDropDown.Bounds);
                    }
                }
            }
            else if (currentSelection.Owner is not null)
            {
                // The selected item could be in a MenuStrip, StatusStrip or ToolStrip. Need invalidate the
                // BehaviorService to reflect the selection change.
                if (TryGetService(out BehaviorService behaviorService))
                {
                    behaviorService.Invalidate(behaviorService.ControlRectInAdornerWindow(currentSelection.Owner));
                }
            }
        }
    }

    /// <summary>
    ///  Allows a designer to filter the set of properties the component it is designing will expose through
    ///  the TypeDescriptor object. This method is called immediately before its corresponding "Post" method.
    ///  If you are overriding this method you should call the base implementation before
    ///  you perform your own filtering.
    /// </summary>
    protected override void PreFilterProperties(IDictionary properties)
    {
        base.PreFilterProperties(properties);
        // Handle shadowed properties
        string[] shadowProps = ["AutoSize", "AccessibleName", "Visible", "Overflow"];

        PropertyDescriptor prop;
        Attribute[] empty = [];
        for (int i = 0; i < shadowProps.Length; i++)
        {
            prop = (PropertyDescriptor)properties[shadowProps[i]];
            if (prop is not null)
            {
                properties[shadowProps[i]] = TypeDescriptor.CreateProperty(typeof(ToolStripItemDesigner), prop, empty);
            }
        }
    }

    // CALLED ONLY IF THE EDIT ACTION WAS ROLLED BACK!!!
    public void RemoveItem()
    {
        _dummyItemAdded = false;
        if (!TryGetService(out IDesignerHost host))
        {
            Debug.Fail("Couldn't get designer host!");
            return;
        }

        // Remove the dummy Item since the Edit was CANCELLED..
        ToolStrip parent = (ToolStrip)ImmediateParent;
        if (parent is ToolStripOverflow)
        {
            parent = ParentComponent as ToolStrip;
        }

        parent.Items.Remove(ToolStripItem);
        host.DestroyComponent(ToolStripItem);
    }

    /// <summary>
    /// Resets the ToolStripItemAutoSize to be the default autosize
    /// </summary>
    private void ResetAutoSize() => ShadowProperties[nameof(AutoSize)] = false;

    /// <summary>
    ///  Restores the AutoSize to be the value set in the property grid.
    /// </summary>
    private void RestoreAutoSize() => ToolStripItem.AutoSize = (bool)ShadowProperties[nameof(AutoSize)];

    /// <summary>
    /// Resets the ToolStrip Visible to be the default value
    /// </summary>
    private void ResetVisible() => Visible = true;

    /// <summary>
    ///  Restore Overflow
    /// </summary>
    private void RestoreOverflow() => ToolStripItem.Overflow = (ToolStripItemOverflow)ShadowProperties[nameof(Overflow)];

    /// <summary>
    ///  Resets Overflow
    /// </summary>
    private void ResetOverflow() => ToolStripItem.Overflow = ToolStripItemOverflow.AsNeeded;

    /// <summary>
    ///  Resets the ToolStripItem AccessibleName to the default
    /// </summary>
    private void ResetAccessibleName() => ShadowProperties[nameof(AccessibleName)] = null;

    /// <summary>
    ///  Restores the AutoSize to be the value set in the property grid.
    /// </summary>
    private void RestoreAccessibleName() => ToolStripItem.AccessibleName = (string)ShadowProperties[nameof(AccessibleName)];

    // internal method called to select the next item from the current item.
    internal void SelectNextItem(bool enterKeyPressed, ToolStripDesigner designer)
    {
        if (ToolStripItem is ToolStripDropDownItem)
        {
            SetSelection(enterKeyPressed);
            return;
        }

        // We are here for simple ToolStripItems.
        ToolStrip parent = (ToolStrip)ImmediateParent;
        if (parent is ToolStripOverflow)
        {
            parent = ToolStripItem.Owner;
        }

        int currentIndex = parent.Items.IndexOf(ToolStripItem);
        ToolStripItem nextItem = parent.Items[currentIndex + 1];

        // Set the selection to the next item in the toolstrip...
        if (TryGetService(out ToolStripKeyboardHandlingService keyboardHandlingService))
        {
            if (nextItem == designer.EditorNode)
            {
                keyboardHandlingService.SelectedDesignerControl = nextItem;
                _selectionService.SetSelectedComponents(null, SelectionTypes.Replace);
            }
            else
            {
                keyboardHandlingService.SelectedDesignerControl = null;
                _selectionService.SetSelectedComponents(new object[] { nextItem });
            }
        }
    }

    // Recursive function to add all the menuItems to the SerializationStore during Morphing..
    private static void SerializeDropDownItems(ToolStripDropDownItem parent, ref SerializationStore _serializedDataForDropDownItems, ComponentSerializationService _serializationService)
    {
        foreach (ToolStripItem item in parent.DropDownItems)
        {
            // Don't Serialize the DesignerToolStripControlHost...
            if (item is not DesignerToolStripControlHost)
            {
                _serializationService.Serialize(_serializedDataForDropDownItems, item);
                if (item is ToolStripDropDownItem dropDownItem)
                {
                    SerializeDropDownItems(dropDownItem, ref _serializedDataForDropDownItems, _serializationService);
                }
            }
        }
    }

    // Sets the Item visibility to honor WYSIWYG
    internal void SetItemVisible(bool toolStripSelected, ToolStripDesigner designer)
    {
        if (toolStripSelected)
        {
            // Set the Visiblity if different.
            if (!_currentVisible)
            {
                ToolStripItem.Visible = true;
                if (designer is not null && !designer.FireSyncSelection)
                {
                    designer.FireSyncSelection = true;
                }
            }
        }
        else
        {
            if (!_currentVisible)
            {
                ToolStripItem.Visible = _currentVisible;
            }
        }
    }

    private bool ShouldSerializeVisible() => !Visible;

    /// <summary>
    /// Since we're shadowing autosize, we get called here to determine whether or not to serialize
    /// </summary>
    private bool ShouldSerializeAutoSize() => (ShadowProperties.Contains(nameof(AutoSize)));

    /// <summary>
    /// Since we're shadowing autosize, we get called here to determine whether or not to serialize
    /// </summary>
    private bool ShouldSerializeAccessibleName() => (ShadowProperties[nameof(AccessibleName)] is not null);

    /// <summary>
    /// Since we're Overflow Size, we get called here to determine whether or not to serialize
    /// </summary>
    private bool ShouldSerializeOverflow() => (ShadowProperties[nameof(Overflow)] is not null);

    /// <summary>
    ///  This Function is called thru the ToolStripEditorManager which is listening for the F2 command.
    /// </summary>
    internal virtual void ShowEditNode(bool clicked)
    {
        // ACTIVATION ONLY FOR TOOLSTRIPMENUITEMS
        if (ToolStripItem is ToolStripMenuItem)
        {
            if (_editorNode is null)
            {
                CreateDummyNode();
            }

            IDesignerHost designerHost = (IDesignerHost)Component.Site.GetService(typeof(IDesignerHost));
            ToolStrip parent = ImmediateParent as ToolStrip;
            Debug.Assert(parent is not null, "ImmediateParent is null for the current ToolStripItem !!");
            if (parent is not null)
            {
                ToolStripDesigner parentDesigner = (ToolStripDesigner)designerHost.GetDesigner(parent);
                BehaviorService behaviorService = GetService<BehaviorService>();
                Point location = behaviorService.ControlToAdornerWindow(parent);

                // Get the original ToolStripItem bounds.
                Rectangle origBoundsInAdornerWindow = ToolStripItem.Bounds;
                origBoundsInAdornerWindow.Offset(location);
                ToolStripItem.AutoSize = false;
                _editorNode.SetWidth(ToolStripItem.Text);
                if (parent.Orientation == Orientation.Horizontal)
                {
                    ToolStripItem.Width = _editorNode.EditorToolStrip.Width + 2;
                }
                else
                {
                    ToolStripItem.Height = _editorNode.EditorToolStrip.Height;
                }

                // Refresh the glyphs.
                if (!_dummyItemAdded)
                {
                    behaviorService.SyncSelection();
                }

                if (ToolStripItem.Placement != ToolStripItemPlacement.None)
                {
                    Rectangle boundsInAdornerWindow = ToolStripItem.Bounds;
                    boundsInAdornerWindow.Offset(location);

                    // Center it in verticaldirection.
                    if (parent.Orientation == Orientation.Horizontal)
                    {
                        boundsInAdornerWindow.X++;
                        boundsInAdornerWindow.Y += (ToolStripItem.Height - _editorNode.EditorToolStrip.Height) / 2;
                        boundsInAdornerWindow.Y++;
                    }
                    else
                    {
                        boundsInAdornerWindow.X += (ToolStripItem.Width - _editorNode.EditorToolStrip.Width) / 2;
                        boundsInAdornerWindow.X++;
                    }

                    _editorNode.Bounds = boundsInAdornerWindow;

                    // Invalidate the union of the original bounds and the new bounds.
                    boundsInAdornerWindow = Rectangle.Union(origBoundsInAdornerWindow, boundsInAdornerWindow);
                    behaviorService.Invalidate(boundsInAdornerWindow);

                    // PLEASE DON'T CHANGE THIS ORDER !!!
                    if (parentDesigner is not null && parentDesigner.EditManager is not null)
                    {
                        parentDesigner.EditManager.ActivateEditor(ToolStripItem);
                    }

                    SelectionManager selectionManager = GetService<SelectionManager>();
                    if (_bodyGlyph is not null)
                    {
                        selectionManager.BodyGlyphAdorner.Glyphs.Remove(_bodyGlyph);
                    }
                }
                else
                {
                    ToolStripItem.AutoSize = AutoSize;
                    if (ToolStripItem is ToolStripDropDownItem) // We have no place to show this item... so Hide the DropDown
                    {
                        if (ToolStripItem is ToolStripDropDownItem ddItem)
                        {
                            ddItem.HideDropDown();
                        }

                        // And select the parent... since we cannot show the current selection.
                        _selectionService.SetSelectedComponents(new object[] { ImmediateParent });
                    }
                }
            }
        }
    }

    // This method is called by the ToolStripDesigner to SetSelections to proper ToolStripItems after
    // the parent ToolStripItem is committed. Consider this : the ToolStrip would cause the NEXT item on the
    // TOPLEVEL to get selected... while on MenuStrip.. we would want the Child ToolStripItem in the DropDown to get
    // selected after the TopLevel MenuStripItem is committed.
    internal virtual bool SetSelection(bool enterKeyPressed) => false;

    internal override void ShowContextMenu(int x, int y)
    {
        if (!TryGetService(out ToolStripKeyboardHandlingService keyboardService))
        {
            return;
        }

        if (!keyboardService.ContextMenuShownByKeyBoard)
        {
            Point newPoint = Point.Empty;
            if (TryGetService(out BehaviorService behaviorService))
            {
                newPoint = behaviorService.ScreenToAdornerWindow(new Point(x, y));
            }

            Rectangle itemBounds = GetGlyphBounds();
            if (itemBounds.Contains(newPoint))
            {
                DesignerContextMenu.Show(x, y);
            }
        }
        else
        {
            keyboardService.ContextMenuShownByKeyBoard = false;
            DesignerContextMenu.Show(x, y);
        }
    }
}
