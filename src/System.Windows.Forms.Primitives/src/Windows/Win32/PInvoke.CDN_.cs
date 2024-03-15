// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

#pragma warning disable format

internal static partial class PInvoke
{
    // https://github.com/microsoft/win32metadata/issues/1301

    public const uint CDN_FIRST             = unchecked(0u - 601u);
    public const uint CDN_LAST              = unchecked(0u - 699u);

    // Notifications from Open or Save dialog
    public const uint CDN_INITDONE          = unchecked(CDN_FIRST - 0x0000);
    public const uint CDN_SELCHANGE         = unchecked(CDN_FIRST - 0x0001);
    public const uint CDN_FOLDERCHANGE      = unchecked(CDN_FIRST - 0x0002);
    public const uint CDN_SHAREVIOLATION    = unchecked(CDN_FIRST - 0x0003);
    public const uint CDN_HELP              = unchecked(CDN_FIRST - 0x0004);
    public const uint CDN_FILEOK            = unchecked(CDN_FIRST - 0x0005);
    public const uint CDN_TYPECHANGE        = unchecked(CDN_FIRST - 0x0006);
    public const uint CDN_INCLUDEITEM       = unchecked(CDN_FIRST - 0x0007);
}

#pragma warning restore format
