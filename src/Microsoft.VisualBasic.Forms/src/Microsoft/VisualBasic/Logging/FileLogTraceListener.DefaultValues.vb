' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Namespace Microsoft.VisualBasic.Logging
    Partial Public Class FileLogTraceListener
        Private Const DATE_FORMAT As String = "yyyy-MM-dd"

        Private Const DEFAULT_NAME As String = "FileLogTraceListener"

        Private Const FILE_EXTENSION As String = ".log"

        Private Const MAX_OPEN_ATTEMPTS As Integer = Integer.MaxValue

        ' The minimum setting allowed for maximum file size
        Private Const MIN_FILE_SIZE As Integer = 1000

        ' Delimiter used when converting a stack to a string
        Private Const STACK_DELIMITER As String = ", "

    End Class
End Namespace
