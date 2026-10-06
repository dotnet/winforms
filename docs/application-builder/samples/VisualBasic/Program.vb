' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports Microsoft.Extensions.DependencyInjection
Imports Microsoft.Extensions.Hosting
Imports Microsoft.Extensions.WinForms
Imports System.Diagnostics
Imports System.Drawing
Imports System.Threading
Imports System.Windows.Forms

Friend Module Program
    <STAThread>
    Friend Sub Main(args As String())
        Application.SetHighDpiMode(HighDpiMode.SystemAware)
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)

        Dim hostBuilder As HostApplicationBuilder = Host.CreateApplicationBuilder(args)
        hostBuilder.Services.AddHostedService(Of HeartbeatService)()
        Dim genericHost As IHost = hostBuilder.Build()

        Dim applicationBuilder As WinFormsApplicationBuilder =
            WinFormsApplication.CreateBuilder().UseHost(genericHost)

        If args.Contains("--custom-context", StringComparer.OrdinalIgnoreCase) Then
            applicationBuilder.UseApplicationContext(New MainApplicationContext())
        Else
            applicationBuilder.UseStartupForm(Of MainForm)()
        End If

        Using application As WinFormsApplication = applicationBuilder.Build()
            application.Run()
        End Using
    End Sub
End Module

Friend NotInheritable Class MainForm
    Inherits Form

    Public Sub New()
        Text = "WinForms Application Builder"
        ClientSize = New Size(520, 170)

        Dim description As New Label With {
            .AutoSize = True,
            .Location = New Point(16, 20),
            .Text = "A hosted background service writes a heartbeat each second."
        }

        Dim shutdownDescription As New Label With {
            .AutoSize = True,
            .Location = New Point(16, 50),
            .Text = "Close this window to cancel the service and stop the host gracefully."
        }

        Dim closeButton As New Button With {
            .Anchor = AnchorStyles.Bottom Or AnchorStyles.Right,
            .Location = New Point(410, 115),
            .Text = "Close"
        }
        AddHandler closeButton.Click, Sub(sender, e) Close()

        Controls.Add(description)
        Controls.Add(shutdownDescription)
        Controls.Add(closeButton)
    End Sub
End Class

Friend NotInheritable Class MainApplicationContext
    Inherits ApplicationContext

    Public Sub New()
        MyBase.New(New MainForm())
    End Sub
End Class

Friend NotInheritable Class HeartbeatService
    Inherits BackgroundService

    Protected Overrides Async Function ExecuteAsync(stoppingToken As CancellationToken) As Task
        Using timer As New PeriodicTimer(TimeSpan.FromSeconds(1))
            Try
                While Await timer.WaitForNextTickAsync(stoppingToken).ConfigureAwait(False)
                    Debug.WriteLine($"Background service heartbeat at {DateTimeOffset.Now}.")
                End While
            Catch ex As OperationCanceledException When stoppingToken.IsCancellationRequested
                Debug.WriteLine("Background service observed host shutdown.")
            End Try
        End Using
    End Function
End Class
