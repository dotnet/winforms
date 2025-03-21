// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.WinForms.Utilities.Shared;

namespace Microsoft.WinForms.Test;

/// <summary>
///  Provides a base class for leveraging Roslyn analyzers and code fixes within unit tests.
/// </summary>
/// <remarks>
///  <para>
///   This class discovers test files used by Roslyn analyzer tests by automatically
///   fetching them into the test context. It handles reading file content, grouping related
///   files, and preparing them for valid test scenarios. The class integrates with 
///   CSharpAnalyzerTest{TAnalyzer,TVerifier} and CSharpCodeFixTest{TAnalyzer,TCodeFix,TVerifier}
///   to simplify test setup and verification.
///  </para>
///  <para>
///   Inherit from this class specifying the analyzer and verifier types (and optionally a 
///   code fix provider when testing code fixes). Override or call the provided methods to customize
///   the test data usage for various scenarios. 
///  </para>
/// </remarks>
/// <typeparam name="TAnalyzer">
///  The analyzer under test.
/// </typeparam>
/// <typeparam name="TVerifier">
///  The type used to verify the analyzer's output.
/// </typeparam>
public abstract partial class RoslynAnalyzerAndCodeFixTestBase<TAnalyzer, TVerifier>
    where TAnalyzer : DiagnosticAnalyzer, new()
    where TVerifier : IVerifier, new()
{
    /// <summary>
    ///  Holds the file paths for the analyzer test files discovered from the test base path.
    /// </summary>
    private static IEnumerable<string> s_analyzerTestFilePaths = null!;

    /// <summary>
    ///  The source language to use for the tests.
    /// </summary>
    public SourceLanguage Language { get; }

    /// <summary>
    ///  Initializes a new instance of the 
    ///  RoslynAnalyzerAndCodeFixTestBase{TAnalyzer, TVerifier} class.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   The constructor extracts all valid test file paths from the specified test base path.
    ///   The test base path is automatically populated using the caller file path.
    ///  </para>
    ///  <para>
    ///   Ensure that test files adhere to naming and directory conventions to be discovered accurately.
    ///  </para>
    /// </remarks>
    /// <param name="language">
    ///  The source language of the test files.
    /// </param>
    /// <param name="testBasePath">
    ///  The file path used as the base location to load test files. This parameter is automatically 
    ///  populated by the CallerFilePath attribute.
    /// </param>
    protected RoslynAnalyzerAndCodeFixTestBase(SourceLanguage language, [CallerFilePath] string? testBasePath = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(testBasePath, nameof(testBasePath));
        s_analyzerTestFilePaths = TestFileLoader.GetTestFilePaths(GetType(), testBasePath);
        Language = language;
    }

    private RoslynAnalyzerAndCodeFixTestBase() { }

    /// <summary>
    ///  Retrieves file sets as an enumerable of object arrays used for data-driven tests.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This method transforms test file paths into an enumerable format suitable for
    ///   theory data or other forms of parameterized testing.
    ///  </para>
    /// </remarks>
    /// <returns>
    ///  An enumerable of object arrays, each containing a test file path.
    /// </returns>
    public static IEnumerable<object> GetFileSets() =>
        GetTestFiles(s_analyzerTestFilePaths);

    /// <summary>
    ///  Loads a matching set of test file contents from a specified location.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Scans the target path for analyzer tests, code-fix tests, and other necessary documents,
    ///   bundling them into a TestDataFileSet. This set is used by the test context to run analyzer
    ///   and code-fix tests comprehensively.
    ///  </para>
    /// </remarks>
    /// <param name="path">
    ///  The directory path in which to search for test files.
    /// </param>
    /// <returns>
    ///  A TestDataFileSet containing the content of all discovered test files.
    /// </returns>
    public static TestDataFileSet GetTestDataFileSet(string path)
    {
        TestDataFileSet testDataFileSet = new();

        foreach (var fileItem in TestFileLoader.EnumerateEntries(path))
        {
            string currentDocument = TestFileLoader.LoadTestFile(fileItem.FilePath);

            switch (fileItem.FileType)
            {
                case TestFileType.AnalyzerTestCode:
                    testDataFileSet.AnalyzerTestCode = currentDocument;
                    break;

                case TestFileType.CodeFixTestCode:
                    testDataFileSet.CodeFixTestCode = currentDocument;
                    break;

                case TestFileType.FixedTestCode:
                    testDataFileSet.FixedTestCode = currentDocument;
                    break;

                case TestFileType.GlobalUsing:
                    testDataFileSet.GlobalUsing = currentDocument;
                    break;

                case TestFileType.AdditionalCodeFile:
                    testDataFileSet.AdditionalCodeFiles.Add(currentDocument);
                    break;
            }
        }

        return testDataFileSet;
    }

    /// <summary>
    ///  Retrieves test file sets for a given test type.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Loads the test file paths using the provided test base path and groups them into a collection 
    ///   of TestDataFileSet instances.
    ///  </para>
    /// </remarks>
    /// <returns>
    ///  An enumerable of TestDataFileSet instances representing grouped test files.
    /// </returns>
    protected static IEnumerable<TestDataFileSet> GetTestFiles(IEnumerable<string> testFilePaths)
    {
        ArgumentNullException.ThrowIfNull(testFilePaths);

        List<TestDataFileSet> testDataFileSets = new();

        foreach (string analyzerTestFilePath in testFilePaths)
        {
            TestDataFileSet testDataFileSet = GetTestDataFileSet(analyzerTestFilePath);
            testDataFileSets.Add(testDataFileSet);
        }

        return testDataFileSets;
    }

    /// <summary>
    ///  Creates a test context configured with the expected fixed solution for code-fix tests.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Provides a ready-to-run CSharpAnalyzerTest instance containing the 'FixedTestCode'. 
    ///   This context is used to verify that applying the code fix transforms the source code as 
    ///   expected.
    ///  </para>
    ///  <para>
    ///   Use this method in scenarios where the code fix should result in a specific final code output.
    ///  </para>
    /// </remarks>
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
    protected CSharpAnalyzerTest<TAnalyzer, TVerifier> GetFixedTestContext(
        TestDataFileSet fileSet,
        ReferenceAssemblies referenceAssemblies,
        [CallerMemberName] string? memberName = default) =>
            GetTestContext(
                fileSet.FixedTestCode,
                fileSet.GlobalUsing,
                fileSet.AdditionalCodeFiles,
                referenceAssemblies,
                memberName);

    /// <summary>
    ///  Creates a test context for analyzer scenarios using unaltered test code.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Sets up a CSharpAnalyzerTest instance that utilizes the 'AnalyzerTestCode'. This context 
    ///   is useful for initial diagnostics verification before any code fix is applied.
    ///  </para>
    ///  <para>
    ///   It excludes fixed code, focusing purely on analyzer diagnostics in the provided file set.
    ///  </para>
    /// </remarks>
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
    protected CSharpAnalyzerTest<TAnalyzer, TVerifier> GetAnalyzerTestContext(
        TestDataFileSet fileSet,
        ReferenceAssemblies referenceAssemblies,
        [CallerMemberName] string? memberName = default) =>
            GetTestContext(
                fileSet.AnalyzerTestCode,
                fileSet.GlobalUsing,
                fileSet.AdditionalCodeFiles,
                referenceAssemblies,
                memberName);

    /// <summary>
    ///  Generates a basic test context for analyzer testing using the provided source code and references.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Creates a base CSharpAnalyzerTest instance configured with the test code and output kind
    ///   set to WindowsApplication. It adds any global using directives and additional source documents.
    ///  </para>
    ///  <para>
    ///   This helper method serves as the foundation for both analyzer tests and code-fix tests,
    ///   ensuring that the test environment is properly set up.
    ///  </para>
    /// </remarks>
    /// <param name="fileContent">
    ///  The primary source code to be analyzed.
    /// </param>
    /// <param name="globalUsing">
    ///  Global using directives to include in the test environment.
    /// </param>
    /// <param name="contextDocuments">
    ///  Additional source documents relevant to the test scenario.
    /// </param>
    /// <param name="referenceAssemblies">
    ///  The reference assemblies required by the test code.
    /// </param>
    /// <param name="memberName">
    ///  Optional. The caller member name for context or logging.
    /// </param>
    /// <returns>
    ///  A configured CSharpAnalyzerTest instance.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///  Thrown if the main test file content is null or empty.
    /// </exception>
    protected CSharpAnalyzerTest<TAnalyzer, TVerifier> GetTestContext(
        string fileContent,
        string globalUsing,
        IEnumerable<string> contextDocuments,
        ReferenceAssemblies referenceAssemblies,
        [CallerMemberName] string? memberName = default)
    {
        if (string.IsNullOrEmpty(fileContent))
        {
            throw new ArgumentException(
                $"Test method '{memberName}' passed a test file without content.");
        }

        CSharpAnalyzerTest<TAnalyzer, TVerifier> context
            = new CSharpAnalyzerTest<TAnalyzer, TVerifier>
            {
                TestCode = fileContent,
                TestState =
                {
                    OutputKind = OutputKind.WindowsApplication,
                },
                ReferenceAssemblies = referenceAssemblies
            };

        if (globalUsing is not null)
        {
            context.TestState.Sources.Add(globalUsing);
        }

        if (contextDocuments is not null)
        {
            foreach (string contextDocument in contextDocuments)
            {
                context.TestState.Sources.Add(contextDocument);
            }
        }

        return context;
    }

    /// <summary>
    ///  Creates a code-fix test context for applying a code fix and verifying the transformation.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Provides scaffolding to test how a code fix provider modifies the source code. It sets the
    ///   initial test code (with expected diagnostic spans) and the fixed code to compare against.
    ///  </para>
    ///  <para>
    ///   The method also configures the number of 'Fix All' iterations permitted in a single document.
    ///  </para>
    /// </remarks>
    /// <typeparam name="TCodeFix">
    ///  The code fix provider to be tested.
    /// </typeparam>
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
    ///  A CSharpCodeFixTest instance configured for code-fix testing.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///  Thrown if the expected CodeFixTestCode or FixedTestCode cannot be found in the file set.
    /// </exception>
    protected CSharpCodeFixTest<TAnalyzer, TCodeFix, TVerifier> GetCodeFixTestContext<TCodeFix>(
        TestDataFileSet fileSet,
        ReferenceAssemblies referenceAssemblies,
        int numberOfFixAllIterations,
        [CallerMemberName] string? memberName = default)
        where TCodeFix : CodeFixProvider, new()
    {
        CSharpCodeFixTest<TAnalyzer, TCodeFix, TVerifier> context = new CSharpCodeFixTest<TAnalyzer, TCodeFix, TVerifier>
        {
            TestCode = fileSet.CodeFixTestCode
                ?? throw new ArgumentException(
                    $"Test method '{memberName}' expected the test file " +
                    $"'CodeFixTestCode.cs' which could not be found."),

            FixedCode = fileSet.FixedTestCode
                ?? throw new ArgumentException(
                    $"Test method '{memberName}' expected the test file " +
                    $"'FixedTestCode.cs' which could not be found."),

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
