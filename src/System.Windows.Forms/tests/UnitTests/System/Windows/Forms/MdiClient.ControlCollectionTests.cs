// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

namespace System.Windows.Forms.Tests;

public class MdiClient_ControlCollectionTests
{
    [WinFormsFact]
    public void ControlCollection_Ctor_Control()
    {
        using MdiClient owner = new();
        MdiClient.ControlCollection collection = new(owner);

        Assert.Empty(collection);
        Assert.False(collection.IsReadOnly);
        Assert.Same(owner, collection.Owner);
    }

    [WinFormsFact]
    public void ControlCollection_Ctor_NullOwner_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("owner", () => new MdiClient.ControlCollection(null));
    }
}
