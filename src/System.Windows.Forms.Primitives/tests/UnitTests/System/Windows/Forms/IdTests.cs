// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

public class IdTests
{
    [Fact]
    public void IdSignedAreEquivalent()
    {
        ((Id)1).Should().Be((Id)(-1));
    }

    [Fact]
    public void IdEqualsInt()
    {
        ((Id)4).Should().Be(4);
    }

    [Fact]
    public void IdEqualsId()
    {
        ((Id)4).Should().Be((Id)4);
    }

    [Fact]
    public void IdToStringIsInt()
    {
        ((Id)5).ToString().Should().Be("5");
    }
}
