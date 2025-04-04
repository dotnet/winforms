' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System
Imports System.Windows.Forms

Namespace MyApplication
    Friend NotInheritable Class Program
        ''' <summary>
        ''' The main entry point for the application.
        ''' </summary>
        <STAThread()>
        Shared Sub Main()
            Application.EnableVisualStyles()
            Application.SetCompatibleTextRenderingDefault(False)
            Application.SetHighDpiMode(HighDpiMode.SystemAware)
            Application.Run(New TestNamespace.MyForm())
        End Sub
    End Class
End Namespace
