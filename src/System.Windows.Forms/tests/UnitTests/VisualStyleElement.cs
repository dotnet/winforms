// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms.Tests
{
    public class VisualStyleElementTests
    {
        [Fact]
        public void CreateElement()
        {
            // arrange
            var className = "a";
            var part = 1;
            var state = 2;

            // act
            var vse = VisualStyleElement.CreateElement(className, part, state);

            // assert
            Assert.NotNull(vse);
            Assert.Equal(className, vse.ClassName);
            Assert.Equal(part, vse.Part);
            Assert.Equal(state, vse.State);
        }
    }
}
