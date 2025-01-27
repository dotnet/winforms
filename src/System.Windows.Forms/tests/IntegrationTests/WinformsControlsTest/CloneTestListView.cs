// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace WinFormsControlsTest;

[DesignerCategory("Default")]
internal class CloneTestListView : ListView
{
    public void InvokeOnItemChecked(ItemCheckedEventArgs e)
    {
        if (!CheckBoxes)
        {
            return;
        }

        base.OnItemChecked(e);
    }
}
