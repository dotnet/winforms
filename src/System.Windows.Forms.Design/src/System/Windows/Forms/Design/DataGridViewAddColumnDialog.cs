// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design.Serialization;
using System.ComponentModel.Design;
using System.ComponentModel;
using System.Globalization;
using System.Drawing;

namespace System.Windows.Forms.Design;

internal class DataGridViewAddColumnDialog :Form
{
    private RadioButton? dataBoundColumnRadioButton;
    private Label? columnInDataSourceLabel;

    private ListBox? dataColumns;

    private RadioButton? unboundColumnRadioButton;
    private TextBox? nameTextBox;

    private ComboBox? columnTypesCombo;

    private TextBox? headerTextBox;
    private Label? nameLabel;

    private Label? typeLabel;

    private Label? headerTextLabel;
    private CheckBox? visibleCheckBox;

    private CheckBox? readOnlyCheckBox;

    private CheckBox? frozenCheckBox;
    private Button? addButton;

    private Button? cancelButton;

    private DataGridViewColumnCollection? dataGridViewColumns;
    private DataGridView? liveDataGridView;
    private int insertAtPosition = -1;
    private int initialDataGridViewColumnsCount = -1;
    private bool persistChangesToDesigner;

    private static Type dataGridViewColumnType = typeof(DataGridViewColumn);
    private static Type iDesignerType = typeof(IDesigner);
    private static Type iTypeDiscoveryServiceType = typeof(ITypeDiscoveryService);
    private static Type iComponentChangeServiceType = typeof(IComponentChangeService);
    private static Type iHelpServiceType = typeof(IHelpService);
    private static Type iUIServiceType = typeof(Design.IUIService);
    private static Type iDesignerHostType = typeof(IDesignerHost);
    private static Type iNameCreationServiceType = typeof(INameCreationService);
    private static Type dataGridViewColumnDesignTimeVisibleAttributeType = typeof(DataGridViewColumnDesignTimeVisibleAttribute);
    private TableLayoutPanel? okCancelTableLayoutPanel;
    private TableLayoutPanel? checkBoxesTableLayoutPanel;
    private TableLayoutPanel? overarchingTableLayoutPanel;

    /// <summary>
    /// Required designer variable.
    /// </summary>
    private IContainer? components;

    public DataGridViewAddColumnDialog(DataGridViewColumnCollection dataGridViewColumns, DataGridView liveDataGridView)
    {
        this.dataGridViewColumns = dataGridViewColumns;
        this.liveDataGridView = liveDataGridView;

        // PERF: set the Dialog Font before InitializeComponent.
        //
        Font uiFont = Control.DefaultFont;
        IUIService? uiService = this.liveDataGridView.Site?.GetService(iUIServiceType) as IUIService;
        if (uiService is not null)
        {
            uiFont = (Font)uiService.Styles["DialogFont"];
        }

        this.Font = uiFont;

        //
        // Required for Windows Form Designer support
        //
        InitializeComponent();

        // put this code in the constructor because we want to keep the dataBoundColumnRadioButton.Checked state
        // when the user closes and reopens the add column dialog
        EnableDataBoundSection();
    }

    // consolidate all the information from the dialog into a data grid view column
    // and insert it in the data grid view column collection
    private void AddColumn()
    {
        ComboBoxItem? comboBoxItem = columnTypesCombo?.SelectedItem as ComboBoxItem;
        Type? columnType = comboBoxItem?.ColumnType;

        DataGridViewColumn? column = System.Activator.CreateInstance(columnType) as DataGridViewColumn;

        // if we want to insert a column before a frozen column then make sure that we insert a frozen column
        bool forceColumnFrozen = dataGridViewColumns.Count > insertAtPosition && dataGridViewColumns[insertAtPosition].Frozen;

        column.Frozen = forceColumnFrozen;

        // if we don't persist changes to the designer then we want to add the columns before
        // setting the Frozen bit
        if (!persistChangesToDesigner)
        {
            // set the header text because the DataGridViewColumnCollection needs it to compute
            // its listbox'x HorizontalOffset
            column.HeaderText = headerTextBox.Text;
            column.Name = nameTextBox.Text;
            column.DisplayIndex = -1;
            this.dataGridViewColumns.Insert(this.insertAtPosition, column);
            insertAtPosition++;
        }

        // if we persist changes directly to the designer then:
        // 1. set the column property values 
        // 2. Add the new column
        // 3. site the column

        // 1. set the property values in the column
        //
        column.HeaderText = this.headerTextBox.Text;
        column.Name = this.nameTextBox.Text;
        column.Visible = this.visibleCheckBox.Checked;
        column.Frozen = this.frozenCheckBox.Checked || forceColumnFrozen;
        column.ReadOnly = this.readOnlyCheckBox.Checked;

        if (this.dataBoundColumnRadioButton.Checked && dataColumns.SelectedIndex > -1)
        {
            column.DataPropertyName = ((ListBoxItem)dataColumns.SelectedItem).PropertyName;
        }

        if (this.persistChangesToDesigner)
        {
            try
            {
                // insert the column before siting the column.
                // if DataGridView throws an exception then the designer does not end up w/ a column that is not in any data grid view.
                column.DisplayIndex = -1;
                this.dataGridViewColumns.Insert(insertAtPosition, column);
                insertAtPosition++;
                // site the column
                this.liveDataGridView.Site.Container.Add(column, column.Name);
            }
            catch (InvalidOperationException ex)
            {
                IUIService? uiService = this.liveDataGridView.Site?.GetService(typeof(IUIService)) as IUIService;
                DataGridViewDesigner.ShowErrorDialog(uiService, ex, this.liveDataGridView);
                return;
            }
        }

        // Set the UserAddedColumn property to true.
        PropertyDescriptorCollection props = TypeDescriptor.GetProperties(column);
        PropertyDescriptor? pd = props["UserAddedColumn"];
        pd?.SetValue(column, true);

        // pick a new Column name
        //
        this.nameTextBox.Text = this.headerTextBox.Text = this.AssignName();
        this.nameTextBox.Focus();
    }

    private string AssignName()
    {
        int colId = 1;
        // string columnName = (SR.DataGridView_ColumnName, colId.ToString());
        string columnName = "Column" + colId.ToString(CultureInfo.InvariantCulture);

        IDesignerHost? host = null;
        IContainer? container = null;

        host = liveDataGridView?.Site?.GetService(iDesignerHostType) as IDesignerHost;
        if (host is not null)
        {
            container = host.Container;
        }

        while (!ValidName(columnName,
                            dataGridViewColumns,
                            container,
                            null /*nameCreationService*/,
                            liveDataGridView.Columns,
                            !persistChangesToDesigner))
        {
            colId++;
            // columnName = (SR.DataGridView_ColumnName, colId.ToString());
            columnName = "Column" + colId.ToString(CultureInfo.InvariantCulture);
        }

        return columnName;
    }

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            components?.Dispose();
        }

        base.Dispose(disposing);
    }

    private void EnableDataBoundSection()
    {
        bool dataGridViewIsDataBound = dataColumns.Items.Count > 0;
        if (dataGridViewIsDataBound)
        {
            dataBoundColumnRadioButton.Enabled = true;
            dataBoundColumnRadioButton.Checked = true;
            dataBoundColumnRadioButton.Focus();
            headerTextBox.Text = nameTextBox.Text = AssignName();
        }
        else
        {
            dataBoundColumnRadioButton.Enabled = false;
            unboundColumnRadioButton.Checked = true;
            nameTextBox.Focus();
            headerTextBox.Text = nameTextBox.Text = AssignName();
        }
    }

    //
    // public because the EditColumns dialog wants to use it as well.
    //
    public static ComponentDesigner GetComponentDesignerForType(ITypeResolutionService tr, Type type)
    {
        ComponentDesigner? compDesigner = null;
        DesignerAttribute? designerAttr = null;
        AttributeCollection? attributes = TypeDescriptor.GetAttributes(type);
        for (int i = 0; i < attributes.Count; i++)
        {
            DesignerAttribute? da = attributes[i] as DesignerAttribute;
            if (da is not null)
            {
                Type? daType = Type.GetType(da.DesignerBaseTypeName);
                if (daType is not null && daType == iDesignerType)
                {
                    designerAttr = da;
                    break;
                }
            }
        }

        if (designerAttr is not null)
        {
            Type? designerType = null;
            if (tr is not null)
            {
                designerType = tr.GetType(designerAttr.DesignerTypeName);
            }
            else
            {
                designerType = Type.GetType(designerAttr.DesignerTypeName);
            }

            if (designerType is not null && typeof(ComponentDesigner).IsAssignableFrom(designerType))
            {
                compDesigner = System.Activator.CreateInstance(designerType) as ComponentDesigner;
            }
        }

        return compDesigner;
    }

    #region Windows Form Designer generated code
    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        ComponentResourceManager resources = new ComponentResourceManager(typeof(DataGridViewAddColumnDialog));
        dataBoundColumnRadioButton = new RadioButton();
        overarchingTableLayoutPanel = new TableLayoutPanel();
        checkBoxesTableLayoutPanel = new TableLayoutPanel();
        frozenCheckBox = new CheckBox();
        visibleCheckBox = new CheckBox();
        readOnlyCheckBox = new CheckBox();
        okCancelTableLayoutPanel = new TableLayoutPanel();
        addButton = new Button();
        cancelButton = new Button();
        columnInDataSourceLabel = new Label();
        dataColumns = new ListBox();
        unboundColumnRadioButton = new RadioButton();
        nameLabel = new Label();
        nameTextBox = new TextBox();
        typeLabel = new Label();
        columnTypesCombo = new ComboBox();
        headerTextLabel = new Label();
        headerTextBox = new TextBox();
        overarchingTableLayoutPanel.SuspendLayout();
        checkBoxesTableLayoutPanel.SuspendLayout();
        okCancelTableLayoutPanel.SuspendLayout();
        SuspendLayout();
        //
        // dataBoundColumnRadioButton
        //
        resources.ApplyResources(dataBoundColumnRadioButton, "dataBoundColumnRadioButton");
        dataBoundColumnRadioButton.Checked = true;
        overarchingTableLayoutPanel.SetColumnSpan(dataBoundColumnRadioButton, 3);
        dataBoundColumnRadioButton.Margin = new Padding(0);
        dataBoundColumnRadioButton.Name = "dataBoundColumnRadioButton";
        dataBoundColumnRadioButton.CheckedChanged += new System.EventHandler(dataBoundColumnRadioButton_CheckedChanged);
        //
        // overarchingTableLayoutPanel
        //
        resources.ApplyResources(this.overarchingTableLayoutPanel, "overarchingTableLayoutPanel");
        overarchingTableLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        overarchingTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 14F));
        overarchingTableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
        overarchingTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 250F));
        overarchingTableLayoutPanel.Controls.Add(checkBoxesTableLayoutPanel, 0, 10);
        overarchingTableLayoutPanel.Controls.Add(okCancelTableLayoutPanel, 2, 11);
        overarchingTableLayoutPanel.Controls.Add(dataBoundColumnRadioButton, 0, 0);
        overarchingTableLayoutPanel.Controls.Add(columnInDataSourceLabel, 1, 1);
        overarchingTableLayoutPanel.Controls.Add(dataColumns, 1, 2);
        overarchingTableLayoutPanel.Controls.Add(unboundColumnRadioButton, 0, 4);
        overarchingTableLayoutPanel.Controls.Add(nameLabel, 1, 5);
        overarchingTableLayoutPanel.Controls.Add(nameTextBox, 2, 5);
        overarchingTableLayoutPanel.Controls.Add(typeLabel, 1, 7);
        overarchingTableLayoutPanel.Controls.Add(columnTypesCombo, 2, 7);
        overarchingTableLayoutPanel.Controls.Add(headerTextLabel, 1, 9);
        overarchingTableLayoutPanel.Controls.Add(headerTextBox, 2, 9);
        overarchingTableLayoutPanel.Margin = new Padding(12, 12, 12, 13);
        overarchingTableLayoutPanel.Name = "overarchingTableLayoutPanel";
        overarchingTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 22F));
        overarchingTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 16F));
        overarchingTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 91F));
        overarchingTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 12F));
        overarchingTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 22F));
        overarchingTableLayoutPanel.RowStyles.Add(new RowStyle());
        overarchingTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 4F));
        overarchingTableLayoutPanel.RowStyles.Add(new RowStyle());
        overarchingTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 4F));
        overarchingTableLayoutPanel.RowStyles.Add(new RowStyle());
        overarchingTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 27F));
        overarchingTableLayoutPanel.RowStyles.Add(new RowStyle());
        // 
        // checkBoxesTableLayoutPanel
        // 
        resources.ApplyResources(checkBoxesTableLayoutPanel, "checkBoxesTableLayoutPanel");
        checkBoxesTableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
        checkBoxesTableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
        checkBoxesTableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
        checkBoxesTableLayoutPanel.Controls.Add(frozenCheckBox, 2, 0);
        checkBoxesTableLayoutPanel.Controls.Add(visibleCheckBox, 0, 0);
        checkBoxesTableLayoutPanel.Controls.Add(readOnlyCheckBox, 1, 0);
        checkBoxesTableLayoutPanel.Margin = new Padding(0);
        overarchingTableLayoutPanel.SetColumnSpan(checkBoxesTableLayoutPanel, 3);
        checkBoxesTableLayoutPanel.Name = "checkBoxesTableLayoutPanel";
        checkBoxesTableLayoutPanel.RowStyles.Add(new RowStyle());
        //
        // frozenCheckBox
        //
        resources.ApplyResources(frozenCheckBox, "frozenCheckBox");
        frozenCheckBox.Margin = new Padding(0);
        frozenCheckBox.Name = "frozenCheckBox";
        //
        // visibleCheckBox
        //
        resources.ApplyResources(visibleCheckBox, "visibleCheckBox");
        visibleCheckBox.Checked = true;
        visibleCheckBox.CheckState = CheckState.Checked;
        visibleCheckBox.Margin = new Padding(0);
        visibleCheckBox.Name = "visibleCheckBox";
        //
        // readOnlyCheckBox
        //
        resources.ApplyResources(readOnlyCheckBox, "readOnlyCheckBox");
        readOnlyCheckBox.Margin = new Padding(0);
        readOnlyCheckBox.Name = "readOnlyCheckBox";
        //
        // okCancelTableLayoutPanel
        //
        resources.ApplyResources(okCancelTableLayoutPanel, "okCancelTableLayoutPanel");
        okCancelTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        okCancelTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        okCancelTableLayoutPanel.Controls.Add(addButton, 0, 0);
        okCancelTableLayoutPanel.Controls.Add(cancelButton, 1, 0);
        okCancelTableLayoutPanel.Margin = new Padding(0, 6, 0, 0);
        okCancelTableLayoutPanel.Name = "okCancelTableLayoutPanel";
        okCancelTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        //
        // addButton
        //
        resources.ApplyResources(addButton, "addButton");
        addButton.Margin = new Padding(0, 0, 3, 0);
        addButton.Name = "addButton";
        addButton.Click += new System.EventHandler(addButton_Click);
        //
        // cancelButton
        //
        resources.ApplyResources(cancelButton, "cancelButton");
        cancelButton.Margin = new Padding(3, 0, 0, 0);
        cancelButton.Name = "cancelButton";
        cancelButton.Click += new EventHandler(cancelButton_Click);
        //
        // columnInDataSourceLabel
        //
        resources.ApplyResources(columnInDataSourceLabel, "columnInDataSourceLabel");
        overarchingTableLayoutPanel.SetColumnSpan(columnInDataSourceLabel, 2);
        columnInDataSourceLabel.Margin = new Padding(0);
        columnInDataSourceLabel.Name = "columnInDataSourceLabel";
        //
        // dataColumns
        //
        resources.ApplyResources(dataColumns, "dataColumns");
        overarchingTableLayoutPanel.SetColumnSpan(dataColumns, 2);
        dataColumns.FormattingEnabled = true;
        dataColumns.Margin = new Padding(0);
        dataColumns.Name = "dataColumns";

        dataColumns.SelectedIndexChanged += new EventHandler(dataColumns_SelectedIndexChanged);
        //
        // unboundColumnRadioButton
        //
        resources.ApplyResources(unboundColumnRadioButton, "unboundColumnRadioButton");
        overarchingTableLayoutPanel.SetColumnSpan(unboundColumnRadioButton, 3);
        unboundColumnRadioButton.Margin = new Padding(0);
        unboundColumnRadioButton.Name = "unboundColumnRadioButton";
        unboundColumnRadioButton.CheckedChanged += new EventHandler(unboundColumnRadioButton_CheckedChanged);
        //
        // nameLabel
        //
        resources.ApplyResources(nameLabel, "nameLabel");
        nameLabel.Margin = new Padding(0);
        nameLabel.Name = "nameLabel";
        //
        // nameTextBox
        //
        resources.ApplyResources(nameTextBox, "nameTextBox");
        nameTextBox.Margin = new Padding(0);
        nameTextBox.Name = "nameTextBox";
        nameTextBox.Validating += new CancelEventHandler(nameTextBox_Validating);
        //
        // typeLabel
        //
        resources.ApplyResources(typeLabel, "typeLabel");
        typeLabel.Margin = new Padding(0);
        typeLabel.Name = "typeLabel";
        //
        // columnTypesCombo
        //
        resources.ApplyResources(columnTypesCombo, "columnTypesCombo");
        columnTypesCombo.DropDownStyle = ComboBoxStyle.DropDownList;
        columnTypesCombo.FormattingEnabled = true;
        columnTypesCombo.Margin = new Padding(0);
        columnTypesCombo.Name = "columnTypesCombo";
        columnTypesCombo.Sorted = true;
        //
        // headerTextLabel
        //
        resources.ApplyResources(headerTextLabel, "headerTextLabel");
        headerTextLabel.Margin = new Padding(0);
        headerTextLabel.Name = "headerTextLabel";
        //
        // headerTextBox
        //
        resources.ApplyResources(headerTextBox, "headerTextBox");
        headerTextBox.Margin = new Padding(0);
        headerTextBox.Name = "headerTextBox";
        //
        // DataGridViewAddColumnDialog
        //
        resources.ApplyResources(this, "$this");
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = cancelButton;
        AutoSizeMode = AutoSizeMode.GrowAndShrink;
        Controls.Add(overarchingTableLayoutPanel);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        HelpButton = true;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "DataGridViewAddColumnDialog";
        ShowIcon = false;
        ShowInTaskbar = false;
        HelpButtonClicked += new CancelEventHandler(DataGridViewAddColumnDialog_HelpButtonClicked);
        Closed += new EventHandler(DataGridViewAddColumnDialog_Closed);
        VisibleChanged += new EventHandler(DataGridViewAddColumnDialog_VisibleChanged);
        Load += new EventHandler(DataGridViewAddColumnDialog_Load);
        HelpRequested += new HelpEventHandler(DataGridViewAddColumnDialog_HelpRequested);
        overarchingTableLayoutPanel.ResumeLayout(false);
        overarchingTableLayoutPanel.PerformLayout();
        checkBoxesTableLayoutPanel.ResumeLayout(false);
        checkBoxesTableLayoutPanel.PerformLayout();
        okCancelTableLayoutPanel.ResumeLayout(false);
        okCancelTableLayoutPanel.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }
    #endregion

    private void dataBoundColumnRadioButton_CheckedChanged(object? sender, System.EventArgs e)
    {
        columnInDataSourceLabel.Enabled = dataBoundColumnRadioButton.Checked;
        dataColumns.Enabled = dataBoundColumnRadioButton.Checked;

        // push the property name into the headerTextBox and into the nameTextBox
        dataColumns_SelectedIndexChanged(null, EventArgs.Empty);
    }

    private void dataColumns_SelectedIndexChanged(object? sender, System.EventArgs e)
    {
        if (dataColumns?.SelectedItem is null)
        {
            return;
        }

        headerTextBox.Text = nameTextBox.Text = ((ListBoxItem)dataColumns.SelectedItem).PropertyName;

        // pick a default data grid view column type
        // NOTE: this will pick one of our data grid view column types
        SetDefaultDataGridViewColumnType(((ListBoxItem)dataColumns.SelectedItem).PropertyType);
    }

    private void unboundColumnRadioButton_CheckedChanged(object? sender, System.EventArgs e)
    {
        if (unboundColumnRadioButton.Checked)
        {
            nameTextBox.Text = headerTextBox.Text = AssignName();
            nameTextBox.Focus();
        }
    }

    private void DataGridViewAddColumnDialog_Closed(object? sender, System.EventArgs e)
    {
        if (persistChangesToDesigner)
        {
            try
            {
                Debug.Assert(initialDataGridViewColumnsCount != -1, "did you forget to set the initialDataGridViewColumnsCount when you started the dialog?");
                IComponentChangeService changeService = (IComponentChangeService)liveDataGridView.Site.GetService(iComponentChangeServiceType);
                if (changeService is null)
                {
                    return;
                }

                // to get good Undo/Redo we need to bring the data grid view column collection
                // back to its initial state before firing the componentChanging event
                DataGridViewColumn[] cols = new DataGridViewColumn[liveDataGridView.Columns.Count - initialDataGridViewColumnsCount];
                for (int i = initialDataGridViewColumnsCount; i < liveDataGridView.Columns.Count; i++)
                {
                    cols[i - initialDataGridViewColumnsCount] = liveDataGridView.Columns[i];
                }

                for (int i = initialDataGridViewColumnsCount; i < liveDataGridView.Columns.Count;)
                {
                    liveDataGridView.Columns.RemoveAt(initialDataGridViewColumnsCount);
                }

                // the data grid view column collection is back to its initial state
                // fire the component changing event
                PropertyDescriptor? prop = TypeDescriptor.GetProperties(liveDataGridView)["Columns"];
                changeService.OnComponentChanging(liveDataGridView, prop);

                // add back the data grid view columns the user added using the Add Columns dialog
                // But first wipe out the existing display index.
                for (int i = 0; i < cols.Length; i++)
                {
                    cols[i].DisplayIndex = -1;
                }

                liveDataGridView.Columns.AddRange(cols);

                // fire component changed event
                changeService.OnComponentChanged(liveDataGridView, prop, null, null);
            }
            catch (InvalidOperationException)
            {
            }
        }
#if DEBUG
        else
        {
            Debug.Assert(this.initialDataGridViewColumnsCount == -1, "did you forget to reset the initialDataGridViewColumnsCount when you started the dialog?");
        }
#endif // DEBUG

        // The DialogResult is OK.
        this.DialogResult = DialogResult.OK;
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
        IHelpService? helpService = liveDataGridView?.Site?.GetService(iHelpServiceType) as IHelpService;
        helpService?.ShowHelpFromKeyword("vs.DataGridViewAddColumnDialog");
    }

    private void DataGridViewAddColumnDialog_Load(object? sender, System.EventArgs e)
    {
        // setup Visible/ReadOnly/Frozen checkboxes
        // localization will change the check boxes text length
        if (dataBoundColumnRadioButton!.Checked)
        {
            headerTextBox!.Text = nameTextBox!.Text = AssignName();
        }
        else
        {
            Debug.Assert(unboundColumnRadioButton!.Checked, "we only have 2 radio buttons");
            string columnName = AssignName();
            headerTextBox!.Text = nameTextBox!.Text = columnName;
        }

        PopulateColumnTypesCombo();

        PopulateDataColumns();

        EnableDataBoundSection();

        cancelButton!.Text = SR.DataGridView_Cancel;
    }

    private void DataGridViewAddColumnDialog_VisibleChanged(object? sender, System.EventArgs e)
    {
        if (Visible && IsHandleCreated)
        {
            // we loaded the form
            if (dataBoundColumnRadioButton!.Checked)
            {
                Debug.Assert(dataColumns!.Enabled, "dataColumns list box and dataBoundColumnRadioButton should be enabled / disabled in sync");
                dataColumns.Select();
            }
            else
            {
                Debug.Assert(unboundColumnRadioButton!.Checked, "We only have 2 radio buttons");
                nameTextBox!.Select();
            }
        }
    }

    private void nameTextBox_Validating(object? sender, CancelEventArgs e)
    {
        IDesignerHost? host = null;
        INameCreationService? nameCreationService = null;
        IContainer? container = null;

        host = liveDataGridView?.Site?.GetService(iDesignerHostType) as IDesignerHost;
        if (host is not null)
        {
            container = host.Container;
        }

        nameCreationService = liveDataGridView?.Site?.GetService(iNameCreationServiceType) as INameCreationService;

        string errorString = string.Empty;
        if (!ValidName(nameTextBox.Text,
                       dataGridViewColumns,
                       container,
                       nameCreationService,
                       liveDataGridView.Columns,
                       !persistChangesToDesigner,
                       out errorString))
        {
            IUIService? uiService = liveDataGridView?.Site?.GetService(iUIServiceType) as IUIService;
            DataGridViewDesigner.ShowErrorDialog(uiService, errorString, liveDataGridView);
            e.Cancel = true;
        }
    }

    // this code will have to change once we figure out how what to do w/ third party
    // data grid view column types
    private void PopulateColumnTypesCombo()
    {
        columnTypesCombo.Items.Clear();

        IDesignerHost? host = liveDataGridView?.Site?.GetService(iDesignerHostType) as IDesignerHost;
        if (host is null)
        {
            return;
        }

        ITypeDiscoveryService? discoveryService = host.GetService(iTypeDiscoveryServiceType) as ITypeDiscoveryService;

        if (discoveryService is null)
        {
            return;
        }

        ICollection<object> columnTypes = (ICollection<object>)DesignerUtils.FilterGenericTypes(discoveryService.GetTypes(dataGridViewColumnType, false /*excludeGlobalTypes*/));

        foreach (Type t in columnTypes)
        {
            if (t == dataGridViewColumnType)
            {
                continue;
            }

            if (t.IsAbstract)
            {
                continue;
            }

            if (!t.IsPublic && !t.IsNestedPublic)
            {
                continue;
            }

            DataGridViewColumnDesignTimeVisibleAttribute? attr = TypeDescriptor.GetAttributes(t)[dataGridViewColumnDesignTimeVisibleAttributeType] as DataGridViewColumnDesignTimeVisibleAttribute;
            if (attr is not null && !attr.Visible)
            {
                continue;
            }

            columnTypesCombo.Items.Add(new ComboBoxItem(t));
        }

        // make the textBoxColumn type the selected type
        columnTypesCombo.SelectedIndex = TypeToSelectedIndex(typeof(DataGridViewTextBoxColumn));
    }

    private void PopulateDataColumns()
    {
        int selectedIndex = dataColumns.SelectedIndex;

        dataColumns.SelectedIndex = -1;
        dataColumns.Items.Clear();

        if (liveDataGridView.DataSource is not null)
        {
            CurrencyManager? cm = null;
            try
            {
                cm = BindingContext[liveDataGridView.DataSource, liveDataGridView.DataMember] as CurrencyManager;
            }
            catch (ArgumentException)
            {
                cm = null;
            }

            PropertyDescriptorCollection props = cm is not null ? cm.GetItemProperties() : null;

            if (props is not null)
            {
                for (int i = 0; i < props.Count; i++)
                {
                    if (typeof(IList<object>).IsAssignableFrom(props[i].PropertyType))
                    {
                        // we have an IList. It could be a byte[] in which case we want to generate an Image column
                        //
                        TypeConverter imageTypeConverter = TypeDescriptor.GetConverter(typeof(Image));
                        if (!imageTypeConverter.CanConvertFrom(props[i].PropertyType))
                        {
                            continue;
                        }
                    }

                    this.dataColumns.Items.Add(new ListBoxItem(props[i].PropertyType, props[i].Name));
                }
            }
        }

        if (selectedIndex != -1 && selectedIndex < this.dataColumns.Items.Count)
        {
            this.dataColumns.SelectedIndex = selectedIndex;
        }
        else
        {
            this.dataColumns.SelectedIndex = this.dataColumns.Items.Count > 0 ? 0 : -1;
        }
    }

    private void addButton_Click(object? sender, System.EventArgs e)
    {
        this.cancelButton.Text = (SR.DataGridView_Close);

        // AddColumn takes all the information from this dialog, makes a DataGridViewColumn out of it
        // and inserts it at the right index in the data grid view column collection
        this.AddColumn();
    }

    private void cancelButton_Click(object? sender, System.EventArgs e)
    {
        this.Close();
        /*
        if (this.cancelButtonIsCloseButton)
        {
            this.AddColumn();
        }
        */
    }

    protected override bool ProcessDialogKey(Keys keyData)
    {
        if ((keyData & Keys.Modifiers) == 0)
        {
            switch (keyData & Keys.KeyCode)
            {
                case Keys.Enter:
                    // Validate the name before adding the column.
                    IDesignerHost? host = null;
                    INameCreationService? nameCreationService = null;
                    IContainer? container = null;

                    host = this.liveDataGridView.Site.GetService(iDesignerHostType) as IDesignerHost;
                    if (host is not null)
                    {
                        container = host.Container;
                    }

                    nameCreationService = this.liveDataGridView.Site.GetService(iNameCreationServiceType) as INameCreationService;

                    string errorString = string.Empty;
                    if (ValidName(this.nameTextBox.Text,
                                  this.dataGridViewColumns,
                                  container,
                                  nameCreationService,
                                  this.liveDataGridView.Columns,
                                  !this.persistChangesToDesigner,
                                  out errorString))
                    {
                        this.AddColumn();
                        this.Close();
                    }
                    else
                    {
                        IUIService uiService = (IUIService)this.liveDataGridView.Site.GetService(iUIServiceType);
                        Design.DataGridViewDesigner.ShowErrorDialog(uiService, errorString, this.liveDataGridView);
                    }

                    return true;
            }
        }

        return base.ProcessDialogKey(keyData);
    }

    internal void Start(int insertAtPosition, bool persistChangesToDesigner)
    {
        this.insertAtPosition = insertAtPosition;
        this.persistChangesToDesigner = persistChangesToDesigner;

        if (this.persistChangesToDesigner)
        {
            initialDataGridViewColumnsCount = liveDataGridView.Columns.Count;
        }
        else
        {
            initialDataGridViewColumnsCount = -1;
        }
    }

    private void SetDefaultDataGridViewColumnType(Type type)
    {
        TypeConverter imageTypeConverter = TypeDescriptor.GetConverter(typeof(Image));

        if (type == typeof(bool) || type == typeof(CheckState))
        {
            // get the data grid view check box column type
            columnTypesCombo.SelectedIndex = TypeToSelectedIndex(typeof(DataGridViewCheckBoxColumn));
        }
        else if (typeof(System.Drawing.Image).IsAssignableFrom(type) || imageTypeConverter.CanConvertFrom(type))
        {
            // get the data grid view image column type
            columnTypesCombo.SelectedIndex = TypeToSelectedIndex(typeof(DataGridViewImageColumn));
        }
        else
        {
            // get the data grid view text box column type
            this.columnTypesCombo.SelectedIndex = this.TypeToSelectedIndex(typeof(DataGridViewTextBoxColumn));
        }
    }

    private int TypeToSelectedIndex(Type type)
    {
        for (int i = 0; i < this.columnTypesCombo.Items.Count; i++)
        {
            if (type == ((ComboBoxItem)this.columnTypesCombo.Items[i]).ColumnType)
            {
                return i;
            }
        }

        Debug.Assert(false, "we should have found a type by now");

        return -1;
    }

    public static bool ValidName(string name,
                                 DataGridViewColumnCollection columns,
                                 IContainer container,
                                 INameCreationService nameCreationService,
                                 DataGridViewColumnCollection liveColumns,
                                 bool allowDuplicateNameInLiveColumnCollection)
    {
        if (columns.Contains(name))
        {
            return false;
        }

        if (container is not null && container.Components[name] is not null)
        {
            if (!allowDuplicateNameInLiveColumnCollection || liveColumns is null || !liveColumns.Contains(name))
            {
                return false;
            }
        }

        if (nameCreationService is not null && !nameCreationService.IsValidName(name))
        {
            if (!allowDuplicateNameInLiveColumnCollection || liveColumns is null || !liveColumns.Contains(name))
            {
                return false;
            }
        }

        return true;
    }

    /// <devdoc>
    ///     A column name is valid if it does not cause any name conflicts in the DataGridViewColumnCollection
    ///     and the IContainer::Component collection.
    /// </devdoc>
    public static bool ValidName(string name,
                                 DataGridViewColumnCollection columns,
                                 IContainer container,
                                 INameCreationService nameCreationService,
                                 DataGridViewColumnCollection liveColumns,
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
        private Type propertyType;
        private string propertyName;

        public ListBoxItem(Type propertyType, string propertyName)
        {
            this.propertyType = propertyType;
            this.propertyName = propertyName;
        }

        public Type PropertyType
        {
            get
            {
                return propertyType;
            }
        }

        public string PropertyName
        {
            get
            {
                return propertyName;
            }
        }

        public override string ToString()
        {
            return propertyName;
        }
    }

    private class ComboBoxItem
    {
        private Type columnType;
        public ComboBoxItem(Type columnType)
        {
            this.columnType = columnType;
        }

        public override string ToString()
        {
            return columnType.Name;
        }

        public Type ColumnType
        {
            get
            {
                return columnType;
            }
        }
    }
}
