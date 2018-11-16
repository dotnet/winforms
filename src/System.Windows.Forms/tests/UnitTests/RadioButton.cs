// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    using System.Drawing;

    public class RadioButtonTests
    {
        [Fact]
        public void RadioButton_Constructor()
        {
            var button = new RadioButton();

            Assert.NotNull(button);
            Assert.Equal(ContentAlignment.MiddleLeft, button.TextAlign);
            Assert.False(button.TabStop);
        }
    }
}
