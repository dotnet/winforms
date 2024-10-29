' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Net
Imports Microsoft.VisualBasic.FileIO
Imports Microsoft.VisualBasic.MyServices.Internal

Imports VbUtils = Microsoft.VisualBasic.CompilerServices.ExceptionUtils

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
            Try
                DownloadFileAsync(
                    address,
                    destinationFileName,
                    userName:=DEFAULT_USERNAME,
                    password:=DEFAULT_PASSWORD,
                    dialog:=Nothing,
                    connectionTimeout:=DEFAULT_TIMEOUT,
                    overwrite:=False).Wait()
            Catch ex As Exception
                If ex.InnerException IsNot Nothing Then
                    Throw ex.InnerException
                End If
                Throw
            End Try
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

            Try
                DownloadFileAsync(
                    address,
                    destinationFileName,
                    userName,
                    password,
                    dialog:=Nothing,
                    connectionTimeout:=DEFAULT_TIMEOUT,
                    overwrite:=False).Wait()
            Catch ex As Exception
                If ex.InnerException IsNot Nothing Then
                    Throw ex.InnerException
                End If
                Throw
            End Try
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
        ''' <param name="overwrite">
        '''  Indicates whether or not the file should be overwritten if local file already exists.
        ''' </param>
        Public Sub DownloadFile(
            address As String,
            destinationFileName As String,
            userName As String,
            password As String,
            showUI As Boolean,
            connectionTimeout As Integer,
            overwrite As Boolean)

            Dim dialog As ProgressDialog = Nothing
            Try
                dialog = GetProgressDialog(address, destinationFileName, showUI)

                Dim t As Task = DownloadFileAsync(
                    address,
                    destinationFileName,
                    userName,
                    password,
                    dialog,
                    connectionTimeout,
                    overwrite,
                    onUserCancel:=UICancelOption.ThrowException)

                If t.IsFaulted Then
                    ' This will be true if any parameters are bad
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
                    If TryCast(ex.InnerException, OperationCanceledException) IsNot Nothing AndAlso
                        Environment.UserInteractive Then

                        If showUI AndAlso Environment.UserInteractive Then
                            Try
                                IO.File.Delete(destinationFileName)
                            Catch
                                ' ignore error
                            End Try
                            Throw New OperationCanceledException()
                        End If
                    End If

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
                Throw VbUtils.GetArgumentNullException(NameOf(address))
            End If

            Dim addressUri As Uri = GetUri(address.Trim())

            ' Get network credentials
            Dim networkCredentials As ICredentials = GetNetworkCredentials(userName, password)

            Dim dialog As ProgressDialog = Nothing
            Try
                If showUI AndAlso Environment.UserInteractive Then
                    'Construct the local file. This will validate the full name and path
                    Dim fullFilename As String = CompilerServices.FileSystemUtils.NormalizeFilePath(destinationFileName, NameOf(destinationFileName))
                    dialog = GetProgressDialog(address, destinationFileName, showUI)
                End If

                Dim t As Task = DownloadFileAsync(
                    addressUri,
                    destinationFileName,
                    networkCredentials,
                    dialog,
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
        ''' <param name="destinationFileName">Name and path of file where download is saved.</param>
        ''' <param name="networkCredentials">The credentials of the user performing the download.</param>
        ''' <param name="showUI">Indicates whether or not to show a progress bar.</param>
        ''' <param name="connectionTimeout">Time allotted before giving up on a connection.</param>
        ''' <param name="overwrite">Indicates whether or not the file should be overwritten if local file already exists.</param>
        ''' <remarks>Calls to all the other overloads will come through here.</remarks>
        Public Sub DownloadFile(
            address As Uri,
            destinationFileName As String,
            networkCredentials As ICredentials,
            showUI As Boolean,
            connectionTimeout As Integer,
            overwrite As Boolean)

            If address Is Nothing Then
                Throw VbUtils.GetArgumentNullException(NameOf(address))
            End If

            Dim dialog As ProgressDialog = Nothing
            Try
                dialog = GetProgressDialog(address.AbsolutePath, destinationFileName, showUI)
                Dim t As Task = DownloadFileAsync(
                    addressUri:=address,
                    destinationFileName,
                    networkCredentials,
                    dialog,
                    connectionTimeout,
                    overwrite)

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
        ''' <param name="networkCredentials">The credentials of the user performing the download.</param>
        ''' <param name="showUI">Indicates whether or not to show a progress bar.</param>
        ''' <param name="connectionTimeout">Time allotted before giving up on a connection.</param>
        ''' <param name="overwrite">Indicates whether or not the file should be overwritten if local file already exists.</param>
        ''' <param name="onUserCancel">Indicates what to do if user cancels dialog (either throw or do nothing).</param>
        ''' <remarks>Calls to all the other overloads will come through here.</remarks>
        Public Sub DownloadFile(
            address As Uri,
            destinationFileName As String,
            networkCredentials As ICredentials,
            showUI As Boolean,
            connectionTimeout As Integer,
            overwrite As Boolean,
            onUserCancel As UICancelOption)

            If connectionTimeout <= 0 Then
                Throw VbUtils.GetArgumentExceptionWithArgName(NameOf(connectionTimeout), SR.Network_BadConnectionTimeout)
            End If

            If address Is Nothing Then
                Throw VbUtils.GetArgumentNullException(NameOf(address))
            End If

            Dim dialog As ProgressDialog = Nothing
            Try
                dialog = GetProgressDialog(address.AbsolutePath, destinationFileName, showUI)
                Dim t As Task = DownloadFileAsync(
                    addressUri:=address,
                    destinationFileName,
                    networkCredentials,
                    dialog,
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
        ''' <param name="destinationFileName">Name and path of file where download is saved.</param>
        ''' <param name="userName">The name of the user performing the download.</param>
        ''' <param name="password">The user's password.</param>
        ''' <param name="showUI">Indicates whether or not to show a progress bar.</param>
        ''' <param name="connectionTimeout">Time allotted before giving up on a connection.</param>
        ''' <param name="overwrite">Indicates whether or not the file should be overwritten if local file already exists.</param>
        Public Sub DownloadFile(
            address As Uri,
            destinationFileName As String,
            userName As String,
            password As String,
            showUI As Boolean,
            connectionTimeout As Integer,
            overwrite As Boolean)

            If address Is Nothing Then
                Throw VbUtils.GetArgumentNullException(NameOf(address))
            End If

            Dim dialog As ProgressDialog = Nothing
            Dim networkCredentials As ICredentials = GetNetworkCredentials(userName, password)

            Try
                dialog = GetProgressDialog(address.AbsolutePath, destinationFileName, showUI)
                Dim t As Task = DownloadFileAsync(
                    addressUri:=address,
                    destinationFileName,
                    networkCredentials,
                    dialog,
                    connectionTimeout,
                    overwrite,
                    onUserCancel:=UICancelOption.ThrowException)

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
        ''' <param name="userName">The name of the user performing the download.</param>
        ''' <param name="password">The user's password.</param>
        ''' <param name="showUI">Indicates whether or not to show a progress bar.</param>
        ''' <param name="connectionTimeout">Time allotted before giving up on a connection.</param>
        ''' <param name="overwrite">Indicates whether or not the file should be overwritten if local file already exists.</param>
        ''' <param name="onUserCancel">Indicates what to do if user cancels dialog (either throw or do nothing).</param>
        Public Sub DownloadFile(
            address As Uri,
            destinationFileName As String,
            userName As String,
            password As String,
            showUI As Boolean,
            connectionTimeout As Integer,
            overwrite As Boolean,
            onUserCancel As UICancelOption)

            If connectionTimeout <= 0 Then
                Throw VbUtils.GetArgumentExceptionWithArgName(
                    argumentName:=NameOf(connectionTimeout),
                    resourceKey:=SR.Network_BadConnectionTimeout)
            End If

            If address Is Nothing Then
                Throw VbUtils.GetArgumentNullException(NameOf(address))
            End If

            ' Get network credentials
            Dim networkCredentials As ICredentials = GetNetworkCredentials(userName, password)

            Dim dialog As ProgressDialog = Nothing
            Try
                dialog = GetProgressDialog(address.AbsolutePath, destinationFileName, showUI)
                Dim t As Task = DownloadFileAsync(
                    addressUri:=address,
                    destinationFileName,
                    networkCredentials,
                    dialog,
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
        ''' <param name="destinationFileName">Name and path of file where download is saved.</param>
        Public Sub DownloadFile(address As Uri, destinationFileName As String)
            Try

                DownloadFileAsync(
                    addressUri:=address,
                    destinationFileName,
                    userName:=DEFAULT_USERNAME,
                    password:=DEFAULT_PASSWORD,
                    dialog:=Nothing,
                    connectionTimeout:=DEFAULT_TIMEOUT,
                    overwrite:=False).Wait()
            Catch ex As Exception
                If ex.InnerException IsNot Nothing Then
                    Throw ex.InnerException
                End If
                Throw
            End Try
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

            Try
                DownloadFileAsync(
                    addressUri:=address,
                    destinationFileName,
                    userName,
                    password,
                    dialog:=Nothing,
                    connectionTimeout:=DEFAULT_TIMEOUT,
                    overwrite:=False).Wait()
            Catch ex As Exception
                If ex.InnerException IsNot Nothing Then
                    Throw ex.InnerException
                End If
                Throw
            End Try
        End Sub

    End Class
End Namespace
