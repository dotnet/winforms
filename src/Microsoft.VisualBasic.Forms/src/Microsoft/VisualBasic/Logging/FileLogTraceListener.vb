' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Option Explicit On
Option Strict On

Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Globalization
Imports System.IO
Imports System.Reflection
Imports System.Security.Permissions
Imports System.Windows.Forms
Imports System.Text
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices
Imports Microsoft.VisualBasic.CompilerServices.ExceptionUtils
Imports Microsoft.VisualBasic.CompilerServices.Utils

Namespace Microsoft.VisualBasic.Logging

    ''' <summary>
    ''' Options for the location of a log's directory
    ''' </summary>
    Public Enum LogFileLocation As Integer
        ' Changes to this enum must be reflected in ValidateLogfileLocationEnumValue()
        TempDirectory
        LocalUserApplicationDirectory
        CommonApplicationDirectory
        ExecutableDirectory
        Custom
    End Enum

    ''' <summary>
    ''' Options for the date stamp in the name of a log file
    ''' </summary>
    Public Enum LogFileCreationScheduleOption As Integer
        None        '(default)
        Daily       'YYYY-MM-DD for today
        Weekly      'YYYY-MM-DD for first day of this week
    End Enum

    ''' <summary>
    ''' Options for behavior when resources are exhausted
    ''' </summary>
    Public Enum DiskSpaceExhaustedOption As Integer
        ThrowException
        DiscardMessages
    End Enum

    ''' <summary>
    ''' Class for logging to a text file
    ''' </summary>
    ''' <remarks>
    ''' TraceListener is ComVisible(False), Microsoft.VisualBasic.dll is ComVisible(True).
    ''' Therefore, mark FileLogTraceListener as ComVisible(False).
    ''' </remarks>
    <System.Runtime.InteropServices.ComVisible(False)> _
    Public Class FileLogTraceListener
        Inherits TraceListener

        ''' <summary>
        ''' Creates a FileLogTraceListener with the passed in name
        ''' </summary>
        ''' <param name="name">The name of the listener</param>
        Public Sub New(ByVal name As String)
            MyBase.New(name)
        End Sub

        ''' <summary>
        ''' Creates a FileLogTraceListener with default name
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()
            Me.New(DEFAULT_NAME)
        End Sub

        ''' <summary>
        ''' Indicates the log's directory
        ''' </summary>
        ''' <value>An enum which can indicate one of several logical locations for the log</value>
        Public Property Location() As LogFileLocation
            Get
                If Not m_PropertiesSet(LOCATION_INDEX) Then
                    If Attributes.ContainsKey(KEY_LOCATION) Then
                        Dim converter As TypeConverter = TypeDescriptor.GetConverter(GetType(LogFileLocation))
                        Me.Location = DirectCast(converter.ConvertFromInvariantString(Attributes(KEY_LOCATION)), LogFileLocation)
                    End If
                End If
                Return m_Location
            End Get
            Set(ByVal value As LogFileLocation)
                ValidateLogFileLocationEnumValue(value, "value")

                ' If the location is changing we need to close the current file
                If m_Location <> value Then
                    CloseCurrentStream()
                End If
                m_Location = value
                m_PropertiesSet(LOCATION_INDEX) = True
            End Set
        End Property

        ''' <summary>
        ''' Indicates whether or not the stream should be flushed after every write
        ''' </summary>
        ''' <value>True if the stream should be flushed after every write, otherwise False</value>
        Public Property AutoFlush() As Boolean
            Get
                If Not m_PropertiesSet(AUTOFLUSH_INDEX) Then
                    If Attributes.ContainsKey(KEY_AUTOFLUSH) Then
                        Me.AutoFlush = Convert.ToBoolean(Attributes(KEY_AUTOFLUSH), CultureInfo.InvariantCulture)
                    End If
                End If

                Return m_AutoFlush
            End Get
            Set(ByVal value As Boolean)
                DemandWritePermission()
                m_AutoFlush = value
                m_PropertiesSet(AUTOFLUSH_INDEX) = True
            End Set
        End Property

        ''' <summary>
        ''' Indicates whether or not the the host name of the logging machine should
        ''' be included in the output.
        ''' </summary>
        ''' <value>True if the HostId should be included, otherwise False</value>
        Public Property IncludeHostName() As Boolean
            Get
                If Not m_PropertiesSet(INCLUDEHOSTNAME_INDEX) Then
                    If Attributes.ContainsKey(KEY_INCLUDEHOSTNAME) Then
                        Me.IncludeHostName = Convert.ToBoolean(Attributes(KEY_INCLUDEHOSTNAME), CultureInfo.InvariantCulture)
                    End If
                End If
                Return m_IncludeHostName
            End Get
            Set(ByVal value As Boolean)
                DemandWritePermission()
                m_IncludeHostName = value
                m_PropertiesSet(INCLUDEHOSTNAME_INDEX) = True
            End Set
        End Property

        ''' <summary>
        ''' Indicates whether or not the file should be appended to or overwritten
        ''' </summary>
        ''' <value>True if the file should be appended to, otherwise False</value>
        Public Property Append() As Boolean
            Get
                If Not m_PropertiesSet(APPEND_INDEX) Then
                    If Attributes.ContainsKey(KEY_APPEND) Then
                        Me.Append = Convert.ToBoolean(Attributes(KEY_APPEND), CultureInfo.InvariantCulture)
                    End If
                End If

                Return m_Append
            End Get
            Set(ByVal value As Boolean)

                DemandWritePermission()

                ' If this property is changing, we need to close the current file
                If value <> m_Append Then
                    CloseCurrentStream()
                End If
                m_Append = value
                m_PropertiesSet(APPEND_INDEX) = True
            End Set
        End Property

        ''' <summary>
        ''' Indicates what to do when the size of the log trespasses on the MaxFileSize
        ''' or the ReserveDiskSpace set by the user
        ''' </summary>
        ''' <value>An enum indicating the desired behavior (do nothing, throw)</value>
        Public Property DiskSpaceExhaustedBehavior() As DiskSpaceExhaustedOption
            Get
                If Not m_PropertiesSet(DISKSPACEEXHAUSTEDBEHAVIOR_INDEX) Then
                    If Attributes.ContainsKey(KEY_DISKSPACEEXHAUSTEDBEHAVIOR) Then
                        Dim converter As TypeConverter = TypeDescriptor.GetConverter(GetType(DiskSpaceExhaustedOption))
                        Me.DiskSpaceExhaustedBehavior = DirectCast(converter.ConvertFromInvariantString(Attributes(KEY_DISKSPACEEXHAUSTEDBEHAVIOR)), DiskSpaceExhaustedOption)
                    End If
                End If
                Return m_DiskSpaceExhaustedBehavior
            End Get
            Set(ByVal value As DiskSpaceExhaustedOption)
                DemandWritePermission()
                ValidateDiskSpaceExhaustedOptionEnumValue(value, "value")
                m_DiskSpaceExhaustedBehavior = value
                m_PropertiesSet(DISKSPACEEXHAUSTEDBEHAVIOR_INDEX) = True
            End Set
        End Property

        ''' <summary>
        ''' The name of the log file not including DateStamp, file number, Path or extension
        ''' </summary>
        ''' <value>The name of the log file</value>
        Public Property BaseFileName() As String
            Get
                If Not m_PropertiesSet(BASEFILENAME_INDEX) Then
                    If Attributes.ContainsKey(KEY_BASEFILENAME) Then
                        Me.BaseFileName = Attributes(KEY_BASEFILENAME)
                    End If
                End If
                Return m_BaseFileName
            End Get
            Set(ByVal value As String)
                If value = "" Then
                    Throw GetArgumentNullException("value", SR.ApplicationLogBaseNameNull)
                End If

                ' Test the file name. This will throw if the name is invalid. 
                Path.GetFullPath(value)

                If String.Compare(value, m_BaseFileName, StringComparison.OrdinalIgnoreCase) <> 0 Then
                    CloseCurrentStream()
                    m_BaseFileName = value
                End If

                m_PropertiesSet(BASEFILENAME_INDEX) = True
            End Set
        End Property

        ''' <summary>
        '''  The fullname and path of the actual log file including DateStamp and file number
        ''' </summary>
        ''' <value>The full name and path</value>
        ''' <remarks>Calling this method will open the log file if it's not already open</remarks>
        Public ReadOnly Property FullLogFileName() As String
            Get
                ' The only way to reliably know the file name is to open the file. If we
                ' don't have a stream, get one (this will open the file)
                EnsureStreamIsOpen()

                ' We shouldn't use fields for demands so we use a local variable
                Dim returnPath As String = m_FullFileName
                Dim filePermission As New FileIOPermission(FileIOPermissionAccess.PathDiscovery, returnPath)
                filePermission.Demand()

                Return returnPath
            End Get
        End Property

        ''' <summary>
        ''' Indicates what Date to stamp the log file with (none, first day of week, day)
        ''' </summary>
        ''' <value>An enum indicating how to stamp the file</value>
        Public Property LogFileCreationSchedule() As LogFileCreationScheduleOption
            Get
                If Not m_PropertiesSet(LOGFILECREATIONSCHEDULE_INDEX) Then
                    If Attributes.ContainsKey(KEY_LOGFILECREATIONSCHEDULE) Then
                        Dim converter As TypeConverter = TypeDescriptor.GetConverter(GetType(LogFileCreationScheduleOption))
                        Me.LogFileCreationSchedule = DirectCast(converter.ConvertFromInvariantString(Attributes(KEY_LOGFILECREATIONSCHEDULE)), LogFileCreationScheduleOption)
                    End If
                End If
                Return m_LogFileDateStamp
            End Get
            Set(ByVal value As LogFileCreationScheduleOption)
                ValidateLogFileCreationScheduleOptionEnumValue(value, "value")

                If value <> m_LogFileDateStamp Then
                    CloseCurrentStream()
                    m_LogFileDateStamp = value
                End If

                m_PropertiesSet(LOGFILECREATIONSCHEDULE_INDEX) = True
            End Set
        End Property

        ''' <summary>
        ''' The maximum size in bytes the log file is allowed to grow to
        ''' </summary>
        ''' <value>The maximum size</value>
        Public Property MaxFileSize() As Long
            Get
                If Not m_PropertiesSet(MAXFILESIZE_INDEX) Then
                    If Attributes.ContainsKey(KEY_MAXFILESIZE) Then
                        Me.MaxFileSize = Convert.ToInt64(Attributes(KEY_MAXFILESIZE), CultureInfo.InvariantCulture)
                    End If
                End If
                Return m_MaxFileSize
            End Get
            Set(ByVal value As Long)
                DemandWritePermission()
                If value < MIN_FILE_SIZE Then
                    Throw GetArgumentExceptionWithArgName("value", SR.ApplicationLogNumberTooSmall, "MaxFileSize")
                End If
                m_MaxFileSize = value
                m_PropertiesSet(MAXFILESIZE_INDEX) = True
            End Set
        End Property

        ''' <summary>
        ''' The amount of disk space, in bytes, that must be available after a write
        ''' </summary>
        ''' <value>The reserved disk space</value>
        Public Property ReserveDiskSpace() As Long
            Get
                If Not m_PropertiesSet(RESERVEDISKSPACE_INDEX) Then
                    If Attributes.ContainsKey(KEY_RESERVEDISKSPACE) Then
                        Me.ReserveDiskSpace = Convert.ToInt64(Attributes(KEY_RESERVEDISKSPACE), CultureInfo.InvariantCulture)
                    End If
                End If
                Return m_ReserveDiskSpace
            End Get
            Set(ByVal value As Long)
                DemandWritePermission()
                If value < 0 Then
                    Throw GetArgumentExceptionWithArgName("value", SR.ApplicationLog_NegativeNumber, "ReserveDiskSpace")
                End If
                m_ReserveDiskSpace = value
                m_PropertiesSet(RESERVEDISKSPACE_INDEX) = True
            End Set
        End Property

        ''' <summary>
        ''' The delimiter to be used to delimit fields in a line of output
        ''' </summary>
        ''' <value>The delimiter</value>
        Public Property Delimiter() As String
            Get
                If Not m_PropertiesSet(DELIMITER_INDEX) Then
                    If Attributes.ContainsKey(KEY_DELIMITER) Then
                        Me.Delimiter = Attributes(KEY_DELIMITER)
                    End If
                End If
                Return m_Delimiter
            End Get
            Set(ByVal value As String)
                m_Delimiter = value
                m_PropertiesSet(DELIMITER_INDEX) = True
            End Set
        End Property

        ''' <summary>
        ''' The encoding to try when opening a file. 
        ''' </summary>
        ''' <value>The encoding</value>
        ''' <remarks>
        ''' If Append is true then this value will be trumped by the actual encoding value
        ''' of the file
        ''' </remarks>
        Public Property Encoding() As Encoding
            Get
                If Not m_PropertiesSet(ENCODING_INDEX) Then
                    If Attributes.ContainsKey(KEY_ENCODING) Then
                        Me.Encoding = System.Text.Encoding.GetEncoding(Attributes(KEY_ENCODING))
                    End If
                End If
                Return m_Encoding
            End Get
            Set(ByVal value As Encoding)
                If value Is Nothing Then
                    Throw GetArgumentNullException("value")
                End If
                m_Encoding = value
                m_PropertiesSet(ENCODING_INDEX) = True
            End Set
        End Property

        ''' <summary>
        ''' The directory to be used if Location is set to Custom
        ''' </summary>
        ''' <value>The name of the directory</value>
        ''' <remarks>This will throw if the path cannot be resolved</remarks>
        Public Property CustomLocation() As String
            Get
                If Not m_PropertiesSet(CUSTOMLOCATION_INDEX) Then
                    If Attributes.ContainsKey(KEY_CUSTOMLOCATION) Then
                        Me.CustomLocation = Attributes(KEY_CUSTOMLOCATION)
                    End If
                End If

                Dim fileName As String = Path.GetFullPath(m_CustomLocation)
                Dim filePermission As New FileIOPermission(FileIOPermissionAccess.PathDiscovery, fileName)
                filePermission.Demand()
                Return fileName
            End Get
            Set(ByVal value As String)

                ' Validate the path
                Dim tempPath As String = Path.GetFullPath(value)

                If Not Directory.Exists(tempPath) Then
                    Directory.CreateDirectory(tempPath)
                End If

                ' If we're using custom location and the value is changing we need to
                ' close the stream
                If Me.Location = LogFileLocation.Custom And String.Compare(tempPath, m_CustomLocation, StringComparison.OrdinalIgnoreCase) <> 0 Then
                    CloseCurrentStream()
                End If

                ' Since the user is setting a custom path, set Location to custom
                Me.Location = LogFileLocation.Custom

                m_CustomLocation = tempPath
                m_PropertiesSet(CUSTOMLOCATION_INDEX) = True

            End Set
        End Property

        ''' <summary>
        ''' Writes the message to the log
        ''' </summary>
        ''' <param name="message">The message to be written</param>
        Public Overloads Overrides Sub Write(ByVal message As String)

            ' Use Try block to attempt to close stream if an exception is thrown
            Try
                HandleDateChange()

                ' Check resources
                Dim NewEntrySize As Int64 = Me.Encoding.GetByteCount(message)

                If ResourcesAvailable(NewEntrySize) Then
                    ListenerStream.Write(message)
                    If Me.AutoFlush Then
                        ListenerStream.Flush()
                    End If
                End If
            Catch
                CloseCurrentStream()
                Throw
            End Try

        End Sub

        ''' <summary>
        ''' Writes the message to the log as a line
        ''' </summary>
        ''' <param name="message">The message to be written</param>
        Public Overloads Overrides Sub WriteLine(ByVal message As String)

            ' Use Try block to attempt to close stream if an exception is thrown
            Try
                HandleDateChange()

                ' Check resources
                Dim NewEntrySize As Int64 = Me.Encoding.GetByteCount(message & vbCrLf)

                If ResourcesAvailable(NewEntrySize) Then
                    ListenerStream.WriteLine(message)
                    If Me.AutoFlush Then
                        ListenerStream.Flush()
                    End If
                End If
            Catch
                CloseCurrentStream()
                Throw
            End Try
        End Sub

        ''' <summary>
        ''' Event fired by TraceSourceListener resulting in writing to the log
        ''' </summary>
        ''' <param name="eventCache">Cache of information</param>
        ''' <param name="source">The name of the TraceSourceListener</param>
        ''' <param name="eventType">The eventType of the message</param>
        ''' <param name="id">The id of the message</param>
        ''' <param name="message">The message</param>
        Public Overrides Sub TraceEvent(ByVal eventCache As TraceEventCache, ByVal source As String, ByVal eventType As TraceEventType, ByVal id As Integer, ByVal message As String)

            If Me.Filter IsNot Nothing Then
                If Not Me.Filter.ShouldTrace(eventCache, source, eventType, id, message, Nothing, Nothing, Nothing) Then
                    Return
                End If
            End If
            Dim outBuilder As New StringBuilder

            ' Add fields that always appear (source, eventType, id, message)
            ' source
            outBuilder.Append(source & Me.Delimiter)

            ' eventType
            outBuilder.Append([Enum].GetName(GetType(TraceEventType), eventType) & Me.Delimiter)

            ' id
            outBuilder.Append(id.ToString(CultureInfo.InvariantCulture) & Me.Delimiter)

            ' message
            outBuilder.Append(message)

            ' Add optional fields
            ' Callstack
            If (Me.TraceOutputOptions And TraceOptions.Callstack) = TraceOptions.Callstack Then
                outBuilder.Append(Me.Delimiter & eventCache.Callstack)
            End If

            ' LogicalOperationStack
            If (Me.TraceOutputOptions And TraceOptions.LogicalOperationStack) = TraceOptions.LogicalOperationStack Then
                outBuilder.Append(Me.Delimiter & StackToString(eventCache.LogicalOperationStack))
            End If

            ' DateTime
            If (Me.TraceOutputOptions And TraceOptions.DateTime) = TraceOptions.DateTime Then
                ' Add datetime. Time will be in GMT.
                outBuilder.Append(Me.Delimiter & eventCache.DateTime.ToString("u", CultureInfo.InvariantCulture))
            End If

            ' ProcessId
            If (Me.TraceOutputOptions And TraceOptions.ProcessId) = TraceOptions.ProcessId Then
                outBuilder.Append(Me.Delimiter & eventCache.ProcessId.ToString(CultureInfo.InvariantCulture))
            End If

            ' ThreadId
            If (Me.TraceOutputOptions And TraceOptions.ThreadId) = TraceOptions.ThreadId Then
                outBuilder.Append(Me.Delimiter & eventCache.ThreadId)
            End If

            ' Timestamp
            If (Me.TraceOutputOptions And TraceOptions.Timestamp) = TraceOptions.Timestamp Then
                outBuilder.Append(Me.Delimiter & eventCache.Timestamp.ToString(CultureInfo.InvariantCulture))
            End If

            ' HostName
            If Me.IncludeHostName Then
                outBuilder.Append(Me.Delimiter & HostName)
            End If

            WriteLine(outBuilder.ToString())

        End Sub

        ''' <summary>
        ''' Event fired by TraceSourceListener resulting in writing to the log
        ''' </summary>
        ''' <param name="eventCache">Cache of information</param>
        ''' <param name="source">The name of the TraceSourceListener</param>
        ''' <param name="eventType">The eventType of the message</param>
        ''' <param name="id">The id of the message</param>
        ''' <param name="format">A string with placeholders that serves as a format for the message</param>
        ''' <param name="args">The values for the placeholders in format</param>
        Public Overrides Sub TraceEvent(ByVal eventCache As TraceEventCache, ByVal source As String, ByVal eventType As TraceEventType, ByVal id As Integer, ByVal format As String, ByVal ParamArray args() As Object)

            ' Create the message
            Dim message As String = Nothing
            If args IsNot Nothing Then
                message = String.Format(CultureInfo.InvariantCulture, format, args)
            Else
                message = format
            End If

            TraceEvent(eventCache, source, eventType, id, message)
        End Sub

        ''' <summary>
        ''' Method of the base class we override to keep message format consistent
        ''' </summary>
        ''' <param name="eventCache">Cache of information</param>
        ''' <param name="source">The name of the TraceSourceListener</param>
        ''' <param name="eventType">The eventType of the message</param>
        ''' <param name="id">The id of the message</param>
        ''' <param name="data">An object containing the message to be logged</param>
        Public Overrides Sub TraceData(ByVal eventCache As TraceEventCache, ByVal source As String, ByVal eventType As TraceEventType, ByVal id As Integer, ByVal data As Object)

            Dim message As String = ""
            If data IsNot Nothing Then
                message = data.ToString()
            End If

            TraceEvent(eventCache, source, eventType, id, message)
        End Sub

        ''' <summary>
        ''' Method of the base class we override to keep message format consistent
        ''' </summary>
        ''' <param name="eventCache">Cache of information</param>
        ''' <param name="source">The name of the TraceSourceListener</param>
        ''' <param name="eventType">The eventType of the message</param>
        ''' <param name="id">The id of the message</param>
        ''' <param name="data">A list of objects making up the message to be logged</param>
        Public Overrides Sub TraceData(ByVal eventCache As TraceEventCache, ByVal source As String, ByVal eventType As TraceEventType, ByVal id As Integer, ByVal ParamArray data As Object())

            Dim messageBuilder As New StringBuilder()
            If data IsNot Nothing Then
                Dim bound As Integer = data.Length - 1
                For i As Integer = 0 To bound
                    messageBuilder.Append(data(i).ToString())
                    If i <> bound Then
                        messageBuilder.Append(Me.Delimiter)
                    End If
                Next i
            End If

            TraceEvent(eventCache, source, eventType, id, messageBuilder.ToString())
        End Sub

        ''' <summary>
        ''' Flushes the underlying stream
        ''' </summary>
        Public Overrides Sub Flush()
            If m_Stream IsNot Nothing Then
                m_Stream.Flush()
            End If
        End Sub

        ''' <summary>
        ''' Closes the underlying stream
        ''' </summary>
        Public Overrides Sub Close()
            Dispose(True)
        End Sub

        ''' <summary>
        ''' Gets a list of all the attributes recognized by the this listener. Trying to use an item not in this list
        ''' in a config file will cause a configuration exception
        ''' </summary>
        ''' <returns>An array of attribute names</returns>
        Protected Overrides Function GetSupportedAttributes() As String()
            Return m_SupportedAttributes
        End Function

        ''' <summary>
        ''' Makes sure stream is flushed
        ''' </summary>
        ''' <param name="disposing"></param>
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing Then
                CloseCurrentStream()
            End If
        End Sub

        ''' <summary>
        ''' Gets the log file name under the current configuration. 
        ''' </summary>
        ''' <value>The log file name</value>
        ''' <remarks>
        ''' Includes the full path and the datestamp, but does not include the
        ''' file number or the extension.
        ''' </remarks>
        Private ReadOnly Property LogFileName() As String
            Get
                Dim basePath As String

                ' Get the directory
                Select Case Me.Location
                    Case LogFileLocation.CommonApplicationDirectory
                        basePath = Application.CommonAppDataPath
                    Case LogFileLocation.ExecutableDirectory
                        basePath = Path.GetDirectoryName(Application.ExecutablePath)
                    Case LogFileLocation.LocalUserApplicationDirectory
                        basePath = Application.UserAppDataPath
                    Case LogFileLocation.TempDirectory
                        basePath = Path.GetTempPath()
                    Case LogFileLocation.Custom
                        If Me.CustomLocation = "" Then
                            basePath = Application.UserAppDataPath
                        Else
                            basePath = Me.CustomLocation
                        End If
                    Case Else
                        Debug.Fail("Unrecognized location")
                        basePath = Application.UserAppDataPath
                End Select

                ' Add the base name
                Dim fileName As String = Me.BaseFileName

                ' Add DateTime Stamp
                Select Case Me.LogFileCreationSchedule
                    Case LogFileCreationScheduleOption.Daily
                        fileName += "-" & Now.Date.ToString(DATE_FORMAT, CultureInfo.InvariantCulture)
                    Case LogFileCreationScheduleOption.Weekly
                        ' Get first day of week
                        m_FirstDayOfWeek = Now.AddDays(-Now.DayOfWeek)
                        fileName += "-" & m_FirstDayOfWeek.Date.ToString(DATE_FORMAT, CultureInfo.InvariantCulture)
                    Case LogFileCreationScheduleOption.None
                    Case Else
                        Debug.Fail("Unrecognized LogFileCreationSchedule")
                End Select

                Return Path.Combine(basePath, fileName)
            End Get
        End Property

        ''' <summary>
        ''' Gets the stream to use for writing to the log
        ''' </summary>
        ''' <value>The stream</value>
        Private ReadOnly Property ListenerStream() As ReferencedStream
            Get
                EnsureStreamIsOpen()

                Debug.Assert(m_Stream IsNot Nothing, "Unable to get stream")
                Return m_Stream
            End Get
        End Property

        ''' <summary>
        ''' Gets or creates the stream used for writing to the log
        ''' </summary>
        ''' <returns>The stream</returns>
        Private Function GetStream() As ReferencedStream

            ' Check the hash table to see if this file is already opened by another 
            ' FileLogTraceListener in the same process
            Dim i As Integer = 0
            Dim refStream As ReferencedStream = Nothing
            Dim BaseStreamName As String = Path.GetFullPath(LogFileName & FILE_EXTENSION)

            While refStream Is Nothing AndAlso i < MAX_OPEN_ATTEMPTS
                ' This should only be true if processes outside our process have
                ' MAX_OPEN_ATTEMPTS files open using the naming schema (file-1.log, file-2.log ... file-MAX_OPEN_ATTEMPTS.log)

                Dim fileName As String
                If i = 0 Then
                    fileName = Path.GetFullPath(LogFileName & FILE_EXTENSION)
                Else
                    fileName = Path.GetFullPath(LogFileName & "-" & i.ToString(CultureInfo.InvariantCulture) & FILE_EXTENSION)
                End If

                Dim caseInsensitiveKey As String = fileName.ToUpper(CultureInfo.InvariantCulture)
                SyncLock m_Streams

                    If m_Streams.ContainsKey(caseInsensitiveKey) Then
                        refStream = m_Streams(caseInsensitiveKey)
                        If Not refStream.IsInUse Then
                            ' This means that the referenced stream has somehow entered an invalid state so remove it
                            Debug.Fail("Referenced stream is in invalid state")
                            m_Streams.Remove(caseInsensitiveKey)
                            refStream = Nothing
                        Else
                            If Me.Append Then
                                ' We are handing off an already existing stream, so we need to make sure the caller has permissions to write to this stream
                                Dim filePermission As New FileIOPermission(FileIOPermissionAccess.Write, fileName)
                                filePermission.Demand()

                                refStream.AddReference()
                                m_FullFileName = fileName
                                Return refStream
                            Else
                                ' The user wants to overwrite, so we need to open a new stream
                                i += 1
                                refStream = Nothing
                                Continue While
                            End If
                        End If
                    End If

                    ' Try to open the file
                    Dim fileEncoding As Encoding = Me.Encoding
                    Try
                        If Me.Append Then
                            ' Try to get the file's actual encoding. If we get it, that trumps
                            ' the user specified value
                            fileEncoding = GetFileEncoding(fileName)
                            If fileEncoding Is Nothing Then
                                fileEncoding = Me.Encoding
                            End If
                        End If

                        Dim baseStreamWriter As New StreamWriter(fileName, Me.Append, fileEncoding)
                        refStream = New ReferencedStream(baseStreamWriter)
                        refStream.AddReference()
                        m_Streams.Add(caseInsensitiveKey, refStream)
                        m_FullFileName = fileName
                        Return refStream
                    Catch ex As IOException
                    End Try

                    i += 1
                End SyncLock
            End While
            'If we fall out the loop, we have failed to obtain a valid stream name.  This occurs if there are files on your system
            'ranging from  BaseStreamName0..BaseStreamName{integer.MaxValue} which is pretty unlikely but hey.
            Throw GetInvalidOperationException(SR.ApplicationLog_ExhaustedPossibleStreamNames, BaseStreamName)
        End Function

        ''' <summary>
        ''' Makes sure we have an open stream
        ''' </summary>
        Private Sub EnsureStreamIsOpen()
            If m_Stream Is Nothing Then
                m_Stream = GetStream()
            End If
        End Sub

        ''' <summary>
        ''' Closes the stream.
        ''' </summary>
        ''' <remarks>This method should be safe to call whether or not there is a stream</remarks>
        Private Sub CloseCurrentStream()
            If m_Stream IsNot Nothing Then
                SyncLock m_Streams
                    m_Stream.CloseStream()
                    If Not m_Stream.IsInUse Then
                        m_Streams.Remove(m_FullFileName.ToUpper(CultureInfo.InvariantCulture))
                    End If
                    m_Stream = Nothing
                End SyncLock
            End If
        End Sub

        ''' <summary>
        ''' Indicates whether or not the current date has changed to new day
        ''' </summary>
        ''' <returns>True if the date has changed, otherwise False</returns>
        Private Function DayChanged() As Boolean
            Return m_Day.Date <> Now.Date
        End Function

        ''' <summary>
        ''' Indicates whether or not the date has changed to a new week
        ''' </summary>
        ''' <returns>True if the date has changed, otherwise False</returns>
        Private Function WeekChanged() As Boolean
            Return m_FirstDayOfWeek.Date <> GetFirstDayOfWeek(Now.Date)
        End Function

        ''' <summary>
        ''' Utility to get the date of the first day of the week from the passed in date
        ''' </summary>
        ''' <param name="checkDate">The date being checked</param>
        Private Shared Function GetFirstDayOfWeek(ByVal checkDate As Date) As Date
            Return checkDate.AddDays(-checkDate.DayOfWeek).Date
        End Function

        ''' <summary>
        ''' Checks for date changes and carries out appropriate actions
        ''' </summary>
        ''' <remarks>
        ''' If the user has selected a DateStamp option then a change of
        ''' date means we need to open a new file.
        ''' </remarks>
        Private Sub HandleDateChange()
            If Me.LogFileCreationSchedule = LogFileCreationScheduleOption.Daily Then
                If DayChanged() Then
                    CloseCurrentStream()
                End If
            ElseIf Me.LogFileCreationSchedule = LogFileCreationScheduleOption.Weekly Then
                If WeekChanged() Then
                    CloseCurrentStream()
                End If
            End If
        End Sub

        ''' <summary>
        ''' Checks the size of the current log plus the new entry and the free disk space against
        ''' the user's limits.
        ''' </summary>
        ''' <param name="newEntrySize">The size of what's about to be written to the file</param>
        ''' <returns>True if the limits aren't trespassed, otherwise False</returns>
        ''' <remarks>This method is not 100% accurate if AutoFlush is False</remarks>
        Private Function ResourcesAvailable(ByVal newEntrySize As Long) As Boolean

            If ListenerStream.FileSize + newEntrySize > Me.MaxFileSize Then
                If Me.DiskSpaceExhaustedBehavior = DiskSpaceExhaustedOption.ThrowException Then
                    Throw New InvalidOperationException(GetResourceString(SR.ApplicationLog_FileExceedsMaximumSize))
                End If
                Return False
            End If

            If Me.GetFreeDiskSpace() - newEntrySize < Me.ReserveDiskSpace Then
                If Me.DiskSpaceExhaustedBehavior = DiskSpaceExhaustedOption.ThrowException Then
                    Throw New InvalidOperationException(GetResourceString(SR.ApplicationLog_ReservedSpaceEncroached))
                End If
                Return False
            End If

            Return True
        End Function

        ''' <summary>
        ''' Returns the total amount of free disk space available to the current user
        ''' </summary>
        ''' <returns>The total amount, in bytes, of free disk space available to the current user</returns>
        ''' <remarks>Throws an exception if API fails</remarks>
        Private Function GetFreeDiskSpace() As Long
            Dim PathName As String = Path.GetPathRoot(Path.GetFullPath(FullLogFileName))

            'Initialize FreeUserSpace so we can determine if its value is changed by the API call
            Dim FreeUserSpace As Long = -1
            Dim TotalUserSpace As Long
            Dim TotalFreeSpace As Long

            Dim discoveryPermission As New FileIOPermission(FileIOPermissionAccess.PathDiscovery, PathName)
            discoveryPermission.Demand()

            If UnsafeNativeMethods.GetDiskFreeSpaceEx(PathName, FreeUserSpace, TotalUserSpace, TotalFreeSpace) Then
                If FreeUserSpace > -1 Then
                    Return FreeUserSpace
                End If
            End If

            Throw GetWin32Exception(SR.ApplicationLog_FreeSpaceError)
        End Function

        ''' <summary>
        ''' Opens a file and attempts to determine the file's encoding
        ''' </summary>
        ''' <returns>The encoding or Nothing</returns>
        Private Function GetFileEncoding(ByVal fileName As String) As Encoding

            If File.Exists(fileName) Then
                Dim Reader As StreamReader = Nothing
                Try

                    'Attempt to determine the encodoing of the file. The call to Reader.ReadLine
                    'will change the current encoding of Reader to that of the file.
                    Reader = New StreamReader(fileName, Me.Encoding, True)

                    'Ignore 0 length file
                    If Reader.BaseStream.Length > 0 Then
                        Reader.ReadLine()

                        Return Reader.CurrentEncoding
                    End If
                Finally
                    If Reader IsNot Nothing Then
                        Reader.Close()
                    End If
                End Try
            End If

            Return Nothing
        End Function

        ''' <summary>
        ''' Gets the host name
        ''' </summary>
        ''' <value>The host name</value>
        ''' <remarks>We use the machine name because we can get that even if not hooked up to a network</remarks>
        Private ReadOnly Property HostName() As String
            Get
                If m_HostName = "" Then
                    ' Use the machine name
                    m_HostName = System.Environment.MachineName
                End If
                Return m_HostName
            End Get
        End Property

        ''' <summary>
        ''' Demands a FileIO write permission.
        ''' </summary>
        ''' <remarks>This method should be called by public API that doesn't map to TraceListener.
        ''' This ensures these API cannot be used to circumvent CAS
        '''</remarks>
        Private Sub DemandWritePermission()
            Debug.Assert(Path.GetDirectoryName(Me.LogFileName) <> "", "The log directory shouldn't be empty.")
            Dim fileName As String = Path.GetDirectoryName(Me.LogFileName)
            Dim filePermission As New FileIOPermission(FileIOPermissionAccess.Write, fileName)
            filePermission.Demand()
        End Sub

        ''' <summary>
        ''' Validates that the value being passed as an LogFileLocation enum is a legal value
        ''' </summary>
        ''' <param name="value"></param>
        Private Sub ValidateLogFileLocationEnumValue(ByVal value As LogFileLocation, ByVal paramName As String)
            If value < LogFileLocation.TempDirectory OrElse value > LogFileLocation.Custom Then
                Throw New System.ComponentModel.InvalidEnumArgumentException(paramName, DirectCast(value, Integer), GetType(LogFileLocation))
            End If
        End Sub

        ''' <summary>
        ''' Validates that the value being passed as an DiskSpaceExhaustedOption enum is a legal value
        ''' </summary>
        ''' <param name="value"></param>
        Private Sub ValidateDiskSpaceExhaustedOptionEnumValue(ByVal value As DiskSpaceExhaustedOption, ByVal paramName As String)
            If value < DiskSpaceExhaustedOption.ThrowException OrElse value > DiskSpaceExhaustedOption.DiscardMessages Then
                Throw New System.ComponentModel.InvalidEnumArgumentException(paramName, DirectCast(value, Integer), GetType(DiskSpaceExhaustedOption))
            End If
        End Sub

        ''' <summary>
        ''' Validates that the value being passed as an LogFileCreationScheduleOption enum is a legal value
        ''' </summary>
        ''' <param name="value"></param>
        Private Sub ValidateLogFileCreationScheduleOptionEnumValue(ByVal value As LogFileCreationScheduleOption, ByVal paramName As String)
            If value < LogFileCreationScheduleOption.None OrElse value > LogFileCreationScheduleOption.Weekly Then
                Throw New System.ComponentModel.InvalidEnumArgumentException(paramName, DirectCast(value, Integer), GetType(LogFileCreationScheduleOption))
            End If
        End Sub

        ''' <summary>
        ''' Convert a stack into a string
        ''' </summary>
        ''' <param name="stack"></param>
        ''' <returns>Returns the stack as a .csv string</returns>
        Private Shared Function StackToString(ByVal stack As System.Collections.Stack) As String
            Debug.Assert(stack IsNot Nothing, "Stack wasn't created.")

            Dim length As Integer = STACK_DELIMITER.Length
            Dim sb As New StringBuilder()

            For Each obj As Object In stack
                sb.Append(obj.ToString() & STACK_DELIMITER)
            Next

            ' Escape the quotes
            sb.Replace("""", """""")

            ' Remove trailing delimiter
            If sb.Length >= length Then
                sb.Remove(sb.Length - length, length)
            End If

            Return """" & sb.ToString() & """"

        End Function

        ' Indicates the location of the log's directory
        Private m_Location As LogFileLocation = LogFileLocation.LocalUserApplicationDirectory

        ' Indicates whether or not to flush after every write
        Private m_AutoFlush As Boolean = False

        ' Indicates whether to append to or overwrite the log file
        Private m_Append As Boolean = True

        ' Indicates whether or not to include the host id in the output
        Private m_IncludeHostName As Boolean = False

        ' Indicates what behavior should take place when a resource level has been passed
        Private m_DiskSpaceExhaustedBehavior As DiskSpaceExhaustedOption = DiskSpaceExhaustedOption.DiscardMessages

        ' Stores the name of the file minus the path, date stamp, and file number
        Private m_BaseFileName As String = Path.GetFileNameWithoutExtension(Application.ExecutablePath)

        ' Indicate which date stamp should be used in the log file name
        Private m_LogFileDateStamp As LogFileCreationScheduleOption = LogFileCreationScheduleOption.None

        ' The maximum size of the log file
        Private m_MaxFileSize As Long = 5000000L

        ' The amount of free disk space there needs to be on the drive of the log file
        Private m_ReserveDiskSpace As Long = 10000000L

        ' The delimiter to be used to separate fields in a line of output
        Private m_Delimiter As String = vbTab

        ' The encoding of the log file
        Private m_Encoding As Encoding = System.Text.Encoding.UTF8

        ' The full name and path of the log file
        Private m_FullFileName As String

        ' Directory to be used for the log file if Location is set to Custom
        Private m_CustomLocation As String = Application.UserAppDataPath

        ' Reference counted stream used for writing to the log file
        Private m_Stream As ReferencedStream

        Private m_Day As DateTime = Now.Date

        Private m_FirstDayOfWeek As DateTime = GetFirstDayOfWeek(Now.Date)

        Private m_HostName As String

        ' Indicates whether or not properties have been set
        ' Note: Properties that use m_PropertiesSet to track whether or not
        '       they've been set should always be set through the property setter and not
        '       by directly changing the corresponding private field.
        Private m_PropertiesSet As New System.Collections.BitArray(PROPERTY_COUNT, False)

        ' Table of all of the files opened by any FileLogTraceListener in the current process
        Private Shared m_Streams As New Dictionary(Of String, ReferencedStream)

        ' A list of supported attributes
        Private m_SupportedAttributes() As String = New String() {KEY_APPEND, KEY_APPEND_PASCAL, KEY_AUTOFLUSH, KEY_AUTOFLUSH_PASCAL, KEY_AUTOFLUSH_CAMEL, _
                                                                  KEY_BASEFILENAME, KEY_BASEFILENAME_PASCAL, KEY_BASEFILENAME_CAMEL, KEY_BASEFILENAME_PASCAL_ALT, KEY_BASEFILENAME_CAMEL_ALT, _
                                                                  KEY_CUSTOMLOCATION, KEY_CUSTOMLOCATION_PASCAL, KEY_CUSTOMLOCATION_CAMEL, KEY_DELIMITER, KEY_DELIMITER_PASCAL, _
                                                                  KEY_DISKSPACEEXHAUSTEDBEHAVIOR, KEY_DISKSPACEEXHAUSTEDBEHAVIOR_PASCAL, KEY_DISKSPACEEXHAUSTEDBEHAVIOR_CAMEL, _
                                                                  KEY_ENCODING, KEY_ENCODING_PASCAL, KEY_INCLUDEHOSTNAME, KEY_INCLUDEHOSTNAME_PASCAL, KEY_INCLUDEHOSTNAME_CAMEL, KEY_LOCATION, KEY_LOCATION_PASCAL, _
                                                                  KEY_LOGFILECREATIONSCHEDULE, KEY_LOGFILECREATIONSCHEDULE_PASCAL, KEY_LOGFILECREATIONSCHEDULE_CAMEL, _
                                                                  KEY_MAXFILESIZE, KEY_MAXFILESIZE_PASCAL, KEY_MAXFILESIZE_CAMEL, KEY_RESERVEDISKSPACE, KEY_RESERVEDISKSPACE_PASCAL, KEY_RESERVEDISKSPACE_CAMEL}

        ' Identifies properties in the bitarray
        Private Const PROPERTY_COUNT As Integer = 12
        Private Const APPEND_INDEX As Integer = 0
        Private Const AUTOFLUSH_INDEX As Integer = 1
        Private Const BASEFILENAME_INDEX As Integer = 2
        Private Const CUSTOMLOCATION_INDEX As Integer = 3
        Private Const DELIMITER_INDEX As Integer = 4
        Private Const DISKSPACEEXHAUSTEDBEHAVIOR_INDEX As Integer = 5
        Private Const ENCODING_INDEX As Integer = 6
        Private Const INCLUDEHOSTNAME_INDEX As Integer = 7
        Private Const LOCATION_INDEX As Integer = 8
        Private Const LOGFILECREATIONSCHEDULE_INDEX As Integer = 9
        Private Const MAXFILESIZE_INDEX As Integer = 10
        Private Const RESERVEDISKSPACE_INDEX As Integer = 11

        Private Const DATE_FORMAT As String = "yyyy-MM-dd"
        Private Const FILE_EXTENSION As String = ".log"
        Private Const MAX_OPEN_ATTEMPTS As Integer = Integer.MaxValue

        ' Name to be used when parameterless constructor is called
        Private Const DEFAULT_NAME As String = "FileLogTraceListener"

        ' The minimum setting allowed for maximum file size
        Private Const MIN_FILE_SIZE As Integer = 1000

        ' Attribute keys used to access properties set in the config file
        Private Const KEY_APPEND As String = "append"
        Private Const KEY_APPEND_PASCAL As String = "Append"

        Private Const KEY_AUTOFLUSH As String = "autoflush"
        Private Const KEY_AUTOFLUSH_PASCAL As String = "AutoFlush"
        Private Const KEY_AUTOFLUSH_CAMEL As String = "autoFlush"

        Private Const KEY_BASEFILENAME As String = "basefilename"
        Private Const KEY_BASEFILENAME_PASCAL As String = "BaseFilename"
        Private Const KEY_BASEFILENAME_CAMEL As String = "baseFilename"
        Private Const KEY_BASEFILENAME_PASCAL_ALT As String = "BaseFileName"
        Private Const KEY_BASEFILENAME_CAMEL_ALT As String = "baseFileName"

        Private Const KEY_CUSTOMLOCATION As String = "customlocation"
        Private Const KEY_CUSTOMLOCATION_PASCAL As String = "CustomLocation"
        Private Const KEY_CUSTOMLOCATION_CAMEL As String = "customLocation"

        Private Const KEY_DELIMITER As String = "delimiter"
        Private Const KEY_DELIMITER_PASCAL As String = "Delimiter"

        Private Const KEY_DISKSPACEEXHAUSTEDBEHAVIOR As String = "diskspaceexhaustedbehavior"
        Private Const KEY_DISKSPACEEXHAUSTEDBEHAVIOR_PASCAL As String = "DiskSpaceExhaustedBehavior"
        Private Const KEY_DISKSPACEEXHAUSTEDBEHAVIOR_CAMEL As String = "diskSpaceExhaustedBehavior"

        Private Const KEY_ENCODING As String = "encoding"
        Private Const KEY_ENCODING_PASCAL As String = "Encoding"

        Private Const KEY_INCLUDEHOSTNAME As String = "includehostname"
        Private Const KEY_INCLUDEHOSTNAME_PASCAL As String = "IncludeHostName"
        Private Const KEY_INCLUDEHOSTNAME_CAMEL As String = "includeHostName"

        Private Const KEY_LOCATION As String = "location"
        Private Const KEY_LOCATION_PASCAL As String = "Location"

        Private Const KEY_LOGFILECREATIONSCHEDULE As String = "logfilecreationschedule"
        Private Const KEY_LOGFILECREATIONSCHEDULE_PASCAL As String = "LogFileCreationSchedule"
        Private Const KEY_LOGFILECREATIONSCHEDULE_CAMEL As String = "logFileCreationSchedule"

        Private Const KEY_MAXFILESIZE As String = "maxfilesize"
        Private Const KEY_MAXFILESIZE_PASCAL As String = "MaxFileSize"
        Private Const KEY_MAXFILESIZE_CAMEL As String = "maxFileSize"

        Private Const KEY_RESERVEDISKSPACE As String = "reservediskspace"
        Private Const KEY_RESERVEDISKSPACE_PASCAL As String = "ReserveDiskSpace"
        Private Const KEY_RESERVEDISKSPACE_CAMEL As String = "reserveDiskSpace"

        ' Delimiter used when converting a stack to a string
        Private Const STACK_DELIMITER As String = ", "

        ''' <summary>
        ''' Wraps a StreamWriter and keeps a reference count. This enables multiple 
        ''' FileLogTraceListeners on multiple threads to access the same file.
        ''' </summary>
        Friend Class ReferencedStream
            Implements IDisposable

            ''' <summary>
            ''' Creates a new referenced stream
            ''' </summary>
            ''' <param name="stream">The stream that does the actual writing</param>
            Friend Sub New(ByVal stream As StreamWriter)
                m_Stream = stream
            End Sub

            ''' <summary>
            ''' Writes a message to the stream
            ''' </summary>
            ''' <param name="message">The message to write</param>
            Friend Sub Write(ByVal message As String)
                SyncLock m_SyncObject
                    m_Stream.Write(message)
                End SyncLock
            End Sub

            ''' <summary>
            ''' Writes a message to the stream as a line
            ''' </summary>
            ''' <param name="message">The message to write</param>
            Friend Sub WriteLine(ByVal message As String)
                SyncLock m_SyncObject
                    m_Stream.WriteLine(message)
                End SyncLock
            End Sub

            ''' <summary>
            ''' Increments the reference count for the stream
            ''' </summary>
            Friend Sub AddReference()
                SyncLock m_SyncObject
                    m_ReferenceCount += 1
                End SyncLock
            End Sub

            ''' <summary>
            ''' Flushes the stream
            ''' </summary>
            Friend Sub Flush()
                SyncLock m_SyncObject
                    m_Stream.Flush()
                End SyncLock
            End Sub

            ''' <summary>
            ''' Decrements the reference count to the stream and closes the stream if the reference count 
            ''' is zero
            ''' </summary>
            Friend Sub CloseStream()
                SyncLock m_SyncObject
                    Try
                        m_ReferenceCount -= 1
                        m_Stream.Flush()
                        Debug.Assert(m_ReferenceCount >= 0, "Ref count is below 0")
                    Finally
                        If m_ReferenceCount <= 0 Then
                            m_Stream.Close()
                            m_Stream = Nothing
                        End If
                    End Try
                End SyncLock
            End Sub

            ''' <summary>
            ''' Indicates whether or not the stream is still in use by a FileLogTraceListener
            ''' </summary>
            ''' <value>True if the stream is being used, otherwise False</value>
            Friend ReadOnly Property IsInUse() As Boolean
                Get
                    Return m_Stream IsNot Nothing
                End Get
            End Property

            ''' <summary>
            ''' The size of the log file
            ''' </summary>
            ''' <value>The size</value>
            Friend ReadOnly Property FileSize() As Long
                Get
                    Return m_Stream.BaseStream.Length
                End Get
            End Property

            ''' <summary>
            ''' Ensures the stream is closed (flushed) no matter how we are closed
            ''' </summary>
            ''' <param name="disposing">Indicates who called dispose</param>
            Private Overloads Sub Dispose(ByVal disposing As Boolean)
                If disposing Then
                    If Not m_Disposed Then
                        If m_Stream IsNot Nothing Then
                            m_Stream.Close()
                        End If
                        m_Disposed = True
                    End If
                End If
            End Sub

            ''' <summary>
            ''' Standard implementation of IDisposable
            ''' </summary>
            Public Overloads Sub Dispose() Implements IDisposable.Dispose
                ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
                Dispose(True)
                GC.SuppressFinalize(Me)
            End Sub

            ''' <summary>
            ''' Ensures stream is closed at GC
            ''' </summary>
            Protected Overrides Sub Finalize()
                ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
                Dispose(False)
                MyBase.Finalize()
            End Sub

            ' The stream that does the writing
            Private m_Stream As StreamWriter

            ' The number of FileLogTraceListeners using the stream
            Private m_ReferenceCount As Integer = 0

            ' Used for synchronizing writing and reference counting
            Private m_SyncObject As Object = New Object

            ' Indicates whether or not the object has been disposed
            Private m_Disposed As Boolean = False

        End Class 'ReferencedStream

    End Class 'FileLogTraceListener

End Namespace
