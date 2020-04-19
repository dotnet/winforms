' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Imports System.Globalization

Namespace Microsoft.VisualBasic.CompilerServices

    ' Purpose: various helpers for the vb runtime functions
    Friend NotInheritable Class Utils

        Friend Shared Function GetResourceString(ResourceId As vbErrors) As String
            Dim id As String = "ID" & CStr(ResourceId)
            Return SR.GetResourceString(id, id)
        End Function

        Friend Shared Function GetResourceString(resourceKey As String, ParamArray args() As String) As String
            Return String.Format(GetCultureInfo(), resourceKey, args)
        End Function

        Friend Shared Function GetCultureInfo() As CultureInfo
            Return System.Threading.Thread.CurrentThread.CurrentCulture
        End Function

    End Class

End Namespace

