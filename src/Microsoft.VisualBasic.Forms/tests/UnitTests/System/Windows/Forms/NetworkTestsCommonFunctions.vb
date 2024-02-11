' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.IO
Imports System.Runtime.CompilerServices

Namespace Microsoft.VisualBasic.Forms.Tests

    Partial Public Class NetworkTests
        Private Shared ReadOnly s_baseTempPath As String = Path.Combine(Path.GetTempPath, "DownLoadTest9d9e3a8-7a46-4333-a0eb-4faf76994801")

        Private Shared Function CreateTempDirectory(<CallerMemberName> Optional memberName As String = Nothing) As String

            Directory.CreateDirectory(s_baseTempPath)
            Dim folder As String = Path.Combine(s_baseTempPath, $"{memberName}Test")
            Directory.CreateDirectory(folder)
            Return folder
        End Function

        ''' <summary>
        ''' If size >= 0 then create the file with size length
        ''' The file will contain the letters A-Z repeating as needed.
        ''' </summary>
        ''' <param name="tmpFilePath">Full path to working directory</param>
        ''' <param name="size">File size to be created</param>
        ''' <returns>
        ''' The full path and file name of the created file
        ''' If size = -1 not file is create but the full path is returned
        ''' </returns>
        Private Shared Function CreateTempFile(tmpFilePath As String, Optional size As Integer = -1) As String
            Dim filename As String = Path.Combine(tmpFilePath, "testing.txt")
            If size >= 0 Then
                Using destinationStream As FileStream = File.Create(filename)
                    ' Below allows for limited testing of download data
                    For i As Long = 0 To size - 1
                        destinationStream.WriteByte(CByte((AscW("A") + (i Mod 26))))
                    Next
                    destinationStream.Flush()
                    destinationStream.Close()
                End Using
            End If
            Return filename
        End Function

    End Class

End Namespace
