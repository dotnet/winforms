// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

namespace System.Windows.Forms.Tests;

public partial class ClipboardTests
{
    // Isolate each STA test from state left by earlier tests or prior external clipboard activity.
    public ClipboardTests() => ClearClipboardIfSta();

    // Restore an empty clipboard so this machine-global resource does not leak state into later tests.
    public void Dispose() => ClearClipboardIfSta();

    private void ClearClipboardIfSta()
    {
        if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
        {
            Clipboard.Clear();
        }
    }
}
