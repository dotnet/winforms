' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Namespace Microsoft.VisualBasic.Logging

    Partial Public Class FileLogTraceListener

        ' Name to be used when parameterless constructor is called
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

        ' Identifies properties in the BitArray
        Private Const PROPERTY_COUNT As Integer = 12

        ' Indicates whether or not properties have been set
        ' Note: Properties that use _propertiesSet to track whether or not
        '       they've been set should always be set through the property setter and not
        '       by directly changing the corresponding private field.
        Private ReadOnly _propertiesSet As New BitArray(PROPERTY_COUNT, False)

    End Class
End Namespace
