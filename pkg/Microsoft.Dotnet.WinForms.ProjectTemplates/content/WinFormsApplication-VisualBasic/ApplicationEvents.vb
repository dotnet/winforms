Imports Microsoft.VisualBasic.ApplicationServices

Namespace My
    ' The following events are available for MyApplication:
    ' Startup: Raised when the application starts, before the startup form is created.
    ' Shutdown: Raised after all application forms are closed.  This event is not raised if the application terminates abnormally.
    ' UnhandledException: Raised if the application encounters an unhandled exception.
    ' StartupNextInstance: Raised when launching a single-instance application and the application is already active. 
    ' NetworkAvailabilityChanged: Raised when the network connection is connected or disconnected.

    ' **NEW** ApplyHighDpiMode: Raised when the application queries the HighDpiMode to set it for the application.

    ' Example:

    ' Private Sub MyApplication_ApplyHighDpiMode(sender As Object, e As ApplyHighDpiModeEventArgs) Handles Me.ApplyHighDpiMode
    '     e.HighDpiMode = HighDpiMode.PerMonitorV2
    ' End Sub

    Partial Friend Class MyApplication

    End Class
End Namespace
