// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;

namespace Microsoft.WinForms.Test;

public abstract partial class RoslynAnalyzerAndCodeFixTestBase<TAnalyzer, TVerifier>
    where TAnalyzer : DiagnosticAnalyzer, new()
    where TVerifier : IVerifier, new()
{
    public class TestDataFileSet
    {
        public string AnalyzerTestCode { get; set; } = string.Empty;
        public string CodeFixTestCode { get; set; } = string.Empty;
        public string FixedTestCode { get; set; } = string.Empty;
        public string GlobalUsing { get; set; } = string.Empty;
        public List<string> AdditionalCodeFiles { get; set; } = [];
    }
}
