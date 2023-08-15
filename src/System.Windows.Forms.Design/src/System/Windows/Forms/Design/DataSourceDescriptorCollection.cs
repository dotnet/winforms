// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;

namespace System.Windows.Forms.Design;

internal class DataSourceDescriptorCollection : CollectionBase
{
    public DataSourceDescriptorCollection() : base()
    {
    }

    public int Add(DataSourceDescriptor value)
    {
        return List.Add(value);
    }

    public int IndexOf(DataSourceDescriptor value)
    {
        return List.IndexOf(value);
    }

    public void Insert(int index, DataSourceDescriptor value)
    {
        List.Insert(index, value);
    }

    public bool Contains(DataSourceDescriptor value)
    {
        return List.Contains(value);
    }

    public void CopyTo(DataSourceDescriptor[] array, int index)
    {
        List.CopyTo(array, index);
    }

    public void Remove(DataSourceDescriptor value)
    {
        List.Remove(value);
    }

    public DataSourceDescriptor this[int index]
    {
        get
        {
            return (DataSourceDescriptor)List[index]!;
        }

        set
        {
            List[index] = value;
        }
    }
}
