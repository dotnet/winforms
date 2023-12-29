// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class DataGridViewBindingCompleteEventArgsTests
{
    [Theory]
    [InlineData(ListChangedType.ItemAdded)]
    [InlineData(ListChangedType.Reset - 1)]
    public void Ctor_ListChangedType(ListChangedType listChangedType)
    {
        DataGridViewBindingCompleteEventArgs e = new(listChangedType);
        Assert.Equal(listChangedType, e.ListChangedType);
    }
}
