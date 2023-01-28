// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms
{
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
                if (m.MsgInternal < User32.WM.KEYFIRST || m.MsgInternal > User32.WM.KEYLAST)
                {
                    return false;
                }

                if (m.MsgInternal == User32.WM.KEYDOWN && (Keys)(nint)m.WParamInternal == Keys.Escape)
                {
                    _owner.SplitEnd(false);
                }

                return true;
            }
        }
    }
}
