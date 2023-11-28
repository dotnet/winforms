// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using TASKDIALOGCONFIG_MainIcon = Windows.Win32.UI.Controls.TASKDIALOGCONFIG._Anonymous1_e__Union;
using TASKDIALOGCONFIG_FooterIcon = Windows.Win32.UI.Controls.TASKDIALOGCONFIG._Anonymous2_e__Union;

namespace Windows.Win32.UI.Controls;

internal partial struct TASKDIALOGCONFIG
{
    [UnscopedRef]
    public ref TASKDIALOGCONFIG_MainIcon mainIcon => ref Anonymous1;

    [UnscopedRef]
    public ref TASKDIALOGCONFIG_FooterIcon footerIcon => ref Anonymous2;
}
