// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.WinForms.Utilities.Shared;

/// <summary>
///  Represents the source language for a file.
/// </summary>
/// <remarks>
///  <para>
///   The <see cref="SourceLanguage"/> enum is used to specify the programming
///   language of the associated file.
///  </para>
///  <para>
///   It helps analyzers or similar tools determine language-specific behaviors
///   for processing tasks.
///  </para>
/// </remarks>
public enum SourceLanguage
{
    /// <summary>
    ///  Indicates no specific language is set.
    /// </summary>
    None,

    /// <summary>
    ///  Represents the C# language.
    /// </summary>
    CSharp,

    /// <summary>
    ///  Represents the Visual Basic language.
    /// </summary>
    VisualBasic
}
