// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Design;

/// <summary>
///  A collection editor that is specifically designed to edit arrays containing strings.
/// </summary>
internal class StringArrayEditor : StringCollectionEditor
{
    public StringArrayEditor(Type type)
        : base(type)
    {
    }

    protected override Type CreateCollectionItemType() => CollectionType.GetElementType() ?? typeof(string[]);

    /// <summary>
    ///  We implement the getting and setting of items on this collection.
    /// </summary>
    protected override object[] GetItems(object? editValue)
    {
        if (editValue is not Array valueArray)
        {
            return [];
        }

        object[] items = new object[valueArray.GetLength(0)];
        Array.Copy(valueArray, items, items.Length);
        return items;
    }

    /// <summary>
    ///  We implement the getting and setting of items on this collection.
    ///  It should return an instance to replace <paramref name="editValue"/> with, or
    ///  <paramref name="editValue"/> if there is no need to replace the instance.
    /// </summary>
    protected override object? SetItems(object? editValue, object[]? value)
    {
        if (editValue is Array or null && value is not null)
        {
            Array newArray = Array.CreateInstance(CollectionItemType, value.Length);
            Array.Copy(value, newArray, value.Length);
            return newArray;
        }

        return editValue;
    }
}
