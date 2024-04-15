// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace System.Windows.Forms.Design;

/// <summary>
///  Provides an editor for a ListView subitems collection.
/// </summary>
internal class ListViewSubItemCollectionEditor : CollectionEditor
{
    private static int s_count;
    private ListViewItem.ListViewSubItem _firstSubItem;

    /// <summary>
    ///  Initializes a new instance of the <see cref="ListViewSubItemCollectionEditor"/> class.
    /// </summary>
    public ListViewSubItemCollectionEditor(Type type) : base(type)
    { }

    /// <inheritdoc />
    protected override object CreateInstance(Type type)
    {
        object instance = base.CreateInstance(type);

        // Create a default site-like name.
        if (instance is ListViewItem.ListViewSubItem item)
        {
            item.Name = SR.ListViewSubItemBaseName + s_count++;
        }

        return instance;
    }

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

    protected override object[] GetItems(object editValue)
    {
        var subItems = (ListViewItem.ListViewSubItemCollection)editValue;

        if (subItems.Count == 0)
        {
            return [];
        }

        // Save the first sub item.
        _firstSubItem = subItems[0];

        if (subItems.Count == 1)
        {
            return [];
        }

        object[] values = new object[subItems.Count - 1];
        int index = 0;
        IEnumerator enumerator = subItems.GetEnumerator();
        enumerator.MoveNext();

        while (enumerator.MoveNext())
        {
            values[index++] = enumerator.Current;
        }

        return values;
    }

    protected override object SetItems(object editValue, object[] value)
    {
        IList list = editValue as IList;
        list.Clear();

        list.Add(_firstSubItem);

        for (int i = 0; i < value.Length; i++)
        {
            list.Add(value[i]);
        }

        return editValue;
    }
}
