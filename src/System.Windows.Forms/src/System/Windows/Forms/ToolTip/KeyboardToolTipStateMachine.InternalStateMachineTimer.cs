// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

internal sealed partial class KeyboardToolTipStateMachine
{
    private sealed class InternalStateMachineTimer : Timer
    {
        public void ClearTimerTickHandlers() => _onTimer = null;
    }
}
