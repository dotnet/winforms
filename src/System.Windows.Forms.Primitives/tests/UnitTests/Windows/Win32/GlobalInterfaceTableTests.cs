// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using Windows.Win32.System.Com;

namespace System.Windows.Forms.Primitives.Tests.Windows.Win32;

public unsafe class GlobalInterfaceTableTests
{
    [Fact]
    public void GlobalInterfaceTable_CheckRefCounts()
    {
        using var stream = ComHelpers.TryGetComScope<IStream>(new MyStream(), out HRESULT hr);
        Assert.True(hr.Succeeded);

        uint count = stream.Value->AddRef();
        Assert.Equal(2u, count);

        count = stream.Value->Release();
        Assert.Equal(1u, count);

        uint cookie = GlobalInterfaceTable.RegisterInterface(stream.Value);
        count = stream.Value->AddRef();
        // +1 for the AddRef, and +1 for the git registration
        Assert.Equal(3u, count);

        count = stream.Value->Release();
        Assert.Equal(2u, count);

        GlobalInterfaceTable.RevokeInterface(cookie);
        count = stream.Value->AddRef();
        Assert.Equal(2u, count);
        stream.Value->Release();
    }

    internal class MyStream : IStream.Interface, IManagedWrapper<IStream, ISequentialStream>
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
