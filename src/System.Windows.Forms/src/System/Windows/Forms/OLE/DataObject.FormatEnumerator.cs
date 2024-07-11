// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;
using Windows.Win32.System.Com;
using ComTypes = System.Runtime.InteropServices.ComTypes;

namespace System.Windows.Forms;

public partial class DataObject
{
    /// <summary>
    ///  Part of IComDataObject, used to interop with OLE.
    /// </summary>
    private unsafe class FormatEnumerator : ComTypes.IEnumFORMATETC, IEnumFORMATETC.Interface, IManagedWrapper<IEnumFORMATETC>
    {
        private readonly IDataObject _parent;
        private readonly List<ComTypes.FORMATETC> _formats = [];
        private int _current;

        public FormatEnumerator(IDataObject parent) : this(parent, parent.GetFormats())
        {
        }

        private FormatEnumerator(FormatEnumerator source)
        {
            _parent = source._parent;
            _current = source._current;
            _formats.AddRange(source._formats);
        }

        public FormatEnumerator(IDataObject parent, string[]? formats)
        {
            _parent = parent;

            if (formats is null)
            {
                return;
            }

            for (int i = 0; i < formats.Length; i++)
            {
                string format = formats[i];
                ComTypes.FORMATETC temp = new()
                {
                    cfFormat = (short)(ushort)DataFormats.GetFormat(format).Id,
                    dwAspect = ComTypes.DVASPECT.DVASPECT_CONTENT,
                    ptd = 0,
                    lindex = -1,
                    tymed = format.Equals(DataFormats.Bitmap) ? ComTypes.TYMED.TYMED_GDI : ComTypes.TYMED.TYMED_HGLOBAL
                };

                _formats.Add(temp);
            }
        }

        public int Next(int celt, ComTypes.FORMATETC[] rgelt, int[]? pceltFetched)
        {
            if (_current >= _formats.Count || celt <= 0)
            {
                if (pceltFetched is not null)
                {
                    pceltFetched[0] = 0;
                }

                return (int)HRESULT.S_FALSE;
            }

            ComTypes.FORMATETC current = _formats[_current];
            rgelt[0] = new()
            {
                cfFormat = current.cfFormat,
                tymed = current.tymed,
                dwAspect = ComTypes.DVASPECT.DVASPECT_CONTENT,
                ptd = 0,
                lindex = -1
            };

            if (pceltFetched is not null)
            {
                pceltFetched[0] = 1;
            }

            _current++;
            return (int)HRESULT.S_OK;
        }

        public int Skip(int celt)
        {
            if (_current + celt >= _formats.Count)
            {
                return (int)HRESULT.S_FALSE;
            }

            _current += celt;
            return (int)HRESULT.S_OK;
        }

        public int Reset()
        {
            _current = 0;
            return (int)HRESULT.S_OK;
        }

        public void Clone(out ComTypes.IEnumFORMATETC ppenum)
        {
            ppenum = new FormatEnumerator(this);
        }

        HRESULT IEnumFORMATETC.Interface.Next(uint celt, FORMATETC* rgelt, uint* pceltFetched)
        {
            if (rgelt is null)
            {
                return HRESULT.E_POINTER;
            }

            ComTypes.FORMATETC[] elt = new ComTypes.FORMATETC[celt];
            int[] celtFetched = new int[1];

            // Eliminate null bang after https://github.com/dotnet/runtime/pull/68537 lands, or
            // IEnumFORMATETC annotations would be corrected.
            int result = Next((int)celt, elt, pceltFetched is null ? null! : celtFetched);
            for (int i = 0; i < celt; i++)
            {
                rgelt[i] = Unsafe.As<ComTypes.FORMATETC, FORMATETC>(ref elt[i]);
            }

            if (pceltFetched is not null)
            {
                *pceltFetched = (uint)celtFetched[0];
            }

            return (HRESULT)result;
        }

        HRESULT IEnumFORMATETC.Interface.Skip(uint celt) => (HRESULT)Skip((int)celt);

        HRESULT IEnumFORMATETC.Interface.Reset() => (HRESULT)Reset();

        HRESULT IEnumFORMATETC.Interface.Clone(IEnumFORMATETC** ppenum)
        {
            if (ppenum is null)
            {
                return HRESULT.E_POINTER;
            }

            Clone(out var cloned);
            *ppenum = ComHelpers.GetComPointer<IEnumFORMATETC>(cloned);
            return HRESULT.S_OK;
        }
    }
}
