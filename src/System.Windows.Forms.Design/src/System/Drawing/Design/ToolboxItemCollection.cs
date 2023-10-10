// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;

namespace System.Drawing.Design;

/// <summary>
///  A collection that stores <see cref="ToolboxItem"/> objects.
/// </summary>
public sealed class ToolboxItemCollection : ReadOnlyCollectionBase
{
    /// <summary>
    ///  Initializes a new instance of <see cref="ToolboxItemCollection"/> based on another <see cref="ToolboxItemCollection"/>.
    /// </summary>
    public ToolboxItemCollection(ToolboxItemCollection value)
    {
        InnerList.AddRange(value);
    }

    /// <summary>
    ///  Initializes a new instance of <see cref="ToolboxItemCollection"/> containing any array of <see cref="ToolboxItem"/> objects.
    /// </summary>
    public ToolboxItemCollection(ToolboxItem[] value)
    {
        InnerList.AddRange(value);
    }

    /// <summary>
    ///  Represents the entry at the specified index of the <see cref="ToolboxItem"/>.
    /// </summary>
    public ToolboxItem this[int index] => (ToolboxItem)InnerList[index]!;

    /// <summary>
    ///  Gets a value indicating whether the
    /// <see cref="ToolboxItemCollection"/> contains the specified <see cref="ToolboxItem"/>.
    /// </summary>
    public bool Contains(ToolboxItem value) => InnerList.Contains(value);

    /// <summary>
    ///  Copies the <see cref="ToolboxItemCollection"/> values to a one-dimensional <see cref="Array"/> instance at the
    ///  specified index.
    /// </summary>
    public void CopyTo(ToolboxItem[] array, int index)
    {
        InnerList.CopyTo(array, index);
    }

    /// <summary>
    ///  Returns the index of a <see cref="ToolboxItem"/> in
    ///  the <see cref="ToolboxItemCollection"/> .
    /// </summary>
    public int IndexOf(ToolboxItem value) => InnerList.IndexOf(value);
}
