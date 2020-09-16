' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Security
Imports System.Text
Imports System.Threading
Imports Microsoft.VisualBasic.CompilerServices
Imports Microsoft.VisualBasic.CompilerServices.ExceptionUtils
Imports Microsoft.VisualBasic.CompilerServices.Utils

Namespace Microsoft.VisualBasic

    ' Helper methods invoked through reflection from Microsoft.VisualBasic.Interaction in Microsoft.VisualBasic.Core.dll. 
    ' Do not change this API without also updating that dependent module.
    Friend Module _Interaction

        Public Function Shell(PathName As String, Style As AppWinStyle, Wait As Boolean, Timeout As Integer) As Integer
            Dim StartupInfo As New NativeTypes.STARTUPINFO
            Dim ProcessInfo As New NativeTypes.PROCESS_INFORMATION
            Dim ok As Integer
            Dim safeProcessHandle As New NativeTypes.LateInitSafeHandleZeroOrMinusOneIsInvalid()
            Dim safeThreadHandle As New NativeTypes.LateInitSafeHandleZeroOrMinusOneIsInvalid()
            Dim ErrorCode As Integer = 0

            If (PathName Is Nothing) Then
                Throw New NullReferenceException(GetResourceString(SR.Argument_InvalidNullValue1, "Pathname"))
            End If

            If (Style < 0 OrElse Style > 9) Then
                Throw New ArgumentException(GetResourceString(SR.Argument_InvalidValue1, "Style"))
            End If

            NativeMethods.GetStartupInfo(StartupInfo)
            Try
                StartupInfo.dwFlags = NativeTypes.STARTF_USESHOWWINDOW  ' we want to specify the initial window style (minimized, etc) so set this bit.
                StartupInfo.wShowWindow = Style

                'We have to have unmanaged permissions to do this, so asking for path permissions would be redundant
                'Note: We are using the StartupInfo (defined in NativeTypes.StartupInfo) in CreateProcess() even though this version
                'of the StartupInfo type uses IntPtr instead of String because GetStartupInfo() above requires that version so we don't
                'free the string fields since the API manages it instead.  But its OK here because we are just passing along the memory
                'that GetStartupInfo() allocated along to CreateProcess() which just reads the string fields.

#Disable Warning SYSLIB0004 ' Type or member is obsolete
                RuntimeHelpers.PrepareConstrainedRegions()
#Enable Warning SYSLIB0004 ' Type or member is obsolete
                Try
                Finally
                    ok = NativeMethods.CreateProcess(Nothing, PathName, Nothing, Nothing, False, NativeTypes.NORMAL_PRIORITY_CLASS, Nothing, Nothing, StartupInfo, ProcessInfo)
                    If ok = 0 Then
                        ErrorCode = Marshal.GetLastWin32Error()
                    End If
                    If ProcessInfo.hProcess <> IntPtr.Zero AndAlso ProcessInfo.hProcess <> NativeTypes.INVALID_HANDLE Then
                        safeProcessHandle.InitialSetHandle(ProcessInfo.hProcess)
                    End If
                    If ProcessInfo.hThread <> IntPtr.Zero AndAlso ProcessInfo.hThread <> NativeTypes.INVALID_HANDLE Then
                        safeThreadHandle.InitialSetHandle(ProcessInfo.hThread)
                    End If
                End Try

                Try
                    If (ok <> 0) Then
                        If Wait Then
                            ' Is infinite wait okay here ?
                            ' This is okay since this is marked as requiring the HostPermission with ExternalProcessMgmt rights
                            ok = NativeMethods.WaitForSingleObject(safeProcessHandle, Timeout)

                            If ok = 0 Then 'succeeded
                                'Process ran to completion
                                Shell = 0
                            Else
                                'Wait timed out
                                Shell = ProcessInfo.dwProcessId
                            End If
                        Else
                            NativeMethods.WaitForInputIdle(safeProcessHandle, 10000)
                            Shell = ProcessInfo.dwProcessId
                        End If
                    Else
                        'Check for a win32 error access denied. If it is, make and throw the exception.
                        'If not, throw FileNotFound
                        Const ERROR_ACCESS_DENIED As Integer = 5
                        If ErrorCode = ERROR_ACCESS_DENIED Then
                            Throw VbMakeException(vbErrors.PermissionDenied)
                        End If

                        Throw VbMakeException(vbErrors.FileNotFound)
                    End If
                Finally
                    safeProcessHandle.Close() ' Close the process handle will not cause the process to stop.
                    safeThreadHandle.Close()
                End Try
            Finally
                StartupInfo.Dispose()
            End Try
        End Function

        Public Sub AppActivateByProcessId(ProcessId As Integer)
            'As an optimization, we will only check the UI permission once we actually know we found the app to activate - we'll do that in AppActivateHelper

            Dim ProcessIdOwningWindow As Integer
            'Note, a process can have multiple windows.  What we want to do is dig through to find one
            'that we can actually activate.  So first ignore all the ones that are not visible and don't support mouse
            'or keyboard input
            Dim WindowHandle As IntPtr = NativeMethods.GetWindow(NativeMethods.GetDesktopWindow(), NativeTypes.GW_CHILD)

            Do While (IntPtr.op_Inequality(WindowHandle, IntPtr.Zero))
                SafeNativeMethods.GetWindowThreadProcessId(WindowHandle, ProcessIdOwningWindow)
                If (ProcessIdOwningWindow = ProcessId) AndAlso SafeNativeMethods.IsWindowEnabled(WindowHandle) AndAlso SafeNativeMethods.IsWindowVisible(WindowHandle) Then
                    Exit Do 'We found a window belonging to the desired process that we can actually activate and will support user input
                End If

                'keep rummaging through windows looking for one that belongs to the process we are after
                WindowHandle = NativeMethods.GetWindow(WindowHandle, NativeTypes.GW_HWNDNEXT)
            Loop

            'If we didn't find a window during the pass above, try the less desirable route of finding any window that belongs to the process
            If IntPtr.op_Equality(WindowHandle, IntPtr.Zero) Then
                WindowHandle = NativeMethods.GetWindow(NativeMethods.GetDesktopWindow(), NativeTypes.GW_CHILD)

                Do While IntPtr.op_Inequality(WindowHandle, IntPtr.Zero)
                    SafeNativeMethods.GetWindowThreadProcessId(WindowHandle, ProcessIdOwningWindow)
                    If (ProcessIdOwningWindow = ProcessId) Then
                        Exit Do
                    End If

                    'keep rummaging through windows looking for one that belongs to the process we are after
                    WindowHandle = NativeMethods.GetWindow(WindowHandle, NativeTypes.GW_HWNDNEXT)
                Loop
            End If

            If IntPtr.op_Equality(WindowHandle, IntPtr.Zero) Then 'we never found a window belonging to the desired process
                Throw New ArgumentException(GetResourceString(SR.ProcessNotFound, CStr(ProcessId)))
            Else
                AppActivateHelper(WindowHandle, CStr(ProcessId))
            End If
        End Sub

        Public Sub AppActivateByTitle(Title As String)
            'As an optimization, we will only check the UI permission once we actually know we found the app to activate - we'll do that in AppActivateHelper
            Dim WindowHandle As IntPtr = NativeMethods.FindWindow(Nothing, Title) 'see if we can find the window using an exact match on the title
            Const MAX_TITLE_LENGTH As Integer = 511

            '  if no match, search through all parent windows
            If IntPtr.op_Equality(WindowHandle, IntPtr.Zero) Then
                Dim AppTitle As String
                ' Old implementation uses MAX_TITLE_LENGTH characters, INCLUDING NULL character.
                ' Interop code will extend string builder to handle NULL character.
                Dim AppTitleBuilder As New StringBuilder(MAX_TITLE_LENGTH)
                Dim AppTitleLength As Integer
                Dim TitleLength As Integer = Len(Title)

                'Loop through all children of the desktop
                WindowHandle = NativeMethods.GetWindow(NativeMethods.GetDesktopWindow(), NativeTypes.GW_CHILD)
                Do While IntPtr.op_Inequality(WindowHandle, IntPtr.Zero)
                    '  get the window caption and test for a left-aligned substring
                    AppTitleLength = NativeMethods.GetWindowText(WindowHandle, AppTitleBuilder, AppTitleBuilder.Capacity)
                    AppTitle = AppTitleBuilder.ToString()

                    If AppTitleLength >= TitleLength Then
                        If String.Compare(AppTitle, 0, Title, 0, TitleLength, StringComparison.OrdinalIgnoreCase) = 0 Then
                            Exit Do 'found one
                        End If
                    End If

                    'keep looking
                    WindowHandle = NativeMethods.GetWindow(WindowHandle, NativeTypes.GW_HWNDNEXT)
                Loop

                If IntPtr.op_Equality(WindowHandle, IntPtr.Zero) Then
                    ' We didn't find it so try right aligned
                    WindowHandle = NativeMethods.GetWindow(NativeMethods.GetDesktopWindow(), NativeTypes.GW_CHILD)

                    Do While IntPtr.op_Inequality(WindowHandle, IntPtr.Zero)
                        '  get the window caption and test for a right-aligned substring
                        AppTitleLength = NativeMethods.GetWindowText(WindowHandle, AppTitleBuilder, AppTitleBuilder.Capacity)
                        AppTitle = AppTitleBuilder.ToString()

                        If AppTitleLength >= TitleLength Then
                            If String.Compare(Right(AppTitle, TitleLength), 0, Title, 0, TitleLength, StringComparison.OrdinalIgnoreCase) = 0 Then
                                Exit Do 'found a match
                            End If
                        End If

                        'keep looking
                        WindowHandle = NativeMethods.GetWindow(WindowHandle, NativeTypes.GW_HWNDNEXT)
                    Loop
                End If
            End If

            If IntPtr.op_Equality(WindowHandle, IntPtr.Zero) Then 'no match
                Throw New ArgumentException(GetResourceString(SR.ProcessNotFound, Title))
            Else
                AppActivateHelper(WindowHandle, Title)
            End If
        End Sub

        Private Sub AppActivateHelper(hwndApp As IntPtr, ProcessId As String)
            '  if no window with name (full or truncated) or task id, return an error
            '  if the window is not enabled or not visible, get the first window owned by it that is not enabled or not visible
            Dim hwndOwned As IntPtr
            If (Not SafeNativeMethods.IsWindowEnabled(hwndApp) OrElse Not SafeNativeMethods.IsWindowVisible(hwndApp)) Then
                '  scan to the next window until failure
                hwndOwned = NativeMethods.GetWindow(hwndApp, NativeTypes.GW_HWNDFIRST)
                Do While IntPtr.op_Inequality(hwndOwned, IntPtr.Zero)
                    If IntPtr.op_Equality(NativeMethods.GetWindow(hwndOwned, NativeTypes.GW_OWNER), hwndApp) Then
                        If (Not SafeNativeMethods.IsWindowEnabled(hwndOwned) OrElse Not SafeNativeMethods.IsWindowVisible(hwndOwned)) Then
                            hwndApp = hwndOwned
                            hwndOwned = NativeMethods.GetWindow(hwndApp, NativeTypes.GW_HWNDFIRST)
                        Else
                            Exit Do
                        End If
                    End If
                    hwndOwned = NativeMethods.GetWindow(hwndOwned, NativeTypes.GW_HWNDNEXT)
                Loop

                '  if scan failed, return an error
                If IntPtr.op_Equality(hwndOwned, IntPtr.Zero) Then
                    Throw New ArgumentException(GetResourceString(SR.ProcessNotFound, ProcessId))
                End If

                '  set active window to the owned one
                hwndApp = hwndOwned
            End If

            ' SetActiveWindow on Win32 only activates the Window - it does
            ' not bring to to the foreground unless the window belongs to
            ' the current thread.  NativeMethods.SetForegroundWindow() activates the
            ' window, moves it to the foreground, and bumps the priority
            ' of the thread which owns the window.

            Dim dwDummy As Integer ' dummy arg for SafeNativeMethods.GetWindowThreadProcessId

            ' Attach ourselves to the window we want to set focus to
            NativeMethods.AttachThreadInput(0, SafeNativeMethods.GetWindowThreadProcessId(hwndApp, dwDummy), 1)
            ' Make it foreground and give it focus, this will occur
            ' synchronously because we are attached.
            NativeMethods.SetForegroundWindow(hwndApp)
            NativeMethods.SetFocus(hwndApp)
            ' Unattached ourselves from the window
            NativeMethods.AttachThreadInput(0, SafeNativeMethods.GetWindowThreadProcessId(hwndApp, dwDummy), 0)
        End Sub

        Private NotInheritable Class InputBoxHandler
            Private ReadOnly _prompt As String
            Private ReadOnly _title As String
            Private ReadOnly _defaultResponse As String
            Private ReadOnly _xPos As Integer
            Private ReadOnly _yPos As Integer
            Private _result As String
            Private ReadOnly _parentWindow As Windows.Forms.IWin32Window
            Private _exception As Exception

            Sub New(Prompt As String, Title As String, DefaultResponse As String, XPos As Integer, YPos As Integer, ParentWindow As Windows.Forms.IWin32Window)
                _prompt = Prompt
                _title = Title
                _defaultResponse = DefaultResponse
                _xPos = XPos
                _yPos = YPos
                _parentWindow = ParentWindow
            End Sub

            Public Sub StartHere()
                Try
                    _result = InternalInputBox(_prompt, _title, _defaultResponse, _xPos, _yPos, _parentWindow)
                Catch ex As Exception
                    _exception = ex
                End Try
            End Sub

            Public ReadOnly Property Result() As String
                Get
                    Return _result
                End Get
            End Property

            Friend ReadOnly Property Exception As Exception
                Get
                    Return _exception
                End Get
            End Property
        End Class

        Public Function InputBox(Prompt As String, Title As String, DefaultResponse As String, XPos As Integer, YPos As Integer) As String
            Dim vbhost As IVbHost
            Dim ParentWindow As Windows.Forms.IWin32Window = Nothing

            vbhost = CompilerServices.HostServices.VBHost
            If vbhost IsNot Nothing Then 'If we are hosted then we want to use the host as the parent window.  If no parent window that's fine.
                ParentWindow = vbhost.GetParentWindow()
            End If

            If String.IsNullOrEmpty(Title) Then
                If vbhost Is Nothing Then
                    Title = GetTitleFromAssembly(System.Reflection.Assembly.GetCallingAssembly())
                Else
                    Title = vbhost.GetWindowTitle()
                End If
            End If

            'Threading state can only be set once, and will most often be already set
            'but set to STA and check if it isn't STA, then we need to start another thread
            'to display the InputBox
            If System.Threading.Thread.CurrentThread.GetApartmentState() <> Threading.ApartmentState.STA Then
                Dim InputHandler As New InputBoxHandler(Prompt, Title, DefaultResponse, XPos, YPos, ParentWindow)
                Dim thread As New Thread(New ThreadStart(AddressOf InputHandler.StartHere))
                thread.Start()
                thread.Join()

                If InputHandler.Exception IsNot Nothing Then
                    Throw InputHandler.Exception
                End If

                Return InputHandler.Result
            Else
                Return InternalInputBox(Prompt, Title, DefaultResponse, XPos, YPos, ParentWindow)
            End If
        End Function

        Private Function GetTitleFromAssembly(CallingAssembly As Reflection.Assembly) As String

            Dim Title As String

            'Get the Assembly name of the calling assembly 
            'Assembly.GetName requires PathDiscovery permission so we try this first
            'and if it throws we catch the security exception and parse the name
            'from the full assembly name
            Try
                Title = CallingAssembly.GetName().Name
            Catch ex As SecurityException
                Dim FullName As String = CallingAssembly.FullName

                'Find the text up to the first comma. Note, this fails if the assembly has
                'a comma in its name
                Dim FirstCommaLocation As Integer = FullName.IndexOf(","c)
                If FirstCommaLocation >= 0 Then
                    Title = FullName.Substring(0, FirstCommaLocation)
                Else
                    'The name is not in the format we're expecting so return an empty string
                    Title = ""
                End If
            End Try

            Return Title

        End Function

        Private Function InternalInputBox(Prompt As String, Title As String, DefaultResponse As String, XPos As Integer, YPos As Integer, ParentWindow As Windows.Forms.IWin32Window) As String
            Dim Box As VBInputBox = New VBInputBox(Prompt, Title, DefaultResponse, XPos, YPos)
            Box.ShowDialog(ParentWindow)

            InternalInputBox = Box.Output
            Box.Dispose()
        End Function

        Public Function MsgBox(Prompt As Object, Buttons As MsgBoxStyle, Title As Object) As MsgBoxResult
            Dim sPrompt As String = Nothing
            Dim sTitle As String
            Dim vbhost As IVbHost
            Dim ParentWindow As Windows.Forms.IWin32Window = Nothing

            vbhost = CompilerServices.HostServices.VBHost
            If Not vbhost Is Nothing Then
                ParentWindow = vbhost.GetParentWindow()
            End If

            'Only allow legal button combinations to be set, one choice from each group
            'These bit constants are defined in System.Windows.Forms.MessageBox
            'Low-order 4 bits (0x000f), legal values: 0, 1, 2, 3, 4, 5
            '     next 4 bits (0x00f0), legal values: 0, &H10, &H20, &H30, &H40
            '     next 4 bits (0x0f00), legal values: 0, &H100, &H200
            If ((Buttons And &HFI) > MsgBoxStyle.RetryCancel) OrElse ((Buttons And &HF0I) > MsgBoxStyle.Information) _
                OrElse ((Buttons And &HF00I) > MsgBoxStyle.DefaultButton3) Then
                Buttons = MsgBoxStyle.OkOnly
            End If

            Try
                If Not Prompt Is Nothing Then
                    sPrompt = DirectCast(Conversions.ChangeType(Prompt, GetType(String)), String)
                End If
            Catch ex As StackOverflowException
                Throw
            Catch ex As OutOfMemoryException
                Throw
            Catch ex As ThreadAbortException
                Throw
            Catch
                Throw New ArgumentException(GetResourceString(SR.Argument_InvalidValueType2, "Prompt", "String"))
            End Try

            Try
                If Title Is Nothing Then
                    If vbhost Is Nothing Then
                        sTitle = GetTitleFromAssembly(System.Reflection.Assembly.GetCallingAssembly())
                    Else
                        sTitle = vbhost.GetWindowTitle()
                    End If
                Else
                    sTitle = CStr(Title) 'allows the title to be an expression, e.g. msgbox(prompt, Title:=1+5)
                End If
            Catch ex As StackOverflowException
                Throw
            Catch ex As OutOfMemoryException
                Throw
            Catch ex As ThreadAbortException
                Throw
            Catch
                Throw New ArgumentException(GetResourceString(SR.Argument_InvalidValueType2, "Title", "String"))
            End Try

            Return CType(System.Windows.Forms.MessageBox.Show(ParentWindow, sPrompt, sTitle,
                 CType(Buttons And &HF, Windows.Forms.MessageBoxButtons),
                 CType(Buttons And &HF0, Windows.Forms.MessageBoxIcon),
                 CType(Buttons And &HF00, Windows.Forms.MessageBoxDefaultButton),
                 CType(Buttons And &HFFFFF000, Windows.Forms.MessageBoxOptions)),
                 MsgBoxResult)
        End Function

    End Module

End Namespace

