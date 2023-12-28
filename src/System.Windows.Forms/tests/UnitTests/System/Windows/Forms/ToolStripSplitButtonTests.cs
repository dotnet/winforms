// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

public class ToolStripSplitButtonTests
{
    public static IEnumerable<object[]> ToolStripItem_Set_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new SubToolStripItem() };
    }

    [WinFormsTheory]
    [MemberData(nameof(ToolStripItem_Set_TestData))]
    public void ToolStripSplitButton_DefaultItem_Set_GetReturnsExpected(ToolStripItem value)
    {
        using ToolStripSplitButton button = new()
        {
            DefaultItem = value
        };
        Assert.Same(value, button.DefaultItem);

        // Set same.
        button.DefaultItem = value;
        Assert.Same(value, button.DefaultItem);
    }

    [WinFormsFact]
    public void ToolStripSplitButton_DefaultItem_SetWithHandler_CallsOnDefaultItemChanged()
    {
        using ToolStripSplitButton button = new();

        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(button, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        button.DefaultItemChanged += handler;

        // Set non-null.
        using SubToolStripItem item1 = new();
        button.DefaultItem = item1;
        Assert.Same(item1, button.DefaultItem);
        Assert.Equal(1, callCount);

        // Set same.
        button.DefaultItem = item1;
        Assert.Same(item1, button.DefaultItem);
        Assert.Equal(1, callCount);

        // Set different.
        using SubToolStripItem item2 = new();
        button.DefaultItem = item2;
        Assert.Same(item2, button.DefaultItem);
        Assert.Equal(2, callCount);

        // Set null.
        button.DefaultItem = null;
        Assert.Null(button.DefaultItem);
        Assert.Equal(3, callCount);

        // Remove handler.
        button.DefaultItemChanged -= handler;
        button.DefaultItem = item1;
        Assert.Equal(item1, button.DefaultItem);
        Assert.Equal(3, callCount);
    }

    private class SubToolStripItem : ToolStripItem
    {
        public SubToolStripItem() : base()
        {
        }
    }
}
