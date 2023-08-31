// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.IntegrationTests.Common;

namespace Microsoft.VisualBasic.MyServices.Internal.Tests;

public class ProgressDialogTests
{
    [Fact]
    public void ShowProgressDialog()
    {
        string exePath = TestHelpers.GetExePath("VisualBasicRuntimeTest");
        ProcessStartInfo startInfo = new() { FileName = exePath, Arguments = "ProgressDialog.ShowProgressDialog" };
        Process process = TestHelpers.StartProcess(startInfo);
        TestHelpers.EndProcess(process, timeout: 1000);
        Assert.True(process.HasExited);
        Assert.NotEqual(2, process.ExitCode);
    }
}
