// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices.ComTypes;
using Com = Windows.Win32.System.Com;

namespace System.Windows.Forms;

public unsafe partial class DataObject
{
    private sealed partial class ComDataObjectWrapper
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
#if DEBUG
                _enumStatData = new(enumStatData, takeOwnership: true, trackDisposal: false);
#else
                _dataObject = new(enumStatData, takeOwnership: true);
#endif
            }

            void IEnumSTATDATA.Clone(out IEnumSTATDATA newEnum)
            {
                using var enumStatData = _enumStatData.GetInterface();
                Com.IEnumSTATDATA* result;
                enumStatData.Value->Clone(&result).ThrowOnFailure();
                newEnum = new EnumStatDataWrapper(result);
            }

            unsafe int IEnumSTATDATA.Next(int celt, STATDATA[] rgelt, int[] pceltFetched)
            {
                using var enumStataData = _enumStatData.GetInterface();
                Com.STATDATA[] nativeStatData = new Com.STATDATA[rgelt.Length];
                for (int i = 0; i < nativeStatData.Length; i++)
                {
                    nativeStatData[i] = rgelt[i];
                }

                fixed (int* ppceltFetched = pceltFetched)
                fixed (Com.STATDATA* pNativeStatData = nativeStatData)
                {
                    return enumStataData.Value->Next((uint)celt, pNativeStatData, (uint*)ppceltFetched);
                }
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
}
