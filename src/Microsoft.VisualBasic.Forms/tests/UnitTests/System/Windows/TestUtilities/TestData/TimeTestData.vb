' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports Xunit

Namespace Microsoft.VisualBasic.Forms.Tests

    Public Class TimeTestData
        Inherits TheoryData(Of DualTimeZones)

#Disable Warning CA1825 ' Avoid zero-length array allocations
        Public Sub New()
            Add(New DualTimeZones(TimeZoneNames.GMT))
            Add(New DualTimeZones(TimeZoneNames.Local))
        End Sub
#Enable Warning CA1825 ' Avoid zero-length array allocations

    End Class
End Namespace
