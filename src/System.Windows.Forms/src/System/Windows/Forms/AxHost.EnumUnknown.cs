// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    public abstract partial class AxHost
    {
        internal class EnumUnknown : Ole32.IEnumUnknown
        {
            private readonly object[] arr;
            private int loc;
            private readonly int size;

            internal EnumUnknown(object[] arr)
            {
                //if (AxHTraceSwitch.TraceVerbose) Debug.WriteObject(arr);
                this.arr = arr;
                loc = 0;
                size = (arr is null) ? 0 : arr.Length;
            }

            private EnumUnknown(object[] arr, int loc) : this(arr)
            {
                this.loc = loc;
            }

            unsafe HRESULT Ole32.IEnumUnknown.Next(uint celt, IntPtr rgelt, uint* pceltFetched)
            {
                if (pceltFetched != null)
                {
                    *pceltFetched = 0;
                }

                if (celt < 0)
                {
                    return HRESULT.E_INVALIDARG;
                }

                uint fetched = 0;
                if (loc >= size)
                {
                    fetched = 0;
                }
                else
                {
                    for (; loc < size && fetched < celt; ++loc)
                    {
                        if (arr[loc] != null)
                        {
                            Marshal.WriteIntPtr(rgelt, Marshal.GetIUnknownForObject(arr[loc]));
                            rgelt = (IntPtr)((long)rgelt + (long)sizeof(IntPtr));
                            ++fetched;
                        }
                    }
                }

                if (pceltFetched != null)
                {
                    *pceltFetched = fetched;
                }

                if (fetched != celt)
                {
                    return HRESULT.S_FALSE;
                }

                return HRESULT.S_OK;
            }

            HRESULT Ole32.IEnumUnknown.Skip(uint celt)
            {
                loc += (int)celt;
                if (loc >= size)
                {
                    return HRESULT.S_FALSE;
                }

                return HRESULT.S_OK;
            }

            HRESULT Ole32.IEnumUnknown.Reset()
            {
                loc = 0;
                return HRESULT.S_OK;
            }

            HRESULT Ole32.IEnumUnknown.Clone(out Ole32.IEnumUnknown ppenum)
            {
                ppenum = new EnumUnknown(arr, loc);
                return HRESULT.S_OK;
            }
        }
    }
}
