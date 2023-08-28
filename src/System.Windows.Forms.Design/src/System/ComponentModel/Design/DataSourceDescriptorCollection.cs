// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;

namespace System.ComponentModel.Design;

public class DataSourceDescriptorCollection : CollectionBase
{
    public DataSourceDescriptorCollection() : base()
    {
    }

    public int Add(DataSourceDescriptor value) => List.Add(value);

    public int IndexOf(DataSourceDescriptor value) => List.IndexOf(value);

    public void Insert(int index, DataSourceDescriptor value) => List.Insert(index, value);

    public bool Contains(DataSourceDescriptor value) => List.Contains(value);

    public void CopyTo(DataSourceDescriptor[] array, int index) => List.CopyTo(array, index);

    public void Remove(DataSourceDescriptor value) => List.Remove(value);

    public DataSourceDescriptor? this[int index]
    {
        get => List[index] as DataSourceDescriptor;
        set => List[index] = value;
    }
}
