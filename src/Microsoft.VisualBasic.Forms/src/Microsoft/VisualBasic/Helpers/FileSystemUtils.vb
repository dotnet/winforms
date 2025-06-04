' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.IO
Imports System.Security

Imports VbUtils = Microsoft.VisualBasic.CompilerServices.ExceptionUtils

Namespace Microsoft.VisualBasic.CompilerServices

    ''' <summary>
    '''  Internal utilities from Microsoft.VisualBasic.FileIO.FileSystem.
    ''' </summary>
    Friend NotInheritable Class FileSystemUtils

        ''' <summary>
        '''  Returns the given path in long format (v.s 8.3 format) if the path exists.
        ''' </summary>
        ''' <param name="fullPath">The path to resolve to long format.</param>
        ''' <returns>The given path in long format if the path exists.</returns>
        ''' <remarks>
        '''  GetLongPathName is a PInvoke call and requires unmanaged code permission.
        '''  Use <see cref="DirectoryInfo.GetFiles"/> and <see cref="DirectoryInfo.GetDirectories"/> (which call FindFirstFile) so that we always have permission.
        '''</remarks>
        Private Shared Function GetLongPath(fullPath As String) As String
            Debug.Assert(Not String.IsNullOrEmpty(fullPath) AndAlso IO.Path.IsPathRooted(fullPath), "Must be full path")
            Try
                ' If root path, return itself. UNC path do not recognize 8.3 format in root path, so this is fine.
                If IsRoot(fullPath) Then
                    Return fullPath
                End If

                ' DirectoryInfo.GetFiles and GetDirectories call FindFirstFile which resolves 8.3 path.
                ' Get the DirectoryInfo (user must have code permission or access permission).
                Dim dInfo As New DirectoryInfo(FileIO.FileSystem.GetParentPath(fullPath))

                If IO.File.Exists(fullPath) Then
                    Debug.Assert(dInfo.GetFiles(IO.Path.GetFileName(fullPath)).Length = 1, "Must found exactly 1")
                    Return dInfo.GetFiles(IO.Path.GetFileName(fullPath))(0).FullName
                ElseIf IO.Directory.Exists(fullPath) Then
                    Debug.Assert(dInfo.GetDirectories(IO.Path.GetFileName(fullPath)).Length = 1,
                                 "Must found exactly 1")
                    Return dInfo.GetDirectories(IO.Path.GetFileName(fullPath))(0).FullName
                Else
                    ' Path does not exist, cannot resolve.
                    Return fullPath
                End If
            Catch ex As Exception
                ' Ignore these type of exceptions and return FullPath. These type of exceptions should either be caught by calling functions
                ' or indicate that caller does not have enough permission and should get back the 8.3 path.
                If TypeOf ex Is ArgumentException OrElse
                    TypeOf ex Is ArgumentNullException OrElse
                    TypeOf ex Is PathTooLongException OrElse
                    TypeOf ex Is NotSupportedException OrElse
                    TypeOf ex Is DirectoryNotFoundException OrElse
                    TypeOf ex Is SecurityException OrElse
                    TypeOf ex Is UnauthorizedAccessException Then

                    Debug.Assert(Not (TypeOf ex Is ArgumentException OrElse
                        TypeOf ex Is ArgumentNullException OrElse
                        TypeOf ex Is PathTooLongException OrElse
                        TypeOf ex Is NotSupportedException), "These exceptions should be caught above")

                    Return fullPath
                Else
                    Throw
                End If
            End Try
        End Function

        ''' <summary>
        '''  Checks if the full path is a root path.
        ''' </summary>
        ''' <param name="path">The path to check.</param>
        ''' <returns><see langword="True"/> if FullPath is a root path, <see langword="False"/> otherwise.</returns>
        ''' <remarks>
        '''   IO.Path.GetPathRoot: C: -> C:, C:\ -> C:\, \\machine\share -> \\machine\share,
        '''           BUT \\machine\share\ -> \\machine\share (No separator here).
        '''   Therefore, remove any separators at the end to have correct result.
        ''' </remarks>
        Private Shared Function IsRoot(path As String) As Boolean
            ' This function accepts a relative path since GetParentPath will call this,
            ' and GetParentPath accept relative paths.
            If Not IO.Path.IsPathRooted(path) Then
                Return False
            End If

            path = path.TrimEnd(IO.Path.DirectorySeparatorChar, IO.Path.AltDirectorySeparatorChar)
            Return String.Equals(path, IO.Path.GetPathRoot(path),
                    StringComparison.OrdinalIgnoreCase)
        End Function

        ''' <summary>
        '''  Removes all directory separators at the end of a path.
        ''' </summary>
        ''' <param name="path">a full or relative path.</param>
        ''' <returns>If Path is a root path, the same value. Otherwise, removes any directory separators at the end.</returns>
        ''' <remarks>We decided not to return path with separators at the end.</remarks>
        Private Shared Function RemoveEndingSeparator(path As String) As String
            If IO.Path.IsPathRooted(path) Then
                ' If the path is rooted, attempt to check if it is a root path.
                ' Note: IO.Path.GetPathRoot: C: -> C:, C:\ -> C:\, \\myshare\mydir -> \\myshare\mydir
                ' BUT \\myshare\mydir\ -> \\myshare\mydir!!! This function will remove the ending separator of
                ' \\myshare\mydir\ as well. Do not use IsRoot here.
                If path.Equals(IO.Path.GetPathRoot(path), StringComparison.OrdinalIgnoreCase) Then
                    Return path
                End If
            End If

            ' Otherwise, remove all separators at the end.
            Return path.TrimEnd(IO.Path.DirectorySeparatorChar, IO.Path.AltDirectorySeparatorChar)
        End Function

        ''' <summary>
        '''  Throw <see cref="ArgumentException "/> if the file path ends with a separator.
        ''' </summary>
        ''' <param name="path">The file path.</param>
        ''' <param name="paramName">The parameter name to include in <see cref="ArgumentException "/> .</param>
        Friend Shared Sub CheckFilePathTrailingSeparator(path As String,
                                paramName As String)
            ' Check for argument null
            If String.IsNullOrEmpty(path) Then
                Throw VbUtils.GetArgumentNullException(paramName)
            End If
            If path.EndsWith(IO.Path.DirectorySeparatorChar, StringComparison.Ordinal) Or
                path.EndsWith(IO.Path.AltDirectorySeparatorChar, StringComparison.Ordinal) Then
                Throw VbUtils.GetArgumentExceptionWithArgName(paramName, SR.IO_FilePathException)
            End If
        End Sub

        ''' <summary>
        '''  Normalize the path, but throw exception if the path ends with separator.
        ''' </summary>
        ''' <param name="path">The input path.</param>
        ''' <param name="paramName">The parameter name to include in the
        '''  exception if one is raised.
        ''' </param>
        ''' <returns>The normalized path.</returns>
        Friend Shared Function NormalizeFilePath(
            path As String,
            paramName As String) As String

            CheckFilePathTrailingSeparator(path, paramName)
            Return NormalizePath(path)
        End Function

        ''' <summary>
        '''  Get full path, get long format, and remove any pending separator.
        ''' </summary>
        ''' <param name="path">The path to be normalized.</param>
        ''' <returns>The normalized path.</returns>
        ''' <exception cref="Path.GetFullPath">
        '''  <see cref="Path.GetFullPath"/> for possible exceptions.
        ''' </exception>
        ''' <remarks>
        '''  Keep this function since we might change the implementation / behavior later.
        ''' </remarks>
        Friend Shared Function NormalizePath(path As String) As String
            Return GetLongPath(RemoveEndingSeparator(IO.Path.GetFullPath(path)))
        End Function

    End Class
End Namespace
