// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class Ole32
    {
        public struct TYPEATTR
        {
            public Guid guid;
            public Kernel32.LCID lcid;
            public uint dwReserved;
            public DispatchID memidConstructor;
            public DispatchID memidDestructor;
            public IntPtr lpstrSchema;
            public uint cbSizeInstance;
            public TYPEKIND typekind;
            public ushort cFuncs;
            public ushort cVars;
            public ushort cImplTypes;
            public ushort cbSizeVft;
            public ushort cbAlignment;
            public ushort wTypeFlags;
            public ushort wMajorVerNum;
            public ushort wMinorVerNum;
            public TYPEDESC tdescAlias;
            public IDLDESC idldescType;
        }
    }
}
