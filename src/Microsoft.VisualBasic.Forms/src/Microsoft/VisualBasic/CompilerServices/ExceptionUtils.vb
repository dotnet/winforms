﻿' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Globalization
Imports System.Threading

Namespace Microsoft.VisualBasic.CompilerServices

    Friend Enum vbErrors
        None = 0
        FileNotFound = 53
        PermissionDenied = 70
    End Enum

    ' Implements error utilities for Basic
    Friend Module ExceptionUtils

        Friend Function GetCultureInfo() As CultureInfo
            Return Thread.CurrentThread.CurrentCulture
        End Function

        Friend Function GetResourceString(resourceKey As String, ParamArray args() As String) As String
            Return String.Format(GetCultureInfo(), resourceKey, args)
        End Function

        Friend Function GetResourceString(ResourceId As Integer) As String
            Dim id As String = $"ID{ResourceId}"
            ' always return a string
            Return $"{SR.GetResourceString(id, id)}"
        End Function

        Friend Function VbMakeException(ResourceId As Integer) As Exception
            Dim description As String = ""

            If ResourceId > 0 AndAlso ResourceId <= &HFFFFI Then
                description = GetResourceString(ResourceId)
            End If

            Select Case ResourceId

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
        ''' <param name="ArgumentName">The name of the argument (parameter). Not localized.</param>
        ''' <param name="ResourceID">The resource ID.</param>
        ''' <param name="PlaceHolders">Strings that will replace place holders in the resource string, if any.</param>
        ''' <returns>A new instance of ArgumentException.</returns>
        ''' <remarks>This is the preferred way to construct an argument exception.</remarks>
        Friend Function GetArgumentExceptionWithArgName(ArgumentName As String,
                            ResourceID As String,
                            ParamArray PlaceHolders() As String) As ArgumentException

            Return New ArgumentException(GetResourceString(ResourceID, PlaceHolders), ArgumentName)
        End Function

        ''' <summary>
        '''  Returns a new instance of ArgumentNullException with message: "Argument cannot be Nothing."
        ''' </summary>
        ''' <param name="ArgumentName">The name of the argument (parameter). Not localized.</param>
        ''' <returns>A new instance of ArgumentNullException.</returns>
        Friend Function GetArgumentNullException(ArgumentName As String) As ArgumentNullException

            Return New ArgumentNullException(ArgumentName, GetResourceString(SR.General_ArgumentNullException))
        End Function

        ''' <summary>
        '''  Returns a new instance of ArgumentNullException with the message from resource file.
        ''' </summary>
        ''' <param name="ArgumentName">The name of the argument (parameter). Not localized.</param>
        ''' <param name="ResourceID">The resource ID.</param>
        ''' <param name="PlaceHolders">Strings that will replace place holders in the resource string, if any.</param>
        ''' <returns>A new instance of ArgumentNullException.</returns>
        Friend Function GetArgumentNullException(ArgumentName As String,
                            ResourceID As String,
                            ParamArray PlaceHolders() As String) As ArgumentNullException

            Return New ArgumentNullException(ArgumentName, GetResourceString(ResourceID, PlaceHolders))
        End Function

        ''' <summary>
        '''  Returns a new instance of IO.DirectoryNotFoundException with the message from resource file.
        ''' </summary>
        ''' <param name="ResourceID">The resource ID.</param>
        ''' <param name="PlaceHolders">Strings that will replace place holders in the resource string, if any.</param>
        ''' <returns>A new instance of IO.DirectoryNotFoundException.</returns>
        Friend Function GetDirectoryNotFoundException(ResourceID As String,
                            ParamArray PlaceHolders() As String) As IO.DirectoryNotFoundException

            Return New IO.DirectoryNotFoundException(GetResourceString(ResourceID, PlaceHolders))
        End Function

        ''' <summary>
        '''  Returns a new instance of IO.FileNotFoundException with the message from resource file.
        ''' </summary>
        ''' <param name="FileName">The file name (path) of the not found file.</param>
        ''' <param name="ResourceID">The resource ID.</param>
        ''' <param name="PlaceHolders">Strings that will replace place holders in the resource string, if any.</param>
        ''' <returns>A new instance of IO.FileNotFoundException.</returns>
        Friend Function GetFileNotFoundException(FileName As String,
                           ResourceID As String,
                           ParamArray PlaceHolders() As String) As IO.FileNotFoundException

            Return New IO.FileNotFoundException(GetResourceString(ResourceID, PlaceHolders), FileName)
        End Function

        ''' <summary>
        '''  Returns a new instance of InvalidOperationException with the message from resource file.
        ''' </summary>
        ''' <param name="ResourceID">The resource ID.</param>
        ''' <param name="PlaceHolders">Strings that will replace place holders in the resource string, if any.</param>
        ''' <returns>A new instance of InvalidOperationException.</returns>
        Friend Function GetInvalidOperationException(ResourceID As String,
                            ParamArray PlaceHolders() As String) As InvalidOperationException

            Return New InvalidOperationException(GetResourceString(ResourceID, PlaceHolders))
        End Function

        ''' <summary>
        '''  Returns a new instance of IO.IOException with the message from resource file.
        ''' </summary>
        ''' <param name="ResourceID">The resource ID.</param>
        ''' <param name="PlaceHolders">Strings that will replace place holders in the resource string, if any.</param>
        ''' <returns>A new instance of IO.IOException.</returns>
        Friend Function GetIOException(ResourceID As String,
                            ParamArray PlaceHolders() As String) As IO.IOException

            Return New IO.IOException(GetResourceString(ResourceID, PlaceHolders))
        End Function

        ''' <summary>
        '''  Returns a new instance of Win32Exception with the message from resource file and the last Win32 error.
        ''' </summary>
        ''' <param name="ResourceID">The resource ID.</param>
        ''' <param name="PlaceHolders">Strings that will replace place holders in the resource string, if any.</param>
        ''' <returns>A new instance of Win32Exception.</returns>
        ''' <remarks>There is no way to exclude the Win32 error so this function will call Marshal.GetLastWin32Error all the time.</remarks>

        Friend Function GetWin32Exception(ResourceID As String,
                            ParamArray PlaceHolders() As String) As ComponentModel.Win32Exception

            Return New ComponentModel.Win32Exception(Runtime.InteropServices.Marshal.GetLastWin32Error(), GetResourceString(ResourceID, PlaceHolders))
        End Function

    End Module
End Namespace
