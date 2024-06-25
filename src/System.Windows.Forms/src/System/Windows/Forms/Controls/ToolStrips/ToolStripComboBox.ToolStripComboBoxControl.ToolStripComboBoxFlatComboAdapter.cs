// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Drawing.Drawing2D;

namespace System.Windows.Forms;

public partial class ToolStripComboBox
{
    internal partial class ToolStripComboBoxControl : ComboBox
    {
        internal class ToolStripComboBoxFlatComboAdapter : FlatComboAdapter
        {
            public ToolStripComboBoxFlatComboAdapter(ComboBox comboBox) : base(comboBox, smallButton: true)
            {
            }

            private static bool UseBaseAdapter(ComboBox comboBox)
            {
                if (comboBox is not ToolStripComboBoxControl toolStripComboBox
                    || toolStripComboBox.Owner?.Renderer is not ToolStripProfessionalRenderer)
                {
                    Debug.Assert(comboBox is ToolStripComboBoxControl, "Why are we here and not a toolstrip combo?");
                    return true;
                }

                return false;
            }

            private static ProfessionalColorTable GetColorTable(ToolStripComboBoxControl? toolStripComboBoxControl)
            {
                if (toolStripComboBoxControl is not null)
                {
                    return toolStripComboBoxControl.ColorTable;
                }

                return ProfessionalColors.ColorTable;
            }

            protected override Color GetOuterBorderColor(ComboBox comboBox)
            {
                if (UseBaseAdapter(comboBox))
                {
                    return base.GetOuterBorderColor(comboBox);
                }

                return (comboBox.Enabled) ? Application.ApplicationColors.Window : GetColorTable(comboBox as ToolStripComboBoxControl).ComboBoxBorder;
            }

            protected override Color GetPopupOuterBorderColor(ComboBox comboBox, bool focused)
            {
                if (UseBaseAdapter(comboBox))
                {
                    return base.GetPopupOuterBorderColor(comboBox, focused);
                }

                if (!comboBox.Enabled)
                {
                    return Application.ApplicationColors.ControlDark;
                }

                return focused
                    ? GetColorTable(comboBox as ToolStripComboBoxControl).ComboBoxBorder
                    : Application.ApplicationColors.Window;
            }

            protected override void DrawFlatComboDropDown(ComboBox comboBox, Graphics g, Rectangle dropDownRect)
            {
                if (UseBaseAdapter(comboBox))
                {
                    base.DrawFlatComboDropDown(comboBox, g, dropDownRect);
                    return;
                }

                if (!comboBox.Enabled || !ToolStripManager.VisualStylesEnabled)
                {
                    g.FillRectangle(SystemBrushes.Control, dropDownRect);
                }
                else
                {
                    ToolStripComboBoxControl? toolStripComboBox = comboBox as ToolStripComboBoxControl;
                    ProfessionalColorTable colorTable = GetColorTable(toolStripComboBox);

                    if (!comboBox.DroppedDown)
                    {
                        bool focused = comboBox.ContainsFocus || comboBox.MouseIsOver;
                        if (focused)
                        {
                            using Brush b = new LinearGradientBrush(
                                dropDownRect,
                                colorTable.ComboBoxButtonSelectedGradientBegin,
                                colorTable.ComboBoxButtonSelectedGradientEnd,
                                LinearGradientMode.Vertical);

                            g.FillRectangle(b, dropDownRect);
                        }
                        else if (toolStripComboBox is not null
                            && toolStripComboBox.Owner is not null
                            && toolStripComboBox.Owner.IsOnOverflow)
                        {
                            using var b = colorTable.ComboBoxButtonOnOverflow.GetCachedSolidBrushScope();
                            g.FillRectangle(b, dropDownRect);
                        }
                        else
                        {
                            using Brush b = new LinearGradientBrush(
                                dropDownRect,
                                colorTable.ComboBoxButtonGradientBegin,
                                colorTable.ComboBoxButtonGradientEnd,
                                LinearGradientMode.Vertical);

                            g.FillRectangle(b, dropDownRect);
                        }
                    }
                    else
                    {
                        using Brush b = new LinearGradientBrush(
                            dropDownRect,
                            colorTable.ComboBoxButtonPressedGradientBegin,
                            colorTable.ComboBoxButtonPressedGradientEnd,
                            LinearGradientMode.Vertical);

                        g.FillRectangle(b, dropDownRect);
                    }
                }

                Brush brush;
                if (comboBox.Enabled)
                {
                    brush = SystemInformation.HighContrast
                        && (comboBox.ContainsFocus || comboBox.MouseIsOver)
                        && ToolStripManager.VisualStylesEnabled
                            ? SystemBrushes.HighlightText
                            : SystemBrushes.ControlText;
                }
                else
                {
                    brush = SystemBrushes.GrayText;
                }

                Point middle = new(dropDownRect.Left + dropDownRect.Width / 2, dropDownRect.Top + dropDownRect.Height / 2);

                // If the width is odd - favor pushing it over one pixel right.
                middle.X += (dropDownRect.Width % 2);
                g.FillPolygon(brush, new Point[]
                {
                    new(middle.X - s_offsetPixels, middle.Y - 1),
                    new(middle.X + s_offsetPixels + 1, middle.Y - 1),
                    new(middle.X, middle.Y + s_offsetPixels)
                });
            }
        }
    }
}
