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

        using (var agileStream = new AgileComPointer<IStream>(stream, takeOwnership: true))
        {
            count = stream->AddRef();
            Assert.Equal(2u, count);
            using var astream = agileStream.GetInterface();
        }

        count = stream->Release();
        Assert.Equal(0u, count);
    }

    [StaFact]
    public async Task AgileComPointer_IsSameNativeObject_True_ForMultiThread()
    {
        using AgileComPointer<IStream> agileStream = CreateMyStreamAgileComPointer();
        using AgileComPointer<IStream> agileStream2 = await CloneFromDifferentThread(agileStream);
        Assert.True(agileStream.IsSameNativeObject(agileStream2));

        unsafe AgileComPointer<IStream> CreateMyStreamAgileComPointer()
        {
            GlobalInterfaceTableTests.MyStream myStream = new();
            AgileComPointer<IStream> agile = new(ComHelpers.GetComPointer<IStream>(myStream), takeOwnership: true);
            return agile;
        }

        unsafe Task<AgileComPointer<IStream>> CloneFromDifferentThread(AgileComPointer<IStream> stream)
        {
            return Task.Run(() =>
            {
                return new AgileComPointer<IStream>(stream.GetInterface(), takeOwnership: true);
            });
        }
    }
}
