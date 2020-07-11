// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

/// <summary>
///  Used to abstract access to classes that contain a potentially owned handle.
/// </summary>
/// <remarks>
///  The key benefit of this is that we can keep the owning class from being
///  collected during interop calls. <see cref="HandleRef"/> wraps arbitrary
///  owners with target handles. Having this interface allows implicit use
///  of the classes (such as System.Windows.Forms.Control) that
///  meet this common pattern in interop and encourages correct alignment
///  with the proper owner.
///
///  Note that keeping objects alive is necessary ONLY when the object has
///  a finalizer that will explicitly close the handle.
/// </remarks>
internal interface IHandle
{
    public IntPtr Handle { get; }
}
