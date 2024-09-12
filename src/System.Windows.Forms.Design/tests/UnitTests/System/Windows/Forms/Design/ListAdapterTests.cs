// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Collections;

namespace System.Windows.Forms.Design.Tests;

public class ListAdapterTests
{
    [Fact]
    public void ListAdapter_Unwrap_ReturnsOriginalList()
    {
        ArrayList originalList = [1, 2, 3];
        ListAdapter<int> adapter = new(originalList);

        IList unwrappedList = adapter.Unwrap();

        unwrappedList.Should().BeSameAs(originalList);
    }
}
