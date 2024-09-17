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
    '''  It exposes them through the type <see cref="My"/>
    ''' </summary>
    <EditorBrowsable(EditorBrowsableState.Never)>
    Public Class FileSystemProxy

        Private _specialDirectoriesProxy As SpecialDirectoriesProxy

        ''' <summary>
        '''  Proxy class can only created by internal classes.
        ''' </summary>
        Friend Sub New()
        End Sub

        ''' <summary>
        '''  Get or set the current working directory.
        ''' </summary>
        ''' <value>A String containing the path to the directory.</value>
        Public Property CurrentDirectory() As String
            Get
                Return FileIO.FileSystem.CurrentDirectory
            End Get
            Set(value As String)
                FileIO.FileSystem.CurrentDirectory = value
            End Set
        End Property

        ''' <summary>
        '''  Returns the names of all available drives on the computer.
        ''' </summary>
        ''' <value>A ReadOnlyCollection(Of DriveInfo) containing all the current drives' names.</value>
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
        ''' <value>a cached instance of SpecialDirectoriesProxy</value>
        Public ReadOnly Property SpecialDirectories() As SpecialDirectoriesProxy
            Get
                If _specialDirectoriesProxy Is Nothing Then
                    _specialDirectoriesProxy = New SpecialDirectoriesProxy
                End If
                Return _specialDirectoriesProxy
            End Get
        End Property

        ''' <summary>
        '''  Combines two path strings by adding a path separator.
        ''' </summary>
        ''' <param name="baseDirectory">The first part of the path.</param>
        ''' <param name="relativePath">The second part of the path, must be a relative path.</param>
        ''' <returns>A String contains the combined path.</returns>
        Public Function CombinePath(baseDirectory As String, relativePath As String) As String
            Return FileIO.FileSystem.CombinePath(baseDirectory, relativePath)
        End Function

        ''' <summary>
        '''  Copy an existing directory to a new directory,
        '''  throwing exception if there are existing files with the same name.
        ''' </summary>
        ''' <param name="sourceDirectoryName">The path to the source directory, can be relative or absolute.</param>
        ''' <param name="destinationDirectoryName">The path to the target directory, can be relative or absolute.
        ''' Parent directory will always be created.</param>
        Public Sub CopyDirectory(sourceDirectoryName As String, destinationDirectoryName As String)
            FileIO.FileSystem.CopyDirectory(sourceDirectoryName, destinationDirectoryName)
        End Sub

        ''' <summary>
        '''  Copy an existing directory to a new directory,
        '''  overwriting existing files with the same name if specified.
        ''' </summary>
        ''' <param name="sourceDirectoryName">The path to the source directory, can be relative or absolute.</param>
        ''' <param name="destinationDirectoryName">The path to the target directory, can be relative or absolute.
        ''' Parent directory will always be created.</param>
        ''' <param name="overwrite">
        '''  <see langword="True"/> to overwrite existing files with the same name.
        '''  Otherwise <see langword="False"/>.
        ''' </param>
        Public Sub CopyDirectory(sourceDirectoryName As String, destinationDirectoryName As String, overwrite As Boolean)
            FileIO.FileSystem.CopyDirectory(sourceDirectoryName, destinationDirectoryName, overwrite)
        End Sub

        ''' <summary>
        '''  Copy an existing directory to a new directory,
        '''  displaying progress dialog and confirmation dialogs if specified,
        '''  throwing exception if user cancels the operation (only applies if displaying progress dialog and
        '''  confirmation dialogs).
        ''' </summary>
        ''' <param name="sourceDirectoryName">The path to the source directory, can be relative or absolute.</param>
        ''' <param name="destinationDirectoryName">The path to the target directory, can be relative or absolute.
        '''  Parent directory will always be created.</param>
        ''' <param name="showUI">
        '''  ShowDialogs to display progress and confirmation dialogs. Otherwise HideDialogs.
        ''' </param>
        Public Sub CopyDirectory(sourceDirectoryName As String, destinationDirectoryName As String, showUI As UIOption)
            FileIO.FileSystem.CopyDirectory(sourceDirectoryName, destinationDirectoryName, showUI)
        End Sub

        ''' <summary>
        '''  Copy an existing directory to a new directory,
        '''  displaying progress dialog and confirmation dialogs if specified,
        '''  throwing exception if user cancels the operation if specified.
        '''  (only applies if displaying progress dialog and confirmation dialogs).
        ''' </summary>
        ''' <param name="sourceDirectoryName">The path to the source directory, can be relative or absolute.</param>
        ''' <param name="destinationDirectoryName">The path to the target directory, can be relative or absolute.
        ''' Parent directory will always be created.</param>
        ''' <param name="showUI">
        '''  ShowDialogs to display progress and confirmation dialogs. Otherwise HideDialogs.
        ''' </param>
        ''' <param name="onUserCancel">
        '''  ThrowException to throw exception if user cancels the operation. Otherwise DoNothing.
        ''' </param>
        Public Sub CopyDirectory(
            sourceDirectoryName As String,
            destinationDirectoryName As String,
            showUI As UIOption,
            onUserCancel As UICancelOption)

            FileIO.FileSystem.CopyDirectory(sourceDirectoryName, destinationDirectoryName, showUI, onUserCancel)
        End Sub

        ''' <summary>
        '''  Copy an existing file to a new file. Overwriting a file of the same name is not allowed.
        ''' </summary>
        ''' <param name="sourceFileName">The path to the source file, can be relative or absolute.</param>
        ''' <param name="destinationFileName">The path to the destination file, can be relative or absolute.
        '''  Parent directory will always be created.
        ''' </param>
        Public Sub CopyFile(sourceFileName As String, destinationFileName As String)
            FileIO.FileSystem.CopyFile(sourceFileName, destinationFileName)
        End Sub

        ''' <summary>
        '''  Copy an existing file to a new file. Overwriting a file of the same name if specified.
        ''' </summary>
        ''' <param name="sourceFileName">The path to the source file, can be relative or absolute.</param>
        ''' <param name="destinationFileName">The path to the destination file, can be relative or absolute.
        '''  Parent directory will always be created.
        ''' </param>
        ''' <param name="overwrite">
        '''   <see langword="True"/> to overwrite existing file with the same name.
        '''   Otherwise <see langword="False"/>.
        '''  </param>
        Public Sub CopyFile(sourceFileName As String, destinationFileName As String, overwrite As Boolean)
            FileIO.FileSystem.CopyFile(sourceFileName, destinationFileName, overwrite)
        End Sub

        ''' <summary>
        '''  Copy an existing file to a new file,
        '''  displaying progress dialog and confirmation dialogs if specified,
        '''  will throw exception if user cancels the operation.
        ''' </summary>
        ''' <param name="sourceFileName">The path to the source file, can be relative or absolute.</param>
        ''' <param name="destinationFileName">
        '''  The path to the destination file, can be relative or absolute. Parent directory will always be created.
        ''' </param>
        ''' <param name="showUI">
        '''  ShowDialogs to display progress and confirmation dialogs. Otherwise HideDialogs.
        ''' </param>
        Public Sub CopyFile(sourceFileName As String, destinationFileName As String, showUI As UIOption)
            FileIO.FileSystem.CopyFile(sourceFileName, destinationFileName, showUI)
        End Sub

        ''' <summary>
        '''  Copy an existing file to a new file,
        '''  displaying progress dialog and confirmation dialogs if specified,
        '''  will throw exception if user cancels the operation if specified.
        ''' </summary>
        ''' <param name="sourceFileName">The path to the source file, can be relative or absolute.</param>
        ''' <param name="destinationFileName">
        '''  The path to the destination file, can be relative or absolute. Parent directory will always be created.
        ''' </param>
        ''' <param name="showUI">
        '''  ShowDialogs to display progress and confirmation dialogs. Otherwise HideDialogs.
        ''' </param>
        ''' <param name="onUserCancel">
        '''  ThrowException to throw exception if user cancels the operation. Otherwise DoNothing.
        ''' </param>
        ''' <remarks>onUserCancel will be ignored if showUI = HideDialogs.</remarks>
        Public Sub CopyFile(
            sourceFileName As String,
            destinationFileName As String,
            showUI As UIOption,
            onUserCancel As UICancelOption)

            FileIO.FileSystem.CopyFile(sourceFileName, destinationFileName, showUI, onUserCancel)
        End Sub

        ''' <summary>
        '''  Creates a directory from the given path (including all parent directories).
        ''' </summary>
        ''' <param name="directory">The path to create the directory at.</param>
        Public Sub CreateDirectory(directory As String)
            FileIO.FileSystem.CreateDirectory(directory)
        End Sub

        ''' <summary>
        '''  Delete the given directory, with options to recursively delete.
        ''' </summary>
        ''' <param name="directory">The path to the directory.</param>
        ''' <param name="onDirectoryNotEmpty">
        '''  DeleteAllContents to delete everything.
        '''  ThrowIfDirectoryNonEmpty to throw exception if the directory is not empty.
        ''' </param>
        Public Sub DeleteDirectory(directory As String, onDirectoryNotEmpty As DeleteDirectoryOption)
            FileIO.FileSystem.DeleteDirectory(directory, onDirectoryNotEmpty)
        End Sub

        ''' <summary>
        '''  Delete the given directory, with options to recursively delete, show progress UI,
        '''  send file to Recycle Bin; throwing exception if user cancels.
        ''' </summary>
        ''' <param name="directory">The path to the directory.</param>
        ''' <param name="showUI">
        '''  <see langword="True"/> to shows progress window.
        '''  Otherwise, <see langword="False"/>.
        ''' </param>
        ''' <param name="recycle">SendToRecycleBin to delete to Recycle Bin. Otherwise DeletePermanently.</param>
        Public Sub DeleteDirectory(directory As String, showUI As UIOption, recycle As RecycleOption)
            FileIO.FileSystem.DeleteDirectory(directory, showUI, recycle)
        End Sub

        ''' <summary>
        '''  Delete the given directory, with options to recursively delete, show progress UI,
        '''  send file to Recycle Bin, and whether to throw exception if user cancels.
        ''' </summary>
        ''' <param name="directory">The path to the directory.</param>
        ''' <param name="showUI">
        '''  ShowDialogs to display progress and confirmation dialogs. Otherwise HideDialogs.
        ''' </param>
        ''' <param name="recycle">SendToRecycleBin to delete to Recycle Bin. Otherwise DeletePermanently.</param>
        ''' <param name="onUserCancel">Throw exception when user cancel the UI operation or not.</param>
        Public Sub DeleteDirectory(
            directory As String,
            showUI As UIOption,
            recycle As RecycleOption,
            onUserCancel As UICancelOption)

            FileIO.FileSystem.DeleteDirectory(directory, showUI, recycle, onUserCancel)
        End Sub

        ''' <summary>
        '''  Delete the given file.
        ''' </summary>
        ''' <param name="file">The path to the file.</param>
        Public Sub DeleteFile(file As String)
            FileIO.FileSystem.DeleteFile(file)
        End Sub

        ''' <summary>
        '''   Delete the given file, with options to show progress UI, delete to recycle bin,
        '''   and whether to throw exception if user cancels.
        ''' </summary>
        ''' <param name="file">The path to the file.</param>
        ''' <param name="showUI">ShowDialogs to display progress and confirmation dialogs. Otherwise HideDialogs.</param>
        ''' <param name="recycle">SendToRecycleBin to delete to Recycle Bin. Otherwise DeletePermanently.</param>
        ''' <param name="onUserCancel">Throw exception when user cancel the UI operation or not.</param>
        ''' <exception cref="IO.Path.GetFullPath">IO.Path.GetFullPath() exceptions: if FilePath is invalid.</exception>
        ''' <exception cref="IO.FileNotFoundException">if a file does not exist at FilePath</exception>
        Public Sub DeleteFile(
            file As String,
            showUI As UIOption,
            recycle As RecycleOption,
            onUserCancel As UICancelOption)

            FileIO.FileSystem.DeleteFile(file, showUI, recycle, onUserCancel)
        End Sub

        ''' <summary>
        '''  Delete the given file, with options to show progress UI, delete to recycle bin.
        ''' </summary>
        ''' <param name="file">The path to the file.</param>
        ''' <param name="showUI">ShowDialogs to display progress and confirmation dialogs. Otherwise HideDialogs.</param>
        ''' <param name="recycle">SendToRecycleBin to delete to Recycle Bin. Otherwise DeletePermanently.</param>
        Public Sub DeleteFile(file As String, showUI As UIOption, recycle As RecycleOption)
            FileIO.FileSystem.DeleteFile(file, showUI, recycle)
        End Sub

        ''' <summary>
        '''  Determines whether the given path refers to an existing directory on disk.
        ''' </summary>
        ''' <param name="directory">The path to verify.</param>
        ''' <returns>
        '''  <see langword="True"/> if DirectoryPath refers to an existing directory.
        '''  Otherwise, <see langword="False"/>.
        ''' </returns>
        Public Function DirectoryExists(directory As String) As Boolean
            Return FileIO.FileSystem.DirectoryExists(directory)
        End Function

        ''' <summary>
        '''  Determines whether the given path refers to an existing file on disk.
        ''' </summary>
        ''' <param name="file">The path to verify.</param>
        ''' <returns>
        '''  <see langword="True"/> if FilePath refers to an existing file on disk.
        '''  Otherwise, <see langword="False"/>.
        ''' </returns>
        Public Function FileExists(file As String) As Boolean
            Return FileIO.FileSystem.FileExists(file)
        End Function

        ''' <summary>
        '''  Find files in the given folder that contain the given text.
        ''' </summary>
        ''' <param name="directory">The folder path to start from.</param>
        ''' <param name="containsText">The text to be found in file.</param>
        ''' <param name="ignoreCase">
        '''   <see langword="True"/> to ignore case.
        '''   Otherwise, <see langword="False"/>.
        '''  </param>
        ''' <param name="searchType">SearchAllSubDirectories to find recursively. Otherwise, SearchTopLevelOnly.</param>
        ''' <returns>A string array containing the files that match the search condition.</returns>
        Public Function FindInFiles(
            directory As String,
            containsText As String,
            ignoreCase As Boolean,
            searchType As SearchOption) As ReadOnlyCollection(Of String)

            Return FileIO.FileSystem.FindInFiles(directory, containsText, ignoreCase, searchType)
        End Function

        ''' <summary>
        '''  Find files in the given folder that contain the given text.
        ''' </summary>
        ''' <param name="directory">The folder path to start from.</param>
        ''' <param name="containsText">The text to be found in file.</param>
        ''' <param name="ignoreCase">
        '''  <see langword="True"/> to ignore case.
        '''  Otherwise, <see langword="False"/>.
        ''' </param>
        ''' <param name="searchType">SearchAllSubDirectories to find recursively. Otherwise, SearchTopLevelOnly.</param>
        ''' <param name="fileWildcards">The search patterns to use for the file name ("*.*")</param>
        ''' <returns>A string array containing the files that match the search condition.</returns>
        ''' <exception cref="System.ArgumentNullException">
        '''  If one of the pattern is Null, Empty or all-spaces string.
        ''' </exception>
        Public Function FindInFiles(
            directory As String,
            containsText As String,
            ignoreCase As Boolean,
            searchType As SearchOption,
            ParamArray fileWildcards() As String) As ReadOnlyCollection(Of String)

            Return FileIO.FileSystem.FindInFiles(directory, containsText, ignoreCase, searchType, fileWildcards)
        End Function

        ''' <summary>
        '''  Return the paths of sub directories found directly under a directory.
        ''' </summary>
        ''' <param name="directory">The directory to find the sub directories inside.</param>
        ''' <returns>A ReadOnlyCollection(Of String) containing the matched directories' paths.</returns>
        Public Function GetDirectories(directory As String) As ReadOnlyCollection(Of String)
            Return FileIO.FileSystem.GetDirectories(directory)
        End Function

        ''' <summary>
        '''  Return the paths of sub directories found under a directory with the specified name patterns.
        ''' </summary>
        ''' <param name="directory">The directory to find the sub directories inside.</param>
        ''' <param name="searchType">SearchAllSubDirectories to find recursively. Otherwise, SearchTopLevelOnly.</param>
        ''' <param name="wildcards">The wildcards for the file name, for example "*.bmp", "*.txt"</param>
        ''' <returns>A ReadOnlyCollection(Of String) containing the matched directories' paths.</returns>
        Public Function GetDirectories(
            directory As String,
            searchType As SearchOption,
            ParamArray wildcards() As String) As ReadOnlyCollection(Of String)

            Return FileIO.FileSystem.GetDirectories(directory, searchType, wildcards)
        End Function

        ''' <summary>
        '''  Returns the information object about the specified directory.
        ''' </summary>
        ''' <param name="directory">The path to the directory.</param>
        ''' <returns>A DirectoryInfo object containing the information about the specified directory.</returns>
        Public Function GetDirectoryInfo(directory As String) As IO.DirectoryInfo
            Return FileIO.FileSystem.GetDirectoryInfo(directory)
        End Function

        ''' <summary>
        '''  Return the information about the specified drive.
        ''' </summary>
        ''' <param name="drive">The path to the drive.</param>
        ''' <returns>A DriveInfo object containing the information about the specified drive.</returns>
        Public Function GetDriveInfo(drive As String) As IO.DriveInfo
            Return FileIO.FileSystem.GetDriveInfo(drive)
        End Function

        ''' <summary>
        '''  Returns the information about the specified file.
        ''' </summary>
        ''' <param name="file">The path to the file.</param>
        ''' <returns>A FileInfo object containing the information about the specified file.</returns>
        Public Function GetFileInfo(file As String) As IO.FileInfo
            Return FileIO.FileSystem.GetFileInfo(file)
        End Function

        ''' <summary>
        '''  Return an unordered collection of file paths found directly under a directory.
        ''' </summary>
        ''' <param name="directory">The directory to find the files inside.</param>
        ''' <returns>A ReadOnlyCollection(Of String) containing the matched files' paths.</returns>
        Public Function GetFiles(directory As String) As ReadOnlyCollection(Of String)
            Return FileIO.FileSystem.GetFiles(directory)
        End Function

        ''' <summary>
        '''  Return an unordered collection of file paths found under a directory
        ''' with the specified name patterns and containing the specified text.
        ''' </summary>
        ''' <param name="directory">The directory to find the files inside.</param>
        ''' <param name="searchType">SearchAllSubDirectories to find recursively. Otherwise, SearchTopLevelOnly.</param>
        ''' <param name="wildcards">The wildcards for the file name, for example "*.bmp", "*.txt"</param>
        ''' <returns>A ReadOnlyCollection(Of String) containing the matched files' paths.</returns>
        Public Function GetFiles(
            directory As String,
            searchType As SearchOption,
            ParamArray wildcards() As String) As ReadOnlyCollection(Of String)

            Return FileIO.FileSystem.GetFiles(directory, searchType, wildcards)
        End Function

        ''' <summary>
        '''  Return the name (and extension) from the given path string.
        ''' </summary>
        ''' <param name="path">The path string from which to obtain the file name (and extension).</param>
        ''' <returns>A String containing the name of the file or directory.</returns>
        ''' <exception cref="ArgumentException">path contains one or more of the invalid characters
        ''' defined in InvalidPathChars.</exception>
        Public Function GetName(path As String) As String
            Return FileIO.FileSystem.GetName(path)
        End Function

        ''' <summary>
        '''  Returns the parent directory's path from a specified path.
        ''' </summary>
        ''' <param name="path">The path to a file or directory, this can be absolute or relative.</param>
        ''' <returns>
        '''  The path to the parent directory of that file or directory
        '''  (whether absolute or relative depends on the input), or an empty string if Path is a root directory.
        ''' </returns>
        ''' <exception cref="IO.Path.GetFullPath">See IO.Path.GetFullPath: If path is an invalid path.</exception>
        ''' <remarks>
        '''  The path will be normalized (for example: C:\Dir1////\\\Dir2 will become C:\Dir1\Dir2)
        '''  but will not be resolved (for example: C:\Dir1\Dir2\..\Dir3 WILL NOT become C:\Dir1\Dir3). Use CombinePath.
        ''' </remarks>
        Public Function GetParentPath(path As String) As String
            Return FileIO.FileSystem.GetParentPath(path)
        End Function

        ''' <summary>
        '''  Create a uniquely named zero-byte temporary file on disk and return the full path to that file.
        ''' </summary>
        ''' <returns>A String containing the name of the temporary file.</returns>
        Public Function GetTempFileName() As String
            Return FileIO.FileSystem.GetTempFileName()
        End Function

        ''' <summary>
        '''  Move an existing directory to a new directory,
        '''  throwing exception if there are existing files with the same name.
        ''' </summary>
        ''' <param name="sourceDirectoryName">The path to the source directory, can be relative or absolute.</param>
        ''' <param name="destinationDirectoryName">The path to the target directory, can be relative or absolute. Parent directory will always be created.</param>
        Public Sub MoveDirectory(sourceDirectoryName As String, destinationDirectoryName As String)
            FileIO.FileSystem.MoveDirectory(sourceDirectoryName, destinationDirectoryName)
        End Sub

        ''' <summary>
        '''  Move an existing directory to a new directory, overwriting existing files with the same name if specified.
        ''' </summary>
        ''' <param name="sourceDirectoryName">The path to the source directory, can be relative or absolute.</param>
        ''' <param name="destinationDirectoryName">
        '''  The path to the target directory, can be relative or absolute.
        '''  Parent directory will always be created.
        ''' </param>
        ''' <param name="overwrite">
        '''  <see langword="True"/> to overwrite existing files with the same name.
        '''  Otherwise <see langword="False"/>.
        ''' </param>
        Public Sub MoveDirectory(sourceDirectoryName As String, destinationDirectoryName As String, overwrite As Boolean)
            FileIO.FileSystem.MoveDirectory(sourceDirectoryName, destinationDirectoryName, overwrite)
        End Sub

        ''' <summary>
        '''  Move an existing directory to a new directory,
        '''  displaying progress dialog and confirmation dialogs if specified,
        '''  throwing exception if user cancels the operation
        '''  (only applies if displaying progress dialog and confirmation dialogs).
        ''' </summary>
        ''' <param name="sourceDirectoryName">The path to the source directory, can be relative or absolute.</param>
        ''' <param name="destinationDirectoryName">The path to the target directory, can be relative or absolute. Parent directory will always be created.</param>
        ''' <param name="showUI">ShowDialogs to display progress and confirmation dialogs. Otherwise HideDialogs.</param>
        Public Sub MoveDirectory(sourceDirectoryName As String, destinationDirectoryName As String, showUI As UIOption)
            FileIO.FileSystem.MoveDirectory(sourceDirectoryName, destinationDirectoryName, showUI)
        End Sub

        ''' <summary>
        '''  Move an existing directory to a new directory,
        '''  displaying progress dialog and confirmation dialogs if specified,
        '''  throwing exception if user cancels the operation if specified.
        '''  (only applies if displaying progress dialog and confirmation dialogs).
        ''' </summary>
        ''' <param name="sourceDirectoryName">The path to the source directory, can be relative or absolute.</param>
        ''' <param name="destinationDirectoryName">The path to the target directory, can be relative or absolute. Parent directory will always be created.</param>
        ''' <param name="showUI">ShowDialogs to display progress and confirmation dialogs. Otherwise HideDialogs.</param>
        ''' <param name="onUserCancel">ThrowException to throw exception if user cancels the operation. Otherwise DoNothing.</param>
        Public Sub MoveDirectory(
            sourceDirectoryName As String,
            destinationDirectoryName As String,
            showUI As UIOption,
            onUserCancel As UICancelOption)

            FileIO.FileSystem.MoveDirectory(sourceDirectoryName, destinationDirectoryName, showUI, onUserCancel)
        End Sub

        ''' <summary>
        '''  Move an existing file to a new file. Overwriting a file of the same name is not allowed.
        ''' </summary>
        ''' <param name="sourceFileName">The path to the source file, can be relative or absolute.</param>
        ''' <param name="destinationFileName">
        '''  The path to the destination file, can be relative or absolute.
        '''  Parent directory will always be created.
        ''' </param>
        Public Sub MoveFile(sourceFileName As String, destinationFileName As String)
            FileIO.FileSystem.MoveFile(sourceFileName, destinationFileName)
        End Sub

        ''' <summary>
        '''  Move an existing file to a new file. Overwriting a file of the same name if specified.
        ''' </summary>
        ''' <param name="sourceFileName">The path to the source file, can be relative or absolute.</param>
        ''' <param name="destinationFileName">
        '''  The path to the destination file, can be relative or absolute.
        '''  Parent directory will always be created.
        ''' </param>
        ''' <param name="overwrite">
        '''  <see langword="True"/>
        '''   to overwrite existing file with the same name.
        '''   Otherwise <see langword="False"/>.
        ''' </param>
        Public Sub MoveFile(sourceFileName As String, destinationFileName As String, overwrite As Boolean)
            FileIO.FileSystem.MoveFile(sourceFileName, destinationFileName, overwrite)
        End Sub

        ''' <summary>
        '''  Move an existing file to a new file,
        '''  displaying progress dialog and confirmation dialogs if specified,
        '''  will throw exception if user cancels the operation.
        ''' </summary>
        ''' <param name="sourceFileName">The path to the source file, can be relative or absolute.</param>
        ''' <param name="destinationFileName">
        '''  The path to the destination file, can be relative or absolute.
        '''  Parent directory will always be created.
        ''' </param>
        ''' <param name="showUI">ShowDialogs to display progress and confirmation dialogs. Otherwise HideDialogs.</param>
        Public Sub MoveFile(sourceFileName As String, destinationFileName As String, showUI As UIOption)
            FileIO.FileSystem.MoveFile(sourceFileName, destinationFileName, showUI)
        End Sub

        ''' <summary>
        '''  Move an existing file to a new file,
        '''  displaying progress dialog and confirmation dialogs if specified,
        '''  will throw exception if user cancels the operation if specified.
        ''' </summary>
        ''' <param name="sourceFileName">The path to the source file, can be relative or absolute.</param>
        ''' <param name="destinationFileName">
        '''  The path to the destination file, can be relative or absolute.
        '''  Parent directory will always be created.
        ''' </param>
        ''' <param name="showUI">ShowDialogs to display progress and confirmation dialogs. Otherwise HideDialogs.</param>
        ''' <param name="onUserCancel">
        '''  ThrowException to throw exception if user cancels the operation. Otherwise DoNothing.
        ''' </param>
        ''' <remarks>onUserCancel will be ignored if showUI = HideDialogs.</remarks>
        Public Sub MoveFile(
            sourceFileName As String,
            destinationFileName As String,
            showUI As UIOption,
            onUserCancel As UICancelOption)

            FileIO.FileSystem.MoveFile(sourceFileName, destinationFileName, showUI, onUserCancel)
        End Sub

        ''' <summary>
        '''  Return an instance of a TextFieldParser for the given file.
        ''' </summary>
        ''' <param name="file">The path to the file to parse.</param>
        ''' <returns>An instance of a TextFieldParser.</returns>
        Public Function OpenTextFieldParser(file As String) As TextFieldParser
            Return FileIO.FileSystem.OpenTextFieldParser(file)
        End Function

        ''' <summary>
        '''  Return an instance of a TextFieldParser for the given file using the given delimiters.
        ''' </summary>
        ''' <param name="file">The path to the file to parse.</param>
        ''' <param name="delimiters">A list of delimiters.</param>
        ''' <returns>An instance of a TextFieldParser</returns>
        Public Function OpenTextFieldParser(file As String, ParamArray delimiters As String()) As TextFieldParser
            Return FileIO.FileSystem.OpenTextFieldParser(file, delimiters)
        End Function

        ''' <summary>
        '''  Return an instance of a TextFieldParser for the given file using the given field widths.
        ''' </summary>
        ''' <param name="file">The path to the file to parse.</param>
        ''' <param name="fieldWidths">A list of field widths.</param>
        ''' <returns>An instance of a TextFieldParser</returns>
        Public Function OpenTextFieldParser(file As String, ParamArray fieldWidths As Integer()) As TextFieldParser
            Return FileIO.FileSystem.OpenTextFieldParser(file, fieldWidths)
        End Function

        ''' <summary>
        '''  Return a StreamReader for reading the given file using UTF-8 as preferred encoding.
        ''' </summary>
        ''' <param name="file">The file to open the StreamReader on.</param>
        ''' <returns>An instance of System.IO.StreamReader opened on the file (with FileShare.Read).</returns>
        Public Function OpenTextFileReader(file As String) As IO.StreamReader
            Return FileIO.FileSystem.OpenTextFileReader(file)
        End Function

        ''' <summary>
        '''  Return a StreamReader for reading the given file using the given encoding as preferred encoding.
        ''' </summary>
        ''' <param name="file">The file to open the StreamReader on.</param>
        ''' <param name="Encoding">
        '''  The preferred encoding that will be used if the encoding of the file could not be detected.
        ''' </param>
        ''' <returns>An instance of System.IO.StreamReader opened on the file (with FileShare.Read).</returns>
        Public Function OpenTextFileReader(file As String, encoding As Encoding) As IO.StreamReader
            Return FileIO.FileSystem.OpenTextFileReader(file, encoding)
        End Function

        ''' <summary>
        '''  Return a StreamWriter for writing to the given file using UTF-8 encoding.
        ''' </summary>
        ''' <param name="file">The file to write to.</param>
        ''' <param name="Append">
        '''  <see langword="True"/> to append to the content of the file.
        '''  <see langword="False"/> to overwrite the content of the file.
        ''' </param>
        ''' <returns>An instance of StreamWriter opened on the file (with FileShare.Read).</returns>
        Public Function OpenTextFileWriter(file As String, append As Boolean) As IO.StreamWriter
            Return FileIO.FileSystem.OpenTextFileWriter(file, append)
        End Function

        ''' <summary>
        '''  Return a StreamWriter for writing to the given file using the given encoding.
        ''' </summary>
        ''' <param name="file">The file to write to.</param>
        ''' <param name="Append">
        '''  <see langword="True"/> to append to the content of the file.
        '''  <see langword="False"/> to overwrite the content of the file.
        ''' </param>
        ''' <param name="Encoding">The encoding to use to write to the file.</param>
        ''' <returns>An instance of StreamWriter opened on the file (with FileShare.Read).</returns>
        Public Function OpenTextFileWriter(
            file As String,
            append As Boolean,
            encoding As Encoding) As IO.StreamWriter

            Return FileIO.FileSystem.OpenTextFileWriter(file, append, encoding)
        End Function

        ''' <summary>
        '''  Read the whole content of a file into a byte array.
        ''' </summary>
        ''' <param name="file">The path to the file.</param>
        ''' <returns>A byte array contains the content of the file.</returns>
        ''' <exception cref="IO.IOException">
        '''  If the length of the file is larger than Integer.MaxValue (~2GB).
        ''' </exception>
        ''' <exception cref="IO.FileStream">See FileStream constructor and Read: for other exceptions.</exception>
        Public Function ReadAllBytes(file As String) As Byte()
            Return FileIO.FileSystem.ReadAllBytes(file)
        End Function

        ''' <summary>
        '''  Read the whole content of a text file into a string using UTF-8 encoding.
        ''' </summary>
        ''' <param name="file">The path to the text file.</param>
        ''' <returns>A String contains the content of the given file.</returns>
        ''' <exception cref="IO.StreamReader">See StreamReader constructor and ReadToEnd.</exception>
        Public Function ReadAllText(file As String) As String
            Return FileIO.FileSystem.ReadAllText(file)
        End Function

        ''' <summary>
        '''  Read the whole content of a text file into a string using the given encoding.
        ''' </summary>
        ''' <param name="file">The path to the text file.</param>
        ''' <param name="encoding">The character encoding to use if the encoding was not detected.</param>
        ''' <returns>A String contains the content of the given file.</returns>
        ''' <exception cref="IO.StreamReader">See StreamReader constructor and ReadToEnd.</exception>
        Public Function ReadAllText(file As String, encoding As Encoding) As String
            Return FileIO.FileSystem.ReadAllText(file, encoding)
        End Function

        ''' <summary>
        '''  Rename a directory, does not act like a move.
        ''' </summary>
        ''' <param name="directory">The path of the directory to be renamed.</param>
        ''' <param name="newName">The new name to change to. This must not contain path information.</param>
        ''' <exception cref="IO.Path.GetFullPath">IO.Path.GetFullPath exceptions: If directory is invalid.</exception>
        ''' <exception cref="System.ArgumentException">If newName is Nothing or Empty String or contains path information.</exception>
        ''' <exception cref="IO.FileNotFoundException">If directory does not point to an existing directory.</exception>
        ''' <exception cref="IO.IOException">If directory points to a root directory.
        '''     Or if there's an existing directory or an existing file with the same name.</exception>
        Public Sub RenameDirectory(directory As String, newName As String)
            FileIO.FileSystem.RenameDirectory(directory, newName)
        End Sub

        ''' <summary>
        '''  Renames a file, does not change the file location.
        ''' </summary>
        ''' <param name="file">The path to the file.</param>
        ''' <param name="newName">The new name to change to. This must not contain path information.</param>
        ''' <exception cref="IO.Path.GetFullPath">IO.Path.GetFullPath exceptions: If file is invalid.</exception>
        ''' <exception cref="System.ArgumentException">
        '''  If newName is Nothing or Empty String or contains path information.
        ''' </exception>
        ''' <exception cref="IO.FileNotFoundException">If file does not point to an existing file.</exception>
        ''' <exception cref="IO.IOException">
        '''  If there's an existing directory or an existing file with the same name.
        ''' </exception>
        Public Sub RenameFile(file As String, newName As String)
            FileIO.FileSystem.RenameFile(file, newName)
        End Sub

        ''' <summary>
        '''  Overwrites or appends the specified byte array to the specified file,
        '''  creating the file if it does not exist.
        ''' </summary>
        ''' <param name="file">The path to the file.</param>
        ''' <param name="data">The byte array to write to the file.</param>
        ''' <param name="append">
        '''  <see langword="True"/> to append the text to the existing content.
        '''  <see langword="False"/> to overwrite the existing content.
        ''' </param>
        ''' <exception cref="IO.FileStream">See FileStream constructor and Write: For other exceptions.</exception>
        Public Sub WriteAllBytes(file As String, data() As Byte, append As Boolean)
            FileIO.FileSystem.WriteAllBytes(file, data, append)
        End Sub

        ''' <summary>
        '''  Overwrites or appends the given text using UTF-8 encoding to the given file,
        '''  creating the file if it does not exist.
        ''' </summary>
        ''' <param name="file">The path to the file.</param>
        ''' <param name="text">The text to write to the file.</param>
        ''' <param name="append">
        '''  <see langword="True"/> to append the text to the existing content.
        '''  <see langword="False"/> to overwrite the existing content.</param>
        ''' <exception cref="IO.StreamWriter">See StreamWriter constructor and Write: For other exceptions.</exception>
        Public Sub WriteAllText(file As String, text As String, append As Boolean)
            FileIO.FileSystem.WriteAllText(file, text, append)
        End Sub

        ''' <summary>
        '''  Overwrites or appends the given text using the given encoding to the given file,
        '''  creating the file if it does not exist.
        ''' </summary>
        ''' <param name="file">The path to the file.</param>
        ''' <param name="text">The text to write to the file.</param>
        ''' <param name="append">
        '''  <see langword="True"/> to append the text to the existing content.
        '''  <see langword="False"/> to overwrite the existing content.</param>
        ''' <param name="encoding">The encoding to use.</param>
        ''' <exception cref="IO.StreamWriter">
        '''  See StreamWriter constructor and Write: For other exceptions.
        ''' </exception>
        Public Sub WriteAllText(
            file As String,
            text As String,
            append As Boolean,
            encoding As Encoding)

            FileIO.FileSystem.WriteAllText(file, text, append, encoding)
        End Sub

    End Class
End Namespace
