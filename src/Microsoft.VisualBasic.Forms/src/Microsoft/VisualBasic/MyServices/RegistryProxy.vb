' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Imports Microsoft.Win32
Imports System.ComponentModel

Namespace Microsoft.VisualBasic.MyServices

    ''' <summary>
    ''' An extremely thin wrapper around Microsoft.Win32.Registry to expose the type through My.
    ''' </summary>
    <EditorBrowsable(EditorBrowsableState.Never)>
    Public Class RegistryProxy

        Public ReadOnly Property CurrentUser() As RegistryKey
            Get
                Return Registry.CurrentUser
            End Get
        End Property

        Public ReadOnly Property LocalMachine() As RegistryKey
            Get
                Return Registry.LocalMachine
            End Get
        End Property

        Public ReadOnly Property ClassesRoot() As RegistryKey
            Get
                Return Registry.ClassesRoot
            End Get
        End Property

        Public ReadOnly Property Users() As RegistryKey
            Get
                Return Registry.Users
            End Get
        End Property

        Public ReadOnly Property PerformanceData() As RegistryKey
            Get
                Return Registry.PerformanceData
            End Get
        End Property

        Public ReadOnly Property CurrentConfig() As RegistryKey
            Get
                Return Registry.CurrentConfig
            End Get
        End Property

        Public Function GetValue(keyName As String, valueName As String,
            defaultValue As Object) As Object

            Return Registry.GetValue(keyName, valueName, defaultValue)
        End Function

        Public Sub SetValue(keyName As String, valueName As String, value As Object)
            Registry.SetValue(keyName, valueName, value)
        End Sub

        Public Sub SetValue(keyName As String, valueName As String, value As Object,
            valueKind As RegistryValueKind)

            Registry.SetValue(keyName, valueName, value, valueKind)
        End Sub

        ''' <summary>
        ''' Proxy class can only created by internal classes.
        ''' </summary>
        Friend Sub New()
        End Sub

    End Class
End Namespace

