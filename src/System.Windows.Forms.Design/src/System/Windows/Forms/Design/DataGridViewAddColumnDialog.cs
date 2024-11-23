// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel.Design.Serialization;
using System.ComponentModel.Design;
using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms.Design;

internal class DataGridViewAddColumnDialog : Form
{
    private RadioButton _dataBoundColumnRadioButton;
    private Label _columnInDataSourceLabel;

    private ListBox _dataColumns;

    private RadioButton _unboundColumnRadioButton;
    private TextBox _nameTextBox;

    private ComboBox _columnTypesCombo;

    private TextBox _headerTextBox;
    private Label _nameLabel;

    private Label _typeLabel;

    private Label _headerTextLabel;
    private CheckBox _visibleCheckBox;

    private CheckBox _readOnlyCheckBox;

    private CheckBox _frozenCheckBox;
    private Button _addButton;

    private Button _cancelButton;

    private readonly DataGridViewColumnCollection _dataGridViewColumns;
    private readonly DataGridView _liveDataGridView;
    private int _insertAtPosition = -1;
    private int _initialDataGridViewColumnsCount = -1;
    private bool _persistChangesToDesigner;

    private TableLayoutPanel _okCancelTableLayoutPanel;
    private TableLayoutPanel _checkBoxesTableLayoutPanel;
    private TableLayoutPanel _overarchingTableLayoutPanel;

    public DataGridViewAddColumnDialog(DataGridViewColumnCollection dataGridViewColumns, DataGridView liveDataGridView)
    {
        _dataGridViewColumns = dataGridViewColumns;
        _liveDataGridView = liveDataGridView;

        // PERF: set the Dialog Font before InitializeComponent.
        Font uiFont = DefaultFont;
        if (_liveDataGridView.Site.TryGetService(out IUIService? uiService))
        {
            uiFont = (Font)uiService.Styles["DialogFont"]!;
        }

        Font = uiFont;

        //
        // Required for Windows Form Designer support
        //
        InitializeComponent();

        // put this code in the constructor because we want to keep the dataBoundColumnRadioButton.Checked state
        // when the user closes and reopens the add column dialog
        EnableDataBoundSection();
    }

    /// <summary>
    ///  consolidate all the information from the dialog into a data grid view column
    ///  and insert it in the data grid view column collection
    /// </summary>
    private void AddColumn()
    {
        ComboBoxItem? comboBoxItem = (ComboBoxItem)_columnTypesCombo.SelectedItem!;
        Type columnType = comboBoxItem.ColumnType;

        DataGridViewColumn column = (DataGridViewColumn)Activator.CreateInstance(columnType)!;

        // if we want to insert a column before a frozen column then make sure that we insert a frozen column
        bool forceColumnFrozen = _dataGridViewColumns.Count > _insertAtPosition && _dataGridViewColumns[_insertAtPosition].Frozen;

        column.Frozen = forceColumnFrozen;

        // if we don't persist changes to the designer then we want to add the columns before
        // setting the Frozen bit
        if (!_persistChangesToDesigner)
        {
            // set the header text because the DataGridViewColumnCollection needs it to compute
            // its listbox'x HorizontalOffset
            column.HeaderText = _headerTextBox.Text;
            column.Name = _nameTextBox.Text;
            column.DisplayIndex = -1;
            _dataGridViewColumns.Insert(_insertAtPosition, column);
            _insertAtPosition++;
        }

        // if we persist changes directly to the designer then:
        // 1. set the column property values
        // 2. Add the new column
        // 3. site the column

        // 1. set the property values in the column
        column.HeaderText = _headerTextBox.Text;
        column.Name = _nameTextBox.Text;
        column.Visible = _visibleCheckBox.Checked;
        column.Frozen = _frozenCheckBox.Checked || forceColumnFrozen;
        column.ReadOnly = _readOnlyCheckBox.Checked;

        if (_dataBoundColumnRadioButton.Checked && _dataColumns.SelectedIndex > -1)
        {
            column.DataPropertyName = ((ListBoxItem)_dataColumns.SelectedItem!).PropertyName;
        }

        if (_persistChangesToDesigner)
        {
            try
            {
                // insert the column before siting the column.
                // if DataGridView throws an exception then the designer does not end up w/ a column that is not in any data grid view.
                column.DisplayIndex = -1;
                _dataGridViewColumns.Insert(_insertAtPosition, column);
                _insertAtPosition++;
                // site the column
                _liveDataGridView.Site?.Container?.Add(column, column.Name);
            }
            catch (InvalidOperationException ex)
            {
                IUIService? uiService = _liveDataGridView.Site?.GetService<IUIService>();
                DataGridViewDesigner.ShowErrorDialog(uiService, ex, _liveDataGridView);
                return;
            }
        }

        // Set the UserAddedColumn property to true.
        PropertyDescriptorCollection props = TypeDescriptor.GetProperties(column);
        PropertyDescriptor? pd = props["UserAddedColumn"];
        pd?.SetValue(column, true);

        // pick a new Column name
        _nameTextBox.Text = _headerTextBox.Text = AssignName();
        _nameTextBox.Focus();
    }

    private string AssignName()
    {
        int colId = 1;
        // string columnName = (SR.DataGridView_ColumnName, colId.ToString());
        string columnName;

        IDesignerHost? host = _liveDataGridView.Site?.GetService<IDesignerHost>();
        IContainer? container = host?.Container;

        do
        {
            columnName = $"Column{colId++}";
        }
        while (!ValidName(columnName,
                            _dataGridViewColumns,
                            container,
                            nameCreationService: null,
                            _liveDataGridView.Columns,
                            !_persistChangesToDesigner));

        return columnName;
    }

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    protected override void Dispose(bool disposing) => base.Dispose(disposing);

    private void EnableDataBoundSection()
    {
        bool dataGridViewIsDataBound = _dataColumns.Items.Count > 0;
        if (dataGridViewIsDataBound)
        {
            _dataBoundColumnRadioButton.Enabled = true;
            _dataBoundColumnRadioButton.Checked = true;
            _dataBoundColumnRadioButton.Focus();
        }
        else
        {
            _dataBoundColumnRadioButton.Enabled = false;
            _unboundColumnRadioButton.Checked = true;
            _nameTextBox.Focus();
        }

        _headerTextBox.Text = _nameTextBox.Text = AssignName();
    }

    /// <summary>
    ///  public because the EditColumns dialog wants to use it as well.
    /// </summary>
    /// <param name="typeResolutionService"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static ComponentDesigner? GetComponentDesignerForType(ITypeResolutionService? typeResolutionService, Type type)
    {
        ComponentDesigner? compDesigner = null;
        DesignerAttribute? designerAttribute = null;
        AttributeCollection? attributes = TypeDescriptor.GetAttributes(type);
        for (int i = 0; i < attributes.Count; i++)
        {
            if (attributes[i] is DesignerAttribute attribute)
            {
                Type? daType = Type.GetType(attribute.DesignerBaseTypeName);
                if (daType == typeof(IDesigner))
                {
                    designerAttribute = attribute;
                    break;
                }
            }
        }

        if (designerAttribute is not null)
        {
            Type? designerType = typeResolutionService is not null
                ? typeResolutionService.GetType(designerAttribute.DesignerTypeName)
                : Type.GetType(designerAttribute.DesignerTypeName);

            if (designerType is not null && typeof(ComponentDesigner).IsAssignableFrom(designerType))
            {
                compDesigner = Activator.CreateInstance(designerType) as ComponentDesigner;
            }
        }

        return compDesigner;
    }

    #region Windows Form Designer generated code
    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    [MemberNotNull(nameof(_dataBoundColumnRadioButton))]
    [MemberNotNull(nameof(_overarchingTableLayoutPanel))]
    [MemberNotNull(nameof(_checkBoxesTableLayoutPanel))]
    [MemberNotNull(nameof(_frozenCheckBox))]
    [MemberNotNull(nameof(_visibleCheckBox))]
    [MemberNotNull(nameof(_readOnlyCheckBox))]
    [MemberNotNull(nameof(_okCancelTableLayoutPanel))]
    [MemberNotNull(nameof(_addButton))]
    [MemberNotNull(nameof(_cancelButton))]
    [MemberNotNull(nameof(_columnInDataSourceLabel))]
    [MemberNotNull(nameof(_dataColumns))]
    [MemberNotNull(nameof(_unboundColumnRadioButton))]
    [MemberNotNull(nameof(_nameLabel))]
    [MemberNotNull(nameof(_nameTextBox))]
    [MemberNotNull(nameof(_typeLabel))]
    [MemberNotNull(nameof(_columnTypesCombo))]
    [MemberNotNull(nameof(_headerTextLabel))]
    [MemberNotNull(nameof(_headerTextBox))]
    private void InitializeComponent()
    {
        ComponentResourceManager resources = new(typeof(DataGridViewAddColumnDialog));
        _dataBoundColumnRadioButton = new RadioButton();
        _overarchingTableLayoutPanel = new TableLayoutPanel();
        _checkBoxesTableLayoutPanel = new TableLayoutPanel();
        _frozenCheckBox = new CheckBox();
        _visibleCheckBox = new CheckBox();
        _readOnlyCheckBox = new CheckBox();
        _okCancelTableLayoutPanel = new TableLayoutPanel();
        _addButton = new Button();
        _cancelButton = new Button();
        _columnInDataSourceLabel = new Label();
        _dataColumns = new ListBox();
        _unboundColumnRadioButton = new RadioButton();
        _nameLabel = new Label();
        _nameTextBox = new TextBox();
        _typeLabel = new Label();
        _columnTypesCombo = new ComboBox();
        _headerTextLabel = new Label();
        _headerTextBox = new TextBox();
        _overarchingTableLayoutPanel.SuspendLayout();
        _checkBoxesTableLayoutPanel.SuspendLayout();
        _okCancelTableLayoutPanel.SuspendLayout();
        SuspendLayout();
        //
        // dataBoundColumnRadioButton
        //
        resources.ApplyResources(_dataBoundColumnRadioButton, "dataBoundColumnRadioButton");
        _dataBoundColumnRadioButton.Checked = true;
        _overarchingTableLayoutPanel.SetColumnSpan(_dataBoundColumnRadioButton, 3);
        _dataBoundColumnRadioButton.Margin = new Padding(0);
        _dataBoundColumnRadioButton.Name = "dataBoundColumnRadioButton";
        _dataBoundColumnRadioButton.CheckedChanged += dataBoundColumnRadioButton_CheckedChanged;
        //
        // overarchingTableLayoutPanel
        //
        resources.ApplyResources(_overarchingTableLayoutPanel, "overarchingTableLayoutPanel");
        _overarchingTableLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _overarchingTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 14F));
        _overarchingTableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
        _overarchingTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 250F));
        _overarchingTableLayoutPanel.Controls.Add(_checkBoxesTableLayoutPanel, 0, 10);
        _overarchingTableLayoutPanel.Controls.Add(_okCancelTableLayoutPanel, 2, 11);
        _overarchingTableLayoutPanel.Controls.Add(_dataBoundColumnRadioButton, 0, 0);
        _overarchingTableLayoutPanel.Controls.Add(_columnInDataSourceLabel, 1, 1);
        _overarchingTableLayoutPanel.Controls.Add(_dataColumns, 1, 2);
        _overarchingTableLayoutPanel.Controls.Add(_unboundColumnRadioButton, 0, 4);
        _overarchingTableLayoutPanel.Controls.Add(_nameLabel, 1, 5);
        _overarchingTableLayoutPanel.Controls.Add(_nameTextBox, 2, 5);
        _overarchingTableLayoutPanel.Controls.Add(_typeLabel, 1, 7);
        _overarchingTableLayoutPanel.Controls.Add(_columnTypesCombo, 2, 7);
        _overarchingTableLayoutPanel.Controls.Add(_headerTextLabel, 1, 9);
        _overarchingTableLayoutPanel.Controls.Add(_headerTextBox, 2, 9);
        _overarchingTableLayoutPanel.Margin = new Padding(12, 12, 12, 13);
        _overarchingTableLayoutPanel.Name = "overarchingTableLayoutPanel";
        _overarchingTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 22F));
        _overarchingTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 16F));
        _overarchingTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 91F));
        _overarchingTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 12F));
        _overarchingTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 22F));
        _overarchingTableLayoutPanel.RowStyles.Add(new RowStyle());
        _overarchingTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 4F));
        _overarchingTableLayoutPanel.RowStyles.Add(new RowStyle());
        _overarchingTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 4F));
        _overarchingTableLayoutPanel.RowStyles.Add(new RowStyle());
        _overarchingTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 27F));
        _overarchingTableLayoutPanel.RowStyles.Add(new RowStyle());
        //
        // checkBoxesTableLayoutPanel
        //
        resources.ApplyResources(_checkBoxesTableLayoutPanel, "checkBoxesTableLayoutPanel");
        _checkBoxesTableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
        _checkBoxesTableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
        _checkBoxesTableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
        _checkBoxesTableLayoutPanel.Controls.Add(_frozenCheckBox, 2, 0);
        _checkBoxesTableLayoutPanel.Controls.Add(_visibleCheckBox, 0, 0);
        _checkBoxesTableLayoutPanel.Controls.Add(_readOnlyCheckBox, 1, 0);
        _checkBoxesTableLayoutPanel.Margin = new Padding(0);
        _overarchingTableLayoutPanel.SetColumnSpan(_checkBoxesTableLayoutPanel, 3);
        _checkBoxesTableLayoutPanel.Name = "checkBoxesTableLayoutPanel";
        _checkBoxesTableLayoutPanel.RowStyles.Add(new RowStyle());
        //
        // frozenCheckBox
        //
        resources.ApplyResources(_frozenCheckBox, "frozenCheckBox");
        _frozenCheckBox.Margin = new Padding(0);
        _frozenCheckBox.Name = "frozenCheckBox";
        //
        // visibleCheckBox
        //
        resources.ApplyResources(_visibleCheckBox, "visibleCheckBox");
        _visibleCheckBox.Checked = true;
        _visibleCheckBox.CheckState = CheckState.Checked;
        _visibleCheckBox.Margin = new Padding(0);
        _visibleCheckBox.Name = "visibleCheckBox";
        //
        // readOnlyCheckBox
        //
        resources.ApplyResources(_readOnlyCheckBox, "readOnlyCheckBox");
        _readOnlyCheckBox.Margin = new Padding(0);
        _readOnlyCheckBox.Name = "readOnlyCheckBox";
        //
        // okCancelTableLayoutPanel
        //
        resources.ApplyResources(_okCancelTableLayoutPanel, "okCancelTableLayoutPanel");
        _okCancelTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        _okCancelTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        _okCancelTableLayoutPanel.Controls.Add(_addButton, 0, 0);
        _okCancelTableLayoutPanel.Controls.Add(_cancelButton, 1, 0);
        _okCancelTableLayoutPanel.Margin = new Padding(0, 6, 0, 0);
        _okCancelTableLayoutPanel.Name = "okCancelTableLayoutPanel";
        _okCancelTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        //
        // addButton
        //
        resources.ApplyResources(_addButton, "addButton");
        _addButton.Margin = new Padding(0, 0, 3, 0);
        _addButton.Name = "addButton";
        _addButton.Click += addButton_Click;
        //
        // cancelButton
        //
        resources.ApplyResources(_cancelButton, "cancelButton");
        _cancelButton.Margin = new Padding(3, 0, 0, 0);
        _cancelButton.Name = "cancelButton";
        _cancelButton.Click += cancelButton_Click;
        //
        // columnInDataSourceLabel
        //
        resources.ApplyResources(_columnInDataSourceLabel, "columnInDataSourceLabel");
        _overarchingTableLayoutPanel.SetColumnSpan(_columnInDataSourceLabel, 2);
        _columnInDataSourceLabel.Margin = new Padding(0);
        _columnInDataSourceLabel.Name = "columnInDataSourceLabel";
        //
        // dataColumns
        //
        resources.ApplyResources(_dataColumns, "dataColumns");
        _overarchingTableLayoutPanel.SetColumnSpan(_dataColumns, 2);
        _dataColumns.FormattingEnabled = true;
        _dataColumns.Margin = new Padding(0);
        _dataColumns.Name = "dataColumns";

        _dataColumns.SelectedIndexChanged += dataColumns_SelectedIndexChanged;
        //
        // unboundColumnRadioButton
        //
        resources.ApplyResources(_unboundColumnRadioButton, "unboundColumnRadioButton");
        _overarchingTableLayoutPanel.SetColumnSpan(_unboundColumnRadioButton, 3);
        _unboundColumnRadioButton.Margin = new Padding(0);
        _unboundColumnRadioButton.Name = "unboundColumnRadioButton";
        _unboundColumnRadioButton.CheckedChanged += unboundColumnRadioButton_CheckedChanged;
        //
        // nameLabel
        //
        resources.ApplyResources(_nameLabel, "nameLabel");
        _nameLabel.Margin = new Padding(0);
        _nameLabel.Name = "nameLabel";
        //
        // nameTextBox
        //
        resources.ApplyResources(_nameTextBox, "nameTextBox");
        _nameTextBox.Margin = new Padding(0);
        _nameTextBox.Name = "nameTextBox";
        _nameTextBox.Validating += nameTextBox_Validating;
        //
        // typeLabel
        //
        resources.ApplyResources(_typeLabel, "typeLabel");
        _typeLabel.Margin = new Padding(0);
        _typeLabel.Name = "typeLabel";
        //
        // columnTypesCombo
        //
        resources.ApplyResources(_columnTypesCombo, "columnTypesCombo");
        _columnTypesCombo.DropDownStyle = ComboBoxStyle.DropDownList;
        _columnTypesCombo.FormattingEnabled = true;
        _columnTypesCombo.Margin = new Padding(0);
        _columnTypesCombo.Name = "columnTypesCombo";
        _columnTypesCombo.Sorted = true;
        //
        // headerTextLabel
        //
        resources.ApplyResources(_headerTextLabel, "headerTextLabel");
        _headerTextLabel.Margin = new Padding(0);
        _headerTextLabel.Name = "headerTextLabel";
        //
        // headerTextBox
        //
        resources.ApplyResources(_headerTextBox, "headerTextBox");
        _headerTextBox.Margin = new Padding(0);
        _headerTextBox.Name = "headerTextBox";
        //
        // DataGridViewAddColumnDialog
        //
        resources.ApplyResources(this, "$this");
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = _cancelButton;
        AutoSizeMode = AutoSizeMode.GrowAndShrink;
        Controls.Add(_overarchingTableLayoutPanel);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        HelpButton = true;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "DataGridViewAddColumnDialog";
        ShowIcon = false;
        ShowInTaskbar = false;
        HelpButtonClicked += DataGridViewAddColumnDialog_HelpButtonClicked;
        FormClosed += DataGridViewAddColumnDialog_Closed;
        VisibleChanged += DataGridViewAddColumnDialog_VisibleChanged;
        Load += DataGridViewAddColumnDialog_Load;
        HelpRequested += DataGridViewAddColumnDialog_HelpRequested;
        _overarchingTableLayoutPanel.ResumeLayout(false);
        _overarchingTableLayoutPanel.PerformLayout();
        _checkBoxesTableLayoutPanel.ResumeLayout(false);
        _checkBoxesTableLayoutPanel.PerformLayout();
        _okCancelTableLayoutPanel.ResumeLayout(false);
        _okCancelTableLayoutPanel.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }
    #endregion

    private void dataBoundColumnRadioButton_CheckedChanged(object? sender, EventArgs e)
    {
        _columnInDataSourceLabel.Enabled = _dataBoundColumnRadioButton.Checked;
        _dataColumns.Enabled = _dataBoundColumnRadioButton.Checked;

        // push the property name into the headerTextBox and into the nameTextBox
        dataColumns_SelectedIndexChanged(null, EventArgs.Empty);
    }

    private void dataColumns_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (_dataColumns.SelectedItem is null)
        {
            return;
        }

        _headerTextBox.Text = _nameTextBox.Text = ((ListBoxItem)_dataColumns.SelectedItem).PropertyName;

        // pick a default data grid view column type
        // NOTE: this will pick one of our data grid view column types
        SetDefaultDataGridViewColumnType(((ListBoxItem)_dataColumns.SelectedItem).PropertyType);
    }

    private void unboundColumnRadioButton_CheckedChanged(object? sender, EventArgs e)
    {
        if (_unboundColumnRadioButton.Checked)
        {
            _nameTextBox.Text = _headerTextBox.Text = AssignName();
            _nameTextBox.Focus();
        }
    }

    private void DataGridViewAddColumnDialog_Closed(object? sender, EventArgs e)
    {
        if (_persistChangesToDesigner)
        {
            try
            {
                Debug.Assert(_initialDataGridViewColumnsCount != -1, "did you forget to set the initialDataGridViewColumnsCount when you started the dialog?");
                if (!_liveDataGridView.Site.TryGetService(out IComponentChangeService? changeService))
                {
                    return;
                }

                // to get good Undo/Redo we need to bring the data grid view column collection
                // back to its initial state before firing the componentChanging event
                DataGridViewColumn[] cols = new DataGridViewColumn[_liveDataGridView.Columns.Count - _initialDataGridViewColumnsCount];
                for (int i = _initialDataGridViewColumnsCount; i < _liveDataGridView.Columns.Count; i++)
                {
                    cols[i - _initialDataGridViewColumnsCount] = _liveDataGridView.Columns[i];
                }

                for (int i = _initialDataGridViewColumnsCount; i < _liveDataGridView.Columns.Count;)
                {
                    _liveDataGridView.Columns.RemoveAt(_initialDataGridViewColumnsCount);
                }

                // the data grid view column collection is back to its initial state
                // fire the component changing event
                PropertyDescriptor? prop = TypeDescriptor.GetProperties(_liveDataGridView)["Columns"];
                changeService.OnComponentChanging(_liveDataGridView, prop);

                // add back the data grid view columns the user added using the Add Columns dialog
                // But first wipe out the existing display index.
                for (int i = 0; i < cols.Length; i++)
                {
                    cols[i].DisplayIndex = -1;
                }

                _liveDataGridView.Columns.AddRange(cols);

                // fire component changed event
                changeService.OnComponentChanged(_liveDataGridView, prop, null, null);
            }
            catch (InvalidOperationException)
            {
            }
        }
#if DEBUG
        else
        {
            Debug.Assert(_initialDataGridViewColumnsCount == -1, "did you forget to reset the _initialDataGridViewColumnsCount when you started the dialog?");
        }
#endif // DEBUG

        // The DialogResult is OK.
        DialogResult = DialogResult.OK;
    }

    private void DataGridViewAddColumnDialog_HelpButtonClicked(object? sender, CancelEventArgs e)
    {
        DataGridViewAddColumnDialog_HelpRequestHandled();
        e.Cancel = true;
    }

    private void DataGridViewAddColumnDialog_HelpRequested(object? sender, HelpEventArgs e)
    {
        DataGridViewAddColumnDialog_HelpRequestHandled();
        e.Handled = true;
    }

    private void DataGridViewAddColumnDialog_HelpRequestHandled()
    {
        IHelpService? helpService = _liveDataGridView.Site?.GetService<IHelpService>();
        helpService?.ShowHelpFromKeyword("vs.DataGridViewAddColumnDialog");
    }

    private void DataGridViewAddColumnDialog_Load(object? sender, EventArgs e)
    {
        // setup Visible/ReadOnly/Frozen checkboxes
        // localization will change the check boxes text length
        if (_dataBoundColumnRadioButton.Checked)
        {
            _headerTextBox.Text = _nameTextBox.Text = AssignName();
        }
        else
        {
            Debug.Assert(_unboundColumnRadioButton.Checked, "we only have 2 radio buttons");
            string columnName = AssignName();
            _headerTextBox.Text = _nameTextBox.Text = columnName;
        }

        PopulateColumnTypesCombo();

        PopulateDataColumns();

        EnableDataBoundSection();

        _cancelButton.Text = SR.DataGridView_Cancel;
    }

    private void DataGridViewAddColumnDialog_VisibleChanged(object? sender, EventArgs e)
    {
        if (Visible && IsHandleCreated)
        {
            // we loaded the form
            if (_dataBoundColumnRadioButton.Checked)
            {
                Debug.Assert(_dataColumns.Enabled, "dataColumns list box and dataBoundColumnRadioButton should be enabled / disabled in sync");
                _dataColumns.Select();
            }
            else
            {
                Debug.Assert(_unboundColumnRadioButton.Checked, "We only have 2 radio buttons");
                _nameTextBox.Select();
            }
        }
    }

    private void nameTextBox_Validating(object? sender, CancelEventArgs e)
    {
        INameCreationService? nameCreationService = null;
        IContainer? container = null;

        if (_liveDataGridView.Site.TryGetService(out IDesignerHost? host))
        {
            container = host.Container;
            nameCreationService = _liveDataGridView.Site.GetService<INameCreationService>();
        }

        if (!ValidName(
            _nameTextBox.Text,
            _dataGridViewColumns,
            container,
            nameCreationService,
            _liveDataGridView.Columns,
            !_persistChangesToDesigner,
            out string errorString))
        {
            IUIService? uiService = _liveDataGridView.Site?.GetService<IUIService>();
            DataGridViewDesigner.ShowErrorDialog(uiService, errorString, _liveDataGridView);
            e.Cancel = true;
        }
    }

    private void PopulateColumnTypesCombo()
    {
        _columnTypesCombo.Items.Clear();

        if (!_liveDataGridView.Site.TryGetService(out IDesignerHost? host))
        {
            return;
        }

        ITypeDiscoveryService? discoveryService = host.GetService<ITypeDiscoveryService>();

        if (discoveryService is null)
        {
            return;
        }

        ICollection columnTypes = DesignerUtils.FilterGenericTypes(discoveryService.GetTypes(typeof(DataGridViewColumn), excludeGlobalTypes: false));

        foreach (Type type in columnTypes)
        {
            if (type == typeof(DataGridViewColumn))
            {
                continue;
            }

            if (type.IsAbstract)
            {
                continue;
            }

            if (!type.IsPublic && !type.IsNestedPublic)
            {
                continue;
            }

            if (TypeDescriptorHelper.TryGetAttribute(type, out DataGridViewColumnDesignTimeVisibleAttribute? attribute) && !attribute.Visible)
            {
                continue;
            }

            _columnTypesCombo.Items.Add(new ComboBoxItem(type));
        }

        // make the textBoxColumn type the selected type
        _columnTypesCombo.SelectedIndex = TypeToSelectedIndex(typeof(DataGridViewTextBoxColumn));
    }

    private void PopulateDataColumns()
    {
        int selectedIndex = _dataColumns.SelectedIndex;

        _dataColumns.SelectedIndex = -1;
        _dataColumns.Items.Clear();

        if (_liveDataGridView.DataSource is not null)
        {
            CurrencyManager? currencyManager = null;
            try
            {
                currencyManager = BindingContext?[_liveDataGridView.DataSource, _liveDataGridView.DataMember] as CurrencyManager;
            }
            catch (ArgumentException)
            {
            }

            PropertyDescriptorCollection? propertyDescriptorCollection = currencyManager?.GetItemProperties();

            if (propertyDescriptorCollection is not null)
            {
                for (int i = 0; i < propertyDescriptorCollection.Count; i++)
                {
                    PropertyDescriptor propertyDescriptor = propertyDescriptorCollection[i];
                    if (typeof(IList<object>).IsAssignableFrom(propertyDescriptor.PropertyType))
                    {
                        // we have an IList. It could be a byte[] in which case we want to generate an Image column
                        TypeConverter imageTypeConverter = TypeDescriptor.GetConverter(typeof(Image));
                        if (!imageTypeConverter.CanConvertFrom(propertyDescriptor.PropertyType))
                        {
                            continue;
                        }
                    }

                    _dataColumns.Items.Add(new ListBoxItem(propertyDescriptor.PropertyType, propertyDescriptor.Name));
                }
            }
        }

        _dataColumns.SelectedIndex = selectedIndex != -1 && selectedIndex < _dataColumns.Items.Count
            ? selectedIndex
            : _dataColumns.Items.Count > 0 ? 0 : -1;
    }

    private void addButton_Click(object? sender, EventArgs e)
    {
        _cancelButton.Text = (SR.DataGridView_Close);

        // AddColumn takes all the information from this dialog, makes a DataGridViewColumn out of it
        // and inserts it at the right index in the data grid view column collection
        AddColumn();
    }

    private void cancelButton_Click(object? sender, EventArgs e)
    {
        Close();
    }

    protected override bool ProcessDialogKey(Keys keyData)
    {
        if ((keyData & Keys.Modifiers) == 0)
        {
            switch (keyData & Keys.KeyCode)
            {
                case Keys.Enter:
                    // Validate the name before adding the column.
                    IDesignerHost? host = _liveDataGridView.Site?.GetService<IDesignerHost>();
                    IContainer? container = host?.Container;
                    INameCreationService? nameCreationService = _liveDataGridView.Site?.GetService<INameCreationService>();

                    if (ValidName(
                        _nameTextBox.Text,
                        _dataGridViewColumns,
                        container,
                        nameCreationService,
                        _liveDataGridView.Columns,
                        !_persistChangesToDesigner,
                        out string errorString))
                    {
                        AddColumn();
                        Close();
                    }
                    else
                    {
                        IUIService? uiService = _liveDataGridView.Site?.GetService<IUIService>();
                        DataGridViewDesigner.ShowErrorDialog(uiService, errorString, _liveDataGridView);
                    }

                    return true;
            }
        }

        return base.ProcessDialogKey(keyData);
    }

    internal void Start(int insertAtPosition, bool persistChangesToDesigner)
    {
        _insertAtPosition = insertAtPosition;
        _persistChangesToDesigner = persistChangesToDesigner;

        if (_persistChangesToDesigner)
        {
            _initialDataGridViewColumnsCount = _liveDataGridView.Columns.Count;
        }
        else
        {
            _initialDataGridViewColumnsCount = -1;
        }
    }

    private void SetDefaultDataGridViewColumnType(Type type)
    {
        TypeConverter imageTypeConverter = TypeDescriptor.GetConverter(typeof(Image));

        if (type == typeof(bool) || type == typeof(CheckState))
        {
            // get the data grid view check box column type
            _columnTypesCombo.SelectedIndex = TypeToSelectedIndex(typeof(DataGridViewCheckBoxColumn));
        }
        else if (typeof(Image).IsAssignableFrom(type) || imageTypeConverter.CanConvertFrom(type))
        {
            // get the data grid view image column type
            _columnTypesCombo.SelectedIndex = TypeToSelectedIndex(typeof(DataGridViewImageColumn));
        }
        else
        {
            // get the data grid view text box column type
            _columnTypesCombo.SelectedIndex = TypeToSelectedIndex(typeof(DataGridViewTextBoxColumn));
        }
    }

    private int TypeToSelectedIndex(Type type)
    {
        for (int i = 0; i < _columnTypesCombo.Items.Count; i++)
        {
            if (type == ((ComboBoxItem)_columnTypesCombo.Items[i]!).ColumnType)
            {
                return i;
            }
        }

        Debug.Fail("we should have found a type by now");

        return -1;
    }

    public static bool ValidName(string name,
        DataGridViewColumnCollection columns,
        IContainer? container,
        INameCreationService? nameCreationService,
        DataGridViewColumnCollection liveColumns,
        bool allowDuplicateNameInLiveColumnCollection)
        => ValidName(name, columns, container, nameCreationService, liveColumns, allowDuplicateNameInLiveColumnCollection, out _);

    /// <devdoc>
    ///  A column name is valid if it does not cause any name conflicts in the DataGridViewColumnCollection
    ///  and the IContainer::Component collection.
    /// </devdoc>
    public static bool ValidName(string name,
        DataGridViewColumnCollection columns,
        IContainer? container,
        INameCreationService? nameCreationService,
        DataGridViewColumnCollection? liveColumns,
        bool allowDuplicateNameInLiveColumnCollection,
        out string errorString)
    {
        if (columns.Contains(name))
        {
            errorString = string.Format(SR.DataGridViewDuplicateColumnName, name);
            return false;
        }

        if (container is not null && container.Components[name] is not null)
        {
            if (!allowDuplicateNameInLiveColumnCollection || liveColumns is null || !liveColumns.Contains(name))
            {
                errorString = string.Format(SR.DesignerHostDuplicateName, name);
                return false;
            }
        }

        if (nameCreationService is not null && !nameCreationService.IsValidName(name))
        {
            if (!allowDuplicateNameInLiveColumnCollection || liveColumns is null || !liveColumns.Contains(name))
            {
                errorString = string.Format(SR.CodeDomDesignerLoaderInvalidIdentifier, name);
                return false;
            }
        }

        errorString = string.Empty;
        return true;
    }

    private class ListBoxItem
    {
        public ListBoxItem(Type propertyType, string propertyName)
        {
            PropertyType = propertyType;
            PropertyName = propertyName;
        }

        public Type PropertyType { get; }

        public string PropertyName { get; }

        public override string ToString() => PropertyName;
    }

    private class ComboBoxItem
    {
        public ComboBoxItem(Type columnType)
        {
            ColumnType = columnType;
        }

        public override string ToString() => ColumnType.Name;

        public Type ColumnType { get; }
    }
}
