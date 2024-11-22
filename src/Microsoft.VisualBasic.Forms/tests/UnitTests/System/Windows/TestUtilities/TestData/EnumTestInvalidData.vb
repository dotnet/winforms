' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Namespace Microsoft.VisualBasic.Forms.Tests

    Public Class EnumTestInvalidData(Of T)
        Implements IEnumerable(Of Object())

        Public Iterator Function GetEnumerator() As IEnumerator(Of Object()) Implements IEnumerable(Of Object()).GetEnumerator
            Yield {New EnumValueAndThrowIndicatorData(Of T)(CType(CObj(-1), T), True)}
            Yield {New EnumValueAndThrowIndicatorData(Of T)(CType(CObj(Integer.MaxValue), T), True)}
        End Function

        Public Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Return GetEnumerator()
        End Function

    End Class

End Namespace
