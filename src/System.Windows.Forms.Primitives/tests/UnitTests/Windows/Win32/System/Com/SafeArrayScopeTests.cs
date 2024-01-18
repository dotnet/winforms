// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.System.Com.Tests;

public unsafe class SafeArrayScopeTests
{
    [Fact]
    public void SafeArrayScope_Construct_Array_Success()
    {
        int[] intArray = [1, 2, 3];
        using SafeArrayScope<int> scope = new(intArray);
        Assert.Equal(intArray.Length, scope.Length);
        for (int i = 0; i < intArray.Length; i++)
        {
            Assert.Equal(intArray[i], scope[i]);
        }
    }

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
    public void SafeArrayScope_NullAfterDispose()
    {
        SafeArrayScope<string> scope = new(size: 1);
        Assert.False(scope.IsNull);

        scope.Dispose();
        Assert.True(scope.IsNull);
    }

    [Fact]
    public void SafeArrayScope_Construct_OfCOM_ThrowsArgumentException()
    {
        string message = "Use ComSafeArrayScope instead";
        ArgumentException e = Assert.Throws<ArgumentException>(() => new SafeArrayScope<IUnknown>(size: 1));
        Assert.Equal(message, e.Message);
        SAFEARRAY* array = SAFEARRAY.CreateEmpty(Variant.VARENUM.VT_UNKNOWN);
        try
        {
            e = Assert.Throws<ArgumentException>(() => new SafeArrayScope<IUnknown>(array));
            Assert.Equal(message, e.Message);
        }
        finally
        {
            PInvokeCore.SafeArrayDestroy(array);
        }
    }

    [Fact]
    public void SafeArrayScope_Construct_MismatchType_ThrowsArgumentException()
    {
        SAFEARRAY* array = SAFEARRAY.CreateEmpty(Variant.VARENUM.VT_INT);
        try
        {
            ArgumentException e = Assert.Throws<ArgumentException>(() => new SafeArrayScope<string>(array));
            Assert.Equal("Wanted SafeArrayScope<System.String> but got SAFEARRAY with VarType=VT_INT", e.Message);
        }
        finally
        {
            PInvokeCore.SafeArrayDestroy(array);
        }
    }
}
