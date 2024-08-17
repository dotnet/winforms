' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Runtime.InteropServices
Imports System.Security
Imports System.Text
Imports System.Threading
Imports System.Windows.Forms
Imports Microsoft.VisualBasic.CompilerServices

Imports ExUtils = Microsoft.VisualBasic.CompilerServices.ExceptionUtils
Imports NativeMethods = Microsoft.VisualBasic.CompilerServices.NativeMethods

Namespace Microsoft.VisualBasic

    ' Helper methods invoked through reflection from Microsoft.VisualBasic.Interaction in Microsoft.VisualBasic.Core.dll.
    ' Do not change this API without also updating that dependent module.
    Friend Module _Interaction

        Private Sub AppActivateHelper(hwndApp As IntPtr, processId As String)
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
                    Throw New ArgumentException(ExUtils.GetResourceString(SR.ProcessNotFound, processId))
                End If

                '  set active window to the owned one
                hwndApp = hwndOwned
            End If

            ' SetActiveWindow on Win32 only activates the Window - it does
            ' not bring to to the foreground unless the window belongs to
            ' the current thread. NativeMethods.SetForegroundWindow() activates the
            ' window, moves it to the foreground, and bumps the priority
            ' of the thread which owns the window.

            Dim dwDummy As Integer ' dummy arg for SafeNativeMethods.GetWindowThreadProcessId

            ok = NativeMethods.CreateProcess(Nothing, PathName, Nothing, Nothing, False, NativeTypes.NORMAL_PRIORITY_CLASS, Nothing, Nothing, startupInfo, processInfo)
            If ok = 0 Then
                ' Make it foreground and give it focus, this will occur
                ' synchronously because we are attached.
                NativeMethods.SetForegroundWindow(hwndApp)
                NativeMethods.SetFocus(hwndApp)
                ' Unattached ourselves from the window
                NativeMethods.AttachThreadInput(0, SafeNativeMethods.GetWindowThreadProcessId(hwndApp, dwDummy), 0)
        End Sub

        Private Function GetTitleFromAssembly(callingAssembly As Reflection.Assembly) As String
            If (ok <> 0) Then
                If Wait Then
                        ' Is infinite wait okay here ?
                        ' This is okay since this is marked as requiring the HostPermission with ExternalProcessMgmt rights
                        ok = NativeMethods.WaitForSingleObject(safeProcessHandle, Timeout)

                        'Find the text up to the first comma. Note, this fails if the assembly has
                        'a comma in its name
                        Dim firstCommaLocation As Integer = fullName.IndexOf(","c)
                        If firstCommaLocation >= 0 Then
                            title = fullName.Substring(0, firstCommaLocation)
                        Else
                            'The name is not in the format we're expecting so return an empty string
                            title = String.Empty
                        End If
            End Try

            Return title

            End Function

        Private Function InternalInputBox(prompt As String, title As String, defaultResponse As String, xPos As Integer, yPos As Integer, parentWindow As IWin32Window) As String
            Dim box As VBInputBox = New VBInputBox(prompt, title, defaultResponse, xPos, yPos)
            box.ShowDialog(parentWindow)

            InternalInputBox = box.Output
            box.Dispose()
        End Function

        Public Sub AppActivateByProcessId(ProcessId As Integer)
            'As an optimization, we will only check the UI permission once we actually know we found the app to activate - we'll do that in AppActivateHelper

            Dim processIdOwningWindow As Integer
            'Note, a process can have multiple windows. What we want to do is dig through to find one
            'that we can actually activate. So first ignore all the ones that are not visible and don't support mouse
            'or keyboard input
            Dim windowHandle As IntPtr = NativeMethods.GetWindow(NativeMethods.GetDesktopWindow(), NativeTypes.GW_CHILD)

            Do While (IntPtr.op_Inequality(windowHandle, IntPtr.Zero))
                SafeNativeMethods.GetWindowThreadProcessId(windowHandle, processIdOwningWindow)
                If (processIdOwningWindow = ProcessId) AndAlso SafeNativeMethods.IsWindowEnabled(windowHandle) AndAlso SafeNativeMethods.IsWindowVisible(windowHandle) Then
                    Exit Do 'We found a window belonging to the desired process that we can actually activate and will support user input
                End If

                'keep rummaging through windows looking for one that belongs to the process we are after
                windowHandle = NativeMethods.GetWindow(windowHandle, NativeTypes.GW_HWNDNEXT)
            Loop

            'If we didn't find a window during the pass above, try the less desirable route of finding any window that belongs to the process
            If IntPtr.op_Equality(windowHandle, IntPtr.Zero) Then
                windowHandle = NativeMethods.GetWindow(NativeMethods.GetDesktopWindow(), NativeTypes.GW_CHILD)

                Do While IntPtr.op_Inequality(windowHandle, IntPtr.Zero)
                    SafeNativeMethods.GetWindowThreadProcessId(windowHandle, processIdOwningWindow)
                    If (processIdOwningWindow = ProcessId) Then
                        Exit Do
                    End If

                    'keep rummaging through windows looking for one that belongs to the process we are after
                    windowHandle = NativeMethods.GetWindow(windowHandle, NativeTypes.GW_HWNDNEXT)
                Loop
            End If

            If IntPtr.op_Equality(windowHandle, IntPtr.Zero) Then 'we never found a window belonging to the desired process
                Throw New ArgumentException(ExUtils.GetResourceString(SR.ProcessNotFound, CStr(ProcessId)))
            Else
                AppActivateHelper(windowHandle, CStr(ProcessId))
            End If
        End Sub

        Public Sub AppActivateByTitle(Title As String)
            'As an optimization, we will only check the UI permission once we actually know we found the app to activate - we'll do that in AppActivateHelper
            Dim windowHandle As IntPtr = NativeMethods.FindWindow(Nothing, Title) 'see if we can find the window using an exact match on the title
            Const MAX_TITLE_LENGTH As Integer = 511

            '  if no match, search through all parent windows
            If IntPtr.op_Equality(windowHandle, IntPtr.Zero) Then
                Dim appTitle As String
                ' Old implementation uses MAX_TITLE_LENGTH characters, INCLUDING NULL character.
                ' Interop code will extend string builder to handle NULL character.
                Dim appTitleBuilder As New StringBuilder(MAX_TITLE_LENGTH)
                Dim appTitleLength As Integer
                Dim titleLength As Integer = Len(Title)

                'Loop through all children of the desktop
                windowHandle = NativeMethods.GetWindow(NativeMethods.GetDesktopWindow(), NativeTypes.GW_CHILD)
                Do While IntPtr.op_Inequality(windowHandle, IntPtr.Zero)
                    '  get the window caption and test for a left-aligned substring
                    appTitleLength = NativeMethods.GetWindowText(windowHandle, appTitleBuilder, appTitleBuilder.Capacity)
                    appTitle = appTitleBuilder.ToString()

                    If appTitleLength >= titleLength Then
                        If String.Compare(appTitle, 0, Title, 0, titleLength, StringComparison.OrdinalIgnoreCase) = 0 Then
                            Exit Do 'found one
                        End If
                    End If

                    'keep looking
                    windowHandle = NativeMethods.GetWindow(windowHandle, NativeTypes.GW_HWNDNEXT)
                Loop

                If IntPtr.op_Equality(windowHandle, IntPtr.Zero) Then
                    ' We didn't find it so try right aligned
                    windowHandle = NativeMethods.GetWindow(NativeMethods.GetDesktopWindow(), NativeTypes.GW_CHILD)

                    Do While IntPtr.op_Inequality(windowHandle, IntPtr.Zero)
                        '  get the window caption and test for a right-aligned substring
                        appTitleLength = NativeMethods.GetWindowText(windowHandle, appTitleBuilder, appTitleBuilder.Capacity)
                        appTitle = appTitleBuilder.ToString()

                        If appTitleLength >= titleLength Then
                            If String.Compare(Right(appTitle, titleLength), 0, Title, 0, titleLength, StringComparison.OrdinalIgnoreCase) = 0 Then
                                Exit Do 'found a match
                            End If
                        End If

                        'keep looking
                        windowHandle = NativeMethods.GetWindow(windowHandle, NativeTypes.GW_HWNDNEXT)
                    Loop
                End If
            End If

            If IntPtr.op_Equality(windowHandle, IntPtr.Zero) Then 'no match
                Throw New ArgumentException(ExUtils.GetResourceString(SR.ProcessNotFound, Title))
            Else
                AppActivateHelper(windowHandle, Title)
            End If
        End Sub

        Private Sub AppActivateHelper(hwndApp As IntPtr, processId As String)
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

                    'Threading state can only be set once, and will most often be already set
                    'but set to STA and check if it isn't STA, then we need to start another thread
                    'to display the InputBox
                    If Thread.CurrentThread.GetApartmentState() <> ApartmentState.STA Then
                Dim inputHandler As New InputBoxHandler(Prompt, Title, DefaultResponse, XPos, YPos, parentWindow)
                Dim thread As New Thread(New ThreadStart(AddressOf inputHandler.StartHere))
                thread.Start()
                thread.Join()

                If inputHandler.Exception IsNot Nothing Then
                    Throw inputHandler.Exception
                End If

                Return inputHandler.Result
            Else
                Return InternalInputBox(Prompt, Title, DefaultResponse, XPos, YPos, parentWindow)
            End If
        End Function

        Public Function MsgBox(Prompt As Object, Buttons As MsgBoxStyle, Title As Object) As MsgBoxResult
            Dim sPrompt As String = Nothing
            Dim sTitle As String
            Dim vbHost As IVbHost
            Dim parentWindow As IWin32Window = Nothing

            vbHost = HostServices.VBHost
            If vbHost IsNot Nothing Then
                parentWindow = vbHost.GetParentWindow()
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
                If Prompt IsNot Nothing Then
                    sPrompt = DirectCast(Conversions.ChangeType(Prompt, GetType(String)), String)
                End If
            Catch ex As StackOverflowException
                Throw
            Catch ex As OutOfMemoryException
                Throw
            Catch ex As ThreadAbortException
                Throw
            Catch
                Throw New ArgumentException(ExUtils.GetResourceString(SR.Argument_InvalidValueType2, "Prompt", "String"))
            End Try

            Try
                If Title Is Nothing Then
                    If vbHost Is Nothing Then
                        sTitle = GetTitleFromAssembly(Reflection.Assembly.GetCallingAssembly())
                    Else
                        sTitle = vbHost.GetWindowTitle()
                    End If
                Else
                    sTitle = CStr(Title) 'allows the title to be an expression, e.g. MsgBox(prompt, Title:=1+5)
                End If
            Catch ex As StackOverflowException
                Throw
            Catch ex As OutOfMemoryException
                Throw
            Catch ex As ThreadAbortException
                Throw
            Catch
                Throw New ArgumentException(ExUtils.GetResourceString(SR.Argument_InvalidValueType2, "Title", "String"))
            End Try

            Return CType(MessageBox.Show(parentWindow, sPrompt, sTitle,
                 CType(Buttons And &HF, MessageBoxButtons),
                 CType(Buttons And &HF0, MessageBoxIcon),
                 CType(Buttons And &HF00, MessageBoxDefaultButton),
                 CType(Buttons And &HFFFFF000, MessageBoxOptions)),
                 MsgBoxResult)
        End Function

    End Module
End Namespace
