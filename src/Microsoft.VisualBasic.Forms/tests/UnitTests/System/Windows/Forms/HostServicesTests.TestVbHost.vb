' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Windows.Forms
Imports FluentAssertions
Imports Microsoft.VisualBasic.CompilerServices
Imports Xunit

Namespace Microsoft.VisualBasic.Forms.Tests

    Partial Public Class HostServicesTests
        Private Shared ReadOnly s_control As New Control()
        Private Shared ReadOnly s_title As String = GetUniqueText()

        Private NotInheritable Class TestVbHost
            Implements IVbHost

            Public Function GetParentWindow() As IWin32Window Implements IVbHost.GetParentWindow
                Return s_control
            End Function

            Public Function GetWindowTitle() As String Implements IVbHost.GetWindowTitle
                Return s_title
            End Function

        End Class
    End Class
End Namespace
