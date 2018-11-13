﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    public class CheckedListBoxTests
    {
        [Fact]
        public void Constructor()
        {
            var box = new CheckedListBox();
            
            Assert.NotNull(box);
        }
    }
}
