// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design;

namespace System.Windows.Forms.Design;

/// <summary>
///  Associates DesignerVerb with ToolStripMenuItem.
/// </summary>
internal class DesignerVerbToolStripMenuItem : ToolStripMenuItem
{
    private readonly DesignerVerb _verb;

    public DesignerVerbToolStripMenuItem(DesignerVerb verb)
    {
        _verb = verb;
        Text = verb.Text;
        RefreshItem();
    }

    public void RefreshItem()
    {
        if (_verb is not null)
        {
            Visible = _verb.Visible;
            Enabled = _verb.Enabled;
            Checked = _verb.Checked;
        }
    }

    protected override void OnClick(EventArgs e)
    {
        _verb?.Invoke();
    }
}
