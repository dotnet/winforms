// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.WinForms.Test;

/// <summary>
///  Specifies the type of test file.
/// </summary>
public enum TestFileType
{
    /// <summary>
    ///  Analyzer test code file.
    /// </summary>
    AnalyzerTestCode,

    /// <summary>
    ///  Code fix test code file.
    /// </summary>
    CodeFixTestCode,

    /// <summary>
    ///  Fixed code file.
    /// </summary>
    FixedCode,

    /// <summary>
    ///  Global using file.
    /// </summary>
    GlobalUsing,

    /// <summary>
    ///  Additional code file.
    /// </summary>
    AdditionalCodeFile
}
