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
///   This class is responsible for discovering test files likewise used by Roslyn analyzer tests
///   and automatically fetching them into the test context. Its methods handle reading content
///   from physical files, grouping them, and preparing them for valid test scenarios.
///  </para>
///  <para>Usage:</para>
///  <list type="bullet">
///   <item><description>Inherit from this base class, specifying the analyzer and verifier types.</description></item>
///   <item><description>Call or override provided methods to customize how the test data is used in your test scenarios.</description></item>
///   <item><description>
///    The class seamlessly integrates with <see cref="CSharpAnalyzerTest{TAnalyzer,TVerifier}"/>
///    and <see cref="CSharpCodeFixTest{TAnalyzer,TCodeFix,TVerifier}"/> to configure test expectations and references.
///   </description></item>
///  </list>
/// </remarks>
/// <typeparam name="TAnalyzer">The analyzer under test.</typeparam>
/// <typeparam name="TVerifier">The type that verifies the analyzer's output.</typeparam>
public abstract partial class RoslynAnalyzerAndCodeFixTestBase<TAnalyzer, TVerifier>
    where TAnalyzer : DiagnosticAnalyzer, new()
    where TVerifier : IVerifier, new()
{
    private readonly IEnumerable<string> _analyzerTestFilePaths;

    /// <summary>
    ///  Initializes a new instance of the <see cref="RoslynAnalyzerAndCodeFixTestBase{TAnalyzer, TVerifier}"/> class.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   The constructor extracts all valid test file paths from the specified <paramref name="testBasePath"/>.
    ///  </para>
    ///  <para>
    ///   This path is automatically populated using the caller file path.
    ///   Ensure that test files follow the naming and directory conventions so they can be discovered accurately.
    ///  </para>
    /// </remarks>
    /// <param name="testBasePath">The path containing the analyzer/fixture test files.</param>
    protected RoslynAnalyzerAndCodeFixTestBase(SourceLanguage language, [CallerFilePath] string? testBasePath = null)
    {
        ArgumentNullException.ThrowIfNull(testBasePath);
        _analyzerTestFilePaths = TestFileLoader.GetTestFilePaths(GetType(), testBasePath);
    }

    /// <summary>
    ///  Loads a matching set of test file content from a specified location.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This method scans the target path for analyzer test files, code-fix files, and any necessary
    ///   additional documents, then bundles them into a <see cref="TestDataFileSet"/>. This combination
    ///   of files can be used by the test context to run analyzer or code-fix tests thoroughly.
    ///  </para>
    /// </remarks>
    /// <param name="path">The path to search for test files.</param>
    /// <returns>A <see cref="TestDataFileSet"/> holding the contents of all discovered files.</returns>
    public async Task<TestDataFileSet> GetTestDataFileSetAsync(string path)
    {
        TestDataFileSet testDataFileSet = new();

        foreach (var fileItem in TestFileLoader.EnumerateEntries(path))
        {
            string currentDocument = await TestFileLoader.LoadTestFileAsync(fileItem.FilePath)
                .ConfigureAwait(false);

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
    ///  Asynchronously enumerates all discovered test files and executes the specified test delegate.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This method is a convenient way to apply the same test logic over multiple sets of test data.
    ///   It loops over the discovered file paths, obtains a <see cref="TestDataFileSet"/> for each,
    ///   and invokes your specified <paramref name="testMethod"/> with it.
    ///  </para>
    /// </remarks>
    /// <param name="testMethod">The test method accepting test file data that will be evaluated.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected Task EnumerateTestFilesAsync(Func<TestDataFileSet, Task> testMethod)
    {
        return Task.WhenAll(_analyzerTestFilePaths.Select(async path =>
        {
            TestDataFileSet testDataFileSet = await GetTestDataFileSetAsync(path)
                .ConfigureAwait(false);

            await testMethod.Invoke(testDataFileSet)
                .ConfigureAwait(false);
        }));
    }

    /// <summary>
    ///  Creates a test context configured with the expected fixed solution for code-fix tests.
    /// </summary>
    /// <remarks>
    /// <para>
    ///  This method is intended to provide a ready-to-run <see cref="CSharpAnalyzerTest{TAnalyzer,TVerifier}"/>
    ///  instance containing the changes specified in the 'FixedTestCode'.
    /// </para>
    /// <para>
    ///  Use it in situations where you need to verify that a code fix successfully transforms
    ///  the original test code into the desired corrected form.
    /// </para>
    /// </remarks>
    /// <param name="fileSet">Holds various file contents used for the test.</param>
    /// <param name="referenceAssemblies">References needed by the test code's compilation.</param>
    /// <param name="memberName">Optional caller member name for diagnostic context.</param>
    /// <returns>A test context prepared with the fixed code, references, and state.</returns>
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
    ///  Creates a test context for the analyzer scenario using unaltered test code.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This method sets up a <see cref="CSharpAnalyzerTest{TAnalyzer,TVerifier}"/> that includes
    ///   the 'AnalyzerTestCode' content. It does not include any fixed code.
    ///  </para>
    ///  <para>
    ///   This is useful for discovering which diagnostics might appear in the given file set
    ///   before any code-fix is applied.
    ///  </para>
    /// </remarks>
    /// <param name="fileSet">Holds various file contents used for the test.</param>
    /// <param name="referenceAssemblies">References needed by the test code's compilation.</param>
    /// <param name="memberName">Optional caller member name for diagnostic context.</param>
    /// <returns>A test context that can verify diagnostics raised by the analyzer.</returns>
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
    ///  Generates a basic <see cref="CSharpAnalyzerTest{TAnalyzer,TVerifier}"/>
    ///  using the provided test code and references.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This helper method creates a base test context and configures the
    ///   output kind to be a Windows application.
    ///  </para>
    ///  <para>
    ///   It automatically adds the specified <paramref name="globalUsing"/> statements and any additional
    ///   context documents. Helpful for verifying code diagnostics in multiple scenarios without rewriting test scaffolding.
    ///  </para>
    /// </remarks>
    /// <param name="fileContent">The main source content to analyze.</param>
    /// <param name="globalUsing">Global using directives to include in the test environment.</param>
    /// <param name="contextDocuments">Additional source documents relevant to the test scenario.</param>
    /// <param name="referenceAssemblies">References needed by the test code's compilation.</param>
    /// <param name="memberName">Optional caller member name for context or logging.</param>
    /// <returns>A configured <see cref="CSharpAnalyzerTest{TAnalyzer,TVerifier}"/> instance.</returns>
    /// <exception cref="ArgumentException">
    ///  Thrown if the main <paramref name="fileContent"/> is null or empty.
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

        if (contextDocuments is null)
        {
            return context;
        }

        foreach (string contextDocument in contextDocuments)
        {
            context.TestState.Sources.Add(contextDocument);
        }

        return context;
    }

    /// <summary>
    ///  Creates a code-fix test context for applying a <typeparamref name="TCodeFix"/>
    ///  to the original and verifying the transformed result.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This method provides the scaffolding to test how <typeparamref name="TCodeFix"/> modifies
    ///   the source code. It loads the initial test code (which includes expected diagnostic spans)
    ///   and sets the 'FixedCode' property to compare against the desired result. It also configures
    ///   the number of 'Fix All' iterations allowed in one document.
    ///  </para>
    /// </remarks>
    /// <typeparam name="TCodeFix">The code fix provider to test.</typeparam>
    /// <param name="fileSet">Holds various file contents used for the test scenario.</param>
    /// <param name="referenceAssemblies">References needed by the test code's compilation.</param>
    /// <param name="numberOfFixAllIterations">The number of 'Fix All' operations to apply for the test.</param>
    /// <param name="memberName">Optional caller member name for context or logging.</param>
    /// <returns>
    ///  A <see cref="CSharpCodeFixTest{TAnalyzer,TCodeFix,TVerifier}"/> initialized for code-fix testing.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///  Thrown if the expected 'CodeFixTestCode' or 'FixedTestCode' cannot be found in the <paramref name="fileSet"/>.
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

        // The FixedState should use the same additional Sources than the TestState.
        if (fileSet.GlobalUsing is not null)
        {
            context.TestState.Sources.Add(fileSet.GlobalUsing);
            context.FixedState.Sources.Add(fileSet.GlobalUsing);
        }

        // The FixedState should use the same additional Sources than the TestState,
        // which also includes any additional (support) files which just serve the
        // purpose to make the to-test-code compile in the first place.
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
