// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;

namespace System.ComponentModel.Design;

public class DataSourceGroupCollection : CollectionBase
{
    public DataSourceGroupCollection() : base()
    {
    }

    public int Add(DataSourceGroup value) => List.Add(value);

    public int IndexOf(DataSourceGroup value) => List.IndexOf(value);

    public void Insert(int index, DataSourceGroup value) => List.Insert(index, value);

    public bool Contains(DataSourceGroup value) => List.Contains(value);

    public void CopyTo(DataSourceGroup[] array, int index) => List.CopyTo(array, index);

    public void Remove(DataSourceGroup value) => List.Remove(value);

    public DataSourceGroup? this[int index]
    {
        get => List[index] as DataSourceGroup;

        set => List[index] = value;
    }
}
