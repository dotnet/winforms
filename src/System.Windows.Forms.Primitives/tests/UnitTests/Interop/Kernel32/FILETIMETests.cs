// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using Xunit;
using static Interop.Kernel32;

namespace System.Windows.Forms.Tests.Interop.Kernel32
{
    // NB: doesn't require thread affinity
    public class FILETIMETests
    {
        [Fact]
        public unsafe void FILETIME_Sizeof_Invoke_ReturnsExpected()
        {
            Assert.Equal(8, Marshal.SizeOf<FILETIME>());
            Assert.Equal(8, sizeof(FILETIME));
        }

        [Fact]
        public void FILETIME_Ctor_Default()
        {
            var ft = new FILETIME();
            Assert.Equal(0u, ft.dwLowDateTime);
            Assert.Equal(0u, ft.dwHighDateTime);
        }

        [Fact]
        public void FILETIME_Ctor_DateTime()
        {
            var dt = new DateTime(2020, 05, 13, 13, 3, 12);
            var ft = new FILETIME(dt);
            Assert.Equal(3680495616u, ft.dwLowDateTime);
            Assert.Equal(30812454u, ft.dwHighDateTime);
        }

        [Fact]
        public void FILETIME_ToDateTime_Invoke_ReturnsExpected()
        {
            var ft = new FILETIME()
            {
                dwLowDateTime = 3680495616u,
                dwHighDateTime = 30812454u
            };
            Assert.Equal(new DateTime(2020, 05, 13, 13, 3, 12), ft.ToDateTime());
        }
    }
}
