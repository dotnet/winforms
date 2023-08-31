// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;

namespace System.Windows.Forms.Design;

/// <summary>
///  Provides an editor for a ListView groups collection.
/// </summary>
internal class ListViewGroupCollectionEditor : CollectionEditor
{
    private object _editValue;

    public ListViewGroupCollectionEditor(Type type) : base(type)
    { }

    /// <summary>
    ///  Creates a ListViewGroup instance.
    /// </summary>
    protected override object CreateInstance(Type itemType)
    {
        ListViewGroup group = (ListViewGroup)base.CreateInstance(itemType);

        // Create an unique name for the list view group.
        group.Name = CreateListViewGroupName((ListViewGroupCollection)_editValue);

        return group;
    }

    private string CreateListViewGroupName(ListViewGroupCollection collection)
    {
        ReadOnlySpan<char> listViewGroupName = nameof(ListViewGroup);

        if (Context.TryGetService(out INameCreationService nameService)
            && Context.TryGetService(out IContainer container))
        {
            listViewGroupName = nameService.CreateName(container, typeof(ListViewGroup));
        }

        // Strip the digits from the end.
        while (char.IsDigit(listViewGroupName[^1]))
        {
            listViewGroupName = listViewGroupName[0..^1];
        }

        int i = 1;
        string result = $"{listViewGroupName}{i}";

        while (collection[result] is not null)
        {
            i++;
            result = $"{listViewGroupName}{i}";
        }

        return result;
    }

    public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
    {
        _editValue = value;
        object result;

        // This will block while the ListViewGroupCollectionDialog is running.
        result = base.EditValue(context, provider, value);

        // The user is done with the ListViewGroupCollectionDialog, don't need the edit value any longer.
        _editValue = null;

        return result;
    }
}
