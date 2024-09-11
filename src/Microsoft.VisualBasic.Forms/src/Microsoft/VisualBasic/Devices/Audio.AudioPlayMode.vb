' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Namespace Microsoft.VisualBasic

    ''' <summary>
    '''  Enum for three ways to play a .wav file.
    ''' </summary>
    Public Enum AudioPlayMode
        ' Any changes to this enum must be reflected in ValidateAudioPlayModeEnum()
        WaitToComplete = 0 'Synchronous
        Background = 1     'Asynchronous
        BackgroundLoop = 2 'Asynchronous and looping
    End Enum

End Namespace
