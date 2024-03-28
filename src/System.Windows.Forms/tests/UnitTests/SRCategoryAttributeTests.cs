// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class SRCategoryAttributeTests
{
    [Fact]
    public void VerifyCategoryForValidCategoryAttribute()
    {
        SRCategoryAttribute srCategoryAttribute = new(nameof(SR.CatAccessibility));
        Assert.Equal(SR.CatAccessibility, srCategoryAttribute.Category);
    }

    [Fact]
    public void InvalidCategoryShouldReturnCategoryNameAsIs()
    {
        const string fakeCategory = "fakeCategory";
        SRCategoryAttribute srCategoryAttribute = new(fakeCategory);
        Assert.Equal(fakeCategory, srCategoryAttribute.Category);
    }
}
