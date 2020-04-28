' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Namespace Microsoft.VisualBasic.CompilerServices

    <ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)>
    Public Interface IVbHost
        Function GetParentWindow() As System.Windows.Forms.IWin32Window
        Function GetWindowTitle() As String
    End Interface

    <ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)>
    Public NotInheritable Class HostServices

#Disable Warning IDE0032 ' Use auto property, Justification:=<Public API>
        Private Shared s_host As IVbHost
#Enable Warning IDE0032 ' Use auto property

        Public Shared Property VBHost() As IVbHost
            Get
                Return s_host
            End Get

            Set(Value As IVbHost)
                s_host = Value
            End Set
        End Property

    End Class

End Namespace


