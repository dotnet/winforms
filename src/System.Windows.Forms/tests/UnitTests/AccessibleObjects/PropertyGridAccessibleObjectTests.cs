// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests.AccessibleObjects
{
    public class PropertyGridAccessibleObjectTests
    {
        [Fact]
        public void PropertyGridAccessibleObject_Ctor_Default()
        {
            PropertyGrid propertyGrid = new PropertyGrid();

            var accessibleObject = new PropertyGridAccessibleObject(propertyGrid);
            Assert.NotNull(accessibleObject.Owner);
            Assert.Equal(propertyGrid, accessibleObject.Owner);
        }
    }
}
