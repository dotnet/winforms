' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Namespace Microsoft.VisualBasic.Devices

    ''' <summary>
    '''  A wrapper object that acts as a discovery mechanism to quickly find out
    '''  the current local time of the machine and the GMT time.
    ''' </summary>
    Public Class Clock

#Disable Warning IDE0049 ' Use language keywords instead of framework type names for type references, Justification:=<Public API>

        ''' <inheritdoc cref="DateTime.Now"/>
        Public ReadOnly Property LocalTime() As DateTime
            Get
                Return DateTime.Now
            End Get
        End Property

        ''' <inheritdoc cref="DateTime.UtcNow"/>
        Public ReadOnly Property GmtTime() As DateTime
            Get
                Return DateTime.UtcNow
            End Get
        End Property

#Enable Warning IDE0049

        ''' <inheritdoc cref="Environment.TickCount"/>
        Public ReadOnly Property TickCount() As Integer
            Get
                Return Environment.TickCount
            End Get
        End Property

    End Class
End Namespace
