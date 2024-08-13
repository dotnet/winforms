' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Net
Imports Microsoft.VisualBasic.FileIO
Imports Microsoft.VisualBasic.MyServices.Internal

Imports ExUtils = Microsoft.VisualBasic.CompilerServices.ExceptionUtils

Namespace Microsoft.VisualBasic.Devices

    ''' <summary>
    '''  An object that allows easy access to some simple network properties and functionality.
    ''' </summary>
    Partial Public Class Network

        Public Sub New()
        End Sub

        ''' <summary>
        '''  Downloads a file from the network to the specified path.
        ''' </summary>
        ''' <param name="address">Address to the remote file, http, ftp etc...</param>
        ''' <param name="destinationFileName">Name and path of file where download is saved.</param>
        Public Sub DownloadFile(address As String, destinationFileName As String)
            address,
            destinationFileName,
            userName:=DEFAULT_USERNAME,
            password:=DEFAULT_PASSWORD,
            connectionTimeout:=DEFAULT_TIMEOUT,

        End Sub

        ''' <summary>
        '''  Downloads a file from the network to the specified path.
        ''' </summary>
        ''' <param name="address">Address to the remote file, http, ftp etc...</param>
        ''' <param name="destinationFileName">Name and path of file where download is saved.</param>
        ''' <param name="userName">The name of the user performing the download.</param>
        ''' <param name="password">The user's password</param>
        Public Sub DownloadFile(
            address As String,
            destinationFileName As String,
            userName As String,
            password As String)

            address,
            destinationFileName,
            userName,
            password,
            connectionTimeout:=DEFAULT_TIMEOUT,
        End Sub

        ''' <summary>
        '''  Downloads a file from the network to the specified path.
        ''' </summary>
        ''' <param name="address">Uri to the remote file.</param>
        ''' <param name="destinationFileName">Name and path of file where download is saved.</param>
        ''' <param name="userName">The name of the user performing the download.</param>
        ''' <param name="password">The user's password.</param>
        Public Sub DownloadFile(
            address As Uri,
            destinationFileName As String,
            userName As String,
            password As String)

            DownloadFile(
                address,
                destinationFileName,
                userName,
                password,
                showUI:=False,
                connectionTimeout:=DEFAULT_TIMEOUT,
                overwrite:=False)
        End Sub

        ''' <summary>
        '''  Downloads a file from the network to the specified path.
        ''' </summary>
        ''' <param name="address">Address to the remote file, http, ftp etc...</param>
        ''' <param name="destinationFileName">Name and path of file where download is saved.</param>
        ''' <param name="userName">The name of the user performing the download.</param>
        ''' <param name="password">The user's password.</param>
        ''' <param name="showUI">Indicates whether or not to show a progress bar.</param>
        ''' <param name="connectionTimeout">Time allotted before giving up on a connection.</param>
        Public Sub DownloadFile(
            address As String,
            destinationFileName As String,
            userName As String,
            password As String,
            showUI As Boolean,
            connectionTimeout As Integer,
            overwrite As Boolean)

            address,
            destinationFileName,
            userName,
            password,
            connectionTimeout,
            overwrite,
        End Sub

        ''' <summary>
        '''  Downloads a file from the network to the specified path.
        ''' </summary>
        ''' <param name="address">Address to the remote file, http, ftp etc...</param>
        ''' <param name="destinationFileName">Name and path of file where download is saved.</param>
        ''' <param name="userName">The name of the user performing the download.</param>
        ''' <param name="password">The user's password.</param>
        ''' <param name="showUI">Indicates whether or not to show a progress bar.</param>
        ''' <param name="connectionTimeout">Time allotted before giving up on a connection.</param>
        ''' <param name="overwrite">Indicates whether or not the file should be
        '''   overwritten if local file already exists.
        ''' </param>
        ''' <param name="onUserCancel">
        '''  Indicates what to do if user cancels dialog (either throw or do nothing).
        ''' </param>
        Public Sub DownloadFile(
            address As String,
            destinationFileName As String,
            userName As String,
            password As String,
            showUI As Boolean,
            connectionTimeout As Integer,
            overwrite As Boolean,
            onUserCancel As UICancelOption)

            ' We're safe from DownloadFile(Nothing, ...) due to overload failure (DownloadFile(String,...) vs. DownloadFile(Uri,...)).
            ' However, it is good practice to verify address before calling Trim.
            If String.IsNullOrWhiteSpace(address) Then
                Throw ExUtils.GetArgumentNullException(NameOf(address))
            End If

            Dim addressUri As Uri = GetUri(address.Trim())

            ' Get network credentials
            Dim networkCredentials As ICredentials = GetNetworkCredentials(userName, password)

            destinationFileName,
            networkCredentials,
            connectionTimeout,
            overwrite,
            onUserCancel)

                If t.IsFaulted Then
                ' IsFaulted will be true if any parameters are bad
                Throw t.Exception
            Else
                dialog?.ShowProgressDialog()
                t.Wait()
                If t.IsFaulted Then
                    Throw t.Exception
                End If
            End If
            Catch ex As Exception
            If ex.InnerException IsNot Nothing Then
                Throw ex.InnerException
            End If
            Throw
            Finally
            CloseProgressDialog(dialog)
            End Try
        End Sub

        ''' <summary>
        '''  Downloads a file from the network to the specified path.
        ''' </summary>
        ''' <param name="showUI">Indicates whether or not to show a progress bar.</param>
        ''' <param name="connectionTimeout">Time allotted before giving up on a connection.</param>
        Public Sub DownloadFile(
            address As Uri,
            destinationFileName As String,
            showUI As Boolean,
            connectionTimeout As Integer,
            overwrite As Boolean)

            destinationFileName,
            connectionTimeout,
        End Sub

        ''' <summary>
        '''  Downloads a file from the network to the specified path.
        ''' </summary>
        ''' <param name="address">Uri to the remote file.</param>
        ''' <param name="showUI">Indicates whether or not to show a progress bar.</param>
        Public Sub DownloadFile(
            address As Uri,
            destinationFileName As String,
            showUI As Boolean,
            connectionTimeout As Integer,
            overwrite As Boolean,
            onUserCancel As UICancelOption)


            destinationFileName,
            networkCredentials,
            connectionTimeout,
            overwrite,
            onUserCancel)

                If t.IsFaulted Then
                ' IsFaulted will be true if any parameters are bad
                Throw t.Exception
            Else
                dialog?.ShowProgressDialog()
                t.Wait()
                If t.IsFaulted Then
                    Throw t.Exception
                End If
            End If
            Catch ex As Exception
            If ex.InnerException IsNot Nothing Then
                Throw ex.InnerException
            End If
            Throw
            Finally
            CloseProgressDialog(dialog)
            End Try
        End Sub

        ''' <summary>
        '''  Downloads a file from the network to the specified path.
        ''' </summary>
        ''' <param name="address">Uri to the remote file.</param>
        ''' <param name="showUI">Indicates whether or not to show a progress bar.</param>
        ''' <param name="connectionTimeout">Time allotted before giving up on a connection.</param>
        Public Sub DownloadFile(
            address As Uri,
            destinationFileName As String,
            showUI As Boolean,
            connectionTimeout As Integer,
            overwrite As Boolean)

            destinationFileName,
            networkCredentials,
            connectionTimeout,
            overwrite,

                If t.IsFaulted Then
                ' IsFaulted will be true if any parameters are bad
                Throw t.Exception
            Else
                dialog?.ShowProgressDialog()
                t.Wait()
                If t.IsFaulted Then
                    Throw t.Exception
                End If
            End If
            Catch ex As Exception
            If ex.InnerException IsNot Nothing Then
                Throw ex.InnerException
            End If
            Throw
            Finally
            CloseProgressDialog(dialog)
            End Try
        End Sub

        ''' <summary>
        '''  Downloads a file from the network to the specified path.
        ''' </summary>
        ''' <param name="address">Uri to the remote file.</param>
        ''' <param name="destinationFileName">Name and path of file where download is saved.</param>
        ''' <param name="showUI">Indicates whether or not to show a progress bar.</param>
        ''' <param name="connectionTimeout">Time allotted before giving up on a connection.</param>
        ''' <param name="overwrite">Indicates whether or not the file should be overwritten if local file already exists.</param>
        ''' <param name="onUserCancel">Indicates what to do if user cancels dialog (either throw or do nothing).</param>
        ''' <remarks>Calls to all the other overloads will come through here.</remarks>
        Public Sub DownloadFile(
            address As Uri,
            destinationFileName As String,
            showUI As Boolean,
            connectionTimeout As Integer,
            overwrite As Boolean,
            onUserCancel As UICancelOption)

            If connectionTimeout <= 0 Then
                Throw ExUtils.GetArgumentExceptionWithArgName(NameOf(connectionTimeout), SR.Network_BadConnectionTimeout)
            End If

            If address Is Nothing Then
                Throw ExUtils.GetArgumentNullException(NameOf(address))
            End If



            End If

            ' Set credentials if we have any
            If networkCredentials IsNot Nothing Then
                client.Credentials = networkCredentials
            End If
            End If


            End If


            End If

        End Sub

    End Class
End Namespace
