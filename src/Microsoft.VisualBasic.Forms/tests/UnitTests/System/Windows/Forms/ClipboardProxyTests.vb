' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Collections.Specialized
Imports System.IO
Imports System.Threading
Imports System.Windows.Forms
Imports FluentAssertions
Imports Xunit

Namespace Microsoft.VisualBasic.Forms.Tests

    ''' <summary>
    '''  These are just checking the Proxy functions, the underlying functions are tested elsewhere.
    ''' </summary>
    <Collection("Sequential")>
    <UISettings(MaxAttempts:=3)>
    Public Class ClipboardProxyTests

        <WinFormsFact>
        Public Sub ClipboardProxy_ContainsText()
            Dim clipboardProxy As New MyServices.ClipboardProxy
            Dim text As String = GetUniqueText()
            clipboardProxy.SetText(text)
            Clipboard.ContainsText.Should.Be(clipboardProxy.ContainsText)
            Clipboard.ContainsText.Should.Be(clipboardProxy.ContainsText(TextDataFormat.Text))
            Dim expected As Boolean = clipboardProxy.ContainsData(DataFormats.Text)
            Clipboard.ContainsData(DataFormats.Text).Should.Be(expected)
            clipboardProxy.GetText().Should.Be(text)
            text = GetUniqueText()
            clipboardProxy.SetText(text, TextDataFormat.Text)
            clipboardProxy.GetText(TextDataFormat.Text).Should.Be(text)
        End Sub

        <WinFormsFact>
        Public Sub ClipboardProxy_GetAudioStream_InvokeMultipleTimes_Success()
            Dim clipboardProxy As New MyServices.ClipboardProxy
            Dim audioBytes As Byte() = {1, 2, 3}
            clipboardProxy.SetAudio(audioBytes)
            Dim result As Stream = clipboardProxy.GetAudioStream()
            clipboardProxy.GetAudioStream().Length.Should.Be(result.Length)
        End Sub

        <WinFormsFact>
        Public Sub ClipboardProxy_SetAudio_InvokeByteArray_GetReturnsExpected()
            Dim clipboardProxy As New MyServices.ClipboardProxy
            Dim audioBytes As Byte() = {1, 2, 3}
            clipboardProxy.SetAudio(audioBytes)
            clipboardProxy.ContainsAudio().Should.BeTrue()
            clipboardProxy.ContainsData(DataFormats.WaveAudio).Should.BeTrue()
            Dim memoryStream As MemoryStream = CType(clipboardProxy.GetData(DataFormats.WaveAudio), MemoryStream)
            memoryStream.Length.Should.Be(audioBytes.Length)
            Dim audioStream As MemoryStream = CType(clipboardProxy.GetAudioStream(), MemoryStream)
            audioStream.Should.NotBeNull()
            audioBytes.Length.Should.Be(CInt(audioStream.Length))
        End Sub

        <WinFormsFact>
        Public Sub ClipboardProxy_SetAudio_InvokeStream_GetReturnsExpected()
            Dim clipboardProxy As New MyServices.ClipboardProxy
            Dim audioBytes() As Byte = {1, 2, 3}
            Using audioStream As New MemoryStream(audioBytes)
                clipboardProxy.SetAudio(audioStream)
                audioStream.Should.BeOfType(Of MemoryStream)()
                Dim length As Integer = DirectCast(clipboardProxy.GetAudioStream(), MemoryStream).ToArray().Length
                audioBytes.Length.Should.Be(length)
                Dim waveAudio As MemoryStream = CType(clipboardProxy.GetData(DataFormats.WaveAudio), MemoryStream)
                waveAudio.ToArray.Length.Should.Be(audioBytes.Length)
                clipboardProxy.ContainsAudio().Should.BeTrue()
                clipboardProxy.ContainsData(DataFormats.WaveAudio).Should.BeTrue()
            End Using
        End Sub

        <WinFormsFact>
        Public Sub ClipboardProxy_SetFileDropList_Invoke_GetReturnsExpected()
            Dim clipboardProxy As New MyServices.ClipboardProxy
            Dim filePaths As New StringCollection From {"filePath", "filePath2"}
            clipboardProxy.SetFileDropList(filePaths)
            clipboardProxy.ContainsFileDropList().Should.BeTrue()
            clipboardProxy.GetFileDropList().Equals(filePaths)
        End Sub

    End Class
End Namespace
