// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using static Interop;

namespace System.Windows.Forms
{
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
                if (m.MsgInternal >= User32.WM.KEYFIRST && m.MsgInternal <= User32.WM.KEYLAST)
                {
                    if ((m.MsgInternal == User32.WM.KEYDOWN && (Keys)m.WParamInternal == Keys.Escape)
                        || (m.MsgInternal == User32.WM.SYSKEYDOWN))
                    {
                        // Notify that splitMOVE was reverted. This is used in ONKEYUP.
                        _owner._splitBegin = false;
                        _owner.SplitEnd(false);
                        _owner._splitterClick = false;
                        _owner._splitterDrag = false;
                    }

                    return true;
                }

                return false;
            }
        }
    }
}
