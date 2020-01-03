// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class Ole32
    {
        public unsafe struct FUNCDESC
        {
            public DispatchID memid;
            public IntPtr lprgscode;
            public ELEMDESC* lprgelemdescParam;
            public FUNCKIND funckind;
            public INVOKEKIND invkind;
            public CALLCONV callconv;
            public short cParams;
            public short cParamsOpt;
            public short oVft;
            public short cScodes;
            public ELEMDESC elemdescFunc;
            public FUNCFLAGS wFuncFlags;
        }
    }
}
