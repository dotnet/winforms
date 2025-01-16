// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Runtime.InteropServices;

namespace System.Windows.Forms;

#nullable disable

public partial class DataGridColumnStyle
{
    /// <summary>
    ///  This type is provided for binary compatibility with .NET Framework and is not intended to be used directly from your code.
    /// </summary>
    [ComVisible(true)]
    [Obsolete(
        Obsoletions.DataGridMessage,
        error: false,
        DiagnosticId = Obsoletions.UnsupportedControlsDiagnosticId,
        UrlFormat = Obsoletions.SharedUrlFormat)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Browsable(false)]
    protected class DataGridColumnHeaderAccessibleObject : AccessibleObject
    {
        public DataGridColumnHeaderAccessibleObject(DataGridColumnStyle owner) => throw new PlatformNotSupportedException();

        public DataGridColumnHeaderAccessibleObject() => throw new PlatformNotSupportedException();

        protected DataGridColumnStyle Owner => throw null;
    }
}
