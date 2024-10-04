' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.ComponentModel
Imports System.IO
Imports Microsoft.VisualBasic.FileIO

Namespace Microsoft.VisualBasic.MyServices

    ''' <summary>
    '''  This class contains properties that will return the Special Directories
    '''  specific to the current user (My Documents, My Music ...) and those specific
    '''  to the current Application that a developer expects to be able to find quickly.
    ''' </summary>
    <EditorBrowsable(EditorBrowsableState.Never)>
    Public Class SpecialDirectoriesProxy

        ''' <summary>
        '''  Proxy class can only created by internal classes.
        ''' </summary>
        Friend Sub New()
        End Sub

        ''' <summary>
        '''  Returns the directory that serves as a common repository for data files
        '''  from your application used by all users.
        ''' </summary>
        ''' <value><see langword="String"/></value>
        ''' <exception cref="DirectoryNotFoundException">
        '''  The path is empty, usually because the operating system does not support the directory.
        ''' </exception>
        ''' <remarks>
        '''  If a path does not exist, one is created.
        '''  For additional details
        '''  <see href=" https://learn.microsoft.com/windows/win32/shell/knownfolderid?redirectedfrom=MSDN"/>.
        ''' </remarks>
        Public ReadOnly Property AllUsersApplicationData() As String
            Get
                Return SpecialDirectories.AllUsersApplicationData
            End Get
        End Property

        ''' <summary>
        '''  The path to the Application Data directory for the current user.
        ''' </summary>
        ''' <value><see langword="String"/></value>
        ''' <remarks>
        '''  If a path does not exist, one is created.
        '''  For additional details
        '''  <see href=" https://learn.microsoft.com/windows/win32/shell/knownfolderid?redirectedfrom=MSDN"/>.
        ''' </remarks>
        ''' <exception cref="DirectoryNotFoundException">
        '''  The path is empty, usually because the operating system does not support the directory.
        ''' </exception>
        Public ReadOnly Property CurrentUserApplicationData() As String
            Get
                Return SpecialDirectories.CurrentUserApplicationData
            End Get
        End Property

        ''' <summary>
        '''  The path to the Desktop directory.
        ''' </summary>
        ''' <value><see langword="String"/></value>
        ''' <remarks>
        '''  If a path does not exist, one is created.
        '''  For additional details
        '''  <see href=" https://learn.microsoft.com/windows/win32/shell/knownfolderid?redirectedfrom=MSDN"/>.
        ''' </remarks>
        ''' <exception cref="DirectoryNotFoundException">
        '''  The path is empty, usually because the operating system does not support the directory.
        ''' </exception>
        Public ReadOnly Property Desktop() As String
            Get
                Return SpecialDirectories.Desktop
            End Get
        End Property

        ''' <summary>
        '''  The path to the My Documents directory
        ''' </summary>
        ''' <value><see langword="String"/></value>
        ''' <remarks>
        '''  If a path does not exist, one is created.
        '''  For additional details
        '''  <see href=" https://learn.microsoft.com/windows/win32/shell/knownfolderid?redirectedfrom=MSDN"/>.
        ''' </remarks>
        ''' <exception cref="DirectoryNotFoundException">
        '''  The path is empty, usually because the operating system does not support the directory.
        ''' </exception>
        Public ReadOnly Property MyDocuments() As String
            Get
                Return SpecialDirectories.MyDocuments
            End Get
        End Property

        ''' <summary>
        '''  The path to the My Music directory.
        ''' </summary>
        ''' <value><see langword="String"/></value>
        ''' <remarks>
        '''  If a path does not exist, one is created.
        '''  For additional details
        '''  <see href=" https://learn.microsoft.com/windows/win32/shell/knownfolderid?redirectedfrom=MSDN"/>.
        ''' </remarks>
        ''' <exception cref="DirectoryNotFoundException">
        '''  The path is empty, usually because the operating system does not support the directory.
        ''' </exception>
        Public ReadOnly Property MyMusic() As String
            Get
                Return SpecialDirectories.MyMusic
            End Get
        End Property

        ''' <summary>
        '''  The path to the My Pictures directory.
        ''' </summary>
        ''' <value><see langword="String"/></value>
        ''' <remarks>
        '''  If a path does not exist, one is created.
        '''  For additional details
        '''  <see href=" https://learn.microsoft.com/windows/win32/shell/knownfolderid?redirectedfrom=MSDN"/>.
        ''' </remarks>
        ''' <exception cref="DirectoryNotFoundException">
        '''  The path is empty, usually because the operating system does not support the directory.
        ''' </exception>
        Public ReadOnly Property MyPictures() As String
            Get
                Return SpecialDirectories.MyPictures
            End Get
        End Property

        ''' <summary>
        '''  The path to the ProgramFiles directory.
        ''' </summary>
        ''' <value><see langword="String"/></value>
        ''' <remarks>
        '''  If a path does not exist, one is created.
        '''  For additional details
        '''  <see href=" https://learn.microsoft.com/windows/win32/shell/knownfolderid?redirectedfrom=MSDN"/>.
        ''' </remarks>
        ''' <exception cref="DirectoryNotFoundException">
        '''  The path is empty, usually because the operating system does not support the directory.
        ''' </exception>
        Public ReadOnly Property ProgramFiles() As String
            Get
                Return SpecialDirectories.ProgramFiles
            End Get
        End Property

        ''' <summary>
        '''  The path to the Programs directory.
        ''' </summary>
        ''' <value><see langword="String"/></value>
        ''' <remarks>
        '''  If a path does not exist, one is created.
        '''  For additional details
        '''  <see href=" https://learn.microsoft.com/windows/win32/shell/knownfolderid?redirectedfrom=MSDN"/>.
        ''' </remarks>
        ''' <exception cref="DirectoryNotFoundException">
        '''  The path is empty, usually because the operating system does not support the directory.
        ''' </exception>
        Public ReadOnly Property Programs() As String
            Get
                Return SpecialDirectories.Programs
            End Get
        End Property

        ''' <summary>
        '''  The path to the Temp directory.
        ''' </summary>
        ''' <value><see langword="String"/></value>
        ''' <remarks>This property will always return a valid value.</remarks>
        Public ReadOnly Property Temp() As String
            Get
                Return SpecialDirectories.Temp
            End Get
        End Property

    End Class
End Namespace
