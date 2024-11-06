// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

public class ReadOnlyControlCollectionTests
{
    [WinFormsFact]
    public void ReadOnlyControlCollection_Ctor_Control()
    {
        using Control owner = new();
        ReadOnlyControlCollection collection = new(owner, false);

        Assert.Empty(collection);
        Assert.False(collection.IsReadOnly);
        Assert.Same(owner, collection.Owner);
    }

    [WinFormsFact]
    public void ReadOnlyControlCollection_Ctor_NullOwner_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("owner", () => new ReadOnlyControlCollection(null, false));
    }
}
