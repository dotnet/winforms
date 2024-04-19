' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Threading

Namespace Microsoft.VisualBasic.CompilerServices

    Friend Enum vbErrors
        None = 0
        FileNotFound = 53
        PermissionDenied = 70
    End Enum

    ' Implements error utilities for Basic
    Friend Module ExceptionUtils

        Friend Function GetResourceString(resourceKey As String,
                            ParamArray args() As String) As String
            Return String.Format(Thread.CurrentThread.CurrentCulture, resourceKey, args)
        End Function

        Friend Function GetResourceString(resourceId As Integer) As String
            Dim id As String = $"ID{resourceId}"
            ' always return a string
            Return $"{SR.GetResourceString(id, id)}"
        End Function

        Friend Function VbMakeException(resourceId As Integer) As Exception
            Dim description As String = ""

            If resourceId > 0 AndAlso resourceId <= &HFFFFI Then
                description = GetResourceString(resourceId)
            End If

            Select Case resourceId

                Case vbErrors.FileNotFound
                    Return New IO.FileNotFoundException(description)

                Case vbErrors.PermissionDenied
                    Return New IO.IOException(description)

                Case Else
                    Return New Exception(description)
            End Select

        End Function

        ''' <summary>
        '''  Returns a new instance of ArgumentException with the message from resource file and the Exception.ArgumentName property set.
        ''' </summary>
        ''' <param name="argumentName">The name of the argument (parameter). Not localized.</param>
        ''' <param name="resourceID">The resource ID.</param>
        ''' <param name="placeHolders">Strings that will replace place holders in the resource string, if any.</param>
        ''' <returns>A new instance of ArgumentException.</returns>
        ''' <remarks>This is the preferred way to construct an argument exception.</remarks>
        Friend Function GetArgumentExceptionWithArgName(argumentName As String,
                            resourceID As String,
                            ParamArray placeHolders() As String) As ArgumentException

            Return New ArgumentException(GetResourceString(resourceID, placeHolders), argumentName)
        End Function

        ''' <summary>
        '''  Returns a new instance of ArgumentNullException with message: "Argument cannot be Nothing."
        ''' </summary>
        ''' <param name="argumentName">The name of the argument (parameter). Not localized.</param>
        ''' <returns>A new instance of ArgumentNullException.</returns>
        Friend Function GetArgumentNullException(argumentName As String) As ArgumentNullException

            Return New ArgumentNullException(argumentName, GetResourceString(SR.General_ArgumentNullException))
        End Function

        ''' <summary>
        '''  Returns a new instance of ArgumentNullException with the message from resource file.
        ''' </summary>
        ''' <param name="argumentName">The name of the argument (parameter). Not localized.</param>
        ''' <param name="resourceID">The resource ID.</param>
        ''' <param name="placeHolders">Strings that will replace place holders in the resource string, if any.</param>
        ''' <returns>A new instance of ArgumentNullException.</returns>
        Friend Function GetArgumentNullException(argumentName As String,
                            resourceID As String,
                            ParamArray placeHolders() As String) As ArgumentNullException

            Return New ArgumentNullException(argumentName, GetResourceString(resourceID, placeHolders))
        End Function

        ''' <summary>
        '''  Returns a new instance of IO.DirectoryNotFoundException with the message from resource file.
        ''' </summary>
        ''' <param name="resourceID">The resource ID.</param>
        ''' <param name="placeHolders">Strings that will replace place holders in the resource string, if any.</param>
        ''' <returns>A new instance of IO.DirectoryNotFoundException.</returns>
        Friend Function GetDirectoryNotFoundException(resourceID As String,
                            ParamArray placeHolders() As String) As IO.DirectoryNotFoundException

            Return New IO.DirectoryNotFoundException(GetResourceString(resourceID, placeHolders))
        End Function

        ''' <summary>
        '''  Returns a new instance of IO.FileNotFoundException with the message from resource file.
        ''' </summary>
        ''' <param name="fileName">The file name (path) of the not found file.</param>
        ''' <param name="resourceID">The resource ID.</param>
        ''' <param name="placeHolders">Strings that will replace place holders in the resource string, if any.</param>
        ''' <returns>A new instance of IO.FileNotFoundException.</returns>
        Friend Function GetFileNotFoundException(fileName As String,
                           resourceID As String,
                           ParamArray placeHolders() As String) As IO.FileNotFoundException

            Return New IO.FileNotFoundException(GetResourceString(resourceID, placeHolders), fileName)
        End Function

        ''' <summary>
        '''  Returns a new instance of InvalidOperationException with the message from resource file.
        ''' </summary>
        ''' <param name="resourceID">The resource ID.</param>
        ''' <param name="placeHolders">Strings that will replace place holders in the resource string, if any.</param>
        ''' <returns>A new instance of InvalidOperationException.</returns>
        Friend Function GetInvalidOperationException(resourceID As String,
                            ParamArray placeHolders() As String) As InvalidOperationException

            Return New InvalidOperationException(GetResourceString(resourceID, placeHolders))
        End Function

        ''' <summary>
        '''  Returns a new instance of IO.IOException with the message from resource file.
        ''' </summary>
        ''' <param name="resourceID">The resource ID.</param>
        ''' <param name="placeHolders">Strings that will replace place holders in the resource string, if any.</param>
        ''' <returns>A new instance of IO.IOException.</returns>
        Friend Function GetIOException(resourceID As String,
                            ParamArray placeHolders() As String) As IO.IOException

            Return New IO.IOException(GetResourceString(resourceID, placeHolders))
        End Function

        ''' <summary>
        '''  Returns a new instance of Win32Exception with the message from resource file and the last Win32 error.
        ''' </summary>
        ''' <param name="resourceID">The resource ID.</param>
        ''' <param name="placeHolders">Strings that will replace place holders in the resource string, if any.</param>
        ''' <returns>A new instance of Win32Exception.</returns>
        ''' <remarks>There is no way to exclude the Win32 error so this function will call Marshal.GetLastWin32Error all the time.</remarks>

        Friend Function GetWin32Exception(resourceID As String,
                            ParamArray placeHolders() As String) As ComponentModel.Win32Exception

            Return New ComponentModel.Win32Exception(Runtime.InteropServices.Marshal.GetLastWin32Error(), GetResourceString(resourceID, placeHolders))
        End Function

    End Module
End Namespace
