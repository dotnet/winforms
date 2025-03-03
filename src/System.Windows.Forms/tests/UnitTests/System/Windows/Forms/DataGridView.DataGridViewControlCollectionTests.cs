// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

namespace System.Windows.Forms.Tests;

public class DataGridView_DataGridViewControlCollectionTests
{
    [WinFormsFact]
    public void DataGridViewControlCollection_Ctor_Control()
    {
        using DataGridView owner = new();
        DataGridView.DataGridViewControlCollection collection = new(owner);

        Assert.Empty(collection);
        Assert.False(collection.IsReadOnly);
        Assert.Same(owner, collection.Owner);
    }

    [WinFormsFact]
    public void DataGridViewControlCollection_Ctor_NullOwner_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("owner", () => new DataGridView.DataGridViewControlCollection(null));
    }
}
