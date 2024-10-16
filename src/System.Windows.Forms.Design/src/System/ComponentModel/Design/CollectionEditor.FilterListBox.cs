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
                case PInvokeCore.WM_KEYDOWN:
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
                            PInvokeCore.SendMessage(PInvoke.GetFocus(), PInvokeCore.WM_KEYDOWN, _lastKeyDown.WParamInternal, _lastKeyDown.LParamInternal);
                        }
                    }

                    break;

                case PInvokeCore.WM_CHAR:

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
                        PInvokeCore.SendMessage(hwnd, PInvokeCore.WM_KEYDOWN, _lastKeyDown.WParamInternal, _lastKeyDown.LParamInternal);
                        PInvokeCore.SendMessage(hwnd, PInvokeCore.WM_CHAR, m.WParamInternal, m.LParamInternal);
                        return;
                    }

                    break;
            }

            base.WndProc(ref m);
        }
    }
}
