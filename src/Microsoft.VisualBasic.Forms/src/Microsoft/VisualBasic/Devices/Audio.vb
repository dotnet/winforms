' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Option Explicit On
Option Strict On
Imports System.IO
Imports Microsoft.VisualBasic.CompilerServices.ExceptionUtils

Namespace Microsoft.VisualBasic

    ''' <summary>
    '''  Enum for three ways to play a .wav file
    ''' </summary>
    Public Enum AudioPlayMode
        ' Any changes to this enum must be reflected in ValidateAudioPlayModeEnum()
        WaitToComplete = 0 'Synchronous
        Background = 1     'Asynchronous
        BackgroundLoop = 2 'Asynchronous and looping
    End Enum

    Namespace Devices

        ''' <summary>
        '''  An object that makes it easy to play wav files
        ''' </summary>
        Public Class Audio

            ''' <summary>
            '''  Creates a new Audio object
            ''' </summary>
            Public Sub New()
            End Sub

            ''' <summary>
            '''  Plays a .wav file in background mode
            ''' </summary>
            ''' <param name="location">The name of the file</param>
            Public Sub Play(location As String)
                Play(location, AudioPlayMode.Background)
            End Sub

            ''' <summary>
            ''' Plays a .wav file in the passed in mode
            ''' </summary>
            ''' <param name="location">The name of the file</param>
            ''' <param name="playMode">
            ''' An enum value representing the mode, Background (async), 
            ''' WaitToComplete (sync) or BackgroundLoop
            ''' </param>
            Public Sub Play(location As String, playMode As AudioPlayMode)
                ValidateAudioPlayModeEnum(playMode, NameOf(playMode))
                Dim safeFilename As String = ValidateFilename(location)
                Dim sound As Media.SoundPlayer = New Media.SoundPlayer(safeFilename)
                Play(sound, playMode)
            End Sub

            ''' <summary>
            '''   Plays a Byte array representation of a .wav file in the passed in mode
            ''' </summary>
            ''' <param name="data">The array representing the .wav file</param>
            ''' <param name="playMode">The mode in which the array should be played</param>
            Public Sub Play(data() As Byte, playMode As AudioPlayMode)
                If data Is Nothing Then
                    Throw GetArgumentNullException("data")
                End If
                ValidateAudioPlayModeEnum(playMode, NameOf(playMode))

                Dim soundStream As MemoryStream = New MemoryStream(data)
                Play(soundStream, playMode)
                soundStream.Close()
            End Sub

            ''' <summary>
            '''  Plays a stream representation of a .wav file in the passed in mode
            ''' </summary>
            ''' <param name="stream">The stream representing the .wav file</param>
            ''' <param name="playMode">The mode in which the stream should be played</param>
            Public Sub Play(stream As Stream, playMode As AudioPlayMode)
                ValidateAudioPlayModeEnum(playMode, NameOf(playMode))
                If stream Is Nothing Then
                    Throw GetArgumentNullException("stream")
                End If

                Play(New Media.SoundPlayer(stream), playMode)
            End Sub

            ''' <summary>
            '''   Plays a system messageBeep sound.
            ''' </summary>
            ''' <param name="systemSound">The sound to be played</param>
            ''' <remarks>Plays the sound asynchronously</remarks>
            Public Sub PlaySystemSound(systemSound As Media.SystemSound)
                If systemSound Is Nothing Then
                    Throw GetArgumentNullException("systemSound")
                End If

                systemSound.Play()

            End Sub

            ''' <summary>
            '''  Stops the play of any playing sound
            ''' </summary>
            Public Sub [Stop]()
                Dim sound As New Media.SoundPlayer()
                InternalStop(sound)
            End Sub

            ''' <summary>
            '''  Plays the passed in SoundPlayer in the passed in mode
            ''' </summary>
            ''' <param name="sound">The SoundPlayer to play</param>
            ''' <param name="mode">The mode in which to play the sound</param>
            Private Sub Play(sound As Media.SoundPlayer, mode As AudioPlayMode)

                Debug.Assert(sound IsNot Nothing, "There's no SoundPlayer")
                Debug.Assert([Enum].IsDefined(GetType(AudioPlayMode), mode), "Enum value is out of range")

                ' Stopping the sound ensures it's safe to dispose it. This could happen when we change the value of m_Sound below
                If _sound IsNot Nothing Then
                    InternalStop(_sound)
                End If

                _sound = sound

                Select Case mode
                    Case AudioPlayMode.WaitToComplete
                        _sound.PlaySync()
                    Case AudioPlayMode.Background
                        _sound.Play()
                    Case AudioPlayMode.BackgroundLoop
                        _sound.PlayLooping()
                    Case Else
                        Debug.Fail("Unknown AudioPlayMode")
                End Select

            End Sub

            ''' <summary>
            ''' SoundPlayer.Stop requires unmanaged code permissions. This method allows us to wrap calls to SoundPlayer.Stop
            ''' with the appropriate Demand/Assert
            ''' </summary>
            ''' <param name="sound"></param>
            Private Shared Sub InternalStop(sound As Media.SoundPlayer)

                ' Stop requires unmanaged code permission. Stop demands SafeSubWindows permissions, so we don't need to do it here                     
#Disable Warning SYSLIB0003 ' Type or member is obsolete
                Call New Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityPermissionFlag.UnmanagedCode).Assert()
                Try
                    sound.Stop()
                Finally
                    System.Security.CodeAccessPermission.RevertAssert()
#Enable Warning SYSLIB0003 ' Type or member is obsolete
                End Try
            End Sub

            ''' <summary>
            '''  Gets the full name and path for the file. Throws if unable to get full name and path
            ''' </summary>
            ''' <param name="location">The filename being tested</param>
            ''' <returns>A full name and path of the file</returns>
            Private Function ValidateFilename(location As String) As String
                If String.IsNullOrEmpty(location) Then
                    Throw GetArgumentNullException("location")
                End If

                Return location
            End Function

            ''' <summary>
            ''' Validates that the value being passed as an AudioPlayMode enum is a legal value
            ''' </summary>
            ''' <param name="value"></param>
            Private Sub ValidateAudioPlayModeEnum(value As AudioPlayMode, paramName As String)
                If value < AudioPlayMode.WaitToComplete OrElse value > AudioPlayMode.BackgroundLoop Then
                    Throw New ComponentModel.InvalidEnumArgumentException(paramName, DirectCast(value, Integer), GetType(AudioPlayMode))
                End If
            End Sub

            ' Object that plays the sounds. We use a private member so we can ensure we have a reference for async plays
            Private _sound As Media.SoundPlayer

        End Class 'Audio
    End Namespace
End Namespace
