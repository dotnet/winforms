// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    public class SRCategoryAttributeTests
    {
        [Fact]
        public void VerifyCategoryForValidCategoryAttribute()
        {
            SRCategoryAttribute srCategoryAttribute = new SRCategoryAttribute(nameof(SR.CatAccessibility));
            Assert.True(string.Compare(srCategoryAttribute.Category, SR.CatAccessibility, StringComparison.Ordinal) == 0);            
        }

        [Fact]
        public void InvalidCategoryShouldReturnCategoryNameAsIs()
        {
            const string fakeCategory = "fakeCategory";
            SRCategoryAttribute srCategoryAttribute = new SRCategoryAttribute(fakeCategory);
            Assert.True(string.Compare(srCategoryAttribute.Category, fakeCategory, StringComparison.Ordinal) == 0);
        }
    }
}
