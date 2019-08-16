' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Imports System

Namespace Microsoft.VisualBasic.CompilerServices

    <System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)> _
    Public Interface IVbHost
        Function GetParentWindow() As System.Windows.Forms.IWin32Window
        Function GetWindowTitle() As String
    End Interface

    <System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)>
    Public NotInheritable Class HostServices

        Private Shared m_host As IVbHost

        Public Shared Property VBHost() As IVbHost
            Get
                Return m_host
            End Get

            Set(ByVal Value As IVbHost)
                m_host = Value
            End Set
        End Property

    End Class

End Namespace


