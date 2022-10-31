// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Windows.Win32.System.Com;

// https://github.com/microsoft/win32metadata/issues/1319

/// <summary>
///  Wrapper for a VARIANT boolean type.
/// </summary>
internal enum VARIANT_BOOL : short
{
    FALSE = 0,
    TRUE = -1,
}
