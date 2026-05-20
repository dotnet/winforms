// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

#if NET11_0_OR_GREATER
/// <summary>
///  Specifies how WinForms presents a form while its initial appearance is prepared.
/// </summary>
public enum FormAppearanceMode
{
    /// <summary>
    ///  Uses the classic WinForms form presentation behavior.
    /// </summary>
    Classic = 0,

    /// <summary>
    ///  Defers the initial top-level form presentation to help prevent default-background flash.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Deferral applies to the form background. Deep child-control trees can still produce visible
    ///   updates after the form is shown.
    ///  </para>
    /// </remarks>
    Deferred = 1
}
#endif
