' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Option Strict On
Option Explicit On
Option Infer On

Imports System.ComponentModel
Imports Microsoft.VisualBasic.CompilerServices.Utils

Namespace Microsoft.VisualBasic.ApplicationServices

    ''' <summary>
    ''' Exception for when we launch a single-instance application and it can't connect with the 
    ''' original instance.
    ''' </summary>
    <EditorBrowsable(EditorBrowsableState.Never)>
    <Serializable()>
    Public Class CantStartSingleInstanceException : Inherits Exception

        ''' <summary>
        '''  Creates a new exception
        ''' </summary>
        Public Sub New()
            MyBase.New(GetResourceString(SR.AppModel_SingleInstanceCantConnect))
        End Sub

        Public Sub New(ByVal message As String)
            MyBase.New(message)
        End Sub

        Public Sub New(ByVal message As String, ByVal inner As Exception)
            MyBase.New(message, inner)
        End Sub

        ' Deserialization constructor must be defined since we are serializable
        <EditorBrowsable(EditorBrowsableState.Advanced)>
        <Obsolete("Type or member is obsolete", DiagnosticId:="SYSLIB0051")>
        Protected Sub New(ByVal info As Runtime.Serialization.SerializationInfo, ByVal context As Runtime.Serialization.StreamingContext)
            MyBase.New(info, context)
        End Sub
    End Class
End Namespace
