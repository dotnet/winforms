// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal partial class Interop
{
    internal static partial class Oleaut32
    {
        [Flags]
        public enum MULTICLASSINFO : uint
        {
            GETTYPEINFO = 0x00000001,
            GETNUMRESERVEDDISPIDS = 0x00000002,
            GETIIDPRIMARY = 0x00000004,
            GETIIDSOURCE = 0x00000008,
        }
    }
}
