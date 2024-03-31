' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.IO
Imports System.Runtime.CompilerServices

Namespace Microsoft.VisualBasic.Forms.Tests

    Public Module TempDirectoryFileFunctions

        ' The base path is system temp directory/a guaranteed unique directory based on a GUID/A temp directory bases on TestName
        Friend ReadOnly s_baseTempPath As String = Path.Combine(Path.GetTempPath, "DownLoadTest9d9e3a8-7a46-4333-a0eb-4faf76994801")

        ''' <summary>
        '''  Creates or returns a directory based on the name of the function that
        '''  call it. The base directory is described above.
        '''  Even if directory exists this call will success and just return it
        ''' </summary>
        ''' <param name="memberName"></param>
        ''' <param name="lineNumber">If >1 use line number as part of name</param>
        ''' <returns></returns>
        Friend Function CreateTempDirectory(<CallerMemberName> Optional memberName As String = Nothing, Optional lineNumber As Integer = -1) As String
            Dim folder As String
            If lineNumber > 1 Then
                folder = Path.Combine(s_baseTempPath, $"{memberName}{lineNumber}")
            Else
                folder = Path.Combine(s_baseTempPath, memberName)
            End If

            Directory.CreateDirectory(folder)
            Return folder
        End Function

        ''' <summary>
        '''  If size >= 0 then create the file with size length
        ''' </summary>
        ''' <param name="tmpFilePath">Full path to working directory</param>
        ''' <param name="optionalFilename"></param>
        ''' <param name="size">File size to be created</param>
        ''' <returns>
        '''  The full path and file name of the created file
        '''  If size = -1 no file is create but the full path is returned
        ''' </returns>
        Friend Function CreateTempFile(tmpFilePath As String, Optional optionalFilename As String = "Testing.Text", Optional size As Integer = -1) As String
            Dim filename As String = GetDestinationFileName(tmpFilePath, optionalFilename)

            If size >= 0 Then
                Using destinationStream As FileStream = File.Create(filename)
                    destinationStream.Write(New Byte(size - 1) {})
                    destinationStream.Flush()
                    destinationStream.Close()
                End Using
            End If
            Return filename
        End Function

        Friend Function GetDestinationFileName(tmpFilePath As String, Optional filename As String = "testing.txt") As String
            Return Path.Combine(tmpFilePath, filename)
        End Function

    End Module
End Namespace
