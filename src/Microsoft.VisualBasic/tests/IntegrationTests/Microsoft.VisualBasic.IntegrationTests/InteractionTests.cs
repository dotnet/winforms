// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms.IntegrationTests.Common;
using Xunit;

namespace Microsoft.VisualBasic.IntegrationTests
{
    public class InteractionTests
    {
        [Fact]
        public void AppActivate_ProcessId()
        {
            Process process = StartTestProcess("Interaction.MsgBox");
            try
            {
                Interaction.AppActivate(process.Id);
            }
            catch (ArgumentException)
            {
                // Modal dialog may be closed automatically on build machine.
            }
            EndProcess(process);
        }

        [Fact]
        public void AppActivate_Title_ArgumentException()
        {
            // Exception.ToString() called to verify message is constructed successfully.
            _ = Assert.Throws<ArgumentException>(() => Interaction.AppActivate(GetUniqueName())).ToString();
        }

        [Fact]
        public void InputBox()
        {
            Process process = StartTestProcess("Interaction.InputBox");
            EndProcess(process);
        }

        [Fact]
        public void InputBox_VbHost()
        {
            Process process = StartTestProcess("Interaction.InputBox_VbHost");
            EndProcess(process);
        }

        [Fact]
        public void VBInputBox_ShowDialog()
        {
            Process process = StartTestProcess("VBInputBox.ShowDialog");
            EndProcess(process);
            Assert.True(process.HasExited);
            Assert.NotEqual(2, process.ExitCode);
        }

        [Fact]
        public void MsgBox()
        {
            Process process = StartTestProcess("Interaction.MsgBox");
            try
            {
                TestHelpers.SendEnterKeyToProcess(process);
            }
            catch (ArgumentException)
            {
                // Modal dialog may be closed automatically on build machine.
            }
            EndProcess(process);
        }

        [Fact]
        public void MsgBox_VbHost()
        {
            Process process = StartTestProcess("Interaction.MsgBox_VbHost");
            try
            {
                TestHelpers.SendEnterKeyToProcess(process);
            }
            catch (ArgumentException)
            {
                // Modal dialog may be closed automatically on build machine.
            }
            EndProcess(process);
        }

        [Fact]
        public void Shell()
        {
            int processId = Interaction.Shell(_exePath);
            Process process = Process.GetProcessById(processId);
            process.Kill();
            process.WaitForExit();
        }

        [Fact]
        public void Shell_NullReferenceException()
        {
            // Exception.ToString() called to verify message is constructed successfully.
            _ = Assert.Throws<NullReferenceException>(() => Interaction.Shell(null)).ToString();
        }

        [Fact]
        public void Shell_FileNotFoundException()
        {
            var path = Path.Combine(Path.GetTempPath(), GetUniqueName());
            // Exception.ToString() called to verify message is constructed successfully.
            _ = Assert.Throws<FileNotFoundException>(() => Interaction.Shell(path)).ToString();
        }

        private static readonly string _exePath = TestHelpers.GetExePath("VisualBasicRuntimeTest");

        private static Process StartTestProcess(string arguments)
        {
            var startInfo = new ProcessStartInfo { FileName = _exePath, Arguments = arguments };
            return TestHelpers.StartProcess(startInfo);
        }

        private static void EndProcess(Process process)
        {
            TestHelpers.EndProcess(process, 1000);
        }

        private static string GetUniqueName() => Guid.NewGuid().ToString("D");
    }
}
