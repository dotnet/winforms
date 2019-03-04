// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    public class QuestionEventArgsTests
    {
        [Fact]
        public void Ctor_Default()
        {
            var e = new QuestionEventArgs();
            Assert.False(e.Response);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Ctor_Bool(bool response)
        {
            var e = new QuestionEventArgs(response);
            Assert.Equal(response, e.Response);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Response_Set_GetReturnsExpected(bool value)
        {
            var e = new QuestionEventArgs(!value)
            {
                Response = value
            };
            Assert.Equal(value, e.Response);
        }
    }
}
