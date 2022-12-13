// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms.Tests.TestResources;
using Xunit;

namespace System.Windows.Forms.Tests;

public class AxHostVisualBasic6Tests : IClassFixture<ThreadExceptionFixture>
{
    [WinFormsFact(Skip = "Causes test run to abort, must be run manually.")]
    public void AxHost_SimpleControl_Create()
    {
        if (RuntimeInformation.ProcessArchitecture != Architecture.X86)
        {
            return;
        }

        using Form form = new();
        form.Shown += (object sender, EventArgs e) => form.Close();
        using DynamicAxHost control = new(ComClasses.VisualBasicSimpleControl);
        ((ISupportInitialize)control).BeginInit();
        form.Controls.Add(control);
        ((ISupportInitialize)control).EndInit();
        form.ShowDialog();
    }
}
