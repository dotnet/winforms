// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design.Serialization;
using System.ComponentModel.Design;
using System.ComponentModel;
using System.Globalization;
using System.Drawing;

namespace System.Windows.Forms.Design;

internal class DataGridViewAddColumnDialog : System.Windows.Forms.Form
{
    private System.Windows.Forms.RadioButton dataBoundColumnRadioButton;
    private System.Windows.Forms.Label columnInDataSourceLabel;

    private System.Windows.Forms.ListBox dataColumns;

    private System.Windows.Forms.RadioButton unboundColumnRadioButton;
    private System.Windows.Forms.TextBox nameTextBox;

    private System.Windows.Forms.ComboBox columnTypesCombo;

    private System.Windows.Forms.TextBox headerTextBox;
    private System.Windows.Forms.Label nameLabel;

    private System.Windows.Forms.Label typeLabel;

    private System.Windows.Forms.Label headerTextLabel;
    private System.Windows.Forms.CheckBox visibleCheckBox;

    private System.Windows.Forms.CheckBox readOnlyCheckBox;

    private System.Windows.Forms.CheckBox frozenCheckBox;
    private System.Windows.Forms.Button addButton;

    private System.Windows.Forms.Button cancelButton;

    private System.Windows.Forms.DataGridViewColumnCollection dataGridViewColumns;
    private System.Windows.Forms.DataGridView liveDataGridView;
    private int insertAtPosition = -1;
    private int initialDataGridViewColumnsCount = -1;
    private bool persistChangesToDesigner = false;

    private static Type dataGridViewColumnType = typeof(System.Windows.Forms.DataGridViewColumn);
    private static Type iDesignerType = typeof(System.ComponentModel.Design.IDesigner);
    private static Type iTypeResolutionServiceType = typeof(System.ComponentModel.Design.ITypeResolutionService);
    private static Type iTypeDiscoveryServiceType = typeof(System.ComponentModel.Design.ITypeDiscoveryService);
    private static Type iComponentChangeServiceType = typeof(System.ComponentModel.Design.IComponentChangeService);
    private static Type iHelpServiceType = typeof(System.ComponentModel.Design.IHelpService);
    private static Type iUIServiceType = typeof(System.Windows.Forms.Design.IUIService);
    private static Type iDesignerHostType = typeof(System.ComponentModel.Design.IDesignerHost);
    private static Type iNameCreationServiceType = typeof(System.ComponentModel.Design.Serialization.INameCreationService);
    private static Type dataGridViewColumnDesignTimeVisibleAttributeType = typeof(DataGridViewColumnDesignTimeVisibleAttribute);

    private static Type[] columnTypes = new Type[]
    {
        typeof(DataGridViewButtonColumn),
        typeof(DataGridViewCheckBoxColumn),
        typeof(DataGridViewComboBoxColumn),
        typeof(DataGridViewImageColumn),
        typeof(DataGridViewLinkColumn),
        typeof(DataGridViewTextBoxColumn)
    };
    private TableLayoutPanel okCancelTableLayoutPanel;
    private TableLayoutPanel checkBoxesTableLayoutPanel;
    private TableLayoutPanel overarchingTableLayoutPanel;

    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    public DataGridViewAddColumnDialog(DataGridViewColumnCollection dataGridViewColumns, DataGridView liveDataGridView)
    {
        this.dataGridViewColumns = dataGridViewColumns;
        this.liveDataGridView = liveDataGridView;

        // PERF: set the Dialog Font before InitializeComponent.
        //
        Font uiFont = Control.DefaultFont;
        IUIService uiService = (IUIService)this.liveDataGridView.Site.GetService(iUIServiceType);
        if (uiService != null)
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
        Type columnType = ((ComboBoxItem)this.columnTypesCombo.SelectedItem).ColumnType;

        DataGridViewColumn column = System.Activator.CreateInstance(columnType) as DataGridViewColumn;

        // if we want to insert a column before a frozen column then make sure that we insert a frozen column
        bool forceColumnFrozen = this.dataGridViewColumns.Count > this.insertAtPosition && this.dataGridViewColumns[this.insertAtPosition].Frozen;

        column.Frozen = forceColumnFrozen;

        // if we don't persist changes to the designer then we want to add the columns before
        // setting the Frozen bit
        if (!persistChangesToDesigner)
        {
            // set the header text because the DataGridViewColumnCollection needs it to compute
            // its listbox'x HorizontalOffset
            column.HeaderText = this.headerTextBox.Text;
            column.Name = this.nameTextBox.Text;
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
            catch (System.InvalidOperationException ex)
            {
                IUIService uiService = (IUIService)this.liveDataGridView.Site.GetService(typeof(IUIService));
                DataGridViewDesigner.ShowErrorDialog(uiService, ex, this.liveDataGridView);
                return;
            }
        }

        // Set the UserAddedColumn property to true.
        PropertyDescriptorCollection props = TypeDescriptor.GetProperties(column);
        PropertyDescriptor pd = props["UserAddedColumn"];
        if (pd != null)
        {
            pd.SetValue(column, true);
        }

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

        IDesignerHost host = null;
        IContainer container = null;

        host = this.liveDataGridView.Site.GetService(iDesignerHostType) as IDesignerHost;
        if (host != null)
        {
            container = host.Container;
        }

        while (!System.Windows.Forms.Design.DataGridViewAddColumnDialog.ValidName(columnName,
                                  this.dataGridViewColumns,
                                  container,
                                  null /*nameCreationService*/,
                                  this.liveDataGridView.Columns,
                                  !this.persistChangesToDesigner))
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
            if (components != null)
            {
                components.Dispose();
            }
        }

        base.Dispose(disposing);
    }

    private void EnableDataBoundSection()
    {
        bool dataGridViewIsDataBound = this.dataColumns.Items.Count > 0;
        if (dataGridViewIsDataBound)
        {
            this.dataBoundColumnRadioButton.Enabled = true;
            this.dataBoundColumnRadioButton.Checked = true;
            this.dataBoundColumnRadioButton.Focus();
            this.headerTextBox.Text = this.nameTextBox.Text = this.AssignName();
        }
        else
        {
            this.dataBoundColumnRadioButton.Enabled = false;
            this.unboundColumnRadioButton.Checked = true;
            this.nameTextBox.Focus();
            this.headerTextBox.Text = this.nameTextBox.Text = this.AssignName();
        }
    }

    //
    // public because the EditColumns dialog wants to use it as well.
    //
    public static ComponentDesigner GetComponentDesignerForType(ITypeResolutionService tr, Type type)
    {
        ComponentDesigner compDesigner = null;
        DesignerAttribute designerAttr = null;
        AttributeCollection attributes = TypeDescriptor.GetAttributes(type);
        for (int i = 0; i < attributes.Count; i++)
        {
            DesignerAttribute da = attributes[i] as DesignerAttribute;
            if (da != null)
            {
                Type daType = Type.GetType(da.DesignerBaseTypeName);
                if (daType != null && daType == iDesignerType)
                {
                    designerAttr = da;
                    break;
                }
            }
        }

        if (designerAttr != null)
        {
            Type designerType = null;
            if (tr != null)
            {
                designerType = tr.GetType(designerAttr.DesignerTypeName);
            }
            else
            {
                designerType = Type.GetType(designerAttr.DesignerTypeName);
            }

            if (designerType != null && typeof(System.ComponentModel.Design.ComponentDesigner).IsAssignableFrom(designerType))
            {
                compDesigner = (ComponentDesigner)System.Activator.CreateInstance(designerType) as ComponentDesigner;
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
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataGridViewAddColumnDialog));
        this.dataBoundColumnRadioButton = new System.Windows.Forms.RadioButton();
        this.overarchingTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
        this.checkBoxesTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
        this.frozenCheckBox = new System.Windows.Forms.CheckBox();
        this.visibleCheckBox = new System.Windows.Forms.CheckBox();
        this.readOnlyCheckBox = new System.Windows.Forms.CheckBox();
        this.okCancelTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
        this.addButton = new System.Windows.Forms.Button();
        this.cancelButton = new System.Windows.Forms.Button();
        this.columnInDataSourceLabel = new System.Windows.Forms.Label();
        this.dataColumns = new System.Windows.Forms.ListBox();
        this.unboundColumnRadioButton = new System.Windows.Forms.RadioButton();
        this.nameLabel = new System.Windows.Forms.Label();
        this.nameTextBox = new System.Windows.Forms.TextBox();
        this.typeLabel = new System.Windows.Forms.Label();
        this.columnTypesCombo = new System.Windows.Forms.ComboBox();
        this.headerTextLabel = new System.Windows.Forms.Label();
        this.headerTextBox = new System.Windows.Forms.TextBox();
        this.overarchingTableLayoutPanel.SuspendLayout();
        this.checkBoxesTableLayoutPanel.SuspendLayout();
        this.okCancelTableLayoutPanel.SuspendLayout();
        this.SuspendLayout();
        // 
        // dataBoundColumnRadioButton
        // 
        resources.ApplyResources(this.dataBoundColumnRadioButton, "dataBoundColumnRadioButton");
        this.dataBoundColumnRadioButton.Checked = true;
        this.overarchingTableLayoutPanel.SetColumnSpan(this.dataBoundColumnRadioButton, 3);
        this.dataBoundColumnRadioButton.Margin = new System.Windows.Forms.Padding(0);
        this.dataBoundColumnRadioButton.Name = "dataBoundColumnRadioButton";
        this.dataBoundColumnRadioButton.CheckedChanged += new System.EventHandler(this.dataBoundColumnRadioButton_CheckedChanged);
        // 
        // overarchingTableLayoutPanel
        // 
        resources.ApplyResources(this.overarchingTableLayoutPanel, "overarchingTableLayoutPanel");
        this.overarchingTableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
        this.overarchingTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(SizeType.Absolute, 14F));
        this.overarchingTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
        this.overarchingTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(SizeType.Absolute, 250F));
        this.overarchingTableLayoutPanel.Controls.Add(this.checkBoxesTableLayoutPanel, 0, 10);
        this.overarchingTableLayoutPanel.Controls.Add(this.okCancelTableLayoutPanel, 2, 11);
        this.overarchingTableLayoutPanel.Controls.Add(this.dataBoundColumnRadioButton, 0, 0);
        this.overarchingTableLayoutPanel.Controls.Add(this.columnInDataSourceLabel, 1, 1);
        this.overarchingTableLayoutPanel.Controls.Add(this.dataColumns, 1, 2);
        this.overarchingTableLayoutPanel.Controls.Add(this.unboundColumnRadioButton, 0, 4);
        this.overarchingTableLayoutPanel.Controls.Add(this.nameLabel, 1, 5);
        this.overarchingTableLayoutPanel.Controls.Add(this.nameTextBox, 2, 5);
        this.overarchingTableLayoutPanel.Controls.Add(this.typeLabel, 1, 7);
        this.overarchingTableLayoutPanel.Controls.Add(this.columnTypesCombo, 2, 7);
        this.overarchingTableLayoutPanel.Controls.Add(this.headerTextLabel, 1, 9);
        this.overarchingTableLayoutPanel.Controls.Add(this.headerTextBox, 2, 9);
        this.overarchingTableLayoutPanel.Margin = new System.Windows.Forms.Padding(12, 12, 12, 13);
        this.overarchingTableLayoutPanel.Name = "overarchingTableLayoutPanel";
        this.overarchingTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(SizeType.Absolute, 22F));
        this.overarchingTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(SizeType.Absolute, 16F));
        this.overarchingTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(SizeType.Absolute, 91F));
        this.overarchingTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(SizeType.Absolute, 12F));
        this.overarchingTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(SizeType.Absolute, 22F));
        this.overarchingTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
        this.overarchingTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(SizeType.Absolute, 4F));
        this.overarchingTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
        this.overarchingTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(SizeType.Absolute, 4F));
        this.overarchingTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
        this.overarchingTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(SizeType.Absolute, 27F));
        this.overarchingTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
        // 
        // checkBoxesTableLayoutPanel
        // 
        resources.ApplyResources(this.checkBoxesTableLayoutPanel, "checkBoxesTableLayoutPanel");
        this.checkBoxesTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
        this.checkBoxesTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
        this.checkBoxesTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
        this.checkBoxesTableLayoutPanel.Controls.Add(this.frozenCheckBox, 2, 0);
        this.checkBoxesTableLayoutPanel.Controls.Add(this.visibleCheckBox, 0, 0);
        this.checkBoxesTableLayoutPanel.Controls.Add(this.readOnlyCheckBox, 1, 0);
        this.checkBoxesTableLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
        this.overarchingTableLayoutPanel.SetColumnSpan(this.checkBoxesTableLayoutPanel, 3);
        this.checkBoxesTableLayoutPanel.Name = "checkBoxesTableLayoutPanel";
        this.checkBoxesTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
        // 
        // frozenCheckBox
        // 
        resources.ApplyResources(this.frozenCheckBox, "frozenCheckBox");
        this.frozenCheckBox.Margin = new System.Windows.Forms.Padding(0);
        this.frozenCheckBox.Name = "frozenCheckBox";
        // 
        // visibleCheckBox
        // 
        resources.ApplyResources(this.visibleCheckBox, "visibleCheckBox");
        this.visibleCheckBox.Checked = true;
        this.visibleCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
        this.visibleCheckBox.Margin = new System.Windows.Forms.Padding(0);
        this.visibleCheckBox.Name = "visibleCheckBox";
        // 
        // readOnlyCheckBox
        // 
        resources.ApplyResources(this.readOnlyCheckBox, "readOnlyCheckBox");
        this.readOnlyCheckBox.Margin = new System.Windows.Forms.Padding(0);
        this.readOnlyCheckBox.Name = "readOnlyCheckBox";
        // 
        // okCancelTableLayoutPanel
        // 
        resources.ApplyResources(this.okCancelTableLayoutPanel, "okCancelTableLayoutPanel");
        this.okCancelTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
        this.okCancelTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
        this.okCancelTableLayoutPanel.Controls.Add(this.addButton, 0, 0);
        this.okCancelTableLayoutPanel.Controls.Add(this.cancelButton, 1, 0);
        this.okCancelTableLayoutPanel.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
        this.okCancelTableLayoutPanel.Name = "okCancelTableLayoutPanel";
        this.okCancelTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
        // 
        // addButton
        // 
        resources.ApplyResources(this.addButton, "addButton");
        this.addButton.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
        this.addButton.Name = "addButton";
        this.addButton.Click += new System.EventHandler(this.addButton_Click);
        // 
        // cancelButton
        // 
        resources.ApplyResources(this.cancelButton, "cancelButton");
        this.cancelButton.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
        this.cancelButton.Name = "cancelButton";
        this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
        // 
        // columnInDataSourceLabel
        // 
        resources.ApplyResources(this.columnInDataSourceLabel, "columnInDataSourceLabel");
        this.overarchingTableLayoutPanel.SetColumnSpan(this.columnInDataSourceLabel, 2);
        this.columnInDataSourceLabel.Margin = new System.Windows.Forms.Padding(0);
        this.columnInDataSourceLabel.Name = "columnInDataSourceLabel";
        // 
        // dataColumns
        // 
        resources.ApplyResources(this.dataColumns, "dataColumns");
        this.overarchingTableLayoutPanel.SetColumnSpan(this.dataColumns, 2);
        this.dataColumns.FormattingEnabled = true;
        this.dataColumns.Margin = new System.Windows.Forms.Padding(0);
        this.dataColumns.Name = "dataColumns";

        this.dataColumns.SelectedIndexChanged += new System.EventHandler(this.dataColumns_SelectedIndexChanged);
        // 
        // unboundColumnRadioButton
        // 
        resources.ApplyResources(this.unboundColumnRadioButton, "unboundColumnRadioButton");
        this.overarchingTableLayoutPanel.SetColumnSpan(this.unboundColumnRadioButton, 3);
        this.unboundColumnRadioButton.Margin = new System.Windows.Forms.Padding(0);
        this.unboundColumnRadioButton.Name = "unboundColumnRadioButton";
        this.unboundColumnRadioButton.CheckedChanged += new System.EventHandler(this.unboundColumnRadioButton_CheckedChanged);
        // 
        // nameLabel
        // 
        resources.ApplyResources(this.nameLabel, "nameLabel");
        this.nameLabel.Margin = new System.Windows.Forms.Padding(0);
        this.nameLabel.Name = "nameLabel";
        // 
        // nameTextBox
        // 
        resources.ApplyResources(this.nameTextBox, "nameTextBox");
        this.nameTextBox.Margin = new System.Windows.Forms.Padding(0);
        this.nameTextBox.Name = "nameTextBox";
        this.nameTextBox.Validating += new CancelEventHandler(this.nameTextBox_Validating);
        // 
        // typeLabel
        // 
        resources.ApplyResources(this.typeLabel, "typeLabel");
        this.typeLabel.Margin = new System.Windows.Forms.Padding(0);
        this.typeLabel.Name = "typeLabel";
        // 
        // columnTypesCombo
        // 
        resources.ApplyResources(this.columnTypesCombo, "columnTypesCombo");
        this.columnTypesCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.columnTypesCombo.FormattingEnabled = true;
        this.columnTypesCombo.Margin = new System.Windows.Forms.Padding(0);
        this.columnTypesCombo.Name = "columnTypesCombo";
        this.columnTypesCombo.Sorted = true;
        // 
        // headerTextLabel
        // 
        resources.ApplyResources(this.headerTextLabel, "headerTextLabel");
        this.headerTextLabel.Margin = new System.Windows.Forms.Padding(0);
        this.headerTextLabel.Name = "headerTextLabel";
        // 
        // headerTextBox
        // 
        resources.ApplyResources(this.headerTextBox, "headerTextBox");
        this.headerTextBox.Margin = new System.Windows.Forms.Padding(0);
        this.headerTextBox.Name = "headerTextBox";
        // 
        // DataGridViewAddColumnDialog
        // 
        resources.ApplyResources(this, "$this");
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.CancelButton = this.cancelButton;
        this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
        this.Controls.Add(this.overarchingTableLayoutPanel);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        this.HelpButton = true;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.Name = "DataGridViewAddColumnDialog";
        this.ShowIcon = false;
        this.ShowInTaskbar = false;
        this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.DataGridViewAddColumnDialog_HelpButtonClicked);
        this.Closed += new System.EventHandler(this.DataGridViewAddColumnDialog_Closed);
        this.VisibleChanged += new System.EventHandler(this.DataGridViewAddColumnDialog_VisibleChanged);
        this.Load += new System.EventHandler(this.DataGridViewAddColumnDialog_Load);
        this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.DataGridViewAddColumnDialog_HelpRequested);
        this.overarchingTableLayoutPanel.ResumeLayout(false);
        this.overarchingTableLayoutPanel.PerformLayout();
        this.checkBoxesTableLayoutPanel.ResumeLayout(false);
        this.checkBoxesTableLayoutPanel.PerformLayout();
        this.okCancelTableLayoutPanel.ResumeLayout(false);
        this.okCancelTableLayoutPanel.PerformLayout();
        this.ResumeLayout(false);
        this.PerformLayout();
    }
    #endregion

    private void dataBoundColumnRadioButton_CheckedChanged(object sender, System.EventArgs e)
    {
        this.columnInDataSourceLabel.Enabled = this.dataBoundColumnRadioButton.Checked;
        this.dataColumns.Enabled = this.dataBoundColumnRadioButton.Checked;

        // push the property name into the headerTextBox and into the nameTextBox
        dataColumns_SelectedIndexChanged(null, EventArgs.Empty);
    }

    private void dataColumns_SelectedIndexChanged(object sender, System.EventArgs e)
    {
        if (this.dataColumns.SelectedItem == null)
        {
            return;
        }

        this.headerTextBox.Text = this.nameTextBox.Text = ((ListBoxItem)this.dataColumns.SelectedItem).PropertyName;

        // pick a default data grid view column type
        // NOTE: this will pick one of our data grid view column types
        SetDefaultDataGridViewColumnType(((ListBoxItem)this.dataColumns.SelectedItem).PropertyType);
    }

    private void unboundColumnRadioButton_CheckedChanged(object sender, System.EventArgs e)
    {
        if (this.unboundColumnRadioButton.Checked)
        {
            this.nameTextBox.Text = this.headerTextBox.Text = this.AssignName();
            this.nameTextBox.Focus();
        }
    }

    private void DataGridViewAddColumnDialog_Closed(object sender, System.EventArgs e)
    {
        if (this.persistChangesToDesigner)
        {
            try
            {
                Debug.Assert(this.initialDataGridViewColumnsCount != -1, "did you forget to set the initialDataGridViewColumnsCount when you started the dialog?");
                IComponentChangeService changeService = (IComponentChangeService)this.liveDataGridView.Site.GetService(iComponentChangeServiceType);
                if (changeService == null)
                {
                    return;
                }

                // to get good Undo/Redo we need to bring the data grid view column collection
                // back to its initial state before firing the componentChanging event
                DataGridViewColumn[] cols = new DataGridViewColumn[this.liveDataGridView.Columns.Count - this.initialDataGridViewColumnsCount];
                for (int i = this.initialDataGridViewColumnsCount; i < this.liveDataGridView.Columns.Count; i++)
                {
                    cols[i - this.initialDataGridViewColumnsCount] = this.liveDataGridView.Columns[i];
                }

                for (int i = this.initialDataGridViewColumnsCount; i < this.liveDataGridView.Columns.Count;)
                {
                    this.liveDataGridView.Columns.RemoveAt(this.initialDataGridViewColumnsCount);
                }

                // the data grid view column collection is back to its initial state
                // fire the component changing event
                PropertyDescriptor prop = TypeDescriptor.GetProperties(this.liveDataGridView)["Columns"];
                changeService.OnComponentChanging(this.liveDataGridView, prop);

                // add back the data grid view columns the user added using the Add Columns dialog
                // But first wipe out the existing display index.
                for (int i = 0; i < cols.Length; i++)
                {
                    cols[i].DisplayIndex = -1;
                }

                this.liveDataGridView.Columns.AddRange(cols);

                // fire component changed event
                changeService.OnComponentChanged(this.liveDataGridView, prop, null, null);
            }
            catch (System.InvalidOperationException)
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

    private void DataGridViewAddColumnDialog_HelpButtonClicked(object sender, CancelEventArgs e)
    {
        DataGridViewAddColumnDialog_HelpRequestHandled();
        e.Cancel = true;
    }

    private void DataGridViewAddColumnDialog_HelpRequested(object sender, System.Windows.Forms.HelpEventArgs e)
    {
        DataGridViewAddColumnDialog_HelpRequestHandled();
        e.Handled = true;
    }

    private void DataGridViewAddColumnDialog_HelpRequestHandled()
    {
        IHelpService helpService = this.liveDataGridView.Site.GetService(iHelpServiceType) as IHelpService;
        if (helpService != null)
        {
            helpService.ShowHelpFromKeyword("vs.DataGridViewAddColumnDialog");
        }
    }

    private void DataGridViewAddColumnDialog_Load(object sender, System.EventArgs e)
    {
        // setup Visible/ReadOnly/Frozen checkboxes
        // localization will change the check boxes text length
        if (this.dataBoundColumnRadioButton.Checked)
        {
            this.headerTextBox.Text = this.nameTextBox.Text = AssignName();
        }
        else
        {
            Debug.Assert(this.unboundColumnRadioButton.Checked, "we only have 2 radio buttons");
            string columnName = this.AssignName();
            this.headerTextBox.Text = this.nameTextBox.Text = columnName;
        }

        PopulateColumnTypesCombo();

        PopulateDataColumns();

        EnableDataBoundSection();

        this.cancelButton.Text = (SR.DataGridView_Cancel);
    }

    private void DataGridViewAddColumnDialog_VisibleChanged(object sender, System.EventArgs e)
    {
        if (this.Visible && this.IsHandleCreated)
        {
            // we loaded the form
            if (this.dataBoundColumnRadioButton.Checked)
            {
                Debug.Assert(this.dataColumns.Enabled, "dataColumns list box and dataBoundColumnRadioButton should be enabled / disabled in sync");
                this.dataColumns.Select();
            }
            else
            {
                Debug.Assert(this.unboundColumnRadioButton.Checked, "We only have 2 radio buttons");
                this.nameTextBox.Select();
            }
        }
    }

    private void nameTextBox_Validating(object sender, CancelEventArgs e)
    {
        IDesignerHost host = null;
        INameCreationService nameCreationService = null;
        IContainer container = null;

        host = this.liveDataGridView.Site.GetService(iDesignerHostType) as IDesignerHost;
        if (host != null)
        {
            container = host.Container;
        }

        nameCreationService = this.liveDataGridView.Site.GetService(iNameCreationServiceType) as INameCreationService;

        string errorString = String.Empty;
        if (!ValidName(this.nameTextBox.Text,
                       this.dataGridViewColumns,
                       container,
                       nameCreationService,
                       this.liveDataGridView.Columns,
                       !this.persistChangesToDesigner,
                       out errorString))
        {
            IUIService uiService = (IUIService)this.liveDataGridView.Site.GetService(iUIServiceType);
            System.Windows.Forms.Design.DataGridViewDesigner.ShowErrorDialog(uiService, errorString, this.liveDataGridView);
            e.Cancel = true;
        }
    }

    // this code will have to change once we figure out how what to do w/ third party
    // data grid view column types
    private void PopulateColumnTypesCombo()
    {
        this.columnTypesCombo.Items.Clear();

        IDesignerHost host = (IDesignerHost)this.liveDataGridView.Site.GetService(iDesignerHostType);
        if (host == null)
        {
            return;
        }

        ITypeDiscoveryService discoveryService = (ITypeDiscoveryService)host.GetService(iTypeDiscoveryServiceType);

        if (discoveryService == null)
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

            DataGridViewColumnDesignTimeVisibleAttribute attr = TypeDescriptor.GetAttributes(t)[dataGridViewColumnDesignTimeVisibleAttributeType] as DataGridViewColumnDesignTimeVisibleAttribute;
            if (attr != null && !attr.Visible)
            {
                continue;
            }

            this.columnTypesCombo.Items.Add(new ComboBoxItem(t));
        }

        // make the textBoxColumn type the selected type
        this.columnTypesCombo.SelectedIndex = this.TypeToSelectedIndex(typeof(System.Windows.Forms.DataGridViewTextBoxColumn));
    }

    private void PopulateDataColumns()
    {
        int selectedIndex = this.dataColumns.SelectedIndex;

        this.dataColumns.SelectedIndex = -1;
        this.dataColumns.Items.Clear();

        if (this.liveDataGridView.DataSource != null)
        {
            CurrencyManager cm = null;
            try
            {
                cm = this.BindingContext[this.liveDataGridView.DataSource, this.liveDataGridView.DataMember] as CurrencyManager;
            }
            catch (ArgumentException)
            {
                cm = null;
            }

            PropertyDescriptorCollection props = cm != null ? cm.GetItemProperties() : null;

            if (props != null)
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

    private void addButton_Click(object sender, System.EventArgs e)
    {
        this.cancelButton.Text = (SR.DataGridView_Close);

        // AddColumn takes all the information from this dialog, makes a DataGridViewColumn out of it
        // and inserts it at the right index in the data grid view column collection
        this.AddColumn();
    }

    private void cancelButton_Click(object sender, System.EventArgs e)
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
                    IDesignerHost host = null;
                    INameCreationService nameCreationService = null;
                    IContainer container = null;

                    host = this.liveDataGridView.Site.GetService(iDesignerHostType) as IDesignerHost;
                    if (host != null)
                    {
                        container = host.Container;
                    }

                    nameCreationService = this.liveDataGridView.Site.GetService(iNameCreationServiceType) as INameCreationService;

                    string errorString = String.Empty;
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
                        System.Windows.Forms.Design.DataGridViewDesigner.ShowErrorDialog(uiService, errorString, this.liveDataGridView);
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
            this.initialDataGridViewColumnsCount = this.liveDataGridView.Columns.Count;
        }
        else
        {
            this.initialDataGridViewColumnsCount = -1;
        }
    }

    private void SetDefaultDataGridViewColumnType(Type type)
    {
        TypeConverter imageTypeConverter = TypeDescriptor.GetConverter(typeof(System.Drawing.Image));

        if (type == typeof(bool) || type == typeof(CheckState))
        {
            // get the data grid view check box column type
            this.columnTypesCombo.SelectedIndex = this.TypeToSelectedIndex(typeof(System.Windows.Forms.DataGridViewCheckBoxColumn));
        }
        else if (typeof(System.Drawing.Image).IsAssignableFrom(type) || imageTypeConverter.CanConvertFrom(type))
        {
            // get the data grid view image column type
            this.columnTypesCombo.SelectedIndex = this.TypeToSelectedIndex(typeof(System.Windows.Forms.DataGridViewImageColumn));
        }
        else
        {
            // get the data grid view text box column type
            this.columnTypesCombo.SelectedIndex = this.TypeToSelectedIndex(typeof(System.Windows.Forms.DataGridViewTextBoxColumn));
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

        if (container != null && container.Components[name] != null)
        {
            if (!allowDuplicateNameInLiveColumnCollection || liveColumns == null || !liveColumns.Contains(name))
            {
                return false;
            }
        }

        if (nameCreationService != null && !nameCreationService.IsValidName(name))
        {
            if (!allowDuplicateNameInLiveColumnCollection || liveColumns == null || !liveColumns.Contains(name))
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

        if (container != null && container.Components[name] != null)
        {
            if (!allowDuplicateNameInLiveColumnCollection || liveColumns == null || !liveColumns.Contains(name))
            {
                errorString = string.Format(SR.DesignerHostDuplicateName, name);
                return false;
            }
        }

        if (nameCreationService != null && !nameCreationService.IsValidName(name))
        {
            if (!allowDuplicateNameInLiveColumnCollection || liveColumns == null || !liveColumns.Contains(name))
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
        Type propertyType;
        string propertyName;

        public ListBoxItem(Type propertyType, string propertyName)
        {
            this.propertyType = propertyType;
            this.propertyName = propertyName;
        }

        public Type PropertyType
        {
            get
            {
                return this.propertyType;
            }
        }

        public string PropertyName
        {
            get
            {
                return this.propertyName;
            }
        }

        public override string ToString()
        {
            return this.propertyName;
        }
    }

    private class ComboBoxItem
    {
        Type columnType;
        public ComboBoxItem(Type columnType)
        {
            this.columnType = columnType;
        }

        public override string ToString()
        {
            return this.columnType.Name;
        }

        public Type ColumnType
        {
            get
            {
                return this.columnType;
            }
        }
    }
}
