// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Tests;

public class NativeUIntExtensionsTests
{
    [Fact]
    public void MinValue_IsZero()
    {
        nuint min = nuint.MinValue;
        ((ulong)min).Should().Be(0UL);
    }

    [Fact]
    public void MaxValue_Is32Or64BitMax()
    {
        nuint max = nuint.MaxValue;

        if (UIntPtr.Size == 4)
        {
            max.Should().Be(unchecked((nuint)uint.MaxValue));
        }
        else
        {
            max.Should().Be(unchecked((nuint)ulong.MaxValue));
        }
    }

    [Fact]
    public void MaxValue_IsGreaterThanMinValue()
    {
        ((ulong)nuint.MaxValue > (ulong)nuint.MinValue).Should().BeTrue();
    }

    [Fact]
    public void MaxValue_IsGreaterThanZero()
    {
        ((ulong)nuint.MaxValue > 0UL).Should().BeTrue();
    }
}
