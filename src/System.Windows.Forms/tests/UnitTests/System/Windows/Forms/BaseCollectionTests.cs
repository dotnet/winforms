// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;

namespace System.Windows.Forms.Tests;

public class BaseCollectionTests
{
    [WinFormsFact]
    public void BaseCollection_Ctor_Default()
    {
        SubBaseCollection collection = new();
        Assert.Throws<NullReferenceException>(() => collection.Count);
        Assert.False(collection.IsReadOnly);
        Assert.False(collection.IsSynchronized);
        Assert.Null(collection.List);
        Assert.Same(collection, collection.SyncRoot);
    }

    [WinFormsFact]
    public void BaseCollection_Count_GetWithList_ReturnsExpected()
    {
        CustomBaseCollection collection = new([1, 2, 3]);
        Assert.Equal(3, collection.Count);
    }

    [WinFormsFact]
    public void BaseCollection_CopyTo_InvokeWithList_Success()
    {
        CustomBaseCollection collection = new([1, 2, 3]);
        object[] array = [0, 0, 0, 0, 4];
        collection.CopyTo(array, 1);
        Assert.Equal([0, 1, 2, 3, 4], array);
    }

    [WinFormsFact]
    public void BaseCollection_CopyTo_InvokeDefault_ThrowsNullReferenceException()
    {
        BaseCollection collection = new();
        Assert.Throws<NullReferenceException>(() => collection.CopyTo(Array.Empty<object>(), 0));
    }

    [WinFormsFact]
    public void BaseCollection_GetEnumerator_InvokeWithList_Success()
    {
        CustomBaseCollection collection = new([1, 2, 3]);
        IEnumerator enumerator = collection.GetEnumerator();
        Assert.True(enumerator.MoveNext());
        Assert.Equal(1, enumerator.Current);
    }

    [WinFormsFact]
    public void BaseCollection_GetEnumerator_InvokeDefault_ThrowsNullReferenceException()
    {
        BaseCollection collection = new();
        Assert.Throws<NullReferenceException>(() => collection.CopyTo(Array.Empty<object>(), 0));
    }

    private class CustomBaseCollection : BaseCollection
    {
        private readonly ArrayList _list;

        public CustomBaseCollection(ArrayList list) => _list = list;

        protected override ArrayList List => _list;
    }

    private class SubBaseCollection : BaseCollection
    {
        public new ArrayList List => base.List;
    }
}
