// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

internal static partial class PInvoke
{
    // https://github.com/microsoft/win32metadata/pull/1701

    public const uint LVIM_BEFORE = 0x00000000;
    public const uint LVIM_AFTER = 0x00000001; // TODO: Remove after metadata update
}
