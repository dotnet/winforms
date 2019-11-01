// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace System.Windows.Forms.Design
{
    /// <summary>
    /// Main class for collection editor for ToolStripItemCollections. Allows a single level of ToolStripItem children to be designed.
    /// </summary>
    internal class ToolStripCollectionEditor : CollectionEditor
    {
        /// <summary>
        /// Default contstructor.
        /// </summary>
        public ToolStripCollectionEditor() : base(typeof(ToolStripItemCollection))
        {
        }

        /// <summary>
        /// Overridden to reation our editor form instead of the standard collection editor form.
        /// </summary>
        /// <returns>An instance of a ToolStripItemEditorForm</returns>
        protected override CollectionForm CreateCollectionForm()
        {
            return new ToolStripItemEditorForm(this);
        }

        /// <summary>
        /// Gets the help topic to display for the dialog help button or pressing F1.
        /// Override to display a different help topic.
        /// </summary>
        protected override string HelpTopic => "net.ComponentModel.ToolStripCollectionEditor";

        /// <summary>
        /// Edits the value.
        /// </summary>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            // Get ahold of the designer for the component that is launching this editor.
            // If it is a winbar, then we want to let it know this editor is up.
            ToolStripDesigner designer = null;

            // See if the selected component is a winbar or winbar item that is directly on the form.
            if (provider != null)
            {
                ISelectionService selectionService = (ISelectionService)provider.GetService(typeof(ISelectionService));
                if (selectionService != null)
                {
                    object primarySelection = selectionService.PrimarySelection;

                    // If it's a drop down item, just pop up to it's owner.
                    if (primarySelection is ToolStripDropDownItem)
                    {
                        primarySelection = ((ToolStripDropDownItem)primarySelection).Owner;
                    }

                    // Now get the designer.
                    if (primarySelection is ToolStrip)
                    {
                        IDesignerHost host = (IDesignerHost)provider.GetService(typeof(IDesignerHost));
                        if (host != null)
                        {
                            designer = host.GetDesigner((IComponent)primarySelection) as ToolStripDesigner;
                        }
                    }
                }
            }

            try
            {
                if (designer != null)
                {
                    designer.EditingCollection = true;
                }

                return base.EditValue(context, provider, value);
            }
            finally
            {
                if (designer != null)
                {
                    designer.EditingCollection = false;
                }
            }
        }

        /// <summary>
        /// Our internal form UI for the ToolStripItemCollectionEditor.
        /// </summary>
        protected class ToolStripItemEditorForm : CollectionForm
        {
            private ToolStripCollectionEditor editor = null;
            private const int ICON_DIMENSION = 16;
            private const int SEPARATOR_HEIGHT = 4;
            private const int TEXT_IMAGE_SPACING = 6;
            private const int INDENT_SPACING = 4;
            private const int IMAGE_PADDING = 1;

            private static bool isScalingInitialized = false;
            private static int iconHeight = ICON_DIMENSION;
            private static int iconWidth = ICON_DIMENSION;
            private static int separatorHeight = SEPARATOR_HEIGHT;
            private static int textImageSpacing = TEXT_IMAGE_SPACING;
            private static int indentSpacing = INDENT_SPACING;
            private static int imagePaddingX = IMAGE_PADDING;
            private static int imagePaddingY = IMAGE_PADDING;

            private ToolStripCustomTypeDescriptor toolStripCustomTypeDescriptor = null;

            /// <summary>
            /// The amount of fudgespace we use when GDI+ returns us a string length.
            /// </summary>
            private const int GdiPlusFudge = 5;

            /// <summary>
            /// The collection we're actually editing.
            /// </summary>
            private ToolStripItemCollection _targetToolStripCollection;

            /// <summary>
            /// Our list of items that we're editing.
            /// </summary>
            private EditorItemCollection _itemList = null;

            /// <summary>
            /// The start index of custom items in the new item type dropdown.
            /// </summary>
            int customItemIndex = -1;

            /// <summary>
            /// All our control instance variables.
            /// </summary>
            private TableLayoutPanel tableLayoutPanel;
            private TableLayoutPanel addTableLayoutPanel;
            private TableLayoutPanel okCancelTableLayoutPanel;
            private Button btnCancel;
            private Button btnOK;
            private Button btnMoveUp;
            private Button btnMoveDown;
            private Label lblItems;
            private ComboBox newItemTypes;
            private Button btnAddNew;
            private CollectionEditor.FilterListBox listBoxItems;
            private Label selectedItemName;
            private Button btnRemove;
            private Label lblMembers;

            private IComponentChangeService _componentChangeSvc;
            private string _originalText = null;

            /// <summary>
            /// Create the form and set it up.
            /// </summary>
            /// <param name="parent">The collection editor that spawned us.</param>
            internal ToolStripItemEditorForm(CollectionEditor parent) : base(parent)
            {
                if (!isScalingInitialized)
                {
                    if (DpiHelper.IsScalingRequired)
                    {
                        iconHeight = DpiHelper.LogicalToDeviceUnitsY(ICON_DIMENSION);
                        iconWidth = DpiHelper.LogicalToDeviceUnitsX(ICON_DIMENSION);
                        separatorHeight = DpiHelper.LogicalToDeviceUnitsY(SEPARATOR_HEIGHT);
                        textImageSpacing = DpiHelper.LogicalToDeviceUnitsX(TEXT_IMAGE_SPACING);
                        indentSpacing = DpiHelper.LogicalToDeviceUnitsX(INDENT_SPACING);
                        imagePaddingX = DpiHelper.LogicalToDeviceUnitsX(IMAGE_PADDING);
                        imagePaddingY = DpiHelper.LogicalToDeviceUnitsY(IMAGE_PADDING);
                    }

                    isScalingInitialized = true;
                }

                this.editor = (ToolStripCollectionEditor)parent;
                InitializeComponent();
                if (DpiHelper.IsScalingRequired)
                {
                    DpiHelper.ScaleButtonImageLogicalToDevice(btnMoveUp);
                    DpiHelper.ScaleButtonImageLogicalToDevice(btnMoveDown);
                    DpiHelper.ScaleButtonImageLogicalToDevice(btnRemove);
                }

                this.ActiveControl = listBoxItems;
                this._originalText = Text;
                SetStyle(ControlStyles.ResizeRedraw, true);
            }

            /// <summary>
            /// The collection that we're editing.  Setting this causes us to sync our contents with that collection.
            /// </summary>
            internal ToolStripItemCollection Collection
            {
                set
                {
                    if (value != this._targetToolStripCollection)
                    {
                        // Clear our existing list of items.
                        if (_itemList != null)
                        {
                            _itemList.Clear();
                        }

                        // Add any existing items to our list.
                        if (value != null)
                        {
                            if (Context != null)
                            {
                                // Create a new list around the new value.
                                _itemList = new EditorItemCollection(this, listBoxItems.Items, value);

                                ToolStrip realToolStrip = ToolStripFromObject(Context.Instance);
                                _itemList.Add(realToolStrip);

                                ToolStripItem itemInstance = Context.Instance as ToolStripItem;
                                if (itemInstance != null && itemInstance.Site != null)
                                {
                                    this.Text = _originalText + " (" + itemInstance.Site.Name + "." + Context.PropertyDescriptor.Name + ")";
                                }

                                foreach (ToolStripItem item in value)
                                {
                                    if (item is DesignerToolStripControlHost)
                                    {
                                        continue;
                                    }

                                    _itemList.Add(item);
                                }

                                IComponentChangeService changeSvc = (IComponentChangeService)Context.GetService(typeof(IComponentChangeService));
                                if (changeSvc != null)
                                {
                                    changeSvc.ComponentChanged += new ComponentChangedEventHandler(OnComponentChanged);
                                }
                            }
                        }
                        else
                        {
                            if (_componentChangeSvc != null)
                            {
                                _componentChangeSvc.ComponentChanged -= new ComponentChangedEventHandler(OnComponentChanged);
                            }

                            _componentChangeSvc = null;
                        }

                        _targetToolStripCollection = value;
                    }
                }
            }

            private IComponentChangeService ComponentChangeService
            {
                get
                {
                    if (_componentChangeSvc == null && Context != null)
                    {
                        _componentChangeSvc = (IComponentChangeService)Context.GetService(typeof(IComponentChangeService));
                    }
                    return _componentChangeSvc;
                }
            }

            #region Windows Form Designer generated code

            /// <summary>
            /// Required method for Designer support - do not modify
            /// the contents of this method with the code editor.
            /// </summary>
            private void InitializeComponent()
            {
                ComponentResourceManager resources = new ComponentResourceManager(typeof(ToolStripItemEditorForm));
                this.btnCancel = new Button();
                this.btnOK = new Button();
                this.tableLayoutPanel = new TableLayoutPanel();
                this.addTableLayoutPanel = new TableLayoutPanel();
                this.btnAddNew = new Button();
                this.newItemTypes = new ImageComboBox();
                this.okCancelTableLayoutPanel = new TableLayoutPanel();
                this.lblItems = new Label();
                this.selectedItemName = new Label();
                this.lblMembers = new Label();
                this.listBoxItems = new CollectionEditor.FilterListBox();
                this.btnMoveUp = new Button();
                this.btnMoveDown = new Button();
                this.btnRemove = new Button();
                this.tableLayoutPanel.SuspendLayout();
                this.addTableLayoutPanel.SuspendLayout();
                this.okCancelTableLayoutPanel.SuspendLayout();
                this.SuspendLayout();
                // 
                // btnCancel
                // 
                resources.ApplyResources(this.btnCancel, "btnCancel");
                this.btnCancel.DialogResult = DialogResult.Cancel;
                this.btnCancel.Margin = new Padding(3, 0, 0, 0);
                this.btnCancel.Name = "btnCancel";
                // 
                // btnOK
                // 
                resources.ApplyResources(this.btnOK, "btnOK");
                this.btnOK.Margin = new Padding(0, 0, 3, 0);
                this.btnOK.Name = "btnOK";
                // 
                // tableLayoutPanel
                // 
                resources.ApplyResources(this.tableLayoutPanel, "tableLayoutPanel");
                this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 274F));
                this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
                this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                this.tableLayoutPanel.Controls.Add(this.addTableLayoutPanel, 0, 1);
                this.tableLayoutPanel.Controls.Add(this.okCancelTableLayoutPanel, 0, 6);
                this.tableLayoutPanel.Controls.Add(this.lblItems, 0, 0);
                this.tableLayoutPanel.Controls.Add(this.selectedItemName, 2, 0);
                this.tableLayoutPanel.Controls.Add(this.lblMembers, 0, 2);
                this.tableLayoutPanel.Controls.Add(this.listBoxItems, 0, 3);
                this.tableLayoutPanel.Controls.Add(this.btnMoveUp, 1, 3);
                this.tableLayoutPanel.Controls.Add(this.btnMoveDown, 1, 4);
                this.tableLayoutPanel.Controls.Add(this.btnRemove, 1, 5);
                this.tableLayoutPanel.Name = "tableLayoutPanel";
                this.tableLayoutPanel.RowStyles.Add(new RowStyle());
                this.tableLayoutPanel.RowStyles.Add(new RowStyle());
                this.tableLayoutPanel.RowStyles.Add(new RowStyle());
                this.tableLayoutPanel.RowStyles.Add(new RowStyle());
                this.tableLayoutPanel.RowStyles.Add(new RowStyle());
                this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
                this.tableLayoutPanel.RowStyles.Add(new RowStyle());
                // 
                // addTableLayoutPanel
                // 
                resources.ApplyResources(this.addTableLayoutPanel, "addTableLayoutPanel");
                this.addTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                this.addTableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
                this.addTableLayoutPanel.Controls.Add(this.btnAddNew, 1, 0);
                this.addTableLayoutPanel.Controls.Add(this.newItemTypes, 0, 0);
                this.addTableLayoutPanel.Margin = new Padding(0, 3, 3, 3);
                this.addTableLayoutPanel.Name = "addTableLayoutPanel";
                this.addTableLayoutPanel.AutoSize = true;
                this.addTableLayoutPanel.RowStyles.Add(new RowStyle());
                // 
                // btnAddNew
                // 
                resources.ApplyResources(this.btnAddNew, "btnAddNew");
                this.btnAddNew.Margin = new Padding(3, 0, 0, 0);
                this.btnAddNew.Name = "btnAddNew";
                // 
                // newItemTypes
                // 
                resources.ApplyResources(this.newItemTypes, "newItemTypes");
                this.newItemTypes.DropDownStyle = ComboBoxStyle.DropDownList;
                this.newItemTypes.FormattingEnabled = true;
                this.newItemTypes.Margin = new Padding(0, 0, 3, 0);
                this.newItemTypes.Name = "newItemTypes";
                this.newItemTypes.DrawMode = DrawMode.OwnerDrawVariable;

                // 
                // okCancelTableLayoutPanel
                // 
                resources.ApplyResources(this.okCancelTableLayoutPanel, "okCancelTableLayoutPanel");
                this.tableLayoutPanel.SetColumnSpan(this.okCancelTableLayoutPanel, 3);
                this.okCancelTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
                this.okCancelTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
                this.okCancelTableLayoutPanel.Controls.Add(this.btnOK, 0, 0);
                this.okCancelTableLayoutPanel.Controls.Add(this.btnCancel, 1, 0);
                this.okCancelTableLayoutPanel.Margin = new Padding(3, 6, 0, 0);
                this.okCancelTableLayoutPanel.Name = "okCancelTableLayoutPanel";
                this.okCancelTableLayoutPanel.RowStyles.Add(new RowStyle());
                // 
                // lblItems
                // 
                resources.ApplyResources(this.lblItems, "lblItems");
                this.lblItems.Margin = new Padding(0, 3, 3, 0);
                this.lblItems.Name = "lblItems";
                // 
                // selectedItemName
                // 
                resources.ApplyResources(this.selectedItemName, "selectedItemName");
                this.selectedItemName.Margin = new Padding(3, 3, 3, 0);
                this.selectedItemName.Name = "selectedItemName";
                // 
                // lblMembers
                // 
                resources.ApplyResources(this.lblMembers, "lblMembers");
                this.lblMembers.Margin = new Padding(0, 3, 3, 0);
                this.lblMembers.Name = "lblMembers";
                // 
                // listBoxItems
                // 
                resources.ApplyResources(this.listBoxItems, "listBoxItems");
                this.listBoxItems.DrawMode = DrawMode.OwnerDrawVariable;
                this.listBoxItems.FormattingEnabled = true;
                this.listBoxItems.Margin = new Padding(0, 3, 3, 3);
                this.listBoxItems.Name = "listBoxItems";
                this.tableLayoutPanel.SetRowSpan(this.listBoxItems, 3);
                this.listBoxItems.SelectionMode = SelectionMode.MultiExtended;
                // 
                // btnMoveUp
                // 
                resources.ApplyResources(this.btnMoveUp, "btnMoveUp");
                this.btnMoveUp.Margin = new Padding(3, 3, 18, 0);
                this.btnMoveUp.Name = "btnMoveUp";
                // 
                // btnMoveDown
                // 
                resources.ApplyResources(this.btnMoveDown, "btnMoveDown");
                this.btnMoveDown.Margin = new Padding(3, 1, 18, 3);
                this.btnMoveDown.Name = "btnMoveDown";
                // 
                // btnRemove
                // 
                resources.ApplyResources(this.btnRemove, "btnRemove");
                this.btnRemove.Margin = new Padding(3, 3, 18, 3);
                this.btnRemove.Name = "btnRemove";
                // 
                // ToolStripCollectionEditor
                // 
                this.AutoScaleMode = AutoScaleMode.Font;
                this.AcceptButton = this.btnOK;
                resources.ApplyResources(this, "$this");
                this.CancelButton = this.btnCancel;
                this.Controls.Add(this.tableLayoutPanel);
                this.HelpButton = true;
                this.MaximizeBox = false;
                this.MinimizeBox = false;
                this.Name = "ToolStripCollectionEditor";
                this.Padding = new Padding(9);
                this.ShowIcon = false;
                this.ShowInTaskbar = false;
                this.SizeGripStyle = SizeGripStyle.Show;
                this.tableLayoutPanel.ResumeLayout(false);
                this.tableLayoutPanel.PerformLayout();
                this.addTableLayoutPanel.ResumeLayout(false);
                this.addTableLayoutPanel.PerformLayout();
                this.okCancelTableLayoutPanel.ResumeLayout(false);
                this.okCancelTableLayoutPanel.PerformLayout();
                this.ResumeLayout(false);

                //events
                this.HelpButtonClicked += new CancelEventHandler(this.ToolStripCollectionEditor_HelpButtonClicked);
                this.newItemTypes.DropDown += new System.EventHandler(this.OnnewItemTypes_DropDown);
                this.newItemTypes.HandleCreated += new EventHandler(OnComboHandleCreated);
                this.newItemTypes.SelectedIndexChanged += new System.EventHandler(this.OnnewItemTypes_SelectedIndexChanged);
                this.btnAddNew.Click += new System.EventHandler(this.OnnewItemTypes_SelectionChangeCommitted);
                this.btnMoveUp.Click += new System.EventHandler(this.OnbtnMoveUp_Click);
                this.btnMoveDown.Click += new System.EventHandler(this.OnbtnMoveDown_Click);
                this.btnRemove.Click += new System.EventHandler(this.OnbtnRemove_Click);
                this.btnOK.Click += new System.EventHandler(this.OnbtnOK_Click);
                this.selectedItemName.Paint += new PaintEventHandler(this.OnselectedItemName_Paint);
                this.listBoxItems.SelectedIndexChanged += new System.EventHandler(this.OnlistBoxItems_SelectedIndexChanged);
                this.listBoxItems.DrawItem += new DrawItemEventHandler(this.OnlistBoxItems_DrawItem);
                this.listBoxItems.MeasureItem += new MeasureItemEventHandler(this.OnlistBoxItems_MeasureItem);

                this.Load += new System.EventHandler(this.OnFormLoad);
            }
            #endregion

            private void OnComboHandleCreated(object sender, EventArgs e)
            {
                // BUGBUG: syncing the MeasureItem event forces handle creation.
                this.newItemTypes.HandleCreated -= new EventHandler(OnComboHandleCreated);

                this.newItemTypes.MeasureItem += new MeasureItemEventHandler(this.OnlistBoxItems_MeasureItem);
                this.newItemTypes.DrawItem += new DrawItemEventHandler(this.OnlistBoxItems_DrawItem);
            }

            /// <summary>
            /// Adds a new item to our list.
            /// This will add a preview item and a listbox item as well.
            /// </summary>
            /// <param name="newItem">The item we're adding.</param>
            /// <param name="index">The index to add it at, or -1 to add it at the end.</param>
            private void AddItem(ToolStripItem newItem, int index)
            {
                if (index == -1)
                {
                    _itemList.Add(newItem);
                }
                else
                {
                    // Make sure we're legit.
                    if (index < 0 || index >= _itemList.Count)
                    {
                        throw new IndexOutOfRangeException();
                    }

                    _itemList.Insert(index, newItem);
                }

                ToolStrip ownerToolStrip = (Context != null) ? ToolStripFromObject(Context.Instance) : null;
                if (ownerToolStrip != null)
                {
                    // Set the owner to be the real winbar.
                    ownerToolStrip.Items.Add(newItem);
                }

                // Clear the current selection and set a new one.
                listBoxItems.ClearSelected();
                listBoxItems.SelectedItem = newItem;
            }

            /// <summary>
            /// Move an item from one index to the other.
            /// </summary>
            private void MoveItem(int fromIndex, int toIndex)
            {
                _itemList.Move(fromIndex, toIndex);
            }

            private void OnComponentChanged(object sender, ComponentChangedEventArgs e)
            {
                if (e.Component is ToolStripItem && e.Member is PropertyDescriptor && e.Member.Name == "Name")
                {
                    this.lblItems.Invalidate();
                }
            }

            /// <summary>
            /// Pick up the new collection value.
            /// </summary>
            protected override void OnEditValueChanged()
            {
                Collection = (ToolStripItemCollection)EditValue;
            }

            /// <summary>
            /// Called when the form loads...add the types from the list into the combo box.
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="e"></param>
            private void OnFormLoad(Object sender, System.EventArgs e)
            {
                // Set the font as appropriate.
                this.newItemTypes.ItemHeight = Math.Max(iconHeight, Font.Height);

                Component component = Context.Instance as Component;
                Debug.Assert(component != null, "why isnt the editor instance a component?");
                if (component != null)
                {
                    Type[] newToolStripItemTypes = ToolStripDesignerUtils.GetStandardItemTypes(component);

                    newItemTypes.Items.Clear();
                    foreach (Type t in newToolStripItemTypes)
                    {
                        this.newItemTypes.Items.Add(new TypeListItem(t));
                    }

                    newItemTypes.SelectedIndex = 0;

                    customItemIndex = -1;

                    newToolStripItemTypes = ToolStripDesignerUtils.GetCustomItemTypes(component, component.Site);
                    if (newToolStripItemTypes.Length > 0)
                    {
                        customItemIndex = newItemTypes.Items.Count;
                        foreach (Type t in newToolStripItemTypes)
                        {
                            this.newItemTypes.Items.Add(new TypeListItem(t));
                        }
                    }

                    if (listBoxItems.Items.Count > 0)
                    {
                        listBoxItems.SelectedIndex = 0;
                    }
                }
            }

            /// <summary>
            /// Handle a click of the OK button.
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void OnbtnOK_Click(System.Object sender, System.EventArgs e)
            {
                DialogResult = DialogResult.OK;
            }

            private void ToolStripCollectionEditor_HelpButtonClicked(object sender, CancelEventArgs e)
            {
                e.Cancel = true;
                editor.ShowHelp();
            }

            /// <summary>
            /// Remove all the selected items.
            /// </summary>
            private void OnbtnRemove_Click(Object sender, EventArgs e)
            {
                // Move the selected items into an array so it doesn't change as we remove from it.
                ToolStripItem[] items = new ToolStripItem[listBoxItems.SelectedItems.Count];
                listBoxItems.SelectedItems.CopyTo(items, 0);

                // Now remove each of the items.
                for (int i = 0; i < items.Length; i++)
                {
                    RemoveItem(items[i]);
                }

            }

            /// <summary>
            /// Move the selected item down one notch in the list.
            /// </summary>
            private void OnbtnMoveDown_Click(Object sender, EventArgs e)
            {
                ToolStripItem currentItem = (ToolStripItem)listBoxItems.SelectedItem;
                int currentIndex = listBoxItems.Items.IndexOf(currentItem);
                MoveItem(currentIndex, ++currentIndex);
                listBoxItems.SelectedIndex = currentIndex;
            }

            /// <summary>
            /// Move the selected item up one notch in the list.
            /// </summary>
            private void OnbtnMoveUp_Click(Object sender, EventArgs e)
            {
                ToolStripItem currentItem = (ToolStripItem)listBoxItems.SelectedItem;
                int currentIndex = listBoxItems.Items.IndexOf(currentItem);
                if (currentIndex > 1)
                {
                    MoveItem(currentIndex, --currentIndex);
                    listBoxItems.SelectedIndex = currentIndex;
                }
            }

            /// <summary>
            /// When we drop the combo, make sure it's wide enough to show the 
            /// full text from all the items.
            /// </summary>
            private void OnnewItemTypes_DropDown(Object sender, EventArgs e)
            {
                if (newItemTypes.Tag == null || (bool)newItemTypes.Tag == false)
                {
                    int itemWidth = newItemTypes.ItemHeight;
                    int dropDownHeight = 0;

                    // Walk the items and get the widest one.
                    using (Graphics g = newItemTypes.CreateGraphics())
                    {
                        foreach (TypeListItem item in newItemTypes.Items)
                        {
                            itemWidth = (int)Math.Max(itemWidth, newItemTypes.ItemHeight + 1 + g.MeasureString(item.Type.Name, newItemTypes.Font).Width + GdiPlusFudge);
                            dropDownHeight += (Font.Height + separatorHeight) + 2 * imagePaddingY;
                        }
                    }

                    newItemTypes.DropDownWidth = itemWidth;
                    newItemTypes.DropDownHeight = dropDownHeight;

                    // Store that we've already done this work.
                    newItemTypes.Tag = true;
                }
            }

            /// <summary>
            /// When the user makes an actual selection change, add an item to the winbar.
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="e">The event arguments.</param>
            private void OnnewItemTypes_SelectionChangeCommitted(Object sender, EventArgs e)
            {
                // Get the item type.
                TypeListItem typeItem = newItemTypes.SelectedItem as TypeListItem;
                if (typeItem != null)
                {
                    // Create the item
                    ToolStripItem newItem = (ToolStripItem)CreateInstance(typeItem.Type);

                    // Set the Image property and DisplayStyle...
                    if (newItem is ToolStripButton || newItem is ToolStripSplitButton || newItem is ToolStripDropDownButton)
                    {
                        Image image = null;
                        try
                        {
                            image = new Bitmap(typeof(ToolStripButton), "blank.bmp");
                        }
                        catch (Exception ex)
                        {
                            if (ClientUtils.IsCriticalException(ex))
                            {
                                throw;
                            }
                        }

                        PropertyDescriptor imageProperty = TypeDescriptor.GetProperties(newItem)["Image"];
                        Debug.Assert(imageProperty != null, "Could not find 'Image' property in ToolStripItem.");
                        if (imageProperty != null && image != null)
                        {
                            imageProperty.SetValue(newItem, image);
                        }

                        PropertyDescriptor dispProperty = TypeDescriptor.GetProperties(newItem)["DisplayStyle"];
                        Debug.Assert(dispProperty != null, "Could not find 'DisplayStyle' property in ToolStripItem.");
                        if (dispProperty != null)
                        {
                            dispProperty.SetValue(newItem, ToolStripItemDisplayStyle.Image);
                        }

                        PropertyDescriptor imageTransProperty = TypeDescriptor.GetProperties(newItem)["ImageTransparentColor"];
                        Debug.Assert(imageTransProperty != null, "Could not find 'ImageTransparentColor' property in ToolStripItem.");
                        if (imageTransProperty != null)
                        {
                            imageTransProperty.SetValue(newItem, Color.Magenta);
                        }
                    }

                    // Add it.
                    AddItem(newItem, -1);
                    listBoxItems.Focus();
                }
            }

            /// <summary>
            /// Just invalidate the combo on a selection change.
            /// </summary>
            private void OnnewItemTypes_SelectedIndexChanged(Object sender, EventArgs e)
            {
                newItemTypes.Invalidate();
            }

            /// <summary>
            /// Custom measureItem for the ListBox items.
            /// </summary>
            private void OnlistBoxItems_MeasureItem(object sender, MeasureItemEventArgs e)
            {
                int separator = 0;
                if (sender is ComboBox)
                {
                    bool drawSeparator = e.Index == customItemIndex;

                    if (e.Index >= 0 && drawSeparator)
                    {
                        separator = separatorHeight;
                    }
                }

                Font measureFont = this.Font;
                e.ItemHeight = Math.Max(iconHeight + separator, measureFont.Height + separator) + 2 * imagePaddingY;
            }

            /// <summary>
            /// Custom draw the list box item with the icon and the text.
            /// We actually share this code b/t the list box and the combobox.
            /// </summary>
            private void OnlistBoxItems_DrawItem(Object sender, DrawItemEventArgs e)
            {
                if (e.Index == -1)
                {
                    return;
                }

                Type itemType = null;
                string itemText = null;
                bool indentItem = false;
                bool drawSeparator = false;
                bool isComboEditBox = ((e.State & DrawItemState.ComboBoxEdit) == DrawItemState.ComboBoxEdit);

                if (sender is ListBox)
                {
                    ListBox listBox = sender as ListBox;
                    
                    // The list box has the items directly.
                    Component item = listBox.Items[e.Index] as Component;
                    if (item == null)
                    {
                        Debug.Fail("Unexpected listbox item painted!");
                        return;
                    }

                    if (item is ToolStripItem)
                    {
                        indentItem = true;
                    }

                    itemType = item.GetType();
                    itemText = (item.Site != null) ? item.Site.Name : itemType.Name;
                }
                else if (sender is ComboBox)
                {
                    // The combobox has just the types.
                    drawSeparator = ((e.Index == customItemIndex) && !isComboEditBox); // never draw the separator in the edit box, even if it is the selected index.
                    TypeListItem typeListItem = ((ComboBox)sender).Items[e.Index] as TypeListItem;
                    if (typeListItem == null)
                    {
                        Debug.Fail("Unexpected combobox item.");
                        return;
                    }

                    itemType = typeListItem.Type;
                    itemText = typeListItem.ToString();
                }
                else
                {
                    Debug.Fail("Unexpected sender calling DrawItem.");
                    return;
                }

                // We've got ourselves an item type. Draw it.
                if (itemType != null)
                {
                    Color textColor = Color.Empty;

                    if (drawSeparator)
                    {
                        e.Graphics.DrawLine(SystemPens.ControlDark, e.Bounds.X + 2, e.Bounds.Y + 2, e.Bounds.Right - 2, e.Bounds.Y + 2);
                    }

                    // Get the toolbox bitmap, draw it, and then draw the text.  We just
                    // draw the bitmap as a square based on the height of this line item.

                    // Calculate the image rect.
                    Rectangle imageBounds = e.Bounds;
                    imageBounds.Size = new Size(iconWidth, iconHeight);
                    int xOffset = (isComboEditBox) ? 0 : imagePaddingX * 2;
                    imageBounds.Offset(xOffset, imagePaddingX);

                    if (drawSeparator)
                    {
                        imageBounds.Offset(0, separatorHeight);
                    }

                    if (indentItem)
                    {
                        imageBounds.X += iconWidth + indentSpacing;
                    }

                    // Make sure after all this we stil are within bounds and are square.
                    if (!isComboEditBox)
                    {
                        imageBounds.Intersect(e.Bounds);
                    }

                    // Draw the image if it's there.
                    Bitmap tbxBitmap = ToolStripDesignerUtils.GetToolboxBitmap(itemType);
                    if (tbxBitmap != null)
                    {
                        if (isComboEditBox)
                        {
                            // Paint the icon of the combo's textbox area.
                            e.Graphics.DrawImage(tbxBitmap, e.Bounds.X, e.Bounds.Y, iconWidth, iconHeight);
                        }
                        else
                        {
                            e.Graphics.FillRectangle(SystemBrushes.Window, imageBounds);
                            e.Graphics.DrawImage(tbxBitmap, imageBounds);
                        }
                    }

                    // Calculate the text rect.
                    Rectangle textBounds = e.Bounds;
                    textBounds.X = imageBounds.Right + textImageSpacing;
                    textBounds.Y = imageBounds.Top - imagePaddingY;
                    if (!isComboEditBox)
                    {
                        textBounds.Y += imagePaddingY * 2;
                    }

                    textBounds.Intersect(e.Bounds);

                    // Draw the background as necessary.
                    Rectangle fillBounds = e.Bounds;
                    fillBounds.X = textBounds.X - 2;

                    if (drawSeparator)
                    {
                        fillBounds.Y += separatorHeight;
                        fillBounds.Height -= separatorHeight;
                    }

                    if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                    {
                        textColor = SystemColors.HighlightText;
                        e.Graphics.FillRectangle(SystemBrushes.Highlight, fillBounds);
                    }
                    else
                    {
                        textColor = SystemColors.WindowText;
                        e.Graphics.FillRectangle(SystemBrushes.Window, fillBounds);
                    }

                    // Render the text.
                    if (!string.IsNullOrEmpty(itemText))
                    {
                        TextFormatFlags format = TextFormatFlags.Top | TextFormatFlags.Left;
                        TextRenderer.DrawText(e.Graphics, itemText, this.Font, textBounds, textColor, format);
                    }

                    // Finally, draw the focusrect.
                    if ((e.State & DrawItemState.Focus) == DrawItemState.Focus)
                    {
                        fillBounds.Width -= 1;
                        ControlPaint.DrawFocusRectangle(e.Graphics, fillBounds, e.ForeColor, e.BackColor);
                    }
                }
            }

            /// <summary>
            /// Push the selected items into the property grid.
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="e">The event arguments.</param>
            private void OnlistBoxItems_SelectedIndexChanged(Object sender, EventArgs e)
            {
                // Push the items into the grid.
                object[] objList = new object[listBoxItems.SelectedItems.Count];
                if (objList.Length > 0)
                {
                    listBoxItems.SelectedItems.CopyTo(objList, 0);
                }

                // ToolStrip is selected ... remove the items property.
                if (objList.Length == 1 && objList[0] is ToolStrip)
                {
                    ToolStrip parentStrip = objList[0] as ToolStrip;
                    if (parentStrip != null && parentStrip.Site != null)
                    {
                        if (toolStripCustomTypeDescriptor == null)
                        {
                            toolStripCustomTypeDescriptor = new ToolStripCustomTypeDescriptor((ToolStrip)objList[0]);
                        }
                    }
                }

                // Enable the up/down button and the remove button based on the items.
                btnMoveUp.Enabled = (listBoxItems.SelectedItems.Count == 1) && (listBoxItems.SelectedIndex > 1);
                btnMoveDown.Enabled = (listBoxItems.SelectedItems.Count == 1) && (listBoxItems.SelectedIndex < listBoxItems.Items.Count - 1);
                this.btnRemove.Enabled = objList.Length > 0;

                // Cannot remove a Winbar thru this CollectionEditor.
                foreach (object obj in listBoxItems.SelectedItems)
                {
                    if (obj is ToolStrip)
                    {
                        btnRemove.Enabled = btnMoveUp.Enabled = btnMoveDown.Enabled = false;
                        break;
                    }
                }

                // Invalidate the listbox and the label above the grid.
                listBoxItems.Invalidate();
                selectedItemName.Invalidate();
            }

            /// <summary>
            /// Invalidate the ListBox and the SelectedItemName Label on top of the propertyGrid.
            /// </summary>
            private void PropertyGrid_propertyValueChanged(object sender, PropertyValueChangedEventArgs e)
            {
                // Invalidate the listbox and the label above the grid.
                listBoxItems.Invalidate();
                selectedItemName.Invalidate();
            }

            /// <summary>
            /// Paint the name and type of the currently selected items in the label above the property grid.
            /// </summary>
            private void OnselectedItemName_Paint(System.Object sender, PaintEventArgs e)
            {
                // Make the bolded font for the type name.
                using (Font boldFont = new Font(selectedItemName.Font, FontStyle.Bold))
                {
                    Label label = sender as Label;
                    Rectangle bounds = label.ClientRectangle;
                    StringFormat stringFormat = null;

                    bool rightToLeft = (label.RightToLeft == RightToLeft.Yes);

                    if (rightToLeft)
                    {
                        stringFormat = new StringFormat(StringFormatFlags.DirectionRightToLeft);
                    }
                    else
                    {
                        stringFormat = new StringFormat();
                    }

                    stringFormat.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Show;

                    // Based on the count, just paint the name, (Multiple Items), or (None)
                    switch (listBoxItems.SelectedItems.Count)
                    {
                        case 1:

                            // For a single item, we paint it's classname in bold, then the item name.
                            Component selectedItem = null;
                            if (listBoxItems.SelectedItem is ToolStrip)
                            {
                                selectedItem = (ToolStrip)listBoxItems.SelectedItem;
                            }
                            else
                            {
                                selectedItem = (ToolStripItem)listBoxItems.SelectedItem;
                            }

                            string className = "&" + selectedItem.GetType().Name;
                            if (selectedItem.Site != null)
                            {
                                e.Graphics.FillRectangle(SystemBrushes.Control, bounds);   // erase background
                                string itemName = selectedItem.Site.Name;

                                if (label != null)
                                {
                                    label.Text = className + itemName;
                                }

                                int classWidth = 0;
                                classWidth = (int)e.Graphics.MeasureString(className, boldFont).Width;
                                e.Graphics.DrawString(className, boldFont, SystemBrushes.WindowText, bounds, stringFormat);
                                
                                int itemTextWidth = (int)e.Graphics.MeasureString(itemName, selectedItemName.Font).Width;
                                Rectangle textRect = new Rectangle(classWidth + GdiPlusFudge, 0, bounds.Width - (classWidth + GdiPlusFudge), bounds.Height);
                                if (itemTextWidth > textRect.Width)
                                {
                                    label.AutoEllipsis = true;
                                }
                                else
                                {
                                    label.AutoEllipsis = false;
                                }

                                TextFormatFlags flags = TextFormatFlags.EndEllipsis;
                                if (rightToLeft)
                                {
                                    flags |= TextFormatFlags.RightToLeft;
                                }

                                TextRenderer.DrawText(e.Graphics, itemName, selectedItemName.Font, textRect, SystemColors.WindowText, flags);
                            }
                            break;

                        case 0:
                            e.Graphics.FillRectangle(SystemBrushes.Control, bounds); // Erase background.
                            if (label != null)
                            {
                                label.Text = SR.ToolStripItemCollectionEditorLabelNone;
                            }
                            e.Graphics.DrawString(SR.ToolStripItemCollectionEditorLabelNone, boldFont, SystemBrushes.WindowText, bounds, stringFormat);
                            break;

                        default:
                            e.Graphics.FillRectangle(SystemBrushes.Control, bounds); // erase background
                            if (label != null)
                            {
                                label.Text = SR.ToolStripItemCollectionEditorLabelMultipleItems;
                            }
                            e.Graphics.DrawString(SR.ToolStripItemCollectionEditorLabelMultipleItems, boldFont, SystemBrushes.WindowText, bounds, stringFormat);
                            break;
                    }

                    stringFormat.Dispose();
                }
            }

            /// <summary>
            /// Removes an item from the list and the preview winbar.
            /// </summary>
            /// <param name="item">The item to remove.</param>
            private void RemoveItem(ToolStripItem item)
            {
                int index;
                try
                {
                    // Remove the item from the list.
                    index = _itemList.IndexOf(item);
                    _itemList.Remove(item);
                }
                finally
                {
                    item.Dispose();
                }

                // Now set up our selection.
                if (_itemList.Count > 0)
                {
                    listBoxItems.ClearSelected();
                    index = Math.Max(0, Math.Min(index, listBoxItems.Items.Count - 1));
                    listBoxItems.SelectedIndex = index;
                }
            }

            /// <devdoc>
            /// Fishes out the ToolStrip from the object - which can be a ToolStrip or a ToolStripDropDownItem.
            /// </devdoc>
            internal static ToolStrip ToolStripFromObject(object instance)
            {
                ToolStrip currentToolStrip = null;

                if (instance != null)
                {
                    if (instance is ToolStripDropDownItem)
                    {
                        currentToolStrip = ((ToolStripDropDownItem)instance).DropDown;
                    }
                    else
                    {
                        currentToolStrip = instance as ToolStrip;
                    }
                }

                return currentToolStrip;
            }

            private class ImageComboBox : ComboBox
            {
                public ImageComboBox()
                {
                }
                private Rectangle ImageRect
                {
                    get
                    {
                        // Need to add padding for RTL combobox.
                        if (RightToLeft == RightToLeft.Yes)
                        {
                            return new Rectangle(4 + SystemInformation.HorizontalScrollBarThumbWidth, 3, iconWidth, iconHeight);
                        }

                        return new Rectangle(3, 3, iconWidth, iconHeight);
                    }
                }

                protected override void OnDropDownClosed(EventArgs e)
                {
                    base.OnDropDownClosed(e);
                    Invalidate(ImageRect);
                }

                protected override void OnSelectedIndexChanged(EventArgs e)
                {
                    base.OnSelectedIndexChanged(e);
                    Invalidate(ImageRect);
                }

                protected override void WndProc(ref Message m)
                {
                    base.WndProc(ref m);
                    switch (m.Msg)
                    {
                        case Interop.WindowMessages.WM_SETFOCUS:
                        case Interop.WindowMessages.WM_KILLFOCUS:
                            Invalidate(ImageRect);
                            break;
                    }
                }
            }

            /// <summary>
            /// This is a magic collection.  It's job is to keep the preview winbar and the listbox in sync and manage both sets of items.
            /// It contains a list of EditorItem objects, which whold the information for each item, and a reference to the real ToolStripItem component,
            /// the and the preview ToolStripItem.  The order and contents of this combobox always match that of the real collection, the list box, and the preview winbar.
            /// 
            /// It operates generically on three ILists: the listbox.Items collection, the previewToolStrip.Items collection, and the actual ToolStripItemCollection being designed.
            /// </summary>
            private class EditorItemCollection : CollectionBase
            {
                private IList _listBoxList;               // the ListBox.Items collection
                private IList _targetCollectionList;      // the real deal target collection
                private ToolStripItemEditorForm _owner;                     // the owner form that created this collection.

                /// <summary>
                /// Setup the collection's variables.
                /// </summary>
                internal EditorItemCollection(ToolStripItemEditorForm owner, IList displayList, IList componentList)
                {
                    _owner = owner;
                    _listBoxList = displayList;
                    _targetCollectionList = componentList;
                }

                /// <summary>
                /// Add a new winbaritem.  See OnInsertComplete for the actual add operation.
                /// </summary>
                /// <param name="item"></param>
                public void Add(object item)
                {
                    List.Add(new EditorItem(item));
                }

                /// <summary>
                /// This is a little tricky, since our list doesn't actually contain 
                /// ToolStripItems, but rather EditorItems, we have to walk those.  No bother,
                /// this list is always pretty short.
                /// </summary>
                public int IndexOf(ToolStripItem item)
                {
                    for (int i = 0; i < List.Count; i++)
                    {
                        EditorItem editorItem = (EditorItem)List[i];
                        if (editorItem.Component == item)
                        {
                            return i;
                        }
                    }
                    return -1;
                }

                /// <summary>
                /// Insert an item into the list somewhere.  See OnInsertComplete for the real meaty stuff.
                /// </summary>
                /// <param name="index"></param>
                /// <param name="item"></param>
                public void Insert(int index, ToolStripItem item)
                {
                    List.Insert(index, new EditorItem(item));
                }

                /// <summary>
                /// Move an item from one array position to another.
                /// </summary>
                public void Move(int fromIndex, int toIndex)
                {
                    if (toIndex == fromIndex)
                    {
                        return;
                    }

                    EditorItem editorItem = (EditorItem)List[fromIndex];
                    if (editorItem.Host != null)
                    {
                        return;
                    }
                    try
                    {
                        _owner.Context.OnComponentChanging();
                        _listBoxList.Remove(editorItem.Component);
                        _targetCollectionList.Remove(editorItem.Component);

                        InnerList.Remove(editorItem);

                        // Put all back in.
                        _listBoxList.Insert(toIndex, editorItem.Component);

                        // Since ToolStrip is also an item of the listBoxItems,
                        // lets decrement the counter by one.

                        // ToolStrip is Always the TOP item,
                        // so it is SAFE to assume that 
                        // the index that we want is always currentIndex - 1.

                        // This is required as the _targetList doesnt contain the ToolStrip.
                        _targetCollectionList.Insert(toIndex - 1, editorItem.Component);

                        InnerList.Insert(toIndex, editorItem);
                    }
                    finally
                    {
                        _owner.Context.OnComponentChanged();
                    }
                }

                /// <summary>
                /// Clear has a differnet semantic than remove.  Clear simply dumps all the items
                /// out of the listbox and the preview and zero's this collection.  Remove is more 
                /// destructive in that it affects the target collection.
                /// </summary>
                protected override void OnClear()
                {
                    _listBoxList.Clear();
                    foreach (EditorItem item in List)
                    {
                        item.Dispose();
                    }

                    // We don't do sync target list here.
                    base.OnClear();
                }

                /// <summary>
                /// After an item is inserted into the collection, we do the work
                /// to make sure that we sync up the three lists.
                /// </summary>
                /// <param name="index">The index.</param>
                /// <param name="value">The value.</param>
                protected override void OnInsertComplete(int index, object value)
                {
                    EditorItem item = (EditorItem)value;
                    if (item.Host != null)
                    {
                        // Insert into the listbox.
                        _listBoxList.Insert(index, item.Host);
                        base.OnInsertComplete(index, value);
                        return;
                    }

                    // Check the target collection first, if it's already there,
                    // chill. Otherwise we need to push it in. In the case that we're
                    // sync'ing to an existing list, we don't want to be re-adding
                    // everything.
                    if (!_targetCollectionList.Contains(item.Component))
                    {
                        try
                        {
                            _owner.Context.OnComponentChanging();
                            _targetCollectionList.Insert(index - 1, item.Component);
                        }
                        finally
                        {
                            _owner.Context.OnComponentChanged();
                        }
                    }

                    // Insert into the listbox and into the preview.
                    _listBoxList.Insert(index, item.Component);
                    base.OnInsertComplete(index, value);
                }

                /// <summary>
                /// Really remove an item from the collections.
                /// </summary>
                /// <param name="index">The index.</param>
                /// <param name="value">The values.</param>
                protected override void OnRemove(int index, object value)
                {
                    EditorItem editorItem = (EditorItem)List[index];

                    // Pull it from the listbox and preview.
                    _listBoxList.RemoveAt(index);

                    // Yank it from the collection. The code that calls this
                    // collection is responsible for disposing it to destroy
                    // it in the designer.
                    try
                    {
                        _owner.Context.OnComponentChanging();
                        _targetCollectionList.RemoveAt(index - 1);
                    }
                    finally
                    {
                        _owner.Context.OnComponentChanged();
                    }

                    // Finally dispose the editor item which cleansup the 
                    // preview item.
                    editorItem.Dispose();
                    base.OnRemove(index, value);
                }

                /// <summary>
                /// Remove an item.  See OnRemove for the gory details.
                /// </summary>
                /// <param name="item">The item.</param>
                public void Remove(ToolStripItem item)
                {
                    int index = IndexOf(item);
                    List.RemoveAt(index);
                }

                /// <summary>
                /// This class keeps track of the mapping between a winbaritem in the designer, in the 
                /// preview winbar, and in the listbox.
                /// </summary>
                private class EditorItem
                {
                    public ToolStripItem _component; // The real deal item in the designer.
                    public ToolStrip _host;

                    internal EditorItem(object componentItem)
                    {
                        if (componentItem is ToolStrip)
                        {
                            _host = (ToolStrip)componentItem;
                        }
                        else
                        {
                            _component = (ToolStripItem)componentItem;
                        }
                    }

                    /// <summary>
                    /// The item that's actually being created in the designer.
                    /// </summary>
                    public ToolStripItem Component
                    {
                        get
                        {
                            return _component;
                        }
                    }

                    /// <summary>
                    /// The ToolStrip that's actually being created in the designer.
                    /// </summary>
                    public ToolStrip Host
                    {
                        get
                        {
                            return _host;
                        }
                    }

                    /// <summary>
                    /// Cleanup our mess.
                    /// </summary>
                    public void Dispose()
                    {
                        GC.SuppressFinalize(this);
                        this._component = null;
                    }
                }
            }

            private class TypeListItem
            {
                public readonly Type Type;

                public TypeListItem(Type t)
                {
                    Type = t;
                }

                public override string ToString()
                {
                    return ToolStripDesignerUtils.GetToolboxDescription(Type);
                }
            }
        }
    }
}
