' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Option Strict On

Imports Microsoft.VisualBasic.CompilerServices.Utils

Namespace Microsoft.VisualBasic.CompilerServices

    Friend Enum vbErrors
        None = 0
        FileNotFound = 53
        PermissionDenied = 70
    End Enum

    ' Implements error utilities for Basic
    Friend NotInheritable Class ExceptionUtils

        ' Prevent creation.
        Private Sub New()
        End Sub

        Friend Shared Function VbMakeException(hr As Integer) As Exception
            Dim sMsg As String

            If hr > 0 AndAlso hr <= &HFFFFI Then
                sMsg = GetResourceString(CType(hr, vbErrors))
            Else
                sMsg = ""
            End If
            VbMakeException = VbMakeExceptionEx(hr, sMsg)
        End Function

        Friend Shared Function VbMakeExceptionEx(number As Integer, sMsg As String) As Exception
            Dim vBDefinedError As Boolean

            VbMakeExceptionEx = BuildException(number, sMsg, vBDefinedError)

            If vBDefinedError Then
                ' .NET Framework implementation calls:
                ' Err().SetUnmappedError(number)
            End If

        End Function

        Friend Shared Function BuildException(Number As Integer, Description As String, ByRef VBDefinedError As Boolean) As Exception

            VBDefinedError = True

            Select Case Number

                Case vbErrors.None

                Case vbErrors.FileNotFound
                    Return New IO.FileNotFoundException(Description)

                Case vbErrors.PermissionDenied
                    Return New IO.IOException(Description)

                Case Else
                    'Fall below to default
                    VBDefinedError = False
                    Return New Exception(Description)
            End Select

            VBDefinedError = False
            Return New Exception(Description)

        End Function

        ''' <summary>
        ''' Returns a new instance of ArgumentException with the message from resource file and the Exception.ArgumentName property set.
        ''' </summary>
        ''' <param name="ArgumentName">The name of the argument (parameter). Not localized.</param>
        ''' <param name="ResourceID">The resource ID.</param>
        ''' <param name="PlaceHolders">Strings that will replace place holders in the resource string, if any.</param>
        ''' <returns>A new instance of ArgumentException.</returns>
        ''' <remarks>This is the preferred way to construct an argument exception.</remarks>
        Friend Shared Function GetArgumentExceptionWithArgName(ArgumentName As String,
            ResourceID As String, ParamArray PlaceHolders() As String) As ArgumentException

            Return New ArgumentException(GetResourceString(ResourceID, PlaceHolders), ArgumentName)
        End Function

        ''' <summary>
        ''' Returns a new instance of ArgumentNullException with message: "Argument cannot be Nothing."
        ''' </summary>
        ''' <param name="ArgumentName">The name of the argument (parameter). Not localized.</param>
        ''' <returns>A new instance of ArgumentNullException.</returns>
        Friend Shared Function GetArgumentNullException(ArgumentName As String) As ArgumentNullException

            Return New ArgumentNullException(ArgumentName, GetResourceString(SR.General_ArgumentNullException))
        End Function

        ''' <summary>
        ''' Returns a new instance of ArgumentNullException with the message from resource file.
        ''' </summary>
        ''' <param name="ArgumentName">The name of the argument (parameter). Not localized.</param>
        ''' <param name="ResourceID">The resource ID.</param>
        ''' <param name="PlaceHolders">Strings that will replace place holders in the resource string, if any.</param>
        ''' <returns>A new instance of ArgumentNullException.</returns>
        Friend Shared Function GetArgumentNullException(ArgumentName As String,
            ResourceID As String, ParamArray PlaceHolders() As String) As ArgumentNullException

            Return New ArgumentNullException(ArgumentName, GetResourceString(ResourceID, PlaceHolders))
        End Function

        ''' <summary>
        ''' Returns a new instance of IO.DirectoryNotFoundException with the message from resource file.
        ''' </summary>
        ''' <param name="ResourceID">The resource ID.</param>
        ''' <param name="PlaceHolders">Strings that will replace place holders in the resource string, if any.</param>
        ''' <returns>A new instance of IO.DirectoryNotFoundException.</returns>
        Friend Shared Function GetDirectoryNotFoundException(
            ResourceID As String, ParamArray PlaceHolders() As String) As IO.DirectoryNotFoundException

            Return New IO.DirectoryNotFoundException(GetResourceString(ResourceID, PlaceHolders))
        End Function

        ''' <summary>
        ''' Returns a new instance of IO.FileNotFoundException with the message from resource file.
        ''' </summary>
        ''' <param name="FileName">The file name (path) of the not found file.</param>
        ''' <param name="ResourceID">The resource ID.</param>
        ''' <param name="PlaceHolders">Strings that will replace place holders in the resource string, if any.</param>
        ''' <returns>A new instance of IO.FileNotFoundException.</returns>
        Friend Shared Function GetFileNotFoundException(FileName As String,
            ResourceID As String, ParamArray PlaceHolders() As String) As IO.FileNotFoundException

            Return New IO.FileNotFoundException(GetResourceString(ResourceID, PlaceHolders), FileName)
        End Function

        ''' <summary>
        ''' Returns a new instance of InvalidOperationException with the message from resource file.
        ''' </summary>
        ''' <param name="ResourceID">The resource ID.</param>
        ''' <param name="PlaceHolders">Strings that will replace place holders in the resource string, if any.</param>
        ''' <returns>A new instance of InvalidOperationException.</returns>
        Friend Shared Function GetInvalidOperationException(
            ResourceID As String, ParamArray PlaceHolders() As String) As InvalidOperationException

            Return New InvalidOperationException(GetResourceString(ResourceID, PlaceHolders))
        End Function

        ''' <summary>
        ''' Returns a new instance of IO.IOException with the message from resource file.
        ''' </summary>
        ''' <param name="ResourceID">The resource ID.</param>
        ''' <param name="PlaceHolders">Strings that will replace place holders in the resource string, if any.</param>
        ''' <returns>A new instance of IO.IOException.</returns>
        Friend Shared Function GetIOException(ResourceID As String, ParamArray PlaceHolders() As String) As IO.IOException

            Return New IO.IOException(GetResourceString(ResourceID, PlaceHolders))
        End Function

        ''' <summary>
        ''' Returns a new instance of Win32Exception with the message from resource file and the last Win32 error.
        ''' </summary>
        ''' <param name="ResourceID">The resource ID.</param>
        ''' <param name="PlaceHolders">Strings that will replace place holders in the resource string, if any.</param>
        ''' <returns>A new instance of Win32Exception.</returns>
        ''' <remarks>There is no way to exclude the Win32 error so this function will call Marshal.GetLastWin32Error all the time.</remarks>

        Friend Shared Function GetWin32Exception(
            ResourceID As String, ParamArray PlaceHolders() As String) As ComponentModel.Win32Exception

            Return New ComponentModel.Win32Exception(System.Runtime.InteropServices.Marshal.GetLastWin32Error(), GetResourceString(ResourceID, PlaceHolders))
        End Function

    End Class

End Namespace
