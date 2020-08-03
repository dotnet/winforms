// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Drawing;
using static Interop;

namespace System.Windows.Forms
{
    public partial class DataGridViewTextBoxEditingControl : TextBox, IDataGridViewEditingControl
    {
        private const DataGridViewContentAlignment AnyTop = DataGridViewContentAlignment.TopLeft | DataGridViewContentAlignment.TopCenter | DataGridViewContentAlignment.TopRight;
        private const DataGridViewContentAlignment AnyRight = DataGridViewContentAlignment.TopRight | DataGridViewContentAlignment.MiddleRight | DataGridViewContentAlignment.BottomRight;
        private const DataGridViewContentAlignment AnyCenter = DataGridViewContentAlignment.TopCenter | DataGridViewContentAlignment.MiddleCenter | DataGridViewContentAlignment.BottomCenter;

        private DataGridView _dataGridView;
        private bool _valueChanged;
        private bool _repositionOnValueChange;
        private int _rowIndex;

        public DataGridViewTextBoxEditingControl() : base()
        {
            TabStop = false;
        }

        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new DataGridViewTextBoxEditingControlAccessibleObject(this);
        }

        public virtual DataGridView EditingControlDataGridView
        {
            get
            {
                return _dataGridView;
            }
            set
            {
                _dataGridView = value;
            }
        }

        public virtual object EditingControlFormattedValue
        {
            get
            {
                return GetEditingControlFormattedValue(DataGridViewDataErrorContexts.Formatting);
            }
            set
            {
                Text = (string)value;
            }
        }

        public virtual int EditingControlRowIndex
        {
            get
            {
                return _rowIndex;
            }
            set
            {
                _rowIndex = value;
            }
        }

        public virtual bool EditingControlValueChanged
        {
            get
            {
                return _valueChanged;
            }
            set
            {
                _valueChanged = value;
            }
        }

        public virtual Cursor EditingPanelCursor
        {
            get
            {
                return Cursors.Default;
            }
        }

        public virtual bool RepositionEditingControlOnValueChange
        {
            get
            {
                return _repositionOnValueChange;
            }
        }

        internal override bool SupportsUiaProviders => true;

        public virtual void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
        {
            if (dataGridViewCellStyle is null)
            {
                throw new ArgumentNullException(nameof(dataGridViewCellStyle));
            }

            Font = dataGridViewCellStyle.Font;
            if (dataGridViewCellStyle.BackColor.A < 255)
            {
                // Our TextBox does not support transparent back colors
                Color opaqueBackColor = Color.FromArgb(255, dataGridViewCellStyle.BackColor);
                BackColor = opaqueBackColor;

                if (_dataGridView != null)
                {
                    _dataGridView.EditingPanel.BackColor = opaqueBackColor;
                }
            }
            else
            {
                BackColor = dataGridViewCellStyle.BackColor;
            }
            ForeColor = dataGridViewCellStyle.ForeColor;
            if (dataGridViewCellStyle.WrapMode == DataGridViewTriState.True)
            {
                WordWrap = true;
            }
            TextAlign = TranslateAlignment(dataGridViewCellStyle.Alignment);
            _repositionOnValueChange = (dataGridViewCellStyle.WrapMode == DataGridViewTriState.True && (dataGridViewCellStyle.Alignment & AnyTop) == 0);
        }

        public virtual bool EditingControlWantsInputKey(Keys keyData, bool dataGridViewWantsInputKey)
        {
            switch (keyData & Keys.KeyCode)
            {
                case Keys.Right:
                    // If the end of the selection is at the end of the string
                    // let the DataGridView treat the key message
                    if ((RightToLeft == RightToLeft.No && !(SelectionLength == 0 && SelectionStart == Text.Length)) ||
                        (RightToLeft == RightToLeft.Yes && !(SelectionLength == 0 && SelectionStart == 0)))
                    {
                        return true;
                    }
                    break;

                case Keys.Left:
                    // If the end of the selection is at the beginning of the string
                    // or if the entire text is selected and we did not start editing
                    // send this character to the dataGridView, else process the key event
                    if ((RightToLeft == RightToLeft.No && !(SelectionLength == 0 && SelectionStart == 0)) ||
                        (RightToLeft == RightToLeft.Yes && !(SelectionLength == 0 && SelectionStart == Text.Length)))
                    {
                        return true;
                    }
                    break;

                case Keys.Down:
                    // If the end of the selection is on the last line of the text then
                    // send this character to the dataGridView, else process the key event
                    int end = SelectionStart + SelectionLength;
                    if (Text.IndexOf("\r\n", end) != -1)
                    {
                        return true;
                    }
                    break;

                case Keys.Up:
                    // If the end of the selection is on the first line of the text then
                    // send this character to the dataGridView, else process the key event
                    if (!(Text.IndexOf("\r\n") < 0 || SelectionStart + SelectionLength < Text.IndexOf("\r\n")))
                    {
                        return true;
                    }
                    break;

                case Keys.Home:
                case Keys.End:
                    if (SelectionLength != Text.Length)
                    {
                        return true;
                    }
                    break;

                case Keys.Prior:
                case Keys.Next:
                    if (_valueChanged)
                    {
                        return true;
                    }
                    break;

                case Keys.Delete:
                    if (SelectionLength > 0 ||
                        SelectionStart < Text.Length)
                    {
                        return true;
                    }
                    break;

                case Keys.Enter:
                    if ((keyData & (Keys.Control | Keys.Shift | Keys.Alt)) == Keys.Shift && Multiline && AcceptsReturn)
                    {
                        return true;
                    }
                    break;
            }
            return !dataGridViewWantsInputKey;
        }

        public virtual object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
        {
            return Text;
        }

        public virtual void PrepareEditingControlForEdit(bool selectAll)
        {
            if (selectAll)
            {
                SelectAll();
            }
            else
            {
                // Do not select all the text, but
                // position the caret at the end of the text
                SelectionStart = Text.Length;
            }
        }

        private void NotifyDataGridViewOfValueChange()
        {
            _valueChanged = true;
            _dataGridView?.NotifyCurrentCellDirty(true);
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            AccessibilityObject.RaiseAutomationEvent(UiaCore.UIA.AutomationFocusChangedEventId);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            // Forwarding to grid control. Can't prevent the TextBox from handling the mouse wheel as expected.
            _dataGridView?.OnMouseWheelInternal(e);
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            // Let the DataGridView know about the value change
            NotifyDataGridViewOfValueChange();
        }

        protected override bool ProcessKeyEventArgs(ref Message m)
        {
            switch ((Keys)(int)m.WParam)
            {
                case Keys.Enter:
                    if (m.Msg == (int)User32.WM.CHAR &&
                        !(ModifierKeys == Keys.Shift && Multiline && AcceptsReturn))
                    {
                        // Ignore the Enter key and don't add it to the textbox content. This happens when failing validation brings
                        // up a dialog box for example.
                        // Shift-Enter for multiline textboxes need to be accepted however.
                        return true;
                    }
                    break;

                case Keys.LineFeed:
                    if (m.Msg == (int)User32.WM.CHAR &&
                        ModifierKeys == Keys.Control && Multiline && AcceptsReturn)
                    {
                        // Ignore linefeed character when user hits Ctrl-Enter to commit the cell.
                        return true;
                    }
                    break;

                case Keys.A:
                    if (m.Msg == (int)User32.WM.KEYDOWN && ModifierKeys == Keys.Control)
                    {
                        SelectAll();
                        return true;
                    }
                    break;
            }
            return base.ProcessKeyEventArgs(ref m);
        }

        private static HorizontalAlignment TranslateAlignment(DataGridViewContentAlignment align)
        {
            if ((align & AnyRight) != 0)
            {
                return HorizontalAlignment.Right;
            }
            else if ((align & AnyCenter) != 0)
            {
                return HorizontalAlignment.Center;
            }
            else
            {
                return HorizontalAlignment.Left;
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (IsHandleCreated)
            {
                _dataGridView?.SetAccessibleObjectParent(this.AccessibilityObject);
            }
        }
    }
}
