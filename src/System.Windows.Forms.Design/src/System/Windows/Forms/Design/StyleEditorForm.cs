// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Globalization;

namespace System.Windows.Forms.Design;

internal partial class StyleCollectionEditor
{
    protected class StyleEditorForm : CollectionForm
    {
        private readonly StyleCollectionEditor _editor;
        private bool _isRowCollection;
        private readonly TableLayoutPanel _tableLayoutPanel;
        private readonly TableLayoutPanelDesigner _tableLayoutPanelDesigner;
        private readonly IComponentChangeService _componentChangeService;
        private readonly List<Control> _deleteList;

        private bool _isDialogDirty;
        private bool _haveInvoked;
        private bool _performEnsure;

        // ListView subitem indices.
        private const int MemberIndex = 0;
        private const int TypeIndex = 1;
        private const int ValueIndex = 2;

        private readonly PropertyDescriptor _rowStyleProp;
        private readonly PropertyDescriptor _colStyleProp;

        /// <summary>
        /// All our control instance variables.
        /// </summary>

        private TableLayoutPanel _overarchingTableLayoutPanel;

        private TableLayoutPanel _addRemoveInsertTableLayoutPanel;
        private Button _addButton;
        private Button _removeButton;
        private Button _insertButton;

        private TableLayoutPanel _okCancelTableLayoutPanel;
        private Button _okButton;
        private Button _cancelButton;

        private Label _memberTypeLabel;
        private ComboBox _columnsOrRowsComboBox;

        private GroupBox _sizeTypeGroupBox;
        private RadioButton _absoluteRadioButton;
        private RadioButton _percentRadioButton;
        private RadioButton _autoSizedRadioButton;

        private NavigationalTableLayoutPanel _sizeTypeTableLayoutPanel;
        private Label _pixelsLabel;
        private NumericUpDown _absoluteNumericUpDown;
        private Label _percentLabel;
        private NumericUpDown _percentNumericUpDown;

        private ListView _columnsAndRowsListView;
        private ColumnHeader _membersColumnHeader;
        private ColumnHeader _sizeTypeColumnHeader;
        private TableLayoutPanel _helperTextTableLayoutPanel;
        private PictureBox _infoPictureBox1;
        private PictureBox _infoPictureBox2;
        private LinkLabel _helperLinkLabel1;
        private LinkLabel _helperLinkLabel2;
        private TableLayoutPanel _showTableLayoutPanel;
        private ColumnHeader _valueColumnHeader;

        private const int UpDownLeftMargin = 10;
        private int _scaledUpDownLeftMargin = UpDownLeftMargin;

        private const int UpDownTopMargin = 4;
        private int _scaledUpDownTopMargin = UpDownTopMargin;

        private const int LabelRightMargin = 5;
        private int _scaledLabelRightMargin = LabelRightMargin;

        internal StyleEditorForm(CollectionEditor editor, bool isRowCollection) : base(editor)
        {
            _editor = (StyleCollectionEditor)editor;
            _isRowCollection = isRowCollection;
            InitializeComponent();
            HookEvents();

            // Enable Vista explorer list view style
            DesignerUtils.ApplyListViewThemeStyles(_columnsAndRowsListView);

            ActiveControl = _columnsAndRowsListView;
            _tableLayoutPanel = Context.Instance as TableLayoutPanel;
            _tableLayoutPanel.SuspendLayout();

            _deleteList = [];

            // Get the designer associated with the TLP
            var host = _tableLayoutPanel.Site.GetService<IDesignerHost>();
            if (host is not null)
            {
                _tableLayoutPanelDesigner = host.GetDesigner(_tableLayoutPanel) as TableLayoutPanelDesigner;
                _componentChangeService = host.GetService<IComponentChangeService>();
            }

            _rowStyleProp = TypeDescriptor.GetProperties(_tableLayoutPanel)["RowStyles"];
            _colStyleProp = TypeDescriptor.GetProperties(_tableLayoutPanel)["ColumnStyles"];

            _tableLayoutPanelDesigner.SuspendEnsureAvailableStyles();
        }

        private void HookEvents()
        {
            HelpButtonClicked += OnHelpButtonClicked;

            _columnsAndRowsListView.SelectedIndexChanged += OnListViewSelectedIndexChanged;
            _columnsOrRowsComboBox.SelectionChangeCommitted += OnComboBoxSelectionChangeCommitted;

            _okButton.Click += OnOkButtonClick;
            _cancelButton.Click += OnCancelButtonClick;

            _addButton.Click += OnAddButtonClick;
            _removeButton.Click += OnRemoveButtonClick;
            _insertButton.Click += OnInsertButtonClick;

            _absoluteRadioButton.Enter += OnAbsoluteEnter;
            _absoluteNumericUpDown.ValueChanged += OnValueChanged;

            _percentRadioButton.Enter += OnPercentEnter;
            _percentNumericUpDown.ValueChanged += OnValueChanged;

            _autoSizedRadioButton.Enter += OnAutoSizeEnter;

            _helperLinkLabel1.LinkClicked += OnLink1Click;
            _helperLinkLabel2.LinkClicked += OnLink2Click;

            FormClosed += StyleEditorClosed;
        }

        private void _closeButton_Click(object sender, EventArgs e) => throw new NotImplementedException();

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            ComponentResourceManager resources = new(typeof(StyleCollectionEditor));
            _addRemoveInsertTableLayoutPanel = new TableLayoutPanel();
            _addButton = new Button();
            _removeButton = new Button();
            _insertButton = new Button();
            _okCancelTableLayoutPanel = new TableLayoutPanel();
            _okButton = new Button();
            _cancelButton = new Button();
            _overarchingTableLayoutPanel = new TableLayoutPanel();
            _showTableLayoutPanel = new TableLayoutPanel();
            _memberTypeLabel = new Label();
            _columnsOrRowsComboBox = new ComboBox();
            _columnsAndRowsListView = new ListView();
            _membersColumnHeader = new ColumnHeader(resources.GetString("columnsAndRowsListView.Columns"));
            _sizeTypeColumnHeader = new ColumnHeader(resources.GetString("columnsAndRowsListView.Columns1"));
            _valueColumnHeader = new ColumnHeader(resources.GetString("columnsAndRowsListView.Columns2"));
            _helperTextTableLayoutPanel = new TableLayoutPanel();
            _infoPictureBox2 = new PictureBox();
            _infoPictureBox1 = new PictureBox();
            _helperLinkLabel1 = new LinkLabel();
            _helperLinkLabel2 = new LinkLabel();
            _sizeTypeGroupBox = new GroupBox();
            _sizeTypeTableLayoutPanel = new NavigationalTableLayoutPanel();
            _absoluteNumericUpDown = new NumericUpDown();
            _absoluteRadioButton = new RadioButton();
            _pixelsLabel = new Label();
            _percentLabel = new Label();
            _percentRadioButton = new RadioButton();
            _autoSizedRadioButton = new RadioButton();
            _percentNumericUpDown = new NumericUpDown();
            _addRemoveInsertTableLayoutPanel.SuspendLayout();
            _okCancelTableLayoutPanel.SuspendLayout();
            _overarchingTableLayoutPanel.SuspendLayout();
            _showTableLayoutPanel.SuspendLayout();
            _helperTextTableLayoutPanel.SuspendLayout();
            ((ISupportInitialize)_infoPictureBox2).BeginInit();
            ((ISupportInitialize)_infoPictureBox1).BeginInit();
            _sizeTypeGroupBox.SuspendLayout();
            _sizeTypeTableLayoutPanel.SuspendLayout();
            ((ISupportInitialize)_absoluteNumericUpDown).BeginInit();
            ((ISupportInitialize)_percentNumericUpDown).BeginInit();
            SuspendLayout();

            // addRemoveInsertTableLayoutPanel
            resources.ApplyResources(_addRemoveInsertTableLayoutPanel, "addRemoveInsertTableLayoutPanel");
            _addRemoveInsertTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33F));
            _addRemoveInsertTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33F));
            _addRemoveInsertTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33F));
            _addRemoveInsertTableLayoutPanel.Controls.Add(_addButton, 0, 0);
            _addRemoveInsertTableLayoutPanel.Controls.Add(_removeButton, 1, 0);
            _addRemoveInsertTableLayoutPanel.Controls.Add(_insertButton, 2, 0);
            _addRemoveInsertTableLayoutPanel.Margin = new Padding(0, 3, 3, 3);
            _addRemoveInsertTableLayoutPanel.Name = "addRemoveInsertTableLayoutPanel";
            _addRemoveInsertTableLayoutPanel.RowStyles.Add(new RowStyle());

            // addButton
            resources.ApplyResources(_addButton, "addButton");
            _addButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            _addButton.Margin = new Padding(0, 0, 4, 0);
            _addButton.MinimumSize = new Size(75, 23);
            _addButton.Name = "addButton";
            _addButton.Padding = new Padding(10, 0, 10, 0);

            // removeButton
            resources.ApplyResources(_removeButton, "removeButton");
            _removeButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            _removeButton.Margin = new Padding(2, 0, 2, 0);
            _removeButton.MinimumSize = new Size(75, 23);
            _removeButton.Name = "removeButton";
            _removeButton.Padding = new Padding(10, 0, 10, 0);

            // insertButton
            resources.ApplyResources(_insertButton, "insertButton");
            _insertButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            _insertButton.Margin = new Padding(4, 0, 0, 0);
            _insertButton.MinimumSize = new Size(75, 23);
            _insertButton.Name = "insertButton";
            _insertButton.Padding = new Padding(10, 0, 10, 0);

            // okCancelTableLayoutPanel
            resources.ApplyResources(_okCancelTableLayoutPanel, "okCancelTableLayoutPanel");
            _overarchingTableLayoutPanel.SetColumnSpan(_okCancelTableLayoutPanel, 2);
            _okCancelTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            _okCancelTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            _okCancelTableLayoutPanel.Controls.Add(_okButton, 0, 0);
            _okCancelTableLayoutPanel.Controls.Add(_cancelButton, 1, 0);
            _okCancelTableLayoutPanel.Margin = new Padding(0, 6, 0, 0);
            _okCancelTableLayoutPanel.Name = "okCancelTableLayoutPanel";
            _okCancelTableLayoutPanel.RowStyles.Add(new RowStyle());

            // okButton
            resources.ApplyResources(_okButton, "okButton");
            _okButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            _okButton.Margin = new Padding(0, 0, 3, 0);
            _okButton.MinimumSize = new Size(75, 23);
            _okButton.Name = "okButton";
            _okButton.Padding = new Padding(10, 0, 10, 0);

            // cancelButton
            resources.ApplyResources(_cancelButton, "cancelButton");
            _cancelButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            _cancelButton.DialogResult = DialogResult.Cancel;
            _cancelButton.Margin = new Padding(3, 0, 0, 0);
            _cancelButton.MinimumSize = new Size(75, 23);
            _cancelButton.Name = "cancelButton";
            _cancelButton.Padding = new Padding(10, 0, 10, 0);

            // overarchingTableLayoutPanel
            resources.ApplyResources(_overarchingTableLayoutPanel, "overarchingTableLayoutPanel");
            _overarchingTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            _overarchingTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            _overarchingTableLayoutPanel.Controls.Add(_sizeTypeGroupBox, 1, 0);
            _overarchingTableLayoutPanel.Controls.Add(_okCancelTableLayoutPanel, 0, 4);
            _overarchingTableLayoutPanel.Controls.Add(_showTableLayoutPanel, 0, 0);
            _overarchingTableLayoutPanel.Controls.Add(_addRemoveInsertTableLayoutPanel, 0, 3);
            _overarchingTableLayoutPanel.Controls.Add(_columnsAndRowsListView, 0, 1);
            _overarchingTableLayoutPanel.Controls.Add(_helperTextTableLayoutPanel, 1, 2);
            _overarchingTableLayoutPanel.Name = "overarchingTableLayoutPanel";
            _overarchingTableLayoutPanel.RowStyles.Add(new RowStyle());
            _overarchingTableLayoutPanel.RowStyles.Add(new RowStyle());
            _overarchingTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            _overarchingTableLayoutPanel.RowStyles.Add(new RowStyle());
            _overarchingTableLayoutPanel.RowStyles.Add(new RowStyle());

            // showTableLayoutPanel
            resources.ApplyResources(_showTableLayoutPanel, "showTableLayoutPanel");
            _showTableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
            _showTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            _showTableLayoutPanel.Controls.Add(_memberTypeLabel, 0, 0);
            _showTableLayoutPanel.Controls.Add(_columnsOrRowsComboBox, 1, 0);
            _showTableLayoutPanel.Margin = new Padding(0, 0, 3, 3);
            _showTableLayoutPanel.Name = "showTableLayoutPanel";
            _showTableLayoutPanel.RowStyles.Add(new RowStyle());

            // memberTypeLabel
            resources.ApplyResources(_memberTypeLabel, "memberTypeLabel");
            _memberTypeLabel.Margin = new Padding(0, 0, 3, 0);
            _memberTypeLabel.Name = "memberTypeLabel";

            // columnsOrRowsComboBox
            resources.ApplyResources(_columnsOrRowsComboBox, "columnsOrRowsComboBox");
            _columnsOrRowsComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            _columnsOrRowsComboBox.FormattingEnabled = true;
            _columnsOrRowsComboBox.Items.AddRange(
            [
                resources.GetString("columnsOrRowsComboBox.Items"),
                resources.GetString("columnsOrRowsComboBox.Items1")
            ]);
            _columnsOrRowsComboBox.Margin = new Padding(3, 0, 0, 0);
            _columnsOrRowsComboBox.Name = "columnsOrRowsComboBox";

            // columnsAndRowsListView
            resources.ApplyResources(_columnsAndRowsListView, "columnsAndRowsListView");
            _columnsAndRowsListView.Columns.AddRange(
            [
                 _membersColumnHeader,
                 _sizeTypeColumnHeader,
                 _valueColumnHeader
            ]);
            _columnsAndRowsListView.FullRowSelect = true;
            _columnsAndRowsListView.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            _columnsAndRowsListView.HideSelection = false;
            _columnsAndRowsListView.Margin = new Padding(0, 3, 3, 3);
            _columnsAndRowsListView.Name = "columnsAndRowsListView";
            _overarchingTableLayoutPanel.SetRowSpan(_columnsAndRowsListView, 2);
            _columnsAndRowsListView.View = View.Details;

            // membersColumnHeader
            resources.ApplyResources(_membersColumnHeader, "membersColumnHeader");

            // sizeTypeColumnHeader
            resources.ApplyResources(_sizeTypeColumnHeader, "sizeTypeColumnHeader");

            // valueColumnHeader
            resources.ApplyResources(_valueColumnHeader, "valueColumnHeader");

            // helperTextTableLayoutPanel
            resources.ApplyResources(_helperTextTableLayoutPanel, "helperTextTableLayoutPanel");
            _helperTextTableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
            _helperTextTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            _helperTextTableLayoutPanel.Controls.Add(_infoPictureBox2, 0, 1);
            _helperTextTableLayoutPanel.Controls.Add(_infoPictureBox1, 0, 0);
            _helperTextTableLayoutPanel.Controls.Add(_helperLinkLabel1, 1, 0);
            _helperTextTableLayoutPanel.Controls.Add(_helperLinkLabel2, 1, 1);
            _helperTextTableLayoutPanel.Margin = new Padding(6, 6, 0, 3);
            _helperTextTableLayoutPanel.Name = "helperTextTableLayoutPanel";
            _helperTextTableLayoutPanel.RowStyles.Add(new RowStyle());
            _helperTextTableLayoutPanel.RowStyles.Add(new RowStyle());

            // infoPictureBox2
            resources.ApplyResources(_infoPictureBox2, "infoPictureBox2");
            _infoPictureBox2.Name = "infoPictureBox2";
            _infoPictureBox2.TabStop = false;

            // infoPictureBox1
            resources.ApplyResources(_infoPictureBox1, "infoPictureBox1");
            _infoPictureBox1.Name = "infoPictureBox1";
            _infoPictureBox1.TabStop = false;
            _infoPictureBox1.Image = ScaleHelper.ScaleToDpi(_infoPictureBox1.Image as Bitmap, ScaleHelper.InitialSystemDpi);
            _infoPictureBox2.Image = ScaleHelper.ScaleToDpi(_infoPictureBox2.Image as Bitmap, ScaleHelper.InitialSystemDpi);
            _scaledUpDownLeftMargin = ScaleHelper.ScaleToInitialSystemDpi(UpDownLeftMargin);
            _scaledUpDownTopMargin = ScaleHelper.ScaleToInitialSystemDpi(UpDownTopMargin);
            _scaledLabelRightMargin = ScaleHelper.ScaleToInitialSystemDpi(LabelRightMargin);

            // helperLinkLabel1
            resources.ApplyResources(_helperLinkLabel1, "helperLinkLabel1");
            _helperLinkLabel1.Margin = new Padding(3, 0, 0, 3);
            _helperLinkLabel1.Name = "helperLinkLabel1";
            _helperLinkLabel1.TabStop = true;
            _helperLinkLabel1.UseCompatibleTextRendering = true;

            // helperLinkLabel2
            resources.ApplyResources(_helperLinkLabel2, "helperLinkLabel2");
            _helperLinkLabel2.Margin = new Padding(3, 3, 0, 0);
            _helperLinkLabel2.Name = "helperLinkLabel2";
            _helperLinkLabel2.TabStop = true;
            _helperLinkLabel2.UseCompatibleTextRendering = true;

            // sizeTypeGroupBox
            resources.ApplyResources(_sizeTypeGroupBox, "sizeTypeGroupBox");
            _sizeTypeGroupBox.Controls.Add(_sizeTypeTableLayoutPanel);
            _sizeTypeGroupBox.Margin = new Padding(6, 0, 0, 3);
            _sizeTypeGroupBox.Name = "sizeTypeGroupBox";
            _sizeTypeGroupBox.Padding = new Padding(0);
            _overarchingTableLayoutPanel.SetRowSpan(_sizeTypeGroupBox, 2);
            _sizeTypeGroupBox.TabStop = false;

            // sizeTypeTableLayoutPanel
            resources.ApplyResources(_sizeTypeTableLayoutPanel, "sizeTypeTableLayoutPanel");
            _sizeTypeTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3F));
            _sizeTypeTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3F));
            _sizeTypeTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3F));
            _sizeTypeTableLayoutPanel.Controls.Add(_absoluteNumericUpDown, 1, 0);
            _sizeTypeTableLayoutPanel.Controls.Add(_absoluteRadioButton, 0, 0);
            _sizeTypeTableLayoutPanel.Controls.Add(_pixelsLabel, 2, 0);
            _sizeTypeTableLayoutPanel.Controls.Add(_percentLabel, 2, 1);
            _sizeTypeTableLayoutPanel.Controls.Add(_percentRadioButton, 0, 1);
            _sizeTypeTableLayoutPanel.Controls.Add(_autoSizedRadioButton, 0, 2);
            _sizeTypeTableLayoutPanel.Controls.Add(_percentNumericUpDown, 1, 1);
            _sizeTypeTableLayoutPanel.Margin = new Padding(9);
            _sizeTypeTableLayoutPanel.Name = "sizeTypeTableLayoutPanel";
            _sizeTypeTableLayoutPanel.AutoSize = true;
            _sizeTypeTableLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            // absoluteNumericUpDown
            resources.ApplyResources(_absoluteNumericUpDown, "absoluteNumericUpDown");
            _absoluteNumericUpDown.Maximum = new decimal(99999u);

            _absoluteNumericUpDown.Name = "absoluteNumericUpDown";
            _absoluteNumericUpDown.Margin = new Padding(_scaledUpDownLeftMargin, _scaledUpDownTopMargin, 0, 0);
            _absoluteNumericUpDown.AutoScaleMode = AutoScaleMode.Font;

            // absoluteRadioButton
            resources.ApplyResources(_absoluteRadioButton, "absoluteRadioButton");
            _absoluteRadioButton.Margin = new Padding(0, 3, 3, 0);
            _absoluteRadioButton.Name = "absoluteRadioButton";

            // pixelsLabel
            resources.ApplyResources(_pixelsLabel, "pixelsLabel");
            _pixelsLabel.Name = "pixelsLabel";
            _pixelsLabel.Margin = new Padding(0, 0, _scaledLabelRightMargin, 0);

            // percentLabel
            resources.ApplyResources(_percentLabel, "percentLabel");
            _percentLabel.Name = "percentLabel";
            _percentLabel.Margin = new Padding(0, 0, _scaledLabelRightMargin, 0);

            // percentRadioButton
            resources.ApplyResources(_percentRadioButton, "percentRadioButton");
            _percentRadioButton.Margin = new Padding(0, 3, 3, 0);
            _percentRadioButton.Name = "percentRadioButton";

            // autoSizedRadioButton
            resources.ApplyResources(_autoSizedRadioButton, "autoSizedRadioButton");
            _autoSizedRadioButton.Margin = new Padding(0, 3, 3, 0);
            _autoSizedRadioButton.Name = "autoSizedRadioButton";

            // percentNumericUpDown
            resources.ApplyResources(_percentNumericUpDown, "percentNumericUpDown");
            _percentNumericUpDown.DecimalPlaces = 2;
            _percentNumericUpDown.Maximum = new decimal(9999u);
            _percentNumericUpDown.Name = "percentNumericUpDown";
            _percentNumericUpDown.Margin = new Padding(_scaledUpDownLeftMargin, _scaledUpDownTopMargin, 0, 0);
            _percentNumericUpDown.AutoScaleMode = AutoScaleMode.Font;

            // StyleCollectionEditor
            AcceptButton = _okButton;
            resources.ApplyResources(this, "$this");
            CancelButton = _cancelButton;
            Controls.Add(_overarchingTableLayoutPanel);
            HelpButton = true;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Form1";
            ShowIcon = false;
            ShowInTaskbar = false;
            _addRemoveInsertTableLayoutPanel.ResumeLayout(false);
            _addRemoveInsertTableLayoutPanel.PerformLayout();
            _okCancelTableLayoutPanel.ResumeLayout(false);
            _okCancelTableLayoutPanel.PerformLayout();
            _overarchingTableLayoutPanel.ResumeLayout(false);
            _overarchingTableLayoutPanel.PerformLayout();
            _showTableLayoutPanel.ResumeLayout(false);
            _showTableLayoutPanel.PerformLayout();
            _helperTextTableLayoutPanel.ResumeLayout(false);
            _helperTextTableLayoutPanel.PerformLayout();
            ((ISupportInitialize)_infoPictureBox2).EndInit();
            ((ISupportInitialize)_infoPictureBox1).EndInit();
            _sizeTypeGroupBox.ResumeLayout();
            _sizeTypeTableLayoutPanel.ResumeLayout(false);
            _sizeTypeTableLayoutPanel.PerformLayout();
            ((ISupportInitialize)_absoluteNumericUpDown).EndInit();
            ((ISupportInitialize)_percentNumericUpDown).EndInit();
            ResumeLayout(false);

            AutoScaleMode = AutoScaleMode.Font;
        }

        #endregion

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            // We need to set the dirty flag here, since the initialization down above, might
            // cause it to get set in one of the methods below.
            _isDialogDirty = false;
            _columnsAndRowsListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void OnLink1Click(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CancelEventArgs cancelEvent = new();
            _editor._helpTopic = "net.ComponentModel.StyleCollectionEditor.TLP.SpanRowsColumns";
            OnHelpButtonClicked(sender, cancelEvent);
        }

        private void OnLink2Click(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CancelEventArgs cancelEvent = new();
            _editor._helpTopic = "net.ComponentModel.StyleCollectionEditor.TLP.AnchorDock";
            OnHelpButtonClicked(sender, cancelEvent);
        }

        private void OnHelpButtonClicked(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            _editor._helpTopic = "net.ComponentModel.StyleCollectionEditor";
            _editor.ShowHelp();
        }

        protected override void OnEditValueChanged()
        {
        }

        protected internal override DialogResult ShowEditorDialog(IWindowsFormsEditorService edSvc)
        {
            if (_componentChangeService is not null)
            {
                if (_rowStyleProp is not null)
                {
                    _componentChangeService.OnComponentChanging(_tableLayoutPanel, _rowStyleProp);
                }

                if (_colStyleProp is not null)
                {
                    _componentChangeService.OnComponentChanging(_tableLayoutPanel, _colStyleProp);
                }
            }

            // We can't use ColumnCount/RowCount, since they are only guaranteed to reflect the true count
            // when GrowMode == Fixed
            int[] columnWidths = _tableLayoutPanel.GetColumnWidths();
            int[] rowHeights = _tableLayoutPanel.GetRowHeights();

            // If we have more, then let's remove the extra ones. This is because we don't want any new row/col that we might add
            // to inherit leftover styles. We want to make sure than any new rol/col is of type Absolute and of size 20.
            if (_tableLayoutPanel.ColumnStyles.Count > columnWidths.Length)
            {
                int diff = _tableLayoutPanel.ColumnStyles.Count - columnWidths.Length;
                for (int i = 0; i < diff; ++i)
                {
                    _tableLayoutPanel.ColumnStyles.RemoveAt(_tableLayoutPanel.ColumnStyles.Count - 1);
                }
            }

            if (_tableLayoutPanel.RowStyles.Count > rowHeights.Length)
            {
                int diff = _tableLayoutPanel.RowStyles.Count - rowHeights.Length;
                for (int i = 0; i < diff; ++i)
                {
                    _tableLayoutPanel.RowStyles.RemoveAt(_tableLayoutPanel.RowStyles.Count - 1);
                }
            }

            // This will cause the listView to be initialized
            _columnsOrRowsComboBox.SelectedIndex = _isRowCollection ? 1 : 0;
            InitListView();
            return base.ShowEditorDialog(edSvc);
        }

        private static string FormatValueString(SizeType type, float value)
        {
            if (type == SizeType.Absolute)
            {
                return value.ToString(CultureInfo.CurrentCulture);
            }
            else if (type == SizeType.Percent)
            {
                // value will be multiplied by 100, so let's adjust for that
                return (value / 100).ToString("P", CultureInfo.CurrentCulture);
            }
            else
            {
                return string.Empty;
            }
        }

        // Populate the listView with the correct values - happens when the dialog is brought up, or
        // when the user changes the selection in the comboBox
        private void InitListView()
        {
            _columnsAndRowsListView.Items.Clear();

            string baseName = _isRowCollection ? "Row" : "Column"; // these should not be localized

            int styleCount = _isRowCollection ? _tableLayoutPanel.RowStyles.Count : _tableLayoutPanel.ColumnStyles.Count;

            for (int i = 0; i < styleCount; ++i)
            {
                string sizeType;
                string sizeValue;

                if (_isRowCollection)
                {
                    RowStyle rowStyle = _tableLayoutPanel.RowStyles[i];
                    sizeType = rowStyle.SizeType.ToString();
                    sizeValue = FormatValueString(rowStyle.SizeType, rowStyle.Height);
                }
                else
                {
                    ColumnStyle colStyle = _tableLayoutPanel.ColumnStyles[i];
                    sizeType = colStyle.SizeType.ToString();
                    sizeValue = FormatValueString(colStyle.SizeType, colStyle.Width);
                }

                // We add 1, since we want the Member to read <Column|Row>1,2,3...
                _columnsAndRowsListView.Items.Add(new ListViewItem([baseName + (i + 1).ToString(CultureInfo.InvariantCulture), sizeType, sizeValue]));
            }

            if (styleCount > 0)
            {
                ClearAndSetSelectionAndFocus(0);
            }

            // We should already have something selected
            _removeButton.Enabled = _columnsAndRowsListView.Items.Count > 1;
        }

        private void UpdateListViewItem(int index, string member, string type, string value)
        {
            _columnsAndRowsListView.Items[index].SubItems[MemberIndex].Text = member;
            _columnsAndRowsListView.Items[index].SubItems[TypeIndex].Text = type;
            _columnsAndRowsListView.Items[index].SubItems[ValueIndex].Text = value;
        }

        private void UpdateListViewMember()
        {
            // let's do a for loop rather than for-each, don't have to do the object creation
            for (int i = 0; i < _columnsAndRowsListView.Items.Count; ++i)
            {
                _columnsAndRowsListView.Items[i].SubItems[MemberIndex].Text = (_isRowCollection ? "Row" : "Column") + (i + 1).ToString(CultureInfo.InvariantCulture);
            }
        }

        private void OnComboBoxSelectionChangeCommitted(object sender, EventArgs e)
        {
            _isRowCollection = _columnsOrRowsComboBox.SelectedIndex != 0;
            InitListView();
        }

        private void OnListViewSelectedIndexChanged(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection coll = _columnsAndRowsListView.SelectedItems;

            if (coll.Count == 0)
            {
                // When the selection changes from one item to another, we will get a temporary state
                // where the selection collection is empty. We don't want to disable buttons in this case
                // since that will cause flashing, so let's do a beginInvoke here. In the delegate we check
                // if the collection really is empty and then disable buttons and whatever else we have to do.
                if (!_haveInvoked)
                {
                    BeginInvoke(new EventHandler(OnListSelectionComplete));
                    _haveInvoked = true;
                }

                return;
            }

            _sizeTypeGroupBox.Enabled = true;
            _insertButton.Enabled = true;
            _removeButton.Enabled = coll.Count != _columnsAndRowsListView.Items.Count && _columnsAndRowsListView.Items.Count > 1;

            if (coll.Count == 1)
            {
                // Get the index
                int index = _columnsAndRowsListView.Items.IndexOf(coll[0]);
                if (_isRowCollection)
                {
                    UpdateGroupBox(_tableLayoutPanel.RowStyles[index].SizeType, _tableLayoutPanel.RowStyles[index].Height);
                }
                else
                {
                    UpdateGroupBox(_tableLayoutPanel.ColumnStyles[index].SizeType, _tableLayoutPanel.ColumnStyles[index].Width);
                }
            }
            else
            {
                // Multi-selection
                // Check if all the items in the selection are of the same type and value
                SizeType type;
                float value = 0;
                bool sameValues = true;
                int index = _columnsAndRowsListView.Items.IndexOf(coll[0]);

                type = _isRowCollection ? _tableLayoutPanel.RowStyles[index].SizeType : _tableLayoutPanel.ColumnStyles[index].SizeType;
                value = _isRowCollection ? _tableLayoutPanel.RowStyles[index].Height : _tableLayoutPanel.ColumnStyles[index].Width;
                for (int i = 1; i < coll.Count; i++)
                {
                    index = _columnsAndRowsListView.Items.IndexOf(coll[i]);
                    if (type != (_isRowCollection ? _tableLayoutPanel.RowStyles[index].SizeType : _tableLayoutPanel.ColumnStyles[index].SizeType))
                    {
                        sameValues = false;
                        break;
                    }

                    if (value != (_isRowCollection ? _tableLayoutPanel.RowStyles[index].Height : _tableLayoutPanel.ColumnStyles[index].Width))
                    {
                        sameValues = false;
                        break;
                    }
                }

                if (!sameValues)
                {
                    ResetAllRadioButtons();
                }
                else
                {
                    UpdateGroupBox(type, value);
                }
            }
        }

        private void OnListSelectionComplete(object sender, EventArgs e)
        {
            _haveInvoked = false;

            // Nothing is truly selected in the listView
            if (_columnsAndRowsListView.SelectedItems.Count == 0)
            {
                ResetAllRadioButtons();
                _sizeTypeGroupBox.Enabled = false;
                _insertButton.Enabled = false;
                _removeButton.Enabled = false;
            }
        }

        private void ResetAllRadioButtons()
        {
            _absoluteRadioButton.Checked = false;
            ResetAbsolute();

            _percentRadioButton.Checked = false;
            ResetPercent();

            _autoSizedRadioButton.Checked = false;
        }

        private void ResetAbsolute()
        {
            // Unhook the event while we reset.
            // If we didn't the setting the value would cause OnValueChanged below to get called.
            // If we then go ahead and update the listView, which we don't want in the reset case.
            _absoluteNumericUpDown.ValueChanged -= OnValueChanged;
            _absoluteNumericUpDown.Enabled = false;
            _absoluteNumericUpDown.Value = DesignerUtils.s_minimumStyleSize;
            _absoluteNumericUpDown.ValueChanged += OnValueChanged;
        }

        private void ResetPercent()
        {
            // Unhook the event while we reset.
            // If we didn't the setting the value would cause OnValueChanged below to get called.
            // If we then go ahead and update the listView, which we don't want in the reset case.
            _percentNumericUpDown.ValueChanged -= OnValueChanged;
            _percentNumericUpDown.Enabled = false;
            _percentNumericUpDown.Value = DesignerUtils.s_minimumStylePercent;
            _percentNumericUpDown.ValueChanged += OnValueChanged;
        }

        private void UpdateGroupBox(SizeType type, float value)
        {
            switch (type)
            {
                case SizeType.Absolute:
                    _absoluteRadioButton.Checked = true;
                    _absoluteNumericUpDown.Enabled = true;
                    try
                    {
                        _absoluteNumericUpDown.Value = (decimal)value;
                    }

                    catch (ArgumentOutOfRangeException)
                    {
                        _absoluteNumericUpDown.Value = DesignerUtils.s_minimumStyleSize;
                    }

                    ResetPercent();
                    break;
                case SizeType.Percent:
                    _percentRadioButton.Checked = true;
                    _percentNumericUpDown.Enabled = true;
                    try
                    {
                        _percentNumericUpDown.Value = (decimal)value;
                    }

                    catch (ArgumentOutOfRangeException)
                    {
                        _percentNumericUpDown.Value = DesignerUtils.s_minimumStylePercent;
                    }

                    ResetAbsolute();
                    break;
                case SizeType.AutoSize:
                    _autoSizedRadioButton.Checked = true;
                    ResetAbsolute();
                    ResetPercent();
                    break;
                default:
                    Debug.Fail("Unsupported SizeType");
                    break;
            }
        }

        private void ClearAndSetSelectionAndFocus(int index)
        {
            _columnsAndRowsListView.BeginUpdate();
            _columnsAndRowsListView.Focus();
            if (_columnsAndRowsListView.FocusedItem is not null)
            {
                _columnsAndRowsListView.FocusedItem.Focused = false;
            }

            _columnsAndRowsListView.SelectedItems.Clear();
            _columnsAndRowsListView.Items[index].Selected = true;
            _columnsAndRowsListView.Items[index].Focused = true;
            _columnsAndRowsListView.Items[index].EnsureVisible();
            _columnsAndRowsListView.EndUpdate();
        }

        /// <summary>
        ///  Adds an item to the listView at the specified index
        /// </summary>
        private void AddItem(int index)
        {
            _tableLayoutPanelDesigner.InsertRowCol(_isRowCollection, index);

            string member = _isRowCollection
                ? $"Row{_tableLayoutPanel.RowStyles.Count}"
                : $"Column{_tableLayoutPanel.RowStyles.Count}";

            _columnsAndRowsListView.Items.Insert(
                index,
                new ListViewItem(
                [
                    member, SizeType.Absolute.ToString(),
                    DesignerUtils.s_minimumStyleSize.ToString(CultureInfo.InvariantCulture)
                ]));

            // If we inserted at the beginning, then we have to change the Member of string of all the existing listView items,
            // so we might as well just update the entire listView.
            UpdateListViewMember();
            ClearAndSetSelectionAndFocus(index);
        }

        private void OnAddButtonClick(object sender, EventArgs e)
        {
            _isDialogDirty = true;

            // Add an item to the end of the listView
            AddItem(_columnsAndRowsListView.Items.Count);
        }

        private void OnInsertButtonClick(object sender, EventArgs e)
        {
            _isDialogDirty = true;

            // Insert an item before the 1st selected item
            AddItem(_columnsAndRowsListView.SelectedIndices[0]);
            _tableLayoutPanelDesigner.FixUpControlsOnInsert(_isRowCollection, _columnsAndRowsListView.SelectedIndices[0]);
        }

        private void OnRemoveButtonClick(object sender, EventArgs e)
        {
            if ((_columnsAndRowsListView.Items.Count == 1)
                || (_columnsAndRowsListView.Items.Count == _columnsAndRowsListView.SelectedIndices.Count))
            {
                // We can't remove anything when we have just 1 row/col or if all rows/cols are selected
                return;
            }

            _isDialogDirty = true;

            // Store the new index
            int newIndex = _columnsAndRowsListView.SelectedIndices[0];

            // Remove from the end of the selection -- that way we have less things to adjust
            for (int i = _columnsAndRowsListView.SelectedIndices.Count - 1; i >= 0; i--)
            {
                int index = _columnsAndRowsListView.SelectedIndices[i];

                // First update any controls in any row/col that's AFTER the row/col we are deleting
                _tableLayoutPanelDesigner.FixUpControlsOnDelete(_isRowCollection, index, _deleteList);

                // Then remove the row/col
                _tableLayoutPanelDesigner.DeleteRowCol(_isRowCollection, index);

                // Then remove the listView item
                if (_isRowCollection)
                {
                    _columnsAndRowsListView.Items.RemoveAt(index);
                }
                else
                {
                    _columnsAndRowsListView.Items.RemoveAt(index);
                }
            }

            if (newIndex >= _columnsAndRowsListView.Items.Count)
            {
                newIndex -= 1;
            }

            // If we removed at the beginning, then we have to change the Member of string of all the existing listView items,
            // so we might as well just update the entire listView.
            UpdateListViewMember();
            ClearAndSetSelectionAndFocus(newIndex);
        }

        private void UpdateTypeAndValue(SizeType type, float value)
        {
            for (int i = 0; i < _columnsAndRowsListView.SelectedIndices.Count; i++)
            {
                int index = _columnsAndRowsListView.SelectedIndices[i];

                if (_isRowCollection)
                {
                    _tableLayoutPanel.RowStyles[index].SizeType = type;
                    _tableLayoutPanel.RowStyles[index].Height = value;
                }
                else
                {
                    _tableLayoutPanel.ColumnStyles[index].SizeType = type;
                    _tableLayoutPanel.ColumnStyles[index].Width = value;
                }

                UpdateListViewItem(index, _columnsAndRowsListView.Items[index].SubItems[MemberIndex].Text, type.ToString(), FormatValueString(type, value));
            }
        }

        private void OnAbsoluteEnter(object sender, EventArgs e)
        {
            _isDialogDirty = true;
            UpdateTypeAndValue(SizeType.Absolute, (float)_absoluteNumericUpDown.Value);
            _absoluteNumericUpDown.Enabled = true;
            ResetPercent();
        }

        private void OnPercentEnter(object sender, EventArgs e)
        {
            _isDialogDirty = true;
            UpdateTypeAndValue(SizeType.Percent, (float)_percentNumericUpDown.Value);
            _percentNumericUpDown.Enabled = true;
            ResetAbsolute();
        }

        private void OnAutoSizeEnter(object sender, EventArgs e)
        {
            _isDialogDirty = true;
            UpdateTypeAndValue(SizeType.AutoSize, 0);
            ResetAbsolute();
            ResetPercent();
        }

        private void OnValueChanged(object sender, EventArgs e)
        {
            if (_absoluteNumericUpDown == sender && _absoluteRadioButton.Checked)
            {
                _isDialogDirty = true;
                UpdateTypeAndValue(SizeType.Absolute, (float)_absoluteNumericUpDown.Value);
            }
            else if (_percentNumericUpDown == sender && _percentRadioButton.Checked)
            {
                _isDialogDirty = true;
                UpdateTypeAndValue(SizeType.Percent, (float)_percentNumericUpDown.Value);
            }
        }

        private void NormalizePercentStyle(bool normalizeRow)
        {
            int count = normalizeRow ? _tableLayoutPanel.RowStyles.Count : _tableLayoutPanel.ColumnStyles.Count;
            float total = 0;

            // Loop through an calculate the percentage total
            for (int i = 0; i < count; i++)
            {
                if (normalizeRow)
                {
                    if (_tableLayoutPanel.RowStyles[i].SizeType != SizeType.Percent)
                    {
                        continue;
                    }

                    total += _tableLayoutPanel.RowStyles[i].Height;
                }
                else
                {
                    if (_tableLayoutPanel.ColumnStyles[i].SizeType != SizeType.Percent)
                    {
                        continue;
                    }

                    total += _tableLayoutPanel.ColumnStyles[i].Width;
                }
            }

            if (total is 100 or 0)
            {
                return;
            }

            // Now loop through and set the normalized value
            for (int i = 0; i < count; i++)
            {
                if (normalizeRow)
                {
                    if (_tableLayoutPanel.RowStyles[i].SizeType != SizeType.Percent)
                    {
                        continue;
                    }

                    _tableLayoutPanel.RowStyles[i].Height = (_tableLayoutPanel.RowStyles[i].Height * 100) / total;
                }
                else
                {
                    if (_tableLayoutPanel.ColumnStyles[i].SizeType != SizeType.Percent)
                    {
                        continue;
                    }

                    _tableLayoutPanel.ColumnStyles[i].Width = (_tableLayoutPanel.ColumnStyles[i].Width * 100) / total;
                }
            }
        }

        /// <summary>
        ///  Take all the styles of SizeType.Percent and normalize them to 100%
        /// </summary>
        private void NormalizePercentStyles()
        {
            NormalizePercentStyle(normalizeRow: true);
            NormalizePercentStyle(normalizeRow: false);
        }

        private void OnOkButtonClick(object sender, EventArgs e)
        {
            try
            {
                if (_isDialogDirty)
                {
                    if (_absoluteRadioButton.Checked)
                    {
                        UpdateTypeAndValue(SizeType.Absolute, (float)_absoluteNumericUpDown.Value);
                    }
                    else if (_percentRadioButton.Checked)
                    {
                        UpdateTypeAndValue(SizeType.Percent, (float)_percentNumericUpDown.Value);
                    }
                    else if (_autoSizedRadioButton.Checked)
                    {
                        UpdateTypeAndValue(SizeType.AutoSize, 0);
                    }

                    // Now normalize all percentages.
                    NormalizePercentStyles();

                    // If you change this,you should also change the code in TableLayoutPanelDesigner.OnRemoveInternal
                    if (_deleteList.Count > 0)
                    {
                        PropertyDescriptor childProperty = TypeDescriptor.GetProperties(_tableLayoutPanel)[nameof(TableLayoutPanel.Controls)];
                        if (_componentChangeService is not null && childProperty is not null)
                        {
                            _componentChangeService.OnComponentChanging(_tableLayoutPanel, childProperty);
                        }

                        var host = _tableLayoutPanel.Site.GetService<IDesignerHost>();

                        if (host is not null)
                        {
                            foreach (object obj in _deleteList)
                            {
                                List<IComponent> componentList = [];
                                DesignerUtils.GetAssociatedComponents((IComponent)obj, host, componentList);
                                foreach (IComponent component in componentList)
                                {
                                    _componentChangeService.OnComponentChanging(component, null);
                                }

                                host.DestroyComponent(obj as Component);
                            }
                        }

                        if (_componentChangeService is not null && childProperty is not null)
                        {
                            _componentChangeService.OnComponentChanged(_tableLayoutPanel, childProperty, null, null);
                        }
                    }

                    if (_componentChangeService is not null)
                    {
                        if (_rowStyleProp is not null)
                        {
                            _componentChangeService.OnComponentChanged(_tableLayoutPanel, _rowStyleProp, null, null);
                        }

                        if (_colStyleProp is not null)
                        {
                            _componentChangeService.OnComponentChanged(_tableLayoutPanel, _colStyleProp, null, null);
                        }
                    }

                    DialogResult = DialogResult.OK;
                }
                else
                {
                    DialogResult = DialogResult.Cancel;
                }
            }
            finally
            {
                _performEnsure = true;
            }
        }

        private void OnCancelButtonClick(object sender, EventArgs e)
        {
            _performEnsure = false;
            DialogResult = DialogResult.Cancel;
        }

        private void StyleEditorClosed(object sender, FormClosedEventArgs e)
        {
            _tableLayoutPanelDesigner.ResumeEnsureAvailableStyles(_performEnsure);
            _tableLayoutPanel.ResumeLayout();
        }
    }
}
