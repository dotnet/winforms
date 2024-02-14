// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices.ComTypes;
using Com = Windows.Win32.System.Com;

namespace System.Windows.Forms;

public unsafe partial class DataObject
{
    /// <summary>
    ///  A wrapper that responds to <see cref="IEnumFORMATETC"/> calls by
    ///  forwarding to the underlying native <see cref="Com.IEnumFORMATETC"/>.
    /// </summary>
    private class EnumFormatEtcWrapper : IEnumFORMATETC
    {
        private readonly AgileComPointer<Com.IEnumFORMATETC> _enumFormatEtc;

        public EnumFormatEtcWrapper(Com.IEnumFORMATETC* enumFormatEtc)
        {
            _enumFormatEtc = new(enumFormatEtc, takeOwnership: false);
        }

        void IEnumFORMATETC.Clone(out IEnumFORMATETC newEnum)
        {
            using var formatEtc = _enumFormatEtc.GetInterface();
            using ComScope<Com.IEnumFORMATETC> result = new(null);
            formatEtc.Value->Clone(result).ThrowOnFailure();
            newEnum = new EnumFormatEtcWrapper(result);
        }

        int IEnumFORMATETC.Next(int celt, FORMATETC[] rgelt, int[] pceltFetched)
        {
            using var formatEtc = _enumFormatEtc.GetInterface();
            fixed (int* ppceltFetched = pceltFetched)
            fixed (FORMATETC* pRgelt = rgelt)
            {
                return formatEtc.Value->Next((uint)celt, (Com.FORMATETC*)pRgelt, (uint*)ppceltFetched);
            }
        }

        int IEnumFORMATETC.Reset()
        {
            using var formatEtc = _enumFormatEtc.GetInterface();
            return formatEtc.Value->Reset();
        }

        int IEnumFORMATETC.Skip(int celt)
        {
            using var formatEtc = _enumFormatEtc.GetInterface();
            return formatEtc.Value->Skip((uint)celt);
        }
    }
}
