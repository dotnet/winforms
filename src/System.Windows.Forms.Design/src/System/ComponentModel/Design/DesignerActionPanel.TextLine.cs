// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace System.ComponentModel.Design
{
    internal sealed partial class DesignerActionPanel
    {
        private class TextLine : Line
        {
            private Label _label;
            private DesignerActionTextItem _textItem;

            public TextLine(IServiceProvider serviceProvider, DesignerActionPanel actionPanel)
                : base(serviceProvider, actionPanel)
            {
                actionPanel.FontChanged += new EventHandler(OnParentControlFontChanged);
            }

            public sealed override string FocusId
            {
                get => string.Empty;
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
                controls.Add(_label);
            }

            public sealed override void Focus()
            {
                Debug.Fail("Should never try to focus a TextLine");
            }

            public override Size LayoutControls(int top, int width, bool measureOnly)
            {
                Size labelSize = _label.GetPreferredSize(new Size(int.MaxValue, int.MaxValue));
                if (!measureOnly)
                {
                    _label.Location = new Point(LineLeftMargin, top + LineVerticalPadding / 2);
                    _label.Size = labelSize;
                }

                return labelSize + new Size(LineLeftMargin + LineRightMargin, LineVerticalPadding);
            }

            private void OnParentControlFontChanged(object sender, EventArgs e)
            {
                if (_label is not null && _label.Font is not null)
                {
                    _label.Font = GetFont();
                }
            }

            protected virtual Font GetFont()
            {
                return ActionPanel.Font;
            }

            internal override void UpdateActionItem(DesignerActionList actionList, DesignerActionItem actionItem, ToolTip toolTip, ref int currentTabIndex)
            {
                _textItem = (DesignerActionTextItem)actionItem;
                _label.Text = StripAmpersands(_textItem.DisplayName);
                _label.Font = GetFont();
                _label.TabIndex = currentTabIndex++;
                toolTip.SetToolTip(_label, _textItem.Description);
            }
        }
    }
}
