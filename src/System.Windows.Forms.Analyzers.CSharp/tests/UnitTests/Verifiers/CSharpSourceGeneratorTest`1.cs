// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using static Microsoft.CodeAnalysis.Testing.ReferenceAssemblies;

namespace System.Windows.Forms.Analyzers.Tests;

public class CSharpSourceGeneratorTest<TSourceGenerator> : CSharpSourceGeneratorTest<TSourceGenerator, XUnitVerifier>
    where TSourceGenerator : ISourceGenerator, new()
{
    public CSharpSourceGeneratorTest()
    {
        ReferenceAssemblies = NetFramework.Net472.WindowsForms;
    }
}
