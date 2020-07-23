// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using Xunit;
using System.Reflection;

namespace System.Windows.Forms.PropertyGridInternal.Tests
{
    public class GridViewListBoxTests
    {
        [WinFormsFact]
        public void GridViewListBoxAccessibleObject_checks_arguments()
        {
            using var form = new Form();
            using var propertyGrid = new PropertyGrid();
            using var button = new Button();

            propertyGrid.SelectedObject = button;
            form.Controls.Add(propertyGrid);
            form.Controls.Add(button);

            using PropertyGridView propertyGridView = propertyGrid.TestAccessor().Dynamic._gridView as PropertyGridView;

            using var gridViewListBox = new PropertyGridView.GridViewListBox(propertyGridView);
            var gridViewListBoxAccessibleObject = gridViewListBox.AccessibilityObject;
            Assert.NotNull(gridViewListBoxAccessibleObject);

            Type gridViewListBoxAccessibleObjectType = gridViewListBoxAccessibleObject.GetType();
            Assert.Equal("GridViewListBoxAccessibleObject", gridViewListBoxAccessibleObjectType.Name);

            Assert.Throws<ArgumentNullException>(() =>
            {
                ConstructorInfo constructorInfo = gridViewListBoxAccessibleObjectType.GetConstructors()[0];

                PropertyGridView.GridViewListBox owningGridViewListBox = new PropertyGridView.GridViewListBox(null);
                constructorInfo.Invoke(new object[] { owningGridViewListBox });
            });
        }
    }
}
