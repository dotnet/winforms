// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    using System.Drawing;

    public class PropertyGridTests
    {
        [Fact]
        public void PropertyGrid_Constructor()
        {
            var pg = new PropertyGrid();

            Assert.NotNull(pg);
            Assert.Equal(AutoScaleMode.None, pg.AutoScaleMode);
            Assert.Equal(0, pg.TabIndex);
            Assert.NotNull(pg.Controls);
            Assert.Equal(4, pg.Controls.Count);
            Assert.Equal(PropertySort.Categorized | PropertySort.Alphabetical, pg.PropertySort);
            Assert.Equal("PropertyGrid", pg.Text);
        }
    }
}
