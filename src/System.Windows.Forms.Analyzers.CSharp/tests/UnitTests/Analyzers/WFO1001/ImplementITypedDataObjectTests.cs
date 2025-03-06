// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.Analyzers.Diagnostics;
using System.Windows.Forms.CSharp.Analyzers.ImplementITypedDataObject;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;

namespace System.Windows.Forms.Analyzers.Tests;

public sealed class ImplementITypedDataObjectTests
{
    private const string DiagnosticId = DiagnosticIDs.ImplementITypedDataObject;

    [Fact]
    public async Task UntypedInterface()
    {
        // internal class UntypedInterface :IDataObject
        string input = await TestFileLoader.GetCSAnalyzerTestCodeAsync();
        await RaiseTheWarning(
            input,
            [
                DiagnosticResult.CompilerWarning(DiagnosticId)
                    .WithSpan(8, 16, 8, 16 + nameof(UntypedInterface).Length)
                    .WithArguments(nameof(UntypedInterface))
            ]);
    }

    [Fact]
    public async Task DerivedFromUntyped()
    {
        // internal class DerivedFromUntyped : UntypedInterface
        string input = await TestFileLoader.GetCSAnalyzerTestCodeAsync();
        await RaiseTheWarning(
            input,
            [
                DiagnosticResult.CompilerWarning(DiagnosticId)
                    .WithSpan(8, 23, 8, 23 + nameof(DerivedFromUntyped).Length)
                    .WithArguments(nameof(DerivedFromUntyped)),
                DiagnosticResult.CompilerWarning(DiagnosticId)
                    .WithSpan(13, 16, 13, 16 + nameof(UntypedInterface).Length)
                    .WithArguments(nameof(UntypedInterface)),
            ]);
    }

    [Fact]
    public async Task UntypedWithAlias()
    {
        // internal class UntypedWithAlias : IManagedDataObject
        string input = await TestFileLoader.GetCSAnalyzerTestCodeAsync();
        await RaiseTheWarning(
            input,
            [
                DiagnosticResult.CompilerWarning(DiagnosticId)
                    .WithSpan(8, 16, 8, 16 + nameof(UntypedWithAlias).Length)
                    .WithArguments(nameof(UntypedWithAlias))
            ]);
    }

    [Fact]
    public async Task UntypedWithNamespace()
    {
        // internal class UntypedWithNamespace :Forms.IDataObject
        string input = await TestFileLoader.GetCSAnalyzerTestCodeAsync();
        await RaiseTheWarning(
            input,
            [
                DiagnosticResult.CompilerWarning(DiagnosticId)
                    .WithSpan(8, 16, 8, 16 + nameof(UntypedWithNamespace).Length)
                    .WithArguments(nameof(UntypedWithNamespace))
            ]);
    }

    [Fact]
    public async Task UntypedUnimplemented()
    {
        // internal class UntypedUnimplemented :IDataObject
        string input = await TestFileLoader.GetCSAnalyzerTestCodeAsync();
        await RaiseTheWarning(input,
            [
                DiagnosticResult.CompilerWarning(DiagnosticId)
                    .WithSpan(8, 16, 8, 16 + nameof(UntypedUnimplemented).Length)
                    .WithArguments(nameof(UntypedUnimplemented)),
                DiagnosticResult.CompilerError("CS0535")
                    .WithSpan(8, 39, 8, 39 + "IDataObject".Length)
                    .WithArguments($"System.Windows.Forms.Analyzers.CSharp.Tests.{nameof(UntypedUnimplemented)}", "System.Windows.Forms.IDataObject.GetData(string, bool)"),
            ]);
    }

    [Fact]
    public async Task TypedInterface()
    {
        // internal class TypedInterface :ITypedDataObject
        string input = await TestFileLoader.GetCSAnalyzerTestCodeAsync();
        await NoWarning(input);
    }

    [Fact]
    public async Task TypedWithNamespace()
    {
        // internal class TypedWithNamespace : Forms.ITypedDataObject
        string input = await TestFileLoader.GetCSAnalyzerTestCodeAsync();
        await NoWarning(input);
    }

    [Fact]
    public async Task TypedWithAlias()
    {
        // internal class TypedWithAlias : IManagedDataObject, System.Windows.Forms.IDataObject
        string input = await TestFileLoader.GetCSAnalyzerTestCodeAsync();
        await NoWarning(input);
    }

    [Fact]
    public async Task TwoInterfaces()
    {
        // internal class TwoInterfaces :IDataObject, ITypedDataObject
        string input = await TestFileLoader.GetCSAnalyzerTestCodeAsync();
        await NoWarning(input);
    }

    [Fact]
    public async Task UnrelatedIDataObject()
    {
        // Name collision, this analyzer is not applicable
        string input = await TestFileLoader.GetCSAnalyzerTestCodeAsync();
        await NoWarning(input);
    }

    private static async Task RaiseTheWarning(string input, List<DiagnosticResult> diagnostics)
    {
        var context = CreateContext(input);
        context.TestState.ExpectedDiagnostics.AddRange(diagnostics);

        await context.RunAsync().ConfigureAwait(false);
    }

    private static async Task NoWarning(string input) => await CreateContext(input).RunAsync().ConfigureAwait(false);

    private static CSharpAnalyzerTest<ImplementITypedDataObjectAnalyzer, DefaultVerifier> CreateContext(string input)
    {
        Assert.NotNull(CurrentReferences.NetCoreAppReferences);
        Assert.True(File.Exists(CurrentReferences.WinFormsRefPath));

        CSharpAnalyzerTest<ImplementITypedDataObjectAnalyzer, DefaultVerifier> context = new()
        {
            TestCode = input,
            TestState =
            {
                OutputKind = OutputKind.DynamicallyLinkedLibrary,
                AdditionalReferences = { CurrentReferences.WinFormsRefPath }
            },
            ReferenceAssemblies = CurrentReferences.NetCoreAppReferences
        };

        return context;
    }
}
