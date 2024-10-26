// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace Windows.Win32.Foundation;

/// <remarks>
///  <para>
///   Never convert native constants (such as <see cref="PInvokeCore.CLR_NONE"/> to <see cref="Color"/> or pass them through
///   any conversion in <see cref="Color"/>, <see cref="ColorTranslator"/>, etc. as they can change the value.
///   <see cref="COLORREF"/> is a DWORD- passing constants in native code would just pass the value as is.
///  </para>
///  <para>
///   <see href="https://learn.microsoft.com/windows/win32/gdi/colorref#">
///    Read more on https://learn.microsoft.com.
///   </see>
///  </para>
/// </remarks>
internal readonly partial struct COLORREF
{
    public static implicit operator COLORREF(Color color) => new((uint)ColorTranslator.ToWin32(color));
    public static implicit operator Color(COLORREF color) => ColorTranslator.FromWin32((int)color.Value);
    public static implicit operator COLORREF(int color) => new((uint)color);
}
