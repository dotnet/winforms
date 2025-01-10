// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

#nullable disable

public partial class DataGridColumnStyle
{
    [Obsolete(
        Obsoletions.DataGridMessage,
        error: false,
        DiagnosticId = Obsoletions.UnsupportedControlsDiagnosticId,
        UrlFormat = Obsoletions.SharedUrlFormat)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Browsable(false)]
    protected class CompModSwitches
    {
        public CompModSwitches()  => throw new PlatformNotSupportedException();

        public static TraceSwitch DGEditColumnEditing => throw new PlatformNotSupportedException();
    }
}
