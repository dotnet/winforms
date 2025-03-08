﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.CSharp.Analyzers.MissingPropertySerializationConfiguration;
using System.Windows.Forms.CSharp.CodeFixes.AddDesignerSerializationVisibility;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.WinForms.Test;
using Microsoft.WinForms.Utilities.Shared;

namespace System.Windows.Forms.Analyzers.CSharp.Tests.AnalyzerTests.MissingPropertySerializationConfiguration;

/// <summary>
///  Represents a set of test scenarios for custom controls to verify
///  property serialization behavior.
/// </summary>
/// <remarks>
///  <para>
///   This class is derived from <see cref="RoslynAnalyzerAndCodeFixTestBase{TAnalyzer, TVerifier}"/>"/>
///   and is intended to validate how properties are serialized in custom controls during
///   analyzer and code-fix operations.
///  </para>
///  <para>
///   Use the provided methods to test scenarios such as missing property serialization and to
///   confirm that the code fix provider can correct those issues.
///  </para>
/// </remarks>
public class CustomControlScenarios
    : RoslynAnalyzerAndCodeFixTestBase<MissingPropertySerializationConfigurationAnalyzer, DefaultVerifier>
{
    /// <summary>
    ///  Initializes a new instance of the <see cref="CustomControlScenarios"/> class.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Calls the base constructor with the C# source language to set up the
    ///   environment for analyzer tests.
    ///  </para>
    /// </remarks>
    public CustomControlScenarios()
        : base(SourceLanguage.CSharp)
    {
    }

    /// <summary>
    ///  Retrieves a collection of reference assemblies for test scenarios.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This returns .NET 9.0 reference assemblies to ensure that the
    ///   analyzer uses APIs introduced in .NET 9.0 and above.
    ///  </para>
    /// </remarks>
    public static IEnumerable<object[]> GetReferenceAssemblies()
    {
        yield return [ReferenceAssemblies.Net.Net90Windows];
    }

    /// <summary>
    ///  Tests the diagnostics produced by
    ///  <see cref="MissingPropertySerializationConfigurationAnalyzer"/>.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   First, it verifies that the initial diagnostics match the
    ///   expected results. Then, it re-runs the test context to confirm
    ///   that any corrections remain consistent.
    ///  </para>
    ///  <para>
    ///   This method depends on the provided file set and reference
    ///   assemblies to generate the appropriate environment for analysis.
    ///  </para>
    /// </remarks>
    [Theory]
    [CodeTestData(nameof(GetReferenceAssemblies))]
    public async Task TestDiagnostics(
        ReferenceAssemblies referenceAssemblies,
        TestDataFileSet fileSet)
    {
        var context = GetAnalyzerTestContext(fileSet, referenceAssemblies);
        await context.RunAsync();

        context = GetFixedTestContext(fileSet, referenceAssemblies);
        await context.RunAsync();
    }

    /// <summary>
    ///  Tests the code-fix provider to ensure it correctly applies designer serialization attributes.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This uses the <see cref="AddDesignerSerializationVisibilityCodeFixProvider"/>
    ///   to verify that the fix properly addresses missing or invalid property serialization attributes.
    ///  </para>
    ///  <para>
    ///   It also configures the code-fix test behavior to skip fix-all
    ///   checks at the project and solution level, focusing on the
    ///   document-level fix itself.
    ///  </para>
    /// </remarks>
    [Theory]
    [CodeTestData(nameof(GetReferenceAssemblies))]
    public async Task TestCodeFix(
        ReferenceAssemblies referenceAssemblies,
        TestDataFileSet fileSet)
    {
        var context = GetCodeFixTestContext<AddDesignerSerializationVisibilityCodeFixProvider>(
            fileSet,
            referenceAssemblies,
            numberOfFixAllIterations: -2);

        context.CodeFixTestBehaviors =
            CodeFixTestBehaviors.SkipFixAllInProjectCheck |
            CodeFixTestBehaviors.SkipFixAllInSolutionCheck;

        await context.RunAsync();
    }
}
