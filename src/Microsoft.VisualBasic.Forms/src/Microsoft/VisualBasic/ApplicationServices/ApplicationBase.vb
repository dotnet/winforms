' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Threading

Imports VbUtils = Microsoft.VisualBasic.CompilerServices.ExceptionUtils

Namespace Microsoft.VisualBasic.ApplicationServices

    ''' <summary>
    '''  Abstract class that defines the application Startup/Shutdown model for VB
    '''  Windows Applications such as console, WinForms, dll, service.
    ''' </summary>
    Public Class ApplicationBase

        ' The executing application (the EntryAssembly)
        Private _info As AssemblyInfo

        'Lazy-initialized and cached log object.
        Private _log As Logging.Log

        Public Sub New()
        End Sub

        ''' <summary>
        '''  Gets the information about the current culture used by the current thread.
        ''' </summary>
        Public ReadOnly Property Culture() As Globalization.CultureInfo
            Get
                Return Thread.CurrentThread.CurrentCulture
            End Get
        End Property

        ''' <summary>
        '''  Returns the info about the application. If we are executing in a DLL, we still return the info
        '''  about the application, not the DLL.
        ''' </summary>
        Public ReadOnly Property Info() As AssemblyInfo
            Get
                If _info Is Nothing Then
                    Dim assembly As Reflection.Assembly = If(Reflection.Assembly.GetEntryAssembly(), Reflection.Assembly.GetCallingAssembly())
                    _info = New AssemblyInfo(assembly)
                End If
                Return _info
            End Get
        End Property

        ''' <summary>
        '''  Provides access to logging capability.
        ''' </summary>
        ''' <value>
        '''  Returns a <see cref="Logging.Log"/> object used for logging
        '''  to OS log, debug window and a delimited text file or xml log.
        ''' </value>
        Public ReadOnly Property Log() As Logging.Log
            Get
                If _log Is Nothing Then
                    _log = New Logging.Log
                End If
                Return _log
            End Get
        End Property

        ''' <summary>
        '''  Gets the information about the current culture used by the Resource
        '''  Manager to look up culture-specific resource at run time.
        ''' </summary>
        ''' <value>
        '''  The <see cref="Globalization.CultureInfo"/> object that represents the culture used by the
        '''  Resource Manager to look up culture-specific resources at run time.
        ''' </value>
        Public ReadOnly Property UICulture() As Globalization.CultureInfo
            Get
                Return Thread.CurrentThread.CurrentUICulture
            End Get
        End Property

        ''' <summary>
        '''  Changes the culture currently in used by the current thread.
        ''' </summary>
        ''' <remarks>
        '''  <see cref="Globalization.CultureInfo"/> constructor will throw exceptions if cultureName is Nothing
        '''  or an invalid CultureInfo ID. We are not catching those exceptions.
        ''' </remarks>
        Public Sub ChangeCulture(cultureName As String)
            Thread.CurrentThread.CurrentCulture = New Globalization.CultureInfo(cultureName)
        End Sub

        ''' <summary>
        '''  Changes the culture currently used by the Resource Manager to look
        '''  up culture-specific resource at runtime.
        ''' </summary>
        ''' <remarks>
        '''  <see cref="Globalization.CultureInfo"/> constructor will throw exceptions if cultureName is Nothing
        '''  or an invalid CultureInfo ID. We are not catching those exceptions.
        ''' </remarks>
        Public Sub ChangeUICulture(cultureName As String)
            Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(cultureName)
        End Sub

        ''' <summary>
        '''  Returns the value of the specified <see cref="Environment"/> variable.
        ''' </summary>
        ''' <param name="Name">A String containing the name of the environment variable.</param>
        ''' <returns>A string containing the value of the environment variable.</returns>
        ''' <exception cref="ArgumentNullException">if name is Nothing.</exception>
        ''' <exception cref="ArgumentException">if the specified environment variable does not exist.</exception>
        Public Function GetEnvironmentVariable(name As String) As String

            ' Framework returns Null if not found.
            Dim variableValue As String = Environment.GetEnvironmentVariable(name)

            ' Since the explicitly requested a specific environment variable and we couldn't find it, throw
            If variableValue Is Nothing Then
                Throw VbUtils.GetArgumentExceptionWithArgName(
                    NameOf(name),
                    SR.EnvVarNotFound_Name,
                    name)
            End If

            Return variableValue
        End Function

    End Class
End Namespace
