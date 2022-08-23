﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using Windows.Win32.UI.Shell.Common;
using Xunit;

namespace System.Windows.Forms.Tests.Interop.Shell32Tests
{
    public class ShellItemTests
    {
        [Fact]
        public unsafe void SHParseDisplayName_ValidPath()
        {
            string path = Path.GetTempPath();
            uint rgflnOut = default;
            HRESULT result = PInvoke.SHParseDisplayName(path, pbc: null, out ITEMIDLIST* ppidl, 0, &rgflnOut);
            try
            {
                Assert.Equal(HRESULT.S_OK, result);
                Assert.NotEqual(0, (nint)ppidl);
            }
            finally
            {
                if (ppidl is not null)
                {
                    Marshal.FreeCoTaskMem((nint)ppidl);
                }
            }
        }
    }
}
