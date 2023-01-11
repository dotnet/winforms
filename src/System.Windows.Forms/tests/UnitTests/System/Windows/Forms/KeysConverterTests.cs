// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    public class KeysConverterTests
    {
        [Theory]
        [InlineData("Ctrl + N", Keys.Control | Keys.N)]
        [InlineData("G", Keys.G)]
        public void ConvertFrom_ShouldConvertKeys(string input, Keys keys)
        {
            KeysConverter converter = new();

            var result = (Keys)converter.ConvertFrom(input);

            Assert.Equal(keys, result);
        }

        [Theory]
        [InlineData(Keys.None, "None")]
        [InlineData(Keys.S, "S")]
        [InlineData(Keys.Control | Keys.C, "Ctrl+C")]
        [InlineData(Keys.Control | Keys.Alt | Keys.D, "Ctrl+Alt+D")]
        public void ConvertToString_ShouldConvertKeys(Keys keys, string expectedResult)
        {
            KeysConverter converter = new();
            var result = converter.ConvertToString(keys);
            Assert.Equal(expectedResult, result);
        }
    }
}
