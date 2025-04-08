' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System

Option Strict On
Option Explicit On

Namespace Test
    Friend NotInheritable Class Program
        ''' <summary>
        ''' The main entry point for the application.
        ''' </summary>
        <STAThread()>
        Shared Sub Main()
            Dim component As New MyComponent()
        End Sub
    End Class
End Namespace
