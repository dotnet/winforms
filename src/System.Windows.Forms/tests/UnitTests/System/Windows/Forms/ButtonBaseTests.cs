// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ButtonBaseTests
    {
        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(TextImageRelation))]
        public void ToolStripItem_TextImageRelation_Set_GetReturnsExpected(TextImageRelation value)
        {
            var button = new SubButtonBase
            {
                TextImageRelation = value
            };
            Assert.Equal(value, button.TextImageRelation);

            // Set same.
            button.TextImageRelation = value;
            Assert.Equal(value, button.TextImageRelation);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(TextImageRelation))]
        [InlineData((TextImageRelation)3)]
        [InlineData((TextImageRelation)5)]
        [InlineData((TextImageRelation)6)]
        [InlineData((TextImageRelation)7)]
        public void ButtonBase_TextImageRelation_SetInvalid_ThrowsInvalidEnumArgumentException(TextImageRelation value)
        {
            var button = new SubButtonBase();
            Assert.Throws<InvalidEnumArgumentException>("value", () => button.TextImageRelation = value);
        }

        private class SubButtonBase : ButtonBase
        {
        }
    }
}
