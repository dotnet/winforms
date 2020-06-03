// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;
using static Interop.ComCtl32;

namespace System.Windows.Forms.Tests.InteropTests
{
    public class LVITEMWTests
    {
        [Fact]
        public unsafe void UpdateText_should_throw_AOOR_if_cchTextMax_less_than_1()
        {
            var lvi = new LVITEMW
            {
                cchTextMax = 0,
            };

            Assert.Throws<ArgumentOutOfRangeException>(() => lvi.UpdateText("012345"));
        }

#pragma warning disable xUnit1026 // Theory methods should use all of their parameters
        [Theory]
        [MemberData(nameof(UpdateText_TestData))]
        public unsafe void UpdateText_should_limit_input_text_to_cchTextMax_less_1_text_longer(string originalText, int maxLength, string newText, string expected)
        {
            fixed (char* pOriginalText = originalText)
            {
                var lvi = new LVITEMW
                {
                    cchTextMax = maxLength,
                    pszText = pOriginalText
                };

                lvi.UpdateText(newText);

                var text = new string(lvi.pszText);
                Assert.Equal(expected, text);
                Assert.Equal(maxLength - 1, text.Length);
                Assert.Equal(text.Length + 1, lvi.cchTextMax);
                Assert.Equal(maxLength, lvi.cchTextMax);
            }
        }
#pragma warning restore xUnit1026 // Theory methods should use all of their parameters

        public static IEnumerable<object[]> UpdateText_TestData()
        {
            yield return new object[] { "abcdefg", "abcdefg".Length, "0123456", "012345" };
            yield return new object[] { "abcdefg", "abcdefg".Length, "0123456789", "012345" };
        }

        [Fact]
        public unsafe void UpdateText_should_set_cchTextMax_to_input_text_length_plus_1_if_text_shorter()
        {
            string originalText = "abcdefghi";
            fixed (char* pOriginalText = originalText)
            {
                var lvi = new LVITEMW
                {
                    cchTextMax = originalText.Length,
                    pszText = pOriginalText
                };

                lvi.UpdateText("012345");

                var sText = new ReadOnlySpan<char>(lvi.pszText, lvi.cchTextMax);

                var text = new string(lvi.pszText);
                Assert.Equal("012345", text);
                Assert.Equal(lvi.cchTextMax, text.Length + 1);
            }
        }

        [Fact]
        public unsafe void UpdateText_should_set_null_terminated_text()
        {
            string originalText = "abcdefghi";
            fixed (char* pOriginalText = originalText)
            {
                var lvi = new LVITEMW
                {
                    cchTextMax = originalText.Length,
                    pszText = pOriginalText
                };

                lvi.UpdateText("012345");

                var sText = new ReadOnlySpan<char>(lvi.pszText, lvi.cchTextMax);
                Assert.Equal(sText.ToArray(), new char[] { '0', '1', '2', '3', '4', '5', '\0' });
            }
        }
    }
}
