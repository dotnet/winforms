// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    public partial class ToolTip
    {
        private class ToolTipNativeWindow : NativeWindow
        {
            private readonly ToolTip _control;

            internal ToolTipNativeWindow(ToolTip control)
            {
                _control = control;
            }

            protected override void WndProc(ref Message m) => _control?.WndProc(ref m);
        }
    }
}
