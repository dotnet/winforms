// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms;

namespace System.ComponentModel.Design;

public sealed partial class BinaryEditor
{
    private class BinaryUI : Form
    {
        private readonly BinaryEditor _editor;
        private object? _value;

        private RadioButton _radioAuto;
        private Button _buttonSave;
        private Button _buttonOK;
        private ByteViewer _byteViewer;
        private GroupBox _groupBoxMode;
        private RadioButton _radioHex;
        private RadioButton _radioAnsi;
        private TableLayoutPanel _radioButtonsTableLayoutPanel;
        private TableLayoutPanel _okSaveTableLayoutPanel;
        private TableLayoutPanel _overarchingTableLayoutPanel;
        private RadioButton _radioUnicode;

        public BinaryUI(BinaryEditor editor)
        {
            _editor = editor;
            InitializeComponent();
        }

        public object? Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                byte[]? bytes = null;

                if (value is not null)
                {
                    bytes = ConvertToBytes(value);
                }

                if (bytes is not null)
                {
                    _byteViewer.SetBytes(bytes);
                    _byteViewer.Enabled = true;
                }
                else
                {
                    _byteViewer.SetBytes([]);
                    _byteViewer.Enabled = false;
                }
            }
        }

        private void RadioAuto_checkedChanged(object? source, EventArgs e)
        {
            if (_radioAuto.Checked)
            {
                _byteViewer.SetDisplayMode(DisplayMode.Auto);
            }
        }

        private void RadioHex_checkedChanged(object? source, EventArgs e)
        {
            if (_radioHex.Checked)
            {
                _byteViewer.SetDisplayMode(DisplayMode.Hexdump);
            }
        }

        private void RadioAnsi_checkedChanged(object? source, EventArgs e)
        {
            if (_radioAnsi.Checked)
            {
                _byteViewer.SetDisplayMode(DisplayMode.Ansi);
            }
        }

        private void RadioUnicode_checkedChanged(object? source, EventArgs e)
        {
            if (_radioUnicode.Checked)
            {
                _byteViewer.SetDisplayMode(DisplayMode.Unicode);
            }
        }

        private void ButtonOK_click(object? source, EventArgs e)
        {
            object localValue = _value!;
            ConvertToValue(_byteViewer.GetBytes(), ref localValue);
            _value = localValue;
        }

        private void ButtonSave_click(object? source, EventArgs e)
        {
            try
            {
                SaveFileDialog sfd = new()
                {
                    FileName = SR.BinaryEditorFileName,
                    Title = SR.BinaryEditorSaveFile,
                    Filter = SR.BinaryEditorAllFiles + " (*.*)|*.*"
                };

                DialogResult result = sfd.ShowDialog();
                if (result == DialogResult.OK)
                {
                    _byteViewer.SaveToFile(sfd.FileName);
                }
            }
            catch (IOException x)
            {
                RTLAwareMessageBox.Show(null, string.Format(SR.BinaryEditorFileError, x.Message),
                                SR.BinaryEditorTitle, MessageBoxButtons.OK,
                                MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, 0);
            }
        }

        private void Form_HelpRequested(object? sender, HelpEventArgs e)
        {
            _editor.ShowHelp();
        }

        private void Form_HelpButtonClicked(object? sender, CancelEventArgs e)
        {
            e.Cancel = true;
            _editor.ShowHelp();
        }

        [MemberNotNull(nameof(_byteViewer))]
        [MemberNotNull(nameof(_buttonOK))]
        [MemberNotNull(nameof(_buttonSave))]
        [MemberNotNull(nameof(_groupBoxMode))]
        [MemberNotNull(nameof(_radioButtonsTableLayoutPanel))]
        [MemberNotNull(nameof(_radioUnicode))]
        [MemberNotNull(nameof(_radioAuto))]
        [MemberNotNull(nameof(_radioAnsi))]
        [MemberNotNull(nameof(_radioHex))]
        [MemberNotNull(nameof(_okSaveTableLayoutPanel))]
        [MemberNotNull(nameof(_overarchingTableLayoutPanel))]
        private void InitializeComponent()
        {
            ComponentResourceManager resources = new(typeof(BinaryEditor));
            _byteViewer = new ByteViewer();
            _buttonOK = new Button();
            _buttonSave = new Button();
            _groupBoxMode = new GroupBox();
            _radioButtonsTableLayoutPanel = new TableLayoutPanel();
            _radioUnicode = new RadioButton();
            _radioAuto = new RadioButton();
            _radioAnsi = new RadioButton();
            _radioHex = new RadioButton();
            _okSaveTableLayoutPanel = new TableLayoutPanel();
            _overarchingTableLayoutPanel = new TableLayoutPanel();
            _byteViewer.SuspendLayout();
            _groupBoxMode.SuspendLayout();
            _radioButtonsTableLayoutPanel.SuspendLayout();
            _okSaveTableLayoutPanel.SuspendLayout();
            _overarchingTableLayoutPanel.SuspendLayout();
            SuspendLayout();

            //
            // byteViewer
            //
            resources.ApplyResources(_byteViewer, "byteViewer");
            _byteViewer.SetDisplayMode(DisplayMode.Auto);
            _byteViewer.Name = "byteViewer";
            _byteViewer.Margin = Padding.Empty;
            _byteViewer.Dock = DockStyle.Fill;
            //
            // buttonOK
            //
            resources.ApplyResources(_buttonOK, "buttonOK");
            _buttonOK.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            _buttonOK.DialogResult = DialogResult.OK;
            _buttonOK.Margin = new Padding(0, 0, 3, 0);
            _buttonOK.MinimumSize = new Size(75, 23);
            _buttonOK.Name = "buttonOK";
            _buttonOK.Padding = new Padding(10, 0, 10, 0);
            _buttonOK.Click += ButtonOK_click;
            //
            // buttonSave
            //
            resources.ApplyResources(_buttonSave, "buttonSave");
            _buttonSave.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            _buttonSave.Margin = new Padding(3, 0, 0, 0);
            _buttonSave.MinimumSize = new Size(75, 23);
            _buttonSave.Name = "buttonSave";
            _buttonSave.Padding = new Padding(10, 0, 10, 0);
            _buttonSave.Click += ButtonSave_click;
            //
            // groupBoxMode
            //
            resources.ApplyResources(_groupBoxMode, "groupBoxMode");
            _groupBoxMode.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            _groupBoxMode.Controls.Add(_radioButtonsTableLayoutPanel);
            _groupBoxMode.Margin = new Padding(0, 3, 0, 3);
            _groupBoxMode.Name = "groupBoxMode";
            _groupBoxMode.Padding = new Padding(0);
            _groupBoxMode.TabStop = false;
            //
            // radioButtonsTableLayoutPanel
            //
            resources.ApplyResources(_radioButtonsTableLayoutPanel, "radioButtonsTableLayoutPanel");
            _radioButtonsTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            _radioButtonsTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            _radioButtonsTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            _radioButtonsTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            _radioButtonsTableLayoutPanel.Controls.Add(_radioUnicode, 3, 0);
            _radioButtonsTableLayoutPanel.Controls.Add(_radioAuto, 0, 0);
            _radioButtonsTableLayoutPanel.Controls.Add(_radioAnsi, 2, 0);
            _radioButtonsTableLayoutPanel.Controls.Add(_radioHex, 1, 0);
            _radioButtonsTableLayoutPanel.Margin = new Padding(9);
            _radioButtonsTableLayoutPanel.Name = "radioButtonsTableLayoutPanel";
            _radioButtonsTableLayoutPanel.RowStyles.Add(new RowStyle());
            //
            // radioUnicode
            //
            resources.ApplyResources(_radioUnicode, "radioUnicode");
            _radioUnicode.Margin = new Padding(3, 0, 0, 0);
            _radioUnicode.Name = "radioUnicode";
            _radioUnicode.CheckedChanged += RadioUnicode_checkedChanged;
            //
            // radioAuto
            //
            resources.ApplyResources(_radioAuto, "radioAuto");
            _radioAuto.Checked = true;
            _radioAuto.Margin = new Padding(0, 0, 3, 0);
            _radioAuto.Name = "radioAuto";
            _radioAuto.CheckedChanged += RadioAuto_checkedChanged;
            //
            // radioAnsi
            //
            resources.ApplyResources(_radioAnsi, "radioAnsi");
            _radioAnsi.Margin = new Padding(3, 0, 3, 0);
            _radioAnsi.Name = "radioAnsi";
            _radioAnsi.CheckedChanged += RadioAnsi_checkedChanged;
            //
            // radioHex
            //
            resources.ApplyResources(_radioHex, "radioHex");
            _radioHex.Margin = new Padding(3, 0, 3, 0);
            _radioHex.Name = "radioHex";
            _radioHex.CheckedChanged += RadioHex_checkedChanged;
            //
            // okSaveTableLayoutPanel
            //
            resources.ApplyResources(_okSaveTableLayoutPanel, "okSaveTableLayoutPanel");
            _okSaveTableLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            _okSaveTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            _okSaveTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            _okSaveTableLayoutPanel.Controls.Add(_buttonOK, 0, 0);
            _okSaveTableLayoutPanel.Controls.Add(_buttonSave, 1, 0);
            _okSaveTableLayoutPanel.Margin = new Padding(0, 9, 0, 0);
            _okSaveTableLayoutPanel.Name = "okSaveTableLayoutPanel";
            _okSaveTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            //
            // overarchingTableLayoutPanel
            //
            resources.ApplyResources(_overarchingTableLayoutPanel, "overarchingTableLayoutPanel");
            _overarchingTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            _overarchingTableLayoutPanel.Controls.Add(_byteViewer, 0, 0);
            _overarchingTableLayoutPanel.Controls.Add(_groupBoxMode, 0, 1);
            _overarchingTableLayoutPanel.Controls.Add(_okSaveTableLayoutPanel, 0, 2);
            _overarchingTableLayoutPanel.Name = "overarchingTableLayoutPanel";
            _overarchingTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            _overarchingTableLayoutPanel.RowStyles.Add(new RowStyle());
            _overarchingTableLayoutPanel.RowStyles.Add(new RowStyle());

            //
            // BinaryUI
            //
            AcceptButton = _buttonOK;
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = _buttonOK;
            Controls.Add(_overarchingTableLayoutPanel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            HelpButton = true;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "BinaryUI";
            ShowIcon = false;
            ShowInTaskbar = false;
            HelpRequested += Form_HelpRequested;
            HelpButtonClicked += Form_HelpButtonClicked;
            _byteViewer.ResumeLayout(false);
            _byteViewer.PerformLayout();
            _groupBoxMode.ResumeLayout(false);
            _groupBoxMode.PerformLayout();
            _radioButtonsTableLayoutPanel.ResumeLayout(false);
            _radioButtonsTableLayoutPanel.PerformLayout();
            _okSaveTableLayoutPanel.ResumeLayout(false);
            _okSaveTableLayoutPanel.PerformLayout();
            _overarchingTableLayoutPanel.ResumeLayout(false);
            _overarchingTableLayoutPanel.PerformLayout();
            ResumeLayout(false);
        }
    }
}
