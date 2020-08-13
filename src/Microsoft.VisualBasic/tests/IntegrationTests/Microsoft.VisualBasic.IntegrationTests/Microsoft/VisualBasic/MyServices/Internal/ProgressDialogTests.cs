// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Windows.Forms.IntegrationTests.Common;
using Xunit;

namespace Microsoft.VisualBasic.MyServices.Internal.Tests
{
    public class ProgressDialogTests
    {
        [Fact]
        public void ShowProgressDialog()
        {
            string exePath = TestHelpers.GetExePath("VisualBasicRuntimeTest");
            var startInfo = new ProcessStartInfo { FileName = exePath, Arguments = "ProgressDialog.ShowProgressDialog" };
            var process = TestHelpers.StartProcess(startInfo);
            TestHelpers.EndProcess(process, timeout: 1000);
            Assert.True(process.HasExited);
            Assert.NotEqual(2, process.ExitCode);
        }
    }
}
