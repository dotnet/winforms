// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

public class SplitContainer_SplitContainerTypedControlCollectionTests
{
    [WinFormsFact]
    public void SplitContainerTypedControlCollection_Ctor_Control()
    {
        using SplitContainer owner = new();
        SplitContainer.SplitContainerTypedControlCollection collection = new(owner, typeof(SplitterPanel), false);

        Assert.Empty(collection);
        Assert.False(collection.IsReadOnly);
        Assert.Same(owner, collection.Owner);
    }

    [WinFormsFact]
    public void SplitContainerTypedControlCollection_Ctor_NullOwner_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("owner", () => new SplitContainer.SplitContainerTypedControlCollection(null, typeof(SplitterPanel), false));
    }
}
