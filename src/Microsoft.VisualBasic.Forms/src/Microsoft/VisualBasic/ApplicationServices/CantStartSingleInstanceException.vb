' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.ComponentModel

Imports VbUtils = Microsoft.VisualBasic.CompilerServices.ExceptionUtils

Namespace Microsoft.VisualBasic.ApplicationServices

    ''' <summary>
    '''  Exception for when we launch a single-instance application and it can't
    '''  connect with the original instance.
    ''' </summary>
    <EditorBrowsable(EditorBrowsableState.Never)>
    <Serializable()>
    Public Class CantStartSingleInstanceException : Inherits Exception

        ' Deserialization constructor must be defined since we are serializable
        <EditorBrowsable(EditorBrowsableState.Advanced)>
        <Obsolete("Type or member is obsolete", DiagnosticId:="SYSLIB0051")>
        Protected Sub New(info As Runtime.Serialization.SerializationInfo, context As Runtime.Serialization.StreamingContext)
            MyBase.New(info, context)
        End Sub

        ''' <summary>
        '''  Creates a new <see cref="Exception"/>.
        ''' </summary>
        Public Sub New()
            MyBase.New(VbUtils.GetResourceString(SR.AppModel_SingleInstanceCantConnect))
        End Sub

        ''' <summary>
        '''  Creates a new <see cref="Exception"/>.
        ''' </summary>
        ''' <inheritdoc cref="Exception.New(String)"/>
        Public Sub New(message As String)
            MyBase.New(message)
        End Sub

        ''' <summary>
        '''  Creates a new <see cref="Exception"/>.
        ''' </summary>
        ''' <inheritdoc cref="Exception.New(String, Exception)"/>
        Public Sub New(message As String, inner As Exception)
            MyBase.New(message, inner)
        End Sub

    End Class
End Namespace
