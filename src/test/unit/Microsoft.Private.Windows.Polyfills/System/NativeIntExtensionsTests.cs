// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Tests;

public class NativeIntExtensionsTests
{
    [Fact]
    public void MinValue_Is32Or64BitMin()
    {
        nint min = nint.MinValue;

        if (IntPtr.Size == 4)
        {
            min.Should().Be(unchecked((nint)int.MinValue));
        }
        else
        {
            min.Should().Be(unchecked((nint)long.MinValue));
        }
    }

    [Fact]
    public void MaxValue_Is32Or64BitMax()
    {
        nint max = nint.MaxValue;

        if (IntPtr.Size == 4)
        {
            max.Should().Be(unchecked((nint)int.MaxValue));
        }
        else
        {
            max.Should().Be(unchecked((nint)long.MaxValue));
        }
    }

    [Fact]
    public void MaxValue_IsGreaterThanMinValue()
    {
        ((long)nint.MaxValue > (long)nint.MinValue).Should().BeTrue();
    }

    [Fact]
    public void MinValue_IsLessThanZero()
    {
        ((long)nint.MinValue < 0L).Should().BeTrue();
    }

    [Fact]
    public void MaxValue_IsGreaterThanZero()
    {
        ((long)nint.MaxValue > 0L).Should().BeTrue();
    }
}
