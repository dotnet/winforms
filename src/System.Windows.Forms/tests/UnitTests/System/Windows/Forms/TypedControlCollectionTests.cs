// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

public class TypedControlCollectionTests
{
    [WinFormsFact]
    public void TypedControlCollection_Ctor_Control()
    {
        using Panel owner = new();
        TypedControlCollection collection = new(owner, typeof(Panel), false);

        Assert.Empty(collection);
        Assert.False(collection.IsReadOnly);
        Assert.Same(owner, collection.Owner);
    }

    [WinFormsFact]
    public void TypedControlCollection_Ctor_NullOwner_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("owner", () => new TypedControlCollection(null, typeof(Panel), false));
    }
}
