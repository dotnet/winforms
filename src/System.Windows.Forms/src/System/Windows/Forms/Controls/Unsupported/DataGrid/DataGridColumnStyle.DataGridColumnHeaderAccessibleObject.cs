// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Runtime.InteropServices;

namespace System.Windows.Forms;

#nullable disable

public partial class DataGridColumnStyle
{
    [ComVisible(true)]
    [Obsolete(
        Obsoletions.DataGridMessage,
        error: false,
        DiagnosticId = Obsoletions.DataGridDiagnosticId,
        UrlFormat = Obsoletions.SharedUrlFormat)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected class DataGridColumnHeaderAccessibleObject : AccessibleObject
    {
        public DataGridColumnHeaderAccessibleObject(DataGridColumnStyle owner) =>
            throw new PlatformNotSupportedException();

        public DataGridColumnHeaderAccessibleObject() => throw new PlatformNotSupportedException();

        protected DataGridColumnStyle Owner => throw new PlatformNotSupportedException();
    }
}
