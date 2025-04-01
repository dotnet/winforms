// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.Analyzers.Diagnostics;
using System.Windows.Forms.Analyzers.Tests.Microsoft.WinForms;
using System.Windows.Forms.CSharp.Analyzers.MissingPropertySerializationConfiguration;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.WinForms.Test;
using Microsoft.WinForms.Utilities.Shared;

namespace System.Windows.Forms.Analyzers.CSharp.Tests.AnalyzerTests.MissingPropertySerializationConfiguration;

/// <summary>
///  Tests specific edge cases for the MissingPropertySerializationConfigurationAnalyzer:
///  - Static properties which should not get flagged
///  - Properties in classes implementing non-System.ComponentModel.IComponent interfaces
///  - Properties with private setters
///  - Inherited properties that are already attributed correctly
/// </summary>
public class EdgeCaseScenarios
    : RoslynAnalyzerAndCodeFixTestBase<MissingPropertySerializationConfigurationAnalyzer, DefaultVerifier>
{
    /// <summary>
    ///  Initializes a new instance of the <see cref="EdgeCaseScenarios"/> class.
    /// </summary>
    public EdgeCaseScenarios()
        : base(SourceLanguage.CSharp)
    {
    }
    
    /// <summary>
    ///  Retrieves reference assemblies for the latest target framework versions.
    /// </summary>
    public static IEnumerable<object[]> GetReferenceAssemblies()
    {
        NetVersion[] tfms =
        [
             NetVersion.Net6_0,
             NetVersion.Net7_0,
             NetVersion.Net8_0,

            // In this case, we're saying, we want to use .NET 9, but instead of using
            // the Desktop-Package which comes with it, we take the BuildOutput from the
            // this repo's artifacts folder.
            NetVersion.WinFormsBuild
        ];

        foreach (ReferenceAssemblies refAssembly in ReferenceAssemblyGenerator.GetForLatestTFMs(tfms))
        {
            yield return new object[] { refAssembly };
        }
    }

    /// <summary>
    ///  Tests that the analyzer correctly handles edge cases:
    ///  - Not flagging static properties
    ///  - Not flagging properties in classes that implement non-System.ComponentModel.IComponent
    ///  - Not flagging properties with private setters
    ///  - Not flagging overridden properties when the base is properly attributed
    /// </summary>
    [Theory]
    [CodeTestData(nameof(GetReferenceAssemblies))]
    public async Task TestAnalyzerDiagnostics(
        ReferenceAssemblies referenceAssemblies,
        TestDataFileSet fileSet)
    {
        var context = GetAnalyzerTestContext(fileSet, referenceAssemblies);

        string diagnosticId = DiagnosticIDs.MissingPropertySerializationConfiguration;

        // We expect no diagnostics for these edge cases
        context.ExpectedDiagnostics.Clear();

        context.ExpectedDiagnostics.Add(
            DiagnosticResult
                .CompilerError(diagnosticId)
                .WithSpan(87, 23, 87, 38));

        await context.RunAsync();
    }
}
