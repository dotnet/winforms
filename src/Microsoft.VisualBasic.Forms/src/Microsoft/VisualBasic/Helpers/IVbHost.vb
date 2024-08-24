' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Namespace Microsoft.VisualBasic.CompilerServices

    <ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)>
    Public Interface IVbHost

        Function GetParentWindow() As System.Windows.Forms.IWin32Window

        Function GetWindowTitle() As String

    End Interface

End Namespace
