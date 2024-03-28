// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

namespace System.ComponentModel.Design;

/// <summary>
///  Edits an array of values.
/// </summary>
public class ArrayEditor : CollectionEditor
{
    /// <summary>
    ///  Initializes a new instance of <see cref="ArrayEditor"/>
    ///  using the specified type for the array.
    /// </summary>
    public ArrayEditor(Type type) : base(type)
    {
    }

    /// <summary>
    ///  Gets or sets the data type this collection contains.
    /// </summary>
    protected override Type CreateCollectionItemType()
        => CollectionType?.GetElementType();

    /// <summary>
    ///  Gets the items in the array.
    /// </summary>
    protected override object[] GetItems(object editValue)
    {
        if (editValue is Array valueArray)
        {
            object[] items = new object[valueArray.GetLength(0)];
            Array.Copy(valueArray, items, items.Length);
            return items;
        }

        return [];
    }

    /// <summary>
    ///  Sets the items in the array.
    /// </summary>
    protected override object SetItems(object editValue, object[] value)
    {
        if (editValue is not null and not Array)
        {
            return editValue;
        }

        if (value is null)
        {
            return null;
        }

        Array newArray = Array.CreateInstance(CollectionItemType, value.Length);
        Array.Copy(value, newArray, value.Length);
        return newArray;
    }
}
