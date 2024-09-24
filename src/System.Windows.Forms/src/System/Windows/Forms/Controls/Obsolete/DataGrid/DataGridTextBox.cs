// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

[Obsolete(
    Obsoletions.DataGridMessage,
    error: false,
    DiagnosticId = Obsoletions.DataGridDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat)]
[EditorBrowsable(EditorBrowsableState.Never)]
public class DataGridTextBox : TextBox
{
    public DataGridTextBox() => throw new PlatformNotSupportedException();

    public void SetDataGrid(DataGrid parentGrid) => throw new PlatformNotSupportedException();

#pragma warning disable CS0114 // Member hides inherited member; missing override keyword
    protected void WndProc(ref Message m) => throw new PlatformNotSupportedException();

    protected void OnMouseWheel(MouseEventArgs e) => throw new PlatformNotSupportedException();

    protected void OnKeyPress(KeyPressEventArgs e) => throw new PlatformNotSupportedException();

    protected internal bool ProcessKeyMessage(ref Message m) => throw new PlatformNotSupportedException();
#pragma warning restore CS0114

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool IsInEditOrNavigateMode
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }
}
