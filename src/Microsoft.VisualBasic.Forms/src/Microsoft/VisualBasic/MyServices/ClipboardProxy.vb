' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Collections.Specialized
Imports System.ComponentModel
Imports System.Drawing
Imports System.IO
Imports System.Windows.Forms

Namespace Microsoft.VisualBasic.MyServices

    ''' <summary>
    '''  A class that wraps <see cref="Clipboard"/> so that
    '''  a clipboard can be instanced.
    ''' </summary>
    <EditorBrowsable(EditorBrowsableState.Never)>
    Public Class ClipboardProxy

        ''' <summary>
        '''  Only allows instantiation of the class
        ''' </summary>
        Friend Sub New()
        End Sub

        ''' <inheritdoc cref="Clipboard.Clear()"/>
        Public Sub Clear()
            Clipboard.Clear()
        End Sub

        ''' <inheritdoc cref="Clipboard.ContainsAudio()"/>
        Public Function ContainsAudio() As Boolean
            Return Clipboard.ContainsAudio()
        End Function

        ''' <inheritdoc cref="Clipboard.ContainsData(String)"/>
        Public Function ContainsData(format As String) As Boolean
            Return Clipboard.ContainsData(format)
        End Function

        ''' <inheritdoc cref="Clipboard.ContainsFileDropList()"/>
        Public Function ContainsFileDropList() As Boolean
            Return Clipboard.ContainsFileDropList()
        End Function

        ''' <inheritdoc cref="Clipboard.ContainsImage()"/>
        Public Function ContainsImage() As Boolean
            Return Clipboard.ContainsImage()
        End Function

        ''' <inheritdoc cref="Clipboard.ContainsText()"/>
        Public Function ContainsText() As Boolean
            Return Clipboard.ContainsText
        End Function

        ''' <inheritdoc cref="Clipboard.ContainsText()"/>
        Public Function ContainsText(format As TextDataFormat) As Boolean
            Return Clipboard.ContainsText(format)
        End Function

        ''' <inheritdoc cref="Clipboard.GetAudioStream()"/>
        Public Function GetAudioStream() As Stream
            Return Clipboard.GetAudioStream()
        End Function

        ''' <inheritdoc cref="Clipboard.GetData(String)"/>
        Public Function GetData(format As String) As Object
            Return Clipboard.GetData(format)
        End Function

        ''' <inheritdoc cref="Clipboard.GetDataObject()"/>
        <EditorBrowsable(EditorBrowsableState.Advanced)>
        Public Function GetDataObject() As IDataObject
            Return Clipboard.GetDataObject()
        End Function

        ''' <inheritdoc cref="Clipboard.GetFileDropList()"/>
        Public Function GetFileDropList() As StringCollection
            Return Clipboard.GetFileDropList()
        End Function

        ''' <inheritdoc cref="Clipboard.GetImage()"/>
        Public Function GetImage() As Image
            Return Clipboard.GetImage()
        End Function

        ''' <inheritdoc cref="Clipboard.GetText()"/>
        Public Function GetText() As String
            Return Clipboard.GetText()
        End Function

        ''' <inheritdoc cref="Clipboard.GetText(TextDataFormat)"/>
        Public Function GetText(format As TextDataFormat) As String
            Return Clipboard.GetText(format)
        End Function

        ''' <inheritdoc cref="Clipboard.SetAudio(Byte())"/>
        Public Sub SetAudio(audioBytes As Byte())
            Clipboard.SetAudio(audioBytes)
        End Sub

        ''' <inheritdoc cref="Clipboard.SetAudio(Stream)"/>
        Public Sub SetAudio(audioStream As Stream)
            Clipboard.SetAudio(audioStream)
        End Sub

        ''' <inheritdoc cref="Clipboard.SetData(String, Object)"/>
        Public Sub SetData(format As String, data As Object)
            Clipboard.SetData(format, data)
        End Sub

        ''' <inheritdoc cref="SetDataObject(DataObject)"/>
        <EditorBrowsable(EditorBrowsableState.Advanced)>
        Public Sub SetDataObject(data As DataObject)
            Clipboard.SetDataObject(data)
        End Sub

        ''' <inheritdoc cref="SetFileDropList(StringCollection)"/>
        Public Sub SetFileDropList(filePaths As StringCollection)
            Clipboard.SetFileDropList(filePaths)
        End Sub

        ''' <inheritdoc cref="SetImage(Image)"/>
        Public Sub SetImage(image As Image)
            Clipboard.SetImage(image)
        End Sub

        ''' <inheritdoc cref="SetText(String)"/>
        Public Sub SetText(text As String)
            Clipboard.SetText(text)
        End Sub

        ''' <inheritdoc cref="SetText(String, TextDataFormat)"/>
        Public Sub SetText(text As String, format As TextDataFormat)
            Clipboard.SetText(text, format)
        End Sub

    End Class
End Namespace
