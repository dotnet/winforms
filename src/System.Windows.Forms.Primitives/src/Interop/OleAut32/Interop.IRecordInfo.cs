// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Oleaut32
    {
        [ComImport]
        [Guid("0000002F-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public unsafe interface IRecordInfo
        {
            [PreserveSig]
            HRESULT RecordInit(
                void* pvNew);

            [PreserveSig]
            HRESULT RecordClear(
                void* pvExisting);

            [PreserveSig]
            HRESULT RecordCopy(
                void* pvExisting,
                void* pvNew);

            [PreserveSig]
            HRESULT GetGuid(
                Guid* pguid);

            [PreserveSig]
            HRESULT GetName(
                BSTR* pbstrName);

            [PreserveSig]
            HRESULT GetSize(
                uint* pcbSize);

            [PreserveSig]
            HRESULT GetTypeInfo(
                out ITypeInfo ppTypeInfo);

            [PreserveSig]
            HRESULT GetField(
                void* pvData,
                [MarshalAs(UnmanagedType.LPWStr)] out string szFieldName,
                VARIANT* pvarField);

            [PreserveSig]
            HRESULT GetFieldNoCopy(
                void* pvData,
                [MarshalAs(UnmanagedType.LPWStr)] out string szFieldName,
                VARIANT* pvarField,
                void* ppvDataCArray);

            [PreserveSig]
            HRESULT PutField(
                Ole32.INVOKEKIND wFlags,
                void* pvData,
                [MarshalAs(UnmanagedType.LPWStr)] out string szFieldName,
                VARIANT* pvarField);

            [PreserveSig]
            HRESULT PutFieldNoCopy(
                Ole32.INVOKEKIND wFlags,
                void* pvData,
                [MarshalAs(UnmanagedType.LPWStr)] out string szFieldName,
                VARIANT* pvarField);

            [PreserveSig]
            HRESULT GetFieldNames(
                uint* pcNames,
                BSTR* rgBstrNames);

            [PreserveSig]
            BOOL IsMatchingType(
                ref IRecordInfo pRecordInfo);

            [PreserveSig]
            void* RecordCreate();

            [PreserveSig]
            HRESULT RecordCreateCopy(
                void* pvSource,
                void** ppvDest);

            [PreserveSig]
            HRESULT RecordDestroy(
                void* pvRecord);
        }
    }
}
