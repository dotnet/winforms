' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Option Explicit On
Option Strict On

Namespace Microsoft.VisualBasic.Devices

    ''' <summary>
    '''  Used to pass network connectivity status.
    ''' </summary>
    Public Class NetworkAvailableEventArgs
        Inherits EventArgs
        Public Sub New(ByVal networkAvailable As Boolean)
            IsNetworkAvailable = networkAvailable
        End Sub

        Public ReadOnly Property IsNetworkAvailable() As Boolean
    End Class

    ''' <summary>
    '''  An object that allows easy access to some simple network properties and functionality.
    ''' </summary>
    Public Class Network

        ''' <summary>
        '''  Creates class and hooks up events
        ''' </summary>
        Public Sub New()
        End Sub

    End Class

End Namespace
