// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

public partial class DataGrid
{
    [Obsolete(
        Obsoletions.DataGridHitTestInfoMessage,
        error: false,
        DiagnosticId = Obsoletions.DataGridHitTestInfoDiagnosticId,
        UrlFormat = Obsoletions.SharedUrlFormat),
        EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class HitTestInfo
    {
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int Column => throw new PlatformNotSupportedException();

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int Row => throw new PlatformNotSupportedException();

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public HitTestType Type => throw new PlatformNotSupportedException();

#pragma warning disable CS0114 // Member hides inherited member; missing override keyword
        public bool Equals(object obj) => throw new PlatformNotSupportedException();
#pragma warning restore CS0114 // Member hides inherited member; missing override keyword

#pragma warning disable CS0114 // Member hides inherited member; missing override keyword
        public int GetHashCode() => throw new PlatformNotSupportedException();
#pragma warning restore CS0114 // Member hides inherited member; missing override keyword

#pragma warning disable CS0114 // Member hides inherited member; missing override keyword
        public string ToString() => throw new PlatformNotSupportedException();
#pragma warning restore CS0114 // Member hides inherited member; missing override keyword
    }
}
