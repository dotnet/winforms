// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.VisualBasic;
using Xunit;

namespace System.Windows.Forms.Analyzers.Tests;

/// <summary>
///  Verifies that analyzer compilations use the repository's configured targeting pack.
/// </summary>
public class CurrentReferencesTests
{
    [Fact]
    public void ParseRuntimeConfiguration_UsesRuntimeVersion()
    {
        (string tfm, string version) = CurrentReferences.ParseRuntimeConfiguration(
            """
            {"runtimeOptions":{"tfm":"net11.0","framework":{"name":"Microsoft.NETCore.App","version":"11.0.0-rc.1.123"}}}
            """);

        Assert.Equal("net11.0", tfm);
        Assert.Equal("11.0.0-rc.1.123", version);
    }

    [Theory]
    [InlineData("{}")]
    [InlineData("""{"runtimeOptions":{"tfm":"net11.0"}}""")]
    public void ParseRuntimeConfiguration_MissingMetadata_Throws(string configuration)
        => Assert.Throws<InvalidOperationException>(() => CurrentReferences.ParseRuntimeConfiguration(configuration));

    [Fact]
    public void NetCoreAppReferences_UseTestRuntimeConfiguration()
    {
        string configurationPath = Path.ChangeExtension(
            typeof(CurrentReferences).Assembly.Location,
            ".runtimeconfig.json");

        (string tfm, string version) = CurrentReferences.ParseRuntimeConfiguration(File.ReadAllText(configurationPath));

        Assert.Equal(tfm, CurrentReferences.Tfm);
        Assert.Equal(version, CurrentReferences.NetCoreRefsVersion);
        Assert.Equal(version, CurrentReferences.NetCoreAppReferences?.ReferenceAssemblyPackage?.Version);
    }

    [Theory]
    [InlineData(LanguageNames.CSharp)]
    [InlineData(LanguageNames.VisualBasic)]
    [SkipOnArchitecture(TestArchitectures.X86, "Analyzer tests hit OutOfMemoryException on x86 due to memory-mapped NuGet package extraction")]
    public async Task NetCoreAppReferences_CompileWithWinFormsReference(string language)
    {
        Assert.NotNull(CurrentReferences.NetCoreAppReferences);
        Assert.True(File.Exists(CurrentReferences.WinFormsRefPath));

        ImmutableArray<MetadataReference> references = (await CurrentReferences.NetCoreAppReferences.ResolveAsync(
            language,
            TestContext.Current.CancellationToken))
            .Add(MetadataReference.CreateFromFile(CurrentReferences.WinFormsRefPath));

        Compilation compilation = language == LanguageNames.CSharp
            ? CSharpCompilation.Create(
                "ReferenceCompatibility",
                [CSharpSyntaxTree.ParseText(
                    "class Test : System.Windows.Forms.Control { }",
                    cancellationToken: TestContext.Current.CancellationToken)],
                references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
            : VisualBasicCompilation.Create(
                "ReferenceCompatibility",
                [VisualBasicSyntaxTree.ParseText(
                    "Class Test\nInherits System.Windows.Forms.Control\nEnd Class",
                    cancellationToken: TestContext.Current.CancellationToken)],
                references,
                new VisualBasicCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        Assert.Empty(compilation.GetDiagnostics(TestContext.Current.CancellationToken)
            .Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error));
    }
}
