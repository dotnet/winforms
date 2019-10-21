// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.VisualBasic.Devices;
using Xunit;

namespace Microsoft.VisualBasic.MyServices.Tests
{
    public class RegistryProxyTests
    {
        [Fact]
        public void Properties()
        {
            var registry = (new ServerComputer()).Registry;
            Assert.Equal(Microsoft.Win32.Registry.CurrentUser, registry.CurrentUser);
            Assert.Equal(Microsoft.Win32.Registry.LocalMachine, registry.LocalMachine);
            Assert.Equal(Microsoft.Win32.Registry.ClassesRoot, registry.ClassesRoot);
            Assert.Equal(Microsoft.Win32.Registry.Users, registry.Users);
            Assert.Equal(Microsoft.Win32.Registry.PerformanceData, registry.PerformanceData);
            Assert.Equal(Microsoft.Win32.Registry.CurrentConfig, registry.CurrentConfig);
        }

        [Fact]
        public void GetValue_ArgumentException()
        {
            var keyName = GetUniqueName();
            var registry = (new ServerComputer()).Registry;
            Assert.Throws<ArgumentException>(() => registry.GetValue(keyName, "", null));
        }

        [Fact]
        public void SetValue_ArgumentException()
        {
            var keyName = GetUniqueName();
            var valueName = GetUniqueName();
            var registry = (new ServerComputer()).Registry;
            Assert.Throws<ArgumentException>(() => registry.SetValue(keyName, valueName, ""));
            Assert.Throws<ArgumentException>(() => registry.SetValue(keyName, valueName, "", Microsoft.Win32.RegistryValueKind.String));
        }

        private static string GetUniqueName() => Guid.NewGuid().ToString("D");
    }
}
