﻿' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Text
Imports Xunit

Namespace Microsoft.VisualBasic.Forms.Tests

    Partial Public Class AssemblyPublicKeyIsAsExpectedTest
        Private Const AssemblyPK As String = "002400000480000094000000060200000024000052534131000400000100010007d1fa57c4aed9f0a32e84aa0faefd0de9e8fd6aec8f87fb03766c834c99921eb23be79ad9d5dcc1dd9ad236132102900b723cf980957fc4e177108fc607774f29e8320e92ea05ece4e821c0a5efe8f1645c4c0c93c1ab99285d622caa652c1dfad63d745d6f2de5f17e5eaf0fc4963d261c8a12436518206dc093344d5ad293"

        ''' <summary>
        '''  Test to find out a correct PublicKey in InternalsVisibleTo for assembly under test.
        ''' </summary>
        <WinFormsFact>
        Public Sub AssemblyPublicKeyIsAsExpected()
            Dim publicKey() As Byte = GetType(NetworkTests).Assembly.GetName().GetPublicKey()
            Dim sb = New StringBuilder()
            For Each [byte] As Byte In publicKey
                sb.AppendFormat("{0:x2}", [byte])
            Next [byte]

            ' Set breakpoint here to find out what's in sb
            Assert.Equal(AssemblyPK, sb.ToString())
        End Sub

    End Class

End Namespace
