// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    public class SRDescriptionAttributeTests
    {
        [Fact]
        public void VerifyDescriptionAttributeValue()
        {
            SRDescriptionAttribute srDescriptionAttribute = new SRDescriptionAttribute(nameof(SR.AboutBoxDesc));
            Assert.True(string.Compare(srDescriptionAttribute.Description, SR.AboutBoxDesc, StringComparison.Ordinal) == 0);

            // Getting srDescriptionAttribute.Description again should also return description value
            Assert.True(string.Compare(srDescriptionAttribute.Description, SR.AboutBoxDesc, StringComparison.Ordinal) == 0);
        }

        [Fact]
        public void InvalidDescriptionAttributeShouldReturnNull()
        {
            SRDescriptionAttribute srDescriptionAttribute = new SRDescriptionAttribute("fake");
            Assert.Null(srDescriptionAttribute.Description);
        }
    }
}
