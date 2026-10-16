// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Xunit;

namespace System.Windows.Forms.Analyzers.Tests;

/// <summary>
///  Verifies that analyzer compilations use the configured SDK's runtime reference pack.
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
    public void ResolveReferencePackVersion_MultiplePreviews_SelectsExactMatch()
        => Assert.Equal("11.0.0-rc.1.123", CurrentReferences.ResolveReferencePackVersion(
            "11.0.0-rc.1.123",
            ["11.0.0-preview.1.123", "11.0.0-rc.1.123", "11.0.0-rc.2.123"]));

    [Fact]
    public void ResolveReferencePackVersion_MissingMatch_Throws()
    {
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            CurrentReferences.ResolveReferencePackVersion("11.0.0-rc.1.123", ["11.0.0-preview.1.123"]));

        Assert.Contains("11.0.0-rc.1.123", exception.Message);
    }
}
