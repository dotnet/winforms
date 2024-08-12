// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace WinFormsControlsTest;

internal class CloneTestListView : ListView
{
    public void InvokeOnItemChecked(ItemCheckedEventArgs e)
    {
        if (!CheckBoxes)
        {
            return;
        }

        if (e.Item.ListView == this)
        {
            base.OnItemChecked(e);
        }
    }
}
