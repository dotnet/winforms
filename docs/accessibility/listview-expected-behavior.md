# ListView accessibility objects expected behavior

This document describes the expected behavior of [ListView](https://docs.microsoft.com/dotnet/api/system.windows.forms.listview) , [ListView](https://docs.microsoft.com/dotnet/api/system.windows.forms.listviewgroup), [ListViewSubItem](https://docs.microsoft.com/dotnet/api/system.windows.forms.listviewitem.listviewsubitem) accessibility objects when using Inspect.


- [Overview](#Overview)
- [ListView](#ListView)
    - [Accessibility tree for ListView](##Accessibility-tree-for-ListView)
- [ListViewGroup](#ListViewGroup)
    - [Accessibility tree for a ListView with displayed ListViewGroups](##Accessibility-tree-for-a-ListView-with-displayed-ListViewGroups)
	- [Accessibility tree for a ListView with disabled visual styles](##Accessibility-tree-for-a-ListView-with-disabled-visual-styles)	
	- [Accessibility tree for a ListView with disabled ListViewGroups](##Accessibility-tree-for-a-ListView-with-disabled-ListViewGroups)
	- [Accessibility tree for a ListView in virtual mode](##Accessibility-tree-for-a-ListView-in-virtual-mode)
	- [Accessibility tree for a ListView without ListViewGroups](##Accessibility-tree-for-a-ListView-without-ListViewGroups)
- [ListViewSubItem](#ListViewSubItem)
    - [Accessibility tree for a ListViewItem](##Accessibility-tree-for-a-ListViewItem)
	- [Accessibility tree for a ListViewItem (Details view)](##Accessibility-tree-for-a-ListViewItem-(Details-view))
	- [Accessibility tree for a ListViewItem (Tile view)](##Accessibility-tree-for-a-ListViewItem-(Tile-view))

# Overview

[ListView](https://docs.microsoft.com/dotnet/api/system.windows.forms.listview) is a Win32 native control provided by Windows.
Windows Forms runtime provides a support for [UI Automation](https://docs.microsoft.com/dotnet/framework/ui-automation/ui-automation-overview) and, thus, responsible for providing required accessible objects with correct properties and actions to make all aspects of the `ListView` control accessible for customers.

# ListView

## Accessibility tree for ListView

`ListView` accessibility tree must contain all visible items based on the selected `View` [view](https://docs.microsoft.com/dotnet/api/system.windows.forms.listview.view).

<details>
<summary>1. Details view</summary>
</br>

![listview-inspect-details-view-tree][listview-inspect-details-view-tree]

</details>
</br>

<details>
<summary>2. LargeIcon view</summary>
</br>

![listview-inspect-largeicon-view-tree][listview-inspect-largeicon-view-tree]

</details>
</br>

<details>
<summary>3. List view</summary>
</br>

![listview-inspect-list-view-tree][listview-inspect-list-view-tree]

</details>
</br>

<details>
<summary>4. SmallIcon view</summary>
</br>

![listview-inspect-smallicon-view-tree][listview-inspect-smallicon-view-tree]

</details>
</br>

<details>
<summary>5. Tile view</summary>
</br>

![listview-inspect-tile-view-tree][listview-inspect-tile-view-tree]

</details>
</br>

# ListViewGroup

The `ListView` always contains one or more [ListViewGroup](https://docs.microsoft.com/dotnet/api/system.windows.forms.listviewgroup), but depending on a configuration of the operating system, application and/or the `ListView` itself, these [groups](https://docs.microsoft.com/dotnet/api/system.windows.forms.listview.groups) can be visible or not.

## Accessibility tree for a ListView with displayed ListViewGroups

A `ListViewGroup` is display if the following conditions are met:
1. An application has [visual styles](https://docs.microsoft.com/dotnet/api/system.windows.forms.application.enablevisualstyles) are enabled.
2. A `ListView` is not in a [virtual mode](https://docs.microsoft.com/dotnet/api/system.windows.forms.listview.virtualmode).
3. The [ShowGroups](https://docs.microsoft.com/dotnet/api/system.windows.forms.listview.showgroups) property has `true` value
4. A `ListView` has one or more [groups](https://docs.microsoft.com/dotnet/api/system.windows.forms.listview.groups)
5. A `ListView` is not in a `List` view

```cs
ListViewGroup listViewGroup = new ListViewGroup("Test group");
listView.Groups.Add(listViewGroup);

listView.ShowGroups = true;
listView.Columns.Add(new ColumnHeader { Text = "Column 1" });
listView.Columns.Add(new ColumnHeader { Text = "Column 2" });
listView.Columns.Add(new ColumnHeader { Text = "Column 3" });
listView.Columns.Add(new ColumnHeader { Text = "Column 4" });

listView.Items.Add(new ListViewItem(new string[] { "Item 1", "SubItem 11", "SubItem 12", "SubItem 13" }, imageIndex: 0, group: listViewGroup));
listView.Items.Add(new ListViewItem(new string[] { "Item 2", "SubItem 21", "SubItem 22", "SubItem 23" }, imageIndex: 0, group: listViewGroup));
listView.Items.Add(new ListViewItem(new string[] { "Item 3", "SubItem 31" }, imageIndex: 0));
```

<details>
<summary>1. Details view</summary>
</br>

![listview-withgroup-inspect-details-view-tree][listview-withgroup-inspect-details-view-tree]

</details>
</br>

<details>
<summary>2. LargeIcon view</summary>
</br>

![listview-withgroup-inspect-largeicon-view-tree][listview-withgroup-inspect-largeicon-view-tree]

</details>
</br>

<details>
<summary>3. List view</summary>
</br>

![listview-withgroup-inspect-list-view-tree][listview-withgroup-inspect-list-view-tree]

</details>
</br>

<details>
<summary>4. SmallIcon view</summary>
</br>

![listview-withgroup-inspect-smallicon-view-tree][listview-withgroup-inspect-smallicon-view-tree]

</details>
</br>

<details>
<summary>5. Tile view</summary>
</br>

![listview-withgroup-inspect-tile-view-tree][listview-withgroup-inspect-tile-view-tree]

</details>
</br>

## Accessibility tree for a ListView with disabled visual styles
`ListViewGroups` are not visible when [visual styles](https://docs.microsoft.com/dotnet/api/system.windows.forms.application.enablevisualstyles) are disabled.

```cs
ListViewGroup listViewGroup = new ListViewGroup("Test group");
listView.Groups.Add(listViewGroup);

listView.ShowGroups = true;
listView.Columns.Add(new ColumnHeader { Text = "Column 1" });
listView.Columns.Add(new ColumnHeader { Text = "Column 2" });
listView.Columns.Add(new ColumnHeader { Text = "Column 3" });
listView.Columns.Add(new ColumnHeader { Text = "Column 4" });

listView.Items.Add(new ListViewItem(new string[] { "Item 1", "SubItem 11", "SubItem 12", "SubItem 13" }, imageIndex: 0, group: listViewGroup));
listView.Items.Add(new ListViewItem(new string[] { "Item 2", "SubItem 21", "SubItem 22", "SubItem 23" }, imageIndex: 0, group: listViewGroup));
listView.Items.Add(new ListViewItem(new string[] { "Item 3", "SubItem 31" }, imageIndex: 0));
```

<details>
<summary>1. Details view</summary>
</br>

![listview-disabledstyles-inspect-details-view-tree][listview-disabledstyles-inspect-details-view-tree]

</details>
</br>

<details>
<summary>2. LargeIcon view</summary>
</br>

![listview-disabledstyles-inspect-largeicon-view-tree][listview-disabledstyles-inspect-largeicon-view-tree]

</details>
</br>

<details>
<summary>3. List view</summary>
</br>

![listview-disabledstyles-inspect-list-view-tree][listview-disabledstyles-inspect-list-view-tree]

</details>
</br>

<details>
<summary>4. SmallIcon view</summary>
</br>

![listview-disabledstyles-inspect-smallicon-view-tree][listview-disabledstyles-inspect-smallicon-view-tree]

</details>
</br>

<details>
<summary>5. Tile view</summary>
</br>

![listview-disabledstyles-inspect-tile-view-tree][listview-disabledstyles-inspect-tile-view-tree]

</details>
</br>

## Accessibility tree for a ListView with disabled ListViewGroups
`ListViewGroups` are not visible when [ShowGroups](https://docs.microsoft.com/dotnet/api/system.windows.forms.listview.showgroups) property is set to `false`.

```cs
ListViewGroup listViewGroup = new ListViewGroup("Test group");
listView.Groups.Add(listViewGroup);

listView.ShowGroups = false;
listView.Columns.Add(new ColumnHeader { Text = "Column 1" });
listView.Columns.Add(new ColumnHeader { Text = "Column 2" });
listView.Columns.Add(new ColumnHeader { Text = "Column 3" });
listView.Columns.Add(new ColumnHeader { Text = "Column 4" });

listView.Items.Add(new ListViewItem(new string[] { "Item 1", "SubItem 11", "SubItem 12", "SubItem 13" }, imageIndex: 0, group: listViewGroup));
listView.Items.Add(new ListViewItem(new string[] { "Item 2", "SubItem 21", "SubItem 22", "SubItem 23" }, imageIndex: 0, group: listViewGroup));
listView.Items.Add(new ListViewItem(new string[] { "Item 3", "SubItem 31" }, imageIndex: 0));
```

<details>
<summary>1. Details view</summary>
</br>

![listview-showgroupdisabled-inspect-details-view-tree][listview-showgroupdisabled-inspect-details-view-tree]

</details>
</br>

<details>
<summary>2. LargeIcon view</summary>
</br>

![listview-showgroupdisabled-inspect-largeicon-view-tree][listview-showgroupdisabled-inspect-largeicon-view-tree]

</details>
</br>

<details>
<summary>3. List view</summary>
</br>

![listview-showgroupdisabled-inspect-list-view-tree][listview-showgroupdisabled-inspect-list-view-tree]

</details>
</br>

<details>
<summary>4. SmallIcon view</summary>
</br>

![listview-showgroupdisabled-inspect-smallicon-view-tree][listview-showgroupdisabled-inspect-smallicon-view-tree]

</details>
</br>

<details>
<summary>5. Tile view</summary>
</br>

![listview-showgroupdisabled-inspect-tile-view-tree][listview-showgroupdisabled-inspect-tile-view-tree]

</details>
</br>

## Accessibility tree for a ListView in virtual mode
`ListViewGroups` are not visible when `ListView` is in [virtual mode](https://docs.microsoft.com/dotnet/api/system.windows.forms.listview.virtualmode).

```cs
ListViewGroup listViewGroup = new ListViewGroup("Test group");
listView.Groups.Add(listViewGroup);

listView.VirtualMode = virtualMode;
listView.VirtualListSize = 3;

listView.Columns.Add(new ColumnHeader { Text = "Column 1" });
listView.Columns.Add(new ColumnHeader { Text = "Column 2" });
listView.Columns.Add(new ColumnHeader { Text = "Column 3" });
listView.Columns.Add(new ColumnHeader { Text = "Column 4" });

ListViewItem listItem1 = new ListViewItem(new string[] { "Item 1", "SubItem 11", "SubItem 12", "SubItem 13" }, imageIndex: 0, group: listViewGroup));
ListViewItem listItem2 = new ListViewItem(new string[] { "Item 2", "SubItem 21", "SubItem 22", "SubItem 23" }, imageIndex: 0, group: listViewGroup));
ListViewItem listItem3 = new ListViewItem(new string[] { "Item 3", "SubItem 31", "SubItem 32", "SubItem 33" }, imageIndex: 0));

listView.RetrieveVirtualItem += (s, e) =>
    {
        e.Item = e.ItemIndex switch
        {
            0 => listItem1,
            1 => listItem2,
            2 => listItem3,
            _ => throw new NotImplementedException()
        };
    };

listItem1.SetItemIndex(listView, 0);
listItem2.SetItemIndex(listView, 1);
listItem3.SetItemIndex(listView, 2);
```

<details>
<summary>1. Details view</summary>
</br>

![listview-virtualmode-inspect-details-view-tree][listview-virtualmode-inspect-details-view-tree]

</details>
</br>

<details>
<summary>2. LargeIcon view</summary>
</br>

![listview-virtualmode-inspect-largeicon-view-tree][listview-virtualmode-inspect-largeicon-view-tree]

</details>
</br>

<details>
<summary>3. List view</summary>
</br>

![listview-virtualmode-inspect-list-view-tree][listview-virtualmode-inspect-list-view-tree]

</details>
</br>

<details>
<summary>4. SmallIcon view</summary>
</br>

![listview-virtualmode-inspect-smallicon-view-tree][listview-virtualmode-inspect-smallicon-view-tree]

</details>
</br>

<details>
<summary>5. Tile view</summary>
</br>

The ListView in virtual mode does not support "Tile" view

</details>
</br>

## Accessibility tree for a ListView without ListViewGroups
`ListViewGroups` are not visible when `ListView` has no [groups](https://docs.microsoft.com/dotnet/api/system.windows.forms.listview.groups).

```cs
listView.ShowGroups = true;
listView.Columns.Add(new ColumnHeader { Text = "Column 1" });
listView.Columns.Add(new ColumnHeader { Text = "Column 2" });
listView.Columns.Add(new ColumnHeader { Text = "Column 3" });
listView.Columns.Add(new ColumnHeader { Text = "Column 4" });

listView.Items.Add(new ListViewItem(new string[] { "Item 1", "SubItem 11", "SubItem 12", "SubItem 13" }, imageIndex: 0));
listView.Items.Add(new ListViewItem(new string[] { "Item 2", "SubItem 21", "SubItem 22", "SubItem 23" }, imageIndex: 0));
listView.Items.Add(new ListViewItem(new string[] { "Item 3", "SubItem 31" }, imageIndex: 0));
```

<details>
<summary>1. Details view</summary>
</br>

![listview-withoutgroup-inspect-details-view-tree][listview-withoutgroup-inspect-details-view-tree]

</details>
</br>

<details>
<summary>2. LargeIcon view</summary>
</br>

![listview-withoutgroup-inspect-largeicon-view-tree][listview-withoutgroup-inspect-largeicon-view-tree]

</details>
</br>

<details>
<summary>3. List view</summary>
</br>

![listview-withoutgroup-inspect-list-view-tree][listview-withoutgroup-inspect-list-view-tree]

</details>
</br>

<details>
<summary>4. SmallIcon view</summary>
</br>

![listview-withoutgroup-inspect-smallicon-view-tree][listview-withoutgroup-inspect-smallicon-view-tree]

</details>
</br>

<details>
<summary>5. Tile view</summary>
</br>

![listview-withoutgroup-inspect-tile-view-tree][listview-withoutgroup-inspect-tile-view-tree]

</details>
</br>

# ListViewSubItem

## Accessibility tree for a ListViewItem
A [ListViewSubItem](https://docs.microsoft.com/dotnet/api/system.windows.forms.listviewitem) is display if the following conditions are met:
1. A `ListView` is in a `List` or `Details` [views](https://docs.microsoft.com/dotnet/api/system.windows.forms.listview.view).
2. A `ListView` has one or more [ListViewSubItems](https://docs.microsoft.com/dotnet/api/system.windows.forms.listviewitem.subitems)
3. A `ListView` has one or more [Columns](https://docs.microsoft.com/dotnet/api/system.windows.forms.listviewitem.subitems)
4. The `ListViewItem` has a [TileSize](https://docs.microsoft.com/dotnet/api/system.windows.forms.listview.tilesize) enough to display `ListViewSubItems` (only for `Tile` view)


```cs
listView.Columns.Add(new ColumnHeader { Text = "Column 1" });
listView.Columns.Add(new ColumnHeader { Text = "Column 2" });
listView.Columns.Add(new ColumnHeader { Text = "Column 3" });
listView.Columns.Add(new ColumnHeader { Text = "Column 4" });

listView.Items.Add(new ListViewItem(new string[] { "Item 1", "SubItem 11", "SubItem 12", "SubItem 13" }, imageIndex: 0));
listView.Items.Add(new ListViewItem(new string[] { "Item 2", "SubItem 21", "SubItem 22", "SubItem 23" }, imageIndex: 0));

listView.TileSize = new Size(100, 100);  
```

<details>
<summary>1. Details view</summary>
</br>

![listviewsubitem-inspect-details-view-tree][listviewsubitem-inspect-details-view-tree]

</details>
</br>

<details>
<summary>2. LargeIcon view</summary>
</br>

![listviewsubitem-inspect-largeicon-view-tree][listviewsubitem-inspect-largeicon-view-tree]
This view does not support ListViewSubItems

</details>
</br>

<details>
<summary>3. List view</summary>
</br>

![listviewsubitem-inspect-list-view-tree][listviewsubitem-inspect-list-view-tree]
This view does not support ListViewSubItems

</details>
</br>

<details>
<summary>4. SmallIcon view</summary>
</br>

![listviewsubitem-inspect-smallicon-view-tree][listviewsubitem-inspect-smallicon-view-tree]
This view does not support ListViewSubItems

</details>
</br>

<details>
<summary>5. Tile view</summary>
</br>

![listviewsubitem-inspect-tile-view-tree][listviewsubitem-inspect-tile-view-tree]

</details>
</br>

## Accessibility tree for a ListViewItem (Details view)

It is typical for this mode that when displaying accessibility objects for `ListViewSubItems` in the Inspect tree, the main parameter is not the number of [ListViewSubItems](https://docs.microsoft.com/dotnet/api/system.windows.forms.listviewitem.subitems), but the number of [Columns](https://docs.microsoft.com/dotnet/api/system.windows.forms.listview.columns) that the user can see. 

In the case when the number of `Columns` and `ListViewSubItems` is the same, the user sees that all `Columns` contain data and all `ListViewSubItems` are displayed in the Accessibility tree.

```cs
listView.Columns.Add(new ColumnHeader { Text = "Column 1" });
listView.Columns.Add(new ColumnHeader { Text = "Column 2" });
listView.Columns.Add(new ColumnHeader { Text = "Column 3" });
listView.Columns.Add(new ColumnHeader { Text = "Column 4" });

listView.Items.Add(new ListViewItem(new string[] { "Item 1", "SubItem 11", "SubItem 12", "SubItem 13" }, imageIndex: 0));
listView.Items.Add(new ListViewItem(new string[] { "Item 2", "SubItem 21", "SubItem 22", "SubItem 23" }, imageIndex: 0));
```

![listviewsubitem-inspect-details-view-tree][listviewsubitem-inspect-details-view-tree]

If the number of `Columns` is less than the number of `ListViewSubItems`, then the user sees only those `ListViewSubItems` for which there are `Columns`. As a result, only `ListViewSubItems` with matching `Columns` are displayed in the Inspector tree. `ListViewSubItems` without corresponding `Columns` on the screen and in the Accessibility tree are not displayed.

```cs
listView.Columns.Add(new ColumnHeader { Text = "Column 1" });
listView.Columns.Add(new ColumnHeader { Text = "Column 2" });

listView.Items.Add(new ListViewItem(new string[] { "Item 1", "SubItem 11", "SubItem 12", "SubItem 13" }, imageIndex: 0));
listView.Items.Add(new ListViewItem(new string[] { "Item 2", "SubItem 21", "SubItem 22", "SubItem 23" }, imageIndex: 0));
```

![listviewsubitem-twocolumns-inspect-details-view-tree][listviewsubitem-twocolumns-inspect-details-view-tree]

If the number of `Columns` is greater than the number of `ListViewSubItems`, then a fake accessibility object is displayed for non-existent `ListViewSubItems`. This is because whether they exist or not, the user sees the cell for the `ListViewSubItem`, so it should have an accessibility object to interact with the Accessibility tree.

```cs
listView.Columns.Add(new ColumnHeader { Text = "Column 1" });
listView.Columns.Add(new ColumnHeader { Text = "Column 2" });
listView.Columns.Add(new ColumnHeader { Text = "Column 3" });
listView.Columns.Add(new ColumnHeader { Text = "Column 4" });

listView.Items.Add(new ListViewItem(new string[] { "Item 1", "SubItem 11" }, imageIndex: 0));
listView.Items.Add(new ListViewItem(new string[] { "Item 2", "SubItem 21" }, imageIndex: 0));
```

![listviewsubitem-twosubitems-inspect-details-view-tree][listviewsubitem-twosubitems-inspect-details-view-tree]

## Accessibility tree for a ListViewItem (Tile view)
This view takes many factors into account when displaying `ListViewSubItems`.

The first `ListViewSubItem` (with 0 index) is never shown in the Accessibility tree, as its data is displayed within the `ListViewItem`. `ListViewItem.FragmentNavigate(NavigateDirection.FirstChild)`, `ListViewItem.FragmentNavigate(NavigateDirection.LastChild)` and
`ListViewSubItem.FragmentNavigate(NavigateDirection.PreviousSibling)` methods, also never return a 0-indexed `ListViewSubItem`

All [ListViewSubItem](https://docs.microsoft.com/dotnet/api/system.windows.forms.listviewitem) is display if the following conditions are met:
1. A `ListView` has one or more [ListViewSubItems](https://docs.microsoft.com/dotnet/api/system.windows.forms.listviewitem.subitems)
2. A `ListView` has one or more [Columns](https://docs.microsoft.com/dotnet/api/system.windows.forms.listviewitem.subitems)
3. The `ListViewItem` has a [TileSize](https://docs.microsoft.com/dotnet/api/system.windows.forms.listview.tilesize) enough to display `ListViewSubItems`

```cs
listView.Columns.Add(new ColumnHeader { Text = "Column 1" });
listView.Columns.Add(new ColumnHeader { Text = "Column 2" });
listView.Columns.Add(new ColumnHeader { Text = "Column 3" });
listView.Columns.Add(new ColumnHeader { Text = "Column 4" });

listView.Items.Add(new ListViewItem(new string[] { "Item 1", "SubItem 11", "SubItem 12", "SubItem 13" }, imageIndex: 0));
listView.Items.Add(new ListViewItem(new string[] { "Item 2", "SubItem 21", "SubItem 22", "SubItem 23" }, imageIndex: 0));

listView.TileSize = new Size(100, 100);  
```

![listviewsubitem-inspect-tile-view-tree][listviewsubitem-inspect-tile-view-tree]

If the number of `Columns` is less than the number of `ListViewSubItems`, then only `ListViewSubItems` with corresponding `Columns` are displayed. The rest of the `ListViewSubItems` are hidden both on the screen and in the Accessibility tree.

```cs
listView.Columns.Add(new ColumnHeader { Text = "Column 1" });
listView.Columns.Add(new ColumnHeader { Text = "Column 2" });

listView.Items.Add(new ListViewItem(new string[] { "Item 1", "SubItem 11", "SubItem 12", "SubItem 13" }, imageIndex: 0));
listView.Items.Add(new ListViewItem(new string[] { "Item 2", "SubItem 21", "SubItem 22", "SubItem 23" }, imageIndex: 0));

listView.TileSize = new Size(100, 100);  
```

![listviewsubitem-twocolumns-inspect-tile-view-tree][listviewsubitem-twocolumns-inspect-tile-view-tree]

If the number of `ListViewSubItems` is less than the number of `Columns`, then only existing `ListViewSubItems` are displayed in the Accessibility tree (as opposed to the `Details` view), since nonexistent `ListViewSubItems` are not displayed on the screen and the user cannot interact with them.

```cs
listView.Columns.Add(new ColumnHeader { Text = "Column 1" });
listView.Columns.Add(new ColumnHeader { Text = "Column 2" });
listView.Columns.Add(new ColumnHeader { Text = "Column 3" });
listView.Columns.Add(new ColumnHeader { Text = "Column 4" });

listView.Items.Add(new ListViewItem(new string[] { "Item 1", "SubItem 11", "SubItem 12" }, imageIndex: 0));
listView.Items.Add(new ListViewItem(new string[] { "Item 2", "SubItem 21", "SubItem 22" }, imageIndex: 0));

listView.TileSize = new Size(100, 100);  
```

![listviewsubitem-threeitems-inspect-tile-view-tree][listviewsubitem-threeitems-inspect-tile-view-tree]

If the [TileSize](https://docs.microsoft.com/dotnet/api/system.windows.forms.listview.tilesize) of the `ListViewItem` is too small, then only those `ListViewSubItems` that are included in the visible area of ​​the `ListViewItem` are displayed. The rest of the `ListViewSubItems` are hidden both on the screen and in the Accessibility tree.

```cs
listView.Columns.Add(new ColumnHeader { Text = "Column 1" });
listView.Columns.Add(new ColumnHeader { Text = "Column 2" });
listView.Columns.Add(new ColumnHeader { Text = "Column 3" });
listView.Columns.Add(new ColumnHeader { Text = "Column 4" });

listView.Items.Add(new ListViewItem(new string[] { "Item 1", "SubItem 11", "SubItem 12", "SubItem 13" }, imageIndex: 0));
listView.Items.Add(new ListViewItem(new string[] { "Item 2", "SubItem 21", "SubItem 22", "SubItem 23" }, imageIndex: 0));

listView.TileSize = new Size(100, 40);  
```

![listviewsubitem-smallsize-inspect-tile-view-tree][listviewsubitem-smallsize-inspect-tile-view-tree]

`ListViewSubItems` are not displayed when [visual styles](https://docs.microsoft.com/dotnet/api/system.windows.forms.application.enablevisualstyles) are disabled for an application. As a result, `ListViewSubItems` are also hidden in the Accessibility tree

```cs
listView.Columns.Add(new ColumnHeader { Text = "Column 1" });
listView.Columns.Add(new ColumnHeader { Text = "Column 2" });
listView.Columns.Add(new ColumnHeader { Text = "Column 3" });
listView.Columns.Add(new ColumnHeader { Text = "Column 4" });

listView.Items.Add(new ListViewItem(new string[] { "Item 1", "SubItem 11", "SubItem 12", "SubItem 13" }, imageIndex: 0));
listView.Items.Add(new ListViewItem(new string[] { "Item 2", "SubItem 21", "SubItem 22", "SubItem 23" }, imageIndex: 0));

listView.TileSize = new Size(100, 100);  
```

![listviewsubitem-disabledvisualstyles-inspect-tile-view-tree][listviewsubitem-disabledvisualstyles-inspect-tile-view-tree]

[listview-inspect-details-view-tree]: ../images/listview-inspect-details-view-tree.png
[listview-inspect-largeicon-view-tree]: ../images/listview-inspect-largeicon-view-tree.png
[listview-inspect-list-view-tree]: ../images/listview-inspect-list-view-tree.png
[listview-inspect-smallicon-view-tree]: ../images/listview-inspect-smallicon-view-tree.png
[listview-inspect-tile-view-tree]: ../images/listview-inspect-tile-view-tree.png
[listview-withgroup-inspect-details-view-tree]: ../images/listview-withgroup-inspect-details-view-tree.png
[listview-withgroup-inspect-largeicon-view-tree]: ../images/listview-withgroup-inspect-largeicon-view-tree.png
[listview-withgroup-inspect-list-view-tree]: ../images/listview-withgroup-inspect-list-view-tree.png
[listview-withgroup-inspect-smallicon-view-tree]: ../images/listview-withgroup-inspect-smallicon-view-tree.png
[listview-withgroup-inspect-tile-view-tree]: ../images/listview-withgroup-inspect-tile-view-tree.png
[listview-disabledstyles-inspect-details-view-tree]: ../images/listview-disabledstyles-inspect-details-view-tree.png
[listview-disabledstyles-inspect-largeicon-view-tree]: ../images/listview-disabledstyles-inspect-largeicon-view-tree.png
[listview-disabledstyles-inspect-list-view-tree]: ../images/listview-disabledstyles-inspect-list-view-tree.png
[listview-disabledstyles-inspect-smallicon-view-tree]: ../images/listview-disabledstyles-inspect-smallicon-view-tree.png
[listview-disabledstyles-inspect-tile-view-tree]: ../images/listview-disabledstyles-inspect-tile-view-tree.png
[listview-showgroupdisabled-inspect-details-view-tree]: ../images/listview-showgroupdisabled-inspect-details-view-tree.png
[listview-showgroupdisabled-inspect-largeicon-view-tree]: ../images/listview-showgroupdisabled-inspect-largeicon-view-tree.png
[listview-showgroupdisabled-inspect-list-view-tree]: ../images/listview-showgroupdisabled-inspect-list-view-tree.png
[listview-showgroupdisabled-inspect-smallicon-view-tree]: ../images/listview-showgroupdisabled-inspect-smallicon-view-tree.png
[listview-showgroupdisabled-inspect-tile-view-tree]: ../images/listview-showgroupdisabled-inspect-tile-view-tree.png
[listview-withoutgroup-inspect-details-view-tree]: ../images/listview-withoutgroup-inspect-details-view-tree.png
[listview-withoutgroup-inspect-largeicon-view-tree]: ../images/listview-withoutgroup-inspect-largeicon-view-tree.png
[listview-withoutgroup-inspect-list-view-tree]: ../images/listview-withoutgroup-inspect-list-view-tree.png
[listview-withoutgroup-inspect-smallicon-view-tree]: ../images/listview-withoutgroup-inspect-smallicon-view-tree.png
[listview-withoutgroup-inspect-tile-view-tree]: ../images/listview-withoutgroup-inspect-tile-view-tree.png
[listview-virtualmode-inspect-details-view-tree]: ../images/listview-virtualmode-inspect-details-view-tree.png
[listview-virtualmode-inspect-largeicon-view-tree]: ../images/listview-virtualmode-inspect-largeicon-view-tree.png
[listview-virtualmode-inspect-list-view-tree]: ../images/listview-virtualmode-inspect-list-view-tree.png
[listview-virtualmode-inspect-smallicon-view-tree]: ../images/listview-virtualmode-inspect-smallicon-view-tree.png
[listviewsubitem-inspect-details-view-tree]: ../images/listviewsubitem-inspect-details-view-tree.png
[listviewsubitem-inspect-largeicon-view-tree]: ../images/listviewsubitem-inspect-largeicon-view-tree.png
[listviewsubitem-inspect-list-view-tree]: ../images/listviewsubitem-inspect-list-view-tree.png
[listviewsubitem-inspect-smallicon-view-tree]: ../images/listviewsubitem-inspect-smallicon-view-tree.png
[listviewsubitem-inspect-tile-view-tree]: ../images/listviewsubitem-inspect-tile-view-tree.png
[listviewsubitem-twocolumns-inspect-details-view-tree]: ../images/listviewsubitem-twocolumns-inspect-details-view-tree.png
[listviewsubitem-twosubitems-inspect-details-view-tree]: ../images/listviewsubitem-twosubitems-inspect-details-view-tree.png
[listviewsubitem-twocolumns-inspect-tile-view-tree]: ../images/listviewsubitem-twocolumns-inspect-tile-view-tree.png
[listviewsubitem-threeitems-inspect-tile-view-tree]: ../images/listviewsubitem-threeitems-inspect-tile-view-tree.png
[listviewsubitem-smallsize-inspect-tile-view-tree]: ../images/listviewsubitem-smallsize-inspect-tile-view-tree.png
[listviewsubitem-disabledvisualstyles-inspect-tile-view-tree]: ../images/listviewsubitem-disabledvisualstyles-inspect-tile-view-tree.png