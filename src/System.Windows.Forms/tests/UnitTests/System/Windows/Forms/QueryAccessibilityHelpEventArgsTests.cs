// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class QueryAccessibilityHelpEventArgsTests
    {
        [Fact]
        public void Ctor_Default()
        {
            var e = new QueryAccessibilityHelpEventArgs();
            Assert.Null(e.HelpNamespace);
            Assert.Null(e.HelpString);
            Assert.Null(e.HelpKeyword);
        }

        [Theory]
        [InlineData(null, null, null)]
        [InlineData("", "", "")]
        [InlineData("helpNamespace", "helpString", "helpKeyword")]
        public void Ctor_String_String_String(string helpNamespace, string helpString, string helpKeyword)
        {
            var e = new QueryAccessibilityHelpEventArgs(helpNamespace, helpString, helpKeyword);
            Assert.Equal(helpNamespace, e.HelpNamespace);
            Assert.Equal(helpString, e.HelpString);
            Assert.Equal(helpKeyword, e.HelpKeyword);
        }

        public static IEnumerable<object[]> String_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { "" };
            yield return new object[] { "value" };
        }

        [Theory]
        [MemberData(nameof(String_TestData))]
        public void HelpNamespace_Set_GetReturnsExpected(string value)
        {
            var e = new QueryAccessibilityHelpEventArgs("helpNamespace", "helpString", "helpKeyword")
            {
                HelpNamespace = value
            };
            Assert.Equal(value, e.HelpNamespace);
        }

        [Theory]
        [MemberData(nameof(String_TestData))]
        public void HelpString_Set_GetReturnsExpected(string value)
        {
            var e = new QueryAccessibilityHelpEventArgs("helpNamespace", "helpString", "helpKeyword")
            {
                HelpString = value
            };
            Assert.Equal(value, e.HelpString);
        }

        [Theory]
        [MemberData(nameof(String_TestData))]
        public void HelpKeyword_Set_GetReturnsExpected(string value)
        {
            var e = new QueryAccessibilityHelpEventArgs("helpNamespace", "helpString", "helpKeyword")
            {
                HelpKeyword = value
            };
            Assert.Equal(value, e.HelpKeyword);
        }
    }
}
