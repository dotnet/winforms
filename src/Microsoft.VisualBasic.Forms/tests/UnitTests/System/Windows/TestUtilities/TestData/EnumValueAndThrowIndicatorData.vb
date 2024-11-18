' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Namespace Microsoft.VisualBasic.Forms.Tests
    ''' <summary>
    ''' This <see langword="Class"/> holds one Enum Value of any type and in indicator is it is valid for the class.
    ''' </summary>
    ''' <typeparam name="T">Any Enum Type</typeparam>
    Public Class EnumValueAndThrowIndicatorData(Of T)
        ''' <summary>
        '''  Type T can be and <see langword="Enum"/> but Flags are not supported if they
        '''  use all bits of an <see langword="Integer"/>.
        ''' </summary>
        ''' <param name="value">Any Enum value</param>
        ''' <param name="throws">
        '''  <see langword="True"/> indicates the value is an invalid value for <see langword="Enum"/> T
        '''  and is expected to <see langword="Throw"/>.
        '''  <see langword="False"/> if value is valid for <see langword="Enum"/> T.
        ''' </param>
        Public Sub New(value As T, throws As Boolean)
            Me.Value = value
            Me.Throws = throws
        End Sub

        Public Property Throws As Boolean
        Public Property Value As T
    End Class
End Namespace
