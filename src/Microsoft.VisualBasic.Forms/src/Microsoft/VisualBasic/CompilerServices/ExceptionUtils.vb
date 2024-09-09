' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Runtime.InteropServices
Imports VbUtils = Microsoft.VisualBasic.CompilerServices.Utils

Namespace Microsoft.VisualBasic.CompilerServices

    ' Implements error utilities for Basic
    Friend Module ExceptionUtils

        Friend Function BuildException(
            resourceId As Integer,
            description As String,
            ByRef vbDefinedError As Boolean) As Exception

            vbDefinedError = True

            Select Case resourceId

                Case VbErrors.None

                Case VbErrors.FileNotFound
                    Return New IO.FileNotFoundException(description)

                Case VbErrors.PermissionDenied
                    Return New IO.IOException(description)

                Case Else
                    'Fall below to default
                    vbDefinedError = False
                    Return New Exception(description)
            End Select

            vbDefinedError = False
            Return New Exception(description)

        End Function

        ''' <summary>
        '''  Returns a new instance of <see cref="ArgumentException"/> with the message from resource file and the Exception.ArgumentName property set.
        ''' </summary>
        ''' <param name="argumentName">The name of the argument (parameter). Not localized.</param>
        ''' <param name="resourceID">The resource ID.</param>
        ''' <param name="placeHolders">Strings that will replace place holders in the resource string, if any.</param>
        ''' <returns>A new instance of <see cref="ArgumentException"/>.</returns>
        ''' <remarks>This is the preferred way to construct an argument exception.</remarks>
        Friend Function GetArgumentExceptionWithArgName(
            argumentName As String,
            resourceID As String,
            ParamArray placeHolders() As String) As ArgumentException

            Return New ArgumentException(VbUtils.GetResourceString(resourceID, placeHolders), argumentName)
        End Function

        ''' <summary>
        '''  Returns a new instance of <see cref="ArgumentNullException"/> with message: "Argument cannot be Nothing."
        ''' </summary>
        ''' <param name="argumentName">The name of the argument (parameter). Not localized.</param>
        ''' <returns>A new instance of <see cref="ArgumentNullException"/>.</returns>
        Friend Function GetArgumentNullException(argumentName As String) As ArgumentNullException

            Return New ArgumentNullException(argumentName, VbUtils.GetResourceString(SR.General_ArgumentNullException))
        End Function

        ''' <summary>
        '''  Returns a new instance of <see cref="ArgumentNullException"/> with the message from resource file.
        ''' </summary>
        ''' <param name="argumentName">The name of the argument (parameter). Not localized.</param>
        ''' <param name="resourceID">The resource ID.</param>
        ''' <param name="placeHolders">Strings that will replace place holders in the resource string, if any.</param>
        ''' <returns>A new instance of <see cref="ArgumentNullException"/>.</returns>
        Friend Function GetArgumentNullException(
            argumentName As String,
            resourceID As String,
            ParamArray placeHolders() As String) As ArgumentNullException

            Return New ArgumentNullException(argumentName, VbUtils.GetResourceString(resourceID, placeHolders))
        End Function

        ''' <summary>
        '''  Returns a new instance of <see cref="IO.DirectoryNotFoundException"/> with the message from resource file.
        ''' </summary>
        ''' <param name="resourceID">The resource ID.</param>
        ''' <param name="placeHolders">Strings that will replace place holders in the resource string, if any.</param>
        ''' <returns>A new instance of <see cref="IO.DirectoryNotFoundException"/>.</returns>
        Friend Function GetDirectoryNotFoundException(
            resourceID As String,
            ParamArray placeHolders() As String) As IO.DirectoryNotFoundException

            Return New IO.DirectoryNotFoundException(VbUtils.GetResourceString(resourceID, placeHolders))
        End Function

        ''' <summary>
        '''  Returns a new instance of <see cref="IO.FileNotFoundException"/> with the message from resource file.
        ''' </summary>
        ''' <param name="fileName">The file name (path) of the not found file.</param>
        ''' <param name="resourceID">The resource ID.</param>
        ''' <param name="placeHolders">Strings that will replace place holders in the resource string, if any.</param>
        ''' <returns>A new instance of <see cref="IO.FileNotFoundException"/>.</returns>
        Friend Function GetFileNotFoundException(
            fileName As String,
            resourceID As String,
            ParamArray placeHolders() As String) As IO.FileNotFoundException

            Return New IO.FileNotFoundException(VbUtils.GetResourceString(resourceID, placeHolders), fileName)
        End Function

        ''' <summary>
        '''  Returns a new instance of <see cref="InvalidOperationException"/> with the message from resource file.
        ''' </summary>
        ''' <param name="resourceID">The resource ID.</param>
        ''' <param name="placeHolders">Strings that will replace place holders in the resource string, if any.</param>
        ''' <returns>A new instance of <see cref="InvalidOperationException"/>.</returns>
        Friend Function GetInvalidOperationException(
            resourceID As String,
            ParamArray placeHolders() As String) As InvalidOperationException

            Return New InvalidOperationException(VbUtils.GetResourceString(resourceID, placeHolders))
        End Function

        ''' <summary>
        '''  Returns a new instance of <see cref="IO.IOException"/> with the message from resource file.
        ''' </summary>
        ''' <param name="resourceID">The resource ID.</param>
        ''' <param name="placeHolders">Strings that will replace place holders in the resource string, if any.</param>
        ''' <returns>A new instance of <see cref="IO.IOException"/>.</returns>
        Friend Function GetIOException(
            resourceID As String,
            ParamArray placeHolders() As String) As IO.IOException

            Return New IO.IOException(VbUtils.GetResourceString(resourceID, placeHolders))
        End Function

        ''' <summary>
        '''  Returns a new instance of <see cref="ComponentModel.Win32Exception"/> with the message from resource file and the last Win32 error.
        ''' </summary>
        ''' <param name="resourceID">The resource ID.</param>
        ''' <param name="placeHolders">Strings that will replace place holders in the resource string, if any.</param>
        ''' <returns>A new instance of <see cref="ComponentModel.Win32Exception"/>.</returns>
        ''' <remarks>There is no way to exclude the Win32 error so this function will call Marshal.GetLastWin32Error all the time.</remarks>
        Friend Function GetWin32Exception(
            resourceID As String,
            ParamArray placeHolders() As String) As ComponentModel.Win32Exception

            Return New ComponentModel.Win32Exception(Marshal.GetLastWin32Error(), VbUtils.GetResourceString(resourceID, placeHolders))
        End Function

        Friend Function VbMakeException(hr As Integer) As Exception
            Dim sMsg As String

            If hr > 0 AndAlso hr <= &HFFFFI Then
                sMsg = VbUtils.GetResourceString(CType(hr, VbErrors))
            Else
                sMsg = String.Empty
            End If
            VbMakeException = VbMakeExceptionEx(hr, sMsg)
        End Function

        Friend Function VbMakeExceptionEx(number As Integer, sMsg As String) As Exception
            Dim vBDefinedError As Boolean

            VbMakeExceptionEx = BuildException(number, sMsg, vBDefinedError)

            If vBDefinedError Then
                ' .NET Framework implementation calls:
                ' Err().SetUnmappedError(number)
            End If

        End Function

    End Module
End Namespace
