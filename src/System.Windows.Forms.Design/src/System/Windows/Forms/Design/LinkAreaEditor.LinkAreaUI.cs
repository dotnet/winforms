// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;

namespace System.Windows.Forms.Design;

internal partial class LinkAreaEditor
{
    /// <summary>
    ///  Dialog box for the link area.
    /// </summary>
    internal class LinkAreaUI : Form
    {
        private Label _caption = new();
        private TextBox _sampleEdit = new();
        private Button _okButton = new();
        private Button _cancelButton = new();
        private TableLayoutPanel _okCancelTableLayoutPanel;
        private readonly IHelpService? _helpService;

        public LinkAreaUI(IHelpService? helpService)
        {
            _helpService = helpService;
            InitializeComponent();
        }

        [AllowNull]
        public string SampleText
        {
            get => _sampleEdit.Text;
            set
            {
                _sampleEdit.Text = value;
                UpdateSelection();
            }
        }

        public object? Value { get; private set; }

        public void End() => Value = null;

        [MemberNotNull(nameof(_caption))]
        [MemberNotNull(nameof(_sampleEdit))]
        [MemberNotNull(nameof(_okButton))]
        [MemberNotNull(nameof(_cancelButton))]
        [MemberNotNull(nameof(_okCancelTableLayoutPanel))]
        private void InitializeComponent()
        {
            ComponentResourceManager resources = new(typeof(LinkAreaEditor));
            _caption = new Label();
            _sampleEdit = new TextBox();
            _okButton = new Button();
            _cancelButton = new Button();
            _okCancelTableLayoutPanel = new TableLayoutPanel();
            _okCancelTableLayoutPanel.SuspendLayout();
            SuspendLayout();
            _okButton.Click += okButton_click;

            // caption
            resources.ApplyResources(_caption, "caption");
            _caption.Margin = new Padding(3, 1, 3, 0);
            _caption.Name = "caption";

            // sampleEdit
            resources.ApplyResources(_sampleEdit, "sampleEdit");
            _sampleEdit.Margin = new Padding(3, 2, 3, 3);
            _sampleEdit.Name = "sampleEdit";
            _sampleEdit.HideSelection = false;
            _sampleEdit.ScrollBars = ScrollBars.Vertical;

            // okButton
            resources.ApplyResources(_okButton, "okButton");
            _okButton.DialogResult = DialogResult.OK;
            _okButton.Margin = new Padding(0, 0, 2, 0);
            _okButton.Name = "okButton";

            // cancelButton
            resources.ApplyResources(_cancelButton, "cancelButton");
            _cancelButton.DialogResult = DialogResult.Cancel;
            _cancelButton.Margin = new Padding(3, 0, 0, 0);
            _cancelButton.Name = "cancelButton";

            // okCancelTableLayoutPanel
            resources.ApplyResources(_okCancelTableLayoutPanel, "okCancelTableLayoutPanel");
            _okCancelTableLayoutPanel.ColumnCount = 2;
            _okCancelTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            _okCancelTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            _okCancelTableLayoutPanel.Controls.Add(_okButton, 0, 0);
            _okCancelTableLayoutPanel.Controls.Add(_cancelButton, 1, 0);
            _okCancelTableLayoutPanel.Margin = new Padding(3, 1, 3, 3);
            _okCancelTableLayoutPanel.Name = "okCancelTableLayoutPanel";
            _okCancelTableLayoutPanel.RowCount = 1;
            _okCancelTableLayoutPanel.RowStyles.Add(new RowStyle());
            _okCancelTableLayoutPanel.RowStyles.Add(new RowStyle());

            // LinkAreaEditor
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = _cancelButton;
            Controls.Add(_okCancelTableLayoutPanel);
            Controls.Add(_sampleEdit);
            Controls.Add(_caption);
            HelpButton = true;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "LinkAreaEditor";
            ShowIcon = false;
            ShowInTaskbar = false;
            HelpButtonClicked += LinkAreaEditor_HelpButtonClicked;
            _okCancelTableLayoutPanel.ResumeLayout(false);
            _okCancelTableLayoutPanel.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        private void okButton_click(object? sender, EventArgs e)
        {
            Value = new LinkArea(_sampleEdit.SelectionStart, _sampleEdit.SelectionLength);
        }

        private void LinkAreaEditor_HelpButtonClicked(object? sender, CancelEventArgs e)
        {
            e.Cancel = true;
            _helpService?.ShowHelpFromKeyword("net.ComponentModel.LinkAreaEditor");
        }

        public void Start(object? value)
        {
            Value = value;
            UpdateSelection();
            ActiveControl = _sampleEdit;
        }

        private void UpdateSelection()
        {
            if (Value is not LinkArea linkArea)
            {
                return;
            }

            try
            {
                _sampleEdit.SelectionStart = linkArea.Start;
                _sampleEdit.SelectionLength = linkArea.Length;
            }
            catch (Exception ex) when (!ex.IsCriticalException())
            {
            }
        }
    }
}
