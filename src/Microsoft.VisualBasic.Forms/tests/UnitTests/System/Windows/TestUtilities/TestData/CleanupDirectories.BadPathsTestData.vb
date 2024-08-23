' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Namespace Microsoft.VisualBasic.Forms.Tests

    Public Class BadPathsTestData
        Implements IEnumerable(Of Object())

        Public Iterator Function GetEnumerator() As IEnumerator(Of Object()) Implements IEnumerable(Of Object()).GetEnumerator
            Yield {Nothing}
            Yield {String.Empty}
            Yield {" "}
            Yield {IO.Path.GetPathRoot(IO.Path.GetTempPath)}
            Yield {IO.Path.GetTempPath}
        End Function

        Public Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Return GetEnumerator()
        End Function

    End Class
End Namespace
