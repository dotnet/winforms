' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Collections.ObjectModel
Imports System.Text
Imports Microsoft.VisualBasic.FileIO
Imports Microsoft.VisualBasic.MyServices
Imports Xunit

Namespace Microsoft.VisualBasic.Forms.Tests

    Public Class FileIoProxyTests
        Private ReadOnly _csvSampleData As String =
            "Index,Customer Id,First Name,Last Name,Company,City,Country
1,DD37Cf93aecA6Dc,Sheryl,Baxter,Rasmussen Group,East Leonard,Chile"
        Private ReadOnly _fixedSampleData As String =
            "IndexFirstLastCompanyCityCountry
4321;1234;321;654321;123;1234567"
        Private ReadOnly _fileSystem As FileSystemProxy = New Devices.ServerComputer().FileSystem

        <WinFormsFact>
        Public Sub DriveProxyTest()
            Assert.Equal(_fileSystem.Drives.Count, FileIO.FileSystem.Drives.Count)
        End Sub

        <WinFormsFact>
        Public Sub FindInFilesProxyTest()
            Dim testDirectory As String = CreateTempDirectory()
            Dim fileA As String = CreateTempFile(testDirectory, NameOf(fileA))
            _fileSystem.WriteAllText(fileA, "A", append:=False)
            Dim fileB As String = CreateTempFile(testDirectory, NameOf(fileB), size:=1)
            Dim fileC As String = CreateTempFile(testDirectory, NameOf(fileC))
            _fileSystem.WriteAllText(fileC, "C", append:=False)
            Dim filenames As ReadOnlyCollection(Of String) = _fileSystem.FindInFiles(testDirectory, containsText:="A", ignoreCase:=True, SearchOption.SearchTopLevelOnly)
            Assert.Equal(filenames.Count, 1)
            Assert.Equal(filenames(0), _fileSystem.CombinePath(testDirectory, NameOf(fileA)))
            filenames = _fileSystem.FindInFiles(testDirectory, containsText:="A", ignoreCase:=True, SearchOption.SearchTopLevelOnly, "*C")
            Assert.Equal(filenames.Count, 0)
            _fileSystem.DeleteDirectory(testDirectory, DeleteDirectoryOption.DeleteAllContents)
        End Sub

        <WinFormsFact>
        Public Sub ReadAllTextProxyTest()
            Dim testDirectory As String = CreateTempDirectory()
            Dim fileA As String = CreateTempFile(testDirectory, NameOf(fileA))
            _fileSystem.WriteAllText(fileA, "A", append:=False)
            Dim text As String = _fileSystem.ReadAllText(fileA)
            Assert.Equal(text, "A")
            _fileSystem.DeleteDirectory(testDirectory, DeleteDirectoryOption.DeleteAllContents)
        End Sub

        <WinFormsFact>
        Public Sub ReadAllTextWithEncodingProxyTest()
            Dim testDirectory As String = CreateTempDirectory()
            Dim fileA As String = CreateTempFile(testDirectory, NameOf(fileA))
            _fileSystem.WriteAllText(fileA, "A", append:=False, Encoding.UTF8)
            Dim text As String = _fileSystem.ReadAllText(fileA, Encoding.UTF8)
            Assert.Equal(text, "A")
            _fileSystem.DeleteDirectory(testDirectory, DeleteDirectoryOption.DeleteAllContents)
        End Sub

        <WinFormsFact>
        Public Sub ReadAllBytesProxyTest()
            Dim testDirectory As String = CreateTempDirectory()
            Dim fileA As String = CreateTempFile(testDirectory, NameOf(fileA))
            Dim byteArray As Byte() = {4}
            _fileSystem.WriteAllBytes(fileA, byteArray, append:=False)

            Dim bytes As Byte() = _fileSystem.ReadAllBytes(fileA)
            Assert.Equal(bytes.Length, 1)
            Assert.Equal(bytes(0), 4)
            _fileSystem.DeleteDirectory(testDirectory, DeleteDirectoryOption.DeleteAllContents)
        End Sub

        <WinFormsFact>
        Public Sub CopyFileProxyTest()
            Dim testDirectory As String = CreateTempDirectory()
            Dim file1 As String = CreateTempFile(testDirectory, NameOf(file1))
            Dim byteArray As Byte() = {4}
            _fileSystem.WriteAllBytes(file1, byteArray, append:=False)

            Dim file2 As String = CreateTempFile(testDirectory, NameOf(file2))
            _fileSystem.CopyFile(file1, file2)
            Dim bytes As Byte() = _fileSystem.ReadAllBytes(file2)
            Assert.Equal(bytes.Length, 1)
            Assert.Equal(bytes(0), 4)
            _fileSystem.DeleteDirectory(testDirectory, DeleteDirectoryOption.DeleteAllContents)
        End Sub

        <WinFormsFact>
        Public Sub CopyFileWithOverwriteProxyTest()
            Dim testDirectory As String = CreateTempDirectory()
            Dim file1 As String = CreateTempFile(testDirectory, NameOf(file1))
            Dim byteArray As Byte() = {4}
            _fileSystem.WriteAllBytes(file1, byteArray, append:=False)

            Dim file2 As String = CreateTempFile(testDirectory, NameOf(file2), size:=1)
            _fileSystem.CopyFile(file1, file2, overwrite:=True)
            Dim bytes As Byte() = _fileSystem.ReadAllBytes(file2)
            Assert.Equal(bytes.Length, 1)
            Assert.Equal(bytes(0), 4)
            _fileSystem.DeleteDirectory(testDirectory, DeleteDirectoryOption.DeleteAllContents)
        End Sub

        <WinFormsFact>
        Public Sub CopyFileWithShowUIProxyTest()
            Dim testDirectory As String = CreateTempDirectory()
            Dim file1 As String = CreateTempFile(testDirectory, NameOf(file1))
            Dim byteArray As Byte() = {4}
            _fileSystem.WriteAllBytes(file1, byteArray, append:=False)

            Dim file2 As String = CreateTempFile(testDirectory, NameOf(file2))
            _fileSystem.CopyFile(file1, file2, showUI:=UIOption.OnlyErrorDialogs)
            Dim bytes As Byte() = _fileSystem.ReadAllBytes(file2)
            Assert.Equal(bytes.Length, 1)
            Assert.Equal(bytes(0), 4)
            _fileSystem.DeleteDirectory(testDirectory, DeleteDirectoryOption.DeleteAllContents)
        End Sub

        <WinFormsFact>
        Public Sub CopyFileWithShowUIWithCancelOptionProxyTest()
            Dim testDirectory As String = CreateTempDirectory()
            Dim file1 As String = CreateTempFile(testDirectory, NameOf(file1))
            Dim byteArray As Byte() = {4}
            _fileSystem.WriteAllBytes(file1, byteArray, append:=False)

            Dim file2 As String = CreateTempFile(testDirectory, NameOf(file2))
            _fileSystem.CopyFile(file1, file2, UIOption.OnlyErrorDialogs, UICancelOption.DoNothing)
            Dim bytes As Byte() = _fileSystem.ReadAllBytes(file2)
            Assert.Equal(bytes.Length, 1)
            Assert.Equal(bytes(0), 4)
            _fileSystem.DeleteDirectory(testDirectory, DeleteDirectoryOption.DeleteAllContents)
        End Sub

        <WinFormsFact>
        Public Sub MoveFileWithShowUIProxyTest()
            Dim testDirectory As String = CreateTempDirectory()
            Dim file1 As String = CreateTempFile(testDirectory, NameOf(file1))
            Dim byteArray As Byte() = {4}
            _fileSystem.WriteAllBytes(file1, byteArray, append:=False)

            Dim file2 As String = CreateTempFile(testDirectory, NameOf(file2))
            _fileSystem.MoveFile(file1, file2, showUI:=UIOption.OnlyErrorDialogs)
            Dim bytes As Byte() = _fileSystem.ReadAllBytes(file2)
            Assert.False(IO.File.Exists(file1))
            Assert.True(IO.File.Exists(file2))
            Assert.Equal(bytes.Length, 1)
            Assert.Equal(bytes(0), 4)
            _fileSystem.DeleteDirectory(testDirectory, DeleteDirectoryOption.DeleteAllContents)
        End Sub

        <WinFormsFact>
        Public Sub MoveFileWithShowUIWithCancelOptionProxyTest()
            Dim testDirectory As String = CreateTempDirectory()
            Dim file1 As String = CreateTempFile(testDirectory, NameOf(file1))
            Dim byteArray As Byte() = {4}
            _fileSystem.WriteAllBytes(file1, byteArray, append:=False)

            Dim file2 As String = CreateTempFile(testDirectory, NameOf(file2))
            _fileSystem.MoveFile(file1, file2, showUI:=UIOption.OnlyErrorDialogs, UICancelOption.DoNothing)
            Dim bytes As Byte() = _fileSystem.ReadAllBytes(file2)
            Assert.False(IO.File.Exists(file1))
            Assert.True(IO.File.Exists(file2))
            Assert.Equal(bytes.Length, 1)
            Assert.Equal(bytes(0), 4)
            _fileSystem.DeleteDirectory(testDirectory, DeleteDirectoryOption.DeleteAllContents)
        End Sub

        <WinFormsFact>
        Public Sub CopyDirectoryWithShowUIProxyTest()
            Dim testDirectory As String = CreateTempDirectory(lineNumber:=1)
            Dim file1 As String = CreateTempFile(testDirectory, NameOf(file1))
            Dim byteArray As Byte() = {4}
            _fileSystem.WriteAllBytes(file1, byteArray, append:=False)

            Dim destinationDirectory As String = CreateTempDirectory(lineNumber:=2)
            _fileSystem.CopyDirectory(testDirectory, destinationDirectory, UIOption.OnlyErrorDialogs)
            Assert.True(IO.Directory.Exists(testDirectory))
            Assert.True(IO.Directory.Exists(destinationDirectory))
            Assert.Equal(IO.Directory.EnumerateFiles(testDirectory).Count, IO.Directory.EnumerateFiles(destinationDirectory).Count)
            _fileSystem.DeleteDirectory(testDirectory, DeleteDirectoryOption.DeleteAllContents)
            _fileSystem.DeleteDirectory(destinationDirectory, DeleteDirectoryOption.DeleteAllContents)
        End Sub

        <WinFormsFact>
        Public Sub CopyDirectoryWithShowUICancelOptionsProxyTest()
            Dim testDirectory As String = CreateTempDirectory(lineNumber:=1)
            Dim file1 As String = CreateTempFile(testDirectory, NameOf(file1))
            Dim byteArray As Byte() = {4}
            _fileSystem.WriteAllBytes(file1, byteArray, append:=False)

            Dim destinationDirectory As String = CreateTempDirectory(lineNumber:=2)
            _fileSystem.CopyDirectory(testDirectory, destinationDirectory, UIOption.OnlyErrorDialogs, UICancelOption.DoNothing)
            Assert.True(IO.Directory.Exists(testDirectory))
            Assert.True(IO.Directory.Exists(destinationDirectory))
            Assert.Equal(IO.Directory.EnumerateFiles(testDirectory).Count, IO.Directory.EnumerateFiles(destinationDirectory).Count)
            _fileSystem.DeleteDirectory(testDirectory, DeleteDirectoryOption.DeleteAllContents)
            _fileSystem.DeleteDirectory(destinationDirectory, DeleteDirectoryOption.DeleteAllContents)
        End Sub

        <WinFormsFact>
        Public Sub MoveDirectoryWithShowUIProxyTest()
            Dim testDirectory As String = CreateTempDirectory(lineNumber:=1)
            Dim file1 As String = CreateTempFile(testDirectory, NameOf(file1))
            Dim byteArray As Byte() = {4}
            _fileSystem.WriteAllBytes(file1, byteArray, append:=False)

            Dim destinationDirectory As String = CreateTempDirectory(lineNumber:=2)
            _fileSystem.MoveDirectory(testDirectory, destinationDirectory, UIOption.OnlyErrorDialogs)
            Assert.False(IO.Directory.Exists(testDirectory))
            Assert.True(IO.Directory.Exists(destinationDirectory))
            Assert.Equal(1, IO.Directory.EnumerateFiles(destinationDirectory).Count)
            _fileSystem.DeleteDirectory(destinationDirectory, DeleteDirectoryOption.DeleteAllContents)
        End Sub

        <WinFormsFact>
        Public Sub MoveDirectoryWithShowUICancelOptionsProxyTest()
            Dim testDirectory As String = CreateTempDirectory(lineNumber:=1)
            Dim file1 As String = CreateTempFile(testDirectory, NameOf(file1))
            Dim byteArray As Byte() = {4}
            _fileSystem.WriteAllBytes(file1, byteArray, append:=False)

            Dim destinationDirectory As String = CreateTempDirectory(lineNumber:=2)
            _fileSystem.MoveDirectory(testDirectory, destinationDirectory, UIOption.OnlyErrorDialogs, UICancelOption.DoNothing)
            Assert.False(IO.Directory.Exists(testDirectory))
            Assert.True(IO.Directory.Exists(destinationDirectory))
            Assert.Equal(1, IO.Directory.EnumerateFiles(destinationDirectory).Count)
            _fileSystem.DeleteDirectory(destinationDirectory, DeleteDirectoryOption.DeleteAllContents)
        End Sub

        <WinFormsFact>
        Public Sub DeleteFileProxyTest()
            Dim testDirectory As String = CreateTempDirectory()
            Dim file1 As String = CreateTempFile(testDirectory, NameOf(file1), size:=1)
            Dim byteArray As Byte() = {4}
            _fileSystem.WriteAllBytes(file1, byteArray, append:=False)
            Assert.True(IO.File.Exists(file1))

            _fileSystem.DeleteFile(file1)
            Assert.False(IO.File.Exists(file1))

            _fileSystem.DeleteDirectory(testDirectory, DeleteDirectoryOption.DeleteAllContents)
        End Sub

        <WinFormsFact>
        Public Sub DeleteFileWithRecycleOptionProxyTest()
            Dim testDirectory As String = CreateTempDirectory()
            Dim file1 As String = CreateTempFile(testDirectory, NameOf(file1), size:=1)
            Dim byteArray As Byte() = {4}
            _fileSystem.WriteAllBytes(file1, byteArray, append:=False)
            Assert.True(IO.File.Exists(file1))

            _fileSystem.DeleteFile(file1, UIOption.OnlyErrorDialogs, RecycleOption.DeletePermanently)
            Assert.False(IO.File.Exists(file1))

            _fileSystem.DeleteDirectory(testDirectory, DeleteDirectoryOption.DeleteAllContents)
        End Sub

        <WinFormsFact>
        Public Sub DeleteFileWithUIProxyTest()
            Dim testDirectory As String = CreateTempDirectory()
            Dim file1 As String = CreateTempFile(testDirectory, NameOf(file1), size:=1)
            Dim byteArray As Byte() = {4}
            _fileSystem.WriteAllBytes(file1, byteArray, append:=False)
            Assert.True(IO.File.Exists(file1))

            _fileSystem.DeleteFile(file1, UIOption.OnlyErrorDialogs, RecycleOption.DeletePermanently, UICancelOption.DoNothing)
            Assert.False(IO.File.Exists(file1))

            _fileSystem.DeleteDirectory(testDirectory, DeleteDirectoryOption.DeleteAllContents)
        End Sub

        <WinFormsFact>
        Public Sub DeleteDirectoryWithUIProxyRecycleTest()
            Dim testDirectory As String = CreateTempDirectory()
            Dim file1 As String = CreateTempFile(testDirectory, NameOf(file1), size:=1)
            Dim byteArray As Byte() = {4}
            _fileSystem.WriteAllBytes(file1, byteArray, append:=False)
            Assert.True(IO.File.Exists(file1))

            _fileSystem.DeleteDirectory(testDirectory, showUI:=UIOption.OnlyErrorDialogs, RecycleOption.DeletePermanently)
            Assert.False(IO.Directory.Exists(testDirectory))
        End Sub

        <WinFormsFact>
        Public Sub DeleteDirectoryRecycleWithUICancelOptionsProxyTest()
            Dim testDirectory As String = CreateTempDirectory()
            Dim file1 As String = CreateTempFile(testDirectory, NameOf(file1), size:=1)
            Dim byteArray As Byte() = {4}
            _fileSystem.WriteAllBytes(file1, byteArray, append:=False)
            Assert.True(IO.File.Exists(file1))

            _fileSystem.DeleteDirectory(testDirectory, showUI:=UIOption.OnlyErrorDialogs, RecycleOption.DeletePermanently, UICancelOption.DoNothing)
            Assert.False(IO.Directory.Exists(testDirectory))
        End Sub

        <WinFormsTheory>
        <InlineData(Nothing)>
        <InlineData(",")>
        Public Sub OpenTextFieldParserProxyTest(delimiter As String)
            Dim testDirectory As String = CreateTempDirectory()
            Dim fileCSV As String = CreateTempFile(testDirectory)
            _fileSystem.WriteAllText(fileCSV, _csvSampleData, append:=False)
            Dim reader As TextFieldParser
            If delimiter Is Nothing Then
                reader = _fileSystem.OpenTextFieldParser(fileCSV)
                reader.TextFieldType = FieldType.Delimited
                reader.Delimiters = {","}
            Else
                reader = _fileSystem.OpenTextFieldParser(fileCSV, delimiter)
            End If
            Dim currentRow As String()
            Dim totalRows As Integer = 0
            While Not reader.EndOfData
                totalRows += 1
                currentRow = reader.ReadFields()
                Dim currentField As String
                Dim totalFields As Integer = 0
                For Each currentField In currentRow
                    totalFields += 1
                Next
                Assert.Equal(7, totalFields)
            End While
            Assert.Equal(2, totalRows)
            reader.Close()
            _fileSystem.DeleteDirectory(testDirectory, DeleteDirectoryOption.DeleteAllContents)
        End Sub

        <WinFormsFact>
        Public Sub OpenTextFixedFieldParserProxyTest()
            Dim testDirectory As String = CreateTempDirectory()
            Dim fileCSV As String = CreateTempFile(testDirectory)
            _fileSystem.WriteAllText(fileCSV, _fixedSampleData, append:=False)
            Dim reader As TextFieldParser = _fileSystem.OpenTextFieldParser(fileCSV, 5, 5, 4, 7, 4, 7)
            Dim currentRow As String()
            Dim totalRows As Integer = 0
            Dim splitData As String() = _fixedSampleData.Split(vbCrLf)(1).Split(";")
            While Not reader.EndOfData
                currentRow = reader.ReadFields()
                Dim currentField As String
                Dim totalFields As Integer = 0
                For Each currentField In currentRow
                    If totalRows = 1 Then
                        Assert.Equal(splitData(totalFields), currentField.TrimEnd(";"c))
                    End If
                    totalFields += 1
                Next
                Assert.Equal(6, totalFields)
                totalRows += 1
            End While
            Assert.Equal(2, totalRows)
            reader.Close()
            _fileSystem.DeleteDirectory(testDirectory, DeleteDirectoryOption.DeleteAllContents)
        End Sub

        <WinFormsFact>
        Public Sub OpenTextStreamParserProxyTest()
            Dim testDirectory As String = CreateTempDirectory()
            Dim fileA As String = CreateTempFile(testDirectory)
            _fileSystem.WriteAllText(fileA, "A", append:=False)
            Using fileReader As IO.StreamReader = _fileSystem.OpenTextFileReader(fileA)
                Dim text As String = fileReader.ReadLine()
                Assert.Equal(text, "A")
            End Using

            _fileSystem.DeleteDirectory(testDirectory, DeleteDirectoryOption.DeleteAllContents)
        End Sub

        <WinFormsFact>
        Public Sub OpenTextFileWriterProxyTest()
            Dim testDirectory As String = CreateTempDirectory()
            Dim fileA As String = CreateTempFile(testDirectory)
            _fileSystem.WriteAllText(fileA, "A", append:=False)
            Using fileWriter As IO.StreamWriter = _fileSystem.OpenTextFileWriter(fileA, append:=False)
                fileWriter.WriteLine("A")
            End Using
            Using fileReader As IO.StreamReader = _fileSystem.OpenTextFileReader(fileA, Encoding.ASCII)
                Dim text As String = fileReader.ReadLine()
                Assert.Equal(text, "A")
            End Using

            _fileSystem.DeleteDirectory(testDirectory, DeleteDirectoryOption.DeleteAllContents)
        End Sub

        <WinFormsFact>
        Public Sub OpenEncodedTextFileWriterProxyTest()
            Dim testDirectory As String = CreateTempDirectory()
            Dim fileA As String = CreateTempFile(testDirectory)
            _fileSystem.WriteAllText(fileA, "A", append:=False)
            Using fileWriter As IO.StreamWriter = _fileSystem.OpenTextFileWriter(fileA, append:=False, Encoding.ASCII)
                fileWriter.WriteLine("A")
            End Using
            Using fileReader As IO.StreamReader = _fileSystem.OpenTextFileReader(fileA, Encoding.ASCII)
                Dim text As String = fileReader.ReadLine()
                Assert.Equal(text, "A")
            End Using

            _fileSystem.DeleteDirectory(testDirectory, DeleteDirectoryOption.DeleteAllContents)
        End Sub

    End Class
End Namespace
