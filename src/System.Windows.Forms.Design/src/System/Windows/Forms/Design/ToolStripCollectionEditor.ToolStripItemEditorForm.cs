// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;

#nullable disable

namespace System.Windows.Forms.Design;

internal partial class ToolStripCollectionEditor
{
    /// <summary>
    ///  Our internal form UI for the <see cref="ToolStripCollectionEditor" />.
    /// </summary>
    protected class ToolStripItemEditorForm : CollectionForm
    {
        private readonly ToolStripCollectionEditor _editor;
        private const int ICON_DIMENSION = 16;
        private const int SEPARATOR_HEIGHT = 4;
        private const int TEXT_IMAGE_SPACING = 6;
        private const int INDENT_SPACING = 4;
        private const int IMAGE_PADDING = 1;

        private static bool s_isScalingInitialized;
        private static int s_iconHeight = ICON_DIMENSION;
        private static int s_iconWidth = ICON_DIMENSION;
        private static int s_separatorHeight = SEPARATOR_HEIGHT;
        private static int s_textImageSpacing = TEXT_IMAGE_SPACING;
        private static int s_indentSpacing = INDENT_SPACING;
        private static int s_imagePaddingX = IMAGE_PADDING;
        private static int s_imagePaddingY = IMAGE_PADDING;

        private ToolStripCustomTypeDescriptor _toolStripCustomTypeDescriptor;

        /// <summary>
        ///  The amount of space we use when GDI+ returns us a string length.
        /// </summary>
        private const int GdiPlusExtraSpace = 5;

        /// <summary>
        ///  The collection we're actually editing.
        /// </summary>
        private ToolStripItemCollection _targetToolStripCollection;

        /// <summary>
        ///  Our list of items that we're editing.
        /// </summary>
        private EditorItemCollection _itemList;

        /// <summary>
        ///  The start index of custom items in the new item type dropdown.
        /// </summary>
        private int _customItemIndex = -1;

        /// <summary>
        ///  All our control instance variables.
        /// </summary>
        private TableLayoutPanel _tableLayoutPanel;
        private TableLayoutPanel _addTableLayoutPanel;
        private TableLayoutPanel _okCancelTableLayoutPanel;
        private Button _btnCancel;
        private Button _btnOK;
        private Button _btnMoveUp;
        private Button _btnMoveDown;
        private Label _lblItems;
        private ImageComboBox _newItemTypes;
        private Button _btnAddNew;
        private FilterListBox _listBoxItems;
        private Label _selectedItemName;
        private Button _btnRemove;
        private VsPropertyGrid _selectedItemProps;
        private Label _lblMembers;

        private IComponentChangeService _componentChangeSvc;
        private readonly string _originalText;

        /// <summary>
        ///  Create the form and set it up.
        /// </summary>
        /// <param name="parent">The collection editor that spawned us.</param>
        internal ToolStripItemEditorForm(CollectionEditor parent) : base(parent)
        {
            if (!s_isScalingInitialized)
            {
                if (ScaleHelper.IsScalingRequired)
                {
                    s_iconHeight = LogicalToDeviceUnits(ICON_DIMENSION);
                    s_iconWidth = LogicalToDeviceUnits(ICON_DIMENSION);
                    s_separatorHeight = LogicalToDeviceUnits(SEPARATOR_HEIGHT);
                    s_textImageSpacing = LogicalToDeviceUnits(TEXT_IMAGE_SPACING);
                    s_indentSpacing = LogicalToDeviceUnits(INDENT_SPACING);
                    s_imagePaddingX = LogicalToDeviceUnits(IMAGE_PADDING);
                    s_imagePaddingY = LogicalToDeviceUnits(IMAGE_PADDING);
                }

                s_isScalingInitialized = true;
            }

            _editor = (ToolStripCollectionEditor)parent;
            InitializeComponent();
            if (ScaleHelper.IsScalingRequired)
            {
                ScaleButtonImageLogicalToDevice(_btnMoveUp);
                ScaleButtonImageLogicalToDevice(_btnMoveDown);
                ScaleButtonImageLogicalToDevice(_btnRemove);
            }

            ActiveControl = _listBoxItems;
            _originalText = Text;
            SetStyle(ControlStyles.ResizeRedraw, true);
        }

        /// <summary>
        ///  The collection that we're editing. Setting this causes us to sync our contents with that collection.
        /// </summary>
        internal ToolStripItemCollection Collection
        {
            set
            {
                if (value != _targetToolStripCollection)
                {
                    // Clear our existing list of items.
                    _itemList?.Clear();

                    // Add any existing items to our list.
                    if (value is not null)
                    {
                        if (Context is not null)
                        {
                            // Create a new list around the new value.
                            _itemList = new(this, _listBoxItems.Items, value);

                            ToolStrip realToolStrip = ToolStripFromObject(Context.Instance);
                            _itemList.Add(realToolStrip);

                            if (Context.Instance is ToolStripItem itemInstance && itemInstance.Site is not null)
                            {
                                Text = $"{_originalText} ({itemInstance.Site.Name}.{Context.PropertyDescriptor.Name})";
                            }

                            foreach (ToolStripItem item in value)
                            {
                                if (item is DesignerToolStripControlHost)
                                {
                                    continue;
                                }

                                _itemList.Add(item);
                            }

                            if (Context.GetService<IComponentChangeService>() is IComponentChangeService changeService)
                            {
                                changeService.ComponentChanged += OnComponentChanged;
                            }

                            _selectedItemProps.Site = new PropertyGridSite(Context, _selectedItemProps);
                        }
                    }
                    else
                    {
                        if (_componentChangeSvc is not null)
                        {
                            _componentChangeSvc.ComponentChanged -= OnComponentChanged;
                        }

                        _componentChangeSvc = null;
                        _selectedItemProps.Site = null;
                    }

                    _targetToolStripCollection = value;
                }
            }
        }

        #region Windows Form Designer generated code
        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            ComponentResourceManager resources = new(typeof(ToolStripItemEditorForm));
            _btnCancel = new();
            _btnOK = new();
            _tableLayoutPanel = new();
            _addTableLayoutPanel = new();
            _btnAddNew = new();
            _newItemTypes = new();
            _okCancelTableLayoutPanel = new();
            _lblItems = new();
            _selectedItemName = new();
            _selectedItemProps = new();
            _lblMembers = new();
            _listBoxItems = new();
            _btnMoveUp = new();
            _btnMoveDown = new();
            _btnRemove = new();
            _tableLayoutPanel.SuspendLayout();
            _addTableLayoutPanel.SuspendLayout();
            _okCancelTableLayoutPanel.SuspendLayout();
            SuspendLayout();
            // btnCancel
            resources.ApplyResources(_btnCancel, "btnCancel");
            _btnCancel.DialogResult = DialogResult.Cancel;
            _btnCancel.Margin = new(3, 0, 0, 0);
            _btnCancel.Name = "btnCancel";
            // btnOK
            resources.ApplyResources(_btnOK, "btnOK");
            _btnOK.Margin = new(0, 0, 3, 0);
            _btnOK.Name = "btnOK";
            // tableLayoutPanel
            resources.ApplyResources(_tableLayoutPanel, "tableLayoutPanel");
            _tableLayoutPanel.ColumnStyles.Add(new(SizeType.Absolute, 274F));
            _tableLayoutPanel.ColumnStyles.Add(new());
            _tableLayoutPanel.ColumnStyles.Add(new(SizeType.Percent, 100F));
            _tableLayoutPanel.Controls.Add(_addTableLayoutPanel, 0, 1);
            _tableLayoutPanel.Controls.Add(_okCancelTableLayoutPanel, 0, 6);
            _tableLayoutPanel.Controls.Add(_lblItems, 0, 0);
            _tableLayoutPanel.Controls.Add(_selectedItemName, 2, 0);
            _tableLayoutPanel.Controls.Add(_selectedItemProps, 2, 1);
            _tableLayoutPanel.Controls.Add(_lblMembers, 0, 2);
            _tableLayoutPanel.Controls.Add(_listBoxItems, 0, 3);
            _tableLayoutPanel.Controls.Add(_btnMoveUp, 1, 3);
            _tableLayoutPanel.Controls.Add(_btnMoveDown, 1, 4);
            _tableLayoutPanel.Controls.Add(_btnRemove, 1, 5);
            _tableLayoutPanel.Name = "tableLayoutPanel";
            _tableLayoutPanel.RowStyles.Add(new());
            _tableLayoutPanel.RowStyles.Add(new());
            _tableLayoutPanel.RowStyles.Add(new());
            _tableLayoutPanel.RowStyles.Add(new());
            _tableLayoutPanel.RowStyles.Add(new());
            _tableLayoutPanel.RowStyles.Add(new(SizeType.Percent, 100F));
            _tableLayoutPanel.RowStyles.Add(new());
            // addTableLayoutPanel
            resources.ApplyResources(_addTableLayoutPanel, "addTableLayoutPanel");
            _addTableLayoutPanel.ColumnStyles.Add(new(SizeType.Percent, 100F));
            _addTableLayoutPanel.ColumnStyles.Add(new());
            _addTableLayoutPanel.Controls.Add(_btnAddNew, 1, 0);
            _addTableLayoutPanel.Controls.Add(_newItemTypes, 0, 0);
            _addTableLayoutPanel.Margin = new(0, 3, 3, 3);
            _addTableLayoutPanel.Name = "addTableLayoutPanel";
            _addTableLayoutPanel.AutoSize = true;
            _addTableLayoutPanel.RowStyles.Add(new());
            // btnAddNew
            resources.ApplyResources(_btnAddNew, "btnAddNew");
            _btnAddNew.Margin = new(3, 0, 0, 0);
            _btnAddNew.Name = "btnAddNew";
            // newItemTypes
            resources.ApplyResources(_newItemTypes, "newItemTypes");
            _newItemTypes.DropDownStyle = ComboBoxStyle.DropDownList;
            _newItemTypes.FormattingEnabled = true;
            _newItemTypes.Margin = new(0, 0, 3, 0);
            _newItemTypes.Name = "newItemTypes";
            _newItemTypes.DrawMode = DrawMode.OwnerDrawVariable;

            // okCancelTableLayoutPanel
            resources.ApplyResources(_okCancelTableLayoutPanel, "okCancelTableLayoutPanel");
            _tableLayoutPanel.SetColumnSpan(_okCancelTableLayoutPanel, 3);
            _okCancelTableLayoutPanel.ColumnStyles.Add(new(SizeType.Percent, 50F));
            _okCancelTableLayoutPanel.ColumnStyles.Add(new(SizeType.Percent, 50F));
            _okCancelTableLayoutPanel.Controls.Add(_btnOK, 0, 0);
            _okCancelTableLayoutPanel.Controls.Add(_btnCancel, 1, 0);
            _okCancelTableLayoutPanel.Margin = new(3, 6, 0, 0);
            _okCancelTableLayoutPanel.Name = "okCancelTableLayoutPanel";
            _okCancelTableLayoutPanel.RowStyles.Add(new());
            // lblItems
            resources.ApplyResources(_lblItems, "lblItems");
            _lblItems.Margin = new(0, 3, 3, 0);
            _lblItems.Name = "lblItems";
            // selectedItemName
            resources.ApplyResources(_selectedItemName, "selectedItemName");
            _selectedItemName.Margin = new(3, 3, 3, 0);
            _selectedItemName.Name = "selectedItemName";
            // selectedItemProps
            _selectedItemProps.CommandsVisibleIfAvailable = false;
            resources.ApplyResources(_selectedItemProps, "selectedItemProps");
            _selectedItemProps.Margin = new(3, 3, 0, 3);
            _selectedItemProps.Name = "selectedItemProps";
            _tableLayoutPanel.SetRowSpan(_selectedItemProps, 5);
            // lblMembers
            resources.ApplyResources(_lblMembers, "lblMembers");
            _lblMembers.Margin = new(0, 3, 3, 0);
            _lblMembers.Name = "lblMembers";
            // listBoxItems
            resources.ApplyResources(_listBoxItems, "listBoxItems");
            _listBoxItems.DrawMode = DrawMode.OwnerDrawVariable;
            _listBoxItems.FormattingEnabled = true;
            _listBoxItems.Margin = new(0, 3, 3, 3);
            _listBoxItems.Name = "listBoxItems";
            _tableLayoutPanel.SetRowSpan(_listBoxItems, 3);
            _listBoxItems.SelectionMode = SelectionMode.MultiExtended;
            // btnMoveUp
            resources.ApplyResources(_btnMoveUp, "btnMoveUp");
            _btnMoveUp.Margin = new(3, 3, 18, 0);
            _btnMoveUp.Name = "btnMoveUp";
            // btnMoveDown
            resources.ApplyResources(_btnMoveDown, "btnMoveDown");
            _btnMoveDown.Margin = new(3, 1, 18, 3);
            _btnMoveDown.Name = "btnMoveDown";
            // btnRemove
            resources.ApplyResources(_btnRemove, "btnRemove");
            _btnRemove.Margin = new(3, 3, 18, 3);
            _btnRemove.Name = "btnRemove";
            // ToolStripCollectionEditor
            AutoScaleMode = AutoScaleMode.Font;
            AcceptButton = _btnOK;
            resources.ApplyResources(this, "$this");
            CancelButton = _btnCancel;
            Controls.Add(_tableLayoutPanel);
            HelpButton = true;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ToolStripCollectionEditor";
            Padding = new(9);
            ShowIcon = false;
            ShowInTaskbar = false;
            SizeGripStyle = SizeGripStyle.Show;
            _tableLayoutPanel.ResumeLayout(false);
            _tableLayoutPanel.PerformLayout();
            _addTableLayoutPanel.ResumeLayout(false);
            _addTableLayoutPanel.PerformLayout();
            _okCancelTableLayoutPanel.ResumeLayout(false);
            _okCancelTableLayoutPanel.PerformLayout();
            ResumeLayout(false);

            // Events
            HelpButtonClicked += ToolStripCollectionEditor_HelpButtonClicked;
            _newItemTypes.DropDown += OnNewItemTypes_DropDown;
            _newItemTypes.HandleCreated += OnComboHandleCreated;
            _newItemTypes.SelectedIndexChanged += OnNewItemTypes_SelectedIndexChanged;
            _btnAddNew.Click += OnNewItemTypes_SelectionChangeCommitted;
            _btnMoveUp.Click += OnBtnMoveUp_Click;
            _btnMoveDown.Click += OnBtnMoveDown_Click;
            _btnRemove.Click += OnBtnRemove_Click;
            _btnOK.Click += OnBtnOK_Click;
            _selectedItemName.Paint += OnSelectedItemName_Paint;
            _listBoxItems.SelectedIndexChanged += OnListBoxItems_SelectedIndexChanged;
            _listBoxItems.DrawItem += OnListBoxItems_DrawItem;
            _listBoxItems.MeasureItem += OnListBoxItems_MeasureItem;

            _selectedItemProps.PropertyValueChanged += PropertyGrid_propertyValueChanged;
            Load += OnFormLoad;
        }
        #endregion

        /// <summary>
        ///  Create a new button bitmap scaled for the device units.
        ///  Note: original image might be disposed.
        /// </summary>
        /// <param name="button">button with an image, image size is defined in logical units</param>
        private static void ScaleButtonImageLogicalToDevice(Button button)
        {
            if (button?.Image is not Bitmap buttonBitmap)
            {
                return;
            }

            button.Image = ScaleHelper.ScaleToDpi(buttonBitmap, ScaleHelper.InitialSystemDpi, disposeBitmap: true);
        }

        private void OnComboHandleCreated(object sender, EventArgs e)
        {
            // BUGBUG: syncing the MeasureItem event forces handle creation.
            _newItemTypes.HandleCreated -= OnComboHandleCreated;

            _newItemTypes.MeasureItem += OnListBoxItems_MeasureItem;
            _newItemTypes.DrawItem += OnListBoxItems_DrawItem;
        }

        /// <summary>
        ///  Add a new item to our list. This will add a preview item
        ///  and a list box item as well.
        /// </summary>
        /// <param name="newItem">The item we're adding</param>
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
                    return;
                }

                _itemList.Insert(index, newItem);
            }

            ToolStrip ownerToolStrip = Context is { Instance: not null } contextInstance
                ? ToolStripFromObject(contextInstance)
                : null;

            // Set the owner to be the real ToolStrip
            ownerToolStrip?.Items.Add(newItem);

            // Clear the current selection and set a new one.
            _listBoxItems.ClearSelected();
            _listBoxItems.SelectedItem = newItem;
        }

        /// <summary>
        ///  Move an item from one index to the other.
        /// </summary>
        private void MoveItem(int fromIndex, int toIndex)
        {
            _itemList.Move(fromIndex, toIndex);
        }

        private void OnComponentChanged(object sender, ComponentChangedEventArgs e)
        {
            if (e.Component is ToolStripItem && e.Member is PropertyDescriptor && e.Member.Name == "Name")
            {
                _lblItems.Invalidate();
            }
        }

        /// <summary>
        ///  Pick up the new collection value.
        /// </summary>
        protected override void OnEditValueChanged()
        {
            _selectedItemProps.SelectedObjects = null;
            Collection = (ToolStripItemCollection)EditValue;
        }

        /// <summary>
        ///  Called when the form loads...add the types from the list into the combo box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFormLoad(object sender, EventArgs e)
        {
            // Set the font as appropriate.
            _newItemTypes.ItemHeight = Math.Max(s_iconHeight, Font.Height);

            if (Context.Instance is not Component component)
            {
                return;
            }

            Type[] newToolStripItemTypes = ToolStripDesignerUtils.GetStandardItemTypes(component);

            _newItemTypes.Items.Clear();
            foreach (Type t in newToolStripItemTypes)
            {
                _newItemTypes.Items.Add(new TypeListItem(t));
            }

            _newItemTypes.SelectedIndex = 0;

            _customItemIndex = -1;

            newToolStripItemTypes = ToolStripDesignerUtils.GetCustomItemTypes(component, component.Site);
            if (newToolStripItemTypes.Length > 0)
            {
                _customItemIndex = _newItemTypes.Items.Count;
                foreach (Type t in newToolStripItemTypes)
                {
                    _newItemTypes.Items.Add(new TypeListItem(t));
                }
            }

            if (_listBoxItems.Items.Count > 0)
            {
                _listBoxItems.SelectedIndex = 0;
            }
        }

        /// <summary>
        ///  Handle a click of the OK button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBtnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void ToolStripCollectionEditor_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            _editor.ShowHelp();
        }

        /// <summary>
        ///  Remove all the selected items.
        /// </summary>
        private void OnBtnRemove_Click(object sender, EventArgs e)
        {
            // Move the selected items into an array so it doesn't change as we remove from it.
            ToolStripItem[] items = new ToolStripItem[_listBoxItems.SelectedItems.Count];
            _listBoxItems.SelectedItems.CopyTo(items, 0);

            // Now remove each of the items.
            for (int i = 0; i < items.Length; i++)
            {
                RemoveItem(items[i]);
            }
        }

        /// <summary>
        ///  Move the selected item down one notch in the list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBtnMoveDown_Click(object sender, EventArgs e)
        {
            ToolStripItem currentItem = (ToolStripItem)_listBoxItems.SelectedItem;
            int currentIndex = _listBoxItems.Items.IndexOf(currentItem);
            MoveItem(currentIndex, ++currentIndex);
            _listBoxItems.SelectedIndex = currentIndex;
        }

        /// <summary>
        ///  Move the selected item up one notch in the list.
        /// </summary>
        private void OnBtnMoveUp_Click(object sender, EventArgs e)
        {
            ToolStripItem currentItem = (ToolStripItem)_listBoxItems.SelectedItem;
            int currentIndex = _listBoxItems.Items.IndexOf(currentItem);
            if (currentIndex > 1)
            {
                MoveItem(currentIndex, --currentIndex);
                _listBoxItems.SelectedIndex = currentIndex;
            }
        }

        /// <summary>
        ///  When we drop the combo, make sure it's wide enough to show the
        ///  full text from all the items.
        /// </summary>
        private void OnNewItemTypes_DropDown(object sender, EventArgs e)
        {
            if (_newItemTypes.Tag is null || !(bool)_newItemTypes.Tag)
            {
                int itemWidth = _newItemTypes.ItemHeight;
                int dropDownHeight = 0;

                // Walk the items and get the widest one.
                using (Graphics g = _newItemTypes.CreateGraphics())
                {
                    foreach (TypeListItem item in _newItemTypes.Items)
                    {
                        itemWidth = (int)Math.Max(itemWidth, _newItemTypes.ItemHeight + 1
                            + g.MeasureString(item.Type.Name, _newItemTypes.Font).Width + GdiPlusExtraSpace);
                        dropDownHeight += (Font.Height + s_separatorHeight) + 2 * s_imagePaddingY;
                    }
                }

                _newItemTypes.DropDownWidth = itemWidth;
                _newItemTypes.DropDownHeight = dropDownHeight;

                // Store that we've already done this work.
                _newItemTypes.Tag = true;
            }
        }

        /// <summary>
        ///  When the user makes an actual selection change, add an item to the ToolStrip.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        // Okay to not catch non-CLS compliant exceptions
        private void OnNewItemTypes_SelectionChangeCommitted(object sender, EventArgs e)
        {
            // Get the item type.
            if (_newItemTypes.SelectedItem is not TypeListItem typeItem)
            {
                return;
            }

            // Create the ToolStripItem.
            ToolStripItem newItem = (ToolStripItem)CreateInstance(typeItem.Type);
            // Set the Image property and DisplayStyle.
            if (newItem is ToolStripButton or ToolStripSplitButton or ToolStripDropDownButton)
            {
                Image image = null;
                try
                {
                    image = new Bitmap(typeof(ToolStripItemEditorForm), "BlankToolstrip.bmp");
                }
                catch (Exception ex)
                {
                    if (ex.IsCriticalException())
                    {
                        throw;
                    }
                }

                PropertyDescriptor imageProperty = TypeDescriptor.GetProperties(newItem)[nameof(Image)];

                if (imageProperty is not null && image is not null)
                {
                    imageProperty.SetValue(newItem, image);
                }

                PropertyDescriptor displayProperty = TypeDescriptor.GetProperties(newItem)["DisplayStyle"];
                displayProperty?.SetValue(newItem, ToolStripItemDisplayStyle.Image);

                PropertyDescriptor imageTransProperty = TypeDescriptor.GetProperties(newItem)["ImageTransparentColor"];
                imageTransProperty?.SetValue(newItem, Color.Magenta);
            }

            // Add it.
            AddItem(newItem, -1);
            _listBoxItems.Focus();
        }

        /// <summary>
        ///  Just invalidate the combo on a selection change.
        /// </summary>
        private void OnNewItemTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            _newItemTypes.Invalidate();
        }

        /// <summary>
        ///  Custom measureItem for the ListBox items...
        /// </summary>
        private void OnListBoxItems_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            int separator = 0;
            if (sender is ComboBox)
            {
                bool drawSeparator = e.Index == _customItemIndex;

                if (e.Index >= 0 && drawSeparator)
                {
                    separator = s_separatorHeight;
                }
            }

            Font measureFont = Font;
            e.ItemHeight = Math.Max(s_iconHeight + separator, measureFont.Height + separator) + 2 * s_imagePaddingY;
        }

        /// <summary>
        ///  Custom draw the list box item with the icon and the text.
        ///  We actually share this code between the list box and the combo box.
        /// </summary>
        private void OnListBoxItems_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index == -1)
            {
                return;
            }

            // Fish out the item  type. we're so cool we can share this code
            // with just this difference.
            Type itemType;
            string itemText;
            bool indentItem = false;
            bool drawSeparator = false;
            bool isComboEditBox = (e.State & DrawItemState.ComboBoxEdit) == DrawItemState.ComboBoxEdit;

            if (sender is ListBox listBox)
            {
                // The list box has the items directly.
                if (listBox.Items[e.Index] is not Component item)
                {
                    Debug.Fail("Unexpected list box item painted!");
                    return;
                }

                if (item is ToolStripItem)
                {
                    indentItem = true;
                }

                itemType = item.GetType();
                itemText = (item.Site is not null) ? item.Site.Name : itemType.Name;
            }
            else if (sender is ComboBox box)
            {
                // The combo box has just the types.
                // Never draw the separator in the edit box, even if it is the selected index.
                drawSeparator = (e.Index == _customItemIndex) && !isComboEditBox;
                if (box.Items[e.Index] is not TypeListItem typeListItem)
                {
                    Debug.Fail("Unexpected combo box item!");
                    return;
                }

                itemType = typeListItem.Type;
                itemText = typeListItem.ToString();
            }
            else
            {
                Debug.Fail("Unexpected sender calling DrawItem");
                return;
            }

            // We've got ourselves an item type.  draw it.
            if (itemType is not null)
            {
                if (drawSeparator)
                {
                    e.Graphics.DrawLine(SystemPens.ControlDark, e.Bounds.X + 2, e.Bounds.Y + 2, e.Bounds.Right - 2, e.Bounds.Y + 2);
                }

                // Get the toolbox bitmap, draw it, and then draw the text.  We just
                // draw the bitmap as a square based on the height of this line item.
                // Calculate the image rect
                Rectangle imageBounds = e.Bounds;
                imageBounds.Size = new(s_iconWidth, s_iconHeight);
                int xOffset = isComboEditBox ? 0 : s_imagePaddingX * 2;
                imageBounds.Offset(xOffset, s_imagePaddingX);

                if (drawSeparator)
                {
                    imageBounds.Offset(0, s_separatorHeight);
                }

                if (indentItem)
                {
                    imageBounds.X += s_iconWidth + s_indentSpacing;
                }

                // Make sure after all this we still are within bounds and are square.
                if (!isComboEditBox)
                {
                    imageBounds.Intersect(e.Bounds);
                }

                // Draw the image if it's there.
                Bitmap tbxBitmap = ToolStripDesignerUtils.GetToolboxBitmap(itemType);
                if (tbxBitmap is not null)
                {
                    if (isComboEditBox)
                    {
                        // Paint the icon of the combo's textbox area.
                        e.Graphics.DrawImage(tbxBitmap, e.Bounds.X, e.Bounds.Y, s_iconWidth, s_iconHeight);
                    }
                    else
                    {
                        e.Graphics.FillRectangle(SystemBrushes.Window, imageBounds);
                        e.Graphics.DrawImage(tbxBitmap, imageBounds);
                    }
                }

                // Calculate the text rect
                Rectangle textBounds = e.Bounds;
                textBounds.X = imageBounds.Right + s_textImageSpacing;
                textBounds.Y = imageBounds.Top - s_imagePaddingY;
                if (!isComboEditBox)
                {
                    textBounds.Y += s_imagePaddingY * 2;
                }

                textBounds.Intersect(e.Bounds);

                // Draw the background as necessary.
                Rectangle fillBounds = e.Bounds;
                fillBounds.X = textBounds.X - 2;

                if (drawSeparator)
                {
                    fillBounds.Y += s_separatorHeight;
                    fillBounds.Height -= s_separatorHeight;
                }

                Color textColor;
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
                    TextRenderer.DrawText(e.Graphics, itemText, Font, textBounds, textColor, format);
                }

                // Finally, draw the focus rect.
                if ((e.State & DrawItemState.Focus) == DrawItemState.Focus)
                {
                    fillBounds.Width -= 1;
                    ControlPaint.DrawFocusRectangle(e.Graphics, fillBounds, e.ForeColor, e.BackColor);
                }
            }
        }

        /// <summary>
        ///  Push the selected items into the property grid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnListBoxItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Push the items into the grid.
            object[] selectedItems = new object[_listBoxItems.SelectedItems.Count];
            if (selectedItems.Length > 0)
            {
                _listBoxItems.SelectedItems.CopyTo(selectedItems, 0);
            }

            // ToolStrip is selected. Remove the items property.
            if (selectedItems.Length == 1 && selectedItems[0] is ToolStrip toolStrip)
            {
                ToolStrip parentStrip = toolStrip;
                if (parentStrip is not null && parentStrip.Site is not null)
                {
                    _toolStripCustomTypeDescriptor ??= new(toolStrip);

                    _selectedItemProps.SelectedObjects = [_toolStripCustomTypeDescriptor];
                }
                else
                {
                    // If null parentStrip or non sited then don't show the properties.
                    _selectedItemProps.SelectedObjects = null;
                }
            }
            else
            {
                _selectedItemProps.SelectedObjects = selectedItems;
            }

            // Enable the up/down button and the remove button based on the items.
            _btnMoveUp.Enabled = (_listBoxItems.SelectedItems.Count == 1) && (_listBoxItems.SelectedIndex > 1);
            _btnMoveDown.Enabled = (_listBoxItems.SelectedItems.Count == 1) && (_listBoxItems.SelectedIndex < _listBoxItems.Items.Count - 1);
            _btnRemove.Enabled = selectedItems.Length > 0;

            // Cannot remove a ToolStrip through this CollectionEditor.
            foreach (object obj in _listBoxItems.SelectedItems)
            {
                if (obj is ToolStrip)
                {
                    _btnRemove.Enabled = _btnMoveUp.Enabled = _btnMoveDown.Enabled = false;
                    break;
                }
            }

            // Invalidate the list box and the label above the grid.
            _listBoxItems.Invalidate();
            _selectedItemName.Invalidate();
        }

        /// <summary>
        ///  Invalidate the <see cref="ListBox"/> and the SelectedItemName Label on top of the propertyGrid.
        /// </summary>
        private void PropertyGrid_propertyValueChanged(object sender, PropertyValueChangedEventArgs e)
        {
            // Invalidate the list box and the label above the grid.
            _listBoxItems.Invalidate();
            _selectedItemName.Invalidate();
        }

        /// <summary>
        ///  Paint the name and type of the currently selected items in the label above the property grid.
        /// </summary>
        private void OnSelectedItemName_Paint(object sender, PaintEventArgs e)
        {
            // Make the bolded font for the type name.
            using Font boldFont = new(_selectedItemName.Font, FontStyle.Bold);
            Label label = sender as Label;
            Rectangle bounds = label.ClientRectangle;
            StringFormat stringFormat = null;

            bool rightToLeft = label.RightToLeft == RightToLeft.Yes;

            stringFormat = rightToLeft ? new(StringFormatFlags.DirectionRightToLeft) : new();

            stringFormat.HotkeyPrefix = Drawing.Text.HotkeyPrefix.Show;

            // Based on the count, just paint the name, (Multiple Items), or (None).
            switch (_listBoxItems.SelectedItems.Count)
            {
                case 1:
                    // For a single item, we paint it's class name in bold, then the item name.
                    Component selectedItem = null;
                    selectedItem = _listBoxItems.SelectedItem is ToolStrip strip ? strip : (ToolStripItem)_listBoxItems.SelectedItem;

                    string className = "&" + selectedItem.GetType().Name;
                    // Erase background
                    e.Graphics.FillRectangle(SystemBrushes.Control, bounds);
                    string itemName = selectedItem.Site?.Name ?? string.Empty;

                    if (label is not null)
                    {
                        label.Text = className + itemName;
                    }

                    int classWidth = 0;
                    classWidth = (int)e.Graphics.MeasureString(className, boldFont).Width;
                    e.Graphics.DrawString(className, boldFont, SystemBrushes.WindowText, bounds, stringFormat);
                    int itemTextWidth = (int)e.Graphics.MeasureString(itemName, _selectedItemName.Font).Width;
                    Rectangle textRect = new(classWidth + GdiPlusExtraSpace, 0, bounds.Width - (classWidth + GdiPlusExtraSpace), bounds.Height);
                    label.AutoEllipsis = itemTextWidth > textRect.Width;

                    TextFormatFlags flags = TextFormatFlags.EndEllipsis | TextFormatFlags.PreserveGraphicsTranslateTransform | TextFormatFlags.PreserveGraphicsClipping;
                    if (rightToLeft)
                    {
                        flags |= TextFormatFlags.RightToLeft;
                    }

                    TextRenderer.DrawText(e.Graphics, itemName, _selectedItemName.Font, textRect, SystemColors.WindowText, flags);

                    break;
                case 0:
                    // Erase background.
                    e.Graphics.FillRectangle(SystemBrushes.Control, bounds);
                    if (label is not null)
                    {
                        label.Text = SR.ToolStripItemCollectionEditorLabelNone;
                    }

                    e.Graphics.DrawString(SR.ToolStripItemCollectionEditorLabelNone, boldFont, SystemBrushes.WindowText, bounds, stringFormat);
                    break;
                default:
                    // Erase background.
                    e.Graphics.FillRectangle(SystemBrushes.Control, bounds);
                    if (label is not null)
                    {
                        label.Text = SR.ToolStripItemCollectionEditorLabelMultipleItems;
                    }

                    e.Graphics.DrawString(SR.ToolStripItemCollectionEditorLabelMultipleItems, boldFont, SystemBrushes.WindowText, bounds, stringFormat);
                    break;
            }

            stringFormat.Dispose();
        }

        /// <summary>
        ///  Removes an item from the list and the preview ToolStrip
        /// </summary>
        /// <param name="item"></param>
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
                _listBoxItems.ClearSelected();
                index = Math.Max(0, Math.Min(index, _listBoxItems.Items.Count - 1));
                _listBoxItems.SelectedIndex = index;
            }
        }

        /// <devdoc>
        ///  Fishes out the ToolStrip from the object - which can be a ToolStrip or a <see cref="ToolStripDropDownItem" />
        /// </devdoc>
        internal static ToolStrip ToolStripFromObject(object instance)
        {
            ToolStrip currentToolStrip = null;

            if (instance is not null)
            {
                currentToolStrip = instance is ToolStripDropDownItem toolStripDropDownItem ? toolStripDropDownItem.DropDown : instance as ToolStrip;
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
                    // Need to add padding for RTL combo box.
                    return RightToLeft == RightToLeft.Yes
                        ? new(4 + SystemInformation.HorizontalScrollBarThumbWidth, 3, s_iconWidth, s_iconHeight)
                        : new(3, 3, s_iconWidth, s_iconHeight);
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
                switch (m.MsgInternal)
                {
                    case PInvokeCore.WM_SETFOCUS:
                    case PInvokeCore.WM_KILLFOCUS:
                        Invalidate(ImageRect);
                        break;
                }
            }
        }

        /// <summary>
        ///  This is a magic collection.  It's job is to keep the preview ToolStrip and the list box in sync and manage both sets of items.
        ///  It contains a list of EditorItem objects, which whole the information for each item, and a reference to the real ToolStripItem component,
        ///  the and the preview ToolStripItem.  The order and contents of this combo box always match that of the real collection, the list box, and the preview ToolStrip.
        ///  It operates generically on three ILists: the list box.Items collection, the previewToolStrip.Items collection, and the actual ToolStripItemCollection being designed.
        /// </summary>
        private class EditorItemCollection : CollectionBase
        {
            // The ListBox.Items collection.
            private readonly IList _listBoxList;
            // The real deal target collection.
            private readonly IList _targetCollectionList;
            // The owner form that created this collection.
            private readonly ToolStripItemEditorForm _owner;

            /// <summary>
            ///  Setup the collection's variables.
            /// </summary>
            internal EditorItemCollection(ToolStripItemEditorForm owner, IList displayList, IList componentList)
            {
                _owner = owner;
                _listBoxList = displayList;
                _targetCollectionList = componentList;
            }

            /// <summary>
            ///  Add a new ToolStrip item.  See OnInsertComplete for the actual add operation.
            /// </summary>
            /// <param name="item"></param>
            public void Add(object item)
            {
                List.Add(new EditorItem(item));
            }

            /// <summary>
            ///  This is a little tricky, since our list doesn't actually contain
            ///  ToolStripItems, but rather EditorItems, we have to walk those.  No bother,
            ///  this list is always pretty short.
            /// </summary>
            public int IndexOf(ToolStripItem item)
            {
                for (int i = 0; i < List.Count; i++)
                {
                    EditorItem editorItem = (EditorItem)List[i]!;
                    if (editorItem.Component == item)
                    {
                        return i;
                    }
                }

                return -1;
            }

            /// <summary>
            ///  Insert an item into the list somewhere.  See OnInsertComplete for the real meaty stuff.
            /// </summary>
            /// <param name="index"></param>
            /// <param name="item"></param>
            public void Insert(int index, ToolStripItem item)
            {
                List.Insert(index, new EditorItem(item));
            }

            /// <summary>
            ///  Move an item from one array position to another.
            /// </summary>
            public void Move(int fromIndex, int toIndex)
            {
                if (toIndex == fromIndex)
                {
                    return;
                }

                EditorItem editorItem = (EditorItem)List[fromIndex]!;
                if (editorItem.Host is not null)
                {
                    return;
                }

                try
                {
                    _owner.Context?.OnComponentChanging();

                    // Yank 'em all outate there.
                    _listBoxList.Remove(editorItem.Component);
                    _targetCollectionList.Remove(editorItem.Component);

                    InnerList.Remove(editorItem);

                    // Put 'em all back in.
                    _listBoxList.Insert(toIndex, editorItem.Component);
                    // Since ToolStrip is also an item of the listBoxItems
                    // Lets decrement the counter by one.

                    // ToolStrip is Always the TOP item
                    // so it is SAFE to assume that
                    // the index that we want is always currentIndex - 1.

                    // This is required as the _targetList doesn't contain the ToolStrip.
                    _targetCollectionList.Insert(toIndex - 1, editorItem.Component);

                    InnerList.Insert(toIndex, editorItem);
                }
                finally
                {
                    _owner.Context?.OnComponentChanged();
                }
            }

            /// <summary>
            ///  Clear has a different semantic than remove.  Clear simply dumps all the items
            ///  out of the list box and the preview and zero's this collection.  Remove is more
            ///  destructive in that it affects the target collection.
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
            ///  After an item is inserted into the collection, we do the work
            ///  to make sure that we sync up the three lists.
            /// </summary>
            /// <param name="index"></param>
            /// <param name="value"></param>
            protected override void OnInsertComplete(int index, object value)
            {
                if (value is null)
                {
                    return;
                }

                EditorItem item = (EditorItem)value;
                if (item.Host is not null)
                {
                    // Insert into the list box.
                    _listBoxList.Insert(index, item.Host);
                    base.OnInsertComplete(index, value);
                    return;
                }

                // Check the target collection first, if it's already there,
                // chill.  Otherwise we need to push it in.  In the case that we're
                // sync'ing to an existing list, we don't want to be re-adding everything.
                if (!_targetCollectionList.Contains(item.Component))
                {
                    try
                    {
                        _owner.Context?.OnComponentChanging();
                        _targetCollectionList.Insert(index - 1, item.Component);
                    }
                    finally
                    {
                        _owner.Context?.OnComponentChanged();
                    }
                }

                // Insert into the list box and into the preview
                _listBoxList.Insert(index, item.Component);
                base.OnInsertComplete(index, value);
            }

            /// <summary>
            ///  Really remove an item from the collections.
            /// </summary>
            protected override void OnRemove(int index, object value)
            {
                object item = List[index];
                if (item is null)
                {
                    return;
                }

                EditorItem editorItem = (EditorItem)item!;

                // Pull it from the list box and preview
                _listBoxList.RemoveAt(index);

                // Yank it from the collection.  The code that calls this
                // collection is responsible for disposing it to destroy
                // it in the designer.
                try
                {
                    _owner.Context?.OnComponentChanging();
                    _targetCollectionList.RemoveAt(index - 1);
                }
                finally
                {
                    _owner.Context?.OnComponentChanged();
                }

                // Finally dispose the editor item which cleanup the preview item.
                editorItem?.Dispose();
                base.OnRemove(index, value);
            }

            /// <summary>
            ///  Remove an item.See OnRemove for details.
            /// </summary>
            /// <param name="item"></param>
            public void Remove(ToolStripItem item)
            {
                int index = IndexOf(item);
                List.RemoveAt(index);
            }

            /// <summary>
            ///  This class keeps track of the mapping between a ToolStrip item in the designer, in the
            ///  preview ToolStrip, and in the list box.
            /// </summary>
            private class EditorItem
            {
                // The real deal item in the designer.
                public ToolStripItem _component;
                public ToolStrip _host;

                internal EditorItem(object componentItem)
                {
                    if (componentItem is ToolStrip toolStrip)
                    {
                        _host = toolStrip;
                    }
                    else
                    {
                        _component = (ToolStripItem)componentItem;
                    }
                }

                /// <summary>
                ///  The item that's actually being created in the designer.
                /// </summary>
                public ToolStripItem Component => _component;

                /// <summary>
                ///  The ToolStrip that's actually being created in the designer.
                /// </summary>
                public ToolStrip Host => _host;

                /// <summary>
                ///  Cleanup our mess.
                /// </summary>
                public void Dispose()
                {
                    GC.SuppressFinalize(this);
                    _component = null;
                }
            }
        }

        private class TypeListItem
        {
            public readonly Type Type;

            public TypeListItem(Type type)
            {
                Type = type;
            }

            public override string ToString()
            {
                return ToolStripDesignerUtils.GetToolboxDescription(Type);
            }
        }
    }
}
