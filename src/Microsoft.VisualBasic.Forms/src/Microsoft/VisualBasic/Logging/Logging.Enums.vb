' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Namespace Microsoft.VisualBasic.Logging

    ''' <summary>
    '''  Options for behavior when resources are exhausted.
    ''' </summary>
    Public Enum DiskSpaceExhaustedOption As Integer
        ThrowException
        DiscardMessages
    End Enum

    ''' <summary>
    '''  Options for the date stamp in the name of a log file.
    ''' </summary>
    Public Enum LogFileCreationScheduleOption As Integer
        None        '(default)
        Daily       'YYYY-MM-DD for today
        Weekly      'YYYY-MM-DD for first day of this week
    End Enum

    ''' <summary>
    '''  Options for the location of a log's directory.
    ''' </summary>
    Public Enum LogFileLocation As Integer

        ' Changes to this enum must be reflected in ValidateLogfileLocationEnumValue()
        TempDirectory
        LocalUserApplicationDirectory
        CommonApplicationDirectory
        ExecutableDirectory
        Custom
    End Enum

End Namespace
