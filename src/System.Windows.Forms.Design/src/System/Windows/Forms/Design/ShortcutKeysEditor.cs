// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  Provides an editor for picking shortcut keys.
    /// </summary>
    [CLSCompliant(false)]
    public class ShortcutKeysEditor : UITypeEditor
    {
        private ShortcutKeysUI _shortcutKeysUI;

        /// <summary>
        ///  Edits the given object value using the editor style provided by ShortcutKeysEditor.GetEditStyle.
        /// </summary>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider is null)
            {
                return value;
            }
            if (!(provider.GetService(typeof(IWindowsFormsEditorService)) is IWindowsFormsEditorService edSvc))
            {
                return value;
            }

            if (_shortcutKeysUI is null)
            {
                _shortcutKeysUI = new ShortcutKeysUI(this)
                {
                    BackColor = SystemColors.Control
                };
            }

            _shortcutKeysUI.Start(edSvc, value);
            edSvc.DropDownControl(_shortcutKeysUI);

            if (_shortcutKeysUI.Value != null)
            {
                value = _shortcutKeysUI.Value;
            }

            _shortcutKeysUI.End();
            return value;
        }

        /// <summary>
        ///  Gets the editing style of the Edit method.
        /// </summary>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        /// <summary>
        ///  Editor UI for the shortcut keys editor.
        /// </summary>
        private class ShortcutKeysUI : UserControl
        {
            /// <summary>
            ///  Array of keys that are present in the drop down list of the combo box.
            /// </summary>
            private static readonly Keys[] validKeys =
            {
                Keys.A, Keys.B, Keys.C, Keys.D, Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7,
                Keys.D8, Keys.D9,
                Keys.Delete, Keys.Down, Keys.E, Keys.End, Keys.F, Keys.F1, Keys.F10, Keys.F11, Keys.F12, Keys.F13,
                Keys.F14, Keys.F15,
                Keys.F16, Keys.F17, Keys.F18, Keys.F19, Keys.F2, Keys.F20, Keys.F21, Keys.F22, Keys.F23, Keys.F24,
                Keys.F3, Keys.F4,
                Keys.F5, Keys.F6, Keys.F7, Keys.F8, Keys.F9, Keys.G, Keys.H, Keys.I, Keys.Insert, Keys.J, Keys.K,
                Keys.L, Keys.Left,
                Keys.M, Keys.N, Keys.NumLock, Keys.NumPad0, Keys.NumPad1, Keys.NumPad2, Keys.NumPad3, Keys.NumPad4,
                Keys.NumPad5,
                Keys.NumPad6, Keys.NumPad7, Keys.NumPad8, Keys.NumPad9, Keys.O, Keys.OemBackslash, Keys.OemClear,
                Keys.OemCloseBrackets,
                Keys.Oemcomma, Keys.OemMinus, Keys.OemOpenBrackets, Keys.OemPeriod, Keys.OemPipe, Keys.Oemplus,
                Keys.OemQuestion,
                Keys.OemQuotes, Keys.OemSemicolon, Keys.Oemtilde, Keys.P, Keys.Pause, Keys.Q, Keys.R, Keys.Right,
                Keys.S, Keys.Space,
                Keys.T, Keys.Tab, Keys.U, Keys.Up, Keys.V, Keys.W, Keys.X, Keys.Y, Keys.Z
            };

            private Button btnReset;
            private CheckBox chkAlt;
            private CheckBox chkCtrl;
            private CheckBox chkShift;
            private ComboBox cmbKey;
            private readonly ShortcutKeysEditor editor;
            private TypeConverter keysConverter;
            private Label lblKey;
            private Label lblModifiers;
            private object originalValue, currentValue;
            private TableLayoutPanel tlpInner;
            private TableLayoutPanel tlpOuter;
            private Keys unknownKeyCode;
            private bool updateCurrentValue;

            public ShortcutKeysUI(ShortcutKeysEditor editor)
            {
                this.editor = editor;
                keysConverter = null;
                End();
                InitializeComponent();
                AdjustSize();

#if DEBUG
                // Looking for duplicates in validKeys
                int keyCount = validKeys.Length;
                for (int key1 = 0; key1 < keyCount - 1; key1++)
                {
                    for (int key2 = key1 + 1; key2 < keyCount; key2++)
                    {
                        Debug.Assert((int)validKeys[key1] != (int)validKeys[key2]);
                    }
                }
#endif
            }

            /// <summary>
            ///  Allows someone else to close our dropdown.
            /// </summary>
            // Can be called through reflection.
            public IWindowsFormsEditorService EditorService
            {
                get;
                private set;
            }

            /// <summary>
            ///  Returns the Keys' type converter.
            /// </summary>
            private TypeConverter KeysConverter
            {
                get
                {
                    if (keysConverter is null)
                    {
                        keysConverter = TypeDescriptor.GetConverter(typeof(Keys));
                    }

                    Debug.Assert(keysConverter != null);
                    return keysConverter;
                }
            }

            /// <summary>
            ///  Returns the selected keys. If only modifers were selected, we return Keys.None.
            /// </summary>
            public object Value
            {
                get
                {
                    if (currentValue is Keys currentKeys && (currentKeys & Keys.KeyCode) == 0)
                    {
                        return Keys.None;
                    }

                    return currentValue;
                }
            }

            /// <summary>
            ///  Triggered when the user clicks the Reset button. The value is set to Keys.None
            /// </summary>
            private void btnReset_Click(object sender, EventArgs e)
            {
                chkCtrl.Checked = false;
                chkAlt.Checked = false;
                chkShift.Checked = false;
                cmbKey.SelectedIndex = -1;
            }

            private void chkModifier_CheckedChanged(object sender, EventArgs e)
            {
                UpdateCurrentValue();
            }

            private void cmbKey_SelectedIndexChanged(object sender, EventArgs e)
            {
                UpdateCurrentValue();
            }

            public void End()
            {
                EditorService = null;
                originalValue = null;
                currentValue = null;
                updateCurrentValue = false;
                if (unknownKeyCode != Keys.None)
                {
                    cmbKey.Items.RemoveAt(0);
                    unknownKeyCode = Keys.None;
                }
            }

            private void InitializeComponent()
            {
                ComponentResourceManager resources = new ComponentResourceManager(typeof(ShortcutKeysEditor));

                tlpOuter = new TableLayoutPanel();
                lblModifiers = new Label();
                chkCtrl = new CheckBox();
                chkAlt = new CheckBox();
                chkShift = new CheckBox();
                tlpInner = new TableLayoutPanel();
                lblKey = new Label();
                cmbKey = new ComboBox();
                btnReset = new Button();
                tlpOuter.SuspendLayout();
                tlpInner.SuspendLayout();
                SuspendLayout();

                //
                // tlpOuter
                //
                resources.ApplyResources(tlpOuter, "tlpOuter");
                tlpOuter.ColumnCount = 3;
                tlpOuter.ColumnStyles.Add(new ColumnStyle());
                tlpOuter.ColumnStyles.Add(new ColumnStyle());
                tlpOuter.ColumnStyles.Add(new ColumnStyle());
                tlpOuter.Controls.Add(lblModifiers, 0, 0);
                tlpOuter.Controls.Add(chkCtrl, 0, 1);
                tlpOuter.Controls.Add(chkShift, 1, 1);
                tlpOuter.Controls.Add(chkAlt, 2, 1);
                tlpOuter.Name = "tlpOuter";
                tlpOuter.RowCount = 2;
                tlpOuter.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
                tlpOuter.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));

                //
                // lblModifiers
                //
                resources.ApplyResources(lblModifiers, "lblModifiers");
                tlpOuter.SetColumnSpan(lblModifiers, 3);
                lblModifiers.Name = "lblModifiers";

                //
                // chkCtrl
                //
                resources.ApplyResources(chkCtrl, "chkCtrl");
                chkCtrl.Name = "chkCtrl";
                // this margin setting makes this control left-align with cmbKey and indented from lblModifiers/lblKey
                chkCtrl.Margin = new Padding(12, 3, 3, 3);

                chkCtrl.CheckedChanged += chkModifier_CheckedChanged;

                //
                // chkAlt
                //
                resources.ApplyResources(chkAlt, "chkAlt");
                chkAlt.Name = "chkAlt";

                chkAlt.CheckedChanged += chkModifier_CheckedChanged;

                //
                // chkShift
                //
                resources.ApplyResources(chkShift, "chkShift");
                chkShift.Name = "chkShift";

                chkShift.CheckedChanged += chkModifier_CheckedChanged;

                //
                // tlpInner
                //
                resources.ApplyResources(tlpInner, "tlpInner");
                tlpInner.ColumnCount = 2;
                tlpInner.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                tlpInner.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                tlpInner.Controls.Add(lblKey, 0, 0);
                tlpInner.Controls.Add(cmbKey, 0, 1);
                tlpInner.Controls.Add(btnReset, 1, 1);
                tlpInner.Name = "tlpInner";
                tlpInner.RowCount = 2;
                tlpInner.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
                tlpInner.RowStyles.Add(new RowStyle(SizeType.AutoSize));

                //
                // lblKey
                //
                resources.ApplyResources(lblKey, "lblKey");
                tlpInner.SetColumnSpan(lblKey, 2);
                lblKey.Name = "lblKey";

                //
                // cmbKey
                //
                resources.ApplyResources(cmbKey, "cmbKey");
                cmbKey.DropDownStyle = ComboBoxStyle.DropDownList;
                cmbKey.Name = "cmbKey";
                // this margin setting makes this control align with chkCtrl and indented from lblModifiers/lblKey
                // the top margin makes the combobox and btnReset align properly
                cmbKey.Margin = new Padding(12, 4, 3, 3);
                cmbKey.Padding = cmbKey.Margin;

                foreach (Keys keyCode in validKeys)
                {
                    cmbKey.Items.Add(KeysConverter.ConvertToString(keyCode));
                }

                cmbKey.SelectedIndexChanged += cmbKey_SelectedIndexChanged;

                //
                // btnReset
                //
                resources.ApplyResources(btnReset, "btnReset");
                btnReset.Name = "btnReset";

                btnReset.Click += btnReset_Click;

                resources.ApplyResources(this, "$this");
                Controls.AddRange(new Control[] { tlpInner, tlpOuter });
                Name = "ShortcutKeysUI";
                Padding = new Padding(4);

                tlpOuter.ResumeLayout(false);
                tlpOuter.PerformLayout();
                tlpInner.ResumeLayout(false);
                tlpInner.PerformLayout();
                ResumeLayout(false);
                PerformLayout();
            }

            private void AdjustSize()
            {
                ComponentResourceManager resources = new ComponentResourceManager(typeof(ShortcutKeysEditor));
                Size btnResetSize = (Size)resources.GetObject("btnReset.Size");
                Size = new Size(Size.Width + btnReset.Size.Width - btnResetSize.Width, Size.Height);
            }

            /// <summary>
            ///  Returns True if the given key is part of the valid keys array.
            /// </summary>
            private static bool IsValidKey(Keys keyCode)
            {
                Debug.Assert((keyCode & Keys.KeyCode) == keyCode);
                foreach (Keys validKeyCode in validKeys)
                {
                    if (validKeyCode == keyCode)
                    {
                        return true;
                    }
                }

                return false;
            }

            /// <summary>
            ///  The Ctrl checkbox gets the focus by default.
            /// </summary>
            protected override void OnGotFocus(EventArgs e)
            {
                base.OnGotFocus(e);
                chkCtrl.Focus();
            }

            /// <summary>
            ///  Fix keyboard navigation and handle escape key
            /// </summary>
            protected override bool ProcessDialogKey(Keys keyData)
            {
                Keys keyCode = keyData & Keys.KeyCode;
                Keys keyModifiers = keyData & Keys.Modifiers;
                switch (keyCode)
                {
                    // REGISB: We shouldn't have to handle this. Could be a bug in the table layout panel. Check it out.
                    case Keys.Tab:
                        if (keyModifiers == Keys.Shift &&
                            chkCtrl.Focused)
                        {
                            btnReset.Focus();
                            return true;
                        }

                        break;

                    case Keys.Left:
                        if ((keyModifiers & (Keys.Control | Keys.Alt)) == 0)
                        {
                            if (chkCtrl.Focused)
                            {
                                btnReset.Focus();
                                return true;
                            }
                        }

                        break;

                    case Keys.Right:
                        if ((keyModifiers & (Keys.Control | Keys.Alt)) == 0)
                        {
                            if (chkShift.Focused)
                            {
                                cmbKey.Focus();
                                return true;
                            }

                            if (btnReset.Focused)
                            {
                                chkCtrl.Focus();
                                return true;
                            }
                        }

                        break;

                    case Keys.Escape:
                        if (!cmbKey.Focused ||
                            (keyModifiers & (Keys.Control | Keys.Alt)) != 0 ||
                            !cmbKey.DroppedDown)
                        {
                            currentValue = originalValue;
                        }

                        break;
                }

                return base.ProcessDialogKey(keyData);
            }

            /// <summary>
            ///  Triggered whenever the user drops down the editor.
            /// </summary>
            public void Start(IWindowsFormsEditorService edSvc, object value)
            {
                Debug.Assert(edSvc != null);
                Debug.Assert(!updateCurrentValue);
                EditorService = edSvc;
                originalValue = currentValue = value;

                Keys keys = value is Keys ? (Keys)value : Keys.None;
                chkCtrl.Checked = (keys & Keys.Control) != 0;
                chkAlt.Checked = (keys & Keys.Alt) != 0;
                chkShift.Checked = (keys & Keys.Shift) != 0;

                Keys keyCode = keys & Keys.KeyCode;
                if (keyCode == Keys.None)
                {
                    cmbKey.SelectedIndex = -1;
                }
                else if (IsValidKey(keyCode))
                {
                    cmbKey.SelectedItem = KeysConverter.ConvertToString(keyCode);
                }
                else
                {
                    cmbKey.Items.Insert(0, SR.ShortcutKeys_InvalidKey);
                    cmbKey.SelectedIndex = 0;
                    unknownKeyCode = keyCode;
                }

                updateCurrentValue = true;
            }

            /// <summary>
            ///  Update the current value based on the state of the UI controls.
            /// </summary>
            private void UpdateCurrentValue()
            {
                if (!updateCurrentValue)
                {
                    return;
                }

                int cmbKeySelectedIndex = cmbKey.SelectedIndex;
                Keys valueKeys = Keys.None;
                if (chkCtrl.Checked)
                {
                    valueKeys |= Keys.Control;
                }

                if (chkAlt.Checked)
                {
                    valueKeys |= Keys.Alt;
                }

                if (chkShift.Checked)
                {
                    valueKeys |= Keys.Shift;
                }

                if (unknownKeyCode != Keys.None && cmbKeySelectedIndex == 0)
                {
                    valueKeys |= unknownKeyCode;
                }
                else if (cmbKeySelectedIndex != -1)
                {
                    valueKeys |= validKeys[unknownKeyCode == Keys.None ? cmbKeySelectedIndex : cmbKeySelectedIndex - 1];
                }

                currentValue = valueKeys;
            }
        }
    }
}
