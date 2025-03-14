' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Collections.Specialized
Imports System.IO
Imports System.Windows.Forms
Imports FluentAssertions
Imports Xunit

Namespace Microsoft.VisualBasic.Forms.Tests

    ''' <summary>
    '''  These are just checking the <see cref="MyServices.ClipboardProxy" /> functions,
    '''  the underlying functions are tested elsewhere.
    ''' </summary>
    <Collection("Sequential")>
    <UISettings(MaxAttempts:=3)>
    Public Class ClipboardProxyTests

        ''' <summary>
        '''  Testing only that <see cref=" MyServices.ClipboardProxy"/> contains <see cref="TextDataFormat.Text"/>.
        ''' </summary>
        <WinFormsFact>
        Public Sub ClipboardProxy_ContainsTextOfSpecificFormat()
            Dim clipboardProxy As New MyServices.ClipboardProxy
            Dim text As String = GetUniqueText()
            clipboardProxy.SetText(text)
            Dim expected As Boolean = clipboardProxy.ContainsText(format:=TextDataFormat.Text)
            Clipboard.ContainsText(format:=TextDataFormat.Text).Should.Be(expected)
        End Sub

        <WinFormsFact>
        Public Sub ClipboardProxy_SetAudio_InvokeByteArray_GetReturnsExpected()
            Dim clipboardProxy As New MyServices.ClipboardProxy
            Dim audioBytes As Byte() = {1, 2, 3}
            clipboardProxy.SetAudio(audioBytes)
            clipboardProxy.ContainsAudio().Should.Be(Clipboard.ContainsAudio)
            Using clipboardProxyAudio As MemoryStream = CType(clipboardProxy.GetAudioStream, MemoryStream)
                Using clipboardAudio As MemoryStream = CType(Clipboard.GetAudioStream, MemoryStream)
                    clipboardProxyAudio.ToArray.Should.BeEquivalentTo(clipboardAudio.ToArray)
                End Using
            End Using
        End Sub

        <WinFormsFact>
        Public Sub ClipboardProxy_SetAudio_InvokeStream_GetReturnsExpected()
            Dim clipboardProxy As New MyServices.ClipboardProxy
            Dim audioBytes() As Byte = {1, 2, 3}
            Using audioStream As New MemoryStream(audioBytes)
                clipboardProxy.SetAudio(audioStream)
                clipboardProxy.ContainsAudio().Should.Be(Clipboard.ContainsAudio)
                Using clipboardProxyWaveAudio As MemoryStream = CType(clipboardProxy.GetAudioStream, MemoryStream)
                    Using clipboardWaveAudio As MemoryStream = CType(Clipboard.GetAudioStream, MemoryStream)
                        clipboardProxyWaveAudio.ToArray.Should.BeEquivalentTo(clipboardWaveAudio.ToArray)
                    End Using
                End Using

            End Using
        End Sub

        <WinFormsFact>
        Public Sub ClipboardProxy_FileDropList_Invoke_GetReturnsExpected()
            Dim clipboardProxy As New MyServices.ClipboardProxy
            Dim filePaths As New StringCollection From {"filePath", "filePath2"}
            clipboardProxy.SetFileDropList(filePaths)
            clipboardProxy.ContainsFileDropList().Should.Be(Clipboard.ContainsFileDropList)
            clipboardProxy.GetFileDropList.Should.BeEquivalentTo(Clipboard.GetFileDropList)
        End Sub

    End Class
End Namespace
