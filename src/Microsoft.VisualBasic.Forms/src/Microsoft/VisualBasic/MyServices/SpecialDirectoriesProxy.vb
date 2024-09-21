' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.ComponentModel
Imports System.IO
Imports System.Security.Permissions
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
        ''' <value><see cref="SpecialDirectories.AllUsersApplicationData"/></value>
        ''' <exception cref="DirectoryNotFoundException">
        '''  The path is empty, usually because the operating system does not support the directory.
        ''' </exception>
        ''' <remarks>
        '''  If a path does not exist, one is created in the following format
        '''  C:\Documents and Settings\All Users\Application Data\[CompanyName]\[ProductName]\[ProductVersion]
        '''
        '''  See above for reason why we don't use System.Environment.GetFolderPath(*).
        ''' </remarks>
        Public ReadOnly Property AllUsersApplicationData() As String
            Get
                Return SpecialDirectories.AllUsersApplicationData
            End Get
        End Property

        ''' <summary>
        '''  The path to the Application Data directory for the current user.
        ''' </summary>
        ''' <value><see cref="SpecialDirectories.CurrentUserApplicationData"/></value>
        ''' <exception cref="DirectoryNotFoundException">
        '''  The path is empty, usually because the operating system does not support the directory.
        ''' </exception>
        ''' <remarks>
        '''  If a path does not exist, one is created in the following format
        '''  C:\Documents and Settings\[UserName]\Application Data\[CompanyName]\[ProductName]\[ProductVersion]
        '''
        '''  We choose to use System.Windows.Forms.Application.* instead of System.Environment.GetFolderPath(*)
        '''  since the second function will only return the C:\Documents and Settings\[UserName]\Application Data\
        '''  The first function separates applications by CompanyName, ProductName, ProductVersion.
        '''  The only catch is that CompanyName, ProductName has to be specified in the AssemblyInfo.vb file,
        '''  otherwise the name of the assembly will be used instead (which still has a level of separation).
        '''
        '''  Also, we chose to use UserAppDataPath instead of LocalUserAppDataPath since this directory
        '''  will work with Roaming User as well.
        ''' </remarks>
        Public ReadOnly Property CurrentUserApplicationData() As String
            Get
                Return SpecialDirectories.CurrentUserApplicationData
            End Get
        End Property

        ''' <summary>
        '''  The path to the Desktop directory.
        ''' </summary>
        ''' <value><see cref="SpecialDirectories.Desktop"/></value>
        ''' <remarks>This directory is C:\Users\[UserName]\Desktop.</remarks>
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
        ''' <value><see cref="SpecialDirectories.MyDocuments"/></value>
        ''' <remarks>This directory is usually: C:\Documents and Settings\[UserName]\My Documents.</remarks>
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
        ''' <value><see cref="SpecialDirectories.MyMusic"/></value>
        ''' <remarks>This directory is C:\Documents and Settings\[UserName]\My Music</remarks>
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
        ''' <value><see cref="SpecialDirectories.MyPictures"/></value>
        ''' <remarks>This directory is C:\Documents and Settings\[UserName]\My Pictures.</remarks>
        ''' <exception cref="DirectoryNotFoundException">
        '''  The path is empty, usually because the operating system does not support the directory.
        ''' </exception>
        Public ReadOnly Property MyPictures() As String
            Get
                Return SpecialDirectories.MyPictures
            End Get
        End Property

        ''' <summary>
        '''
        ''' </summary>
        ''' <value><see cref="SpecialDirectories.ProgramFiles"/></value>
        ''' <remarks>This directory is C:\Program Files.</remarks>
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
        ''' <value><see cref="SpecialDirectories.Programs"/></value>
        ''' <remarks>This directory is C:\Document and Settings\[UserName]\Start Menu\Programs.</remarks>
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
        ''' <value><see cref="SpecialDirectories.Temp"/></value>
        ''' <remarks>
        '''  According to Win32 API document, GetTempPath should always return a value even if TEMP and TMP = "".
        '''  Also, this is not updated if TEMP or TMP is changed in Windows. The reason is
        '''  each process has its own copy of the environment variables and this copy is not updated.
        ''' </remarks>
        ''' <exception cref="DirectoryNotFoundException">
        '''  The path is empty, usually because the operating system does not support the directory.
        ''' </exception>
        Public ReadOnly Property Temp() As String
            Get
                Return SpecialDirectories.Temp
            End Get
        End Property

    End Class
End Namespace
