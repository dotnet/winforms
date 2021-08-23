﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using System.Windows.Forms;
using static Interop;

namespace System.ComponentModel.Design
{
    public partial class CollectionEditor
    {
        /// <summary>
        ///  List box filled with ListItem objects representing the collection.
        /// </summary>
        internal class FilterListBox : ListBox
        {
            private const int VK_PROCESSKEY = 0xE5;
            private PropertyGrid _grid;
            private Message _lastKeyDown;

            private PropertyGrid PropertyGrid
            {
                get
                {
                    if (_grid is null)
                    {
                        foreach (Control c in Parent.Controls)
                        {
                            if (c is PropertyGrid)
                            {
                                _grid = (PropertyGrid)c;
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
                switch ((User32.WM)m.Msg)
                {
                    case User32.WM.KEYDOWN:
                        _lastKeyDown = m;

                        // the first thing the ime does on a key it cares about is send a VK_PROCESSKEY, so we use that to sling focus to the grid.
                        if (unchecked((int)(long)m.WParam) == VK_PROCESSKEY)
                        {
                            if (PropertyGrid != null)
                            {
                                PropertyGrid.Focus();
                                User32.SetFocus(new HandleRef(PropertyGrid, PropertyGrid.Handle));
                                Application.DoEvents();
                            }
                            else
                            {
                                break;
                            }

                            if (PropertyGrid.Focused || PropertyGrid.ContainsFocus)
                            {
                                // recreate the keystroke to the newly activated window
                                User32.SendMessageW(User32.GetFocus(), User32.WM.KEYDOWN, _lastKeyDown.WParam, _lastKeyDown.LParam);
                            }
                        }

                        break;

                    case User32.WM.CHAR:

                        if ((Control.ModifierKeys & (Keys.Control | Keys.Alt)) != 0)
                        {
                            break;
                        }

                        if (PropertyGrid != null)
                        {
                            PropertyGrid.Focus();
                            User32.SetFocus(new HandleRef(PropertyGrid, PropertyGrid.Handle));
                            Application.DoEvents();
                        }
                        else
                        {
                            break;
                        }

                        // Make sure we changed focus properly recreate the keystroke to the newly activated window
                        if (PropertyGrid.Focused || PropertyGrid.ContainsFocus)
                        {
                            IntPtr hWnd = User32.GetFocus();
                            User32.SendMessageW(hWnd, User32.WM.KEYDOWN, _lastKeyDown.WParam, _lastKeyDown.LParam);
                            User32.SendMessageW(hWnd, User32.WM.CHAR, m.WParam, m.LParam);
                            return;
                        }

                        break;
                }

                base.WndProc(ref m);
            }
        }
    }
}
