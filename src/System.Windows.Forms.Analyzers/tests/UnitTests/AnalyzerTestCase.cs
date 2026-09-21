// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Text;

namespace System.Windows.Forms.Analyzers.Tests;

/// <summary>
///  Represents a named source file in an analyzer test compilation.
/// </summary>
public readonly record struct AnalyzerTestSource(string Path, string Source);

/// <summary>
///  Describes the language-neutral inputs and expectations for an analyzer test.
/// </summary>
/// <remarks>
///  <para>
///   Language-specific factories translate this model into Roslyn's C# or Visual Basic
///   analyzer and code-fix test types.
///  </para>
/// </remarks>
public sealed class AnalyzerTestCase
{
    /// <summary>
    ///  Initializes a new analyzer test case.
    /// </summary>
    public AnalyzerTestCase(
        ReferenceAssemblies referenceAssemblies,
        params AnalyzerTestSource[] sources)
    {
        ArgumentNullException.ThrowIfNull(referenceAssemblies);
        ArgumentNullException.ThrowIfNull(sources);

        ReferenceAssemblies = referenceAssemblies;
        Sources.AddRange(sources);
    }

    /// <summary>
    ///  Gets the reference assemblies used to compile the test sources.
    /// </summary>
    public ReferenceAssemblies ReferenceAssemblies { get; }

    /// <summary>
    ///  Gets or sets the compilation output kind.
    /// </summary>
    public OutputKind OutputKind { get; set; } = OutputKind.DynamicallyLinkedLibrary;

    /// <summary>
    ///  Gets the analyzer input sources.
    /// </summary>
    public List<AnalyzerTestSource> Sources { get; } = [];

    /// <summary>
    ///  Gets the expected fixed sources for a code-fix test.
    /// </summary>
    public List<AnalyzerTestSource> FixedSources { get; } = [];

    /// <summary>
    ///  Gets additional metadata reference paths.
    /// </summary>
    public List<string> AdditionalReferences { get; } = [];

    /// <summary>
    ///  Gets additional non-source files.
    /// </summary>
    public List<(string Path, SourceText Content)> AdditionalFiles { get; } = [];

    /// <summary>
    ///  Gets analyzer configuration files.
    /// </summary>
    public List<(string Path, string Content)> AnalyzerConfigFiles { get; } = [];

    /// <summary>
    ///  Gets diagnostics expected from the test compilation.
    /// </summary>
    public List<DiagnosticResult> ExpectedDiagnostics { get; } = [];

    /// <summary>
    ///  Gets or sets the number of fix-all iterations for code-fix tests.
    /// </summary>
    public int NumberOfFixAllIterations { get; set; }
}
