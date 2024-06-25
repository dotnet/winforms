' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Namespace Microsoft.VisualBasic.CompilerServices

    <ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)>
    Public NotInheritable Class HostServices

        Private Shared s_host As IVbHost

        Public Shared Property VBHost() As IVbHost
            Get
                Return s_host
            End Get

            Set(Value As IVbHost)
                s_host = Value
            End Set
        End Property

    End Class
End Namespace
