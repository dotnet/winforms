// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Com;

namespace System.Windows.Forms.Primitives.Tests.Windows.Win32;

public class AgileComPointerTests
{
    [Fact]
    public unsafe void AgileComPointer_CheckRefCounts()
    {
        IStream* stream = ComHelpers.GetComPointer<IStream>(new GlobalInterfaceTableTests.MyStream());

        uint count;

        using (AgileComPointer<IStream> agileStream = new(stream, takeOwnership: true))
        {
            count = stream->AddRef();
            Assert.Equal(2u, count);
            using var astream = agileStream.GetInterface();
        }

        count = stream->Release();
        Assert.Equal(0u, count);
    }

    [StaFact]
    public async Task AgileComPointer_MultiThread_COMPointerValue_ForSameObject()
    {
        using AgileComPointer<IStream> agileStream = CreateMyStreamAgileComPointer(out nint originalPtr);
        try
        {
            using AgileComPointer<IStream> proxyAgileStream = await GetProxyAgileComPointer(agileStream);
            Validate();

            unsafe void Validate()
            {
                using var proxyStreamPtr = proxyAgileStream.GetInterface();
                Assert.NotEqual(originalPtr, (nint)proxyStreamPtr.Value);

                // Both COM pointers must be registered in the GIT to determine identity,
                // even if IUnknown has been queried.
                using var proxyUnknownPtr = proxyAgileStream.GetInterface<IUnknown>();
                using ComScope<IUnknown> originalUnknownPtr = new(null);
                ((IStream*)originalPtr)->QueryInterface(IID.Get<IUnknown>(), originalUnknownPtr);
                Assert.NotEqual((nint)originalUnknownPtr.Value, proxyUnknownPtr);

                // This will succeed at determining identity since both COM pointers have
                // been registered in the GIT via AgileComPointer
                Assert.True(agileStream.IsSameNativeObject(proxyAgileStream));
            }
        }
        finally
        {
            unsafe
            {
                ((IStream*)originalPtr)->Release();
            }
        }

        unsafe AgileComPointer<IStream> CreateMyStreamAgileComPointer(out nint originalPtr)
        {
            GlobalInterfaceTableTests.MyStream myStream = new();
            IStream* streamPtr = ComHelpers.GetComPointer<IStream>(myStream);
            originalPtr = (nint)streamPtr;
            AgileComPointer<IStream> agile = new(streamPtr, takeOwnership: false);

            using ComScope<IStream> streamScope = agile.GetInterface();
            Assert.Equal(originalPtr, (nint)streamScope.Value);

            return agile;
        }

        unsafe Task<AgileComPointer<IStream>> GetProxyAgileComPointer(AgileComPointer<IStream> stream)
        {
            return Task.Run(() =>
            {
                // stream.GetInterface() returns a proxy because this is not the same thread the
                // object was created on.
                return new AgileComPointer<IStream>(stream.GetInterface(), takeOwnership: true);
            });
        }
    }
}
