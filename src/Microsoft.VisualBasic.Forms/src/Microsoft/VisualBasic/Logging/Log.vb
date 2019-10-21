' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Option Explicit On
Option Strict On

Imports System
Imports System.Collections.Generic
Imports System.Collections.Specialized
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Globalization
Imports System.Security.Principal
Imports System.Text
Imports Microsoft.VisualBasic.CompilerServices
Imports Microsoft.VisualBasic.CompilerServices.ExceptionUtils

Namespace Microsoft.VisualBasic.Logging

    ''' <summary>
    ''' Enables logging to configured TraceListeners
    ''' </summary>
    Public Class Log

        ''' <summary>
        ''' Creates a Log and the underlying TraceSource based on the platform
        ''' </summary> 
        ''' <remarks>Right now we only support Winapp as an application platform</remarks>
        Public Sub New()
            ' Set trace source for platform. Right now we only support WinApp
            m_TraceSource = New DefaultTraceSource(WINAPP_SOURCE_NAME)
            If Not m_TraceSource.HasBeenConfigured Then
                InitializeWithDefaultsSinceNoConfigExists()
            End If
            ' Make sure to flush the log when the application closes
            AddHandler System.AppDomain.CurrentDomain.ProcessExit, AddressOf CloseOnProcessExit
        End Sub

        ''' <summary>
        ''' Creates a Log and the underlying TraceSource based on the passed in name
        ''' </summary>
        ''' <param name="name">The name of the TraceSource to be created</param>
        Public Sub New(ByVal name As String)
            m_TraceSource = New DefaultTraceSource(name)
            If Not m_TraceSource.HasBeenConfigured Then
                InitializeWithDefaultsSinceNoConfigExists()
            End If
        End Sub

        ''' <summary>
        ''' Has the TraceSource fire a TraceEvent for all of its listeners
        ''' </summary>
        ''' <param name="message">The message to be logged</param>
        Public Sub WriteEntry(ByVal message As String)
            WriteEntry(message, TraceEventType.Information, TraceEventTypeToId(TraceEventType.Information))
        End Sub

        ''' <summary>
        ''' Has the TraceSource fire a TraceEvent for all of its listeners
        ''' </summary>
        ''' <param name="message">The message to be logged</param>
        ''' <param name="severity">The type of message (error, info, etc...)</param>
        Public Sub WriteEntry(ByVal message As String, ByVal severity As TraceEventType)
            WriteEntry(message, severity, TraceEventTypeToId(severity))
        End Sub

        ''' <summary>
        ''' Has the TraceSource fire a TraceEvent for all of its listeners
        ''' </summary>
        ''' <param name="message">The message to be logged</param>
        ''' <param name="severity">The type of message (error, info, etc...)</param>
        ''' <param name="id">An id for the message (used for corelation)</param>
        Public Sub WriteEntry(ByVal message As String, ByVal severity As TraceEventType, ByVal id As Integer)
            If message Is Nothing Then
                message = ""
            End If
            m_TraceSource.TraceEvent(severity, id, message)
        End Sub

        ''' <summary>
        ''' Has the TraceSource fire a TraceEvent for all listeners using information in an exception to form the message
        ''' </summary>
        ''' <param name="ex">The exception being logged</param>
        Public Sub WriteException(ByVal ex As Exception)
            WriteException(ex, TraceEventType.Error, "", TraceEventTypeToId(TraceEventType.Error))
        End Sub

        ''' <summary>
        ''' Has the TraceSource fire a TraceEvent for all listeners using information in an exception to form the message 
        ''' and appending additional info
        ''' </summary>
        ''' <param name="ex">The exception being logged</param>
        ''' <param name="severity">The type of message (error, info, etc...)</param>
        ''' <param name="additionalInfo">Extra information to append to the message</param>
        Public Sub WriteException(ByVal ex As Exception, ByVal severity As TraceEventType, ByVal additionalInfo As String)
            WriteException(ex, severity, additionalInfo, TraceEventTypeToId(severity))
        End Sub

        ''' <summary>
        ''' Has the TraceSource fire a TraceEvent for all listeners using information in an exception to form the message
        ''' and appending additional info
        ''' </summary>
        ''' <param name="ex">The exception being logged</param>
        ''' <param name="severity">The type of message (error, info, etc...)</param>
        ''' <param name="additionalInfo">Extra information to append to the message</param>
        ''' <param name="id">An id for the message (used for corelation)</param>
        Public Sub WriteException(ByVal ex As Exception, ByVal severity As TraceEventType, ByVal additionalInfo As String, ByVal id As Integer)

            If ex Is Nothing Then
                Throw GetArgumentNullException("ex")
            End If

            Dim builder As New StringBuilder()
            builder.Append(ex.Message)

            If additionalInfo <> "" Then
                builder.Append(" ")
                builder.Append(additionalInfo)
            End If

            m_TraceSource.TraceEvent(severity, id, builder.ToString())

        End Sub

        ''' <summary>
        ''' Gives access to the log's underlying TraceSource
        ''' </summary>
        ''' <value>The log's underlying TraceSource</value>
        <EditorBrowsable(EditorBrowsableState.Advanced)>
        Public ReadOnly Property TraceSource() As TraceSource
            Get
                Return m_TraceSource 'Note, this is a downcast from the DefaultTraceSource class we are using
            End Get
        End Property

        ''' <summary>
        ''' Returns the file log trace listener we create for the Log
        ''' </summary>
        ''' <value>The file log trace listener</value>
        Public ReadOnly Property DefaultFileLogWriter() As FileLogTraceListener
            Get
                Return CType(TraceSource.Listeners(DEFAULT_FILE_LOG_TRACE_LISTENER_NAME), FileLogTraceListener)
            End Get
        End Property

        ''' <summary>
        ''' Encapsulates a System.Diagnostics.TraceSource.  The value add is that it knows if it was initialized 
        ''' using a config file or not.
        ''' </summary>
        Friend NotInheritable Class DefaultTraceSource
            Inherits TraceSource

            ''' <summary>
            ''' TraceSource has other constructors, this is the only one we care about for this internal class
            ''' </summary>
            ''' <param name="name"></param>
            Sub New(ByVal name As String)
                MyBase.New(name)
            End Sub

            ''' <summary>
            ''' Tells us whether this TraceSource found a config file to configure itself from
            ''' </summary>
            ''' <value>True - The TraceSource was configured from a config file</value>
            Public ReadOnly Property HasBeenConfigured() As Boolean
                Get
                    ' This forces initialization of the attributes list
                    If listenerAttributes Is Nothing Then
                        listenerAttributes = Me.Attributes
                    End If
                    Return m_HasBeenInitializedFromConfigFile
                End Get
            End Property

            ''' <summary>
            ''' Overriding this function is the trick that tells us whether this trace source was configured 
            ''' from a config file or not.  It only gets called if a config file was found.
            ''' </summary>
            Protected Overrides Function GetSupportedAttributes() As String()
                m_HasBeenInitializedFromConfigFile = True
                Return MyBase.GetSupportedAttributes()
            End Function

            Private m_HasBeenInitializedFromConfigFile As Boolean 'True if we this TraceSource is initialized from a config file.  False if somebody just news one up.
            Private listenerAttributes As StringDictionary

        End Class

        ''' <summary>
        ''' When there is no config file to configure the trace source, this function is called in order to
        ''' configure the trace source according to the defaults they would have had in a default AppConfig 
        ''' </summary>
        Protected Friend Overridable Sub InitializeWithDefaultsSinceNoConfigExists()
            'By default, you get a file log listener that picks everything from level Information on up.
            m_TraceSource.Listeners.Add(New FileLogTraceListener(DEFAULT_FILE_LOG_TRACE_LISTENER_NAME))
            m_TraceSource.Switch.Level = SourceLevels.Information
        End Sub

        ''' <summary>
        ''' Make sure we flush the log on exit
        ''' </summary>
        Private Sub CloseOnProcessExit(ByVal sender As Object, ByVal e As System.EventArgs)
            RemoveHandler System.AppDomain.CurrentDomain.ProcessExit, AddressOf CloseOnProcessExit
            Me.TraceSource.Close()
        End Sub

        ''' <summary>
        ''' Adds the default id values
        ''' </summary>
        ''' <remarks>Fix FxCop violation InitializeReferenceTypeStaticFieldsInline</remarks>
        Private Shared Function InitializeIDHash() As Dictionary(Of TraceEventType, Integer)
            Dim result As New Dictionary(Of TraceEventType, Integer)(10)

            ' Populate table with the fx pre defined ids
            With result
                .Add(TraceEventType.Information, 0)
                .Add(TraceEventType.Warning, 1)
                .Add(TraceEventType.Error, 2)
                .Add(TraceEventType.Critical, 3)
                .Add(TraceEventType.Start, 4)
                .Add(TraceEventType.Stop, 5)
                .Add(TraceEventType.Suspend, 6)
                .Add(TraceEventType.Resume, 7)
                .Add(TraceEventType.Verbose, 8)
                .Add(TraceEventType.Transfer, 9)
            End With

            Return result
        End Function

        ''' <summary>
        ''' Converts a TraceEventType to an Id
        ''' </summary>
        ''' <param name="traceEventValue"></param>
        ''' <returns>The Id</returns>
        Private Function TraceEventTypeToId(ByVal traceEventValue As TraceEventType) As Integer
            If m_IdHash.ContainsKey(traceEventValue) Then
                Return m_IdHash(traceEventValue)
            End If

            Return 0
        End Function

        ' The underlying TraceSource for the log
        Private m_TraceSource As DefaultTraceSource

        ' A table of default id values
        Private Shared m_IdHash As Dictionary(Of TraceEventType, Integer) = InitializeIDHash()

        ' Names of TraceSources
        Private Const WINAPP_SOURCE_NAME As String = "DefaultSource"
        Private Const DEFAULT_FILE_LOG_TRACE_LISTENER_NAME As String = "FileLog" 'taken from appconfig

    End Class

End Namespace
