// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Ole32
    {
        public unsafe struct CADWORD
        {
            public uint cElems;
            public uint* pElems;

            public unsafe uint[] ConvertAndFree()
            {
                try
                {
                    return (cElems == 0 || pElems is null)
                        ? Array.Empty<uint>()
                        : new Span<uint>(pElems, (int)cElems).ToArray();
                }
                finally
                {
                    Free();
                }
            }

            public void Free() => Marshal.FreeCoTaskMem((IntPtr)pElems);
        }
    }
}
