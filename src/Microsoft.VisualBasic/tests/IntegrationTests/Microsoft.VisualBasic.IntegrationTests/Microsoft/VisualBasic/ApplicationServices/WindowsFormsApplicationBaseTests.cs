// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
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
            var process1 = TestHelpers.StartProcess(startInfo);
            Assert.False(process1.HasExited);
            TestHelpers.EndProcess(process, timeout: 1000);
            Assert.True(process.HasExited);
            TestHelpers.EndProcess(process1, timeout: 1000);
            Assert.True(process1.HasExited);
        }

        [Fact]
        public void RunSingleInstance()
        {
            string exePath = TestHelpers.GetExePath("VisualBasicRuntimeTest");
            var startInfo = new ProcessStartInfo { FileName = exePath, Arguments = "WindowsFormsApplicationBase.RunSingleInstance" };
            List<Process> ProcessList = new List<Process>();
            for (int i = 0; i <= 9; i++)
            {
                ProcessList.Add(TestHelpers.StartProcess(startInfo));
            }
            System.Threading.Thread.Sleep(1000);
            for (int i = 1; i <= 9; i++)
            {
            Assert.True(ProcessList[i].HasExited, $"Instance {i} didn't exit");
            }
            TestHelpers.EndProcess(ProcessList[0], timeout: 1000);
            Assert.True(ProcessList[0].HasExited, "First Instance didn't exit");
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
