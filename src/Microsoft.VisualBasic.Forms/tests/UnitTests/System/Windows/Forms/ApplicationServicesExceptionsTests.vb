' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.IO
Imports FluentAssertions
Imports Microsoft.VisualBasic.ApplicationServices
Imports Xunit

Imports VbUtils = Microsoft.VisualBasic.CompilerServices.ExceptionUtils

Namespace Microsoft.VisualBasic.Forms.Tests

    Public Class ApplicationServicesExceptionsTests

        <WinFormsFact>
        Public Sub NewCantStartSingleInstanceException()
            Dim testCode As Action =
                Sub()
                    Throw New CantStartSingleInstanceException()
                End Sub

            Dim expectedWildcardPattern As String =
                VbUtils.GetResourceString(SR.AppModel_SingleInstanceCantConnect)
            testCode.Should.Throw(Of CantStartSingleInstanceException)() _
                .WithMessage(expectedWildcardPattern)

            testCode =
                Sub()
                    Throw New CantStartSingleInstanceException("Test")
                End Sub
            testCode.Should.Throw(Of CantStartSingleInstanceException)().WithMessage("Test")

            testCode =
                Sub()
                    Throw New CantStartSingleInstanceException("Test1", New DirectoryNotFoundException)
                End Sub
            testCode.Should.Throw(Of CantStartSingleInstanceException)() _
                .WithMessage("Test1") _
                .WithInnerException(Of DirectoryNotFoundException)()
        End Sub

        <WinFormsFact>
        Public Sub NewNoStartupFormException()
            Dim testCode As Action =
                Sub()
                    Throw New NoStartupFormException()
                End Sub

            Dim expectedWildcardPattern As String =
                VbUtils.GetResourceString(SR.AppModel_NoStartupForm)
            testCode.Should.Throw(Of NoStartupFormException)() _
                .WithMessage(expectedWildcardPattern)

            testCode =
                Sub()
                    Throw New NoStartupFormException("Test")
                End Sub
            testCode.Should.Throw(Of NoStartupFormException)().WithMessage("Test")

            testCode =
                Sub()
                    Throw New NoStartupFormException("Test1", New DirectoryNotFoundException)
                End Sub
            testCode.Should.Throw(Of NoStartupFormException)() _
                .WithMessage("Test1") _
                .WithInnerException(Of DirectoryNotFoundException)()
        End Sub

    End Class
End Namespace
