// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Com;
using Accessibility;

namespace System.Windows.Forms;

internal static unsafe class AccessibilityExtensions
{
    /// <inheritdoc cref="PInvoke.LresultFromObject(Guid*, WPARAM, IUnknown*)"/>
    internal static LRESULT GetLRESULT(this IAccessible accessible, WPARAM wparam)
    {
        // https://learn.microsoft.com/windows/win32/winauto/how-to-handle-wm-getobject

        using var unknown = ComHelpers.TryGetComScope<IUnknown>(accessible, out _);

        return PInvoke.LresultFromObject(
            IID.Get<global::Windows.Win32.UI.Accessibility.IAccessible>(),
            wparam,
            unknown);
    }
}
