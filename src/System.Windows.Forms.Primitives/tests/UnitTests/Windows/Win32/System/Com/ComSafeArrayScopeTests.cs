// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace Windows.Win32.System.Com.Tests;

public unsafe class ComSafeArrayScopeTests
{
    [Fact]
    public void ComSafeArrayScope_Construct_NonCOM_ThrowArgumentException()
    {
        SAFEARRAY* array = SAFEARRAY.CreateEmpty(VARENUM.VT_INT);
        try
        {
            Assert.Throws<ArgumentException>(() => new ComSafeArrayScope<IUnknown>(array));
        }
        finally
        {
            PInvokeCore.SafeArrayDestroy(array);
        }
    }

    [Fact]
    public void ComSafeArrayScope_CreateComSafeArrayScopeExtension_Success()
    {
        IRawElementProviderSimple.Interface[] providers = [new MyRawElementProviderSimple()];
        using var scope = providers.CreateComSafeArrayScope<IRawElementProviderSimple, IRawElementProviderSimple.Interface>();
        Assert.False(scope.IsNull);
        Assert.Equal(1, scope.Length);
        using var expected = ComHelpers.GetComScope<IRawElementProviderSimple>(providers[0]);
        Assert.Equal((nint)expected.Value, (nint)scope[0]);
    }

    private class MyRawElementProviderSimple : IRawElementProviderSimple.Interface, IManagedWrapper<IRawElementProviderSimple>
    {
        public HRESULT get_ProviderOptions(ProviderOptions* pRetVal) => throw new NotImplementedException();
        public HRESULT GetPatternProvider(UIA_PATTERN_ID patternId, IUnknown** pRetVal) => throw new NotImplementedException();
        public HRESULT GetPropertyValue(UIA_PROPERTY_ID propertyId, VARIANT* pRetVal) => throw new NotImplementedException();
        public HRESULT get_HostRawElementProvider(IRawElementProviderSimple** pRetVal) => throw new NotImplementedException();
    }
}
