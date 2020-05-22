// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using System.Windows.Forms.PropertyGridInternal;
using Moq;
using Xunit;

namespace System.Windows.Forms.Tests.PropertyGridInternal.Tests
{
    public class GridViewListBoxAccessibleObjectTests
    {
        [WinFormsFact]
        public void GridViewListBoxAccessibleObject_DoesNotThrowException_OnFocus()
        {
            Mock<IServiceProvider> mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            PropertyGrid propertyGrid = new PropertyGrid();

            PropertyGridView propertyGridView = new PropertyGridView(mockServiceProvider.Object, propertyGrid);
            var dropDownListBoxAccessibleObject = propertyGridView.DropDownListBoxAccessibleObject;

            Type type = dropDownListBoxAccessibleObject.GetType();

            var controlAccessibleObject = (Control.ControlAccessibleObject)dropDownListBoxAccessibleObject;
            var dropDownListBox = (ListBox)controlAccessibleObject.Owner;
            Assert.Null(dropDownListBox.SelectedItem);

            // Verify that invoking SetListBoxItemFocus does not lead to exception throwing even if SelectedItem = null.
            type.InvokeMember("SetListBoxItemFocus",
                BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod,
                null, dropDownListBoxAccessibleObject, Array.Empty<Object>());
        }
    }
}
