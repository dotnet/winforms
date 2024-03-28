// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

internal sealed partial class KeyboardToolTipStateMachine
{
    private enum SmEvent : byte
    {
        FocusedTool,
        LeftTool,
        InitialDelayTimerExpired, // internal
        ReshowDelayTimerExpired, // internal
        DismissTooltips, // internal
        RefocusWaitDelayExpired // internal
    }
}
