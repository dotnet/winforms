// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using WinForms.Common.Tests;
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetColorWithEmptyTheoryData))]
        public void BackColor_Set_GetReturnsExpected(Color value)
        {
            var property = new AmbientProperties
            {
                BackColor = value
            };
            Assert.Equal(value, property.BackColor);

            // Set same.
            property.BackColor = value;
            Assert.Equal(value, property.BackColor);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetCursorTheoryData))]
        public void Cursor_Set_GetReturnsExpected(Cursor value)
        {
            var property = new AmbientProperties
            {
                Cursor = value
            };
            Assert.Equal(value, property.Cursor);

            // Set same.
            property.Cursor = value;
            Assert.Equal(value, property.Cursor);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetFontTheoryData))]
        public void Font_Set_GetReturnsExpected(Font value)
        {
            var property = new AmbientProperties
            {
                Font = value
            };
            Assert.Equal(value, property.Font);

            // Set same.
            property.Font = value;
            Assert.Equal(value, property.Font);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetColorTheoryData))]
        public void ForeColor_Set_GetReturnsExpected(Color value)
        {
            var property = new AmbientProperties
            {
                ForeColor = value
            };
            Assert.Equal(value, property.ForeColor);

            // Set same.
            property.ForeColor = value;
            Assert.Equal(value, property.ForeColor);
        }
    }
}
