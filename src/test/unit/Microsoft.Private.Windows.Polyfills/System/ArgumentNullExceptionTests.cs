// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Tests;

public class ArgumentNullExceptionTests
{
    [Fact]
    public void ThrowIfNull_NonNull_DoesNotThrow()
    {
        object value = new();
        ArgumentNullException.ThrowIfNull(value);
    }

    [Fact]
    public void ThrowIfNull_Null_ThrowsArgumentNullException()
    {
        object? value = null;
        Action action = () => ArgumentNullException.ThrowIfNull(value);
        action.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be(nameof(value));
    }

    [Fact]
    public unsafe void ThrowIfNull_Pointer_NonNull_DoesNotThrow()
    {
        int i = 42;
        void* value = &i;
        ArgumentNullException.ThrowIfNull(value);
    }

    [Fact]
    public unsafe void ThrowIfNull_Pointer_Null_ThrowsArgumentNullException()
    {
        void* value = null;
        Action action = () => ArgumentNullException.ThrowIfNull(value);
        action.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be(nameof(value));
    }

    [Fact]
    public void ThrowIfNull_CustomParamName()
    {
        object? value = null;
        Action action = () => ArgumentNullException.ThrowIfNull(value, "customParam");
        action.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("customParam");
    }
}
