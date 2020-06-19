// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  Custom ContextMenu section for ToolStripMenuItems.
    /// </summary>
    internal class ToolStripItemCustomMenuItemCollection : CustomMenuItemCollection
    {
        private readonly ToolStripItem currentItem;
        private readonly IServiceProvider serviceProvider;

        private ToolStripMenuItem imageToolStripMenuItem;
        private ToolStripMenuItem enabledToolStripMenuItem;

        private ToolStripMenuItem isLinkToolStripMenuItem;
        private ToolStripMenuItem springToolStripMenuItem;

        private ToolStripMenuItem checkedToolStripMenuItem;
        private ToolStripMenuItem showShortcutKeysToolStripMenuItem;

        private ToolStripMenuItem alignmentToolStripMenuItem;
        private ToolStripMenuItem displayStyleToolStripMenuItem;

        private ToolStripSeparator toolStripSeparator1;

        private ToolStripMenuItem convertToolStripMenuItem;
        private ToolStripMenuItem insertToolStripMenuItem;

        private ToolStripMenuItem leftToolStripMenuItem;
        private ToolStripMenuItem rightToolStripMenuItem;

        private ToolStripMenuItem noneStyleToolStripMenuItem;
        private ToolStripMenuItem textStyleToolStripMenuItem;
        private ToolStripMenuItem imageStyleToolStripMenuItem;
        private ToolStripMenuItem imageTextStyleToolStripMenuItem;

        private ToolStripMenuItem editItemsToolStripMenuItem;
        private CollectionEditVerbManager verbManager;

        public ToolStripItemCustomMenuItemCollection(IServiceProvider provider, Component currentItem) : base()
        {
            serviceProvider = provider;
            this.currentItem = currentItem as ToolStripItem;
            PopulateList();
        }

        /// <summary>
        ///  Parent ToolStrip.
        /// </summary>
        private ToolStrip ParentTool
        {
            get => currentItem.Owner;
        }

        /// <summary>
        ///  creates a item representing an item, respecting Browsable.
        /// </summary>
        private ToolStripMenuItem CreatePropertyBasedItem(string text, string propertyName, string imageName)
        {
            ToolStripMenuItem item = new ToolStripMenuItem(text);
            bool browsable = IsPropertyBrowsable(propertyName);
            item.Visible = browsable;
            if (browsable)
            {
                if (!string.IsNullOrEmpty(imageName))
                {
                    item.Image = new Icon(typeof(ToolStripMenuItem), imageName).ToBitmap();
                    item.ImageTransparentColor = Color.Magenta;
                }

                if (serviceProvider.GetService(typeof(IUIService)) is IUIService uis)
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
            ToolStripMenuItem item = new ToolStripMenuItem(name)
            {
                Tag = new EnumValueDescription(propertyName, value)
            };
            item.Click += new EventHandler(OnEnumValueChanged);
            return item;
        }

        private ToolStripMenuItem CreateBooleanItem(string text, string propertyName)
        {
            ToolStripMenuItem item = new ToolStripMenuItem(text);
            bool browsable = IsPropertyBrowsable(propertyName);
            item.Visible = browsable;
            item.Tag = propertyName;
            item.CheckOnClick = true;
            item.Click += new EventHandler(OnBooleanValueChanged);
            return item;
        }

        // Property names are hard-coded intentionally
        private void PopulateList()
        {
            ToolStripItem selectedItem = currentItem;
            if (!(selectedItem is ToolStripControlHost) && !(selectedItem is ToolStripSeparator))
            {
                imageToolStripMenuItem = new ToolStripMenuItem
                {
                    Text = SR.ToolStripItemContextMenuSetImage,
                    Image = new Icon(typeof(ToolStripMenuItem), "image").ToBitmap(),
                    ImageTransparentColor = Color.Magenta
                };
                //Add event Handlers
                imageToolStripMenuItem.Click += new EventHandler(OnImageToolStripMenuItemClick);
                enabledToolStripMenuItem = CreateBooleanItem("E&nabled", "Enabled");
                AddRange(new ToolStripItem[] { imageToolStripMenuItem, enabledToolStripMenuItem });
                if (selectedItem is ToolStripMenuItem)
                {
                    checkedToolStripMenuItem = CreateBooleanItem("C&hecked", "Checked");
                    showShortcutKeysToolStripMenuItem = CreateBooleanItem("ShowShortcut&Keys", "ShowShortcutKeys");
                    AddRange(new System.Windows.Forms.ToolStripItem[] { checkedToolStripMenuItem, showShortcutKeysToolStripMenuItem });
                }
                else
                {
                    if (selectedItem is ToolStripLabel)
                    {
                        isLinkToolStripMenuItem = CreateBooleanItem("IsLin&k", "IsLink");
                        Add(isLinkToolStripMenuItem);
                    }

                    if (selectedItem is ToolStripStatusLabel)
                    {
                        springToolStripMenuItem = CreateBooleanItem("Sprin&g", "Spring");
                        Add(springToolStripMenuItem);
                    }

                    leftToolStripMenuItem = CreateEnumValueItem("Alignment", "Left", ToolStripItemAlignment.Left);
                    rightToolStripMenuItem = CreateEnumValueItem("Alignment", "Right", ToolStripItemAlignment.Right);
                    noneStyleToolStripMenuItem = CreateEnumValueItem("DisplayStyle", "None", ToolStripItemDisplayStyle.None);
                    textStyleToolStripMenuItem = CreateEnumValueItem("DisplayStyle", "Text", ToolStripItemDisplayStyle.Text);
                    imageStyleToolStripMenuItem = CreateEnumValueItem("DisplayStyle", "Image", ToolStripItemDisplayStyle.Image);
                    imageTextStyleToolStripMenuItem = CreateEnumValueItem("DisplayStyle", "ImageAndText", ToolStripItemDisplayStyle.ImageAndText);
                    // alignmentToolStripMenuItem
                    alignmentToolStripMenuItem = CreatePropertyBasedItem("Ali&gnment", "Alignment", "alignment");
                    alignmentToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { leftToolStripMenuItem, rightToolStripMenuItem });
                    // displayStyleToolStripMenuItem
                    displayStyleToolStripMenuItem = CreatePropertyBasedItem("Displa&yStyle", "DisplayStyle", "displaystyle");
                    displayStyleToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { noneStyleToolStripMenuItem, textStyleToolStripMenuItem, imageStyleToolStripMenuItem, imageTextStyleToolStripMenuItem });

                    if (serviceProvider.GetService(typeof(IUIService)) is IUIService uis)
                    {
                        // We already have code which expects VsRenderer and DialogFont to be always available without the need for null checks
                        ToolStripProfessionalRenderer renderer = (ToolStripProfessionalRenderer)uis.Styles["VsRenderer"];
                        alignmentToolStripMenuItem.DropDown.Renderer = renderer;
                        displayStyleToolStripMenuItem.DropDown.Renderer = renderer;

                        Font font = (Font)uis.Styles["DialogFont"];
                        alignmentToolStripMenuItem.DropDown.Font = font;
                        displayStyleToolStripMenuItem.DropDown.Font = font;

                        // VsColorPanelText may be undefined, so we do need the check for Color here
                        object panelTextObject = uis.Styles["VsColorPanelText"];
                        if (panelTextObject is Color panelTextColor)
                        {
                            alignmentToolStripMenuItem.DropDown.ForeColor = panelTextColor;
                            displayStyleToolStripMenuItem.DropDown.ForeColor = panelTextColor;
                        }
                    }
                    AddRange(new System.Windows.Forms.ToolStripItem[] { alignmentToolStripMenuItem, displayStyleToolStripMenuItem, });
                }
                toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
                Add(toolStripSeparator1);
            }

            convertToolStripMenuItem = new ToolStripMenuItem
            {
                Text = SR.ToolStripItemContextMenuConvertTo,
                DropDown = ToolStripDesignerUtils.GetNewItemDropDown(ParentTool, currentItem, new EventHandler(AddNewItemClick), true, serviceProvider, true)
            };
            insertToolStripMenuItem = new ToolStripMenuItem
            {
                Text = SR.ToolStripItemContextMenuInsert,
                DropDown = ToolStripDesignerUtils.GetNewItemDropDown(ParentTool, currentItem, new EventHandler(AddNewItemClick), false, serviceProvider, true)
            };

            AddRange(new System.Windows.Forms.ToolStripItem[] { convertToolStripMenuItem, insertToolStripMenuItem });

            if (currentItem is ToolStripDropDownItem)
            {
                IDesignerHost _designerHost = (IDesignerHost)serviceProvider.GetService(typeof(IDesignerHost));
                if (_designerHost != null)
                {
                    if (_designerHost.GetDesigner(currentItem) is ToolStripItemDesigner itemDesigner)
                    {
                        verbManager = new CollectionEditVerbManager(SR.ToolStripDropDownItemCollectionEditorVerb, itemDesigner, TypeDescriptor.GetProperties(currentItem)["DropDownItems"], false);
                        editItemsToolStripMenuItem = new ToolStripMenuItem
                        {
                            Text = SR.ToolStripDropDownItemCollectionEditorVerb
                        };
                        editItemsToolStripMenuItem.Click += new EventHandler(OnEditItemsMenuItemClick);
                        editItemsToolStripMenuItem.Image = new Icon(typeof(ToolStripMenuItem), "editdropdownlist").ToBitmap();
                        editItemsToolStripMenuItem.ImageTransparentColor = Color.Magenta;
                        Add(editItemsToolStripMenuItem);
                    }
                }
            }
        }

        private void OnEditItemsMenuItemClick(object sender, EventArgs e)
        {
            if (verbManager != null)
            {
                verbManager.EditItemsVerb.Invoke();
            }
        }

        private void OnImageToolStripMenuItemClick(object sender, EventArgs e)
        {
            IDesignerHost _designerHost = (IDesignerHost)serviceProvider.GetService(typeof(IDesignerHost));
            if (_designerHost != null)
            {
                if (_designerHost.GetDesigner(currentItem) is ToolStripItemDesigner itemDesigner)
                {
                    try
                    {
                        // EditorServiceContext will check if the user has changed the property and set it for us.
                        EditorServiceContext.EditValue(itemDesigner, currentItem, "Image");
                    }
                    catch (InvalidOperationException ex)
                    {
                        IUIService uiService = (IUIService)serviceProvider.GetService(typeof(IUIService));
                        uiService.ShowError(ex.Message);
                    }
                }
            }
        }

        private void OnBooleanValueChanged(object sender, EventArgs e)
        {
            ToolStripItem item = sender as ToolStripItem;
            Debug.Assert(item != null, "Why is item null?");
            if (item != null)
            {
                string propertyName = item.Tag as string;
                Debug.Assert(propertyName != null, "Why is propertyName null?");
                if (propertyName != null)
                {
                    bool currentValue = (bool)GetProperty(propertyName);
                    ChangeProperty(propertyName, !currentValue);
                }
            }
        }

        private void OnEnumValueChanged(object sender, EventArgs e)
        {
            ToolStripItem item = sender as ToolStripItem;
            Debug.Assert(item != null, "Why is item null?");
            if (item != null)
            {
                EnumValueDescription desc = item.Tag as EnumValueDescription;
                Debug.Assert(desc != null, "Why is desc null?");
                if (desc != null && !string.IsNullOrEmpty(desc.PropertyName))
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
                //we are morphing the currentItem
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
            if (t != currentItem.GetType())
            {
                IDesignerHost _designerHost = (IDesignerHost)serviceProvider.GetService(typeof(IDesignerHost));
                ToolStripItemDesigner _designer = (ToolStripItemDesigner)_designerHost.GetDesigner(currentItem);
                _designer.MorphCurrentItem(t);
            }
        }

        private void InsertItem(Type t)
        {
            if (currentItem is ToolStripMenuItem)
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
                InsertIntoDropDown((ToolStripDropDown)currentItem.Owner, t);
            }
        }

        private void TryCancelTransaction(ref DesignerTransaction transaction)
        {
            if (transaction != null)
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
            IDesignerHost designerHost = (IDesignerHost)serviceProvider.GetService(typeof(IDesignerHost));
            Debug.Assert(designerHost != null, "Why didn't we get a designer host?");
            int dummyIndex = parent.Items.IndexOf(currentItem);
            if (parent != null)
            {
                if (parent.OwnerItem is ToolStripDropDownItem ownerItem)
                {
                    if (ownerItem.DropDownDirection == ToolStripDropDownDirection.AboveLeft || ownerItem.DropDownDirection == ToolStripDropDownDirection.AboveRight)
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
                if (designer is ComponentDesigner)
                {
                    ((ComponentDesigner)designer).InitializeNewComponent(null);
                }

                parent.Items.Insert(dummyIndex, (ToolStripItem)component);
                // set the selection to our new item.. since we destroyed Original component.. we have to ask SelectionServive from new Component
                ISelectionService selSvc = (ISelectionService)serviceProvider.GetService(typeof(ISelectionService));
                if (selSvc != null)
                {
                    selSvc.SetSelectedComponents(new object[] { component }, SelectionTypes.Replace);
                }
            }
            catch (Exception ex)
            {
                // We need to cancel the ToolStripDesigner's nested MenuItemTransaction; otherwise, we can't cancel our Transaction and the Designer will be left in an unusable state
                if ((parent != null) && (parent.OwnerItem != null) && (parent.OwnerItem.Owner != null))
                {
                    if (designerHost.GetDesigner(parent.OwnerItem.Owner) is ToolStripDesigner toolStripDesigner)
                    {
                        toolStripDesigner.CancelPendingMenuItemTransaction();
                    }
                }

                // Cancel our new Item transaction
                TryCancelTransaction(ref newItemTransaction);

                if (ClientUtils.IsCriticalException(ex))
                {
                    throw;
                }
            }
            finally
            {
                if (newItemTransaction != null)
                {
                    newItemTransaction.Commit();
                    newItemTransaction = null;
                }
            }
        }

        /// <summary>
        ///  Insert Item into Main MenuStrip.
        /// </summary>
        private void InsertIntoMainMenu(MenuStrip parent, Type t)
        {
            IDesignerHost designerHost = (IDesignerHost)serviceProvider.GetService(typeof(IDesignerHost));
            Debug.Assert(designerHost != null, "Why didn't we get a designer host?");
            int dummyIndex = parent.Items.IndexOf(currentItem);
            DesignerTransaction newItemTransaction = designerHost.CreateTransaction(SR.ToolStripAddingItem);
            try
            {
                // the code in ComponentAdded will actually get the add done.
                IComponent component = designerHost.CreateComponent(t);
                IDesigner designer = designerHost.GetDesigner(component);
                if (designer is ComponentDesigner)
                {
                    ((ComponentDesigner)designer).InitializeNewComponent(null);
                }
                Debug.Assert(dummyIndex != -1, "Why is item index negative?");
                parent.Items.Insert(dummyIndex, (ToolStripItem)component);
                // set the selection to our new item.. since we destroyed Original component.. we have to ask SelectionServive from new Component
                ISelectionService selSvc = (ISelectionService)serviceProvider.GetService(typeof(ISelectionService));
                if (selSvc != null)
                {
                    selSvc.SetSelectedComponents(new object[] { component }, SelectionTypes.Replace);
                }
            }
            catch (Exception ex)
            {
                TryCancelTransaction(ref newItemTransaction);
                if (ClientUtils.IsCriticalException(ex))
                {
                    throw;
                }
            }
            finally
            {
                if (newItemTransaction != null)
                {
                    newItemTransaction.Commit();
                    newItemTransaction = null;
                }
            }
        }

        /// <summary>
        ///  Insert Item into StatusStrip.
        /// </summary>
        private void InsertIntoStatusStrip(StatusStrip parent, Type t)
        {
            IDesignerHost designerHost = (IDesignerHost)serviceProvider.GetService(typeof(IDesignerHost));
            Debug.Assert(designerHost != null, "Why didn't we get a designer host?");
            int dummyIndex = parent.Items.IndexOf(currentItem);
            DesignerTransaction newItemTransaction = designerHost.CreateTransaction(SR.ToolStripAddingItem);
            try
            {
                // the code in ComponentAdded will actually get the add done.
                IComponent component = designerHost.CreateComponent(t);
                IDesigner designer = designerHost.GetDesigner(component);
                if (designer is ComponentDesigner)
                {
                    ((ComponentDesigner)designer).InitializeNewComponent(null);
                }
                Debug.Assert(dummyIndex != -1, "Why is item index negative?");
                parent.Items.Insert(dummyIndex, (ToolStripItem)component);
                // set the selection to our new item.. since we destroyed Original component.. we have to ask SelectionServive from new Component
                ISelectionService selSvc = (ISelectionService)serviceProvider.GetService(typeof(ISelectionService));
                if (selSvc != null)
                {
                    selSvc.SetSelectedComponents(new object[] { component }, SelectionTypes.Replace);
                }
            }
            catch (Exception ex)
            {
                TryCancelTransaction(ref newItemTransaction);
                if (ClientUtils.IsCriticalException(ex))
                {
                    throw;
                }
            }
            finally
            {
                if (newItemTransaction != null)
                {
                    newItemTransaction.Commit();
                    newItemTransaction = null;
                }
            }
        }

        /// <summary>
        ///  Insert Item into ToolStrip.
        /// </summary>
        private void InsertToolStripItem(Type t)
        {
            IDesignerHost designerHost = (IDesignerHost)serviceProvider.GetService(typeof(IDesignerHost));
            Debug.Assert(designerHost != null, "Why didn't we get a designer host?");
            ToolStrip parent = ParentTool;
            int dummyIndex = parent.Items.IndexOf(currentItem);
            DesignerTransaction newItemTransaction = designerHost.CreateTransaction(SR.ToolStripAddingItem);
            try
            {
                // the code in ComponentAdded will actually get the add done.
                IComponent component = designerHost.CreateComponent(t);
                IDesigner designer = designerHost.GetDesigner(component);
                if (designer is ComponentDesigner)
                {
                    ((ComponentDesigner)designer).InitializeNewComponent(null);
                }
                //Set the Image property and DisplayStyle...
                if (component is ToolStripButton || component is ToolStripSplitButton || component is ToolStripDropDownButton)
                {
                    Image image = null;
                    try
                    {
                        image = new Icon(typeof(ToolStripButton), "blank").ToBitmap();
                    }
                    catch (Exception ex)
                    {
                        if (ClientUtils.IsCriticalException(ex))
                        {
                            throw;
                        }
                    }
                    ChangeProperty(component, "Image", image);
                    ChangeProperty(component, "DisplayStyle", ToolStripItemDisplayStyle.Image);
                    ChangeProperty(component, "ImageTransparentColor", Color.Magenta);
                }

                Debug.Assert(dummyIndex != -1, "Why is item index negative?");
                parent.Items.Insert(dummyIndex, (ToolStripItem)component);
                // set the selection to our new item.. since we destroyed Original component.. we have to ask SelectionServive from new Component
                ISelectionService selSvc = (ISelectionService)serviceProvider.GetService(typeof(ISelectionService));
                if (selSvc != null)
                {
                    selSvc.SetSelectedComponents(new object[] { component }, SelectionTypes.Replace);
                }
            }
            catch (Exception ex)
            {
                if (newItemTransaction != null)
                {
                    newItemTransaction.Cancel();
                    newItemTransaction = null;
                }
                if (ClientUtils.IsCriticalException(ex))
                {
                    throw;
                }
            }

            finally
            {
                if (newItemTransaction != null)
                {
                    newItemTransaction.Commit();
                    newItemTransaction = null;
                }
            }
        }

        private bool IsPropertyBrowsable(string propertyName)
        {
            PropertyDescriptor getProperty = TypeDescriptor.GetProperties(currentItem)[propertyName];
            Debug.Assert(getProperty != null, "Could not find given property in control.");
            if (getProperty != null)
            {
                if (getProperty.Attributes[typeof(BrowsableAttribute)] is BrowsableAttribute attribute)
                {
                    return attribute.Browsable;
                }
            }
            return true;
        }

        //helper function to get the property on the actual Control
        private object GetProperty(string propertyName)
        {
            PropertyDescriptor getProperty = TypeDescriptor.GetProperties(currentItem)[propertyName];
            Debug.Assert(getProperty != null, "Could not find given property in control.");
            if (getProperty != null)
            {
                return getProperty.GetValue(currentItem);
            }
            return null;
        }

        //helper function to change the property on the actual Control
        protected void ChangeProperty(string propertyName, object value)
        {
            ChangeProperty(currentItem, propertyName, value);
        }

        protected void ChangeProperty(IComponent target, string propertyName, object value)
        {
            PropertyDescriptor changingProperty = TypeDescriptor.GetProperties(target)[propertyName];
            Debug.Assert(changingProperty != null, "Could not find given property in control.");
            try
            {
                if (changingProperty != null)
                {
                    changingProperty.SetValue(target, value);
                }
            }
            catch (InvalidOperationException ex)
            {
                IUIService uiService = (IUIService)serviceProvider.GetService(typeof(IUIService));
                uiService.ShowError(ex.Message);
            }
        }

        private void RefreshAlignment()
        {
            ToolStripItemAlignment currentAlignmentValue = (ToolStripItemAlignment)GetProperty("Alignment");
            leftToolStripMenuItem.Checked = (currentAlignmentValue == ToolStripItemAlignment.Left) ? true : false;
            rightToolStripMenuItem.Checked = (currentAlignmentValue == ToolStripItemAlignment.Right) ? true : false;
        }

        private void RefreshDisplayStyle()
        {
            ToolStripItemDisplayStyle currentDisplayStyleValue = (ToolStripItemDisplayStyle)GetProperty("DisplayStyle");
            noneStyleToolStripMenuItem.Checked = (currentDisplayStyleValue == ToolStripItemDisplayStyle.None) ? true : false;
            textStyleToolStripMenuItem.Checked = (currentDisplayStyleValue == ToolStripItemDisplayStyle.Text) ? true : false;
            imageStyleToolStripMenuItem.Checked = (currentDisplayStyleValue == ToolStripItemDisplayStyle.Image) ? true : false;
            imageTextStyleToolStripMenuItem.Checked = (currentDisplayStyleValue == ToolStripItemDisplayStyle.ImageAndText) ? true : false;
        }

        public override void RefreshItems()
        {
            base.RefreshItems();
            ToolStripItem selectedItem = currentItem;
            if (!(selectedItem is ToolStripControlHost) && !(selectedItem is ToolStripSeparator))
            {
                enabledToolStripMenuItem.Checked = (bool)GetProperty("Enabled");
                if (selectedItem is ToolStripMenuItem)
                {
                    checkedToolStripMenuItem.Checked = (bool)GetProperty("Checked");
                    showShortcutKeysToolStripMenuItem.Checked = (bool)GetProperty("ShowShortcutKeys");
                }
                else
                {
                    if (selectedItem is ToolStripLabel)
                    {
                        isLinkToolStripMenuItem.Checked = (bool)GetProperty("IsLink");
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
}
