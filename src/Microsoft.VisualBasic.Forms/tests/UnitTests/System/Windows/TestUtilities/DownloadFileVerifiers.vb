' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.IO
Imports System.Net
Imports FluentAssertions

Namespace Microsoft.VisualBasic.Forms.Tests

    Public Module DownloadFileVerifiers
        ''' <summary>
        '''  Verify that testDirectory exists, that destinationFileName exist and what its length is.
        ''' </summary>
        ''' <param name="testDirectory">A Unique directory under the systems Temp directory.</param>
        ''' <param name="destinationFileName">The full path and filename of the new file.</param>
        ''' <param name="listener"></param>
        Friend Sub VerifyFailedDownload(
                testDirectory As String,
                destinationFileName As String,
                listener As HttpListener)

            If Not String.IsNullOrWhiteSpace(testDirectory) Then
                Directory.Exists(testDirectory).Should.BeTrue()
            End If
            If Not String.IsNullOrWhiteSpace(destinationFileName) Then
                Call New FileInfo(destinationFileName).Exists.Should.BeFalse()
            End If
            listener.Stop()
            listener.Close()
        End Sub

        ''' <summary>
        '''  Verify that testDirectory exists, that destinationFileName exist and what its length is.
        ''' </summary>
        ''' <param name="testDirectory">A Unique directory under the systems Temp directory.</param>
        ''' <returns>
        '''  The size in bytes of the destination file, this saves the caller from having to
        '''  do another FileInfo call.
        ''' </returns>
        ''' <param name="destinationFileName">The full path and filename of the new file.</param>
        ''' <param name="listener"></param>
        Friend Function VerifySuccessfulDownload(
                testDirectory As String,
                destinationFileName As String,
                listener As HttpListener) As Long

            Directory.Exists(testDirectory).Should.BeTrue()
            Dim fileInfo As New FileInfo(destinationFileName)
            fileInfo.Exists.Should.BeTrue()
            Directory.Exists(fileInfo.DirectoryName).Should.BeTrue()
            ' This directory should not be systems Temp Directory because it must be created
            Path.GetTempPath.Should.NotBe(fileInfo.DirectoryName)
            listener.Stop()
            listener.Close()
            Return fileInfo.Length
        End Function

    End Module
End Namespace
