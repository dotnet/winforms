// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static Interop;


namespace System.Windows.Forms.Tests.InteropTests
{
    public class RtlGetVersionTests
    {
        [Fact]
        public void BasicFunctionality()
        {
            NtDll.RtlGetVersion(out var info);

            // Windows 7 was 6.1, 8 is 6.2, 8.1 is 6.3, 10 is 10
            switch (info.dwMajorVersion)
            {
                case 6:
                    Assert.True(info.dwMinorVersion >= 1 && info.dwMinorVersion <= 3, $"Minor version {info.dwMinorVersion} isn't valid for major version 6");
                    break;
                case 10:
                    Assert.True(info.dwMinorVersion == 0, $"Minor version {info.dwMinorVersion} should be 0 for Windows 10");
                    Assert.True(info.dwBuildNumber >= 10240, $"Build number {info.dwBuildNumber} should be at least Threshold 1 (10240)");
                    break;
                default:
                    Assert.True(false, $"Major version {info.dwMajorVersion} not a known OS version");
                    break;
            }
        }
    }
}
