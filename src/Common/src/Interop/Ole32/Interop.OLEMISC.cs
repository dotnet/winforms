// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal partial class Interop
{
    internal static partial class Ole32
    {
        [Flags]
        public enum OLEMISC : uint
        {
            RECOMPOSEONRESIZE = 0x1,
            ONLYICONIC = 0x2,
            INSERTNOTREPLACE = 0x4,
            STATIC = 0x8,
            CANTLINKINSIDE = 0x10,
            CANLINKBYOLE1 = 0x20,
            ISLINKOBJECT = 0x40,
            INSIDEOUT = 0x80,
            ACTIVATEWHENVISIBLE = 0x100,
            RENDERINGISDEVICEINDEPENDENT = 0x200,
            INVISIBLEATRUNTIME = 0x400,
            ALWAYSRUN = 0x800,
            ACTSLIKEBUTTON = 0x1000,
            ACTSLIKELABEL = 0x2000,
            NOUIACTIVATE = 0x4000,
            ALIGNABLE = 0x8000,
            SIMPLEFRAME = 0x10000,
            SETCLIENTSITEFIRST = 0x20000,
            IMEMODE = 0x40000,
            IGNOREACTIVATEWHENVISIBLE = 0x80000,
            WANTSTOMENUMERGE = 0x100000,
            SUPPORTSMULTILEVELUNDO = 0x200000,
        }
    }
}
