// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices.ComTypes;
using Com = Windows.Win32.System.Com;

namespace System.Windows.Forms;

public unsafe partial class DataObject
{
    /// <summary>
    ///  A wrapper that responds to <see cref="IEnumSTATDATA"/> calls by
    ///  forwarding to the underlying native <see cref="Com.IEnumSTATDATA"/>.
    /// </summary>
    private class EnumStatDataWrapper : IEnumSTATDATA
    {
        private readonly AgileComPointer<Com.IEnumSTATDATA> _enumStatData;

        public EnumStatDataWrapper(Com.IEnumSTATDATA* enumStatData)
        {
            _enumStatData = new(enumStatData, takeOwnership: false);
        }

        void IEnumSTATDATA.Clone(out IEnumSTATDATA newEnum)
        {
            using var enumStatData = _enumStatData.GetInterface();
            using ComScope<Com.IEnumSTATDATA> result = new(null);
            enumStatData.Value->Clone(result).ThrowOnFailure();
            newEnum = new EnumStatDataWrapper(result);
        }

        unsafe int IEnumSTATDATA.Next(int celt, STATDATA[] rgelt, int[] pceltFetched)
        {
            if (rgelt is null || (pceltFetched is not null && pceltFetched.Length == 0))
            {
                return HRESULT.E_POINTER;
            }

            if (celt > 1 && pceltFetched is null)
            {
                return HRESULT.E_INVALIDARG;
            }

            if (celt > rgelt.Length)
            {
                if (pceltFetched is not null)
                {
                    pceltFetched[0] = 0;
                }

                return HRESULT.E_INVALIDARG;
            }

            using var enumStataData = _enumStatData.GetInterface();
            Com.STATDATA[] nativeStatData = new Com.STATDATA[rgelt.Length];
            HRESULT result;
            fixed (int* ppceltFetched = pceltFetched)
            fixed (Com.STATDATA* pNativeStatData = nativeStatData)
            {
                result = enumStataData.Value->Next((uint)celt, pNativeStatData, (uint*)ppceltFetched);

                for (int i = 0; i < *ppceltFetched; i++)
                {
                    rgelt[i] = Com.STATDATA.ConvertToRuntimeStatData(nativeStatData[i]);
                }
            }

            return result;
        }

        int IEnumSTATDATA.Reset()
        {
            using var enumStatData = _enumStatData.GetInterface();
            return enumStatData.Value->Reset();
        }

        int IEnumSTATDATA.Skip(int celt)
        {
            using var enumStatData = _enumStatData.GetInterface();
            return enumStatData.Value->Skip((uint)celt);
        }
    }
}
