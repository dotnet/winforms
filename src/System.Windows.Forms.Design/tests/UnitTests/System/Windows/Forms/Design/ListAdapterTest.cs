// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Collections;

namespace System.Windows.Forms.Design.Tests;

public class ListAdapterTest
{
    [Fact]
    public void ListAdapter_Unwrap_ReturnsOriginalList()
    {
        // Arrange
        var originalList = new ArrayList { 1, 2, 3 };
        ListAdapter<int> adapter = new(originalList);

        // Act
        var unwrappedList = adapter.Unwrap();

        // Assert
        Assert.Same(originalList, unwrappedList);
    }
}


