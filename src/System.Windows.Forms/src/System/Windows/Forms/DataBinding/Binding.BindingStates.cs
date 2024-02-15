// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public partial class Binding
{
    private enum BindingStates : byte
    {
        FormattingEnabled      = 0b0000_0001,
        Modified               = 0b0000_0010,
        InSetPropValue         = 0b0000_0100,
        InPushOrPull           = 0b0000_1000,
        InOnBindingComplete    = 0b0001_0000,
        DataSourceNullValueSet = 0b0010_0000
    }
}
