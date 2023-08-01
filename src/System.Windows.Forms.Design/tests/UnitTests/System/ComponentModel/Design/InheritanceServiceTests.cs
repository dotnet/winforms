// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.ComponentModel.Design.Tests;

public class InheritanceServicerTests
{
    [Fact]
    public void InheritanceService_Constructor()
    {
        InheritanceService underTest = new();
        Assert.NotNull(underTest);
    }
}
