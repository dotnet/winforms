// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Windows.Forms;

namespace System.ComponentModel.Design
{
    internal sealed partial class DesignerActionPanel
    {
        private class TextBoxPropertyLine : PropertyLine
        {
            private TextBox _textBox;
            private EditorLabel _readOnlyTextBoxLabel;
            private Control _editControl;
            private Label _label;
            private int _editXPos;
            private bool _textBoxDirty;
            private Point _editRegionLocation;
            private Point _editRegionRelativeLocation;
            private Size _editRegionSize;

            public TextBoxPropertyLine(IServiceProvider serviceProvider, DesignerActionPanel actionPanel)
                : base(serviceProvider, actionPanel)
            {
            }

            protected Control EditControl
            {
                get => _editControl;
            }

            protected Point EditRegionLocation
            {
                get => _editRegionLocation;
            }

            protected Point EditRegionRelativeLocation
            {
                get => _editRegionRelativeLocation;
            }

            protected Size EditRegionSize
            {
                get => _editRegionSize;
            }

            protected override void AddControls(List<Control> controls)
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
                _readOnlyTextBoxLabel.MouseClick += new MouseEventHandler(OnReadOnlyTextBoxLabelClick);
                _readOnlyTextBoxLabel.Enter += new EventHandler(OnReadOnlyTextBoxLabelEnter);
                _readOnlyTextBoxLabel.Leave += new EventHandler(OnReadOnlyTextBoxLabelLeave);
                _readOnlyTextBoxLabel.KeyDown += new KeyEventHandler(OnReadOnlyTextBoxLabelKeyDown);

                _textBox = new TextBox
                {
                    BorderStyle = BorderStyle.None,
                    TextAlign = HorizontalAlignment.Left,
                    Visible = false
                };
                _textBox.TextChanged += new EventHandler(OnTextBoxTextChanged);
                _textBox.KeyDown += new KeyEventHandler(OnTextBoxKeyDown);
                _textBox.LostFocus += new EventHandler(OnTextBoxLostFocus);

                controls.Add(_readOnlyTextBoxLabel);
                controls.Add(_textBox);
                controls.Add(_label);
            }

            public sealed override void Focus()
            {
                _editControl.Focus();
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
                // Figure out our minimum width, Compare to proposed width, If we are smaller, widen the textbox to fit the line based on the bonus
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
                    _editRegionRelativeLocation = new Point(editRegionXPos, TextBoxTopPadding);
                    _editRegionSize = new Size(EditInputWidth + textBoxWidthBonus, textBoxPreferredHeight + TextBoxLineInnerPadding * 2);

                    _label.Location = new Point(LineLeftMargin, top);
                    int labelPreferredWidth = _label.GetPreferredSize(new Size(int.MaxValue, int.MaxValue)).Width;
                    _label.Size = new Size(labelPreferredWidth, height);
                    int specialPadding = 0;
                    if (_editControl is TextBox)
                    {
                        specialPadding = 2;
                    }

                    _editControl.Location = new Point(_editRegionLocation.X + GetTextBoxLeftPadding(textBoxPreferredHeight) + 1 + specialPadding, _editRegionLocation.Y + TextBoxLineInnerPadding + 1);
                    _editControl.Width = _editRegionSize.Width - GetTextBoxRightPadding(textBoxPreferredHeight) - GetTextBoxLeftPadding(textBoxPreferredHeight) - specialPadding;
                    _editControl.Height = _editRegionSize.Height - TextBoxLineInnerPadding * 2 - 1;
                }

                return new Size(width, height);
            }

            protected virtual bool IsReadOnly() => IsReadOnlyProperty(PropertyDescriptor);

            protected override void OnPropertyTaskItemUpdated(ToolTip toolTip, ref int currentTabIndex)
            {
                _label.Text = StripAmpersands(PropertyItem.DisplayName);
                _label.TabIndex = currentTabIndex++;
                toolTip.SetToolTip(_label, PropertyItem.Description);
                _textBoxDirty = false;

                if (IsReadOnly())
                {
                    _readOnlyTextBoxLabel.Visible = true;
                    _textBox.Visible = false;
                    // REVIEW: Setting Visible to false doesn't seem to work, so position far away
                    _textBox.Location = new Point(int.MaxValue, int.MaxValue);
                    _editControl = _readOnlyTextBoxLabel;
                }
                else
                {
                    _readOnlyTextBoxLabel.Visible = false;
                    // REVIEW: Setting Visible to false doesn't seem to work, so position far away
                    _readOnlyTextBoxLabel.Location = new Point(int.MaxValue, int.MaxValue);
                    _textBox.Visible = true;
                    _editControl = _textBox;
                }

                _editControl.AccessibleDescription = PropertyItem.Description;
                _editControl.AccessibleName = StripAmpersands(PropertyItem.DisplayName);
                _editControl.TabIndex = currentTabIndex++;
                _editControl.BringToFront();
            }

            protected virtual void OnReadOnlyTextBoxLabelClick(object sender, MouseEventArgs e)
            {
                if (e.Button == MouseButtons.Left)
                {
                    Focus();
                }
            }

            private void OnReadOnlyTextBoxLabelEnter(object sender, EventArgs e)
            {
                _readOnlyTextBoxLabel.ForeColor = SystemColors.HighlightText;
                _readOnlyTextBoxLabel.BackColor = SystemColors.Highlight;
            }

            private void OnReadOnlyTextBoxLabelLeave(object sender, EventArgs e)
            {
                _readOnlyTextBoxLabel.ForeColor = SystemColors.WindowText;
                _readOnlyTextBoxLabel.BackColor = SystemColors.Window;
            }

            protected TypeConverter.StandardValuesCollection GetStandardValues()
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
                    TypeConverter.StandardValuesCollection standardValues = GetStandardValues();
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
                    TypeConverter.StandardValuesCollection standardValues = GetStandardValues();
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
                            SetValue(standardValues[standardValues.Count - 1]);
                        }
                    }

                    return;
                }
            }

            private void OnReadOnlyTextBoxLabelKeyDown(object sender, KeyEventArgs e)
            {
                // Delegate the rest of the processing to a common helper
                OnEditControlKeyDown(e);
            }

            private void OnTextBoxKeyDown(object sender, KeyEventArgs e)
            {
                if (ActionPanel.DropDownActive)
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

            private void OnTextBoxLostFocus(object sender, EventArgs e)
            {
                if (ActionPanel.DropDownActive)
                {
                    return;
                }

                UpdateValue();
            }

            private void OnTextBoxTextChanged(object sender, EventArgs e) => _textBoxDirty = true;

            protected override void OnValueChanged() => _editControl.Text = PropertyDescriptor.Converter.ConvertToString(TypeDescriptorContext, Value);

            public override void PaintLine(Graphics g, int lineWidth, int lineHeight)
            {
                Rectangle editRect = new Rectangle(EditRegionRelativeLocation, EditRegionSize);
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
                    SetValue(_editControl.Text);
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
                    // (these consts are defined in winuser.h)
                    AccessibilityNotifyClients(AccessibleEvents.Focus, 0, -1);
                }

                protected override bool IsInputKey(Keys keyData)
                {
                    if (keyData == Keys.Down ||
                        keyData == Keys.Up)
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

                    public override string Value
                    {
                        get => Owner.Text;
                    }
                }
            }
        }
    }
}
