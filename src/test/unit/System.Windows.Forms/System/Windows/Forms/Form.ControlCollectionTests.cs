// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

namespace System.Windows.Forms.Tests;

public class Form_ControlCollection
{
    [WinFormsFact]
    public void ControlCollection_Ctor_Control()
    {
        using Form owner = new();
        Form.ControlCollection collection = new(owner);

        Assert.Empty(collection);
        Assert.False(collection.IsReadOnly);
        Assert.Same(owner, collection.Owner);
    }

    [WinFormsFact]
    public void ControlCollection_Ctor_NullOwner_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("owner", () => new Form.ControlCollection(null));
    }

    [WinFormsFact]
    public void ControlCollection_Add_InvalidTabPageParent_DoesNotLeaveHalfAddedControl()
    {
        using Form form = new();
        using TabPage tabPage = new();

        int oldCount = form.Controls.Count;

        Assert.ThrowsAny<Exception>(() => form.Controls.Add(tabPage));

        Assert.Equal(oldCount, form.Controls.Count);
        Assert.False(form.Controls.Contains(tabPage));
        Assert.Null(tabPage.Parent);
    }

    [WinFormsFact]
    public void ControlCollection_Clear_AfterFailedTabPageAdd_DoesNotHang()
    {
        using Form form = new();
        using Button button = new() { Name = "button1" };
        using TabPage tabPage = new();

        form.Controls.Add(button);

        Assert.ThrowsAny<Exception>(() => form.Controls.Add(tabPage));

        // If the failed add left the TabPage half-added,
        // Clear() could hang. After the fix, it should complete normally.
        form.Controls.Clear();

        Assert.Empty(form.Controls);
        Assert.Null(tabPage.Parent);
    }
}
