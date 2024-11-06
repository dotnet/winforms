// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.Drawing;
using Moq;
using Size = System.Drawing.Size;

namespace System.Windows.Forms.Tests;

public class TabControlTabPageCollectionTests
{
    public static IEnumerable<object[]> Add_TestData()
    {
        yield return new object[] { TabAppearance.Buttons };
        yield return new object[] { TabAppearance.FlatButtons };
        yield return new object[] { TabAppearance.Normal };
    }

    [WinFormsTheory]
    [MemberData(nameof(Add_TestData))]
    public void TabPageCollection_Add_InvokeValueWithoutHandleOwnerWithoutHandle_Success(TabAppearance appearance)
    {
        using TabControl owner = new()
        {
            Appearance = appearance,
            Bounds = new Rectangle(0, 0, 400, 300)
        };
        using TabPage value1 = new();
        using TabPage value2 = new();
        var collection = new TabControl.TabPageCollection(owner);

        int layoutCallCount1 = 0;
        value1.Layout += (sender, e) => layoutCallCount1++;
        int layoutCallCount2 = 0;
        value2.Layout += (sender, e) => layoutCallCount2++;
        int parentLayoutCallCount = 0;
        List<LayoutEventArgs> events = [];
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(owner, sender);
            events.Add(e);
            parentLayoutCallCount++;
        }

        owner.Layout += parentHandler;

        try
        {
            // Add first.
            collection.Add(value1);
            Assert.Same(value1, Assert.Single(collection));
            Assert.Same(value1, Assert.Single(owner.TabPages));
            Assert.Same(value1, Assert.Single(owner.Controls));
            Assert.Same(owner, value1.Parent);
            Assert.False(value1.Visible);
            Assert.Equal(new Rectangle(0, 0, 200, 100), value1.Bounds);
            Assert.Null(value1.Site);
            Assert.Equal(-1, owner.SelectedIndex);
            Assert.Equal(0, layoutCallCount1);
            Assert.Equal(2, parentLayoutCallCount);
            Assert.Same(value1, events[0].AffectedControl);
            Assert.Equal("Parent", events[0].AffectedProperty);
            Assert.Same(value1, events[1].AffectedControl);
            Assert.Equal("Visible", events[1].AffectedProperty);
            Assert.False(value1.IsHandleCreated);
            Assert.False(owner.IsHandleCreated);

            // Add another.
            collection.Add(value2);
            Assert.Equal(new TabPage[] { value1, value2 }, collection.Cast<TabPage>());
            Assert.Equal(new TabPage[] { value1, value2 }, owner.TabPages.Cast<TabPage>());
            Assert.Equal(new Control[] { value1, value2 }, owner.Controls.Cast<Control>());
            Assert.Same(owner, value1.Parent);
            Assert.False(value1.Visible);
            Assert.Equal(new Rectangle(0, 0, 200, 100), value1.Bounds);
            Assert.Null(value1.Site);
            Assert.Same(owner, value2.Parent);
            Assert.False(value2.Visible);
            Assert.Equal(new Rectangle(0, 0, 200, 100), value2.Bounds);
            Assert.Null(value2.Site);
            Assert.Equal(-1, owner.SelectedIndex);
            Assert.Equal(0, layoutCallCount1);
            Assert.Equal(0, layoutCallCount2);
            Assert.Equal(4, parentLayoutCallCount);
            Assert.Same(value1, events[0].AffectedControl);
            Assert.Equal("Parent", events[0].AffectedProperty);
            Assert.Same(value1, events[1].AffectedControl);
            Assert.Equal("Visible", events[1].AffectedProperty);
            Assert.Same(value2, events[2].AffectedControl);
            Assert.Equal("Parent", events[2].AffectedProperty);
            Assert.Same(value2, events[3].AffectedControl);
            Assert.Equal("Visible", events[3].AffectedProperty);
            Assert.False(value1.IsHandleCreated);
            Assert.False(value2.IsHandleCreated);
            Assert.False(owner.IsHandleCreated);

            // Add again.
            collection.Add(value1);
            Assert.Equal(new TabPage[] { value1, value2, value1 }, collection.Cast<TabPage>());
            Assert.Equal(new TabPage[] { value1, value2, value1 }, owner.TabPages.Cast<TabPage>());
            Assert.Equal(new Control[] { value2, value1 }, owner.Controls.Cast<Control>());
            Assert.Same(owner, value1.Parent);
            Assert.False(value1.Visible);
            Assert.Same(owner, value2.Parent);
            Assert.Equal(new Rectangle(0, 0, 200, 100), value1.Bounds);
            Assert.Null(value1.Site);
            Assert.False(value2.Visible);
            Assert.Equal(new Rectangle(0, 0, 200, 100), value2.Bounds);
            Assert.Null(value2.Site);
            Assert.Equal(-1, owner.SelectedIndex);
            Assert.Equal(0, layoutCallCount1);
            Assert.Equal(5, parentLayoutCallCount);
            Assert.Same(value1, events[0].AffectedControl);
            Assert.Equal("Parent", events[0].AffectedProperty);
            Assert.Same(value1, events[1].AffectedControl);
            Assert.Equal("Visible", events[1].AffectedProperty);
            Assert.Same(value2, events[2].AffectedControl);
            Assert.Equal("Parent", events[2].AffectedProperty);
            Assert.Same(value2, events[3].AffectedControl);
            Assert.Equal("Visible", events[3].AffectedProperty);
            Assert.Same(value1, events[4].AffectedControl);
            Assert.Equal("ChildIndex", events[4].AffectedProperty);
            Assert.False(value1.IsHandleCreated);
            Assert.False(value2.IsHandleCreated);
            Assert.False(owner.IsHandleCreated);
        }
        finally
        {
            owner.Layout -= parentHandler;
        }
    }

    public static IEnumerable<object[]> Add_WithHandle_TestData()
    {
        yield return new object[] { TabAppearance.Buttons, Size.Empty, 0 };
        yield return new object[] { TabAppearance.FlatButtons, Size.Empty, 1 };
        yield return new object[] { TabAppearance.Normal, Size.Empty, 0 };

        yield return new object[] { TabAppearance.Buttons, new Size(100, 120), 0 };
        yield return new object[] { TabAppearance.FlatButtons, new Size(100, 120), 1 };
        yield return new object[] { TabAppearance.Normal, new Size(100, 120), 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(Add_WithHandle_TestData))]
    public void TabPageCollection_Add_InvokeValueWithoutHandleOwnerWithHandle_Success(TabAppearance appearance, Size itemSize, int expectedParentInvalidatedCallCount)
    {
        using TabControl owner = new()
        {
            Appearance = appearance,
            ItemSize = itemSize,
            Bounds = new Rectangle(0, 0, 400, 300)
        };
        using TabPage value1 = new();
        using TabPage value2 = new();
        var collection = new TabControl.TabPageCollection(owner);

        int layoutCallCount1 = 0;
        value1.Layout += (sender, e) => layoutCallCount1++;
        int layoutCallCount2 = 0;
        value2.Layout += (sender, e) => layoutCallCount2++;
        int parentLayoutCallCount = 0;
        List<LayoutEventArgs> events = [];
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(owner, sender);
            events.Add(e);
            parentLayoutCallCount++;
        }

        owner.Layout += parentHandler;
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int parentInvalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => parentInvalidatedCallCount++;
        int parentStyleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
        int parentCreatedCallCount = 0;
        owner.HandleCreated += (sender, e) => parentCreatedCallCount++;

        try
        {
            // Add first.
            collection.Add(value1);
            Assert.Same(value1, Assert.Single(collection));
            Assert.Same(value1, Assert.Single(owner.TabPages));
            Assert.Same(value1, Assert.Single(owner.Controls));
            Assert.Same(owner, value1.Parent);
            Assert.True(value1.Visible);
            Assert.Equal(owner.DisplayRectangle, value1.Bounds);
            Assert.Null(value1.Site);
            Assert.Equal(0, owner.SelectedIndex);
            Assert.Equal(2, layoutCallCount1);
            Assert.Equal(5, parentLayoutCallCount);
            Assert.Same(value1, events[0].AffectedControl);
            Assert.Equal("Parent", events[0].AffectedProperty);
            Assert.Same(value1, events[1].AffectedControl);
            Assert.Equal("Visible", events[1].AffectedProperty);
            Assert.Same(value1, events[2].AffectedControl);
            Assert.Equal("Bounds", events[2].AffectedProperty);
            Assert.Same(value1, events[3].AffectedControl);
            Assert.Equal("Bounds", events[3].AffectedProperty);
            Assert.Same(value1, events[4].AffectedControl);
            Assert.Equal("Visible", events[4].AffectedProperty);
            Assert.True(value1.IsHandleCreated);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(expectedParentInvalidatedCallCount, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);

            // Add another.
            collection.Add(value2);
            Assert.Equal(new TabPage[] { value1, value2 }, collection.Cast<TabPage>());
            Assert.Equal(new TabPage[] { value1, value2 }, owner.TabPages.Cast<TabPage>());
            Assert.Equal(new Control[] { value1, value2 }, owner.Controls.Cast<Control>());
            Assert.Same(owner, value1.Parent);
            Assert.True(value1.Visible);
            Assert.Equal(owner.DisplayRectangle, value1.Bounds);
            Assert.Null(value1.Site);
            Assert.Same(owner, value2.Parent);
            Assert.False(value2.Visible);
            Assert.Equal(owner.DisplayRectangle, value2.Bounds);
            Assert.Null(value2.Site);
            Assert.Equal(0, owner.SelectedIndex);
            Assert.Equal(2, layoutCallCount1);
            Assert.Equal(1, layoutCallCount2);
            Assert.Equal(9, parentLayoutCallCount);
            Assert.Same(value1, events[0].AffectedControl);
            Assert.Equal("Parent", events[0].AffectedProperty);
            Assert.Same(value1, events[1].AffectedControl);
            Assert.Equal("Visible", events[1].AffectedProperty);
            Assert.Same(value1, events[2].AffectedControl);
            Assert.Equal("Bounds", events[2].AffectedProperty);
            Assert.Same(value1, events[3].AffectedControl);
            Assert.Equal("Bounds", events[3].AffectedProperty);
            Assert.Same(value1, events[4].AffectedControl);
            Assert.Equal("Visible", events[4].AffectedProperty);
            Assert.Same(value2, events[5].AffectedControl);
            Assert.Equal("Parent", events[5].AffectedProperty);
            Assert.Same(value2, events[6].AffectedControl);
            Assert.Equal("Visible", events[6].AffectedProperty);
            Assert.Same(value2, events[7].AffectedControl);
            Assert.Equal("Bounds", events[7].AffectedProperty);
            Assert.Same(value2, events[8].AffectedControl);
            Assert.Equal("Bounds", events[8].AffectedProperty);
            Assert.True(value1.IsHandleCreated);
            Assert.False(value2.IsHandleCreated);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(expectedParentInvalidatedCallCount * 2, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);

            // Add again.
            collection.Add(value1);
            Assert.Equal(new TabPage[] { value1, value2, value1 }, collection.Cast<TabPage>());
            Assert.Equal(new TabPage[] { value1, value2, value1 }, owner.TabPages.Cast<TabPage>());
            Assert.Equal(new Control[] { value2, value1 }, owner.Controls.Cast<Control>());
            Assert.Same(owner, value1.Parent);
            Assert.False(value1.Visible);
            Assert.Equal(owner.DisplayRectangle, value1.Bounds);
            Assert.Null(value1.Site);
            Assert.Same(owner, value2.Parent);
            Assert.False(value2.Visible);
            Assert.Equal(owner.DisplayRectangle, value2.Bounds);
            Assert.Null(value2.Site);
            Assert.Equal(0, owner.SelectedIndex);
            Assert.Equal(3, layoutCallCount1);
            Assert.Equal(1, layoutCallCount2);
            Assert.Equal(13, parentLayoutCallCount);
            Assert.Same(value1, events[0].AffectedControl);
            Assert.Equal("Parent", events[0].AffectedProperty);
            Assert.Same(value1, events[1].AffectedControl);
            Assert.Equal("Visible", events[1].AffectedProperty);
            Assert.Same(value1, events[2].AffectedControl);
            Assert.Equal("Bounds", events[2].AffectedProperty);
            Assert.Same(value1, events[3].AffectedControl);
            Assert.Equal("Bounds", events[3].AffectedProperty);
            Assert.Same(value1, events[4].AffectedControl);
            Assert.Equal("Visible", events[4].AffectedProperty);
            Assert.Same(value2, events[5].AffectedControl);
            Assert.Equal("Parent", events[5].AffectedProperty);
            Assert.Same(value2, events[6].AffectedControl);
            Assert.Equal("Visible", events[6].AffectedProperty);
            Assert.Same(value2, events[7].AffectedControl);
            Assert.Equal("Bounds", events[7].AffectedProperty);
            Assert.Same(value2, events[8].AffectedControl);
            Assert.Equal("Bounds", events[8].AffectedProperty);
            Assert.Same(value1, events[9].AffectedControl);
            Assert.Equal("ChildIndex", events[9].AffectedProperty);
            Assert.Same(value1, events[10].AffectedControl);
            Assert.Equal("Visible", events[10].AffectedProperty);
            Assert.Same(value1, events[11].AffectedControl);
            Assert.Equal("Visible", events[11].AffectedProperty);
            Assert.Same(value1, events[12].AffectedControl);
            Assert.Equal("Visible", events[12].AffectedProperty);
            Assert.True(value1.IsHandleCreated);
            Assert.False(value2.IsHandleCreated);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(expectedParentInvalidatedCallCount * 3, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);
        }
        finally
        {
            owner.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Add_TestData))]
    public void TabPageCollection_Add_InvokeValueWithHandleOwnerWithoutHandle_Success(TabAppearance appearance)
    {
        using TabControl owner = new()
        {
            Appearance = appearance,
            Bounds = new Rectangle(0, 0, 400, 300)
        };
        using TabPage value1 = new();
        using TabPage value2 = new();
        var collection = new TabControl.TabPageCollection(owner);

        int layoutCallCount1 = 0;
        value1.Layout += (sender, e) => layoutCallCount1++;
        int layoutCallCount2 = 0;
        value2.Layout += (sender, e) => layoutCallCount2++;
        int parentLayoutCallCount = 0;
        List<LayoutEventArgs> events = [];
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(owner, sender);
            events.Add(e);
            parentLayoutCallCount++;
        }

        owner.Layout += parentHandler;
        Assert.NotEqual(IntPtr.Zero, value1.Handle);
        int invalidatedCallCount1 = 0;
        value1.Invalidated += (sender, e) => invalidatedCallCount1++;
        int styleChangedCallCount1 = 0;
        value1.StyleChanged += (sender, e) => styleChangedCallCount1++;
        int createdCallCount1 = 0;
        value1.HandleCreated += (sender, e) => createdCallCount1++;
        Assert.NotEqual(IntPtr.Zero, value2.Handle);
        int invalidatedCallCount2 = 0;
        value2.Invalidated += (sender, e) => invalidatedCallCount2++;
        int styleChangedCallCount2 = 0;
        value2.StyleChanged += (sender, e) => styleChangedCallCount2++;
        int createdCallCount2 = 0;
        value2.HandleCreated += (sender, e) => createdCallCount2++;

        try
        {
            // Add first.
            collection.Add(value1);
            Assert.Same(value1, Assert.Single(collection));
            Assert.Same(value1, Assert.Single(owner.TabPages));
            Assert.Same(value1, Assert.Single(owner.Controls));
            Assert.Same(owner, value1.Parent);
            Assert.False(value1.Visible);
            Assert.Equal(new Rectangle(0, 0, 200, 100), value1.Bounds);
            Assert.Null(value1.Site);
            Assert.Equal(-1, owner.SelectedIndex);
            Assert.Equal(0, layoutCallCount1);
            Assert.Equal(2, parentLayoutCallCount);
            Assert.Same(value1, events[0].AffectedControl);
            Assert.Equal("Parent", events[0].AffectedProperty);
            Assert.Same(value1, events[1].AffectedControl);
            Assert.Equal("Visible", events[1].AffectedProperty);
            Assert.True(value1.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount1);
            Assert.Equal(0, styleChangedCallCount1);
            Assert.Equal(0, createdCallCount1);
            Assert.False(owner.IsHandleCreated);

            // Add another.
            collection.Add(value2);
            Assert.Equal(new TabPage[] { value1, value2 }, collection.Cast<TabPage>());
            Assert.Equal(new TabPage[] { value1, value2 }, owner.TabPages.Cast<TabPage>());
            Assert.Equal(new Control[] { value1, value2 }, owner.Controls.Cast<Control>());
            Assert.Same(owner, value1.Parent);
            Assert.False(value1.Visible);
            Assert.Equal(new Rectangle(0, 0, 200, 100), value1.Bounds);
            Assert.Null(value1.Site);
            Assert.Same(owner, value2.Parent);
            Assert.False(value2.Visible);
            Assert.Equal(new Rectangle(0, 0, 200, 100), value2.Bounds);
            Assert.Null(value2.Site);
            Assert.Equal(-1, owner.SelectedIndex);
            Assert.Equal(0, layoutCallCount1);
            Assert.Equal(0, layoutCallCount2);
            Assert.Equal(4, parentLayoutCallCount);
            Assert.Same(value1, events[0].AffectedControl);
            Assert.Equal("Parent", events[0].AffectedProperty);
            Assert.Same(value1, events[1].AffectedControl);
            Assert.Equal("Visible", events[1].AffectedProperty);
            Assert.Same(value2, events[2].AffectedControl);
            Assert.Equal("Parent", events[2].AffectedProperty);
            Assert.Same(value2, events[3].AffectedControl);
            Assert.Equal("Visible", events[3].AffectedProperty);
            Assert.True(value1.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount1);
            Assert.Equal(0, styleChangedCallCount1);
            Assert.Equal(0, createdCallCount1);
            Assert.True(value2.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount2);
            Assert.Equal(0, styleChangedCallCount2);
            Assert.Equal(0, createdCallCount2);
            Assert.False(owner.IsHandleCreated);

            // Add again.
            collection.Add(value1);
            Assert.Equal(new TabPage[] { value1, value2, value1, }, collection.Cast<TabPage>());
            Assert.Equal(new TabPage[] { value1, value2, value1 }, owner.TabPages.Cast<TabPage>());
            Assert.Equal(new Control[] { value2, value1 }, owner.Controls.Cast<Control>());
            Assert.Same(owner, value1.Parent);
            Assert.False(value1.Visible);
            Assert.Same(owner, value2.Parent);
            Assert.Equal(new Rectangle(0, 0, 200, 100), value1.Bounds);
            Assert.Null(value1.Site);
            Assert.False(value2.Visible);
            Assert.Equal(new Rectangle(0, 0, 200, 100), value2.Bounds);
            Assert.Null(value2.Site);
            Assert.Equal(-1, owner.SelectedIndex);
            Assert.Equal(0, layoutCallCount1);
            Assert.Equal(5, parentLayoutCallCount);
            Assert.Same(value1, events[0].AffectedControl);
            Assert.Equal("Parent", events[0].AffectedProperty);
            Assert.Same(value1, events[1].AffectedControl);
            Assert.Equal("Visible", events[1].AffectedProperty);
            Assert.Same(value2, events[2].AffectedControl);
            Assert.Equal("Parent", events[2].AffectedProperty);
            Assert.Same(value2, events[3].AffectedControl);
            Assert.Equal("Visible", events[3].AffectedProperty);
            Assert.Same(value1, events[4].AffectedControl);
            Assert.Equal("ChildIndex", events[4].AffectedProperty);
            Assert.True(value1.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount1);
            Assert.Equal(0, styleChangedCallCount1);
            Assert.Equal(0, createdCallCount1);
            Assert.True(value2.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount2);
            Assert.Equal(0, styleChangedCallCount2);
            Assert.Equal(0, createdCallCount2);
            Assert.False(owner.IsHandleCreated);
        }
        finally
        {
            owner.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Add_WithHandle_TestData))]
    public void TabPageCollection_Add_InvokeValueWithHandleOwnerWithHandle_Success(TabAppearance appearance, Size itemSize, int expectedParentInvalidatedCallCount)
    {
        using TabControl owner = new()
        {
            Appearance = appearance,
            ItemSize = itemSize,
            Bounds = new Rectangle(0, 0, 400, 300)
        };
        using TabPage value1 = new();
        using TabPage value2 = new();
        var collection = new TabControl.TabPageCollection(owner);

        int layoutCallCount1 = 0;
        value1.Layout += (sender, e) => layoutCallCount1++;
        int layoutCallCount2 = 0;
        value2.Layout += (sender, e) => layoutCallCount2++;
        int parentLayoutCallCount = 0;
        List<LayoutEventArgs> events = [];
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(owner, sender);
            events.Add(e);
            parentLayoutCallCount++;
        }

        owner.Layout += parentHandler;
        Assert.NotEqual(IntPtr.Zero, value1.Handle);
        int invalidatedCallCount1 = 0;
        value1.Invalidated += (sender, e) => invalidatedCallCount1++;
        int styleChangedCallCount1 = 0;
        value1.StyleChanged += (sender, e) => styleChangedCallCount1++;
        int createdCallCount1 = 0;
        value1.HandleCreated += (sender, e) => createdCallCount1++;
        Assert.NotEqual(IntPtr.Zero, value2.Handle);
        int invalidatedCallCount2 = 0;
        value2.Invalidated += (sender, e) => invalidatedCallCount2++;
        int styleChangedCallCount2 = 0;
        value2.StyleChanged += (sender, e) => styleChangedCallCount2++;
        int createdCallCount2 = 0;
        value2.HandleCreated += (sender, e) => createdCallCount2++;
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int parentInvalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => parentInvalidatedCallCount++;
        int parentStyleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
        int parentCreatedCallCount = 0;
        owner.HandleCreated += (sender, e) => parentCreatedCallCount++;

        try
        {
            // Add first.
            collection.Add(value1);
            Assert.Same(value1, Assert.Single(collection));
            Assert.Same(value1, Assert.Single(owner.TabPages));
            Assert.Same(value1, Assert.Single(owner.Controls));
            Assert.Same(owner, value1.Parent);
            Assert.True(value1.Visible);
            Assert.Equal(owner.DisplayRectangle, value1.Bounds);
            Assert.Null(value1.Site);
            Assert.Equal(0, owner.SelectedIndex);
            Assert.Equal(2, layoutCallCount1);
            Assert.Equal(5, parentLayoutCallCount);
            Assert.Same(value1, events[0].AffectedControl);
            Assert.Equal("Parent", events[0].AffectedProperty);
            Assert.Same(value1, events[1].AffectedControl);
            Assert.Equal("Visible", events[1].AffectedProperty);
            Assert.Same(value1, events[2].AffectedControl);
            Assert.Equal("Bounds", events[2].AffectedProperty);
            Assert.Same(value1, events[3].AffectedControl);
            Assert.Equal("Bounds", events[3].AffectedProperty);
            Assert.Same(value1, events[4].AffectedControl);
            Assert.Equal("Visible", events[4].AffectedProperty);
            Assert.True(value1.IsHandleCreated);
            Assert.Equal(1, invalidatedCallCount1);
            Assert.Equal(0, styleChangedCallCount1);
            Assert.Equal(0, createdCallCount1);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(expectedParentInvalidatedCallCount, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);

            // Add another.
            collection.Add(value2);
            Assert.Equal(new TabPage[] { value1, value2 }, collection.Cast<TabPage>());
            Assert.Equal(new TabPage[] { value1, value2 }, owner.TabPages.Cast<TabPage>());
            Assert.Equal(new Control[] { value1, value2 }, owner.Controls.Cast<Control>());
            Assert.Same(owner, value1.Parent);
            Assert.True(value1.Visible);
            Assert.Equal(owner.DisplayRectangle, value1.Bounds);
            Assert.Null(value1.Site);
            Assert.Same(owner, value2.Parent);
            Assert.False(value2.Visible);
            Assert.Equal(owner.DisplayRectangle, value2.Bounds);
            Assert.Null(value2.Site);
            Assert.Equal(0, owner.SelectedIndex);
            Assert.Equal(2, layoutCallCount1);
            Assert.Equal(1, layoutCallCount2);
            Assert.Equal(9, parentLayoutCallCount);
            Assert.Same(value1, events[0].AffectedControl);
            Assert.Equal("Parent", events[0].AffectedProperty);
            Assert.Same(value1, events[1].AffectedControl);
            Assert.Equal("Visible", events[1].AffectedProperty);
            Assert.Same(value1, events[2].AffectedControl);
            Assert.Equal("Bounds", events[2].AffectedProperty);
            Assert.Same(value1, events[3].AffectedControl);
            Assert.Equal("Bounds", events[3].AffectedProperty);
            Assert.Same(value1, events[4].AffectedControl);
            Assert.Equal("Visible", events[4].AffectedProperty);
            Assert.Same(value2, events[5].AffectedControl);
            Assert.Equal("Parent", events[5].AffectedProperty);
            Assert.Same(value2, events[6].AffectedControl);
            Assert.Equal("Visible", events[6].AffectedProperty);
            Assert.Same(value2, events[7].AffectedControl);
            Assert.Equal("Bounds", events[7].AffectedProperty);
            Assert.Same(value2, events[8].AffectedControl);
            Assert.Equal("Bounds", events[8].AffectedProperty);
            Assert.True(value1.IsHandleCreated);
            Assert.Equal(2, invalidatedCallCount1);
            Assert.Equal(0, styleChangedCallCount1);
            Assert.Equal(0, createdCallCount1);
            Assert.True(value2.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount2);
            Assert.Equal(0, styleChangedCallCount2);
            Assert.Equal(0, createdCallCount2);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(expectedParentInvalidatedCallCount * 2, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);

            // Add again.
            collection.Add(value1);
            Assert.Equal(new TabPage[] { value1, value2, value1 }, collection.Cast<TabPage>());
            Assert.Equal(new TabPage[] { value1, value2, value1 }, owner.TabPages.Cast<TabPage>());
            Assert.Equal(new Control[] { value2, value1 }, owner.Controls.Cast<Control>());
            Assert.Same(owner, value1.Parent);
            Assert.False(value1.Visible);
            Assert.Equal(owner.DisplayRectangle, value1.Bounds);
            Assert.Null(value1.Site);
            Assert.Same(owner, value2.Parent);
            Assert.False(value2.Visible);
            Assert.Equal(owner.DisplayRectangle, value2.Bounds);
            Assert.Null(value2.Site);
            Assert.Equal(0, owner.SelectedIndex);
            Assert.Equal(3, layoutCallCount1);
            Assert.Equal(1, layoutCallCount2);
            Assert.Equal(13, parentLayoutCallCount);
            Assert.Same(value1, events[0].AffectedControl);
            Assert.Equal("Parent", events[0].AffectedProperty);
            Assert.Same(value1, events[1].AffectedControl);
            Assert.Equal("Visible", events[1].AffectedProperty);
            Assert.Same(value1, events[2].AffectedControl);
            Assert.Equal("Bounds", events[2].AffectedProperty);
            Assert.Same(value1, events[3].AffectedControl);
            Assert.Equal("Bounds", events[3].AffectedProperty);
            Assert.Same(value1, events[4].AffectedControl);
            Assert.Equal("Visible", events[4].AffectedProperty);
            Assert.Same(value2, events[5].AffectedControl);
            Assert.Equal("Parent", events[5].AffectedProperty);
            Assert.Same(value2, events[6].AffectedControl);
            Assert.Equal("Visible", events[6].AffectedProperty);
            Assert.Same(value2, events[7].AffectedControl);
            Assert.Equal("Bounds", events[7].AffectedProperty);
            Assert.Same(value2, events[8].AffectedControl);
            Assert.Equal("Bounds", events[8].AffectedProperty);
            Assert.Same(value1, events[9].AffectedControl);
            Assert.Equal("ChildIndex", events[9].AffectedProperty);
            Assert.Same(value1, events[10].AffectedControl);
            Assert.Equal("Visible", events[10].AffectedProperty);
            Assert.Same(value1, events[11].AffectedControl);
            Assert.Equal("Visible", events[11].AffectedProperty);
            Assert.Same(value1, events[12].AffectedControl);
            Assert.Equal("Visible", events[12].AffectedProperty);
            Assert.True(value1.IsHandleCreated);
            Assert.Equal(3, invalidatedCallCount1);
            Assert.Equal(0, styleChangedCallCount1);
            Assert.Equal(0, createdCallCount1);
            Assert.True(value2.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount2);
            Assert.Equal(0, styleChangedCallCount2);
            Assert.Equal(0, createdCallCount2);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(expectedParentInvalidatedCallCount * 3, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);
        }
        finally
        {
            owner.Layout -= parentHandler;
        }
    }

    [WinFormsFact]
    public void TabPageCollection_Add_OwnerWithSiteContainer_AddsToContainer()
    {
        Container container = new();
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(null);
        mockSite
            .Setup(s => s.Container)
            .Returns(container);
        using TabPage value = new();
        using TabControl owner = new()
        {
            Site = mockSite.Object
        };
        var collection = new TabControl.TabPageCollection(owner)
        {
            value
        };
        Assert.Same(owner, value.Parent);
        Assert.NotNull(value.Site);
        Assert.Same(value, Assert.Single(container.Components));
        mockSite.Verify(s => s.Container, Times.Once());
    }

    [WinFormsFact]
    public void TabPageCollection_Add_OwnerWithSiteContainerValueHasSite_DoesNotAddToContainer()
    {
        Container container = new();
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(null);
        mockSite
            .Setup(s => s.Container)
            .Returns(container);
        Mock<ISite> mockValueSite = new(MockBehavior.Strict);
        mockValueSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(null);
        mockValueSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        using TabPage value = new()
        {
            Site = mockValueSite.Object
        };
        using TabControl owner = new()
        {
            Site = mockSite.Object
        };
        var collection = new TabControl.TabPageCollection(owner)
        {
            value
        };
        Assert.Same(owner, value.Parent);
        Assert.Same(mockValueSite.Object, value.Site);
        Assert.Empty(container.Components);
        mockSite.Verify(s => s.Container, Times.Never());
    }

    [WinFormsFact]
    public void TabPageCollection_Add_OwnerWithSiteNoContainer_DoesNotAddContainer()
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(null);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        using TabPage value = new();
        using TabControl owner = new()
        {
            Site = mockSite.Object
        };
        var collection = new TabControl.TabPageCollection(owner)
        {
            value
        };
        Assert.Same(owner, value.Parent);
        Assert.Null(value.Site);
        mockSite.Verify(s => s.Container, Times.Once());
    }

    [WinFormsFact]
    public void TabPageCollection_Add_ManyControls_Success()
    {
        using TabControl owner = new();
        var collection = new TabControl.TabPageCollection(owner);

        List<TabPage> items = [];
        for (int i = 0; i < 24; i++)
        {
            TabPage value = new();
            items.Add(value);
            collection.Add(value);
            Assert.Equal(items, collection.Cast<TabPage>());
            Assert.Equal(items, owner.TabPages.Cast<TabPage>());
            Assert.Equal(items, owner.Controls.Cast<Control>());
            Assert.Same(owner, value.Parent);
        }
    }

    [WinFormsTheory]
    [InlineData("Text", "Text")]
    [InlineData("&&Text", "&&Text")]
    [InlineData("&", "&&")]
    [InlineData("&Text", "&&Text")]
    public unsafe void TabPageCollection_Add_GetItemsWithHandle_Success(string text, string expectedText)
    {
        using TabControl owner = new();
        Assert.NotEqual(IntPtr.Zero, owner.Handle);

        using TabPage page1 = new();
        using TabPage page2 = new()
        {
            Text = text,
            ImageIndex = 1
        };
        using NullTextTabPage page3 = new();
        var collection = new TabControl.TabPageCollection(owner)
        {
            page1,
            page2,
            page3
        };
        Assert.Equal(3, (int)PInvokeCore.SendMessage(owner, PInvoke.TCM_GETITEMCOUNT));

        char* buffer = stackalloc char[256];
        TCITEMW item = default;
        item.cchTextMax = int.MaxValue;
        item.pszText = buffer;
        item.dwStateMask = (TAB_CONTROL_ITEM_STATE)uint.MaxValue;
        item.mask = (TCITEMHEADERA_MASK)uint.MaxValue;

        // Get item 0.
        Assert.Equal(1, (int)PInvokeCore.SendMessage(owner, PInvoke.TCM_GETITEMW, 0, ref item));
        Assert.Equal(TAB_CONTROL_ITEM_STATE.TCIS_BUTTONPRESSED, item.dwState);
        Assert.Equal(IntPtr.Zero, (nint)item.lParam);
        Assert.Equal(int.MaxValue, item.cchTextMax);
        Assert.Empty(new string(item.pszText));
        Assert.Equal(-1, item.iImage);

        // Get item 1.
        Assert.Equal(1, (int)PInvokeCore.SendMessage(owner, PInvoke.TCM_GETITEMW, 1, ref item));
        Assert.Equal((TAB_CONTROL_ITEM_STATE)0, item.dwState);
        Assert.Equal(IntPtr.Zero, (nint)item.lParam);
        Assert.Equal(int.MaxValue, item.cchTextMax);
        Assert.Equal(expectedText, new string(item.pszText));
        Assert.Equal(1, item.iImage);

        // Get item 2.
        Assert.Equal(1, (int)PInvokeCore.SendMessage(owner, PInvoke.TCM_GETITEMW, 2, ref item));
        Assert.Equal((TAB_CONTROL_ITEM_STATE)0, item.dwState);
        Assert.Equal(IntPtr.Zero, (nint)item.lParam);
        Assert.Equal(int.MaxValue, item.cchTextMax);
        Assert.Empty(new string(item.pszText));
        Assert.Equal(-1, item.iImage);
    }

    [WinFormsTheory]
    [InlineData("Text", "Text")]
    [InlineData("&&Text", "&&Text")]
    [InlineData("&", "&&")]
    [InlineData("&Text", "&&Text")]
    public unsafe void TabPageCollection_Add_GetItemsDesignModeWithHandle_Success(string text, string expectedText)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(null);
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(true);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        using TabControl owner = new()
        {
            Site = mockSite.Object
        };
        Assert.NotEqual(IntPtr.Zero, owner.Handle);

        using TabPage page1 = new();
        using TabPage page2 = new()
        {
            Text = text,
            ImageIndex = 1
        };
        using NullTextTabPage page3 = new();
        var collection = new TabControl.TabPageCollection(owner)
        {
            page1,
            page2,
            page3
        };
        Assert.Equal(3, (int)PInvokeCore.SendMessage(owner, PInvoke.TCM_GETITEMCOUNT));

        char* buffer = stackalloc char[256];
        TCITEMW item = default;
        item.cchTextMax = int.MaxValue;
        item.pszText = buffer;
        item.dwStateMask = (TAB_CONTROL_ITEM_STATE)uint.MaxValue;
        item.mask = (TCITEMHEADERA_MASK)uint.MaxValue;

        // Get item 0.
        Assert.Equal(1, (int)PInvokeCore.SendMessage(owner, PInvoke.TCM_GETITEMW, 0, ref item));
        Assert.Equal(TAB_CONTROL_ITEM_STATE.TCIS_BUTTONPRESSED, item.dwState);
        Assert.Equal(IntPtr.Zero, (nint)item.lParam);
        Assert.Equal(int.MaxValue, item.cchTextMax);
        Assert.Empty(new string(item.pszText));
        Assert.Equal(-1, item.iImage);

        // Get item 1.
        Assert.Equal(1, (int)PInvokeCore.SendMessage(owner, PInvoke.TCM_GETITEMW, 1, ref item));
        Assert.Equal((TAB_CONTROL_ITEM_STATE)0, item.dwState);
        Assert.Equal(IntPtr.Zero, (nint)item.lParam);
        Assert.Equal(int.MaxValue, item.cchTextMax);
        Assert.Equal(expectedText, new string(item.pszText));
        Assert.Equal(1, item.iImage);

        // Get item 2.
        Assert.Equal(1, (int)PInvokeCore.SendMessage(owner, PInvoke.TCM_GETITEMW, 2, ref item));
        Assert.Equal((TAB_CONTROL_ITEM_STATE)0, item.dwState);
        Assert.Equal(IntPtr.Zero, (nint)item.lParam);
        Assert.Equal(int.MaxValue, item.cchTextMax);
        Assert.Empty(new string(item.pszText));
        Assert.Equal(-1, item.iImage);
    }

    [WinFormsFact]
    public void TabPageCollection_Add_NullValue_ThrowsArgumentNullException()
    {
        using TabControl owner = new();
        var collection = new TabControl.TabPageCollection(owner);
        Assert.Throws<ArgumentNullException>("value", () => collection.Add((TabPage)null));
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void TabPageCollection_Add_InvokeString_Success(string text, string expectedText)
    {
        using TabControl owner = new();
        var collection = new TabControl.TabPageCollection(owner)
        {
            text
        };

        TabPage page = Assert.IsType<TabPage>(Assert.Single(collection));
        Assert.Empty(page.Name);
        Assert.Equal(expectedText, page.Text);
        Assert.Equal(-1, page.ImageIndex);
        Assert.Empty(page.ImageKey);
        Assert.Same(owner, page.Parent);
        Assert.False(page.Visible);
    }

    [WinFormsTheory]
    [InlineData(null, null, "", "")]
    [InlineData("", "", "", "")]
    [InlineData("name", "text", "name", "text")]
    public void TabPageCollection_Add_InvokeStringString_Success(string key, string text, string expectedName, string expectedText)
    {
        using TabControl owner = new();
        var collection = new TabControl.TabPageCollection(owner)
        {
            { key, text }
        };

        TabPage page = Assert.IsType<TabPage>(Assert.Single(collection));
        Assert.Equal(expectedName, page.Name);
        Assert.Equal(expectedText, page.Text);
        Assert.Equal(-1, page.ImageIndex);
        Assert.Empty(page.ImageKey);
        Assert.Same(owner, page.Parent);
        Assert.False(page.Visible);
    }

    [WinFormsTheory]
    [InlineData(null, null, -1, "", "")]
    [InlineData("", "", 0, "", "")]
    [InlineData("name", "text", 1, "name", "text")]
    public void TabPageCollection_Add_InvokeStringStringInt_Success(string key, string text, int imageIndex, string expectedName, string expectedText)
    {
        using TabControl owner = new();
        var collection = new TabControl.TabPageCollection(owner)
        {
            { key, text, imageIndex }
        };

        TabPage page = Assert.IsType<TabPage>(Assert.Single(collection));
        Assert.Equal(expectedName, page.Name);
        Assert.Equal(expectedText, page.Text);
        Assert.Equal(imageIndex, page.ImageIndex);
        Assert.Empty(page.ImageKey);
        Assert.Same(owner, page.Parent);
        Assert.False(page.Visible);
    }

    [WinFormsTheory]
    [InlineData(null, null, null, "", "", "")]
    [InlineData("", "", "", "", "", "")]
    [InlineData("name", "text", "imageKey", "name", "text", "imageKey")]
    public void TabPageCollection_Add_InvokeStringStringString_Success(string key, string text, string imageKey, string expectedName, string expectedText, string expectedImageKey)
    {
        using TabControl owner = new();
        var collection = new TabControl.TabPageCollection(owner)
        {
            { key, text, imageKey }
        };

        TabPage page = Assert.IsType<TabPage>(Assert.Single(collection));
        Assert.Equal(expectedName, page.Name);
        Assert.Equal(expectedText, page.Text);
        Assert.Equal(-1, page.ImageIndex);
        Assert.Equal(expectedImageKey, page.ImageKey);
        Assert.Same(owner, page.Parent);
        Assert.False(page.Visible);
    }

    [WinFormsTheory]
    [MemberData(nameof(Add_TestData))]
    public void TabPageCollection_IListAdd_InvokeValueWithoutHandleOwnerWithoutHandle_Success(TabAppearance appearance)
    {
        using TabControl owner = new()
        {
            Appearance = appearance,
            Bounds = new Rectangle(0, 0, 400, 300)
        };
        using TabPage value1 = new();
        using TabPage value2 = new();
        var collection = new TabControl.TabPageCollection(owner);
        IList iList = collection;

        int layoutCallCount1 = 0;
        value1.Layout += (sender, e) => layoutCallCount1++;
        int layoutCallCount2 = 0;
        value2.Layout += (sender, e) => layoutCallCount2++;
        int parentLayoutCallCount = 0;
        List<LayoutEventArgs> events = [];
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(owner, sender);
            events.Add(e);
            parentLayoutCallCount++;
        }

        owner.Layout += parentHandler;

        try
        {
            // Add first.
            iList.Add(value1);
            Assert.Same(value1, Assert.Single(collection));
            Assert.Same(value1, Assert.Single(owner.TabPages));
            Assert.Same(owner, value1.Parent);
            Assert.False(value1.Visible);
            Assert.Equal(new Rectangle(0, 0, 200, 100), value1.Bounds);
            Assert.Null(value1.Site);
            Assert.Equal(-1, owner.SelectedIndex);
            Assert.Equal(0, layoutCallCount1);
            Assert.Equal(2, parentLayoutCallCount);
            Assert.Same(value1, events[0].AffectedControl);
            Assert.Equal("Parent", events[0].AffectedProperty);
            Assert.Same(value1, events[1].AffectedControl);
            Assert.Equal("Visible", events[1].AffectedProperty);
            Assert.False(value1.IsHandleCreated);
            Assert.False(owner.IsHandleCreated);

            // Add another.
            iList.Add(value2);
            Assert.Equal(new TabPage[] { value1, value2 }, collection.Cast<TabPage>());
            Assert.Equal(new TabPage[] { value1, value2 }, owner.TabPages.Cast<TabPage>());
            Assert.Same(owner, value1.Parent);
            Assert.False(value1.Visible);
            Assert.Equal(new Rectangle(0, 0, 200, 100), value1.Bounds);
            Assert.Null(value1.Site);
            Assert.Same(owner, value2.Parent);
            Assert.False(value2.Visible);
            Assert.Equal(new Rectangle(0, 0, 200, 100), value2.Bounds);
            Assert.Null(value2.Site);
            Assert.Equal(-1, owner.SelectedIndex);
            Assert.Equal(0, layoutCallCount1);
            Assert.Equal(0, layoutCallCount2);
            Assert.Equal(4, parentLayoutCallCount);
            Assert.Same(value1, events[0].AffectedControl);
            Assert.Equal("Parent", events[0].AffectedProperty);
            Assert.Same(value1, events[1].AffectedControl);
            Assert.Equal("Visible", events[1].AffectedProperty);
            Assert.Same(value2, events[2].AffectedControl);
            Assert.Equal("Parent", events[2].AffectedProperty);
            Assert.Same(value2, events[3].AffectedControl);
            Assert.Equal("Visible", events[3].AffectedProperty);
            Assert.False(value1.IsHandleCreated);
            Assert.False(value2.IsHandleCreated);
            Assert.False(owner.IsHandleCreated);

            // Add again.
            iList.Add(value1);
            Assert.Equal(new TabPage[] { value1, value2, value1 }, collection.Cast<TabPage>());
            Assert.Equal(new TabPage[] { value1, value2, value1 }, owner.TabPages.Cast<TabPage>());
            Assert.Same(owner, value1.Parent);
            Assert.False(value1.Visible);
            Assert.Same(owner, value2.Parent);
            Assert.Equal(new Rectangle(0, 0, 200, 100), value1.Bounds);
            Assert.Null(value1.Site);
            Assert.False(value2.Visible);
            Assert.Equal(new Rectangle(0, 0, 200, 100), value2.Bounds);
            Assert.Null(value2.Site);
            Assert.Equal(-1, owner.SelectedIndex);
            Assert.Equal(0, layoutCallCount1);
            Assert.Equal(5, parentLayoutCallCount);
            Assert.Same(value1, events[0].AffectedControl);
            Assert.Equal("Parent", events[0].AffectedProperty);
            Assert.Same(value1, events[1].AffectedControl);
            Assert.Equal("Visible", events[1].AffectedProperty);
            Assert.Same(value2, events[2].AffectedControl);
            Assert.Equal("Parent", events[2].AffectedProperty);
            Assert.Same(value2, events[3].AffectedControl);
            Assert.Equal("Visible", events[3].AffectedProperty);
            Assert.Same(value1, events[4].AffectedControl);
            Assert.Equal("ChildIndex", events[4].AffectedProperty);
            Assert.False(value1.IsHandleCreated);
            Assert.False(value2.IsHandleCreated);
            Assert.False(owner.IsHandleCreated);
        }
        finally
        {
            owner.Layout -= parentHandler;
        }
    }

    public static IEnumerable<object[]> IListAdd_InvalidValue_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new() };
        yield return new object[] { new Control() };
    }

    [WinFormsTheory]
    [MemberData(nameof(IListAdd_InvalidValue_TestData))]
    public void TabPageCollection_IListAdd_InvalidValue_ThrowsArgumentException(object value)
    {
        using TabControl owner = new();
        var collection = new TabControl.TabPageCollection(owner);
        IList iList = collection;
        Assert.Throws<ArgumentException>(() => iList.Add(value));
    }

    [WinFormsFact]
    public void TabPageCollection_AddRange_Invoke_Success()
    {
        using TabControl owner = new();
        using TabPage child1 = new();
        using TabPage child2 = new();
        using TabPage child3 = new();
        var collection = new TabControl.TabPageCollection(owner);
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e) => parentLayoutCallCount++;
        owner.Layout += parentHandler;
        int controlAddedCallCount = 0;
        owner.ControlAdded += (sender, e) => controlAddedCallCount++;

        try
        {
            collection.AddRange([child1, child2, child3]);
            Assert.Equal(new TabPage[] { child1, child2, child3 }, collection.Cast<TabPage>());
            Assert.Same(owner, child1.Parent);
            Assert.Same(owner, child2.Parent);
            Assert.Same(owner, child3.Parent);
            Assert.Equal(6, parentLayoutCallCount);
            Assert.Equal(3, controlAddedCallCount);
            Assert.False(owner.IsHandleCreated);
            Assert.False(child1.IsHandleCreated);
            Assert.False(child2.IsHandleCreated);
            Assert.False(child3.IsHandleCreated);

            collection.AddRange([child1, child2, child3]);
            Assert.Equal(new TabPage[] { child1, child2, child3, child1, child2, child3 }, collection.Cast<TabPage>());
            Assert.Same(owner, child1.Parent);
            Assert.Same(owner, child2.Parent);
            Assert.Same(owner, child3.Parent);
            Assert.Equal(9, parentLayoutCallCount);
            Assert.Equal(3, controlAddedCallCount);
            Assert.False(owner.IsHandleCreated);
            Assert.False(child1.IsHandleCreated);
            Assert.False(child2.IsHandleCreated);
            Assert.False(child3.IsHandleCreated);

            // Add empty.
            collection.AddRange(Array.Empty<TabPage>());
            Assert.Equal(new TabPage[] { child1, child2, child3, child1, child2, child3 }, collection.Cast<TabPage>());
            Assert.Same(owner, child1.Parent);
            Assert.Same(owner, child2.Parent);
            Assert.Same(owner, child3.Parent);
            Assert.Equal(9, parentLayoutCallCount);
            Assert.Equal(3, controlAddedCallCount);
            Assert.False(owner.IsHandleCreated);
            Assert.False(child1.IsHandleCreated);
            Assert.False(child2.IsHandleCreated);
            Assert.False(child3.IsHandleCreated);
        }
        finally
        {
            owner.Layout -= parentHandler;
        }
    }

    [WinFormsFact]
    public void TabPageCollection_AddRange_NullPages_ThrowsArgumentNullException()
    {
        using TabControl owner = new();
        var collection = new TabControl.TabPageCollection(owner);
        Assert.Throws<ArgumentNullException>("pages", () => collection.AddRange(null));
    }

    [WinFormsFact]
    public void TabPageCollection_Ctor_TabControl()
    {
        using TabControl owner = new();
        var collection = new TabControl.TabPageCollection(owner);
        IList iList = collection;
        Assert.Empty(collection);
        Assert.False(iList.IsFixedSize);
        Assert.False(collection.IsReadOnly);
        Assert.False(iList.IsReadOnly);
        Assert.False(iList.IsSynchronized);
        Assert.Same(collection, iList.SyncRoot);
    }

    [WinFormsFact]
    public void TabPageCollection_Ctor_NullOwner_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("owner", () => new TabControl.TabPageCollection(null));
    }

    [WinFormsFact]
    public void TabPageCollection_CopyTo_InvokeEmpty_Success()
    {
        using TabControl owner = new();
        var collection = new TabControl.TabPageCollection(owner);
        IList iList = collection;
        iList.CopyTo(null, 0);

        object[] array = [1, 2, 3, 4];
        iList.CopyTo(array, 1);
        Assert.Equal([1, 2, 3, 4], array);
    }

    [WinFormsFact]
    public void TabPageCollection_CopyTo_InvokeNotEmpty_Success()
    {
        using TabControl owner = new();
        using TabPage child1 = new();
        using TabPage child2 = new();
        var collection = new TabControl.TabPageCollection(owner);
        IList iList = collection;
        iList.Add(child1);
        iList.Add(child2);

        object[] array = [1, 2, 3, 4];
        iList.CopyTo(array, 1);
        Assert.Equal([1, child1, child2, 4], array);
    }

    [WinFormsFact]
    public void TabPageCollection_CopyTo_NullArray_ThrowsArgumentNullException()
    {
        using TabControl owner = new();
        using TabPage child = new();
        var collection = new TabControl.TabPageCollection(owner);
        IList iList = collection;
        iList.Add(child);

        Assert.Throws<ArgumentNullException>("destinationArray", () => iList.CopyTo(null, 0));
    }

    [WinFormsFact]
    public void TabPageCollection_CopyTo_InvalidIndex_ThrowsArgumentOutOfRangeException()
    {
        using TabControl owner = new();
        using TabPage child = new();
        var collection = new TabControl.TabPageCollection(owner);
        IList iList = collection;
        iList.Add(child);

        object[] array = [1, 2];
        Assert.Throws<ArgumentOutOfRangeException>("destinationIndex", () => iList.CopyTo(array, -1));
    }

    [WinFormsFact]
    public void TabPageCollection_CopyTo_InvokeNullGetItemsEmpty_Success()
    {
        using NullGetItemsTabControl owner = new();
        var collection = new TabControl.TabPageCollection(owner);
        IList iList = collection;
        iList.CopyTo(null, 0);

        object[] array = [1, 2, 3, 4];
        iList.CopyTo(array, 1);
        Assert.Equal([1, 2, 3, 4], array);
    }

    [WinFormsFact]
    public void TabPageCollection_CopyTo_InvokeInvalidGetItemsEmpty_Success()
    {
        using InvalidGetItemsTabControl owner = new();
        var collection = new TabControl.TabPageCollection(owner);
        IList iList = collection;
        iList.CopyTo(null, 0);

        object[] array = [1, 2, 3, 4];
        iList.CopyTo(array, 1);
        Assert.Equal([1, 2, 3, 4], array);
    }

    [WinFormsFact]
    public void TabPageCollection_CopyTo_InvokeNullGetItemsNotEmpty_Success()
    {
        using NullGetItemsTabControl owner = new();
        using TabPage child1 = new();
        using TabPage child2 = new();
        var collection = new TabControl.TabPageCollection(owner);
        IList iList = collection;
        iList.Add(child1);
        iList.Add(child2);

        object[] array = [1, 2, 3, 4];
        Assert.Throws<ArgumentNullException>("sourceArray", () => iList.CopyTo(array, 1));
        Assert.Equal([1, 2, 3, 4], array);
    }

    [WinFormsFact]
    public void TabPageCollection_CopyTo_InvokeInvalidGetItemsNotEmpty_Success()
    {
        using InvalidGetItemsTabControl owner = new();
        using TabPage child1 = new();
        using TabPage child2 = new();
        var collection = new TabControl.TabPageCollection(owner);
        IList iList = collection;
        iList.Add(child1);
        iList.Add(child2);

        object[] array = [1, 2, 3, 4];
        Assert.Throws<InvalidCastException>(() => iList.CopyTo(array, 1));
        Assert.Equal([1, 2, 3, 4], array);
    }

    [WinFormsFact]
    public void TabPageCollection_Clear_InvokeEmpty_Success()
    {
        using TabControl owner = new();
        var collection = new TabControl.TabPageCollection(owner);
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e) => parentLayoutCallCount++;
        owner.Layout += parentHandler;
        int controlRemovedCallCount = 0;
        owner.ControlRemoved += (sender, e) => controlRemovedCallCount++;

        try
        {
            collection.Clear();
            Assert.Empty(collection);
            Assert.Empty(owner.TabPages);
            Assert.Empty(owner.Controls);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.Equal(0, controlRemovedCallCount);
            Assert.False(owner.IsHandleCreated);

            // Clear again.
            collection.Clear();
            Assert.Empty(collection);
            Assert.Empty(owner.TabPages);
            Assert.Empty(owner.Controls);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.Equal(0, controlRemovedCallCount);
            Assert.False(owner.IsHandleCreated);
        }
        finally
        {
            owner.Layout -= parentHandler;
        }
    }

    [WinFormsFact]
    public void TabPageCollection_Clear_InvokeNotEmpty_Success()
    {
        using TabControl owner = new();
        using TabPage child1 = new();
        using TabPage child2 = new();
        using TabPage child3 = new();
        var collection = new TabControl.TabPageCollection(owner)
        {
            child1,
            child2,
            child3
        };
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(owner, sender);
            Assert.Same(child3, e.AffectedControl);
            Assert.Equal("Parent", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        owner.Layout += parentHandler;
        int controlRemovedCallCount = 0;
        owner.ControlRemoved += (sender, e) => controlRemovedCallCount++;

        try
        {
            collection.Clear();
            Assert.Empty(collection);
            Assert.Empty(owner.TabPages);
            Assert.Empty(owner.Controls);
            Assert.Null(child1.Parent);
            Assert.Null(child2.Parent);
            Assert.Null(child3.Parent);
            Assert.Equal(1, parentLayoutCallCount);
            Assert.Equal(3, controlRemovedCallCount);
            Assert.False(owner.IsHandleCreated);
            Assert.False(child1.IsHandleCreated);
            Assert.False(child2.IsHandleCreated);
            Assert.False(child3.IsHandleCreated);

            // Clear again.
            collection.Clear();
            Assert.Empty(collection);
            Assert.Empty(owner.TabPages);
            Assert.Empty(owner.Controls);
            Assert.Null(child1.Parent);
            Assert.Null(child2.Parent);
            Assert.Null(child3.Parent);
            Assert.Equal(1, parentLayoutCallCount);
            Assert.Equal(3, controlRemovedCallCount);
            Assert.False(owner.IsHandleCreated);
            Assert.False(child1.IsHandleCreated);
            Assert.False(child2.IsHandleCreated);
            Assert.False(child3.IsHandleCreated);
        }
        finally
        {
            owner.Layout -= parentHandler;
        }
    }

    [WinFormsFact]
    public void TabPageCollection_Clear_InvokeEmptyWithHandle_Success()
    {
        using TabControl owner = new();
        var collection = new TabControl.TabPageCollection(owner);

        int controlRemovedCallCount = 0;
        owner.ControlRemoved += (sender, e) => controlRemovedCallCount++;
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int parentInvalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => parentInvalidatedCallCount++;
        int parentStyleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
        int parentCreatedCallCount = 0;
        owner.HandleCreated += (sender, e) => parentCreatedCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e) => parentLayoutCallCount++;
        owner.Layout += parentHandler;

        try
        {
            collection.Clear();
            Assert.Empty(collection);
            Assert.Empty(owner.TabPages);
            Assert.Empty(owner.Controls);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.Equal(0, controlRemovedCallCount);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);

            // Clear again.
            collection.Clear();
            Assert.Empty(collection);
            Assert.Empty(owner.TabPages);
            Assert.Empty(owner.Controls);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.Equal(0, controlRemovedCallCount);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);
        }
        finally
        {
            owner.Layout -= parentHandler;
        }
    }

    [WinFormsFact]
    public void TabPageCollection_Clear_InvokeNotEmptyWithHandle_Success()
    {
        using TabControl owner = new();
        using TabPage child1 = new();
        using TabPage child2 = new();
        using TabPage child3 = new();
        var collection = new TabControl.TabPageCollection(owner)
        {
            child1,
            child2,
            child3
        };

        int controlRemovedCallCount = 0;
        owner.ControlRemoved += (sender, e) => controlRemovedCallCount++;
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int parentInvalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => parentInvalidatedCallCount++;
        int parentStyleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
        int parentCreatedCallCount = 0;
        owner.HandleCreated += (sender, e) => parentCreatedCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(owner, sender);
            Assert.Same(child3, e.AffectedControl);
            Assert.Equal("Parent", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        owner.Layout += parentHandler;

        try
        {
            collection.Clear();
            Assert.Empty(collection);
            Assert.Empty(owner.TabPages);
            Assert.Empty(owner.Controls);
            Assert.Null(child1.Parent);
            Assert.Null(child2.Parent);
            Assert.Null(child3.Parent);
            Assert.Equal(1, parentLayoutCallCount);
            Assert.Equal(3, controlRemovedCallCount);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);
            Assert.True(child1.IsHandleCreated);
            Assert.False(child2.IsHandleCreated);
            Assert.False(child3.IsHandleCreated);

            // Clear again.
            collection.Clear();
            Assert.Empty(collection);
            Assert.Empty(owner.TabPages);
            Assert.Empty(owner.Controls);
            Assert.Null(child1.Parent);
            Assert.Null(child2.Parent);
            Assert.Null(child3.Parent);
            Assert.Equal(1, parentLayoutCallCount);
            Assert.Equal(3, controlRemovedCallCount);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);
            Assert.True(child1.IsHandleCreated);
            Assert.False(child2.IsHandleCreated);
            Assert.False(child3.IsHandleCreated);
        }
        finally
        {
            owner.Layout -= parentHandler;
        }
    }

    [WinFormsFact]
    public void TabPageCollection_Clear_GetItemsWithHandle_Success()
    {
        using TabControl owner = new();
        using TabPage child1 = new();
        using TabPage child2 = new();
        using TabPage child3 = new();
        var collection = new TabControl.TabPageCollection(owner)
        {
            child1,
            child2,
            child3
        };

        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        collection.Clear();
        Assert.Equal(0, (int)PInvokeCore.SendMessage(owner, PInvoke.TCM_GETITEMCOUNT));
    }

    [WinFormsFact]
    public void TabPageCollection_Contains_Invoke_ReturnsExpected()
    {
        using TabControl owner = new();
        using TabPage child1 = new();
        using TabPage child2 = new();
        var collection = new TabControl.TabPageCollection(owner)
        {
            child1,
            child2
        };

        Assert.True(collection.Contains(child1));
        Assert.True(collection.Contains(child2));
        Assert.False(collection.Contains(new TabPage()));
    }

    [WinFormsFact]
    public void TabPageCollection_Contains_InvokeEmpty_ReturnsExpected()
    {
        using TabControl owner = new();
        var collection = new TabControl.TabPageCollection(owner);
        Assert.False(collection.Contains(new TabPage()));
    }

    [WinFormsFact]
    public void TabPageCollection_Contains_NullPage_ThrowsArgumentNullException()
    {
        using TabControl owner = new();
        var collection = new TabControl.TabPageCollection(owner);
        Assert.Throws<ArgumentNullException>("page", () => collection.Contains(null));
    }

    [WinFormsFact]
    public void TabPageCollection_IListContains_InvokeNotEmpty_ReturnsExpected()
    {
        using TabControl owner = new();
        using TabPage child1 = new();
        using TabPage child2 = new();
        var collection = new TabControl.TabPageCollection(owner);
        IList iList = collection;
        iList.Add(child1);
        iList.Add(child2);

        Assert.True(iList.Contains(child1));
        Assert.True(iList.Contains(child2));
        Assert.False(iList.Contains(new TabPage()));
        Assert.False(iList.Contains(new Control()));
        Assert.False(iList.Contains(new object()));
        Assert.False(iList.Contains(null));
    }

    [WinFormsFact]
    public void TabPageCollection_IListContains_InvokeEmpty_ReturnsExpected()
    {
        using TabControl owner = new();
        var collection = new TabControl.TabPageCollection(owner);
        IList iList = collection;
        Assert.False(iList.Contains(new TabPage()));
        Assert.False(iList.Contains(new Control()));
        Assert.False(iList.Contains(new object()));
        Assert.False(iList.Contains(null));
    }

    [WinFormsTheory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("name1", true)]
    [InlineData("NAME1", true)]
    [InlineData("name2", true)]
    [InlineData("NoSuchName", false)]
    [InlineData("abcd", false)]
    [InlineData("abcde", false)]
    [InlineData("abcdef", false)]
    public void TabPageCollection_ContainsKey_Invoke_ReturnsExpected(string key, bool expected)
    {
        using TabControl owner = new();
        using TabPage child1 = new()
        {
            Name = "name1"
        };
        using TabPage child2 = new()
        {
            Name = "name2"
        };
        using TabPage child3 = new()
        {
            Name = "name2"
        };
        var collection = new TabControl.TabPageCollection(owner)
        {
            child1,
            child2,
            child3
        };

        Assert.Equal(expected, collection.ContainsKey(key));

        // Call again.
        Assert.Equal(expected, collection.ContainsKey(key));
        Assert.False(collection.ContainsKey("NoSuchKey"));
    }

    [WinFormsTheory]
    [StringWithNullData]
    public void TabPageCollection_ContainsKey_InvokeEmpty_ReturnsExpected(string key)
    {
        using TabControl owner = new();
        var collection = new TabControl.TabPageCollection(owner);

        Assert.False(collection.ContainsKey(key));

        // Call again.
        Assert.False(collection.ContainsKey(key));
        Assert.False(collection.ContainsKey("NoSuchKey"));
    }

    [WinFormsFact]
    public void TabPageCollection_GetEnumerator_InvokeEmpty_ReturnsExpected()
    {
        using TabControl owner = new();
        var collection = new TabControl.TabPageCollection(owner);
        IEnumerator enumerator = collection.GetEnumerator();
        for (int i = 0; i < 2; i++)
        {
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);

            Assert.False(enumerator.MoveNext());
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);

            // Call again.
            Assert.False(enumerator.MoveNext());
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);

            enumerator.Reset();
        }
    }

    [WinFormsFact]
    public void TabPageCollection_GetEnumerator_InvokeNotEmpty_ReturnsExpected()
    {
        using TabControl owner = new();
        using TabPage child1 = new();
        using TabPage child2 = new();
        using TabPage child3 = new();
        var collection = new TabControl.TabPageCollection(owner)
        {
            child1,
            child2,
            child3
        };

        IEnumerator enumerator = collection.GetEnumerator();
        for (int i = 0; i < 2; i++)
        {
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);

            Assert.True(enumerator.MoveNext());
            Assert.Same(child1, enumerator.Current);

            Assert.True(enumerator.MoveNext());
            Assert.Same(child2, enumerator.Current);

            Assert.True(enumerator.MoveNext());
            Assert.Same(child3, enumerator.Current);

            Assert.False(enumerator.MoveNext());
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);

            // Call again.
            Assert.False(enumerator.MoveNext());
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);

            enumerator.Reset();
        }
    }

    [WinFormsFact]
    public void TabPageCollection_GetEnumerator_AddDuringEnumeration_ReturnsExpected()
    {
        using TabControl owner = new();
        using TabPage child = new();
        var collection = new TabControl.TabPageCollection(owner);
        IEnumerator enumerator = collection.GetEnumerator();
        collection.Add(child);
        for (int i = 0; i < 2; i++)
        {
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);

            Assert.False(enumerator.MoveNext());
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);

            // Call again.
            Assert.False(enumerator.MoveNext());
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);

            enumerator.Reset();
        }
    }

    [WinFormsFact]
    public void TabPageCollection_GetEnumerator_InvokeRemoveBeforeEnumeration_ReturnsExpected()
    {
        using TabControl owner = new();
        using TabPage child1 = new();
        using TabPage child2 = new();
        using TabPage child3 = new();
        var collection = new TabControl.TabPageCollection(owner)
        {
            child1,
            child2,
            child3
        };

        IEnumerator enumerator = collection.GetEnumerator();
        collection.Remove(child1);

        Assert.Throws<InvalidOperationException>(() => enumerator.Current);

        Assert.True(enumerator.MoveNext());
        Assert.Same(child1, enumerator.Current);

        Assert.True(enumerator.MoveNext());
        Assert.Same(child2, enumerator.Current);

        Assert.True(enumerator.MoveNext());
        Assert.Same(child3, enumerator.Current);

        Assert.False(enumerator.MoveNext());
        Assert.Throws<InvalidOperationException>(() => enumerator.Current);

        // Call again.
        Assert.False(enumerator.MoveNext());
        Assert.Throws<InvalidOperationException>(() => enumerator.Current);
    }

    [WinFormsFact]
    public void TabPageCollection_GetEnumerator_InvokeRemoveAtEndOfEnumeration_ReturnsExpected()
    {
        using TabControl owner = new();
        using TabPage child1 = new();
        using TabPage child2 = new();
        using TabPage child3 = new();
        var collection = new TabControl.TabPageCollection(owner)
        {
            child1,
            child2,
            child3
        };

        IEnumerator enumerator = collection.GetEnumerator();
        Assert.Throws<InvalidOperationException>(() => enumerator.Current);

        Assert.True(enumerator.MoveNext());
        Assert.Same(child1, enumerator.Current);

        Assert.True(enumerator.MoveNext());
        Assert.Same(child2, enumerator.Current);

        Assert.True(enumerator.MoveNext());
        Assert.Same(child3, enumerator.Current);

        collection.Remove(child1);
        Assert.Same(child3, enumerator.Current);

        Assert.False(enumerator.MoveNext());
        Assert.Throws<InvalidOperationException>(() => enumerator.Current);

        // Call again.
        Assert.False(enumerator.MoveNext());
        Assert.Throws<InvalidOperationException>(() => enumerator.Current);

        collection.Add(child1);
        Assert.Throws<InvalidOperationException>(() => enumerator.Current);
    }

    [WinFormsFact]
    public void TabPageCollection_GetEnumerator_InvokeNullGetItems_ReturnsExpected()
    {
        using NullGetItemsTabControl owner = new();
        var collection = new TabControl.TabPageCollection(owner);
        IEnumerator enumerator = collection.GetEnumerator();
        for (int i = 0; i < 2; i++)
        {
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);

            Assert.False(enumerator.MoveNext());
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);

            // Call again.
            Assert.False(enumerator.MoveNext());
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);

            enumerator.Reset();
        }
    }

    [WinFormsFact]
    public void TabPageCollection_GetEnumerator_InvokeInvalidGetItems_ReturnsExpected()
    {
        using InvalidGetItemsTabControl owner = new();
        var collection = new TabControl.TabPageCollection(owner);
        Assert.Throws<InvalidCastException>(collection.GetEnumerator);
    }

    [WinFormsFact]
    public void TabPageCollection_IndexOf_InvokeNotEmpty_ReturnsExpected()
    {
        using TabControl owner = new();
        using TabPage child1 = new();
        using TabPage child2 = new();
        var collection = new TabControl.TabPageCollection(owner)
        {
            child1,
            child2
        };

        Assert.Equal(0, collection.IndexOf(child1));
        Assert.Equal(1, collection.IndexOf(child2));
        Assert.Equal(-1, collection.IndexOf(new TabPage()));
    }

    [WinFormsFact]
    public void TabPageCollection_IndexOf_InvokeEmpty_ReturnsExpected()
    {
        using TabControl owner = new();
        var collection = new TabControl.TabPageCollection(owner);
        Assert.Equal(-1, collection.IndexOf(new TabPage()));
    }

    [WinFormsFact]
    public void TabPageCollection_IndexOf_NullPage_ThrowsArgumentNullException()
    {
        using TabControl owner = new();
        var collection = new TabControl.TabPageCollection(owner);
        Assert.Throws<ArgumentNullException>("page", () => collection.IndexOf(null));
    }

    [WinFormsFact]
    public void TabPageCollection_IListIndexOf_InvokeNotEmpty_ReturnsExpected()
    {
        using TabControl owner = new();
        using TabPage child1 = new();
        using TabPage child2 = new();
        var collection = new TabControl.TabPageCollection(owner);
        IList iList = collection;
        iList.Add(child1);
        iList.Add(child2);

        Assert.Equal(0, iList.IndexOf(child1));
        Assert.Equal(1, iList.IndexOf(child2));
        Assert.Equal(-1, iList.IndexOf(new TabPage()));
        Assert.Equal(-1, iList.IndexOf(new Control()));
        Assert.Equal(-1, iList.IndexOf(new object()));
        Assert.Equal(-1, iList.IndexOf(null));
    }

    [WinFormsFact]
    public void TabPageCollection_IListIndexOf_InvokeEmpty_ReturnsExpected()
    {
        using TabControl owner = new();
        var collection = new TabControl.TabPageCollection(owner);
        IList iList = collection;
        Assert.Equal(-1, iList.IndexOf(new TabPage()));
        Assert.Equal(-1, iList.IndexOf(new Control()));
        Assert.Equal(-1, iList.IndexOf(new object()));
        Assert.Equal(-1, iList.IndexOf(null));
    }

    [WinFormsTheory]
    [InlineData(null, -1)]
    [InlineData("", -1)]
    [InlineData("name1", 0)]
    [InlineData("NAME1", 0)]
    [InlineData("name2", 1)]
    [InlineData("NoSuchName", -1)]
    [InlineData("abcd", -1)]
    [InlineData("abcde", -1)]
    [InlineData("abcdef", -1)]
    public void TabPageCollection_IndexOfKey_InvokeNotEmpty_ReturnsExpected(string key, int expected)
    {
        using TabControl owner = new();
        using TabPage child1 = new()
        {
            Name = "name1"
        };
        using TabPage child2 = new()
        {
            Name = "name2"
        };
        using TabPage child3 = new()
        {
            Name = "name2"
        };
        var collection = new TabControl.TabPageCollection(owner)
        {
            child1,
            child2,
            child3
        };

        Assert.Equal(expected, collection.IndexOfKey(key));

        // Call again.
        Assert.Equal(expected, collection.IndexOfKey(key));
        Assert.Equal(-1, collection.IndexOfKey("NoSuchKey"));
    }

    [WinFormsTheory]
    [StringWithNullData]
    public void TabPageCollection_IndexOfKey_InvokeEmpty_ReturnsExpected(string key)
    {
        using TabControl owner = new();
        var collection = new TabControl.TabPageCollection(owner);

        Assert.Equal(-1, collection.IndexOfKey(key));

        // Call again.
        Assert.Equal(-1, collection.IndexOfKey(key));
        Assert.Equal(-1, collection.IndexOfKey("NoSuchKey"));
    }

    [WinFormsTheory]
    [MemberData(nameof(Add_TestData))]
    public void TabPageCollection_Insert_InvokeValueWithoutHandleOwnerWithoutHandle_Success(TabAppearance appearance)
    {
        using TabControl owner = new()
        {
            Appearance = appearance,
            Bounds = new Rectangle(0, 0, 400, 300)
        };
        using TabPage value1 = new();
        using TabPage value2 = new();
        var collection = new TabControl.TabPageCollection(owner);

        int layoutCallCount1 = 0;
        value1.Layout += (sender, e) => layoutCallCount1++;
        int layoutCallCount2 = 0;
        value2.Layout += (sender, e) => layoutCallCount2++;
        int parentLayoutCallCount = 0;
        List<LayoutEventArgs> events = [];
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(owner, sender);
            events.Add(e);
            parentLayoutCallCount++;
        }

        owner.Layout += parentHandler;

        try
        {
            // Add first.
            collection.Insert(0, value1);
            Assert.Same(value1, Assert.Single(collection));
            Assert.Same(value1, Assert.Single(owner.TabPages));
            Assert.Same(value1, Assert.Single(owner.Controls));
            Assert.Same(owner, value1.Parent);
            Assert.False(value1.Visible);
            Assert.Equal(new Rectangle(0, 0, 200, 100), value1.Bounds);
            Assert.Null(value1.Site);
            Assert.Equal(-1, owner.SelectedIndex);
            Assert.Equal(0, layoutCallCount1);
            Assert.Equal(2, parentLayoutCallCount);
            Assert.Same(value1, events[0].AffectedControl);
            Assert.Equal("Parent", events[0].AffectedProperty);
            Assert.Same(value1, events[1].AffectedControl);
            Assert.Equal("Visible", events[1].AffectedProperty);
            Assert.False(value1.IsHandleCreated);
            Assert.False(owner.IsHandleCreated);

            // Add another.
            collection.Insert(0, value2);
            Assert.Equal(new TabPage[] { value2, value1 }, collection.Cast<TabPage>());
            Assert.Equal(new TabPage[] { value2, value1 }, owner.TabPages.Cast<TabPage>());
            Assert.Equal(new Control[] { value2, value1 }, owner.Controls.Cast<Control>());
            Assert.Same(owner, value1.Parent);
            Assert.False(value1.Visible);
            Assert.Equal(new Rectangle(0, 0, 200, 100), value1.Bounds);
            Assert.Null(value1.Site);
            Assert.Same(owner, value2.Parent);
            Assert.False(value2.Visible);
            Assert.Equal(new Rectangle(0, 0, 200, 100), value2.Bounds);
            Assert.Null(value2.Site);
            Assert.Equal(-1, owner.SelectedIndex);
            Assert.Equal(0, layoutCallCount1);
            Assert.Equal(0, layoutCallCount2);
            Assert.Equal(5, parentLayoutCallCount);
            Assert.Same(value1, events[0].AffectedControl);
            Assert.Equal("Parent", events[0].AffectedProperty);
            Assert.Same(value1, events[1].AffectedControl);
            Assert.Equal("Visible", events[1].AffectedProperty);
            Assert.Same(value2, events[2].AffectedControl);
            Assert.Equal("Parent", events[2].AffectedProperty);
            Assert.Same(value2, events[3].AffectedControl);
            Assert.Equal("Visible", events[3].AffectedProperty);
            Assert.Same(value2, events[4].AffectedControl);
            Assert.Equal("ChildIndex", events[4].AffectedProperty);
            Assert.False(value1.IsHandleCreated);
            Assert.False(value2.IsHandleCreated);
            Assert.False(owner.IsHandleCreated);

            // Add again.
            collection.Insert(2, value1);
            Assert.Equal(new TabPage[] { value2, value1, value1 }, collection.Cast<TabPage>());
            Assert.Equal(new TabPage[] { value2, value1, value1 }, owner.TabPages.Cast<TabPage>());
            Assert.Equal(new Control[] { value2, value1 }, owner.Controls.Cast<Control>());
            Assert.Same(owner, value1.Parent);
            Assert.False(value1.Visible);
            Assert.Same(owner, value2.Parent);
            Assert.Equal(new Rectangle(0, 0, 200, 100), value1.Bounds);
            Assert.Null(value1.Site);
            Assert.False(value2.Visible);
            Assert.Equal(new Rectangle(0, 0, 200, 100), value2.Bounds);
            Assert.Null(value2.Site);
            Assert.Equal(-1, owner.SelectedIndex);
            Assert.Equal(0, layoutCallCount1);
            Assert.Equal(7, parentLayoutCallCount);
            Assert.Same(value1, events[0].AffectedControl);
            Assert.Equal("Parent", events[0].AffectedProperty);
            Assert.Same(value1, events[1].AffectedControl);
            Assert.Equal("Visible", events[1].AffectedProperty);
            Assert.Same(value2, events[2].AffectedControl);
            Assert.Equal("Parent", events[2].AffectedProperty);
            Assert.Same(value2, events[3].AffectedControl);
            Assert.Equal("Visible", events[3].AffectedProperty);
            Assert.Same(value2, events[4].AffectedControl);
            Assert.Equal("ChildIndex", events[4].AffectedProperty);
            Assert.Same(value1, events[5].AffectedControl);
            Assert.Equal("ChildIndex", events[5].AffectedProperty);
            Assert.Same(value1, events[6].AffectedControl);
            Assert.Equal("ChildIndex", events[6].AffectedProperty);
            Assert.False(value1.IsHandleCreated);
            Assert.False(value2.IsHandleCreated);
            Assert.False(owner.IsHandleCreated);
        }
        finally
        {
            owner.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Add_WithHandle_TestData))]
    public void TabPageCollection_Insert_InvokeValueWithoutHandleOwnerWithHandle_Success(TabAppearance appearance, Size itemSize, int expectedParentInvalidatedCallCount)
    {
        using TabControl owner = new()
        {
            Appearance = appearance,
            ItemSize = itemSize,
            Bounds = new Rectangle(0, 0, 400, 300)
        };
        using TabPage value1 = new();
        using TabPage value2 = new();
        var collection = new TabControl.TabPageCollection(owner);

        int layoutCallCount1 = 0;
        value1.Layout += (sender, e) => layoutCallCount1++;
        int layoutCallCount2 = 0;
        value2.Layout += (sender, e) => layoutCallCount2++;
        int parentLayoutCallCount = 0;
        List<LayoutEventArgs> events = [];
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(owner, sender);
            events.Add(e);
            parentLayoutCallCount++;
        }

        owner.Layout += parentHandler;
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int parentInvalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => parentInvalidatedCallCount++;
        int parentStyleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
        int parentCreatedCallCount = 0;
        owner.HandleCreated += (sender, e) => parentCreatedCallCount++;

        try
        {
            // Add first.
            collection.Insert(0, value1);
            Assert.Same(value1, Assert.Single(collection));
            Assert.Same(value1, Assert.Single(owner.TabPages));
            Assert.Same(value1, Assert.Single(owner.Controls));
            Assert.Same(owner, value1.Parent);
            Assert.True(value1.Visible);
            Assert.Equal(owner.DisplayRectangle, value1.Bounds);
            Assert.Null(value1.Site);
            Assert.Equal(0, owner.SelectedIndex);
            Assert.Equal(2, layoutCallCount1);
            Assert.Equal(5, parentLayoutCallCount);
            Assert.Same(value1, events[0].AffectedControl);
            Assert.Equal("Parent", events[0].AffectedProperty);
            Assert.Same(value1, events[1].AffectedControl);
            Assert.Equal("Visible", events[1].AffectedProperty);
            Assert.Same(value1, events[2].AffectedControl);
            Assert.Equal("Bounds", events[2].AffectedProperty);
            Assert.Same(value1, events[3].AffectedControl);
            Assert.Equal("Bounds", events[3].AffectedProperty);
            Assert.Same(value1, events[4].AffectedControl);
            Assert.Equal("Visible", events[4].AffectedProperty);
            Assert.True(value1.IsHandleCreated);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(expectedParentInvalidatedCallCount, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);

            // Add another.
            collection.Insert(0, value2);
            Assert.Equal(new TabPage[] { value2, value1 }, collection.Cast<TabPage>());
            Assert.Equal(new TabPage[] { value2, value1 }, owner.TabPages.Cast<TabPage>());
            Assert.Equal(new Control[] { value2, value1 }, owner.Controls.Cast<Control>());
            Assert.Same(owner, value1.Parent);
            Assert.True(value1.Visible);
            Assert.Equal(owner.DisplayRectangle, value1.Bounds);
            Assert.Null(value1.Site);
            Assert.Same(owner, value2.Parent);
            Assert.False(value2.Visible);
            Assert.Equal(owner.DisplayRectangle, value2.Bounds);
            Assert.Null(value2.Site);
            Assert.Equal(1, owner.SelectedIndex);
            Assert.Equal(2, layoutCallCount1);
            Assert.Equal(1, layoutCallCount2);
            Assert.Equal(10, parentLayoutCallCount);
            Assert.Same(value1, events[0].AffectedControl);
            Assert.Equal("Parent", events[0].AffectedProperty);
            Assert.Same(value1, events[1].AffectedControl);
            Assert.Equal("Visible", events[1].AffectedProperty);
            Assert.Same(value1, events[2].AffectedControl);
            Assert.Equal("Bounds", events[2].AffectedProperty);
            Assert.Same(value1, events[3].AffectedControl);
            Assert.Equal("Bounds", events[3].AffectedProperty);
            Assert.Same(value1, events[4].AffectedControl);
            Assert.Equal("Visible", events[4].AffectedProperty);
            Assert.Same(value2, events[5].AffectedControl);
            Assert.Equal("Parent", events[5].AffectedProperty);
            Assert.Same(value2, events[6].AffectedControl);
            Assert.Equal("Visible", events[6].AffectedProperty);
            Assert.Same(value2, events[7].AffectedControl);
            Assert.Equal("Bounds", events[7].AffectedProperty);
            Assert.Same(value2, events[8].AffectedControl);
            Assert.Equal("Bounds", events[8].AffectedProperty);
            Assert.Same(value2, events[9].AffectedControl);
            Assert.Equal("ChildIndex", events[9].AffectedProperty);
            Assert.True(value1.IsHandleCreated);
            Assert.False(value2.IsHandleCreated);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(expectedParentInvalidatedCallCount * 2, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);

            // Add again.
            collection.Insert(2, value1);
            Assert.Equal(new TabPage[] { value2, value1, value1 }, collection.Cast<TabPage>());
            Assert.Equal(new TabPage[] { value2, value1, value1 }, owner.TabPages.Cast<TabPage>());
            Assert.Equal(new Control[] { value2, value1 }, owner.Controls.Cast<Control>());
            Assert.Same(owner, value1.Parent);
            Assert.False(value1.Visible);
            Assert.Equal(owner.DisplayRectangle, value1.Bounds);
            Assert.Null(value1.Site);
            Assert.Same(owner, value2.Parent);
            Assert.False(value2.Visible);
            Assert.Equal(owner.DisplayRectangle, value2.Bounds);
            Assert.Null(value2.Site);
            Assert.Equal(1, owner.SelectedIndex);
            Assert.Equal(3, layoutCallCount1);
            Assert.Equal(1, layoutCallCount2);
            Assert.Equal(15, parentLayoutCallCount);
            Assert.Same(value1, events[0].AffectedControl);
            Assert.Equal("Parent", events[0].AffectedProperty);
            Assert.Same(value1, events[1].AffectedControl);
            Assert.Equal("Visible", events[1].AffectedProperty);
            Assert.Same(value1, events[2].AffectedControl);
            Assert.Equal("Bounds", events[2].AffectedProperty);
            Assert.Same(value1, events[3].AffectedControl);
            Assert.Equal("Bounds", events[3].AffectedProperty);
            Assert.Same(value1, events[4].AffectedControl);
            Assert.Equal("Visible", events[4].AffectedProperty);
            Assert.Same(value2, events[5].AffectedControl);
            Assert.Equal("Parent", events[5].AffectedProperty);
            Assert.Same(value2, events[6].AffectedControl);
            Assert.Equal("Visible", events[6].AffectedProperty);
            Assert.Same(value2, events[7].AffectedControl);
            Assert.Equal("Bounds", events[7].AffectedProperty);
            Assert.Same(value2, events[8].AffectedControl);
            Assert.Equal("Bounds", events[8].AffectedProperty);
            Assert.Same(value2, events[9].AffectedControl);
            Assert.Equal("ChildIndex", events[9].AffectedProperty);
            Assert.Same(value1, events[10].AffectedControl);
            Assert.Equal("ChildIndex", events[10].AffectedProperty);
            Assert.Same(value1, events[11].AffectedControl);
            Assert.Equal("Visible", events[11].AffectedProperty);
            Assert.Same(value1, events[12].AffectedControl);
            Assert.Equal("Visible", events[12].AffectedProperty);
            Assert.Same(value1, events[13].AffectedControl);
            Assert.Equal("Visible", events[13].AffectedProperty);
            Assert.Same(value1, events[14].AffectedControl);
            Assert.Equal("ChildIndex", events[14].AffectedProperty);
            Assert.True(value1.IsHandleCreated);
            Assert.False(value2.IsHandleCreated);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(expectedParentInvalidatedCallCount * 3, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);
        }
        finally
        {
            owner.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Add_TestData))]
    public void TabPageCollection_Insert_InvokeValueWithHandleOwnerWithoutHandle_Success(TabAppearance appearance)
    {
        using TabControl owner = new()
        {
            Appearance = appearance,
            Bounds = new Rectangle(0, 0, 400, 300)
        };
        using TabPage value1 = new();
        using TabPage value2 = new();
        var collection = new TabControl.TabPageCollection(owner);

        int layoutCallCount1 = 0;
        value1.Layout += (sender, e) => layoutCallCount1++;
        int layoutCallCount2 = 0;
        value2.Layout += (sender, e) => layoutCallCount2++;
        int parentLayoutCallCount = 0;
        List<LayoutEventArgs> events = [];
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(owner, sender);
            events.Add(e);
            parentLayoutCallCount++;
        }

        owner.Layout += parentHandler;
        Assert.NotEqual(IntPtr.Zero, value1.Handle);
        int invalidatedCallCount1 = 0;
        value1.Invalidated += (sender, e) => invalidatedCallCount1++;
        int styleChangedCallCount1 = 0;
        value1.StyleChanged += (sender, e) => styleChangedCallCount1++;
        int createdCallCount1 = 0;
        value1.HandleCreated += (sender, e) => createdCallCount1++;
        Assert.NotEqual(IntPtr.Zero, value2.Handle);
        int invalidatedCallCount2 = 0;
        value2.Invalidated += (sender, e) => invalidatedCallCount2++;
        int styleChangedCallCount2 = 0;
        value2.StyleChanged += (sender, e) => styleChangedCallCount2++;
        int createdCallCount2 = 0;
        value2.HandleCreated += (sender, e) => createdCallCount2++;

        try
        {
            // Add first.
            collection.Insert(0, value1);
            Assert.Same(value1, Assert.Single(collection));
            Assert.Same(value1, Assert.Single(owner.TabPages));
            Assert.Same(value1, Assert.Single(owner.Controls));
            Assert.Same(owner, value1.Parent);
            Assert.False(value1.Visible);
            Assert.Equal(new Rectangle(0, 0, 200, 100), value1.Bounds);
            Assert.Null(value1.Site);
            Assert.Equal(-1, owner.SelectedIndex);
            Assert.Equal(0, layoutCallCount1);
            Assert.Equal(2, parentLayoutCallCount);
            Assert.Same(value1, events[0].AffectedControl);
            Assert.Equal("Parent", events[0].AffectedProperty);
            Assert.Same(value1, events[1].AffectedControl);
            Assert.Equal("Visible", events[1].AffectedProperty);
            Assert.True(value1.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount1);
            Assert.Equal(0, styleChangedCallCount1);
            Assert.Equal(0, createdCallCount1);
            Assert.False(owner.IsHandleCreated);

            // Add another.
            collection.Insert(0, value2);
            Assert.Equal(new TabPage[] { value2, value1 }, collection.Cast<TabPage>());
            Assert.Equal(new TabPage[] { value2, value1 }, owner.TabPages.Cast<TabPage>());
            Assert.Equal(new Control[] { value2, value1 }, owner.Controls.Cast<Control>());
            Assert.Same(owner, value1.Parent);
            Assert.False(value1.Visible);
            Assert.Equal(new Rectangle(0, 0, 200, 100), value1.Bounds);
            Assert.Null(value1.Site);
            Assert.Same(owner, value2.Parent);
            Assert.False(value2.Visible);
            Assert.Equal(new Rectangle(0, 0, 200, 100), value2.Bounds);
            Assert.Null(value2.Site);
            Assert.Equal(-1, owner.SelectedIndex);
            Assert.Equal(0, layoutCallCount1);
            Assert.Equal(0, layoutCallCount2);
            Assert.Equal(5, parentLayoutCallCount);
            Assert.Same(value1, events[0].AffectedControl);
            Assert.Equal("Parent", events[0].AffectedProperty);
            Assert.Same(value1, events[1].AffectedControl);
            Assert.Equal("Visible", events[1].AffectedProperty);
            Assert.Same(value2, events[2].AffectedControl);
            Assert.Equal("Parent", events[2].AffectedProperty);
            Assert.Same(value2, events[3].AffectedControl);
            Assert.Equal("Visible", events[3].AffectedProperty);
            Assert.Same(value2, events[4].AffectedControl);
            Assert.Equal("ChildIndex", events[4].AffectedProperty);
            Assert.True(value1.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount1);
            Assert.Equal(0, styleChangedCallCount1);
            Assert.Equal(0, createdCallCount1);
            Assert.True(value2.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount2);
            Assert.Equal(0, styleChangedCallCount2);
            Assert.Equal(0, createdCallCount2);
            Assert.False(owner.IsHandleCreated);

            // Add again.
            collection.Insert(2, value1);
            Assert.Equal(new TabPage[] { value2, value1, value1 }, collection.Cast<TabPage>());
            Assert.Equal(new TabPage[] { value2, value1, value1 }, owner.TabPages.Cast<TabPage>());
            Assert.Equal(new Control[] { value2, value1 }, owner.Controls.Cast<Control>());
            Assert.Same(owner, value1.Parent);
            Assert.False(value1.Visible);
            Assert.Same(owner, value2.Parent);
            Assert.Equal(new Rectangle(0, 0, 200, 100), value1.Bounds);
            Assert.Null(value1.Site);
            Assert.False(value2.Visible);
            Assert.Equal(new Rectangle(0, 0, 200, 100), value2.Bounds);
            Assert.Null(value2.Site);
            Assert.Equal(-1, owner.SelectedIndex);
            Assert.Equal(0, layoutCallCount1);
            Assert.Equal(7, parentLayoutCallCount);
            Assert.Same(value1, events[0].AffectedControl);
            Assert.Equal("Parent", events[0].AffectedProperty);
            Assert.Same(value1, events[1].AffectedControl);
            Assert.Equal("Visible", events[1].AffectedProperty);
            Assert.Same(value2, events[2].AffectedControl);
            Assert.Equal("Parent", events[2].AffectedProperty);
            Assert.Same(value2, events[3].AffectedControl);
            Assert.Equal("Visible", events[3].AffectedProperty);
            Assert.Same(value2, events[4].AffectedControl);
            Assert.Equal("ChildIndex", events[4].AffectedProperty);
            Assert.Same(value1, events[5].AffectedControl);
            Assert.Equal("ChildIndex", events[5].AffectedProperty);
            Assert.Same(value1, events[6].AffectedControl);
            Assert.Equal("ChildIndex", events[6].AffectedProperty);
            Assert.True(value1.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount1);
            Assert.Equal(0, styleChangedCallCount1);
            Assert.Equal(0, createdCallCount1);
            Assert.True(value2.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount2);
            Assert.Equal(0, styleChangedCallCount2);
            Assert.Equal(0, createdCallCount2);
            Assert.False(owner.IsHandleCreated);
        }
        finally
        {
            owner.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Add_WithHandle_TestData))]
    public void TabPageCollection_Insert_InvokeValueWithHandleOwnerWithHandle_Success(TabAppearance appearance, Size itemSize, int expectedParentInvalidatedCallCount)
    {
        using TabControl owner = new()
        {
            Appearance = appearance,
            ItemSize = itemSize,
            Bounds = new Rectangle(0, 0, 400, 300)
        };
        using TabPage value1 = new();
        using TabPage value2 = new();
        var collection = new TabControl.TabPageCollection(owner);

        int layoutCallCount1 = 0;
        value1.Layout += (sender, e) => layoutCallCount1++;
        int layoutCallCount2 = 0;
        value2.Layout += (sender, e) => layoutCallCount2++;
        int parentLayoutCallCount = 0;
        List<LayoutEventArgs> events = [];
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(owner, sender);
            events.Add(e);
            parentLayoutCallCount++;
        }

        owner.Layout += parentHandler;
        Assert.NotEqual(IntPtr.Zero, value1.Handle);
        int invalidatedCallCount1 = 0;
        value1.Invalidated += (sender, e) => invalidatedCallCount1++;
        int styleChangedCallCount1 = 0;
        value1.StyleChanged += (sender, e) => styleChangedCallCount1++;
        int createdCallCount1 = 0;
        value1.HandleCreated += (sender, e) => createdCallCount1++;
        Assert.NotEqual(IntPtr.Zero, value2.Handle);
        int invalidatedCallCount2 = 0;
        value2.Invalidated += (sender, e) => invalidatedCallCount2++;
        int styleChangedCallCount2 = 0;
        value2.StyleChanged += (sender, e) => styleChangedCallCount2++;
        int createdCallCount2 = 0;
        value2.HandleCreated += (sender, e) => createdCallCount2++;
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int parentInvalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => parentInvalidatedCallCount++;
        int parentStyleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
        int parentCreatedCallCount = 0;
        owner.HandleCreated += (sender, e) => parentCreatedCallCount++;

        try
        {
            // Add first.
            collection.Insert(0, value1);
            Assert.Same(value1, Assert.Single(collection));
            Assert.Same(value1, Assert.Single(owner.TabPages));
            Assert.Same(value1, Assert.Single(owner.Controls));
            Assert.Same(owner, value1.Parent);
            Assert.True(value1.Visible);
            Assert.Equal(owner.DisplayRectangle, value1.Bounds);
            Assert.Null(value1.Site);
            Assert.Equal(0, owner.SelectedIndex);
            Assert.Equal(2, layoutCallCount1);
            Assert.Equal(5, parentLayoutCallCount);
            Assert.Same(value1, events[0].AffectedControl);
            Assert.Equal("Parent", events[0].AffectedProperty);
            Assert.Same(value1, events[1].AffectedControl);
            Assert.Equal("Visible", events[1].AffectedProperty);
            Assert.Same(value1, events[2].AffectedControl);
            Assert.Equal("Bounds", events[2].AffectedProperty);
            Assert.Same(value1, events[3].AffectedControl);
            Assert.Equal("Bounds", events[3].AffectedProperty);
            Assert.Same(value1, events[4].AffectedControl);
            Assert.Equal("Visible", events[4].AffectedProperty);
            Assert.True(value1.IsHandleCreated);
            Assert.Equal(1, invalidatedCallCount1);
            Assert.Equal(0, styleChangedCallCount1);
            Assert.Equal(0, createdCallCount1);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(expectedParentInvalidatedCallCount, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);

            // Add another.
            collection.Insert(0, value2);
            Assert.Equal(new TabPage[] { value2, value1 }, collection.Cast<TabPage>());
            Assert.Equal(new TabPage[] { value2, value1 }, owner.TabPages.Cast<TabPage>());
            Assert.Equal(new Control[] { value2, value1 }, owner.Controls.Cast<Control>());
            Assert.Same(owner, value1.Parent);
            Assert.True(value1.Visible);
            Assert.Equal(owner.DisplayRectangle, value1.Bounds);
            Assert.Null(value1.Site);
            Assert.Same(owner, value2.Parent);
            Assert.False(value2.Visible);
            Assert.Equal(owner.DisplayRectangle, value2.Bounds);
            Assert.Null(value2.Site);
            Assert.Equal(1, owner.SelectedIndex);
            Assert.Equal(2, layoutCallCount1);
            Assert.Equal(1, layoutCallCount2);
            Assert.Equal(10, parentLayoutCallCount);
            Assert.Same(value1, events[0].AffectedControl);
            Assert.Equal("Parent", events[0].AffectedProperty);
            Assert.Same(value1, events[1].AffectedControl);
            Assert.Equal("Visible", events[1].AffectedProperty);
            Assert.Same(value1, events[2].AffectedControl);
            Assert.Equal("Bounds", events[2].AffectedProperty);
            Assert.Same(value1, events[3].AffectedControl);
            Assert.Equal("Bounds", events[3].AffectedProperty);
            Assert.Same(value1, events[4].AffectedControl);
            Assert.Equal("Visible", events[4].AffectedProperty);
            Assert.Same(value2, events[5].AffectedControl);
            Assert.Equal("Parent", events[5].AffectedProperty);
            Assert.Same(value2, events[6].AffectedControl);
            Assert.Equal("Visible", events[6].AffectedProperty);
            Assert.Same(value2, events[7].AffectedControl);
            Assert.Equal("Bounds", events[7].AffectedProperty);
            Assert.Same(value2, events[8].AffectedControl);
            Assert.Equal("Bounds", events[8].AffectedProperty);
            Assert.Same(value2, events[9].AffectedControl);
            Assert.Equal("ChildIndex", events[9].AffectedProperty);
            Assert.True(value1.IsHandleCreated);
            Assert.Equal(2, invalidatedCallCount1);
            Assert.Equal(0, styleChangedCallCount1);
            Assert.Equal(0, createdCallCount1);
            Assert.True(value2.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount2);
            Assert.Equal(0, styleChangedCallCount2);
            Assert.Equal(0, createdCallCount2);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(expectedParentInvalidatedCallCount * 2, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);

            // Add again.
            collection.Insert(2, value1);
            Assert.Equal(new TabPage[] { value2, value1, value1 }, collection.Cast<TabPage>());
            Assert.Equal(new TabPage[] { value2, value1, value1 }, owner.TabPages.Cast<TabPage>());
            Assert.Equal(new Control[] { value2, value1 }, owner.Controls.Cast<Control>());
            Assert.Same(owner, value1.Parent);
            Assert.False(value1.Visible);
            Assert.Equal(owner.DisplayRectangle, value1.Bounds);
            Assert.Null(value1.Site);
            Assert.Same(owner, value2.Parent);
            Assert.False(value2.Visible);
            Assert.Equal(owner.DisplayRectangle, value2.Bounds);
            Assert.Null(value2.Site);
            Assert.Equal(1, owner.SelectedIndex);
            Assert.Equal(3, layoutCallCount1);
            Assert.Equal(1, layoutCallCount2);
            Assert.Equal(15, parentLayoutCallCount);
            Assert.Same(value1, events[0].AffectedControl);
            Assert.Equal("Parent", events[0].AffectedProperty);
            Assert.Same(value1, events[1].AffectedControl);
            Assert.Equal("Visible", events[1].AffectedProperty);
            Assert.Same(value1, events[2].AffectedControl);
            Assert.Equal("Bounds", events[2].AffectedProperty);
            Assert.Same(value1, events[3].AffectedControl);
            Assert.Equal("Bounds", events[3].AffectedProperty);
            Assert.Same(value1, events[4].AffectedControl);
            Assert.Equal("Visible", events[4].AffectedProperty);
            Assert.Same(value2, events[5].AffectedControl);
            Assert.Equal("Parent", events[5].AffectedProperty);
            Assert.Same(value2, events[6].AffectedControl);
            Assert.Equal("Visible", events[6].AffectedProperty);
            Assert.Same(value2, events[7].AffectedControl);
            Assert.Equal("Bounds", events[7].AffectedProperty);
            Assert.Same(value2, events[8].AffectedControl);
            Assert.Equal("Bounds", events[8].AffectedProperty);
            Assert.Same(value2, events[9].AffectedControl);
            Assert.Equal("ChildIndex", events[9].AffectedProperty);
            Assert.Same(value1, events[10].AffectedControl);
            Assert.Equal("ChildIndex", events[10].AffectedProperty);
            Assert.Same(value1, events[11].AffectedControl);
            Assert.Equal("Visible", events[11].AffectedProperty);
            Assert.Same(value1, events[12].AffectedControl);
            Assert.Equal("Visible", events[12].AffectedProperty);
            Assert.Same(value1, events[13].AffectedControl);
            Assert.Equal("Visible", events[13].AffectedProperty);
            Assert.Same(value1, events[14].AffectedControl);
            Assert.Equal("ChildIndex", events[14].AffectedProperty);
            Assert.True(value1.IsHandleCreated);
            Assert.Equal(3, invalidatedCallCount1);
            Assert.Equal(0, styleChangedCallCount1);
            Assert.Equal(0, createdCallCount1);
            Assert.True(value2.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount2);
            Assert.Equal(0, styleChangedCallCount2);
            Assert.Equal(0, createdCallCount2);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(expectedParentInvalidatedCallCount * 3, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);
        }
        finally
        {
            owner.Layout -= parentHandler;
        }
    }

    [WinFormsFact]
    public void TabPageCollection_Insert_OwnerWithSiteContainer_AddsToContainer()
    {
        Container container = new();
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(null);
        mockSite
            .Setup(s => s.Container)
            .Returns(container);
        using TabPage value = new();
        using TabControl owner = new()
        {
            Site = mockSite.Object
        };
        var collection = new TabControl.TabPageCollection(owner);
        collection.Insert(0, value);
        Assert.Same(owner, value.Parent);
        Assert.NotNull(value.Site);
        Assert.Same(value, Assert.Single(container.Components));
        mockSite.Verify(s => s.Container, Times.Once());
    }

    [WinFormsFact]
    public void TabPageCollection_Insert_OwnerWithSiteContainerValueHasSite_DoesNotAddToContainer()
    {
        Container container = new();
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(null);
        mockSite
            .Setup(s => s.Container)
            .Returns(container);
        Mock<ISite> mockValueSite = new(MockBehavior.Strict);
        mockValueSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(null);
        mockValueSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        using TabPage value = new()
        {
            Site = mockValueSite.Object
        };
        using TabControl owner = new()
        {
            Site = mockSite.Object
        };
        var collection = new TabControl.TabPageCollection(owner);
        collection.Insert(0, value);
        Assert.Same(owner, value.Parent);
        Assert.Same(mockValueSite.Object, value.Site);
        Assert.Empty(container.Components);
        mockSite.Verify(s => s.Container, Times.Never());
    }

    [WinFormsFact]
    public void TabPageCollection_Insert_OwnerWithSiteNoContainer_DoesNotAddContainer()
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(null);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        using TabPage value = new();
        using TabControl owner = new()
        {
            Site = mockSite.Object
        };
        var collection = new TabControl.TabPageCollection(owner);
        collection.Insert(0, value);
        Assert.Same(owner, value.Parent);
        Assert.Null(value.Site);
        mockSite.Verify(s => s.Container, Times.Once());
    }

    [WinFormsFact]
    public void TabPageCollection_Insert_ManyControls_Success()
    {
        using TabControl owner = new();
        var collection = new TabControl.TabPageCollection(owner);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);

        List<TabPage> items = [];
        for (int i = 0; i < 24; i++)
        {
            TabPage value = new();
            items.Insert(0, value);
            collection.Insert(0, value);
            Assert.Equal(items, collection.Cast<TabPage>());
            Assert.Equal(items, owner.TabPages.Cast<TabPage>());
            Assert.Equal(items, owner.Controls.Cast<TabPage>());
            Assert.Same(owner, value.Parent);
        }
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void TabPageCollection_Insert_InvokeIntString_Success(string text, string expectedText)
    {
        using TabControl owner = new();
        var collection = new TabControl.TabPageCollection(owner);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        collection.Insert(0, text);

        TabPage page = Assert.IsType<TabPage>(Assert.Single(collection));
        Assert.Empty(page.Name);
        Assert.Equal(expectedText, page.Text);
        Assert.Equal(-1, page.ImageIndex);
        Assert.Empty(page.ImageKey);
        Assert.Same(owner, page.Parent);
        Assert.True(page.Visible);
    }

    [WinFormsTheory]
    [InlineData(null, null, "", "")]
    [InlineData("", "", "", "")]
    [InlineData("name", "text", "name", "text")]
    public void TabPageCollection_Insert_InvokeIntStringString_Success(string key, string text, string expectedName, string expectedText)
    {
        using TabControl owner = new();
        var collection = new TabControl.TabPageCollection(owner);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        collection.Insert(0, key, text);

        TabPage page = Assert.IsType<TabPage>(Assert.Single(collection));
        Assert.Equal(expectedName, page.Name);
        Assert.Equal(expectedText, page.Text);
        Assert.Equal(-1, page.ImageIndex);
        Assert.Empty(page.ImageKey);
        Assert.Same(owner, page.Parent);
        Assert.True(page.Visible);
    }

    [WinFormsTheory]
    [InlineData(null, null, -1, "", "")]
    [InlineData("", "", 0, "", "")]
    [InlineData("name", "text", 1, "name", "text")]
    public void TabPageCollection_Insert_InvokeIntStringStringInt_Success(string key, string text, int imageIndex, string expectedName, string expectedText)
    {
        using TabControl owner = new();
        var collection = new TabControl.TabPageCollection(owner);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        collection.Insert(0, key, text, imageIndex);

        TabPage page = Assert.IsType<TabPage>(Assert.Single(collection));
        Assert.Equal(expectedName, page.Name);
        Assert.Equal(expectedText, page.Text);
        Assert.Equal(imageIndex, page.ImageIndex);
        Assert.Empty(page.ImageKey);
        Assert.Same(owner, page.Parent);
        Assert.True(page.Visible);
    }

    [WinFormsTheory]
    [InlineData(null, null, null, "", "", "")]
    [InlineData("", "", "", "", "", "")]
    [InlineData("name", "text", "imageKey", "name", "text", "imageKey")]
    public void TabPageCollection_Insert_InvokeIntStringStringString_Success(string key, string text, string imageKey, string expectedName, string expectedText, string expectedImageKey)
    {
        using TabControl owner = new();
        var collection = new TabControl.TabPageCollection(owner);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        collection.Insert(0, key, text, imageKey);

        TabPage page = Assert.IsType<TabPage>(Assert.Single(collection));
        Assert.Equal(expectedName, page.Name);
        Assert.Equal(expectedText, page.Text);
        Assert.Equal(-1, page.ImageIndex);
        Assert.Equal(expectedImageKey, page.ImageKey);
        Assert.Same(owner, page.Parent);
        Assert.True(page.Visible);
    }

    [WinFormsTheory]
    [InlineData("Text", "Text")]
    [InlineData("&&Text", "&&Text")]
    [InlineData("&", "&&")]
    [InlineData("&Text", "&&Text")]
    public unsafe void TabPageCollection_Insert_GetItemsWithHandle_Success(string text, string expectedText)
    {
        using TabControl owner = new();
        Assert.NotEqual(IntPtr.Zero, owner.Handle);

        using TabPage page1 = new();
        using TabPage page2 = new()
        {
            Text = text,
            ImageIndex = 1
        };
        using NullTextTabPage page3 = new();
        var collection = new TabControl.TabPageCollection(owner);
        collection.Insert(0, page3);
        collection.Insert(0, page2);
        collection.Insert(0, page1);
        Assert.Equal(3, (int)PInvokeCore.SendMessage(owner, PInvoke.TCM_GETITEMCOUNT));

        char* buffer = stackalloc char[256];
        TCITEMW item = default;
        item.cchTextMax = int.MaxValue;
        item.pszText = buffer;
        item.dwStateMask = (TAB_CONTROL_ITEM_STATE)uint.MaxValue;
        item.mask = (TCITEMHEADERA_MASK)uint.MaxValue;

        // Get item 0.
        Assert.Equal(1, (int)PInvokeCore.SendMessage(owner, PInvoke.TCM_GETITEMW, 0, ref item));
        Assert.Equal((TAB_CONTROL_ITEM_STATE)0, item.dwState);
        Assert.Equal(IntPtr.Zero, (nint)item.lParam);
        Assert.Equal(int.MaxValue, item.cchTextMax);
        Assert.Empty(new string(item.pszText));
        Assert.Equal(-1, item.iImage);

        // Get item 1.
        Assert.Equal(1, (int)PInvokeCore.SendMessage(owner, PInvoke.TCM_GETITEMW, 1, ref item));
        Assert.Equal((TAB_CONTROL_ITEM_STATE)0, item.dwState);
        Assert.Equal(IntPtr.Zero, (nint)item.lParam);
        Assert.Equal(int.MaxValue, item.cchTextMax);
        Assert.Equal(expectedText, new string(item.pszText));
        Assert.Equal(1, item.iImage);

        // Get item 2.
        Assert.Equal(1, (int)PInvokeCore.SendMessage(owner, PInvoke.TCM_GETITEMW, 2, ref item));
        Assert.Equal(TAB_CONTROL_ITEM_STATE.TCIS_BUTTONPRESSED, item.dwState);
        Assert.Equal(IntPtr.Zero, (nint)item.lParam);
        Assert.Equal(int.MaxValue, item.cchTextMax);
        Assert.Empty(new string(item.pszText));
        Assert.Equal(-1, item.iImage);
    }

    [WinFormsTheory]
    [InlineData("Text", "Text")]
    [InlineData("&&Text", "&&Text")]
    [InlineData("&", "&&")]
    [InlineData("&Text", "&&Text")]
    public unsafe void TabPageCollection_Insert_GetItemsDesignModeWithHandle_Success(string text, string expectedText)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(null);
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(true);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        using TabControl owner = new()
        {
            Site = mockSite.Object
        };
        Assert.NotEqual(IntPtr.Zero, owner.Handle);

        using TabPage page1 = new();
        using TabPage page2 = new()
        {
            Text = text,
            ImageIndex = 1
        };
        using NullTextTabPage page3 = new();
        var collection = new TabControl.TabPageCollection(owner);
        collection.Insert(0, page3);
        collection.Insert(0, page2);
        collection.Insert(0, page1);
        Assert.Equal(3, (int)PInvokeCore.SendMessage(owner, PInvoke.TCM_GETITEMCOUNT));

        char* buffer = stackalloc char[256];
        TCITEMW item = default;
        item.cchTextMax = int.MaxValue;
        item.pszText = buffer;
        item.dwStateMask = (TAB_CONTROL_ITEM_STATE)uint.MaxValue;
        item.mask = (TCITEMHEADERA_MASK)uint.MaxValue;

        // Get item 0.
        Assert.Equal(1, (int)PInvokeCore.SendMessage(owner, PInvoke.TCM_GETITEMW, 0, ref item));
        Assert.Equal((TAB_CONTROL_ITEM_STATE)0, item.dwState);
        Assert.Equal(IntPtr.Zero, (nint)item.lParam);
        Assert.Equal(int.MaxValue, item.cchTextMax);
        Assert.Empty(new string(item.pszText));
        Assert.Equal(-1, item.iImage);

        // Get item 1.
        Assert.Equal(1, (int)PInvokeCore.SendMessage(owner, PInvoke.TCM_GETITEMW, 1, ref item));
        Assert.Equal((TAB_CONTROL_ITEM_STATE)0, item.dwState);
        Assert.Equal(IntPtr.Zero, (nint)item.lParam);
        Assert.Equal(int.MaxValue, item.cchTextMax);
        Assert.Equal(expectedText, new string(item.pszText));
        Assert.Equal(1, item.iImage);

        // Get item 2.
        Assert.Equal(1, (int)PInvokeCore.SendMessage(owner, PInvoke.TCM_GETITEMW, 2, ref item));
        Assert.Equal(TAB_CONTROL_ITEM_STATE.TCIS_BUTTONPRESSED, item.dwState);
        Assert.Equal(IntPtr.Zero, (nint)item.lParam);
        Assert.Equal(int.MaxValue, item.cchTextMax);
        Assert.Empty(new string(item.pszText));
        Assert.Equal(-1, item.iImage);
    }

    [WinFormsFact]
    public void TabPageCollection_Insert_NullTabPage_ThrowsArgumentNullException()
    {
        using TabControl owner = new();
        var collection = new TabControl.TabPageCollection(owner);
        Assert.Throws<ArgumentNullException>("tabPage", () => collection.Insert(0, (TabPage)null));
    }

    [WinFormsFact]
    public void TabPageCollection_Insert_NegativeIndexEmpty_ThrowsArgumentOutOfRangeException()
    {
        using TabControl owner = new();
        var collection = new TabControl.TabPageCollection(owner);
        using TabPage value = new();
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.Insert(-1, value));
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.Insert(-1, "text"));
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.Insert(-1, "key", "text"));
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.Insert(-1, "key", "text", 1));
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.Insert(-1, "key", "text", "imageKey"));
        Assert.Empty(collection);
    }

    [WinFormsFact]
    public void TabPageCollection_Insert_NegativeIndexEmptyWithHandle_ThrowsArgumentOutOfRangeException()
    {
        using TabControl owner = new();
        var collection = new TabControl.TabPageCollection(owner);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);

        using TabPage value = new();
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.Insert(-1, value));
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.Insert(-1, "text"));
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.Insert(-1, "key", "text"));
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.Insert(-1, "key", "text", 1));
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.Insert(-1, "key", "text", "imageKey"));
        Assert.Empty(collection);
    }

    [WinFormsTheory]
    [InlineData(1)]
    [InlineData(2)]
    public void TabPageCollection_Insert_InvalidIndexEmpty_Nop(int index)
    {
        using TabControl owner = new();
        var collection = new TabControl.TabPageCollection(owner);
        using TabPage value = new();
        Assert.Throws<ArgumentOutOfRangeException>(() => collection.Insert(index, value));
    }

    [WinFormsTheory]
    [InlineData(1)]
    [InlineData(2)]
    public void TabPageCollection_Insert_InvalidIndexEmptyWithHandle_Nop(int index)
    {
        using TabControl owner = new();
        var collection = new TabControl.TabPageCollection(owner);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);

        using TabPage value = new();
        Assert.Throws<ArgumentOutOfRangeException>(() => collection.Insert(index, value));
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(2)]
    [InlineData(3)]
    public void TabPageCollection_Insert_InvalidIndexNotEmpty_ThrowsArgumentOutOfRangeException(int index)
    {
        using TabControl owner = new();
        using TabPage child = new();
        var collection = new TabControl.TabPageCollection(owner)
        {
            child
        };
        using TabPage value = new();
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.Insert(index, value));
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.Insert(index, "text"));
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.Insert(index, "key", "text"));
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.Insert(index, "key", "text", 1));
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.Insert(index, "key", "text", "imageKey"));
        Assert.Same(child, Assert.Single(collection));
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(2)]
    [InlineData(3)]
    public void TabPageCollection_Insert_InvalidIndexNotEmptyWithHandle_ThrowsArgumentOutOfRangeException(int index)
    {
        using TabControl owner = new();
        using TabPage child = new();
        var collection = new TabControl.TabPageCollection(owner)
        {
            child
        };
        Assert.NotEqual(IntPtr.Zero, owner.Handle);

        using TabPage value = new();
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.Insert(index, value));
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.Insert(index, "text"));
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.Insert(index, "key", "text"));
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.Insert(index, "key", "text", 1));
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.Insert(index, "key", "text", "imageKey"));
        Assert.Same(child, Assert.Single(collection));
    }

    [WinFormsTheory]
    [MemberData(nameof(Add_TestData))]
    public void TabPageCollection_IListInsert_InvokeValueWithoutHandleOwnerWithoutHandle_Success(TabAppearance appearance)
    {
        using TabControl owner = new()
        {
            Appearance = appearance,
            Bounds = new Rectangle(0, 0, 400, 300)
        };
        using TabPage value1 = new();
        using TabPage value2 = new();
        var collection = new TabControl.TabPageCollection(owner);
        IList iList = collection;

        int layoutCallCount1 = 0;
        value1.Layout += (sender, e) => layoutCallCount1++;
        int layoutCallCount2 = 0;
        value2.Layout += (sender, e) => layoutCallCount2++;
        int parentLayoutCallCount = 0;
        List<LayoutEventArgs> events = [];
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(owner, sender);
            events.Add(e);
            parentLayoutCallCount++;
        }

        owner.Layout += parentHandler;

        try
        {
            // Add first.
            iList.Insert(0, value1);
            Assert.Same(value1, Assert.Single(collection));
            Assert.Same(value1, Assert.Single(owner.TabPages));
            Assert.Same(value1, Assert.Single(owner.Controls));
            Assert.Same(owner, value1.Parent);
            Assert.False(value1.Visible);
            Assert.Equal(new Rectangle(0, 0, 200, 100), value1.Bounds);
            Assert.Null(value1.Site);
            Assert.Equal(-1, owner.SelectedIndex);
            Assert.Equal(0, layoutCallCount1);
            Assert.Equal(2, parentLayoutCallCount);
            Assert.Same(value1, events[0].AffectedControl);
            Assert.Same("Parent", events[0].AffectedProperty);
            Assert.Same(value1, events[1].AffectedControl);
            Assert.Same("Visible", events[1].AffectedProperty);
            Assert.False(value1.IsHandleCreated);
            Assert.False(owner.IsHandleCreated);

            // Add another.
            iList.Insert(0, value2);
            Assert.Equal(new TabPage[] { value2, value1 }, collection.Cast<TabPage>());
            Assert.Equal(new TabPage[] { value2, value1 }, owner.TabPages.Cast<TabPage>());
            Assert.Equal(new Control[] { value2, value1 }, owner.Controls.Cast<Control>());
            Assert.Same(owner, value1.Parent);
            Assert.False(value1.Visible);
            Assert.Equal(new Rectangle(0, 0, 200, 100), value1.Bounds);
            Assert.Null(value1.Site);
            Assert.Same(owner, value2.Parent);
            Assert.False(value2.Visible);
            Assert.Equal(new Rectangle(0, 0, 200, 100), value2.Bounds);
            Assert.Null(value2.Site);
            Assert.Equal(-1, owner.SelectedIndex);
            Assert.Equal(0, layoutCallCount1);
            Assert.Equal(0, layoutCallCount2);
            Assert.Equal(5, parentLayoutCallCount);
            Assert.Same(value1, events[0].AffectedControl);
            Assert.Same("Parent", events[0].AffectedProperty);
            Assert.Same(value1, events[1].AffectedControl);
            Assert.Same("Visible", events[1].AffectedProperty);
            Assert.Same(value2, events[2].AffectedControl);
            Assert.Same("Parent", events[2].AffectedProperty);
            Assert.Same(value2, events[3].AffectedControl);
            Assert.Same("Visible", events[3].AffectedProperty);
            Assert.Same(value2, events[4].AffectedControl);
            Assert.Same("ChildIndex", events[4].AffectedProperty);
            Assert.False(value1.IsHandleCreated);
            Assert.False(value2.IsHandleCreated);
            Assert.False(owner.IsHandleCreated);

            // Add again.
            iList.Insert(2, value1);
            Assert.Equal(new TabPage[] { value2, value1, value1 }, collection.Cast<TabPage>());
            Assert.Equal(new TabPage[] { value2, value1, value1 }, owner.TabPages.Cast<TabPage>());
            Assert.Equal(new Control[] { value2, value1 }, owner.Controls.Cast<Control>());
            Assert.Same(owner, value1.Parent);
            Assert.False(value1.Visible);
            Assert.Same(owner, value2.Parent);
            Assert.Equal(new Rectangle(0, 0, 200, 100), value1.Bounds);
            Assert.Null(value1.Site);
            Assert.False(value2.Visible);
            Assert.Equal(new Rectangle(0, 0, 200, 100), value2.Bounds);
            Assert.Null(value2.Site);
            Assert.Equal(-1, owner.SelectedIndex);
            Assert.Equal(0, layoutCallCount1);
            Assert.Equal(7, parentLayoutCallCount);
            Assert.Same(value1, events[0].AffectedControl);
            Assert.Same("Parent", events[0].AffectedProperty);
            Assert.Same(value1, events[1].AffectedControl);
            Assert.Same("Visible", events[1].AffectedProperty);
            Assert.Same(value2, events[2].AffectedControl);
            Assert.Same("Parent", events[2].AffectedProperty);
            Assert.Same(value2, events[3].AffectedControl);
            Assert.Same("Visible", events[3].AffectedProperty);
            Assert.Same(value2, events[4].AffectedControl);
            Assert.Same("ChildIndex", events[4].AffectedProperty);
            Assert.Same(value1, events[5].AffectedControl);
            Assert.Same("ChildIndex", events[5].AffectedProperty);
            Assert.Same(value1, events[6].AffectedControl);
            Assert.Same("ChildIndex", events[6].AffectedProperty);
            Assert.False(value1.IsHandleCreated);
            Assert.False(value2.IsHandleCreated);
            Assert.False(owner.IsHandleCreated);
        }
        finally
        {
            owner.Layout -= parentHandler;
        }
    }

    public static IEnumerable<object[]> IListInsert_InvalidValue_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new() };
        yield return new object[] { new Control() };
    }

    [WinFormsTheory]
    [MemberData(nameof(IListInsert_InvalidValue_TestData))]
    public void TabPageCollection_IListInsert_NotTabPage_ThrowsArgumentException(object value)
    {
        using TabControl owner = new();
        var collection = new TabControl.TabPageCollection(owner);
        IList iList = collection;
        Assert.Throws<ArgumentException>(() => iList.Insert(0, value));
    }

    [WinFormsFact]
    public void TabPageCollection_IListInsert_NegativeIndexEmpty_ThrowsArgumentOutOfRangeException()
    {
        using TabControl owner = new();
        var collection = new TabControl.TabPageCollection(owner);
        IList iList = collection;
        using TabPage value = new();
        Assert.Throws<ArgumentOutOfRangeException>("index", () => iList.Insert(-1, value));
        Assert.Empty(collection);
    }

    [WinFormsFact]
    public void TabPageCollection_IListInsert_NegativeIndexEmptyWithHandle_ThrowsArgumentOutOfRangeException()
    {
        using TabControl owner = new();
        var collection = new TabControl.TabPageCollection(owner);
        IList iList = collection;
        Assert.NotEqual(IntPtr.Zero, owner.Handle);

        using TabPage value = new();
        Assert.Throws<ArgumentOutOfRangeException>("index", () => iList.Insert(-1, value));
        Assert.Empty(collection);
    }

    [WinFormsTheory]
    [InlineData(1)]
    [InlineData(2)]
    public void TabPageCollection_IListInsert_InvalidIndexEmpty_Nop(int index)
    {
        using TabControl owner = new();
        var collection = new TabControl.TabPageCollection(owner);
        IList iList = collection;
        using TabPage value = new();
        Assert.Throws<ArgumentOutOfRangeException>(() => iList.Insert(index, value));
    }

    [WinFormsTheory]
    [InlineData(1)]
    [InlineData(2)]
    public void TabPageCollection_IListInsert_InvalidIndexEmptyWithHandle_Nop(int index)
    {
        using TabControl owner = new();
        var collection = new TabControl.TabPageCollection(owner);
        IList iList = collection;
        Assert.NotEqual(IntPtr.Zero, owner.Handle);

        using TabPage value = new();
        Assert.Throws<ArgumentOutOfRangeException>(() => iList.Insert(index, value));
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(2)]
    [InlineData(3)]
    public void TabPageCollection_IListInsert_InvalidIndexNotEmpty_ThrowsArgumentOutOfRangeException(int index)
    {
        using TabControl owner = new();
        using TabPage child = new();
        var collection = new TabControl.TabPageCollection(owner);
        IList iList = collection;
        collection.Add(child);
        using TabPage value = new();
        Assert.Throws<ArgumentOutOfRangeException>("index", () => iList.Insert(index, value));
        Assert.Same(child, Assert.Single(collection));
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(2)]
    [InlineData(3)]
    public void TabPageCollection_IListInsert_InvalidIndexNotEmptyWithHandle_ThrowsArgumentOutOfRangeException(int index)
    {
        using TabControl owner = new();
        using TabPage child = new();
        var collection = new TabControl.TabPageCollection(owner);
        IList iList = collection;
        collection.Add(child);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);

        using TabPage value = new();
        Assert.Throws<ArgumentOutOfRangeException>("index", () => iList.Insert(index, value));
        Assert.Same(child, Assert.Single(collection));
    }

    [WinFormsTheory]
    [InlineData("name1", 0)]
    [InlineData("NAME1", 0)]
    [InlineData("name2", 1)]
    public void TabPageCollection_Item_GetStringValidKey_ReturnsExpected(string key, int expectedIndex)
    {
        using TabControl owner = new();
        using TabPage child1 = new()
        {
            Name = "name1"
        };
        using TabPage child2 = new()
        {
            Name = "name2"
        };
        using TabPage child3 = new()
        {
            Name = "name2"
        };
        var collection = new TabControl.TabPageCollection(owner)
        {
            child1,
            child2,
            child3
        };

        Assert.Equal(collection[expectedIndex], collection[key]);

        // Call again.
        Assert.Equal(collection[expectedIndex], collection[key]);
        Assert.Null(collection["NoSuchKey"]);
    }

    [WinFormsTheory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("NoSuchName")]
    [InlineData("abcd")]
    [InlineData("abcde")]
    [InlineData("abcdef")]
    public void TabPageCollection_Item_GetStringNoSuchKey_ReturnsNull(string key)
    {
        using TabControl owner = new();
        using TabPage child1 = new()
        {
            Name = "name1"
        };
        using TabPage child2 = new()
        {
            Name = "name2"
        };
        using TabPage child3 = new()
        {
            Name = "name2"
        };
        var collection = new TabControl.TabPageCollection(owner)
        {
            child1,
            child2,
            child3
        };

        Assert.Null(collection[key]);

        // Call again.
        Assert.Null(collection[key]);
        Assert.Null(collection["NoSuchKey"]);
    }

    [WinFormsTheory]
    [StringWithNullData]
    public void TabPageCollection_Item_GetStringEmpty_ReturnsNull(string key)
    {
        using TabControl owner = new();
        var collection = new TabControl.TabPageCollection(owner);

        Assert.Null(collection[key]);

        // Call again.
        Assert.Null(collection[key]);
        Assert.Null(collection["NoSuchKey"]);
    }

    [WinFormsFact]
    public void TabPageCollection_Item_GetIntValidIndex_ReturnsExpected()
    {
        using TabControl owner = new();
        using TabPage page1 = new();
        using TabPage page2 = new();
        var collection = new TabControl.TabPageCollection(owner)
        {
            page1,
            page2
        };
        Assert.Same(page1, collection[0]);
        Assert.Same(page2, collection[1]);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void TabPageCollection_Item_GetIntInvalidIndexEmpty_ThrowsArgumentOutOfRangeException(int index)
    {
        using TabControl owner = new();
        var collection = new TabControl.TabPageCollection(owner);
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index]);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(1)]
    [InlineData(2)]
    public void TabPageCollection_Item_GetIntInvalidIndexNotEmpty_ThrowsArgumentOutOfRangeException(int index)
    {
        using TabControl owner = new();
        using TabPage page = new();
        var collection = new TabControl.TabPageCollection(owner)
        {
            page
        };
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index]);
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void TabPageCollection_Item_SetValidIndex_ReturnsExpected(int index)
    {
        using TabControl owner = new();
        using TabPage page1 = new();
        using TabPage page2 = new();
        using TabPage page3 = new();
        var collection = new TabControl.TabPageCollection(owner)
        {
            page1,
            page2,
            page3
        };

        using TabPage newPage = new();
        collection[index] = newPage;
        Assert.Same(newPage, collection[index]);
        Assert.Same(owner, page1.Parent);
        Assert.Same(owner, page2.Parent);
        Assert.Same(owner, page3.Parent);
        Assert.Null(newPage.Parent);
        Assert.Equal(new Control[] { page1, page2, page3 }, owner.Controls.Cast<Control>());
        Assert.False(page1.IsHandleCreated);
        Assert.False(page2.IsHandleCreated);
        Assert.False(page3.IsHandleCreated);
        Assert.False(newPage.IsHandleCreated);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void TabPageCollection_Item_SetValidIndexDesignMode_ReturnsExpected(int index)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(null);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(true);
        using TabControl owner = new()
        {
            Site = mockSite.Object
        };
        using TabPage page1 = new();
        using TabPage page2 = new();
        using TabPage page3 = new();
        var collection = new TabControl.TabPageCollection(owner)
        {
            page1,
            page2,
            page3
        };

        using TabPage newPage = new();
        collection[index] = newPage;
        Assert.Same(newPage, collection[index]);
        Assert.Same(owner, page1.Parent);
        Assert.Same(owner, page2.Parent);
        Assert.Same(owner, page3.Parent);
        Assert.Null(newPage.Parent);
        Assert.Equal(new Control[] { page1, page2, page3 }, owner.Controls.Cast<Control>());
        Assert.False(page1.IsHandleCreated);
        Assert.False(page2.IsHandleCreated);
        Assert.False(page3.IsHandleCreated);
        Assert.False(newPage.IsHandleCreated);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void TabPageCollection_Item_SetValidIndexWithHandle_ReturnsExpected(int index)
    {
        using TabControl owner = new();
        using TabPage page1 = new();
        using TabPage page2 = new();
        using TabPage page3 = new();
        var collection = new TabControl.TabPageCollection(owner)
        {
            page1,
            page2,
            page3
        };

        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int parentInvalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => parentInvalidatedCallCount++;
        int parentStyleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
        int parentCreatedCallCount = 0;
        owner.HandleCreated += (sender, e) => parentCreatedCallCount++;

        using TabPage newPage = new();
        collection[index] = newPage;
        Assert.Same(newPage, collection[index]);
        Assert.Same(owner, page1.Parent);
        Assert.Same(owner, page2.Parent);
        Assert.Same(owner, page3.Parent);
        Assert.Null(newPage.Parent);
        Assert.Equal(new Control[] { page1, page2, page3 }, owner.Controls.Cast<Control>());
        Assert.True(page1.IsHandleCreated);
        Assert.False(page2.IsHandleCreated);
        Assert.False(page3.IsHandleCreated);
        Assert.False(newPage.IsHandleCreated);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, parentInvalidatedCallCount);
        Assert.Equal(0, parentStyleChangedCallCount);
        Assert.Equal(0, parentCreatedCallCount);
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void TabPageCollection_Item_SetValidIndexDesignModeWithHandle_ReturnsExpected(int index)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(null);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(true);
        using TabControl owner = new()
        {
            Site = mockSite.Object
        };
        using TabPage page1 = new();
        using TabPage page2 = new();
        using TabPage page3 = new();
        var collection = new TabControl.TabPageCollection(owner)
        {
            page1,
            page2,
            page3
        };

        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int parentInvalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => parentInvalidatedCallCount++;
        int parentStyleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
        int parentCreatedCallCount = 0;
        owner.HandleCreated += (sender, e) => parentCreatedCallCount++;

        using TabPage newPage = new();
        collection[index] = newPage;
        Assert.Same(newPage, collection[index]);
        Assert.Same(owner, page1.Parent);
        Assert.Same(owner, page2.Parent);
        Assert.Same(owner, page3.Parent);
        Assert.Null(newPage.Parent);
        Assert.Equal(new Control[] { page1, page2, page3 }, owner.Controls.Cast<Control>());
        Assert.True(page1.IsHandleCreated);
        Assert.False(page2.IsHandleCreated);
        Assert.False(page3.IsHandleCreated);
        Assert.False(newPage.IsHandleCreated);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, parentInvalidatedCallCount);
        Assert.Equal(0, parentStyleChangedCallCount);
        Assert.Equal(0, parentCreatedCallCount);
    }

    [WinFormsTheory]
    [InlineData("Text", "Text")]
    [InlineData("&&Text", "&&Text")]
    [InlineData("&", "&&")]
    [InlineData("&Text", "&&Text")]
    public unsafe void TabPageCollection_Item_SetGetItemsWithHandle_Success(string text, string expectedText)
    {
        using TabControl owner = new();
        using TabPage page1 = new();
        using TabPage page2 = new();
        using NullTextTabPage page3 = new();
        var collection = new TabControl.TabPageCollection(owner)
        {
            page1,
            page2,
            page3
        };
        Assert.NotEqual(IntPtr.Zero, owner.Handle);

        using TabPage value = new()
        {
            Text = text,
            ImageIndex = 1
        };
        collection[1] = value;
        Assert.Equal(3, (int)PInvokeCore.SendMessage(owner, PInvoke.TCM_GETITEMCOUNT));

        char* buffer = stackalloc char[256];
        TCITEMW item = default;
        item.cchTextMax = int.MaxValue;
        item.pszText = buffer;
        item.dwStateMask = (TAB_CONTROL_ITEM_STATE)uint.MaxValue;
        item.mask = (TCITEMHEADERA_MASK)uint.MaxValue;

        // Get item 0.
        Assert.Equal(1, (int)PInvokeCore.SendMessage(owner, PInvoke.TCM_GETITEMW, 0, ref item));
        Assert.Equal(TAB_CONTROL_ITEM_STATE.TCIS_BUTTONPRESSED, item.dwState);
        Assert.Equal(IntPtr.Zero, (nint)item.lParam);
        Assert.Equal(int.MaxValue, item.cchTextMax);
        Assert.Empty(new string(item.pszText));
        Assert.Equal(-1, item.iImage);

        // Get item 1.
        Assert.Equal(1, (int)PInvokeCore.SendMessage(owner, PInvoke.TCM_GETITEMW, 1, ref item));
        Assert.Equal((TAB_CONTROL_ITEM_STATE)0, item.dwState);
        Assert.Equal(IntPtr.Zero, (nint)item.lParam);
        Assert.Equal(int.MaxValue, item.cchTextMax);
        Assert.Equal(expectedText, new string(item.pszText));
        Assert.Equal(1, item.iImage);

        // Get item 2.
        Assert.Equal(1, (int)PInvokeCore.SendMessage(owner, PInvoke.TCM_GETITEMW, 2, ref item));
        Assert.Equal((TAB_CONTROL_ITEM_STATE)0, item.dwState);
        Assert.Equal(IntPtr.Zero, (nint)item.lParam);
        Assert.Equal(int.MaxValue, item.cchTextMax);
        Assert.Empty(new string(item.pszText));
        Assert.Equal(-1, item.iImage);
    }

    [WinFormsTheory]
    [InlineData("Text", "Text")]
    [InlineData("&&Text", "&&Text")]
    [InlineData("&", "&&")]
    [InlineData("&Text", "&&Text")]
    public unsafe void TabPageCollection_Item_SetGetItemsDesignModeWithHandle_Success(string text, string expectedText)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(null);
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(true);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        using TabControl owner = new()
        {
            Site = mockSite.Object
        };
        using TabPage page1 = new();
        using TabPage page2 = new();
        using NullTextTabPage page3 = new();
        var collection = new TabControl.TabPageCollection(owner)
        {
            page1,
            page2,
            page3
        };
        Assert.NotEqual(IntPtr.Zero, owner.Handle);

        using TabPage value = new()
        {
            Text = text,
            ImageIndex = 1
        };
        collection[1] = value;
        Assert.Equal(3, (int)PInvokeCore.SendMessage(owner, PInvoke.TCM_GETITEMCOUNT));

        char* buffer = stackalloc char[256];
        TCITEMW item = default;
        item.cchTextMax = int.MaxValue;
        item.pszText = buffer;
        item.dwStateMask = (TAB_CONTROL_ITEM_STATE)uint.MaxValue;
        item.mask = (TCITEMHEADERA_MASK)uint.MaxValue;

        // Get item 0.
        Assert.Equal(1, (int)PInvokeCore.SendMessage(owner, PInvoke.TCM_GETITEMW, 0, ref item));
        Assert.Equal((TAB_CONTROL_ITEM_STATE)0, item.dwState);
        Assert.Equal(IntPtr.Zero, (nint)item.lParam);
        Assert.Equal(int.MaxValue, item.cchTextMax);
        Assert.Empty(new string(item.pszText));
        Assert.Equal(-1, item.iImage);

        // Get item 1.
        Assert.Equal(1, (int)PInvokeCore.SendMessage(owner, PInvoke.TCM_GETITEMW, 1, ref item));
        Assert.Equal(TAB_CONTROL_ITEM_STATE.TCIS_BUTTONPRESSED, item.dwState);
        Assert.Equal(IntPtr.Zero, (nint)item.lParam);
        Assert.Equal(int.MaxValue, item.cchTextMax);
        Assert.Equal(expectedText, new string(item.pszText));
        Assert.Equal(1, item.iImage);

        // Get item 2.
        Assert.Equal(1, (int)PInvokeCore.SendMessage(owner, PInvoke.TCM_GETITEMW, 2, ref item));
        Assert.Equal((TAB_CONTROL_ITEM_STATE)0, item.dwState);
        Assert.Equal(IntPtr.Zero, (nint)item.lParam);
        Assert.Equal(int.MaxValue, item.cchTextMax);
        Assert.Empty(new string(item.pszText));
        Assert.Equal(-1, item.iImage);
    }

    [WinFormsFact]
    public void TabPageCollection_Item_SetNull_ThrowsArgumentNullException()
    {
        using TabControl owner = new();
        using TabPage page = new();
        var collection = new TabControl.TabPageCollection(owner)
        {
            page
        };

        Assert.Throws<ArgumentNullException>("value", () => collection[0] = null);
    }

    [WinFormsFact]
    public void TabPageCollection_Item_SetNullWithHandle_ThrowsArgumentNullException()
    {
        using TabControl owner = new();
        using TabPage page = new();
        var collection = new TabControl.TabPageCollection(owner);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        collection.Add(page);

        Assert.Throws<ArgumentNullException>("value", () => collection[0] = null);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void TabPageCollection_Item_SetInvalidIndexEmpty_ThrowsArgumentOutOfRangeException(int index)
    {
        using TabControl owner = new();
        using TabPage page = new();
        var collection = new TabControl.TabPageCollection(owner);
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index] = page);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(1)]
    [InlineData(2)]
    public void TabPageCollection_Item_SetInvalidIndexNotEmpty_ThrowsArgumentOutOfRangeException(int index)
    {
        using TabControl owner = new();
        using TabPage page1 = new();
        using TabPage page2 = new();
        var collection = new TabControl.TabPageCollection(owner)
        {
            page1
        };
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index] = page2);
    }

    [WinFormsFact]
    public void TabPageCollection_IListItem_GetValidIndex_ReturnsExpected()
    {
        using TabControl owner = new();
        using TabPage page1 = new();
        using TabPage page2 = new();
        var collection = new TabControl.TabPageCollection(owner);
        IList iList = collection;
        iList.Add(page1);
        iList.Add(page2);
        Assert.Same(page1, iList[0]);
        Assert.Same(page2, iList[1]);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void TabPageCollection_IListItem_GetInvalidIndexEmpty_ThrowsArgumentOutOfRangeException(int index)
    {
        using TabControl owner = new();
        var collection = new TabControl.TabPageCollection(owner);
        IList iList = collection;
        Assert.Throws<ArgumentOutOfRangeException>("index", () => iList[index]);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(1)]
    [InlineData(2)]
    public void TabPageCollection_IListItem_GetInvalidIndexNotEmpty_ThrowsArgumentOutOfRangeException(int index)
    {
        using TabControl owner = new();
        using TabPage page = new();
        var collection = new TabControl.TabPageCollection(owner);
        IList iList = collection;
        iList.Add(page);
        Assert.Throws<ArgumentOutOfRangeException>("index", () => iList[index]);
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void TabPageCollection_IListItem_SetValidIndex_ReturnsExpected(int index)
    {
        using TabControl owner = new();
        using TabPage page1 = new();
        using TabPage page2 = new();
        using TabPage page3 = new();
        var collection = new TabControl.TabPageCollection(owner);
        IList iList = collection;
        iList.Add(page1);
        iList.Add(page2);
        iList.Add(page3);

        using TabPage newPage = new();
        iList[index] = newPage;
        Assert.Same(newPage, iList[index]);
        Assert.Same(owner, page1.Parent);
        Assert.Same(owner, page2.Parent);
        Assert.Same(owner, page3.Parent);
        Assert.Null(newPage.Parent);
        Assert.Equal(new Control[] { page1, page2, page3 }, owner.Controls.Cast<Control>());
        Assert.False(page1.IsHandleCreated);
        Assert.False(page2.IsHandleCreated);
        Assert.False(page3.IsHandleCreated);
        Assert.False(newPage.IsHandleCreated);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void TabPageCollection_IListItem_SetValidIndexDesignMode_ReturnsExpected(int index)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(null);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(true);
        using TabControl owner = new()
        {
            Site = mockSite.Object
        };
        using TabPage page1 = new();
        using TabPage page2 = new();
        using TabPage page3 = new();
        var collection = new TabControl.TabPageCollection(owner);
        IList iList = collection;
        iList.Add(page1);
        iList.Add(page2);
        iList.Add(page3);

        using TabPage newPage = new();
        iList[index] = newPage;
        Assert.Same(newPage, iList[index]);
        Assert.Same(owner, page1.Parent);
        Assert.Same(owner, page2.Parent);
        Assert.Same(owner, page3.Parent);
        Assert.Null(newPage.Parent);
        Assert.Equal(new Control[] { page1, page2, page3 }, owner.Controls.Cast<Control>());
        Assert.False(page1.IsHandleCreated);
        Assert.False(page2.IsHandleCreated);
        Assert.False(page3.IsHandleCreated);
        Assert.False(newPage.IsHandleCreated);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void TabPageCollection_IListItem_SetValidIndexWithHandle_ReturnsExpected(int index)
    {
        using TabControl owner = new();
        using TabPage page1 = new();
        using TabPage page2 = new();
        using TabPage page3 = new();
        var collection = new TabControl.TabPageCollection(owner);
        IList iList = collection;
        iList.Add(page1);
        iList.Add(page2);
        iList.Add(page3);

        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int parentInvalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => parentInvalidatedCallCount++;
        int parentStyleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
        int parentCreatedCallCount = 0;
        owner.HandleCreated += (sender, e) => parentCreatedCallCount++;

        using TabPage newPage = new();
        iList[index] = newPage;
        Assert.Same(newPage, iList[index]);
        Assert.Same(owner, page1.Parent);
        Assert.Same(owner, page2.Parent);
        Assert.Same(owner, page3.Parent);
        Assert.Null(newPage.Parent);
        Assert.Equal(new Control[] { page1, page2, page3 }, owner.Controls.Cast<Control>());
        Assert.True(page1.IsHandleCreated);
        Assert.False(page2.IsHandleCreated);
        Assert.False(page3.IsHandleCreated);
        Assert.False(newPage.IsHandleCreated);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, parentInvalidatedCallCount);
        Assert.Equal(0, parentStyleChangedCallCount);
        Assert.Equal(0, parentCreatedCallCount);
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void TabPageCollection_IListItem_SetValidIndexDesignModeWithHandle_ReturnsExpected(int index)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(null);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(true);
        using TabControl owner = new()
        {
            Site = mockSite.Object
        };
        using TabPage page1 = new();
        using TabPage page2 = new();
        using TabPage page3 = new();
        var collection = new TabControl.TabPageCollection(owner);
        IList iList = collection;
        iList.Add(page1);
        iList.Add(page2);
        iList.Add(page3);

        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int parentInvalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => parentInvalidatedCallCount++;
        int parentStyleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
        int parentCreatedCallCount = 0;
        owner.HandleCreated += (sender, e) => parentCreatedCallCount++;

        using TabPage newPage = new();
        iList[index] = newPage;
        Assert.Same(newPage, iList[index]);
        Assert.Same(owner, page1.Parent);
        Assert.Same(owner, page2.Parent);
        Assert.Same(owner, page3.Parent);
        Assert.Null(newPage.Parent);
        Assert.Equal(new Control[] { page1, page2, page3 }, owner.Controls.Cast<Control>());
        Assert.True(page1.IsHandleCreated);
        Assert.False(page2.IsHandleCreated);
        Assert.False(page3.IsHandleCreated);
        Assert.False(newPage.IsHandleCreated);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, parentInvalidatedCallCount);
        Assert.Equal(0, parentStyleChangedCallCount);
        Assert.Equal(0, parentCreatedCallCount);
    }

    public static IEnumerable<object[]> IListItem_InvalidValue_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new() };
        yield return new object[] { new Control() };
    }

    [WinFormsTheory]
    [MemberData(nameof(IListItem_InvalidValue_TestData))]
    public void TabPageCollection_IListItem_SetInvalidValue_ThrowsArgumentException(object value)
    {
        using TabControl owner = new();
        using TabPage page = new();
        var collection = new TabControl.TabPageCollection(owner);
        IList iList = collection;
        iList.Add(page);
        Assert.Throws<ArgumentException>(() => iList[0] = value);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void TabPageCollection_IListItem_SetInvalidIndexEmpty_ThrowsArgumentOutOfRangeException(int index)
    {
        using TabControl owner = new();
        using TabPage page = new();
        var collection = new TabControl.TabPageCollection(owner);
        IList iList = collection;
        Assert.Throws<ArgumentOutOfRangeException>("index", () => iList[index] = page);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(1)]
    [InlineData(2)]
    public void TabPageCollection_IListItem_SetInvalidIndexNotEmpty_ThrowsArgumentOutOfRangeException(int index)
    {
        using TabControl owner = new();
        using TabPage page1 = new();
        using TabPage page2 = new();
        var collection = new TabControl.TabPageCollection(owner);
        IList iList = collection;
        iList.Add(page1);
        Assert.Throws<ArgumentOutOfRangeException>("index", () => iList[index] = page2);
    }

    [WinFormsFact]
    public void TabPageCollection_Remove_InvokeValueWithoutHandleOwnerWithoutHandle_Success()
    {
        using TabControl owner = new()
        {
            Bounds = new Rectangle(0, 0, 400, 300)
        };
        using TabPage value1 = new();
        using TabPage value2 = new();
        var collection = new TabControl.TabPageCollection(owner)
        {
            value1,
            value2
        };

        int layoutCallCount1 = 0;
        value1.Layout += (sender, e) => layoutCallCount1++;
        int layoutCallCount2 = 0;
        value2.Layout += (sender, e) => layoutCallCount2++;
        int parentLayoutCallCount = 0;
        List<LayoutEventArgs> events = [];
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(owner, sender);
            events.Add(e);
            parentLayoutCallCount++;
        }

        owner.Layout += parentHandler;

        try
        {
            // Remove last.
            collection.Remove(value2);
            Assert.Same(value1, Assert.Single(collection));
            Assert.Same(value1, Assert.Single(owner.TabPages));
            Assert.Same(value1, Assert.Single(owner.Controls));
            Assert.Same(owner, value1.Parent);
            Assert.False(value1.Visible);
            Assert.Equal(new Rectangle(0, 0, 200, 100), value1.Bounds);
            Assert.Null(value1.Site);
            Assert.Null(value2.Parent);
            Assert.False(value2.Visible);
            Assert.Equal(new Rectangle(0, 0, 200, 100), value2.Bounds);
            Assert.Null(value2.Site);
            Assert.Equal(-1, owner.SelectedIndex);
            Assert.Equal(0, layoutCallCount1);
            Assert.Equal(0, layoutCallCount2);
            Assert.Equal(1, parentLayoutCallCount);
            Assert.False(value1.IsHandleCreated);
            Assert.False(value2.IsHandleCreated);
            Assert.False(owner.IsHandleCreated);

            // Remove again.
            collection.Remove(value2);
            Assert.Same(value1, Assert.Single(collection));
            Assert.Same(value1, Assert.Single(owner.TabPages));
            Assert.Same(value1, Assert.Single(owner.Controls));
            Assert.Same(owner, value1.Parent);
            Assert.False(value1.Visible);
            Assert.Equal(new Rectangle(0, 0, 200, 100), value1.Bounds);
            Assert.Null(value1.Site);
            Assert.Null(value2.Parent);
            Assert.False(value2.Visible);
            Assert.Equal(new Rectangle(0, 0, 200, 100), value2.Bounds);
            Assert.Null(value2.Site);
            Assert.Equal(-1, owner.SelectedIndex);
            Assert.Equal(0, layoutCallCount1);
            Assert.Equal(0, layoutCallCount2);
            Assert.Equal(1, parentLayoutCallCount);
            Assert.False(value1.IsHandleCreated);
            Assert.False(value2.IsHandleCreated);
            Assert.False(owner.IsHandleCreated);

            // Remove first.
            collection.Remove(value1);
            Assert.Empty(collection);
            Assert.Empty(owner.TabPages);
            Assert.Empty(owner.Controls);
            Assert.Null(value1.Parent);
            Assert.False(value1.Visible);
            Assert.Equal(new Rectangle(0, 0, 200, 100), value1.Bounds);
            Assert.Null(value1.Site);
            Assert.Null(value2.Parent);
            Assert.False(value2.Visible);
            Assert.Equal(new Rectangle(0, 0, 200, 100), value2.Bounds);
            Assert.Null(value2.Site);
            Assert.Equal(-1, owner.SelectedIndex);
            Assert.Equal(0, layoutCallCount1);
            Assert.Equal(0, layoutCallCount2);
            Assert.Equal(2, parentLayoutCallCount);
            Assert.False(value1.IsHandleCreated);
            Assert.False(value2.IsHandleCreated);
            Assert.False(owner.IsHandleCreated);
        }
        finally
        {
            owner.Layout -= parentHandler;
        }
    }

    [WinFormsFact]
    public void TabPageCollection_Remove_InvokeValueWithHandleOwnerWithoutHandle_Success()
    {
        using TabControl owner = new()
        {
            Bounds = new Rectangle(0, 0, 400, 300)
        };
        using TabPage value1 = new();
        using TabPage value2 = new();
        var collection = new TabControl.TabPageCollection(owner)
        {
            value1,
            value2
        };

        int layoutCallCount1 = 0;
        value1.Layout += (sender, e) => layoutCallCount1++;
        int layoutCallCount2 = 0;
        value2.Layout += (sender, e) => layoutCallCount2++;
        int parentLayoutCallCount = 0;
        List<LayoutEventArgs> events = [];
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(owner, sender);
            events.Add(e);
            parentLayoutCallCount++;
        }

        owner.Layout += parentHandler;
        Assert.NotEqual(IntPtr.Zero, value1.Handle);
        int invalidatedCallCount1 = 0;
        value1.Invalidated += (sender, e) => invalidatedCallCount1++;
        int styleChangedCallCount1 = 0;
        value1.StyleChanged += (sender, e) => styleChangedCallCount1++;
        int createdCallCount1 = 0;
        value1.HandleCreated += (sender, e) => createdCallCount1++;
        Assert.NotEqual(IntPtr.Zero, value2.Handle);
        int invalidatedCallCount2 = 0;
        value2.Invalidated += (sender, e) => invalidatedCallCount2++;
        int styleChangedCallCount2 = 0;
        value2.StyleChanged += (sender, e) => styleChangedCallCount2++;
        int createdCallCount2 = 0;
        value2.HandleCreated += (sender, e) => createdCallCount2++;

        try
        {
            // Remove last.
            collection.Remove(value2);
            Assert.Same(value1, Assert.Single(collection));
            Assert.Same(value1, Assert.Single(owner.TabPages));
            Assert.Same(value1, Assert.Single(owner.Controls));
            Assert.Same(owner, value1.Parent);
            Assert.False(value1.Visible);
            Assert.Equal(new Rectangle(0, 0, 200, 100), value1.Bounds);
            Assert.Null(value1.Site);
            Assert.Null(value2.Parent);
            Assert.False(value2.Visible);
            Assert.Equal(new Rectangle(0, 0, 200, 100), value2.Bounds);
            Assert.Null(value2.Site);
            Assert.Equal(-1, owner.SelectedIndex);
            Assert.Equal(0, layoutCallCount1);
            Assert.Equal(0, layoutCallCount2);
            Assert.Equal(1, parentLayoutCallCount);
            Assert.True(value1.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount1);
            Assert.Equal(0, styleChangedCallCount1);
            Assert.Equal(0, createdCallCount1);
            Assert.True(value2.IsHandleCreated);
            Assert.False(owner.IsHandleCreated);

            // Remove again.
            collection.Remove(value2);
            Assert.Same(value1, Assert.Single(collection));
            Assert.Same(value1, Assert.Single(owner.TabPages));
            Assert.Same(value1, Assert.Single(owner.Controls));
            Assert.Same(owner, value1.Parent);
            Assert.False(value1.Visible);
            Assert.Equal(new Rectangle(0, 0, 200, 100), value1.Bounds);
            Assert.Null(value1.Site);
            Assert.Null(value2.Parent);
            Assert.False(value2.Visible);
            Assert.Equal(new Rectangle(0, 0, 200, 100), value2.Bounds);
            Assert.Null(value2.Site);
            Assert.Equal(-1, owner.SelectedIndex);
            Assert.Equal(0, layoutCallCount1);
            Assert.Equal(0, layoutCallCount2);
            Assert.Equal(1, parentLayoutCallCount);
            Assert.True(value1.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount1);
            Assert.Equal(0, styleChangedCallCount1);
            Assert.Equal(0, createdCallCount1);
            Assert.True(value2.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount2);
            Assert.Equal(0, styleChangedCallCount2);
            Assert.Equal(0, createdCallCount2);
            Assert.False(owner.IsHandleCreated);

            // Remove first.
            collection.Remove(value1);
            Assert.Empty(collection);
            Assert.Empty(owner.TabPages);
            Assert.Empty(owner.Controls);
            Assert.Null(value1.Parent);
            Assert.False(value1.Visible);
            Assert.Equal(new Rectangle(0, 0, 200, 100), value1.Bounds);
            Assert.Null(value1.Site);
            Assert.Null(value2.Parent);
            Assert.False(value2.Visible);
            Assert.Equal(new Rectangle(0, 0, 200, 100), value2.Bounds);
            Assert.Null(value2.Site);
            Assert.Equal(-1, owner.SelectedIndex);
            Assert.Equal(0, layoutCallCount1);
            Assert.Equal(0, layoutCallCount2);
            Assert.Equal(2, parentLayoutCallCount);
            Assert.True(value1.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount1);
            Assert.Equal(0, styleChangedCallCount1);
            Assert.Equal(0, createdCallCount1);
            Assert.True(value2.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount2);
            Assert.Equal(0, styleChangedCallCount2);
            Assert.Equal(0, createdCallCount2);
            Assert.False(owner.IsHandleCreated);
        }
        finally
        {
            owner.Layout -= parentHandler;
        }
    }

    [WinFormsFact]
    public void TabPageCollection_Remove_InvokeValueWithoutHandleOwnerWithHandle_Success()
    {
        using TabControl owner = new()
        {
            Bounds = new Rectangle(0, 0, 400, 300)
        };
        using TabPage value1 = new();
        using TabPage value2 = new();
        var collection = new TabControl.TabPageCollection(owner)
        {
            value1,
            value2
        };

        int layoutCallCount1 = 0;
        value1.Layout += (sender, e) => layoutCallCount1++;
        int layoutCallCount2 = 0;
        value2.Layout += (sender, e) => layoutCallCount2++;
        int parentLayoutCallCount = 0;
        List<LayoutEventArgs> events = [];
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(owner, sender);
            events.Add(e);
            parentLayoutCallCount++;
        }

        owner.Layout += parentHandler;
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int parentInvalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => parentInvalidatedCallCount++;
        int parentStyleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
        int parentCreatedCallCount = 0;
        owner.HandleCreated += (sender, e) => parentCreatedCallCount++;

        try
        {
            // Remove last.
            collection.Remove(value2);
            Assert.Same(value1, Assert.Single(collection));
            Assert.Same(value1, Assert.Single(owner.TabPages));
            Assert.Same(value1, Assert.Single(owner.Controls));
            Assert.Same(owner, value1.Parent);
            Assert.True(value1.Visible);
            Assert.Equal(new Rectangle(4, 24, 392, 272), value1.Bounds);
            Assert.Null(value1.Site);
            Assert.Null(value2.Parent);
            Assert.False(value2.Visible);
            Assert.Equal(new Rectangle(4, 24, 392, 272), value2.Bounds);
            Assert.Null(value2.Site);
            Assert.Equal(0, owner.SelectedIndex);
            Assert.Equal(2, layoutCallCount1);
            Assert.Equal(1, layoutCallCount2);
            Assert.Equal(6, parentLayoutCallCount);
            Assert.True(value1.IsHandleCreated);
            Assert.False(value2.IsHandleCreated);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);

            // Remove again.
            collection.Remove(value2);
            Assert.Same(value1, Assert.Single(collection));
            Assert.Same(value1, Assert.Single(owner.TabPages));
            Assert.Same(value1, Assert.Single(owner.Controls));
            Assert.Same(owner, value1.Parent);
            Assert.True(value1.Visible);
            Assert.Equal(new Rectangle(4, 24, 392, 272), value1.Bounds);
            Assert.Null(value1.Site);
            Assert.Null(value2.Parent);
            Assert.False(value2.Visible);
            Assert.Equal(new Rectangle(4, 24, 392, 272), value2.Bounds);
            Assert.Null(value2.Site);
            Assert.Equal(0, owner.SelectedIndex);
            Assert.Equal(2, layoutCallCount1);
            Assert.Equal(1, layoutCallCount2);
            Assert.Equal(6, parentLayoutCallCount);
            Assert.True(value1.IsHandleCreated);
            Assert.False(value2.IsHandleCreated);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);

            // Remove first.
            collection.Remove(value1);
            Assert.Empty(collection);
            Assert.Empty(owner.TabPages);
            Assert.Empty(owner.Controls);
            Assert.Null(value1.Parent);
            Assert.True(value1.Visible);
            Assert.Equal(new Rectangle(4, 24, 392, 272), value1.Bounds);
            Assert.Null(value1.Site);
            Assert.Null(value2.Parent);
            Assert.False(value2.Visible);
            Assert.Equal(new Rectangle(4, 24, 392, 272), value2.Bounds);
            Assert.Null(value2.Site);
            Assert.Equal(-1, owner.SelectedIndex);
            Assert.Equal(3, layoutCallCount1);
            Assert.Equal(1, layoutCallCount2);
            Assert.Equal(7, parentLayoutCallCount);
            Assert.True(value1.IsHandleCreated);
            Assert.False(value2.IsHandleCreated);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);
        }
        finally
        {
            owner.Layout -= parentHandler;
        }
    }

    [WinFormsFact]
    public void TabPageCollection_Remove_InvokeValueWithHandleOwnerWithHandle_Success()
    {
        using TabControl owner = new()
        {
            Bounds = new Rectangle(0, 0, 400, 300)
        };
        using TabPage value1 = new();
        using TabPage value2 = new();
        var collection = new TabControl.TabPageCollection(owner)
        {
            value1,
            value2
        };

        int layoutCallCount1 = 0;
        value1.Layout += (sender, e) => layoutCallCount1++;
        int layoutCallCount2 = 0;
        value2.Layout += (sender, e) => layoutCallCount2++;
        int parentLayoutCallCount = 0;
        List<LayoutEventArgs> events = [];
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(owner, sender);
            events.Add(e);
            parentLayoutCallCount++;
        }

        owner.Layout += parentHandler;
        Assert.NotEqual(IntPtr.Zero, value1.Handle);
        int invalidatedCallCount1 = 0;
        value1.Invalidated += (sender, e) => invalidatedCallCount1++;
        int styleChangedCallCount1 = 0;
        value1.StyleChanged += (sender, e) => styleChangedCallCount1++;
        int createdCallCount1 = 0;
        value1.HandleCreated += (sender, e) => createdCallCount1++;
        Assert.NotEqual(IntPtr.Zero, value2.Handle);
        int invalidatedCallCount2 = 0;
        value2.Invalidated += (sender, e) => invalidatedCallCount2++;
        int styleChangedCallCount2 = 0;
        value2.StyleChanged += (sender, e) => styleChangedCallCount2++;
        int createdCallCount2 = 0;
        value2.HandleCreated += (sender, e) => createdCallCount2++;

        try
        {
            // Remove last.
            collection.Remove(value2);
            Assert.Same(value1, Assert.Single(collection));
            Assert.Same(value1, Assert.Single(owner.TabPages));
            Assert.Same(value1, Assert.Single(owner.Controls));
            Assert.Same(owner, value1.Parent);
            Assert.False(value1.Visible);
            Assert.Equal(new Rectangle(0, 0, 200, 100), value1.Bounds);
            Assert.Null(value1.Site);
            Assert.Null(value2.Parent);
            Assert.False(value2.Visible);
            Assert.Equal(new Rectangle(0, 0, 200, 100), value2.Bounds);
            Assert.Null(value2.Site);
            Assert.Equal(-1, owner.SelectedIndex);
            Assert.Equal(0, layoutCallCount1);
            Assert.Equal(0, layoutCallCount2);
            Assert.Equal(1, parentLayoutCallCount);
            Assert.True(value1.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount1);
            Assert.Equal(0, styleChangedCallCount1);
            Assert.Equal(0, createdCallCount1);
            Assert.True(value2.IsHandleCreated);
            Assert.False(owner.IsHandleCreated);

            // Remove again.
            collection.Remove(value2);
            Assert.Same(value1, Assert.Single(collection));
            Assert.Same(value1, Assert.Single(owner.TabPages));
            Assert.Same(value1, Assert.Single(owner.Controls));
            Assert.Same(owner, value1.Parent);
            Assert.False(value1.Visible);
            Assert.Equal(new Rectangle(0, 0, 200, 100), value1.Bounds);
            Assert.Null(value1.Site);
            Assert.Null(value2.Parent);
            Assert.False(value2.Visible);
            Assert.Equal(new Rectangle(0, 0, 200, 100), value2.Bounds);
            Assert.Null(value2.Site);
            Assert.Equal(-1, owner.SelectedIndex);
            Assert.Equal(0, layoutCallCount1);
            Assert.Equal(0, layoutCallCount2);
            Assert.Equal(1, parentLayoutCallCount);
            Assert.True(value1.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount1);
            Assert.Equal(0, styleChangedCallCount1);
            Assert.Equal(0, createdCallCount1);
            Assert.True(value2.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount2);
            Assert.Equal(0, styleChangedCallCount2);
            Assert.Equal(0, createdCallCount2);
            Assert.False(owner.IsHandleCreated);

            // Remove first.
            collection.Remove(value1);
            Assert.Empty(collection);
            Assert.Empty(owner.TabPages);
            Assert.Empty(owner.Controls);
            Assert.Null(value1.Parent);
            Assert.False(value1.Visible);
            Assert.Equal(new Rectangle(0, 0, 200, 100), value1.Bounds);
            Assert.Null(value1.Site);
            Assert.Null(value2.Parent);
            Assert.False(value2.Visible);
            Assert.Equal(new Rectangle(0, 0, 200, 100), value2.Bounds);
            Assert.Null(value2.Site);
            Assert.Equal(-1, owner.SelectedIndex);
            Assert.Equal(0, layoutCallCount1);
            Assert.Equal(0, layoutCallCount2);
            Assert.Equal(2, parentLayoutCallCount);
            Assert.True(value1.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount1);
            Assert.Equal(0, styleChangedCallCount1);
            Assert.Equal(0, createdCallCount1);
            Assert.True(value2.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount2);
            Assert.Equal(0, styleChangedCallCount2);
            Assert.Equal(0, createdCallCount2);
            Assert.False(owner.IsHandleCreated);
        }
        finally
        {
            owner.Layout -= parentHandler;
        }
    }

    [WinFormsFact]
    public void TabPageCollection_Remove_SelectedTabWithoutHandle_ThrowsArgumentOutOfRangeException()
    {
        using TabControl owner = new();
        using TabPage value1 = new();
        using TabPage value2 = new();
        using TabPage value3 = new();
        using TabPage value4 = new();
        var collection = new TabControl.TabPageCollection(owner)
        {
            value1,
            value2,
            value3,
            value4
        };
        owner.SelectedTab = value4;
        Assert.Same(value4, owner.SelectedTab);

        // Remove other.
        collection.Remove(value2);
        Assert.Equal(new Control[] { value1, value3, value4 }, collection.Cast<TabPage>());
        Assert.Throws<ArgumentOutOfRangeException>(() => owner.SelectedTab);

        // Remove selected.
        collection.Remove(value4);
        Assert.Equal(new Control[] { value1, value3 }, collection.Cast<TabPage>());
        Assert.Throws<ArgumentOutOfRangeException>(() => owner.SelectedTab);

        // Remove selected again.
        collection.Remove(value1);
        Assert.Equal(new Control[] { value3 }, collection.Cast<TabPage>());
        Assert.Throws<ArgumentOutOfRangeException>(() => owner.SelectedTab);

        // Remove selected again.
        collection.Remove(value3);
        Assert.Empty(collection);
        Assert.Null(owner.SelectedTab);
    }

    [WinFormsFact]
    public void TabPageCollection_Remove_SelectedTabWithHandle_SetsSelectedToZero()
    {
        using TabControl owner = new();
        using TabPage value1 = new();
        using TabPage value2 = new();
        using TabPage value3 = new();
        using TabPage value4 = new();
        var collection = new TabControl.TabPageCollection(owner)
        {
            value1,
            value2,
            value3,
            value4
        };
        owner.SelectedTab = value4;
        Assert.Same(value4, owner.SelectedTab);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);

        // Remove other.
        collection.Remove(value2);
        Assert.Equal(new Control[] { value1, value3, value4 }, collection.Cast<TabPage>());
        Assert.Same(value4, owner.SelectedTab);

        // Remove selected.
        collection.Remove(value4);
        Assert.Equal(new Control[] { value1, value3 }, collection.Cast<TabPage>());
        Assert.Same(value1, owner.SelectedTab);

        // Remove selected again.
        collection.Remove(value1);
        Assert.Equal(new Control[] { value3 }, collection.Cast<TabPage>());
        Assert.Same(value3, owner.SelectedTab);

        // Remove selected again.
        collection.Remove(value3);
        Assert.Empty(collection);
        Assert.Null(owner.SelectedTab);
    }

    [WinFormsFact]
    public void TabPageCollection_Remove_ManyControls_Success()
    {
        using TabControl owner = new();
        var collection = new TabControl.TabPageCollection(owner);

        List<TabPage> items = [];
        for (int i = 0; i < 24; i++)
        {
            TabPage value = new();
            items.Add(value);
            collection.Add(value);
            Assert.Equal(items, collection.Cast<TabPage>());
            Assert.Equal(items, owner.TabPages.Cast<TabPage>());
            Assert.Same(owner, value.Parent);
        }

        for (int i = 0; i < 24; i++)
        {
            items.RemoveAt(0);
            collection.Remove(collection[0]);
            Assert.Equal(items, collection.Cast<TabPage>());
            Assert.Equal(items, owner.TabPages.Cast<TabPage>());
        }
    }

    [WinFormsFact]
    public void TabPageCollection_Remove_NoSuchValueEmpty_Nop()
    {
        using TabControl owner = new();
        var collection = new TabControl.TabPageCollection(owner);
        using TabPage value = new();
        collection.Remove(value);
    }

    [WinFormsFact]
    public void TabPageCollection_Remove_NoSuchValueNotEmpty_Nop()
    {
        using TabControl owner = new();
        using TabPage page1 = new();
        using TabPage page2 = new();
        var collection = new TabControl.TabPageCollection(owner)
        {
            page1,
            page1
        };
        using TabPage value = new();
        collection.Remove(value);
    }

    [WinFormsFact]
    public unsafe void TabControlControlCollection_Remove_GetItemsWithHandle_Success()
    {
        using TabControl owner = new();
        using TabPage page1 = new();
        using TabPage page2 = new()
        {
            Text = "Text",
            ImageIndex = 1
        };
        using NullTextTabPage page3 = new();
        var collection = new TabControl.TabPageCollection(owner)
        {
            page1,
            page2,
            page3
        };

        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        collection.Remove(page2);
        Assert.Equal(2, (int)PInvokeCore.SendMessage(owner, PInvoke.TCM_GETITEMCOUNT));

        char* buffer = stackalloc char[256];
        TCITEMW item = default;
        item.cchTextMax = int.MaxValue;
        item.pszText = buffer;
        item.dwStateMask = (TAB_CONTROL_ITEM_STATE)uint.MaxValue;
        item.mask = (TCITEMHEADERA_MASK)uint.MaxValue;

        // Get item 0.
        Assert.Equal(1, (int)PInvokeCore.SendMessage(owner, PInvoke.TCM_GETITEMW, 0, ref item));
        Assert.Equal(TAB_CONTROL_ITEM_STATE.TCIS_BUTTONPRESSED, item.dwState);
        Assert.Equal(IntPtr.Zero, (nint)item.lParam);
        Assert.Equal(int.MaxValue, item.cchTextMax);
        Assert.Empty(new string(item.pszText));
        Assert.Equal(-1, item.iImage);

        // Get item 2.
        Assert.Equal(1, (int)PInvokeCore.SendMessage(owner, PInvoke.TCM_GETITEMW, 1, ref item));
        Assert.Equal((TAB_CONTROL_ITEM_STATE)0, item.dwState);
        Assert.Equal(IntPtr.Zero, (nint)item.lParam);
        Assert.Equal(int.MaxValue, item.cchTextMax);
        Assert.Empty(new string(item.pszText));
        Assert.Equal(-1, item.iImage);
    }

    [WinFormsFact]
    public unsafe void TabControlControlCollection_Remove_GetItemsDesignModeWithHandle_Success()
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(null);
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(true);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        using TabControl owner = new()
        {
            Site = mockSite.Object
        };

        using TabPage page1 = new();
        using TabPage page2 = new()
        {
            Text = "Text",
            ImageIndex = 1
        };
        using NullTextTabPage page3 = new();
        var collection = new TabControl.TabPageCollection(owner)
        {
            page1,
            page2,
            page3
        };

        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        collection.Remove(page2);
        Assert.Equal(2, (int)PInvokeCore.SendMessage(owner, PInvoke.TCM_GETITEMCOUNT));

        char* buffer = stackalloc char[256];
        TCITEMW item = default;
        item.cchTextMax = int.MaxValue;
        item.pszText = buffer;
        item.dwStateMask = (TAB_CONTROL_ITEM_STATE)uint.MaxValue;
        item.mask = (TCITEMHEADERA_MASK)uint.MaxValue;

        // Get item 0.
        Assert.Equal(1, (int)PInvokeCore.SendMessage(owner, PInvoke.TCM_GETITEMW, 0, ref item));
        Assert.Equal(TAB_CONTROL_ITEM_STATE.TCIS_BUTTONPRESSED, item.dwState);
        Assert.Equal(IntPtr.Zero, (nint)item.lParam);
        Assert.Equal(int.MaxValue, item.cchTextMax);
        Assert.Empty(new string(item.pszText));
        Assert.Equal(-1, item.iImage);

        // Get item 1.
        Assert.Equal(1, (int)PInvokeCore.SendMessage(owner, PInvoke.TCM_GETITEMW, 1, ref item));
        Assert.Equal((TAB_CONTROL_ITEM_STATE)0, item.dwState);
        Assert.Equal(IntPtr.Zero, (nint)item.lParam);
        Assert.Equal(int.MaxValue, item.cchTextMax);
        Assert.Empty(new string(item.pszText));
        Assert.Equal(-1, item.iImage);
    }

    [WinFormsFact]
    public void TabPageCollection_Remove_NullValue_ThrowsArgumentNullException()
    {
        using TabControl owner = new();
        var collection = new TabControl.TabPageCollection(owner);
        Assert.Throws<ArgumentNullException>("value", () => collection.Remove(null));
    }

    [WinFormsFact]
    public void TabPageCollection_IListRemove_InvokeValueWithoutHandleOwnerWithoutHandle_Success()
    {
        using TabControl owner = new()
        {
            Bounds = new Rectangle(0, 0, 400, 300)
        };
        using TabPage value1 = new();
        using TabPage value2 = new();
        var collection = new TabControl.TabPageCollection(owner);
        IList iList = collection;
        iList.Add(value1);
        iList.Add(value2);

        int layoutCallCount1 = 0;
        value1.Layout += (sender, e) => layoutCallCount1++;
        int layoutCallCount2 = 0;
        value2.Layout += (sender, e) => layoutCallCount2++;
        int parentLayoutCallCount = 0;
        List<LayoutEventArgs> events = [];
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(owner, sender);
            events.Add(e);
            parentLayoutCallCount++;
        }

        owner.Layout += parentHandler;

        try
        {
            // Remove last.
            iList.Remove(value2);
            Assert.Same(value1, Assert.Single(collection));
            Assert.Same(value1, Assert.Single(owner.TabPages));
            Assert.Same(owner, value1.Parent);
            Assert.False(value1.Visible);
            Assert.Equal(new Rectangle(0, 0, 200, 100), value1.Bounds);
            Assert.Null(value1.Site);
            Assert.Null(value2.Parent);
            Assert.False(value2.Visible);
            Assert.Equal(new Rectangle(0, 0, 200, 100), value2.Bounds);
            Assert.Null(value2.Site);
            Assert.Equal(-1, owner.SelectedIndex);
            Assert.Equal(0, layoutCallCount1);
            Assert.Equal(0, layoutCallCount2);
            Assert.Equal(1, parentLayoutCallCount);
            Assert.False(value1.IsHandleCreated);
            Assert.False(value2.IsHandleCreated);
            Assert.False(owner.IsHandleCreated);

            // Remove again.
            iList.Remove(value2);
            Assert.Same(value1, Assert.Single(collection));
            Assert.Same(value1, Assert.Single(owner.TabPages));
            Assert.Same(owner, value1.Parent);
            Assert.False(value1.Visible);
            Assert.Equal(new Rectangle(0, 0, 200, 100), value1.Bounds);
            Assert.Null(value1.Site);
            Assert.Null(value2.Parent);
            Assert.False(value2.Visible);
            Assert.Equal(new Rectangle(0, 0, 200, 100), value2.Bounds);
            Assert.Null(value2.Site);
            Assert.Equal(-1, owner.SelectedIndex);
            Assert.Equal(0, layoutCallCount1);
            Assert.Equal(0, layoutCallCount2);
            Assert.Equal(1, parentLayoutCallCount);
            Assert.False(value1.IsHandleCreated);
            Assert.False(value2.IsHandleCreated);
            Assert.False(owner.IsHandleCreated);

            // Remove first.
            iList.Remove(value1);
            Assert.Empty(collection);
            Assert.Empty(owner.TabPages);
            Assert.Null(value1.Parent);
            Assert.False(value1.Visible);
            Assert.Equal(new Rectangle(0, 0, 200, 100), value1.Bounds);
            Assert.Null(value1.Site);
            Assert.Null(value2.Parent);
            Assert.False(value2.Visible);
            Assert.Equal(new Rectangle(0, 0, 200, 100), value2.Bounds);
            Assert.Null(value2.Site);
            Assert.Equal(-1, owner.SelectedIndex);
            Assert.Equal(0, layoutCallCount1);
            Assert.Equal(0, layoutCallCount2);
            Assert.Equal(2, parentLayoutCallCount);
            Assert.False(value1.IsHandleCreated);
            Assert.False(value2.IsHandleCreated);
            Assert.False(owner.IsHandleCreated);
        }
        finally
        {
            owner.Layout -= parentHandler;
        }
    }

    public static IEnumerable<object[]> IListRemove_InvalidValue_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new() };
        yield return new object[] { new Control() };
    }

    [WinFormsTheory]
    [MemberData(nameof(IListRemove_InvalidValue_TestData))]
    public void TabPageCollection_IListRemove_InvalidValue_Nop(object value)
    {
        using TabControl owner = new();
        var collection = new TabControl.TabPageCollection(owner);
        IList iList = collection;
        iList.Remove(value);
    }

    [WinFormsFact]
    public void TabPageCollection_RemoveAt_Invoke_Success()
    {
        using TabControl owner = new();
        using TabPage child1 = new();
        using TabPage child2 = new();
        var collection = new TabControl.TabPageCollection(owner)
        {
            child1,
            child2
        };

        int layoutCallCount = 0;
        child1.Layout += (sender, e) => layoutCallCount++;
        int parentLayoutCallCount = 0;
        object affectedControl = null;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(owner, sender);
            Assert.Same(affectedControl, e.AffectedControl);
            Assert.Equal("Parent", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        owner.Layout += parentHandler;

        try
        {
            affectedControl = child2;
            collection.RemoveAt(1);
            Assert.Equal(new TabPage[] { child1 }, collection.Cast<TabPage>());
            Assert.Same(owner, child1.Parent);
            Assert.Null(child2.Parent);
            Assert.Equal(1, parentLayoutCallCount);
            Assert.False(owner.IsHandleCreated);
            Assert.False(child1.IsHandleCreated);
            Assert.False(child2.IsHandleCreated);

            // Remove again.
            affectedControl = child1;
            collection.RemoveAt(0);
            Assert.Empty(collection);
            Assert.Null(child1.Parent);
            Assert.Null(child2.Parent);
            Assert.Equal(2, parentLayoutCallCount);
            Assert.False(owner.IsHandleCreated);
            Assert.False(child1.IsHandleCreated);
            Assert.False(child2.IsHandleCreated);
        }
        finally
        {
            owner.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void TabPageCollection_RemoveAt_InvalidIndexEmpty_ThrowsArgumentOutOfRangeException(int index)
    {
        using TabControl owner = new();
        var collection = new TabControl.TabPageCollection(owner);
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.RemoveAt(index));
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(1)]
    [InlineData(2)]
    public void TabPageCollection_RemoveAtInvalidIndexNotEmpty_ThrowsArgumentOutOfRangeException(int index)
    {
        using TabControl owner = new();
        using TabPage child = new();
        var collection = new TabControl.TabPageCollection(owner)
        {
            child
        };
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.RemoveAt(index));
    }

    [WinFormsTheory]
    [InlineData("name2")]
    [InlineData("NAME2")]
    public void TabPageCollection_RemoveByKey_InvokeValidKey_ReturnsExpected(string key)
    {
        using TabControl owner = new();
        using TabPage child1 = new()
        {
            Name = "name1"
        };
        using TabPage child2 = new()
        {
            Name = "name2"
        };
        using TabPage child3 = new()
        {
            Name = "name2"
        };
        var collection = new TabControl.TabPageCollection(owner)
        {
            child1,
            child2,
            child3
        };

        int layoutCallCount = 0;
        child2.Layout += (sender, e) => layoutCallCount++;
        int parentLayoutCallCount = 0;
        object affectedControl = null;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(owner, sender);
            Assert.Same(affectedControl, e.AffectedControl);
            Assert.Equal("Parent", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        owner.Layout += parentHandler;

        try
        {
            affectedControl = child2;
            collection.RemoveByKey(key);
            Assert.Equal(new TabPage[] { child1, child3 }, collection.Cast<TabPage>());
            Assert.Same(owner, child1.Parent);
            Assert.Null(child2.Parent);
            Assert.Same(owner, child3.Parent);
            Assert.Equal(1, parentLayoutCallCount);
            Assert.False(owner.IsHandleCreated);
            Assert.False(child1.IsHandleCreated);
            Assert.False(child2.IsHandleCreated);
            Assert.False(child3.IsHandleCreated);

            // Remove again.
            affectedControl = child3;
            collection.RemoveByKey(key);
            Assert.Equal(new TabPage[] { child1 }, collection.Cast<TabPage>());
            Assert.Same(owner, child1.Parent);
            Assert.Null(child2.Parent);
            Assert.Null(child3.Parent);
            Assert.Equal(2, parentLayoutCallCount);
            Assert.False(owner.IsHandleCreated);
            Assert.False(child1.IsHandleCreated);
            Assert.False(child2.IsHandleCreated);
            Assert.False(child3.IsHandleCreated);
        }
        finally
        {
            owner.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("NoSuchName")]
    [InlineData("abcd")]
    [InlineData("abcde")]
    [InlineData("abcdef")]
    public void TabPageCollection_RemoveByKey_InvokeNoSuchKey_ReturnsNull(string key)
    {
        using TabControl owner = new();
        using TabPage child1 = new()
        {
            Name = "name1"
        };
        using TabPage child2 = new()
        {
            Name = "name2"
        };
        using TabPage child3 = new()
        {
            Name = "name2"
        };
        var collection = new TabControl.TabPageCollection(owner)
        {
            child1,
            child2,
            child3
        };

        collection.RemoveByKey(key);
        Assert.Equal(new TabPage[] { child1, child2, child3 }, collection.Cast<TabPage>());

        // Call again.
        collection.RemoveByKey(key);
        Assert.Equal(new TabPage[] { child1, child2, child3 }, collection.Cast<TabPage>());
    }

    [WinFormsFact]
    public void TabPageCollection_Insert_By_Index()
    {
        using TabControl TabControl = new();

        using TabPage page1 = new();
        using TabPage page2 = new();
        using TabPage page3 = new();

        page1.Text = "First works";
        TabControl.TabPages.Add(page1);

        page2.Text = "Second works";
        TabControl.TabPages.Insert(1, page2);
        Assert.Equal(2, TabControl.TabPages.Count);
        Assert.Equal(page2, TabControl.TabPages[1]);
        Assert.Equal(page2, TabControl.Controls[1]);

        page3.Text = "Third works";
        TabControl.TabPages.Insert(1, page3);
        Assert.Equal(3, TabControl.TabPages.Count);
        Assert.Equal(page3, TabControl.TabPages[1]);
        Assert.Equal(page3, TabControl.Controls[1]);
        Assert.Equal(page2, TabControl.TabPages[2]);
        Assert.Equal(page2, TabControl.Controls[2]);
    }

    [WinFormsFact]
    public void TabPageCollection_Insert_First_item()
    {
        using TabControl TabControl = new();

        using TabPage page1 = new();
        using TabPage page2 = new();
        using TabPage page3 = new();

        page1.Text = "First works";
        TabControl.TabPages.Insert(0, page1);
        Assert.Single(TabControl.TabPages);
        Assert.Equal(page1, TabControl.TabPages[0]);
        Assert.Equal(page1, TabControl.Controls[0]);

        page2.Text = "Second works";
        TabControl.TabPages.Insert(1, page2);
        Assert.Equal(2, TabControl.TabPages.Count);
        Assert.Equal(page2, TabControl.TabPages[1]);
        Assert.Equal(page2, TabControl.Controls[1]);

        page3.Text = "Third works";
        TabControl.TabPages.Insert(1, page3);
        Assert.Equal(3, TabControl.TabPages.Count);
        Assert.Equal(page3, TabControl.TabPages[1]);
        Assert.Equal(page3, TabControl.Controls[1]);
        Assert.Equal(page2, TabControl.TabPages[2]);
        Assert.Equal(page2, TabControl.Controls[2]);
    }

    private class NullGetItemsTabControl : TabControl
    {
        protected override object[] GetItems() => null;
    }

    private class InvalidGetItemsTabControl : TabControl
    {
        protected override object[] GetItems() => [1];
    }

    private class NullTextTabPage : TabPage
    {
        public override string Text
        {
            get => null;
            set { }
        }
    }
}
