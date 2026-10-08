// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

#if NET11_0_OR_GREATER
/// <summary>
///  Specifies how WinForms presents a top-level <see cref="Form"/> while its initial appearance is
///  prepared.
/// </summary>
public enum FormRevealMode
{
    /// <summary>
    ///  The form inherits its effective reveal behavior from <see cref="Application.DefaultFormRevealMode"/>.
    ///  This is the ambient default and is never returned by <see cref="Form.FormRevealMode"/> after
    ///  resolution.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This value is the ambient sentinel: assigning it to <see cref="Form.FormRevealMode"/> clears any
    ///   local override so the value is inherited from <see cref="Application.DefaultFormRevealMode"/> again.
    ///  </para>
    /// </remarks>
    Inherit = -1,

    /// <summary>
    ///  Uses the classic WinForms form presentation behavior. The form is never cloaked.
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
