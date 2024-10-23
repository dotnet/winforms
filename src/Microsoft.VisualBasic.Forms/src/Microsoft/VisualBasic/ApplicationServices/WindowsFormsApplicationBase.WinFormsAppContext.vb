' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Security
Imports System.Windows.Forms

Namespace Microsoft.VisualBasic.ApplicationServices

    Partial Public Class WindowsFormsApplicationBase

        ''' <summary>
        '''  Encapsulates an ApplicationContext. We have our own to get the shutdown behaviors we
        '''  offer in the application model. This derivation of the ApplicationContext listens for when
        '''  the main form closes and provides for shutting down when the main form closes or the
        '''  last form closes, depending on the mode this application is running in.
        ''' </summary>
        Private NotInheritable Class WinFormsAppContext
            Inherits ApplicationContext

            Private ReadOnly _app As WindowsFormsApplicationBase

            Public Sub New(App As WindowsFormsApplicationBase)
                _app = App
            End Sub

            ''' <summary>
            ''' <list type="number">
            '''    <item>
            '''      <description>Shutdown when the main form closes.</description>
            '''    </item>
            '''    <item>
            '''      <description>Shutdown only after the last form closes.</description>
            '''    </item>
            ''' </list>
            ''' </summary>
            ''' <param name="sender"></param>
            ''' <param name="e"></param>
            <SecuritySafeCritical()>
            Protected Overrides Sub OnMainFormClosed(sender As Object, e As EventArgs)
                If _app.ShutdownStyle = ShutdownMode.AfterMainFormCloses Then
                    MyBase.OnMainFormClosed(sender, e)
                Else ' identify a new main form so we can keep running
                    Dim forms As FormCollection = Application.OpenForms

                    If forms.Count > 0 Then
                        ' Note: Initially I used Process::MainWindowHandle to obtain an open form.
                        ' But that is bad for two reasons:
                        ' 1 - It appears to be broken and returns NULL sometimes even when
                        '     there is still a window around WinForms people are looking at that issue.
                        ' 2 - It returns the first window it hits from enum thread windows,
                        '     which is not necessarily a windows forms form, so that doesn't help
                        '     us even if it did work all the time. So I'll use one of our open forms.
                        '     We may not necessarily get a visible form here but that's OK.
                        '     Some apps may run on an invisible window and we need to keep them going
                        '     until all windows close.
                        MainForm = forms(0)
                    Else
                        MyBase.OnMainFormClosed(sender, e)
                    End If
                End If
            End Sub

        End Class
    End Class
End Namespace
