// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

#nullable disable

public partial class DataGrid
{
    [Obsolete(
        Obsoletions.DataGridMessage,
        error: false,
        DiagnosticId = Obsoletions.DataGridDiagnosticId,
        UrlFormat = Obsoletions.SharedUrlFormat)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class HitTestInfo
    {
        private HitTestInfo() { }

        public static readonly HitTestInfo Nowhere;

        public int Column => throw new PlatformNotSupportedException();

        public int Row => throw new PlatformNotSupportedException();

        public HitTestType Type => throw new PlatformNotSupportedException();
    }
}
