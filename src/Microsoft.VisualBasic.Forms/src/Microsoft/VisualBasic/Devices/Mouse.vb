' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Windows.Forms

Imports VbUtils = Microsoft.VisualBasic.CompilerServices.ExceptionUtils

Namespace Microsoft.VisualBasic.Devices

    ''' <summary>
    '''  A wrapper object that acts as a discovery mechanism for finding
    '''  information about the mouse on your computer such as whether the mouse
    '''  exists, the number of buttons, WheelScrolls details.
    '''
    '''  This class is a Singleton Class. See Common.Computer for details.
    ''' </summary>
    Public Class Mouse

        ''' <summary>
        '''  Gets a value indicating whether the functions of the left and right
        '''  mouses buttons have been swapped.
        ''' </summary>
        ''' <value>
        '''  true if the functions of the left and right mouse buttons are swapped. false otherwise.
        ''' </value>
        ''' <exception cref="InvalidOperationException">If no mouse is installed.</exception>
        Public ReadOnly Property ButtonsSwapped() As Boolean
            Get
                If SystemInformation.MousePresent Then
                    Return SystemInformation.MouseButtonsSwapped
                Else
                    Throw VbUtils.GetInvalidOperationException(SR.Mouse_NoMouseIsPresent)
                End If
            End Get
        End Property

        ''' <summary>
        '''  Gets a value indicating whether a mouse with a mouse wheel is installed
        ''' </summary>
        ''' <value><see langword="True"/> if a mouse with a mouse wheel is installed, false otherwise.</value>
        ''' <exception cref="InvalidOperationException">If no mouse is installed.</exception>
        Public ReadOnly Property WheelExists() As Boolean
            Get
                If SystemInformation.MousePresent Then
                    Return SystemInformation.MouseWheelPresent
                Else
                    Throw VbUtils.GetInvalidOperationException(SR.Mouse_NoMouseIsPresent)
                End If
            End Get
        End Property

        ''' <summary>
        '''  Gets the number of lines to scroll when the mouse wheel is rotated.
        ''' </summary>
        ''' <value>The number of lines to scroll.</value>
        ''' <exception cref="InvalidOperationException">if no mouse is installed or no wheels exists.</exception>
        Public ReadOnly Property WheelScrollLines() As Integer
            Get
                If WheelExists Then
                    Return SystemInformation.MouseWheelScrollLines
                Else
                    Throw VbUtils.GetInvalidOperationException(SR.Mouse_NoWheelIsPresent)
                End If
            End Get
        End Property

    End Class 'Mouse
End Namespace
