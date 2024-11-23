// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace System.Windows.Forms.Design;

internal class DataGridViewColumnCollectionDialog : Form
{
    private Label _selectedColumnsLabel;

    private ListBox _selectedColumns;
    private Button _moveUp;
    private Button _moveDown;
    private Button _deleteButton;
    private Button _addButton;

    private Label _propertyGridLabel;

    private PropertyGrid _propertyGrid1;
    private TableLayoutPanel _okCancelTableLayoutPanel;

    private Button _okButton;

    private Button _cancelButton;

    private DataGridView? _liveDataGridView;

    private IComponentChangeService? _compChangeService;

    private readonly DataGridView _dataGridViewPrivateCopy;
    private readonly DataGridViewColumnCollection _columnsPrivateCopy;
    private readonly Dictionary<DataGridViewColumn, string?> _columnsNames = [];
    private DataGridViewAddColumnDialog? _addColumnDialog;

    private const int OWNERDRAWHORIZONTALBUFFER = 3;
    private const int OWNERDRAWVERTICALBUFFER = 4;
    private const int OWNERDRAWITEMIMAGEBUFFER = 2;

    // static because we can only have one instance of the DataGridViewColumnCollectionDialog running at a time
    private static Bitmap? s_selectedColumnsItemBitmap;

    private bool _columnCollectionChanging;

    private bool _formIsDirty;
    private TableLayoutPanel? _overarchingTableLayoutPanel;
    private TableLayoutPanel? _addRemoveTableLayoutPanel;
    private readonly HashSet<DataGridViewColumn> _userAddedColumns = [];

    internal DataGridViewColumnCollectionDialog()
    {
        // Required for Windows Form Designer support
        InitializeComponent();

        if (_moveUp.Image is Bitmap moveUp)
        {
            _moveUp.Image = ScaleHelper.ScaleToDpi(moveUp, ScaleHelper.InitialSystemDpi, disposeBitmap: true);
        }

        if (_moveDown.Image is Bitmap moveDown)
        {
            _moveDown.Image = ScaleHelper.ScaleToDpi(moveDown, ScaleHelper.InitialSystemDpi, disposeBitmap: true);
        }

        _dataGridViewPrivateCopy = new DataGridView();
        _columnsPrivateCopy = _dataGridViewPrivateCopy.Columns;
        _columnsPrivateCopy.CollectionChanged += columnsPrivateCopy_CollectionChanged;
    }

    private static Bitmap SelectedColumnsItemBitmap
    {
        get
        {
            if (s_selectedColumnsItemBitmap is null)
            {
                s_selectedColumnsItemBitmap = new Bitmap(
                    BitmapSelector.GetResourceStream(typeof(DataGridViewColumnCollectionDialog), "DataGridViewColumnsDialog.selectedColumns.bmp")
                    ?? throw new InvalidOperationException());
                s_selectedColumnsItemBitmap.MakeTransparent(Color.Red);
            }

            return s_selectedColumnsItemBitmap;
        }
    }

    private void columnsPrivateCopy_CollectionChanged(object? sender, CollectionChangeEventArgs e)
    {
        if (_columnCollectionChanging)
        {
            return;
        }

        PopulateSelectedColumns();

        if (e.Action == CollectionChangeAction.Add)
        {
            _selectedColumns.SelectedIndex = _columnsPrivateCopy.IndexOf((DataGridViewColumn)e.Element!);
            ListBoxItem lbi = (ListBoxItem)_selectedColumns.SelectedItem!;
            _userAddedColumns.Add(lbi.DataGridViewColumn);
            _columnsNames[lbi.DataGridViewColumn] = lbi.DataGridViewColumn.Name;
        }

        _formIsDirty = true;
    }

    private void ColumnTypeChanged(ListBoxItem item, Type newType)
    {
        DataGridViewColumn currentColumn = item.DataGridViewColumn;
        Debug.Assert(typeof(DataGridViewColumn).IsAssignableFrom(newType), "we should only have types that can be assigned to a DataGridViewColumn");
        Debug.Assert(_selectedColumns.SelectedItem == item, "we must have lost track of what item is in the property grid");

        DataGridViewColumn newColumn = (DataGridViewColumn)Activator.CreateInstance(newType)!;

        ITypeResolutionService? tr = _liveDataGridView?.Site?.GetService<ITypeResolutionService>();
        ComponentDesigner newColumnDesigner = DataGridViewAddColumnDialog.GetComponentDesignerForType(tr, newType)!;

        CopyDataGridViewColumnProperties(srcColumn: currentColumn, destColumn: newColumn);
        CopyDataGridViewColumnState(srcColumn: currentColumn, destColumn: newColumn);

        _columnCollectionChanging = true;
        int selectedIndex = _selectedColumns.SelectedIndex;

        // steal the focus away from the PropertyGrid
        _selectedColumns.Focus();
        ActiveControl = _selectedColumns;

        try
        {
            // scrub the TypeDescriptor associations
            ListBoxItem listBoxItem = (ListBoxItem)_selectedColumns.SelectedItem;

            _columnsNames.Remove(listBoxItem.DataGridViewColumn, out string? columnSiteName);

            bool userAddedColumn = _userAddedColumns.Remove(listBoxItem.DataGridViewColumn);

            if (listBoxItem.DataGridViewColumnDesigner is not null)
            {
                TypeDescriptor.RemoveAssociation(listBoxItem.DataGridViewColumn, listBoxItem.DataGridViewColumnDesigner);
            }

            _selectedColumns.Items.RemoveAt(selectedIndex);
            _selectedColumns.Items.Insert(selectedIndex, new ListBoxItem(newColumn!, this, newColumnDesigner));

            _columnsPrivateCopy.RemoveAt(selectedIndex);
            // wipe out the display index
            newColumn.DisplayIndex = -1;
            _columnsPrivateCopy.Insert(selectedIndex, newColumn);

            if (!string.IsNullOrEmpty(columnSiteName))
            {
                _columnsNames[newColumn] = columnSiteName;
            }

            if (userAddedColumn)
            {
                _userAddedColumns.Add(newColumn);
            }

            // properties like DataGridViewColumn::Frozen are dependent on the DisplayIndex
            FixColumnCollectionDisplayIndices();

            _selectedColumns.SelectedIndex = selectedIndex;
            _propertyGrid1.SelectedObject = _selectedColumns.SelectedItem;
        }
        finally
        {
            _columnCollectionChanging = false;
        }
    }

    private void CommitChanges()
    {
        if (!_formIsDirty)
        {
            return;
        }

        try
        {
            IComponentChangeService? changeService = _liveDataGridView!.Site?.GetService<IComponentChangeService>();
            PropertyDescriptor? prop = TypeDescriptor.GetProperties(_liveDataGridView)["Columns"];
            IContainer? currentContainer = _liveDataGridView.Site?.Container;

            // Here is the order in which we should do the ComponentChanging/ComponentChanged
            // Container.RemoveComponent, Container.AddComponent
            //
            // 1. OnComponentChanging DataGridView.Columns
            // 2. DataGridView.Columns.Clear();
            // 3. OnComponentChanged DataGridView.Columns
            // 4. IContainer.Remove(dataGridView.Columns)
            // 5. IContainer.Add(new dataGridView.Columns)
            // 6. OnComponentChanging DataGridView.Columns
            // 7. DataGridView.Columns.Add( new DataGridViewColumns)
            // 8. OnComponentChanged DataGridView.Columns

            DataGridViewColumn[] oldColumns = new DataGridViewColumn[_liveDataGridView.Columns.Count];
            _liveDataGridView.Columns.CopyTo(oldColumns, 0);

            // 1. OnComponentChanging DataGridView.Columns
            changeService?.OnComponentChanging(_liveDataGridView, prop);

            // 2. DataGridView.Columns.Clear();
            _liveDataGridView.Columns.Clear();

            // 3. OnComponentChanged DataGridView.Columns
            changeService?.OnComponentChanged(_liveDataGridView, prop, null, null);

            // 4. IContainer.Remove(dataGridView.Columns)
            if (currentContainer is not null)
            {
                for (int i = 0; i < oldColumns.Length; i++)
                {
                    currentContainer.Remove(oldColumns[i]);
                }
            }

            DataGridViewColumn[] newColumns = new DataGridViewColumn[_columnsPrivateCopy.Count];
            bool[] userAddedColumnsInfo = new bool[_columnsPrivateCopy.Count];
            string?[] compNames = new string?[_columnsPrivateCopy.Count];
            for (int i = 0; i < _columnsPrivateCopy.Count; i++)
            {
                DataGridViewColumn newColumn = (DataGridViewColumn)_columnsPrivateCopy[i].Clone();
                // at design time we need to do a shallow copy for ContextMenuStrip property
                newColumn.ContextMenuStrip = _columnsPrivateCopy[i].ContextMenuStrip;

                newColumns[i] = newColumn;
                if (_userAddedColumns.Contains(_columnsPrivateCopy[i]))
                {
                    userAddedColumnsInfo[i] = true;
                }

                if (_columnsNames.TryGetValue(_columnsPrivateCopy[i], out string? compName))
                {
                    compNames[i] = compName;
                }
            }

            // 5. IContainer.Add(new dataGridView.Columns)
            if (currentContainer is not null)
            {
                for (int i = 0; i < newColumns.Length; i++)
                {
                    string? compName = compNames[i];
                    if (!string.IsNullOrEmpty(compName) && ValidateName(currentContainer, compName, newColumns[i]))
                    {
                        currentContainer.Add(newColumns[i], compName);
                    }
                    else
                    {
                        currentContainer.Add(newColumns[i]);
                    }
                }
            }

            // 6. OnComponentChanging DataGridView.Columns
            changeService?.OnComponentChanging(_liveDataGridView, prop);

            // 7. DataGridView.Columns.Add( new DataGridViewColumns)
            for (int i = 0; i < newColumns.Length; i++)
            {
                // wipe out the DisplayIndex
                PropertyDescriptor? propertyDescriptor = TypeDescriptor.GetProperties(newColumns[i])["DisplayIndex"];
                propertyDescriptor?.SetValue(newColumns[i], -1);

                _liveDataGridView.Columns.Add(newColumns[i]);
            }

            // 8. OnComponentChanged DataGridView.Columns
            changeService?.OnComponentChanged(_liveDataGridView, prop, null, null);
            for (int i = 0; i < userAddedColumnsInfo.Length; i++)
            {
                PropertyDescriptor? pd = TypeDescriptor.GetProperties(newColumns[i])["UserAddedColumn"];
                pd?.SetValue(newColumns[i], userAddedColumnsInfo[i]);
            }
        }
        catch (InvalidOperationException ex)
        {
            IUIService? uiService = _liveDataGridView?.Site?.GetService<IUIService>();
            DataGridViewDesigner.ShowErrorDialog(uiService, ex, _liveDataGridView);
            DialogResult = DialogResult.Cancel;
        }
    }

    private void componentChanged(object? sender, ComponentChangedEventArgs e)
    {
        if (e.Component is ListBoxItem && _selectedColumns.Items.Contains(e.Component))
        {
            _formIsDirty = true;
        }
    }

    private static void CopyDataGridViewColumnProperties(DataGridViewColumn srcColumn, DataGridViewColumn destColumn)
    {
        destColumn.AutoSizeMode = srcColumn.AutoSizeMode;
        destColumn.ContextMenuStrip = srcColumn.ContextMenuStrip;
        destColumn.DataPropertyName = srcColumn.DataPropertyName;
        if (srcColumn.HasDefaultCellStyle)
        {
            CopyDefaultCellStyle(srcColumn, destColumn);
        }

        destColumn.DividerWidth = srcColumn.DividerWidth;
        destColumn.HeaderText = srcColumn.HeaderText;
        destColumn.MinimumWidth = srcColumn.MinimumWidth;
        destColumn.Name = srcColumn.Name;
        destColumn.SortMode = srcColumn.SortMode;
        destColumn.Tag = srcColumn.Tag;
        destColumn.ToolTipText = srcColumn.ToolTipText;
        destColumn.Width = srcColumn.Width;
        destColumn.FillWeight = srcColumn.FillWeight;
    }

    private static void CopyDataGridViewColumnState(DataGridViewColumn srcColumn, DataGridViewColumn destColumn)
    {
        destColumn.Frozen = srcColumn.Frozen;
        destColumn.Visible = srcColumn.Visible;
        destColumn.ReadOnly = srcColumn.ReadOnly;
        destColumn.Resizable = srcColumn.Resizable;
    }

    // We don't have any control over the srcColumn constructor.
    // So we do a catch all.
    private static void CopyDefaultCellStyle(DataGridViewColumn srcColumn, DataGridViewColumn destColumn)
    {
        // Here is what we want to do ( see vsw 352177 for more details ):
        // 1. If srcColumn and destColumn have the same type simply copy the default cell style
        //      from source to destination and be done w/ it.
        // 2. Otherwise, determine which properties in the cell style are no longer default and copy those properties.
        //      To do that we need to:
        //      2.a Create a default srcColumn so we get its default cell style. If we get an exception when we are
        //      creating the default cell style then we copy all the public properties.
        //      2.b Go thru the public properties in the DataGridViewCellStyle and copy only the property
        //      that are changed from the default values;
        //      2.c We need to special case the DataGridViewCellStyle::NullValue property. This property
        //      will be copied only if the NullValue has the same type in destColumn and in srcColumn.

        Type srcType = srcColumn.GetType();
        Type destType = destColumn.GetType();

        // 1. If srcColumn and destColumn have the same type simply copy the
        // default cell style from source to destination and be done w/ it.
        if (srcType.IsAssignableFrom(destType) || destType.IsAssignableFrom(srcType))
        {
            destColumn.DefaultCellStyle = srcColumn.DefaultCellStyle;
            return;
        }

        // 2.a Create a default srcColumn so we get its default cell style.
        // If we get an exception when we are creating the default cell style
        //      then we copy all the public properties.
        DataGridViewColumn? defaultSrcColumn = null;
        try
        {
            defaultSrcColumn = Activator.CreateInstance(srcType) as DataGridViewColumn;
        }
        catch (Exception e)
        {
            if (ExceptionExtensions.IsCriticalException(e))
            {
                throw;
            }
        }

        // 2.b Go thru the public properties in the DataGridViewCellStyle and copy only the property
        // that are changed from the default values;
        if (defaultSrcColumn is null || defaultSrcColumn.DefaultCellStyle.Alignment != srcColumn.DefaultCellStyle.Alignment)
        {
            destColumn.DefaultCellStyle.Alignment = srcColumn.DefaultCellStyle.Alignment;
        }

        if (defaultSrcColumn is null || !defaultSrcColumn.DefaultCellStyle.BackColor.Equals(srcColumn.DefaultCellStyle.BackColor))
        {
            destColumn.DefaultCellStyle.BackColor = srcColumn.DefaultCellStyle.BackColor;
        }

        if (defaultSrcColumn is not null && srcColumn.DefaultCellStyle.Font is not null && !srcColumn.DefaultCellStyle.Font.Equals(defaultSrcColumn.DefaultCellStyle.Font))
        {
            destColumn.DefaultCellStyle.Font = srcColumn.DefaultCellStyle.Font;
        }

        if (defaultSrcColumn is null || !defaultSrcColumn.DefaultCellStyle.ForeColor.Equals(srcColumn.DefaultCellStyle.ForeColor))
        {
            destColumn.DefaultCellStyle.ForeColor = srcColumn.DefaultCellStyle.ForeColor;
        }

        if (defaultSrcColumn is null || !defaultSrcColumn.DefaultCellStyle.Format.Equals(srcColumn.DefaultCellStyle.Format))
        {
            destColumn.DefaultCellStyle.Format = srcColumn.DefaultCellStyle.Format;
        }

        if (defaultSrcColumn is null || defaultSrcColumn.DefaultCellStyle.Padding != srcColumn.DefaultCellStyle.Padding)
        {
            destColumn.DefaultCellStyle.Padding = srcColumn.DefaultCellStyle.Padding;
        }

        if (defaultSrcColumn is null || !defaultSrcColumn.DefaultCellStyle.SelectionBackColor.Equals(srcColumn.DefaultCellStyle.SelectionBackColor))
        {
            destColumn.DefaultCellStyle.SelectionBackColor = srcColumn.DefaultCellStyle.SelectionBackColor;
        }

        if (defaultSrcColumn is null || !defaultSrcColumn.DefaultCellStyle.SelectionForeColor.Equals(srcColumn.DefaultCellStyle.SelectionForeColor))
        {
            destColumn.DefaultCellStyle.SelectionForeColor = srcColumn.DefaultCellStyle.SelectionForeColor;
        }

        if (defaultSrcColumn is null || defaultSrcColumn.DefaultCellStyle.WrapMode != srcColumn.DefaultCellStyle.WrapMode)
        {
            destColumn.DefaultCellStyle.WrapMode = srcColumn.DefaultCellStyle.WrapMode;
        }

        // 2.c We need to special case the DataGridViewCellStyle::NullValue property. This property will be copied only if the NullValue
        // has the same type in destColumn and in srcColumn.
        if (!srcColumn.DefaultCellStyle.IsNullValueDefault)
        {
            object? srcNullValue = srcColumn.DefaultCellStyle.NullValue;
            object? destNullValue = destColumn.DefaultCellStyle.NullValue;

            if (srcNullValue is not null && destNullValue is not null && srcNullValue.GetType() == destNullValue.GetType())
            {
                destColumn.DefaultCellStyle.NullValue = srcNullValue;
            }
        }
    }

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
    }

    private void FixColumnCollectionDisplayIndices()
    {
        for (int i = 0; i < _columnsPrivateCopy.Count; i++)
        {
            _columnsPrivateCopy[i].DisplayIndex = i;
        }
    }

    private void HookComponentChangedEventHandler(IComponentChangeService componentChangeService)
    {
        if (componentChangeService is not null)
        {
            componentChangeService.ComponentChanged += componentChanged;
        }
    }

    #region Windows Form Designer generated code
    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    [MemberNotNull(nameof(_addButton))]
    [MemberNotNull(nameof(_deleteButton))]
    [MemberNotNull(nameof(_moveDown))]
    [MemberNotNull(nameof(_moveUp))]
    [MemberNotNull(nameof(_selectedColumns))]
    [MemberNotNull(nameof(_overarchingTableLayoutPanel))]
    [MemberNotNull(nameof(_addRemoveTableLayoutPanel))]
    [MemberNotNull(nameof(_selectedColumnsLabel))]
    [MemberNotNull(nameof(_propertyGridLabel))]
    [MemberNotNull(nameof(_propertyGrid1))]
    [MemberNotNull(nameof(_okCancelTableLayoutPanel))]
    [MemberNotNull(nameof(_cancelButton))]
    [MemberNotNull(nameof(_okButton))]
    private void InitializeComponent()
    {
        ComponentResourceManager resources = new(typeof(DataGridViewColumnCollectionDialog));
        _addButton = new Button();
        _deleteButton = new Button();
        _moveDown = new Button();
        _moveUp = new Button();
        _selectedColumns = new ListBox();
        _overarchingTableLayoutPanel = new TableLayoutPanel();
        _addRemoveTableLayoutPanel = new TableLayoutPanel();
        _selectedColumnsLabel = new Label();
        _propertyGridLabel = new Label();
        _propertyGrid1 = new VsPropertyGrid();
        _okCancelTableLayoutPanel = new TableLayoutPanel();
        _cancelButton = new Button();
        _okButton = new Button();
        _overarchingTableLayoutPanel.SuspendLayout();
        _addRemoveTableLayoutPanel.SuspendLayout();
        _okCancelTableLayoutPanel.SuspendLayout();
        SuspendLayout();
        //
        // addButton
        //
        resources.ApplyResources(_addButton, "addButton");
        _addButton.Margin = new Padding(0, 0, 3, 0);
        _addButton.Name = "addButton";
        _addButton.Padding = new Padding(10, 0, 10, 0);
        _addButton.Click += addButton_Click;
        //
        // deleteButton
        //
        resources.ApplyResources(_deleteButton, "deleteButton");
        _deleteButton.Margin = new Padding(3, 0, 0, 0);
        _deleteButton.Name = "deleteButton";
        _deleteButton.Padding = new Padding(10, 0, 10, 0);
        _deleteButton.Click += deleteButton_Click;
        //
        // moveDown
        //
        resources.ApplyResources(_moveDown, "moveDown");
        _moveDown.Margin = new Padding(0, 1, 18, 0);
        _moveDown.Name = "moveDown";
        _moveDown.Click += moveDown_Click;
        //
        // moveUp
        //
        resources.ApplyResources(_moveUp, "moveUp");
        _moveUp.Margin = new Padding(0, 0, 18, 1);
        _moveUp.Name = "moveUp";
        _moveUp.Click += moveUp_Click;
        //
        // selectedColumns
        //
        resources.ApplyResources(_selectedColumns, "selectedColumns");
        _selectedColumns.DrawMode = DrawMode.OwnerDrawFixed;
        _selectedColumns.Margin = new Padding(0, 2, 3, 3);
        _selectedColumns.Name = "selectedColumns";
        _overarchingTableLayoutPanel.SetRowSpan(_selectedColumns, 2);
        _selectedColumns.SelectedIndexChanged += selectedColumns_SelectedIndexChanged;
        _selectedColumns.KeyPress += selectedColumns_KeyPress;
        _selectedColumns.DrawItem += selectedColumns_DrawItem;
        _selectedColumns.KeyUp += selectedColumns_KeyUp;
        //
        // overarchingTableLayoutPanel
        //
        resources.ApplyResources(_overarchingTableLayoutPanel, "overarchingTableLayoutPanel");
        _overarchingTableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
        _overarchingTableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
        _overarchingTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent));
        _overarchingTableLayoutPanel.Controls.Add(_addRemoveTableLayoutPanel, 0, 3);
        _overarchingTableLayoutPanel.Controls.Add(_moveUp, 1, 1);
        _overarchingTableLayoutPanel.Controls.Add(_selectedColumnsLabel, 0, 0);
        _overarchingTableLayoutPanel.Controls.Add(_moveDown, 1, 2);
        _overarchingTableLayoutPanel.Controls.Add(_propertyGridLabel, 2, 0);
        _overarchingTableLayoutPanel.Controls.Add(_selectedColumns, 0, 1);
        _overarchingTableLayoutPanel.Controls.Add(_propertyGrid1, 2, 1);
        _overarchingTableLayoutPanel.Controls.Add(_okCancelTableLayoutPanel, 0, 4);
        _overarchingTableLayoutPanel.Name = "overarchingTableLayoutPanel";
        _overarchingTableLayoutPanel.RowStyles.Add(new RowStyle());
        _overarchingTableLayoutPanel.RowStyles.Add(new RowStyle());
        _overarchingTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        _overarchingTableLayoutPanel.RowStyles.Add(new RowStyle());
        //
        // addRemoveTableLayoutPanel
        //
        resources.ApplyResources(_addRemoveTableLayoutPanel, "addRemoveTableLayoutPanel");
        _addRemoveTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        _addRemoveTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        _addRemoveTableLayoutPanel.Controls.Add(_addButton, 0, 0);
        _addRemoveTableLayoutPanel.Controls.Add(_deleteButton, 1, 0);
        _addRemoveTableLayoutPanel.Margin = new Padding(0, 3, 3, 3);
        _addRemoveTableLayoutPanel.Name = "addRemoveTableLayoutPanel";
        _addRemoveTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        //
        // selectedColumnsLabel
        //
        resources.ApplyResources(_selectedColumnsLabel, "selectedColumnsLabel");
        _selectedColumnsLabel.Margin = new Padding(0);
        _selectedColumnsLabel.Name = "selectedColumnsLabel";
        //
        // propertyGridLabel
        //
        resources.ApplyResources(_propertyGridLabel, "propertyGridLabel");
        _propertyGridLabel.Margin = new Padding(3, 0, 0, 0);
        _propertyGridLabel.Name = "propertyGridLabel";
        //
        // propertyGrid1
        //
        resources.ApplyResources(_propertyGrid1, "propertyGrid1");
        _propertyGrid1.LineColor = SystemColors.ScrollBar;
        _propertyGrid1.Margin = new Padding(3, 2, 0, 3);
        _propertyGrid1.Name = "propertyGrid1";
        _overarchingTableLayoutPanel.SetRowSpan(_propertyGrid1, 3);
        _propertyGrid1.PropertyValueChanged += propertyGrid1_PropertyValueChanged;
        //
        // okCancelTableLayoutPanel
        //
        resources.ApplyResources(_okCancelTableLayoutPanel, "okCancelTableLayoutPanel");
        _okCancelTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        _okCancelTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        _okCancelTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
        _okCancelTableLayoutPanel.Controls.Add(_cancelButton, 1, 0);
        _okCancelTableLayoutPanel.Controls.Add(_okButton, 0, 0);
        _okCancelTableLayoutPanel.Name = "okCancelTableLayoutPanel";
        _overarchingTableLayoutPanel.SetColumnSpan(_okCancelTableLayoutPanel, 3);
        _okCancelTableLayoutPanel.RowStyles.Add(new RowStyle());
        //
        // cancelButton
        //
        resources.ApplyResources(_cancelButton, "cancelButton");
        _cancelButton.DialogResult = DialogResult.Cancel;
        _cancelButton.Margin = new Padding(3, 0, 0, 0);
        _cancelButton.Name = "cancelButton";
        _cancelButton.Padding = new Padding(10, 0, 10, 0);
        //
        // okButton
        //
        resources.ApplyResources(_okButton, "okButton");
        _okButton.DialogResult = DialogResult.OK;
        _okButton.Margin = new Padding(0, 0, 3, 0);
        _okButton.Name = "okButton";
        _okButton.Padding = new Padding(10, 0, 10, 0);
        _okButton.Click += okButton_Click;
        //
        // DataGridViewColumnCollectionDialog
        //
        AcceptButton = _okButton;
        resources.ApplyResources(this, "$this");
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = _cancelButton;
        Controls.Add(_overarchingTableLayoutPanel);
        HelpButton = true;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "DataGridViewColumnCollectionDialog";
        Padding = new Padding(12);
        ShowIcon = false;
        ShowInTaskbar = false;
        HelpButtonClicked += DataGridViewColumnCollectionDialog_HelpButtonClicked;
        FormClosed += DataGridViewColumnCollectionDialog_Closed;
        Load += DataGridViewColumnCollectionDialog_Load;
        HelpRequested += DataGridViewColumnCollectionDialog_HelpRequested;
        _overarchingTableLayoutPanel.ResumeLayout(false);
        _overarchingTableLayoutPanel.PerformLayout();
        _addRemoveTableLayoutPanel.ResumeLayout(false);
        _addRemoveTableLayoutPanel.PerformLayout();
        _okCancelTableLayoutPanel.ResumeLayout(false);
        _okCancelTableLayoutPanel.PerformLayout();
        ResumeLayout(false);
    }
    #endregion

    private static bool IsColumnAddedByUser(DataGridViewColumn col)
    {
        PropertyDescriptor? propertyDescriptor = TypeDescriptor.GetProperties(col)["UserAddedColumn"];
        object? val = propertyDescriptor?.GetValue(col);
        if (val is not null)
        {
            return (bool)val;
        }
        else
        {
            return false;
        }
    }

    private void okButton_Click(object? sender, EventArgs e)
    {
        CommitChanges();
    }

    private void moveDown_Click(object? sender, EventArgs e)
    {
        int selectedIndex = _selectedColumns.SelectedIndex;
        Debug.Assert(selectedIndex > -1 && selectedIndex < _selectedColumns.Items.Count - 1);

        _columnCollectionChanging = true;
        try
        {
            ListBoxItem? item = (ListBoxItem)_selectedColumns.SelectedItem!;
            _selectedColumns.Items.RemoveAt(selectedIndex);
            _selectedColumns.Items.Insert(selectedIndex + 1, item);

            // now do the same thing to the column collection
            _columnsPrivateCopy.RemoveAt(selectedIndex);

            // if the column we moved was frozen, make sure the column below is frozen too
            if (item.DataGridViewColumn.Frozen)
            {
                _columnsPrivateCopy[selectedIndex].Frozen = true;
#if DEBUG
                // sanity check
                for (int i = 0; i < selectedIndex; i++)
                {
                    Debug.Assert(_columnsPrivateCopy[i].Frozen, "MOVE_DOWN : all the columns up to the one we moved should be frozen");
                }
#endif // DEBUG
            }

            // wipe out the DisplayIndex
            item.DataGridViewColumn.DisplayIndex = -1;
            _columnsPrivateCopy.Insert(selectedIndex + 1, item.DataGridViewColumn);

            // properties like DataGridViewColumn::Frozen are dependent on the DisplayIndex
            FixColumnCollectionDisplayIndices();
        }
        finally
        {
            _columnCollectionChanging = false;
        }

        _formIsDirty = true;
        _selectedColumns.SelectedIndex = selectedIndex + 1;
        _moveUp.Enabled = _selectedColumns.SelectedIndex > 0;
        _moveDown.Enabled = _selectedColumns.SelectedIndex < _selectedColumns.Items.Count - 1;
    }

    private void moveUp_Click(object? sender, EventArgs e)
    {
        int selectedIndex = _selectedColumns.SelectedIndex;
        Debug.Assert(selectedIndex > 0);

        _columnCollectionChanging = true;
        try
        {
            ListBoxItem item = (ListBoxItem)_selectedColumns.Items[selectedIndex - 1];
            _selectedColumns.Items.RemoveAt(selectedIndex - 1);
            _selectedColumns.Items.Insert(selectedIndex, item);

            // now do the same thing to the column collection
            _columnsPrivateCopy.RemoveAt(selectedIndex - 1);

            // we want to keep the Frozen value of the column we move intact
            // if we move up an UnFrozen column and the column above the one we move is Frozen
            // then we need to make the column above the one we move UnFrozen, too
            //
            // columnsPrivateCopy[selectedIndex - 1] points to the column we just moved
            //
            if (item.DataGridViewColumn.Frozen && !_columnsPrivateCopy[selectedIndex - 1].Frozen)
            {
                item.DataGridViewColumn.Frozen = false;
            }

            // wipe out the display index.
            item.DataGridViewColumn.DisplayIndex = -1;
            _columnsPrivateCopy.Insert(selectedIndex, item.DataGridViewColumn);

            // properties like DataGridViewColumn::Frozen are dependent on the DisplayIndex
            FixColumnCollectionDisplayIndices();
        }
        finally
        {
            _columnCollectionChanging = false;
        }

        _formIsDirty = true;
        _selectedColumns.SelectedIndex = selectedIndex - 1;
        _moveUp.Enabled = _selectedColumns.SelectedIndex > 0;
        _moveDown.Enabled = _selectedColumns.SelectedIndex < _selectedColumns.Items.Count - 1;

        // vsw 495403: keep the selected item visible.
        // For some reason, we only have to do this when we move a column up.
        // When we move a column down or when we delete a column, the selected item remains visible.
        if (_selectedColumns.SelectedIndex != -1 && _selectedColumns.TopIndex > _selectedColumns.SelectedIndex)
        {
            _selectedColumns.TopIndex = _selectedColumns.SelectedIndex;
        }
    }

    private void DataGridViewColumnCollectionDialog_Closed(object? sender, EventArgs e)
    {
        // scrub the TypeDescriptor association between DataGridViewColumns and their designers
        for (int i = 0; i < _selectedColumns.Items.Count; i++)
        {
            ListBoxItem? lbi = _selectedColumns.Items[i] as ListBoxItem;
            if (lbi?.DataGridViewColumnDesigner is not null)
            {
                TypeDescriptor.RemoveAssociation(lbi.DataGridViewColumn, lbi.DataGridViewColumnDesigner);
            }
        }

        _columnsNames.Clear();
        _userAddedColumns.Clear();
    }

    private void DataGridViewColumnCollectionDialog_HelpButtonClicked(object? sender, CancelEventArgs e)
    {
        e.Cancel = true;
        DataGridViewColumnCollectionDialog_HelpRequestHandled();
    }

    private void DataGridViewColumnCollectionDialog_HelpRequested(object? sender, HelpEventArgs e)
    {
        DataGridViewColumnCollectionDialog_HelpRequestHandled();
        e.Handled = true;
    }

    private void DataGridViewColumnCollectionDialog_HelpRequestHandled()
    {
        IHelpService? helpService = _liveDataGridView?.Site?.GetService<IHelpService>();
        helpService?.ShowHelpFromKeyword("vs.DataGridViewColumnCollectionDialog");
    }

    private void DataGridViewColumnCollectionDialog_Load(object? sender, EventArgs e)
    {
        // Get the Dialog Font
        IUIService? uiService = _liveDataGridView?.Site?.GetService<IUIService>();
        object? font = uiService?.Styles["DialogFont"];
        if (font is not Font uiFont)
        {
            uiFont = DefaultFont;
        }

        Font = uiFont;

        // keep the selected index to 0 or -1 if there are no selected columns
        _selectedColumns.SelectedIndex = Math.Min(0, _selectedColumns.Items.Count - 1);

        _moveUp.Enabled = _selectedColumns.SelectedIndex > 0;
        _moveDown.Enabled = _selectedColumns.SelectedIndex < _selectedColumns.Items.Count - 1;
        _deleteButton.Enabled = _selectedColumns.Items.Count > 0 && _selectedColumns.SelectedIndex != -1;
        _propertyGrid1.SelectedObject = _selectedColumns.SelectedItem;

        _selectedColumns.ItemHeight = Font.Height + OWNERDRAWVERTICALBUFFER;

        ActiveControl = _selectedColumns;

        SetSelectedColumnsHorizontalExtent();

        _selectedColumns.Focus();

        _formIsDirty = false;
    }

    private void deleteButton_Click(object? sender, EventArgs e)
    {
        Debug.Assert(_selectedColumns.SelectedIndex != -1);
        int selectedIndex = _selectedColumns.SelectedIndex;

        _columnsNames.Remove(_columnsPrivateCopy[selectedIndex]);
        _userAddedColumns.Remove(_columnsPrivateCopy[selectedIndex]);

        _columnsPrivateCopy.RemoveAt(selectedIndex);

        // try to keep the same selected index
        _selectedColumns.SelectedIndex = Math.Min(_selectedColumns.Items.Count - 1, selectedIndex);

        _moveUp.Enabled = _selectedColumns.SelectedIndex > 0;
        _moveDown.Enabled = _selectedColumns.SelectedIndex < _selectedColumns.Items.Count - 1;
        _deleteButton.Enabled = _selectedColumns.Items.Count > 0 && _selectedColumns.SelectedIndex != -1;
        _propertyGrid1.SelectedObject = _selectedColumns.SelectedItem;
    }

    private void addButton_Click(object? sender, EventArgs e)
    {
        int insertIndex = _selectedColumns.SelectedIndex == -1 ? _selectedColumns.Items.Count : _selectedColumns.SelectedIndex + 1;

        if (_addColumnDialog is null)
        {
            // child modal dialog -launching in System Aware mode
            _addColumnDialog = ScaleHelper.InvokeInSystemAwareContext(() => new DataGridViewAddColumnDialog(_columnsPrivateCopy, _liveDataGridView!));
            _addColumnDialog.StartPosition = FormStartPosition.CenterParent;
        }

        _addColumnDialog.Start(insertIndex, persistChangesToDesigner: false);

        _addColumnDialog.ShowDialog(this);
    }

    private void PopulateSelectedColumns()
    {
        int selectedIndex = _selectedColumns.SelectedIndex;

        // scrub the TypeDescriptor association between DataGridViewColumns and their designers
        for (int i = 0; i < _selectedColumns.Items.Count; i++)
        {
            ListBoxItem? lbi = _selectedColumns.Items[i] as ListBoxItem;
            if (lbi?.DataGridViewColumnDesigner is not null)
            {
                TypeDescriptor.RemoveAssociation(lbi.DataGridViewColumn, lbi.DataGridViewColumnDesigner);
            }
        }

        _selectedColumns.Items.Clear();
        ITypeResolutionService? tr = _liveDataGridView?.Site?.GetService<ITypeResolutionService>();

        for (int i = 0; i < _columnsPrivateCopy.Count; i++)
        {
            ComponentDesigner columnDesigner = DataGridViewAddColumnDialog.GetComponentDesignerForType(tr, _columnsPrivateCopy[i].GetType())!;
            _selectedColumns.Items.Add(new ListBoxItem(_columnsPrivateCopy[i], this, columnDesigner));
        }

        _selectedColumns.SelectedIndex = Math.Min(selectedIndex, _selectedColumns.Items.Count - 1);

        SetSelectedColumnsHorizontalExtent();

        if (_selectedColumns.Items.Count == 0)
        {
            _propertyGridLabel.Text = string.Format(SR.DataGridViewProperties);
        }
    }

    private void propertyGrid1_PropertyValueChanged(object? sender, PropertyValueChangedEventArgs e)
    {
        if (_columnCollectionChanging)
        {
            return;
        }

        _formIsDirty = true;
        // refresh the selected columns when the user changed the HeaderText property
        if (e.ChangedItem!.PropertyDescriptor!.Name.Equals("HeaderText"))
        {
            // invalidate the selected index only
            int selectedIndex = _selectedColumns.SelectedIndex;
            Debug.Assert(selectedIndex != -1, "we forgot to take away the selected object from the property grid");
            Rectangle bounds = new(0, selectedIndex * _selectedColumns.ItemHeight, _selectedColumns.Width, _selectedColumns.ItemHeight);
            _columnCollectionChanging = true;
            try
            {
                // for accessibility reasons, we need to reset the item in the selected columns collection.
                _selectedColumns.Items[selectedIndex] = _selectedColumns.Items[selectedIndex];
            }
            finally
            {
                _columnCollectionChanging = false;
            }

            _selectedColumns.Invalidate(bounds);

            // if the header text changed make sure that we update the selected columns HorizontalExtent
            SetSelectedColumnsHorizontalExtent();
        }
        else if (e.ChangedItem.PropertyDescriptor.Name.Equals("DataPropertyName"))
        {
            ListBoxItem? listBoxItem = _selectedColumns.SelectedItem as ListBoxItem;
            DataGridViewColumn? column = listBoxItem?.DataGridViewColumn;

            if (string.IsNullOrEmpty(column?.DataPropertyName))
            {
                _propertyGridLabel.Text = (SR.DataGridViewUnboundColumnProperties);
            }
            else
            {
                _propertyGridLabel.Text = (SR.DataGridViewBoundColumnProperties);
            }
        }
        else if (e.ChangedItem.PropertyDescriptor.Name.Equals("Name"))
        {
            ListBoxItem? listBoxItem = _selectedColumns.SelectedItem as ListBoxItem;
            DataGridViewColumn? col = listBoxItem?.DataGridViewColumn;
            if (col is not null)
            {
                _columnsNames[col] = col.Name;
            }
        }
    }

    private void selectedColumns_DrawItem(object? sender, DrawItemEventArgs e)
    {
        if (e.Index < 0)
        {
            return;
        }

        ListBoxItem? listBoxItem = _selectedColumns.Items[e.Index] as ListBoxItem;

#if DGV_DITHERING
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                ImageAttributes attributes = new();

                colorMap[0].OldColor = Color.White;
                colorMap[0].NewColor = e.BackColor;

                //
                // TO DO : DITHER
                //
                attributes.SetRemapTable(colorMap, ColorAdjustType.Bitmap);

                Rectangle imgRectangle = new(e.Bounds.X + OWNERDRAWITEMIMAGEBUFFER, e.Bounds.Y + OWNERDRAWITEMIMAGEBUFFER, listBoxItem.ToolboxBitmap.Width, listBoxItem.ToolboxBitmap.Height);
                e.Graphics.DrawImage(listBoxItem.ToolboxBitmap,
                                     imgRectangle,
                                     0,
                                     0,
                                     imgRectangle.Width,
                                     imgRectangle.Height,
                                     GraphicsUnit.Pixel,
                                     attributes);
                attributes.Dispose();
            }
            else
            {
#endif // DGV_DITHERING
        e.Graphics.DrawImage(listBoxItem!.ToolboxBitmap!,
                             e.Bounds.X + OWNERDRAWITEMIMAGEBUFFER,
                             e.Bounds.Y + OWNERDRAWITEMIMAGEBUFFER,
                             listBoxItem.ToolboxBitmap!.Width,
                             listBoxItem.ToolboxBitmap.Height);

        Rectangle bounds = e.Bounds;
        bounds.Width -= listBoxItem.ToolboxBitmap.Width + 2 * OWNERDRAWITEMIMAGEBUFFER;
        bounds.X += listBoxItem.ToolboxBitmap.Width + 2 * OWNERDRAWITEMIMAGEBUFFER;
        bounds.Y += OWNERDRAWITEMIMAGEBUFFER;
        bounds.Height -= 2 * OWNERDRAWITEMIMAGEBUFFER;

        Brush selectedBrush = new SolidBrush(e.BackColor);
        Brush foreBrush = new SolidBrush(e.ForeColor);
        Brush backBrush = new SolidBrush(_selectedColumns.BackColor);

        string columnName = ((ListBoxItem)_selectedColumns.Items[e.Index]).ToString();

        if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
        {
            // first get the text rectangle
            int textWidth = Size.Ceiling(e.Graphics.MeasureString(columnName, e.Font!, new SizeF(bounds.Width, bounds.Height))).Width;
            // DANIELHE: the spec calls for + 7 but I think that + 3 does the trick better
            Rectangle focusRectangle = new(bounds.X, e.Bounds.Y + 1, textWidth + OWNERDRAWHORIZONTALBUFFER, e.Bounds.Height - 2);

            e.Graphics.FillRectangle(selectedBrush, focusRectangle);
            focusRectangle.Inflate(-1, -1);

            e.Graphics.DrawString(columnName, e.Font!, foreBrush, focusRectangle);

            focusRectangle.Inflate(1, 1);

            // only paint the focus rectangle when the list box is focused
            if (_selectedColumns.Focused)
            {
                ControlPaint.DrawFocusRectangle(e.Graphics, focusRectangle, e.ForeColor, e.BackColor);
            }

            e.Graphics.FillRectangle(backBrush, new Rectangle(focusRectangle.Right + 1, e.Bounds.Y, e.Bounds.Width - focusRectangle.Right - 1, e.Bounds.Height));
        }
        else
        {
            e.Graphics.FillRectangle(backBrush, new Rectangle(bounds.X, e.Bounds.Y, e.Bounds.Width - bounds.X, e.Bounds.Height));

            e.Graphics.DrawString(columnName, e.Font!, foreBrush, bounds);
        }

        selectedBrush.Dispose();
        backBrush.Dispose();
        foreBrush.Dispose();
    }

    private void selectedColumns_KeyUp(object? sender, KeyEventArgs e)
    {
        if ((e.Modifiers) == 0 && e.KeyCode == Keys.F4)
        {
            _propertyGrid1.Focus();
            e.Handled = true;
        }
    }

    private void selectedColumns_KeyPress(object? sender, KeyPressEventArgs e)
    {
        Keys modifierKeys = ModifierKeys;

        // vsw 479960.
        // Don't let Ctrl-* propagate to the selected columns list box.
        if ((modifierKeys & Keys.Control) != 0)
        {
            e.Handled = true;
        }
    }

    private void selectedColumns_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (_columnCollectionChanging)
        {
            return;
        }

        _propertyGrid1.SelectedObject = _selectedColumns.SelectedItem;

        // enable/disable up/down/delete buttons
        _moveDown.Enabled = _selectedColumns.Items.Count > 0 && _selectedColumns.SelectedIndex != _selectedColumns.Items.Count - 1;
        _moveUp.Enabled = _selectedColumns.Items.Count > 0 && _selectedColumns.SelectedIndex > 0;
        _deleteButton.Enabled = _selectedColumns.Items.Count > 0 && _selectedColumns.SelectedIndex != -1;

        if (_selectedColumns.SelectedItem is not null)
        {
            DataGridViewColumn column = ((ListBoxItem)_selectedColumns.SelectedItem).DataGridViewColumn;
            if (string.IsNullOrEmpty(column.DataPropertyName))
            {
                _propertyGridLabel.Text = (SR.DataGridViewUnboundColumnProperties);
            }
            else
            {
                _propertyGridLabel.Text = (SR.DataGridViewBoundColumnProperties);
            }
        }
        else
        {
            _propertyGridLabel.Text = (SR.DataGridViewProperties);
        }
    }

    internal void SetLiveDataGridView(DataGridView dataGridView)
    {
        IComponentChangeService? newComponentChangeService = dataGridView.Site?.GetService<IComponentChangeService>();

        if (newComponentChangeService != _compChangeService)
        {
            UnhookComponentChangedEventHandler(_compChangeService!);

            _compChangeService = newComponentChangeService;

            HookComponentChangedEventHandler(_compChangeService!);
        }

        _liveDataGridView = dataGridView;

        _dataGridViewPrivateCopy.Site = dataGridView.Site;
        _dataGridViewPrivateCopy.AutoSizeColumnsMode = dataGridView.AutoSizeColumnsMode;
        _dataGridViewPrivateCopy.DataSource = dataGridView.DataSource;
        _dataGridViewPrivateCopy.DataMember = dataGridView.DataMember;
        _columnsNames.Clear();
        _columnsNames.EnsureCapacity(_columnsPrivateCopy.Count);
        _columnsPrivateCopy.Clear();

        _userAddedColumns.Clear();
        _userAddedColumns.EnsureCapacity(_liveDataGridView.Columns.Count);

        // Set ColumnCollectionChanging to true so:
        // 1. the column collection changed event handler does not execute PopulateSelectedColumns over and over again.
        // 2. the collection changed event handler does not add each live column to its userAddedColumns hash table.
        //
        _columnCollectionChanging = true;
        try
        {
            for (int i = 0; i < _liveDataGridView.Columns.Count; i++)
            {
                DataGridViewColumn liveCol = _liveDataGridView.Columns[i];
                DataGridViewColumn col = (DataGridViewColumn)liveCol.Clone();
                // at design time we need to do a shallow copy for the ContextMenuStrip property
                //
                col.ContextMenuStrip = _liveDataGridView.Columns[i].ContextMenuStrip;
                // wipe out the display index before adding the new column.
                col.DisplayIndex = -1;
                _columnsPrivateCopy.Add(col);

                if (liveCol.Site is not null)
                {
                    _columnsNames[col] = liveCol.Site.Name;
                }

                if (IsColumnAddedByUser(liveCol))
                {
                    _userAddedColumns.Add(col);
                }
            }
        }
        finally
        {
            _columnCollectionChanging = false;
        }

        PopulateSelectedColumns();

        _propertyGrid1.Site = new DataGridViewComponentPropertyGridSite(_liveDataGridView.Site, _liveDataGridView);

        _propertyGrid1.SelectedObject = _selectedColumns.SelectedItem;
    }

    private void SetSelectedColumnsHorizontalExtent()
    {
        int maxItemWidth = 0;
        for (int i = 0; i < _selectedColumns.Items.Count; i++)
        {
            int itemWidth = TextRenderer.MeasureText(_selectedColumns.Items[i].ToString(), _selectedColumns.Font).Width;
            maxItemWidth = Math.Max(maxItemWidth, itemWidth);
        }

        _selectedColumns.HorizontalExtent = SelectedColumnsItemBitmap.Width + 2 * OWNERDRAWITEMIMAGEBUFFER + maxItemWidth + OWNERDRAWHORIZONTALBUFFER;
    }

    private void UnhookComponentChangedEventHandler(IComponentChangeService componentChangeService)
    {
        if (componentChangeService is not null)
        {
            componentChangeService.ComponentChanged -= componentChanged;
        }
    }

    private static bool ValidateName(IContainer container, string siteName, IComponent component)
    {
        ComponentCollection components = container.Components;
        if (components is null)
        {
            return true;
        }

        for (int i = 0; i < components.Count; i++)
        {
            ISite? s = components[i]?.Site;

            if (s is not null && s.Name is not null && string.Equals(s.Name, siteName, StringComparison.OrdinalIgnoreCase) && s.Component != component)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    ///  internal because the DataGridViewColumnDataPropertyNameEditor needs to get at the ListBoxItem
    ///  IComponent because some editors for some dataGridViewColumn properties - DataGridViewComboBox::DataSource editor -
    ///  need the site
    /// </summary>
    internal class ListBoxItem : ICustomTypeDescriptor, IComponent
    {
        public DataGridViewColumn DataGridViewColumn { get; }
        public DataGridViewColumnCollectionDialog Owner { get; }
        public ComponentDesigner? DataGridViewColumnDesigner { get; }
        public Image? ToolboxBitmap { get; }

        public ListBoxItem(DataGridViewColumn column, DataGridViewColumnCollectionDialog owner, ComponentDesigner compDesigner)
        {
            DataGridViewColumn = column;
            Owner = owner;
            DataGridViewColumnDesigner = compDesigner;

            if (DataGridViewColumnDesigner is not null)
            {
                DataGridViewColumnDesigner.Initialize(column);
                TypeDescriptor.CreateAssociation(DataGridViewColumn, DataGridViewColumnDesigner);
            }

            if (TypeDescriptorHelper.TryGetAttribute(column, out ToolboxBitmapAttribute? attr))
            {
                ToolboxBitmap = attr.GetImage(column, large: false);
            }
            else
            {
                ToolboxBitmap = SelectedColumnsItemBitmap;
            }

            DataGridViewColumnDesigner? dataGridViewColumnDesigner = compDesigner as DataGridViewColumnDesigner;
            if (dataGridViewColumnDesigner is not null && Owner._liveDataGridView is not null)
            {
                dataGridViewColumnDesigner.LiveDataGridView = Owner._liveDataGridView;
            }
        }

        public override string ToString()
        {
            return DataGridViewColumn.HeaderText;
        }

        // ICustomTypeDescriptor implementation
        AttributeCollection ICustomTypeDescriptor.GetAttributes() => TypeDescriptor.GetAttributes(DataGridViewColumn);

        string? ICustomTypeDescriptor.GetClassName() => TypeDescriptor.GetClassName(DataGridViewColumn);

        string? ICustomTypeDescriptor.GetComponentName() => TypeDescriptor.GetComponentName(DataGridViewColumn);

        [RequiresUnreferencedCode("Generic TypeConverters may require the generic types to be annotated. For example, NullableConverter requires the underlying type to be DynamicallyAccessedMembers All.")]
        TypeConverter ICustomTypeDescriptor.GetConverter() => TypeDescriptor.GetConverter(DataGridViewColumn);

        EventDescriptor? ICustomTypeDescriptor.GetDefaultEvent() => TypeDescriptor.GetDefaultEvent(DataGridViewColumn);

        PropertyDescriptor? ICustomTypeDescriptor.GetDefaultProperty() => TypeDescriptor.GetDefaultProperty(DataGridViewColumn);

        [RequiresUnreferencedCode("Design-time attributes are not preserved when trimming. Types referenced by attributes like EditorAttribute and DesignerAttribute may not be available after trimming.")]
        object? ICustomTypeDescriptor.GetEditor(Type type) => TypeDescriptor.GetEditor(DataGridViewColumn, type);

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents() => TypeDescriptor.GetEvents(DataGridViewColumn);

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[]? attrs) => TypeDescriptor.GetEvents(DataGridViewColumn, attrs!);

        [RequiresUnreferencedCode("PropertyDescriptor's PropertyType cannot be statically discovered.")]
        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties() => ((ICustomTypeDescriptor)this).GetProperties(null);

        [RequiresUnreferencedCode("PropertyDescriptor's PropertyType cannot be statically discovered. The public parameterless constructor or the 'Default' static field may be trimmed from the Attribute's Type.")]
        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[]? attrs)
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(DataGridViewColumn);

            PropertyDescriptor[]? propArray;
            if (DataGridViewColumnDesigner is not null)
            {
                // PropertyDescriptorCollection does not let us change properties.
                // So we have to create a hash table that we pass to PreFilterProperties
                // and then copy back the result from PreFilterProperties

                // We should look into speeding this up w/ our own DataGridViewColumnTypes...
                //
                Dictionary<string, PropertyDescriptor> hash = [];
                for (int i = 0; i < props.Count; i++)
                {
                    hash.Add(props[i].Name, props[i]);
                }

                ((IDesignerFilter)DataGridViewColumnDesigner).PreFilterProperties(hash);

                // PreFilterProperties can add / remove properties.
                // Use the hashtable's Count, not the old property descriptor collection's count.
                propArray = new PropertyDescriptor[hash.Count + 1];
                hash.Values.CopyTo(propArray, 0);
            }
            else
            {
                propArray = new PropertyDescriptor[props.Count + 1];
                props.CopyTo(propArray, 0);
            }

            propArray[^1] = new ColumnTypePropertyDescriptor();

            return new PropertyDescriptorCollection(propArray);
        }

        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor? pd) =>
            pd is ColumnTypePropertyDescriptor ? this : DataGridViewColumn;

        ISite? IComponent.Site
        {
            get => Owner._liveDataGridView?.Site;
            set { }
        }

        event EventHandler? IComponent.Disposed
        {
            add { }
            remove { }
        }

        void IDisposable.Dispose()
        {
        }
    }

    private class ColumnTypePropertyDescriptor : PropertyDescriptor
    {
        public ColumnTypePropertyDescriptor() : base("ColumnType", null)
        {
        }

        public override AttributeCollection Attributes
        {
            get
            {
                EditorAttribute editorAttr = new($"System.Windows.Forms.Design.DataGridViewColumnTypeEditor, {AssemblyRef.SystemDesign}", typeof(Drawing.Design.UITypeEditor));
                DescriptionAttribute descriptionAttr = new(SR.DataGridViewColumnTypePropertyDescription);
                CategoryAttribute categoryAttr = CategoryAttribute.Design;
                // add the description attribute and the categories attribute
                Attribute[] attrs = [editorAttr, descriptionAttr, categoryAttr];
                return new AttributeCollection(attrs);
            }
        }

        public override Type ComponentType => typeof(ListBoxItem);

        public override bool IsReadOnly => false;

        public override Type PropertyType => typeof(Type);

        public override bool CanResetValue(object component)
        {
            Debug.Assert(component is ListBoxItem, "this property descriptor only relates to the data grid view column class");
            return false;
        }

        public override object? GetValue(object? component)
        {
            if (component is null)
            {
                return null;
            }

            ListBoxItem item = (ListBoxItem)component;
            return item.DataGridViewColumn.GetType().Name;
        }

        public override void ResetValue(object component)
        {
        }

        public override void SetValue(object? component, object? value)
        {
            ListBoxItem? item = component as ListBoxItem;
            Type? type = value as Type;
            if (item?.DataGridViewColumn.GetType() != type)
            {
                item?.Owner.ColumnTypeChanged(item, type!);
                OnValueChanged(component, EventArgs.Empty);
            }
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }
    }
}
