' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Namespace Microsoft.VisualBasic.Logging

    Partial Public Class FileLogTraceListener

        ' Attribute keys used to access properties set in the config file
        Private Const KEY_APPEND As String = "append"
        Private Const KEY_APPEND_PASCAL As String = "Append"
        Private Const KEY_AUTOFLUSH As String = "autoflush"
        Private Const KEY_AUTOFLUSH_CAMEL As String = "autoFlush"
        Private Const KEY_AUTOFLUSH_PASCAL As String = "AutoFlush"
        Private Const KEY_BASEFILENAME As String = "basefilename"
        Private Const KEY_BASEFILENAME_CAMEL As String = "baseFilename"
        Private Const KEY_BASEFILENAME_CAMEL_ALT As String = "baseFileName"
        Private Const KEY_BASEFILENAME_PASCAL As String = "BaseFilename"
        Private Const KEY_BASEFILENAME_PASCAL_ALT As String = "BaseFileName"
        Private Const KEY_CUSTOMLOCATION As String = "customlocation"
        Private Const KEY_CUSTOMLOCATION_CAMEL As String = "customLocation"
        Private Const KEY_CUSTOMLOCATION_PASCAL As String = "CustomLocation"
        Private Const KEY_DELIMITER As String = "delimiter"
        Private Const KEY_DELIMITER_PASCAL As String = "Delimiter"
        Private Const KEY_DISKSPACEEXHAUSTEDBEHAVIOR As String = "diskspaceexhaustedbehavior"
        Private Const KEY_DISKSPACEEXHAUSTEDBEHAVIOR_CAMEL As String = "diskSpaceExhaustedBehavior"
        Private Const KEY_DISKSPACEEXHAUSTEDBEHAVIOR_PASCAL As String = "DiskSpaceExhaustedBehavior"
        Private Const KEY_ENCODING As String = "encoding"
        Private Const KEY_ENCODING_PASCAL As String = "Encoding"
        Private Const KEY_INCLUDEHOSTNAME As String = "includehostname"
        Private Const KEY_INCLUDEHOSTNAME_CAMEL As String = "includeHostName"
        Private Const KEY_INCLUDEHOSTNAME_PASCAL As String = "IncludeHostName"
        Private Const KEY_LOCATION As String = "location"
        Private Const KEY_LOCATION_PASCAL As String = "Location"
        Private Const KEY_LOGFILECREATIONSCHEDULE As String = "logfilecreationschedule"
        Private Const KEY_LOGFILECREATIONSCHEDULE_CAMEL As String = "logFileCreationSchedule"
        Private Const KEY_LOGFILECREATIONSCHEDULE_PASCAL As String = "LogFileCreationSchedule"
        Private Const KEY_MAXFILESIZE As String = "maxfilesize"
        Private Const KEY_MAXFILESIZE_CAMEL As String = "maxFileSize"
        Private Const KEY_MAXFILESIZE_PASCAL As String = "MaxFileSize"
        Private Const KEY_RESERVEDISKSPACE As String = "reservediskspace"
        Private Const KEY_RESERVEDISKSPACE_CAMEL As String = "reserveDiskSpace"
        Private Const KEY_RESERVEDISKSPACE_PASCAL As String = "ReserveDiskSpace"

        ' A list of supported attributes
        Private ReadOnly _supportedAttributes() As String = New String() {
            KEY_APPEND, KEY_APPEND_PASCAL, KEY_AUTOFLUSH, KEY_AUTOFLUSH_PASCAL,
            KEY_AUTOFLUSH_CAMEL, KEY_BASEFILENAME, KEY_BASEFILENAME_PASCAL,
            KEY_BASEFILENAME_CAMEL, KEY_BASEFILENAME_PASCAL_ALT,
            KEY_BASEFILENAME_CAMEL_ALT, KEY_CUSTOMLOCATION, KEY_CUSTOMLOCATION_PASCAL,
            KEY_CUSTOMLOCATION_CAMEL, KEY_DELIMITER, KEY_DELIMITER_PASCAL,
            KEY_DISKSPACEEXHAUSTEDBEHAVIOR, KEY_DISKSPACEEXHAUSTEDBEHAVIOR_PASCAL,
            KEY_DISKSPACEEXHAUSTEDBEHAVIOR_CAMEL, KEY_ENCODING, KEY_ENCODING_PASCAL,
            KEY_INCLUDEHOSTNAME, KEY_INCLUDEHOSTNAME_PASCAL, KEY_INCLUDEHOSTNAME_CAMEL,
            KEY_LOCATION, KEY_LOCATION_PASCAL, KEY_LOGFILECREATIONSCHEDULE,
            KEY_LOGFILECREATIONSCHEDULE_PASCAL, KEY_LOGFILECREATIONSCHEDULE_CAMEL,
            KEY_MAXFILESIZE, KEY_MAXFILESIZE_PASCAL, KEY_MAXFILESIZE_CAMEL,
            KEY_RESERVEDISKSPACE, KEY_RESERVEDISKSPACE_PASCAL, KEY_RESERVEDISKSPACE_CAMEL}

    End Class
End Namespace
