// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public unsafe partial class AccessibleObject
{
    /// <summary>
    ///  <see cref="AccessibleObject"/> can't derive directly from <see cref="AccessibleDispatch"/> as
    ///  <see cref="AccessibleObject"/> is public and already has a public base class.
    /// </summary>
    private class AccessibleDispatchAdapter(AccessibleObject accessibleObject) : AccessibleDispatch(accessibleObject)
    {
    }
}
