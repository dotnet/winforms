// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests.PropertyGridInternal.Tests
{
    public class PropertyGridViewTests
    {
        [WinFormsFact]
        public void PropertyGridView_Constructor()
        {
            var propertyGrid = new PropertyGrid();
            var entity = new Entity();
            propertyGrid.SelectedObject = entity;

            var item = propertyGrid.SelectedGridItem;

            Assert.NotNull(item);
            Assert.NotNull(item.Value);
        }
    }

    public class Entity
    {
        string[] Values { get; set; }
    }
}
