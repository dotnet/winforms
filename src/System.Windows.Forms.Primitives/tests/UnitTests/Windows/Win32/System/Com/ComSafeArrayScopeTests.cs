// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

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

    [Fact]
    public void ComSafeArrayScope_CreateFromInterfaceArray_DoesNot_ThrowArgumentException()
    {
        IRawElementProviderSimple.Interface[] providers = [new MyRawElementProviderSimple()];
        Action action = () =>
        {
            using var scope = ComSafeArrayScope<IRawElementProviderSimple>.CreateFromInterfaceArray(providers);
        };

        action.Should().NotThrow<ArgumentException>();
    }

    [Fact]
    public void ComSafeArrayScope_CreateFromInterfaceArray_ThrowsArgumentException()
    {
        IStream.Interface[] streams = [new MyStream()];
        Action action = () =>
        {
            using var scope = ComSafeArrayScope<IStream>.CreateFromInterfaceArray(streams);
        };

        // Note that this test may fail later on when IStream implements IComInterface<IStream.Interface>.
        action.Should().Throw<ArgumentException>();
    }

    private class MyRawElementProviderSimple : IRawElementProviderSimple.Interface, IManagedWrapper<IRawElementProviderSimple>
    {
        public HRESULT get_ProviderOptions(ProviderOptions* pRetVal) => throw new NotImplementedException();
        public HRESULT GetPatternProvider(UIA_PATTERN_ID patternId, IUnknown** pRetVal) => throw new NotImplementedException();
        public HRESULT GetPropertyValue(UIA_PROPERTY_ID propertyId, VARIANT* pRetVal) => throw new NotImplementedException();
        public HRESULT get_HostRawElementProvider(IRawElementProviderSimple** pRetVal) => throw new NotImplementedException();
    }

    private class MyStream : IStream.Interface, IManagedWrapper<IStream, ISequentialStream>
    {
        public unsafe HRESULT Read(void* pv, uint cb, [Optional] uint* pcbRead) => throw new NotImplementedException();
        public unsafe HRESULT Write(void* pv, uint cb, [Optional] uint* pcbWritten) => throw new NotImplementedException();
        public unsafe HRESULT Seek(long dlibMove, SeekOrigin dwOrigin, [Optional] ulong* plibNewPosition) => throw new NotImplementedException();
        public HRESULT SetSize(ulong libNewSize) => throw new NotImplementedException();
        public unsafe HRESULT CopyTo(IStream* pstm, ulong cb, [Optional] ulong* pcbRead, [Optional] ulong* pcbWritten) => throw new NotImplementedException();
        public HRESULT Commit(uint grfCommitFlags) => throw new NotImplementedException();
        public HRESULT Revert() => throw new NotImplementedException();
        public HRESULT LockRegion(ulong libOffset, ulong cb, uint dwLockType) => throw new NotImplementedException();
        public HRESULT UnlockRegion(ulong libOffset, ulong cb, uint dwLockType) => throw new NotImplementedException();
        public unsafe HRESULT Stat(STATSTG* pstatstg, uint grfStatFlag) => throw new NotImplementedException();
        public unsafe HRESULT Clone(IStream** ppstm) => throw new NotImplementedException();
    }
}
