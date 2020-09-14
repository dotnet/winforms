// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using WinForms.Common.Tests;

namespace System.Windows.Forms.PropertyGridInternal.Tests
{
    public class PropertyGridViewTests
    {
        [WinFormsFact]
        public void PropertyGridView_created_for_PropertyGrid()
        {
            using PropertyGrid propertyGrid = new PropertyGrid();
            PropertyGridView propertyGridView = propertyGrid.TestAccessor().GridView;

            Assert.NotNull(propertyGridView);
        }
    }
}
