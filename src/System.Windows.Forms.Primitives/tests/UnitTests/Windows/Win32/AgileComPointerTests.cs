// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.Win32.System.Com;

namespace System.Windows.Forms.Primitives.Tests.Windows.Win32
{
    public unsafe class AgileComPointerTests
    {
        [Fact]
        public void AgileComPointer_CheckRefCounts()
        {
            ComHelpers.GetComPointer(new GlobalInterfaceTableTests.MyStream(), out IStream* stream);

            uint count;

            using (var agileStream = new AgileComPointer<IStream>(stream))
            {
                count = stream->AddRef();
                Assert.Equal(2u, count);
                using var astream = agileStream.GetInterface();
            }

            count = stream->Release();
            Assert.Equal(0u, count);
        }
    }
}
