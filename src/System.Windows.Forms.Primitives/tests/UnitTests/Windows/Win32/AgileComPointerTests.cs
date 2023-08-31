// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Com;

namespace System.Windows.Forms.Primitives.Tests.Windows.Win32;

public unsafe class AgileComPointerTests
{
    [Fact]
    public void AgileComPointer_CheckRefCounts()
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
}
