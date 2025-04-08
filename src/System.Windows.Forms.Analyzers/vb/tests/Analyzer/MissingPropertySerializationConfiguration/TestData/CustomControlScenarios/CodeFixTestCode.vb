Imports System.ComponentModel
Imports System.Drawing

Option Strict On
Option Explicit On

Namespace VisualBasicControls

    ' We are writing the fully-qualified name here to make sure, the Simplifier doesn't remove it,
    ' since this is nothing our code fix touches.
    Public Class ScalableControl
        Inherits System.Windows.Forms.Control

        Private _scaleSize As SizeF = New SizeF(3, 14)

        ''' <summary>
        '''  Sets or gets the scaled size of some foo bar thing.
        ''' </summary>
        <System.ComponentModel.Description("Sets or gets the scaled size of some foo bar thing.")>
        Public Property [|ScaledSize|] As SizeF
            Get
                Return _scaleSize
            End Get
            Set(value As SizeF)
                _scaleSize = value
            End Set
        End Property

        Public Property [|ScaleFactor|] As Single = 1.0F

        ''' <summary>
        '''  Sets or gets the scaled location of some foo bar thing.
        ''' </summary>
        Public Property [|ScaledLocation|] As PointF
    End Class

End Namespace
