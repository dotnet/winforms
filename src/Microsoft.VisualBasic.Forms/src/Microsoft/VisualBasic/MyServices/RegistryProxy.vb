' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.ComponentModel
Imports Microsoft.Win32

Namespace Microsoft.VisualBasic.MyServices

    ''' <summary>
    '''  An extremely thin wrapper around Microsoft.Win32.Registry to expose the type through My.
    ''' </summary>
    <EditorBrowsable(EditorBrowsableState.Never)>
    Public Class RegistryProxy

        ''' <summary>
        '''  Proxy class can only created by internal classes.
        ''' </summary>
        Friend Sub New()
        End Sub

        ''' <inheritdoc cref="Registry.ClassesRoot"/>
        Public ReadOnly Property ClassesRoot() As RegistryKey
            Get
                Return Registry.ClassesRoot
            End Get
        End Property

        ''' <inheritdoc cref="Registry.CurrentConfig"/>
        Public ReadOnly Property CurrentConfig() As RegistryKey
            Get
                Return Registry.CurrentConfig
            End Get
        End Property

        ''' <inheritdoc cref="Registry.CurrentUser"/>
        Public ReadOnly Property CurrentUser() As RegistryKey
            Get
                Return Registry.CurrentUser
            End Get
        End Property

        ''' <inheritdoc cref="Registry.LocalMachine"/>
        Public ReadOnly Property LocalMachine() As RegistryKey
            Get
                Return Registry.LocalMachine
            End Get
        End Property

        ''' <inheritdoc cref="Registry.PerformanceData"/>
        Public ReadOnly Property PerformanceData() As RegistryKey
            Get
                Return Registry.PerformanceData
            End Get
        End Property

        ''' <inheritdoc cref="Registry.Users"/>
        Public ReadOnly Property Users() As RegistryKey
            Get
                Return Registry.Users
            End Get
        End Property

        ''' <inheritdoc cref="Registry.GetValue(String, String, Object)"/>
        Public Function GetValue(keyName As String, valueName As String, defaultValue As Object) As Object

            Return Registry.GetValue(keyName, valueName, defaultValue)
        End Function

        ''' <inheritdoc cref="Registry.SetValue(String, String, Object)"/>
        Public Sub SetValue(keyName As String, valueName As String, value As Object)
            Registry.SetValue(keyName, valueName, value)
        End Sub

        ''' <inheritdoc cref="Registry.SetValue(String, String, Object, RegistryValueKind)"/>
        Public Sub SetValue(
            keyName As String,
            valueName As String,
            value As Object,
            valueKind As RegistryValueKind)

            Registry.SetValue(keyName, valueName, value, valueKind)
        End Sub

    End Class
End Namespace
