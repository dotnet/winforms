// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using Xunit;

namespace Microsoft.VisualBasic.ApplicationServices.Tests
{
    public class WindowsFormsApplicationBaseTests
    {
        private static string GetAppID(Assembly assembly)
        {
            var testAccessor = typeof(WindowsFormsApplicationBase).TestAccessor();
            return testAccessor.Dynamic.GetApplicationInstanceID(assembly);
        }

        [Fact]
        public void GetApplicationInstanceID()
        {
            var assembly = typeof(WindowsFormsApplicationBaseTests).Assembly;
            var expectedId = assembly.ManifestModule.ModuleVersionId.ToString();
            Assert.Equal(expectedId, GetAppID(assembly));
        }
    }
}
