' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Text
Imports Microsoft.VisualBasic.FileIO

Namespace Microsoft.VisualBasic.MyServices

    ''' <summary>
    '''  This class represents the <see cref="FileIO.FileSystem"/> on a computer.
    '''  It allows browsing the existing drives, special directories;
    '''  and also contains some commonly use methods for IO tasks.
    '''  It exposes them through the <see langword="Namespace"/> <see cref="My"/>
    ''' </summary>
    <EditorBrowsable(EditorBrowsableState.Never)>
    Public Class FileSystemProxy

        Private _specialDirectoriesProxy As SpecialDirectoriesProxy

        ''' <summary>
        '''  Proxy class can only created by internal classes.
        ''' </summary>
        Friend Sub New()
        End Sub

        ''' <inheritdoc cref="FileIO.FileSystem.CurrentDirectory"/>
        Public Property CurrentDirectory() As String
            Get
                Return FileIO.FileSystem.CurrentDirectory
            End Get
            Set(value As String)
                FileIO.FileSystem.CurrentDirectory = value
            End Set
        End Property

        ''' <inheritdoc cref="FileIO.FileSystem.Drives"/>
        Public ReadOnly Property Drives() As ReadOnlyCollection(Of IO.DriveInfo)
            Get
                Return FileIO.FileSystem.Drives
            End Get
        End Property

        ''' <summary>
        '''  Returns an instance of <see cref="SpecialDirectoriesProxy"/>
        '''  specific to the current user (My Documents, My Music ...) and those specific
        '''  to the current Application that a developer expects to be able to find quickly.
        ''' </summary>
        ''' <value>a cached instance of <see cref="SpecialDirectoriesProxy"/></value>
        Public ReadOnly Property SpecialDirectories() As SpecialDirectoriesProxy
            Get
                If _specialDirectoriesProxy Is Nothing Then
                    _specialDirectoriesProxy = New SpecialDirectoriesProxy
                End If
                Return _specialDirectoriesProxy
            End Get
        End Property

        ''' <inheritdoc cref="FileIO.FileSystem.CombinePath(String, String)"/>
        Public Function CombinePath(baseDirectory As String, relativePath As String) As String
            Return FileIO.FileSystem.CombinePath(baseDirectory, relativePath)
        End Function

        ''' <inheritdoc cref="FileIO.FileSystem.CopyDirectory(String, String)"/>
        Public Sub CopyDirectory(sourceDirectoryName As String, destinationDirectoryName As String)
            FileIO.FileSystem.CopyDirectory(sourceDirectoryName, destinationDirectoryName)
        End Sub

        ''' <inheritdoc cref="FileIO.FileSystem.CopyDirectory(String, String, Boolean)"/>
        Public Sub CopyDirectory(sourceDirectoryName As String, destinationDirectoryName As String, overwrite As Boolean)
            FileIO.FileSystem.CopyDirectory(sourceDirectoryName, destinationDirectoryName, overwrite)
        End Sub

        ''' <inheritdoc cref="FileIO.FileSystem.CopyDirectory(String, String, UIOption)"/>
        Public Sub CopyDirectory(sourceDirectoryName As String, destinationDirectoryName As String, showUI As UIOption)
            FileIO.FileSystem.CopyDirectory(sourceDirectoryName, destinationDirectoryName, showUI)
        End Sub

        ''' <inheritdoc cref="FileIO.FileSystem.CopyDirectory(String, String, UIOption, UICancelOption)"/>
        Public Sub CopyDirectory(
            sourceDirectoryName As String,
            destinationDirectoryName As String,
            showUI As UIOption,
            onUserCancel As UICancelOption)

            FileIO.FileSystem.CopyDirectory(sourceDirectoryName, destinationDirectoryName, showUI, onUserCancel)
        End Sub

        ''' <inheritdoc cref="IO.File.Copy(String, String)"/>
        Public Sub CopyFile(sourceFileName As String, destinationFileName As String)
            FileIO.FileSystem.CopyFile(sourceFileName, destinationFileName)
        End Sub

        ''' <inheritdoc cref="IO.File.Copy(String, String, Boolean)"/>
        Public Sub CopyFile(sourceFileName As String, destinationFileName As String, overwrite As Boolean)
            FileIO.FileSystem.CopyFile(sourceFileName, destinationFileName, overwrite)
        End Sub

        ''' <inheritdoc cref="FileIO.FileSystem.CopyFile(String, String, UIOption)"/>
        Public Sub CopyFile(sourceFileName As String, destinationFileName As String, showUI As UIOption)
            FileIO.FileSystem.CopyFile(sourceFileName, destinationFileName, showUI)
        End Sub

        ''' <inheritdoc cref="FileIO.FileSystem.CopyFile(String, String, UIOption, UICancelOption)"/>
        Public Sub CopyFile(
            sourceFileName As String,
            destinationFileName As String,
            showUI As UIOption,
            onUserCancel As UICancelOption)

            FileIO.FileSystem.CopyFile(sourceFileName, destinationFileName, showUI, onUserCancel)
        End Sub

        ''' <inheritdoc cref="FileIO.FileSystem.CreateDirectory(String)"/>
        Public Sub CreateDirectory(directory As String)
            FileIO.FileSystem.CreateDirectory(directory)
        End Sub
        ''' <inheritdoc cref="FileIO.FileSystem.DeleteDirectory(String, DeleteDirectoryOption)"/>
        Public Sub DeleteDirectory(directory As String, onDirectoryNotEmpty As DeleteDirectoryOption)
            FileIO.FileSystem.DeleteDirectory(directory, onDirectoryNotEmpty)
        End Sub

        ''' <inheritdoc cref="FileIO.FileSystem.DeleteDirectory(String, UIOption, RecycleOption)"/>
        Public Sub DeleteDirectory(directory As String, showUI As UIOption, recycle As RecycleOption)
            FileIO.FileSystem.DeleteDirectory(directory, showUI, recycle)
        End Sub

        ''' <inheritdoc cref="FileIO.FileSystem.DeleteDirectory(String, UIOption, RecycleOption, UICancelOption)"/>
        Public Sub DeleteDirectory(
            directory As String,
            showUI As UIOption,
            recycle As RecycleOption,
            onUserCancel As UICancelOption)

            FileIO.FileSystem.DeleteDirectory(directory, showUI, recycle, onUserCancel)
        End Sub

        ''' <inheritdoc cref="FileIO.FileSystem.DeleteFile(String)"/>
        Public Sub DeleteFile(file As String)
            FileIO.FileSystem.DeleteFile(file)
        End Sub

        ''' <inheritdoc cref="FileIO.FileSystem.DeleteFile(String, UIOption, RecycleOption, UICancelOption)"/>
        Public Sub DeleteFile(
            file As String,
            showUI As UIOption,
            recycle As RecycleOption,
            onUserCancel As UICancelOption)

            FileIO.FileSystem.DeleteFile(file, showUI, recycle, onUserCancel)
        End Sub

        ''' <inheritdoc cref="FileIO.FileSystem.DeleteFile(String, UIOption, RecycleOption)"/>
        Public Sub DeleteFile(file As String, showUI As UIOption, recycle As RecycleOption)
            FileIO.FileSystem.DeleteFile(file, showUI, recycle)
        End Sub

        ''' <inheritdoc cref="FileIO.FileSystem.DirectoryExists(String)"/>
        Public Function DirectoryExists(directory As String) As Boolean
            Return FileIO.FileSystem.DirectoryExists(directory)
        End Function

        ''' <inheritdoc cref="FileIO.FileSystem.FileExists(String)"/>
        Public Function FileExists(file As String) As Boolean
            Return FileIO.FileSystem.FileExists(file)
        End Function

        ''' <inheritdoc cref="FileIO.FileSystem.FindInFiles(String, String, Boolean, SearchOption)"/>
        Public Function FindInFiles(
            directory As String,
            containsText As String,
            ignoreCase As Boolean,
            searchType As SearchOption) As ReadOnlyCollection(Of String)

            Return FileIO.FileSystem.FindInFiles(directory, containsText, ignoreCase, searchType)
        End Function

        ''' <inheritdoc cref="FileIO.FileSystem.FindInFiles(String, String, Boolean, SearchOption, String()"/>
        Public Function FindInFiles(
            directory As String,
            containsText As String,
            ignoreCase As Boolean,
            searchType As SearchOption,
            ParamArray fileWildcards() As String) As ReadOnlyCollection(Of String)

            Return FileIO.FileSystem.FindInFiles(directory, containsText, ignoreCase, searchType, fileWildcards)
        End Function

        ''' <inheritdoc cref="FileIO.FileSystem.GetDirectories(String)"/>
        Public Function GetDirectories(directory As String) As ReadOnlyCollection(Of String)
            Return FileIO.FileSystem.GetDirectories(directory)
        End Function

        ''' <inheritdoc cref="FileIO.FileSystem.GetDirectories(String, SearchOption, String())"/>
        Public Function GetDirectories(
            directory As String,
            searchType As SearchOption,
            ParamArray wildcards() As String) As ReadOnlyCollection(Of String)

            Return FileIO.FileSystem.GetDirectories(directory, searchType, wildcards)
        End Function

        ''' <inheritdoc cref="IO.DirectoryInfo.New(String)"/>
        Public Function GetDirectoryInfo(directory As String) As IO.DirectoryInfo
            Return FileIO.FileSystem.GetDirectoryInfo(directory)
        End Function

        ''' <inheritdoc cref="IO.DriveInfo.New(String)"/>
        Public Function GetDriveInfo(drive As String) As IO.DriveInfo
            Return FileIO.FileSystem.GetDriveInfo(drive)
        End Function

        ''' <inheritdoc cref="IO.Fileinfo.New(String)"/>
        Public Function GetFileInfo(file As String) As IO.FileInfo
            Return FileIO.FileSystem.GetFileInfo(file)
        End Function

        ''' <inheritdoc cref="FileIO.FileSystem.GetFiles(String)"/>
        Public Function GetFiles(directory As String) As ReadOnlyCollection(Of String)
            Return FileIO.FileSystem.GetFiles(directory)
        End Function

        ''' <inheritdoc cref="FileIO.FileSystem.GetFiles(String, SearchOption, String())"/>
        Public Function GetFiles(
            directory As String,
            searchType As SearchOption,
            ParamArray wildcards() As String) As ReadOnlyCollection(Of String)

            Return FileIO.FileSystem.GetFiles(directory, searchType, wildcards)
        End Function

        ''' <inheritdoc cref="IO.Path.GetFileName(String)"/>
        Public Function GetName(path As String) As String
            Return FileIO.FileSystem.GetName(path)
        End Function

        ''' <inheritdoc cref="IO.Path.GetDirectoryName(String)"/>
        Public Function GetParentPath(path As String) As String
            Return FileIO.FileSystem.GetParentPath(path)
        End Function

        ''' <inheritdoc cref="IO.Path.GetTempFileName()"/>
        Public Function GetTempFileName() As String
            Return FileIO.FileSystem.GetTempFileName()
        End Function

        ''' <inheritdoc cref="IO.Directory.Move(String, String)"/>
        Public Sub MoveDirectory(sourceDirectoryName As String, destinationDirectoryName As String)
            FileIO.FileSystem.MoveDirectory(sourceDirectoryName, destinationDirectoryName)
        End Sub

        ''' <inheritdoc cref="FileIO.FileSystem.MoveDirectory(String, String, Boolean)"/>
        Public Sub MoveDirectory(sourceDirectoryName As String, destinationDirectoryName As String, overwrite As Boolean)
            FileIO.FileSystem.MoveDirectory(sourceDirectoryName, destinationDirectoryName, overwrite)
        End Sub

        ''' <inheritdoc cref="FileIO.FileSystem.MoveDirectory(String, String, UIOption)"/>
        Public Sub MoveDirectory(sourceDirectoryName As String, destinationDirectoryName As String, showUI As UIOption)
            FileIO.FileSystem.MoveDirectory(sourceDirectoryName, destinationDirectoryName, showUI)
        End Sub

        ''' <inheritdoc cref="FileIO.FileSystem.MoveDirectory(String, String, UIOption, UICancelOption)"/>
        Public Sub MoveDirectory(
            sourceDirectoryName As String,
            destinationDirectoryName As String,
            showUI As UIOption,
            onUserCancel As UICancelOption)

            FileIO.FileSystem.MoveDirectory(sourceDirectoryName, destinationDirectoryName, showUI, onUserCancel)
        End Sub

        ''' <inheritdoc cref="IO.File.Move(String, String)"/>
        Public Sub MoveFile(sourceFileName As String, destinationFileName As String)
            FileIO.FileSystem.MoveFile(sourceFileName, destinationFileName)
        End Sub

        ''' <inheritdoc cref="IO.File.Move(String, String, Boolean)"/>
        Public Sub MoveFile(sourceFileName As String, destinationFileName As String, overwrite As Boolean)
            FileIO.FileSystem.MoveFile(sourceFileName, destinationFileName, overwrite)
        End Sub
        ''' <inheritdoc cref="FileIO.FileSystem.MoveFile(String, String, UIOption)"/>
        Public Sub MoveFile(sourceFileName As String, destinationFileName As String, showUI As UIOption)
            FileIO.FileSystem.MoveFile(sourceFileName, destinationFileName, showUI)
        End Sub

        ''' <inheritdoc cref="FileIO.FileSystem.MoveFile(String, String, UIOption, UICancelOption)"/>
        Public Sub MoveFile(
            sourceFileName As String,
            destinationFileName As String,
            showUI As UIOption,
            onUserCancel As UICancelOption)

            FileIO.FileSystem.MoveFile(sourceFileName, destinationFileName, showUI, onUserCancel)
        End Sub

        ''' <inheritdoc cref="FileIO.FileSystem.OpenTextFieldParser(String)"/>
        Public Function OpenTextFieldParser(file As String) As TextFieldParser
            Return FileIO.FileSystem.OpenTextFieldParser(file)
        End Function

        ''' <inheritdoc cref="FileIO.FileSystem.OpenTextFieldParser(String, String())"/>
        Public Function OpenTextFieldParser(file As String, ParamArray delimiters As String()) As TextFieldParser
            Return FileIO.FileSystem.OpenTextFieldParser(file, delimiters)
        End Function

        ''' <inheritdoc cref="FileIO.FileSystem.OpenTextFieldParser(String, Integer())"/>
        Public Function OpenTextFieldParser(file As String, ParamArray fieldWidths As Integer()) As TextFieldParser
            Return FileIO.FileSystem.OpenTextFieldParser(file, fieldWidths)
        End Function

        ''' <inheritdoc cref="IO.StreamReader.New(String)"/>
        Public Function OpenTextFileReader(file As String) As IO.StreamReader
            Return FileIO.FileSystem.OpenTextFileReader(file)
        End Function

        ''' <inheritdoc cref="IO.StreamReader.New(String, Encoding)"/>
        Public Function OpenTextFileReader(file As String, encoding As Encoding) As IO.StreamReader
            Return FileIO.FileSystem.OpenTextFileReader(file, encoding)
        End Function

        ''' <inheritdoc cref="IO.StreamWriter.New(String, Boolean)"/>
        Public Function OpenTextFileWriter(file As String, append As Boolean) As IO.StreamWriter
            Return FileIO.FileSystem.OpenTextFileWriter(file, append)
        End Function

        ''' <inheritdoc cref="IO.StreamWriter.New(String, Boolean, Encoding)"/>
        Public Function OpenTextFileWriter(
            file As String,
            append As Boolean,
            encoding As Encoding) As IO.StreamWriter

            Return FileIO.FileSystem.OpenTextFileWriter(file, append, encoding)
        End Function

        ''' <inheritdoc cref="IO.File.ReadAllBytes(String)"/>
        Public Function ReadAllBytes(file As String) As Byte()
            Return FileIO.FileSystem.ReadAllBytes(file)
        End Function

        ''' <inheritdoc cref="IO.File.ReadAllText(String)"/>
        Public Function ReadAllText(file As String) As String
            Return FileIO.FileSystem.ReadAllText(file)
        End Function

        ''' <inheritdoc cref="IO.File.ReadAllText(String, Encoding)"/>
        Public Function ReadAllText(file As String, encoding As Encoding) As String
            Return FileIO.FileSystem.ReadAllText(file, encoding)
        End Function

        ''' <inheritdoc cref="FileIO.FileSystem.RenameDirectory(String, String)"/>
        Public Sub RenameDirectory(directory As String, newName As String)
            FileIO.FileSystem.RenameDirectory(directory, newName)
        End Sub

        ''' <inheritdoc cref="FileIO.FileSystem.RenameFile(String, String)"/>
        Public Sub RenameFile(file As String, newName As String)
            FileIO.FileSystem.RenameFile(file, newName)
        End Sub

        ''' <inheritdoc cref="FileIO.FileSystem.WriteAllBytes(String, Byte(), Boolean)"/>
        Public Sub WriteAllBytes(file As String, data() As Byte, append As Boolean)
            FileIO.FileSystem.WriteAllBytes(file, data, append)
        End Sub

        ''' <inheritdoc cref="FileIO.FileSystem.WriteAllText(String, String, Boolean)"/>
        Public Sub WriteAllText(file As String, text As String, append As Boolean)
            FileIO.FileSystem.WriteAllText(file, text, append)
        End Sub

        ''' <inheritdoc cref="FileIO.FileSystem.WriteAllText(String, String, Boolean, Encoding)"/>
        Public Sub WriteAllText(
            file As String,
            text As String,
            append As Boolean,
            encoding As Encoding)

            FileIO.FileSystem.WriteAllText(file, text, append, encoding)
        End Sub

    End Class
End Namespace
