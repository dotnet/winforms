// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

public class TabPageTabPageControlCollectionTests
{
    [WinFormsFact]
    public void TabPageControlCollection_Ctor_TabPage()
    {
        using TabPage owner = new();
        var collection = new TabPage.TabPageControlCollection(owner);
        Assert.Empty(collection);
        Assert.False(collection.IsReadOnly);
        Assert.Same(owner, collection.Owner);
    }

    [WinFormsFact]
    public void TabPageControlCollection_NullOwner_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("owner", () => new TabPage.TabPageControlCollection(null));
    }

    [WinFormsFact]
    public void TabPageControlCollection_Add_ControlExistingCollection_Success()
    {
        using TabPage owner = new();
        using Control control1 = new();
        using Control control2 = new();
        TabPage.TabPageControlCollection collection = Assert.IsType<TabPage.TabPageControlCollection>(owner.Controls);
        int parentLayoutCallCount = 0;
        string affectedProperty = null;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(owner, sender);
            Assert.Same(collection.Cast<Control>().Last(), e.AffectedControl);
            Assert.Equal(affectedProperty, e.AffectedProperty);
            parentLayoutCallCount++;
        }

        owner.Layout += parentHandler;
        int layoutCallCount1 = 0;
        control1.Layout += (sender, e) => layoutCallCount1++;
        int layoutCallCount2 = 0;
        control2.Layout += (sender, e) => layoutCallCount2++;

        try
        {
            affectedProperty = "Parent";
            collection.Add(control1);
            Assert.Same(control1, Assert.Single(collection));
            Assert.Same(owner, control1.Parent);
            Assert.Equal(0, control1.TabIndex);
            Assert.Same(control1, Assert.Single(owner.Controls));
            Assert.Equal(0, layoutCallCount1);
            Assert.Equal(1, parentLayoutCallCount);
            Assert.False(owner.IsHandleCreated);
            Assert.False(control1.IsHandleCreated);

            // Add another.
            collection.Add(control2);
            Assert.Equal(new Control[] { control1, control2 }, collection.Cast<Control>());
            Assert.Same(owner, control1.Parent);
            Assert.Equal(0, control1.TabIndex);
            Assert.Same(owner, control2.Parent);
            Assert.Equal(1, control2.TabIndex);
            Assert.Equal(new Control[] { control1, control2 }, owner.Controls.Cast<Control>());
            Assert.Equal(0, layoutCallCount1);
            Assert.Equal(0, layoutCallCount2);
            Assert.Equal(2, parentLayoutCallCount);
            Assert.False(owner.IsHandleCreated);
            Assert.False(control1.IsHandleCreated);
            Assert.False(control2.IsHandleCreated);

            // Add existing.
            affectedProperty = "ChildIndex";
            collection.Add(control1);
            Assert.Equal(new Control[] { control2, control1 }, collection.Cast<Control>());
            Assert.Same(owner, control1.Parent);
            Assert.Equal(0, control1.TabIndex);
            Assert.Same(owner, control2.Parent);
            Assert.Equal(1, control2.TabIndex);
            Assert.Equal(new Control[] { control2, control1 }, owner.Controls.Cast<Control>());
            Assert.Equal(0, layoutCallCount1);
            Assert.Equal(0, layoutCallCount2);
            Assert.Equal(3, parentLayoutCallCount);
            Assert.False(owner.IsHandleCreated);
            Assert.False(control1.IsHandleCreated);
            Assert.False(control2.IsHandleCreated);

            // Add null.
            collection.Add(null);
            Assert.Equal(new Control[] { control2, control1 }, collection.Cast<Control>());
        }
        finally
        {
            owner.Layout -= parentHandler;
        }
    }

    [WinFormsFact]
    public void ControlCollection_Add_TabPageValue_ThrowsArgumentException()
    {
        using TabPage owner = new();
        using TabPage control = new();
        var collection = new TabPage.TabPageControlCollection(owner);
        Assert.Throws<ArgumentException>(() => collection.Add(control));
    }
}
