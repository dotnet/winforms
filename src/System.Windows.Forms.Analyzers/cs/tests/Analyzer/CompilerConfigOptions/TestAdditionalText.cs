// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace System.Windows.Forms.Analyzers.Tests;

// Borrowed from https://github.com/dotnet/roslyn/blob/main/src/Compilers/Test/Core/Mocks/TestAdditionalText.cs

[ExcludeFromCodeCoverage]
public sealed class TestAdditionalText : AdditionalText
{
    private readonly SourceText _text;

    public TestAdditionalText(string path, SourceText text)
    {
        Path = path;
        _text = text;
    }

    public TestAdditionalText(string text = "", Encoding? encoding = null, string path = "dummy")
        : this(path, SourceText.From(text, encoding))
    {
    }

    public override string Path { get; }

    public override SourceText GetText(CancellationToken cancellationToken = default) => _text;
}
