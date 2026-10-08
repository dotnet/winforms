// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public partial class ErrorProvider
{
    private enum ErrorProviderStates : byte
    {
        ShowIcon                 = 0b0000_0001,
        InSetErrorManager        = 0b0000_0010,
        SetErrorManagerOnEndInit = 0b0000_0100,
        Initializing             = 0b0000_1000,
        RightToLeft              = 0b0001_0000,
    }
}
