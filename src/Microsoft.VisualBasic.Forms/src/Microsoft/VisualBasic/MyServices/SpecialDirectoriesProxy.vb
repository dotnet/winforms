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

        ''' <inheritdoc cref="SpecialDirectories.AllUsersApplicationData"/>
        Public ReadOnly Property AllUsersApplicationData() As String
            Get
                Return SpecialDirectories.AllUsersApplicationData
            End Get
        End Property

        ''' <inheritdoc cref="SpecialDirectories.CurrentUserApplicationData"/>
        Public ReadOnly Property CurrentUserApplicationData() As String
            Get
                Return SpecialDirectories.CurrentUserApplicationData
            End Get
        End Property

        ''' <inheritdoc cref="SpecialDirectories.Desktop"/>
        Public ReadOnly Property Desktop() As String
            Get
                Return SpecialDirectories.Desktop
            End Get
        End Property

        ''' <inheritdoc cref="SpecialDirectories.MyDocuments"/>
        Public ReadOnly Property MyDocuments() As String
            Get
                Return SpecialDirectories.MyDocuments
            End Get
        End Property

        ''' <inheritdoc cref="SpecialDirectories.MyMusic"/>
        Public ReadOnly Property MyMusic() As String
            Get
                Return SpecialDirectories.MyMusic
            End Get
        End Property

        ''' <inheritdoc cref="SpecialDirectories.MyPictures"/>
        Public ReadOnly Property MyPictures() As String
            Get
                Return SpecialDirectories.MyPictures
            End Get
        End Property

        ''' <inheritdoc cref="SpecialDirectories.ProgramFiles"/>
        Public ReadOnly Property ProgramFiles() As String
            Get
                Return SpecialDirectories.ProgramFiles
            End Get
        End Property

        ''' <inheritdoc cref="SpecialDirectories.Programs"/>
        Public ReadOnly Property Programs() As String
            Get
                Return SpecialDirectories.Programs
            End Get
        End Property

        ''' <inheritdoc cref="SpecialDirectories.Temp"/>
        Public ReadOnly Property Temp() As String
            Get
                Return SpecialDirectories.Temp
            End Get
        End Property

    End Class
End Namespace
