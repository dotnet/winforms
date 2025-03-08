// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;

namespace Microsoft.WinForms.Test;

/// <summary>
///  Represents code fragments and related data for analyzer and code fix tests.
/// </summary>
/// <remarks>
///  <para>
///   This class collects essential code sections for testing analyzers and code fixes.
///   It holds the initial code for analysis, the code used for applying a fix, and
///   the resulting code after the fix is applied.
///  </para>
///  <para>
///   It also supports adding extra code files and a global using directive,
///   facilitating more complex and realistic testing scenarios.
///  </para>
/// </remarks>
public abstract partial class RoslynAnalyzerAndCodeFixTestBase<TAnalyzer, TVerifier>
    where TAnalyzer : DiagnosticAnalyzer, new()
    where TVerifier : IVerifier, new()
{
    /// <summary>
    ///  Holds code fragments used in analyzer and code fix tests.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This class defines properties for test code inputs and their final
    ///   fixed outputs. It also allows additional code files and a global
    ///   using directive to be included in testing.
    ///  </para>
    ///  <para>
    ///   By combining all these code fragments, it helps ensure that
    ///   analyzers and code fixes are tested in a realistic and
    ///   comprehensive manner.
    ///  </para>
    /// </remarks>
    public class TestDataFileSet
    {
        /// <summary>
        ///  Gets or sets the code for analysis in analyzer tests.
        /// </summary>
        public string AnalyzerTestCode { get; set; } = string.Empty;

        /// <summary>
        ///  Gets or sets the code in which the code fix is tested.
        /// </summary>
        public string CodeFixTestCode { get; set; } = string.Empty;

        /// <summary>
        ///  Gets or sets the final code after the code fix is applied.
        /// </summary>
        public string FixedTestCode { get; set; } = string.Empty;

        /// <summary>
        ///  Gets or sets a global using directive for the tests.
        /// </summary>
        public string GlobalUsing { get; set; } = string.Empty;

        /// <summary>
        ///  Gets or sets any additional code files required by tests.
        /// </summary>
        public List<string> AdditionalCodeFiles { get; set; } = [];
    }
}
