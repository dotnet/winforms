' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Option Explicit On
Option Strict On

Imports System.Windows.Forms
Imports Microsoft.VisualBasic.CompilerServices

Namespace Microsoft.VisualBasic.Devices

    ''' <summary>
    '''  A class representing a computer keyboard. Enables discovery of key
    '''  state information for the most common scenarios and enables SendKeys
    ''' </summary>
    Public Class Keyboard

        ''' <summary>
        ''' Sends keys to the active window as if typed as keyboard with wait = false.
        ''' </summary>
        ''' <param name="keys">A string containing the keys to be sent (typed).</param>
        Public Sub SendKeys(keys As String)
            SendKeys(keys, False)
        End Sub

        ''' <summary>
        ''' Sends keys to the active window as if typed at keyboard. This overloaded
        ''' version uses the same conventions as the VB6 SendKeys.
        ''' </summary>
        ''' <param name="keys">A string containing the keys to be sent (typed).</param>
        ''' <param name="wait">Wait for messages to be processed before returning.</param>
        Public Sub SendKeys(keys As String, wait As Boolean)
            If wait Then
                Windows.Forms.SendKeys.SendWait(keys)
            Else
                Windows.Forms.SendKeys.Send(keys)
            End If
        End Sub

        ''' <summary>
        ''' Gets the state (up or down) of the Shift key.
        ''' </summary>
        ''' <returns>True if the key is down otherwise false.</returns>
        Public ReadOnly Property ShiftKeyDown() As Boolean
            Get
                Dim Keys As Keys = Control.ModifierKeys
                Return CType(Keys And Keys.Shift, Boolean)
            End Get
        End Property

        ''' <summary>
        ''' Gets the state (up or down) of the Alt key.
        ''' </summary>
        ''' <returns>True if the key is down otherwise false.</returns>
        Public ReadOnly Property AltKeyDown() As Boolean
            Get
                Dim Keys As Keys = Control.ModifierKeys
                Return CType(Keys And Keys.Alt, Boolean)
            End Get
        End Property

        ''' <summary>
        ''' Gets the state (up or down) of the Ctrl key.
        ''' </summary>
        ''' <returns>True if the key is down otherwise false.</returns>
        Public ReadOnly Property CtrlKeyDown() As Boolean
            Get
                Dim Keys As Keys = Control.ModifierKeys
                Return CType(Keys And Keys.Control, Boolean)
            End Get
        End Property

        ''' <summary>
        ''' Gets the toggle state of the Caps Lock key.
        ''' </summary>
        ''' <returns>True if the key is down otherwise false.</returns>
        Public ReadOnly Property CapsLock() As Boolean
            Get
                'Security Note: Only the state of the Caps Lock is returned

                'The low order byte of the return value from GetKeyState is 1 if the key is
                'toggled on.
                Return CType((UnsafeNativeMethods.GetKeyState(Keys.CapsLock) And 1), Boolean)
            End Get
        End Property

        ''' <summary>
        ''' Gets the toggle state of the Num Lock key.
        ''' </summary>
        ''' <returns>True if the key is down otherwise false.</returns>
        Public ReadOnly Property NumLock() As Boolean
            Get
                'Security Note: Only the state of the Num Lock is returned

                'The low order byte of the return value from GetKeyState is 1 if the key is
                'toggled on.
                Return CType((UnsafeNativeMethods.GetKeyState(Keys.NumLock) And 1), Boolean)
            End Get
        End Property

        ''' <summary>
        ''' Gets the toggle state of the Scroll Lock key.
        ''' </summary>
        ''' <returns>True if the key is down otherwise false.</returns>
        Public ReadOnly Property ScrollLock() As Boolean
            Get
                'Security Note: Only the state of the Scroll Lock is returned

                'The low order byte of the return value from GetKeyState is 1 if the key is
                'toggled on.
                Return CType((UnsafeNativeMethods.GetKeyState(Keys.Scroll) And 1), Boolean)
            End Get
        End Property

    End Class

End Namespace
