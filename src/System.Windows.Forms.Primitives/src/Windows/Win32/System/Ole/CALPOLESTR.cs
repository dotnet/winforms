﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Windows.Win32.System.Ole;
internal partial struct CALPOLESTR
{
    public unsafe string?[] ConvertAndFree()
    {
        if (cElems == 0 || pElems is null)
        {
            return Array.Empty<string>();
        }

        string?[] values = new string?[(int)cElems];
        for (int i = 0; i < cElems; i++)
        {
            values[i] = pElems[i].ToStringAndCoTaskMemFree();
        }

        return values;
    }
}
