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
        [Guid("A7ABA9C1-8983-11cf-8F20-00805F2CD064")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public unsafe interface IProvideMultipleClassInfo
        {
            [PreserveSig]
            void GetClassInfo_Stub();
            // HRESULT GetClassInfo(out ITypeInfo ppTI);

            [PreserveSig]
            HRESULT GetGUID(
                GUIDKIND dwGuidKind,
                Guid* pGuid);

            [PreserveSig]
            HRESULT GetMultiTypeInfoCount(
                uint* pcti);

            [PreserveSig]
            HRESULT GetInfoOfIndex(
                uint iti,
                MULTICLASSINFO dwFlags,
                out ITypeInfo pTypeInfo,
                uint* pdwTIFlags,
                Ole32.DispatchID* pcdispidReserved,
                Guid* piidPrimary,
                Guid* piidSource);
        }
    }
}
