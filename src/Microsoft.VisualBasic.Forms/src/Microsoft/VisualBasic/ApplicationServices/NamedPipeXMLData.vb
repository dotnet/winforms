' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Imports System.Runtime.Serialization

Namespace Microsoft.VisualBasic.ApplicationServices

    <DataContract>
    Friend NotInheritable Class NamedPipeXmlData
        <DataMember>
        Public Property CommandLineArguments As String()
    End Class

End Namespace
