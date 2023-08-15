// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;

namespace System.Windows.Forms.Design;

internal class DataSourceGroupCollection : CollectionBase
{
    public DataSourceGroupCollection() : base()
    {
    }

    public int Add(DataSourceGroup value)
    {
        return List.Add(value);
    }

    public int IndexOf(DataSourceGroup value)
    {
        return List.IndexOf(value);
    }

    public void Insert(int index, DataSourceGroup value)
    {
        List.Insert(index, value);
    }

    public bool Contains(DataSourceGroup value)
    {
        return List.Contains(value);
    }

    public void CopyTo(DataSourceGroup[] array, int index)
    {
        List.CopyTo(array, index);
    }

    public void Remove(DataSourceGroup value)
    {
        List.Remove(value);
    }

    public DataSourceGroup this[int index]
    {
        get => (DataSourceGroup)List[index]!;

        set => List[index] = value;
    }
}
