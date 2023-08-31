// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public partial class ToolTip
{
    private class ToolTipNativeWindow : NativeWindow
    {
        private readonly ToolTip _toolTip;

        internal ToolTipNativeWindow(ToolTip toolTip)
        {
            _toolTip = toolTip.OrThrowIfNull();
        }

        protected override void WndProc(ref Message m) => _toolTip.WndProc(ref m);
    }
}
