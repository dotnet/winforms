// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms;

public partial class ComboBox
{
    internal class FlatComboAdapter
    {
        private Rectangle _outerBorder;
        private Rectangle _innerBorder;
        private Rectangle _innerInnerBorder;
        internal Rectangle _dropDownRect;
        private Rectangle _whiteFillRect;
        private Rectangle _clientRect;
        private readonly RightToLeft _origRightToLeft; // The combo box's RTL value when we were created

        private const int WhiteFillRectWidth = 5; // used for making the button look smaller than it is

        private const int OFFSET_2PIXELS = 2;
        protected static int s_offsetPixels = OFFSET_2PIXELS;

        public FlatComboAdapter(ComboBox comboBox, bool smallButton)
        {
            // adapter is re-created when combobox is resized, see IsValid method, thus we don't need to handle DPI changed explicitly
            s_offsetPixels = comboBox.LogicalToDeviceUnits(OFFSET_2PIXELS);

            _clientRect = comboBox.ClientRectangle;
            int dropDownButtonWidth = SystemInformation.GetHorizontalScrollBarArrowWidthForDpi(comboBox._deviceDpi);
            _outerBorder = new Rectangle(_clientRect.Location, new Size(_clientRect.Width - 1, _clientRect.Height - 1));
            _innerBorder = new Rectangle(_outerBorder.X + 1, _outerBorder.Y + 1, _outerBorder.Width - dropDownButtonWidth - 2, _outerBorder.Height - 2);
            _innerInnerBorder = new Rectangle(_innerBorder.X + 1, _innerBorder.Y + 1, _innerBorder.Width - 2, _innerBorder.Height - 2);
            _dropDownRect = new Rectangle(_innerBorder.Right + 1, _innerBorder.Y, dropDownButtonWidth, _innerBorder.Height + 1);

            // fill in several pixels of the dropdown rect with white so that it looks like the combo button is thinner.
            if (smallButton)
            {
                _whiteFillRect = _dropDownRect;
                _whiteFillRect.Width = WhiteFillRectWidth;
                _dropDownRect.X += WhiteFillRectWidth;
                _dropDownRect.Width -= WhiteFillRectWidth;
            }

            _origRightToLeft = comboBox.RightToLeft;

            if (_origRightToLeft == RightToLeft.Yes)
            {
                _innerBorder.X = _clientRect.Width - _innerBorder.Right;
                _innerInnerBorder.X = _clientRect.Width - _innerInnerBorder.Right;
                _dropDownRect.X = _clientRect.Width - _dropDownRect.Right;
                _whiteFillRect.X = _clientRect.Width - _whiteFillRect.Right + 1;  // since we're filling, we need to move over to the next px.
            }
        }

        public bool IsValid(ComboBox combo)
        {
            return (combo.ClientRectangle == _clientRect && combo.RightToLeft == _origRightToLeft);
        }

        /// <summary>
        ///  Paints over the edges of the combo box to make it appear flat.
        /// </summary>
        public virtual void DrawFlatCombo(ComboBox comboBox, Graphics g)
        {
            if (comboBox.DropDownStyle == ComboBoxStyle.Simple)
            {
                return;
            }

            Color outerBorderColor = GetOuterBorderColor(comboBox);
            Color innerBorderColor = GetInnerBorderColor(comboBox);
            bool rightToLeft = comboBox.RightToLeft == RightToLeft.Yes;

            // Draw the drop down
            DrawFlatComboDropDown(comboBox, g, _dropDownRect);

            // When we are disabled there is one line of color that seems to eek through if backcolor is set
            // so let's erase it.
            if (!LayoutUtils.IsZeroWidthOrHeight(_whiteFillRect))
            {
                // Fill in two more pixels with white so it looks smaller.
                using var b = innerBorderColor.GetCachedSolidBrushScope();
                g.FillRectangle(b, _whiteFillRect);
            }

            // Draw the outer border

            using var outerBorderPen = outerBorderColor.GetCachedPenScope();
            g.DrawRectangle(outerBorderPen, _outerBorder);
            if (rightToLeft)
            {
                g.DrawRectangle(
                    outerBorderPen,
                    new Rectangle(_outerBorder.X, _outerBorder.Y, _dropDownRect.Width + 1, _outerBorder.Height));
            }
            else
            {
                g.DrawRectangle(
                    outerBorderPen,
                    new Rectangle(_dropDownRect.X, _outerBorder.Y, _outerBorder.Right - _dropDownRect.X, _outerBorder.Height));
            }

            // Draw the inner border
            using var innerBorderPen = innerBorderColor.GetCachedPenScope();
            g.DrawRectangle(innerBorderPen, _innerBorder);
            g.DrawRectangle(innerBorderPen, _innerInnerBorder);

            // Draw a dark border around everything if we're in popup mode
            if ((!comboBox.Enabled) || (comboBox.FlatStyle == FlatStyle.Popup))
            {
                bool focused = comboBox.ContainsFocus || comboBox.MouseIsOver;
                Color borderPenColor = GetPopupOuterBorderColor(comboBox, focused);

                using var borderPen = borderPenColor.GetCachedPenScope();
                Pen innerPen = comboBox.Enabled ? borderPen : SystemPens.Control;

                // Around the dropdown
                if (rightToLeft)
                {
                    g.DrawRectangle(
                        innerPen,
                        new Rectangle(_outerBorder.X, _outerBorder.Y, _dropDownRect.Width + 1, _outerBorder.Height));
                }
                else
                {
                    g.DrawRectangle(
                        innerPen,
                        new Rectangle(_dropDownRect.X, _outerBorder.Y, _outerBorder.Right - _dropDownRect.X, _outerBorder.Height));
                }

                // Around the whole combobox.
                g.DrawRectangle(borderPen, _outerBorder);
            }
        }

        /// <summary>
        ///  Paints over the edges of the combo box to make it appear flat.
        /// </summary>
        protected virtual void DrawFlatComboDropDown(ComboBox comboBox, Graphics g, Rectangle dropDownRect)
        {
            g.FillRectangle(SystemBrushes.Control, dropDownRect);

            Brush brush = (comboBox.Enabled) ? SystemBrushes.ControlText : SystemBrushes.ControlDark;

            Point middle = new(dropDownRect.Left + dropDownRect.Width / 2, dropDownRect.Top + dropDownRect.Height / 2);
            if (_origRightToLeft == RightToLeft.Yes)
            {
                // if the width is odd - favor pushing it over one pixel left.
                middle.X -= (dropDownRect.Width % 2);
            }
            else
            {
                // if the width is odd - favor pushing it over one pixel right.
                middle.X += (dropDownRect.Width % 2);
            }

            g.FillPolygon(
                brush,
                new Point[]
                {
                    new(middle.X - s_offsetPixels, middle.Y - 1),
                    new(middle.X + s_offsetPixels + 1, middle.Y - 1),
                    new(middle.X, middle.Y + s_offsetPixels)
                });
        }

        protected virtual Color GetOuterBorderColor(ComboBox comboBox)
            => comboBox.Enabled ? Application.ApplicationColors.Window : Application.ApplicationColors.ControlDark;

        protected virtual Color GetPopupOuterBorderColor(ComboBox comboBox, bool focused)
        {
            if (!comboBox.Enabled)
            {
                return Application.ApplicationColors.ControlDark;
            }

            return focused ? Application.ApplicationColors.ControlDark : Application.ApplicationColors.Window;
        }

        protected virtual Color GetInnerBorderColor(ComboBox comboBox)
            => comboBox.Enabled ? comboBox.BackColor : Application.ApplicationColors.Control;
    }
}
