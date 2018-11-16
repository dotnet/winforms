// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    public class StatusStripTests
    {
        [Fact]
        public void StatusStrip_Constructor()
        {
            var ss = new StatusStrip();

            Assert.NotNull(ss);
            Assert.False(ss.CanOverflow);
            Assert.Equal(ToolStripLayoutStyle.Table, ss.LayoutStyle);
            Assert.Equal(ToolStripRenderMode.System, ss.RenderMode);
            Assert.Equal(ToolStripGripStyle.Hidden, ss.GripStyle);
            Assert.True(ss.Stretch);
        }
    }
}
