// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class NoneExcludedImageIndexConverterTests
{
    [Fact]
    public void NoneExcludedImageIndexConverter_IncludeNoneAsStandardValue_ReturnsFalse()
    {
        Assert.False(new NoneExcludedImageIndexConverter().TestAccessor().Dynamic.IncludeNoneAsStandardValue);
    }
}
