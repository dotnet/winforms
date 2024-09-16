// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.UITests;

public class MessageBoxTests
{
    [WinFormsFact]
    public void MessageBox_MessageBoxDialogResult_Valid()
    {
        using NoClientNotificationsScope scope = new(enable: true);
        Assert.Equal(DialogResult.None, MessageBox.Show("Testing DialogResult"));
    }
}
