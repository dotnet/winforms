// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Reflection;

namespace System.Windows.Forms.PropertyGridInternal.Tests;

public class PropertyGridView_GridViewListBoxTests
{
    [WinFormsFact]
    public void GridViewListBoxAccessibleObject_checks_arguments()
    {
        using PropertyGrid propertyGrid = new PropertyGrid
        {
            SelectedObject = Size.Empty
        };
        propertyGrid.CreateControl();

        PropertyGridView propertyGridView = propertyGrid.TestAccessor().GridView;

        using PropertyGridView.GridViewListBox gridViewListBox = new(propertyGridView);
        AccessibleObject gridViewListBoxAccessibleObject = gridViewListBox.AccessibilityObject;
        Assert.NotNull(gridViewListBoxAccessibleObject);

        Assert.Throws<ArgumentNullException>(() =>
        {
            Type gridViewListBoxAccessibleObjectType = gridViewListBoxAccessibleObject.GetType();
            Assert.Equal("GridViewListBoxAccessibleObject", gridViewListBoxAccessibleObjectType.Name);
            ConstructorInfo constructorInfo = gridViewListBoxAccessibleObjectType.GetConstructors()[0];

            using PropertyGridView.GridViewListBox owningGridViewListBox = new(null);
            constructorInfo.Invoke([owningGridViewListBox]);
        });
    }
}
