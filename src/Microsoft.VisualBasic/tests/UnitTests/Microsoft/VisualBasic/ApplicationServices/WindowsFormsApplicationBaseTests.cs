// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using Xunit;

namespace Microsoft.VisualBasic.ApplicationServices.Tests
{
    public class WindowsFormsApplicationBaseTests
    {
        private static string GetAppID(Assembly assembly)
        {
            var type = typeof(WindowsFormsApplicationBase);
            var method = type.GetMethod("GetApplicationInstanceID", BindingFlags.NonPublic | BindingFlags.Static);
            return (string)method.Invoke(null, new object[] { assembly });
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
