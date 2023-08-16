// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace System.Windows.Forms.Design;

internal class DataGridViewColumnCollectionDialog : Form
{
    private Label? _selectedColumnsLabel;

    private ListBox? _selectedColumns;
    private Button? _moveUp;
    private Button? _moveDown;
    private Button? _deleteButton;
    private Button? _addButton;

    private Label? _propertyGridLabel;

    private PropertyGrid? _propertyGrid1;
    private TableLayoutPanel? _okCancelTableLayoutPanel;

    private Button? _okButton;

    private Button? _cancelButton;

    private DataGridView? _liveDataGridView;

    private IComponentChangeService? _compChangeService;

    private DataGridView _dataGridViewPrivateCopy;
    private DataGridViewColumnCollection _columnsPrivateCopy;
    private Hashtable? _columnsNames;
    private DataGridViewAddColumnDialog? _addColumnDialog;

    private const int _OWNERDRAWHORIZONTALBUFFER = 3;
    private const int _OWNERDRAWVERTICALBUFFER = 4;
    private const int _OWNERDRAWITEMIMAGEBUFFER = 2;

    // static because we can only have one instance of the DataGridViewColumnCollectionDialog running at a time
    private static Bitmap? _selectedColumnsItemBitmap;
    private static Type _iTypeResolutionServiceType = typeof(ITypeResolutionService);
    private static Type _iComponentChangeServiceType = typeof(IComponentChangeService);
    private static Type _iHelpServiceType = typeof(IHelpService);
    private static Type _iUIServiceType = typeof(IUIService);
    private static Type _toolboxBitmapAttributeType = typeof(ToolboxBitmapAttribute);

    private bool _columnCollectionChanging;

    private bool _formIsDirty;
    private TableLayoutPanel? _overarchingTableLayoutPanel;
    private TableLayoutPanel? _addRemoveTableLayoutPanel;
    private Hashtable? _userAddedColumns;

    private IServiceProvider _serviceProvider;

    internal DataGridViewColumnCollectionDialog(IServiceProvider provider)
    {
        _serviceProvider = provider;

        //
        // Required for Windows Form Designer support
        //
        InitializeComponent();

        if (DpiHelper.IsScalingRequired && _moveUp is not null && _moveDown is not null)
        {
            _moveUp.Image = DpiHelper.ScaleButtonImageLogicalToDevice(_moveUp.Image);
            _moveDown.Image = DpiHelper.ScaleButtonImageLogicalToDevice(_moveDown.Image);
        }

        _dataGridViewPrivateCopy = new DataGridView();
        _columnsPrivateCopy = _dataGridViewPrivateCopy.Columns;
        _columnsPrivateCopy.CollectionChanged += new CollectionChangeEventHandler(columnsPrivateCopy_CollectionChanged);
    }

    private static Bitmap SelectedColumnsItemBitmap
    {
        get
        {
            if (_selectedColumnsItemBitmap is null)
            {
                _selectedColumnsItemBitmap = new Bitmap(BitmapSelector.GetResourceStream(typeof(DataGridViewColumnCollectionDialog), "DataGridViewColumnsDialog.selectedColumns.bmp"));
                _selectedColumnsItemBitmap.MakeTransparent(System.Drawing.Color.Red);
            }

            return _selectedColumnsItemBitmap;
        }
    }

    private void columnsPrivateCopy_CollectionChanged(object? sender, CollectionChangeEventArgs e)
    {
        if (_columnCollectionChanging)
        {
            return;
        }

        PopulateSelectedColumns();

        if (e.Action == CollectionChangeAction.Add && _selectedColumns is not null)
        {
            _selectedColumns.SelectedIndex = _columnsPrivateCopy.IndexOf(e.Element as DataGridViewColumn);
            ListBoxItem? lbi = _selectedColumns.SelectedItem as ListBoxItem;
            _userAddedColumns![lbi!.DataGridViewColumn] = true;
            _columnsNames![lbi.DataGridViewColumn] = lbi.DataGridViewColumn.Name;
        }

        _formIsDirty = true;
    }

    [SuppressMessage("Microsoft.Performance", "CA1808:AvoidCallsThatBoxValueTypes")]
    private void ColumnTypeChanged(ListBoxItem item, Type newType)
    {
        DataGridViewColumn currentColumn = item.DataGridViewColumn;
        Debug.Assert(typeof(DataGridViewColumn).IsAssignableFrom(newType), "we should only have types that can be assigned to a DataGridViewColumn");
        Debug.Assert(_selectedColumns?.SelectedItem == item, "we must have lost track of what item is in the property grid");

        DataGridViewColumn? newColumn = System.Activator.CreateInstance(newType) as DataGridViewColumn;

        ITypeResolutionService? tr = _liveDataGridView?.Site?.GetService(_iTypeResolutionServiceType) as ITypeResolutionService;
        ComponentDesigner newColumnDesigner = DataGridViewAddColumnDialog.GetComponentDesignerForType(tr, newType)!;

        CopyDataGridViewColumnProperties(currentColumn /*srcColumn*/, newColumn! /*destColumn*/);
        CopyDataGridViewColumnState(currentColumn /*srcColumn*/, newColumn! /*destColumn*/);

        _columnCollectionChanging = true;
        int selectedIndex = _selectedColumns.SelectedIndex;

        // steal the focus away from the PropertyGrid
        _selectedColumns.Focus();
        this.ActiveControl = _selectedColumns;

        try
        {
            // scrub the TypeDescriptor associations
            ListBoxItem lbi = (ListBoxItem)_selectedColumns.SelectedItem;

            bool? userAddedColumn = (bool)_userAddedColumns![lbi.DataGridViewColumn]!;

            string columnSiteName = string.Empty;
            if (_columnsNames!.Contains(lbi.DataGridViewColumn))
            {
                columnSiteName = (string)_columnsNames[lbi.DataGridViewColumn]!;
                _columnsNames.Remove(lbi.DataGridViewColumn);
            }

            if (_userAddedColumns.Contains(lbi.DataGridViewColumn))
            {
                _userAddedColumns.Remove(lbi.DataGridViewColumn);
            }

            if (lbi.DataGridViewColumnDesigner is not null)
            {
                TypeDescriptor.RemoveAssociation(lbi.DataGridViewColumn, lbi.DataGridViewColumnDesigner);
            }

            _selectedColumns.Items.RemoveAt(selectedIndex);
            _selectedColumns.Items.Insert(selectedIndex, new ListBoxItem(newColumn!, this, newColumnDesigner));

            _columnsPrivateCopy.RemoveAt(selectedIndex);
            // wipe out the display index
            newColumn!.DisplayIndex = -1;
            _columnsPrivateCopy.Insert(selectedIndex, newColumn);

            if (!string.IsNullOrEmpty(columnSiteName))
            {
                _columnsNames[newColumn] = columnSiteName;
            }

            _userAddedColumns[newColumn] = userAddedColumn;

            // properties like DataGridViewColumn::Frozen are dependent on the DisplayIndex
            FixColumnCollectionDisplayIndices();

            _selectedColumns.SelectedIndex = selectedIndex;
            _propertyGrid1!.SelectedObject = _selectedColumns.SelectedItem;
        }
        finally
        {
            _columnCollectionChanging = false;
        }
    }

    [SuppressMessage("Microsoft.Performance", "CA1808:AvoidCallsThatBoxValueTypes")]
    private void CommitChanges()
    {
        if (_formIsDirty)
        {
            try
            {
                IComponentChangeService? changeService = _liveDataGridView?.Site?.GetService(_iComponentChangeServiceType) as IComponentChangeService;
                PropertyDescriptor? prop = TypeDescriptor.GetProperties(_liveDataGridView!)["Columns"];
                IContainer? currentContainer = _liveDataGridView?.Site is not null ? _liveDataGridView.Site.Container : null;

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

                DataGridViewColumn[] oldColumns = new DataGridViewColumn[_liveDataGridView!.Columns.Count];
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
                string[] compNames = new string[_columnsPrivateCopy.Count];
                for (int i = 0; i < _columnsPrivateCopy!.Count; i++)
                {
                    DataGridViewColumn newColumn = (DataGridViewColumn)_columnsPrivateCopy[i].Clone();
                    // at design time we need to do a shallow copy for ContextMenuStrip property
                    newColumn.ContextMenuStrip = _columnsPrivateCopy[i].ContextMenuStrip;

                    newColumns[i] = newColumn;
                    object? boolObject = _userAddedColumns![_columnsPrivateCopy[i]];
                    if (boolObject is not null && boolObject is bool boolValue)
                    {
                        userAddedColumnsInfo[i] = boolValue;
                    }

                    if (_columnsNames is not null && _columnsPrivateCopy is not null)
                    {
#pragma warning disable CS8601 // Possible null reference assignment.
                        compNames[i] = _columnsNames[_columnsPrivateCopy[i]] as string;
#pragma warning restore CS8601 // Possible null reference assignment.
                    }
                }

                // 5. IContainer.Add(new dataGridView.Columns)
                if (currentContainer is not null)
                {
                    for (int i = 0; i < newColumns.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(compNames[i]) && ValidateName(currentContainer, compNames[i], newColumns[i]))
                        {
                            currentContainer.Add(newColumns[i], compNames[i]);
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
                    PropertyDescriptor? pd = TypeDescriptor.GetProperties(newColumns[i])["DisplayIndex"];
                    pd?.SetValue(newColumns[i], -1);

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
            catch (System.InvalidOperationException ex)
            {
                IUIService? uiService = _liveDataGridView?.Site?.GetService(typeof(IUIService)) as IUIService;
                DataGridViewDesigner.ShowErrorDialog(uiService, ex, _liveDataGridView);
                this.DialogResult = DialogResult.Cancel;
            }
        }
    }

    private void componentChanged(object? sender, ComponentChangedEventArgs e)
    {
        if (e.Component is ListBoxItem && _selectedColumns!.Items.Contains(e.Component))
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
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
    private static void CopyDefaultCellStyle(DataGridViewColumn srcColumn, DataGridViewColumn destColumn)
    {
        // Here is what we want to do ( see vsw 352177 for more details ):
        // 1. If srcColumn and destColumn have the same type simply copy the default cell style from source to destination
        //  and be done w/ it.
        // 2. Otherwise, determine which properties in the cell style are no longer default and copy those properties.
        //      To do that we need to:
        //      2.a Create a default srcColumn so we get its default cell style. If we get an exception when we are creating the default cell style
        //      then we copy all the public properties.
        //      2.b Go thru the public properties in the DataGridViewCellStyle and copy only the property that are changed from the default values;
        //      2.c We need to special case the DataGridViewCellStyle::NullValue property. This property will be copied only if the NullValue
        //      has the same type in destColumn and in srcColumn.

        Type srcType = srcColumn.GetType();
        Type destType = destColumn.GetType();

        // 1. If srcColumn and destColumn have the same type simply copy the default cell style from source to destination
        //  and be done w/ it.
        if (srcType.IsAssignableFrom(destType) || destType.IsAssignableFrom(srcType))
        {
            destColumn.DefaultCellStyle = srcColumn.DefaultCellStyle;
            return;
        }

        //      2.a Create a default srcColumn so we get its default cell style. If we get an exception when we are creating the default cell style
        //      then we copy all the public properties.
        DataGridViewColumn? defaultSrcColumn = null;
        try
        {
            defaultSrcColumn = System.Activator.CreateInstance(srcType) as DataGridViewColumn;
        }
        catch (Exception e)
        {
            if (ClientUtils.IsCriticalException(e))
            {
                throw;
            }

            defaultSrcColumn = null;
        }

        //      2.b Go thru the public properties in the DataGridViewCellStyle and copy only the property that are changed from the default values;
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

        //      2.c We need to special case the DataGridViewCellStyle::NullValue property. This property will be copied only if the NullValue
        //      has the same type in destColumn and in srcColumn.
        if (!srcColumn.DefaultCellStyle.IsNullValueDefault)
        {
            object srcNullValue = srcColumn.DefaultCellStyle.NullValue;
            object destNullValue = destColumn.DefaultCellStyle.NullValue;

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
            componentChangeService.ComponentChanged += new ComponentChangedEventHandler(this.componentChanged);
        }
    }

    #region Windows Form Designer generated code
    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataGridViewColumnCollectionDialog));
        _addButton = new Button();
        _deleteButton = new Button();
        _moveDown = new Button();
        _moveUp = new Button();
        _selectedColumns = new ListBox();
        _overarchingTableLayoutPanel = new TableLayoutPanel();
        _addRemoveTableLayoutPanel = new TableLayoutPanel();
        _selectedColumnsLabel = new Label();
        _propertyGridLabel = new Label();
        _propertyGrid1 = new VsPropertyGrid(_serviceProvider);
        _okCancelTableLayoutPanel = new TableLayoutPanel();
        _cancelButton = new Button();
        _okButton = new Button();
        _overarchingTableLayoutPanel.SuspendLayout();
        _addRemoveTableLayoutPanel.SuspendLayout();
        _okCancelTableLayoutPanel.SuspendLayout();
        this.SuspendLayout();
        //
        // addButton
        //
        resources.ApplyResources(_addButton, "addButton");
        _addButton.Margin = new Padding(0, 0, 3, 0);
        _addButton.Name = "addButton";
        _addButton.Padding = new Padding(10, 0, 10, 0);
        _addButton.Click += new System.EventHandler(this.addButton_Click);
        //
        // deleteButton
        //
        resources.ApplyResources(_deleteButton, "deleteButton");
        _deleteButton.Margin = new Padding(3, 0, 0, 0);
        _deleteButton.Name = "deleteButton";
        _deleteButton.Padding = new Padding(10, 0, 10, 0);
        _deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
        //
        // moveDown
        //
        resources.ApplyResources(_moveDown, "moveDown");
        _moveDown.Margin = new Padding(0, 1, 18, 0);
        _moveDown.Name = "moveDown";
        _moveDown.Click += new System.EventHandler(this.moveDown_Click);
        //
        // moveUp
        //
        resources.ApplyResources(_moveUp, "moveUp");
        _moveUp.Margin = new Padding(0, 0, 18, 1);
        _moveUp.Name = "moveUp";
        _moveUp.Click += new System.EventHandler(this.moveUp_Click);
        //
        // selectedColumns
        //
        resources.ApplyResources(_selectedColumns, "selectedColumns");
        _selectedColumns.DrawMode = DrawMode.OwnerDrawFixed;
        _selectedColumns.Margin = new Padding(0, 2, 3, 3);
        _selectedColumns.Name = "selectedColumns";
        _overarchingTableLayoutPanel.SetRowSpan(_selectedColumns, 2);
        _selectedColumns.SelectedIndexChanged += new System.EventHandler(this.selectedColumns_SelectedIndexChanged);
        _selectedColumns.KeyPress += new KeyPressEventHandler(this.selectedColumns_KeyPress);
        _selectedColumns.DrawItem += new DrawItemEventHandler(this.selectedColumns_DrawItem);
        _selectedColumns.KeyUp += new KeyEventHandler(this.selectedColumns_KeyUp);
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
        _propertyGrid1.LineColor = System.Drawing.SystemColors.ScrollBar;
        _propertyGrid1.Margin = new Padding(3, 2, 0, 3);
        _propertyGrid1.Name = "propertyGrid1";
        _overarchingTableLayoutPanel.SetRowSpan(_propertyGrid1, 3);
        _propertyGrid1.PropertyValueChanged += new PropertyValueChangedEventHandler(this.propertyGrid1_PropertyValueChanged);
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
        _okButton.Click += new EventHandler(okButton_Click);
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
        HelpButtonClicked += new CancelEventHandler(DataGridViewColumnCollectionDialog_HelpButtonClicked);
        Closed += new EventHandler(DataGridViewColumnCollectionDialog_Closed);
        Load += new EventHandler(DataGridViewColumnCollectionDialog_Load);
        HelpRequested += new HelpEventHandler(DataGridViewColumnCollectionDialog_HelpRequested);
        _overarchingTableLayoutPanel.ResumeLayout(false);
        _overarchingTableLayoutPanel.PerformLayout();
        _addRemoveTableLayoutPanel.ResumeLayout(false);
        _addRemoveTableLayoutPanel.PerformLayout();
        _okCancelTableLayoutPanel.ResumeLayout(false);
        _okCancelTableLayoutPanel.PerformLayout();
        ResumeLayout(false);
    }
    #endregion

    [SuppressMessage("Microsoft.Performance", "CA1808:AvoidCallsThatBoxValueTypes")]
    private static bool IsColumnAddedByUser(DataGridViewColumn col)
    {
        PropertyDescriptor? pd = TypeDescriptor.GetProperties(col)["UserAddedColumn"];
        object? val = pd?.GetValue(col);
        if (pd is not null && val is not null)
        {
            return (bool)val;
        }
        else
        {
            return false;
        }
    }

    private void okButton_Click(object? sender, System.EventArgs e)
    {
        CommitChanges();
    }

    private void moveDown_Click(object? sender, System.EventArgs e)
    {
        int selectedIndex = _selectedColumns!.SelectedIndex;
        Debug.Assert(selectedIndex > -1 && selectedIndex < _selectedColumns.Items.Count - 1);

        _columnCollectionChanging = true;
        try
        {
            ListBoxItem? item = _selectedColumns.SelectedItem as ListBoxItem;
            _selectedColumns.Items.RemoveAt(selectedIndex);
            _selectedColumns.Items.Insert(selectedIndex + 1, item!);

            // now do the same thing to the column collection
            _columnsPrivateCopy.RemoveAt(selectedIndex);

            // if the column we moved was frozen, make sure the column below is frozen too
            if (item!.DataGridViewColumn.Frozen)
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
        _moveUp!.Enabled = _selectedColumns.SelectedIndex > 0;
        _moveDown!.Enabled = _selectedColumns.SelectedIndex < _selectedColumns.Items.Count - 1;
    }

    private void moveUp_Click(object? sender, EventArgs e)
    {
        int selectedIndex = _selectedColumns!.SelectedIndex;
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
        _moveUp!.Enabled = _selectedColumns.SelectedIndex > 0;
        _moveDown!.Enabled = _selectedColumns.SelectedIndex < _selectedColumns.Items.Count - 1;

        // vsw 495403: keep the selected item visible.
        // For some reason, we only have to do this when we move a column up.
        // When we move a column down or when we delete a column, the selected item remains visible.
        if (_selectedColumns.SelectedIndex != -1 && _selectedColumns.TopIndex > _selectedColumns.SelectedIndex)
        {
            _selectedColumns.TopIndex = _selectedColumns.SelectedIndex;
        }
    }

    private void DataGridViewColumnCollectionDialog_Closed(object? sender, System.EventArgs e)
    {
        // scrub the TypeDescriptor association between DataGridViewColumns and their designers
        for (int i = 0; i < _selectedColumns!.Items.Count; i++)
        {
            ListBoxItem? lbi = _selectedColumns.Items[i] as ListBoxItem;
            if (lbi?.DataGridViewColumnDesigner is not null)
            {
                TypeDescriptor.RemoveAssociation(lbi.DataGridViewColumn, lbi.DataGridViewColumnDesigner);
            }
        }

        _columnsNames = null;
        _userAddedColumns = null;
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
        IHelpService? helpService = _liveDataGridView?.Site?.GetService(_iHelpServiceType) as IHelpService;
        helpService?.ShowHelpFromKeyword("vs.DataGridViewColumnCollectionDialog");
    }

    private void DataGridViewColumnCollectionDialog_Load(object? sender, EventArgs e)
    {
        // get the Dialog Font
        //
        Font uiFont = Control.DefaultFont;
        IUIService? uiService = _liveDataGridView?.Site?.GetService(_iUIServiceType) as IUIService;
        object? font = uiService?.Styles["DialogFont"];
        if (uiService is not null && font is Font fontValue)
        {
            uiFont = fontValue;
        }

        this.Font = uiFont;

        // keep the selected index to 0 or -1 if there are no selected columns
        _selectedColumns!.SelectedIndex = Math.Min(0, _selectedColumns.Items.Count - 1);

        _moveUp!.Enabled = _selectedColumns.SelectedIndex > 0;
        _moveDown!.Enabled = _selectedColumns.SelectedIndex < _selectedColumns.Items.Count - 1;
        _deleteButton!.Enabled = _selectedColumns.Items.Count > 0 && _selectedColumns.SelectedIndex != -1;
        _propertyGrid1!.SelectedObject = _selectedColumns.SelectedItem;

        _selectedColumns.ItemHeight = this.Font.Height + _OWNERDRAWVERTICALBUFFER;

        this.ActiveControl = _selectedColumns;

        this.SetSelectedColumnsHorizontalExtent();

        _selectedColumns.Focus();

        _formIsDirty = false;
    }

    private void deleteButton_Click(object? sender, System.EventArgs e)
    {
        Debug.Assert(_selectedColumns!.SelectedIndex != -1);
        int selectedIndex = _selectedColumns.SelectedIndex;

        _columnsNames?.Remove(_columnsPrivateCopy[selectedIndex]);
        _userAddedColumns?.Remove(_columnsPrivateCopy[selectedIndex]);

        _columnsPrivateCopy.RemoveAt(selectedIndex);

        // try to keep the same selected index
        _selectedColumns.SelectedIndex = Math.Min(_selectedColumns.Items.Count - 1, selectedIndex);

        _moveUp!.Enabled = _selectedColumns.SelectedIndex > 0;
        _moveDown!.Enabled = _selectedColumns.SelectedIndex < _selectedColumns.Items.Count - 1;
        _deleteButton!.Enabled = _selectedColumns.Items.Count > 0 && _selectedColumns.SelectedIndex != -1;
        _propertyGrid1!.SelectedObject = _selectedColumns.SelectedItem;
    }

    private void addButton_Click(object? sender, System.EventArgs e)
    {
        int insertIndex;
        if (_selectedColumns!.SelectedIndex == -1)
        {
            insertIndex = _selectedColumns.Items.Count;
        }
        else
        {
            insertIndex = _selectedColumns.SelectedIndex + 1;
        }

        if (_addColumnDialog is null)
        {
            // child modal dialog -launching in System Aware mode
            _addColumnDialog = DpiHelper.CreateInstanceInSystemAwareContext(() => new DataGridViewAddColumnDialog(_columnsPrivateCopy, _liveDataGridView!));
            _addColumnDialog.StartPosition = FormStartPosition.CenterParent;
        }

        _addColumnDialog.Start(insertIndex, false /*persistChangesToDesigner*/);

        _addColumnDialog.ShowDialog(this);
    }

    private void PopulateSelectedColumns()
    {
        int selectedIndex = _selectedColumns!.SelectedIndex;

        // scrub the TypeDescriptor association between DataGridViewColumns and their designers
        for (int i = 0; i < _selectedColumns?.Items.Count; i++)
        {
            ListBoxItem? lbi = _selectedColumns?.Items[i] as ListBoxItem;
            if (lbi?.DataGridViewColumnDesigner is not null)
            {
                TypeDescriptor.RemoveAssociation(lbi.DataGridViewColumn, lbi.DataGridViewColumnDesigner);
            }
        }

        _selectedColumns?.Items.Clear();
        ITypeResolutionService? tr = _liveDataGridView?.Site?.GetService(_iTypeResolutionServiceType) as ITypeResolutionService;

        for (int i = 0; i < _columnsPrivateCopy.Count; i++)
        {
            ComponentDesigner columnDesigner = DataGridViewAddColumnDialog.GetComponentDesignerForType(tr, _columnsPrivateCopy[i].GetType())!;
            _selectedColumns?.Items.Add(new ListBoxItem(_columnsPrivateCopy[i], this, columnDesigner));
        }

        _selectedColumns!.SelectedIndex = Math.Min(selectedIndex, _selectedColumns.Items.Count - 1);

        SetSelectedColumnsHorizontalExtent();

        if (_selectedColumns.Items.Count == 0)
        {
            _propertyGridLabel!.Text = string.Format(SR.DataGridViewProperties);
        }
    }

    private void propertyGrid1_PropertyValueChanged(object? sender, PropertyValueChangedEventArgs e)
    {
        if (!_columnCollectionChanging)
        {
            _formIsDirty = true;
            // refresh the selected columns when the user changed the HeaderText property
            if (e.ChangedItem!.PropertyDescriptor!.Name.Equals("HeaderText"))
            {
                // invalidate the selected index only
                int selectedIndex = _selectedColumns!.SelectedIndex;
                Debug.Assert(selectedIndex != -1, "we forgot to take away the selected object from the property grid");
                Rectangle bounds = new Rectangle(0, selectedIndex * _selectedColumns.ItemHeight, _selectedColumns.Width, _selectedColumns.ItemHeight);
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
                this.SetSelectedColumnsHorizontalExtent();
            }
            else if (e.ChangedItem.PropertyDescriptor.Name.Equals("DataPropertyName"))
            {
                ListBoxItem? listBoxItem = _selectedColumns?.SelectedItem as ListBoxItem;
                DataGridViewColumn? col = listBoxItem?.DataGridViewColumn;

                if (string.IsNullOrEmpty(col?.DataPropertyName))
                {
                    _propertyGridLabel!.Text = (SR.DataGridViewUnboundColumnProperties);
                }
                else
                {
                    _propertyGridLabel!.Text = (SR.DataGridViewBoundColumnProperties);
                }
            }
            else if (e.ChangedItem.PropertyDescriptor.Name.Equals("Name"))
            {
                ListBoxItem? listBoxItem = _selectedColumns?.SelectedItem as ListBoxItem;
                DataGridViewColumn? col = listBoxItem?.DataGridViewColumn;
                if (_columnsNames is not null && col is not null)
                {
                    _columnsNames[col] = col.Name;
                }
            }
        }
    }

    private void selectedColumns_DrawItem(object? sender, DrawItemEventArgs e)
    {
        if (e.Index < 0)
        {
            return;
        }

        ListBoxItem? lbi = _selectedColumns?.Items[e.Index] as ListBoxItem;

#if DGV_DITHERING
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                ImageAttributes attr = new ImageAttributes();

                colorMap[0].OldColor = Color.White;
                colorMap[0].NewColor = e.BackColor;

                // 
                // TO DO : DITHER
                //
                attr.SetRemapTable(colorMap, ColorAdjustType.Bitmap);

                Rectangle imgRectangle = new Rectangle(e.Bounds.X + OWNERDRAWITEMIMAGEBUFFER, e.Bounds.Y + OWNERDRAWITEMIMAGEBUFFER, lbi.ToolboxBitmap.Width, lbi.ToolboxBitmap.Height);
                e.Graphics.DrawImage(lbi.ToolboxBitmap,
                                     imgRectangle,
                                     0,
                                     0,
                                     imgRectangle.Width,
                                     imgRectangle.Height,
                                     GraphicsUnit.Pixel,
                                     attr);
                attr.Dispose();
            }
            else
            {
#endif // DGV_DITHERING
        e.Graphics.DrawImage(lbi!.ToolboxBitmap,
                             e.Bounds.X + _OWNERDRAWITEMIMAGEBUFFER,
                             e.Bounds.Y + _OWNERDRAWITEMIMAGEBUFFER,
                             lbi.ToolboxBitmap.Width,
                             lbi.ToolboxBitmap.Height);

        Rectangle bounds = e.Bounds;
        bounds.Width -= lbi.ToolboxBitmap.Width + 2 * _OWNERDRAWITEMIMAGEBUFFER;
        bounds.X += lbi.ToolboxBitmap.Width + 2 * _OWNERDRAWITEMIMAGEBUFFER;
        bounds.Y += _OWNERDRAWITEMIMAGEBUFFER;
        bounds.Height -= 2 * _OWNERDRAWITEMIMAGEBUFFER;

        Brush selectedBrush = new SolidBrush(e.BackColor);
        Brush foreBrush = new SolidBrush(e.ForeColor);
        Brush backBrush = new SolidBrush(_selectedColumns!.BackColor);

        string columnName = ((ListBoxItem)_selectedColumns.Items[e.Index]).ToString();

        if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
        {
            // first get the text rectangle
            int textWidth = Size.Ceiling(e.Graphics.MeasureString(columnName, e.Font!, new SizeF(bounds.Width, bounds.Height))).Width;
            // DANIELHE: the spec calls for + 7 but I think that + 3 does the trick better
            Rectangle focusRectangle = new Rectangle(bounds.X, e.Bounds.Y + 1, textWidth + _OWNERDRAWHORIZONTALBUFFER, e.Bounds.Height - 2);

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
            _propertyGrid1?.Focus();
            e.Handled = true;
        }
    }

    private void selectedColumns_KeyPress(object? sender, KeyPressEventArgs e)
    {
        Keys modifierKeys = Control.ModifierKeys;

        // vsw 479960.
        // Don't let Ctrl-* propagate to the selected columns list box.
        if ((modifierKeys & Keys.Control) != 0)
        {
            e.Handled = true;
        }
    }

    private void selectedColumns_SelectedIndexChanged(object? sender, System.EventArgs e)
    {
        if (_columnCollectionChanging)
        {
            return;
        }

        _propertyGrid1!.SelectedObject = _selectedColumns!.SelectedItem;

        // enable/disable up/down/delete buttons
        _moveDown!.Enabled = _selectedColumns.Items.Count > 0 && _selectedColumns.SelectedIndex != _selectedColumns.Items.Count - 1;
        _moveUp!.Enabled = _selectedColumns.Items.Count > 0 && _selectedColumns.SelectedIndex > 0;
        _deleteButton!.Enabled = _selectedColumns.Items.Count > 0 && _selectedColumns.SelectedIndex != -1;

        if (_selectedColumns.SelectedItem is not null)
        {
            DataGridViewColumn column = ((ListBoxItem)_selectedColumns.SelectedItem).DataGridViewColumn;
            if (string.IsNullOrEmpty(column.DataPropertyName))
            {
                _propertyGridLabel!.Text = (SR.DataGridViewUnboundColumnProperties);
            }
            else
            {
                _propertyGridLabel!.Text = (SR.DataGridViewBoundColumnProperties);
            }
        }
        else
        {
            _propertyGridLabel!.Text = (SR.DataGridViewProperties);
        }
    }

    internal void SetLiveDataGridView(DataGridView dataGridView)
    {
        IComponentChangeService? newComponentChangeService = null;
        if (dataGridView.Site is not null)
        {
            newComponentChangeService = dataGridView.Site.GetService(_iComponentChangeServiceType) as IComponentChangeService;
        }

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
        _columnsNames = new Hashtable(_columnsPrivateCopy.Count);
        _columnsPrivateCopy.Clear();

        _userAddedColumns = new Hashtable(_liveDataGridView.Columns.Count);

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

                _userAddedColumns[col] = IsColumnAddedByUser(_liveDataGridView.Columns[i]);
            }
        }
        finally
        {
            _columnCollectionChanging = false;
        }

        PopulateSelectedColumns();

        _propertyGrid1!.Site = new DataGridViewComponentPropertyGridSite(_liveDataGridView.Site, _liveDataGridView);

        _propertyGrid1.SelectedObject = _selectedColumns!.SelectedItem;
    }

    private void SetSelectedColumnsHorizontalExtent()
    {
        int maxItemWidth = 0;
        for (int i = 0; i < _selectedColumns!.Items.Count; i++)
        {
            int itemWidth = TextRenderer.MeasureText(_selectedColumns.Items[i].ToString(), _selectedColumns.Font).Width;
            maxItemWidth = Math.Max(maxItemWidth, itemWidth);
        }

        _selectedColumns.HorizontalExtent = SelectedColumnsItemBitmap.Width + 2 * _OWNERDRAWITEMIMAGEBUFFER + maxItemWidth + _OWNERDRAWHORIZONTALBUFFER;
    }

    private void UnhookComponentChangedEventHandler(IComponentChangeService componentChangeService)
    {
        if (componentChangeService is not null)
        {
            componentChangeService.ComponentChanged -= new ComponentChangedEventHandler(this.componentChanged);
        }
    }

    private static bool ValidateName(IContainer container, string siteName, IComponent component)
    {
        ComponentCollection comps = container.Components;
        if (comps is null)
        {
            return true;
        }

        for (int i = 0; i < comps.Count; i++)
        {
            IComponent? comp = comps[i];
            if (comp is null || comp.Site is null)
            {
                continue;
            }

            ISite s = comp.Site;

            if (s is not null && s.Name is not null && string.Equals(s.Name, siteName, StringComparison.OrdinalIgnoreCase) && s.Component != component)
            {
                return false;
            }
        }

        return true;
    }

    // internal because the DataGridViewColumnDataPropertyNameEditor needs to get at the ListBoxItem
    // IComponent because some editors for some dataGridViewColumn properties - DataGridViewComboBox::DataSource editor -
    // need the site
    internal class ListBoxItem : ICustomTypeDescriptor, IComponent
    {
        private DataGridViewColumn _column;
        private DataGridViewColumnCollectionDialog _owner;
        private ComponentDesigner? _compDesigner;
        private Image? _toolboxBitmap;
        public ListBoxItem(DataGridViewColumn column, DataGridViewColumnCollectionDialog owner, ComponentDesigner compDesigner)
        {
            _column = column;
            _owner = owner;
            _compDesigner = compDesigner;

            if (_compDesigner is not null)
            {
                _compDesigner.Initialize(column);
                TypeDescriptor.CreateAssociation(_column, _compDesigner);
            }

#pragma warning disable IL2077 // Target parameter argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method. The source field does not have matching annotations.
            ToolboxBitmapAttribute? attr = TypeDescriptor.GetAttributes(column!)[_toolboxBitmapAttributeType!] as ToolboxBitmapAttribute;
#pragma warning restore IL2077 // Target parameter argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method. The source field does not have matching annotations.
            if (attr is not null)
            {
                _toolboxBitmap = attr.GetImage(column, false /*large*/);
            }
            else
            {
                _toolboxBitmap = SelectedColumnsItemBitmap;
            }

            DataGridViewColumnDesigner? dgvColumnDesigner = compDesigner as DataGridViewColumnDesigner;
            if (dgvColumnDesigner is not null && _owner._liveDataGridView is not null)
            {
                dgvColumnDesigner.LiveDataGridView = _owner._liveDataGridView;
            }
        }

        public DataGridViewColumn DataGridViewColumn
        {
            get
            {
                return _column;
            }
        }

        public ComponentDesigner DataGridViewColumnDesigner
        {
            get
            {
                return this._compDesigner!;
            }
        }

        public DataGridViewColumnCollectionDialog Owner
        {
            get
            {
                return this._owner;
            }
        }

        public Image ToolboxBitmap
        {
            get
            {
                return this._toolboxBitmap!;
            }
        }

        public override string ToString()
        {
            return this._column.HeaderText;
        }

        // ICustomTypeDescriptor implementation
        AttributeCollection ICustomTypeDescriptor.GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this._column);
        }

        string? ICustomTypeDescriptor.GetClassName()
        {
            return TypeDescriptor.GetClassName(this._column);
        }

        string? ICustomTypeDescriptor.GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this._column);
        }

        TypeConverter ICustomTypeDescriptor.GetConverter()
        {
            return TypeDescriptor.GetConverter(this._column);
        }

        EventDescriptor? ICustomTypeDescriptor.GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this._column);
        }

        PropertyDescriptor? ICustomTypeDescriptor.GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this._column);
        }

        object? ICustomTypeDescriptor.GetEditor(Type type)
        {
            return TypeDescriptor.GetEditor(this._column, type);
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
        {
            return TypeDescriptor.GetEvents(this._column);
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[]? attrs)
        {
            return TypeDescriptor.GetEvents(this._column, attrs!);
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        {
            return (((ICustomTypeDescriptor)this).GetProperties(null));
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[]? attrs)
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(this._column);

            PropertyDescriptor[]? propArray = null;
            if (this._compDesigner is not null)
            {
                // PropertyDescriptorCollection does not let us change properties.
                // So we have to create a hash table that we pass to PreFilterProperties
                // and then copy back the result from PreFilterProperties

                // We should look into speeding this up w/ our own DataGridViewColumnTypes...
                //
                Hashtable hash = new Hashtable();
                for (int i = 0; i < props.Count; i++)
                {
                    hash.Add(props[i].Name, props[i]);
                }

                ((IDesignerFilter)_compDesigner).PreFilterProperties(hash);

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

            propArray[propArray.Length - 1] = new ColumnTypePropertyDescriptor();

            return new PropertyDescriptorCollection(propArray);
        }

        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor? pd)
        {
            if (pd is null)
            {
                return this._column;
            }
            else if (pd is ColumnTypePropertyDescriptor)
            {
                return this;
            }
            else
            {
                return this._column;
            }
        }

        ISite? IComponent.Site
        {
            get
            {
                return this._owner._liveDataGridView?.Site;
            }
            set
            {
            }
        }

        event EventHandler? IComponent.Disposed
        {
            add
            {
            }
            remove
            {
            }
        }

        [
            SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed")      // The ListBoxItem does not own the ToolBoxBitmap
                                                                                               // so it can't dispose it.
        ]
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
                EditorAttribute editorAttr = new EditorAttribute("Design.DataGridViewColumnTypeEditor, " + AssemblyRef.SystemDesign, typeof(System.Drawing.Design.UITypeEditor));
                DescriptionAttribute descriptionAttr = new DescriptionAttribute((SR.DataGridViewColumnTypePropertyDescription));
                CategoryAttribute categoryAttr = CategoryAttribute.Design;
                // add the description attribute and the categories attribute
                Attribute[] attrs = new Attribute[] { editorAttr, descriptionAttr, categoryAttr };
                return new AttributeCollection(attrs);
            }
        }

        public override Type ComponentType
        {
            get
            {
                return typeof(ListBoxItem);
            }
        }

        public override bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public override Type PropertyType
        {
            get
            {
                return typeof(Type);
            }
        }

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
