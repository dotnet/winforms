// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class MaskedTextBoxTests
    {
        [Fact]
        public void Constructor()
        {
            // act
            var mtb = new MaskedTextBox();

            // assert
            Assert.NotNull(mtb);
        }

        [Fact]
        public void ConstructorString()
        {
            // act
            var mtb = new MaskedTextBox("Hello World!");

            // assert
            Assert.NotNull(mtb);
        }

        [Fact]
        public void ConstructorMaskedTextProvider()
        {
            // act
            var mtb = new MaskedTextBox(new MaskedTextProvider("Hello World!"));

            // assert
            Assert.NotNull(mtb);
        }
    }
}
