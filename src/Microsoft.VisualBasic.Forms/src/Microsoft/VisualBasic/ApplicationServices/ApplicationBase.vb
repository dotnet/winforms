' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Option Strict On
Option Explicit On

Imports System
Imports ExUtils = Microsoft.VisualBasic.CompilerServices.ExceptionUtils

Namespace Microsoft.VisualBasic.ApplicationServices

    ''' <summary>
    ''' Abstract class that defines the application Startup/Shutdown model for VB 
    ''' Windows Applications such as console, winforms, dll, service.
    ''' </summary>
    Public Class ApplicationBase

        Public Sub New()
        End Sub

        ''' <summary>
        ''' Returns the value of the specified environment variable.
        ''' </summary>
        ''' <param name="Name">A String containing the name of the environment variable.</param>
        ''' <returns>A string containing the value of the environment variable.</returns>
        ''' <exception cref="System.ArgumentNullException">if name is Nothing.</exception>
        ''' <exception cref="System.ArgumentException">if the specified environment variable does not exist.</exception>
        Public Function GetEnvironmentVariable(ByVal name As String) As String

            ' Framework returns Null if not found.
            Dim VariableValue As String = System.Environment.GetEnvironmentVariable(name)

            ' Since the explicity requested a specific environment variable and we couldn't find it, throw
            If VariableValue Is Nothing Then
                Throw ExUtils.GetArgumentExceptionWithArgName("name", SR.EnvVarNotFound_Name, name)
            End If

            Return VariableValue
        End Function

        ''' <summary>
        ''' Provides access to logging capability.
        ''' </summary>
        ''' <value>Returns a Microsoft.VisualBasic.Windows.Log object used for logging to OS log, debug window
        ''' and a delimited text file or xml log.</value>
        ''' <remarks></remarks>
        Public ReadOnly Property Log() As Logging.Log
            Get
                If m_Log Is Nothing Then
                    m_Log = New Logging.Log
                End If
                Return m_Log
            End Get
        End Property

        ''' <summary>
        ''' Returns the info about the application.  If we are executing in a DLL, we still return the info
        ''' about the application, not the DLL.
        ''' </summary>
        Public ReadOnly Property Info() As AssemblyInfo
            Get
                If m_Info Is Nothing Then
                    Dim Assembly As System.Reflection.Assembly = System.Reflection.Assembly.GetEntryAssembly()
                    If Assembly Is Nothing Then 'It can be nothing if we are an add-in or a dll on the web
                        Assembly = System.Reflection.Assembly.GetCallingAssembly()
                    End If
                    m_Info = New AssemblyInfo(Assembly)
                End If
                Return m_Info
            End Get
        End Property

        ''' <summary>
        ''' Gets the information about the current culture used by the current thread.
        ''' </summary>
        Public ReadOnly Property Culture() As System.Globalization.CultureInfo
            Get
                Return System.Threading.Thread.CurrentThread.CurrentCulture
            End Get
        End Property

        ''' <summary>
        ''' Gets the information about the current culture used by the Resource
        ''' Manager to look up culture-specific resource at run time.
        ''' </summary>
        ''' <returns>
        ''' The CultureInfo object that represents the culture used by the 
        ''' Resource Manager to look up culture-specific resources at run time.
        ''' </returns>
        Public ReadOnly Property UICulture() As System.Globalization.CultureInfo
            Get
                Return System.Threading.Thread.CurrentThread.CurrentUICulture
            End Get
        End Property

        ''' <summary>
        ''' Changes the culture currently in used by the current thread.
        ''' </summary>
        ''' <remarks>
        ''' CultureInfo constructor will throw exceptions if cultureName is Nothing 
        ''' or an invalid CultureInfo ID. We are not catching those exceptions.
        ''' </remarks>
        Public Sub ChangeCulture(ByVal cultureName As String)
            System.Threading.Thread.CurrentThread.CurrentCulture = New System.Globalization.CultureInfo(cultureName)
        End Sub

        ''' <summary>
        ''' Changes the culture currently used by the Resource Manager to look
        ''' up culture-specific resource at runtime.
        ''' </summary>
        ''' <remarks>
        ''' CultureInfo constructor will throw exceptions if cultureName is Nothing 
        ''' or an invalid CultureInfo ID. We are not catching those exceptions.
        ''' </remarks>
        Public Sub ChangeUICulture(ByVal cultureName As String)
            System.Threading.Thread.CurrentThread.CurrentUICulture = New System.Globalization.CultureInfo(cultureName)
        End Sub

        Private m_Log As Logging.Log 'Lazy-initialized and cached log object.
        Private m_Info As AssemblyInfo ' The executing application (the EntryAssembly)
    End Class 'ApplicationBase
End Namespace
