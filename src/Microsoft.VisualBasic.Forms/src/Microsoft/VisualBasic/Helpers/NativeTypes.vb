' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Namespace Microsoft.VisualBasic.CompilerServices

    <ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)>
    Partial Friend NotInheritable Class NativeTypes

        ' GetWindow() Constants
        Friend Const GW_HWNDFIRST As Integer = 0
        Friend Const GW_HWNDLAST As Integer = 1
        Friend Const GW_HWNDNEXT As Integer = 2
        Friend Const GW_HWNDPREV As Integer = 3
        Friend Const GW_OWNER As Integer = 4
        Friend Const GW_MAX As Integer = 5
        Friend Const GW_CHILD As Integer = 5

        Friend Const NORMAL_PRIORITY_CLASS As Integer = &H20

        Friend Const STARTF_USESHOWWINDOW As Integer = 1

    End Class
End Namespace
