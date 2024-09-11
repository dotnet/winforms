﻿' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports Xunit

Namespace Microsoft.VisualBasic.Forms.Tests

    Public Class TimeTestData
        Implements IEnumerable(Of Object())

        Public Iterator Function GetEnumerator() As IEnumerator(Of Object()) Implements IEnumerable(Of Object()).GetEnumerator
            Yield {New DualTimeZones(TimeZone.GMT)}
            Yield {New DualTimeZones(TimeZone.Local)}
        End Function

        Public Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Return GetEnumerator()
        End Function

    End Class
End Namespace
