' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.ComponentModel
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
        ''' <value>
        '''  A String containing the path to the directory your application can use to store data for all users.
        ''' </value>
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
        '''  Returns the directory that serves as a common repository for data files
        '''  from your application used only by the current user.
        ''' </summary>
        ''' <value>
        '''  A String containing the path to the directory your application can use to store data for the current user.
        ''' </value>
        ''' <remarks>
        '''  If a path does not exist, one is created in the following format
        '''  C:\Documents and Settings\[UserName]\Application Data\[CompanyName]\[ProductName]\[ProductVersion]
        '''
        '''  We choose to use System.Windows.Forms.Application.* instead of System.Environment.GetFolderPath(*)
        '''  since the second function will only return the C:\Documents and Settings\[UserName]\Application Data.\
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
        '''  Return the current user's Desktop directory.
        ''' </summary>
        ''' <value>A String containing the path to the current user's Desktop directory.</value>
        ''' <remarks>This directory is C:\Document and Settings\[UserName]\Desktop.</remarks>
        Public ReadOnly Property Desktop() As String
            Get
                Return SpecialDirectories.Desktop
            End Get
        End Property

        ''' <summary>
        '''  Return the directory that serves as a common repository for user's personal documents.
        ''' </summary>
        ''' <value>A String containing the path to the user's personal documents.</value>
        ''' <exception cref="IO.DirectoryNotFoundException">
        '''  If the system does not have the notion of My Documents directory.
        ''' </exception>
        ''' <remarks>This directory is usually: C:\Documents and Settings\[UserName]\My Documents.</remarks>
        Public ReadOnly Property MyDocuments() As String
            Get
                Return SpecialDirectories.MyDocuments
            End Get
        End Property

        ''' <summary>
        '''  Return the "My Music" directory.
        ''' </summary>
        ''' <value>A String containing the path to the user's "My Music" directory.</value>
        ''' <exception cref="IO.DirectoryNotFoundException">
        '''  If the system does not have the notion of My Music directory.
        ''' </exception>
        ''' <remarks>This directory is C:\Documents and Settings\[UserName]\My Music</remarks>
        Public ReadOnly Property MyMusic() As String
            Get
                Return SpecialDirectories.MyMusic
            End Get
        End Property

        ''' <summary>
        '''  Return the "My Pictures" directory.
        ''' </summary>
        ''' <value>A String containing the path to the user's "My Pictures" directory.</value>
        ''' <exception cref="IO.DirectoryNotFoundException">
        '''  If the system does not have the notion of My Pictures directory.
        ''' </exception>
        ''' <remarks>This directory is C:\Documents and Settings\[UserName]\My Pictures.</remarks>
        Public ReadOnly Property MyPictures() As String
            Get
                Return SpecialDirectories.MyPictures
            End Get
        End Property

        ''' <summary>
        '''  Return the program files directory.
        ''' </summary>
        ''' <value>A String containing the path to the default program directories.</value>
        ''' <remarks>This directory is C:\Program Files.</remarks>
        Public ReadOnly Property ProgramFiles() As String
            Get
                Return SpecialDirectories.ProgramFiles
            End Get
        End Property

        ''' <summary>
        '''  Returns the directory used to store program shortcuts from Start Menu for current user.
        ''' </summary>
        ''' <value>A String containing the path to the Start Menu \ Programs directory.</value>
        ''' <remarks>This directory is C:\Document and Settings\[UserName]\Start Menu\Programs.</remarks>
        Public ReadOnly Property Programs() As String
            Get
                Return SpecialDirectories.Programs
            End Get
        End Property

        ''' <summary>
        '''  Return the directory that contain temporary files for the current user.
        ''' </summary>
        ''' <value>A String containing the path to the temporary directory for the current user.</value>
        ''' <remarks>
        '''  According to Win32 API document, GetTempPath should always return a value even if TEMP and TMP = "".
        '''  Also, this is not updated if TEMP or TMP is changed in Windows. The reason is
        '''  each process has its own copy of the environment variables and this copy is not updated.
        ''' </remarks>
        Public ReadOnly Property Temp() As String
            Get
                Return SpecialDirectories.Temp
            End Get
        End Property

    End Class
End Namespace
