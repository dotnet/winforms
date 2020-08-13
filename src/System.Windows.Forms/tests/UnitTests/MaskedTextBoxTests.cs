// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class MaskedTextBoxTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void MaskedTextBox_Constructor()
        {
            using var mtb = new MaskedTextBox();

            Assert.NotNull(mtb);
        }

        [WinFormsFact]
        public void MaskedTextBox_ConstructorString()
        {
            using var mtb = new MaskedTextBox("Hello World!");

            Assert.NotNull(mtb);
        }

        [WinFormsFact]
        public void MaskedTextBox_ConstructorMaskedTextProvider()
        {
            using var mtb = new MaskedTextBox(new MaskedTextProvider("Hello World!"));

            Assert.NotNull(mtb);
        }
    }
}
