// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;

namespace System.Windows.Forms.Design;

/// <summary>
///  A strongly-typed collection that stores ToolStripMenuItem objects for DesignerContextMenu
/// </summary>
internal class CustomMenuItemCollection : CollectionBase
{
    /// <summary>
    ///  Constructor
    /// </summary>
    public CustomMenuItemCollection()
    {
    }

    /// <summary>
    ///  Add value to the collection
    /// </summary>
    public int Add(ToolStripItem value)
    {
        return List.Add(value);
    }

    /// <summary>
    ///  Add range of values to the collection
    /// </summary>
    public void AddRange(params ToolStripItem[] value)
    {
        for (int i = 0; (i < value.Length); i++)
        {
            Add(value[i]);
        }
    }

    /// <summary>
    ///  Abstract base class version for refreshing the items
    /// </summary>
    public virtual void RefreshItems()
    {
    }
}
