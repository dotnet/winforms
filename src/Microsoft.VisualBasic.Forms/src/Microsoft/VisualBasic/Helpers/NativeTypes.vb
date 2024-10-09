' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports Microsoft.Win32.SafeHandles

Namespace Microsoft.VisualBasic.CompilerServices

    <ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)>
    Partial Friend NotInheritable Class NativeTypes

        ' GetWindow() Constants
        Friend Const GW_HWNDFIRST As Integer = 0
        Friend Const GW_HWNDLAST As Integer = 1
        Friend Const GW_HWNDNEXT As Integer = 2
        Friend Const GW_HWNDPREV As Integer = 3
        Friend Const GW_OWNER As Integer = 4
        Friend Const GW_CHILD As Integer = 5
        Friend Const GW_MAX As Integer = 5

        Friend Const NORMAL_PRIORITY_CLASS As Integer = &H20

        Friend Const STARTF_USESHOWWINDOW As Integer = 1

        ' Handle Values
        Friend Shared ReadOnly s_invalidHandle As New IntPtr(-1)

        ''' <summary>
        '''  Inherits <see cref="SafeHandleZeroOrMinusOneIsInvalid"/>, with additional InitialSetHandle method.
        '''  This is required because call to constructor of SafeHandle is not allowed in constrained region.
        ''' </summary>
        Friend NotInheritable Class LateInitSafeHandleZeroOrMinusOneIsInvalid
            Inherits SafeHandleZeroOrMinusOneIsInvalid

            Friend Sub New()
                MyBase.New(True)
            End Sub

            Protected Overrides Function ReleaseHandle() As Boolean
                Return NativeMethods.CloseHandle(handle) <> 0
            End Function

            Friend Sub InitialSetHandle(h As IntPtr)
                Debug.Assert(IsInvalid, "Safe handle should only be set once.")
                SetHandle(h)
            End Sub

        End Class
    End Class
End Namespace
