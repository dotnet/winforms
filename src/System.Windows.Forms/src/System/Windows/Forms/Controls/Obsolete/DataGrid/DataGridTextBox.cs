// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

#pragma warning disable RS0016 // Add public types and members to the declared API to simplify porting of applications from .NET Framework to .NET.
// These types will not work, but if they are not accessed, other features in the application will work.
[Obsolete(
    Obsoletions.DataGridTextBoxMessage,
    error: false,
    DiagnosticId = Obsoletions.DataGridTextBoxDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat)]
public class DataGridTextBox : TextBox
{
    public DataGridTextBox() : base()
        => throw new PlatformNotSupportedException();

    public void SetDataGrid(DataGrid parentGrid)
        => throw new PlatformNotSupportedException();

    protected override void WndProc(ref Message m)
        => throw new PlatformNotSupportedException();

    protected override void OnMouseWheel(MouseEventArgs e)
    {
    }

    protected override void OnKeyPress(KeyPressEventArgs e)
        => throw new PlatformNotSupportedException();

    protected internal override bool ProcessKeyMessage(ref Message m)
        => throw new PlatformNotSupportedException();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool IsInEditOrNavigateMode
    {
        get => throw new PlatformNotSupportedException();
        set => throw new PlatformNotSupportedException();
    }
}
