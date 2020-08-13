// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        [Flags]
        public enum CDDS : uint
        {
            PREPAINT = 0x00000001,
            POSTPAINT = 0x00000002,
            PREERASE = 0x00000003,
            POSTERASE = 0x00000004,
            ITEM = 0x00010000,
            ITEMPREPAINT = ITEM | PREPAINT,
            ITEMPOSTPAINT = ITEM | POSTPAINT,
            ITEMPREERASE = ITEM | PREERASE,
            ITEMPOSTERASE = ITEM | POSTERASE,
            SUBITEM = 0x00020000,
        }
    }
}
