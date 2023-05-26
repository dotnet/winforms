// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.Win32.UI.Input.KeyboardAndMouse;
using static Interop.User32;

namespace System.Windows.Forms.UITests
{
    public partial class CustomForm
    {
        private class InputRedirector : IMessageFilter
        {
            private CustomForm parentForm;

            public InputRedirector(CustomForm form)
            {
                parentForm = form;
            }

            public bool PreFilterMessage(ref Message m)
            {
                if (m.MsgInternal == WM.KEYDOWN || m.MsgInternal == WM.KEYUP)
                {
                    var keyCode = (VIRTUAL_KEY)(int)m.WParamInternal;
                    if (keyCode == TestKey)
                    {
                        if (parentForm._manualResetEventSlim is not null && !parentForm._manualResetEventSlim.IsSet)
                        {
                            parentForm._manualResetEventSlim.Set();
                        }

                        return true;
                    }
                }

                return false;
            }
        }
    }
}
