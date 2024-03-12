// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms.Design;

public partial class ShortcutKeysEditor
{
    /// <summary>
    ///  Editor UI for the shortcut keys editor.
    /// </summary>
    private class ShortcutKeysUI : UserControl
    {
        /// <summary>
        ///  Array of keys that are present in the drop down list of the combo box.
        /// </summary>
        private static readonly Keys[] s_validKeys =
        [
            Keys.A, Keys.B, Keys.C, Keys.D, Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7,
            Keys.D8, Keys.D9, Keys.Delete, Keys.Down, Keys.E, Keys.End, Keys.F, Keys.F1, Keys.F10, Keys.F11,
            Keys.F12, Keys.F13, Keys.F14, Keys.F15, Keys.F16, Keys.F17, Keys.F18, Keys.F19, Keys.F2, Keys.F20,
            Keys.F21, Keys.F22, Keys.F23, Keys.F24, Keys.F3, Keys.F4, Keys.F5, Keys.F6, Keys.F7, Keys.F8, Keys.F9,
            Keys.G, Keys.H, Keys.I, Keys.Insert, Keys.J, Keys.K, Keys.L, Keys.Left, Keys.M, Keys.N, Keys.NumLock,
            Keys.NumPad0, Keys.NumPad1, Keys.NumPad2, Keys.NumPad3, Keys.NumPad4, Keys.NumPad5, Keys.NumPad6,
            Keys.NumPad7, Keys.NumPad8, Keys.NumPad9, Keys.O, Keys.OemBackslash, Keys.OemClear, Keys.OemCloseBrackets,
            Keys.Oemcomma, Keys.OemMinus, Keys.OemOpenBrackets, Keys.OemPeriod, Keys.OemPipe, Keys.Oemplus,
            Keys.OemQuestion, Keys.OemQuotes, Keys.OemSemicolon, Keys.Oemtilde, Keys.P, Keys.Pause, Keys.Q, Keys.R,
            Keys.Right, Keys.S, Keys.Space, Keys.T, Keys.Tab, Keys.U, Keys.Up, Keys.V, Keys.W, Keys.X, Keys.Y, Keys.Z
        ];

        private Button _resetButton;
        private CheckBox _altCheckBox;
        private CheckBox _ctrlCheckBox;
        private CheckBox _shiftCheckBox;
        private ComboBox _keyComboBox;
        private TypeConverter? _keysConverter;
        private Label _keyLabel;
        private Label _modifiersLabel;
        private object? _originalValue;
        private object? _currentValue;
        private TableLayoutPanel _innerPanel;
        private TableLayoutPanel _outerPanel;
        private Keys _unknownKeyCode;
        private bool _updateCurrentValue;

        public ShortcutKeysUI()
        {
            _keysConverter = null;
            End();
            InitializeComponent();
            AdjustSize();

            // Looking for duplicates in validKeys
            Debug.Assert(s_validKeys.Distinct().Count() == s_validKeys.Length);
        }

        /// <summary>
        ///  Returns the Keys type converter.
        /// </summary>
        private TypeConverter KeysConverter => _keysConverter ??= TypeDescriptor.GetConverter(typeof(Keys));

        /// <summary>
        ///  Returns the selected keys. If only modifiers were selected, we return Keys.None.
        /// </summary>
        public object? Value
        {
            get
            {
                if (_currentValue is Keys currentKeys && (currentKeys & Keys.KeyCode) == 0)
                {
                    return Keys.None;
                }

                return _currentValue;
            }
        }

        /// <summary>
        ///  Triggered when the user clicks the Reset button. The value is set to Keys.None
        /// </summary>
        private void OnResetButtonClick(object? sender, EventArgs e)
        {
            _ctrlCheckBox.Checked = false;
            _altCheckBox.Checked = false;
            _shiftCheckBox.Checked = false;
            _keyComboBox.SelectedIndex = -1;
        }

        private void OnCheckedChanged(object? sender, EventArgs e) => UpdateCurrentValue();

        private void OnSelectedIndexChanged(object? sender, EventArgs e) => UpdateCurrentValue();

        public void End()
        {
            _originalValue = null;
            _currentValue = null;
            _updateCurrentValue = false;
            if (_unknownKeyCode != Keys.None)
            {
                _keyComboBox.Items.RemoveAt(0);
                _unknownKeyCode = Keys.None;
            }
        }

        [MemberNotNull(nameof(_outerPanel))]
        [MemberNotNull(nameof(_modifiersLabel))]
        [MemberNotNull(nameof(_ctrlCheckBox))]
        [MemberNotNull(nameof(_altCheckBox))]
        [MemberNotNull(nameof(_shiftCheckBox))]
        [MemberNotNull(nameof(_innerPanel))]
        [MemberNotNull(nameof(_keyLabel))]
        [MemberNotNull(nameof(_keyComboBox))]
        [MemberNotNull(nameof(_resetButton))]
        private void InitializeComponent()
        {
            ComponentResourceManager resources = new(typeof(ShortcutKeysEditor));

            _outerPanel = new TableLayoutPanel();
            _modifiersLabel = new Label();
            _ctrlCheckBox = new CheckBox();
            _altCheckBox = new CheckBox();
            _shiftCheckBox = new CheckBox();
            _innerPanel = new TableLayoutPanel();
            _keyLabel = new Label();
            _keyComboBox = new ComboBox();
            _resetButton = new Button();
            _outerPanel.SuspendLayout();
            _innerPanel.SuspendLayout();
            SuspendLayout();

            // Outer Panel
            resources.ApplyResources(_outerPanel, "tlpOuter");
            _outerPanel.ColumnCount = 3;
            _outerPanel.ColumnStyles.Add(new ColumnStyle());
            _outerPanel.ColumnStyles.Add(new ColumnStyle());
            _outerPanel.ColumnStyles.Add(new ColumnStyle());
            _outerPanel.Controls.Add(_modifiersLabel, 0, 0);
            _outerPanel.Controls.Add(_ctrlCheckBox, 0, 1);
            _outerPanel.Controls.Add(_shiftCheckBox, 1, 1);
            _outerPanel.Controls.Add(_altCheckBox, 2, 1);
            _outerPanel.Name = "tlpOuter";
            _outerPanel.RowCount = 2;
            _outerPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            _outerPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));

            // Modifiers Label
            resources.ApplyResources(_modifiersLabel, "lblModifiers");
            _outerPanel.SetColumnSpan(_modifiersLabel, 3);
            _modifiersLabel.Name = "lblModifiers";

            // Ctrl CheckBox
            resources.ApplyResources(_ctrlCheckBox, "chkCtrl");
            _ctrlCheckBox.Name = "chkCtrl";

            // This margin setting makes this control left-aligned with the key combo box and indents from the labels.
            _ctrlCheckBox.Margin = new Padding(12, 3, 3, 3);

            _ctrlCheckBox.CheckedChanged += OnCheckedChanged;

            // Alt CheckBox
            resources.ApplyResources(_altCheckBox, "chkAlt");
            _altCheckBox.Name = "chkAlt";

            _altCheckBox.CheckedChanged += OnCheckedChanged;

            // Shift CheckBox
            resources.ApplyResources(_shiftCheckBox, "chkShift");
            _shiftCheckBox.Name = "chkShift";

            _shiftCheckBox.CheckedChanged += OnCheckedChanged;

            // Inner Panel
            resources.ApplyResources(_innerPanel, "tlpInner");
            _innerPanel.ColumnCount = 2;
            _innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            _innerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            _innerPanel.Controls.Add(_keyLabel, 0, 0);
            _innerPanel.Controls.Add(_keyComboBox, 0, 1);
            _innerPanel.Controls.Add(_resetButton, 1, 1);
            _innerPanel.Name = "tlpInner";
            _innerPanel.RowCount = 2;
            _innerPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            _innerPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            // Key Label
            resources.ApplyResources(_keyLabel, "lblKey");
            _innerPanel.SetColumnSpan(_keyLabel, 2);
            _keyLabel.Name = "lblKey";

            // Key ComboBox
            resources.ApplyResources(_keyComboBox, "cmbKey");
            _keyComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            _keyComboBox.Name = "cmbKey";

            // This margin setting makes this control align with the Ctrl CheckBox and indents from the labels.
            // The top margin makes the ComboBox and Reset Button align properly
            _keyComboBox.Margin = new Padding(12, 4, 3, 3);
            _keyComboBox.Padding = _keyComboBox.Margin;

            foreach (Keys keyCode in s_validKeys)
            {
                _keyComboBox.Items.Add(KeysConverter.ConvertToString(keyCode)!);
            }

            _keyComboBox.SelectedIndexChanged += OnSelectedIndexChanged;

            // ResetButton
            resources.ApplyResources(_resetButton, "btnReset");
            _resetButton.Name = "btnReset";

            _resetButton.Click += OnResetButtonClick;

            resources.ApplyResources(this, "$this");
            Controls.AddRange([_innerPanel, _outerPanel]);
            Name = "ShortcutKeysUI";
            Padding = new Padding(4);

            _outerPanel.ResumeLayout(false);
            _outerPanel.PerformLayout();
            _innerPanel.ResumeLayout(false);
            _innerPanel.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        private void AdjustSize()
        {
            ComponentResourceManager resources = new(typeof(ShortcutKeysEditor));
            Size resetButtonSize = (Size)resources.GetObject("btnReset.Size")!;
            Size = new Size(Size.Width + _resetButton.Size.Width - resetButtonSize.Width, Size.Height);
        }

        /// <summary>
        ///  Returns true if the given key is part of the valid keys array.
        /// </summary>
        private static bool IsValidKey(Keys keyCode)
        {
            Debug.Assert((keyCode & Keys.KeyCode) == keyCode);

            return s_validKeys.Contains(keyCode);
        }

        /// <summary>
        ///  The Ctrl checkbox gets the focus by default.
        /// </summary>
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            _ctrlCheckBox.Focus();
        }

        /// <summary>
        ///  Fix keyboard navigation and handle escape key.
        /// </summary>
        protected override bool ProcessDialogKey(Keys keyData)
        {
            Keys keyCode = keyData & Keys.KeyCode;
            Keys keyModifiers = keyData & Keys.Modifiers;
            switch (keyCode)
            {
                // We shouldn't have to handle this. Could be a bug in the table layout panel?
                case Keys.Tab:
                    if (keyModifiers == Keys.Shift &&
                        _ctrlCheckBox.Focused)
                    {
                        _resetButton.Focus();
                        return true;
                    }

                    break;

                case Keys.Left:
                    if ((keyModifiers & (Keys.Control | Keys.Alt)) == 0)
                    {
                        if (_ctrlCheckBox.Focused)
                        {
                            _resetButton.Focus();
                            return true;
                        }
                    }

                    break;

                case Keys.Right:
                    if ((keyModifiers & (Keys.Control | Keys.Alt)) == 0)
                    {
                        if (_shiftCheckBox.Focused)
                        {
                            _keyComboBox.Focus();
                            return true;
                        }

                        if (_resetButton.Focused)
                        {
                            _ctrlCheckBox.Focus();
                            return true;
                        }
                    }

                    break;

                case Keys.Escape:
                    if (!_keyComboBox.Focused ||
                        (keyModifiers & (Keys.Control | Keys.Alt)) != 0 ||
                        !_keyComboBox.DroppedDown)
                    {
                        _currentValue = _originalValue;
                    }

                    break;
            }

            return base.ProcessDialogKey(keyData);
        }

        /// <summary>
        ///  Triggered whenever the user drops down the editor.
        /// </summary>
        public void Start(object? value)
        {
            Debug.Assert(!_updateCurrentValue);
            _originalValue = _currentValue = value;

            Keys keys = value is Keys keys1 ? keys1 : Keys.None;
            _ctrlCheckBox.Checked = (keys & Keys.Control) != 0;
            _altCheckBox.Checked = (keys & Keys.Alt) != 0;
            _shiftCheckBox.Checked = (keys & Keys.Shift) != 0;

            Keys keyCode = keys & Keys.KeyCode;
            if (keyCode == Keys.None)
            {
                _keyComboBox.SelectedIndex = -1;
            }
            else if (IsValidKey(keyCode))
            {
                _keyComboBox.SelectedItem = KeysConverter.ConvertToString(keyCode);
            }
            else
            {
                _keyComboBox.Items.Insert(0, SR.ShortcutKeys_InvalidKey);
                _keyComboBox.SelectedIndex = 0;
                _unknownKeyCode = keyCode;
            }

            _updateCurrentValue = true;
        }

        /// <summary>
        ///  Update the current value based on the state of the UI controls.
        /// </summary>
        private void UpdateCurrentValue()
        {
            if (!_updateCurrentValue)
            {
                return;
            }

            int cmbKeySelectedIndex = _keyComboBox.SelectedIndex;
            Keys valueKeys = Keys.None;
            if (_ctrlCheckBox.Checked)
            {
                valueKeys |= Keys.Control;
            }

            if (_altCheckBox.Checked)
            {
                valueKeys |= Keys.Alt;
            }

            if (_shiftCheckBox.Checked)
            {
                valueKeys |= Keys.Shift;
            }

            if (_unknownKeyCode != Keys.None && cmbKeySelectedIndex == 0)
            {
                valueKeys |= _unknownKeyCode;
            }
            else if (cmbKeySelectedIndex != -1)
            {
                valueKeys |= s_validKeys[_unknownKeyCode == Keys.None ? cmbKeySelectedIndex : cmbKeySelectedIndex - 1];
            }

            _currentValue = valueKeys;
        }
    }
}
