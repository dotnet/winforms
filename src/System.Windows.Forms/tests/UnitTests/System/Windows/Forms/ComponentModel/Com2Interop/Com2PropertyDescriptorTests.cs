// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using System.Text;
using Xunit;

namespace System.Windows.Forms.ComponentModel.Com2Interop.Tests
{
    // NB: doesn't require thread affinity
    public class Com2PropertyDescriptorTests : IClassFixture<ThreadExceptionFixture>
    {
        private static MethodInfo s_miVersionInfo;
        private static Type s_typeCom2PropertyDescriptor;

        static Com2PropertyDescriptorTests()
        {
            s_typeCom2PropertyDescriptor = typeof(Com2PropertyDescriptor);
            s_miVersionInfo = s_typeCom2PropertyDescriptor.GetMethod("TrimNewline", BindingFlags.Static | BindingFlags.NonPublic);
            Assert.NotNull(s_miVersionInfo);
        }

        [Theory]
        [InlineData("", "")]
        [InlineData("\r", "")]
        [InlineData("\n", "")]
        [InlineData("\r\r\r\r", "")]
        [InlineData("\n\n\n\n", "")]
        [InlineData("\r\r\n\n\r\n\r\n", "")]
        [InlineData("bla bla bla", "bla bla bla")]
        [InlineData("bla bla bla\r\r\r\r\r\n\n\r\n\r\n\r\n", "bla bla bla")]
        [InlineData("bla bla bla\r\n\nr\n\r\n\r\n", "bla bla bla\r\n\nr")]
        public void TrimNewline_should_remove_all_trailing_CR_LF(string message, string expected)
        {
            string result = (string)s_miVersionInfo.Invoke(null, new[] { new StringBuilder(message) });

            Assert.Equal(expected, result);
        }
    }
}
