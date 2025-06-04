' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports Microsoft.VisualBasic.MyServices

Namespace Microsoft.VisualBasic.Devices

    ''' <summary>
    '''   A RAD object representing the 'computer' that serves as a discovery
    '''   mechanism for finding principle abstractions in the system that you can
    '''   code against such as the file system, the clipboard, performance
    '''   counters, etc. It also provides functionality you would expect to see
    '''   associated with the computer such as playing sound, timers, access to
    '''   environment variables, etc. This class represent a general computer
    '''   available from a Windows Application, Web app, Dll library, etc.
    ''' </summary>
    Partial Public Class Computer : Inherits ServerComputer

        '  NOTE: The .Net design guidelines state that access to Instance members does not
        '  have to be thread-safe. Access to Shared members does have to be thread-safe.
        '  Since My.Computer creates the instance of Computer in a thread-safe way,
        '  access to the Computer will necessarily be thread-safe. There is nothing to
        '  prevent a user from passing our computer object across threads or creating
        '  their own instance and then getting into trouble. But that is completely
        '  consistent with the rest of the FX design. It is MY.* that is thread safe
        '  and leads to best practice access to these objects. If you Dim them up yourself,
        '  you are responsible for managing the threading.

        'Lazy initialized cache for the clipboard class. (proxies can be shared - they have no state)
        Private Shared s_clipboard As ClipboardProxy

        'Lazy initialized cache for the Keyboard class. SHARED because Keyboard behaves as a ReadOnly singleton class
        Private Shared s_keyboardInstance As Keyboard

        'Lazy initialized cache for the Mouse class. SHARED because Mouse behaves as a ReadOnly singleton class
        Private Shared s_mouse As Mouse

        'Lazy initialized cache for the Audio class.
        Private _audio As Audio

        ''' <summary>
        '''  Gets an Audio object which can play sound files or resources.
        ''' </summary>
        ''' <value>A <see cref="Computer.Audio"/> object.</value>
        Public ReadOnly Property Audio() As Audio
            Get
                If _audio IsNot Nothing Then Return _audio
                _audio = New Audio()
                Return _audio
            End Get
        End Property

        ''' <summary>
        '''  A thin wrapper for <see cref="System.Windows.Forms.Clipboard"/>.
        ''' </summary>
        ''' <value>An object representing the <see cref="Computer.Clipboard"/>.</value>
        Public ReadOnly Property Clipboard() As ClipboardProxy
            Get
                If s_clipboard Is Nothing Then
                    s_clipboard = New ClipboardProxy()
                End If

                Return s_clipboard
            End Get
        End Property

        ''' <summary>
        '''  This property returns the <see cref="Devices.Keyboard"/> object
        '''  representing some keyboard properties and a send keys method
        ''' </summary>
        ''' <value>An instance of the <see cref="Computer.Keyboard"/> class.</value>
        Public ReadOnly Property Keyboard() As Keyboard
            Get
                If s_keyboardInstance IsNot Nothing Then Return s_keyboardInstance
                s_keyboardInstance = New Keyboard
                Return s_keyboardInstance
            End Get
        End Property

        ''' <summary>
        '''  This property returns the Mouse object containing information about
        '''  the physical mouse installed to the machine.
        ''' </summary>
        ''' <value>An instance of the <see cref="Computer.Mouse"/> class.</value>
        Public ReadOnly Property Mouse() As Mouse
            Get
                If s_mouse IsNot Nothing Then Return s_mouse
                s_mouse = New Mouse
                Return s_mouse
            End Get
        End Property

        ''' <summary>
        '''  This property returns the primary display screen.
        ''' </summary>
        ''' <value>A <see cref="System.Windows.Forms.Screen"/> object as the primary screen.</value>
        Public ReadOnly Property Screen() As System.Windows.Forms.Screen
            Get
                'Don't cache this. The Screen class responds to display resolution changes by nulling out AllScreens, which
                'PrimaryScreen relies on to find the primary. So we always need to access the latest PrimaryScreen so we
                'will get the current resolution reported.
                Return System.Windows.Forms.Screen.PrimaryScreen
            End Get
        End Property

    End Class
End Namespace
