' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Option Strict On
Option Explicit On

Imports System.Drawing

Namespace VisualBasicControls

    Public Module Program
        Public Sub Main(args As String())
            Dim control As New ScalableControl()

            control.ScaleFactor = 1.5F
            control.ScaledSize = New SizeF(100, 100)
            control.ScaledLocation = New PointF(10, 10)
        End Sub
    End Module

End Namespace
