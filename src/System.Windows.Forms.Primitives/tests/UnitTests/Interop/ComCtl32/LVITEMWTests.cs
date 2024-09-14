// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Controls;

namespace System.Windows.Forms.Tests.InteropTests;

public class LVITEMWTests
{
    [Fact]
    public unsafe void UpdateText_should_throw_AOOR_if_cchTextMax_less_than_1()
    {
        LVITEMW lvi = new()
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
            LVITEMW lvi = new()
            {
                cchTextMax = maxLength,
                pszText = pOriginalText
            };

            lvi.UpdateText(newText);

            string text = new(lvi.pszText);
            Assert.Equal(expected, text);
            Assert.Equal(maxLength - 1, text.Length);
            Assert.Equal(text.Length + 1, lvi.cchTextMax);
            Assert.Equal(maxLength, lvi.cchTextMax);
        }
    }
#pragma warning restore xUnit1026

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
            LVITEMW lvi = new()
            {
                cchTextMax = originalText.Length,
                pszText = pOriginalText
            };

            lvi.UpdateText("012345");

            ReadOnlySpan<char> sText = new(lvi.pszText, lvi.cchTextMax);

            string text = new(lvi.pszText);
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
            LVITEMW lvi = new()
            {
                cchTextMax = originalText.Length,
                pszText = pOriginalText
            };

            lvi.UpdateText("012345");

            ReadOnlySpan<char> sText = new(lvi.pszText, lvi.cchTextMax);
            Assert.Equal(sText.ToArray(), new char[] { '0', '1', '2', '3', '4', '5', '\0' });
        }
    }
}
