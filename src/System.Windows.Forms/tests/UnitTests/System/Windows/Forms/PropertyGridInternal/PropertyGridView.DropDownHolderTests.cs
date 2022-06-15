// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using Xunit;

namespace System.Windows.Forms.PropertyGridInternal.Tests
{
    public class PropertyGridView_DropDownHolderTests
    {
        [WinFormsFact]
        public void DropDownHolder_AccessibilityObject_Constructor_initializes_correctly()
        {
            using PropertyGridView propertyGridView = new PropertyGridView(null, null);
            propertyGridView.BackColor = Color.Green;
            using PropertyGridView.DropDownHolder dropDownHolder = new PropertyGridView.DropDownHolder(propertyGridView);

            Assert.Equal(Color.Green, dropDownHolder.BackColor);
        }

        [WinFormsFact]
        public void DropDownHolder_SupportsUiaProviders_returns_true()
        {
            using PropertyGridView propertyGridView = new PropertyGridView(null, null);
            using PropertyGridView.DropDownHolder dropDownHolder = new PropertyGridView.DropDownHolder(propertyGridView);
            Assert.True(dropDownHolder.SupportsUiaProviders);
        }

        [WinFormsFact]
        public void DropDownHolder_CreateAccessibilityObject_creates_DropDownHolderAccessibleObject()
        {
            using PropertyGrid propertyGrid = new PropertyGrid();
            PropertyGridView propertyGridView = propertyGrid.TestAccessor().GridView;
            using PropertyGridView.DropDownHolder dropDownHolder = new PropertyGridView.DropDownHolder(propertyGridView);

            AccessibleObject accessibleObject = dropDownHolder.AccessibilityObject;
            Assert.Equal("DropDownHolderAccessibleObject", accessibleObject.GetType().Name);
        }
    }
}
