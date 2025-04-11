// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.WinForms.Test;

/// <summary>
///  Specifies the type of test file.
/// </summary>
public enum TestFileType
{
    /// <summary>
    ///  Identifies the Analyzer test code file, which contains the code to be analyzed.
    ///  A file is such, when the filename is the same as
    ///  defined in <see cref="TestFileLoader.AnalyzerTestCode"/>.
    /// </summary>
    AnalyzerTestCode,

    /// <summary>
    ///  Identifies the code-fix test code file, which contains the code to be fixed.
    ///  A file is such, when the filename is the same as
    ///  defined in <see cref="TestFileLoader.CodeFixTestCode"/>.
    /// </summary>
    CodeFixTestCode,

    /// <summary>
    ///  Identifies the fixed test code file, which is the expected output of the code-fix test.
    ///  A file is such, when the filename is the same as
    ///  defined in <see cref="TestFileLoader.CodeFixTestCode"/>.
    /// </summary>
    FixedTestCode,

    /// <summary>
    ///  Global using file.
    ///  A file is such, when the filename is the same as
    ///  defined in <see cref="TestFileLoader.GlobalUsing"/>.
    /// </summary>
    GlobalUsing,

    /// <summary>
    ///  Additional code file. This can be any file that is not identified by the other types.
    /// </summary>
    AdditionalCodeFile
}
