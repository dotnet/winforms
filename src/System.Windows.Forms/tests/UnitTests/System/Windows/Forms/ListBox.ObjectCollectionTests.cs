// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;

namespace System.Windows.Forms.Tests;

public class ListBoxObjectCollectionTests
{
    [WinFormsFact]
    public void ListBoxObjectCollection_Ctor_ListBox()
    {
        using ListBox owner = new();
        var collection = new ListBox.ObjectCollection(owner);
        Assert.Empty(collection);
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.False(collection.IsReadOnly);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_Ctor_ListBox_ObjectArray()
    {
        using ListBox owner = new();
        var collection = new ListBox.ObjectCollection(owner, (object[])[3, 2, 1]);
        Assert.Equal(3, collection.Count);
        Assert.Equal(new object[] { 3, 2, 1 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.False(collection.IsReadOnly);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_Ctor_ListBox_ObjectCollection()
    {
        using ListBox otherOwner = new();
        var otherCollection = new ListBox.ObjectCollection(otherOwner, (object[])[3, 2, 1]);
        using ListBox owner = new();
        var collection = new ListBox.ObjectCollection(owner, otherCollection);
        Assert.Equal(3, collection.Count);
        Assert.Equal(new object[] { 3, 2, 1 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.False(collection.IsReadOnly);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_Ctor_NullOwner_ThrowsArgumentNullException()
    {
        using ListBox owner = new();
        Assert.Throws<ArgumentNullException>("owner", () => new ListBox.ObjectCollection(null));
        Assert.Throws<ArgumentNullException>("owner", () => new ListBox.ObjectCollection(null, new ListBox.ObjectCollection(owner)));
        Assert.Throws<ArgumentNullException>("owner", () => new ListBox.ObjectCollection(null, Array.Empty<object>()));
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_Ctor_OwnerHasDataSource_ThrowsArgumentException()
    {
        using ListBox owner = new()
        {
            DataSource = Array.Empty<object>()
        };
        using ListBox otherOwner = new();
        var otherCollection = new ListBox.ObjectCollection(otherOwner);

        var emptyCollection = new ListBox.ObjectCollection(owner);
        Assert.Empty(emptyCollection);
        Assert.Empty(emptyCollection);
        Assert.Empty(emptyCollection);
        Assert.Empty(owner.Items);
        Assert.False(emptyCollection.IsReadOnly);
        Assert.Throws<ArgumentException>(() => new ListBox.ObjectCollection(owner, Array.Empty<object>()));
        Assert.Throws<ArgumentException>(() => new ListBox.ObjectCollection(owner, otherCollection));
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_Ctor_NullValue_ThrowsArgumentNullException()
    {
        using ListBox owner = new();
        Assert.Throws<ArgumentNullException>("value", () => new ListBox.ObjectCollection(owner, (object[])null));
        Assert.Throws<ArgumentNullException>("value", () => new ListBox.ObjectCollection(owner, (ListBox.ObjectCollection)null));
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_ICollection_Properties_GetReturnsExpected()
    {
        using ListBox owner = new();
        ICollection collection = new ListBox.ObjectCollection(owner);
        Assert.Empty(collection);
        Assert.False(collection.IsSynchronized);
        Assert.Same(collection, collection.SyncRoot);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IList_Properties_GetReturnsExpected()
    {
        using ListBox owner = new();
        IList collection = new ListBox.ObjectCollection(owner);
        Assert.Empty(collection);
        Assert.False(collection.IsFixedSize);
        Assert.False(collection.IsReadOnly);
        Assert.False(collection.IsSynchronized);
        Assert.Same(collection, collection.SyncRoot);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_Item_Get_ReturnsExpected()
    {
        using ListBox owner = new();
        var collection = new ListBox.ObjectCollection(owner);
        collection.AddRange((object[])[2, 1, 1, 3]);

        Assert.Equal(2, collection[0]);
        Assert.Equal(1, collection[1]);
        Assert.Equal(1, collection[2]);
        Assert.Equal(3, collection[3]);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_Item_GetSorted_ReturnsExpected()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        var collection = new ListBox.ObjectCollection(owner);
        collection.AddRange((object[])[2, 1, 1, 3]);

        Assert.Equal(1, collection[0]);
        Assert.Equal(1, collection[1]);
        Assert.Equal(2, collection[2]);
        Assert.Equal(3, collection[3]);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void ListBoxObjectCollection_Item_GetInvalidIndexEmpty_ThrowsArgumentOutOfRangeException(int index)
    {
        using ListBox owner = new();
        var collection = new ListBox.ObjectCollection(owner);
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index]);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(1)]
    [InlineData(2)]
    public void ListBoxObjectCollection_Item_GetInvalidIndexNotEmpty_ThrowsArgumentOutOfRangeException(int index)
    {
        using ListBox owner = new();
        var collection = new ListBox.ObjectCollection(owner)
        {
            1
        };
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index]);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_Item_SetNotSorted_GetReturnsExpected()
    {
        using ListBox owner = new();
        var collection = new ListBox.ObjectCollection(owner);
        collection.AddRange((object[])[2, 1, 3]);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Set first.
        collection[0] = 4;
        Assert.Equal(new int[] { 4, 1, 3 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Set middle.
        collection[1] = 2;
        Assert.Equal(new int[] { 4, 2, 3 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Set last.
        collection[2] = 5;
        Assert.Equal(new int[] { 4, 2, 5 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Set same.
        collection[2] = 5;
        Assert.Equal(new int[] { 4, 2, 5 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_Item_SetSorted_GetReturnsExpected()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        var collection = new ListBox.ObjectCollection(owner);
        collection.AddRange((object[])[2, 1, 3]);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Set first.
        collection[0] = 4;
        Assert.Equal(new int[] { 4, 2, 3 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Set middle.
        collection[1] = 1;
        Assert.Equal(new int[] { 4, 1, 3 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Set last.
        collection[2] = 5;
        Assert.Equal(new int[] { 4, 1, 5 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Set same.
        collection[2] = 5;
        Assert.Equal(new int[] { 4, 1, 5 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_Item_SetItems_GetReturnsExpected()
    {
        using ListBox owner = new();
        ListBox.ObjectCollection collection = owner.Items;
        collection.AddRange((object[])[2, 1, 3]);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Set first.
        collection[0] = 4;
        Assert.Equal(new int[] { 4, 1, 3 }, collection.Cast<int>());
        Assert.Equal(new int[] { 4, 1, 3 }, owner.Items.Cast<int>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Set middle.
        collection[1] = 2;
        Assert.Equal(new int[] { 4, 2, 3 }, collection.Cast<int>());
        Assert.Equal(new int[] { 4, 2, 3 }, owner.Items.Cast<int>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Set last.
        collection[2] = 5;
        Assert.Equal(new int[] { 4, 2, 5 }, collection.Cast<int>());
        Assert.Equal(new int[] { 4, 2, 5 }, owner.Items.Cast<int>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Set same.
        collection[2] = 5;
        Assert.Equal(new int[] { 4, 2, 5 }, collection.Cast<int>());
        Assert.Equal(new int[] { 4, 2, 5 }, owner.Items.Cast<int>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_Item_SetItemsSorted_GetReturnsExpected()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        ListBox.ObjectCollection collection = owner.Items;
        collection.AddRange((object[])[2, 1, 3]);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Set first.
        collection[0] = 4;
        Assert.Equal(new int[] { 4, 2, 3 }, collection.Cast<int>());
        Assert.Equal(new int[] { 4, 2, 3 }, owner.Items.Cast<int>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Set middle.
        collection[1] = 1;
        Assert.Equal(new int[] { 4, 1, 3 }, collection.Cast<int>());
        Assert.Equal(new int[] { 4, 1, 3 }, owner.Items.Cast<int>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Set last.
        collection[2] = 5;
        Assert.Equal(new int[] { 4, 1, 5 }, collection.Cast<int>());
        Assert.Equal(new int[] { 4, 1, 5 }, owner.Items.Cast<int>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Set same.
        collection[2] = 5;
        Assert.Equal(new int[] { 4, 1, 5 }, collection.Cast<int>());
        Assert.Equal(new int[] { 4, 1, 5 }, owner.Items.Cast<int>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

#if false
    [WinFormsFact]
    public unsafe void ListBoxObjectCollection_Item_SetNotSortedWithHandle_GetReturnsExpected()
    {
        using ListBox owner = new();
        var collection = new ListBox.ObjectCollection(owner);
        collection.AddRange(new object[] { 2, 1, 3 });
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;
        char *textBuffer = stackalloc char[256];

        // Set first.
        collection[0] = 4;
        Assert.Equal(new int[] { 4, 1, 3 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(3, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("4", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));

        // Set middle.
        collection[1] = 2;
        Assert.Equal(new int[] { 4, 2, 3 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(3, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("4", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));

        // Set last.
        collection[2] = 5;
        Assert.Equal(new int[] { 4, 2, 5 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(3, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("4", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("5", new string(textBuffer));

        // Set same.
        collection[2] = 5;
        Assert.Equal(new int[] { 4, 2, 5 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(3, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("4", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("5", new string(textBuffer));
    }

    [WinFormsFact]
    public unsafe void ListBoxObjectCollection_Item_SetSortedWithHandle_GetReturnsExpected()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        var collection = new ListBox.ObjectCollection(owner);
        collection.AddRange(new object[] { 2, 1, 3 });
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;
        char *textBuffer = stackalloc char[256];

        // Set first.
        collection[0] = 4;
        Assert.Equal(new int[] { 4, 2, 3 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(3, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("4", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));

        // Set middle.
        collection[1] = 1;
        Assert.Equal(new int[] { 4, 1, 3 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(3, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("4", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));

        // Set last.
        collection[2] = 5;
        Assert.Equal(new int[] { 4, 1, 5 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(3, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("4", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("5", new string(textBuffer));

        // Set same.
        collection[2] = 5;
        Assert.Equal(new int[] { 4, 1, 5 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(3, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("4", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("5", new string(textBuffer));
    }
#endif

    [WinFormsFact]
    public unsafe void ListBoxObjectCollection_Item_SetItemsWithHandle_GetReturnsExpected()
    {
        using ListBox owner = new();
        ListBox.ObjectCollection collection = owner.Items;
        collection.AddRange((object[])[2, 1, 3]);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;
        char* textBuffer = stackalloc char[256];

        // Set first.
        collection[0] = 4;
        Assert.Equal(new int[] { 4, 1, 3 }, collection.Cast<int>());
        Assert.Equal(new int[] { 4, 1, 3 }, owner.Items.Cast<int>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(3, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("4", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));

        // Set middle.
        collection[1] = 2;
        Assert.Equal(new int[] { 4, 2, 3 }, collection.Cast<int>());
        Assert.Equal(new int[] { 4, 2, 3 }, owner.Items.Cast<int>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(3, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("4", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));

        // Set last.
        collection[2] = 5;
        Assert.Equal(new int[] { 4, 2, 5 }, collection.Cast<int>());
        Assert.Equal(new int[] { 4, 2, 5 }, owner.Items.Cast<int>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(3, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("4", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("5", new string(textBuffer));

        // Set same.
        collection[2] = 5;
        Assert.Equal(new int[] { 4, 2, 5 }, collection.Cast<int>());
        Assert.Equal(new int[] { 4, 2, 5 }, owner.Items.Cast<int>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(3, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("4", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("5", new string(textBuffer));
    }

    [WinFormsFact]
    public unsafe void ListBoxObjectCollection_Item_SetItemsSortedWithHandle_GetReturnsExpected()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        ListBox.ObjectCollection collection = owner.Items;
        collection.AddRange((object[])[2, 1, 3]);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;
        char* textBuffer = stackalloc char[256];

        // Set first.
        collection[0] = 4;
        Assert.Equal(new int[] { 4, 2, 3 }, collection.Cast<int>());
        Assert.Equal(new int[] { 4, 2, 3 }, owner.Items.Cast<int>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(3, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("4", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));

        // Set middle.
        collection[1] = 1;
        Assert.Equal(new int[] { 4, 1, 3 }, collection.Cast<int>());
        Assert.Equal(new int[] { 4, 1, 3 }, owner.Items.Cast<int>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(3, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("4", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));

        // Set last.
        collection[2] = 5;
        Assert.Equal(new int[] { 4, 1, 5 }, collection.Cast<int>());
        Assert.Equal(new int[] { 4, 1, 5 }, owner.Items.Cast<int>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(3, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("4", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("5", new string(textBuffer));

        // Set same.
        collection[2] = 5;
        Assert.Equal(new int[] { 4, 1, 5 }, collection.Cast<int>());
        Assert.Equal(new int[] { 4, 1, 5 }, owner.Items.Cast<int>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(3, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("4", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("5", new string(textBuffer));
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_Item_SetWithOneSelectedItemNotSorted_Success()
    {
        using ListBox owner = new();
        ListBox.ObjectCollection collection = owner.Items;
        collection.Add(0);
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Set before.
        collection[0] = -1;
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Set after.
        collection[3] = 5;
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Set selected.
        collection[1] = 2;
        Assert.Equal(new object[] { 2 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Set same.
        collection[1] = 2;
        Assert.Equal(new object[] { 2 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_Item_SetWithOneSelectedItemSorted_Success()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        ListBox.ObjectCollection collection = owner.Items;
        collection.Add(0);
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Set before.
        collection[0] = -1;
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Set after.
        collection[3] = 5;
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Set selected.
        collection[1] = 2;
        Assert.Equal(new object[] { 2 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Set same.
        collection[1] = 2;
        Assert.Equal(new object[] { 2 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(SelectionMode.MultiExtended)]
    [InlineData(SelectionMode.MultiSimple)]
    public void ListBoxObjectCollection_Item_SetWithMultipleSelectedItemsNotSorted_Success(SelectionMode selectionMode)
    {
        using ListBox owner = new()
        {
            SelectionMode = selectionMode
        };
        ListBox.ObjectCollection collection = owner.Items;
        collection.Add(0);
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        collection.Add(5);
        owner.SelectedItems.Add(1);
        owner.SelectedItems.Add(3);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Set before.
        collection[0] = -1;
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 2 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Set after.
        collection[3] = 5;
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 2 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Set first selection.
        collection[1] = 2;
        Assert.Equal(new object[] { 2, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 2 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Set first selection same.
        collection[1] = 2;
        Assert.Equal(new object[] { 2, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 2 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Set last selection.
        collection[2] = 4;
        Assert.Equal(new object[] { 2, 4 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 2 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Set last selection same.
        collection[2] = 4;
        Assert.Equal(new object[] { 2, 4 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 2 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(SelectionMode.MultiExtended)]
    [InlineData(SelectionMode.MultiSimple)]
    public void ListBoxObjectCollection_Item_SetWithMultipleSelectedItemsSorted_Success(SelectionMode selectionMode)
    {
        using ListBox owner = new()
        {
            SelectionMode = selectionMode,
            Sorted = true
        };
        ListBox.ObjectCollection collection = owner.Items;
        collection.Add(0);
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        collection.Add(5);
        owner.SelectedItems.Add(1);
        owner.SelectedItems.Add(3);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Set before.
        collection[0] = -1;
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 2 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Set after.
        collection[3] = 5;
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 2 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Set first selection.
        collection[1] = 2;
        Assert.Equal(new object[] { 2, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 2 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Set first selection same.
        collection[1] = 2;
        Assert.Equal(new object[] { 2, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 2 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Set last selection.
        collection[2] = 4;
        Assert.Equal(new object[] { 2, 4 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 2 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Set last selection same.
        collection[2] = 4;
        Assert.Equal(new object[] { 2, 4 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 2 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_Item_SetWithOneSelectedItemNotSortedWithHandle_Success()
    {
        using ListBox owner = new();
        ListBox.ObjectCollection collection = owner.Items;
        collection.Add(0);
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) =>
        {
            Assert.Same(owner, sender);
            Assert.Same(EventArgs.Empty, e);
            selectedIndexChangedCallCount++;
        };

        // Set before.
        collection[0] = -1;
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set after.
        collection[3] = 5;
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set selected.
        collection[1] = 2;
        Assert.Equal(new object[] { 2 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(2, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set selected same.
        collection[1] = 2;
        Assert.Equal(new object[] { 2 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(3, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_Item_SetWithOneSelectedItemSortedWithHandle_Success()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        ListBox.ObjectCollection collection = owner.Items;
        collection.Add(0);
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) =>
        {
            Assert.Same(owner, sender);
            Assert.Same(EventArgs.Empty, e);
            selectedIndexChangedCallCount++;
        };

        // Set before.
        collection[0] = -1;
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set after.
        collection[3] = 5;
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set selected.
        collection[1] = 2;
        Assert.Equal(new object[] { 2 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(2, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        collection[1] = 2;
        Assert.Equal(new object[] { 2 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(3, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(SelectionMode.MultiExtended)]
    [InlineData(SelectionMode.MultiSimple)]
    public void ListBoxObjectCollection_Item_SetWithMultipleSelectedItemsNotSortedWithHandle_Success(SelectionMode selectionMode)
    {
        using ListBox owner = new()
        {
            SelectionMode = selectionMode
        };
        ListBox.ObjectCollection collection = owner.Items;
        collection.Add(0);
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        collection.Add(5);
        owner.SelectedItems.Add(1);
        owner.SelectedItems.Add(3);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) =>
        {
            Assert.Same(owner, sender);
            Assert.Same(EventArgs.Empty, e);
            selectedIndexChangedCallCount++;
        };

        // Set before.
        collection[0] = -1;
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 2 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set after.
        collection[3] = 5;
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 2 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set first selection.
        collection[1] = 2;
        Assert.Equal(new object[] { 2, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 2 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(2, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set first selection same.
        collection[1] = 2;
        Assert.Equal(new object[] { 2, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 2 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(3, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set last selection.
        collection[2] = 4;
        Assert.Equal(new object[] { 2 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(4, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set last selection same.
        collection[2] = 4;
        Assert.Equal(new object[] { 2 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(4, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(SelectionMode.MultiExtended)]
    [InlineData(SelectionMode.MultiSimple)]
    public void ListBoxObjectCollection_Item_SetWithMultipleSelectedItemsSortedWithHandle_Success(SelectionMode selectionMode)
    {
        using ListBox owner = new()
        {
            SelectionMode = selectionMode,
            Sorted = true
        };
        ListBox.ObjectCollection collection = owner.Items;
        collection.Add(0);
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        collection.Add(5);
        owner.SelectedItems.Add(1);
        owner.SelectedItems.Add(3);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) =>
        {
            Assert.Same(owner, sender);
            Assert.Same(EventArgs.Empty, e);
            selectedIndexChangedCallCount++;
        };

        // Set before.
        collection[0] = -1;
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 2 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set after.
        collection[3] = 5;
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 2 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set first selection.
        collection[1] = 2;
        Assert.Equal(new object[] { 2, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 2 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(2, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set first selection same.
        collection[1] = 2;
        Assert.Equal(new object[] { 2, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 2 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(3, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set last selection.
        collection[2] = 4;
        Assert.Equal(new object[] { 2 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(4, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set last selection same.
        collection[2] = 4;
        Assert.Equal(new object[] { 2 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(4, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void ListBoxObjectCollection_Item_SetNullValueEmpty_ThrowsArgumentNullException(int index)
    {
        using ListBox owner = new();
        var collection = new ListBox.ObjectCollection(owner);
        Assert.Throws<ArgumentNullException>("value", () => collection[index] = null);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void ListBoxObjectCollection_Item_SetNullValueNotEmpty_ThrowsArgumentNullException(int index)
    {
        using ListBox owner = new();
        var collection = new ListBox.ObjectCollection(owner)
        {
            1
        };
        Assert.Throws<ArgumentNullException>("value", () => collection[index] = null);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void ListBoxObjectCollection_Item_SetInvalidIndexEmpty_ThrowsArgumentOutOfRangeException(int index)
    {
        using ListBox owner = new();
        var collection = new ListBox.ObjectCollection(owner);
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index] = 2);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(1)]
    [InlineData(2)]
    public void ListBoxObjectCollection_Item_SetInvalidIndexNotEmpty_ThrowsArgumentOutOfRangeException(int index)
    {
        using ListBox owner = new();
        var collection = new ListBox.ObjectCollection(owner)
        {
            1
        };
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index] = 2);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_Add_NotSorted_Success()
    {
        using ListBox owner = new();
        var collection = new ListBox.ObjectCollection(owner);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Add one.
        Assert.Equal(0, collection.Add(2));
        Assert.Single(collection);
        Assert.Equal(new object[] { 2 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Add another.
        Assert.Equal(1, collection.Add(1));
        Assert.Equal(2, collection.Count);
        Assert.Equal(new object[] { 2, 1 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Add same.
        Assert.Equal(2, collection.Add(1));
        Assert.Equal(3, collection.Count);
        Assert.Equal(new object[] { 2, 1, 1 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Add another.
        Assert.Equal(3, collection.Add(3));
        Assert.Equal(4, collection.Count);
        Assert.Equal(new object[] { 2, 1, 1, 3 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_Add_ItemsNotSorted_Success()
    {
        using ListBox owner = new();
        ListBox.ObjectCollection collection = owner.Items;
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Add one.
        Assert.Equal(0, collection.Add(2));
        Assert.Single(collection);
        Assert.Equal(new object[] { 2 }, collection.Cast<object>());
        Assert.Equal(new object[] { 2 }, owner.Items.Cast<object>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Add another.
        Assert.Equal(1, collection.Add(1));
        Assert.Equal(2, collection.Count);
        Assert.Equal(new object[] { 2, 1 }, collection.Cast<object>());
        Assert.Equal(new object[] { 2, 1 }, owner.Items.Cast<object>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Add same.
        Assert.Equal(2, collection.Add(1));
        Assert.Equal(3, collection.Count);
        Assert.Equal(new object[] { 2, 1, 1 }, collection.Cast<object>());
        Assert.Equal(new object[] { 2, 1, 1 }, owner.Items.Cast<object>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Add another.
        Assert.Equal(3, collection.Add(3));
        Assert.Equal(4, collection.Count);
        Assert.Equal(new object[] { 2, 1, 1, 3 }, collection.Cast<object>());
        Assert.Equal(new object[] { 2, 1, 1, 3 }, owner.Items.Cast<object>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_Add_Sorted_Success()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        var collection = new ListBox.ObjectCollection(owner);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Add one.
        Assert.Equal(0, collection.Add(2));
        Assert.Single(collection);
        Assert.Equal(new object[] { 2 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Add another.
        Assert.Equal(0, collection.Add(1));
        Assert.Equal(2, collection.Count);
        Assert.Equal(new object[] { 1, 2 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Add same.
        Assert.Equal(0, collection.Add(1));
        Assert.Equal(3, collection.Count);
        Assert.Equal(new object[] { 1, 1, 2 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Add another.
        Assert.Equal(3, collection.Add(3));
        Assert.Equal(4, collection.Count);
        Assert.Equal(new object[] { 1, 1, 2, 3 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_Add_ItemsSorted_Success()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        ListBox.ObjectCollection collection = owner.Items;
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Add one.
        Assert.Equal(0, collection.Add(2));
        Assert.Single(collection);
        Assert.Equal(new object[] { 2 }, collection.Cast<object>());
        Assert.Equal(new object[] { 2 }, owner.Items.Cast<object>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Add another.
        Assert.Equal(0, collection.Add(1));
        Assert.Equal(2, collection.Count);
        Assert.Equal(new object[] { 1, 2 }, collection.Cast<object>());
        Assert.Equal(new object[] { 1, 2 }, owner.Items.Cast<object>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Add same.
        Assert.Equal(0, collection.Add(1));
        Assert.Equal(3, collection.Count);
        Assert.Equal(new object[] { 1, 1, 2 }, collection.Cast<object>());
        Assert.Equal(new object[] { 1, 1, 2 }, owner.Items.Cast<object>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Add another.
        Assert.Equal(3, collection.Add(3));
        Assert.Equal(4, collection.Count);
        Assert.Equal(new object[] { 1, 1, 2, 3 }, collection.Cast<object>());
        Assert.Equal(new object[] { 1, 1, 2, 3 }, owner.Items.Cast<object>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public unsafe void ListBoxObjectCollection_Add_NotSortedWithHandle_Success()
    {
        using ListBox owner = new();
        var collection = new ListBox.ObjectCollection(owner);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        char* textBuffer = stackalloc char[256];

        // Add one.
        Assert.Equal(0, collection.Add(2));
        Assert.Single(collection);
        Assert.Equal(new object[] { 2 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(1, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));

        // Add another.
        Assert.Equal(1, collection.Add(1));
        Assert.Equal(2, collection.Count);
        Assert.Equal(new object[] { 2, 1 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(2, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));

        // Add same.
        Assert.Equal(2, collection.Add(1));
        Assert.Equal(3, collection.Count);
        Assert.Equal(new object[] { 2, 1, 1 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(3, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));

        // Add another.
        Assert.Equal(3, collection.Add(3));
        Assert.Equal(4, collection.Count);
        Assert.Equal(new object[] { 2, 1, 1, 3 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(4, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 3, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));
    }

    [WinFormsFact]
    public unsafe void ListBoxObjectCollection_Add_ItemsNotSortedWithHandle_Success()
    {
        using ListBox owner = new();
        ListBox.ObjectCollection collection = owner.Items;
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        char* textBuffer = stackalloc char[256];

        // Add one.
        Assert.Equal(0, collection.Add(2));
        Assert.Single(collection);
        Assert.Equal(new object[] { 2 }, collection.Cast<object>());
        Assert.Equal(new object[] { 2 }, owner.Items.Cast<object>());
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(1, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));

        // Add another.
        Assert.Equal(1, collection.Add(1));
        Assert.Equal(2, collection.Count);
        Assert.Equal(new object[] { 2, 1 }, collection.Cast<object>());
        Assert.Equal(new object[] { 2, 1 }, owner.Items.Cast<object>());
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(2, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));

        // Add same.
        Assert.Equal(2, collection.Add(1));
        Assert.Equal(3, collection.Count);
        Assert.Equal(new object[] { 2, 1, 1 }, collection.Cast<object>());
        Assert.Equal(new object[] { 2, 1, 1 }, owner.Items.Cast<object>());
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(3, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));

        // Add another.
        Assert.Equal(3, collection.Add(3));
        Assert.Equal(4, collection.Count);
        Assert.Equal(new object[] { 2, 1, 1, 3 }, collection.Cast<object>());
        Assert.Equal(new object[] { 2, 1, 1, 3 }, owner.Items.Cast<object>());
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(4, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 3, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));
    }

    [WinFormsFact]
    public unsafe void ListBoxObjectCollection_Add_SortedWithHandle_Success()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        var collection = new ListBox.ObjectCollection(owner);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        char* textBuffer = stackalloc char[256];

        // Add one.
        Assert.Equal(0, collection.Add(2));
        Assert.Single(collection);
        Assert.Equal(new object[] { 2 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(1, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));

        // Add another.
        Assert.Equal(0, collection.Add(1));
        Assert.Equal(2, collection.Count);
        Assert.Equal(new object[] { 1, 2 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(2, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));

        // Add same.
        Assert.Equal(0, collection.Add(1));
        Assert.Equal(3, collection.Count);
        Assert.Equal(new object[] { 1, 1, 2 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(3, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));

        // Add another.
        Assert.Equal(3, collection.Add(3));
        Assert.Equal(4, collection.Count);
        Assert.Equal(new object[] { 1, 1, 2, 3 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(4, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 3, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));
    }

    [WinFormsFact]
    public unsafe void ListBoxObjectCollection_Add_ItemsSortedWithHandle_Success()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        ListBox.ObjectCollection collection = owner.Items;
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        char* textBuffer = stackalloc char[256];

        // Add one.
        Assert.Equal(0, collection.Add(2));
        Assert.Single(collection);
        Assert.Equal(new object[] { 2 }, collection.Cast<object>());
        Assert.Equal(new object[] { 2 }, owner.Items.Cast<object>());
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(1, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));

        // Add another.
        Assert.Equal(0, collection.Add(1));
        Assert.Equal(2, collection.Count);
        Assert.Equal(new object[] { 1, 2 }, collection.Cast<object>());
        Assert.Equal(new object[] { 1, 2 }, owner.Items.Cast<object>());
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(2, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));

        // Add same.
        Assert.Equal(0, collection.Add(1));
        Assert.Equal(3, collection.Count);
        Assert.Equal(new object[] { 1, 1, 2 }, collection.Cast<object>());
        Assert.Equal(new object[] { 1, 1, 2 }, owner.Items.Cast<object>());
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(3, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));

        // Add another.
        Assert.Equal(3, collection.Add(3));
        Assert.Equal(4, collection.Count);
        Assert.Equal(new object[] { 1, 1, 2, 3 }, collection.Cast<object>());
        Assert.Equal(new object[] { 1, 1, 2, 3 }, owner.Items.Cast<object>());
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(4, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 3, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_Add_WithOneSelectedItemNotSorted_Success()
    {
        using ListBox owner = new();
        ListBox.ObjectCollection collection = owner.Items;
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Add before.
        collection.Add(0);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Add after.
        collection.Add(5);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_Add_WithOneSelectedItemSorted_Success()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        ListBox.ObjectCollection collection = owner.Items;
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Add before.
        collection.Add(0);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Add after.
        collection.Add(5);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(SelectionMode.MultiExtended)]
    [InlineData(SelectionMode.MultiSimple)]
    public void ListBoxObjectCollection_Add_WithMultipleSelectedItemsNotSorted_Success(SelectionMode selectionMode)
    {
        using ListBox owner = new()
        {
            SelectionMode = selectionMode
        };
        ListBox.ObjectCollection collection = owner.Items;
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        collection.Add(5);
        owner.SelectedItems.Add(1);
        owner.SelectedItems.Add(3);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Add before.
        collection.Add(0);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0, 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Add middle.
        collection.Add(2);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0, 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Add after.
        collection.Add(4);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0, 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(SelectionMode.MultiExtended)]
    [InlineData(SelectionMode.MultiSimple)]
    public void ListBoxObjectCollection_Add_WithMultipleSelectedItemsSorted_Success(SelectionMode selectionMode)
    {
        using ListBox owner = new()
        {
            SelectionMode = selectionMode,
            Sorted = true
        };
        ListBox.ObjectCollection collection = owner.Items;
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);
        owner.SelectedItems.Add(3);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Add before.
        collection.Add(0);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 2 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.False(owner.IsHandleCreated);

        // Add middle.
        collection.Add(2);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 3 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.False(owner.IsHandleCreated);

        // Add after.
        collection.Add(5);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 3 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_Add_WithOneSelectedItemNotSortedWithHandle_Succes()
    {
        using ListBox owner = new();
        ListBox.ObjectCollection collection = owner.Items;
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;

        // Add before.
        collection.Add(0);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Add after.
        collection.Add(5);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_Add_WithOneSelectedItemSortedWithHandle_Succes()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        ListBox.ObjectCollection collection = owner.Items;
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;

        // Add before.
        collection.Add(0);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Add after.
        collection.Add(5);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(SelectionMode.MultiExtended)]
    [InlineData(SelectionMode.MultiSimple)]
    public void ListBoxObjectCollection_Add_WithMultipleSelectedItemsNotSortedWithHandle_Succes(SelectionMode selectionMode)
    {
        using ListBox owner = new()
        {
            SelectionMode = selectionMode
        };
        ListBox.ObjectCollection collection = owner.Items;
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        collection.Add(5);
        owner.SelectedItems.Add(1);
        owner.SelectedItems.Add(3);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;

        // Add before.
        collection.Add(0);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0, 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Add middle.
        collection.Add(2);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0, 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Add after.
        collection.Add(4);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0, 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(SelectionMode.MultiExtended)]
    [InlineData(SelectionMode.MultiSimple)]
    public void ListBoxObjectCollection_Add_WithMultipleSelectedItemsSortedWithHandle_Succes(SelectionMode selectionMode)
    {
        using ListBox owner = new()
        {
            SelectionMode = selectionMode,
            Sorted = true
        };
        ListBox.ObjectCollection collection = owner.Items;
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);
        owner.SelectedItems.Add(3);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;

        // Add before.
        collection.Add(0);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 2 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Add middle.
        collection.Add(2);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 3 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Add after.
        collection.Add(5);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 3 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public unsafe void ListBoxObjectCollection_Add_CreateHandle_Success()
    {
        using ListBox owner = new();
        var collection = new ListBox.ObjectCollection(owner)
        {
            2,
            1,
            1,
            3
        };
        char* textBuffer = stackalloc char[256];

        Assert.Equal(0, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
    }

    [WinFormsFact]
    public unsafe void ListBoxObjectCollection_Add_ItemsCreateHandle_Success()
    {
        using ListBox owner = new();
        ListBox.ObjectCollection collection = owner.Items;
        collection.Add(2);
        collection.Add(1);
        collection.Add(1);
        collection.Add(3);
        char* textBuffer = stackalloc char[256];

        Assert.Equal(4, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 3, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_Add_NullItem_ThrowsArgumentNullException()
    {
        using ListBox owner = new();
        var collection = new ListBox.ObjectCollection(owner);
        Assert.Throws<ArgumentNullException>("item", () => collection.Add(null));
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_Add_OwnerHasDataSource_ThrowsArgumentException()
    {
        using ListBox owner = new()
        {
            DataSource = Array.Empty<object>()
        };
        var collection = new ListBox.ObjectCollection(owner);
        Assert.Throws<ArgumentException>(() => collection.Add(1));
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(-2)]
    public void ListBoxObjectCollection_Add_ErrorAdding_ThrowsOutOfMemoryException(int result)
    {
        // Note that this is not an actual out of memory, we're artificially setting up an error case
        // that we surface as `OutOfMemoryException` (see ListBox.NativeAdd(Object item))

        using CustomAddStringListBox owner = new()
        {
            AddStringResult = result
        };
        var collection = new ListBox.ObjectCollection(owner);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        Assert.Throws<OutOfMemoryException>(() => collection.Add(1));
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_AddRange_ObjectArrayNotSorted_Success()
    {
        using ListBox owner = new();
        var collection = new ListBox.ObjectCollection(owner);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        collection.AddRange((object[])[2, 1, 1, 3]);
        Assert.Equal(4, collection.Count);
        Assert.Equal(new object[] { 2, 1, 1, 3 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Add empty.
        collection.AddRange(Array.Empty<object>());
        Assert.Equal(4, collection.Count);
        Assert.Equal(new object[] { 2, 1, 1, 3 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_AddRange_ObjectArrayItemsNotSorted_Success()
    {
        using ListBox owner = new();
        ListBox.ObjectCollection collection = owner.Items;
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        collection.AddRange((object[])[2, 1, 1, 3]);
        Assert.Equal(4, collection.Count);
        Assert.Equal(new object[] { 2, 1, 1, 3 }, collection.Cast<object>());
        Assert.Equal(new object[] { 2, 1, 1, 3 }, owner.Items.Cast<object>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Add empty.
        collection.AddRange(Array.Empty<object>());
        Assert.Equal(4, collection.Count);
        Assert.Equal(new object[] { 2, 1, 1, 3 }, collection.Cast<object>());
        Assert.Equal(new object[] { 2, 1, 1, 3 }, owner.Items.Cast<object>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_AddRange_ObjectArraySorted_Success()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        var collection = new ListBox.ObjectCollection(owner);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        collection.AddRange((object[])[2, 1, 1, 3]);
        Assert.Equal(4, collection.Count);
        Assert.Equal(new object[] { 1, 1, 2, 3 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Add empty.
        collection.AddRange(Array.Empty<object>());
        Assert.Equal(4, collection.Count);
        Assert.Equal(new object[] { 1, 1, 2, 3 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_AddRange_ObjectArrayItemsSorted_Success()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        ListBox.ObjectCollection collection = owner.Items;
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        collection.AddRange((object[])[2, 1, 1, 3]);
        Assert.Equal(4, collection.Count);
        Assert.Equal(new object[] { 1, 1, 2, 3 }, collection.Cast<object>());
        Assert.Equal(new object[] { 1, 1, 2, 3 }, owner.Items.Cast<object>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Add empty.
        collection.AddRange(Array.Empty<object>());
        Assert.Equal(4, collection.Count);
        Assert.Equal(new object[] { 1, 1, 2, 3 }, collection.Cast<object>());
        Assert.Equal(new object[] { 1, 1, 2, 3 }, owner.Items.Cast<object>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public unsafe void ListBoxObjectCollection_AddRange_ObjectArrayNotSortedWithHandle_Success()
    {
        using ListBox owner = new();
        var collection = new ListBox.ObjectCollection(owner);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        char* textBuffer = stackalloc char[256];

        collection.AddRange((object[])[2, 1, 1, 3]);
        Assert.Equal(4, collection.Count);
        Assert.Equal(new object[] { 2, 1, 1, 3 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(4, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 3, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));

        // Add empty.
        collection.AddRange(Array.Empty<object>());
        Assert.Equal(4, collection.Count);
        Assert.Equal(new object[] { 2, 1, 1, 3 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(4, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 3, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));
    }

    [WinFormsFact]
    public unsafe void ListBoxObjectCollection_AddRange_ObjectArrayItemsNotSortedWithHandle_Success()
    {
        using ListBox owner = new();
        ListBox.ObjectCollection collection = owner.Items;
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        char* textBuffer = stackalloc char[256];

        collection.AddRange((object[])[2, 1, 1, 3]);
        Assert.Equal(4, collection.Count);
        Assert.Equal(new object[] { 2, 1, 1, 3 }, collection.Cast<object>());
        Assert.Equal(new object[] { 2, 1, 1, 3 }, owner.Items.Cast<object>());
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(4, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 3, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));

        // Add empty.
        collection.AddRange(Array.Empty<object>());
        Assert.Equal(4, collection.Count);
        Assert.Equal(new object[] { 2, 1, 1, 3 }, collection.Cast<object>());
        Assert.Equal(new object[] { 2, 1, 1, 3 }, owner.Items.Cast<object>());
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(4, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 3, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));
    }

    [WinFormsFact]
    public unsafe void ListBoxObjectCollection_AddRange_ObjectArraySortedWithHandle_Success()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        var collection = new ListBox.ObjectCollection(owner);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        char* textBuffer = stackalloc char[256];

        collection.AddRange((object[])[2, 1, 1, 3]);
        Assert.Equal(4, collection.Count);
        Assert.Equal(new object[] { 1, 1, 2, 3 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(4, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 3, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));

        // Add empty.
        collection.AddRange(Array.Empty<object>());
        Assert.Equal(4, collection.Count);
        Assert.Equal(new object[] { 1, 1, 2, 3 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(4, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 3, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));
    }

    [WinFormsFact]
    public unsafe void ListBoxObjectCollection_AddRange_ObjectArrayItemsSortedWithHandle_Success()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        ListBox.ObjectCollection collection = owner.Items;
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        char* textBuffer = stackalloc char[256];

        collection.AddRange((object[])[2, 1, 1, 3]);
        Assert.Equal(4, collection.Count);
        Assert.Equal(new object[] { 1, 1, 2, 3 }, collection.Cast<object>());
        Assert.Equal(new object[] { 1, 1, 2, 3 }, owner.Items.Cast<object>());
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(4, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 3, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));

        // Add empty.
        collection.AddRange(Array.Empty<object>());
        Assert.Equal(4, collection.Count);
        Assert.Equal(new object[] { 1, 1, 2, 3 }, collection.Cast<object>());
        Assert.Equal(new object[] { 1, 1, 2, 3 }, owner.Items.Cast<object>());
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(4, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 3, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_AddRange_ObjectArrayWithOneSelectedItemNotSorted_Success()
    {
        using ListBox owner = new();
        ListBox.ObjectCollection collection = owner.Items;
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        collection.AddRange((object[])[0, 5]);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Add empty.
        collection.AddRange(Array.Empty<object>());
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_AddRange_ObjectArrayWithOneSelectedItemSorted_Success()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        ListBox.ObjectCollection collection = owner.Items;
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        collection.AddRange((object[])[0, 5]);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Add empty.
        collection.AddRange(Array.Empty<object>());
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(SelectionMode.MultiExtended)]
    [InlineData(SelectionMode.MultiSimple)]
    public void ListBoxObjectCollection_AddRange_ObjectArrayWithMultipleSelectedItemsNotSorted_Success(SelectionMode selectionMode)
    {
        using ListBox owner = new()
        {
            SelectionMode = selectionMode
        };
        ListBox.ObjectCollection collection = owner.Items;
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        collection.Add(5);
        owner.SelectedItems.Add(1);
        owner.SelectedItems.Add(3);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        collection.AddRange((object[])[0, 2, 4]);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0, 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Add empty.
        collection.AddRange(Array.Empty<object>());
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0, 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(SelectionMode.MultiExtended)]
    [InlineData(SelectionMode.MultiSimple)]
    public void ListBoxObjectCollection_AddRange_ObjectArrayWithMultipleSelectedItemsSorted_Success(SelectionMode selectionMode)
    {
        using ListBox owner = new()
        {
            SelectionMode = selectionMode,
            Sorted = true
        };
        ListBox.ObjectCollection collection = owner.Items;
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);
        owner.SelectedItems.Add(3);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        collection.AddRange((object[])[0, 2, 4]);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 3 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.False(owner.IsHandleCreated);

        // Add empty.
        collection.AddRange(Array.Empty<object>());
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 3 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_AddRange_ObjectArrayWithOneSelectedItemNotSortedWithHandle_Succes()
    {
        using ListBox owner = new();
        ListBox.ObjectCollection collection = owner.Items;
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;

        collection.AddRange((object[])[0, 5]);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Add empty.
        collection.AddRange(Array.Empty<object>());
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_AddRange_ObjectArrayWithOneSelectedItemSortedWithHandle_Succes()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        ListBox.ObjectCollection collection = owner.Items;
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;

        collection.AddRange((object[])[0, 5]);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Add empty.
        collection.AddRange(Array.Empty<object>());
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(SelectionMode.MultiExtended)]
    [InlineData(SelectionMode.MultiSimple)]
    public void ListBoxObjectCollection_AddRange_ObjectArrayWithMultipleSelectedItemsNotSortedWithHandle_Succes(SelectionMode selectionMode)
    {
        using ListBox owner = new()
        {
            SelectionMode = selectionMode
        };
        ListBox.ObjectCollection collection = owner.Items;
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        collection.Add(5);
        owner.SelectedItems.Add(1);
        owner.SelectedItems.Add(3);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;

        collection.AddRange((object[])[0, 2, 4]);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0, 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Add empty.
        collection.AddRange(Array.Empty<object>());
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0, 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(SelectionMode.MultiExtended)]
    [InlineData(SelectionMode.MultiSimple)]
    public void ListBoxObjectCollection_AddRange_ObjectArrayWithMultipleSelectedItemsSortedWithHandle_Succes(SelectionMode selectionMode)
    {
        using ListBox owner = new()
        {
            SelectionMode = selectionMode,
            Sorted = true
        };
        ListBox.ObjectCollection collection = owner.Items;
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);
        owner.SelectedItems.Add(3);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;

        collection.AddRange((object[])[0, 2, 5]);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 3 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Add empty.
        collection.AddRange(Array.Empty<object>());
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 3 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public unsafe void ListBoxObjectCollection_AddRange_ObjectArrayCreateHandle_Success()
    {
        using ListBox owner = new();
        var collection = new ListBox.ObjectCollection(owner);
        collection.AddRange((object[])[2, 1, 1, 3]);
        char* textBuffer = stackalloc char[256];

        Assert.Equal(0, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
    }

    [WinFormsFact]
    public unsafe void ListBoxObjectCollection_AddRange_ObjectArrayItemsCreateHandle_Success()
    {
        using ListBox owner = new();
        ListBox.ObjectCollection collection = owner.Items;
        collection.AddRange((object[])[2, 1, 1, 3]);
        char* textBuffer = stackalloc char[256];

        Assert.Equal(4, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 3, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_AddRange_ObjectCollectionNotSorted_Success()
    {
        using ListBox owner = new();
        var collection = new ListBox.ObjectCollection(owner);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        using ListBox otherOwner = new();
        var otherCollection = new ListBox.ObjectCollection(otherOwner);
        otherCollection.AddRange((object[])[2, 1, 1, 3]);
        collection.AddRange(otherCollection);
        Assert.Equal(4, collection.Count);
        Assert.Equal(new object[] { 2, 1, 1, 3 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Add empty.
        using ListBox emptyOwner = new();
        var emptyCollection = new ListBox.ObjectCollection(emptyOwner);
        collection.AddRange(emptyCollection);
        Assert.Equal(4, collection.Count);
        Assert.Equal(new object[] { 2, 1, 1, 3 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_AddRange_ObjectCollectionItemsNotSorted_Success()
    {
        using ListBox owner = new();
        ListBox.ObjectCollection collection = owner.Items;
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        using ListBox otherOwner = new();
        var otherCollection = new ListBox.ObjectCollection(otherOwner);
        otherCollection.AddRange((object[])[2, 1, 1, 3]);
        collection.AddRange(otherCollection);
        Assert.Equal(4, collection.Count);
        Assert.Equal(new object[] { 2, 1, 1, 3 }, collection.Cast<object>());
        Assert.Equal(new object[] { 2, 1, 1, 3 }, owner.Items.Cast<object>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Add empty.
        using ListBox emptyOwner = new();
        var emptyCollection = new ListBox.ObjectCollection(emptyOwner);
        collection.AddRange(emptyCollection);
        Assert.Equal(4, collection.Count);
        Assert.Equal(new object[] { 2, 1, 1, 3 }, collection.Cast<object>());
        Assert.Equal(new object[] { 2, 1, 1, 3 }, owner.Items.Cast<object>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_AddRange_ObjectCollectionSorted_Success()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        var collection = new ListBox.ObjectCollection(owner);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        using ListBox otherOwner = new();
        var otherCollection = new ListBox.ObjectCollection(otherOwner);
        otherCollection.AddRange((object[])[2, 1, 1, 3]);
        collection.AddRange(otherCollection);
        Assert.Equal(4, collection.Count);
        Assert.Equal(new object[] { 1, 1, 2, 3 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Add empty.
        using ListBox emptyOwner = new();
        var emptyCollection = new ListBox.ObjectCollection(emptyOwner);
        collection.AddRange(emptyCollection);
        Assert.Equal(4, collection.Count);
        Assert.Equal(new object[] { 1, 1, 2, 3 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_AddRange_ObjectCollectionItemsSorted_Success()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        ListBox.ObjectCollection collection = owner.Items;
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        using ListBox otherOwner = new();
        var otherCollection = new ListBox.ObjectCollection(otherOwner);
        otherCollection.AddRange((object[])[2, 1, 1, 3]);
        collection.AddRange(otherCollection);
        Assert.Equal(4, collection.Count);
        Assert.Equal(new object[] { 1, 1, 2, 3 }, collection.Cast<object>());
        Assert.Equal(new object[] { 1, 1, 2, 3 }, owner.Items.Cast<object>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Add empty.
        using ListBox emptyOwner = new();
        var emptyCollection = new ListBox.ObjectCollection(emptyOwner);
        collection.AddRange(emptyCollection);
        Assert.Equal(4, collection.Count);
        Assert.Equal(new object[] { 1, 1, 2, 3 }, collection.Cast<object>());
        Assert.Equal(new object[] { 1, 1, 2, 3 }, owner.Items.Cast<object>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public unsafe void ListBoxObjectCollection_AddRange_ObjectCollectionNotSortedWithHandle_Success()
    {
        using ListBox owner = new();
        var collection = new ListBox.ObjectCollection(owner);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        char* textBuffer = stackalloc char[256];

        using ListBox otherOwner = new();
        var otherCollection = new ListBox.ObjectCollection(otherOwner);
        otherCollection.AddRange((object[])[2, 1, 1, 3]);
        collection.AddRange(otherCollection);
        Assert.Equal(4, collection.Count);
        Assert.Equal(new object[] { 2, 1, 1, 3 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(4, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 3, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));

        // Add empty.
        using ListBox emptyOwner = new();
        var emptyCollection = new ListBox.ObjectCollection(emptyOwner);
        collection.AddRange(emptyCollection);
        Assert.Equal(4, collection.Count);
        Assert.Equal(new object[] { 2, 1, 1, 3 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(4, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 3, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));
    }

    [WinFormsFact]
    public unsafe void ListBoxObjectCollection_AddRange_ObjectCollectionItemsNotSortedWithHandle_Success()
    {
        using ListBox owner = new();
        ListBox.ObjectCollection collection = owner.Items;
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        char* textBuffer = stackalloc char[256];

        using ListBox otherOwner = new();
        var otherCollection = new ListBox.ObjectCollection(otherOwner);
        otherCollection.AddRange((object[])[2, 1, 1, 3]);
        collection.AddRange(otherCollection);
        Assert.Equal(4, collection.Count);
        Assert.Equal(new object[] { 2, 1, 1, 3 }, collection.Cast<object>());
        Assert.Equal(new object[] { 2, 1, 1, 3 }, owner.Items.Cast<object>());
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(4, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 3, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));

        // Add empty.
        using ListBox emptyOwner = new();
        var emptyCollection = new ListBox.ObjectCollection(emptyOwner);
        collection.AddRange(emptyCollection);
        Assert.Equal(4, collection.Count);
        Assert.Equal(new object[] { 2, 1, 1, 3 }, collection.Cast<object>());
        Assert.Equal(new object[] { 2, 1, 1, 3 }, owner.Items.Cast<object>());
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(4, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 3, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));
    }

    [WinFormsFact]
    public unsafe void ListBoxObjectCollection_AddRange_ObjectCollectionSortedWithHandle_Success()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        var collection = new ListBox.ObjectCollection(owner);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        char* textBuffer = stackalloc char[256];

        using ListBox otherOwner = new();
        var otherCollection = new ListBox.ObjectCollection(otherOwner);
        otherCollection.AddRange((object[])[2, 1, 1, 3]);
        collection.AddRange(otherCollection);
        Assert.Equal(4, collection.Count);
        Assert.Equal(new object[] { 1, 1, 2, 3 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(4, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 3, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));

        // Add empty.
        using ListBox emptyOwner = new();
        var emptyCollection = new ListBox.ObjectCollection(emptyOwner);
        collection.AddRange(emptyCollection);
        Assert.Equal(4, collection.Count);
        Assert.Equal(new object[] { 1, 1, 2, 3 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(4, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 3, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));
    }

    [WinFormsFact]
    public unsafe void ListBoxObjectCollection_AddRange_ObjectCollectionItemsSortedWithHandle_Success()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        ListBox.ObjectCollection collection = owner.Items;
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        char* textBuffer = stackalloc char[256];

        using ListBox otherOwner = new();
        var otherCollection = new ListBox.ObjectCollection(otherOwner);
        otherCollection.AddRange((object[])[2, 1, 1, 3]);
        collection.AddRange(otherCollection);
        Assert.Equal(4, collection.Count);
        Assert.Equal(new object[] { 1, 1, 2, 3 }, collection.Cast<object>());
        Assert.Equal(new object[] { 1, 1, 2, 3 }, owner.Items.Cast<object>());
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(4, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 3, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));

        // Add empty.
        using ListBox emptyOwner = new();
        var emptyCollection = new ListBox.ObjectCollection(emptyOwner);
        collection.AddRange(emptyCollection);
        Assert.Equal(4, collection.Count);
        Assert.Equal(new object[] { 1, 1, 2, 3 }, collection.Cast<object>());
        Assert.Equal(new object[] { 1, 1, 2, 3 }, owner.Items.Cast<object>());
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(4, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 3, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_AddRange_ObjectCollectionWithOneSelectedItemNotSorted_Success()
    {
        using ListBox owner = new();
        ListBox.ObjectCollection collection = owner.Items;
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        using ListBox otherOwner = new();
        var otherCollection = new ListBox.ObjectCollection(otherOwner);
        otherCollection.AddRange((object[])[0, 5]);
        collection.AddRange(otherCollection);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Add empty.
        using ListBox emptyOwner = new();
        var emptyCollection = new ListBox.ObjectCollection(emptyOwner);
        collection.AddRange(emptyCollection);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_AddRange_ObjectCollectionWithOneSelectedItemSorted_Success()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        ListBox.ObjectCollection collection = owner.Items;
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        using ListBox otherOwner = new();
        var otherCollection = new ListBox.ObjectCollection(otherOwner);
        otherCollection.AddRange((object[])[0, 5]);
        collection.AddRange(otherCollection);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Add empty.
        using ListBox emptyOwner = new();
        var emptyCollection = new ListBox.ObjectCollection(emptyOwner);
        collection.AddRange(emptyCollection);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(SelectionMode.MultiExtended)]
    [InlineData(SelectionMode.MultiSimple)]
    public void ListBoxObjectCollection_AddRange_ObjectCollectionWithMultipleSelectedItemsNotSorted_Success(SelectionMode selectionMode)
    {
        using ListBox owner = new()
        {
            SelectionMode = selectionMode
        };
        ListBox.ObjectCollection collection = owner.Items;
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        collection.Add(5);
        owner.SelectedItems.Add(1);
        owner.SelectedItems.Add(3);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        using ListBox otherOwner = new();
        var otherCollection = new ListBox.ObjectCollection(otherOwner);
        otherCollection.AddRange((object[])[0, 2, 4]);
        collection.AddRange(otherCollection);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0, 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Add empty.
        using ListBox emptyOwner = new();
        var emptyCollection = new ListBox.ObjectCollection(emptyOwner);
        collection.AddRange(emptyCollection);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0, 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(SelectionMode.MultiExtended)]
    [InlineData(SelectionMode.MultiSimple)]
    public void ListBoxObjectCollection_AddRange_ObjectCollectionWithMultipleSelectedItemsSorted_Success(SelectionMode selectionMode)
    {
        using ListBox owner = new()
        {
            SelectionMode = selectionMode,
            Sorted = true
        };
        ListBox.ObjectCollection collection = owner.Items;
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);
        owner.SelectedItems.Add(3);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        using ListBox otherOwner = new();
        var otherCollection = new ListBox.ObjectCollection(otherOwner);
        otherCollection.AddRange((object[])[0, 2, 4]);
        collection.AddRange(otherCollection);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 3 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.False(owner.IsHandleCreated);

        // Add empty.
        using ListBox emptyOwner = new();
        var emptyCollection = new ListBox.ObjectCollection(emptyOwner);
        collection.AddRange(emptyCollection);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 3 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_AddRange_ObjectCollectionWithOneSelectedItemNotSortedWithHandle_Succes()
    {
        using ListBox owner = new();
        ListBox.ObjectCollection collection = owner.Items;
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;

        using ListBox otherOwner = new();
        var otherCollection = new ListBox.ObjectCollection(otherOwner);
        otherCollection.AddRange((object[])[0, 5]);
        collection.AddRange(otherCollection);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Add empty.
        using ListBox emptyOwner = new();
        var emptyCollection = new ListBox.ObjectCollection(emptyOwner);
        collection.AddRange(emptyCollection);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_AddRange_ObjectCollectionWithOneSelectedItemSortedWithHandle_Succes()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        ListBox.ObjectCollection collection = owner.Items;
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;

        using ListBox otherOwner = new();
        var otherCollection = new ListBox.ObjectCollection(otherOwner);
        otherCollection.AddRange((object[])[0, 5]);
        collection.AddRange(otherCollection);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Add empty.
        using ListBox emptyOwner = new();
        var emptyCollection = new ListBox.ObjectCollection(emptyOwner);
        collection.AddRange(emptyCollection);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(SelectionMode.MultiExtended)]
    [InlineData(SelectionMode.MultiSimple)]
    public void ListBoxObjectCollection_AddRange_ObjectCollectionWithMultipleSelectedItemsNotSortedWithHandle_Succes(SelectionMode selectionMode)
    {
        using ListBox owner = new()
        {
            SelectionMode = selectionMode
        };
        ListBox.ObjectCollection collection = owner.Items;
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        collection.Add(5);
        owner.SelectedItems.Add(1);
        owner.SelectedItems.Add(3);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;

        using ListBox otherOwner = new();
        var otherCollection = new ListBox.ObjectCollection(otherOwner);
        otherCollection.AddRange((object[])[0, 2, 4]);
        collection.AddRange(otherCollection);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0, 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Add empty.
        using ListBox emptyOwner = new();
        var emptyCollection = new ListBox.ObjectCollection(emptyOwner);
        collection.AddRange(emptyCollection);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0, 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(SelectionMode.MultiExtended)]
    [InlineData(SelectionMode.MultiSimple)]
    public void ListBoxObjectCollection_AddRange_ObjectCollectionWithMultipleSelectedItemsSortedWithHandle_Succes(SelectionMode selectionMode)
    {
        using ListBox owner = new()
        {
            SelectionMode = selectionMode,
            Sorted = true
        };
        ListBox.ObjectCollection collection = owner.Items;
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);
        owner.SelectedItems.Add(3);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;

        using ListBox otherOwner = new();
        var otherCollection = new ListBox.ObjectCollection(otherOwner);
        otherCollection.AddRange((object[])[0, 2, 5]);
        collection.AddRange(otherCollection);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 3 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Add empty.
        using ListBox emptyOwner = new();
        var emptyCollection = new ListBox.ObjectCollection(emptyOwner);
        collection.AddRange(emptyCollection);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 3 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public unsafe void ListBoxObjectCollection_AddRange_ObjectCollectionCreateHandle_Success()
    {
        using ListBox owner = new();
        var collection = new ListBox.ObjectCollection(owner);
        using ListBox otherOwner = new();
        var otherCollection = new ListBox.ObjectCollection(otherOwner);
        otherCollection.AddRange((object[])[2, 1, 1, 3]);
        collection.AddRange(otherCollection);
        char* textBuffer = stackalloc char[256];

        Assert.Equal(0, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
    }

    [WinFormsFact]
    public unsafe void ListBoxObjectCollection_AddRange_ObjectCollectionItemsCreateHandle_Success()
    {
        using ListBox owner = new();
        ListBox.ObjectCollection collection = owner.Items;
        using ListBox otherOwner = new();
        var otherCollection = new ListBox.ObjectCollection(otherOwner);
        otherCollection.AddRange((object[])[2, 1, 1, 3]);
        collection.AddRange(otherCollection);
        char* textBuffer = stackalloc char[256];

        Assert.Equal(4, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 3, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_AddRange_SameObjectCollectionEmpty_Success()
    {
        using ListBox owner = new();
        var collection = new ListBox.ObjectCollection(owner);
        collection.AddRange(collection);
        Assert.Empty(collection);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_AddRange_SameObjectCollectionNotEmptyOneValue_Success()
    {
        using ListBox owner = new();
        var collection = new ListBox.ObjectCollection(owner)
        {
            1
        };
        Assert.Throws<InvalidOperationException>(() => collection.AddRange(collection));
        Assert.Equal(new object[] { 1, 1 }, collection.Cast<object>());
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_AddRange_SameObjectCollectionNotEmptyMultipleValues_Success()
    {
        using ListBox owner = new();
        var collection = new ListBox.ObjectCollection(owner)
        {
            1,
            2
        };
        Assert.Throws<InvalidOperationException>(() => collection.AddRange(collection));
        Assert.Equal(new object[] { 1, 2, 1 }, collection.Cast<object>());
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_AddRange_NullItems_ThrowsArgumentNullException()
    {
        using ListBox owner = new();
        var collection = new ListBox.ObjectCollection(owner);
        Assert.Throws<ArgumentNullException>("items", () => collection.AddRange((object[])null));
        Assert.Throws<ArgumentNullException>("value", () => collection.AddRange((ListBox.ObjectCollection)null));
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_AddRange_ObjectArrayContainsNullItem_ThrowsArgumentNullException()
    {
        using ListBox owner = new();
        var collection = new ListBox.ObjectCollection(owner);
        Assert.Throws<ArgumentNullException>("item", () => collection.AddRange((object[])[null]));
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_AddRange_OwnerHasDataSource_ThrowsArgumentException()
    {
        using ListBox owner = new()
        {
            DataSource = Array.Empty<object>()
        };
        var collection = new ListBox.ObjectCollection(owner);
        using ListBox otherOwner = new();
        var otherCollection = new ListBox.ObjectCollection(otherOwner);
        Assert.Throws<ArgumentException>(() => collection.AddRange(Array.Empty<object>()));
        Assert.Throws<ArgumentException>(() => collection.AddRange(otherCollection));
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(-2)]
    public void ListBoxObjectCollection_AddRange_ErrorAdding_ThrowsOutOfMemoryException(int result)
    {
        // Note that this is not an actual out of memory, we're artificially setting up an error case
        // that we surface as `OutOfMemoryException` (see ListBox.NativeAdd(Object item))

        using CustomAddStringListBox owner = new()
        {
            AddStringResult = result
        };
        var collection = new ListBox.ObjectCollection(owner);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        using ListBox otherOwner = new();
        var otherCollection = new ListBox.ObjectCollection(otherOwner)
        {
            1
        };

        Assert.Throws<OutOfMemoryException>(() => collection.AddRange((object[])[1]));
        Assert.Throws<OutOfMemoryException>(() => collection.AddRange(otherCollection));
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_Clear_InvokeEmpty_Success()
    {
        using ListBox owner = new();
        var collection = new ListBox.ObjectCollection(owner);
        collection.Clear();
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.False(owner.IsHandleCreated);

        // Call again.
        collection.Clear();
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_Clear_InvokeItemsEmpty_Success()
    {
        using ListBox owner = new();
        ListBox.ObjectCollection collection = owner.Items;
        collection.Clear();
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.False(owner.IsHandleCreated);

        // Call again.
        collection.Clear();
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_Clear_InvokeNotEmpty_Success()
    {
        using ListBox owner = new();
        var collection = new ListBox.ObjectCollection(owner)
        {
            1
        };

        collection.Clear();
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.False(owner.IsHandleCreated);

        // Call again.
        collection.Clear();
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_Clear_InvokeItemsNotEmpty_Success()
    {
        using ListBox owner = new();
        ListBox.ObjectCollection collection = owner.Items;
        collection.Add(1);

        collection.Clear();
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.False(owner.IsHandleCreated);

        // Call again.
        collection.Clear();
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_Clear_InvokeEmptyWithHandle_Success()
    {
        using ListBox owner = new();
        var collection = new ListBox.ObjectCollection(owner);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;

        collection.Clear();
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(0, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));

        // Call again.
        collection.Clear();
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(0, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_Clear_InvokeItemsEmptyWithHandle_Success()
    {
        using ListBox owner = new();
        ListBox.ObjectCollection collection = owner.Items;
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;

        collection.Clear();
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(0, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));

        // Call again.
        collection.Clear();
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(0, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_Clear_InvokeNotEmptyWithHandle_Success()
    {
        using ListBox owner = new();
        var collection = new ListBox.ObjectCollection(owner);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        collection.Add(1);

        collection.Clear();
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(0, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));

        // Call again.
        collection.Clear();
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(0, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_Clear_InvokeItemsNotEmptyWithHandle_Success()
    {
        using ListBox owner = new();
        ListBox.ObjectCollection collection = owner.Items;
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        collection.Add(1);

        collection.Clear();
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(0, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));

        // Call again.
        collection.Clear();
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(0, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_Contains_InvokeEmpty_ReturnsExpected()
    {
        using ListBox owner = new();
        var collection = new ListBox.ObjectCollection(owner);
        Assert.False(collection.Contains(0));
        Assert.False(collection.Contains(1));
        Assert.False(collection.Contains(new object()));
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_Contains_InvokeWithValues_ReturnsExpected()
    {
        using ListBox owner = new();
        var collection = new ListBox.ObjectCollection(owner)
        {
            // Add one.
            1
        };
        Assert.False(collection.Contains(0));
        Assert.True(collection.Contains(1));
        Assert.False(collection.Contains(2));
        Assert.False(collection.Contains(new object()));
        Assert.False(owner.IsHandleCreated);

        // Add another.
        collection.Add(0);
        Assert.True(collection.Contains(0));
        Assert.True(collection.Contains(1));
        Assert.False(collection.Contains(2));
        Assert.False(collection.Contains(new object()));
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_Contains_NullValue_ThrowsArgumentNullException()
    {
        using ListBox owner = new();
        var collection = new ListBox.ObjectCollection(owner);
        Assert.Throws<ArgumentNullException>("value", () => collection.Contains(null));
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_CopyTo_InvokeEmpty_Success()
    {
        using ListBox owner = new();
        var collection = new ListBox.ObjectCollection(owner);
        object[] array = ["1", "2", "3"];
        collection.CopyTo(array, 1);
        Assert.Equal(["1", "2", "3"], array);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_CopyTo_InvokeNotEmpty_ReturnsExpected()
    {
        using ListBox owner = new();
        var collection = new ListBox.ObjectCollection(owner)
        {
            1,
            2
        };
        object[] array = ["1", "2", "3"];
        collection.CopyTo(array, 1);
        Assert.Equal(["1", 1, 2], array);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_CopyTo_NullArrayEmpty_ThrowsArgumentNullException()
    {
        using ListBox owner = new();
        var collection = new ListBox.ObjectCollection(owner);
        Assert.Throws<ArgumentNullException>("destination", () => collection.CopyTo(null, 0));
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_CopyTo_NullArrayNotEmpty_ThrowsArgumentNullException()
    {
        using ListBox owner = new();
        var collection = new ListBox.ObjectCollection(owner)
        {
            1
        };
        Assert.Throws<ArgumentNullException>("destination", () => collection.CopyTo(null, 0));
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_GetEnumerator_InvokeEmpty_ReturnsExpected()
    {
        using ListBox owner = new();
        var collection = new ListBox.ObjectCollection(owner);

        IEnumerator enumerator = collection.GetEnumerator();
        for (int i = 0; i < 2; i++)
        {
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);

            Assert.False(enumerator.MoveNext());
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);

            // Move again.
            Assert.False(enumerator.MoveNext());
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);

            // Reset.
            enumerator.Reset();
        }
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_GetEnumerator_InvokeNotEmpty_ReturnsExpected()
    {
        using ListBox owner = new();
        var collection = new ListBox.ObjectCollection(owner)
        {
            2
        };

        IEnumerator enumerator = collection.GetEnumerator();
        for (int i = 0; i < 2; i++)
        {
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);

            Assert.True(enumerator.MoveNext());
            Assert.Equal(2, enumerator.Current);

            Assert.False(enumerator.MoveNext());
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);

            // Move again.
            Assert.False(enumerator.MoveNext());
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);

            // Reset.
            enumerator.Reset();
        }
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IndexOf_InvokeEmpty_ReturnsExpected()
    {
        using ListBox owner = new();
        var collection = new ListBox.ObjectCollection(owner);
        Assert.Equal(-1, collection.IndexOf(0));
        Assert.Equal(-1, collection.IndexOf(1));
        Assert.Equal(-1, collection.IndexOf(new object()));
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IndexOf_InvokeWithValues_ReturnsExpected()
    {
        using ListBox owner = new();
        var collection = new ListBox.ObjectCollection(owner)
        {
            // Add one.
            1
        };
        Assert.Equal(-1, collection.IndexOf(0));
        Assert.Equal(0, collection.IndexOf(1));
        Assert.Equal(-1, collection.IndexOf(2));
        Assert.Equal(-1, collection.IndexOf(new object()));
        Assert.False(owner.IsHandleCreated);

        // Add another.
        collection.Add(0);
        Assert.Equal(1, collection.IndexOf(0));
        Assert.Equal(0, collection.IndexOf(1));
        Assert.Equal(-1, collection.IndexOf(2));
        Assert.Equal(-1, collection.IndexOf(new object()));
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IndexOf_NullValue_ThrowsArgumentNullException()
    {
        using ListBox owner = new();
        var collection = new ListBox.ObjectCollection(owner);
        Assert.Throws<ArgumentNullException>("value", () => collection.IndexOf(null));
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_Insert_NotSorted_Success()
    {
        using ListBox owner = new();
        var collection = new ListBox.ObjectCollection(owner);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Insert first.
        collection.Insert(0, 1);
        Assert.Single(collection);
        Assert.Equal(new object[] { 1 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Insert end.
        collection.Insert(0, 2);
        Assert.Equal(2, collection.Count);
        Assert.Equal(new object[] { 2, 1 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Insert same.
        collection.Insert(1, 2);
        Assert.Equal(3, collection.Count);
        Assert.Equal(new object[] { 2, 2, 1 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Insert end.
        collection.Insert(3, 3);
        Assert.Equal(4, collection.Count);
        Assert.Equal(new object[] { 2, 2, 1, 3 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_Insert_ItemsNotSorted_Success()
    {
        using ListBox owner = new();
        ListBox.ObjectCollection collection = owner.Items;
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Insert first.
        collection.Insert(0, 1);
        Assert.Single(collection);
        Assert.Equal(new object[] { 1 }, collection.Cast<object>());
        Assert.Equal(new object[] { 1 }, owner.Items.Cast<object>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Insert end.
        collection.Insert(0, 2);
        Assert.Equal(2, collection.Count);
        Assert.Equal(new object[] { 2, 1 }, collection.Cast<object>());
        Assert.Equal(new object[] { 2, 1 }, owner.Items.Cast<object>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Insert same.
        collection.Insert(0, 2);
        Assert.Equal(3, collection.Count);
        Assert.Equal(new object[] { 2, 2, 1 }, collection.Cast<object>());
        Assert.Equal(new object[] { 2, 2, 1 }, owner.Items.Cast<object>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Insert end.
        collection.Insert(3, 3);
        Assert.Equal(4, collection.Count);
        Assert.Equal(new object[] { 2, 2, 1, 3 }, collection.Cast<object>());
        Assert.Equal(new object[] { 2, 2, 1, 3 }, owner.Items.Cast<object>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_Insert_Sorted_Success()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        var collection = new ListBox.ObjectCollection(owner);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Insert first.
        collection.Insert(0, 1);
        Assert.Single(collection);
        Assert.Equal(new object[] { 1 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Insert end.
        collection.Insert(0, 2);
        Assert.Equal(2, collection.Count);
        Assert.Equal(new object[] { 1, 2 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Insert same.
        collection.Insert(0, 2);
        Assert.Equal(3, collection.Count);
        Assert.Equal(new object[] { 1, 2, 2 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Insert end.
        collection.Insert(3, 3);
        Assert.Equal(4, collection.Count);
        Assert.Equal(new object[] { 1, 2, 2, 3 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_Insert_ItemsSorted_Success()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        ListBox.ObjectCollection collection = owner.Items;
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Insert first.
        collection.Insert(0, 1);
        Assert.Single(collection);
        Assert.Equal(new object[] { 1 }, collection.Cast<object>());
        Assert.Equal(new object[] { 1 }, owner.Items.Cast<object>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Insert end.
        collection.Insert(0, 2);
        Assert.Equal(2, collection.Count);
        Assert.Equal(new object[] { 1, 2 }, collection.Cast<object>());
        Assert.Equal(new object[] { 1, 2 }, owner.Items.Cast<object>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Insert same.
        collection.Insert(0, 2);
        Assert.Equal(3, collection.Count);
        Assert.Equal(new object[] { 1, 2, 2 }, collection.Cast<object>());
        Assert.Equal(new object[] { 1, 2, 2 }, owner.Items.Cast<object>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Insert end.
        collection.Insert(3, 3);
        Assert.Equal(4, collection.Count);
        Assert.Equal(new object[] { 1, 2, 2, 3 }, collection.Cast<object>());
        Assert.Equal(new object[] { 1, 2, 2, 3 }, owner.Items.Cast<object>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public unsafe void ListBoxObjectCollection_Insert_NotSortedWithHandle_Success()
    {
        using ListBox owner = new();
        var collection = new ListBox.ObjectCollection(owner);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        char* textBuffer = stackalloc char[256];

        // Insert first.
        collection.Insert(0, 1);
        Assert.Single(collection);
        Assert.Equal(new object[] { 1 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(1, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));

        // Insert end.
        collection.Insert(0, 2);
        Assert.Equal(2, collection.Count);
        Assert.Equal(new object[] { 2, 1 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(2, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));

        // Insert same.
        collection.Insert(0, 2);
        Assert.Equal(3, collection.Count);
        Assert.Equal(new object[] { 2, 2, 1 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(3, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));

        // Insert end.
        collection.Insert(3, 3);
        Assert.Equal(4, collection.Count);
        Assert.Equal(new object[] { 2, 2, 1, 3 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(4, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 3, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));
    }

    [WinFormsFact]
    public unsafe void ListBoxObjectCollection_Insert_ItemsNotSortedWithHandle_Success()
    {
        using ListBox owner = new();
        ListBox.ObjectCollection collection = owner.Items;
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        char* textBuffer = stackalloc char[256];

        // Insert first.
        collection.Insert(0, 1);
        Assert.Single(collection);
        Assert.Equal(new object[] { 1 }, collection.Cast<object>());
        Assert.Equal(new object[] { 1 }, owner.Items.Cast<object>());
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(1, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));

        // Insert end.
        collection.Insert(0, 2);
        Assert.Equal(2, collection.Count);
        Assert.Equal(new object[] { 2, 1 }, collection.Cast<object>());
        Assert.Equal(new object[] { 2, 1 }, owner.Items.Cast<object>());
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(2, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));

        // Insert same.
        collection.Insert(0, 2);
        Assert.Equal(3, collection.Count);
        Assert.Equal(new object[] { 2, 2, 1 }, collection.Cast<object>());
        Assert.Equal(new object[] { 2, 2, 1 }, owner.Items.Cast<object>());
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(3, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));

        // Insert end.
        collection.Insert(3, 3);
        Assert.Equal(4, collection.Count);
        Assert.Equal(new object[] { 2, 2, 1, 3 }, collection.Cast<object>());
        Assert.Equal(new object[] { 2, 2, 1, 3 }, owner.Items.Cast<object>());
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(4, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 3, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));
    }

    [WinFormsFact]
    public unsafe void ListBoxObjectCollection_Insert_SortedWithHandle_Success()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        var collection = new ListBox.ObjectCollection(owner);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        char* textBuffer = stackalloc char[256];

        // Insert first.
        collection.Insert(0, 1);
        Assert.Single(collection);
        Assert.Equal(new object[] { 1 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(1, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));

        // Insert end.
        collection.Insert(0, 2);
        Assert.Equal(2, collection.Count);
        Assert.Equal(new object[] { 1, 2 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(2, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));

        // Insert same.
        collection.Insert(0, 2);
        Assert.Equal(3, collection.Count);
        Assert.Equal(new object[] { 1, 2, 2 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(3, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));

        // Insert end.
        collection.Insert(3, 3);
        Assert.Equal(4, collection.Count);
        Assert.Equal(new object[] { 1, 2, 2, 3 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(4, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 3, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));
    }

    [WinFormsFact]
    public unsafe void ListBoxObjectCollection_Insert_ItemsSortedWithHandle_Success()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        ListBox.ObjectCollection collection = owner.Items;
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        char* textBuffer = stackalloc char[256];

        // Insert first.
        collection.Insert(0, 1);
        Assert.Single(collection);
        Assert.Equal(new object[] { 1 }, collection.Cast<object>());
        Assert.Equal(new object[] { 1 }, owner.Items.Cast<object>());
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(1, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));

        // Insert end.
        collection.Insert(0, 2);
        Assert.Equal(2, collection.Count);
        Assert.Equal(new object[] { 1, 2 }, collection.Cast<object>());
        Assert.Equal(new object[] { 1, 2 }, owner.Items.Cast<object>());
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(2, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));

        // Insert same.
        collection.Insert(0, 2);
        Assert.Equal(3, collection.Count);
        Assert.Equal(new object[] { 1, 2, 2 }, collection.Cast<object>());
        Assert.Equal(new object[] { 1, 2, 2 }, owner.Items.Cast<object>());
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(3, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));

        // Insert end.
        collection.Insert(3, 3);
        Assert.Equal(4, collection.Count);
        Assert.Equal(new object[] { 1, 2, 2, 3 }, collection.Cast<object>());
        Assert.Equal(new object[] { 1, 2, 2, 3 }, owner.Items.Cast<object>());
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(4, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 3, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_Insert_WithOneSelectedItemNotSorted_Success()
    {
        using ListBox owner = new();
        ListBox.ObjectCollection collection = owner.Items;
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Insert before.
        collection.Insert(3, 0);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Insert after.
        collection.Insert(4, 5);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_Insert_WithOneSelectedItemSorted_Success()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        ListBox.ObjectCollection collection = owner.Items;
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);

        // Insert before.
        collection.Insert(3, 0);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1 }, owner.SelectedIndices.Cast<int>());
        Assert.False(owner.IsHandleCreated);

        // Insert after.
        collection.Insert(4, 5);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1 }, owner.SelectedIndices.Cast<int>());
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(SelectionMode.MultiExtended)]
    [InlineData(SelectionMode.MultiSimple)]
    public void ListBoxObjectCollection_Insert_WithMultipleSelectedItemsNotSorted_Success(SelectionMode selectionMode)
    {
        using ListBox owner = new()
        {
            SelectionMode = selectionMode
        };
        ListBox.ObjectCollection collection = owner.Items;
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        collection.Add(5);
        owner.SelectedItems.Add(1);
        owner.SelectedItems.Add(3);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Insert before.
        collection.Insert(3, 0);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0, 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Insert middle.
        collection.Insert(2, 2);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0, 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Insert after.
        collection.Insert(3, 4);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0, 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(SelectionMode.MultiExtended)]
    [InlineData(SelectionMode.MultiSimple)]
    public void ListBoxObjectCollection_Insert_WithMultipleSelectedItemsSorted_Success(SelectionMode selectionMode)
    {
        using ListBox owner = new()
        {
            SelectionMode = selectionMode,
            Sorted = true
        };
        ListBox.ObjectCollection collection = owner.Items;
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);
        owner.SelectedItems.Add(3);

        // Insert before.
        collection.Insert(3, 0);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 2 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.False(owner.IsHandleCreated);

        // Insert middle.
        collection.Insert(2, 2);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 3 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.False(owner.IsHandleCreated);

        // Insert after.
        collection.Insert(4, 5);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 3 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_Insert_WithOneSelectedItemNotSortedWithHandle_Succes()
    {
        using ListBox owner = new();
        ListBox.ObjectCollection collection = owner.Items;
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;

        // Insert before.
        collection.Insert(3, 0);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Insert after.
        collection.Insert(4, 5);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_Insert_WithOneSelectedItemSortedWithHandle_Succes()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        ListBox.ObjectCollection collection = owner.Items;
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;

        // Insert before.
        collection.Insert(3, 0);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Insert after.
        collection.Insert(4, 5);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(SelectionMode.MultiExtended)]
    [InlineData(SelectionMode.MultiSimple)]
    public void ListBoxObjectCollection_Insert_WithMultipleSelectedItemsNotSortedWithHandle_Succes(SelectionMode selectionMode)
    {
        using ListBox owner = new()
        {
            SelectionMode = selectionMode
        };
        ListBox.ObjectCollection collection = owner.Items;
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        collection.Add(5);
        owner.SelectedItems.Add(1);
        owner.SelectedItems.Add(3);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;

        // Insert before.
        collection.Insert(3, 0);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0, 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Insert middle.
        collection.Insert(2, 2);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0, 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Insert after.
        collection.Insert(3, 4);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0, 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(SelectionMode.MultiExtended)]
    [InlineData(SelectionMode.MultiSimple)]
    public void ListBoxObjectCollection_Insert_WithMultipleSelectedItemsSortedWithHandle_Succes(SelectionMode selectionMode)
    {
        using ListBox owner = new()
        {
            SelectionMode = selectionMode,
            Sorted = true
        };
        ListBox.ObjectCollection collection = owner.Items;
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);
        owner.SelectedItems.Add(3);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;

        // Insert before.
        collection.Insert(3, 0);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 2 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Insert middle.
        collection.Insert(2, 2);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 3 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Insert after.
        collection.Insert(4, 5);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 3 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public unsafe void ListBoxObjectCollection_Insert_CreateHandle_Success()
    {
        using ListBox owner = new();
        var collection = new ListBox.ObjectCollection(owner);
        collection.Insert(0, 2);
        collection.Insert(1, 1);
        collection.Insert(2, 1);
        collection.Insert(3, 3);
        char* textBuffer = stackalloc char[256];

        Assert.Equal(0, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
    }

    [WinFormsFact]
    public unsafe void ListBoxObjectCollection_Insert_ItemsCreateHandle_Success()
    {
        using ListBox owner = new();
        ListBox.ObjectCollection collection = owner.Items;
        collection.Insert(0, 2);
        collection.Insert(1, 1);
        collection.Insert(2, 1);
        collection.Insert(3, 3);
        char* textBuffer = stackalloc char[256];

        Assert.Equal(4, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 3, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(1)]
    [InlineData(2)]
    public void ListBoxObjectCollection_Insert_InvalidIndexEmpty_ThrowsArgumentOutOfRangeException(int index)
    {
        using ListBox owner = new();
        var collection = new ListBox.ObjectCollection(owner);
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.Insert(index, 1));
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(2)]
    [InlineData(3)]
    public void ListBoxObjectCollection_Insert_InvalidIndexNotEmpty_ThrowsArgumentOutOfRangeException(int index)
    {
        using ListBox owner = new();
        var collection = new ListBox.ObjectCollection(owner)
        {
            1
        };
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.Insert(index, 1));
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_Insert_NullItem_ThrowsArgumentNullException()
    {
        using ListBox owner = new();
        var collection = new ListBox.ObjectCollection(owner);
        Assert.Throws<ArgumentNullException>("item", () => collection.Insert(0, null));
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_Insert_OwnerHasDataSource_ThrowsArgumentException()
    {
        using ListBox owner = new()
        {
            DataSource = Array.Empty<object>()
        };
        var collection = new ListBox.ObjectCollection(owner);
        Assert.Throws<ArgumentException>(() => collection.Insert(0, 1));
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(-2)]
    public void ListBoxObjectCollection_Insert_ErrorAdding_ThrowsOutOfMemoryException(int result)
    {
        // Note that this is not an actual out of memory, we're artificially setting up an error case
        // that we surface as `OutOfMemoryException` (see ListBox.NativeAdd(Object item))

        using CustomInsertStringListBox owner = new()
        {
            InsertStringResult = result
        };
        var collection = new ListBox.ObjectCollection(owner);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        Assert.Throws<OutOfMemoryException>(() => collection.Insert(0, 1));
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_Remove_NotSorted_Success()
    {
        using ListBox owner = new();
        var collection = new ListBox.ObjectCollection(owner)
        {
            2,
            1,
            3
        };
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Remove middle.
        collection.Remove(1);
        Assert.Equal(2, collection.Count);
        Assert.Equal(new int[] { 2, 3 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Remove again.
        collection.Remove(1);
        Assert.Equal(2, collection.Count);
        Assert.Equal(new int[] { 2, 3 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Remove last.
        collection.Remove(3);
        Assert.Single(collection);
        Assert.Equal(new int[] { 2 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Remove first.
        collection.Remove(2);
        Assert.Empty(collection);
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_Remove_ItemsNotSorted_Success()
    {
        using ListBox owner = new();
        ListBox.ObjectCollection collection = owner.Items;
        collection.Add(2);
        collection.Add(1);
        collection.Add(3);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Remove middle.
        collection.Remove(1);
        Assert.Equal(2, collection.Count);
        Assert.Equal(new int[] { 2, 3 }, collection.Cast<int>());
        Assert.Equal(new int[] { 2, 3 }, owner.Items.Cast<int>());
        Assert.False(owner.IsHandleCreated);

        // Remove again.
        collection.Remove(1);
        Assert.Equal(2, collection.Count);
        Assert.Equal(new int[] { 2, 3 }, collection.Cast<int>());
        Assert.Equal(new int[] { 2, 3 }, owner.Items.Cast<int>());
        Assert.False(owner.IsHandleCreated);

        // Remove last.
        collection.Remove(3);
        Assert.Single(collection);
        Assert.Equal(new int[] { 2 }, collection.Cast<int>());
        Assert.Equal(new int[] { 2 }, owner.Items.Cast<int>());
        Assert.False(owner.IsHandleCreated);

        // Remove first.
        collection.Remove(2);
        Assert.Empty(collection);
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_Remove_Sorted_Success()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        var collection = new ListBox.ObjectCollection(owner)
        {
            2,
            1,
            3
        };
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Remove middle.
        collection.Remove(2);
        Assert.Equal(2, collection.Count);
        Assert.Equal(new int[] { 1, 3 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Remove again.
        collection.Remove(2);
        Assert.Equal(2, collection.Count);
        Assert.Equal(new int[] { 1, 3 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Remove last.
        collection.Remove(3);
        Assert.Single(collection);
        Assert.Equal(new int[] { 1 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Remove first.
        collection.Remove(1);
        Assert.Empty(collection);
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_Remove_ItemsSorted_Success()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        ListBox.ObjectCollection collection = owner.Items;
        collection.Add(2);
        collection.Add(1);
        collection.Add(3);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Remove middle.
        collection.Remove(2);
        Assert.Equal(2, collection.Count);
        Assert.Equal(new int[] { 1, 3 }, collection.Cast<int>());
        Assert.Equal(new int[] { 1, 3 }, owner.Items.Cast<int>());
        Assert.False(owner.IsHandleCreated);

        // Remove again.
        collection.Remove(2);
        Assert.Equal(2, collection.Count);
        Assert.Equal(new int[] { 1, 3 }, collection.Cast<int>());
        Assert.Equal(new int[] { 1, 3 }, owner.Items.Cast<int>());
        Assert.False(owner.IsHandleCreated);

        // Remove last.
        collection.Remove(3);
        Assert.Single(collection);
        Assert.Equal(new int[] { 1 }, collection.Cast<int>());
        Assert.Equal(new int[] { 1 }, owner.Items.Cast<int>());
        Assert.False(owner.IsHandleCreated);

        // Remove first.
        collection.Remove(1);
        Assert.Empty(collection);
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public unsafe void ListBoxObjectCollection_Remove_NotSortedWithHandle_Success()
    {
        using ListBox owner = new();
        var collection = new ListBox.ObjectCollection(owner);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        collection.Add(2);
        collection.Add(1);
        collection.Add(3);
        char* textBuffer = stackalloc char[256];

        // Remove middle.
        collection.Remove(1);
        Assert.Equal(2, collection.Count);
        Assert.Equal(new int[] { 2, 3 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(2, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));

        // Remove again.
        collection.Remove(1);
        Assert.Equal(2, collection.Count);
        Assert.Equal(new int[] { 2, 3 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(2, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));

        // Remove last.
        collection.Remove(3);
        Assert.Single(collection);
        Assert.Equal(new int[] { 2 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(1, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));

        // Remove first.
        collection.Remove(2);
        Assert.Empty(collection);
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(0, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
    }

    [WinFormsFact]
    public unsafe void ListBoxObjectCollection_Remove_ItemsNotSortedWithHandle_Success()
    {
        using ListBox owner = new();
        ListBox.ObjectCollection collection = owner.Items;
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        collection.Add(2);
        collection.Add(1);
        collection.Add(3);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;
        char* textBuffer = stackalloc char[256];

        // Remove middle.
        collection.Remove(1);
        Assert.Equal(2, collection.Count);
        Assert.Equal(new int[] { 2, 3 }, collection.Cast<int>());
        Assert.Equal(new int[] { 2, 3 }, owner.Items.Cast<int>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(2, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));

        // Remove again.
        collection.Remove(1);
        Assert.Equal(2, collection.Count);
        Assert.Equal(new int[] { 2, 3 }, collection.Cast<int>());
        Assert.Equal(new int[] { 2, 3 }, owner.Items.Cast<int>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(2, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));

        // Remove last.
        collection.Remove(3);
        Assert.Single(collection);
        Assert.Equal(new int[] { 2 }, collection.Cast<int>());
        Assert.Equal(new int[] { 2 }, owner.Items.Cast<int>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(1, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));

        // Remove first.
        collection.Remove(2);
        Assert.Empty(collection);
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(0, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
    }

    [WinFormsFact]
    public unsafe void ListBoxObjectCollection_Remove_SortedWithHandle_Success()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        var collection = new ListBox.ObjectCollection(owner);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        collection.Add(2);
        collection.Add(1);
        collection.Add(3);
        char* textBuffer = stackalloc char[256];

        // Remove middle.
        collection.Remove(2);
        Assert.Equal(2, collection.Count);
        Assert.Equal(new int[] { 1, 3 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(2, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));

        // Remove again.
        collection.Remove(2);
        Assert.Equal(2, collection.Count);
        Assert.Equal(new int[] { 1, 3 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(2, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));

        // Remove last.
        collection.Remove(3);
        Assert.Single(collection);
        Assert.Equal(new int[] { 1 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(1, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));

        // Remove first.
        collection.Remove(1);
        Assert.Empty(collection);
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(0, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
    }

    [WinFormsFact]
    public unsafe void ListBoxObjectCollection_Remove_ItemsSortedWithHandle_Success()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        ListBox.ObjectCollection collection = owner.Items;
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        collection.Add(2);
        collection.Add(1);
        collection.Add(3);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;
        char* textBuffer = stackalloc char[256];

        // Remove middle.
        collection.Remove(2);
        Assert.Equal(2, collection.Count);
        Assert.Equal(new int[] { 1, 3 }, collection.Cast<int>());
        Assert.Equal(new int[] { 1, 3 }, owner.Items.Cast<int>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(2, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));

        // Remove again.
        collection.Remove(2);
        Assert.Equal(2, collection.Count);
        Assert.Equal(new int[] { 1, 3 }, collection.Cast<int>());
        Assert.Equal(new int[] { 1, 3 }, owner.Items.Cast<int>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(2, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));

        // Remove last.
        collection.Remove(3);
        Assert.Single(collection);
        Assert.Equal(new int[] { 1 }, collection.Cast<int>());
        Assert.Equal(new int[] { 1 }, owner.Items.Cast<int>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(1, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));

        // Remove first.
        collection.Remove(1);
        Assert.Empty(collection);
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(0, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_Remove_WithOneSelectedItemNotSorted_Success()
    {
        using ListBox owner = new();
        ListBox.ObjectCollection collection = owner.Items;
        collection.Add(0);
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Remove before.
        collection.Remove(0);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Remove after.
        collection.Remove(4);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Remove selected.
        collection.Remove(1);
        Assert.Empty(owner.SelectedItems);
        Assert.Empty(owner.SelectedIndices);
        Assert.Equal(-1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_Remove_WithOneSelectedItemSorted_Success()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        ListBox.ObjectCollection collection = owner.Items;
        collection.Add(0);
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Remove before.
        collection.Remove(0);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Remove after.
        collection.Remove(4);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Remove selected.
        collection.Remove(1);
        Assert.Empty(owner.SelectedItems);
        Assert.Empty(owner.SelectedIndices);
        Assert.Equal(-1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(SelectionMode.MultiExtended)]
    [InlineData(SelectionMode.MultiSimple)]
    public void ListBoxObjectCollection_Remove_WithMultipleSelectedItemsNotSorted_Success(SelectionMode selectionMode)
    {
        using ListBox owner = new()
        {
            SelectionMode = selectionMode
        };
        ListBox.ObjectCollection collection = owner.Items;
        collection.Add(0);
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        collection.Add(5);
        owner.SelectedItems.Add(1);
        owner.SelectedItems.Add(3);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Remove before.
        collection.Remove(0);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0, 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Remove after.
        collection.Remove(4);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0, 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Remove first selection.
        collection.Remove(1);
        Assert.Equal(new object[] { 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Remove last selection.
        collection.Remove(3);
        Assert.Empty(owner.SelectedItems);
        Assert.Empty(owner.SelectedIndices);
        Assert.Equal(-1, owner.SelectedIndex);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(SelectionMode.MultiExtended)]
    [InlineData(SelectionMode.MultiSimple)]
    public void ListBoxObjectCollection_Remove_WithMultipleSelectedItemsSorted_Success(SelectionMode selectionMode)
    {
        using ListBox owner = new()
        {
            SelectionMode = selectionMode,
            Sorted = true
        };
        ListBox.ObjectCollection collection = owner.Items;
        collection.Add(0);
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);
        owner.SelectedItems.Add(3);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Remove before.
        collection.Remove(0);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0, 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Remove after.
        collection.Remove(4);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0, 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Remove first selection.
        collection.Remove(1);
        Assert.Equal(new object[] { 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Remove last selection.
        collection.Remove(3);
        Assert.Empty(owner.SelectedItems);
        Assert.Empty(owner.SelectedIndices);
        Assert.Equal(-1, owner.SelectedIndex);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_Remove_WithOneSelectedItemNotSortedWithHandle_Success()
    {
        using ListBox owner = new();
        ListBox.ObjectCollection collection = owner.Items;
        collection.Add(0);
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) =>
        {
            Assert.Same(owner, sender);
            Assert.Same(EventArgs.Empty, e);
            selectedIndexChangedCallCount++;
        };

        // Remove before.
        collection.Remove(0);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove after.
        collection.Remove(4);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove selected.
        collection.Remove(1);
        Assert.Empty(owner.SelectedItems);
        Assert.Empty(owner.SelectedIndices);
        Assert.Equal(-1, owner.SelectedIndex);
        Assert.Equal(1, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_Remove_WithOneSelectedItemSortedWithHandle_Success()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        ListBox.ObjectCollection collection = owner.Items;
        collection.Add(0);
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) =>
        {
            Assert.Same(owner, sender);
            Assert.Same(EventArgs.Empty, e);
            selectedIndexChangedCallCount++;
        };

        // Remove before.
        collection.Remove(0);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove after.
        collection.Remove(4);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove selected.
        collection.Remove(1);
        Assert.Empty(owner.SelectedItems);
        Assert.Empty(owner.SelectedIndices);
        Assert.Equal(-1, owner.SelectedIndex);
        Assert.Equal(1, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(SelectionMode.MultiExtended)]
    [InlineData(SelectionMode.MultiSimple)]
    public void ListBoxObjectCollection_Remove_WithMultipleSelectedItemsNotSortedWithHandle_Success(SelectionMode selectionMode)
    {
        using ListBox owner = new()
        {
            SelectionMode = selectionMode
        };
        ListBox.ObjectCollection collection = owner.Items;
        collection.Add(0);
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        collection.Add(5);
        owner.SelectedItems.Add(1);
        owner.SelectedItems.Add(3);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Remove before.
        collection.Remove(0);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0, 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove after.
        collection.Remove(4);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0, 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove first selection.
        collection.Remove(1);
        Assert.Equal(new object[] { 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove last selection.
        collection.Remove(3);
        Assert.Empty(owner.SelectedItems);
        Assert.Empty(owner.SelectedIndices);
        Assert.Equal(-1, owner.SelectedIndex);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(SelectionMode.MultiExtended)]
    [InlineData(SelectionMode.MultiSimple)]
    public void ListBoxObjectCollection_Remove_WithMultipleSelectedItemsSortedWithHandle_Success(SelectionMode selectionMode)
    {
        using ListBox owner = new()
        {
            SelectionMode = selectionMode,
            Sorted = true
        };
        ListBox.ObjectCollection collection = owner.Items;
        collection.Add(0);
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);
        owner.SelectedItems.Add(3);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Remove before.
        collection.Remove(0);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0, 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove after.
        collection.Remove(4);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0, 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove first selection.
        collection.Remove(1);
        Assert.Equal(new object[] { 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove last selection.
        collection.Remove(3);
        Assert.Empty(owner.SelectedItems);
        Assert.Empty(owner.SelectedIndices);
        Assert.Equal(-1, owner.SelectedIndex);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_Remove_OwnerHasDataSourceEmpty_ThrowsArgumentException()
    {
        using ListBox owner = new()
        {
            DataSource = Array.Empty<object>()
        };
        var collection = new ListBox.ObjectCollection(owner);
        Assert.Throws<ArgumentException>(() => collection.Remove(1));
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_Remove_OwnerHasDataSourceNotEmpty_ThrowsArgumentException()
    {
        using ListBox owner = new()
        {
            DataSource = new object[] { 1 }
        };
        var collection = new ListBox.ObjectCollection(owner);
        Assert.Throws<ArgumentException>(() => collection.Remove(1));
        Assert.Throws<ArgumentException>(() => collection.Remove(2));
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_RemoveAt_NotSorted_Success()
    {
        using ListBox owner = new();
        var collection = new ListBox.ObjectCollection(owner)
        {
            2,
            1,
            3
        };
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Remove middle.
        collection.RemoveAt(1);
        Assert.Equal(2, collection.Count);
        Assert.Equal(new int[] { 2, 3 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Remove last.
        collection.RemoveAt(1);
        Assert.Single(collection);
        Assert.Equal(new int[] { 2 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Remove first.
        collection.RemoveAt(0);
        Assert.Empty(collection);
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_RemoveAt_ItemsNotSorted_Success()
    {
        using ListBox owner = new();
        ListBox.ObjectCollection collection = owner.Items;
        collection.Add(2);
        collection.Add(1);
        collection.Add(3);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Remove middle.
        collection.RemoveAt(1);
        Assert.Equal(2, collection.Count);
        Assert.Equal(new int[] { 2, 3 }, collection.Cast<int>());
        Assert.Equal(new int[] { 2, 3 }, owner.Items.Cast<int>());
        Assert.False(owner.IsHandleCreated);

        // Remove last.
        collection.RemoveAt(1);
        Assert.Single(collection);
        Assert.Equal(new int[] { 2 }, collection.Cast<int>());
        Assert.Equal(new int[] { 2 }, owner.Items.Cast<int>());
        Assert.False(owner.IsHandleCreated);

        // Remove first.
        collection.RemoveAt(0);
        Assert.Empty(collection);
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_RemoveAt_Sorted_Success()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        var collection = new ListBox.ObjectCollection(owner)
        {
            2,
            1,
            3
        };
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Remove middle.
        collection.RemoveAt(1);
        Assert.Equal(2, collection.Count);
        Assert.Equal(new int[] { 1, 3 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Remove last.
        collection.RemoveAt(1);
        Assert.Single(collection);
        Assert.Equal(new int[] { 1 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Remove first.
        collection.RemoveAt(0);
        Assert.Empty(collection);
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_RemoveAt_ItemsSorted_Success()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        ListBox.ObjectCollection collection = owner.Items;
        collection.Add(2);
        collection.Add(1);
        collection.Add(3);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Remove middle.
        collection.RemoveAt(1);
        Assert.Equal(2, collection.Count);
        Assert.Equal(new int[] { 1, 3 }, collection.Cast<int>());
        Assert.Equal(new int[] { 1, 3 }, owner.Items.Cast<int>());
        Assert.False(owner.IsHandleCreated);

        // Remove last.
        collection.RemoveAt(1);
        Assert.Single(collection);
        Assert.Equal(new int[] { 1 }, collection.Cast<int>());
        Assert.Equal(new int[] { 1 }, owner.Items.Cast<int>());
        Assert.False(owner.IsHandleCreated);

        // Remove first.
        collection.RemoveAt(0);
        Assert.Empty(collection);
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public unsafe void ListBoxObjectCollection_RemoveAt_NotSortedWithHandle_Success()
    {
        using ListBox owner = new();
        var collection = new ListBox.ObjectCollection(owner);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        collection.Add(2);
        collection.Add(1);
        collection.Add(3);
        char* textBuffer = stackalloc char[256];

        // Remove middle.
        collection.RemoveAt(1);
        Assert.Equal(2, collection.Count);
        Assert.Equal(new int[] { 2, 3 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(2, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));

        // Remove last.
        collection.RemoveAt(1);
        Assert.Single(collection);
        Assert.Equal(new int[] { 2 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(1, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));

        // Remove first.
        collection.RemoveAt(0);
        Assert.Empty(collection);
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(0, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
    }

    [WinFormsFact]
    public unsafe void ListBoxObjectCollection_RemoveAt_ItemsNotSortedWithHandle_Success()
    {
        using ListBox owner = new();
        ListBox.ObjectCollection collection = owner.Items;
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        collection.Add(2);
        collection.Add(1);
        collection.Add(3);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;
        char* textBuffer = stackalloc char[256];

        // Remove middle.
        collection.RemoveAt(1);
        Assert.Equal(2, collection.Count);
        Assert.Equal(new int[] { 2, 3 }, collection.Cast<int>());
        Assert.Equal(new int[] { 2, 3 }, owner.Items.Cast<int>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(2, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));

        // Remove last.
        collection.RemoveAt(1);
        Assert.Single(collection);
        Assert.Equal(new int[] { 2 }, collection.Cast<int>());
        Assert.Equal(new int[] { 2 }, owner.Items.Cast<int>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(1, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));

        // Remove first.
        collection.RemoveAt(0);
        Assert.Empty(collection);
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(0, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
    }

    [WinFormsFact]
    public unsafe void ListBoxObjectCollection_RemoveAt_SortedWithHandle_Success()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        var collection = new ListBox.ObjectCollection(owner);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        collection.Add(2);
        collection.Add(1);
        collection.Add(3);
        char* textBuffer = stackalloc char[256];

        // Remove middle.
        collection.RemoveAt(1);
        Assert.Equal(2, collection.Count);
        Assert.Equal(new int[] { 1, 3 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(2, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));

        // Remove last.
        collection.RemoveAt(1);
        Assert.Single(collection);
        Assert.Equal(new int[] { 1 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(1, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));

        // Remove first.
        collection.RemoveAt(0);
        Assert.Empty(collection);
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(0, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
    }

    [WinFormsFact]
    public unsafe void ListBoxObjectCollection_RemoveAt_ItemsSortedWithHandle_Success()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        ListBox.ObjectCollection collection = owner.Items;
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        collection.Add(2);
        collection.Add(1);
        collection.Add(3);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;
        char* textBuffer = stackalloc char[256];

        // Remove middle.
        collection.RemoveAt(1);
        Assert.Equal(2, collection.Count);
        Assert.Equal(new int[] { 1, 3 }, collection.Cast<int>());
        Assert.Equal(new int[] { 1, 3 }, owner.Items.Cast<int>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(2, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));

        // Remove last.
        collection.RemoveAt(1);
        Assert.Single(collection);
        Assert.Equal(new int[] { 1 }, collection.Cast<int>());
        Assert.Equal(new int[] { 1 }, owner.Items.Cast<int>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(1, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));

        // Remove first.
        collection.RemoveAt(0);
        Assert.Empty(collection);
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(0, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_RemoveAt_WithOneSelectedItemNotSorted_Success()
    {
        using ListBox owner = new();
        ListBox.ObjectCollection collection = owner.Items;
        collection.Add(0);
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Remove before.
        collection.RemoveAt(0);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Remove after.
        collection.RemoveAt(2);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Remove selected.
        collection.RemoveAt(0);
        Assert.Empty(owner.SelectedItems);
        Assert.Empty(owner.SelectedIndices);
        Assert.Equal(-1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_RemoveAt_WithOneSelectedItemSorted_Success()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        ListBox.ObjectCollection collection = owner.Items;
        collection.Add(0);
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Remove before.
        collection.RemoveAt(0);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Remove after.
        collection.RemoveAt(2);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Remove selected.
        collection.RemoveAt(0);
        Assert.Empty(owner.SelectedItems);
        Assert.Empty(owner.SelectedIndices);
        Assert.Equal(-1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(SelectionMode.MultiExtended)]
    [InlineData(SelectionMode.MultiSimple)]
    public void ListBoxObjectCollection_RemoveAt_WithMultipleSelectedItemsNotSorted_Success(SelectionMode selectionMode)
    {
        using ListBox owner = new()
        {
            SelectionMode = selectionMode
        };
        ListBox.ObjectCollection collection = owner.Items;
        collection.Add(0);
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        collection.Add(5);
        owner.SelectedItems.Add(1);
        owner.SelectedItems.Add(3);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Remove before.
        collection.RemoveAt(0);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0, 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Remove after.
        collection.RemoveAt(2);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0, 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Remove first selection.
        collection.RemoveAt(0);
        Assert.Equal(new object[] { 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Remove last selection.
        collection.RemoveAt(0);
        Assert.Empty(owner.SelectedItems);
        Assert.Empty(owner.SelectedIndices);
        Assert.Equal(-1, owner.SelectedIndex);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(SelectionMode.MultiExtended)]
    [InlineData(SelectionMode.MultiSimple)]
    public void ListBoxObjectCollection_RemoveAt_WithMultipleSelectedItemsSorted_Success(SelectionMode selectionMode)
    {
        using ListBox owner = new()
        {
            SelectionMode = selectionMode,
            Sorted = true
        };
        ListBox.ObjectCollection collection = owner.Items;
        collection.Add(0);
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);
        owner.SelectedItems.Add(3);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Remove before.
        collection.RemoveAt(0);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0, 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Remove after.
        collection.RemoveAt(2);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0, 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Remove first selection.
        collection.RemoveAt(0);
        Assert.Equal(new object[] { 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Remove last selection.
        collection.RemoveAt(0);
        Assert.Empty(owner.SelectedItems);
        Assert.Empty(owner.SelectedIndices);
        Assert.Equal(-1, owner.SelectedIndex);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_RemoveAt_WithOneSelectedItemNotSortedWithHandle_Success()
    {
        using ListBox owner = new();
        ListBox.ObjectCollection collection = owner.Items;
        collection.Add(0);
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) =>
        {
            Assert.Same(owner, sender);
            Assert.Same(EventArgs.Empty, e);
            selectedIndexChangedCallCount++;
        };

        // Remove before.
        collection.RemoveAt(0);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove after.
        collection.RemoveAt(2);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove selected.
        collection.RemoveAt(0);
        Assert.Empty(owner.SelectedItems);
        Assert.Empty(owner.SelectedIndices);
        Assert.Equal(-1, owner.SelectedIndex);
        Assert.Equal(1, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_RemoveAt_WithOneSelectedItemSortedWithHandle_Success()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        ListBox.ObjectCollection collection = owner.Items;
        collection.Add(0);
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) =>
        {
            Assert.Same(owner, sender);
            Assert.Same(EventArgs.Empty, e);
            selectedIndexChangedCallCount++;
        };

        // Remove before.
        collection.RemoveAt(0);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove after.
        collection.RemoveAt(2);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove selected.
        collection.RemoveAt(0);
        Assert.Empty(owner.SelectedItems);
        Assert.Empty(owner.SelectedIndices);
        Assert.Equal(-1, owner.SelectedIndex);
        Assert.Equal(1, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(SelectionMode.MultiExtended)]
    [InlineData(SelectionMode.MultiSimple)]
    public void ListBoxObjectCollection_RemoveAt_WithMultipleSelectedItemsNotSortedWithHandle_Success(SelectionMode selectionMode)
    {
        using ListBox owner = new()
        {
            SelectionMode = selectionMode
        };
        ListBox.ObjectCollection collection = owner.Items;
        collection.Add(0);
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        collection.Add(5);
        owner.SelectedItems.Add(1);
        owner.SelectedItems.Add(3);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Remove before.
        collection.RemoveAt(0);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0, 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove after.
        collection.RemoveAt(2);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0, 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove first selection.
        collection.RemoveAt(0);
        Assert.Equal(new object[] { 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove last selection.
        collection.RemoveAt(0);
        Assert.Empty(owner.SelectedItems);
        Assert.Empty(owner.SelectedIndices);
        Assert.Equal(-1, owner.SelectedIndex);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(SelectionMode.MultiExtended)]
    [InlineData(SelectionMode.MultiSimple)]
    public void ListBoxObjectCollection_RemoveAt_WithMultipleSelectedItemsSortedWithHandle_Success(SelectionMode selectionMode)
    {
        using ListBox owner = new()
        {
            SelectionMode = selectionMode,
            Sorted = true
        };
        ListBox.ObjectCollection collection = owner.Items;
        collection.Add(0);
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);
        owner.SelectedItems.Add(3);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Remove before.
        collection.RemoveAt(0);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0, 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove after.
        collection.RemoveAt(2);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0, 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove first selection.
        collection.RemoveAt(0);
        Assert.Equal(new object[] { 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove last selection.
        collection.RemoveAt(0);
        Assert.Empty(owner.SelectedItems);
        Assert.Empty(owner.SelectedIndices);
        Assert.Equal(-1, owner.SelectedIndex);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void ListBoxObjectCollection_RemoveAt_InvalidIndexEmpty_ThrowsArgumentOutOfRangeException(int index)
    {
        using ListBox owner = new();
        var collection = new ListBox.ObjectCollection(owner);
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.RemoveAt(index));
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(1)]
    [InlineData(2)]
    public void ListBoxObjectCollection_RemoveAt_InvalidIndexNotEmpty_ThrowsArgumentOutOfRangeException(int index)
    {
        using ListBox owner = new();
        var collection = new ListBox.ObjectCollection(owner)
        {
            1
        };
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.RemoveAt(index));
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_RemoveAt_OwnerHasDataSourceEmpty_ThrowsArgumentException()
    {
        using ListBox owner = new()
        {
            DataSource = Array.Empty<object>()
        };
        var collection = new ListBox.ObjectCollection(owner);
        Assert.Throws<ArgumentException>(() => collection.RemoveAt(1));
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_RemoveAt_OwnerHasDataSourceNotEmpty_ThrowsArgumentException()
    {
        using ListBox owner = new()
        {
            DataSource = new object[] { 1 }
        };
        var collection = new ListBox.ObjectCollection(owner);
        Assert.Throws<ArgumentException>(() => collection.RemoveAt(1));
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListItem_Get_ReturnsExpected()
    {
        using ListBox owner = new();
        IList collection = new ListBox.ObjectCollection(owner)
        {
            2,
            1,
            1,
            3
        };

        Assert.Equal(2, collection[0]);
        Assert.Equal(1, collection[1]);
        Assert.Equal(1, collection[2]);
        Assert.Equal(3, collection[3]);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListItem_GetSorted_ReturnsExpected()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        IList collection = new ListBox.ObjectCollection(owner)
        {
            2,
            1,
            1,
            3
        };

        Assert.Equal(1, collection[0]);
        Assert.Equal(1, collection[1]);
        Assert.Equal(2, collection[2]);
        Assert.Equal(3, collection[3]);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void ListBoxObjectCollection_IListItem_GetInvalidIndexEmpty_ThrowsArgumentOutOfRangeException(int index)
    {
        using ListBox owner = new();
        IList collection = new ListBox.ObjectCollection(owner);
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index]);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(1)]
    [InlineData(2)]
    public void ListBoxObjectCollection_IListItem_GetInvalidIndexNotEmpty_ThrowsArgumentOutOfRangeException(int index)
    {
        using ListBox owner = new();
        IList collection = new ListBox.ObjectCollection(owner)
        {
            1
        };
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index]);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListItem_SetNotSorted_GetReturnsExpected()
    {
        using ListBox owner = new();
        IList collection = new ListBox.ObjectCollection(owner)
        {
            2,
            1,
            3
        };
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Set first.
        collection[0] = 4;
        Assert.Equal(new int[] { 4, 1, 3 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Set middle.
        collection[1] = 2;
        Assert.Equal(new int[] { 4, 2, 3 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Set last.
        collection[2] = 5;
        Assert.Equal(new int[] { 4, 2, 5 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Set same.
        collection[2] = 5;
        Assert.Equal(new int[] { 4, 2, 5 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListItem_SetSorted_GetReturnsExpected()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        IList collection = new ListBox.ObjectCollection(owner)
        {
            2,
            1,
            3
        };
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Set first.
        collection[0] = 4;
        Assert.Equal(new int[] { 4, 2, 3 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Set middle.
        collection[1] = 1;
        Assert.Equal(new int[] { 4, 1, 3 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Set last.
        collection[2] = 5;
        Assert.Equal(new int[] { 4, 1, 5 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Set same.
        collection[2] = 5;
        Assert.Equal(new int[] { 4, 1, 5 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListItem_SetItems_GetReturnsExpected()
    {
        using ListBox owner = new();
        IList collection = owner.Items;
        collection.Add(2);
        collection.Add(1);
        collection.Add(3);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Set first.
        collection[0] = 4;
        Assert.Equal(new int[] { 4, 1, 3 }, collection.Cast<int>());
        Assert.Equal(new int[] { 4, 1, 3 }, owner.Items.Cast<int>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Set middle.
        collection[1] = 2;
        Assert.Equal(new int[] { 4, 2, 3 }, collection.Cast<int>());
        Assert.Equal(new int[] { 4, 2, 3 }, owner.Items.Cast<int>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Set last.
        collection[2] = 5;
        Assert.Equal(new int[] { 4, 2, 5 }, collection.Cast<int>());
        Assert.Equal(new int[] { 4, 2, 5 }, owner.Items.Cast<int>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Set same.
        collection[2] = 5;
        Assert.Equal(new int[] { 4, 2, 5 }, collection.Cast<int>());
        Assert.Equal(new int[] { 4, 2, 5 }, owner.Items.Cast<int>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListItem_SetItemsSorted_GetReturnsExpected()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        IList collection = owner.Items;
        collection.Add(2);
        collection.Add(1);
        collection.Add(3);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Set first.
        collection[0] = 4;
        Assert.Equal(new int[] { 4, 2, 3 }, collection.Cast<int>());
        Assert.Equal(new int[] { 4, 2, 3 }, owner.Items.Cast<int>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Set middle.
        collection[1] = 1;
        Assert.Equal(new int[] { 4, 1, 3 }, collection.Cast<int>());
        Assert.Equal(new int[] { 4, 1, 3 }, owner.Items.Cast<int>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Set last.
        collection[2] = 5;
        Assert.Equal(new int[] { 4, 1, 5 }, collection.Cast<int>());
        Assert.Equal(new int[] { 4, 1, 5 }, owner.Items.Cast<int>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Set same.
        collection[2] = 5;
        Assert.Equal(new int[] { 4, 1, 5 }, collection.Cast<int>());
        Assert.Equal(new int[] { 4, 1, 5 }, owner.Items.Cast<int>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

#if false
    [WinFormsFact]
    public unsafe void ListBoxObjectCollection_IListItem_SetNotSortedWithHandle_GetReturnsExpected()
    {
        using ListBox owner = new();
        IList collection = new ListBox.ObjectCollection(owner);
        collection.Add(2);
        collection.Add(1);
        collection.Add(3);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;
        char *textBuffer = stackalloc char[256];

        // Set first.
        collection[0] = 4;
        Assert.Equal(new int[] { 4, 1, 3 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(3, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("4", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));

        // Set middle.
        collection[1] = 2;
        Assert.Equal(new int[] { 4, 2, 3 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(3, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("4", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));

        // Set last.
        collection[2] = 5;
        Assert.Equal(new int[] { 4, 2, 5 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(3, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("4", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("5", new string(textBuffer));

        // Set same.
        collection[2] = 5;
        Assert.Equal(new int[] { 4, 2, 5 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(3, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("4", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("5", new string(textBuffer));
    }

    [WinFormsFact]
    public unsafe void ListBoxObjectCollection_IListItem_SetSortedWithHandle_GetReturnsExpected()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        IList collection = new ListBox.ObjectCollection(owner);
        collection.Add(2);
        collection.Add(1);
        collection.Add(3);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;
        char *textBuffer = stackalloc char[256];

        // Set first.
        collection[0] = 4;
        Assert.Equal(new int[] { 4, 2, 3 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(3, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("4", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));

        // Set middle.
        collection[1] = 1;
        Assert.Equal(new int[] { 4, 1, 3 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(3, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("4", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));

        // Set last.
        collection[2] = 5;
        Assert.Equal(new int[] { 4, 1, 5 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(3, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("4", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("5", new string(textBuffer));

        // Set same.
        collection[2] = 5;
        Assert.Equal(new int[] { 4, 1, 5 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(3, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("4", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("5", new string(textBuffer));
    }
#endif

    [WinFormsFact]
    public unsafe void ListBoxObjectCollection_IListItem_SetItemsWithHandle_GetReturnsExpected()
    {
        using ListBox owner = new();
        IList collection = owner.Items;
        collection.Add(2);
        collection.Add(1);
        collection.Add(3);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;
        char* textBuffer = stackalloc char[256];

        // Set first.
        collection[0] = 4;
        Assert.Equal(new int[] { 4, 1, 3 }, collection.Cast<int>());
        Assert.Equal(new int[] { 4, 1, 3 }, owner.Items.Cast<int>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(3, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("4", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));

        // Set middle.
        collection[1] = 2;
        Assert.Equal(new int[] { 4, 2, 3 }, collection.Cast<int>());
        Assert.Equal(new int[] { 4, 2, 3 }, owner.Items.Cast<int>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(3, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("4", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));

        // Set last.
        collection[2] = 5;
        Assert.Equal(new int[] { 4, 2, 5 }, collection.Cast<int>());
        Assert.Equal(new int[] { 4, 2, 5 }, owner.Items.Cast<int>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(3, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("4", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("5", new string(textBuffer));

        // Set same.
        collection[2] = 5;
        Assert.Equal(new int[] { 4, 2, 5 }, collection.Cast<int>());
        Assert.Equal(new int[] { 4, 2, 5 }, owner.Items.Cast<int>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(3, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("4", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("5", new string(textBuffer));
    }

    [WinFormsFact]
    public unsafe void ListBoxObjectCollection_IListItem_SetItemsSortedWithHandle_GetReturnsExpected()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        IList collection = owner.Items;
        collection.Add(2);
        collection.Add(1);
        collection.Add(3);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;
        char* textBuffer = stackalloc char[256];

        // Set first.
        collection[0] = 4;
        Assert.Equal(new int[] { 4, 2, 3 }, collection.Cast<int>());
        Assert.Equal(new int[] { 4, 2, 3 }, owner.Items.Cast<int>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(3, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("4", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));

        // Set middle.
        collection[1] = 1;
        Assert.Equal(new int[] { 4, 1, 3 }, collection.Cast<int>());
        Assert.Equal(new int[] { 4, 1, 3 }, owner.Items.Cast<int>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(3, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("4", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));

        // Set last.
        collection[2] = 5;
        Assert.Equal(new int[] { 4, 1, 5 }, collection.Cast<int>());
        Assert.Equal(new int[] { 4, 1, 5 }, owner.Items.Cast<int>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(3, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("4", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("5", new string(textBuffer));

        // Set same.
        collection[2] = 5;
        Assert.Equal(new int[] { 4, 1, 5 }, collection.Cast<int>());
        Assert.Equal(new int[] { 4, 1, 5 }, owner.Items.Cast<int>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(3, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("4", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("5", new string(textBuffer));
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListItem_SetWithOneSelectedItemNotSorted_Success()
    {
        using ListBox owner = new();
        IList collection = owner.Items;
        collection.Add(0);
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Set before.
        collection[0] = -1;
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Set after.
        collection[3] = 5;
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Set selected.
        collection[1] = 2;
        Assert.Equal(new object[] { 2 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Set same.
        collection[1] = 2;
        Assert.Equal(new object[] { 2 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListItem_SetWithOneSelectedItemSorted_Success()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        IList collection = owner.Items;
        collection.Add(0);
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Set before.
        collection[0] = -1;
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Set after.
        collection[3] = 5;
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Set selected.
        collection[1] = 2;
        Assert.Equal(new object[] { 2 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Set same.
        collection[1] = 2;
        Assert.Equal(new object[] { 2 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(SelectionMode.MultiExtended)]
    [InlineData(SelectionMode.MultiSimple)]
    public void ListBoxObjectCollection_IListItem_SetWithMultipleSelectedItemsNotSorted_Success(SelectionMode selectionMode)
    {
        using ListBox owner = new()
        {
            SelectionMode = selectionMode
        };
        IList collection = owner.Items;
        collection.Add(0);
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        collection.Add(5);
        owner.SelectedItems.Add(1);
        owner.SelectedItems.Add(3);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Set before.
        collection[0] = -1;
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 2 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Set after.
        collection[3] = 5;
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 2 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Set first selection.
        collection[1] = 2;
        Assert.Equal(new object[] { 2, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 2 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Set first selection same.
        collection[1] = 2;
        Assert.Equal(new object[] { 2, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 2 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Set last selection.
        collection[2] = 4;
        Assert.Equal(new object[] { 2, 4 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 2 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Set last selection same.
        collection[2] = 4;
        Assert.Equal(new object[] { 2, 4 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 2 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(SelectionMode.MultiExtended)]
    [InlineData(SelectionMode.MultiSimple)]
    public void ListBoxObjectCollection_IListItem_SetWithMultipleSelectedItemsSorted_Success(SelectionMode selectionMode)
    {
        using ListBox owner = new()
        {
            SelectionMode = selectionMode,
            Sorted = true
        };
        IList collection = owner.Items;
        collection.Add(0);
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        collection.Add(5);
        owner.SelectedItems.Add(1);
        owner.SelectedItems.Add(3);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Set before.
        collection[0] = -1;
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 2 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Set after.
        collection[3] = 5;
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 2 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Set first selection.
        collection[1] = 2;
        Assert.Equal(new object[] { 2, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 2 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Set first selection same.
        collection[1] = 2;
        Assert.Equal(new object[] { 2, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 2 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Set last selection.
        collection[2] = 4;
        Assert.Equal(new object[] { 2, 4 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 2 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Set last selection same.
        collection[2] = 4;
        Assert.Equal(new object[] { 2, 4 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 2 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListItem_SetWithOneSelectedItemNotSortedWithHandle_Success()
    {
        using ListBox owner = new();
        IList collection = owner.Items;
        collection.Add(0);
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) =>
        {
            Assert.Same(owner, sender);
            Assert.Same(EventArgs.Empty, e);
            selectedIndexChangedCallCount++;
        };

        // Set before.
        collection[0] = -1;
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set after.
        collection[3] = 5;
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set selected.
        collection[1] = 2;
        Assert.Equal(new object[] { 2 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(2, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set selected same.
        collection[1] = 2;
        Assert.Equal(new object[] { 2 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(3, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListItem_SetWithOneSelectedItemSortedWithHandle_Success()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        IList collection = owner.Items;
        collection.Add(0);
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) =>
        {
            Assert.Same(owner, sender);
            Assert.Same(EventArgs.Empty, e);
            selectedIndexChangedCallCount++;
        };

        // Set before.
        collection[0] = -1;
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set after.
        collection[3] = 5;
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set selected.
        collection[1] = 2;
        Assert.Equal(new object[] { 2 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(2, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        collection[1] = 2;
        Assert.Equal(new object[] { 2 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(3, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(SelectionMode.MultiExtended)]
    [InlineData(SelectionMode.MultiSimple)]
    public void ListBoxObjectCollection_IListItem_SetWithMultipleSelectedItemsNotSortedWithHandle_Success(SelectionMode selectionMode)
    {
        using ListBox owner = new()
        {
            SelectionMode = selectionMode
        };
        IList collection = owner.Items;
        collection.Add(0);
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        collection.Add(5);
        owner.SelectedItems.Add(1);
        owner.SelectedItems.Add(3);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) =>
        {
            Assert.Same(owner, sender);
            Assert.Same(EventArgs.Empty, e);
            selectedIndexChangedCallCount++;
        };

        // Set before.
        collection[0] = -1;
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 2 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set after.
        collection[3] = 5;
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 2 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set first selection.
        collection[1] = 2;
        Assert.Equal(new object[] { 2, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 2 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(2, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set first selection same.
        collection[1] = 2;
        Assert.Equal(new object[] { 2, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 2 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(3, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set last selection.
        collection[2] = 4;
        Assert.Equal(new object[] { 2 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(4, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set last selection same.
        collection[2] = 4;
        Assert.Equal(new object[] { 2 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(4, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(SelectionMode.MultiExtended)]
    [InlineData(SelectionMode.MultiSimple)]
    public void ListBoxObjectCollection_IListItem_SetWithMultipleSelectedItemsSortedWithHandle_Success(SelectionMode selectionMode)
    {
        using ListBox owner = new()
        {
            SelectionMode = selectionMode,
            Sorted = true
        };
        IList collection = owner.Items;
        collection.Add(0);
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        collection.Add(5);
        owner.SelectedItems.Add(1);
        owner.SelectedItems.Add(3);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) =>
        {
            Assert.Same(owner, sender);
            Assert.Same(EventArgs.Empty, e);
            selectedIndexChangedCallCount++;
        };

        // Set before.
        collection[0] = -1;
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 2 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set after.
        collection[3] = 5;
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 2 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set first selection.
        collection[1] = 2;
        Assert.Equal(new object[] { 2, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 2 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(2, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set first selection same.
        collection[1] = 2;
        Assert.Equal(new object[] { 2, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 2 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(3, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set last selection.
        collection[2] = 4;
        Assert.Equal(new object[] { 2 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(4, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set last selection same.
        collection[2] = 4;
        Assert.Equal(new object[] { 2 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(4, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void ListBoxObjectCollection_IListItem_SetNullValueEmpty_ThrowsArgumentNullException(int index)
    {
        using ListBox owner = new();
        IList collection = new ListBox.ObjectCollection(owner);
        Assert.Throws<ArgumentNullException>("value", () => collection[index] = null);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void ListBoxObjectCollection_IListItem_SetNullValueNotEmpty_ThrowsArgumentNullException(int index)
    {
        using ListBox owner = new();
        IList collection = new ListBox.ObjectCollection(owner)
        {
            1
        };
        Assert.Throws<ArgumentNullException>("value", () => collection[index] = null);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void ListBoxObjectCollection_IListItem_SetInvalidIndexEmpty_ThrowsArgumentOutOfRangeException(int index)
    {
        using ListBox owner = new();
        IList collection = new ListBox.ObjectCollection(owner);
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index] = 2);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(1)]
    [InlineData(2)]
    public void ListBoxObjectCollection_IListItem_SetInvalidIndexNotEmpty_ThrowsArgumentOutOfRangeException(int index)
    {
        using ListBox owner = new();
        IList collection = new ListBox.ObjectCollection(owner)
        {
            1
        };
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index] = 2);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListAdd_NotSorted_Success()
    {
        using ListBox owner = new();
        IList collection = new ListBox.ObjectCollection(owner);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Add one.
        Assert.Equal(0, collection.Add(2));
        Assert.Single(collection);
        Assert.Equal(new object[] { 2 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Add another.
        Assert.Equal(1, collection.Add(1));
        Assert.Equal(2, collection.Count);
        Assert.Equal(new object[] { 2, 1 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Add same.
        Assert.Equal(2, collection.Add(1));
        Assert.Equal(3, collection.Count);
        Assert.Equal(new object[] { 2, 1, 1 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Add another.
        Assert.Equal(3, collection.Add(3));
        Assert.Equal(4, collection.Count);
        Assert.Equal(new object[] { 2, 1, 1, 3 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListAdd_ItemsNotSorted_Success()
    {
        using ListBox owner = new();
        IList collection = owner.Items;
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Add one.
        Assert.Equal(0, collection.Add(2));
        Assert.Single(collection);
        Assert.Equal(new object[] { 2 }, collection.Cast<object>());
        Assert.Equal(new object[] { 2 }, owner.Items.Cast<object>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Add another.
        Assert.Equal(1, collection.Add(1));
        Assert.Equal(2, collection.Count);
        Assert.Equal(new object[] { 2, 1 }, collection.Cast<object>());
        Assert.Equal(new object[] { 2, 1 }, owner.Items.Cast<object>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Add same.
        Assert.Equal(2, collection.Add(1));
        Assert.Equal(3, collection.Count);
        Assert.Equal(new object[] { 2, 1, 1 }, collection.Cast<object>());
        Assert.Equal(new object[] { 2, 1, 1 }, owner.Items.Cast<object>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Add another.
        Assert.Equal(3, collection.Add(3));
        Assert.Equal(4, collection.Count);
        Assert.Equal(new object[] { 2, 1, 1, 3 }, collection.Cast<object>());
        Assert.Equal(new object[] { 2, 1, 1, 3 }, owner.Items.Cast<object>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListAdd_Sorted_Success()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        IList collection = new ListBox.ObjectCollection(owner);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Add one.
        Assert.Equal(0, collection.Add(2));
        Assert.Single(collection);
        Assert.Equal(new object[] { 2 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Add another.
        Assert.Equal(0, collection.Add(1));
        Assert.Equal(2, collection.Count);
        Assert.Equal(new object[] { 1, 2 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Add same.
        Assert.Equal(0, collection.Add(1));
        Assert.Equal(3, collection.Count);
        Assert.Equal(new object[] { 1, 1, 2 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Add another.
        Assert.Equal(3, collection.Add(3));
        Assert.Equal(4, collection.Count);
        Assert.Equal(new object[] { 1, 1, 2, 3 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListAdd_ItemsSorted_Success()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        IList collection = owner.Items;
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Add one.
        Assert.Equal(0, collection.Add(2));
        Assert.Single(collection);
        Assert.Equal(new object[] { 2 }, collection.Cast<object>());
        Assert.Equal(new object[] { 2 }, owner.Items.Cast<object>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Add another.
        Assert.Equal(0, collection.Add(1));
        Assert.Equal(2, collection.Count);
        Assert.Equal(new object[] { 1, 2 }, collection.Cast<object>());
        Assert.Equal(new object[] { 1, 2 }, owner.Items.Cast<object>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Add same.
        Assert.Equal(0, collection.Add(1));
        Assert.Equal(3, collection.Count);
        Assert.Equal(new object[] { 1, 1, 2 }, collection.Cast<object>());
        Assert.Equal(new object[] { 1, 1, 2 }, owner.Items.Cast<object>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Add another.
        Assert.Equal(3, collection.Add(3));
        Assert.Equal(4, collection.Count);
        Assert.Equal(new object[] { 1, 1, 2, 3 }, collection.Cast<object>());
        Assert.Equal(new object[] { 1, 1, 2, 3 }, owner.Items.Cast<object>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public unsafe void ListBoxObjectCollection_IListAdd_NotSortedWithHandle_Success()
    {
        using ListBox owner = new();
        IList collection = new ListBox.ObjectCollection(owner);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        char* textBuffer = stackalloc char[256];

        // Add one.
        Assert.Equal(0, collection.Add(2));
        Assert.Single(collection);
        Assert.Equal(new object[] { 2 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(1, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));

        // Add another.
        Assert.Equal(1, collection.Add(1));
        Assert.Equal(2, collection.Count);
        Assert.Equal(new object[] { 2, 1 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(2, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));

        // Add same.
        Assert.Equal(2, collection.Add(1));
        Assert.Equal(3, collection.Count);
        Assert.Equal(new object[] { 2, 1, 1 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(3, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));

        // Add another.
        Assert.Equal(3, collection.Add(3));
        Assert.Equal(4, collection.Count);
        Assert.Equal(new object[] { 2, 1, 1, 3 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(4, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 3, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));
    }

    [WinFormsFact]
    public unsafe void ListBoxObjectCollection_IListAdd_ItemsNotSortedWithHandle_Success()
    {
        using ListBox owner = new();
        IList collection = owner.Items;
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        char* textBuffer = stackalloc char[256];

        // Add one.
        Assert.Equal(0, collection.Add(2));
        Assert.Single(collection);
        Assert.Equal(new object[] { 2 }, collection.Cast<object>());
        Assert.Equal(new object[] { 2 }, owner.Items.Cast<object>());
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(1, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));

        // Add another.
        Assert.Equal(1, collection.Add(1));
        Assert.Equal(2, collection.Count);
        Assert.Equal(new object[] { 2, 1 }, collection.Cast<object>());
        Assert.Equal(new object[] { 2, 1 }, owner.Items.Cast<object>());
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(2, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));

        // Add same.
        Assert.Equal(2, collection.Add(1));
        Assert.Equal(3, collection.Count);
        Assert.Equal(new object[] { 2, 1, 1 }, collection.Cast<object>());
        Assert.Equal(new object[] { 2, 1, 1 }, owner.Items.Cast<object>());
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(3, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));

        // Add another.
        Assert.Equal(3, collection.Add(3));
        Assert.Equal(4, collection.Count);
        Assert.Equal(new object[] { 2, 1, 1, 3 }, collection.Cast<object>());
        Assert.Equal(new object[] { 2, 1, 1, 3 }, owner.Items.Cast<object>());
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(4, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 3, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));
    }

    [WinFormsFact]
    public unsafe void ListBoxObjectCollection_IListAdd_SortedWithHandle_Success()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        IList collection = new ListBox.ObjectCollection(owner);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        char* textBuffer = stackalloc char[256];

        // Add one.
        Assert.Equal(0, collection.Add(2));
        Assert.Single(collection);
        Assert.Equal(new object[] { 2 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(1, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));

        // Add another.
        Assert.Equal(0, collection.Add(1));
        Assert.Equal(2, collection.Count);
        Assert.Equal(new object[] { 1, 2 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(2, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));

        // Add same.
        Assert.Equal(0, collection.Add(1));
        Assert.Equal(3, collection.Count);
        Assert.Equal(new object[] { 1, 1, 2 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(3, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));

        // Add another.
        Assert.Equal(3, collection.Add(3));
        Assert.Equal(4, collection.Count);
        Assert.Equal(new object[] { 1, 1, 2, 3 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(4, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 3, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));
    }

    [WinFormsFact]
    public unsafe void ListBoxObjectCollection_IListAdd_ItemsSortedWithHandle_Success()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        IList collection = owner.Items;
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        char* textBuffer = stackalloc char[256];

        // Add one.
        Assert.Equal(0, collection.Add(2));
        Assert.Single(collection);
        Assert.Equal(new object[] { 2 }, collection.Cast<object>());
        Assert.Equal(new object[] { 2 }, owner.Items.Cast<object>());
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(1, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));

        // Add another.
        Assert.Equal(0, collection.Add(1));
        Assert.Equal(2, collection.Count);
        Assert.Equal(new object[] { 1, 2 }, collection.Cast<object>());
        Assert.Equal(new object[] { 1, 2 }, owner.Items.Cast<object>());
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(2, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));

        // Add same.
        Assert.Equal(0, collection.Add(1));
        Assert.Equal(3, collection.Count);
        Assert.Equal(new object[] { 1, 1, 2 }, collection.Cast<object>());
        Assert.Equal(new object[] { 1, 1, 2 }, owner.Items.Cast<object>());
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(3, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));

        // Add another.
        Assert.Equal(3, collection.Add(3));
        Assert.Equal(4, collection.Count);
        Assert.Equal(new object[] { 1, 1, 2, 3 }, collection.Cast<object>());
        Assert.Equal(new object[] { 1, 1, 2, 3 }, owner.Items.Cast<object>());
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(4, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 3, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListAdd_WithOneSelectedItemNotSorted_Success()
    {
        using ListBox owner = new();
        IList collection = owner.Items;
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Add before.
        collection.Add(0);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Add after.
        collection.Add(5);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListAdd_WithOneSelectedItemSorted_Success()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        IList collection = owner.Items;
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Add before.
        collection.Add(0);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Add after.
        collection.Add(5);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(SelectionMode.MultiExtended)]
    [InlineData(SelectionMode.MultiSimple)]
    public void ListBoxObjectCollection_IListAdd_WithMultipleSelectedItemsNotSorted_Success(SelectionMode selectionMode)
    {
        using ListBox owner = new()
        {
            SelectionMode = selectionMode
        };
        IList collection = owner.Items;
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        collection.Add(5);
        owner.SelectedItems.Add(1);
        owner.SelectedItems.Add(3);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Add before.
        collection.Add(0);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0, 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Add middle.
        collection.Add(2);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0, 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Add after.
        collection.Add(4);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0, 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(SelectionMode.MultiExtended)]
    [InlineData(SelectionMode.MultiSimple)]
    public void ListBoxObjectCollection_IListAdd_WithMultipleSelectedItemsSorted_Success(SelectionMode selectionMode)
    {
        using ListBox owner = new()
        {
            SelectionMode = selectionMode,
            Sorted = true
        };
        IList collection = owner.Items;
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);
        owner.SelectedItems.Add(3);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Add before.
        collection.Add(0);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 2 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.False(owner.IsHandleCreated);

        // Add middle.
        collection.Add(2);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 3 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.False(owner.IsHandleCreated);

        // Add after.
        collection.Add(5);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 3 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListAdd_WithOneSelectedItemNotSortedWithHandle_Succes()
    {
        using ListBox owner = new();
        IList collection = owner.Items;
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;

        // Add before.
        collection.Add(0);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Add after.
        collection.Add(5);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListAdd_WithOneSelectedItemSortedWithHandle_Succes()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        IList collection = owner.Items;
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;

        // Add before.
        collection.Add(0);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Add after.
        collection.Add(5);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(SelectionMode.MultiExtended)]
    [InlineData(SelectionMode.MultiSimple)]
    public void ListBoxObjectCollection_IListAdd_WithMultipleSelectedItemsNotSortedWithHandle_Succes(SelectionMode selectionMode)
    {
        using ListBox owner = new()
        {
            SelectionMode = selectionMode
        };
        IList collection = owner.Items;
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        collection.Add(5);
        owner.SelectedItems.Add(1);
        owner.SelectedItems.Add(3);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;

        // Add before.
        collection.Add(0);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0, 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Add middle.
        collection.Add(2);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0, 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Add after.
        collection.Add(4);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0, 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(SelectionMode.MultiExtended)]
    [InlineData(SelectionMode.MultiSimple)]
    public void ListBoxObjectCollection_IListAdd_WithMultipleSelectedItemsSortedWithHandle_Succes(SelectionMode selectionMode)
    {
        using ListBox owner = new()
        {
            SelectionMode = selectionMode,
            Sorted = true
        };
        IList collection = owner.Items;
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);
        owner.SelectedItems.Add(3);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;

        // Add before.
        collection.Add(0);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 2 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Add middle.
        collection.Add(2);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 3 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Add after.
        collection.Add(5);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 3 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public unsafe void ListBoxObjectCollection_IListAdd_CreateHandle_Success()
    {
        using ListBox owner = new();
        IList collection = new ListBox.ObjectCollection(owner)
        {
            2,
            1,
            1,
            3
        };
        char* textBuffer = stackalloc char[256];

        Assert.Equal(0, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
    }

    [WinFormsFact]
    public unsafe void ListBoxObjectCollection_IListAdd_ItemsCreateHandle_Success()
    {
        using ListBox owner = new();
        IList collection = owner.Items;
        collection.Add(2);
        collection.Add(1);
        collection.Add(1);
        collection.Add(3);
        char* textBuffer = stackalloc char[256];

        Assert.Equal(4, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 3, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListAdd_NullItem_ThrowsArgumentNullException()
    {
        using ListBox owner = new();
        IList collection = new ListBox.ObjectCollection(owner);
        Assert.Throws<ArgumentNullException>("item", () => collection.Add(null));
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListAdd_OwnerHasDataSource_ThrowsArgumentException()
    {
        using ListBox owner = new()
        {
            DataSource = Array.Empty<object>()
        };
        IList collection = new ListBox.ObjectCollection(owner);
        Assert.Throws<ArgumentException>(() => collection.Add(1));
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(-2)]
    public void ListBoxObjectCollection_IListAdd_ErrorAdding_ThrowsOutOfMemoryException(int result)
    {
        // Note that this is not an actual out of memory, we're artificially setting up an error case
        // that we surface as `OutOfMemoryException` (see ListBox.NativeAdd(Object item))

        using CustomAddStringListBox owner = new()
        {
            AddStringResult = result
        };
        IList collection = new ListBox.ObjectCollection(owner);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        Assert.Throws<OutOfMemoryException>(() => collection.Add(1));
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListClear_InvokeEmpty_Success()
    {
        using ListBox owner = new();
        IList collection = new ListBox.ObjectCollection(owner);
        collection.Clear();
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.False(owner.IsHandleCreated);

        // Call again.
        collection.Clear();
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListClear_InvokeItemsEmpty_Success()
    {
        using ListBox owner = new();
        IList collection = owner.Items;
        collection.Clear();
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.False(owner.IsHandleCreated);

        // Call again.
        collection.Clear();
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListClear_InvokeNotEmpty_Success()
    {
        using ListBox owner = new();
        IList collection = new ListBox.ObjectCollection(owner)
        {
            1
        };

        collection.Clear();
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.False(owner.IsHandleCreated);

        // Call again.
        collection.Clear();
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListClear_InvokeItemsNotEmpty_Success()
    {
        using ListBox owner = new();
        IList collection = owner.Items;
        collection.Add(1);

        collection.Clear();
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.False(owner.IsHandleCreated);

        // Call again.
        collection.Clear();
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListClear_InvokeEmptyWithHandle_Success()
    {
        using ListBox owner = new();
        IList collection = new ListBox.ObjectCollection(owner);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;

        collection.Clear();
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(0, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));

        // Call again.
        collection.Clear();
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(0, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListClear_InvokeItemsEmptyWithHandle_Success()
    {
        using ListBox owner = new();
        IList collection = owner.Items;
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;

        collection.Clear();
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(0, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));

        // Call again.
        collection.Clear();
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(0, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListClear_InvokeNotEmptyWithHandle_Success()
    {
        using ListBox owner = new();
        IList collection = new ListBox.ObjectCollection(owner);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        collection.Add(1);

        collection.Clear();
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(0, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));

        // Call again.
        collection.Clear();
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(0, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListClear_InvokeItemsNotEmptyWithHandle_Success()
    {
        using ListBox owner = new();
        IList collection = owner.Items;
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        collection.Add(1);

        collection.Clear();
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(0, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));

        // Call again.
        collection.Clear();
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(0, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListContains_InvokeEmpty_ReturnsExpected()
    {
        using ListBox owner = new();
        IList collection = new ListBox.ObjectCollection(owner);
        Assert.False(collection.Contains(0));
        Assert.False(collection.Contains(1));
        Assert.False(collection.Contains(new object()));
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListContains_InvokeWithValues_ReturnsExpected()
    {
        using ListBox owner = new();
        IList collection = new ListBox.ObjectCollection(owner)
        {
            // Add one.
            1
        };
        Assert.False(collection.Contains(0));
        Assert.True(collection.Contains(1));
        Assert.False(collection.Contains(2));
        Assert.False(collection.Contains(new object()));
        Assert.False(owner.IsHandleCreated);

        // Add another.
        collection.Add(0);
        Assert.True(collection.Contains(0));
        Assert.True(collection.Contains(1));
        Assert.False(collection.Contains(2));
        Assert.False(collection.Contains(new object()));
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListContains_NullValue_ThrowsArgumentNullException()
    {
        using ListBox owner = new();
        IList collection = new ListBox.ObjectCollection(owner);
        Assert.Throws<ArgumentNullException>("value", () => collection.Contains(null));
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListCopyTo_InvokeEmpty_Success()
    {
        using ListBox owner = new();
        IList collection = new ListBox.ObjectCollection(owner);
        object[] array = ["1", "2", "3"];
        collection.CopyTo(array, 1);
        Assert.Equal(["1", "2", "3"], array);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListCopyTo_InvokeNotEmpty_ReturnsExpected()
    {
        using ListBox owner = new();
        IList collection = new ListBox.ObjectCollection(owner)
        {
            1,
            2
        };
        object[] array = ["1", "2", "3"];
        collection.CopyTo(array, 1);
        Assert.Equal(["1", 1, 2], array);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListCopyTo_NullArrayEmpty_ThrowsArgumentNullException()
    {
        using ListBox owner = new();
        IList collection = new ListBox.ObjectCollection(owner);
        Assert.Throws<ArgumentNullException>("destination", () => collection.CopyTo(null, 0));
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListCopyTo_NullArrayNotEmpty_ThrowsArgumentNullException()
    {
        using ListBox owner = new();
        IList collection = new ListBox.ObjectCollection(owner)
        {
            1
        };
        Assert.Throws<ArgumentNullException>("destination", () => collection.CopyTo(null, 0));
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListGetEnumerator_InvokeEmpty_ReturnsExpected()
    {
        using ListBox owner = new();
        IList collection = new ListBox.ObjectCollection(owner);

        IEnumerator enumerator = collection.GetEnumerator();
        for (int i = 0; i < 2; i++)
        {
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);

            Assert.False(enumerator.MoveNext());
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);

            // Move again.
            Assert.False(enumerator.MoveNext());
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);

            // Reset.
            enumerator.Reset();
        }
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListGetEnumerator_InvokeNotEmpty_ReturnsExpected()
    {
        using ListBox owner = new();
        IList collection = new ListBox.ObjectCollection(owner)
        {
            2
        };

        IEnumerator enumerator = collection.GetEnumerator();
        for (int i = 0; i < 2; i++)
        {
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);

            Assert.True(enumerator.MoveNext());
            Assert.Equal(2, enumerator.Current);

            Assert.False(enumerator.MoveNext());
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);

            // Move again.
            Assert.False(enumerator.MoveNext());
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);

            // Reset.
            enumerator.Reset();
        }
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListIndexOf_InvokeEmpty_ReturnsExpected()
    {
        using ListBox owner = new();
        IList collection = new ListBox.ObjectCollection(owner);
        Assert.Equal(-1, collection.IndexOf(0));
        Assert.Equal(-1, collection.IndexOf(1));
        Assert.Equal(-1, collection.IndexOf(new object()));
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListIndexOf_InvokeWithValues_ReturnsExpected()
    {
        using ListBox owner = new();
        IList collection = new ListBox.ObjectCollection(owner)
        {
            // Add one.
            1
        };
        Assert.Equal(-1, collection.IndexOf(0));
        Assert.Equal(0, collection.IndexOf(1));
        Assert.Equal(-1, collection.IndexOf(2));
        Assert.Equal(-1, collection.IndexOf(new object()));
        Assert.False(owner.IsHandleCreated);

        // Add another.
        collection.Add(0);
        Assert.Equal(1, collection.IndexOf(0));
        Assert.Equal(0, collection.IndexOf(1));
        Assert.Equal(-1, collection.IndexOf(2));
        Assert.Equal(-1, collection.IndexOf(new object()));
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListIndexOf_NullValue_ThrowsArgumentNullException()
    {
        using ListBox owner = new();
        IList collection = new ListBox.ObjectCollection(owner);
        Assert.Throws<ArgumentNullException>("value", () => collection.IndexOf(null));
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListInsert_NotSorted_Success()
    {
        using ListBox owner = new();
        IList collection = new ListBox.ObjectCollection(owner);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Insert first.
        collection.Insert(0, 1);
        Assert.Single(collection);
        Assert.Equal(new object[] { 1 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Insert end.
        collection.Insert(0, 2);
        Assert.Equal(2, collection.Count);
        Assert.Equal(new object[] { 2, 1 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Insert same.
        collection.Insert(1, 2);
        Assert.Equal(3, collection.Count);
        Assert.Equal(new object[] { 2, 2, 1 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Insert end.
        collection.Insert(3, 3);
        Assert.Equal(4, collection.Count);
        Assert.Equal(new object[] { 2, 2, 1, 3 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListInsert_ItemsNotSorted_Success()
    {
        using ListBox owner = new();
        IList collection = owner.Items;
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Insert first.
        collection.Insert(0, 1);
        Assert.Single(collection);
        Assert.Equal(new object[] { 1 }, collection.Cast<object>());
        Assert.Equal(new object[] { 1 }, owner.Items.Cast<object>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Insert end.
        collection.Insert(0, 2);
        Assert.Equal(2, collection.Count);
        Assert.Equal(new object[] { 2, 1 }, collection.Cast<object>());
        Assert.Equal(new object[] { 2, 1 }, owner.Items.Cast<object>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Insert same.
        collection.Insert(0, 2);
        Assert.Equal(3, collection.Count);
        Assert.Equal(new object[] { 2, 2, 1 }, collection.Cast<object>());
        Assert.Equal(new object[] { 2, 2, 1 }, owner.Items.Cast<object>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Insert end.
        collection.Insert(3, 3);
        Assert.Equal(4, collection.Count);
        Assert.Equal(new object[] { 2, 2, 1, 3 }, collection.Cast<object>());
        Assert.Equal(new object[] { 2, 2, 1, 3 }, owner.Items.Cast<object>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListInsert_Sorted_Success()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        IList collection = new ListBox.ObjectCollection(owner);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Insert first.
        collection.Insert(0, 1);
        Assert.Single(collection);
        Assert.Equal(new object[] { 1 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Insert end.
        collection.Insert(0, 2);
        Assert.Equal(2, collection.Count);
        Assert.Equal(new object[] { 1, 2 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Insert same.
        collection.Insert(0, 2);
        Assert.Equal(3, collection.Count);
        Assert.Equal(new object[] { 1, 2, 2 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Insert end.
        collection.Insert(3, 3);
        Assert.Equal(4, collection.Count);
        Assert.Equal(new object[] { 1, 2, 2, 3 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListInsert_ItemsSorted_Success()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        IList collection = owner.Items;
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Insert first.
        collection.Insert(0, 1);
        Assert.Single(collection);
        Assert.Equal(new object[] { 1 }, collection.Cast<object>());
        Assert.Equal(new object[] { 1 }, owner.Items.Cast<object>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Insert end.
        collection.Insert(0, 2);
        Assert.Equal(2, collection.Count);
        Assert.Equal(new object[] { 1, 2 }, collection.Cast<object>());
        Assert.Equal(new object[] { 1, 2 }, owner.Items.Cast<object>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Insert same.
        collection.Insert(0, 2);
        Assert.Equal(3, collection.Count);
        Assert.Equal(new object[] { 1, 2, 2 }, collection.Cast<object>());
        Assert.Equal(new object[] { 1, 2, 2 }, owner.Items.Cast<object>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Insert end.
        collection.Insert(3, 3);
        Assert.Equal(4, collection.Count);
        Assert.Equal(new object[] { 1, 2, 2, 3 }, collection.Cast<object>());
        Assert.Equal(new object[] { 1, 2, 2, 3 }, owner.Items.Cast<object>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public unsafe void ListBoxObjectCollection_IListInsert_NotSortedWithHandle_Success()
    {
        using ListBox owner = new();
        IList collection = new ListBox.ObjectCollection(owner);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        char* textBuffer = stackalloc char[256];

        // Insert first.
        collection.Insert(0, 1);
        Assert.Single(collection);
        Assert.Equal(new object[] { 1 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(1, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));

        // Insert end.
        collection.Insert(0, 2);
        Assert.Equal(2, collection.Count);
        Assert.Equal(new object[] { 2, 1 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(2, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));

        // Insert same.
        collection.Insert(0, 2);
        Assert.Equal(3, collection.Count);
        Assert.Equal(new object[] { 2, 2, 1 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(3, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));

        // Insert end.
        collection.Insert(3, 3);
        Assert.Equal(4, collection.Count);
        Assert.Equal(new object[] { 2, 2, 1, 3 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(4, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 3, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));
    }

    [WinFormsFact]
    public unsafe void ListBoxObjectCollection_IListInsert_ItemsNotSortedWithHandle_Success()
    {
        using ListBox owner = new();
        IList collection = owner.Items;
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        char* textBuffer = stackalloc char[256];

        // Insert first.
        collection.Insert(0, 1);
        Assert.Single(collection);
        Assert.Equal(new object[] { 1 }, collection.Cast<object>());
        Assert.Equal(new object[] { 1 }, owner.Items.Cast<object>());
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(1, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));

        // Insert end.
        collection.Insert(0, 2);
        Assert.Equal(2, collection.Count);
        Assert.Equal(new object[] { 2, 1 }, collection.Cast<object>());
        Assert.Equal(new object[] { 2, 1 }, owner.Items.Cast<object>());
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(2, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));

        // Insert same.
        collection.Insert(0, 2);
        Assert.Equal(3, collection.Count);
        Assert.Equal(new object[] { 2, 2, 1 }, collection.Cast<object>());
        Assert.Equal(new object[] { 2, 2, 1 }, owner.Items.Cast<object>());
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(3, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));

        // Insert end.
        collection.Insert(3, 3);
        Assert.Equal(4, collection.Count);
        Assert.Equal(new object[] { 2, 2, 1, 3 }, collection.Cast<object>());
        Assert.Equal(new object[] { 2, 2, 1, 3 }, owner.Items.Cast<object>());
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(4, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 3, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));
    }

    [WinFormsFact]
    public unsafe void ListBoxObjectCollection_IListInsert_SortedWithHandle_Success()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        IList collection = new ListBox.ObjectCollection(owner);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        char* textBuffer = stackalloc char[256];

        // Insert first.
        collection.Insert(0, 1);
        Assert.Single(collection);
        Assert.Equal(new object[] { 1 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(1, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));

        // Insert end.
        collection.Insert(0, 2);
        Assert.Equal(2, collection.Count);
        Assert.Equal(new object[] { 1, 2 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(2, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));

        // Insert same.
        collection.Insert(0, 2);
        Assert.Equal(3, collection.Count);
        Assert.Equal(new object[] { 1, 2, 2 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(3, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));

        // Insert end.
        collection.Insert(3, 3);
        Assert.Equal(4, collection.Count);
        Assert.Equal(new object[] { 1, 2, 2, 3 }, collection.Cast<object>());
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(4, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 3, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));
    }

    [WinFormsFact]
    public unsafe void ListBoxObjectCollection_IListInsert_ItemsSortedWithHandle_Success()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        IList collection = owner.Items;
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        char* textBuffer = stackalloc char[256];

        // Insert first.
        collection.Insert(0, 1);
        Assert.Single(collection);
        Assert.Equal(new object[] { 1 }, collection.Cast<object>());
        Assert.Equal(new object[] { 1 }, owner.Items.Cast<object>());
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(1, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));

        // Insert end.
        collection.Insert(0, 2);
        Assert.Equal(2, collection.Count);
        Assert.Equal(new object[] { 1, 2 }, collection.Cast<object>());
        Assert.Equal(new object[] { 1, 2 }, owner.Items.Cast<object>());
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(2, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));

        // Insert same.
        collection.Insert(0, 2);
        Assert.Equal(3, collection.Count);
        Assert.Equal(new object[] { 1, 2, 2 }, collection.Cast<object>());
        Assert.Equal(new object[] { 1, 2, 2 }, owner.Items.Cast<object>());
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(3, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));

        // Insert end.
        collection.Insert(3, 3);
        Assert.Equal(4, collection.Count);
        Assert.Equal(new object[] { 1, 2, 2, 3 }, collection.Cast<object>());
        Assert.Equal(new object[] { 1, 2, 2, 3 }, owner.Items.Cast<object>());
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(4, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 3, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListInsert_WithOneSelectedItemNotSorted_Success()
    {
        using ListBox owner = new();
        IList collection = owner.Items;
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Insert before.
        collection.Insert(3, 0);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Insert after.
        collection.Insert(4, 5);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListInsert_WithOneSelectedItemSorted_Success()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        IList collection = owner.Items;
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);

        // Insert before.
        collection.Insert(3, 0);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1 }, owner.SelectedIndices.Cast<int>());
        Assert.False(owner.IsHandleCreated);

        // Insert after.
        collection.Insert(4, 5);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1 }, owner.SelectedIndices.Cast<int>());
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(SelectionMode.MultiExtended)]
    [InlineData(SelectionMode.MultiSimple)]
    public void ListBoxObjectCollection_IListInsert_WithMultipleSelectedItemsNotSorted_Success(SelectionMode selectionMode)
    {
        using ListBox owner = new()
        {
            SelectionMode = selectionMode
        };
        IList collection = owner.Items;
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        collection.Add(5);
        owner.SelectedItems.Add(1);
        owner.SelectedItems.Add(3);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Insert before.
        collection.Insert(3, 0);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0, 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Insert middle.
        collection.Insert(2, 2);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0, 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Insert after.
        collection.Insert(3, 4);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0, 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(SelectionMode.MultiExtended)]
    [InlineData(SelectionMode.MultiSimple)]
    public void ListBoxObjectCollection_IListInsert_WithMultipleSelectedItemsSorted_Success(SelectionMode selectionMode)
    {
        using ListBox owner = new()
        {
            SelectionMode = selectionMode,
            Sorted = true
        };
        IList collection = owner.Items;
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);
        owner.SelectedItems.Add(3);

        // Insert before.
        collection.Insert(3, 0);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 2 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.False(owner.IsHandleCreated);

        // Insert middle.
        collection.Insert(2, 2);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 3 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.False(owner.IsHandleCreated);

        // Insert after.
        collection.Insert(4, 5);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 3 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListInsert_WithOneSelectedItemNotSortedWithHandle_Succes()
    {
        using ListBox owner = new();
        IList collection = owner.Items;
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;

        // Insert before.
        collection.Insert(3, 0);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Insert after.
        collection.Insert(4, 5);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListInsert_WithOneSelectedItemSortedWithHandle_Succes()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        IList collection = owner.Items;
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;

        // Insert before.
        collection.Insert(3, 0);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Insert after.
        collection.Insert(4, 5);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(SelectionMode.MultiExtended)]
    [InlineData(SelectionMode.MultiSimple)]
    public void ListBoxObjectCollection_IListInsert_WithMultipleSelectedItemsNotSortedWithHandle_Succes(SelectionMode selectionMode)
    {
        using ListBox owner = new()
        {
            SelectionMode = selectionMode
        };
        IList collection = owner.Items;
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        collection.Add(5);
        owner.SelectedItems.Add(1);
        owner.SelectedItems.Add(3);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;

        // Insert before.
        collection.Insert(3, 0);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0, 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Insert middle.
        collection.Insert(2, 2);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0, 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Insert after.
        collection.Insert(3, 4);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0, 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(SelectionMode.MultiExtended)]
    [InlineData(SelectionMode.MultiSimple)]
    public void ListBoxObjectCollection_IListInsert_WithMultipleSelectedItemsSortedWithHandle_Succes(SelectionMode selectionMode)
    {
        using ListBox owner = new()
        {
            SelectionMode = selectionMode,
            Sorted = true
        };
        IList collection = owner.Items;
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);
        owner.SelectedItems.Add(3);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;

        // Insert before.
        collection.Insert(3, 0);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 2 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Insert middle.
        collection.Insert(2, 2);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 3 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Insert after.
        collection.Insert(4, 5);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 1, 3 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public unsafe void ListBoxObjectCollection_IListInsert_CreateHandle_Success()
    {
        using ListBox owner = new();
        IList collection = new ListBox.ObjectCollection(owner);
        collection.Insert(0, 2);
        collection.Insert(1, 1);
        collection.Insert(2, 1);
        collection.Insert(3, 3);
        char* textBuffer = stackalloc char[256];

        Assert.Equal(0, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
    }

    [WinFormsFact]
    public unsafe void ListBoxObjectCollection_IListInsert_ItemsCreateHandle_Success()
    {
        using ListBox owner = new();
        IList collection = owner.Items;
        collection.Insert(0, 2);
        collection.Insert(1, 1);
        collection.Insert(2, 1);
        collection.Insert(3, 3);
        char* textBuffer = stackalloc char[256];

        Assert.Equal(4, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 2, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 3, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(1)]
    [InlineData(2)]
    public void ListBoxObjectCollection_IListInsert_InvalidIndexEmpty_ThrowsArgumentOutOfRangeException(int index)
    {
        using ListBox owner = new();
        IList collection = new ListBox.ObjectCollection(owner);
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.Insert(index, 1));
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(2)]
    [InlineData(3)]
    public void ListBoxObjectCollection_IListInsert_InvalidIndexNotEmpty_ThrowsArgumentOutOfRangeException(int index)
    {
        using ListBox owner = new();
        IList collection = new ListBox.ObjectCollection(owner)
        {
            1
        };
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.Insert(index, 1));
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListInsert_NullItem_ThrowsArgumentNullException()
    {
        using ListBox owner = new();
        IList collection = new ListBox.ObjectCollection(owner);
        Assert.Throws<ArgumentNullException>("item", () => collection.Insert(0, null));
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListInsert_OwnerHasDataSource_ThrowsArgumentException()
    {
        using ListBox owner = new()
        {
            DataSource = Array.Empty<object>()
        };
        IList collection = new ListBox.ObjectCollection(owner);
        Assert.Throws<ArgumentException>(() => collection.Insert(0, 1));
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(-2)]
    public void ListBoxObjectCollection_IListInsert_ErrorAdding_ThrowsOutOfMemoryException(int result)
    {
        // Note that this is not an actual out of memory, we're artificially setting up an error case
        // that we surface as `OutOfMemoryException` (see ListBox.NativeAdd(Object item))

        using CustomInsertStringListBox owner = new()
        {
            InsertStringResult = result
        };
        IList collection = new ListBox.ObjectCollection(owner);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        Assert.Throws<OutOfMemoryException>(() => collection.Insert(0, 1));
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListRemove_NotSorted_Success()
    {
        using ListBox owner = new();
        IList collection = new ListBox.ObjectCollection(owner)
        {
            2,
            1,
            3
        };
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Remove middle.
        collection.Remove(1);
        Assert.Equal(2, collection.Count);
        Assert.Equal(new int[] { 2, 3 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Remove again.
        collection.Remove(1);
        Assert.Equal(2, collection.Count);
        Assert.Equal(new int[] { 2, 3 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Remove last.
        collection.Remove(3);
        Assert.Single(collection);
        Assert.Equal(new int[] { 2 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Remove first.
        collection.Remove(2);
        Assert.Empty(collection);
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListRemove_ItemsNotSorted_Success()
    {
        using ListBox owner = new();
        IList collection = owner.Items;
        collection.Add(2);
        collection.Add(1);
        collection.Add(3);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Remove middle.
        collection.Remove(1);
        Assert.Equal(2, collection.Count);
        Assert.Equal(new int[] { 2, 3 }, collection.Cast<int>());
        Assert.Equal(new int[] { 2, 3 }, owner.Items.Cast<int>());
        Assert.False(owner.IsHandleCreated);

        // Remove again.
        collection.Remove(1);
        Assert.Equal(2, collection.Count);
        Assert.Equal(new int[] { 2, 3 }, collection.Cast<int>());
        Assert.Equal(new int[] { 2, 3 }, owner.Items.Cast<int>());
        Assert.False(owner.IsHandleCreated);

        // Remove last.
        collection.Remove(3);
        Assert.Single(collection);
        Assert.Equal(new int[] { 2 }, collection.Cast<int>());
        Assert.Equal(new int[] { 2 }, owner.Items.Cast<int>());
        Assert.False(owner.IsHandleCreated);

        // Remove first.
        collection.Remove(2);
        Assert.Empty(collection);
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListRemove_Sorted_Success()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        IList collection = new ListBox.ObjectCollection(owner)
        {
            2,
            1,
            3
        };
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Remove middle.
        collection.Remove(2);
        Assert.Equal(2, collection.Count);
        Assert.Equal(new int[] { 1, 3 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Remove again.
        collection.Remove(2);
        Assert.Equal(2, collection.Count);
        Assert.Equal(new int[] { 1, 3 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Remove last.
        collection.Remove(3);
        Assert.Single(collection);
        Assert.Equal(new int[] { 1 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Remove first.
        collection.Remove(1);
        Assert.Empty(collection);
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListRemove_ItemsSorted_Success()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        IList collection = owner.Items;
        collection.Add(2);
        collection.Add(1);
        collection.Add(3);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Remove middle.
        collection.Remove(2);
        Assert.Equal(2, collection.Count);
        Assert.Equal(new int[] { 1, 3 }, collection.Cast<int>());
        Assert.Equal(new int[] { 1, 3 }, owner.Items.Cast<int>());
        Assert.False(owner.IsHandleCreated);

        // Remove again.
        collection.Remove(2);
        Assert.Equal(2, collection.Count);
        Assert.Equal(new int[] { 1, 3 }, collection.Cast<int>());
        Assert.Equal(new int[] { 1, 3 }, owner.Items.Cast<int>());
        Assert.False(owner.IsHandleCreated);

        // Remove last.
        collection.Remove(3);
        Assert.Single(collection);
        Assert.Equal(new int[] { 1 }, collection.Cast<int>());
        Assert.Equal(new int[] { 1 }, owner.Items.Cast<int>());
        Assert.False(owner.IsHandleCreated);

        // Remove first.
        collection.Remove(1);
        Assert.Empty(collection);
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public unsafe void ListBoxObjectCollection_IListRemove_NotSortedWithHandle_Success()
    {
        using ListBox owner = new();
        IList collection = new ListBox.ObjectCollection(owner);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        collection.Add(2);
        collection.Add(1);
        collection.Add(3);
        char* textBuffer = stackalloc char[256];

        // Remove middle.
        collection.Remove(1);
        Assert.Equal(2, collection.Count);
        Assert.Equal(new int[] { 2, 3 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(2, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));

        // Remove again.
        collection.Remove(1);
        Assert.Equal(2, collection.Count);
        Assert.Equal(new int[] { 2, 3 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(2, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));

        // Remove last.
        collection.Remove(3);
        Assert.Single(collection);
        Assert.Equal(new int[] { 2 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(1, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));

        // Remove first.
        collection.Remove(2);
        Assert.Empty(collection);
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(0, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
    }

    [WinFormsFact]
    public unsafe void ListBoxObjectCollection_IListRemove_ItemsNotSortedWithHandle_Success()
    {
        using ListBox owner = new();
        IList collection = owner.Items;
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        collection.Add(2);
        collection.Add(1);
        collection.Add(3);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;
        char* textBuffer = stackalloc char[256];

        // Remove middle.
        collection.Remove(1);
        Assert.Equal(2, collection.Count);
        Assert.Equal(new int[] { 2, 3 }, collection.Cast<int>());
        Assert.Equal(new int[] { 2, 3 }, owner.Items.Cast<int>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(2, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));

        // Remove again.
        collection.Remove(1);
        Assert.Equal(2, collection.Count);
        Assert.Equal(new int[] { 2, 3 }, collection.Cast<int>());
        Assert.Equal(new int[] { 2, 3 }, owner.Items.Cast<int>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(2, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));

        // Remove last.
        collection.Remove(3);
        Assert.Single(collection);
        Assert.Equal(new int[] { 2 }, collection.Cast<int>());
        Assert.Equal(new int[] { 2 }, owner.Items.Cast<int>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(1, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));

        // Remove first.
        collection.Remove(2);
        Assert.Empty(collection);
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(0, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
    }

    [WinFormsFact]
    public unsafe void ListBoxObjectCollection_IListRemove_SortedWithHandle_Success()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        IList collection = new ListBox.ObjectCollection(owner);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        collection.Add(2);
        collection.Add(1);
        collection.Add(3);
        char* textBuffer = stackalloc char[256];

        // Remove middle.
        collection.Remove(2);
        Assert.Equal(2, collection.Count);
        Assert.Equal(new int[] { 1, 3 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(2, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));

        // Remove again.
        collection.Remove(2);
        Assert.Equal(2, collection.Count);
        Assert.Equal(new int[] { 1, 3 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(2, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));

        // Remove last.
        collection.Remove(3);
        Assert.Single(collection);
        Assert.Equal(new int[] { 1 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(1, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));

        // Remove first.
        collection.Remove(1);
        Assert.Empty(collection);
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(0, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
    }

    [WinFormsFact]
    public unsafe void ListBoxObjectCollection_IListRemove_ItemsSortedWithHandle_Success()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        IList collection = owner.Items;
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        collection.Add(2);
        collection.Add(1);
        collection.Add(3);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;
        char* textBuffer = stackalloc char[256];

        // Remove middle.
        collection.Remove(2);
        Assert.Equal(2, collection.Count);
        Assert.Equal(new int[] { 1, 3 }, collection.Cast<int>());
        Assert.Equal(new int[] { 1, 3 }, owner.Items.Cast<int>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(2, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));

        // Remove again.
        collection.Remove(2);
        Assert.Equal(2, collection.Count);
        Assert.Equal(new int[] { 1, 3 }, collection.Cast<int>());
        Assert.Equal(new int[] { 1, 3 }, owner.Items.Cast<int>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(2, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));

        // Remove last.
        collection.Remove(3);
        Assert.Single(collection);
        Assert.Equal(new int[] { 1 }, collection.Cast<int>());
        Assert.Equal(new int[] { 1 }, owner.Items.Cast<int>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(1, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));

        // Remove first.
        collection.Remove(1);
        Assert.Empty(collection);
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(0, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListRemove_WithOneSelectedItemNotSorted_Success()
    {
        using ListBox owner = new();
        IList collection = owner.Items;
        collection.Add(0);
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Remove before.
        collection.Remove(0);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Remove after.
        collection.Remove(4);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Remove selected.
        collection.Remove(1);
        Assert.Empty(owner.SelectedItems);
        Assert.Empty(owner.SelectedIndices);
        Assert.Equal(-1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListRemove_WithOneSelectedItemSorted_Success()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        IList collection = owner.Items;
        collection.Add(0);
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Remove before.
        collection.Remove(0);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Remove after.
        collection.Remove(4);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Remove selected.
        collection.Remove(1);
        Assert.Empty(owner.SelectedItems);
        Assert.Empty(owner.SelectedIndices);
        Assert.Equal(-1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(SelectionMode.MultiExtended)]
    [InlineData(SelectionMode.MultiSimple)]
    public void ListBoxObjectCollection_IListRemove_WithMultipleSelectedItemsNotSorted_Success(SelectionMode selectionMode)
    {
        using ListBox owner = new()
        {
            SelectionMode = selectionMode
        };
        IList collection = owner.Items;
        collection.Add(0);
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        collection.Add(5);
        owner.SelectedItems.Add(1);
        owner.SelectedItems.Add(3);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Remove before.
        collection.Remove(0);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0, 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Remove after.
        collection.Remove(4);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0, 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Remove first selection.
        collection.Remove(1);
        Assert.Equal(new object[] { 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Remove last selection.
        collection.Remove(3);
        Assert.Empty(owner.SelectedItems);
        Assert.Empty(owner.SelectedIndices);
        Assert.Equal(-1, owner.SelectedIndex);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(SelectionMode.MultiExtended)]
    [InlineData(SelectionMode.MultiSimple)]
    public void ListBoxObjectCollection_IListRemove_WithMultipleSelectedItemsSorted_Success(SelectionMode selectionMode)
    {
        using ListBox owner = new()
        {
            SelectionMode = selectionMode,
            Sorted = true
        };
        IList collection = owner.Items;
        collection.Add(0);
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);
        owner.SelectedItems.Add(3);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Remove before.
        collection.Remove(0);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0, 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Remove after.
        collection.Remove(4);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0, 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Remove first selection.
        collection.Remove(1);
        Assert.Equal(new object[] { 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Remove last selection.
        collection.Remove(3);
        Assert.Empty(owner.SelectedItems);
        Assert.Empty(owner.SelectedIndices);
        Assert.Equal(-1, owner.SelectedIndex);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListRemove_WithOneSelectedItemNotSortedWithHandle_Success()
    {
        using ListBox owner = new();
        IList collection = owner.Items;
        collection.Add(0);
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) =>
        {
            Assert.Same(owner, sender);
            Assert.Same(EventArgs.Empty, e);
            selectedIndexChangedCallCount++;
        };

        // Remove before.
        collection.Remove(0);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove after.
        collection.Remove(4);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove selected.
        collection.Remove(1);
        Assert.Empty(owner.SelectedItems);
        Assert.Empty(owner.SelectedIndices);
        Assert.Equal(-1, owner.SelectedIndex);
        Assert.Equal(1, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListRemove_WithOneSelectedItemSortedWithHandle_Success()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        IList collection = owner.Items;
        collection.Add(0);
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) =>
        {
            Assert.Same(owner, sender);
            Assert.Same(EventArgs.Empty, e);
            selectedIndexChangedCallCount++;
        };

        // Remove before.
        collection.Remove(0);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove after.
        collection.Remove(4);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove selected.
        collection.Remove(1);
        Assert.Empty(owner.SelectedItems);
        Assert.Empty(owner.SelectedIndices);
        Assert.Equal(-1, owner.SelectedIndex);
        Assert.Equal(1, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(SelectionMode.MultiExtended)]
    [InlineData(SelectionMode.MultiSimple)]
    public void ListBoxObjectCollection_IListRemove_WithMultipleSelectedItemsNotSortedWithHandle_Success(SelectionMode selectionMode)
    {
        using ListBox owner = new()
        {
            SelectionMode = selectionMode
        };
        IList collection = owner.Items;
        collection.Add(0);
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        collection.Add(5);
        owner.SelectedItems.Add(1);
        owner.SelectedItems.Add(3);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Remove before.
        collection.Remove(0);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0, 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove after.
        collection.Remove(4);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0, 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove first selection.
        collection.Remove(1);
        Assert.Equal(new object[] { 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove last selection.
        collection.Remove(3);
        Assert.Empty(owner.SelectedItems);
        Assert.Empty(owner.SelectedIndices);
        Assert.Equal(-1, owner.SelectedIndex);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(SelectionMode.MultiExtended)]
    [InlineData(SelectionMode.MultiSimple)]
    public void ListBoxObjectCollection_IListRemove_WithMultipleSelectedItemsSortedWithHandle_Success(SelectionMode selectionMode)
    {
        using ListBox owner = new()
        {
            SelectionMode = selectionMode,
            Sorted = true
        };
        IList collection = owner.Items;
        collection.Add(0);
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);
        owner.SelectedItems.Add(3);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Remove before.
        collection.Remove(0);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0, 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove after.
        collection.Remove(4);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0, 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove first selection.
        collection.Remove(1);
        Assert.Equal(new object[] { 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove last selection.
        collection.Remove(3);
        Assert.Empty(owner.SelectedItems);
        Assert.Empty(owner.SelectedIndices);
        Assert.Equal(-1, owner.SelectedIndex);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListRemove_OwnerHasDataSourceEmpty_ThrowsArgumentException()
    {
        using ListBox owner = new()
        {
            DataSource = Array.Empty<object>()
        };
        IList collection = new ListBox.ObjectCollection(owner);
        Assert.Throws<ArgumentException>(() => collection.Remove(1));
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListRemove_OwnerHasDataSourceNotEmpty_ThrowsArgumentException()
    {
        using ListBox owner = new()
        {
            DataSource = new object[] { 1 }
        };
        IList collection = new ListBox.ObjectCollection(owner);
        Assert.Throws<ArgumentException>(() => collection.Remove(1));
        Assert.Throws<ArgumentException>(() => collection.Remove(2));
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListRemoveAt_NotSorted_Success()
    {
        using ListBox owner = new();
        IList collection = new ListBox.ObjectCollection(owner)
        {
            2,
            1,
            3
        };
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Remove middle.
        collection.RemoveAt(1);
        Assert.Equal(2, collection.Count);
        Assert.Equal(new int[] { 2, 3 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Remove last.
        collection.RemoveAt(1);
        Assert.Single(collection);
        Assert.Equal(new int[] { 2 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Remove first.
        collection.RemoveAt(0);
        Assert.Empty(collection);
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListRemoveAt_ItemsNotSorted_Success()
    {
        using ListBox owner = new();
        IList collection = owner.Items;
        collection.Add(2);
        collection.Add(1);
        collection.Add(3);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Remove middle.
        collection.RemoveAt(1);
        Assert.Equal(2, collection.Count);
        Assert.Equal(new int[] { 2, 3 }, collection.Cast<int>());
        Assert.Equal(new int[] { 2, 3 }, owner.Items.Cast<int>());
        Assert.False(owner.IsHandleCreated);

        // Remove last.
        collection.RemoveAt(1);
        Assert.Single(collection);
        Assert.Equal(new int[] { 2 }, collection.Cast<int>());
        Assert.Equal(new int[] { 2 }, owner.Items.Cast<int>());
        Assert.False(owner.IsHandleCreated);

        // Remove first.
        collection.RemoveAt(0);
        Assert.Empty(collection);
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListRemoveAt_Sorted_Success()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        IList collection = new ListBox.ObjectCollection(owner)
        {
            2,
            1,
            3
        };
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Remove middle.
        collection.RemoveAt(1);
        Assert.Equal(2, collection.Count);
        Assert.Equal(new int[] { 1, 3 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Remove last.
        collection.RemoveAt(1);
        Assert.Single(collection);
        Assert.Equal(new int[] { 1 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Remove first.
        collection.RemoveAt(0);
        Assert.Empty(collection);
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListRemoveAt_ItemsSorted_Success()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        IList collection = owner.Items;
        collection.Add(2);
        collection.Add(1);
        collection.Add(3);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Remove middle.
        collection.RemoveAt(1);
        Assert.Equal(2, collection.Count);
        Assert.Equal(new int[] { 1, 3 }, collection.Cast<int>());
        Assert.Equal(new int[] { 1, 3 }, owner.Items.Cast<int>());
        Assert.False(owner.IsHandleCreated);

        // Remove last.
        collection.RemoveAt(1);
        Assert.Single(collection);
        Assert.Equal(new int[] { 1 }, collection.Cast<int>());
        Assert.Equal(new int[] { 1 }, owner.Items.Cast<int>());
        Assert.False(owner.IsHandleCreated);

        // Remove first.
        collection.RemoveAt(0);
        Assert.Empty(collection);
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public unsafe void ListBoxObjectCollection_IListRemoveAt_NotSortedWithHandle_Success()
    {
        using ListBox owner = new();
        IList collection = new ListBox.ObjectCollection(owner);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        collection.Add(2);
        collection.Add(1);
        collection.Add(3);
        char* textBuffer = stackalloc char[256];

        // Remove middle.
        collection.RemoveAt(1);
        Assert.Equal(2, collection.Count);
        Assert.Equal(new int[] { 2, 3 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(2, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));

        // Remove last.
        collection.RemoveAt(1);
        Assert.Single(collection);
        Assert.Equal(new int[] { 2 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(1, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));

        // Remove first.
        collection.RemoveAt(0);
        Assert.Empty(collection);
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(0, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
    }

    [WinFormsFact]
    public unsafe void ListBoxObjectCollection_IListRemoveAt_ItemsNotSortedWithHandle_Success()
    {
        using ListBox owner = new();
        IList collection = owner.Items;
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        collection.Add(2);
        collection.Add(1);
        collection.Add(3);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;
        char* textBuffer = stackalloc char[256];

        // Remove middle.
        collection.RemoveAt(1);
        Assert.Equal(2, collection.Count);
        Assert.Equal(new int[] { 2, 3 }, collection.Cast<int>());
        Assert.Equal(new int[] { 2, 3 }, owner.Items.Cast<int>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(2, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));

        // Remove last.
        collection.RemoveAt(1);
        Assert.Single(collection);
        Assert.Equal(new int[] { 2 }, collection.Cast<int>());
        Assert.Equal(new int[] { 2 }, owner.Items.Cast<int>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(1, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("2", new string(textBuffer));

        // Remove first.
        collection.RemoveAt(0);
        Assert.Empty(collection);
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(0, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
    }

    [WinFormsFact]
    public unsafe void ListBoxObjectCollection_IListRemoveAt_SortedWithHandle_Success()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        IList collection = new ListBox.ObjectCollection(owner);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        collection.Add(2);
        collection.Add(1);
        collection.Add(3);
        char* textBuffer = stackalloc char[256];

        // Remove middle.
        collection.RemoveAt(1);
        Assert.Equal(2, collection.Count);
        Assert.Equal(new int[] { 1, 3 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(2, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));

        // Remove last.
        collection.RemoveAt(1);
        Assert.Single(collection);
        Assert.Equal(new int[] { 1 }, collection.Cast<int>());
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(1, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));

        // Remove first.
        collection.RemoveAt(0);
        Assert.Empty(collection);
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(0, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
    }

    [WinFormsFact]
    public unsafe void ListBoxObjectCollection_IListRemoveAt_ItemsSortedWithHandle_Success()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        IList collection = owner.Items;
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        collection.Add(2);
        collection.Add(1);
        collection.Add(3);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;
        char* textBuffer = stackalloc char[256];

        // Remove middle.
        collection.RemoveAt(1);
        Assert.Equal(2, collection.Count);
        Assert.Equal(new int[] { 1, 3 }, collection.Cast<int>());
        Assert.Equal(new int[] { 1, 3 }, owner.Items.Cast<int>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(2, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 1, (nint)textBuffer);
        Assert.Equal("3", new string(textBuffer));

        // Remove last.
        collection.RemoveAt(1);
        Assert.Single(collection);
        Assert.Equal(new int[] { 1 }, collection.Cast<int>());
        Assert.Equal(new int[] { 1 }, owner.Items.Cast<int>());
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(1, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
        PInvokeCore.SendMessage(owner, PInvoke.LB_GETTEXT, 0, (nint)textBuffer);
        Assert.Equal("1", new string(textBuffer));

        // Remove first.
        collection.RemoveAt(0);
        Assert.Empty(collection);
        Assert.Empty(collection);
        Assert.Empty(owner.Items);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(0, (int)PInvokeCore.SendMessage(owner, PInvoke.LB_GETCOUNT));
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListRemoveAt_WithOneSelectedItemNotSorted_Success()
    {
        using ListBox owner = new();
        IList collection = owner.Items;
        collection.Add(0);
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Remove before.
        collection.RemoveAt(0);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Remove after.
        collection.RemoveAt(2);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Remove selected.
        collection.RemoveAt(0);
        Assert.Empty(owner.SelectedItems);
        Assert.Empty(owner.SelectedIndices);
        Assert.Equal(-1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListRemoveAt_WithOneSelectedItemSorted_Success()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        IList collection = owner.Items;
        collection.Add(0);
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Remove before.
        collection.RemoveAt(0);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Remove after.
        collection.RemoveAt(2);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Remove selected.
        collection.RemoveAt(0);
        Assert.Empty(owner.SelectedItems);
        Assert.Empty(owner.SelectedIndices);
        Assert.Equal(-1, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(SelectionMode.MultiExtended)]
    [InlineData(SelectionMode.MultiSimple)]
    public void ListBoxObjectCollection_IListRemoveAt_WithMultipleSelectedItemsNotSorted_Success(SelectionMode selectionMode)
    {
        using ListBox owner = new()
        {
            SelectionMode = selectionMode
        };
        IList collection = owner.Items;
        collection.Add(0);
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        collection.Add(5);
        owner.SelectedItems.Add(1);
        owner.SelectedItems.Add(3);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Remove before.
        collection.RemoveAt(0);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0, 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Remove after.
        collection.RemoveAt(2);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0, 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Remove first selection.
        collection.RemoveAt(0);
        Assert.Equal(new object[] { 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Remove last selection.
        collection.RemoveAt(0);
        Assert.Empty(owner.SelectedItems);
        Assert.Empty(owner.SelectedIndices);
        Assert.Equal(-1, owner.SelectedIndex);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(SelectionMode.MultiExtended)]
    [InlineData(SelectionMode.MultiSimple)]
    public void ListBoxObjectCollection_IListRemoveAt_WithMultipleSelectedItemsSorted_Success(SelectionMode selectionMode)
    {
        using ListBox owner = new()
        {
            SelectionMode = selectionMode,
            Sorted = true
        };
        IList collection = owner.Items;
        collection.Add(0);
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);
        owner.SelectedItems.Add(3);
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Remove before.
        collection.RemoveAt(0);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0, 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Remove after.
        collection.RemoveAt(2);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0, 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Remove first selection.
        collection.RemoveAt(0);
        Assert.Equal(new object[] { 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.False(owner.IsHandleCreated);

        // Remove last selection.
        collection.RemoveAt(0);
        Assert.Empty(owner.SelectedItems);
        Assert.Empty(owner.SelectedIndices);
        Assert.Equal(-1, owner.SelectedIndex);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListRemoveAt_WithOneSelectedItemNotSortedWithHandle_Success()
    {
        using ListBox owner = new();
        IList collection = owner.Items;
        collection.Add(0);
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) =>
        {
            Assert.Same(owner, sender);
            Assert.Same(EventArgs.Empty, e);
            selectedIndexChangedCallCount++;
        };

        // Remove before.
        collection.RemoveAt(0);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove after.
        collection.RemoveAt(2);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove selected.
        collection.RemoveAt(0);
        Assert.Empty(owner.SelectedItems);
        Assert.Empty(owner.SelectedIndices);
        Assert.Equal(-1, owner.SelectedIndex);
        Assert.Equal(1, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListRemoveAt_WithOneSelectedItemSortedWithHandle_Success()
    {
        using ListBox owner = new()
        {
            Sorted = true
        };
        IList collection = owner.Items;
        collection.Add(0);
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) =>
        {
            Assert.Same(owner, sender);
            Assert.Same(EventArgs.Empty, e);
            selectedIndexChangedCallCount++;
        };

        // Remove before.
        collection.RemoveAt(0);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove after.
        collection.RemoveAt(2);
        Assert.Equal(new object[] { 1 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.Equal(0, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove selected.
        collection.RemoveAt(0);
        Assert.Empty(owner.SelectedItems);
        Assert.Empty(owner.SelectedIndices);
        Assert.Equal(-1, owner.SelectedIndex);
        Assert.Equal(1, selectedIndexChangedCallCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(SelectionMode.MultiExtended)]
    [InlineData(SelectionMode.MultiSimple)]
    public void ListBoxObjectCollection_IListRemoveAt_WithMultipleSelectedItemsNotSortedWithHandle_Success(SelectionMode selectionMode)
    {
        using ListBox owner = new()
        {
            SelectionMode = selectionMode
        };
        IList collection = owner.Items;
        collection.Add(0);
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        collection.Add(5);
        owner.SelectedItems.Add(1);
        owner.SelectedItems.Add(3);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Remove before.
        collection.RemoveAt(0);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0, 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove after.
        collection.RemoveAt(2);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0, 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove first selection.
        collection.RemoveAt(0);
        Assert.Equal(new object[] { 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove last selection.
        collection.RemoveAt(0);
        Assert.Empty(owner.SelectedItems);
        Assert.Empty(owner.SelectedIndices);
        Assert.Equal(-1, owner.SelectedIndex);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(SelectionMode.MultiExtended)]
    [InlineData(SelectionMode.MultiSimple)]
    public void ListBoxObjectCollection_IListRemoveAt_WithMultipleSelectedItemsSortedWithHandle_Success(SelectionMode selectionMode)
    {
        using ListBox owner = new()
        {
            SelectionMode = selectionMode,
            Sorted = true
        };
        IList collection = owner.Items;
        collection.Add(0);
        collection.Add(1);
        collection.Add(3);
        collection.Add(4);
        owner.SelectedItems.Add(1);
        owner.SelectedItems.Add(3);
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        int selectedIndexChangedCallCount = 0;
        owner.SelectedIndexChanged += (sender, e) => selectedIndexChangedCallCount++;

        // Remove before.
        collection.RemoveAt(0);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0, 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove after.
        collection.RemoveAt(2);
        Assert.Equal(new object[] { 1, 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0, 1 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove first selection.
        collection.RemoveAt(0);
        Assert.Equal(new object[] { 3 }, owner.SelectedItems.Cast<object>());
        Assert.Equal(new int[] { 0 }, owner.SelectedIndices.Cast<int>());
        Assert.Equal(0, owner.SelectedIndex);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove last selection.
        collection.RemoveAt(0);
        Assert.Empty(owner.SelectedItems);
        Assert.Empty(owner.SelectedIndices);
        Assert.Equal(-1, owner.SelectedIndex);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void ListBoxObjectCollection_IListRemoveAt_InvalidIndexEmpty_ThrowsArgumentOutOfRangeException(int index)
    {
        using ListBox owner = new();
        IList collection = new ListBox.ObjectCollection(owner);
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.RemoveAt(index));
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(1)]
    [InlineData(2)]
    public void ListBoxObjectCollection_IListRemoveAt_InvalidIndexNotEmpty_ThrowsArgumentOutOfRangeException(int index)
    {
        using ListBox owner = new();
        IList collection = new ListBox.ObjectCollection(owner)
        {
            1
        };
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.RemoveAt(index));
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListRemoveAt_OwnerHasDataSourceEmpty_ThrowsArgumentException()
    {
        using ListBox owner = new()
        {
            DataSource = Array.Empty<object>()
        };
        IList collection = new ListBox.ObjectCollection(owner);
        Assert.Throws<ArgumentException>(() => collection.RemoveAt(1));
    }

    [WinFormsFact]
    public void ListBoxObjectCollection_IListRemoveAt_OwnerHasDataSourceNotEmpty_ThrowsArgumentException()
    {
        using ListBox owner = new()
        {
            DataSource = new object[] { 1 }
        };
        IList collection = new ListBox.ObjectCollection(owner);
        Assert.Throws<ArgumentException>(() => collection.RemoveAt(1));
    }

    private class CustomAddStringListBox : ListBox
    {
        public IntPtr AddStringResult { get; set; }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == (int)PInvoke.LB_ADDSTRING)
            {
                m.Result = AddStringResult;
                return;
            }

            base.WndProc(ref m);
        }
    }

    private class CustomInsertStringListBox : ListBox
    {
        public IntPtr InsertStringResult { get; set; }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == (int)PInvoke.LB_INSERTSTRING)
            {
                m.Result = InsertStringResult;
                return;
            }

            base.WndProc(ref m);
        }
    }
}
