// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ToolStripTests
    {
        [Fact]
        public void ToolStrip_Constructor()
        {
            var ts = new ToolStrip();

            Assert.NotNull(ts);
            Assert.False(ts.ImageScalingSize.IsEmpty);
            Assert.Equal(16, ts.ImageScalingSize.Width);
            Assert.Equal(16, ts.ImageScalingSize.Height);
            Assert.True(ts.CanOverflow);
            Assert.False(ts.TabStop);
            Assert.False(ts.MenuAutoExpand);
            Assert.NotNull(ToolStripManager.ToolStrips);
            Assert.True(ToolStripManager.ToolStrips.Contains(ts));            
            Assert.True(ts.AutoSize);
            Assert.False(ts.CausesValidation);
            Assert.Equal(100, ts.Size.Width);
            Assert.Equal(25, ts.Size.Height);
            Assert.True(ts.ShowItemToolTips);
        }

        [Fact]
        public void ToolStrip_ConstructorItems()
        {
            var button = new ToolStripButton();
            var items = new ToolStripItem[1] { button };

            var ts = new ToolStrip(items);

            Assert.NotNull(ts);
            Assert.NotNull(ts.Items);
            Assert.Single(ts.Items);
            Assert.Equal(button, ts.Items[0]);
        }
    }
}
