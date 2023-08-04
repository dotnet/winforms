﻿' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Option Strict On
Option Explicit On
Option Infer On

Imports System.ComponentModel
Imports Microsoft.VisualBasic.CompilerServices.Utils

Namespace Microsoft.VisualBasic.ApplicationServices

    ''' <summary>
    ''' Exception for when the WinForms VB application model isn't supplied with a startup form
    ''' </summary>
    <EditorBrowsable(EditorBrowsableState.Never)>
    <Serializable()>
    Public Class NoStartupFormException : Inherits Exception

        ''' <summary>
        '''  Creates a new exception
        ''' </summary>
        Public Sub New()
            MyBase.New(GetResourceString(SR.AppModel_NoStartupForm))
        End Sub

        Public Sub New(message As String)
            MyBase.New(message)
        End Sub

        Public Sub New(message As String, inner As Exception)
            MyBase.New(message, inner)
        End Sub

        ' De-serialization constructor must be defined since we are serializable
        <EditorBrowsable(EditorBrowsableState.Advanced)>
        <Obsolete("Type or member obsolete.", DiagnosticId:="SYSLIB0051")>
        Protected Sub New(info As Runtime.Serialization.SerializationInfo, context As Runtime.Serialization.StreamingContext)
            MyBase.New(info, context)
        End Sub
    End Class
End Namespace
