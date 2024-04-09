' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Namespace Microsoft.VisualBasic.Forms.Tests
    Friend Module TestUtilities

        Friend Function GetUniqueText() As String
            Return Guid.NewGuid().ToString("D")
        End Function

    End Module
End Namespace
