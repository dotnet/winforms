' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Option Explicit On
Option Strict On

Imports System.Collections.Specialized
Imports System.ComponentModel
Imports System.Drawing
Imports System.IO
Imports System.Security.Permissions
Imports System.Windows.Forms

Namespace Microsoft.VisualBasic.MyServices

#Disable Warning SYSLIB0003 ' Type or member is obsolete
    ''' <summary>
    ''' A class that wraps System.Windows.Forms.Clipboard so that
    ''' a clipboard can be instanced.
    ''' </summary>
    <EditorBrowsable(EditorBrowsableState.Never), HostProtection(Resources:=HostProtectionResource.ExternalProcessMgmt)>
    Public Class ClipboardProxy
#Enable Warning SYSLIB0003 ' Type or member is obsolete

        ''' <summary>
        ''' Only Allows instantiation of the class
        ''' </summary>
        Friend Sub New()
        End Sub

        ''' <summary>
        ''' Gets text from the clipboard
        ''' </summary>
        ''' <returns>The text as a String</returns>
        Public Function GetText() As String
            Return Clipboard.GetText()
        End Function

        ''' <summary>
        ''' Gets text from the clipboard saved in the passed in format
        ''' </summary>
        ''' <param name="format">The type of text to get</param>
        ''' <returns>The text as a String</returns>
        Public Function GetText(format As System.Windows.Forms.TextDataFormat) As String
            Return Clipboard.GetText(format)
        End Function

        ''' <summary>
        ''' Indicates whether or not text is available on the clipboard
        ''' </summary>
        ''' <returns>True if text is available, otherwise False</returns>
        Public Function ContainsText() As Boolean
            Return Clipboard.ContainsText
        End Function

        ''' <summary>
        ''' Indicates whether or not text is available on the clipboard in 
        ''' the passed in format
        ''' </summary>
        ''' <param name="format">The type of text being checked for</param>
        ''' <returns>True if text is available, otherwise False</returns>
        Public Function ContainsText(format As System.Windows.Forms.TextDataFormat) As Boolean
            Return Clipboard.ContainsText(format)
        End Function

        ''' <summary>
        ''' Saves the passed in String to the clipboard
        ''' </summary>
        ''' <param name="text">The String to save</param>
        Public Sub SetText(text As String)
            Clipboard.SetText(text)
        End Sub

        ''' <summary>
        ''' Saves the passed in String to the clipboard in the passed in format
        ''' </summary>
        ''' <param name="text">The String to save</param>
        ''' <param name="format">The format in which to save the String</param>
        Public Sub SetText(text As String, format As System.Windows.Forms.TextDataFormat)
            Clipboard.SetText(text, format)
        End Sub

        ''' <summary>
        ''' Gets an Image from the clipboard
        ''' </summary>
        ''' <returns>The image</returns>
        Public Function GetImage() As Image
            Return Clipboard.GetImage()
        End Function

        ''' <summary>
        ''' Indicate whether or not an image has been saved to the clipboard
        ''' </summary>
        ''' <returns>True if an image is available, otherwise False</returns>
        Public Function ContainsImage() As Boolean
            Return Clipboard.ContainsImage()
        End Function

        ''' <summary>
        ''' Saves the passed in image to the clipboard
        ''' </summary>
        ''' <param name="image">The image to be saved</param>
        Public Sub SetImage(image As Image)
            Clipboard.SetImage(image)
        End Sub

        ''' <summary>
        ''' Gets an audio stream from the clipboard
        ''' </summary>
        ''' <returns>The audio stream as a Stream</returns>
        Public Function GetAudioStream() As Stream
            Return Clipboard.GetAudioStream()
        End Function

        ''' <summary>
        ''' Indicates whether or not there's an audio stream saved to the clipboard
        ''' </summary>
        ''' <returns>True if an audio stream is available, otherwise False</returns>
        Public Function ContainsAudio() As Boolean
            Return Clipboard.ContainsAudio()
        End Function

        ''' <summary>
        ''' Saves the passed in audio byte array to the clipboard
        ''' </summary>
        ''' <param name="audioBytes">The byte array to be saved</param>
        Public Sub SetAudio(audioBytes As Byte())
            Clipboard.SetAudio(audioBytes)
        End Sub

        ''' <summary>
        ''' Saves the passed in audio stream to the clipboard
        ''' </summary>
        ''' <param name="audioStream">The stream to be saved</param>
        Public Sub SetAudio(audioStream As Stream)
            Clipboard.SetAudio(audioStream)
        End Sub

        ''' <summary>
        ''' Gets a file drop list from the clipboard
        ''' </summary>
        ''' <returns>The list of file paths as a StringCollection</returns>
        Public Function GetFileDropList() As StringCollection
            Return Clipboard.GetFileDropList()
        End Function

        ''' <summary>
        ''' Indicates whether or not a file drop list has been saved to the clipboard
        ''' </summary>
        ''' <returns>True if a file drop list is available, otherwise False</returns>
        Public Function ContainsFileDropList() As Boolean
            Return Clipboard.ContainsFileDropList()
        End Function

        ''' <summary>
        ''' Saves the passed in file drop list to the clipboard
        ''' </summary>
        ''' <param name="filePaths">The file drop list as a StringCollection</param>
        Public Sub SetFileDropList(filePaths As StringCollection)
            Clipboard.SetFileDropList(filePaths)
        End Sub

        ''' <summary>
        ''' Gets data from the clipboard that's been saved in the passed in format.
        ''' </summary>
        ''' <param name="format">The type of data being sought</param>
        ''' <returns>The data</returns>
        Public Function GetData(format As String) As Object
            Return Clipboard.GetData(format)
        End Function

        ''' <summary>
        ''' Indicates whether or not there is data on the clipboard in the passed in format
        ''' </summary>
        ''' <param name="format"></param>
        ''' <returns>True if there's data in the passed in format, otherwise False</returns>
        Public Function ContainsData(format As String) As Boolean
            Return Clipboard.ContainsData(format)
        End Function

        ''' <summary>
        ''' Saves the passed in data to the clipboard in the passed in format
        ''' </summary>
        ''' <param name="format">The format in which to save the data</param>
        ''' <param name="data">The data to be saved</param>
        Public Sub SetData(format As String, data As Object)
            Clipboard.SetData(format, data)
        End Sub

        ''' <summary>
        ''' Removes everything from the clipboard
        ''' </summary>
        Public Sub Clear()
            Clipboard.Clear()
        End Sub

        ''' <summary>
        ''' Gets a Data Object from the clipboard.
        ''' </summary>
        ''' <returns>The data object</returns>
        ''' <remarks>This gives the ability to save an object in multiple formats</remarks>
        <EditorBrowsable(EditorBrowsableState.Advanced)>
        Public Function GetDataObject() As System.Windows.Forms.IDataObject
            Return Clipboard.GetDataObject()
        End Function

        ''' <summary>
        ''' Saves a DataObject to the clipboard
        ''' </summary>
        ''' <param name="data">The data object to be saved</param>
        ''' <remarks>This gives the ability to save an object in multiple formats</remarks>
        <EditorBrowsable(EditorBrowsableState.Advanced)>
        Public Sub SetDataObject(data As System.Windows.Forms.DataObject)
            Clipboard.SetDataObject(data)
        End Sub
    End Class
End Namespace
