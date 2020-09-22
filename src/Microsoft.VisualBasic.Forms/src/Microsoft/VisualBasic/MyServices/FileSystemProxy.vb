' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Option Strict On
Option Explicit On

Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Text
Imports Microsoft.VisualBasic.FileIO

Namespace Microsoft.VisualBasic.MyServices

    ''' <summary>
    ''' An extremely thin wrapper around Microsoft.VisualBasic.FileIO.FileSystem to expose the type through My.
    ''' </summary>
    <EditorBrowsable(EditorBrowsableState.Never)>
    Public Class FileSystemProxy

        Public ReadOnly Property Drives() As ReadOnlyCollection(Of IO.DriveInfo)
            Get
                Return FileIO.FileSystem.Drives
            End Get
        End Property

        Public ReadOnly Property SpecialDirectories() As SpecialDirectoriesProxy
            Get
                If _specialDirectoriesProxy Is Nothing Then
                    _specialDirectoriesProxy = New SpecialDirectoriesProxy
                End If
                Return _specialDirectoriesProxy
            End Get
        End Property

        Public Property CurrentDirectory() As String
            Get
                Return FileIO.FileSystem.CurrentDirectory
            End Get
            Set(value As String)
                FileIO.FileSystem.CurrentDirectory = value
            End Set
        End Property

        Public Function DirectoryExists(directory As String) As Boolean
            Return FileIO.FileSystem.DirectoryExists(directory)
        End Function

        Public Function FileExists(file As String) As Boolean
            Return FileIO.FileSystem.FileExists(file)
        End Function

        Public Sub CreateDirectory(directory As String)
            FileIO.FileSystem.CreateDirectory(directory)
        End Sub

        Public Function GetDirectoryInfo(directory As String) As IO.DirectoryInfo
            Return FileIO.FileSystem.GetDirectoryInfo(directory)
        End Function

        Public Function GetFileInfo(file As String) As IO.FileInfo
            Return FileIO.FileSystem.GetFileInfo(file)
        End Function

        Public Function GetDriveInfo(drive As String) As IO.DriveInfo
            Return FileIO.FileSystem.GetDriveInfo(drive)
        End Function

        Public Function GetFiles(directory As String) As ReadOnlyCollection(Of String)
            Return FileIO.FileSystem.GetFiles(directory)
        End Function

        Public Function GetFiles(directory As String, searchType As SearchOption,
            ParamArray wildcards() As String) As ReadOnlyCollection(Of String)

            Return FileIO.FileSystem.GetFiles(directory, searchType, wildcards)
        End Function

        Public Function GetDirectories(directory As String) As ReadOnlyCollection(Of String)
            Return FileIO.FileSystem.GetDirectories(directory)
        End Function

        Public Function GetDirectories(directory As String, searchType As SearchOption,
            ParamArray wildcards() As String) As ReadOnlyCollection(Of String)

            Return FileIO.FileSystem.GetDirectories(directory, searchType, wildcards)
        End Function

        Public Function FindInFiles(directory As String,
            containsText As String, ignoreCase As Boolean, searchType As SearchOption) As ReadOnlyCollection(Of String)

            Return FileIO.FileSystem.FindInFiles(directory, containsText, ignoreCase, searchType)
        End Function

        Public Function FindInFiles(directory As String, containsText As String, ignoreCase As Boolean,
            searchType As SearchOption, ParamArray fileWildcards() As String) As ReadOnlyCollection(Of String)

            Return FileIO.FileSystem.FindInFiles(directory, containsText, ignoreCase, searchType, fileWildcards)
        End Function

        Public Function GetParentPath(path As String) As String
            Return FileIO.FileSystem.GetParentPath(path)
        End Function

        Public Function CombinePath(baseDirectory As String, relativePath As String) As String
            Return FileIO.FileSystem.CombinePath(baseDirectory, relativePath)
        End Function

        Public Function GetName(path As String) As String
            Return FileIO.FileSystem.GetName(path)
        End Function

        Public Function GetTempFileName() As String
            Return FileIO.FileSystem.GetTempFileName()
        End Function

        Public Function ReadAllText(file As String) As String
            Return FileIO.FileSystem.ReadAllText(file)
        End Function

        Public Function ReadAllText(file As String, encoding As Encoding) As String
            Return FileIO.FileSystem.ReadAllText(file, encoding)
        End Function

        Public Function ReadAllBytes(file As String) As Byte()
            Return FileIO.FileSystem.ReadAllBytes(file)
        End Function

        Public Sub WriteAllText(file As String, text As String, append As Boolean)
            FileIO.FileSystem.WriteAllText(file, text, append)
        End Sub

        Public Sub WriteAllText(file As String, text As String, append As Boolean,
            encoding As Encoding)

            FileIO.FileSystem.WriteAllText(file, text, append, encoding)
        End Sub

        Public Sub WriteAllBytes(file As String, data() As Byte, append As Boolean)
            FileIO.FileSystem.WriteAllBytes(file, data, append)
        End Sub

        Public Sub CopyFile(sourceFileName As String, destinationFileName As String)
            FileIO.FileSystem.CopyFile(sourceFileName, destinationFileName)
        End Sub

        Public Sub CopyFile(sourceFileName As String, destinationFileName As String, overwrite As Boolean)
            FileIO.FileSystem.CopyFile(sourceFileName, destinationFileName, overwrite)
        End Sub

        Public Sub CopyFile(sourceFileName As String, destinationFileName As String, showUI As UIOption)
            FileIO.FileSystem.CopyFile(sourceFileName, destinationFileName, showUI)
        End Sub

        Public Sub CopyFile(sourceFileName As String, destinationFileName As String, showUI As UIOption, onUserCancel As UICancelOption)
            FileIO.FileSystem.CopyFile(sourceFileName, destinationFileName, showUI, onUserCancel)
        End Sub

        Public Sub MoveFile(sourceFileName As String, destinationFileName As String)
            FileIO.FileSystem.MoveFile(sourceFileName, destinationFileName)
        End Sub

        Public Sub MoveFile(sourceFileName As String, destinationFileName As String, overwrite As Boolean)
            FileIO.FileSystem.MoveFile(sourceFileName, destinationFileName, overwrite)
        End Sub

        Public Sub MoveFile(sourceFileName As String, destinationFileName As String, showUI As UIOption)
            FileIO.FileSystem.MoveFile(sourceFileName, destinationFileName, showUI)
        End Sub

        Public Sub MoveFile(sourceFileName As String, destinationFileName As String, showUI As UIOption, onUserCancel As UICancelOption)
            FileIO.FileSystem.MoveFile(sourceFileName, destinationFileName, showUI, onUserCancel)
        End Sub

        Public Sub CopyDirectory(sourceDirectoryName As String, destinationDirectoryName As String)
            FileIO.FileSystem.CopyDirectory(sourceDirectoryName, destinationDirectoryName)
        End Sub

        Public Sub CopyDirectory(sourceDirectoryName As String, destinationDirectoryName As String, overwrite As Boolean)
            FileIO.FileSystem.CopyDirectory(sourceDirectoryName, destinationDirectoryName, overwrite)
        End Sub

        Public Sub CopyDirectory(sourceDirectoryName As String, destinationDirectoryName As String, showUI As UIOption)
            FileIO.FileSystem.CopyDirectory(sourceDirectoryName, destinationDirectoryName, showUI)
        End Sub

        Public Sub CopyDirectory(sourceDirectoryName As String, destinationDirectoryName As String, showUI As UIOption, onUserCancel As UICancelOption)
            FileIO.FileSystem.CopyDirectory(sourceDirectoryName, destinationDirectoryName, showUI, onUserCancel)
        End Sub

        Public Sub MoveDirectory(sourceDirectoryName As String, destinationDirectoryName As String)
            FileIO.FileSystem.MoveDirectory(sourceDirectoryName, destinationDirectoryName)
        End Sub

        Public Sub MoveDirectory(sourceDirectoryName As String, destinationDirectoryName As String, overwrite As Boolean)
            FileIO.FileSystem.MoveDirectory(sourceDirectoryName, destinationDirectoryName, overwrite)
        End Sub

        Public Sub MoveDirectory(sourceDirectoryName As String, destinationDirectoryName As String, showUI As UIOption)
            FileIO.FileSystem.MoveDirectory(sourceDirectoryName, destinationDirectoryName, showUI)
        End Sub

        Public Sub MoveDirectory(sourceDirectoryName As String, destinationDirectoryName As String, showUI As UIOption, onUserCancel As UICancelOption)
            FileIO.FileSystem.MoveDirectory(sourceDirectoryName, destinationDirectoryName, showUI, onUserCancel)
        End Sub

        Public Sub DeleteFile(file As String)
            FileIO.FileSystem.DeleteFile(file)
        End Sub

        Public Sub DeleteFile(file As String, showUI As UIOption, recycle As RecycleOption)
            FileIO.FileSystem.DeleteFile(file, showUI, recycle)
        End Sub

        Public Sub DeleteFile(file As String, showUI As UIOption, recycle As RecycleOption,
            onUserCancel As UICancelOption)

            FileIO.FileSystem.DeleteFile(file, showUI, recycle, onUserCancel)
        End Sub

        Public Sub DeleteDirectory(directory As String, onDirectoryNotEmpty As DeleteDirectoryOption)
            FileIO.FileSystem.DeleteDirectory(directory, onDirectoryNotEmpty)
        End Sub

        Public Sub DeleteDirectory(directory As String, showUI As UIOption, recycle As RecycleOption)

            FileIO.FileSystem.DeleteDirectory(directory, showUI, recycle)
        End Sub

        Public Sub DeleteDirectory(directory As String,
            showUI As UIOption, recycle As RecycleOption, onUserCancel As UICancelOption)

            FileIO.FileSystem.DeleteDirectory(directory, showUI, recycle, onUserCancel)
        End Sub

        Public Sub RenameFile(file As String, newName As String)
            FileIO.FileSystem.RenameFile(file, newName)
        End Sub

        Public Sub RenameDirectory(directory As String, newName As String)
            FileIO.FileSystem.RenameDirectory(directory, newName)
        End Sub

        Public Function OpenTextFieldParser(file As String) As TextFieldParser
            Return FileIO.FileSystem.OpenTextFieldParser(file)
        End Function

        Public Function OpenTextFieldParser(file As String, ParamArray delimiters As String()) As TextFieldParser
            Return FileIO.FileSystem.OpenTextFieldParser(file, delimiters)
        End Function

        Public Function OpenTextFieldParser(file As String, ParamArray fieldWidths As Integer()) As TextFieldParser
            Return FileIO.FileSystem.OpenTextFieldParser(file, fieldWidths)
        End Function

        Public Function OpenTextFileReader(file As String) As IO.StreamReader
            Return FileIO.FileSystem.OpenTextFileReader(file)
        End Function

        Public Function OpenTextFileReader(file As String, encoding As Encoding) As IO.StreamReader
            Return FileIO.FileSystem.OpenTextFileReader(file, encoding)
        End Function

        Public Function OpenTextFileWriter(file As String, append As Boolean) As IO.StreamWriter
            Return FileIO.FileSystem.OpenTextFileWriter(file, append)
        End Function

        Public Function OpenTextFileWriter(file As String, append As Boolean,
            encoding As Encoding) As IO.StreamWriter

            Return FileIO.FileSystem.OpenTextFileWriter(file, append, encoding)
        End Function

        ''' <summary>
        ''' Proxy class can only created by internal classes.
        ''' </summary>
        Friend Sub New()
        End Sub

        Private _specialDirectoriesProxy As SpecialDirectoriesProxy
    End Class
End Namespace
