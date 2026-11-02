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

        Assert.Throws<ArgumentException>(() => form.Controls.Add(tabPage));

        Assert.Equal(oldCount, form.Controls.Count);
        Assert.False(form.Controls.Contains(tabPage));
        Assert.Null(tabPage.Parent);
    }

    [WinFormsFact]
    public void ControlCollection_Clear_AfterFailedTabPageAdd_DoesNotHang()
    {
        using Form form = new();
        using Button button = new();
        using TabPage tabPage = new();

        form.Controls.Add(button);

        Assert.Throws<ArgumentException>(() => form.Controls.Add(tabPage));

        // Check the invariant before Clear so a regression fails instead of hanging.
        Assert.Same(button, Assert.Single(form.Controls));
        Assert.Null(tabPage.Parent);

        form.Controls.Clear();

        Assert.Empty(form.Controls);
        Assert.Null(button.Parent);
        Assert.Null(tabPage.Parent);
    }

    [WinFormsTheory]
    [InlineData(null, 8)]
    [InlineData(3, 3)]
    public void ControlCollection_Add_InvalidTabPageParent_PreservesTabIndexForValidParent(int? tabIndex, int expectedTabIndex)
    {
        using Form form = new();
        using TabPage tabPage = new();
        using TabControl tabControl = new();
        using TabPage existingTabPage = new() { TabIndex = 7 };

        if (tabIndex is int index)
        {
            tabPage.TabIndex = index;
        }

        Assert.Throws<ArgumentException>(() => form.Controls.Add(tabPage));

        Assert.Empty(form.Controls);
        Assert.Null(tabPage.Parent);
        Assert.Equal(tabIndex ?? 0, tabPage.TabIndex);

        tabControl.Controls.Add(existingTabPage);
        tabControl.Controls.Add(tabPage);

        Assert.Same(tabControl, tabPage.Parent);
        Assert.Equal(expectedTabIndex, tabPage.TabIndex);
    }

    [WinFormsTheory]
    [InlineData(false)]
    [InlineData(true)]
    public void ControlCollection_Add_ParentChangedThrows_PreservesAssignedParent(bool createHandle)
    {
        using Form form = new();
        using Button existingControl = new() { TabIndex = 5 };
        using Control control = new();
        InvalidOperationException exception = new();

        form.Controls.Add(existingControl);

        if (createHandle)
        {
            _ = form.Handle;
        }

        void ParentChangedHandler(object sender, EventArgs e)
        {
            if (createHandle)
            {
                _ = control.Handle;
            }

            throw exception;
        }

        control.ParentChanged += ParentChangedHandler;
        try
        {
            Assert.Same(exception, Assert.Throws<InvalidOperationException>(() => form.Controls.Add(control)));
        }
        finally
        {
            control.ParentChanged -= ParentChangedHandler;
        }

        Assert.Same(form, control.Parent);
        Assert.Contains(control, form.Controls.Cast<Control>());
        Assert.Equal(6, control.TabIndex);
        Assert.Equal(createHandle, control.IsHandleCreated);

        if (createHandle)
        {
            Assert.Equal(form.Handle, PInvoke.GetParent(control));
        }
    }
}
