' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports Microsoft.VisualBasic.ApplicationServices

Namespace Microsoft.VisualBasic.Forms.Tests

    Public Class ShutdownModeData
        Implements IEnumerable(Of Object())

        Public Iterator Function GetEnumerator() As IEnumerator(Of Object()) Implements IEnumerable(Of Object()).GetEnumerator
            For Each mode As AuthenticationMode In [Enum].GetValues(Of ShutdownMode)
                Yield {mode}
            Next
        End Function

        Public Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Return GetEnumerator()
        End Function

    End Class
End Namespace
