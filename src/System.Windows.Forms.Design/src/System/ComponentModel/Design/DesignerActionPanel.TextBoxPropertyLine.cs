// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms;

namespace System.ComponentModel.Design;

internal sealed partial class DesignerActionPanel
{
    private class TextBoxPropertyLine : PropertyLine
    {
        private readonly TextBox _textBox;
        private readonly EditorLabel _readOnlyTextBoxLabel;
        private readonly Label _label;
        private int _editXPos;
        private bool _textBoxDirty;
        private Point _editRegionLocation;
        private Size _editRegionSize;

        protected TextBoxPropertyLine(IServiceProvider serviceProvider, DesignerActionPanel actionPanel)
            : base(serviceProvider, actionPanel)
        {
            _label = new Label
            {
                BackColor = Color.Transparent,
                ForeColor = ActionPanel.LabelForeColor,
                TextAlign = ContentAlignment.MiddleLeft,
                UseMnemonic = false
            };

            _readOnlyTextBoxLabel = new EditorLabel
            {
                BackColor = Color.Transparent,
                ForeColor = SystemColors.WindowText,
                TabStop = true,
                TextAlign = ContentAlignment.TopLeft,
                UseMnemonic = false,
                Visible = false
            };

            _readOnlyTextBoxLabel.MouseClick += OnReadOnlyTextBoxLabelClick;
            _readOnlyTextBoxLabel.Enter += OnReadOnlyTextBoxLabelEnter;
            _readOnlyTextBoxLabel.Leave += OnReadOnlyTextBoxLabelLeave;
            _readOnlyTextBoxLabel.KeyDown += OnReadOnlyTextBoxLabelKeyDown;

            _textBox = new TextBox
            {
                BorderStyle = BorderStyle.None,
                TextAlign = HorizontalAlignment.Left,
                Visible = false
            };

            _textBox.TextChanged += OnTextBoxTextChanged;
            _textBox.KeyDown += OnTextBoxKeyDown;
            _textBox.LostFocus += OnTextBoxLostFocus;

            AddedControls.Add(_readOnlyTextBoxLabel);
            AddedControls.Add(_textBox);
            AddedControls.Add(_label);
        }

        protected Control? EditControl { get; private set; }

        protected Point EditRegionLocation => _editRegionLocation;

        protected Point EditRegionRelativeLocation { get; private set; }

        protected Size EditRegionSize => _editRegionSize;

        public sealed override void Focus()
        {
            EditControl!.Focus();
        }

        internal int GetEditRegionXPos()
        {
            if (string.IsNullOrEmpty(_label.Text))
            {
                return LineLeftMargin;
            }

            return LineLeftMargin + _label.GetPreferredSize(new Size(int.MaxValue, int.MaxValue)).Width + TextBoxLineCenterMargin;
        }

        protected virtual int GetTextBoxLeftPadding(int textBoxHeight) => TextBoxLineInnerPadding;

        protected virtual int GetTextBoxRightPadding(int textBoxHeight) => TextBoxLineInnerPadding;

        public override Size LayoutControls(int top, int width, bool measureOnly)
        {
            // Figure out our minimum width, Compare to proposed width,
            // If we are smaller, widen the textbox to fit the line based on the bonus
            int textBoxPreferredHeight = _textBox.GetPreferredSize(new Size(int.MaxValue, int.MaxValue)).Height;
            textBoxPreferredHeight += TextBoxHeightFixup;
            int height = textBoxPreferredHeight + LineVerticalPadding + TextBoxLineInnerPadding * 2 + 2; // 2 == border size

            int editRegionXPos = Math.Max(_editXPos, GetEditRegionXPos());
            int minimumWidth = editRegionXPos + EditInputWidth + LineRightMargin;
            width = Math.Max(width, minimumWidth);
            int textBoxWidthBonus = width - minimumWidth;

            if (!measureOnly)
            {
                _editRegionLocation = new Point(editRegionXPos, top + TextBoxTopPadding);
                EditRegionRelativeLocation = new Point(editRegionXPos, TextBoxTopPadding);
                _editRegionSize = new Size(EditInputWidth + textBoxWidthBonus, textBoxPreferredHeight + TextBoxLineInnerPadding * 2);

                _label.Location = new Point(LineLeftMargin, top);
                int labelPreferredWidth = _label.GetPreferredSize(new Size(int.MaxValue, int.MaxValue)).Width;
                _label.Size = new Size(labelPreferredWidth, height);
                int specialPadding = EditControl is TextBox ? 2 : 0;

                EditControl!.Location = new Point(_editRegionLocation.X + GetTextBoxLeftPadding(textBoxPreferredHeight) + 1 + specialPadding, _editRegionLocation.Y + TextBoxLineInnerPadding + 1);
                EditControl.Width = _editRegionSize.Width - GetTextBoxRightPadding(textBoxPreferredHeight) - GetTextBoxLeftPadding(textBoxPreferredHeight) - specialPadding;
                EditControl.Height = _editRegionSize.Height - TextBoxLineInnerPadding * 2 - 1;
            }

            return new Size(width, height);
        }

        protected virtual bool IsReadOnly() => IsReadOnlyProperty(PropertyDescriptor);

        [MemberNotNull(nameof(EditControl))]
        protected override void OnPropertyTaskItemUpdated(ToolTip toolTip, ref int currentTabIndex)
        {
            _label.Text = StripAmpersands(PropertyItem!.DisplayName);
            _label.TabIndex = currentTabIndex++;
            toolTip.SetToolTip(_label, PropertyItem.Description);
            _textBoxDirty = false;

            if (IsReadOnly())
            {
                _readOnlyTextBoxLabel.Visible = true;
                _textBox.Visible = false;
                // REVIEW: Setting Visible to false doesn't seem to work, so position far away
                _textBox.Location = new Point(int.MaxValue, int.MaxValue);
                EditControl = _readOnlyTextBoxLabel;
            }
            else
            {
                _readOnlyTextBoxLabel.Visible = false;
                // REVIEW: Setting Visible to false doesn't seem to work, so position far away
                _readOnlyTextBoxLabel.Location = new Point(int.MaxValue, int.MaxValue);
                _textBox.Visible = true;
                EditControl = _textBox;
            }

            EditControl.AccessibleDescription = PropertyItem.Description;
            EditControl.AccessibleName = StripAmpersands(PropertyItem.DisplayName);
            EditControl.TabIndex = currentTabIndex++;
            EditControl.BringToFront();
        }

        protected virtual void OnReadOnlyTextBoxLabelClick(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Focus();
            }
        }

        private void OnReadOnlyTextBoxLabelEnter(object? sender, EventArgs e)
        {
            _readOnlyTextBoxLabel.ForeColor = SystemColors.HighlightText;
            _readOnlyTextBoxLabel.BackColor = SystemColors.Highlight;
        }

        private void OnReadOnlyTextBoxLabelLeave(object? sender, EventArgs e)
        {
            _readOnlyTextBoxLabel.ForeColor = SystemColors.WindowText;
            _readOnlyTextBoxLabel.BackColor = SystemColors.Window;
        }

        protected TypeConverter.StandardValuesCollection? GetStandardValues()
        {
            TypeConverter converter = PropertyDescriptor.Converter;
            if (converter is not null &&
                converter.GetStandardValuesSupported(TypeDescriptorContext))
            {
                return converter.GetStandardValues(TypeDescriptorContext);
            }

            return null;
        }

        private void OnEditControlKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
            {
                e.Handled = true;
                // Try to find the existing value and then pick the one after it
                TypeConverter.StandardValuesCollection? standardValues = GetStandardValues();
                if (standardValues is not null)
                {
                    for (int i = 0; i < standardValues.Count; i++)
                    {
                        if (Equals(Value, standardValues[i]))
                        {
                            if (i < standardValues.Count - 1)
                            {
                                SetValue(standardValues[i + 1]);
                            }

                            return;
                        }
                    }

                    // Previous value wasn't found, select the first one by default
                    if (standardValues.Count > 0)
                    {
                        SetValue(standardValues[0]);
                    }
                }

                return;
            }

            if (e.KeyCode == Keys.Up)
            {
                e.Handled = true;
                // Try to find the existing value and then pick the one before it
                TypeConverter.StandardValuesCollection? standardValues = GetStandardValues();
                if (standardValues is not null)
                {
                    for (int i = 0; i < standardValues.Count; i++)
                    {
                        if (Equals(Value, standardValues[i]))
                        {
                            if (i > 0)
                            {
                                SetValue(standardValues[i - 1]);
                            }

                            return;
                        }
                    }

                    // Previous value wasn't found, select the first one by default
                    if (standardValues.Count > 0)
                    {
                        SetValue(standardValues[^1]);
                    }
                }

                return;
            }
        }

        private void OnReadOnlyTextBoxLabelKeyDown(object? sender, KeyEventArgs e)
        {
            // Delegate the rest of the processing to a common helper
            OnEditControlKeyDown(e);
        }

        private void OnTextBoxKeyDown(object? sender, KeyEventArgs e)
        {
            if (ActionPanel._dropDownActive)
            {
                return;
            }

            if (e.KeyCode == Keys.Enter)
            {
                UpdateValue();
                e.Handled = true;
                return;
            }

            // Delegate the rest of the processing to a common helper
            OnEditControlKeyDown(e);
        }

        private void OnTextBoxLostFocus(object? sender, EventArgs e)
        {
            if (ActionPanel._dropDownActive)
            {
                return;
            }

            UpdateValue();
        }

        private void OnTextBoxTextChanged(object? sender, EventArgs e) => _textBoxDirty = true;

        protected override void OnValueChanged() => EditControl!.Text = PropertyDescriptor.Converter.ConvertToString(TypeDescriptorContext, Value);

        public override void PaintLine(Graphics g, int lineWidth, int lineHeight)
        {
            Rectangle editRect = new(EditRegionRelativeLocation, EditRegionSize);
            g.FillRectangle(SystemBrushes.Window, editRect);
            g.DrawRectangle(SystemPens.ControlDark, editRect);
        }

        internal void SetEditRegionXPos(int xPos)
        {
            // Ignore the x-position if we have no text. This allows the textbox to span the entire width of the panel.
            if (!string.IsNullOrEmpty(_label.Text))
            {
                _editXPos = xPos;
            }
            else
            {
                _editXPos = LineLeftMargin;
            }
        }

        private void UpdateValue()
        {
            if (_textBoxDirty)
            {
                SetValue(EditControl!.Text);
                _textBoxDirty = false;
            }
        }

        /// <summary>
        ///  Custom label that provides accurate accessibility information and focus abilities.
        /// </summary>
        private sealed class EditorLabel : Label
        {
            public EditorLabel()
            {
                SetStyle(ControlStyles.Selectable, true);
            }

            protected override AccessibleObject CreateAccessibilityInstance() => new EditorLabelAccessibleObject(this);

            protected override void OnGotFocus(EventArgs e)
            {
                base.OnGotFocus(e);

                // Since we are not a standard focusable control, we have to raise our own accessibility events.
                // objectID = OBJID_WINDOW, childID = CHILDID_SELF - 1 (the -1 is because WinForms always adds 1 to the value)
                // (these constants are defined in winuser.h)
                AccessibilityNotifyClients(AccessibleEvents.Focus, 0, -1);
            }

            protected override bool IsInputKey(Keys keyData)
            {
                if (keyData is Keys.Down or Keys.Up)
                {
                    return true;
                }

                return base.IsInputKey(keyData);
            }

            private sealed class EditorLabelAccessibleObject : ControlAccessibleObject
            {
                public EditorLabelAccessibleObject(EditorLabel owner)
                    : base(owner)
                {
                }

                public override string? Value => Owner?.Text;
            }
        }

        public static StandardLineInfo CreateLineInfo(DesignerActionList list, DesignerActionPropertyItem item) => new Info(list, item);

        private sealed class Info(DesignerActionList list, DesignerActionPropertyItem item) : PropertyLineInfo(list, item)
        {
            public override Line CreateLine(IServiceProvider serviceProvider, DesignerActionPanel actionPanel)
            {
                return new TextBoxPropertyLine(serviceProvider, actionPanel);
            }

            public override Type LineType => typeof(TextBoxPropertyLine);
        }
    }
}
