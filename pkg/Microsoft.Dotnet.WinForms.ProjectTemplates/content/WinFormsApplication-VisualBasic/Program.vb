Friend Module Program

    ' Start and set HighDpiMode, Styles and TextRenderingDefault.
    <STAThread()>
    Friend Sub Main(args As String())
        Application.SetHighDpiMode(HighDpiMode.SystemAware)
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)
        Application.Run(New Form1)
    End Sub

End Module
