﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    public class NumericUpDownTests
    {
        [Fact]
        public void Constructor()
        {
            // act
            var nud = new NumericUpDown();

            // assert
            Assert.NotNull(nud);
            Assert.Equal("0", nud.Text);
        }
    }
}
