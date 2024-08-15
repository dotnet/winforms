' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Runtime.InteropServices

Namespace Microsoft.VisualBasic.CompilerServices

    Partial Friend NotInheritable Class NativeTypes

#Disable Warning CA1812 ' Supress warning as this is a type used in PInvoke and shouldn't be changed.

        ''' <summary>
        '''  Important!  This class should be used where the API being called has allocated the strings. That is why lpReserved, etc. are declared as IntPtrs instead
        '''  of Strings - so that the marshaling layer won't release the memory. This caused us problems in the shell() functions. We would call GetStartupInfo()
        '''  which doesn't expect the memory for the strings to be freed. But because the strings were previously defined as type String, the marshaller would
        '''  and we got memory corruption problems detectable while running AppVerifier.
        '''  If you use this structure with an API like CreateProcess() then you are supplying the strings so you'll need another version of this class that defines lpReserved, etc.
        '''  as String so that the memory will get cleaned up.
        ''' </summary>
        <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Auto)>
        Friend NotInheritable Class STARTUPINFO
            Implements IDisposable

            Public cb As Integer
            Public lpReserved As IntPtr = IntPtr.Zero 'not string - see summary
            Public lpDesktop As IntPtr = IntPtr.Zero 'not string - see summary
            Public lpTitle As IntPtr = IntPtr.Zero 'not string - see summary
            Public dwX As Integer
            Public dwY As Integer
            Public dwXSize As Integer
            Public dwYSize As Integer
            Public dwXCountChars As Integer
            Public dwYCountChars As Integer
            Public dwFillAttribute As Integer
            Public dwFlags As Integer
            Public wShowWindow As Short
            Public cbReserved2 As Short
            Public lpReserved2 As IntPtr = IntPtr.Zero
            Public hStdInput As IntPtr = IntPtr.Zero
            Public hStdOutput As IntPtr = IntPtr.Zero
            Public hStdError As IntPtr = IntPtr.Zero

            Friend Sub New()
            End Sub

            ' To detect redundant calls. Default initialize = False.
            Private _hasBeenDisposed As Boolean

            Protected Overrides Sub Finalize()
                Dispose(False)
            End Sub

            ' IDisposable
            Private Sub Dispose(disposing As Boolean)
                If Not _hasBeenDisposed Then
                    If disposing Then
                        _hasBeenDisposed = True

                        ' 256 Defined in windows.h
                        Const STARTF_USESTDHANDLES As Integer = 256
                        If (dwFlags And STARTF_USESTDHANDLES) <> 0 Then
                            If hStdInput <> IntPtr.Zero AndAlso hStdInput <> s_invalidHandle Then
                                NativeMethods.CloseHandle(hStdInput)
                                hStdInput = s_invalidHandle
                            End If

                            If hStdOutput <> IntPtr.Zero AndAlso hStdOutput <> s_invalidHandle Then
                                NativeMethods.CloseHandle(hStdOutput)
                                hStdOutput = s_invalidHandle
                            End If

                            If hStdError <> IntPtr.Zero AndAlso hStdError <> s_invalidHandle Then
                                NativeMethods.CloseHandle(hStdError)
                                hStdError = s_invalidHandle
                            End If
                        End If 'Me.dwFlags and STARTF_USESTDHANDLES

                    End If
                End If
            End Sub

            ' This code correctly implements the disposable pattern.
            Friend Sub Dispose() Implements IDisposable.Dispose
                ' Do not change this code. Put cleanup code in Dispose(disposing As Boolean) above.
                Dispose(True)
                GC.SuppressFinalize(Me)
            End Sub

        End Class
    End Class
End Namespace
