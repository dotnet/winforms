// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using Microsoft.DotNet.RemoteExecutor;
using Moq;

namespace System.Windows.Forms.Design.Tests;

/// <summary>
///  Verifies that applying edited items preserves valid ListView group membership.
/// </summary>
public class ListViewItemCollectionEditorTests
{
    [WinFormsTheory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public void SetItems_PreservesGroups(bool createHandle, bool showGroups)
    {
        using ListView listView = new() { ShowGroups = showGroups };
        ListViewGroup firstGroup = new("First");
        ListViewGroup secondGroup = new("Second");
        listView.Groups.AddRange([firstGroup, secondGroup]);
        ListViewItem firstItem = listView.Items.Add("First");
        ListViewItem secondItem = listView.Items.Add("Second");
        ListViewItem thirdItem = listView.Items.Add("Third");
        ListViewItem ungroupedItem = listView.Items.Add("Ungrouped");

        if (createHandle)
        {
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
        }

        firstItem.Group = firstGroup;
        secondItem.Group = secondGroup;
        thirdItem.Group = firstGroup;
        SubListViewItemCollectionEditor editor = new();

        object? result = editor.SetItems(listView.Items, [firstItem, secondItem, thirdItem, ungroupedItem]);

        Assert.Same(listView.Items, result);
        Assert.Equal([firstItem, secondItem, thirdItem, ungroupedItem], listView.Items.Cast<ListViewItem>());
        Assert.Same(firstGroup, firstItem.Group);
        Assert.Same(secondGroup, secondItem.Group);
        Assert.Same(firstGroup, thirdItem.Group);
        Assert.Null(ungroupedItem.Group);
        Assert.Equal([firstItem, thirdItem], firstGroup.Items.Cast<ListViewItem>());
        Assert.Equal([secondItem], secondGroup.Items.Cast<ListViewItem>());
        Assert.Equal(createHandle, listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(false)]
    [InlineData(true)]
    public void SetItems_NewItemWithGroup_PreservesGroup(bool createHandle)
    {
        using ListView listView = new();
        ListViewGroup group = new("Group");
        listView.Groups.Add(group);
        ListViewItem existingItem = listView.Items.Add("Existing");
        existingItem.Group = group;

        if (createHandle)
        {
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
        }

        ListViewItem newItem = new("New") { Group = group };
        SubListViewItemCollectionEditor editor = new();

        editor.SetItems(listView.Items, [existingItem, newItem]);

        Assert.Same(group, existingItem.Group);
        Assert.Same(group, newItem.Group);
        Assert.Equal([existingItem, newItem], listView.Items.Cast<ListViewItem>());
        Assert.Equal(2, group.Items.Count);
        Assert.Single(group.Items.Cast<ListViewItem>(), item => item == existingItem);
        Assert.Single(group.Items.Cast<ListViewItem>(), item => item == newItem);
    }

    [WinFormsTheory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public void SetItems_InvalidGroup_DoesNotRestoreGroup(bool createHandle, bool removeGroup)
    {
        using ListView listView = new();
        using ListView otherListView = new();
        ListViewGroup group = new("Group");
        (removeGroup ? listView : otherListView).Groups.Add(group);
        ListViewItem item = new("Item") { Group = group };
        listView.Items.Add(item);

        if (createHandle)
        {
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
        }

        if (removeGroup)
        {
            listView.Groups.Remove(group);
        }

        SubListViewItemCollectionEditor editor = new();

        editor.SetItems(listView.Items, [item]);

        Assert.Null(item.Group);
        Assert.Same(item, Assert.Single(listView.Items.Cast<ListViewItem>()));
        Assert.Empty(group.Items);
    }

    [WinFormsTheory]
    [InlineData(false)]
    [InlineData(true)]
    public void SetItems_NullValue_ClearsItems(bool createHandle)
    {
        using ListView listView = new();
        ListViewGroup group = new("Group");
        listView.Groups.Add(group);
        ListViewItem item = listView.Items.Add("Item");
        item.Group = group;

        if (createHandle)
        {
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
        }

        SubListViewItemCollectionEditor editor = new();

        object? result = editor.SetItems(listView.Items, null);

        Assert.Same(listView.Items, result);
        Assert.Empty(listView.Items);
        Assert.Null(item.Group);
        Assert.Empty(group.Items);
    }

    [WinFormsTheory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public void SetItems_RemovedItem_PreservesExpectedGroupMembership(bool createHandle, bool designMode)
    {
        RemoteExecutor.Invoke((createHandleValue, designModeValue) =>
        {
            Application.EnableVisualStyles();
            bool shouldCreateHandle = bool.Parse(createHandleValue);
            bool isDesignMode = bool.Parse(designModeValue);
            using ListView listView = new();
            Mock<ISite> site = new();
            site.Setup(s => s.DesignMode).Returns(isDesignMode);
            listView.Site = site.Object;
            ListViewGroup group = new("Group");
            listView.Groups.Add(group);
            ListViewItem retainedItem = listView.Items.Add("Retained");
            ListViewItem removedItem = listView.Items.Add("Removed");
            retainedItem.Group = group;
            removedItem.Group = group;

            if (shouldCreateHandle)
            {
                Assert.NotEqual(IntPtr.Zero, listView.Handle);
            }

            SubListViewItemCollectionEditor editor = new();

            editor.SetItems(listView.Items, [retainedItem]);

            Assert.Same(retainedItem, Assert.Single(listView.Items.Cast<ListViewItem>()));
            Assert.Same(group, retainedItem.Group);
            Assert.Null(removedItem.ListView);
            Assert.Equal(shouldCreateHandle, listView.IsHandleCreated);

            if (isDesignMode)
            {
                // Design-time unhosting intentionally retains a removed item's group.
                Assert.Same(group, removedItem.Group);
                Assert.Equal([retainedItem, removedItem], group.Items.Cast<ListViewItem>());
            }
            else
            {
                Assert.Null(removedItem.Group);
                Assert.Same(retainedItem, Assert.Single(group.Items.Cast<ListViewItem>()));
            }
        }, createHandle.ToString(), designMode.ToString()).Dispose();
    }

    [WinFormsTheory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public void SetItems_ChangedGroup_PreservesEditedGroupAfterHandleRecreation(bool createHandle, bool designMode)
    {
        RemoteExecutor.Invoke((createHandleValue, designModeValue) =>
        {
            // Native grouping requires visual styles; isolate the process-wide setting.
            Application.EnableVisualStyles();
            bool shouldCreateHandle = bool.Parse(createHandleValue);
            using SubListView listView = new();
            Mock<ISite> site = new();
            site.Setup(s => s.DesignMode).Returns(bool.Parse(designModeValue));
            listView.Site = site.Object;
            ListViewGroup originalGroup = new("Original");
            ListViewGroup editedGroup = new("Edited");
            listView.Groups.AddRange([originalGroup, editedGroup]);
            ListViewItem changedItem = listView.Items.Add("Changed");
            ListViewItem clearedItem = listView.Items.Add("Cleared");
            changedItem.Group = originalGroup;
            clearedItem.Group = originalGroup;

            if (shouldCreateHandle)
            {
                Assert.NotEqual(IntPtr.Zero, listView.Handle);
            }

            changedItem.Group = editedGroup;
            clearedItem.Group = null;
            SubListViewItemCollectionEditor editor = new();

            editor.SetItems(listView.Items, [changedItem, clearedItem]);

            AssertGroupMembership();
            Assert.Equal(shouldCreateHandle, listView.IsHandleCreated);

            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            AssertGroupMembership();

            int handleCreatedCallCount = 0;
            listView.HandleCreated += (_, _) => handleCreatedCallCount++;

            listView.RecreateHandle();

            Assert.True(listView.IsHandleCreated);
            Assert.Equal(1, handleCreatedCallCount);
            AssertGroupMembership();

            void AssertGroupMembership()
            {
                Assert.Same(editedGroup, changedItem.Group);
                Assert.Null(clearedItem.Group);
                Assert.Empty(originalGroup.Items);
                Assert.Same(changedItem, Assert.Single(editedGroup.Items.Cast<ListViewItem>()));
            }
        }, createHandle.ToString(), designMode.ToString()).Dispose();
    }

    [WinFormsTheory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public void SetItems_MultipleSelectedGroupedItems_PreservesGroupMembership(bool createHandle, bool designMode)
    {
        RemoteExecutor.Invoke((createHandleValue, designModeValue) =>
        {
            Application.EnableVisualStyles();
            bool shouldCreateHandle = bool.Parse(createHandleValue);
            using SubListView listView = new();
            Mock<ISite> site = new();
            site.Setup(s => s.DesignMode).Returns(bool.Parse(designModeValue));
            listView.Site = site.Object;
            ListViewGroup firstGroup = new("First");
            ListViewGroup secondGroup = new("Second");
            listView.Groups.AddRange([firstGroup, secondGroup]);
            ListViewItem firstItem = listView.Items.Add("First");
            ListViewItem secondItem = listView.Items.Add("Second");
            ListViewItem thirdItem = listView.Items.Add("Third");
            ListViewItem unselectedItem = listView.Items.Add("Unselected");
            firstItem.Group = firstGroup;
            secondItem.Group = firstGroup;
            thirdItem.Group = secondGroup;
            unselectedItem.Group = secondGroup;

            if (shouldCreateHandle)
            {
                Assert.NotEqual(IntPtr.Zero, listView.Handle);
            }

            // The collection editor passes its selected items to PropertyGrid.SelectedObjects.
            using PropertyGrid propertyGrid = new()
            {
                SelectedObjects = [firstItem, secondItem, thirdItem]
            };
            Assert.Equal([firstItem, secondItem, thirdItem], propertyGrid.SelectedObjects);
            SubListViewItemCollectionEditor editor = new();

            editor.SetItems(listView.Items, [firstItem, secondItem, thirdItem, unselectedItem]);

            AssertGroupMembership();
            Assert.Equal(shouldCreateHandle, listView.IsHandleCreated);

            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            listView.RecreateHandle();

            AssertGroupMembership();

            void AssertGroupMembership()
            {
                Assert.Equal([firstItem, secondItem, thirdItem, unselectedItem], listView.Items.Cast<ListViewItem>());
                Assert.Same(firstGroup, firstItem.Group);
                Assert.Same(firstGroup, secondItem.Group);
                Assert.Same(secondGroup, thirdItem.Group);
                Assert.Same(secondGroup, unselectedItem.Group);
                Assert.Equal([firstItem, secondItem], firstGroup.Items.Cast<ListViewItem>());
                Assert.Equal([thirdItem, unselectedItem], secondGroup.Items.Cast<ListViewItem>());
            }
        }, createHandle.ToString(), designMode.ToString()).Dispose();
    }

    /// <summary>
    ///  Exposes handle recreation for group membership verification.
    /// </summary>
    private class SubListView : ListView
    {
        public new void RecreateHandle() => base.RecreateHandle();
    }

    /// <summary>
    ///  Exposes collection application without reflection or a modal editor dialog.
    /// </summary>
    private class SubListViewItemCollectionEditor()
        : ListViewItemCollectionEditor(typeof(ListView.ListViewItemCollection))
    {
        /// <inheritdoc />
        public new object? SetItems(object? editValue, object[]? value)
            => base.SetItems(editValue!, value!);
    }
}
