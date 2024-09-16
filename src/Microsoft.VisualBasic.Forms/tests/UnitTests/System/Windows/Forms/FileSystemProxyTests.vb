' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Collections.ObjectModel
Imports System.IO
Imports System.Text
Imports FluentAssertions
Imports Microsoft.VisualBasic.CompilerServices
Imports Microsoft.VisualBasic.FileIO
Imports Microsoft.VisualBasic.MyServices
Imports Xunit

Namespace Microsoft.VisualBasic.Forms.Tests

    Public Class FileSystemProxyTests
        Inherits VbFileCleanupTestBase

        Private ReadOnly _fileSystem As FileSystemProxy = New Devices.ServerComputer().FileSystem

        Private ReadOnly _sampleDataCVS As String =
            "Index,Customer Id,First Name,Last Name,Company,City,Country" & vbCrLf &
            "1,DD37Cf93aecA6Dc,Sheryl,Baxter,Rasmussen Group,East Leonard,Chile"

        Private ReadOnly _sampleDataFixed As String =
            "IndexFirstLastCompanyCityCountry" & vbCrLf &
            "4321;1234;321;654321;123;1234567"

        <WinFormsFact>
        Public Sub CopyDirectoryWithShowUICancelOptionsProxyTest()
            Dim sourceDirectoryName As String = CreateTempDirectory(lineNumber:=1)
            Dim sourceFileName As String = CreateTempFile(sourceDirectoryName, filename:=NameOf(sourceFileName))
            Dim byteArray As Byte() = {4}
            _fileSystem.WriteAllBytes(sourceFileName, byteArray, append:=False)

            Dim destinationDirectoryName As String = CreateTempDirectory(lineNumber:=2)
            _fileSystem.CopyDirectory(
                sourceDirectoryName,
                destinationDirectoryName,
                showUI:=UIOption.OnlyErrorDialogs,
                onUserCancel:=UICancelOption.DoNothing)
            Directory.Exists(sourceDirectoryName).Should.BeTrue()
            Directory.Exists(destinationDirectoryName).Should.BeTrue()

            Dim count As Integer = Directory.EnumerateFiles(destinationDirectoryName).Count
            Directory.EnumerateFiles(sourceDirectoryName).Count.Should.Be(count)
        End Sub

        <WinFormsFact>
        Public Sub CopyDirectoryWithShowUIProxyTest()
            Dim sourceDirectoryName As String = CreateTempDirectory(lineNumber:=1)
            Dim sourceFileName As String = CreateTempFile(sourceDirectoryName, NameOf(sourceFileName))
            Dim data As Byte() = {4}
            _fileSystem.WriteAllBytes(sourceFileName, data, append:=False)

            Dim destinationDirectoryName As String = CreateTempDirectory(lineNumber:=2)
            _fileSystem.CopyDirectory(sourceDirectoryName, destinationDirectoryName, UIOption.OnlyErrorDialogs)
            Directory.Exists(sourceDirectoryName).Should.BeTrue()
            Directory.Exists(destinationDirectoryName).Should.BeTrue()
            Dim expected As Integer = Directory.EnumerateFiles(sourceDirectoryName).Count
            Directory.EnumerateFiles(destinationDirectoryName).Count.Should.Be(expected)
        End Sub

        <WinFormsFact>
        Public Sub CopyFileWithShowUIProxyTest()
            Dim sourceDirectoryName As String = CreateTempDirectory()
            Dim sourceFileName As String = CreateTempFile(sourceDirectoryName, NameOf(sourceFileName))
            Dim byteArray As Byte() = {4}
            _fileSystem.WriteAllBytes(sourceFileName, byteArray, append:=False)

            Dim destinationFileName As String = CreateTempFile(sourceDirectoryName, NameOf(destinationFileName))
            _fileSystem.CopyFile(sourceFileName, destinationFileName, showUI:=UIOption.OnlyErrorDialogs)
            Dim bytes As Byte() = _fileSystem.ReadAllBytes(destinationFileName)
            bytes.Length.Should.Be(1)
            bytes(0).Should.Be(4)
        End Sub

        <WinFormsFact>
        Public Sub CopyFileWithShowUIWithCancelOptionProxyTest()
            Dim sourceDirectoryName As String = CreateTempDirectory()
            Dim sourceFileName As String = CreateTempFile(sourceDirectoryName, NameOf(sourceFileName))
            Dim byteArray As Byte() = {4}
            _fileSystem.WriteAllBytes(sourceFileName, byteArray, append:=False)

            Dim destinationFileName As String = CreateTempFile(sourceDirectoryName, NameOf(destinationFileName))
            _fileSystem.CopyFile(
                sourceFileName,
                destinationFileName,
                showUI:=UIOption.OnlyErrorDialogs,
                onUserCancel:=UICancelOption.DoNothing)

            Dim bytes As Byte() = _fileSystem.ReadAllBytes(destinationFileName)
            bytes.Length.Should.Be(1)
            bytes(0).Should.Be(4)
        End Sub

        <WinFormsFact>
        Public Sub DeleteDirectoryRecycleWithUICancelOptionsProxyTest()
            Dim sourceDirectoryName As String = CreateTempDirectory()
            Dim sourceFileName As String = CreateTempFile(sourceDirectoryName, NameOf(sourceFileName), size:=1)
            Dim data As Byte() = {4}
            _fileSystem.WriteAllBytes(sourceFileName, data, append:=False)
            File.Exists(sourceFileName).Should.BeTrue()

            _fileSystem.DeleteDirectory(
                directory:=sourceDirectoryName,
                showUI:=UIOption.OnlyErrorDialogs,
                recycle:=RecycleOption.DeletePermanently,
                onUserCancel:=UICancelOption.DoNothing)
            Directory.Exists(sourceDirectoryName).Should.BeFalse()
        End Sub

        <WinFormsFact>
        Public Sub DeleteDirectoryWithUIProxyRecycleTest()
            Dim sourceDirectoryName As String = CreateTempDirectory()
            Dim sourceFileName As String = CreateTempFile(sourceDirectoryName, NameOf(sourceFileName), size:=1)
            Dim data As Byte() = {4}
            _fileSystem.WriteAllBytes(sourceFileName, data, append:=False)
            File.Exists(sourceFileName).Should.BeTrue()

            _fileSystem.DeleteDirectory(
                directory:=sourceDirectoryName,
                showUI:=UIOption.OnlyErrorDialogs,
                recycle:=RecycleOption.DeletePermanently)
            Directory.Exists(sourceDirectoryName).Should.BeFalse()
        End Sub

        <WinFormsFact>
        Public Sub DeleteFileWithRecycleOptionProxyTest()
            Dim sourceDirectoryName As String = CreateTempDirectory()
            Dim sourceFileName As String = CreateTempFile(sourceDirectoryName, NameOf(sourceFileName), size:=1)
            Dim byteArray As Byte() = {4}
            _fileSystem.WriteAllBytes(sourceFileName, byteArray, append:=False)
            File.Exists(sourceFileName).Should.BeTrue()

            _fileSystem.DeleteFile(
                sourceFileName,
                showUI:=UIOption.OnlyErrorDialogs,
                recycle:=RecycleOption.DeletePermanently)
            File.Exists(sourceFileName).Should.BeFalse()

            _fileSystem.DeleteDirectory(
                directory:=sourceDirectoryName,
                onDirectoryNotEmpty:=DeleteDirectoryOption.DeleteAllContents)
        End Sub

        <WinFormsFact>
        Public Sub DeleteFileWithUIProxyTest()
            Dim sourceDirectoryName As String = CreateTempDirectory()
            Dim sourceFileName As String = CreateTempFile(sourceDirectoryName, NameOf(sourceFileName), size:=1)
            Dim data As Byte() = {4}
            _fileSystem.WriteAllBytes(sourceFileName, data, append:=False)
            File.Exists(sourceFileName).Should.BeTrue()

            _fileSystem.DeleteFile(
                file:=sourceFileName,
                showUI:=UIOption.OnlyErrorDialogs,
                recycle:=RecycleOption.DeletePermanently,
                onUserCancel:=UICancelOption.DoNothing)
            File.Exists(sourceFileName).Should.BeFalse()
        End Sub

        <WinFormsFact>
        Public Sub DriveProxyTest()
            _fileSystem.Drives.Count.Should.Be(FileIO.FileSystem.Drives.Count)
        End Sub

        <WinFormsFact>
        Public Sub FileNormalizePathEmptyStringTest_Fail()
            Dim testCode As Action = Sub() FileSystemUtils.NormalizePath("")
            testCode.Should.Throw(Of ArgumentException)()
        End Sub

        <WinFormsFact>
        Public Sub FileNormalizePathNullTest_Fail()
            Dim testCode As Action = Sub() FileSystemUtils.NormalizePath(Nothing)
            testCode.Should.Throw(Of ArgumentNullException)()
        End Sub

        <WinFormsFact>
        Public Sub FileNormalizePathTest_Success()
            Dim expected As String = Path.GetTempPath.TrimEnd(Path.DirectorySeparatorChar)
            FileSystemUtils.NormalizePath(Path.GetTempPath).Should.Be(expected)
        End Sub

        <WinFormsFact>
        Public Sub FindInFilesProxyTest()
            Dim sourceDirectoryName As String = CreateTempDirectory()
            Dim fileA As String = CreateTempFile(sourceDirectoryName, NameOf(fileA))
            _fileSystem.WriteAllText(fileA, "A", append:=False)
            Dim fileB As String = CreateTempFile(sourceDirectoryName, NameOf(fileB), size:=1)
            Dim fileC As String = CreateTempFile(sourceDirectoryName, NameOf(fileC))
            _fileSystem.WriteAllText(fileC, "C", append:=False)
            Dim filenames As ReadOnlyCollection(Of String) = _fileSystem.FindInFiles(
                directory:=sourceDirectoryName,
                containsText:="A",
                ignoreCase:=True,
                searchType:=FileIO.SearchOption.SearchTopLevelOnly)
            filenames.Count.Should.Be(1)
            Dim expected As String = filenames(0)
            _fileSystem.CombinePath(sourceDirectoryName, NameOf(fileA)).Should.Be(expected)
            filenames = _fileSystem.FindInFiles(
                directory:=sourceDirectoryName,
                containsText:="A",
                ignoreCase:=True,
                searchType:=FileIO.SearchOption.SearchTopLevelOnly,
                "*C")
            filenames.Count.Should.Be(0)
        End Sub

        <WinFormsFact>
        Public Sub MoveDirectoryWithShowUICancelOptionsProxyTest()
            Dim sourceDirectoryName As String = CreateTempDirectory(lineNumber:=1)
            Dim sourceFileName As String = CreateTempFile(sourceDirectoryName, NameOf(sourceFileName))
            Dim byteArray As Byte() = {4}
            _fileSystem.WriteAllBytes(sourceFileName, byteArray, append:=False)

            Dim destinationDirectoryName As String = CreateTempDirectory(lineNumber:=2)
            _fileSystem.MoveDirectory(
                sourceDirectoryName,
                destinationDirectoryName,
                showUI:=UIOption.OnlyErrorDialogs,
                onUserCancel:=UICancelOption.DoNothing)

            Directory.Exists(sourceDirectoryName).Should.BeFalse()
            Directory.Exists(destinationDirectoryName).Should.BeTrue()
            Directory.EnumerateFiles(destinationDirectoryName).Count.Should.Be(1)
        End Sub

        <WinFormsFact>
        Public Sub MoveFileWithShowUIProxyTest()
            Dim sourceDirectoryName As String = CreateTempDirectory()
            Dim sourceFileName As String = CreateTempFile(sourceDirectoryName, NameOf(sourceFileName))
            Dim byteArray As Byte() = {4}
            _fileSystem.WriteAllBytes(sourceFileName, byteArray, append:=False)

            Dim file2 As String = CreateTempFile(sourceDirectoryName, NameOf(file2))
            _fileSystem.MoveFile(sourceFileName, file2, showUI:=UIOption.OnlyErrorDialogs)
            Dim bytes As Byte() = _fileSystem.ReadAllBytes(file2)
            File.Exists(sourceFileName).Should.BeFalse()
            File.Exists(file2).Should.BeTrue()
            bytes.Length.Should.Be(1)
            bytes(0).Should.Be(4)
        End Sub

        <WinFormsFact>
        Public Sub MoveFileWithShowUIWithCancelOptionProxyTest()
            Dim sourceDirectoryName As String = CreateTempDirectory()
            Dim sourceFileName As String = CreateTempFile(sourceDirectoryName, NameOf(sourceFileName))
            Dim byteArray As Byte() = {4}
            _fileSystem.WriteAllBytes(sourceFileName, byteArray, append:=False)

            Dim destinationFileName As String = CreateTempFile(sourceDirectoryName, NameOf(destinationFileName))
            _fileSystem.MoveFile(
                sourceFileName,
                destinationFileName,
                showUI:=UIOption.OnlyErrorDialogs,
                onUserCancel:=UICancelOption.DoNothing)
            Dim bytes As Byte() = _fileSystem.ReadAllBytes(destinationFileName)
            File.Exists(sourceFileName).Should.BeFalse()
            File.Exists(destinationFileName).Should.BeTrue()
            bytes.Length.Should.Be(1)
            bytes(0).Should.Be(4)
        End Sub

        <WinFormsFact>
        Public Sub OpenEncodedTextFileWriterProxyTest()
            Dim sourceDirectoryName As String = CreateTempDirectory()
            Dim fileA As String = CreateTempFile(sourceDirectoryName)
            _fileSystem.WriteAllText(fileA, "A", append:=False)
            Using fileWriter As IO.StreamWriter = _fileSystem.OpenTextFileWriter(
                    file:=fileA,
                    append:=False,
                    encoding:=Encoding.ASCII)
                fileWriter.WriteLine("A")
            End Using
            Using fileReader As IO.StreamReader = _fileSystem.OpenTextFileReader(
                    file:=fileA,
                    encoding:=Encoding.ASCII)
                Dim text As String = fileReader.ReadLine()
                text.Should.Be("A")
            End Using
        End Sub

        <WinFormsTheory>
        <InlineData(Nothing)>
        <InlineData(",")>
        Public Sub OpenTextFieldParserProxyTest(delimiter As String)
            Dim sourceDirectoryName As String = CreateTempDirectory()
            Dim fileCSV As String = CreateTempFile(sourceDirectoryName)
            _fileSystem.WriteAllText(fileCSV, _sampleDataCVS, append:=False)
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
                totalFields.Should.Be(7)
            End While
            totalRows.Should.Be(2)
            reader.Close()
        End Sub

        <WinFormsFact>
        Public Sub OpenTextFileWriterProxyTest()
            Dim sourceDirectoryName As String = CreateTempDirectory()
            Dim fileA As String = CreateTempFile(sourceDirectoryName)
            _fileSystem.WriteAllText(fileA, "A", append:=False)
            Using fileWriter As IO.StreamWriter = _fileSystem.OpenTextFileWriter(fileA, append:=False)
                fileWriter.WriteLine("A")
            End Using
            Using fileReader As IO.StreamReader = _fileSystem.OpenTextFileReader(fileA, Encoding.ASCII)
                Dim text As String = fileReader.ReadLine()
                text.Should.Be("A")
            End Using
        End Sub

        <WinFormsFact>
        Public Sub OpenTextFixedFieldParserProxyTest()
            Dim sourceDirectoryName As String = CreateTempDirectory()
            Dim fileCSV As String = CreateTempFile(sourceDirectoryName)
            _fileSystem.WriteAllText(fileCSV, _sampleDataFixed, append:=False)
            Dim reader As TextFieldParser = _fileSystem.OpenTextFieldParser(fileCSV, 5, 5, 4, 7, 4, 7)
            Dim currentRow As String()
            Dim totalRows As Integer = 0
            Dim splitData As String() = _sampleDataFixed.Split(vbCrLf)(1).Split(";")
            While Not reader.EndOfData
                currentRow = reader.ReadFields()
                Dim currentField As String
                Dim totalFields As Integer = 0
                For Each currentField In currentRow
                    If totalRows = 1 Then
                        splitData(totalFields).Should.Be(currentField.TrimEnd(";"c))
                    End If
                    totalFields += 1
                Next
                totalFields.Should.Be(6)
                totalRows += 1
            End While
            totalRows.Should.Be(2)
            reader.Close()
        End Sub

        <WinFormsFact>
        Public Sub OpenTextStreamParserProxyTest()
            Dim sourceDirectoryName As String = CreateTempDirectory()
            Dim fileA As String = CreateTempFile(sourceDirectoryName)
            _fileSystem.WriteAllText(fileA, "A", append:=False)
            Using fileReader As IO.StreamReader = _fileSystem.OpenTextFileReader(fileA)
                Dim text As String = fileReader.ReadLine()
                text.Should.Be("A")
            End Using
        End Sub

        <WinFormsFact>
        Public Sub ReadAllBytesProxyTest()
            Dim sourceDirectoryName As String = CreateTempDirectory()
            Dim fileA As String = CreateTempFile(sourceDirectoryName, NameOf(fileA))
            Dim byteArray As Byte() = {4}
            _fileSystem.WriteAllBytes(fileA, byteArray, append:=False)

            Dim bytes As Byte() = _fileSystem.ReadAllBytes(fileA)
            bytes.Length.Should.Be(1)
            bytes(0).Should.Be(4)
        End Sub

        <WinFormsFact>
        Public Sub ReadAllTextProxyTest()
            Dim sourceDirectoryName As String = CreateTempDirectory()
            Dim fileA As String = CreateTempFile(sourceDirectoryName, NameOf(fileA))
            _fileSystem.WriteAllText(fileA, "A", append:=False)
            Dim text As String = _fileSystem.ReadAllText(fileA)
            text.Should.Be("A")
        End Sub

        <WinFormsFact>
        Public Sub ReadAllTextWithEncodingProxyTest()
            Dim sourceDirectoryName As String = CreateTempDirectory()
            Dim fileA As String = CreateTempFile(sourceDirectoryName, NameOf(fileA))
            _fileSystem.WriteAllText(fileA, "A", append:=False, Encoding.UTF8)
            Dim text As String = _fileSystem.ReadAllText(fileA, Encoding.UTF8)
            text.Should.Be("A")
        End Sub

        <WinFormsFact>
        Public Sub SpecialDirectoriesTest()
            _fileSystem.SpecialDirectories.AllUsersApplicationData.
                Should.Be(SpecialDirectories.AllUsersApplicationData)
            _fileSystem.SpecialDirectories.CurrentUserApplicationData.
                Should.Be(SpecialDirectories.CurrentUserApplicationData)
            _fileSystem.SpecialDirectories.Desktop.Should.Be(SpecialDirectories.Desktop)
            _fileSystem.SpecialDirectories.MyDocuments.Should.Be(SpecialDirectories.MyDocuments)
            _fileSystem.SpecialDirectories.MyMusic.Should.Be(SpecialDirectories.MyMusic)
            _fileSystem.SpecialDirectories.MyPictures.Should.Be(SpecialDirectories.MyPictures)
            _fileSystem.SpecialDirectories.ProgramFiles.Should.Be(SpecialDirectories.ProgramFiles)
            _fileSystem.SpecialDirectories.Programs.Should.Be(SpecialDirectories.Programs)
            Dim temp As String = _fileSystem.SpecialDirectories.Temp
            temp.Should.Be(SpecialDirectories.Temp)
            temp &= Path.DirectorySeparatorChar
            temp.Should.Be(Path.GetTempPath)
        End Sub

    End Class
End Namespace
