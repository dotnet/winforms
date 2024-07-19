// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;

namespace System.Windows.Forms.Design;

internal class ColumnHeaderCollectionEditor : CollectionEditor
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ImageCollectionEditor"/> class.
    /// </summary>
    public ColumnHeaderCollectionEditor(Type type)
        : base(type)
    {
    }

    /// <summary>
    /// Gets the help topic to display for the dialog help button or pressing F1. Override to display a different help topic.
    /// </summary>
    protected override string HelpTopic
    {
        get => "net.ComponentModel.ColumnHeaderCollectionEditor";
    }

    /// <summary>
    /// Sets the specified collection to have the specified array of items.
    /// </summary>
    protected override object? SetItems(object? editValue, object[]? value)
    {
        if (editValue is ListView.ColumnHeaderCollection list)
        {
            list.Clear();
            if (value is not null)
            {
                ColumnHeader[] colHeaders = new ColumnHeader[value.Length];
                Array.Copy(value, 0, colHeaders, 0, value.Length);
                list.AddRange(colHeaders);
            }
        }

        return editValue;
    }

    /// <summary>
    ///  Removes the item from listview column header collection
    /// </summary>
    internal override void OnItemRemoving(object? item)
    {
        if (Context?.Instance is not ListView listview)
        {
            return;
        }

        if (item is ColumnHeader column)
        {
            IComponentChangeService? changeService = Context.GetService<IComponentChangeService>();
            PropertyDescriptor? property = null;
            if (changeService is not null)
            {
                property = TypeDescriptor.GetProperties(Context.Instance)["Columns"];
                changeService.OnComponentChanging(Context.Instance, property);
            }

            listview.Columns.Remove(column);

            if (changeService is not null && property is not null)
            {
                changeService.OnComponentChanged(Context.Instance, property);
            }
        }
    }
}
