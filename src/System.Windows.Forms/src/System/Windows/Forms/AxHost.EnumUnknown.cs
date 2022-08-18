// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    public abstract partial class AxHost
    {
        internal class EnumUnknown : Ole32.IEnumUnknown
        {
            private readonly object[]? _array;
            private int _location;
            private readonly int _size;

            internal EnumUnknown(object[]? array)
            {
                _array = array;
                _location = 0;
                _size = (array is null) ? 0 : array.Length;
            }

            private EnumUnknown(object[]? array, int location) : this(array)
            {
                _location = location;
            }

            unsafe HRESULT Ole32.IEnumUnknown.Next(uint celt, IntPtr rgelt, uint* pceltFetched)
            {
                if (pceltFetched is not null)
                {
                    *pceltFetched = 0;
                }

                if (celt < 0)
                {
                    return HRESULT.Values.E_INVALIDARG;
                }

                uint fetched = 0;
                if (_location >= _size)
                {
                    fetched = 0;
                }
                else
                {
                    for (; _location < _size && fetched < celt; ++_location)
                    {
                        if (_array![_location] is not null)
                        {
                            Marshal.WriteIntPtr(rgelt, Marshal.GetIUnknownForObject(_array[_location]));
                            rgelt = (IntPtr)((long)rgelt + sizeof(IntPtr));
                            ++fetched;
                        }
                    }
                }

                if (pceltFetched is not null)
                {
                    *pceltFetched = fetched;
                }

                if (fetched != celt)
                {
                    return HRESULT.Values.S_FALSE;
                }

                return HRESULT.Values.S_OK;
            }

            HRESULT Ole32.IEnumUnknown.Skip(uint celt)
            {
                _location += (int)celt;
                if (_location >= _size)
                {
                    return HRESULT.Values.S_FALSE;
                }

                return HRESULT.Values.S_OK;
            }

            HRESULT Ole32.IEnumUnknown.Reset()
            {
                _location = 0;
                return HRESULT.Values.S_OK;
            }

            HRESULT Ole32.IEnumUnknown.Clone(out Ole32.IEnumUnknown ppenum)
            {
                ppenum = new EnumUnknown(_array, _location);
                return HRESULT.Values.S_OK;
            }
        }
    }
}
