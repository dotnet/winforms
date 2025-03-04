// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace System.Windows.Forms.Analyzers.Tests;

// Borrowed from https://github.com/dotnet/roslyn/blob/main/src/Compilers/Core/Portable/DiagnosticAnalyzer/AnalyzerConfigOptions.cs

[ExcludeFromCodeCoverage]
internal sealed class CompilerAnalyzerConfigOptions : AnalyzerConfigOptions
{
    public static CompilerAnalyzerConfigOptions Empty { get; } = new(ImmutableDictionary.Create<string, string>());

    private readonly ImmutableDictionary<string, string> _backing;

    public CompilerAnalyzerConfigOptions(ImmutableDictionary<string, string> properties)
    {
        _backing = properties;
    }

    public override bool TryGetValue(string key, [NotNullWhen(true)] out string? value) => _backing.TryGetValue(key, out value);
}
