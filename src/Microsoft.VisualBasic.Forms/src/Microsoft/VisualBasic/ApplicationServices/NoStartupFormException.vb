' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.ComponentModel
Imports System.Runtime.Serialization

Imports VbUtils = Microsoft.VisualBasic.CompilerServices.ExceptionUtils

Namespace Microsoft.VisualBasic.ApplicationServices

    ''' <summary>
    '''  Exception for when the WinForms VB application model isn't supplied with a startup form
    ''' </summary>
    <EditorBrowsable(EditorBrowsableState.Never)>
    <Serializable()>
    Public Class NoStartupFormException : Inherits Exception

        ' De-serialization constructor must be defined since we are serializable
        <EditorBrowsable(EditorBrowsableState.Advanced)>
        <Obsolete("Type or member obsolete.", DiagnosticId:="SYSLIB0051")>
        Protected Sub New(info As SerializationInfo, context As StreamingContext)
            MyBase.New(info, context)
        End Sub

        ''' <summary>
        '''  Creates a new <see cref="NoStartupFormException"/>.
        ''' </summary>
        Public Sub New()
            MyBase.New(VbUtils.GetResourceString(SR.AppModel_NoStartupForm))
        End Sub

        ''' <summary>
        '''  Creates a new <see cref="NoStartupFormException"/>.
        ''' </summary>
        ''' <inheritdoc cref="Exception.New(String)"/>
        Public Sub New(message As String)
            MyBase.New(message)
        End Sub

        ''' <summary>
        '''  Creates a new <see cref="NoStartupFormException"/>.
        ''' </summary>
        ''' <inheritdoc cref="Exception.New(String, Exception)"/>
        Public Sub New(message As String, inner As Exception)
            MyBase.New(message, inner)
        End Sub

    End Class
End Namespace
