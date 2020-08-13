// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class OsVersionTests : IClassFixture<ThreadExceptionFixture>
    {
        private static Interop.NtDll.RTL_OSVERSIONINFOEX s_realVersionInfo;
        private static FieldInfo s_fiVersionInfo;
        private static Type s_typeOsVersion;

        static OsVersionTests()
        {
            // invoke the call and populate the backing field that we're going to replace
            bool _ = OsVersion.IsWindows8_1OrGreater;

            s_typeOsVersion = typeof(OsVersion);
            s_fiVersionInfo = s_typeOsVersion.GetField("s_versionInfo", Reflection.BindingFlags.Static | Reflection.BindingFlags.NonPublic);
            Assert.NotNull(s_fiVersionInfo);

            s_realVersionInfo = (Interop.NtDll.RTL_OSVERSIONINFOEX)s_fiVersionInfo.GetValue(null);
        }

        [StaTheory]
        [InlineData(5, 0, 0, false)]
        [InlineData(6, 0, 0, false)]
        [InlineData(10, 0, 0, false)]
        [InlineData(10, 0, 14393, true)]
        [InlineData(10, 0, 15063, true)]
        [InlineData(11, 0, 20000, true)] // it is a bit of a leap of faith that we have another OS version and its build number increments in sequence
        public void IsWindows10_1607OrGreater(uint major, uint minor, uint buildNumber, bool expected)
        {
            PerformAssert(major, minor, buildNumber, () => Assert.Equal(expected, OsVersion.IsWindows10_1607OrGreater));
        }

        [StaTheory]
        [InlineData(5, 0, 0, false)]
        [InlineData(6, 0, 0, false)]
        [InlineData(10, 0, 0, false)]
        [InlineData(10, 0, 14393, false)]
        [InlineData(10, 0, 15063, true)]
        [InlineData(11, 0, 20000, true)] // it is a bit of a leap of faith that we have another OS version and its build number increments in sequence
        public void IsWindows10_1703OrGreater(uint major, uint minor, uint buildNumber, bool expected)
        {
            PerformAssert(major, minor, buildNumber, () => Assert.Equal(expected, OsVersion.IsWindows10_1703OrGreater));
        }

        [StaTheory]
        [InlineData(5, 0, 0, false)]
        [InlineData(6, 2, 0, false)]
        [InlineData(6, 3, 0, true)]
        [InlineData(10, 0, 0, true)]
        [InlineData(10, 0, 14393, true)]
        [InlineData(10, 0, 15063, true)]
        [InlineData(11, 0, 20000, true)] // it is a bit of a leap of faith that we have another OS version and its build number increments in sequence
        public void IsWindows8_1OrGreater(uint major, uint minor, uint buildNumber, bool expected)
        {
            PerformAssert(major, minor, buildNumber, () => Assert.Equal(expected, OsVersion.IsWindows8_1OrGreater));
        }

        private void PerformAssert(uint major, uint minor, uint buildNumber, Action assertion)
        {
            try
            {
                Interop.NtDll.RTL_OSVERSIONINFOEX verInfo = new Interop.NtDll.RTL_OSVERSIONINFOEX
                {
                    dwMajorVersion = major,
                    dwMinorVersion = minor,
                    dwBuildNumber = buildNumber
                };
                s_fiVersionInfo.SetValue(null, verInfo);

                assertion();
            }
            finally
            {
                // restore the real OS version
                s_fiVersionInfo.SetValue(null, s_realVersionInfo);
            }
        }
    }
}
