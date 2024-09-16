// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;

namespace System.Windows.Forms.Design;

internal partial class StringCollectionEditor
{
    /// <summary>
    ///  StringCollectionForm allows visible editing of a string array.
    ///  Each line in the edit box is an array entry.
    /// </summary>
    private class StringCollectionForm : CollectionForm
    {
        private Label _instruction;
        private TextBox _textEntry;
        private Button _okButton;
        private Button _cancelButton;
        private TableLayoutPanel _overarchingLayoutPanel;

        private readonly StringCollectionEditor _editor;

        /// <summary>
        ///  Constructs a StringCollectionForm.
        /// </summary>
        public StringCollectionForm(CollectionEditor editor)
            : base(editor)
        {
            _editor = (StringCollectionEditor)editor;
            InitializeComponent();
            HookEvents();
        }

        private void Edit1_keyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Escape)
            {
                return;
            }

            _cancelButton.PerformClick();
            e.Handled = true;
        }

        private void StringCollectionEditor_HelpButtonClicked(object? sender, CancelEventArgs e)
        {
            e.Cancel = true;
            _editor.ShowHelp();
        }

        private void Form_HelpRequested(object? sender, HelpEventArgs e)
        {
            _editor.ShowHelp();
        }

        private void HookEvents()
        {
            _textEntry.KeyDown += Edit1_keyDown;
            _okButton.Click += OKButton_click;
            HelpButtonClicked += StringCollectionEditor_HelpButtonClicked;
        }

        /// <summary>
        ///  NOTE: The following code is required by the form designer.
        ///  It can be modified using the form editor. Do not modify it using the code editor.
        /// </summary>
        [MemberNotNull(nameof(_instruction))]
        [MemberNotNull(nameof(_textEntry))]
        [MemberNotNull(nameof(_okButton))]
        [MemberNotNull(nameof(_cancelButton))]
        [MemberNotNull(nameof(_overarchingLayoutPanel))]
        private void InitializeComponent()
        {
            ComponentResourceManager resources = new(typeof(StringCollectionEditor));
            _instruction = new Label();
            _textEntry = new TextBox();
            _okButton = new Button();
            _cancelButton = new Button();
            _overarchingLayoutPanel = new TableLayoutPanel();
            _overarchingLayoutPanel.SuspendLayout();
            SuspendLayout();
            // instruction
            //
            resources.ApplyResources(_instruction, "instruction");
            _overarchingLayoutPanel.SetColumnSpan(_instruction, 2);
            _instruction.Name = "instruction";
            //
            // textEntry
            //
            resources.ApplyResources(_textEntry, "textEntry");
            _overarchingLayoutPanel.SetColumnSpan(_textEntry, 2);
            _textEntry.AcceptsTab = true;
            _textEntry.AcceptsReturn = true;
            _textEntry.Name = "textEntry";
            //
            // okButton
            //
            resources.ApplyResources(_okButton, "okButton");
            _okButton.DialogResult = DialogResult.OK;
            _okButton.Name = "okButton";
            //
            // cancelButton
            //
            resources.ApplyResources(_cancelButton, "cancelButton");
            _cancelButton.DialogResult = DialogResult.Cancel;
            _cancelButton.Name = "cancelButton";
            //
            // overarchingLayoutPanel
            //
            resources.ApplyResources(_overarchingLayoutPanel, "overarchingLayoutPanel");
            _overarchingLayoutPanel.Controls.Add(_instruction, 0, 0);
            _overarchingLayoutPanel.Controls.Add(_textEntry, 0, 2);
            _overarchingLayoutPanel.Controls.Add(_okButton, 0, 3);
            _overarchingLayoutPanel.Controls.Add(_cancelButton, 1, 3);
            _overarchingLayoutPanel.Name = "overarchingLayoutPanel";
            //
            // StringCollectionEditor
            //
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(_overarchingLayoutPanel);
            HelpButton = true;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "StringCollectionEditor";
            ShowIcon = false;
            ShowInTaskbar = false;
            _overarchingLayoutPanel.ResumeLayout(false);
            _overarchingLayoutPanel.PerformLayout();
            HelpRequested += Form_HelpRequested;
            ResumeLayout(false);
            PerformLayout();
        }

        /// <summary>
        ///  Commits the changes to the editor.
        /// </summary>
        private void OKButton_click(object? sender, EventArgs e)
        {
            // Split the text into array of lines.
            string[] lines = _textEntry.Text.Split('\n');

            // Remove trailing carriage return characters.
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = lines[i].TrimEnd('\r');
            }

            // Check if the content has changed.
            if (lines.Length != Items.Length)
            {
                UpdateItems(lines);
                return;
            }

            for (int i = 0; i < lines.Length; ++i)
            {
                if (!lines[i].Equals(Items[i]?.ToString()))
                {
                    UpdateItems(lines);
                    return;
                }
            }

            DialogResult = DialogResult.Cancel;

            void UpdateItems(string[] newLines)
            {
                // If the last line is empty, we don't want to create an item from it.
                if (newLines[^1].Length == 0)
                {
                    Array.Resize(ref newLines, newLines.Length - 1);
                }

                // Assign newLines to Items.
                Items = newLines;
            }
        }

        /// <summary>
        ///  This is called when the value property in the CollectionForm has changed.
        ///  In it you should update your user interface to reflect the current value.
        /// </summary>
        protected override void OnEditValueChanged()
            => _textEntry.Text = string.Join(Environment.NewLine, Items);
    }
}
