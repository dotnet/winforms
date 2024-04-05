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
        Private ReadOnly _clipboardProxy As New MyServices.ClipboardProxy

        <WinFormsFact>
        Public Sub Clipboard_ContainsText()
            Dim text As String = "This is a Test!"
            _clipboardProxy.SetText(text)
            Assert.Equal(text, _clipboardProxy.GetText())
        End Sub

        <WinFormsFact>
        Public Sub Clipboard_ContainsTextWithFormat()
            Dim text As String = "This is a Test!"
            _clipboardProxy.SetText(text, TextDataFormat.Text)
            Assert.Equal(text, _clipboardProxy.GetText(TextDataFormat.Text))
        End Sub

        <WinFormsFact>
        Public Sub Clipboard_GetAudioStream_InvokeMultipleTimes_Success()
            Dim result As Stream = _clipboardProxy.GetAudioStream()
            Assert.Equal(result.Length, _clipboardProxy.GetAudioStream().Length)
        End Sub

        <WinFormsFact>
        Public Sub Clipboard_SetAudio_InvokeByteArray_GetReturnsExpected()
            Dim audioBytes As Byte() = {1, 2, 3}
            _clipboardProxy.SetAudio(audioBytes)
            Assert.True(_clipboardProxy.ContainsAudio())
            Assert.True(_clipboardProxy.ContainsData(DataFormats.WaveAudio))
            Assert.Equal(audioBytes.Length, Assert.IsType(Of MemoryStream)(Assert.IsType(Of MemoryStream)(CType(_clipboardProxy.GetAudioStream(), MemoryStream))).Length)
            Assert.Equal(audioBytes.Length, CType(_clipboardProxy.GetData(DataFormats.WaveAudio), MemoryStream).Length)
        End Sub

        <WinFormsFact>
        Public Sub Clipboard_SetAudio_InvokeStream_GetReturnsExpected()
            Dim audioBytes() As Byte = {1, 2, 3}
            Using audioStream As New MemoryStream(audioBytes)
                _clipboardProxy.SetAudio(audioStream)
                Assert.Equal(audioBytes.Length, Assert.IsType(Of MemoryStream)(_clipboardProxy.GetAudioStream()).ToArray().Length)
                Assert.Equal(audioBytes.Length, Assert.IsType(Of MemoryStream)(_clipboardProxy.GetData(DataFormats.WaveAudio)).ToArray().Length)
                Assert.True(_clipboardProxy.ContainsAudio())
                Assert.True(_clipboardProxy.ContainsData(DataFormats.WaveAudio))
            End Using
        End Sub

        <WinFormsFact>
        Public Sub Clipboard_SetFileDropList_Invoke_GetReturnsExpected()
            Dim filePaths As New StringCollection From
                {"filePath", "filePath2"}

            _clipboardProxy.SetFileDropList(filePaths)
            Assert.Equal(filePaths, _clipboardProxy.GetFileDropList())
            Assert.True(_clipboardProxy.ContainsFileDropList())
        End Sub

    End Class
End Namespace
