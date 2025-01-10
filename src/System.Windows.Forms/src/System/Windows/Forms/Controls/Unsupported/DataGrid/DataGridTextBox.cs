// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Runtime.InteropServices;

namespace System.Windows.Forms;

#nullable disable

[Obsolete(
    Obsoletions.DataGridMessage,
    error: false,
    DiagnosticId = Obsoletions.UnsupportedControlsDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat)]
[EditorBrowsable(EditorBrowsableState.Never)]
[Browsable(false)]
[ComVisible(true)]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ToolboxItem(false)]
[DesignTimeVisible(false)]
[DefaultProperty("GridEditName")]
public class DataGridTextBox : TextBox
{
    public DataGridTextBox() => throw new PlatformNotSupportedException();

    public void SetDataGrid(DataGrid parentGrid) { }

    public bool IsInEditOrNavigateMode
    {
        get => throw null;
        set { }
    }
}
