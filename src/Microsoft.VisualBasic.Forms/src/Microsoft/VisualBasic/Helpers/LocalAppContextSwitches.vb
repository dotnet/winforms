' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Imports System.Runtime.CompilerServices
Imports System.Runtime.Versioning

Namespace System.Windows.Forms.Primitives

    ' Borrowed from https://github.com/dotnet/runtime/blob/main/src/libraries/Common/src/System/LocalAppContextSwitches.Common.cs
    Friend Module LocalAppContextSwitches
        Private Const AssumeVbLogClassWasConfiguredByConfigFileName As String = "System.Windows.Forms.AssumeVbLogClassWasConfiguredByConfigFile"

        Private ReadOnly s_targetFrameworkName As FrameworkName = GetTargetFrameworkName()

        Private s_assumeVbLogClassWasConfiguredByConfigFile As Integer
        Private ReadOnly s_isNetCoreApp As Boolean = (s_targetFrameworkName?.Identifier) = ".NETCoreApp"

        Private Function GetTargetFrameworkName() As FrameworkName
            Dim targetFrameworkName As String = AppContext.TargetFrameworkName

            Return If(targetFrameworkName Is Nothing, Nothing, New FrameworkName(targetFrameworkName))
        End Function

        Public ReadOnly Property AssumeVbLogClassWasConfiguredByConfigFile() As Boolean
            Get
                Return GetSwitchValue(AssumeVbLogClassWasConfiguredByConfigFileName, s_assumeVbLogClassWasConfiguredByConfigFile)
            End Get
        End Property

        ' Currently not used, we may need this in the future.
        Private Function GetCachedSwitchValue(switchName As String, cachedSwitchValue As Integer) As Boolean
            If cachedSwitchValue < 0 Then
                Return False
            End If

            If cachedSwitchValue > 0 Then
                Return True
            End If

            Return GetSwitchValue(switchName, cachedSwitchValue)
        End Function

        Private Function GetSwitchValue(ByVal switchName As String, ByRef cachedSwitchValue As Integer) As Boolean
            Dim isSwitchEnabled As Boolean
            Dim hasSwitch As Boolean = AppContext.TryGetSwitch(switchName, isSwitchEnabled)

            If Not hasSwitch Then
                isSwitchEnabled = GetSwitchDefaultValue(switchName)
            End If

            ' Is caching of the switches disabled?
            Dim disableCaching As Boolean
            AppContext.TryGetSwitch("TestSwitch.LocalAppContext.DisableCaching", disableCaching)

            If Not disableCaching Then
                cachedSwitchValue = If(isSwitchEnabled, 1, -1)
            End If

            Return isSwitchEnabled

        End Function

        Private Function GetSwitchDefaultValue(switchName As String) As Boolean

            If Not s_isNetCoreApp Then
                Return False
            End If

            ' Future versions of .NET may introduce more switches basing on other dependencies here.
            Return False

        End Function
    End Module
End Namespace
