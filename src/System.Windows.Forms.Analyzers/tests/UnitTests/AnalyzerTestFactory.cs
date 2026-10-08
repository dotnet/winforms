// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.VisualBasic.Testing;

namespace System.Windows.Forms.Analyzers.Tests;

/// <summary>
///  Creates consistently configured Roslyn analyzer and code-fix tests.
/// </summary>
public static class AnalyzerTestFactory
{
    /// <summary>
    ///  Creates a C# analyzer test.
    /// </summary>
    public static CSharpAnalyzerTest<TAnalyzer, DefaultVerifier> CreateCSharpAnalyzerTest<TAnalyzer>(
        AnalyzerTestCase testCase)
        where TAnalyzer : DiagnosticAnalyzer, new()
    {
        ArgumentNullException.ThrowIfNull(testCase);

        CSharpAnalyzerTest<TAnalyzer, DefaultVerifier> test = new()
        {
            ReferenceAssemblies = testCase.ReferenceAssemblies
        };

        Configure(test.TestState, testCase);

        return test;
    }

    /// <summary>
    ///  Creates a Visual Basic analyzer test.
    /// </summary>
    public static VisualBasicAnalyzerTest<TAnalyzer, DefaultVerifier> CreateVisualBasicAnalyzerTest<TAnalyzer>(
        AnalyzerTestCase testCase)
        where TAnalyzer : DiagnosticAnalyzer, new()
    {
        ArgumentNullException.ThrowIfNull(testCase);

        VisualBasicAnalyzerTest<TAnalyzer, DefaultVerifier> test = new()
        {
            ReferenceAssemblies = testCase.ReferenceAssemblies
        };

        Configure(test.TestState, testCase);

        return test;
    }

    /// <summary>
    ///  Creates a C# analyzer and code-fix test.
    /// </summary>
    public static CSharpCodeFixTest<TAnalyzer, TCodeFix, DefaultVerifier> CreateCSharpCodeFixTest<TAnalyzer, TCodeFix>(
        AnalyzerTestCase testCase)
        where TAnalyzer : DiagnosticAnalyzer, new()
        where TCodeFix : CodeFixProvider, new()
    {
        ArgumentNullException.ThrowIfNull(testCase);

        CSharpCodeFixTest<TAnalyzer, TCodeFix, DefaultVerifier> test = new()
        {
            ReferenceAssemblies = testCase.ReferenceAssemblies,
            NumberOfFixAllIterations = testCase.NumberOfFixAllIterations
        };

        Configure(test.TestState, testCase);
        ConfigureFixedState(test.FixedState, testCase);

        return test;
    }

    /// <summary>
    ///  Creates a Visual Basic analyzer and code-fix test.
    /// </summary>
    public static VisualBasicCodeFixTest<TAnalyzer, TCodeFix, DefaultVerifier> CreateVisualBasicCodeFixTest<TAnalyzer, TCodeFix>(
        AnalyzerTestCase testCase)
        where TAnalyzer : DiagnosticAnalyzer, new()
        where TCodeFix : CodeFixProvider, new()
    {
        ArgumentNullException.ThrowIfNull(testCase);

        VisualBasicCodeFixTest<TAnalyzer, TCodeFix, DefaultVerifier> test = new()
        {
            ReferenceAssemblies = testCase.ReferenceAssemblies,
            NumberOfFixAllIterations = testCase.NumberOfFixAllIterations
        };

        Configure(test.TestState, testCase);
        ConfigureFixedState(test.FixedState, testCase);

        return test;
    }

    private static void Configure(SolutionState state, AnalyzerTestCase testCase)
    {
        state.OutputKind = testCase.OutputKind;

        foreach (AnalyzerTestSource source in testCase.Sources)
        {
            state.Sources.Add((NormalizeSourcePath(source.Path), source.Source));
        }

        foreach (string additionalReference in testCase.AdditionalReferences)
        {
            state.AdditionalReferences.Add(additionalReference);
        }

        state.AdditionalFiles.AddRange(testCase.AdditionalFiles);
        foreach ((string path, string content) in testCase.AnalyzerConfigFiles)
        {
            state.AnalyzerConfigFiles.Add((path, content));
        }

        state.ExpectedDiagnostics.AddRange(testCase.ExpectedDiagnostics);
    }

    private static void ConfigureFixedState(SolutionState state, AnalyzerTestCase testCase)
    {
        state.OutputKind = testCase.OutputKind;

        foreach (AnalyzerTestSource source in testCase.FixedSources)
        {
            state.Sources.Add((NormalizeSourcePath(source.Path), source.Source));
        }

        foreach (string additionalReference in testCase.AdditionalReferences)
        {
            state.AdditionalReferences.Add(additionalReference);
        }
    }

    private static string NormalizeSourcePath(string path)
        => Path.IsPathRooted(path) ? path : $"/0/{path}";
}
