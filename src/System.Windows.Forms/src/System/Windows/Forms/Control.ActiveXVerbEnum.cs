// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;

namespace System.Windows.Forms
{
    public partial class Control
    {
        /// <summary>
        ///  Simple verb enumerator.
        /// </summary>
        private class ActiveXVerbEnum : UnsafeNativeMethods.IEnumOLEVERB
        {
            private readonly NativeMethods.tagOLEVERB[] _verbs;
            private int _current;

            internal ActiveXVerbEnum(NativeMethods.tagOLEVERB[] verbs)
            {
                _verbs = verbs;
                _current = 0;
            }

            public int Next(int celt, NativeMethods.tagOLEVERB rgelt, int[] pceltFetched)
            {
                int fetched = 0;

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
                    pceltFetched[0] = fetched;
                }

#if DEBUG
                if (CompModSwitches.ActiveX.TraceInfo)
                {
                    Debug.WriteLine($"AxSource:IEnumOLEVERB::Next returning {fetched} verbs:");
                    Debug.Indent();
                    for (int i = _current - fetched; i < _current; i++)
                    {
                        Debug.WriteLine($"{i}: {_verbs[i].lVerb} {_verbs[i].lpszVerbName ?? string.Empty}");
                    }
                    Debug.Unindent();
                }
#endif
                return (celt == 0 ? NativeMethods.S_OK : NativeMethods.S_FALSE);
            }

            public int Skip(int celt)
            {
                if (_current + celt < _verbs.Length)
                {
                    _current += celt;
                    return NativeMethods.S_OK;
                }
                else
                {
                    _current = _verbs.Length;
                    return NativeMethods.S_FALSE;
                }
            }

            public void Reset()
            {
                _current = 0;
            }

            public void Clone(out UnsafeNativeMethods.IEnumOLEVERB ppenum)
            {
                ppenum = new ActiveXVerbEnum(_verbs);
            }
        }
    }
}
