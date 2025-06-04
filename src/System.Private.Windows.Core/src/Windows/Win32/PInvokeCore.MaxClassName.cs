// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

internal static partial class PInvokeCore
{
    /// <summary>
    ///  <para>
    ///   The maximum length for lpszClassName is 256. If lpszClassName is greater than the maximum length,
    ///   the RegisterClassEx function will fail.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/winuser/ns-winuser-wndclassexw#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   The maximum name for an ATOM is 255 characters, which is where this limitation comes from. There is no
    ///   public define, internally it is RTL_ATOM_MAXIMUM_NAME_LENGTH.
    ///  </para>
    /// </remarks>
    public const int MaxClassName = 256;
}
