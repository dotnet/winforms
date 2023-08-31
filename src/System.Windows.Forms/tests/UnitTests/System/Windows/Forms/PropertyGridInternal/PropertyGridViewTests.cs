// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.PropertyGridInternal.Tests;

public partial class PropertyGridViewTests
{
    [WinFormsFact]
    public void PropertyGridView_Ctor_Default()
    {
        using PropertyGrid propertyGrid = new();
        PropertyGridView propertyGridView = propertyGrid.TestAccessor().GridView;

        // TODO: validate properties

        Assert.NotNull(propertyGridView);
    }
}
