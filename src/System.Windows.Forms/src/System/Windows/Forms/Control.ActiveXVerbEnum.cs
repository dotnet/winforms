// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Diagnostics;
using static Interop;

namespace System.Windows.Forms
{
    public partial class Control
    {
        /// <summary>
        ///  Simple verb enumerator.
        /// </summary>
        private class ActiveXVerbEnum : Ole32.IEnumOLEVERB
        {
            private readonly Ole32.OLEVERB[] _verbs;
            private uint _current;

            internal ActiveXVerbEnum(Ole32.OLEVERB[] verbs)
            {
                _verbs = verbs;
                _current = 0;
            }

            public unsafe HRESULT Next(uint celt, Ole32.OLEVERB rgelt, uint* pceltFetched)
            {
                uint fetched = 0;

                if (celt != 1)
                {
                    Debug.Fail("Caller of IEnumOLEVERB requested celt > 1, but clr marshalling does not support this.");
                    celt = 1;
                }

                while (celt > 0 && _current < _verbs.Length)
                {
                    rgelt.lVerb = _verbs[_current].lVerb;
                    rgelt.lpszVerbName = _verbs[_current].lpszVerbName;
                    rgelt.fuFlags = _verbs[_current].fuFlags;
                    rgelt.grfAttribs = _verbs[_current].grfAttribs;
                    celt--;
                    _current++;
                    fetched++;
                }

                if (pceltFetched != null)
                {
                    *pceltFetched = fetched;
                }

#if DEBUG
                if (CompModSwitches.ActiveX.TraceInfo)
                {
                    Debug.WriteLine($"AxSource:IEnumOLEVERB::Next returning {fetched} verbs:");
                    Debug.Indent();
                    for (uint i = _current - fetched; i < _current; i++)
                    {
                        Debug.WriteLine($"{i}: {_verbs[i].lVerb} {_verbs[i].lpszVerbName ?? string.Empty}");
                    }
                    Debug.Unindent();
                }
#endif
                return (celt == 0 ? HRESULT.S_OK : HRESULT.S_FALSE);
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

            public HRESULT Clone(out Ole32.IEnumOLEVERB ppenum)
            {
                ppenum = new ActiveXVerbEnum(_verbs);
                return HRESULT.S_OK;
            }
        }
    }
}
