// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        [Flags]
        public enum CDIS : uint
        {
            SELECTED = 0x0001,
            GRAYED = 0x0002,
            DISABLED = 0x0004,
            CHECKED = 0x0008,
            FOCUS = 0x0010,
            DEFAULT = 0x0020,
            HOT = 0x0040,
            MARKED = 0x0080,
            INDETERMINATE = 0x0100,
            SHOWKEYBOARDCUES = 0x0200,
            NEARHOT = 0x0400,
            OTHERSIDEHOT = 0x0800,
            DROPHILITED = 0x1000,
        }
    }
}
