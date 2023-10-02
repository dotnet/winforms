// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Com;

namespace System.Windows.Forms.Primitives.Tests.Windows.Win32.System.Com;

public class SafeArrayScopeTests
{
    [Fact]
    public void SafeArrayScope_Construct_KnownType_Success()
    {
        using SafeArrayScope<string> scope = new(size: 1);
        using SafeArrayScope<int> scope2 = new(size: 0);
        Assert.False(scope.IsNull);
        Assert.False(scope2.IsNull);
    }

    [Fact]
    public void SafeArrayScope_Construct_UnknownType_Throws()
    {
        Assert.Throws<ArgumentException>(() => new SafeArrayScope<short>(size: 1));
    }

    [Fact]
    public void SafeArrayScope_StringIndexing_Success()
    {
        using SafeArrayScope<string> scope = new(size: 2);
        Assert.False(scope.IsNull);
        string input1 = "1";
        string input2 = "2";
        scope[0] = input1;
        scope[1] = input2;
        Assert.Equal(input1, scope[0]);
        Assert.Equal(input2, scope[1]);
    }

    [Fact]
    public void SafeArrayScope_IntIndexing_Success()
    {
        using SafeArrayScope<int> scope = new(size: 2);
        Assert.False(scope.IsNull);
        scope[0] = 1;
        scope[1] = 2;
        Assert.Equal(1, scope[0]);
        Assert.Equal(2, scope[1]);
    }

    [Fact]
    public void SafeArrayScope_Dispose_Success()
    {
        SafeArrayScope<string> scope = new(size: 1);
        Assert.False(scope.IsNull);

        scope.Dispose();
        Assert.False(scope.IsNull);
    }
}
