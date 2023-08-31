// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

internal partial class MdiControlStrip
{
    // when the system menu item shortcut is evaluated - pop the dropdown
    internal class SystemMenuItem : ToolStripMenuItem
    {
        public SystemMenuItem()
        {
            AccessibleName = SR.MDIChildSystemMenuItemAccessibleName;
        }

        protected internal override bool ProcessCmdKey(ref Message m, Keys keyData)
        {
            if (Visible && ShortcutKeys == keyData)
            {
                ShowDropDown();
                DropDown.SelectNextToolStripItem(start: null, forward: true);
                return true;
            }

            return base.ProcessCmdKey(ref m, keyData);
        }

        protected override void OnOwnerChanged(EventArgs e)
        {
            if (HasDropDownItems && DropDown.Visible)
            {
                HideDropDown();
            }

            base.OnOwnerChanged(e);
        }
    }
}
