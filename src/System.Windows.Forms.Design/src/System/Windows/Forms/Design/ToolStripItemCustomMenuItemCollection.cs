// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;

namespace System.Windows.Forms.Design;

/// <summary>
///  Custom ContextMenu section for ToolStripMenuItems.
/// </summary>
internal class ToolStripItemCustomMenuItemCollection : CustomMenuItemCollection
{
    private readonly ToolStripItem _currentItem;
    private readonly IServiceProvider _serviceProvider;

    private ToolStripMenuItem _imageToolStripMenuItem;
    private ToolStripMenuItem _enabledToolStripMenuItem;

    private ToolStripMenuItem _isLinkToolStripMenuItem;
    private ToolStripMenuItem _springToolStripMenuItem;

    private ToolStripMenuItem _checkedToolStripMenuItem;
    private ToolStripMenuItem _showShortcutKeysToolStripMenuItem;

    private ToolStripMenuItem _alignmentToolStripMenuItem;
    private ToolStripMenuItem _displayStyleToolStripMenuItem;

    private ToolStripSeparator _toolStripSeparator1;

    private ToolStripMenuItem _convertToolStripMenuItem;
    private ToolStripMenuItem _insertToolStripMenuItem;

    private ToolStripMenuItem _leftToolStripMenuItem;
    private ToolStripMenuItem _rightToolStripMenuItem;

    private ToolStripMenuItem _noneStyleToolStripMenuItem;
    private ToolStripMenuItem _textStyleToolStripMenuItem;
    private ToolStripMenuItem _imageStyleToolStripMenuItem;
    private ToolStripMenuItem _imageTextStyleToolStripMenuItem;

    private ToolStripMenuItem _editItemsToolStripMenuItem;
    private CollectionEditVerbManager _verbManager;

    public ToolStripItemCustomMenuItemCollection(IServiceProvider provider, Component currentItem) : base()
    {
        _serviceProvider = provider;
        _currentItem = currentItem as ToolStripItem;
        PopulateList();
    }

    /// <summary>
    ///  Parent ToolStrip.
    /// </summary>
    private ToolStrip ParentTool
    {
        get => _currentItem.Owner;
    }

    /// <summary>
    ///  creates a item representing an item, respecting Browsable.
    /// </summary>
    private ToolStripMenuItem CreatePropertyBasedItem(string text, string propertyName, string imageName)
    {
        ToolStripMenuItem item = new(text);
        bool browsable = IsPropertyBrowsable(propertyName);
        item.Visible = browsable;
        if (browsable)
        {
            if (!string.IsNullOrEmpty(imageName))
            {
                item.Image = new Icon(typeof(ToolStripMenuItem), imageName).ToBitmap();
                item.ImageTransparentColor = Color.Magenta;
            }

            if (_serviceProvider.GetService(typeof(IUIService)) is IUIService uis)
            {
                item.DropDown.Renderer = (ToolStripProfessionalRenderer)uis.Styles["VsRenderer"];
                item.DropDown.Font = (Font)uis.Styles["DialogFont"];
            }
        }

        return item;
    }

    /// <summary>
    ///  creates an item that when clicked changes the enum value.
    /// </summary>
    private ToolStripMenuItem CreateEnumValueItem(string propertyName, string name, object value)
    {
        ToolStripMenuItem item = new(name)
        {
            Tag = new EnumValueDescription(propertyName, value)
        };

        item.Click += OnEnumValueChanged;
        return item;
    }

    private ToolStripMenuItem CreateBooleanItem(string text, string propertyName)
    {
        ToolStripMenuItem item = new(text);
        bool browsable = IsPropertyBrowsable(propertyName);
        item.Visible = browsable;
        item.Tag = propertyName;
        item.CheckOnClick = true;
        item.Click += OnBooleanValueChanged;
        return item;
    }

    // Property names are hard-coded intentionally
    private void PopulateList()
    {
        ToolStripItem selectedItem = _currentItem;
        if (selectedItem is not ToolStripControlHost and not ToolStripSeparator)
        {
            _imageToolStripMenuItem = new ToolStripMenuItem
            {
                Text = SR.ToolStripItemContextMenuSetImage,
                Image = new Icon(typeof(ToolStripMenuItem), "image").ToBitmap(),
                ImageTransparentColor = Color.Magenta
            };

            // Add event Handlers
            _imageToolStripMenuItem.Click += OnImageToolStripMenuItemClick;
            _enabledToolStripMenuItem = CreateBooleanItem("E&nabled", "Enabled");
            AddRange([_imageToolStripMenuItem, _enabledToolStripMenuItem]);

            if (selectedItem is ToolStripMenuItem)
            {
                _checkedToolStripMenuItem = CreateBooleanItem("C&hecked", "Checked");
                _showShortcutKeysToolStripMenuItem = CreateBooleanItem("ShowShortcut&Keys", "ShowShortcutKeys");
                AddRange([_checkedToolStripMenuItem, _showShortcutKeysToolStripMenuItem]);
            }
            else
            {
                if (selectedItem is ToolStripLabel)
                {
                    _isLinkToolStripMenuItem = CreateBooleanItem("IsLin&k", "IsLink");
                    Add(_isLinkToolStripMenuItem);
                }

                if (selectedItem is ToolStripStatusLabel)
                {
                    _springToolStripMenuItem = CreateBooleanItem("Sprin&g", "Spring");
                    Add(_springToolStripMenuItem);
                }

                _leftToolStripMenuItem = CreateEnumValueItem("Alignment", "Left", ToolStripItemAlignment.Left);
                _rightToolStripMenuItem = CreateEnumValueItem("Alignment", "Right", ToolStripItemAlignment.Right);
                _noneStyleToolStripMenuItem = CreateEnumValueItem("DisplayStyle", "None", ToolStripItemDisplayStyle.None);
                _textStyleToolStripMenuItem = CreateEnumValueItem("DisplayStyle", "Text", ToolStripItemDisplayStyle.Text);
                _imageStyleToolStripMenuItem = CreateEnumValueItem("DisplayStyle", "Image", ToolStripItemDisplayStyle.Image);
                _imageTextStyleToolStripMenuItem = CreateEnumValueItem("DisplayStyle", "ImageAndText", ToolStripItemDisplayStyle.ImageAndText);
                // alignmentToolStripMenuItem
                _alignmentToolStripMenuItem = CreatePropertyBasedItem("Ali&gnment", "Alignment", "alignment");
                _alignmentToolStripMenuItem.DropDownItems.AddRange((ToolStripItem[])[_leftToolStripMenuItem, _rightToolStripMenuItem]);
                // displayStyleToolStripMenuItem
                _displayStyleToolStripMenuItem = CreatePropertyBasedItem("Displa&yStyle", "DisplayStyle", "displaystyle");
                _displayStyleToolStripMenuItem.DropDownItems.AddRange((ToolStripItem[])[_noneStyleToolStripMenuItem, _textStyleToolStripMenuItem, _imageStyleToolStripMenuItem, _imageTextStyleToolStripMenuItem]);

                if (_serviceProvider.GetService(typeof(IUIService)) is IUIService uis)
                {
                    // We already have code which expects VsRenderer and DialogFont to be always available without the need for null checks
                    ToolStripProfessionalRenderer renderer = (ToolStripProfessionalRenderer)uis.Styles["VsRenderer"];
                    _alignmentToolStripMenuItem.DropDown.Renderer = renderer;
                    _displayStyleToolStripMenuItem.DropDown.Renderer = renderer;

                    Font font = (Font)uis.Styles["DialogFont"];
                    _alignmentToolStripMenuItem.DropDown.Font = font;
                    _displayStyleToolStripMenuItem.DropDown.Font = font;

                    // VsColorPanelText may be undefined, so we do need the check for Color here
                    object panelTextObject = uis.Styles["VsColorPanelText"];
                    if (panelTextObject is Color panelTextColor)
                    {
                        _alignmentToolStripMenuItem.DropDown.ForeColor = panelTextColor;
                        _displayStyleToolStripMenuItem.DropDown.ForeColor = panelTextColor;
                    }
                }

                AddRange([_alignmentToolStripMenuItem, _displayStyleToolStripMenuItem,]);
            }

            _toolStripSeparator1 = new ToolStripSeparator();
            Add(_toolStripSeparator1);
        }

        _convertToolStripMenuItem = new ToolStripMenuItem
        {
            Text = SR.ToolStripItemContextMenuConvertTo,
            DropDown = ToolStripDesignerUtils.GetNewItemDropDown(ParentTool, _currentItem, new EventHandler(AddNewItemClick), true, _serviceProvider, true)
        };
        _insertToolStripMenuItem = new ToolStripMenuItem
        {
            Text = SR.ToolStripItemContextMenuInsert,
            DropDown = ToolStripDesignerUtils.GetNewItemDropDown(ParentTool, _currentItem, new EventHandler(AddNewItemClick), false, _serviceProvider, true)
        };

        AddRange([_convertToolStripMenuItem, _insertToolStripMenuItem]);

        if (_currentItem is ToolStripDropDownItem)
        {
            IDesignerHost _designerHost = (IDesignerHost)_serviceProvider.GetService(typeof(IDesignerHost));
            if (_designerHost is not null)
            {
                if (_designerHost.GetDesigner(_currentItem) is ToolStripItemDesigner itemDesigner)
                {
                    _verbManager = new CollectionEditVerbManager(SR.ToolStripDropDownItemCollectionEditorVerb, itemDesigner, TypeDescriptor.GetProperties(_currentItem)["DropDownItems"], false);
                    _editItemsToolStripMenuItem = new ToolStripMenuItem
                    {
                        Text = SR.ToolStripDropDownItemCollectionEditorVerb
                    };
                    _editItemsToolStripMenuItem.Click += OnEditItemsMenuItemClick;
                    _editItemsToolStripMenuItem.Image = new Icon(typeof(ToolStripMenuItem), "editdropdownlist").ToBitmap();
                    _editItemsToolStripMenuItem.ImageTransparentColor = Color.Magenta;
                    Add(_editItemsToolStripMenuItem);
                }
            }
        }
    }

    private void OnEditItemsMenuItemClick(object sender, EventArgs e)
    {
        _verbManager?.EditItemsVerb.Invoke();
    }

    private void OnImageToolStripMenuItemClick(object sender, EventArgs e)
    {
        IDesignerHost _designerHost = (IDesignerHost)_serviceProvider.GetService(typeof(IDesignerHost));
        if (_designerHost is not null)
        {
            if (_designerHost.GetDesigner(_currentItem) is ToolStripItemDesigner itemDesigner)
            {
                try
                {
                    // EditorServiceContext will check if the user has changed the property and set it for us.
                    EditorServiceContext.EditValue(itemDesigner, _currentItem, "Image");
                }
                catch (InvalidOperationException ex)
                {
                    IUIService uiService = (IUIService)_serviceProvider.GetService(typeof(IUIService));
                    uiService.ShowError(ex.Message);
                }
            }
        }
    }

    private void OnBooleanValueChanged(object sender, EventArgs e)
    {
        ToolStripItem item = sender as ToolStripItem;
        Debug.Assert(item is not null, "Why is item null?");
        if (item is not null)
        {
            string propertyName = item.Tag as string;
            Debug.Assert(propertyName is not null, "Why is propertyName null?");
            if (propertyName is not null)
            {
                bool currentValue = (bool)GetProperty(propertyName);
                ChangeProperty(propertyName, !currentValue);
            }
        }
    }

    private void OnEnumValueChanged(object sender, EventArgs e)
    {
        ToolStripItem item = sender as ToolStripItem;
        Debug.Assert(item is not null, "Why is item null?");
        if (item is not null)
        {
            EnumValueDescription desc = item.Tag as EnumValueDescription;
            Debug.Assert(desc is not null, "Why is desc null?");
            if (desc is not null && !string.IsNullOrEmpty(desc.PropertyName))
            {
                ChangeProperty(desc.PropertyName, desc.Value);
            }
        }
    }

    private void AddNewItemClick(object sender, EventArgs e)
    {
        ItemTypeToolStripMenuItem senderItem = (ItemTypeToolStripMenuItem)sender;
        Type t = senderItem.ItemType;
        if (senderItem.ConvertTo)
        {
            // we are morphing the currentItem
            MorphToolStripItem(t);
        }
        else
        {
            // we are inserting a new item..
            InsertItem(t);
        }
    }

    private void MorphToolStripItem(Type t)
    {
        // Go thru morphing routine only if we have different type.
        if (t != _currentItem.GetType())
        {
            IDesignerHost _designerHost = (IDesignerHost)_serviceProvider.GetService(typeof(IDesignerHost));
            ToolStripItemDesigner _designer = (ToolStripItemDesigner)_designerHost.GetDesigner(_currentItem);
            _designer.MorphCurrentItem(t);
        }
    }

    private void InsertItem(Type t)
    {
        if (_currentItem is ToolStripMenuItem)
        {
            InsertMenuItem(t);
        }
        else
        {
            InsertStripItem(t);
        }
    }

    /// <summary>
    ///  Insert MenuItem into ToolStrip.
    /// </summary>
    private void InsertStripItem(Type t)
    {
        if (ParentTool is StatusStrip parent)
        {
            InsertIntoStatusStrip(parent, t);
        }
        else
        {
            InsertToolStripItem(t);
        }
    }

    /// <summary>
    ///  Insert MenuItem into ToolStrip.
    /// </summary>
    private void InsertMenuItem(Type t)
    {
        if (ParentTool is MenuStrip parent)
        {
            InsertIntoMainMenu(parent, t);
        }
        else
        {
            InsertIntoDropDown((ToolStripDropDown)_currentItem.Owner, t);
        }
    }

    private static void TryCancelTransaction(ref DesignerTransaction transaction)
    {
        if (transaction is not null)
        {
            try
            {
                transaction.Cancel();
                transaction = null;
            }
            catch
            {
            }
        }
    }

    /// <summary>
    ///  Insert Item into DropDownMenu.
    /// </summary>
    private void InsertIntoDropDown(ToolStripDropDown parent, Type t)
    {
        IDesignerHost designerHost = (IDesignerHost)_serviceProvider.GetService(typeof(IDesignerHost));
        Debug.Assert(designerHost is not null, "Why didn't we get a designer host?");
        int dummyIndex = parent.Items.IndexOf(_currentItem);
        if (parent is not null)
        {
            if (parent.OwnerItem is ToolStripDropDownItem ownerItem)
            {
                if (ownerItem.DropDownDirection is ToolStripDropDownDirection.AboveLeft or ToolStripDropDownDirection.AboveRight)
                {
                    dummyIndex++;
                }
            }
        }

        DesignerTransaction newItemTransaction = designerHost.CreateTransaction(SR.ToolStripAddingItem);
        try
        {
            // the code in ComponentAdded will actually get the add done.
            IComponent component = designerHost.CreateComponent(t);
            IDesigner designer = designerHost.GetDesigner(component);
            if (designer is ComponentDesigner componentDesigner)
            {
                componentDesigner.InitializeNewComponent(null);
            }

            parent.Items.Insert(dummyIndex, (ToolStripItem)component);
            // set the selection to our new item.. since we destroyed Original component..
            // we have to ask SelectionService from new Component
            ISelectionService selSvc = (ISelectionService)_serviceProvider.GetService(typeof(ISelectionService));
            selSvc?.SetSelectedComponents(new object[] { component }, SelectionTypes.Replace);
        }
        catch (Exception ex)
        {
            // We need to cancel the ToolStripDesigner's nested MenuItemTransaction; otherwise,
            // we can't cancel our Transaction and the Designer will be left in an unusable state
            if ((parent is not null) && (parent.OwnerItem is not null) && (parent.OwnerItem.Owner is not null))
            {
                if (designerHost.GetDesigner(parent.OwnerItem.Owner) is ToolStripDesigner toolStripDesigner)
                {
                    toolStripDesigner.CancelPendingMenuItemTransaction();
                }
            }

            // Cancel our new Item transaction
            TryCancelTransaction(ref newItemTransaction);

            if (ex.IsCriticalException())
            {
                throw;
            }
        }
        finally
        {
            newItemTransaction?.Commit();
        }
    }

    /// <summary>
    ///  Insert Item into Main MenuStrip.
    /// </summary>
    private void InsertIntoMainMenu(MenuStrip parent, Type t)
    {
        IDesignerHost designerHost = (IDesignerHost)_serviceProvider.GetService(typeof(IDesignerHost));
        Debug.Assert(designerHost is not null, "Why didn't we get a designer host?");
        int dummyIndex = parent.Items.IndexOf(_currentItem);
        DesignerTransaction newItemTransaction = designerHost.CreateTransaction(SR.ToolStripAddingItem);
        try
        {
            // the code in ComponentAdded will actually get the add done.
            IComponent component = designerHost.CreateComponent(t);
            IDesigner designer = designerHost.GetDesigner(component);
            if (designer is ComponentDesigner componentDesigner)
            {
                componentDesigner.InitializeNewComponent(null);
            }

            Debug.Assert(dummyIndex != -1, "Why is item index negative?");
            parent.Items.Insert(dummyIndex, (ToolStripItem)component);
            // set the selection to our new item.. since we destroyed Original component.. we have to ask SelectionService from new Component
            ISelectionService selSvc = (ISelectionService)_serviceProvider.GetService(typeof(ISelectionService));
            selSvc?.SetSelectedComponents(new object[] { component }, SelectionTypes.Replace);
        }
        catch (Exception ex)
        {
            TryCancelTransaction(ref newItemTransaction);
            if (ex.IsCriticalException())
            {
                throw;
            }
        }
        finally
        {
            newItemTransaction?.Commit();
        }
    }

    /// <summary>
    ///  Insert Item into StatusStrip.
    /// </summary>
    private void InsertIntoStatusStrip(StatusStrip parent, Type t)
    {
        IDesignerHost designerHost = (IDesignerHost)_serviceProvider.GetService(typeof(IDesignerHost));
        Debug.Assert(designerHost is not null, "Why didn't we get a designer host?");
        int dummyIndex = parent.Items.IndexOf(_currentItem);
        DesignerTransaction newItemTransaction = designerHost.CreateTransaction(SR.ToolStripAddingItem);
        try
        {
            // the code in ComponentAdded will actually get the add done.
            IComponent component = designerHost.CreateComponent(t);
            IDesigner designer = designerHost.GetDesigner(component);
            if (designer is ComponentDesigner componentDesigner)
            {
                componentDesigner.InitializeNewComponent(null);
            }

            Debug.Assert(dummyIndex != -1, "Why is item index negative?");
            parent.Items.Insert(dummyIndex, (ToolStripItem)component);
            // set the selection to our new item.. since we destroyed Original component.. we have to ask SelectionService from new Component
            ISelectionService selSvc = (ISelectionService)_serviceProvider.GetService(typeof(ISelectionService));
            selSvc?.SetSelectedComponents(new object[] { component }, SelectionTypes.Replace);
        }
        catch (Exception ex)
        {
            TryCancelTransaction(ref newItemTransaction);
            if (ex.IsCriticalException())
            {
                throw;
            }
        }
        finally
        {
            newItemTransaction?.Commit();
        }
    }

    /// <summary>
    ///  Insert Item into ToolStrip.
    /// </summary>
    private void InsertToolStripItem(Type t)
    {
        IDesignerHost designerHost = (IDesignerHost)_serviceProvider.GetService(typeof(IDesignerHost));
        Debug.Assert(designerHost is not null, "Why didn't we get a designer host?");
        ToolStrip parent = ParentTool;
        int dummyIndex = parent.Items.IndexOf(_currentItem);
        DesignerTransaction newItemTransaction = designerHost.CreateTransaction(SR.ToolStripAddingItem);
        try
        {
            // the code in ComponentAdded will actually get the add done.
            IComponent component = designerHost.CreateComponent(t);
            IDesigner designer = designerHost.GetDesigner(component);
            if (designer is ComponentDesigner componentDesigner)
            {
                componentDesigner.InitializeNewComponent(null);
            }

            // Set the Image property and DisplayStyle...
            if (component is ToolStripButton or ToolStripSplitButton or ToolStripDropDownButton)
            {
                Image image = null;
                try
                {
                    image = new Icon(typeof(ToolStripButton), "blank").ToBitmap();
                }
                catch (Exception ex) when (!ex.IsCriticalException())
                {
                }

                ChangeProperty(component, "Image", image);
                ChangeProperty(component, "DisplayStyle", ToolStripItemDisplayStyle.Image);
                ChangeProperty(component, "ImageTransparentColor", Color.Magenta);
            }

            Debug.Assert(dummyIndex != -1, "Why is item index negative?");
            parent.Items.Insert(dummyIndex, (ToolStripItem)component);
            // set the selection to our new item.. since we destroyed Original component.. we have to ask SelectionService from new Component
            ISelectionService selSvc = (ISelectionService)_serviceProvider.GetService(typeof(ISelectionService));
            selSvc?.SetSelectedComponents(new object[] { component }, SelectionTypes.Replace);
        }
        catch (Exception ex)
        {
            if (newItemTransaction is not null)
            {
                newItemTransaction.Cancel();
                newItemTransaction = null;
            }

            if (ex.IsCriticalException())
            {
                throw;
            }
        }
        finally
        {
            newItemTransaction?.Commit();
        }
    }

    private bool IsPropertyBrowsable(string propertyName)
    {
        PropertyDescriptor getProperty = TypeDescriptor.GetProperties(_currentItem)[propertyName];
        Debug.Assert(getProperty is not null, "Could not find given property in control.");
        if (getProperty is not null)
        {
            if (getProperty.Attributes[typeof(BrowsableAttribute)] is BrowsableAttribute attribute)
            {
                return attribute.Browsable;
            }
        }

        return true;
    }

    // helper function to get the property on the actual Control
    private object GetProperty(string propertyName)
    {
        PropertyDescriptor getProperty = TypeDescriptor.GetProperties(_currentItem)[propertyName];
        Debug.Assert(getProperty is not null, "Could not find given property in control.");
        if (getProperty is not null)
        {
            return getProperty.GetValue(_currentItem);
        }

        return null;
    }

    // helper function to change the property on the actual Control
    protected void ChangeProperty(string propertyName, object value)
    {
        ChangeProperty(_currentItem, propertyName, value);
    }

    protected void ChangeProperty(IComponent target, string propertyName, object value)
    {
        PropertyDescriptor changingProperty = TypeDescriptor.GetProperties(target)[propertyName];
        Debug.Assert(changingProperty is not null, "Could not find given property in control.");
        try
        {
            changingProperty?.SetValue(target, value);
        }
        catch (InvalidOperationException ex)
        {
            IUIService uiService = (IUIService)_serviceProvider.GetService(typeof(IUIService));
            uiService.ShowError(ex.Message);
        }
    }

    private void RefreshAlignment()
    {
        ToolStripItemAlignment currentAlignmentValue = (ToolStripItemAlignment)GetProperty("Alignment");
        _leftToolStripMenuItem.Checked = currentAlignmentValue == ToolStripItemAlignment.Left;
        _rightToolStripMenuItem.Checked = currentAlignmentValue == ToolStripItemAlignment.Right;
    }

    private void RefreshDisplayStyle()
    {
        ToolStripItemDisplayStyle currentDisplayStyleValue = (ToolStripItemDisplayStyle)GetProperty("DisplayStyle");
        _noneStyleToolStripMenuItem.Checked = currentDisplayStyleValue == ToolStripItemDisplayStyle.None;
        _textStyleToolStripMenuItem.Checked = currentDisplayStyleValue == ToolStripItemDisplayStyle.Text;
        _imageStyleToolStripMenuItem.Checked = currentDisplayStyleValue == ToolStripItemDisplayStyle.Image;
        _imageTextStyleToolStripMenuItem.Checked = currentDisplayStyleValue == ToolStripItemDisplayStyle.ImageAndText;
    }

    public override void RefreshItems()
    {
        base.RefreshItems();
        ToolStripItem selectedItem = _currentItem;
        if (selectedItem is not ToolStripControlHost and not ToolStripSeparator)
        {
            _enabledToolStripMenuItem.Checked = (bool)GetProperty("Enabled");
            if (selectedItem is ToolStripMenuItem)
            {
                _checkedToolStripMenuItem.Checked = (bool)GetProperty("Checked");
                _showShortcutKeysToolStripMenuItem.Checked = (bool)GetProperty("ShowShortcutKeys");
            }
            else
            {
                if (selectedItem is ToolStripLabel)
                {
                    _isLinkToolStripMenuItem.Checked = (bool)GetProperty("IsLink");
                }

                RefreshAlignment();
                RefreshDisplayStyle();
            }
        }
    }

    // tiny little class to handle enum value changes
    private class EnumValueDescription
    {
        public EnumValueDescription(string propertyName, object value)
        {
            PropertyName = propertyName;
            Value = value;
        }

        public string PropertyName;
        public object Value;
    }
}
