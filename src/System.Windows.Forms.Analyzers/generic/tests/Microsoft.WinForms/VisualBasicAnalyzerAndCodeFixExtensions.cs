// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.VisualBasic.Testing;

namespace Microsoft.WinForms.Test;

/// <summary>
///  Extension methods for Visual Basic analyzer and code fix testing.
/// </summary>
/// <remarks>
///  <para>
///   Provides extension methods for the RoslynAnalyzerAndCodeFixTestBase class
///   to support Visual Basic language test scenarios.
///  </para>
/// </remarks>
public static class VisualBasicAnalyzerAndCodeFixExtensions
{
    /// <summary>
    ///  Creates a test context for analyzer scenarios using Visual Basic test code.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Sets up a VisualBasicAnalyzerTest instance that utilizes the 'AnalyzerTestCode'.
    ///   This context is useful for initial diagnostics verification before any code fix is applied.
    ///  </para>
    /// </remarks>
    /// <typeparam name="TAnalyzer">
    ///  The analyzer under test.
    /// </typeparam>
    /// <typeparam name="TVerifier">
    ///  The type used to verify the analyzer's output.
    /// </typeparam>
    /// <param name="testBase">
    ///  The base test class instance.
    /// </param>
    /// <param name="fileSet">
    ///  The set of test files containing source code for diagnostics analysis.
    /// </param>
    /// <param name="referenceAssemblies">
    ///  The reference assemblies needed for compiling the test code.
    /// </param>
    /// <param name="memberName">
    ///  Optional. The caller member name for diagnostic context.
    /// </param>
    /// <returns>
    ///  A test context that verifies diagnostics produced by the analyzer.
    /// </returns>
    public static VisualBasicAnalyzerTest<TAnalyzer, TVerifier> GetVisualBasicAnalyzerTestContext<TAnalyzer, TVerifier>(
        this RoslynAnalyzerAndCodeFixTestBase<TAnalyzer, TVerifier> testBase,
        RoslynAnalyzerAndCodeFixTestBase<TAnalyzer, TVerifier>.TestDataFileSet fileSet,
        ReferenceAssemblies referenceAssemblies,
        [CallerMemberName] string? memberName = default)
        where TAnalyzer : DiagnosticAnalyzer, new()
        where TVerifier : IVerifier, new()
    {
        if (string.IsNullOrEmpty(fileSet.AnalyzerTestCode))
        {
            throw new ArgumentException(
                $"Test method '{memberName}' passed a test file without content.");
        }

        VisualBasicAnalyzerTest<TAnalyzer, TVerifier> context = new()
        {
            TestCode = fileSet.AnalyzerTestCode,
            TestState =
            {
                OutputKind = OutputKind.WindowsApplication,
            },
            ReferenceAssemblies = referenceAssemblies
        };

        if (fileSet.GlobalUsing is not null)
        {
            context.TestState.Sources.Add(fileSet.GlobalUsing);
        }

        if (fileSet.AdditionalCodeFiles is not null)
        {
            foreach (string contextDocument in fileSet.AdditionalCodeFiles)
            {
                context.TestState.Sources.Add(contextDocument);
            }
        }

        return context;
    }

    /// <summary>
    ///  Creates a test context configured with the expected fixed solution for Visual Basic code-fix tests.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Provides a ready-to-run VisualBasicAnalyzerTest instance containing the 'FixedTestCode'.
    ///   This context is used to verify that applying the code fix transforms the source code as
    ///   expected.
    ///  </para>
    /// </remarks>
    /// <typeparam name="TAnalyzer">
    ///  The analyzer under test.
    /// </typeparam>
    /// <typeparam name="TVerifier">
    ///  The type used to verify the analyzer's output.
    /// </typeparam>
    /// <param name="testBase">
    ///  The base test class instance.
    /// </param>
    /// <param name="fileSet">
    ///  The set of test files containing various code fragments for the fix.
    /// </param>
    /// <param name="referenceAssemblies">
    ///  The reference assemblies needed by the test code's compilation.
    /// </param>
    /// <param name="memberName">
    ///  Optional. The caller member name for diagnostic or logging context.
    /// </param>
    /// <returns>
    ///  A test context prepared with fixed code, references, and state.
    /// </returns>
    public static VisualBasicAnalyzerTest<TAnalyzer, TVerifier> GetVisualBasicFixedTestContext<TAnalyzer, TVerifier>(
        this RoslynAnalyzerAndCodeFixTestBase<TAnalyzer, TVerifier> testBase,
        RoslynAnalyzerAndCodeFixTestBase<TAnalyzer, TVerifier>.TestDataFileSet fileSet,
        ReferenceAssemblies referenceAssemblies,
        [CallerMemberName] string? memberName = default)
        where TAnalyzer : DiagnosticAnalyzer, new()
        where TVerifier : IVerifier, new()
    {
        if (string.IsNullOrEmpty(fileSet.FixedTestCode))
        {
            throw new ArgumentException(
                $"Test method '{memberName}' passed a test file without content.");
        }

        VisualBasicAnalyzerTest<TAnalyzer, TVerifier> context = new()
        {
            TestCode = fileSet.FixedTestCode,
            TestState =
            {
                OutputKind = OutputKind.WindowsApplication,
            },
            ReferenceAssemblies = referenceAssemblies
        };

        if (fileSet.GlobalUsing is not null)
        {
            context.TestState.Sources.Add(fileSet.GlobalUsing);
        }

        if (fileSet.AdditionalCodeFiles is not null)
        {
            foreach (string contextDocument in fileSet.AdditionalCodeFiles)
            {
                context.TestState.Sources.Add(contextDocument);
            }
        }

        return context;
    }

    /// <summary>
    ///  Creates a code-fix test context for applying a code fix and verifying the transformation in Visual Basic.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Provides scaffolding to test how a code fix provider modifies the Visual Basic source code. It sets the
    ///   initial test code (with expected diagnostic spans) and the fixed code to compare against.
    ///  </para>
    ///  <para>
    ///   The method also configures the number of 'Fix All' iterations permitted in a single document.
    ///  </para>
    /// </remarks>
    /// <typeparam name="TAnalyzer">
    ///  The analyzer under test.
    /// </typeparam>
    /// <typeparam name="TCodeFix">
    ///  The code fix provider to be tested.
    /// </typeparam>
    /// <typeparam name="TVerifier">
    ///  The type used to verify the analyzer's output.
    /// </typeparam>
    /// <param name="testBase">
    ///  The base test class instance.
    /// </param>
    /// <param name="fileSet">
    ///  The set of test files containing the code fix test input and expected outputs.
    /// </param>
    /// <param name="referenceAssemblies">
    ///  The reference assemblies needed by the test code.
    /// </param>
    /// <param name="numberOfFixAllIterations">
    ///  The number of allowed 'Fix All' iterations for the document.
    /// </param>
    /// <param name="memberName">
    ///  Optional. The caller member name for context or logging.
    /// </param>
    /// <returns>
    ///  A VisualBasicCodeFixTest instance configured for code-fix testing.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///  Thrown if the expected CodeFixTestCode or FixedTestCode cannot be found in the file set.
    /// </exception>
    public static VisualBasicCodeFixTest<TAnalyzer, TCodeFix, TVerifier> GetVisualBasicCodeFixTestContext<TAnalyzer, TCodeFix, TVerifier>(
        this RoslynAnalyzerAndCodeFixTestBase<TAnalyzer, TVerifier> testBase,
        RoslynAnalyzerAndCodeFixTestBase<TAnalyzer, TVerifier>.TestDataFileSet fileSet,
        ReferenceAssemblies referenceAssemblies,
        int numberOfFixAllIterations,
        [CallerMemberName] string? memberName = default)
        where TAnalyzer : DiagnosticAnalyzer, new()
        where TCodeFix : CodeFixProvider, new()
        where TVerifier : IVerifier, new()
    {
        VisualBasicCodeFixTest<TAnalyzer, TCodeFix, TVerifier> context = new()
        {
            TestCode = fileSet.CodeFixTestCode
                ?? throw new ArgumentException(
                    $"Test method '{memberName}' expected the test file " +
                    $"'CodeFixTestCode.vb' which could not be found."),

            FixedCode = fileSet.FixedTestCode
                ?? throw new ArgumentException(
                    $"Test method '{memberName}' expected the test file " +
                    $"'FixedTestCode.vb' which could not be found."),

            TestState =
            {
                OutputKind = OutputKind.WindowsApplication,
            },

            ReferenceAssemblies = referenceAssemblies,
            NumberOfFixAllInDocumentIterations = numberOfFixAllIterations
        };

        // Include global using directives in both TestState and FixedState.
        if (fileSet.GlobalUsing is not null)
        {
            context.TestState.Sources.Add(fileSet.GlobalUsing);
            context.FixedState.Sources.Add(fileSet.GlobalUsing);
        }

        // Include additional code files in both TestState and FixedState.
        if (fileSet.AdditionalCodeFiles is not null)
        {
            foreach (string contextDocument in fileSet.AdditionalCodeFiles)
            {
                context.TestState.Sources.Add(contextDocument);
                context.FixedState.Sources.Add(contextDocument);
            }
        }

        return context;
    }
}
