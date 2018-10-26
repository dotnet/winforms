﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    public class PictureBoxTests
    {
        [Fact]
        public void Constructor()
        {
            // act
            var pb = new PictureBox();

            // assert
            Assert.NotNull(pb);
            Assert.False(pb.TabStop);
        }
    }
}
