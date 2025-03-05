// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public partial class CurrencyManager
{
    [Flags]
    private enum CurrencyManagerStates : byte
    {
        Bound                           = 0b0000_0001,
        ShouldBind                      = 0b0000_0010,
        PullingData                     = 0b0000_0100,
        InChangeRecordState             = 0b0000_1000,
        SuspendPushDataInCurrentChanged = 0b0001_0000,
    }
}
