// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Ole32
    {
        public unsafe struct CALPOLESTR
        {
            public uint cElems;
            public char** pElems;

            public unsafe string?[] ConvertAndFree()
            {
                try
                {
                    if (cElems == 0 || pElems is null)
                    {
                        return Array.Empty<string>();
                    }

                    string?[] values = new string?[(int)cElems];
                    for (int i = 0; i < cElems; i++)
                    {
                        values[i] = Marshal.PtrToStringUni((IntPtr)(void*)pElems[i]);
                    }

                    return values;
                }
                finally
                {
                    Free();
                }
            }

            public unsafe void Free()
            {
                for (int i = 0; i < cElems; i++)
                {
                    Marshal.FreeCoTaskMem((IntPtr)pElems[i]);
                }

                Marshal.FreeCoTaskMem((IntPtr)pElems);
            }
        }
    }
}
