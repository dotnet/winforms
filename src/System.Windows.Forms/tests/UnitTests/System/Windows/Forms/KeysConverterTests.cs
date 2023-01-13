// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    public class KeysConverterTests
    {
        [Theory]
        [InlineData("Ctrl+Alt+Shift+A", Keys.Control | Keys.Alt | Keys.Shift | Keys.A)]
        [InlineData("Ctrl+Alt+Shift+F1", Keys.Control | Keys.Alt | Keys.Shift | Keys.F1)]
        [InlineData("Ctrl+Alt+D", Keys.Control | Keys.Alt | Keys.D)]
        [InlineData("Ctrl + N", Keys.Control | Keys.N)]
        [InlineData("G", Keys.G)]
        [InlineData("None", Keys.None)]
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
        [InlineData(Keys.Control | Keys.Add, "Ctrl+Add")]
        [InlineData(Keys.Control | Keys.Alt | Keys.D, "Ctrl+Alt+D")]
        [InlineData(Keys.Control | Keys.Alt | Keys.Shift | Keys.A, "Ctrl+Alt+Shift+A")]
        [InlineData(Keys.Control | Keys.Alt | Keys.Shift | Keys.F1, "Ctrl+Alt+Shift+F1")]
        public void ConvertToString_ShouldConvertKeys(Keys keys, string expectedResult)
        {
            KeysConverter converter = new();
            var result = converter.ConvertToString(keys);
            Assert.Equal(expectedResult, result);
        }

        public static IEnumerable<object[]> ConvertToEnumArray_ShouldConvertKeys_TestData()
        {
            yield return new object[] { Keys.None, new Enum[] { Keys.None } };
            yield return new object[] { Keys.S, new Enum[] { Keys.S } };
            yield return new object[] { Keys.Control | Keys.C, new Enum[] { Keys.Control, Keys.C } };
            yield return new object[] { Keys.Control | Keys.Add, new Enum[] { Keys.Control, Keys.Add } };
            yield return new object[] { Keys.Control | Keys.Alt | Keys.D, new Enum[] { Keys.Control, Keys.Alt, Keys.D } };
            yield return new object[] { Keys.Control | Keys.Alt | Keys.Shift | Keys.A, new Enum[] { Keys.Control, Keys.Alt, Keys.Shift, Keys.A } };
            yield return new object[] { Keys.Control | Keys.Alt | Keys.Shift | Keys.F1, new Enum[] { Keys.Control, Keys.Alt, Keys.Shift, Keys.F1 } };
        }

        [Theory]
        [MemberData(nameof(ConvertToEnumArray_ShouldConvertKeys_TestData))]
        public void ConvertToEnumArray_ShouldConvertKeys(Keys keys, Enum[] expectedResult)
        {
            KeysConverter converter = new();
            var result = converter.ConvertTo(keys, typeof(Enum[]));
            Assert.Equal(expectedResult, result);
        }
    }
}
