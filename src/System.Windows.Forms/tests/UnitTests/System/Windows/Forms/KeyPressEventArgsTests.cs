// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    public class KeyPressEventArgsTests
    {
        [Theory]
        [InlineData('\0')]
        [InlineData('a')]
        public void Ctor_Char(char keyChar)
        {
            var e = new KeyPressEventArgs(keyChar);
            Assert.Equal(keyChar, e.KeyChar);
            Assert.False(e.Handled);
        }

        [Theory]
        [InlineData('\0')]
        [InlineData('a')]
        public void KeyChar_Set_GetReturnsExpected(char value)
        {
            var e = new KeyPressEventArgs('b')
            {
                KeyChar = value
            };
            Assert.Equal(value, e.KeyChar);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Handled_Set_GetReturnsExpected(bool value)
        {
            var e = new KeyPressEventArgs('a')
            {
                Handled = value
            };
            Assert.Equal(value, e.Handled);
        }
    }
}
