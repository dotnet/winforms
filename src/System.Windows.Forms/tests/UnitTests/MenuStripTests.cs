// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using Moq;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class MenuStripTests
    {
        [Fact]
        public void MenuStrip_Constructor()
        {
            var ms = new MenuStrip();

            Assert.NotNull(ms);
            Assert.False(ms.CanOverflow);
            Assert.Equal(ToolStripGripStyle.Hidden, ms.GripStyle);
            Assert.True(ms.Stretch);
        }
    }
}
