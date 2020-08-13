' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.
Option Strict On
Option Explicit On

Imports System.Security

Imports ExUtils = Microsoft.VisualBasic.CompilerServices.ExceptionUtils

Namespace Microsoft.VisualBasic.CompilerServices

    ''' <summary>
    '''  Internal utilities from Microsoft.VisualBasic.FileIO.FileSystem.
    ''' </summary>
    Friend Class FileSystemUtils

        ''' <summary>
        ''' Normalize the path, but throw exception if the path ends with separator.
        ''' </summary>
        ''' <param name="Path">The input path.</param>
        ''' <param name="ParamName">The parameter name to include in the exception if one is raised.</param>
        ''' <returns>The normalized path.</returns>
        Friend Shared Function NormalizeFilePath(Path As String, ParamName As String) As String
            CheckFilePathTrailingSeparator(Path, ParamName)
            Return NormalizePath(Path)
        End Function

        ''' <summary>
        ''' Get full path, get long format, and remove any pending separator.
        ''' </summary>
        ''' <param name="Path">The path to be normalized.</param>
        ''' <returns>The normalized path.</returns>
        ''' <exception cref="IO.Path.GetFullPath">See IO.Path.GetFullPath for possible exceptions.</exception>
        ''' <remarks>Keep this function since we might change the implementation / behavior later.</remarks>
        Friend Shared Function NormalizePath(Path As String) As String
            Return GetLongPath(RemoveEndingSeparator(IO.Path.GetFullPath(Path)))
        End Function

        ''' <summary>
        ''' Throw ArgumentException if the file path ends with a separator.
        ''' </summary>
        ''' <param name="path">The file path.</param>
        ''' <param name="paramName">The parameter name to include in ArgumentException.</param>
        Friend Shared Sub CheckFilePathTrailingSeparator(path As String, paramName As String)
            If String.IsNullOrEmpty(path) Then ' Check for argument null
                Throw ExUtils.GetArgumentNullException(paramName)
            End If
            If path.EndsWith(IO.Path.DirectorySeparatorChar, StringComparison.Ordinal) Or
                path.EndsWith(IO.Path.AltDirectorySeparatorChar, StringComparison.Ordinal) Then
                Throw ExUtils.GetArgumentExceptionWithArgName(paramName, SR.IO_FilePathException)
            End If
        End Sub

        ''' <summary>
        '''  Returns the given path in long format (v.s 8.3 format) if the path exists.
        ''' </summary>
        ''' <param name="FullPath">The path to resolve to long format.</param>
        ''' <returns>The given path in long format if the path exists.</returns>
        ''' <remarks>
        '''  GetLongPathName is a PInvoke call and requires unmanaged code permission.
        '''  Use DirectoryInfo.GetFiles and GetDirectories (which call FindFirstFile) so that we always have permission.
        '''</remarks>
        Private Shared Function GetLongPath(FullPath As String) As String
            Debug.Assert(Not String.IsNullOrEmpty(FullPath) AndAlso IO.Path.IsPathRooted(FullPath), "Must be full path")
            Try
                ' If root path, return itself. UNC path do not recognize 8.3 format in root path, so this is fine.
                If IsRoot(FullPath) Then
                    Return FullPath
                End If

                ' DirectoryInfo.GetFiles and GetDirectories call FindFirstFile which resolves 8.3 path.
                ' Get the DirectoryInfo (user must have code permission or access permission).
                Dim DInfo As New IO.DirectoryInfo(FileIO.FileSystem.GetParentPath(FullPath))

                If IO.File.Exists(FullPath) Then
                    Debug.Assert(DInfo.GetFiles(IO.Path.GetFileName(FullPath)).Length = 1, "Must found exactly 1")
                    Return DInfo.GetFiles(IO.Path.GetFileName(FullPath))(0).FullName
                ElseIf IO.Directory.Exists(FullPath) Then
                    Debug.Assert(DInfo.GetDirectories(IO.Path.GetFileName(FullPath)).Length = 1,
                                 "Must found exactly 1")
                    Return DInfo.GetDirectories(IO.Path.GetFileName(FullPath))(0).FullName
                Else
                    Return FullPath ' Path does not exist, cannot resolve.
                End If
            Catch ex As Exception
                ' Ignore these type of exceptions and return FullPath. These type of exceptions should either be caught by calling functions
                ' or indicate that caller does not have enough permission and should get back the 8.3 path.
                If TypeOf ex Is ArgumentException OrElse
                    TypeOf ex Is ArgumentNullException OrElse
                    TypeOf ex Is IO.PathTooLongException OrElse
                    TypeOf ex Is NotSupportedException OrElse
                    TypeOf ex Is IO.DirectoryNotFoundException OrElse
                    TypeOf ex Is SecurityException OrElse
                    TypeOf ex Is UnauthorizedAccessException Then

                    Debug.Assert(Not (TypeOf ex Is ArgumentException OrElse
                        TypeOf ex Is ArgumentNullException OrElse
                        TypeOf ex Is IO.PathTooLongException OrElse
                        TypeOf ex Is NotSupportedException), "These exceptions should be caught above")

                    Return FullPath
                Else
                    Throw
                End If
            End Try
        End Function

        ''' <summary>
        ''' Checks if the full path is a root path.
        ''' </summary>
        ''' <param name="Path">The path to check.</param>
        ''' <returns>True if FullPath is a root path, False otherwise.</returns>
        ''' <remarks>
        '''   IO.Path.GetPathRoot: C: -> C:, C:\ -> C:\, \\machine\share -> \\machine\share,
        '''           BUT \\machine\share\ -> \\machine\share (No separator here).
        '''   Therefore, remove any separators at the end to have correct result.
        ''' </remarks>
        Private Shared Function IsRoot(Path As String) As Boolean
            ' This function accepts a relative path since GetParentPath will call this,
            ' and GetParentPath accept relative paths.
            If Not IO.Path.IsPathRooted(Path) Then
                Return False
            End If

            Path = Path.TrimEnd(IO.Path.DirectorySeparatorChar, IO.Path.AltDirectorySeparatorChar)
            Return String.Compare(Path, IO.Path.GetPathRoot(Path),
                    StringComparison.OrdinalIgnoreCase) = 0
        End Function

        ''' <summary>
        ''' Removes all directory separators at the end of a path.
        ''' </summary>
        ''' <param name="Path">a full or relative path.</param>
        ''' <returns>If Path is a root path, the same value. Otherwise, removes any directory separators at the end.</returns>
        ''' <remarks>We decided not to return path with separators at the end.</remarks>
        Private Shared Function RemoveEndingSeparator(Path As String) As String
            If IO.Path.IsPathRooted(Path) Then
                ' If the path is rooted, attempt to check if it is a root path.
                ' Note: IO.Path.GetPathRoot: C: -> C:, C:\ -> C:\, \\myshare\mydir -> \\myshare\mydir
                ' BUT \\myshare\mydir\ -> \\myshare\mydir!!! This function will remove the ending separator of
                ' \\myshare\mydir\ as well. Do not use IsRoot here.
                If Path.Equals(IO.Path.GetPathRoot(Path), StringComparison.OrdinalIgnoreCase) Then
                    Return Path
                End If
            End If

            ' Otherwise, remove all separators at the end.
            Return Path.TrimEnd(IO.Path.DirectorySeparatorChar, IO.Path.AltDirectorySeparatorChar)
        End Function

    End Class

End Namespace
