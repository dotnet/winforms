' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.IO
Imports System.Runtime.CompilerServices

Namespace Microsoft.VisualBasic.Forms.Tests

    Public MustInherit Class VbFileCleanupTestBase
        Implements IDisposable

        Private Shared ReadOnly s_baseTempPath As String = Path.Combine(Path.GetTempPath, "DownLoadTest9d9e3a8-7a46-4333-a0eb-4faf76994801")
        Friend Const DefaultFileName As String = "Testing.Txt"
        Friend ReadOnly _testDirectories As New HashSet(Of String)

        Protected Overrides Sub Finalize()
            Dispose(disposing:=False)
        End Sub

        ' The base path is system temp directory / a guaranteed unique directory based on a GUID / a temp directory based on TestName
        Friend Shared ReadOnly Property BaseTempPath As String
            Get
                Return s_baseTempPath
            End Get
        End Property

        Protected Overridable Sub Dispose(disposing As Boolean)
            Try
                For Each testDirectoryName As String In _testDirectories
                    If Directory.Exists(testDirectoryName) Then
                        Directory.Delete(testDirectoryName, recursive:=True)
                    End If
                Next
            Catch
            End Try
        End Sub

        ''' <summary>
        '''  If size >= 0 then create the file with size length.
        ''' </summary>
        ''' <param name="sourceDirectoryName">Full path to working directory.</param>
        ''' <param name="filename"></param>
        ''' <param name="size">Size in bytes of the file to be created.</param>
        ''' <returns>
        '''  The full path and file name of the created file.
        '''  If size = -1 no file is create but the full path is returned.
        ''' </returns>
        Friend Shared Function CreateTempFile(sourceDirectoryName As String, Optional filename As String = DefaultFileName, Optional size As Integer = -1) As String
            Dim filenameWithPath As String = Path.Combine(sourceDirectoryName, filename)

            If size >= 0 Then
                Using destinationStream As FileStream = File.Create(filenameWithPath)
                    destinationStream.Write(New Byte(size - 1) {})
                    destinationStream.Flush()
                    destinationStream.Close()
                End Using
            End If
            Return filenameWithPath
        End Function

        Friend Shared Function DirectoryIsAccessible(directoryPath As String) As Boolean
            If String.IsNullOrWhiteSpace(directoryPath) Then
                Return False
            End If

            Try
                Dim info As New DirectoryInfo(directoryPath)
                If Not info.Exists Then
                    Return False
                End If
                Dim path As String = IO.Path.Combine(directoryPath, GetUniqueFileName())
                Using stream As FileStream = File.Create(path)
                    stream.Close()
                End Using
                File.Delete(path)
            Catch s As Security.SecurityException
                Return False
            Catch
                Return False
            End Try
            Return True
        End Function

        Friend Shared Function GetUniqueFileNameWithPath(testDirectory As String) As String
            Return Path.Combine(testDirectory, GetUniqueFileName())
        End Function

        ''' <summary>
        '''  Creates or returns a directory based on the name of the function that
        '''  call it. The base directory is described above.
        '''  Even if directory exists this call will success and just return it.
        ''' </summary>
        ''' <param name="memberName"></param>
        ''' <param name="lineNumber">If >0 use line number as part of name.</param>
        ''' <returns>The name of a directory that is safe to write to and is verified to exist.</returns>
        Friend Function CreateTempDirectory(<CallerMemberName> Optional memberName As String = Nothing, Optional lineNumber As Integer = -1) As String
            Dim folder As String
            If lineNumber > 0 Then
                folder = Path.Combine(BaseTempPath, $"{memberName}{lineNumber}")
            Else
                folder = Path.Combine(BaseTempPath, memberName)
            End If

            If _testDirectories.Add(folder) Then
                Directory.CreateDirectory(folder)
            End If

            Return folder
        End Function

        Friend Sub Dispose() Implements IDisposable.Dispose
            Dispose(disposing:=True)
            GC.SuppressFinalize(Me)
        End Sub

    End Class
End Namespace
