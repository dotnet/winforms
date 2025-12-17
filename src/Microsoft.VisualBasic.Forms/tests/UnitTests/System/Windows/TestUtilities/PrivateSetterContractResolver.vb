' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Reflection
Imports System.Text.Json
Imports System.Text.Json.Serialization.Metadata

Namespace Microsoft.VisualBasic.Forms.Tests
    Public Class PrivateSetterContractResolver
        Inherits DefaultJsonTypeInfoResolver

        Public Overrides Function GetTypeInfo(type As Type, options As JsonSerializerOptions) As JsonTypeInfo
            Dim jsonTypeInfo As JsonTypeInfo = MyBase.GetTypeInfo(type, options)

            If jsonTypeInfo.Kind = JsonTypeInfoKind.Object Then
                For Each [property] As JsonPropertyInfo In jsonTypeInfo.Properties
                    [property].Set = Function(obj, value)
                                         Dim prop As PropertyInfo = type.GetProperty(
                                             [property].Name,
                                             BindingFlags.Public _
                                                 Or BindingFlags.NonPublic _
                                                 Or BindingFlags.Instance _
                                                 Or BindingFlags.IgnoreCase)
                                         prop.SetValue(obj, value, Nothing)
                                         Return True
                                     End Function
                Next
            End If

            Return jsonTypeInfo
        End Function
    End Class
End Namespace
