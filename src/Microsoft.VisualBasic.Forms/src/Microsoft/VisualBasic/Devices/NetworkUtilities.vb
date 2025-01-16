' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Net
Imports System.Windows.Forms
Imports Microsoft.VisualBasic.CompilerServices
Imports Microsoft.VisualBasic.MyServices.Internal

Imports VbUtils = Microsoft.VisualBasic.CompilerServices.ExceptionUtils

Namespace Microsoft.VisualBasic.Devices
    Friend Module NetworkUtilities

        ' Password used in overloads where there is no password parameter
        Friend Const DEFAULT_PASSWORD As String = ""

        ' Default timeout value
        Friend Const DEFAULT_TIMEOUT As Integer = 100000

        ' UserName used in overloads where there is no userName parameter
        Friend Const DEFAULT_USERNAME As String = ""

        ''' <summary>
        '''  Posts a message to close the <see cref="ProgressDialog"/>.
        ''' </summary>
        Friend Sub CloseProgressDialog(dialog As ProgressDialog)
            ' Don't invoke unless dialog is up and running
            If dialog IsNot Nothing Then
                dialog.IndicateClosing()

                If dialog.IsHandleCreated Then
                    dialog.BeginInvoke(New MethodInvoker(AddressOf dialog.CloseDialog))
                Else
                    ' Ensure dialog is closed. If we get here it means the file was copied before the handle for
                    ' the progress dialog was created.
                    dialog.Close()
                End If
            End If
        End Sub

        ''' <summary>
        '''  Gets network credentials from a userName and password.
        ''' </summary>
        ''' <param name="userName">The name of the user.</param>
        ''' <param name="password">The password of the user.</param>
        ''' <returns>A <see langword="New"/> <see cref="NetworkCredential"/> or <see langword="Nothing"/>.</returns>
        Friend Function GetNetworkCredentials(userName As String, password As String) As ICredentials

            Return If(String.IsNullOrWhiteSpace(userName) OrElse String.IsNullOrWhiteSpace(password),
                      Nothing,
                      DirectCast(New NetworkCredential(userName, password), ICredentials)
                     )
        End Function

        ''' <summary>
        '''  Centralize setup a <see cref="ProgressDialog"/> to be used with FileDownload and FileUpload.
        ''' </summary>
        ''' <param name="address">Address to the remote file, http, ftp etc...</param>
        ''' <param name="destinationFileName">Name and path of file where download is saved.</param>
        ''' <param name="showUI">Indicates whether or not to show a progress bar.</param>
        ''' <returns>
        '''  <see langword="New"/> <see cref="ProgressDialog"/> if InteractiveEnvironment <see langword="True"/>
        '''  otherwise return <see langword="Nothing"/>.
        ''' </returns>
        Friend Function GetProgressDialog(
            address As String,
            destinationFileName As String,
            showUI As Boolean) As ProgressDialog

            If InteractiveEnvironment(showUI) Then
                'Construct the local file. This will validate the full name and path
                Dim fullFilename As String = FileSystemUtils.NormalizeFilePath(
                    path:=destinationFileName,
                    paramName:=NameOf(destinationFileName))
                Return New ProgressDialog With {
                    .Text = VbUtils.GetResourceString(SR.ProgressDialogDownloadingTitle, address),
                    .LabelText = VbUtils.GetResourceString(
                        resourceKey:=SR.ProgressDialogDownloadingLabel,
                        address,
                        fullFilename)
                    }
            End If
            Return Nothing
        End Function

        ''' <summary>
        '''  Gets a <see cref="Uri"/> from a uri string.
        '''  We also use this function to validate the UriString (remote file address).
        ''' </summary>
        ''' <param name="address">The remote file address.</param>
        ''' <returns>
        '''  A <see cref="Uri"/> if successful, otherwise it throws an <see cref="UriFormatException"/>.
        ''' </returns>
        Friend Function GetUri(address As String) As Uri
            Try
                Return New Uri(address)
            Catch ex As UriFormatException
                'Throw an exception with an error message more appropriate to our API
                Throw GetArgumentExceptionWithArgName(
                    argumentName:=NameOf(address),
                    resourceKey:=SR.Network_InvalidUriString,
                    address)
            End Try
        End Function

        Friend Function InteractiveEnvironment(showUI As Boolean) As Boolean
            Return showUI AndAlso Environment.UserInteractive
        End Function

    End Module
End Namespace
