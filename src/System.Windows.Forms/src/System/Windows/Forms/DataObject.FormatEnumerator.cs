// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices.ComTypes;
using Com = Windows.Win32.System.Com;

namespace System.Windows.Forms
{
    public partial class DataObject
    {
        /// <summary>
        ///  Part of IComDataObject, used to interop with OLE.
        /// </summary>
        private class FormatEnumerator : IEnumFORMATETC, Com.IEnumFORMATETC.Interface, Com.IManagedWrapper<Com.IEnumFORMATETC>
        {
            private IDataObject _parent;
            private readonly List<FORMATETC> _formats = new List<FORMATETC>();
            private int _current;

            public FormatEnumerator(IDataObject parent) : this(parent, parent.GetFormats())
            {
                CompModSwitches.DataObject.TraceVerbose($"FormatEnumerator: Constructed: {parent}");
            }

            private FormatEnumerator(FormatEnumerator source)
            {
                _parent = source._parent;
                _current = source._current;
                _formats.AddRange(source._formats);
            }

            public FormatEnumerator(IDataObject parent, string[]? formats)
            {
                CompModSwitches.DataObject.TraceVerbose($"FormatEnumerator: Constructed: {parent}, string[{(formats?.Length ?? 0)}]");

                _parent = parent;

                if (formats is null)
                {
                    return;
                }

                for (int i = 0; i < formats.Length; i++)
                {
                    string format = formats[i];
                    FORMATETC temp = new()
                    {
                        cfFormat = unchecked((short)(ushort)(DataFormats.GetFormat(format).Id)),
                        dwAspect = DVASPECT.DVASPECT_CONTENT,
                        ptd = 0,
                        lindex = -1,
                        tymed = format.Equals(DataFormats.Bitmap) ? TYMED.TYMED_GDI : TYMED.TYMED_HGLOBAL
                    };

                    _formats.Add(temp);
                }
            }

            public int Next(int celt, FORMATETC[] rgelt, int[]? pceltFetched)
            {
                CompModSwitches.DataObject.TraceVerbose("FormatEnumerator: Next");
                if (_current >= _formats.Count || celt <= 0)
                {
                    if (pceltFetched is not null)
                    {
                        pceltFetched[0] = 0;
                    }

                    return (int)HRESULT.S_FALSE;
                }

                FORMATETC current = _formats[_current];
                rgelt[0].cfFormat = current.cfFormat;
                rgelt[0].tymed = current.tymed;
                rgelt[0].dwAspect = DVASPECT.DVASPECT_CONTENT;
                rgelt[0].ptd = 0;
                rgelt[0].lindex = -1;

                if (pceltFetched is not null)
                {
                    pceltFetched[0] = 1;
                }

                _current++;
                return (int)HRESULT.S_OK;
            }

            public int Skip(int celt)
            {
                CompModSwitches.DataObject.TraceVerbose("FormatEnumerator: Skip");
                if (_current + celt >= _formats.Count)
                {
                    return (int)HRESULT.S_FALSE;
                }

                _current += celt;
                return (int)HRESULT.S_OK;
            }

            public int Reset()
            {
                CompModSwitches.DataObject.TraceVerbose("FormatEnumerator: Reset");
                _current = 0;
                return (int)HRESULT.S_OK;
            }

            public void Clone(out IEnumFORMATETC ppenum)
            {
                CompModSwitches.DataObject.TraceVerbose("FormatEnumerator: Clone");
                ppenum = new FormatEnumerator(this);
            }

            unsafe HRESULT Com.IEnumFORMATETC.Interface.Next(uint celt, Com.FORMATETC* rgelt, uint* pceltFetched)
            {
                if (rgelt is null)
                {
                    return HRESULT.E_POINTER;
                }

                FORMATETC[] elt = new FORMATETC[celt];
                int[] celtFetched = new int[1];

                // Eliminate null bang after https://github.com/dotnet/runtime/pull/68537 lands, or
                // IEnumFORMATETC annotations would be corrected.
                var result = ((IEnumFORMATETC)this).Next((int)celt, elt, pceltFetched is null ? null! : celtFetched);
                for (var i = 0; i < celt; i++)
                {
                    rgelt[i] = elt[i];
                }

                if (pceltFetched is not null)
                {
                    *pceltFetched = (uint)celtFetched[0];
                }

                return (HRESULT)result;
            }

            HRESULT Com.IEnumFORMATETC.Interface.Skip(uint celt) => (HRESULT)((IEnumFORMATETC)this).Skip((int)celt);

            HRESULT Com.IEnumFORMATETC.Interface.Reset() => (HRESULT)((IEnumFORMATETC)this).Reset();

            unsafe HRESULT Com.IEnumFORMATETC.Interface.Clone(Com.IEnumFORMATETC** ppenum)
            {
                if (ppenum is null)
                {
                    return HRESULT.E_POINTER;
                }

                try
                {
                    *ppenum = null;
                    ((IEnumFORMATETC)this).Clone(out var cloned);
                    if (ComHelpers.TryGetComPointer(cloned, out Com.IEnumFORMATETC* p))
                    {
                        *ppenum = p;
                        return HRESULT.S_OK;
                    }

                    Debug.Fail("Why couldn't we get a COM pointer for the cloned object?");
                    return HRESULT.E_FAIL;
                }
                catch (Exception ex)
                {
                    return ex;
                }
            }
        }
    }
}
