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

    Public Class FileIoProxyTests

        Private ReadOnly _csvSampleData As String =
            "Index,Customer Id,First Name,Last Name,Company,City,Country
1,DD37Cf93aecA6Dc,Sheryl,Baxter,Rasmussen Group,East Leonard,Chile"

        Private ReadOnly _fileSystem As FileSystemProxy = New Devices.ServerComputer().FileSystem

        Private ReadOnly _fixedSampleData As String =
                    "IndexFirstLastCompanyCityCountry
4321;1234;321;654321;123;1234567"

        Private Sub CleanupDirectories(testDirectory As String, Optional destinationDirectory As String = Nothing, Optional onDirectoryNotEmpty As DeleteDirectoryOption = DeleteDirectoryOption.DeleteAllContents)
            If String.IsNullOrEmpty(testDirectory) Then
                Throw New ArgumentException($"'{NameOf(testDirectory)}' cannot be null or empty.", NameOf(testDirectory))
            End If
            If testDirectory = Path.GetTempPath Then
                Throw New ArgumentException($"'{NameOf(testDirectory)}' cannot be {Path.GetTempPath}.", NameOf(testDirectory))
            End If
            If Not testDirectory.StartsWith(Path.GetTempPath, StringComparison.InvariantCultureIgnoreCase) Then
                Throw New ArgumentException($"'{NameOf(testDirectory)}' must start with {Path.GetTempPath}.", NameOf(testDirectory))
            End If

            _fileSystem.DeleteDirectory(testDirectory, onDirectoryNotEmpty)

            If destinationDirectory IsNot Nothing Then
                If destinationDirectory = Path.GetTempPath Then
                    Throw New ArgumentException($"'{NameOf(destinationDirectory)}' cannot be {Path.GetTempPath}.", NameOf(destinationDirectory))
                End If
                If Not destinationDirectory.StartsWith(Path.GetTempPath, StringComparison.InvariantCultureIgnoreCase) Then
                    Throw New ArgumentException($"'{NameOf(destinationDirectory)}' must start with {Path.GetTempPath}.", NameOf(destinationDirectory))
                End If
                _fileSystem.DeleteDirectory(destinationDirectory, onDirectoryNotEmpty)
            End If
        End Sub

        <WinFormsTheory>
        <ClassData(GetType(BadPathsTestData))>
        Public Sub CleanupDirectoriesTests(testDirectory As String)
            Dim testCode As Action =
                Sub()
                    CleanupDirectories(testDirectory, onDirectoryNotEmpty:=DeleteDirectoryOption.ThrowIfDirectoryNonEmpty)
                End Sub

            testCode.Should.Throw(Of ArgumentException)()
        End Sub

        <WinFormsTheory>
        <ClassData(GetType(BadPathsTestData))>
        Public Sub CleanupDirectoriesWithDestinationDirectoryTests(destinationDirectory As String)
            Dim testDirectory As String = CreateTempDirectory()
            Dim testCode As Action = Sub() CleanupDirectories(testDirectory, destinationDirectory, onDirectoryNotEmpty:=DeleteDirectoryOption.ThrowIfDirectoryNonEmpty)
            If destinationDirectory IsNot Nothing Then
                testCode.Should.Throw(Of ArgumentException)()
            End If
        End Sub

        <WinFormsFact>
        Public Sub CopyDirectoryProxyTest()
            Dim testDirectory As String = CreateTempDirectory(lineNumber:=1)
            Dim file1 As String = CreateTempFile(tmpFilePath:=testDirectory, optionalFilename:=NameOf(file1))
            Dim byteArray As Byte() = {4}
            _fileSystem.WriteAllBytes(file1, byteArray, append:=False)

            Dim destinationDirectory As String = CreateTempDirectory(lineNumber:=2)
            _fileSystem.CopyDirectory(
                sourceDirectoryName:=testDirectory,
                destinationDirectoryName:=destinationDirectory)
            Directory.Exists(testDirectory).Should.BeTrue()
            Directory.Exists(destinationDirectory).Should.BeTrue()

            Dim count As Integer = Directory.EnumerateFiles(destinationDirectory).Count
            Directory.EnumerateFiles(testDirectory).Count.Should.Be(count)

            CleanupDirectories(testDirectory, destinationDirectory)
        End Sub

        <WinFormsFact>
        Public Sub CopyDirectoryProxyWithOverwriteTest()
            Dim testDirectory As String = CreateTempDirectory(lineNumber:=1)
            Dim file1 As String = CreateTempFile(tmpFilePath:=testDirectory, optionalFilename:=NameOf(file1))
            Dim byteArray As Byte() = {4}
            _fileSystem.WriteAllBytes(file1, byteArray, append:=False)

            Dim destinationDirectory As String = CreateTempDirectory(lineNumber:=2)
            _fileSystem.CopyDirectory(
                sourceDirectoryName:=testDirectory,
                destinationDirectoryName:=destinationDirectory,
                overwrite:=False)
            Directory.Exists(testDirectory).Should.BeTrue()
            Directory.Exists(destinationDirectory).Should.BeTrue()

            Dim count As Integer = Directory.EnumerateFiles(destinationDirectory).Count
            Directory.EnumerateFiles(testDirectory).Count.Should.Be(count)

            CleanupDirectories(testDirectory, destinationDirectory)
        End Sub

        <WinFormsFact>
        Public Sub CopyDirectoryWithShowUICancelOptionsProxyTest()
            Dim testDirectory As String = CreateTempDirectory(lineNumber:=1)
            Dim file1 As String = CreateTempFile(tmpFilePath:=testDirectory, optionalFilename:=NameOf(file1))
            Dim byteArray As Byte() = {4}
            _fileSystem.WriteAllBytes(file1, byteArray, append:=False)

            Dim destinationDirectory As String = CreateTempDirectory(lineNumber:=2)
            _fileSystem.CopyDirectory(
                sourceDirectoryName:=testDirectory,
                destinationDirectoryName:=destinationDirectory,
                showUI:=UIOption.OnlyErrorDialogs,
                onUserCancel:=UICancelOption.DoNothing)
            Directory.Exists(testDirectory).Should.BeTrue()
            Directory.Exists(destinationDirectory).Should.BeTrue()

            Dim count As Integer = Directory.EnumerateFiles(destinationDirectory).Count
            Directory.EnumerateFiles(testDirectory).Count.Should.Be(count)

            CleanupDirectories(testDirectory, destinationDirectory)
        End Sub

        <WinFormsFact>
        Public Sub CopyDirectoryWithShowUIProxyTest()
            Dim testDirectory As String = CreateTempDirectory(lineNumber:=1)
            Dim file1 As String = CreateTempFile(testDirectory, NameOf(file1))
            Dim byteArray As Byte() = {4}
            _fileSystem.WriteAllBytes(file1, byteArray, append:=False)

            Dim destinationDirectory As String = CreateTempDirectory(lineNumber:=2)
            _fileSystem.CopyDirectory(testDirectory, destinationDirectory, UIOption.OnlyErrorDialogs)
            Directory.Exists(testDirectory).Should.BeTrue()
            Directory.Exists(destinationDirectory).Should.BeTrue()
            Directory.EnumerateFiles(destinationDirectory).Count.Should.Be(Directory.EnumerateFiles(testDirectory).Count)

            CleanupDirectories(testDirectory, destinationDirectory)
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
            bytes.Length.Should.Be(1)
            bytes(0).Should.Be(4)

            CleanupDirectories(testDirectory)
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
            bytes.Length.Should.Be(1)
            bytes(0).Should.Be(4)

            CleanupDirectories(testDirectory)
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
            bytes.Length.Should.Be(1)
            bytes(0).Should.Be(4)

            CleanupDirectories(testDirectory)
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
            bytes.Length.Should.Be(1)
            bytes(0).Should.Be(4)

            CleanupDirectories(testDirectory)
        End Sub

        <WinFormsFact>
        Public Sub CreateDirectoryTest()
            Dim testDirectory As String = Path.Combine(s_baseTempPath, $"A{GetUniqueText()}")
            _fileSystem.CreateDirectory(testDirectory)
            IO.Directory.Exists(testDirectory).Should.BeTrue()
            CleanupDirectories(testDirectory)
        End Sub

        <WinFormsFact>
        Public Sub CurrentDirectoryTest()
            Dim testDirectory As String = CreateTempDirectory(lineNumber:=1)
            Dim savedCurrentDirectory As String = FileIO.FileSystem.CurrentDirectory
            _fileSystem.CurrentDirectory = testDirectory
            _fileSystem.CurrentDirectory.Should.Be(testDirectory)
            _fileSystem.CurrentDirectory = savedCurrentDirectory
            _fileSystem.CurrentDirectory.Should.NotBe(testDirectory)
            CleanupDirectories(testDirectory)
        End Sub

        <WinFormsFact>
        Public Sub DeleteDirectoryRecycleWithUICancelOptionsProxyTest()
            Dim testDirectory As String = CreateTempDirectory()
            Dim file1 As String = CreateTempFile(testDirectory, NameOf(file1), size:=1)
            Dim byteArray As Byte() = {4}
            _fileSystem.WriteAllBytes(file1, byteArray, append:=False)
            File.Exists(file1).Should.BeTrue()

            _fileSystem.DeleteDirectory(testDirectory, showUI:=UIOption.OnlyErrorDialogs, RecycleOption.DeletePermanently, UICancelOption.DoNothing)
            Directory.Exists(testDirectory).Should.BeFalse()
        End Sub

        <WinFormsFact>
        Public Sub DeleteDirectoryWithUIProxyRecycleTest()
            Dim testDirectory As String = CreateTempDirectory()
            Dim file1 As String = CreateTempFile(testDirectory, NameOf(file1), size:=1)
            Dim byteArray As Byte() = {4}
            _fileSystem.WriteAllBytes(file1, byteArray, append:=False)
            File.Exists(file1).Should.BeTrue()

            _fileSystem.DeleteDirectory(
                directory:=testDirectory,
                showUI:=UIOption.OnlyErrorDialogs,
                recycle:=RecycleOption.DeletePermanently)
            Directory.Exists(testDirectory).Should.BeFalse()
        End Sub

        <WinFormsFact>
        Public Sub DeleteFileProxyTest()
            Dim testDirectory As String = CreateTempDirectory()
            Dim file1 As String = CreateTempFile(testDirectory, NameOf(file1), size:=1)
            Dim byteArray As Byte() = {4}
            _fileSystem.WriteAllBytes(file1, byteArray, append:=False)
            File.Exists(file1).Should.BeTrue()

            _fileSystem.DeleteFile(file1)
            File.Exists(file1).Should.BeFalse()

            _fileSystem.DeleteDirectory(
                directory:=testDirectory,
                onDirectoryNotEmpty:=DeleteDirectoryOption.DeleteAllContents)
        End Sub

        <WinFormsFact>
        Public Sub DeleteFileWithRecycleOptionProxyTest()
            Dim testDirectory As String = CreateTempDirectory()
            Dim file1 As String = CreateTempFile(testDirectory, NameOf(file1), size:=1)
            Dim byteArray As Byte() = {4}
            _fileSystem.WriteAllBytes(file1, byteArray, append:=False)
            File.Exists(file1).Should.BeTrue()

            _fileSystem.DeleteFile(file1, UIOption.OnlyErrorDialogs, RecycleOption.DeletePermanently)
            File.Exists(file1).Should.BeFalse()

            _fileSystem.DeleteDirectory(
                directory:=testDirectory,
                onDirectoryNotEmpty:=DeleteDirectoryOption.DeleteAllContents)
        End Sub

        <WinFormsFact>
        Public Sub DeleteFileWithUIProxyTest()
            Dim testDirectory As String = CreateTempDirectory()
            Dim file1 As String = CreateTempFile(testDirectory, NameOf(file1), size:=1)
            Dim byteArray As Byte() = {4}
            _fileSystem.WriteAllBytes(file1, byteArray, append:=False)
            File.Exists(file1).Should.BeTrue()

            _fileSystem.DeleteFile(file1, UIOption.OnlyErrorDialogs, RecycleOption.DeletePermanently, UICancelOption.DoNothing)
            File.Exists(file1).Should.BeFalse()

            CleanupDirectories(testDirectory)
        End Sub

        <WinFormsFact>
        Public Sub DirectoryExistsProxyTest()
            _fileSystem.DirectoryExists(s_baseTempPath).Should.BeTrue()
        End Sub

        <WinFormsFact>
        Public Sub DriveProxyTest()
            _fileSystem.Drives.Count.Should.Be(FileIO.FileSystem.Drives.Count)
        End Sub

        <WinFormsFact>
        Public Sub FileExistsProxyTest()
            Dim testDirectory As String = CreateTempDirectory()
            Dim file1 As String = CreateTempFile(testDirectory, NameOf(file1), size:=1)
            _fileSystem.FileExists(file1).Should.BeTrue()
            CleanupDirectories(testDirectory)
        End Sub

        <WinFormsFact>
        Public Sub GetDirectoriesProxyTest()
            Dim testDirectory As String = CreateTempDirectory()
            _fileSystem.GetDirectories(s_baseTempPath).Count.Should.BeGreaterThan(0)
            CleanupDirectories(testDirectory)
        End Sub

        <WinFormsFact>
        Public Sub GetDirectoriesWithSearchTypeProxyTest()
            Dim testDirectory As String = CreateTempDirectory()
            _fileSystem.GetDirectories(s_baseTempPath, FileIO.SearchOption.SearchAllSubDirectories, "*").Count.Should.BeGreaterThan(0)
            CleanupDirectories(testDirectory)
        End Sub

        <WinFormsFact>
        Public Sub GetDirectoryInfoProxyTest()
            Dim testDirectory As String = CreateTempDirectory()
            _fileSystem.GetDirectoryInfo(testDirectory).FullName.Should.Be(testDirectory)
            CleanupDirectories(testDirectory)
        End Sub

        <WinFormsFact>
        Public Sub GetDriveInfoProxyTest()
            Dim drive0Name As String = IO.DriveInfo.GetDrives(0).Name
            _fileSystem.GetDriveInfo(drive0Name).Name.Should.Be(drive0Name)
        End Sub

        <WinFormsFact>
        Public Sub GetFileInfoProxyTest()
            Dim testDirectory As String = CreateTempDirectory()
            Dim file1 As String = CreateTempFile(testDirectory, NameOf(file1), size:=1)
            _fileSystem.GetFileInfo(file1).Exists.Should.BeTrue()
            CleanupDirectories(testDirectory)
        End Sub

        <WinFormsFact>
        Public Sub GetFilesProxyTest()
            Dim testDirectory As String = CreateTempDirectory()
            Dim file1 As String = CreateTempFile(testDirectory, NameOf(file1), size:=1)
            _fileSystem.GetFiles(testDirectory).Count.Should.Be(1)
            CleanupDirectories(testDirectory)
        End Sub

        <WinFormsFact>
        Public Sub GetFilesWithDirectoryAndSearchOptionProxyTest()
            Dim testDirectory As String = CreateTempDirectory()
            Dim file1 As String = CreateTempFile(testDirectory, NameOf(file1), size:=1)
            _fileSystem.GetFiles(testDirectory, FileIO.SearchOption.SearchTopLevelOnly, {"*"}).Count.Should.Be(1)
            CleanupDirectories(testDirectory)
        End Sub

        <WinFormsFact>
        Public Sub GetNameProxyTest() ' As String
            Dim testDirectory As String = CreateTempDirectory()
            Dim file1 As String = CreateTempFile(testDirectory, NameOf(file1), size:=1)
            _fileSystem.GetName(file1).Should.Be(NameOf(file1))
            CleanupDirectories(testDirectory)
        End Sub

        <WinFormsFact>
        Public Sub GetParentPathProxyTest()
            Dim testDirectory As String = CreateTempDirectory()
            _fileSystem.GetParentPath(testDirectory).Should.Be(s_baseTempPath)
        End Sub

        <WinFormsFact>
        Public Sub GetTempFileNameProxyTest()
            Dim tmpFileName As String = _fileSystem.GetTempFileName
            File.Exists(tmpFileName).Should.BeTrue()
            File.Delete(tmpFileName)
        End Sub


        <WinFormsFact>
        Public Sub FileNormalizePathEmptyStringTest_Fail()
            Dim testCode As Action = Sub() FileSystemUtils.NormalizePath("")
            testCode.Should.Throw(Of ArgumentException).WithMessage(
                expectedWildcardPattern:="The path is empty. (Parameter 'path')")
        End Sub

        <WinFormsFact>
        Public Sub FileNormalizePathNullTest_Fail()
            Dim testCode As Action = Sub() FileSystemUtils.NormalizePath(Nothing)
            testCode.Should.Throw(Of ArgumentNullException).WithMessage(
                expectedWildcardPattern:="Value cannot be null. (Parameter 'path')")
        End Sub

        <WinFormsFact>
        Public Sub FileNormalizePathTest_Success()
            FileSystemUtils.NormalizePath(Path.GetTempPath).Should.Be(Path.GetTempPath.TrimEnd(Path.DirectorySeparatorChar))
        End Sub

        <WinFormsFact>
        Public Sub FindInFilesProxyTest()
            Dim testDirectory As String = CreateTempDirectory()
            Dim fileA As String = CreateTempFile(testDirectory, NameOf(fileA))
            _fileSystem.WriteAllText(fileA, "A", append:=False)
            Dim fileB As String = CreateTempFile(testDirectory, NameOf(fileB), size:=1)
            Dim fileC As String = CreateTempFile(testDirectory, NameOf(fileC))
            _fileSystem.WriteAllText(fileC, "C", append:=False)
            Dim filenames As ReadOnlyCollection(Of String) = _fileSystem.FindInFiles(testDirectory, containsText:="A", ignoreCase:=True, FileIO.SearchOption.SearchTopLevelOnly)
            filenames.Count.Should.Be(1)
            _fileSystem.CombinePath(testDirectory, NameOf(fileA)).Should.Be(filenames(0))
            filenames = _fileSystem.FindInFiles(testDirectory, containsText:="A", ignoreCase:=True, FileIO.SearchOption.SearchTopLevelOnly, "*C")
            filenames.Count.Should.Be(0)

            CleanupDirectories(testDirectory)
        End Sub

        <WinFormsFact>
        Public Sub MoveDirectoryWithNoOptionsProxyTest()
            Dim testDirectory As String = CreateTempDirectory(lineNumber:=1)
            Dim file1 As String = CreateTempFile(testDirectory, NameOf(file1))
            Dim byteArray As Byte() = {4}
            _fileSystem.WriteAllBytes(file1, byteArray, append:=False)

            Dim destinationDirectory As String = CreateTempDirectory(lineNumber:=2)
            _fileSystem.MoveDirectory(testDirectory, destinationDirectory)
            Directory.Exists(testDirectory).Should.BeFalse()
            Directory.Exists(destinationDirectory).Should.BeTrue()
            Directory.EnumerateFiles(destinationDirectory).Count.Should.Be(1)

            CleanupDirectories(destinationDirectory)
        End Sub

        <WinFormsFact>
        Public Sub MoveDirectoryWithOverwriteProxyTest()
            Dim testDirectory As String = CreateTempDirectory(lineNumber:=1)
            Dim file1 As String = CreateTempFile(testDirectory, NameOf(file1))
            Dim byteArray As Byte() = {4}
            _fileSystem.WriteAllBytes(file1, byteArray, append:=False)

            Dim destinationDirectory As String = CreateTempDirectory(lineNumber:=2)
            _fileSystem.MoveDirectory(testDirectory, destinationDirectory, overwrite:=True)
            Directory.Exists(testDirectory).Should.BeFalse()
            Directory.Exists(destinationDirectory).Should.BeTrue()
            Directory.EnumerateFiles(destinationDirectory).Count.Should.Be(1)

            CleanupDirectories(destinationDirectory)
        End Sub

        <WinFormsFact>
        Public Sub MoveDirectoryWithShowUICancelOptionsProxyTest()
            Dim testDirectory As String = CreateTempDirectory(lineNumber:=1)
            Dim file1 As String = CreateTempFile(testDirectory, NameOf(file1))
            Dim byteArray As Byte() = {4}
            _fileSystem.WriteAllBytes(file1, byteArray, append:=False)

            Dim destinationDirectory As String = CreateTempDirectory(lineNumber:=2)
            _fileSystem.MoveDirectory(testDirectory, destinationDirectory, UIOption.OnlyErrorDialogs, UICancelOption.DoNothing)
            Directory.Exists(testDirectory).Should.BeFalse()
            Directory.Exists(destinationDirectory).Should.BeTrue()
            Directory.EnumerateFiles(destinationDirectory).Count.Should.Be(1)

            CleanupDirectories(destinationDirectory)
        End Sub

        <WinFormsFact>
        Public Sub MoveDirectoryWithShowUIProxyTest()
            Dim testDirectory As String = CreateTempDirectory(lineNumber:=1)
            Dim file1 As String = CreateTempFile(testDirectory, NameOf(file1))
            Dim byteArray As Byte() = {4}
            _fileSystem.WriteAllBytes(file1, byteArray, append:=False)

            Dim destinationDirectory As String = CreateTempDirectory(lineNumber:=2)
            _fileSystem.MoveDirectory(testDirectory, destinationDirectory, UIOption.OnlyErrorDialogs)
            Directory.Exists(testDirectory).Should.BeFalse()
            Directory.Exists(destinationDirectory).Should.BeTrue()
            Directory.EnumerateFiles(destinationDirectory).Count.Should.Be(1)

            CleanupDirectories(destinationDirectory)
        End Sub

        <WinFormsFact>
        Public Sub MoveFileTest()
            Dim testDirectory As String = CreateTempDirectory()
            Dim file1 As String = CreateTempFile(testDirectory, NameOf(file1))
            Dim byteArray As Byte() = {4}
            _fileSystem.WriteAllBytes(file1, byteArray, append:=False)

            Dim file2 As String = CreateTempFile(testDirectory, NameOf(file2))
            _fileSystem.MoveFile(file1, file2)
            Dim bytes As Byte() = _fileSystem.ReadAllBytes(file2)
            File.Exists(file1).Should.BeFalse()
            File.Exists(file2).Should.BeTrue()
            bytes.Length.Should.Be(1)
            bytes(0).Should.Be(4)

            CleanupDirectories(testDirectory)
        End Sub

        <WinFormsFact>
        Public Sub MoveFileWithOverwriteTest()
            Dim testDirectory As String = CreateTempDirectory()
            Dim file1 As String = CreateTempFile(testDirectory, NameOf(file1))
            Dim byteArray As Byte() = {4}
            _fileSystem.WriteAllBytes(file1, byteArray, append:=False)

            Dim file2 As String = CreateTempFile(testDirectory, NameOf(file2))
            _fileSystem.MoveFile(file1, file2, overwrite:=True)
            Dim bytes As Byte() = _fileSystem.ReadAllBytes(file2)
            File.Exists(file1).Should.BeFalse()
            File.Exists(file2).Should.BeTrue()
            bytes.Length.Should.Be(1)
            bytes(0).Should.Be(4)

            CleanupDirectories(testDirectory)
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
            File.Exists(file1).Should.BeFalse()
            File.Exists(file2).Should.BeTrue()
            bytes.Length.Should.Be(1)
            bytes(0).Should.Be(4)

            CleanupDirectories(testDirectory)
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
            File.Exists(file1).Should.BeFalse()
            File.Exists(file2).Should.BeTrue()
            bytes.Length.Should.Be(1)
            bytes(0).Should.Be(4)

            CleanupDirectories(testDirectory)
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
                text.Should.Be("A")
            End Using

            CleanupDirectories(testDirectory)
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
                totalFields.Should.Be(7)
            End While
            totalRows.Should.Be(2)
            reader.Close()

            CleanupDirectories(testDirectory)
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
                text.Should.Be("A")
            End Using

            CleanupDirectories(testDirectory)
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
                        splitData(totalFields).Should.Be(currentField.TrimEnd(";"c))
                    End If
                    totalFields += 1
                Next
                totalFields.Should.Be(6)
                totalRows += 1
            End While
            totalRows.Should.Be(2)
            reader.Close()

            CleanupDirectories(testDirectory)
        End Sub

        <WinFormsFact>
        Public Sub OpenTextStreamParserProxyTest()
            Dim testDirectory As String = CreateTempDirectory()
            Dim fileA As String = CreateTempFile(testDirectory)
            _fileSystem.WriteAllText(fileA, "A", append:=False)
            Using fileReader As IO.StreamReader = _fileSystem.OpenTextFileReader(fileA)
                Dim text As String = fileReader.ReadLine()
                text.Should.Be("A")
            End Using

            CleanupDirectories(testDirectory)
        End Sub

        <WinFormsFact>
        Public Sub ReadAllBytesProxyTest()
            Dim testDirectory As String = CreateTempDirectory()
            Dim fileA As String = CreateTempFile(testDirectory, NameOf(fileA))
            Dim byteArray As Byte() = {4}
            _fileSystem.WriteAllBytes(fileA, byteArray, append:=False)

            Dim bytes As Byte() = _fileSystem.ReadAllBytes(fileA)
            bytes.Length.Should.Be(1)
            bytes(0).Should.Be(4)

            CleanupDirectories(testDirectory)
        End Sub

        <WinFormsFact>
        Public Sub ReadAllTextProxyTest()
            Dim testDirectory As String = CreateTempDirectory()
            Dim fileA As String = CreateTempFile(testDirectory, NameOf(fileA))
            _fileSystem.WriteAllText(fileA, "A", append:=False)
            Dim text As String = _fileSystem.ReadAllText(fileA)
            text.Should.Be("A")

            CleanupDirectories(testDirectory)
        End Sub

        <WinFormsFact>
        Public Sub ReadAllTextWithEncodingProxyTest()
            Dim testDirectory As String = CreateTempDirectory()
            Dim fileA As String = CreateTempFile(testDirectory, NameOf(fileA))
            _fileSystem.WriteAllText(fileA, "A", append:=False, Encoding.UTF8)
            Dim text As String = _fileSystem.ReadAllText(fileA, Encoding.UTF8)
            text.Should.Be("A")

            CleanupDirectories(testDirectory)
        End Sub

        <WinFormsFact>
        Public Sub RenameDirectoryTest()
            Dim testDirectory As String = CreateTempDirectory(lineNumber:=1)
            Dim file1 As String = CreateTempFile(testDirectory, NameOf(file1))
            Dim byteArray As Byte() = {4}
            _fileSystem.WriteAllBytes(file1, byteArray, append:=False)

            Dim destinationDirectory As String = CreateTempDirectory(lineNumber:=2)
            Dim destinationFileName As String = Path.GetFileName(destinationDirectory)
            IO.Directory.Delete(destinationDirectory)
            _fileSystem.RenameDirectory(testDirectory, destinationFileName)
            Directory.Exists(testDirectory).Should.BeFalse()
            Directory.Exists(destinationDirectory).Should.BeTrue()
            Directory.EnumerateFiles(destinationDirectory).Count.Should.Be(1)

            CleanupDirectories(destinationDirectory)
        End Sub

        <WinFormsFact>
        Public Sub RenameFileTest()
            Dim testDirectory As String = CreateTempDirectory()
            Dim file1 As String = CreateTempFile(testDirectory, NameOf(file1))
            Dim byteArray As Byte() = {4}
            _fileSystem.WriteAllBytes(file1, byteArray, append:=False)

            Dim file2 As String = CreateTempFile(testDirectory, NameOf(file2))
            _fileSystem.RenameFile(file1, NameOf(file2))
            Dim bytes As Byte() = _fileSystem.ReadAllBytes(file2)
            File.Exists(file1).Should.BeFalse()
            File.Exists(file2).Should.BeTrue()
            bytes.Length.Should.Be(1)
            bytes(0).Should.Be(4)

            CleanupDirectories(testDirectory)
        End Sub

        <WinFormsFact>
        Public Sub SpecialDirectoriesTest()
            _fileSystem.SpecialDirectories.AllUsersApplicationData.Should.Be(SpecialDirectories.AllUsersApplicationData)
            _fileSystem.SpecialDirectories.CurrentUserApplicationData.Should.Be(SpecialDirectories.CurrentUserApplicationData)
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
