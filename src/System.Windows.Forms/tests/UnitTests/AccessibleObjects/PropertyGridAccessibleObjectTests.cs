// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Forms.PropertyGridInternal;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests.AccessibleObjects
{
    public class PropertyGridAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void PropertyGridAccessibleObject_Ctor_Default()
        {
            using PropertyGrid propertyGrid = new PropertyGrid();
            PropertyGridAccessibleObject accessibleObject = new PropertyGridAccessibleObject(propertyGrid);
            Assert.NotNull(accessibleObject.Owner);
            Assert.Equal(propertyGrid, accessibleObject.Owner);
        }

        [WinFormsTheory]
        [InlineData((int)UiaCore.UIA.TableItemPatternId)]
        [InlineData((int)UiaCore.UIA.GridItemPatternId)]
        public void GridEntryAccessibleObject_SupportsPattern(int pattern)
        {
            using PropertyGrid propertyGrid = new PropertyGrid();
            using ComboBox comboBox = new ComboBox();
            propertyGrid.SelectedObject = comboBox;
            GridEntry defaultGridEntry = propertyGrid.GetDefaultGridEntry();
            GridEntry parentGridEntry = defaultGridEntry.ParentGridEntry; // Category which has item pattern.
            AccessibleObject accessibleObject = parentGridEntry.AccessibilityObject;
            Assert.True(accessibleObject.IsPatternSupported((UiaCore.UIA)pattern));
        }

        [WinFormsTheory]
        [InlineData((int)UiaCore.UIA.GridPatternId)]
        [InlineData((int)UiaCore.UIA.TablePatternId)]
        public void PropertyGridAccessibleObject_SupportsPattern(int pattern)
        {
            using PropertyGrid propertyGrid = new PropertyGrid();
            propertyGrid.CreateControl();
            using ComboBox comboBox = new ComboBox();
            propertyGrid.SelectedObject = comboBox;
            PropertyGridAccessibleObject propertyGridAccessibleObject = new PropertyGridAccessibleObject(propertyGrid);

            // First child should be PropertyGrid toolbox.
            AccessibleObject firstChild = (AccessibleObject)propertyGridAccessibleObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild);

            // Second child entry should be PropertyGridView.
            AccessibleObject gridViewChild = (AccessibleObject)firstChild.FragmentNavigate(UiaCore.NavigateDirection.NextSibling);

            Assert.True(gridViewChild.IsPatternSupported((UiaCore.UIA)pattern));
        }
    }
}
