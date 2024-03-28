' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.IO
Imports System.Runtime.CompilerServices

Namespace Microsoft.VisualBasic.Forms.Tests

    Partial Public Class NetworkTests

        ' The base path is system temp directory/a guaranteed unique directory based on a GUID/A temp directory bases on TestName
        Private Shared ReadOnly s_baseTempPath As String = Path.Combine(Path.GetTempPath, "DownLoadTest9d9e3a8-7a46-4333-a0eb-4faf76994801")

        ''' <summary>
        '''  Creates or returns a directory based on the name of the function that
        '''  call it. The base directory is described above.
        '''  Even if directory exists this call will success and just return it
        ''' </summary>
        ''' <param name="memberName"></param>
        ''' <returns></returns>
        Private Shared Function CreateTempDirectory(<CallerMemberName> Optional memberName As String = Nothing) As String
            Dim folder As String = Path.Combine(s_baseTempPath, $"{memberName}Test")

            Directory.CreateDirectory(folder)
            Return folder
        End Function

        ''' <summary>
        '''  If size >= 0 then create the file with size length
        ''' </summary>
        ''' <param name="tmpFilePath">Full path to working directory</param>
        ''' <param name="size">File size to be created</param>
        ''' <returns>
        '''  The full path and file name of the created file
        '''  If size = -1 no file is create but the full path is returned
        ''' </returns>
        Private Shared Function CreateTempFile(tmpFilePath As String, size As Integer) As String
            Dim filename As String = GetDestinationFileName(tmpFilePath)

            If size >= 0 Then
                Using destinationStream As FileStream = File.Create(filename)
                    destinationStream.Write(New Byte(size - 1) {})
                    destinationStream.Flush()
                    destinationStream.Close()
                End Using
            End If
            Return filename
        End Function

        Private Shared Function GetDestinationFileName(tmpFilePath As String) As String
            Return Path.Combine(tmpFilePath, "testing.txt")
        End Function

    End Class
End Namespace
