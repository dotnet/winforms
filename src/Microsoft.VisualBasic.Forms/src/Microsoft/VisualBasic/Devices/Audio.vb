' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.IO

Imports VbUtils = Microsoft.VisualBasic.CompilerServices.ExceptionUtils

Namespace Microsoft.VisualBasic.Devices

    ''' <summary>
    '''  An object that makes it easy to play wav files.
    ''' </summary>
    Public Class Audio

        ' Object that plays the sounds. We use a private member so we can ensure we have a reference for async plays
        Private _sound As Media.SoundPlayer

        ''' <summary>
        '''  Creates a new <see cref="Audio"/> object.
        ''' </summary>
        Public Sub New()
        End Sub

        ''' <summary>
        '''  Validates that the value being passed as an <see cref="AudioPlayMode"/> enum is a legal value.
        ''' </summary>
        ''' <param name="value"></param>
        Private Shared Sub ValidateAudioPlayModeEnum(value As AudioPlayMode, paramName As String)
            If value < AudioPlayMode.WaitToComplete OrElse value > AudioPlayMode.BackgroundLoop Then
                Throw New ComponentModel.InvalidEnumArgumentException(paramName, value, GetType(AudioPlayMode))
            End If
        End Sub

        ''' <summary>
        '''  Gets the full name and path for the file.
        ''' </summary>
        ''' <param name="location">The filename being tested.</param>
        ''' <returns>A full name and path of the file.</returns>
        ''' <exception cref="ArgumentNullException">
        '''  If location is <see langword="Nothing"/> or <see cref="String.Empty"/>.
        ''' </exception>
        Private Shared Function ValidateFilename(location As String) As String
            If String.IsNullOrEmpty(location) Then
                Throw VbUtils.GetArgumentNullException(NameOf(location))
            End If

            Return location
        End Function

        ''' <summary>
        '''  Plays the passed in <see cref="Media.SoundPlayer"/> in the passed in <see cref="AudioPlayMode"/>.
        ''' </summary>
        ''' <param name="sound">The SoundPlayer to play.</param>
        ''' <param name="mode">The mode in which to play the sound.</param>
        Private Sub Play(sound As Media.SoundPlayer, mode As AudioPlayMode)

            Debug.Assert(sound IsNot Nothing, "There's no SoundPlayer")
            Debug.Assert([Enum].IsDefined(mode), "Enum value is out of range")

            ' Stopping the sound ensures it's safe to dispose it. This could happen when we change the value of _Sound below
            _sound?.Stop()

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
        '''  Stops the play of any playing sound.
        ''' </summary>
        Public Sub [Stop]()
            Dim sound As New Media.SoundPlayer()
            sound.Stop()
        End Sub

        ''' <summary>
        '''  Plays a .wav file in background mode.
        ''' </summary>
        ''' <param name="location">The name of the file.</param>
        Public Sub Play(location As String)
            Play(location, AudioPlayMode.Background)
        End Sub

        ''' <summary>
        '''  Plays a .wav file in the passed in mode.
        ''' </summary>
        ''' <param name="location">The name of the file.</param>
        ''' <param name="playMode">
        '''  An enum value representing the <see cref="AudioPlayMode"/> Background (async),
        '''  WaitToComplete (sync) or BackgroundLoop
        ''' </param>
        Public Sub Play(location As String, playMode As AudioPlayMode)
            ValidateAudioPlayModeEnum(playMode, NameOf(playMode))
            Dim safeFilename As String = ValidateFilename(location)
            Dim sound As New Media.SoundPlayer(safeFilename)
            Play(sound, playMode)
        End Sub

        ''' <summary>
        '''   Plays a Byte array representation of a .wav file in the passed in mode.
        ''' </summary>
        ''' <param name="data">The <see langword="Byte"/> array representing the .wav file.</param>
        ''' <param name="playMode">The mode in which the array should be played.</param>
        ''' <exception cref="ArgumentNullException">if data is <see langword="Nothing"/>.</exception>
        Public Sub Play(data() As Byte, playMode As AudioPlayMode)
            If data Is Nothing Then
                Throw VbUtils.GetArgumentNullException(NameOf(data))
            End If
            ValidateAudioPlayModeEnum(playMode, NameOf(playMode))

            Dim soundStream As New MemoryStream(data)
            Play(soundStream, playMode)
            soundStream.Close()
        End Sub

        ''' <summary>
        '''  Plays a <see cref="Stream"/> representation of a .wav file in the passed in <see cref="AudioPlayMode"/>.
        ''' </summary>
        ''' <param name="stream">The stream representing the .wav file.</param>
        ''' <param name="playMode">The mode in which the stream should be played.</param>
        ''' <exception cref="ArgumentNullException">if stream is <see langword="Nothing"/>.</exception>
        Public Sub Play(stream As Stream, playMode As AudioPlayMode)
            ValidateAudioPlayModeEnum(playMode, NameOf(playMode))
            If stream Is Nothing Then
                Throw VbUtils.GetArgumentNullException(NameOf(stream))
            End If

            Play(New Media.SoundPlayer(stream), playMode)
        End Sub

        ''' <summary>
        '''   Plays a system sound.
        ''' </summary>
        ''' <param name="systemSound">The sound to be played.</param>
        ''' <remarks>Plays the sound asynchronously.</remarks>
        ''' <exception cref="ArgumentNullException">if systemSound is <see langword="Nothing"/>.</exception>
        Public Sub PlaySystemSound(systemSound As Media.SystemSound)
            If systemSound Is Nothing Then
                Throw VbUtils.GetArgumentNullException(NameOf(systemSound))
            End If

            systemSound.Play()
        End Sub

    End Class
End Namespace
