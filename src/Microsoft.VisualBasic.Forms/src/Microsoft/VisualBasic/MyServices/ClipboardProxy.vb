' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Collections.Specialized
Imports System.ComponentModel
Imports System.Drawing
Imports System.IO
Imports System.Windows.Forms
Imports System.Reflection.Metadata
Imports System.Runtime.InteropServices

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

        ''' <summary>
        '''  Removes all data from the <see cref="Clipboard"/>.
        ''' </summary>
        Public Sub Clear()
            Clipboard.Clear()
        End Sub

        ''' <summary>
        '''  Indicates whether there is data on the <see cref="Clipboard"/>
        '''  in the <see cref="DataFormats.WaveAudio"/> format./>.
        ''' </summary>
        ''' <returns>
        '''  <see langword="True"/> if an audio <see cref="Stream"/> is available,
        '''  otherwise <see langword="False"/>.
        ''' </returns>
        Public Function ContainsAudio() As Boolean
            Return Clipboard.ContainsAudio()
        End Function

        ''' <summary>
        '''  Indicates whether or not there is data on the <see cref="Clipboard"/> in the passed in format
        '''  or can be converted to that format.
        ''' </summary>
        ''' <param name="format"></param>
        ''' <returns>
        '''  <see langword="True"/> if there's data in the passed in format,
        '''  otherwise <see langword="False"/>.
        ''' </returns>
        Public Function ContainsData(format As String) As Boolean
            Return Clipboard.ContainsData(format)
        End Function

        ''' <summary>
        '''  Indicates whether there is data on the <see cref="Clipboard"/>
        '''  that is in the <see cref="DataFormats.FileDrop"/> format or can be converted to that format.
        ''' </summary>
        ''' <returns>
        '''  <see langword="True"/> if a file drop list is available,
        '''  otherwise <see langword="False"/>.
        ''' </returns>
        Public Function ContainsFileDropList() As Boolean
            Return Clipboard.ContainsFileDropList()
        End Function

        ''' <summary>
        '''  Indicates whether there Is data on the <see cref="Clipboard"/>
        '''  that Is in the <see cref="DataFormats.Bitmap"/> format or can be converted to that format.
        ''' </summary>
        ''' <returns>
        '''  <see langword="True"/> if an image is available,
        '''  otherwise <see langword="False"/>.
        ''' </returns>
        Public Function ContainsImage() As Boolean
            Return Clipboard.ContainsImage()
        End Function

        ''' <summary>
        '''  Indicates whether there is text data on the <see cref="Clipboard"/>
        '''  in <see cref="TextDataFormat.UnicodeText"/> format.
        ''' </summary>
        ''' <returns>
        '''  <see langword="True"/> if text is available,
        '''  otherwise <see langword="False"/>.
        ''' </returns>
        Public Function ContainsText() As Boolean
            Return Clipboard.ContainsText
        End Function

        ''' <summary>
        '''  Indicates whether there is text data on the <see cref="Clipboard"/> in the format
        '''  indicated by the specified <see cref="TextDataFormat"/> value.
        ''' </summary>
        ''' <param name="format">The type of text being checked for.</param>
        ''' <returns>
        '''  <see langword="True"/> if text is available,
        '''  otherwise <see langword="False"/>.
        ''' </returns>
        Public Function ContainsText(format As TextDataFormat) As Boolean
            Return Clipboard.ContainsText(format)
        End Function

        ''' <summary>
        '''   Retrieves an audio stream from the <see cref="Clipboard"/>.
        ''' </summary>
        ''' <returns>The audio stream as a <see cref="Stream"/>.</returns>
        Public Function GetAudioStream() As Stream
            Return Clipboard.GetAudioStream()
        End Function

        ''' <summary>
        '''  Gets data from the <see cref="Clipboard"/> that's been saved in the passed in format.
        ''' </summary>
        ''' <param name="format">The type of data being sought.</param>
        ''' <returns>The data.</returns>
        <Obsolete(
            Obsoletions.ClipboardProxyGetDataMessage,
            False,
            DiagnosticId:=Obsoletions.ClipboardGetDataDiagnosticId,
            UrlFormat:=Obsoletions.SharedUrlFormat)>
        Public Function GetData(format As String) As Object
#Disable Warning WFDEV005 ' Type or member is obsolete
            Return Clipboard.GetData(format)
#Enable Warning WFDEV005
        End Function

        ''' <summary>
        '''  Retrieves the data that is currently on the system <see cref="Clipboard"/>.
        ''' </summary>
        ''' <returns>The <see cref="IDataObject"/>.</returns>
        ''' <remarks>
        '''  This gives the ability to save an object in multiple formats.
        ''' </remarks>
        <EditorBrowsable(EditorBrowsableState.Advanced)>
        Public Function GetDataObject() As IDataObject
            Return Clipboard.GetDataObject()
        End Function

        ''' <summary>
        '''  Retrieves a collection of file names from the <see cref="Clipboard"/>.
        ''' </summary>
        ''' <returns>The list of file paths as a <see cref="StringCollection"/>.</returns>
        Public Function GetFileDropList() As StringCollection
            Return Clipboard.GetFileDropList()
        End Function

        ''' <summary>
        '''  Retrieves an <see cref="Image"/> from the <see cref="Clipboard"/>.
        ''' </summary>
        ''' <returns><see cref="Image"/></returns>
        Public Function GetImage() As Image
            Return Clipboard.GetImage()
        End Function

        ''' <summary>
        '''  Retrieves text data from the <see cref="Clipboard"/>
        '''  in the <see cref="TextDataFormat.UnicodeText"/> format.
        ''' </summary>
        ''' <returns>The text as a <see cref="String"/>.</returns>
        Public Function GetText() As String
            Return Clipboard.GetText()
        End Function

        ''' <summary>
        '''  Retrieves text data from the <see cref="Clipboard"/>
        '''  in the format indicated by the specified <see cref="TextDataFormat"/> value.
        ''' </summary>
        ''' <param name="format">The type of text to get.</param>
        ''' <returns>The text as a <see cref="String"/>.</returns>
        Public Function GetText(format As TextDataFormat) As String
            Return Clipboard.GetText(format)
        End Function

        ''' <summary>
        '''  Clears the <see cref="Clipboard"/> and
        '''  then adds data in the <see cref="DataFormats.WaveAudio"/> format.
        ''' </summary>
        ''' <param name="audioBytes">The byte array to be saved.</param>
        Public Sub SetAudio(audioBytes As Byte())
            Clipboard.SetAudio(audioBytes)
        End Sub

        ''' <summary>
        '''  Clears the <see cref="Clipboard"/> and
        '''  then adds data in the <see cref="DataFormats.WaveAudio"/> format.
        ''' </summary>
        ''' <param name="audioStream">The <see cref="Stream"/> to be saved.</param>
        Public Sub SetAudio(audioStream As Stream)
            Clipboard.SetAudio(audioStream)
        End Sub

        ''' <summary>
        '''  Clears the <see cref="Clipboard"/> and
        '''  then adds data in the specified format.
        ''' </summary>
        ''' <param name="format">The format in which to save the data.</param>
        ''' <param name="data">The data to be saved.</param>
        Public Sub SetData(format As String, data As Object)
            Clipboard.SetData(format, data)
        End Sub

        ''' <summary>
        '''  Places nonpersistent <see cref="DataObject"/> on the <see cref="Clipboard"/>.
        ''' </summary>
        ''' <param name="data">The <see cref="DataObject"/> to be saved.</param>
        ''' <remarks>This gives the ability to save an object in multiple formats.</remarks>
        <EditorBrowsable(EditorBrowsableState.Advanced)>
        Public Sub SetDataObject(data As DataObject)
            Clipboard.SetDataObject(data)
        End Sub

        ''' <summary>
        '''  Clears the <see cref="Clipboard"/>> and then adds a collection of file names
        '''  in the <see cref="DataFormats.FileDrop"/> format.
        ''' </summary>
        ''' <param name="filePaths">
        '''  The file drop list as a <see cref="StringCollection"/>.
        ''' </param>
        Public Sub SetFileDropList(filePaths As StringCollection)
            Clipboard.SetFileDropList(filePaths)
        End Sub

        ''' <inheritdoc cref="Clipboard.TryGetData(Of T)(String, Func(Of TypeName, Type), ByRef T)" />
        Public Function TryGetData(Of T)(format As String, resolver As Func(Of TypeName, Type), <Out> ByRef data As T) As Boolean
            Return Clipboard.TryGetData(format, resolver, data)
        End Function

        ''' <inheritdoc cref="Clipboard.TryGetData(Of T)(String, ByRef T)" />
        Public Function TryGetData(Of T)(format As String, <Out> ByRef data As T) As Boolean
            Return Clipboard.TryGetData(format, data)
        End Function

        ''' <summary>
        '''  Saves the passed in <see cref="Image"/> to the clipboard.
        ''' </summary>
        ''' <param name="image">The <see cref="Image"/> to be saved.</param>
        Public Sub SetImage(image As Image)
            Clipboard.SetImage(image)
        End Sub

        ''' <inheritdoc cref="Clipboard.SetDataAsJson(Of T)(String, T)"/>
        Public Sub SetDataAsJson(Of T)(format As String, data As T)
            Clipboard.SetDataAsJson(format, data)
        End Sub

        ''' <summary>
        '''  Saves the passed in <see cref="String" /> to the clipboard.
        ''' </summary>
        ''' <param name="text">The <see cref="String"/> to save.</param>
        Public Sub SetText(text As String)
            Clipboard.SetText(text)
        End Sub

        ''' <summary>
        '''  Saves the passed in <see cref="String" />  to the clipboard in the passed in <paramref name="format"/>.
        ''' </summary>
        ''' <param name="text">The <see cref="String" />  to save.</param>
        ''' <param name="format">The format in which to save the <see cref="String" /> .</param>
        Public Sub SetText(text As String, format As TextDataFormat)
            Clipboard.SetText(text, format)
        End Sub

    End Class
End Namespace
