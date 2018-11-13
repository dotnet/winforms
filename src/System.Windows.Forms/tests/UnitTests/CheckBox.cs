// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using System.Drawing;

namespace System.Windows.Forms.Tests
{
    public class CheckBoxTests
    {
        [Fact]
        public void Constructor()
        {
            // act
            var box = new CheckBox();

            // assert
            Assert.NotNull(box);
            Assert.True(box.AutoCheck);
            Assert.Equal(ContentAlignment.MiddleLeft, box.TextAlign);
        }
    }
}
