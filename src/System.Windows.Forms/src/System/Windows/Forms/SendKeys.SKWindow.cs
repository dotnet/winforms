// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms
{
    public partial class SendKeys
    {
        /// <summary>
        ///  SendKeys creates a window to monitor WM_CANCELJOURNAL messages.
        /// </summary>
        private class SKWindow : Control
        {
            public SKWindow()
            {
                SetState(States.TopLevel, true);
                SetExtendedState(ExtendedStates.InterestedInUserPreferenceChanged, false);
                SetBounds(-1, -1, 0, 0);
                Visible = false;
            }

            protected override void WndProc(ref Message m)
            {
                if (m.Msg == (int)User32.WM.CANCELJOURNAL)
                {
                    try
                    {
                        JournalCancel();
                    }
                    catch
                    {
                    }
                }
            }
        }
    }
}
