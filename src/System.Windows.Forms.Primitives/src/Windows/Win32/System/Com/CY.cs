// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Windows.Win32.System.Com;

internal partial struct CY
{
    // https://learn.microsoft.com/openspecs/windows_protocols/ms-oaut/5a2b34c4-d109-438e-9ec8-84816d8de40d

    public static explicit operator decimal(CY value) => decimal.FromOACurrency(value.int64);
    public static explicit operator CY(decimal value) => new() { int64 = decimal.ToOACurrency(value) };

    public static explicit operator float(CY value) => (float)(value.int64 / 10000f);
    public static explicit operator CY(float value) => new() { int64 = (long)(value * 10000) };
}
