// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests.AccessibleObjects
{
    public class PropertyGridViewAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [Theory]
        [InlineData((int)UiaCore.UIA.GridPatternId)]
        [InlineData((int)UiaCore.UIA.TablePatternId)]
        public void PropertyGridAccessibleObject_Supports_TablePattern(int pattern)
        {
            PropertyGrid propertyGrid = new PropertyGrid();
            ComboBox comboBox = new ComboBox();
            propertyGrid.SelectedObject = comboBox;

            var propertyGridAccessibleObject = new PropertyGridAccessibleObject(propertyGrid);

            // First child should be PropertyGrid toolbox.
            var firstChild = propertyGridAccessibleObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild) as AccessibleObject;

            // Second child entry should be PropertyGridView.
            var gridViewChild = firstChild.FragmentNavigate(UiaCore.NavigateDirection.NextSibling) as AccessibleObject;

            Assert.True(gridViewChild.IsPatternSupported(((UiaCore.UIA)pattern)));
        }

        [Theory]
        [InlineData((int)UiaCore.UIA.GridItemPatternId)]
        [InlineData((int)UiaCore.UIA.TableItemPatternId)]
        public void PropertyGridViewAccessibleObject_Supports_TablePattern(int pattern)
        {
            PropertyGrid propertyGrid = new PropertyGrid();
            ComboBox comboBox = new ComboBox();
            propertyGrid.SelectedObject = comboBox;

            var defaultGridEntry = propertyGrid.GetDefaultGridEntry();
            var parentGridEntry = defaultGridEntry.ParentGridEntry; // Category which has item pattern.
            var accessibleObject = parentGridEntry.AccessibilityObject;
            Assert.True(accessibleObject.IsPatternSupported((UiaCore.UIA)pattern));
        }
    }
}
