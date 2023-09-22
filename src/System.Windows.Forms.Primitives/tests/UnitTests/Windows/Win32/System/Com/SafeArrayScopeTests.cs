// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Com;
using Windows.Win32.System.Variant;

namespace System.Windows.Forms.Primitives.Tests.Windows.Win32.System.Com;

public class SafeArrayScopeTests
{
    [Fact]
    public void Construct_Success()
    {
        SAFEARRAYBOUND bound = new()
        {
            cElements = 1,
            lLbound = 0
        };

        using SafeArrayScope scope = new(VARENUM.VT_BSTR, 1, bound);
        Assert.False(scope.IsNull);
    }

    [Fact]
    public void Dispose_Success()
    {
        SAFEARRAYBOUND bound = new()
        {
            cElements = 1,
            lLbound = 0
        };

        SafeArrayScope scope = new(VARENUM.VT_BSTR, 1, bound);
        Assert.False(scope.IsNull);

        scope.Dispose();
        Assert.True(scope.IsNull);
    }
}
