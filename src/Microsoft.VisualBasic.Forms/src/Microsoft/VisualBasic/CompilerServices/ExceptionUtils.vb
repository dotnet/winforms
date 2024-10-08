' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Runtime.InteropServices
Imports System.Threading

Namespace Microsoft.VisualBasic.CompilerServices

    ' Implements error utilities for Basic
    Friend Module ExceptionUtils

        ''' <summary>
        '''  Returns a new instance of <see cref="ArgumentException"/> with the message from resource file and the Exception.ArgumentName property set.
        ''' </summary>
        ''' <param name="argumentName">The name of the argument (parameter). Not localized.</param>
        ''' <param name="resourceKey">The resource ID.</param>
        ''' <param name="placeHolders">Strings that will replace place holders in the resource string, if any.</param>
        ''' <returns>A new instance of <see cref="ArgumentException"/>.</returns>
        ''' <remarks>This is the preferred way to construct an argument exception.</remarks>
        Friend Function GetArgumentExceptionWithArgName(
            argumentName As String,
            resourceKey As String,
            ParamArray placeHolders() As String) As ArgumentException

            Return New ArgumentException(GetResourceString(resourceKey, placeHolders), argumentName)
        End Function

        ''' <summary>
        '''  Returns a new instance of <see cref="ArgumentNullException"/> with message: "Argument cannot be Nothing."
        ''' </summary>
        ''' <param name="argumentName">The name of the argument (parameter). Not localized.</param>
        ''' <returns>A new instance of <see cref="ArgumentNullException"/>.</returns>
        Friend Function GetArgumentNullException(argumentName As String) As ArgumentNullException

            Return New ArgumentNullException(argumentName, GetResourceString(SR.General_ArgumentNullException))
        End Function

        ''' <summary>
        '''  Returns a new instance of <see cref="ArgumentNullException"/> with the message from resource file.
        ''' </summary>
        ''' <param name="argumentName">The name of the argument (parameter). Not localized.</param>
        ''' <param name="resourceKey">The resource ID.</param>
        ''' <param name="placeHolders">Strings that will replace place holders in the resource string, if any.</param>
        ''' <returns>A new instance of <see cref="ArgumentNullException"/>.</returns>
        Friend Function GetArgumentNullException(
            argumentName As String,
            resourceKey As String,
            ParamArray placeHolders() As String) As ArgumentNullException

            Return New ArgumentNullException(argumentName, GetResourceString(resourceKey, placeHolders))
        End Function

        ''' <summary>
        '''  Returns a new instance of <see cref="IO.DirectoryNotFoundException"/> with the message from resource file.
        ''' </summary>
        ''' <param name="resourceKey">The resource ID.</param>
        ''' <param name="placeHolders">Strings that will replace place holders in the resource string, if any.</param>
        ''' <returns>A new instance of <see cref="IO.DirectoryNotFoundException"/>.</returns>
        Friend Function GetDirectoryNotFoundException(
            resourceKey As String,
            ParamArray placeHolders() As String) As IO.DirectoryNotFoundException

            Return New IO.DirectoryNotFoundException(GetResourceString(resourceKey, placeHolders))
        End Function

        ''' <summary>
        '''  Returns a new instance of <see cref="IO.FileNotFoundException"/> with the message from resource file.
        ''' </summary>
        ''' <param name="fileName">The file name (path) of the not found file.</param>
        ''' <param name="resourceKey">The resource ID.</param>
        ''' <param name="placeHolders">Strings that will replace place holders in the resource string, if any.</param>
        ''' <returns>A new instance of <see cref="IO.FileNotFoundException"/>.</returns>
        Friend Function GetFileNotFoundException(
            fileName As String,
            resourceKey As String,
            ParamArray placeHolders() As String) As IO.FileNotFoundException

            Return New IO.FileNotFoundException(GetResourceString(resourceKey, placeHolders), fileName)
        End Function

        ''' <summary>
        '''  Returns a new instance of <see cref="InvalidOperationException"/> with the message from resource file.
        ''' </summary>
        ''' <param name="resourceKey">The resource ID.</param>
        ''' <param name="placeHolders">Strings that will replace place holders in the resource string, if any.</param>
        ''' <returns>A new instance of <see cref="InvalidOperationException"/>.</returns>
        Friend Function GetInvalidOperationException(
            resourceKey As String,
            ParamArray placeHolders() As String) As InvalidOperationException

            Return New InvalidOperationException(GetResourceString(resourceKey, placeHolders))
        End Function

        ''' <summary>
        '''  Returns a new instance of <see cref="IO.IOException"/> with the message from resource file.
        ''' </summary>
        ''' <param name="resourceKey">The resource ID.</param>
        ''' <param name="placeHolders">Strings that will replace place holders in the resource string, if any.</param>
        ''' <returns>A new instance of <see cref="IO.IOException"/>.</returns>
        Friend Function GetIOException(
            resourceKey As String,
            ParamArray placeHolders() As String) As IO.IOException

            Return New IO.IOException(GetResourceString(resourceKey, placeHolders))
        End Function

        Friend Function GetResourceString(
            resourceKey As String,
            ParamArray args() As String) As String

            Return String.Format(Thread.CurrentThread.CurrentCulture, resourceKey, args)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="resourceId">A <see cref="VbErrors"/> value that will become the ResourceKey</param>
        ''' <returns>A <see langword="String"/> localized version of the resource.</returns>
        Friend Function GetResourceString(resourceId As VbErrors) As String
            Dim resourceKey As String = $"ID{CStr(resourceId)}"
            Return SR.GetResourceString(resourceKey, resourceKey)
        End Function

        ''' <summary>
        '''  Returns a new instance of <see cref="ComponentModel.Win32Exception"/> with the message from resource file and the last Win32 error.
        ''' </summary>
        ''' <param name="resourceKey">The resource ID.</param>
        ''' <param name="placeHolders">Strings that will replace place holders in the resource string, if any.</param>
        ''' <returns>A new instance of <see cref="ComponentModel.Win32Exception"/>.</returns>
        ''' <remarks>There is no way to exclude the Win32 error so this function will call Marshal.GetLastWin32Error all the time.</remarks>
        Friend Function GetWin32Exception(
            resourceKey As String,
            ParamArray placeHolders() As String) As ComponentModel.Win32Exception

            Return New ComponentModel.Win32Exception(Marshal.GetLastWin32Error(), GetResourceString(resourceKey, placeHolders))
        End Function

        Friend Function VbMakeException(resourceId As Integer) As Exception
            Dim description As String = String.Empty

            If resourceId > 0 AndAlso resourceId <= &HFFFFI Then
                description = GetResourceString(DirectCast(resourceId, VbErrors))
            End If

            Select Case resourceId

                Case VbErrors.FileNotFound
                    Return New IO.FileNotFoundException(description)

                Case VbErrors.PermissionDenied
                    Return New IO.IOException(description)

                Case Else
                    Return New Exception(description)
            End Select

        End Function

    End Module
End Namespace
