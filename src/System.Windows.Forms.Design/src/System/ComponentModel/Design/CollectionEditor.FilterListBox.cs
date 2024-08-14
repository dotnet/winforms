// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms;

namespace System.ComponentModel.Design;

public partial class CollectionEditor
{
    /// <summary>
    ///  List box filled with ListItem objects representing the collection.
    /// </summary>
    internal class FilterListBox : ListBox
    {
        private const int VK_PROCESSKEY = 0xE5;
        private PropertyGrid? _grid;
        private Message _lastKeyDown;

        private PropertyGrid? PropertyGrid
        {
            get
            {
                if (_grid is null && Parent is not null)
                {
                    foreach (Control c in Parent.Controls)
                    {
                        if (c is PropertyGrid grid)
                        {
                            _grid = grid;
                            break;
                        }
                    }
                }

                return _grid;
            }
        }

        /// <summary>
        ///  Expose the protected RefreshItem() method so that CollectionEditor can use it
        /// </summary>
        public new void RefreshItem(int index) => base.RefreshItem(index);

        protected override void WndProc(ref Message m)
        {
            switch (m.MsgInternal)
            {
                case PInvoke.WM_KEYDOWN:
                    _lastKeyDown = m;

                    // The first thing the ime does on a key it cares about is send a VK_PROCESSKEY, so we use
                    // that to sling focus to the grid.
                    if (m.WParamInternal == VK_PROCESSKEY)
                    {
                        if (PropertyGrid is not null)
                        {
                            PropertyGrid.Focus();
                            PInvoke.SetFocus(PropertyGrid);
                            Application.DoEvents();
                        }
                        else
                        {
                            break;
                        }

                        if (PropertyGrid.Focused || PropertyGrid.ContainsFocus)
                        {
                            // Recreate the keystroke to the newly activated window.
                            PInvoke.SendMessage(PInvoke.GetFocus(), PInvoke.WM_KEYDOWN, _lastKeyDown.WParamInternal, _lastKeyDown.LParamInternal);
                        }
                    }

                    break;

                case PInvoke.WM_CHAR:

                    if ((ModifierKeys & (Keys.Control | Keys.Alt)) != 0)
                    {
                        break;
                    }

                    if (PropertyGrid is not null)
                    {
                        PropertyGrid.Focus();
                        PInvoke.SetFocus(PropertyGrid);
                        Application.DoEvents();
                    }
                    else
                    {
                        break;
                    }

                    // Make sure we changed focus properly recreate the keystroke to the newly activated window
                    if (PropertyGrid.Focused || PropertyGrid.ContainsFocus)
                    {
                        HWND hwnd = PInvoke.GetFocus();
                        PInvoke.SendMessage(hwnd, PInvoke.WM_KEYDOWN, _lastKeyDown.WParamInternal, _lastKeyDown.LParamInternal);
                        PInvoke.SendMessage(hwnd, PInvoke.WM_CHAR, m.WParamInternal, m.LParamInternal);
                        return;
                    }

                    break;

                case PInvoke.WM_DRAWITEM:
                    ListBox_drawItem(this, new DrawItemEventArgs(m.WParamInternal, m.LParamInternal));
                    break;
            }

            base.WndProc(ref m);
        }

        private void ListBox_drawItem(object? sender, DrawItemEventArgs e)
        {
            if (e.Index != -1)
            {
                ListItem item = (ListItem)Items[e.Index];

                Graphics g = e.Graphics;

                int c = Items.Count;
                int maxC = (c > 1) ? c - 1 : c;
                // We add the +4 is a fudge factor...
                SizeF sizeW = g.MeasureString(maxC.ToString(CultureInfo.CurrentCulture), Font);

                int charactersInNumber = ((int)(Math.Log(maxC) / s_log10) + 1); // Luckily, this is never called if count = 0
                int w = 4 + charactersInNumber * (Font.Height / 2);

                w = Math.Max(w, (int)Math.Ceiling(sizeW.Width));
                w += SystemInformation.BorderSize.Width * 4;

                Rectangle button = e.Bounds with { Width = w };

                ControlPaint.DrawButton(g, button, ButtonState.Normal);
                button.Inflate(-SystemInformation.BorderSize.Width * 2, -SystemInformation.BorderSize.Height * 2);

                int offset = w;

                Color backColor = SystemColors.Window;
                Color textColor = SystemColors.WindowText;
                if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                {
                    backColor = SystemColors.Highlight;
                    textColor = SystemColors.HighlightText;
                }

                Rectangle res = e.Bounds with { X = e.Bounds.X + offset, Width = e.Bounds.Width - offset };
                g.FillRectangle(new SolidBrush(backColor), res);
                if ((e.State & DrawItemState.Focus) == DrawItemState.Focus)
                {
                    ControlPaint.DrawFocusRectangle(g, res);
                }

                offset += 2;

                if (item.Editor is not null && item.Editor.GetPaintValueSupported())
                {
                    Rectangle baseVar = new(e.Bounds.X + offset, e.Bounds.Y + 1, PaintWidth, e.Bounds.Height - 3);
                    g.DrawRectangle(SystemPens.ControlText, baseVar.X, baseVar.Y, baseVar.Width - 1, baseVar.Height - 1);
                    baseVar.Inflate(-1, -1);
                    item.Editor.PaintValue(item.Value, g, baseVar);
                    offset += PaintIndent + TextIndent;
                }

                using (StringFormat format = new())
                {
                    format.Alignment = StringAlignment.Center;
                    g.DrawString(e.Index.ToString(CultureInfo.CurrentCulture), Font, SystemBrushes.ControlText,
                        e.Bounds with { Width = w }, format);
                }

                string itemText = GetDisplayText(item);

                using (Brush textBrush = new SolidBrush(textColor))
                {
                    g.DrawString(itemText, Font, textBrush,
                        e.Bounds with { X = e.Bounds.X + offset, Width = e.Bounds.Width - offset });
                }

                // Check to see if we need to change the horizontal extent of the listBox
                int width = offset + (int)g.MeasureString(itemText, Font).Width;
                if (width > e.Bounds.Width && HorizontalExtent < width)
                {
                    HorizontalExtent = width;
                }
            }
        }
    }
}
