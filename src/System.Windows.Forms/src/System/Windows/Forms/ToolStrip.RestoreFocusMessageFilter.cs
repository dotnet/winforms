// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Diagnostics;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    public partial class ToolStrip
    {
        internal sealed class RestoreFocusMessageFilter : IMessageFilter
        {
            private readonly ToolStrip _ownerToolStrip;

            public RestoreFocusMessageFilter(ToolStrip ownerToolStrip)
            {
                _ownerToolStrip = ownerToolStrip;
            }

            public bool PreFilterMessage(ref Message m)
            {
                if (_ownerToolStrip.Disposing || _ownerToolStrip.IsDisposed || _ownerToolStrip.IsDropDown)
                {
                    return false;
                }

                // if the app has changed activation, restore focus

                switch ((User32.WM)m.Msg)
                {
                    case User32.WM.LBUTTONDOWN:
                    case User32.WM.RBUTTONDOWN:
                    case User32.WM.MBUTTONDOWN:
                    case User32.WM.NCLBUTTONDOWN:
                    case User32.WM.NCRBUTTONDOWN:
                    case User32.WM.NCMBUTTONDOWN:
                        if (_ownerToolStrip.ContainsFocus)
                        {
                            // if we've clicked on something that's not a child of the toolstrip and we
                            // currently have focus, restore it.
                            if (!User32.IsChild(new HandleRef(_ownerToolStrip, _ownerToolStrip.Handle), m.HWnd).IsTrue())
                            {
                                IntPtr rootHwnd = User32.GetAncestor(_ownerToolStrip, User32.GA.ROOT);
                                if (rootHwnd == m.HWnd || User32.IsChild(rootHwnd, m.HWnd).IsTrue())
                                {
                                    // Only RestoreFocus if the hwnd is a child of the root window and isn't on the toolstrip.
                                    RestoreFocusInternal();
                                }
                            }
                        }

                        return false;

                    default:
                        return false;
                }
            }

            private void RestoreFocusInternal()
            {
                Debug.WriteLineIf(s_snapFocusDebug.TraceVerbose, "[ToolStrip.RestoreFocusFilter] Detected a click, restoring focus.");

                _ownerToolStrip.BeginInvoke(new BooleanMethodInvoker(_ownerToolStrip.RestoreFocusInternal), new object[] { ToolStripManager.ModalMenuFilter.InMenuMode });

                // PERF,

                Application.ThreadContext.FromCurrent().RemoveMessageFilter(this);
            }
        }
    }
}
