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
        DiagnosticId = Obsoletions.DataGridDiagnosticId,
        UrlFormat = Obsoletions.SharedUrlFormat)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected class CompModSwitches
    {
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static TraceSwitch DGEditColumnEditing => throw new PlatformNotSupportedException();
    }
}
