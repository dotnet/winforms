// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class AmbientPropertiesTests
    {
        [Fact]
        public void Ctor_Default()
        {
            var property = new AmbientProperties();
            Assert.Equal(Color.Empty, property.BackColor);
            Assert.Null(property.Cursor);
            Assert.Null(property.Font);
            Assert.Equal(Color.Empty, property.ForeColor);
        }

        [Fact]
        public void BackColor_Set_GetReturnsExpected()
        {
            var property = new AmbientProperties
            {
                BackColor = Color.Red
            };
            Assert.Equal(Color.Red, property.BackColor);
        }

        [Fact]
        public void Cursor_Set_GetReturnsExpected()
        {
            var cursor = new Cursor((IntPtr)1);
            var property = new AmbientProperties
            {
                Cursor = cursor
            };
            Assert.Equal(cursor, property.Cursor);
        }

        [Fact]
        public void Font_Set_GetReturnsExpected()
        {
            var property = new AmbientProperties
            {
                Font = SystemFonts.DefaultFont
            };
            Assert.Equal(SystemFonts.DefaultFont, property.Font);
        }

        [Fact]
        public void ForeColor_Set_GetReturnsExpected()
        {
            var property = new AmbientProperties
            {
                ForeColor = Color.Red
            };
            Assert.Equal(Color.Red, property.ForeColor);
        }
    }
}
