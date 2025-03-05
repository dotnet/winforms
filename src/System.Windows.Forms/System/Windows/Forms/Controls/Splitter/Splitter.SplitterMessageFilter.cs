// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public partial class Splitter
{
    private class SplitterMessageFilter : IMessageFilter
    {
        private readonly Splitter _owner;

        public SplitterMessageFilter(Splitter splitter)
        {
            _owner = splitter;
        }

        /// <summary>
        /// </summary>
        public bool PreFilterMessage(ref Message m)
        {
            if (m.MsgInternal < PInvokeCore.WM_KEYFIRST || m.MsgInternal > PInvokeCore.WM_KEYLAST)
            {
                return false;
            }

            if (m.MsgInternal == PInvokeCore.WM_KEYDOWN && (Keys)(nint)m.WParamInternal == Keys.Escape)
            {
                _owner.SplitEnd(false);
            }

            return true;
        }
    }
}
