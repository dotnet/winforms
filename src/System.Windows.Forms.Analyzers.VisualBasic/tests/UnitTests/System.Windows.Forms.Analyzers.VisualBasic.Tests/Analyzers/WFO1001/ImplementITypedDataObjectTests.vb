' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Windows.Forms.Analyzers.Diagnostics
Imports System.Windows.Forms.VisualBasic.Analyzers.ImplementITypedDataObjectInAdditionToIDataObject
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Testing
Imports Microsoft.CodeAnalysis.VisualBasic.Testing
Imports Xunit

Public NotInheritable Class ImplementITypedDataObjectTests

    Private Const DiagnosticId As String = DiagnosticIDs.ImplementITypedDataObject

    <Fact>
    Public Async Function UntypedInterface() As Task
        ' internal class UntypedInterface :IDataObject
        Dim input As String = Await TestFileLoader.GetVBAnalyzerTestCodeAsync()
        Await RaiseTheWarning(input, New List(Of DiagnosticResult) From {
            DiagnosticResult.CompilerWarning(DiagnosticId).WithSpan(8, 18, 8, 18 + NameOf(UntypedInterface).Length) _
                .WithArguments(NameOf(UntypedInterface))
        })
    End Function

    <Fact>
    Public Async Function DerivedFromUntyped() As Task
        ' internal class DerivedFromUntyped : UntypedInterface
        Dim input As String = Await TestFileLoader.GetVBAnalyzerTestCodeAsync()
        Await RaiseTheWarning(input, New List(Of DiagnosticResult) From {
            DiagnosticResult.CompilerWarning(DiagnosticId).WithSpan(8, 33, 8, 33 + NameOf(DerivedFromUntyped).Length) _
                .WithArguments(NameOf(DerivedFromUntyped)),
            DiagnosticResult.CompilerWarning(DiagnosticId).WithSpan(15, 18, 15, 18 + NameOf(UntypedInterface).Length) _
                .WithArguments(NameOf(UntypedInterface))
        })
    End Function

    <Fact>
    Public Async Function UntypedWithAlias() As Task
        ' internal class UntypedWithAlias : IManagedDataObject
        Dim input As String = Await TestFileLoader.GetVBAnalyzerTestCodeAsync()
        Await RaiseTheWarning(input, New List(Of DiagnosticResult) From {
            DiagnosticResult.CompilerWarning(DiagnosticId).WithSpan(8, 18, 8, 18 + NameOf(UntypedWithAlias).Length) _
                .WithArguments(NameOf(UntypedWithAlias))
        })
    End Function

    <Fact>
    Public Async Function UntypedWithNamespace() As Task
        ' internal class UntypedWithNamespace :Forms.IDataObject
        Dim input As String = Await TestFileLoader.GetVBAnalyzerTestCodeAsync()
        Await RaiseTheWarning(input, New List(Of DiagnosticResult) From {
            DiagnosticResult.CompilerWarning(DiagnosticId).WithSpan(8, 18, 8, 18 + NameOf(UntypedWithNamespace).Length) _
                .WithArguments(NameOf(UntypedWithNamespace))
        })
    End Function

    <Fact>
    Public Async Function UntypedUnimplemented() As Task
        ' internal class UntypedUnimplemented :IDataObject
        Dim input As String = Await TestFileLoader.GetVBAnalyzerTestCodeAsync()
        Await RaiseTheWarning(input, New List(Of DiagnosticResult) From {
            DiagnosticResult.CompilerWarning(DiagnosticId) _
                .WithSpan(9, 18, 9, 18 + NameOf(UntypedUnimplemented).Length) _
                .WithArguments(NameOf(UntypedUnimplemented)),
            DiagnosticResult.CompilerError("BC30149").WithSpan(10, 20, 10, 31) _
                .WithArguments("Class", "UntypedUnimplemented", "Function GetData(format As String, autoConvert As Boolean) As Object", "IDataObject")
        })
    End Function

    <Fact>
    Public Async Function TypedInterface() As Task
        ' internal class TypedInterface :ITypedDataObject
        Dim input As String = Await TestFileLoader.GetVBAnalyzerTestCodeAsync()
        Await NoWarning(input)
    End Function

    <Fact>
    Public Async Function TypedWithNamespace() As Task
        ' internal class TypedWithNamespace : Forms.ITypedDataObject
        Dim input As String = Await TestFileLoader.GetVBAnalyzerTestCodeAsync()
        Await NoWarning(input)
    End Function

    <Fact>
    Public Async Function TypedWithAlias() As Task
        ' internal class TypedWithAlias : IManagedDataObject, System.Windows.Forms.IDataObject
        Dim input As String = Await TestFileLoader.GetVBAnalyzerTestCodeAsync()
        Await NoWarning(input)
    End Function

    <Fact>
    Public Async Function TwoInterfaces() As Task
        ' internal class TwoInterfaces :IDataObject, ITypedDataObject
        Dim input As String = Await TestFileLoader.GetVBAnalyzerTestCodeAsync()
        Await NoWarning(input)
    End Function

    <Fact>
    Public Async Function UnrelatedIDataObject() As Task
        ' Name collision, this analyzer is not applicable
        Dim input As String = Await TestFileLoader.GetVBAnalyzerTestCodeAsync()
        Await NoWarning(input)
    End Function

    Private Shared Async Function RaiseTheWarning(input As String, diagnostics As List(Of DiagnosticResult)) As Task
        Dim context = CreateContext(input)
        context.TestState.ExpectedDiagnostics.AddRange(diagnostics)

        Await context.RunAsync().ConfigureAwait(False)
    End Function

    Private Shared Async Function NoWarning(input As String) As Task
        Await CreateContext(input).RunAsync().ConfigureAwait(False)
    End Function

    Private Shared Function CreateContext(input As String) As VisualBasicAnalyzerTest(Of ImplementITypedDataObjectInAdditionToIDataObjectAnalyzer, DefaultVerifier)
        Assert.NotNull(CurrentReferences.NetCoreAppReferences)
        Assert.NotNull(CurrentReferences.WinFormsRefPath)

        Dim context As New VisualBasicAnalyzerTest(Of ImplementITypedDataObjectInAdditionToIDataObjectAnalyzer, DefaultVerifier) With {
            .TestCode = input,
            .ReferenceAssemblies = CurrentReferences.NetCoreAppReferences
        }

        context.TestState.OutputKind = OutputKind.DynamicallyLinkedLibrary
        context.TestState.AdditionalReferences.Add(CurrentReferences.WinFormsRefPath)

        Return context
    End Function

End Class
