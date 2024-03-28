// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Com;
using Windows.Win32.System.Ole;

namespace System.Windows.Forms;

public partial class Control
{
    /// <summary>
    ///  Simple verb enumerator.
    /// </summary>
    private unsafe class ActiveXVerbEnum : IEnumOLEVERB.Interface, IManagedWrapper<IEnumOLEVERB>
    {
        private readonly OLEVERB[] _verbs;
        private uint _current;

        internal ActiveXVerbEnum(OLEVERB[] verbs)
        {
            _verbs = verbs;
            _current = 0;
        }

        public unsafe HRESULT Next(uint celt, OLEVERB* rgelt, uint* pceltFetched)
        {
            if (rgelt is null || (pceltFetched is null && celt != 1))
            {
                return HRESULT.E_POINTER;
            }

            uint fetched = 0;

            if (celt != 1)
            {
                Debug.Fail("Caller of IEnumOLEVERB requested celt > 1, but clr marshalling does not support this.");
                celt = 1;
            }

            while (celt > 0 && _current < _verbs.Length)
            {
                rgelt[fetched].lVerb = _verbs[_current].lVerb;
                rgelt[fetched].lpszVerbName = _verbs[_current].lpszVerbName;
                rgelt[fetched].fuFlags = _verbs[_current].fuFlags;
                rgelt[fetched].grfAttribs = _verbs[_current].grfAttribs;
                celt--;
                _current++;
                fetched++;
            }

            if (pceltFetched is not null)
            {
                *pceltFetched = fetched;
            }

            return celt == 0 ? HRESULT.S_OK : HRESULT.S_FALSE;
        }

        public HRESULT Skip(uint celt)
        {
            if (_current + celt < _verbs.Length)
            {
                _current += celt;
                return HRESULT.S_OK;
            }

            _current = (uint)_verbs.Length;
            return HRESULT.S_FALSE;
        }

        public HRESULT Reset()
        {
            _current = 0;
            return HRESULT.S_OK;
        }

        public HRESULT Clone(IEnumOLEVERB** ppenum)
        {
            *ppenum = ComHelpers.GetComPointer<IEnumOLEVERB>(new ActiveXVerbEnum(_verbs));
            return HRESULT.S_OK;
        }
    }
}
