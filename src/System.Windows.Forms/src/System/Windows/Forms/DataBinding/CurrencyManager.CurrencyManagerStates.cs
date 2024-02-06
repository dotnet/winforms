// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public partial class CurrencyManager
{
    [Flags]
    private enum CurrencyManagerStates : byte
    {
        Bound = 1 << 0,
        ShouldBind = 1 << 1,
        PullingData = 1 << 2,
        InChangeRecordState = 1 << 3,
        SuspendPushDataInCurrentChanged = 1 << 4,
    }
}
