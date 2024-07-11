// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using static Interop.Richedit;

namespace System.Windows.Forms.Tests.Interop.Richedit;

// NB: doesn't require thread affinity
public class CHARFORMAT2WTests
{
    [Fact]
    public unsafe void CharFormat_Size()
    {
        Assert.Equal(116, sizeof(CHARFORMAT2W));
    }

    [Fact]
    public unsafe void CharFormat_FaceName()
    {
        CHARFORMAT2W charFormat = default;
        charFormat.FaceName = "TwoFace";
        Assert.Equal("TwoFace", charFormat.FaceName.ToString());

        // Set a smaller name to make sure it gets terminated properly.
        charFormat.FaceName = "Face";
        Assert.Equal("Face", charFormat.FaceName.ToString());

        // CHARFORMAT has space for 32 characters, we want to see it gets
        // cut to 31 to make room for the null.
        string bigString = new('*', 32);

        charFormat.FaceName = bigString;
        Assert.True(charFormat.FaceName.SequenceEqual(bigString.AsSpan()[1..]));
    }
}
