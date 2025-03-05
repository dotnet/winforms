// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

namespace System.Windows.Forms.Tests;

public class Form_ControlCollection
{
    [WinFormsFact]
    public void ControlCollection_Ctor_Control()
    {
        using Form owner = new();
        Form.ControlCollection collection = new(owner);

        Assert.Empty(collection);
        Assert.False(collection.IsReadOnly);
        Assert.Same(owner, collection.Owner);
    }

    [WinFormsFact]
    public void ControlCollection_Ctor_NullOwner_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("owner", () => new Form.ControlCollection(null));
    }
}
