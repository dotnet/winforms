﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ToolStripDropDownMenuTests
    {
        [Fact]
        public void Constructor()
        {
            var menu = new ToolStripDropDownMenu();
            
            Assert.NotNull(menu);
        }

        [Fact]
        public void ConstructorOwnerItemBool()
        {
            var owner = new ToolStripButton();
            var isAutoGenerated = true;
            
            var menu = new ToolStripDropDownMenu(owner, isAutoGenerated);
            
            Assert.NotNull(menu);
            Assert.Equal(owner, menu.OwnerItem);
            Assert.True(menu.IsAutoGenerated);
        }
    }
}
