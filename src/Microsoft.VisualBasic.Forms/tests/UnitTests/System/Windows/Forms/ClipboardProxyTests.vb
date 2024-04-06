' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Collections.Specialized
Imports System.IO
Imports System.Windows.Forms
Imports Xunit

Namespace Microsoft.VisualBasic.Forms.Tests

    ''' <summary>
    ''' These are just checking the Proxy functions, the underlaying functions are tested elsewhere
    ''' </summary>
    Partial Public Class ClipboardProxyTests
        Private ReadOnly _lockObject As New Object

        Private Function GetUniqueText() As String
            Return Guid.NewGuid().ToString("D")
        End Function

        <WinFormsFact>
        Public Sub Clipboard_ContainsText()
            SyncLock _lockObject
                Dim text As String = GetUniqueText()
                Dim clipboardProxy As New MyServices.ClipboardProxy
                clipboardProxy.SetText(text)
                Assert.Equal(text, clipboardProxy.GetText())
            End SyncLock
        End Sub

        <WinFormsFact>
        Public Sub Clipboard_ContainsTextWithFormat()
            SyncLock _lockObject
                Dim text As String = GetUniqueText()
                Dim clipboardProxy As New MyServices.ClipboardProxy
                clipboardProxy.SetText(text, TextDataFormat.Text)
                Assert.Equal(text, clipboardProxy.GetText(TextDataFormat.Text))
            End SyncLock
        End Sub

        <WinFormsFact>
        Public Sub Clipboard_GetAudioStream_InvokeMultipleTimes_Success()
            SyncLock _lockObject
                Dim clipboardProxy As New MyServices.ClipboardProxy
                Dim result As Stream = clipboardProxy.GetAudioStream()
                Assert.Equal(result.Length, clipboardProxy.GetAudioStream().Length)
            End SyncLock
        End Sub

        <WinFormsFact>
        Public Sub Clipboard_SetAudio_InvokeByteArray_GetReturnsExpected()
            SyncLock _lockObject
                Dim audioBytes As Byte() = {1, 2, 3}
                Dim clipboardProxy As New MyServices.ClipboardProxy
                clipboardProxy.SetAudio(audioBytes)
                Assert.True(clipboardProxy.ContainsAudio())
                Assert.True(clipboardProxy.ContainsData(DataFormats.WaveAudio))
                Assert.Equal(audioBytes.Length, Assert.IsType(Of MemoryStream)(Assert.IsType(Of MemoryStream)(CType(clipboardProxy.GetAudioStream(), MemoryStream))).Length)
                Assert.Equal(audioBytes.Length, CType(clipboardProxy.GetData(DataFormats.WaveAudio), MemoryStream).Length)
            End SyncLock
        End Sub

        <WinFormsFact>
        Public Sub Clipboard_SetAudio_InvokeStream_GetReturnsExpected()
            SyncLock _lockObject
                Dim audioBytes() As Byte = {1, 2, 3}
                Dim clipboardProxy As New MyServices.ClipboardProxy
                Using audioStream As New MemoryStream(audioBytes)
                    clipboardProxy.SetAudio(audioStream)
                    Assert.Equal(audioBytes.Length, Assert.IsType(Of MemoryStream)(clipboardProxy.GetAudioStream()).ToArray().Length)
                    Assert.Equal(audioBytes.Length, Assert.IsType(Of MemoryStream)(clipboardProxy.GetData(DataFormats.WaveAudio)).ToArray().Length)
                    Assert.True(clipboardProxy.ContainsAudio())
                    Assert.True(clipboardProxy.ContainsData(DataFormats.WaveAudio))
                End Using
            End SyncLock
        End Sub

        <WinFormsFact>
        Public Sub Clipboard_SetFileDropList_Invoke_GetReturnsExpected()
            SyncLock _lockObject
                Dim clipboardProxy As New MyServices.ClipboardProxy
                Dim filePaths As New StringCollection From
                {"filePath", "filePath2"}
                clipboardProxy.SetFileDropList(filePaths)
                Assert.True(clipboardProxy.ContainsFileDropList())
                Assert.Equal(filePaths, clipboardProxy.GetFileDropList())
            End SyncLock
        End Sub

    End Class
End Namespace
