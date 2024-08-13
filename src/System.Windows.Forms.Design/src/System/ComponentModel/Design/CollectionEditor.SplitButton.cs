// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace System.ComponentModel.Design;

public partial class CollectionEditor
{
    internal class SplitButton : Button
    {
        private PushButtonState _state;
        private const int PushButtonWidth = 14;
        private Rectangle _dropDownRectangle;
        private bool _showSplit;

        private static bool s_isScalingInitialized;
        private const int Offset2Pixels = 2;
        private static int s_offset2X = Offset2Pixels;
        private static int s_offset2Y = Offset2Pixels;

        public SplitButton() : base()
        {
            if (!s_isScalingInitialized)
            {
                s_offset2X = ScaleHelper.ScaleToInitialSystemDpi(Offset2Pixels);
                s_offset2Y = ScaleHelper.ScaleToInitialSystemDpi(Offset2Pixels);
                s_isScalingInitialized = true;
            }
        }

        public bool ShowSplit
        {
            set
            {
                if (value != _showSplit)
                {
                    _showSplit = value;
                    Invalidate();
                }
            }
        }

        private PushButtonState State
        {
            get => _state;
            set
            {
                if (!_state.Equals(value))
                {
                    _state = value;
                    Invalidate();
                }
            }
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            Size preferredSize = base.GetPreferredSize(proposedSize);
            if (_showSplit && !string.IsNullOrEmpty(Text) && TextRenderer.MeasureText(Text, Font).Width + PushButtonWidth > preferredSize.Width)
            {
                return preferredSize + new Size(PushButtonWidth, 0);
            }

            return preferredSize;
        }

        protected override bool IsInputKey(Keys keyData)
        {
            if (keyData is Keys.Down && _showSplit)
            {
                return true;
            }

            return base.IsInputKey(keyData);
        }

        protected override void OnGotFocus(EventArgs e)
        {
            if (!_showSplit)
            {
                base.OnGotFocus(e);
                return;
            }

            if (State is not (PushButtonState.Pressed or PushButtonState.Disabled))
            {
                State = PushButtonState.Default;
            }
        }

        protected override void OnKeyDown(KeyEventArgs kevent)
        {
            if (kevent.KeyCode is Keys.Down && _showSplit)
            {
                ShowContextMenuStrip();
            }
            else
            {
                // We need to pass the unhandled characters (including Keys.Space) on
                // to base.OnKeyDown when it's not to drop the split menu
                base.OnKeyDown(kevent);
            }
        }

        protected override void OnLostFocus(EventArgs e)
        {
            if (!_showSplit)
            {
                base.OnLostFocus(e);
                return;
            }

            if (State is not (PushButtonState.Pressed or PushButtonState.Disabled))
            {
                State = PushButtonState.Normal;
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (!_showSplit)
            {
                base.OnMouseDown(e);
                return;
            }

            if (_dropDownRectangle.Contains(e.Location))
            {
                ShowContextMenuStrip();
            }
            else
            {
                State = PushButtonState.Pressed;
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            if (!_showSplit)
            {
                base.OnMouseEnter(e);
                return;
            }

            if (State is not (PushButtonState.Pressed or PushButtonState.Disabled))
            {
                State = PushButtonState.Hot;
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            if (!_showSplit)
            {
                base.OnMouseLeave(e);
                return;
            }

            if (State is not (PushButtonState.Pressed or PushButtonState.Disabled))
            {
                State = Focused ? PushButtonState.Default : PushButtonState.Normal;
            }
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            if (!_showSplit)
            {
                base.OnMouseUp(mevent);
                return;
            }

            if (ContextMenuStrip is not { Visible: true })
            {
                SetButtonDrawState();
                if (Parent is not null && Bounds.Contains(Parent.PointToClient(Cursor.Position)) && !_dropDownRectangle.Contains(mevent.Location))
                {
                    OnClick(EventArgs.Empty);
                }
            }
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);

            if (!_showSplit)
            {
                return;
            }

            Graphics g = pevent.Graphics;
            Rectangle bounds = new(0, 0, Width, Height);
            TextFormatFlags formatFlags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter;

            ButtonRenderer.DrawButton(g, bounds, State);

            _dropDownRectangle = new Rectangle(bounds.Right - PushButtonWidth - 1, 4, PushButtonWidth, bounds.Height - 8);

            if (RightToLeft == RightToLeft.Yes)
            {
                _dropDownRectangle.X = bounds.Left + 1;

                g.DrawLine(SystemPens.ButtonHighlight, bounds.Left + PushButtonWidth, 4, bounds.Left + PushButtonWidth, bounds.Bottom - 4);
                g.DrawLine(SystemPens.ButtonHighlight, bounds.Left + PushButtonWidth + 1, 4, bounds.Left + PushButtonWidth + 1, bounds.Bottom - 4);
                bounds.Offset(PushButtonWidth, 0);
                bounds.Width -= PushButtonWidth;
            }
            else
            {
                g.DrawLine(SystemPens.ButtonHighlight, bounds.Right - PushButtonWidth, 4, bounds.Right - PushButtonWidth, bounds.Bottom - 4);
                g.DrawLine(SystemPens.ButtonHighlight, bounds.Right - PushButtonWidth - 1, 4, bounds.Right - PushButtonWidth - 1, bounds.Bottom - 4);
                bounds.Width -= PushButtonWidth;
            }

            PaintArrow(g, _dropDownRectangle);

            // If we don't use mnemonic, set formatFlag to NoPrefix as this will show ampersand.
            if (!UseMnemonic)
            {
                formatFlags |= TextFormatFlags.NoPrefix;
            }
            else if (!ShowKeyboardCues)
            {
                formatFlags |= TextFormatFlags.HidePrefix;
            }

            if (!string.IsNullOrEmpty(Text))
            {
                TextRenderer.DrawText(pevent, Text, Font, bounds, SystemColors.ControlText, formatFlags);
            }

            if (Focused)
            {
                bounds.Inflate(-4, -4);
            }
        }

        private static void PaintArrow(IDeviceContext deviceContext, Rectangle dropDownRect)
        {
            Point middle = new(
                Convert.ToInt32(dropDownRect.Left + dropDownRect.Width / 2),
                Convert.ToInt32(dropDownRect.Top + dropDownRect.Height / 2));

            // If the width is odd - favor pushing it over one pixel right.
            middle.X += (dropDownRect.Width % 2);

            Point[] arrow = [
                new(middle.X - s_offset2X, middle.Y - 1),
                new(middle.X + s_offset2X + 1, middle.Y - 1),
                new(middle.X, middle.Y + s_offset2Y)
            ];

            deviceContext.TryGetGraphics(create: true)?.FillPolygon(SystemBrushes.ControlText, arrow);
        }

        private void ShowContextMenuStrip()
        {
            State = PushButtonState.Pressed;
            if (ContextMenuStrip is not null)
            {
                ContextMenuStrip.Closed += ContextMenuStrip_Closed;
                ContextMenuStrip.Show(this, 0, Height);
            }
        }

        private void ContextMenuStrip_Closed(object? sender, ToolStripDropDownClosedEventArgs e)
        {
            if (sender is ContextMenuStrip cms)
            {
                cms.Closed -= ContextMenuStrip_Closed;
            }

            SetButtonDrawState();
        }

        private void SetButtonDrawState()
        {
            if (Parent is not null && Bounds.Contains(Parent.PointToClient(Cursor.Position)))
            {
                State = PushButtonState.Hot;
            }
            else if (Focused)
            {
                State = PushButtonState.Default;
            }
            else
            {
                State = PushButtonState.Normal;
            }
        }
    }
}
