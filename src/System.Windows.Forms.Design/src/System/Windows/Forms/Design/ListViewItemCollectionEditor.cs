// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.ComponentModel;
using System.ComponentModel.Design;

namespace System.Windows.Forms.Design;

/// <summary>
///  Provides an editor for a ListView items collection.
/// </summary>
internal class ListViewItemCollectionEditor : CollectionEditor
{
    /// <summary>
    ///  Initializes a new instance of the <see cref="ListViewItemCollectionEditor"/> class.
    /// </summary>
    public ListViewItemCollectionEditor(Type type) : base(type)
    { }

    /// <inheritdoc />
    protected override string GetDisplayText(object value)
    {
        if (value is null)
        {
            return string.Empty;
        }

        string text;

        PropertyDescriptor property = TypeDescriptor.GetDefaultProperty(CollectionType);
        if (property?.PropertyType == typeof(string))
        {
            text = (string)property.GetValue(value);

            if (!string.IsNullOrEmpty(text))
            {
                return text;
            }
        }

        text = TypeDescriptor.GetConverter(value).ConvertToString(value);

        if (string.IsNullOrEmpty(text))
        {
            text = value.GetType().Name;
        }

        return text;
    }

    protected override object SetItems(object editValue, object[] value)
    {
        // The base implementation clears the collection and re-adds the items. Clearing unhosts each item
        // from the ListView, which removes it from its group when the ListView isn't in design mode
        // (e.g., a ListView shown in a PropertyGrid at run-time). Remember the groups to restore them.

        ListViewGroup[] groups = null;

        if (value is not null)
        {
            groups = new ListViewGroup[value.Length];
            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] is ListViewItem item)
                {
                    groups[i] = item.Group;
                }
            }
        }

        List<(ListViewItem Item, ListViewGroup Group)> previousGroups = [];
        if (editValue is ListView.ListViewItemCollection items)
        {
            foreach (ListViewItem item in items)
            {
                if (item.Group is { } group)
                {
                    previousGroups.Add((item, group));
                }
            }
        }

        object result = base.SetItems(editValue, value);

        // Unhosting can clear Group without removing the item from the group's collection.
        // Clean up all original items, including those removed by the editor.
        foreach ((ListViewItem item, ListViewGroup group) in previousGroups)
        {
            if (item.Group != group)
            {
                group.Items.Remove(item);
            }
        }

        if (groups is not null)
        {
            for (int i = 0; i < value.Length; i++)
            {
                if (groups[i] is { } group
                    && value[i] is ListViewItem item
                    && item.Group != group
                    && item.ListView == group.ListView)
                {
                    item.Group = group;
                }
            }
        }

        return result;
    }
}
