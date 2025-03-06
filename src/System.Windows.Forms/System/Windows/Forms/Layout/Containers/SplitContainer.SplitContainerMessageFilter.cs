// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public partial class SplitContainer
{
    private class SplitContainerMessageFilter : IMessageFilter
    {
        private readonly SplitContainer _owner;

        public SplitContainerMessageFilter(SplitContainer splitContainer)
        {
            _owner = splitContainer;
        }

        bool IMessageFilter.PreFilterMessage(ref Message m)
        {
            if (m.MsgInternal < PInvokeCore.WM_KEYFIRST || m.MsgInternal > PInvokeCore.WM_KEYLAST)
            {
                return false;
            }

            if ((m.MsgInternal == PInvokeCore.WM_KEYDOWN && (Keys)(nint)m.WParamInternal == Keys.Escape)
                || (m.MsgInternal == PInvokeCore.WM_SYSKEYDOWN))
            {
                // Notify that splitMOVE was reverted. This is used in ONKEYUP.
                _owner._splitBegin = false;
                _owner.SplitEnd(false);
                _owner._splitterClick = false;
                _owner._splitterDrag = false;
            }

            return true;
        }
    }
}
