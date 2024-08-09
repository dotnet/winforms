// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms;

namespace System.ComponentModel.Design;

internal sealed partial class DesignerActionPanel
{
    private sealed class PanelHeaderLine : Line
    {
        private DesignerActionPanelHeaderItem? _panelHeaderItem;
        private readonly Label _titleLabel;
        private readonly Label _subtitleLabel;
        private bool _formActive;

        private PanelHeaderLine(IServiceProvider serviceProvider, DesignerActionPanel actionPanel)
            : base(serviceProvider, actionPanel)
        {
            actionPanel.FontChanged += OnParentControlFontChanged;

            _titleLabel = new Label
            {
                BackColor = Color.Transparent,
                ForeColor = ActionPanel.TitleBarTextColor,
                TextAlign = ContentAlignment.MiddleLeft,
                UseMnemonic = false
            };

            _subtitleLabel = new Label
            {
                BackColor = Color.Transparent,
                ForeColor = ActionPanel.TitleBarTextColor,
                TextAlign = ContentAlignment.MiddleLeft,
                UseMnemonic = false
            };

            AddedControls.Add(_titleLabel);
            AddedControls.Add(_subtitleLabel);

            // TODO: Need to figure out how to unhook these events. Perhaps have Initialize() and Cleanup() methods.
            ActionPanel.FormActivated += OnFormActivated;
            ActionPanel.FormDeactivate += OnFormDeactivate;
        }

        public override string FocusId => string.Empty;

        public override void Focus()
        {
            Debug.Fail("Should never try to focus a PanelHeaderLine");
        }

        public override Size LayoutControls(int top, int width, bool measureOnly)
        {
            Size titleSize = _titleLabel.GetPreferredSize(new Size(int.MaxValue, int.MaxValue));
            Size subtitleSize = Size.Empty;
            if (!string.IsNullOrEmpty(_panelHeaderItem!.Subtitle))
            {
                subtitleSize = _subtitleLabel.GetPreferredSize(new Size(int.MaxValue, int.MaxValue));
            }

            if (!measureOnly)
            {
                _titleLabel.Location = new Point(LineLeftMargin, top + PanelHeaderVerticalPadding);
                _titleLabel.Size = titleSize;

                _subtitleLabel.Location = new Point(LineLeftMargin, top + PanelHeaderVerticalPadding * 2 + titleSize.Height);
                _subtitleLabel.Size = subtitleSize;
            }

            int newWidth = Math.Max(titleSize.Width, subtitleSize.Width) + 2 * PanelHeaderHorizontalPadding;
            int newHeight = (subtitleSize.IsEmpty ? (titleSize.Height + 2 * PanelHeaderVerticalPadding) : (titleSize.Height + subtitleSize.Height + 3 * PanelHeaderVerticalPadding));
            return new Size(newWidth + 2, newHeight + 1);
        }

        private void OnFormActivated(object? sender, EventArgs e)
        {
            // TODO: Figure out better rect
            _formActive = true;
            ActionPanel.Invalidate();
            // ActionPanel.Invalidate(new Rectangle(EditRegionLocation, EditRegionSize), false);
        }

        private void OnFormDeactivate(object? sender, EventArgs e)
        {
            // TODO: Figure out better rect
            _formActive = false;
            ActionPanel.Invalidate();
        }

        private void OnParentControlFontChanged(object? sender, EventArgs e)
        {
            _titleLabel.Font = new Font(ActionPanel.Font, FontStyle.Bold);
            _subtitleLabel.Font = ActionPanel.Font;
        }

        public override void PaintLine(Graphics g, int lineWidth, int lineHeight)
        {
            Color backColor = (_formActive || ActionPanel._dropDownActive) ? ActionPanel.TitleBarColor : ActionPanel.TitleBarUnselectedColor;
            using SolidBrush b = new(backColor);
            g.FillRectangle(b, 1, 1, lineWidth - 2, lineHeight - 1);

            // Paint a line under the title label
            using Pen p = new(ActionPanel.BorderColor);
            g.DrawLine(p, 0, lineHeight - 1, lineWidth, lineHeight - 1);
        }

        internal override void UpdateActionItem(LineInfo lineInfo, ToolTip toolTip, ref int currentTabIndex)
        {
            Info info = (Info)lineInfo;
            _panelHeaderItem = info.Item;

            _titleLabel.Text = _panelHeaderItem.DisplayName;
            _titleLabel.TabIndex = currentTabIndex++;
            _subtitleLabel.Text = _panelHeaderItem.Subtitle;
            _subtitleLabel.TabIndex = currentTabIndex++;
            _subtitleLabel.Visible = (_subtitleLabel.Text.Length != 0);

            // Force the font to update
            OnParentControlFontChanged(null, EventArgs.Empty);
        }

        public sealed class Info(DesignerActionPanelHeaderItem item) : LineInfo
        {
            public override DesignerActionPanelHeaderItem Item { get; } = item;
            public override Line CreateLine(IServiceProvider serviceProvider, DesignerActionPanel actionPanel)
            {
                return new PanelHeaderLine(serviceProvider, actionPanel);
            }

            public override Type LineType => typeof(PanelHeaderLine);
        }
    }
}
