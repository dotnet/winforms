// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class SRDescriptionAttributeTests
{
    [Fact]
    public void VerifyDescriptionAttributeValue()
    {
        SRDescriptionAttribute srDescriptionAttribute = new(nameof(SR.AboutBoxDesc));
        Assert.Equal(SR.AboutBoxDesc, srDescriptionAttribute.Description);

        // Getting srDescriptionAttribute.Description again should also return description value
        Assert.Equal(SR.AboutBoxDesc, srDescriptionAttribute.Description);
    }

    [Fact]
    public void InvalidDescriptionAttributeShouldReturnNull()
    {
        SRDescriptionAttribute srDescriptionAttribute = new("fake");
        Assert.Null(srDescriptionAttribute.Description);
    }
}
