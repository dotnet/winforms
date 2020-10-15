// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Runtime.InteropServices;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests.Interop.Shell32Tests
{
    public class ShellItemTests
    {
        [Fact]
        public void SHILCreateFromPath_ValidPath()
        {
            string path = Path.GetTempPath();
            uint rgflnOut = default;
            HRESULT result = Shell32.SHILCreateFromPath(path, out IntPtr ppidl, ref rgflnOut);
            try
            {
                Assert.Equal(HRESULT.S_OK, result);
                Assert.NotEqual(IntPtr.Zero, ppidl);
            }
            finally
            {
                if (ppidl != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(ppidl);
                }
            }
        }
    }
}
