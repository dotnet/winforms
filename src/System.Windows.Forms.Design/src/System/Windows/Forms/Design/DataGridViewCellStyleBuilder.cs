// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;

namespace System.Windows.Forms.Design;

internal class DataGridViewCellStyleBuilder : Form
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private PropertyGrid _cellStyleProperties;
    private GroupBox _previewGroupBox;
    private Button _okButton;
    private Button _cancelButton;
    private Label _label1;
    private readonly DataGridView _listenerDataGridView;
    private DataGridView _sampleDataGridView;
    private DataGridView _sampleDataGridViewSelected;
    private TableLayoutPanel _sampleViewTableLayoutPanel;
    private TableLayoutPanel _okCancelTableLayoutPanel;
    private TableLayoutPanel _overarchingTableLayoutPanel;
    private TableLayoutPanel _sampleViewGridsTableLayoutPanel;
    private Label _normalLabel;
    private Label _selectedLabel;

    private DataGridViewCellStyle? _cellStyle;
    private ITypeDescriptorContext? _context;

    public DataGridViewCellStyleBuilder(IServiceProvider serviceProvider, IComponent component)
    {
        // Required for Windows Form Designer support
        InitializeComponent();

        // Adds columns and rows to the grid, also resizes them
        InitializeGrids();

        _listenerDataGridView = new DataGridView();

        _cellStyleProperties.Site = new DataGridViewComponentPropertyGridSite(serviceProvider, component);
    }

    private void InitializeGrids()
    {
        _sampleDataGridViewSelected.Size = new Drawing.Size(100, Font.Height + 9);
        _sampleDataGridView.Size = new Drawing.Size(100, Font.Height + 9);
        _sampleDataGridView.AccessibilityObject.Name = SR.CellStyleBuilderNormalPreviewAccName;

        DataGridViewRow row = new();
        row.Cells.Add(new DialogDataGridViewCell());
        row.Cells[0].Value = "####";
        row.Cells[0].AccessibilityObject.Name = SR.CellStyleBuilderSelectedPreviewAccName;

        _sampleDataGridViewSelected.Columns.Add(new DataGridViewTextBoxColumn());
        _sampleDataGridViewSelected.Rows.Add(row);
        _sampleDataGridViewSelected.Rows[0].Selected = true;
        _sampleDataGridViewSelected.AccessibilityObject.Name = SR.CellStyleBuilderSelectedPreviewAccName;

        row = new DataGridViewRow();
        row.Cells.Add(new DialogDataGridViewCell());
        row.Cells[0].Value = "####";
        row.Cells[0].AccessibilityObject.Name = SR.CellStyleBuilderNormalPreviewAccName;

        _sampleDataGridView.Columns.Add(new DataGridViewTextBoxColumn());
        _sampleDataGridView.Rows.Add(row);
    }

    [DisallowNull]
    public DataGridViewCellStyle? CellStyle
    {
        get => _cellStyle;
        set
        {
            _cellStyle = new DataGridViewCellStyle(value);
            _cellStyleProperties.SelectedObject = _cellStyle;
            ListenerDataGridViewDefaultCellStyleChanged(sender: null, EventArgs.Empty);
            _listenerDataGridView.DefaultCellStyle = _cellStyle;
            _listenerDataGridView.DefaultCellStyleChanged += ListenerDataGridViewDefaultCellStyleChanged;
        }
    }

    public ITypeDescriptorContext? Context
    {
        set => _context = value;
    }

    private void ListenerDataGridViewDefaultCellStyleChanged(object? sender, EventArgs e)
    {
        DataGridViewCellStyle cellStyleTmp = new(_cellStyle!);
        _sampleDataGridView.DefaultCellStyle = cellStyleTmp;
        _sampleDataGridViewSelected.DefaultCellStyle = cellStyleTmp;
    }

    #region Windows Form Designer generated code
    /// <summary>
    ///  Required method for Designer support - do not modify the contents of this method with the code editor.
    /// </summary>
    [MemberNotNull(nameof(_cellStyleProperties))]
    [MemberNotNull(nameof(_sampleViewTableLayoutPanel))]
    [MemberNotNull(nameof(_sampleViewGridsTableLayoutPanel))]
    [MemberNotNull(nameof(_normalLabel))]
    [MemberNotNull(nameof(_sampleDataGridView))]
    [MemberNotNull(nameof(_selectedLabel))]
    [MemberNotNull(nameof(_sampleDataGridViewSelected))]
    [MemberNotNull(nameof(_label1))]
    [MemberNotNull(nameof(_okButton))]
    [MemberNotNull(nameof(_cancelButton))]
    [MemberNotNull(nameof(_okCancelTableLayoutPanel))]
    [MemberNotNull(nameof(_previewGroupBox))]
    [MemberNotNull(nameof(_overarchingTableLayoutPanel))]
    private void InitializeComponent()
    {
        ComponentResourceManager resources = new(typeof(DataGridViewCellStyleBuilder));
        _cellStyleProperties = new PropertyGrid();
        _sampleViewTableLayoutPanel = new TableLayoutPanel();
        _sampleViewGridsTableLayoutPanel = new TableLayoutPanel();
        _normalLabel = new Label();
        _sampleDataGridView = new DataGridView();
        _selectedLabel = new Label();
        _sampleDataGridViewSelected = new DataGridView();
        _label1 = new Label();
        _okButton = new Button();
        _cancelButton = new Button();
        _okCancelTableLayoutPanel = new TableLayoutPanel();
        _previewGroupBox = new GroupBox();
        _overarchingTableLayoutPanel = new TableLayoutPanel();
        _sampleViewTableLayoutPanel.SuspendLayout();
        _sampleViewGridsTableLayoutPanel.SuspendLayout();
        ((ISupportInitialize)(_sampleDataGridView)).BeginInit();
        ((ISupportInitialize)(_sampleDataGridViewSelected)).BeginInit();
        _okCancelTableLayoutPanel.SuspendLayout();
        _previewGroupBox.SuspendLayout();
        _overarchingTableLayoutPanel.SuspendLayout();
        SuspendLayout();
        // cellStyleProperties
        resources.ApplyResources(_cellStyleProperties, "cellStyleProperties");

        // Linecolor assigned here is causing issues in the HC mode. Going with runtime default for HC mode.
        if (!SystemInformation.HighContrast)
        {
            _cellStyleProperties.LineColor = Drawing.SystemColors.ScrollBar;
        }

        _cellStyleProperties.Margin = new Padding(0, 0, 0, 3);
        _cellStyleProperties.Name = "cellStyleProperties";
        _cellStyleProperties.ToolbarVisible = false;
        // sampleViewTableLayoutPanel
        resources.ApplyResources(_sampleViewTableLayoutPanel, "sampleViewTableLayoutPanel");
        _sampleViewTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 423F));
        _sampleViewTableLayoutPanel.Controls.Add(_sampleViewGridsTableLayoutPanel, 0, 1);
        _sampleViewTableLayoutPanel.Controls.Add(_label1, 0, 0);
        _sampleViewTableLayoutPanel.Name = "sampleViewTableLayoutPanel";
        _sampleViewTableLayoutPanel.RowStyles.Add(new RowStyle());
        _sampleViewTableLayoutPanel.RowStyles.Add(new RowStyle());
        // sampleViewGridsTableLayoutPanel
        resources.ApplyResources(_sampleViewGridsTableLayoutPanel, "sampleViewGridsTableLayoutPanel");
        _sampleViewGridsTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F));
        _sampleViewGridsTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
        _sampleViewGridsTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
        _sampleViewGridsTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
        _sampleViewGridsTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F));
        _sampleViewGridsTableLayoutPanel.Controls.Add(_normalLabel, 1, 0);
        _sampleViewGridsTableLayoutPanel.Controls.Add(_sampleDataGridView, 1, 1);
        _sampleViewGridsTableLayoutPanel.Controls.Add(_selectedLabel, 3, 0);
        _sampleViewGridsTableLayoutPanel.Controls.Add(_sampleDataGridViewSelected, 3, 1);
        _sampleViewGridsTableLayoutPanel.Margin = new Padding(0, 3, 0, 0);
        _sampleViewGridsTableLayoutPanel.Name = "sampleViewGridsTableLayoutPanel";
        _sampleViewGridsTableLayoutPanel.RowStyles.Add(new RowStyle());
        _sampleViewGridsTableLayoutPanel.RowStyles.Add(new RowStyle());
        // normalLabel
        resources.ApplyResources(_normalLabel, "normalLabel");
        _normalLabel.Margin = new Padding(0);
        _normalLabel.Name = "normalLabel";
        // sampleDataGridView
        _sampleDataGridView.AllowUserToAddRows = false;
        resources.ApplyResources(_sampleDataGridView, "sampleDataGridView");
        _sampleDataGridView.ColumnHeadersVisible = false;
        _sampleDataGridView.Margin = new Padding(0);
        _sampleDataGridView.Name = "sampleDataGridView";
        _sampleDataGridView.ReadOnly = true;
        _sampleDataGridView.RowHeadersVisible = false;
        _sampleDataGridView.CellStateChanged += sampleDataGridView_CellStateChanged;
        // selectedLabel
        resources.ApplyResources(_selectedLabel, "selectedLabel");
        _selectedLabel.Margin = new Padding(0);
        _selectedLabel.Name = "selectedLabel";
        // sampleDataGridViewSelected
        _sampleDataGridViewSelected.AllowUserToAddRows = false;
        resources.ApplyResources(_sampleDataGridViewSelected, "sampleDataGridViewSelected");
        _sampleDataGridViewSelected.ColumnHeadersVisible = false;
        _sampleDataGridViewSelected.Margin = new Padding(0);
        _sampleDataGridViewSelected.Name = "sampleDataGridViewSelected";
        _sampleDataGridViewSelected.ReadOnly = true;
        _sampleDataGridViewSelected.RowHeadersVisible = false;
        // label1
        resources.ApplyResources(_label1, "label1");
        _label1.Margin = new Padding(0, 0, 0, 3);
        _label1.Name = "label1";
        // okButton
        resources.ApplyResources(_okButton, "okButton");
        _okButton.DialogResult = DialogResult.OK;
        _okButton.Margin = new Padding(0, 0, 3, 0);
        _okButton.Name = "okButton";
        // cancelButton
        resources.ApplyResources(_cancelButton, "cancelButton");
        _cancelButton.DialogResult = DialogResult.Cancel;
        _cancelButton.Margin = new Padding(3, 0, 0, 0);
        _cancelButton.Name = "cancelButton";
        // okCancelTableLayoutPanel
        resources.ApplyResources(_okCancelTableLayoutPanel, "okCancelTableLayoutPanel");
        _okCancelTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        _okCancelTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        _okCancelTableLayoutPanel.Controls.Add(_okButton, 0, 0);
        _okCancelTableLayoutPanel.Controls.Add(_cancelButton, 1, 0);
        _okCancelTableLayoutPanel.Margin = new Padding(0, 3, 0, 0);
        _okCancelTableLayoutPanel.Name = "okCancelTableLayoutPanel";
        _okCancelTableLayoutPanel.RowStyles.Add(new RowStyle());
        // previewGroupBox
        resources.ApplyResources(_previewGroupBox, "previewGroupBox");
        _previewGroupBox.Controls.Add(_sampleViewTableLayoutPanel);
        _previewGroupBox.Margin = new Padding(0, 3, 0, 3);
        _previewGroupBox.Name = "previewGroupBox";
        _previewGroupBox.TabStop = false;
        // overarchingTableLayoutPanel
        resources.ApplyResources(_overarchingTableLayoutPanel, "overarchingTableLayoutPanel");
        _overarchingTableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
        _overarchingTableLayoutPanel.Controls.Add(_cellStyleProperties, 0, 0);
        _overarchingTableLayoutPanel.Controls.Add(_okCancelTableLayoutPanel, 0, 2);
        _overarchingTableLayoutPanel.Controls.Add(_previewGroupBox, 0, 1);
        _overarchingTableLayoutPanel.Name = "overarchingTableLayoutPanel";
        _overarchingTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        _overarchingTableLayoutPanel.RowStyles.Add(new RowStyle());
        _overarchingTableLayoutPanel.RowStyles.Add(new RowStyle());
        // DataGridViewCellStyleBuilder
        resources.ApplyResources(this, "$this");
        AutoScaleDimensions = new Drawing.SizeF(6F, 13F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(_overarchingTableLayoutPanel);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        HelpButton = true;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "DataGridViewCellStyleBuilder";
        ShowIcon = false;
        ShowInTaskbar = false;
        HelpButtonClicked += DataGridViewCellStyleBuilder_HelpButtonClicked;
        HelpRequested += DataGridViewCellStyleBuilder_HelpRequested;
        Load += DataGridViewCellStyleBuilder_Load;
        _sampleViewTableLayoutPanel.ResumeLayout(false);
        _sampleViewTableLayoutPanel.PerformLayout();
        _sampleViewGridsTableLayoutPanel.ResumeLayout(false);
        _sampleViewGridsTableLayoutPanel.PerformLayout();
        ((ISupportInitialize)(_sampleDataGridView)).EndInit();
        ((ISupportInitialize)(_sampleDataGridViewSelected)).EndInit();
        _okCancelTableLayoutPanel.ResumeLayout(false);
        _okCancelTableLayoutPanel.PerformLayout();
        _previewGroupBox.ResumeLayout(false);
        _previewGroupBox.PerformLayout();
        _overarchingTableLayoutPanel.ResumeLayout(false);
        _overarchingTableLayoutPanel.PerformLayout();
        ResumeLayout(false);
    }
    #endregion

    protected override bool ProcessDialogKey(Keys keyData)
    {
        if ((keyData & Keys.Modifiers) == 0 && (keyData & Keys.KeyCode) == Keys.Escape)
        {
            Close();
            return true;
        }
        else
        {
            return base.ProcessDialogKey(keyData);
        }
    }

    private void DataGridViewCellStyleBuilder_HelpButtonClicked(object? sender, CancelEventArgs e)
    {
        e.Cancel = true;
        DataGridViewCellStyleBuilder_HelpRequestHandled();
    }

    private void DataGridViewCellStyleBuilder_HelpRequested(object? sender, HelpEventArgs e)
    {
        e.Handled = true;
        DataGridViewCellStyleBuilder_HelpRequestHandled();
    }

    private void DataGridViewCellStyleBuilder_HelpRequestHandled()
    {
        if (_context!.GetService(typeof(IHelpService)) is IHelpService helpService)
        {
            helpService.ShowHelpFromKeyword("vs.CellStyleDialog");
        }
    }

    private void DataGridViewCellStyleBuilder_Load(object? sender, EventArgs e)
    {
        // The cell inside the sampleDataGridView should not be selected.
        _sampleDataGridView.ClearSelection();

        // make sure that the cell inside the sampleDataGridView and sampleDataGridViewSelected fill their respective dataGridView's
        _sampleDataGridView.Rows[0].Height = _sampleDataGridView.Height;
        _sampleDataGridView.Columns[0].Width = _sampleDataGridView.Width;

        _sampleDataGridViewSelected.Rows[0].Height = _sampleDataGridViewSelected.Height;
        _sampleDataGridViewSelected.Columns[0].Width = _sampleDataGridViewSelected.Width;

        // sync the Layout event for both sample DataGridView's so that
        // when the sample DataGridView's are laid out we know to change the size of their cells
        _sampleDataGridView.Layout += sampleDataGridView_Layout;
        _sampleDataGridViewSelected.Layout += sampleDataGridView_Layout;
    }

    private void sampleDataGridView_CellStateChanged(object? sender, DataGridViewCellStateChangedEventArgs e)
    {
        Debug.Assert(e.Cell == _sampleDataGridView.Rows[0].Cells[0], "the sample data grid view has only one cell");
        Debug.Assert(sender == _sampleDataGridView, "did we forget to unhook notification");
        if ((e.StateChanged & DataGridViewElementStates.Selected) != 0 && (e.Cell.State & DataGridViewElementStates.Selected) != 0)
        {
            // The cell inside the sample data grid view became selected. We don't want that to happen
            _sampleDataGridView.ClearSelection();
        }
    }

    private void sampleDataGridView_Layout(object? sender, LayoutEventArgs e)
    {
        DataGridView dataGridView = (DataGridView)sender!;
        dataGridView.Rows[0].Height = dataGridView.Height;
        dataGridView.Columns[0].Width = dataGridView.Width;
    }

    private class DialogDataGridViewCell : DataGridViewTextBoxCell
    {
        private DialogDataGridViewCellAccessibleObject? _accObj;
        protected override AccessibleObject CreateAccessibilityInstance()
        {
            _accObj ??= new DialogDataGridViewCellAccessibleObject(this);

            return _accObj;
        }

        private class DialogDataGridViewCellAccessibleObject : DataGridViewCellAccessibleObject
        {
            public DialogDataGridViewCellAccessibleObject(DataGridViewCell owner) : base(owner)
            {
            }

            public override string? Name { get; set; } = string.Empty;
        }
    }
}
