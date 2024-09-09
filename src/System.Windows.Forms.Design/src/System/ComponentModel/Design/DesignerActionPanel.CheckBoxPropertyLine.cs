// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms;

namespace System.ComponentModel.Design;

internal sealed partial class DesignerActionPanel
{
    private sealed class CheckBoxPropertyLine : PropertyLine
    {
        private readonly CheckBox _checkBox;

        private CheckBoxPropertyLine(IServiceProvider serviceProvider, DesignerActionPanel actionPanel)
            : base(serviceProvider, actionPanel)
        {
            _checkBox = new CheckBox
            {
                BackColor = Color.Transparent,
                CheckAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleLeft,
                UseMnemonic = false,
                ForeColor = ActionPanel.LabelForeColor
            };

            _checkBox.CheckedChanged += OnCheckBoxCheckedChanged;

            AddedControls.Add(_checkBox);
        }

        public override void Focus() => _checkBox.Focus();

        public override Size LayoutControls(int top, int width, bool measureOnly)
        {
            Size checkBoxPreferredSize = _checkBox.GetPreferredSize(new Size(int.MaxValue, int.MaxValue));
            if (!measureOnly)
            {
                _checkBox.Location = new Point(LineLeftMargin, top + LineVerticalPadding / 2);
                _checkBox.Size = checkBoxPreferredSize;
            }

            return checkBoxPreferredSize + new Size(LineLeftMargin + LineRightMargin, LineVerticalPadding);
        }

        private void OnCheckBoxCheckedChanged(object? sender, EventArgs e)
        {
            SetValue(_checkBox.Checked);
        }

        protected override void OnPropertyTaskItemUpdated(ToolTip toolTip, ref int currentTabIndex)
        {
            _checkBox.Text = StripAmpersands(PropertyItem!.DisplayName);
            _checkBox.AccessibleDescription = PropertyItem.Description;
            _checkBox.TabIndex = currentTabIndex++;

            toolTip.SetToolTip(_checkBox, PropertyItem.Description);
        }

        protected override void OnValueChanged()
        {
            _checkBox.Checked = (bool)Value!;
        }

        public static StandardLineInfo CreateLineInfo(DesignerActionList list, DesignerActionPropertyItem item) => new Info(list, item);

        private sealed class Info(DesignerActionList list, DesignerActionPropertyItem item) : PropertyLineInfo(list, item)
        {
            public override Line CreateLine(IServiceProvider serviceProvider, DesignerActionPanel actionPanel)
            {
                return new CheckBoxPropertyLine(serviceProvider, actionPanel);
            }

            public override Type LineType => typeof(CheckBoxPropertyLine);
        }
    }
}
