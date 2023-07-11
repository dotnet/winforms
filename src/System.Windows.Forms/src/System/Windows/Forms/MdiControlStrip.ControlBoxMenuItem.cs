// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

internal partial class MdiControlStrip
{
    // when the system menu item shortcut is evaluated - pop the dropdown
    internal class ControlBoxMenuItem : ToolStripMenuItem
    {
        internal ControlBoxMenuItem(HMENU hMenu, uint nativeMenuCommandId, IWin32Window targetWindow)
            : base(hMenu, (int)nativeMenuCommandId, targetWindow)
        {
        }

        internal override bool CanKeyboardSelect => false;
    }
}
