// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Forms.Internal;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ApiHelperTests
    {
        [Theory]
        [InlineData(ExternDll.Gdi32, nameof(IntSafeNativeMethods.GdiFlush))]
        [InlineData(ExternDll.Kernel32, nameof(IntSafeNativeMethods.GetUserDefaultLCID))]
        public void KnownAvailableApi(string libName, string procName)
        {
            Assert.True(ApiHelper.IsApiAvailable(libName, procName));
        }

        [Fact]
        public void DuplicateCalls()
        {
            // Test duplicate calls for an API that exists
            Assert.True(ApiHelper.IsApiAvailable(ExternDll.Kernel32, nameof(IntSafeNativeMethods.GetUserDefaultLCID)));
            Assert.True(ApiHelper.IsApiAvailable(ExternDll.Kernel32, nameof(IntSafeNativeMethods.GetUserDefaultLCID)));

            // Test duplicate calls for an API that does not exist (in this case, an incorrect DLL for the given API)
            Assert.False(ApiHelper.IsApiAvailable(ExternDll.Gdi32, nameof(IntSafeNativeMethods.GetUserDefaultLCID)));
            Assert.False(ApiHelper.IsApiAvailable(ExternDll.Gdi32, nameof(IntSafeNativeMethods.GetUserDefaultLCID)));
        }

        [Fact]
        public void UnknownProcedureName()
        {
            const string moduleName = ExternDll.Kernel32;

            // Verify that the module name is correct
            Assert.True(ApiHelper.IsApiAvailable(moduleName, nameof(IntSafeNativeMethods.GetUserDefaultLCID)));

            // Verify that the API returns false for unknown procedures passed to the same module
            Assert.False(ApiHelper.IsApiAvailable(moduleName, "NotAnActualProcedure"));
            Assert.False(ApiHelper.IsApiAvailable(moduleName, ""));
            Assert.False(ApiHelper.IsApiAvailable(moduleName, null));
        }

        [Fact]
        public void UnknownModuleName()
        {
            const string procedureName = nameof(IntSafeNativeMethods.GetUserDefaultLCID);

            // Verify that the procedure name is correct
            Assert.True(ApiHelper.IsApiAvailable(ExternDll.Kernel32, procedureName));

            // Verify that the API returns false for the same name passed to unknown modules
            Assert.False(ApiHelper.IsApiAvailable("NotAnActualModule", procedureName));
            Assert.False(ApiHelper.IsApiAvailable("", procedureName));
            Assert.False(ApiHelper.IsApiAvailable(null, procedureName));
        }
    }
}
