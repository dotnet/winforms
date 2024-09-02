// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms;

namespace System.ComponentModel.Design;

internal sealed partial class DesignerActionPanel
{
    private class TextLine : Line
    {
        private readonly Label _label;
        private DesignerActionTextItem? _textItem;

        protected TextLine(IServiceProvider serviceProvider, DesignerActionPanel actionPanel)
            : base(serviceProvider, actionPanel)
        {
            actionPanel.FontChanged += OnParentControlFontChanged;
            _label = new Label
            {
                BackColor = Color.Transparent,
                ForeColor = ActionPanel.LabelForeColor,
                TextAlign = ContentAlignment.MiddleLeft,
                UseMnemonic = false,
            };

            AddedControls.Add(_label);
        }

        public sealed override string FocusId => string.Empty;

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

        private void OnParentControlFontChanged(object? sender, EventArgs e)
        {
            if (_label.Font is not null)
            {
                _label.Font = GetFont();
            }
        }

        protected virtual Font GetFont()
        {
            return ActionPanel.Font;
        }

        internal override void UpdateActionItem(LineInfo lineInfo, ToolTip toolTip, ref int currentTabIndex)
        {
            TextLineInfo textLineInfo = (TextLineInfo)lineInfo;
            _textItem = textLineInfo.Item;
            _label.Text = StripAmpersands(_textItem.DisplayName);
            _label.Font = GetFont();
            _label.TabIndex = currentTabIndex++;
            toolTip.SetToolTip(_label, _textItem.Description);
        }

        public static StandardLineInfo CreateLineInfo(DesignerActionList list, DesignerActionTextItem item) => new TextLineInfo(list, item);

        protected class TextLineInfo(DesignerActionList list, DesignerActionTextItem item) : StandardLineInfo(list)
        {
            public override DesignerActionTextItem Item { get; } = item;
            public override Line CreateLine(IServiceProvider serviceProvider, DesignerActionPanel actionPanel)
            {
                return new TextLine(serviceProvider, actionPanel);
            }

            public override Type LineType => typeof(TextLine);
        }
    }
}
