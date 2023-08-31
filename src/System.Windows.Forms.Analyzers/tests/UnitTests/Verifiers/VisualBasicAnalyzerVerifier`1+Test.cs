// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Microsoft.CodeAnalysis.VisualBasic.Testing;

namespace System.Windows.Forms.Analyzers.Tests;

public static partial class VisualBasicAnalyzerVerifier<TAnalyzer>
    where TAnalyzer : DiagnosticAnalyzer, new()
{
    public class Test : VisualBasicAnalyzerTest<TAnalyzer, XUnitVerifier>
    {
        public Test()
        {
            ReferenceAssemblies = ReferenceAssemblies.NetFramework.Net472.WindowsForms;
        }
    }
}
