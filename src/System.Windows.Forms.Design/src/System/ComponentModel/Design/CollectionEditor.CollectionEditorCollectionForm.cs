// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace System.ComponentModel.Design;

public partial class CollectionEditor
{
    /// <summary>
    ///  This is the collection editor's default implementation of a collection form.
    /// </summary>
    private class CollectionEditorCollectionForm : CollectionForm
    {
        private const int TextIndent = 1;
        private const int PaintWidth = 20;
        private const int PaintIndent = 26;
        private static readonly double s_log10 = Math.Log(10);

        private List<object>? _createdItems;
        private List<object>? _removedItems;
        private List<object>? _originalItems;

        private readonly CollectionEditor _editor;

        private FilterListBox _listBox;
        private SplitButton _addButton;
        private Button _removeButton;
        private Button _cancelButton;
        private Button _okButton;
        private Button _downButton;
        private Button _upButton;
        private PropertyGrid _propertyGrid;
        private Label _membersLabel;
        private Label _propertiesLabel;
        private TableLayoutPanel _okCancelTableLayoutPanel;
        private TableLayoutPanel _overArchingTableLayoutPanel;
        private TableLayoutPanel _addRemoveTableLayoutPanel;

        private int _suspendEnabledCount;

        private bool _dirty;

        public CollectionEditorCollectionForm(CollectionEditor editor) : base(editor)
        {
            _editor = editor;
            InitializeComponent();
            if (ScaleHelper.IsScalingRequired)
            {
                ScaleButtonImageLogicalToDevice(_downButton);
                ScaleButtonImageLogicalToDevice(_upButton);
            }

            Text = string.Format(SR.CollectionEditorCaption, CollectionItemType.Name);

            HookEvents();

            Type[] newItemTypes = NewItemTypes;
            if (newItemTypes.Length > 1)
            {
                EventHandler addDownMenuClick = new(AddDownMenu_click);
                _addButton.ShowSplit = true;
                ContextMenuStrip addDownMenu = new();
                _addButton.ContextMenuStrip = addDownMenu;
                for (int i = 0; i < newItemTypes.Length; i++)
                {
                    addDownMenu.Items.Add(new TypeMenuItem(newItemTypes[i], addDownMenuClick));
                }
            }

            AdjustListBoxItemHeight();
        }

        private bool IsImmutable
        {
            get
            {
                foreach (ListItem item in _listBox.SelectedItems)
                {
                    Type type = item.Value.GetType();

                    // The type is considered immutable if the converter is defined as requiring a
                    // create instance or all the properties are read-only.
                    if (!TypeDescriptor.GetConverter(type).GetCreateInstanceSupported())
                    {
                        foreach (PropertyDescriptor p in TypeDescriptor.GetProperties(type))
                        {
                            if (!p.IsReadOnly)
                            {
                                return false;
                            }
                        }
                    }
                }

                return true;
            }
        }

        /// <summary>
        ///  Adds a new element to the collection.
        /// </summary>
        private void AddButton_click(object? sender, EventArgs e)
        {
            PerformAdd();
        }

        /// <summary>
        ///  Processes a click of the drop down type menu. This creates a new instance.
        /// </summary>
        private void AddDownMenu_click(object? sender, EventArgs e)
        {
            if (sender is TypeMenuItem typeMenuItem)
            {
                CreateAndAddInstance(typeMenuItem.ItemType);
            }
        }

        /// <summary>
        ///  This Function adds the individual objects to the ListBox.
        /// </summary>
        private void AddItems(IList instances)
        {
            _createdItems ??= [];

            _listBox.BeginUpdate();
            try
            {
                foreach (object? instance in instances)
                {
                    if (instance is not null)
                    {
                        _dirty = true;
                        _createdItems.Add(instance);
                        ListItem created = new(_editor, instance);
                        _listBox.Items.Add(created);
                    }
                }
            }
            finally
            {
                _listBox.EndUpdate();
            }

            if (instances.Count == 1)
            {
                // optimize for the case where we just added one thing...
                UpdateItemWidths(_listBox.Items[^1] as ListItem);
            }
            else
            {
                UpdateItemWidths(null);
            }

            SuspendEnabledUpdates();
            try
            {
                _listBox.ClearSelected();
                _listBox.SelectedIndex = _listBox.Items.Count - 1;

                object[] items = new object[_listBox.Items.Count];
                for (int i = 0; i < items.Length; i++)
                {
                    items[i] = ((ListItem)_listBox.Items[i]).Value;
                }

                Items = items;

                // If someone changes the edit value which resets the selindex, we
                // should keep the new index.
                if (_listBox.Items.Count > 0 && _listBox.SelectedIndex != _listBox.Items.Count - 1)
                {
                    _listBox.ClearSelected();
                    _listBox.SelectedIndex = _listBox.Items.Count - 1;
                }
            }
            finally
            {
                ResumeEnabledUpdates(true);
            }
        }

        private void AdjustListBoxItemHeight()
        {
            _listBox.ItemHeight = Font.Height + SystemInformation.BorderSize.Width * 2;
        }

        /// <summary>
        ///  Determines whether removal of a specific list item should be permitted.
        ///  Used to determine enabled/disabled state of the Remove (X) button.
        ///  Items added after editor was opened may always be removed.
        ///  Items that existed before editor was opened require a call to CanRemoveInstance.
        /// </summary>
        private bool AllowRemoveInstance(object value)
        {
            if (_createdItems is not null && _createdItems.Contains(value))
            {
                return true;
            }

            return CanRemoveInstance(value);
        }

        private int CalcItemWidth(Graphics g, ListItem item)
        {
            int c = Math.Max(2, _listBox.Items.Count);
            SizeF sizeW = g.MeasureString(c.ToString(CultureInfo.CurrentCulture), _listBox.Font);

            int charactersInNumber = ((int)(Math.Log(c - 1) / s_log10) + 1);
            int w = 4 + charactersInNumber * (Font.Height / 2);

            w = Math.Max(w, (int)Math.Ceiling(sizeW.Width));
            w += SystemInformation.BorderSize.Width * 4;

            SizeF size = g.MeasureString(GetDisplayText(item), _listBox.Font);
            int pic = 0;
            if (item.Editor is not null && item.Editor.GetPaintValueSupported())
            {
                pic = PaintWidth + TextIndent;
            }

            return (int)Math.Ceiling(size.Width) + w + pic + SystemInformation.BorderSize.Width * 4;
        }

        /// <summary>
        ///  Aborts changes made in the editor.
        /// </summary>
        private void CancelButton_click(object? sender, EventArgs e)
        {
            try
            {
                _editor.CancelChanges();

                if (!CollectionEditable || !_dirty)
                {
                    return;
                }

                _dirty = false;
                _listBox.Items.Clear();

                if (_createdItems is not null)
                {
                    if (_createdItems is [IComponent { Site: not null }, ..])
                    {
                        // here we bail now because we don't want to do the "undo" manually,
                        // we're part of a transaction, we've added item, the rollback will be
                        // handled by the undo engine because the component in the collection are sited
                        // doing it here kills perf because the undo of the transaction has to roll back the remove and then
                        // rollback the add. This is useless and is only needed for non sited component or other classes
                        return;
                    }

                    object[] items = [.. _createdItems];
                    for (int i = 0; i < items.Length; i++)
                    {
                        DestroyInstance(items[i]);
                    }

                    _createdItems.Clear();
                }

                _removedItems?.Clear();

                // Restore the original contents. Because objects get parented during CreateAndAddInstance, the underlying collection
                // gets changed during add, but not other operations. Not all consumers of this dialog can roll back every single change,
                // but this will at least roll back the additions, removals and reordering. See ASURT #85470.
                if (_originalItems is not null && _originalItems.Count > 0)
                {
                    Items = [.. _originalItems];
                    _originalItems.Clear();
                }
                else
                {
                    Items = [];
                }
            }
            catch (Exception ex)
            {
                DialogResult = DialogResult.None;
                DisplayError(ex);
            }
        }

        /// <summary>
        ///  Performs a create instance and then adds the instance to the list box.
        /// </summary>
        private void CreateAndAddInstance(Type type)
        {
            try
            {
                object instance = CreateInstance(type);
                IList multipleInstance = _editor.GetObjectsFromInstance(instance);

                if (multipleInstance is not null)
                {
                    AddItems(multipleInstance);
                }
            }
            catch (Exception e)
            {
                DisplayError(e);
            }
        }

        /// <summary>
        ///  Moves the selected item down one.
        /// </summary>
        private void DownButton_click(object? sender, EventArgs e)
        {
            try
            {
                SuspendEnabledUpdates();
                _dirty = true;
                int index = _listBox.SelectedIndex;
                if (index == _listBox.Items.Count - 1)
                {
                    return;
                }

                int ti = _listBox.TopIndex;
                (_listBox.Items[index + 1], _listBox.Items[index]) = (_listBox.Items[index], _listBox.Items[index + 1]);

                if (ti < _listBox.Items.Count - 1)
                {
                    _listBox.TopIndex = ti + 1;
                }

                _listBox.ClearSelected();
                _listBox.SelectedIndex = index + 1;

                // enabling/disabling the buttons has moved the focus to the OK button, move it back to the sender
                Control ctrlSender = (Control)sender!;

                if (ctrlSender.Enabled)
                {
                    ctrlSender.Focus();
                }
            }
            finally
            {
                ResumeEnabledUpdates(true);
            }
        }

        private void CollectionEditor_HelpButtonClicked(object? sender, CancelEventArgs e)
        {
            e.Cancel = true;
            _editor.ShowHelp();
        }

        private void Form_HelpRequested(object? sender, HelpEventArgs e)
        {
            _editor.ShowHelp();
        }

        /// <summary>
        ///  Retrieves the display text for the given list item (if any). The item determines its own display text
        ///  through its ToString() method, which delegates to the GetDisplayText() override on the parent CollectionEditor.
        ///  This means in theory that the text can change at any time (ie. its not fixed when the item is added to the list).
        ///  The item returns its display text through ToString() so that the same text will be reported to Accessibility clients.
        /// </summary>
        private static string GetDisplayText(ListItem? item)
        {
            return (item is null) ? string.Empty : item.ToString();
        }

        private void HookEvents()
        {
            _listBox.KeyDown += ListBox_keyDown;
            _listBox.DrawItem += ListBox_drawItem;
            _listBox.SelectedIndexChanged += ListBox_SelectedIndexChanged;
            _listBox.HandleCreated += ListBox_HandleCreated;
            _upButton.Click += UpButton_Click;
            _downButton.Click += DownButton_click;
            _propertyGrid.PropertyValueChanged += PropertyGrid_propertyValueChanged;
            _addButton.Click += AddButton_click;
            _removeButton.Click += RemoveButton_Click;
            _okButton.Click += OKButton_Click;
            _cancelButton.Click += CancelButton_click;
            HelpButtonClicked += CollectionEditor_HelpButtonClicked;
            HelpRequested += Form_HelpRequested;
            Shown += Form_Shown;
        }

        [MemberNotNull(nameof(_membersLabel))]
        [MemberNotNull(nameof(_listBox))]
        [MemberNotNull(nameof(_upButton))]
        [MemberNotNull(nameof(_downButton))]
        [MemberNotNull(nameof(_propertiesLabel))]
        [MemberNotNull(nameof(_propertyGrid))]
        [MemberNotNull(nameof(_addButton))]
        [MemberNotNull(nameof(_removeButton))]
        [MemberNotNull(nameof(_okButton))]
        [MemberNotNull(nameof(_cancelButton))]
        [MemberNotNull(nameof(_okCancelTableLayoutPanel))]
        [MemberNotNull(nameof(_overArchingTableLayoutPanel))]
        [MemberNotNull(nameof(_addRemoveTableLayoutPanel))]
        private void InitializeComponent()
        {
            ComponentResourceManager resources = new(typeof(CollectionEditor));
            _membersLabel = new Label();
            _listBox = new FilterListBox();
            _upButton = new Button();
            _downButton = new Button();
            _propertiesLabel = new Label();
            _propertyGrid = new PropertyGrid();
            _addButton = new SplitButton();
            _removeButton = new Button();
            _okButton = new Button();
            _cancelButton = new Button();
            _okCancelTableLayoutPanel = new TableLayoutPanel();
            _overArchingTableLayoutPanel = new TableLayoutPanel();
            _addRemoveTableLayoutPanel = new TableLayoutPanel();
            _okCancelTableLayoutPanel.SuspendLayout();
            _overArchingTableLayoutPanel.SuspendLayout();
            _addRemoveTableLayoutPanel.SuspendLayout();
            SuspendLayout();

            resources.ApplyResources(_membersLabel, "membersLabel");
            _membersLabel.Margin = new Padding(0, 0, 3, 3);
            _membersLabel.Name = "membersLabel";

            resources.ApplyResources(_listBox, "listbox");
            _listBox.SelectionMode = (CanSelectMultipleInstances() ? SelectionMode.MultiExtended : SelectionMode.One);
            _listBox.DrawMode = DrawMode.OwnerDrawFixed;
            _listBox.FormattingEnabled = true;
            _listBox.Margin = new Padding(0, 3, 3, 3);
            _listBox.Name = "listbox";
            _overArchingTableLayoutPanel.SetRowSpan(_listBox, 2);

            resources.ApplyResources(_upButton, "upButton");
            _upButton.Name = "upButton";

            resources.ApplyResources(_downButton, "downButton");
            _downButton.Name = "downButton";

            resources.ApplyResources(_propertiesLabel, "propertiesLabel");
            _propertiesLabel.AutoEllipsis = true;
            _propertiesLabel.Margin = new Padding(0, 0, 3, 3);
            _propertiesLabel.Name = "propertiesLabel";

            resources.ApplyResources(_propertyGrid, "propertyBrowser");
            _propertyGrid.CommandsVisibleIfAvailable = false;
            _propertyGrid.Margin = new Padding(3, 3, 0, 3);
            _propertyGrid.Name = "propertyBrowser";
            _overArchingTableLayoutPanel.SetRowSpan(_propertyGrid, 3);

            resources.ApplyResources(_addButton, "addButton");
            _addButton.Margin = new Padding(0, 3, 3, 3);
            _addButton.Name = "addButton";

            resources.ApplyResources(_removeButton, "removeButton");
            _removeButton.Margin = new Padding(3, 3, 0, 3);
            _removeButton.Name = "removeButton";

            resources.ApplyResources(_okButton, "okButton");
            _okButton.DialogResult = DialogResult.OK;
            _okButton.Margin = new Padding(0, 3, 3, 0);
            _okButton.Name = "okButton";

            resources.ApplyResources(_cancelButton, "cancelButton");
            _cancelButton.DialogResult = DialogResult.Cancel;
            _cancelButton.Margin = new Padding(3, 3, 0, 0);
            _cancelButton.Name = "cancelButton";

            resources.ApplyResources(_okCancelTableLayoutPanel, "okCancelTableLayoutPanel");
            _overArchingTableLayoutPanel.SetColumnSpan(_okCancelTableLayoutPanel, 3);
            _okCancelTableLayoutPanel.Controls.Add(_okButton, 0, 0);
            _okCancelTableLayoutPanel.Controls.Add(_cancelButton, 1, 0);
            _okCancelTableLayoutPanel.Margin = new Padding(3, 3, 0, 0);
            _okCancelTableLayoutPanel.Name = "okCancelTableLayoutPanel";

            resources.ApplyResources(_overArchingTableLayoutPanel, "overArchingTableLayoutPanel");
            _overArchingTableLayoutPanel.Controls.Add(_downButton, 1, 2);
            _overArchingTableLayoutPanel.Controls.Add(_addRemoveTableLayoutPanel, 0, 3);
            _overArchingTableLayoutPanel.Controls.Add(_propertiesLabel, 2, 0);
            _overArchingTableLayoutPanel.Controls.Add(_membersLabel, 0, 0);
            _overArchingTableLayoutPanel.Controls.Add(_listBox, 0, 1);
            _overArchingTableLayoutPanel.Controls.Add(_propertyGrid, 2, 1);
            _overArchingTableLayoutPanel.Controls.Add(_okCancelTableLayoutPanel, 0, 4);
            _overArchingTableLayoutPanel.Controls.Add(_upButton, 1, 1);
            _overArchingTableLayoutPanel.Name = "overArchingTableLayoutPanel";

            resources.ApplyResources(_addRemoveTableLayoutPanel, "addRemoveTableLayoutPanel");
            _addRemoveTableLayoutPanel.Controls.Add(_addButton, 0, 0);
            _addRemoveTableLayoutPanel.Controls.Add(_removeButton, 2, 0);
            _addRemoveTableLayoutPanel.Margin = new Padding(0, 3, 3, 3);
            _addRemoveTableLayoutPanel.Name = "addRemoveTableLayoutPanel";

            AcceptButton = _okButton;
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = _cancelButton;
            Controls.Add(_overArchingTableLayoutPanel);
            HelpButton = true;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "CollectionEditor";
            ShowIcon = false;
            ShowInTaskbar = false;
            _okCancelTableLayoutPanel.ResumeLayout(false);
            _okCancelTableLayoutPanel.PerformLayout();
            _overArchingTableLayoutPanel.ResumeLayout(false);
            _overArchingTableLayoutPanel.PerformLayout();
            _addRemoveTableLayoutPanel.ResumeLayout(false);
            _addRemoveTableLayoutPanel.PerformLayout();
            ResumeLayout(false);
        }

        private void UpdateItemWidths(ListItem? item)
        {
            if (!_listBox.IsHandleCreated)
            {
                return;
            }

            using Graphics g = _listBox.CreateGraphics();
            int old = _listBox.HorizontalExtent;

            if (item is not null)
            {
                int w = CalcItemWidth(g, item);
                if (w > old)
                {
                    _listBox.HorizontalExtent = w;
                }
            }
            else
            {
                int max = 0;
                foreach (ListItem i in _listBox.Items)
                {
                    int w = CalcItemWidth(g, i);
                    if (w > max)
                    {
                        max = w;
                    }
                }

                _listBox.HorizontalExtent = max;
            }
        }

        /// <summary>
        ///  This draws a row of the listBox.
        /// </summary>
        private void ListBox_drawItem(object? sender, DrawItemEventArgs e)
        {
            if (e.Index != -1)
            {
                ListItem item = (ListItem)_listBox.Items[e.Index];

                Graphics g = e.Graphics;

                int c = _listBox.Items.Count;
                int maxC = (c > 1) ? c - 1 : c;
                // We add the +4 is a fudge factor...
                SizeF sizeW = g.MeasureString(maxC.ToString(CultureInfo.CurrentCulture), _listBox.Font);

                int charactersInNumber = ((int)(Math.Log(maxC) / s_log10) + 1); // Luckily, this is never called if count = 0
                int w = 4 + charactersInNumber * (Font.Height / 2);

                w = Math.Max(w, (int)Math.Ceiling(sizeW.Width));
                w += SystemInformation.BorderSize.Width * 4;

                Rectangle button = e.Bounds with { Width = w };

                ControlPaint.DrawButton(g, button, ButtonState.Normal);
                button.Inflate(-SystemInformation.BorderSize.Width * 2, -SystemInformation.BorderSize.Height * 2);

                int offset = w;

                Color backColor = SystemColors.Window;
                Color textColor = SystemColors.WindowText;
                if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                {
                    backColor = SystemColors.Highlight;
                    textColor = SystemColors.HighlightText;
                }

                Rectangle res = e.Bounds with { X = e.Bounds.X + offset, Width = e.Bounds.Width - offset };
                g.FillRectangle(new SolidBrush(backColor), res);
                if ((e.State & DrawItemState.Focus) == DrawItemState.Focus)
                {
                    ControlPaint.DrawFocusRectangle(g, res);
                }

                offset += 2;

                if (item.Editor is not null && item.Editor.GetPaintValueSupported())
                {
                    Rectangle baseVar = new(e.Bounds.X + offset, e.Bounds.Y + 1, PaintWidth, e.Bounds.Height - 3);
                    g.DrawRectangle(SystemPens.ControlText, baseVar.X, baseVar.Y, baseVar.Width - 1, baseVar.Height - 1);
                    baseVar.Inflate(-1, -1);
                    item.Editor.PaintValue(item.Value, g, baseVar);
                    offset += PaintIndent + TextIndent;
                }

                using (StringFormat format = new())
                {
                    format.Alignment = StringAlignment.Center;
                    g.DrawString(e.Index.ToString(CultureInfo.CurrentCulture), Font, SystemBrushes.ControlText,
                        e.Bounds with { Width = w }, format);
                }

                string itemText = GetDisplayText(item);

                using (Brush textBrush = new SolidBrush(textColor))
                {
                    g.DrawString(itemText, Font, textBrush,
                        e.Bounds with { X = e.Bounds.X + offset, Width = e.Bounds.Width - offset });
                }

                // Check to see if we need to change the horizontal extent of the listBox
                int width = offset + (int)g.MeasureString(itemText, Font).Width;
                if (width > e.Bounds.Width && _listBox.HorizontalExtent < width)
                {
                    _listBox.HorizontalExtent = width;
                }
            }
        }

        /// <summary>
        ///  Handles keypress events for the list box.
        /// </summary>
        private void ListBox_keyDown(object? sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Delete:
                    PerformRemove();
                    break;
                case Keys.Insert:
                    PerformAdd();
                    break;
            }
        }

        /// <summary>
        ///  Event that fires when the selected list box index changes.
        /// </summary>
        private void ListBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            UpdateEnabled();
        }

        /// <summary>
        ///  Event that fires when the list box's window handle is created.
        /// </summary>
        private void ListBox_HandleCreated(object? sender, EventArgs e)
        {
            UpdateItemWidths(null);
        }

        /// <summary>
        ///  Commits the changes to the editor.
        /// </summary>
        private void OKButton_Click(object? sender, EventArgs e)
        {
            try
            {
                if (!_dirty || !CollectionEditable)
                {
                    _dirty = false;
                    DialogResult = DialogResult.Cancel;
                    return;
                }

                if (_dirty)
                {
                    object[] items = new object[_listBox.Items.Count];
                    for (int i = 0; i < items.Length; i++)
                    {
                        items[i] = ((ListItem)_listBox.Items[i]).Value;
                    }

                    Items = items;
                }

                if (_removedItems is not null && _dirty)
                {
                    object[] deadItems = [.. _removedItems];

                    for (int i = 0; i < deadItems.Length; i++)
                    {
                        DestroyInstance(deadItems[i]);
                    }

                    _removedItems.Clear();
                }

                _createdItems?.Clear();
                _originalItems?.Clear();

                _listBox.Items.Clear();
                _dirty = false;
            }
            catch (Exception ex)
            {
                DialogResult = DialogResult.None;
                DisplayError(ex);
            }
        }

        /// <summary>
        ///  Reflect any change events to the instance object
        /// </summary>
        private void OnComponentChanged(object? sender, ComponentChangedEventArgs e)
        {
            // see if this is any of the items in our list...this can happen if we launched a child editor
            if (!_dirty && _originalItems is not null)
            {
                foreach (object? item in _originalItems)
                {
                    if (item == e.Component)
                    {
                        _dirty = true;
                        break;
                    }
                }
            }
        }

        /// <summary>
        ///  This is called when the value property in the CollectionForm has changed.
        ///  In it you should update your user interface to reflect the current value.
        /// </summary>
        protected override void OnEditValueChanged()
        {
            if (!Visible)
            {
                return;
            }

            // Remember these contents for cancellation
            _originalItems ??= [];

            _originalItems.Clear();

            // Now update the list box.
            _listBox.Items.Clear();
            _propertyGrid.Site = new PropertyGridSite(Context, _propertyGrid);
            if (EditValue is not null)
            {
                SuspendEnabledUpdates();
                try
                {
                    object[] items = Items;
                    for (int i = 0; i < items.Length; i++)
                    {
                        _listBox.Items.Add(new ListItem(_editor, items[i]));
                        _originalItems.Add(items[i]);
                    }

                    if (_listBox.Items.Count > 0)
                    {
                        _listBox.SelectedIndex = 0;
                    }
                }
                finally
                {
                    ResumeEnabledUpdates(true);
                }
            }
            else
            {
                UpdateEnabled();
            }

            AdjustListBoxItemHeight();
            UpdateItemWidths(null);
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            AdjustListBoxItemHeight();
        }

        /// <summary>
        ///  Performs the actual add of new items. This is invoked by the add button
        ///  as well as the insert key on the list box.
        /// </summary>
        private void PerformAdd()
        {
            CreateAndAddInstance(NewItemTypes[0]);
        }

        /// <summary>
        ///  Performs a remove by deleting all items currently selected in the list box.
        ///  This is called by the delete button as well as the delete key on the list box.
        /// </summary>
        private void PerformRemove()
        {
            int index = _listBox.SelectedIndex;

            if (index != -1)
            {
                SuspendEnabledUpdates();
                try
                {
                    if (_listBox.SelectedItems.Count > 1)
                    {
                        List<ListItem> toBeDeleted = [.._listBox.SelectedItems.Cast<ListItem>()];
                        foreach (ListItem item in toBeDeleted)
                        {
                            RemoveInternal(item);
                        }
                    }
                    else
                    {
                        RemoveInternal((ListItem?)_listBox.SelectedItem);
                    }

                    if (index < _listBox.Items.Count)
                    {
                        _listBox.SelectedIndex = index;
                    }
                    else if (_listBox.Items.Count > 0)
                    {
                        _listBox.SelectedIndex = _listBox.Items.Count - 1;
                    }
                }
                finally
                {
                    ResumeEnabledUpdates(true);
                }
            }
        }

        /// <summary>
        ///  When something in the properties window changes, we update pertinent text here.
        /// </summary>
        private void PropertyGrid_propertyValueChanged(object? sender, PropertyValueChangedEventArgs e)
        {
            _dirty = true;

            // Refresh selected listBox item so that it picks up any name change
            SuspendEnabledUpdates();
            try
            {
                int selectedItem = _listBox.SelectedIndex;
                if (selectedItem >= 0)
                {
                    _listBox.RefreshItem(_listBox.SelectedIndex);
                }
            }
            finally
            {
                ResumeEnabledUpdates(false);
            }

            // if a property changes, invalidate the grid in case it affects the item's name.
            UpdateItemWidths(null);
            _listBox.Invalidate();

            // also update the string above the grid.
            _propertiesLabel.Text = string.Format(SR.CollectionEditorProperties, GetDisplayText((ListItem?)_listBox.SelectedItem));
        }

        /// <summary>
        ///  Used to actually remove the items, one by one.
        /// </summary>
        private void RemoveInternal(ListItem? item)
        {
            if (item is not null)
            {
                _editor.OnItemRemoving(item.Value);

                _dirty = true;

                if (_createdItems is not null && _createdItems.Contains(item.Value))
                {
                    DestroyInstance(item.Value);
                    _createdItems.Remove(item.Value);
                    _listBox.Items.Remove(item);
                }
                else
                {
                    try
                    {
                        if (CanRemoveInstance(item.Value))
                        {
                            _removedItems ??= [];

                            _removedItems.Add(item.Value);
                            _listBox.Items.Remove(item);
                        }
                        else
                        {
                            throw new InvalidOperationException(string.Format(SR.CollectionEditorCantRemoveItem, GetDisplayText(item)));
                        }
                    }
                    catch (Exception ex)
                    {
                        DisplayError(ex);
                    }
                }

                UpdateItemWidths(null);
            }
        }

        /// <summary>
        ///  Removes the selected item.
        /// </summary>
        private void RemoveButton_Click(object? sender, EventArgs e)
        {
            PerformRemove();

            // enabling/disabling the buttons has moved the focus to the OK button, move it back to the sender
            Control ctrlSender = (Control)sender!;
            if (ctrlSender.Enabled)
            {
                ctrlSender.Focus();
            }
        }

        /// <summary>
        ///  used to prevent flicker when playing with the list box selection call resume when done.
        ///  Calls to UpdateEnabled will return silently until Resume is called
        /// </summary>
        private void ResumeEnabledUpdates(bool updateNow)
        {
            _suspendEnabledCount--;

            Debug.Assert(_suspendEnabledCount >= 0, "Mismatch suspend/resume enabled");

            if (updateNow)
            {
                UpdateEnabled();
            }
            else
            {
                BeginInvoke(new MethodInvoker(UpdateEnabled));
            }
        }

        /// <summary>
        ///  Used to prevent flicker when playing with the list box selection call resume when done.
        ///  Calls to UpdateEnabled will return silently until Resume is called
        /// </summary>
        private void SuspendEnabledUpdates() => _suspendEnabledCount++;

        /// <summary>
        ///  Called to show the dialog via the IWindowsFormsEditorService
        /// </summary>
        protected internal override DialogResult ShowEditorDialog(IWindowsFormsEditorService edSvc)
        {
            IComponentChangeService? changeService = _editor.Context?.GetService<IComponentChangeService>();
            DialogResult result = DialogResult.OK;
            try
            {
                if (changeService is not null)
                {
                    changeService.ComponentChanged += OnComponentChanged;
                }

                // This is cached across requests, so reset the initial focus.
                ActiveControl = _listBox;
                result = base.ShowEditorDialog(edSvc);
            }
            finally
            {
                if (changeService is not null)
                {
                    changeService.ComponentChanged -= OnComponentChanged;
                }
            }

            return result;
        }

        /// <summary>
        ///  Moves an item up one in the list box.
        /// </summary>
        private void UpButton_Click(object? sender, EventArgs e)
        {
            int index = _listBox.SelectedIndex;
            if (index == 0)
            {
                return;
            }

            _dirty = true;
            try
            {
                SuspendEnabledUpdates();
                int ti = _listBox.TopIndex;
                (_listBox.Items[index - 1], _listBox.Items[index]) = (_listBox.Items[index], _listBox.Items[index - 1]);

                if (ti > 0)
                {
                    _listBox.TopIndex = ti - 1;
                }

                _listBox.ClearSelected();
                _listBox.SelectedIndex = index - 1;

                // enabling/disabling the buttons has moved the focus to the OK button, move it back to the sender
                Control ctrlSender = (Control)sender!;

                if (ctrlSender.Enabled)
                {
                    ctrlSender.Focus();
                }
            }
            finally
            {
                ResumeEnabledUpdates(true);
            }
        }

        /// <summary>
        ///  Updates the set of enabled buttons.
        /// </summary>
        private void UpdateEnabled()
        {
            if (_suspendEnabledCount > 0)
            {
                // We're in the midst of a suspend/resume block Resume should call us back.
                return;
            }

            bool editEnabled = (_listBox.SelectedItem is not null) && CollectionEditable;
            _removeButton.Enabled = editEnabled && AllowRemoveInstance(((ListItem)_listBox.SelectedItem!).Value);
            _upButton.Enabled = editEnabled && _listBox.Items.Count > 1;
            _downButton.Enabled = editEnabled && _listBox.Items.Count > 1;
            _propertyGrid.Enabled = editEnabled;
            _addButton.Enabled = CollectionEditable;

            if (_listBox.SelectedItem is not null)
            {
                object[] items;

                // If we are to create new instances from the items, then we must wrap them in an outer object.
                // otherwise, the user will be presented with a batch of read only properties, which isn't terribly useful.
                if (IsImmutable)
                {
                    items = [new SelectionWrapper(CollectionType, CollectionItemType, _listBox, _listBox.SelectedItems)];
                }
                else
                {
                    items = new object[_listBox.SelectedItems.Count];
                    for (int i = 0; i < items.Length; i++)
                    {
                        items[i] = ((ListItem)_listBox.SelectedItems[i]!).Value;
                    }
                }

                int selectedItemCount = _listBox.SelectedItems.Count;
                if (selectedItemCount is 1 or -1)
                {
                    // handle both single select listBoxes and a single item selected in a multi-select listBox
                    _propertiesLabel.Text = string.Format(SR.CollectionEditorProperties, GetDisplayText((ListItem)_listBox.SelectedItem));
                }
                else
                {
                    _propertiesLabel.Text = SR.CollectionEditorPropertiesMultiSelect;
                }

                if (_editor.IsAnyObjectInheritedReadOnly(items))
                {
                    _propertyGrid.SelectedObjects = null;
                    _propertyGrid.Enabled = false;
                    _removeButton.Enabled = false;
                    _upButton.Enabled = false;
                    _downButton.Enabled = false;
                    _propertiesLabel.Text = SR.CollectionEditorInheritedReadOnlySelection;
                }
                else
                {
                    _propertyGrid.Enabled = true;
                    _propertyGrid.SelectedObjects = items;
                }
            }
            else
            {
                _propertiesLabel.Text = SR.CollectionEditorPropertiesNone;
                _propertyGrid.SelectedObject = null;
            }
        }

        /// <summary>
        ///  When the form is first shown, update controls due to the edit value changes which happened when the form is invisible.
        /// </summary>
        private void Form_Shown(object? sender, EventArgs e)
        {
            OnEditValueChanged();
        }

        /// <summary>
        ///  Create a new button bitmap scaled for the device units.
        ///  Note: original image might be disposed.
        /// </summary>
        /// <param name="button">button with an image, image size is defined in logical units</param>
        private static void ScaleButtonImageLogicalToDevice(Button? button)
        {
            if (button?.Image is not Bitmap buttonBitmap)
            {
                return;
            }

            button.Image = ScaleHelper.ScaleToDpi(buttonBitmap, ScaleHelper.InitialSystemDpi, disposeBitmap: true);
        }

        /// <summary>
        ///  This class implements a custom type descriptor that is used to provide
        ///  properties for the set of selected items in the collection editor.
        ///  It provides a single property that is equivalent to the editor's collection item type.
        /// </summary>
        private class SelectionWrapper : PropertyDescriptor, ICustomTypeDescriptor
        {
            private readonly Control _control;
            private readonly ICollection _collection;
            private readonly PropertyDescriptorCollection _properties;
            private object? _value;

            public SelectionWrapper(Type collectionType, Type collectionItemType, Control control, ICollection collection)
                : base("Value", [new CategoryAttribute(collectionItemType.Name)])
            {
                ComponentType = collectionType;
                PropertyType = collectionItemType;
                _control = control;
                _collection = collection;
                _properties = new PropertyDescriptorCollection([this]);

                Debug.Assert(collection.Count > 0, "We should only be wrapped if there is a selection");
                _value = this;

                // In a multiselect case, see if the values are different. If so, NULL our value to represent indeterminate.
                foreach (ListItem li in collection)
                {
                    if (_value == this)
                    {
                        _value = li.Value;
                    }
                    else
                    {
                        object? nextValue = li.Value;
                        if (_value is not null)
                        {
                            if (nextValue is null)
                            {
                                _value = null;
                                break;
                            }
                            else
                            {
                                if (!_value.Equals(nextValue))
                                {
                                    _value = null;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            if (nextValue is not null)
                            {
                                _value = null;
                                break;
                            }
                        }
                    }
                }
            }

            /// <summary>
            ///  When overridden in a derived class, gets the type of the component this property is bound to.
            /// </summary>
            public override Type ComponentType { get; }

            /// <summary>
            ///  When overridden in a derived class, gets a value indicating whether this property is read-only.
            /// </summary>
            public override bool IsReadOnly => false;

            /// <summary>
            ///  When overridden in a derived class, gets the type of the property.
            /// </summary>
            public override Type PropertyType { get; }

            /// <summary>
            ///  When overridden in a derived class, indicates whether resetting the <paramref name="component"/>
            ///  will change the value of the <paramref name="component"/>.
            /// </summary>
            public override bool CanResetValue(object component) => false;

            /// <summary>
            ///  When overridden in a derived class, gets the current value of the property on a component.
            /// </summary>
            public override object? GetValue(object? component) => _value;

            /// <summary>
            ///  When overridden in a derived class, resets the value for this property of the component.
            /// </summary>
            public override void ResetValue(object component)
            {
            }

            /// <summary>
            ///  When overridden in a derived class, sets the value of the component to a different value.
            /// </summary>
            public override void SetValue(object? component, object? value)
            {
                _value = value;

                foreach (ListItem li in _collection)
                {
                    li.Value = value!;
                }

                _control.Invalidate();
                OnValueChanged(component, EventArgs.Empty);
            }

            /// <summary>
            ///  When overridden in a derived class, indicates whether the value of this property needs to be persisted.
            /// </summary>
            public override bool ShouldSerializeValue(object component) => false;

            /// <summary>
            ///  Retrieves an array of member attributes for the given object.
            /// </summary>
            AttributeCollection ICustomTypeDescriptor.GetAttributes() => TypeDescriptor.GetAttributes(PropertyType);

            /// <summary>
            ///  Retrieves the class name for this object. If null is returned, the type name is used.
            /// </summary>
            string ICustomTypeDescriptor.GetClassName() => PropertyType.Name;

            /// <summary>
            ///  Retrieves the name for this object. If null is returned, the default is used.
            /// </summary>
            string? ICustomTypeDescriptor.GetComponentName() => null;

            /// <summary>
            ///  Retrieves the type converter for this object.
            /// </summary>
            [RequiresUnreferencedCode("Generic TypeConverters may require the generic types to be annotated. For example, NullableConverter requires the underlying type to be DynamicallyAccessedMembers All.")]
            TypeConverter? ICustomTypeDescriptor.GetConverter() => null;

            /// <summary>
            ///  Retrieves the default event.
            /// </summary>
            EventDescriptor? ICustomTypeDescriptor.GetDefaultEvent() => null;

            /// <summary>
            ///  Retrieves the default property.
            /// </summary>
            PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty() => this;

            /// <summary>
            ///  Retrieves the an editor for this object.
            /// </summary>
            [RequiresUnreferencedCode("Design-time attributes are not preserved when trimming. Types referenced by attributes like EditorAttribute and DesignerAttribute may not be available after trimming.")]
            object? ICustomTypeDescriptor.GetEditor(Type editorBaseType) => null;

            /// <summary>
            ///  Retrieves an array of events that the given component instance provides.
            ///  This may differ from the set of events the class provides.
            ///  If the component is sited, the site may add or remove additional events.
            /// </summary>
            EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
            {
                return EventDescriptorCollection.Empty;
            }

            /// <summary>
            ///  Retrieves an array of events that the given component instance provides.
            ///  This may differ from the set of events the class provides.
            ///  If the component is sited, the site may add or remove additional events.
            ///  The returned array of events will be filtered by the given set of attributes.
            /// </summary>
            EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[]? attributes) => EventDescriptorCollection.Empty;

            /// <summary>
            ///  Retrieves an array of properties that the given component instance provides.
            ///  This may differ from the set of properties the class provides.
            ///  If the component is sited, the site may add or remove additional properties.
            /// </summary>
            [RequiresUnreferencedCode("PropertyDescriptor's PropertyType cannot be statically discovered.")]
            PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties() => _properties;

            /// <summary>
            ///  Retrieves an array of properties that the given component instance provides.
            ///  This may differ from the set of properties the class provides.
            ///  If the component is sited, the site may add or remove additional properties.
            ///  The returned array of properties will be filtered by the given set of attributes.
            /// </summary>
            [RequiresUnreferencedCode("PropertyDescriptor's PropertyType cannot be statically discovered. The public parameterless constructor or the 'Default' static field may be trimmed from the Attribute's Type.")]
            PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[]? attributes) => _properties;

            /// <summary>
            ///  Retrieves the object that directly depends on this value being edited.
            ///  This is generally the object that is required for the PropertyDescriptor's GetValue and SetValue methods.
            ///  If 'null' is passed for the PropertyDescriptor, the ICustomComponent descriptor implementation should
            ///  return the default object, that is the main object that exposes the properties and attributes.
            /// </summary>
            object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor? pd) => this;
        }

        /// <summary>
        ///  This is a single entry in our list box. It contains the value we're editing as well
        ///  as accessors for the type converter and UI editor.
        /// </summary>
        private class ListItem
        {
            private object _value;
            private object? _uiTypeEditor;
            private readonly CollectionEditor _parentCollectionEditor;

            public ListItem(CollectionEditor parentCollectionEditor, object value)
            {
                _value = value;
                _parentCollectionEditor = parentCollectionEditor;
            }

            public override string ToString() => _parentCollectionEditor.GetDisplayText(_value);

            public UITypeEditor? Editor
            {
                get
                {
                    if (_uiTypeEditor is null)
                    {
                        _uiTypeEditor = TypeDescriptor.GetEditor(_value, typeof(UITypeEditor));
                        _uiTypeEditor ??= this;
                    }

                    if (_uiTypeEditor != this)
                    {
                        return (UITypeEditor)_uiTypeEditor;
                    }

                    return null;
                }
            }

            public object Value
            {
                get => _value;
                set
                {
                    _uiTypeEditor = null;
                    _value = value;
                }
            }
        }

        /// <summary>
        ///  Menu items we attach to the drop down menu if there are multiple types the collection editor can create.
        /// </summary>
        private class TypeMenuItem : ToolStripMenuItem
        {
            public TypeMenuItem(Type itemType, EventHandler handler) : base(itemType.Name, null, handler)
            {
                ItemType = itemType;
            }

            public Type ItemType { get; }
        }
    }
}
