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
            private readonly Splitter owner;

            public SplitterMessageFilter(Splitter splitter)
            {
                owner = splitter;
            }

            /// <summary>
            /// </summary>
            public bool PreFilterMessage(ref Message m)
            {
                if (m.MsgInternal >= User32.WM.KEYFIRST && m.MsgInternal <= User32.WM.KEYLAST)
                {
                    if (m.MsgInternal == User32.WM.KEYDOWN && (Keys)m.WParamInternal == Keys.Escape)
                    {
                        owner.SplitEnd(false);
                    }

                    return true;
                }

                return false;
            }
        }
    }
}
