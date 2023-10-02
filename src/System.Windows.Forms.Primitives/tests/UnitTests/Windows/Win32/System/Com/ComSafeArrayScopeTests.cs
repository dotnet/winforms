// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.System.Com.Tests;

public unsafe class ComSafeArrayScopeTests
{
    [Fact]
    public void ComSafeArrayScope_Construct_NonCOM_ThrowArgumentException()
    {
        SAFEARRAY* array = SAFEARRAY.CreateEmpty(Variant.VARENUM.VT_INT);
        try
        {
            Assert.Throws<ArgumentException>(() => new ComSafeArrayScope<IUnknown>(array));
        }
        finally
        {
            PInvoke.SafeArrayDestroy(array);
        }
    }
}
