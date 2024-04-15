' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Globalization
Imports System.Threading

Namespace Microsoft.VisualBasic.CompilerServices

    ' Purpose: various helpers for the vb runtime functions
    Friend NotInheritable Class Utils1
        Friend Shared Function GetCultureInfo() As CultureInfo
            Return Thread.CurrentThread.CurrentCulture
        End Function

        Friend Shared Function GetResourceString(resourceKey As String, ParamArray args() As String) As String
            Return String.Format(GetCultureInfo(), resourceKey, args)
        End Function

        Friend Shared Function GetResourceString(ResourceId As Integer) As String
            Dim id As String = $"ID{ResourceId}"
            ' always return a string
            Return $"{SR.GetResourceString(id, id)}"
        End Function
    End Class
End Namespace
