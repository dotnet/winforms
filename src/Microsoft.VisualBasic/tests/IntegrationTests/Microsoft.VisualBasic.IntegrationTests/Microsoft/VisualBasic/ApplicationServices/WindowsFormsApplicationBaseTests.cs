// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Windows.Forms.IntegrationTests.Common;
using Microsoft.VisualBasic.ApplicationServices;
using Xunit;

namespace Microsoft.VisualBasic.IntegrationTests
{
    public class WindowsFormsApplicationBaseTests
    {
        [Fact]
        public void Run()
        {
            string exePath = TestHelpers.GetExePath("VisualBasicRuntimeTest");
            var startInfo = new ProcessStartInfo { FileName = exePath, Arguments = "WindowsFormsApplicationBase.Run" };
            var process = TestHelpers.StartProcess(startInfo);
            TestHelpers.EndProcess(process, timeout: 1000);
            Assert.True(process.HasExited);
        }

        [Fact]
        public void Run_NoStartupFormException()
        {
            var application = new WindowsFormsApplicationBase();
            // Exception.ToString() called to verify message is constructed successfully.
            _ = Assert.Throws<NoStartupFormException>(() => application.Run(new string[0])).ToString();
        }
    }
}
