' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Option Strict Off

Imports System.Reflection
Imports FluentAssertions
Imports Microsoft.VisualBasic.ApplicationServices
Imports Xunit

Namespace Microsoft.VisualBasic.Forms.Tests

    Partial Public Class WindowsFormsApplicationBaseTests
        Inherits WindowsFormsApplicationBase

        <WinFormsFact>
        Public Sub PropertiesUsingDynamic()
            Dim testAccessor As ITestAccessor = GetType(WindowsFormsApplicationBase).TestAccessor()
            Dim applicationInstanceID As String = testAccessor.Dynamic.GetApplicationInstanceID(Assembly.GetCallingAssembly)
            Dim guidOutput As Guid = Nothing
            Guid.TryParse(applicationInstanceID, guidOutput).Should.BeTrue()
        End Sub

        <WinFormsTheory>
        <ClassData(GetType(AuthenticationModeData))>
        Public Sub ValidateAuthenticationModeEnumValues(mode As AuthenticationMode)
            Dim testAccessor As ITestAccessor = GetType(WindowsFormsApplicationBase).TestAccessor()
            Dim testCode As Action =
                Sub()
                    testAccessor.Dynamic.ValidateAuthenticationModeEnumValue(mode, NameOf(AuthenticationMode))
                End Sub

            testCode.Should.NotThrow()
        End Sub

    End Class
End Namespace
