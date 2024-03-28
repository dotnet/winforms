// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.Primitives;

namespace System.Windows.Forms.UITests;

public class MessageBoxTests
{
    [WinFormsFact]
    public void MessageBox_MessageBoxDialogResult_Valid()
    {
        bool defaultSwitchValue = LocalAppContextSwitches.GetCachedSwitchValue(LocalAppContextSwitches.NoClientNotificationsSwitchName);
        LocalAppContextSwitches.SetLocalAppContextSwitchValue(LocalAppContextSwitches.NoClientNotificationsSwitchName, true);
        Assert.Equal(DialogResult.None, MessageBox.Show("Testing DialogResult"));
        LocalAppContextSwitches.SetLocalAppContextSwitchValue(LocalAppContextSwitches.NoClientNotificationsSwitchName, defaultSwitchValue);
    }
}
