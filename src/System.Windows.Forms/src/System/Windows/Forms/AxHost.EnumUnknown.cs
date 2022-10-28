// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.InteropServices;
using Windows.Win32.System.Com;

namespace System.Windows.Forms
{
    public abstract partial class AxHost
    {
        internal class EnumUnknown : IEnumUnknown.Interface
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

            unsafe HRESULT IEnumUnknown.Interface.Next(uint celt, IUnknown** rgelt, uint* pceltFetched)
            {
                if (rgelt is null)
                {
                    return HRESULT.E_POINTER;
                }

                if (pceltFetched is not null)
                {
                    *pceltFetched = 0;
                }

                if (celt < 0)
                {
                    return HRESULT.E_INVALIDARG;
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
                            *rgelt = (IUnknown*)Marshal.GetIUnknownForObject(_array[_location]);
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
                    return HRESULT.S_FALSE;
                }

                return HRESULT.S_OK;
            }

            HRESULT IEnumUnknown.Interface.Skip(uint celt)
            {
                _location += (int)celt;
                if (_location >= _size)
                {
                    return HRESULT.S_FALSE;
                }

                return HRESULT.S_OK;
            }

            HRESULT IEnumUnknown.Interface.Reset()
            {
                _location = 0;
                return HRESULT.S_OK;
            }

            unsafe HRESULT IEnumUnknown.Interface.Clone(IEnumUnknown** ppenum)
            {
                if (ppenum is null)
                {
                    return HRESULT.E_POINTER;
                }

                bool result = ComHelpers.TryGetComPointer(new EnumUnknown(_array, _location), out *ppenum);
                Debug.Assert(result);
                return HRESULT.S_OK;
            }
        }
    }
}
