// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

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

            switch (m.MsgInternal)
            {
                case PInvokeCore.WM_LBUTTONDOWN:
                case PInvokeCore.WM_RBUTTONDOWN:
                case PInvokeCore.WM_MBUTTONDOWN:
                case PInvokeCore.WM_NCLBUTTONDOWN:
                case PInvokeCore.WM_NCRBUTTONDOWN:
                case PInvokeCore.WM_NCMBUTTONDOWN:
                    if (_ownerToolStrip.ContainsFocus)
                    {
                        // If we've clicked on something that's not a child of the toolstrip and we currently have focus, restore it.
                        if (!PInvoke.IsChild(_ownerToolStrip, m.HWND))
                        {
                            HWND rootHwnd = PInvoke.GetAncestor(_ownerToolStrip, GET_ANCESTOR_FLAGS.GA_ROOT);
                            if (rootHwnd == m.HWND || PInvoke.IsChild(rootHwnd, m.HWND))
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
            _ownerToolStrip.BeginInvoke(new BooleanMethodInvoker(_ownerToolStrip.RestoreFocusInternal), [ToolStripManager.ModalMenuFilter.InMenuMode]);
            Application.ThreadContext.FromCurrent().RemoveMessageFilter(this);
        }
    }
}
