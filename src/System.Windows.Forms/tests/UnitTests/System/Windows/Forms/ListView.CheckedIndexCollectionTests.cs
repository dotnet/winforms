// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

public class ListView_CheckedIndexCollectionTests
{
    [WinFormsFact]
    public void CheckedIndexCollection_Ctor_OwnerIsNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("owner", () => { new ListView.CheckedIndexCollection(null); });
    }
}
