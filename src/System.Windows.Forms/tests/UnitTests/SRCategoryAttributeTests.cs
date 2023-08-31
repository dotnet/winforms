// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class SRCategoryAttributeTests
{
    [Fact]
    public void VerifyCategoryForValidCategoryAttribute()
    {
        SRCategoryAttribute srCategoryAttribute = new SRCategoryAttribute(nameof(SR.CatAccessibility));
        Assert.True(string.Equals(srCategoryAttribute.Category, SR.CatAccessibility, StringComparison.Ordinal));
    }

    [Fact]
    public void InvalidCategoryShouldReturnCategoryNameAsIs()
    {
        const string fakeCategory = "fakeCategory";
        SRCategoryAttribute srCategoryAttribute = new SRCategoryAttribute(fakeCategory);
        Assert.True(string.Equals(srCategoryAttribute.Category, fakeCategory, StringComparison.Ordinal));
    }
}
