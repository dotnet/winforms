' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Option Explicit On
Option Strict On

Imports System.ComponentModel
Imports System.Globalization
Imports System.IO
Imports System.Security.Permissions
Imports System.Text
Imports System.Windows.Forms
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
    <Runtime.InteropServices.ComVisible(False)>
    Public Class FileLogTraceListener
        Inherits TraceListener

        ''' <summary>
        ''' Creates a FileLogTraceListener with the passed in name
        ''' </summary>
        ''' <param name="name">The name of the listener</param>
        Public Sub New(name As String)
            MyBase.New(name)
        End Sub

        ''' <summary>
        ''' Creates a FileLogTraceListener with default name
        ''' </summary>
        Public Sub New()
            Me.New(DEFAULT_NAME)
        End Sub

        ''' <summary>
        ''' Indicates the log's directory
        ''' </summary>
        ''' <value>An enum which can indicate one of several logical locations for the log</value>
        Public Property Location() As LogFileLocation
            Get
                If Not _propertiesSet(LOCATION_INDEX) Then
                    If Attributes.ContainsKey(KEY_LOCATION) Then
                        Dim converter As TypeConverter = TypeDescriptor.GetConverter(GetType(LogFileLocation))
                        Me.Location = DirectCast(converter.ConvertFromInvariantString(Attributes(KEY_LOCATION)), LogFileLocation)
                    End If
                End If
                Return _location
            End Get
            Set(value As LogFileLocation)
                ValidateLogFileLocationEnumValue(value, NameOf(value))

                ' If the location is changing we need to close the current file
                If _location <> value Then
                    CloseCurrentStream()
                End If
                _location = value
                _propertiesSet(LOCATION_INDEX) = True
            End Set
        End Property

        ''' <summary>
        ''' Indicates whether or not the stream should be flushed after every write
        ''' </summary>
        ''' <value>True if the stream should be flushed after every write, otherwise False</value>
        Public Property AutoFlush() As Boolean
            Get
                If Not _propertiesSet(AUTOFLUSH_INDEX) Then
                    If Attributes.ContainsKey(KEY_AUTOFLUSH) Then
                        Me.AutoFlush = Convert.ToBoolean(Attributes(KEY_AUTOFLUSH), CultureInfo.InvariantCulture)
                    End If
                End If

                Return _autoFlush
            End Get
            Set(value As Boolean)
                DemandWritePermission()
                _autoFlush = value
                _propertiesSet(AUTOFLUSH_INDEX) = True
            End Set
        End Property

        ''' <summary>
        ''' Indicates whether or not the host name of the logging machine should
        ''' be included in the output.
        ''' </summary>
        ''' <value>True if the HostId should be included, otherwise False</value>
        Public Property IncludeHostName() As Boolean
            Get
                If Not _propertiesSet(INCLUDEHOSTNAME_INDEX) Then
                    If Attributes.ContainsKey(KEY_INCLUDEHOSTNAME) Then
                        Me.IncludeHostName = Convert.ToBoolean(Attributes(KEY_INCLUDEHOSTNAME), CultureInfo.InvariantCulture)
                    End If
                End If
                Return _includeHostName
            End Get
            Set(value As Boolean)
                DemandWritePermission()
                _includeHostName = value
                _propertiesSet(INCLUDEHOSTNAME_INDEX) = True
            End Set
        End Property

        ''' <summary>
        ''' Indicates whether or not the file should be appended to or overwritten
        ''' </summary>
        ''' <value>True if the file should be appended to, otherwise False</value>
        Public Property Append() As Boolean
            Get
                If Not _propertiesSet(APPEND_INDEX) Then
                    If Attributes.ContainsKey(KEY_APPEND) Then
                        Me.Append = Convert.ToBoolean(Attributes(KEY_APPEND), CultureInfo.InvariantCulture)
                    End If
                End If

                Return _append
            End Get
            Set(value As Boolean)

                DemandWritePermission()

                ' If this property is changing, we need to close the current file
                If value <> _append Then
                    CloseCurrentStream()
                End If
                _append = value
                _propertiesSet(APPEND_INDEX) = True
            End Set
        End Property

        ''' <summary>
        ''' Indicates what to do when the size of the log trespasses on the MaxFileSize
        ''' or the ReserveDiskSpace set by the user
        ''' </summary>
        ''' <value>An enum indicating the desired behavior (do nothing, throw)</value>
        Public Property DiskSpaceExhaustedBehavior() As DiskSpaceExhaustedOption
            Get
                If Not _propertiesSet(DISKSPACEEXHAUSTEDBEHAVIOR_INDEX) Then
                    If Attributes.ContainsKey(KEY_DISKSPACEEXHAUSTEDBEHAVIOR) Then
                        Dim converter As TypeConverter = TypeDescriptor.GetConverter(GetType(DiskSpaceExhaustedOption))
                        Me.DiskSpaceExhaustedBehavior = DirectCast(converter.ConvertFromInvariantString(Attributes(KEY_DISKSPACEEXHAUSTEDBEHAVIOR)), DiskSpaceExhaustedOption)
                    End If
                End If
                Return _diskSpaceExhaustedBehavior
            End Get
            Set(value As DiskSpaceExhaustedOption)
                DemandWritePermission()
                ValidateDiskSpaceExhaustedOptionEnumValue(value, NameOf(value))
                _diskSpaceExhaustedBehavior = value
                _propertiesSet(DISKSPACEEXHAUSTEDBEHAVIOR_INDEX) = True
            End Set
        End Property

        ''' <summary>
        ''' The name of the log file not including DateStamp, file number, Path or extension
        ''' </summary>
        ''' <value>The name of the log file</value>
        Public Property BaseFileName() As String
            Get
                If Not _propertiesSet(BASEFILENAME_INDEX) Then
                    If Attributes.ContainsKey(KEY_BASEFILENAME) Then
                        Me.BaseFileName = Attributes(KEY_BASEFILENAME)
                    End If
                End If
                Return _baseFileName
            End Get
            Set(value As String)
                If String.IsNullOrEmpty(value) Then
                    Throw GetArgumentNullException("value", SR.ApplicationLogBaseNameNull)
                End If

                ' Test the file name. This will throw if the name is invalid.
                Path.GetFullPath(value)

                If String.Compare(value, _baseFileName, StringComparison.OrdinalIgnoreCase) <> 0 Then
                    CloseCurrentStream()
                    _baseFileName = value
                End If

                _propertiesSet(BASEFILENAME_INDEX) = True
            End Set
        End Property

        ''' <summary>
        '''  The full name and path of the actual log file including DateStamp and file number
        ''' </summary>
        ''' <value>The full name and path</value>
        ''' <remarks>Calling this method will open the log file if it's not already open</remarks>
        Public ReadOnly Property FullLogFileName() As String
            Get
                ' The only way to reliably know the file name is to open the file. If we
                ' don't have a stream, get one (this will open the file)
                EnsureStreamIsOpen()

                ' We shouldn't use fields for demands so we use a local variable
                Dim returnPath As String = _fullFileName
#Disable Warning SYSLIB0003 ' Type or member is obsolete
                Dim filePermission As New FileIOPermission(FileIOPermissionAccess.PathDiscovery, returnPath)
#Enable Warning SYSLIB0003 ' Type or member is obsolete
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
                If Not _propertiesSet(LOGFILECREATIONSCHEDULE_INDEX) Then
                    If Attributes.ContainsKey(KEY_LOGFILECREATIONSCHEDULE) Then
                        Dim converter As TypeConverter = TypeDescriptor.GetConverter(GetType(LogFileCreationScheduleOption))
                        Me.LogFileCreationSchedule = DirectCast(converter.ConvertFromInvariantString(Attributes(KEY_LOGFILECREATIONSCHEDULE)), LogFileCreationScheduleOption)
                    End If
                End If
                Return _logFileDateStamp
            End Get
            Set(value As LogFileCreationScheduleOption)
                ValidateLogFileCreationScheduleOptionEnumValue(value, NameOf(value))

                If value <> _logFileDateStamp Then
                    CloseCurrentStream()
                    _logFileDateStamp = value
                End If

                _propertiesSet(LOGFILECREATIONSCHEDULE_INDEX) = True
            End Set
        End Property

        ''' <summary>
        ''' The maximum size in bytes the log file is allowed to grow to
        ''' </summary>
        ''' <value>The maximum size</value>
        Public Property MaxFileSize() As Long
            Get
                If Not _propertiesSet(MAXFILESIZE_INDEX) Then
                    If Attributes.ContainsKey(KEY_MAXFILESIZE) Then
                        Me.MaxFileSize = Convert.ToInt64(Attributes(KEY_MAXFILESIZE), CultureInfo.InvariantCulture)
                    End If
                End If
                Return _maxFileSize
            End Get
            Set(value As Long)
                DemandWritePermission()
                If value < MIN_FILE_SIZE Then
                    Throw GetArgumentExceptionWithArgName("value", SR.ApplicationLogNumberTooSmall, "MaxFileSize")
                End If
                _maxFileSize = value
                _propertiesSet(MAXFILESIZE_INDEX) = True
            End Set
        End Property

        ''' <summary>
        ''' The amount of disk space, in bytes, that must be available after a write
        ''' </summary>
        ''' <value>The reserved disk space</value>
        Public Property ReserveDiskSpace() As Long
            Get
                If Not _propertiesSet(RESERVEDISKSPACE_INDEX) Then
                    If Attributes.ContainsKey(KEY_RESERVEDISKSPACE) Then
                        Me.ReserveDiskSpace = Convert.ToInt64(Attributes(KEY_RESERVEDISKSPACE), CultureInfo.InvariantCulture)
                    End If
                End If
                Return _reserveDiskSpace
            End Get
            Set(value As Long)
                DemandWritePermission()
                If value < 0 Then
                    Throw GetArgumentExceptionWithArgName("value", SR.ApplicationLog_NegativeNumber, "ReserveDiskSpace")
                End If
                _reserveDiskSpace = value
                _propertiesSet(RESERVEDISKSPACE_INDEX) = True
            End Set
        End Property

        ''' <summary>
        ''' The delimiter to be used to delimit fields in a line of output
        ''' </summary>
        ''' <value>The delimiter</value>
        Public Property Delimiter() As String
            Get
                If Not _propertiesSet(DELIMITER_INDEX) Then
                    If Attributes.ContainsKey(KEY_DELIMITER) Then
                        Me.Delimiter = Attributes(KEY_DELIMITER)
                    End If
                End If
                Return _delimiter
            End Get
            Set(value As String)
                _delimiter = value
                _propertiesSet(DELIMITER_INDEX) = True
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
                If Not _propertiesSet(ENCODING_INDEX) Then
                    If Attributes.ContainsKey(KEY_ENCODING) Then
                        Me.Encoding = System.Text.Encoding.GetEncoding(Attributes(KEY_ENCODING))
                    End If
                End If
                Return _encoding
            End Get
            Set(value As Encoding)
                If value Is Nothing Then
                    Throw GetArgumentNullException("value")
                End If
                _encoding = value
                _propertiesSet(ENCODING_INDEX) = True
            End Set
        End Property

        ''' <summary>
        ''' The directory to be used if Location is set to Custom
        ''' </summary>
        ''' <value>The name of the directory</value>
        ''' <remarks>This will throw if the path cannot be resolved</remarks>
        Public Property CustomLocation() As String
            Get
                If Not _propertiesSet(CUSTOMLOCATION_INDEX) Then
                    If Attributes.ContainsKey(KEY_CUSTOMLOCATION) Then
                        Me.CustomLocation = Attributes(KEY_CUSTOMLOCATION)
                    End If
                End If

                Dim fileName As String = Path.GetFullPath(_customLocation)
#Disable Warning SYSLIB0003 ' Type or member is obsolete
                Dim filePermission As New FileIOPermission(FileIOPermissionAccess.PathDiscovery, fileName)
#Enable Warning SYSLIB0003 ' Type or member is obsolete
                filePermission.Demand()
                Return fileName
            End Get
            Set(value As String)

                ' Validate the path
                Dim tempPath As String = Path.GetFullPath(value)

                If Not Directory.Exists(tempPath) Then
                    Directory.CreateDirectory(tempPath)
                End If

                ' If we're using custom location and the value is changing we need to
                ' close the stream
                If Me.Location = LogFileLocation.Custom And String.Compare(tempPath, _customLocation, StringComparison.OrdinalIgnoreCase) <> 0 Then
                    CloseCurrentStream()
                End If

                ' Since the user is setting a custom path, set Location to custom
                Location = LogFileLocation.Custom

                _customLocation = tempPath
                _propertiesSet(CUSTOMLOCATION_INDEX) = True

            End Set
        End Property

        ''' <summary>
        ''' Writes the message to the log
        ''' </summary>
        ''' <param name="message">The message to be written</param>
        Public Overloads Overrides Sub Write(message As String)

            ' Use Try block to attempt to close stream if an exception is thrown
            Try
                HandleDateChange()

                ' Check resources
                Dim NewEntrySize As Long = Encoding.GetByteCount(message)

                If ResourcesAvailable(NewEntrySize) Then
                    ListenerStream.Write(message)
                    If AutoFlush Then
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
        Public Overloads Overrides Sub WriteLine(message As String)

            ' Use Try block to attempt to close stream if an exception is thrown
            Try
                HandleDateChange()

                ' Check resources
                Dim NewEntrySize As Long = Encoding.GetByteCount(message & vbCrLf)

                If ResourcesAvailable(NewEntrySize) Then
                    ListenerStream.WriteLine(message)
                    If AutoFlush Then
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
        Public Overrides Sub TraceEvent(eventCache As TraceEventCache, source As String, eventType As TraceEventType, id As Integer, message As String)

            If Filter IsNot Nothing Then
                If Not Filter.ShouldTrace(eventCache, source, eventType, id, message, Nothing, Nothing, Nothing) Then
                    Return
                End If
            End If
            Dim outBuilder As New StringBuilder

            ' Add fields that always appear (source, eventType, id, message)
            ' source
            outBuilder.Append(source & Delimiter)

            ' eventType
            outBuilder.Append([Enum].GetName(GetType(TraceEventType), eventType) & Delimiter)

            ' id
            outBuilder.Append(id.ToString(CultureInfo.InvariantCulture) & Delimiter)

            ' message
            outBuilder.Append(message)

            ' Add optional fields
            ' Callstack
            If (Me.TraceOutputOptions And TraceOptions.Callstack) = TraceOptions.Callstack Then
                outBuilder.Append(Delimiter & eventCache.Callstack)
            End If

            ' LogicalOperationStack
            If (Me.TraceOutputOptions And TraceOptions.LogicalOperationStack) = TraceOptions.LogicalOperationStack Then
                outBuilder.Append(Delimiter & StackToString(eventCache.LogicalOperationStack))
            End If

            ' DateTime
            If (Me.TraceOutputOptions And TraceOptions.DateTime) = TraceOptions.DateTime Then
                ' Add DateTime. Time will be in GMT.
                outBuilder.Append(Delimiter & eventCache.DateTime.ToString("u", CultureInfo.InvariantCulture))
            End If

            ' ProcessId
            If (Me.TraceOutputOptions And TraceOptions.ProcessId) = TraceOptions.ProcessId Then
                outBuilder.Append(Delimiter & eventCache.ProcessId.ToString(CultureInfo.InvariantCulture))
            End If

            ' ThreadId
            If (Me.TraceOutputOptions And TraceOptions.ThreadId) = TraceOptions.ThreadId Then
                outBuilder.Append(Delimiter & eventCache.ThreadId)
            End If

            ' Timestamp
            If (Me.TraceOutputOptions And TraceOptions.Timestamp) = TraceOptions.Timestamp Then
                outBuilder.Append(Delimiter & eventCache.Timestamp.ToString(CultureInfo.InvariantCulture))
            End If

            ' HostName
            If IncludeHostName Then
                outBuilder.Append(Delimiter & HostName)
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
        Public Overrides Sub TraceEvent(eventCache As TraceEventCache, source As String, eventType As TraceEventType, id As Integer, format As String, ParamArray args() As Object)

            ' Create the message
            Dim message As String
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
        Public Overrides Sub TraceData(eventCache As TraceEventCache, source As String, eventType As TraceEventType, id As Integer, data As Object)

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
        Public Overrides Sub TraceData(eventCache As TraceEventCache, source As String, eventType As TraceEventType, id As Integer, ParamArray data As Object())

            Dim messageBuilder As New StringBuilder()
            If data IsNot Nothing Then
                Dim bound As Integer = data.Length - 1
                For i As Integer = 0 To bound
                    messageBuilder.Append(data(i).ToString())
                    If i <> bound Then
                        messageBuilder.Append(Delimiter)
                    End If
                Next i
            End If

            TraceEvent(eventCache, source, eventType, id, messageBuilder.ToString())
        End Sub

        ''' <summary>
        ''' Flushes the underlying stream
        ''' </summary>
        Public Overrides Sub Flush()
            If _stream IsNot Nothing Then
                _stream.Flush()
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
            Return _supportedAttributes
        End Function

        ''' <summary>
        ''' Makes sure stream is flushed
        ''' </summary>
        ''' <param name="disposing"></param>
        Protected Overrides Sub Dispose(disposing As Boolean)
            If disposing Then
                CloseCurrentStream()
            End If
        End Sub

        ''' <summary>
        ''' Gets the log file name under the current configuration.
        ''' </summary>
        ''' <value>The log file name</value>
        ''' <remarks>
        ''' Includes the full path and the DateStamp, but does not include the
        ''' file number or the extension.
        ''' </remarks>
        Private ReadOnly Property LogFileName() As String
            Get
                Dim basePath As String

                ' Get the directory
                Select Case Location
                    Case LogFileLocation.CommonApplicationDirectory
                        basePath = Application.CommonAppDataPath
                    Case LogFileLocation.ExecutableDirectory
                        basePath = Path.GetDirectoryName(Application.ExecutablePath)
                    Case LogFileLocation.LocalUserApplicationDirectory
                        basePath = Application.UserAppDataPath
                    Case LogFileLocation.TempDirectory
                        basePath = Path.GetTempPath()
                    Case LogFileLocation.Custom
                        If String.IsNullOrEmpty(CustomLocation) Then
                            basePath = Application.UserAppDataPath
                        Else
                            basePath = CustomLocation
                        End If
                    Case Else
                        Debug.Fail("Unrecognized location")
                        basePath = Application.UserAppDataPath
                End Select

                ' Add the base name
                Dim fileName As String = BaseFileName

                ' Add DateTime Stamp
                Select Case LogFileCreationSchedule
                    Case LogFileCreationScheduleOption.Daily
                        fileName += "-" & Now.Date.ToString(DATE_FORMAT, CultureInfo.InvariantCulture)
                    Case LogFileCreationScheduleOption.Weekly
                        ' Get first day of week
                        _firstDayOfWeek = Now.AddDays(-Now.DayOfWeek)
                        fileName += "-" & _firstDayOfWeek.Date.ToString(DATE_FORMAT, CultureInfo.InvariantCulture)
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

                Debug.Assert(_stream IsNot Nothing, "Unable to get stream")
                Return _stream
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
                SyncLock s_streams

                    If s_streams.ContainsKey(caseInsensitiveKey) Then
                        refStream = s_streams(caseInsensitiveKey)
                        If Not refStream.IsInUse Then
                            ' This means that the referenced stream has somehow entered an invalid state so remove it
                            Debug.Fail("Referenced stream is in invalid state")
                            s_streams.Remove(caseInsensitiveKey)
                            refStream = Nothing
                        Else
                            If Append Then
                                ' We are handing off an already existing stream, so we need to make sure the caller has permissions to write to this stream
#Disable Warning SYSLIB0003 ' Type or member is obsolete
                                Dim filePermission As New FileIOPermission(FileIOPermissionAccess.Write, fileName)
#Enable Warning SYSLIB0003 ' Type or member is obsolete
                                filePermission.Demand()

                                refStream.AddReference()
                                _fullFileName = fileName
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
                    Dim fileEncoding As Encoding = Encoding
                    Try
                        If Append Then
                            ' Try to get the file's actual encoding. If we get it, that trumps
                            ' the user specified value
                            fileEncoding = GetFileEncoding(fileName)
                            If fileEncoding Is Nothing Then
                                fileEncoding = Encoding
                            End If
                        End If

                        Dim baseStreamWriter As New StreamWriter(fileName, Append, fileEncoding)
                        refStream = New ReferencedStream(baseStreamWriter)
                        refStream.AddReference()
                        s_streams.Add(caseInsensitiveKey, refStream)
                        _fullFileName = fileName
                        Return refStream
                    Catch ex As IOException
                    End Try

                    i += 1
                End SyncLock
            End While
            'If we fall out the loop, we have failed to obtain a valid stream name.  This occurs if there are files on your system
            'ranging from BaseStreamName0..BaseStreamName{integer.MaxValue} which is pretty unlikely but hey.
            Throw GetInvalidOperationException(SR.ApplicationLog_ExhaustedPossibleStreamNames, BaseStreamName)
        End Function

        ''' <summary>
        ''' Makes sure we have an open stream
        ''' </summary>
        Private Sub EnsureStreamIsOpen()
            If _stream Is Nothing Then
                _stream = GetStream()
            End If
        End Sub

        ''' <summary>
        ''' Closes the stream.
        ''' </summary>
        ''' <remarks>This method should be safe to call whether or not there is a stream</remarks>
        Private Sub CloseCurrentStream()
            If _stream IsNot Nothing Then
                SyncLock s_streams
                    _stream.CloseStream()
                    If Not _stream.IsInUse Then
                        s_streams.Remove(_fullFileName.ToUpper(CultureInfo.InvariantCulture))
                    End If
                    _stream = Nothing
                End SyncLock
            End If
        End Sub

        ''' <summary>
        ''' Indicates whether or not the current date has changed to new day
        ''' </summary>
        ''' <returns>True if the date has changed, otherwise False</returns>
        Private Function DayChanged() As Boolean
            Return _day.Date <> Now.Date
        End Function

        ''' <summary>
        ''' Indicates whether or not the date has changed to a new week
        ''' </summary>
        ''' <returns>True if the date has changed, otherwise False</returns>
        Private Function WeekChanged() As Boolean
            Return _firstDayOfWeek.Date <> GetFirstDayOfWeek(Now.Date)
        End Function

        ''' <summary>
        ''' Utility to get the date of the first day of the week from the passed in date
        ''' </summary>
        ''' <param name="checkDate">The date being checked</param>
        Private Shared Function GetFirstDayOfWeek(checkDate As Date) As Date
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
        Private Function ResourcesAvailable(newEntrySize As Long) As Boolean

            If ListenerStream.FileSize + newEntrySize > MaxFileSize Then
                If Me.DiskSpaceExhaustedBehavior = DiskSpaceExhaustedOption.ThrowException Then
                    Throw New InvalidOperationException(GetResourceString(SR.ApplicationLog_FileExceedsMaximumSize))
                End If
                Return False
            End If

            If GetFreeDiskSpace() - newEntrySize < ReserveDiskSpace Then
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

#Disable Warning SYSLIB0003 ' Type or member is obsolete
            Dim discoveryPermission As New FileIOPermission(FileIOPermissionAccess.PathDiscovery, PathName)
#Enable Warning SYSLIB0003 ' Type or member is obsolete
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
        Private Function GetFileEncoding(fileName As String) As Encoding

            If File.Exists(fileName) Then
                Dim Reader As StreamReader = Nothing
                Try

                    'Attempt to determine the encoding of the file. The call to Reader.ReadLine
                    'will change the current encoding of Reader to that of the file.
                    Reader = New StreamReader(fileName, Encoding, True)

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
                If String.IsNullOrEmpty(_hostName) Then
                    ' Use the machine name
                    _hostName = System.Environment.MachineName
                End If
                Return _hostName
            End Get
        End Property

        ''' <summary>
        ''' Demands a FileIO write permission.
        ''' </summary>
        ''' <remarks>This method should be called by public API that doesn't map to TraceListener.
        ''' This ensures these API cannot be used to circumvent CAS
        '''</remarks>
        Private Sub DemandWritePermission()
            Debug.Assert(Not String.IsNullOrWhiteSpace(Path.GetDirectoryName(LogFileName)), "The log directory shouldn't be empty.")
            Dim fileName As String = Path.GetDirectoryName(LogFileName)
#Disable Warning SYSLIB0003 ' Type or member is obsolete
            Dim filePermission As New FileIOPermission(FileIOPermissionAccess.Write, fileName)
#Enable Warning SYSLIB0003 ' Type or member is obsolete
            filePermission.Demand()
        End Sub

        ''' <summary>
        ''' Validates that the value being passed as an LogFileLocation enum is a legal value
        ''' </summary>
        ''' <param name="value"></param>
        Private Sub ValidateLogFileLocationEnumValue(value As LogFileLocation, paramName As String)
            If value < LogFileLocation.TempDirectory OrElse value > LogFileLocation.Custom Then
                Throw New InvalidEnumArgumentException(paramName, DirectCast(value, Integer), GetType(LogFileLocation))
            End If
        End Sub

        ''' <summary>
        ''' Validates that the value being passed as an DiskSpaceExhaustedOption enum is a legal value
        ''' </summary>
        ''' <param name="value"></param>
        Private Sub ValidateDiskSpaceExhaustedOptionEnumValue(value As DiskSpaceExhaustedOption, paramName As String)
            If value < DiskSpaceExhaustedOption.ThrowException OrElse value > DiskSpaceExhaustedOption.DiscardMessages Then
                Throw New InvalidEnumArgumentException(paramName, DirectCast(value, Integer), GetType(DiskSpaceExhaustedOption))
            End If
        End Sub

        ''' <summary>
        ''' Validates that the value being passed as an LogFileCreationScheduleOption enum is a legal value
        ''' </summary>
        ''' <param name="value"></param>
        Private Sub ValidateLogFileCreationScheduleOptionEnumValue(value As LogFileCreationScheduleOption, paramName As String)
            If value < LogFileCreationScheduleOption.None OrElse value > LogFileCreationScheduleOption.Weekly Then
                Throw New InvalidEnumArgumentException(paramName, DirectCast(value, Integer), GetType(LogFileCreationScheduleOption))
            End If
        End Sub

        ''' <summary>
        ''' Convert a stack into a string
        ''' </summary>
        ''' <param name="stack"></param>
        ''' <returns>Returns the stack as a .csv string</returns>
        Private Shared Function StackToString(stack As Stack) As String
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
        Private _location As LogFileLocation = LogFileLocation.LocalUserApplicationDirectory

        ' Indicates whether or not to flush after every write
        Private _autoFlush As Boolean

        ' Indicates whether to append to or overwrite the log file
        Private _append As Boolean = True

        ' Indicates whether or not to include the host id in the output
        Private _includeHostName As Boolean

        ' Indicates what behavior should take place when a resource level has been passed
        Private _diskSpaceExhaustedBehavior As DiskSpaceExhaustedOption = DiskSpaceExhaustedOption.DiscardMessages

        ' Stores the name of the file minus the path, date stamp, and file number
        Private _baseFileName As String = Path.GetFileNameWithoutExtension(Application.ExecutablePath)

        ' Indicate which date stamp should be used in the log file name
        Private _logFileDateStamp As LogFileCreationScheduleOption = LogFileCreationScheduleOption.None

        ' The maximum size of the log file
        Private _maxFileSize As Long = 5000000L

        ' The amount of free disk space there needs to be on the drive of the log file
        Private _reserveDiskSpace As Long = 10000000L

        ' The delimiter to be used to separate fields in a line of output
        Private _delimiter As String = vbTab

        ' The encoding of the log file
        Private _encoding As Encoding = System.Text.Encoding.UTF8

        ' The full name and path of the log file
        Private _fullFileName As String

        ' Directory to be used for the log file if Location is set to Custom
        Private _customLocation As String = Application.UserAppDataPath

        ' Reference counted stream used for writing to the log file
        Private _stream As ReferencedStream

        Private ReadOnly _day As Date = Now.Date

        Private _firstDayOfWeek As Date = GetFirstDayOfWeek(Now.Date)

        Private _hostName As String

        ' Indicates whether or not properties have been set
        ' Note: Properties that use m_PropertiesSet to track whether or not
        '       they've been set should always be set through the property setter and not
        '       by directly changing the corresponding private field.
        Private ReadOnly _propertiesSet As New BitArray(PROPERTY_COUNT, False)

        ' Table of all of the files opened by any FileLogTraceListener in the current process
        Private Shared ReadOnly s_streams As New Dictionary(Of String, ReferencedStream)

        ' A list of supported attributes
        Private ReadOnly _supportedAttributes() As String = New String() {KEY_APPEND, KEY_APPEND_PASCAL, KEY_AUTOFLUSH, KEY_AUTOFLUSH_PASCAL, KEY_AUTOFLUSH_CAMEL,
                                                                  KEY_BASEFILENAME, KEY_BASEFILENAME_PASCAL, KEY_BASEFILENAME_CAMEL, KEY_BASEFILENAME_PASCAL_ALT, KEY_BASEFILENAME_CAMEL_ALT,
                                                                  KEY_CUSTOMLOCATION, KEY_CUSTOMLOCATION_PASCAL, KEY_CUSTOMLOCATION_CAMEL, KEY_DELIMITER, KEY_DELIMITER_PASCAL,
                                                                  KEY_DISKSPACEEXHAUSTEDBEHAVIOR, KEY_DISKSPACEEXHAUSTEDBEHAVIOR_PASCAL, KEY_DISKSPACEEXHAUSTEDBEHAVIOR_CAMEL,
                                                                  KEY_ENCODING, KEY_ENCODING_PASCAL, KEY_INCLUDEHOSTNAME, KEY_INCLUDEHOSTNAME_PASCAL, KEY_INCLUDEHOSTNAME_CAMEL, KEY_LOCATION, KEY_LOCATION_PASCAL,
                                                                  KEY_LOGFILECREATIONSCHEDULE, KEY_LOGFILECREATIONSCHEDULE_PASCAL, KEY_LOGFILECREATIONSCHEDULE_CAMEL,
                                                                  KEY_MAXFILESIZE, KEY_MAXFILESIZE_PASCAL, KEY_MAXFILESIZE_CAMEL, KEY_RESERVEDISKSPACE, KEY_RESERVEDISKSPACE_PASCAL, KEY_RESERVEDISKSPACE_CAMEL}

        ' Identifies properties in the BitArray
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
            Friend Sub New(stream As StreamWriter)
                _stream = stream
            End Sub

            ''' <summary>
            ''' Writes a message to the stream
            ''' </summary>
            ''' <param name="message">The message to write</param>
            Friend Sub Write(message As String)
                SyncLock _syncObject
                    _stream.Write(message)
                End SyncLock
            End Sub

            ''' <summary>
            ''' Writes a message to the stream as a line
            ''' </summary>
            ''' <param name="message">The message to write</param>
            Friend Sub WriteLine(message As String)
                SyncLock _syncObject
                    _stream.WriteLine(message)
                End SyncLock
            End Sub

            ''' <summary>
            ''' Increments the reference count for the stream
            ''' </summary>
            Friend Sub AddReference()
                SyncLock _syncObject
                    _referenceCount += 1
                End SyncLock
            End Sub

            ''' <summary>
            ''' Flushes the stream
            ''' </summary>
            Friend Sub Flush()
                SyncLock _syncObject
                    _stream.Flush()
                End SyncLock
            End Sub

            ''' <summary>
            ''' Decrements the reference count to the stream and closes the stream if the reference count
            ''' is zero
            ''' </summary>
            Friend Sub CloseStream()
                SyncLock _syncObject
                    Try
                        _referenceCount -= 1
                        _stream.Flush()
                        Debug.Assert(_referenceCount >= 0, "Ref count is below 0")
                    Finally
                        If _referenceCount <= 0 Then
                            _stream.Close()
                            _stream = Nothing
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
                    Return _stream IsNot Nothing
                End Get
            End Property

            ''' <summary>
            ''' The size of the log file
            ''' </summary>
            ''' <value>The size</value>
            Friend ReadOnly Property FileSize() As Long
                Get
                    Return _stream.BaseStream.Length
                End Get
            End Property

            ''' <summary>
            ''' Ensures the stream is closed (flushed) no matter how we are closed
            ''' </summary>
            ''' <param name="disposing">Indicates who called dispose</param>
            Private Overloads Sub Dispose(disposing As Boolean)
                If disposing Then
                    If Not _disposed Then
                        If _stream IsNot Nothing Then
                            _stream.Close()
                        End If
                        _disposed = True
                    End If
                End If
            End Sub

            ''' <summary>
            ''' Standard implementation of IDisposable
            ''' </summary>
            Public Overloads Sub Dispose() Implements IDisposable.Dispose
                ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
                Dispose(True)
                GC.SuppressFinalize(Me)
            End Sub

            ''' <summary>
            ''' Ensures stream is closed at GC
            ''' </summary>
            Protected Overrides Sub Finalize()
                ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
                Dispose(False)
                MyBase.Finalize()
            End Sub

            ' The stream that does the writing
            Private _stream As StreamWriter

            ' The number of FileLogTraceListeners using the stream
            Private _referenceCount As Integer

            ' Used for synchronizing writing and reference counting
            Private ReadOnly _syncObject As Object = New Object

            ' Indicates whether or not the object has been disposed
            Private _disposed As Boolean

        End Class 'ReferencedStream

    End Class 'FileLogTraceListener

End Namespace
